using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32AB2EDINRS:Win32
    {
        public Win32AB2EDINRS()
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
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\AB2EDINRS.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\AB2EDINRS{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }
        public delegate void pCallback(int PercDone, string Msg);
        /*
                AB2edinrs_MPR

                function AB2edinrs_MPR(
                          pProgID, pProgramID: integer;
                          pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                          pCallback: Pointer; pMPRdatum: TDateTime;
                          pSoort: integer; pInputFile, pOutputFile: PAnsiChar): Integer; stdcall;


                pMPRdatum: om alleen gegevens van 1 mpr uitslag in het bestand te zetten

                pSoort
                0 = alles
                1 = alleen melkgegevens
                2 = Rovecom (melkgegevens + voorspelling)

                pInputFile: bestand met de verwachte/voorspelde giften 
                (optioneel, voorlopig alleen voor Rovecom). 
                Dit bestand kan worden gemaakt met AB_voorspellingMelk (ABlwbsk.dll)  eg: Win32ABLWBSK


                     * function AB2edinrs_MPR(
                      pProgID, pProgramID: integer;
                      pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                      pCallback: Pointer;
                      pMPRdatum, pMutatiesBegin, pMutatiesEind: TDateTime;
                      pSoort: integer; pInputFile, pOutputFile: PAnsiChar): Integer; stdcall;
         
                         * function AB2edinrs_MPR(
                          pProgID, pProgramID: integer;
                          pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                          pCallback: Pointer;
                          pMPRdatum, pMutatiesBegin, pMutatiesEind: TDateTime;
                          pSoort, pEMMmelk: integer; 
                pInputFile, pOutputFile: PAnsiChar): Integer; stdcall;
    
 
                        function AB2edinrs_MPR(
                        pProgID, pProgramID: integer;
                        pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                        pCallback: Pointer;
                        pMPRdatum, pMutatiesBegin, pMutatiesEind: TDateTime;
                        pSoort, pEMMmelk, pAlleenLaatsteLactatie: integer;
                        pInputFile, pOutputFile: PAnsiChar): Integer; stdcall;


            function AB2edinrs_MPR(
          pProgID, pProgramID: integer;
                      pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                      pCallback: Pointer;
                      pMPRdatum, pMutatiesBegin, pMutatiesEind: TDateTime;
                      pSoort, pEMMmelk, pAlleenLaatsteLactatie: integer;
                      pInputFile, pOutputFile, pRVOusername, 
                      pRVOpassword: PAnsiChar): Integer; stdcall;


         * 
        */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int dAB2edinrs_MPR(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum, DateTime pMutatiesBegin, DateTime pMutatiesEind,
                                int pSoort,int pEMMmelk, int pAlleenLaatsteLactatie,String pInputFile, String pOutputFile, String pRVOusername, String pRVOpassword);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AAB2edinrs_MPR(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc, DateTime pMPRdatum, DateTime pMutatiesBegin, DateTime pMutatiesEind,
                                int pSoort,int pEMMmelk, int pAlleenLaatsteLactatie, String pInputFile, String pOutputFile, String pRVOusername, String pRVOpassword)
        {

            LockObject padlock;


            dAB2edinrs_MPR handle = (dAB2edinrs_MPR)ExecuteProcedureDLLStack(typeof(dAB2edinrs_MPR), "AB2edinrs_MPR", GetLockList(), out padlock);
            int tmp = 0;
            try
            {

                tmp = handle(pProgID, pProgramID,
                                UBNnr, pHostName, pUsername, Decodeer_PW(pPassword), pLog,
                                ReadDataProc, pMPRdatum, pMutatiesBegin, pMutatiesEind,
                                pSoort, pEMMmelk, pAlleenLaatsteLactatie, pInputFile, pOutputFile, pRVOusername, Decodeer_PW(pRVOpassword));

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            return tmp;
        }
    }
}
