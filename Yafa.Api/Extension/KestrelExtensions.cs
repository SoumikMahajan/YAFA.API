using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Yafa.Api.Extension
{
    #pragma warning disable CS1591 // Disable XML comment warnings for this class
	public static class KestrelExtensions
    {
        public static IServiceCollection ConfigureKestrel(this IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
            });
            return services;
        }
    }
}
