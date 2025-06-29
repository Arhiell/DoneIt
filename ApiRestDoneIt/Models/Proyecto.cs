
#nullable disable
using System;
using System.Collections.Generic;

namespace ApiRestDoneIt.Models;

public partial class Proyecto
{
    public int id_proyecto { get; set; }

    public string nombre { get; set; }

    public string descripcion { get; set; }

    public DateOnly? fecha_creacion { get; set; }

    public int id_usuario { get; set; }

    public virtual Usuario id_usuarioNavigation { get; set; }

    public virtual ICollection<Tarea> tareas { get; set; } = new List<Tarea>();
}