using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Leeftijd : Win32
    {
        [ThreadStatic]
        private static Win32Leeftijd singleton;

        public static Win32Leeftijd Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Leeftijd();
            }
            return singleton;
        }

        public Win32Leeftijd()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Leeftijdlijst(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pBeginGebdat, pEindGebdat: TDateTime;
          pGroepid, pStalid, pAfdelingid: integer;
          pHok: shortstring; pSortering: integer): Integer; stdcall;

         */
        public int call_lst_AB_Leeftijdlijst(int pProgID, int pProgramID, int pSoortBestand,
                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                  pCallback ReadDataProc,
                                  DateTime pBeginGebdat, DateTime pEindGebdat,
                                  int pGroepid,int pStalid,int pAfdelingid,
                                  string pHok, int pSortering)
        {
            lock (typeof(Win32Leeftijd))
            {
                lst_AB_Leeftijdlijst handle = (lst_AB_Leeftijdlijst)ExecuteProcedureDLL(typeof(lst_AB_Leeftijdlijst), "LEEFTIJD.DLL", "lst_AB_Leeftijdlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pBeginGebdat, pEindGebdat,
                                 pGroepid, pStalid, pAfdelingid,
                                 pHok, pSortering);

                FreeDLL("LEEFTIJD.DLL");
                return tmp;
            }
        }



        delegate int lst_AB_Leeftijdlijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pBeginGebdat, DateTime pEindGebdat,
                                               int pGroepid, int pStalid, int pAfdelingid,
                                               string pHok, int pSortering);
    }
}
