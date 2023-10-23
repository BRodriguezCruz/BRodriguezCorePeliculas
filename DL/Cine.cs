using System;
using System.Collections.Generic;

namespace DL;

public partial class Cine
{
    public int IdCine { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public double? Ventas { get; set; }

    public int IdZona { get; set; }

    public virtual Zona IdZonaNavigation { get; set; } = null!;

    //ALIAS EN LA BD
    public string NombreZona { get; set; }
}
