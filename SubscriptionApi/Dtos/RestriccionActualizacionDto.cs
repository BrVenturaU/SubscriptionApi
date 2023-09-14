using System.ComponentModel.DataAnnotations;

namespace SubscriptionApi.Dtos
{
    public class RestriccionActualizacionDto
    {
        [Required]
        public string Valor { get; set; }
    }
}
