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


namespace udp_synchronous
{
    public partial class Form1 : Form
    {
        public static Thread thread_serverStart = new Thread(new ThreadStart(start_receiving));
        public static EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);// for handling thread(pausing it)
        public static IPEndPoint IPEndPoint_serverToListen = new IPEndPoint(IPAddress.Any, 2500); // we'll receive data from port number 2500,but we'll listen to any ip.
        public static IPEndPoint IPEndPoint_server = new IPEndPoint(IPAddress.Parse("10.0.2.255"), 2500); // we'll send data to this ipendpoint. its server's ipendpoint.
        public static UdpClient UdpClient_server; // 2500 >> port number client will listen when receiving data.
        public static UdpClient UdpClient_client1 = new UdpClient(); // port number doesnt needed when sending.
        public static UdpClient UdpClient_client2 = new UdpClient();

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false; // for doing processes between threads.
            thread_serverStart.Start();// server started. but ewh waits serverStart button to continue.
        }
        
        //server start and start receiving
        private void button_ServerStart_Click(object sender, EventArgs e)
        {
            Button thisButton = sender as Button;
            if (thread_serverStart.ThreadState != ThreadState.Running)
            {
                Program.mainForm.listBox1.Items.Add("Server started listening...");
                UdpClient_server = new UdpClient(2500); // if we set this at first, it ll always listen and record the datas. we dont want to listen when we stopped. so we reconnecting everytime we start.
                thisButton.Text = "Stop Server";
                ewh.Set();
            }
            else
            {
                thisButton.Text = "Start Server";
                ewh.Reset();
                UdpClient_server.Close();
            }
        }
        // receiving data continuously
        public static void start_receiving()
        {
            ewh.WaitOne();
            string data = null;
            while (true)
            {
                data = receive_data(UdpClient_server, IPEndPoint_serverToListen);
                Program.mainForm.listBox1.Items.Add(data);
                ewh.WaitOne(); // we have to write it here. thread normally waits inside receive_data. when we send data it he gets the data but it stop when it encounters ewh.waitone .
            }
        }
        public static string receive_data(UdpClient receiver, IPEndPoint toListen)
        {
            try
            {
                return Encoding.ASCII.GetString(receiver.Receive(ref toListen)) +" " + toListen.ToString();//toListen contains the sender info.
            }
            catch (SocketException ex)
            {
                return "Server stopped listening...";//when we stop this process continues and it causes problem because we try to access udpclient_server howevet we deleted it on 
            }
        }

        //sending data
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
            sender.Send(data_byte, data_byte.Length, IPEndPoint_server);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread_serverStart.Abort();// we need to abort the thread before closing udpclient because we are using udpclient on the thread and when we close udp, it gives error because it cant find udpclient.
            if(UdpClient_server != null) // we need to check this or it shoots exception.5
                UdpClient_server.Close(); // we need to close udp. if we dont, when we close the form, program keeps running.
        }
    }

    
}

//note:when we create udpclient with new udpclient(2500); it always listen even when we dont read. when we begin reading, first it gives all the
//     data it stored during waiting and then it continuous the normal receiving. To prevent this, when we stopped server(reading thread), we 
//     also need to close the udp client. when we restart reading process, we need the declare it again like this >> udpclient = new udpclient(2500)
