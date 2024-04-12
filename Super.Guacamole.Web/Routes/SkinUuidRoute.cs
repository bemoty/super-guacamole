using Super.Guacamole.Common;
using Super.Guacamole.Image.Cache;
using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Super.Guacamole.Web.Routes;

public class SkinUuidRoute(IAsyncCache<Guid, byte[]> skinCache) : RouteHandler
{
    public override async Task HandleGet(HttpContextBase ctx)
    {
        var uuid = ctx.Request.Url.Parameters["uuid"];
        if (uuid == null)
        {
            ctx.Response.StatusCode = 400;
            await ctx.Response.Send("Missing UUID parameter");
        }
        else
        {
            try
            {
                var guid = Guid.Parse(uuid);
                var texture = await skinCache.Get(guid);
                ctx.Response.Headers["Content-Type"] = "image/png";
                ctx.Response.Headers["Cache-Control"] = $"public, max-age={Configuration.CacheTimeSeconds}";
                await ctx.Response.Send(texture);
            }
            catch (FormatException)
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.Send("Invalid UUID format");
            }
        }
    }

    public override void Register(HostBuilder hostBuilder, string path)
    {
        hostBuilder.MapParameteRoute(HttpMethod.GET, path, HandleGet);
        RegisterFallbacks(hostBuilder, path);
    }
}