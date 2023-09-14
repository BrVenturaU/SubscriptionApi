using SubscriptionApi.Contexts;
using SubscriptionApi.Entities;

namespace SubscriptionApi.Services
{
    public class SubscriptionKeyService
    {
        private readonly AppDbContext _context;

        public SubscriptionKeyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateSubscriptionKey(string usuarioId, TipoLlave tipoLlave)
        {
            var subscriptionKey = GenerateKey();

            var apiKey = new LlaveApi
            {
                Key = subscriptionKey,
                TipoLlave = tipoLlave,
                UsuarioId = usuarioId
            };

            _context.Add(apiKey);
            await _context.SaveChangesAsync();
        }

        public string GenerateKey() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
