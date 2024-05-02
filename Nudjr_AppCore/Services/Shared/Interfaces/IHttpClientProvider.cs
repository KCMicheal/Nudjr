using Nudjr_Domain.Enums;

namespace Nudjr_AppCore.Services.Shared.Interfaces
{
    public interface IHttpClientProvider
    {
        Task<HttpResponseMessage> DoHttpGetAsync(string url, string authorizationToken, TokenType tokenType);
        Task<HttpResponseMessage> DoHttpGetAsync(string url, Dictionary<string, string> headers);
        Task<HttpResponseMessage> DoHttpPostAsync(string url, string authorizationToken, object requestBodyModel, TokenType tokenType);
        Task<HttpResponseMessage> DoHttpPostAsync(string url);
        Task<HttpResponseMessage> DoHttpPostAsync(string url, object body, Dictionary<string, string> headers);
        Task<T> ParseNudjrApiResponse<T>(HttpResponseMessage response);
    }
}
