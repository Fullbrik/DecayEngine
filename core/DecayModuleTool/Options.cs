using CommandLine;

namespace DecayModuleTool
{
    public class Options
    {
        [Option('l', "generate-module-list", Required = false, HelpText = "Should we rebuild the module list file? This is needed to access any module files, but can take a long time.")]
        public bool GenerateModuleList { get; set; }

        // [Option('f', "file", Required = true, HelpText = "The module file to use (.dmod).")]
        // public string File { get; set; }

        [Option('f', "folder", Required = true, HelpText = "The modules folder to operate off of.")]
        public string Folder { get; set; }

        [Option('m', "module", Required = true, HelpText = "The module to be done actions upon. You can set this to \"n\" if you are doing an action that doesn't require a module.")]
        public string Module { get; set; }

        [Option('r', "recursive", Required = false, HelpText = "Use this flag to do tasks on any dependencies (Deps) of the module as well.")]
        public bool Recursive { get; set; }

        [Option('s', "provide-std", Required = false, HelpText = "Use this flag if we should copy the standard module files into the module.")]
        public bool ProvideSTD { get; set; }

        [Option('b', "build", Required = false, HelpText = "Use this flag if we should build the module.")]
        public bool Build { get; set; }

        [Option('c', "collect-bin-path", Required = false, HelpText = "Where should we collect the binaries from the modules into a folder? Leave this blank to skip this step.")]
        public string CollectBinPath { get; set; } = null;

        [Option("copy-std-to-folder", Required = false, HelpText = "(Optional) Copy the std libs for rust into a folder. This is used for building for a platform.")]
        public string CopySTDToFolder { get; set; } = null;
    }
}