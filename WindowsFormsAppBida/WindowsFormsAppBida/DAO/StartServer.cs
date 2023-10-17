using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Management.Instrumentation;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppBida.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowsFormsAppBida.fTableManager;
namespace WindowsFormsAppBida.DAO
{

    public class StartServer
    {
        private static StartServer instance;
        public StartServer() { }
        
        private IPEndPoint ipe;
        private Socket server;
        private Socket client;
        private byte[] datasend = new byte[6];
        public byte[] datareceive = new byte[1024];

        public delegate void MyEventHandler(object sender, EventArgs e);
        public event EventHandler OnMyEventOnLed;
        public static StartServer Instance
        {
            get { if (instance == null) instance = new StartServer(); return StartServer.instance; }
            private set { StartServer.instance = value; }
        }


        public static byte Data;

        public void startServer()
        {

            try
            {
                string ipServer = "192.168.1.201";
                int port = 8234;
                this.ipe = new IPEndPoint(IPAddress.Parse(ipServer), port);
                this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.server.Bind((EndPoint)this.ipe);
                this.server.Listen(10);
                this.client = this.server.Accept();

                SystemSounds.Exclamation.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                Application.Exit();
            }

        }

        public void sendMessage(byte[] data)
        {
            this.client.Send(data);


        }



        public bool receiving = true;

        public void StartReceiving()
        {
            Thread receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        public void StopReceiving()
        {
            this.receiving = false;
        }
        public void ReceiveData()
        {
                while (this.receiving)
                {
                
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesReceived = this.client.Receive(buffer);
                    if (bytesReceived > 0)
                    {
                        this.ProcessReceivedData(buffer, bytesReceived);
                    }
                    else
                    {
                        break; // Socket was closed
                    }


                }
                catch 
                {
                    MessageBox.Show("Kết nối gặp vấn đề. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    break;
                }

            }
            
        }


        

        public void ProcessReceivedData(byte[] data, int length)
        {
            byte[] idArray = TableDAO.Instance.LoadTableIdArray();
            List<byte> normalTableIds = new List<byte>();

            for (int i = 0; i < length; i += 6)
            {
                byte[] receivedBytes = data.Skip(i).Take(6).ToArray();

                if (receivedBytes.Length == 6 && receivedBytes[0] == 0xF2 && receivedBytes[5] == 0xAA)
                {
                    byte idTable = receivedBytes[3];
                    byte command = receivedBytes[2];

                    switch (command)
                    {
                        case 0x01:
                            if (data.Take(length).SequenceEqual(new byte[] { 0xF2, 0x04, 0x01, idTable, 0x0A, 0xAA }))
                            {
                                TableDAO.Instance.UpdateStatusLoraMeshTable(idTable);
                                MessageBox.Show("Kích hoạt bàn mới thành công");

                            }
                            else
                            {
                                string message = Encoding.ASCII.GetString(data, 0, length);
                                // Process the received data
                            }
                            break;

                        case 0x02:
                            
                            if (data.Take(length).SequenceEqual(new byte[] { 0xF2, 0x04, 0x02, idTable, 0x01, 0xAA }))
                            {
                                int idAccount = AccountDAO.Instance.GetIdAccount();
                                int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(idTable);
                                DateTime dateTime = DateTime.Now;
                                if (idBill == -1)
                                {
                                    BillDAO.Instance.InsertBill(dateTime, idTable, idAccount);
                                    TableDAO.Instance.OnStatusTable(idTable);

                                }

                                else
                                {
                                    MessageBox.Show("Bàn đang hoạt động !", "Thông báo");
                                }
                                OnMyEventOnLed?.Invoke(this, EventArgs.Empty);
                            }
                            if (data.Take(length).SequenceEqual(new byte[] { 0xF2, 0x04, 0x02, idTable, 0x00, 0xAA }))
                            {                              
                                
                            
                            }

                            break;
                        case 0x03:
                            break;
                        case 0x04:
                            if (idArray.Contains(idTable) && receivedBytes.SequenceEqual(new byte[] { 0xF2, 0x04, 0x04, idTable, 0x0A, 0xAA }))
                            {
                                MessageBox.Show("Hệ thống hoạt động ");
                            }
                            break;

                        default:
                            // Invalid command
                            break;
                    }
                }

            }
            

        }



        public void checkDevice()
        {
            byte[] checkDevice = { 0xF1, 0x03, 0x04, 0x00, 0xAA };
            sendMessage(checkDevice);
        }

        public void newSystem()
        {
            byte[] newSystem = { 0xFC, 0x03, 0x0C, 0x44, 0x54, 0x4B, 0xEE };
            sendMessage(newSystem);
        }

        public void newAddDevice(byte newAdd)
        {
            byte[] newDevice = { 0xF1, 0x04, 0x01, 0xFF, newAdd, 0xAA };
            sendMessage(newDevice);
        }


        public void onLed(byte dev)
        {
            //byte[] offLed = new byte[6] { 0xF1, 0x04, 0x02, dev, 0x00, 0xAA };
            this.datasend[0] = 0xF1;
            this.datasend[1] = 0x04;
            this.datasend[2] = 0x02;
            this.datasend[3] = dev;
            this.datasend[4] = 0x01;
            this.datasend[5] = 0xAA;
            sendMessage(datasend);
        }

        public void offLed(byte dev)
        {
            //byte[] offLed = new byte[6] { 0xF1, 0x04, 0x02, dev, 0x00, 0xAA };
            this.datasend[0] = 0xF1;
            this.datasend[1] = 0x04;
            this.datasend[2] = 0x02;
            this.datasend[3] = dev;
            this.datasend[4] = 0x00;
            this.datasend[5] = 0xAA;
            sendMessage(datasend);
        }

        public void checkRelay(byte dev)
        {
            //byte[] checkByte = new byte[6] { 0xF1, 0x03, 0x03, dev, 0xAA };
            this.datasend[0] = 0xF1;
            this.datasend[1] = 0x03;
            this.datasend[2] = 0x03;
            this.datasend[3] = dev;
            this.datasend[4] = 0xAA;
            sendMessage(datasend);
        }

        
        public void stopServer()
        {
            this.server.Close();
            StopReceiving();
        }

        public void autosendCheckStatusDevice()
        {
            byte[] idArray = TableDAO.Instance.LoadTableIdArray();
            foreach (byte dev in idArray)
            {
                byte[] checkstt = new byte[5] { 0xF1, 0x03, 0x04, dev, 0xAA };
                sendMessage(checkstt);
                Thread.Sleep(250);
            }
        }
    }


}
