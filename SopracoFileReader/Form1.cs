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

            string[] args = Environment.GetCommandLineArgs();
            if (args != null && args.Length > 1) {
                if (args[1] == "autostart") { button1_Click(button1, null); }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileProcessor fp = new FileProcessor();
            fp.OnStart();

            button1.Text = "Running"; 
        }

    }
}
