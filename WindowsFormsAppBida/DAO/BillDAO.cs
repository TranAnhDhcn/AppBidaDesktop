 using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsAppBida.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return BillDAO.instance; }
            private set { BillDAO.instance = value; }
        }

        private BillDAO() { }

        /// <summary>
        /// Thành công: bill ID
        /// thất bại: -1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUncheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Bill WHERE idTable = " + id + " AND status = 0");

            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }

            return -1;
        }

        public void CheckOut(int id, int discount, float totalPrice)
        {
            string query = "UPDATE dbo.Bill SET dateCheckOut = '" + DateTime.Now + "', status = 1, " + "discount = " + discount + ", totalPrice = " + totalPrice + " WHERE id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }
        public void InsertBill(DateTime dateCheckIn, int id, int idAccount)
        {
            DataProvider.Instance.ExecuteNonQuery("EXEC USP_InsertBill @dateCheckIn , @idTable , @idAccount", new object[] {dateCheckIn, id, idAccount });
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }

        public DataTable GetListFood()
        {
            return DataProvider.Instance.ExecuteQuery("Select * from Food");
        }


        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("SELECT MAX(id) FROM dbo.Bill");
            }
            catch
            {
                return 0;
            }
        }

        public int GetIdBill(int id)
        {
            return(int) DataProvider.Instance.ExecuteScalar("select id from Bill where status = 0 and idTable = " + id);
            
        }

        public DateTime GetDateCheckIn(int id)
        {
            return (DateTime) DataProvider.Instance.ExecuteScalar("select DateCheckIn from Bill where status = 0 and idTable = " + id);

        }

        public DataTable GetTotalRevenueDay(int date, int month)
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT COUNT(*) as BillCountToday, SUM(totalPrice) as TotalPriceToday FROM Bill WHERE DAY(DateCheckIn) = "+ date +" AND MONTH(DateCheckIn) = "+month+"");
            return dt;
        }

        public DataTable GetTotalRevenueWeek()
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT COUNT(*) as BillCountWeek, SUM(totalPrice) as TotalPriceWeek\r\nFROM Bill\r\nWHERE DateCheckIn >= DATEADD(wk, DATEDIFF(wk,0,GETDATE()), 0)");
            return dt;
        }


        public DataTable GetTotalRevenueMonth(int month)
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT COUNT(*) as BillCountMonth, SUM(totalPrice) as TotalPriceMonth\r\nFROM Bill\r\nWHERE MONTH(DateCheckIn) = " + month);
            return dt;
        }

        public DataTable GetTotalRevenueYear(int year)
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT COUNT(*) as BillCountYear, SUM(totalPrice) as TotalPriceYear\r\nFROM Bill\r\nWHERE YEAR(DateCheckIn) = " + year);
            return dt;
        }


        public DataTable GetTotalRevenue(String dateIn, String dateOut)
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT COUNT(*) AS TotalBills, SUM(totalPrice) AS TotalPrice FROM Bill WHERE DateCheckIn >= '"+dateIn+"' AND (DateCheckOut < DATEADD(day, 1, '"+dateOut+"') OR DateCheckOut IS NULL)");
            return dt;
        }
        public DataTable GetTotalRevenueInEqualOut(String dateIn)
        {
            DataTable dt = DataProvider.Instance.ExecuteQuery("SELECT COUNT(*) AS TotalBills, SUM(totalPrice) AS TotalPrice FROM Bill WHERE DateCheckIn >=' " + dateIn + " ' ");
            return dt;
        }
    }
}
