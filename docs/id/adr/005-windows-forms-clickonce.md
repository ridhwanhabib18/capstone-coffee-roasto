# ADR-005: Windows Forms di .NET Framework dengan ClickOnce

- Status: Digantikan oleh [ADR-006](006-wpf-on-dotnet-10.md) (v2.0)
- Tanggal: 2026-11-09

## Konteks
Mesin dioperasikan dari PC Windows yang mungkin tidak online. Aplikasi butuh port serial, grafik langsung, dan
installer sederhana.

## Keputusan
Bangun aplikasi Windows Forms di .NET Framework 4.7.2, pakai kontrol `SerialPort` dan `Chart` bawaan, dan
distribusikan dengan ClickOnce (`setup.exe`, `CoffeeRoasto_GUI.application`, `autorun.inf`).

## Alasan
- Semua yang dibutuhkan ada di framework; tidak ada runtime yang perlu dipasang di Windows 10/11.
- Designer memudahkan tim mengubah tata letak dengan cepat.
- ClickOnce terpasang dari folder, flashdisk, atau CD tanpa alat admin.

## Konsekuensi
Hanya Windows, dan tidak ada tes otomatis (logika ada di code-behind form). Memindahkan logika ke kelas, seperti pada
`Coffee`, memungkinkan unit test. Pindah ke .NET 8 Windows Forms mungkin dilakukan nanti.

## Alternatif yang ditolak
- WPF: lebih berat untuk alat satu layar.
- UI web dengan jembatan serial: butuh program tambahan di PC.
