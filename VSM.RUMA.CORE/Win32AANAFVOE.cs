using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32LstAanAfvoe : Win32
    {
        [ThreadStatic]
        private static Win32LstAanAfvoe singleton;

        public static Win32LstAanAfvoe Init()
        {
            if (singleton == null)
            {
                singleton = new Win32LstAanAfvoe();
            }
            return singleton;
        }
        //public Win32LstAanAfvoe()
        //    : base(false)
        //{
        //}

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
        /*
         function lst_AB_AanvoerAfvoerLijst(
            pProgID, pProgramID, pSoortBestand: Integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pCallBack: Pointer;
            pDtBegin, pDtEnd: TDateTime;
            pSoortLijst, pSort, pDierAanwezig, pThrIdAfnemer,
            pSubTotHandelaar, pUniekLevNr: Integer;
            pHoknr: PAnsiChar): Integer; stdcall; export; 
         * 
         * function lst_AB_AanvoerAfvoerLijst(
            pProgID, pProgramID, pSoortBestand: Integer;
            pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
            pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;
            pCallBack: Pointer;
            pDtBegin, pDtEnd: TDateTime;
            pSoortLijst, pSort, pDierAanwezig, pThrIdAfnemer,
            pSubTotHandelaar, pUniekLevNr: Integer;
            pHoknr: PAnsiChar): Integer; stdcall; export;
         */
        public int call_lst_AB_AanvoerAfvoerLijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               string pResourceFolder,string  pTaalcode, int pTaalnr,
                                               pCallback ReadDataProc,
                                               DateTime pDtBegin, DateTime pDtEnd,
                                               int pSoortLijst, int pSort, int pDierAanwezig, int pThrIdAfnemer,
                                               int pSubTotHandelaar, int pUniekLevNr, string pHoknr)
        {


            LockObject padlock;
            lst_AB_AanvoerAfvoerLijst handle = (lst_AB_AanvoerAfvoerLijst)ExecuteProcedureDLLStack(typeof(lst_AB_AanvoerAfvoerLijst), "lst_AB_AanvoerAfvoerLijst", GetLockList("AANAFVOE.DLL"), out padlock);
            int tmp;
            try
            {
                unLogger.WriteError("call_lst_AB_AanvoerAfvoerLijst With:" + padlock.DLLname);

                tmp = handle(pProgID, pProgramID, pSoortBestand,
                                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                pResourceFolder, pTaalcode, pTaalnr,
                                ReadDataProc,
                                pDtBegin, pDtEnd, pSoortLijst, pSort, pDierAanwezig, pThrIdAfnemer, pSubTotHandelaar, pUniekLevNr, pHoknr);

                

            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
            unLogger.WriteError("call_lst_AB_AanvoerAfvoerLijst :return=" + tmp.ToString());
            return tmp;


        }

        delegate int lst_AB_AanvoerAfvoerLijst(int pProgID, int pProgramID, int pSoortBestand,
                                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                               string pResourceFolder, string pTaalcode, int pTaalnr,
                                               pCallback ReadDataProc,
                                               DateTime pDtBegin, DateTime pDtEnd,
                                               int pSoortLijst, int pSort, int pDierAanwezig, int pThrIdAfnemer,
                                               int pSubTotHandelaar, int pUniekLevNr, string pHoknr);                
        
        public delegate void pCallback(int PercDone, string Msg);
    }
}
