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
    public partial class Options : Form
    {
        
        public Options()
        {
            InitializeComponent();
        }

        public void Initializer(string id, string pass, string name,  string contact)
        {
            txtID.Text = id;
            txtPassword.Text = pass;
            txtName.Text = name;
            txtContact.Text = contact;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                connection.connection.DB();

                string str = "delete from UserInfo where ID = " + txtID.Text + "";
                SqlCommand command = new SqlCommand(str, connection.connection.con);
                command.ExecuteNonQuery();
                MessageBox.Show("Successfully Deleted");

                connection.connection.con.Close();
                
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Dispose();

            }
            
           
        }

         void btnModify_Click(object sender, EventArgs e)
        {
            try
            {

                connection.connection.DB();

                string input = "Update UserInfo set Pass ="+"'"+txtPassword.Text+"', Name = "+"'" + txtName.Text + "',Gender = "+ "'" + boxGender.Text + "',Contact = "+ "'" + txtContact.Text +"'where " +"ID = "+txtID.Text;
                //,Name ="+" '" + txtName.Text + "',Gender = "+ "'" + boxGender.Text + "',Contact = "+ "" + txtContact.Text + "
                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Successfully Modified");

                connection.connection.con.Close();
     
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        

        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            txtContact.ForeColor = Color.Black;
        }
    }
}
