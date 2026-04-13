param(
    [string]$SourceDir = "src/Callen/bin/Release",
    [string]$OutputDir = "dist/callen",
    [string]$DatabaseFile = "data/Callen.db"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-RepoPath([string]$path) {
    return [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$path"))
}

$sourcePath = Resolve-RepoPath $SourceDir
$outputPath = Resolve-RepoPath $OutputDir
$dbPath = Resolve-RepoPath $DatabaseFile

if (-not (Test-Path $sourcePath)) {
    throw "Release output not found: $sourcePath`nBuild Release first in Visual Studio."
}

if (Test-Path $outputPath) {
    Remove-Item -LiteralPath $outputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $outputPath | Out-Null

# Copy all runtime files, then prune debug/build leftovers.
Copy-Item -Path (Join-Path $sourcePath "*") -Destination $outputPath -Recurse -Force

Get-ChildItem -Path $outputPath -File -Recurse |
    Where-Object {
        $_.Extension -in @(".pdb", ".xml") -or
        $_.Name -like "*.vshost.*" -or
        $_.Name -like "*.manifest"
    } |
    Remove-Item -Force

$dataDir = Join-Path $outputPath "data"
$imagesDir = Join-Path $outputPath "images"
New-Item -ItemType Directory -Path $dataDir -Force | Out-Null
New-Item -ItemType Directory -Path $imagesDir -Force | Out-Null

if (Test-Path $dbPath) {
    Copy-Item -LiteralPath $dbPath -Destination (Join-Path $dataDir "Callen.db") -Force
}

$configPath = Join-Path $outputPath "Callen.exe.config"
if (Test-Path $configPath) {
    [xml]$config = Get-Content -Raw $configPath
    if ($null -eq $config.configuration.appSettings) {
        $appSettings = $config.CreateElement("appSettings")
        $config.configuration.AppendChild($appSettings) | Out-Null
    }

    $dbSetting = $config.configuration.appSettings.add | Where-Object { $_.key -eq "database_path" }
    if ($null -eq $dbSetting) {
        $dbSetting = $config.CreateElement("add")
        $dbSetting.SetAttribute("key", "database_path")
        $dbSetting.SetAttribute("value", ".\data\Callen.db")
        $config.configuration.appSettings.AppendChild($dbSetting) | Out-Null
    } else {
        $dbSetting.value = ".\data\Callen.db"
    }

    $imageSetting = $config.configuration.appSettings.add | Where-Object { $_.key -eq "image_path" }
    if ($null -eq $imageSetting) {
        $imageSetting = $config.CreateElement("add")
        $imageSetting.SetAttribute("key", "image_path")
        $imageSetting.SetAttribute("value", ".\images")
        $config.configuration.appSettings.AppendChild($imageSetting) | Out-Null
    } else {
        $imageSetting.value = ".\images"
    }

    $config.Save($configPath)
}

Write-Host "Portable package created:" -ForegroundColor Green
Write-Host "  $outputPath"
Write-Host ""
Write-Host "Run: $([System.IO.Path]::Combine($outputPath, 'Callen.exe'))"
