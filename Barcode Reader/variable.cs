using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Barcode_Reader
{
    class variable
    {
        public string id_pegawai;
        public string nama;
        public string pangkat;
        public DateTime batas_absen = DateTime.Parse("08:00:00 AM");


        public string jam_absen = DateTime.Now.ToString("hh:mm:ss tt");
        public void constructor_variable()
        {
            
        }
    }
}
