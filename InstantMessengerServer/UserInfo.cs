﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace InstantMessengerServer
{
    [Serializable]
    public class UserInfo
    {
        public string UserName;
        public string Password;
        public string MobileNo;
        public string EmailAddress;
        [NonSerialized] public bool LoggedIn;      // Is logged in and connected?
        [NonSerialized] public Client Connection;  // Connection info
        
        public UserInfo(string user, string pass)
        {
            this.UserName = user;
            this.Password = pass;
            this.LoggedIn = false;
        }
        public UserInfo(string user, string pass,string emailaddress,string mobileno)
        {
            this.UserName = user;
            this.Password = pass;
            this.EmailAddress = emailaddress;
            this.MobileNo = mobileno;
            this.LoggedIn = false;
        }
        public UserInfo(string user, string pass, string emailaddress, string mobileno, Client conn)
        {
            this.UserName = user;
            this.Password = pass;
            this.EmailAddress = emailaddress;
            this.MobileNo = mobileno;
            this.LoggedIn = true;
            this.Connection = conn;
        }
    }
}
