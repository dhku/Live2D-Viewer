using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// socket
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts;


namespace Assets.Scripts.Network
{
    public class Client
    {
        private Thread receiveThread;
        private TcpClient client;
        private TcpListener listener;
        private int port = 5066;
        private VtuberCore manager;
        private MovementPacket packet;

        bool isConnected = false;

        public Client(VtuberCore manager)
        {
            this.packet = new MovementPacket();
            this.manager = manager;
        }

        public void Start()
        {
            InitTCP();
        }

        // Launch TCP to receive message from python
        public void InitTCP()
        {
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveData()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Byte[] bytes = new Byte[1024];

                while (true)
                {
                    using (client = listener.AcceptTcpClient())
                    {
                        if (this.isConnected == false) this.isConnected = true;

                        using (NetworkStream stream = client.GetStream())
                        {

                            int length;

                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incommingData = new byte[length];
                                Array.Copy(bytes, 0, incommingData, 0, length);
                                string clientMessage = Encoding.ASCII.GetString(incommingData);
                                string[] res = clientMessage.Split(' ');

                                this.packet.roll = float.Parse(res[0]);
                                this.packet.pitch = float.Parse(res[1]);
                                this.packet.yaw = float.Parse(res[2]);
                                this.packet.ear_left = float.Parse(res[3]);
                                this.packet.ear_right = float.Parse(res[4]);
                                this.packet.x_ratio_left = float.Parse(res[5]);
                                this.packet.y_ratio_left = float.Parse(res[6]);
                                this.packet.x_ratio_right = float.Parse(res[7]);
                                this.packet.y_ratio_right = float.Parse(res[8]);
                                this.packet.mar = float.Parse(res[9]);
                                this.packet.mouth_dist = float.Parse(res[10]);

                                this.manager.UpdatePacket(this.packet);

                                //Debug.Log(string.Format("Roll: {0:F}; Pitch: {1:F}; Yaw: {2:F}", this.packet.roll, this.packet.pitch, this.packet.yaw));
                                //Debug.Log(string.Format("ear_left: {0:F}; ear_right: {1:F}; mouth_dist: {2:F}", this.packet.ear_left, this.packet.ear_right, this.packet.mouth_dist));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SocketException) return;
                Debug.Log(e.ToString());
                Thread.Sleep(500);
                this.isConnected = false;
                this.manager.ConnectionReset();
            }
        }

        public bool IsConnected()
        {
            return this.isConnected;
        }

        public void Close()
        {
            if(client != null) client.Close();
            if(listener != null) listener.Stop();
            if(receiveThread != null) receiveThread.Abort();
        }

    }

}


