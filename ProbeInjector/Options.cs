using CommandLine;

namespace ProbeInjector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: CommandLineParser
    internal class Options
    {
        [Option('r', "read", Required = true, HelpText = "Input assembly file path to be processed.")]
        public string InputFilePath { get; set; }


        [Option('w', "write", Required = false, HelpText = "Output assembly file path.")]
        public string OutputFilePath { get; set; }


        [Option('p', "probe", Required = true, HelpText = "The probe dll to inject.")]
        public string ProbeFilePath { get; set; }

        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}
