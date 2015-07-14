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
    /// Interaction logic for ParametersWindow.xaml
    /// </summary>
    public partial class ParametersWindow : Page
    {
        public static int Width = 550, Height = 150;
        string name;
        Dictionary<string, List<UIElement>> UIElements;
        Dictionary<string, Parameter> para;
        public ParametersWindow()
        {
            InitializeComponent();
            UIElements = new Dictionary<string, List<UIElement>>();
        }
        public void set(string name,Dictionary<string ,Parameter> pMenu){
            para = pMenu;
            ClearUI();
            Width = 500;
            Height = 100 + pMenu.Count * 15;
            this.WindowWidth = Width;
            this.WindowHeight = Height;
            GridX.Width = Math.Max(Width,550);
            GridX.Height = Math.Max(Height, 180) ;
            
            Width = (int)GridX.Width;
            Height = (int)GridX.Height;
            this.name = name;
            SetUI(pMenu);
        }
        void ClearUI()
        {
            CanvasX.Children.Clear();
        }
        void SetUI(Dictionary<string, Parameter> pMenu)
        {
            UIElements.Clear();
            for (int i = 0; i < pMenu.Count; i++)
            {
                Label lbl = new Label();
                lbl.Content = pMenu.ElementAt(i).Key;
                Canvas.SetTop(lbl, (i) * 50 + 10);
                Canvas.SetLeft(lbl, 10);
                CanvasX.Children.Add(lbl);
                if (pMenu.ElementAt(i).Value.type == ParameterType.INT || pMenu.ElementAt(i).Value.type == ParameterType.FLOAT)
                {
                    Slider sldr = new Slider();
                    sldr.Width = 400;
                    sldr.ValueChanged += sldr_ValueChanged;
                    sldr.Minimum = (double)pMenu.ElementAt(i).Value.min;
                    sldr.Maximum = (double)pMenu.ElementAt(i).Value.max;
                    if (pMenu.ElementAt(i).Value.type == ParameterType.INT)
                    {
                        sldr.TickFrequency = 1;
                    }
                    sldr.Value = (double)pMenu.ElementAt(i).Value.value;
                    Canvas.SetTop(sldr, (i) * 50 + 13);
                    Canvas.SetLeft(sldr, 75);
                    CanvasX.Children.Add(sldr);
                    TextBox txt = new TextBox();
                    Canvas.SetTop(txt, (i) * 50 + 13);
                    Canvas.SetLeft(txt, 500);
                    txt.Width = 40;
                    txt.Height = 25;
                    txt.TextChanged += txt_TextChanged;
                    txt.Text = pMenu.ElementAt(i).Value.value.ToString();
                    CanvasX.Children.Add(txt);
                    UIElements.Add(pMenu.ElementAt(i).Key, new List<UIElement> { lbl, sldr, txt });
                }
                else if (pMenu.ElementAt(i).Value.type == ParameterType.BOOl)
                {
                    CheckBox boxy = new CheckBox();
                    Canvas.SetTop(boxy, (i) * 50 + 13+3);
                    Canvas.SetLeft(boxy, 75);
                    boxy.Checked += checkbox_Check;
                    boxy.Unchecked += checkbox_UnCheck;
                    CanvasX.Children.Add(boxy);
                    UIElements.Add(pMenu.ElementAt(i).Key, new List<UIElement> { lbl, boxy });
                    
                }else{
                    TextBox path = new TextBox();
                    path.Width = 450;
                    Canvas.SetTop(path, (i) * 50 + 13);
                    Canvas.SetLeft(path, 75);
                    CanvasX.Children.Add(path);

                    Button open = new Button();
                    open.Content = "Browse";
                    open.Width = 50;
                    open.Height = path.Height;
                    open.Click += OpenClick;
                    Canvas.SetTop(open, (i) * 50 + 13);
                    Canvas.SetLeft(open, 500);
                    CanvasX.Children.Add(open);
                    UIElements.Add(pMenu.ElementAt(i).Key, new List<UIElement> { lbl, path, open });

                }
            }
        }

        private void checkbox_UnCheck(object sender, RoutedEventArgs e)
        {
            CheckBox boxy = sender as CheckBox;
            for (int i = 0; i < UIElements.Count; i++)
            {
                CheckBox btn = UIElements.ElementAt(i).Value[1] as CheckBox;
                if (boxy == btn)
                {
                    Label lbl = UIElements.ElementAt(i).Value[0] as Label;
                    string key = (string)lbl.Content;
                    Parameter p = para[key];
                    para.Remove(key);
                    p.value = false;
                    para.Add(key, p);
                }
            }
        }

        private void checkbox_Check(object sender, RoutedEventArgs e)
        {
            CheckBox boxy = sender as CheckBox;
            for (int i = 0; i < UIElements.Count; i++)
            {
                CheckBox btn = UIElements.ElementAt(i).Value[1] as CheckBox;
                if (boxy == btn)
                {
                    Label lbl = UIElements.ElementAt(i).Value[0] as Label;
                    string key = (string)lbl.Content;
                    Parameter p = para[key];
                    para.Remove(key);
                    p.value = true;
                    para.Add(key, p);
                }
            }
        }

        void OpenClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < UIElements.Count; i++)
            {
                Button btn = UIElements.ElementAt(i).Value[2] as Button;
                if (sender == btn)
                {
                    Label lbl = UIElements.ElementAt(i).Value[0] as Label;
                    string key=(string)lbl.Content;
                    Parameter p = para[key];
                    para.Remove(key);
                    p.value = ImageStream.OpenImageb();
                    TextBox path = UIElements.ElementAt(i).Value[1] as TextBox;
                    RImage img = p.value as RImage;
                    path.Text = img.Path.LocalPath;
                    para.Add(key, p);
                }
            }
        }
        
        void sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sldr = sender as Slider;
            foreach (var element in UIElements)
            {
                if (element.Value[1] == sender)
                {
                    TextBox txt = element.Value[2] as TextBox;
                    txt.Text = e.NewValue.ToString();
                    break;
                }
            }
        }

        void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            foreach (var element in UIElements)
            {
                if (element.Value[2] == sender)
                {
                    try
                    {
                        Slider sldr = element.Value[1] as Slider;
                        sldr.Value = double.Parse(txt.Text);
                        Parameter x = para[element.Key];
                        x.value = double.Parse(txt.Text);
                        para.Remove(element.Key);
                        para.Add(element.Key, x);
                    }
                    catch (Exception ec)
                    {
                        
                    }

                    break;
                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UIAssembler.hideParameter();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            UIAssembler.hideParameter();
            if (name != null)
            {
                Globals.Operations[name].setPara(para);
                Globals.Operations[name].run();
            }
        }
    }
}
