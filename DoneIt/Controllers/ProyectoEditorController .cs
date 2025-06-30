using DoneIt.Models;
using DoneIt.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;  // para Select, Take, Skip
using System.Collections.Generic; // para List<T>

namespace DoneIt.Controllers
{
    public class ProyectoEditorController : Controller
    {
        private readonly DoneItContext _context;

        public ProyectoEditorController(DoneItContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Editor()
        {
            var nombreUsuario = HttpContext.Session.GetString("UsuarioLogueado");
            if (string.IsNullOrEmpty(nombreUsuario))
                return RedirectToAction("Login", "Cuenta");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
            if (usuario == null)
                return RedirectToAction("Login", "Cuenta");

            var idUsuario = usuario.IdUsuario;

            var proyectos = await _context.Proyectos
                .Where(p => p.IdUsuario == idUsuario)
                .Include(p => p.Tareas)
                .OrderByDescending(p => p.FechaCreacion ?? System.DateTime.MinValue)
                .ToListAsync();

            Console.WriteLine($"IdUsuario: {idUsuario}");
            foreach (var proyecto in proyectos)
            {
                Console.WriteLine($"Proyecto: {proyecto.IdProyecto} - {proyecto.Nombre} - Tareas: {proyecto.Tareas.Count}");
            }

            var vista = new ProyectosEditorViewModel
            {
                ProyectosRecientes = proyectos.Take(3).ToList(),
                ProyectosRestantes = proyectos.Skip(3).ToList(),
                IdUsuario = idUsuario
            };

            return View("~/Views/Home/Proyecto.cshtml", vista);
        }

        [HttpPost]
        public async Task<IActionResult> EditarProyecto(Proyecto proyecto)
        {
            var proyectoDb = await _context.Proyectos.FindAsync(proyecto.IdProyecto);
            if (proyectoDb == null) return NotFound();

            proyectoDb.Nombre = proyecto.Nombre;
            proyectoDb.Descripcion = proyecto.Descripcion;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Proyecto actualizado con éxito.";
            return RedirectToAction("Editor");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarProyecto(int id)
        {

            var proyecto = await _context.Proyectos.Include(p => p.Tareas)
                .FirstOrDefaultAsync(p => p.IdProyecto == id);
            if (proyecto == null) return NotFound();

            _context.Tareas.RemoveRange(proyecto.Tareas);
            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Proyecto y sus tareas eliminados.";
            return RedirectToAction("Editor");
        }

        [HttpPost]
        public async Task<IActionResult> EditarTarea(Tarea tarea)
        {
            var tareaDb = await _context.Tareas.FindAsync(tarea.IdTarea);
            if (tareaDb == null) return NotFound();

            tareaDb.Titulo = tarea.Titulo;
            tareaDb.Descripcion = tarea.Descripcion;
            tareaDb.FechaInicio = tarea.FechaInicio;
            tareaDb.FechaFin = tarea.FechaFin;
            tareaDb.Estado = tarea.Estado;
            tareaDb.Prioridad = tarea.Prioridad;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Tarea actualizada.";
            return RedirectToAction("Editor");
        }


        [HttpPost]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            Console.WriteLine($"🧨 Entró a EliminarTarea con ID: {id}");

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                return NotFound();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Tarea eliminada.";
            return RedirectToAction("Editor");
        }
        }
    }

