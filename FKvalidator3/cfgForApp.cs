using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKvalidator3_1
{
    public class СfgForApp
    {
        public bool LsInnKppUse { get;  }
        public string KbkTrue { get;  }
        public string KbkFalse { get;  }
        public string KbkRent { get; }
        public int StartIndexNumberLs { get; }
        public int LengNubmerLs { get;  }
        public string InnGovServis { get; }
        public bool AllBankLv { get; }
        public bool AllBankLg { get; }
        public bool OtherLv { get; }

        public СfgForApp()
        {
            LsInnKppUse = Convert.ToBoolean(ConfigurationManager.AppSettings["lsinnkpp"]??throw new ArgumentException(nameof(LsInnKppUse)));
            KbkTrue = ConfigurationManager.AppSettings["KBKTrue"] ?? throw new ArgumentException(nameof(KbkTrue));
            KbkFalse = ConfigurationManager.AppSettings["KBKFalse"] ?? throw new ArgumentException(nameof(KbkFalse));
            KbkRent = ConfigurationManager.AppSettings["KBKRent"] ?? throw new ArgumentException(nameof(KbkRent));
            StartIndexNumberLs = Convert.ToInt16(ConfigurationManager.AppSettings["StartIndexNumberLs"] ?? throw new ArgumentException(nameof(StartIndexNumberLs)));
            LengNubmerLs = Convert.ToInt16(ConfigurationManager.AppSettings["LengNumbersLs"] ?? throw new ArgumentException(nameof(LengNubmerLs)));
            InnGovServis = ConfigurationManager.AppSettings["INNGovServis"] ?? throw new ArgumentException(nameof(InnGovServis));
            AllBankLg = Convert.ToBoolean(ConfigurationManager.AppSettings["AllBankLg"] ?? throw new ArgumentException(nameof(AllBankLg)));
            AllBankLv = Convert.ToBoolean(ConfigurationManager.AppSettings["AllBankLv"] ?? throw new ArgumentException(nameof(AllBankLv)));
            OtherLv = Convert.ToBoolean(ConfigurationManager.AppSettings["OtherLv"] ?? throw new ArgumentException(nameof(OtherLv)));
        }
    }
}
