using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading; // for thread
using System.Net; // for ipendpoint
using System.Net.Sockets; // for udpclient
using System.Diagnostics; // for taskkill at the end

namespace udp_asynchronous
{
    public partial class Form1 : Form
    {
        public static EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);// for handling thread(pausing it)
        public static IPEndPoint IPEndPoint_serverToListen = new IPEndPoint(IPAddress.Any, 2500); // we'll receive data from port number 2500,but we'll listen to any ip.
        public static IPEndPoint IPEndPoint_server = new IPEndPoint(IPAddress.Parse("10.0.2.255"), 2500); // we'll send data to this ipendpoint. its server's ipendpoint.
        public static UdpClient UdpClient_server; // 2500 >> port number client will listen when receiving data.
        public static UdpClient UdpClient_client1 = new UdpClient(); // port number doesnt needed when sending.
        public static UdpClient UdpClient_client2 = new UdpClient();
        public static bool isReceiving = false; // for tracking the process

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false; // for doing processes between threads.
        }

        //server start and start receiving
        private void button_ServerStart_Click(object sender, EventArgs e)
        {
            Button thisButton = sender as Button;
            if (isReceiving == false)
            {
                isReceiving = true;
                Program.mainForm.listBox1.Items.Add("Server started listening...");
                thisButton.Text = "Stop Server";
                UdpClient_server = new UdpClient(2500); // if we set this at first, it ll always listen and record the datas. we dont want to listen when we stopped. so we reconnecting everytime we start.
                //start_receiving();
                ewh.Set();
                UdpClient_server.BeginReceive(receive_callBack, UdpClient_server); // starts listening
            }
            else
            {
                isReceiving = false;
                Program.mainForm.listBox1.Items.Add("Server stopped listening...");
                thisButton.Text = "Start Server";
                ewh.Reset();
                UdpClient_server.Close();// we need to close it so it stops listening. Otherwise it continues storing the datas.
            }
        }

        //receiving data asynchronously
        private static void receive_callBack(IAsyncResult result)
        {
            try
            {
                ewh.WaitOne();
                string data = Encoding.ASCII.GetString(UdpClient_server.EndReceive(result, ref IPEndPoint_serverToListen)) + " " + IPEndPoint_serverToListen.ToString();
                Program.mainForm.listBox1.Items.Add(data);
                UdpClient_server.BeginReceive(receive_callBack, UdpClient_server);
            }
            catch (SocketException ex)
            {
                //when we stop this process continues and it causes problem because we try to access udpclient_server howevet we deleted it on 
            }
            catch (ArgumentException ex)
            {
                //exception occurs when asycnhronous receiving stops.
            }
        }

        //sending data asynchronously
        private void button_client1Send_Click(object sender, EventArgs e)
        {
            send_data(UdpClient_client1, IPEndPoint_server, Program.mainForm.textBox2.Text);
        }
        private void button_client2Send_Click(object sender, EventArgs e)
        {
            send_data(UdpClient_client2, IPEndPoint_server, Program.mainForm.textBox1.Text);
        }
        public void send_data(UdpClient sender, IPEndPoint remoteIP, string data)
        {
            byte[] data_byte = Encoding.ASCII.GetBytes(data);
            sender.SendAsync(data_byte, data_byte.Length, remoteIP);
        }


        //closing processes
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill(); // for killing all threads
            //Environment.Exit(Environment.ExitCode);
        }
    }


}
