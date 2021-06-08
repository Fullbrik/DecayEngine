using System.Threading.Tasks;
using CommandLine;

namespace DecayModuleTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                if (options.GenerateModuleList) ModuleList.ModuleListGenerator.GenerateModuleList(options.Folder);

                ModuleList.ModuleList.LoadFolder(options.Folder);

                var task = new ModuleTask(options).Start();

                while (!task.IsCompleted) ;

                if (!string.IsNullOrWhiteSpace(options.CopySTDToFolder)) STD.STDProvider.ProvideToFolder(options.CopySTDToFolder, "rust");
            });
        }
    }
}
