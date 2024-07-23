using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32lstPredikaat : Win32
    {
        [ThreadStatic]
        private static Win32lstPredikaat singleton;

        public static Win32lstPredikaat Init()
        {
            if (singleton == null)
            {
                singleton = new Win32lstPredikaat();
            }
            return singleton;
        }

        public int call_lst_AB_Predikaatlijst(int pProgID, int pProgramID, int pSoortBestand,
                                              string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              pCallback ReadDataProc,
                                              int pAniId, int pSoortLijst)
        {
            lock (typeof(Win32lstPredikaat))
            {
                lst_AB_Predikaatlijst handle = (lst_AB_Predikaatlijst)ExecuteProcedureDLL(typeof(lst_AB_Predikaatlijst), "lstPredikaat.dll", "lst_AB_Predikaatlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pAniId, pSoortLijst);

                FreeDLL("lstPredikaat.dll");
                return tmp;
            }
        }

        delegate int lst_AB_Predikaatlijst(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           pCallback ReadDataProc,
                                           int pAniId, int pSoortLijst);

        public delegate void pCallback(int PercDone, string Msg);

    }
}
