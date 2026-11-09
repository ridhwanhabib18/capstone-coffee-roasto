# ADR-003: File Excel untuk profil referensi dan ekspor

- Status: Diterima
- Tanggal: 2026-11-09

## Konteks
Operator sudah merencanakan roasting di spreadsheet dan ingin menyimpan catatan tiap proses.

## Keputusan
Profil referensi dibaca dari `.xls`/`.xlsx` dengan ExcelDataReader, hanya memakai kolom berheader `beantemperature`.
Data rekaman diekspor dengan Microsoft Excel Interop.

## Alasan
- Tidak ada format file baru yang harus diajarkan; contoh sudah ada di `profiles/`.
- ExcelDataReader membaca kedua format tanpa Excel terpasang.

## Konsekuensi
Ekspor butuh Excel di PC dan melewati baris terakhir grid. Salah ketik header menghasilkan tabel kosong. File lock
Excel (`~$*.xlsx`) bisa terbawa di folder profil dan tidak boleh di-commit.

## Alternatif yang ditolak
- CSV: tanpa dependensi, tetapi bukan cara operator menyimpan profil. Layak ditambahkan sebagai ekspor kedua.
