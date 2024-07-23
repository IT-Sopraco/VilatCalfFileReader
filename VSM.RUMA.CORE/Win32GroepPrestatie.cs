using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32GroepPrestatie : Win32
    {
        [ThreadStatic]
        private static Win32GroepPrestatie singleton;

        public static Win32GroepPrestatie Init()
        {
            if (singleton == null)
            {
                singleton = new Win32GroepPrestatie();
            }
            return singleton;
        }

        /*
         function lst_AB_groepprestatie(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pAanwezigheid, pAflamperiode: integer; pBegindat, pEinddat: TDateTime;
              pPerHok: Integer; pAlleenHok: PAnsiChar;
              pSortering: Integer): Integer; stdcall;
         */

        public int call_lst_AB_groepprestatie(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           string pResourceFolder, string pTaalcode, int pTaalnr,
                                           pCallback ReadDataProc, int pAanwezigheid, int pAflamperiode, DateTime pBegindat, DateTime pEinddat,
                                           int pPerHok, string pAlleenHok, int pSortering)
        {
            lock (typeof(Win32GroepPrestatie))
            {
                string sFilename = "GRPPREST.DLL";

                lst_AB_groepprestatie handle = (lst_AB_groepprestatie)ExecuteProcedureDLL(typeof(lst_AB_groepprestatie), sFilename, "lst_AB_groepprestatie");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc, pAanwezigheid, pAflamperiode, pBegindat, pEinddat, pPerHok, pAlleenHok, pSortering);

                FreeDLL(sFilename);
                return tmp;                
            }               
         
        }


        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_groepprestatie(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           string pResourceFolder, string pTaalcode, int pTaalnr,
                                           pCallback ReadDataProc, int pAanwezigheid, int pAflamperiode, DateTime pBegindat, DateTime pEinddat,
                                           int pPerHok, string pAlleenHok, int pSortering);	
    }
}
