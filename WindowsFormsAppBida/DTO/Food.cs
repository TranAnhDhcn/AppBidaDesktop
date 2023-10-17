using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppBida.DTO
{
    public class Food
    {
        public Food(int id, string name,DateTime? timeIn, int salary, int categoryID, float price,byte[] image)
        {
            this.ID = id;
            this.Name = name;
            this.CategoryID = categoryID;
            this.Price = price;
            this.TimeIn = timeIn;
            this.Salary = salary;
            this.Image = image;
           
        }

        public Food(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name"].ToString();
            this.TimeIn = (DateTime?)row["timeIn"];
            this.Salary = (int)row["salary"];
            this.CategoryID = (int)row["idcategory"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
            object imageValue = row["image"];
            if (imageValue != DBNull.Value)
            {
                this.Image = (byte[])imageValue;
            }
            else
            {
                this.Image = null; // or assign a default byte array if necessary
            }
        }


        private DateTime? timeIn;

        private byte[] image;
        

        private int salary;
        private float price;

        public float Price
        {
            get { return price; }
            set { price = value; }
        }

        private int categoryID;

        public int CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int iD;

        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        public int Salary { get => salary; set => salary = value; }
        public DateTime? TimeIn { get => timeIn; set => timeIn = value; }
        public byte[] Image { get => image; set => image = value; }
    }
}
