using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32lstabgebgew : Win32
    {
     
        [ThreadStatic]
        private static Win32lstabgebgew singleton;

        public static Win32lstabgebgew Init()
        {
            if (singleton == null)
            {
                singleton = new Win32lstabgebgew();
            }
            return singleton;
        }

        static readonly object padlock = new object();

        public delegate void pCallback(int PercDone, string Msg);
        	
		public int call_lst_AB_Geboortebewijs(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                              string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
											  pCallback ReadDataProc, 
											  string pRavBestand, string pLevensnummer)
        {
            lock (padlock)
            {
                string sFilename = "LSTABGEBBEW.DLL";

                lst_AB_Geboortebewijs handle = (lst_AB_Geboortebewijs)ExecuteProcedureDLL(
                                                                        typeof(lst_AB_Geboortebewijs), 
                                                                        sFilename, 
                                                                        "lst_AB_Geboortebewijs");
                
                unLogger.WriteDebug("ProgId       : " + pProgID.ToString());
                unLogger.WriteDebug("ProgramID    : " + pProgramID.ToString());
                unLogger.WriteDebug("SoortBestand : " + pSoortBestand.ToString());
                unLogger.WriteDebug("Ubnnr        : " + pUbnnr);
                unLogger.WriteDebug("HostName     : " + pHostName);
                unLogger.WriteDebug("UserName     : " + pUserName);
                unLogger.WriteDebug("Password     : " + Decodeer_PW(pPassword));
                unLogger.WriteDebug("Bestand      : " + pBestand);
                unLogger.WriteDebug("Log          : " + pLog);
                unLogger.WriteDebug("RavBestand   : " + pRavBestand);
                unLogger.WriteDebug("Levensnummer : " + pLevensnummer);

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pRavBestand, pLevensnummer);

                //FreeDLL(sFilename);
                return tmp;
            }
        }

		delegate int lst_AB_Geboortebewijs(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                           string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
										   pCallback ReadDataProc,
                                           string pRavBestand, string pLevensnummer);
    }
}
