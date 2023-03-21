using System.Diagnostics.CodeAnalysis;

namespace OpenInstallerPlatform.Modules {

    public sealed class EnvironmentModule {

        private uint m_exitCode = 0;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public static string programFiles(string folder = "") => Path.Combine(Environment.GetFolderPath ( Environment.SpecialFolder.ProgramFiles ), folder);

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public static string programFilesX86 ( string folder = "" ) => Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.ProgramFilesX86 ), folder);

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public static string profileFolder ( string folder = "" ) => Path.Combine ( Environment.GetEnvironmentVariable( Environment.OSVersion.Platform == PlatformID.Win32NT ? "%HOMEPATH%" : "$HOME" ) ?? "", folder);

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public static bool isWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public static bool isUnix => Environment.OSVersion.Platform == PlatformID.Unix;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public static bool isMacOSX => Environment.OSVersion.Platform == PlatformID.MacOSX;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void setExitCode ( uint exitCode ) => m_exitCode = exitCode;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public uint getExitCode () => m_exitCode;

    }

}
