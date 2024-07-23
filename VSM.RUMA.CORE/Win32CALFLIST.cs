using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32CALFLIST : Win32
	{
        [ThreadStatic]
		private static Win32CALFLIST singleton;

		public static Win32CALFLIST Init()
		{
			if (singleton == null)
			{
				singleton = new Win32CALFLIST();
			}
			return singleton;
		}
      
        public Win32CALFLIST()
            : base(false)
        {
        }

        /*
         * 19-10-2016
                 function lst_AB_Wachttijdlijst(
                    pProgID, pProgramID, pSoortBestand: Integer;
                    pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                    pResourceFolder, pTaalcode: PAnsiChar; pTaalnr, pLandnr: integer;
                    pCallback: Pointer; pDate: TDateTime;
                    pSort, pPerhok: Integer; pAlleenHok: pAnsiChar): Integer; stdcall; 
         */
		public int call_lst_AB_Wachttijdlijst(int pProgID, int pProgramID, int pSoortBestand,
											  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              string pResourceFolder, string pTaalcode, int pTaalnr, int pLandnr,
											  pCallback ReadDataProc, 
											  DateTime pDate, int pSort, int pPerHok, string pAlleenHok)
		{
            lock (typeof(Win32CALFLIST))
            {
                lst_AB_Wachttijdlijst handle = (lst_AB_Wachttijdlijst)ExecuteProcedureDLL(typeof(lst_AB_Wachttijdlijst), "CALFLIST.dll", "lst_AB_Wachttijdlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr, pLandnr,
                                 ReadDataProc,
                                 pDate, pSort, pPerHok, pAlleenHok);

                FreeDLL("CALFLIST.dll");
                return tmp;
            }
		}

        public int call_lst_AB_Afleverlijst(int pProgID, int pProgramID, int pSoortBestand,
											string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
											pCallback ReadDataProc, 
											DateTime pDtBegin, DateTime pDtEnd, int pGroupId, int pSort)
        {
            lock (typeof(Win32CALFLIST))
            {
                lst_AB_Afleverlijst handle = (lst_AB_Afleverlijst)ExecuteProcedureDLL(typeof(lst_AB_Afleverlijst), "CALFLIST.dll", "lst_AB_Afleverlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd, pGroupId, pSort);

                FreeDLL("CALFLIST.dll");
                return tmp;
            }        
        }

        /*
         function lst_AB_Behandelinglijst(
                pProgID, pProgramID, pSoortBestand: Integer;
                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                pCallback: Pointer;
                pDtBegin, pDtEnd: TDateTime; pTreKind, pPlanId: Integer;
                pLaatsteBehandeling, pDierZonder, pDierAanwezig: Boolean; pSort,
                pGroupId, pPerHok: Integer; pAlleenHok: PChar): Integer; stdcall;
         */

        public int call_lst_AB_Behandelinglijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                DateTime pDtBegin, DateTime pDtEnd, int pTreKind, int pPlanId,
                                                bool pLaatsteBehandeling, bool pDierZonder, bool pDierAanwezig, int pSort, int pGroupId,
                                                int pPerHok, string pAlleenHok)
        {                         
            lock (typeof(Win32CALFLIST))
            {
                lst_AB_Behandelinglijst handle = (lst_AB_Behandelinglijst)ExecuteProcedureDLL(typeof(lst_AB_Behandelinglijst), "CALFLIST.dll", "lst_AB_Behandelinglijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd, pTreKind, pPlanId,
                                 pLaatsteBehandeling, pDierZonder, pDierAanwezig, pSort, pGroupId,
                                 pPerHok, pAlleenHok);

                FreeDLL("CALFLIST.dll");
                return tmp;
            }
        }

		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_Wachttijdlijst(int pProgID, int pProgramID, int pSoortBestand,
					                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           string pResourceFolder, string pTaalcode, int pTaalnr, int pLandnr,
										   pCallback ReadDataProc, 
										   DateTime pDate, int pSort, int pPerHok, string pAlleenHok);

        delegate int lst_AB_Afleverlijst(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         pCallback ReadDataProc,
                                         DateTime pDtBegin, DateTime pDtEnd, int pGroupId, int pSort);



        delegate int lst_AB_Behandelinglijst(int pProgID, int pProgramID, int pSoortBestand,
                                             string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                             string pResourceFolder, string pTaalcode, int pTaalnr,
                                             pCallback ReadDataProc,
                                             DateTime pDtBegin, DateTime pDtEnd, int pTreKind, int pPlanId,
                                             bool pLaatsteBehandeling, bool pDierZonder, bool pDierAanwezig, int pSort, int pGroupId, int pPerHok, string pAlleenHok);

    }
}
