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
            try
            {
                NetworkInterface.GetIsNetworkAvailable();
                StartServer.Instance.startServer();
                InitializeComponent();
                SearchSerialPort();
                StartServer.Instance.StartReceiving();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void bunifuBtnLogin_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            if (string.IsNullOrEmpty(bunifuTxbUserName.Text))
            {
                this.UseWaitCursor = false;
                MessageBox.Show("Vui lòng không để trống email đăng nhập!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!checkBoxIdCard.Checked && !checkBoxPass.Checked)
            {
                this.UseWaitCursor = false;
                MessageBox.Show("Vui lòng chọn cách thức đăng nhập", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (checkBoxIdCard.Checked && checkBoxPass.Checked)
            {
                this.UseWaitCursor = false;
                MessageBox.Show("Vui lòng chọn 1 cách thức đăng nhập", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checkBoxIdCard.Checked = false;
                checkBoxPass.Checked = false;
            }
            else if (checkBoxPass.Checked)
            {
                string email = bunifuTxbUserName.Text;
                string passWord = bunifuTxbPassWord.Text;
                if (Login(email, passWord))
                {
                    this.UseWaitCursor = false;
                    Account loginAccount = AccountDAO.Instance.GetAccountByUserEmail(email);
                    AccountDAO.Instance.updateStatusAcc(email, 1);
                    fTableManager f = new fTableManager(loginAccount);
                    StartServer.Instance.checkDevice();
                    StartServer.Instance.autosendCheckStatusDevice();


                    this.Hide();
                    f.ShowDialog();
                    this.Show();
                    AccountDAO.Instance.updateStatusAcc(email, 0);
                }
                else
                {
                    this.UseWaitCursor = false;
                    MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bunifuTxbPassWord.Clear();
                }
            }
            else if (checkBoxIdCard.Checked)
            {
                string email = bunifuTxbUserName.Text;
                string idCard = bunifuTxbPassWord.Text;
                if (LoginCard(email, idCard))
                {
                    this.UseWaitCursor = false;
                    Account loginAccount = AccountDAO.Instance.GetAccountByUserEmail(email);
                    AccountDAO.Instance.updateStatusAcc(email, 1);
                    fTableManager f = new fTableManager(loginAccount);
                    StartServer.Instance.checkDevice();
                    StartServer.Instance.autosendCheckStatusDevice();


                    this.Hide();
                    f.ShowDialog();
                    this.Show();
                    AccountDAO.Instance.updateStatusAcc(email, 0);
                }
                else
                {
                    this.UseWaitCursor = false;
                    MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bunifuTxbPassWord.Clear();
                }
            }



        }



        bool Login(string email, string passWord)
        {
            return AccountDAO.Instance.Login(email, passWord);
        }
        bool LoginCard(string email, string idCard)
        {
            return AccountDAO.Instance.LoginCard(email, idCard);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                StartServer.Instance.stopServer();
                
                Application.Exit();
                
            }
        }

        public static class AppSession
        {
            public static Account CurrentAccount { get; set; }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Đăng kí tài khoản sẽ kích hoạt hệ thống mới. Chỉ quản lý mới được sử dụng "), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
        private void SearchSerialPort()
        {
            // Get a list of available serial ports
            string[] ports = SerialPort.GetPortNames();

            // Iterate through the available ports
            foreach (string port in ports)
            {
                // Check if the port is the USB reader
                if (IsUSBReader(port))
                {
                    // Open the serial port
                    serialPort.PortName = port;
                    serialPort.BaudRate = 9600; // Adjust the baud rate as needed
                    serialPort.Open();

                    // Attach the data received event handler
                    serialPort.DataReceived += serialPort_DataReceived;

                    // Break out of the loop since the reader is found
                    break;
                }
            }
        }


        private bool IsUSBReader(string port)
        {
            // Query the system for connected USB devices
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%USB Serial Port%'");

            // Iterate through the connected USB devices
            foreach (ManagementObject device in searcher.Get())
            {
                // Check if the device description contains the port name
                if (device["Description"].ToString().Contains(port))
                {
                    return true;
                }
            }

            return false;
        }

        private void checkBoxPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPass.Checked)
            {
                checkBoxPass.Checked = true;
                checkBoxIdCard.Checked = false;
                bunifuTxbPassWord.Clear();
            }    
            
        }

        private void checkBoxIdCard_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIdCard.Checked)
            {
                checkBoxPass.Checked = false;
                checkBoxIdCard.Checked = true;
                bunifuTxbPassWord.Clear();
            }
        }
    }
}
