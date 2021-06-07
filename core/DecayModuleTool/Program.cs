using System;
using CommandLine;

namespace DecayModuleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                if (options.GenerateModuleList) ModuleList.ModuleListGenerator.GenerateModuleList(options.Folder);

                // //If we want to copy headers, copy them
                // if (options.CopyHeaders) STD.STDProvider.ProvideSTD(options.File);

                // //If we want to build the module, build it
                // if (options.Build) Build.ModuleBuilder.BuildFile(options.File);
            });
        }
    }
}
