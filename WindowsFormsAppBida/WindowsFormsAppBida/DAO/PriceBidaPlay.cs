using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppBida.DAO
{
    public class PriceBidaPlay
    {
        private static PriceBidaPlay instance;
        public PriceBidaPlay() { } 

        public static PriceBidaPlay Instance
        {
            get { if (instance == null) instance = new PriceBidaPlay(); return PriceBidaPlay.instance; }
            private set { PriceBidaPlay.instance = value; }
        }

        public double PriceNom ()
        {
            String query = "select tableNom from PriceBidaHour";
            return (double) DataProvider.Instance.ExecuteScalar(query);
        }
        public double PriceVip()
        {
            String query = "select tableVip from PriceBidaHour";
            return (double)DataProvider.Instance.ExecuteScalar(query);
        }

        public void updatePriceNom(double price)
        {
            DataProvider.Instance.ExecuteNonQuery("UPDATE PriceBidaHour SET tableNom = " +price);
        }

    }
}
