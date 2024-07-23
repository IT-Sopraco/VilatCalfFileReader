using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32SOAPZUIVALG : Win32
    {
        public Win32SOAPZUIVALG()
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
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\SOAPZUIVALG.DLL");

                    string result = Path.GetFileName(fileInf.FullName);
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\SOAPZUIVALG{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }
        /*
         *  
            SOAPZUIVALG.DLL
            -------------
          procedure ophalenXMLZuivelGegevens(pFabriek: integer; pUsername, pPassword: PAnsiChar;
                pHistorieVanafDatum: TDateTime; pUBNhistorie: PAnsiChar;
                pOutputFolder: PAnsiChar;
                var csvStatus, csvCode, csvOmschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dophalenXMLZuivelGegevens(int pFabriek, String pUsername, String pPassword,
                             DateTime pHistorieVanafDatum, String pUBNhistorie,
                             String pOutputFolder,
                             ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ophalenXMLZuivelGegevens(int pFabriek, String pUsername, String pPassword,
                             DateTime pHistorieVanafDatum, String pUBNhistorie,
                             String pOutputFolder,
                             ref String csvStatus, ref String csvCode, ref String csvOmschrijving,
                             int pMaxStrLen)
        {

            LockObject padlock;

            dophalenXMLZuivelGegevens handle = (dophalenXMLZuivelGegevens)ExecuteProcedureDLLStack(typeof(dophalenXMLZuivelGegevens), "ophalenXMLZuivelGegevens", GetLockList(), out padlock);
            try
            {

                StringBuilder lcsvStatus = new StringBuilder();
                lcsvStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lcsvCode = new StringBuilder();
                lcsvCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lcsvOmschrijving = new StringBuilder();
                lcsvOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pFabriek, pUsername, pPassword,
                              pHistorieVanafDatum, pUBNhistorie,
                              pOutputFolder,
                              ref  lcsvStatus, ref  lcsvCode, ref  lcsvOmschrijving,
                              pMaxStrLen);

                csvStatus = lcsvStatus.ToString();
                csvCode = lcsvCode.ToString();
                csvOmschrijving = lcsvOmschrijving.ToString();

            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);
            }
        }
    }
}
