using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32verwerkVVBmonster: Win32
    {
        [ThreadStatic]
        private static Win32verwerkVVBmonster singleton;

        public static Win32verwerkVVBmonster Init()
        {
            if (singleton == null)
            {
                singleton = new Win32verwerkVVBmonster();
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
         function AB_verwerkVVBmonster(
              pProgID, pProgramID: integer;
              pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
              pCallback: Pointer;
              pVVBbestand, pOutputFile: PAnsiChar;
              var pMPRdatum: TDateTime): Integer; stdcall;

              leest een vvb monster bestand in
              pOutputFile  is voor  Win32EDINRSEldaMelkcontrol  impedinrs.dll -> AB_leesEldaMelkcontrole
         */

        public int call_AB_verwerkVVBmonster(int pProgID, int pProgramID, 
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
               pCallback ReadDataProc,
               string pVVBbestand,string pOutputFile,out DateTime pMPRdatum)
        {
            LockObject padlock;
            AB_verwerkVVBmonster handle = (AB_verwerkVVBmonster)ExecuteProcedureDLLStack(typeof(AB_verwerkVVBmonster), "AB_verwerkVVBmonster", GetLockList("verwerkVVBmonster.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                               ReadDataProc,
                               pVVBbestand, pOutputFile,out pMPRdatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB_verwerkVVBmonster(int pProgID, int pProgramID,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
               pCallback ReadDataProc,
               string pVVBbestand, string pOutputFile,out DateTime pMPRdatum);
    }
}
