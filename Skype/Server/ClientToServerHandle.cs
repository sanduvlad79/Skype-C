﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Interogare;
namespace Server
{
    class ClientToServerHandle : ClientToServerCOM.I_Out_COM
    {
        private Dictionary<string, string> Clients = new Dictionary<string, string>();
        // userName, channelURL //
        private XmlDataBase db = new XmlDataBase();

        public string getClientURL(string userName)
        {
            return Clients[userName];
        }

        public void AddFriend(string userName,string friend)
        {
            db.AddFriend(userName, friend);
        }

      

        public string[] GetFriends(string username)
        {
            return db.AllFriends(username);
        }

        public int Register(string userName, string password, string email, string nume)
        {
            if(db.Register(userName,password,password,nume,email)==1)
            {
                return 1;
            }else
            {
                return 0;
            }
        }

        public int SignIn(string userName, string password, string channelURL)
        {
            if (Clients.ContainsKey(userName))
                if (db.LogIn(userName, password) == 1)
                {
                   // Clients.Add(userName, channelURL);
                    return 1;
                }
                else
                    return 0;
            else
            {
                if (db.LogIn(userName, password) == 1)
                {
                    Clients.Add(userName, channelURL);
                    return 1;
                }
                else
                    return 0;
               
            }
            
        }


        public List<string> SearchUsers(string query, string username)
        {
            return db.FindUsers(query, username);
        }

        public void ChangeStatus(string userName, String status)
        {
            db.ChangeStatus(userName, status);

        }

        public int SignOut(string userName)
        {
            db.LogOut(userName);
            Clients.Remove(userName);
            return 1;
        }
    }
}
