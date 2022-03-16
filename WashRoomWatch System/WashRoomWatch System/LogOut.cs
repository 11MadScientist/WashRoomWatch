using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashRoomWatch_System
{
    public partial class LogOut : Form
    {
        public Form1 x;
        public LogOut(Form1 x)
        {
            this.x = x;
            InitializeComponent(); 
            

        }

     

        private void LogOut_Load(object sender, EventArgs e)
        {
            Thread t1 = new Thread(barLoad);
            Thread t2 = new Thread(textLoad);
            t1.Start();
            t2.Start();
        }
        public void textLoad()
        {
            for (int i = 0; i <= 340; i++)
            {
                try
                {
                    if (i == 34)
                    {
                        L.Invoke(new Action(() => L.Show()));
                    }

                    else if (i == 68)
                    {
                        O.Invoke(new Action(() => O.Show()));

                    }

                    else if (i == 102)
                    {
                        G1.Invoke(new Action(() => G1.Show()));
                    }

                    else if (i == 136)
                    {
                        G2.Invoke(new Action(() => G2.Show()));
                    }

                    else if (i == 170)
                    {
                        I.Invoke(new Action(() => I.Show()));
                    }

                    else if (i == 204)
                    {
                        N.Invoke(new Action(() => N.Show()));
                    }

                    else if (i == 238)
                    {
                        G3.Invoke(new Action(() => G3.Show()));
                    }

                    else if (i == 272)
                    {
                        O2.Invoke(new Action(() => O2.Show()));
                    }

                    else if (i == 306)
                    {
                        F1.Invoke(new Action(() => F1.Show()));
                    }

                    else if (i == 340)
                    {
                        F2.Invoke(new Action(() => F2.Show()));
                    }

                    Thread.Sleep(15);
                }
                catch (Exception) { }
                
                    
            }
        }
       
      

        public void barLoad()
        {
            
            for (int i = 0; i <= 340; i++)
            {
                txtGreen.Invoke(new Action(()=> txtGreen.Size = new Size(i, 23)));
                Thread.Sleep(14);
            }
            
           
            this.Invoke(new Action(()=> this.Close()));
            x.Invoke(new Action(() => x.Show()));

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
