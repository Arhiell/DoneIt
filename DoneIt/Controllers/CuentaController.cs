using System.Net.NetworkInformation;
using DoneIt.Helpers;
using DoneIt.Models;
using DoneIt.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DoneIt.Controllers
{
    public class CuentaController : Controller
    {
        private readonly DoneItContext _context;

        public CuentaController(DoneItContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Acá verifica si el usuario o correo ya existe
            var existe = await _context.Usuarios
                .AnyAsync(u => u.Email == model.Email || u.NombreUsuario == model.NombreUsuario);

            if (existe)
            {
                ModelState.AddModelError("", "El usuario o correo ya están registrados.");
                return View(model);
            }

            // Llamo al PasswordHelper y creo el hash de la contraseña
            var hash = PasswordHelper.HashPassword(model.Password);

            // Crea un nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Email = model.Email,
                NombreUsuario = model.NombreUsuario,
                FechaNacimiento = model.FechaNacimiento,
                PasswordHash = hash,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Email == model.Identificador || u.NombreUsuario == model.Identificador);

            if (usuario == null || !PasswordHelper.VerifyPassword(model.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError("", "Email o contraseña incorrectos.");
                return View(model);
            }

            // Se guarda el nombre de usuario para controlar el mambo de sesión
            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("UsuarioLogueado", usuario.NombreUsuario);
            HttpContext.Session.SetString("NombreCompleto", $"{usuario.Nombre} {usuario.Apellido}");
            HttpContext.Session.SetString("Email", usuario.Email);
            HttpContext.Session.SetString("FechaNacimiento",
                usuario.FechaNacimiento.HasValue
                ? usuario.FechaNacimiento.Value.ToString("yyyy-MM-dd")
                : "No disponible");
            HttpContext.Session.SetString("FechaRegistro",
                usuario.FechaRegistro.HasValue
                    ? usuario.FechaRegistro.Value.ToString("yyyy-MM-dd")
                    : "No disponible");

            // Redirigimos a la página principal o menú (por ahora Home/Index)
            return RedirectToAction("Index", "Home");
        }

        // Zona de recuperación de password
        [HttpGet]
        public IActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarPassword(RecuperarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (usuario == null)
            {
                ModelState.AddModelError("", "No existe un usuario con ese email.");
                return View(model);
            }

            var token = Guid.NewGuid().ToString();
            usuario.TokenRecuperacion = token;
            usuario.VencimientoToken = DateTime.Now.AddHours(1);
            await _context.SaveChangesAsync();

            var urlRecuperacion = Url.Action("RestablecerPassword", "Cuenta", new { token }, Request.Scheme);
            var mensaje = $"Hacé clic en este enlace para restablecer tu contraseña: {urlRecuperacion}";

            await EmailHelper.EnviarCorreo(usuario.Email, "Recuperación de contraseña", mensaje);

            TempData["Mensaje"] = "Se envió un enlace de recuperación a tu correo.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> RestablecerPassword(string token)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenRecuperacion == token);
            if (usuario == null || usuario.VencimientoToken < DateTime.Now)
            {
                TempData["Error"] = "Token inválido o expirado.";
                return RedirectToAction("Login");
            }

            var model = new RestablecerPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerPassword(RestablecerPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenRecuperacion == model.Token);
            if (usuario == null || usuario.VencimientoToken < DateTime.Now)
            {
                TempData["Error"] = "Token inválido o expirado.";
                return RedirectToAction("Login");
            }

            usuario.PasswordHash = PasswordHelper.HashPassword(model.NuevaPassword);
            usuario.TokenRecuperacion = null;
            usuario.VencimientoToken = null;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Tu contraseña ha sido restablecida correctamente.";
            return RedirectToAction("Login");
        }
    }
}