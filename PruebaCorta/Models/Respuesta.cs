using System.ComponentModel.DataAnnotations;

namespace PruebaCorta.Models
{
    public class Respuesta
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Respuesta")]
        [Required(ErrorMessage = "La respuesta es requerida")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "La repuesta debe tener entre 2 y 300 caracteres")]
        public string Texto { get; set; }
        public int PreguntaId { get; set; }
        public int UsuarioId { get; set; }
        public Pregunta Pregunta { get; set; }
        public Usuario Usuario { get; set; }
    }
}
