using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using ImageResizing.BlurOp;

namespace ImageResizing
{
    public class IdentityMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;

        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int Val)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = Val;
        }
    }

    class Blur
    {
        public static void init(){
            SmoothOp smooth = new SmoothOp();
            GussianFilterOp GF = new GussianFilterOp();
            SharpenOp sP = new SharpenOp();
            MedianRemovalOp mdOp = new MedianRemovalOp();
            EdgeDetectOp EDOp = new EdgeDetectOp();
        }
        public static Bitmap Matrix3x3(Bitmap b, IdentityMatrix m)
        {

            Bitmap bSrc = (Bitmap)b.Clone(); // extract a copy

            // Lock a portion of bits so that it can be changed programmatically with a better performance than (set/get pixel)
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // get the stride width ( scan line )
            int stride = bmData.Stride;
            int stride2 = stride * 2;

            // get the address of the first line
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride + 6 - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                            (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
                            (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[4 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                            (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
                            (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }

                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return b;
        }
        public static Bitmap Smooth(Bitmap b, int nWeight )
        {
            IdentityMatrix m = new IdentityMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.Factor = nWeight + 8;

            Bitmap bSrc = new Bitmap(2,2);

            bSrc = Matrix3x3(b, m);

            return bSrc;
        }
        public static Bitmap GaussianFilter(Bitmap b, int nWeight)
        {
            IdentityMatrix m = new IdentityMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            m.Factor = nWeight + 12;

            Bitmap bSrc = new Bitmap(2, 2);

            bSrc = Matrix3x3(b, m);

            return bSrc;
        }
        public static Bitmap Sharpen(Bitmap b, int nWeight )
        {
            IdentityMatrix m = new IdentityMatrix();
            m.SetAll(0);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
            m.Factor = nWeight - 8;

            Bitmap bSrc = new Bitmap(2, 2);

            bSrc = Matrix3x3(b, m);

            return bSrc;
        }
        public static Bitmap MeanRemoval(Bitmap b, int nWeight )
        {
            IdentityMatrix m = new IdentityMatrix();
            m.SetAll(-1);
            m.Pixel = nWeight;
            m.Factor = nWeight - 8;

            Bitmap bSrc = new Bitmap(2, 2);

            bSrc = Matrix3x3(b, m);

            return bSrc;
        }
        public static Bitmap EdgeDetection(Bitmap b)
        {
            IdentityMatrix m = new IdentityMatrix();

            m.TopLeft = m.TopMid = m.TopRight = -1;
            m.MidLeft = m.Pixel = m.MidRight = 0;
            m.BottomLeft = m.BottomMid = m.BottomRight = 1;

            m.Offset = 127;

            Bitmap bSrc = new Bitmap(2, 2);

            bSrc = Matrix3x3(b, m);

            return bSrc;
        }
        public static double[,] LoadPicture(Bitmap Orginal_bm)
        {
            double[,] Buffer;
            int Width = Orginal_bm.Width;
            int Height = Orginal_bm.Height;
            Buffer = new double[Width,Height];

            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    Buffer[i, j] = (Orginal_bm.GetPixel(i, j).B + Orginal_bm.GetPixel(i, j).G + Orginal_bm.GetPixel(i, j).R) / 3;

                    return Buffer;
        }
    }
}
