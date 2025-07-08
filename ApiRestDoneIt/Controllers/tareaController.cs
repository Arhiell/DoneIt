using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Data;
using ApiRestDoneIt.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


[Route("api/[controller]")]
[ApiController]
public class TareasController : ControllerBase
{
    private readonly DoneItContext _context;

    public TareasController(DoneItContext context) => _context = context;
    // obtener todas las tareas con el proyecto asociado
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas()
        => await _context.Tareas.Include(t => t.id_proyectoNavigation).ToListAsync();


    // GET: api/tarea/id
    // obtener una tarea por id con el proyecto asociado
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTarea(int id)
    {
        var tarea = await _context.Tareas
            .Include(t => t.id_proyectoNavigation)
            .FirstOrDefaultAsync(t => t.id_tarea == id);

        if (tarea == null) return NotFound();

        var tareaDTO = new TareaDTO
        {
            id_tarea = tarea.id_tarea,
            titulo = tarea.titulo,
            descripcion = tarea.descripcion,
            fecha_inicio = tarea.fecha_inicio,
            fecha_fin = tarea.fecha_fin,
            estado = tarea.estado,
            prioridad = tarea.prioridad,
            Proyecto = new ProyectoDTO
            {
                id_proyecto = tarea.id_proyectoNavigation.id_proyecto,
                nombre = tarea.id_proyectoNavigation.nombre
            }
        };

        return Ok(tareaDTO);
    }

    // GET: api/Tareas/lista
    // obtener tareas paginadas con el proyecto asociado
    [HttpGet("lista")]
    public async Task<IActionResult> GetTareas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var total = await _context.Tareas.CountAsync();
        var tareas = await _context.Tareas
            .Include(t => t.id_proyectoNavigation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var tareasDTO = tareas.Select(t => new TareaDTO
        {
            id_tarea = t.id_tarea,
            titulo = t.titulo,
            descripcion = t.descripcion,
            fecha_inicio = t.fecha_inicio,
            fecha_fin = t.fecha_fin,
            estado = t.estado,
            prioridad = t.prioridad,
            Proyecto = new ProyectoDTO
            {
                id_proyecto = t.id_proyectoNavigation.id_proyecto,
                nombre = t.id_proyectoNavigation.nombre
            }
        });

        return Ok(new { total, tareas = tareasDTO });
    }
    // obtener tareas por id de proyecto
    // GET: api/Tareas/proyecto/idproyecto
    [HttpGet("proyecto/{idProyecto}")]
    [Authorize]
    public async Task<IActionResult> GetTareasPorProyecto(int idProyecto)
    {
        // Obtener id del usuario autenticado
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized("No se pudo determinar el usuario autenticado.");

        // Verificar que el proyecto existe y pertenece al usuario
        var proyecto = await _context.Proyectos.FindAsync(idProyecto);
        if (proyecto == null)
            return NotFound("Proyecto no encontrado.");

        if (proyecto.id_usuario != idUsuario)
            return Forbid("No tienes permiso para ver tareas de este proyecto.");

        var tareas = await _context.Tareas
            .Where(t => t.id_proyecto == idProyecto)
            .ToListAsync();

        return Ok(tareas);
    }

    // post/api/Tareas
    // crear una nueva tarea
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Tarea>> PostTarea([FromBody] Tarea tarea)
    {
        if (tarea == null)
            return BadRequest("Tarea no válida.");

        // Obtener id del usuario autenticado
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized("No se pudo determinar el usuario autenticado.");

        // Validar que el proyecto existe y le pertenece al usuario autenticado
        var proyecto = await _context.Proyectos.FindAsync(tarea.id_proyecto);

        if (proyecto == null)
            return NotFound("El proyecto no existe.");

        if (proyecto.id_usuario != idUsuario)
            return Forbid("No tienes permiso para agregar tareas a este proyecto.");

        _context.Tareas.Add(tarea);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTarea), new { id = tarea.id_tarea }, tarea);
    }

 
    // actualizar una tarea existente
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutTarea(int id, Tarea tarea)
    {
        if (id != tarea.id_tarea) return BadRequest();

        // Obtener usuario autenticado
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized();

        // Verificar que la tarea existe
        var tareaExistente = await _context.Tareas
            .Include(t => t.id_proyectoNavigation) // incluir proyecto para validar dueño
            .FirstOrDefaultAsync(t => t.id_tarea == id); // buscar por id

        if (tareaExistente == null)
            return NotFound("Tarea no encontrada.");

        if (tareaExistente.id_proyectoNavigation.id_usuario != idUsuario) // verificar que el proyecto le pertenece al usuario autenticado
            return Forbid("No puedes modificar esta tarea.");

        // Actualizar campos permitidos
        tareaExistente.descripcion = tarea.descripcion;
        tareaExistente.fecha_inicio = tarea.fecha_inicio;
        tareaExistente.fecha_fin = tarea.fecha_fin;
        tareaExistente.estado = tarea.estado;
        tareaExistente.prioridad = tarea.prioridad;

        await _context.SaveChangesAsync();
        return NoContent();
    }


    // eliminar una tarea por id
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTarea(int id)
    {
        var tarea = await _context.Tareas
            .Include(t => t.id_proyectoNavigation)
            .FirstOrDefaultAsync(t => t.id_tarea == id);

        if (tarea == null)
            return NotFound();

        // Validar que el usuario autenticado es el dueño del proyecto
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized();

        if (tarea.id_proyectoNavigation.id_usuario != idUsuario)
            return Forbid("No puedes eliminar esta tarea.");

        _context.Tareas.Remove(tarea);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}
