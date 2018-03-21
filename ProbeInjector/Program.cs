using System;
using System.IO;
using System.Reflection;
using CommandLine;
using Mono.Cecil;
using ProbeInjector.Probe;

namespace ProbeInjector
{
    internal static class Program
    {
        /// <summary>
        /// Entry of the <see cref="ProbeInjector"/>
        /// </summary>
        /// <param name="args">
        /// The Arguments
        /// 
        /// Example arguments:
        /// -r "..\..\..\..\eShopOnWeb\src\Web\bin\Any CPU\Release\netcoreapp2.0\Web.dll" -w "..\..\..\..\eShopOnWeb\src\Web\bin\Any CPU\Release\netcoreapp2.0\Web2.dll" -p "..\..\..\NetStandardAzureTelemetyProbe\bin\Release\netstandard2.0\NetStandardAzureTelemetyProbe.dll"
        /// -r "..\..\..\..\eShopOnWeb\src\Web\bin\Any CPU\Release\netcoreapp2.0\Web.dll" -w "..\..\..\..\eShopOnWeb\src\Web\bin\Any CPU\Release\netcoreapp2.0\Web2.dll" -p "..\..\..\DiagnosticsTraceProbe\bin\Debug\netstandard2.0\DiagnosticsTraceProbe.dll"
        /// -r "..\..\..\..\eShopOnWeb\src\Web\bin\Any CPU\Release\netcoreapp2.0\Web.dll" -w "..\..\..\..\eShopOnWeb\src\Web\bin\Any CPU\Release\netcoreapp2.0\Web2.dll" -p "..\..\..\AzureTelemetryProbe\bin\Debug\AzureTelemetryProbe.dll"
        /// -r "..\..\..\..\eShopOnWeb\src\Web\bin\Release\netcoreapp2.0\Web.dll" -w "..\..\..\..\eShopOnWeb\src\Web\bin\Release\netcoreapp2.0\Web2.dll" -p "..\..\..\AzureTelemetryProbe\bin\Debug\AzureTelemetryProbe.dll"
        /// -r "..\..\..\..\visual-studio-aspnet-test\visual-studio-aspnet-test\bin\visual-studio-aspnet-test.dll" -w "..\..\..\..\visual-studio-aspnet-test\visual-studio-aspnet-test\bin\visual-studio-aspnet-test2.dll" -p "..\..\..\AzureTelemetryProbe\bin\Debug\AzureTelemetryProbe.dll"
        /// -r "..\..\..\InjectionVictim\bin\Debug\InjectionVictim.exe" -w "..\..\..\InjectionVictim\bin\Debug\InjectionVictimNew.exe" -p "..\..\..\AzureTelemetryProbe\bin\Debug\AzureTelemetryProbe.dll"
        /// -r "..\..\..\InjectionVictim\bin\Debug\InjectionVictim.exe" -w "..\..\..\InjectionVictim\bin\Debug\InjectionVictimNew.exe" -p "..\..\..\DiagnosticsTraceProbe\bin\Debug\netstandard2.0\DiagnosticsTraceProbe.dll"
        /// </param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(errors => Console.Read());
        }

        private static void Run(Options options)
        {
            HandleIoExceptions(() =>
            {
                Console.WriteLine("Checking Probe...");
                var assemblyName = AssemblyName.GetAssemblyName(options.ProbeFilePath);

                Console.WriteLine($"Loading Probe {assemblyName.Name}...");
                var probeAssembly = ProbeAssemblyFactory.LoadAssembly(options.ProbeFilePath);

                Console.WriteLine($"Loading Target {assemblyName.Name}...");
                var fileBytes = File.ReadAllBytes(options.TargetFilePath);
                using (var memoryStream = new MemoryStream(fileBytes))
                {
                    using (var targetAssemblyDefinition = AssemblyDefinition.ReadAssembly(memoryStream))
                    {
                        Console.WriteLine("Injecting References...");
                        var rewriter = new IlRewriter(targetAssemblyDefinition);
                        rewriter.Inject(probeAssembly);

                        Console.WriteLine("Writing Output...");
                        targetAssemblyDefinition.Write(!string.IsNullOrWhiteSpace(options.OutputFilePath)
                            ? options.OutputFilePath
                            : options.TargetFilePath);
                    }
                }

                Console.WriteLine("Copying Dlls to Output-Directory...");
                var sourceDirectory = Path.GetDirectoryName(options.ProbeFilePath) ?? throw new InvalidOperationException("probe has no directory");
                var destinationDirectory = Path.GetDirectoryName(options.TargetFilePath) ?? throw new InvalidOperationException("target has no directory");
                FileHelper.CopyDllsInDirectory(sourceDirectory, destinationDirectory);
            }, options.Verbose);
        }

        #region ErrorHandling

        private static void HandleIoExceptions(Action action, bool verbose)
        {
            try
            {
                action();
                return;
            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine("The file cannot be found.");
                ConsoleWriteExceptionIfVerbose(exception, verbose);
            }
            catch (BadImageFormatException exception)
            {
                Console.WriteLine("The file is not an assembly.");
                ConsoleWriteExceptionIfVerbose(exception, verbose);
            }
            catch (FileLoadException exception)
            {
                Console.WriteLine("The assembly has already been loaded.");
                ConsoleWriteExceptionIfVerbose(exception, verbose);
            }
            catch (Exception exception)
            {
                Console.WriteLine("An unknown error occurred.");
                Console.WriteLine(exception);
            }
            Console.Read();
        }

        private static void ConsoleWriteExceptionIfVerbose(Exception exception, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}
