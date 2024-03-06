using Microsoft.EntityFrameworkCore;
using TokenRefresh.Api.Data;

namespace TokenRefresh.Api.DataConfig
{
    public static class DataConfig
    {
        public static IServiceCollection AddDataConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services
             .AddDbContextPool<ApplicationDbContext>(opts => opts
             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
             .UseSqlServer(configuration.GetConnectionString("SQLConnection"), b => b
             .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            return services;
        }
    }
}
