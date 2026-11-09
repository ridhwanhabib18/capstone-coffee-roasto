# Panduan hardware

9 Nov 2026 · @M Ridhwan Habib A

Apa yang diharapkan firmware ada di mesin sangrai. Nomor pin diambil dari `Config.h` firmware; cocokkan dengan pengkabelan Anda sebelum menyalakan kompor.

## Daftar komponen
| Komponen | Fungsi |
| --- | --- |
| Arduino (AVR, serial USB) | Menjalankan firmware |
| 2 × MAX6675 + termokopel tipe K | Suhu drum dan biji (12-bit, resolusi 0,25 °C) |
| 4 × kanal relay | Kompor, motor drum, blower, cooler |
| Servo hobi | Menggerakkan valve gas antara terbuka dan tertutup |
| Kompor gas dengan valve | Sumber panas |
| PC dengan port USB | Menjalankan GUI |

## Peta pin
| Sinyal | Pin Arduino | Catatan |
| --- | --- | --- |
| Relay kompor (`PIN_BURNER`) | 9 | Aktif LOW; sketsa uji menyalakannya bergantian seperti SSR |
| Relay cooler (`PIN_COOLER`) | 10 | Aktif LOW |
| Servo, valve gas (`PIN_VALVE_SERVO`) | 11 | 0° = terbuka penuh, 40° = tertutup penuh |
| Relay motor drum (`PIN_DRUM_MOTOR`) | 12 | Aktif LOW |
| Relay blower (`PIN_BLOWER`) | 13 | Aktif LOW |
| Termokopel drum (MAX6675 #1) | DO 3, CS 4, CLK 5 | `drumSensor` |
| Termokopel biji (MAX6675 #2) | DO 6, CS 7, CLK 8 | `beanSensor` |

Semua relay **aktif LOW**: `HIGH` berarti mati dan pin diset `HIGH` di `setup()`, sehingga setelah reset semuanya mati dan valve terbuka.

## Rantai pengukuran suhu
1. `readTemperature()` (`TemperatureSensors.ino`) membaca kedua sensor tiap 1 detik (`SAMPLE_INTERVAL_MS`).
2. Setelah `SAMPLES_PER_READING` = 4 pembacaan, hasilnya dirata-rata.
3. Koreksi regresi linear diterapkan: biji = (T₂ − 3,4921) / 0,9194 dan drum = (T₁ − 6,8969) / 0,8711. Baris yang dikomentari menyimpan koefisien sebelumnya.
4. Perubahan di bawah 0,25 °C (resolusi MAX6675) diabaikan untuk mengurangi fluktuasi.
5. Hasilnya dicetak sebagai frame data (lihat [protokol serial](serial-protocol.md)).

Bila termokopel atau penguat diganti, ulangi regresi; konstanta itu milik sensor yang asli.

## Perilaku keselamatan
- Semua output mati saat daya dinyalakan.
- Mode PID berhenti pada suhu biji ≥ 250 °C: kompor mati, blower mati, drum hidup, cooler mati, valve terbuka, `X` dikirim.
- `s` dari GUI mengakhiri heating atau PID kapan saja.
- Setelah profil selesai, cooler menyala dan drum tetap berputar sampai operator mematikannya.

## Sketsa uji
| Sketsa | Tujuan |
| --- | --- |
| `Pengujian_Hardware` | Menghidupkan dan mematikan relay kompor tiap 500 ms untuk mengecek pensaklaran |
| `moveServo` | Draf ramp servo lambat (100 ms per derajat). Belum bisa dikompilasi: `myServo` tidak dideklarasikan, `moveServo()` dipanggil tanpa argumen, dan ada kurung berlebih |
