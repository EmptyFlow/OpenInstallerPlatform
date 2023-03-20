namespace OpenInstallerPlatform.Models {

    internal record ConfigurationModel {

        public string InstalledVersion { get; init; } = "";

        public Dictionary<string, string> ApplicationFolders { get; init; } = new Dictionary<string, string> ();

    }

}
