using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Extensions
{
    public static class IServiceExtensions
    {
        public static void AddServiceExtensions(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers();
            services.AddOpenApi();
            services.AddHttpContextAccessor();
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddHttpContextAccessor();
            services.ConfigureLocalizator();
            services.ConfigureCors(configuration);
            services.ConfigureDbContext(configuration);
            services.ConfigureIdentity();
            // services.ConfigureSerilog(configuration); Deprecated now it runs in the Host rather than in Services
            services.ConfigureAuthentication(configuration);
            services.ConfigureAuthorization();
            services.ConfigureDependencyInjection();
            services.ConfigureOptionsPattern(configuration);
        }
    }
}