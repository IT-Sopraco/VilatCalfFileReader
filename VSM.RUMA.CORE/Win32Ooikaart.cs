using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VSM.RUMA.CORE
{
    public class Win32Ooikaart : Win32
    {
        [ThreadStatic]
        private static Win32Ooikaart singleton;

        public static Win32Ooikaart Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Ooikaart();
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
         function lst_AB_Schapenkaart(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              pAniId: Integer; pRamProdUitgebreid: Boolean): Integer; 
         */

        public int call_lst_AB_Schapenkaart(int pProgID, int pProgramID, int pSoortBestand,
                                            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                            string pResourceFolder, string pTaalcode, int pTaalnr,
                                            pCallback ReadDataProc,
                                            int pAniId, bool pRamProdUitgebreid)
        {
            LockObject padlock;
            lst_AB_Schapenkaart handle = (lst_AB_Schapenkaart)ExecuteProcedureDLLStack(typeof(lst_AB_Schapenkaart), "lst_AB_Schapenkaart", GetLockList("OOIKAART.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                              pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                              pResourceFolder, pTaalcode, pTaalnr,
                              ReadDataProc,
                              pAniId, pRamProdUitgebreid);



            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }
        delegate int lst_AB_Schapenkaart(int pProgID, int pProgramID, int pSoortBestand,
                                 string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                 string pResourceFolder, string pTaalcode, int pTaalnr,
                                 pCallback ReadDataProc,
                                 int pAniId,bool pRamProdUitgebreid);
       
        /*
         function lst_AB_Afstambewijs(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              plAniId, pSoortLijst: Integer): Integer; export; stdcall; 
         */
        public int call_lst_AB_Afstambewijs(int pProgID, int pProgramID, int pSoortBestand,
                                            string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                            string pResourceFolder, string pTaalcode, int pTaalnr,
                                            pCallback ReadDataProc,
                                            int pAniId,  int pSoortLijst)
        {
            LockObject padlock;
            lst_AB_Afstambewijs handle = (lst_AB_Afstambewijs)ExecuteProcedureDLLStack(typeof(lst_AB_Afstambewijs), "lst_AB_Afstambewijs", GetLockList("OOIKAART.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pAniId,   pSoortLijst);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Afstambewijs(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         string pResourceFolder, string pTaalcode, int pTaalnr,
                                         pCallback ReadDataProc,
                                         int pAniId, int pSoortLijst);

        /*
         function lst_AB_Afstambewijs2(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
              pCallback: Pointer;
              plAniId, pSoortLijst: Integer) : Integer; stdcall;
         */

        public int call_lst_AB_Afstambewijs2(int pProgID, int pProgramID, int pSoortBestand,
                                    string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                    string pResourceFolder, string pTaalcode, int pTaalnr,
                                    pCallback ReadDataProc,
                                    int pAniId, int pSoortLijst)
        {
            LockObject padlock;
            lst_AB_Afstambewijs2 handle = (lst_AB_Afstambewijs2)ExecuteProcedureDLLStack(typeof(lst_AB_Afstambewijs2), "lst_AB_Afstambewijs2", GetLockList("OOIKAART.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 pResourceFolder, pTaalcode, pTaalnr,
                                 ReadDataProc,
                                 pAniId, pSoortLijst);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Afstambewijs2(int pProgID, int pProgramID, int pSoortBestand,
                                         string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                         string pResourceFolder, string pTaalcode, int pTaalnr,
                                         pCallback ReadDataProc,
                                         int pAniId, int pSoortLijst);
    }
}
