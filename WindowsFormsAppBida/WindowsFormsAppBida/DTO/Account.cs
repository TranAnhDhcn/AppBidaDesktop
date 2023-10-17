using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppBida.DTO
{
    public class Account
    {
        public Account(string email, string name, int sex,  int phone, int type, DateTime? datew, int status, byte[] image, string idCard ,string passWord = null) 
        {
            this.Email = email;
            this.Name = name;
            this.Sex = sex;
            this.PassWord1 = passWord;
            this.Phone = phone;
            this.Type = type;
            this.DateW = DateW;
            this.Status = status;
            this.Image = image;
            this.IdCard = idCard;
           
        }

        public Account(DataRow row)
        {
            this.Email = row["email"].ToString();
            this.Name = row["name"].ToString();
            this.Sex = (int)row["sex"];
            this.PassWord1 = row["passWord"].ToString();
            this.Phone = (int)row["phone"];
            this.Type = (int)row["type"];
            this.DateW = (DateTime?)row["dateW"];
            this.Status = (int)row["status"];
            object imageValue = row["image"];
            if (imageValue != DBNull.Value)
            {
                this.Image = (byte[])imageValue;
            }
            else
            {
                this.Image = null; // or assign a default byte array if necessary
            }
            this.IdCard = row["idCard"].ToString();
        }
        private string idCard;
        private string email;
        private string name;
        private int sex;
        private string PassWord;
        private int phone;
        private int type;
        private DateTime? dateW;
        private int status;
        private byte[] image;
        public string Email { get => email; set => email = value; }
        public string Name { get => name; set => name = value; }
        public int Sex { get => sex; set => sex = value; }
        public int Phone { get => phone; set => phone = value; }

        public int Status { get => status; set => status = value; }
        public int Type { get => type; set => type = value; }
        public DateTime? DateW { get => dateW; set => dateW = value; }
        public string PassWord1 { get => PassWord; set => PassWord = value; }
        public byte[] Image { get => image; set => image = value; }
        public string IdCard { get => idCard; set => idCard = value; }
    }
}
