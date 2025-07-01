using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestDoneIt.Models;
using ApiRestDoneIt.Data;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly DoneItContext _context;

    public UsuariosController(DoneItContext context) => _context = context;
    // obtener todos los usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        => await _context.Usuarios.ToListAsync();
    // obtener un usuario por id
    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        return usuario == null ? NotFound() : usuario;
    }
    // crear un nuevo usuario
    [HttpPost]
    public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario) 
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.id_usuario }, usuario);
    }
    // actualizar un usuario existente
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(int id, Usuario usuario) 
    {
        if (id != usuario.id_usuario) return BadRequest();

        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    // actualizar datos de usuario
    [HttpPut("modificar")]
    public async Task<IActionResult> PutUsuario(Usuario usuario)
    {
        if (usuario == null) return BadRequest("Usuario no puede ser nulo");
        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    // eliminar un usuario por id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
