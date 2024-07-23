using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
    public class Win32stolist : Win32
    {
        [ThreadStatic]
        private static Win32stolist singleton;

        public static Win32stolist Init()
        {
            if (singleton == null)
            {
                singleton = new Win32stolist();
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






        public void call_lst_AB_STO_veevervanging(int pProgID, int pProgramID, int pSoortBestand,
                                                  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
                                                  pCallback ReadDataProc,
                                                  DateTime pPeilDatum)
        {
            lock (typeof(Win32stolist))
            {
                lst_AB_STO_veevervanging handle = (lst_AB_STO_veevervanging)ExecuteProcedureDLL(typeof(lst_AB_STO_veevervanging), "STOLIST.DLL", "lst_AB_STO_veevervanging");

                handle(pProgID, pProgramID, pSoortBestand,
                             pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLogdir,
                             ReadDataProc,
                             pPeilDatum);

                FreeDLL("STOLIST.DLL");
            }
        }

        delegate void lst_AB_STO_veevervanging(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLogdir,
                                               pCallback ReadDataProc,
                                               DateTime pPeilDatum);


        /*
 function lst_AB_STO_vruchtbaarheid(
         pProgID, pProgramID, pSoortBestand: Integer;
         pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
         pCallback: Pointer;
         pPeildatum: TDateTime): integer; stdcall;
 */
        public int call_lst_AB_STO_vruchtbaarheid(int pProgID, int pProgramID, int pSoortBestand,
                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                       pCallback ReadDataProc,
                       DateTime pPeildatum)
        {
            LockObject padlock;
            lst_AB_STO_vruchtbaarheid handle = (lst_AB_STO_vruchtbaarheid)ExecuteProcedureDLLStack(typeof(lst_AB_STO_vruchtbaarheid), "lst_AB_STO_vruchtbaarheid", GetLockList("STOLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pPeildatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_STO_vruchtbaarheid(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pPeildatum);

        /*
         function lst_AB_STO_dieroverzicht(
                   pProgID, pProgramID, pSoortBestand: Integer;
                   pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                   pCallback: Pointer; pSortering: integer): Integer; stdcall;
         
         * function lst_AB_STO_dieroverzicht(
                   pProgID, pProgramID, pSoortBestand: Integer;
                   pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                   pCallback: Pointer;
                   pDatum: TDateTime; pSortering: integer): Integer; stdcall;
         */
        public int call_lst_AB_STO_dieroverzicht(int pProgID, int pProgramID, int pSoortBestand,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
               pCallback ReadDataProc,
               DateTime pDatum, int pSortering)
        {
            LockObject padlock;
            lst_AB_STO_dieroverzicht handle = (lst_AB_STO_dieroverzicht)ExecuteProcedureDLLStack(typeof(lst_AB_STO_dieroverzicht), "lst_AB_STO_dieroverzicht", GetLockList("STOLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pDatum, pSortering);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_STO_dieroverzicht(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pDatum, int pSortering);


        /*
         function lst_AB_BedrijfsVruchtbaarheid(
                       pProgID, pProgramID, pSoortBestand: Integer;
                       pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                       pCallback: Pointer;
                       pDatum: TDateTime;
                       pToonDieren, pWachtperiode, pDraagtijd: integer): Integer; stdcall;

                        pToonDieren: 0/1
                        pWachtperiode: aantal dagen dat een veehouder wacht voordat hij een koe voor de eerste keer insemineert na afkalven. Moet de veehouder dus zelf in kunnen geven op het instelscherm voor de lijst
                        pDraagtijd: draagtijd op het bewuste bedrijf 
         */

        public int call_lst_AB_BedrijfsVruchtbaarheid(int pProgID, int pProgramID, int pSoortBestand,
           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
           pCallback ReadDataProc,
           DateTime pDatum, int pToonDieren, int pWachtperiode, int pDraagtijd)
        {
            LockObject padlock;
            lst_AB_BedrijfsVruchtbaarheid handle = (lst_AB_BedrijfsVruchtbaarheid)ExecuteProcedureDLLStack(typeof(lst_AB_BedrijfsVruchtbaarheid), "lst_AB_BedrijfsVruchtbaarheid", GetLockList("STOLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pDatum, pToonDieren, pWachtperiode, pDraagtijd);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_BedrijfsVruchtbaarheid(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pDatum, int pToonDieren, int pWachtperiode, int pDraagtijd);
    }





}
