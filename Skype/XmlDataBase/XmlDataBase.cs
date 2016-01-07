﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Interogare
{
    public class XmlDataBase
    {
        public int VerifyUser(string username, string parola)
        {
            int count = 0;
            XElement xDoc = XElement.Load("DB.xml");
            IEnumerable<XElement> address =
                from el in xDoc.Elements("users").Elements("user")
                where (string)el.Attribute("username") == username & (string)el.Element("parola") == parola
                select el;
            foreach (XElement el in address)
                count++;
            return count;
        }

        public int UserExists(string username)
        {
            int count = 0;
            XElement xDoc = XElement.Load("DB.xml");
            IEnumerable<XElement> address =
                from el in xDoc.Elements("users").Elements("user")
                where (string)el.Attribute("username") == username 
                select el;
            foreach (XElement el in address)
                count++;
            return count;
        }

        public int LogIn(string username, string parola)
        {
            if (VerifyUser(username, parola) == 1)
            {
                //Console.WriteLine("Autentificare reusita!");
                ChangeStatus(username, "online");
                return 1;
            }
            else
            {
                //Console.WriteLine("Username-ul sau parola sunt gresite");
                return 0;
            }
        }

        public int LogOut(string username)
        {
            if (UserExists(username) == 1)
            {
                //Console.WriteLine("Delogare reusita!");
                ChangeStatus(username, "offline");
                return 1;
            }
            else
            {
                //Console.WriteLine("Username-ul sau parola sunt gresite");
                return 0;
            }
        }

        public int Delete(string username, string parola)
        {
            if (VerifyUser(username, parola) == 0)
            {
                //Console.WriteLine("Nu ai dreptul sa stergi acest cont");
                return 0;
            }
            else
            {

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("DB.xml");
                XmlNodeList nodes = xDoc.SelectNodes("//user[@username='" + username + "']");
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    nodes[i].ParentNode.RemoveChild(nodes[i]);
                }
                xDoc.Save("DB.xml");

                //Console.WriteLine("Cont sters!");
                return 1;
            }
        }

        public int Register(string username, string parola, string reParola, string nume, string email)
        {
            if (!parola.Equals(reParola))
            {
                return 0;
            }

            int count = 0;

            XElement xDoc = XElement.Load("DB.xml");

            if (xDoc == null)
            {
                //Console.WriteLine("S-a produs o eroare la incarcarea xml-ului");
                return 0;
            }

            IEnumerable<XElement> address =
                from el in xDoc.Elements("users").Elements("user")
                where (string)el.Attribute("username") == username
                select el;

            foreach (XElement el in address)
                count++;

            if (count == 0)
            {
                var myNewElement = new XElement("user",
                   new XAttribute("username", username),
                   new XAttribute("status", "offline"),
                   new XElement("parola", parola),
                   new XElement("nume", nume),
                   new XElement("email", email)
                    //And so on ...
                );

                var myNewElement2 = new XElement(username);

                xDoc.Element("users").Add(myNewElement);
                xDoc.Element("friends").Add(myNewElement2);
                xDoc.Save("DB.xml");
                //Console.WriteLine("Inregistrare reusita");
                return 1;
            }
            else
            {
                //Console.WriteLine("Username deja folosit");
                return 0;
            }
        }

        public int AddFriend(string username, string friend)
        {
            var myNewElement = new XElement("friend", friend);

            XElement xDoc = XElement.Load("DB.xml");
            if (xDoc == null)
                //Console.WriteLine("S-a produs o eroare la incarcarea xml-ului");
                return 0;
            xDoc.Element("friends").Element(username).Add(myNewElement);
            xDoc.Save("DB.xml");
            return 1;
        }

        public int ChangeStatus(string username, string status)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("DB.xml");
            XmlElement formData = (XmlElement)xDoc.SelectSingleNode("//users/user[@username='" + username + "']");
            if (formData != null)
            {
                formData.SetAttribute("status", status);
            }
            xDoc.Save("DB.xml");
            return 1;
        }

        public string[] AllFriends(string username)
        {
            List<string> friends = new List<string>();

            XElement xDoc = XElement.Load("DB.xml");

            IEnumerable<XElement> address =
                from el in xDoc.Elements("friends").Elements(username).Elements("friend")
                select el;

            foreach (XElement el in address)
            {
                friends.Add((string)el);
            }
            return AllFriendsDetails(friends);
        }

        public List<string> GetFriends(string username)
        {
            List<string> friends = new List<string>();

            XElement xDoc = XElement.Load("DB.xml");
            IEnumerable<XElement> address =
                from el in xDoc.Elements("friends").Elements(username)
                select el;

            foreach (XElement el in address)
            {
                friends.Add((string)el.Element("friend"));
            }
            return friends;
        }

        public List<string> FindUsers(string query,string username)
        {
            List<string> Users = new List<string>();
            List<string> users = new List<string>();
            int f=0;
            XElement xDoc = XElement.Load("DB.xml");
            List<string> friends = GetFriends(username);

            IEnumerable<XElement> Ausers =
                from el in xDoc.Elements("users").Elements("user")
                where (string)el.Attribute("username") != username
                select el;
            foreach (XElement el in Ausers)
            {
                users.Add((string)el.Attribute("username"));
            }

            foreach (string el in users)
            {

                if (el.Contains(query))
                {
                    f = 0;
                    foreach(string u in friends)
                    {
                        if (u == el)
                        {
                            f = 1;
                            break;
                        }
                    }
                    if (f != 1)
                    {
                        Users.Add(el);
                        
                    }
                }
            }
            return Users;
        }

        public string[] AllFriendsDetails(List<string> friends)
        {
            List<List<string>> Details = new List<List<String>>();
            string[] detalii = new string[friends.Count];
            for (int j = 0; j < friends.Count; j++)
            {
                detalii[j] = string.Empty;
            }
            //username status
            //username2 status2
            int i = 0;
            List<string> user = new List<String>();
            XElement xDoc = XElement.Load("DB.xml");

            foreach (string el in friends)
            {
                user.Clear();
                user.Add(el);
                
                IEnumerable<XElement> address =
                    from elm in xDoc.Elements("users").Elements("user")
                    where (string)elm.Attribute("username") == el
                    select elm;

                foreach (XElement elm in address)
                {
                    //user.Add((string)elm.Attribute("username"));
                    user.Add((string)elm.Attribute("status"));
                }

                Details.Add(user);
                detalii[i] = (user.ElementAt(0) + " " + user.ElementAt(1)).ToString();
                i++;
            }
            return detalii;
        }

        public int AddMessage(string sender, string receiver, string message)
        {
            int count = 0;
            XElement xDoc = XElement.Load("DB.xml");

            if (xDoc == null)
                //Console.WriteLine("S-a produs o eroare la incarcarea xml-ului");
                return 0;

            IEnumerable<XElement> address =
                from el in xDoc.Elements("messages").Elements("message")
                where (string)el.Attribute("sender") == sender & (string)el.Attribute("receiver") == receiver
                select el;
            foreach (XElement el in address)
                count++;
            if(count == 0)
            {
                DateTime thisDay = DateTime.Today;
                var myNewElement = new XElement("message",
                    new XAttribute("sender", sender),
                    new XAttribute("receiver", receiver),
                    new XElement("text",
                    new XAttribute("created", thisDay.ToString()), message)
                    );

                xDoc.Element("messages").Add(myNewElement);
                xDoc.Save("DB.xml");
            }
            else
            {
                DateTime thisDay = DateTime.Today;
                var myNewElement = new XElement("text",
                    new XAttribute("created", thisDay.ToString()), message);

                xDoc.Element("messages").Element("message").Attribute("sender");
                xDoc.Save("DB.xml");
                return 1;
            }
            return 1;
        }
    }
}
