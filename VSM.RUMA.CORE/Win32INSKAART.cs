using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32INSKAART:Win32
    {
        [ThreadStatic]
        private static Win32INSKAART singleton;

        public static Win32INSKAART Init()
        {
            if (singleton == null)
            {
                singleton = new Win32INSKAART();
            }
            return singleton;
        }

        public Win32INSKAART()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);
        /*
         function lst_AB_Registratiekaart(
            pProgID, pProgramID, pSoortBestand: Integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pCallBack: Pointer; pAniid: integer): Integer; stdcall;
         */

        public int call_lst_AB_Registratiekaart(int pProgID, int pProgramID, int pSoortBestand,
                   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                   pCallback ReadDataProc,
                   int pAniid)
        {
            lock (typeof(Win32INSKAART))
            {
                lst_AB_Registratiekaart handle = (lst_AB_Registratiekaart)ExecuteProcedureDLL(typeof(lst_AB_Registratiekaart), "INSKAART.DLL", "lst_AB_Registratiekaart");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pAniid);

                FreeDLL("INSKAART.DLL");
                return tmp;
            }
        }




        delegate int lst_AB_Registratiekaart(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               int pAniid);
    }
}
