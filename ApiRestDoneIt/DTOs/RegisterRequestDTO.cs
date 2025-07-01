namespace ApiRestDoneIt.DTOs
{
    public class RegisterRequestDTO
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Contrasena { get; set; }
    }
}
