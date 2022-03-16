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
    public partial class Form1 : Form
    {
        Dashboard dash;
        string status;
        public Form1()
        {
            
            InitializeComponent();
        }
        public int getID()
        {
            return int.Parse(txtID.Text);
        }
        public string getStatus()
        {
            return status;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnSignin_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            
            f2.Show();
        }

        public static Boolean isNumber(string a)
        {
            try
            {
                Int32.Parse(a);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Input!");
                return false;
            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (!isNumber(txtID.Text))
            {
                txtID.Clear();
                txtPassword.Clear();

                return;
            }
            int id = Int32.Parse(txtID.Text);
            string pass = txtPassword.Text;
            string Pass;


            try
            {
                connection.connection.DB();
                string str = "select * from UserInfo where ID = "+id+"";
                SqlCommand command = new SqlCommand(str,connection.connection.con);
                function.function.dataReader = command.ExecuteReader();

                if (function.function.dataReader.Read())
                {
                    status = function.function.dataReader.GetString(7);
                    Pass = function.function.dataReader.GetString(1);

                    if (Pass.CompareTo(pass) == 0)
                    {
                        Pass = function.function.dataReader.GetString(7);
                        if (string.Compare(Pass, "Block") == 0)
                        {
                            MessageBox.Show("You are Banned from this System","ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtID.Clear();
                            txtPassword.Clear();
                            return;
                        }
                        MessageBox.Show("Logged In Successfully", "WELCOME", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        txtPassword.Clear();
                        this.Hide();
                        dash = new Dashboard(this);
                        dash.userRestrict();
                        dash.Show();
                        

                    }
                    else
                    {
                        MessageBox.Show("Incorrect Password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }

                }
                else
                {
                    MessageBox.Show("Log In Failed: ID not Found!");
                    txtID.Focus();
                    txtID.Clear();
                    txtPassword.Clear();
                }
                
                

                

                connection.connection.con.Close();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            connection.connection.DB();
            string str = "select * from Administrator where ID ='"+txtID.Text+"' ";
            SqlCommand cmd = new SqlCommand(str, connection.connection.con);
            function.function.dataReader = cmd.ExecuteReader();
            if (function.function.dataReader.Read())
            {
                status = function.function.dataReader.GetString(7);
                if (string.Compare(txtPassword.Text, function.function.dataReader.GetString(1)) == 0)
                {
                    MessageBox.Show("Logged In Successfully", "WELCOME", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.Hide();
                    txtPassword.Clear();
                    dash = new Dashboard(this);
                    dash.adminRestrict();
                    dash.Show();
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
        }

        private void TxtID_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
               
                txtID.Focus();
                errorProvider1.SetError(txtID, "ID is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtID, null);
            }
        }

        private void TxtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                
                txtID.Focus();
                errorProvider1.SetError(txtPassword, "Password is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtPassword, null);
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
                e.Handled = true;

            }
        }

        private void TxtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
                e.Handled = true;

            }
        }
    }
}
