using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsAppBida.DTO
{
    public class Table
    {
        public Table (int id, string name, string status, string classification)
        {
            this.ID= id;
            this.Name= name;    
            this.Status= status;
            this.Classification = classification;
        }

        public Table(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name"].ToString();
            this.Status = row["status"].ToString();
            this.Classification = row["classification"].ToString();
        }


        private string classification;

        private int iD;
        public int ID 
        { 
            get => iD; 
            set => iD = value; 
        }

        private string name;
        public string Name 
        { 
            get => name; 
            set => name = value; 
        }


        private string status;
        public string Status 
        { 
            get => status; 
            set => status = value; 
        }
        public string Classification { get => classification; set => classification = value; }
    }
}
