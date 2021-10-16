var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
    {
        CleanDirectory($"./src/PgPartner/bin/{configuration}");
        CleanDirectory($"./src/PgPartner.SampleApp/bin/{configuration}");
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreBuild("./src/PgPartner.SampleApp/PgPartner.SampleApp.sln", new DotNetCoreBuildSettings
        {
            Configuration = configuration,
        });
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);