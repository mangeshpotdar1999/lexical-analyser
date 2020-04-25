using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using ParserGenerator;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InhaCC
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }



        Scanner scanner;

        private void bLG_Click(object sender, EventArgs e)
        {
            var sg = new ScannerGenerator();

            try
            {
                foreach (var line in rtbLLD.Lines)
                    sg.PushRule(line.Split(new[] { "=>" }, StringSplitOptions.None)[1].Replace("\"", "").Trim(), line.Split(new[] { "=>" }, StringSplitOptions.None)[0].Trim());
                sg.Generate();
                scanner = sg.CreateScannerInstance();
                rtbLS.AppendText("New scanner instance generated!\r\n" + sg.PrintDiagram());
            }
            catch (Exception ex)
            {
                rtbLS.Text = $"Scanner build error!\r\n{ex.Message}\r\n{ex.StackTrace}";
            }
        }

        private void bLT_Click(object sender, EventArgs e)
        {
            if (scanner == null)
            {
                rtbLS.AppendText("Create scanner instance before testing!\r\n");
                return;
            }

            rtbLS.AppendText(" ----------- Start Lexing -----------\r\n");
            scanner.AllocateTarget(rtbLT.Text);

            try
            {
                while (scanner.Valid())
                {
                    var ss = scanner.Next();
                    if (scanner.Error())
                        rtbLS.AppendText("Error!\r\n");
                    rtbLS.AppendText($"{ss.Item1},".PadRight(10) + $" {ss.Item2} - line:{ss.Item3}, column:{ss.Item4}\r\n");
                }
            }
            catch (Exception ex)
            {
                rtbLS.AppendText("Error!\r\nCheck test case!\r\n"+ex);
            }
            rtbLS.AppendText(" ------------ End Lexing ------------\r\n");
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void rtbLT_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
