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
- Do not commit `bin/`, `obj/`, `.vs/`, `packages/`, `*.pfx` or the Excel lock files (`~$*.xlsx`).

## Stack
GUI: C#, .NET Framework 4.7.2, Windows Forms, `System.Windows.Forms.DataVisualization` chart,
`System.IO.Ports.SerialPort`, ExcelDataReader 3.6.0 (read profiles), Microsoft.Office.Interop.Excel (export).
Firmware: Arduino (AVR), `max6675` and `Servo` libraries, 9600 baud.

## Architecture rules
- `Form1.cs` (`Form_CoffeeRoasto`) is the only screen. Hardware I/O goes through `serialPort1`; UI updates from the
  serial thread must use `BeginInvoke`.
- Every button maps to one protocol character (see the protocol document); no hidden commands.
- The PID loop runs on the Arduino, not in the GUI. The GUI only supplies set points and shows state (ADR-002).
- Incoming frames are `@<bean>A<drum>B<reference>C\n`; status letters `X`, `V`, `Y`, `Z` are single-line events.
- Relays are active LOW: `HIGH` = off, `LOW` = on. The valve servo is 0° open, 40° closed.
- Reference profiles are Excel files with a header cell named `beantemperature`.

## Language
- Code identifiers and new comments in English; existing Indonesian comments may stay.
- Keep the existing names (`kompor` = burner, `suhu` = temperature) in the firmware; the docs give the translation.
- Docs exist in English (primary) and Indonesian; update both together.

## Commands
- Build GUI: `nuget restore` then `msbuild "gui/CoffeeRoasto_GUI.sln" /p:Configuration=Release`
- Run GUI: open the solution in Visual Studio and press F5, or run `bin/Release/CoffeeRoasto_GUI.exe`
- Flash firmware: Arduino IDE, open `firmware/Program_PID_CP/Program_PID_CP.ino`, Upload
- There is no automated test suite; verify with the checklist in `docs/en/deployment.md`.
