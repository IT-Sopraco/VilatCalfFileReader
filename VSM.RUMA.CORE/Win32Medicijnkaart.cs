using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Medicijnkaart : Win32
    {
        [ThreadStatic]
        private static Win32Medicijnkaart singleton;

        public static Win32Medicijnkaart Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Medicijnkaart();
            }
            return singleton;
        }

        public Win32Medicijnkaart()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Medicijnregistratiekaart(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer;
              pBegindatum, pEinddatum: TDateTime; pGroepnr: integer;
              pAfvoerdat: TDateTime; pThridBestemming: integer; pHoknr: PAnsiChar;
              pGeregistreerdeMedicijnen, pNietGeregistreerdeMedicijnen,
              pExclKoppelBehandelingen, pSortering: integer): Integer; stdcall;
         * 
         * function lst_AB_Medicijnregistratiekaart(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pBegindatum, pEinddatum: TDateTime; pGroepnr: integer;
              pAfvoerdat: TDateTime; pThridBestemming: integer; pHoknr: PAnsiChar;
              pGeregistreerdeMedicijnen, pNietGeregistreerdeMedicijnen,
              pExclKoppelBehandelingen, pSortering: integer): Integer; stdcall;

         */

        public int call_lst_AB_Medicijnregistratiekaart(int pProgID, int pProgramID, int pSoortBestand,
                          string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                          string pResourceFolder, string pTaalcode, int pTaalnr,
                          pCallback ReadDataProc,
                          DateTime pBegindatum, DateTime pEinddatum, int pGroepnr,
                               DateTime pAfvoerdat, int pThridBestemming, string pHoknr,
                               int pGeregistreerdeMedicijnen, int pNietGeregistreerdeMedicijnen,
                               int pExclKoppelBehandelingen, int pSortering)
        {
            lock (typeof(Win32Leeftijd))
            {
                lst_AB_Medicijnregistratiekaart handle = (lst_AB_Medicijnregistratiekaart)ExecuteProcedureDLL(typeof(lst_AB_Medicijnregistratiekaart), "Medreg.DLL", "lst_AB_Medicijnregistratiekaart");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                pResourceFolder, pTaalcode, pTaalnr,
                                ReadDataProc,
                                pBegindatum, pEinddatum, pGroepnr,
                                pAfvoerdat, pThridBestemming, pHoknr,
                                pGeregistreerdeMedicijnen, pNietGeregistreerdeMedicijnen,
                                pExclKoppelBehandelingen, pSortering);

                FreeDLL("Medreg.DLL");
                return tmp;
            }
        }



        delegate int lst_AB_Medicijnregistratiekaart(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               string pResourceFolder, string pTaalcode, int pTaalnr,
                                               pCallback ReadDataProc,
                                               DateTime pBegindatum, DateTime pEinddatum, int pGroepnr,
                               DateTime pAfvoerdat, int pThridBestemming, string pHoknr,
                               int pGeregistreerdeMedicijnen, int pNietGeregistreerdeMedicijnen,
                               int pExclKoppelBehandelingen, int pSortering);
    }
}
