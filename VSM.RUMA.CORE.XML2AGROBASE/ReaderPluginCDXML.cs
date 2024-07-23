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
    /// <summary>
    /// ReaderPluginCDXML, vangt .CDXML bestanden af en verwerkt het
    /// op dezelfde manier als de Gewone ReaderPlugin (pda2ruma) bestanden verwerkt
    /// </summary>
    [Serializable]
    public class ReaderPluginCDXML : IReaderPlugin
    {
        public String GetFilter()
        {
            return "*.CDXML*";
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

        public int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser, String agrobasePassword, int fileLogId, String Bestandsnaam)
        {
            return new ReaderPlugin().LeesFile(thrId, agroFactuurToken, programId, agrobaseUser, agrobasePassword, fileLogId, Bestandsnaam);
        }

        public void setSaveToDB(AFSavetoDB value)
        {
            //db niet nodig.
        }

    }
}
