using Microsoft.EntityFrameworkCore;
using Yafa.Infrastructure.Data;

namespace Yafa.Api.Extension
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public static class DatabaseExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            Dapper.SqlMapper.AddTypeHandler(new NullableDateOnlyTypeHandler());
            return services;
        }
    }
}
