using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Yafa.Application.Common.Utility;

namespace Yafa.Api.Extension
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var key = config.GetValue<string>("ApiSettings:Secret")!;
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = secretKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
						RoleClaimType = ClaimTypes.Role,
						NameClaimType = ClaimTypes.Name
					};
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
							string? token = null;

							// 1. Try Authorization header first (for APIs / Postman)
							var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

							if (!string.IsNullOrEmpty(authHeader))
							{
								if (authHeader.StartsWith("Bearer "))
									token = authHeader.Substring("Bearer ".Length).Trim();
								else
									token = authHeader;
							}

							// 2. If no header → fallback to cookie (for Scalar UI / browser)
							if (string.IsNullOrEmpty(token))
							{
								token = context.Request.Cookies["yafa_auth"] ?? "";
							}

							context.Token = token;

							return Task.CompletedTask;
						},
                        OnChallenge = async context =>
                        {
                            context.HandleResponse(); // suppress default 401

                            var response = new APIResponse
                            {
                                StatusCode = HttpStatusCode.Unauthorized,
                                IsSuccess = false,
                                ErrorMessages = new List<string>
                                {
                                    "Unauthorized. Token is missing, invalid, or expired."
                                }
                            };

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var json = JsonSerializer.Serialize(response);
                            await context.Response.WriteAsync(json);
                        },

                        OnForbidden = async context =>
                        {
                            var response = new APIResponse
                            {
                                StatusCode = HttpStatusCode.Forbidden,
                                IsSuccess = false,
                                ErrorMessages = new List<string>
                                {
                                    "Forbidden. You do not have permission to access this resource."
                                }
                            };

                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var json = JsonSerializer.Serialize(response);
                            await context.Response.WriteAsync(json);
                        }
                    };
                });

            return services;
        }
    }
}
