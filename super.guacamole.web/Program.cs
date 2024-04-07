using super.guacamole.common;
using super.guacamole.image;
using super.guacamole.image.Cache;
using super.guacamole.web.Routes;
using WatsonWebserver.Core;
using HostBuilder = WatsonWebserver.Extensions.HostBuilderExtension.HostBuilder;

namespace super.guacamole.web;

public static class Program
{
    private static readonly IAsyncCache<Guid, byte[]> SkinCache =
        new LruMemoryAsyncCache<Guid, byte[]>(10000, new SkinProvider()); // todo: DI

    private static readonly Configuration Configuration = new();

    private static readonly Dictionary<string, RouteHandler> Routes = new()
    {
        { "/skin/{uuid}", new SkinUuidRoute(SkinCache) },
        { "/avatar/{uuid}", new AvatarUuidRoute(SkinCache) }
    };

    public static void Main(string[] args)
    {
        var hostname = Configuration.Hostname;
        var port = Configuration.Port;
        var hostBuilder = new HostBuilder(hostname, port, false, DefaultRoute);

        foreach (var (path, handler) in Routes)
        {
            handler.Register(hostBuilder, path);
        }

        var server = hostBuilder.Build();
        server.Start();
        Console.WriteLine($"Listening on {hostname}:{port}");
        Console.ReadLine();
    }

    private static async Task DefaultRoute(HttpContextBase ctx) =>
        await ctx.Response.Send("Hello from the default route!");
}