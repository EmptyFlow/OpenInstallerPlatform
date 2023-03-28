using OpenInstallerPlatform.Modules;

namespace OpenInstallerPlatform.InstallerModules {

    public class NetworkModule {

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly LoggerModule _logger;

        public NetworkModule ( IHttpClientFactory httpClientFactory, LoggerModule logger ) {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException ( nameof ( httpClientFactory ) );
            _logger = logger ?? throw new ArgumentNullException ( nameof ( logger ) );
        }

        /// <summary>
        /// Make GET request and return result as string.
        /// </summary>
        /// <param name="url">URL for request.</param>
        /// <returns>Response as string.</returns>
        public async Task<string> GetAsString ( string url ) {
            try {
                return await _httpClientFactory.CreateClient ().GetStringAsync ( url );
            } catch ( Exception ex ) {
                _logger.Error ( "NetworkModule", $"GetAsString:Error while performing {url}: {ex.Message}" );
                return "";
            }
        }

        /// <summary>
        /// Make GET request and download result file to pathToSave folder.
        /// </summary>
        /// <param name="url">URL for request.</param>
        /// <param name="pathToSave">Path to save file.</param>
        public async Task<bool> GetDownloadFile ( string url, string pathToSave, string replaceFileName = "" ) {
            try {
                using var item = await _httpClientFactory.CreateClient ().GetAsync ( url );
                using var stream = await item.Content.ReadAsStreamAsync ();

                if ( !Directory.Exists ( pathToSave ) ) Directory.CreateDirectory ( pathToSave );

                if ( !string.IsNullOrEmpty ( replaceFileName ) ) pathToSave = Path.Combine ( pathToSave, replaceFileName );
                if ( string.IsNullOrEmpty ( replaceFileName ) ) {
                    if ( item.Content.Headers.Contains( "Content-Disposition" ) ) {
                        var value = item.Content.Headers.GetValues ( "Content-Disposition" ).FirstOrDefault ( a => a.Contains ( "filename" ) );
                        if ( value == null ) {
                            _logger.Error ( "NetworkModule", $"GetDownloadFile:Can't resolve file name from Content-Disposition header" );
                            return false;
                        }
                        value = value.Replace ( "filename=\"", "" ).Replace ( "\"", "" );
                        pathToSave = Path.Combine ( pathToSave, value ?? "" );
                    } else {
                        _logger.Error ( "NetworkModule", $"GetDownloadFile:Can't resolve file name, please set parameter replaceFileName" );
                        return false;
                    }
                }

                using var resultFile = File.OpenWrite ( pathToSave );
                await stream.CopyToAsync ( resultFile );
                return true;
            } catch ( Exception ex ) {
                _logger.Error ( "NetworkModule", $"GetDownloadFile:Error while downloading file {url}: {ex.Message}" );
                return false;
            }
        }

    }

}
