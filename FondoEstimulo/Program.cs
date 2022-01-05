using FondoEstimulo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace FondoEstimulo
{
    public class Program
    {
        #region Private Methods

        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<FondoEstimuloContext>();

                DbInitializer.Initialize(services, context).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocurrio un error al intentar crear la base de datos");
            }
        }

        #endregion Private Methods

        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                    Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Inicializando servidor web");
                var host = CreateHostBuilder(args).Build();

                CreateDbIfNotExists(host);

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Servidor terminado de forma inesperada");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        #endregion Public Methods
    }
}