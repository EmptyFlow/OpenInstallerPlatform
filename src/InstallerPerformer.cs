using Jint;
using Jint.Native;
using OpenInstallerPlatform.Modules;
using System.Reflection;

namespace OpenInstallerPlatform {

    internal static class InstallerPerformer {

        private static bool Finished = false;

        private static readonly ConfigurationModule m_configurationModule = new( LoggerModule.Instance );

        public static async Task RunInstaller () {

            var engine = new Engine (
                    options => {
                        options.EnableModules ( new ResourcesModuleLoader () );
                        options.CatchClrExceptions ();
                    }
            );

            AddModules ( engine );

            var installerModule = engine.ImportModule ( "./installer.js" );

            var promise = installerModule.Get ( "default" ).Call ();
            if ( promise.IsPromise () ) {
                var stateGetMethod = GetStateMethod ( promise );

                while ( true ) {
                    await Task.Delay ( 1000 );

                    var state = stateGetMethod?.Invoke ( promise, null );
                    if ( state != null ) {
                        var isRejected = (int) state == 2;
                        var isCompleted = (int) state == 3;
                        if ( isRejected ) {
                            //TODO: get error message
                            LoggerModule.Instance.error ( "RunInstaller", "main promise is rejected!" );
                            return;
                        }
                        break;
                    }

                    if ( Finished ) break;
                }
            }

            await m_configurationModule.SaveConfiguration ();
        }

        private static void AddModules ( Engine engine ) {
            engine.AddModule (
                "installer",
                builder => builder
                    .ExportFunction ( "finishInstall", () => Finished = true )
                    .ExportObject ( "Configuration", m_configurationModule )
                    .ExportObject ( "Logger", LoggerModule.Instance )
                    .ExportObject ( "FileSystem", new FilesModule ( LoggerModule.Instance, Assembly.GetExecutingAssembly () ) )
                    .ExportObject ( "Environment", new EnvironmentModule () )
            );
        }

        /// <summary>
        /// Sorry for ugly code, I try to avoid it but I don't know how to do it :(
        /// </summary>
        private static MethodInfo? GetStateMethod ( JsValue promise ) {
            var stateProperty = promise.GetType ().GetProperties ( BindingFlags.Instance | BindingFlags.NonPublic ).FirstOrDefault ( a => a.Name == "State" );
            var stateGetMethod = stateProperty?.GetGetMethod ( nonPublic: true );
            return stateGetMethod;
        }

    }

}
