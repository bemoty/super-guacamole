using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Super.Guacamole.Image.Cache;
using WatsonWebserver.Core;
using WatsonWebserver.Extensions.HostBuilderExtension;
using Configuration = Super.Guacamole.Common.Configuration;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Super.Guacamole.Web.Routes;

public class AvatarUuidRoute(IAsyncCache<Guid, byte[]> skinCache) : RouteHandler
{
    public override async Task HandleGet(HttpContextBase ctx)
    {
        var uuid = ctx.Request.Url.Parameters["uuid"];
        var queryParameters = extractQueryParameters(ctx.Request.Url.RawWithQuery);

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

                using var image = await SixLabors.ImageSharp.Image.LoadAsync(new MemoryStream(texture));
                using var head = image.Clone(i => i.Crop(new Rectangle(8, 8, 8, 8)));

                // Add helm according to query
                if (queryParameters.ContainsKey("helm"))
                {
                    var helm = image.Clone(i => i.Crop(new Rectangle(40, 8, 8, 8)));
                    head.Mutate(i =>
                    {
                        i.DrawImage(helm, 1f);
                        helm.Dispose();
                    });
                }

                // Resize the head according to query
                if (queryParameters.TryGetValue("size", out var rawSize))
                    try
                    {
                        var size = int.Parse(rawSize);
                        head.Mutate(i => i.Resize(size, size, new BoxResampler()));
                    }
                    catch (FormatException)
                    {
                        ctx.Response.StatusCode = 400;
                        await ctx.Response.Send("Invalid size format");
                        return;
                    }

                using var outputStream = new MemoryStream();
                await head.SaveAsync(outputStream, new WebpEncoder());
                ctx.Response.Headers["Content-Type"] = "image/webp";
                ctx.Response.Headers["Cache-Control"] = $"public, max-age={Configuration.CacheTimeSeconds}";
                await ctx.Response.Send(outputStream.ToArray());
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