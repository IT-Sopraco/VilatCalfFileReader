using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Lst_Bloedonderzoeken : Win32
    {
        [ThreadStatic]
        private static Win32Lst_Bloedonderzoeken singleton;

        public static Win32Lst_Bloedonderzoeken Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Lst_Bloedonderzoeken();
            }
            return singleton;
        }

        public int call_lst_AB_BloedonderzoekAuthorisatie(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword, 
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        int pFarmId, int pBloAuthorized, int pBloAuthorizedByThrID,
                                        DateTime pDtMonster,
                                        string pDierZiekteCSV,
                                        int pTypeUitslag,
                                        string pInzendNummer,
                                        string pRedenInzending,
                                        string pOpmerking)
        {
            lock (typeof(Win32Lst_Bloedonderzoeken))
            {
                string sFilename = "Lst_Bloedonderzoeken.dll";

                lst_AB_BloedonderzoekAuthorisatie handle = 
                    (lst_AB_BloedonderzoekAuthorisatie)ExecuteProcedureDLL(
                        typeof(lst_AB_BloedonderzoekAuthorisatie), sFilename, "lst_AB_BloedonderzoekAuthorisatie");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pFarmId, pBloAuthorized, pBloAuthorizedByThrID,
                                 pDtMonster, 
                                 pDierZiekteCSV, 
                                 pTypeUitslag,
                                 pInzendNummer,
                                 pRedenInzending,
                                 pOpmerking);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_BloedonderzoekAuthorisatie(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword, 
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        int pFarmId, int pBloAuthorized, int pBloAuthorizedByThrID,
                                        DateTime pDtMonster,
                                        string pDierZiekteCSV,
                                        int pTypeUitslag,
                                        string pInzendNummer,
                                        string pRedenInzending,
                                        string pOpmerking);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
