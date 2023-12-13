using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace FKvalidator3_1
{
    public abstract class LSClass
    {
        public virtual string LS { get; set; }
        public virtual string INN { get; set; }
        public virtual string KPP { get; set; }
        public LSClass(string line)
        {
            int count = line.Count(c => c == '=');
            if ((count > 2) || (count < 1))
                throw new ArgumentException($"{line} в указаной строке неожиданное количество знаков =");
            if (count == 2)
            {
                KPP = line.Substring(line.LastIndexOf("=") + 1);
                line = line.Replace("=" + KPP, "");
            }
            else 
                KPP = string.Empty;
            INN = line.Substring(line.IndexOf("=") + 1);
            LS = line.Replace("=" + INN, "");
        }
    }
    public class LSINN: LSClass
    {
        private static readonly string staticKpp;
        public override string KPP { get { return staticKpp; } }
        static  LSINN() 
        {
            staticKpp = ConfigurationManager.AppSettings["KPP"];
        }
        public LSINN(string line)
            :base(line)
        {
            
        }
        //public 
    }
    public class LSINNKPP : LSClass
    {
        public LSINNKPP(string line)
            :base(line)
        {
           
        }
        //public string? KPP { get; set; }
        //public void
        /*public new void Set(string line)
        {
            KPP= line.Substring(line.LastIndexOf("=") + 1);
            base.Set(line.Replace("=" + KPP, ""));
        }*/
    }
}
