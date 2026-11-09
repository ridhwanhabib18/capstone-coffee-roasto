# Opens the real window, replays a recorded roast through it and saves screenshots (and a GIF) to docs/assets/.
# Needs the .NET 10 SDK and, for the GIF, Python with Pillow.
$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$shots = Join-Path $root "docs\assets\screenshots"
$trace = Join-Path $root "tests\firmware-sim\golden\scenario12.txt"
$profile = Join-Path $root "profiles\Testing Baru.xls.xlsx"

Get-ChildItem $shots -Filter *.png -ErrorAction SilentlyContinue | Remove-Item
dotnet run --project (Join-Path $PSScriptRoot "Screenshots.csproj") -c Release -- $trace $profile $shots
if ($LASTEXITCODE -ne 0) { throw "screenshot tool failed ($LASTEXITCODE)" }

# The animated version of the same roast.
$gif = Join-Path $root "docs\assets\flows\pid-roast.gif"
New-Item -ItemType Directory -Force (Split-Path $gif) | Out-Null
$py = @"
import glob
from PIL import Image
files = sorted(glob.glob(r'$shots\_frames\frame*.png'))
frames = [Image.open(f).convert('RGB').resize((1100, int(1100 * Image.open(f).height / Image.open(f).width))).convert('P', palette=Image.ADAPTIVE) for f in files]
frames[0].save(r'$gif', save_all=True, append_images=frames[1:], duration=350, loop=0, optimize=True)
print('gif:', len(frames), 'frames')
"@
$py | python -
