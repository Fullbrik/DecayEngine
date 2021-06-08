using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DecayModuleTool.Build
{
    public static class ModuleBuilder
    {
        public static PlatformDependantModuleConfig LibName { get; } = new PlatformDependantModuleConfig()
        {
            Linux = "libmodule.so",
            Macos = "libmodule.dylib",
            Windows = "Module.dll"
        };

        public static void BuildModule(string module, string collectDir)
        {
            Console.WriteLine($"Started build on module {module}");

            string file = ModuleList.ModuleList.Instance.GetModuleFile(module);

            //Make sure we have a valid file
            if (File.Exists(file))
            {
                //Load the module
                var dmod = ModuleFile.LoadModule(module);

                //Get the path of the module
                var modulePath = Path.GetDirectoryName(file);

                //Execute the prebuild and build commands
                System(dmod.PreBuild.Get(), modulePath);
                System(dmod.Build, modulePath);

                //Cache a bunch of folders we're about to use
                string buildFolder = Path.Combine(modulePath, dmod.Folders["build"]);
                string targetFile = Path.Combine(modulePath, dmod.Output.Get());
                string destFile = Path.Combine(buildFolder, LibName.Get());

                //Make sure our output environment is ready to use
                if (!Directory.Exists(buildFolder)) Directory.CreateDirectory(buildFolder);
                if (!File.Exists(targetFile)) throw new FileNotFoundException("Unable to get module library", targetFile);
                if (File.Exists(destFile)) File.Delete(destFile);

                //Copy the file from wherever the output normally would be from the compiler to the proper output
                File.Copy(targetFile, destFile);

                //If we are collecting all binaries to a folder, collect it now
                if (!string.IsNullOrWhiteSpace(collectDir))
                {
                    collectDir = Path.GetFullPath(collectDir);

                    Console.WriteLine($"Putting module lib into the collect folder {collectDir}...");

                    string destCollectDir = Path.Combine(collectDir, dmod.Name);
                    string destCollectFile = Path.Combine(destCollectDir, LibName.Get());
                    if (!Directory.Exists(destCollectDir)) Directory.CreateDirectory(destCollectDir);
                    if (File.Exists(destCollectFile)) File.Delete(destCollectFile);

                    File.Copy(destFile, destCollectFile);

                    Console.WriteLine("Put module lib into the collect folder.");
                }

                Console.WriteLine($"Finished build on Module {module}");
            }
            else
            {
                throw new FileNotFoundException("Couldn't find module file", file);
            }
        }

        public static void System(string command, string workingDirectory)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                WorkingDirectory = workingDirectory,
            };

            Process process;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    psi.FileName = "bash";
                    psi.Arguments = $"-c " + "\"" + command.Replace("\"", "\\\"") + "\"";
                    process = Process.Start(psi);
                    process.WaitForExit();
                    Console.WriteLine($"Executed: \"{command}\" using sh with the working directory {workingDirectory}");
                    break;
                case PlatformID.Win32NT:
                    psi.FileName = "cmd.exe";
                    psi.Arguments = "/C " + command;
                    process = Process.Start(psi);
                    process.WaitForExit();
                    Console.WriteLine($"Executed: \"{command}\" using cmd with the working directory {workingDirectory}");
                    break;
                default:
                    Console.WriteLine($"Failed to execute: \"{command}\" b/c we couldn't determine the target platform");
                    break;
            }
        }
    }
}