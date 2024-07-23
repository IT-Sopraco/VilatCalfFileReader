using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{

	public class win32lstabbedreg : Win32
	{
        [ThreadStatic]
		private static win32lstabbedreg singleton;

		public static win32lstabbedreg Init()
		{
			if (singleton == null)
			{
				singleton = new win32lstabbedreg();
			}
			return singleton;
		}

		public int call_lst_AB_Bedrijfsregister(int pProgID, int pProgramID, int pSoortBestand,
												string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
												pCallback ReadDataProc, 
												DateTime pDtBegin, DateTime pDtEnd)
		{
            lock (typeof(win32lstabbedreg))
            {
                lst_AB_Bedrijfsregister handle = (lst_AB_Bedrijfsregister)ExecuteProcedureDLL(typeof(lst_AB_Bedrijfsregister), "LSTABBEDREG.DLL", "lst_AB_Bedrijfsregister");

                int tmpint = 0;
                tmpint = handle(pProgID, pProgramID, pSoortBestand,
                                pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                ReadDataProc,
                                pDtBegin, pDtEnd);

                FreeDLL("LSTABBEDREG.DLL");
                return tmpint;
            }
		}


		public delegate void pCallback(int PercDone, string Msg);
		 
		delegate int lst_AB_Bedrijfsregister(int pProgID, int pProgramID, int pSoortBestand,
											 string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
											 pCallback ReadDataProc, 
											 DateTime pDtBegin, DateTime pDtEnd);
	
	}
}
