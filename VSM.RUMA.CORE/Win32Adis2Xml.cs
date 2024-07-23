using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSM.RUMA.CORE
{
    public class Win32Adis2Xml : Win32
    {
        [ThreadStatic]
        private static Win32Adis2Xml singleton;

        public static Win32Adis2Xml Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Adis2Xml();
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

        ///////////////////////////////////////////////////////////////////////////
        /*
             function convertLKVNRW_To_RumaXML(pInputFile, pXmlFile, pLogFile:
                    PAnsiChar; pCallBack: Pointer): integer; stdcall;
         */

        public int call_convertLKVNRW_To_RumaXML(string pInputFile, string pXmlFile, string pLogFile, pCallback ReadDataProc)
        {
            LockObject padlock;
            convertLKVNRW_To_RumaXML handle = (convertLKVNRW_To_RumaXML)ExecuteProcedureDLLStack(typeof(convertLKVNRW_To_RumaXML), "convertLKVNRW_To_RumaXML", GetLockList("ADIS2XML.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle( pInputFile,  pXmlFile,  pLogFile,  ReadDataProc);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int convertLKVNRW_To_RumaXML(string pInputFile, string pXmlFile, string pLogFile, pCallback ReadDataProc);
    }
}
