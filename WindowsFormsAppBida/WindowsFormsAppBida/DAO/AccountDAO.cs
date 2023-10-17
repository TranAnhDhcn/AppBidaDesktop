using OracleInternal.Secure.Network;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsAppBida.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }
        }

        private AccountDAO() { }

        public List<Account> LoadAccountList()
        {
            List<Account> accountList = new List<Account>();

            DataTable data = DataProvider.Instance.ExecuteQuery("select * from Account where active  = 1");

            foreach (DataRow item in data.Rows)
            {
                Account account = new Account(item);
                accountList.Add(account);
            }

            return accountList;
        }


        public bool Login(string email, string passWord)
        {
            byte[] temp = Encoding.UTF8.GetBytes(passWord);
            byte[] hasData = new SHA256CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item.ToString("x2"); // Chuyển đổi thành chuỗi hexa để lưu trữ giá trị mã hóa
            }
            string query = "EXEC USP_Login   @passWord ,   @email ";

            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { hasPass , email });

            return result.Rows.Count > 0;
        }

        public bool LoginCard(string email, string idCard)
        {
            string query = "EXEC USP_LoginCard   @idCard ,   @email ";

            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { idCard, email });

            return result.Rows.Count > 0;
        }

        public void updateStatusAcc(string email,  int status)
        {
            string query = "UPDATE Account SET status = "+ status +" WHERE email = N'" + email + "'  ";
            DataTable result = DataProvider.Instance.ExecuteQuery(query);
        }

        public int GetIdAccount()
        {
            return (int)DataProvider.Instance.ExecuteScalar("SELECT id FROM Account WHERE status = 1");
        }

        public bool UpdateAccount(string name, string email, int phone, string pass, byte[] image, int id)
        {
            byte[] temp = Encoding.UTF8.GetBytes(pass);
            byte[] hasData = new SHA256CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item.ToString("x2"); // Chuyển đổi thành chuỗi hexa để lưu trữ giá trị mã hóa
            }
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount  @name , @email , @phone , @pass , @image , @id", new object[] { name, email, phone, hasPass, image, id });

            return result > 0;
        }

        public bool UpdateInFoAccount(string name, string email, int phone , byte[] image, int id)
        {

            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateInFoAccount  @name , @email , @phone , @image , @id", new object[] { name, email, phone, image, id });

            return result > 0;
        }

        public bool UpdateInFoPassAccount(string pass, int id)
        {
            byte[] temp = Encoding.UTF8.GetBytes(pass);
            byte[] hasData = new SHA256CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item.ToString("x2"); // Chuyển đổi thành chuỗi hexa để lưu trữ giá trị mã hóa
            }

            int result = DataProvider.Instance.ExecuteNonQuery("EXEC USP_UpdateInFoPassAccount @pass , @id ", new object[] { hasPass, id });

            return result > 0;
        }

        public bool UpdateInFoIdCard(string idCard, int id)
        {

            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateInFoIdCard  @idCard , @id", new object[] { idCard, id });

            return result > 0;
        }

        public DataTable GetListAccountInfo(string name)
        {
            return DataProvider.Instance.ExecuteQuery("SELECT  * from dbo.Account where active = 1 AND name = N'"+ name +"'");
        }

        public bool UpdateAccountStaff(string email, string name, int phone, int id )
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccountStaff  @email , @name , @phone , @id", new object[] { email, name, phone , id});

            return result > 0;
        }



        public Account GetAccountByUserEmail(string email)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * from account where active = 1 AND email = '" + email + "'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("SELECT id, name ,email,phone, dateW from dbo.Account where  Type = 0 and active = 1 ");
        }

        public bool InsertAdmin(string email, string name, int sex, string PassWord, int phone, int type, byte[] image, string idCard)
        {
            byte[] temp = Encoding.UTF8.GetBytes(PassWord);
            byte[] hasData = new SHA256CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item.ToString("x2"); // Chuyển đổi thành chuỗi hexa để lưu trữ giá trị mã hóa
            }

            string query = "USP_InsertAdminAcc @email , @name , @sex , @PassWord , @phone , @type , @image , @idCard";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { email, name, sex, hasPass, phone, type, image , idCard});

            return result > 0;
        }





        public bool UpdateActiveAccountStaff(int id)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("UPDATE Account SET active = 0 where id = " + id);
            return result > 0;
        }

        public bool UpdatePassAcount(string email, string PassWord)
        {
            byte[] temp = Encoding.UTF8.GetBytes(PassWord);
            byte[] hasData = new SHA256CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item.ToString("x2"); // Chuyển đổi thành chuỗi hexa để lưu trữ giá trị mã hóa
            }

            string query = "USP_UpdatePassAccount @email , @pass";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { email, hasPass});

            return result > 0;
        }
    }
}
