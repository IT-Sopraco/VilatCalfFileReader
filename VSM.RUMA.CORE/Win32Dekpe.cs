using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE
{
    public class Win32Dekpe:Win32
    {
        [ThreadStatic]
        private static Win32Dekpe singleton;

        public static Win32Dekpe Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Dekpe();
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
            function lst_AB_DekDrachtAflam(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pBeginDatum, pEindDatum: TDateTime; pSoortBok, pBokid,
                  pSubtotaalPerBok, pGeitDetails, pAlleBedrijven: integer): Integer; stdcall;
         */

        public int call_lst_AB_DekDrachtAflam(int pProgID, int pProgramID, int pSoortBestand,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
               pCallback ReadDataProc,
               DateTime pBeginDatum, DateTime pEindDatum, int pSoortBok, int pBokid,
                 int pSubtotaalPerBok, int pGeitDetails, int pAlleBedrijven)
        {
            LockObject padlock;
            lst_AB_DekDrachtAflam handle = (lst_AB_DekDrachtAflam)ExecuteProcedureDLLStack(typeof(lst_AB_DekDrachtAflam), "lst_AB_DekDrachtAflam", GetLockList("DEKPE.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pBeginDatum, pEindDatum, pSoortBok, pBokid,
                               pSubtotaalPerBok, pGeitDetails, pAlleBedrijven);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_DekDrachtAflam(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pBeginDatum, DateTime pEindDatum, int pSoortBok, int pBokid,
                               int pSubtotaalPerBok, int pGeitDetails, int pAlleBedrijven);

        /*
          function lst_AB_periodeoverzichtdekkingen(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pCallBack: Pointer;
                  pBeginDatum, pEindDatum: TDateTime): Integer; stdcall;
         * 
         * function lst_AB_periodeoverzichtdekkingen(
                  pProgID, pProgramID, pSoortBestand: Integer;
                  pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                  pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                  pCallBack: Pointer;
                  pBeginDatum, pEindDatum: TDateTime): Integer; stdcall;
         */

        public int call_lst_AB_periodeoverzichtdekkingen(int pProgID, int pProgramID, int pSoortBestand,
                   string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                   string pResourceFolder, string pTaalcode, int pTaalnr,
                   pCallback ReadDataProc,
                   DateTime pBeginDatum, DateTime pEindDatum )
        {
            LockObject padlock;
            lst_AB_periodeoverzichtdekkingen handle = (lst_AB_periodeoverzichtdekkingen)ExecuteProcedureDLLStack(typeof(lst_AB_periodeoverzichtdekkingen), "lst_AB_periodeoverzichtdekkingen", GetLockList("DEKPE.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               pResourceFolder, pTaalcode, pTaalnr,
                               ReadDataProc,
                               pBeginDatum, pEindDatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_periodeoverzichtdekkingen(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               string pResourceFolder, string pTaalcode, int pTaalnr,
                               pCallback ReadDataProc,
                               DateTime pBeginDatum, DateTime pEindDatum);
    }
}
