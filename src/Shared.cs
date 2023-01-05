namespace ModManager;

/// <summary>
/// This class contains objects that are reused.
/// </summary>
public static class Shared
{
    /// <summary>
    /// Backing HTTP Client, allows us to lazily load the HttpClient.
    /// </summary>
    private static HttpClient? client;
    /// <summary>
    /// The configuration of the Mod Manager.
    /// </summary>
    public static Configuration? ApplicationConfiguration { get; set; }
    /// <summary>
    /// The client used to make HTTP requests.
    /// </summary>
    public static HttpClient HttpClient
    {
        get
        {
            // Check if client is null, if it is, initialize it.
            return client ??= new(new HttpClientHandler()
            {
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                UseCookies = false,
                UseProxy = false
            });
        }
    }
}