using System;
using System.Linq;
using System.Reflection;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using Fake.DotNet;
using System.Collections.Generic;

using System.IO;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.Utilities;
using Nuke.Common.Git;

using System.Diagnostics;
using Z.ExtensionMethods;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.CompressionTasks;

using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
   public static string ProjectName = "Coinbase";
   public static string GitHubUrl = "https://github.com/bchavez/Coinbase";
   
   public static class Folders
   {
      public static AbsolutePath CompileOutput = RootDirectory / "__compile";
      public static AbsolutePath Package = RootDirectory / "__package";
      public static AbsolutePath Test = RootDirectory / "__test";
      public static AbsolutePath Source = RootDirectory / "Source";
   }

   public static class Files
   {
      public static AbsolutePath History = RootDirectory / "HISTORY.md";
      public static AbsolutePath SolutionFile = Folders.Source / $"{ProjectName}.sln";
      public static AbsolutePath SnkFile = Folders.Source / $"{ProjectName}.snk";
      public static AbsolutePath SnkEncZipFile = Folders.Source / $"{ProjectName}.snk.enc.zip";
      public static AbsolutePath SnkFilePublic = Folders.Source / $"{ProjectName}.snk.pub";
   }

   public static int Main()
   {
      return Execute<Build>(x => x.Compile);
   }
   protected override void OnBuildInitialized()
   {
      base.OnBuildInitialized();
      // BUG: https://github.com/nuke-build/nuke/issues/844
      ProjectModelTasks.Initialize();
      this.CoinbaseProject = this.Solution.Coinbase;
      this.TestProject = this.Solution.Coinbase_Tests;
   }


   [Solution(GenerateProjects = true)]
   readonly Solution Solution;
   Project CoinbaseProject;
   Project TestProject;

   [GitRepository]
   readonly GitRepository GitRepo;


   Target Compile => _ => _
    .DependsOn(Clean)
    .DependsOn(Restore)
    .DependsOn(BuildInfo)
    .Executes(() =>
    {
       var nowarns = new[] { 1591, 1573 };

       DotNetBuild(b => b
         .SetProjectFile(this.Solution)
         .SetConfiguration(Configuration.Debug)
         .SetNoWarns2(nowarns)
         .EnableNoRestore()
         );


       DotNetBuild(b => b
         .SetProjectFile(this.Solution)
         .SetConfiguration(Configuration.Release)
         .SetNoWarns2(nowarns)
         .EnableNoRestore()
         );

       CopyDirectoryRecursively(this.CoinbaseProject.BinFolder(), this.CoinbaseProject.CompileOutput());

    });

   Target Pack => _ => _
   .DependsOn(Clean)
   .DependsOn(Compile)
   .Executes(() =>
   {

      DotNetPack(p => p
         .SetProject(this.CoinbaseProject)
         .EnableNoBuild()
         .SetConfiguration(Configuration.Release)
         .SetOutputDirectory(Folders.Package)
      );

   });

   Target Zip => _ => _
   .DependsOn(Clean)
   .DependsOn(Compile)
   .Executes(() => {

      var zipPath = Folders.Package / this.CoinbaseProject.ZipFile();
      Folders.CompileOutput.ZipTo(zipPath);
   });


   Target Clean => _ => _
       .Before(Restore)
       .Executes(() =>
       {
          //Debugger.Launch();

          Folders.Test.CreateOrCleanDirectory();
          Folders.CompileOutput.CreateOrCleanDirectory();
          Folders.Package.CreateOrCleanDirectory();
          
          var projects = this.Solution.AllProjects.Where(p => !p.Name.Contains("Builder"));
          foreach (var project in projects)
          {
             var dir = project.Directory;
             var binAndObjs = dir.GlobDirectories("**/bin", "**/obj");
             foreach (var d in binAndObjs) {
                d.DeleteDirectory();
             }
          }

          var coinbaseProjectMsb = CoinbaseProject.GetMSBuildProject();
          coinbaseProjectMsb.SetProperty("Version", "0.0.0-localbuild");
          coinbaseProjectMsb.SetProperty("PackageReleaseNotes", string.Empty);
          coinbaseProjectMsb.SetProperty("AssemblyOriginatorKeyFile", string.Empty);
          coinbaseProjectMsb.SetProperty("SignAssembly", "false");
          coinbaseProjectMsb.Save();

          var testProjectMsb = TestProject.GetMSBuildProject();
          testProjectMsb.SetProperty("AssemblyOriginatorKeyFile", string.Empty);
          testProjectMsb.SetProperty("SignAssembly", "false");
          testProjectMsb.Save();

          var bti = new BuildTimeInfo(
             BuildTimeUtc: DateTime.Parse("1/1/2015"),
             ExtraAttributes: MakeAttributes(false),
             FullVersion: "0.0.0-localbuild"
             );

          MakeBuildInfo(this.CoinbaseProject, bti);
       });

   Target Restore => _ => _
       .Executes(() =>
       {

          DotNetRestore(r => r
            .SetProjectFile(this.Solution)
          );


       });

   Target BuildInfo => _ => _
    .After(Restore)
    .Executes(() =>
    {
       //Debugger.Launch();
       var customAttributes = MakeAttributes(includeSnk: false);

       var fullVersion = BuildContext.GetFullVersion();

       var bti = new BuildTimeInfo(
          BuildTimeUtc: DateTime.UtcNow,
          ExtraAttributes: customAttributes,
          FullVersion: fullVersion
          );

       Log.Information("Build Time Info: {@BuildTimeInfo}", bti);

       //Debugger.Launch();
       MakeBuildInfo(this.CoinbaseProject, bti);

       var coinbaseProjectMsb = this.CoinbaseProject.GetMSBuildProject();
       coinbaseProjectMsb.SetProperty("Version", fullVersion);

       if( BuildContext.IsReleaseBuild )
       {
          var releaseNotes = History.NugetText(Files.History, GitHubUrl);
          coinbaseProjectMsb.SetProperty("PackageReleaseNotes", releaseNotes);
       }

       coinbaseProjectMsb.Save();
    });


   Target Test => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {

       var logFilePath = Folders.Test / "{assembly}.{framework}.results.xml";

       DotNetTest(t => t
         .SetProjectFile(this.TestProject)
         .EnableNoBuild()
         .AddLoggers($"nunit;LogFilePath=\"{logFilePath}\"",
                      "Appveyor")
         .SetTestAdapterPath(".")
       );

    });


   Target CI => _ => _
    .DependsOn(Test)
    .DependsOn(Zip)
    .DependsOn(Pack)    
    .Executes(() =>
    {

       //

    });



   AssemblyInfo.Attribute[] MakeAttributes(bool includeSnk)
   {
      var attributes = new List<AssemblyInfo.Attribute>
          {
             AssemblyInfo.Description(GitHubUrl)
          };

      string visiableTo;
      if (includeSnk)
      {
         var pubKeyBytes = File.ReadAllBytes(Files.SnkFilePublic);
         var pubKeyHex = Convert.ToHexString(pubKeyBytes).ToLowerInvariant();
         visiableTo = $"{TestProject.Name}, PublicKey={pubKeyHex}";
      }
      else
      {
         visiableTo = TestProject.Name;
      }

      var visiableToAsm = AssemblyInfo.InternalsVisibleTo(visiableTo);
      attributes.Add(visiableToAsm);
      return attributes.ToArray();
   }


}
