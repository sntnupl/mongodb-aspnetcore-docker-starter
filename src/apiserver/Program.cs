using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MongoCore.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var domain = "0.0.0.0";
            var port = "5000";

            if (args.Length == 2) {
                domain = args[0];
                port = args[1];
            }
            else {
	        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("API_HOST"))) 
		    domain = Environment.GetEnvironmentVariable("API_HOST");
	        if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("API_PORT"))) 
		    domain = Environment.GetEnvironmentVariable("API_PORT");
	    }
            
            var server = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls($"http://{domain}:{port}")
                .UseStartup<Startup>()
                .Build();
            server.Run();
        }
    }
}
