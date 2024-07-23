using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32VERVOERD: Win32
    {
        [ThreadStatic]
        private static Win32VERVOERD singleton;

        public static Win32VERVOERD Init()
        {
            if (singleton == null)
            {
                singleton = new Win32VERVOERD();
            }
            return singleton;
        }
        public delegate void pCallback(int PercDone, string Msg);

        //BUG 1945 
        /*
         function lst_AB_VervoersdocumentSG(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pTransportDatumTijd, pTransportDuur: TDateTime;
                  pThrIdBestemming, pUBNidBestamming,
                  pThrIdTransporteur, pUBNidTransporteur,
                  pIdTransportmiddel: Integer;
                  pAniIds: PAnsiChar): Integer; stdcall;

         */

        public int call_lst_AB_VervoersdocumentSG(int pProgID, int pProgramID, int pSoortBestand,
                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                    pCallback ReadDataProc,
                    DateTime pTransportDatumTijd, DateTime pTransportDuur,
                    int pThrIdBestemming, int pUBNidBestamming,
                    int pThrIdTransporteur, int pUBNidTransporteur,
                    int pIdTransportmiddel,
                    string pAniIds)
        {
            lock (typeof(Win32GEMPROD))
            {
                string sFilename = "VERVOERD.dll";

                lst_AB_VervoersdocumentSG handle = (lst_AB_VervoersdocumentSG)ExecuteProcedureDLL(typeof(lst_AB_VervoersdocumentSG), sFilename, "lst_AB_VervoersdocumentSG");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        ReadDataProc,
                        pTransportDatumTijd, pTransportDuur,
                        pThrIdBestemming, pUBNidBestamming,
                        pThrIdTransporteur, pUBNidTransporteur,
                        pIdTransportmiddel,
                        pAniIds);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_VervoersdocumentSG(int pProgID, int pProgramID, int pSoortBestand,
                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                    pCallback ReadDataProc,
                    DateTime pTransportDatumTijd, DateTime pTransportDuur,
                    int pThrIdBestemming, int pUBNidBestamming,
                    int pThrIdTransporteur, int pUBNidTransporteur,
                    int pIdTransportmiddel,
                    string pAniIds);

        /*
         function lst_AB_VervoersdocumentSG_DK(
                      pProgID, pProgramID, pSoortBestand: Integer;
                      pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                      pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                      pCallBack: Pointer;
                      pHerdNr: PAnsiChar; pAanmaakDatum: TDateTime;
                      pAniIds: PAnsiChar): Integer; stdcall;


                    pHerdNr: zal meestal ubn zijn (zie hieronder)
                    pAanmaakDatum: voor de zekerheid, zal denk ik altijd vandaag zijn
                    pAniids: (punt)komma gescheiden aniid's die op de lijst moeten komen.
         */

        public int call_lst_AB_VervoersdocumentSG_DK(int pProgID, int pProgramID, int pSoortBestand,
            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
            string pResourceFolder,string pTaalcode, int pTaalnr,
            pCallback ReadDataProc,
            string pHerdNr, DateTime pAanmaakDatum,
            string pAniIds)
        {
            lock (typeof(Win32GEMPROD))
            {
                string sFilename = "VERVOERD.dll";

                lst_AB_VervoersdocumentSG_DK handle = (lst_AB_VervoersdocumentSG_DK)ExecuteProcedureDLL(typeof(lst_AB_VervoersdocumentSG_DK), sFilename, "lst_AB_VervoersdocumentSG_DK");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pHerdNr, pAanmaakDatum,
                        pAniIds);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_VervoersdocumentSG_DK(int pProgID, int pProgramID, int pSoortBestand,
                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                    string pResourceFolder, string pTaalcode, int pTaalnr,
                    pCallback ReadDataProc,
                    string pHerdNr, DateTime pAanmaakDatum,
                    string pAniIds);
    }
}
