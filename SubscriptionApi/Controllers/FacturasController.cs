using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionApi.Contexts;
using SubscriptionApi.Services;

namespace SubscriptionApi.Controllers
{
    [ApiController]
    [Route("api/facturas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FacturasController:ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly AuthorizationService _authorizationService;

        public FacturasController(AppDbContext context, IMapper mapper,
            AuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Pay(int id)
        {
            var userId = _authorizationService.GetUserIdValue();
            var factura = await _context.Facturas
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == id && f.UsuarioId == userId);

            if(factura == null) 
                return NotFound();

            if (factura.Pagada)
                return BadRequest("La factura ya ha sido pagada.");

            // Procesamos el pago con la pasarela.

            factura.Pagada = true;
            await _context.SaveChangesAsync();

            var existsOverduePaids = await _context.Facturas
                .AnyAsync(f => !f.Pagada &&
                f.FechaLimitePago < DateTime.Today &&
                f.UsuarioId == userId);

            if(existsOverduePaids)
                return NoContent();

            factura.Usuario.Impago = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
