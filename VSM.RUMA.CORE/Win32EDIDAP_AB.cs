using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32EDIDAP_AB : Win32
    {
        [ThreadStatic]
        private static Win32EDIDAP_AB singleton;

        public static Win32EDIDAP_AB Init()
        {
            if (singleton == null)
            {
                singleton = new Win32EDIDAP_AB();
            }
            return singleton;
        }
        //BUG 1917 Edidap export
        /*
         function AB_ExportToEdiDap(
              pProgID, pProgramID: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
              pCallBack: Pointer; pDatumVanaf: TDateTime;
              pEdiDapBestand: PAnsiChar): Integer; stdcall;

         */
        public int call_AB_ExportToEdiDap(int pProgID, int pProgramID, 
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                        pCallback ReadDataProc, DateTime pDatumVanaf,
                        string pEdiDapBestand)
        {
            lock (typeof(Win32EDIDAP_AB))
            {
                string sFilename = "EDIDAP_AB.dll";

                AB_ExportToEdiDap handle = (AB_ExportToEdiDap)ExecuteProcedureDLL(typeof(AB_ExportToEdiDap), sFilename, "AB_ExportToEdiDap");

                int tmp = handle(pProgID, pProgramID,
                        pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pLog,
                        ReadDataProc, pDatumVanaf,
                        pEdiDapBestand);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int AB_ExportToEdiDap(int pProgID, int pProgramID,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                        pCallback ReadDataProc, DateTime pDatumVanaf,
                        string pEdiDapBestand);


        public delegate void pCallback(int PercDone, string Msg);
    }
}
