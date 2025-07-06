using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Data;
using ApiRestDoneIt.DTOs;

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

    // GET: api/tarea/lista
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
    // GET: api/tarea/proyecto/idproyecto
    [HttpGet("proyecto/{idProyecto}")]
    public async Task<IActionResult> GetTareasPorProyecto(int idProyecto)
    {
        var tareas = await _context.Tareas
            .Where(t => t.id_proyecto == idProyecto)
            .ToListAsync();

        return Ok(tareas);
    }
    // post/api/tarea
    // crear una nueva tarea
    [HttpPost]
    public async Task<ActionResult<Tarea>> PostTarea(Tarea tarea)
    {
        if (tarea is null)
        {
            throw new ArgumentNullException(nameof(tarea));
        }

        _context.Tareas.Add(tarea);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTarea), new { id = tarea.id_tarea }, tarea);
    }
    // actualizar una tarea existente
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTarea(int id, Tarea tarea)
    {
        if (id != tarea.id_tarea) return BadRequest();
        _context.Entry(tarea).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    // eliminar una tarea por id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTarea(int id)
    {
        var tarea = await _context.Tareas.FindAsync(id);
        if (tarea == null) return NotFound();
        _context.Tareas.Remove(tarea);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}
