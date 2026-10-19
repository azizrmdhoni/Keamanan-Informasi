using System.Windows;
using System.Windows.Controls;
using PendaftaranMhs.Models;
using PendaftaranMhs.Repositories;

namespace PendaftaranMhs;

public partial class MainWindow : Window
{
    private readonly MahasiswaRepository _repo = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => MuatAwal();
    }

    private void MuatAwal()
    {
        try
        {
            var prodi = _repo.AmbilProgramAktif();
            prodi.Insert(0, new ProgramStudi { Id = 0, Nama = "Semua program studi" });

            CmbProdi.ItemsSource = prodi;
            CmbProdi.SelectedIndex = 0;   // memicu SelectionChanged -> MuatMahasiswa()
        }
        catch (Exception ex)
        {
            TampilkanError(ex);
        }
    }

    private void MuatMahasiswa()
    {
        try
        {
            int prodiId = (CmbProdi.SelectedItem as ProgramStudi)?.Id ?? 0;
            var data = _repo.AmbilSemua(prodiId);

            GridMahasiswa.ItemsSource = data;
            TxtStatus.Text = $"{data.Count} mahasiswa ditampilkan";
        }
        catch (Exception ex)
        {
            TampilkanError(ex);
        }
    }

    private void TampilkanError(Exception ex)
    {
        TxtStatus.Text = "Gagal mengambil data.";
        MessageBox.Show(
            "Tidak dapat mengambil data dari SQL Server.\n\n" + ex.Message,
            "Kesalahan Database", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void CmbProdi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => MuatMahasiswa();

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        => MuatMahasiswa();
}
