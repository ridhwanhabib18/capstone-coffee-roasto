# Desain Sistem Coffee Roasto

9 Nov 2026 · @M Ridhwan Habib A

Coffee Roasto terdiri dari dua program: aplikasi desktop Windows Forms dan firmware Arduino, dihubungkan kabel serial USB. Firmware memegang loop kontrol real-time; aplikasi mengawasi, memplot, dan merekam. Dokumen ini menjelaskan sistem sebagaimana dibangun, dari kode di repositori. Diagramnya ada sebagai berkas Mermaid di [`docs/en/diagrams/`](../en/diagrams/) dan dirender di versi [bahasa Inggris](../en/system-design.md#2-system-context).

## 1. Cakupan & tujuan kualitas

Desain ditujukan untuk satu mesin sangrai, satu operator pada satu waktu, dengan PC Windows di samping mesin.

| Tujuan kualitas | Target | Respons desain |
| --- | --- | --- |
| Keselamatan | Kompor tidak tetap hidup setelah stop atau suhu berlebih | Relay default mati, perintah `s`, batas keras 250 °C di firmware |
| Kualitas kontrol | Suhu biji mengikuti profil | PID di board, set point diperbarui tiap ~4 detik |
| Responsif | Data baru tampil dalam sedetik setelah tiba | Event serial + `BeginInvoke` ke thread UI |
| Ketahanan | Aplikasi tidak crash karena frame rusak atau kabel dicabut | `try/catch` pada parsing dan penulisan serial |
| Sederhana | Operator cukup memilih port COM | Satu layar, satu tombol per aksi |
| Mudah dirawat | Kedua sisi terbaca oleh mahasiswa | Protokol satu karakter, satu `Form1.cs`, satu `.ino` |

**Gaya arsitektur:** GUI pengawas ditambah kontroler tertanam. Setelah sebuah mode berjalan, kontroler tidak bergantung pada PC untuk tetap aman, kecuali perintah `s` dan suplai profil.

## 2. Konteks sistem

Operator memakai GUI; GUI berbicara serial 9600 baud dengan firmware; firmware membaca dua termokopel dan menggerakkan kompor, motor drum, blower, cooler, dan valve; file Excel dibaca sebagai profil dan ditulis sebagai ekspor. (Diagram: `diagrams/context.mmd`.)

## 3. Komponen

GUI adalah satu layar (`Form_CoffeeRoasto`, dipecah menjadi beberapa file partial) ditambah beberapa kelas kecil dengan satu tugas masing-masing. Hanya `UI/` yang mengenal layar, sehingga `Hardware/` dan `Profiles/` bisa dibaca dan diuji sendiri.

| Folder / file | Kelas | Tanggung jawab |
| --- | --- | --- |
| `Form1.cs` | `Form_CoffeeRoasto` | Membuat objek bantu, menangani baris masuk (telemetri dan kejadian) |
| `Form1.Connection.cs` | (partial) | Daftar port COM, connect, disconnect, mengaktifkan atau menonaktifkan kontrol |
| `Form1.Manual.cs` | (partial) | Tombol burner, motor drum, blower, cooler, valve; set point manual |
| `Form1.Modes.cs` | (partial) | Start/stop heating, auto, dan PID beserta lampunya |
| `Form1.Recording.cs` | (partial) | Start/stop perekaman, ekspor |
| `Form1.Profile.cs` | (partial) | Memilih profil, memilih sheet, mengirimnya |
| `Form1.Designer.cs` | (dibuat otomatis) | Kontrol dan event dari designer Visual Studio |
| `Hardware/RoasterCommand.cs` | `RoasterCommand` | Karakter perintah, satu konstanta untuk tiap perintah |
| `Hardware/RoasterLink.cs` | `RoasterLink` | Port serial: buka (9600 baud), tutup, kirim, baca satu baris |
| `Hardware/FrameParser.cs`, `ParsedLine.cs` | `FrameParser`, `ParsedLine`, `Telemetry`, `RoasterEvent` | Mengubah baris yang diterima menjadi pembacaan atau kejadian (logika murni) |
| `Profiles/ProfileReader.cs` | `ProfileReader` | Membaca kolom `beantemperature` dari tiap sheet Excel |
| `Profiles/ProfileTransmitter.cs` | `ProfileTransmitter` | Mengirim profil per sepuluh nilai, berjarak 40 detik, di thread latar |
| `Recording/RecordingSession.cs`, `Coffee.cs` | `RecordingSession`, `Coffee` | Sampel yang dikumpulkan saat merekam |
| `Recording/ExcelExporter.cs` | `ExcelExporter` | Menyalin data grid ke workbook Excel baru |
| `UI/LampBoard.cs` | `LampBoard`, `Lamp` | Semua lampu indikator berdasarkan nama: `On(...)`, `Off(...)`, `AllOff()` |
| `UI/PilotLamp.cs` | `PilotLamp` | `Button` bulat; `BackColor` adalah warna lampu (hijau tua = mati, lime = hidup) |
| `UI/TemperatureChart.cs` | `TemperatureChart` | Grafik dan ketiga serinya |
| `UI/RoastClock.cs` | `RoastClock` | Sumbu waktu (mm:ss sejak connect atau mode dimulai) |
| `Program.cs` | `Program` | Titik masuk |
| firmware `Program_PID_CP.ino` | | `setup()` dan `loop()`: baca perintah, baca sensor, arahkan ke mode |
| firmware `Config.h`, `Commands.h`, `Globals.h` | | Pin dan batas, karakter protokol, objek bersama |
| firmware `Actuators.ino` | | Relay, servo valve, pengalihan kompor oleh keluaran PID |
| firmware `TemperatureSensors.ino` | | Sampling, rata-rata, kalibrasi, dead band, frame data |
| firmware `Pid.ino` | | Perhitungan PID |
| firmware `ReferenceProfile.h` | `ReferenceProfile` | Kelompok sepuluh set point yang menunggu dan yang aktif |
| firmware `ManualMode.ino` | | Satu perintah per output, set point ketikan, penahan kompor manual |
| firmware `PidMode.ino`, `HeatingMode.ino` | | Dua loop yang berjalan sampai selesai, dihentikan, atau suhu berlebih |

Ketergantungan searah: `Form1.*` memakai `Hardware`, `Profiles`, `Recording`, dan `UI`; `Profiles` memakai `Hardware`. `Hardware`, `Profiles`, dan `Recording` tidak merujuk ke form.

(Diagram: `diagrams/components.mmd`.)

## 4. Tampilan runtime

**Telemetri.** Saat CONNECT, GUI membuka port dan mengirim `#`. Firmware membaca sensor tiap 1 detik; setelah 4 sampel ia merata-rata, mengkalibrasi, menerapkan dead band 0,25 °C, dan mengirim `@bean A drum B ref C`. GUI mem-parsing, membagi 100, dan memperbarui label dan grafik lewat `BeginInvoke`; bila perekaman aktif, ia menambah `Coffee(bean, drum)` ke daftar. (Diagram: `diagrams/sequences/telemetry.mmd`.)

**Roasting otomatis dengan profil.** PID MODE ON mengirim `q`; firmware menyalakan motor drum dan blower, mematikan cooler, dan memakai set point 150 °C. Setelah START, GUI mengirim kelompok sepuluh nilai tiap 40 detik; firmware memakai satu nilai per ~4 detik. Burner hidup saat PID = 40 dan mati saat PID = 0 (`Y`/`Z`). Saat profil habis (set point 0) firmware mengirim `X`; bila operator menekan OFF atau suhu biji ≥ 250 °C, `s` atau batas suhu menghentikannya. (Diagram: `diagrams/sequences/pid-profile.mmd`.)

## 5. Mode

Idle → Manual (perintah tunggal) · Idle/Manual → Heating (`h`) → Idle setelah 600 detik (`V`) atau `s` · Idle/Manual → PID (`q`) → set point 150 °C → profil → Cooling saat set point 0 (`X`) · PID/profil → Idle pada `s` atau biji ≥ 250 °C. (Diagram: `diagrams/modes.mmd`.)

## 6. Loop kontrol

- **Sampling:** tiap 1 detik satu pembacaan per sensor; empat pembacaan dirata-rata, jadi frame baru muncul tiap ~4 detik.
- **Kalibrasi:** regresi linear per sensor, lalu dead band 0,25 °C (lihat [panduan hardware](hardware.md)).
- **PID:** `e = set point − biji`; `PID = 21,095·e + 0,2·(e + e_prev) + (e − e_prev)`, dibatasi 0–40. Angka yang sama menggerakkan valve (`servo = 40 − PID`) dan relay kompor (hidup di 40, mati di 0).
- **Set point:** 150 °C sampai profil tiba, lalu satu nilai per sampel, lalu `0` untuk mengakhiri.
- **Batas:** biji ≥ 250 °C menghentikan mode PID; mode heating berjalan 600 detik.

## 7. Data

| Data | Lokasi | Bentuk |
| --- | --- | --- |
| Suhu biji, drum, referensi terakhir | `beantemperature`, `drumtemperature`, `referencetemperature` di form | `double`, °C |
| Rekaman | `List<Coffee>` → `dataGridView` | Di memori, dihapus saat START; hilang saat ditutup kecuali diekspor |
| Profil | `DataSet` dari ExcelDataReader | Hanya kolom berheader `beantemperature`; satu tabel per sheet |
| Ekspor | Workbook Excel baru, sheet "Exported from gridview" | Baris header + satu baris per rekaman |
| Contoh file | `profiles/` | Profil `.xls`, `.xlsx` |

Tidak ada database dan tidak ada akses jaringan.

## 8. Threading

Event serial `DataReceived` berjalan di thread pool dan memanggil `RoasterLink.TryReadLine`; barisnya lalu diserahkan ke `HandleLine` di thread UI dengan `BeginInvoke`. Pengiriman profil berjalan di `BackgroundWorker` dan tidur 40 detik di antara kelompok, jadi UI tetap responsif. Hasil parsing, lampu, label, grafik, dan message box semuanya ditangani di thread UI.

## 9. Build & distribusi

Solution Visual Studio `CoffeeRoasto_GUI.sln`, target .NET Framework 4.7.2, output WinExe. Paket NuGet: ExcelDataReader 3.6.0, ExcelDataReader.DataSet 3.6.0, Microsoft.Office.Interop.Excel 15.0.4795.1000, System.Runtime.Extensions 4.3.1. Dipublikasikan dengan ClickOnce (`setup.exe`, `CoffeeRoasto_GUI.application`, `autorun.inf` untuk CD atau flashdisk). Lihat [panduan deploy](deployment.md).

## 10. Keterbatasan yang diketahui

Titik-titik di mana kode dan maksudnya berbeda. Daftar ini agar tidak ada yang terkejut; dokumentasi ini tidak mengubah satu pun.

| # | Area | Pengamatan | Dampak |
| --- | --- | --- | --- |
| 1 | PID firmware | `PREVIOUS_ERROR` tetap 0 dan `KD` (30) tidak dipakai; integral = `KI·e` dan turunan = `e` | Berperilaku seperti P + I kecil + P tambahan; gain MATLAB belum sepenuhnya berlaku |
| 2 | Parsing GUI | `Convert.ToDouble(...) / 100` bergantung pada lokal PC | Nilai salah di PC berlokal Inggris |
| 3 | Auto mode `m` | GUI mengirim `m` tetapi firmware tidak punya handler | AUTO MODE ON tidak berguna; pakai PID MODE |
| 4 | Framing serial | GUI `Write("n")` tanpa newline, firmware menunggu `\n` | Perintah tiba setelah timeout `readStringUntil` 1 detik |
| 5 | Threading | ~~`Data_Parsing` mengubah lampu di thread serial~~ **Diperbaiki**: baris ditangani di thread UI | tidak ada |
| 6 | Tombol Burner ON | ~~menulis tanpa cek `IsOpen`~~ **Diperbaiki**: semua perintah lewat `RoasterLink.Send` | tidak ada |
| 7 | Heating OFF | `button_StopHeatingMode_Click` menampilkan "Roasting Selesai" sebelum mengirim `s` | Pesan menyesatkan |
| 8 | Pengiriman profil | Memblokir 40 detik antar kelompok dan tidak bisa dibatalkan dari UI | Harus menekan OFF (`s`) untuk membatalkan |
| 9 | `moveServo.ino` | Tidak bisa dikompilasi | Tidak dipakai firmware |
| 10 | Repositori | `bin/`, `obj/`, `.vs/`, `packages/`, `Release.rar`, dan kunci penandatanganan ada di pohon | Repositori besar; jangan masukkan ke version control |
| 11 | Ekspor | `Excel Interop` butuh Excel terpasang (baris terakhir yang terlewat sudah **diperbaiki**) | Gagal di PC tanpa Excel |
| 12 | Akhir profil | Tanpa baris baru firmware memakai ulang kelompok sepuluh terakhir, dan hanya nilai kosong yang mengakhiri roasting | Profil dengan jumlah baris kelipatan sepuluh tidak berakhir sendiri; tekan OFF atau tunggu 250 °C |
