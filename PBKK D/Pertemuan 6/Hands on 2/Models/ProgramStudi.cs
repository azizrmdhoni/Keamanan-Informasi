namespace PendaftaranMhs.Models;

public class ProgramStudi
{
    public int Id { get; set; }
    public string Kode { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public bool Aktif { get; set; }
}
