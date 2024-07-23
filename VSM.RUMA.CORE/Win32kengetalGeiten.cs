using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32kengetalGeiten : Win32
    {
        [ThreadStatic]
        private static Win32kengetalGeiten singleton;

        public static Win32kengetalGeiten Init()
        {
            if (singleton == null)
            {
                singleton = new Win32kengetalGeiten();
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
         function lst_AB_kengetallenGeiten( pProgID, pProgramID, pSoortBestand: integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pCallback: Pointer; pMaand, pJaar: integer): Integer; stdcall;
         
          function lst_AB_kengetallenGeiten(
              pProgID, pProgramID, pSoortBestand: integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallback: Pointer; pMaand, pJaar, pReferentieGroep: integer;
              pTestVersie: integer = 0): Integer; stdcall;
         */

        public int call_lst_AB_kengetallenGeiten(int pProgID, int pProgramID, int pSoortBestand,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                pCallback ReadDataProc,
                int pMaand, int pJaar, int pReferentieGroep, int pTestVersie)
        {
            LockObject padlock;
            lst_AB_kengetallenGeiten handle = (lst_AB_kengetallenGeiten)ExecuteProcedureDLLStack(typeof(lst_AB_kengetallenGeiten), "lst_AB_kengetallenGeiten", GetLockList("kengetalGeiten.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        ReadDataProc,
                        pMaand, pJaar, pReferentieGroep, pTestVersie);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_kengetallenGeiten(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        pCallback ReadDataProc,
                        int pMaand, int pJaar, int pReferentieGroep, int pTestVersie);

        //////////////////////////////////////////////////////////////////////////

        public int call_AB_kengetallenGeitenVerzamelen(int pProgID, int pProgramID,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                pCallback ReadDataProc, int pJaar, int pMaand, int pSoort)
        {
            LockObject padlock;
            AB_kengetallenGeitenVerzamelen handle = (AB_kengetallenGeitenVerzamelen)ExecuteProcedureDLLStack(typeof(AB_kengetallenGeitenVerzamelen), "AB_kengetallenGeitenVerzamelen", GetLockList("kengetalGeiten.DLL"), out padlock);
            int tmp;
            try
            {
                tmp = handle(pProgID, pProgramID,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                        ReadDataProc,
                        pMaand, pJaar, pSoort);
            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);
            }

            return tmp;
        }

        delegate int AB_kengetallenGeitenVerzamelen(int pProgID, int pProgramID,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                pCallback ReadDataProc, int pJaar, int pMaand, int pSoort);
    }
}
