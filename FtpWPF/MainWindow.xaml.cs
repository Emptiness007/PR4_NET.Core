using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
            EtherealWind.Tools.ImageExtensions imageExtensions = new EtherealWind.Tools.ImageExtensions();
            string gifPath = System.IO.Path.Combine(Directory.GetCurrentDirectory() + "/Gif/");
            BitmapImage[] frames = imageExtensions.BitmapsInit(gifPath, "png", 20);
            imageExtensions.PlayAsGif(bottomGif, frames, 5.0, true);
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
