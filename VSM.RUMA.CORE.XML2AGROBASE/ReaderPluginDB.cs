using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    /// <summary>
    /// ReaderPluginDB, vangt .DB bestanden af (uit dirzip?) en verwerkt het
    /// op dezelfde manier als de Gewone ReaderPlugin (pda2ruma) bestanden verwerkt
    /// </summary>
    [Serializable]
    public class ReaderPluginDB : IReaderPlugin
    {
        public String GetFilter()
        {
            return "*.DB*";
        }

        public List<String> getExcludeList()
        {
            List<String> Excludes = new List<String>();
            Excludes.Add(".ZIP");
            Excludes.Add(".TMP");
            Excludes.Add(".FXML");
            Excludes.Add(".FAIL");
            return Excludes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thrId"></param>
        /// <param name="agroFactuurToken"></param>
        /// <param name="programId"></param>
        /// <param name="agrobaseUser"></param>
        /// <param name="agrobasePassword"></param>
        /// <param name="fileLogId"></param>
        /// <param name="bestandsnaam"></param>
        /// <returns></returns>
        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser, String agrobasePassword, int fileLogId, string bestandsnaam)
        {
            string lPrefix = $"{nameof(ReaderPluginDB)}.{nameof(LeesFile)} Bestandsnaam: {Path.GetFileName(bestandsnaam)} -";
            try
            {
                var db = Facade.GetInstance().getSaveToDB(agroFactuurToken);
                string dbHost = unRechten.getDBHost();
                string logDir = unLogger.getLogDir("DB");
                unLogger.WriteInfo($"{lPrefix} Begonnen.");

                var pda2Agrobase = new Win32PDA2Agrobase();

                unLogger.WriteDebug($"{lPrefix} DBHost:         {dbHost}");
                unLogger.WriteDebug($"{lPrefix} Log dir:        {logDir}");
                unLogger.WriteDebug($"{lPrefix} Bestand:        {bestandsnaam}");
                unLogger.WriteDebug($"{lPrefix} Third:          {thrId}");
                unLogger.WriteDebug($"{lPrefix} ProgramId:      {programId}");
                unLogger.WriteDebug($"{lPrefix} Agrobase User:  {programId}");
                unLogger.WriteDebug($"{lPrefix} Agrobase User:  {programId}");
                unLogger.WriteDebug($"{lPrefix} Filelog ID:     {fileLogId}");

                string file = Path.GetFileNameWithoutExtension(bestandsnaam);
                string logFile = $"{logDir}pda2agrobase#{file}#{DateTime.Now.Ticks}.log";
                unLogger.WriteDebug($"{lPrefix} Log file:       {logFile}");

                FILELOG fl = db.getFileLogById(fileLogId);
                fl.Logfilepath = logFile;
                if (db.saveFileLog(fl) <= 0)
                {
                    //Loggen, voor de rest is de file ingelezen dus niet returnen als error               
                    unLogger.WriteError($"{lPrefix} Error tijdens opslaan FILELOG.");
                }

                int result = pda2Agrobase.importXMLMySQL(programId, agrobaseUser, agrobasePassword, logFile, dbHost, bestandsnaam, fileLogId);
                unLogger.WriteInfo($"{lPrefix} Result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                unLogger.WriteError($"{lPrefix} Ex: {ex.Message}", ex);
                return -2;
            }
            finally
            {
                unLogger.WriteInfo($"{lPrefix} Finally.");
            }
        }

        public void setSaveToDB(AFSavetoDB value)
        {
            //db niet nodig.
        }

    }
}
