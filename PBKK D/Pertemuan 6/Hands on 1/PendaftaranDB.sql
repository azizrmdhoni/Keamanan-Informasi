-- Buat database

CREATE DATABASE PendaftaranDB;
GO

USE PendaftaranDB;
GO
SELECT DB_NAME() AS DatabaseAktif;

-- Buat tabel ProgramStudi dan isi datanya
USE PendaftaranDB;
GO

CREATE TABLE ProgramStudi
(
    Id         INT IDENTITY(1,1)
               CONSTRAINT PK_ProgramStudi PRIMARY KEY,
    Kode       VARCHAR(10)  NOT NULL
               CONSTRAINT UQ_ProgramStudi_Kode UNIQUE,
    Nama       VARCHAR(100) NOT NULL,
    Aktif      BIT          NOT NULL
               CONSTRAINT DF_ProgramStudi_Aktif DEFAULT 1,
    DibuatPada DATETIME2    NOT NULL
               CONSTRAINT DF_ProgramStudi_Dibuat DEFAULT SYSDATETIME()
);
GO

INSERT INTO ProgramStudi (Kode, Nama) VALUES
('IF',  'Informatika'),
('SIS', 'Sistem Informasi'),
('MJ',  'Manajemen'),
('EL',  'Teknik Elektro'),
('DKV', 'Desain Komunikasi Visual');
GO

-- Buat tabel Mahasiswa (relasi 1:N)
USE PendaftaranDB;
GO

CREATE TABLE Mahasiswa
(
    Id             INT IDENTITY(1,1)
                   CONSTRAINT PK_Mahasiswa PRIMARY KEY,
    Nim            VARCHAR(20)  NOT NULL
                   CONSTRAINT UQ_Mahasiswa_Nim UNIQUE,
    Nama           VARCHAR(100) NOT NULL,
    ProgramStudiId INT          NOT NULL,
    TanggalLahir   DATE         NULL,
    Alamat         VARCHAR(250) NULL,
    NoTelepon      VARCHAR(20)  NULL,
    DibuatPada     DATETIME2    NOT NULL
                   CONSTRAINT DF_Mahasiswa_Dibuat DEFAULT SYSDATETIME(),
    DiubahPada     DATETIME2    NULL,
    CONSTRAINT FK_Mahasiswa_ProgramStudi
        FOREIGN KEY (ProgramStudiId) REFERENCES ProgramStudi(Id)
);
GO

INSERT INTO Mahasiswa (Nim, Nama, ProgramStudiId, TanggalLahir, Alamat, NoTelepon) VALUES
('25001', 'Arief Hidayat',   1, '2004-02-14', 'Jl. Ketintang No. 8, Surabaya',         '081210002001'),
('25002', 'Nadia Putri',     2, '2005-06-30', 'Jl. Gubeng Kertajaya No. 15, Surabaya', '082210002002'),
('25003', 'Yoga Firmansyah', 1, '2004-10-09', 'Jl. Rungkut Asri No. 22, Surabaya',     '081310002003'),
('25004', 'Laras Wulandari', 3, '2005-01-25', 'Jl. Darmo Permai No. 5, Surabaya',      '085610002004'),
('25005', 'Hendra Kusuma',   4, '2004-08-17', 'Jl. Keputih Tegal No. 3, Surabaya',     '081910002005');
GO

SELECT * FROM Mahasiswa;

-- Gabungkan dua tabel dengan JOIN
USE PendaftaranDB;
GO

SELECT m.Id, m.Nim, m.Nama, p.Kode, p.Nama AS Program, m.TanggalLahir, m.NoTelepon
FROM Mahasiswa m
INNER JOIN ProgramStudi p ON p.Id = m.ProgramStudiId;

-- Uji Foreign Key dan UNIQUE
USE PendaftaranDB;
GO

-- Uji Foreign Key: program dengan Id 99 tidak ada
INSERT INTO Mahasiswa (Nim, Nama, ProgramStudiId) VALUES ('25099', 'Uji FK', 99);
SELECT * FROM ProgramStudi;

-- Uji UNIQUE: NIM 25001 sudah dipakai
INSERT INTO Mahasiswa (Nim, Nama, ProgramStudiId) VALUES ('25001', 'Uji Duplikat', 1);

-- Query challenge

USE PendaftaranDB;
GO

-- 1. Tampilkan seluruh mahasiswa
SELECT *
FROM Mahasiswa
 
-- 2. Tampilkan mahasiswa Teknik Informatika:
SELECT m.Nim, m.Nama
FROM Mahasiswa m
JOIN ProgramStudi p ON p.Id = m.ProgramStudiId
WHERE p.Kode = 'IF';

-- 3. Hitung jumlah mahasiswa:
SELECT COUNT(*) AS TotalMahasiswa FROM Mahasiswa;

-- 4. Hitung mahasiswa berdasarkan program studi
SELECT p.Nama, COUNT(m.Id) AS Jumlah
FROM ProgramStudi p
LEFT JOIN Mahasiswa m ON m.ProgramStudiId = p.Id
GROUP BY p.Id, p.Nama;

-- Mini assignment
-- Tambahkan 3 mahasiswa baru
INSERT INTO Mahasiswa (Nim, Nama, ProgramStudiId, TanggalLahir, Alamat, NoTelepon) VALUES
('25006', 'Bima Saputra',  1, '2004-04-03', 'Jl. Manyar No. 12, Surabaya',  '081210002006'),
('25007', 'Citra Dewanti', 2, '2005-09-19', 'Jl. Mulyorejo No. 7, Surabaya', '082210002007'),
('25008', 'Dani Setiawan', 4, '2004-12-01', 'Jl. Sukolilo No. 30, Surabaya', '081310002008');
GO

-- Jalankan JOIN berurutan nama
SELECT m.Nim, m.Nama, p.Nama AS ProgramStudi, m.TanggalLahir, m.NoTelepon
FROM Mahasiswa m
INNER JOIN ProgramStudi p ON p.Id = m.ProgramStudiId
ORDER BY m.Nama;
