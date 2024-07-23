using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.SRV.FILEREADER
{


    public partial class XMLreaderservice : ServiceBase
    {
        private List<FileSystemWatcher> mFileWatchers = new List<FileSystemWatcher>();
        private MultiValueDictionary<FileWatchData, IReaderPlugin> mFilePlugins = new MultiValueDictionary<FileWatchData, IReaderPlugin>();
        //private Dictionary<String, IReaderPlugin> mFilePlugins = new Dictionary<String, IReaderPlugin>();
        //private Dictionary<String, Thread> mThreads = new Dictionary<String, Thread>();
<<<<<<< HEAD
        private Dictionary<String, ManualResetEvent> mCallbacks = new Dictionary<String, ManualResetEvent>();
=======
        //private Dictionary<String, ManualResetEvent> mCallbacks = new Dictionary<String, ManualResetEvent>();
>>>>>>> develop
        private AFSavetoDB DB;
        private DBConnectionToken mAgrofactuurToken;
        //private TimerJobs Scheduler;
        private ConcurrentQueue<FileReaderQueueItem> queue = new ConcurrentQueue<FileReaderQueueItem>();
        private Object monitor = new Object();

        static readonly object padlock = new object();

        public XMLreaderservice()
        {
            //AppDomain.CurrentDomain.SetShadowCopyFiles();
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            unLogger.Configure("log4net.config", unRechten.getDBHost());
            log4net.ThreadContext.Properties["Filename"] = "main";
            if (!Directory.Exists(Win32.GetBaseDir() + "plugin\\"))
                Directory.CreateDirectory(Win32.GetBaseDir() + "plugin\\");
            unLogger.WriteInfo(String.Format("using Database: {0}", unRechten.getDBHost()));
            AddFileWatch(Win32.GetBaseDir() + "plugin\\", Properties.Settings.Default.Filter, false);
            mAgrofactuurToken = Facade.GetInstance().getRechten().Inloggen("00000", Facade.GetInstance().getRechten().base64Encode("kcD0QJC3"));

            StringBuilder QRY_Childs = new StringBuilder();
            QRY_Childs.Append(" SELECT *");
            QRY_Childs.Append(" FROM slavetest.ticker");
            QRY_Childs.Append(" WHERE mysqltype = 'master'");
            DataTable Connections = DB.GetDataBase().QueryData(mAgrofactuurToken, QRY_Childs);
            //foreach (DataRow Row in Connections.Rows)
            //{

            //    /Facade.GetInstance().getRechten().VeranderAdministratie(ref mAgrofactuurToken,"",Convert.ToInt32(Row["programid"]));
            //    mAgrofactuurToken.AddChildConnection(Convert.ToInt32(Row["programid"]),


            //}
            mFilePlugins.ValueAdded += new MultiValueDictionaryChanged(mFilePlugins_ValueAdded);
            mFilePlugins.KeyAdded += new MultiValueDictionaryKeyAdded(mFilePlugins_KeyAdded);
            InitializeComponent();
            int minworkers;
            int minio;
            int maxworkers;
            int maxio;

            AppDomain.CurrentDomain.SetData("InleesDir", Properties.Settings.Default.BasePath);

            ThreadPool.GetMinThreads(out minworkers, out minio);
            unLogger.WriteInfo(String.Format("Thread Pool Min :  {0}, {1}", minworkers, minio));
            ThreadPool.GetMaxThreads(out maxworkers, out maxio);
            //maxworkers = 24;
            //maxio = 24;

            //ThreadPool.SetMaxThreads(maxworkers, maxio);
            unLogger.WriteInfo(String.Format("Thread Pool Max :  {0}, {1}", maxworkers, maxio));

            //ThreadPool.SetMinThreads(50, 50);

            //while (true)
            //{
            //    unLogger.WriteError("Test");
            //    Thread.Sleep(1000);
            //}
            //unLogger.WriteError("Ready");
        }




        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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

        private FileSystemWatcher AddFileWatch(String pPath, String pFilter, bool SubDirectories)
        {
            FileSystemWatcher lFileWatcher = new FileSystemWatcher(pPath);
            lFileWatcher.EnableRaisingEvents = false;
            lFileWatcher.InternalBufferSize = 65536;
            lFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime;
            lFileWatcher.IncludeSubdirectories = SubDirectories;
            lFileWatcher.Filter = pFilter;
            lFileWatcher.Created += new FileSystemEventHandler(XmlWatcher_CreatedOrChanged);
            lFileWatcher.Changed += new FileSystemEventHandler(XmlWatcher_CreatedOrChanged);
            lFileWatcher.Renamed += new RenamedEventHandler(XmlWatcher_Renamed);
            lFileWatcher.Error += new ErrorEventHandler(lFileWatcher_Error);
            mFileWatchers.Add(lFileWatcher);
            return lFileWatcher;
        }

        void lFileWatcher_Error(object sender, ErrorEventArgs e)
        {
            unLogger.WriteError("FOUT: Buffer overflow bij Filewatch ", e.GetException());
        }

        void XmlWatcher_CreatedOrChanged(object sender, FileSystemEventArgs e)
        {
            unLogger.WriteInfo(e.Name + " is Aangemaakt of Aangepast");
            try
            {

                FileSystemWatcher lFileWatcher = (FileSystemWatcher)sender;
                FileWatchData lFWD = new FileWatchData(lFileWatcher.Filter, lFileWatcher.Path);
                String[] PathList = e.Name.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                //String DB = PathList[0];

                if ((!lFileWatcher.IncludeSubdirectories) || PathList.Length >= 2)
                {
                    lock (padlock)
                    {
                        FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(e.FullPath, PathList, lFWD);

                        if (!queue.Contains(FileReaderItem) && File.Exists(e.FullPath))
                        {
                            unLogger.WriteDebug(e.FullPath + " in Wachtrij voor inlezen");
                            //
                            //ManualResetEvent thre = new ManualResetEvent(false);
                            //WaitCallback starter = delegate { Inlezen(e.FullPath, PathList, lFWD); thre.Set(); };
                            //ThreadPool.QueueUserWorkItem(starter);
                            //System.Threading.ThreadStart starter = delegate { Inlezen(e.FullPath, DB, lFileWatcher.Filter); };
                            //Thread thr = new Thread(starter);
                            //thr.Name = e.Name + "][" + lFileWatcher.Filter;
                            queue.Enqueue(FileReaderItem);
                            //thr.Start();
                        }
                        //else if (mCallbacks[e.FullPath + "," + lFileWatcher.Filter].WaitOne(0))
                        //{
                        //    unLogger.WriteDebug(e.FullPath + " is al Ingelezen");
                        //    mCallbacks.Remove(e.FullPath + "," + lFileWatcher.Filter);
                        //}
                        else
                        {
                            //SetLastAccessTimeFile(e.FullPath);
                            unLogger.WriteDebug(e.FullPath + "  staat al in wachtrij om in te lezen");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
            }

        }

        void XmlWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            unLogger.WriteInfo(e.Name + " is Hernoemd");
            try
            {
                FileSystemWatcher lFileWatcher = (FileSystemWatcher)sender;
                FileWatchData lFWD = new FileWatchData(lFileWatcher.Filter, lFileWatcher.Path);


                if ((Path.GetExtension(e.OldName).ToUpper() != Path.GetExtension(lFileWatcher.Filter.Replace("*", "")).ToUpper() &&
                     Path.GetExtension(e.Name).ToUpper() == Path.GetExtension(lFileWatcher.Filter.Replace("*", "")).ToUpper()) ||
                    (Path.GetExtension(Path.GetFileNameWithoutExtension(e.OldName)).ToUpper() != lFileWatcher.Filter.Replace("*", "").ToUpper() &&
                     Path.GetExtension(Path.GetFileNameWithoutExtension(e.Name)).ToUpper() == lFileWatcher.Filter.Replace("*", "").ToUpper()) ||
                     (WasExcluded(e.OldName, lFWD) &&
                     Path.GetExtension(Path.GetFileNameWithoutExtension(e.Name)).ToUpper() == lFileWatcher.Filter.Replace("*", "").ToUpper()) ||
                     (WasExcluded(e.OldName, lFWD) &&
                     Path.GetExtension(e.Name).ToUpper() == Path.GetExtension(lFileWatcher.Filter.Replace("*", "")).ToUpper()))
                {
                    String[] PathList = e.Name.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                    if ((!lFileWatcher.IncludeSubdirectories) || PathList.Length >= 2)
                    {
                        lock (padlock)
                        {
                            FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(e.FullPath, PathList, lFWD);

                            if (!queue.Contains(FileReaderItem) && File.Exists(e.FullPath))
                            {
                                unLogger.WriteDebug(e.FullPath + " in Wachtrij voor inlezen");
                                //ManualResetEvent thre = new ManualResetEvent(false);
                                //WaitCallback starter = delegate { Inlezen(e.FullPath, PathList, lFWD); thre.Set(); };
                                //ThreadPool.QueueUserWorkItem(starter);
                                //ThreadPool.
                                queue.Enqueue(FileReaderItem);



                                //System.Threading.ThreadStart starter = delegate { Inlezen(e.FullPath, DB, lFileWatcher.Filter); };
                                //Thread thr = new Thread(starter);
                                //thr.Name = e.Name + "][" + lFileWatcher.Filter;
                                //mThreads.Add(e.Name + "][" + lFileWatcher.Filter, thr);
                                //thr.Start();
                            }
                            else
                            {
                                //SetLastAccessTimeFile(e.FullPath);
                                unLogger.WriteDebug(e.FullPath + " staat al in wachtrij om in te lezen");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
            }

        }


        private void ProcessQueue()
        {
            FileReaderQueueItem FileReaderItem;
            if (queue.TryDequeue(out FileReaderItem))
            {
                // wachten totdat de ftp server klaar is met uploaden.
                while (File.GetLastWriteTime(FileReaderItem.File) > DateTime.Now.AddSeconds(-10) ||
                      File.GetLastAccessTime(FileReaderItem.File) > DateTime.Now.AddSeconds(-10) ||
                      File.GetCreationTime(FileReaderItem.File) > DateTime.Now.AddSeconds(-10) ||
                      !FileSizeChanged(FileReaderItem.File))
                {
                    unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " nog niet klaar met uploaden wachten...");
                    Thread.Sleep(11000);
                    if (!File.Exists(FileReaderItem.File))
                    {
                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " is verwijderd of verplaatst(was een TMP bestand?) inlezen afbreken.");
                        return;
                    }
                }
            }
        }


        //void Inlezen(String pPath,String[] PathList, FileWatchData pFWD)
        //void Inlezen(String pPath,String[] PathList, FileWatchData pFWD)
        void Inlezen()
        {
            FileReaderQueueItem FileReaderItem;

            if (!queue.TryDequeue(out FileReaderItem))
            {

            }
            else
            {
                // wachten totdat de ftp server klaar is met uploaden.
                while (File.GetLastWriteTime(FileReaderItem.File) > DateTime.Now.AddSeconds(-10) ||
                      File.GetLastAccessTime(FileReaderItem.File) > DateTime.Now.AddSeconds(-10) ||
                      File.GetCreationTime(FileReaderItem.File) > DateTime.Now.AddSeconds(-10) ||
                      !FileSizeChanged(FileReaderItem.File))
                {
                    unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " nog niet klaar met uploaden wachten...");
                    Thread.Sleep(11000);
                    if (!File.Exists(FileReaderItem.File))
                    {
<<<<<<< HEAD
                        case "agrobase":
                            switch (PathList[1].ToLower())
                            {
                                case "agrobase_t4c3":
                                    programid = 2;         // lely robot
                                    mAgrofactuurToken.AddChildConnection(2, mAgrofactuurToken.getChildConnection(2));
                                    break;
                                case "agrobase_VC5":
                                    programid = 53;         // progid 1,4
                                    mAgrofactuurToken.AddChildConnection(53, mAgrofactuurToken.getChildConnection(53));
                                    break;
                                case "processserver":
                                    programid = 1;

                                    if (pPath.ToLower().Contains("lely"))
                                    {
                                        programid = 53;
                                    }
                                    else if (pPath.ToLower().Contains("veecode"))
                                    {
                                        programid = 53;
                                    }
                                    else if (pPath.ToLower().Contains("id2000"))
                                    {
                                        programid = 4200;
                                    }
                                    else if (pPath.ToLower().Contains("fullwood"))
                                    {
                                        programid = 3500;
                                    }
                                    else if (pPath.ToLower().Contains("gea"))
                                    {
                                        programid = 3600;
                                    }
                                    else if (pPath.ToLower().Contains("dairyplan"))
                                    {
                                        programid = 3600;
                                    }
                                    else if (pPath.ToLower().Contains("delaval"))
                                    {
                                        programid = 3700;
                                    }
                                    else if (pPath.ToLower().Contains("delpro"))
                                    {
                                        programid = 3700;
                                    }
                                    else if (pPath.ToLower().Contains("alpro"))
                                    {
                                        programid = 3700;
                                    }
                                    else if (pPath.ToLower().Contains("velos"))
                                    {
                                        programid = 3800;
                                    }
                                    else if (pPath.ToLower().Contains("sac"))
                                    {
                                        programid = 3900;
                                    }
                                    else if (pPath.ToLower().Contains("dairymaster"))
                                    {
                                        programid = 4000;
                                    }
                                    else if (pPath.ToLower().Contains("boumatic"))
                                    {
                                        programid = 4100;
                                    }
                                    else if (pPath.ToLower().Contains("bmi"))
                                    {
                                        programid = 4100;
                                    }

                                    // progid 1,4
                                    mAgrofactuurToken.AddChildConnection(programid, mAgrofactuurToken.getChildConnection(programid));
                                    break;
                                case "hetdeenssysteem":
                                    programid = 13000;         // progid 1,4
                                    mAgrofactuurToken.AddChildConnection(13000, mAgrofactuurToken.getChildConnection(13000));
                                    break;
                                default:
                                    programid = 1;         // progid 1,4
                                    mAgrofactuurToken.AddChildConnection(1, mAgrofactuurToken.getChildConnection(1));
                                    break;
                            }
                            break;
                        case "agrobase_calf":
                            switch (PathList[1].ToLower())
                            {
=======
                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " is verwijderd of verplaatst(was een TMP bestand?) inlezen afbreken.");
>>>>>>> develop

                    }
                }
            }



            try
            {

                log4net.ThreadContext.Properties["Filename"] = Path.GetFileName(FileReaderItem.File);
                string ext = ".old";

                unLogger.WriteDebug("programid :" + FileReaderItem.ProgramId);
                System.Threading.Monitor.Enter(mFilePlugins);

                if (mFilePlugins.ContainsKey(FileReaderItem.FileWatcherInfo))
                {
                    List<IReaderPlugin> Plugins = mFilePlugins[FileReaderItem.FileWatcherInfo];
                    AFSavetoDB DB = Facade.GetInstance().getSaveToDB((DBConnectionToken)mAgrofactuurToken.Clone());
                    System.Threading.Monitor.Exit(mFilePlugins);

                    bool processed = false;
                    bool skipped = false;



                    foreach (IReaderPlugin Plugin in Plugins)
                    {
<<<<<<< HEAD
                        skipped = IsExcluded(pPath, Plugin);
                        if (skipped || Path.GetExtension(pPath) == ".fail" || Path.GetExtension(pPath) == ".old")
=======
                        skipped = IsExcluded(FileReaderItem.File, Plugin);
                        if (skipped)
>>>>>>> develop
                        {
                            continue;
                        }
                        //File.SetLastAccessTime(pPath, DateTime.Now);
                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " Start inlezen met module: " + Plugin.GetType().FullName);
                        int FileLogId = FileLogger.AddFileLog(DB, Plugin.GetType().FullName, 0, "", FileReaderItem.File, "", 0);
                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " FileLogId= " + FileLogId.ToString());


#if DEBUG
                        int exitcode = Plugin.LeesFile(-99, mAgrofactuurToken, programid, "00000", "kcD0QJC3", FileLogId, pPath);

#else
                        int exitcode = new ChildStarter().LeesFile(Plugin, FileReaderItem.ProgramId, FileReaderItem.File, mAgrofactuurToken, FileLogId);
#endif



                        /*
                        if (exitcode == 0)
                        {
                            processed = true;
                            FileLogger.FileLogAddReturnCode(DB, FileLogId, 1);
                        }
                        else FileLogger.FileLogAddReturnCode(DB, FileLogId, exitcode + 100);
                        */

                        FileLogger.FileLogAddReturnCode(DB, FileLogId, exitcode); //logResult code


                        if (exitcode == 99)
                            skipped = true;
                        if (exitcode == 1)
                            processed = true;


                        //if (Plugin.LeesFile(programid, pPath, mAgrofactuurToken))
                        //    processed = true;
                    }
                    if (!processed)
                    {
                        //wachten op andere modules
                        if (skipped) return;
                        //System.Threading.Thread.Sleep(new TimeSpan(0, 0, 30, 0));

                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " Inlezen Mislukt!");
                        ext = ".fail";

                        if (!File.Exists(FileReaderItem.File)) return;
                    }
                    else unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " Inlezen afgerond.");

                }
                else if (FileReaderItem.FileWatcherInfo.Filter == Properties.Settings.Default.Filter)
                {

                    //if (!PluginLoader.LoadPluginDLL(DB, pPath, Scheduler, mFilePlugins))
                    if (!PluginLoader.LoadPluginDLL(DB, FileReaderItem.File, mFilePlugins))
                        unLogger.WriteWarnFormat("READERPLUGIN", "[MODULEMANAGER] {0} heeft geen (geldige) plugin interfaces", FileReaderItem.File);
                    else unLogger.WriteInfoFormat("READERPLUGIN", "[MODULEMANAGER] Module {0}  is Actief.", FileReaderItem.File);

                    System.Threading.Monitor.Exit(mFilePlugins);
                    return;
                }
                else

                    System.Threading.Monitor.Exit(mFilePlugins);


                if (!File.Exists(FileReaderItem.File)) return;
                else RenameFile(FileReaderItem.File, FileReaderItem.FileWatcherInfo, ext);
            }
            catch (ThreadAbortException ex)
            {
                unLogger.WriteErrorFormat("ThreadAbortException: Nog bezig met Bestand inlezen! niet hernoemen {0}", ex.Message);
                unLogger.WriteDebug(ex.Message, ex);
            }
            catch (Exception ex)
            {

                unLogger.WriteError(ex.Message, ex);
                string ext = ".fail";


                try
                {
                    Thread.Sleep(1000);
                    if (File.Exists(FileReaderItem.File))
                    {
                        RenameFile(FileReaderItem.File, FileReaderItem.FileWatcherInfo, ext);
                    }
                }
                catch (Exception exren)
                {
                    unLogger.WriteError(exren.Message, exren);
                }
            }

        }


        private bool DetermineByHeader(String pPath, String pDB)
        {
            return false;
        }

        private bool FileSizeChanged(String pPath)
        {
            if (File.Exists(pPath))
            {
                FileInfo fi = new FileInfo(pPath);
                long size1 = fi.Length;
                Thread.Sleep(1000);
                fi.Refresh();
                long size2 = fi.Length;
                return size1 == size2;
            }
            return false;
        }

        private bool WasExcluded(String pPath, FileWatchData FWData)
        {
            List<IReaderPlugin> Plugins = mFilePlugins[FWData];
            foreach (IReaderPlugin Plugin in Plugins)
            {
                if (IsExcluded(pPath, Plugin))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsExcluded(String pPath, IReaderPlugin Plugin)
        {
            foreach (String ExcludedFilter in Plugin.getExcludeList())
            {
                if (Path.GetFileName(pPath).ToUpper().Contains(ExcludedFilter.ToUpper()))
                {
                    unLogger.WriteDebug(Path.GetFileName(pPath) + " niet ingelezen door " + Plugin.GetType().FullName + " (Bestand bevat tekens die op de exclude lijst staan)");
                    return true;
                }
                //else if (Plugin.GetFilter().ToUpperInvariant() == "*.XLSX" && pPath.ToUpper().Contains(ExcludedFilter.ToUpper()))
                //{
                //    unLogger.WriteDebug(Path.GetFileName(pPath) + " niet ingelezen door " + Plugin.GetType().FullName + " (Bestand bevat tekens die op de exclude lijst staan)");
                //    return true;
                //}
            }
            return false;
        }

        void mFilePlugins_ValueAdded(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs ValueAdded)
        {
            //if (ValueAdded.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //    foreach (object NewItem in ValueAdded.NewItems)
            //    {
            //        IReaderPlugin Plugin = (IReaderPlugin)NewItem;
            //        String PluginFilter = Plugin.GetFilter().ToUpper();
            //        FileSystemWatcher lFileWatcher = AddFileWatch(Properties.Settings.Default.Path, PluginFilter, true);
            //        StartFileWatch(lFileWatcher);
            //    }
            //}
        }

        void mFilePlugins_KeyAdded(object sender, CollectionChangeEventArgs KeyAdded)
        {
            if (KeyAdded.Action == CollectionChangeAction.Add)
            {
                String PluginFilter = KeyAdded.Element.ToString().ToUpper();
                FileSystemWatcher lFileWatcher = AddFileWatch(Properties.Settings.Default.BasePath, PluginFilter, true);
                StartFileWatch(lFileWatcher);
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool RenameFile(String pPath, FileWatchData pFWD, string ext)
        {

            bool Result = false;
            try
            {
                String lFilter;
                if (pFWD.Filter.EndsWith("*"))
                    lFilter = pFWD.Filter.Remove(pFWD.Filter.Length - 1);
                else
                    lFilter = pFWD.Filter;
                string orgext = pFWD.Filter.Replace("*", "").Replace(".", "");
                string newFilename;
                if (Path.GetExtension(pPath).ToUpper() == Path.GetExtension(lFilter).ToUpper())
                    newFilename = Path.ChangeExtension(Path.GetFileName(pPath), "_" + orgext + "_" + DateTime.Now.Ticks + ext);
                else
                    newFilename = Path.GetFileName(pPath).ToLower().Replace(Path.GetExtension(lFilter.ToLower()), String.Empty) + "_" + orgext + "_" + DateTime.Now.Ticks + ext;

                string curDir = Path.GetDirectoryName(pPath);
                string newDir;
                if (ext == ".old")
                {
                    newDir = Properties.Settings.Default.BasePath + "_history";
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Year.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Month.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Day.ToString();
                    newDir = curDir.Replace(Properties.Settings.Default.BasePath, newDir);
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                }
                else newDir = curDir;



                File.Move(pPath, newDir + Path.DirectorySeparatorChar + newFilename);
                Result = true;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
            }
            return Result;
        }

        public void ConsoleStart()
        {
            OnStart(new List<String>().ToArray());
        }

        protected override void OnStart(string[] args)
        {

<<<<<<< HEAD

=======
            OnContinue();
        }

        protected override void OnContinue()
        {
>>>>>>> develop
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                StartFileWatch(lFileWatcher);
            }
<<<<<<< HEAD
            Scheduler = new TimerJobs(mAgrofactuurToken);
        }

        protected override void OnContinue()
        {
            unLogger.WriteError("Service kan niet hervat worden, de service moet eerst gestopt worden en daarna opnieuw aangezet worden");
            throw new NotSupportedException();
=======
>>>>>>> develop
        }

        private void StartFileWatch(FileSystemWatcher lFileWatcher)
        {
            lFileWatcher.EnableRaisingEvents = true;
            if (Properties.Settings.Default.LoadExistingFiles)
            {
                String[] FileList = System.IO.Directory.GetFiles(lFileWatcher.Path, lFileWatcher.Filter, SearchOption.AllDirectories);

                ThreadStart starter = delegate { TouchFiles(FileList); };
                Thread File = new Thread(starter);
                File.Priority = ThreadPriority.Lowest;
                File.Start();
            }
        }

        private static void TouchFiles(String[] pFileList)
        {
            System.Threading.Thread.Sleep(0);

            var DateOrder = from lFilename in pFileList
                            orderby new FileInfo(lFilename).CreationTime
                            select lFilename;
            List<String> lFileList = DateOrder.ToList();

            foreach (String lFile in lFileList)
            {
                TouchFile(lFile);
                System.Threading.Thread.Sleep(1000);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void TouchFile(String lFile)
        {
            unLogger.WriteDebug(lFile);
            try
            {
                File.SetLastAccessTime(lFile, DateTime.Now);
                File.SetLastWriteTime(lFile, DateTime.Now);
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
            }
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private static void SetLastEventTrigger(String lFile)
        //{
        //    try
        //    {
        //        //(lFile, DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        unLogger.WriteError(ex.Message, ex);
        //    }
        //}


        protected override void OnPause()
        {
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                lFileWatcher.EnableRaisingEvents = false;
            }
        }


        protected override void OnPause()
        {
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                lFileWatcher.EnableRaisingEvents = false;
            }
        }

        public void ConsoleStop()
        {
            OnStop();
        }
        protected override void OnStop()
        {
            OnPause();
            System.Threading.Thread.Sleep(1000);
<<<<<<< HEAD
            mCallbacks.Clear();
=======
            //mCallbacks.Clear();
            //queue.
        }
    }


    class FileWatchData
    {

        public FileWatchData(string pFilter, string pFolder)
        {
            Filter = pFilter;
            Folder = pFolder;
        }

        public string Filter
        {
            get;
            set;
        }

        public string Folder
        {
            get;
            set;
        }


        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(FileWatchData))
            {

                return this.Filter == ((FileWatchData)obj).Filter && this.Folder == ((FileWatchData)obj).Folder;
            }
            else
                return base.Equals(obj);
>>>>>>> develop
        }


        public int GetHashCode(FileWatchData obj)
        {
            return this.Filter.GetHashCode() ^ obj.Folder.GetHashCode();
        }

    }


}
