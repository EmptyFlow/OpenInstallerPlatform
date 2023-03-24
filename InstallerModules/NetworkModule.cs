using OpenInstallerPlatform.Modules;

namespace OpenInstallerPlatform.InstallerModules {

    public class NetworkModule {

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly LoggerModule _logger;

        public NetworkModule ( IHttpClientFactory httpClientFactory, LoggerModule logger ) {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException ( nameof ( httpClientFactory ) );
            _logger = logger ?? throw new ArgumentNullException ( nameof ( logger ) );
        }

        public async Task<string> GetAsString ( string url ) {
            try {
                return await _httpClientFactory.CreateClient ().GetStringAsync ( url );
            } catch ( Exception ex ) {
                _logger.Error ( "NetworkModule", $"Error while performing {url}: {ex.Message}" );
                return "";
            }
        }

    }

}
