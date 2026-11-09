# Runs the firmware simulation and compares every trace with golden/.
#
#   ./run.ps1                 compile the current firmware, run scenarios 1-12, compare
#   ./run.ps1 -Sketch <dir>   use another sketch folder (for example an older copy)
#   ./run.ps1 -Update         write the traces to golden/ instead of comparing (only after a deliberate change)
#
# Needs the Visual Studio C++ build tools (cl.exe). No board and no Arduino IDE are needed.
param(
    [string]$Sketch = (Join-Path $PSScriptRoot "..\..\firmware\Program_PID_CP"),
    [switch]$Update
)
$ErrorActionPreference = "Stop"
$here = $PSScriptRoot
$out = Join-Path $here "out"
New-Item -ItemType Directory -Force $out | Out-Null
$Sketch = (Resolve-Path $Sketch).Path

# 1. One translation unit, like the Arduino build: the main tab first, then the other tabs.
$mainFile = Join-Path $Sketch "Program_PID_CP.ino"
$main = [IO.File]::ReadAllText($mainFile)
$main = $main -replace '#include "ReferenceProfile.h"', "#include `"ReferenceProfile.h`"`n#include `"prototypes.h`""
$tabs = Get-ChildItem $Sketch -Filter *.ino | Where-Object { $_.Name -ne "Program_PID_CP.ino" } | Sort-Object Name
$sketchCpp = Join-Path $out "sketch.cpp"
$text = $main + "`n" + (($tabs | ForEach-Object { [IO.File]::ReadAllText($_.FullName) }) -join "`n")
[IO.File]::WriteAllText($sketchCpp, $text)

# 2. Compile with MSVC.
$vcvars = Get-ChildItem "C:\Program Files*\Microsoft Visual Studio\*\*\VC\Auxiliary\Build\vcvars64.bat" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $vcvars) { throw "vcvars64.bat not found: install the Visual Studio C++ build tools" }
$exe = Join-Path $out "firmware-sim.exe"
$cl = "call `"$($vcvars.FullName)`" >nul 2>&1 && cl /nologo /EHsc /O1 /w /I`"$here\shim`" /I`"$Sketch`" /I`"$here`" /DSKETCH=\`"$sketchCpp\`" `"$here\harness.cpp`" /Fe:`"$exe`" /Fo:`"$out\harness.obj`" 2>&1"
$log = cmd /c $cl
if (-not (Test-Path $exe)) { $log | Select-Object -Last 15; throw "compile failed" }

# 3. Run every scenario and compare.
$failed = 0
foreach ($n in 1..12) {
    $trace = Join-Path $out "scenario$n.txt"
    & $exe $n (Join-Path $here "demo-profile.txt") | Out-File -Encoding ascii $trace
    $golden = Join-Path $here "golden\scenario$n.txt"
    if ($Update) {
        Copy-Item $trace $golden -Force
        "scenario ${n}: golden updated ($((Get-Content $trace).Count) lines)"
        continue
    }
    $diff = @(Compare-Object (Get-Content $golden) (Get-Content $trace))
    if ($diff.Count -eq 0) {
        "scenario ${n}: PASS ($((Get-Content $trace).Count) lines)"
    } else {
        "scenario ${n}: FAIL ($($diff.Count) differing lines, first: $($diff[0].InputObject))"
        $failed++
    }
}
if ($failed) { "$failed scenario(s) FAILED"; exit 1 } else { "all scenarios match golden/" }
