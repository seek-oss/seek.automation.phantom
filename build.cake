var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var projectName = "seek.automation.phantom";
var projectDescription = "A pact-based service simulator";

var releaseNotes = ParseReleaseNotes("ReleaseNotes.md");
var buildNumber = EnvironmentVariable("BUILD_NUMBER") ?? "0";
var version = string.Format("{0}.{1}", releaseNotes.Version, buildNumber);

Setup(context =>
			{
				CopyFileToDirectory("tools/nuget.exe", "tools/cake/");

				NuGetInstall("xunit.runner.console", new NuGetInstallSettings {
										ExcludeVersion  = true,
										OutputDirectory = "./tools"
								});
			}
	 );

Teardown(context =>
				{
					Information("Completed!");
				}
		);

Task("Clean")
	.Does(() =>
				{
					CleanDirectories(string.Format("{0}/**/bin", projectName));
					CleanDirectories(string.Format("{0}/**/obj", projectName));
				}
		 );

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
				{
					NuGetRestore(string.Format("src/{0}.sln", projectName));
				}
		 );

Task("AssemblyInfo")
    .IsDependentOn("Restore-NuGet-Packages")
	.Does(() => 
		{
			CreateAssemblyInfo(string.Format("src/{0}/Properties/AssemblyInfo.cs", projectName), new AssemblyInfoSettings  
			{
				Title = projectName,
				Description = projectDescription,
				Guid = "4dd5b14a-ef02-4ce0-9c33-52a4a4ea05f5",
				Product = projectName,
				Version = version,
				FileVersion = version
			});
		});

Task("Build-Solution")
    .IsDependentOn("AssemblyInfo")
    .Does(() =>
				{
					MSBuild(string.Format("src/{0}.sln", projectName), new MSBuildSettings()
						.UseToolVersion(MSBuildToolVersion.NET46)
						.SetVerbosity(Verbosity.Minimal)
						.SetConfiguration(configuration)
						);
				}
		 );

Task("Run-Unit-Tests")
    .IsDependentOn("Build-Solution")
    .Does(() =>
				{
					XUnit2(string.Format("src/{0}.tests/**/bin/{1}/*.Tests.dll", projectName, configuration));
				}
		 );


Task("Default")
    .IsDependentOn("Run-Unit-Tests");

RunTarget(target);
