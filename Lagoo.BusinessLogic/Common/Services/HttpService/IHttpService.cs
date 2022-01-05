namespace Lagoo.BusinessLogic.Common.Services.HttpService;

/// <summary>
///   An interface for needed Http functionality
/// </summary>
public interface IHttpService
{
    Task<Stream> GetStreamAsync(string url);

    Task<TItem> GetAsync<TItem>(string url, IDictionary<string, string>? queryParams = null);

    Task<TItem> PostAsync<TItem>(string url, HttpContent content);

    void SetBearerToken(string token);
}