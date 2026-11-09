# Coffee Roasto — Monitoring dan Kontrol Mesin Sangrai Kopi

[English](README.md)

Coffee Roasto adalah proyek capstone yang mengotomatiskan mesin sangrai kopi tipe drum. Arduino membaca dua
termokopel dan menggerakkan kompor, motor drum, blower, cooler, dan valve gas; aplikasi desktop Windows
menampilkan suhu secara langsung, merekamnya, dan menjalankan roasting otomatis dengan kontroler PID yang
mengikuti profil suhu referensi.

> Status: GUI dan firmware Arduino sudah dibangun dan diuji pada mesin. Kekurangan yang diketahui ada di
> [desain sistem](docs/id/system-design.md#10-keterbatasan-yang-diketahui).

## Sorotan
- Suhu biji, drum, dan referensi tampil langsung di grafik, satu data tiap 4 detik
- Kontrol manual kompor, motor drum, blower, cooler, dan valve dengan lampu indikator status
- Mode heating: memanaskan drum sampai 150 °C selama 10 menit
- Mode PID: mengikuti profil referensi dari file Excel (suhu biji per langkah)
- Pengaman berhenti di 250 °C dan sinyal "roasting selesai" dari hardware
- Rekam sesi dan ekspor ke Excel

## Tech stack
C# · .NET Framework 4.7.2 · Windows Forms · Chart (DataVisualization) · ExcelDataReader · Excel Interop ·
Arduino (C++) · penguat termokopel MAX6675 · Servo · serial USB (9600 baud)

## Dokumentasi
- [Kebutuhan (sesuai yang dibangun)](docs/id/PRD.md)
- [Desain sistem](docs/id/system-design.md)
- [Keputusan arsitektur](docs/id/adr/)
- [Protokol serial](docs/id/serial-protocol.md)
- [Panduan hardware](docs/id/hardware.md)
- [Tanya jawab teknis](docs/id/technical-qa.md)
- [Build, instalasi, dan operasi](docs/id/deployment.md)
- Diagram (Mermaid): [`docs/en/diagrams/`](docs/en/diagrams/)

## Struktur repositori
| Path | Isi |
| --- | --- |
| `gui/` | Solution Visual Studio aplikasi desktop (`Form1.cs`, `Coffee.cs`, `PilotLamp.cs`) |
| `installer/` | Installer ClickOnce untuk GUI (`setup.exe`, `CoffeeRoasto_GUI.application`, `autorun.inf`) |
| `profiles/` | Contoh profil suhu referensi (`.xls`, `.xlsx`) |
| `firmware/Program_PID_CP/` | Firmware yang berjalan di mesin |
| `firmware/Pengujian_Hardware/` | Sketsa uji relay |
| `firmware/moveServo/` | Sketsa ramp servo yang belum selesai (tidak bisa dikompilasi, lihat keterbatasan) |

## Memulai
Kebutuhan: Windows dengan .NET Framework 4.7.2, Microsoft Excel (hanya untuk tombol ekspor), Visual Studio 2017
atau lebih baru (untuk build GUI), Arduino IDE dengan library `MAX6675` (untuk mengunggah firmware).

```text
1. Buka firmware/Program_PID_CP/Program_PID_CP.ino di Arduino IDE,
   pasang library "MAX6675", lalu unggah ke board.
2. Pasang GUI: jalankan "installer/setup.exe"
   (atau buka CoffeeRoasto_GUI.sln di Visual Studio lalu tekan F5).
3. Colokkan board, jalankan Coffee Roasto, pilih port COM, lalu tekan CONNECT.
```

Pengaturan regional Windows harus memakai koma sebagai pemisah desimal (misalnya Indonesia); lihat
[pemecahan masalah](docs/id/deployment.md#pemecahan-masalah).

| Aksi | Fungsi |
| --- | --- |
| CONNECT / DISCONNECT | Membuka atau menutup port serial 9600 baud; semua kontrol nonaktif sebelum terhubung |
| Burner, Motor drum, Blower drum, Cooler, Motor valve | ON/OFF manual (valve: OPEN/CLOSE); lampu menyala hijau terang saat aktif |
| HEATING MODE ON/OFF | Pemanasan ke 150 °C selama 600 detik dengan motor drum dan blower hidup |
| PID MODE ON | Memulai loop PID dengan set point 150 °C sampai profil dikirim |
| BROWSE + SHEET + START | Memuat kolom `beantemperature` dari sheet Excel dan mengirimnya ke board per 10 data |
| INPUT TEMPERATURE + SEND | Mengatur satu suhu referensi secara manual |
| RECORD DATA START / STOP / EXPORT | Mengumpulkan data ke tabel dan mengekspornya ke Excel |

Build lewat command line: `msbuild "gui/CoffeeRoasto_GUI.sln" /p:Configuration=Release`
(jalankan `nuget restore` lebih dulu).

## Keselamatan
Firmware mematikan kompor dan berhenti di 250 °C, tetapi kompor gas, drum panas, dan bagian yang bergerak tetap
berbahaya. Jangan tinggalkan proses roasting tanpa pengawasan; uji perubahan dengan kompor dilepas dahulu.
