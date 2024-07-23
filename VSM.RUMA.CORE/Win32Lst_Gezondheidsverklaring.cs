using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Lst_Gezondheidsverklaring : Win32
    {

        [ThreadStatic]
        private static Win32Lst_Gezondheidsverklaring singleton;

        public static Win32Lst_Gezondheidsverklaring Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Lst_Gezondheidsverklaring();
            }
            return singleton;
        }
        /* BUG 1110
         
         * G:\agrobase\Website_Dev\lib\Lst_Gezondheidsverklaring.dll

         
         function lst_Gezondheidsverklaring_Afdrukken(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer;
              pRavBestand: PAnsiChar
              ): Integer; stdcall; Export;


         */
        public int call_lst_Gezondheidsverklaring_Afdrukken(int pProgID, int pProgramID, int pSoortBestand,
                                              string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              pCallback ReadDataProc,
                                              string pRavBestand )
        {
            lock (typeof(Win32Lst_Gezondheidsverklaring))
            {
                string sFilename = "Lst_Gezondheidsverklaring.dll";

                lst_Gezondheidsverklaring_Afdrukken handle = (lst_Gezondheidsverklaring_Afdrukken)ExecuteProcedureDLL(typeof(lst_Gezondheidsverklaring_Afdrukken), sFilename, "lst_Gezondheidsverklaring_Afdrukken");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pRavBestand);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_Gezondheidsverklaring_Afdrukken(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           pCallback ReadDataProc,
                                           string pRavBestand);

        public delegate void pCallback(int PercDone, string Msg); 
    }
}
