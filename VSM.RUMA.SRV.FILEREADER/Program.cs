using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace VSM.RUMA.SRV.FILEREADER
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && (System.Environment.UserInteractive || args.GetLength(0) > 0 && args[0] == "/Console"))
            {
                switch (args[0])
                {
                    case "/install":
                        System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "/uninstall":
                        System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    case "/Console":
                    case "/console":
                        FileProcessService Prog = new FileProcessService();
                        try
                        {
                            Prog.ConsoleStart();
                            Thread.Sleep(System.Threading.Timeout.Infinite);
                        }
                        finally
                        {
                            Prog.ConsoleStop();
                        }
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                        {
                            new FileProcessService()
                        };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
