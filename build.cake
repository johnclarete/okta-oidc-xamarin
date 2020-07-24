// Arguments.
var target = Argument("target", "DefaultTarget");
var configuration = Argument("configuration", "Release");

// Define directories.
var solutionFile = GetFiles("./Okta.Xamarin/*.sln").First();

// Common, Android and iOS.
var commonProject = GetFiles("./Okta.Xamarin/Okta.Xamarin/Okta.Xamarin.csproj").First();
var androidProject = GetFiles("./Okta.Xamarin/Okta.Xamarin.Android/Okta.Xamarin.Android.csproj").First();
var iOSProject = GetFiles("./Okta.Xamarin/Okta.Xamarin.iOS/Okta.Xamarin.iOS.csproj").First();

// Output folders.
var artifactsDirectory = Directory(System.IO.Path.Combine(Environment.CurrentDirectory, "artifacts"));
var solutionOutputDirectory = Directory(System.IO.Path.Combine(artifactsDirectory, "SolutionOutput")); 
var commonOutputDirectory = Directory(System.IO.Path.Combine(artifactsDirectory, "Common"));
var androidOutputDirectory = Directory(System.IO.Path.Combine(artifactsDirectory, "Android"));
var iOSOutputDirectory = Directory(System.IO.Path.Combine(artifactsDirectory, "iOS"));

// Tests.
var testsProject = GetFiles("./Okta.Xamarin/Okta.Xamarin.Test/*.csproj").First();

Task("Clean")
    .Does(() => 
    {
        CleanDirectory(artifactsDirectory);

        MSBuild(solutionFile, settings => settings
            .SetConfiguration(configuration)
            .WithTarget("Clean")
            .SetVerbosity(Verbosity.Minimal)); 
    });

Task("Restore-Packages")
    .Does(() => 
    {
        NuGetRestore(solutionFile);
    });

Task("Build-Solution")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
    { 	
        MSBuild(solutionFile, settings =>
            settings
                .SetConfiguration(configuration)  
                .WithProperty("OutputPath", solutionOutputDirectory)         
                .WithProperty("DebugSymbols", "false")
                .WithProperty("TreatWarningsAsErrors", "false")
                .SetVerbosity(Verbosity.Minimal));
    });    

Task("Build-Common")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
    { 	
        MSBuild(commonProject, settings =>
            settings
                .SetConfiguration(configuration)  
                .WithProperty("OutputPath", commonOutputDirectory)
                .WithProperty("DebugSymbols", "false")
                .WithProperty("TreatWarningsAsErrors", "false")
                .SetVerbosity(Verbosity.Minimal));
    }); 

Task("Build-Android")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
    { 	
        MSBuild(androidProject, settings =>
            settings
                .SetConfiguration(configuration)  
                .WithProperty("OutputPath", androidOutputDirectory)         
                .WithProperty("DebugSymbols", "false")
                .WithProperty("TreatWarningsAsErrors", "false")
                .SetVerbosity(Verbosity.Minimal));
    });

Task("Build-iOS")
    .IsDependentOn("Restore-Packages")
    .Does (() =>
    {
        MSBuild(iOSProject, settings => 
            settings
                .SetConfiguration(configuration)   
                .WithTarget("Build")
                .WithProperty("Platform", "iPhoneSimulator")
                .WithProperty("OutputPath", iOSOutputDirectory)
                .WithProperty("TreatWarningsAsErrors", "false")
                .SetVerbosity(Verbosity.Minimal));
    });

Task("Run-Tests")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
    {		
        DotNetCoreTest(testsProject.FullPath, 
            new DotNetCoreTestSettings()
            {
                Configuration = configuration
                //NoBuild = true // Running tests will build the test project first, uncomment this line if this behavior should change
            });
    });

Task("CommonTarget")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-Common")
    .IsDependentOn("Run-Tests");

Task("AndroidTarget")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-Android")
    .IsDependentOn("Run-Tests");

Task("iOSTarget")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-iOS")
    .IsDependentOn("Run-Tests");

Task("DefaultTarget")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-Solution")
    .IsDependentOn("Run-Tests");

Console.WriteLine("Cake target is " + target);
RunTarget(target);
