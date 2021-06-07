using System;
using System.IO;
using Newtonsoft.Json;

namespace DecayModuleTool
{
    public static class HeaderGenerator
    {
        public static void GenerateHeader(string file)
        {
            Console.WriteLine($"Started generating headers for file {file}");

            //If the file has no extension, it prob has a .dmod extension irl
            if (!Path.HasExtension(file)) file = file + ".dmod";

            //Make sure we have a valid file
            if (File.Exists(file))
            {
                //Read the file's text and parse it
                string text = File.ReadAllText(file);
                ModuleFile dmod = JsonConvert.DeserializeObject<ModuleFile>(text);

                //Get the path of the module
                var modulePath = Path.GetDirectoryName(file);

                string headerFile = Path.Combine(modulePath, dmod.Header);
            }
        }
    }
}