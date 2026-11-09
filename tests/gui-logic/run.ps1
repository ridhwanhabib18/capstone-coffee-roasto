# Compiles the GUI classes without a screen together with LogicTests.cs and runs the tests.
# Needs the Visual Studio Roslyn compiler and the NuGet packages of the GUI (run `nuget restore` in gui/ first).
$ErrorActionPreference = "Stop"
$gui = Join-Path $PSScriptRoot "..\..\gui"
$out = Join-Path $PSScriptRoot "out"
New-Item -ItemType Directory -Force $out | Out-Null

$csc = Get-ChildItem "C:\Program Files*\Microsoft Visual Studio\*\*\MSBuild\Current\Bin\Roslyn\csc.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $csc) { throw "Roslyn csc.exe not found: install Visual Studio or its Build Tools" }

$reader = Join-Path $gui "packages\ExcelDataReader.3.6.0\lib\net45\ExcelDataReader.dll"
$readerDataSet = Join-Path $gui "packages\ExcelDataReader.DataSet.3.6.0\lib\net35\ExcelDataReader.DataSet.dll"
if (-not (Test-Path $reader)) { throw "ExcelDataReader not found: run nuget restore in gui/" }

$sources = @(Get-ChildItem (Join-Path $gui "Hardware\*.cs")) + @(Get-ChildItem (Join-Path $gui "Profiles\*.cs")) +
           @(Get-ChildItem (Join-Path $gui "Recording\RecordingSession.cs")) + @(Get-ChildItem (Join-Path $gui "Recording\Coffee.cs")) +
           @(Get-Item (Join-Path $PSScriptRoot "LogicTests.cs"))

$exe = Join-Path $out "LogicTests.exe"
& $csc.FullName -nologo -out:$exe -r:System.dll -r:System.Core.dll -r:System.Data.dll -r:System.Windows.Forms.dll `
    "-r:$reader" "-r:$readerDataSet" ($sources | ForEach-Object { $_.FullName })
if ($LASTEXITCODE -ne 0) { throw "compile failed" }
Copy-Item $reader, $readerDataSet $out -Force

& $exe
exit $LASTEXITCODE
