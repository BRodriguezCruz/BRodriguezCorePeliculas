using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace PLMVC.Controllers
{
    public class DulceriaController : Controller
    {

        /*
        [HttpGet]
        public IActionResult GetAll()
        {
            ML.Dulceria dulceria = new ML.Dulceria();
            ML.Result result = BL.Dulceria.GetAll();

            if (result.Correct)
            {
                dulceria.Productos = result.Objects;
            }
            return View(dulceria);
        }
        */

        [HttpGet]
        public IActionResult GetAll()
        {
            ML.Dulceria dulceria = new ML.Dulceria();
            dulceria.Productos = new List<object>(); // inicializar para agregar mas abajo los productos

            using (HttpClient cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("http://localhost:5045/api/");
                var responseTask = cliente.GetAsync("dulceria");
                responseTask.Wait();

                var resultadoServicio = responseTask.Result;

                if (resultadoServicio.IsSuccessStatusCode)
                {
                    var leerTarea = resultadoServicio.Content.ReadAsAsync<ML.Result>();
                    leerTarea.Wait();

                    foreach (var resultDulceria in leerTarea.Result.Objects)
                    {
                        ML.Dulceria resultItemList = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Dulceria>(resultDulceria.ToString());
                        dulceria.Productos.Add(resultItemList);
                    }
                }
            }
            return View(dulceria);
        }

        public IActionResult AddCarrito(int? idProducto, bool limpiar)
        {
            if (limpiar == true)
            {
                HttpContext.Session.Remove("Carrito"); //elimina los datos del carrito limpiando la session
            }
            else
            {
                bool existe = false; //bandera usada para validacion mas abajo
                ML.Venta carrito = new ML.Venta(); //instancia de objeto para acceder a props en venta mas abajo
                carrito.Carrito = new List<object>();

                ML.Result result = BL.Dulceria.GetById(idProducto.Value); //consumimos el metodo del bl para traer el producto y sus datos

                if (HttpContext.Session.GetString("Carrito") == null) //validamos si la session existe, ya que si es falso, significa que la crearemos para 
                {                                                    //almacenar los datos de forma temporal pero que persistan en el carrito 
                    if (result.Correct) //validamos el GetById
                    {
                        ML.Dulceria dulceria = (ML.Dulceria)result.Object; // pasamos el objeto en result a dulceria
                        dulceria.Cantidad = 1; //igualamos a 1 ya que si no existe y se crea la session, se creo pq si o si ya habrá al menos un producto en la session
                        carrito.Carrito.Add(dulceria);//agregamos el objeto de dulceria a una lista de objetos para el carrito

                        //Creamos la SESSION, para guardar lista de objetos CARRITO y a su vez Serializamos lista de objetos "Carrito" ya que las sessiones en NET CORE solo aceptan datos primitivos
                        HttpContext.Session.SetString("Carrito", Newtonsoft.Json.JsonConvert.SerializeObject(carrito.Carrito));
                    }
                }
                else
                {
                    ML.Dulceria dulceria = (ML.Dulceria)result.Object;
                    Deserealizar(carrito); // recupera el carrito que ya estaba y desereaiza

                    foreach (ML.Dulceria dulceria1 in carrito.Carrito)
                    {
                        if (dulceria.IdDulceria == dulceria1.IdDulceria)
                        {
                            dulceria1.Cantidad += 1;
                            existe = true;
                            break; //rompemos el ciclo si encuentra coincidencia en el ID pues si ya lo encontro ya sumo la cantidad, no hay necesidad de seguri buscando
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    if (existe == true)
                    {
                        HttpContext.Session.SetString("Carrito", Newtonsoft.Json.JsonConvert.SerializeObject(carrito.Carrito)); //se agrego la cantidad y se volvio a serializar con esa  nueva cantidad
                                                                                                                                //Esto se hace para aumentar SOLO LA CANTIDAD Dde un producto ya agregado en el carrito
                    }
                    else
                    {
                        dulceria.Cantidad = 1; // se crea nuevamente la session con un producto de dulceria nuevo agregado al carrito (lo cual significa una columna o card etc... MÁS. 
                        carrito.Carrito.Add(dulceria);
                        HttpContext.Session.SetString("Carrito", Newtonsoft.Json.JsonConvert.SerializeObject(carrito.Carrito));
                    }
                }

            }
            return RedirectToAction("GetAll"); //nombre del metodo al cual se va a redirigir
        }

        public ML.Venta Deserealizar(ML.Venta carrito)
        {
            var ventaSession = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(HttpContext.Session.GetString("Carrito"));

            foreach (var registro in ventaSession)
            {
                ML.Dulceria articulo = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Dulceria>(registro.ToString());
                carrito.Carrito.Add(articulo);
            }
            return carrito;
        }

        public IActionResult Carrito()
        {
            ML.Venta carrito = new ML.Venta();
            carrito.Carrito = new List<object>();

            if (HttpContext.Session.GetString("Carrito") == null)
            {
                return View(carrito);
            }
            else
            {
                Deserealizar(carrito);
                return View(carrito);
            }

        }
        public IActionResult AddorDeleteAmount(int idProducto, bool aumentarOdisminuir)
        {
            bool existe = false; //bandera usada para validacion mas abajo
            ML.Venta carrito = new ML.Venta(); //instancia de objeto para acceder a props en venta mas abajo
            carrito.Carrito = new List<object>();
            ML.Dulceria dulceria = new ML.Dulceria();

            ML.Result result = BL.Dulceria.GetById(idProducto); //consumimos el metodo del bl para traer el producto y sus datos

            if (result.Correct) //validamos el GetById
            {
                dulceria = (ML.Dulceria)result.Object;
                Deserealizar(carrito); // recupera el carrito que ya estaba y desereaiza

                foreach (ML.Dulceria dulceria1 in carrito.Carrito)
                {
                    if (dulceria.IdDulceria == dulceria1.IdDulceria)
                    {
                        if (aumentarOdisminuir)
                        {
                            dulceria1.Cantidad += 1;
                            existe = true;
                            break; //rompemos el ciclo si encuentra coincidencia en el ID pues si ya lo encontro ya sumo la cantidad, no hay necesidad de seguri buscando

                        }
                        else
                        {
                            dulceria1.Cantidad -= 1;
                            existe = true;

                            if (dulceria1.Cantidad == 0)
                            {
                                carrito.Carrito.Remove(dulceria1);
                            }
                            break; //rompemos el ciclo si encuentra coincidencia en el ID pues si ya lo encontro ya sumo la cantidad, no hay necesidad de seguri buscando
                        }
                    }
                }
                if (existe == true)
                {
                    /*if (cantCero == true)
                     {
                        //carrito.Carrito.RemoveAll(registro => registro.Cantidad == 0);
                        for (int i = carrito.Carrito.Count - 1; i >= 0; i--)
                        {
                            if (carrito.Carrito[i].Cantidad == 0)
                            {
                                carrito.Carrito.Remove(i);
                            }
                        }*/
                    /*
                    List<ML.Dulceria> copiaLista = new List<ML.Dulceria>(carrito.Carrito);

                    foreach (ML.Dulceria registro in copiaLista)
                    {
                        if (registro.Cantidad == 0)
                        {
                            carrito.Carrito.Remove(registro);
                        }
                    }*/
                    HttpContext.Session.SetString("Carrito", Newtonsoft.Json.JsonConvert.SerializeObject(carrito.Carrito)); //se agrego la cantidad y se volvio a serializar con esa  nueva cantidad
                                                                                                                            //Esto se hace para aumentar SOLO LA CANTIDAD Dde un producto ya agregado en el carrito                    }
                }
                //modal que muestra si se agrego o elimino una cantidad en algun producto del carrito
                if (!aumentarOdisminuir)
                {
                    ViewBag.Message = "SE ELIMINO CORRECTAMENTE ESTE PRODUCTO, CANTIDAD ELIMINADA: 1";
                }
                else
                {
                    ViewBag.Message = "SE AGREGO CORRECTAMENTE SU PRODUCTO, CANTIDAD AGREGADA: 1";
                }
            }
            return PartialView("Modal"); //nombre del metodo al cual se va a redirigir
        }
    }
}

