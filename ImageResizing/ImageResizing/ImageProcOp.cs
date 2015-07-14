using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ImageResizing
{
    // Instead of using Bitmap Class, i used a 1D array to represent the input image
    // byte[] pixelBuffer is the array that holds image's pixels
    // size of this array is the height of the picture multiplied by a Stride function that gets the length 
    // of the Row of pixels rounded to 4 byte-boundry
    // a pixel color will bre represented by an index for byte of color, 3 indices for Red && Green && Blue 
    class ImageProcOp
    {
        public static void init()
        {
            BilResize br = new BilResize();
            Greyscale gr = new Greyscale();
            Brightness bn = new Brightness();
            InvertOp IN = new InvertOp();
            GammaOp Gm = new GammaOp();
            ContrastOp CO = new ContrastOp();
            AddImages ia = new AddImages();
            Sltppr sp = new Sltppr();
            Median md = new Median();
            Tint t = new Tint();
        }
        // make the colors tinted

        public static Bitmap Tint(Bitmap Picture, float _red, float _green, float _blue)
        {
            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            Picture.UnlockBits(sourceData);
            float color = 0;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p];
                color += pixelBuffer[p + 1];
                color += pixelBuffer[p + 2];
                // New Color = OldColor + (255 - OldColor) * ColorTintedValue

                double R = pixelBuffer[p] + (255 - pixelBuffer[p]) * _red;
                double G = pixelBuffer[p + 1] + (255 - pixelBuffer[p + 1]) * _green;
                double B = pixelBuffer[p + 2] + (255 - pixelBuffer[p + 2]) * _blue;

                //if (R < 0) R = 1;
                if (R > 255) R = 255;

                //if (G < 0) G = 1;
                if (G > 255) G = 255;

                //if (B < 0) B = 1;
                if (B > 255) B = 255;

                pixelBuffer[p] = (byte)R;
                pixelBuffer[p + 1] = (byte)G;
                pixelBuffer[p + 2] = (byte)B;
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;
        }
        // Noise Generation by randomly spread the salt and pepper 

        public static Bitmap NoiseGenerate(Bitmap Picture, int Size)
        {
            Random rnd = new Random();
            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] PixelOut = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            Picture.UnlockBits(sourceData);
            float color = 0;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p];
                color += pixelBuffer[p + 1];
                color += pixelBuffer[p + 2];

                int R = pixelBuffer[p] - rnd.Next(-Size, Size + 1); ;
                int G = pixelBuffer[p + 1] - rnd.Next(-Size, Size + 1);
                int B = pixelBuffer[p + 2] - rnd.Next(-Size, Size + 1); ;

                if (R > 255) R = 255;
                if (R < 0) R = 0;

                if (G > 255) G = 255;
                if (G < 0) G = 0;

                if (B > 255) B = 255;
                if (B < 0) B = 0;

                pixelBuffer[p] = (byte)R;
                pixelBuffer[p + 1] = (byte)G;
                pixelBuffer[p + 2] = (byte)B;
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);
            return resBitmap;
        }

        public static Bitmap AddPictures(Bitmap First, Bitmap Second)
        {
            Bitmap Result = new Bitmap(First);
            System.Drawing.Color Col1 = new System.Drawing.Color();
            System.Drawing.Color Col2 = new System.Drawing.Color();
            int Width, Height;

            //if (First.Width > Second.Width) Width = First.Width;
            //else Width = Second.Width;

            //if (First.Height > Second.Height) Height = First.Height;
            //else Height = Second.Height;

            for (int x = 0; x < First.Width; x++)
            {
                for (int y = 0; y < First.Height; y++)
                {
                    Col1 = First.GetPixel(x, y);
                    Col2 = Second.GetPixel(x, y);

                    Result.SetPixel(x, y, System.Drawing.Color.FromArgb(Math.Abs(Col1.R - Col2.R),
                        Math.Abs(Col1.G - Col2.G), Math.Abs(Col1.B - Col2.B)));
                }
            }
            return Result;
        }
        // Grayscalling an image by iteratring through the array of bytes and multiply each pixel colors by a factor 

        public static Bitmap GrayScale(Bitmap Picture)
        {
            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);// get the bitmap data
            //Stride to get the pixels rounded up to 4 bytes (Color, RED, GREEN, BLUE)

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            Picture.UnlockBits(sourceData);
            float color = 0;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p] * 0.20f;
                color += pixelBuffer[p + 1] * 0.50f;
                color += pixelBuffer[p + 2] * 0.3f;

                pixelBuffer[p] = (byte)color;
                pixelBuffer[p + 1] = pixelBuffer[p];
                pixelBuffer[p + 2] = pixelBuffer[p];
                pixelBuffer[p + 3] = 255;
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;
        }
        //Increasing the photo's brigtness by increasing the colors by the user input 

        public static Bitmap Brightness(Bitmap Picture, int Bright)
        {
            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            Picture.UnlockBits(sourceData);
            float color = 0;

            if (Bright > 255) Bright = 255;
            if (Bright < -255) Bright = 255;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p];
                color += pixelBuffer[p + 1];
                color += pixelBuffer[p + 2];

                int R = pixelBuffer[p] + Bright;
                int G = pixelBuffer[p + 1] + Bright;
                int B = pixelBuffer[p + 2] + Bright;

                if (R < 0) R = 1;
                if (R > 255) R = 255;

                if (G < 0) G = 1;
                if (G > 255) G = 255;

                if (B < 0) B = 1;
                if (B > 255) B = 255;

                pixelBuffer[p] = (byte)R;
                pixelBuffer[p + 1] = (byte)G;
                pixelBuffer[p + 2] = (byte)B;
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;
        }
        // Invert the images color

        public static Bitmap Invert(Bitmap Picture)
        {
            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            Picture.UnlockBits(sourceData);
            float color = 0;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p];
                color += pixelBuffer[p + 1];
                color += pixelBuffer[p + 2];

                pixelBuffer[p] = (byte)(255 - pixelBuffer[p]);
                pixelBuffer[p + 1] = (byte)(255 - pixelBuffer[p + 1]);
                pixelBuffer[p + 2] = (byte)(255 - pixelBuffer[p + 2]);
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;

        }
        // Gamma Correction using a helper function to calculate the formula

        public static Bitmap Gamma(Bitmap Picture, double Red, double Green, double Blue)
        {
            byte[] RedGamma = GammaArrays(Red);
            byte[] GreenGamma = GammaArrays(Green);
            byte[] BlueGamma = GammaArrays(Blue);

            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            Picture.UnlockBits(sourceData);
            float color = 0;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p];
                color += pixelBuffer[p + 1];
                color += pixelBuffer[p + 2];

                pixelBuffer[p] = (byte)RedGamma[pixelBuffer[p]];
                pixelBuffer[p + 1] = (byte)GreenGamma[pixelBuffer[p + 1]];
                pixelBuffer[p + 2] = (byte)BlueGamma[pixelBuffer[p + 2]];
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;
        }

        private static byte[] GammaArrays(double Col)
        {
            byte[] Arr = new byte[255 + 1];

            for (int i = 0; i < 255 + 1; i++)
            {
                Arr[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / Col)) + 0.5));
            }
            return Arr;
        }

        public static Bitmap BilinearResize(Bitmap Picture, int Width, int Height, bool BilinearCheck)
        {
            if (BilinearCheck == true)
            {
                double YScale, XScale;
                System.Drawing.Color Up, Down, Left, Right;
                Up = new System.Drawing.Color();
                Down = new System.Drawing.Color();
                Left = new System.Drawing.Color();
                Right = new System.Drawing.Color();
                byte RED, GREEN, BLUE;
                byte First, Second;
                Bitmap Temp = (Bitmap)Picture.Clone();
                Picture = new Bitmap(Width, Height, Temp.PixelFormat);
                double XFactor = (double)Temp.Width / (double)Width;
                double YFactor = (double)Temp.Height / (double)Height;

                for (int x = 0; x < Picture.Width; x++)
                {
                    for (int y = 0; y < Picture.Height; y++)
                    {
                        int XFloor = (int)Math.Floor(x * XFactor);
                        int XCeil = XFloor + 1;
                        int YFloor = (int)Math.Floor(y * YFactor);
                        int YCeil = YFloor + 1;

                        if (XCeil >= Picture.Width) XCeil = XFloor;
                        if (YCeil >= Picture.Height) YCeil = YFloor;

                        XScale = x * XFactor - XFloor;
                        YScale = y * YFactor - YFloor;

                        double XSubs1 = 1.0 - XScale;
                        double YSub1 = 1.0 - YScale;

                        Up = Temp.GetPixel(XCeil, YCeil);
                        Down = Temp.GetPixel(XFloor, YFloor);
                        Left = Temp.GetPixel(XFloor, YCeil);
                        Right = Temp.GetPixel(XCeil, YFloor);

                        // RED
                        First = (byte)(XSubs1 * Down.R + XScale * Right.R);
                        Second = (byte)(XSubs1 * Left.R + XScale * Up.R);
                        RED = (byte)(YSub1 * (double)(First) + YScale * (double)(Second));

                        // BLUE
                        First = (byte)(XSubs1 * Down.B + XScale * Right.B);
                        Second = (byte)(XSubs1 * Left.B + XScale * Up.B);
                        BLUE = (byte)(YSub1 * (double)(First) + YScale * (double)(Second));

                        //GREEN
                        First = (byte)(XSubs1 * Down.G + XScale * Right.G);
                        Second = (byte)(XSubs1 * Left.G + XScale * Up.G);
                        GREEN = (byte)(YSub1 * (double)(First) + YScale * (double)(Second));

                        Picture.SetPixel(x, y, System.Drawing.Color.FromArgb(255, RED, GREEN, BLUE));
                    }
                }
                return Picture;
            }
            else
            {
                return Picture;
            }

        }
        // Contrasting photo

        public static Bitmap Contrast(Bitmap Picture, double ContrastValue)
        {
            if (ContrastValue > 100) ContrastValue = 100;
            if (ContrastValue < -100) ContrastValue = -100;

            ContrastValue = (ContrastValue + 100.0) / 100.0;
            ContrastValue = ContrastValue * ContrastValue;

            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            Picture.UnlockBits(sourceData);
            float color = 0;

            for (int p = 0; p < pixelBuffer.Length; p += 4)
            {
                color = pixelBuffer[p];
                color += pixelBuffer[p + 1];
                color += pixelBuffer[p + 2];
                // New Color = (((((OldColor / 255.0) - 0.5) * ContrastValue) + 0.5) + 255.0

                double pixelRed = pixelBuffer[p] / 255.0;
                pixelRed -= 0.5;
                pixelRed *= ContrastValue;
                pixelRed += 0.5;
                pixelRed *= 255;

                if (pixelRed > 255) pixelRed = 255;
                if (pixelRed < 0) pixelRed = 0;

                double pixelGreen = pixelBuffer[p + 1] / 255.0;
                pixelGreen -= 0.5;
                pixelGreen *= ContrastValue;
                pixelGreen += 0.5;
                pixelGreen *= 255;

                if (pixelGreen > 255) pixelGreen = 255;
                if (pixelGreen < 0) pixelGreen = 0;

                double pixelBlue = pixelBuffer[p + 2] / 255.0;
                pixelBlue -= 0.5;
                pixelBlue *= ContrastValue;
                pixelBlue += 0.5;
                pixelBlue *= 255;

                if (pixelBlue > 255) pixelBlue = 255;
                if (pixelBlue < 0) pixelBlue = 0;

                pixelBuffer[p] = (byte)pixelRed;
                pixelBuffer[p + 1] = (byte)pixelGreen;
                pixelBuffer[p + 2] = (byte)pixelBlue;
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;
        }
        // Median Filter tp remove the noise, with an option yo user to gray scale image 
        // the algorithm is worrking as follow:
        // get the matrix size, the matrix size represents a region in the photo
        // for example 3 x 3 .. represents 3 x 3 matrix that i am working to it to set the color of the median pixel

        public static Bitmap Median(Bitmap Picture, int matrixSize, bool grayScale = false)
        {
            BitmapData sourceData = Picture.LockBits(new Rectangle(0, 0, Picture.Width, Picture.Height), ImageLockMode.ReadOnly,
               PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] PixelOut = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            Picture.UnlockBits(sourceData);

            if (grayScale == true)
            {
                float color = 0;

                for (int p = 0; p < pixelBuffer.Length; p += 4)
                {
                    color = pixelBuffer[p] * 0.20f;
                    color += pixelBuffer[p + 1] * 0.50f;
                    color += pixelBuffer[p + 2] * 0.3f;

                    pixelBuffer[p] = (byte)color;
                    pixelBuffer[p + 1] = pixelBuffer[p];
                    pixelBuffer[p + 2] = pixelBuffer[p];
                    pixelBuffer[p + 3] = 255;
                }
            }

            byte[] midPixel;
            int filterMedian = (matrixSize - 1) / 2;
            List<int> adjcent = new List<int>();

            for (int newY = filterMedian; newY < Picture.Height - filterMedian; newY++)
            {
                for (int newX = filterMedian; newX < Picture.Width - filterMedian; newX++)
                {
                    int newByte = newY * sourceData.Stride + newX * 4;
                    adjcent.Clear();

                    for (int filterY = -filterMedian; filterY <= filterMedian; filterY++)
                    {
                        for (int filterX = -filterMedian; filterX <= filterMedian; filterX++)
                        {
                            int calculateAll = newByte + (filterX * 4) + (filterY * sourceData.Stride);
                            adjcent.Add(BitConverter.ToInt32(pixelBuffer, calculateAll));
                        }
                    }
                    adjcent.Sort();
                    midPixel = BitConverter.GetBytes(adjcent[filterMedian]);

                    PixelOut[newByte] = midPixel[0];
                    PixelOut[newByte + 1] = midPixel[1];
                    PixelOut[newByte + 2] = midPixel[2];
                    PixelOut[newByte + 3] = midPixel[3];
                }
            }

            Bitmap resBitmap = new Bitmap(Picture.Width, Picture.Height);
            BitmapData resData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width, resBitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(PixelOut, 0, resData.Scan0, pixelBuffer.Length);
            resBitmap.UnlockBits(resData);

            return resBitmap;
        }

    }
}
