namespace OpenInstallerPlatform.InstallerModules {

    public class NetworkModule {

        readonly HttpClient m_client = new();

        public async Task<string> GetAsString ( string url ) {
            return await m_client.GetStringAsync ( url );
        }

    }

}
