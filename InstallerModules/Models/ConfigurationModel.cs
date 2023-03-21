namespace OpenInstallerPlatform.Models {

    public record ConfigurationModel {

        public string InstalledVersion { get; init; } = "";

        public Dictionary<string, string> Variables { get; init; } = new Dictionary<string, string>();

        public Dictionary<string, string> ApplicationFolders { get; init; } = new Dictionary<string, string> ();

    }

}
