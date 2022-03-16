using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashRoomWatch_System.connection
{
    class connection
    {
        public static SqlConnection con;
        private static string db = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\+Crazy+\C#\WashRoomWatch System\WashRoomWatch System\UserInfoDatabase.mdf;Integrated Security=True";
        public static void DB()
        {
            try
            {
                con = new SqlConnection(db);
                con.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        
    }
}
