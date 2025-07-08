using System;
using System.Collections.Generic;

namespace DoneIt.Models.ViewModels
{
    public class ProyectoConTareasViewModel
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<TareaViewModel> Tareas { get; set; } = new();
    }

    public class TareaViewModel
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
    }
}