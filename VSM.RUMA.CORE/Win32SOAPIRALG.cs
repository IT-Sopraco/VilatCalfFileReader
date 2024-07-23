using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{

    public class Win32SOAPIRALG : Win32
    {

        public Win32SOAPIRALG()
            : base(false)
        {
        }

        //static readonly LockObject[] COLLpadlocks = new LockObject[]
        //    { 
        //        new LockObject("SOAPIRALG1.DLL"),
        //        new LockObject("SOAPIRALG2.DLL"),
        //        new LockObject("SOAPIRALG3.DLL"),
        //        new LockObject("SOAPIRALG4.DLL")  
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
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\SOAPIRALG.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\SOAPIRALG{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDIRaanvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime Aanvoerdat,
                                int SoortAanvoer, int pPaspoortnr,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder UBNherkomst,
                                ref StringBuilder pMeldingsnr, String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDIRafvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime Afvoerdat,
                                int SoortAfvoer, int Afvoerreden, int pPaspoortnr, String pLandBestemming, String UBNbestemmingIn,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder UBNbestemmingUit,
                                ref StringBuilder pMeldingsnr, String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDIRdoodgeborenmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrMoeder, DateTime Geboortedat,
                                String Geslacht, int Geboorteverloop, int Bevleesheid,
                                int Gewicht, int Overlevingsstatus,int pMeerling, int pMoederRecentAangekocht,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                ref StringBuilder pMeldingsnr, String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDIRgeboortemelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, String Naam, String Werknummer,
                                String LevensnrMoeder, DateTime Geboortedat, DateTime pSterftedatum,
                                String Geslacht, String Haarkleur, int Bijzonderheden, int Geboorteverloop,
                                int Bevleesheid, int Gewicht, int Overlevingsstatus,int CodeNaamgeving,
                                int pRasType, int pPaspoortUrgentie, int pMoederRecentAangekocht,
                                bool Opfok, bool Registratiekaart,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                ref StringBuilder pMeldingsnr, String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDIRimportmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, String Naam,
                                String Werknummer, String LevensnrMoeder, String LandVertrek,
                                String LandGeboorte, String OrgineelLevensnr,
                                DateTime Geboortedat, DateTime Importdat,
                                String Geslacht, String Haarkleur, bool Subsidie,
                                String nrGezondheidsCert,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                String pLogfile, int pMaxStrLen);



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRaanvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                DateTime Aanvoerdat,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRafvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                DateTime Afvoerdat,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRdoodgeborenmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String LevensnrMoeder,
                                DateTime Geboortedat,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRgeboortemelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String LevensnrMoeder,
                                DateTime Geboortedat,
                                String Geslacht, String Haarkleur,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRimportmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String Naam, String Werknummer, String LevensnrMoeder,
                                String LandVertrek, String LandGeboorte, String OrgineelLevensnr,
                                DateTime Geboortedat, DateTime Importdat,
                                String Geslacht, String Haarkleur, bool Subsidie,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRexportmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                DateTime Exportdat,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);




        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRaanvoermeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String UBNHerkomst, int Diersoort,
                                DateTime Aanvoerdat,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRafvoermeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String UBNBestemming, int Diersoort,
                                DateTime Afvoerdat,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRdoodmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, int Diersoort,
                                DateTime Dooddatum,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRdoodgeborenmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String LevensnrMoeder, int Diersoort,
                                DateTime Geboortedat,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRgeboortemeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String LevensnrMoeder, int Diersoort,
                                DateTime Geboortedat,
                                String Geslacht, String Haarkleur,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRimportmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String Naam, String Werknummer, String LevensnrMoeder,
                                String LandVertrek, String LandGeboorte, String OrgineelLevensnr, int Diersoort,
                                DateTime Geboortedat, DateTime Importdat,
                                String Geslacht, String Haarkleur, bool Subsidie,
                                String nrGezondheidsCert,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRexportmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                int Diersoort, DateTime Exportdat,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRnoodslachtmeldingV2(String pUsername, String pPassword,int pTestServer,
                                String UBNnr, String BRSnr, 
                                String Levensnr,int Diersoort, DateTime Noodslachtdat,
                                String UBNnoodslacht,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);



        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVQkoortsVaccinatieMelding(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                int Diersoort,
                                DateTime VaccinatieDatum, int VaccinatieNr,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRomnummermelding(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr_oud, String Levensnr_nieuw,
                                int Diersoort,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                                String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIntrekkenMelding(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr, String MeldingsNr,
                             int pDierSoort,
                             ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                             String pLogfile, int pMaxStrLen);

        /*procedure LNVIROntbrekendeMeldingenV2(pUsername, pPassword: PAnsiChar; 
            PTestServer: Integer;
            UBNNr, BRSnr: PAnsiChar;
            pDierSoort, pMeldingType: Integer;
            pBegindatum, pEinddatum: TDateTime;
            var csvMeldingType, csvUBNnr2ePartij, csvDatum: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            pLogFile: PAnsiChar;
            pMaxStrLen: Integer); stdcall;
            */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIROntbrekendeMeldingenV2(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr,
                             int pDierSoort, int pMeldingType,
                             DateTime pBegindatum, DateTime pEinddatum,
                             ref StringBuilder csvMeldingType, ref StringBuilder csvUBNnr2ePartij, ref StringBuilder csvDatum,
                             ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                             String pLogfile, int pMaxStrLen);


        /*procedure LNVIRRaadplegenMeldingenAlgV2(pUsername, pPassword: PAnsiChar; 
            PTestServer: Integer;
            UBNNr, BRSnr, Levensnr: PAnsiChar;
            pDierSoort, pMeldingType, pMeldingStatus: Integer;
            UBNnr2ePartijd: PAnsiChar;
            pBegindatum, pEinddatum: TDateTime;
            pIndGebeurtenisDatum,pEigenMeldingen, pAndereMeldingen: integer;
            pOutputFile: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            pLogFile: PAnsiChar;
            pMaxStrLen: Integer); stdcall;
            */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRRaadplegenMeldingenAlgV2(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr, String Levensnr,
                             int pDierSoort, int pMeldingType, int pMeldingStatus,
                             String UBNnr2ePartijd,
                             DateTime pBegindatum, DateTime pEinddatum,
                             int pIndGebeurtenisDatum,int pEigenMeldingen,int pAndereMeldingen,
                             String pOutputFile,
                             ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                             String pLogfile, int pMaxStrLen);

        /*
            procedure LNVIRRaadplegenMeldingDetailsV2(
            pUsername, pPassword: PAnsiChar; PTestServer: Integer;
            pUBNNr, pBRSnr, pMeldingnummer: PAnsiChar;
            var pPrognr, pMeldingType, pMeldingStatus: Integer;
            var pGebeurtenisDatum, pHerstelDatum, pIntrekDatum: TDateTime;
            var pLevensnr, pNieuwLevensnr: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            pLogFile: PAnsiChar;
            pMaxStrLen: Integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVIRRaadplegenMeldingDetailsV2(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr, String pMeldingnummer,
                             ref int pPrognr, ref int pMeldingType, ref int pMeldingStatus,
                             ref DateTime pGebeurtenisDatum, ref DateTime pHerstelDatum, ref DateTime pIntrekDatum,
                             ref StringBuilder pLevensnr, ref StringBuilder pNieuwLevensnr,
                             ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                             String pLogfile, int pMaxStrLen);


        /*procedure LNVDierstatusV2(pUsername, pPassword: PAnsiChar; pTestServer: integer;
            Levensnr: PAnsiChar; pDiersoort: integer;
            var BRSnrHouder, UBNhouder, Werknummer: PAnsiChar;
            var Geboortedat, Importdat: TDateTime;
            var LandCodeHerkomst, LandCodeOorsprong: PAnsiChar;
            var Geslacht, Haarkleur: PAnsiChar;
            var Einddatum: TDateTime; var RedenEinde: PAnsiChar;
            var LevensnrMoeder: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            pLogFile: PAnsiChar;
            pMaxStrLen: Integer); stdcall;
            */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVDierstatusV2(String pUsername, String pPassword, int PTestServer,
                             String Levensnr, int pDierSoort,
                             ref StringBuilder BRSnrHouder, ref StringBuilder UBNhouder, ref  StringBuilder Werknummer,
                             ref DateTime Geboortedat, ref  DateTime Importdat,
                             ref StringBuilder LandCodeHerkomst, ref  StringBuilder LandCodeOorsprong,
                             ref StringBuilder Geslacht, ref StringBuilder Haarkleur,
                             ref DateTime Einddatum, ref StringBuilder RedenEinde,
                             ref StringBuilder LevensnrMoeder,
                             ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                             String pLogfile, int pMaxStrLen);

        /* procedure LNVDierdetailsV2(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                UBNnr, BRSnr, Levensnr: PAnsiChar;
                pPrognr, ophVerblijfplaatsen,
                ophVlaggen, ophNakomelingen: integer;
                var Werknummer: PAnsiChar;
                var Geboortedat, Importdat: TDateTime;
                var LandCodeHerkomst, LandCodeOorsprong: PAnsiChar;
                var Geslacht, Haarkleur: PAnsiChar;
                var Einddatum: TDateTime; var RedenEinde: PAnsiChar;
                var LevensnrMoeder: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pDetailFile, pLogFile: PAnsiChar;
                pMaxStrLen: Integer); stdcall;
         * 
         * procedure LNVDierdetailsV2(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                UBNnr, BRSnr, Levensnr: PAnsiChar;
                pPrognr, ophVerblijfplaatsen,
                ophVlaggen, ophNakomelingen: integer;
                var LNVprognr: integer; var Werknummer: PAnsiChar;
                var Geboortedat, Importdat: TDateTime;
                var LandCodeHerkomst, LandCodeOorsprong: PAnsiChar;
                var Geslacht, Haarkleur: PAnsiChar;
                var Einddatum: TDateTime; var RedenEinde: PAnsiChar;
                var LevensnrMoeder: PAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                pDetailFile, pLogFile: PAnsiChar;
                pMaxStrLen: Integer); stdcall;


                 */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVDierdetailsV2(String pUsername, String pPassword, int PTestServer,
                             String UBNnr, String BRSnr, String Levensnr,
                             int pPrognr, int ophVerblijfplaatsen,
                             int ophVlaggen, int ophNakomelingen,
                             ref int LNVprognr, ref StringBuilder Werknummer,
                             ref DateTime Geboortedat, ref  DateTime Importdat,
                             ref StringBuilder LandCodeHerkomst, ref  StringBuilder LandCodeOorsprong,
                             ref StringBuilder Geslacht, ref StringBuilder Haarkleur,
                             ref DateTime Einddatum, ref StringBuilder RedenEinde,
                             ref StringBuilder LevensnrMoeder, ref StringBuilder VervangenLevensnr,
                             ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                             String pDetailFile, String pLogFile,
                             int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNVdierVerblijfplaatsenV2(String pUsername, String pPassword, int PTestServer,
                            String pUBNNr, String pBRSnr, String Levensnr, int pDierSoort,
                            ref StringBuilder csvVerblijfplaats,
                            ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                            String pLogfile, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dLNV_Meldnummer_ADHV_LevensNR(String pUsername, String pPassword, int PTestServer,
                            String pUBNNr, String pBRSnr, String Levensnr,
                            DateTime pDatum, int pDierSoort, int pMeldingType, String pLogfile,
                            ref StringBuilder Meldingsnr, ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                            int pMaxStrLen);

        //[MethodImpl(MethodImplOptions.Synchronized)] serie





        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDIRaanvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime Aanvoerdat,
                                int SoortAanvoer, int pPaspoortnr,
                                ref String Status, ref String Code, ref String Omschrijving, ref String UBNherkomst,
                                ref String pMeldingsnr, String pLogfile, int pMaxStrLen)
        {


            // if (System.Threading.Monitor.TryEnter(padlock))
            //{
            LockObject padlock;


            dCRDIRaanvoermelding handle = (dCRDIRaanvoermelding)ExecuteProcedureDLLStack(typeof(dCRDIRaanvoermelding), "CRDIRaanvoermelding", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lUBNherkomst = new StringBuilder();
                lUBNherkomst.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    Land, UBNnr, Levensnr, Aanvoerdat,
                                    SoortAanvoer, pPaspoortnr,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, ref lMeldingsnr, pLogfile, pMaxStrLen);

                //}
                //else if (System.Threading.Monitor.TryEnter(padlock1))
                //{
                //    dCRDIRaanvoermelding handle = (dCRDIRaanvoermelding)ExecuteProcedureDLLStack(typeof(dCRDIRaanvoermelding), "SOAPIRALG1.DLL", "CRDIRaanvoermelding");
                //    handle(pUsername, pPassword, pTestServer,
                //                        Land, UBNnr, Levensnr, Aanvoerdat,
                //                        SoortAanvoer,
                //                        ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, pLogfile, pMaxStrLen);
                //    System.Threading.Monitor.Exit(padlock1);

                //}
                //else if (System.Threading.Monitor.TryEnter(padlock2))
                //{
                //    dCRDIRaanvoermelding handle = (dCRDIRaanvoermelding)ExecuteProcedureDLLStack(typeof(dCRDIRaanvoermelding), "SOAPIRALG2.DLL", "CRDIRaanvoermelding");
                //    handle(pUsername, pPassword, pTestServer,
                //                        Land, UBNnr, Levensnr, Aanvoerdat,
                //                        SoortAanvoer,
                //                        ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, pLogfile, pMaxStrLen);
                //    System.Threading.Monitor.Exit(padlock2);
                //}
                //else if (System.Threading.Monitor.TryEnter(padlock3))
                //{
                //    dCRDIRaanvoermelding handle = (dCRDIRaanvoermelding)ExecuteProcedureDLLStack(typeof(dCRDIRaanvoermelding), "SOAPIRALG3.DLL", "CRDIRaanvoermelding");
                //    handle(pUsername, pPassword, pTestServer,
                //                        Land, UBNnr, Levensnr, Aanvoerdat,
                //                        SoortAanvoer,
                //                        ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, pLogfile, pMaxStrLen);
                //    System.Threading.Monitor.Exit(padlock3);
                //}


                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                UBNherkomst = lUBNherkomst.ToString();
                pMeldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDIRafvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime Afvoerdat,
                                int SoortAfvoer, int Afvoerreden, int pPaspoortnr, String pLandBestemming,String UBNbestemmingIn,
                                ref String Status, ref String Code, ref String Omschrijving, ref String UBNbestemmingUit,
                                ref String pMeldingsnr, String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dCRDIRafvoermelding handle = (dCRDIRafvoermelding)ExecuteProcedureDLLStack(typeof(dCRDIRafvoermelding), "CRDIRafvoermelding", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lUBNbestemming = new StringBuilder();
                lUBNbestemming.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);

                handle(pUsername, pPassword, pTestServer,
                                    Land, UBNnr, Levensnr, Afvoerdat,
                                    SoortAfvoer, Afvoerreden, pPaspoortnr, pLandBestemming, UBNbestemmingIn,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemming, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                UBNbestemmingUit = lUBNbestemming.ToString();
                pMeldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDIRdoodgeborenmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrMoeder, DateTime Geboortedat,
                                String Geslacht, int Geboorteverloop, int Bevleesheid,
                                int Gewicht, int Overlevingsstatus,
                                int pMeerling, int pMoederRecentAangekocht,
                                ref String Status, ref String Code, ref String Omschrijving,
                                ref String pMeldingsnr, String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dCRDIRdoodgeborenmelding handle = (dCRDIRdoodgeborenmelding)ExecuteProcedureDLLStack(typeof(dCRDIRdoodgeborenmelding), "CRDIRdoodgeborenmelding", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    Land, UBNnr, LevensnrMoeder, Geboortedat,
                                    Geslacht, Geboorteverloop, Bevleesheid,
                                    Gewicht, Overlevingsstatus,
                                    pMeerling, pMoederRecentAangekocht,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                pMeldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDIRgeboortemelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, String Naam, String Werknummer,
                                String LevensnrMoeder, DateTime Geboortedat, DateTime pSterftedatum,
                                String Geslacht, String Haarkleur, int Bijzonderheden, int Geboorteverloop,
                                int Bevleesheid, int Gewicht, int Overlevingsstatus,int CodeNaamgeving,
                                int pRasType, int pPaspoortUrgentie, int pMoederRecentAangekocht,
                                bool Opfok, bool Registratiekaart,
                                ref String Status, ref String Code, ref String Omschrijving,
                                ref String pMeldingsnr, String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dCRDIRgeboortemelding handle = (dCRDIRgeboortemelding)ExecuteProcedureDLLStack(typeof(dCRDIRgeboortemelding), "CRDIRgeboortemelding", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    Land, UBNnr, Levensnr, Naam, Werknummer,
                                    LevensnrMoeder, Geboortedat, pSterftedatum,
                                    Geslacht, Haarkleur, Bijzonderheden, Geboorteverloop,
                                    Bevleesheid, Gewicht, Overlevingsstatus, CodeNaamgeving,
                                    pRasType, pPaspoortUrgentie, pMoederRecentAangekocht,
                                    Opfok, Registratiekaart,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                pMeldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDIRimportmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, String Naam,
                                String Werknummer, String LevensnrMoeder, String LandVertrek,
                                String LandGeboorte, String OrgineelLevensnr,
                                DateTime Geboortedat, DateTime Importdat,
                                String Geslacht, String Haarkleur, bool Subsidie,
                                String nrGezondheidsCert,
                                ref String Status, ref String Code, ref String Omschrijving,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dCRDIRimportmelding handle = (dCRDIRimportmelding)ExecuteProcedureDLLStack(typeof(dCRDIRimportmelding), "CRDIRimportmelding", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    Land, UBNnr, Levensnr, Naam,
                                    Werknummer, LevensnrMoeder, LandVertrek,
                                    LandGeboorte, OrgineelLevensnr,
                                    Geboortedat, Importdat,
                                    Geslacht, Haarkleur, Subsidie,
                                    nrGezondheidsCert,
                                    ref lStatus, ref lCode, ref lOmschrijving, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRaanvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                DateTime Aanvoerdat,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRaanvoermelding handle = (dLNVIRaanvoermelding)ExecuteProcedureDLLStack(typeof(dLNVIRaanvoermelding), "LNVIRaanvoermelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr,
                                    Aanvoerdat,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRafvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                DateTime Afvoerdat,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRafvoermelding handle = (dLNVIRafvoermelding)ExecuteProcedureDLLStack(typeof(dLNVIRafvoermelding), "LNVIRafvoermelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr,
                                    Afvoerdat,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                //FreeDLL("SOAPIRALG.DLL")
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRdoodgeborenmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String LevensnrMoeder,
                                DateTime Geboortedat,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRdoodgeborenmelding handle = (dLNVIRdoodgeborenmelding)ExecuteProcedureDLLStack(typeof(dLNVIRdoodgeborenmelding), "LNVIRdoodgeborenmelding", GetLockList(), out padlock);
            try
            {
                String lLevensnrMoeder = Voorloopnul(LevensnrMoeder);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, LevensnrMoeder,
                                    Geboortedat,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRgeboortemelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String LevensnrMoeder,
                                DateTime Geboortedat,
                                String Geslacht, String Haarkleur,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRgeboortemelding handle = (dLNVIRgeboortemelding)ExecuteProcedureDLLStack(typeof(dLNVIRgeboortemelding), "LNVIRgeboortemelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                String lLevensnrMoeder = Voorloopnul(LevensnrMoeder);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, lLevensnrMoeder,
                                    Geboortedat,
                                    Geslacht, Haarkleur,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                //FreeDLL("SOAPIRALG.DLL")
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRimportmelding(String pUsername, String pPassword, Boolean pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String Naam, String Werknummer, String LevensnrMoeder,
                                String LandVertrek, String LandGeboorte, String OrgineelLevensnr,
                                DateTime Geboortedat, DateTime Importdat,
                                String Geslacht, String Haarkleur, bool Subsidie,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRimportmelding handle = (dLNVIRimportmelding)ExecuteProcedureDLLStack(typeof(dLNVIRimportmelding), "LNVIRimportmelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, Naam, Werknummer, LevensnrMoeder,
                                    LandVertrek, LandGeboorte, OrgineelLevensnr,
                                    Geboortedat, Importdat,
                                    Geslacht, Haarkleur, Subsidie,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRexportmelding(String pUsername, String pPassword, bool pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                DateTime Exportdat,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRexportmelding handle = (dLNVIRexportmelding)ExecuteProcedureDLLStack(typeof(dLNVIRexportmelding), "LNVIRexportmelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr,
                                    Exportdat,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr,
                                    pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                //FreeDLL("SOAPIRALG.DLL")
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRaanvoermeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String UBNHerkomst, int Diersoort,
                                DateTime Aanvoerdat,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRaanvoermeldingV2 handle = (dLNVIRaanvoermeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRaanvoermeldingV2), "LNVIRaanvoermeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, UBNHerkomst, Diersoort,
                                    Aanvoerdat, HerstelMelding, Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRafvoermeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String UBNBestemming, int Diersoort,
                                DateTime Afvoerdat,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRafvoermeldingV2 handle = (dLNVIRafvoermeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRafvoermeldingV2), "LNVIRafvoermeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, UBNBestemming, Diersoort,
                                    Afvoerdat, HerstelMelding, Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                //FreeDLL("SOAPIRALG.DLL")
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRdoodmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, int Diersoort,
                                DateTime Dooddatum,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRdoodmeldingV2 handle = (dLNVIRdoodmeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRdoodmeldingV2), "LNVIRdoodmeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, Diersoort,
                                    Dooddatum, HerstelMelding, Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRdoodgeborenmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String LevensnrMoeder, int Diersoort,
                                DateTime Geboortedat,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRdoodgeborenmeldingV2 handle = (dLNVIRdoodgeborenmeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRdoodgeborenmeldingV2), "LNVIRdoodgeborenmeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnrMoeder = Voorloopnul(LevensnrMoeder);

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnrMoeder, Diersoort,
                                    Geboortedat,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRgeboortemeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String LevensnrMoeder, int Diersoort,
                                DateTime Geboortedat,
                                String Geslacht, String Haarkleur,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRgeboortemeldingV2 handle = (dLNVIRgeboortemeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRgeboortemeldingV2), "LNVIRgeboortemeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                String lLevensnrMoeder = Voorloopnul(LevensnrMoeder);

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);

                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, lLevensnrMoeder, Diersoort,
                                    Geboortedat,
                                    Geslacht, Haarkleur, HerstelMelding, Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRimportmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr, String Naam, String Werknummer, String LevensnrMoeder,
                                String LandVertrek, String LandGeboorte, String OrgineelLevensnr, int Diersoort,
                                DateTime Geboortedat, DateTime Importdat,
                                String Geslacht, String Haarkleur, bool Subsidie,
                                String nrGezondheidsCert,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRimportmeldingV2 handle = (dLNVIRimportmeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRimportmeldingV2), "LNVIRimportmeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, Naam, Werknummer, LevensnrMoeder,
                                    LandVertrek, LandGeboorte, OrgineelLevensnr, Diersoort,
                                    Geboortedat, Importdat,
                                    Geslacht, Haarkleur, Subsidie,
                                    nrGezondheidsCert,
                                    HerstelMelding,
                                    Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRexportmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                int Diersoort, DateTime Exportdat,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRexportmeldingV2 handle = (dLNVIRexportmeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRexportmeldingV2), "LNVIRexportmeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, Diersoort, Exportdat,
                                    HerstelMelding,
                                    Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRnoodslachtmeldingV2(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr,
                                String Levensnr, int Diersoort, DateTime Noodslachtdat,
                                String UBNnoodslacht,
                                int HerstelMelding,
                                String Meldingnr_Oorsprong,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRnoodslachtmeldingV2 handle = (dLNVIRnoodslachtmeldingV2)ExecuteProcedureDLLStack(typeof(dLNVIRnoodslachtmeldingV2), "LNVIRnoodslachtmeldingV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, Diersoort, Noodslachtdat,
                                    UBNnoodslacht,
                                    HerstelMelding,
                                    Meldingnr_Oorsprong,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVQkoortsVaccinatieMelding(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr,
                                int Diersoort,
                                DateTime VaccinatieDatum, int VaccinatieNr,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVQkoortsVaccinatieMelding handle = (dLNVQkoortsVaccinatieMelding)ExecuteProcedureDLLStack(typeof(dLNVQkoortsVaccinatieMelding), "LNVQkoortsVaccinatieMelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr, Diersoort, VaccinatieDatum, VaccinatieNr,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRomnummermelding(String pUsername, String pPassword, int pTestServer,
                                String UBNnr, String BRSnr, String Levensnr_oud, String Levensnr_nieuw,
                                int Diersoort,
                                ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRomnummermelding handle = (dLNVIRomnummermelding)ExecuteProcedureDLLStack(typeof(dLNVIRomnummermelding), "LNVIRomnummermelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr_oud = Voorloopnul(Levensnr_oud);
                String lLevensnr_nieuw = Voorloopnul(Levensnr_nieuw);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, lLevensnr_oud, lLevensnr_nieuw, Diersoort,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIntrekkenMelding(String pUsername, String pPassword, int pTestServer,
                     String UBNnr, String BRSnr, String MeldingsNr,
                     int pDierSoort,
                     ref String Status, ref String Code, ref String Omschrijving,
                     String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIntrekkenMelding handle = (dLNVIntrekkenMelding)ExecuteProcedureDLLStack(typeof(dLNVIntrekkenMelding), "LNVIRintrekkenMeldingV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, pTestServer,
                                    UBNnr, BRSnr, MeldingsNr,
                                    pDierSoort,
                                    ref lStatus, ref lCode, ref lOmschrijving,
                                    pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIROntbrekendeMeldingenV2(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr,
                             int pDierSoort, int pMeldingType,
                             DateTime pBegindatum, DateTime pEinddatum,
                             ref String csvMeldingType, ref String csvUBNnr2ePartij, ref  String csvDatum,
                             ref String Status, ref String Code, ref String Omschrijving,
                             String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIROntbrekendeMeldingenV2 handle = (dLNVIROntbrekendeMeldingenV2)ExecuteProcedureDLLStack(typeof(dLNVIROntbrekendeMeldingenV2), "LNVIROntbrekendeMeldingenV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);

                StringBuilder lcsvMeldingType = new StringBuilder();
                lcsvMeldingType.EnsureCapacity(pMaxStrLen);
                StringBuilder lcsvUBNnr2ePartij = new StringBuilder();
                lcsvUBNnr2ePartij.EnsureCapacity(pMaxStrLen);
                StringBuilder lcsvDatum = new StringBuilder();
                lcsvDatum.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                                 UBNNr, BRSnr,
                                 pDierSoort, pMeldingType,
                                 pBegindatum, pEinddatum,
                                 ref lcsvMeldingType, ref lcsvUBNnr2ePartij, ref lcsvDatum,
                                 ref  lStatus, ref  lCode, ref  lOmschrijving,
                                 pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();

                csvMeldingType = lcsvMeldingType.ToString();
                csvUBNnr2ePartij = lcsvUBNnr2ePartij.ToString();
                csvDatum = lcsvDatum.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRRaadplegenMeldingenAlgV2(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr, String Levensnr,
                             int pDierSoort, int pMeldingType, int pMeldingStatus,
                             String UBNnr2ePartijd,
                             DateTime pBegindatum, DateTime pEinddatum,
                             int pIndGebeurtenisDatum, int pEigenMeldingen, int pAndereMeldingen,
                             String pOutputFile,
                             ref String Status, ref String Code, ref String Omschrijving,
                             String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRRaadplegenMeldingenAlgV2 handle = (dLNVIRRaadplegenMeldingenAlgV2)ExecuteProcedureDLLStack(typeof(dLNVIRRaadplegenMeldingenAlgV2), "LNVIRRaadplegenMeldingenAlgV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                                 UBNNr, BRSnr, Levensnr,
                                 pDierSoort, pMeldingType, pMeldingStatus,
                                 UBNnr2ePartijd,
                                 pBegindatum, pEinddatum,
                                 pIndGebeurtenisDatum, pEigenMeldingen, pAndereMeldingen,
                                 pOutputFile,
                                 ref lStatus, ref lCode, ref lOmschrijving,
                                 pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }


 

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVIRRaadplegenMeldingDetailsV2(String pUsername, String pPassword, int PTestServer,
                             String UBNNr, String BRSnr, String pMeldingnummer,
                             ref int pPrognr, ref int pMeldingType, ref int pMeldingStatus,
                             ref DateTime pGebeurtenisDatum, ref DateTime pHerstelDatum, ref DateTime pIntrekDatum, 
                             ref String pLevensnr, ref String pNieuwLevensnr,
                             ref String Status, ref String Code, ref String Omschrijving,
                             String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVIRRaadplegenMeldingDetailsV2 handle = (dLNVIRRaadplegenMeldingDetailsV2)ExecuteProcedureDLLStack(typeof(dLNVIRRaadplegenMeldingDetailsV2), "LNVIRRaadplegenMeldingDetailsV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                //2014-05-02 luc REF WAARDES ZIJN ALTIJD STRINGBUILDER !!!

                StringBuilder lLevensnr = new StringBuilder(pLevensnr);
                lLevensnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lNieuwLevensnr = new StringBuilder(pNieuwLevensnr);
                lNieuwLevensnr.EnsureCapacity(pMaxStrLen); 


                handle(pUsername, pPassword, PTestServer,
                                 UBNNr, BRSnr, pMeldingnummer,
                                 ref pPrognr, ref pMeldingType, ref pMeldingStatus,
                                 ref pGebeurtenisDatum, ref pHerstelDatum, ref pIntrekDatum,
                                 ref lLevensnr, ref lNieuwLevensnr,
                                 ref lStatus, ref lCode, ref lOmschrijving,
                                 pLogfile, pMaxStrLen);

                pLevensnr = lLevensnr.ToString();
                pNieuwLevensnr = lNieuwLevensnr.ToString();
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //FreeDLL("SOAPIRALG.DLL")
        }




        [MethodImpl(MethodImplOptions.Synchronized)]
        [Obsolete]
        public void LNVDierstatusV2(String pUsername, String pPassword, int PTestServer,
                             String Levensnr, int pDierSoort,
                             ref String BRSnrHouder, ref String UBNhouder, ref String Werknummer,
                             ref DateTime Geboortedat, ref DateTime Importdat,
                             ref String LandCodeHerkomst, ref String LandCodeOorsprong,
                             ref String Geslacht, ref String Haarkleur,
                             ref DateTime Einddatum, ref  String RedenEinde,
                             ref String LevensnrMoeder,
                             ref String Status, ref String Code, ref String Omschrijving,
                             String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVDierstatusV2 handle = (dLNVDierstatusV2)ExecuteProcedureDLLStack(typeof(dLNVDierstatusV2), "LNVDierstatusV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lBRSnrHouder = new StringBuilder();
                lBRSnrHouder.EnsureCapacity(pMaxStrLen);
                StringBuilder lUBNhouder = new StringBuilder();
                lUBNhouder.EnsureCapacity(pMaxStrLen);
                StringBuilder lWerknummer = new StringBuilder();
                lWerknummer.EnsureCapacity(pMaxStrLen);
                StringBuilder lLandCodeHerkomst = new StringBuilder();
                lLandCodeHerkomst.EnsureCapacity(pMaxStrLen);
                StringBuilder lLandCodeOorsprong = new StringBuilder();
                lLandCodeOorsprong.EnsureCapacity(pMaxStrLen);
                StringBuilder lHaarkleur = new StringBuilder();
                lHaarkleur.EnsureCapacity(pMaxStrLen);
                StringBuilder lGeslacht = new StringBuilder();
                lGeslacht.EnsureCapacity(pMaxStrLen);
                StringBuilder lRedenEinde = new StringBuilder();
                lRedenEinde.EnsureCapacity(pMaxStrLen);
                StringBuilder lLevensnrMoeder = new StringBuilder();
                lLevensnrMoeder.EnsureCapacity(pMaxStrLen);

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                                 Levensnr, pDierSoort,
                                 ref lBRSnrHouder, ref lUBNhouder, ref lWerknummer,
                                 ref Geboortedat, ref Importdat,
                                 ref lLandCodeHerkomst, ref lLandCodeOorsprong,
                                 ref lGeslacht, ref lHaarkleur,
                                 ref Einddatum, ref lRedenEinde,
                                 ref lLevensnrMoeder,
                                 ref lStatus, ref  lCode, ref lOmschrijving,
                                 pLogfile, pMaxStrLen);
                BRSnrHouder = lBRSnrHouder.ToString();
                UBNhouder = lUBNhouder.ToString();
                Werknummer = lWerknummer.ToString();
                LandCodeHerkomst = lLandCodeHerkomst.ToString();
                LandCodeOorsprong = lLandCodeOorsprong.ToString();
                Haarkleur = lHaarkleur.ToString();
                Geslacht = lGeslacht.ToString();
                RedenEinde = lRedenEinde.ToString();
                LevensnrMoeder = lLevensnrMoeder.ToString();

                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                //FreeDLL("SOAPIRALG.DLL")
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        [Obsolete] 
        public void LNVdierVerblijfplaatsenV2(String pUsername, String pPassword, int PTestServer,
                            String pUBNNr, String pBRSnr, String Levensnr, int pDierSoort,
                            ref String csvVerblijfplaats,
                            ref String Status, ref String Code, ref String Omschrijving,
                            String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNVdierVerblijfplaatsenV2 handle = (dLNVdierVerblijfplaatsenV2)ExecuteProcedureDLLStack(typeof(dLNVdierVerblijfplaatsenV2), "LNVdierVerblijfplaatsenV2", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lcsvVerblijfplaats = new StringBuilder();
                lcsvVerblijfplaats.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                     pUBNNr, pBRSnr, lLevensnr,
                     pDierSoort,
                     ref lcsvVerblijfplaats,
                     ref lStatus, ref lCode, ref lOmschrijving,
                     pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                csvVerblijfplaats = lcsvVerblijfplaats.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        //BUG 1326
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNVDierdetailsV2(String pUsername, String pPassword, int PTestServer,
                             String UBNnr, String BRSnr, String Levensnr,
                             int pPrognr, int ophVerblijfplaatsen,
                             int ophVlaggen, int ophNakomelingen,
                             ref int LNVprognr, ref String Werknummer,
                             ref DateTime Geboortedat, ref  DateTime Importdat,
                             ref String LandCodeHerkomst, ref  String LandCodeOorsprong,
                             ref String Geslacht, ref String Haarkleur,
                             ref DateTime Einddatum, ref String RedenEinde,
                             ref String LevensnrMoeder, ref String VervangenLevensnr,
                             ref String Status, ref String Code, ref String Omschrijving,
                             String pDetailFile, String pLogFile,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dLNVDierdetailsV2 handle = (dLNVDierdetailsV2)ExecuteProcedureDLLStack(typeof(dLNVDierdetailsV2), "LNVDierdetailsV2", GetLockList(), out padlock);
            try
            {
                StringBuilder lWerknummer = new StringBuilder();
                lWerknummer.EnsureCapacity(pMaxStrLen);
                StringBuilder lLandCodeHerkomst = new StringBuilder();
                lLandCodeHerkomst.EnsureCapacity(pMaxStrLen);
                StringBuilder lLandCodeOorsprong = new StringBuilder();
                lLandCodeOorsprong.EnsureCapacity(pMaxStrLen);
                StringBuilder lHaarkleur = new StringBuilder();
                lHaarkleur.EnsureCapacity(pMaxStrLen);
                StringBuilder lGeslacht = new StringBuilder();
                lGeslacht.EnsureCapacity(pMaxStrLen);
                StringBuilder lRedenEinde = new StringBuilder();
                lRedenEinde.EnsureCapacity(pMaxStrLen);
                StringBuilder lLevensnrMoeder = new StringBuilder();
                lLevensnrMoeder.EnsureCapacity(pMaxStrLen);
                StringBuilder lVervangenLevensnr = new StringBuilder();
                lVervangenLevensnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                                 UBNnr, BRSnr, Levensnr,
                                 pPrognr, ophVerblijfplaatsen,
                                 ophVlaggen, ophNakomelingen,
                                 ref LNVprognr, ref lWerknummer,
                                 ref Geboortedat, ref Importdat,
                                 ref lLandCodeHerkomst, ref lLandCodeOorsprong,
                                 ref lGeslacht, ref lHaarkleur,
                                 ref Einddatum, ref lRedenEinde,
                                 ref lLevensnrMoeder, ref lVervangenLevensnr,
                                 ref lStatus, ref  lCode, ref lOmschrijving,
                                 pDetailFile, pLogFile, pMaxStrLen);

                Werknummer = lWerknummer.ToString();
                LandCodeHerkomst = lLandCodeHerkomst.ToString();
                LandCodeOorsprong = lLandCodeOorsprong.ToString();
                Haarkleur = lHaarkleur.ToString();
                Geslacht = lGeslacht.ToString();
                RedenEinde = lRedenEinde.ToString();
                LevensnrMoeder = lLevensnrMoeder.ToString();
                VervangenLevensnr = lVervangenLevensnr.ToString();
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                //FreeDLL("SOAPIRALG.DLL")
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LNV_Meldnummer_ADHV_LevensNR(String pUsername, String pPassword, int PTestServer,
                            String pUBNNr, String pBRSnr, String Levensnr,
                            DateTime pDatum, int pDierSoort, int pMeldingType,
                            ref String Status, ref String Code, ref String Omschrijving, ref String Meldingsnr,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dLNV_Meldnummer_ADHV_LevensNR handle = (dLNV_Meldnummer_ADHV_LevensNR)ExecuteProcedureDLLStack(typeof(dLNV_Meldnummer_ADHV_LevensNR), "LNV_Meldnummer_ADHV_LevensNR", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                     pUBNNr, pBRSnr, lLevensnr,
                     pDatum,
                     pDierSoort, pMeldingType,
                     pLogfile,
                     ref lMeldingsnr, ref lStatus, ref lCode, ref lOmschrijving,
                      pMaxStrLen);
                Status = lStatus.ToString();
                Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
                Meldingsnr = lMeldingsnr.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        #region I&R HOND Meldingen

        /*
         procedure IRHondGeboortemelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64; pChipDatum,
                     pGeboortedatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
         * string pKVKaanleveraar,
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondGeboortemelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer, DateTime pChipDatum,
                             DateTime pGeboortedatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             string pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondGeboortemelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer, DateTime pChipDatum,
                             DateTime pGeboortedatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             string pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondGeboortemelding handle = (dIRHondGeboortemelding)ExecuteProcedureDLLStack(typeof(dIRHondGeboortemelding), "IRHondGeboortemelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer, pChipDatum,
                             pGeboortedatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking, pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
         procedure IRHondAanvoermelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pAanvoerdatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondAanvoermelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pAanvoerdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             string pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondAanvoermelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pAanvoerdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             string pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondAanvoermelding handle = (dIRHondAanvoermelding)ExecuteProcedureDLLStack(typeof(dIRHondAanvoermelding), "IRHondAanvoermelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pAanvoerdatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking, pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondDoodmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pDooddatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;
        */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondDoodmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pDooddatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             string pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondDoodmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pDooddatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             string pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondDoodmelding handle = (dIRHondDoodmelding)ExecuteProcedureDLLStack(typeof(dIRHondDoodmelding), "IRHondDoodmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pDooddatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
         procedure IRHondAfvoermelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pAfvoerdatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondAfvoermelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pAfvoerdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondAfvoermelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pAfvoerdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondAfvoermelding handle = (dIRHondAfvoermelding)ExecuteProcedureDLLStack(typeof(dIRHondAfvoermelding), "IRHondAfvoermelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pAfvoerdatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondImportmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pImportdatum, pGeboortedatum, pChipdatum,
                     pDatumOntvangstAanleveraar: TDateTime;
                     pLandHerkomst: PAnsiChar;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondImportmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pImportdatum, DateTime pGeboortedatum, DateTime pChipdatum,
                             DateTime pDatumOntvangstAanleveraar,
                             string pLandHerkomst,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondImportmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pImportdatum, DateTime pGeboortedatum, DateTime pChipdatum,
                             DateTime pDatumOntvangstAanleveraar,
                             string pLandHerkomst,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondImportmelding handle = (dIRHondImportmelding)ExecuteProcedureDLLStack(typeof(dIRHondImportmelding), "IRHondImportmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pImportdatum, pGeboortedatum, pChipdatum,
                             pDatumOntvangstAanleveraar,
                             pLandHerkomst,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
         procedure IRHondExportmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pExportdatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondExportmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pExportdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondExportmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pExportdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondExportmelding handle = (dIRHondExportmelding)ExecuteProcedureDLLStack(typeof(dIRHondExportmelding), "IRHondExportmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pExportdatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondVermissingmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pVermissingdatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;
        */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondVermissingmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pVermissingdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondVermissingmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pVermissingdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,   
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondVermissingmelding handle = (dIRHondVermissingmelding)ExecuteProcedureDLLStack(typeof(dIRHondVermissingmelding), "IRHondVermissingmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pVermissingdatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondGevondenmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pGevondendatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondGevondenmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pGevondendatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondGevondenmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pGevondendatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondGevondenmelding handle = (dIRHondGevondenmelding)ExecuteProcedureDLLStack(typeof(dIRHondGevondenmelding), "IRHondGevondenmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pGevondendatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondVervangingmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnrOud, pChipnrNieuw: Int64;
                     pVervangingdatum, pDatumOntvangstAanleveraar: TDateTime;
                     pRedenVervanging: PAnsiChar;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondVervangingmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnrOud, Int64 pChipnrNieuw,
                             DateTime pVervangingdatum, DateTime pDatumOntvangstAanleveraar,
                             string pRedenVervanging,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondVervangingmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnrOud, Int64 pChipnrNieuw,
                             DateTime pVervangingdatum, DateTime pDatumOntvangstAanleveraar,
                             string pRedenVervanging,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondVervangingmelding handle = (dIRHondVervangingmelding)ExecuteProcedureDLLStack(typeof(dIRHondVervangingmelding), "IRHondVervangingmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnrOud, pChipnrNieuw,
                             pVervangingdatum, pDatumOntvangstAanleveraar,
                             pRedenVervanging,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondIntrekmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pMeldingsnrOrg: Int64;
                     pDatumOntvangstAanleveraar: TDateTime;
                     pRedenIntrekking: PAnsiChar;
                     pKenmerkAanleveraar: int64;
                     pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondIntrekmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pMeldingsnrOrg,
                             DateTime pDatumOntvangstAanleveraar,
                             string pRedenIntrekking,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pCertVingerAfdruk,
                             String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondIntrekmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pMeldingsnrOrg,
                             DateTime pDatumOntvangstAanleveraar,
                             string pRedenIntrekking,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pCertVingerAfdruk,
                             String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondIntrekmelding handle = (dIRHondIntrekmelding)ExecuteProcedureDLLStack(typeof(dIRHondIntrekmelding), "IRHondIntrekmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pMeldingsnrOrg,
                             pDatumOntvangstAanleveraar,
                             pRedenIntrekking,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pCertVingerAfdruk,
                             pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
         procedure IRHondContactmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pGebeurtenisdatum, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pContactTekst, pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondContactmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pGebeurtenisdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pContactTekst, String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondContactmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pGebeurtenisdatum, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pContactTekst, String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondContactmelding handle = (dIRHondContactmelding)ExecuteProcedureDLLStack(typeof(dIRHondContactmelding), "IRHondContactmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pGebeurtenisdatum, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pContactTekst, pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondAdreswijzigingmelding(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pChipnummer: Int64;
                     pDatumAdreswijziging, pDatumOntvangstAanleveraar: TDateTime;
                     pDierSoort: integer;
                     pKenmerkAanleveraar: int64;
                     pOpmerking: PAnsiChar;
                     pRelatieFile, pLogFile: PAnsiChar;
                     var RegistratieDatum: TDateTime;
                     var Status, Omschrijving, Meldingsnr: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondAdreswijzigingmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pDatumAdreswijziging, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondAdreswijzigingmelding(String pUsername, String pPassword, int PTestServer,
                             Int64 pChipnummer,
                             DateTime pDatumAdreswijziging, DateTime pDatumOntvangstAanleveraar,
                             int pDierSoort,
                             Int64 pKenmerkAanleveraar,
                             String pKVKaanleveraar,
                             String pOpmerking,
                             String pCertVingerAfdruk,
                             String pRelatieFile, String pLogFile,
                             ref DateTime RegistratieDatum,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondAdreswijzigingmelding handle = (dIRHondAdreswijzigingmelding)ExecuteProcedureDLLStack(typeof(dIRHondAdreswijzigingmelding), "IRHondAdreswijzigingmelding", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pChipnummer,
                             pDatumAdreswijziging, pDatumOntvangstAanleveraar,
                             pDierSoort,
                             pKenmerkAanleveraar,
                             pKVKaanleveraar,
                             pOpmerking,
                             pCertVingerAfdruk,
                             pRelatieFile, pLogFile,
                             ref RegistratieDatum,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        /*
            procedure IRHondRaadplegenmeldingen(
                     pUsername, pPassword: PAnsiChar; pTestServer: integer;
                     pNaamChipper, pNaamHouder: PAnsiChar;
                     pChipnummer: Int64;
                     pDatumGebeurtenisLaag, pDatumGebeurtenisHoog,
                     pDatumOntvangstAanleveraarLaag,
                     pDatumOntvangstAanleveraarHoog,
                     pDatumRegistratieLaag,
                     pDatumRegistratieHoog: TDateTime;
                     pDierSoort: integer;
                     pHuisnrChipper, pHuisnrHouder: integer;
                     pAlleenOpenbaar, pIngetrokkenOverslaan: integer;
                     pKenmerkAanleveraarLaag,
                     pKenmerkAanleveraarHoog: int64;
                     pKvKNummerHouder: PAnsiChar;
                     pMaxAantMeldingen: integer;
                     pMeldingsnrLaag, pMeldingsnrHoog: int64;
                     pPlaatsnaamChipper, pPlaatsnaamHouder: PAnsiChar;
                     pPostcodeChipper, pPostcodeHouder: PAnsiChar;
                     pRegnrAanleveraar: int64;
                     pStraatnaamChipper, pStraatNaamHouder: PAnsiChar;
                     pTypeMelding: integer;
                     pMeldingenFile, pRelatieFile, pLogFile: PAnsiChar;
                     var Status, Omschrijving: PAnsiChar;
                     pMaxStrLen: Integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dIRHondRaadplegenmeldingen(String pUsername, String pPassword, int PTestServer,
                             String pKVKaanleveraar,
                             String pNaamChipper, String pNaamHouder,
                             Int64 pChipnummer,
                             DateTime pDatumGebeurtenisLaag, DateTime pDatumGebeurtenisHoog,
                             DateTime DatumOntvangstAanleveraarLaag,
                             DateTime pDatumOntvangstAanleveraarHoog,
                             DateTime pDatumRegistratieLaag,
                             DateTime pDatumRegistratieHoog,
                             int pDierSoort,
                             int pHuisnrChipper, int pHuisnrHouder,
                             int pAlleenOpenbaar, int pIngetrokkenOverslaan,
                             Int64 pKenmerkAanleveraarLaag,
                             Int64 pKenmerkAanleveraarHoog,
                             String pKvKNummerHouder,
                             int pMaxAantMeldingen,
                             Int64 pMeldingsnrLaag, Int64 pMeldingsnrHoog,
                             String pPlaatsnaamChipper, String pPlaatsnaamHouder,
                             String pPostcodeChipper, String pPostcodeHouder,
                             Int64 pRegnrAanleveraar,
                             String pStraatnaamChipper, String pStraatNaamHouder,
                             int pTypeMelding,
                             String pCertVingerAfdruk,
                             String pMeldingenFile, String pRelatieFile, String pLogFile,
                             ref StringBuilder Status, ref StringBuilder Omschrijving, ref StringBuilder Meldingsnr,
                             int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IRHondRaadplegenmeldingen(String pUsername, String pPassword, int PTestServer,
            String pKVKaanleveraar,                 
            String pNaamChipper, String pNaamHouder,
                             Int64 pChipnummer,
                             DateTime pDatumGebeurtenisLaag, DateTime pDatumGebeurtenisHoog,
                             DateTime DatumOntvangstAanleveraarLaag,
                             DateTime pDatumOntvangstAanleveraarHoog,
                             DateTime pDatumRegistratieLaag,
                             DateTime pDatumRegistratieHoog,
                             int pDierSoort,
                             int pHuisnrChipper, int pHuisnrHouder,
                             int pAlleenOpenbaar, int pIngetrokkenOverslaan,
                             Int64 pKenmerkAanleveraarLaag,
                             Int64 pKenmerkAanleveraarHoog,
                             String pKvKNummerHouder,
                             int pMaxAantMeldingen,
                             Int64 pMeldingsnrLaag, Int64 pMeldingsnrHoog,
                             String pPlaatsnaamChipper, String pPlaatsnaamHouder,
                             String pPostcodeChipper, String pPostcodeHouder,
                             Int64 pRegnrAanleveraar,
                             String pStraatnaamChipper, String pStraatNaamHouder,
                             int pTypeMelding,
                             String pCertVingerAfdruk,
                             String pMeldingenFile, String pRelatieFile, String pLogFile,
                             ref String Status, ref String Omschrijving, ref String Meldingsnr,
                             int pMaxStrLen)
        {
            LockObject padlock;
            dIRHondRaadplegenmeldingen handle = (dIRHondRaadplegenmeldingen)ExecuteProcedureDLLStack(typeof(dIRHondRaadplegenmeldingen), "IRHondRaadplegenmeldingen", GetLockList(), out padlock);
            try
            {

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lMeldingsnr = new StringBuilder();
                lMeldingsnr.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                handle(pUsername, pPassword, PTestServer,
                             pKVKaanleveraar,
                             pNaamChipper, pNaamHouder,
                             pChipnummer,
                             pDatumGebeurtenisLaag, pDatumGebeurtenisHoog,
                             DatumOntvangstAanleveraarLaag,
                             pDatumOntvangstAanleveraarHoog,
                             pDatumRegistratieLaag,
                             pDatumRegistratieHoog,
                             pDierSoort,
                             pHuisnrChipper, pHuisnrHouder,
                             pAlleenOpenbaar, pIngetrokkenOverslaan,
                             pKenmerkAanleveraarLaag,
                             pKenmerkAanleveraarHoog,
                             pKvKNummerHouder,
                             pMaxAantMeldingen,
                             pMeldingsnrLaag, pMeldingsnrHoog,
                             pPlaatsnaamChipper, pPlaatsnaamHouder,
                             pPostcodeChipper, pPostcodeHouder,
                             pRegnrAanleveraar,
                             pStraatnaamChipper, pStraatNaamHouder,
                             pTypeMelding,
                             pCertVingerAfdruk,
                             pMeldingenFile, pRelatieFile, pLogFile,
                             ref lStatus, ref  lOmschrijving, ref  lMeldingsnr,
                             pMaxStrLen);
                Status = lStatus.ToString();
                Meldingsnr = lMeldingsnr.ToString();
                Omschrijving = lOmschrijving.ToString();

            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }

        #endregion




        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dFHRSgeboortemelding(bool pTestServer,
                                String UBNnr, String Levensnr,String Naam, String Werknummer, String LevensnrMoeder,
                                DateTime Geboortedat,
                                String Geslacht, String Haarkleur, int Bijzonderheden,
                                bool Opfok,
                                ref StringBuilder Status,ref StringBuilder Omschrijving,
                                String pLogfile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FHRSgeboortemelding(bool pTestServer,
                                String UBNnr, String Levensnr, String Naam, String Werknummer, String LevensnrMoeder,
                                DateTime Geboortedat,
                                String Geslacht, String Haarkleur, int Bijzonderheden,
                                bool Opfok,
                                ref String Status, ref String Omschrijving,
                                String pLogfile, int pMaxStrLen)
        {
            LockObject padlock;
            dFHRSgeboortemelding handle = (dFHRSgeboortemelding)ExecuteProcedureDLLStack(typeof(dFHRSgeboortemelding), "FHRSgeboortemelding", GetLockList(), out padlock);
            try
            {
                String lLevensnr = Voorloopnul(Levensnr);
                String lLevensnrMoeder = Voorloopnul(LevensnrMoeder);

                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);

                handle(pTestServer,
                                    UBNnr, lLevensnr, Naam, Werknummer, lLevensnrMoeder,
                                    Geboortedat,
                                    Geslacht, Haarkleur, Bijzonderheden, Opfok,
                                    ref lStatus, ref lOmschrijving, pLogfile, pMaxStrLen);
                Status = lStatus.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }


        private String Voorloopnul(String Levensnr)
        {
            char[] splittrim = { ' ' };
            String lLevensnr = Levensnr;
            string[] teststring = Levensnr.Split(splittrim);
            if (teststring.Length == 2 && lLevensnr.ToUpper().StartsWith("NL"))
            {
                if (teststring[1].StartsWith("0"))
                {
                    teststring[1] = removevoorloopnullen(teststring[1]);
                    lLevensnr = teststring[0].Trim() + " " + teststring[1].Trim();
                }

            }
            return lLevensnr;
        }

        private string removevoorloopnullen(string p)
        {
            while (p.StartsWith("0"))
            {
                int lengte = p.Length;
                if (lengte > 1)
                {
                    p = p.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }
            return p;
        }


    }
}
