using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32EDINRSEldaMelkcontrol : Win32
    {
        [ThreadStatic]
        private static Win32EDINRSEldaMelkcontrol singleton;

        public static Win32EDINRSEldaMelkcontrol Init()
        {
            if (singleton == null)
            {
                singleton = new Win32EDINRSEldaMelkcontrol();
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
         function AB_leesEldaMelkcontrole(
                pProgID, pProgramID: integer;
                pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                pCallback: Pointer; pMPRdatum: TDateTime;
	            pSoortDiernr: integer;
                pInputFile: PAnsiChar): Integer; stdcall;

                pSoortDiernr
                Hiermee kun je aangeven welk soort diernummer in het bestand staat
                1 = Koenummer
                2 = Oornummer
                3 = Laatste 8 cijfers van levensnummer
                4 = Hele levensnummer
            
                pInputFile  komt uit Win32verwerkVVBmonster : AB_verwerkVVBmonster
                elda pm bestand

         */

        public int call_AB_leesEldaMelkcontrole(int pProgID, int pProgramID,
            string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
            pCallback ReadDataProc, DateTime pMPRdatum,
            int pSoortDiernr,
            string pInputFile)
        {
            LockObject padlock;
            AB_leesEldaMelkcontrole handle = (AB_leesEldaMelkcontrole)ExecuteProcedureDLLStack(typeof(AB_leesEldaMelkcontrole), "AB_leesEldaMelkcontrole", GetLockList("IMPEDINRS.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                                ReadDataProc, pMPRdatum,
                                pSoortDiernr,
                                pInputFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB_leesEldaMelkcontrole(int pProgID, int pProgramID,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
               pCallback ReadDataProc, DateTime pMPRdatum,
               int pSoortDiernr,
               string pInputFile);

        /*
            function AB_controleerEldaMelkcontroleBestand(
              pProgID, pProgramID: integer;
              pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
              pCallback: Pointer; pMPRdatum: TDateTime;
	          pSoortDiernr: integer;
              pInputFile, pOutPutfile: PAnsiChar): Integer; stdcall;

         */

        public int call_AB_controleerEldaMelkcontroleBestand(int pProgID, int pProgramID,
                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                    pCallback ReadDataProc,DateTime pMPRdatum,
                    int pSoortDiernr,
                    string pInputFile, string pOutputFile )
        {
            LockObject padlock;
            AB_controleerEldaMelkcontroleBestand handle = (AB_controleerEldaMelkcontroleBestand)ExecuteProcedureDLLStack(typeof(AB_controleerEldaMelkcontroleBestand), "AB_controleerEldaMelkcontroleBestand", GetLockList("IMPEDINRS.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                                ReadDataProc,  pMPRdatum,
                                pSoortDiernr,
                                pInputFile,   pOutputFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB_controleerEldaMelkcontroleBestand(int pProgID, int pProgramID,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
               pCallback ReadDataProc, DateTime pMPRdatum,
               int pSoortDiernr,
               string pInputFile, string pOutputFile);
    }
}
