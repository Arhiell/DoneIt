using DoneIt.Models;
using DoneIt.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DoneIt.Controllers
{
    public class ProyectoTareaController : Controller
    {
        private readonly DoneItContext _context;

        public ProyectoTareaController(DoneItContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProyectoConTareasViewModel model)
        {
            try
            {
                var nombreUsuario = HttpContext.Session.GetString("UsuarioLogueado");
                if (string.IsNullOrEmpty(nombreUsuario))
                    return BadRequest("Sesión expirada.");

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
                if (usuario == null)
                    return BadRequest("Usuario no encontrado.");

                var proyecto = new Proyecto
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    FechaCreacion = DateTime.Now,
                    IdUsuario = usuario.IdUsuario
                };

                _context.Proyectos.Add(proyecto);
                await _context.SaveChangesAsync();

                foreach (var t in model.Tareas)
                {
                    var tarea = new Tarea
                    {
                        Titulo = t.Titulo,
                        Descripcion = t.Descripcion,
                        FechaInicio = t.FechaInicio,
                        FechaFin = t.FechaFin,
                        Estado = t.Estado,
                        Prioridad = t.Prioridad,
                        IdProyecto = proyecto.IdProyecto
                    };

                    _context.Tareas.Add(tarea);
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Proyecto y tareas creados con éxito." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR EN /ProyectoTarea/Crear:");
                Console.WriteLine(ex.ToString());                   

                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
    }
}
