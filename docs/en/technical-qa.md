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

## 6. Why does the GUI divide the readings by 100?

It does not rescale anything intentionally: on a PC with an Indonesian culture `Convert.ToDouble("37.11")` reads the dot as a thousands separator and returns 3711, and the division restores 37.11. On an English-culture PC the same code returns 0.37. The robust fix is `double.Parse(s, CultureInfo.InvariantCulture)` without the division.

## 7. Why Windows Forms on .NET Framework 4.7.2?

The roaster PC is Windows, the chart control (`System.Windows.Forms.DataVisualization`), `SerialPort` and ClickOnce are all built in, and no runtime has to be installed on Windows 10 or 11. The trade-off is that the app is Windows-only and Excel Interop needs Excel (ADR-005).

## 8. Why Excel for profiles and export?

Roasters already keep profiles and logs in spreadsheets. ExcelDataReader reads `.xls` and `.xlsx` without Excel installed; the export uses Excel Interop and does need it. A CSV export would remove that dependency.

## 9. Is the system safe?

It has several layers: relays default off, the 250 °C stop, the `s` command and the over-time end of heating mode. It is not a certified safety system: there is no hardware over-temperature cut-off in the repository and the firmware depends on the PC link for `s`. Treat it as a prototype and keep an operator at the machine.

## 10. What does "Release.rar", "UpgradeLog*.htm" and the `.pfx` in the repository mean?

`UpgradeLog*.htm` are Visual Studio project-upgrade reports, `Release.rar` is an archived build, and `CoffeeRoasto_GUI_TemporaryKey.pfx` is the temporary certificate Visual Studio creates for ClickOnce signing. None is needed to build; the `.gitignore` of the project already excludes build output.

## 11. What would be the next improvements?
- Fix the PID (update `errorx`, use Kd) and re-tune on the machine
- Parse with the invariant culture and remove the division
- Add a newline to every command, or switch the firmware to `Serial.read()`
- Marshal all lamp and message updates to the UI thread
- Remove or implement AUTO MODE
- Log roasts (date, beans, weight) and export CSV
