using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
	public class Win32Geblyst : Win32
	{
        [ThreadStatic]
		private static Win32Geblyst singleton;

		public static Win32Geblyst Init()
		{
			if (singleton == null)
			{
				singleton = new Win32Geblyst();
			}
			return singleton;
		}

        /*
         function lst_AB_geboortelijst(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pListMode: integer; pLabVerloop: integer; pNumberOfLines: Integer;
              pSort: Integer): Integer; stdcall; 
         */

        public int call_lst_AB_geboortelijst(int pProgID, int pProgramID, int pSoortBestand,
											 string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                             string pResourceFolder, string pTaalcode, int pTaalnr,
											 pCallback ReadDataProc,
											 int pListMode, int pLabVerloop, int pNumberOfLines,
                                             int pSort)
		{
            lock (typeof(Win32Geblyst))
            {
                string sFilename = "GEBLYST.DLL";

                lst_AB_geboortelijst handle = (lst_AB_geboortelijst)ExecuteProcedureDLL(typeof(lst_AB_geboortelijst), 
                                               sFilename, "lst_AB_geboortelijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pListMode, pLabVerloop, pNumberOfLines,
                                 pSort);

                FreeDLL(sFilename);
                return tmp;
            }
		}
        
		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_geboortelijst(int pProgID, int pProgramID, int pSoortBestand,
										  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                          string pResourceFolder, string pTaalcode, int pTaalnr,
                                          pCallback ReadDataProc,
										  int pListMode, int pLabVerloop, int pNumberOfLines,
                                          int pSort);
 	}
}
