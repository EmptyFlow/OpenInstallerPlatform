using OpenInstallerPlatform.InstallerModules;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace OpenInstallerPlatform.Modules {

    public class FilesModule {

        private readonly Assembly m_resourcesAssembly;

        private readonly string m_resourcesPrefix = "";

        private readonly LoggerModule m_loggerModule;

        public FilesModule ( LoggerModule loggerModule, Assembly assembly ) {
            m_resourcesAssembly = assembly;
            m_resourcesPrefix = m_resourcesAssembly.GetName ().Name + ".Content.";
            m_loggerModule = loggerModule ?? throw new ArgumentNullException ( nameof ( loggerModule ) );
        }

        private async Task<bool> CopyFileFromResource ( string resoursePath, string fileName ) {
            if ( string.IsNullOrEmpty ( resoursePath ) || string.IsNullOrEmpty ( fileName ) ) return false;

            try {
                var stream = m_resourcesAssembly.GetManifestResourceStream ( resoursePath );
                if ( stream == null ) return false;

                using var file = File.OpenWrite ( fileName );
                await stream.CopyToAsync ( file );
                return true;
            } catch ( Exception exception ) {
                m_loggerModule.error ( "FilesModule", $"Error while copying file {resoursePath} to {fileName}. {exception.Message}" );
                return false;
            }
        }

        private bool CreateDirectoryIfNotExists ( string path ) {
            if ( string.IsNullOrEmpty ( path ) ) return false;

            try {
                if ( !Directory.Exists ( path ) ) Directory.CreateDirectory ( path );
                return true;
            } catch ( Exception exception ) {
                m_loggerModule.error ( "FilesModule", $"Error while creating directory {path}. {exception.Message}" );
                return false;
            }
        }

        private (string, bool) GetEmbeddedFileName ( string resourcePath ) {
            if ( string.IsNullOrEmpty ( resourcePath ) ) return ("", false);

            var fileName = EmbeddedResources.GetFileName ( resourcePath, m_resourcesAssembly );
            if ( fileName == null ) return ("", false);

            return (fileName, true);
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public async Task<bool> copyFolder ( string folder, string finishFolder, bool createSameStructure = false ) {
            if ( !Directory.Exists ( finishFolder ) ) Directory.CreateDirectory ( finishFolder );

            var names = m_resourcesAssembly.GetManifestResourceNames ();

            var prefixPath = m_resourcesPrefix + folder + ".";

            foreach ( var name in names ) {
                if ( !name.StartsWith ( prefixPath ) ) continue;

                var (fileName, fileNameAccessed) = GetEmbeddedFileName ( name );
                if ( !fileNameAccessed ) break;

                var createDirectory = name.Replace ( m_resourcesPrefix, "" ).Replace ( fileName, "" ).Replace ( ".", "/" );
                if ( !createSameStructure && createDirectory.StartsWith ( folder ) ) createDirectory = createDirectory.Substring ( folder.Length + 1 );
                var itemPath = Path.Combine ( finishFolder, createDirectory );

                var directory = itemPath.Replace ( fileName, "" );
                if ( !CreateDirectoryIfNotExists ( directory ) ) break;

                itemPath = Path.Combine ( itemPath, fileName );
                if ( File.Exists ( itemPath ) ) File.Delete ( itemPath );
                if ( !await CopyFileFromResource ( name, itemPath ) ) break;

                m_loggerModule.information ( "FilesModule", $"Sucessfully copied file {itemPath}" );
            }

            m_loggerModule.information ( "FilesModule", $"Sucessfully performed `copyFolder` {folder} to {finishFolder}" );

            return true;
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public async Task<bool> copyFile ( string fileName, string targetFolder, string replaceFileName = "" ) {
            var resourcePath = m_resourcesPrefix + fileName.Replace ( "/", "." ).Replace ( "\\", "." );

            var (originalFileName, fileNameAccessed) = GetEmbeddedFileName ( resourcePath );
            if ( !fileNameAccessed ) return false;

            var itemPath = Path.Combine ( targetFolder, resourcePath.Replace ( m_resourcesPrefix, "" ).Replace ( originalFileName, "" ).Replace ( ".", "/" ) );

            var directory = itemPath.Replace ( originalFileName, "" );
            if ( !CreateDirectoryIfNotExists ( directory ) ) return false;

            if ( !string.IsNullOrEmpty ( replaceFileName ) ) itemPath = itemPath.Replace ( originalFileName, replaceFileName );

            if ( File.Exists ( itemPath ) ) File.Delete ( itemPath );
            if ( !await CopyFileFromResource ( resourcePath, itemPath ) ) return false;

            m_loggerModule.information ( "FilesModule", $"Sucessfully copied file {itemPath}" );
            return true;
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public async Task<string> readTextFile ( string fileName ) {
            var resourcePath = m_resourcesPrefix + fileName.Replace ( "/", "." ).Replace ( "\\", "." );

            var stream = m_resourcesAssembly.GetManifestResourceStream ( resourcePath );
            if ( stream == null ) {
                m_loggerModule.error ( "FilesModule", $"Error while open path {fileName} for read text content." );
                return "";
            }
            using var streamReader = new StreamReader ( stream );
            return await streamReader.ReadToEndAsync ();
        }

    }

}
