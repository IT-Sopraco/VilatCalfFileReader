using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
	public class Win32Weeglyst : Win32
	{
        [ThreadStatic]
		private static Win32Weeglyst singleton;

		public static Win32Weeglyst Init()
		{
			if (singleton == null)
			{
				singleton = new Win32Weeglyst();
			}
			return singleton;
		}

        public Win32Weeglyst()
            : base(false)
        {
        }

        /*
         function lst_AB_Weeglijsten(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer;
              pEigenSelectie, pAanwezig, pAfgevoerd, pInclAfgMoeders, pWegingen : Boolean;
              pStalNr, pGroepNr, pSort: Integer; pHoknr: PAnsiChar): Integer; 
            *  
            Erik had wel die parameter erbij staan maar daar deed ie nog niet veel mee. 
         *  Ik heb nu een parameter pAniids toegevoegd waar je de aniid's van de geselecteerde dieren in moet zetten, 
         *  gescheiden door een komma.

            function lst_AB_Weeglijsten(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer;
              pEigenSelectie, pAanwezig, pAfgevoerd, pInclAfgMoeders, pWegingen : Boolean;
              pStalNr, pGroepNr, pSort: Integer; pHoknr, pAniids: PAnsiChar): Integer; stdcall; export; 
         * 
         * 
         * function lst_AB_Weeglijsten(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pEigenSelectie, pAanwezig, pAfgevoerd, pInclAfgMoeders, pWegingen : Boolean;
              pStalNr, pGroepNr, pSort: Integer; pHoknr, pAniids: PAnsiChar): Integer; stdcall;
         */

        public int call_lst_AB_Weeglijsten(int pProgID, int pProgramID, int pSoortBestand,
										   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
			                               string pResourceFolder,string pTaalcode, int pTaalnr,							   
                                           pCallback ReadDataProc, 
										   bool pEigenSelectie, bool pAanwezig, bool pAfgevoerd, bool pInclAfgMoeders, bool pWegingen,
                                           int pStalNr, int pGroepNr, int pSort, string pHoknr, string pAniids)
		{
            lock (typeof(Win32Weeglyst))
            {
                string sFilename = "WEEGLYST.dll";

                lst_AB_Weeglijsten handle = (lst_AB_Weeglijsten)ExecuteProcedureDLL(typeof(lst_AB_Weeglijsten), 
                                            sFilename, "lst_AB_Weeglijsten");

                int tmpint = 0;
                tmpint = handle(pProgID, pProgramID, pSoortBestand,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                pResourceFolder,pTaalcode, pTaalnr,
                                ReadDataProc,
                                pEigenSelectie, pAanwezig, pAfgevoerd, pInclAfgMoeders, pWegingen,
                                pStalNr, pGroepNr, pSort, pHoknr, pAniids);

                FreeDLL(sFilename);
                return tmpint;
            }
		}

		public delegate void pCallback(int PercDone, string Msg);
        
		delegate int lst_AB_Weeglijsten(int pProgID, int pProgramID, int pSoortBestand,
										string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                        string pResourceFolder, string pTaalcode, int pTaalnr,
										pCallback ReadDataProc, 
										bool pEigenSelectie, bool pAanwezig, bool pAfgevoerd, bool pInclAfgMoeders, bool pWegingen,
                                        int pStalNr, int pGroepNr, int pSort, string pHoknr, string pAniids);
	}
}
