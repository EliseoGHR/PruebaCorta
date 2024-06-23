using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaCorta.Models;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace PruebaCorta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static string conn = "Data Source=DESKTOP-PAKLHRS;Initial Catalog=PruebaCortaTec;Integrated Security=True;Encrypt=False";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            List<Pregunta> preguntas = new List<Pregunta>();

            using (SqlConnection cn = new SqlConnection(conn))
            {
                SqlCommand cmd = new SqlCommand("ObtenerPreguntas", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        preguntas.Add(new Pregunta
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Texto = reader.GetString(reader.GetOrdinal("Texto")),
                            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                            Estatus = reader.GetBoolean(reader.GetOrdinal("Estatus")) ? (byte)1 : (byte)0,
                            UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                            Usuario = new Usuario
                            {
                                NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario"))
                            }
                        });
                    }
                }
            }

            return View(preguntas.OrderByDescending(p => p.FechaCreacion));
        }


        [HttpGet]
        public IActionResult HacerPregunta()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HacerPregunta(string Texto)
        {

            if (!string.IsNullOrEmpty(Texto))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("HacerPregunta", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        Pregunta nuevaPregunta = new Pregunta
                        {
                            Texto = Texto,
                            FechaCreacion = DateTime.Now,
                            Estatus = 1, 
                            UsuarioId = int.Parse(User.FindFirst("Id").Value) 
                        };

                        command.Parameters.AddWithValue("@Texto", nuevaPregunta.Texto);
                        command.Parameters.AddWithValue("@FechaCreacion", nuevaPregunta.FechaCreacion);
                        command.Parameters.AddWithValue("@Estatus", nuevaPregunta.Estatus);
                        command.Parameters.AddWithValue("@UsuarioId", nuevaPregunta.UsuarioId);

                        int id = Convert.ToInt32(command.ExecuteScalar());

                        nuevaPregunta.Id = id;

                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.ErrorMessage = "Ocurrió un error al insertar la pregunta.";
                }
            }


            return View();
        }

        [HttpGet]
        public ActionResult ResponderPregunta(int id)
        {
            ViewBag.PreguntaId = id;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResponderPregunta(int preguntaId, string texto)
        {
            if (!string.IsNullOrEmpty(texto))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("ResponderPregunta", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        Respuesta nuevaRespuesta = new Respuesta
                        {
                            Texto = texto,
                            FechaCreacion = DateTime.Now,
                            PreguntaId = preguntaId,
                            UsuarioId = int.Parse(User.FindFirst("Id").Value)
                        };

                        command.Parameters.AddWithValue("@Texto", nuevaRespuesta.Texto);
                        command.Parameters.AddWithValue("@FechaCreacion", nuevaRespuesta.FechaCreacion);
                        command.Parameters.AddWithValue("@PreguntaId", nuevaRespuesta.PreguntaId);
                        command.Parameters.AddWithValue("@UsuarioId", nuevaRespuesta.UsuarioId);

                        command.ExecuteNonQuery();

                        return RedirectToAction("Index", new { id = preguntaId });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.ErrorMessage = "Ocurrió un error al insertar la respuesta.";
                }
            }

            ViewBag.PreguntaId = preguntaId;
            return View();
        }


        public async Task<ActionResult> VerRespuesta(int id)
        {
            List<Respuesta> respuestas = new List<Respuesta>();

            try
            {
                using (SqlConnection cn = new SqlConnection(conn))
                {
                    SqlCommand cmd = new SqlCommand("ObtenerRespuestasPorPregunta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PreguntaId", id);

                    cn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            respuestas.Add(new Respuesta
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Texto = reader.GetString(reader.GetOrdinal("Texto")),
                                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                Usuario = new Usuario
                                {
                                    NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario"))
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.ErrorMessage = "Ocurrió un error al obtener las respuestas.";
            }

            ViewBag.PreguntaId = id;
            return View(respuestas);
        }

        public async Task<ActionResult> CerrarPregunta(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("ObtenerDePreguntaUsuarioCreador", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PreguntaId", id);

                    int creadorId = (int)cmd.ExecuteScalar();

                    if (creadorId == int.Parse(User.FindFirst("Id").Value))
                    {
                        SqlCommand cerrarCmd = new SqlCommand("CerrarPregunta", connection);
                        cerrarCmd.CommandType = CommandType.StoredProcedure;
                        cerrarCmd.Parameters.AddWithValue("@PreguntaId", id);

                        cerrarCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        return Unauthorized(); 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }
    




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
