using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    public class Win32SoapLNValg : Win32
    {
        public Win32SoapLNValg()
            : base(false)
        {
        }

        //static readonly LockObject[] COLLpadlocks = new LockObject[]
        //    { 
        //        new LockObject("SOAPLNVALG1.DLL"),
        //        new LockObject("SOAPLNVALG2.DLL"),
        //        new LockObject("SOAPLNVALG3.DLL"),
        //        new LockObject("SOAPLNVALG4.DLL")  
        //    };



        static readonly object padlock = new object();


        private static LockObject[] LockList;

        public static LockObject[] GetLockList()
        {           

            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\SOAPLNVALG.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\SOAPLNVALG{0}.DLL", add));
                        add++;
                    }
                    LockList =  locklist.ToArray();
                }
                return LockList;
            }            
        }

        //static readonly object padlock = new object();

        //Terugmelding van de procedures:

        //Status : G (goed) / F (fout) / W (waarschuwing)
        //Code : een foutcode
        //Omschrijving : uitgebreiden omschrijving voor de reden van F of W

        //Inloggegevens voor de RVO testserver Versie 1
        //Gebruikersnaam:	7890119
        //Wachtwoord:		78x011x
        //UBN:			2510
        //BRS nummer:	200684966

        //Inloggegevens voor de RVO testserver Versie 2
        //Schapen/Geiten:
        //Gebruikersnaam:	8904467
        //Wachtwoord:		8x0HHn7
        //UBN:			152460
        //BRS nummer:	10005948


        //Diersoort voor RVO Versie 2:
        //1 = Rundvee
        //3 = Schapen
        //4 = Geiten

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVDierstatus(String pUsername, String pPassword, Boolean pTestServer,
                                out string Levensnr,
                                out string BRSnrHouder, out string UBNhouder, out string Werknummer,
                                out DateTime Geboortedat, out DateTime Importdat,
                                out string LandCodeHerkomst, out string LandCodeOorsprong,
                                out string Geslacht, out string Haarkleur,
                                out DateTime Einddatum, out string RedenEinde,
                                out string LevensnrMoeder,
                                out string Status, out string Code, out string Omschrijving,
                                int pMaxStrLen);
   
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVDierstatus(String pUsername, String pPassword, Boolean pTestServer,
                                out string Levensnr,
                                out string BRSnrHouder, out string UBNhouder, out string Werknummer,
                                out DateTime Geboortedat, out DateTime Importdat,
                                out string LandCodeHerkomst, out string LandCodeOorsprong,
                                out string Geslacht, out string Haarkleur,
                                out DateTime Einddatum, out string RedenEinde,
                                out string LevensnrMoeder,
                                out string Status, out string Code, out string Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            LNVDierstatus handle = (LNVDierstatus)ExecuteProcedureDLLStack(typeof(LNVDierstatus), "LNVDierstatus", GetLockList(), out padlock);
            try
            {
                handle(pUsername, pPassword, pTestServer, out Levensnr,
                                    out BRSnrHouder, out  UBNhouder, out  Werknummer,
                                     out Geboortedat, out  Importdat,
                                     out LandCodeHerkomst, out  LandCodeOorsprong,
                                     out Geslacht, out Haarkleur,
                                     out Einddatum, out  RedenEinde,
                                     out LevensnrMoeder,
                                     out Status, out Code, out Omschrijving,
                                     pMaxStrLen);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*procedure LNVOpvragenOormerkenV2(pUsername, pPassword: PAnsiChar; 
                    pTestServer: integer;
                    pBRSnr: pAnsiChar; pPrognr: integer;
                    pOormerkBestand, pLogBestand: PAnsiChar;
                    pCallback: Pointer;
                    var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;
                    */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVOpvragenOormerkenV2(string pUsername, string pPassword, int pTestServer,
                                string pBRSnr, int pDiersoort,
                                string pOormerkBestand, string pLogBestand,
                                TPercentageDoneCallback ReadDataProc,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVOpvragenOormerkenV2(String pUsername, String pPassword, int pTestServer,
                                String pBRSnr, int pDiersoort,
                                string pOormerkBestand, string pLogBestand,
                                TPercentageDoneCallback ReadDataProc,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            LNVOpvragenOormerkenV2 handle = (LNVOpvragenOormerkenV2)ExecuteProcedureDLLStack(typeof(LNVOpvragenOormerkenV2), "LNVOpvragenOormerkenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer, pBRSnr,
                                         pDiersoort, pOormerkBestand, pLogBestand,
                                         ReadDataProc, ref  lStatus, ref  lCode, ref  lOmschrijving,
                                         pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
         procedure LNVOpvragenBesteldeOormerkenV2(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                pBRSnr: pAnsiChar; pPrognr: integer;
                pOormerkBestand, pLogBestand: PAnsiChar;
                pCallback: Pointer;
                var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;


                Voor oormerkleveranciers.
                Als Status = G dan zal pOormerkBestand de bestelde oormerken bevatten.

                Per regel:
                levensnr ; codeFabrikant ; codeLeverancier ; naamLeverancier ; merktype ; omschrijvingMerktype ; codeSoortMerk ; codeVormMerk ; BestelDatum ; Werknummer ; merkVersieNr

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVOpvragenBesteldeOormerkenV2(string pUsername, string pPassword, int pTestServer,
                                string pBRSnr, int pPrognr,
                                string pOormerkBestand, string pLogBestand,
                                TPercentageDoneCallback ReadDataProc,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVOpvragenBesteldeOormerkenV2(String pUsername, String pPassword, int pTestServer,
                                String pBRSnr, int pPrognr,
                                string pOormerkBestand, string pLogBestand,
                                TPercentageDoneCallback ReadDataProc,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            LNVOpvragenBesteldeOormerkenV2 handle = (LNVOpvragenBesteldeOormerkenV2)ExecuteProcedureDLLStack(typeof(LNVOpvragenBesteldeOormerkenV2), "LNVOpvragenBesteldeOormerkenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer, pBRSnr,
                                         pPrognr, pOormerkBestand, pLogBestand,
                                         ReadDataProc, ref  lStatus, ref  lCode, ref  lOmschrijving,
                                         pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }



        /*procedure LNVopvragenOntvangstAdressen(
            pUsername,
            pPassword: PAnsiChar;
            pTestServer: integer;
            pBrsNrHouder: PAnsiChar;
            pLogBestand: PAnsiChar;
            xmlAdressenFile : PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;*/


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVopvragenOntvangstAdressen(String pUsername, String pPassword, int pTestServer,
                                String pBrsNrHouder, String pLogBestand, String xmlAdressenFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVopvragenOntvangstAdressen(String pUsername, String pPassword, int pTestServer,
                                String pBrsNrHouder, String pLogBestand, String xmlAdressenFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {

            LockObject padlock;
            LNVopvragenOntvangstAdressen handle = (LNVopvragenOntvangstAdressen)ExecuteProcedureDLLStack(typeof(LNVopvragenOntvangstAdressen), "LNVopvragenOntvangstAdressen", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                        pBrsNrHouder, pLogBestand, xmlAdressenFile,
                                        ref  Status, ref  Code, ref  Omschrijving,
                                        pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

        }


        //TPercentageDoneCallback = procedure(PercDone: integer; Msg: PAnsiChar); stdcall;

        public delegate void TPercentageDoneCallback(int PercDone, string Msg);

        //Als Status = G dan zal pOormerkBestand de vrije oormerken (1 per regel) bevatten. 
        //Dit zijn de vrije oormerken volgens RVO op dat moment. 
        //Ze kunnen in het management pakket al gebruikt zijn maar nog niet I&R gemeld.


        /*
         procedure LNVopvragenHouderWaarvoorGemachtigd(
                pUsername, pPassword: PAnsiChar;
                pTestServer: integer;
                pBrsNrHouder: PAnsiChar;
                pLogBestand: PAnsiChar;
                xmlHouderFile : PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVopvragenHouderWaarvoorGemachtigd(string pUsername, string pPassword, 
                                int pTestServer,
                                string pBrsNrHouder,
                                string pLogBestand,
                                string xmlHouderFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVopvragenHouderWaarvoorGemachtigd(String pUsername, String pPassword, int pTestServer,
                                string pBrsNrHouder,
                                string pLogBestand,
                                string xmlHouderFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            LNVopvragenHouderWaarvoorGemachtigd handle = (LNVopvragenHouderWaarvoorGemachtigd)ExecuteProcedureDLLStack(typeof(LNVopvragenHouderWaarvoorGemachtigd), "LNVopvragenHouderWaarvoorGemachtigd", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer, 
                                    pBrsNrHouder,
                                    pLogBestand,
                                    xmlHouderFile, 
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                         pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }



        /*
         procedure LNVaanvraganPaspoortV2(
                pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                pBRSnr, pLevensnummer, pEmailAdres: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pLogBestand: PAnsiChar; pMaxStrLen: integer); stdcall;
     
         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVaanvraganPaspoortV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnr,
                                string pLevensnummer,
                                string pEmailAdres,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogBestand, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVaanvraganPaspoortV2(String pUsername, String pPassword, 
                                int pTestServer,
                                string pBRSnr,
                                string pLevensnummer,
                                string pEmailAdres,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogBestand, int pMaxStrLen)
        {
            LockObject padlock;
            LNVaanvraganPaspoortV2 handle = (LNVaanvraganPaspoortV2)ExecuteProcedureDLLStack(typeof(LNVaanvraganPaspoortV2), "LNVaanvraganPaspoortV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnr,
                                    pLevensnummer,
                                    pEmailAdres,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogBestand, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        /*
         procedure LNVOpvragenMEUBNWaarvoorGemachtigdV2(
                pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                MEUBNfile: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pLogBestand: PAnsiChar; pMaxStrLen: integer); stdcall;

                MEUBNfile is een csv bestand:

                ubnnr; brsnr; soortbedrijf; naam;adres; postcode; woonplaats;diersoort1;diersoort2; t/m diersoort20

         */


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVOpvragenMEUBNWaarvoorGemachtigdV2(string pUsername, string pPassword,
                                int pTestServer,
                                string MEUBNfile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogBestand, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVOpvragenMEUBNWaarvoorGemachtigdV2(String pUsername, String pPassword,
                                int pTestServer,
                                string MEUBNfile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogBestand, int pMaxStrLen)
        {
            LockObject padlock;
            LNVOpvragenMEUBNWaarvoorGemachtigdV2 handle = (LNVOpvragenMEUBNWaarvoorGemachtigdV2)ExecuteProcedureDLLStack(typeof(LNVOpvragenMEUBNWaarvoorGemachtigdV2), "LNVOpvragenMEUBNWaarvoorGemachtigdV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    MEUBNfile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogBestand, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }



        /*
         procedure LNVBestellenOormerkenV2(
                pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                pPrognr: integer; pBRSnummer, pLevensnr,
                pOormerkCode, pAdresType: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pLogBestand: PAnsiChar; pMaxStrLen: integer); stdcall;

                Herbestellen van verloren oormerken

                Beschikbare oormerkcodes kunnen opgehaald worden met LNVRaadplegenMerktypenV2.

                pAdresType
                0 = Woonadres
                1 = Postadres

         */


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVBestellenOormerkenV2(string pUsername, string pPassword,
                                int pTestServer,
                                int pPrognr, string pBRSnummer, string pLevensnr,
                                string pOormerkCode,string pAdresType,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogBestand, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVBestellenOormerkenV2(String pUsername, String pPassword,
                                int pTestServer,
                                int pPrognr, string pBRSnummer, string pLevensnr,
                                string pOormerkCode, string pAdresType,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogBestand, int pMaxStrLen)
        {
            LockObject padlock;
            LNVBestellenOormerkenV2 handle = (LNVBestellenOormerkenV2)ExecuteProcedureDLLStack(typeof(LNVBestellenOormerkenV2), "LNVBestellenOormerkenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pPrognr,  pBRSnummer,  pLevensnr,
                                    pOormerkCode, pAdresType,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogBestand, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure LNVRaadplegenMerktypenV2(
                pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                pPrognr, pSoort: integer; pOutputFile, pLogfile: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pMaxStrLen: integer); stdcall;

                * Opvragen beschikbarre merktypen voor herbestelling of nieuwe bestelling

                pSoort
                1 = Herbestellen (verloren oormerk)
                2 = Nieuwe bestelling (vrije oormerken)

                pOutputfile is een csv bestand:

                Merkcode ; Fabrikant ; Leverancier ; NaamLeverancier ;Soort ; Vorm ; Omschrijving ; AantalLos ; AantalPerDoos

                Fabrikant: 
                A = Allflex
                C =  Chevillot
                D =  Daploma
                Y =  y-tex
                Q = Q-Flex
                P = Schippers
                M = Metagam
                G = Os
                X = Reyflex France
                N = Dalton
                E = Dalton (elektronisch)
                S = Caisley
                O = Onbekend

                Soort
                B = Bolusmerk
                E = Elektronisch oormerk
                G = Groepsmerk
                I = Injectaat
                O = Oormerk
                P = Pootband
                S = Slachtmerk
                T = Tijdelijk
                TA = Tatoeage

                Vorm : Zie Labels -> LabKind = 107  (9107  voor AGRO_LABELS)

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenMerktypenV2(string pUsername, string pPassword,
                                int pTestServer,
                                int  pPrognr, int pSoort, string pOutputFile, string pLogfile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenMerktypenV2(String pUsername, String pPassword,
                                int pTestServer,
                                int pPrognr, int pSoort, string pOutputFile, string pLogfile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenMerktypenV2 handle = (LNVRaadplegenMerktypenV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenMerktypenV2), "LNVRaadplegenMerktypenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername,  pPassword, pTestServer,
                                    pPrognr,  pSoort, pOutputFile, pLogfile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        /*
         procedure LNVRaadplegenInformatieProductenV2(
                pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                pBRSnummer: PAnsiChar;
                pOutPutFile: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

                Opvragen van voor de gebruiker beschikbare producten

                pOutputFile is een csv bestand

                codeProduct ; productOmschrijving ; codeMedium ; codeWijzeVerzending ; NIetVoorDiersoort ; NietVoorDiersoort ; NietVoorDiersoort ; NietvoorDiersoort

                NietVoorDiersoort:
                1 = Rundvee
                2 = Varkens
                3 = Schapen
                4 = Geiten

         */
   
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenInformatieProductenV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer,
                                string pOutPutFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenInformatieProductenV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer,
                                string pOutPutFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenInformatieProductenV2 handle = (LNVRaadplegenInformatieProductenV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenInformatieProductenV2), "LNVRaadplegenInformatieProductenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer,
                                    pOutPutFile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            
             procedure LNVRaadplegenInfoProdParametersV2(
                pUsername, pPassword: String; pTestServer: Integer;
                pBRSnummer, pCodeProduct: String;
                pOutPutFile: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

                Opvragen van de verplichte en optionele parameters voor het aanvragen van een bepaald informatieproduct

                pCodeProduct : wat bijv terugkomt van LNVRaadplegenInfomatieProductenV2

                pOutPutFile is een csv bestand:

                parameterCode ; Omschrijving ; TypeGegeven ; Verplicht ; Muteerbaar ; DefaultWaarde ; Volgordenummer

                TypeGegeven
                N = Numeriek
                C = Tekst

                Verplicht / Muteerbaar : J/N

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenInfoProdParametersV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer, string pCodeProduct,
                                string pOutPutFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenInfoProdParametersV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer, string pCodeProduct,
                                string pOutPutFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenInfoProdParametersV2 handle = (LNVRaadplegenInfoProdParametersV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenInfoProdParametersV2), "LNVRaadplegenInfoProdParametersV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer, pCodeProduct,
                                    pOutPutFile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
          procedure LNVVastleggenInformatieProductV2(
                pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                pBRSnummer, pCodeProduct, pAdresType, pFrequentie,
                pOrgAanvraagnr, pParameters: PAnsiChar;
                pDatumIngang, pDatumEinde: TDateTime;
                var pAanvraagNummer, pOrderVolgnr: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

         * Zie verder SOAPLNVALG-1.DOC bij sys2 agrobase documentatie algemeen
         * 
         */


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVVastleggenInformatieProductV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer, string pCodeProduct,
                                string  pAdresType,string  pFrequentie,
                                string  pOrgAanvraagnr, string  pParameters,
                                DateTime pDatumIngang,DateTime pDatumEinde,
                                ref StringBuilder pAanvraagNummer, ref StringBuilder pOrderVolgnr,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVVastleggenInformatieProductV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer, string pCodeProduct,
                                string pAdresType, string pFrequentie,
                                string pOrgAanvraagnr, string pParameters,
                                DateTime pDatumIngang, DateTime pDatumEinde,
                                ref String pAanvraagNummer, ref String pOrderVolgnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVVastleggenInformatieProductV2 handle = (LNVVastleggenInformatieProductV2)ExecuteProcedureDLLStack(typeof(LNVVastleggenInformatieProductV2), "LNVVastleggenInformatieProductV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lpAanvraagNummer = new StringBuilder();
                lpAanvraagNummer.EnsureCapacity(pMaxStrLen);
                StringBuilder lpOrderVolgnr = new StringBuilder();
                lpOrderVolgnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer, pCodeProduct,
                                    pAdresType,  pFrequentie,
                                    pOrgAanvraagnr,  pParameters,
                                    pDatumIngang,  pDatumEinde,
                                    ref lpAanvraagNummer, ref lpOrderVolgnr,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                pAanvraagNummer = lpAanvraagNummer.ToString();
                pOrderVolgnr = lpOrderVolgnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
                 procedure LNVRaadplegenOrdersV2(
                    pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                    pBRSnummer, pAanvraagNummer: PAnsiChar;
                    pOrderFile: PAnsiChar;
                    var Status, Code, Omschrijving: PAnsiChar;
                    pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

                    pAanvraagNummer : zoals dat terugkomt uit LNVVastleggenInformatieProductV2

                    pOrderFile is een csv bestand:

                    orderVolgnr ; AanvraagDatum ; LeveringsDatum ; Prijs ; StatusCode ; StatusToelichting

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenOrdersV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagNummer,
                                string pOrderFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenOrdersV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagNummer,
                                string pOrderFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenOrdersV2 handle = (LNVRaadplegenOrdersV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenOrdersV2), "LNVRaadplegenOrdersV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer, pAanvraagNummer,
                                    pOrderFile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        /*
            procedure LNVRaadplegenOrderParametersV2(
                    pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                    pBRSnummer, pAanvraagnr, pOrderVolgnr: PAnsiChar;
                    pParameterFile: PAnsiChar;
                    var Status, Code, Omschrijving: PAnsiChar;
                    pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

            Geeft meer details van een bepaalde order terug.

            pAanvraagnr, pOrderVolgnr : zoals die terugkomen uit LNVVastleggenInformatieProductV2 of LNVRaadplegenOrdersV2

            pParameterFile is een csv bestand:

            ParameterCode ; ParameterOmschrijving ; ParameterWaarde

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenOrderParametersV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagnr, string pOrderVolgnr,
                                string pParameterFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenOrderParametersV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagnr, string pOrderVolgnr,
                                string pParameterFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenOrderParametersV2 handle = (LNVRaadplegenOrderParametersV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenOrderParametersV2), "LNVRaadplegenOrderParametersV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer, pAanvraagnr, pOrderVolgnr,
                                    pParameterFile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
                     procedure LNVRaadplegenAanvraagInformatieProductenV2(
                            pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                            pBRSnummer, pAanvraagNummer, pProductCode: PAnsiChar;
                            pDatumBeginAanvraag, pDatumEindeAanvraag: TDateTime;
                            pInfoProdFile: PAnsiChar;
                            var Status, Code, Omschrijving: PAnsiChar;
                            pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

                    Opvragen van aangevraagde informatie producten.

                    pAanvraagNummer, pProductCode, pDatumBeginAanvraag, pDatumEindeAanvraag zijn optioneel

                    pInfoProdFile is een csv bestand met de aangevraagde producten:

                    Aanvraagnr ; ProductCode ; DatumAanvraag ; DatumIngang ; DatumEinde ; Omschrijving ; Medium ; WijzeVerzending ; Frequentie ; Status

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenAanvraagInformatieProductenV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagNummer, string pProductCode,
                                DateTime pDatumBeginAanvraag, DateTime pDatumEindeAanvraag,
                                string pInfoProdFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenAanvraagInformatieProductenV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagNummer, string pProductCode,
                                DateTime pDatumBeginAanvraag, DateTime pDatumEindeAanvraag,
                                string pInfoProdFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenAanvraagInformatieProductenV2 handle = (LNVRaadplegenAanvraagInformatieProductenV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenAanvraagInformatieProductenV2), "LNVRaadplegenAanvraagInformatieProductenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer,  pAanvraagNummer,  pProductCode,
                                    pDatumBeginAanvraag, pDatumEindeAanvraag,
                                    pInfoProdFile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
                procedure LNVRaadplegenAanvraagInfoProdParametersV2(
                        pUsername, pPassword: PAnsiChar; pTestServer: Integer;
                        pBRSnummer, pAanvraagnr: PAnsiChar;
                        pParameterFile: PAnsiChar;
                        var Status, Code, Omschrijving: PAnsiChar;
                        pLogFile: PAnsiChar; pMaxStrLen: integer); stdcall;

                Meer details van een aangevraagd informatie product.

                pParameterFile is een csv bestand

                ParameterCode ; ParameterOmschrijving ; ParameterWaarde

         */


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void LNVRaadplegenAanvraagInfoProdParametersV2(string pUsername, string pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagnr,
                                string pParameterFile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                string pLogFile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void call_LNVRaadplegenAanvraagInfoProdParametersV2(String pUsername, String pPassword,
                                int pTestServer,
                                string pBRSnummer, string pAanvraagnr,
                                string pParameterFile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                string pLogFile, int pMaxStrLen)
        {
            LockObject padlock;
            LNVRaadplegenAanvraagInfoProdParametersV2 handle = (LNVRaadplegenAanvraagInfoProdParametersV2)ExecuteProcedureDLLStack(typeof(LNVRaadplegenAanvraagInfoProdParametersV2), "LNVRaadplegenAanvraagInfoProdParametersV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    pBRSnummer, pAanvraagnr,
                                    pParameterFile,
                                    ref  lStatus, ref  lCode, ref  lOmschrijving,
                                    pLogFile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

    }

}
