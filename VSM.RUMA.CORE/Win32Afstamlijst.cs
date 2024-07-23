using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Afstamlijst : Win32
    {
        [ThreadStatic]
        private static Win32Afstamlijst singleton;

        public static Win32Afstamlijst Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Afstamlijst();
            }
            return singleton;
        }

        /*
         function lst_AB_Afstamlijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr, pLandnr: integer;
              pCallBack: Pointer;
              pSoortlijst, pGrootvaders, pResponders: integer;
              pUbnnrBestemming: pAnsiChar; pBegindat, pEinddat: TDateTime;
              pHoknr: PAnsiChar; pSortering: integer): Integer; stdcall; 
         */

        public int call_lst_AB_Afstamlijst(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       string pResourceFolder, string pTaalcode, int pTaalnr, int pLandnr,
                                       pCallback ReadDataProc, 
                                       int pSoortlijst, int pGrootvaders, int pResponders,
                                       string pUbnnrBestemming, DateTime pBegindat, DateTime pEinddat,
                                       string pHoknr, int pSortering)
        {
            lock (typeof(Win32Afstamlijst))
            {
                string sFilename = "AFSTAMGE.DLL";

                lst_AB_Afstamlijst handle = (lst_AB_Afstamlijst)ExecuteProcedureDLL(typeof(lst_AB_Afstamlijst), sFilename, "lst_AB_Afstamlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr, pLandnr,
                                 ReadDataProc, 
                                 pSoortlijst, pGrootvaders, pResponders, pUbnnrBestemming, pBegindat, pEinddat,
                                 pHoknr, pSortering);

                FreeDLL(sFilename);
                return tmp;
            }
        }
        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_Afstamlijst(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       string pResourceFolder, string pTaalcode, int pTaalnr, int pLandnr,
                                       pCallback ReadDataProc, 
                                       int pSoortlijst, int pGrootvaders, int pResponders,
                                       string pUbnnrBestemming, DateTime pBegindat, DateTime pEinddat,
                                       string pHoknr, int pSortering);                               
                 
    }
}
