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
using System.Windows.Controls.Primitives;


namespace ImageResizing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Globals.mainWindow = this;
            Globals.paraWindow = new ParametersWindow();
            UIAssembler.init();
            
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UIAssembler.AssembleUI();
        }
        
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            
            ImageStream.OpenImage();
            UIAssembler.AssembleUI();
        }

        private void MainGrid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (Globals.UIElements.ContainsKey("StatBar"))
            {
                StatusBar x = Globals.UIElements["StatBar"] as StatusBar;
                x.Width = e.NewSize.Width;
            }
        }
    }
}
