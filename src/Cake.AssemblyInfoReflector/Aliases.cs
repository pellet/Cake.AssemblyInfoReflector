using Cake.Common.Solution.Project.Properties;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using System;

namespace Cake.AssemblyInfoReflector
{
    /// <summary>
    /// Contains functionality related to assembly info.
    /// </summary>
    [CakeAliasCategory("Assembly Info")]
    [CakeNamespaceImport("Cake.Common.Solution.Project.Properties")]
    public static class AssemblyInfoReflectorAliases
    {
        /// <summary>
        /// Reflects assembly attributes from an existing assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>The values of the assembly info attributes.</returns>
        /// <example>
        /// var assemblyInfo = ReflectAssemblyInfo("./bin/release/MyShinyNewAssembly.dll");
        /// Information("Version: {0}", assemblyInfo.AssemblyVersion);
        /// Information("Informational version: {0}", assemblyInfo.AssemblyInformationalVersion);
        /// </example>
        [CakeMethodAlias]
        public static AssemblyInfoParseResult ReflectAssemblyInfo(this ICakeContext context, FilePath assemblyPath)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var reflector = new AssemblyInfoReflector(context.FileSystem, context.Environment);
            return reflector.Reflect(assemblyPath);
        }
    }
}
