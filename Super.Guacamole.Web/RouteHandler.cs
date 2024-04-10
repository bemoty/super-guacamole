using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Super.Guacamole.Web;

public abstract class RouteHandler
{
    private async Task Fallback(HttpContextBase ctx)
    {
        ctx.Response.StatusCode = 405; // Method not allowed
        await ctx.Response.Send();
    }

    public virtual async Task HandleGet(HttpContextBase ctx)
    {
        await Fallback(ctx);
    }

    public virtual async Task HandlePost(HttpContextBase ctx)
    {
        await Fallback(ctx);
    }

    public virtual void Register(HostBuilder hostBuilder, string path)
    {
        hostBuilder.MapStaticRoute(HttpMethod.GET, path, HandleGet);
        hostBuilder.MapStaticRoute(HttpMethod.POST, path, HandlePost);

        RegisterFallbacks(hostBuilder, path);
    }

    protected void RegisterFallbacks(HostBuilder hostBuilder, string path)
    {
        foreach (var value in Enum.GetValues(typeof(HttpMethod)).Cast<HttpMethod>())
        {
            if (value is HttpMethod.GET or HttpMethod.POST)
            {
                continue;
            }

            hostBuilder.MapStaticRoute(value, path, Fallback);
        }
    }

    protected Dictionary<string, string> extractQueryParameters(string path)
    {
        var queryParameters = new Dictionary<string, string>();
        var query = path.Split('?');
        if (query.Length <= 1) return queryParameters;

        var parameters = query[1].Split('&');
        foreach (var parameter in parameters)
        {
            var keyValue = parameter.Split('=');
            if (keyValue.Length < 2)
            {
                queryParameters[keyValue[0]] = "";
            }
            else
            {
                queryParameters[keyValue[0]] = keyValue[1];
            }
        }

        return queryParameters;
    }
}