using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageResizing.BlurOp
{
    class SmoothOp : Operation
    {
        public SmoothOp():base()
        {
            name = "Smooth";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x;
            x.min = 1;
            x.max = 10;
            x.value = 1.0;
            x.type = ParameterType.FLOAT;
            parameters.Add("Weight",x);
        }
        protected override void instructions()
        {
            double v = (double)parameters["Weight"].value;
            Bitmap res = Blur.Smooth(ImageStream.CurrentImage.Bitmap, (int)v);
            ImageStream.CurrentImage.FlushBitmap(res);
        }
    }

    class GussianFilterOp : Operation
    {
        public GussianFilterOp()
            : base()
        {
            name = "GussianFilter";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x;
            x.min = 0;
            x.max = 10;
            x.value = 1.0;
            x.type = ParameterType.FLOAT;
            parameters.Add("Weight", x);
        }
        protected override void instructions()
        {
            double v = (double)parameters["Weight"].value;
            Bitmap res = Blur.GaussianFilter(ImageStream.CurrentImage.Bitmap, (int)v);
            ImageStream.CurrentImage.FlushBitmap(res);
        }
    }
    class SharpenOp : Operation
    {
        public SharpenOp()
            : base()
        {
            name = "Sharpen";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x;
            x.min = 1;
            x.max = 20;
            x.value = 1.0;
            x.type = ParameterType.FLOAT;
            parameters.Add("Weight", x);
        }
        protected override void instructions()
        {
            double v = (double)parameters["Weight"].value;
            Bitmap res = Blur.Sharpen(ImageStream.CurrentImage.Bitmap, (int)v);
            ImageStream.CurrentImage.FlushBitmap(res);
        }
    }
    class MedianRemovalOp : Operation
    {
        public MedianRemovalOp()
            : base()
        {
            name = "MedianRemoval";
            register();
        }
        protected override void defineParameters()
        {
            Parameter x;
            x.min = 1;
            x.max = 20;
            x.value = 1.0;
            x.type = ParameterType.FLOAT;
            parameters.Add("Weight", x);
        }
        protected override void instructions()
        {
            double v = (double)parameters["Weight"].value;
            Bitmap res = Blur.MeanRemoval(ImageStream.CurrentImage.Bitmap, (int)v);
            ImageStream.CurrentImage.FlushBitmap(res);
        }
    }
    class EdgeDetectOp : Operation
    {
        public EdgeDetectOp()
            : base()
        {
            name = "EdgeDetect";
            register();
        }
        protected override void defineParameters()
        {
            
        }
        protected override void instructions()
        {
            
            Bitmap res = Blur.EdgeDetection(ImageStream.CurrentImage.Bitmap);
            ImageStream.CurrentImage.FlushBitmap(res);
        }
    }

}
