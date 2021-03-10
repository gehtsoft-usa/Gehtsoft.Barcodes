using System.IO;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

partial class Build
{
    [Parameter("The version of NuGet package")]
    readonly string Version;

    [Parameter("The fingerprint of certificate that was added to the 'Personal' certificate storage")]
    readonly string CertificateFingerprint;

    [Parameter("The name of published library")]
    readonly string PackageName = "Gehtsoft.Barcodes";
    
    AbsolutePath PackagesDirectory => OutputDirectory / "packages";
    AbsolutePath NuspecFile => RootDirectory / $"{PackageName}.nuspec";
    AbsolutePath NupkgFile => PackagesDirectory / $"{PackageName}.{Version}.nupkg";
    
    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Requires(() => Version)
        .Produces(NupkgFile)
        .Executes(() =>
        {
            NuGetPack(_ => _
                .SetOutputDirectory(PackagesDirectory)
                .SetConfiguration(Configuration)
                .DisableBuild()
                .SetVersion(Version)
                .SetTargetPath(NuspecFile));
        });

    Target Sign => _ => _
        .DependsOn(Pack)
        .Requires(() => CertificateFingerprint)
        .Requires(() => Equals(Configuration, Configuration.Release))
        .Executes(() =>
        {
            NuGetTasks.NuGet(
                $"sign \"{NupkgFile}\" -CertificateFingerprint \"{CertificateFingerprint}\" -Timestamper http://sha256timestamp.ws.symantec.com/sha256/timestamp",
                PackagesDirectory);
        });

    AbsolutePath LocalNuGetFeedPath => OutputDirectory / "local-barcodes-nuget-feed";
    Target AddNupkgToLocalNuGet => _ => _
        .DependsOn(Pack)
        .After(Sign)
        .Executes(() =>
        {
            if (!DirectoryExists(LocalNuGetFeedPath))
            {
                Directory.CreateDirectory(LocalNuGetFeedPath);
                NuGetSourcesAdd(_ => _
                    .SetName("Local Barcodes NuGet Feed")
                    .SetSource(LocalNuGetFeedPath));
            }

            NuGetTasks.NuGet($"add \"{NupkgFile}\" -source \"{LocalNuGetFeedPath}\"");
        });

    Target UsePackageInTests => _ => _
        .DependsOn(AddNupkgToLocalNuGet)
        .Executes(() =>
        {
            TestsDirectory.GlobFiles("**/*.csproj").ForEach(testProjectFile =>
            {
                DotNetTasks.DotNet($"remove \"{testProjectFile}\" reference \"{ProjectFile}\"");
                DotNetTasks.DotNet($"add \"{testProjectFile}\" package {PackageName} -v {Version} -s \"{LocalNuGetFeedPath}\"");
            });
        });

    Target RemoveNupkgFromLocalNuGet => _ => _
        .Requires(() => Version)
        .Requires(() => DirectoryExists(LocalNuGetFeedPath))
        .Executes(() =>
        {
            NuGetTasks.NuGet($"delete {PackageName} {Version} -NonInteractive -Source \"Local Barcodes NuGet Feed\"");
        });
}