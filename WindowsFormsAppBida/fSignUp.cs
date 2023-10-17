using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.Design;
using System.Windows.Forms;
using Twilio;
using Twilio.Clients;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using static System.Net.WebRequestMethods;
using Twilio.TwiML.Voice;
using System.Data.SqlClient;
using System.IO;
using WindowsFormsAppBida.DAO;
using Org.BouncyCastle.Utilities.Encoders;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Globalization;
using WindowsFormsAppBida.DTO;
using System.IO.Ports;
using System.Management;
using ServiceStack;

namespace WindowsFormsAppBida
{
    public partial class fSignUp : Form
    {
        private string toNumber;
        private int otp;
        SerialPort serialPort = new SerialPort();
        public fSignUp()
        {
            InitializeComponent();
            SearchSerialPort();
        }
        string imgUrl;
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            Image img = imageAccount.Image;
            byte[] arr;
            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));

            if (txbNameAdmin.Text == String.Empty)
            {
                MessageBox.Show("Vui lòng điền Tên quản lý", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (txbEmailAdmin.Text == String.Empty)
            {
                MessageBox.Show("Vui lòng điền email quản lý", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!Regex.IsMatch(txbEmailAdmin.Text, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                MessageBox.Show("Email quản lý không hợp lệ. Vui lòng nhập địa chỉ email của Gmail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (txbPhoneAdmin.Text == String.Empty && txbPhoneAdmin.MaxLength == 10)
            {
                MessageBox.Show("Vui lòng điền số điện thoại !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (cbSexAdmin.Text == String.Empty)
            {
                MessageBox.Show("Vui lòng lựa chọn giới tính", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (txbPassAdmin.Text == String.Empty || txbRePassAdmin.Text == String.Empty)
            {
                MessageBox.Show("Vui lòng nhập mật khẩu", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (txbPassAdmin.Text != txbRePassAdmin.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else if (!checkBoxSigUp.Checked)
            {
                MessageBox.Show("Vui lòng chấp nhận điều khoản sử dụng", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string email = txbEmailAdmin.Text;
                string name = txbNameAdmin.Text;
                int sex = 0;
                if (cbSexAdmin.Text == "Nam")
                {
                    sex = 1;
                }    

                string PassWord = txbPassAdmin.Text;
                int phone = Convert.ToInt32(txbPhoneAdmin.Text);
                int type = 1;
                try
                {
                    if (InsertAdmin(email, name, sex, PassWord, phone, type, arr))
                    {
                        MessageBox.Show("Đăng kí thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng thử lại", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Lỗi khi thêm dữ liệu vui lòng liên hệ bộ phận kỹ thuật! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }


        }

        void sendOtp()
        {
             Random rnd = new Random();
             otp = rnd.Next(100000, 999999);

             var accountSid = "AC080d88413f87163109a58d2302df0a73";
             var authToken = "0bcfdfef2d2ba2298fe8d3a3846042b4";
             TwilioClient.Init(accountSid, authToken);

             var messageOptions = new CreateMessageOptions(new PhoneNumber(toNumber));
             messageOptions.From = new PhoneNumber("+16073605409");
             messageOptions.Body = "Your OTP is: " + otp.ToString();

            try
            {
                var message = MessageResource.Create(messageOptions);
                Console.WriteLine(message.Body);
                MessageBox.Show("Mã OTP đã được gửi vào SĐT đăng kí hệ thống");
                txbOtp.Enabled = true;
            }
            catch 
            {
                MessageBox.Show("SĐT không hợp lệ hoặc không hoạt động vui lòng kiểm tra lại");
                txbOtp.Enabled = false;
            }

                  
        }

        void verifyOtp()
        {

                int userOtp = Convert.ToInt32(txbOtp.Text);
                if (userOtp == otp)
                {
                    btnSignUp.Enabled = true;
                  
                }
                else
                {
                    MessageBox.Show("Lỗi xác thực OTP \n Vui lòng nhập lại SĐT hoặc mã OTP");
                    txbOtp.Clear();
                }

           
        }



        private void txbOtp_TextChange(object sender, EventArgs e)
        {
            if (txbOtp.Text.Length == 6)
            {
                verifyOtp();
            }
        }

        private void txbNameAdmin_TextChange(object sender, EventArgs e)
        {

        }

        bool InsertAdmin(string email, string name, int sex, string PassWord, int phone, int type, byte[] image)
        {
            return AccountDAO.Instance.InsertAdmin(email, name, sex, PassWord, phone, type, image);
        }

        private void txbPassAdmin_TextChange(object sender, EventArgs e)
        {
            txbPassAdmin.PasswordChar = '*';
        }

        private void txbRePassAdmin_TextChange(object sender, EventArgs e)
        {
            txbRePassAdmin.PasswordChar = '*';
        }

        private string NormalizeText(string text)
        {
            // Remove diacritics (accent marks)
            string normalizedText = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Convert to lowercase for case-insensitive comparison
            normalizedText = stringBuilder.ToString().ToLower();

            return normalizedText;
        }

public bool CheckIfNameOrEmailExistsAccount(List<Account> accountList, string textboxValue)
{
    string normalizedTextboxValue = NormalizeText(textboxValue);
    bool matchFound = false; // Flag to track if a match has been found

    foreach (Account account in accountList)
    {
        string normalizedName = NormalizeText(account.Name);
        string normalizedEmail = NormalizeText(account.Email);
        string normalizedPhone = NormalizeText(account.Phone.ToString());
        string normalizedIdCard = NormalizeText(account.IdCard);

        if (string.Equals(normalizedTextboxValue, normalizedName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalizedTextboxValue, normalizedEmail, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalizedTextboxValue, normalizedPhone, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalizedTextboxValue, normalizedIdCard, StringComparison.OrdinalIgnoreCase))
        {
            // Match found
            if (matchFound)
            {
                return true; // Already found a match before, no need to continue
            }
            else
            {
                matchFound = true; // Set the flag to true
            }
        }
    }

    return matchFound; // Return the flag value
}



        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
            if (txbPhoneAdmin.Text.Length == 10 )
            {
                toNumber = "+84" + txbPhoneAdmin.Text;               
                sendOtp();

            }
            else
            {
                MessageBox.Show("SĐT không hợp lệ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }    
        }

        private void imageAccount_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imgUrl = ofd.FileName;
                    imageAccount.Image = Image.FromFile(ofd.FileName);
                }
                else
                {
                    imageAccount.Image = Properties.Resources.noimage;
                }    
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine(); // Read the received data
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


        private void txbIdCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }




        private void txbPhoneAdmin_Leave(object sender, EventArgs e)
        {
            string phone = txbPhoneAdmin.Text.Trim();
            int firstDigitIndex = phone.IndexOfAny("0123456789".ToCharArray());

            if (firstDigitIndex >= 0)
            {
                string substring = phone.Substring(firstDigitIndex + 1);
                bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), substring);

                if (isMatch)
                {
                    MessageBox.Show("SĐT đã tồn tại. Vui lòng nhập SĐT khác!");
                    txbPhoneAdmin.Clear();
                }
                else
                {
                    bunifuPictureBox1.Visible = true;
                }

            }
        }



        private void txbEmailAdmin_Leave(object sender, EventArgs e)
        {
            string text = txbEmailAdmin.Text.Trim();
            bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), text);

            if (isMatch && !String.IsNullOrEmpty(text))
            {
                MessageBox.Show("Email đã tồn tại. Vui lòng nhập email khác!");
                txbEmailAdmin.Clear();
            }   
        }


    }
}
