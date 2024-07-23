using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32stallijst : Win32
	{
        //[ThreadStatic]
        //private static Win32stallijst singleton;

        //public static Win32stallijst Init()
        //{
        //    if (singleton == null)
        //    {
        //        singleton = new Win32stallijst();
        //    }
        //    return singleton;
        //}
        public Win32stallijst()
            : base(false)
        {
        }
        /*
         * function lst_AB_RundveeStallijst(
                  pProgID, pProgramID, pSoortBestand: integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pPeildatum: TDateTime; pShowMedeEigenaar,
                  pGroupId, pSort, pToonKeto, pAlleenAfgekalfd: Integer): Integer; stdcall;

                    pToonKeto: 0/1
                    pAlleenAfgekalfd: 0/1
         * 
         * function lst_AB_RundveeStallijst(
                pProgID, pProgramID, pSoortBestand: integer;
                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog,
                pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                pCallBack: Pointer;
                pPeildatum: TDateTime; pShowMedeEigenaar,
                pGroupId, pSort, pToonKeto, pAlleenAfgekalfd: Integer): Integer;
         */
        public int call_lst_AB_RundveeStallijst(int pProgID, int pProgramID, int pSoortBestand,
                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                string pResourceFolder, string pTaalcode, int pTaalnr,                                     
                                pCallback ReadDataProc,
                                DateTime pPeildatum, int pShowMedeEigenaar, int pGroupId, int pSort,int pToonKeto,int pAlleenAfgekalfd)
        {
            lock (typeof(Win32stallijst))
            {

                lst_AB_RundveeStallijst handle = (lst_AB_RundveeStallijst)ExecuteProcedureDLL(typeof(lst_AB_RundveeStallijst), "LSTSTALLIJST.DLL", "lst_AB_RundveeStallijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pPeildatum, pShowMedeEigenaar, pGroupId, pSort, pToonKeto, pAlleenAfgekalfd);

                FreeDLL("LSTSTALLIJST.DLL");
                return tmp;
            }
        }

        /*
         function lst_AB_SchapenStallijst(
            pProgID, pProgramID, pSoortBestand: Integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pCallBack: Pointer;
            pPeildatum: TDateTime;
            pShowMedeEigenaar, pAniSex, pAniScrapie, pSoortLijst, pSort, pShowAniAlternateNumber: Integer;
            pGebDatBegin, pGebDatEnd: TDateTime;
            pPerHok, pAlleenHok: boolean; pHoknr: pAnsiChar): Integer; stdcall; export; 

        function lst_AB_SchapenStallijst(
            pProgID, pProgramID, pSoortBestand: Integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
            pCallBack: Pointer;
            pPeildatum: TDateTime;
            pShowMedeEigenaar, pAniSex, pAniScrapie, pSoortLijst, pSort, pShowAniAlternateNumber: Integer;
            pGebDatBegin, pGebDatEnd: TDateTime; pPerHok, pAlleenHok: boolean;
            pHoknr: pAnsiChar; pLevnrNaam: integer): Integer; stdcall;
         * 
         */

        public int call_lst_AB_SchapenStallijst(int pProgID, int pProgramID, int pSoortBestand,
                                              string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              string pResourceFolder, string pTaalcode, int pTaalnr,
                                              pCallback ReadDataProc,
                                              DateTime pPeildatum, int pShowMedeEigenaar, int pAniSex, int pAniScrapie, int pSoortLijst, int pSort, int pShowAniAlternateNumber,
                                              DateTime pGebDatBegin, DateTime pGebDatEnd,
                                              bool pPerHok, bool pAlleenHok, string pHoknr, int pLevnrNaam)
        {

            lock (typeof(Win32stallijst))
            {

                lst_AB_SchapenStallijst handle = (lst_AB_SchapenStallijst)ExecuteProcedureDLL(typeof(lst_AB_SchapenStallijst), "LSTSTALLIJST.DLL", "lst_AB_SchapenStallijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pPeildatum, pShowMedeEigenaar, pAniSex, pAniScrapie, pSoortLijst, pSort, pShowAniAlternateNumber,
                                 pGebDatBegin, pGebDatEnd,
                                 pPerHok, pAlleenHok, pHoknr, pLevnrNaam);

                FreeDLL("LSTSTALLIJST.DLL");
                return tmp;
            }
        }

		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_RundveeStallijst(int pProgID, int pProgramID, int pSoortBestand,
											  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              string pResourceFolder, string pTaalcode, int pTaalnr,
											  pCallback ReadDataProc,
											  DateTime pPeildatum, int pShowMedeEigenaar, int pGroupId, int pSort,int pToonKeto,int pAlleenAfgekalfd);


		delegate int lst_AB_SchapenStallijst(int pProgID, int pProgramID, int pSoortBestand,
											  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              string pResourceFolder, string pTaalcode, int pTaalnr,
											  pCallback ReadDataProc,
											  DateTime pPeildatum, int pShowMedeEigenaar, int pAniSex, int pAniScrapie, int pSoortLijst, int pSort, int pShowAniAlternateNumber,
                                              DateTime pGebDatBegin, DateTime pGebDatEnd,
                                              bool pPerHok, bool pAlleenHok, string pHoknr, int pLevnrNaam);
	}

}
