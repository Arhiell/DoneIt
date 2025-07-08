using System;
using System.Collections.Generic;

namespace DoneIt.Models;

public partial class Tarea
{
    public int IdTarea { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string Estado { get; set; } = null!;

    public string Prioridad { get; set; } = null!;

    public int IdProyecto { get; set; }

    public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
}
