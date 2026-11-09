# ADR-001: Protokol serial satu karakter

- Status: Diterima
- Tanggal: 2026-11-09

## Konteks
GUI dan Arduino harus bertukar perintah, suhu, dan kejadian lewat satu koneksi serial USB. Kedua program kecil dan
dirawat oleh mahasiswa.

## Keputusan
Perintah dari GUI berupa satu karakter ASCII (`n`, `k`, `t`, `d`, `h`, `q`, `s` ...); angka dan baris profil dikirim
sebagai teks biasa. Arduino menjawab dengan frame teks `@<bean>A<drum>B<reference>C` dan kejadian satu huruf
(`X`, `V`, `Y`, `Z`). Semuanya didokumentasikan di `docs/id/serial-protocol.md`.

## Alasan
- Mudah di-parse di AVR dengan perbandingan `String`.
- Bisa diuji manual dari serial monitor Arduino.
- Tanpa framing, checksum, atau library di kedua sisi.

## Konsekuensi
Perintah tidak menjelaskan diri dan tidak ada deteksi error. Perintah baru butuh karakter bebas dan perubahan di
kedua sisi. Teks lain dibaca sebagai suhu, jadi salah ketik menjadi set point; perintah `m` di GUI saat ini jatuh ke
aturan ini.

## Alternatif yang ditolak
- JSON per baris: terbaca, tetapi berat untuk board kelas Uno dan tidak perlu untuk 15 perintah.
- Framing biner dengan checksum: kuat, tetapi lebih sulit di-debug oleh tim.
