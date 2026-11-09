# Tanya jawab teknis Coffee Roasto

Pertanyaan dan alasan di balik keputusan teknis Coffee Roasto, untuk rujukan dan sidang capstone. Fakta diambil dari kode di repositori ini.

## 1. Mengapa PID ada di Arduino, bukan di GUI?

Loop kontrol butuh periode yang stabil dan harus tetap jalan bila PC lambat, sibuk, atau dicabut. Arduino mengambil sampel tiap detik dan langsung bertindak; PC hanya mengirim set point dan menampilkan hasil. Timer Windows bukan real-time dan kabel serial bisa tercabut (ADR-002).

## 2. Mengapa perintah satu karakter lewat serial?

Kedua sisi adalah program kecil buatan mahasiswa. Satu karakter mudah di-parse di AVR (`String == "k"`), mudah diuji dari serial monitor Arduino, dan tidak butuh framing. Kekurangannya: perintah tidak menjelaskan diri, jadi semuanya didaftar di [protokol serial](serial-protocol.md) (ADR-001).

## 3. Bagaimana suhu diukur dan dikoreksi?

Dua termokopel tipe K lewat penguat MAX6675 (12-bit, langkah 0,25 °C). Tiap pembacaan adalah rata-rata 4 sampel berjarak 1 detik, dikoreksi dengan regresi linear terhadap termometer referensi (biji = (T − 3,4921) / 0,9194, drum = (T − 6,8969) / 0,8711), dan diabaikan bila berubah kurang dari 0,25 °C (ADR-004).

## 4. Apa arti nilai PID?

Kp = 21,095, Ki = 0,2, Kd = 30 berasal dari simulasi MATLAB mesin. Keluaran dibatasi 0–40 karena itu rentang servo valve gas dalam derajat: 0° terbuka, 40° tertutup. Error besar (PID = 40) menutup valve dan menyalakan relay kompor; tanpa error (PID = 0) valve terbuka dan kompor mati. Di firmware saat ini variabel error sebelumnya tidak pernah diperbarui dan Kd tidak dipakai, sehingga loop lebih mirip P dengan suku I kecil (desain sistem, keterbatasan 1).

## 5. Mengapa profil dikirim sepuluh nilai tiap 40 detik?

Arduino memakai array tetap (`Array_suhu[15]`) dan buffer serial 64 byte, dan memakai satu set point per pembacaan rata-rata (~4 detik). Sepuluh nilai habis dalam ~40 detik, jadi GUI mengirim kelompok berikutnya tepat saat yang terakhir terpakai. Firmware menyimpan buffer kedua agar baris baru tidak menimpa nilai yang masih dipakai. Mengirim seluruh profil sekaligus akan melebihi keduanya.

## 6. Mengapa GUI membagi pembacaan dengan 100?

Bukan untuk mengubah skala: di PC berlokal Indonesia `Convert.ToDouble("37.11")` membaca titik sebagai pemisah ribuan dan menghasilkan 3711, sehingga pembagian mengembalikan 37,11. Di PC berlokal Inggris kode yang sama menghasilkan 0,37. Perbaikan yang kuat: `double.Parse(s, CultureInfo.InvariantCulture)` tanpa pembagian.

## 7. Mengapa Windows Forms di .NET Framework 4.7.2?

PC mesin berbasis Windows, kontrol grafik (`System.Windows.Forms.DataVisualization`), `SerialPort`, dan ClickOnce sudah bawaan, dan tidak ada runtime yang perlu dipasang di Windows 10 atau 11. Kompromi: aplikasi hanya untuk Windows dan Excel Interop butuh Excel (ADR-005).

## 8. Mengapa Excel untuk profil dan ekspor?

Penyangrai sudah menyimpan profil dan log di spreadsheet. ExcelDataReader membaca `.xls` dan `.xlsx` tanpa Excel terpasang; ekspor memakai Excel Interop dan memang butuh Excel. Ekspor CSV akan menghapus ketergantungan itu.

## 9. Apakah sistem ini aman?

Ada beberapa lapis: relay default mati, batas 250 °C, perintah `s`, dan berakhirnya mode heating karena waktu. Ini bukan sistem keselamatan tersertifikasi: tidak ada pemutus suhu lebih berbasis hardware di repositori dan firmware bergantung pada hubungan PC untuk `s`. Anggap sebagai prototipe dan tetap awasi mesin.

## 10. Apa arti "Release.rar", "UpgradeLog*.htm", dan `.pfx` di repositori?

`UpgradeLog*.htm` adalah laporan upgrade proyek Visual Studio, `Release.rar` adalah arsip build, dan `CoffeeRoasto_GUI_TemporaryKey.pfx` adalah sertifikat sementara yang dibuat Visual Studio untuk penandatanganan ClickOnce. Tidak ada yang dibutuhkan untuk build; `.gitignore` proyek sudah mengecualikan hasil build.

## 11. Apa perbaikan berikutnya?
- Perbaiki PID (perbarui `errorx`, pakai Kd) dan tuning ulang di mesin
- Parse dengan invariant culture dan hapus pembagian
- Tambahkan newline pada tiap perintah, atau ganti firmware ke `Serial.read()`
- Alihkan semua pembaruan lampu dan pesan ke thread UI
- Hapus atau implementasikan AUTO MODE
- Catat roasting (tanggal, biji, berat) dan ekspor CSV
