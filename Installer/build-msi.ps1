param(
    [string]$Version = "0.2.0",
    [switch]$SelfContained
)

$ErrorActionPreference = "Stop"

$installerDir = $PSScriptRoot
$repoRoot = Split-Path -Parent $installerDir
$publishDir = Join-Path $installerDir "publish"
$outDir = Join-Path $repoRoot "dist"
$wix = (Get-Command wix -ErrorAction SilentlyContinue)?.Source
if (-not $wix) {
    $wix = Join-Path $env:USERPROFILE ".dotnet\tools\wix.exe"
}
if (-not (Test-Path $wix)) {
    throw "wix tool not found. Install with: dotnet tool install --global wix"
}

New-Item -ItemType Directory -Force -Path $publishDir, $outDir | Out-Null

$publishArgs = @(
    "publish", (Join-Path $repoRoot "TimeClock"),
    "--configuration", "Release",
    "--runtime", "win-x64",
    "-p:PublishSingleFile=true",
    "-p:DebugType=none",
    "-o", $publishDir
)
if ($SelfContained) {
    $publishArgs += "--self-contained", "true", "-p:EnableCompressionInSingleFile=true", "-p:IncludeNativeLibrariesForSelfExtract=true"
}
else {
    $publishArgs += "--self-contained", "false", "-p:StripSymbols=true"
    $env:DOTNET_ROLL_FORWARD = "Disable"
}

Write-Host "==> dotnet publish ($($(if ($SelfContained) {'self-contained'} else {'framework-dependent'})))"
try {
    & dotnet @publishArgs
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
}
finally {
    Remove-Item Env:DOTNET_ROLL_FORWARD -ErrorAction SilentlyContinue
}

$suffix = if ($SelfContained) { "-sc" } else { "" }
$msiPath = Join-Path $outDir "TimeClock-$Version-x64$suffix.msi"

Write-Host "==> wix build"
Push-Location $installerDir
try {
    & $wix build (Join-Path $installerDir "Product.wxs") `
        -arch x64 `
        -d MsiVersion=$Version `
        -d PublishDir=$publishDir `
        -o $msiPath
    if ($LASTEXITCODE -ne 0) { throw "wix build failed" }
}
finally {
    Pop-Location
}

$size = "{0:N1}" -f ((Get-Item $msiPath).Length / 1MB)
Write-Host "==> MSI created: $msiPath ($size MB)"
