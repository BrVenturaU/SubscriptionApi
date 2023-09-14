using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SubscriptionApi.Dtos;
using SubscriptionApi.Services;
using SubscriptionApi.Entities;

namespace SubscriptionApi.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<Usuario> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<Usuario> signInManager;
        private readonly SubscriptionKeyService subscriptionKeyService;

        public CuentasController(UserManager<Usuario> userManager,
            IConfiguration configuration,
            SignInManager<Usuario> signInManager, SubscriptionKeyService subscriptionKeyService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.subscriptionKeyService = subscriptionKeyService;
        }

        [HttpPost("registrar")] // api/cuentas/registrar
        public async Task<ActionResult<UsuarioAutenticadoDto>> Registrar(AutenticarUsuarioDto credencialesUsuario)
        {
            var usuario = new Usuario { UserName = credencialesUsuario.Email, 
                Email = credencialesUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (!resultado.Succeeded)
                return BadRequest(resultado.Errors);

            await subscriptionKeyService.CreateSubscriptionKey(usuario.Id, TipoLlave.GRATUITA);
            return await ConstruirToken(credencialesUsuario, usuario.Id);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioAutenticadoDto>> Login(AutenticarUsuarioDto credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
                return await ConstruirToken(credencialesUsuario, usuario.Id);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UsuarioAutenticadoDto>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var idClaim = HttpContext.User.Claims
                .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var email = emailClaim.Value;
            var credencialesUsuario = new AutenticarUsuarioDto()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario, idClaim.Value);
        }

        private async Task<UsuarioAutenticadoDto> ConstruirToken(AutenticarUsuarioDto credencialesUsuario, string usuarioId)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId),
                new Claim(ClaimTypes.Email, credencialesUsuario.Email),
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new UsuarioAutenticadoDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        [HttpPost("HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDto editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("Admin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDto editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("Admin", "1"));
            return NoContent();
        }
    }
}
