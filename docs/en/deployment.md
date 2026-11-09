# Build, install and operate

How to build Coffee Roasto v2.0, flash the firmware and run a roast. The design behind it is in the [system design](system-design.md); the wiring is in the [hardware guide](hardware.md).

## What you need
- A Windows PC with a USB port. **Windows 10 version 1607 or newer, 64-bit.** The published `CoffeeRoasto.exe` is self-contained: no .NET and no Excel have to be installed.
- To build the app: the **.NET 10 SDK** (`dotnet --version` shows 10.x). Visual Studio is optional.
- To flash the firmware: the **Arduino IDE** (or `arduino-cli`) and the `MAX6675 library` (the `Servo` library ships with the IDE).
- The roaster wired as in the hardware guide.
- Regional settings do not matter any more: the app reads the temperatures with the invariant culture.

## Steps

### 1. Flash the firmware
1. Open `firmware/Program_PID_CP/Program_PID_CP.ino`. The other `.ino` files and the `.h` files in that folder open as tabs of the same sketch. To only check that it compiles: `arduino-cli compile --fqbn arduino:avr:uno firmware/Program_PID_CP`.
2. Select the board and port, then Upload.
3. Open the serial monitor at 9600 baud. After a few seconds a frame like `@37.11A38.67B0C` should appear every ~4 s. Close the monitor before starting the app (only one program can hold the port).

### 2. Get the app
**Use the published file.** Copy `installer/dist/CoffeeRoasto.exe` to the PC and run it (about 74 MB, no installer).

**Or build it yourself:**
```powershell
./check.ps1                  # restore, build, 32 tests, firmware simulation
./installer/publish.ps1      # writes installer/dist/CoffeeRoasto.exe
dotnet run --project src/CoffeeRoasto.App -c Release    # run it without publishing
```
`check.ps1` restores first and on its own: restoring and building the WPF project in one `dotnet build` can fail on a clean checkout.

### 3. Run a roast
1. Start Coffee Roasto, choose the board in **COM port** (press the refresh button if it is missing) and press **Connect**. The status pill turns green and the control cards switch on.
2. Check the readings against a known temperature and that every lamp is grey.
3. Optional pre-heat: **Heating mode ON**. A toast says "Heating finished" after 10 minutes.
4. Press **PID mode ON**, then **Browse**, choose the profile file and the sheet, and press **Send profile**.
5. Press **Start** in the data card if you want a log; press **Stop** when done, then **Export** and choose where to save the `.xlsx`.
6. When the toast "Roasting finished" appears the cooler runs and the drum keeps turning; stop them with the OFF buttons after the beans are out.
7. **Disconnect** before closing.

The **Dark** button switches the theme; the chart follows it. To abort at any moment press the OFF button of the running mode (sends `s`).

### Profile file format
An `.xls` or `.xlsx` workbook; every sheet that has a header cell exactly `beantemperature` is offered. Each row below the header is one set point in °C, one per ~4 s. Blank cells and a profile shorter than a multiple of ten end the roast (set point 0). Samples: `profiles/`.

## Verification checklist
- [ ] Serial monitor shows frames every ~4 s at 9600 baud
- [ ] The app shows the same values as the monitor
- [ ] Each manual button toggles the right relay and lamp (burner disconnected)
- [ ] OFF of a running mode stops the outputs
- [ ] With a simulated bean value ≥ 250 °C, PID mode ends with the toast "Roasting finished"
- [ ] Export opens an `.xlsx` with all samples (check the last row)
- [ ] The theme switch makes the whole window, chart included, dark and back

## Troubleshooting
| Symptom | Cause | Fix |
| --- | --- | --- |
| Port not in the list | Cable or driver; the list is read at start and by the refresh button | Replug, press refresh, install the CH340/FTDI driver |
| "Access to the port is denied" | Serial monitor or another program holds the port | Close it |
| Control cards stay grey | Not connected | Press Connect |
| Buttons react after about a second | Commands have no newline, the firmware waits for its read timeout | Expected (system design, limitation 4) |
| AUTO MODE does nothing | The firmware has no handler for `m` | Use PID mode |
| Chart empty after Send profile | PID mode was not on | Press PID mode ON first |
| The window is not on the Windows 10 look (no Mica) | Mica needs Windows 11; Windows 10 shows a plain background | Expected, everything else works |
| `dotnet build` fails with missing `.g.cs` files | Restore and build in one command on a clean checkout | `dotnet restore CoffeeRoasto.slnx`, then `dotnet build --no-restore`, or use `check.ps1` |

## Releasing
Raise `<Version>` in both `.csproj` files and `<AssemblyTitle>` of the app, run `./check.ps1`, run `./installer/publish.ps1`, start the new `CoffeeRoasto.exe` and go through the checklist above. `installer/dist/`, `bin/` and `obj/` are not in git. The v1.0 ClickOnce installer is only kept for reference in `installer/v1-clickonce/`.
