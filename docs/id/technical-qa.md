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

## 6. Mengapa aplikasi membaca suhu dengan invariant culture?

Firmware mencetak titik (`37.11`). v1 memakai `Convert.ToDouble(...) / 100`, yang hanya menghasilkan 37,11 di PC berlokal Indonesia (titik dibaca sebagai pemisah ribuan, 3711) dan menghasilkan 0,37 di tempat lain. v2.0 membaca teksnya dengan `CultureInfo.InvariantCulture`, sehingga semua PC mendapat angka yang sama, dan satu tes unit menjalankan parser di tiga culture.

## 7. Mengapa WPF dengan WPF-UI di .NET 10?

Tampilan Fluent sungguhan (kartu, Mica, tema terang dan gelap) datang bersama WPF-UI; meniru semuanya di Windows Forms berarti menggambar tiap kontrol sendiri. .NET 10 didukung bertahun-tahun, high-DPI-nya benar, dan bisa dipublikasikan sebagai satu file self-contained. Kontrol `Chart` v1 tidak ada di sana, jadi grafiknya memakai ScottPlot. Kompromi: hanya untuk Windows dan file 74 MB (ADR-006).

## 8. Mengapa Excel untuk profil dan ekspor?

Penyangrai sudah menyimpan profil dan log di spreadsheet. ExcelDataReader membaca `.xls` dan `.xlsx` tanpa Excel terpasang; ekspor menulis `.xlsx` dengan ClosedXML, juga tanpa Excel. Ekspor CSV bisa ditambahkan nanti.

## 9. Apakah sistem ini aman?

Ada beberapa lapis: relay default mati, batas 250 °C, perintah `s`, dan berakhirnya mode heating karena waktu. Ini bukan sistem keselamatan tersertifikasi: tidak ada pemutus suhu lebih berbasis hardware di repositori dan firmware bergantung pada hubungan PC untuk `s`. Anggap sebagai prototipe dan tetap awasi mesin.

## 10. Apa yang terjadi pada `Release.rar`, `UpgradeLog*.htm`, dan `.pfx` v1?

Itu sisa Visual Studio: laporan upgrade proyek, arsip build, dan sertifikat penandatanganan ClickOnce sementara. Tidak ada yang dibutuhkan untuk build dan tidak ada lagi di git; salinan lokalnya ada di folder `archive/` yang diabaikan.

## 11. Apa perbaikan berikutnya?
- Perbaiki PID (pakai error sebelumnya yang sebenarnya dan `KD`) dan tuning ulang di mesin
- Tambahkan newline pada tiap perintah, atau ganti firmware ke `Serial.read()` (keterbatasan 4)
- Hapus atau implementasikan AUTO MODE
- Tombol batal untuk pengiriman profil
- Catat roasting (tanggal, biji, berat) di samping ekspor suhu
- Penanda fase roasting (drying, first crack) di grafik
