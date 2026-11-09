# Coffee Roasto Requirements (as built)

Nov 9, 2026 · @M Ridhwan Habib A

This is the English version and the primary reference; the Indonesian version is updated alongside it. The project existed before this page, so the requirements below were written back from the code of the GUI and the firmware. Each one has an acceptance check that can be answered yes or no. Anything the code does not show is under Open questions, not invented.

## 1. Summary & problem

**Product name:** Coffee Roasto

**One-line summary:** Coffee Roasto monitors and controls a drum coffee roaster from a Windows PC, and roasts automatically by following a reference temperature profile with a PID controller on an Arduino.

**Problem:** Roasting by hand depends on the operator's eye and timing, the temperature is not recorded, and two roasts of the same beans rarely match. The roaster needs temperature measurement, repeatable control and a record of every run.

## 2. Goals, non-goals & success metrics

**Goals**
- Show bean, drum and reference temperature live
- Switch every actuator by hand with a visible state
- Pre-heat the drum automatically
- Follow a reference profile from Excel with PID control
- Record a run and export it to Excel

**Non-goals (out of scope for v1)**
- Profiles with more than the bean temperature column (no rate-of-rise, no first-crack markers)
- Cloud storage, remote access or multi-roaster support
- A database; history is kept as Excel files
- Mobile or web clients
- Tuning the PID from the GUI

**Success metrics**

| Metric | Target | How to measure |
| --- | --- | --- |
| Reading interval | One frame every ~4 s | Watch the chart timestamps |
| Over-temperature stop | Burner off when bean ≥ 250 °C | Test with the burner disconnected and a simulated reading |
| Profile tracking | Bean temperature follows the profile | Compare Bean and Reference series after a run |
| Time to first reading | < 1 min from launch (select port, CONNECT) | Stopwatch |

## 3. Users & roles

| Role | Description | Can do |
| --- | --- | --- |
| Operator | Person running the roaster | Everything in the GUI; there is no login |

## 4. Functional requirements

Each item names the control it belongs to.

### 4.1 Connection
- **FR-1** The operator picks a COM port from the drop-down (listed when opened) and presses CONNECT. *Accept:* the progress bar shows 100 %, CONNECT is disabled, DISCONNECT and all controls are enabled.
- **FR-2** Before a connection all controls except CONNECT are disabled. *Accept:* on launch the buttons are grey.
- **FR-3** DISCONNECT closes the port, clears the chart and data table and resets the labels to `....ºC`. A closing window also closes the port.

### 4.2 Live monitoring
- **FR-4** Bean, drum and reference temperature are shown as labels and as three chart series, with time (mm:ss) since connect or since a mode started on the X axis. *Accept:* a new point appears with each frame.
- **FR-5** The reference label shows `....ºC` while no set point is active.

### 4.3 Manual control
- **FR-6** Burner, motor drum, blower drum, cooler: ON and OFF buttons. Motor valve: OPEN and CLOSE. *Accept:* each press sends its protocol character and changes the lamp (lime = on, dark green = off).
- **FR-7** The operator can type a temperature and press SEND to give the PID a set point.

### 4.4 Heating mode
- **FR-8** HEATING ON starts pre-heating at 150 °C for 600 s with drum motor, blower and valve open; the cooler lamp is off. *Accept:* the firmware sends `V` at the end and the GUI shows "Heating Selesai".

### 4.5 Automatic roasting
- **FR-9** PID MODE ON starts the PID loop with a 150 °C set point, drum motor and blower on and the cooler off.
- **FR-10** BROWSE opens a `.xls` or `.xlsx` file; SHEET lists its sheets; the data table shows the column `beantemperature` of the chosen sheet.
- **FR-11** START sends the column to the firmware in groups of ten values, waiting 40 s between groups. *Accept:* the reference series follows the sheet.
- **FR-12** When the profile ends the firmware sends `X`: burner off, blower off, drum and cooler on. The GUI shows "Roasting Selesai".
- **FR-13** OFF (or a bean temperature ≥ 250 °C) stops the roast immediately.

### 4.6 Recording
- **FR-14** START clears the chart, table and the list, and starts collecting bean and drum values; STOP shows them in the data table.
- **FR-15** EXPORT opens Excel with the table headers and rows on a sheet named "Exported from gridview".

### 4.7 Status
- **FR-16** Pilot lamps show the state of burner, motor drum, blower drum, cooler, valve, recording, heating mode, auto mode and PID mode. The burner lamp also follows the firmware events `Y` and `Z`.

## 5. Non-functional requirements
| Area | Requirement |
| --- | --- |
| Safety | All outputs off at power-up; stop at 250 °C; relays active LOW |
| Platform | Windows, .NET Framework 4.7.2; Arduino AVR board at 9600 baud |
| Locale | PC culture with comma decimal separator (see limitations in the system design) |
| Dependencies | Excel installed for EXPORT |
| Offline | Works without a network |
| Language | UI in English with two Indonesian message boxes ("Roasting Selesai", "Heating Selesai") |

## 6. Glossary

| Indonesian (original code) | English (current code) |
| --- | --- |
| kompor | burner (`PIN_BURNER`, `setBurner`) |
| motorDC | drum motor (`PIN_DRUM_MOTOR`) |
| suhu | temperature |
| suhu referensi | reference temperature / set point |
| pemanasan | heating |
| biji | bean |
| waktu_ref | heating time, 600 s (`HEATING_DURATION_S`) |
| Pengujian Hardware | hardware test |
| Data Suhu Referensi | reference temperature data |

## 7. Implementation status
| Area | Status |
| --- | --- |
| Connection, live chart, labels | Done |
| Manual control and pilot lamps | Done |
| Heating mode | Done |
| PID mode and profile from Excel | Done (see known limitations 1 and 8) |
| Auto mode `m` | GUI only; no firmware handler |
| Recording and export | Done (needs Excel) |
| Servo ramp (`moveServo.ino`) | Draft, does not compile |

## 8. Open questions
1. Should AUTO MODE (`m`) be removed from the GUI, or implemented in the firmware?
2. Are Kd = 30 and the previous-error term meant to be active (known limitation 1)?
3. Should the temperature parser use `CultureInfo.InvariantCulture` instead of relying on the PC locale?
4. Is a roast log (date, bean type, weight, result) needed next to the temperature export?
