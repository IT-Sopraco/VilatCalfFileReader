using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace VSM.RUMA.CORE
{
    public class Win32Deklijst : Win32
    {
        [ThreadStatic]
        private static Win32Deklijst singleton;

        public static Win32Deklijst Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Deklijst();
            }
            return singleton;
        }
        /*
         function lst_AB_Deklijsten(pProgID, pProgramID, pSoortBestand: integer;
                           pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                           pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                           pCallback: Pointer;
                           pDtBegin, pDtEnd: TDateTime;
                           pSort: Integer): Integer;
         */
        public int call_lst_AB_Deklijsten(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           string pResourceFolder, string pTaalcode, int pTaalnr,
										   pCallback ReadDataProc, 
										   DateTime pDtBegin, DateTime pDtEnd,
										   int pSort)
        {
            lock (typeof(Win32Deklijst))
            {
                string sFilename = "DEKLIJST.dll";

                lst_AB_Deklijsten handle = (lst_AB_Deklijsten)ExecuteProcedureDLL(typeof(lst_AB_Deklijsten), sFilename, "lst_AB_Deklijsten");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, 
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc, 
                                 pDtBegin, pDtEnd, pSort);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_Deklijsten(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       string pResourceFolder, string pTaalcode, int pTaalnr,
									   pCallback ReadDataProc, 
									   DateTime pDtBegin, DateTime pDtEnd, 
									   int pSort);	
    }
}
