# Build, instalasi, dan operasi

Cara membangun GUI, mengunggah firmware, dan menjalankan satu proses roasting. Desain di baliknya ada di [desain sistem](system-design.md); pengkabelan ada di [panduan hardware](hardware.md).

## Yang dibutuhkan
- PC Windows dengan port USB, **.NET Framework 4.7.2** (sudah ada di Windows 10/11) dan, untuk tombol EXPORT, **Microsoft Excel**.
- Untuk build GUI: **Visual Studio 2017 atau lebih baru** dengan workload ".NET desktop development", dan akses NuGet (ExcelDataReader 3.6.0, ExcelDataReader.DataSet 3.6.0, Microsoft.Office.Interop.Excel 15.0.4795.1000, System.Runtime.Extensions 4.3.1).
- Untuk mengunggah firmware: **Arduino IDE** dan library `MAX6675` (library `Servo` sudah ada di IDE).
- Mesin terpasang sesuai panduan hardware.
- Format regional Windows yang memakai **koma sebagai pemisah desimal** (misalnya Indonesia). Lihat [pemecahan masalah](#pemecahan-masalah).

## Langkah

### 1. Unggah firmware
1. Buka `firmware/Program_PID_CP/Program_PID_CP.ino`. Dua file `.ino` lain di folder itu (`read_temperature`, `pisahkansuhuref`) terbuka sebagai tab sketsa yang sama.
2. Pilih board dan port, lalu Upload.
3. Buka serial monitor 9600 baud. Setelah beberapa detik frame seperti `@37.11A38.67B0C` harus muncul tiap ~4 detik. Tutup monitor sebelum menjalankan GUI (hanya satu program yang bisa memegang port).

### 2. Pasang atau build GUI
Pasang: jalankan `installer/setup.exe` (ClickOnce). `autorun.inf` menjalankannya saat folder berada di CD atau flashdisk.

Build:
```bash
cd gui
nuget restore CoffeeRoasto_GUI.sln
msbuild CoffeeRoasto_GUI.sln /p:Configuration=Release
# hasil: bin/Release/CoffeeRoasto_GUI.exe (+ DLL ExcelDataReader di sebelahnya)
```
Jalankan dari `bin/Release` bersama DLL-nya; aplikasi tidak butuh folder `profiles`, folder itu hanya berisi contoh profil.

### 3. Menjalankan roasting
1. Jalankan Coffee Roasto, buka daftar COM PORT, pilih board, tekan **CONNECT**. Bar terisi dan kontrol aktif.
2. Cek pembacaan terhadap suhu yang diketahui dan pastikan semua lampu hijau tua.
3. Opsional, pemanasan awal: **HEATING MODE ON**. Tunggu "Heating Selesai" (10 menit).
4. Tekan **PID MODE ON**, lalu **BROWSE**, pilih file profil dan sheet, dan tekan **START**.
5. Tekan **RECORD DATA START** bila ingin log; tekan **STOP** saat selesai, lalu **EXPORT**.
6. Saat "Roasting Selesai" muncul, cooler menyala dan drum tetap berputar; matikan dengan tombol OFF setelah biji dikeluarkan.
7. **DISCONNECT** sebelum menutup.

Untuk membatalkan kapan saja, tekan tombol OFF dari mode yang berjalan (mengirim `s`).

### Format file profil
Workbook `.xls` atau `.xlsx`; setiap sheet yang punya sel header persis `beantemperature` ditawarkan. Tiap baris di bawah header adalah satu set point dalam °C, satu per ~4 detik. Sel kosong dan profil yang bukan kelipatan sepuluh mengakhiri roasting (set point 0). Contoh: `profiles/`.

## Daftar pengecekan
- [ ] Serial monitor menampilkan frame tiap ~4 detik pada 9600 baud
- [ ] GUI menampilkan nilai yang sama dengan monitor (bukan 0,37 untuk 37)
- [ ] Tiap tombol manual menyalakan relay dan lampu yang benar (kompor dilepas)
- [ ] OFF pada mode yang berjalan menghentikan output
- [ ] Dengan nilai biji simulasi ≥ 250 °C, mode PID berakhir dengan `X`
- [ ] EXPORT membuka Excel dengan header dan baris

## Pemecahan masalah
| Gejala | Penyebab | Solusi |
| --- | --- | --- |
| Port tidak ada di daftar | Kabel atau driver; daftar dibaca saat drop-down dibuka | Colok ulang, buka daftar lagi, pasang driver CH340/FTDI |
| "Access to the port is denied" | Serial monitor atau program lain memegang port | Tutup program itu |
| Suhu 100 kali terlalu kecil (0,37) | Kultur PC memakai titik sebagai desimal | Ubah format regional Windows, atau parse dengan invariant culture dan hapus `/ 100` |
| Kontrol tetap abu-abu | Belum terhubung | Tekan CONNECT |
| Tombol bereaksi setelah sekitar sedetik | Perintah tanpa newline, firmware menunggu timeout baca | Wajar (desain sistem, keterbatasan 4) |
| AUTO MODE tidak berbuat apa-apa | Firmware tidak punya handler `m` | Pakai PID MODE |
| Grafik kosong setelah START profil | Mode PID belum aktif | Tekan PID MODE ON lebih dulu |
| EXPORT gagal | Excel tidak terpasang | Pasang Excel, atau salin tabel data manual |
| "Roasting Selesai" langsung setelah heating OFF | Pesan ditampilkan saat tombol ditekan | Wajar (keterbatasan 7) |

## Rilis
Setelah perubahan, build ulang Release, publikasikan ulang lewat Visual Studio (Build → Publish) untuk menyegarkan `setup.exe` dan `CoffeeRoasto_GUI.application`, dan uji dengan daftar pengecekan di atas. Jauhkan `bin/`, `obj/`, `.vs/`, `packages/`, dan `.pfx` dari version control.
