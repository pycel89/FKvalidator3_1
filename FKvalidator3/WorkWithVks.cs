using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FKvalidator3_1
{
    internal class WorkWithVks
    {
        private readonly List<LSClass> lsDsLg; //= ReadLS("DSLG.lst");
        private readonly List<LSClass> lsDsLv; //= ReadLS("DSLV.lst");
        private readonly List<LSClass> lsOther; //= ReadLS("OTHERLV.lst");
        private readonly List<string> list523;
        private readonly СfgForApp cfg;
        const string folderTemp = @".\FKTemp\";
        readonly Encoding encodingFK = new UTF8Encoding(false);
        public WorkWithVks(СfgForApp сfgForApp)
        {
            cfg = сfgForApp;
            lsDsLg = ReadLS("DSLG.lst");
            lsDsLv = ReadLS("DSLV.lst");
            if (cfg.OtherLv)
            {
                lsOther = ReadLS("OTHERLV.lst");
            }
            list523 = new();
            ControlCreateFolder(@".\out");
            ControlCreateFolder(@".\log");
            using (StreamReader streamReaderLV523 = new /*StreamReader*/(@"lv523.lst", encodingFK))
            {
                string? line;
                while ((line = streamReaderLV523.ReadLine()) is not null)
                {
                    list523.Add(line);
                }
            }
        }

        static void ControlCreateFolder(string nameFolder)
        {
            if (!Directory.Exists(nameFolder))
                Directory.CreateDirectory(nameFolder);
        }
        List<LSClass> ReadLS(string fileName)
        {
            List<LSClass> listLS = new();
            try
            {
                using (StreamReader streamReader = new(fileName, encodingFK))
                {
                    string? line;
                    while ((line = /*await*/ streamReader.ReadLine()) != null)
                    {
                        LSClass tempLS;
                        if (cfg.LsInnKppUse)
                        {
                            tempLS = new LSINNKPP(line);
                        }
                        else
                        {
                            tempLS = new LSINN(line);
                        }
                        //LSINN tempLS = new(line);
                        //tempLS.Set(line);
                        listLS.Add(tempLS);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка возникла в файле {fileName} ");
            }
            return listLS;
        }
        public void ReadWriteZip(string fileNameZip,bool massImport)
        {
            CleanFolder(folderTemp);
            ZipFile.ExtractToDirectory(fileNameZip, folderTemp);
            string fullZipName="";
            string[] fileTemp = Directory.GetFiles(folderTemp);
            if (!massImport)
            {
                if (fileTemp.Length != 2)
                {
                    MessageBox.Show("Архив не соответсвует Выписке");
                    return;
                }
                //работа в FKtem
                if (new FileInfo(fileTemp[0]).Length < new FileInfo(fileTemp[1]).Length)
                    (fileTemp[0], fileTemp[1]) = (fileTemp[1], fileTemp[0]);
                string fileVQ = fileTemp[1].Substring(9);
                string fileVB = fileTemp[0].Substring(9);
                //работа с VQ файлом
                (var dateVKS, var account) = ReadVQFile(fileVQ);
                if ((account is null) || (dateVKS is null))
                {
                    MessageBox.Show($"в файле {fileVQ} не найдены необходимые поля");
                    return;
                }
                ReadWriteVBFile(fileVB, account);
                fullZipName = @".\out\" + dateVKS + "_" + account + ".zip";
            }
            else
            {
                List<VqLinkVb> LineVqVb = MassReadWrite(fileTemp);
                for(int i=0;i<LineVqVb.Count;i++)
                {
                    ReadWriteVBFile(LineVqVb[i].VbName.Substring(LineVqVb[i].VbName.LastIndexOf(@"\")+1), LineVqVb[i].Account);
                }
                fullZipName = @".\out\" + fileNameZip.Substring(fileNameZip.LastIndexOf("\\") + 1);

            }
            if (File.Exists(fullZipName))
            {
                File.Delete(fullZipName);
            }
            CleanFolder(@".\out");
            ZipFile.CreateFromDirectory(folderTemp, fullZipName);
        }
        List<VqLinkVb> MassReadWrite(string[] _fileTemp)
        {
            List<VqLinkVb> listFilesInOrder = new List<VqLinkVb>();
            foreach (string file in _fileTemp)
            {
                if (file.Contains(".VQ"))
                {
                    VqLinkVb lineFileOrder = new(file);
                    List<XmlFKVks> listXmlVQ = new /*List<XmlFKVks>*/();
                    using (StreamReader streamReader = new /*StreamReader*/(file, encodingFK))
                    {
                        string? lineVQ = streamReader.ReadLine();
                        lineVQ = streamReader.ReadLine();
                        for (int i = 0; i < 20; ++i)
                        {
                            XmlFKVks tempLine = new /*XmlFKVks*/();
                            tempLine.Readxml(streamReader.ReadLine());
                            listXmlVQ.Add(tempLine);
                        }
                    }
                    XmlFKVks acc = listXmlVQ.Find(x => x.EndXml is @"</TtlPrt_TR_AccNum>");
                    XmlFKVks guid = listXmlVQ.Find(x => x.EndXml is @"</DocGUID>");
                    lineFileOrder.Account = acc.BodyXml;
                    lineFileOrder.Guid = guid.BodyXml;
                    listFilesInOrder.Add(lineFileOrder);
                }
            }
            foreach (string file in _fileTemp)
            {
                if (file.Contains(".BD"))
                {
                    List<XmlFKVks> listXmlVB = new /*List<XmlFKVks>*/();
                    using (StreamReader streamReader = new /*StreamReader*/(file, encodingFK))
                    {
                        string? lineVB = streamReader.ReadLine();
                        while ((lineVB = streamReader.ReadLine()) is not null)
                        {
                            if (lineVB.Contains("GUID"))
                            {
                                XmlFKVks tempLine = new /*XmlFKVks*/();
                                tempLine.Readxml(lineVB);
                                for (int i = 0; i < listFilesInOrder.Count; ++i)
                                {
                                    tempLine.BodyXml = tempLine.BodyXml.ToLower();
                                    if (listFilesInOrder[i].Guid.Contains(tempLine.BodyXml, StringComparison.InvariantCultureIgnoreCase))
                                        listFilesInOrder[i].VbName = file;
                                }

                            }
                        }
                    }
                }
            }
            return listFilesInOrder;
        }

        static void CleanFolder(string nameFolder)
        {
            /*красиво и праивльно удалять
             * DirectoryInfo DirInfo = new DirectoryInfo(NameFolder);
            foreach (FileInfo file in DirInfo.GetFiles())
            {
                file.Delete();                
            }*/
            // а вот так пришлось удалять потому что кто знает что могут случайно выбрать
            if (Directory.Exists(nameFolder))
                Directory.Delete(nameFolder, true);
            Directory.CreateDirectory(nameFolder);
        }
        (string?, string?) ReadVQFile(string fileName)
        {
            List<XmlFKVks> listXmlVQ = new /*List<XmlFKVks>*/(10);
            try
            {
                using (StreamReader streamReader = new /*StreamReader*/(folderTemp + fileName, encodingFK))
                {
                    string? lineVQ = streamReader.ReadLine();
                    lineVQ = streamReader.ReadLine();
                    for (int i = 0; i < 10; ++i)
                    {
                        XmlFKVks tempLine = new /*XmlFKVks*/();
                        tempLine.Readxml(streamReader.ReadLine());
                        listXmlVQ.Add(tempLine);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка возникала при обработке файла" + fileName);
            }
            XmlFKVks? dateVks = listXmlVQ.Find(x => x.EndXml is @"</TtlPrt_DocDate>");
            XmlFKVks? accVks = listXmlVQ.Find(x => x.EndXml is @"</TtlPrt_TR_AccNum>");
            return (dateVks.BodyXml, accVks.BodyXml);
        }
        void ReadWriteVBFile(string fileName, string account)
        {
            List<XmlFKVks> xmlFKVks = new(25);
            bool vKSReadToXML = false;
            try
            {
                using (StreamReader streamReaderVB = new /*StreamReader*/(folderTemp + fileName, encodingFK))
                using (StreamWriter streamWriter = new(folderTemp + "_" + fileName, true, encodingFK))
                {
                    string? lineVB;
                    while ((lineVB = streamReaderVB.ReadLine()) is not null)
                    {
                        // начинаем писать в xml только с этого тега большая часть выписки не нуждается в изменении
                        if (lineVB.Contains("<Inf_PAY>"))
                        {
                            vKSReadToXML = true;
                        }
                        if (!vKSReadToXML)
                        {
                            streamWriter.WriteLine(lineVB);
                        }
                        if (vKSReadToXML)
                        {
                            XmlFKVks tempLine = new /*XmlFKVks*/();
                            tempLine.Readxml(lineVB);
                            xmlFKVks.Add(tempLine);
                        }
                        if (lineVB.Contains("<KBK>"))
                        {
                            vKSReadToXML = false;
                        }
                        if ((!vKSReadToXML && (xmlFKVks.Count > 0)) || (xmlFKVks.Count == 24))
                        {
                            // все опарации с xml перед ее записью в ВКС
                            ChangeXmlVKS(xmlFKVks, account);
                            //запись выдернутого куска xml
                            foreach (XmlFKVks writeXmlLine in xmlFKVks)
                                streamWriter.WriteLine(writeXmlLine.ToString());
                            //очищаем xml тк размер вытаскиваемой части может меняться в зависимости от банка плательщика
                            xmlFKVks.Clear();
                            vKSReadToXML = false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка возникала при обработке файла" + fileName);
            }
            File.Delete(folderTemp + fileName);
            File.Move(folderTemp + "_" + fileName, folderTemp + fileName);
        }
        void ChangeXmlVKS(List<XmlFKVks> listXmlFkVks, string? accaunt)
        {
            int[] indexXml = GetAllIndexXml(listXmlFkVks);
            // если это исходящий платеж, его править не нужно.
            if (listXmlFkVks[indexXml[0]].BodyXml == accaunt)
                return;
            // иногда система пропускает платежи с неверным счетом получателя, и они не сядет если он неверный//
            if (listXmlFkVks[indexXml[0]].BodyXml != accaunt)
                listXmlFkVks[indexXml[3]].BodyXml = accaunt;
            // если нет кбк то такой платеж нас не интересует, и зачем его проверять
            if (indexXml[5] < 0)
            {
                return;
            }
            if ((cfg.InnGovServis.Length == 10) && (listXmlFkVks[indexXml[1]].BodyXml == cfg.InnGovServis))
            {
                GovServises(listXmlFkVks, lsDsLg, indexXml);
                GovServises(listXmlFkVks, lsDsLv, indexXml);
                return;
            }
            bool flagForChangeKbk = false;
            if (cfg.AllBankLg)
            {
                flagForChangeKbk = AllBanks(listXmlFkVks, lsDsLg, indexXml, false);
            }
            if ((!flagForChangeKbk) && (cfg.AllBankLv))
            {
                flagForChangeKbk = AllBanks(listXmlFkVks, lsDsLv, indexXml, false);
            }
            if ((!flagForChangeKbk) && (cfg.OtherLv))
            {
                flagForChangeKbk = AllBanks(listXmlFkVks, lsOther, indexXml, true);
            }
            if (flagForChangeKbk)
            {
                foreach (string line523 in list523)
                {
                    if (listXmlFkVks[indexXml[4]].BodyXml.Contains("эквайринг", StringComparison.InvariantCultureIgnoreCase))
                    {
                        listXmlFkVks[indexXml[5]].BodyXml = cfg.KbkTrue;
                        break;

                    }
                    if (listXmlFkVks[indexXml[4]].BodyXml.Contains("аренд", StringComparison.InvariantCultureIgnoreCase))
                    {
                        listXmlFkVks[indexXml[5]].BodyXml = cfg.KbkRent;
                        break;

                    }
                    if (listXmlFkVks[indexXml[4]].BodyXml.Contains(line523, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // если мы найдем в назначении платежа "запретное слово" то присвоим кбк который не даст автоматически зачислиться платежу
                        listXmlFkVks[indexXml[5]].BodyXml = cfg.KbkFalse;
                        break;
                    }
                    listXmlFkVks[indexXml[5]].BodyXml = cfg.KbkTrue;
                }
            }
            return;
        }
        void GovServises(List<XmlFKVks> listXmlFkVks, List<LSClass> listLs, int[] indexXml)
        {
            foreach (LSClass ls in listLs)
            {
                if (listXmlFkVks[indexXml[4]].BodyXml.Contains(ls.LS))
                {
                    listXmlFkVks[indexXml[1]].BodyXml = ls.INN;
                    listXmlFkVks[indexXml[1] + 1].BodyXml = ls.KPP;
                    listXmlFkVks[indexXml[5]].BodyXml = cfg.KbkTrue;
                    return;
                }
            }
        }
        bool AllBanks(List<XmlFKVks> listXmlFkVks, List<LSClass> listLs, int[] indexXml, bool otherLS)
        {
            LSClass? ls = listLs.Find(x => x.INN == listXmlFkVks[indexXml[1]].BodyXml);
            if (ls is not null)                //foreach (LSINN ls in listLs)
            {
                listXmlFkVks[indexXml[1] + 1].BodyXml = ls.KPP;
                string lstemp;
                lstemp = ls.LS.Substring(cfg.StartIndexNumberLs, cfg.LengNubmerLs);                
                // если мы находим часть счета в назначении платежа
                if (ChangeXmlLine(listXmlFkVks, ls, lstemp, indexXml[4]))
                {
                    return true;
                }
                //если мы находим часть счета в получателе
                if (ChangeXmlLine(listXmlFkVks, ls, lstemp, indexXml[2]))
                {
                    return true;
                }

                if (otherLS)
                {
                    listXmlFkVks[indexXml[2]].BodyXml = listXmlFkVks[indexXml[2]].BodyXml.Replace("ЛАГ", "ЛАГ ");
                    listXmlFkVks[indexXml[2]].BodyXml = listXmlFkVks[indexXml[2]].BodyXml.Replace("ЛБГ", "ЛБГ ");

                    listXmlFkVks[indexXml[4]].BodyXml = ls.LS + " " + listXmlFkVks[indexXml[4]].BodyXml;
                    return true;
                }
            }
            return false;
        }

        static bool ChangeXmlLine(List<XmlFKVks> listXmlFkVks, LSClass ls, string lstemp, int indexXml)
        {
            // если мы находим часть счета в назначении платежа
            if ((listXmlFkVks[indexXml].BodyXml != null) && (listXmlFkVks[indexXml].BodyXml.Contains(lstemp)) /*&& (listXmlFkVks[indexXml[1]].BodyXml == ls.INN)*/)
            {
                // если он там не целиком то мы его заменяем на цельный
                if (!listXmlFkVks[indexXml].BodyXml!.Contains(ls.LS))
                {
                    listXmlFkVks[indexXml].BodyXml = listXmlFkVks[indexXml].BodyXml.Replace(lstemp, " " + ls.LS + " ");
                }
                return true;
            }
            return false;
        }

        static int[] GetAllIndexXml(List<XmlFKVks> listXmlFkVks)
        {
            int[] indexXml = new int[6];
            indexXml[0] = listXmlFkVks.FindIndex(x => x.StartXml.Contains("<BS_PAY>"));
            indexXml[1] = listXmlFkVks.FindLastIndex(x => x.StartXml.Contains("<INN_PAY>"));
            indexXml[2] = listXmlFkVks.FindLastIndex(x => x.StartXml.Contains("<CName_PAY>"));
            indexXml[3] = listXmlFkVks.FindLastIndex(x => x.StartXml.Contains("<BS_PAY>"));
            indexXml[4] = listXmlFkVks.FindLastIndex(x => x.StartXml.Contains("<Purpose>"));
            indexXml[5] = listXmlFkVks.FindLastIndex(x => x.StartXml.Contains("<KBK>"));
            return indexXml;
        }
    }
}
