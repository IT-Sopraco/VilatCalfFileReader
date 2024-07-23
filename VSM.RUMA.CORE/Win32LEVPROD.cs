using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32LEVPROD : Win32
    {
        [ThreadStatic]
        private static Win32LEVPROD singleton;

        public static Win32LEVPROD Init()
        {
            if (singleton == null)
            {
                singleton = new Win32LEVPROD();
            }
            return singleton;
        }

        public Win32LEVPROD()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Levensproduktielijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pBegindat, pEinddat: TDateTime;
              pInclAfgevoerd, pToonDieren, pSortering: integer): Integer; stdcall;
         */

        public int call_lst_AB_Levensproduktielijst(int pProgID, int pProgramID, int pSoortBestand,
           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
           pCallback ReadDataProc,
           DateTime pBegindat,DateTime pEinddat,
           int pInclAfgevoerd,int pToonDieren,int pSortering)
        {
            lock (typeof(Win32LEVPROD))
            {
                lst_AB_Levensproduktielijst handle = (lst_AB_Levensproduktielijst)ExecuteProcedureDLL(typeof(lst_AB_Levensproduktielijst), "LEVPROD.DLL", "lst_AB_Levensproduktielijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pBegindat, pEinddat,
                                 pInclAfgevoerd, pToonDieren, pSortering);

                FreeDLL("LEVPROD.DLL");
                return tmp;
            }
        }




        delegate int lst_AB_Levensproduktielijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pBegindat, DateTime pEinddat,
                                               int pInclAfgevoerd, int pToonDieren, int pSortering);
    }
}
