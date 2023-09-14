using System.ComponentModel.DataAnnotations;

namespace SubscriptionApi.Dtos
{
    public class AutenticarUsuarioDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
