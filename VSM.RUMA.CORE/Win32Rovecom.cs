using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32Rovecom: Win32
    {
        [ThreadStatic]
        private static Win32Hok singleton;

        public static Win32Hok Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Hok();
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
         sendDataToRovecom

                function sendDataToRovecom(
                pXMLfile, pLogFile: PAnsiChar;
                pUserName, pPassword: PAnsiChar;
                pDateFrom: TDateTime): integer; stdcall;


                Resultaat: 0=goed/-1=fout

                pXMLfile is de normale agrobase/ruma xml. 
                pDateFrom is vanaf welke datum de melkgegevens verstuurd moeten worden (afhankelijk van wanneer het de vorige/laatste keer is verstuurd). 

                De xml moet minimaal het volgende bevatten: 
                - koegegevens (animal): aanwezige dieren met tenminste 1 lactatie 
                - kalender gegevens (events) van tenminste de laatste lactatie 
                - MPR melkgegevens (analyse) van crdelta van tenminste de laatste lactatie 
                - emm gegevens van tenminste vanaf pDateFrom

         */

        public int std_sendDataToRovecom(string pXMLfile, string pLogFile, string pUserName, string pPassword, DateTime pDateFrom)
        {
            LockObject padlock;
            sendDataToRovecom handle = (sendDataToRovecom)ExecuteProcedureDLLStack(typeof(sendDataToRovecom), "sendDataToRovecom", GetLockList("ROVECOMALG.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pXMLfile, pLogFile, pUserName, pPassword, pDateFrom);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int sendDataToRovecom(string pXMLfile, string pLogFile, string pUserName, string pPassword, DateTime pDateFrom);

        /*
            getDataFromRovecom

                function getDataFromRovecom(
                pXMLfile, pLogFile: PAnsiChar;
                pUserName, pPassword: PAnsiChar;
                var pLastDateTime: TDateTime): integer; stdcall;

                Resultaat: 0=goed/-1=fout

                pLastDateTime: de datum/tijd  van de laatste voerberekening door Rovecom

         */

        public int std_getDataFromRovecom(string pXMLfile, string pLogFile, string pUserName, string pPassword,out DateTime pLastDateTime)
        {
            LockObject padlock;
            getDataFromRovecom handle = (getDataFromRovecom)ExecuteProcedureDLLStack(typeof(getDataFromRovecom), "getDataFromRovecom", GetLockList("ROVECOMALG.DLL"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pXMLfile, pLogFile, pUserName, pPassword,out pLastDateTime);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int getDataFromRovecom(string pXMLfile, string pLogFile, string pUserName, string pPassword,out DateTime pLastDateTime);
    }
}
