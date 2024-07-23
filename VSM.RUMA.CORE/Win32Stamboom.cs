using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Stamboom : Win32
    {

      
        [ThreadStatic]
        private static Win32Stamboom singleton;

        public static Win32Stamboom Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Stamboom();
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
         function lst_AB_Stamboom(pProgID, pProgramID, pSoortBestand: Integer;
                         pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                         pCallBack: Pointer;
                         pAniId: Integer): Integer; stdcall;
         
         * 19-10-2016
         * function lst_AB_Stamboom(
                          pProgID, pProgramID, pSoortBestand: Integer;
                          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                          pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                          pCallBack: Pointer;
                          pAniId: Integer): Integer; stdcall; export;
         */
        public int call_lst_AB_Stamboom(int pProgID, int pProgramID, int pSoortBestand,
        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
        string pResourceFolder, string pTaalcode, int pTaalnr,
        pCallback ReadDataProc,
        int pAniId)
        {
            LockObject padlock;
            lst_AB_Stamboom handle = (lst_AB_Stamboom)ExecuteProcedureDLLStack(typeof(lst_AB_Stamboom), "lst_AB_Stamboom", GetLockList("STAMBOOM.DLL"), out padlock);
            int tmp;
            try
            {

                tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pAniId);

            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Stamboom(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder, string pTaalcode, int pTaalnr,
                        pCallback ReadDataProc,
                        int pAniId);
    }
}
