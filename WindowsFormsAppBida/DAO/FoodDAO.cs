using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;

namespace WindowsFormsAppBida.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance
        {
            get { if (instance == null) instance = new FoodDAO(); return FoodDAO.instance; }
            private set { FoodDAO.instance = value; }
        }

        private FoodDAO() { }

        public List<Food> GetFoodByCategoryID(int id)
        {
            List<Food> list = new List<Food>();

            string query = "select * from Food where idCategory = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }

            return list;
        }

        public List<Food> GetListFood()
        {
            List<Food> list = new List<Food>();

            string query = "SELECT f.* FROM Food f JOIN FoodCategory fc ON f.idCategory = fc.id WHERE fc.active = 1";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }

            return list;
        }





        public List<Food> SearchFoodByName(string  name)
        {
            List<Food> list = new List<Food>();

            string query = string.Format("SELECT * FROM Food f JOIN FoodCategory fc ON f.idCategory = fc.id WHERE fc.active = 1 AND dbo.Search(f.name) like N'%' + dbo.Search(N'{0}') + '%'", name);

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }

            return list;
        }



        public bool InsertFood(string name, int id, int salary, decimal price, byte[] image)
        {
            string query = "INSERT INTO Food (name, timeIn, salary, idCategory, price, image) VALUES ( @name , GETDATE() , @salary , @idCategory , @price , @image )";

            object[] parameters = { name, salary, id, price, image };

            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }


        //public bool UpdateFood(int idFood, string name, int id,int salary, float price, byte[] image)
        //{
        //    string query = string.Format("UPDATE dbo.Food SET name = N'{0}', idCategory = {1}, price = {2}, salary = {3}, image = {4} WHERE id = {5}", name, id, price, salary, idFood, image);
        //    int result = DataProvider.Instance.ExecuteNonQuery(query);

        //    return result > 0;
        //}

        public bool UpdateFood(int foodId, string name, int id, int salary, decimal price, byte[] image)
        {
            string query = "UPDATE Food SET name = @name , timeIn = GETDATE() , salary = @salary , idCategory = @idCategory , price = @price , image = @image WHERE id = @foodId";

            object[] parameters = { name, salary, id, price, image, foodId };

            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);
            return result > 0;
        }


        public bool DeleteFood(int idFood)
        {
            BillInfoDAO.Instance.DeleteBillInfoByFoodId(idFood);
            string query = string.Format("Delete Food where id = {0}", idFood);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public int GetSalaryFood(int id )
        {
            return(int) DataProvider.Instance.ExecuteScalar("select salary from Food where id = " +id);
        }

        public void UpdateSalry(int salary, int id)
        {
            DataProvider.Instance.ExecuteNonQuery("UPDATE Food SET salary = "+salary+" WHERE id = " + id);
        }

        
    }
}
