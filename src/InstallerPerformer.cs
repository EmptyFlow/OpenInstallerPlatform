using Jint;
using Jint.Native;
using OpenInstallerPlatform.InstallerModules.Helpers;
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
                var (completed , message) = await PromiseAwaiter.Await ( promise, () => Finished );
                if ( !completed ) LoggerModule.Instance.error ( "RunInstaller", "main promise is rejected!" );
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

    }

}
