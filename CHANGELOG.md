# Changelog

## 2.0.0 — 2026-11-09
**Coffee Roasto v2.0: new generation of GUI for Monitoring Temperature Regulation System on Coffee Roaster Machine.**

### New
- New app on **.NET 10** with **WPF and WPF-UI** (Fluent): cards, Mica backdrop on Windows 11, light and dark theme.
- Chart with **ScottPlot 5**: mm:ss time axis, zoom and pan, dashed reference line, theme-aware colours.
- Reading tiles for bean, drum, reference, **bean − reference**, **rate of rise (°C/min)** and elapsed time.
- Toasts instead of modal message boxes ("Roasting finished", "Heating finished", errors).
- Export to **`.xlsx` with ClosedXML**: Excel does not have to be installed.
- One self-contained single-file `CoffeeRoasto.exe` (`installer/publish.ps1`), no installer and no .NET on the PC.
- Tests: 32 xUnit tests (`Core` and the view model), a firmware simulation with golden traces of the original firmware
  (12 scenarios), a screenshot tool that drives the real window, and `check.ps1` to run everything.
- Documentation in English and Indonesian: requirements, system design, ADRs (new ADR-006), protocol, hardware, testing.

### Changed
- Firmware split into tabs (`Config.h`, `Commands.h`, `Actuators`, `TemperatureSensors`, `Pid`, modes). Behaviour is
  identical to the original in all simulated scenarios; one rare early end of heating mode was removed.
- The PID lamp is switched off together with the other lamps when the roast finishes.
- The heating OFF button no longer shows "Roasting Selesai" before it sends the stop command.
- Repository: `src/`, `tests/`, `tools/`, `firmware/`, `profiles/`, `installer/`; build output and the signing key are no longer tracked.

### Fixed
- Temperatures no longer depend on the regional settings of the PC (the dot of the firmware is read with the invariant culture).
- Export no longer skips the last row and no longer needs Excel.
- Serial lines are handled on the UI thread; every command checks that the port is open.

### Known limitations
See `docs/en/system-design.md`, section 10 (PID gains, the `m` command, profile end, command framing, `moveServo.ino`).

### Not changed
The serial protocol and the behaviour of the firmware.

## 1.0.0
The original Windows Forms app on .NET Framework 4.7.2 (ClickOnce installer in `installer/v1-clickonce/`) and the
single-file Arduino sketch. Its source is in the git history before the commit that introduced v2.0.
