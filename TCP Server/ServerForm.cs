using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace TCP_Server
{
    //Source: https://www.youtube.com/watch?v=Bq1JhTHlxek
    public partial class ServerForm : Form
    {
        private Socket _serverSocket, _clientSocket;
        private byte[] _buffer;

        public ServerForm()
        {
            string path = @"C:\Users\kaden\Documents\Visual Studio 2017\Projects\Simple Async Socket\Simple Async Socket\bin\Debug\TCP Client.exe";
            Process.Start(path);
            InitializeComponent();
            StartServer();
        }

        private void StartServer()
        {
            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8001));
                _serverSocket.Listen(2);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                _clientSocket = _serverSocket.EndAccept(ar);
                _buffer = new byte[_clientSocket.ReceiveBufferSize];
                _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int received = _clientSocket.EndReceive(ar);
                if (received == 0)
                {
                    return;
                }

                Array.Resize(ref _buffer, received);
                string text = Encoding.ASCII.GetString(_buffer);
                AppendToTextBox(text);
                Array.Resize(ref _buffer, _clientSocket.ReceiveBufferSize);
                _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendToTextBox(string text)
        {
            MethodInvoker invoker = new MethodInvoker(delegate
            {
                textBox1.Text += text + "\r\n" + "\r\n";
            });

            Invoke(invoker);
        }
    }
}
