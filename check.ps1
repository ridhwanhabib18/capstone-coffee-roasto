# Everything that can be checked without a roaster: restore, build, tests, firmware simulation.
#
#   ./check.ps1             all of it
#   ./check.ps1 -Quick      skip the firmware simulation (it needs the Visual Studio C++ build tools)
#
# Restore runs first and on its own: restoring and building the WPF project in one dotnet command can fail on a
# clean checkout, restore-then-build always works.
param([switch]$Quick)
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

dotnet restore CoffeeRoasto.slnx -nologo -v q
if ($LASTEXITCODE -ne 0) { throw "restore failed" }

dotnet build CoffeeRoasto.slnx --no-restore -nologo -v q
if ($LASTEXITCODE -ne 0) { throw "build failed" }

dotnet test CoffeeRoasto.slnx --no-build -nologo -v q
if ($LASTEXITCODE -ne 0) { throw "tests failed" }

if (-not $Quick) {
    & (Join-Path $PSScriptRoot "tests\firmware-sim\run.ps1")
    if ($LASTEXITCODE -ne 0) { throw "firmware simulation failed" }
}
"all checks passed"
