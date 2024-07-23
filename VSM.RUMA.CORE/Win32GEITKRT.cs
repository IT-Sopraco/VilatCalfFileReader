using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
	public class Win32GEITKRT: Win32
	{
        [ThreadStatic]
		private static Win32GEITKRT singleton;

		public static Win32GEITKRT Init()
		{
			if (singleton == null)
			{
				singleton = new Win32GEITKRT();
			}
			return singleton;
		}

        public Win32GEITKRT()
            : base(false)
        {
        }

		public int call_lst_AB_Geitkaart(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                         string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
										 pCallback ReadDataProc, 
										 int pAniId, bool pRightsMilk, bool pBeperkt, bool pAfstamming)
		{
            lock (typeof(Win32GEITKRT))
            {
                string sFilename = "GEITKRT.dll";

                lst_AB_Geitkaart handle = (lst_AB_Geitkaart)ExecuteProcedureDLL(typeof(lst_AB_Geitkaart), 
                                          sFilename, 
                                          "lst_AB_Geitkaart");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, pUbnnr, pHostName, 
                                 pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pAniId, pRightsMilk, pBeperkt, pAfstamming);

                FreeDLL(sFilename);
                return tmp;
            }
		}

		public delegate void pCallback(int PercDone, string Msg);

		delegate int lst_AB_Geitkaart(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr, 
                                      string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
									  pCallback ReadDataProc, 
									  int pAniId, bool pRightsMilk, bool pBeperkt, bool pAfstamming);


        /*
         function lst_AB_AfstammingsbewijsGeit(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pAniId: Integer; pDochters, pZonen, pZussen, pBroers: Boolean): Integer; stdcall;
         */

        public int call_lst_AB_AfstammingsbewijsGeit(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr,
                                         string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         pCallback ReadDataProc,
                                         int pAniId, bool pDochters, bool pZonen, bool pZussen, bool pBroers)
        {
            lock (typeof(Win32GEITKRT))
            {
                string sFilename = "GEITKRT.dll";

                lst_AB_AfstammingsbewijsGeit handle = (lst_AB_AfstammingsbewijsGeit)ExecuteProcedureDLL(typeof(lst_AB_AfstammingsbewijsGeit),
                                          sFilename,
                                          "lst_AB_AfstammingsbewijsGeit");

                int tmp = handle(pProgID, pProgramID, pSoortBestand, pUbnnr, pHostName,
                                 pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pAniId, pDochters, pZonen, pZussen, pBroers);

                FreeDLL(sFilename);
                return tmp;
            }
        }



        delegate int lst_AB_AfstammingsbewijsGeit(int pProgID, int pProgramID, int pSoortBestand, string pUbnnr,
                                      string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                      pCallback ReadDataProc,
                                      int pAniId, bool pDochters, bool pZonen, bool pZussen, bool pBroers);
    }
}
