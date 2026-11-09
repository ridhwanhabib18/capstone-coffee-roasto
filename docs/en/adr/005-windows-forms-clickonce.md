# ADR-005: Windows Forms on .NET Framework with ClickOnce

- Status: Accepted
- Date: 2026-11-09

## Context
The roaster is operated from a Windows PC that may not be online. The app needs a serial port, a live chart and a
simple installer.

## Decision
Build a Windows Forms app on .NET Framework 4.7.2, use the built-in `SerialPort` and `Chart` controls, and
distribute it with ClickOnce (`setup.exe`, `CoffeeRoasto_GUI.application`, `autorun.inf`).

## Reasons
- Everything needed is in the framework; no runtime to install on Windows 10/11.
- The designer lets the team change the layout quickly.
- ClickOnce installs from a folder, a USB stick or a CD without admin tooling.

## Consequences
Windows only, and no automated tests (logic lives in the form's code-behind). Moving logic into classes, as done for
`Coffee`, would allow unit tests. A move to .NET 8 Windows Forms is possible later.

## Rejected alternatives
- WPF: heavier for a one-screen tool.
- Web UI with a serial bridge: needs an extra program on the PC.
