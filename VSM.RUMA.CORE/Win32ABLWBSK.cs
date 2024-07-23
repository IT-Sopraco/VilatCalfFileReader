using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32ABLWBSK: Win32
    {

        public Win32ABLWBSK()
            : base(false)
        {
        }

        static readonly object padlock = new object();


        private static LockObject[] LockList;

        public static LockObject[] GetLockList()
        {

            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\ABlwbsk.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\ABlwbsk{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        /*
                 
                function AB_lwbsk(
                          pProgID, pProgramID: integer;
                          pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                          pCallback: Pointer; pMPRdatum: TDateTime;
                          pSoortmeting: integer; pLWBSKmodule: PAnsiChar): Integer; stdcall;


                Berekenen van 305dagen productie, LW, NO, BSK.

                pMPRdatum: datum van melkmeting waar de berekening op toegepastt moet worden

                pSoortmeting
                0 = officieel (NRS/ELDA, melkschepper)
                1 = eigen meting

                pLWBSKmodule: nrs lwbsk exe (standaard c:\nrs1\NRSLW044.exe)

         */

        public delegate void pCallback(int PercDone, string Msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int dAB_lwbsk(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum,
                                int pSoortmeting, String pLWBSKmodule);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AB_lwbsk(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum,
                                int pSoortmeting, String pLWBSKmodule)
        {

            LockObject padlock;


            dAB_lwbsk handle = (dAB_lwbsk)ExecuteProcedureDLLStack(typeof(dAB_lwbsk), "AB_lwbsk", GetLockList(), out padlock);
            int tmp = 0;
            try
            {
             
                tmp = handle(pProgID, pProgramID,
                                UBNnr, pHostName, pUsername, Decodeer_PW(pPassword), pLog,
                                ReadDataProc, pMPRdatum,
                                pSoortmeting,  pLWBSKmodule);
               
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            return tmp;
        }

        /*
                AB_voorspellingMelk

                function AB_voorspellingMelk(
                          pProgID, pProgramID: integer;
                          pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                          pCallback: Pointer; pMPRdatum: TDateTime;
                          pLWBSKmodule, pOutputFile: PAnsiChar): Integer; stdcall;

                Bereken van de verwachte melkgift op de mpr datum en mpr datum + 3 weken

                pMPRdatum: melkmeting waar de verwachte gift voor berekend moet worden
                pLWBSKmodule: nrs lwbsk exe (standaard c:\nrs1\NRSLW044.exe)

                pOutputFile: csv bestand met de volgende kolommen

                Levensnr ; verwKgmelk ; verwPercVet ; verwPercEiwit ; verwKgmelk3wkn ; verwPercVet3wkn ; verwPercEiwit3wkn

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int dAB_voorspellingMelk(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum,
                                String pLWBSKmodule, String pOutputFile);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AB_voorspellingMelk(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum,
                                String pLWBSKmodule, String pOutputFile)
        {

            LockObject padlock;


            dAB_voorspellingMelk handle = (dAB_voorspellingMelk)ExecuteProcedureDLLStack(typeof(dAB_voorspellingMelk), "AB_voorspellingMelk", GetLockList(), out padlock);
            int tmp = 0;
            try
            {

                tmp = handle(pProgID, pProgramID,
                                UBNnr, pHostName, pUsername, Decodeer_PW(pPassword), pLog,
                                ReadDataProc, pMPRdatum,
                                pLWBSKmodule, pOutputFile);

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            return tmp;
        }

        /*
                AB_VerwachteMelkgift

                function AB_VerwachteMelkgift(
                          pProgID, pProgramID: integer;
                          pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                          pCallback: Pointer; pMPRdatum: TDateTime;
                          pLWBSKmodule, pOutputFile: PAnsiChar): Integer; stdcall;

                Bereken van de verwachte melkgiften op een bestaande MPR datum (van alle aanwezige dieren op de opgegeven datum).

                pMPRdatum: bestaande melkmeting waar de verwachte gift voor berekend moet worden 
                pLWBSKmodule: nrs lwbsk exe (standaard c:\nrs1\NRSLW044.exe)

                pOutputFile: csv bestand met de volgende kolommen

                Levensnr ; verwKgmelk ; verwPercVet ; verwPercEiwit 

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int dAB_VerwachteMelkgift(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum,
                                String pLWBSKmodule, String pOutputFile);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AB_VerwachteMelkgift(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum,
                                String pLWBSKmodule, String pOutputFile)
        {

            LockObject padlock;


            dAB_VerwachteMelkgift handle = (dAB_VerwachteMelkgift)ExecuteProcedureDLLStack(typeof(dAB_VerwachteMelkgift), "AB_VerwachteMelkgift", GetLockList(), out padlock);
            int tmp = 0;
            try
            {

                tmp = handle(pProgID, pProgramID,
                                UBNnr, pHostName, pUsername, Decodeer_PW(pPassword), pLog,
                                ReadDataProc, pMPRdatum,
                                pLWBSKmodule, pOutputFile);

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            return tmp;
        }
    }
}
