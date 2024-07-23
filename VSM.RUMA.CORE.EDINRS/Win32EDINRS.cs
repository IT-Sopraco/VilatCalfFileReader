using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    class Win32EDINRS : Win32
    {
        private int pMaxStrLen = 255;
        public Win32EDINRS()
            : base(false)
        {
            LoadLibraryRelative("lib\\IMPEDINRS.DLL");
        }
        ~Win32EDINRS()
        {
            base.FreeLibraryRelative("lib\\IMPEDINRS.DLL");
            base.Dispose();
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
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\IMPEDINRS.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\IMPEDINRS{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        public delegate void pCallback(int PercDone, string Msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int HeaderInformatie(String pBestand,
              ref StringBuilder Berichttype, ref StringBuilder Versienr, ref StringBuilder Specificatie,
              ref DateTime BestandsDatum, ref DateTime BestandsTijd, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int Bedrijfsgegevens(String pBestand, ref StringBuilder Land, ref StringBuilder UBNnr,
             ref StringBuilder Naam, ref StringBuilder Voorvoegsel, ref StringBuilder Straat, ref StringBuilder Huisnummer,
             ref StringBuilder Postcode, ref StringBuilder Woonplaats, ref StringBuilder Telefoonnr, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int RegistratieRund(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref StringBuilder Naam, ref StringBuilder Geslacht, ref StringBuilder RegistratieSoort,
            ref DateTime geboortedat, ref DateTime importdat,
            ref StringBuilder UBNfokker, ref StringBuilder Haarkleur, ref StringBuilder LandHerkomst,
            ref StringBuilder LevnrMoeder, ref StringBuilder LevnrVader,
            ref int Draagtijd, ref int CodeRelatieMetBedrijf, int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int KIstier(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref StringBuilder KIcode,
            ref StringBuilder NaamAfgekort, ref StringBuilder NaamEigenaar, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int AfvoerRund(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref StringBuilder UBNnr,
            ref DateTime Datum,
            ref int CodeAfvoer, ref int CodeRedenAfvoer, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int RegistratieRundOpBedrijf(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref StringBuilder UBNnr,
            ref int groepsnr, ref int koenr, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int AanvoerRund(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref StringBuilder UBNnr,
            ref DateTime Datum,
            ref int CodeAanvoer, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int RegistratieRas(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref StringBuilder CodeRas,
            ref int RasDeel, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int AfkalvingKoe(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref DateTime Datum,
            ref int AantalKalveren, ref int Pariteit, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int LactatieProduktie(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref DateTime DatumAfkalven,
            ref int KgMelkLact, ref int KgEiwitLactX100, ref int KgVetLactX100,
            ref int KgMelk305, ref int KgEiwit305x100, ref int KgVet305x100,
            ref int LactatieWaarde, ref int NettoOpbrengst,
            ref int DagenInLactatie, ref int BSKx10, ref int IndicatieEMM, ref int DierStatus, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int DagProduktie(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref DateTime Datum,
            ref int KgMelkDagProdX10, ref int PercEiwitDagProdX100, ref int PercVetDagProdX100,
            ref int PercLactoseX100, ref int Ureum, ref int MelkCelgetal, ref int IndicatieEMM, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int DagLactatieProduktie(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref DateTime Datum,
            ref int KgMelkDagProdX10, ref int PercEiwitDagProdX100, ref int PercVetDagProdX100,
            ref int PercLactoseX100, ref int Ureum, ref int MelkCelgetal,
            ref DateTime DatumAfkalven,
            ref int KgMelkLact, ref int KgEiwitLactX100, ref int KgVetLactX100,
            ref int KgMelk305, ref int KgEiwit305x100, ref int KgVet305x100,
            ref int LactatieWaarde, ref int NettoOpbrengst,
            ref int DagenInLactatie, ref int BSKx10, ref int IndicatieEMM, ref int DierStatus, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int DekkingRund(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref DateTime Datum,
            ref StringBuilder DekInfo, ref StringBuilder Levensnrstier,
            ref DateTime DatumEindeSamenweiden, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int ImplantatieEmbryo(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref DateTime Datum,
            ref StringBuilder LevensnrETmoeder, ref StringBuilder Levensnrstier, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int MutatieProduktiestatus(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref int StatusProduktie,
            ref DateTime Datum, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int MutatieDrachtigheidsstatus(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr,
            ref int StatusDracht,
            ref DateTime Datum, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int BedrijfsgegevensMelking(String pBestand, int pRegelnr,
            ref StringBuilder UBNnr, ref DateTime DatumProduktie,
            ref DateTime TijdBeginMelking, ref DateTime TijdEindMelking, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int BedrijfsgegevensDagproduktie(String pBestand, int pRegelnr,
            ref StringBuilder UBNnr, ref DateTime DatumProduktie,
            ref int AantKoeienBemonsterd, ref int AantKoeienDroog, ref int AantKoeienMelkgevend,
            ref int AantMelkingenPerDag, ref int CodeAfleidingDagprod,
            ref DateTime DatumOnderzoekVetEiwit,
            ref int KgMelkDagprod, ref int KgVetDagprodX100, ref int KgEiwitDagprodX100,
            ref int KgMelk305gem, ref int KgVet305gemX100, ref int KgEiwit305gemX100,
            ref int KgMelk305st, ref int KgVet305stX100, ref int KgEiwit305stX100,
            ref int NettoOpbrengst, ref int BSKx10, ref int StatusDagProduktie,
            ref int MPR24hLaatste, ref int MPR24hJaarGem, ref int IndicatieEMM, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int IRretour_Terugmelding(String pBestand, int pRegelnr,
            ref StringBuilder UBNnr, ref StringBuilder Levensnr, ref StringBuilder Koenummer, ref StringBuilder Naam, ref StringBuilder Geslacht,
            ref StringBuilder Haarkleur, ref StringBuilder LevensnrMoeder, ref StringBuilder CodeOpvragenNaam,
            ref DateTime DatumMutatie,
            ref int SoortMutatie, ref int BijzonderHeden,
            ref int IndVeeverbetering,
            ref DateTime DatumMelding, ref DateTime TijdMelding,
            ref DateTime DatumVerwerking, ref DateTime TijdVerwerking,
            ref int NummerBron,
            ref StringBuilder Herkomst, ref StringBuilder BerichtID, ref StringBuilder VersienrBerichtType,
            ref StringBuilder ReleasenrBerichtType, ref StringBuilder ZenderID, ref StringBuilder TypeMedium,
            ref StringBuilder CodeVerwerking, ref StringBuilder CodeResultaatVerwerking,
            ref StringBuilder OmschrijvingResultaat, ref StringBuilder IKBrund,
            ref StringBuilder IRBlokkadeRund, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int websterHT2data(String pBestand, int pRegelnr,
            ref StringBuilder UBNnr, ref int SoortMutatie,
            ref DateTime DatumMutatie, ref DateTime TijdMutatie,
            ref StringBuilder Levensnummer, ref DateTime Geboortedatum,
            ref StringBuilder Geslacht, ref StringBuilder Haarkleur, ref StringBuilder LevnrMoeder, int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int FokwaardenExterieur(String pBestand, int pRegelnr,
           ref StringBuilder Levensnr, ref DateTime Datum,
           ref int IndFWkoeDracht, ref int JaarBasis, ref int Betrouwbh,
           ref StringBuilder RasBasis, ref int AchterSpeenplaatsing, ref int AchterUierhoogte,
           ref int Benen, ref int BespieringBovenbalk, ref int BespieringOnderbalk,
           ref int Hoogtemaat, ref int Inhoud, ref int Klauwhoek, ref int KruisBreedte,
           ref int Kruisligging, ref int Openheid, ref int Ophangband,
           ref int Speenlengte, ref int StandAchterbenen,
           ref int AchterbeenstandAchteraanzicht,
           ref int AlgVoorkomen, ref int FrameRoodbont, ref int FrameZwartbont,
           ref int FrameMRIJ, ref int Uier, ref int UierDiepte,
           ref int Voorspeenplaatsing, ref int VoorUieraanhechting,
           ref int Voorhand, ref int Conditiescore, ref int Robustheid,
           ref int Beengebruik, int pMaxStrLen);


        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate int FokwaardenGeboorteKenmerken(String pBestand, int pRegelnr,
            ref StringBuilder Levensnr, ref DateTime Datum,
            ref int JaarBasis, ref StringBuilder RasBasis, ref int AchterSpeenplaatsing, ref int AchterUierhoogte, 
            ref int Draagtijd, ref int Geboortegewicht, ref int Geboortegemak, ref int BthOverleving, ref int Overleving,
            ref int BthAfkalfgemak, ref int Afkalfgemak, int pMaxStrLen);

 


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_HeaderInformatie(String pBestand,
                      out String Berichttype, out String Versienr, out String Specificatie,
                      out DateTime BestandsDatum, out DateTime BestandsTijd)
        {
            int result = -1;
            StringBuilder lBerichttype = new StringBuilder();
            StringBuilder lVersienr = new StringBuilder();
            StringBuilder lSpecificatie = new StringBuilder();
            unLogger.WriteDebug("HeaderInformatie In");
            LockObject padlock;
            HeaderInformatie handle = (HeaderInformatie)ExecuteProcedureDLLStack(typeof(HeaderInformatie), "edinrs_HeaderInformatie", GetLockList(), out padlock);
            try
            {
                lBerichttype.EnsureCapacity(pMaxStrLen);
                lVersienr.EnsureCapacity(pMaxStrLen);
                lSpecificatie.EnsureCapacity(pMaxStrLen);
                BestandsDatum = DateTime.MinValue;
                BestandsTijd = DateTime.MinValue;
                unLogger.WriteDebug("HeaderInformatie Exe In");
                result = handle(pBestand,
                    ref lBerichttype,
                    ref lVersienr,
                    ref lSpecificatie,
                    ref BestandsDatum,
                    ref BestandsTijd, pMaxStrLen);
                unLogger.WriteDebug("HeaderInformatie Exe Uit");
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Berichttype = lBerichttype.ToString();
            Versienr = lVersienr.ToString();
            Specificatie = lSpecificatie.ToString();
            unLogger.WriteDebug("HeaderInformatie Uit");
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_Bedrijfsgegevens(String pBestand, out String Land, out String UBNnr,
                      out String Naam, out String Voorvoegsel, out String Straat, out String Huisnummer,
                      out String Postcode, out String Woonplaats, out String Telefoonnr)
        {
            int result = -1;
            unLogger.WriteDebug("Bedrijfsgegevens In");
            StringBuilder lLand = new StringBuilder();
            StringBuilder lUBNnr = new StringBuilder();
            StringBuilder lNaam = new StringBuilder();
            StringBuilder lVoorvoegsel = new StringBuilder();
            StringBuilder lStraat = new StringBuilder();
            StringBuilder lHuisnummer = new StringBuilder();
            StringBuilder lPostcode = new StringBuilder();
            StringBuilder lWoonplaats = new StringBuilder();
            StringBuilder lTelefoonnr = new StringBuilder();
            LockObject padlock;
            Bedrijfsgegevens handle = (Bedrijfsgegevens)ExecuteProcedureDLLStack(typeof(Bedrijfsgegevens), "edinrs_Bedrijfsgegevens", GetLockList(), out padlock);
            try
            {

                lLand.EnsureCapacity(pMaxStrLen);
                lUBNnr.EnsureCapacity(pMaxStrLen);
                lNaam.EnsureCapacity(pMaxStrLen);
                lVoorvoegsel.EnsureCapacity(pMaxStrLen);
                lStraat.EnsureCapacity(pMaxStrLen);
                lHuisnummer.EnsureCapacity(pMaxStrLen);
                lPostcode.EnsureCapacity(pMaxStrLen);
                lWoonplaats.EnsureCapacity(pMaxStrLen);
                lTelefoonnr.EnsureCapacity(pMaxStrLen);
                unLogger.WriteDebug("Bedrijfsgegevens Exe In");


                result = handle(pBestand, ref lLand, ref lUBNnr,
                          ref lNaam, ref lVoorvoegsel, ref lStraat, ref lHuisnummer,
                          ref lPostcode, ref lWoonplaats, ref lTelefoonnr, pMaxStrLen);

                unLogger.WriteDebug("Bedrijfsgegevens  Exe Uit");
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            //
            Land = lLand.ToString();
            UBNnr = lUBNnr.ToString();
            Naam = lNaam.ToString();
            Voorvoegsel = lVoorvoegsel.ToString();
            Straat = lStraat.ToString();
            Huisnummer = lHuisnummer.ToString();
            Postcode = lPostcode.ToString();
            Woonplaats = lWoonplaats.ToString();
            Telefoonnr = lTelefoonnr.ToString();
            unLogger.WriteDebug("Bedrijfsgegevens Uit");
            return result;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_RegistratieRund(String pBestand, int pRegelnr,
            out String Levensnr, out String Naam, out String Geslacht, out String RegistratieSoort,
            out DateTime geboortedat, out DateTime importdat,
            out String UBNfokker, out String Haarkleur, out String LandHerkomst,
            out String LevnrMoeder, out String LevnrVader,
            out int Draagtijd, out int CodeRelatieMetBedrijf)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lNaam = new StringBuilder();
            lNaam.EnsureCapacity(pMaxStrLen);
            StringBuilder lGeslacht = new StringBuilder();
            lGeslacht.EnsureCapacity(pMaxStrLen);
            StringBuilder lRegistratieSoort = new StringBuilder();
            lRegistratieSoort.EnsureCapacity(pMaxStrLen);
            StringBuilder lUBNfokker = new StringBuilder();
            lUBNfokker.EnsureCapacity(pMaxStrLen);
            StringBuilder lHaarkleur = new StringBuilder();
            lHaarkleur.EnsureCapacity(pMaxStrLen);
            StringBuilder lLandHerkomst = new StringBuilder();
            lLandHerkomst.EnsureCapacity(pMaxStrLen);
            StringBuilder lLevnrMoeder = new StringBuilder();
            lLevnrMoeder.EnsureCapacity(pMaxStrLen);
            StringBuilder lLevnrVader = new StringBuilder();
            lLevnrVader.EnsureCapacity(pMaxStrLen);
            LockObject padlock;
            RegistratieRund handle = (RegistratieRund)ExecuteProcedureDLLStack(typeof(RegistratieRund), "edinrs_RegistratieRund", GetLockList(), out padlock);
            try
            {
                geboortedat = DateTime.MinValue;
                importdat = DateTime.MinValue;
                Draagtijd = -1;
                CodeRelatieMetBedrijf = -1;
                result = handle(pBestand, pRegelnr,
                               ref lLevensnr, ref lNaam, ref lGeslacht, ref lRegistratieSoort,
                               ref geboortedat, ref importdat,
                               ref lUBNfokker, ref lHaarkleur, ref lLandHerkomst,
                               ref lLevnrMoeder, ref lLevnrVader,
                               ref Draagtijd, ref CodeRelatieMetBedrijf, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            Naam = lNaam.ToString();
            Geslacht = lGeslacht.ToString();
            RegistratieSoort = lRegistratieSoort.ToString();
            UBNfokker = lUBNfokker.ToString();
            Haarkleur = lHaarkleur.ToString();
            LandHerkomst = lLandHerkomst.ToString();
            LevnrMoeder = lLevnrMoeder.ToString();
            LevnrVader = lLevnrVader.ToString();
            return result;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_KIstier(String pBestand, int pRegelnr,
            out String Levensnr, out String KIcode,
            out String NaamAfgekort, out String NaamEigenaar)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lKIcode = new StringBuilder();
            lKIcode.EnsureCapacity(pMaxStrLen);
            StringBuilder lNaamAfgekort = new StringBuilder();
            lNaamAfgekort.EnsureCapacity(pMaxStrLen);
            StringBuilder lNaamEigenaar = new StringBuilder();
            lNaamEigenaar.EnsureCapacity(pMaxStrLen);
            LockObject padlock;
            KIstier handle = (KIstier)ExecuteProcedureDLLStack(typeof(KIstier), "edinrs_KIstier", GetLockList(), out padlock);
            try
            {
                result = handle(pBestand, pRegelnr,
                                ref lLevensnr, ref lKIcode,
                                ref lNaamAfgekort, ref lNaamEigenaar, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            KIcode = lKIcode.ToString();
            NaamAfgekort = lNaamAfgekort.ToString();
            NaamEigenaar = lNaamEigenaar.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_AfvoerRund(String pBestand, int pRegelnr,
            out String Levensnr, out String UBNnr,
            out DateTime Datum,
            out int CodeAfvoer, out int CodeRedenAfvoer)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lUBNnr = new StringBuilder();
            lUBNnr.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            CodeAfvoer = -1;
            CodeRedenAfvoer = -1;
            LockObject padlock;
            AfvoerRund handle = (AfvoerRund)ExecuteProcedureDLLStack(typeof(AfvoerRund), "edinrs_AfvoerRund", GetLockList(), out padlock);
            try
            {
                result = handle(pBestand, pRegelnr,
                                ref lLevensnr, ref lUBNnr,
                                ref Datum,
                                ref CodeAfvoer, ref CodeRedenAfvoer, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            UBNnr = lUBNnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_RegistratieRundOpBedrijf(String pBestand, int pRegelnr,
            out String Levensnr, out String UBNnr,
            out int groepsnr, out int koenr)
        {
            int result = -1;
            LockObject padlock;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lUBNnr = new StringBuilder();
            lUBNnr.EnsureCapacity(pMaxStrLen);
            groepsnr = -1;
            koenr = -1;
            RegistratieRundOpBedrijf handle = (RegistratieRundOpBedrijf)ExecuteProcedureDLLStack(typeof(RegistratieRundOpBedrijf), "edinrs_RegistratieRundOpBedrijf", GetLockList(), out padlock);

            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr, ref lUBNnr,
                    ref groepsnr, ref koenr, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            UBNnr = lUBNnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_AanvoerRund(String pBestand, int pRegelnr,
            out String Levensnr, out String UBNnr,
            out DateTime Datum,
            out int CodeAanvoer)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lUBNnr = new StringBuilder();
            lUBNnr.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            CodeAanvoer = -1;
            LockObject padlock;
            AanvoerRund handle = (AanvoerRund)ExecuteProcedureDLLStack(typeof(AanvoerRund), "edinrs_AanvoerRund", GetLockList(), out padlock);
            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr, ref lUBNnr,
                    ref Datum,
                    ref CodeAanvoer, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            UBNnr = lUBNnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_RegistratieRas(String pBestand, int pRegelnr,
            out String Levensnr, out String CodeRas,
            out int RasDeel)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lCodeRas = new StringBuilder();
            lCodeRas.EnsureCapacity(pMaxStrLen);
            RasDeel = -1;
            LockObject padlock;
            RegistratieRas handle = (RegistratieRas)ExecuteProcedureDLLStack(typeof(RegistratieRas), "edinrs_RegistratieRas", GetLockList(), out padlock);

            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr, ref lCodeRas, ref RasDeel, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            CodeRas = lCodeRas.ToString();
            return result;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_AfkalvingKoe(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out int AantalKalveren, out int Pariteit)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            AantalKalveren = -1;
            Pariteit = -1;
            LockObject padlock;
            AfkalvingKoe handle = (AfkalvingKoe)ExecuteProcedureDLLStack(typeof(AfkalvingKoe), "edinrs_AfkalvingKoe", GetLockList(), out padlock);
            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref Datum,
                    ref AantalKalveren, ref Pariteit, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_LactatieProduktie(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime DatumAfkalven,
            out int KgMelkLact, out int KgEiwitLactX100, out int KgVetLactX100,
            out int KgMelk305, out int KgEiwit305x100, out int KgVet305x100,
            out int LactatieWaarde, out int NettoOpbrengst,
            out int DagenInLactatie, out int BSKx10, out int IndicatieEMM, out int DierStatus)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            DatumAfkalven = DateTime.MinValue;
            KgMelkLact = -1;
            KgEiwitLactX100 = -1;
            KgVetLactX100 = -1;
            KgMelk305 = -1;
            KgEiwit305x100 = -1;
            KgVet305x100 = -1;
            LactatieWaarde = -1;
            NettoOpbrengst = -1;
            DagenInLactatie = -1;
            BSKx10 = -1;
            IndicatieEMM = -1;
            DierStatus = -1;
            LockObject padlock;
            LactatieProduktie handle = (LactatieProduktie)ExecuteProcedureDLLStack(typeof(LactatieProduktie), "edinrs_LactatieProduktie", GetLockList(), out padlock);
            try
            {

                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref DatumAfkalven,
                    ref KgMelkLact, ref KgEiwitLactX100, ref KgVetLactX100,
                    ref KgMelk305, ref KgEiwit305x100, ref KgVet305x100,
                    ref LactatieWaarde, ref NettoOpbrengst,
                    ref DagenInLactatie, ref BSKx10, ref IndicatieEMM, ref DierStatus, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_DagProduktie(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out int KgMelkDagProdX10, out int PercEiwitDagProdX100, out int PercVetDagProdX100,
            out int PercLactoseX100, out int Ureum, out int MelkCelgetal, out int IndicatieEMM)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            KgMelkDagProdX10 = -1;
            PercEiwitDagProdX100 = -1;
            PercVetDagProdX100 = -1;
            PercLactoseX100 = -1;
            Ureum = -1;
            MelkCelgetal = -1;
            IndicatieEMM = -1;
            LockObject padlock;
            DagProduktie handle = (DagProduktie)ExecuteProcedureDLLStack(typeof(DagProduktie), "edinrs_DagProduktie", GetLockList(), out padlock);
            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref Datum,
                    ref KgMelkDagProdX10, ref PercEiwitDagProdX100, ref PercVetDagProdX100,
                    ref PercLactoseX100, ref Ureum, ref MelkCelgetal, ref IndicatieEMM, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_DagLactatieProduktie(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out int KgMelkDagProdX10, out int PercEiwitDagProdX100, out int PercVetDagProdX100,
            out int PercLactoseX100, out int Ureum, out int MelkCelgetal,
            out DateTime DatumAfkalven,
            out int KgMelkLact, out int KgEiwitLactX100, out int KgVetLactX100,
            out int KgMelk305, out int KgEiwit305x100, out int KgVet305x100,
            out int LactatieWaarde, out int NettoOpbrengst,
            out int DagenInLactatie, out int BSKx10, out int IndicatieEMM, out int DierStatus)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            KgMelkDagProdX10 = -1;
            PercEiwitDagProdX100 = -1;
            PercVetDagProdX100 = -1;
            PercLactoseX100 = -1;
            Ureum = -1;
            MelkCelgetal = -1;
            DatumAfkalven = DateTime.MinValue;
            KgMelkLact = -1;
            KgEiwitLactX100 = -1;
            KgVetLactX100 = -1;
            KgMelk305 = -1;
            KgEiwit305x100 = -1;
            KgVet305x100 = -1;
            LactatieWaarde = -1;
            NettoOpbrengst = -1;
            DagenInLactatie = -1;
            BSKx10 = -1;
            IndicatieEMM = -1;
            DierStatus = -1;
            LockObject padlock;
            DagLactatieProduktie handle = (DagLactatieProduktie)ExecuteProcedureDLLStack(typeof(DagLactatieProduktie), "edinrs_DagLactatieProduktie", GetLockList(), out padlock);

            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref Datum,
                    ref KgMelkDagProdX10, ref PercEiwitDagProdX100, ref PercVetDagProdX100,
                    ref PercLactoseX100, ref Ureum, ref MelkCelgetal,
                    ref DatumAfkalven,
                    ref KgMelkLact, ref KgEiwitLactX100, ref KgVetLactX100,
                    ref KgMelk305, ref KgEiwit305x100, ref KgVet305x100,
                    ref LactatieWaarde, ref NettoOpbrengst,
                    ref DagenInLactatie, ref BSKx10, ref IndicatieEMM, ref DierStatus, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_DekkingRund(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out String DekInfo, out String Levensnrstier,
            out DateTime DatumEindeSamenweiden)
        {
            int result = -1;
            LockObject padlock;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lDekInfo = new StringBuilder();
            lDekInfo.EnsureCapacity(pMaxStrLen);
            StringBuilder lLevensnrstier = new StringBuilder();
            lLevensnrstier.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            DatumEindeSamenweiden = DateTime.MinValue;
            DekkingRund handle = (DekkingRund)ExecuteProcedureDLLStack(typeof(DekkingRund), "edinrs_DekkingRund", GetLockList(), out padlock);

            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref Datum,
                    ref lDekInfo, ref lLevensnrstier,
                    ref DatumEindeSamenweiden, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            DekInfo = lDekInfo.ToString();
            Levensnrstier = lLevensnrstier.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_ImplantatieEmbryo(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out String LevensnrETmoeder, out String Levensnrstier)
        {
            int result = -1;
            LockObject padlock;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lLevensnrETmoeder = new StringBuilder();
            lLevensnrETmoeder.EnsureCapacity(pMaxStrLen);
            StringBuilder lLevensnrstier = new StringBuilder();
            lLevensnrstier.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            ImplantatieEmbryo handle = (ImplantatieEmbryo)ExecuteProcedureDLLStack(typeof(ImplantatieEmbryo), "edinrs_ImplantatieEmbryo", GetLockList(), out padlock);

            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref Datum,
                    ref lLevensnrETmoeder, ref lLevensnrstier, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            LevensnrETmoeder = lLevensnrETmoeder.ToString();
            Levensnrstier = lLevensnrstier.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_MutatieProduktiestatus(String pBestand, int pRegelnr,
            out String Levensnr,
            out int StatusProduktie,
            out DateTime Datum)
        {
            int result = -1;
            LockObject padlock;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StatusProduktie = -1;
            Datum = DateTime.MinValue;

            MutatieProduktiestatus handle = (MutatieProduktiestatus)ExecuteProcedureDLLStack(typeof(MutatieProduktiestatus), "edinrs_MutatieProduktiestatus", GetLockList(), out padlock);

            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref StatusProduktie,
                    ref Datum, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            Levensnr = lLevensnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_MutatieDrachtigheidsstatus(String pBestand, int pRegelnr,
            out String Levensnr,
            out int StatusDracht,
            out DateTime Datum)
        {
            int result = -1;
            LockObject padlock;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            Datum = DateTime.MinValue;
            StatusDracht = -1;
            MutatieDrachtigheidsstatus handle = (MutatieDrachtigheidsstatus)ExecuteProcedureDLLStack(typeof(MutatieDrachtigheidsstatus), "edinrs_MutatieDrachtigheidsstatus", GetLockList(), out padlock);


            try
            {
                result = handle(pBestand, pRegelnr,
                    ref lLevensnr,
                    ref StatusDracht,
                    ref Datum, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

            Levensnr = lLevensnr.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_BedrijfsgegevensMelking(String pBestand, int pRegelnr,
            out String UBNnr, out DateTime DatumProduktie,
            out DateTime TijdBeginMelking, out DateTime TijdEindMelking)
        {
            int result = -1;
            LockObject padlock;
            StringBuilder lUBNnr = new StringBuilder();
            lUBNnr.EnsureCapacity(pMaxStrLen);
            DatumProduktie = DateTime.MinValue;
            TijdBeginMelking = DateTime.MinValue;
            TijdEindMelking = DateTime.MinValue;
            BedrijfsgegevensMelking handle = (BedrijfsgegevensMelking)ExecuteProcedureDLLStack(typeof(BedrijfsgegevensMelking), "edinrs_BedrijfsgegevensMelking", GetLockList(), out padlock);
            try
            {
                result = handle(pBestand, pRegelnr,
                                ref lUBNnr, ref DatumProduktie,
                                ref TijdBeginMelking, ref TijdEindMelking, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            UBNnr = lUBNnr.ToString();

            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_BedrijfsgegevensDagproduktie(String pBestand, int pRegelnr,
            out String UBNnr, out DateTime DatumProduktie,
            out int AantKoeienBemonsterd, out int AantKoeienDroog, out int AantKoeienMelkgevend,
            out int AantMelkingenPerDag, out int CodeAfleidingDagprod,
            out DateTime DatumOnderzoekVetEiwit,
            out int KgMelkDagprod, out int KgVetDagprodX100, out int KgEiwitDagprodX100,
            out int KgMelk305gem, out int KgVet305gemX100, out int KgEiwit305gemX100,
            out int KgMelk305st, out int KgVet305stX100, out int KgEiwit305stX100,
            out int NettoOpbrengst, out int BSKx10, out int StatusDagProduktie,
            out int MPR24hLaatste, out int MPR24hJaarGem, out int IndicatieEMM)
        {
            int result = -1;
            StringBuilder lUBNnr = new StringBuilder();
            lUBNnr.EnsureCapacity(pMaxStrLen);
            DatumProduktie = DateTime.MinValue;
            AantKoeienBemonsterd = -1;
            AantKoeienDroog = -1;
            AantKoeienMelkgevend = -1;
            AantMelkingenPerDag = -1;
            CodeAfleidingDagprod = -1;
            DatumOnderzoekVetEiwit = DateTime.MinValue;
            KgMelkDagprod = -1;
            KgVetDagprodX100 = -1;
            KgEiwitDagprodX100 = -1;
            KgMelk305gem = -1;
            KgVet305gemX100 = -1;
            KgEiwit305gemX100 = -1;
            KgMelk305st = -1;
            KgVet305stX100 = -1;
            KgEiwit305stX100 = -1;
            NettoOpbrengst = -1;
            BSKx10 = -1;
            StatusDagProduktie = -1;
            MPR24hLaatste = -1;
            MPR24hJaarGem = -1;
            IndicatieEMM = -1;
            LockObject padlock;
            BedrijfsgegevensDagproduktie handle = (BedrijfsgegevensDagproduktie)ExecuteProcedureDLLStack(typeof(BedrijfsgegevensDagproduktie), "edinrs_BedrijfsgegevensDagproduktie", GetLockList(), out padlock);


            try
            {
                result = handle(pBestand, pRegelnr,
                                ref lUBNnr, ref DatumProduktie,
                                ref AantKoeienBemonsterd, ref  AantKoeienDroog, ref AantKoeienMelkgevend,
                                ref AantMelkingenPerDag, ref CodeAfleidingDagprod,
                                ref DatumOnderzoekVetEiwit,
                                ref KgMelkDagprod, ref KgVetDagprodX100, ref KgEiwitDagprodX100,
                                ref KgMelk305gem, ref KgVet305gemX100, ref KgEiwit305gemX100,
                                ref KgMelk305st, ref KgVet305stX100, ref KgEiwit305stX100,
                                ref NettoOpbrengst, ref BSKx10, ref StatusDagProduktie,
                                ref MPR24hLaatste, ref MPR24hJaarGem, ref IndicatieEMM, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

            UBNnr = lUBNnr.ToString();

            return result;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_FokwaardenExterieur(String pBestand, int pRegelnr,
            ref String Levensnr, ref DateTime Datum,
            ref int IndFWkoeDracht, ref int JaarBasis,ref int Betrouwbh,
            ref String RasBasis, ref int AchterSpeenplaatsing, ref int AchterUierhoogte, 
            ref int Benen, ref int BespieringBovenbalk,  ref int  BespieringOnderbalk,
            ref int Hoogtemaat, ref int Inhoud, ref int Klauwhoek, ref int KruisBreedte,
            ref int Kruisligging, ref int Openheid, ref int Ophangband,
            ref int Speenlengte, ref int StandAchterbenen,
            ref int AchterbeenstandAchteraanzicht,
            ref int AlgVoorkomen, ref int FrameRoodbont, ref int FrameZwartbont,
            ref int FrameMRIJ, ref int Uier, ref int UierDiepte,
            ref int Voorspeenplaatsing, ref int VoorUieraanhechting,
            ref int Voorhand, ref int Conditiescore, ref int Robustheid,
            ref int Beengebruik)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lRasBasis = new StringBuilder();
            lRasBasis.EnsureCapacity(pMaxStrLen);
      
            LockObject padlock;
            FokwaardenExterieur handle = (FokwaardenExterieur)ExecuteProcedureDLLStack(typeof(FokwaardenExterieur), "edinrs_FokwaardenExterieur", GetLockList(), out padlock);


            try
            {
                result = handle(pBestand, pRegelnr,
                        ref lLevensnr, ref Datum,
                        ref IndFWkoeDracht, ref JaarBasis,ref Betrouwbh,
                        ref lRasBasis, ref AchterSpeenplaatsing, ref AchterUierhoogte, 
                        ref Benen, ref BespieringBovenbalk,  ref BespieringOnderbalk,
                        ref Hoogtemaat, ref Inhoud, ref Klauwhoek, ref KruisBreedte,
                        ref Kruisligging, ref Openheid, ref Ophangband,
                        ref Speenlengte, ref StandAchterbenen,
                        ref AchterbeenstandAchteraanzicht,
                        ref AlgVoorkomen, ref FrameRoodbont, ref FrameZwartbont,
                        ref FrameMRIJ, ref Uier, ref UierDiepte,
                        ref Voorspeenplaatsing, ref VoorUieraanhechting,
                        ref Voorhand, ref Conditiescore, ref Robustheid,
                        ref Beengebruik, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

            Levensnr = lLevensnr.ToString();
            RasBasis = RasBasis.ToString();

            return result;
        }


        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int edinrs_FokwaardenGeboorteKenmerken(String pBestand, int pRegelnr,
            ref String Levensnr, ref DateTime Datum,
            ref int JaarBasis,ref String RasBasis, ref int AchterSpeenplaatsing, ref int AchterUierhoogte, 
            ref int Draagtijd, ref int Geboortegewicht, ref int Geboortegemak, ref int BthOverleving, ref int Overleving,
            ref int BthAfkalfgemak, ref int Afkalfgemak)
        {
            int result = -1;
            StringBuilder lLevensnr = new StringBuilder();
            lLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lRasBasis = new StringBuilder();
            lRasBasis.EnsureCapacity(pMaxStrLen);
      
            LockObject padlock;
            FokwaardenGeboorteKenmerken handle = (FokwaardenGeboorteKenmerken)ExecuteProcedureDLLStack(typeof(FokwaardenGeboorteKenmerken), "edinrs_FokwaardenGeboorteKenmerken", GetLockList(), out padlock);


            try
            {
                result = handle(pBestand, pRegelnr,
                        ref lLevensnr, ref Datum,
                        ref JaarBasis,
                        ref lRasBasis, ref AchterSpeenplaatsing, ref AchterUierhoogte, 
                        ref Draagtijd, ref Geboortegewicht, ref Geboortegemak, ref BthOverleving, ref Overleving,
                        ref BthAfkalfgemak, ref Afkalfgemak, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

            Levensnr = lLevensnr.ToString();
            RasBasis = RasBasis.ToString();

            return result;
        }

        /// ///////////////////////////////////////////
   

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int LeesIRretour_Terugmelding(String pBestand, int pRegelnr,
            out String UBNnr, out String Levensnr, out String Koenummer, out String Naam, out String Geslacht,
            out String Haarkleur, out String LevensnrMoeder, out String CodeOpvragenNaam,
            out DateTime DatumMutatie,
            out int SoortMutatie, out int BijzonderHeden,
            out int IndVeeverbetering,
            out DateTime DatumMelding, out DateTime TijdMelding,
            out DateTime DatumVerwerking, out DateTime TijdVerwerking,
            out int NummerBron,
            out String Herkomst, out String BerichtID, out String VersienrBerichtType,
            out String ReleasenrBerichtType, out String ZenderID, out String TypeMedium,
            out String CodeVerwerking, out String CodeResultaatVerwerking,
            out String OmschrijvingResultaat, out String IKBrund,
            out String IRBlokkadeRund)
        {
            int result = -1;
            StringBuilder lUBNnr = new StringBuilder();
            StringBuilder lLevensnr = new StringBuilder();
            StringBuilder lKoenummer = new StringBuilder();
            StringBuilder lNaam = new StringBuilder();
            StringBuilder lGeslacht = new StringBuilder();
            StringBuilder lHaarkleur = new StringBuilder();
            StringBuilder lLevensnrMoeder = new StringBuilder();
            StringBuilder lCodeOpvragenNaam = new StringBuilder();
            StringBuilder lHerkomst = new StringBuilder();
            StringBuilder lBerichtID = new StringBuilder();
            StringBuilder lVersienrBerichtType = new StringBuilder();
            StringBuilder lReleasenrBerichtType = new StringBuilder();
            StringBuilder lZenderID = new StringBuilder();
            StringBuilder lTypeMedium = new StringBuilder();
            StringBuilder lCodeVerwerking = new StringBuilder();
            StringBuilder lCodeResultaatVerwerking = new StringBuilder();
            StringBuilder lOmschrijvingResultaat = new StringBuilder();
            StringBuilder lIKBrund = new StringBuilder();
            StringBuilder lIRBlokkadeRund = new StringBuilder();
            LockObject padlock;
            IRretour_Terugmelding handle = (IRretour_Terugmelding)ExecuteProcedureDLLStack(typeof(IRretour_Terugmelding), "IRretour_Terugmelding", GetLockList(), out padlock);
            try
            {

                DatumMutatie = DateTime.MinValue;
                SoortMutatie = -1;
                BijzonderHeden = -1;
                IndVeeverbetering = -1;
                DatumMelding = DateTime.MinValue;
                TijdMelding = DateTime.MinValue;
                DatumVerwerking = DateTime.MinValue;
                TijdVerwerking = DateTime.MinValue;
                NummerBron = -1;

                result = handle(pBestand, pRegelnr,
                            ref lUBNnr, ref lLevensnr, ref lKoenummer, ref lNaam, ref lGeslacht,
                            ref lHaarkleur, ref lLevensnrMoeder, ref lCodeOpvragenNaam,
                            ref DatumMutatie,
                            ref SoortMutatie, ref BijzonderHeden,
                            ref IndVeeverbetering,
                            ref DatumMelding, ref TijdMelding,
                            ref DatumVerwerking, ref TijdVerwerking,
                            ref NummerBron,
                            ref lHerkomst, ref lBerichtID, ref lVersienrBerichtType,
                            ref lReleasenrBerichtType, ref lZenderID, ref lTypeMedium,
                            ref lCodeVerwerking, ref lCodeResultaatVerwerking,
                            ref lOmschrijvingResultaat, ref lIKBrund,
                            ref lIRBlokkadeRund, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }

            UBNnr = lUBNnr.ToString();
            Levensnr = lLevensnr.ToString();
            Koenummer = lKoenummer.ToString();
            Naam = lNaam.ToString();
            Geslacht = lGeslacht.ToString();
            Haarkleur = lHaarkleur.ToString();
            LevensnrMoeder = lLevensnrMoeder.ToString();
            CodeOpvragenNaam = lCodeOpvragenNaam.ToString();
            Herkomst = lHerkomst.ToString();
            BerichtID = lBerichtID.ToString();
            VersienrBerichtType = lVersienrBerichtType.ToString();
            ReleasenrBerichtType = lReleasenrBerichtType.ToString();
            ZenderID = lZenderID.ToString();
            TypeMedium = lTypeMedium.ToString();
            CodeVerwerking = lCodeVerwerking.ToString();
            CodeResultaatVerwerking = lCodeResultaatVerwerking.ToString();
            OmschrijvingResultaat = lOmschrijvingResultaat.ToString();
            IKBrund = lIKBrund.ToString();
            IRBlokkadeRund = lIRBlokkadeRund.ToString();
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int LeeswebsterHT2data(String pBestand, int pRegelnr,
            out String UBNnr, out int SoortMutatie,
            out DateTime DatumMutatie, out DateTime TijdMutatie,
            out String Levensnummer, out DateTime Geboortedatum,
            out String Geslacht, out String Haarkleur, out String LevnrMoeder)
        {
            int result = -1;
            StringBuilder lUBNnr = new StringBuilder();
            StringBuilder lLevensnummer = new StringBuilder();
            StringBuilder lGeslacht = new StringBuilder();
            StringBuilder lHaarkleur = new StringBuilder();
            StringBuilder lLevnrMoeder = new StringBuilder();
            LockObject padlock;
            websterHT2data handle = (websterHT2data)ExecuteProcedureDLLStack(typeof(websterHT2data), "websterHT2data", GetLockList(), out padlock);
            try
            {
                SoortMutatie = -1;
                DatumMutatie = DateTime.MinValue;
                TijdMutatie = DateTime.MinValue;
                Geboortedatum = DateTime.MinValue;

                result = handle(pBestand, pRegelnr,
                ref lUBNnr, ref SoortMutatie,
                ref DatumMutatie, ref TijdMutatie,
                ref lLevensnummer, ref Geboortedatum,
                ref lGeslacht, ref lHaarkleur, ref lLevnrMoeder, pMaxStrLen);
                FreeDLL(padlock.DLLname);
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
            UBNnr = lUBNnr.ToString();
            Levensnummer = lLevensnummer.ToString();
            Geslacht = lGeslacht.ToString();
            Haarkleur = lHaarkleur.ToString();
            LevnrMoeder = lLevnrMoeder.ToString();
            return result;
        }

    }
}
