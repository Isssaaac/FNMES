using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.Utility.Network
{
    public class SocketHelper
    {

        //1.声明关于事件的委托；
        public delegate void MessageEventHandler(object sender, byte[] bytesList);
        //2.声明事件；   
        public event MessageEventHandler MessageHandler;

        private Socket socket;
        private IPAddress ip;
        private IPEndPoint iPEnd;

        public SocketHelper(string ipAddress, int port)
        {
            ip = IPAddress.Parse(ipAddress);
            iPEnd = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public SocketHelper()
        {
        }


        public byte[] SendSync(string ipAddress, int port, byte[] bytes)
        {
            try
            {
                byte[] retBytes = new byte[8];
                IPAddress ip = IPAddress.Parse(ipAddress);
                IPEndPoint iPEnd = new IPEndPoint(ip, port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = 1000;
                socket.SendTimeout = 1000;
                socket.Connect(iPEnd);
                socket.Send(bytes, bytes.Length, SocketFlags.None);
                Thread.Sleep(100);
                socket.Receive(retBytes, socket.Available, SocketFlags.None);
                socket.Close();
                return retBytes;
            }
            catch
            {
                List<byte> list = new List<byte>();
                list.Add(0x00);
                list.Add(0x00);
                list.Add(0x00);
                list.Add(0x00);
                list.Add(0x00);
                list.Add(0x00);
                list.Add(0x00);
                list.Add(0x00);
                return list.ToArray();
            }

        }

        public List<byte> SendSync(string ipAddress, int port, List<byte> bytes)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint iPEnd = new IPEndPoint(ip, port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                byte[] retBytes = new byte[8];
                socket.ReceiveTimeout = 5000;
                socket.SendTimeout = 5000;
                socket.Connect(iPEnd);
                socket.Send(bytes.ToArray(), bytes.Count, SocketFlags.None);
                Thread.Sleep(200);
                socket.Receive(retBytes, socket.Available, SocketFlags.None);
                socket.Close();
                return retBytes.ToList();
            }
            catch (Exception ex)
            {
                try { socket.Close(); } catch { }
                throw ex;
            }

        }
        public void SendAsyn(string ipAddress, int port, byte[] bytes)
        {
            new Thread(() =>
            {
                byte[] retBytes = new byte[8];
                IPAddress ip = IPAddress.Parse(ipAddress);
                IPEndPoint iPEnd = new IPEndPoint(ip, port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(iPEnd);
                socket.Send(bytes, bytes.Length, SocketFlags.None);
                Thread.Sleep(100);
                socket.Receive(retBytes, socket.Available, SocketFlags.None);
                socket.Close();
                MessageHandler?.Invoke(this, retBytes);
            }).Start();

        }

        #region 1.建立连接
        public bool Connect()
        {
            try
            {
                socket.Connect(iPEnd);
                //socket.BeginConnect(iPEnd, new AsyncCallback(ConnectCallBack), socket);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 2.发送
        public bool Send(byte[] bytes)
        {
            try
            {
                int length = socket.Send(bytes, bytes.Length, SocketFlags.None);
                if (length != bytes.Length)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Send(List<byte> bytes)
        {
            try
            {
                int length = socket.Send(bytes.ToArray(), bytes.Count, SocketFlags.None);
                if (length != bytes.Count)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion 

        #region 3.接收
        public bool Receive(ref byte[] bytes)
        {
            int byteCount = socket.Receive(bytes, socket.Available, SocketFlags.None);
            if (byteCount != bytes.Length)
            {
                return false;
            }
            return true;
        }

        public bool Receive(ref List<byte> bytesList)
        {
            byte[] bytes = new byte[8];
            bool flag = Receive(ref bytes);
            bytesList = bytes.ToList<byte>();
            return flag;
        }
        #endregion

        #region  4.关闭连接
        public void Close()
        {
            socket.Close();
        }
        #endregion

        #region 判断是否连接
        public bool IsConnected { get { return socket.Connected; } }

        #endregion
    }
}
