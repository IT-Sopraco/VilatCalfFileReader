using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32lstattentielijst : Win32 
    {
        [ThreadStatic]
		private static Win32lstattentielijst singleton;

		public static Win32lstattentielijst Init()
		{
			if (singleton == null)
			{
				singleton = new Win32lstattentielijst();
			}
			return singleton;
		}

        public Win32lstattentielijst()
            : base(false)
        {
        }

		public int call_lst_AB_Attentielijst_Kalf(int pProgID, int pProgramID, int pSoortBestand,
												  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
												  pCallback ReadDataProc, 
												  int pSoortLijst, DateTime pDtBegin, DateTime pDtEnd, int pAantalBehandelingen)
		{
            lock (typeof(Win32lstattentielijst))
            {
                lst_AB_Attentielijst_Kalf handle = (lst_AB_Attentielijst_Kalf)ExecuteProcedureDLL(typeof(lst_AB_Attentielijst_Kalf), "lstattentielijst.DLL", "lst_AB_Attentielijst_Kalf");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pSoortLijst, pDtBegin, pDtEnd, pAantalBehandelingen);

                FreeDLL("lstattentielijst.DLL");
                return tmp;
            }
		}
        
        public delegate void pCallback(int PercDone, string Msg);

        delegate int lst_AB_Attentielijst_Kalf(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               int pSoortLijst, DateTime pDtBegin, DateTime pDtEnd, int pAantalBehandelingen);
    }
}
