using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32LOCATIE : Win32
    {
        [ThreadStatic]
        private static Win32LOCATIE singleton;

        public static Win32LOCATIE Init()
        {
            if (singleton == null)
            {
                singleton = new Win32LOCATIE();
            }
            return singleton;
        }

        public Win32LOCATIE()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Locatielijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pInPeriode: integer; pBegindat, pEinddat: TDateTime;
              pAlleenLaatsetLocatie, pAanwezigheid: integer;
              pSoortLocatie, pGroepid, pStalid, pAfdelingid: integer;
              pHokVan, pHokTot: shortstring; pSortering: integer): Integer; stdcall;

         * function lst_AB_Locatielijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallBack: Pointer;
              pInPeriode: integer; pBegindat, pEinddat: TDateTime;
              pAlleenLaatsetLocatie, pAanwezigheid: integer;
              pSoortLocatie, pGroepid, pStalid, pAfdelingid: integer;
              pHokVan, pHokTot: pAnsiChar; pSortering: integer): Integer; stdcall;

         */

        public int call_lst_AB_Locatielijst(int pProgID, int pProgramID, int pSoortBestand,
                            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                            string pResourceFolder, string pTaalcode, int pTaalnr,
                            pCallback ReadDataProc,
                            int pInPeriode, DateTime pBegindat, DateTime pEinddat,
                            int pAlleenLaatsetLocatie,int  pAanwezigheid,
                            int pSoortLocatie, int pGroepid, int pStalid, int pAfdelingid,
                            string pHokVan, string pHokTot, int pSortering)
        {
            lock (typeof(Win32LOCATIE))
            {
                lst_AB_Locatielijst handle = (lst_AB_Locatielijst)ExecuteProcedureDLL(typeof(lst_AB_Locatielijst), "LOCATIE.DLL", "lst_AB_Locatielijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pInPeriode, pBegindat, pEinddat,
                                 pAlleenLaatsetLocatie, pAanwezigheid,
                                 pSoortLocatie, pGroepid, pStalid, pAfdelingid,
                                 pHokVan, pHokTot, pSortering);

                FreeDLL("LOCATIE.DLL");
                return tmp;
            }
        }



        delegate int lst_AB_Locatielijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                int pInPeriode, DateTime pBegindat, DateTime pEinddat,
                                                int pAlleenLaatsetLocatie, int pAanwezigheid,
                                                int pSoortLocatie, int pGroepid, int pStalid, int pAfdelingid,
                                                string pHokVan, string pHokTot, int pSortering);
    }
}
