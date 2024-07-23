using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32ab2xml: Win32
    {
        [ThreadStatic]
        private static Win32ab2xml singleton;

        public static Win32ab2xml Init()
        {
            if (singleton == null)
            {
                singleton = new Win32ab2xml();
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
            pFileName = pFileName.Replace(".dll", "");
            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\" + pFileName + ".dll");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\" + pFileName + "{0}.dll", add));
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
         function AB_aangekochteGeitenProductieXML(
                  pProgID, pProgramID: integer;
                  pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                  pCallback: Pointer;
                  pXMLfile: PAnsiChar;
                  pAanvoerdatBegin, pAanvoerDatEind: TDateTime;
                  pUBNherkomst: PAnsiChar;
                  pInclMelkgegevens: integer): Integer; stdcall;


                    pInclMelkgegevens: 0/1 
         */

        public int call_AB_aangekochteGeitenProductieXML(int pProgID, int pProgramID,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                        pCallback ReadDataProc,
                        string pXMLfile,
                        DateTime pAanvoerdatBegin,DateTime pAanvoerDatEind,
                        string pUBNherkomst,
                        int pInclMelkgegevens)
        {
            LockObject padlock;
            AB_aangekochteGeitenProductieXML handle = (AB_aangekochteGeitenProductieXML)ExecuteProcedureDLLStack(typeof(AB_aangekochteGeitenProductieXML), "AB_aangekochteGeitenProductieXML", GetLockList("ab2xml.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                        pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                        ReadDataProc,
                        pXMLfile,
                        pAanvoerdatBegin, pAanvoerDatEind,
                        pUBNherkomst,
                        pInclMelkgegevens);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB_aangekochteGeitenProductieXML(int pProgID, int pProgramID,
                        string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                        pCallback ReadDataProc,
                        string pXMLfile,
                        DateTime pAanvoerdatBegin,DateTime pAanvoerDatEind,
                        string pUBNherkomst,
                        int pInclMelkgegevens);
    }
}
