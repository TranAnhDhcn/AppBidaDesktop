using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using static System.Net.WebRequestMethods;

namespace WindowsFormsAppBida
{
    public partial class fForgetPass : Form
    {
        private string email;
        private int otp;
        private DateTime otpExpirationTime;
        public fForgetPass()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;


        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            string pass = txbPassAccNew.Text;
            email = txbEmailAcc.Text ;
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập Email tài khoản", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (pass == string.Empty )
            {
                MessageBox.Show("Vui lòng nhập mật khẩu tài khoản", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (pass != txbRePassAcc.Text)
            {
                MessageBox.Show("Mật khẩu nhập không khớp", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if(UpdatePassAcount(email, pass))
                {
                    MessageBox.Show("Đổi mật khẩu thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lỗi thiết lập mật khẩu. Vui lòng thử lại sau", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            
        }

        bool UpdatePassAcount(string email, string pass)
        {
            return AccountDAO.Instance.UpdatePassAcount(email, pass);
        }

        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
            SendEmail();
        }

        void SendEmail()
        {
            Random rnd = new Random();
            otp = rnd.Next(100000, 999999);
            otpExpirationTime = DateTime.Now.AddMinutes(5);

            string to, from, pass, mail;
            to = txbEmailAcc.Text;
            from = "atdgroup.2023@gmail.com";
            mail = otp.ToString();
            pass = "kiuogdxmbrtymgyg";
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Body = "Sử dụng mã này để hoàn tất việc thiết lập mật khẩu: \n " + mail + "\n Mã xác minh sẽ có hiệu lực trong vòng 5 phút.";
            message.Subject = "Mã xác minh cho đặt lại mật khẩu BidaApp";
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);
            try
            {
                smtp.Send(message);
                MessageBox.Show("Mã xác minh đã được gửi tới Email tài khoản có thể bị ẩn trong thư mục Spam. \n Vui lòng nhập mã xác minh để hoàn tất việc thiết lập.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex);
            }
        }

        private void txbOtp_TextChange(object sender, EventArgs e)
        {
            if (txbOtp.Text.Length == 6) // Wait until 6 digits are entered
            {
                int userOtp;
                if (int.TryParse(txbOtp.Text, out userOtp))
                {
                    if (userOtp == otp)
                    {
                        txbPassAccNew.Enabled = true;
                        txbRePassAcc.Enabled = true;
                        bunifuButton21.Enabled = true;
                        txbOtp.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Mã xác minh không đúng. Vui lòng kiểm tra lại Email hoặc mã xác minh", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập mã xác minh 6 số", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Clear the textbox after checking the OTP
                txbOtp.Text = string.Empty;
            }
        }

        private void timevcode_Tick(object sender, EventArgs e)
        {
            // Check if the OTP has expired
            if (DateTime.Now > otpExpirationTime)
            {
                MessageBox.Show("Mã xác minh đã hết hiệu lực. Vui lòng lấy mã xác minh mới để thiết lập mật khẩu!","Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Clear the OTP and reset the expiration time
                otp = 0;
                otpExpirationTime = DateTime.MinValue;
            }
        }

        private void txbPassAccNew_TextChange(object sender, EventArgs e)
        {
            txbPassAccNew.PasswordChar = '*';
        }

        private void txbRePassAcc_TextChange(object sender, EventArgs e)
        {
            txbRePassAcc.PasswordChar = '*';
        }
    }
}
