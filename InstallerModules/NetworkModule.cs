
namespace OpenInstallerPlatform.InstallerModules {

    public class NetworkModule {
        
        private readonly IHttpClientFactory _httpClientFactory;

        public NetworkModule( IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        public async Task<string> GetAsString ( string url ) {
            return await _httpClientFactory.CreateClient().GetStringAsync ( url );
        }

    }

}
