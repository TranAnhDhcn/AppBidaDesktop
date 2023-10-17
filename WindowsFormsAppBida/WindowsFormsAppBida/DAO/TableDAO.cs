using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsAppBida.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }
        }

        public static int TableWidth = 190;
        public static int TableHeight = 190;

        private TableDAO() { }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public List<Table> LoadTableListNom()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableListNom");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public List<Table> LoadTableListVip()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableListVip");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }



        public void OnStatusTable(int id )
        {
            DataProvider.Instance.ExecuteNonQuery("UPDATE TableBida SET status = N'Có người' WHERE id = " + id);
        }

        public Table GetTableById(int id)
        {
            Table table = null;
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM TableBida WHERE active = 1 AND id = " + id);

            if (data.Rows.Count > 0)
            {
                DataRow item = data.Rows[0];
                table = new Table(item);
            }

            return table;
        }



        public void CheckTable(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("UPDATE TableBida SET classification = 'Nom', status = N'Trống' WHERE id = " + id);

        }
        public void SwitchTable(int id1, int id2)
        {

            DataProvider.Instance.ExecuteQuery("USP_SwapTable  @idTable1 , @idTabel2", new object[] { id1, id2 });
            StartServer.Instance.offLed((byte)id1);
            Thread.Sleep(2000);
            StartServer.Instance.onLed((byte)id2);


            //DataProvider.Instance.ExecuteQuery("USP_USP_SwitchTabel  @idTable1 , @idTabel2", new object[] { id1, id2 });
        }


        public bool InsertTable(string name,string classification)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("insert into TableBida (name, classification) VALUES (N'" + name + "', N'" + classification +"')");
            return result > 0;
        }

        public bool UpdateActiveTable(int id)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("UPDATE TableBida SET active = 0 WHERE id =" + id);
            return result > 0;
        }

        public byte[] LoadTableIdArray()
        {
            List<byte> idList = new List<byte>();

            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT id FROM dbo.TableBida where active = 1 and statusLoraMesh = 1");

            foreach (DataRow item in data.Rows)
            {
                byte id = Convert.ToByte(item["id"]);
                idList.Add(id);
            }

            return idList.ToArray();
        }




        public bool UpdateNameTable(int id, string name, string classification)
        {
            string query = string.Format("update TableBida set name = N'{0}', classification = N'{1}' where id = {2}", name, classification, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public int getStatusLoraMesh(int id)
        {
            return(int) DataProvider.Instance.ExecuteScalar("select statusLoraMesh from TableBida where active = 1 and id = " + id);
        }

        public void UpdateStatusLoraMeshTable(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("update TableBida set statusLoraMesh = 1 where active = 1 AND id = " + id);
        }
    }
}
