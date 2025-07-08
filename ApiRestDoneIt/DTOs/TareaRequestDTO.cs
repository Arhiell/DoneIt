namespace ApiRestDoneIt.DTOs
{
    public class TareaDTO
    {
        public int id_tarea { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public DateTime? fecha_inicio { get; set; }
        public DateTime? fecha_fin { get; set; }
        public string estado { get; set; }
        public string prioridad { get; set; }

        // Simplificamos la info del proyecto
        public ProyectoDTO Proyecto { get; set; }
    }
}