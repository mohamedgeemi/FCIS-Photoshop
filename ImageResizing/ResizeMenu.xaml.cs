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
    /// Interaction logic for ResizeMenu.xaml
    /// </summary>
    public partial class ResizeMenu : Page
    {
        public ResizeMenu()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Globals.Operations["CAResizing"].execute();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Globals.Operations["BilResize"].execute();
        }
    }
}
