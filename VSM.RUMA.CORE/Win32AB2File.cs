using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32AB2File : Win32
    {
        [ThreadStatic]
        private static Win32ab2xml singleton;

        public static Win32ab2xml Init()
        {
            if (singleton == null)
            {
                singleton = new Win32ab2xml();
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
             function AB2file_rekflesnummers(
                  pProgID, pProgramID: integer;
                  pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                  pCallback: Pointer;
                  pMPRdatum: TDateTime;
                  pEMMmelk: integer; pOutputFile: PAnsiChar): Integer; stdcall;


                pEMMmelk :
                1 = Melkmeters 2x per dag (avond + ochtend)
                2 = Melkmeters 7 daags gemiddelde
                3 = Melkmeters 3x per dag (avond + ochtend + middag)
                4 = Melkmeters (ochtend + middag + avond op zelfde dag)
                9 = Robot

                pOutputFile is een csv bestand:
                levensnummer ; datummelking ; reknummer ; flesnummer

         */

        public int call_AB2file_rekflesnummers(int pProgID, int pProgramID,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                pCallback ReadDataProc,
                DateTime pMPRdatum,
                int pEMMmelk, string pOutputFile)
        {
            LockObject padlock;
            AB2file_rekflesnummers handle = (AB2file_rekflesnummers)ExecuteProcedureDLLStack(typeof(AB2file_rekflesnummers), "AB2file_rekflesnummers", GetLockList("AB2File.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                        ReadDataProc,
                        pMPRdatum,
                        pEMMmelk, pOutputFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB2file_rekflesnummers(int pProgID, int pProgramID,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                        pCallback ReadDataProc,
                        DateTime pMPRdatum,
                        int pEMMmelk, string pOutputFile);
    }
}
