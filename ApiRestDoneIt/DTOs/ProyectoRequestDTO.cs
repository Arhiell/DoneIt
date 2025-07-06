namespace ApiRestDoneIt.DTOs
{
    public class ProyectoDTO
    {
        public int id_proyecto { get; set; }
        public string nombre { get; set; }
        public DateTime fecha_creacion { get; set; }
        public int id_usuario { get; set; }
    }
}