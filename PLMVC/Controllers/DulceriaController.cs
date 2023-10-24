//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using HtmlAgilityPack;
using System.Text;
using ML;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Pdf;

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
                    GetCarritoAndDeserealizar(carrito); // recupera el carrito que ya estaba y desereaiza

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

        public ML.Venta GetCarritoAndDeserealizar(ML.Venta carrito)
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
                GetCarritoAndDeserealizar(carrito);
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
                GetCarritoAndDeserealizar(carrito); // recupera el carrito que ya estaba y desereaiza

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
        /*
        [HttpPost]
        public FileResult Export(string ExportData) //inicialmente utilice ITEXTSHARP para que corriera y HTML AGILITY PARA EL 1ER BLOQUE (LAS librerias estan comentadas)
        {
            HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
            HtmlDocument doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(ExportData);
            ExportData = doc.DocumentNode.OuterHtml;

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader reader = new StringReader(ExportData);
                Document PdfFile = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                PdfFile.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, PdfFile, reader);
                PdfFile.Close();
                return File(stream.ToArray(), "application/pdf", "ExportData.pdf");
            }
        }
        
       //ESTE METODO Y EL OTRO DE ARRIBA HACEN LO MISMO
       [HttpPost]
       public FileResult Export(string ExportData)
       {
           HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
           HtmlDocument doc = new HtmlDocument();
           doc.OptionFixNestedTags = true;
           doc.LoadHtml(ExportData);
           ExportData = doc.DocumentNode.OuterHtml;

           using (MemoryStream stream = new System.IO.MemoryStream())
           {
               Encoding unicode = Encoding.UTF8;
               StringReader sr = new StringReader(ExportData);
               Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
               PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
               pdfDoc.Open();
               XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
               pdfDoc.Close();
               return File(stream.ToArray(), "application/pdf", "ExportData.pdf");
           }
       }*/

        
        public IActionResult GenerarPDF()
        {
            ML.Venta carrito = new ML.Venta();
            carrito.Carrito = new List<object>();
            GetCarritoAndDeserealizar(carrito);

            // Crear un nuevo documento PDF en una ubicación temporal
            string rutaTempPDF = Path.GetTempFileName() + ".pdf";

            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(rutaTempPDF)))
            {
                using (Document document = new Document(pdfDocument))
                {
                    document.Add(new Paragraph("DETALLES DE COMPRA:"));
                    document.Add(new Paragraph(" "));

                    // Crear la tabla para mostrar la lista de objetos
                    Table table = new Table(6); // 5 columnas
                    table.SetWidth(UnitValue.CreatePercentValue(100)); // Ancho de la tabla al 100% del documento

                    // Añadir las celdas de encabezado a la tabla
                    table.AddHeaderCell("ID Producto");
                    table.AddHeaderCell("Producto");
                    table.AddHeaderCell("Precio");
                    table.AddHeaderCell("Cantidad");
                    table.AddHeaderCell("Imagen");
                    table.AddHeaderCell("SubTotal");

                    decimal? total = 0;

                    foreach (ML.Dulceria producto in carrito.Carrito)
                    {
                        table.AddCell(producto.IdDulceria.ToString());
                        table.AddCell(producto.Nombre);
                        table.AddCell(producto.Precio.ToString());
                        table.AddCell(producto.Cantidad.ToString());
                        byte[] imageBytes = Convert.FromBase64String(producto.Imagen);
                        ImageData data = ImageDataFactory.Create(imageBytes);
                        Image image = new Image(data);
                        table.AddCell(image.SetWidth(50).SetHeight(50));
                        table.AddCell((producto.Cantidad * producto.Precio).ToString());
                        total += ((producto.Cantidad * producto.Precio));
                    }

                    // Añadir la tabla al documento
                    document.Add(table);

                    //Añadir el total de la compra 
                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph("El total de su compra fue:" + total));
                }
                // Leer el archivo PDF como un arreglo de bytes
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaTempPDF);

                // Eliminar el archivo temporal
                System.IO.File.Delete(rutaTempPDF);
                HttpContext.Session.Remove("Carrito");

                // Descargar el archivo PDF
                return new FileStreamResult(new MemoryStream(fileBytes), "application/pdf")
                {
                    FileDownloadName = "DetalleCompra.pdf"
                };
            }
        }
    }
}

