# CLAUDE.md — Coffee Roasto

Coffee Roasto is a capstone project: a Windows Forms app (C#, .NET Framework 4.7.2) that monitors and controls a
drum coffee roaster, and the Arduino firmware that runs the roaster's heater, motors and temperature loop.

## Source of truth
- Requirements (as built): `docs/en/PRD.md` (Indonesian copy: `docs/id/PRD.md`)
- System design: `docs/en/system-design.md`, diagrams in `docs/en/diagrams/`
- Serial protocol: `docs/en/serial-protocol.md` — the contract between GUI and firmware
- Hardware pins and wiring: `docs/en/hardware.md`
- Decisions: `docs/en/adr/` (Indonesian copies in `docs/id/adr/`)
If code and docs disagree, stop and ask. Never invent requirements that are not in the PRD.

## How we work
- The GUI and the firmware share one protocol. Change both sides and `docs/en/serial-protocol.md` in the same change.
- This controls a gas burner. Never change a safety rule (250 °C stop, burner off on `s`, relay idle = HIGH) without
  asking, and say in the summary how the change was tested (burner disconnected first).
- Do not add NuGet packages or Arduino libraries without approval; check the license.
- GitHub: this repo always uses the account `ridhwanhabib18` (`ridhwanhabib18/capstone-coffee-roasto`). Before any push or `gh` call run `gh auth status`; if another account is active, `gh auth switch --user ridhwanhabib18`. Never use `habibepson`.
- Do not commit `bin/`, `obj/`, `.vs/`, `packages/`, `*.pfx` or the Excel lock files (`~$*.xlsx`).

## Stack
GUI: C#, .NET Framework 4.7.2, Windows Forms, `System.Windows.Forms.DataVisualization` chart,
`System.IO.Ports.SerialPort`, ExcelDataReader 3.6.0 (read profiles), Microsoft.Office.Interop.Excel (export).
Firmware: Arduino (AVR), `max6675` and `Servo` libraries, 9600 baud.

## Architecture rules
- Firmware (`firmware/Program_PID_CP/`): fixed values in `Config.h`, protocol characters in `Commands.h`, shared objects in
  `Globals.h`; one tab per job (`Actuators`, `TemperatureSensors`, `Pid`, `ManualMode`, `PidMode`, `HeatingMode`,
  `ReferenceProfile.h`). Every output goes through the functions in `Actuators.ino`, never a bare `digitalWrite`.
- The refactored firmware was checked against the original with a host-side trace comparison (11 scenarios, no
  differences). After changing firmware logic, compile with `arduino-cli` and re-test on the machine with the burner disconnected.
- `Form_CoffeeRoasto` is the only screen, split into `Form1.cs` (setup, incoming data) and `Form1.<Area>.cs` partials
  (Connection, Manual, Modes, Recording, Profile). Handlers stay thin: send a command, set lamps.
- Folders: `Hardware/` (serial link, commands, frame parser), `Profiles/` (Excel profile in, transmitter), `Recording/`
  (samples, Excel export), `UI/` (lamps, chart, clock). `Hardware`, `Profiles` and `Recording` never reference the form.
- All serial I/O goes through `RoasterLink`; the serial thread only reads a line and hands it to the UI thread with
  `BeginInvoke`. Command characters live in `RoasterCommand`, never as string literals in handlers.
- New `.cs` files must be added to `CoffeeRoasto_GUI.csproj` (old-style project, no globbing).
- Every button maps to one protocol character (see the protocol document); no hidden commands.
- The PID loop runs on the Arduino, not in the GUI. The GUI only supplies set points and shows state (ADR-002).
- Incoming frames are `@<bean>A<drum>B<reference>C\n`; status letters `X`, `V`, `Y`, `Z` are single-line events.
- Relays are active LOW: `HIGH` = off, `LOW` = on. The valve servo is 0° open, 40° closed.
- Reference profiles are Excel files with a header cell named `beantemperature`.

## Language
- Code identifiers and new comments in English; existing Indonesian comments may stay.
- The firmware uses English names too (`PIN_BURNER`, `setBurner`, `roaster.setpoint`); the glossary in the PRD maps the old Indonesian ones (`kompor`, `suhu_ref_saatini`).
- Docs exist in English (primary) and Indonesian; update both together.

## Commands
- Build GUI: `nuget restore` then `msbuild "gui/CoffeeRoasto_GUI.sln" /p:Configuration=Release`
- Run GUI: open the solution in Visual Studio and press F5, or run `bin/Release/CoffeeRoasto_GUI.exe`
- Flash firmware: Arduino IDE, open `firmware/Program_PID_CP/Program_PID_CP.ino`, Upload
- Compile-check firmware: `arduino-cli compile --fqbn arduino:avr:uno firmware/Program_PID_CP` (needs the `MAX6675 library` and `Servo` libraries)
- Tests (no roaster needed): `tests/gui-logic/run.ps1` and `tests/firmware-sim/run.ps1`; both must pass before a push. After a deliberate
  firmware behaviour change, review and update `tests/firmware-sim/golden/` (see `docs/en/testing.md`).
- Screenshots and GIF: `tools/screenshots/run.ps1` (rerun it after a visible GUI change).
- Real-machine checks still follow the checklist in `docs/en/deployment.md`.
