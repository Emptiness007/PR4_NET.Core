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
using Common;
using Newtonsoft.Json;

namespace FtpWPF
{
    /// <summary>
    /// Логика взаимодействия для main.xaml
    /// </summary>
    public partial class main : Page
    {

        private static List<string> allFolders = new List<string>();

        public main()
        {
            InitializeComponent();

            EtherealWind.Tools.ImageExtensions imageExtensions = new EtherealWind.Tools.ImageExtensions();
            string gifPath = System.IO.Path.Combine(Directory.GetCurrentDirectory() + "/Gif/");
            BitmapImage[] frames = imageExtensions.BitmapsInit(gifPath, "png", 20);
            imageExtensions.PlayAsGif(bottomGif, frames, 5.0, true);
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> allDisk = new List<string>();
            foreach (DriveInfo drive in drives)
            {
                AddTreeViewItemWithIcon(drive.Name);
            }
            SendMessage();
        }

        private void SendMessage()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(MainWindow.IPAddress, MainWindow.Port);
                Socket socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                socket.Connect(endPoint);

                if (socket.Connected)
                {

                    string message = $"cd";
                    ViewModelSend viewModelSend = new ViewModelSend(message, MainWindow.Id);
                    byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                    int BytesSend = socket.Send(messageByte);
                    byte[] bytes = new byte[10485760];
                    int BytesRec = socket.Receive(bytes);
                    string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                    ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);
                }
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Подключение не удалось!");
            }
        }

        private void GetCurrentDirectory(string dir)
        {
            IPEndPoint endPoint = new IPEndPoint(Client.Program.IPAddress, Client.Program.Port);
            Socket socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            socket.Connect(endPoint);

            if (socket.Connected)
            {
                string message = "cd " + dir;
                ViewModelSend viewModelSend = new ViewModelSend(message, 1);
                byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                int BytesSend = socket.Send(messageByte);
                byte[] bytes = new byte[10485760];
                int BytesRec = socket.Receive(bytes);
                string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);
                allFolders = JsonConvert.DeserializeObject<List<string>>(viewModelMessage.Data);
                foreach (string Name in allFolders)
                {
                    System.Windows.MessageBox.Show(Name);
                }
            }
            socket.Close();
        }


        private void TreeViewFolders(object sender, MouseButtonEventArgs e)
        {
        }

        private void DownloadFiles(object sender, RoutedEventArgs e)
        {

        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {

        }

        private void AddTreeViewItemWithIcon(string name)
        {
            TreeViewItem newItem = new TreeViewItem
            {
                Tag = name
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Wpf.Ui.Controls.SymbolIcon icon = new Wpf.Ui.Controls.SymbolIcon(Wpf.Ui.Controls.SymbolRegular.Folder24);

            TextBlock textBlock = new TextBlock
            {
                Text = name,
                Foreground = System.Windows.Media.Brushes.White,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                Margin = new System.Windows.Thickness(5, 0, 0, 0)
            };

            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(textBlock);

            newItem.Header = stackPanel;
            newItem.MouseDoubleClick += TreeViewItem_Click;

            treeViewFolders.Items.Add(newItem);
        }

        private void TreeViewItem_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TreeViewItem clickedItem = sender as System.Windows.Controls.TreeViewItem;

            if (clickedItem != null)
            {
                var stackPanel = clickedItem.Header as StackPanel;
                if (stackPanel != null && stackPanel.Children.Count >= 2)
                {
                    var textBlock = stackPanel.Children[1] as TextBlock;
                    if (textBlock != null)
                    {
                        string directoryName = textBlock.Text;

                        string currentPath = GetCurrentDirectoryPath(directoryName);

                        if (clickedItem.Items.Count == 0)
                        {
                            AddSubDirectories(clickedItem, currentPath);
                        }
                    }
                }
            }
        }

        private string GetCurrentDirectoryPath(string directoryName)
        {
            return System.IO.Path.Combine(@"D:\", directoryName);
        }

        private void AddSubDirectories(TreeViewItem parentItem, string directoryPath)
        {
            try
            {
                string[] subdirectories = Directory.GetDirectories(directoryPath);

                foreach (var subdirectory in subdirectories)
                {
                    string subdirectoryName = System.IO.Path.GetFileName(subdirectory);
                    AddTreeViewItemWithIconToParent(parentItem, subdirectoryName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subdirectories: {ex.Message}");
            }
        }

        private void AddTreeViewItemWithIconToParent(TreeViewItem parentItem, string subdirectoryName)
        {
            TreeViewItem subItem = new TreeViewItem
            {
                Tag = subdirectoryName
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Wpf.Ui.Controls.SymbolIcon icon = new Wpf.Ui.Controls.SymbolIcon(Wpf.Ui.Controls.SymbolRegular.Folder24);

            TextBlock textBlock = new TextBlock
            {
                Text = subdirectoryName,
                Foreground = System.Windows.Media.Brushes.White,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                Margin = new System.Windows.Thickness(5, 0, 0, 0)
            };

            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(textBlock);

            subItem.Header = stackPanel;
            subItem.MouseDoubleClick += TreeViewItem_Click;

            parentItem.Items.Add(subItem);
        }

    }

}
