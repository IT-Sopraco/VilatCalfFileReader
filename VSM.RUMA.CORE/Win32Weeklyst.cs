using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32Weeklyst: Win32
	{
        [ThreadStatic]
		private static Win32Weeklyst singleton;

		public static Win32Weeklyst Init()
		{
			if (singleton == null)
			{
				singleton = new Win32Weeklyst();
			}
			return singleton;
		}
                
		public int call_lst_AB_Koppelresultaten(
                            int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                            string pHostName, string pUserName, string pPassword, 
                            string pBestand, string pLog,
                            pCallback ReadDataProc,
                            int pGroupId, 
                            int pUitvalCorrectie, int pInclVat, int pDefaultBehandelKosten,
                            int pSavedValueId, int pEnableLegenda)
		{
            lock (typeof(Win32Weeklyst))
            {
                string sFileName = "WEEKLYST.dll";

                lst_AB_Koppelresultaten handle = (lst_AB_Koppelresultaten)ExecuteProcedureDLL(
                                                  typeof(lst_AB_Koppelresultaten), 
                                                  sFileName, 
                                                  "lst_AB_Koppelresultaten");
                
                int tmpint = handle(pProgID, pProgramID, pSoortBestand, pUbnnr, 
                                    pHostName, pUserName, Decodeer_PW(pPassword), 
                                    pBestand, pLog,
                                    ReadDataProc,
                                    pGroupId, 
                                    pUitvalCorrectie, pInclVat, pDefaultBehandelKosten,
                                    pSavedValueId, pEnableLegenda);

                FreeDLL(sFileName);
                return tmpint;
            }
		}

        public int call_lst_AB_Koppelresultaten_Multi(
                            int pProgID, int pProgramID, int pSoortBestand, 
                            string pHostName, string pUserName, string pPassword,
                            string pXmlBestand, string pBestand, string pLog,
                            pCallback ReadDataProc,
                            int pUitvalCorrectie, int pInclVat, int pDefaultBehandelKosten,
                            int pSavedValueId, int pEnableLegenda,
                            int pSaveValues, string pSaveValues_Title, string pSaveValues_Comment)
        {
            lock (typeof(Win32Weeklyst))
            {
                string sFileName = "WEEKLYST.dll";

                lst_AB_Koppelresultaten_Multi handle = (lst_AB_Koppelresultaten_Multi)ExecuteProcedureDLL(
                                                        typeof(lst_AB_Koppelresultaten_Multi),
                                                        sFileName,
                                                        "lst_AB_Koppelresultaten_Multi");

                int tmpint = handle(pProgID, pProgramID, pSoortBestand, 
                                    pHostName, pUserName, Decodeer_PW(pPassword),
                                    pXmlBestand, pBestand, pLog,
                                    ReadDataProc,
                                    pUitvalCorrectie, pInclVat, pDefaultBehandelKosten,
                                    pSavedValueId, pEnableLegenda,
                                    pSaveValues, pSaveValues_Title, pSaveValues_Comment);

                FreeDLL(sFileName);
                return tmpint;
            }
        }
        
        public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_Koppelresultaten(
                int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                string pHostName, string pUserName, string pPassword, 
                string pBestand, string pLog,
				pCallback ReadDataProc,
                int pGroupId, 
                int pUitvalCorrectie, int pInclVat, int pDefaultBehandelKosten,
                int pSavedValueId, int pEnableLegenda);

        delegate int lst_AB_Koppelresultaten_Multi(
                int pProgID, int pProgramID, int pSoortBestand, 
                string pHostName, string pUserName, string pPassword,
                string pXmlBestand, string pBestand, string pLog,
                pCallback ReadDataProc,            
                int pUitvalCorrectie, int pInclVat, int pDefaultBehandelKosten,
                int pSavedValueId, int pEnableLegenda,
                int pSaveValues, string pSaveValues_Title, string pSaveValues_Comment);
    }
}