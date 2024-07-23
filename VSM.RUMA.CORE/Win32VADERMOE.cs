using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
	public class Win32VADERMOE : Win32
	{
        [ThreadStatic]
		private static Win32VADERMOE singleton;

		public static Win32VADERMOE Init()
		{
			if (singleton == null)
			{
				singleton = new Win32VADERMOE();
			}
			return singleton;
		}

        /*
         function lst_AB_Vadermoederlijst(pProgID, pProgramID, pSoortBestand: integer;
                                 pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                                 pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                                 pCallback: Pointer;
                                 pDtBegin, pDtEnd: TDateTime;
                                 pSort, pDierAanwezig: Integer): Integer; export; stdcall;
         */

        public int call_lst_AB_Vadermoederlijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                DateTime pDtBegin, DateTime pDtEnd,
                                                int pSort, int pDierAanwezig)     
        {
            lock (typeof(Win32VADERMOE))
            {
                string sFilename = "VADERMOE.dll";
        
                lst_AB_Vadermoederlijst handle = (lst_AB_Vadermoederlijst)ExecuteProcedureDLL(typeof(lst_AB_Vadermoederlijst), sFilename, "lst_AB_Vadermoederlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd, pSort, pDierAanwezig);

                FreeDLL(sFilename);
                return tmp;
            }
		}

        delegate int lst_AB_Vadermoederlijst(int pProgID, int pProgramID, int pSoortBestand,
                                             string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                             string pResourceFolder, string pTaalcode, int pTaalnr,
                                             pCallback ReadDataProc,
                                             DateTime pDtBegin, DateTime pDtEnd,
                                             int pSort, int pDierAanwezig);     

        public delegate void pCallback(int PercDone, string Msg);
    }

}
