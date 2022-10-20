using System.Net.Http.Headers;
using System.Text.Json;
using Lagoo.BusinessLogic.Common.Exceptions.Base;

namespace Lagoo.BusinessLogic.Common.Services.HttpService;

/// <summary>
///   Wrapper over HttpClient with additional functionality
/// </summary>
public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///   Send a GET request to the specified Url and
    ///   return the response body as a stream in an asynchronous operation
    /// </summary>
    /// <param name="url">The Url the request is sent to</param>
    /// <returns>The task object representing the asynchronous operation</returns>
    public Task<Stream> GetStreamAsync(string url) => _httpClient.GetStreamAsync(url);

    /// <summary>
    ///   Send a GET request to the specified Url as an asynchronous operation 
    /// </summary>
    /// <param name="url">The Url the request is sent to</param>
    /// <param name="queryParams">Dictionary of Query String Parameters for the request</param>
    /// <typeparam name="TItem">Type for deserialization</typeparam>
    /// <returns>Serialized requested data</returns>
    public async Task<TItem> GetAsync<TItem>(string url, IDictionary<string, string>? queryParams = null)
    {
        var query = queryParams is null
            ? url
            : await GenerateUrlWithQueryParamsAsync(url, queryParams);

        var response = await _httpClient.GetAsync(query);

        return await DeserializeHttpResponseAsync<TItem>(response);
    }

    /// <summary>
    ///   Send a POST request to the specified Url as an asynchronous operation
    /// </summary>
    /// <param name="url">The Url the request is sent to</param>
    /// <param name="content">The HTTP request content sent to the server</param>
    /// <typeparam name="TItem">Type for deserialization</typeparam>
    /// <returns>Serialized requested data</returns>
    public async Task<TItem> PostAsync<TItem>(string url, HttpContent content)
    {
        var response = await _httpClient.PostAsync(url, content);

        return await DeserializeHttpResponseAsync<TItem>(response);
    }

    /// <summary>
    ///   Sets a bearer token to the request
    /// </summary>
    /// <param name="token">Bearer token to set to the request</param>
    public void SetBearerToken(string token) => _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    private static async Task<string> GenerateUrlWithQueryParamsAsync(string url, IDictionary<string, string> queryParams)
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