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

.PARAMETER FailedXmlDir
    Directory to save copies of malformed XML files for debugging.
    Created automatically if failures are found. Defaults to "failed-xml"
#>
param(
    [string]$Path = ".",
    [string]$PackagePattern = "SadConsole*.nupkg",
    [string]$FailedXmlDir = "failed-xml"
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

                    # Use XmlReader for clean error messages (no full-content dump)
                    $settings = New-Object System.Xml.XmlReaderSettings
                    $settings.DtdProcessing = [System.Xml.DtdProcessing]::Ignore
                    $stringReader = New-Object System.IO.StringReader($content)
                    $xmlReader = [System.Xml.XmlReader]::Create($stringReader, $settings)
                    try {
                        while ($xmlReader.Read()) { }
                    }
                    finally {
                        $xmlReader.Dispose()
                        $stringReader.Dispose()
                    }
                    Write-Host "      OK" -ForegroundColor Green
                    $checkedCount++
                }
                catch {
                    # Extract just the line/position info, not the entire XML content
                    $msg = $_.Exception.InnerException ? $_.Exception.InnerException.Message : $_.Exception.Message
                    Write-Host "      INVALID XML: $msg" -ForegroundColor Red

                    # Save the malformed XML for artifact upload
                    if (-not (Test-Path $FailedXmlDir)) {
                        New-Item -ItemType Directory -Path $FailedXmlDir -Force | Out-Null
                    }
                    $safeName = "$($nupkg.BaseName)_$($entry.FullName -replace '[/\\]', '_')"
                    $failedPath = Join-Path $FailedXmlDir $safeName
                    Set-Content -LiteralPath $failedPath -Value $content -Encoding UTF8
                    Write-Host "      Saved to: $failedPath" -ForegroundColor Yellow

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
