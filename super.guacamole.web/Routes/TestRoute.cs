using WatsonWebserver.Core;

namespace super.guacamole.web.Routes;

public class TestRoute : RouteHandler
{
    public override Task HandleGet(HttpContextBase ctx)
    {
        return ctx.Response.Send("Hello from the TestRoute!");
    }
}