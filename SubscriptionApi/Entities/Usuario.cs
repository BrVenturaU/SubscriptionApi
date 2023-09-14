using Microsoft.AspNetCore.Identity;

namespace SubscriptionApi.Entities
{
    public class Usuario: IdentityUser
    {
        public bool Impago { get; set; }
    }
}
