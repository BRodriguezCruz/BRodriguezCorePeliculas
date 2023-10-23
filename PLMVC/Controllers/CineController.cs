using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace PLMVC.Controllers
{
    public class CineController : Controller
    {

        /* METODO QUE CONSUME DIRECTAMENTE EL BL 
        public IActionResult GetAll()
        {
            ML.Cine cine = new ML.Cine();
            ML.Result result = BL.Cine.GetAll();

            if (result.Correct)
            {
                cine.Cines = result.Objects;
            }
            return View(cine);
        }
        */

        [HttpGet] //METODO QUE CONSUME A TRAVES DEL WEB SERVICE
        public IActionResult GetAll()
        {
            ML.Cine cine = new ML.Cine();
            cine.Cines = new List<object>();

            using (HttpClient cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("http://localhost:5045/api/");
                var respuestaTarea = cliente.GetAsync("cine");
                respuestaTarea.Wait(); //aqui entra al servicio web y espera respuesta

                var resultadoServicio = respuestaTarea.Result;

                if (resultadoServicio.IsSuccessStatusCode)
                {
                    var leerTarea = resultadoServicio.Content.ReadAsAsync<ML.Result>();
                    leerTarea.Wait();

                    foreach (var resultCines in leerTarea.Result.Objects)
                    {
                        ML.Cine resultItemsList = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Cine>(resultCines.ToString());
                        cine.Cines.Add(resultItemsList);    
                    }
                }
            }
            return View(cine);
        }


        public IActionResult Form(int? idCine)
        {
            ML.Cine cine = new ML.Cine();
            cine.Zona = new ML.Zona();
            ML.Result result = BL.Zona.GetAll();

            if (idCine != null)
            {
                using (HttpClient cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri("http://localhost:5045/api/");
                    var respuestaTarea = cliente.GetAsync("cine/" + idCine);
                    respuestaTarea.Wait();//entra al servicio

                    var resutServicio = respuestaTarea.Result;

                    if (resutServicio.IsSuccessStatusCode)
                    {
                        var leerTarea = resutServicio.Content.ReadAsAsync<ML.Result>();
                        leerTarea.Wait();

                        ML.Cine resultItemList = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Cine>(leerTarea.Result.Object.ToString());

                        cine = resultItemList;
                    }
                }
                cine.Zona.Zonas = result.Objects;
            }
            else
            {
                cine.Zona.Zonas = result.Objects;
            }
            return View(cine);
        }

        [HttpPost]
        public ActionResult Form(ML.Cine cine)
        {
            cine.Cines = new List<object>();
            cine.Zona.Zonas = new List<object>();
            cine.Zona.Nombre = "";

            if (cine.IdCine == 0)
            {
                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri("http://localhost:5045/api/");

                    //HTTP POST
                    var postTask = cliente.PostAsJsonAsync("cine", cine);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "CINE AGREGADO CORRECTAMENTE";
                    }
                    else
                    {
                        ViewBag.Message = "ERROR, EL CINE NO SE AGREGO";
                    }
                }
            }
            else
            {
                using (var cliente = new HttpClient())
                {
                    int idCine = cine.IdCine;

                    cliente.BaseAddress = new Uri("http://localhost:5045/api/");

                    var putTask = cliente.PutAsJsonAsync("cine/" + idCine, cine);
                    putTask.Wait();

                    var result = putTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "SE ACTUALIZO CORRECTAMENTE EL CINE";
                    }
                    else
                    {
                        ViewBag.Message = "NO SE LOGRO ACTUALIZAR EL CINE";
                    }
                }
            }

            return PartialView("Modal");
        }
    }
}
