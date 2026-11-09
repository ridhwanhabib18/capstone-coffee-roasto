# ADR-003: File Excel untuk profil referensi dan ekspor

- Status: Diterima
- Tanggal: 2026-11-09

## Konteks
Operator sudah merencanakan roasting di spreadsheet dan ingin menyimpan catatan tiap proses.

## Keputusan
Profil referensi dibaca dari `.xls`/`.xlsx` dengan ExcelDataReader, hanya memakai kolom berheader `beantemperature`.
Data rekaman diekspor ke `.xlsx` dengan ClosedXML (v1 memakai Microsoft Excel Interop).

## Alasan
- Tidak ada format file baru yang harus diajarkan; contoh sudah ada di `profiles/`.
- ExcelDataReader dan ClosedXML bekerja tanpa Excel terpasang; ExcelDataReader membaca kedua format.

## Konsekuensi
Salah ketik header menghasilkan tabel kosong. File `.xls` lama butuh penyedia code page yang didaftarkan `ProfileReader`.
File lock Excel (`~$*.xlsx`) bisa terbawa di folder profil dan tidak boleh di-commit. Ekspor v1 butuh Excel dan melewati
baris terakhir; v2.0 tidak membutuhkan keduanya.

## Alternatif yang ditolak
- CSV: tanpa dependensi, tetapi bukan cara operator menyimpan profil. Layak ditambahkan sebagai ekspor kedua.
