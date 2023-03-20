using Microsoft.Extensions.FileProviders;

namespace OpenInstallerPlatform {

    internal static class EmbeddedResources {

        private static readonly Dictionary<string, string> m_fileNames = new Dictionary<string, string>();

        public static string GetFileName(string resourcePath) => m_fileNames.ContainsKey( resourcePath ) ? m_fileNames[resourcePath] : "";

        static EmbeddedResources () {
            var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider ( typeof ( InstallerPerformer ).Assembly );
            var folder = manifestEmbeddedProvider.GetDirectoryContents ( "Content" );

            var directoryProperty = folder.GetType ().GetProperty ( "Directory" );
            var manifestDirectory = directoryProperty?.GetGetMethod ()?.Invoke ( folder, null );

            if ( manifestDirectory != null ) ProcessFolder ( manifestDirectory );

            dynamic? GetPropertyValue ( dynamic value, string property ) {
                var directoryProperty = value.GetType ().GetProperty ( property );
                return directoryProperty?.GetGetMethod ()?.Invoke ( value, null );
            }

            bool HasProperty ( dynamic value, string property ) => value.GetType ().GetProperty ( property ) != null;

            void ProcessFolder ( dynamic directory ) {
                if ( m_fileNames == null ) return;

                var children = GetPropertyValue ( directory, "Children" );
                if ( children == null ) return;

                foreach ( var content in children ) {
                    if ( HasProperty ( content, "Children" ) ) {
                        ProcessFolder ( content );
                        continue;
                    }
                    var resourcePath = GetPropertyValue ( content, "ResourcePath" );
                    if ( !m_fileNames.ContainsKey ( resourcePath ) ) m_fileNames.Add ( resourcePath, GetPropertyValue ( content, "Name" ) );
                }
            }
        }

    }

}
