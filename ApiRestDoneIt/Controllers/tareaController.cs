using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Data;

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
    // obtener una tarea por id con el proyecto asociado
    [HttpGet("{id}")]
    public async Task<ActionResult<Tarea>> GetTarea(int id)
    {
        var tarea = await _context.Tareas.Include(t => t.id_proyectoNavigation).FirstOrDefaultAsync(t => t.id_tarea == id);
        return tarea == null ? NotFound() : tarea;
    }
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
