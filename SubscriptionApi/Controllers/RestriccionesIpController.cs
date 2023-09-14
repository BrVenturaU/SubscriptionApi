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
    [ApiController]
    [Route("api/restricciones-ip")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesIpController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly SubscriptionKeyService _subscriptionKeyService;
        private readonly AuthorizationService _authorizationService;

        public RestriccionesIpController(AppDbContext context, IMapper mapper,
            SubscriptionKeyService subscriptionKeyService,
            AuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _subscriptionKeyService = subscriptionKeyService;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionCreacionDto creacionDto)
        {
            var userId = _authorizationService.GetUserIdValue();
            var key = await _context.LlavesApi
                .FirstOrDefaultAsync(k => k.Id == creacionDto.LlaveId && k.UsuarioId == userId);

            if (key == null)
                return NotFound();

            var restriccion = new RestriccionIp()
            {
                Ip = creacionDto.Valor,
                LlaveId = creacionDto.LlaveId
            };

            _context.Add(restriccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestriccionActualizacionDto actualizacionDto)
        {
            var userId = _authorizationService.GetUserIdValue();

            var restriccion = await _context.RestriccionesIp
                .Include(rd => rd.Llave)
                .FirstOrDefaultAsync(rd => rd.Id == id);

            if (restriccion == null || restriccion.Llave.UsuarioId != userId)
                return NotFound();

            restriccion.Ip = actualizacionDto.Valor;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = _authorizationService.GetUserIdValue();

            var restriccion = await _context.RestriccionesIp
                .Include(rd => rd.Llave)
                .FirstOrDefaultAsync(rd => rd.Id == id);

            if (restriccion == null || restriccion.Llave.UsuarioId != userId)
                return NotFound();

            _context.Remove(restriccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
