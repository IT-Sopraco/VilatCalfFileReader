using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.EDINRS;

namespace VSM.RUMA.CORE
{

    public delegate void ProgressUpdateEvent(Facade sender, int progress, string message);

    [Guid("7A531511-0282-4076-9B17-1EB4AD217DF5"),
    ClassInterface(ClassInterfaceType.AutoDual)]
    public class Facade : MarshalByRefObject, IFacade
    {
        Facade()
        {
            unLogger.Configure("log4net.config", unRechten.getDBHost());

            unLogger.WriteInfo("###################################################################################################################################################");
            unLogger.WriteInfo("Current host:      " + System.Net.Dns.GetHostName());
            unLogger.WriteInfo("Current Date/Time: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            unLogger.WriteInfo("");
            unLogger.WriteInfo("Executable name:   " + System.AppDomain.CurrentDomain.FriendlyName); 
            unLogger.WriteInfo("Current Directory: " + System.IO.Directory.GetCurrentDirectory());
            unLogger.WriteInfo("###################################################################################################################################################");
        }


        public void LogConnections()
        {
            unLogger.WriteInfo("###################################################################################################################################################");
            unLogger.WriteInfo("Database Connections:");
            foreach (var kvp in SlaveToDBs)
            {
                var connInf = SaveToDB.GetConnectionInfo(kvp.Key);
                unLogger.WriteInfo($"User: {connInf.User} Host: {connInf.Host} Pooling: {connInf.Pooling} MinPoolSize: {connInf.MinPoolSize} MaxPoolSize: {connInf.MaxPoolSize}");
            }
            unLogger.WriteInfo("###################################################################################################################################################");
        }

        public VSMMysqlConnectionInfo LogConnection(DBConnectionToken token)
        {
            var connInf = getSaveToDB(token).GetConnectionInfo(token);
            unLogger.WriteInfo($@"Database Connection - User: {connInf.User} Host: {connInf.Host} Pooling: {connInf.Pooling} MinPoolSize: {connInf.MinPoolSize} MaxPoolSize: {connInf.MaxPoolSize}");
            return connInf;
        }

        ~Facade()
        {
            unLogger.WriteInfo("Process End.. Current Directory : " + System.IO.Directory.GetCurrentDirectory());
            unLogger.FlushBuffers();
        }


        public void UpdateProgress(int progress, string message)
        {
            if (Application_Update != null) { Application_Update(this, progress, message); }
            System.Threading.Thread.Sleep(0);
        }

        //internal static IDatabase GetDataBase()
        //{
        //    Facade instance = null;
        //    if (instance == null)
        //    {
        //        instance = GetInstance();
        //    }
        //    if (instance.SaveToDB == null)
        //    {
        //        unLogger.WriteInfo("GetDataBase");
        //        switch (VSM.RUMA.CORE.Properties.Settings.Default.DatabaseType.ToUpper())
        //        {
        //            case "MYSQL":
        //                instance.SaveToDB = new MySQLSavetoDB();
        //                break;
        //            case "SQLITE":
        //                //instance.SaveToDB = new SQLiteSavetoDB();
        //                break;
        //            default:
        //                instance.SaveToDB = new MySQLSavetoDB();
        //                break;
        //        }
        //        if (instance.SaveToDB.Plugin() != "T4C")
        //        {
        //            switch (PluginLoader.LoadPlugin("VSM.RUMA.ASP.T4CPLUGIN.T4CPluginStarter", "plugin\\VSM.RUMA.ASP.T4CPLUGIN.dll", instance))
        //            {
        //                case 0:
        //                    //instance.SaveToDB.WriteError("VSM.RUMA.ASP.T4CPLUGIN.dll loaded", 0, 0);
        //                    unLogger.WriteDebug("VSM.RUMA.ASP.T4CPLUGIN.dll loaded");
        //                    break;
        //                case 1:
        //                    //instance.SaveToDB.WriteError("VSM.RUMA.ASP.T4CPLUGIN.dll not loaded", 0, 0);
        //                    unLogger.WriteError("VSM.RUMA.ASP.T4CPLUGIN.dll not loaded");
        //                    break;
        //                case 2:
        //                    instance.SaveToDB = null;
        //                    throw new Exception("T4C plugin API not online");
        //                //break;
        //            }

        //        }

        //    }
        //    return instance.SaveToDB.GetDataBase();
        //}

        private static Facade instance;

        private static readonly object padlock = new object();
        private static readonly object dblock = new object();

        public static Facade GetInstance()
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new Facade();
                }
                return instance;
            }
        }

        public static void LogError(String Message, Exception ex)
        {
            unLogger.WriteError(Message, ex);
        }

        public static void LogObject(object pObject)
        {
            unLogger.WriteObject(pObject);
        }

        public event DomainEventHandler Application_Loading;

        public event ProgressUpdateEvent Application_Update;

        public void LoadApplication(DBConnectionToken pToken, int FarmId)
        {
            /*
            XML2AGROBASE.unXMLReader reader = new XML2AGROBASE.unXMLReader(this);
            List<String> lFileList = SortFilebyDate(unDatacomReader.GetFilesDatacomIn("XML"));
            foreach (String lFilename in lFileList)
            {
                if (!reader.LeesBestand(lFilename))
                {
                    unLogger.WriteError(lFilename + " Import Mislukt!");
                }
                else
                {
                    unDatacomReader.RenameFileDatacomIn(lFilename, "PDAxml");
                }
            }
            */
            unLogger.WriteInfo("LoadApplication");
            if (Application_Loading != null) Application_Loading(this, pToken, FarmId);
        }

        public void GetReadEdiDap(string pBestand, int pUBNId, int pProgId, int pProgramID)
        {
            //getedidap(pFarmId, pProgId);
            //ReadEdiDap(pBestand, pUBNId, pProgId, pProgramID);

        }

        private void ReadEdiDap(string pBestand, int pUBNId, int pProgId, int pProgramID)
        {
            //this.UpdateProgress(5, "Bestanden Inlezen");
            ////List<String> lFileList = unDatacomReader.GetFilesDatacomIn("DAP");
            ////foreach (String lFilename in lFileList)
            ////{
            //if (!this.getEDIDAP().LeesBestand(pBestand, pUBNId, pProgId, pProgramID))
            //{

            //    //this.getSaveToDB().WriteError(pBestand + " Import Failed!", pUBNId, pProgId);
            //    unLogger.WriteError(pBestand + " Import Mislukt!");
            //}
            //else
            //{
            //    //unDatacomReader.RenameFileDatacomIn(lFilename);
            //}
            ////}

            ////inlezen
        }

        private List<String> SortFilebyDate(List<String> pFileList)
        {
            var DateOrder = from lFilename in pFileList
                            orderby lFilename
                            select lFilename;
            List<String> lFileList = DateOrder.ToList();
            return lFileList;
        }

        public List<String> SortFilebyAdisHeader(List<String> pFileList, DBConnectionToken pToken)
        {
            var DateOrder = from lFilename in pFileList
                            orderby this.getEDINRS(pToken).LeesHeader(lFilename)
                            select lFilename;
            List<String> lFileList = DateOrder.ToList();
            return lFileList;
        }

        //LEGACY
        private AFSavetoDB SaveToDB;

        [Obsolete("GetSaveToDB voortaan aanroepen met DB Token",true)]
        public AFSavetoDB getSaveToDB()
        {
            lock (dblock)
            {
                if (SaveToDB == null)
                {
                    switch (VSM.RUMA.CORE.Properties.Settings.Default.DatabaseType.ToUpper())
                    {
                        case "MYSQL":
                            instance.SaveToDB = new VSM.RUMA.CORE.DB.MYSQL.MySQLSavetoDB();
                            break;
                        case "SQLITE":
                            //instance.SaveToDB = new SQLiteSavetoDB();
                            break;
                        default:
                            instance.SaveToDB = new VSM.RUMA.CORE.DB.MYSQL.MySQLSavetoDB();
                            break;
                    }
                }
                return SaveToDB;
            }
        }


        private Dictionary<DBConnectionToken, AFSavetoDB> SaveToDBs = new Dictionary<DBConnectionToken, AFSavetoDB>();
        private Dictionary<DBConnectionToken, VSM.RUMA.CORE.DB.MYSQL.DBSelectQueries> SlaveToDBs = new Dictionary<DBConnectionToken, VSM.RUMA.CORE.DB.MYSQL.DBSelectQueries>();
        private int initresult = 1;

        public AFSavetoDB getMaster(DBConnectionToken pToken)
        {
            return getSaveToDB(pToken);
        }

        public VSM.RUMA.CORE.DB.MYSQL.DBSelectQueries getSlave(DBConnectionToken pToken)
        {
            lock (dblock)
            {
                if (!TokenInDictionary(SlaveToDBs, pToken))
                {
                    SlaveToDBs.Add(pToken, new VSM.RUMA.CORE.DB.MYSQL.DBSelectQueries(pToken));
                }
                return SlaveToDBs[pToken];
            }
        }

        public AFSavetoDB getSaveToDB(DBConnectionToken pToken)
        {
            lock (dblock)
            {
                if (!TokenInDictionary(SaveToDBs, pToken))
                {
                    SaveToDBs.Add(pToken, new DBMasterQueries(pToken));
                    if (initresult != 1)
                    {
                        switch (PluginLoader.LoadPlugin("VSM.RUMA.ASP.T4CPLUGIN.T4CPluginStarter", "plugin\\VSM.RUMA.ASP.T4CPLUGIN.dll", instance, pToken))
                        {
                            case 0:
                                unLogger.WriteDebug("VSM.RUMA.ASP.T4CPLUGIN.dll loaded");
                                break;
                            case 1:
                                unLogger.WriteError("VSM.RUMA.ASP.T4CPLUGIN.dll not loaded");
                                break;
                            case 2:
                                SaveToDBs.Remove(pToken);
                                throw new Exception("T4C plugin API not online");
                        }
                    }
                }
                return SaveToDBs[pToken];
            }
        }


        private bool TokenInDictionary<T>(Dictionary<DBConnectionToken, T> pDictionary, DBConnectionToken pToken)
        {
            try
            {
                if (pDictionary == null || pToken == null)
                    return false;
                return pDictionary.ContainsKey(pToken);
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Facade TokenInSaveToDBs", ex);
                return false;
            }
        }

        public void setSaveToDB(AFSavetoDB value, DBConnectionToken pToken)
        {
            lock (dblock)
            {
                if (value == null)
                {
                    if (pToken != null)
                    {
                        if (TokenInDictionary(SaveToDBs,pToken)) SaveToDBs.Remove(pToken);
                        else unLogger.WriteInfo("TOKEN", "Token not in SaveToDBs!");
                    }
                    else unLogger.WriteInfo("TOKEN", "SaveToDBs.Remove : Token is null! ");
                }
                else
                {
                    //LEGACY
                    SaveToDB = value;
                    SaveToDBs[pToken] = value;
                }
            }
        }



        public bool getInlog(ref String pUsername, ref String pPassword)
        {
            initresult = PluginLoader.InitPlugin("VSM.RUMA.ASP.T4CPLUGIN.T4CPluginStarter", "plugin\\VSM.RUMA.ASP.T4CPLUGIN.dll", ref pUsername, ref pPassword);
            if (initresult != 1)
            {
                if (pUsername.Equals(String.Empty) || pPassword.Equals(String.Empty))
                {
                    return false;
                }

                return true;
            }
            return false;

        }


        private AFIenRMeldingen Meldingen = new RUMAIenRMeldingen();

        public AFIenRMeldingen getMeldingen()
        {
            return Meldingen;
        }

        public void setMeldingen(AFIenRMeldingen value)
        {
            Meldingen = value;
        }

        private AFcomEDINRS mEDINRS;

        public AFcomEDINRS getEDINRS(DBConnectionToken pToken)
        {
            if (mEDINRS == null) mEDINRS = new RUMAcomEDINRS(this, pToken,0);
            return mEDINRS;
        }

        public void setEDINRS(AFcomEDINRS value)
        {
            mEDINRS = value;
        }


        private unRechten Rechten = new unRechten();

        public unRechten getRechten()
        {
            return Rechten;
        }

        public string Version()
        {
            System.Reflection.Assembly AssemblyInfo = this.GetType().Assembly;
            String LoaderTitle = VSM_Ruma_OnlineCulture.getStaticResource("versie", "Versie") + ": " + AssemblyInfo.GetName().Version.ToString();
            return LoaderTitle;
        }

        public Version getVersion()
        {
            System.Reflection.Assembly AssemblyInfo = this.GetType().Assembly;
            return AssemblyInfo.GetName().Version;
        }


        public void Update()
        {

        }

        public void setDatacomPath(String Path)
        {
            unDatacomReader.DatacomPath = Path;
        }

        public string getDatacomPath()
        {
            return unDatacomReader.DatacomPath;
        }

        private SendReceive SendReceive = new SendReceive();

        public SendReceive getSendReceive()
        {
            return SendReceive;
        }

        ISendReceive IFacade.getSendReceive()
        {
            return SendReceive;
        }
    }
}
