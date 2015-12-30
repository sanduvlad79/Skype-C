﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientToServerCOM
{
    public class Wrapper
    {
        private static I_Out_COM comToServer;
        private static Wrapper instance;

        private Wrapper()
        {
            //
        }

        public static Wrapper GetInstance()
        {
            if (instance == null)
            {
                instance = new Wrapper();
            }
            return instance;
        }

        public void SignIn(string userName, string password, string channelURL)
        {
            comToServer.SignIn(userName, password, channelURL);
        }

        public void SignOut(string userName)
        {
            comToServer.SignOut(userName);
        }

        public void SearchFriends(string searchBy)
        {
            comToServer.SearchFriends(searchBy);
        }

        public void AddFriend(string userName)
        {
            comToServer.AddFriend(userName);
        }

        public void SetAvailableState(AvailableState state)
        {
            comToServer.SetAvailableState(state);
        }


        public void Attach(I_Out_COM ComServer)
        {
            comToServer = ComServer;
        }
    }
}