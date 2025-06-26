public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string NombreUsuario { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public string? TokenRecuperacion { get; set; }
    public DateTime? VencimientoToken { get; set; }

    public ICollection<Proyecto> Proyectos { get; set; }
}
