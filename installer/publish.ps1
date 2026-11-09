# Builds Coffee Roasto v2.0 as one self-contained .exe: the PC of the operator does not need .NET installed.
#
#   ./publish.ps1                  -> installer/dist/CoffeeRoasto.exe (about 150 MB, Windows 10 1607 or newer, 64-bit)
#   ./publish.ps1 -Runtime win-arm64
#
# The old ClickOnce installer of v1.0 is kept in installer/v1-clickonce/ for reference.
param([string]$Runtime = "win-x64")
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$out = Join-Path $PSScriptRoot "dist"

dotnet publish (Join-Path $root "src\CoffeeRoasto.App\CoffeeRoasto.App.csproj") `
    -c Release -r $Runtime --self-contained true `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true `
    -o $out
if ($LASTEXITCODE -ne 0) { throw "publish failed" }

$exe = Join-Path $out "CoffeeRoasto.exe"
"{0}: {1:N0} MB" -f $exe, ((Get-Item $exe).Length / 1MB)
