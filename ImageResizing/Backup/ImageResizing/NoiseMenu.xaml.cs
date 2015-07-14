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
    /// Interaction logic for NoiseMenu.xaml
    /// </summary>
    public partial class NoiseMenu : Page
    {
        public NoiseMenu()
        {
            InitializeComponent();
        }

        private void SltPprClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Sltppr"].execute();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Median"].execute();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Tint"].execute();
        }
    }
}
