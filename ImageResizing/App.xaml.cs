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
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            Button x = sender as Button;
            UIElement Grid = VisualTreeHelper.GetParent(x) as UIElement;
            UIElement tb = VisualTreeHelper.GetParent(Grid) as UIElement;
            UIAssembler.CloseTab(tb);
        }
    }
}
