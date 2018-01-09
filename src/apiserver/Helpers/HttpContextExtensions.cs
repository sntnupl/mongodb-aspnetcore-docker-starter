using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MongoCore.WebApi.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task HandleAppExceptions(this HttpContext ctxt, ILoggerFactory loggerFactory)
        {
            var exceptionHandlerFeature = ctxt.Features.Get<IExceptionHandlerFeature>();
            if (null != exceptionHandlerFeature) {
                var logger = loggerFactory.CreateLogger("Global Exception Logger");
                logger.LogError(500, exceptionHandlerFeature.Error.ToString(), exceptionHandlerFeature.Error.Message);
            }
            ctxt.Response.StatusCode = 500;
            await ctxt.Response.WriteAsync("An unexpected fault happened. Please try again later.");
        }
    }
}