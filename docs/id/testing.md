# Pengujian dan demo

9 Nov 2026 · @M Ridhwan Habib A

Cara Coffee Roasto diuji tanpa mesin, apa yang dibuktikan pengujian, dan apa yang tidak bisa. Alat yang sama membuat screenshot dan animasi di README, jadi gambarnya adalah aplikasi yang sebenarnya.

## 1. Yang diuji

| Lapisan | Alat | Yang diperiksa | Hasil (9 Nov 2026) |
| --- | --- | --- | --- |
| Logika GUI | `tests/gui-logic/run.ps1` | `FrameParser`, `ProfileTransmitter.BuildChunks` (dibandingkan dengan algoritma asli untuk 0–45 baris), `RecordingSession` | 69 pemeriksaan lolos |
| Firmware | `tests/firmware-sim/run.ps1` | Sketsa dijalankan di PC pada 12 skenario; perubahan pin, sudut servo, dan baris serial dibandingkan dengan `golden/` | 12 dari 12 cocok |
| Kompilasi (GUI) | MSBuild, Release | Solution terbangun tanpa warning | lolos |
| Kompilasi (firmware) | `arduino-cli compile --fqbn arduino:avr:uno` | Tanpa warning dengan `--warnings all`; flash 11.104 byte (34 %), RAM 334 byte (16 %) | lolos |
| Tampilan | `tools/screenshots/run.ps1` | Jendela asli menggambar satu roasting penuh; dipakai untuk gambar di bawah | 5 screenshot, 1 GIF |

Keluaran mentah ada di [`docs/assets/testing/`](../assets/testing/).

## 2. Simulasi firmware

`tests/firmware-sim/harness.cpp` mengompilasi sketsa asli bersama pengganti kecil untuk inti Arduino (`shim/`: `String`, `Serial`, `millis`, `digitalWrite`, `Servo`, `MAX6675`). Jam virtual hanya maju saat firmware menunggu, sehingga roasting 29 menit berjalan kurang dari satu detik dan hasilnya selalu sama.

| # | Skenario | Berakhir dengan |
| --- | --- | --- |
| 1 | Semua perintah manual, lalu set point ketikan 120 °C | `s` |
| 2 | Mode heating sampai 600 detik habis | `V` |
| 3 | Mode heating dihentikan operator | `s` |
| 4 | Mode PID, suhu biji melonjak di atas 250 °C | `X` (suhu berlebih) |
| 5 | Mode PID, profil dua baris, baris terakhir pendek | `X` (profil habis) |
| 6 | Mode PID dihentikan operator | `X` |
| 7 | Mode PID, satu kelompok sepuluh yang dipakai ulang sampai dihentikan | `X` setelah `s` |
| 8 | Set point ketikan, lalu mode PID | `X` setelah `s` |
| 9, 10, 11 | Suhu berada di sekitar set point, sehingga PID keluar dari batasnya (heating, PID, manual) | sesuai skrip |
| 12 | Satu roasting penuh: profil rekaman `Testing Baru.xls.xlsx` (427 baris) dalam loop tertutup dengan model pemanasan sederhana | `X` pada 28:42 |

**Asal `golden/`.** Jejaknya direkam dari firmware asli satu-file (commit `a5cca6b`), bukan dari yang sudah di-refactor. Firmware hasil refactor mereproduksinya persis; itulah bukti bahwa penataan ulang tidak mengubah perilaku. Mutasi sengaja pada rumus PID terdeteksi oleh skenario 9 dan 10, jadi perbandingannya peka.

**Satu detail perbandingan.** Di PC `double` berukuran 64 bit, sedangkan di AVR 32 bit seperti `float`. Karena itu jejak emas direkam dengan konstanta kalibrasi bertipe `float`, seperti perilakunya di board; dengan konstanta 64 bit satu nilai dari 1.268 baris berbeda di desimal kedua.

Skenario 12 dalam angka: 458 frame data, suhu biji mulai dari 139 °C dan mengikuti profil sampai 210 °C (puncak 211 °C), kompor dinyalakan 69 kali, dan setelah 3 menit pertama suhu biji rata-rata 1,3 °C di bawah profil (antara −5,6 dan +1,9 °C). Model pemanasannya hanya mainan; angka ini menggambarkan logika kontroler, bukan mesin sungguhan.

## 3. Screenshot dan animasi

`tools/screenshots/run.ps1` membangun GUI, membuka jendela asli, dan memutar ulang skenario 12 lewat `HandleLine` seolah barisnya datang dari port serial. Hanya port serial yang tidak ada; status terhubung dan lampu yang akan diatur tombol diatur manual. Jam dikunci dengan `RoastClock.FreezeAt` supaya sumbu waktu menampilkan 29 menit. Keluaran: `docs/assets/screenshots/` dan `docs/assets/flows/pid-roast.gif`.

| | |
| --- | --- |
| ![Awal](../assets/screenshots/01-start.png) | ![Terhubung](../assets/screenshots/02-connected.png) |
| 1. Awal: semua mati sampai CONNECT | 2. Terhubung: kontrol aktif, port COM dipilih |
| ![Profil](../assets/screenshots/03-profile-loaded.png) | ![5 menit](../assets/screenshots/04-pid-running-05min.png) |
| 3. File dan sheet profil dimuat ke tabel data | 4. Mode PID setelah 5 menit |
| ![15 menit](../assets/screenshots/04-pid-running-15min.png) | ![Selesai](../assets/screenshots/05-roast-finished.png) |
| 4. Mode PID setelah 15 menit: suhu biji mengikuti referensi, lampu burner mengikuti `Y` dan `Z` | 5. Roasting selesai: rekaman ada di tabel data, lampu cooler menyala |

![Roasting PID](../assets/flows/pid-roast.gif)

## 4. Cara menjalankan

```powershell
tests/gui-logic/run.ps1          # butuh Visual Studio (Roslyn) dan `nuget restore` di gui/
tests/firmware-sim/run.ps1       # butuh Visual Studio C++ build tools (cl.exe)
tools/screenshots/run.ps1        # butuh MSBuild; GIF butuh Python dengan Pillow
```

Setelah perubahan perilaku firmware yang disengaja, jalankan `tests/firmware-sim/run.ps1 -Update` dan tinjau selisih `golden/` sebelum di-commit. Tambahkan prototype ke `tests/firmware-sim/prototypes.h` bila sebuah tab mendapat fungsi baru.

## 5. Yang tidak diuji

- Mesin sungguhan: relay, servo, termokopel, valve gas, dan perilaku termalnya. Pakai daftar pengecekan di [panduan deploy](deployment.md) dengan kompor dilepas lebih dulu.
- Koneksi serial itu sendiri (kabel, driver, timeout). Simulasi menyerahkan baris langsung ke kode.
- Ekspor Excel (butuh Excel) dan dialog file.
- Timeout `readStringUntil` 1 detik akibat perintah tanpa newline (keterbatasan 4); simulasi mengirim baris utuh.
- Handler event GUI itu sendiri; logikanya tipis dan diperiksa lewat screenshot.
