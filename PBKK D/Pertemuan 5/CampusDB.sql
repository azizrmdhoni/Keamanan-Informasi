CREATE DATABASE CampusDB;
GO
USE CampusDB;
GO

CREATE TABLE Mahasiswa
(
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nim     VARCHAR(20)  NOT NULL UNIQUE,
    Nama    VARCHAR(100) NOT NULL,
    Jurusan VARCHAR(100) NOT NULL,
    Gender  VARCHAR(20)  NOT NULL,
    Email   VARCHAR(100) NULL
);
GO

INSERT INTO Mahasiswa (Nim, Nama, Jurusan, Gender, Email) VALUES
('24101', 'Raka Pratama',   'Informatika',      'Laki-laki', 'raka@mail.com'),
('24102', 'Dewi Lestari',   'Sistem Informasi', 'Perempuan', 'dewi@mail.com'),
('24103', 'Fajar Nugroho',  'Informatika',      'Laki-laki', 'fajar@mail.com'),
('24104', 'Maya Anggraini', 'Sistem Informasi', 'Perempuan', 'maya@mail.com');
