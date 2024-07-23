using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32SELFATH : Win32
    {
        [ThreadStatic]
        private static Win32SELFATH singleton;

        public static Win32SELFATH Init()
        {
            if (singleton == null)
            {
                singleton = new Win32SELFATH();
            }
            return singleton;
        }


        /*
         function lst_AB_Kruisingsoverzicht(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallBack: Pointer;
              pGeselecteerdeBulls: pAnsiChar;
              pAlleenRelatie: Boolean): Integer; stdcall; export; 

         */
        public int call_lst_AB_Kruisingsoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                  string pResourceFolder, string pTaalcode, int pTaalnr,                                      
                                                  pCallback ReadDataProc,
                                                  string pGeselecteerdeBulls,
                                                  bool pAlleenRelatie)
        {
            lock (typeof(Win32SELFATH))
            {
                string sFilename = "SELFATH.dll";

                lst_AB_Kruisingsoverzicht handle = (lst_AB_Kruisingsoverzicht)ExecuteProcedureDLL(
                        typeof(lst_AB_Kruisingsoverzicht), sFilename, "lst_AB_Kruisingsoverzicht");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pGeselecteerdeBulls, pAlleenRelatie);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        /*
         function lst_AB_Verwantschap(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallBack: Pointer;
              pGeselecteerdeBulls: PAnsiChar;
              pAlleenRelatie, pRelatieAanwezig: Boolean): Integer; stdcall; export;
         */
        public int call_lst_AB_Verwantschap(int pProgID, int pProgramID, int pSoortBestand,
                                            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                            string pResourceFolder, string pTaalcode, int pTaalnr,
                                            pCallback ReadDataProc,
                                            string pGeselecteerdeBulls,
                                            bool pAlleenRelatie, bool pRelatieAanwezig)
        {
            lock (typeof(Win32SELFATH))
            {
                string sFilename = "SELFATH.dll";

                lst_AB_Verwantschap handle = (lst_AB_Verwantschap)ExecuteProcedureDLL(
                        typeof(lst_AB_Verwantschap), sFilename, "lst_AB_Verwantschap");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pGeselecteerdeBulls, pAlleenRelatie, pRelatieAanwezig);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_Kruisingsoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               string pResourceFolder, string pTaalcode, int pTaalnr,
                                               pCallback ReadDataProc,
                                               string pGeselecteerdeBulls,
                                               bool pAlleenRelatie);

        delegate int lst_AB_Verwantschap(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         string pResourceFolder, string pTaalcode, int pTaalnr,
                                         pCallback ReadDataProc,
                                         string pGeselecteerdeBulls,
                                         bool pAlleenRelatie, bool pRelatieAanwezig);

        /*
         function lst_AB_InteeltCoefficient(
                pProgID, pProgramID, pSoortBestand: Integer;
                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                pCallBack: Pointer;
                pBullid: integer; pAniids: PAnsiChar): Integer; stdcall; export;

                pBullid = aniid van de gekozen ram
                pAniids = aniid's van de geselecteerd ooien gescheiden door puntkomma.
         
         * 19-10-2016
         * function lst_AB_InteeltCoefficient(
                    pProgID, pProgramID, pSoortBestand: Integer;
                    pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                    pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                    pCallBack: Pointer;
                    pBullid: integer; pAniids: PAnsiChar): Integer; stdcall; export; 
         */

        public int call_lst_AB_InteeltCoefficient(int pProgID, int pProgramID, int pSoortBestand,
                                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                    string pResourceFolder, string pTaalcode, int pTaalnr,
                                    pCallback ReadDataProc,
                                    int pBullid, string pAniids)
        {
            lock (typeof(Win32SELFATH))
            {
                string sFilename = "SELFATH.dll";

                lst_AB_InteeltCoefficient handle = (lst_AB_InteeltCoefficient)ExecuteProcedureDLL(
                        typeof(lst_AB_InteeltCoefficient), sFilename, "lst_AB_InteeltCoefficient");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pBullid, pAniids);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_InteeltCoefficient(int pProgID, int pProgramID, int pSoortBestand,
                                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                    string pResourceFolder, string pTaalcode, int pTaalnr,
                                    pCallback ReadDataProc,
                                    int pBullid, string pAniids);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
