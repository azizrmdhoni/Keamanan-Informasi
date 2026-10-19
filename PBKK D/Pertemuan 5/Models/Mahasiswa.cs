namespace CampusRoster.Models;

public class Mahasiswa
{
    public int Id { get; set; }
    public string Nim { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string Jurusan { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? Email { get; set; }
}
