using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Yafa.Api.Middleware;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos;
using Yafa.Domain.Dtos.Payments;
using Yafa.Domain.Validator;
using Yafa.Infrastructure.Repository;

namespace Yafa.Api.Extension
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration config)
		{
			var path = Path.Combine("wwwroot", "firebase", "yafa-service-account.json");

			var credential = CredentialFactory
				.FromFile<ServiceAccountCredential>(path);

			if (FirebaseApp.DefaultInstance == null)
			{
				FirebaseApp.Create(new AppOptions
				{
					Credential = credential.ToGoogleCredential()
				});
			}

			services.AddHttpContextAccessor();
            services.AddHttpClient("GoogleImages");
            services.Configure<LocalMailSettings>(config.GetSection("LocalMailSettings"));
			services.Configure<StripeSettings>(config.GetSection("Stripe"));
			services.AddTransient<ExceptionHandling>();
            services.AddTransient<ScalarAuthMiddleware>();
            services.AddSingleton(config);
			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<ApiResponseService>();
			services.AddScoped<ILogService, LogService>();
			services.AddScoped<IFileRepo, FileService>();
			services.AddScoped<IFirebaseService, FirebaseService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			// Fluent Validation
			services.AddValidatorsFromAssembly(typeof(ValidatorAssemblyMarker).Assembly);
			services.AddControllers(options => options.Filters.Add<ValidationFilter>());
			services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
			// CORS setup
			services.AddCors(options =>
			{
				options.AddPolicy("AllowReactApp", policy =>
				{
					policy
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
				});
			});

			return services;
		}

		public static IServiceCollection RegisterInfrastructure(this IServiceCollection services)
		{
			// If you later want to register repos or other infra services, extend here
			return services;
		}
	}
}
