using System;
using System.IO;
using Newtonsoft.Json;

namespace DecayModuleTool.STD
{
    public static class STDProvider
    {
        public static void ProvideSTD(string module)
        {
            Console.WriteLine($"Started copying the standard module libs (STD) for module {module}...");

            string file = ModuleList.ModuleList.Instance.GetModuleFile(module);

            //Make sure we have a valid file
            if (File.Exists(file))
            {
                //Load the module file, or the cached version if there is one.
                var dmod = ModuleFile.LoadModule(module);

                //Make sure there is a language before we continue
                if (!string.IsNullOrWhiteSpace(dmod.Lang))
                {
                    //Get the path of the module
                    var modulePath = Path.GetDirectoryName(file);

                    string moduleSTDDir = Path.Combine(modulePath, dmod.Folders["std"]);

                    //Make sure we have a directory to drop them in
                    if (!Directory.Exists(moduleSTDDir)) Directory.CreateDirectory(moduleSTDDir);

                    if (ProvideToFolder(moduleSTDDir, dmod.Lang))
                        Console.WriteLine($"Completed copying the standard module libs (STD) for module {module}.");
                    else
                        Console.WriteLine($"Aborted copying the standard module libs (STD) for module {module} because the language \"{dmod.Lang}\" isn't supported. (maybe its spelled wrong?)");
                }
            }
            else
            {
                Console.WriteLine($"Aborted copying the standard module libs (STD) for module {module} because it doesn't have a language set.");
            }
        }

        public static bool ProvideToFolder(string folder, string lang)
        {
            //Get the directory with headers and the directory to drop them
            string stdDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STD", lang);

            if (Directory.Exists(stdDir) && Directory.Exists(folder))
            {

                //Go through each header to copy
                foreach (var std in Directory.EnumerateFiles(stdDir))
                {
                    string fileName = Path.GetFileName(std);
                    string newFileName = Path.Combine(folder, fileName);

                    //Remove the new file if it already exist
                    if (File.Exists(newFileName)) File.Delete(newFileName);

                    //Copy the header
                    File.Copy(std, newFileName);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}