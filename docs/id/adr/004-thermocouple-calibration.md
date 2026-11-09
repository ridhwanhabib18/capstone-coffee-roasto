# ADR-004: Rata-rata termokopel, koreksi regresi, dan dead band

- Status: Diterima
- Tanggal: 2026-11-09

## Konteks
Pembacaan MAX6675 berukuran 12-bit (langkah 0,25 °C), berderau, dan bergeser dari suhu sebenarnya.

## Keputusan
Ambil empat sampel berjarak 1 detik, rata-ratakan, koreksi tiap kanal dengan regresi linear yang diukur terhadap
termometer referensi (biji = (T − 3,4921) / 0,9194, drum = (T − 6,8969) / 0,8711), dan pertahankan nilai sebelumnya
bila perubahan di bawah 0,25 °C.

## Alasan
- Rata-rata mengurangi derau dengan harga periode pembaruan 4 detik, dapat diterima untuk sistem termal yang lambat.
- Regresi menghilangkan offset sistematis probe yang terpasang.
- Dead band mencegah tampilan dan PID bereaksi pada kedipan satu langkah.

## Konsekuensi
Mengganti probe atau penguat butuh regresi baru. PID hanya melihat pembacaan tiap ~4 detik, jadi tiap baris profil
adalah satu langkah 4 detik. Konstanta ada di `Config.h`; koefisien sebelumnya (biji = (T + 0,9845) / 0,9655, drum = (T − 3,6176) / 0,8963) ada di riwayat git.

## Alternatif yang ditolak
- Pembacaan mentah: terlalu berderau untuk kontroler.
- Rata-rata bergerak dengan langkah lebih pendek: lebih cepat, tetapi butuh RAM lebih banyak dan peninjauan ulang
  timing PID.
