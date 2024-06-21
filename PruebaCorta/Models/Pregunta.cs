using System.ComponentModel.DataAnnotations;

namespace PruebaCorta.Models
{
    public class Pregunta
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Pregunta")]
        [Required(ErrorMessage = "La pregunta es requerida")]
        [StringLength(300, MinimumLength = 4, ErrorMessage = "La pregunta debe tener entre 4 y 300 caracteres")]
        public string Texto { get; set; }

        [Display(Name = "Fecha")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Estado")]
        public byte Estatus { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        /// <summary>
        /// 1= Pregunta abierta
        /// 2= Pregunta Cerrada
        /// </summary>
        public IList<Respuesta> Respuestas { get; set; }
    }
}
