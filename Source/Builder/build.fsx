﻿//#if INTERACTIVE
//open System
//Environment.CurrentDirectory <- workingDir
//#else
//#endif

// include Fake lib

#r "paket:

nuget Fake.Core                    
nuget Fake.Core.Target             
nuget Fake.Core.Xml                
nuget Fake.Runtime                 
nuget Fake.DotNet.NuGet            
nuget Fake.DotNet.Cli              
nuget Fake.DotNet.AssemblyInfoFile 
nuget Fake.DotNet.MSBuild          
nuget Fake.IO.FileSystem           
nuget Fake.IO.Zip                  
nuget Fake.Tools.Git               
nuget Fake.DotNet.Testing.xUnit2   
nuget Fake.BuildServer.AppVeyor    

nuget SharpCompress = 0.35.0
nuget FSharp.Data = 6.3.0

nuget secure-file                  = 1.0.31

nuget Z.ExtensionMethods.WithTwoNamespace
nuget System.Runtime.Caching //"

#load ".\\.fake\\build.fsx\\intellisense.fsx"

#load @"Utils.fsx"

#if !FAKE
  #r "netstandard"
#endif

open Fake
open Utils
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.DotNet
open Fake.Core
open Fake.IO.Globbing
open Utils
open Z.ExtensionMethods

open System.Reflection
open System.Collections.Generic
open SharpCompress
open SharpCompress.Archives
open Newtonsoft.Json

open Fake.BuildServer

BuildServer.install[
   AppVeyor.Installer
]

let workingDir = ChangeWorkingFolder();

Trace.trace (sprintf "WORKING DIR: %s" workingDir)

let ProjectName = "Coinbase";
let GitHubUrl = "https://github.com/bchavez/Coinbase"

let Folders = Setup.Folders(workingDir)
let Files = Setup.Files(ProjectName, Folders)

let CoinbaseProject = NugetProject("Coinbase", "Coinbase API for .NET", Folders)
let TestProject = TestProject("Coinbase.Tests", Folders)

let EnableSigning = false

Target.description "MAIN BUILD TASK"
Target.create "dnx" (fun _ ->
    Trace.trace "DNX Build Task"


    
    let msbParams = { MSBuild.CliArguments.Create()
                      with
                        NoWarn = Some([
                                       "CS1573"; //Disable XML doc warning
                                       "CS1591"; //Disable XML doc warning
                                      ]) 
                    }

    let releaseConfig (opts : DotNet.BuildOptions) =
        {opts with 
              Configuration = DotNet.BuildConfiguration.Release
              MSBuildParams = msbParams
        }
    
    let debugConfig (opts : DotNet.BuildOptions) =
        { opts with 
               Configuration = DotNet.BuildConfiguration.Debug 
               MSBuildParams = msbParams 
        }

    DotNet.build releaseConfig CoinbaseProject.Folder
    DotNet.build debugConfig CoinbaseProject.Folder

    Shell.copyDir CoinbaseProject.OutputDirectory (CoinbaseProject.Folder @@ "bin") FileFilter.allFiles

    DotNet.build debugConfig TestProject.Folder
)

Target.description "NUGET PACKAGE RESTORE TASK"
Target.create "restore" (fun _ -> 
     Trace.trace ".NET Core Restore"
     
     DotNet.restore id CoinbaseProject.Folder
     DotNet.restore id TestProject.Folder
)

open System.Xml

Target.description "NUGET PACKAGE TASK"
Target.create "nuget" (fun _ ->
    Trace.trace "NuGet Task"

    let config (opts: DotNet.PackOptions) =
         {opts with
               Configuration = DotNet.BuildConfiguration.Release
               OutputPath = Some(Folders.Package) }
    
    DotNet.pack config CoinbaseProject.Folder
)


Target.description "PROJECT ZIP TASK"
Target.create "zip" (fun _ -> 
    Trace.trace "Zip Task"

    !!(CoinbaseProject.OutputDirectory @@ "**")
    |> Zip.zip Folders.CompileOutput (Folders.Package @@ CoinbaseProject.Zip)
)


let MakeAttributes (includeSnk:bool) =
    let attrs = [
                    AssemblyInfo.Description GitHubUrl
                ]
    if includeSnk then
        let pubKey = ReadFileAsHexString Files.SnkFilePublic
        let visibleTo = sprintf "%s, PublicKey=%s" TestProject.Name pubKey
        attrs @ [ AssemblyInfo.InternalsVisibleTo(visibleTo) ]
    else
        attrs @ [ AssemblyInfo.InternalsVisibleTo(TestProject.Name) ]


Target.description "PROJECT BUILDINFO TASK"
Target.create "BuildInfo" (fun _ ->
    
    Trace.trace "Writing Assembly Build Info"
    
    let includeSnk = EnableSigning && BuildContext.IsTaggedBuild

    let customAttributes = MakeAttributes(includeSnk)

    MakeBuildInfo CoinbaseProject Folders (fun bip -> { bip with ExtraAttrs = customAttributes } )

    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/Version" BuildContext.FullVersion

    let releaseNotes = History.NugetText Files.History GitHubUrl
    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/PackageReleaseNotes" releaseNotes
)


Target.description "PROJECT CLEAN TASK"
Target.create "Clean" (fun _ ->
    File.delete Files.TestResultFile
    Shell.cleanDirs [Folders.CompileOutput; Folders.Package;]

    let projects = [CoinbaseProject.Folder; TestProject.Folder;]

    for project in projects do
      Shell.cleanDir (project @@ "bin")
      Shell.cleanDir (project @@ "obj")

    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/Version" "0.0.0-localbuild"
    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/PackageReleaseNotes" ""
    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/AssemblyOriginatorKeyFile" ""
    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/SignAssembly" "false"

    Xml.pokeInnerText TestProject.ProjectFile "/Project/PropertyGroup/AssemblyOriginatorKeyFile" ""
    Xml.pokeInnerText TestProject.ProjectFile "/Project/PropertyGroup/SignAssembly" "false"

    MakeBuildInfo CoinbaseProject Folders (fun bip ->
         {bip with
            DateTime = System.DateTime.Parse("1/1/2015")
            ExtraAttrs = MakeAttributes(false) 
            VersionContext = Some "0.0.0-localbuild" })

)

open Fake.DotNet.Testing

let RunTests(logger: string option) =
   let config (opts: DotNet.TestOptions) =
      {opts with  
            NoBuild = true
            Logger = logger}
   DotNet.test config TestProject.Folder

open Fake.BuildServer

Target.create "ci" (fun _ ->
    Trace.trace "ci Task"
)

Target.description "PROJECT TEST TASK"
Target.create "test" (fun _ ->
    Trace.trace "TEST"
    RunTests(None)
)

Target.description "CI TEST TASK"
Target.create "citest" (fun _ ->
    Trace.trace "CI TEST"
    RunTests(Some "Appveyor")
)


Target.description "PROJECT SIGNING KEY SETUP TASK"
Target.create "setup-snk"(fun _ ->
    Trace.trace "Decrypting Strong Name Key (SNK) file."
    let decryptSecret = Environment.environVarOrFail "SNKFILE_SECRET"
    Helpers.decryptFile Files.SnkFile decryptSecret
    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/AssemblyOriginatorKeyFile" Files.SnkFile
    Xml.pokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/SignAssembly" "true"

    Xml.pokeInnerText TestProject.ProjectFile "/Project/PropertyGroup/AssemblyOriginatorKeyFile" Files.SnkFile
    Xml.pokeInnerText TestProject.ProjectFile "/Project/PropertyGroup/SignAssembly" "true"
)

open Fake.Core.TargetOperators

// Build order and dependencies are read from left to right. For example,
// Do_This_First ==> Do_This_Second ==> Do_This_Third
//     Clean     ==>     Restore    ==>     Build
//
// REFERENCE:
//  ( ==> ) x y               | Defines a dependency - y is dependent on x
//  ( =?> ) x (y, condition)  | Defines a conditional dependency - y is dependent on x if the condition is true
//  ( <== ) x y
//  ( <=> ) x y               | Defines that x and y are not dependent on each other but y is dependent on all dependencies of x.
//  ( <=? ) y x
//  ( ?=> ) x y               | Defines a soft dependency. x must run before y, if it is present, but y does not require x to be run.


"Clean"  ==> "restore"  ==> "BuildInfo"

"BuildInfo" =?> ("setup-snk", EnableSigning && BuildContext.IsTaggedBuild) ==> "dnx" ==> "zip"

"dnx" ==> "nuget"

"dnx" ==> "test"

"nuget" <=> "zip" ==> "ci"


// start build
Target.runOrDefault "dnx"
