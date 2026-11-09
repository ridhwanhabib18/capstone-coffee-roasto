# Desain Sistem Coffee Roasto

9 Nov 2026 · @M Ridhwan Habib A

Coffee Roasto v2.0 terdiri dari dua program: aplikasi desktop WPF di .NET 10 dan firmware Arduino, dihubungkan kabel serial USB. Firmware memegang loop kontrol real-time; aplikasi mengawasi, memplot, dan merekam. Dokumen ini menjelaskan sistem sebagaimana dibangun, dari kode di repositori. Diagramnya ada sebagai berkas Mermaid di [`docs/en/diagrams/`](../en/diagrams/) dan dirender di versi [bahasa Inggris](../en/system-design.md#2-system-context).

## 1. Cakupan & tujuan kualitas

Desain ditujukan untuk satu mesin sangrai, satu operator pada satu waktu, dengan PC Windows di samping mesin.

| Tujuan kualitas | Target | Respons desain |
| --- | --- | --- |
| Keselamatan | Kompor tidak tetap hidup setelah stop atau suhu berlebih | Relay default mati, perintah `s`, batas keras 250 °C di firmware |
| Kualitas kontrol | Suhu biji mengikuti profil | PID di board, set point diperbarui tiap ~4 detik |
| Responsif | Data baru tampil dalam sedetik setelah tiba | Event serial, lalu `Dispatcher.BeginInvoke` ke thread UI |
| Ketahanan | Aplikasi tidak crash karena frame rusak atau kabel dicabut | `try/catch` pada parsing dan penulisan serial |
| Sederhana | Operator cukup memilih port COM | Satu jendela, satu tombol per aksi, satu `.exe` tanpa installer |
| Mudah dirawat | Kedua sisi terbaca oleh mahasiswa | Protokol satu karakter; `Core` tanpa layar, view model, jendela tipis; satu tab kecil per tugas di firmware |

**Gaya arsitektur:** GUI pengawas ditambah kontroler tertanam. Setelah sebuah mode berjalan, kontroler tidak bergantung pada PC untuk tetap aman, kecuali perintah `s` dan suplai profil.

## 2. Konteks sistem

Operator memakai GUI; GUI berbicara serial 9600 baud dengan firmware; firmware membaca dua termokopel dan menggerakkan kompor, motor drum, blower, cooler, dan valve; file Excel dibaca sebagai profil dan ditulis sebagai ekspor. (Diagram: `diagrams/context.mmd`.)

## 3. Komponen

Aplikasi adalah jendela WPF di atas library yang tidak punya layar. View model adalah satu-satunya yang diajak bicara jendela; `Core` bisa dibaca dan diuji sendiri.

| Folder / file | Kelas | Tanggung jawab |
| --- | --- | --- |
| `src/CoffeeRoasto.Core/Hardware/RoasterCommand.cs` | `RoasterCommand` | Karakter perintah, satu konstanta untuk tiap perintah |
| `Hardware/RoasterLink.cs` | `RoasterLink` | Port serial (9600 baud): buka, tutup, kirim, event `LineReceived` |
| `Hardware/FrameParser.cs`, `ParsedLine.cs` | `FrameParser`, `ParsedLine`, `Telemetry`, `RoasterEvent` | Mengubah baris yang diterima menjadi pembacaan atau kejadian; invariant culture; logika murni |
| `Profiles/ProfileReader.cs` | `ProfileReader` | Membaca kolom `beantemperature` dari tiap sheet file `.xls` atau `.xlsx` |
| `Profiles/ProfileTransmitter.cs` | `ProfileTransmitter` | Mengirim profil per sepuluh nilai, berjarak 40 detik, sebagai task latar |
| `Recording/RecordingSession.cs` | `RecordingSession`, `Coffee` | Sampel yang dikumpulkan saat merekam |
| `Recording/ExcelExporter.cs` | `ExcelExporter` | Menulis rekaman ke file `.xlsx` dengan ClosedXML; Excel tidak perlu terpasang |
| `src/CoffeeRoasto.App/ViewModels/MainViewModel.cs` | `MainViewModel` | Semua yang ditampilkan jendela (pembacaan, selisih, rate of rise, lampu, tabel) dan setiap perintah di balik tombol |
| `ViewModels/TemperatureHistory.cs` | `TemperatureHistory` | Titik-titik grafik; memicu `Changed` |
| `MainWindow.xaml`, `.xaml.cs` | `MainWindow` | Jendela Fluent (WPF-UI): tata letak, tema terang dan gelap, grafik ScottPlot, dialog file, toast |
| `Controls/LampIndicator.xaml`, `KpiTile.xaml` | `LampIndicator`, `KpiTile` | Lampu bulat berhalo dan kartu dengan satu pembacaan |
| `App.xaml`, `App.xaml.cs` | `App` | Start-up, kamus tema Fluent, warna ketiga garis |
| firmware `Program_PID_CP.ino` | | `setup()` dan `loop()`: baca perintah, baca sensor, arahkan ke mode |
| firmware `Config.h`, `Commands.h`, `Globals.h` | | Pin dan batas, karakter protokol, objek bersama |
| firmware `Actuators.ino` | | Relay, servo valve, pengalihan kompor oleh keluaran PID |
| firmware `TemperatureSensors.ino` | | Sampling, rata-rata, kalibrasi, dead band, frame data |
| firmware `Pid.ino` | | Perhitungan PID |
| firmware `ReferenceProfile.h` | `ReferenceProfile` | Kelompok sepuluh set point yang menunggu dan yang aktif |
| firmware `ManualMode.ino` | | Satu perintah per output, set point ketikan, penahan kompor manual |
| firmware `PidMode.ino`, `HeatingMode.ino` | | Dua loop yang berjalan sampai selesai, dihentikan, atau suhu berlebih |

Ketergantungan searah: `MainWindow` memakai `MainViewModel`; view model memakai `Core`; `Core` tidak mengenal jendela. Tes dan alat screenshot memakai view model tanpa jendela. (Diagram: `diagrams/components.mmd`.)

## 4. Tampilan runtime

**Telemetri.** Saat CONNECT, GUI membuka port dan mengirim `#`. Firmware membaca sensor tiap 1 detik; setelah 4 sampel ia merata-rata, mengkalibrasi, menerapkan dead band 0,25 °C, dan mengirim `@bean A drum B ref C`. GUI mem-parsing, membagi 100, dan memperbarui label dan grafik lewat `BeginInvoke`; bila perekaman aktif, ia menambah `Coffee(bean, drum)` ke daftar. (Diagram: `diagrams/sequences/telemetry.mmd`.)

**Roasting otomatis dengan profil.** PID MODE ON mengirim `q`; firmware menyalakan motor drum dan blower, mematikan cooler, dan memakai set point 150 °C. Setelah START, GUI mengirim kelompok sepuluh nilai tiap 40 detik; firmware memakai satu nilai per ~4 detik. Burner hidup saat PID = 40 dan mati saat PID = 0 (`Y`/`Z`). Saat profil habis (set point 0) firmware mengirim `X`; bila operator menekan OFF atau suhu biji ≥ 250 °C, `s` atau batas suhu menghentikannya. (Diagram: `diagrams/sequences/pid-profile.mmd`.)

## 5. Mode

Idle → Manual (perintah tunggal) · Idle/Manual → Heating (`h`) → Idle setelah 600 detik (`V`) atau `s` · Idle/Manual → PID (`q`) → set point 150 °C → profil → Cooling saat set point 0 (`X`) · PID/profil → Idle pada `s` atau biji ≥ 250 °C. (Diagram: `diagrams/modes.mmd`.)

## 6. Loop kontrol

- **Sampling:** tiap 1 detik satu pembacaan per sensor; empat pembacaan dirata-rata, jadi frame baru muncul tiap ~4 detik.
- **Kalibrasi:** regresi linear per sensor, lalu dead band 0,25 °C (lihat [panduan hardware](hardware.md)).
- **PID:** `e = set point − biji`; `PID = 21,095·e + 0,2·(e + e_prev) + (e − e_prev)`, dibatasi 0–40. Angka yang sama menggerakkan valve (`servo = 40 − PID`, jadi error besar membuka valve gas ke 0° dan tanpa error menutupnya di 40°) dan relay kompor (hidup di 40, mati di 0).
- **Set point:** 150 °C sampai profil tiba, lalu satu nilai per sampel, lalu `0` untuk mengakhiri.
- **Batas:** biji ≥ 250 °C menghentikan mode PID; mode heating berjalan 600 detik.

## 7. Data

| Data | Lokasi | Bentuk |
| --- | --- | --- |
| Pembacaan terakhir | `BeanText`, `DrumText`, `ReferenceText`, `DeltaText`, `RateText`, `ElapsedText` di view model | Teks, °C, satu desimal |
| Titik grafik | `TemperatureHistory` | Empat list `double`: detik, biji, drum, referensi |
| Rekaman | `RecordingSession.Samples` | Di memori, dihapus saat START; hilang saat ditutup kecuali diekspor |
| Profil | `DataTableCollection` dari ExcelDataReader | Hanya kolom berheader `beantemperature`; satu tabel per sheet |
| Ekspor | `.xlsx` baru, sheet "Roast" | Baris header + satu baris per sampel: waktu, biji, drum, referensi |
| Contoh file | `profiles/` | Profil `.xls`, `.xlsx` |

Tidak ada database dan tidak ada akses jaringan.

## 8. Threading

`SerialPort.DataReceived` berjalan di thread pool. `RoasterLink` membaca barisnya di sana dan memicu `LineReceived`; view model menyerahkan tiap baris ke thread UI (`Post`, bawaannya `Dispatcher.BeginInvoke`) dan baru kemudian mem-parsing dan memperbarui layar. Pengiriman profil adalah `Task` latar yang menunggu 40 detik di antara kelompok. Jam layar adalah `DispatcherTimer` satu detik. Semua yang di-bind jendela hanya berubah di thread UI.

## 9. Build & distribusi

`CoffeeRoasto.slnx` berisi empat proyek: `CoffeeRoasto.Core` (net10.0), `CoffeeRoasto.App` (net10.0-windows, WPF), `CoffeeRoasto.Tests` (xUnit), dan `tools/screenshots`. Paket (semua MIT, BSD, atau Apache): WPF-UI, ScottPlot.WPF, CommunityToolkit.Mvvm, ClosedXML, ExcelDataReader (+ DataSet), System.IO.Ports, xUnit. `installer/publish.ps1` membuat satu `CoffeeRoasto.exe` self-contained single-file (sekitar 74 MB) yang tidak butuh .NET di PC. Installer ClickOnce v1.0 disimpan di `installer/v1-clickonce/`. Lihat [panduan deploy](deployment.md).

## 10. Keterbatasan yang diketahui

Titik-titik di mana kode dan maksudnya berbeda. Daftar ini agar tidak ada yang terkejut.

| # | Area | Pengamatan | Dampak |
| --- | --- | --- | --- |
| 1 | PID firmware | `PREVIOUS_ERROR` tetap 0 dan `KD` (30) tidak dipakai; integral = `KI·e` dan turunan = `e` | Berperilaku seperti P + I kecil + P tambahan; gain MATLAB belum sepenuhnya berlaku |
| 2 | Parsing aplikasi | ~~`Convert.ToDouble(...) / 100` bergantung pada lokal PC~~ **Diperbaiki di v2.0**: nilai dibaca dengan invariant culture | tidak ada |
| 3 | Auto mode `m` | Aplikasi mengirim `m` tetapi firmware tidak punya handler | AUTO MODE ON tidak berguna; pakai PID MODE |
| 4 | Framing serial | Aplikasi menulis perintah tanpa newline, firmware menunggu `\n` | Perintah tiba setelah timeout `readStringUntil` 1 detik |
| 5 | Threading | ~~Lampu diubah di thread serial~~ **Diperbaiki**: baris ditangani di thread UI | tidak ada |
| 6 | Tombol Burner ON | ~~menulis tanpa cek `IsOpen`~~ **Diperbaiki**: semua perintah lewat `RoasterLink.Send` | tidak ada |
| 7 | Heating OFF | ~~menampilkan "Roasting Selesai" sebelum mengirim `s`~~ **Diperbaiki di v2.0**: tanpa pesan | tidak ada |
| 8 | Pengiriman profil | Menunggu 40 detik antar kelompok dan tanpa tombol batal; OFF menghentikan firmware tetapi aplikasi terus mengirim baris | Tutup aplikasi atau tunggu untuk membatalkan |
| 9 | `moveServo.ino` | Tidak bisa dikompilasi | Tidak dipakai firmware |
| 10 | Repositori | ~~hasil build dan kunci penandatanganan ada di pohon~~ **Diperbaiki**: diabaikan git, file v1 di `archive/` dan `installer/v1-clickonce/` | tidak ada |
| 11 | Ekspor | ~~butuh Excel dan melewati baris terakhir~~ **Diperbaiki di v2.0**: ClosedXML menulis `.xlsx`, setiap sampel diekspor | tidak ada |
| 12 | Akhir profil | Tanpa baris baru firmware memakai ulang kelompok sepuluh terakhir, dan hanya nilai kosong yang mengakhiri roasting | Profil dengan jumlah baris kelipatan sepuluh tidak berakhir sendiri; tekan OFF atau tunggu 250 °C |
| 13 | Build | `dotnet build` yang me-restore dan membangun proyek WPF dalam satu perintah bisa gagal di checkout bersih | Jalankan `dotnet restore` lebih dulu, atau pakai `check.ps1` |
