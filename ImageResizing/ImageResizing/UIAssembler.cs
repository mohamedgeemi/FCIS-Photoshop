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
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;

namespace ImageResizing
{
    class UIAssembler
    {
        public static void init()
        {
            TabControl Tb = new TabControl();
            Tb.Name = "TabManager";

            Globals.UIElements.Add(Tb.Name, Tb);
            Blur.init();
            ContentAwareResize.init();
            ImageProcOp.init();
        }
        
        public static double getWindowVC()
        {
            return (System.Windows.SystemParameters.PrimaryScreenHeight - 600) / 2;
        }
        public static double getWindowHC()
        {
            return (System.Windows.SystemParameters.PrimaryScreenWidth - 900) / 2;
        }
        public static void CloseTab(UIElement tb)
        {
            TabControl t = Globals.UIElements["TabManager"] as TabControl;
            t.Items.Remove(tb);
            if (Globals.UIElements.ContainsKey("StatBar"))
            {
                StatusBar sb = Globals.UIElements["StatBar"] as StatusBar;
                sb.Items.Clear();
                if (t.Items.Count > 0)
                {
                    TextBlock lbl = new TextBlock();
                    string co = "";
                    if (ImageStream.CurrentImage != null)
                    {
                        co += ImageStream.CurrentImage.Name + " | Width = ";
                        co += ImageStream.CurrentImage.Bitmap.Width + " | Height = ";
                        co += ImageStream.CurrentImage.Bitmap.Height;
                    }
                    lbl.Text = co;
                    sb.Items.Add(lbl);
                }
            }
        }
        public static void addTab(RImage img)
        {
            TabItem it = new TabItem();
            it.Header = img.Name;
            Grid g = new Grid();
            Image i = new Image();
            i.Source = img.Image;
            

            g.Children.Add(i);
            it.Content = g;
            TabControl tbMan = Globals.UIElements["TabManager"] as TabControl;
            tbMan.SelectionChanged += SelectionChanged;
            tbMan.VerticalAlignment = VerticalAlignment.Stretch;
            tbMan.HorizontalAlignment = HorizontalAlignment.Stretch;
            tbMan.Items.Add(it);
            tbMan.SelectedIndex = tbMan.Items.Count - 1;
        }
        public static void AssembleUI()
        {
            Globals.mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            Animator.Animate(Globals.mainWindow, MainWindow.WidthProperty, 900, 3);
            Animator.Animate(Globals.mainWindow, MainWindow.HeightProperty, 600, 3);
            Animator.Animate(Globals.mainWindow, MainWindow.TopProperty, getWindowVC(), 3);
            Animator.Animate(Globals.mainWindow, MainWindow.LeftProperty, getWindowHC(), 3);

            
            ClearUI();
            
            AssembleNUI();
        }
        static void ClearUI()
        {

            Globals.mainWindow.MainGrid.Children.Clear();
            Globals.mainWindow.MainGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 38, 38));
        }
        static void setCanvas()
        {
            Canvas mainCanvas = new Canvas();
            mainCanvas.Name = "mainCanvas";
            mainCanvas.Margin = new Thickness(0, 60, 0, 0);
            Globals.UIElements.Add("mainCanvas", mainCanvas);
            Globals.mainWindow.MainGrid.Children.Add(mainCanvas);
        }
        static void AssembleNUI()
        {

            Globals.mainWindow.MainGrid.Children.Add(Globals.UIElements["TabManager"]);
            setCanvas();
            Canvas mainCanvas = Globals.UIElements["mainCanvas"] as Canvas;
            StatusBar sb = new StatusBar();
            sb.Width = 900;
            sb.Height = 25;
            sb.VerticalAlignment = VerticalAlignment.Bottom;
            TextBlock lbl = new TextBlock();
            string co = "";
            if (ImageStream.CurrentImage != null)
            {
                co += ImageStream.CurrentImage.Name + " | Width = ";
                co += ImageStream.CurrentImage.Bitmap.Width + " | Height = ";
                co += ImageStream.CurrentImage.Bitmap.Height;
            }
            lbl.Text = co;
            sb.Items.Add(lbl);
            Globals.UIElements.Add("StatBar", sb);
            Globals.mainWindow.MainGrid.Children.Add(sb);
            assembleMenus();
            assembleMenuFrame();
        }
        static void assembleMenus()
        {
            addMenuButton("FileMenuButton", "FileMenu", "FileMenuButton");
            addMenuButton("BlurMenuButton", "BlurMenu", "BlurMenuButton");
            addMenuButton("ResizeMenuButton", "ResizeMenu", "ResizeMenuButton");
            addMenuButton("FilterMenuButton", "FilterMenu", "FilterMenuButton");
            addMenuButton("AlgebraMenuButton", "ImageAlgebra", "AlgebraMenuButton");
            addMenuButton("NoiseMenuButton", "NoiseMenu", "NoiseMenuButton");
        }

        public static void addMenuButton(string name,string PageName,string styleName)
        {
            Canvas mainCanvas = Globals.UIElements["mainCanvas"] as Canvas;
            Button FileMenuButton = new Button();
            FileMenuButton.Name = PageName;
            FileMenuButton.MouseEnter += MenuButtonOver;
            FileMenuButton.Style = Globals.mainWindow.FindResource(styleName) as Style;
            Canvas.SetTop(FileMenuButton, 10+65*(Globals.menuButtonCount));
            Canvas.SetLeft(FileMenuButton, 10);

            mainCanvas.Children.Add(FileMenuButton);
            Globals.UIElements.Add(name, FileMenuButton);
            Globals.menuButtonCount++;
        }
        static void assembleMenuFrame()
        {
            Canvas mainCanvas = Globals.UIElements["mainCanvas"] as Canvas;
            Frame MenuFrame = new Frame();
            
            //MenuFrame.Width = FileMenu.Width;
            //MenuFrame.Height = FileMenu.Height;
            MenuFrame.MouseLeave += MenuFrameLeave;
            MenuFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            MenuFrame.Visibility = Visibility.Hidden;
            mainCanvas.Children.Add(MenuFrame);
            Globals.UIElements.Add("MenuFrame", MenuFrame);

            Frame ParameterFrame = new Frame();


            ParameterFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            ParameterFrame.Visibility = Visibility.Hidden;
            mainCanvas.Children.Add(ParameterFrame);
            Globals.UIElements.Add("ParameterFrame", ParameterFrame);

            Frame ProFrame = new Frame();
            ProFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            ProFrame.Visibility = Visibility.Hidden;
            mainCanvas.Children.Add(ProFrame);
            Globals.UIElements.Add("ProFrame", ProFrame);

            

        }
        public static void ShowProWin()
        {
            if (Globals.UIElements.ContainsKey("StopCanvas"))
            {
                Canvas Stop = Globals.UIElements["StopCanvas"] as Canvas;
                Stop.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                Stop.Opacity = .2;
                Stop.Visibility = Visibility.Visible;
                //Globals.mainWindow.MainGrid.Children.Add(Stop);
            }
            else
            {
                Canvas Stop = new Canvas();
                Stop.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                Stop.Opacity = .2;
                Globals.UIElements.Add("StopCanvas", Stop);

                Globals.mainWindow.MainGrid.Children.Add(Stop);
            }
            Frame ProFrame = Globals.UIElements["ProFrame"] as Frame;
            if (Globals.mainWindow.WindowState != WindowState.Maximized)
            {
                Canvas.SetTop(ProFrame, (Globals.mainWindow.Height - ParametersWindow.Height) / 2);
                Canvas.SetLeft(ProFrame, (Globals.mainWindow.Width - ParametersWindow.Width) / 2);
            }
            else
            {
                Canvas.SetTop(ProFrame, (SystemParameters.PrimaryScreenHeight - ParametersWindow.Height) / 2);
                Canvas.SetLeft(ProFrame, (SystemParameters.PrimaryScreenWidth - ParametersWindow.Width) / 2);
            }
            ProFrame.Opacity = 0;
            ProFrame.Content = Globals.ProWin;
            ProFrame.Visibility = Visibility.Visible;
            Animator.Animate(ProFrame, Frame.OpacityProperty, 1, 0);

        }
        public static void HideProWin()
        {
            Globals.ProWin.setProgress(0);
            Frame ProFrame = Globals.UIElements["ProFrame"] as Frame;
            ProFrame.Opacity = 0;
            ProFrame.Visibility = Visibility.Hidden;
            Canvas Stop = Globals.UIElements["StopCanvas"] as Canvas;
            Stop.Visibility = Visibility.Hidden;
            //Globals.mainWindow.MainGrid.Children.Remove(Stop);
        }
        public static void ShowParameter(string Name,Dictionary<string, Parameter> pMenu)
        {

            
            Globals.paraWindow.set(Name, pMenu);
            Frame ParameterFrame = Globals.UIElements["ParameterFrame"] as Frame;
            if (Globals.mainWindow.WindowState != WindowState.Maximized)
            {
                Canvas.SetTop(ParameterFrame, (Globals.mainWindow.Height - ParametersWindow.Height) / 2);
                Canvas.SetLeft(ParameterFrame, (Globals.mainWindow.Width - ParametersWindow.Width) / 2);
            }
            else
            {
                Canvas.SetTop(ParameterFrame, (SystemParameters.PrimaryScreenHeight - ParametersWindow.Height) / 2);
                Canvas.SetLeft(ParameterFrame, (SystemParameters.PrimaryScreenWidth - ParametersWindow.Width) / 2);
            }
            ParameterFrame.Opacity = 0;
            ParameterFrame.Content = Globals.paraWindow;
            ParameterFrame.Visibility = Visibility.Visible;
            Animator.Animate(ParameterFrame, Frame.OpacityProperty, 1, 1);
            
            
        }
        public static void hideParameter()
        {
            Frame ParameterFrame = Globals.UIElements["ParameterFrame"] as Frame;
            Animator.Animate(ParameterFrame, Frame.OpacityProperty, 0, 1);
            ParameterFrame.Visibility = Visibility.Hidden;
            
        }
        static void ShowMenu(string name, Button sender)
        {
            Frame MenuFrame = Globals.UIElements["MenuFrame"] as Frame;
            MenuFrame.Source = new Uri(name, UriKind.Relative);
            Page c= new Page();
            
            Canvas.SetTop(MenuFrame, Canvas.GetTop(sender)+5);
            Canvas.SetLeft(MenuFrame, Canvas.GetLeft(sender)+75);
            MenuFrame.Opacity = 0;
            MenuFrame.Visibility = Visibility.Visible;
            
            Animator.Animate(MenuFrame, Frame.OpacityProperty, 1, 1);
        }
        public static void updateTab(BitmapSource source)
        {
            TabControl TC = Globals.UIElements["TabManager"] as TabControl;
            Grid g = TC.SelectedContent as Grid;
            Image img = g.Children[0] as Image;
            img.Source = source;
        }
        public static void ShowMsg(string msgText)
        {
            MessageBox.Show(msgText,"Time");
        }
        private static void MenuButtonOver(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            ShowMenu(s.Name + ".xaml", s);
        }
        private static void MenuFrameLeave(object sender, MouseEventArgs e)
        {
            Frame MenuFrame = sender as Frame;
            MenuFrame.Opacity = 0;
            MenuFrame.Source = null;
            MenuFrame.Visibility = Visibility.Hidden;
        }
        private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            TabItem tb = e.AddedItems[0] as TabItem;
            Grid gd = tb.Content as Grid;
            Image img = VisualTreeHelper.GetChild(gd, 0) as Image;
            ImageStream.setCurrentImage(img);
            if (Globals.UIElements.ContainsKey("StatBar"))
            {
                
                TextBlock lbl = new TextBlock();
                string co = "";
                if (ImageStream.CurrentImage != null)
                {
                    co += ImageStream.CurrentImage.Name + " | Width = ";
                    co += ImageStream.CurrentImage.Bitmap.Width + " | Height = ";
                    co += ImageStream.CurrentImage.Bitmap.Height;
                }
                lbl.Text = co;
                StatusBar sb = Globals.UIElements["StatBar"] as StatusBar;
                sb.Items.Clear();
                sb.Items.Add(lbl);
            }
        }
    }
}
