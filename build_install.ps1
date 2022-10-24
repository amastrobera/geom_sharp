#  Script to deploy the Revit solution
#  - Why using it: testing the Release version of the DLL on the developer's computer
#  - Where to use it: you are developing on Windows, and have Revit on (the same) Windows
# author: Angelo Mastroberardino
# version: 2.0
# requires: 
#  - Windows 7+
#  - powershell (usually included) 
#     - set .ps1 files to be executed by powershell.exe
#     - Set-ExecutionPolicy RemoteSigned (Y) from Admin powershell

$app_main = "GeomSharp"

$build_config = $args[0]
if ([string]::IsNullOrEmpty($build_config) -or $build_config -ieq "Release") {
    $build_config = "Release"
} elseif ($build_config -ieq "Debug") {
    $build_config = "Debug"
} else {
    Write-Host "Invalid build configuration:" $build_config
    pause
    exit 1  
}
Write-Host "Build configuration:" $build_config

# rebuild the app
Write-Host "====== installing app ======"

$msbuild = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
Write-Host "    found msbuild: [$msbuild]"

&$msbuild -t:build -restore $app_main -property:Configuration=$build_config
Write-Host "... done"


Write-Host "================================="
Write-Host "... finished"
pause
exit 0

