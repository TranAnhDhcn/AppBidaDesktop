using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsAppBida
{
    public partial class fCreateNewAccStaff : Form
    {
        SerialPort serialPort = new SerialPort();
        string imgUrl;
        string idcard;
        public fCreateNewAccStaff()
        {
            InitializeComponent();
            SearchSerialPort();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

        bool InsertAdmin(string email, string name, int sex, string PassWord, int phone, int type, byte[] image, string idCard)
        {
            return AccountDAO.Instance.InsertAdmin(email, name, sex, PassWord, phone, type, image, idCard);
        }

        private void txbPassStaff_TextChange(object sender, EventArgs e)
        {
            txbPassStaff.PasswordChar = '*';
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

            foreach (Account account in accountList)
            {
                string normalizedName = NormalizeText(account.Name);
                string normalizedEmail = NormalizeText(account.Email);
                string normalizedPhone = NormalizeText(account.Phone.ToString());
                string normalizedIdCard = NormalizeText(account.IdCard.ToString());

                if (string.Equals(normalizedTextboxValue, normalizedName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(normalizedTextboxValue, normalizedEmail, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(normalizedTextboxValue, normalizedPhone, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(normalizedTextboxValue, normalizedIdCard, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Match found
                }
            }

            return false; // No match found
        }


        private void txbPhoneStaff_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void imageAccountStaff_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imgUrl = ofd.FileName;
                    imageAccountStaff.Image = Image.FromFile(ofd.FileName);
                }
                else
                {
                    imageAccountStaff.Image = Properties.Resources.noimage;
                }
            }
        }

        private void btnAddNewAccStaff_Click_1(object sender, EventArgs e)
        {
            Image img = imageAccountStaff.Image;
            byte[] arr;
            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));


            if (string.IsNullOrWhiteSpace(txbNameStaff.Text))
            {
                MessageBox.Show("Vui lòng điền Tên nhân viên", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else if (!Regex.IsMatch(txbEmailStaff.Text, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                MessageBox.Show("Email không hợp lệ. Vui lòng nhập địa chỉ email của Gmail", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (string.IsNullOrWhiteSpace(txbPhoneStaff.Text))
            {
                MessageBox.Show("Vui lòng không để trống SĐT", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (txbPhoneStaff.Text.Length != 10)
            {
                MessageBox.Show("SĐT không hợp lệ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (cbSexStaff.Text == String.Empty)
            {
                MessageBox.Show("Vui lòng lựa chọn giới tính", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (txbPassStaff.Text == String.Empty)
            {
                MessageBox.Show("Vui lòng nhập mật khẩu", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (string.IsNullOrWhiteSpace(txbIdCard.Text))
            {
                MessageBox.Show("Vui lòng quét thẻ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string email = txbEmailStaff.Text;
                string name = txbNameStaff.Text;
                int sex = 0;
                if (cbSexStaff.Text == "Nam")
                {
                    sex = 1;
                }

                string PassWord = txbPassStaff.Text;
                int phone = Convert.ToInt32(txbPhoneStaff.Text);
                string idCard = txbIdCard.Text;
                try
                {
                    InsertAdmin(email, name, sex, PassWord, phone, 0, arr, idCard);
                    MessageBox.Show("Đăng kí tài khoản nhân viên thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Lỗi khi thêm dữ liệu vui lòng liên hệ bộ phận kỹ thuật! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



            }
        }

        private void txbPassStaff_TextChange_1(object sender, EventArgs e)
        {
            txbPassStaff.PasswordChar = '*';
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine(); // Read the received data

            // Update the textbox with the received data
            BeginInvoke(new Action(() =>
            {
                idcard = data;
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

        private void txbIdCard_TextChange(object sender, EventArgs e)
        {
            txbIdCard.PasswordChar = '*';
           
        }

        private void txbIdCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txbEmailStaff_Leave(object sender, EventArgs e)
        {
            string text = txbEmailStaff.Text.Trim();
            bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), text);

            if (isMatch && !String.IsNullOrEmpty(text))
            {
                MessageBox.Show("Email đã tồn tại. Vui lòng nhập email khác!" , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txbEmailStaff.Clear();
            }
        }

        private void txbPhoneStaff_Leave(object sender, EventArgs e)
        {
            string phone = txbPhoneStaff.Text.Trim();
            int firstDigitIndex = phone.IndexOfAny("0123456789".ToCharArray());

            if (firstDigitIndex >= 0)
            {
                string substring = phone.Substring(firstDigitIndex + 1);
                bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), substring);

                if (isMatch)
                {
                    MessageBox.Show("SĐT đã tồn tại. Vui lòng nhập SĐT khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txbPhoneStaff.Clear();
                }

            }
        }

        private void txbIdCard_Leave(object sender, EventArgs e)
        {
            string text = txbIdCard.Text.Trim();
            bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), text);

            if (isMatch && !String.IsNullOrEmpty(text))
            {
                MessageBox.Show("Mã thẻ đã tồn tại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txbIdCard.Clear();
            }
        }

    }
}
