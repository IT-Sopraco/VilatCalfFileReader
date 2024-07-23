using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
	public class Win32LSTRDBMPR : Win32
	{
        [ThreadStatic]
		private static Win32LSTRDBMPR singleton;

		public static Win32LSTRDBMPR Init()
		{
			if (singleton == null)
			{
				singleton = new Win32LSTRDBMPR();
			}
			return singleton;
		}

		public delegate void pCallback(int PercDone, string Msg);

        public int call_lst_AB_MPRuitslag(int pProgID, int pProgramID, int pSoortBestand,
                                          string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                          pCallback ReadDataProc,
                                          DateTime pMPRdatum)
        {
            lock (typeof(Win32LSTRDBMPR))
            {
                string sFilename = "LSTRDBMPR.dll";

                lst_AB_MPRuitslag handle = (lst_AB_MPRuitslag)ExecuteProcedureDLL(typeof(lst_AB_MPRuitslag), 
                                                                                  sFilename, "lst_AB_MPRuitslag");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pMPRdatum);
         
                FreeDLL(sFilename);
                return tmp;
            }
        }
                
		delegate int lst_AB_MPRuitslag(int pProgID, int pProgramID, int pSoortBestand,
									   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
									   pCallback ReadDataProc, 
									   DateTime pMPRdatum);
	}	
}
