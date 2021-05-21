using CommandLine;

namespace DecayBuildTool
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "The module file to use (.dmod)")]
        public string File { get; set; }

        [Option('b', "build", Required = false, HelpText = "Use this flag if we should build the module")]
        public bool Build { get; set; }

        [Option('h', "copy-headers", Required = false, HelpText = "Use this flag if we should copy the header files to the module")]
        public bool CopyHeaders { get; set; }
    }
}