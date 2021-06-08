using System;
using System.Linq;
using System.Threading.Tasks;

namespace DecayModuleTool
{
    public class ModuleTask
    {
        public enum TaskType
        {
            ProvideSTD,
            Build
        }

        public Options Options { get; }
        public string Module { get; }

        public ModuleTask(Options options, string module = null)
        {
            Options = options;
            Module = (module == null) ? Options.Module : module;
        }

        public async Task Start()
        {
            //If we are doing this recursively, we need to load the module to find its dependencies.
            if (Options.Recursive)
            {
                Console.WriteLine($"Doing tasks for dependencies of module {Module}");

                //Here, we use linq to go through each dependency in the module, turn it into a ModuleTask.
                var deps = ModuleFile.LoadModule(Module).Deps;

                if (deps.Length > 0)
                {
                    var tasks = deps.Select((dep) => new ModuleTask(Options, dep).Start()).ToArray();
                    await Task.WhenAll(tasks);
                }
            }

            await Task.WhenAll(ProvideSTD(), Build());
        }

        private Task ProvideSTD()
        {
            return Task.Run(() =>
            {
                //If we want to provide std, provide it
                if (Options.ProvideSTD) STD.STDProvider.ProvideSTD(Module);
            });
        }

        private Task Build()
        {
            return Task.Run(() =>
            {
                //If we want to build the module, build it
                if (Options.Build) DecayModuleTool.Build.ModuleBuilder.BuildModule(Module, Options.CollectBinPath);
            });
        }
    }
}