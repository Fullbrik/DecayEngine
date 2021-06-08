using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DecayModuleTool.ModuleList;
using Newtonsoft.Json;

namespace DecayModuleTool
{
    [Serializable]
    public class ModuleFile
    {
        private static Dictionary<string, ModuleFile> ModuleFileCache { get; } = new Dictionary<string, ModuleFile>();

        public static ModuleFile LoadModule(string module)
        {
            if (ModuleFileCache.ContainsKey(module))
            {
                return ModuleFileCache[module];
            }
            else
            {
                string file = ModuleList.ModuleList.Instance.GetModuleFile(module);

                if (File.Exists(file))
                {
                    //Read the file's text and parse it
                    string text = File.ReadAllText(file);
                    ModuleFile dmod = JsonConvert.DeserializeObject<ModuleFile>(text);

                    //Add it to the cache
                    ModuleFileCache.Add(module, dmod);

                    //And return it
                    return dmod;
                }
                else
                {
                    throw new FileNotFoundException($"Couldn't get module file for module {module}", file);
                }
            }
        }

        public string Name { get; set; }
        public Dictionary<string, string> Folders { get; set; } = new Dictionary<string, string>();
        public string Lang { get; set; }
        public string Header { get; set; }
        public PlatformDependantModuleConfig PreBuild { get; set; }
        public string Build { get; set; }
        public PlatformDependantModuleConfig Output { get; set; }
        public string[] Deps { get; set; }
    }

    [Serializable]
    public class PlatformDependantModuleConfig
    {
        public string Linux { get; set; }
        public string Windows { get; set; }
        public string Macos { get; set; }

        public string Get()
        {
            var platform = Environment.OSVersion.Platform;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Macos;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Windows;
            }
            else
            {
                throw new Exception("Cannot determine operating system!");
            }
        }
    }
}