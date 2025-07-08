using System.ComponentModel.DataAnnotations;

//ViewModel para la recuperación de contraseña
namespace DoneIt.ViewModels
{
    public class RecuperarPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }
    }
}
