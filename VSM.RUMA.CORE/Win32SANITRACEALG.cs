using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32SANITRACEALG : Win32
    {
        public Win32SANITRACEALG()
            : base(false)
        {
        }

        /// <summary>
        /// Copy region padlok per Dll
        /// And change every  call 
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

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTIRaanvoermelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr,int PENVolgnr, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Aanvoerdat, DateTime Aanvoertijd,
                                int Diersoort, int SubDiersoort,
                                int AankomstStatus, int VersienrPaspoort,
                                int VRVidKaart,
                                String BTWnrBestemming, String BTWnrTransporteur,
                                String Kentekenplaat, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTIRafvoermelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Afvoerdat, DateTime Afvoertijd,
                                int SoortAfvoer, int Diersoort, int SubDiersoort,
                                int VersienrPaspoort, int VRVafvoerreden,
                                String BTWnrBestemming, String BTWnrTransporteur,
                                String Kentekenplaat, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                int pMaxStrLen);



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTIRdoodmelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Dooddatum,
                                int Diersoort, int SubDiersoort, int VersienrPaspoort, int VRVafvoerreden,
                                String BTWnrTransporteur, String BTWnrRendac, String InrichtingsnrRendac, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                int pMaxStrLen);




        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTIRexportmelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Exportdatum, int Diersoort,
                                String BTWnrTransporteur, String ExportCertificaat,
                                String Landbestemming, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                int pMaxStrLen);



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTIRimportmelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr, String Naam,
                                String LevensnrMoeder,
                                DateTime Geboortedatum, DateTime Importdatum,
                                int Diersoort, int SubDiersoort, int RasType, int Geslacht, string Haarkleur,
                                int AankomstStatus, int PaspoortUrgentie, int VRVidKaart,
                                String LandcodeHerkomst, String ImportCertificaat, String BTWnrBestemming, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTIRgeboortemelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String BTWnr, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr, String Naam,
                                String LevensnrMoeder,
                                DateTime Geboortedatum,
                                Boolean Doodgeboreen, Boolean MoederRecentAangekocht, Boolean ET, Boolean Zoogkalf,
                                int Diersoort, int RasType, int Geslacht, string Haarkleur, int Bijzonderheden,
                                int GeboorteVerloop, int GeboorteGewicht, int Bevleesdheid,
                                int PaspoortUrgentie, int VRVidKaart,
                                bool Meerling, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSanitraceOpvragenOormerken(String pUsername, String pPassword, int pTestServer,
                                String pBeslagnr, int pDiersoort,
                                String pOormerkBestand,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTverblijfplaatsen(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr, String Beslagnr, String Levensnr,
                                int Diersoort,
                                String pOutputFile, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTstallijst(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String Beslagnr,
                                int Diersoort,
                                String pOutputFile, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen);



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTVersienrPaspoort(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                String pLevensnrFile, String pOutputFile, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTRaadplegenMeldingenAlg(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String Beslagnr,
                                DateTime pMelddatumVan, DateTime pMelddatumTot,
                                int pSoortMelding,
                                String pMeldingenFile, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTRaadplegenVerplaatsingDetails(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                long pMeldingsnr,
                                String pDetailsFile, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTRaadplegenMutatieDetails(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                long pMeldingsnr,
                                ref int pSoortMelding, ref DateTime pDatum,
                                ref StringBuilder pLevensnr, ref StringBuilder pInrichtingsnr, ref StringBuilder pBeslagnr,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen, String pLogFile);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTAanvragenPaspoorten(String pUsername, String pPassword, int pTestServer,
                String Taal,
                int Diersoort, int SubDiersoort,
                String Levensnummer,
                ref StringBuilder Status, ref StringBuilder Omschrijving,
                int pMaxStrLen, String pLogFile);

        /*
         procedure STDierdetails(
               pUsername, pPassword: PAnsiChar; pTestServer: integer;
               Taal: PAnsiChar;
               pLevensnummers: PAnsiChar;
               pDierFile, pLogFile: PAnsiChar;
               var Status, Omschrijving: PAnsiChar;
               pMaxStrLen: integer); stdcall; external 'sanitracealg.dll';


                pLevensnummers: 1 of meer levensnummers (gescheiden door puntkomma)
                pDierFile is een csv bestand met per regel:
                levensnr ; naam ; geb.dat ; geslacht ; haarkleur ; rastype ; versienr paspoort ; levensnr moeder ; inrichtingsnr fokker
                Rastype
	                1 = Melk
	                2 = Vlees
	                3 = Gemend

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dSTDierdetails(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                String pLevensnummers,
                                String pDierfile, String pLogFile,
                                ref StringBuilder Status, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        //================MethodImpl Section==========================

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STIRaanvoermelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr,int PENvolgnr, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Aanvoerdat, DateTime Aanvoertijd,
                                int Diersoort, int SubDiersoort,
                                int AankomstStatus, int VersienrPaspoort,
                                int VRVidKaart,
                                String BTWnrBestemming, String BTWnrTransporteur,
                                String Kentekenplaat, String pLogFile,
                                ref String Status, ref String Omschrijving, ref String Meldingsnr,
                                int pMaxStrLen)
        {

            LockObject padlock;                                                      
            dSTIRaanvoermelding handle = (dSTIRaanvoermelding)ExecuteProcedureDLLStack(typeof(dSTIRaanvoermelding), "STIRaanvoermelding", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    Taal, PENnr, PENvolgnr, Inrichtingsnr,
                                    pBeslagnr, Levensnr,
                                    Aanvoerdat, Aanvoertijd,
                                    Diersoort, SubDiersoort,
                                    AankomstStatus, VersienrPaspoort,
                                    VRVidKaart,
                                    BTWnrBestemming, BTWnrTransporteur,
                                    Kentekenplaat, pLogFile,
                                    ref lStatus, ref lOmschrijving, ref lMeldingsnr, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STIRafvoermelding(String pUsername, String pPassword, int pTestServer,
                        String Taal, String Inrichtingsnr,
                        String pBeslagnr, String Levensnr,
                        DateTime Afvoerdat, DateTime Afvoertijd,
                        int SoortAfvoer, int Diersoort, int SubDiersoort,
                        int VersienrPaspoort, int VRVafvoerreden,
                        String BTWnrBestemming, String BTWnrTransporteur,
                        String Kentekenplaat, String pLogFile,
                        ref String Status, ref String Omschrijving, ref String Meldingsnr,
                        int pMaxStrLen)
        {

            LockObject padlock;
            dSTIRafvoermelding handle = (dSTIRafvoermelding)ExecuteProcedureDLLStack(typeof(dSTIRafvoermelding), "STIRafvoermelding", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, Inrichtingsnr,
                            pBeslagnr, Levensnr,
                            Afvoerdat, Afvoertijd,
                            SoortAfvoer, Diersoort, SubDiersoort,
                            VersienrPaspoort, VRVafvoerreden,
                            BTWnrBestemming, BTWnrTransporteur,
                            Kentekenplaat, pLogFile,
                            ref lStatus, ref lOmschrijving, ref lMeldingsnr, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }




        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STIRdoodmelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Dooddatum,
                                int Diersoort, int SubDiersoort, int VersienrPaspoort, int VRVafvoerreden,
                                String BTWnrTransporteur, String BTWnrRendac, String InrichtingsnrRendac, String pLogFile,
                                ref String Status, ref String Omschrijving, ref String Meldingsnr,
                                int pMaxStrLen)
        {

            LockObject padlock;
            dSTIRdoodmelding handle = (dSTIRdoodmelding)ExecuteProcedureDLLStack(typeof(dSTIRdoodmelding),  "STIRdoodmelding", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, Inrichtingsnr,
                            pBeslagnr, Levensnr,
                            Dooddatum,
                            Diersoort, SubDiersoort, VersienrPaspoort, VRVafvoerreden,
                            BTWnrTransporteur, BTWnrRendac, InrichtingsnrRendac, pLogFile,
                            ref lStatus, ref lOmschrijving, ref lMeldingsnr, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STIRexportmelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr,
                                DateTime Exportdatum, int Diersoort,
                                String BTWnrTransporteur, String ExportCertificaat,
                                String Landbestemming, String pLogFile,
                                ref String Status, ref String Omschrijving, ref String Meldingsnr,
                                int pMaxStrLen)
        {

            LockObject padlock;
            dSTIRexportmelding handle = (dSTIRexportmelding)ExecuteProcedureDLLStack(typeof(dSTIRexportmelding),  "STIRexportmelding", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, Inrichtingsnr,
                            pBeslagnr, Levensnr,
                            Exportdatum, Diersoort,
                            BTWnrTransporteur, ExportCertificaat,
                            Landbestemming, pLogFile,
                            ref lStatus, ref lOmschrijving, ref lMeldingsnr, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }




        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STIRimportmelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr, String Naam,
                                String LevensnrMoeder,
                                DateTime Geboortedatum, DateTime Importdatum,
                                int Diersoort, int SubDiersoort, int RasType, int Geslacht, string Haarkleur,
                                int AankomstStatus, int PaspoortUrgentie, int VRVidKaart,
                                String LandcodeHerkomst, String ImportCertificaat, String BTWnrBestemming, String pLogFile,
                                ref String Status, ref String Omschrijving, ref String Meldingsnr,
                                int pMaxStrLen)
        {

            LockObject padlock;
            dSTIRimportmelding handle = (dSTIRimportmelding)ExecuteProcedureDLLStack(typeof(dSTIRimportmelding),  "STIRimportmelding", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, PENnr, Inrichtingsnr,
                            pBeslagnr, Levensnr, Naam,
                            LevensnrMoeder,
                            Geboortedatum, Importdatum,
                            Diersoort, SubDiersoort, RasType, Geslacht, Haarkleur,
                            AankomstStatus, PaspoortUrgentie, VRVidKaart,
                            LandcodeHerkomst, ImportCertificaat, BTWnrBestemming, pLogFile,
                            ref lStatus, ref lOmschrijving, ref lMeldingsnr, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STIRgeboortemelding(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String BTWnr, String Inrichtingsnr,
                                String pBeslagnr, String Levensnr, String Naam,
                                String LevensnrMoeder,
                                DateTime Geboortedatum,
                                Boolean Doodgeboreen, Boolean MoederRecentAangekocht, Boolean ET, Boolean Zoogkalf,
                                int Diersoort, int RasType, int Geslacht, string Haarkleur, int Bijzonderheden,
                                int GeboorteVerloop, int GeboorteGewicht, int Bevleesdheid,
                                int PaspoortUrgentie, int VRVidKaart,
                                bool Meerling, String pLogFile,
                                ref String Status, ref String Omschrijving, ref String Meldingsnr,
                                int pMaxStrLen)
        {

            LockObject padlock;

            dSTIRgeboortemelding handle = (dSTIRgeboortemelding)ExecuteProcedureDLLStack(typeof(dSTIRgeboortemelding),  "STIRgeboortemelding", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, PENnr, BTWnr, Inrichtingsnr,
                            pBeslagnr, Levensnr, Naam,
                            LevensnrMoeder,
                            Geboortedatum,
                            Doodgeboreen, MoederRecentAangekocht, ET, Zoogkalf,
                            Diersoort, RasType, Geslacht, Haarkleur, Bijzonderheden,
                            GeboorteVerloop, GeboorteGewicht, Bevleesdheid,
                            PaspoortUrgentie, VRVidKaart,
                            Meerling, pLogFile,
                            ref lStatus, ref lOmschrijving, ref lMeldingsnr, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SanitraceOpvragenOormerken(String pUsername, String pPassword, int pTestServer,
                                String pBeslagnr, int pDiersoort,
                                String pOormerkBestand,
                                ref String Status, ref String Omschrijving,
                                int pMaxStrLen)
        {

            LockObject padlock;
            dSanitraceOpvragenOormerken handle = (dSanitraceOpvragenOormerken)ExecuteProcedureDLLStack(typeof(dSanitraceOpvragenOormerken),   "SanitraceOpvragenOormerken", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            pBeslagnr,
                            pDiersoort, pOormerkBestand,
                            ref lStatus, ref lOmschrijving, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }




        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STverblijfplaatsen(String pUsername, String pPassword, int pTestServer,
                                String Taal, String Inrichtingsnr, String Beslagnr, String Levensnr,
                                int Diersoort,
                                String pOutputFile, String pLogFile,
                                ref String Status, ref String Omschrijving,
                                int pMaxStrLen)
        {

            LockObject padlock;
            dSTverblijfplaatsen handle = (dSTverblijfplaatsen)ExecuteProcedureDLLStack(typeof(dSTverblijfplaatsen),   "STverblijfplaatsen", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, Inrichtingsnr, Beslagnr, Levensnr,
                                     Diersoort,
                                     pOutputFile, pLogFile,
                            ref lStatus, ref lOmschrijving, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STstallijst(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String Beslagnr,
                                int Diersoort,
                                String pOutputFile, String pLogFile,
                                ref String Status, ref String Omschrijving,
                                int pMaxStrLen)
        {

            LockObject padlock;
            dSTstallijst handle = (dSTstallijst)ExecuteProcedureDLLStack(typeof(dSTstallijst),   "STstallijst", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, PENnr, Beslagnr,
                            Diersoort,
                            pOutputFile, pLogFile,
                            ref lStatus, ref lOmschrijving, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STVersienrPaspoort(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                String pLevensnrFile, String pOutputFile, String pLogFile,
                                ref String Status, ref String Omschrijving, int pMaxStrLen)
        {

            LockObject padlock;
            dSTVersienrPaspoort handle = (dSTVersienrPaspoort)ExecuteProcedureDLLStack(typeof(dSTVersienrPaspoort),   "STVersienrPaspoort", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal,
                            pLevensnrFile, pOutputFile, pLogFile,
                            ref lStatus, ref lOmschrijving, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STRaadplegenMeldingenAlg(String pUsername, String pPassword, int pTestServer,
                                String Taal, String PENnr, String Beslagnr,
                                DateTime pMelddatumVan, DateTime pMelddatumTot,
                                int pSoortMelding,
                                String pMeldingenFile, String pLogFile,
                                ref String Status, ref String Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            dSTRaadplegenMeldingenAlg handle = (dSTRaadplegenMeldingenAlg)ExecuteProcedureDLLStack(typeof(dSTRaadplegenMeldingenAlg),   "STRaadplegenMeldingenAlg", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, PENnr, Beslagnr,
                            pMelddatumVan, pMelddatumTot,
                            pSoortMelding, pMeldingenFile, pLogFile,
                            ref lStatus, ref lOmschrijving, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            catch (Exception exc) 
            {
                unLogger.WriteError(exc.ToString());
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STRaadplegenVerplaatsingDetails(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                long pMeldingsnr,
                                String pDetailsFile, String pLogFile,
                                ref String Status, ref String Omschrijving,
                                int pMaxStrLen)
        {
            LockObject padlock;
            dSTRaadplegenVerplaatsingDetails handle = (dSTRaadplegenVerplaatsingDetails)ExecuteProcedureDLLStack(typeof(dSTRaadplegenVerplaatsingDetails),   "STRaadplegenVerplaatsingDetails", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal, pMeldingsnr,
                            pDetailsFile, pLogFile,
                            ref lStatus, ref lOmschrijving, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STRaadplegenMutatieDetails(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                long pMeldingsnr,
                                ref int pSoortMelding, ref DateTime pDatum,
                                ref String pLevensnr, ref String pInrichtingsnr, ref String pBeslagnr,
                                ref String Status, ref String Omschrijving,
                                int pMaxStrLen, String pLogFile)
        {
            LockObject padlock;
            dSTRaadplegenMutatieDetails handle = (dSTRaadplegenMutatieDetails)ExecuteProcedureDLLStack(typeof(dSTRaadplegenMutatieDetails),  "STRaadplegenMutatieDetails", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder Levensnr = new StringBuilder();
                Levensnr.EnsureCapacity(pMaxStrLen);
                StringBuilder Inrichtingsnr = new StringBuilder();
                Inrichtingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder Beslagnr = new StringBuilder();
                Beslagnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal,
                            pMeldingsnr,
                            ref pSoortMelding, ref pDatum,
                            ref Levensnr, ref Inrichtingsnr, ref Beslagnr,
                            ref lStatus, ref lOmschrijving, pMaxStrLen, pLogFile);


                pLevensnr = Levensnr.ToString();
                pInrichtingsnr = Inrichtingsnr.ToString();
                pBeslagnr = Beslagnr.ToString();
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            catch (Exception exc) { unLogger.WriteError(exc.ToString());}
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }

        ///////////////////////
     
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STDierdetails(String pUsername, String pPassword, int pTestServer,
                                String Taal,
                                String pLevensnummers,
                               String pDierfile, String pLogFile,
                               ref String Status, ref String Omschrijving,
                               int pMaxStrLen)
        {
            LockObject padlock;
            dSTDierdetails handle = (dSTDierdetails)ExecuteProcedureDLLStack(typeof(dSTDierdetails),   "STDierdetails", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                            Taal,
                            pLevensnummers,
                            pDierfile, pLogFile,
                            ref lStatus, ref lOmschrijving,
                            pMaxStrLen);



                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }

        /*
         procedure STAanvragenPaspoorten(
               pUsername, pPassword: PAnsiChar; pTestServer: integer;
               Taal: PAnsiChar;
               Diersoort, SubDiersoort: integer;
               Levensnummer: PAnsiChar;
               var Status, Omschrijving: PAnsiChar;
               pMaxStrLen: integer; pLogFile: PAnsiChar); stdcall;

         */



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void STAanvragenPaspoorten(String pUsername, String pPassword, int pTestServer,
                        String Taal,
                        int Diersoort, int SubDiersoort,
                        String Levensnummer,
                        ref String Status, ref String Omschrijving,
                        int pMaxStrLen, String pLogFile)
        {

            LockObject padlock;
            dSTAanvragenPaspoorten handle = (dSTAanvragenPaspoorten)ExecuteProcedureDLLStack(typeof(dSTAanvragenPaspoorten), "STAanvragenPaspoorten", GetLockList("SANITRACEALG.DLL"), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                        Taal,
                        Diersoort, SubDiersoort,
                        Levensnummer,
                        ref lStatus, ref lOmschrijving,
                        pMaxStrLen, pLogFile);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            
            }
            finally
            {
                unLogger.WriteInfo("DLL name=" + padlock.DLLname);
                FreeDLL(padlock.DLLname);
                System.Threading.Monitor.Exit(padlock);

            }
        }

    }
}
