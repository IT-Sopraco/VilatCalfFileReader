using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.COMMONS;
using System.Activities;
using System.Activities.DynamicUpdate;
using System.Configuration;

namespace VSM.RUMA.SRV.FILEREADER.CORE
{
    [Serializable]
    public class FileProcessor : MarshalByRefObject, VSM.RUMA.SRV.FILEREADER.IFileProcessor
    {

        private List<FileSystemWatcher> mFileWatchers = new List<FileSystemWatcher>();
        private MultiValueDictionary<FileWatchData, IReaderPlugin> mFilePlugins = new MultiValueDictionary<FileWatchData, IReaderPlugin>();
        private AFSavetoDB DB;
        private DBConnectionToken mAgrofactuurToken;
        private FileQueue queue = new FileQueue();
        private static readonly object uniqueLock = new object();
        private List<BackgroundWorker> Workers = new List<BackgroundWorker>();

        private bool ServiceRunning;

        static AutoResetEvent RestartWorkers = new AutoResetEvent(false);

        private String BasePath;
        public FileProcessor()
        {
            BasePath = Convert.ToString(AppDomain.CurrentDomain.GetData("InleesDir"));
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            unLogger.Configure("log4net.config", unRechten.getDBHost());
            log4net.ThreadContext.Properties["Filename"] = "main";
            unLogger.WriteInfo(String.Format("using Database: {0}", unRechten.getDBHost()));

            mAgrofactuurToken = new unServiceRechten().GetToken();
            unLogger.WriteInfo($@"mAgrofactuurToken :{mAgrofactuurToken}");
            DB = Facade.GetInstance().getSaveToDB(mAgrofactuurToken);
            mFilePlugins.KeyAdded += new MultiValueDictionaryKeyAdded(mFilePlugins_KeyAdded);

            ActivateDLLWatcher();
        }

        private void ActivateDLLWatcher()
        {
            try
            {
                FileWatchData DLLFWD = new FileWatchData("*.DLL", Win32.GetBaseDir() + "plugin\\");
                EnqueueFiles(DLLFWD, false);
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Fout bij inladen plugin's :" + ex.Message, ex);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
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
            string lPrefix = $"{nameof(FileProcessor)}.{nameof(XmlWatcher_CreatedOrChanged)} File: '{e.Name}' -";

            //unLogger.WriteInfo(e.Name + " is Aangemaakt of Aangepast");
            try
            {
                FileSystemWatcher lFileWatcher = (FileSystemWatcher)sender;
                FileWatchData lFWD = new FileWatchData(lFileWatcher.Filter, lFileWatcher.Path);
                String[] PathList = e.Name.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                
                if ((!lFileWatcher.IncludeSubdirectories) || PathList.Length >= 2)
                {
                    lock (uniqueLock)
                    {
                        FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(e.FullPath, PathList, lFWD);

                        if (File.Exists(e.FullPath))
                        {
                            if (queue.TryEnqueue(FileReaderItem))
                            {
                                unLogger.WriteInfo($"{lPrefix} Filter: {lFWD.Filter} Folder: {lFWD.Folder} Wachtrij lengte: {queue.Count} ({e.FullPath})");
                            }
                            else
                            {
                                unLogger.WriteDebug($"{lPrefix} Enqueue mislukt.{e.FullPath}");
                            }
                        }
                        else
                        {
                            unLogger.WriteDebug($"{lPrefix} Bestaat niet ({e.FullPath});");
                        }
                    }
                    WakeWorkers();
                }
                else
                {
                    unLogger.WriteDebug($@"!lFileWatcher.IncludeSubdirectories:{!lFileWatcher.IncludeSubdirectories}");
                    unLogger.WriteDebug($@"PathList.Length:{PathList.Length}");
                    unLogger.WriteDebug($"{lPrefix} Bestand overgeslagen.");                    
                }
            }
            catch (Exception ex)
            {
                string msg = $"{lPrefix} ex: {ex.Message}";
                unLogger.WriteError(msg);                                    
                unLogger.WriteDebug(msg, ex);
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
                        lock (uniqueLock)
                        {
                            FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(e.FullPath, PathList, lFWD);
                            var check = from n in queue where n.Key == FileReaderItem select n;
                            if (check.Count() == 0 && File.Exists(e.FullPath) && queue.TryEnqueue(FileReaderItem))
                            {
                                unLogger.WriteDebug(String.Format("{0} in XmlWatcher_Renamed Wachtrij voor inlezen Filter: {1} Folder: {2} Wachtrij lengte {3}", e.FullPath, lFWD.Filter, lFWD.Folder, queue.Count));
                            }
                            else
                            {
                                unLogger.WriteDebug(e.FullPath + " XmlWatcher_Renamed staat al in wachtrij om in te lezen");
                            }
                        }
                        WakeWorkers();
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
            RestartWorkers.Set();
        }



        private static byte MaxWorkers()
        {
            try
            {
                return 5;
            }
            catch
            {
                return 20;
            }
        }


        private void ProcessQueue_DoWork(object sender, DoWorkEventArgs e)
        {
            string lPrefix = $"{nameof(FileProcessor)}.{nameof(ProcessQueue_DoWork)} -";

            FileReaderQueueItem FileReaderItem;
            while (ServiceRunning)
            {
                try
                {
                    unLogger.WriteInfo($@"queue.Count():{queue.Count()} MaxWorkers():{MaxWorkers()}");
                    if (queue.Count() < MaxWorkers())
                    {
                        if (ConfigurationManager.AppSettings["FilereaderWaitFolder"] != null)
                        {
                            ThreadStart starter = delegate {
                                try
                                {
                                    DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["FilereaderWaitFolder"]);
                                    var files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                                    if (files.Count() > 0)
                                    {
                                        string extension = Path.GetExtension(files[0].FullName).Replace(".","");
                                        string destination =  Path.Combine(BasePath, files[0].Name);
                                        if (ConfigurationManager.AppSettings[$@"extensionfolder_{extension}"] != null)
                                        {
                                            destination = Path.Combine(BasePath, ConfigurationManager.AppSettings[$@"extensionfolder_{extension}"], files[0].Name);
                                            unLogger.WriteInfo($@"Move:{files[0].FullName}, {destination}");
                                            File.Move(files[0].FullName, destination);
                                            Thread.Sleep(2000);
                                        }
                                      
                                    }
                                }
                                catch(Exception exc) 
                                {
                                    unLogger.WriteError($@"FilereaderWaitFolder: {exc.ToString()}");
                                }
                            };
                            Thread parathread = new Thread(starter);
                            parathread.Name = "FilereaderWaitFolder" + parathread.ManagedThreadId + Thread.CurrentThread.Name;
                            parathread.Start();
                        }
                    }
                    if (queue.TryDequeue(out FileReaderItem))
                    {
                        if (File.Exists(FileReaderItem.File))
                        {
                            Inlezen(FileReaderItem);
                        }
                        else
                        {
                            unLogger.WriteWarn($"{lPrefix} File: {Path.GetFileName(FileReaderItem.File)} is verwijderd of verplaatst(was een TMP bestand?) inlezen afbreken.");
                        }
                    }
                    else RestartWorkers.WaitOne();
                }
                catch (Exception ex)
                {
                    string msg = $"{lPrefix} Ex: {ex.Message}";
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);
                }
            }
        }


        void Inlezen(FileReaderQueueItem FileReaderItem, bool retry = false)
        {
            string lPrefix = $"{nameof(FileProcessor)}.{nameof(Inlezen)} -";

            try
            {
                string fileName = Path.GetFileName(FileReaderItem.File);
                lPrefix = $"{nameof(FileProcessor)}.{nameof(Inlezen)} Bestand: {Path.GetFileName(fileName)} -";

                log4net.ThreadContext.Properties["Filename"] = fileName;
                string ext = ".old";

                //unLogger.WriteDebug("programid :" + FileReaderItem.ProgramId);

                System.Threading.Monitor.Enter(mFilePlugins);

                unLogger.WriteDebug($"{lPrefix} Begonnen.");

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
                            unLogger.WriteDebug($" skipped. IsExcluded {lPrefix}");
                            continue;
                        }
                        unLogger.WriteDebug($@"getFileLogByHostnameAndFileName('{System.Net.Dns.GetHostName()}', '{Path.GetFileName(FileReaderItem.File)}')");
                        var files = DB.getFileLogsByHostnameAndFileName(System.Net.Dns.GetHostName(), Path.GetFileName(FileReaderItem.File));
                        int minutes = -55;
                        if (ConfigurationManager.AppSettings["lastinfilelogminutes"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["lastinfilelogminutes"]))
                        {
                            if (!int.TryParse(ConfigurationManager.AppSettings["lastinfilelogminutes"].ToString(), out minutes))
                            {
                                minutes = -55;
                            }
                            else
                            {
                                minutes = minutes > 0 ? minutes * -1 : minutes;
                            }
                        }

                        if (files != null && files.Count()>0)
                        {
                            var check = from n in files where n.Datetime > DateTime.Now.AddMinutes(minutes)  select n;
                            if(check.Count() > 0)
                            {
                                unLogger.WriteError($"skipped. because already in filelog last {minutes*-1} minutes :{FileReaderItem.File} (set settings.config: lastinfilelogminutes)");
                                continue;
                            }
                        }
                        int fileLogId = FileLogger.AddFileLog(DB, Plugin.GetType().FullName, 0, "", FileReaderItem.File, "", 0);
                        unLogger.WriteDebug($"{lPrefix} FileLogId: {fileLogId} Module: {Plugin.GetType().FullName}");


                        #if DEBUG
                            int exitcode = Plugin.LeesFile(-99, mAgrofactuurToken, FileReaderItem.ProgramId, "00000", "kcD0QJC3", fileLogId, FileReaderItem.File);
                        #else
                            int exitcode = new ChildStarter().LeesFile(Plugin, FileReaderItem.ProgramId, FileReaderItem.File, mAgrofactuurToken, fileLogId);
                        #endif
                        unLogger.WriteDebug($@"{lPrefix} exitcode:{exitcode}");
                        FileLogger.FileLogAddReturnCode(DB, fileLogId, exitcode); //logResult code

                        if (exitcode == 1)
                            processed = true;
                    }

                    unLogger.WriteDebug($@"{lPrefix} processed:{processed}");
                    if (!processed)
                    {
                        //wachten op andere modules files met algemene filter niet op fail zetten.
                        if (skipped || FileReaderItem.FileWatcherInfo.Filter == "*.*")
                            return;

                        if (!File.Exists(FileReaderItem.File))
                        {
                            unLogger.WriteDebug($"{lPrefix} {FileReaderItem.File} Bestaat niet.");
                            return;
                        }

                        else if (retry == false)
                        {
                            unLogger.WriteDebug($"{lPrefix} Retry");
                            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 0, 10));
                            Inlezen(FileReaderItem, true);
                            return;
                        }
                        unLogger.WriteError($"{lPrefix} Inlezen mislukt. Filter: {FileReaderItem.FileWatcherInfo.Filter}");
                        ext = ".fail";
                    }
                    else
                    {
                        unLogger.WriteInfo($"{lPrefix} Ingelezen.");
                    }

                }
                else if (FileReaderItem.FileWatcherInfo.Filter.ToUpperInvariant() == "*.DLL")
                {
                    if (!PluginLoader.LoadPlugins(DB, FileReaderItem.File, mFilePlugins))
                        unLogger.WriteError($"{lPrefix} [MODULEMANAGER] {fileName} heeft geen (geldige) plugin interfaces.");
                    else
                        unLogger.WriteInfo($"{lPrefix} [MODULEMANAGER] Module {FileReaderItem.File} is Actief.");

                    System.Threading.Monitor.Exit(mFilePlugins);
                    return;
                }
                else
                {
                    unLogger.WriteDebug($@"{lPrefix} Monitor.Exit");
                    System.Threading.Monitor.Exit(mFilePlugins);
                }
            

                RenameFile(FileReaderItem.File, FileReaderItem.FileWatcherInfo, ext);


            }
            catch (ThreadAbortException ex)
            {
                string msg = $"{lPrefix} ThreadAbortException: Nog bezig met Bestand inlezen! niet hernoemen {ex.Message}";

                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
            }
            catch (Exception ex)
            {
                string msg = $"{lPrefix} Fout bij inlezen bestand {FileReaderItem.File} Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);

                string ext = ".fail";
                try
                {
                    Thread.Sleep(1000);
                    unLogger.WriteDebug($@"{lPrefix} {FileReaderItem.File} File.exists:{File.Exists(FileReaderItem.File)} RenameFile ");
                    if (File.Exists(FileReaderItem.File))
                        RenameFile(FileReaderItem.File, FileReaderItem.FileWatcherInfo, ext);

                }
                catch (Exception exren)
                {
                    string msg2 = $"{lPrefix} Ex InnerEx: {exren.Message}";
                    unLogger.WriteError(msg2);
                    unLogger.WriteDebug(msg2, exren);
                }
            }
            finally
            {
                unLogger.WriteDebug($"{lPrefix} Klaar.");
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
                Thread.Sleep(10);
                fi.Refresh();
                if (File.Exists(pPath))
                {
                    long size2 = fi.Length;
                    return size1 == size2;
                }
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
            }
            return false;
        }

        void mFilePlugins_KeyAdded(object sender, CollectionChangeEventArgs KeyAdded)
        {
            if (KeyAdded.Action == CollectionChangeAction.Add)
            {
                bool SubDirectories = true;
                FileWatchData FWD = KeyAdded.Element as FileWatchData;
                FileSystemWatcher lFileWatcher = AddFileWatch(FWD.Folder, FWD.Filter, SubDirectories);
                if (lFileWatcher != null) { StartFileWatch(lFileWatcher, SubDirectories); }
            }
        }

        private bool RenameFile(String pPath, FileWatchData pFWD, string ext)
        {
            bool Result = false;

            string lPrefix = $"{nameof(FileProcessor)}.{nameof(RenameFile)} Path: {pPath} Ext: {ext} -";

            try
            {
                string lFilter;
                if (pFWD.Filter.EndsWith("*"))
                    lFilter = pFWD.Filter.Remove(pFWD.Filter.Length - 1);
                else
                    lFilter = pFWD.Filter;

                string orgext = pFWD.Filter.Replace("*", "").Replace(".", "");
                string newFilename;


                string fileName = Path.GetFileName(pPath);
                string filterExtUpper = Path.GetExtension(lFilter).ToUpperInvariant();
                string pathExtUpper = Path.GetExtension(pPath).ToUpperInvariant();

                if (pathExtUpper == filterExtUpper)
                {
                    newFilename = Path.ChangeExtension(fileName, "_" + orgext + "_" + DateTime.Now.Ticks + ext);
                }
                else if (lFilter != "")
                {
                    newFilename = fileName.ToLower().Replace(filterExtUpper.ToLower(), "") + "_" + orgext + "_" + DateTime.Now.Ticks + ext;
                }
                else
                {
                    newFilename = Path.ChangeExtension(fileName, "_" + Path.GetExtension(pPath) + "_" + DateTime.Now.Ticks + ext);
                }

                string curDir = Path.GetDirectoryName(pPath);
                string newDir;
                if (ext == ".old")
                {
                    newDir = Win32.GetBaseDir() + "Files_history";
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Year.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Month.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir += DateTime.Now.Day.ToString();
                    newDir += Path.DirectorySeparatorChar;
                    newDir = curDir.Replace(BasePath, newDir);
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                }
                else
                    newDir = curDir;


                string target = Path.Combine(newDir, newFilename);
                unLogger.WriteDebug($"{lPrefix} verplaatsen naar: {target}.");
                try
                {
                    if (ConfigurationManager.AppSettings["sleepbeforemovemilliseconds"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["sleepbeforemovemilliseconds"]))
                    {
                        int sleep = 0;
                        if (int.TryParse(ConfigurationManager.AppSettings["sleepbeforemovemilliseconds"], out sleep))
                        {
                            unLogger.WriteDebug($"Een klein moment...({sleep} millisec)");
                            Thread.Sleep(sleep);
                        }
                    }
                }
                catch { }
                File.Move(pPath, target);
                Result = true;
            }
            catch (Exception ex)
            {
                string msg = $"{lPrefix} Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
            }
            return Result;
        }

        private void StartFileWatch(FileSystemWatcher pFileWatcher, bool SubDirectories)
        {
            FileWatchData lFWD = new FileWatchData(pFileWatcher.Filter, pFileWatcher.Path);
            ThreadStart starter = delegate { EnqueueFiles(lFWD, SubDirectories); };
            Thread File = new Thread(starter);
            File.Priority = ThreadPriority.Lowest;
            File.Start();
            pFileWatcher.EnableRaisingEvents = true;
        }

        private void EnqueueFiles(FileWatchData lFWD, bool SubDirectories)
        {
            SearchOption subdirs = SearchOption.TopDirectoryOnly;
            if (SubDirectories)
                subdirs = SearchOption.AllDirectories;

            String[] FileList = System.IO.Directory.GetFiles(lFWD.Folder, lFWD.Filter, subdirs);
            var DateOrder = from lFilename in FileList
                            orderby new FileInfo(lFilename).CreationTime
                            select lFilename;
            List<String> lFileList = DateOrder.ToList();
            //ths is for the plugin DLL's
            foreach (String lFile in lFileList)
            {
                String[] PathList = lFile.Replace(BasePath, "").Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                lock (uniqueLock)
                {
                    FileReaderQueueItem FileReaderItem = new FileReaderQueueItem(lFile, PathList, lFWD);
                    var check = from n in queue where n.Key == FileReaderItem select n;
                    if (check.Count() == 0 && File.Exists(lFile) && queue.TryEnqueue(FileReaderItem))
                    {
                        unLogger.WriteDebug(String.Format("{0} EnqueueFiles in Wachtrij voor inlezen Filter: {1} Folder: {2}", lFile, lFWD.Filter, lFWD.Folder));
                    }
                    else
                    {
                        unLogger.WriteDebug(lFile + " EnqueueFiles staat al in wachtrij om in te lezen");
                    }
                }
                WakeWorkers();
            }
        }

        public void OnContinue()
        {
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                StartFileWatch(lFileWatcher, lFileWatcher.IncludeSubdirectories);
            }
        }

        public void OnStart()
        {
            ServiceRunning = true;
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                StartFileWatch(lFileWatcher, lFileWatcher.IncludeSubdirectories);
            }
            for (int i = 0; i < MaxWorkers(); i++)
            {
                BackgroundWorker LoadBW = new BackgroundWorker();
                LoadBW.DoWork += ProcessQueue_DoWork;
                Workers.Add(LoadBW);
                LoadBW.RunWorkerAsync();
            }
        }

        public void OnStop()
        {
            OnPause();
            ServiceRunning = false;
            RestartWorkers.Set();
        }

        public void OnPause()
        {
            foreach (FileSystemWatcher lFileWatcher in mFileWatchers)
            {
                lFileWatcher.EnableRaisingEvents = false;
            }
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
