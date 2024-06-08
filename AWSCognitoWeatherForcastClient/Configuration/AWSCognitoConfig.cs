using System.Collections.Generic;

namespace AWSCognitoWeatherForcastClient.Configuration
{
    public class AWSCognitoConfig
    {
        public const string Name = "AWSCognito";

        public string TokenUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public IEnumerable<string> Scope { get; set; }
    }
}
