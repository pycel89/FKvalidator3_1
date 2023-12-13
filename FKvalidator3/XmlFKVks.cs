using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKvalidator3_1
{
    internal class XmlFKVks
    {
        public string? StartXml { get; set; }
        public string? BodyXml { get; set; }
        public string? EndXml { get; set; }
        public void Readxml(string? FullStr)
        {
            if (FullStr?.IndexOf(@"</") >= 0)
            {
                EndXml = FullStr.Substring(FullStr.IndexOf(@"</"));
                FullStr = FullStr.Replace(EndXml, "");
                BodyXml = FullStr.Substring(FullStr.LastIndexOf(">") + 1);
                StartXml = FullStr.Replace(BodyXml, "");
            }
            else
            {
                BodyXml = EndXml = null;
                StartXml = FullStr;
            }
        }
        // так надо, да и это более логичное название для функции
        public override string  ToString()
        {
            return StartXml + BodyXml + EndXml;
            
        }
    }
}
