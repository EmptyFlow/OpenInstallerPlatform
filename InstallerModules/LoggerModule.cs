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

        public static void Log (string message) => Console.WriteLine ( message );

        public void Information ( string group, string message ) {
            Console.WriteLine ( GetLine ( "info", group, message ) );
        }

        public void Warning ( string group, string message ) {
            Console.WriteLine ( GetLine ( "warn", group, message ) );
        }

        public void Error ( string group, string message ) {
            Console.WriteLine ( GetLine ( "err", group, message ) );
        }

    }

}
