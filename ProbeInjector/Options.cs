using System.Collections.Generic;
using CommandLine;

namespace ProbeInjector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: CommandLineParser
    internal class Options
    {
        [Option('r', "read", Required = true, HelpText = "Input assemblies to be processed.")]
        public IEnumerable<string> AssemblyFilePaths { get; set; }

        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}
