using System.Linq;
using System.Collections.Generic;
namespace DecayModuleTool.ModuleList
{
    public class ModuleList
    {
        private Dictionary<string, string> List { get; } = new Dictionary<string, string>();

        public void Add(string name, string path)
        {
            List.Add(name, path);
        }

        public override string ToString()
        {
            return string.Join('\n', List.Select((kvp) => $"{kvp.Key}:{kvp.Value}"));
        }
    }
}