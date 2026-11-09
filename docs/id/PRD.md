# Kebutuhan Coffee Roasto (sesuai yang dibangun)

9 Nov 2026 · @M Ridhwan Habib A

Ini versi bahasa Indonesia; versi Inggris adalah rujukan utama dan diperbarui bersamaan. Proyek sudah ada sebelum halaman ini, jadi kebutuhan di bawah ditulis balik dari kode GUI dan firmware. Tiap kebutuhan punya pengecekan penerimaan yang dijawab ya atau tidak. Hal yang tidak terlihat di kode ada di Pertanyaan terbuka, tidak dikarang.

## 1. Ringkasan & masalah

**Nama produk:** Coffee Roasto

**Ringkasan satu kalimat:** Coffee Roasto memantau dan mengontrol mesin sangrai kopi tipe drum dari PC Windows, dan menyangrai otomatis dengan mengikuti profil suhu referensi memakai kontroler PID di Arduino.

**Masalah:** Menyangrai manual bergantung pada mata dan waktu operator, suhu tidak tercatat, dan dua sangrai dari biji yang sama jarang sama. Mesin butuh pengukuran suhu, kontrol yang bisa diulang, dan catatan tiap proses.

## 2. Tujuan, bukan tujuan & metrik keberhasilan

**Tujuan**
- Menampilkan suhu biji, drum, dan referensi secara langsung
- Menyalakan tiap aktuator secara manual dengan status yang terlihat
- Memanaskan drum otomatis
- Mengikuti profil referensi dari Excel dengan kontrol PID
- Merekam proses dan mengekspornya ke Excel

**Bukan tujuan (di luar cakupan v1)**
- Profil dengan kolom selain suhu biji (tanpa rate-of-rise, tanpa penanda first crack)
- Penyimpanan cloud, akses jarak jauh, atau banyak mesin
- Database; riwayat disimpan sebagai file Excel
- Klien mobile atau web
- Tuning PID dari GUI

**Metrik keberhasilan**

| Metrik | Target | Cara mengukur |
| --- | --- | --- |
| Interval data | Satu frame tiap ~4 detik | Lihat waktu pada grafik |
| Berhenti saat suhu berlebih | Kompor mati saat biji ≥ 250 °C | Uji dengan kompor dilepas dan pembacaan simulasi |
| Mengikuti profil | Suhu biji mengikuti profil | Bandingkan seri Bean dan Reference setelah proses |
| Waktu ke data pertama | < 1 menit dari peluncuran (pilih port, CONNECT) | Stopwatch |

## 3. Pengguna & peran

| Peran | Deskripsi | Bisa melakukan |
| --- | --- | --- |
| Operator | Orang yang menjalankan mesin | Semua fitur GUI; tidak ada login |

## 4. Kebutuhan fungsional

### 4.1 Koneksi
- **FR-1** Operator memilih port COM dari drop-down (daftar dimuat saat dibuka) lalu menekan CONNECT. *Terima:* progress bar 100 %, CONNECT nonaktif, DISCONNECT dan semua kontrol aktif.
- **FR-2** Sebelum terhubung, semua kontrol selain CONNECT nonaktif. *Terima:* saat dibuka tombol berwarna abu-abu.
- **FR-3** DISCONNECT menutup port, mengosongkan grafik dan tabel, dan mengembalikan label ke `....ºC`. Menutup jendela juga menutup port.

### 4.2 Pemantauan langsung
- **FR-4** Suhu biji, drum, dan referensi tampil sebagai label dan tiga seri grafik, dengan waktu (mm:ss) sejak connect atau sejak mode dimulai di sumbu X. *Terima:* titik baru muncul tiap frame.
- **FR-5** Label referensi menampilkan `....ºC` selama tidak ada set point aktif.

### 4.3 Kontrol manual
- **FR-6** Burner, motor drum, blower drum, cooler: tombol ON dan OFF. Motor valve: OPEN dan CLOSE. *Terima:* tiap tekan mengirim karakter protokolnya dan mengubah lampu (hijau terang = hidup, hijau redup = mati).
- **FR-7** Operator dapat mengetik suhu dan menekan SEND untuk memberi set point ke PID.

### 4.4 Mode heating
- **FR-8** HEATING ON memulai pemanasan 150 °C selama 600 detik dengan motor drum, blower, dan valve terbuka; lampu cooler mati. *Terima:* firmware mengirim `V` di akhir dan GUI menampilkan "Heating Selesai".

### 4.5 Roasting otomatis
- **FR-9** PID MODE ON memulai loop PID dengan set point 150 °C, motor drum dan blower hidup, cooler mati.
- **FR-10** BROWSE membuka file `.xls` atau `.xlsx`; SHEET mendaftar sheet-nya; tabel data menampilkan kolom `beantemperature` dari sheet terpilih.
- **FR-11** START mengirim kolom itu ke firmware per sepuluh nilai, menunggu 40 detik antar kelompok. *Terima:* seri referensi mengikuti sheet.
- **FR-12** Saat profil habis firmware mengirim `X`: kompor mati, blower mati, drum dan cooler hidup. GUI menampilkan "Roasting Selesai".
- **FR-13** OFF (atau suhu biji ≥ 250 °C) menghentikan roasting seketika.

### 4.6 Perekaman
- **FR-14** START mengosongkan grafik, tabel, dan daftar, lalu mulai mengumpulkan nilai biji dan drum; STOP menampilkannya di tabel data.
- **FR-15** EXPORT membuka Excel berisi header dan baris tabel pada sheet "Exported from gridview".

### 4.7 Status
- **FR-16** Lampu indikator menunjukkan status burner, motor drum, blower drum, cooler, valve, perekaman, mode heating, auto, dan PID. Lampu burner juga mengikuti kejadian firmware `Y` dan `Z`.

## 5. Kebutuhan non-fungsional
| Area | Kebutuhan |
| --- | --- |
| Keselamatan | Semua output mati saat daya hidup; berhenti di 250 °C; relay aktif LOW |
| Platform | Windows, .NET Framework 4.7.2; board Arduino AVR 9600 baud |
| Lokal | Kultur PC dengan pemisah desimal koma (lihat keterbatasan di desain sistem) |
| Dependensi | Excel terpasang untuk EXPORT |
| Offline | Berjalan tanpa jaringan |
| Bahasa | UI berbahasa Inggris dengan dua message box berbahasa Indonesia ("Roasting Selesai", "Heating Selesai") |

## 6. Glosarium

| Indonesia (kode asli) | Inggris (kode sekarang) |
| --- | --- |
| kompor | burner (`PIN_BURNER`, `setBurner`) |
| motorDC | drum motor (`PIN_DRUM_MOTOR`) |
| suhu | temperature |
| suhu referensi | reference temperature / set point |
| pemanasan | heating |
| biji | bean |
| waktu_ref | waktu heating, 600 detik (`HEATING_DURATION_S`) |
| Pengujian Hardware | hardware test |
| Data Suhu Referensi | reference temperature data |

## 7. Status implementasi
| Area | Status |
| --- | --- |
| Koneksi, grafik langsung, label | Selesai |
| Kontrol manual dan lampu | Selesai |
| Mode heating | Selesai |
| Mode PID dan profil dari Excel | Selesai (lihat keterbatasan 1 dan 8) |
| Auto mode `m` | Hanya di GUI; firmware tidak punya handler |
| Perekaman dan ekspor | Selesai (butuh Excel) |
| Ramp servo (`moveServo.ino`) | Draf, tidak bisa dikompilasi |

## 8. Pertanyaan terbuka
1. Apakah AUTO MODE (`m`) dihapus dari GUI, atau diimplementasikan di firmware?
2. Apakah Kd = 30 dan suku error sebelumnya seharusnya aktif (keterbatasan 1)?
3. Apakah parser suhu sebaiknya memakai `CultureInfo.InvariantCulture`, bukan bergantung pada lokal PC?
4. Apakah perlu log roasting (tanggal, jenis biji, berat, hasil) di samping ekspor suhu?
