# Build, install and operate

How to build the GUI, flash the firmware and run a roast. The design behind it is in the [system design](system-design.md); the wiring is in the [hardware guide](hardware.md).

## What you need
- A Windows PC with a USB port, **.NET Framework 4.7.2** (included in Windows 10/11) and, for the EXPORT button, **Microsoft Excel**.
- To build the GUI: **Visual Studio 2017 or newer** with the ".NET desktop development" workload, and NuGet access (ExcelDataReader 3.6.0, ExcelDataReader.DataSet 3.6.0, Microsoft.Office.Interop.Excel 15.0.4795.1000, System.Runtime.Extensions 4.3.1).
- To flash the firmware: the **Arduino IDE** and the `MAX6675` library (the `Servo` library ships with the IDE).
- The roaster wired as in the hardware guide.
- A Windows regional format that uses a **comma as the decimal separator** (for example Indonesian). See [troubleshooting](#troubleshooting).

## Steps

### 1. Flash the firmware
1. Open `firmware/Program_PID_CP/Program_PID_CP.ino`. The two other `.ino` files in that folder (`read_temperature`, `pisahkansuhuref`) open as tabs of the same sketch.
2. Select the board and port, then Upload.
3. Open the serial monitor at 9600 baud. After a few seconds a frame like `@37.11A38.67B0C` should appear every ~4 s. Close the monitor before starting the GUI (only one program can hold the port).

### 2. Install or build the GUI
Install: run `installer/setup.exe` (ClickOnce). `autorun.inf` starts it when the folder is on a CD or USB stick.

Build:
```bash
cd gui
nuget restore CoffeeRoasto_GUI.sln
msbuild CoffeeRoasto_GUI.sln /p:Configuration=Release
# output: bin/Release/CoffeeRoasto_GUI.exe (+ ExcelDataReader DLLs next to it)
```
Run it from `bin/Release` with its DLLs; the app does not need the `profiles` folder, it only offers sample profiles.

### 3. Run a roast
1. Start Coffee Roasto, open the COM PORT list, pick the board, press **CONNECT**. The bar fills and the controls turn on.
2. Check the readings against a known temperature and that every lamp is dark green.
3. Optional pre-heat: **HEATING MODE ON**. Wait for "Heating Selesai" (10 minutes).
4. Press **PID MODE ON**, then **BROWSE**, pick the profile file and the sheet, and press **START**.
5. Press **RECORD DATA START** if you want a log; press **STOP** when done, then **EXPORT**.
6. When "Roasting Selesai" appears the cooler runs and the drum keeps turning; stop them with the OFF buttons after the beans are out.
7. **DISCONNECT** before closing.

To abort at any moment press the OFF button of the running mode (sends `s`).

### Profile file format
An `.xls` or `.xlsx` workbook; every sheet that has a header cell exactly `beantemperature` is offered. Each row below the header is one set point in °C, one per ~4 s. Blank cells and a profile shorter than a multiple of ten end the roast (set point 0). Samples: `profiles/`.

## Verification checklist
- [ ] Serial monitor shows frames every ~4 s at 9600 baud
- [ ] GUI shows the same values as the monitor (not 0.37 for 37)
- [ ] Each manual button toggles the right relay and lamp (burner disconnected)
- [ ] OFF of a running mode stops the outputs
- [ ] With a simulated bean value ≥ 250 °C, PID mode ends with `X`
- [ ] EXPORT opens Excel with headers and rows

## Troubleshooting
| Symptom | Cause | Fix |
| --- | --- | --- |
| Port not in the list | Cable or driver; list is read when the drop-down opens | Replug, open the list again, install the CH340/FTDI driver |
| "Access to the port is denied" | Serial monitor or another program holds the port | Close it |
| Temperatures 100 times too small (0.37) | PC culture uses a dot as decimal separator | Change the Windows regional format, or parse with the invariant culture and drop `/ 100` |
| Controls stay grey | Not connected | Press CONNECT |
| Buttons react after about a second | Commands have no newline, the firmware waits for its read timeout | Expected (system design, limitation 4) |
| AUTO MODE does nothing | The firmware has no handler for `m` | Use PID MODE |
| Chart empty after START of a profile | PID mode was not on | Press PID MODE ON first |
| EXPORT fails | Excel not installed | Install Excel, or copy the data table by hand |
| "Roasting Selesai" right after pressing heating OFF | Message is shown on the button press | Expected (limitation 7) |

## Releasing
After a change, rebuild in Release, republish through Visual Studio (Build → Publish) to refresh `setup.exe` and `CoffeeRoasto_GUI.application`, and test with the checklist above. Keep `bin/`, `obj/`, `.vs/`, `packages/` and the `.pfx` out of version control.
