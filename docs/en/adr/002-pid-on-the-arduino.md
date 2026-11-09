# ADR-002: PID control runs on the Arduino

- Status: Accepted
- Date: 2026-11-09

## Context
The roaster must hold the bean temperature on a profile. The loop can live in the PC app or on the board.

## Decision
The PID and all safety rules run in the firmware. The GUI only sends the mode (`h`, `q`, `s`), the set points and
shows the frames it receives.

## Reasons
- A steady 1 s sampling period; Windows timers are not real-time.
- Safety does not depend on the PC: heating ends after 600 s and PID stops at 250 °C by itself.
- A closed or frozen GUI does not leave the loop running blind (apart from the last mode).

## Consequences
Tuning means re-flashing the board. The gains (Kp 21.095, Ki 0.2, Kd 30) were taken from a MATLAB simulation and are
constants in the sketch. The current code does not use Kd or update the previous error (system design, limitation 1).
If the cable is pulled, the board keeps its last mode; there is no watchdog.

## Rejected alternatives
- PID in C# with the board as a dumb I/O device: simpler to tune, but loses safety when the PC hangs.
