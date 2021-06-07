using CommandLine;

namespace DecayModuleTool
{
    public class Options
    {
        [Option('l', "generate-module-list", Required = false, HelpText = "Should we rebuild the module list file? This is needed to access any module files, but can take a long time.")]
        public bool GenerateModuleList{ get; set; }

        // [Option('f', "file", Required = true, HelpText = "The module file to use (.dmod).")]
        // public string File { get; set; }

        [Option('f', "folder", Required = true, HelpText = "The modules folder to operate off of.")]
        public string Folder { get; set; }

        [Option('m', "module", Required = true, HelpText = "The module to be done actions upon. You can set this to \"n\" if you are doing an action that doesn't require a module.")]
        public string Module{ get; set; }

        [Option('b', "build", Required = false, HelpText = "Use this flag if we should build the module.")]
        public bool Build { get; set; }

        [Option('s', "provide-std", Required = false, HelpText = "Use this flag if we should copy the standard module files into the module.")]
        public bool CopyHeaders { get; set; }
    }
}