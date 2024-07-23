using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
    class Win32instinfoxml : Win32
    {
        [ThreadStatic]
        private static Win32instinfoxml singleton;

        public static Win32instinfoxml Init()
        {
            if (singleton == null)
            {
                singleton = new Win32instinfoxml();
            }
            return singleton;
        }

        delegate void dmaakInstallatieXML(string pUBNnr, string pFolder,
                    string pCSVlicenties, string pProgramname, string pVersion);
        
        public void maakInstallatieXML(string pUBNnr, string pFolder,
              string pCSVlicenties, string pProgramname, string pVersion)
        {
            dmaakInstallatieXML handle = (dmaakInstallatieXML)ExecuteProcedureDLL(typeof(dmaakInstallatieXML), "instinfoxml.dll", "maakInstallatieXML");
            handle(pUBNnr, pFolder,
                     pCSVlicenties, pProgramname, pVersion);
            FreeDLL("instinfoxml.dll");
        }
    }
}
