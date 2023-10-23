using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Net.Mail;
using System.Web;

namespace PLMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UsuarioController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            //Convert the string into an array of bytes.
            byte[] messageBytes = Encoding.UTF8.GetBytes(password);

            //Create the hash value from the array of bytes.
            //byte[] hashValue = SHA256.HashData(messageBytes); convierte la corriente de bytes en byte20, y no coincide con el byte11 de la BD aunque
            //sea la contraseña correcta, no esta mal, solo es diferente formato el hasheo


            /* 
             FORMA 2 DE ENCRIPTAR ---> (Si se usa esta forma, el hasValue y messageBytes de arriba se deben comentar)
             var bcrypt = new Rfc2898DeriveBytes(password, new byte[0], 10000, HashAlgorithmName.SHA256);
             var passwordHash = bcrypt.GetBytes(20); este convierte la corrientes de bytes en tamaño de byte20
            */

            ML.Result result = BL.Usuario.GetByEmail(email);

            if (result.Correct)
            {
                ML.Usuario usuario = new ML.Usuario();
                usuario = (ML.Usuario)result.Object;

                if ((usuario.Password.SequenceEqual(messageBytes)))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Login = true;
                    ViewBag.Message = "CONTRASEÑA INCORRECTA, INTENTA DE NUEVO";
                    return PartialView("Modal");
                }
            }
            else
            {
                ViewBag.Login = true;
                ViewBag.Message = "USUARIO NO EXISTENTE";
                return PartialView("Modal");
            }
        }

        [HttpPost]
        public IActionResult AddUser(ML.Usuario usuario, string password)
        {
            //Convert the string into an array of bytes.
            byte[] convertHexadecimal = Encoding.UTF8.GetBytes(password);

            //Create the hash value from the array of bytes.
            //byte[] hashValue = SHA256.HashData(convertHexadecimal); se hace le hasheo en corriente de bytes32 o byte11, no recuerdo bien jeje

            /* 
            FORMA 2 DE ENCRIPTAR ---> (Si se usa esta forma, el hasValue de arriba se debe comentar)            
            var bcrypt = new Rfc2898DeriveBytes(convertHexadecimal, new byte[0], 10000, HashAlgorithmName.SHA256);
            var passwordHash = bcrypt.GetBytes(20); 
            */

            usuario.Password = convertHexadecimal;

            ML.Result result = BL.Usuario.Add(usuario);

            if (result.Correct)
            {
                ViewBag.Message = "TE REGISTRASTE CORRECTAMENTE, AHORA INICIA SESIÓN";
                ViewBag.Login = true;
            }
            else
            {
                ViewBag.Message = "ERROR, NO SE LOGRO REGISTRAR, VUELVE A INTENTAR";
                ViewBag.Login = true;

            }
            return PartialView("Modal");
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(string email)
        {
            if (HttpContext.Session.GetString("Correo") != null)
            {
                HttpContext.Session.Remove("Correo"); //Limpia la session

                ML.Result result = BL.Usuario.GetByEmail(email);

                if (result.Correct)
                {
                    ML.Usuario usuario = new ML.Usuario();
                    usuario = (ML.Usuario)result.Object;

                    if (usuario.Email == email)
                    {
                        string emailOrigen = "brodriguezc14@gmail.com";

                        MailMessage mailMessage = new MailMessage(emailOrigen, email, "Recuperar Contraseña", "<p>Correo para recuperar contraseña</p>");
                        mailMessage.IsBodyHtml = true;

                        //string contenidoHTML = System.IO.File.ReadAllText(@"C:\users\digis\Documents\IISExpress\Leonardo Escogido Bravo\Proyecto2023Ecommerce\PL\Views\Usuario\Email.html");

                        //string contenidoHTML = System.IO.File.ReadAllText(Path.Combine("Views", "Usuario", "Email.html"));


                        string contenidoHTML = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Templates", "Correo.html"));

                        mailMessage.Body = contenidoHTML;
                        //string url = "http://localhost:5057/Usuario/NewPassword/" + HttpUtility.UrlEncode(email);
                        string url = "http://localhost:5016/Usuario/UpdatePassword/" + HttpUtility.UrlEncode(email);
                        mailMessage.Body = mailMessage.Body.Replace("{Url}", url);
                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new System.Net.NetworkCredential(emailOrigen, "rcehfpqnaowihkvp");

                        smtpClient.Send(mailMessage);
                        smtpClient.Dispose();

                        //ViewBag.Modal = "show";
                        HttpContext.Session.SetString("Correo", Newtonsoft.Json.JsonConvert.SerializeObject(email));
                        ViewBag.Message = "Se ha enviado un correo de confirmación a tu correo electronico";
                        ViewBag.Login = true;
                    }
                    else
                    {
                        ViewBag.Message = "El correo electronico ingresado no existe, verifica y vuelve a intentarlo";
                        ViewBag.Login = true;
                    }
                }
                else
                {
                    ViewBag.Message = "Ocurrio un error inesperado, por favor intenta nuevamente";
                    ViewBag.Login = true;
                }
            }
            else
            {
                ViewBag.Message = "Ocurrio un error inesperado, por favor intenta nuevamente";
            }
            return PartialView("Modal");
        }

        [HttpGet]
        public ActionResult UpdatePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassword(string password)
        {
            ML.Usuario usuario = new ML.Usuario();

            if (HttpContext.Session.GetString("Correo") != null)
            {
                //Convert the string into an array of bytes.
                byte[] convertHexadecimal = Encoding.UTF8.GetBytes(password);
                var correoDeserealizado = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(HttpContext.Session.GetString("Correo"));

                usuario.Email = correoDeserealizado;
                usuario.Password = convertHexadecimal;

                ML.Result result = BL.Usuario.UpdatePassword(usuario);

                if (result.Correct)
                {
                    ViewBag.Message = "SE HA ACTUALIZADO CORRECTAMENTE TU CONTRASEÑA, AHORA INICIA SESION";
                    ViewBag.Login = true;
                }
                else
                {
                    ViewBag.Message = "ERROR, NO SE HA ACTUALIZADO CORRECTAMENTE TU CONTRASEÑA, INTENTA NUEVAMENTE";
                    ViewBag.Login = true;
                }
            }
            else
            {
                ViewBag.Message = "HUBO UN ERROR INESPERADO, NO SE ENCONTRO TU CORREO, INTENTA NUEVAMENTE";
                ViewBag.Login = true;
            }

            return PartialView("Modal");
        }

        public string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
