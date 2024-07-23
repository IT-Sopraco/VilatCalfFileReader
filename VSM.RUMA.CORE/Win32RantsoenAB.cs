using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32RantsoenAB:Win32
    {


        [ThreadStatic]
        private static Win32RantsoenAB singleton;

        public static Win32RantsoenAB Init()
        {
            if (singleton == null)
            {
                singleton = new Win32RantsoenAB();
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
            pFileName = pFileName.Replace(".DLL", "").Replace(".dll", "");
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
           Hierbij een blackbox voor de rantsoenberekening. Dat hoef jij dus niet te doen en kun je werken aan de schermen.

            function AB_RantsoenBerekening(pProgID, pProgramID: integer;
                                      pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                                      pCallback: Pointer;
                                      pResultaatFile: PAnsiChar): Integer; stdcall;

            pResultaatFile is een csv bestand met
            aniid ; levensnummer ; grpepnr ; diersoort ; voer1 ; voer2; voer3; enz.

            groepnr = GROUPS -> groupid
            diersoort: 1=koe, 2=vaars, 3=droog, 4=jongvee, 5=2e kalfs koe 
         * 
            Je moet de resultaten tonen in een scherm en als ze het accepteren moet je het opslaan in FEED_ADVICE en FEED_ADVICE_DETAIL (target veld).
            De afwerking van de maximale stijging/daling moet nog wel uitgevoerd worden voordat het naar de voercomputer wordt gestuurd.
            Er komen nu nog random voergiften uit. Maar als het ingebouwd is en ik krijg het voor maandag af dan hoeft alleen deze dll geupdate te worden.

         */

        public int call_AB_RantsoenBerekeningt(int pProgID, int pProgramID,
                                      string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                                      pCallback ReadDataProc,
                                      string pResultaatFile)
        {
            LockObject padlock;
            AB_RantsoenBerekening handle = (AB_RantsoenBerekening)ExecuteProcedureDLLStack(typeof(AB_RantsoenBerekening), "AB_RantsoenBerekening", GetLockList("Rantsoen_AB.dll"), out padlock);
            int tmp;
            try
            {



                tmp = handle(pProgID, pProgramID,
                               pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pLog,
                               ReadDataProc,
                               pResultaatFile);



            }
            finally
            {
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }

            return tmp;
        }

        delegate int AB_RantsoenBerekening(int pProgID, int pProgramID,
                                      string pUbnnr, string pHostName, string pUserName, string pPassword, string pLog,
                                      pCallback ReadDataProc,
                                      string pResultaatFile);

    }
}
