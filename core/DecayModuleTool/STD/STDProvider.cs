using System;
using System.IO;
using Newtonsoft.Json;

namespace DecayModuleTool.STD
{
    public static class STDProvider
    {
        public static void ProvideSTD(string file)
        {
            Console.WriteLine($"Started copying the standard module libs (STD) for file {file}...");

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

                //Get the directory with headers and the directory to drop them
                string headerDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STD", dmod.Lang);
                string moduleHeaderDir = Path.Combine(modulePath, dmod.Folders["headers"]);

                //Make sure we have a directory to drop them in
                if (!Directory.Exists(moduleHeaderDir)) Directory.CreateDirectory(moduleHeaderDir);

                //Go through each header to copy
                foreach (var header in Directory.EnumerateFiles(headerDir))
                {
                    string fileName = Path.GetFileName(header);
                    string newFileName = Path.Combine(moduleHeaderDir, fileName);

                    //Remove the new file if it already exist
                    if (File.Exists(newFileName)) File.Delete(newFileName);

                    //Copy the header
                    File.Copy(header, newFileName);
                }
            }
        }
    }
}