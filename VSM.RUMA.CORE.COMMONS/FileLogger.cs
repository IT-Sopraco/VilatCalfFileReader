using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE.COMMONS
{
    public class FileLogger
    {

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int AddFileLog(AFSavetoDB DB, int errorcount, string Errormemo, string Filepath, string Logfilepath, int returncode)
        {
            try
            {
                unLogger.WriteDebug(String.Format("AddFileLog Reader:{0}  Filename: {1}",System.Reflection.Assembly.GetCallingAssembly().GetName().Name,Filepath));

                FILELOG_READER FlR = DB.getFileLogReaderByDLLName(System.Reflection.Assembly.GetCallingAssembly().GetName().Name);
                string Filehost = System.Net.Dns.GetHostName();
                return AddFileLog(DB, FlR.Filelog_reader_id, errorcount, Errormemo, Filehost, Filepath, Logfilepath, returncode);
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(System.Reflection.Assembly.GetCallingAssembly().GetName().Name);
                unLogger.WriteDebug(Filepath);
                unLogger.WriteError(ex.Message, ex);
                return 0;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int AddFileLog(AFSavetoDB DB, string readername, int errorcount, string Errormemo, string Filepath, string Logfilepath, int returncode)
        {
            try
            {
                unLogger.WriteDebug(String.Format("AddFileLog Reader:{0}  Filename: {1}", readername, Filepath));
                FILELOG_READER FlR = DB.getFileLogReaderByDLLName(readername);
                string Filehost = System.Net.Dns.GetHostName();
                unLogger.WriteDebug("Host : " + Filehost);
                return AddFileLog(DB, FlR.Filelog_reader_id, errorcount, Errormemo, Filehost, Filepath, Logfilepath, returncode);
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(readername);
                unLogger.WriteDebug(Filepath);
                unLogger.WriteError(ex.Message, ex);
                return 0;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int AddFileLog(AFSavetoDB DB, int filelog_reader_id, int errorcount, string Errormemo, string Filehost, string Filepath, string Logfilepath, int returncode)
        {
            try
            {
                FILELOG fl = new FILELOG();
                fl.Datetime = DateTime.Now;
                fl.Filelog_reader_id = filelog_reader_id;
                fl.Error = errorcount;
                fl.Errormemo = Errormemo;
                fl.Filehost = Filehost;
                fl.Filepath = Filepath;
                fl.Logfilepath = Logfilepath;
                fl.Retcode = returncode;
                int result = DB.saveFileLog(fl);
                return fl.Filelog_id;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return 0;
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int UpdateFileLog(AFSavetoDB DB, int filelog_id, int filelog_reader_id, int errorcount, string Errormemo, string Filehost, string Filepath, string Logfilepath, int returncode)
        {
            try
            {
                FILELOG fl = DB.getFileLogById(filelog_id);
                fl.Datetime = DateTime.Now;
                fl.Filelog_id = filelog_id;
                fl.Filelog_reader_id = filelog_reader_id;
                fl.Error = errorcount;
                fl.Errormemo = Errormemo;
                fl.Filehost = Filehost;
                fl.Filepath = Filepath;
                fl.Logfilepath = Logfilepath;
                fl.Retcode = returncode;
                int result = DB.saveFileLog(fl);
                return fl.Filelog_id;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return 0;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int FileLogAddErrorMessage(AFSavetoDB DB, int filelog_id, string Errormemo)
        {
            try
            {
                FILELOG fl = DB.getFileLogById(filelog_id);
                if (filelog_id > 0)
                {
                    fl.Datetime = DateTime.Now;
                    fl.Filelog_id = filelog_id;
                    fl.Error = fl.Error++;
                    fl.Errormemo += ";" + Errormemo;
                    int result = DB.saveFileLog(fl);
                    return fl.Filelog_id;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return 0;
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int FileLogAddReturnCode(AFSavetoDB DB, int filelog_id, int returncode)
        {
            try
            {
                FILELOG fl = DB.getFileLogById(filelog_id);
                if (filelog_id > 0)
                {
                    fl.Datetime = DateTime.Now;
                    fl.Filelog_id = filelog_id;
                    fl.Retcode = returncode;
                    int result = DB.saveFileLog(fl);
                    if (result != filelog_id) unLogger.WriteError("Filelogger ID: " + result + " aangemaakt voor resultaat van " + filelog_id);
                    return fl.Filelog_id;
                }
                else
                {
                    unLogger.WriteError("Filelogger ID: " + filelog_id + " record niet gevonden! returncode : " + returncode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return 0;
            }
        }
    }
}
