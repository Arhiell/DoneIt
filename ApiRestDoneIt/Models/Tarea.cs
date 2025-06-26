public class Tarea
{
    public int IdTarea { get; set; }
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public string Estado { get; set; }     // 'Pendiente', 'En Proceso', 'Finalizado'
    public string Prioridad { get; set; }  // 'Bajo', 'Medio', 'Alto'

    public int IdProyecto { get; set; }
    public Proyecto Proyecto { get; set; }
}
