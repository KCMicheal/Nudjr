using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ExceptionModels;
using Nudjr_Domain.Models.ResposneModels;
using Newtonsoft.Json;
using System.Net;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> DoHttpGetAsync(string url, string authorizationToken, TokenType tokenType)
        {

            HttpClient client = _httpClientFactory.CreateClient();
            client.SetAuthorization(authorizationToken, tokenType);

            HttpResponseMessage response = await client.GetAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> DoHttpGetAsync(string url, Dictionary<string, string> headers)
        {
            HttpClient client = _httpClientFactory.CreateClient();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.GetAsync(url);
            return response;
        }

        public async Task<HttpResponseMessage> DoHttpPostAsync(string url, string authorizationToken, object requestBodyModel, TokenType tokenType)
        {
            //Serialaize and Deserialize Request Body To Convert Enums To String
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() }
            };

            string serializedBody = JsonConvert.SerializeObject(requestBodyModel, settings);

            HttpClient client = _httpClientFactory.CreateClient();
            client.SetAuthorization(authorizationToken, tokenType);

            StringContent httpContent = new StringContent(serializedBody, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, httpContent);
            return response;
        }


        public async Task<HttpResponseMessage> DoHttpPostAsync(string url)
        {
            //Serialaize and Deserialize Request Body To Convert Enums To String
            var requestBody = new object { };

            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsync(url, null);
            return response;
        }

        public async Task<HttpResponseMessage> DoHttpPostAsync(string url, object body, Dictionary<string, string> headers)
        {
            HttpClient client = _httpClientFactory.CreateClient();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            StringContent httpContent = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");

            if (body != null)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() }
                };
                string serializedBody = JsonConvert.SerializeObject(body, settings);


                httpContent = new StringContent(serializedBody, System.Text.Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await client.PostAsync(url, httpContent);

            return response;
        }

        public async Task<T> ParseNudjrApiResponse<T>(HttpResponseMessage response)
        {
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                T? model = JsonConvert.DeserializeObject<T>(responseString);
                return model;
            }
            else
            {
                ErrorDetails? errorModel = JsonConvert.DeserializeObject<ErrorDetails>(responseString);
                if (errorModel != null)
                    throw new NudjrAPIException(errorModel.Message);

                throw new InvalidOperationException("An Error Occurred While Sending Request.");
            }
        }
    }

    public static class HttpClientExtension
    {
        public static HttpClient SetAuthorization(this HttpClient httpClient, string token, TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Bearer:
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    break;
                default:
                    break;
            }
            return httpClient;
        }
    }
}
