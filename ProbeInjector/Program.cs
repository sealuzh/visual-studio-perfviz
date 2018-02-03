using System;
using System.IO;
using System.Reflection;
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
            try
            {
                // Checking and loading Assemblies
                AssemblyName.GetAssemblyName(options.ProbeFilePath);
                var probeAssembly = new ProbeAssembly(options.ProbeFilePath);
                var targetAssemblyDefinition = AssemblyDefinition.ReadAssembly(options.InputFilePath);

                // Injecting References
                var rewriter = new IlRewriter(targetAssemblyDefinition, probeAssembly);
                rewriter.InjectProbe();

                // Writing Output
                var outputFilePath = options.OutputFilePath ?? FileHelper.GetAvailableFilePath(options.InputFilePath);
                targetAssemblyDefinition.Write(outputFilePath);

                var sourceDirectory = Path.GetDirectoryName(options.ProbeFilePath) ?? throw new InvalidOperationException("probe has no directory");
                var destinationDirectory = Path.GetDirectoryName(options.InputFilePath) ?? throw new InvalidOperationException("target has no directory");
                FileHelper.CopyDllsInDirectory(sourceDirectory, destinationDirectory);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file cannot be found.");
                Console.Read();
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("The file is not an assembly.");
                Console.Read();
            }
            catch (FileLoadException)
            {
                Console.WriteLine("The assembly has already been loaded.");
                Console.Read();
            }
        }

    }
}
