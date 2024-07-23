using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.COMMONS;

namespace VSM.RUMA.SRV.FILEREADER.CORE
{
    partial class FileProcessService : ServiceBase
    {
        private List<FileSystemWatcher> mFileWatchers = new List<FileSystemWatcher>();
        private MultiValueDictionary<FileWatchData, IReaderPlugin> mFilePlugins = new MultiValueDictionary<FileWatchData, IReaderPlugin>();
        private AFSavetoDB DB;
        private DBConnectionToken mAgrofactuurToken;
        private ConcurrentQueue<FileReaderQueueItem> queue = new ConcurrentQueue<FileReaderQueueItem>();
        //private BlockingCollection<FileReaderQueueItem> queue = new BlockingCollection<FileReaderQueueItem>();
        private static readonly object padlock = new object();

        private TaskFactory factory = new TaskFactory();
        private List<Task> Workers = new List<Task>();
        private bool ServiceRunning;


        static AutoResetEvent RestartWorkers = new AutoResetEvent(false);

        private String BasePath;
        public FileProcessService()
        {
            InitializeComponent();
            BasePath = Convert.ToString(AppDomain.CurrentDomain.GetData("InleesDir"));
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            unLogger.Configure("log4net.config", unRechten.getDBHost());
            log4net.ThreadContext.Properties["Filename"] = "main";
            unLogger.WriteInfo(String.Format("using Database: {0}", unRechten.getDBHost()));
            AddFileWatch(Win32.GetBaseDir() + "plugin\\", "*.DLL", false);
            mAgrofactuurToken = Facade.GetInstance().getRechten().Inloggen("00000", Facade.GetInstance().getRechten().base64Encode("kcD0QJC3"));
            DB = new VSM.RUMA.CORE.DB.MYSQL.MySQLSavetoDB(mAgrofactuurToken);
            mFilePlugins.ValueAdded += new MultiValueDictionaryChanged(mFilePlugins_ValueAdded);
            mFilePlugins.KeyAdded += new MultiValueDictionaryKeyAdded(mFilePlugins_KeyAdded);
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

                if ((!lFileWatcher.IncludeSubdirectories) || PathList.Length >= 2)
                {
                    lock (padlock)
                    {
                        FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(e.FullPath, PathList, lFWD);

                        if (!queue.Contains(FileReaderItem) && File.Exists(e.FullPath))
                        {
                            unLogger.WriteDebug(e.FullPath + " in Wachtrij voor inlezen");
                            queue.Enqueue(FileReaderItem);
                            WakeWorkers();
                        }
                        else
                        {
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

                                queue.Enqueue(FileReaderItem);
                                WakeWorkers();
                            }
                            else
                            {
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

        private void WakeWorkers()
        {
            int queueCount = queue.Count;
            if (queueCount < Environment.ProcessorCount)
            {
                RestartWorkers.Set();
            }
            else if (queueCount > 100 && Workers.Count < MaxWorkers())
            {
                Task HiLoadTask = factory.StartNew(ProcessQueue, TaskCreationOptions.PreferFairness);
                Workers.Add(HiLoadTask);
            }
            else if (queueCount > 100)
            {
                foreach (var IdleTask in Workers.Where(t => t.IsCompleted))
                {
                    IdleTask.Start();
                }
            }
        }

        private static int MaxWorkers()
        {
            return 20;
        }


        private void ProcessQueue()
        {
            FileReaderQueueItem FileReaderItem;
            while (ServiceRunning)
            {
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
                            while (!queue.TryDequeue(out FileReaderItem))
                                RestartWorkers.WaitOne();
                        }
                    }
                    Inlezen(FileReaderItem);
                }
                else if (Workers.Where(t => !t.IsCompleted).Count() > Environment.ProcessorCount)
                {
                    break;
                }
                else RestartWorkers.WaitOne();
                //else
                //{
                //    Monitor.
                //    Monitor.Wait()
                //}
            }
        }
        void Inlezen(FileReaderQueueItem FileReaderItem)
        {
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
                        skipped = IsExcluded(FileReaderItem.File, Plugin);
                        if (skipped)
                        {
                            continue;
                        }
                        //File.SetLastAccessTime(pPath, DateTime.Now);
                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " Start inlezen met module: " + Plugin.GetType().FullName);
                        int FileLogId = FileLogger.AddFileLog(DB, Plugin.GetType().FullName, 0, "", FileReaderItem.File, "", 0);
                        unLogger.WriteDebug(Path.GetFileName(FileReaderItem.File) + " FileLogId= " + FileLogId.ToString());


#if DEBUG
                        int exitcode = Plugin.LeesFile(-99, mAgrofactuurToken, FileReaderItem.ProgramId, "00000", "kcD0QJC3", FileLogId, FileReaderItem.File);

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
                else if (FileReaderItem.FileWatcherInfo.Filter == "*.DLL")
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
                FileWatchData FWD = KeyAdded.Element as FileWatchData;
                FileSystemWatcher lFileWatcher = AddFileWatch(FWD.Folder, FWD.Filter, true);
                StartFileWatch(lFileWatcher);
            }
        }

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
                    newDir = BasePath + "_history";
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Year.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Month.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Day.ToString();
                    newDir = curDir.Replace(BasePath, newDir);
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

        private void StartFileWatch(FileSystemWatcher pFileWatcher)
        {
            String[] FileList = System.IO.Directory.GetFiles(pFileWatcher.Path, pFileWatcher.Filter, SearchOption.AllDirectories);

            FileWatchData lFWD = new FileWatchData(pFileWatcher.Filter, pFileWatcher.Path);
            ThreadStart starter = delegate { EnqueueFiles(lFWD, FileList); };
            Thread File = new Thread(starter);
            File.Priority = ThreadPriority.Lowest;
            File.Start();
            pFileWatcher.EnableRaisingEvents = true;
        }

        private void EnqueueFiles(FileWatchData lFWD, String[] pFileList)
        {
            var DateOrder = from lFilename in pFileList
                            orderby new FileInfo(lFilename).CreationTime
                            select lFilename;
            List<String> lFileList = DateOrder.ToList();

            foreach (String lFile in lFileList)
            {
                String[] PathList = lFile.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                lock (padlock)
                {
                    FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(lFile, PathList, lFWD);
                    if (!queue.Contains(FileReaderItem) && File.Exists(lFile))
                    {
                        unLogger.WriteDebug(lFile + " in Wachtrij voor inlezen");
                        queue.Enqueue(FileReaderItem);
                        WakeWorkers();
                    }
                    else
                    {
                        unLogger.WriteDebug(lFile + " staat al in wachtrij om in te lezen");
                    }
                }

            }
        }

        protected override void OnContinue()
        {
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                lFileWatcher.EnableRaisingEvents = true;
            }
        }

        protected override void OnStart(string[] args)
        {
            ServiceRunning = true;
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                StartFileWatch(lFileWatcher);
            }

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Workers.Add(factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning));
            }
        }

        protected override void OnStop()
        {
            ServiceRunning = false;
            RestartWorkers.Set();
            Task.WaitAll(Workers.ToArray(), new CancellationToken(true));
        }
        protected override void OnPause()
        {
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                lFileWatcher.EnableRaisingEvents = false;
            }
            RestartWorkers.Set();
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
        }


        public override int GetHashCode()
        {
            return this.Filter.GetHashCode() ^ this.Folder.GetHashCode();
        }

    }
}
