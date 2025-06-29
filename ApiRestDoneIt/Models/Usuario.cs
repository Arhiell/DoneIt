
#nullable disable
using System;
using System.Collections.Generic;

namespace ApiRestDoneIt.Models;

public partial class Usuario
{
    public int id_usuario { get; set; }

    public string nombre { get; set; }

    public string apellido { get; set; }

    public string email { get; set; }

    public DateOnly? fecha_nacimiento { get; set; }

    public string nombre_usuario { get; set; }

    public string password_hash { get; set; }

    public string salt { get; set; }

    public DateTime? fecha_registro { get; set; }

    public string token_recuperacion { get; set; }

    public DateTime? vencimiento_token { get; set; }

    public virtual ICollection<Proyecto> proyectos { get; set; } = new List<Proyecto>();
}