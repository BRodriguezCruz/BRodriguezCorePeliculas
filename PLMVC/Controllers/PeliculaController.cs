using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PLMVC.Models;

namespace PLMVC.Controllers
{
    public class PeliculaController : Controller
    {
        /*
        [HttpGet]
        public IActionResult GetAll()
        {
            Root root = new Root();
            Pelicula pelicula = new Pelicula();
            pelicula.Peliculas = new List<object>();

            //LLAMADO AL SERVICIO

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("https://api.themoviedb.org/3/movie/popular");
                var responseTask = cliente.GetAsync("?api_key=a56696452b180e5a0358adb1ec887f2b");
                responseTask.Wait(); //aqui entra al servicio web

                var resultService = responseTask.Result;

                if (resultService.IsSuccessStatusCode)
                {
                    var readTask = resultService.Content.ReadAsAsync<Root>();
                    readTask.Wait();

                    foreach(var resultPelicula in readTask.Result.results)
                    {
                        //Pelicula resultItemsList = Newtonsoft.Json.JsonConvert.DeserializeObject<Pelicula>(resultPelicula.ToString()); no se usa pues ya llega deserializado
                        pelicula.Peliculas.Add(resultPelicula);
                    }
                }
            }
            return View(pelicula);
        }
        */


        //GET ALL CON LA VARIABLE DYNAMIC, aqui no se usa ningun modelo como el creado para el metodo de arriba
        [HttpGet]
        public IActionResult GetAll()
        {
            Pelicula pelicula = new Pelicula();
            pelicula.Peliculas = new List<object>();

            //LLAMADO AL SERVICIO

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("https://api.themoviedb.org/3/movie/popular");
                var responseTask = cliente.GetAsync("?api_key=a56696452b180e5a0358adb1ec887f2b");
                responseTask.Wait(); //aqui entra al servicio web

                var resultService = responseTask.Result;

                if (resultService.IsSuccessStatusCode)
                {
                    var readTask = resultService.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result);
                    readTask.Wait();

                    foreach (var resultPelicula in resultJSON.results)
                    {
                        Pelicula peliculaItem = new Pelicula(); //se instancia un objeto auxiliar para meter el valor en esas prop a la lista en la instancia "peliculas",
                                                                //sin esta instancia la vista manda la misma pelicula en todas las CARDS 
                                                                //aqui ya no se usa el modelo root ya que DYNAMIC LEE MI JSON y ya solo agarro lo que ocupe para mis prop en pelicula


                        //aqui enviamos los parametros que usamos en la vista, pueden ser todos o no
                        peliculaItem.overview = resultPelicula.overview;
                        peliculaItem.title = resultPelicula.title;
                        peliculaItem.backdrop_path = resultPelicula.backdrop_path;
                        peliculaItem.poster_path = resultPelicula.poster_path;
                        peliculaItem.id = resultPelicula.id;

                        pelicula.Peliculas.Add(peliculaItem);
                    }
                }
            }
            return View(pelicula);
        }



        public IActionResult Favorite(int idPelicula, bool fav)
        {
            using (var cliente = new HttpClient())
            {

                /* (FORMA 1) SE CREO UN MODELO PARA MANDAR LOS PARAMETROS DEL BODY 
                Favorito favorito = new Favorito();

                favorito.media_id = idPelicula;
                favorito.media_type = "movie";
                favorito.favorite = true;
                */

                //(FORMA 2) CREACION DE UN OBJETO ANONIMO PARA NO CREAR UN MODELO COMPLETO 
                var favorito = new
                {
                    media_id = idPelicula,
                    media_type = "movie",
                    favorite = fav //AQUI DETERMINA SI SE AÑADE O SE QUITA DE FAVORITOS
                };

                cliente.BaseAddress = new Uri("https://api.themoviedb.org/3/account/20522135/favorite");
                var postTask = cliente.PostAsJsonAsync("?api_key=a56696452b180e5a0358adb1ec887f2b&session_id=87020e62fddbf6147d2f7d06693a1d0cf65b1576", favorito);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    if (fav)
                    {
                        ViewBag.Message = "PELICULA AGREGADA A FAVORITOS";
                    }
                    else
                    {
                        ViewBag.Message = "PELICULA ELIMINADA DE FAVORITOS";

                    }
                }
                else
                {
                    ViewBag.Message = "ERROR, FALLO AL HACER LA OPERACION LA PELICULA DE FAVORITOS";
                }
            }
            return PartialView("Modal");
        }


        [HttpGet]
        public IActionResult GetFavorite()
        {
            Pelicula pelicula = new Pelicula();
            pelicula.Peliculas = new List<object>();

            //LLAMADO AL SERVICIO

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("https://api.themoviedb.org/3/account/20522135/favorite/movies");
                var responseTask = cliente.GetAsync("?api_key=a56696452b180e5a0358adb1ec887f2b&language=en-ES&session_id=87020e62fddbf6147d2f7d06693a1d0cf65b1576");
                responseTask.Wait(); //aqui entra al servicio web

                var resultService = responseTask.Result;

                if (resultService.IsSuccessStatusCode)
                {
                    var readTask = resultService.Content.ReadAsStringAsync();
                    dynamic resultJSON = JObject.Parse(readTask.Result.ToString());
                    readTask.Wait();

                    foreach (var resultPelicula in resultJSON.results)
                    {
                        Pelicula peliculaItem = new Pelicula(); //se instancia un objeto auxiliar para meter el valor en esas prop a la lista en la instancia "peliculas",
                                                                //sin esta instancia la vista manda la misma pelicula en todas las CARDS 
                                                                //aqui ya no se usa el modelo root ya que DYNAMIC LEE MI JSON y ya solo agarro lo que ocupe para mis prop en pelicula


                        //aqui enviamos los parametros que usamos en la vista, pueden ser todos o no
                        peliculaItem.overview = resultPelicula.overview;
                        peliculaItem.title = resultPelicula.title;
                        peliculaItem.backdrop_path = resultPelicula.backdrop_path;
                        peliculaItem.poster_path = resultPelicula.poster_path;
                        peliculaItem.id = resultPelicula.id;

                        pelicula.Peliculas.Add(peliculaItem);
                    }
                }
            }
            return View(pelicula);
        }

    }
}
