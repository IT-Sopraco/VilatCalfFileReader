using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE
{
    public class Win32Milkcont : Win32
    {

        [ThreadStatic]
        private static Win32Milkcont singleton;

        public static Win32Milkcont Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Milkcont();
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
            function lst_AB_CelgetalUitslag(
                       pProgID, pProgramID, pSoortBestand: Integer;
                       pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                       pCallBack: Pointer;
                       pDatum: TDateTime; pToonAlleDieren: integer): Integer; stdcall;

            pToonAlleDieren: 0/1
        */


        public int call_lst_AB_CelgetalUitslag(int pProgID, int pProgramID, int pLogoID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pDatum, int pToonAlleDieren)
        {
            LockObject padlock;
            lst_AB_CelgetalUitslag handle = (lst_AB_CelgetalUitslag)ExecuteProcedureDLLStack(typeof(lst_AB_CelgetalUitslag), "lst_AB_CelgetalUitslag", GetLockList("MILKCONT.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pLogoID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pDatum, pToonAlleDieren);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_CelgetalUitslag(int pProgID, int pProgramID, int pLogoID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pDatum, int pToonAlleDieren);


        /*
         function lst_AB_Snelzicht(
               pProgID, pProgramID, pSoortBestand: Integer;
               pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
               pCallBack: Pointer;
               pDatum: TDateTime; pVerwMelkFile: PAnsiChar): Integer; stdcall;
         */

        public int call_lst_AB_Snelzicht(int pProgID, int pProgramID, int pSoortBestand,
                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                       pCallback ReadDataProc,
                       DateTime pDatum, string pVerwMelkFile)
        {
            LockObject padlock;
            lst_AB_Snelzicht handle = (lst_AB_Snelzicht)ExecuteProcedureDLLStack(typeof(lst_AB_Snelzicht), "lst_AB_Snelzicht", GetLockList("MILKCONT.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pDatum, pVerwMelkFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Snelzicht(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pDatum, string pVerwMelkFile);
        /*
             function AB_CelgetalDatums(
                  pProgID, pProgramID: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pDatums: PAnsiChar; pMaxStrLen: integer): Integer; stdcall;
         */





        public static LockObject[] GetLockList()
        {

            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\MILKCONT.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\MILKCONT{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void AB_CelgetalDatums(int pProgID, int pProgramID,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                               pCallback ReadDataProc,
                               ref StringBuilder Datums, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_AB_CelgetalDatums(int pProgID, int pProgramID,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                               pCallback ReadDataProc,
                               ref String pDatums, int pMaxStrLen)
        {


            LockObject padlock;


            AB_CelgetalDatums handle = (AB_CelgetalDatums)ExecuteProcedureDLLStack(typeof(AB_CelgetalDatums), "AB_CelgetalDatums", GetLockList(), out padlock);
            try
            {
                StringBuilder lDatums = new StringBuilder(pMaxStrLen);
                lDatums.EnsureCapacity(pMaxStrLen);
                handle(pProgID, pProgramID,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                               ReadDataProc,
                               ref lDatums, pMaxStrLen);


                pDatums = lDatums.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        /*
         function lst_AB_MPRuitslag(pProgID, pProgramID, pSoortBestand: Integer;
                           pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                           pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                           pCallback: Pointer; pMPRdatum: TDateTime;
                           pExtraGegevens, pSubTotaalPerRantsoenGroep,
                           pSoortDiernr, pSoortDieren, pSoortHok, pAlleenSubtotalen,
                           pAantalBesteSlechtste, pSortering: integer;
                           pHoknr: PAnsiChar): Integer; stdcall;
         */


        public int call_lst_AB_MPRuitslag(int pProgID, int pProgramID, int pLogoID, int pSoortBestand,
                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                       string pResourceFolder, string pTaalcode, int pTaalnr,
                       pCallback ReadDataProc,
                       DateTime pMPRdatum, int pExtraGegevens, int pSubTotaalPerRantsoenGroep,
                       int pSoortDiernr, int pSoortDieren, int pSoortHok, int pAlleenSubtotalen,
                       int pAantalBesteSlechtste, int pSortering,
                       string pHoknr)
        {
            LockObject padlock;
            lst_AB_MPRuitslag handle = (lst_AB_MPRuitslag)ExecuteProcedureDLLStack(typeof(lst_AB_MPRuitslag), "lst_AB_MPRuitslag", GetLockList("MILKCONT.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pLogoID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               pResourceFolder, pTaalcode, pTaalnr,
                               ReadDataProc,
                               pMPRdatum, pExtraGegevens, pSubTotaalPerRantsoenGroep,
                               pSoortDiernr, pSoortDieren, pSoortHok, pAlleenSubtotalen,
                               pAantalBesteSlechtste, pSortering,
                               pHoknr);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_MPRuitslag(int pProgID, int pProgramID, int pLogoID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               string pResourceFolder, string pTaalcode, int pTaalnr,
                               pCallback ReadDataProc,
                               DateTime pMPRdatum, int pExtraGegevens, int pSubTotaalPerRantsoenGroep,
                               int pSoortDiernr, int pSoortDieren, int pSoortHok, int pAlleenSubtotalen,
                               int pAantalBesteSlechtste, int pSortering,
                               string pHoknr);

        /*
            Hierbij de lijst "Stoplicht celgetal". Deze is alleen voor melkkoeien administraties

            function lst_AB_StoplichtCelgetal(
                               pProgID, pProgramID, pSoortBestand: Integer;
                               pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                               pCallback: Pointer): Integer; stdcall; 
         */
        public int call_lst_AB_StoplichtCelgetal(int pProgID, int pProgramID, int pSoortBestand,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
               pCallback ReadDataProc)
        {
            LockObject padlock;
            lst_AB_StoplichtCelgetal handle = (lst_AB_StoplichtCelgetal)ExecuteProcedureDLLStack(typeof(lst_AB_StoplichtCelgetal), "lst_AB_StoplichtCelgetal", GetLockList("MILKCONT.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_StoplichtCelgetal(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc);

        /*
         
        function lst_AB_KoeAttenties(
                     pProgID, pProgramID, pSoortBestand: Integer;
                     pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                     pCallback: Pointer;
                     pMPRdatum: TDateTime; pVerwMelkFile: PAnsiChar): Integer; stdcall; 
         */

       public int call_lst_AB_KoeAttenties(int pProgID, int pProgramID, int pSoortBestand,
       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
       pCallback ReadDataProc,DateTime pMPRdatum, string pVerwMelkFile)
        {
            LockObject padlock;
            lst_AB_KoeAttenties handle = (lst_AB_KoeAttenties)ExecuteProcedureDLLStack(typeof(lst_AB_KoeAttenties), "lst_AB_KoeAttenties", GetLockList("MILKCONT.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,pMPRdatum,  pVerwMelkFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_KoeAttenties(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc, DateTime pMPRdatum, string pVerwMelkFile);

        /*
             Hierbij de lijst MPR Voeding (alleen voor melkkoeien).

                function lst_AB_MPRvoeding(pProgID, pProgramID, pSoortBestand: Integer;
                                           pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                                           pCallback: Pointer; pMPRdatum: TDateTime;
                                           pVerwMelkFile: PAnsiChar): Integer; stdcall;


                pVerwMelkFile: bestand met verwachte melkgiften. Deze kan aangemaakt worden met ABLWBSK.DLL -> AB_VerwachteMelkgift. 
                     
         
             */
        public int call_lst_AB_MPRvoeding(int pProgID, int pProgramID, int pSoortBestand,
        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
        pCallback ReadDataProc, DateTime pMPRdatum, string pVerwMelkFile)
        {
            LockObject padlock;
            lst_AB_MPRvoeding handle = (lst_AB_MPRvoeding)ExecuteProcedureDLLStack(typeof(lst_AB_MPRvoeding), "lst_AB_MPRvoeding", GetLockList("MILKCONT.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc, pMPRdatum, pVerwMelkFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_MPRvoeding(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc, DateTime pMPRdatum, string pVerwMelkFile);
    }
}
