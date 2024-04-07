using super.guacamole.common;
using super.guacamole.image;
using super.guacamole.web.Routes;
using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;

namespace super.guacamole.web;

public static class Program
{
    private static readonly Configuration Configuration = new();
    
    private static readonly Dictionary<string, RouteHandler> Routes = new()
    {
        {"/test", new TestRoute()}
    };
    
    public static void Main(string[] args)
    {
        var hostname = Configuration.Hostname;
        var port = Configuration.Port;
        var builder = new HostBuilder(hostname, port, false, DefaultRoute);
        
        foreach (var (path, handler) in Routes)
        {
            handler.Register(builder, path);
        }
        
        SkinProvider skinProvider = new();
        var texture = skinProvider.Provide(Guid.Parse("951b54fc-6189-4fbd-8b9c-affd70ab8449"));
        Console.WriteLine(texture);

        var server = builder.Build();
        server.Start();
        Console.WriteLine($"Listening on {hostname}:{port}");
        Console.ReadLine();
    }

    private static async Task DefaultRoute(HttpContextBase ctx) =>
        await ctx.Response.Send("Hello from the default route!");
}
