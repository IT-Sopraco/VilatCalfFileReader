using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32LstFokExt : Win32
    {
        [ThreadStatic]
        private static Win32LstFokExt singleton;

        public static Win32LstFokExt Init()
        {
            if (singleton == null)
            {
                singleton = new Win32LstFokExt();
            }
            return singleton;
        }

        public int call_lst_AB_Fokwaardenoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                                   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                   pCallback ReadDataProc,
                                                   DateTime pDatum,
                                                   int pSort, int pDierAanwezig)
        {
            lock (typeof(Win32LstFokExt))
            {
                string sFilename = "LSTFOKEXT.dll";

                lst_AB_Fokwaardenoverzicht handle = (lst_AB_Fokwaardenoverzicht)ExecuteProcedureDLL(typeof(lst_AB_Fokwaardenoverzicht), sFilename, "lst_AB_Fokwaardenoverzicht");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDatum, pSort, pDierAanwezig);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public int call_lst_AB_Exterieurwaardenoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                         pCallback ReadDataProc,
                                                         int pExtJaar, int pSort, int pDierAanwezig, int pSoortLijst) 
        {
            lock (typeof(Win32LstFokExt))
            {
                string sFilename = "LSTFOKEXT.dll";

                lst_AB_Exterieurwaardenoverzicht handle = (lst_AB_Exterieurwaardenoverzicht)ExecuteProcedureDLL(typeof(lst_AB_Exterieurwaardenoverzicht), sFilename, "lst_AB_Exterieurwaardenoverzicht");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pExtJaar, pSort, pDierAanwezig, pSoortLijst);

                FreeDLL(sFilename);
                return tmp;
            }        
        }

        delegate int lst_AB_Fokwaardenoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                pCallback ReadDataProc,
                                                DateTime pDatum, 
                                                int pSort, int pDierAanwezig);

        delegate int lst_AB_Exterieurwaardenoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                                      string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                      pCallback ReadDataProc,
                                                      int pExtJaar, int pSort, int pDierAanwezig, int pSoortLijst);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
