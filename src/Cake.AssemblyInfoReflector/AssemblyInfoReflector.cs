using Cake.Common.Solution.Project.Properties;
using Cake.Core;
using Cake.Core.IO;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cake.AssemblyInfoReflector
{
    /// <summary>
    /// The assembly info reflector.
    /// </summary>
    public sealed class AssemblyInfoReflector
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoReflector"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        public AssemblyInfoReflector(IFileSystem fileSystem, ICakeEnvironment environment)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            _fileSystem = fileSystem;
            _environment = environment;
        }

        /// <summary>
        /// Retrieves information from an assembly by reflection-only loading it and examining it's attributes.
        /// </summary>
        /// <param name="assemblyInfoPath">The file path.</param>
        /// <returns>Information about the assembly info content.</returns>
        public AssemblyInfoParseResult Reflect(FilePath assemblyInfoPath)
        {
            if (assemblyInfoPath == null)
            {
                throw new ArgumentNullException("assemblyInfoPath");
            }

            if (assemblyInfoPath.IsRelative)
            {
                assemblyInfoPath = assemblyInfoPath.MakeAbsolute(_environment);
            }

            var file = _fileSystem.GetFile(assemblyInfoPath);
            if (!file.Exists)
            {
                const string format = "Assembly file '{0}' does not exist.";
                var message = string.Format(CultureInfo.InvariantCulture, format, assemblyInfoPath.FullPath);
                throw new CakeException(message);
            }

            var module = ModuleDefinition.ReadModule(assemblyInfoPath.FullPath);

            var clscompliantattribute = module.getValue<CLSCompliantAttribute>();
            var assemblycompanyattribute = module.getValue<AssemblyCompanyAttribute>();
            var comvisibleattribute = module.getValue<ComVisibleAttribute>(caa => ((bool)caa.Value).ToString());
            var assemblyconfigurationattribute = module.getValue<AssemblyConfigurationAttribute>();
            var assemblycopyrightattribute = module.getValue<AssemblyCopyrightAttribute>();
            var assemblydescriptionattribute = module.getValue<AssemblyDescriptionAttribute>();
            var assemblyfileversionattribute = module.getValue<AssemblyFileVersionAttribute>();
            var guidattribute = module.getValue<GuidAttribute>();
            var assemblyinformationalversionattribute = module.getValue<AssemblyInformationalVersionAttribute>();
            var assemblyproductattribute = module.getValue<AssemblyProductAttribute>();
            var assemblytitleattribute = module.getValue<AssemblyTitleAttribute>();
            var assemblytrademarkattribute = module.getValue<AssemblyTrademarkAttribute>();
            var assemblyversionattribute = module.getValue<AssemblyVersionAttribute>();
            var internalsvisibletoattribute = module.getValues<InternalsVisibleToAttribute>();

            var result = new AssemblyInfoParseResult(
                    clscompliantattribute,
                    assemblycompanyattribute,
                    comvisibleattribute,
                    assemblyconfigurationattribute,
                    assemblycopyrightattribute,
                    assemblydescriptionattribute,
                    assemblyfileversionattribute,
                    guidattribute,
                    assemblyinformationalversionattribute,
                    assemblyproductattribute,
                    assemblytitleattribute,
                    assemblytrademarkattribute,
                    assemblyversionattribute,
                    internalsvisibletoattribute
                );
            return result;
        }
    }

    internal static class ModuleDefinitionExtensions
    {
        internal static string getValue<T>(this ModuleDefinition me) where T : Attribute
        {
            return me.getValue<T>(caa => (string)caa.Value);
        }

        internal static string getValue<T>(this ModuleDefinition me, Func<CustomAttributeArgument, string> translator) where T : Attribute
        {
            var attribute = me.Assembly.CustomAttributes.Where(ca => ca.AttributeType.FullName == typeof(T).FullName).SingleOrDefault();
            if (attribute != null)
            {
                var argument = attribute.ConstructorArguments.First();
                return translator(argument);
            }
            else
            {
                return null;
            }
        }

        internal static IEnumerable<string> getValues<T>(this ModuleDefinition me) where T : Attribute
        {
            var attributes = me.Assembly.CustomAttributes.Where(ca => ca.AttributeType.FullName == typeof(T).FullName);
            if (attributes.Any())
            {
                var values = attributes.Select(at => at.ConstructorArguments.First().Value).OfType<string>();
                return values;
            }
            else
            {
                return null;
            }
        }
    }

}
