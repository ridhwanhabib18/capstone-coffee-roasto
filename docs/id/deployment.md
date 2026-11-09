# Build, instalasi, dan operasi

Cara membangun Coffee Roasto v2.0, mengunggah firmware, dan menjalankan satu proses roasting. Desain di baliknya ada di [desain sistem](system-design.md); pengkabelan ada di [panduan hardware](hardware.md).

## Yang dibutuhkan
- PC Windows dengan port USB. **Windows 10 versi 1607 atau lebih baru, 64-bit.** `CoffeeRoasto.exe` hasil publish bersifat self-contained: .NET dan Excel tidak perlu dipasang.
- Untuk build aplikasi: **.NET 10 SDK** (`dotnet --version` menampilkan 10.x). Visual Studio bersifat opsional.
- Untuk mengunggah firmware: **Arduino IDE** (atau `arduino-cli`) dan library `MAX6675 library` (library `Servo` sudah ada di IDE).
- Mesin terpasang sesuai panduan hardware.
- Pengaturan regional tidak lagi berpengaruh: aplikasi membaca suhu dengan invariant culture.

## Langkah

### 1. Unggah firmware
1. Buka `firmware/Program_PID_CP/Program_PID_CP.ino`. File `.ino` lain dan file `.h` di folder itu terbuka sebagai tab sketsa yang sama. Untuk sekadar memeriksa bisa dikompilasi: `arduino-cli compile --fqbn arduino:avr:uno firmware/Program_PID_CP`.
2. Pilih board dan port, lalu Upload.
3. Buka serial monitor 9600 baud. Setelah beberapa detik frame seperti `@37.11A38.67B0C` harus muncul tiap ~4 detik. Tutup monitor sebelum menjalankan aplikasi (hanya satu program yang bisa memegang port).

### 2. Dapatkan aplikasi
**Pakai file hasil publish.** Salin `installer/dist/CoffeeRoasto.exe` ke PC lalu jalankan (sekitar 74 MB, tanpa installer).

**Atau build sendiri:**
```powershell
./check.ps1                  # restore, build, 32 tes, simulasi firmware
./installer/publish.ps1      # menulis installer/dist/CoffeeRoasto.exe
dotnet run --project src/CoffeeRoasto.App -c Release    # jalankan tanpa publish
```
`check.ps1` melakukan restore lebih dulu dan terpisah: me-restore dan membangun proyek WPF dalam satu `dotnet build` bisa gagal di checkout bersih.

### 3. Menjalankan roasting
1. Jalankan Coffee Roasto, pilih board di **COM port** (tekan tombol refresh bila tidak ada) lalu tekan **Connect**. Pil status menjadi hijau dan kartu kontrol aktif.
2. Cek pembacaan terhadap suhu yang diketahui dan pastikan semua lampu abu-abu.
3. Opsional, pemanasan awal: **Heating mode ON**. Toast "Heating finished" muncul setelah 10 menit.
4. Tekan **PID mode ON**, lalu **Browse**, pilih file profil dan sheet, dan tekan **Send profile**.
5. Tekan **Start** di kartu data bila ingin log; tekan **Stop** saat selesai, lalu **Export** dan pilih tempat menyimpan `.xlsx`.
6. Saat toast "Roasting finished" muncul, cooler menyala dan drum tetap berputar; matikan dengan tombol OFF setelah biji dikeluarkan.
7. **Disconnect** sebelum menutup.

Tombol **Dark** mengganti tema; grafik ikut berubah. Untuk membatalkan kapan saja, tekan tombol OFF dari mode yang berjalan (mengirim `s`).

### Format file profil
Workbook `.xls` atau `.xlsx`; setiap sheet yang punya sel header persis `beantemperature` ditawarkan. Tiap baris di bawah header adalah satu set point dalam °C, satu per ~4 detik. Sel kosong dan profil yang bukan kelipatan sepuluh mengakhiri roasting (set point 0). Contoh: `profiles/`.

## Daftar pengecekan
- [ ] Serial monitor menampilkan frame tiap ~4 detik pada 9600 baud
- [ ] Aplikasi menampilkan nilai yang sama dengan monitor
- [ ] Tiap tombol manual menyalakan relay dan lampu yang benar (kompor dilepas)
- [ ] OFF pada mode yang berjalan menghentikan output
- [ ] Dengan nilai biji simulasi ≥ 250 °C, mode PID berakhir dengan toast "Roasting finished"
- [ ] Export membuka `.xlsx` berisi semua sampel (periksa baris terakhir)
- [ ] Tombol tema membuat seluruh jendela, termasuk grafik, gelap lalu terang lagi

## Pemecahan masalah
| Gejala | Penyebab | Solusi |
| --- | --- | --- |
| Port tidak ada di daftar | Kabel atau driver; daftar dibaca saat mulai dan oleh tombol refresh | Colok ulang, tekan refresh, pasang driver CH340/FTDI |
| "Access to the port is denied" | Serial monitor atau program lain memegang port | Tutup program itu |
| Kartu kontrol tetap abu-abu | Belum terhubung | Tekan Connect |
| Tombol bereaksi setelah sekitar sedetik | Perintah tanpa newline, firmware menunggu timeout baca | Wajar (desain sistem, keterbatasan 4) |
| AUTO MODE tidak berbuat apa-apa | Firmware tidak punya handler `m` | Pakai mode PID |
| Grafik kosong setelah Send profile | Mode PID belum aktif | Tekan PID mode ON lebih dulu |
| Jendela tanpa tampilan Mica | Mica butuh Windows 11; Windows 10 memakai latar polos | Wajar, fitur lain tetap berjalan |
| `dotnet build` gagal dengan file `.g.cs` hilang | Restore dan build dalam satu perintah di checkout bersih | `dotnet restore CoffeeRoasto.slnx`, lalu `dotnet build --no-restore`, atau pakai `check.ps1` |

## Rilis
Naikkan `<Version>` di kedua `.csproj` dan `<AssemblyTitle>` aplikasi, jalankan `./check.ps1`, jalankan `./installer/publish.ps1`, buka `CoffeeRoasto.exe` yang baru dan jalani daftar pengecekan di atas. `installer/dist/`, `bin/`, dan `obj/` tidak ada di git. Installer ClickOnce v1.0 hanya disimpan sebagai rujukan di `installer/v1-clickonce/`.
