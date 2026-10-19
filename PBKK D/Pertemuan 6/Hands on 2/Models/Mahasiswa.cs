namespace PendaftaranMhs.Models;

public class Mahasiswa
{
    public int Id { get; set; }
    public string Nim { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public int ProgramStudiId { get; set; }
    public string NamaProgram { get; set; } = string.Empty;   // hasil JOIN
    public DateTime? TanggalLahir { get; set; }
    public string Alamat { get; set; } = string.Empty;
    public string NoTelepon { get; set; } = string.Empty;
    public DateTime DibuatPada { get; set; }
    public DateTime? DiubahPada { get; set; }
}
