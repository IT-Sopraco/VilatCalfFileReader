using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32DROOGZET : Win32
    {
        //BUG 1803


        [ThreadStatic]
        private static Win32DROOGZET singleton;

        public static Win32DROOGZET Init()
        {
            if (singleton == null)
            {
                singleton = new Win32DROOGZET();
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
            pFileName = pFileName.Replace(".DLL", "");
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
        function lst_AB_Droogzetlijst(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pSoortlijst, pAlleenNuDroog, pSortering: integer;
          pBeginDatum, pEindDatum: TDateTime): Integer; stdcall;
        */


          public int call_lst_AB_Droogzetlijst(int pProgID, int pProgramID, int pSoortBestand,
                                                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                                pCallback ReadDataProc,
                                                int pSoortlijst, int pAlleenNuDroog, int pSortering,
                                                DateTime pBeginDatum, DateTime pEindDatum)
        {
            LockObject padlock;
            lst_AB_Droogzetlijst handle = (lst_AB_Droogzetlijst)ExecuteProcedureDLLStack(typeof(lst_AB_Droogzetlijst), "lst_AB_Droogzetlijst", GetLockList("DROOGZET.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pSoortlijst, pAlleenNuDroog, pSortering,
                               pBeginDatum, pEindDatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Droogzetlijst(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               int pSoortlijst, int pAlleenNuDroog, int pSortering,
                               DateTime pBeginDatum, DateTime pEindDatum);

        /*
            function lst_AB_DroogstandEvaluatie(
                       pProgID, pProgramID, pSoortBestand: Integer;
                       pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                       pCallBack: Pointer;
                       pJaar, pMaand, pIndeling: integer): Integer; stdcall;

             pIndeling: 1=Maand, 2=Kwartaal
         */

        public int call_lst_AB_DroogstandEvaluatie(int pProgID, int pProgramID, int pSoortBestand,
                                      string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                      pCallback ReadDataProc,
                                      int pJaar, int pMaand, int pIndeling)
        {
            LockObject padlock;
            lst_AB_DroogstandEvaluatie handle = (lst_AB_DroogstandEvaluatie)ExecuteProcedureDLLStack(typeof(lst_AB_DroogstandEvaluatie), "lst_AB_DroogstandEvaluatie", GetLockList("DROOGZET.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pJaar, pMaand, pIndeling);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_DroogstandEvaluatie(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               int pJaar, int pMaand, int pIndeling);
    }
}
