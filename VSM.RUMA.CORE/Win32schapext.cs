using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
	public class Win32schapext : Win32
	{
        [ThreadStatic]
		private static Win32schapext singleton;

		public static Win32schapext Init()
		{
			if (singleton == null)
			{
				singleton = new Win32schapext();
			}
			return singleton;
		}

		public void call_lst_AB_Dieroverzicht_Productie(int pProgID, int pProgramID, int pSoortBestand,
														string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
														pCallback ReadDataProc, 
														DateTime pKeurDatum,
														int pSortering,
														int pFokwaarde_56dgn_Gewicht,
														int pFokwaarde_135dgn_Gewicht,
														int pFokwaarde_Worpgrootte,
														int pFokwaarde_Twt,
														int pFokwaarde_Moederzorg)
	    {
            lock (typeof(Win32schapext))
            {
                lst_AB_Dieroverzicht_Productie handle = (lst_AB_Dieroverzicht_Productie)ExecuteProcedureDLL(typeof(lst_AB_Dieroverzicht_Productie), "SCHAPEXT.DLL", "lst_AB_Dieroverzicht_Productie");

                handle(pProgID, pProgramID, pSoortBestand,
                       pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLogdir,
                       ReadDataProc,
                       pKeurDatum,
                       pSortering,
                       pFokwaarde_56dgn_Gewicht,
                       pFokwaarde_135dgn_Gewicht,
                       pFokwaarde_Worpgrootte,
                       pFokwaarde_Twt,
                       pFokwaarde_Moederzorg);

                FreeDLL("SCHAPEXT.DLL");
            }
	    }

		public void call_lst_AB_Dieroverzicht_Exterieur(int pProgID, int pProgramID, int pSoortBestand,
													    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
													    pCallback ReadDataProc, 
														DateTime pKeurdatum, int pGroep, int pSortering)
		{
            lock (typeof(Win32schapext))
            {
                lst_AB_Dieroverzicht_Exterieur handle = (lst_AB_Dieroverzicht_Exterieur)ExecuteProcedureDLL(typeof(lst_AB_Dieroverzicht_Exterieur), "SCHAPEXT.DLL", "lst_AB_Dieroverzicht_Exterieur");

                handle(pProgID, pProgramID, pSoortBestand,
                             pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLogdir,
                             ReadDataProc,
                             pKeurdatum, pGroep, pSortering);

                FreeDLL("SCHAPEXT.DLL");
            }
		}

		public delegate void pCallback(int PercDone, string Msg);

		delegate void lst_AB_Dieroverzicht_Productie(int pProgID, int pProgramID, int pSoortBestand,
													 string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
													 pCallback ReadDataProc, 
													 DateTime pKeurDatum,
													 int pSortering,
													 int pFokwaarde_56dgn_Gewicht,
													 int pFokwaarde_135dgn_Gewicht,
													 int pFokwaarde_Worpgrootte,
													 int pFokwaarde_Twt,
													 int pFokwaarde_Moederzorg);

		delegate void lst_AB_Dieroverzicht_Exterieur(int pProgID, int pProgramID, int pSoortBestand,
													 string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
													 pCallback ReadDataProc, 
													 DateTime pKeurdatum, int pGroep, int pSortering);

	}

	//#region static calls
	//public class Win32schapext_static
	//{
	//  public delegate void pCallback(int PercDone, string Msg);

	//  public static void lstMySQL_Dieroverzicht_Exterieur(string pUbnnr, int pPrognr,
	//                                              string pUserName, string pPassword,
	//                                              string pPDFbestand, string pLogdir,
	//                                              bool pLocalDB, pCallback ReadDataProc,
	//                                              DateTime pKeurdatum, int pGroep, int pSortering, int SoortBestand)
	//  {
	//    MySQL_Dieroverzicht_Exterieur(pUbnnr, pPrognr,
	//                                         pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword),
	//                                         pPDFbestand, pLogdir, pLocalDB, ReadDataProc,
	//                                         pKeurdatum, pGroep, pSortering, SoortBestand);
	//  }

	//  public static void lstMySQL_Dieroverzicht_Productie(string pUbnnr, int pPrognr, string pUserName, string pPassword,
	//                                          string pPDFbestand, string pLogdir,
	//                                          bool pLocalDB, pCallback ReadDataProc,
	//                                          DateTime pKeurDatum,
	//                                          int pSortering,
	//                                          int pFokwaarde_56dgn_Gewicht,
	//                                          int pFokwaarde_135dgn_Gewicht,
	//                                          int pFokwaarde_Worpgrootte,
	//                                          int pFokwaarde_Twt,
	//                                          int pFokwaarde_Moederzorg,
	//                                          int SoortBestand)
	//  {
	//    MySQL_Dieroverzicht_Productie(pUbnnr, pPrognr, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword),
	//                                         pPDFbestand, pLogdir, pLocalDB, ReadDataProc,
	//                                         pKeurDatum, pSortering, pFokwaarde_56dgn_Gewicht,
	//                                         pFokwaarde_135dgn_Gewicht, pFokwaarde_Worpgrootte, pFokwaarde_Twt,
	//                                         pFokwaarde_Moederzorg, SoortBestand);
	//  }

	//  [DllImport("SCHAPEXT.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	//  private extern static void MySQL_Dieroverzicht_Exterieur(string pUbnnr, int pPrognr,
	//                                              string pUserName, string pPassword,
	//                                              string pPDFbestand, string pLogdir,
	//                                              bool pLocalDB, pCallback ReadDataProc,
	//                                              DateTime pKeurdatum, int pGroep, int pSortering, int SoortBestand);


	//  [DllImport("SCHAPEXT.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
	//  private extern static void MySQL_Dieroverzicht_Productie(string pUbnnr, int pPrognr, string pUserName, string pPassword,
	//                                          string pPDFbestand, string pLogdir,
	//                                          bool pLocalDB, pCallback ReadDataProc,
	//                                          DateTime pKeurDatum,
	//                                          int pSortering,
	//                                          int pFokwaarde_56dgn_Gewicht,
	//                                          int pFokwaarde_135dgn_Gewicht,
	//                                          int pFokwaarde_Worpgrootte,
	//                                          int pFokwaarde_Twt,
	//                                          int pFokwaarde_Moederzorg,
	//                                          int SoortBestand);

	//}
	//  #endregion
}
