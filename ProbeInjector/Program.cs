using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Mono.Cecil;

namespace ProbeInjector
{
    static class Program
    {
        /// <summary>
        /// Probe is copied to the ProbeInjector folder
        /// </summary>
        private const string ProbeAssemblyFilePath = nameof(Probe) + ".dll";

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(errors => Console.Read());
        }

        private static void Run(Options options)
        {
            foreach (var assemblyFilePath in options.AssemblyFilePaths)
            {
                // Injecting References
                var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFilePath);
                InjectProbe(assemblyDefinition);
                var outputFilePath = FileHelper.GetAvailableFilePath(assemblyFilePath);
                assemblyDefinition.Write(outputFilePath);

                // Moving the Probe to the folder of the assembly
                var directory = Path.GetDirectoryName(assemblyFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    var probeDestinationFilePath = Path.Combine(directory, ProbeAssemblyFilePath);
                    File.Copy(ProbeAssemblyFilePath, probeDestinationFilePath, true);
                }
            }
        }

        private static void InjectProbe(AssemblyDefinition assemblyDefinition)
        {
            var rewriter = new IlRewriter(assemblyDefinition);
            foreach (var typeDefinition in assemblyDefinition.MainModule.Types)
            {
                for (var j = 0; j < typeDefinition.Methods.Count; j++)
                {
                    //IsSpecialName not optimal.
                    if (!typeDefinition.Methods[j].IsSpecialName)
                    {
                        typeDefinition.Methods[j] = rewriter.HookMethod(typeDefinition.Methods[j]);
                    }
                }
            }
        }
    }
}
