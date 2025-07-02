using DoneIt.Helpers;      
using DoneIt.Models;        
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DoneIt.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly DoneItContext _context;
        public UsuarioController(DoneItContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Perfil()
        {
            // Si no hay sesión, redirigí al login
            if (HttpContext.Session.GetString("UsuarioLogueado") == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(string NombreUsuario, string Email, string PasswordAntigua, string PasswordNueva)
        {
            var usuarioLogueado = HttpContext.Session.GetString("UsuarioLogueado");
            if (usuarioLogueado == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == usuarioLogueado);
            if (usuario == null)
                return RedirectToAction("Login", "Cuenta");

            bool huboCambios = false;

            // Actualizar nombre de usuario si no está vacío y distinto
            if (!string.IsNullOrWhiteSpace(NombreUsuario) && NombreUsuario != usuario.NombreUsuario)
            {
                // Podrías agregar validaciones para evitar duplicados
                usuario.NombreUsuario = NombreUsuario;
                HttpContext.Session.SetString("UsuarioLogueado", NombreUsuario);
                huboCambios = true;
            }

            // Actualizar email si no está vacío y distinto
            if (!string.IsNullOrWhiteSpace(Email) && Email != usuario.Email)
            {
                // Validar formato y evitar duplicados idealmente
                usuario.Email = Email;
                HttpContext.Session.SetString("Email", Email);
                huboCambios = true;
            }

            // Cambiar contraseña solo si PasswordAntigua y PasswordNueva están completos y válidos
            if (!string.IsNullOrEmpty(PasswordAntigua) && !string.IsNullOrEmpty(PasswordNueva))
            {
                // Verificar contraseña antigua
                if (PasswordHelper.VerifyPassword(PasswordAntigua, usuario.PasswordHash))
                {
                    // Actualizar hash con la nueva contraseña
                    usuario.PasswordHash = PasswordHelper.HashPassword(PasswordNueva);
                    huboCambios = true;
                }
                else
                {
                    ModelState.AddModelError("", "La contraseña actual es incorrecta.");
                    return RedirectToAction("Perfil", "Home");
                }
            }
            else if (!string.IsNullOrEmpty(PasswordAntigua) || !string.IsNullOrEmpty(PasswordNueva))
            {
                ModelState.AddModelError("", "Para cambiar la contraseña debe ingresar ambas, la actual y la nueva.");
                return RedirectToAction("Perfil", "Home");
            }

            if (huboCambios)
            {
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Datos actualizados correctamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se modificó ningún dato.";
            }

            return RedirectToAction("Perfil", "Home");
        }
    }
}
