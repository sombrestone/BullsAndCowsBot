using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace TelegramAsp.Controllers
{
    public class MessageHandler
    {
        static string messagesFilePath = Directory.GetCurrentDirectory() + "\\Resources\\exceptionMessages.xml";
        static string errorsLogFilePath = Directory.GetCurrentDirectory() + "\\Resources\\error.txt";

        public static string Error(Exception e)
        {
            File.WriteAllText(errorsLogFilePath, e.Message);
            return parce(e.Message);    
        }

        public static string GetValue(string code)
        {
            return parce(code);
        }

        static string parce(string code)
        {
            exists();
            string result = "На сервере произошла ошибка(";
            XDocument doc = XDocument.Load(messagesFilePath);
            XElement messages = doc.Element("messages");
            XElement message = messages.Element(code);
            if (message != null) result = message.Value;
            return result;
        }

        static void exists()
        {
            if (!File.Exists(messagesFilePath))
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("messages");
                root.Add(new XElement("unnownCommand", "Данной команды не существует."));
                root.Add(new XElement("nullArguments", "В команде не указан обязательный аргумент."));
                root.Add(new XElement("userNotFound", "Данного пользователя не существует в базе бота.\n" +
                        "Можно приглашать только тех польхователей, которые написали мне команду \"/start\"."));
                root.Add(new XElement("userExists", "Мы с вами уже знакомы."));
                root.Add(new XElement("selfInvite", "Вы пытаетесь пригласить сами себя!"));
                root.Add(new XElement("default", "На сервере произошла ошибка("));
                doc.Add(root);
                doc.Save(messagesFilePath);
            }
        }
    }
}
