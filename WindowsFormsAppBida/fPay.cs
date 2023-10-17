using Google.Protobuf.WellKnownTypes;
using ServiceStack.Script;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DAO;
using WindowsFormsAppBida.DTO;
using static WindowsFormsAppBida.fLogin;

namespace WindowsFormsAppBida
{
    public partial class fPay : Form
    {
        private int tableID;
        private int discount;
        private string tableName;
        private int idBill;
        private double totalPrice;
        private string classification;


        public fPay(int tableID, string tableName, int idBill, DateTime dateTimeStart, List<String[]> data, int discount, string classification, string nameAccCheckOut)
        {
            InitializeComponent();
            this.discount = discount;
            this.tableName = tableName;
            this.idBill = idBill;
            this.tableID = tableID;
            this.classification = classification;

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            lbNameTale.Text = tableName;
            lbIdBill.Text = ""+idBill;
            lbNameAcc.Text = nameAccCheckOut;          
            lbDateBill.Text = dateTimeStart.ToString("dd/MM/yyyy");
            txbDiscount.Text = discount.ToString();

            DateTime dateTimeNow = DateTime.Now;
            lbTimePlay.Text = dateTimeStart.ToString("HH:mm tt") + " - " + dateTimeNow.ToString("HH:mm tt");


            TimeSpan duration = dateTimeNow - dateTimeStart;
            string durationString = duration.ToString("hh\\:mm");

            // tính tiền giờ chơi   
            double servicePricePerHour;  // Giá tiền dịch vụ theo giờ là 100.000 đồng
            if (classification == "Nom")
            {
                servicePricePerHour = PriceBidaPlay.Instance.PriceNom();
            }    
            else
            {
                servicePricePerHour = PriceBidaPlay.Instance.PriceVip();
            }    

           
            int totalHours = duration.Hours;//lấy giờ
            int minutes = duration.Minutes;//lấy phút


            double totalPriceBidaHour = 0;
            if (totalHours >= 1)
            {
                // Tính giá tiền dịch vụ theo số giờ sử dụng và giá tiền theo giờ
                if(minutes < 40)
                {
                    totalPriceBidaHour = totalHours * servicePricePerHour;
                }
                else
                {
                    totalPriceBidaHour = totalHours * servicePricePerHour + servicePricePerHour/2 ;
                }    
                


            }
            else
            {
                totalPriceBidaHour = servicePricePerHour;

            }


            // hiện thị tiền giờ chơi
            dataGridViewPay.Rows[0].Cells["Column01"].Value = "Thời gian chơi";
            dataGridViewPay.Rows[0].Cells["Column02"].Value = durationString + "p";
            dataGridViewPay.Rows[0].Cells["Column03"].Value = totalPriceBidaHour.ToString("N0");
            //Hiện thị tiền dịch vụ ăn uống
            double totalPriceFood = 0; //Tổng tiền ăn uống
            foreach (string[] rowData in data)
            {
                totalPriceFood += Convert.ToDouble(rowData[2]);
                dataGridViewPay.Rows.Add(rowData[0], rowData[1], rowData[2]);
            }


            txbSubTotal.Text = (totalPriceBidaHour + totalPriceFood).ToString("N0") + " VNĐ"; 
            totalPrice = (totalPriceBidaHour + totalPriceFood) - ((totalPriceBidaHour + totalPriceFood) / 100) * discount;
            txbTotalPrice.Text = totalPrice.ToString("N0") + " VNĐ";
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            BillDAO.Instance.CheckOut(idBill, discount, (float)totalPrice);
            TableDAO.Instance.CheckTable(tableID);

            this.Close();
            if (checkBoxBill.Checked)
                printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("HỆ THỐNG QUẢN LÝ BIDA", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(5, 0));
            e.Graphics.DrawString("Nhân viên thanh toán: " + lbNameAcc.Text, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(0, 40));
            e.Graphics.DrawString(lbNameTale.Text , new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(0, 55));
            e.Graphics.DrawString("----------------------------------------------------", new Font("Arial", 10, FontStyle.Regular), Brushes.Black, new Point(0, 75));

            e.Graphics.DrawString("HÓA ĐƠN BÁN HÀNG", new Font("Arial", 9, FontStyle.Bold), Brushes.Black, new Point(28, 95));
            e.Graphics.DrawString(DateTime.Now.ToShortDateString(), new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(0, 115));
            e.Graphics.DrawString("HĐ: " + lbIdBill.Text, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(110, 115));
            e.Graphics.DrawString("Thời gian chơi: " + lbTimePlay.Text, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(0, 130));


            int temp = 150;
            int i = 1;
            foreach (DataGridViewRow row in dataGridViewPay.Rows)
            {
                // Access the cell values in each column of the current row
                string name = row.Cells["Column01"].Value.ToString();
                string count = row.Cells["Column02"].Value.ToString();
                string price = row.Cells["Column03"].Value.ToString();

                // Use the cell values as needed, including the sequence number
                e.Graphics.DrawString(i + ". " + name, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(0, temp));
                e.Graphics.DrawString("SL: " + count, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(10, temp + 10));
                e.Graphics.DrawString("Giá: " + price, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(110, temp + 10));
                temp += 30;

                i++;
            }

            int temp2 = temp + 5;
            e.Graphics.DrawString("Tạm tính: " +txbSubTotal.Text, new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(0, temp2 + 20));

            e.Graphics.DrawString("Giảm giá: " + txbDiscount.Text + "%", new Font("Arial", 7, FontStyle.Italic), Brushes.Black, new Point(0, temp2 + 35));

            e.Graphics.DrawString("Tổng tiền: " + txbTotalPrice.Text , new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(0, temp2 + 50));

            e.Graphics.DrawString("---------------------------------------------------------", new Font("Arial", 10, FontStyle.Regular), Brushes.Black, new Point(0, temp2 + 70));

            e.Graphics.DrawString("CẢM ƠN QUÝ KHÁCH VÀ HẸN GẶP LẠI", new Font("Arial", 7, FontStyle.Italic), Brushes.Black, new Point(0, temp2 + 90));


        }

        private void checkBoxBill_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                   
               
            }
        }
    }
}
