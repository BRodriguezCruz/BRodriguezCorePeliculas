using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Usuario
    {
        public static ML.Result Add(ML.Usuario usuario)
        {
            ML.Result result = new ML.Result();

            try 
            { 
                using(DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Database.ExecuteSqlRaw($"UsuarioAdd '{usuario.Nombre}', '{usuario.UserName}', '{usuario.Email}', @Password", new SqlParameter("Password", usuario.Password));
                
                    if(query > 0)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct= false;
                    }
                }           
            } 
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        public static ML.Result GetByEmail(string email)
        {
            ML.Result result = new ML.Result();

            try
            {
                using(DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Usuarios.FromSqlRaw($"UsuarioGetByEmail '{email}'").AsEnumerable().SingleOrDefault();

                    if (query != null)
                    {
                        ML.Usuario usuario = new ML.Usuario();

                        usuario.IdUsuario = query.IdUsuario;
                        usuario.Nombre = query.Nombre;
                        usuario.UserName = query.UserName;
                        usuario.Email = query.Email;
                        usuario.Password = query.Password;                      
                        

                        result.Object = usuario;
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return result;
        }

        public static ML.Result UpdatePassword(ML.Usuario usuario)
        {
            ML.Result result = new ML.Result();

            try
            {
                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Database.ExecuteSqlRaw($"UsuarioUpdatePassword '{usuario.Email}', @Password", new SqlParameter("Password", usuario.Password));

                    if (query > 0)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return result;
        }
    }
}
