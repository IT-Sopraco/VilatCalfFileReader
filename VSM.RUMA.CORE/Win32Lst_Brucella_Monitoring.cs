using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Lst_Brucella_Monitoring : Win32
    {
        [ThreadStatic]
        private static Win32Lst_Brucella_Monitoring singleton;

        public static Win32Lst_Brucella_Monitoring Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Lst_Brucella_Monitoring();
            }
            return singleton;
        }

        public delegate void pCallback(int PercDone, string Msg);

        /* BUG 1796
         * function lst_Brucella_Inzend_Formulier( // BRUF001-1
           pProgID, pProgramID, pSoortBestand: Integer;
           pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
           pCallback: Pointer
           ): Integer; stdcall; Export;

        */

        public int call_lst_Brucella_Inzend_Formulier(
                                       int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword,
                                       string pBestand, string pLog,
                                       pCallback ReadDataProc, int Pbzs_ID)
        {
            lock (typeof(Win32Lst_Brucella_Monitoring))
            {
                string sFilename = "Lst_Brucella_Monitoring.dll";

                lst_Brucella_Inzend_Formulier handle =
                    (lst_Brucella_Inzend_Formulier)ExecuteProcedureDLL(
                        typeof(lst_Brucella_Inzend_Formulier), sFilename, "lst_Brucella_Inzend_Formulier");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc, Pbzs_ID);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_Brucella_Inzend_Formulier(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc, int Pbzs_ID);
        /*
            function lst_Brucella_Declaratie_Formulier( // BRUF002-1
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer
              ): Integer; stdcall; Export;

        */

        public int call_lst_Brucella_Declaratie_Formulier(
                                       int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword,
                                       string pBestand, string pLog,
                                       pCallback ReadDataProc, int Pbzs_ID)
        {
            lock (typeof(Win32Lst_Brucella_Monitoring))
            {
                string sFilename = "Lst_Brucella_Monitoring.dll";

                lst_Brucella_Declaratie_Formulier handle =
                    (lst_Brucella_Declaratie_Formulier)ExecuteProcedureDLL(
                        typeof(lst_Brucella_Declaratie_Formulier), sFilename, "lst_Brucella_Declaratie_Formulier");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc, Pbzs_ID);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_Brucella_Declaratie_Formulier(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc, int Pbzs_ID);
        /*
            function lst_Brucella_Instructie_Formulier( // BRUF003-1
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer
              ): Integer; stdcall; Export;
         */

        public int call_lst_Brucella_Instructie_Formulier(
                                      int pProgID, int pProgramID, int pSoortBestand,
                                      string pUbnnr, string pHostName, string pUserName, string pPassword,
                                      string pBestand, string pLog,
                                      pCallback ReadDataProc, int Pbzs_ID)
        {
            lock (typeof(Win32Lst_Brucella_Monitoring))
            {
                string sFilename = "Lst_Brucella_Monitoring.dll";

                lst_Brucella_Instructie_Formulier handle =
                    (lst_Brucella_Instructie_Formulier)ExecuteProcedureDLL(
                        typeof(lst_Brucella_Instructie_Formulier), sFilename, "lst_Brucella_Instructie_Formulier");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc, Pbzs_ID);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_Brucella_Instructie_Formulier(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc, int Pbzs_ID);
    }
}
