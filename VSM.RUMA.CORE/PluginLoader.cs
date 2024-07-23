using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VSM.RUMA.CORE
{
    class PluginLoader
    {


        public static int InitPlugin(string title_class, string Filename, ref String pUsername, ref String pPassword)
        {
            try
            {
                if (System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + Filename))
                {
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                    Assembly assembly = Assembly.LoadFile(System.AppDomain.CurrentDomain.BaseDirectory + Filename);
                    Type t = assembly.GetType(title_class);
                    if (t != null)
                    {
                        IPluginStarter plugin;
                        object ClassObj = Activator.CreateInstance(t);

                        object Result = t.InvokeMember("getInstance", BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObj, null);
                        plugin = (IPluginStarter)Result;
                        if (plugin.getInlog(ref pUsername, ref pPassword))
                        {
                            return 0;
                        }
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("LoadPlugin" + Filename, ex);
                return 1;
            }
            return 1;

        }

        public static int LoadPlugin(string title_class, string Filename, Facade pFacade, DBConnectionToken pToken)
        {
            try
            {
                if (System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + Filename))
                {
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                    Assembly assembly = Assembly.LoadFile(System.AppDomain.CurrentDomain.BaseDirectory + Filename);
                    Type t = assembly.GetType(title_class);
                    if (t != null)
                    {
                        IPluginStarter plugin;
                        object ClassObj = Activator.CreateInstance(t);

                        object Result = t.InvokeMember("getInstance", BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObj, null);
                        plugin = (IPluginStarter)Result;
                        if (plugin.Execute(ref pFacade, pToken))
                        {
                            return 0;
                        }
                        return 2;
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("LoadPlugin" + Filename, ex);
                return 1;
            }
            return 1;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyPath;
            int endindex = args.Name.IndexOf(',');
            string dllname = args.Name.Substring(0, endindex);
            string ExecPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            //
            assemblyPath = Path.Combine(ExecPath, dllname + ".dll");
            if (!File.Exists(assemblyPath))
            {
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                assemblyPath = Path.Combine(basePath, dllname + ".dll");
            }
            if (!File.Exists(assemblyPath))
            {
                string binPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");
                assemblyPath = Path.Combine(binPath, dllname + ".dll");
            }
            if (!File.Exists(assemblyPath))
            {
                string pluginPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "plugin");
                assemblyPath = Path.Combine(pluginPath, dllname + ".dll");
            }

            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;


        }
    }
}
