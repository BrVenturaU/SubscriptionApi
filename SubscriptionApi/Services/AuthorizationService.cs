using System.Security.Claims;

namespace SubscriptionApi.Services
{
    public class AuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Claim GetClaim(string claimType)
        {
            return _httpContextAccessor
                .HttpContext
                .User
                .Claims
                .FirstOrDefault(c => c.Type.Equals(claimType)) ?? new Claim(claimType, "");
        }

        public string GetClaimValue(string claimType) => GetClaim(claimType).Value;

        public string GetUserIdValue() => GetClaimValue(ClaimTypes.NameIdentifier);
        public string GetUserEmailValue => GetClaimValue(ClaimTypes.Email);


    }
}
