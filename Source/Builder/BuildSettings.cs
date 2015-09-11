using System;
using FluentBuild;
using FluentBuild.AssemblyInfoBuilding;
using FluentFs.Core;

namespace Builder
{
    public class Folders
    {
        public static readonly Directory WorkingFolder = new Directory( Properties.CurrentDirectory );
        public static readonly Directory CompileOutput = WorkingFolder.SubFolder( "__compile" );
        public static readonly Directory Package = WorkingFolder.SubFolder( "__package" );
        public static readonly Directory Source = WorkingFolder.SubFolder( "source" );

        public static readonly Directory Lib = Source.SubFolder( "packages" );
    }

    public class Projects
    {
        private static void GlobalAssemblyInfo(IAssemblyInfoDetails aid)
        {
            aid.Company( "Brian Chavez" )
               .Copyright( "Brian Chavez © " + DateTime.UtcNow.Year )
               .Version( Properties.CommandLineProperties.Version() )
               .FileVersion( Properties.CommandLineProperties.Version() )
               .InformationalVersion( "{0} built on {1} UTC".With( Properties.CommandLineProperties.Version(), DateTime.UtcNow ) )
               .Trademark( "MIT License" )
               .Description( "http://www.github.com/bchavez/Coinbase" )
               .ComVisible(false);
        }

        public static readonly File SolutionFile = Folders.Source.File( "Coinbase.sln" );

        public class CoinbaseProject
        {
            public static readonly Directory Folder = Folders.Source.SubFolder( "Coinbase" );
            public static readonly File ProjectFile = Folder.File( "Coinbase.csproj" );
            public static readonly Directory OutputDirectory = Folders.CompileOutput.SubFolder( "Coinbase" );
            public static readonly File OutputDll = OutputDirectory.File( "Coinbase.dll" );
            public static readonly Directory PackageDir = Folders.Package.SubFolder( "Coinbase" );
            
            public static readonly File NugetSpec = Folders.Source.SubFolder(".nuget").File( "Coinbase.nuspec" );
            public static readonly File NugetNupkg = Folders.Package.File( "Coinbase.{0}.nupkg".With( Properties.CommandLineProperties.Version() ) );

            public static readonly Action<IAssemblyInfoDetails> AssemblyInfo =
                i =>
                    {
                        i.Title("Coinbase API for .NET")
                            .Product("Coinbase API");

                        GlobalAssemblyInfo(i);
                    };
        }
        public class CoinbaseMvcProject
        {
            public static readonly Directory Folder = Folders.Source.SubFolder("Coinbase.Mvc");
            public static readonly File ProjectFile = Folder.File("Coinbase.Mvc.csproj");
            public static readonly Directory OutputDirectory = Folders.CompileOutput.SubFolder("Coinbase.Mvc");
            public static readonly File OutputDll = OutputDirectory.File("Coinbase.Mvc.dll");
            public static readonly Directory PackageDir = Folders.Package.SubFolder("Coinbase.Mvc");

            public static readonly File NugetSpec = Folders.Source.SubFolder(".nuget").File("Coinbase.Mvc.nuspec");
            public static readonly File NugetNupkg = Folders.Package.File("Coinbase.Mvc.{0}.nupkg".With(Properties.CommandLineProperties.Version()));

            public static readonly Action<IAssemblyInfoDetails> AssemblyInfo =
                i =>
                    {
                        i.Title("Coinbase.Mvc for .NET")
                            .Product("Coinbase.Mvc");

                    GlobalAssemblyInfo(i);
                };
        }
        public class Tests
        {
            public static readonly Directory Folder = Folders.Source.SubFolder( "Coinbase.Tests" );
        }
    }


}
