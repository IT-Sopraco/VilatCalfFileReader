using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32RestList : Win32
    {
        [ThreadStatic]
        private static Win32RestList singleton;

        public static Win32RestList Init()
        {
            if (singleton == null)
            {
                singleton = new Win32RestList();
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


        ///////////////////////////////////////////////////

        /*
         function lst_AB_Restlijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pDatum: TDateTime; pMinKgRestVoerX100, pMinPercRestvoer,
              pMinPercAfwMelk, p9voersoorten, pSubtotalen,
              pAantDecimalenVoer: integer;
              pAlleenHok: PAnsiChar): Integer; stdcall;
         * 
         * 
         * function lst_AB_Restlijst(
              pProgID, pProgramID, pSoortBestand: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
              pCallBack: Pointer;
              pDatum: TDateTime; pMinKgRestVoerX100, pMinPercRestvoer,
              pMinPercAfwMelk, p9voersoorten, pSubtotalen,
              pAantDecimalenVoer, pAlleenDierenMetVoer: integer;
              pAlleenHok: PAnsiChar): Integer; stdcall;
         * 
         */
        public int call_lst_AB_Restlijst(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        pCallback ReadDataProc,
                        DateTime pDatum, int pMinKgRestVoerX100, int pMinPercRestvoer,
                        int pMinPercAfwMelk, int p9voersoorten, int pSubtotalen,
                        int pAantDecimalenVoer,int pAlleenDierenMetVoer,
                        string pAlleenHok)
        {
            LockObject padlock;
            lst_AB_Restlijst handle = (lst_AB_Restlijst)ExecuteProcedureDLLStack(typeof(lst_AB_Restlijst), "lst_AB_Restlijst", GetLockList("RESTLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        ReadDataProc,
                        pDatum, pMinKgRestVoerX100, pMinPercRestvoer,
                        pMinPercAfwMelk, p9voersoorten, pSubtotalen,
                        pAantDecimalenVoer, pAlleenDierenMetVoer,
                        pAlleenHok);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Restlijst(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        pCallback ReadDataProc,
                        DateTime pDatum, int pMinKgRestVoerX100, int pMinPercRestvoer,
                        int pMinPercAfwMelk, int p9voersoorten, int pSubtotalen,
                        int pAantDecimalenVoer, int pAlleenDierenMetVoer,
                        string pAlleenHok);

        ///////////////////////////////////////////////////
        /*
         * zie mail 5-8-14 Lijst MS Optima box
         Ik heb de bezoek- en supplementlijst aangepast en een nieuwe lijst gemaakt als vervanger van de restlijst (Keto attentie):

            function lst_AB_KetoAttentie(
                      pProgID, pProgramID, pSoortBestand: Integer;
                      pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                      pCallBack: Pointer;
                      pDatum: TDateTime): Integer; stdcall;
         * 
         * function lst_AB_KetoAttentie(
                      pProgID, pProgramID, pSoortBestand: Integer;
                      pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog,
                      pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                      pCallBack: Pointer;
                      pDatum: TDateTime): Integer; stdcall;
         */


        public int call_lst_AB_KetoAttentie(int pProgID, int pProgramID, int pSoortBestand,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                string pResourceFolder, string pTaalcode, int pTaalnr, 
                pCallback ReadDataProc,
                DateTime pDatum)
        {
            LockObject padlock;
            lst_AB_KetoAttentie handle = (lst_AB_KetoAttentie)ExecuteProcedureDLLStack(typeof(lst_AB_KetoAttentie), "lst_AB_KetoAttentie", GetLockList("RESTLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr, 
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

        delegate int lst_AB_KetoAttentie(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder, string pTaalcode, int pTaalnr, 
                        pCallback ReadDataProc,
                        DateTime pDatum);



        ///////////////////////////////////////////////////

        /*

   
                function lst_AB_VoerboxBezoeken(
                          pProgID, pProgramID, pSoortBestand: Integer;
                          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog,
                          pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                          pCallBack: Pointer;
                          pDatum: TDateTime; pSortering: integer): Integer; stdcall;

            pAantalDagen
                0 = 1 week
                1 = 4 weken
                function lst_AB_VoerboxBezoeken(
                          pProgID, pProgramID, pSoortBestand: Integer;
                          pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog,
                          pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                          pCallBack: Pointer;
                          pDatum: TDateTime; pSortering, pAantalDagen: integer): Integer; stdcall;
         * 
         */
        public int call_lst_AB_VoerboxBezoeken(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder,string pTaalcode,int pTaalnr, 
                        pCallback ReadDataProc,
                        DateTime pDatum, int pSortering, int pAantalDagen)
        {
            LockObject padlock;
            lst_AB_VoerboxBezoeken handle = (lst_AB_VoerboxBezoeken)ExecuteProcedureDLLStack(typeof(lst_AB_VoerboxBezoeken), "lst_AB_VoerboxBezoeken", GetLockList("RESTLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder,pTaalcode, pTaalnr,
                        ReadDataProc,
                        pDatum, pSortering, pAantalDagen);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_VoerboxBezoeken(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder, string pTaalcode, int pTaalnr,
                        pCallback ReadDataProc,
                        DateTime pDatum, int pSorteringm, int pAantalDagen);

        /*
         
            function lst_AB_Supplementverstrekking(
                      pProgID, pProgramID, pSoortBestand: Integer;
                      pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog,
                      pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
                      pCallBack: Pointer;
                      pDatum: TDateTime; pSortering: integer): Integer; stdcall;
         */
        public int call_lst_AB_Supplementverstrekking(int pProgID, int pProgramID, int pSoortBestand,
                string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                string pResourceFolder, string pTaalcode, int pTaalnr,
                pCallback ReadDataProc,
                DateTime pDatum, int pSortering)
        {
            LockObject padlock;
            lst_AB_Supplementverstrekking handle = (lst_AB_Supplementverstrekking)ExecuteProcedureDLLStack(typeof(lst_AB_Supplementverstrekking), "lst_AB_Supplementverstrekking", GetLockList("RESTLIST.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                        pResourceFolder, pTaalcode, pTaalnr,
                        ReadDataProc,
                        pDatum, pSortering);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Supplementverstrekking(int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                        string pResourceFolder, string pTaalcode, int pTaalnr,
                        pCallback ReadDataProc,
                        DateTime pDatum, int pSortering);

    }
}
