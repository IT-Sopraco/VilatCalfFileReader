using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Melkafrekening : Win32
    {
        [ThreadStatic]
        private static Win32Melkafrekening singleton;

        public static Win32Melkafrekening Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Melkafrekening();
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
             function lst_AB_Melkafrekening(pProgID, pProgramID, pSoortBestand: Integer;
                           pUbnnr, pHostName, pUserName, pPassword, pBestand, pLog,
                           pResourceFolder, pTaalcode: PAnsiChar; pTaalnr: integer;                          
                           pCallback: Pointer;
                           pCooperationNumber, pSupplierNumber,
                           pInvoiceNumberYear: integer): Integer; stdcall;

                    pTaalcode: NL, DE, EN, enz
                    pTaalnr: 528, 276 enz
         
         */

        public int call_lst_AB_Melkafrekening(int pProgID, int pProgramID, int pSoortBestand,
               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
               string pResourceFolder, string pTaalcode, int pTaalnr,
               pCallback ReadDataProc,
               int pCooperationNumber, int pSupplierNumber,
               int pInvoiceNumberYear)
        {
            LockObject padlock;
            lst_AB_Melkafrekening handle = (lst_AB_Melkafrekening)ExecuteProcedureDLLStack(typeof(lst_AB_Melkafrekening), "lst_AB_Melkafrekening", GetLockList("MILKINVO.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID, pSoortBestand,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                               pResourceFolder, pTaalcode, pTaalnr,
                               ReadDataProc,
                               pCooperationNumber, pSupplierNumber,
                               pInvoiceNumberYear);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int lst_AB_Melkafrekening(int pProgID, int pProgramID, int pSoortBestand,
                               string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                               string pResourceFolder, string pTaalcode, int pTaalnr,
                               pCallback ReadDataProc,
                               int pCooperationNumber, int pSupplierNumber,
                               int pInvoiceNumberYear);
    }
}
