using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32KengetallenlijstNSFO : Win32
    {
        [ThreadStatic]
        private static Win32KengetallenlijstNSFO singleton;

        public static Win32KengetallenlijstNSFO Init()
        {
            if (singleton == null)
            {
                singleton = new Win32KengetallenlijstNSFO();
            }
            return singleton;
        }

        public int call_lst_AB_KengetallenNSFO(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
										   pCallback ReadDataProc, int pJaar)
        {
            lock (typeof(Win32KengetallenlijstNSFO))
            {
                string sFilename = "kengetalNSFO.dll";

                lst_AB_KengetallenNSFO handle = (lst_AB_KengetallenNSFO)ExecuteProcedureDLL(typeof(lst_AB_KengetallenNSFO), sFilename, "lst_AB_kengetallenNSFO");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, 
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,                                  pJaar);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_KengetallenNSFO(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       pCallback ReadDataProc, int pJaar);


        /*
         * BUG 2080
         function lst_AB_kengetallenNSFO_stamboek(
       pProgID, pProgramID, pSoortBestand: integer;
       pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
       pCallback: Pointer; pGebied: PAnsiChar; pJaar: integer): Integer; stdcall;
         */
        public int call_lst_AB_kengetallenNSFO_stamboek(int pProgID, int pProgramID, int pSoortBestand,
                                   string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                   pCallback ReadDataProc, string pGebied, int pJaar)
        {
            lock (typeof(Win32KengetallenlijstNSFO))
            {
                string sFilename = "kengetalNSFO.dll";

                lst_AB_kengetallenNSFO_stamboek handle = (lst_AB_kengetallenNSFO_stamboek)ExecuteProcedureDLL(typeof(lst_AB_kengetallenNSFO_stamboek), sFilename, "lst_AB_kengetallenNSFO_stamboek");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc, pGebied, pJaar);

                FreeDLL(sFilename);
                return tmp;
            }
        }
        delegate int lst_AB_kengetallenNSFO_stamboek(int pProgID, int pProgramID, int pSoortBestand,
                                    string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                    pCallback ReadDataProc, string pGebied, int pJaar);
    }
}

