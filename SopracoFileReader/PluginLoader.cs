using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.COMMONS;


namespace SopracoFileReader
{
    class PluginLoader
    {

        public static bool LoadPlugins(AFSavetoDB DB, string Pluginpath, MultiValueDictionary<FileWatchData, IReaderPlugin> PluginList)
        {

            string Filepath = Pluginpath.Replace("plugin\\", String.Empty);
            bool result = false;
            try
            {
                // kijken of de plugin dll ook in de hoofdmap staat
                if (System.IO.File.Exists(Filepath))
                {

                    string Assemblyname = Path.GetFileNameWithoutExtension(Filepath);
                    AppDomainSetup adsetup = new AppDomainSetup();
                    adsetup.ApplicationName = String.Format("FILEREADER{0}", Assemblyname);
                    adsetup.ShadowCopyFiles = "true";
                    System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                    AppDomain app = AppDomain.CreateDomain(Filepath, adevidence, adsetup);
                    app.UnhandledException += new UnhandledExceptionEventHandler(PluginDomain_UnhandledException);
                    app.DomainUnload += new EventHandler(PluginDomain_DomainUnload);
                    String InleesDir = AppDomain.CurrentDomain.GetData("InleesDir").ToString();
                    app.SetData("InleesDir", InleesDir);

                    Type ReaderPluginType = typeof(IReaderPlugin);
                    Assembly assembly = app.Load(Assemblyname);
//                    Assembly assembly = (IReaderPlugin)app.CreateInstanceAndUnwrap(Filepath, ReaderPluginType.FullName);

                    var ReaderPlugintypes = from p in assembly.GetTypes()
                                            where p.GetInterface(ReaderPluginType.Name) != null
                                            select p.FullName;
                    foreach (String Name in ReaderPlugintypes)
                    {
                        
                        Object Pluginobj = app.CreateInstanceAndUnwrap(Assemblyname, Name);
                        string Filter = (Pluginobj as IReaderPlugin).GetFilter().ToUpper();
                        (Pluginobj as IReaderPlugin).setSaveToDB(DB);
                        if (Pluginobj.GetType().GetInterface(typeof(IDirectoryReaderPlugin).Name) != null)
                        {
                            PluginList.Add(new FileWatchData(Filter, (Pluginobj as IDirectoryReaderPlugin).GetDirectory()), (Pluginobj as IReaderPlugin));
                        }
                        else { 
                            PluginList.Add(new FileWatchData(Filter, InleesDir), (Pluginobj as IReaderPlugin));
                        }
                        result = true;
                    }

                    #region TimerTasks
                    Type TimerTaskType = typeof(ITimerTask);
                    var TimerTasktypes = from p in assembly.GetTypes()
                                         where p.GetInterface(TimerTaskType.Name) != null
                                         select p.FullName;

                    foreach (String Name in TimerTasktypes)
                    {
                        // oude code voor het opstarten van de Timertasks via de filereader, dit loopt tegenwoordig via de windows scheduler
                        //ITimerTask Task;
                        //Task = (ITimerTask)app.CreateInstanceAndUnwrap(Assemblyname, Name);
                        //Task.setFacade(Facade.GetInstance());
                        //JobScheduler.ScheduleTask(Task);
                        unLogger.WriteError("ITimerTask is no longer supported by version 2.0 " + Name + " not scheduled");                     
                        result = true;
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {
                unLogger.WriteWarn("IReaderPlugin " + Path.GetFileNameWithoutExtension(Filepath), ex);
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

        public static IReaderPlugin LoadPluginDLL(DBConnectionToken pToken,string Module)
        {
            try
            {

                String Assemblyname = Module.Substring(0, Module.LastIndexOf('.'));
                String ModuleType = Module.Substring(Module.LastIndexOf('.'));


                AppDomainSetup adsetup = new AppDomainSetup();
                adsetup.ApplicationName = String.Format("FILEREADER {0}", Module);
                adsetup.ShadowCopyFiles = "true";
                System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain app = AppDomain.CreateDomain(Module, adevidence, adsetup);
                app.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                app.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
                Type ReaderPluginType = typeof(IReaderPlugin);
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                unLogger.WriteDebug("Resolving Assembly: " + Assemblyname);
                Assembly assembly = app.Load(Assemblyname);
                unLogger.WriteDebug("Resolved Assembly: " + assembly.FullName);
                var ReaderPlugintypes = assembly.GetTypes().Where(p => ReaderPluginType.IsAssignableFrom(p));

                foreach (Type t in ReaderPlugintypes)
                {
                    IReaderPlugin Plugin; ;
                    Plugin = (IReaderPlugin)app.CreateInstanceAndUnwrap(Assemblyname, t.FullName);
                    string Filter = Plugin.GetFilter().ToUpper();
                    Plugin.setSaveToDB(Facade.GetInstance().getSaveToDB(pToken));
                    if (Plugin.GetType().FullName == Module)
                     return Plugin;
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("LoadPluginDLL " + Module, ex);
            }
            unLogger.WriteError("Plugin Not Found");
            return null;
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            unLogger.WriteDebug("Loaded Assembly: " + args.LoadedAssembly.FullName);
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            unLogger.WriteError("Module Unloaded...");
        }


        //static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    string folderPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plugin");
        //    int endindex = args.Name.IndexOf(',');
        //    string dllname = args.Name.Substring(0, endindex);
        //    string assemblyPath = Path.Combine(folderPath, dllname + ".dll");
        //    Assembly assembly = Assembly.LoadFrom(assemblyPath);
        //    return assembly;
        //}


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
