using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions("build",
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.MacOsLatest,
    InvokedTargets = new[] {nameof(Compile)},
    OnPushBranches = new[] {"main"})]
[GitHubActions("test",
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.MacOsLatest,
    InvokedTargets = new[] {nameof(Test)},
    OnPushBranches = new[] {"main"})]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Release'")]
    readonly Configuration Configuration = Configuration.Release;

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath ProjectFile => SourceDirectory / "Gehtsoft.Barcodes.csproj";

    Target Compile => _ => _
        .Executes(() =>
        {
            MSBuildTasks.MSBuild(_ => _
                .SetTargets("Clean", "Rebuild")
                .EnableNoLogo()
                .SetWarningLevel(0)
                .EnableRestore()
                .SetProjectFile(ProjectFile)
                .SetConfiguration(Configuration));
        });
    
    IEnumerable<Project> TestProjects => Solution.GetProjects("*Tests");
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";
    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Executes(() =>
        {
            EnsureCleanDirectory(TestResultDirectory);
            DotNetTest(_ => _
                    .SetConfiguration(Configuration)
                    .SetVerbosity(DotNetVerbosity.Minimal)
                    .SetNoBuild(InvokedTargets.Contains(Compile))
                    .SetResultsDirectory(TestResultDirectory)
                    .CombineWith(TestProjects, (_, v) => _
                        .SetProjectFile(v)
                        .SetLogger($"trx;LogFileName={v.Name}.trx")),
                completeOnFailure: false);
        });
}
