using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32BEDREGIS : Win32
    {
        [ThreadStatic]
        private static Win32BEDREGIS singleton;

        public static Win32BEDREGIS Init()
        {
            if (singleton == null)
            {
                singleton = new Win32BEDREGIS();
            }
            return singleton;
        }
        public Win32BEDREGIS()
            : base(false)
        {
        }

        public int call_lst_AB_bedrijfsregisterSG(int pProgID, int pProgramID, int pSoortBestand,
                                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                  pCallback ReadDataProc,
                                                  DateTime pDtBegin, DateTime pDtEnd, bool pMutatiesOptellen)
        {
            lock (typeof(Win32BEDREGIS))
            {
                lst_AB_bedrijfsregisterSG handle = (lst_AB_bedrijfsregisterSG)ExecuteProcedureDLL(typeof(lst_AB_bedrijfsregisterSG), "BEDREGIS.DLL", "lst_AB_bedrijfsregisterSG");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd, pMutatiesOptellen);

                FreeDLL("BEDREGIS.DLL");
                return tmp;
            }
        }


        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_bedrijfsregisterSG(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pDtBegin, DateTime pDtEnd, bool pMutatiesOptellen);

        /*
         function lst_AB_bedrijfsregister(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pCompact, pGroeppnr, pAanvoerGroep, pStal, pAfdeling: integer;
              pBeginDatum, pEindDatum: TDateTime;
              pLand, pIncclAanwInPeriode, pSortering: integer): Integer; stdcall;


         */

        //LET OP DIT IS EEN ANDERE DLL ZIE BUG 1814
        public int call_lst_AB_bedrijfsregister(int pProgID, int pProgramID, int pSoortBestand,
                                          string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                          pCallback ReadDataProc,
                                          int pCompact, int pGroeppnr, int pAanvoerGroep, int pStal, int pAfdeling,
                                          DateTime pDtBegin, DateTime pDtEnd,
                                          int pLand, int pIncclAanwInPeriode, int pSortering)
        {
            lock (typeof(Win32BEDREGIS))
            {
                lst_AB_bedrijfsregister handle = (lst_AB_bedrijfsregister)ExecuteProcedureDLL(typeof(lst_AB_bedrijfsregister), "REGISTB.DLL", "lst_AB_bedrijfsregister");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                           pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                          ReadDataProc,
                                           pCompact, pGroeppnr, pAanvoerGroep, pStal, pAfdeling,
                                           pDtBegin, pDtEnd,
                                          pLand, pIncclAanwInPeriode, pSortering);

                FreeDLL("REGISTB.DLL");
                return tmp;
            }
        }




        delegate int lst_AB_bedrijfsregister(int pProgID, int pProgramID, int pSoortBestand,
                                          string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                          pCallback ReadDataProc,
                                          int pCompact, int pGroeppnr, int pAanvoerGroep, int pStal, int pAfdeling,
                                          DateTime pDtBegin, DateTime pDtEnd,
                                          int pLand, int pIncclAanwInPeriode, int pSortering);
    }
}
