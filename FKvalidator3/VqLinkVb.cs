using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKvalidator3_1
{
    internal class VqLinkVb
    {
        public string VqName { get; set; }
        public string Guid { get; set; }
        public string Account { get; set; }
        public string VbName { get; set; }
        public VqLinkVb(string vqname)
        {
            VqName = vqname;
        }
    }
}
