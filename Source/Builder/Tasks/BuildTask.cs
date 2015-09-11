using System;
using Fluent.IO;
using FluentBuild;
using FluentFs.Core;

namespace Builder.Tasks
{
    public class BuildTask : BuildFile
    {
        public BuildTask()
        {
            AddTask("clean", Clean);
            AddTask("build", CompileSources);
        }

        public void Clean()
        {
            Folders.CompileOutput.Wipe();
            Folders.Package.Wipe();
        }

        public void CompileSources()
        {
            //File assemblyInfoFile = Folders.CompileOutput.File("Global.AssemblyInfo.cs");

            Task.CreateAssemblyInfo.Language.CSharp(aid =>
            {
                Projects.CoinbaseProject.AssemblyInfo(aid);
                aid.OutputPath(Projects.CoinbaseProject.Folder.SubFolder("Properties").File("AssemblyInfo.cs"));
            });
            Task.CreateAssemblyInfo.Language.CSharp(aid =>
            {
                Projects.CoinbaseMvcProject.AssemblyInfo(aid);
                aid.OutputPath(Projects.CoinbaseMvcProject.Folder.SubFolder("Properties").File("AssemblyInfo.cs"));
            });
            
            Task.Build.MsBuild(msb =>
            {
                msb.Configuration("Release")
                    .ProjectOrSolutionFilePath(Projects.CoinbaseProject.ProjectFile)
                    .AddTarget("Rebuild")
                    .OutputDirectory(Projects.CoinbaseProject.OutputDirectory);
            });
            Task.Build.MsBuild(msb =>
            {
                msb.Configuration("Release")
                    .ProjectOrSolutionFilePath(Projects.CoinbaseMvcProject.ProjectFile)
                    .AddTarget("Rebuild")
                    .OutputDirectory(Projects.CoinbaseMvcProject.OutputDirectory);
            });

            Defaults.Logger.WriteHeader("BUILD COMPLETE. Packaging ...");


            //copy compile directory to package directory
            Path.Get(Projects.CoinbaseProject.OutputDirectory.ToString())
                .Copy(Projects.CoinbaseProject.PackageDir.ToString(), Overwrite.Always, true);

            Path.Get(Projects.CoinbaseMvcProject.OutputDirectory.ToString())
                .Copy(Projects.CoinbaseMvcProject.PackageDir.ToString(), Overwrite.Always, true);

            string version = Properties.CommandLineProperties.Version();

            Defaults.Logger.Write("RESULTS", "NuGet packing");

            Path nuget = Path.Get(Folders.Lib.ToString())
                .Files("NuGet.exe", true).First();

            Task.Run.Executable(e => e.ExecutablePath(nuget.FullPath)
                .WithArguments("pack", Projects.CoinbaseProject.NugetSpec.Path, "-Version", version, "-OutputDirectory",
                    Folders.Package.ToString()));
            Task.Run.Executable(e => e.ExecutablePath(nuget.FullPath)
                .WithArguments("pack", Projects.CoinbaseMvcProject.NugetSpec.Path, "-Version", version,
                    "-OutputDirectory", Folders.Package.ToString()));

            Defaults.Logger.Write("RESULTS", "Setting NuGet PUSH script");


            //Defaults.Logger.Write( "RESULTS", pushcmd );
            System.IO.File.WriteAllText("nuget.push.bat",
                "{0} push {1}".With(nuget.MakeRelative().ToString(),
                    Path.Get(Projects.CoinbaseProject.NugetNupkg.ToString()).MakeRelative().ToString()) +
                Environment.NewLine
                +
                "{0} push {1}".With(nuget.MakeRelative().ToString(),
                    Path.Get(Projects.CoinbaseMvcProject.NugetNupkg.ToString()).MakeRelative().ToString())
                );
        }
    }
}
