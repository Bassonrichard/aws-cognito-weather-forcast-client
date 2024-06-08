using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using AWSCognitoWeatherForcastClient.Configuration;
using AWSCognitoWeatherForcastClient.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AWSCognitoWeatherForcastClient.Services
{
    public interface ICommonApplicationService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForcast();
    }

    public class CommonApplicationService : ICommonApplicationService
    {
        private const string GRANT_TYPE = "client_credentials";
        private const string HTTP_CLIENT_NAME = "CommonApplication";
        private readonly AWSCognitoConfig _AWSCognitoConfig;
        private string _accessToken;
        private readonly IHttpClientFactory _httpClientFactory;

        public CommonApplicationService(
             IOptions<AWSCognitoConfig> AWSCognitoConfigOptions,
             IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _AWSCognitoConfig = AWSCognitoConfigOptions.Value;
        }

        private async Task GetToken()
        {
            var uri = new Uri(_AWSCognitoConfig.TokenUrl);
            var data = new Dictionary<string, string>
            {
                {"grant_type", GRANT_TYPE},
                {"scope", string.Join(" ", _AWSCognitoConfig.Scope)}
            };
            var content = new FormUrlEncodedContent(data);
            string requestToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(_AWSCognitoConfig.ClientId + ":" + _AWSCognitoConfig.ClientSecret));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + requestToken);
            var response = await client.PostAsync(uri, content);

            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<JWTToken>(jsonResponseBody);

            _accessToken = token.AccessToken;
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForcast()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                await GetToken();
            }
            var uri = new Uri("api/WeatherForecast", UriKind.Relative);

            var _httpClient = _httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            var response = await _httpClient.GetAsync(uri);

            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            var weatherForecast = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(jsonResponseBody);

            return weatherForecast;
        }
    }
}
