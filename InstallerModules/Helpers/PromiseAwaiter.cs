using System.Reflection;

namespace OpenInstallerPlatform.InstallerModules.Helpers {

    /// <summary>
    /// Class has helper for awaiting promise completion.
    /// </summary>
    public static class PromiseAwaiter {

        private static MethodInfo? GetStateMethod ( object promise ) {
            var stateProperty = promise.GetType ().GetProperties ( BindingFlags.Instance | BindingFlags.NonPublic ).FirstOrDefault ( a => a.Name == "State" );
            var stateGetMethod = stateProperty?.GetGetMethod ( nonPublic: true );
            return stateGetMethod;
        }

        public static async Task<(bool completed, string message)> Await ( object promise, Func<bool> finishedMark ) {
            var stateGetMethod = GetStateMethod ( promise );

            while ( true ) {
                await Task.Delay ( 1000 );

                var state = stateGetMethod?.Invoke ( promise, null );
                if ( state != null ) {
                    var isRejected = (int) state == 2;
                    var isCompleted = (int) state == 1;
                    if ( isRejected ) return (false, "");
                    if ( isCompleted ) break;
                }

                if ( finishedMark != null && finishedMark () ) break;
            }

            return (true, "");
        }

    }

}
