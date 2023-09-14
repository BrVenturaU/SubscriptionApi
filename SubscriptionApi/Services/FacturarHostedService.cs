using Microsoft.EntityFrameworkCore;
using SubscriptionApi.Contexts;

namespace SubscriptionApi.Services
{
    public class FacturarHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public FacturarHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ProcesarFacturas, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private void ProcesarFacturas(object state)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            AsignarImpago(context);
            EmitirFacturas(context);
        }

        private static void AsignarImpago(AppDbContext context)
        {
            var procedure = "SP_ASIGNAR_IMPAGO";
            context.Database.ExecuteSqlInterpolated($"EXEC {procedure}");
        }

        private static void EmitirFacturas(AppDbContext context)
        {
            var today = DateTime.Today;
            var comparisonDate = today.AddMonths(-1);
            // Verificamos si el lote de facturas del mes anterior han sido emitidas o no.
            var hanSidoEmitidas = context.FacturasEmitidas
                .Any(fe => fe.Anio == comparisonDate.Year && fe.Mes == comparisonDate.Month);
            if (hanSidoEmitidas)
                return;

            var startDate = new DateTime(comparisonDate.Year, comparisonDate.Month, 1);
            var endDate = startDate.AddMonths(1);
            var procedure = "SP_CREACION_FACTURAS";
            context.Database
                .ExecuteSqlInterpolated($"EXEC {procedure} {startDate.ToString("yyyy-MM-dd")}, {endDate.ToString("yyyy-MM-dd")}");
        }
    }
}
