using Microsoft.AspNetCore.Mvc;

namespace PLMVC.Controllers
{
    public class EstadisticaController : Controller
    {

        public IActionResult GetEstadistica()
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
    }
}
