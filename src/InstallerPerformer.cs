using Jint;
using Microsoft.Extensions.DependencyInjection;
using OpenInstallerPlatform.InstallerModules;
using OpenInstallerPlatform.InstallerModules.Helpers;
using OpenInstallerPlatform.Modules;
using System.Reflection;

namespace OpenInstallerPlatform {

    internal static class InstallerPerformer {

        private static bool Finished = false;

        public static async Task RunInstaller ( ServiceProvider serviceProvider ) {
            var configurationModule = serviceProvider.GetService<ConfigurationModule> ();
            if ( configurationModule == null ) throw new ArgumentException ( nameof ( ConfigurationModule ) );

            var engine = new Engine (
                    options => {
                        options.EnableModules ( new ResourcesModuleLoader () );
                        options.CatchClrExceptions ();
                    }
            );

            AddModules ( engine, serviceProvider, configurationModule );

            var installerModule = engine.ImportModule ( "./installer.js" );

            var promise = installerModule.Get ( "default" ).Call ();
            if ( promise.IsPromise () ) {
                var (completed, message) = await PromiseAwaiter.Await ( promise, () => Finished );
                if ( !completed ) LoggerModule.Instance.Error ( "RunInstaller", "main promise is rejected! " + message );
            }

            await configurationModule.SaveConfiguration ();
        }

        private static void AddModules ( Engine engine, ServiceProvider serviceProvider, ConfigurationModule configurationModule ) {
            engine.AddModule (
                "installer",
                builder => builder
                    .ExportFunction ( "finishInstall", () => Finished = true )
                    .ExportObject ( "Configuration", configurationModule )
                    .ExportObject ( "Logger", serviceProvider.GetService<LoggerModule> () ?? throw new ArgumentException ( nameof ( LoggerModule ) ) )
                    .ExportObject ( "FileSystem", new FilesModule ( LoggerModule.Instance, Assembly.GetExecutingAssembly () ) )
                    .ExportObject ( "Environment", serviceProvider.GetService<EnvironmentModule> () ?? throw new ArgumentException ( nameof ( EnvironmentModule ) ) )
                    .ExportObject ( "Network", serviceProvider.GetService<NetworkModule> () ?? throw new ArgumentException ( nameof ( NetworkModule ) ) )
            );
        }

    }

}
