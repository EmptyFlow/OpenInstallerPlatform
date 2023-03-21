using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace OpenInstallerPlatform.Modules {

    /// <summary>
    /// Logger module.
    /// </summary>
    public class LoggerModule {

        public static readonly LoggerModule Instance = new ();

        private string m_DateTimeFormat = "yyyy-MM-dd hh:mm:ss.ms";

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public string dateTimeFormat => m_DateTimeFormat;

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void setDateTimeFormat( string dateTimeFormat ) => m_DateTimeFormat = dateTimeFormat;

        private string GetLine ( string type, string group, string message ) {
            return $"[{DateTime.Now.ToString ( dateTimeFormat )}]<{type}>{group}: {message}";
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void information ( string group, string message ) {
#if DEBUG
            Console.WriteLine ( GetLine ( "info", group, message ) );
#else
            
#endif
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void warning ( string group, string message ) {
#if DEBUG
            Console.WriteLine ( GetLine ( "warn", group, message ) );
#else
            
#endif
        }

        [SuppressMessage ( "Style", "IDE1006:Naming Styles", Justification = "<Pending>" )]
        public void error ( string group, string message ) {
#if DEBUG
            Console.WriteLine ( GetLine ( "err", group, message ) );
#else
            
#endif
        }

    }
}
