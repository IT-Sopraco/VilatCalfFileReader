using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32STATUSLS : Win32
    {
        [ThreadStatic]
        private static Win32STATUSLS singleton;

        public static Win32STATUSLS Init()
        {
            if (singleton == null)
            {
                singleton = new Win32STATUSLS();
            }
            return singleton;
        }

        public Win32STATUSLS()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Statuslijst(
                pProgID, pProgramID, pSoortBestand: Integer;
                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                pCallBack: Pointer;
                pBegindat, pEinddat: TDateTime;
                pStatus, pAlleenLaatsteStatus, pInclAfgevoerd,
                pSortering: integer): Integer; stdcall;
         * 
         * function lst_AB_Statuslijst(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                  pCallBack: Pointer;
                  pBegindat, pEinddat: TDateTime;
                  pStatus, pAlleenLaatsteStatus, pInclAfgevoerd,
                  pSortering: integer): Integer; stdcall;
         */
        public int call_lst_AB_Statuslijst(int pProgID, int pProgramID, int pSoortBestand,
                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                           string pResourceFolder, string pTaalcode, int pTaalnr,
                           pCallback ReadDataProc,
                           DateTime pBegindat, DateTime pEinddat,
                           int pStatus, int pAlleenLaatsteStatus, int pInclAfgevoerd,
                           int pSortering)
        {
            lock (typeof(Win32STATUSLS))
            {
                lst_AB_Statuslijst handle = (lst_AB_Statuslijst)ExecuteProcedureDLL(typeof(lst_AB_Statuslijst), "STATUSLS.DLL", "lst_AB_Statuslijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pBegindat, pEinddat,
                                 pStatus, pAlleenLaatsteStatus, pInclAfgevoerd,
                                 pSortering);

                FreeDLL("STATUSLS.DLL");
                return tmp;
            }
        }




        delegate int lst_AB_Statuslijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               string pResourceFolder, string pTaalcode, int pTaalnr,
                                               pCallback ReadDataProc,
                                               DateTime pBegindat, DateTime pEinddat,
                                               int pStatus, int pAlleenLaatsteStatus, int pInclAfgevoerd,
                                               int pSortering);
    }
}
