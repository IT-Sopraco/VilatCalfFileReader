using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32EdinrsResponders :Win32
    {
        [ThreadStatic]
        private static Win32EdinrsResponders singleton;

        public static Win32EdinrsResponders Init()
        {
            if (singleton == null)
            {
                singleton = new Win32EdinrsResponders();
            }
            return singleton;
        }
        /*
        function AB_importResponders(
          pProgID, pProgramID: integer;
          pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
          pCallback: Pointer; pKoppelingnr: integer;
          pInputFile: PAnsiChar): Integer; stdcall;
        */

        public int call_AB_importResponders(int pProgID, int pProgramID, 
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pLog,
                                        pCallback ReadDataProc,
                                        int pKoppelingnr, string pInputFile)
        {
            lock (typeof(Win32EdinrsResponders))
            {
                string sFilename = "IMPEDINRS";

                AB_importResponders handle =
                    (AB_importResponders)ExecuteProcedureDLL(
                        typeof(AB_importResponders), sFilename, "AB_importResponders");

                int tmp = handle(pProgID, pProgramID, 
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), 
                                 pLog,
                                 ReadDataProc,
                                 pKoppelingnr, pInputFile);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int AB_importResponders(int pProgID, int pProgramID, 
                                     string pUbnnr, string pHostName, string pUserName, string pPassword,
                                     string pLog,
                                     pCallback ReadDataProc,
                                     int pKoppelingnr, string pInputFile);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
