using Hangfire.Annotations;
using Hangfire.Dashboard;

#pragma warning disable CS1591 // The event 'HangfireAuthFilter.OnAuthorization' is never used

namespace Yafa.Api.Extension
{	
	public class HangfireAdminAuthFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize(DashboardContext context)
		{
			var httpContext = context.GetHttpContext();

			var Isauth = httpContext.User.Identity?.IsAuthenticated;
			var IsAdmin = httpContext.User.IsInRole("Admin");

			// Same cookie your Scalar portal uses — already authenticated
			return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole("Admin");
		}
	}
}
