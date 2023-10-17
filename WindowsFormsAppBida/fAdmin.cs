using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Metadata;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using System.Diagnostics;
using MySqlX.XDevAPI.Relational;
using iTextSharp.xmp.impl;
using Image = System.Drawing.Image;
using Org.BouncyCastle.Math;
using System.Text.RegularExpressions;
using System.Globalization;
using Table = WindowsFormsAppBida.DTO.Table;
using System.Xml;
using System.Speech.Synthesis.TtsEngine;
using System.Web.UI.WebControls;
using ServiceStack;
using ServiceStack.Script;
using ServiceStack.Messaging;

namespace WindowsFormsAppBida
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        string imgeUrl = null;
        CultureInfo culture = new CultureInfo("vi-VN");
        public fAdmin()
        {
            InitializeComponent();

            //xóa hiển thị doanh thu
            this.Controls.Remove(lbTotalBill);
            this.Controls.Remove(lbTotalRevenue);
            Load_f();
        }


        #region methods

        List<Food> SearchFoodByName(string Name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(Name);
            return listFood;
        }
        void Load_f()
        {
            dtgvFood.DataSource = foodList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadCategoryIntoCombobox(cbFoodCategory);
            AddFoodBinding();

            LoadListFoodCatergory();


            LoadListTable();
            

            LoadListAccount();


            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.MaxDate = DateTime.Today;
            dtpkToDate.MaxDate = DateTime.Today;

            dtpkFromDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpkToDate.Value = DateTime.Today;
            DoanhThu(dtpkFromDate.Value.ToString("yyyy-MM-dd"), dtpkToDate.Value.ToString("yyyy-MM-dd"));
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
            dtgvBill.Columns["Doanh thu"].DefaultCellStyle.Format = "N0";

        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Id", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never)
            {
                FormattingEnabled = true,
                FormatString = "F0"
            });

            nmFoodSalary.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Salary", true, DataSourceUpdateMode.Never));
            imgFood.DataBindings.Add(new Binding("Image", dtgvFood.DataSource, "Image", true, DataSourceUpdateMode.Never));
        }


        void LoadCategoryIntoCombobox(System.Windows.Forms.ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void LoadListFood()
        {

            foodList.DataSource = FoodDAO.Instance.GetListFood();

            // Apply custom formatting to the ColumnPrice in the DataGridView
            dtgvFood.Columns["ColumnPrice"].DefaultCellStyle.Format = "N0";
        }

        void LoadListFoodCatergory()
        {
            dtgvCategory.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void LoadListTable()
        {
            dtgvTable.DataSource = TableDAO.Instance.LoadTableList();
        }

        void LoadListAccount()
        {
            dtgvAccount.DataSource = AccountDAO.Instance.GetListAccount();
        }

        private void tpFood_MouseDown(object sender, MouseEventArgs e)
        {
            LoadListFood();
        }

        private void txbSearchFoodName_KeyDown_1(object sender, KeyEventArgs e)
        {
           
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

        //


        #endregion

        #region events


        private void btnViewBill_Click_1(object sender, EventArgs e)
        {
            DateTime fromDate = dtpkFromDate.Value;
            DateTime fromDate1 = dtpkToDate.Value;
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);


            string dateIn = fromDate.ToString("yyyy-MM-dd");
            string dateOut = fromDate1.ToString("yyyy-MM-dd");

            DataTable dt = new DataTable();

            if (dateIn != dateOut)
            {
                if (fromDate < fromDate1)
                {
                    dt = BillDAO.Instance.GetTotalRevenue(dateIn, dateOut);
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn lại ngày");
                    lbTotalBill.Text = "";
                    lbTotalRevenue.Text = "";
                }

            }
            else
            {
                dt = BillDAO.Instance.GetTotalRevenueInEqualOut(dateIn);
            }

            if (dt.Rows.Count > 0)
            {
                lbTotalRevenue.Text = ((double)dt.Rows[0]["TotalPrice"]).ToString("N0") + " VNĐ";
                lbTotalBill.Text = dt.Rows[0]["TotalBills"].ToString();
            }
        }

        void DoanhThu(string dateIn, string dateOut)
        {
            DataTable dt = BillDAO.Instance.GetTotalRevenue(dateIn, dateOut);

            if (dt.Rows.Count > 0)
            {
                lbTotalRevenue.Text = ((double)dt.Rows[0]["TotalPrice"]).ToString("N0") + " VNĐ";

                lbTotalBill.Text = dt.Rows[0]["TotalBills"].ToString();
            }
        }



        #endregion





        #region food
        private void imgFood_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imgeUrl = ofd.FileName;
                    imgFood.Image = Image.FromFile(ofd.FileName);

                }
            }
        }
        public bool CheckIfNameExists(List<Food> foodList, string textboxValue)
        {
            string normalizedTextboxValue = NormalizeText(textboxValue);

            foreach (Food food in foodList)
            {
                string normalizedFoodName = NormalizeText(food.Name);

                if (string.Equals(normalizedTextboxValue, normalizedFoodName, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Match found
                }
            }

            return false; // No match found
        }
        private void btnDeleteFood_Click_1(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);
            bool isOccupied = IsTableOccupied();
            if (!isOccupied)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc xóa món {0} ? ", txbFoodName.Text), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (dtgvFood.Rows.Count != 0)
                    {
                        if (FoodDAO.Instance.DeleteFood(id))
                        {
                            MessageBox.Show("Xóa món thành công");
                            LoadListFood();
                            if (deleteFood != null)
                                deleteFood(this, new EventArgs());
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Dữ liệu món ăn đang trống. Vui lòng thêm mới", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng thanh toán các bàn đang hoạt động trước khi xóa danh mục!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        string foodNameCurrent;
        private void txbFoodName_Click(object sender, EventArgs e)
        {
            foodNameCurrent = txbFoodName.Text;
        }

        private void txbFoodName_TextChange(object sender, EventArgs e)
        {

        }

        private void txbFoodName_Leave(object sender, EventArgs e)
        {
            if (txbFoodName.Text != foodNameCurrent)
            {
                bool isMatch = CheckIfNameExists(FoodDAO.Instance.GetListFood(), txbFoodName.Text);

                if (isMatch)
                {
                    MessageBox.Show("Tên món đã tồn tại. Vui lòng nhập tên khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadListFood();
                }
            }
        }
        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            decimal price = decimal.Parse(nmFoodPrice.Text);
            int salary = int.Parse(nmFoodSalary.Text);
            int id = Convert.ToInt32(txbFoodID.Text);

            //hinh anh
            Image img = imgFood.Image;
            byte[] arr;
            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));

            bool isOccupied = IsTableOccupied();
            if (!isOccupied)
            {
                if (dtgvFood.Rows.Count != 0)
                {
                    if(price == 0)
                    {
                        MessageBox.Show("Gía món ăn bằng 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (FoodDAO.Instance.UpdateFood(id, name, categoryID, salary, price, arr))
                        {

                            MessageBox.Show("Sửa món thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadListFood();
                            //if (updateFood != null)
                            //    updateFood(this, new EventArgs());
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Dữ liệu món ăn đang trống", " Vui lòng thêm mới", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng thanh toán các bàn đang hoạt động trước khi sửa món ăn!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private void btnAddFoodNew_Click(object sender, EventArgs e)
        {

            fAddNewFood f = new fAddNewFood();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
            LoadListFood();
            if (insertFood != null)
                insertFood(this, new EventArgs());
        }

        private void nmFoodSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (nmFoodSalary.Text.Length >= 7 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;

            }
        }

        private void nmFoodPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            // Giới hạn độ dài của textbox thành 8 chữ số

            if (nmFoodPrice.Text.Length >= 7 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
               
            }
        }
        private void txbFoodID_TextChange(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbFoodID.Text))
            {
                MessageBox.Show("Vui lòng không để trống mã món ăn!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadListFood();
            }
        }

        private void nmFoodSalary_TextChange(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmFoodSalary.Text))
            {
                MessageBox.Show("Vui lòng không để trống số lượng món ăn!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadListFood();
            }
            
        }

        private void nmFoodPrice_TextChange(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmFoodPrice.Text))
            {
                MessageBox.Show("Vui lòng không để trống giá món ăn!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadListFood();
            }
        }

        private void txbFoodName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbFoodName.Text))
            {              
                MessageBox.Show("Vui lòng không để trống tên món ăn!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadListFood();
            }



            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["idCategory"].Value;
                    Category cateogory = CategoryDAO.Instance.GetCategoryByID(id);
                    cbFoodCategory.SelectedItem = cateogory;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == cateogory.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index;
                }

            }
            catch { }
        }

        #endregion

        #region Tabfoodcategory
       

        public bool CheckIfNameExistsCategory(List<Category> categoryList, string textboxValue)
        {
            string normalizedTextboxValue = NormalizeText(textboxValue);

            foreach (Category category in categoryList)
            {
                string normalizedFoodName = NormalizeText(category.Name);

                if (string.Equals(normalizedTextboxValue, normalizedFoodName, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Match found
                }
            }

            return false; // No match found
        }
        private void dtgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dtgvCategory.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ColumnIdCategory"].Value);
            string nameCategory = row.Cells["ColumnNameCategory1"].Value.ToString();
            if (dtgvCategory.Columns[e.ColumnIndex].Name == "ColumnDelete")
            {
                bool isOccupied = IsTableOccupied();
                if (!isOccupied)
                {
                    if (MessageBox.Show(string.Format("Bạn có chắc xóa danh mục {0} ?", nameCategory), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (CategoryDAO.Instance.EditActiveFoodCategory(id))
                        {
                            MessageBox.Show("Xóa danh mục thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadListFoodCatergory();
                            LoadListFood();
                            LoadCategoryIntoCombobox(cbFoodCategory);
                            if (deleteFoodCategory != null)
                                deleteFoodCategory(this, new EventArgs());
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng thanh toán các bàn đang hoạt động trước khi xóa danh mục!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }    

                
            }


        }
        string columnNameCategory;
        private void dtgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dtgvCategory.Rows[e.RowIndex];
            columnNameCategory = row.Cells["ColumnNameCategory1"].Value.ToString();

        }

        private void dtgvCategory_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dtgvCategory.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ColumnIdCategory"].Value);

            // Lấy cell của cột "ColumnNameCategory"
            DataGridViewCell nameCell = row.Cells["ColumnNameCategory1"];

            bool isOccupied = IsTableOccupied();
            if (!isOccupied)
            {
                // Kiểm tra giá trị của cell nameCell
                if (nameCell != null && nameCell.Value != null && !string.IsNullOrWhiteSpace(nameCell.Value.ToString()))
                {
                    string nameCategoryEdit = nameCell.Value.ToString();

                    if (nameCategoryEdit != columnNameCategory)
                    {
                        bool isMatch = CheckIfNameExistsCategory(CategoryDAO.Instance.GetListCategory(), nameCategoryEdit);
                        TableDAO.Instance.LoadTableList();
                        if (isMatch)
                        {
                            MessageBox.Show("Tên danh mục đã tồn tại. Vui lòng nhập tên khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            BeginInvoke(new Action(() =>
                            {
                                LoadListFoodCatergory();
                            }));
                        }
                        else
                        {
                            if (CategoryDAO.Instance.UpdateFoodCategory(id, nameCategoryEdit))
                            {
                                MessageBox.Show("Sửa danh mục món ăn thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadListFoodCatergory();
                                LoadListFood();
                                LoadCategoryIntoCombobox(cbFoodCategory);
                                if (updateFoodCategory != null)
                                    updateFoodCategory(this, new EventArgs());
                            }
                            else
                            {
                                MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                }
                else
                {
                    // Xử lý khi cell trống hoặc giá trị của cell là null hoặc trống
                    MessageBox.Show("Vui lòng không để trống tên danh mục!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BeginInvoke(new Action(() =>
                    {
                        LoadListFoodCatergory();
                    }));
                }
            }
            else
            {
                MessageBox.Show("Vui lòng thanh toán các bàn đang hoạt động trước khi sửa danh mục!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            fCategoryAddNew f = new fCategoryAddNew();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
            LoadListFoodCatergory();
            if (insertFoodCategory != null)
                insertFoodCategory(this, new EventArgs());
        }

        private event EventHandler updateFoodCategory;
        public event EventHandler UpdateFoodCategory
        {
            add { updateFoodCategory += value; }
            remove { updateFoodCategory -= value; }
        }

        private event EventHandler insertFoodCategory;
        public event EventHandler InsertFoodCategory
        {
            add { insertFoodCategory += value; }
            remove { InsertFoodCategory -= value; }
        }

        private event EventHandler deleteFoodCategory;
        public event EventHandler DeleteFoodCategory
        {
            add { deleteFoodCategory += value; }
            remove { DeleteFoodCategory -= value; }
        }





        #endregion

        #region TabTableBida

        public bool CheckIfNameExistsTable(List<Table> tableList, string textboxValue)
        {
            string normalizedTextboxValue = NormalizeText(textboxValue);

            foreach (Table table in tableList)
            {
                string normalizedTableName = NormalizeText(table.Name);

                if (string.Equals(normalizedTextboxValue, normalizedTableName, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Match found
                }
            }

            return false; // No match found
        }

        private void dtgvTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow row = dtgvTable.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ColumnID"].Value);
            string nameTable = row.Cells["ColumnNameTable"].Value.ToString();

            DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dtgvTable.Columns["ColumnActive"];


            if (dtgvTable.Columns[e.ColumnIndex].Name == "ColumnDeleteTable")
            {
                if (MessageBox.Show(string.Format("Bạn có chắc xóa {0} ? ", nameTable), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if(row.Cells["ColumnStatus"].Value.ToString() == "Trống")
                    {
                        if (TableDAO.Instance.UpdateActiveTable(id))
                        {
                            MessageBox.Show("Xóa " + nameTable + " thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadListTable();
                            if (deleteTable != null)
                                deleteTable(this, new EventArgs());
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi xóa bàn. Vui lòng kiểm tra bàn và thử lại sau !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }    
                    else
                    {
                        MessageBox.Show(" Vui lòng thanh toán bàn chơi trước khi xóa ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        BeginInvoke(new Action(() =>
                        {
                            LoadListTable();
                        }));
                    }    
                   
                }
            }

            // khi nhan active (cap ban)
            if (dtgvTable.Columns[e.ColumnIndex].Name == "ColumnActive")
            {
                if (TableDAO.Instance.getStatusLoraMesh(id) == 0)
                {
                    byte[] addTable= { 0xF1, 0x04, 0x01, 0xFF, (byte)id, 0xAA };
                    
                }
                else
                {
                    MessageBox.Show("Bàn đã được kích hoạt", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }    
            }

        }

        public static bool CheckTableNameFormat(string nameTable)
        {
            string pattern = @"^Bàn\s\d+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(nameTable);
        }

        string nameTableCurrent;
        string classificationCurent;
        private void dtgvTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dtgvTable.Rows[e.RowIndex];
            nameTableCurrent = row.Cells["ColumnNameTable"].Value.ToString();
            classificationCurent = row.Cells["ColumnNameClassification"].Value.ToString();
        }
        private void dtgvTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //sửa bàn chơi
            DataGridViewRow row = dtgvTable.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ColumnID"].Value);
            string nameTable;
            string classificationTable;
            // Lấy cell của cột "ColumnNameCategory"
            DataGridViewCell nameCell = row.Cells["ColumnNameTable"];
            DataGridViewCell classsificationCell = row.Cells["ColumnNameClassification"];

            bool isOccupied = IsTableOccupied();
            if (!isOccupied)
            {
                // Kiểm tra giá trị của cell nameCell
                if (nameCell != null && nameCell.Value != null && !string.IsNullOrWhiteSpace(nameCell.Value.ToString()) &&
                    classsificationCell != null && classsificationCell.Value != null && !string.IsNullOrWhiteSpace(classsificationCell.Value.ToString()))
                {
                    nameTable = row.Cells["ColumnNameTable"].Value.ToString();
                    classificationTable = row.Cells["ColumnNameClassification"].Value.ToString();

                    if (nameTable != nameTableCurrent || classificationTable != classificationCurent)
                    {
                        bool hasValidFormat = CheckTableNameFormat(nameTable); // kiểm tra chuỗi bàn + số
                        bool isMatch = CheckIfNameExistsTable(TableDAO.Instance.LoadTableList(), nameTable); // kiểm tra tên bàn tồn tại hay không

                        if (e.ColumnIndex == dtgvTable.Columns["ColumnNameTable"].Index)
                        {
                            if (hasValidFormat)
                            {
                                if (isMatch)
                                {
                                    MessageBox.Show("Tên bàn chơi đã tồn tại. Vui lòng nhập tên khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    BeginInvoke(new Action(() =>
                                    {
                                        LoadListTable();
                                    }));
                                }
                                else
                                {
                                    if (TableDAO.Instance.UpdateNameTable(id, nameTable, classificationTable))
                                    {
                                        MessageBox.Show("Sửa tên bàn chơi thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        BeginInvoke(new Action(() =>
                                        {
                                            LoadListTable();
                                            if (updateTable != null)
                                                updateTable(this, new EventArgs());
                                        }));

                                    }
                                    else
                                    {
                                        MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(" Sửa bàn theo mẫu sau: Tên bàn chơi = (Bàn + số) ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                BeginInvoke(new Action(() =>
                                {
                                    LoadListTable();
                                }));
                            }
                        }

                        if (e.ColumnIndex == dtgvTable.Columns["ColumnNameClassification"].Index)
                        {
                            if (classificationTable == "Nom" || classificationTable == "Vip")
                            {
                                if (TableDAO.Instance.UpdateNameTable(id, nameTable, classificationTable))
                                {
                                    MessageBox.Show(nameTable + " sửa thông tin loại bàn thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    BeginInvoke(new Action(() =>
                                    {
                                        LoadListTable();
                                        if (updateTable != null)
                                            updateTable(this, new EventArgs());
                                    }));

                                }
                                else
                                {
                                    MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show(" Sửa bàn theo mẫu sau: Loại bàn chơi (Nom - Vip)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                BeginInvoke(new Action(() =>
                                {
                                    LoadListTable();

                                }));
                            }
                        }
                    }
                }
                else
                {
                    // Xử lý khi cell trống hoặc giá trị của cell là null hoặc trống
                    MessageBox.Show("Vui lòng không để trống tên bàn chơi hoặc loại bàn chơi", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    BeginInvoke(new Action(() =>
                    {
                        LoadListTable();
                    }));
                }
            }
            else
            {
                MessageBox.Show("Vui lòng thanh toán các bàn đang hoạt động trước khi chỉnh sửa bàn chơi!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BeginInvoke(new Action(() =>
                {
                    LoadListTable();
                }));
            }

               

       }

        private void btnAddNewTableBida_Click(object sender, EventArgs e)
        {
            string nameTable = txbNameTableNew.Text;

            bool isMatch = CheckIfNameExistsTable(TableDAO.Instance.LoadTableList(), nameTable);

            if (isMatch)
            {
                MessageBox.Show("Tên bàn chơi đã tồn tại. Vui lòng nhập tên khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bool hasValidFormat = CheckTableNameFormat(nameTable);
                if (hasValidFormat && cbSelcetClassficationTable.SelectedItem != null)
                {

                    if (TableDAO.Instance.InsertTable(nameTable, cbSelcetClassficationTable.SelectedItem.ToString()))
                    {
                        MessageBox.Show("Thêm bàn chơi thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadListTable();
                        txbNameTableNew.Clear();
                        if (insertTable != null)
                            insertTable(this, new EventArgs());
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
                else if(!hasValidFormat)
                {
                    MessageBox.Show("Vui lòng nhập tên bàn theo mẫu sau: (Bàn + số) ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }    
                else if(cbSelcetClassficationTable.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại bàn chơi", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }    
                else
                {
                    MessageBox.Show("Vui lòng nhập tên bàn theo mẫu sau: (Bàn + số) và Chọn loại bàn chơi", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }



        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { UpdateTable -= value; }
        }


        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }





        #endregion


        #region TabDoanhThu
        private void btnDay_Click_1(object sender, EventArgs e)
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            DataTable dt = BillDAO.Instance.GetTotalRevenueDay(day, month);
            lbTotalRevenue.Text = ((double)dt.Rows[0]["TotalPriceToday"]).ToString("N0") + " VNĐ";

            lbTotalBill.Text = dt.Rows[0]["BillCountToday"].ToString();
        }

        private void btnWeek_Click_1(object sender, EventArgs e)
        {
            DataTable dt = BillDAO.Instance.GetTotalRevenueWeek();
            lbTotalRevenue.Text = ((double)dt.Rows[0]["TotalPriceWeek"]).ToString("N0") + " VNĐ";

            lbTotalBill.Text = dt.Rows[0]["BillCountWeek"].ToString();
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            int month = DateTime.Now.Month; 
            DataTable dt = BillDAO.Instance.GetTotalRevenueMonth(month);
            lbTotalRevenue.Text = ((double)dt.Rows[0]["TotalPriceMonth"]).ToString("N0") + " VNĐ";
            lbTotalBill.Text = dt.Rows[0]["BillCountMonth"].ToString();
        }

        private void btnYear_Click(object sender, EventArgs e)
        {
            int year = DateTime.Now.Year; ;
            DataTable dt = BillDAO.Instance.GetTotalRevenueYear(year);
            lbTotalRevenue.Text = ((double)dt.Rows[0]["TotalPriceYear"]).ToString("N0") + " VNĐ";
            lbTotalBill.Text = dt.Rows[0]["BillCountYear"].ToString();
        }

        private void exportCsv_Click(object sender, EventArgs e)
        {
            if (dtgvBill.Rows.Count > 0)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV file (*.csv)|*.csv";
                saveDialog.Title = "Export Bill to CSV";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (TextWriter writer = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8))
                    {
                        // Write the header row
                        for (int i = 0; i < dtgvBill.Columns.Count; i++)
                        {
                            writer.Write(dtgvBill.Columns[i].HeaderText);
                            if (i < dtgvBill.Columns.Count - 1)
                            {
                                writer.Write(",");
                            }
                        }
                        writer.WriteLine();

                        // Write the data rows
                        for (int i = 0; i < dtgvBill.Rows.Count; i++)
                        {
                            DataGridViewRow row = dtgvBill.Rows[i];
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                writer.Write(row.Cells[j].Value?.ToString());
                                if (j < row.Cells.Count - 1)
                                {
                                    writer.Write(",");
                                }
                            }
                            writer.WriteLine();
                        }
                    }

                    MessageBox.Show("Lưu file thành công");
                    Process.Start(saveDialog.FileName);
                }
            }
            else
            {
                MessageBox.Show("Hóa đơn trống !!!", "Info");
            }
        }

        private void bunifuPictureBox3_Click(object sender, EventArgs e)
        {
            if (dtgvBill.Rows.Count > 0)
            {
                
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveDialog.Title = "Export Bill to PDF";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Create a new PDF document
                    Document document = new Document();
                    PdfWriter.GetInstance(document, new FileStream(saveDialog.FileName, FileMode.Create));
                    document.Open();

                    // Add a table to the document
                    PdfPTable table = new PdfPTable(dtgvBill.Columns.Count);
                    for (int i = 0; i < dtgvBill.Columns.Count; i++)
                    {
                        string headerText = dtgvBill.Columns[i].HeaderText;
                        if (headerText == "Giảm giá")
                        {
                            headerText = "Discount (%)"; // explicitly set the header text
                        }
                        table.AddCell(new Phrase(headerText));
                    }
                    table.HeaderRows = 1;

                    // Write the data rows
                    for (int i = 0; i < dtgvBill.Rows.Count; i++)
                    {
                        DataGridViewRow row = dtgvBill.Rows[i];
                        for (int j = 0; j < row.Cells.Count; j++)
                        {
                            table.AddCell(new Phrase(row.Cells[j].Value?.ToString()));
                        }
                    }

                    document.Add(table);
                    document.Close();
                    Process.Start(saveDialog.FileName);

                }
            }
            else
            {
                MessageBox.Show("Hóa đơn trống !!!", "Info");
            }

        }


        #endregion


        #region AccounStaff
        private void btnAddStaffNew_Click(object sender, EventArgs e)
        {
            fCreateNewAccStaff fCreateNewAccStaff = new fCreateNewAccStaff();
            fCreateNewAccStaff.ShowDialog();
            LoadListAccount();
        }


        //private event EventHandler updateAccountStaff;
        //public event EventHandler UpdateAccountStaff
        //{
        //    add { updateAccountStaff += value; }
        //    remove { updateAccountStaff -= value; }
        //}


        public bool CheckIfNameOrEmailExistsAccount(List<Account> accountList, string textboxValue)
        {
            string normalizedTextboxValue = NormalizeText(textboxValue);

            foreach (Account account in accountList)
            {
                string normalizedName = NormalizeText(account.Name);
                string normalizedEmail = NormalizeText(account.Email);
                string normalizedPhone = NormalizeText(account.Phone.ToString());

                if (string.Equals(normalizedTextboxValue, normalizedName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(normalizedTextboxValue, normalizedEmail, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(normalizedTextboxValue, normalizedPhone, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Match found
                }
            }

            return false; // No match found
        }

        private void dtgvAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dtgvAccount.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ColumnIdAcc"].Value);
            string nameAcc = row.Cells["ColumnNameAcc"].Value.ToString();


            if (dtgvAccount.Columns[e.ColumnIndex].Name == "ColumnDeleteAcc")
            {
                if (MessageBox.Show(string.Format("Bạn có chắc xóa tài khoản {0} ?", nameAcc), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if( AccountDAO.Instance.UpdateActiveAccountStaff(id))
                    {
                        MessageBox.Show("Xóa tài khoản " + nameAcc + " thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadListAccount();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi xóa tài khoản. Vui lòng thử lại", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        string nameAccCurrent;
        string emailCurrent;
        int phoneCurrent;

        private void dtgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dtgvAccount.Rows[e.RowIndex];
            nameAccCurrent = row.Cells["ColumnNameAcc"].Value.ToString();
            emailCurrent = row.Cells["ColumnEmail"].Value.ToString();
            phoneCurrent = Convert.ToInt32(row.Cells["ColumnPhone"].Value);
        }

        private void dtgvAccount_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //sửa tài khoản
            DataGridViewRow row = dtgvAccount.Rows[e.RowIndex];
            int id = Convert.ToInt32(row.Cells["ColumnIdAcc"].Value);

            DataGridViewCell nameCell = row.Cells["ColumnNameAcc"];
            DataGridViewCell emailCell = row.Cells["ColumnEmail"];
            DataGridViewCell phoneCell = row.Cells["ColumnPhone"];



            // Kiểm tra giá trị của cell nameCell
            if (nameCell != null && nameCell.Value != null && !string.IsNullOrWhiteSpace(nameCell.Value.ToString()) &&
                emailCell != null && emailCell.Value != null && !string.IsNullOrWhiteSpace(emailCell.Value.ToString()) &&
                phoneCell != null && phoneCell.Value != null && !string.IsNullOrWhiteSpace(phoneCell.Value.ToString()))
            {
                string name = row.Cells["ColumnNameAcc"].Value.ToString();
                string email = row.Cells["ColumnEmail"].Value.ToString();
                int phone = Convert.ToInt32(row.Cells["ColumnPhone"].Value);


                if (e.ColumnIndex == dtgvAccount.Columns["ColumnNameAcc"].Index)
                { 
                    if(name != nameAccCurrent)
                    {
                        if (AccountDAO.Instance.UpdateAccountStaff(email, name, phone, id))
                        {
                            MessageBox.Show("Sửa tên tài khoản thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.BeginInvoke(new Action(() => LoadListAccount()));
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                    
                if (e.ColumnIndex == dtgvAccount.Columns["ColumnEmail"].Index)
                {
                    if(email != emailCurrent)
                    {
                        bool isMatch = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), email);
                        if (isMatch)
                        {
                            MessageBox.Show("Email đã tồn tại. Vui lòng nhập email khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.BeginInvoke(new Action(() => LoadListAccount()));
                        }
                        else
                        {

                            if(AccountDAO.Instance.UpdateAccountStaff(email, name, phone, id))
                            {
                                MessageBox.Show("Sửa email tài khoản thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.BeginInvoke(new Action(() => LoadListAccount()));
                            }
                            else
                            {
                                MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }    
                    }

                }
                
                if (e.ColumnIndex == dtgvAccount.Columns["ColumnPhone"].Index)
                {
                    bool isMatch1 = CheckIfNameOrEmailExistsAccount(AccountDAO.Instance.LoadAccountList(), phone.ToString());
                    if (isMatch1)
                    {
                        MessageBox.Show("SĐT đã tồn tại. Vui lòng nhập SĐT khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.BeginInvoke(new Action(() => LoadListAccount()));
                    }
                    else
                    {

                        if (AccountDAO.Instance.UpdateAccountStaff(email, name, phone, id))
                        {
                            MessageBox.Show("Sửa SĐT tài khoản thành công !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.BeginInvoke(new Action(() => LoadListAccount()));
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi. Vui lòng thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng không để trống", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new Action(() => LoadListAccount()));
            }
        }

        #endregion


        #region Price
        private void btnSumitEditPrice_Click(object sender, EventArgs e)
        {
           
            

        }

        private void cbSelectClassficationTablePrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPrice();
        }

        void loadPrice()
        {
            if (cbSelectClassficationTablePrice.SelectedItem.ToString() == "Thường")
            {
                txbPriceNowBida.Text = PriceBidaPlay.Instance.PriceNom().ToString("N0") + " VNĐ";
            }
            else
            {
                txbPriceNowBida.Text = PriceBidaPlay.Instance.PriceVip().ToString("N0") + " VNĐ";
            }
        }


        private void btnSumitEditPrice_Click_1(object sender, EventArgs e)
        {
            int inputPrice = int.Parse(txbPriceBidaPlay.Text);
            bool isOccupied = IsTableOccupied();
            if (!isOccupied)
            {
                if (string.IsNullOrWhiteSpace(cbSelectClassficationTablePrice.Text))
                {
                    MessageBox.Show("Vui lòng chọn phân loại bàn chơi", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (string.IsNullOrWhiteSpace(txbPriceBidaPlay.Text))
                {
                    MessageBox.Show("Vui lòng nhập giá tiền", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (inputPrice == 0)
                {
                    MessageBox.Show("Giá tiền giờ chơi = 0", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (cbSelectClassficationTablePrice.SelectedItem.ToString() == "Thường")
                {
                    if (MessageBox.Show(string.Format("Bạn có muốn thay đổi giá tiền của bàn Thường ? ", txbFoodName.Text), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DataProvider.Instance.ExecuteNonQuery("UPDATE PriceBidaHour SET tableNom = " + txbPriceBidaPlay.Text);
                        txbPriceBidaPlay.Clear();
                        MessageBox.Show("Cập nhật giá tiền thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadPrice();
                    }
                }
                else if (cbSelectClassficationTablePrice.SelectedItem.ToString() == "Vip")
                {
                    if (MessageBox.Show(string.Format("Bạn có muốn thay đổi giá tiền của bàn Vip ? ", txbFoodName.Text), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DataProvider.Instance.ExecuteNonQuery("UPDATE PriceBidaHour SET tableVip = " + txbPriceBidaPlay.Text);
                        MessageBox.Show("Cập nhật giá tiền thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txbPriceBidaPlay.Clear();
                        loadPrice();
                    }

                }

            }
            else
            {
                MessageBox.Show("Vui lòng thanh toán các bàn đang hoạt động trước khi xóa danh mục!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }








        #endregion

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            
        }

        private void txbSearchFoodName_TextChange(object sender, EventArgs e)
        {
            // Convert the text to the desired format (e.g., removing diacritics)
            string convertedText = ConvertToUnaccented(txbSearchFoodName.Text);

            // Update the TextBox with the converted text
            txbSearchFoodName.Text = convertedText;


        }

        private string ConvertToUnaccented(string text)
        {

            string unaccentedText = Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[\p{Mn}]", "");

            return unaccentedText;
        }

        private void txbSearchFoodName_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {

                List<Food> listFood = SearchFoodByName(txbSearchFoodName.Text);
                if(listFood.Count > 0)
                    foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);

            }catch (Exception ex) 
            {
                MessageBox.Show(""+ex);
            }
        }

        public bool IsTableOccupied()
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);

                if (table.Status == "Có người")
                {
                    return true;
                }
            }

            return false;
        }

        private void txbPriceBidaPlay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            // Giới hạn độ dài của textbox thành 10 chữ số

            if (txbPriceBidaPlay.Text.Length >= 7 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;

            }

        }

        private void dtgvBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bunifuPanel7_Click(object sender, EventArgs e)
        {

        }

        private void dtgvFood_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
        }

        private void nmFoodSalary_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            // Giới hạn độ dài của textbox thành 10 chữ số

            if (nmFoodSalary.Text.Length >= 5 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;

            }
        }

    }
}
