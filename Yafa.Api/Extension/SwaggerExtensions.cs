using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Yafa.Api.Extension
{
	#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public static class SwaggerExtensions
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
			services.AddSwaggerGen(options =>
			{              
                // Configure Swagger document
                options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Yafa API",
					Description = "Yafa API Documentation"
				});

				//// Include XML comments if available
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
				if (File.Exists(xmlPath))
				{
					options.IncludeXmlComments(xmlPath);
				}
			});
			return services;
        }
    }
}
