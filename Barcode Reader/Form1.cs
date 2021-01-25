using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TouchlessLib;
using ZXing;
using ZXing.QrCode;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Data.OleDb;
using System.Globalization;

namespace Barcode_Reader
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCaptureDevices;
        private VideoCaptureDevice FinalVideo;
        private Bitmap bitmap;
        db_connection db = new db_connection();
        variable variable = new variable();
        string hari = DateTime.Today.ToString("dd/MM/yyyy");
        string query;
        //string jam = "08:00:00";

        public Form1()
        {
            InitializeComponent();
            comboBox1.Hide();
            timer1.Start();
        }

        private void start_webcam()
        {
            //menentukan camera device yang akan dipakai
            VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCaptureDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalVideo = new VideoCaptureDevice();

            //memilih camera device yang akan dipakai dan menjalankan camera
            FinalVideo = new VideoCaptureDevice(VideoCaptureDevices[comboBox1.SelectedIndex].MonikerString);
            FinalVideo.NewFrame += new NewFrameEventHandler(kamera_NewFrame);
            FinalVideo.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //DateTime batas_absen = DateTime.ParseExact(hari + " " + jam, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            start_webcam();
            txtbox_kode.Enabled = false;
            txtbox_nama.Enabled = false;
            txtbox_pangkat.Enabled = false;
            txtbox_keterangan.Enabled = false;
            DateTime jam = Convert.ToDateTime(variable.jam_absen);
            int t = TimeSpan.Compare(variable.batas_absen.TimeOfDay, jam.TimeOfDay);
            //MessageBox.Show(Convert.ToString(jam));
            listview_load();
        }

        void kamera_NewFrame(object sender, NewFrameEventArgs eventArg)
        {
            //menampilkan hasil rekaman ke picturebox
            bitmap = (Bitmap)eventArg.Frame.Clone();
            pictureBox1.Image = bitmap;
        }

        private void barcode_read()
        {
            //membaca qrcode
            var barcode_writer = new BarcodeWriter();
            barcode_writer.Format = BarcodeFormat.QR_CODE;
        }
        #region
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        private void txtbox_nama_TextChanged(object sender, EventArgs e)
        {

        }

        #endregion

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BarcodeReader Reader = new BarcodeReader();
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);
            try
            {
                MessageBox.Show(Convert.ToString(result));
            }
            catch (Exception ex)
            {

            }
        }

        private void pengosongan_textbox()
        {
            txtbox_pangkat.Clear();
            txtbox_nama.Clear();
            txtbox_kode.Clear();
            txtbox_keterangan.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //pengosongan_textbox();
            label_tanggal.Text = DateTime.Now.ToString("hh:mm:ss tt");
            //event timer untuk membaca qrcode setiap detik
            BarcodeReader Reader = new BarcodeReader();
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);
            try
            {
                string decoded = result.ToString().Trim();

                if (decoded != "")
                {
                    //DateTime batas_absen = DateTime.ParseExact(hari + " " + jam, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                    //timer1.Stop();
                    MessageBox.Show(decoded);
                    db.connect(Application.StartupPath);
                    string id = decoded;
                    //MessageBox.Show(id);
                    query = "SELECT id_pegawai, nama, pangkat FROM data_pegawai WHERE id_pegawai ='" + id + "';";
                    db.sql_reader(query);
                    while(db.db_read.Read())
                    {
                        variable.id_pegawai = db.db_read.GetValue(0).ToString();
                        variable.nama = db.db_read.GetString(1);
                        variable.pangkat = db.db_read.GetString(2);
                    }
                    db.con.Close();
                    ////pengecekkan apakah absen sudah diambil sebelumnya pada hari ini
                    //query = "SELECT jam_absen FROM absen WHERE id_pegawai = '"+ id +"';";
                    //db.con.Open();
                    //db.sql_reader(query);
                    //while (db.db_read.Read())
                    //{
                    //    variable.jam_absen = db.db_read.GetDateTime(0);
                    //}
                    //db.con.Close();
                    ////if (variable.jam_absen == Convert.ToDateTime("01/01/0001 00:00:00"))
                    ////{
                    ////    variable.jam_absen = Convert.ToDateTime("00/00/0000 00:00:00");
                    ////}
                    ////MessageBox.Show(Convert.ToString(variable.jam_absen));
                    if (id == variable.id_pegawai)
                    {
                        txtbox_kode.Text = variable.id_pegawai;
                        txtbox_nama.Text = variable.nama;
                        txtbox_pangkat.Text = variable.pangkat;
                        DateTime jam = Convert.ToDateTime(variable.jam_absen);
                        int t = TimeSpan.Compare(variable.batas_absen.TimeOfDay, jam.TimeOfDay);
                        if (t >= 0)
                        {
                            //penginputan data absen ke database
                            query = "INSERT INTO absen(id_pegawai, nama, jam_absen, keterangan) VALUES ('" + variable.id_pegawai + "', '" + variable.nama + "', '"
                                + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "', 'Tepat Waktu');";
                            db.con.Open();
                            db.sql_execution(query);
                            db.con.Close();
                            txtbox_kode.Text = variable.id_pegawai;
                            txtbox_nama.Text = variable.nama;
                            txtbox_pangkat.Text = variable.pangkat;
                            txtbox_keterangan.Text = "Tepat Waktu";
                        }
                        else
                        {
                            query = "INSERT INTO absen(id_pegawai, nama, jam_absen, keterangan) VALUES ('" + variable.id_pegawai + "', '" + variable.nama + "', '"
                                + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "', 'Terlambat');";
                            db.con.Open();
                            db.sql_execution(query);
                            db.con.Close();
                            txtbox_kode.Text = variable.id_pegawai;
                            txtbox_nama.Text = variable.nama;
                            txtbox_pangkat.Text = variable.pangkat;
                            txtbox_keterangan.Text = "Terlambat";
                        }
                        decoded = "";
                        listview_load();
                    }
                }
                else
                {
                    MessageBox.Show(Convert.ToString(result));
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void listview_load()
        {
            db.connect(Application.StartupPath);
            ListViewItem LV1;
            query = "SELECT id_pegawai, nama, jam_absen, keterangan FROM absen;";
            listView1.Items.Clear();
            //db.con.Open();
            db.sql_reader(query);
            while (db.db_read.Read())
            {
                LV1 = listView1.Items.Add(db.db_read["id_pegawai"].ToString());
                LV1.SubItems.Add(db.db_read["nama"].ToString());
                LV1.SubItems.Add(db.db_read["jam_absen"].ToString());
                LV1.SubItems.Add(db.db_read["keterangan"].ToString());
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //pemberhenti kamera setelah form diclose
            FinalVideo.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}