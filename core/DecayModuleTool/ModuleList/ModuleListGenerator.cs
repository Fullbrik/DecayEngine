using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DecayModuleTool.ModuleList
{
    public static class ModuleListGenerator
    {
        public static void GenerateModuleList(string folder)
        {
            Console.WriteLine($"Generating module list for folder {folder}...");

            //A list of all our paths
            ModuleList list = new ModuleList();

            //Make sure we are operating on a real directory
            if (Directory.Exists(folder))
            {
                //Next, search all directories for modules, including sub-folders
                ProbeFolder(folder, list);

                //And write it into .dmodlist
                File.WriteAllText(Path.Combine(folder, ".dmodlist"), list.ToString());
            }
            else
            {
                throw new Exception($"Couldn't find modules directory \"{folder}\"");
            }
        }

        private static void ProbeFolder(string folder, ModuleList list)
        {
            Console.WriteLine($"Probing folder {folder}...");

            //Check if directory is a module, in which case we don't need to probe more
            if (File.Exists(Path.Combine(folder, "module.dmod")))
            {
                //Read the dmod file to get the name of the module
                string text = File.ReadAllText(Path.Combine(folder, "module.dmod"));
                var dmod = JsonConvert.DeserializeObject<ModuleFile>(text);

                //Add it to the list
                list.Add(dmod.Name, Path.GetFullPath(folder));
            }
            else
            {
                //Otherwise, we need to search more folders
                var subDirs = Directory.GetDirectories(folder);
                foreach (var dir in subDirs)
                {
                    //We do recursive b/c I can. If you don't like it, make a PR and fix it
                    ProbeFolder(dir, list);
                }
            }
        }
    }
}