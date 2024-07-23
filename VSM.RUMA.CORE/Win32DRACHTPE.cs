using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32DRACHTPE : Win32
    {
        //BUG 1802

        [ThreadStatic]
        private static Win32DRACHTPE singleton;

        public static Win32DRACHTPE Init()
        {
            if (singleton == null)
            {
                singleton = new Win32DRACHTPE();
            }
            return singleton;
        }

        /// <summary>
        /// Copy region padlok per Lijst
        /// And change every  call_lst_function like the one below 
        /// </summary>
        #region Padlock

        static readonly object padlock = new object();

        private static LockObject[] LockList;

        private static LockObject[] GetLockList(string pFileName)
        {
            pFileName = pFileName.Replace(".DLL", "").Replace(".dll", "");
            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\" + pFileName + ".DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\" + pFileName + "{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        #endregion

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function lst_AB_drachtcontrolelijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallBack: Pointer;
              pAlleenStatus, pAlleenAanweezig: integer;
              pHoknr: PAnsiChar; pSortering: integer;
              pBeginDatum, pEindDatum: TDateTime): Integer; stdcall;
         */
        public int call_lst_AB_drachtcontrolelijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                int pAlleenStatus, int pAlleenAanwezig, string pHoknr, int pSortering,
                                                DateTime pBeginDatum, DateTime pEindDatum)
        {
            lock (typeof(Win32DRACHTPE))
            {
                string sFilename = "DRACHTPE.dll";

                lst_AB_drachtcontrolelijst handle = (lst_AB_drachtcontrolelijst)ExecuteProcedureDLL(typeof(lst_AB_drachtcontrolelijst), sFilename, "lst_AB_drachtcontrolelijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pAlleenStatus, pAlleenAanwezig, pHoknr, pSortering,
                        pBeginDatum, pEindDatum);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public int call_lst_AB_drachtanalyse(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                pCallback ReadDataProc,
                                                DateTime pBeginDatum, DateTime pEindDatum)
        {
            lock (typeof(Win32DRACHTPE))
            {
                string sFilename = "DRACHTPE.dll";

                lst_AB_drachtanalyse handle = (lst_AB_drachtanalyse)ExecuteProcedureDLL(typeof(lst_AB_drachtanalyse), sFilename, "lst_AB_drachtanalyse");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        ReadDataProc,
                        pBeginDatum, pEindDatum);

                FreeDLL(sFilename);
                return tmp;
            }
        }


        public delegate int lst_AB_drachtcontrolelijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                string pResourceFolder, string pTaalcode, int pTaalnr,
                                                pCallback ReadDataProc,
                                                int pAlleenStatus, int pAlleenAanwezig, string pHoknr, int pSortering,
                                                DateTime pBeginDatum, DateTime pEindDatum);

        public delegate int lst_AB_drachtanalyse(int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        DateTime pBeginDatum, DateTime pEindDatum);

        /*
         function lst_AB_DrachtresultatenGKN(
                pProgID, pProgramID: Integer;
                pHostName, pUserName, pPassword, pCsvBestand, pLog: PAnsiChar;
                pCallBack: Pointer;
                pBeginDatum, pEindDatum: TDateTime): Integer; stdcall;
         */

        public int call_lst_AB_DrachtresultatenGKN(int pProgID, int pProgramID,
           string pHostName, string pUserName, string pPassword, string pCsvBestand, string pLog,
           pCallback ReadDataProc,
           DateTime pBeginDatum, DateTime pEindDatum)
        {
            LockObject padlock;
            lst_AB_DrachtresultatenGKN handle = (lst_AB_DrachtresultatenGKN)ExecuteProcedureDLLStack(typeof(lst_AB_DrachtresultatenGKN), "lst_AB_DrachtresultatenGKN", GetLockList("DRACHTPE.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                       pHostName, pUserName, Decodeer_PW(pPassword), pCsvBestand, pLog,
                       ReadDataProc,
                       pBeginDatum, pEindDatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_DrachtresultatenGKN(int pProgID, int pProgramID,
           string pHostName, string pUserName, string pPassword, string pCsvBestand, string pLog,
           pCallback ReadDataProc,
           DateTime pBeginDatum, DateTime pEindDatum);

    }
}
