using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;

namespace WindowsFormsAppBida
{
    public partial class fAddNewFood : Form
    {
        string imgeUrl = null;
        public fAddNewFood()
        {
            InitializeComponent();
            LoadCategoryIntoCombobox();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        void LoadCategoryIntoCombobox()
        {
            cbFoodCategoryNew.DataSource = CategoryDAO.Instance.GetListCategory();
            cbFoodCategoryNew.DisplayMember = "Name";
        }

        //kiem tra chu hoa thuong co dau
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
       


        private void btnAddFoodNew_Click_1(object sender, EventArgs e)
        {
            string name = txbFoodNameNew.Text;
            int categoryID = (cbFoodCategoryNew.SelectedItem as Category).ID;
            int salary = (int)nmFoodSalaryNew.Value;
            float price = (float)nmFoodPriceNew.Value;


            //hinh anh
            Image img = pictureBox1.Image;
            byte[] arr;
            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));
            if (txbFoodNameNew.Text == string.Empty && nmFoodPriceNew.Value == 0 && nmFoodSalaryNew.Value == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txbFoodNameNew.Text == string.Empty)
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (nmFoodSalaryNew.Value == 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng sản phẩm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (nmFoodPriceNew.Value == 0)
            {
                MessageBox.Show("Vui lòng nhập giá sản phẩm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string textboxValue = txbFoodNameNew.Text;
                bool isMatch = CheckIfNameExists(FoodDAO.Instance.GetListFood(), textboxValue);

                if (isMatch)
                {
                    MessageBox.Show("Tên món đã tồn tại. Vui lòng nhập tên khác!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (FoodDAO.Instance.InsertFood(name, categoryID, salary, price, arr))
                    {
                        MessageBox.Show("Thêm món thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imgeUrl = ofd.FileName;
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.noimage;
                }
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
