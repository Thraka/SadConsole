<#
.SYNOPSIS
    Validates XML documentation files inside .nupkg packages.
    Opens each .nupkg as a zip and parses every .xml entry to catch malformed docs
    before they reach NuGet. Exits with code 1 if any XML is invalid.

.PARAMETER Path
    The directory to search for .nupkg files.
    Defaults to the current directory.

.PARAMETER PackagePattern
    Glob pattern for .nupkg files to inspect.
    Defaults to "SadConsole*.nupkg"
#>
param(
    [string]$Path = ".",
    [string]$PackagePattern = "SadConsole*.nupkg"
)

Add-Type -AssemblyName System.IO.Compression.FileSystem
$ErrorActionPreference = "Stop"
$failed = $false
$checkedCount = 0

Write-Host "`n=== Validating XML inside .nupkg packages ===" -ForegroundColor Cyan

$nupkgs = Get-ChildItem -Path $Path -Filter $PackagePattern -File |
          Where-Object { $_.Extension -eq ".nupkg" }

if ($nupkgs.Count -eq 0) {
    Write-Error "No .nupkg files matching '$PackagePattern' found in '$Path'"
    exit 1
}

foreach ($nupkg in $nupkgs) {
    Write-Host "  Inspecting: $($nupkg.Name)"
    try {
        $zip = [System.IO.Compression.ZipFile]::OpenRead($nupkg.FullName)
        try {
            $xmlEntries = $zip.Entries | Where-Object {
                $_.FullName -like "*.xml" -and
                $_.FullName -notlike "*.nuspec" -and
                $_.Length -gt 0
            }

            if ($xmlEntries.Count -eq 0) {
                Write-Warning "    No XML documentation entries found inside package"
                continue
            }

            foreach ($entry in $xmlEntries) {
                Write-Host "    Entry: $($entry.FullName)"
                try {
                    $stream = $entry.Open()
                    $reader = New-Object System.IO.StreamReader($stream)
                    $content = $reader.ReadToEnd()
                    $reader.Dispose()
                    $stream.Dispose()

                    $null = [xml]$content
                    Write-Host "      OK" -ForegroundColor Green
                    $checkedCount++
                }
                catch {
                    Write-Host "      INVALID XML: $($_.Exception.Message)" -ForegroundColor Red
                    $failed = $true
                }
            }
        }
        finally {
            $zip.Dispose()
        }
    }
    catch {
        Write-Host "    Failed to open package: $($_.Exception.Message)" -ForegroundColor Red
        $failed = $true
    }
}

# ── Result ───────────────────────────────────────────────────────────
Write-Host ""
if ($checkedCount -eq 0 -and -not $failed) {
    Write-Error "No XML documentation files were found inside any package."
    exit 1
}

if ($failed) {
    Write-Error "One or more XML documentation files inside .nupkg packages are malformed."
    exit 1
}

Write-Host "All $checkedCount XML documentation files are valid." -ForegroundColor Green
exit 0
