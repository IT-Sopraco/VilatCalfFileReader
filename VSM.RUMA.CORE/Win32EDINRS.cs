using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VSM.RUMA.CORE
{
    class Win32EDINRS
    {
        public delegate void pCallback(int PercDone, string Msg);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_Bedrijfsgegevens(String pBestand, out String Land, out String UBNnr,
                      out String Naam, out String Voorvoegsel, out String Straat, out String Huisnummer,
                      out String Postcode, out String Woonplaats, out String Telefoonnr);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_RegistratieRund(String pBestand, int pRegelnr,
            out String Levensnr, out String Naam, out String Geslacht, out String RegistratieSoort,
            out DateTime geboortedat, out DateTime importdat,
            out String UBNfokker, out String Haarkleur, out String LandHerkomst,
            out String LevnrMoeder, out String LevnrVader,
            out int Draagtijd, out int CodeRelatieMetBedrijf);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_KIstier(String pBestand, int pRegelnr,
            out String Levensnr, out String KIcode,
            out String NaamAfgekort, out String NaamEigenaar);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_AfvoerRund(String pBestand, int pRegelnr,
            out String Levensnr, out String UBNnr,
            out DateTime Datum,
            out int CodeAfvoer, out int CodeRedenAfvoer);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_RegistratieRundOpBedrijf(String pBestand, int pRegelnr,
            out String Levensnr, out String UBNnr,
            out int groepsnr, out int koenr);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_AanvoerRund(String pBestand, int pRegelnr,
            out String Levensnr, out String UBNnr,
            out DateTime Datum,
            out int CodeAanvoer);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_RegistratieRas(String pBestand, int pRegelnr,
            out String Levensnr, out String CodeRas,
            out int RasDeel);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_AfkalvingKoe(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out int AantalKalveren, out int Pariteit);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_LactatieProduktie(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime DatumAfkalven,
            out int KgMelkLact, out int KgEiwitLactX100, out int KgVetLactX100,
            out int KgMelk305, out int KgEiwit305x100, out int KgVet305x100,
            out int LactatieWaarde, out int NettoOpbrengst,
            out int DagenInLactatie, out int BSKx10, out int IndicatieEMM, out int DierStatus);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_DagProduktie(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out int KgMelkDagProdX10, out int PercEiwitDagProdX100, out int PercVetDagProdX100,
            out int PercLactoseX100, out int Ureum, out int MelkCelgetal, out int IndicatieEMM);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_DagLactatieProduktie(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out int KgMelkDagProdX10, out int PercEiwitDagProdX100, out int PercVetDagProdX100,
            out int PercLactoseX100, out int Ureum, out int MelkCelgetal,
            out DateTime DatumAfkalven,
            out int KgMelkLact, out int KgEiwitLactX100, out int KgVetLactX100,
            out int KgMelk305, out int KgEiwit305x100, out int KgVet305x100,
            out int LactatieWaarde, out int NettoOpbrengst,
            out int DagenInLactatie, out int BSKx10, out int IndicatieEMM, out int DierStatus);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_DekkingRund(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out String DekInfo, out String Levensnrstier,
            out DateTime DatumEindeSamenweiden);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_ImplantatieEmbryo(String pBestand, int pRegelnr,
            out String Levensnr,
            out DateTime Datum,
            out String LevensnrETmoeder, out String Levensnrstier);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_MutatieProduktiestatus(String pBestand, int pRegelnr,
            out String Levensnr,
            out int StatusProduktie,
            out DateTime Datum);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_MutatieDrachtigheidsstatus(String pBestand, int pRegelnr,
            out String Levensnr,
            out int StatusDracht,
            out DateTime Datum);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_BedrijfsgegevensMelking(String pBestand, int pRegelnr,
            out string UBNnr, out DateTime DatumProduktie,
            out DateTime TijdBeginMelking, out DateTime TijdEindMelking);


        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int edinrs_BedrijfsgegevensDagproduktie(String pBestand, int pRegelnr,
            out String UBNnr, out DateTime DatumProduktie,
            out int AantKoeienBemonsterd, out int AantKoeienDroog, out int AantKoeienMelkgevend,
            out int AantMelkingenPerDag, out int CodeAfleidingDagprod,
            out DateTime DatumOnderzoekVetEiwit,
            out int KgMelkDagprod, out int KgVetDagprodX100, out int KgEiwitDagprodX100,
            out int KgMelk305gem, out int KgVet305gemX100, out int KgEiwit305gemX100,
            out int KgMelk305st, out int KgVet305stX100, out int KgEiwit305stX100,
            out int NettoOpbrengst, out int BSKx10, out int StatusDagProduktie,
            out int MPR24hLaatste, out int MPR24hJaarGem, out int IndicatieEMM);


        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int IRretour_Terugmelding(String pBestand, int pRegelnr,
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
            out String IRBlokkadeRund);

        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int websterHT2data(String pBestand, int pRegelnr,
            out String UBNnr, out int SoortMutatie,
            out DateTime DatumMutatie, out DateTime TijdMutatie,
            out String Levensnummer, out DateTime Geboortedatum,
            out String Geslacht, out String Haarkleur, out String LevnrMoeder);

        /*
         function AB_leesEldaMelkcontrole(
                pProgID, pProgramID: integer;
                pUbnnr, pHostName, pUserName, pPassword, pLog: PAnsiChar;
                pCallback: Pointer; pMPRdatum: TDateTime;
	            pSoortDiernr: integer;
                pInputFile: PAnsiChar): Integer; stdcall;

         */
        [DllImport("IMPEDINRS.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int AB_leesEldaMelkcontrole(int pProgID,int pProgramID,
            String UBNnr, out int SoortMutatie,
            out DateTime DatumMutatie, out DateTime TijdMutatie,
            out String Levensnummer, out DateTime Geboortedatum,
            out String Geslacht, out String Haarkleur, out String LevnrMoeder);
    }
}
