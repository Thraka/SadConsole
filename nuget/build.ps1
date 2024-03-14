# Delete any NuGet packages
Write-Output "Removing old nupkg files"
Remove-Item "*.nupkg","*.snupkg" -Force

# Build SadConsole
Write-Output "Building SadConsole Debug and Release"
$output = Invoke-Expression "dotnet restore ..\SadConsole\SadConsole.csproj -c Debug --no-cache"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
$output = Invoke-Expression "dotnet build ..\SadConsole\SadConsole.csproj -c Debug"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
$output = Invoke-Expression "dotnet restore ..\SadConsole\SadConsole.csproj -c Release --no-cache"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
$output = Invoke-Expression "dotnet build ..\SadConsole\SadConsole.csproj -c Release"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }

# Find the version we're using
$version = (Get-Content ..\SadConsole\SadConsole.csproj | Select-String "<Version>(.*)<").Matches[0].Groups[1].Value
$nugetKey = Get-Content nuget.key
Write-Output "Target SadConsole version is $version"

# Push packages to nuget
Write-Output "Pushing SadConsole packages"
$sadConsolePackages = Get-ChildItem "SadConsole.*.nupkg" | Select-Object -ExpandProperty Name

foreach ($package in $sadConsolePackages) {
    $output = Invoke-Expression "dotnet nuget push `"$package`" -s nuget.org -k $nugetKey"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
}

Write-Output "Query NuGet for 10 minutes to find the new package"

$timeout = New-TimeSpan -Minutes 10
$timer = [Diagnostics.StopWatch]::StartNew()
[Boolean]$foundPackage = $false

# Loop searching for the new SadConsole package
while ($timer.elapsed -lt $timeout){

    $existingVersions = (Invoke-WebRequest "https://api-v2v3search-0.nuget.org/query?q=PackageId:SadConsole&prerelease=true").Content | ConvertFrom-Json

    if ($existingVersions.totalHits -eq 0) {
        throw "Unable to get any results from NuGet"
    }

    if ($null -eq ($existingVersions.data.versions | Where-Object version -eq $version)) {
        Write-Output "Waiting 30 seconds to retry..."
        Start-Sleep -Seconds 30
    }
    else {
        Write-Output "Found package. Waiting 1 extra minute to let things settle"
        $foundPackage = $true
        Start-Sleep -Seconds 60
        break
    }
}

# Found the SadConsole package, start building and pushing the other packages
if ($foundPackage){

    $projects = "SadConsole.Extended", "SadConsole.Host.MonoGame", "SadConsole.Host.SFML", "SadConsole.Host.FNA"

    foreach ($project in $projects) {
            
        # SadConsole Extended
        Write-Output "Building $project Debug and Release"
        $output = Invoke-Expression "dotnet restore ..\$project\$project.csproj -c Debug -p:UseProjectReferences=false --no-cache"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
        $output = Invoke-Expression "dotnet build ..\$project\$project.csproj -c Debug -p:UseProjectReferences=false"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
        $output = Invoke-Expression "dotnet restore ..\$project\$project.csproj -c Release -p:UseProjectReferences=false --no-cache"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
        $output = Invoke-Expression "dotnet build ..\$project\$project.csproj -c Release -p:UseProjectReferences=false"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }

        # Push packages to nuget
        Write-Output "Pushing SadConsole packages"
        $sadConsolePackages = Get-ChildItem "$project*.nupkg" | Select-Object -ExpandProperty Name

        foreach ($package in $sadConsolePackages) {
            $output = Invoke-Expression "dotnet nuget push `"$package`" -s nuget.org -k $nugetKey"; if ($LASTEXITCODE -ne 0) { Write-Error "Failed"; Write-Output $output; throw }
        }

    }

    # Archive the packages
    Move-Item "*.nupkg","*.snupkg" .\archive\ -force
}