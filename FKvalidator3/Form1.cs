using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Configuration;
//using FKvalidator3_1;

namespace FKvalidator3_1
{


    internal partial class Form1 : Form
    {
        const string folderTemp = @".\FKTemp\";
        //readonly Encoding encodingFK = Encoding.Latin1;
        //public cfgForApp cfgForApp = new();
        //ConfigApp = new();
        WorkWithVks work_vks;

        public Form1( WorkWithVks work)
        {
            
            InitializeComponent();
            work_vks = work;
            checkBox1.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["MassImport"]);

        }
        
        private void ��������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            openFileDialog1.Filter = "zip|*.zip";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            work_vks.ReadWriteZip(openFileDialog1.FileName, checkBox1.Checked);
            MessageBox.Show("������� ����������");
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("��������� ������� ��� ��������� ������� ��� ���� �� ��\n����������� ������� ���������� 2 ������� ���� �� �� �. ���������� ����� - ������� ������ ���������\n��� �����: \n���(8552)-34-22-72\nt.me\\pycel", "� ���������");
        }
    }
}


