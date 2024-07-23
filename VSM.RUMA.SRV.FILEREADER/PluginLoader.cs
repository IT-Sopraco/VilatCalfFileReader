using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.COMMONS;


namespace VSM.RUMA.SRV.FILEREADER
{
    class PluginLoader : MarshalByRefObject
    {

        //public static bool LoadPluginDLL(VSM.RUMA.CORE.DB.MYSQL.MySQLSavetoDB DB, string Filepath, TimerJobs JobScheduler, MultiValueDictionary<String, IReaderPlugin> PluginList)
        public static bool LoadPluginDLL(AFSavetoDB DB, string Pluginpath, MultiValueDictionary<FileWatchData, IReaderPlugin> PluginList)
        {


            string Filepath = Pluginpath.Replace("plugin\\", String.Empty);
            bool result = false;
            try
            {
                // kijken of de plugin dll ook in de hoofdmap staat
                if (System.IO.File.Exists(Filepath))
                {
                    String Assemblyname = Path.GetFileNameWithoutExtension(Filepath);
                    AppDomainSetup adsetup = new AppDomainSetup();
                    adsetup.ApplicationName = String.Format("FILEREADER{0}",Assemblyname);
                    //adsetup.ShadowCopyDirectories = String.Format("", Path.GetDirectoryName(Filepath), adsetup.PrivateBinPath, adsetup.ApplicationBase);
                    adsetup.ShadowCopyFiles = "true";
                    System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                    AppDomain app = AppDomain.CreateDomain(Filepath, adevidence, adsetup);
                    //app.SetShadowCopyFiles();
                    app.UnhandledException += new UnhandledExceptionEventHandler(PluginDomain_UnhandledException);
                    app.DomainUnload += new EventHandler(PluginDomain_DomainUnload);
                    String InleesDir = AppDomain.CurrentDomain.GetData("InleesDir").ToString();
                    app.SetData("InleesDir", InleesDir);

                    Type ReaderPluginType = typeof(IReaderPlugin);
                    Assembly assembly = app.Load(Assemblyname);
                    var ReaderPlugintypes = from p in assembly.GetTypes()
                                            where p.GetInterface(ReaderPluginType.Name) != null
                                            select p.FullName;
                    foreach (String Name in ReaderPlugintypes)
                    {
                        IReaderPlugin Plugin;
                        Plugin = (IReaderPlugin)app.CreateInstanceAndUnwrap(Assemblyname, Name);
                        string Filter = Plugin.GetFilter().ToUpper();
                        Plugin.setSaveToDB(DB);

                        //TODO IReaderPlugin updaten om inleesmap aan te geven
                        PluginList.Add(new FileWatchData(Filter, InleesDir), Plugin);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("IReaderPlugin " + Path.GetFileNameWithoutExtension(Filepath), ex);
                return false;
            }
            return result;
        }

        static void PluginDomain_DomainUnload(object sender, EventArgs e)
        {
            unLogger.WriteError("Module Unloaded...");
        }




        static void PluginDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder SB_Error = new StringBuilder();
            if (e.IsTerminating) SB_Error.Append("[FATAL]");

            try
            {

                Exception ex = (Exception)e.ExceptionObject;
                SB_Error.Append(ex.Message);
                unLogger.WriteError(SB_Error.ToString(), ex);
            }
            catch
            {
                SB_Error.Append("Onbekende Fout!");
                unLogger.WriteError(SB_Error.ToString());
                unLogger.WriteObject(SB_Error.ToString(), e.ExceptionObject);
            }

        }
    }
}
