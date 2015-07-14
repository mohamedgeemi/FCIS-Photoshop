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
    /// Interaction logic for SampleOp.xaml
    /// </summary>
    public partial class SampleOp : Page
    {
        public static int Width = 200;
        public static int Height = 194;

        public SampleOp()
        {
            InitializeComponent();
        }

        private void SmoothClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Smooth"].execute();
        }
        private void SharpenClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["Sharpen"].execute();
        }
        private void GussianClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["GussianFilter"].execute();
        }
        private void MDClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["MedianRemoval"].execute();
        }
        private void EDClick(object sender, RoutedEventArgs e)
        {
            Globals.Operations["EdgeDetect"].execute();
        }
    }
}
