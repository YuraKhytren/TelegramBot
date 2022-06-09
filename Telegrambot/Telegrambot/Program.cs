using Microsoft.Extensions.DependencyInjection;
using Telegrambot;
using Telegrambot.Service;

public class Program
{

    public static IServiceCollection ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IService, Service>();
        return serviceCollection;

    }
    public static async Task Main()
    {
        var services = ConfigureServices();
        var serviceProvider = services.BuildServiceProvider();

        Bot bot = new Bot(serviceProvider.GetService<IService>());

        await bot.StartProgram();
    }
}


