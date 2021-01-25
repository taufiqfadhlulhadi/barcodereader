using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Barcode_Reader
{
    class db_connection
    {
        public OleDbConnection con = new OleDbConnection();
        public OleDbCommand db_com = new OleDbCommand();
        public OleDbDataReader db_read;
        public string error_message;
        
        public int connect (string path)
        {
            //oledb connector dan connection string
            try
            {
                con.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" 
                    + path + "\\barcode_db.accdb;Persist Security Info=False;";
                con.Open();
                return 1;
            }
            catch(Exception ex)
            {

                return 0;
            }
        }
        public void sql_execution(string query)
        {
            //fungsi eksekusi query database
            try
            {
                db_com = new OleDbCommand(query, con);
                //con.Open();
                db_com.Connection = con;
                db_com.ExecuteNonQuery();
                con.Close();
                //MessageBox.Show("insert selesai");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }
        public void sql_reader(string query)
        {
            //fungsi reader database
            try
            {
                db_com = new OleDbCommand(query);
                db_com.Connection = con;
                //MessageBox.Show(query);
                db_read = db_com.ExecuteReader();
                //MessageBox.Show("read selesai");
            }
            catch (Exception ex)
            {
                error_message = "Ada error di reader";
                MessageBox.Show(Convert.ToString(ex));
            }
        }
    }
}
