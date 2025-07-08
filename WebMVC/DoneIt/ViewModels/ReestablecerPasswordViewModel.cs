using System.ComponentModel.DataAnnotations;

// ViewMdoel para reestablecer la contraseña
namespace DoneIt.ViewModels
{
    public class RestablecerPasswordViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string NuevaPassword { get; set; }
    }
}