# Delete any NuGet packages
Write-Output "Removing old nupkg files"
Remove-Item "*.nupkg","*.snupkg" -Force

# Build SadConsole
Write-Output "Building SadConsole.Extended Debug and Release"
$output = Invoke-Expression "dotnet build ..\SadConsole.Extended\SadConsole.Extended.csproj -c Debug -p:UseProjectReferences=false"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
$output = Invoke-Expression "dotnet build ..\SadConsole.Extended\SadConsole.Extended.csproj -c Release -p:UseProjectReferences=false"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }

$nugetKey = Get-Content nuget.key

# Push packages to nuget
Write-Output "Pushing SadConsole packages"
$sadConsolePackages = Get-ChildItem "SadConsole.Extended*.nupkg" | Select-Object -ExpandProperty Name

foreach ($package in $sadConsolePackages) {
    $output = Invoke-Expression "dotnet nuget push `"$package`" -s nuget.org -k $nugetKey"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
}

Write-Output "Finished"

# Archive the packages
Move-Item "*.nupkg","*.snupkg" .\archive\ -force
