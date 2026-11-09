# ADR-003: Excel files for reference profiles and exports

- Status: Accepted
- Date: 2026-11-09

## Context
Operators already plan roasts in spreadsheets and want to keep a record of every run.

## Decision
Reference profiles are read from `.xls`/`.xlsx` with ExcelDataReader, using only the column whose header is
`beantemperature`. Recorded data is exported to `.xlsx` with ClosedXML (v1 used Microsoft Excel Interop).

## Reasons
- No new file format to teach; samples already exist in `profiles/`.
- ExcelDataReader and ClosedXML work without Excel installed; ExcelDataReader reads both formats.

## Consequences
A header typo gives an empty table. Old `.xls` files need the code-page provider that `ProfileReader` registers.
Excel lock files (`~$*.xlsx`) can end up in the profile folder and should not be committed. The v1 export needed Excel
and skipped the last row; v2.0 needs neither.

## Rejected alternatives
- CSV: no dependency, but not how operators keep their profiles. Worth adding as a second export.
