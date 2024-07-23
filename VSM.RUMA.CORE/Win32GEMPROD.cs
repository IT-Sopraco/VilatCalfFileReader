using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32GEMPROD : Win32
    {
        [ThreadStatic]
        private static Win32GEMPROD singleton;

        public static Win32GEMPROD Init()
        {
            if (singleton == null)
            {
                singleton = new Win32GEMPROD();
            }
            return singleton;
        }

        /*
         * BUG 1888
         * 
          function lst_AB_gemproduktielijst_ooi(
                  pProgID, pProgramID, pSoortBestand: integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallback: Pointer;
                  pWorpnr, pAflamperiode: integer; pBegindat, pEinddat: TDateTime;
                  pPerRas, pPerGroep, pGrafieken, pPerHok: Integer;
                  pAlleenHok: PAnsiChar; pSortering: Integer): Integer; stdcall;

          function lst_AB_gemproduktielijst_ooi(
                  pProgID, pProgramID, pSoortBestand: integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                  pCallback: Pointer;
                  pWorpnr, pAflamperiode: integer; pBegindat, pEinddat: TDateTime;
                  pPerRas, pPerGroep, pGrafieken, pPerHok: Integer;
                  pAlleenHok: PAnsiChar; pSortering: Integer): Integer; stdcall;

         */

        public int call_lst_AB_gemproduktielijst_ooi(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder, string pTaalcode, int pTaalnr,
                        pCallback ReadDataProc,
                        int pWorpnr, int pAflamperiode, DateTime pBegindat, DateTime pEinddat,
                        int pPerRas, int pPerGroep, int pGrafieken, int pPerHok,
                        string pAlleenHok, int pSortering)
        {
            lock (typeof(Win32GEMPROD))
            {
                string sFilename = "GEMPROD.dll";

                lst_AB_gemproduktielijst_ooi handle = (lst_AB_gemproduktielijst_ooi)ExecuteProcedureDLL(typeof(lst_AB_gemproduktielijst_ooi), sFilename, "lst_AB_gemproduktielijst_ooi");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pWorpnr, pAflamperiode, pBegindat, pEinddat,
                        pPerRas, pPerGroep, pGrafieken, pPerHok,
                        pAlleenHok, pSortering);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_gemproduktielijst_ooi(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         string pResourceFolder, string pTaalcode, int pTaalnr,
                                         pCallback ReadDataProc,
                                         int pWorpnr, int pAflamperiode, DateTime pBegindat, DateTime pEinddat,
                                         int pPerRas, int pPerGroep, int pGrafieken, int pPerHok,
                                         string pAlleenHok, int pSortering);

        /*
          function lst_AB_gemproduktielijst_lam(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer;
              pGroepnr, pPerHok: Integer;
              pAlleenHok: PAnsiChar; pSortering: Integer): Integer; stdcall;
         * 
         function lst_AB_gemproduktielijst_lam(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pGroepnr, pPerHok: Integer;
              pAlleenHok: PAnsiChar; pSortering: Integer): Integer; stdcall; 
         */

        public int call_lst_AB_gemproduktielijst_lam(int pProgID, int pProgramID, int pSoortBestand,
                     string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                     string pResourceFolder, string pTaalcode, int pTaalnr,
                     pCallback ReadDataProc,
                     int pGroepnr, int pPerHok,
                     string pAlleenHok, int pSortering)
        {
            lock (typeof(Win32GEMPROD))
            {
                string sFilename = "GEMPROD.dll";

                lst_AB_gemproduktielijst_lam handle = (lst_AB_gemproduktielijst_lam)ExecuteProcedureDLL(typeof(lst_AB_gemproduktielijst_lam), sFilename, "lst_AB_gemproduktielijst_lam");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pGroepnr, pPerHok,
                        pAlleenHok, pSortering);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_gemproduktielijst_lam(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         string pResourceFolder, string pTaalcode, int pTaalnr,
                                         pCallback ReadDataProc,
                                         int pGroepnr, int pPerHok,
                                         string pAlleenHok, int pSortering);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
