# ADR-003: Excel files for reference profiles and exports

- Status: Accepted
- Date: 2026-11-09

## Context
Operators already plan roasts in spreadsheets and want to keep a record of every run.

## Decision
Reference profiles are read from `.xls`/`.xlsx` with ExcelDataReader, using only the column whose header is
`beantemperature`. Recorded data is exported with Microsoft Excel Interop.

## Reasons
- No new file format to teach; samples already exist in `profiles/`.
- ExcelDataReader reads both formats without Excel installed.

## Consequences
The export needs Excel on the PC and skips the last grid row. A header typo gives an empty table. Excel lock files
(`~$*.xlsx`) can end up in the profile folder and should not be committed.

## Rejected alternatives
- CSV: no dependency, but not how operators keep their profiles. Worth adding as a second export.
