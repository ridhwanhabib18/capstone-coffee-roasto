# Protokol serial

9 Nov 2026 · @M Ridhwan Habib A

Kontrak antara Coffee Roasto v2.0 (`RoasterCommand`, `RoasterLink`, dan `FrameParser` di `CoffeeRoasto.Core`) dan firmware Arduino (`Program_PID_CP.ino`). Isinya diturunkan dari kode kedua sisi; ubah keduanya dan halaman ini bersamaan (ADR-001).

**Koneksi:** serial USB, 9600 baud, 8N1 (bawaan .NET dan Arduino), teks ASCII. Firmware membaca satu baris dengan `Serial.readStringUntil('\n')`.

## 1. GUI → Arduino (perintah)

| Karakter | Dikirim oleh (tombol) | Aksi firmware | Lampu di GUI |
| --- | --- | --- | --- |
| `#` | CONNECT | Bukan perintah; dibaca sebagai set point `0`. Menghapus referensi | — |
| `n` | Burner ON | Relay kompor hidup, set point 0 | Burner hijau terang |
| `k` | Burner OFF | Relay kompor mati, set point 0 | Burner hijau redup |
| `t` | Motor drum ON | Motor drum hidup | Motor drum hijau terang |
| `d` | Motor drum OFF | Motor drum mati | Motor drum hijau redup |
| `y` | Blower drum ON | Blower hidup | Blower hijau terang |
| `b` | Blower drum OFF | Blower mati | Blower hijau redup |
| `p` | Cooler ON | Cooler hidup | Cooler hijau terang |
| `c` | Cooler OFF | Cooler mati | Cooler hijau redup |
| `v` | Motor valve OPEN | Servo ke 0° (valve terbuka penuh) | Valve hijau terang |
| `x` | Motor valve CLOSE | Servo ke 40° (valve tertutup penuh) | Valve hijau redup |
| `h` | Heating mode ON | Loop heating: set point 150 °C selama 600 detik (bagian 4) | Heating, drum, blower, valve hijau terang |
| `q` | PID mode ON | Loop PID dengan set point 150 °C sampai profil tiba (bagian 4) | PID, drum, blower, valve hijau terang |
| `s` | OFF heating, PID, atau auto | Menghentikan loop yang berjalan, kompor mati | Lampu mode mati |
| `m` | Auto mode ON | **Tidak ada handler di firmware ini**; dibaca sebagai set point `0` | Auto hijau terang |
| `<angka>` | INPUT TEMPERATURE + SEND | Teks lain dibaca dengan `toFloat()` sebagai suhu referensi (°C) | — |
| `t1,t2,…,t10` | START profil dari file | Sepuluh set point dipisah koma, dikirim dengan `WriteLine` | — |

`s` hanya dibaca di dalam loop heating dan PID; pada mode manual tidak ada yang perlu dihentikan.

## 2. Arduino → GUI (telemetri dan kejadian)

### Frame data

```text
@<bean>A<drum>B<reference>C\n
@37.11A38.67B150.00C
```

| Field | Arti | Sumber |
| --- | --- | --- |
| `<bean>` | Suhu biji, terkalibrasi, °C | `beanSensor` (CS 7) |
| `<drum>` | Suhu drum, terkalibrasi, °C | `drumSensor` (CS 4) |
| `<reference>` | Set point saat ini, °C; `0` bila tidak ada | `roaster.setpoint` |

Satu frame dikirim per pembacaan rata-rata: 4 sampel tiap 1 detik, jadi **satu frame tiap ~4 detik**. `FrameParser` mencari penanda `@`, `A`, `B`, `C`, membaca tiap field sebagai angka, lalu meneruskan pembacaannya. Frame yang penandanya kurang atau angkanya rusak diabaikan.

> **Pemisah desimal.** Firmware mencetak nilai dengan titik (`37.11`). v2.0 membacanya dengan invariant culture, sehingga hasilnya tidak lagi bergantung pada pengaturan regional PC. (Aplikasi v1 membagi 100 dan hanya benar di PC berlokal Indonesia.)

### Baris kejadian

| Baris | Arti | Reaksi GUI |
| --- | --- | --- |
| `Y` | Kompor dinyalakan oleh PID | Lampu burner hijau terang |
| `Z` | Kompor dimatikan | Lampu burner hijau redup |
| `X` | Roasting selesai (profil habis, `s`, atau biji ≥ 250 °C) | Pesan "Roasting Selesai"; lampu auto, blower, burner mati; lampu drum dan cooler hijau terang |
| `V` | Heating selesai (600 detik) | Pesan "Heating Selesai"; lampu heating, blower, burner mati |

## 3. Pengiriman profil

GUI membaca kolom `beantemperature` dari sheet, menelusuri baris, dan membentuk kelompok sepuluh nilai dipisah koma. Setelah tiap kelompok sepuluh, GUI menunggu **40 detik** (10 nilai × ~4 detik per set point) agar firmware tidak kehabisan set point; kelompok terakhir yang lebih pendek dikirim di akhir. Firmware (`ReferenceProfile.h`) memecah baris pada koma menjadi satu kelompok sepuluh yang menunggu, menjadikannya kelompok aktif saat yang sebelumnya habis, dan maju satu set point per pembacaan rata-rata. Bila tidak ada baris baru, kelompok yang sama dipakai lagi, sehingga profil yang panjangnya kelipatan sepuluh tidak berakhir sendiri (keterbatasan 12). Nilai kosong menjadi `0`, dan set point `0` mengakhiri roasting (`X`).

Aktifkan mode PID (`q`) **sebelum** menekan START: profil hanya dibaca di dalam loop PID.

## 4. Mode firmware

| Mode | Valve | Aturan kompor | Berakhir |
| --- | --- | --- | --- |
| Manual | sesuai perintah | sesuai perintah | — |
| Heating (`h`) | terbuka (0°) | hidup saat PID ≥ 40 dan dalam 600 detik, mati saat PID ≤ 0 | setelah 600 detik (`V`) atau `s` |
| PID (`q`) | `40 − PID` derajat | hidup saat PID = 40, mati saat PID = 0 | set point 0 (`X`), `s`, atau biji ≥ 250 °C (`X`) |

`PID = Kp·e + Ki·(e + e_prev) + (e − e_prev)`, dibatasi 0–40, dengan `e = set point − suhu biji`, Kp = 21,095, Ki = 0,2 (Kd = 30 dideklarasikan tetapi tidak dipakai, dan `e_prev` tidak pernah diperbarui; lihat keterbatasan di desain sistem).
