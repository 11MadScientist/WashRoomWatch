using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashRoomWatch_System
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
           
            if (txtAdmincode.Text.CompareTo("El Psy Congroo") != 0)
            {
                MessageBox.Show("Incorrect Code", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (!Form1.isNumber(txtID.Text))
                {
                    MessageBox.Show("ID must be a digit");
                    txtID.ForeColor = Color.Red;

                    return;
                }


                connection.connection.DB();
                string input = "Insert into Administrator(ID,Pass,Name,Gender,Contact, occWashRoom, occRoom, status) values(" + txtID.Text + ",'" + txtPassword.Text + "','" + txtName.Text + "','" + boxGender.Text + "'," + txtContact.Text + ",'" + "None" + "','" + "None" + "','Admin')";
                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Registration Successful");

                connection.connection.con.Close();
                this.Dispose();

            }
            catch (Exception)
            {
                MessageBox.Show("Incomplete Data", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
