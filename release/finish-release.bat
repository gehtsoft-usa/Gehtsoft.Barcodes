@ECHO off

rem Navigate to root directory
cd ..

rem Use the Gehtsoft.Barcodes package from 'nuget.org' instead of local NuGet feed
nuke RemoveNupkgFromLocalNuGet --no-logo

rem Rebuild the project and run 'Test' target again, with the package reference taken from nuget.org, not a local feed,
rem to ensure that everything works as expected after the publication to the global NuGet feed.
nuke Test --no-logo
pause