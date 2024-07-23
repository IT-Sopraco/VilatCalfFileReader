using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32Medicijn: Win32
	{
        [ThreadStatic]
		private static Win32Medicijn singleton;

		public static Win32Medicijn Init()
		{
			if (singleton == null)
			{
				singleton = new Win32Medicijn();
			}
			return singleton;
		}

        public Win32Medicijn()
            : base(false)
        {
        }


        /*
         function lst_AB_MedicijnAankoop(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pDtBegin, pDtEnd: TDateTime): Integer; export; stdcall; 
         */
        public int call_lst_AB_MedicijnAankoop(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                               string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               string pResourceFolder, string pTaalcode, int pTaalnr,
											   pCallback ReadDataProc, 
											   DateTime pDtBegin, DateTime pDtEnd)
        {
            lock (typeof(Win32Medicijn))
            {             
                string sFilename = "MEDICIJN.dll";
                lst_AB_MedicijnAankoop handle = (lst_AB_MedicijnAankoop)ExecuteProcedureDLL(typeof(lst_AB_MedicijnAankoop), 
                                                sFilename, 
                                                "lst_AB_MedicijnAankoop");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, pUbnnr, 
                                 pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd);

                FreeDLL(sFilename);
                return tmp;
            }
		}

        /*
         function lst_AB_MedicijnVoorraad(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pAlleMedicijnen: Boolean): Integer; export; stdcall;
         */
        public int call_lst_AB_MedicijnVoorraad(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                                string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
											    pCallback ReadDataProc, 
												bool pAlleMedicijnen)																						 
		{
            lock (typeof(Win32Medicijn))
            {
                string sFilename = "MEDICIJN.dll";             
                lst_AB_MedicijnVoorraad handle = (lst_AB_MedicijnVoorraad)ExecuteProcedureDLL(typeof(lst_AB_MedicijnVoorraad), 
                                                 sFilename, 
                                                 "lst_AB_MedicijnVoorraad");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, pUbnnr, 
                                 pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pAlleMedicijnen);

                FreeDLL(sFilename);
                return tmp;
            }
        }
        
		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_MedicijnAankoop(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                            string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                            string pResourceFolder, string pTaalcode, int pTaalnr,
											pCallback ReadDataProc, 
											DateTime pDtBegin, DateTime pDtEnd);
        
		delegate int lst_AB_MedicijnVoorraad(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                             string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                             string pResourceFolder, string pTaalcode, int pTaalnr,
											 pCallback ReadDataProc, 
											 bool pAlleMedicijnen);	
	}
}
