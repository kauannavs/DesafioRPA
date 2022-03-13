using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace PokeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory+ "/log/PokeApi.txt", rollingInterval: RollingInterval.Infinite, fileSizeLimitBytes: 1048576)
            .CreateLogger();

            try
            {
                Log.Information("Iniciando Aplicação");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Aplicação parada por erro", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
