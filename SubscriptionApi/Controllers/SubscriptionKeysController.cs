using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionApi.Contexts;
using SubscriptionApi.Dtos;
using SubscriptionApi.Entities;
using SubscriptionApi.Services;

namespace SubscriptionApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/subscription-keys")]
    public class SubscriptionKeysController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly SubscriptionKeyService _subscriptionKeyService;
        private readonly AuthorizationService _authorizationService;

        public SubscriptionKeysController(AppDbContext context, IMapper mapper, 
            SubscriptionKeyService subscriptionKeyService,
            AuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _subscriptionKeyService = subscriptionKeyService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LlaveDto>>> Get()
        {
            var userId = _authorizationService.GetUserIdValue();
            var userKeys = await _context.LlavesApi
                .Include(l => l.RestriccionesDominio)
                .Include(l => l.RestriccionesIp)
                .Where(l => l.UsuarioId.Equals(userId))
                .ToListAsync();


            return _mapper.Map<List<LlaveDto>>(userKeys);
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            var userId = _authorizationService.GetUserIdValue();
            await _subscriptionKeyService.CreateSubscriptionKey(userId, TipoLlave.PROFESIONAL);
            return Ok("Llave creada exitosamente.");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> ChangeSubscriptionKey(int id)
        {
            var userId = _authorizationService.GetUserIdValue();
            var subscriptionKey = await _context.LlavesApi
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == userId);
            if (subscriptionKey == null)
                return NotFound();

            subscriptionKey.Key = _subscriptionKeyService.GenerateKey();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}/status")]
        public async Task<ActionResult> ChangeStatus(int id)
        {
            var userId = _authorizationService.GetUserIdValue();
            var subscriptionKey = await _context.LlavesApi
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == userId);
            if (subscriptionKey == null)
                return NotFound();

            subscriptionKey.Activa = !subscriptionKey.Activa;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
