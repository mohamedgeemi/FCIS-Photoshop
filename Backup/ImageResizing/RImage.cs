using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Security.Cryptography;

namespace ImageResizing
{
    public enum CompareState { SizeMisMatch, OK, PixelMisMatch }
    struct Pixel{
        public byte r,g,b;
        public Pixel(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
    struct RImageBack
    {
        public Bitmap FoImage;
        public BitmapImage img;
    }
    class RImage
    {
        string name;
        Uri path;
        BitmapImage img;
        BitmapSource source;
        Bitmap FormsBitmap;
        byte[] pixelData;
        RImageBack last;
        Coor cords;
        int width, height, rawStride;
        public Pixel this[int i,int j]
        {
            get { return getPixel(i, j); }
        }
        System.Windows.Media.PixelFormat pf;
        public Bitmap Bitmap
        {
            get { return FormsBitmap; }
        }
        public IntPtr PixelsPtr
        {
            get
            {
                IntPtr ptr = Marshal.AllocHGlobal(pixelData.Length);
                Marshal.Copy(pixelData, 0, ptr, pixelData.Length);
                return ptr;
            }
        }
        public byte[] Pixels
        {
            get { return pixelData; }
        }
        public int Stride
        {
            get { return rawStride; }
        }
        public BitmapSource Source
        {
            get { return source; }
        }
        public Uri Path
        {
            get { return path; }
        }
        public string Name
        {
            get { return name; }
        }
        public BitmapImage Image
        {
            get { return img; }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public RImage(string name, Uri path)
        {
            this.name = name;
            this.path = path;
            
            load();
        }
        void load()
        {
            FormsBitmap = new System.Drawing.Bitmap(path.LocalPath);
            img = new BitmapImage(path);
            source = img;
            width = (int)FormsBitmap.Width;
            height = (int)FormsBitmap.Height;
            rawStride = (width * img.Format.BitsPerPixel + 7) / 8;
            pixelData = new byte[height * rawStride];
            img.CopyPixels(pixelData, rawStride, 0);
            pf = img.Format;
            last.FoImage = FormsBitmap;
            last.img = img;
        }
        public void setBitmap(BitmapImage b,Bitmap bb)
        {
            FormsBitmap = bb;
            img = b;
            width = (int)img.Width;
            height = (int)img.Height;
            rawStride = (width * img.Format.BitsPerPixel + 7) / 8;
            pixelData = new byte[height * rawStride];
            img.CopyPixels(pixelData, rawStride, 0);
            pf = img.Format;

        }
        public void setPixel(int x,int y,Pixel c)
        {
            int ix = x * 3;
            int iy = y * rawStride;
            pixelData[ix + iy] = c.r;
            pixelData[ix + iy + 1] = c.g;
            pixelData[ix + iy + 2] = c.b;
            
        }
        public Pixel getPixel(int x, int y)
        {
            Pixel c = new Pixel();
            int ix = x * 3;
            int iy = y * rawStride;
            pixelData[ix + iy] = c.r;
            pixelData[ix + iy + 1] = c.g;
            pixelData[ix + iy + 2] = c.b;
            return c;
        }
        void Sync()
        {
            ImageStream.setImage(cords, this);
        }
        public void Flush()
        {
            
            cords = ImageStream.getCoor(this);
            source = BitmapSource.Create(width, height, img.DpiX, img.DpiY, pf, null, pixelData, rawStride);
            UIAssembler.updateTab(source);
            Sync();
        }
        public void FlushBitmap(Bitmap b){
            
            cords = ImageStream.getCoor(this);
            setBitmap(Globals.Bitmap2BitmapImage(b),b);
            Sync();
        }

        public bool Compare(RImage img)
        {
            if (width != img.Width || height != img.Height)
                return false;

            SHA256Managed shaM = new SHA256Managed();
            byte[] img1 = shaM.ComputeHash(pixelData);
            byte[] img2 = shaM.ComputeHash(img.Pixels);

            byte[] hash1 = shaM.ComputeHash(img1);
            byte[] hash2 = shaM.ComputeHash(img2);
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }
            return true;
        }
        public static CompareState Compare(BitmapSource img1, BitmapSource img2)
        {
            CompareState res = CompareState.OK;
            if (img1.Width != img2.Width || img1.Height != img2.Height)
            {
                res = CompareState.SizeMisMatch;
                return res;
            }

            byte[] ipi1, ipi2;
            ipi1 = new byte[(int)(img1.Height * (int)((img1.Width * img1.Format.BitsPerPixel + 7) / 8))];
            ipi2 = new byte[(int)(img2.Height * (int)((img2.Width * img2.Format.BitsPerPixel + 7) / 8))];
            img1.CopyPixels(ipi1, (int)((img1.Width * img1.Format.BitsPerPixel + 7) / 8), 0);
            img2.CopyPixels(ipi2, (int)((img2.Width * img2.Format.BitsPerPixel + 7) / 8), 0);
            SHA256Managed shaM = new SHA256Managed();
            byte[] hash1 = shaM.ComputeHash(ipi1);
            byte[] hash2 = shaM.ComputeHash(ipi2);
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    res = CompareState.PixelMisMatch;
                    return res;
                }
            }
            return res;

        }
    }
}
