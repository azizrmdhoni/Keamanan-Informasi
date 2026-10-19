using CampusRoster.Models;
using Microsoft.Data.SqlClient;

namespace CampusRoster.Data;

public class MahasiswaRepository
{
    private const string ConnStr =
        @"Server=(localdb)\MSSQLLocalDB;Database=CampusDB;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string KolomSelect = "SELECT Id, Nim, Nama, Jurusan, Gender, Email FROM Mahasiswa";

    public List<Mahasiswa> Cari(string? kataKunci = null)
    {
        using var conn = new SqlConnection(ConnStr);
        using var cmd = new SqlCommand(
            KolomSelect +
            " WHERE Nim LIKE @k OR Nama LIKE @k OR Jurusan LIKE @k ORDER BY Id DESC", conn);
        cmd.Parameters.AddWithValue("@k", $"%{kataKunci?.Trim()}%");

        conn.Open();
        using var reader = cmd.ExecuteReader();

        var hasil = new List<Mahasiswa>();
        while (reader.Read())
        {
            hasil.Add(new Mahasiswa
            {
                Id      = reader.GetInt32(0),
                Nim     = reader.GetString(1),
                Nama    = reader.GetString(2),
                Jurusan = reader.GetString(3),
                Gender  = reader.GetString(4),
                Email   = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }
        return hasil;
    }

    public void Tambah(Mahasiswa m)
    {
        const string sql =
            "INSERT INTO Mahasiswa (Nim, Nama, Jurusan, Gender, Email) " +
            "VALUES (@nim, @nama, @jurusan, @gender, @email)";
        Jalankan(sql, m, sertakanId: false);
    }

    public void Ubah(Mahasiswa m)
    {
        const string sql =
            "UPDATE Mahasiswa SET Nim = @nim, Nama = @nama, Jurusan = @jurusan, " +
            "Gender = @gender, Email = @email WHERE Id = @id";
        Jalankan(sql, m, sertakanId: true);
    }

    public void Hapus(int id)
    {
        using var conn = new SqlConnection(ConnStr);
        using var cmd = new SqlCommand("DELETE FROM Mahasiswa WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    private static void Jalankan(string sql, Mahasiswa m, bool sertakanId)
    {
        using var conn = new SqlConnection(ConnStr);
        using var cmd = new SqlCommand(sql, conn);

        if (sertakanId) cmd.Parameters.AddWithValue("@id", m.Id);
        cmd.Parameters.AddWithValue("@nim", m.Nim);
        cmd.Parameters.AddWithValue("@nama", m.Nama);
        cmd.Parameters.AddWithValue("@jurusan", m.Jurusan);
        cmd.Parameters.AddWithValue("@gender", m.Gender);
        cmd.Parameters.AddWithValue("@email", (object?)m.Email ?? DBNull.Value);

        conn.Open();
        cmd.ExecuteNonQuery();
    }
}
