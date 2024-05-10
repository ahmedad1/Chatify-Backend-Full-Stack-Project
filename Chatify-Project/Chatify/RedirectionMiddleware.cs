using Microsoft.IdentityModel.Tokens;

namespace Chatify
{
    public class RedirectionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string ext=Path.GetExtension(context.Request.Path);
            if (context.Request.Path == "/")
                context.Response.Redirect("/index.html");
            else if (!context.Request.Path.StartsWithSegments("/api") && ext.IsNullOrEmpty()&&!context.Request.Path.Value.Contains("chat"))
            {
                context.Response.Redirect($"/index.html?route={context.Request.Path.Value.TrimStart('/')}");


            }
            else
                await next(context);
            
            
        }
    }
}
