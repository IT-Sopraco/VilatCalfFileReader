using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SopracoFileReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //filesystemwatcher textwatcher = new filesystemwatcher();
            //textwatcher.filter = "*.txt";
            //textwatcher.path = "c:\\temp\\testwatch";
            //textwatcher.created += textwatcher_created;
            //addwatcher(textwatcher);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileProcessor fp = new FileProcessor();
            fp.OnStart();

            button1.Text = "Running";
            
        }

    }
}
