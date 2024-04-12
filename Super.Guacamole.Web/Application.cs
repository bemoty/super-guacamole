using NLog;
using Super.Guacamole.Common;
using Super.Guacamole.Image.Cache;
using Super.Guacamole.Web.Routes;
using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;

namespace Super.Guacamole.Web;

public class Application(IAsyncCache<Guid, byte[]> skinCache) : IApplication
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly Dictionary<string, RouteHandler> _routes = new()
    {
        { "/skin/{uuid}", new SkinUuidRoute(skinCache) },
        { "/avatar/{uuid}", new AvatarUuidRoute(skinCache) }
    };

    public void Run()
    {
        var hostname = Configuration.Hostname;
        var port = Configuration.Port;
        var hostBuilder = new HostBuilder(hostname, port, false, DefaultRoute);

        foreach (var (path, handler) in _routes) handler.Register(hostBuilder, path);

        var server = hostBuilder.Build();
        server.Start();
        _logger.Info($"Listening on {hostname}:{port}");
        Console.ReadLine();
    }

    private static async Task DefaultRoute(HttpContextBase ctx)
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.Send();
    }
}

internal interface IApplication
{
    public void Run();
}