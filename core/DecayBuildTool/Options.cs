using CommandLine;

namespace DecayBuildTool
{
    public class Options
    {
        [Option('f', "file")]
        public string File { get; set; }
    }
}