using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsAppBida.DAO;

namespace WindowsFormsAppBida.DTO
{
    public class BillInfo
    {
        //private static BillDAO instance;

        public BillInfo(int id, int foodID, int billID, int count ) 
        {
            this.ID = id;
            this.BillID = billID;
            this.Count = count;
            this.FoodID = foodID;
        }

        public BillInfo(DataRow row)
        {
            this.ID = (int)row["id"];
            this.BillID = (int)row["idbill"];
            this.FoodID = (int)row["idfood"];
            this.Count = (int)row["idcount"];
        }

        private int foodID;
        private int count;
        private int billID;   
        private int iD;


        public int ID { get => iD; set => iD = value; }
        public int FoodID { get => foodID; set => foodID = value; }
        public int Count { get => count; set => count = value; }
        public int BillID { get => billID; set => billID = value; }
    }
}
