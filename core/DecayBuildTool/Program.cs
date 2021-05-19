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
                ModuleBuilder.BuildFile(options.File);
            });
        }
    }
}
