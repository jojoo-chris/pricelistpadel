using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace pricelistpadel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            dateTimePicker1.MinDate = DateTime.Today;

            radioButton1.CheckedChanged += (s, e) => UpdateTotal();
            radioButton2.CheckedChanged += (s, e) => UpdateTotal();
            checkBox1.CheckedChanged += (s, e) => UpdateTotal();
            checkBox2.CheckedChanged += (s, e) => UpdateTotal();
            comboBox1.SelectedIndexChanged += (s, e) => UpdateTotal();
            comboBox2.SelectedIndexChanged += (s, e) => UpdateTotal();

            button1.Click += Button1_Click;
        }

        private void UpdateTotal()
        {
            int total = 0;

            int jamMulai = GetJamFromCombo(comboBox1.Text);
            int jamSelesai = GetJamFromCombo(comboBox2.Text);
            int durasi = 0;

            if (jamMulai > 0 && jamSelesai > 0)
            {
                if (jamSelesai <= jamMulai)
                {
                    labeltotal.Text = "Total Pemesanan: Rp 0";
                    MessageBox.Show("Jam Selesai harus lebih besar dari Jam Mulai!",
                                    "Peringatan",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                durasi = jamSelesai - jamMulai;
            }

            if (radioButton1.Checked)
                total += durasi * 200000;
            if (radioButton2.Checked)
                total += durasi * 250000;

            if (checkBox1.Checked) total += 50000;
            if (checkBox2.Checked) total += 30000;

            labeltotal.Text = $"Total Pemesanan: Rp {total:N0}";
        }

        private int GetJamFromCombo(string jamText)
        {
            if (string.IsNullOrEmpty(jamText) || jamText.StartsWith("Jam")) return 0;
            if (jamText.Contains(".")) jamText = jamText.Replace(".", ":");
            if (DateTime.TryParse(jamText, out DateTime jam))
                return jam.Hour;
            return 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string nama = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(nama))
            {
                MessageBox.Show("Harap isi nama pemesan terlebih dahulu!",
                                "Peringatan",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Harap pilih jenis lapangan (Siang/Malam)!",
                                "Peringatan",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            int jamMulai = GetJamFromCombo(comboBox1.Text);
            int jamSelesai = GetJamFromCombo(comboBox2.Text);

            if (jamMulai == 0 || jamSelesai == 0)
            {
                MessageBox.Show("Harap pilih Jam Mulai dan Jam Selesai!",
                                "Peringatan",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // tanggal
            if (dateTimePicker1.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Tanggal pemesanan tidak boleh sebelum hari ini!",
                                "Peringatan",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            string tanggal = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string lapangan = radioButton1.Checked ? "Siang" :
                              radioButton2.Checked ? "Malam" : "-";
            string raket = checkBox1.Checked ? "Raket" : "";
            string bola = checkBox2.Checked ? "Bola" : "";

            string total = labeltotal.Text;

            string pesan = $"Halo Admin, saya mau pesan padel:\n\n" +
                           $"*Nama:* {nama}\n" +
                           $"*Tanggal:* {tanggal}\n" +
                           $"*Jam:* {jamMulai}:00 - {jamSelesai}:00\n" +
                           $"*Lapangan:* {lapangan}\n" +
                           $"*Alat:* {raket} {bola}\n" +
                           $"*{total}*";

            string noWA = "6281228201891";
            string url = $"https://wa.me/{noWA}?text={Uri.EscapeDataString(pesan)}";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membuka WhatsApp: " + ex.Message);
            }
        }
    }
}
