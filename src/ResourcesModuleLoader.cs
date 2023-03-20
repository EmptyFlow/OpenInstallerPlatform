using Jint.Runtime.Modules;
using Jint;
using Esprima.Ast;
using Esprima;
using SystemAssembly = System.Reflection.Assembly;

namespace OpenInstallerPlatform {
    public class ResourcesModuleLoader : IModuleLoader {

        private readonly SystemAssembly m_resourcesAssembly;

        private readonly string m_resourcesPrefix;

        public ResourcesModuleLoader()
        {
            m_resourcesAssembly = SystemAssembly.GetExecutingAssembly ();
            m_resourcesPrefix = m_resourcesAssembly.GetName ().Name + ".Content.";
        }

        public Module LoadModule ( Engine engine, ResolvedSpecifier resolved ) {
            var expectedFileName = m_resourcesPrefix + resolved.Specifier.Replace ( "/", "." ).Replace ( "\\", "." );

            var stream = m_resourcesAssembly.GetManifestResourceStream ( expectedFileName ) ?? throw new Exception ( $"Error while loading module: not found {resolved.Specifier}" );
            using StreamReader reader = new( stream );
            var content = reader.ReadToEnd ();

            Module module;
            try {
                module = new JavaScriptParser ().ParseModule ( content, resolved.Specifier );
            } catch ( ParserException ex ) {
                throw new Exception ( $"Error while loading module: error in module '{resolved.Specifier}': {ex.Error}" );
            } catch ( Exception ) {
                throw new Exception ( $"Could not load module {resolved.Specifier}" );
            }

            return module;
        }

        public ResolvedSpecifier Resolve ( string? referencingModuleLocation, string specifier ) {

            return new ResolvedSpecifier (
                specifier.StartsWith("./") ? specifier.Substring(2) : specifier,
                specifier,
                null,
                SpecifierType.RelativeOrAbsolute
            );
        }
    }

}
