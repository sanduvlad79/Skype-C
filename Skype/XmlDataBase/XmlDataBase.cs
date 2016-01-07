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

        public int userExists(string username)
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
            if (userExists(username) == 1)
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
            var myNewElement = new XElement("friend", "ion");

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
    }
}