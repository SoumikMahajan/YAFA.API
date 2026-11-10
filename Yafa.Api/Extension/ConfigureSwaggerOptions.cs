using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Yafa.Api.Extension;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
        {
            //Type = SecuritySchemeType.ApiKey,
            //In = ParameterLocation.Header,
            //Name = "Authorization",
            //Description = "Enter your token directly (no Bearer prefix)",
            //BearerFormat = "JWT",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",         
            Description = "Enter your token directly (no Bearer prefix)",
            BearerFormat = "JWT",
        });
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });

		options.OperationFilter<SecurityRequirementsOperationFilter>();
		//options.OperationFilter<RequireAllQueryParamsFilter>();
	}

	public class SecurityRequirementsOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			// Check if the endpoint has [AllowAnonymous] attribute
			var hasAllowAnonymous = context.MethodInfo.DeclaringType != null &&
				(context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>().Any() ||
				 context.MethodInfo.GetCustomAttributes(true).OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>().Any());

			// If it has [AllowAnonymous], don't require authentication
			if (hasAllowAnonymous)
			{
				operation.Security?.Clear();
			}
		}
	}

	//public class RequireAllQueryParamsFilter : IOperationFilter
	//{
	//	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	//	{
	//		foreach (var param in operation.Parameters ?? [])
	//		{
	//			if (operation.Parameters == null) return;

	//			var nonNullableParamNames = context.MethodInfo.GetParameters()
	//			.Where(p => Nullable.GetUnderlyingType(p.ParameterType) == null
	//						&& p.ParameterType != typeof(string))
	//			.Select(p => p.Name)
	//			.ToHashSet(StringComparer.OrdinalIgnoreCase);

	//			if (param.In == ParameterLocation.Query && nonNullableParamNames.Contains(param.Name) && param is OpenApiParameter concreteParam)
	//			{
	//				concreteParam.Required = true;
	//			}

	//		}
	//	}
	//}
}
