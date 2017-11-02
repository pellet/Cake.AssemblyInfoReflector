#addin "Cake.FileHelpers"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionFile = File("./src/cake.AssemblyInfoReflector.sln");
var solutionInfo = ParseSolution(solutionFile);

var solutionProject = solutionInfo.Projects.First();

Information("Building project '{0}'", solutionProject.Name);

var projectFile = solutionProject.Path;
var projectInfo = ParseProject(projectFile);

var nuSpecFile = projectFile.ChangeExtension(".nuspec");

var assemblyInfoFile = projectFile.GetDirectory().Combine("properties").CombineWithFilePath("assemblyInfo.cs");
var assemblyInfo = ParseAssemblyInfo(assemblyInfoFile);

Task("Build")
 //   .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        MSBuild(solutionFile, settings => settings.SetConfiguration(configuration));
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>{
        var nuGetPackSettings = new NuGetPackSettings {
            Id = solutionProject.Name,
            Version = assemblyInfo.AssemblyVersion, // überschreibt den Wert aus der .nuspec-Datei
            BasePath = "./src/" + solutionProject.Name,
            OutputDirectory = "./src/" + solutionProject.Name + "/bin/" + configuration,
            Properties = new Dictionary<string, string>
	  		{
	    		{ "Configuration", configuration }		// "debug" oder "release" festlegen für NuGet
	  		}
        };
        NuGetPack(nuSpecFile, nuGetPackSettings);
});

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);