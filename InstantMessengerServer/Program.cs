﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace InstantMessengerServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.WriteLine();
            Console.WriteLine("Press enter to close program.");
            Console.ReadLine();
        }

        // Self-signed certificate for SSL encryption.
        // You can generate one using my generate_cert script in tools directory (OpenSSL is required).
        public X509Certificate2 cert = new X509Certificate2("server.pfx", "instant");

        // IP of this computer. If you are running all clients at the same computer you can use 127.0.0.1 (localhost). 
        public IPAddress ip = IPAddress.Parse("197.242.148.230");
        public int port = 2000;
        public bool running = true;
        public TcpListener server;

        public Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();  // Information about users + connections info.

        public Program()
        {
            Console.Title = "FundXchangeChat Server";
            Console.WriteLine("----- ChatMessenger Server -----");
            LoadUsers();
            Console.WriteLine("[{0}] Starting server...", DateTime.Now);

            server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine("[{0}] Server is running properly!", DateTime.Now);
            
            Listen();
        }

        void Listen()  // Listen to incoming connections.
        {
            while (running)
            {
                TcpClient tcpClient = server.AcceptTcpClient();  // Accept incoming connection.
                Client client = new Client(this, tcpClient);     // Handle in another thread.
            }
        }

        string usersFileName = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()) + "\\users.dat";
        string userinfofile = "";// = System.IO.Directory.GetCurrentDirectory() + "\\userinfo.csv";
        public void SaveUsers()  // Save users data
        {
            try
            {
                Console.WriteLine("[{0}] Saving users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Create, FileAccess.Write);
                bf.Serialize(file, users.Values.ToArray());  // Serialize UserInfo array
                file.Close();
                Console.WriteLine("[{0}] Users saved!", DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void LoadUsers()  // Load users data
        {
            try
            {
                userinfofile = usersFileName.Replace("\\users.dat", "\\userinfo.csv");
                Console.WriteLine("[{0}] Loading users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Open, FileAccess.Read);
                UserInfo[] infos = (UserInfo[])bf.Deserialize(file);      // Deserialize UserInfo array
                file.Close();
                users = infos.ToDictionary((u) => u.UserName, (u) => u);  // Convert UserInfo array to Dictionary
                String csv = String.Join(
    Environment.NewLine,
    users.Select(d => d.Key + ";" + d.Value + ";")
);
                System.IO.File.WriteAllText(userinfofile, csv);
                Console.WriteLine("[{0}] Users loaded! ({1})", DateTime.Now, users.Count);
            }
            catch { }
        }
    }
}
