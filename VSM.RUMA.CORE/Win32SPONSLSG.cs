using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32SPONSLSG : Win32
    {
        //BUG 1804
        [ThreadStatic]
        private static Win32SPONSLSG singleton;

        public static Win32SPONSLSG Init()
        {
            if (singleton == null)
            {
                singleton = new Win32SPONSLSG();
            }
            return singleton;
        }

        public int call_lst_AB_Sponslijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                pCallback ReadDataProc,
                                                int pSortering)
        {
            lock (typeof(Win32SPONSLSG))
            {
                string sFilename = "SPONSLSG.dll";

                lst_AB_Sponslijst handle = (lst_AB_Sponslijst)ExecuteProcedureDLL(typeof(lst_AB_Sponslijst), sFilename, "lst_AB_Sponslijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                        ReadDataProc,
                        pSortering);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_Sponslijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                pCallback ReadDataProc,
                                                int pSortering);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
