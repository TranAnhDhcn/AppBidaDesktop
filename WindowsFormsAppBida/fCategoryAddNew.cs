using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;

namespace WindowsFormsAppBida
{
    public partial class fCategoryAddNew : Form
    {
        public fCategoryAddNew()
        {
            InitializeComponent();
            LoadListFoodCatergory();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        void LoadListFoodCatergory()
        {
            CategoryDAO.Instance.GetListCategory();
        }

        private void fCategoryAddNew_FormClosed(object sender, FormClosedEventArgs e)
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


        private void bunifuButton22_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            string name = txbCategoryNew.Text;
            bool isMatch = CheckIfNameExistsCategory(CategoryDAO.Instance.GetListCategory(), name);

            if (isMatch)
            {
                MessageBox.Show("Tên danh mục đã tồn tại. Vui lòng nhập tên khác!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                if (CategoryDAO.Instance.InsertFoodCategory(name))
                {
                    MessageBox.Show("Thêm danh mục món ăn thành công", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Có lỗi");
                }
            }
        }

        private void txbCategoryNew_TextChange(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txbCategoryNew.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }    
        }
    }
}
