using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageResizing
{
    class BilResize : Operation
    {

        public BilResize()
            : base()
        {
            name = "BilResize";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x;
            x.min = 1;
            x.max = 1000;

            x.value = 0.0;
            if (ImageStream.CurrentImage != null)
            {
                x.max = ImageStream.CurrentImage.Width;
                x.value = (double)ImageStream.CurrentImage.Width;
            }
            x.type = ParameterType.INT;
            parameters.Add("Width", x);
            Parameter y;
            y.min = 1;
            y.max = 1000;
            y.value = 0.0;
            if (ImageStream.CurrentImage != null)
            {
                y.max = ImageStream.CurrentImage.Height;
                y.value = (double)ImageStream.CurrentImage.Height;
            }
            y.type = ParameterType.INT;
            parameters.Add("Height", y);
        }
        protected override void instructions()
        {
            double v = (double)parameters["Height"].value;
            double h = (double)parameters["Width"].value;

            Bitmap res = ImageProcOp.BilinearResize(ImageStream.CurrentImage.Bitmap, (int)h, (int)v, true);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }

    class Greyscale : Operation
    {

        public Greyscale()
            : base()
        {
            name = "Greyscale";
            register();
        }
        protected override void defineParameters()
        {

        }
        protected override void instructions()
        {
            Bitmap res = ImageProcOp.GrayScale(ImageStream.CurrentImage.Bitmap);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }

    class Brightness : Operation
    {

        public Brightness()
            : base()
        {
            name = "Brightness";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.min = -255;
            x.max = 255;
            x.value = 0.0;
            x.type = ParameterType.INT;
            parameters.Add("Value", x);
        }
        protected override void instructions()
        {
            double v = (double)parameters["Value"].value;
            Bitmap res = ImageProcOp.Brightness(ImageStream.CurrentImage.Bitmap, (int)v);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }
    class InvertOp : Operation
    {

        public InvertOp()
            : base()
        {
            name = "Invert";
            register();
        }
        protected override void defineParameters()
        {

        }
        protected override void instructions()
        {

            Bitmap res = ImageProcOp.Invert(ImageStream.CurrentImage.Bitmap);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }

    class GammaOp : Operation
    {

        public GammaOp()
            : base()
        {
            name = "Gamma";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.max = 0.5f;
            x.min = 0.2f;
            x.value = 0.2;
            x.type = ParameterType.FLOAT;
            parameters.Add("Red", x);

            Parameter y = new Parameter();
            y.max = 0.5f;
            y.min = 0.2f;
            y.value = 0.2;
            y.type = ParameterType.FLOAT;
            parameters.Add("Green", y);

            Parameter z = new Parameter();
            z.max = 0.5f;
            z.min = 0.2f;
            z.value = 0.2;
            z.type = ParameterType.FLOAT;
            parameters.Add("Blue", z);
        }
        protected override void instructions()
        {
            double x = (double)parameters["Red"].value;
            double y = (double)parameters["Green"].value;
            double z = (double)parameters["Blue"].value;
            Bitmap res = ImageProcOp.Gamma(ImageStream.CurrentImage.Bitmap, x, y, z);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }

    class ContrastOp : Operation
    {

        public ContrastOp()
            : base()
        {
            name = "Contrast";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.max = 100f;
            x.min = -100f;
            x.value = 0.0;
            x.type = ParameterType.FLOAT;
            parameters.Add("Value", x);
        }
        protected override void instructions()
        {
            double x = (double)parameters["Value"].value;
            Bitmap res = ImageProcOp.Contrast(ImageStream.CurrentImage.Bitmap, x);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }

    class AddImages : Operation
    {

        public AddImages()
            : base()
        {
            name = "AddImages";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.value = null;
            x.type = ParameterType.BITMAP;
            parameters.Add("Value", x);
        }
        protected override void instructions()
        {
            RImage x = (RImage)parameters["Value"].value;
            Bitmap res = ImageProcOp.AddPictures(ImageStream.CurrentImage.Bitmap, x.Bitmap);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }
    class Sltppr : Operation
    {

        public Sltppr()
            : base()
        {
            name = "Sltppr";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.min = 1;
            x.max = 500;
            x.value = 1.0;
            x.type = ParameterType.INT;
            parameters.Add("Size", x);

        }
        protected override void instructions()
        {
            double s = (double)parameters["Size"].value;
            Bitmap res = ImageProcOp.NoiseGenerate(ImageStream.CurrentImage.Bitmap, (int)s);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }
    class Median : Operation
    {

        public Median()
            : base()
        {
            name = "Median";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.min = 1;
            x.max = 10;
            x.value = 1.0;
            x.type = ParameterType.INT;
            parameters.Add("Size", x);
            Parameter y = new Parameter();
            y.type = ParameterType.BOOl;
            y.value = false;
            parameters.Add("GreyScale", y);

        }
        protected override void instructions()
        {
            double s = (double)parameters["Size"].value;
            bool b = (bool)parameters["GreyScale"].value;
            Bitmap res = ImageProcOp.Median(ImageStream.CurrentImage.Bitmap, (int)s,b);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }
    class Tint : Operation
    {

        public Tint()
            : base()
        {
            name = "Tint";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x = new Parameter();
            x.min = 1;
            x.max = 100;
            x.value = 0.0;
            x.type = ParameterType.INT;
            parameters.Add("Red", x);
            Parameter y = new Parameter();
            y.min = 1;
            y.max = 100;
            y.value = 0.0;
            y.type = ParameterType.INT;
            parameters.Add("Green", y);
            Parameter z = new Parameter();
            z.min = 1;
            z.max = 100;
            z.value = 0.0;
            z.type = ParameterType.INT;
            parameters.Add("Blue", z);
            

        }
        protected override void instructions()
        {
            double x = (double)parameters["Red"].value;
            double y = (double)parameters["Green"].value;
            double z = (double)parameters["Blue"].value;

            Bitmap res = ImageProcOp.Tint(ImageStream.CurrentImage.Bitmap, (float)x / 100.0f, (float)y / 100.0f, (float)z / 100.0f);
            ImageStream.CurrentImage.FlushBitmap(res);
        }

    }

}
