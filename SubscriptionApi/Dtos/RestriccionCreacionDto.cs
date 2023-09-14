using System.ComponentModel.DataAnnotations;

namespace SubscriptionApi.Dtos
{
    public class RestriccionCreacionDto
    {
        public int LlaveId { get; set; }
        [Required]
        public string Valor { get; set; }
    }
}
