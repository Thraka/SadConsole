# Delete any NuGet packages
Write-Output "Removing old nupkg files"
Remove-Item "SadConsole.Templates.*.nupkg" -Force

# Build SadConsole
Write-Output "Packing template project"
$output = Invoke-Expression "dotnet pack ..\templates\SadConsole.Templates.csproj -p:UseProjectReferences=false"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }

# Find the version we're using
$version = (Get-Content ..\templates\SadConsole.Templates.csproj | Select-String "<PackageVersion>(.*)<").Matches[0].Groups[1].Value
$nugetKey = Get-Content nuget.key
Write-Output "Template version is $version"

# Push packages to nuget
Write-Output "Pushing template package"
$sadConsolePackages = Get-ChildItem "SadConsole.Templates.$version.nupkg" | Select-Object -ExpandProperty Name

foreach ($package in $sadConsolePackages) {
    $output = Invoke-Expression "dotnet nuget push `"$package`" -s nuget.org -k $nugetKey --skip-duplicate"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
    
    # Archive the package
    Move-Item *.nupkg .\archive\ -force
}

Write-Output "Done"
