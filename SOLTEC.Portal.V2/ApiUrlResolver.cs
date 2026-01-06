using System.Net;

public class ApiUrlResolver
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiUrlResolver(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string?> GetActiveApiUrlAsync()
    {
        var urls = _config.GetSection("ApiSettings:Urls").Get<List<string>>();
        var client = _httpClientFactory.CreateClient();

        foreach (var url in urls)
        {
            try
            {
                var testUrl = new Uri(new Uri(url), "/api/transmision/isOnline"); 
                var response = await client.GetAsync(testUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return url; 
                }
            }
            catch
            {
                // Ignora y sigue con la siguiente URL
            }
        }

        return null; // Ninguna funcionó
    }
}
