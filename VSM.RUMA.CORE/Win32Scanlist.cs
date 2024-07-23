using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32Scanlist : Win32
	{
        [ThreadStatic]
		private static Win32Scanlist singleton;

		public static Win32Scanlist Init()
		{
			if (singleton == null)
			{
				singleton = new Win32Scanlist();
			}
			return singleton;
		}

		public int call_lst_AB_AfvoerSelectieLijst(int pProgID, int pProgramID, int pSoortBestand,
												   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
												   pCallback ReadDataProc,
                                                   DateTime pDatum, int pBestemming, int pHandelaar, int pUniekLevNr)
		{
            lock (typeof(Win32Scanlist))
            {
                lst_AB_AfvoerSelectieLijst handle = (lst_AB_AfvoerSelectieLijst)ExecuteProcedureDLL(typeof(lst_AB_AfvoerSelectieLijst), "SCANLIST.dll", "lst_AB_AfvoerSelectieLijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDatum, pBestemming, pHandelaar, pUniekLevNr);

                FreeDLL("SCANLIST.dll");
                return tmp;
            }
		}

		delegate int lst_AB_AfvoerSelectieLijst(int pProgID, int pProgramID, int pSoortBestand,
											    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
												pCallback ReadDataProc,
                                                DateTime pDatum, int pBestemming, int pHandelaar, int pUniekLevNr);

		public delegate void pCallback(int PercDone, string Msg);
	}
}
