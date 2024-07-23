using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32SOAPLNV_AB : Win32
    {
        public delegate void pCallback(int PercDone, string Msg);


        public Win32SOAPLNV_AB()
            : base(false)
        {
        }


        static readonly object padlck = new object();

        private static LockObject[] LockList;

        public static LockObject[] GetLockList()
        {

            lock (padlck)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\SOAPLNV_AB.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\SOAPLNV_AB{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        /*function AB_stallijstDieren_LNV(
            pPrognr: Integer;
            pUbnnr, pLNVrelatienr, pLogfile: PAnsiChar;
            pUserNameLNV, pPasswordLNV: PAnsiChar;
            pLNVtestserver: integer;
            pPeildatum: TDateTime;
            pDierFile: PAnsiChar): integer; stdcall;
         * 
         * function AB_stallijstDieren_LNV(
            pPrognr: Integer;
            pUbnnr, pLNVrelatienr, pLogfile: PAnsiChar;
            pUserNameLNV, pPasswordLNV: PAnsiChar;
            pLNVtestserver: integer;
            pBegindatum, pEinddatum: TDateTime;
            pDierFile: PAnsiChar): integer; stdcall;

         * 
        */

        public int call_AB_stallijstDieren_LNV(
          int pPrognr,
          string pUbnnr, string pLNVrelatienr, string pLogfile,
          string pUserNameLNV, string pPasswordLNV,
          int pLNVtestserver,
          DateTime pBegindatum, DateTime pEinddatum,
          string pDierfile)
        {

            LockObject padlock;
            AB_stallijstDieren_LNV handle = (AB_stallijstDieren_LNV)ExecuteProcedureDLLStack(typeof(AB_stallijstDieren_LNV), "AB_stallijstDieren_LNV", GetLockList(), out padlock);
            try
            {
                int tmp = handle(pPrognr,
                    pUbnnr, pLNVrelatienr, pLogfile,
                    pUserNameLNV, pPasswordLNV,
                    pLNVtestserver,
                    pBegindatum,
                    pEinddatum,
                    pDierfile);

                FreeDLL(padlock.DLLname);
                return tmp;
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        delegate int AB_stallijstDieren_LNV(
                int pPrognr,
                string pUbnnr, string pLNVrelatienr, string pLogfile,
                string pUserNameLNV, string pPasswordLNV,
                int pLNVtestserver,
                DateTime pBegindatum, DateTime pEinddatum,
                string pDierfile);

        /*
         procedure AB_stallijst_LNV(
            pPrognr, pProgramID: Integer;
            pUbnnr, pLNVrelatienr, pUserNameAB, pPasswordAB, 
            pHostName, pLogfile: PAnsiChar;
            pUserNameLNV, pPasswordLNV: PAnsiChar;
            pLNVtestserver: integer; pCallback: Pointer;
            pDierfile: PAnsiChar;
            var AantDieren: integer); stdcall;

         */
        public void call_AB_stallijst_LNV(int pProgID, int pProgramID,
                  string pUbnnr, string pLNVrelatienr, string pUserName, string pPassword,
                  string pHostName, string pLogdir,
                  string pUserNameLNV, string pPasswordLNV,
                  int pLNVtestserver, pCallback ReadDataProc,
                  string pDierfile,int pStandaardGeslacht,
            //DateTime pPeildatum,
                  ref int AantDieren)
        {
            LockObject padlock;
            AB_stallijst_LNV handle = (AB_stallijst_LNV)ExecuteProcedureDLLStack(typeof(AB_stallijst_LNV), "AB_stallijst_LNV", GetLockList(), out padlock);
            try
            {
                handle(pProgID, pProgramID,
                       pUbnnr, pLNVrelatienr, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pHostName, pLogdir,
                       pUserNameLNV, pPasswordLNV,
                       pLNVtestserver, ReadDataProc,
                       pDierfile, pStandaardGeslacht,
                    //pPeildatum,
                       ref AantDieren);

                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        delegate void AB_stallijst_LNV(int pProgID, int pProgramID,
                                       string pUbnnr, string pLNVrelatienr, string pUserName, string pPassword, string pHostName, string pLogdir,
                                       string pUserNameLNV, string pPasswordLNV,
                                       int pLNVtestserver, pCallback ReadDataProc,
                                       string pDierfile,int pStandaardGeslacht,
            //DateTime pPeildatum,
                                       ref int AantDieren);


        /*procedure AB_opvragenVerblijfplaatsen_LNV(pPrognr, pProgramID: Integer;
            pUbnnr, pLNVrelatienr, pUserNameAB, pPasswordAB, pLogfile: PAnsiChar;
            pUserNameLNV, pPasswordLNV: PAnsiChar;
            pLNVtestserver: integer;
            pBegindatum, pEindDatum: TDateTime;
            pCallback: Pointer;
            var AantVerplaatsingen: integer); stdcall;
            */
        public void call_AB_opvragenVerblijfplaatsen_LNV(int pProgID, int pProgramID,
                                                         string pUbnnr, string pLNVrelatienr, string pUserName, string pPassword, string pHostName, string pLogdir,
                                                         string pUserNameLNV, string pPasswordLNV,
                                                         int pLNVtestserver,
                                                         DateTime pBegindatum, DateTime pEindDatum,
                                                         pCallback ReadDataProc,
                                                         ref int AantVerplaatsingen)
        {
            LockObject padlock;
            AB_opvragenVerblijfplaatsen_LNV handle = (AB_opvragenVerblijfplaatsen_LNV)ExecuteProcedureDLLStack(typeof(AB_opvragenVerblijfplaatsen_LNV), "AB_opvragenVerblijfplaatsen_LNV", GetLockList(), out padlock);
            try
            {
                handle(pProgID, pProgramID,
                       pUbnnr, pLNVrelatienr, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pHostName, pLogdir,
                       pUserNameLNV, pPasswordLNV,
                       pLNVtestserver, pBegindatum, pEindDatum, ReadDataProc,
                       ref AantVerplaatsingen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

        }

        delegate void AB_opvragenVerblijfplaatsen_LNV(int pProgID, int pProgramID,
                                                      string pUbnnr, string pLNVrelatienr, string pUserName, string pPassword, string pHostName, string pLogdir,
                                                      string pUserNameLNV, string pPasswordLNV,
                                                      int pLNVtestserver,
                                                      DateTime pBegindatum, DateTime pEindDatum, pCallback ReadDataProc,
                                                      ref int AantVerplaatsingen);//VAR param in Delphi




    }
}
