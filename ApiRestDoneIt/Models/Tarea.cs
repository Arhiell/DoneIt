
#nullable disable
using System;
using System.Collections.Generic;

namespace ApiRestDoneIt.Models;

public partial class Tarea
{
    public int id_tarea { get; set; }

    public string titulo { get; set; }

    public string descripcion { get; set; }

    public DateTime? fecha_inicio { get; set; }

    public DateTime? fecha_fin { get; set; }

    public string estado { get; set; }

    public string prioridad { get; set; }

    public int id_proyecto { get; set; }

    public virtual Proyecto id_proyectoNavigation { get; set; }
}