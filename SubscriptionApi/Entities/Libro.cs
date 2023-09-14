using System.ComponentModel.DataAnnotations;
using SubscriptionApi.Validaciones;

namespace SubscriptionApi.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [IsFirstLetterCapital]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
