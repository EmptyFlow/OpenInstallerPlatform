using System.Reflection;

namespace OpenInstallerPlatform.InstallerModules.Helpers {

    /// <summary>
    /// Class has helper for awaiting promise completion.
    /// </summary>
    public static class PromiseAwaiter {

        private static MethodInfo? GetPropertyMethod ( object promise, string propertyName ) {
            var stateProperty = promise.GetType ().GetProperties ( BindingFlags.Instance | BindingFlags.NonPublic ).FirstOrDefault ( a => a.Name == propertyName );
            var stateGetMethod = stateProperty?.GetGetMethod ( nonPublic: true );
            return stateGetMethod;
        }

        public static async Task<(bool completed, string message)> Await ( object promise, Func<bool> finishedMark ) {
            var stateGetMethod = GetPropertyMethod ( promise, "State" );
            var valueGetMethod = GetPropertyMethod ( promise, "Value" );

            while ( true ) {
                await Task.Delay ( 1000 );

                var state = stateGetMethod?.Invoke ( promise, null );
                if ( state != null ) {
                    var isRejected = (int) state == 2;
                    var isCompleted = (int) state == 1;
                    if ( isRejected ) {
                        var message = valueGetMethod?.Invoke ( promise, null )?.ToString() ?? "";
                        return (false, message);
                    }
                    if ( isCompleted ) break;
                }

                if ( finishedMark != null && finishedMark () ) break;
            }

            return (true, "");
        }

    }

}
