using Microsoft.EntityFrameworkCore;

namespace BL
{
    public class Cine
    {
        public static ML.Result GetAll()
        {
            ML.Result result = new ML.Result();

            try
            {

                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Cines.FromSqlRaw("CineGetAll").ToList();

                    result.Objects = new List<object>();

                    if (query != null)
                    {
                        foreach (var registro in query)
                        {
                            ML.Cine cine = new ML.Cine();

                            cine.IdCine = registro.IdCine;
                            cine.Nombre = registro.Nombre;
                            cine.Ventas = registro.Ventas;
                            cine.Direccion = registro.Direccion;


                            cine.Zona = new ML.Zona();
                            cine.Zona.IdZona = registro.IdZona;
                            cine.Zona.Nombre = registro.NombreZona;

                            result.Objects.Add(cine);
                        }
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                    }
                }
            }
            catch(Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.Correct = false;
                result.Ex = ex;
            }
            return result;
        }

        public static ML.Result GetById(int idCine)
        {
            ML.Result result = new ML.Result();

            try
            {

                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    var query = context.Cines.FromSqlRaw($"CineGetById {idCine}").AsEnumerable().SingleOrDefault();

                    if (query != null)
                    {

                            ML.Cine cine = new ML.Cine();

                            cine.IdCine = query.IdCine;
                            cine.Nombre = query.Nombre;
                            cine.Ventas = query.Ventas;
                            cine.Direccion = query.Direccion;


                            cine.Zona = new ML.Zona();
                            cine.Zona.IdZona = query.IdCine;
                            cine.Zona.Nombre = query.NombreZona;

                            result.Object = cine;

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
                result.ErrorMessage = ex.Message;
                result.Correct = false;
                result.Ex = ex;
            }
            return result;
        }

        public static ML.Result Add(ML.Cine cine)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {

                    var query = context.Database.ExecuteSqlRaw($"CineAdd '{cine.Nombre}', '{cine.Direccion}', {cine.Ventas}, {cine.Zona.IdZona}");

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
                result.ErrorMessage = ex.Message;
                result.Correct = false;
                result.Ex = ex;
            }

            return result;
        }

        public static ML.Result Update(ML.Cine cine)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {
                    cine.Cines = new List<object>();
                    cine.Zona.Zonas = new List<object>();
                    cine.Zona.Nombre = "";

                    var query = context.Database.ExecuteSqlRaw($"CineUpdate {cine.IdCine},'{cine.Nombre}', '{cine.Direccion}', {cine.Ventas}, {cine.Zona.IdZona}");

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
                result.ErrorMessage = ex.Message;
                result.Correct = false;
                result.Ex = ex;
            }

            return result;
        }

        public static ML.Result Delete(int idCine)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (DL.BrodriguezCineContext context = new DL.BrodriguezCineContext())
                {

                    var query = context.Database.ExecuteSqlRaw($"CineDelete {idCine}");

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
                result.ErrorMessage = ex.Message;
                result.Correct = false;
                result.Ex = ex;
            }

            return result;
        }
    }
}