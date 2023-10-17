using Bunifu.UI.WinForms;
using iTextSharp.text;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;

namespace WindowsFormsAppBida
{
    public partial class fInfoAccount : Form
    {
        private string name;
        private DataTable accountTable;
        string imgUrl;

        public fInfoAccount(string name)
        {
            InitializeComponent();
            this.name = name;
            showInFo();
            hideTool();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        string passOld;
        void showInFo()
        {
            lbNameInfoAcc.Text = name;
            accountTable = AccountDAO.Instance.GetListAccountInfo(name);
            txbIdInfoAcc.Text = accountTable.Rows[0]["id"].ToString();
            txbNameInfoAcc.Text = accountTable.Rows[0]["name"].ToString();
            txbEmailInfoAcc.Text = accountTable.Rows[0]["email"].ToString();
            txbPhoneInfoAcc.Text = accountTable.Rows[0]["phone"].ToString();
            passOld = accountTable.Rows[0]["PassWord"].ToString();          
            byte[] img = (byte[])accountTable.Rows[0]["image"];

            if (accountTable.Rows[0]["type"].ToString() == "1")
            {
                lbTypeInfoAcc.Text = "Quản lý";
            }
            else
            {
                lbTypeInfoAcc.Text = "Nhân viên";
            }

            if (accountTable.Rows[0]["sex"].ToString() == "1")
            {
                txbSexInfoAcc.Text = "Nam";
            }
            else
            {
                txbSexInfoAcc.Text = "Nữ";
            }

            txbDateInfoAcc.Text = accountTable.Rows[0]["dateW"].ToString();

            using (MemoryStream ms = new MemoryStream(img))
            {
                bunifuPictureBox1.Image = System.Drawing.Image.FromStream(ms);
            }
        }

        void hideTool()
        {
            txbPassInfoAcc.Visible = false;
            txbNewPass.Visible = false;
            txbIdCard.Visible = false;
            bunifuLabel6.Visible = false;
            bunifuLabel8.Visible = false;
            bunifuLabel9.Visible = false;
        }
        public bool CheckIfNameOrEmailExistsAccount(List<Account> accountList, string textboxValue)
        {
            string normalizedTextboxValue = NormalizeText(textboxValue);

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
                    return true; // Match found
                }
            }

            return false; // No match found
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

        string emailCurrent;
        private void txbEmailInfoAcc_Click(object sender, EventArgs e)
        {
            emailCurrent = txbEmailInfoAcc.Text;
        }

        private void txbEmailInfoAcc_Leave(object sender, EventArgs e)
        {
            if (emailCurrent != txbEmailInfoAcc.Text)
            {
                bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), txbEmailInfoAcc.Text);
                if (isMatch)
                {
                    MessageBox.Show("Email đã tồn tại. Vui lòng nhập email khác!");
                    txbEmailInfoAcc.Clear();
                }
            }
        }


        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imgUrl = ofd.FileName;
                    bunifuPictureBox1.Image = System.Drawing.Image.FromFile(ofd.FileName);
                }
            }
        }

        private void txbPassInfoAcc_TextChange(object sender, EventArgs e)
        {
            txbPassInfoAcc.PasswordChar = '*';

        }


        string phoneCurrent;
        private void txbPhoneInfoAcc_Click(object sender, EventArgs e)
        {
            phoneCurrent = "0"+txbPhoneInfoAcc.Text;
        }

        private void txbPhoneInfoAcc_Leave(object sender, EventArgs e)
        {
            if (phoneCurrent != txbPhoneInfoAcc.Text)
            {
                string phone = txbPhoneInfoAcc.Text.Trim();
                int firstDigitIndex = phone.IndexOfAny("0123456789".ToCharArray());

                if (firstDigitIndex >= 0)
                {
                    string substring = phone.Substring(firstDigitIndex + 1);
                    bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), substring);

                    if (isMatch)
                    {
                        MessageBox.Show("SĐT đã tồn tại. Vui lòng nhập SĐT khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txbPhoneInfoAcc.Clear();
                    }

                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txbNewPass_TextChange(object sender, EventArgs e)
        {
            txbNewPass.PasswordChar = '*';
        }

        private void checkBoxUpdateIdCard_CheckedChanged(object sender, EventArgs e)
        {
            hideTool();
            if (checkBoxUpdateIdCard.Checked)
            {
                checkBoxUpdatePass.Checked = false;
                checkBoxUpdateInfo.Checked = false;
                checkBoxUpdateIdCard.Checked = true;
                txbPassInfoAcc.Visible = true;
               
                txbIdCard.Visible = true;

                bunifuLabel6.Visible = true;
              
                bunifuLabel9.Visible = true;
                txbNameInfoAcc.Enabled = false;
                txbPhoneInfoAcc.Enabled = false;
                txbEmailInfoAcc.Enabled = false;
                bunifuPictureBox1.Enabled = false;
            }
        }

        private void checkBoxUpdateInfo_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxUpdateInfo.Checked)
            {
                checkBoxUpdatePass.Checked = false;
                checkBoxUpdateInfo.Checked = true;
                checkBoxUpdateIdCard.Checked = false;

                txbNameInfoAcc.Enabled = true;
                txbPhoneInfoAcc.Enabled = true;
                txbEmailInfoAcc.Enabled = true;
                bunifuPictureBox1.Enabled = true;
                hideTool();
            }
        }

        private void checkBoxUpdatePass_CheckedChanged(object sender, EventArgs e)
        {
            hideTool();
            if (checkBoxUpdatePass.Checked)
            {
                checkBoxUpdatePass.Checked = true;
                checkBoxUpdateInfo.Checked = false;
                checkBoxUpdateIdCard.Checked = false;

                txbPassInfoAcc.Visible = true;
                txbNewPass.Visible = true;
               

                bunifuLabel6.Visible = true;
                bunifuLabel8.Visible = true;

                txbNameInfoAcc.Enabled = false;
                txbPhoneInfoAcc.Enabled = false;
                txbEmailInfoAcc.Enabled = false;
                bunifuPictureBox1.Enabled = false;


            }
        }

        private void txbIdCard_Leave(object sender, EventArgs e)
        {
            string text = txbIdCard.Text.Trim();
            bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), text);

            if (isMatch && !String.IsNullOrEmpty(text))
            {
                MessageBox.Show("Mã thẻ đã tồn tại!. Vui lòng sử dụng thẻ khác", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbIdCard.Clear();
            }
        }

        private void txbIdCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txbIdCard_TextChange(object sender, EventArgs e)
        {
            txbIdCard.PasswordChar = '*';
        }

        private void btnSumit_Click_1(object sender, EventArgs e)
        {
            System.Drawing.Image img = bunifuPictureBox1.Image;
            byte[] arr;
            try
            {
                ImageConverter converter = new ImageConverter();
                arr = (byte[])converter.ConvertTo(img, typeof(byte[]));
 
            }
            catch
            {
                arr = (byte[])accountTable.Rows[0]["image"];             
            }



            byte[] temp = Encoding.UTF8.GetBytes(txbPassInfoAcc.Text);
            byte[] hasData = new SHA256CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item.ToString("x2"); // Chuyển đổi thành chuỗi hexa để lưu trữ giá trị mã hóa
            }


            if (string.IsNullOrWhiteSpace(txbNameInfoAcc.Text))
            {
                MessageBox.Show("Vui lòng không để trống tên", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(txbEmailInfoAcc.Text))
            {
                MessageBox.Show("Vui lòng không để trống email", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(txbPhoneInfoAcc.Text))
            {
                MessageBox.Show("Vui lòng không để trống SĐT", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }  
            else
            {
                string name = txbNameInfoAcc.Text;
                string email = txbEmailInfoAcc.Text;
                int phone = int.Parse(txbPhoneInfoAcc.Text);
                int id = int.Parse(txbIdInfoAcc.Text);
                string pass = txbNewPass.Text;

                if (checkBoxUpdateInfo.Checked)
                {

                    if (AccountDAO.Instance.UpdateInFoAccount(name, email, phone, arr, id))
                    {
                        MessageBox.Show("Sửa thông tin tài khoản thành công. Vui lòng đăng nhập lại để sử dụng hệ thống!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi cập nhật thông tin. Vui lòng thử lại sau ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

                if (checkBoxUpdatePass.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txbPassInfoAcc.Text))
                    {
                        MessageBox.Show("Vui lòng không để trống mật khẩu", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (string.IsNullOrWhiteSpace(txbNewPass.Text))
                    {
                        MessageBox.Show("Vui lòng không để trống mật khẩu mới", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    else if (passOld != hasPass)
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng. Vui lòng nhập lại", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (AccountDAO.Instance.UpdateInFoPassAccount(txbNewPass.Text, id))
                        {
                            MessageBox.Show("Thay đổi mật khẩu thành công. Vui lòng đăng nhập lại để sử dụng hệ thống!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Lỗi thay đổi mật khẩu. Vui lòng thử lại sau ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }

                if (checkBoxUpdateIdCard.Checked)
                {
                    if (passOld != hasPass)
                    {
                        MessageBox.Show("Mật khẩu không đúng. Vui lòng nhập lại", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (string.IsNullOrWhiteSpace(txbIdCard.Text))
                    {
                        MessageBox.Show("Mã thẻ đang trống. Vui lòng quét thẻ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    }
                    else
                    {
                        if (AccountDAO.Instance.UpdateInFoIdCard(txbIdCard.Text, id))
                        {
                            MessageBox.Show("Thay đổi thẻ thành công. Vui lòng đăng nhập lại để sử dụng hệ thống!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Lỗi thay đổi thẻ. Vui lòng thử lại sau ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

            }
        }
    }
}
