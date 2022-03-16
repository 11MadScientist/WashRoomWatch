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
    public partial class Form2 : Form
    {
        public Form2()
        {
           
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
        public void Clear()
        {
            txtID.Clear();
            txtName.Clear();
            txtPassword.Clear();
            
            txtContact.Clear();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            

            try
            {
                if (!Form1.isNumber(txtID.Text))
                {
                    MessageBox.Show("ID must be a digit");
                    txtID.ForeColor = Color.Red;                    

                    return;
                }
                if (boxGender.Text.CompareTo("") == 0)
                {
                    MessageBox.Show("Blank Gender");
                    return;
                }
                

                connection.connection.DB();
                string input = "Insert into UserInfo(ID,Pass,Name,Gender,Contact, occWashRoom, occRoom, Status) values("+txtID.Text+",'"+txtPassword.Text+"','"+txtName.Text+"','"+boxGender.Text+"',"+txtContact.Text+",'"+"None"+ "','" + "None" + "','Inactive')";
                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Registration Successful");
               
                connection.connection.con.Close();
                this.Dispose();

            }
            catch (Exception)
            {
                if(txtID.Text.CompareTo("") != 0)
                    MessageBox.Show("[INVALID] data duplication", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                MessageBox.Show("Invalid Data", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            txtID.ForeColor = Color.Black;
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            txtContact.ForeColor = Color.Black;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Dispose();

        }

        private void TxtID_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider1.SetError(txtID, "ID is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtID, null);
            }

            try
            { Int32.Parse(txtID.Text); }
            catch (Exception)
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider1.SetError(txtID, "ID should be a number");
            }
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                e.Cancel = true;
                txtPassword.Focus();
                errorProvider1.SetError(txtPassword, "Password is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtPassword, null);
            }
        }

        private void TxtName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                e.Cancel = true;
                txtName.Focus();
                errorProvider1.SetError(txtName, "User Name is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtName, null);
            }
        }

        private void BoxGender_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(boxGender.Text))
            {
                e.Cancel = true;
                boxGender.Focus();
                errorProvider1.SetError(boxGender, "Gender is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(boxGender, null);
            }
        }

        private void TxtContact_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtContact.Text))
            {
                e.Cancel = true;
                txtContact.Focus();
                errorProvider1.SetError(txtContact, "Contact Number/address is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtContact, null);
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

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtName.Focus();
                e.Handled = true;

            }
        }

        private void TxtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                boxGender.Focus();
                e.Handled = true;

            }
        }

       

        private void BoxGender_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtContact.Focus();
                e.Handled = true;

            }
        }

        private void TxtContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
               
                e.Handled = true;
                return;

            }
        }
    }
}
