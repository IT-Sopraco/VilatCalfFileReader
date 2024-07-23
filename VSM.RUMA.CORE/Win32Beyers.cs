using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VSM.RUMA.CORE
{
    public class Win32Beyers:Win32
    {
        [ThreadStatic]
        private static Win32Beyers singleton;

        public static Win32Beyers Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Beyers();
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
            pFileName = pFileName.Replace(".dll", "");
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
            function Beyer_Overzicht(pDriverName, pDatabase: PAnsiChar;
                                     pSoortBestand: Integer;
                                     pBestand, pLogFile: PAnsiChar;
                                     pCallBack: Pointer): integer; stdcall;

                pDriverName is de naam van de ODBC driver voor MS Access. 
         *      Deze naam kun je vinden bij de ODBC instellingen in windows.
                pDatabase is de naam plus volledig pad van de mdb file van Beyer.
         * 
         * 
         * function Beyer_Overzicht(pDriverName, pDatabase: PAnsiChar;
                         pSoortBestand: Integer;
                         pBestand, pLogFile: PAnsiChar;
                         pCallBack: Pointer;
                         pVeehouderID, pMedewerkerID: integer;
                         pBegindat, pEinddat: TDateTime): integer; stdcall;

            pVeehouderID en pMedewerkerID mogen ook de warde 0 hebben. 
         *  In dat geval wordt de lijst dan over alle veehouders resp medewerkers gemaakt.
         * 
         */

        public int call_Beyer_Overzicht(string pDriverName, string pDatabase, int pSoortBestand,
               string pBestand, string pLog,
               pCallback ReadDataProc,
               int pVeehouderID,int pMedewerkerID,
               DateTime  pBegindat,DateTime pEinddat)
        {
            LockObject padlock;
            Beyer_Overzicht handle = (Beyer_Overzicht)ExecuteProcedureDLLStack(typeof(Beyer_Overzicht), "Beyer_Overzicht", GetLockList("Beyer_lijsten.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pDriverName, pDatabase, pSoortBestand,
                pBestand, pLog,
                ReadDataProc,
                pVeehouderID, pMedewerkerID,
                pBegindat, pEinddat);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int Beyer_Overzicht(string pDriverName, string pDatabase, int pSoortBestand,
               string pBestand, string pLog,
               pCallback ReadDataProc,
               int pVeehouderID, int pMedewerkerID,
               DateTime pBegindat, DateTime pEinddat);

        /*
         function Beyer_GetVeehouders(pDriverName, pDatabase: PAnsiChar;
                         pVeehouderBestand, pLogFile: PAnsiChar;
                         pCallBack: Pointer): integer; stdcall;
                         pVeehouderBestand is een csv bestand:
                         ID ; UBN ; naam ; voorvoegsel ; voorleters ; tussenvoegsel ; adres ; postcode ; plaats
         */

        public int call_Beyer_GetVeehouders(string pDriverName, string pDatabase,
                            string pVeehouderBestand, string pLog,
                            pCallback ReadDataProc)
        {
            LockObject padlock;
            Beyer_GetVeehouders handle = (Beyer_GetVeehouders)ExecuteProcedureDLLStack(typeof(Beyer_GetVeehouders), "Beyer_GetVeehouders", GetLockList("Beyer_lijsten.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pDriverName, pDatabase,
                pVeehouderBestand, pLog,
                ReadDataProc);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int Beyer_GetVeehouders(string pDriverName, string pDatabase,
               string pVeehouderBestand, string pLog,
               pCallback ReadDataProc);


        /*
         
          function Beyer_GetMedewerkers(pDriverName, pDatabase: PAnsiChar;
                         pMedewerkerBestand, pLogFile: PAnsiChar;
                         pCallBack: Pointer): integer; stdcall;

                         pMedewerkerBestand is een csv bestand:
                         ID ; inseminatornr ; naam ; voorvoegsel ; voorletters ; tussenvoegsel ; adres ; postcode ; plaats
         
         */

        public int call_Beyer_GetMedewerkers(string pDriverName, string pDatabase,
                     string pMedewerkerBestand, string pLog,
                     pCallback ReadDataProc)
        {
            LockObject padlock;
            Beyer_GetMedewerkers handle = (Beyer_GetMedewerkers)ExecuteProcedureDLLStack(typeof(Beyer_GetMedewerkers), "Beyer_GetMedewerkers", GetLockList("Beyer_lijsten.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pDriverName, pDatabase,
                pMedewerkerBestand, pLog,
                ReadDataProc);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int Beyer_GetMedewerkers(string pDriverName, string pDatabase,
               string pMedewerkerBestand, string pLog,
               pCallback ReadDataProc);
    }
}
