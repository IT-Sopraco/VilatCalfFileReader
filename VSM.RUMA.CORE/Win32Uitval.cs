using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Uitval : Win32
    {
        [ThreadStatic]
        private static Win32Uitval singleton;

        public static Win32Uitval Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Uitval();
            }
            return singleton;
        }

        /*
         function lst_AB_Uitvallijst(pProgID, pProgramID, pSoortBestand: Integer;
                            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                            pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                            pCallBack: Pointer;
                            pDtBegin, pDtEnd: TDateTime;
                            pGroupId, pSort, pThrIdLeverancier,
                            pPerHok: Integer; pAlleenHok: PAnsiChar): Integer; stdcall; export; 
         */
        public int call_lst_AB_Uitvallijst(int pProgID, int pProgramID, int pSoortBestand,
                                            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                            string pResourceFolder, string pTaalcode, int pTaalnr,
                                            pCallback ReadDataProc,
                                            DateTime pDtBegin, DateTime pDtEnd,
                                            int pGroupId, int pSort, int pThrIdLeverancier)
        {
            lock (typeof(Win32Uitval))
            {
                string sFilename = "UITVAL.dll";

                lst_AB_Uitvallijst handle = (lst_AB_Uitvallijst)ExecuteProcedureDLL(
                                                                typeof(lst_AB_Uitvallijst), 
                                                                sFilename, 
                                                                "lst_AB_Uitvallijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd,
                                 pGroupId, pSort, pThrIdLeverancier);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_Uitvallijst(int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                        string pResourceFolder, string pTaalcode, int pTaalnr,
                                        pCallback ReadDataProc,
                                        DateTime pDtBegin, DateTime pDtEnd,
                                        int pGroupId, int pSort, int pThrIdLeverancier);
        /*
         function lst_AB_Uitvallijst_Overzicht(
                            pProgID, pProgramID, pSoortBestand: Integer;
                            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                            pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                            pCallBack: Pointer;
                            pDtBegin, pDtEnd: TDateTime;
                            pSort, pGroupBy, pThrIdLeverancier: Integer): Integer; stdcall; export;
         */
        public int call_lst_AB_Uitvallijst_Overzicht(
                                            int pProgID, int pProgramID, int pSoortBestand,
                                            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                            string pResourceFolder,string pTaalcode, int pTaalnr,
                                            pCallback ReadDataProc,
                                            DateTime pDtBegin, DateTime pDtEnd,
                                            int pSort, int pGroupBy, int pThrIdLeverancier) 
        {
            lock (typeof(Win32Uitval))
            {
                string sFilename = "UITVAL.dll";

                lst_AB_Uitvallijst_Overzicht handle = (lst_AB_Uitvallijst_Overzicht)ExecuteProcedureDLL(
                                                                                    typeof(lst_AB_Uitvallijst_Overzicht), 
                                                                                    sFilename, 
                                                                                    "lst_AB_Uitvallijst_Overzicht");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd,
                                 pSort, pGroupBy, pThrIdLeverancier);

                FreeDLL(sFilename);
                return tmp;
            }        
        }
        delegate int lst_AB_Uitvallijst_Overzicht(
                                int pProgID, int pProgramID, int pSoortBestand,
                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                pCallback ReadDataProc,
                                DateTime pDtBegin, DateTime pDtEnd,
                                int pSort, int pGroupBy, int pThrIdLeverancier);
 
        /////////////////////
     

        public int call_lst_AB_Uitvallijst_MetHok(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder, string pTaalcode, int pTaalnr,
                        pCallback ReadDataProc,
                        DateTime pDtBegin, DateTime pDtEnd,
                        int pGroupId, int pSort, int pThrIdLeverancier,
                        int pPerHok, string pAlleenHok)
        {
            lock (typeof(Win32Uitval))
            {
                string sFilename = "UITVAL.dll";

                lst_AB_Uitvallijst_MetHok handle = (lst_AB_Uitvallijst_MetHok)ExecuteProcedureDLL(
                                                                typeof(lst_AB_Uitvallijst_MetHok),
                                                                sFilename,
                                                                "lst_AB_Uitvallijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd,
                                 pGroupId, pSort, pThrIdLeverancier,
                                 pPerHok, pAlleenHok);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        


        delegate int lst_AB_Uitvallijst_MetHok(int pProgID, int pProgramID, int pSoortBestand,
                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                       string pResourceFolder, string pTaalcode, int pTaalnr,
                       pCallback ReadDataProc,
                       DateTime pDtBegin, DateTime pDtEnd,
                       int pGroupId, int pSort, int pThrIdLeverancier,
                       int pPerHok, string pAlleenHok);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
