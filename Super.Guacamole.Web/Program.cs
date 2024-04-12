using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Super.Guacamole.Image;
using Super.Guacamole.Image.Cache;

namespace Super.Guacamole.Web;

public static class Program
{
    public static void Main()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        ServiceCollection services = [];
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog(config);
        });
        services.AddSingleton<IProvider<Guid, byte[]>, SkinProvider>();
        services.AddSingleton<IAsyncCache<Guid, byte[]>, LruMemoryAsyncCache<Guid, byte[]>>();
        services.AddSingleton<IApplication, Application>();

        using var provider = services.BuildServiceProvider();
        var app = provider.GetRequiredService<IApplication>();
        app.Run();
    }
}