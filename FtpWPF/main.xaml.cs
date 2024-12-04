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
using System.Xml.Linq;
using System.Linq;

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
            treeViewFolders.Items.Clear();
            AddBack("C:\\");
            AddSubDirectories("C:\\");
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

                        IPEndPoint endPoint = new IPEndPoint(Client.Program.IPAddress, Client.Program.Port);
                        Socket socket = new Socket(
                            AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
                        socket.Connect(endPoint);

                        if (socket.Connected)
                        {
                            byte[] fileData = System.IO.File.ReadAllBytes(selectedFilePath);

                            FileInfoFTP fileInfo = new FileInfoFTP(fileData, System.IO.Path.GetFileName(selectedFilePath));
                           
                            string jsonData = JsonConvert.SerializeObject(fileInfo);

                            ViewModelSend viewModelSend = new ViewModelSend(jsonData, MainWindow.Id);
                            byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                            int BytesSend = socket.Send(messageByte);
                            byte[] bytes = new byte[10485760];
                            int BytesRec = socket.Receive(bytes);
                            string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                        }
                        socket.Close();

                        MessageBox.Show($"Файл {System.IO.Path.GetFileName(selectedFilePath)} был успешно добавлен в директорию {currentDirectory}");

                        treeViewFolders.Items.Clear();
                        AddBack("C:\\");
                        AddSubDirectories("C:\\");

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
            ListBoxItem newItem = new ListBoxItem
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

            newItem.Content = stackPanel;
            newItem.MouseDoubleClick += TreeViewItem_Click;

            treeViewFolders.Items.Add(newItem);
        }

        private void TreeViewItem_Click(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem clickedItem = sender as ListBoxItem;
            if (clickedItem != null)
            {
                var stackPanel = clickedItem.Content as StackPanel;
                if (stackPanel != null && stackPanel.Children.Count >= 2)
                {
                    var textBlock = stackPanel.Children[1] as TextBlock;
                    if (textBlock != null)
                    {
                        string name = textBlock.Text;

                        string parentPath = currDir + clickedItem.Tag;


                        if (Directory.Exists(parentPath))
                        {
                            currDir = parentPath;
                            path.Text = currDir;
                            allFolders = GetFilesFolders(parentPath);
                            treeViewFolders.Items.Clear();
                            AddBack(name);
                            AddSubDirectories(parentPath);
                        }
                        else if (System.IO.File.Exists(parentPath))
                        {
                            OpenFile(parentPath);
                        }
                    }
                }
            }
        }

        private void AddBack(string name)
        {
            ListBoxItem newItem = new ListBoxItem
            {
                Tag = name
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock
            {
                Text = "Back",
                Foreground = System.Windows.Media.Brushes.White,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                Margin = new System.Windows.Thickness(5, 0, 0, 0)
            };

            stackPanel.Children.Add(textBlock);

            newItem.Content = stackPanel;
            newItem.MouseDoubleClick += GoBack;

            treeViewFolders.Items.Add(newItem);
        }

        private string GetCurrentPath(ListBoxItem item)
        {
            string path = string.Empty;

            while (item != null && item.Parent != null)
            {
                var parentItem = item.Parent as ListBoxItem;
                if (parentItem != null)
                {
                    var stackPanel = parentItem.Content as StackPanel;
                    if (stackPanel != null && stackPanel.Children.Count >= 2)
                    {
                        var textBlock = stackPanel.Children[1] as TextBlock;
                        if (textBlock != null)
                        {
                            path = System.IO.Path.Combine(textBlock.Text, path);
                        }
                    }
                }
                item = item.Parent as ListBoxItem;
            }

            if (!string.IsNullOrEmpty(path))
            {
                path = System.IO.Path.Combine(@"D:\", path);
            }
            return path;
        }


        private List<string> GetFilesFolders(string dir)
        {
            List<string> FoldersFiles = new List<string>();
            IPEndPoint endPoint = new IPEndPoint(Client.Program.IPAddress, Client.Program.Port);
            Socket socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            socket.Connect(endPoint);
            if (socket.Connected)
            {
                string message = "cd " + dir;
                ViewModelSend viewModelSend = new ViewModelSend(message, MainWindow.Id);
                byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                int BytesSend = socket.Send(messageByte);
                byte[] bytes = new byte[10485760];
                int BytesRec = socket.Receive(bytes);
                string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
                ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);
                FoldersFiles = JsonConvert.DeserializeObject<List<string>>(viewModelMessage.Data);
            }

            socket.Close();

            return FoldersFiles;
        }

        private void AddSubDirectories(string directoryPath)
        {
            try
            {
                path.Text = directoryPath;
                currDir = directoryPath;
                List<string> subdirectories = new List<string>();
                List<string> files = new List<string>();
                allFolders = GetFilesFolders(directoryPath);
                foreach (var i in allFolders)
                {
                    if (i.EndsWith("/"))
                    {
                        subdirectories.Add(i);
                        
                        AddTreeViewItemWithIcon(i);
                    }
                    else
                    {
                        files.Add(i);
                        AddTreeViewItemWithIcon(i);
                    }
                         
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
                    IPEndPoint endPoint = new IPEndPoint(Client.Program.IPAddress, Client.Program.Port);
                    Socket socket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp);
                    socket.Connect(endPoint);

                    if (socket.Connected)
                    {
                        string message = "get " + filePath;
                        ViewModelSend viewModelSend = new ViewModelSend(message, MainWindow.Id);
                        byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                        int BytesSend = socket.Send(messageByte);

                        byte[] bytes = new byte[10485760]; // 10MB buffer size
                        int BytesRec = socket.Receive(bytes);

                        string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);

                        string downloadPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", System.IO.Path.GetFileName(filePath));

                        System.IO.File.WriteAllBytes(downloadPath, bytes.Take(BytesRec).ToArray());
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = downloadPath,
                            UseShellExecute = true
                        });
                    }
                    socket.Close();

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
            ListBoxItem newItem = new ListBoxItem
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

            newItem.Content = stackPanel;
            newItem.MouseDoubleClick += TreeViewItem_Click;

            parentItem.Items.Add(newItem);
        }

        private void AddTreeViewItemWithIconToParent(TreeView parentItem, string name, bool isDirectory)
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

        private void RemoveChildDirectories(ItemCollection items)
        {
            foreach (var item in items)
            {
                TreeViewItem treeViewItem = item as TreeViewItem;
                if (treeViewItem != null)
                {
                    RemoveChildDirectories(treeViewItem.Items);

                    treeViewItem.Items.Clear();
                }
            }
        }

        private void GoBack(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(currDir))
                {
                    string parentDir = System.IO.Directory.GetParent(currDir)?.FullName;

                    if (parentDir != null)
                    {
                        path.Text = currDir;

                        treeViewFolders.Items.Clear();
                        AddBack(currDir);
                        AddSubDirectories(ShortenPath(currDir));
                        if (path.Text.Length < 5)
                        {
                            path.Text = path.Text.Substring(0, 3);
                            currDir = currDir.Substring(0, 3);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You are already at the root directory!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when navigating back: {ex.Message}");
            }
        }

        private void GetPath(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                treeViewFolders.Items.Clear();
                AddBack(path.Text);
                AddSubDirectories(path.Text);
            }
        }

        public static string ShortenPath(string path)
        {
            string normalizedPath = System.IO.Path.GetFullPath(path);

            string parentDirectory = Directory.GetParent(normalizedPath)?.FullName;
            currDir = $"{Directory.GetParent(parentDirectory)?.FullName}/";
            return $"{Directory.GetParent(parentDirectory)?.FullName}/"  ?? normalizedPath;
        }
    }

}
