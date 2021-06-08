using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace DecayModuleTool.ModuleList
{
    public class ModuleList
    {
        public static ModuleList Instance { get; private set; }

        public static void LoadFile(string path)
        {
            Instance = new ModuleList();
            Instance.MountFile(path);
        }

        public static void LoadFolder(string folder)
        {
            LoadFile(Path.Combine(folder, ".dmodlist"));
        }

        private Dictionary<string, string> List { get; } = new Dictionary<string, string>();

        public void Add(string name, string path)
        {
            List.Add(name, path);
        }

        public override string ToString()
        {
            return string.Join('\n', List.Select((kvp) => $"{kvp.Key}:{kvp.Value}"));
        }

        public void MountFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                //Load all the lines
                var lines = File.ReadAllLines(fileName);

                //Go through each line
                foreach (var line in lines)
                {
                    //Split the line by the : char
                    var split = line.Split(':');

                    //Make sure we have at least a name and path
                    if (split.Length >= 2)
                    {
                        //And add it to the list
                        List.Add(split[0], split[1]);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Couldn't find dmodlist file (.dmodlist).", fileName);
            }
        }

        public string GetModuleFile(string module)
        {
            if (List.ContainsKey(module))
                return Path.Combine(List[module], "module.dmod");
            else
                return null;
        }
    }
}