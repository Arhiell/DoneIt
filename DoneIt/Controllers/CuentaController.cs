using DoneIt.Helpers;
using DoneIt.Models;
using DoneIt.ViewModels;
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
                Salt = "", //No se usa porque el salt está dentro del hash [cambiar en la bd (borrar a la mierda el campo salt)] 
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
            HttpContext.Session.SetString("UsuarioLogueado", usuario.NombreUsuario);

            // Redirigimos a la página principal o menú (por ahora Home/Index)
            return RedirectToAction("Index", "Home");
        }
    }
}
