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
    /// Interaction logic for FilterMenu.xaml
    /// </summary>
    public partial class FilterMenu : Page
    {
        public FilterMenu()
        {
            InitializeComponent();
        }

        private void BiResizeClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["BilResize"].execute();
        }

        private void GrayscaleClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Greyscale"].execute();
        }

        private void BrightnessClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Brightness"].execute();
        }

        private void InvertClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Invert"].execute();
        }

        private void GammaClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Gamma"].execute();
        }

        private void ContrastClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Contrast"].execute();
        }
    }
}
