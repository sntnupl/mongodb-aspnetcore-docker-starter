using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MongoCore.WebApi.Helpers
{
    public class AppConfig: IAppConfig
    {
        public string MongoDbConnectionUrl { get; set; }
        public string ClientOrigin { get; set; }
        public TokenConfigurations TokenConfigs { get; set; }

        public AppConfig(IConfiguration configuration)
        {
            InitToDefault();
            UpdateConfigFromSettings();
            UpdateConfigFromEnv();
        }

        private void UpdateConfigFromSettings() 
        {
            if (!string.IsNullOrEmpty(configuration["ConnectionStrings:MongoDbConnectionUrl"])) 
                MongoDbConnectionUrl = configuration["ConnectionStrings:MongoDbConnectionUrl"];
            if (!string.IsNullOrEmpty(configuration["ClientOrigin"]))
                ClientOrigin = configuration["ClientOrigin"];
            if (!string.IsNullOrEmpty(configuration["Tokens:Issuer"]))
                TokenConfigs.Issuer = configuration["Tokens:Issuer"];
            if (!string.IsNullOrEmpty(configuration["Tokens:Audience"]))
                TokenConfigs.Audience = configuration["Tokens:Audience"];
            if (!string.IsNullOrEmpty(configuration["Tokens:SecretKey"]))
                TokenConfigs.Key = configuration["Tokens:SecretKey"];
        }

	private void UpdateConfigFromEnv() 
	{
	    if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGODB_URL")))
	        MongoDbConnectionUrl = Environment.GetEnvironmentVariable("MONGODB_URL");
	}
        
        public static AppConfig CreateDefaultConfig()
        {
            return new AppConfig();
        }
        
        public Dictionary<string, string> GenerateDictFacet()
        {
            return new Dictionary<string, string>
            {
                {"ConnectionStrings:MongoDbConnectionUrl", MongoDbConnectionUrl},
                {"ClientOrigin", ClientOrigin},
                {"Tokens:Issuer", TokenConfigs.Issuer},
                {"Tokens:Audience", TokenConfigs.Audience},
                {"Tokens:SecretKey", TokenConfigs.Key},  
            };
        }
        
        private AppConfig()
        {
            InitToDefault();
        }

        private void InitToDefault()
        {
            MongoDbConnectionUrl = "mongodb://localhost:27017";
            ClientOrigin = "";
            
            TokenConfigs = new TokenConfigurations
            {
                Issuer = "www.default.com",
                Audience = "www.default.com",
                Key = "secret-key"
            };
        }
    }
}
