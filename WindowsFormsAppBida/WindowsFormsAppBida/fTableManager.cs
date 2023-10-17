using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using WindowsFormsAppBida;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Data.SqlClient;
using System.Collections;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Drawing.Design;
using Org.BouncyCastle.Asn1.Cmp;
using Newtonsoft.Json.Linq;
using static WindowsFormsAppBida.fLogin;
using Timer = System.Windows.Forms.Timer;
using static Bunifu.UI.WinForms.Helpers.Transitions.Transition;
using static WindowsFormsAppBida.DAO.StartServer;
using System.ComponentModel.Design;
using System.Security.Principal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using System.Reflection;
using static MetroFramework.Drawing.MetroPaint.BorderColor;
using System.IO;

namespace WindowsFormsAppBida
{
    public partial class fTableManager : Form
    {
        private int idTemp;

        //private static fTableManager instance;

        private Account loginAccount;
        CultureInfo culture = new CultureInfo("vi-VN");
        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        //public static fTableManager Instance { get => instance; set => instance = value; }


        public fTableManager(Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc;
            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbSwitchTable);
            
            showAccount();
            StartServer.Instance.OnMyEventOnLed += StartServer_OnMyEventOnLed;
            checkListTable();

            additionFoodCount.Visible = true;
            additionFoodCount.Visible = true;

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }



        #region Method
        void ChangeAccount(int type)
        {
            bunifuBtnAdmin.Enabled = type == 1;
            txbThongTinTaiKhoanHD.Text = LoginAccount.Name;

            if (LoginAccount.Image != null && LoginAccount.Image.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(LoginAccount.Image))
                {
                    imgAvatarAcc.Image = Image.FromStream(ms);
                }
            }
            else
            {
                imgAvatarAcc.Image = Properties.Resources.noimage;
            }
        }


        void showAccount()
        {
            //txbThongTinTaiKhoanHD.Text = AppSession.CurrentAccount.Name ;            
        }
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }


        public void LoadTable()
        {
            flpTable.Controls.Clear();

            // List<Table> tableList = TableDAO.Instance.LoadTableList();
            List<Table> tableList;
            if (checkBoxNom.Checked && checkBoxVip.Checked)
            {
                tableList = TableDAO.Instance.LoadTableList();
            }    

            else if (checkBoxVip.Checked)
            {
                tableList = TableDAO.Instance.LoadTableListVip();
            }

            else if (checkBoxNom.Checked )
            {
                tableList = TableDAO.Instance.LoadTableListNom();
            }

            else
            {
                tableList = TableDAO.Instance.LoadTableList();
            }

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.LightPink;
                        break;
                }

                flpTable.Controls.Add(btn);

            }

        }

        void checkListTable()
        {
            List<Table> tableList = TableDAO.Instance.LoadTableList();
            if (tableList.Count == 0)
            {
                additionFoodCount.Enabled = false;
                subtractionFoodCount.Enabled = false;
                btnCheckOut.Enabled = false;
                txbDisCount.Enabled = false;
                btnAddTable.Enabled = false;
                btnSwitchTable.Enabled = false;
                bunifuBtnAuto.Enabled = false;
                subtractionFoodCount.Enabled = false;
                additionFoodCount.Enabled = false;
            }

        }

        //void ShowBill(int id)
        //{
        //    lsvBill.Items.Clear();
        //    List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
        //    float totalPrice = 0;
        //    foreach (DTO.Menu item in listBillInfo)
        //    {
        //        ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
        //        lsvItem.SubItems.Add(item.Count.ToString());
        //        lsvItem.SubItems.Add(item.Price.ToString());
        //        lsvItem.SubItems.Add(item.TotalPrice.ToString());
        //        totalPrice += item.TotalPrice;
        //        lsvBill.Items.Add(lsvItem);
        //    }
        //    CultureInfo culture = new CultureInfo("vi-VN");

        //    //Thread.CurrentThread.CurrentCulture = culture;

        //    txbTotalPrice.Text = totalPrice.ToString("c", culture);
        //}

        void ShowBill(int id)
        {
            int index = 0;
            lsvBill1.Rows.Clear();
            List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            //float totalPrice = 0;
            foreach (DTO.Menu item in listBillInfo)
            {
                DataGridViewRow dgvRow = new DataGridViewRow();
                dgvRow.CreateCells(lsvBill1);
                dgvRow.Cells[0].Value = ++index;
                dgvRow.Cells[1].Value = item.FoodName.ToString();
                dgvRow.Cells[2].Value = item.Count.ToString();
                dgvRow.Cells[3].Value = item.Price.ToString();
                dgvRow.Cells[4].Value = item.TotalPrice.ToString();

                //totalPrice += item.TotalPrice;

                lsvBill1.Rows.Add(dgvRow);
            }

            //CultureInfo culture = new CultureInfo("vi-VN");
            //txbTotalPrice.Text = totalPrice.ToString("c", culture);
        }


        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill1.Tag = (sender as Button).Tag;

            ShowBill(tableID);

            ///////////////////////
            Table table = lsvBill1.Tag as Table;
            txbIdTableShow.Text = table.Name.ToString();
            idTemp = table.ID;
            ///////////////////////

            if (table.Status != "Trống")
            {
                string query = "SELECT  MAX(DateCheckIn) FROM Bill WHERE idTable =" + tableID;
                object dateTimeObject = DataProvider.Instance.ExecuteScalar(query);
                if (dateTimeObject != null)
                {
                    DateTime dateTime = (DateTime)dateTimeObject;
                    txbShowTimeTable.Text = dateTime.ToString("dd/MM/yy - HH:mm:ss");
                    additionFoodCount.Enabled = true;
                    subtractionFoodCount.Enabled = true;
                    btnAddTable.Enabled = false;
                    btnCheckOut.Enabled = true;
                }

            }
            else
            {
                txbShowTimeTable.Clear();
                additionFoodCount.Enabled = false;
                subtractionFoodCount.Enabled = false;
                btnCheckOut.Enabled = false;
                btnAddTable.Enabled = true;
            }

        }


        void AddFood(int count)
        {
            Table table = lsvBill1.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;

            if (idBill == -1)
            {
                MessageBox.Show("Vui lòng cấp bàn chơi !", "Thông báo");
                //BillDAO.Instance.InsertBill(table.ID);
                //BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
                ShowBill(table.ID);
                int salary;
                
                salary = FoodDAO.Instance.GetSalaryFood(foodID) - count;
               
                    //salary = FoodDAO.Instance.GetSalaryFood(foodID) - BillInfoDAO.Instance.GetCountBillInfo(idBill);
                    

                FoodDAO.Instance.UpdateSalry(salary, foodID);

            }

            
            //LoadTable();
        }

        #endregion


        #region Events




        private void bunifuBtnAdmin_Click(object sender, EventArgs e)
        {
           
            fAdmin f = new fAdmin();
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.DeleteFoodCategory += f_DeleteFoodCategory;
            f.UpdateFoodCategory += f_UpdateFoodCategory;
            f.InsertFoodCategory += f_InsertFoodCategory;
            f.InsertTable += f_InsertTable;
            f.DeleteTable += f_DeleteTable;
            f.UpdateTable += f_UpdateTable;
            f.ShowDialog();
        }

        

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //Task.Run(() => CheckFlag());

        }


        void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill1.Tag != null)
                ShowBill((lsvBill1.Tag as Table).ID);
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill1.Tag != null)
                ShowBill((lsvBill1.Tag as Table).ID);
            LoadTable();
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill1.Tag != null)
                ShowBill((lsvBill1.Tag as Table).ID);
        }

        void f_DeleteFoodCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        void f_UpdateFoodCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        void f_InsertFoodCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
        }

        void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }
         void f_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
        }


        private void autosendResetDevice()
        {
            byte[] idArray = TableDAO.Instance.LoadTableIdArray();
            foreach (byte idTable in idArray)
            {
                string query = string.Format("SELECT status FROM dbo.TableBida WHERE id = {0}", idTable);
                DataTable statusData = DataProvider.Instance.ExecuteQuery(query);
                string status = statusData.Rows[0]["status"].ToString();
                if (status == "Có người")
                {
                    StartServer.Instance.onLed(idTable);
                    Thread.Sleep(350);
                }
                else if (status == "Trống")
                {
                    StartServer.Instance.offLed(idTable);
                    Thread.Sleep(350);
                }
            }
        }
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }

        private void btnAddTable_Click_1(object sender, EventArgs e)
        {

            if (lsvBill1.Tag != null)
            {
                Table table = lsvBill1.Tag as Table;
                if (table != null)
                {
                    int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
                    int idAccount = AccountDAO.Instance.GetIdAccount();

                    StartServer.Instance.onLed((byte)table.ID);
                    BillDAO.Instance.InsertBill(DateTime.Now, table.ID, idAccount);
                    TableDAO.Instance.OnStatusTable(table.ID);

                    ShowBill(table.ID);

                    LoadTable();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn chơi");
            }



            //Table table = lsvBill1.Tag as Table;
            //int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            //int idAccount = AccountDAO.Instance.GetIdAccount();

            //StartServer.Instance.onLed((byte)table.ID);
            //BillDAO.Instance.InsertBill(DateTime.Now, table.ID, idAccount);
            //TableDAO.Instance.OnStatusTable(table.ID);
 
            //ShowBill(table.ID);

            //LoadTable();

        }

        private void btnSwitchTable_Click_1(object sender, EventArgs e)
        {

            int id1 = (lsvBill1.Tag as Table).ID;
            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            try
            {
                if (id1 == idTemp &&
                    (lsvBill1.Tag as Table).Status != "Trống"
                    && id2 != idTemp
                    && MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill1.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    TableDAO.Instance.SwitchTable(id1, id2);

                    LoadTable();

                }
            }
            catch 
            {
                MessageBox.Show("Vui lòng chọn bàn đang hoạt động");
            }

        }


        private void LoadTable1()
        {
            if (this.flpTable.InvokeRequired)
            {
                this.flpTable.Invoke(new MethodInvoker(LoadTable));
                return;
            }
        }




        private void StartServer_OnMyEventOnLed(object sender, EventArgs e)
        {
            LoadTable1();
 
        }


        private void btnAddFood_Click_1(object sender, EventArgs e)
        {
            Table table = lsvBill1.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                MessageBox.Show("Vui lòng cấp bàn chơi !", "Thông báo");
                //BillDAO.Instance.InsertBill(table.ID);
                //BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);
            LoadTable();
        }

        private void additionFoodCount_Click_1(object sender, EventArgs e)
        {
            int foodID = (cbFood.SelectedItem as Food).ID;

            if(FoodDAO.Instance.GetSalaryFood(foodID) != 0)
            {
                int count = (int)nmFoodCount.Value;
                AddFood(count);
            }    
            else
            {
                MessageBox.Show("Sản phẩm đã hết trong kho. Vui lòng nhập kho để sử dụng");
            }    
            
           
        }

        private void subtractionFoodCount_Click(object sender, EventArgs e)
        {
            string columnName = "Column2"; // Replace with the actual column name in the DataGridView
            string selectedItemName = (cbFood.SelectedItem as Food)?.Name; // Replace "Food" with your class name

                foreach (DataGridViewRow row in lsvBill1.Rows)
                {
                    if (row.Cells[columnName].Value != null && row.Cells[columnName].Value.ToString() == selectedItemName)
                    {
                        int count = (int)nmFoodCount.Value;
                        AddFood(count * (-1));
                    }
                    else
                    {
                        
                    }
                }
            




            //int count = (int)nmFoodCount.Value;
            //AddFood(count * (-1));
            
        }

        private void btnCheckOut_Click_1(object sender, EventArgs e)
        {
            Table table = lsvBill1.Tag as Table;
            string tableName = table.Name;
            int tableID = table.ID;
            int idBill = BillDAO.Instance.GetIdBill(table.ID);
            string classfication = table.Classification;
            string nameAccCheckOut = txbThongTinTaiKhoanHD.Text;
            txbDisCount.KeyPress += new KeyPressEventHandler(txbDisCount_KeyPress);

            int discount = 0;

            if (txbDisCount.Text != String.Empty)
            {
                discount = int.Parse(txbDisCount.Text);
            }
            else
            {
                discount = 0;
            }

            

            // Lấy thông tin bill
            List<string[]> data = new List<string[]>();

            foreach (DataGridViewRow row in lsvBill1.Rows)
            {
                string[] rowData = new string[3];
                rowData[0] = row.Cells[1].Value.ToString();
                rowData[1] = row.Cells[2].Value.ToString();
                rowData[2] = row.Cells[3].Value.ToString();
                data.Add(rowData);
            }

            // Lấy thời gian check in
            DateTime dateTimeStart = BillDAO.Instance.GetDateCheckIn(table.ID);
        

            // Xử lí thanh toán
            if (table.Status == "Có người")
            {
                if (idBill != -1)
                {
                    fPay fPay = new fPay(tableID ,tableName, idBill, dateTimeStart, data, discount, classfication, nameAccCheckOut );
                    fPay.ShowDialog();
                    this.Show();                 
                }
                else
                {
                    TableDAO.Instance.CheckTable(table.ID);

                }
               
                LoadTable();
                ShowBill(tableID);
                txbShowTimeTable.Clear();
                btnCheckOut.Enabled = true;
            }
            else
            {
                btnCheckOut.Enabled = false;
            }

        }

        private void bunifuBtnAuto_Click(object sender, EventArgs e)
        {
            autosendResetDevice();
        }

        private void txbDisCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a digit or a control character (e.g. backspace, delete)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // If the pressed key is not a digit or a control character, set the Handled property to true
                // to indicate that the event has been handled and the character should not be processed
                e.Handled = true;
            }
        }

        #endregion



        #region bunifu

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void checkBoxVip_CheckedChanged(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void txbThongTinTaiKhoanHD_Click(object sender, EventArgs e)
        {
            bunifuPanelAcc.Visible = true;
            var timer = new Timer();
            timer.Interval = 2000; 
            timer.Tick += (s, eventArgs) =>
            {
                bunifuPanelAcc.Visible = false;
                timer.Stop();
            };
            timer.Start();
        }

        private void bunifuBtnInfoAcc_Click(object sender, EventArgs e)
        {
            bunifuPanelAcc.Visible = false;
            string name = LoginAccount.Name;
            fInfoAccount fInfoAccount = new fInfoAccount(name);
            fInfoAccount.ShowDialog();
        }

        private void bunifuBtnChangePass_Click(object sender, EventArgs e)
        {
            bunifuPanelAcc.Visible = false;
        }

        private void bunifuBtnLogAcc_Click(object sender, EventArgs e)
        {
            bunifuPanelAcc.Visible = false;
            this.Close();
        }

        #endregion



        private void fTableManager_Load(object sender, EventArgs e)
        {

        }

        private void checkSalary()
        {


        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            txbHelp.Visible = true;
            var timer = new Timer();
            timer.Interval = 1500; 
            timer.Tick += (s, eventArgs) =>
            {
                txbHelp.Visible = false;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
