# Serial protocol

Nov 9, 2026 · @M Ridhwan Habib A

The contract between the Coffee Roasto GUI (`Form1.cs`) and the Arduino firmware (`Program_PID_CP.ino`). It is derived from the code of both sides; change both and this page together (ADR-001).

**Link:** USB serial, 9600 baud, 8N1 (the .NET and Arduino defaults), ASCII text. The firmware reads a line with `Serial.readStringUntil('\n')`.

## 1. GUI → Arduino (commands)

| Char | Sent by (button) | Firmware action | Lamp in GUI |
| --- | --- | --- | --- |
| `#` | CONNECT | Not a command; read as set point `0`. Clears the reference | — |
| `n` | Burner ON | Burner relay on, set point 0 | Burner lime |
| `k` | Burner OFF | Burner relay off, set point 0 | Burner dark green |
| `t` | Motor drum ON | Drum motor on | Motor drum lime |
| `d` | Motor drum OFF | Drum motor off | Motor drum dark green |
| `y` | Blower drum ON | Blower on | Blower lime |
| `b` | Blower drum OFF | Blower off | Blower dark green |
| `p` | Cooler ON | Cooler on | Cooler lime |
| `c` | Cooler OFF | Cooler off | Cooler dark green |
| `v` | Motor valve OPEN | Servo to 0° (valve fully open) | Valve lime |
| `x` | Motor valve CLOSE | Servo to 40° (valve fully closed) | Valve dark green |
| `h` | Heating mode ON | Heating loop: set point 150 °C for 600 s (section 4) | Heating, drum, blower, valve lime |
| `q` | PID mode ON | PID loop with set point 150 °C until a profile arrives (section 4) | PID, drum, blower, valve lime |
| `s` | OFF of heating, PID or auto mode | Stops the running loop, burner off | Mode lamp off |
| `m` | Auto mode ON | **No handler in this firmware**; read as set point `0` | Auto lime |
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

One frame is sent per averaged reading: 4 samples at 1 s each, so **one frame every ~4 s**. The GUI finds the markers `@`, `A`, `B`, `C` with `IndexOf`, converts each field with `Convert.ToDouble(...) / 100` (see the decimal-separator note below) and plots it. A frame with a missing marker is ignored.

> **Decimal separator.** The firmware prints values with a dot (`37.11`). The GUI divides by 100 because it runs on an Indonesian Windows where `Convert.ToDouble("37.11")` reads the dot as a thousands separator (= 3711 → 37.11). On a PC with an English culture the same code gives 0.37. Keep the PC on an Indonesian (comma decimal) culture, or fix the parser (section 10 of the system design).

### Event lines

| Line | Meaning | GUI reaction |
| --- | --- | --- |
| `Y` | Burner switched on by the PID | Burner lamp lime |
| `Z` | Burner switched off | Burner lamp dark green |
| `X` | Roasting finished (profile ended, `s`, or bean ≥ 250 °C) | Message "Roasting Selesai"; auto, blower and burner lamps off; drum and cooler lamps lime |
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
