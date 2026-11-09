# Serial protocol

Nov 9, 2026 · @M Ridhwan Habib A

The contract between Coffee Roasto v2.0 (`RoasterCommand`, `RoasterLink` and `FrameParser` in `CoffeeRoasto.Core`) and the Arduino firmware (`Program_PID_CP.ino`). It is derived from the code of both sides; change both and this page together (ADR-001).

**Link:** USB serial, 9600 baud, 8N1 (the .NET and Arduino defaults), ASCII text. The firmware reads a line with `Serial.readStringUntil('\n')`.

## 1. GUI → Arduino (commands)

| Char | Sent by (button) | Firmware action | Lamp in GUI |
| --- | --- | --- | --- |
| `#` | CONNECT | Not a command; read as set point `0`. Clears the reference | — |
| `n` | Burner ON | Burner relay on, set point 0 | Burner bright green |
| `k` | Burner OFF | Burner relay off, set point 0 | Burner dim green |
| `t` | Motor drum ON | Drum motor on | Motor drum bright green |
| `d` | Motor drum OFF | Drum motor off | Motor drum dim green |
| `y` | Blower drum ON | Blower on | Blower bright green |
| `b` | Blower drum OFF | Blower off | Blower dim green |
| `p` | Cooler ON | Cooler on | Cooler bright green |
| `c` | Cooler OFF | Cooler off | Cooler dim green |
| `v` | Motor valve OPEN | Servo to 0° (valve fully open) | Valve bright green |
| `x` | Motor valve CLOSE | Servo to 40° (valve fully closed) | Valve dim green |
| `h` | Heating mode ON | Heating loop: set point 150 °C for 600 s (section 4) | Heating, drum, blower, valve bright green |
| `q` | PID mode ON | PID loop with set point 150 °C until a profile arrives (section 4) | PID, drum, blower, valve bright green |
| `s` | OFF of heating, PID or auto mode | Stops the running loop, burner off | Mode lamp off |
| `m` | Auto mode ON | **No handler in this firmware**; read as set point `0` | Auto bright green |
| `<number>` | INPUT TEMPERATURE + SEND | Any other text is parsed with `toFloat()` as the reference temperature (°C) | — |
| `t1,t2,…,t10` | START of the file-based profile | Ten comma-separated set points, sent with `WriteLine` | — |

`s` is only read inside the heating and PID loops; in manual mode there is nothing to stop.

## 2. Arduino → GUI (telemetry and events)

### Data frame

```text
@<bean>A<drum>B<reference>C\n
@37.11A38.67B150.00C
```

| Field | Meaning | Source |
| --- | --- | --- |
| `<bean>` | Bean temperature, calibrated, °C | `beanSensor` (CS 7) |
| `<drum>` | Drum temperature, calibrated, °C | `drumSensor` (CS 4) |
| `<reference>` | Current set point, °C; `0` when none | `roaster.setpoint` |

One frame is sent per averaged reading: 4 samples at 1 s each, so **one frame every ~4 s**. `FrameParser` finds the markers `@`, `A`, `B`, `C`, reads each field as a number and passes the reading on. A frame with a missing marker or a damaged number is ignored.

> **Decimal separator.** The firmware prints values with a dot (`37.11`). v2.0 reads them with the invariant culture, so the result no longer depends on the regional settings of the PC. (The v1 app divided by 100 and only worked on a PC with an Indonesian culture.)

### Event lines

| Line | Meaning | GUI reaction |
| --- | --- | --- |
| `Y` | Burner switched on by the PID | Burner lamp bright green |
| `Z` | Burner switched off | Burner lamp dim green |
| `X` | Roasting finished (profile ended, `s`, or bean ≥ 250 °C) | Message "Roasting Selesai"; auto, blower and burner lamps off; drum and cooler lamps bright green |
| `V` | Heating finished (600 s) | Message "Heating Selesai"; heating, blower, burner lamps off |

## 3. Profile transfer

The GUI reads the sheet column `beantemperature`, walks the rows and builds groups of ten values separated by commas. After each group of ten it waits **40 s** (10 values × ~4 s per set point) so the firmware does not run out of set points; a shorter last group is sent at the end. The firmware (`ReferenceProfile.h`) splits the line on commas into a pending group of ten, makes it the active group when the previous one is used up, and advances one set point per averaged reading. If no new line has arrived, the same group is used again, so a profile whose length is a multiple of ten never ends by itself (known limitation 12). An empty value converts to `0`, and a set point of `0` ends the roast (`X`).

Start PID mode (`q`) **before** pressing START: the profile is only read inside the PID loop.

## 4. Firmware modes

| Mode | Valve | Burner rule | Ends |
| --- | --- | --- | --- |
| Manual | by command | by command | — |
| Heating (`h`) | open (0°) | on when PID ≥ 40 and within 600 s, off when PID ≤ 0 | after 600 s (`V`) or `s` |
| PID (`q`) | `40 − PID` degrees | on when PID = 40, off when PID = 0 | set point 0 (`X`), `s`, or bean ≥ 250 °C (`X`) |

`PID = Kp·e + Ki·(e + e_prev) + (e − e_prev)`, clamped to 0–40, with `e = set point − bean temperature`, Kp = 21.095, Ki = 0.2 (Kd = 30 is declared but not used, and `e_prev` is never updated; see the system design, known limitations).
