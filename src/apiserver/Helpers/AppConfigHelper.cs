using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace MongoCore.WebApi.Helpers
{
    public static class AppConfigHelper
    {
        public static void AddLogging(this IApplicationBuilder app, ILoggerFactory loggerFactory, bool isDevelopment)
        {
            if (isDevelopment) {
                loggerFactory.AddConsole().AddNLog();
                app.UseDeveloperExceptionPage();
            }
            else {
                loggerFactory.AddNLog();
                app.UseExceptionHandler(appBuilder => {
                    appBuilder.Run(async ctxt => {
                        await ctxt.HandleAppExceptions(loggerFactory);
                    });
                });
            }
        }
    }
}