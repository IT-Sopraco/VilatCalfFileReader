using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
namespace VSM.RUMA.CORE
{
    public class Win32geitenid : Win32
    {
        [ThreadStatic]
        private static Win32geitenid singleton;

        public static Win32geitenid Init()
        {
            if (singleton == null)
            {
                singleton = new Win32geitenid();
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

        /*
         function lst_AB_GeitenIdlijst(
                 pProgID, pProgramID, pSoortBestand: Integer;
                 pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog: PAnsiChar;
                 pCallBack: Pointer;
                 pSoortlijst, pWeergaveOuders, pWeergaveBeknopt: integer;
                 pDatum: TDateTime;
                 pAlleenAanwezig, pBeknopt, pExtraGegevens,
                 pResponders, pPerHok, pAlleenHok: boolean;
                 pHoknr: PAnsiChar; pSortering: integer): Integer; stdcall;
         */

        public int call_lst_AB_GeitenIdlijst(int pProgID, int pProgramID, int pSoortBestand,
                                              string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                              pCallback ReadDataProc,
                                              int pSoortlijst,int pWeergaveOuders,int pWeergaveBeknopt,
                                              DateTime pDatum,
                                              bool pAlleenAanwezig, bool pBeknopt, bool pExtraGegevens,
                                              bool pResponders, bool pPerHok, bool pAlleenHok,
                                              string pHoknr,int pSortering)
        {

            lock (typeof(Win32stallijst))
            {

                lst_AB_GeitenIdlijst handle = (lst_AB_GeitenIdlijst)ExecuteProcedureDLL(typeof(lst_AB_GeitenIdlijst), "GEITENID.DLL", "lst_AB_GeitenIdlijst");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Facade.GetInstance().getRechten().Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pSoortlijst, pWeergaveOuders, pWeergaveBeknopt,
                                 pDatum,
                                 pAlleenAanwezig, pBeknopt, pExtraGegevens,
                                 pResponders, pPerHok, pAlleenHok,
                                 pHoknr, pSortering);

                FreeDLL("GEITENID.DLL");
                return tmp;
            }
        }

		public delegate void pCallback(int PercDone, string Msg);


        delegate int lst_AB_GeitenIdlijst(int pProgID, int pProgramID, int pSoortBestand,
											  string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
											  pCallback ReadDataProc,
                                              int pSoortlijst, int pWeergaveOuders, int pWeergaveBeknopt,
                                              DateTime pDatum,
                                              bool pAlleenAanwezig, bool pBeknopt, bool pExtraGegevens,
                                              bool pResponders, bool pPerHok, bool pAlleenHok,
                                              string pHoknr, int pSortering);
    }
}
