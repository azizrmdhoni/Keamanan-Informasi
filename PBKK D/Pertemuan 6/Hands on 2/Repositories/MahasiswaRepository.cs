using Microsoft.Data.SqlClient;
using PendaftaranMhs.Models;

namespace PendaftaranMhs.Repositories;

public class MahasiswaRepository
{
    // Samakan "Server=" dengan nama server yang Anda pakai di SSMS.
    private const string ConnStr =
        @"Server=.\SQLEXPRESS;Database=PendaftaranDB;Trusted_Connection=True;TrustServerCertificate=True;";

    /// <summary>Ambil mahasiswa. programStudiId = 0 berarti semua program.</summary>
    public List<Mahasiswa> AmbilSemua(int programStudiId = 0)
    {
        const string sql = """
            SELECT m.Id, m.Nim, m.Nama, m.ProgramStudiId, p.Nama,
                   m.TanggalLahir, m.Alamat, m.NoTelepon,
                   m.DibuatPada, m.DiubahPada
            FROM Mahasiswa m
            INNER JOIN ProgramStudi p ON p.Id = m.ProgramStudiId
            WHERE (@prodi = 0 OR m.ProgramStudiId = @prodi)
            ORDER BY m.Nama
            """;

        using var conn = new SqlConnection(ConnStr);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@prodi", programStudiId);

        conn.Open();
        using var r = cmd.ExecuteReader();

        var hasil = new List<Mahasiswa>();
        while (r.Read())
        {
            hasil.Add(new Mahasiswa
            {
                Id             = r.GetInt32(0),
                Nim            = r.GetString(1),
                Nama           = r.GetString(2),
                ProgramStudiId = r.GetInt32(3),
                NamaProgram    = r.GetString(4),
                TanggalLahir   = r.IsDBNull(5) ? null : r.GetDateTime(5),
                Alamat         = r.IsDBNull(6) ? "" : r.GetString(6),
                NoTelepon      = r.IsDBNull(7) ? "" : r.GetString(7),
                DibuatPada     = r.GetDateTime(8),
                DiubahPada     = r.IsDBNull(9) ? null : r.GetDateTime(9)
            });
        }
        return hasil;
    }

    public List<ProgramStudi> AmbilProgramAktif()
    {
        const string sql =
            "SELECT Id, Kode, Nama, Aktif FROM ProgramStudi WHERE Aktif = 1 ORDER BY Nama";

        using var conn = new SqlConnection(ConnStr);
        using var cmd = new SqlCommand(sql, conn);

        conn.Open();
        using var r = cmd.ExecuteReader();

        var hasil = new List<ProgramStudi>();
        while (r.Read())
        {
            hasil.Add(new ProgramStudi
            {
                Id    = r.GetInt32(0),
                Kode  = r.GetString(1),
                Nama  = r.GetString(2),
                Aktif = r.GetBoolean(3)
            });
        }
        return hasil;
    }
}
