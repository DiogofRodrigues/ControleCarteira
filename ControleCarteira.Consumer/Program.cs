using ControleCarteira.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ControleCarteira.Consumer;
public class Program
{
    public static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();

        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton(configuration);
                services.AddHostedService<Worker>();
                Startup.Configure(configuration, services, enableSwagger: false, enableRabbitMq: true);
            })
            .Build();

        host.Run();
    }
}