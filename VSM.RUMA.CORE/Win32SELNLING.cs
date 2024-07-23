using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
	public class Win32SELNLING : Win32
	{
        [ThreadStatic]
		private static Win32SELNLING singleton;

		public static Win32SELNLING Init()
		{
			if (singleton == null)
			{
				singleton = new Win32SELNLING();
			}
			return singleton;
		}
        public Win32SELNLING()
            : base(false)
        {
        }

        /*
         function lst_AB_selectie_Nling(
                pProgID, pProgramID, pSoortBestand: integer;
                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                pCallback: Pointer;
                pSort, pAniling, pDierAanwezig: Integer;
                pDtBegin, pDtEnd: TDateTime;
                pPerHok: Integer; pAlleenHok: PAnsiChar): Integer; stdcall;
         */

        public int call_lst_AB_selectie_Nling(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                              string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              string pResourceFolder, string pTaalcode, int pTaalnr,
											  pCallback ReadDataProc, 
											  int pSort, int pAniling, int pDierAanwezig,
											  DateTime pDtBegin, DateTime pDtEnd,
                                              int pPerHok, string pAlleenHok)
		{
            lock (typeof(Win32SELNLING))
            {
                string sFilename = "SELNLING.dll";

                lst_AB_selectie_Nling handle = (lst_AB_selectie_Nling)ExecuteProcedureDLL(typeof(lst_AB_selectie_Nling), 
                                                sFilename, 
                                                "lst_AB_selectie_Nling");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pSort, pAniling, pDierAanwezig,
                                 pDtBegin, pDtEnd,
                                 pPerHok, pAlleenHok);

                FreeDLL(sFilename);
                return tmp;
            }
		}
        
		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_selectie_Nling(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                           string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           string pResourceFolder, string pTaalcode, int pTaalnr,
										   pCallback ReadDataProc, 
										   int pSort, int pAniling, int pDierAanwezig,
										   DateTime pDtBegin, DateTime pDtEnd,
                                           int pPerHok, string pAlleenHok);				
	}
}
