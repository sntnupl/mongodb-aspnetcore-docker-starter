using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using MongoCore.DbDriver;

namespace MongoCore.WebApi.Helpers
{
    public static class ServiceConfigHelper
    {
        public static void AddCorsServiceAndPolicies(this IServiceCollection services, IAppConfig config)
        {
            services.AddCors(options => {
                options.AddPolicy("AllowAnyOrigin",
                    builder => {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                options.AddPolicy("AllowSpecificOrigin",
                    builder => {
                        builder.WithOrigins(config.ClientOrigin)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
        }
        
        public static void AddMvcServices(this IServiceCollection services, IAppConfig config)
        {
            services.AddMvc(options => {
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                options.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
                //options.ReturnHttpNotAcceptable = true;
                //options.Filters.Add(new RequireHttpsAttribute());
            });
        }

        public static void AddDatabaseServices(this IServiceCollection services, IAppConfig config)
        {
            services.AddTransient<IDbInitializer>(service => new MongoDbInitializer(config.MongoDbConnectionUrl));
            services.AddSingleton<IUserManager>(service => new UserManager(config.MongoDbConnectionUrl));
            services.AddSingleton<IRepository>(service => new AppRepository(config.MongoDbConnectionUrl));
        }
        
        public static void AddAuthServices(this IServiceCollection services, IAppConfig config)
        {
            //services.AddSingleton<ITokenProvider>(tokenProvider);
            
            //SetupCustomValidators(services);
            //SetupPolicies(services);
            //SetupDatabaseContexts(services, config);
            //SetupIdentity(services);
        }
    }
}