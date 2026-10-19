using System.Collections.ObjectModel;
using System.Windows.Input;
using CampusRoster.Data;
using CampusRoster.Models;
using Microsoft.Data.SqlClient;

namespace CampusRoster.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly MahasiswaRepository _repo = new();
    private List<Mahasiswa> _semuaData = new();
    private int _idDiedit;

    // ---------- Data untuk tampilan ----------
    public ObservableCollection<Mahasiswa> Daftar { get; } = new();
    public IReadOnlyList<string> PilihanJurusan { get; } = new[] { "Informatika", "Sistem Informasi" };
    public IReadOnlyList<string> PilihanGender { get; } = new[] { "Laki-laki", "Perempuan" };

    // ---------- Field form ----------
    private string _nim = "";
    public string Nim { get => _nim; set => Set(ref _nim, value); }

    private string _nama = "";
    public string Nama { get => _nama; set => Set(ref _nama, value); }

    private string? _jurusan;
    public string? Jurusan { get => _jurusan; set => Set(ref _jurusan, value); }

    private string? _gender;
    public string? Gender { get => _gender; set => Set(ref _gender, value); }

    private string _email = "";
    public string Email { get => _email; set => Set(ref _email, value); }

    // ---------- Pencarian, pesan, mode ----------
    private string _kataKunci = "";
    public string KataKunci { get => _kataKunci; set => Set(ref _kataKunci, value); }

    private string _pesan = "";
    public string Pesan { get => _pesan; set => Set(ref _pesan, value); }

    public bool ModeEdit => _idDiedit != 0;
    public string TeksSimpan => ModeEdit ? "Perbarui" : "Simpan";

    // ---------- Baris terpilih di DataGrid ----------
    private Mahasiswa? _dipilih;
    public Mahasiswa? Dipilih
    {
        get => _dipilih;
        set
        {
            if (!Set(ref _dipilih, value) || value is null) return;
            IsiForm(value);
        }
    }

    // ---------- Statistik ----------
    public int Total => _semuaData.Count;
    public int JumlahInformatika => _semuaData.Count(m => m.Jurusan == "Informatika");
    public int JumlahSistemInformasi => _semuaData.Count(m => m.Jurusan == "Sistem Informasi");
    public int JumlahLaki => _semuaData.Count(m => m.Gender == "Laki-laki");
    public int JumlahPerempuan => _semuaData.Count(m => m.Gender == "Perempuan");

    // ---------- Command ----------
    public ICommand SimpanCommand { get; }
    public ICommand HapusCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand CariCommand { get; }

    public MainViewModel()
    {
        SimpanCommand = new RelayCommand(Simpan);
        HapusCommand  = new RelayCommand(Hapus, () => ModeEdit);
        ResetCommand  = new RelayCommand(() => { KosongkanForm(); Pesan = ""; });
        CariCommand   = new RelayCommand(MuatData);

        MuatData();
    }

    // ---------- Logika ----------
    private void MuatData()
    {
        try
        {
            _semuaData = _repo.Cari();
            var tampil = string.IsNullOrWhiteSpace(KataKunci) ? _semuaData : _repo.Cari(KataKunci);

            Daftar.Clear();
            foreach (var m in tampil) Daftar.Add(m);

            Raise(nameof(Total));
            Raise(nameof(JumlahInformatika));
            Raise(nameof(JumlahSistemInformasi));
            Raise(nameof(JumlahLaki));
            Raise(nameof(JumlahPerempuan));
        }
        catch (SqlException ex)
        {
            Pesan = "Gagal terhubung ke database: " + ex.Message;
        }
    }

    private void Simpan()
    {
        if (string.IsNullOrWhiteSpace(Nim) || string.IsNullOrWhiteSpace(Nama)
            || Jurusan is null || Gender is null)
        {
            Pesan = "NIM, nama, jurusan, dan gender wajib diisi.";
            return;
        }

        var data = new Mahasiswa
        {
            Id      = _idDiedit,
            Nim     = Nim.Trim(),
            Nama    = Nama.Trim(),
            Jurusan = Jurusan,
            Gender  = Gender,
            Email   = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim()
        };

        try
        {
            bool baru = data.Id == 0;
            if (baru) _repo.Tambah(data); else _repo.Ubah(data);

            MuatData();
            KosongkanForm();
            Pesan = baru ? "Data berhasil ditambahkan." : "Perubahan berhasil disimpan.";
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            Pesan = $"NIM {data.Nim} sudah terdaftar.";
        }
        catch (SqlException ex)
        {
            Pesan = "Gagal menyimpan: " + ex.Message;
        }
    }

    private void Hapus()
    {
        if (!ModeEdit) return;
        try
        {
            _repo.Hapus(_idDiedit);
            MuatData();
            KosongkanForm();
            Pesan = "Data berhasil dihapus.";
        }
        catch (SqlException ex)
        {
            Pesan = "Gagal menghapus: " + ex.Message;
        }
    }

    private void IsiForm(Mahasiswa m)
    {
        _idDiedit = m.Id;
        Nim = m.Nim;
        Nama = m.Nama;
        Jurusan = m.Jurusan;
        Gender = m.Gender;
        Email = m.Email ?? "";
        NotifikasiMode();
    }

    private void KosongkanForm()
    {
        _idDiedit = 0;
        Nim = Nama = Email = "";
        Jurusan = null;
        Gender = null;
        Dipilih = null;
        NotifikasiMode();
    }

    private void NotifikasiMode()
    {
        Raise(nameof(ModeEdit));
        Raise(nameof(TeksSimpan));
    }
}
