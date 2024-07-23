using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32AFLAM : Win32
    {
        [ThreadStatic]
        private static Win32AFLAM singleton;

        public static Win32AFLAM Init()
        {
            if (singleton == null)
            {
                singleton = new Win32AFLAM();
            }
            return singleton;
        }
        /*
         BUG 1888 hok erbij  pPerHok: en  pAlleenHok
         * 
         * function lst_AB_Aflamlijst(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pSoortlijst, pLamDetails, pTotaalOverzicht: integer;
                  pBeginDatum, pEindDatum: TDateTime;
                  pAlleenAanwezigeMoeders, pSortering: integer;
                  pPerHok: integer; pAlleenHok: PAnsiChar): Integer; stdcall;
         * 
         * function lst_AB_Aflamlijst(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pResourceFolder, pTaalcode: PAnsiChar; pTaalnr, pLandnr: integer;
                  pCallBack: Pointer;
                  pSoortlijst, pLamDetails, pTotaalOverzicht: integer;
                  pBeginDatum, pEindDatum: TDateTime;
                  pAlleenAanwezigeMoeders, pSortering: integer;
                  pPerHok: integer; pAlleenHok: PAnsiChar): Integer; stdcall;

         * 
         */
        public int call_lst_AB_Aflamlijst(int pProgID, int pProgramID, int pSoortBestand,
                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                string pResourceFolder, string pTaalcode, int pTaalnr, int pLandnr,
                                pCallback ReadDataProc,
                                int pSoortlijst, int pLamDetails, int pTotaaloverzicht,
                                DateTime pBeginDatum, DateTime pEindDatum,
                                int pAlleenAanwezigeMoeders, int pSortering,
                                int pPerHok, string pAlleenHok)
        {
            lock (typeof(Win32AFLAM))
            {
                string sFilename = "AFLAM.dll";

                lst_AB_Aflamlijst handle = (lst_AB_Aflamlijst)ExecuteProcedureDLL(typeof(lst_AB_Aflamlijst), sFilename, "lst_AB_Aflamlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr, pLandnr,
                        ReadDataProc,
                        pSoortlijst, pLamDetails, pTotaaloverzicht,
                        pBeginDatum, pEindDatum,
                        pAlleenAanwezigeMoeders, pSortering,
                        pPerHok, pAlleenHok);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_Aflamlijst(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         string pResourceFolder, string pTaalcode, int pTaalnr, int pLandnr,
                                         pCallback ReadDataProc,
                                         int pSoortlijst, int pLamDetails, int pTotaaloverzicht,
                                         DateTime pBeginDatum, DateTime pEindDatum,
                                         int pAlleenAanwezigeMoeders, int pSortering,
                                         int pPerHok, string pAlleenHok);


        public delegate void pCallback(int PercDone, string Msg);

    }
}
