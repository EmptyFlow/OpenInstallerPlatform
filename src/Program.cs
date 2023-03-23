using Microsoft.Extensions.DependencyInjection;
using OpenInstallerPlatform;
using OpenInstallerPlatform.InstallerModules;
using OpenInstallerPlatform.Modules;
using System.Reflection;

var collection = new ServiceCollection ()
    .AddHttpClient ()
    .AddSingleton<NetworkModule> ()
    .AddSingleton<LoggerModule> ()
    .AddSingleton<ConfigurationModule> ()
    .AddSingleton ( ( serviceProvider ) => new FilesModule ( serviceProvider.GetService<LoggerModule> (), Assembly.GetExecutingAssembly () ) )
    .AddSingleton<EnvironmentModule> ()
    .BuildServiceProvider ();

await InstallerPerformer.RunInstaller ( collection );
