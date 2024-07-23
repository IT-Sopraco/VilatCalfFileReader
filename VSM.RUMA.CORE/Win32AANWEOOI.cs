using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32AANWEOOI : Win32
    {
        [ThreadStatic]
        private static Win32AANWEOOI singleton;

        public static Win32AANWEOOI Init()
        {
            if (singleton == null)
            {
                singleton = new Win32AANWEOOI();
            }
            return singleton;
        }

        /*
         function lst_AB_Deklijsten_Leeg(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer): Integer; stdcall; 
         */
        public int call_lst_AB_Deklijsten_Leeg(
                int pProgID, int pProgramID, int pSoortBestand,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                string pResourceFolder, string pTaalcode, int pTaalnr,
                pCallback ReadDataProc)
        {
            lock (typeof(Win32AANWEOOI))
            {
                string sFilename = "AANWEOOI.DLL";

                lst_AB_Deklijsten_Leeg handle = (lst_AB_Deklijsten_Leeg)ExecuteProcedureDLL(typeof(lst_AB_Deklijsten_Leeg), sFilename, "lst_AB_Deklijsten_Leeg");
                //Decodeer_PW(pPassword)
                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_Deklijsten_Leeg(int pProgID, int pProgramID, int pSoortBestand,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
                string pResourceFolder, string pTaalcode, int pTaalnr,
                pCallback ReadDataProc);
    }
}
