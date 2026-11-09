# ADR-006: WPF dengan WPF-UI di .NET 10, library Core tanpa layar, dan exe single-file

- Status: Diterima
- Tanggal: 2026-11-09
- Menggantikan: [ADR-005](005-windows-forms-clickonce.md)

## Konteks
v1.0 adalah aplikasi Windows Forms di .NET Framework 4.7.2 dengan ClickOnce. Tampilannya kuno, kontrol grafiknya tidak ada
di .NET modern, Excel Interop butuh Excel, tampilan buram di layar berskala, dan semua logika ada di balik form. Proyek
ini kini boleh memakai paket tambahan.

## Keputusan
- **.NET 10** (`net10.0`, `net10.0-windows`), solusi berisi `CoffeeRoasto.Core` (tanpa layar), `CoffeeRoasto.App` (WPF),
  tes xUnit, dan alat screenshot.
- **WPF dengan WPF-UI 4** untuk tampilan Fluent sungguhan (kartu, Mica, tema terang dan gelap), **ScottPlot 5** untuk
  grafik, **CommunityToolkit.Mvvm** untuk view model, **ClosedXML** untuk ekspor `.xlsx`, **ExcelDataReader** untuk
  membaca profil, **System.IO.Ports** untuk port serial.
- **MVVM**: jendela di-bind ke `MainViewModel`; logikanya (`FrameParser`, `ProfileTransmitter`, `RecordingSession`, ...) ada di `Core`.
- **Distribusi**: satu `CoffeeRoasto.exe` self-contained single-file dari `installer/publish.ps1`; tanpa installer, tanpa .NET di PC.

## Alasan
- WPF-UI memberi kontrol Fluent asli dan pengganti tema; meniru semuanya di WinForms berarti menggambar kontrol sendiri.
- ScottPlot berlisensi MIT, cepat, dan memberi zoom, crosshair, dan sumbu waktu yang benar; `Chart` lama tidak ada di .NET 10.
- Dengan logika di luar form, view model dan `Core` diuji dengan xUnit dan jendela bisa dijalankan oleh alat untuk
  membuat screenshot sungguhan.
- High-DPI benar secara bawaan, dan .NET 10 didukung bertahun-tahun, sedangkan .NET Framework 4.7.2 tidak berkembang.
- Satu file adalah hal paling sederhana untuk diserahkan ke operator mesin yang sering offline.

## Konsekuensi
- Sekitar 1.000 baris WinForms diganti; protokol serial dan firmware tidak berubah.
- File tunggalnya berukuran 74 MB (terkompresi, self-contained).
- ScottPlot membawa `SkiaSharp.Views.WPF`, yang di-restore untuk .NET Framework (NU1701, disembunyikan); sudah diperiksa
  dengan screenshot dan exe hasil publish.
- Mica butuh Windows 11; Windows 10 memakai latar polos.
- Membangun proyek WPF dengan restore implisit di checkout bersih bisa gagal; `check.ps1` melakukan restore lebih dulu.
- File ClickOnce v1.0 tetap di `installer/v1-clickonce/` hanya sebagai rujukan.

## Alternatif yang ditolak
- Tetap di WinForms dengan kontrol Fluent gambar sendiri: banyak kode untuk hasil lebih buruk, dan tanpa ScottPlot untuk grafik baru.
- WinUI 3: setup lebih banyak (packaging, windowing) untuk tampilan yang sama dengan WPF-UI pada alat satu jendela.
- UI kit WinForms (Guna, Siticone, ReaLTaiizor): berbayar atau lisensi tidak jelas, bukan Fluent, dan terikat satu vendor.
- UI web dengan jembatan serial: butuh program tambahan di PC.
