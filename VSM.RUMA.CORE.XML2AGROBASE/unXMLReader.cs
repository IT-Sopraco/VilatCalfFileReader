using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB;
using System.Reflection;

namespace VSM.RUMA.CORE.XML2AGROBASE
{
    public class unXMLReader
    {
        IFacade mFacade;

        public unXMLReader(IFacade pFacade)
        {
            mFacade = pFacade;
        }

        public bool LeesBestand(String pBestandsnaam)
        {
            Win32PDA2Agrobase PDA2Agrobase = new Win32PDA2Agrobase();
            unLogger.WriteDebug("XML2AGROBASE.unXMLReader HostName : " + unRechten.getDBHost());
            unLogger.WriteDebug("XML2AGROBASE.unXMLReader LogDir   : " + unLogger.getLogDir());
            return PDA2Agrobase.importXMLMySQLxml(1, "vilatcalf_write", "Ay9OwPhbYqRm", unLogger.getLogDir("XML"), unRechten.getDBHost(), pBestandsnaam) > 0;
        }
    }
}
