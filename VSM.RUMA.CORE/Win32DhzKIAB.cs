using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32DhzKIAB : Win32
    {
        public Win32DhzKIAB()
            : base(false)
        {
        }

        static readonly object padlock = new object();


        private static LockObject[] LockList;

        public static LockObject[] GetLockList()
        {

            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\DHZKI_AB.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\DHZKI_AB{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }
 

        public delegate void pCallback(int PercDone, string Msg);

        /*
         function AB_maakKIBbestand(
              pProgID, pProgramID: integer;
              pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
              pCallback: Pointer;
              pNrKIorganisatie, pKIBfile: PAnsiChar): Integer; stdcall;
         
         */

 

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int dAB_maakKIBbestand(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc,
                                String pNrKIorganisatie, String pKIBfile);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AB_maakKIBbestand(int pProgID, int pProgramID,
                                String UBNnr, String pHostName, String pUsername, String pPassword, String pLog,
                                pCallback ReadDataProc,
                                String pNrKIorganisatie, String pKIBfile)
        {

            LockObject padlock;


            dAB_maakKIBbestand handle = (dAB_maakKIBbestand)ExecuteProcedureDLLStack(typeof(dAB_maakKIBbestand), "AB_maakKIBbestand", GetLockList(), out padlock);
            int tmp = 0;
            try
            {

                tmp = handle(pProgID, pProgramID,
                                UBNnr, pHostName, pUsername, Decodeer_PW(pPassword), pLog,
                                ReadDataProc,
                                pNrKIorganisatie, pKIBfile);

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            return tmp;
        }
    }
}
