using System.Text.Encodings.Web;
using Yafa.Api.Middleware;

namespace Yafa.Api.Extension
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");

            app.ConfigureExceptionMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
			app.UseMiddleware<ScalarAuthMiddleware>();
			app.MapControllers();
            return app;
        }

        public static WebApplication ConfigurScalarUI(this WebApplication app)
        {
            app.MapSwagger("/openapi/{documentName}.json");

            app.MapGet("/scalar-portal", (HttpContext ctx) =>
            {
                if (!(ctx.User?.Identity?.IsAuthenticated ?? false))
                    return Results.Redirect("/login.html");

                if (!ctx.User.IsInRole("Admin"))
                    return Results.Redirect("/login.html");

                var rawToken = ctx.Request.Cookies["yafa_auth"] ?? string.Empty;
                var safeToken = HtmlEncoder.Default.Encode(rawToken);

                return Results.Content(BuildScalarPortalHtml(), "text/html");
			});

			app.MapGet("/login", (HttpContext ctx) =>
			{
				if (ctx.User?.Identity?.IsAuthenticated ?? false)
                    return Results.Redirect("/scalar-portal");

                return Results.Redirect("/login.html");
			});

			app.MapGet("/", (HttpContext ctx) =>
			{
				if (ctx.User?.Identity?.IsAuthenticated ?? false)
                    return Results.Redirect("/scalar-portal");

                return Results.Redirect("/login.html");
			});
			return app;
        }

		private static string BuildScalarPortalHtml()
		{
			return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <title>YAFA API Documentation</title>

                <style>
                    body { margin: 0; padding: 0; }

                    #logout-btn {
                        position: fixed;
                        top: 14px;
                        right: 18px;
                        z-index: 9999;
                        background: rgba(30,32,48,.85);
                        border: 1px solid rgba(255,255,255,.12);
                        color: #94a3b8;
                        font-size: 12px;
                        font-family: system-ui, sans-serif;
                        padding: 5px 13px;
                        border-radius: 8px;
                        cursor: pointer;
                        backdrop-filter: blur(6px);
                        text-decoration: none;
                        transition: color .2s, border-color .2s;
                    }

                    #logout-btn:hover {
                        color: #e2e8f0;
                        border-color: rgba(255,255,255,.25);
                    }

                    #loading-overlay {
                        position: fixed; inset: 0; background: #0f1117;
                        display: flex; align-items: center; justify-content: center;
                        color: #475569; font-family: system-ui, sans-serif; font-size: 14px;
                        z-index: 8888;
                    }
                </style>
            </head>
            <body>
                <div id="loading-overlay">Loading API portal…</div>
                <div id="app"></div>

                <a href="#" id="logout-btn">&#x2192; Sign out</a>

                <script>
                    (function () {
                        var origin = window.location.origin;
                        var path = window.location.pathname.toLowerCase();
                        var pathPrefix = '';

                        if (path.startsWith('/yafa')) {
                            pathPrefix = '/yafa';
                        } else if (path.startsWith('/api')) {
                            pathPrefix = '/api';
                        }

                        var baseUrl = origin + pathPrefix;

                        var jqueryScript = document.createElement('script');
                        jqueryScript.src = baseUrl + '/swagger-ui/jquery-3.7.1.min.js';

                        jqueryScript.onload = function () {

                            // Load Scalar after jQuery
                            var scalarScript = document.createElement('script');
                            scalarScript.src = baseUrl + '/swagger-ui/scalar.js';

                            scalarScript.onload = function () {
                                initializeApp();
                            };

                            document.head.appendChild(scalarScript);
                        };

                        document.head.appendChild(jqueryScript);

                        function initializeApp() {
                            var refreshTimer = null;
                            var scalarInstance = null;

                            // ── Logout ───────────────────────────────────────────────────────
                            $('#logout-btn').on('click', function (e) {
                                e.preventDefault();
                                if (refreshTimer) clearInterval(refreshTimer);
            
                                $.ajax({
                                    url:  baseUrl + '/api/AdminAuth/logout',
                                    type: 'POST',
                                    xhrFields: { withCredentials: true },
                                    complete: function () {
                                        window.location.href = baseUrl + '/login';
                                    }
                                });
                            });

                            // ── Fetch short-lived Scalar token ───────────────────────────────
                            // APIResponse shape:
                            // { statusCode, isSuccess, errorMessages, result: { token: "eyJ..." } }
                            function fetchScalarToken(onSuccess) {
                                $.ajax({
                                    url:      baseUrl + '/api/AdminAuth/scalar-token',
                                    type:     'GET',
                                    xhrFields: { withCredentials: true },
                                    contentType: 'application/json',
                                    success: function (response) {
                                        // response.isSuccess guards against 200 with error body
                                        console.log('Scalar token response:', response);
                                        if (response.isSuccess && response.result.token) {
                                            onSuccess(response.result.token);
                                        } else {
                                            // Server returned 200 but isSuccess=false — treat as failure
                                            redirectToLogin();
                                        }
                                    },
            
                                    error: function (xhr) {
                                        // 401 = HttpOnly session expired / revoked → force re-login
                                        redirectToLogin();
                                    }
                                });
                            }

                            // ── Silent token refresh (called every 4 min) ────────────────────
                            function refreshScalarToken() {
                                fetchScalarToken(function (newToken) {
                                    var container = document.getElementById('app');
                                    container.innerHTML = '';          // wipe old instance

                                    scalarInstance = Scalar.createApiReference(container, {
                                        url:      baseUrl + '/openapi/v1.json',
                                        theme:    'moon',
                                        layout:   'modern',
                                        darkMode: true,
                                        authentication: {
                                            preferredSecurityScheme: 'bearer',
                                            securitySchemes: {
                                                bearer: { token: newToken }
                                            }
                                        }
                                    });
                                });
                            }

                            // ── Init ─────────────────────────────────────────────────────────
                            fetchScalarToken(function (token) {
                                $('#loading-overlay').hide();
            
                                var container = document.getElementById('app');
            
                                scalarInstance = Scalar.createApiReference(container, {
                                    url:      baseUrl + '/openapi/v1.json',
                                    theme:    'moon',
                                    layout:   'modern',
                                    darkMode: true,
                                    authentication: {
                                        preferredSecurityScheme: 'bearer',
                                        securitySchemes: {
                                            bearer: { token: token }
                                        }
                                    }
                                });
            
                                // Refresh every 2hours JWT expires
                                refreshTimer = setInterval(refreshScalarToken, 2 * 60 * 60 * 1000);
                            });
            
                            function redirectToLogin() {
                                if (refreshTimer) clearInterval(refreshTimer);
                                window.location.href = baseUrl + '/login';
                            }
                        }

                    })();
                </script>

            </body>
            </html>
            """;
		}
	}
}
