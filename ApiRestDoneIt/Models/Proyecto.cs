public class Proyecto
{
    public int IdProyecto { get; set; }
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public DateTime? FechaCreacion { get; set; }

    public int IdUsuario { get; set; }
    public Usuario Usuario { get; set; }

    public ICollection<Tarea> Tareas { get; set; }
}
