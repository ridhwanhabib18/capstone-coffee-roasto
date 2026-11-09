# Hardware guide

Nov 9, 2026 · @M Ridhwan Habib A

What the firmware expects to find on the roaster. Pin numbers are the ones in `Program_PID_CP.ino`; check them against your wiring before powering the burner.

## Bill of materials
| Part | Role |
| --- | --- |
| Arduino (AVR, USB serial) | Runs the firmware |
| 2 × MAX6675 + K-type thermocouple | Drum and bean temperature (12-bit, 0.25 °C resolution) |
| 4 × relay channel | Burner (`kompor`), drum motor, blower, cooler |
| Hobby servo | Moves the gas valve between open and closed |
| Gas burner with valve | Heat source |
| PC with a USB port | Runs the GUI |

## Pin map
| Signal | Arduino pin | Notes |
| --- | --- | --- |
| Burner relay (`kompor`) | 9 | Active LOW; the sketch tests it as a 5 Hz-class SSR switch |
| Cooler relay | 10 | Active LOW |
| Servo (gas valve) | 11 | 0° = fully open, 40° = fully closed |
| Drum motor relay (`motorDC`) | 12 | Active LOW |
| Blower relay | 13 | Active LOW |
| Drum thermocouple (MAX6675 #1) | DO 3, CS 4, CLK 5 | `thermocouple1` |
| Bean thermocouple (MAX6675 #2) | DO 6, CS 7, CLK 8 | `thermocouple2` |

All relays are **active LOW**: `HIGH` is off and the pins are set `HIGH` in `setup()`, so a reset leaves everything off and the valve open.

## Temperature chain
1. `read_temperature()` reads both sensors every 1 s (`interval1`).
2. After `banyak_sampling` = 4 readings it averages them.
3. A linear-regression correction is applied: bean = (T₂ − 3.4921) / 0.9194 and drum = (T₁ − 6.8969) / 0.8711. The commented-out lines show the earlier coefficients.
4. A change smaller than 0.25 °C (the MAX6675 resolution) is ignored to reduce flicker.
5. The result is printed as a data frame (see the [serial protocol](serial-protocol.md)).

If you replace a thermocouple or amplifier, redo the regression; the constants belong to the original sensors.

## Safety behaviour
- All outputs off at power-up.
- PID mode stops at a bean temperature ≥ 250 °C: burner off, blower off, drum on, cooler off, valve open, `X` sent.
- `s` from the GUI ends heating or PID at any time.
- After a finished profile the cooler runs and the drum keeps turning until the operator stops them.

## Test sketches
| Sketch | Purpose |
| --- | --- |
| `Pengujian_Hardware` | Toggles the burner relay every 500 ms to check the switching |
| `moveServo` | Draft of a slow servo ramp (100 ms per degree). It does not compile yet: `myServo` is never declared, `moveServo()` is called without its argument and there are extra parentheses |
