using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32LSTKOPBH : Win32
    {
        [ThreadStatic]
        private static Win32LSTKOPBH singleton;

        public static Win32LSTKOPBH Init()
        {
            if (singleton == null)
            {
                singleton = new Win32LSTKOPBH();
            }
            return singleton;
        }

        public Win32LSTKOPBH()
            : base(false)
        {
        }

        public delegate void pCallback(int PercDone, string Msg);
        /*
         function lst_AB_BehandelingenPerKoppel(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pGroepid, pSoortlijst, pDetails1,
          pGrafieken1: integer): Integer; stdcall;

                  // pGroepid: 0 = alle groepen/koppels

                  // pSoortlijst
                  //  1 = Medicijngebruik/-kosten
                  //  2 = Medicijnen per aandoening

                pDetails1 en pGrafieken1 (0/1): extra instellingen voor soortlijst=1


         */

        public int call_lst_AB_BehandelingenPerKoppel(int pProgID, int pProgramID, int pSoortBestand,
                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                  pCallback ReadDataProc,
                                  int pGroepid, int pSoortlijst, int pDetails1,
                                  int pGrafieken1)
        {
            lock (typeof(Win32LSTKOPBH))
            {
                lst_AB_BehandelingenPerKoppel  handle = (lst_AB_BehandelingenPerKoppel)ExecuteProcedureDLL(typeof(lst_AB_BehandelingenPerKoppel), "LSTKOPBH.DLL", "lst_AB_BehandelingenPerKoppel");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pGroepid, pSoortlijst, pDetails1,
                                 pGrafieken1);

                FreeDLL("LSTKOPBH.DLL");
                return tmp;
            }
        }



        delegate int lst_AB_BehandelingenPerKoppel(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               int pGroepid, int pSoortlijst, int pDetails1,
                                               int pGrafieken1);
    }
}
