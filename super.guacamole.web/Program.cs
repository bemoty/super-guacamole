using WatsonWebserver;
using WatsonWebserver.Core;

namespace super.guacamole.web;

public static class Program
{
    public static void Main(string[] args)
    {
        Webserver server = new Webserver(new WebserverSettings("127.0.0.1", 3000), DefaultRoute);
        server.Start();
        Console.WriteLine("Listening on 127.0.0.1:3000");
        Console.ReadLine();
    }

    static async Task DefaultRoute(HttpContextBase ctx) =>
        await ctx.Response.Send("Hello from the default route!");
}
