using System;
using System.Collections.Generic;
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

namespace ImageResizing
{
    /// <summary>
    /// Interaction logic for FileMenu.xaml
    /// </summary>
    public partial class FileMenu : Page
    {
        public static int Width = 200;
        public static int Height = 116;
        public FileMenu()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ImageStream.OpenImage();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ImageStream.SaveImage();
        }
    }
}
