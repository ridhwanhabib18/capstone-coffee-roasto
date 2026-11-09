# Coffee Roasto technical Q&A

Questions and the reasoning behind the technical decisions of Coffee Roasto, for reference and for the capstone defence. Facts come from the code in this repository.

## 1. Why is the PID on the Arduino and not in the GUI?

A control loop needs a steady period and must keep working if the PC is slow, busy or unplugged. The Arduino samples every second and acts on the result at once; the PC only sends set points and shows the result. A Windows timer is not real-time and a serial cable can be pulled out (ADR-002).

## 2. Why one-character commands over serial?

Both sides are small programs written by students. A single character is trivial to parse on an AVR (`String == "k"`), easy to test from the Arduino serial monitor and needs no framing. The cost is that commands are not self-describing, so they are all listed in the [serial protocol](serial-protocol.md) (ADR-001).

## 3. How is the temperature measured and corrected?

Two K-type thermocouples through MAX6675 amplifiers (12-bit, 0.25 °C steps). Each reading is the average of 4 samples taken 1 s apart, then corrected with a linear regression against a reference thermometer (bean = (T − 3.4921) / 0.9194, drum = (T − 6.8969) / 0.8711) and ignored if it moved less than 0.25 °C (ADR-004).

## 4. What do the PID values mean?

Kp = 21.095, Ki = 0.2, Kd = 30 come from a MATLAB simulation of the roaster. The output is clamped to 0–40 because that is the servo range of the gas valve in degrees: 0° is open, 40° is closed. A large error (PID = 40) closes the valve and turns the burner relay on; no error (PID = 0) leaves the valve open and the burner off. In the current firmware the previous-error variable is never updated and Kd is unused, so the loop is closer to P with a small I term (system design, known limitation 1).

## 5. Why is the profile sent ten values at a time every 40 seconds?

The Arduino has fixed arrays (`Array_suhu[15]`) and a serial buffer of 64 bytes, and it consumes one set point per averaged reading (~4 s). Ten values last about 40 s, so the GUI sends the next group just as the last is used. The firmware keeps a second buffer so a new line does not overwrite values still in use. Sending the whole profile at once would overflow both.

## 6. Why does the app read the temperatures with the invariant culture?

The firmware prints a dot (`37.11`). v1 used `Convert.ToDouble(...) / 100`, which only gave 37.11 on a PC with an Indonesian culture (the dot was read as a thousands separator, 3711) and gave 0.37 elsewhere. v2.0 reads the text with `CultureInfo.InvariantCulture`, so every PC gets the same number, and a unit test runs the parser under three cultures.

## 7. Why WPF with WPF-UI on .NET 10?

A real Fluent look (cards, Mica, light and dark theme) comes with WPF-UI; imitating it in Windows Forms means drawing every control by hand. .NET 10 is supported for years, has correct high-DPI behaviour and publishes as one self-contained file. The `Chart` control of v1 does not exist there, so the chart is ScottPlot. The trade-offs are Windows-only and a 74 MB file (ADR-006).

## 8. Why Excel for profiles and export?

Roasters already keep profiles and logs in spreadsheets. ExcelDataReader reads `.xls` and `.xlsx` without Excel installed; the export writes `.xlsx` with ClosedXML, also without Excel. A CSV export could be added later.

## 9. Is the system safe?

It has several layers: relays default off, the 250 °C stop, the `s` command and the over-time end of heating mode. It is not a certified safety system: there is no hardware over-temperature cut-off in the repository and the firmware depends on the PC link for `s`. Treat it as a prototype and keep an operator at the machine.

## 10. What happened to `Release.rar`, `UpgradeLog*.htm` and the `.pfx` of v1?

They were Visual Studio leftovers: project-upgrade reports, an archived build and the temporary ClickOnce signing certificate. None is needed to build and none is in git any more; the local copies are in the ignored `archive/` folder.

## 11. What would be the next improvements?
- Fix the PID (use a real previous error and `KD`) and re-tune on the machine
- Add a newline to every command, or switch the firmware to `Serial.read()` (known limitation 4)
- Remove or implement AUTO MODE
- A cancel button for the profile transfer
- Log roasts (date, beans, weight) next to the temperature export
- Roast phase markers (drying, first crack) on the chart
