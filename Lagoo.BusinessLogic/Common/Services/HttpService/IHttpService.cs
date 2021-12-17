namespace Lagoo.BusinessLogic.Common.Services.HttpService;

public interface IHttpService
{
    Task<Stream> GetStreamAsync(string url);

    Task<TItem> GetAsync<TItem>(string url, IDictionary<string, string>? queryParams = null);

    void SetBearerToken(string token);
}