using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Behandelplannen : Win32
    {
        [ThreadStatic]
        private static Win32Behandelplannen singleton;

        public static Win32Behandelplannen Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Behandelplannen();
            }
            return singleton;
        }

        /*
         function lst_AB_Behandelplannen(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallBack: Pointer): Integer; stdcall;
         */

        public int call_lst_AB_Behandelplannen(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           string pResourceFolder, string pTaalcode, int pTaalnr,
										   pCallback ReadDataProc)

        {
            lock (typeof(Win32Behandelplannen))
            {
                string sFilename = "TREATPLN.DLL";

                lst_AB_Behandelplannen handle = (lst_AB_Behandelplannen)ExecuteProcedureDLL(typeof(lst_AB_Behandelplannen), sFilename, "lst_AB_Behandelplannen");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, 
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc);

                FreeDLL(sFilename);
                return tmp;
            }           
        }

        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_Behandelplannen(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       string pResourceFolder, string pTaalcode, int pTaalnr,
                                       pCallback ReadDataProc);	
                
           
    }
}

