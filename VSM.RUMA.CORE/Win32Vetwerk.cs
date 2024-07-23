using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Vetwerk : Win32
    {
        [ThreadStatic]
        private static Win32Vetwerk singleton;

        public static Win32Vetwerk Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Vetwerk();
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
         
         function lst_AB_Koewerk(
                   pProgID, pProgramID, pSoortBestand: Integer;
                   pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                   pCallback: Pointer; pSortering: integer): Integer; stdcall;
         
         */

        public int call_lst_AB_Koewerk(int pProgID, int pProgramID, int pSoortBestand,
                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                       pCallback ReadDataProc,
                       int pSortering)
        {
            LockObject padlock;
            lst_AB_Koewerk handle = (lst_AB_Koewerk)ExecuteProcedureDLLStack(typeof(lst_AB_Koewerk), "lst_AB_Koewerk", GetLockList("VetwerkLijsten.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pSortering);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Koewerk(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               int pSortering);

        /*
             function lst_AB_Koeinfo(
                       pProgID, pProgramID, pSoortBestand: Integer;
                       pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                       pCallback: Pointer; pAniid: integer): Integer; stdcall;
        */

        public int call_lst_AB_Koeinfo(int pProgID, int pProgramID, int pSoortBestand,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
               pCallback ReadDataProc,
               int pAniid)
        {
            LockObject padlock;
            lst_AB_Koeinfo handle = (lst_AB_Koeinfo)ExecuteProcedureDLLStack(typeof(lst_AB_Koeinfo), "lst_AB_Koeinfo", GetLockList("VetwerkLijsten.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pAniid);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Koeinfo(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               int pAniid);
    }
}

