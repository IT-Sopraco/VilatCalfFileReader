using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32FEEDSTCK: Win32
	{
        [ThreadStatic]
		private static Win32FEEDSTCK singleton;

		public static Win32FEEDSTCK Init()
		{
			if (singleton == null)
			{
				singleton = new Win32FEEDSTCK();
			}
			return singleton;
		}

        public Win32FEEDSTCK()
            : base(false)
        {
        }

        public int call_lst_AB_poederleveringsoverzicht(int pProgID, int pProgramID, int pSoortBestand,
                                                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                        pCallback ReadDataProc,
                                                        DateTime pDtBegin, DateTime pDtEnd, int pSort, bool pSubTotaal)
        {
            lock (typeof(Win32FEEDSTCK))
            {
                lst_AB_poederleveringsoverzicht handle = (lst_AB_poederleveringsoverzicht)ExecuteProcedureDLL(typeof(lst_AB_poederleveringsoverzicht), "FEEDSTCK.DLL", "lst_AB_poederleveringsoverzicht");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDtBegin, pDtEnd, pSort, pSubTotaal);

                FreeDLL("FEEDSTCK.DLL");
                return tmp;
            }
        }

		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_poederleveringsoverzicht(int pProgID, int pProgramID, int pSoortBestand,
												     string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
													 pCallback ReadDataProc, 
													 DateTime pDtBegin, DateTime pDtEnd, int pSort, bool pSubTotaal);

        /*
         function lst_AB_voerverbruik(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pSoortVoer, pGroepnr: Integer): Integer; stdcall;

                  pSoortVoer
                    0 = Melkpoeder
                    1 = Overig voer
                pGroepnr is verplicht 
         */
        public int call_lst_AB_voerverbruik(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                pCallback ReadDataProc,
                                                int pSoortVoer, int pGroepnr)
        {
            lock (typeof(Win32FEEDSTCK))
            {
                lst_AB_voerverbruik handle = (lst_AB_voerverbruik)ExecuteProcedureDLL(typeof(lst_AB_voerverbruik), "FEEDSTCK.DLL", "lst_AB_voerverbruik");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pSoortVoer, pGroepnr);

                FreeDLL("FEEDSTCK.DLL");
                return tmp;
            }
        }

        delegate int lst_AB_voerverbruik(int pProgID, int pProgramID, int pSoortBestand,
                                                     string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                     pCallback ReadDataProc,
                                                     int pSoortVoer, int pGroepnr);

        /*
         function lst_AB_VoerVoorraad(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pDatum: TDateTime): Integer; stdcall; 
         */
        public int call_lst_AB_VoerVoorraad(int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        DateTime pDatum)
        {
            lock (typeof(Win32FEEDSTCK))
            {
                lst_AB_VoerVoorraad handle = (lst_AB_VoerVoorraad)ExecuteProcedureDLL(typeof(lst_AB_VoerVoorraad), "FEEDSTCK.DLL", "lst_AB_VoerVoorraad");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pDatum);

                FreeDLL("FEEDSTCK.DLL");
                return tmp;
            }
        }

        delegate int lst_AB_VoerVoorraad(int pProgID, int pProgramID, int pSoortBestand,
                                                     string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                     pCallback ReadDataProc,
                                                     DateTime pDatum);

    }
}
