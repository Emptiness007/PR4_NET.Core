using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace FtpWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static int Id = -1;
        public static int Port;
        public static MainWindow mainWindow;
        public static IPAddress IPAddress;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            OpenPage();
        }

        public void OpenPage()
        {
            frame.Navigate(new auth());
        }

        //private void AddItemsFromList(List<string> items)
        //{
        //    // Находим родительский элемент (корневой) по тегу
        //    System.Windows.Controls.TreeViewItem parentItem = FindTreeViewItemByTag(treeViewFolders, "Root");
        //    if (parentItem != null)
        //    {
        //        // Перебираем список строк и добавляем каждый элемент в дерево
        //        foreach (var item in items)
        //        {
        //            System.Windows.Controls.TreeViewItem newItem = new System.Windows.Controls.TreeViewItem
        //            {
        //                Header = new System.Windows.Controls.TextBlock
        //                {
        //                    Text = item,
        //                    Foreground = System.Windows.Media.Brushes.White,
        //                    FontFamily = new System.Windows.Media.FontFamily("Consolas"),
        //                    Margin = new System.Windows.Thickness(5, 0, 0, 0)
        //                },
        //                Tag = item
        //            };

        //            // Добавляем новый элемент к родительскому
        //            parentItem.Items.Add(newItem);
        //        }
        //    }
        //}

        //// Метод для поиска элемента по тегу в дереве
        //private System.Windows.Controls.TreeViewItem FindTreeViewItemByTag(TreeView treeView, string tag)
        //{
        //    foreach (System.Windows.Controls.TreeViewItem item in treeView.Items)
        //    {
        //        if (item.Tag.ToString() == tag)
        //        {
        //            return item;
        //        }

        //        // Рекурсивно ищем в дочерних элементах
        //        System.Windows.Controls.TreeViewItem foundItem = FindTreeViewItemByTagRecursive(item, tag);
        //        if (foundItem != null)
        //        {
        //            return foundItem;
        //        }
        //    }
        //    return null;
        //}

        //// Рекурсивный метод для поиска элемента в поддеревьях
        //private System.Windows.Controls.TreeViewItem FindTreeViewItemByTagRecursive(System.Windows.Controls.TreeViewItem parentItem, string tag)
        //{
        //    foreach (System.Windows.Controls.TreeViewItem item in parentItem.Items)
        //    {
        //        if (item.Tag.ToString() == tag)
        //        {
        //            return item;
        //        }

        //        // Рекурсивно ищем в дочерних элементах
        //        System.Windows.Controls.TreeViewItem foundItem = FindTreeViewItemByTagRecursive(item, tag);
        //        if (foundItem != null)
        //        {
        //            return foundItem;
        //        }
        //    }
        //    return null;
        //}

        private void TreeViewFolders(object sender, MouseButtonEventArgs e)
        {

        }

        private void DownloadFiles(object sender, RoutedEventArgs e)
        {

        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ToggleFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                fullScreenButton.Icon = new SymbolIcon(SymbolRegular.FullScreenMinimize24);
            }
            else
            {
                this.WindowState = WindowState.Normal;
                fullScreenButton.Icon = new SymbolIcon(SymbolRegular.FullScreenMaximize24);
            }
        }
    }
}
