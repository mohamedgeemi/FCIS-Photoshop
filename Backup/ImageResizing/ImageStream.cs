using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    struct Coor
    {
        public string key;
        public int ix;
    }
    class ImageStream
    {
        static Dictionary<string, List<RImage>> Register = new Dictionary<string,List<RImage>>();
        public static RImage cImage = null;
        public static RImage CurrentImage
        {
            get { return cImage; }
            
        }
        public static void setImage(Coor cords,RImage img)
        {
            Register[cords.key][cords.ix] = img;
        }
        public static void setCurrentImage(Image img)
        {

            for (int i = 0; i < Register.Count; i++)
            {
                foreach(RImage im in Register.ElementAt(i).Value)
                    if (im.Source == img.Source)
                    {
                        cImage = im;
                        return;
                    }
            }
        }
        public static Coor getCoor(RImage img)
        {
            Coor res = new Coor();
            for (int i = 0; i < Register.Count; i++)
            {
                foreach (RImage im in Register.ElementAt(i).Value)
                    if (img.Source == im.Source)
                    {
                        res.key = Register.ElementAt(i).Key;
                        res.ix = Register.ElementAt(i).Value.IndexOf(im);
                        return res ;
                    }
            }
            return new Coor();
        }
        public static void OpenImage()
        {
            OpenFileDialog op = new OpenFileDialog();
            DialogResult dr = op.ShowDialog();
            if (dr == DialogResult.OK)
            {
                RImage img = new RImage(op.SafeFileName, new Uri(op.FileName));
                if (!Register.ContainsKey(op.SafeFileName)){
                    List<RImage> lst = new List<RImage>();
                    lst.Add(img);
                    Register.Add(op.SafeFileName, lst);
                }else
                {
                    Register[op.SafeFileName].Add(img);
                }
                cImage = img;
                
                UIAssembler.addTab(img);
            }
        }
        public static RImage OpenImageb()
        {
            OpenFileDialog op = new OpenFileDialog();
            DialogResult dr = op.ShowDialog();
            RImage img = null;
            if (dr == DialogResult.OK)
            {
                img = new RImage(op.SafeFileName, new Uri(op.FileName));
            }
            return img;

        }
        public static void SaveImage()
        {
            if (cImage != null)
            {
                SaveFileDialog sv = new SaveFileDialog();
                sv.Filter = "PNG|*.png|GIF|*.gif|BMP|*.bmp|JPEG|*.jpg;*.jpeg";
                DialogResult dr = sv.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                    string s = sv.FileName;
                    jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(cImage.Image));
                    System.IO.FileStream fileStream = new System.IO.FileStream(s, System.IO.FileMode.Create);
                    try
                    {
                        jpegBitmapEncoder.Save(fileStream);
                    }
                    finally
                    {
                        fileStream.Dispose();
                    }

                }
            }
        }
    }
}
