using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    [Serializable]
    class EDIDAP2XMLReader : IReaderPlugin
    {
        private AFSavetoDB DB;

        public String GetFilter()
        {
            return "*.DAP";
        }

        public List<String> getExcludeList()
        {
            return new List<String>();
        }

        public void setSaveToDB(AFSavetoDB value)
        {
            DB = value;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            try
            {
                String archiefnaam = Bestandsnaam;
                String File = System.IO.Path.GetFileNameWithoutExtension(Bestandsnaam);
                String LogFile = unLogger.getLogDir("XML") + "EDIDAP#" + File + "#" + DateTime.Now.Ticks + ".log";
                Win32PDA2Agrobase PDA2Agrobase = new Win32PDA2Agrobase();
                int Result = PDA2Agrobase.importXMLMySQL(programId,
                        agrobaseUser,
                        agrobasePassword,
                        LogFile, unRechten.getDBHost(), Bestandsnaam, fileLogId);

                unLogger.WriteDebug("EDIDAP2XMLReader.Reader           : " + Bestandsnaam + " result: " + Result.ToString());
                if (Result > 0)
                {
                    unLogger.WriteDebug("EDIDAP2XMLReader.Reader          :" + Bestandsnaam + " ingelezen");
                    return 1;
                }
                else
                {
                    unLogger.WriteDebug("EDIDAP2XMLReader.Reader          :" + Bestandsnaam + " niet ingelezen");
                    return -1;
                }


            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return -2;
            }
        }
    }
}
