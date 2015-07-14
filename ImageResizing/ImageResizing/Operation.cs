using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public enum ParameterType { INT, FLOAT, BITMAP,BOOl };
public struct Parameter
{
    public object value;
    public ParameterType type;
    public float min;
    public float max;
}
namespace ImageResizing
{
    abstract class Operation
    {
        protected Dictionary<string, Parameter> parameters;
        protected string name;
        protected TimeSpan time;
        public string Name
        {
            get { return name; }
        }

        public Operation()
        {
            parameters = new Dictionary<string, Parameter>();
            time = new TimeSpan();
            
        }
        protected void register()
        {
            Globals.Operations.Add(name, this);
        }
        public void execute()
        {
            
            parameters.Clear();
            defineParameters();
            if (parameters.Count > 0)
                UIAssembler.ShowParameter(name, parameters);
            else
                run();
            
        }
        public void setPara(Dictionary<string,Parameter> p)
        {
            parameters = p;
        }
        protected void showTime()
        {
            string io = time.ToString("mm\\:ss\\.ff");

            UIAssembler.ShowMsg("Time = "+io);
        }
        public void run()
        {
            if (ImageStream.CurrentImage != null)
            {
                var watch = Stopwatch.StartNew();
                instructions();
                watch.Stop();
                time = watch.Elapsed;
                ImageStream.CurrentImage.Flush();
                if (name != "CAResizing")
                    showTime();
            }
        }
        protected abstract void instructions();
        protected abstract void defineParameters();
    }

}
