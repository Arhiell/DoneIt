using System.ComponentModel.DataAnnotations;

namespace DoneIt.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar su correo o nombre de usuario")]
        [Display(Name = "Correo o Nombre de Usuario")]
        public string Identificador { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
