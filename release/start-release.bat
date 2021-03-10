@ECHO off

rem Navigate to root directory
cd ..

rem Run 'Test' target
nuke Test --no-logo
pause

rem Pack, sign the package and add it to the local NuGet feed
rem Then install the package in all test projects
nuke Pack Sign AddNupkgToLocalNuGet UsePackageInTests --no-logo

rem Rebuild the project and run 'Test' target again, with the package reference this time, to ensure that everything works as
rem expected after the publication to the NuGet feed.
nuke Test --no-logo
pause

rem Now upload the generated .nupkg file (that is located in output/packages/ folder) to NuGet via the web browser.
rem When the uploaded package is indexed and listed, run the finish-release.bat script.