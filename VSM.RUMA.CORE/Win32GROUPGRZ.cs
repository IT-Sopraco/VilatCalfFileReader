using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32GROUPGRZ : Win32 
    {
        //BUG 1799
        [ThreadStatic]
        private static Win32GROUPGRZ singleton;

        public static Win32GROUPGRZ Init()
        {
            if (singleton == null)
            {
                singleton = new Win32GROUPGRZ();
            }
            return singleton;
        }



        /*
         function lst_AB_GrazingTogetherList(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                  pCallBack: Pointer;
                  pJaar, pPeriode: integer;
                  pBeginDatum, pEindDatum: TDateTime;
                  pInclAfgevoerd, pAniIdFather, pSortering: integer): Integer; stdcall;
         */


        public int call_lst_AB_GrazingTogetherList(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                int pJaar, int pPeriode,
                                                DateTime pBeginDatum, DateTime pEindDatum,
                                                int pInclAfgevoerd, int pAniIdFather, int pSortering)
        {
            lock (typeof(Win32GROUPGRZ))
            {
                string sFilename = "GROUPGRZ.dll";

                lst_AB_GrazingTogetherList handle = (lst_AB_GrazingTogetherList)ExecuteProcedureDLL(typeof(lst_AB_GrazingTogetherList), sFilename, "lst_AB_GrazingTogetherList");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pJaar, pPeriode,
                        pBeginDatum, pEindDatum,
                        pInclAfgevoerd, pAniIdFather, pSortering);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_GrazingTogetherList(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                int pJaar, int pPeriode,
                                                DateTime pBeginDatum, DateTime pEindDatum,
                                                int pInclAfgevoerd, int pAniIdFather, int pSortering);



        public delegate void pCallback(int PercDone, string Msg);


    }
}
