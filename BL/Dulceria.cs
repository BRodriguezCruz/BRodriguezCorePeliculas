using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Dulceria
    {
        public static ML.Result GetAll()
        {
            ML.Result result = new ML.Result();

            try
            {
                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Dulceria.FromSqlRaw("DulceriaGetAll").ToList();

                    result.Objects = new List<object>();

                    if (query != null)
                    {


                        foreach (var registro in query)
                        {
                            ML.Dulceria dulceria = new ML.Dulceria();

                            dulceria.IdDulceria = registro.IdDulceria;
                            dulceria.Nombre = registro.Nombre;
                            dulceria.Precio = registro.Precio;
                            dulceria.Imagen = registro.Imagen;

                            result.Objects.Add(dulceria);
                        }
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

        public static ML.Result GetById(int idDulceria)
        {
            ML.Result result = new ML.Result();

            try
            {
                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Dulceria.FromSqlRaw($"DulceriaGetById {idDulceria}").AsEnumerable().SingleOrDefault();

                    if (query != null)
                    {
                        ML.Dulceria dulceria = new ML.Dulceria();

                        dulceria.IdDulceria = query.IdDulceria;
                        dulceria.Nombre = query.Nombre;
                        dulceria.Precio = query.Precio;
                        dulceria.Imagen = query.Imagen;

                        result.Object = dulceria;
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
