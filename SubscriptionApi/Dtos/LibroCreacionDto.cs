using System.ComponentModel.DataAnnotations;
using SubscriptionApi.Validaciones;

namespace SubscriptionApi.Dtos
{
    public class LibroCreacionDto
    {
        [IsFirstLetterCapital]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
