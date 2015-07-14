using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace ImageResizing
{
    public class Pair : IComparable<Pair>
    {
        int first, second;

        public Pair(int first, int second)
        {
            this.first = first;
            this.second = second;
        }

        public int First
        {
            get { return first; }
            set { first = value; }
        }

        public int Second
        {
            get { return second; }
            set { second = value; }
        }
        public int CompareTo(Pair n)
        {
            if (first < n.first)
                return -1;
            else if (first > n.first)
                return 1;
            else
                return 0;
        }
    }

    public struct MyColor
    {
        public MyColor(byte r, byte g, byte b)
        {
            red = r;
            green = g;
            blue = b;
        }
        public byte red, green, blue;
    }
    public struct Color
    {
        public Color(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }
        public double R, G, B;
    }
    public struct pixel
    {
        public Dir vertical_Dir, horizontal_Dir;
        public int vertical_Energy, horizontal_Energy;
    }
    public enum Dir
    {
        N, NW, NE, Null
    }
    class ContentAwareResize
    {
        public static void init()
        {
            CAResizing CAR = new CAResizing();

        }
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static Color[,] OpenImage(Bitmap img)
        {
            Bitmap original_bm = img;
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            Color[,] Buffer = new Color[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].R = Buffer[y, x].G = Buffer[y, x].B = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].R = p[0];
                            Buffer[y, x].G = p[1];
                            Buffer[y, x].B = p[2];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(MyColor[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(MyColor[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Calculate energy between the given two pixels
        /// </summary>
        /// <param name="Pixel1">First pixel color</param>
        /// <param name="Pixel2">Second pixel color</param>
        /// <returns>Energy between the 2 pixels</returns>
        public static int CalculatePixelsEnergy(MyColor Pixel1, MyColor Pixel2)
        {
            int Energy = Math.Abs(Pixel1.red - Pixel2.red) + Math.Abs(Pixel1.green - Pixel2.green) + Math.Abs(Pixel1.blue - Pixel2.blue);
            return Energy;
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static Bitmap DisplayImage(Color[,] ImageMatrix)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = (byte)ImageMatrix[i, j].R;
                        p[1] = (byte)ImageMatrix[i, j].G;
                        p[2] = (byte)ImageMatrix[i, j].B;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            return ImageBMP;
        }

        /// <summary>
        /// Normal resize of an image
        /// </summary>
        /// <param name="ImageMatrix">2D array of image values</param>
        /// <param name="NewWidth">desired width</param>
        /// <param name="NewHeight">desired height</param>
        /// <returns>Resized image</returns>

        public static MyColor[,] NormalResize(MyColor[,] ImageMatrix, int NewWidth, int NewHeight)
        {
            int i = 0, j = 0;
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            double WidthRatio = (double)(Width) / (double)(NewWidth);
            double HeightRatio = (double)(Height) / (double)(NewHeight);

            int OldWidth = Width;
            int OldHeight = Height;

            MyColor P1, P2, P3, P4;

            MyColor Y1, Y2, X = new MyColor();

            MyColor[,] Data = new MyColor[NewHeight, NewWidth];

            Width = NewWidth;
            Height = NewHeight;

            int floor_x, ceil_x;
            int floor_y, ceil_y;

            double x, y;
            double fraction_x, one_minus_x;
            double fraction_y, one_minus_y;

            for (j = 0; j < NewHeight; j++)
                for (i = 0; i < NewWidth; i++)
                {
                    x = (double)(i) * WidthRatio;
                    y = (double)(j) * HeightRatio;

                    floor_x = (int)(x);
                    ceil_x = floor_x + 1;
                    if (ceil_x >= OldWidth) ceil_x = floor_x;

                    floor_y = (int)(y);
                    ceil_y = floor_y + 1;
                    if (ceil_y >= OldHeight) ceil_y = floor_y;

                    fraction_x = x - floor_x;
                    one_minus_x = 1.0 - fraction_x;

                    fraction_y = y - floor_y;
                    one_minus_y = 1.0 - fraction_y;

                    P1 = ImageMatrix[floor_y, floor_x];
                    P2 = ImageMatrix[ceil_y, floor_x];
                    P3 = ImageMatrix[floor_y, ceil_x];
                    P4 = ImageMatrix[ceil_y, ceil_x];

                    Y1.red = (byte)(one_minus_y * P1.red + fraction_y * P2.red);
                    Y1.green = (byte)(one_minus_y * P1.green + fraction_y * P2.green);
                    Y1.blue = (byte)(one_minus_y * P1.blue + fraction_y * P2.blue);

                    Y2.red = (byte)(one_minus_y * P3.red + fraction_y * P4.red);
                    Y2.green = (byte)(one_minus_y * P3.green + fraction_y * P4.green);
                    Y2.blue = (byte)(one_minus_y * P3.blue + fraction_y * P4.blue);

                    X.red = (byte)(one_minus_x * Y1.red + fraction_x * Y2.red);
                    X.green = (byte)(one_minus_x * Y1.green + fraction_x * Y2.green);
                    X.blue = (byte)(one_minus_x * Y1.blue + fraction_x * Y2.blue);

                    Data[j, i] = X;
                }

            return Data;
        }

        private static int brightness(Color pixel)
        {
            Color newPixle;
            //(0.2126*R) + (0.7152*G) + (0.0722*B)
            newPixle.R = 0.2126 * pixel.R;
            newPixle.G = 0.7152 * pixel.G;
            newPixle.B = 0.0722 * pixel.B;
            return (int)(newPixle.R + newPixle.G + newPixle.B);
        }
        private static int pixel_Energy(Color A, Color B, Color C, Color D, Color E, Color F, Color G, Color H, Color I)
        {
            // using Sobel Operator
            int a = brightness(A);
            int b = brightness(B);
            int c = brightness(C);
            int d = brightness(D);
            int e = brightness(E);
            int f = brightness(F);
            int g = brightness(G);
            int h = brightness(H);
            int i = brightness(I);

            // Ex energy in x dimention , Ey energy in y dimention
            int Ex = a + (2 * d) + g - c - (2 * f) - i;
            int Ey = a + (2 * b) + c - g - (2 * h) - i;
            return (int)Math.Sqrt((Ex * Ex) + (Ey * Ey));
        }
        private static Color getColor(int i, int j, int width, int height, Color[,] image)
        {
            if (0 <= i && i < height && 0 <= j && j < width)
                return image[i, j];
            return new Color(255, 255, 255);
        }
        private static int getEnergy(int i, int j, int width, int height, int[,] energy)
        {
            if (0 <= i && i < height && 0 <= j && j < width)
                return energy[i, j];
            return 255;
        }
        private static int forward_energy(bool vertical, int i, int j, int p1_i, int p1_j, int p2_i, int p2_j, int Width, int Height, Color[,] image)
        {
            // calculte forward energy for pixel at i&j , after removing 2 pixels (p1_i,p1_j) & (p2_i,p2_j)
            Color[] window = new Color[9];
            int count = 0;
            if (vertical)
            {
                for (int k = i - 1; k <= i + 1; ++k)
                {
                    count = 0;
                    for (int u = j - 1; u <= j + 1; ++u)
                    {
                        if ((k == p1_i && u == p1_j) || (k == p2_i && u == p2_j))
                            count = 1;
                        window[(k - (i - 1)) * 3 + (u - (j - 1))] = getColor(k, u + count, Width, Height, image);
                    }
                }
            }
            else
            {
                for (int u = j - 1; u <= j + 1; ++u)
                {
                    count = 0;
                    for (int k = i - 1; k <= i + 1; ++k)
                    {
                        if ((k == p1_i && u == p1_j) || (k == p2_i && u == p2_j))
                            count = 1;
                        window[(k - (i - 1)) * 3 + (u - (j - 1))] = getColor(k + count, u, Width, Height, image);
                    }
                }
            }
            return pixel_Energy(window[0], window[1], window[2], window[3], window[4], window[5], window[6], window[7], window[8]);
        }
        private static void Ver_C(int start, int first_pixel_index, Color[,] image, int[,] energy, int[,] C, int[,] map, int Width, int Height)
        {
            const int INF = 2147483647;
            int[,] case_1 = { { -1, -1 }, { -1, 1 }, { 0, -1 }, { 0, 1 } };

            int begin, end;

            for (int i = start; i < Height; ++i)
            {
                // start = 1 then calculate cost as a Pyramid shape
                if (start == 1)
                {
                    int length = (2 * i) + 1;
                    begin = Math.Max(first_pixel_index - (length / 2), 0);
                    end = Math.Min(first_pixel_index + (length / 2), Width - 1);
                }
                else
                {
                    begin = 0;
                    end = Width - 1;
                }

                for (int j = begin; j <= end; ++j)
                {
                    int[] cost = { INF, INF, INF };
                    // iterate 3 cases
                    for (int k = -1; k <= 1; ++k)
                    {
                        // check boundary
                        if ((k == -1 && j == 0) || (k == 1 && j == Width - 1))
                            continue;
                        int before_erase = 0, after_erase = 0;
                        for (int m = 0; m < 2; ++m)
                        {
                            before_erase += getEnergy(i + case_1[m, 0], j + case_1[m, 1] + k, Width, Height, energy)
                                + getEnergy(i + case_1[m + 2, 0], j + case_1[m + 2, 1], Width, Height, energy);

                            after_erase += forward_energy(true, i + case_1[m, 0], j + case_1[m, 1] + k, i, j, i - 1, j + k, Width, Height, image)
                                + forward_energy(true, i + case_1[m + 2, 0], j + case_1[m + 2, 1], i, j, i - 1, j + k, Width, Height, image);
                        }
                        cost[k + 1] = Math.Abs(after_erase - before_erase) + C[i - 1, j + k];
                    }

                    // get the minimum between the three cases 
                    if (cost[0] <= cost[1] && cost[0] <= cost[2])       // case NW
                    {
                        C[i, j] = cost[0];
                        map[i, j] = -1;
                    }
                    else if (cost[1] <= cost[0] && cost[1] <= cost[2])  // case N
                    {
                        C[i, j] = cost[1];
                        map[i, j] = 0;
                    }
                    else                                               // case NE
                    {
                        C[i, j] = cost[2];
                        map[i, j] = 1;
                    }
                }
            }
        }
        private static void Hor_C(int start, int first_pixel_index, Color[,] image, int[,] energy, int[,] C, int[,] map, int Width, int Height)
        {
            // the same idea of the vertical but the different in the indexes

            const int INF = 2147483647;
            int[,] case_1 = { { -1, -1 }, { -1, 1 }, { 0, -1 }, { 0, 1 } };
            int begin, end;

            for (int j = start; j < Width; ++j)
            {
                if (start == 1)
                {
                    int length = (2 * j) + 1;
                    begin = Math.Max(first_pixel_index - (length / 2), 0);
                    end = Math.Min(first_pixel_index + (length / 2), Height - 1);
                }
                else
                {
                    begin = 0;
                    end = Height - 1;
                }
                for (int i = end; i >= begin; --i)
                {
                    map[i, 0] = 0;
                    int[] cost = { INF, INF, INF };
                    for (int k = -1; k <= 1; ++k)
                    {
                        if ((k == -1 && i == Height - 1) || (k == 1 && i == 0))
                            continue;
                        k *= -1;
                        int before_erase = 0, after_erase = 0;
                        for (int m = 0; m < 2; ++m)
                        {
                            before_erase += getEnergy(i + case_1[m, 1] + k, j + case_1[m, 0], Width, Height, energy)
                                + getEnergy(i + case_1[m + 2, 1], j + case_1[m + 2, 0], Width, Height, energy);

                            after_erase += forward_energy(false, i + case_1[m, 1] + k, j + case_1[m, 0], i, j, i + k, j - 1, Width, Height, image)
                                + forward_energy(false, i + case_1[m + 2, 1], j + case_1[m + 2, 0], i, j, i + k, j - 1, Width, Height, image);
                        }
                        cost[(k * (-1)) + 1] = Math.Abs(after_erase - before_erase) + C[i + k, j - 1];
                        k *= -1;
                    }
                    if (cost[0] <= cost[1] && cost[0] <= cost[2])       // case SW
                    {
                        C[i, j] = cost[0];
                        map[i, j] = 1;
                    }
                    else if (cost[1] <= cost[0] && cost[1] <= cost[2])  // case W
                    {
                        C[i, j] = cost[1];
                        map[i, j] = 0;
                    }
                    else                                               // case NW
                    {
                        C[i, j] = cost[2];
                        map[i, j] = -1;
                    }
                }
            }
        }
        private static void firstTime_Ver_C(Color[,] image, int[,] energy, int[,] C, int[,] map, int Width, int Height)
        {
            // calculate cost of every path using DP
            const int INF = 2147483647;
            int[,] case_1 = { { -1, -1 }, { -1, 1 }, { 0, -1 }, { 0, 1 } };


            for (int i = 1; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    map[0, j] = 0;
                    int[] cost = { INF, INF, INF };
                    for (int k = -1; k <= 1; ++k)
                    {
                        if ((k == -1 && j == 0) || (k == 1 && j == Width - 1))
                            continue;
                        int before_erase = 0, after_erase = 0;
                        for (int m = 0; m < 2; ++m)
                        {
                            before_erase += getEnergy(i + case_1[m, 0], j + case_1[m, 1] + k, Width, Height, energy)
                                + getEnergy(i + case_1[m + 2, 0], j + case_1[m + 2, 1], Width, Height, energy);

                            after_erase += forward_energy(true, i + case_1[m, 0], j + case_1[m, 1] + k, i, j, i - 1, j + k, Width, Height, image)
                                + forward_energy(true, i + case_1[m + 2, 0], j + case_1[m + 2, 1], i, j, i - 1, j + k, Width, Height, image);
                        }
                        cost[k + 1] = Math.Abs(after_erase - before_erase) + C[i - 1, j + k];
                    }
                    if (cost[0] <= cost[1] && cost[0] <= cost[2])       // case NW
                    {
                        C[i, j] = cost[0];
                        map[i, j] = -1;
                    }
                    else if (cost[1] <= cost[0] && cost[1] <= cost[2])  // case N
                    {
                        C[i, j] = cost[1];
                        map[i, j] = 0;
                    }
                    else                                               // case NE
                    {
                        C[i, j] = cost[2];
                        map[i, j] = 1;
                    }
                }
            }
        }
        private static void firstTime_Hor_C(Color[,] image, int[,] energy, int[,] C, int[,] map, int Width, int Height)
        {
            // calculate cost of every path using DP
            const int INF = 2147483647;
            int[,] case_1 = { { -1, -1 }, { -1, 1 }, { 0, -1 }, { 0, 1 } };


            for (int j = 1; j < Width; ++j)
            {
                for (int i = Height - 1; i >= 0; --i)
                {
                    map[i, 0] = 0;
                    int[] cost = { INF, INF, INF };
                    for (int k = -1; k <= 1; ++k)
                    {
                        if ((k == -1 && i == Height - 1) || (k == 1 && i == 0))
                            continue;
                        k *= -1;
                        int before_erase = 0, after_erase = 0;
                        for (int m = 0; m < 2; ++m)
                        {
                            before_erase += getEnergy(i + case_1[m, 1] + k, j + case_1[m, 0], Width, Height, energy)
                                + getEnergy(i + case_1[m + 2, 1], j + case_1[m + 2, 0], Width, Height, energy);

                            after_erase += forward_energy(false, i + case_1[m, 1] + k, j + case_1[m, 0], i, j, i + k, j - 1, Width, Height, image)
                                + forward_energy(false, i + case_1[m + 2, 1], j + case_1[m + 2, 0], i, j, i + k, j - 1, Width, Height, image);
                        }
                        cost[(k * (-1)) + 1] = Math.Abs(after_erase - before_erase) + C[i + k, j - 1];
                        k *= -1;
                    }
                    if (cost[0] <= cost[1] && cost[0] <= cost[2])       // case SW
                    {
                        C[i, j] = cost[0];
                        map[i, j] = 1;
                    }
                    else if (cost[1] <= cost[0] && cost[1] <= cost[2])  // case W
                    {
                        C[i, j] = cost[1];
                        map[i, j] = 0;
                    }
                    else                                               // case NW
                    {
                        C[i, j] = cost[2];
                        map[i, j] = -1;
                    }
                }
            }
        }

        private static Pair delete_ver_seam_carving(int min_ver, int[,] map_ver, int[,] energy, Color[,] image, int[,] C, ref int Width, int Height)
        {
            Color black = new Color(0, 0, 0);
            int min_j = Width - 1;               // min_j: j index for the most left pixel in seam carving
            int back_up = min_ver;          // back_up for min_ver variable
            int[] J_indexes = { -2, -1, 0, 1 };     // coordinate for updating energy


            for (int i = Height - 1; i >= 0; --i)       // shifting to erase the seam carving
            {
                for (int j = min_ver + 1; j < Width; ++j)   // shifting all right pixels to the left 
                {
                    energy[i, j - 1] = energy[i, j];
                    image[i, j - 1] = image[i, j];
                    C[i, j - 1] = C[i, j];
                }
                energy[i, Width - 1] = 255;
                image[i, Width - 1] = black;
                C[i, Width - 1] = 2147483647;
                min_j = Math.Min(min_ver, min_j);
                min_ver = min_ver + map_ver[i, min_ver];    // iteration step to get parent of pixel
            }
            min_ver = back_up;
            for (int i = Height - 1; i >= 0; --i)       // recalculate the energy around the seam carving
            {
                for (int k = 0; k < 4; k++)
                {
                    int j = min_ver + J_indexes[k];
                    if (j >= 0 && j < Width)
                    {
                        energy[i, j] = pixel_Energy(getColor(i - 1, j - 1, Width, Height, image),
                             getColor(i - 1, j, Width, Height, image), getColor(i - 1, j + 1, Width, Height, image)
                             , getColor(i, j - 1, Width, Height, image), getColor(i, j, Width, Height, image)
                             , getColor(i, j + 1, Width, Height, image), getColor(i + 1, j - 1, Width, Height, image)
                             , getColor(i + 1, j, Width, Height, image), getColor(i + 1, j + 1, Width, Height, image));
                    }
                }
                min_ver = min_ver + map_ver[i, min_ver];
            }

            --Width;

            min_j = Math.Max(min_j, 1);

            return new Pair(min_j, min_ver);
        }
        private static Pair delete_hor_seam_carving(int min_hor, int[,] map_hor, int[,] energy, Color[,] image, int[,] C, int Width, ref int Height)
        {
            // same idea of delete_ver_seam_carving function
            Color black = new Color(0, 0, 0);
            int min_i = Height - 1;
            int[] I_indexes = { -2, -1, 0, 1 };
            int back_up = min_hor;

            for (int j = Width - 1; j >= 0; --j)        // shifting to erase the seam carving
            {
                for (int i = min_hor + 1; i < Height; ++i)
                {
                    energy[i - 1, j] = energy[i, j];
                    image[i - 1, j] = image[i, j];
                    C[i - 1, j] = C[i, j];
                }
                energy[Height - 1, j] = 255;
                image[Height - 1, j] = black;
                C[Height - 1, j] = 2147483647;
                min_i = Math.Min(min_hor, min_i);
                min_hor = min_hor + map_hor[min_hor, j];
            }
            min_hor = back_up;
            for (int j = Width - 1; j >= 0; --j)        // recalculate the energy
            {
                for (int k = 0; k < 4; k++)
                {
                    int i = min_hor + I_indexes[k];
                    if (i >= 0 && i < Height)
                    {
                        energy[i, j] = pixel_Energy(getColor(i - 1, j - 1, Width, Height, image),
                            getColor(i - 1, j, Width, Height, image), getColor(i - 1, j + 1, Width, Height, image)
                            , getColor(i, j - 1, Width, Height, image), getColor(i, j, Width, Height, image)
                            , getColor(i, j + 1, Width, Height, image), getColor(i + 1, j - 1, Width, Height, image)
                            , getColor(i + 1, j, Width, Height, image), getColor(i + 1, j + 1, Width, Height, image));
                    }
                }

                min_hor = min_hor + map_hor[min_hor, j];
            }
            --Height;

            min_i = Math.Max(min_i, 1);

            return new Pair(min_i, min_hor);
        }
        private static int beginning_of_min_ver_seam_carving(int[,] C_ver, int Width, int Height)
        {
            int min_ver = 0;
            for (int j = 1; j < Width; ++j)
            {
                if (C_ver[Height - 1, j] < C_ver[Height - 1, min_ver])
                    min_ver = j;
            }
            return min_ver;
        }
        private static int beginning_of_min_hor_seam_carving(int[,] C_hor, int Width, int Height)
        {
            int min_hor = 0;
            for (int i = 1; i < Height; ++i)
            {
                if (C_hor[i, Width - 1] < C_hor[min_hor, Width - 1])
                    min_hor = i;
            }
            return min_hor;
        }
        private static void energy_init(int[,] energy, Color[,] image, int Width, int Height)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    energy[i, j] = pixel_Energy(getColor(i - 1, j - 1, Width, Height, image),
                               getColor(i - 1, j, Width, Height, image), getColor(i - 1, j + 1, Width, Height, image)
                               , getColor(i, j - 1, Width, Height, image), getColor(i, j, Width, Height, image)
                               , getColor(i, j + 1, Width, Height, image), getColor(i + 1, j - 1, Width, Height, image)
                               , getColor(i + 1, j, Width, Height, image), getColor(i + 1, j + 1, Width, Height, image));
                }
            }
        }
        public static Color[,] seamCarving_resize(Color[,] Image, int height_resize, int width_resize, BackgroundWorker bw)
        {
            int OV = height_resize, OH = width_resize;

            if (height_resize == 0 && width_resize == 0)
                return Image;
            Color[,] image = Copy(Image);

            int Height = image.GetLength(0);
            int Width = image.GetLength(1);

            if (height_resize > Height || width_resize > Width)
                return image;
            // Color[,] image = Convert_MyColor_Color(Current_Image);

            int[,] energy = new int[Height, Width];
            energy_init(energy, image, Width, Height);


            int[,] C_ver = new int[Height, Width];
            int[,] map_ver = new int[Height, Width];
            int[,] C_hor = new int[Height, Width];
            int[,] map_hor = new int[Height, Width];

            Pair start = new Pair(0, 0);
            bool optimize_horizontal = false;
            bool first_Time = true;
            while (height_resize != 0 || width_resize != 0)
            {
                if (height_resize > 0 && width_resize > 0)
                {
                    if (first_Time)
                    {
                        first_Time = false;
                        firstTime_Ver_C(image, energy, C_ver, map_ver, Width, Height);
                        firstTime_Hor_C(image, energy, C_hor, map_hor, Width, Height);
                    }
                    else
                    {
                        if (optimize_horizontal)
                        {
                            Ver_C(1, start.Second, image, energy, C_ver, map_ver, Width, Height);
                            Hor_C(start.First, 0, image, energy, C_hor, map_hor, Width, Height);
                        }
                        else
                        {
                            Ver_C(start.First, 0, image, energy, C_ver, map_ver, Width, Height);
                            Hor_C(1, start.Second, image, energy, C_hor, map_hor, Width, Height);
                        }
                    }
                    int min_ver = beginning_of_min_ver_seam_carving(C_ver, Width, Height);
                    int min_hor = beginning_of_min_hor_seam_carving(C_hor, Width, Height);

                    if (C_ver[Height - 1, min_ver] <= C_hor[min_hor, Width - 1])
                    {
                        start = delete_ver_seam_carving(min_ver, map_ver, energy, image, C_ver, ref Width, Height);
                        optimize_horizontal = true;
                        --width_resize;
                    }
                    else
                    {
                        start = delete_hor_seam_carving(min_hor, map_hor, energy, image, C_hor, Width, ref Height);
                        optimize_horizontal = false;
                        --height_resize;
                    }
                }
                else if (width_resize > 0)
                {
                    if (first_Time)
                    {
                        first_Time = false;
                        firstTime_Ver_C(image, energy, C_ver, map_ver, Width, Height);
                    }
                    else
                    {
                        Ver_C(1, start.Second, image, energy, C_ver, map_ver, Width, Height);
                    }
                    int min_ver = beginning_of_min_ver_seam_carving(C_ver, Width, Height);
                    start = delete_ver_seam_carving(min_ver, map_ver, energy, image, C_ver, ref Width, Height);
                    --width_resize;
                }
                else if (height_resize > 0)
                {
                    if (first_Time)
                    {
                        first_Time = false;
                        firstTime_Hor_C(image, energy, C_hor, map_hor, Width, Height);
                    }
                    else
                    {
                        Hor_C(1, start.Second, image, energy, C_hor, map_hor, Width, Height);
                    }
                    int min_hor = beginning_of_min_hor_seam_carving(C_hor, Width, Height);
                    start = delete_hor_seam_carving(min_hor, map_hor, energy, image, C_hor, Width, ref Height);
                    --height_resize;
                }
                double pros = 0;
                int total = OV + OH;
                int cpro = height_resize + width_resize;
                pros = total - cpro;
                pros /= total;
                pros *= 100;
                bw.ReportProgress((int)pros);
            }

            return Copy(image, Width, Height);
        }

        private static Color average_pixel(Color pixel1, Color pixle2)
        {
            return new Color((pixel1.R + pixle2.R) / 2, (pixel1.G + pixle2.G) / 2, (pixel1.B + pixle2.B) / 2);
        }
        private static void add_ver_seam_carving(int start_j, int[,] map_ver, Color[,] image, Color[,] new_Image, int Width, int Height)
        {
            for (int i = Height - 1; i >= 0; --i)
            {
                if (start_j == Width - 1)
                    new_Image[i, start_j + 1] = image[i, start_j];
                else
                    new_Image[i, start_j + 1] = average_pixel(image[i, start_j], image[i, start_j + 1]);
                start_j = start_j + map_ver[i, start_j];
            }
        }
        private static void add_hor_seam_carving(int start_i, int[,] map_hor, Color[,] image, Color[,] new_Image, int Width, int Height)
        {
            for (int j = Width - 1; j >= 0; --j)
            {
                if (start_i == Height - 1)
                    new_Image[start_i + 1, j] = image[start_i, j];
                else
                    new_Image[start_i + 1, j] = average_pixel(image[start_i, j], image[start_i + 1, j]);
                start_i = start_i + map_hor[start_i, j];
            }
        }

        private static List<int> get_ver_path(int start_j, int[,] map_ver, int Height)
        {
            List<int> J_index = new List<int>();
            for (int i = Height - 1; i >= 0; --i)
            {
                J_index.Add(start_j);
                start_j = start_j + map_ver[i, start_j];
            }
            return J_index;
        }
        private static void insert_pixels(List<List<int>> duplicated_pixels, Color[,] image, Color[,] new_Image, int width_diff, int Width, int Height)
        {
            int j_index, count = 0, value = 0, last_count = 0;
            for (int i = 0; i < Height; ++i)
            {
                count = last_count = 0;
                for (int j = 0; j < width_diff; ++j)
                {
                    j_index = duplicated_pixels[i][j];
                    if (j == 0)
                        value = j_index;
                    else
                    {
                        if (value == j_index)
                            ++count;
                        else
                        {
                            last_count += count;
                            value = j_index;
                            count = 0;
                        }
                    }
                    if (j_index == Width - 1)
                        new_Image[i, j_index + 1 + count + last_count] = /*new Color(0,0,0);//*/image[i, j_index];
                    else
                    {
                        if (count != 0)
                            new_Image[i, j_index + 1 + count + last_count] = /*new Color(0,0,0);//*/average_pixel(new_Image[i, j_index + 1 + count - 1 + last_count], image[i, j_index + 1]);
                        else
                            new_Image[i, j_index + 1 + count + last_count] = /*new Color(0, 0, 0); //*/ average_pixel(image[i, j_index], image[i, j_index + 1]);
                    }
                }
            }

            int new_Width = width_diff + Width;






            int new_j = 0, new_i = 0;
            bool check;
            for (int i = 0; i < Height; ++i)
            {

                for (int j = 0; j < Width; ++j)
                {
                    check = new_Image[new_i, new_j].R == -1.0 && new_Image[new_i, new_j].G == -1.0 && new_Image[new_i, new_j].B == -1.0;
                    while (!check)
                    {
                        // new_i += (++new_j / new_Width);
                        new_j++;
                        check = new_Image[new_i, new_j].R == -1 && new_Image[new_i, new_j].G == -1 && new_Image[new_i, new_j].B == -1;
                    }
                    new_Image[new_i, new_j] = image[i, j];
                    //new_i += (++new_j / new_Width);
                    new_j++;

                }
                ++new_i;
                new_j = 0;
            }
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < new_Width; ++j)
                {
                    if (new_Image[i, j].R == -1.0 && new_Image[i, j].G == -1.0 && new_Image[i, j].B == -1.0)
                        throw new OverflowException();
                }

            }
        }
        public static MyColor[,] Image_Enlarging(MyColor[,] Image, int height_resize, int width_resize)
        {
            if (height_resize == 0 && width_resize == 0)
                return Image;


            int Height = Image.GetLength(0);
            int Width = Image.GetLength(1);
            //   MyColor [,] new_Image = new MyColor [Height+height_resize,Width+width_resize];
            bool[,] empty = new bool[Height + height_resize, Width + width_resize];
            Color[,] new_Image = new Color[Height + height_resize, Width + width_resize];
            Color[,] image = Convert_MyColor_Color(Image);

            int[,] energy = new int[Height, Width];
            energy_init(energy, image, Width, Height);

            for (int i = 0; i < Height + height_resize; ++i)
            {
                for (int j = 0; j < Width + width_resize; ++j)
                {
                    new_Image[i, j].R = -1;
                    new_Image[i, j].G = -1;
                    new_Image[i, j].B = -1;
                }
            }

            int[,] C_ver = new int[Height, Width];
            int[,] map_ver = new int[Height, Width];
            int[,] C_hor = new int[Height, Width];
            int[,] map_hor = new int[Height, Width];
            if (width_resize > 0)
            {
                //Ver_C(1, image, energy, C_ver, map_ver, Width, Height);
                List<Pair> ver_seam = new List<Pair>();

                for (int j = 0; j < Width; ++j)
                {
                    ver_seam.Add(new Pair(C_ver[Height - 1, j], j));
                }

                ver_seam.Sort();

                List<List<int>> duplicated_pixels = new List<List<int>>();
                for (int i = Height - 1; i >= 0; --i)
                {
                    duplicated_pixels.Insert(0, new List<int>());
                }


                List<int> path;
                for (int k = 0; k < width_resize; ++k)
                {
                    path = get_ver_path(ver_seam[k].Second, map_ver, Height);
                    for (int i = Height - 1; i >= 0; --i)
                    {
                        duplicated_pixels[i].Add(path[i]);
                    }
                }
                for (int i = Height - 1; i >= 0; --i)
                {
                    duplicated_pixels[i].Sort();
                }
                insert_pixels(duplicated_pixels, image, new_Image, width_resize, Width, Height);
            }
            if (height_resize > 0)
            {
                //Hor_C(1, image, energy, C_hor, map_hor, Width, Height);
            }




            return Convert_Color_MyColor(new_Image);
        }


        private static MyColor[,] Convert_Color_MyColor(Color[,] Image)
        {
            int Height = Image.GetLength(0);
            int Width = Image.GetLength(1);

            MyColor[,] Copy_Image = new MyColor[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Copy_Image[i, j].red = (byte)Image[i, j].R;
                    Copy_Image[i, j].blue = (byte)Image[i, j].B;
                    Copy_Image[i, j].green = (byte)Image[i, j].G;
                }
            }
            return Copy_Image;
        }
        private static Color[,] Copy(Color[,] Image)
        {
            int Height = Image.GetLength(0);
            int Width = Image.GetLength(1);

            Color[,] Copy_Image = new Color[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Copy_Image[i, j].R = Image[i, j].R;
                    Copy_Image[i, j].B = Image[i, j].B;
                    Copy_Image[i, j].G = Image[i, j].G;
                }
            }
            return Copy_Image;
        }
        private static Color[,] Copy(Color[,] Image, int Width, int Height)
        {
            Color[,] Copy_Image = new Color[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Copy_Image[i, j].R = Image[i, j].R;
                    Copy_Image[i, j].B = Image[i, j].B;
                    Copy_Image[i, j].G = Image[i, j].G;
                }
            }
            return Copy_Image;
        }
        private static Color[,] Convert_MyColor_Color(MyColor[,] Image)
        {
            int Height = Image.GetLength(0);
            int Width = Image.GetLength(1);
            Color[,] image = new Color[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int x = Image[i, j].red;
                    image[i, j].R = (double)(x);
                    x = Image[i, j].green;
                    image[i, j].G = (double)(x);
                    x = Image[i, j].blue;
                    image[i, j].B = (double)(x);
                }
            }
            //Color black = new Color(0, 0, 0);
            //for (int i = 0; i < Height + margin; i++)
            //{
            //    for (int j = 0; j <= margin / 2; j++)
            //        image[i, j] = image[i, (Width + margin) - 1 - j] = black;
            //}
            //for (int j = 0; j < Width + margin; j++)
            //{
            //    for (int i = 0; i <= margin / 2; i++)
            //        image[i, j] = image[(Height + margin) - 1 - i,j] = black;
            //}
            return image;
        }
    }
}
