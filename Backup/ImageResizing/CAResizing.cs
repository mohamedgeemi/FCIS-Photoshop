using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
namespace ImageResizing
{
    class CAResizing : Operation
    {
        Color[,] ImageMatrix;
        Color[,] resizedImage;
        RImage img;
        Stopwatch watch;
        public CAResizing()
            : base()
        {
            name = "CAResizing";
            register();
        }
        protected override void defineParameters()
        {
            Parameter w, h;
            w.min = 0.0f;
            w.max = ImageStream.CurrentImage.Width;
            w.value = (double)ImageStream.CurrentImage.Width;
            w.type = ParameterType.INT;
            h.min = 0.0f;
            h.max = ImageStream.CurrentImage.Height;
            h.value = (double)ImageStream.CurrentImage.Height;
            h.type = ParameterType.INT;
            parameters.Add("Width", w);
            parameters.Add("Height", h);
        }
        void ThreadProc(Object stateInfo)
        {
            double w, h;
            w = (double)parameters["Width"].value;
            h = (double)parameters["Height"].value;
            //resizedImage = ContentAwareResize.energyImage(ImageMatrix, (int)w, (int)h);
        }
        protected override void instructions()
        {
            ImageMatrix = ContentAwareResize.OpenImage(ImageStream.CurrentImage.Bitmap);
            img = ImageStream.CurrentImage;
            watch = Stopwatch.StartNew();
            BackgroundWorker bc = new BackgroundWorker();
            bc.DoWork += new DoWorkEventHandler(DoWork);
            bc.ProgressChanged += new ProgressChangedEventHandler(progressChanged);
            bc.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Completed);
            bc.WorkerReportsProgress = true;
            
            bc.RunWorkerAsync();
            UIAssembler.ShowProWin();
            // res = ContentAwareResize.DisplayImage(resizedImage);
            //ImageStream.CurrentImage.FlushBitmap(res);
        }
        void progressChanged(object sender, ProgressChangedEventArgs e)
        {
            Globals.ProWin.setProgress(e.ProgressPercentage);
        }
        void DoWork(object sender, DoWorkEventArgs e)
        {
            double w, h;
            w = (double)parameters["Width"].value;
            h = (double)parameters["Height"].value;
            int hf =(int)( ImageStream.CurrentImage.Height - h);
            int wf = (int)(ImageStream.CurrentImage.Width - w);
            resizedImage = ContentAwareResize.seamCarving_resize(ImageMatrix, hf, wf, (BackgroundWorker)sender);
        }
        void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            watch.Stop();
            time += watch.Elapsed;
            showTime();
            UIAssembler.HideProWin();
            Bitmap res = ContentAwareResize.DisplayImage(resizedImage);
            img.FlushBitmap(res);
            img.Flush();
        }
    }
}
