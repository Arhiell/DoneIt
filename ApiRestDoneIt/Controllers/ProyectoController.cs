using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Data;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class ProyectosController : ControllerBase
{
	private readonly DoneItContext _context;

	public ProyectosController(DoneItContext context) => _context = context;

    // GET: api/proyecto/mis-proyectos
    [HttpGet("mis-proyectos")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Proyecto>>> GetMisProyectos()
    {
        // Supongamos que guardaste el id en el JWT como claim
        var idUsuarioClaim = User.FindFirst("id_usuario")?.Value;

        if (idUsuarioClaim == null) return Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim);

        var proyectos = await _context.Proyectos
            .Where(p => p.id_usuario == idUsuario)
            .ToListAsync();

        return Ok(proyectos);
    }

    // obtener un proyecto por id con el usuario asociado
    [HttpGet("{id}")]
	public async Task<ActionResult<Proyecto>> GetProyecto(int id)
	{
        var proyecto = await _context.Proyectos.FirstOrDefaultAsync(p => p.id_proyecto == id);
        return proyecto == null ? NotFound() : proyecto;
	}
    // crear un nuevo proyecto
    [HttpPost]
	public async Task<ActionResult<Proyecto>> PostProyecto(Proyecto proyecto)
	{
		_context.Proyectos.Add(proyecto);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.id_proyecto }, proyecto);
	}
    // actualizar un proyecto existente
    [HttpPut("{id}")]
	public async Task<IActionResult> PutProyecto(int id, Proyecto proyecto)
	{
		if (id != proyecto.id_proyecto) return BadRequest();
		_context.Entry(proyecto).State = EntityState.Modified;
		await _context.SaveChangesAsync();
		return NoContent();
	}
    // actualizar datos de proyecto
    [HttpDelete("{id}")]
	public async Task<IActionResult> DeleteProyecto(int id)
	{
		var proyecto = await _context.Proyectos.FindAsync(id);
		if (proyecto == null) return NotFound();
		_context.Proyectos.Remove(proyecto);
		await _context.SaveChangesAsync();
		return NoContent();
	}
}
