using System.ComponentModel.DataAnnotations;

namespace PruebaCorta.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; }

        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }


        [Display(Name = "Confimar contraseña")]

        public string ConfirmarContrasena { get; set; }

    }
}
