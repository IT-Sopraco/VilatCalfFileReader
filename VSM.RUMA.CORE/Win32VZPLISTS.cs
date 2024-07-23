using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32VZPLISTS : Win32
    {
        [ThreadStatic]
        private static Win32VZPLISTS singleton;

        public static Win32VZPLISTS Init()
        {
            if (singleton == null)
            {
                singleton = new Win32VZPLISTS();
            }
            return singleton;
        }

        public Win32VZPLISTS()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Aanvoerlijst(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pBegindat, pEinddat: TDateTime;
          pThridLeverancier, pSortering: integer): Integer; stdcall;

         */

        public int call_lst_AB_Aanvoerlijst(int pProgID, int pProgramID, int pSoortBestand,
                                          string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                          pCallback ReadDataProc,
                                          DateTime pBegindat, DateTime pEinddat,
                                          int pThridLeverancier, int pSortering)
        {
            lock (typeof(Win32VZPLISTS))
            {
                lst_AB_Aanvoerlijst handle = (lst_AB_Aanvoerlijst)ExecuteProcedureDLL(typeof(lst_AB_Aanvoerlijst), "VZPLISTS.DLL", "lst_AB_Aanvoerlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pBegindat, pEinddat,
                                 pThridLeverancier, pSortering);

                FreeDLL("VZPLISTS.DLL");
                return tmp;
            }
        }



        delegate int lst_AB_Aanvoerlijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pBegindat, DateTime pEinddat,
                                               int pThridLeverancier, int pSortering);

        /*
            function lst_AB_Afvoerlijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pBegindat1, pEinddat1: TDateTime;
              pThridAfnemer1: integer;
              pDatum2: TDateTime; pMovkind2, pThridAfnemer2,
              pThridTransporteur2, pTransportid2: integer;
              pSortering: integer): Integer; stdcall;
         * 
         * function lst_AB_Afvoerlijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallBack: Pointer;
              pBegindat1, pEinddat1: TDateTime;
              pThridAfnemer1: integer;
              pDatum2: TDateTime; pMovkind2, pThridAfnemer2,
              pThridTransporteur2, pTransportid2: integer;
              pSortering: integer): Integer; stdcall;
         */
        public int call_lst_AB_Afvoerlijst(int pProgID, int pProgramID, int pSoortBestand,
                                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                    string pResourceFolder, string pTaalcode, int pTaalnr,
                                    pCallback ReadDataProc,
                                    DateTime pBegindat1, DateTime pEinddat1,
                                    int pThridAfnemer1,
                                    DateTime pDatum2, int pMovkind2, int pThridAfnemer2,
                                    int pThridTransporteur2, int pTransportid2,
                                    int pSortering)
        {
            lock (typeof(Win32VZPLISTS))
            {
                lst_AB_Afvoerlijst handle = (lst_AB_Afvoerlijst)ExecuteProcedureDLL(typeof(lst_AB_Afvoerlijst), "VZPLISTS.DLL", "lst_AB_Afvoerlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                    pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                    pResourceFolder, pTaalcode, pTaalnr,
                                    ReadDataProc,
                                    pBegindat1, pEinddat1,
                                    pThridAfnemer1,
                                    pDatum2, pMovkind2, pThridAfnemer2,
                                    pThridTransporteur2, pTransportid2, pSortering);

                FreeDLL("VZPLISTS.DLL");
                return tmp;
            }
        }



        delegate int lst_AB_Afvoerlijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                DateTime pBegindat1, DateTime pEinddat1,
                                                int pThridAfnemer1,
                                                DateTime pDatum2, int pMovkind2, int pThridAfnemer2,
                                                int pThridTransporteur2, int pTransportid2, int pSortering);
    }
}
