//#if INTERACTIVE
//open System
//Environment.CurrentDirectory <- workingDir
//#else
//#endif

// include Fake lib
#I @"../packages/build/FAKE/tools"
#I @"../packages/build/DotNetZip/lib/net20"
#r @"FakeLib.dll"
#r @"DotNetZip.dll"

#load @"Utils.fsx"

open Fake
open Utils
open System.Reflection
open Helpers
open Fake.Testing.NUnit3

let workingDir = ChangeWorkingFolder();

trace (sprintf "WORKING DIR: %s" workingDir)

let ProjectName = "Coinbase";
let GitHubUrl = "https://github.com/bchavez/Coinbase"

let Folders = Setup.Folders(workingDir)
let Files = Setup.Files(Folders)
let Projects = Setup.Projects(ProjectName, Folders)

let CoinbaseProject = NugetProject("Coinbase", "Coinbase API for .NET", Folders)
let TestProject = TestProject("Coinbase.Tests", Folders)


Target "dnx" (fun _ ->
    trace "DNX Build Task"

    let tag = "dnx_build"
    
    DotnetBuild CoinbaseProject tag
)

Target "restore" (fun _ -> 
     trace ".NET Core Restore"
     DotnetRestore CoinbaseProject
     DotnetRestore TestProject
 )

open Ionic.Zip
open System.Xml

Target "nuget" (fun _ ->
    trace "NuGet Task"
    
    DotnetPack CoinbaseProject Folders.Package   
)

Target "push" (fun _ ->
    trace "NuGet Push Task"
    
    failwith "Only CI server should publish on NuGet"
)



Target "zip" (fun _ -> 
    trace "Zip Task"

    !!(CoinbaseProject.OutputDirectory @@ "**") |> Zip Folders.CompileOutput (Folders.Package @@ CoinbaseProject.Zip)
)

open AssemblyInfoFile

let MakeAttributes (includeSnk:bool) =
    let attrs = [
                    Attribute.Description GitHubUrl
                ]
    if includeSnk then
        let pubKey = ReadFileAsHexString Projects.SnkFilePublic
        let visibleTo = sprintf "%s, PublicKey=%s" TestProject.Name pubKey
        attrs @ [ Attribute.InternalsVisibleTo(visibleTo) ]
    else
        attrs @ [ Attribute.InternalsVisibleTo(TestProject.Name) ]


Target "BuildInfo" (fun _ ->
    
    trace "Writing Assembly Build Info"

    MakeBuildInfo CoinbaseProject Folders (fun bip -> 
        { bip with
            ExtraAttrs = MakeAttributes(false) } )//BuildContext.IsTaggedBuild) } )

    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/Version" BuildContext.FullVersion

    let releaseNotes = History.NugetText Files.History GitHubUrl
    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/PackageReleaseNotes" releaseNotes
)


Target "Clean" (fun _ ->
    DeleteFile Files.TestResultFile
    CleanDirs [Folders.CompileOutput; Folders.Package]

    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/Version" "0.0.0-localbuild"
    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/PackageReleaseNotes" ""
    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/AssemblyOriginatorKeyFile" ""
    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/SignAssembly" "false"

    MakeBuildInfo CoinbaseProject Folders (fun bip ->
         {bip with
            DateTime = System.DateTime.Parse("1/1/2015")
            ExtraAttrs = MakeAttributes(false) } )

)

open Fake.Testing
open Fake.AppVeyor

Target "ci" (fun _ ->
    trace "ci Task"
)

Target "test" (fun _ ->
    trace "TEST"
    CreateDir Folders.Test

    DotNetCli.Test( fun p ->
    { p with
       WorkingDir = TestProject.Folder
       AdditionalArgs = [
                          "--test-adapter-path:."
                          sprintf "--logger:\"nunit;LogFilePath=%s\"" Files.TestResultFile
                        ]
    })
)

Target "citest" (fun _ ->
    trace "CI TEST"
    
    DotNetCli.Test( fun p ->
    { p with
       WorkingDir = TestProject.Folder
       AdditionalArgs = [
                          "--test-adapter-path:."
                          "--logger:Appveyor"
                        ]
    })
)


Target "setup-snk"(fun _ ->
    trace "Decrypting Strong Name Key (SNK) file."
    let decryptSecret = environVarOrFail "SNKFILE_SECRET"
    decryptFile Projects.SnkFile decryptSecret

    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/AssemblyOriginatorKeyFile" Projects.SnkFile
    XmlPokeInnerText CoinbaseProject.ProjectFile "/Project/PropertyGroup/SignAssembly" "true"
)


"Clean"
    ==> "restore"
    ==> "BuildInfo"

//build systems, order matters
"BuildInfo"
    //=?> ("setup-snk", BuildContext.IsTaggedBuild)
    ==> "dnx"
    ==> "zip"

"BuildInfo"
    //=?> ("setup-snk", BuildContext.IsTaggedBuild)
    ==> "zip"

"dnx"
    ==> "nuget"


"nuget"
    ==> "ci"

"nuget"
    ==> "push"

"zip"
    ==> "ci"


//test task depends on msbuild
"dnx"
    ==> "test"


// start build
RunTargetOrDefault "dnx"
