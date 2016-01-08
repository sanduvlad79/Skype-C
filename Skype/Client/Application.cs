﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Application : Form
    {
        ClientToServerHandle cliToSvr;
        ServerToClientHandle svrToCli;
        String username;
        Dictionary<string, string>friendchoices = new Dictionary<string, string>();

        public Application()
        {
            InitializeComponent();
            this.AcceptButton = SendMessageButton;
            cliToSvr = new ClientToServerHandle();
            svrToCli = new ServerToClientHandle(this);
            ServerToClientCOM.Wrapper.GetInstance().Attach(svrToCli);
        }


        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cliToSvr.SignOut(username)==1)
            {
                loginPanel.Visible = true;
                mainPanel.Visible = false;
            }
        }

        private void RegisterMenuItem_Click(object sender, EventArgs e)
        {
            loginPanel.Visible = false;
            registerPanel.Visible = true;
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loginPanel.Visible = true;
            registerPanel.Visible = false;
        }

        private void bLogin_Click(object sender, EventArgs e)
        {
            String serverIp = cliToSvr.GetServerAddress();
            cliToSvr.InitConnectionToServer(serverIp);
            if(cliToSvr.SignIn(UsernameLoginTextBox.Text, PasswordLoginTextBox.Text)==1)
            {
                username = UsernameLoginTextBox.Text;
                loginPanel.Visible = false;
                mainPanel.Visible = true;
                loginResponseLabel.Text = "";
                UsernameApplicationTextBox.Text = username;
                StatusesComboBox.SelectedItem = "Online";
                ListFriends();
            }
            else
            {
                friendsList.Items.Clear();
                username = UsernameLoginTextBox.Text;
                loginPanel.Visible = true;
                mainPanel.Visible = false;
                loginResponseLabel.Text = "Login Failed";
            }
        }
        

       

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            String serverIp = cliToSvr.GetServerAddress();
            cliToSvr.InitConnectionToServer(serverIp);
            if(cliToSvr.Register(UsernameRegisterTextBox.Text, PasswordRegisterTextBox.Text, EmailRegisterTextBox.Text, NameRegisterTextBox.Text)==1)
            {
                ResponseRegisterLabel.Text = "Register Succeded";
            }else
            {
                ResponseRegisterLabel.Text = "Register Failed";
            }
                
        }

        private void StatusesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cliToSvr.ChangeStatus(username,StatusesComboBox.Text.ToLower());

        }

        private void friendsList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SearchFriendsButton_Click(object sender, EventArgs e)
        {
            listSearchedUsers();
        }

        private void listSearchedUsers()
        {
            string query = searchFriendsText.Text;
            if (query.Length < 2)
            {
                addFriendsLabel.Text = "The searched term needs to be greater than 2 symbols";
                addFriendsList.Items.Clear();
            }
            else
            {
                addFriendsLabel.Text = "";
                addFriendsList.Items.Clear();
                foreach (string u in cliToSvr.SearchUsers(query, username))
                {
                    addFriendsList.Items.Add(u);
                }
                if (addFriendsList.Items.Count > 0)
                {
                    addFriendsLabel.Text = "Click the username you want to add as friend";
                }
            }
        }

        private void ListFriends()
        {
            friendsList.DataSource = null;
            friendsList.Items.Clear();
            var choices = new Dictionary<string, string>();
            string[] friends = cliToSvr.GetFriends(username);
            foreach (string user in friends)
            {
                try
                {
                    friendsList.Items.Add(user.Split(' ')[0] + " - " + user.Split(' ')[1]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            if (choices.Count == 0)
            {
                choices.Add("", "");
            }

            //friendsList.Items.Add("qweqeq");

            //friendsList.DataSource = new BindingSource(friendchoices, null);
            //friendsList.DisplayMember = "Value";
            //friendsList.ValueMember = "Key";
            //friendsList.Refresh();
            //friendchoices.Clear();

        }

        private void addFriendsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string friendSelected = String.Copy(addFriendsList.SelectedItem.ToString());
            cliToSvr.AddFriend(username, friendSelected);
            ListFriends();
            listSearchedUsers();
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {


            if (friendsList.SelectedIndex < 0)
            {
                friendsList.SelectedIndex = 0;
            }

            cliToSvr.SendMessage(username, friendsList.Items[friendsList.SelectedIndex].ToString().Split(' ')[0], SendMessageTextBox.Text);
                //admin1 - offline
            ConversationTextBox.Text += "Me: " + SendMessageTextBox.Text + Environment.NewLine;
            SendMessageTextBox.Text = string.Empty;
        }

        public void DisplayMessageOnScreen(string message)
        {
            Invoke((MethodInvoker)(() => ConversationTextBox.Text += message + Environment.NewLine));
        }
    }
}
