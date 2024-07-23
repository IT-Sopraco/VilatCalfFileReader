using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32AGROLABLISTS : Win32
    {
        [ThreadStatic]
        private static Win32AGROLABLISTS singleton;

        public static Win32AGROLABLISTS Init()
        {
            if (singleton == null)
            {
                singleton = new Win32AGROLABLISTS();
            }
            return singleton;
        }

        public Win32AGROLABLISTS()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        ///////////////////////////////

        delegate int lst_AB_ParaCertificaat(int pSoortBestand, string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog, pCallback callback, DateTime pDatum);
        public int call_lst_AB_ParaCertificaat(int pSoortBestand, string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog, pCallback callback, DateTime pDatum)
        {
            lock (typeof(Win32AGROLABLISTS))
            {
                string sFilename = "AGROLABLISTS.DLL";

                lst_AB_ParaCertificaat handle = (lst_AB_ParaCertificaat)ExecuteProcedureDLL(typeof(lst_AB_ParaCertificaat), sFilename, "lst_AB_ParaCertificaat");

                int tmp = handle(pSoortBestand, pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog, callback, pDatum);

                FreeDLL(sFilename);
                return tmp;
            }
        }
    }
}
