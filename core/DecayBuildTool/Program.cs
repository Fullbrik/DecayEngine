using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Newtonsoft.Json;

namespace DecayBuildTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                //If we want to copy headers, copy them
                if (options.CopyHeaders) HeaderProvider.ProvideHeaders(options.File);

                //If we want to build the module, build it
                if (options.Build) ModuleBuilder.BuildFile(options.File);
            });
        }
    }
}
