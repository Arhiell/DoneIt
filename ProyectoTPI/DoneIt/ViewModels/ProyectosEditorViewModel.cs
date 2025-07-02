using DoneIt.Models;
namespace DoneIt.ViewModels
{
    public class ProyectosEditorViewModel
    {
        public List<Proyecto> ProyectosRecientes { get; set; } = new();
        public List<Proyecto> ProyectosRestantes { get; set; } = new();

        // Agrega esta propiedad para el id del usuario
        public int IdUsuario { get; set; }
    }
}
