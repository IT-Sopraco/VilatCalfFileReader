using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
    public class Win32Melkprod : Win32
    {
        [ThreadStatic]
        private static Win32Melkprod singleton;

        public static Win32Melkprod Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Melkprod();
            }
            return singleton;
        }

        public int call_lst_AB_Melkproductielijst(int pProgID, int pProgramID, int pSoortBestand,
                                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                  pCallback ReadDataProc,
                                                  DateTime pDtBegin, DateTime pDtEnd,
                                                  int pSoortLijst)
        {
            lock (typeof(Win32Melkprod))
            {
                string sFilename = "MELKPROD.DLL";

                lst_AB_Melkproductielijst handle = (lst_AB_Melkproductielijst)ExecuteProcedureDLL(typeof(lst_AB_Melkproductielijst),
                                                   sFilename, "lst_AB_Melkproductielijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd, pSoortLijst);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_Melkproductielijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pDtBegin, DateTime pDtEnd, 
                                               int pSoortLijst);
    }
}