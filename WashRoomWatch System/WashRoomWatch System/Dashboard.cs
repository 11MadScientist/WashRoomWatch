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
    public partial class Dashboard : Form
    {
        private int userID;
        private string userName;
        private string userGender;
        private string userContact;
        private string occWashRoom;
        private string occRoom;
        private string userStatus;
        private string[] Friends;
        private string[] FriendRequests;
        Form1 x;
        public Dashboard(Form1 form)
        {
            x = form;
            userID = x.getID();
            userStatus = x.getStatus();


            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            pnlGender.Hide();
            boxArrow.Hide();
            string str;





            if (sender == this || sender == btnModify || sender == btnUserEdit)
            {


                try
                {
                    connection.connection.DB();
                    str = "update UserInfo set Status = 'Active' where ID ='" + userID + "'";
                    SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                    cmd.ExecuteNonQuery();
                    connection.connection.con.Close();

                    connection.connection.DB();
                    if (userStatus.CompareTo("Admin") == 0)
                        str = "select * from Administrator where ID = '" + userID + "'";
                    else
                        str = "select * from UserInfo where ID ='" + userID + "'";
                    SqlCommand command = new SqlCommand(str, connection.connection.con);
                    function.function.dataReader = command.ExecuteReader();

                    if (function.function.dataReader.Read())
                    {
                        userName = function.function.dataReader.GetString(2);
                        userGender = function.function.dataReader.GetString(3);
                        userContact = function.function.dataReader.GetString(4);
                        occWashRoom = function.function.dataReader.GetString(5);
                        occRoom = function.function.dataReader.GetString(6);
                        userStatus = function.function.dataReader.GetString(7);



                        lblUser.Text = "WELCOME " + userName;
                        lblUser.TextAlign = ContentAlignment.MiddleCenter;
                        lblUserName.Text = userName;

                        lblGender.Text = userGender;
                        lblGender.TextAlign = ContentAlignment.MiddleCenter;

                        if (string.Compare(userGender, "Male") == 0)
                        {
                            boxUserFemale.Hide();
                            boxUserMale.Show();
                        }
                        else
                        {
                            boxUserMale.Hide();
                            boxUserFemale.Show();
                        }

                        lblUserContact.Text = userContact;


                        lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;

                        lblUserStatus.Text = userStatus;

                        if (userStatus.CompareTo("Active") == 0)
                            btnUserStatus.BackColor = Color.ForestGreen;

                        else if (userStatus.CompareTo("Admin") == 0)
                            btnUserStatus.BackColor = Color.Teal;
                        else
                            btnUserStatus.BackColor = Color.Red;



                    }

                    connection.connection.con.Close();




                }
                catch (Exception) { }
            }



            if (sender == lblWashRoom || sender == chckAll)
            {
                connection.connection.DB();

                if (string.Compare(userGender, "Male") == 0)
                    str = "Select RoomName from Male";
                else
                    str = "select RoomName from Female";

                function.function.datagridfill(str, dataRooms);
                connection.connection.con.Close();


            }
            else if (sender == chckAvailable)
            {
                if (string.Compare(userGender, "Male") == 0)
                    str = "Select RoomName from Male where Availability ='" + "Available" + "'";
                else
                    str = "Select RoomName from Female where Availability ='" + "Available" + "'";

                function.function.datagridfill(str, dataRooms);
                connection.connection.con.Close();
            }
            if (sender == btnRequestHelp || sender == lblNotif || sender == dataNotification)
            {
                connection.connection.DB();
                str = "select * from Notifications";
                function.function.datagridfill(str, dataNotification);

            }
            if (sender == btnAddNewRoom || sender == btnRoomMod || sender == btnRoomDel || sender == btnMaintenance)
            {

                connection.connection.DB();

                if (genderTracker == true)
                {
                    str = "Select * from Male";
                    function.function.datagridfill(str, dataGridView2);
                }

                else
                {
                    str = "select * from Female";
                    function.function.datagridfill(str, dataGridView3);
                }



                connection.connection.con.Close();
                pnlGender.Show();
            }

            else
            {
                connection.connection.DB();
                str = "Select ID, Pass, Name, Gender, Contact, occWashroom, occRoom, Status  from UserInfo";
                function.function.datagridfill(str, dataGridView1);
                connection.connection.con.Close();

                connection.connection.DB();
                str = "Select ID, Pass, Name, Gender, Contact, occWashroom, occRoom, Status from Administrator";
                function.function.datagridfill(str, dataAdmin);
                connection.connection.con.Close();

                connection.connection.DB();
                str = "select * from Male";
                function.function.datagridfill(str, dataGridView2);
                connection.connection.con.Close();

                connection.connection.DB();
                str = "select * from Female";
                function.function.datagridfill(str, dataGridView3);
                connection.connection.con.Close();

                //for the piechart
                int male = 0;
                int female = 0;
                double percentage = 0;
                //Male
                connection.connection.DB();
                str = "select * from UserInfo where Gender = 'Male'";
                SqlCommand command = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = command.ExecuteReader();

                while (function.function.dataReader.Read())
                {
                    male++;
                }
                connection.connection.con.Close();
                //Female
                connection.connection.DB();
                str = "select * from UserInfo where Gender = 'Female'";
                command = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = command.ExecuteReader();

                while (function.function.dataReader.Read())
                {
                    female++;
                }
                connection.connection.con.Close();
                //Percentage
                connection.connection.DB();
                str = "select * from UserInfo where Status = 'Active'";
                command = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = command.ExecuteReader();
                while (function.function.dataReader.Read())
                    percentage++;
                connection.connection.con.Close();

                percentage = (percentage / (male + female) * 100);
                lblTotalPopulation.Text = "Population: " + (male + female);
                lblActive.Text = string.Format("Active: {0:F2}%", percentage);

                foreach (var series in piePopulation.Series)
                {
                    series.Points.Clear();
                }
                piePopulation.Titles.Clear();
                piePopulation.Titles.Add("WashRoom Population");

                piePopulation.Series["s1"].Points.AddXY("Male", (male + ""));
                piePopulation.Series["s1"].Points.AddXY("Female", (female + ""));

                // userListDashboard

                connection.connection.DB();
                str = "select Name, ID from UserInfo where ID <>" + userID + "";
                function.function.datagridfill(str, dataUserList);
                connection.connection.con.Close();

                //Announcements datas update
                connection.connection.DB();
                str = "select * from Announcements order by Code Desc";
                function.function.datagridfill(str, dataAnnouncements);
                connection.connection.con.Close();

                connection.connection.DB();
                str = "select * from Announcements order by Code Desc";
                function.function.datagridfill(str, datarRemoveAnnouncements);
                connection.connection.con.Close();






            }





        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void lblTracker_Click(object sender, EventArgs e)
        {

        }

        private void lblSheet_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {



        }

        private void lblData_Click(object sender, EventArgs e)
        {
            Dashboard_Load(sender, e);


        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void lblGender_Click(object sender, EventArgs e)
        {

        }

        private void lblData_Click_1(object sender, EventArgs e)
        {
            hide();
            lblData.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 494);

            dataGridView1.Visible = true;
            dataAdmin.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            pnldata.Show();

            dataGridView1.Visible = true;
            dataAdmin.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;


            lblUserData.Size = new Size(115, 32);
            lblAdminData.Size = new Size(115, 18);
            lblUserData.BackColor = Color.FromArgb(125, 40, 50);
            lblUserSlider.ForeColor = Color.Black;
            dataTracker = true;
            Dashboard_Load(sender, e);
            pnlDataSheet.Show();

            //setting back the buttons 
            lblMale.Size = new Size(115, 32);
            lblFemale.Size = new Size(115, 18);
            lblMale.BackColor = Color.FromArgb(125, 40, 50);
            lblUserSlider.ForeColor = Color.Black;
            genderTracker = true;

            dataGridView1.Show();
            pnlGender.Hide();
            lblUserSlider.Size = new Size(115, 32);
            lblWashRoomSlider.Size = new Size(115, 18);
            lblUserSlider.ForeColor = Color.Black;
            lblWashRoomSlider.ForeColor = Color.Teal;
            btnAddRoom.Hide();


        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_2(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click_1(object sender, EventArgs e)
        {

        }

        private void lblMessages_Click(object sender, EventArgs e)
        {

        }



        private void lblNotif_Click(object sender, EventArgs e)
        {
            hide();

            lblNotif.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 442);
            pnlNotification.Show();
            Dashboard_Load(sender, e);

            radAll.Checked = true;
            
           

        }
        public void userRestrict()
        {
            lblNotif.Visible = false;
            lblData.Visible = false;
            lblAnnouncement.Visible = false;
            pictureBox2.Visible = false;
            pictureBox4.Visible = false;
            pictureBox10.Visible = false;
        }
        public void adminRestrict()
        {
            btnSendRequest.Visible = false;
            pictureBox8.Visible = false;
            lblFriends.Visible = false;
            pictureBox11.Visible = false;
            pnlReport.Visible = false;
            lblReport.Visible = false;
            pnlMessageOptions.Hide();
            pnlViewMessage.Hide();
            pictureBox20.Visible = false;



        }

        private void lblTracker_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void lblDashboard_Click(object sender, EventArgs e)
        {
            hide();
            lblDashboard.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 285);
            pnlDashboard.Show();
            dataGridView1.Visible = true;
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {


        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Visible == true)
                {
                    connection.connection.DB();
                    string str = "select ID, Pass, Name, Gender, Contact, occWashroom, occRoom, Status from UserInfo where ID like" + "'%" + txtSearch.Text + "%' or Name like" + "'%" + txtSearch.Text + "%'";
                    function.function.datagridfill(str, dataGridView1);
                    connection.connection.con.Close();
                }
                else if (dataAdmin.Visible == true)
                {
                    connection.connection.DB();
                    string str = "select ID, Pass, Name, Gender, Contact, occWashroom, occRoom, Status from Administrator where ID like" + "'%" + txtSearch.Text + "%' or Name like" + "'%" + txtSearch.Text + "%'";
                    function.function.datagridfill(str, dataAdmin);
                    connection.connection.con.Close();
                }
                else if (dataGridView2.Visible == true)
                {
                    connection.connection.DB();
                    string str = "select * from Male where RoomName like" + "'%" + txtSearch.Text + "%' or Location like" + "'%" + txtSearch.Text + "%'";
                    function.function.datagridfill(str, dataGridView2);
                    connection.connection.con.Close();
                }
                else
                {
                    connection.connection.DB();
                    string str = "select * from Female where RoomName like" + "'%" + txtSearch.Text + "%' or Location like" + "'%" + txtSearch.Text + "%'";
                    function.function.datagridfill(str, dataGridView3);
                    connection.connection.con.Close();
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.connection.con.Close();
            }
        }

        private void lblWashRoom_Click(object sender, EventArgs e)
        {
            hide();

            boxRoom1.BackColor = Color.LimeGreen;
            //Image Change
            boxRoom1.Image = WashRoomWatch_System.Properties.Resources._13316;

            boxRoom2.BackColor = Color.LimeGreen;
            //Image Change
            boxRoom2.Image = WashRoomWatch_System.Properties.Resources._13316;

            boxRoom3.BackColor = Color.LimeGreen;
            //Image Change
            boxRoom3.Image = WashRoomWatch_System.Properties.Resources._13316;



            Dashboard_Load(sender, e);
            lblWashRoom.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 336);
            pnlWashRoom.Show();

            boxPaperPing.Hide();
            pnlRequestHelp.Hide();
        }

        public void hide()
        {
            pnlDataSheet.Hide();
            pnlWashRoom.Hide();
            pnlDashboard.Hide();
            pnlMessages.Hide();
            pnlFriends.Hide();
            pnlNotification.Hide();
            pnlAnnouncement.Hide();
            Options.Hide();
            pnlAddRoom.Hide();
            pnlMod.Hide();
            pnlEditProfile.Hide();
            pnlReport.Hide();
            lblDashboard.ForeColor = Color.DimGray;
            lblData.ForeColor = Color.DimGray;
            lblMessages.ForeColor = Color.DimGray;
            lblFriends.ForeColor = Color.DimGray;
            lblNotif.ForeColor = Color.DimGray;
            lblWashRoom.ForeColor = Color.DimGray;
            
        }

        private void pnlCondition_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Dashboard_Load(sender, e);
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            string gender = "";
            string room = "";
            if (dataTracker == true)
            {
                gender = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                room = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            }
              
            else
            {
                gender = dataAdmin.CurrentRow.Cells[3].Value.ToString();
                room = dataAdmin.CurrentRow.Cells[5].Value.ToString();
            }
            

            MessageBox.Show(gender+" "+boxGender.Text);

            if (string.Compare(gender, boxGender.Text) != 0 && string.Compare(room, "None") != 0)
            {
                MessageBox.Show("User is still using toilet, changing the Gender is Invalid", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {

                connection.connection.DB();
                string status = txtUserStatus.Text;
                if (string.Compare(status, "Unblock") == 0 || status.Equals(""))
                {
                    status = "Inactive";
                }
                

                string input;
                if (dataTracker == true)
                    input = "Update UserInfo set Pass =" + "'" + txtPassword.Text + "', Name = " + "'" + txtName.Text + "',Gender = " + "'" + boxGender.Text + "',Contact = " + "'" + txtContact.Text + "', Status = '" + status + "' where " + "ID = " + txtID.Text;
                else
                    input = "Update Administrator set Pass =" + "'" + txtPassword.Text + "', Name = " + "'" + txtName.Text + "',Gender = " + "'" + boxGender.Text + "',Contact = " + "'" + txtContact.Text + "', Status = '" + status + "' where " + "ID = " + txtID.Text;

                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Successfully Modified", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                Options.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string str;
                connection.connection.DB();
                if (dataTracker == false)
                    str = "delete from Administrator where ID = " + txtID.Text + "";
                else
                    str = "delete from UserInfo where ID = " + txtID.Text + "";

                SqlCommand command = new SqlCommand(str, connection.connection.con);
                command.ExecuteNonQuery();
                MessageBox.Show("Successfully Deleted", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                Options.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Dispose();

            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Options.Hide();
        }

        private void btnBacktoRooms_Click(object sender, EventArgs e)
        {


        }

        private void btnAddNewRoom_Click(object sender, EventArgs e)
        {




        }

        private void btnAddRoom_Click(object sender, EventArgs e)
        {


        }



        private void pnlWashRoom_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblUser_Click(object sender, EventArgs e)
        {

        }
        private string roomName;
        private void dataRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {



        }

        

        private void btnRoomMod_Click(object sender, EventArgs e)
        {
            string input;

            if (txtRoomLocation.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Invalid null Input", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                connection.connection.DB();
                if (string.Compare(userGender, "Male") == 0)
                    input = "Update Male set Location = " + "'" + txtRoomLocation.Text + "'where " + "RoomName = '" + txtmodRoomName.Text + "'";

                else
                    input = "update Female set Location = '" + txtRoomLocation.Text + "'where RoomName = '" + txtmodRoomName.Text + "'";

                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Successfully Modified", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                pnlMod.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnRoomDel_Click(object sender, EventArgs e)
        {
            string str;

            try
            {
                connection.connection.DB();

                if (string.Compare(userGender, "Male") == 0)
                    str = "delete from Male where RoomName = '" + txtmodRoomName.Text + "'";

                else
                    str = "delete from Female where RoomName = '" + txtmodRoomName.Text + "'";

                SqlCommand command = new SqlCommand(str, connection.connection.con);
                command.ExecuteNonQuery();
                MessageBox.Show("Successfully Deleted", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                pnlMod.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }
        }

        private void btnRoomBack_Click(object sender, EventArgs e)
        {
            pnlMod.Hide();
        }
        public void isFull()
        {
            string str;
            connection.connection.DB();
            if (string.Compare(userGender, "Male") == 0)
                str = "update Male set Availability = 'Full' where Room1 <> 'None' and Room2 <> 'None' and Room3 <> 'None'";

            else
                str = "update Female set Availability= 'Full' where Room1 <> 'None' and Room2 <> 'None' and Room3 <> 'None'";

            SqlCommand cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();

        }
        public void notFull()
        {
            string str;
            connection.connection.DB();
            if (string.Compare(userGender, "Male") == 0)
                str = "update Male set Availability = 'Available' where Availability = 'Full' and (Room1 = 'None' or Room2 = 'None' or Room3 = 'None')";

            else
                str = "update Female set Availability = 'Available' where Availability = 'Full' and (Room1 = 'None' or Room2 = 'None' or Room3 = 'None')";

            SqlCommand cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
        }

        private void boxRoom1_Click(object sender, EventArgs e)
        {

            string cubicle = "Room1";
            if (roomName == null)
                return;

            try
            {
                if (occWashRoom.CompareTo("None") == 0 && boxRoom1.BackColor == Color.LimeGreen)
                {
                    //Image Change
                    boxRoom1.Image = WashRoomWatch_System.Properties.Resources.occupied;
                    //data manipulation
                    occWashRoom = roomName;
                    occRoom = cubicle;
                    boxRoom1.BackColor = Color.Red;
                    boxPaperPing.Show();
                    string str;
                    connection.connection.DB();

                    if (string.Compare(userGender, "Male") == 0)
                        str = "Update Male set Room1 = " + "'" + userName + "'where " + "RoomName = '" + roomName + "'";

                    else
                        str = "Update Female set Room1 = " + "'" + userName + "'where " + "RoomName = '" + roomName + "'";



                    SqlCommand com = new SqlCommand(str, connection.connection.con);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Enjoy the Comfort!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    connection.connection.con.Close();

                    //the username database update part
                    connection.connection.DB();

                    if (userStatus.CompareTo("Admin") == 0)
                        str = "update Administrator set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";
                    else

                        str = "update UserInfo set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";

                    com = new SqlCommand(str, connection.connection.con);
                    com.ExecuteNonQuery();

                    connection.connection.con.Close();

                    isFull();
                    lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;

                }

                else if (occWashRoom.CompareTo(roomName) == 0 && occRoom.CompareTo(cubicle) == 0)
                {
                    //Image Change
                    boxRoom1.Image = WashRoomWatch_System.Properties.Resources._13316;
                    //data manipulation
                    occWashRoom = "None";
                    occRoom = "None";
                    boxRoom1.BackColor = Color.LimeGreen;
                    boxPaperPing.Hide();
                    string str;
                    connection.connection.DB();

                    if (string.Compare(userGender, "Male") == 0)
                        str = "Update Male set Room1 = 'None' where RoomName = '" + roomName + "'";

                    else
                        str = "Update Female set Room1 = 'None' where RoomName = '" + roomName + "'";

                    SqlCommand com = new SqlCommand(str, connection.connection.con);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Please Observe Cleanliness before you Leave!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    connection.connection.con.Close();

                    //the username database update part
                    connection.connection.DB();
                    if (userStatus.CompareTo("Admin") == 0)
                        str = "update Administrator set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";
                    else

                        str = "update UserInfo set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";

                    com = new SqlCommand(str, connection.connection.con);
                    com.ExecuteNonQuery();

                    connection.connection.con.Close();
                    notFull();
                    lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;
                }
                else if (string.Compare(occWashRoom, "None") == 0 && boxRoom1.BackColor == Color.Red)
                {
                    MessageBox.Show("This cubicle is Occupied", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    MessageBox.Show("Cannot Occupy multiple Cubicle!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }








            }
            catch (Exception) { }

        }

        private void boxRoom3_Click(object sender, EventArgs e)
        {
            string cubicle = "Room3";
            if (roomName == null)
                return;
            try
            {
                if (occWashRoom.CompareTo("None") == 0 && boxRoom3.BackColor == Color.LimeGreen)
                {
                    //Image Change
                    boxRoom3.Image = WashRoomWatch_System.Properties.Resources.occupied;
                    //data manipulation
                    occWashRoom = roomName;
                    occRoom = cubicle;
                    boxRoom3.BackColor = Color.Red;
                    boxPaperPing.Show();
                    string str;

                    connection.connection.DB();

                    if (string.Compare(userGender, "Male") == 0)
                        str = "Update Male set Room3 = " + "'" + userName + "'where " + "RoomName = '" + roomName + "'";

                    else
                        str = "Update Female set Room3 = " + "'" + userName + "'where " + "RoomName = '" + roomName + "'";


                    SqlCommand com = new SqlCommand(str, connection.connection.con);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Enjoy the Comfort!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    connection.connection.con.Close();
                    //the username database update part
                    connection.connection.DB();

                    if (userStatus.CompareTo("Admin") == 0)
                        str = "update Administrator set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";
                    else

                        str = "update UserInfo set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";

                    com = new SqlCommand(str, connection.connection.con);
                    com.ExecuteNonQuery();
                    isFull();
                    lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;

                }
                else if (occWashRoom.CompareTo(roomName) == 0 && occRoom.CompareTo(cubicle) == 0)
                {
                    //Image Change
                    boxRoom3.Image = WashRoomWatch_System.Properties.Resources._13316;
                    //data manipulation
                    string str;
                    occWashRoom = "None";
                    occRoom = "None";
                    boxRoom3.BackColor = Color.LimeGreen;
                    boxPaperPing.Hide();

                    connection.connection.DB();

                    if (string.Compare(userGender, "Male") == 0)
                        str = "Update Male set Room3 = 'None' where RoomName = '" + roomName + "'";

                    else
                        str = "Update Female set Room3 = 'None' where RoomName = '" + roomName + "'";


                    SqlCommand com = new SqlCommand(str, connection.connection.con);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Please Observe Cleanliness before you Leave!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    connection.connection.con.Close();
                    //====================
                    connection.connection.DB();

                    if (userStatus.CompareTo("Admin") == 0)
                        str = "update Administrator set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";
                    else

                        str = "update UserInfo set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";

                    com = new SqlCommand(str, connection.connection.con);
                    com.ExecuteNonQuery();
                    connection.connection.con.Close();
                    notFull();
                    lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;
                }
                else if (string.Compare(occWashRoom, "None") == 0 && boxRoom2.BackColor == Color.Red)
                {
                    MessageBox.Show("This cubicle is Occupied", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    MessageBox.Show("Cannot Occupy multiple Cubicle!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception) { }
        }

        private void boxRoom2_Click(object sender, EventArgs e)
        {
            string cubicle = "Room2";
            if (roomName == null)
                return;


            try
            {
                if (occWashRoom.CompareTo("None") == 0 && boxRoom2.BackColor == Color.LimeGreen)
                {
                    //Image Change
                    boxRoom2.Image = WashRoomWatch_System.Properties.Resources.occupied;
                    //data manipulation
                    occWashRoom = roomName;
                    occRoom = cubicle;
                    boxRoom2.BackColor = Color.Red;
                    boxPaperPing.Show();
                    string str;
                    connection.connection.DB();

                    if (string.Compare(userGender, "Male") == 0)
                        str = "Update Male set Room2 = " + "'" + userName + "'where " + "RoomName = '" + roomName + "'";

                    else
                        str = "Update Female set Room2 = " + "'" + userName + "'where " + "RoomName = '" + roomName + "'";


                    SqlCommand com = new SqlCommand(str, connection.connection.con);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Enjoy the Comfort!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    connection.connection.con.Close();

                    //the username database update part
                    connection.connection.DB();

                    if (userStatus.CompareTo("Admin") == 0)
                        str = "update Administrator set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";
                    else

                        str = "update UserInfo set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";

                    com = new SqlCommand(str, connection.connection.con);
                    com.ExecuteNonQuery();
                    connection.connection.con.Close();
                    isFull();
                    lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;
                }
                else if (occWashRoom.CompareTo(roomName) == 0 && occRoom.CompareTo(cubicle) == 0)
                {
                    //Image Change
                    boxRoom2.Image = WashRoomWatch_System.Properties.Resources._13316;
                    //data manipulation
                    string str;
                    occWashRoom = "None";
                    occRoom = "None";
                    boxRoom2.BackColor = Color.LimeGreen;
                    boxPaperPing.Hide();

                    connection.connection.DB();


                    if (string.Compare(userGender, "Male") == 0)
                        str = "Update Male set Room2 = 'None' where RoomName = '" + roomName + "'";

                    else
                        str = "Update Female set Room2 = 'None' where RoomName = '" + roomName + "'";


                    SqlCommand com = new SqlCommand(str, connection.connection.con);

                    com.ExecuteNonQuery();
                    MessageBox.Show("Please Observe Cleanliness before you Leave!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    connection.connection.con.Close();
                    //=============================
                    connection.connection.DB();

                    if (userStatus.CompareTo("Admin") == 0)
                        str = "update Administrator set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";
                    else

                        str = "update UserInfo set occWashRoom =" + "'" + occWashRoom + "', occRoom = " + "'" + occRoom + "'where ID = '" + userID + "'";

                    com = new SqlCommand(str, connection.connection.con);
                    com.ExecuteNonQuery();
                    connection.connection.con.Close();
                    notFull();
                    lblOccupied.Text = "WashRoom: " + occWashRoom + " " + occRoom;
                }
                else if (string.Compare(occWashRoom, "None") == 0 && boxRoom2.BackColor == Color.Red)
                {
                    MessageBox.Show("This cubicle is Occupied", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Cannot Occupy multiple Cubicle!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception) { }
        }

        private void dataRooms_Click(object sender, EventArgs e)
        {

        }

        private void pnlCondition_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void dataRooms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lblLocation.Visible = false;

            if (occWashRoom == dataRooms.CurrentRow.Cells[0].Value + "")
                boxPaperPing.Show();
            else
                boxPaperPing.Hide();
            string str;

            connection.connection.DB();
            roomName = dataRooms.CurrentRow.Cells[0].Value + "";
            if (string.Compare(userGender, "Male") == 0)
                str = "select * from Male where RoomName ='" + roomName + "'";
            else
                str = "select * from Female where RoomName = '" + roomName + "'";

            SqlCommand command = new SqlCommand(str, connection.connection.con);

            function.function.dataReader = command.ExecuteReader();
            try
            {
                if (function.function.dataReader.Read())
                {
                    string avail = function.function.dataReader.GetString(2);
                    string Room1 = function.function.dataReader.GetString(3);
                    string Room2 = function.function.dataReader.GetString(4);
                    string Room3 = function.function.dataReader.GetString(5);


                    if (avail.CompareTo("Available") == 0 || avail.CompareTo("Full") == 0)
                    {
                        pnlCondition.BackColor = Color.DimGray;
                        faucets.BackColor = Color.LimeGreen;
                        door.BackColor = Color.LimeGreen;

                        if (Room1.CompareTo("None") == 0)
                        {
                            boxRoom1.Enabled = true;
                            boxRoom1.BackColor = Color.LimeGreen;
                            //Image Change
                            boxRoom1.Image = WashRoomWatch_System.Properties.Resources._13316;
                        }
                        else if (Room1.CompareTo("Maintenance") == 0)
                        {
                            boxRoom1.BackColor = Color.Yellow;
                            boxRoom1.Enabled = false;
                            //Image Change
                            boxRoom1.Image = WashRoomWatch_System.Properties.Resources.broken;
                        }

                        else
                        {
                            boxRoom1.BackColor = Color.Red;
                            //Image Change
                            boxRoom1.Image = WashRoomWatch_System.Properties.Resources.occupied;
                        }


                        if (Room2.CompareTo("None") == 0)
                        {
                            boxRoom2.Enabled = true;
                            boxRoom2.BackColor = Color.LimeGreen;
                            //Image Change
                            boxRoom2.Image = WashRoomWatch_System.Properties.Resources._13316;
                        }
                        else if (Room2.CompareTo("Maintenance") == 0)
                        {
                            boxRoom2.Enabled = false;
                            boxRoom2.BackColor = Color.Yellow;
                            //Image Change
                            boxRoom2.Image = WashRoomWatch_System.Properties.Resources.broken;
                        }
                        else
                        {
                            boxRoom2.BackColor = Color.Red;
                            //Image Change
                            boxRoom2.Image = WashRoomWatch_System.Properties.Resources.occupied;
                        }



                        if (Room3.CompareTo("None") == 0)
                        {
                            boxRoom3.Enabled = true;
                            boxRoom3.BackColor = Color.LimeGreen;
                            //Image Change
                            boxRoom3.Image = WashRoomWatch_System.Properties.Resources._13316;
                        }
                        else if (Room3.CompareTo("Maintenance") == 0)
                        {
                            boxRoom3.Enabled = false;
                            boxRoom3.BackColor = Color.Yellow;
                            //Image Change
                            boxRoom3.Image = WashRoomWatch_System.Properties.Resources.broken;
                        }

                        else
                        {
                            boxRoom3.BackColor = Color.Red;
                            //Image Change
                            boxRoom3.Image = WashRoomWatch_System.Properties.Resources.occupied;
                        }

                    }

                    else if (string.Compare(avail, "Maintenance") == 0)
                    {

                        boxRoom1.BackColor = Color.Yellow;
                        boxRoom2.BackColor = Color.Yellow;
                        boxRoom3.BackColor = Color.Yellow;
                        faucets.BackColor = Color.Yellow;
                        door.BackColor = Color.Yellow;
                        boxRoom1.Image = WashRoomWatch_System.Properties.Resources.broken;
                        boxRoom2.Image = WashRoomWatch_System.Properties.Resources.broken;
                        boxRoom3.Image = WashRoomWatch_System.Properties.Resources.broken;
                    }
                    connection.connection.con.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                connection.connection.con.Close();
            }
        }

        private void btnMaintenance_Click(object sender, EventArgs e)
        {

        }

        private void btnMaintenance_Click_1(object sender, EventArgs e)
        {


            if (string.Compare(btnMaintenance.Text, "Maintenance") == 0)
            {
                door.BackColor = Color.Yellow;
                faucets.BackColor = Color.Yellow;
                boxRoom1.BackColor = Color.Yellow;
                boxRoom2.BackColor = Color.Yellow;
                boxRoom3.BackColor = Color.Yellow;
                btnMaintenance.Text = "Undo";
                boxRoom1.Enabled = false;
                boxRoom2.Enabled = false;
                boxRoom3.Enabled = false;

                try
                {
                    string str;


                    connection.connection.DB();

                    if (string.Compare(userGender, "Male") == 0)
                        str = "update from Male set Availability ='" + "Maintenance" + "'Room1 ='" + "Maintenance" + "', Room2 = '" + "Maintenance" + "', Room3 = '" + "Maintenance" + "'where RoomName ='" + roomName + "'";

                    else
                        str = "update from Female set Availability ='" + "Maintenance" + "'Room1 ='" + "Maintenance" + "', Room2 = '" + "Maintenance" + "', Room3 = '" + "Maintenance" + "'where RoomName ='" + roomName + "'";

                    SqlCommand command = new SqlCommand(str, connection.connection.con);
                    command.ExecuteNonQuery();
                    connection.connection.con.Close();



                }
                catch (Exception) { }
            }

            else
            {
                try
                {
                    string str;
                    connection.connection.DB();
                    if (string.Compare(userGender, "Male") == 0)
                        str = "update from Male set Availability ='" + "Available" + "'Room1 ='" + "None" + "', Room2 = '" + "None" + "', Room3 = '" + "None" + "'where RoomName ='" + roomName + "'";

                    else
                        str = "update from Male set Availability ='" + "Available" + "'Room1 ='" + "None" + "', Room2 = '" + "None" + "', Room3 = '" + "None" + "'where RoomName ='" + roomName + "'";


                    SqlCommand command = new SqlCommand(str, connection.connection.con);
                    command.ExecuteNonQuery();
                    connection.connection.con.Close();
                }
                catch (Exception) { }

                door.BackColor = Color.LimeGreen;
                faucets.BackColor = Color.LimeGreen;
                boxRoom1.BackColor = Color.LimeGreen;
                boxRoom2.BackColor = Color.LimeGreen;
                boxRoom3.BackColor = Color.LimeGreen;

                btnMaintenance.Text = "Maintenance";
                boxRoom1.Enabled = true;
                boxRoom2.Enabled = true;
                boxRoom3.Enabled = true;

            }
            pnlMod.Hide();



        }

        private void lblUserSlider_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            pnlGender.Hide();
            pnldata.Show();
            lblUserSlider.Size = new Size(115, 32);
            lblWashRoomSlider.Size = new Size(115, 18);
            lblUserSlider.ForeColor = Color.Black;
            lblWashRoomSlider.ForeColor = Color.Teal;
            btnAddRoom.Hide();
        }

        private void lblWashRoomSlider_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView2.Visible = true;
            dataGridView3.Visible = false;
            pnlGender.Show();
            pnldata.Hide();
            lblWashRoomSlider.Size = new Size(115, 32);
            lblUserSlider.Size = new Size(115, 18);
            lblWashRoomSlider.ForeColor = Color.Black;
            lblUserSlider.ForeColor = Color.Teal;
            btnAddRoom.Show();

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private bool genderTracker = true;
        private void lblMale_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataAdmin.Visible = false;
            dataGridView2.Visible = true;
            dataGridView3.Visible = false;
            dataGridView2.Show();
            dataGridView1.Hide();

            lblMale.Size = new Size(115, 32);
            lblFemale.Size = new Size(115, 18);
            lblMale.BackColor = Color.FromArgb(125, 40, 50);

            genderTracker = true;

        }

        private void lblFemale_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataAdmin.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = true;


            lblFemale.Size = new Size(115, 32);
            lblMale.Size = new Size(115, 18);


            genderTracker = false;
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView3_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnlDashboard_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnRoomDel_Click_1(object sender, EventArgs e)
        {


            string room1, room2, room3;
            if (genderTracker == true)
            {

                room1 = dataGridView2.CurrentRow.Cells[3].Value.ToString();
                room2 = dataGridView2.CurrentRow.Cells[4].Value.ToString();
                room3 = dataGridView2.CurrentRow.Cells[5].Value.ToString();


                if ((room1.CompareTo("None") != 0 && room1.CompareTo("Maintenance") != 0))
                {
                    MessageBox.Show("Room 1 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if ((room2.CompareTo("None") != 0 && room2.CompareTo("Maintenance") != 0))
                {
                    MessageBox.Show("Room 2 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cmboRoom2.SelectedIndex = -1;
                    return;
                }



                if ((room3.CompareTo("None") != 0 && room3.CompareTo("Maintenance") != 0))
                {
                    MessageBox.Show("Room 3 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cmboRoom3.SelectedIndex = -1;
                    return;
                }

            }
            else
            {
                room1 = dataGridView3.CurrentRow.Cells[3].Value.ToString();
                room2 = dataGridView3.CurrentRow.Cells[4].Value.ToString();
                room3 = dataGridView3.CurrentRow.Cells[5].Value.ToString();

                if ((room1.CompareTo("None") != 0 && room1.CompareTo("Maintenance") != 0) && cmboRoom1.Text.CompareTo("Maintenance") == 0)
                {
                    MessageBox.Show("Room 1 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if ((room2.CompareTo("None") != 0 && room2.CompareTo("Maintenance") != 0) && cmboRoom2.Text.CompareTo("Maintenance") == 0)
                {
                    MessageBox.Show("Room 2 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cmboRoom2.SelectedIndex = -1;
                    return;
                }



                if ((room3.CompareTo("None") != 0 && room3.CompareTo("Maintenance") != 0) && cmboRoom3.Text.CompareTo("Maintenance") == 0)
                {
                    MessageBox.Show("Room 3 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cmboRoom3.SelectedIndex = -1;
                    return;
                }
            }

            string str;

            try
            {
                connection.connection.DB();

                if (genderTracker)
                    str = "delete from Male where RoomName = '" + txtmodRoomName.Text + "'";

                else
                    str = "delete from Female where RoomName = '" + txtmodRoomName.Text + "'";

                SqlCommand command = new SqlCommand(str, connection.connection.con);
                command.ExecuteNonQuery();
                MessageBox.Show("Successfully Deleted", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                pnlMod.Hide();
                cmboRoom1.SelectedIndex = -1;
                cmboRoom2.SelectedIndex = -1;
                cmboRoom3.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }


        }

        private void btnRoomMod_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtModRoomLocation.Text))
            {
                MessageBox.Show("Room Location is empty", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string room1, room2, room3;
            string input;
            if (genderTracker == true)
            {
                room1 = dataGridView2.CurrentRow.Cells[3].Value.ToString();
                room2 = dataGridView2.CurrentRow.Cells[4].Value.ToString();
                room3 = dataGridView2.CurrentRow.Cells[5].Value.ToString();
            }
            else
            {
                room1 = dataGridView3.CurrentRow.Cells[3].Value.ToString();
                room2 = dataGridView3.CurrentRow.Cells[4].Value.ToString();
                room3 = dataGridView3.CurrentRow.Cells[5].Value.ToString();
            }
            if ((room1.CompareTo("None") != 0 && room1.CompareTo("Maintenance")!=0) && cmboRoom1.Text.CompareTo("Maintenance") == 0)
            {
                MessageBox.Show("Room 1 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            else  
                room1 = cmboRoom1.Text;
            

            if ((room2.CompareTo("None") != 0 && room2.CompareTo("Maintenance") != 0) && cmboRoom2.Text.CompareTo("Maintenance") == 0)
            {
                MessageBox.Show("Room 2 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmboRoom2.SelectedIndex = -1;
                return;
            }
            else 
                room2 = cmboRoom2.Text;
          

            if ((room3.CompareTo("None") != 0 && room3.CompareTo("Maintenance") != 0) && cmboRoom3.Text.CompareTo("Maintenance") == 0)
            {
                MessageBox.Show("Room 3 is currently in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmboRoom3.SelectedIndex = -1;
                return;
            }
            else 
                room3 = cmboRoom3.Text;
           

            try
            {
                connection.connection.DB();
                if (genderTracker == true)
                    input = "Update Male set Location = " + "'" + txtModRoomLocation.Text + "', Room1 = '" + room1 + "', Room2 = '" + room2 + "', Room3 = '" + room3 + "'where " + "RoomName = '" + txtmodRoomName.Text + "'";

                else
                    input = "Update Female set Location = " + "'" + txtModRoomLocation.Text + "', Room1 = '" + cmboRoom1.Text + "', Room2 = '" + cmboRoom2.Text + "', Room3 = '" + cmboRoom3.Text + "'where " + "RoomName = '" + txtmodRoomName.Text + "'";

                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Successfully Modified", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                pnlMod.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Dashboard_Load(sender, e);
        }

        private void btnRoomBack_Click_1(object sender, EventArgs e)
        {
            pnlMod.Hide();
            cmboRoom1.SelectedIndex = -1;
            cmboRoom2.SelectedIndex = -1;
            cmboRoom3.SelectedIndex = -1;
        }

        private void btnMaintenance_Click_2(object sender, EventArgs e)
        {

            if (string.Compare(btnMaintenance.Text, "Maintenance") == 0)
            {
                for (int i = 3; i <= 5; i++)
                {
                    if (genderTracker == true)
                    {
                        if (dataGridView2.CurrentRow.Cells[i].Value.ToString().CompareTo("None") != 0 && dataGridView2.CurrentRow.Cells[i].Value.ToString().CompareTo("Maintenance") != 0)
                        {
                            MessageBox.Show(dataGridView3.CurrentRow.Cells[i].Value.ToString().Equals("Maintenance") + "");
                            MessageBox.Show(dataGridView3.CurrentRow.Cells[i].Value+"   "+("Maintenance") + "");
                            MessageBox.Show("There is still a cubicle in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                       
                        if (dataGridView3.CurrentRow.Cells[i].Value.ToString().CompareTo("None") != 0 && dataGridView3.CurrentRow.Cells[i].Value.ToString().CompareTo("Maintenance") != 0)
                        {
                            MessageBox.Show("There is still a cubicle in use", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                door.BackColor = Color.Yellow;
                faucets.BackColor = Color.Yellow;
                boxRoom1.BackColor = Color.Yellow;
                boxRoom2.BackColor = Color.Yellow;
                boxRoom3.BackColor = Color.Yellow;
                btnMaintenance.Text = "Available";
                boxRoom1.Enabled = false;
                boxRoom2.Enabled = false;
                boxRoom3.Enabled = false;

                try
                {
                    string str;


                    connection.connection.DB();

                    if (genderTracker == true)
                        str = "Update Male set Availability = " + "'" + "Maintenance" + "'where " + "RoomName = '" + roomName + "'";


                    else
                        str = "Update Female set Availability = " + "'" + "Maintenance" + "'where " + "RoomName = '" + roomName + "'";

                    SqlCommand command = new SqlCommand(str, connection.connection.con);
                    command.ExecuteNonQuery();
                    connection.connection.con.Close();



                }
                catch (Exception) { }
            }

            else
            {
                try
                {

                    string str;
                    connection.connection.DB();
                    if (genderTracker == true)
                        str = "Update Male set Availability = " + "'" + "Available" + "'where " + "RoomName = '" + roomName + "'";


                    else
                        str = "Update Female set Availability = " + "'" + "Available" + "'where " + "RoomName = '" + roomName + "'";
                    SqlCommand command = new SqlCommand(str, connection.connection.con);
                    command.ExecuteNonQuery();
                    connection.connection.con.Close();

                    MessageBox.Show("Process Successful", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception) { }

                door.BackColor = Color.LimeGreen;
                faucets.BackColor = Color.LimeGreen;
                boxRoom1.BackColor = Color.LimeGreen;
                boxRoom2.BackColor = Color.LimeGreen;
                boxRoom3.BackColor = Color.LimeGreen;

                btnMaintenance.Text = "Maintenance";
                boxRoom1.Enabled = true;
                boxRoom2.Enabled = true;
                boxRoom3.Enabled = true;


            }
            Dashboard_Load(sender, e);
            pnlMod.Hide();
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            connection.connection.DB();
            string str = "select * from Male where RoomName = '" + dataGridView2.CurrentRow.Cells[0].Value + "'";
            SqlCommand command = new SqlCommand(str, connection.connection.con);
            function.function.dataReader = command.ExecuteReader();

            try
            {
                if (function.function.dataReader.Read())
                {

                    txtmodRoomName.Text = function.function.dataReader.GetString(0);
                    roomName = txtmodRoomName.Text;
                    txtModRoomLocation.Text = function.function.dataReader.GetString(1);
                    btnMaintenance.Text = function.function.dataReader.GetString(2);
                    cmboRoom1.Text = function.function.dataReader.GetString(3);
                    cmboRoom2.Text = function.function.dataReader.GetString(4);
                    cmboRoom3.Text = function.function.dataReader.GetString(5);


                    if (btnMaintenance.Text == "Maintenance")
                        btnMaintenance.Text = "Available";
                    else
                        btnMaintenance.Text = "Maintenance";


                }
            }
            catch (Exception) { }

            connection.connection.con.Close();
            pnlMod.Show();

        }

        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            connection.connection.DB();
            string str = "select * from Female where RoomName = '" + dataGridView3.CurrentRow.Cells[0].Value + "'";
            SqlCommand command = new SqlCommand(str, connection.connection.con);
            function.function.dataReader = command.ExecuteReader();

            try
            {
                if (function.function.dataReader.Read())
                {
                    txtmodRoomName.Text = function.function.dataReader.GetString(0);
                    roomName = txtmodRoomName.Text;
                    txtModRoomLocation.Text = function.function.dataReader.GetString(1);
                    btnMaintenance.Text = function.function.dataReader.GetString(2);
                    cmboRoom1.Text = function.function.dataReader.GetString(3);
                    cmboRoom2.Text = function.function.dataReader.GetString(4);
                    cmboRoom3.Text = function.function.dataReader.GetString(5);



                    if (btnMaintenance.Text == "Maintenance")
                        btnMaintenance.Text = "Available";
                    else
                        btnMaintenance.Text = "Maintenance";




                }
            }
            catch (Exception) { }

            connection.connection.con.Close();
            pnlMod.Show();
        }

        private void btnAddNewRoom_Click_1(object sender, EventArgs e)
        {
            if (txtRoomName.Text.CompareTo("") == 0 || txtRoomLocation.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Invalid null Input", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                string input;
                connection.connection.DB();

                if (genderTracker == true)
                    input = "Insert into Male (RoomName, Location, Availability, Room1, Room2, Room3) values('" + txtRoomName.Text.ToUpper() + "', '" + txtRoomLocation.Text + "','" + "Available" + "','" + "None" + "','" + "None" + "','" + "None" + "')";

                else
                    input = "Insert into Female (RoomName, Location, Availability, Room1, Room2, Room3) values('" + txtRoomName.Text.ToUpper() + "', '" + txtRoomLocation.Text + "','" + "Available" + "','" + "None" + "','" + "None" + "','" + "None" + "')";

                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Room add Successful", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Dashboard_Load(sender, e);
                connection.connection.con.Close();
                txtRoomName.Clear();
                txtRoomLocation.Clear();
                pnlAddRoom.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBacktoRooms_Click_1(object sender, EventArgs e)
        {
            pnlAddRoom.Hide();
        }

        private void btnAddRoom_Click_1(object sender, EventArgs e)
        {
            pnlAddRoom.Show();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            connection.connection.DB();
            string pick = "Select * from UserInfo where ID = " + dataGridView1.CurrentRow.Cells[0].Value + "";
            SqlCommand command = new SqlCommand(pick, connection.connection.con);

            function.function.dataReader = command.ExecuteReader();
            try
            {
                if (function.function.dataReader.Read())
                {


                    txtID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    txtPassword.Text = function.function.dataReader.GetString(1);
                    txtName.Text = function.function.dataReader.GetString(2);
                    boxGender.Text = function.function.dataReader.GetString(3);
                    txtContact.Text = function.function.dataReader.GetString(4);
                    txtUserStatus.Text = function.function.dataReader.GetString(7);
                    Options.Show();



                }
                connection.connection.con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.connection.con.Close();


            }
        }

        private void boxPaperPing_Click(object sender, EventArgs e)
        {
            pnlRequestHelp.Show();

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void comboConcern_TextUpdate(object sender, EventArgs e)
        {

        }

        private void btnRequestBack_Click(object sender, EventArgs e)
        {
            pnlRequestHelp.Hide();
        }

        private void comboConcern_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnRequestHelp_Click(object sender, EventArgs e)
        {
            lblNotif.ForeColor = Color.Red;
            try
            {
                connection.connection.DB();
                string str = "Insert into Notifications (WashRoomName, RoomNo, Concern) values('" + occWashRoom + "','" + occRoom + "', '" + comboConcern.Text + "')";
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                pnlRequestHelp.Hide();
            }
            catch (Exception) { }
        }

        private void dataNotification_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DialogResult rslt = MessageBox.Show("Was the Problem Solved?", "OPTIONS", MessageBoxButtons.YesNo);

            if (rslt == DialogResult.Yes)
            {

                connection.connection.DB();
                string str = "delete from Notifications where WashRoomName = '" + dataNotification.CurrentRow.Cells[0].Value + "' and RoomNo ='" + dataNotification.CurrentRow.Cells[1].Value + "' and Concern ='"+dataNotification.CurrentRow.Cells[2].Value+"'";
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();
                Dashboard_Load(sender, e);
            }



        }

        private void boxLocation_Click(object sender, EventArgs e)
        {
            if (lblLocation.Visible == false)
            {
                try
                {
                    string str;
                    connection.connection.DB();
                    if (string.Compare(userGender, "Male") == 0)
                        str = "select * from Male where RoomName ='" + roomName + "'";
                    else
                        str = "select * from Female where RoomName = '" + roomName + "'";

                    SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                    function.function.dataReader = cmd.ExecuteReader();

                    if (function.function.dataReader.Read())
                    {
                        lblLocation.Text = function.function.dataReader.GetString(1);
                        lblLocation.Visible = true;
                    }
                    connection.connection.con.Close();
                }
                catch (Exception) { }



            }
            else
            {
                lblLocation.Visible = false;
            }
        }

        private void chckAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chckAll.Checked == true)
            {
                Dashboard_Load(sender, e);
                chckAvailable.Checked = false;

            }

        }

        private void chckAvailable_CheckedChanged(object sender, EventArgs e)
        {
            if (chckAvailable.Checked == true)
            {
                chckAll.Checked = false;
                Dashboard_Load(sender, e);

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchWashRoom_TextChanged(object sender, EventArgs e)
        {
            string str;
            if (chckAvailable.Checked == true)
            {
                connection.connection.DB();
                if (string.Compare(userGender, "Male") == 0)
                    str = "select RoomName from Male where (RoomName like '%" + txtSearchWashRoom.Text + "%' or Location like '%" + txtSearchWashRoom.Text + "%') and Availability = 'Available' ";
                else
                    str = "select RoomName from Female where (RoomName like '%" + txtSearchWashRoom.Text + "%' or Location like '%" + txtSearchWashRoom.Text + "%') and Availability = 'Available' ";

                function.function.datagridfill(str, dataRooms);
                connection.connection.con.Close();
            }
            else if (chckAll.Checked == true)
            {
                connection.connection.DB();
                if (string.Compare(userGender, "Male") == 0)
                    str = "select RoomName from Male where RoomName like '%" + txtSearchWashRoom.Text + "%' or Location like '%" + txtSearchWashRoom.Text + "%'";
                else
                    str = "select RoomName from Female where RoomName like '%" + txtSearchWashRoom.Text + "%' or Location like '%" + txtSearchWashRoom.Text + "%'";

                function.function.datagridfill(str, dataRooms);
                connection.connection.con.Close();
            }
        }

        private void lblAll_Click(object sender, EventArgs e)
        {
            chckAll.Checked = true;
            chckAvailable.Checked = false;
        }

        private void lblAvailable_Click(object sender, EventArgs e)
        {
            chckAvailable.Checked = true;
            chckAll.Checked = false;
        }

        private void lblGender_Click_1(object sender, EventArgs e)
        {

        }

        private void lblUserNameDashboard_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void lblContact_Click(object sender, EventArgs e)
        {

        }

        private void panel14_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnUserModify_Click(object sender, EventArgs e)
        {
            string pick;
            connection.connection.DB();
            if (userStatus.CompareTo("Admin") == 0)
                pick = "Select * from Administrator where ID = " + userID + "";

            else
                pick = "Select * from UserInfo where ID = " + userID + "";

            SqlCommand command = new SqlCommand(pick, connection.connection.con);

            function.function.dataReader = command.ExecuteReader();
            try
            {
                if (function.function.dataReader.Read())
                {


                    txtUserId.Text = userID + "";
                    txtUserPassword.Text = function.function.dataReader.GetString(1);
                    txtUserName.Text = function.function.dataReader.GetString(2);
                    comboUserGender.Text = function.function.dataReader.GetString(3);
                    txtUserContact.Text = function.function.dataReader.GetString(4);

                    pnlEditProfile.Show();



                }
                connection.connection.con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.connection.con.Close();


            }
        }


        private void piePopulation_Click(object sender, EventArgs e)
        {

        }

        private void btnUserEdit_Click(object sender, EventArgs e)
        {
            if (string.Compare(userGender, comboUserGender.Text) != 0 && string.Compare(occWashRoom, "None") != 0)
            {
                MessageBox.Show("User is still using toilet, changing the Gender is Invalid", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (string.Compare(txtUserPassword.Text, "") == 0 || string.Compare(txtUserName.Text, "") == 0 || string.Compare(comboUserGender.Text, "") == 0 || string.Compare(txtUserContact.Text, "") == 0)
                {
                    MessageBox.Show("Incomplete Data", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string input;
                connection.connection.DB();

                if (string.Compare(userStatus, "Admin") == 0)
                    input = "Update Admin set Pass ='" + txtUserPassword.Text + "', Name ='" + txtUserName.Text + "',Gender ='" + comboUserGender.Text + "',Contact ='" + txtUserContact.Text + "' where " + "ID = '" + userID + "'";

                else
                    input = "Update UserInfo set Pass ='" + txtUserPassword.Text + "', Name ='" + txtUserName.Text + "',Gender ='" + comboUserGender.Text + "',Contact ='" + txtUserContact.Text + "' where " + "ID = '" + userID + "'";

                SqlCommand com = new SqlCommand(input, connection.connection.con);
                com.ExecuteNonQuery();
                MessageBox.Show("Edited Successfully", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                connection.connection.con.Close();
                Dashboard_Load(sender, e);
                Options.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            pnlEditProfile.Hide();
        }

        private void btnUserBack_Click(object sender, EventArgs e)
        {
            pnlEditProfile.Hide();
        }

        private void dataNotification_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {

        }

        private void boxLogOut_Click(object sender, EventArgs e)
        {

        }

        private void lblLogOut_Click(object sender, EventArgs e)
        {
            connection.connection.DB();
            string str = "update UserInfo set Status = 'Inactive' where ID = '" + userID + "'";
            SqlCommand cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();
            this.Hide();
            LogOut log = new LogOut(x);
            log.Show();



        }

        private void boxLogOut_Click_1(object sender, EventArgs e)
        {

            if (lblLogOut.Visible == false)
            {
                lblLogOut.Visible = true;
                boxArrow.Visible = true;
            }

            else
            {
                lblLogOut.Visible = false;
                boxArrow.Visible = false;
            }



        }

        private void lblMessages_Click_1(object sender, EventArgs e)
        {
            hide();
            lblMessages.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 390);

            lblViewMessages.Size = new Size(115, 32);
            lblCreateMessage.Size = new Size(115, 18);
            
            pnlCreateMessage.Hide();
            pnlExpandMessage.Hide();
            
            pnlMessages.Show();

            radOldMessage.Checked = true;
            radOldMessage.Checked = false;
            //==================
            if (userStatus.CompareTo("Admin") != 0)
            {
                pnlViewMessage.Show();
                radNewMessages.Checked = true;

            }
            else
            {
                pnlCreateMessage.Show();
                dataSuggestion.Hide();
            }
               






            //transferring
            string str= "";
            SqlCommand cmd;
            string newMessage = "";
            string oldMessage = "";
            //retrieving new Message
            try
            {
                connection.connection.DB();
                str = "select NewMessages from UserInfo where ID =" + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();

                if (function.function.dataReader.Read())
                {
                    newMessage = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();

                
            }
            catch (Exception) { return; }
            
            //retrieving old Message
            try
            {
                connection.connection.DB();
                str = "select OldMessages from UserInfo where ID =" + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();

                if (function.function.dataReader.Read())
                {
                    oldMessage = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();
                newMessage += ("|xxx|"+oldMessage);
            }
            catch (Exception) { }

            
            // changing the newMessage to null
            connection.connection.DB();
            str = "update UserInfo set NewMessages = NULL where ID ="+userID+" ";
            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();

            //updating the old messages
            connection.connection.DB();
            str = "update UserInfo set OldMessages = '"+newMessage+"' where ID ="+userID+" ";
            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();

            

            



        }

        private void label29_Click(object sender, EventArgs e)
        {

        }
        bool dataTracker = true;
        private void lblUserData_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataAdmin.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;


            lblUserData.Size = new Size(115, 32);
            lblAdminData.Size = new Size(115, 18);
            lblUserData.BackColor = Color.FromArgb(125, 40, 50);
            lblUserSlider.ForeColor = Color.Black;
            dataTracker = true;
        }

        private void lblAdminData_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataAdmin.Visible = true;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;


            lblAdminData.Size = new Size(115, 32);
            lblUserData.Size = new Size(115, 18);
            lblUserData.BackColor = Color.FromArgb(125, 40, 50);

            dataTracker = false;
        }

        private void pnlGender_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataAdmin_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataAdmin_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            connection.connection.DB();
            string pick = "Select * from Administrator where ID = " + dataAdmin.CurrentRow.Cells[0].Value + "";
            SqlCommand command = new SqlCommand(pick, connection.connection.con);

            function.function.dataReader = command.ExecuteReader();
            try
            {
                if (function.function.dataReader.Read())
                {


                    txtID.Text = dataAdmin.CurrentRow.Cells[0].Value.ToString();
                    txtPassword.Text = function.function.dataReader.GetString(1);
                    txtName.Text = function.function.dataReader.GetString(2);
                    boxGender.Text = function.function.dataReader.GetString(3);
                    txtContact.Text = function.function.dataReader.GetString(4);
                    txtUserStatus.Text = function.function.dataReader.GetString(7);
                    Options.Show();



                }
                connection.connection.con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.connection.con.Close();


            }
        }

        private void boxArrow_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void lblCompose_Click(object sender, EventArgs e)
        {


            lblCompose.Size = new Size(115, 32);
            lblRemove.Size = new Size(115, 18);
            pnlCompose.Show();
            pnlRemoveAnnouncement.Hide();


        }

        private void lblRemove_Click(object sender, EventArgs e)
        {
            connection.connection.DB();
            string str = "select * from Announcements order by Code Desc";
            function.function.datagridfill(str, datarRemoveAnnouncements);
            connection.connection.con.Close();

            lblRemove.Size = new Size(115, 32);
            lblCompose.Size = new Size(115, 18);
            pnlCompose.Hide();
            pnlRemoveAnnouncement.Show();
        }

        private void btnPostAnnouncement_Click(object sender, EventArgs e)
        {
            if (txtCode.Text.Equals("") && txtBody.Text.Equals(""))
            {
                MessageBox.Show("Incomplete Data", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                string str;
                SqlCommand cmd;
                connection.connection.DB();
                str = "Insert into Announcements (Code, Body) values('" + txtCode.Text + "', '" + txtBody.Text + "')";
                cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();
                MessageBox.Show("The Announcement has been Posted!", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                txtCode.Clear();
                txtBody.Clear();
                Dashboard_Load(sender, e);
            }
            catch (Exception)
            {
                MessageBox.Show("The Code is Duplicated and is therefore [INVALID] ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void lblAnnouncement_Click(object sender, EventArgs e)
        {
            lblTracker.Location = new Point(-1, 548);

            lblCompose.Size = new Size(115, 32);
            lblRemove.Size = new Size(115, 18);
            hide();
            pnlAnnouncement.Show();
            pnlCompose.Show();
            pnlRemoveAnnouncement.Hide();


            connection.connection.DB();
            string str = "select * from Announcements order by Code Desc";
            function.function.datagridfill(str, datarRemoveAnnouncements);
            connection.connection.con.Close();
        }

        private void btnClearAnnoucementText_Click(object sender, EventArgs e)
        {
            txtBody.Clear();
            txtCode.Clear();
        }

        private void txtCodeSearch_TextChanged(object sender, EventArgs e)
        {
            connection.connection.DB();
            string str = "select * from Announcements where Code like'" + txtCodeSearch.Text + "%'order by Code Desc";
            function.function.datagridfill(str, datarRemoveAnnouncements);
            connection.connection.con.Close();
        }



        private void btnRemove_Click(object sender, EventArgs e)
        {
            connection.connection.DB();
            string str = "delete from Announcements where Code =" + datarRemoveAnnouncements.CurrentRow.Cells[0].Value + "";
            SqlCommand cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();
            MessageBox.Show("Post Removed successfully", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            Dashboard_Load(sender, e);
            connection.connection.DB();

            str = "select * from Announcements order by Code Desc";
            function.function.datagridfill(str, datarRemoveAnnouncements);
            connection.connection.con.Close();

        }

       

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void pnlMessages_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlNotification_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void lblFriends_Click(object sender, EventArgs e)
        {
            hide();
            lblFriends.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 442);
            pnlFriends.Show();

            //setting the Friends tables
            string str;
            SqlCommand cmd;
            string requests = "";
            string friends = "";

            try
            {
                //friendrequests table
                connection.connection.DB();
                str = "select FriendRequests from UserInfo where ID =" + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    requests = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();

                // Friendrequests datatable
                connection.connection.DB();
                str = "select ID, Name from UserInfo where ID =" + requests + "";
                function.function.datagridfill(str, dataRequests);
                connection.connection.con.Close();

            }
            catch (Exception) { }

            try
            {
                //friends table
                connection.connection.DB();
                str = "select Friends from UserInfo where ID = " + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends = function.function.dataReader.GetString(0);
                }
               
                connection.connection.con.Close();

                connection.connection.DB();
                str = "select ID, Name from UserInfo where ID = " + friends + "";
                function.function.datagridfill(str, dataFriends);
                connection.connection.con.Close();

            }
            catch (Exception) { }
                

                


           



        }
        private void BtnSendRequest_Click_1(object sender, EventArgs e)
        {
            string str;
            SqlCommand cmd;
            string requests = "";
            try
            {
                if (duplicationChecker())
                    return;
                if (friendChecker())
                    return;
               

                connection.connection.DB();
                str = "select FriendRequests from UserInfo where ID =" + dataUserList.CurrentRow.Cells[1].Value + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    requests = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();
                if(requests.CompareTo("") ==0)
                    requests = userID + "";
                else
                    requests += " or ID =" + userID;
            }
            catch (Exception)
            {
                requests = userID + "";
            }

           

            connection.connection.DB();
            str = "update UserInfo set FriendRequests ='" + requests + "' where ID = " + dataUserList.CurrentRow.Cells[1].Value + "";

            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();
            MessageBox.Show("Sent Friend Request.", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void TxtUserListSearch_TextChanged(object sender, EventArgs e)
        {
            connection.connection.DB();
            string str = "select Name, ID from UserInfo where (ID like'%" + txtUserListSearch.Text + "%' or Name like '%" + txtUserListSearch.Text + "%') and ID<>"+userID+"";

            function.function.datagridfill(str, dataUserList);
            connection.connection.con.Close();

        }

       
        private void BtnAcceptRequest_Click(object sender, EventArgs e)
        {
            string str = "";
            SqlCommand cmd;
            string requests = "";
            //getting the values of friend requests
            try
            {
                
                connection.connection.DB();
                str = "select FriendRequests from UserInfo where ID =" + userID + " ";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    requests = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();
            }
            catch (Exception)
            {
                return;
            }
            
            
          
            
            string[] separator = { " or ID ="};

            //updating the value of friendrequests
            FriendRequests = requests.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            requests = "";
            foreach (string s in FriendRequests)
            {
                if (s.CompareTo(dataRequests.CurrentRow.Cells[0].Value + "") == 0)
                    continue;
                else
                {
                    if (string.Compare(requests, "") == 0)
                        requests = s;
                    else
                        requests += (" or ID ="+s);
                }
                
            }
            if (requests.CompareTo("") == 0)
            {
                requests = "NULL";
                str = "update UserInfo set FriendRequests = " + requests + " where ID = " + userID + "";
            }
            else
                str = "update UserInfo set FriendRequests = '" + requests + "' where ID = " + userID + "";
                

            connection.connection.DB();
           
            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();

            //====================================
            //getting the values of Friends

            string friends = "";
            try
            {
                //updating the value of who requested
                
                connection.connection.DB();
                str = "select Friends from UserInfo where ID = " + dataRequests.CurrentRow.Cells[0].Value + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();

            }
            catch (Exception) { }

            //updating the value of friends of who requested
            if (string.Compare(friends, "") == 0)
                friends = userID + "";
            else
                friends += (" or ID =" + userID);


            connection.connection.DB();
            str = "update UserInfo set Friends = '" + friends + "' where ID = " + dataRequests.CurrentRow.Cells[0].Value + "";
            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();

           
            try
            {
                // the one who accepted

                friends = "";
                connection.connection.DB();
                str = "select Friends from UserInfo where ID = " + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();

            }
            catch (Exception) { }

            //updating the value of friends
            if (string.Compare(friends, "") == 0)
                friends = dataRequests.CurrentRow.Cells[0].Value + "";
            else
                friends+=( " or ID =" + dataRequests.CurrentRow.Cells[0].Value);

            connection.connection.DB();
            str = "update UserInfo set Friends = '"+friends+"' where ID = "+userID+"";
            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();
            

            MessageBox.Show("Successfully added", "SUCCESSFUL", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            //==================updates
            //update friendrequests
            connection.connection.DB();
            str = "select ID, Name from UserInfo where ID = " + requests + "";
            function.function.datagridfill(str, dataRequests);
            connection.connection.con.Close();

            //update friends
            connection.connection.DB();
            str = "select ID, Name from UserInfo where ID = " + friends + "";
            function.function.datagridfill(str, dataFriends);
            connection.connection.con.Close();



        }
        public bool friendChecker()
        {
            //getting the values of Friends
            string str = "";
            SqlCommand cmd;
            string[] separator = { " or ID =" };
            string friends = "";
            try
            {


                connection.connection.DB();
                str = "select Friends from UserInfo where ID = " + dataUserList.CurrentRow.Cells[1].Value + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();

            }
            catch (Exception) { }

            //spliting friends
            Friends = friends.Split(separator,StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string s in Friends)
            {
                
                if (s.CompareTo(userID + "") == 0)
                {
                    MessageBox.Show("You are already FRIENDS with this person", "ERROR (Duplication)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            return false;
        }
        public bool duplicationChecker()
        {
            string str = "";
            SqlCommand cmd;
            string requests = "";
           
            string[] separator = { " or ID =" };

            //getting the values of friend requests
            try
            {

                connection.connection.DB();
                str = "select FriendRequests from UserInfo where ID =" + dataUserList.CurrentRow.Cells[1].Value + " ";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    requests = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();

                //===========================================================

                
            }
            catch (Exception)
            {
               
            }

            
            FriendRequests = requests.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            

            foreach (string s in FriendRequests)
            {
                if (s.CompareTo(userID + "") == 0)
                {
                    //spliting it for friend requests
                    MessageBox.Show("You have already sent a friend request to this person", "ERROR (Duplication)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
               

            }

            //=======================================================

           


            return false;

        }

        private void DataRequests_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult rslt = MessageBox.Show("Do you want to DELETE this friend request?", "OPTIONS", MessageBoxButtons.YesNo);

            if (rslt == DialogResult.Yes)
            {
                string str = "";
                SqlCommand cmd;
                string requests = "";
                string[] separator = { " or ID =" };
                
                //getting the values of friend requests
                try
                {

                    connection.connection.DB();
                    str = "select FriendRequests from UserInfo where ID =" + userID + " ";
                    cmd = new SqlCommand(str, connection.connection.con);
                    function.function.dataReader = cmd.ExecuteReader();
                    if (function.function.dataReader.Read())
                    {
                        requests = function.function.dataReader.GetString(0);
                    }
                    connection.connection.con.Close();
                }
                catch (Exception)
                {
                    return;
                }

                //updating the value of friendrequests
                FriendRequests = requests.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                requests = "";
                foreach (string s in FriendRequests)
                {
                    if (s.CompareTo(dataRequests.CurrentRow.Cells[0].Value + "") == 0)
                        continue;
                    else
                    {
                        if (string.Compare(requests, "") == 0)
                            requests = s;
                        else
                            requests += (" or ID =" + s);
                    }

                }
                if (requests.CompareTo("") == 0)
                {
                    requests = "NULL";
                    str = "update UserInfo set FriendRequests = " + requests + " where ID = " + userID + "";
                }
                
                else
                    str = "update UserInfo set FriendRequests = '" + requests + "' where ID = " + userID + "";


                connection.connection.DB();
                
                cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();

                MessageBox.Show("Request Deleted!", "SUCCESSFUL", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //update friendrequests
                connection.connection.DB();
                str = "select ID, Name from UserInfo where ID = " + requests + "";
                function.function.datagridfill(str, dataRequests);
                connection.connection.con.Close();

            }
        }

        private void DataFriends_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult rslt = MessageBox.Show("Do you want to Unfriend Him/Her?", "OPTIONS", MessageBoxButtons.YesNo);

            if (rslt == DialogResult.Yes)
            {
                string str = "";
                SqlCommand cmd;
                string friends = "";
                string[] separator = { " or ID =" };

                
                //Update the other friend
                //getting the values of friends
                try
                {

                    connection.connection.DB();
                    str = "select Friends from UserInfo where ID =" + dataFriends.CurrentRow.Cells[0].Value + " ";
                    cmd = new SqlCommand(str, connection.connection.con);
                    function.function.dataReader = cmd.ExecuteReader();
                    if (function.function.dataReader.Read())
                    {
                        friends = function.function.dataReader.GetString(0);
                    }
                    connection.connection.con.Close();
                }
                catch (Exception)
                {
                    return;
                }

                //updating the value of friendrequests
                Friends = friends.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                friends = "";
                foreach (string s in Friends)
                {
                    if (s.CompareTo(userID + "") == 0)
                        continue;
                    else
                    {
                        if (string.Compare(friends, "") == 0)
                            friends = s;
                        else
                            friends += (" or ID =" + s);
                    }

                }
                if (friends.CompareTo("") == 0)
                {
                    friends = "NULL";
                    str = "update UserInfo set Friends = " + friends + " where ID = " + dataFriends.CurrentRow.Cells[0].Value + "";
                }
                    
                else
                    str = "update UserInfo set Friends = '" + friends + "' where ID = " + dataFriends.CurrentRow.Cells[0].Value + "";

                connection.connection.DB();
                
                cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();

                //getting the values of friends
                try
                {

                    connection.connection.DB();
                    str = "select Friends from UserInfo where ID =" + userID + " ";
                    cmd = new SqlCommand(str, connection.connection.con);
                    function.function.dataReader = cmd.ExecuteReader();
                    if (function.function.dataReader.Read())
                    {
                        friends = function.function.dataReader.GetString(0);
                    }
                    connection.connection.con.Close();
                }
                catch (Exception)
                {
                    return;
                }

                //updating the value of friendrequests
                Friends = friends.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                friends = "";
                foreach (string s in Friends)
                {
                    if (s.CompareTo(dataFriends.CurrentRow.Cells[0].Value + "") == 0)
                        continue;
                    else
                    {
                        if (string.Compare(friends, "") == 0)
                            friends = s;
                        else
                            friends += (" or ID =" + s);
                    }

                }
                if (friends.CompareTo("") == 0)
                {
                    friends = "NULL";
                    str = "update UserInfo set Friends = " + friends + " where ID = " + userID + "";
                }
                    
                else
                    str = "update UserInfo set Friends = '" + friends + "' where ID = " + userID + "";

                connection.connection.DB();
                
                cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();


                MessageBox.Show("User Unfriended", "SUCCESSFUL", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //update friendrequests
                connection.connection.DB();
                str = "select ID, Name from UserInfo where ID = " + friends + "";
                function.function.datagridfill(str, dataFriends);
                connection.connection.con.Close();
            }
        }

        private void BtnRemoveMessage_Click(object sender, EventArgs e)
        {
            string OldMessage = "";
            string str;
            SqlCommand cmd;
            try
            {
                connection.connection.DB();
                str = "select OldMessages from UserInfo where ID =" + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    OldMessage = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();
            }
            catch (Exception) { }

            
            string[] separator = {"|xxx|"};
            string[] messages = OldMessage.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (radNewMessages.Checked == true)
                separator = new string []{ dataNewMessages.CurrentRow.Cells[0].Value + "", dataNewMessages.CurrentRow.Cells[1].Value + "",dataNewMessages.CurrentRow.Cells[2].Value + ""};

            else
                separator = new string[] { dataOldMessages.CurrentRow.Cells[0].Value + "", dataOldMessages.CurrentRow.Cells[1].Value + "", dataOldMessages.CurrentRow.Cells[2].Value + "" };
            

            OldMessage = "";
            for (int i = 0; i < messages.Length; i += 3)
            {
                if (messages[i].CompareTo(separator[0]) == 0 && messages[i + 1].CompareTo(separator[1]) == 0 && messages[i + 2].CompareTo(separator[2]) == 0)
                    continue;
                if (OldMessage.CompareTo("") == 0)
                    OldMessage = (messages[i] + "|xxx|" + messages[i+1]+ "|xxx|" +messages[i+2]);
                else
                    OldMessage += ("|xxx|" + messages[i] + "|xxx|" + messages[i + 1] + "|xxx|" + messages[i + 2]);
            }
            if (OldMessage.CompareTo("") == 0)
                str = "update UserInfo set OldMessages =NULL where ID =" + userID + "";
            else
                str = "update UserInfo set OldMessages ='" + OldMessage + "' where ID =" + userID + "";

            // updating the Oldmessages
            connection.connection.DB();
            
            cmd = new SqlCommand(str, connection.connection.con);
            cmd.ExecuteNonQuery();
            connection.connection.con.Close();
            //reviving
            if (radNewMessages.Checked == true)
            {
                radNewMessages.Checked = false;
                radNewMessages.Checked = true;
            }
            else
            {
                radOldMessage.Checked = false;
                radOldMessage.Checked = true;
            }
            MessageBox.Show("Message Deleted Successfully","SUCCESS",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


        }

        private void LblViewMessages_Click(object sender, EventArgs e)
        {          
                lblViewMessages.Size = new Size(115, 32);
                lblCreateMessage.Size = new Size(115, 18);
                pnlViewMessage.Show();
                pnlCreateMessage.Hide();   
        }

        private void LblCreateMessage_Click(object sender, EventArgs e)
        {
            lblCreateMessage.Size = new Size(115, 32);
            lblViewMessages.Size = new Size(115, 18);
            dataSuggestion.Hide();
            pnlViewMessage.Hide();
            pnlCreateMessage.Show();
        }

       

        private void TxtReceiver_TextChanged(object sender, EventArgs e)
        {
            if (txtReceiver.Text.Equals(""))
            {
                dataSuggestion.Hide();
                return;
            }
                

            string friends = "";
            string str;
            try
            {
                connection.connection.DB();
                str = "select Friends from UserInfo where ID = " + userID + "";
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends += function.function.dataReader.GetString(0);
                }
            }
            catch (Exception)
            {
                return;
            }
            
            try
            {
                connection.connection.DB();

                if(userStatus.CompareTo("Admin") ==0)
                     str = "select ID, Name from UserInfo where Name like'%" + txtReceiver.Text + "%' or ID like '%"+txtReceiver.Text+"%'";
                else
                     str = "select ID, Name from UserInfo where( Name like'%"+txtReceiver.Text+"%' or ID like '%"+txtReceiver.Text+"%') and( ID = "+ friends +" )";

                function.function.datagridfill(str, dataSuggestion);
                connection.connection.con.Close();
                dataSuggestion.Show();
                
            }
            catch (Exception)
            {
                MessageBox.Show("You have no Friends", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataSuggestion.Hide();
                return;
            }
            
        }

       

        private void TxtReceiver_MouseLeave(object sender, EventArgs e)
        {

            //dataSuggestion.Hide();

            //try {
            //         txtReceiver.Text = dataSuggestion.CurrentRow.Cells[1].Value + "";
            //    }
            //catch (Exception) { }
            
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            

            if ((dataSuggestion.CurrentRow.Cells[1].Value + "").CompareTo(txtReceiver.Text) != 0 || txtContent.Text.CompareTo("") == 0)
            {
                MessageBox.Show("Invalid Content");
                return;
            }
            string message;
            if (userStatus.CompareTo("Admin") == 0)
                message = "Admin" + "|xxx|" + "Admin" + "|xxx|" + txtContent.Text + "|xxx|";
            else
                message = userID + "|xxx|" + userName + "|xxx|" + txtContent.Text + "|xxx|";

            try
            {

                connection.connection.DB();
                string str = "select NewMessages from UserInfo where ID = " + dataSuggestion.CurrentRow.Cells[0].Value + "";
                
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    message += function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();
            }
            catch (Exception)
            {
                if (userStatus.CompareTo("Admin") == 0)
                    message = "Admin" + "|xxx|" + "Admin" + "|xxx|" + txtContent.Text;
                else
                    message = userID + "|xxx|" + userName + "|xxx|" + txtContent.Text;
                    
            }
            
            
            try
            {
                connection.connection.DB();
                string str = "update UserInfo set NewMessages = '" + message + "' where ID = " + dataSuggestion.CurrentRow.Cells[0].Value + "";
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();
                MessageBox.Show("Message Delivered!", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                dataSuggestion.Hide();
                txtReceiver.Clear();
                txtContent.Clear();
           
            }
            catch (Exception)
            { return; }
            
           
        }

       

        private void Button4_Click(object sender, EventArgs e)
        {
            txtReceiver.Clear();
            txtContent.Clear();
        }

        private void DataSuggestion_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtReceiver.Text = dataSuggestion.CurrentRow.Cells[1].Value + "";
            dataSuggestion.Hide();

        }

        private void RadNewMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (radNewMessages.Checked == true)
            {
                dataNewMessages.Rows.Clear();
                //retrieving the messages
                string Messages = "";
                SqlCommand cmd;
                try
                {
                    connection.connection.DB();
                    string str = "select NewMessages from UserInfo where ID = " + userID + "";
                    cmd = new SqlCommand(str, connection.connection.con);
                    function.function.dataReader = cmd.ExecuteReader();
                    if (function.function.dataReader.Read())
                    {
                        Messages = function.function.dataReader.GetString(0);
                    }
                    connection.connection.con.Close();
                }
                catch (Exception)
                {
                   
                }
                string[] separator = { "|xxx|" };
                string[] NewMessages = Messages.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                //putting it in the table
                for (int i = 0; i < (NewMessages.Length);)
                {
                    string[] row = { NewMessages[i++], NewMessages[i++], NewMessages[i++] };
                    dataNewMessages.Rows.Add(row);
                }

                dataNewMessages.Show();
                dataOldMessages.Hide();
            }
        }

        private void RadOldMessage_CheckedChanged(object sender, EventArgs e)
        {
            dataOldMessages.Rows.Clear();
            string Messages = "";
            SqlCommand cmd;

            try
            {
                connection.connection.DB();
                string str = "select OldMessages from UserInfo where ID = " + userID + "";
                cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    Messages = function.function.dataReader.GetString(0);
                }
                connection.connection.con.Close();
            }
            catch (Exception)
            {
               
            }
            string[] separator = { "|xxx|" };
            string[] OldMessages = Messages.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            //putting it in the table
            for (int i = 0; i < (OldMessages.Length);)
            {
                string[] row = { OldMessages[i++], OldMessages[i++], OldMessages[i++] };
                dataOldMessages.Rows.Add(row);
                
            }
            dataNewMessages.Hide();
            dataOldMessages.Show();
        }

        private void DataNewMessages_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtContext.Text = dataNewMessages.CurrentRow.Cells[2].Value + "";
            pnlExpandMessage.Show();
        }

        private void DataOldMessages_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtContext.Text = dataOldMessages.CurrentRow.Cells[2].Value + "";
            pnlExpandMessage.Show();
        }

        private void BtnMessageBack_Click(object sender, EventArgs e)
        {
            pnlExpandMessage.Hide();
        }

        private void BtnReply_Click(object sender, EventArgs e)
        {

            txtReceiver.Text = "";
            if (radNewMessages.Checked == true)
                txtReceiver.Text = dataNewMessages.CurrentRow.Cells[1].Value + "";
            
            else
                txtReceiver.Text = dataOldMessages.CurrentRow.Cells[1].Value + "";

            if (txtReceiver.Text.CompareTo("Admin") == 0)
            {
                MessageBox.Show("You cannot reply to this message", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            txtContent.Focus();
            pnlExpandMessage.Hide();
            lblCreateMessage.Size = new Size(115, 32);
            lblViewMessages.Size = new Size(115, 18);
            dataSuggestion.Hide();
            pnlViewMessage.Hide();
            pnlCreateMessage.Show();
        }

        private void TxtUserPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserPassword.Text))
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider1.SetError(txtUserPassword, "Password is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtUserPassword, null);
            }
        }

        private void TxtUserName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider1.SetError(txtUserName, "User Name is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtUserName, null);
            }
        }

        private void ComboUserGender_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(comboUserGender.Text))
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider1.SetError(comboUserGender, "Gender is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(comboUserGender, null);
            }
        }

        private void TxtUserContact_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserContact.Text))
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider1.SetError(txtUserContact, "Contact Number/Address is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtUserContact, null);
            }
        }

        private void TxtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                e.Cancel = true;
                txtID.Focus();
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
                txtID.Focus();
                errorProvider1.SetError(txtName, " Name is Null");

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
                txtID.Focus();
                errorProvider1.SetError(boxGender, "Gender is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(boxGender, null);
            }
        }

        private void TxtModRoomLocation_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtModRoomLocation.Text))
            {
                e.Cancel = true;
                txtID.Focus();
                errorProvider2.SetError(txtModRoomLocation, "Room Location is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider2.SetError(txtModRoomLocation, null);
            }
        }

        private void TxtRoomName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomName.Text))
            {
                
                txtID.Focus();
                errorProvider3.SetError(txtRoomName, "Room Name is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider3.SetError(txtRoomName, null);
            }
        }

        private void TxtRoomLocation_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomLocation.Text))
            {
               
                txtID.Focus();
                errorProvider3.SetError(txtRoomLocation, "Room Location is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider3.SetError(txtRoomLocation, null);
            }
        }

        private void TxtCode_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCode.Text))
            {
                
                txtID.Focus();
                errorProvider4.SetError(txtCode, "The Code is Null");

            }
            else
            {
                e.Cancel = false;
                errorProvider4.SetError(txtCode, null);
            }
            try
            {
                Int32.Parse(txtCode.Text);
            }
            catch (Exception)
            {
               
                txtID.Focus();
                errorProvider4.SetError(txtCode, "The Code should be a NUMBER");
            }
        }

        private void TxtUserPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtUserName.Focus();
                e.Handled = true;

            }
        }

        private void TxtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                comboUserGender.Focus();
                e.Handled = true;

            }
        }

        private void ComboUserGender_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtUserContact.Focus() ;
                e.Handled = true;

            }
        }

        private void TxtUserContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnUserEdit_Click(sender, e);
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
                txtUserStatus.Focus();
                e.Handled = true;

            }
        }

        private void TxtUserStatus_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                
                e.Handled = true;
                return;

            }
        }

        private void TxtReceiver_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {

                txtContent.Focus();
                txtReceiver.Text = dataSuggestion.CurrentRow.Cells[1].Value + "";
                dataSuggestion.Hide();
                e.Handled = true;
            }
        }

       

        private void TxtAcceptSearch_TextChanged(object sender, EventArgs e)
        {
            string friends = "";
            string str;
            try
            {
                connection.connection.DB();
                str = "select FriendRequests from UserInfo where ID = " + userID + "";
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends += function.function.dataReader.GetString(0);
                }
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                connection.connection.DB();

                if (userStatus.CompareTo("Admin") == 0)
                    str = "select ID, Name from UserInfo where Name like'%" +txtAcceptSearch.Text + "%' or ID like '%" + txtAcceptSearch.Text + "%'";
                else
                    str = "select ID, Name from UserInfo where( Name like'%" + txtAcceptSearch.Text + "%' or ID like '%" + txtAcceptSearch.Text + "%') and( ID = " + friends + " )";

                function.function.datagridfill(str, dataRequests);
                connection.connection.con.Close();
                

            }
            catch (Exception)
            {
                MessageBox.Show("You have no FriendRequests", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataSuggestion.Hide();
                return;
            }
        }

        private void TxtSearchFriends_TextChanged(object sender, EventArgs e)
        {
            string friends = "";
            string str;
            try
            {
                connection.connection.DB();
                str = "select Friends from UserInfo where ID = " + userID + "";
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                function.function.dataReader = cmd.ExecuteReader();
                if (function.function.dataReader.Read())
                {
                    friends += function.function.dataReader.GetString(0);
                }
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                connection.connection.DB();

                if (userStatus.CompareTo("Admin") == 0)
                    str = "select ID, Name from UserInfo where Name like'%" + txtSearchFriends.Text + "%' or ID like '%" + txtSearchFriends.Text + "%'";
                else
                    str = "select ID, Name from UserInfo where( Name like'%" + txtSearchFriends.Text + "%' or ID like '%" + txtSearchFriends.Text + "%') and( ID = " + friends + " )";

                function.function.datagridfill(str, dataFriends);
                connection.connection.con.Close();


            }
            catch (Exception)
            {
                MessageBox.Show("You have no FriendRequests", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataSuggestion.Hide();
                return;
            }
        }

        private void Label55_Click(object sender, EventArgs e)
        {

        }

        private void TxtMonth_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (Int32.Parse(txtMonth.Text) >12)
                {
                    
                    txtID.Focus();
                    errorProvider1.SetError(txtMonth, "Invalid Input in [Month]");

                }
                else
                {
                    e.Cancel = false;
                    errorProvider1.SetError(txtMonth, null);
                }
            }
            catch (Exception)
            {
                
                txtID.Focus();
                errorProvider1.SetError(txtMonth, "Invalid Input in [Month]");
            }
        }

        private void TxtDay_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (Int32.Parse(txtDay.Text) > 31)
                {
                    
                    txtID.Focus();
                    errorProvider1.SetError(txtDay, "Invalid Input in [Day]");

                }
                else
                {
                    e.Cancel = false;
                    errorProvider1.SetError(txtDay, null);
                }
            }
            catch (Exception)
            {
                
                txtID.Focus();
                errorProvider1.SetError(txtDay, "Invalid Input in [Day]");
            }
        }

        private void TxtYear_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (Int32.Parse(txtYear.Text) == 0)
                {
                    
                    txtID.Focus();
                    errorProvider1.SetError(txtYear, "Invalid Input in [Year]");

                }
                else
                {
                    e.Cancel = false;
                    errorProvider1.SetError(txtYear, null);
                }
            }
            catch (Exception)
            {
                
                txtID.Focus();
                errorProvider1.SetError(txtYear, "Invalid Input in [Year]");
            }
        }

        private void TxtHr_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (Int32.Parse(txtHr.Text) > 12)
                {
                    
                    txtID.Focus();
                    errorProvider1.SetError(txtHr, "Invalid Input in [Hour]");

                }
                else
                {
                    
                    errorProvider1.SetError(txtHr, null);
                }
            }
            catch (Exception)
            {
                
                txtID.Focus();
                errorProvider1.SetError(txtHr, "Invalid Input in [Hour]");
            }
        }

        private void TxtMin_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (Int32.Parse(txtMin.Text) > 12)
                {
                    
                    txtID.Focus();
                    errorProvider1.SetError(txtMin, "Invalid Input in [Minute]");

                }
                else
                {
                    e.Cancel = false;
                    errorProvider1.SetError(txtMin, null);
                }
            }
            catch (Exception)
            {
                
                txtID.Focus();
                errorProvider1.SetError(txtMin, "Invalid Input in [Minute]");
            }
        }

        private void CmboAmPm_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (cmboAmPm.Text.CompareTo("") == 0)
                {
                    
                    txtID.Focus();
                    errorProvider1.SetError(cmboAmPm, "Invalid Input in [AM/PM]");

                }
                else
                {
                    e.Cancel = false;
                    errorProvider1.SetError(cmboAmPm, null);
                }
            }
            catch (Exception)
            {
                
                txtID.Focus();
                errorProvider1.SetError(cmboAmPm, "Invalid Input in [AM/PM]");
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            if (txtLocation.Text.Equals("") || txtReport.Text.Equals(""))
            {
                MessageBox.Show("Invalid Null Input", "Ooops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connection.connection.DB();
                string str ="Insert into Reports (ID, Location, Report, Status, DateTime) values('" + userID+ "', '" + txtLocation.Text + "','" + txtReport.Text + "', 'Unsolved', '" + (txtMonth.Text + "/" + txtDay.Text + "/" + txtYear.Text + " @ " + txtHr.Text + ":" + txtMin.Text + "[" + cmboAmPm.Text + "]") + "')";
                                
                SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                cmd.ExecuteNonQuery();
                connection.connection.con.Close();

                MessageBox.Show("Incident Reported Successfully", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                pnlReport.Hide();
                txtMonth.Clear();
                txtDay.Clear();
                txtYear.Clear();
                txtHr.Clear();
                txtMin.Clear();
                txtLocation.Clear();
                txtReport.Clear();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Input", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void DataReports_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (radAll.Checked == true)
                return;
            DialogResult rslt = MessageBox.Show("Was the Problem Solved?", "OPTIONS", MessageBoxButtons.YesNo);

            try
            {
                if (rslt == DialogResult.Yes)
                {

                    connection.connection.DB();
                    string str = "update Reports set Status = 'Solved' where ID ='" + dataReports.CurrentRow.Cells[0].Value + "' and Location = '" + dataReports.CurrentRow.Cells[1].Value + "' and DateTime ='" + dataReports.CurrentRow.Cells[3].Value + "' and Report = '" + dataReports.CurrentRow.Cells[2].Value+"'";
                    SqlCommand cmd = new SqlCommand(str, connection.connection.con);
                    cmd.ExecuteNonQuery();
                    connection.connection.con.Close();

                    connection.connection.DB();
                    str = "select ID, Location, Report, DateTime from Reports where Status = 'Unsolved' order by DateTime DESC";
                    function.function.datagridfill(str, dataReports);
                    connection.connection.con.Close();
                }
            }
            catch (Exception )
            {
               
            }
        }

        private void LblReport_Click(object sender, EventArgs e)
        {
            
            lblData.ForeColor = Color.Teal;
            lblTracker.Location = new Point(-1, 494);
            pnlReport.Show();

            string timenow = DateTime.Now.ToString();
                

            char[] separator = {':' ,' ', '/'};
            string[] time = timenow.Split(separator);

            txtDay.Text = time[0];
            txtMonth.Text = time[1];
            txtYear.Text = time[2];
            txtHr.Text = time[3];
            txtMin.Text = time[4];
            txtSec.Text = time[5];
            cmboAmPm.Text = time[6];

            
            
        }

        private void PictureBox20_Click(object sender, EventArgs e)
        {

        }

        private void BtnBackReport_Click(object sender, EventArgs e)
        {
            pnlReport.Hide();
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void RadAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radAll.Checked == true)
            {
                connection.connection.DB();
                string str = "select ID, Location, Report, DateTime, Status from Reports order by DateTime DESC";
                function.function.datagridfill(str, dataReports);
                connection.connection.con.Close();
            }
        }

        private void RadUnsolved_CheckedChanged(object sender, EventArgs e)
        {
            if (radUnsolved.Checked == true)
            {
                connection.connection.DB();
                string str = "select ID, Location, Report, DateTime from Reports where Status = 'Unsolved' order by DateTime DESC";
                function.function.datagridfill(str, dataReports);
                connection.connection.con.Close();
            }
        }

        private void Panel27_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Label54_Click(object sender, EventArgs e)
        {

        }

        private void TxtLocation_TextChanged(object sender, EventArgs e)
        {          

               
            connection.connection.DB();
            string str = "select RoomName from Male where RoomName like'%" + txtLocation.Text + "%' union all select RoomName from Female where RoomName like'%"+txtLocation.Text+"%'";
            function.function.datagridfill(str, dataCombinedRooms);
            connection.connection.con.Close();
            dataCombinedRooms.Show();
            if (txtLocation.Text.Equals(""))
            {
                dataCombinedRooms.Hide();
                return;
            }
        }

        private void TxtLocation_Validating(object sender, CancelEventArgs e)
        {

        }

        private void TxtLocation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtLocation.Text = dataCombinedRooms.CurrentRow.Cells[0].Value + "";
                dataCombinedRooms.Hide();
                txtReport.Focus();
                e.Handled = true;

            }
        }

        private void DataCombinedRooms_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtLocation.Text = dataCombinedRooms.CurrentRow.Cells[0].Value + "";
                dataCombinedRooms.Hide();
                txtReport.Focus();
                e.Handled = true;

            }
        }

        private void TxtReport_Click(object sender, EventArgs e)
        {
            try
            {
                txtLocation.Text = dataCombinedRooms.CurrentRow.Cells[0].Value + "";
                dataCombinedRooms.Hide();
                txtReport.Focus();
            }
            catch (Exception)
            { }
            
                

            
        }

        private void TxtReportSearch_TextChanged(object sender, EventArgs e)
        {
            if (radUnsolved.Checked == true)
            {
                connection.connection.DB();
                string str = "select ID, Location, Report, DateTime from Reports where Status = 'Unsolved' and (DateTime like '%"+txtReportSearch.Text+ "%' or ID like '%" + txtReportSearch.Text + "%' or Location like '%" + txtReportSearch.Text + "%') order by DateTime DESC";
                function.function.datagridfill(str, dataReports);
                connection.connection.con.Close();
            }
            else
            {
                connection.connection.DB();
                string str = "select ID, Location, Report, DateTime, Status from Reports where DateTime like '%" + txtReportSearch.Text + "%' or ID like '%" + txtReportSearch.Text + "%' or Location like '%" + txtReportSearch.Text + "%'  order by DateTime DESC";
                function.function.datagridfill(str, dataReports);
                connection.connection.con.Close();
            }
        }
    } 
}

      
    
