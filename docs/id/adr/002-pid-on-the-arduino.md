# ADR-002: Kontrol PID berjalan di Arduino

- Status: Diterima
- Tanggal: 2026-11-09

## Konteks
Mesin harus menjaga suhu biji sesuai profil. Loop bisa berada di aplikasi PC atau di board.

## Keputusan
PID dan semua aturan keselamatan berjalan di firmware. GUI hanya mengirim mode (`h`, `q`, `s`), set point, dan
menampilkan frame yang diterima.

## Alasan
- Periode sampling 1 detik yang stabil; timer Windows bukan real-time.
- Keselamatan tidak bergantung pada PC: heating berakhir setelah 600 detik dan PID berhenti di 250 °C sendiri.
- GUI yang tertutup atau macet tidak membuat loop berjalan buta (selain mode terakhir).

## Konsekuensi
Tuning berarti mengunggah ulang firmware. Gain (Kp 21,095, Ki 0,2, Kd 30) diambil dari simulasi MATLAB dan menjadi
konstanta di sketsa. Kode saat ini tidak memakai Kd dan tidak memperbarui error sebelumnya (desain sistem,
keterbatasan 1). Bila kabel dicabut, board mempertahankan mode terakhir; tidak ada watchdog.

## Alternatif yang ditolak
- PID di C# dengan board hanya sebagai I/O: lebih mudah di-tuning, tetapi keselamatan hilang saat PC hang.
