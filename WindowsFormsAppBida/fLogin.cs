using WindowsFormsAppBida.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida;
using System.Deployment.Application;
using System.Net.NetworkInformation;
using System.Media;
using System.Net.Sockets;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using WindowsFormsAppBida.DTO;
using System.Threading;
using System.Net.PeerToPeer;
using System.Security.Principal;
using System.Xml;
using System.IO.Ports;
using System.Management;

namespace WindowsFormsAppBida
{
    public partial class fLogin : Form
    {
        private Account loginAccount;
        SerialPort serialPort = new SerialPort();
        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; }
        }
        public fLogin()
        {
            InitializeComponent();
        }


        private void bunifuBtnLogin_Click(object sender, EventArgs e)
        {
            string email = bunifuTxbUserName.Text;
            string passWord = bunifuTxbPassWord.Text;
            if (Login(email, passWord))
            {
                this.UseWaitCursor = false;
                Account loginAccount = AccountDAO.Instance.GetAccountByUserEmail(email);
                AccountDAO.Instance.updateStatusAcc(email, 1);

                fTableManager f = null;

                try
                {
                    f = new fTableManager(loginAccount);
                    this.Hide();
                    f.ShowDialog();
                    this.Show();
                }
                finally
                {
                    if (f != null)
                    {
                        f.Dispose(); // Release resources associated with the form
                    }
                }

                AccountDAO.Instance.updateStatusAcc(email, 0);
            }
            else
            {
                this.UseWaitCursor = false;
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bunifuTxbPassWord.Clear();
            }
        }



        bool Login(string email, string passWord)
        {
            return AccountDAO.Instance.Login(email, passWord);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                Application.Exit();
                
            }
        }

        //public static class AppSession
        //{
        //    public static Account CurrentAccount { get; set; }
        //}

        private void label7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Đăng kí tài khoản mới. Chỉ quản lý mới được sử dụng "), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               fSignUp f = new fSignUp();
               f.ShowDialog();
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            fForgetPass fForgetPass = new fForgetPass();
            fForgetPass.ShowDialog();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine(); // Read the received data

            // Update the textbox with the received data
            BeginInvoke(new Action(() =>
            {
                bunifuTxbPassWord.Text = data;
                serialPort.Close();

            }));



        }
        



    }
}
