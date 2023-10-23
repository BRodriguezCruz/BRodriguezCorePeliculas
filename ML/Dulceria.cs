using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class Dulceria
    {
        public int IdDulceria { get; set; }
        public string Nombre { get; set; }
        public decimal? Precio { get; set; }
        public string Imagen { get; set; }

        //propiedad para guardar las cantidad de porductos seleccionados en el carrito 
        public int Cantidad { get; set; }

        //lista para guardar los productos en el modelo y desempaquetarlos en la vista
        public List<object> Productos { get; set; }
    }
}
