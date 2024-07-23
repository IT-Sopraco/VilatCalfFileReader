using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Afvoerlijst:Win32
    {
        [ThreadStatic]
        private static Win32Afvoerlijst singleton;

        public static Win32Afvoerlijst Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Afvoerlijst();
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
         function lst_AB_AfgevoerdeKoeien(
                   pProgID, pProgramID, pSoortBestand: Integer;
                   pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                   pCallback: Pointer;
                   pEinddatum: TDateTime; pSortering: integer): Integer; stdcall; 
         */
        public int call_lst_AB_AfgevoerdeKoeien(int pProgID, int pProgramID, int pSoortBestand,
                                           string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                           pCallback ReadDataProc,
                                           DateTime pEinddatum, int pSortering)
        {
            lock (typeof(Win32Afvoerlijst))
            {
                string sFilename = "AFVOERLIJST.dll";

                lst_AB_AfgevoerdeKoeien handle = (lst_AB_AfgevoerdeKoeien)ExecuteProcedureDLL(typeof(lst_AB_AfgevoerdeKoeien), sFilename, "lst_AB_AfgevoerdeKoeien");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pEinddatum, pSortering);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_AfgevoerdeKoeien(int pProgID, int pProgramID, int pSoortBestand,
                                       string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                       pCallback ReadDataProc,
                                       DateTime pEinddatum, int pSortering);
    }
}
