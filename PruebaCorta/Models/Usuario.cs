using System.ComponentModel.DataAnnotations;

namespace PruebaCorta.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 4, ErrorMessage = "El Correo Electrónico debe tener entre 4 y 50 caracteres")]
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "La contraseña debe tener entre 5 y 100 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "Confirmar la contraseña es requerida")]
        [Display(Name = "Confimar contraseña")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "La contraseña debe tener entre 5 y 100 caracteres.")]
        public string ConfirmarContrasena { get; set; }

        public IList<Pregunta> Preguntas { get; set; }

        public IList<Respuesta> Respuestas { get; set; }

    }
}
