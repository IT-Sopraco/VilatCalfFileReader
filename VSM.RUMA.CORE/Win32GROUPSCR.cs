using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32GROUPSCR : Win32
    {
        [ThreadStatic]
        private static Win32GROUPSCR singleton;

        public static Win32GROUPSCR Init()
        {
            if (singleton == null)
            {
                singleton = new Win32GROUPSCR();
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


        /*
                function lst_AB_Conditiescorelijst(
                                pProgID, pProgramID, pSoortBestand: Integer;
                                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                                pCallBack: Pointer;
                                pMeetdatum: TDateTime;
                                pToonDieren, pCurve: integer): Integer; stdcall;

                pToonDieren: 0/1
                pCurve: 0=ntv (SCCURVE->Curvenr, deze table bestaat nog niet, zie email van zojuist)
                SCCURVE->CurveKind = 1 : conditiescore curves
                SCCURVE->CurveKind = 2 : klauwscore curves 
         */

        public int call_lst_AB_Conditiescorelijst(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       pCallback ReadDataProc,
                                       DateTime pMeetdatum,
                                       int pToonDieren, int pCurve)
        {


            LockObject padlock;
            lst_AB_Conditiescorelijst handle = (lst_AB_Conditiescorelijst)ExecuteProcedureDLLStack(typeof(lst_AB_Conditiescorelijst), "lst_AB_Conditiescorelijst", GetLockList("GROUPSCR.DLL"), out padlock);
            int tmp;
            try
            {
                unLogger.WriteError("call_lst_AB_Conditiescorelijst With:" + padlock.DLLname);

                tmp = handle(pProgID, pProgramID, pSoortBestand,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                ReadDataProc,
                                pMeetdatum,
                                pToonDieren, pCurve);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
            unLogger.WriteError("call_lst_AB_Conditiescorelijst :return=" + tmp.ToString());
            return tmp;


        }

        delegate int lst_AB_Conditiescorelijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pMeetdatum,
                                               int pToonDieren, int pCurve);


        /*
         function lst_AB_Klauwscorelijst(
                pProgID, pProgramID, pSoortBestand: Integer;
                pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                pCallBack: Pointer;
                pMeetdatum: TDateTime;
                pToonDieren, pCurve: integer): Integer; stdcall;
         */
        public int call_lst_AB_Klauwscorelijst(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pMeetdatum,
                               int pToonDieren, int pCurve)
        {


            LockObject padlock;
            lst_AB_Klauwscorelijst handle = (lst_AB_Klauwscorelijst)ExecuteProcedureDLLStack(typeof(lst_AB_Klauwscorelijst), "lst_AB_Klauwscorelijst", GetLockList("GROUPSCR.DLL"), out padlock);
            int tmp;
            try
            {
                unLogger.WriteError("call_lst_AB_Klauwscorelijst With:" + padlock.DLLname);

                tmp = handle(pProgID, pProgramID, pSoortBestand,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                ReadDataProc,
                                pMeetdatum,
                                pToonDieren, pCurve);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
            unLogger.WriteError("call_lst_AB_Klauwscorelijst :return=" + tmp.ToString());
            return tmp;


        }

        delegate int lst_AB_Klauwscorelijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               pCallback ReadDataProc,
                                               DateTime pMeetdatum,
                                               int pToonDieren, int pCurve);


        public delegate void pCallback(int PercDone, string Msg);
    }
}
