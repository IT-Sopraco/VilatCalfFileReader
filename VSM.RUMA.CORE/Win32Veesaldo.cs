using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Veesaldo : Win32
    {
        [ThreadStatic]
        private static Win32Veesaldo singleton;

        public static Win32Veesaldo Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Veesaldo();
            }
            return singleton;
        }
        //public Win32Veesaldo()
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

        public int call_lst_AB_Veesaldo(int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                        pCallback ReadDataProc,                                        
                                        DateTime pBegindatum,DateTime pEinddatum)
        {
            LockObject padlock;
            lst_AB_Veesaldo handle = (lst_AB_Veesaldo)ExecuteProcedureDLLStack(typeof(lst_AB_Veesaldo), "lst_AB_Veesaldo", GetLockList("VEESALDO.DLL"), out padlock);
            int tmp;
            try
            {

              
               
                tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pBegindatum, pEinddatum);

               
                
            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);
                
            }
     
            return tmp;
        }
        
        delegate int lst_AB_Veesaldo(int pProgID, int pProgramID, int pSoortBestand,
                                     string pUbnnr, string pHostName, string pUserName, string pPassword, string pBestand, string pLog,
                                     pCallback ReadDataProc,
                                     DateTime pBegindatum, DateTime pEinddatum);        

        public delegate void pCallback(int PercDone, string Msg);
    }
}