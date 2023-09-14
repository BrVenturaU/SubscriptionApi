using System.ComponentModel.DataAnnotations;
using SubscriptionApi.Validaciones;

namespace SubscriptionApi.Dtos
{
    public class LibroPatchDto
    {
        [IsFirstLetterCapital]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
