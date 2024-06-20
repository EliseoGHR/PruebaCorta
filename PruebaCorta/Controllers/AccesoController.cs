﻿using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using PruebaCorta.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;



namespace PruebaCorta.Controllers
{
 

    public class AccesoController : Controller
    {
       
        static string conn = "Data Source=DESKTOP-PAKLHRS;Initial Catalog=PruebaCortaTec;Integrated Security=True;Encrypt=False";

        [AllowAnonymous]
        public async Task<IActionResult>Login(string ReturnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Usuario usuario, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(usuario.Contrasena))
            {
                ViewBag.Error = "Credenciales incorrectas";
                ViewBag.pReturnUrl = ReturnUrl;
                return View(usuario);
            }

            usuario.Contrasena = CalcularHashMD5(usuario.Contrasena);

            using (SqlConnection cn = new SqlConnection(conn))
            {
                SqlCommand cmd = new SqlCommand("ValidarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);

                cn.Open();
                usuario.Id = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (usuario.Id != 0)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim("Id", usuario.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });

                if (!string.IsNullOrWhiteSpace(ReturnUrl))
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Usuario no encontrado";
                ViewBag.pReturnUrl = ReturnUrl;
                return View(usuario);
            }

        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario usuario)
        {
            bool registrado;
            string mensaje;

            if (usuario.Contrasena != usuario.ConfirmarContrasena)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View(usuario);
            }

            usuario.Contrasena = CalcularHashMD5(usuario.Contrasena);

            using (SqlConnection cn = new SqlConnection(conn))
            {
                SqlCommand cmd = new SqlCommand("RegistrarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                cmd.Parameters.Add("@Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                cn.Open();
                await cmd.ExecuteNonQueryAsync();

                registrado = Convert.ToBoolean(cmd.Parameters["@Registrado"].Value);
                mensaje = cmd.Parameters["@Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View(usuario);
            }

        }

        private string CalcularHashMD5(string texto)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Convierte la cadena de texto a bytes
                byte[] inputBytes = Encoding.UTF8.GetBytes(texto);

                // Calcula el hash MD5 de los bytes
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convierte el hash a una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }


        }
    }
}
