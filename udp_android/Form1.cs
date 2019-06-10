using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading; // for thread and ewh
using System.Net; // for ipendpoint
using System.Net.Sockets; // for udpclient
using System.Diagnostics; // for process.kill

namespace udp_android
{
    public partial class Form1 : Form
    {
        public static Thread thread_serverStart = new Thread(new ThreadStart(start_receiving));
        public static EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);// for handling thread(pausing it)
        public static IPEndPoint IPEndPoint_serverToListen = new IPEndPoint(IPAddress.Any, 2500); // we'll receive data from port number 2500,but we'll listen to any ip.
        public static IPEndPoint IPEndPoint_server = new IPEndPoint(IPAddress.Parse("10.0.2.238"), 2500); // we'll send data to this ipendpoint. its server's ipendpoint.
        public static UdpClient UdpClient_server; // 2500 >> port number client will listen when receiving data.

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false; // for doing processes between threads.
            thread_serverStart.Start();// server started. but ewh waits serverStart button to continue.
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Button thisButton = sender as Button;
            if (thread_serverStart.ThreadState != System.Threading.ThreadState.Running)
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
                string data = Encoding.ASCII.GetString(receiver.Receive(ref toListen)) + " " + toListen.ToString();//toListen contains the sender info.
                Random rnd = new Random(); // i genereate a random number to send back because i can see only the last message on the app. if i send "success", i cant tell the difference between error and success.
                receiver.Send(Encoding.ASCII.GetBytes("success" + rnd.Next(11,55).ToString()), 9, toListen.Address.ToString(),9998);// we have to know which port the remote device listens
                return data;
            }
            catch (SocketException ex)
            {
                return "Server stopped listening...";//when we stop this process continues and it causes problem because we try to access udpclient_server howevet we deleted it on 
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill(); // for killing all threads
        }
    }
}
