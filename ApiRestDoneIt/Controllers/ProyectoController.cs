using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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
		var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (idUsuarioClaim == null) return Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim);

        var proyectos = await _context.Proyectos
            .Where(p => p.id_usuario == idUsuario)
            .ToListAsync();

        return Ok(proyectos);
    }

    // obtener un proyecto por id con el usuario asociado
	// api/proyecto/
    [HttpGet("{id}")] 
	public async Task<ActionResult<Proyecto>> GetProyecto(int id)
	{
        var proyecto = await _context.Proyectos.FirstOrDefaultAsync(p => p.id_proyecto == id);
        return proyecto == null ? NotFound() : proyecto;
	}
    // crear un nuevo proyecto
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Proyecto>> PostProyecto([FromBody] Proyecto proyecto)
    {
        // Obtener el id del usuario desde el JWT
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized("No se pudo obtener el ID del usuario autenticado.");

        // Validar que el usuario exista (opcional pero recomendado)
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return NotFound("El usuario no existe.");

        // Forzar que el proyecto sea del usuario autenticado
        proyecto.id_usuario = idUsuario;
        proyecto.fecha_creacion = DateOnly.FromDateTime(DateTime.Today);

        _context.Proyectos.Add(proyecto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.id_proyecto }, proyecto);
    }
    // actualizar un proyecto existente
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutProyecto(int id, Proyecto proyecto)
    {
        if (id != proyecto.id_proyecto)
            return BadRequest("ID de proyecto no coincide");

        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized("Usuario no autenticado");

        var proyectoExistente = await _context.Proyectos.FindAsync(id);
        if (proyectoExistente == null)
            return NotFound("Proyecto no encontrado");

        if (proyectoExistente.id_usuario != idUsuario)
            return Forbid("No tienes permisos para modificar este proyecto");

        // Solo actualizás los campos permitidos:
        proyectoExistente.nombre = proyecto.nombre;
        proyectoExistente.descripcion = proyecto.descripcion;

        await _context.SaveChangesAsync();

        return NoContent();
    }
    // eliminar proyecto
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProyecto(int id)
    {
        var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            return Unauthorized("Usuario no autenticado");

        var proyecto = await _context.Proyectos.FindAsync(id);
        if (proyecto == null)
            return NotFound("Proyecto no encontrado");

        if (proyecto.id_usuario != idUsuario)
            return Forbid("No tienes permisos para eliminar este proyecto");

        _context.Proyectos.Remove(proyecto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
