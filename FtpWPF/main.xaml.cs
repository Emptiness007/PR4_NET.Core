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
                string message = "cd D:\\";
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
                        GetCurrentDirectory(directoryName);
                    }
                }
            }
        }

        private System.Windows.Controls.TreeViewItem FindTreeViewItemByTag(TreeView treeView, string tag)
        {
            foreach (System.Windows.Controls.TreeViewItem item in treeView.Items)
            {
                if (item.Tag.ToString() == tag)
                {
                    return item;
                }

                System.Windows.Controls.TreeViewItem foundItem = FindTreeViewItemByTagRecursive(item, tag);
                if (foundItem != null)
                {
                    return foundItem;
                }
            }
            return null;
        }

        private System.Windows.Controls.TreeViewItem FindTreeViewItemByTagRecursive(System.Windows.Controls.TreeViewItem parentItem, string tag)
        {
            foreach (System.Windows.Controls.TreeViewItem item in parentItem.Items)
            {
                if (item.Tag.ToString() == tag)
                {
                    return item;
                }

                System.Windows.Controls.TreeViewItem foundItem = FindTreeViewItemByTagRecursive(item, tag);
                if (foundItem != null)
                {
                    return foundItem;
                }
            }
            return null;
        }
    }

}
