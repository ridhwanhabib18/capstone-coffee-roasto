# ADR-001: Single-character serial protocol

- Status: Accepted
- Date: 2026-11-09

## Context
The GUI and the Arduino must exchange commands, temperatures and events over one USB serial link. Both programs are
small and maintained by students.

## Decision
Commands from the GUI are single ASCII characters (`n`, `k`, `t`, `d`, `h`, `q`, `s` ...); numbers and profile
lines are sent as plain text. The Arduino answers with a text frame `@<bean>A<drum>B<reference>C` and single-letter
events (`X`, `V`, `Y`, `Z`). Everything is documented in `docs/en/serial-protocol.md`.

## Reasons
- Trivial to parse on an AVR with `String` comparison.
- Testable by hand from the Arduino serial monitor.
- No framing, checksum or library needed on either side.

## Consequences
Commands are not self-describing and there is no error detection. A new command needs a free character and a change
on both sides. Any other text is read as a temperature, so a typo becomes a set point; the `m` command of the GUI
currently falls into this rule.

## Rejected alternatives
- JSON lines: readable, but heavy for an Uno-class board and unnecessary for 15 commands.
- Binary framing with checksum: robust, but harder to debug for the team.
