using System.ComponentModel.DataAnnotations;

namespace DoneIt.ViewModels
{
    public class RegistroViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmarPassword { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}
