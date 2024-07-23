using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Activities;
using System.Diagnostics;
using System.ServiceProcess;
using System.Configuration;
namespace VSM.RUMA.SRV.FILEREADER
{
    class FileProcessorLoader : MarshalByRefObject
    {
        private const string COREMODULE = "VSM.RUMA.SRV.FILEREADER.CORE";

        internal static IFileProcessor LoadService()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.SetData("InleesDir", ConfigurationManager.AppSettings["Path"]);

                // nieuw appDomain starten met ShadowCopyFiles aan, hierdoor kunnen de dll's gewoon overschreven worden.
                AppDomainSetup adsetup = new AppDomainSetup();
                adsetup.ApplicationName = "FILEREADERMAINTASK";
                adsetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                adsetup.ShadowCopyFiles = "true";
                System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain app = AppDomain.CreateDomain(adsetup.ApplicationName, adevidence, adsetup);
                app.AssemblyResolve += app_AssemblyResolve;
                app.AssemblyLoad += app_AssemblyLoad;
                EventLog.WriteEntry("VSM.RUMA.SRV.FILEREADER", $"{nameof(FileProcessorLoader)}.{nameof(LoadService)} -  Properties.Settings.Default.Path: { ConfigurationManager.AppSettings["Path"]}", EventLogEntryType.Information);
                app.SetData("InleesDir", ConfigurationManager.AppSettings["Path"]);
                app.SetData("MaxWorkers", ConfigurationManager.AppSettings["MaxWorkers"]);
                app.SetData("AppName", typeof(FileProcessorLoader).Assembly.GetName().Name);
                FileProcessorLoader loader =
                    (FileProcessorLoader)app.CreateInstanceFromAndUnwrap
                    (typeof(FileProcessorLoader).Assembly.Location,
                        typeof(FileProcessorLoader).FullName);
                return loader.LoadCoreModule();

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("VSM.RUMA.SRV.FILEREADER", $"{nameof(FileProcessorLoader)}.{nameof(LoadService)} - Ex: {ex}", EventLogEntryType.Error);
                throw;
            }
        }


        private IFileProcessor LoadCoreModule()
        {
            try
            {
                Type IModule = typeof(IFileProcessor);
                Assembly assembly = AppDomain.CurrentDomain.Load(COREMODULE);
                var ReaderPlugintypes = assembly.GetTypes().Where(p => IModule.IsAssignableFrom(p));
                return (IFileProcessor)Activator.CreateInstance(ReaderPlugintypes.FirstOrDefault());
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("VSM.RUMA.SRV.FILEREADER", $"{nameof(FileProcessorLoader)}.{nameof(LoadCoreModule)} - Ex: {ex}", EventLogEntryType.Error);
                throw;
            }
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine("CurrentDomain Loading :" + args.LoadedAssembly.FullName);
        }

        static void app_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine("Loading :" + args.LoadedAssembly.FullName);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("CurrentDomain Resolving :" + args.Name);
            string keyName = new AssemblyName(args.Name).Name;
            if (keyName.Contains(".resources"))
            {
                return null;
            }
            return Assembly.LoadFrom(args.Name);
        }


        static Assembly app_AssemblyResolve(object sender, ResolveEventArgs args)
        {

            string keyName = new AssemblyName(args.Name).Name;
            Console.WriteLine("Resolving :" + keyName);
            if (keyName.Contains(".resources"))
            {
                return null;
            }
            return Assembly.LoadFrom(args.Name);
        }



    }
}
