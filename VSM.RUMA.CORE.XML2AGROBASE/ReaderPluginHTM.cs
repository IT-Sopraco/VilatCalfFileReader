using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.CompilerServices;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    [Serializable]
    class ReaderPluginHTM : IReaderPlugin
    {
        public String GetFilter()
        {
            return "*.HTM*";
        }

        public List<String> getExcludeList()
        {
            List<String> Excludes =  new List<String>();
            Excludes.Add(".ZIP");
            Excludes.Add(".TMP");
            Excludes.Add(".FXML");
            return Excludes;
        }

        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            try
            {

                Win32PDA2Agrobase PDA2Agrobase = new Win32PDA2Agrobase();
                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM ProgramId : " + programId);
                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM Username  : " + agrobaseUser);
                // wachtwoorden niet loggen
                //unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM Password  : " + agrobasePassword);
                String File = System.IO.Path.GetFileNameWithoutExtension(Bestandsnaam);
                String LogFile = unLogger.getLogDir("XML") + "pda2agrobase#" + File + "#" + DateTime.Now.Ticks + ".log";
                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM LogFile   : " + LogFile);
                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM HostName  : " + unRechten.getDBHost());
                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM File      : " + File);


                unLogger.WriteDebug("");

                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM LogDir    : " + unLogger.getLogDir());

                FILELOG fl = Facade.GetInstance().getSaveToDB(agroFactuurToken).getFileLogById(fileLogId);
                fl.Logfilepath = LogFile;
                Facade.GetInstance().getSaveToDB(agroFactuurToken).saveFileLog(fl);

                int Result = PDA2Agrobase.importXMLMySQL(programId,
                        agrobaseUser,
                        agrobasePassword,
                        LogFile, unRechten.getDBHost(), Bestandsnaam, fileLogId);

                unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM           : " + Bestandsnaam + " result: " + Result.ToString());
                if (Result > 0)
                {
                    unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM          :" + Bestandsnaam + " ingelezen");
                    return 1;
                }
                else
                {
                    unLogger.WriteDebug("XML2AGROBASE.ReaderPluginHTM          :" + Bestandsnaam + " niet ingelezen");
                    return -1;
                }

            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                try
                {
                    FILELOG fl = Facade.GetInstance().getSaveToDB(agroFactuurToken).getFileLogById(fileLogId);
                    fl.Errormemo += ex.Message;
                    fl.Errormemo += ex.StackTrace;
                    Facade.GetInstance().getSaveToDB(agroFactuurToken).saveFileLog(fl);
                }
                catch (Exception flex)
                {
                    unLogger.WriteError("Error saving message to filelog table " + flex.Message, flex);
                    return -52;
                }
                return -2;
            }
        }

        public void setSaveToDB(AFSavetoDB value)
        {
            //db niet nodig.
        }

    }
}
