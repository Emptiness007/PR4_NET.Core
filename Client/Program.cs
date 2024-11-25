using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Common;
using System.IO;

namespace Client
{
    public class Program
    {

        public static IPAddress IPAddress;
        public static int Port;
        public static int Id = -1;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите IP адрес сервера: ");
            string sIpAdress = Console.ReadLine();

            Console.WriteLine("Введите порт: ");
            string sPort = Console.ReadLine();

            if (int.TryParse(sPort, out Port) && IPAddress.TryParse(sIpAdress, out IPAddress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Данные успешно введены. Подключаюсь к серверу.");
                while (true)
                {
                    ConnectServer();
                }
            }
        }

        
    }
}
