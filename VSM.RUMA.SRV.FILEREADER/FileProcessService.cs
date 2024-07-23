using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace VSM.RUMA.SRV.FILEREADER
{
    partial class FileProcessService : ServiceBase
    {
        private IFileProcessor FileProcesser;
        public FileProcessService()
        {
            InitializeComponent();
            FileProcesser = FileProcessorLoader.LoadService();
        }
        public void ConsoleStart()
        {
            OnStart(new List<String>().ToArray());
        }

        protected override void OnContinue()
        {
            FileProcesser.OnContinue();
        }

        protected override void OnStart(string[] args)
        {
            FileProcesser.OnStart();
        }

        protected override void OnStop()
        {
            FileProcesser.OnStop();
        }

        protected override void OnPause()
        {
            FileProcesser.OnPause();
        }

        public void ConsoleStop()
        {
            OnStop();
        }
    }
}
