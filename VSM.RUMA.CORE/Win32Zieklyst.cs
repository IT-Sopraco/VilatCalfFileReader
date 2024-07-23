using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE
{
    public class Win32Zieklyst : Win32
    {
        [ThreadStatic]
        private static Win32Zieklyst singleton;

        public static Win32Zieklyst Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Zieklyst();
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
            Hierbij de lijst "Klauwgezondheid (Digiklauw)". Alleen voor melk/zoogkoeien.
            pDatum moet je door de gebruiker laten kiezen adhv de datums uit agrofactuur.KLAUWGZHBEDRIJF 
         * (kgb_TrimmingDate). 
 
            function lst_AB_Klauwgezondheid(
                   pProgID, pProgramID, pSoortBestand: Integer;
                   pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                   pCallback: Pointer;
                   pDatum: TDateTime): Integer; stdcall; 
        */
        public int call_lst_AB_Klauwgezondheid(int pProgID, int pProgramID, int pSoortBestand,
        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
        pCallback ReadDataProc,
        DateTime pDatum)
        {
            LockObject padlock;
            lst_AB_Klauwgezondheid handle = (lst_AB_Klauwgezondheid)ExecuteProcedureDLLStack(typeof(lst_AB_Klauwgezondheid), "lst_AB_Klauwgezondheid", GetLockList("ZIEKLYST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               ReadDataProc,
                               pDatum);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Klauwgezondheid(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               pCallback ReadDataProc,
                               DateTime pDatum);
    }
}
