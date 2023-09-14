using System.ComponentModel.DataAnnotations;

namespace SubscriptionApi.Dtos
{
    public class EditarAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
