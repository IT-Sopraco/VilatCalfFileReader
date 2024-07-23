﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32AB_Stocklijst_Sopraco : Win32
    {
        [ThreadStatic]
        private static Win32AB_Stocklijst_Sopraco singleton;

        public static Win32AB_Stocklijst_Sopraco Init()
        {
            if (singleton == null)
            {
                singleton = new Win32AB_Stocklijst_Sopraco();
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
         function AB_function_Stocklijst_Sopraco(pProgID, pProgramID, pSoortBestand: Integer;
                                 pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                                 pCallBack: Pointer;
                                 pDatum: TDateTime; pSort: Integer): Integer; stdcall; export;
         */

        public int call_AB_function_Stocklijst_Sopraco(int pProgID, int pProgramID, int pSoortBestand,
                   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                   pCallback ReadDataProc,
                   DateTime TDateTime, int pSort)
        {
            LockObject padlock;
            AB_function_Stocklijst_Sopraco handle = (AB_function_Stocklijst_Sopraco)ExecuteProcedureDLLStack(typeof(AB_function_Stocklijst_Sopraco), "AB_function_Stocklijst_Sopraco", GetLockList("AB_Stocklijst_Sopraco.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               TDateTime,  pSort);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB_function_Stocklijst_Sopraco(int pProgID, int pProgramID, int pSoortBestand,
                   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                   pCallback ReadDataProc,
                   DateTime TDateTime, int pSort);
    }
}
