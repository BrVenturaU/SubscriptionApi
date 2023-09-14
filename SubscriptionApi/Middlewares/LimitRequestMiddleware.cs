using Microsoft.EntityFrameworkCore;
using SubscriptionApi.Contexts;
using SubscriptionApi.Entities;
using SubscriptionApi.Settings;

namespace SubscriptionApi.Middlewares
{
    public static class LimitRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitRequest(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitRequestMiddleware>();
        }
    }

    public class LimitRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public LimitRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, SubscriptionSettings settings, AppDbContext dbContext)
        {
            ;
            // TODO:We could set the routes with Reflection, Expressions to make them declarative
            var path = httpContext.Request.Path.Value;
            var isWhiteListRoute = settings.WhiteListRoutes.Any(r => path.Contains(r));
            if (isWhiteListRoute)
            {
                await _next(httpContext);
                return;
            }

            var subscriptionKeyValues = httpContext.Request.Headers["X-Api-Key"];
            if (subscriptionKeyValues.Count == 0)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Llave de subscripción requerida para consumir el API.");
                return;
            }

            var subscriptionKey = subscriptionKeyValues[0];

            var dbKey = await dbContext
                .LlavesApi
                .Include(l => l.RestriccionesDominio)
                .Include(l => l.RestriccionesIp)
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(l => l.Key == subscriptionKey);
            
            if(dbKey == null || !dbKey.Activa)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Llave de subscripción inválida o inactiva.");
                return;
            }

            if(dbKey.TipoLlave == TipoLlave.GRATUITA)
            {
                var now = DateTime.Today;
                var tomorrow = now.AddDays(1);
                var requestsCount = await dbContext
                    .Peticiones
                    .CountAsync(p => p.LlaveId == dbKey.Id && p.FechaPeticion >= now && p.FechaPeticion < tomorrow);

                if(requestsCount >= settings.FreeLimit)
                {
                    httpContext.Response.Headers.RetryAfter = tomorrow.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
                    httpContext.Response.StatusCode = 429; // To Many Request
                    await httpContext.Response.WriteAsync("Ha excedido el limite de peticiones por día. " +
                        "Si desea realizar más peticiones, actualice su subscripción a una cuenta profesional.");
                    return;
                }

            }

            if (dbKey.Usuario.Impago)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Pagos pendientes de realizar.");
                return;
            }

            var areRestrictionsSatisfied = RequestSatisfiesRestrictions(dbKey, httpContext);

            if (!areRestrictionsSatisfied)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            var request = new Peticion
            {
                LlaveId = dbKey.Id,
                Ruta = path,
                FechaPeticion = DateTime.UtcNow
            };

            dbContext.Add(request);
            await dbContext.SaveChangesAsync();

            await _next(httpContext);
        }

        private bool RequestSatisfiesRestrictions(LlaveApi llaveApi, HttpContext httpContext)
        {
            var hasRestrictions = llaveApi.RestriccionesDominio.Any() || llaveApi.RestriccionesIp.Any();
            if (!hasRestrictions)
                return true;


            var areDomainRestrictionsSatisfied = 
                RequestSatisfiesDomainRestrictions(llaveApi.RestriccionesDominio, httpContext);
            var areIpRestrictionsSatisfied = 
                RequestSatisfiesIpRestrictions(llaveApi.RestriccionesIp, httpContext);

            return areDomainRestrictionsSatisfied || areIpRestrictionsSatisfied;
        }

        private bool RequestSatisfiesIpRestrictions(List<RestriccionIp> restricciones, HttpContext httpContext)
        {
            if (restricciones == null || restricciones.Count == 0)
                return false;

            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            if (ip == string.Empty)
                return false;

            return restricciones.Any(r => r.Ip.Equals(ip));
        }

        private bool RequestSatisfiesDomainRestrictions(List<RestriccionDominio> restricciones, HttpContext httpContext)
        {
            if (restricciones == null || restricciones.Count == 0)
                return false;

            var referer = httpContext.Request.Headers.Referer.ToString();
            if(referer == string.Empty)
                return false;

            Uri uri = new Uri(referer);
            string host = uri.Host;

            return restricciones.Any(r => r.Dominio.Equals(host));
        }
    }
}
