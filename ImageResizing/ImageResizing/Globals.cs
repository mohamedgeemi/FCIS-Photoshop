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
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace ImageResizing
{
    class Globals
    {
        public static MainWindow mainWindow;
        public static ParametersWindow paraWindow;
        public static int menuButtonCount = 0;
        public static Dictionary<string, UIElement> UIElements = new Dictionary<string,UIElement>();
        public static Dictionary<string, Operation> Operations = new Dictionary<string, Operation>();
        public static ProgressbarWin ProWin = new ProgressbarWin();

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage res =new BitmapImage();

            try
            {
               BitmapSource retval = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                MemoryStream memoryStream = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(retval));
                encoder.Save(memoryStream);



               res.BeginInit();
               res.StreamSource = new MemoryStream(memoryStream.ToArray());
               res.EndInit();
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return res;
      }


    }
}
