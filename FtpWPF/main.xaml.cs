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
using static System.Net.WebRequestMethods;

namespace FtpWPF
{
    /// <summary>
    /// Логика взаимодействия для main.xaml
    /// </summary>
    public partial class main : Page
    {

        private static List<string> allFolders = new List<string>();
        private static string currDir;

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



        private void TreeViewFolders(object sender, MouseButtonEventArgs e)
        {
        }

        private void DownloadFiles(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Все файлы (*.*)|*.*";

                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    string currentDirectory = currDir;

                    if (!string.IsNullOrEmpty(currentDirectory))
                    {
                        string destinationPath = System.IO.Path.Combine(currentDirectory, System.IO.Path.GetFileName(selectedFilePath));

                        System.IO.File.Copy(selectedFilePath, destinationPath, true);

                        MessageBox.Show($"Файл {System.IO.Path.GetFileName(selectedFilePath)} был успешно добавлен в директорию {currentDirectory}");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось определить текущую директорию.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении файла: {ex.Message}");
            }
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
                        string name = textBlock.Text;

                        string parentPath = GetCurrentPath(clickedItem);

                        string fullPath = System.IO.Path.Combine(parentPath, name);

                        if (Directory.Exists(fullPath) && clickedItem.Items.Count == 0)
                        {
                            AddSubDirectories(clickedItem, fullPath);
                        }
                        else if (System.IO.File.Exists(fullPath))
                        {
                            OpenFile(fullPath);
                        }
                    }
                }
            }
        }

        private string GetCurrentPath(TreeViewItem item)
        {
            string path = string.Empty;

            while (item != null && item.Parent != null)
            {
                var parentItem = item.Parent as TreeViewItem;
                if (parentItem != null)
                {
                    var stackPanel = parentItem.Header as StackPanel;
                    if (stackPanel != null && stackPanel.Children.Count >= 2)
                    {
                        var textBlock = stackPanel.Children[1] as TextBlock;
                        if (textBlock != null)
                        {
                            path = System.IO.Path.Combine(textBlock.Text, path);
                        }
                    }
                }
                item = item.Parent as TreeViewItem;
            }

            if (!string.IsNullOrEmpty(path))
            {
                path = System.IO.Path.Combine(@"D:\", path);
            }
            return path;
        }




        private void AddSubDirectories(TreeViewItem parentItem, string directoryPath)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(Client.Program.IPAddress, Client.Program.Port);
                Socket socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                socket.Connect(endPoint);
                path.Text = directoryPath;
                currDir = directoryPath;
                if (socket.Connected)
                {
                    string message = "cd " + directoryPath;
                    ViewModelSend viewModelSend = new ViewModelSend(message, MainWindow.Id);
                    byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                    int BytesSend = socket.Send(messageByte);
                    byte[] bytes = new byte[10485760];
                    int BytesRec = socket.Receive(bytes);
                    string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                }
                socket.Close();
                string[] subdirectories = Directory.GetDirectories(directoryPath);
                string[] files = Directory.GetFiles(directoryPath);

                foreach (var subdirectory in subdirectories)
                {
                    string subdirectoryName = System.IO.Path.GetFileName(subdirectory);
                    AddTreeViewItemWithIconToParent(parentItem, subdirectoryName, true);
                }

                foreach (var file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    AddTreeViewItemWithIconToParent(parentItem, fileName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subdirectories: {ex.Message}");
            }
        }

        private void OpenFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Файл не существует!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}");
            }
        }

        private void AddTreeViewItemWithIconToParent(TreeViewItem parentItem, string name, bool isDirectory)
        {
            TreeViewItem newItem = new TreeViewItem
            {
                Tag = name
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Wpf.Ui.Controls.SymbolIcon icon;
            if (isDirectory)
            {
                icon = new Wpf.Ui.Controls.SymbolIcon(Wpf.Ui.Controls.SymbolRegular.Folder24);
            }
            else
            {
                icon = new Wpf.Ui.Controls.SymbolIcon(Wpf.Ui.Controls.SymbolRegular.Document24);
            }

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

            parentItem.Items.Add(newItem);
        }

    }

}
