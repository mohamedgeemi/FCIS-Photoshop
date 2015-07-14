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
using System.Windows.Media.Animation;

namespace ImageResizing
{
    class Animator
    {
        public static void Animate(UIElement element, DependencyProperty property, double toValue, double duration)
        {
            DoubleAnimation Animation = new DoubleAnimation();
            Animation.To = toValue;
            Animation.Duration = new Duration(TimeSpan.FromSeconds(duration));
            element.BeginAnimation(property, Animation);
        }
    }
}
