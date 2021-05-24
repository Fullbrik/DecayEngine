using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DecayBuildTool
{
    [Serializable]
    public class ModuleFile
    {
        public string Name { get; set; }
        public Dictionary<string, string> Folders { get; set; } = new Dictionary<string, string>();
        public string Lang { get; set; }
        public string Header { get; set; }
        public PlatformDependantModuleConfig PreBuild { get; set; }
        public string Build { get; set; }
        public PlatformDependantModuleConfig Output { get; set; }
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