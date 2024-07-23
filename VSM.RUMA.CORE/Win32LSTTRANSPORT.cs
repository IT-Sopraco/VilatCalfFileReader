using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32LSTTRANSPORT : Win32
    {
        [ThreadStatic]
        private static Win32LSTTRANSPORT singleton;

        public static Win32LSTTRANSPORT Init()
        {
            if (singleton == null)
            {
                singleton = new Win32LSTTRANSPORT();
            }
            return singleton;
        }

        public int call_lst_AB_Transportlijst(int pProgID, int pProgramID, int pSoortBestand,
                                              string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              pCallback ReadDataProc,
                                              DateTime pDtBegin, DateTime pDtEnd,
                                              int pSort, int pMovThridExecutive)
        {
            lock (typeof(Win32LSTTRANSPORT))
            {
                string sFilename = "LSTTRANSPORT.dll";

                lst_AB_Transportlijst handle = (lst_AB_Transportlijst)ExecuteProcedureDLL(typeof(lst_AB_Transportlijst), sFilename, "lst_AB_Transportlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd,
                                 pSort, pMovThridExecutive);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_Transportlijst(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           pCallback ReadDataProc,
                                           DateTime pDtBegin, DateTime pDtEnd,
                                           int pSort, int pMovThridExecutive);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
