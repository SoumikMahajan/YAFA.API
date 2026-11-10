namespace Yafa.Api.Middleware;

#pragma warning disable CS1591
public class ScalarAuthMiddleware : IMiddleware
{
    private static readonly string[] ProtectedPrefixes =
    [
        "/scalar-portal",
        "/openapi"
    ];
   

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "/";

        bool isProtected = ProtectedPrefixes.Any(p => path.StartsWith(p, StringComparison.Ordinal));

		if (isProtected)
		{
            if (!(context.User?.Identity?.IsAuthenticated ?? false) || !context.User.IsInRole("Admin"))
            {
                RedirectToLogin(context); // FIX 2: correct cookie delete with Path
                return;
            }
        }

        await next(context);
    }

    private static void RedirectToLogin(HttpContext context)
    {
        context.Response.Cookies.Delete("yafa_auth", new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        context.Response.Redirect("/login.html", permanent: false);
    }
}


//public class ScalarAuthMiddleware
//{
//    private readonly RequestDelegate _next;

//    public ScalarAuthMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        var path = context.Request.Path.Value?.ToLower();

//        if (path != null && (path.StartsWith("/scalar-portal") || path.StartsWith("/openapi")))
//        {
//            if (!(context.User?.Identity?.IsAuthenticated ?? false))
//            {
//                Redirect(context);
//                return;
//            }

//            if (!context.User.IsInRole("Admin"))
//            {
//                Redirect(context);
//                return;
//            }

//        }

//        await _next(context);
//    }

//    private void Redirect(HttpContext context)
//    {
//        context.Response.Cookies.Delete("yafa_auth");
//        context.Response.Redirect("/login.html");
//    }
//}

