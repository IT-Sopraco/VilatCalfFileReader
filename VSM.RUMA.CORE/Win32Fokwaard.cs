using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Fokwaard: Win32
    {
        [ThreadStatic]
        private static Win32Fokwaard singleton;

        public static Win32Fokwaard Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Fokwaard();
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
     * BUG 2078
        FOKWAARD.DLL
        -------------
        function lst_AB_Fokwaarden(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pSoortDieren, pSoortlijst, pAniidBok: integer;
          pHoknr, pCsvKolommen: PAnsiChar; pSortering: integer): Integer; stdcall;
          
        De fokwaarde datum kan/moet nu aan de dll worden doorgegeven:
        function lst_AB_Fokwaarden(
          pProgID, pProgramID, pSoortBestand: Integer;
          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
          pCallBack: Pointer;
          pFWdatum: TDateTime;
          pSoortDieren, pSoortlijst, pAniidBok: integer;
          pHoknr, pCsvKolommen: PAnsiChar; pSortering: integer): Integer; stdcall;
         */


        public int call_lst_AB_Fokwaarden(int pProgID, int pProgramID,int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pFWdatum,
                               int pSoortDieren, int pSoortlijst, int pAniidBok,
                               string pHoknr, string pCsvKolommen,
                               int pSortering)
        {
            LockObject padlock;
            lst_AB_Fokwaarden handle = (lst_AB_Fokwaarden)ExecuteProcedureDLLStack(typeof(lst_AB_Fokwaarden), "lst_AB_Fokwaarden", GetLockList("FOKWAARD.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pFWdatum,
                               pSoortDieren,  pSoortlijst,  pAniidBok,
                               pHoknr, pCsvKolommen,
                               pSortering);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Fokwaarden(int pProgID, int pProgramID,int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pFWdatum,
                               int pSoortDieren, int pSoortlijst, int pAniidBok,
                               string pHoknr, string pCsvKolommen,
                               int pSortering);


        /* 
          function lst_AB_FokwaardenSelectieKolommen(
          pSoortDieren: integer; pLog, pBestand: PAnsiChar): Integer; stdcall; 
        
        */

        public int call_lst_AB_FokwaardenSelectieKolommen(int pSoortDieren,string pLog,string pBestand)
        {
            LockObject padlock;
            lst_AB_FokwaardenSelectieKolommen handle = (lst_AB_FokwaardenSelectieKolommen)ExecuteProcedureDLLStack(typeof(lst_AB_FokwaardenSelectieKolommen), "lst_AB_FokwaardenSelectieKolommen", GetLockList("FOKWAARD.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pSoortDieren, pLog, pBestand);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_FokwaardenSelectieKolommen(int pSoortDieren, string pLog, string pBestand);
    }
}
