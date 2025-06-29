using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiRestDoneIt.Data;
using ApiRestDoneIt.DTOs;
using ApiRestDoneIt.Models;

namespace ApiRestDoneIt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DoneItContext _context;
        private readonly IConfiguration _config;

        public AuthController(DoneItContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            // Validar que no exista el email o nombre de usuario
            if (await _context.Usuarios.AnyAsync(u => u.email == dto.Email || u.nombre_usuario == dto.NombreUsuario))
                return BadRequest(new { mensaje = "Email o nombre de usuario ya en uso." });

            // Generar hash y salt
            CrearHashContrasena(dto.Contrasena, out byte[] hash, out byte[] salt);

            // Mapear a la entidad
            var usuario = new Usuario
            {
                nombre = dto.Nombre,
                apellido = dto.Apellido,
                email = dto.Email,
                fecha_nacimiento = dto.FechaNacimiento,
                nombre_usuario = dto.NombreUsuario,
                password_hash = Convert.ToBase64String(hash),
                salt = Convert.ToBase64String(salt),
                fecha_registro = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(null, new { id = usuario.id_usuario }, new { usuario.id_usuario, usuario.nombre_usuario });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.nombre_usuario == dto.Usuario);
            if (user == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            // Convertir de Base64 a byte[]
            var storedHash = Convert.FromBase64String(user.password_hash);
            var storedSalt = Convert.FromBase64String(user.salt);

            if (!VerificarContrasena(dto.Contrasena, storedHash, storedSalt))
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            // Crear claims y token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id_usuario.ToString()),
                new Claim(ClaimTypes.Name, user.nombre_usuario)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // === Métodos privados para hash/salt ===

        private void CrearHashContrasena(string contrasena, out byte[] hash, out byte[] salt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
        }

        private bool VerificarContrasena(string contrasenaIngresada, byte[] hashGuardado, byte[] salt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
            var hashCalculado = hmac.ComputeHash(Encoding.UTF8.GetBytes(contrasenaIngresada));
            return hashCalculado.SequenceEqual(hashGuardado);
        }
    }
}