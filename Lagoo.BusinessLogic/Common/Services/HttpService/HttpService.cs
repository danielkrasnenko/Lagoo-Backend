using System.Net.Http.Headers;
using System.Text.Json;
using Lagoo.BusinessLogic.Common.Exceptions.Base;

namespace Lagoo.BusinessLogic.Common.Services.HttpService;

/// <summary>
///  Wrapper over HttpClient with additional functionality
/// </summary>
public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Stream> GetStreamAsync(string url) => _httpClient.GetStreamAsync(url);

    public async Task<TItem> GetAsync<TItem>(string url, IDictionary<string, string>? queryParams = null)
    {
        var query = queryParams is null
            ? url
            : await GenerateUrlWithQueryParams(url, queryParams);

        var response = await _httpClient.GetAsync(query);

        return await DeserializeHttpResponseAsync<TItem>(response);
    }

    public void SetBearerToken(string token) => _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    private static async Task<string> GenerateUrlWithQueryParams(string url, IDictionary<string, string> queryParams)
    {
        if (queryParams.Count == 0)
        {
            return url;
        }

        using var urlContent = new FormUrlEncodedContent(queryParams);
        return $"{url}?{await urlContent.ReadAsStringAsync()}";
    }

    private static async Task<TItem> DeserializeHttpResponseAsync<TItem>(HttpResponseMessage responseMessage)
    {
        var responseString = await responseMessage.Content.ReadAsStringAsync();
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new HttpException(responseString);
        }

        var result = JsonSerializer.Deserialize<TItem>(responseString);

        if (result is null)
        {
            throw new InvalidCastException();
        }

        return result;
    }
}