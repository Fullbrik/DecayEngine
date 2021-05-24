using System;
using System.Collections.Generic;

namespace DecayBuildTool
{
    [Serializable]
    public class HeaderFileData
    {
        public string Name;
        public HeaderFunctionData[] HeaderFunctions { get; set; }
    }

    [Serializable]
    public class HeaderFunctionData
    {
        public string[] Name { get; set; }
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public HeaderFunctionContentData[] Contents { get; set; }
    }

    [Serializable]
    public class HeaderFunctionContentData
    {
        public string Command { get; set; }
        public (string Type, string Param, string Value, string If, string Else)[] Packet { get; set; }
    }
}