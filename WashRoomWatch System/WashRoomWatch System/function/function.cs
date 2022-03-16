using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashRoomWatch_System.function
{
    class function:Form1
    {
        public static object SqlData { get; private set; }

        public static SqlDataReader dataReader = null;
        public static void datagridfill(string q, DataGridView dgv)
        {
            try
            {
                connection.connection.DB();
                DataTable dt = new DataTable();
                SqlDataAdapter data = null;
                SqlCommand command = new SqlCommand(q, connection.connection.con);
                data = new SqlDataAdapter(command);
                data.Fill(dt);
                dgv.DataSource = dt;
               connection.connection.con.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
