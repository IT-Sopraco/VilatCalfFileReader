using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32EDIZUIVEL_AB : Win32
    {
        public delegate void pCallback(int PercDone, string Msg);


        public Win32EDIZUIVEL_AB()
            : base(false)
        {
        }


        static readonly object padlck = new object();

        private static LockObject[] LockList;

        public static LockObject[] GetLockList()
        {

            lock (padlck)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\edizuivel_ab.dll");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\edizuivel_ab{0}.dll", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        /*
         function AB_ImportEdizuivel(
              pProgID, pProgramID: Integer;
              pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
              pCallBack: Pointer;
              pEdiZuivelBestand: PAnsiChar): Integer; stdcall;

         */
        public int call_AB_ImportEdizuivel(int pProgID, int pProgramID,
              string pUbnnr, string pHostName, string pUserName, string pPassword,string pLogFile,
              pCallback ReadDataProc,
              string pEdiZuivelBestand)
        {
            LockObject padlock;
            AB_ImportEdizuivel handle = (AB_ImportEdizuivel)ExecuteProcedureDLLStack(typeof(AB_ImportEdizuivel), "AB_ImportEdizuivel", GetLockList(), out padlock);
            try
            {
                int tmp = handle(pProgID, pProgramID,
                pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword),pLogFile,
                ReadDataProc,
                pEdiZuivelBestand);

                FreeDLL(padlock.DLLname);
                return tmp;
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        delegate int AB_ImportEdizuivel(int pProgID, int pProgramID,
          string pUbnnr, string pHostName, string pUserName, string pPassword,string pLogFile,
          pCallback ReadDataProc,
          string pEdiZuivelBestand);
    }
}
