# ADR-004: Thermocouple averaging, regression correction and dead band

- Status: Accepted
- Date: 2026-11-09

## Context
MAX6675 readings are 12-bit (0.25 °C steps), noisy and offset from the real temperature.

## Decision
Take four samples 1 s apart, average them, correct each channel with a linear regression measured against a
reference thermometer (bean = (T − 3.4921) / 0.9194, drum = (T − 6.8969) / 0.8711), and keep the previous value if the
change is below 0.25 °C.

## Reasons
- Averaging reduces noise at the cost of a 4 s update period, acceptable for a slow thermal system.
- The regression removes the systematic offset of the installed probes.
- The dead band stops the display and the PID from reacting to one-step flicker.

## Consequences
Replacing a probe or amplifier requires a new regression. The PID sees a reading only every ~4 s, so each profile
row is one 4 s step. The earlier coefficients are kept as comments in `read_temperature.ino`.

## Rejected alternatives
- Raw readings: too noisy for the controller.
- Moving average with a shorter step: faster but needs more RAM and a longer review of the PID timing.
