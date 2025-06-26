using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ProyectosController : ControllerBase
{
	private readonly DoneItContext _context;

	public ProyectosController(DoneItContext context) => _context = context;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Proyecto>>> GetProyectos()
		=> await _context.Proyectos.Include(p => p.Usuario).ToListAsync();

	[HttpGet("{id}")]
	public async Task<ActionResult<Proyecto>> GetProyecto(int id)
	{
		var proyecto = await _context.Proyectos.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.IdProyecto == id);
		return proyecto == null ? NotFound() : proyecto;
	}

	[HttpPost]
	public async Task<ActionResult<Proyecto>> PostProyecto(Proyecto proyecto)
	{
		_context.Proyectos.Add(proyecto);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.IdProyecto }, proyecto);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> PutProyecto(int id, Proyecto proyecto)
	{
		if (id != proyecto.IdProyecto) return BadRequest();
		_context.Entry(proyecto).State = EntityState.Modified;
		await _context.SaveChangesAsync();
		return NoContent();
	}

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
