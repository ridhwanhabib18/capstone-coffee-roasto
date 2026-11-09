# ADR-006: WPF with WPF-UI on .NET 10, a screenless Core library and a single-file exe

- Status: Accepted
- Date: 2026-11-09
- Supersedes: [ADR-005](005-windows-forms-clickonce.md)

## Context
v1.0 was a Windows Forms app on .NET Framework 4.7.2 with ClickOnce. The screen looked dated, the chart control does not
exist in modern .NET, Excel Interop needs Excel, the display was blurry on scaled screens, and all logic sat behind the
form. The project may now use extra packages.

## Decision
- **.NET 10** (`net10.0`, `net10.0-windows`), a solution of `CoffeeRoasto.Core` (no screen), `CoffeeRoasto.App` (WPF),
  xUnit tests and the screenshot tool.
- **WPF with WPF-UI 4** for a real Fluent look (cards, Mica, light and dark theme), **ScottPlot 5** for the chart,
  **CommunityToolkit.Mvvm** for the view model, **ClosedXML** for `.xlsx` export, **ExcelDataReader** to read profiles,
  **System.IO.Ports** for the serial port.
- **MVVM**: the window binds to `MainViewModel`; the logic (`FrameParser`, `ProfileTransmitter`, `RecordingSession`, ...) lives in `Core`.
- **Distribution**: one self-contained, single-file `CoffeeRoasto.exe` from `installer/publish.ps1`; no installer, no .NET on the PC.

## Reasons
- WPF-UI gives genuine Fluent controls and a theme switch; imitating them in WinForms means hand-drawn controls.
- ScottPlot is MIT, fast, and gives zoom, crosshair and a proper time axis; the old `Chart` is not available on .NET 10.
- With the logic out of the form, the view model and `Core` are tested with xUnit and the window can be driven by a tool
  to make real screenshots.
- High-DPI is correct by default, and .NET 10 is supported for years while .NET Framework 4.7.2 is not moving.
- A single file is the simplest thing to hand to the operator of a machine that is often offline.

## Consequences
- About 1,000 lines of WinForms were replaced; the serial protocol and the firmware did not change.
- The single file is 74 MB (compressed, self-contained).
- ScottPlot pulls in `SkiaSharp.Views.WPF`, restored for .NET Framework (NU1701, suppressed); it was checked with the
  screenshots and the published exe.
- Mica needs Windows 11; Windows 10 shows a plain background.
- Building the WPF project with an implicit restore on a clean checkout can fail; `check.ps1` restores first.
- The ClickOnce files of v1.0 stay in `installer/v1-clickonce/` only for reference.

## Rejected alternatives
- Staying on WinForms with hand-drawn Fluent controls: a lot of code for a worse result, and no ScottPlot for the new chart.
- WinUI 3: more setup (packaging, windowing) for the same look as WPF-UI on a one-window tool.
- UI kits for WinForms (Guna, Siticone, ReaLTaiizor): paid or unclear licences, not Fluent, and tied to one vendor.
- A web UI with a serial bridge: needs an extra program on the PC.
