using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiRestDoneIt.Data;
using ApiRestDoneIt.DTOs;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Services;



namespace ApiRestDoneIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DoneItContext _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        public AuthController(DoneItContext context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }
        // post api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.nombre_usuario == dto.Usuario);

            if (usuario == null || !PasswordHelper.VerifyPassword(dto.Contrasena, usuario.salt, usuario.password_hash))
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            var token = GenerarToken(usuario);

            return Ok(new { token });
        }
        //post api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Registro([FromBody] RegisterRequestDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.nombre_usuario == dto.NombreUsuario))
                return Conflict(new { mensaje = "El nombre de usuario ya existe" });

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(dto.Contrasena, salt);

            var nuevoUsuario = new Usuario
            {
                nombre = dto.Nombre,
                apellido = dto.Apellido,
                email = dto.Email,
                nombre_usuario = dto.NombreUsuario,
                password_hash = hash,
                salt = salt,
                fecha_registro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado con éxito" });
        }
        // Metodo para crear el JWT 
        private string GenerarToken(Usuario usuario) 
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.id_usuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.nombre_usuario)
        };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // recuperar contraseña por email
        // post api/auth/recuperarCon
        [HttpPost("recuperarCon")]
        public async Task<IActionResult> RecuperarContrasena([FromBody] RecuperarConRequestDTO request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.email == request.Email);

            if (usuario != null)
            {
                usuario.token_recuperacion = Guid.NewGuid().ToString();
                usuario.vencimiento_token = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                await _emailService.EnviarCorreoRecuperacion(usuario.email, usuario.token_recuperacion);
            }

            // Siempre devolver lo mismo para evitar que se use para verificar correos existentes
            return Ok("Si el correo está registrado, se envió un mail de recuperación.");
        }
        // restablecer contraseña con token
        // post api/auth/restablecer
        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] ResetConRequestDTO request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.token_recuperacion == request.Token &&
                u.vencimiento_token > DateTime.UtcNow); // Verifica que el token no haya expirado

            if (usuario == null) return BadRequest("Token inválido o vencido");

            // Hashear y guardar nueva contraseña
            var salt = PasswordHelper.GenerateSalt(); 
            usuario.salt = salt;
            usuario.password_hash = PasswordHelper.HashPassword(request.NuevaContrasena, salt);

            // Limpiar token
            usuario.token_recuperacion = null;
            usuario.vencimiento_token = null;

            await _context.SaveChangesAsync();
            return Ok("Contraseña actualizada");
        }



    }
}