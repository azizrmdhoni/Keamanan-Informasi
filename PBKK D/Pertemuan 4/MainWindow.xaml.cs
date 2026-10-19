using System.Windows;
using System.Windows.Controls;

namespace StudentRegistrationApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNim.Text))
            {
                MessageBox.Show("NIM wajib diisi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNim.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama lengkap wajib diisi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNama.Focus();
                return;
            }

            if (cmbProdi.SelectedItem == null)
            {
                MessageBox.Show("Silakan pilih program studi!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rbLaki.IsChecked != true && rbPerempuan.IsChecked != true)
            {
                MessageBox.Show("Silakan pilih jenis kelamin!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nim = txtNim.Text.Trim();
            string nama = txtNama.Text.Trim();
            string prodi = ((ComboBoxItem)cmbProdi.SelectedItem).Content.ToString();
            string jenisKelamin = rbLaki.IsChecked == true ? "Laki-laki" : "Perempuan";

            string dataFormatted = $"{nim}  •  {nama}  •  {prodi}  •  {jenisKelamin}";
            lstMahasiswa.Items.Add(dataFormatted);

            MessageBox.Show("Data mahasiswa berhasil ditambahkan!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

            ResetForm();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (lstMahasiswa.SelectedItem != null)
            {
                lstMahasiswa.Items.Remove(lstMahasiswa.SelectedItem);
                MessageBox.Show("Data berhasil dihapus dari daftar!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Pilih salah satu data di daftar terlebih dahulu untuk dihapus!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResetForm()
        {
            txtNim.Clear();
            txtNama.Clear();
            cmbProdi.SelectedIndex = -1;
            rbLaki.IsChecked = false;
            rbPerempuan.IsChecked = false;
            txtNim.Focus();
        }
    }
}
