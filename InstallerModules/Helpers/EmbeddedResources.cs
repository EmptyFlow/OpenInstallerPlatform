using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace OpenInstallerPlatform.InstallerModules {

    public static class EmbeddedResources {

        private static readonly Dictionary<string, string> m_fileNames = new();

        public static string GetFileName ( string resourcePath, Assembly assembly ) {
            if ( !m_fileNames.Any () ) FillFileNames ( assembly );

            return m_fileNames.TryGetValue ( resourcePath, out var value ) ? value : "";
        }

        private static void FillFileNames ( Assembly assembly ) {
            var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider ( assembly );
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
