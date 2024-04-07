using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace super.guacamole.web;

public abstract class RouteHandler
{
    private Task Fallback(HttpContextBase ctx)
    {
        ctx.Response.StatusCode = 405; // Method not allowed
        return ctx.Response.Send();
    }

    public virtual Task HandleGet(HttpContextBase ctx)
    {
        return Fallback(ctx);
    }

    public virtual Task HandlePost(HttpContextBase ctx)
    {
        return Fallback(ctx);
    }

    public virtual void Register(HostBuilder hostBuilder, String path)
    {
        hostBuilder.MapStaticRoute(HttpMethod.GET, path, HandleGet);
        hostBuilder.MapStaticRoute(HttpMethod.POST, path, HandlePost);
        
        foreach (var value in Enum.GetValues(typeof(HttpMethod)).Cast<HttpMethod>())
        {
            if (value == HttpMethod.GET || value == HttpMethod.POST)
            {
                continue;
            }
            hostBuilder.MapStaticRoute(value, path, Fallback);
        }
    }
}