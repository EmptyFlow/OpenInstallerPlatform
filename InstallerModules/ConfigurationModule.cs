using OpenInstallerPlatform.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenInstallerPlatform.Modules {

    /// <summary>
    /// Configuration module contains variables and operations for save and restore state about instaler.
    /// </summary>
    public class ConfigurationModule {

        private string m_application = "";

        private string m_version = "";

        private string m_unique = "";

        private string m_baseFolder = "";

        private string m_configurationFile = "";

        private ConfigurationModel m_configurationModel = new();

        private readonly LoggerModule m_loggerModule;

        public ConfigurationModule ( LoggerModule loggerModule ) => m_loggerModule = loggerModule ?? throw new ArgumentNullException ( nameof ( loggerModule ) );

        private bool CreateDirectoryIfNotExists ( string path ) {
            try {
                if ( !Directory.Exists ( path ) ) Directory.CreateDirectory ( path );
                return true;
            } catch ( Exception exception ) {
                m_loggerModule.error ( "ConfigurationModule", $"Error while creating directory {path}. {exception.Message}" );
                return false;
            }
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public string application => m_application;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public string version => m_version;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public string unique => m_unique;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public string baseFolder => m_baseFolder;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public async Task<bool> configure ( string application, string version, string unique ) {
            if ( string.IsNullOrEmpty ( application ) ) {
                m_loggerModule.error ( "ConfigurationModule", "in method `configure` parameter `application` is required!" );
                return false;
            }
            if ( string.IsNullOrEmpty ( version ) ) {
                m_loggerModule.error ( "ConfigurationModule", "in method `configure` parameter `version` is required!" );
                return false;
            }
            if ( string.IsNullOrEmpty ( unique ) ) {
                m_loggerModule.error ( "ConfigurationModule", "in method `configure` parameter `unique` is required!" );
                return false;
            }

            m_application = application;
            m_version = version;
            m_unique = unique;

            m_baseFolder = Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.LocalApplicationData ), application, unique );
            m_configurationFile = Path.Combine ( m_baseFolder, "installerConfig" );

            if ( !CreateDirectoryIfNotExists ( m_baseFolder ) ) return false;

            if ( File.Exists ( m_configurationFile ) ) {
                var json = await File.ReadAllTextAsync ( m_configurationFile );
                m_configurationModel = JsonSerializer.Deserialize<ConfigurationModel> ( json ) ?? new ConfigurationModel ();
            }

#if !DEBUG
            if ( m_configurationModel.InstalledVersion == version ) {
                m_loggerModule.information ( "ConfigurationModule", $"application {application} with version {version} already installed" );
                return false;
            }
#endif

            m_loggerModule.information ( "ConfigurationModule", $"application {application} {version} ({unique}) configured" );
            return true;
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void addApplicationFolder ( string name, string folder ) {
            if ( m_configurationModel.ApplicationFolders.ContainsKey ( name ) ) {
                m_configurationModel.ApplicationFolders[name] = folder;
            } else {
                m_configurationModel.ApplicationFolders.Add ( name, folder );
            }
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void removeApplicationFolder ( string name ) {
            if ( !m_configurationModel.ApplicationFolders.ContainsKey ( name ) ) return;

            m_configurationModel.ApplicationFolders.Remove ( name );
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public string applicationFolder ( string name ) {
            if ( !m_configurationModel.ApplicationFolders.ContainsKey ( name ) ) return "";

            return m_configurationModel.ApplicationFolders[name];
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public bool isEmptyApplicationFolders () => !m_configurationModel.ApplicationFolders.Any();

        public async Task SaveConfiguration () {
            if ( string.IsNullOrEmpty ( m_configurationFile ) ) return;

            m_configurationModel = m_configurationModel with { InstalledVersion = m_version };

            await File.WriteAllTextAsync ( m_configurationFile, JsonSerializer.Serialize ( m_configurationModel ) );
        }

    }

}
