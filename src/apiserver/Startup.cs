using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoCore.DbDriver;
using MongoCore.WebApi.Helpers;
using NLog.Web;

namespace MongoCore.WebApi
{
    public class Startup
    {
        private string _dirSettingsRoot = "settings";
        private string _dirNlogSettings = "logger-settings";
        private string _dirAppSettings = "app-settings";
        
        private string _nlogSettings = "nlog.sample.config";
        private string _appSettings = "settings.sample.json";
        
        private IConfiguration Configuration { get; set; }
        private AppConfig MongoAppConfig { get; set; }

        public Startup(IHostingEnvironment env)
        {
            LoadSettingsPaths(env);
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(AppConfig.CreateDefaultConfig().GenerateDictFacet())
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(_appSettings, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            Configuration = configBuilder.Build();
            MongoAppConfig = new AppConfig(Configuration);
            env.ConfigureNLog(_nlogSettings);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton<IAppConfig, AppConfig>();
            services.AddDatabaseServices(MongoAppConfig);
            services.AddAuthServices(MongoAppConfig);
            services.AddMvcServices(MongoAppConfig);
        }

        public void Configure(IApplicationBuilder app, 
                                IHostingEnvironment env, 
                                ILoggerFactory loggerFactory,
                                IDbInitializer dbSeeder)
        {
            app.UseCors(env.IsDevelopment() ? "AllowAnyOrigin" : "AllowSpecificOrigin");
            app.AddLogging(loggerFactory, env.IsDevelopment());
            app.AddNLogWeb();
            
            //app.UseIdentity();
            //app.AddJwt(LibraryAppConfig);
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = MongoAppConfig.TokenConfigs.Issuer,
                    ValidAudience = MongoAppConfig.TokenConfigs.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(MongoAppConfig.TokenConfigs.Key)),
                    ValidateLifetime = true
                }
            });

            app.UseMvc();
            
            dbSeeder.EnsureSeedData();
        }
        
        
        private void LoadSettingsPaths(IHostingEnvironment env)
        {
            var envName = string.IsNullOrEmpty(env.EnvironmentName) ? "development" : env.EnvironmentName.ToLower();
            
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SETTINGS_ROOTDIR")))
                _dirSettingsRoot = Environment.GetEnvironmentVariable("SETTINGS_ROOTDIR");
            
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SETTINGS_NLOG")))
                _dirNlogSettings = Environment.GetEnvironmentVariable("SETTINGS_NLOG");
            
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SETTINGS_APP")))
                _dirAppSettings = Environment.GetEnvironmentVariable("SETTINGS_APP");

            _nlogSettings = Path.Combine(env.ContentRootPath, _dirSettingsRoot, 
                _dirNlogSettings, $"nlog.{envName}.config");
            _appSettings = Path.Combine(env.ContentRootPath, _dirSettingsRoot, 
                _dirAppSettings, $"settings.{envName}.json");
        }
    }
}
