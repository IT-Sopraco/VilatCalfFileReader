using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32KKAART : Win32
    {
        [ThreadStatic]
        private static Win32KKAART singleton;

        public static Win32KKAART Init()
        {
            if (singleton == null)
            {
                singleton = new Win32KKAART();
            }
            return singleton;
        }

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_Koekaart(
              pProgID, pLogoID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pAniId: Integer;
              pSoortLijst: Integer): Integer; stdcall; export;

            pLogoID is het nummer van het logo dat je op de lijst wilt hebben (wat programid dus eerst deed). 
             */

        public int call_lst_AB_Koekaart(int pProgID, int pLogoID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        int pAniId, int pSoortLijst)
        {
            lock (typeof(Win32KKAART))
            {
                string sFilename = "KKAART.dll";

                lst_AB_Koekaart handle =
                    (lst_AB_Koekaart)ExecuteProcedureDLL(
                        typeof(lst_AB_Koekaart), sFilename, "lst_AB_Koekaart");

                int tmp = handle(pProgID, pLogoID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pAniId, pSoortLijst);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_Koekaart(int pProgID, int pLogoID, int pSoortBestand,
                                     string pUbnnr, string pHostName, string pUserName, string pPassword,
                                     string pBestand, string pLog,
                                     pCallback ReadDataProc,
                                     int pAniId, int pSoortLijst);

        /*
         function lst_AB_Dierkaart(
            pProgID, pLogoID, pSoortBestand: Integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pLandnr: integer; pCallBack: Pointer;
            pAniId: Integer): Integer; stdcall;

Voor Vilatca offline is een aparte dierkaart gemaakt (ook bruikbaar voor ruma stierenprogramma's)
pLogoID = 81 : mesterversie (zonder bedragen en kwaliteit)
pLogoID = 810 : admin versie (met bedragen en kwaliteit)

         */

        public int call_lst_AB_Dierkaart(int pProgID, int pLogoID, int pSoortBestand,
                                string pUbnnr, string pHostName, string pUserName, string pPassword,
                                string pBestand, string pLog, int pLandnr,
                                pCallback ReadDataProc,
                                int pAniId)
        {
            lock (typeof(Win32KKAART))
            {
                string sFilename = "KKAART.dll";

                lst_AB_Dierkaart handle =
                    (lst_AB_Dierkaart)ExecuteProcedureDLL(
                        typeof(lst_AB_Dierkaart), sFilename, "lst_AB_Dierkaart");

                int tmp = handle(pProgID, pLogoID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), 
                                 pBestand, pLog, pLandnr,
                                 ReadDataProc,
                                 pAniId);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_Dierkaart(int pProgID, int pLogoID, int pSoortBestand,
                                     string pUbnnr, string pHostName, string pUserName, string pPassword,
                                     string pBestand, string pLog, int pLandnr,
                                     pCallback ReadDataProc,
                                     int pAniId);
    }
}
