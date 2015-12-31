﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerToClientHandle
    {
        ServerToClientCOM.RemotableObject ClientRemoteObject;

        public void SendMessage(string message, string fromUserName, string toUserNameChannelURL)
        {
            ClientRemoteObject =
                (ServerToClientCOM.RemotableObject)Activator.GetObject(typeof(ServerToClientCOM.RemotableObject),
                toUserNameChannelURL);
            ClientRemoteObject.MessageReceived(message, fromUserName);
        }
    }
}
