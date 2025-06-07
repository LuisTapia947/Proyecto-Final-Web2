using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Necli.servises.Interfaces;

public class ReporteMensualService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReporteMensualService> _logger;

    public ReporteMensualService(IServiceProvider serviceProvider, ILogger<ReporteMensualService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ahora = DateTime.Now;

            if (ahora.Day == 1)
            {
                var mesAnterior = ahora.AddMonths(-1);

                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<NecliDbContext>();
                        var transaccionesService = scope.ServiceProvider.GetRequiredService<ITransaccionService>();
                        var correoService = scope.ServiceProvider.GetRequiredService<ICorreoServise>();

                        var usuarios = await context.Usuarios.ToListAsync(stoppingToken);

                        foreach (var usuario in usuarios)
                        {
                            var transacciones = await transaccionesService.ObtenerTransaccionesDelMesAnterior(usuario.Id);

                            if (transacciones == null || !transacciones.Any())
                                continue;

                            var pdf = transaccionesService.GenerarPdfProtegido(transacciones, usuario.Id);

                            var ruta = Path.Combine("ReportesMensuales", mesAnterior.Year.ToString(), mesAnterior.Month.ToString("D2"));
                            Directory.CreateDirectory(ruta);

                            var archivoPdf = Path.Combine(ruta, $"{usuario.Id}.pdf");
                            await File.WriteAllBytesAsync(archivoPdf, pdf, stoppingToken);

                            await correoService.EnviarCorreoConAdjuntoAsync(
                                usuario.Correo,
                                "Reporte mensual de movimientos",
                                "<p>Adjunto tus movimientos del mes anterior.</p>",
                                pdf,
                                $"{usuario.Id}.pdf"
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al generar reportes mensuales");
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            else
            {
                var siguienteDia = DateTime.Today.AddDays(1);
                var tiempoEspera = siguienteDia - ahora;
                await Task.Delay(tiempoEspera, stoppingToken);
            }
        }
    }

}
