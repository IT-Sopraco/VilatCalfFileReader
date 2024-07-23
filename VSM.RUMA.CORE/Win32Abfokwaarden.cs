using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Abfokwaarden : Win32
    {
        [ThreadStatic]
        private static Win32Abfokwaarden singleton;

        public static Win32Abfokwaarden Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Abfokwaarden();
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
            pFileName = pFileName.Replace(".dll", "");
            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\" + pFileName + ".dll");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\" + pFileName + "{0}.dll", add));
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
         * BUG 2072
            abfokwaarden.dll
            -------------
            function AB_maakFokwaardenBestandGeiten(
                      pProgID, pProgramID: integer;
                      pUbnnr, pHostName, pUserName, pPassword,
                      pLog: PAnsiChar; pCallback: Pointer;
                      pOutputFolder: PAnsiChar;
                      pDatum: TDateTime): Integer; stdcall;
         */
        public int call_lst_AB_maakFokwaardenBestandGeiten(int pProgID, int pProgramID,
                                  string pUbnnr, string pHostName, string pUserName, string pPassword,  string pLog,
                                  pCallback ReadDataProc,
                                  string pOutputFolder,
                                  DateTime pDatum)
        {
            LockObject padlock;
            lst_AB_maakFokwaardenBestandGeiten handle = (lst_AB_maakFokwaardenBestandGeiten)ExecuteProcedureDLLStack(typeof(lst_AB_maakFokwaardenBestandGeiten), "AB_maakFokwaardenBestandGeiten", GetLockList("abfokwaarden.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID,  pProgramID,
                                   pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                                  ReadDataProc,
                                   pOutputFolder,
                                   pDatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_maakFokwaardenBestandGeiten(int pProgID, int pProgramID,
                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                                  pCallback ReadDataProc,
                                  string pOutputFolder,
                                  DateTime pDatum);

       
    }
}
