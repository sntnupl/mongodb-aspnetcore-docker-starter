using System.Collections.Generic;

namespace MongoCore.WebApi.Helpers
{
    public class TokenConfigurations
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }
    
    public interface IAppConfig
    {
        string MongoDbConnectionUrl { get; set; }
        string ClientOrigin { get; set; }
        Dictionary<string, string> GenerateDictFacet();
        TokenConfigurations TokenConfigs { get; set; }
    }
}