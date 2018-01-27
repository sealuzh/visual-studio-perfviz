using System;
using System.IO;
using CommandLine;
using Mono.Cecil;

namespace ProbeInjector
{
    internal static class Program
    {

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(errors => Console.Read());
        }

        private static void Run(Options options)
        {
            // Loading Assemblies
            var probeAssembly = new ProbeAssembly(options.ProbeFilePath);
            probeAssembly.Load();
            var targetAssemblyDefinition = AssemblyDefinition.ReadAssembly(options.InputFilePath);

            // Injecting References
            var rewriter = new IlRewriter(targetAssemblyDefinition, probeAssembly);
            rewriter.InjectProbe();

            // Writing Output
            var outputFilePath = options.OutputFilePath ?? FileHelper.GetAvailableFilePath(options.InputFilePath);
            targetAssemblyDefinition.Write(outputFilePath);

            // Moving the ProbeAssembly to the folder of the assembly
            var directory = Path.GetDirectoryName(options.InputFilePath);
            var probeFileName = Path.GetFileName(options.ProbeFilePath);
            if (!string.IsNullOrWhiteSpace(directory) && !string.IsNullOrWhiteSpace(probeFileName))
            {
                var probeDestinationFilePath = Path.Combine(directory, probeFileName);
                File.Copy(options.ProbeFilePath, probeDestinationFilePath, true);
            }
        }

    }
}
