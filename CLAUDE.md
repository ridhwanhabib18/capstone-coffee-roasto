# CLAUDE.md — Coffee Roasto v2.0

Coffee Roasto is a capstone project: a WPF app (C#, .NET 10, Fluent look with WPF-UI) that monitors and controls a drum
coffee roaster, and the Arduino firmware that runs the roaster's heater, motors and temperature loop.
Tagline: "New generation of GUI for Monitoring Temperature Regulation System on Coffee Roaster Machine".

## Source of truth
- Requirements (as built): `docs/en/PRD.md` (Indonesian copy: `docs/id/PRD.md`)
- System design: `docs/en/system-design.md`, diagrams in `docs/en/diagrams/`
- Serial protocol: `docs/en/serial-protocol.md` — the contract between the app and the firmware
- Hardware pins and wiring: `docs/en/hardware.md`
- Decisions: `docs/en/adr/` (Indonesian copies in `docs/id/adr/`)
If code and docs disagree, stop and ask. Never invent requirements that are not in the PRD.

## How we work
- The app and the firmware share one protocol. Change both sides and `docs/en/serial-protocol.md` in the same change.
- This controls a gas burner. Never change a safety rule (250 °C stop, burner off on `s`, relay idle = HIGH) without
  asking, and say in the summary how the change was tested (burner disconnected first).
- GitHub: this repo always uses the account `ridhwanhabib18` (`ridhwanhabib18/capstone-coffee-roasto`). Before any push or
  `gh` call run `gh auth status`; if another account is active, `gh auth switch --user ridhwanhabib18`. Never use `habibepson`.
- Packages: adding NuGet packages or Arduino libraries is allowed in this repo (the usual approval rule is relaxed here).
  Still check the license (MIT, BSD, Apache or similar), prefer well-maintained packages, and list new ones in
  `docs/en/system-design.md` section 9.
- Do not commit `bin/`, `obj/`, `installer/dist/`, `archive/`, `*.pfx` or Excel lock files (`~$*.xlsx`).

## Stack
App: C#, .NET 10 (`net10.0-windows`), WPF, WPF-UI 4 (Fluent, Mica, light/dark), ScottPlot.WPF 5 (chart),
CommunityToolkit.Mvvm (view model), System.IO.Ports, ExcelDataReader (read profiles), ClosedXML (export).
Tests: xUnit. Firmware: Arduino (AVR), `max6675` and `Servo` libraries, 9600 baud.

## Architecture rules
- `src/CoffeeRoasto.Core` has no UI and no WPF reference: serial link, `FrameParser`, profiles, recording, Excel export.
  Everything there is `public`, pure or behind a small class, and covered by xUnit tests.
- `src/CoffeeRoasto.App` is MVVM: `MainWindow.xaml` binds to `MainViewModel`; the view model never references a control.
  The window only does what needs a control: ScottPlot drawing, file dialogs, toasts, the theme switch.
- Every button maps to one protocol character in `RoasterCommand` (no string literals in the view model) and goes through
  `RoasterLink.Send`. Lamps change only when the command was sent.
- The serial thread only reads lines; the view model moves them to the UI thread (`Post`) before touching any property.
- Incoming frames are `@<bean>A<drum>B<reference>C\n`, values with a dot, read with the invariant culture; the status
  letters `X`, `V`, `Y`, `Z` are single-line events.
- The PID loop runs on the Arduino, not in the app. The app only supplies set points and shows state (ADR-002).
- Relays are active LOW: `HIGH` = off, `LOW` = on. The valve servo is 0° open, 40° closed.
- Reference profiles are Excel files with a header cell named `beantemperature`.
- Look and feel: use WPF-UI controls and `DynamicResource` brushes (`TextFillColorPrimaryBrush`, ...) so both themes work;
  the colours of the three chart lines are in `App.xaml`, the plot colours follow the theme in `MainWindow.xaml.cs`.
  Do not set a solid `Background` on the window: it would hide the Mica backdrop.
- Firmware (`firmware/Program_PID_CP/`): fixed values in `Config.h`, protocol characters in `Commands.h`, shared objects in
  `Globals.h`; one tab per job (`Actuators`, `TemperatureSensors`, `Pid`, `ManualMode`, `PidMode`, `HeatingMode`,
  `ReferenceProfile.h`). Every output goes through the functions in `Actuators.ino`, never a bare `digitalWrite`.
- The refactored firmware was checked against the original with a host-side trace comparison (12 scenarios, no
  differences). After changing firmware logic, compile with `arduino-cli` and re-test on the machine with the burner disconnected.

## Language
- Code identifiers and new comments in English. The firmware uses English names too (`PIN_BURNER`, `setBurner`,
  `roaster.setpoint`); the glossary in the PRD maps the old Indonesian ones (`kompor`, `suhu_ref_saatini`).
- Docs exist in English (primary) and Indonesian; update both together.
- The two toasts and all labels of the app are English.

## Commands
- Everything that needs no roaster: `./check.ps1` (restore, build, 32 xUnit tests, firmware simulation); `-Quick` skips the firmware part.
  Restore first and on its own: `dotnet build` with an implicit restore can fail on a clean checkout of the WPF project.
- Run the app: `dotnet run --project src/CoffeeRoasto.App -c Release`
- Publish the single-file exe: `./installer/publish.ps1` (writes `installer/dist/CoffeeRoasto.exe`)
- Flash firmware: Arduino IDE, open `firmware/Program_PID_CP/Program_PID_CP.ino`, Upload
- Compile-check firmware: `arduino-cli compile --fqbn arduino:avr:uno firmware/Program_PID_CP` (needs `MAX6675 library` and `Servo`)
- Firmware simulation only: `tests/firmware-sim/run.ps1`; after a deliberate behaviour change review and update `golden/` (see `docs/en/testing.md`)
- Screenshots and GIF: `tools/screenshots/run.ps1` (rerun it after a visible UI change)
- Real-machine checks still follow the checklist in `docs/en/deployment.md`.
