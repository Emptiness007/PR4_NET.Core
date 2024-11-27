using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FtpWPF
{
    /// <summary>
    /// Логика взаимодействия для auth.xaml
    /// </summary>
    public partial class auth : Page
    {
        public auth()
        {
            InitializeComponent();
            EtherealWind.Tools.ImageExtensions imageExtensions = new EtherealWind.Tools.ImageExtensions();
            string gifPath = System.IO.Path.Combine(Directory.GetCurrentDirectory() + "/Gif/");
            BitmapImage[] frames = imageExtensions.BitmapsInit(gifPath, "png", 20);
            imageExtensions.PlayAsGif(bottomGif, frames, 5.0, true);
        }

        public void ConnectServer()
        {

        }

        private void Login(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.IPAddress = IPAddress.Parse(ip.Text);
                MainWindow.Port = int.Parse(port.Text);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip.Text), int.Parse(port.Text));
                Socket socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                socket.Connect(endPoint);

                if (socket.Connected)
                {

                    string message = $"connect {login.Text} {password.Text}";
                    ViewModelSend viewModelSend = new ViewModelSend(message, -1);
                    byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                    int BytesSend = socket.Send(messageByte);
                    byte[] bytes = new byte[10485760];
                    int BytesRec = socket.Receive(bytes);
                    string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                    ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);

                    if (viewModelMessage.Command == "authorization")
                    {
                        MainWindow.Id = int.Parse(viewModelMessage.Data);
                        viewModelSend.Id = MainWindow.Id;
                    }

                    if (MainWindow.Id > 0)
                    {
                        MainWindow.mainWindow.frame.Navigate(new main());
                        message = $"cd";

                    }
                }
                socket.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Подключение не удалось!");
            }
        }
    }
}
