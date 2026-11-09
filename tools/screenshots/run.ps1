# Builds the GUI, replays a recorded roast through it and saves screenshots (and a GIF) to docs/assets/.
# Needs Visual Studio (MSBuild and Roslyn) and, for the GIF, Python with Pillow.
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$out = Join-Path $PSScriptRoot "out"
New-Item -ItemType Directory -Force $out | Out-Null

$vs = Get-ChildItem "C:\Program Files*\Microsoft Visual Studio\*\*\MSBuild\Current\Bin\MSBuild.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $vs) { throw "MSBuild not found" }
$csc = Join-Path (Split-Path $vs.FullName) "Roslyn\csc.exe"

# 1. The GUI, built exactly as for release (signing is skipped: it needs a certificate that is not in git).
$guiOut = Join-Path $out "gui\"
& $vs.FullName (Join-Path $root "gui\CoffeeRoasto_GUI.sln") -p:Configuration=Release -p:SignManifests=false "-p:OutDir=$guiOut" -nologo -v:q
if ($LASTEXITCODE -ne 0) { throw "GUI build failed" }

# 2. The screenshot tool, compiled against the built GUI.
$exe = Join-Path $guiOut "Screenshots.exe"
& $csc -nologo -target:exe -out:$exe "-r:$guiOut\CoffeeRoasto_GUI.exe" -r:System.dll -r:System.Core.dll -r:System.Data.dll `
    -r:System.Drawing.dll -r:System.Windows.Forms.dll (Join-Path $PSScriptRoot "Screenshots.cs")
if ($LASTEXITCODE -ne 0) { throw "tool build failed" }

# 3. Replay the recorded roast.
$shots = Join-Path $root "docs\assets\screenshots"
$trace = Join-Path $root "tests\firmware-sim\golden\scenario12.txt"
$profile = Join-Path $root "profiles\Testing Baru.xls.xlsx"
& $exe $trace $profile $shots
if ($LASTEXITCODE -ne 0) { throw "screenshot tool failed ($LASTEXITCODE)" }

# 4. The animated version of the same roast.
$gif = Join-Path $root "docs\assets\flows\pid-roast.gif"
New-Item -ItemType Directory -Force (Split-Path $gif) | Out-Null
$py = @"
import glob, sys
from PIL import Image
files = sorted(glob.glob(r'$shots\_frames\frame*.png'))
frames = [Image.open(f).convert('P', palette=Image.ADAPTIVE) for f in files]
frames[0].save(r'$gif', save_all=True, append_images=frames[1:], duration=350, loop=0, optimize=True)
print('gif:', len(frames), 'frames')
"@
$py | python -
