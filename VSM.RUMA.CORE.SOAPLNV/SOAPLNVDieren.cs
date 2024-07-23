using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using VSM.RUMA.CORE.SOAPLNV.DierenWS;

namespace VSM.RUMA.CORE.SOAPLNV
{
    public class SOAPLNVDieren
    {

        public SOAPLNVDieren()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public int? Timeout
        {
            get;
            set;
        }


        public class Dierverblijfplaats
        {
            public DateTime AanvoerDatum
            {
                get;
                set;
            }
            public DateTime AfvoerDatum
            {
                get;
                set;
            }
            public String UBN
            {
                get;
                set;
            }

            public String Postcode
            {
                get;
                set;
            }

            public String Bedrijfstype
            {
                get;
                set;
            }

        }

        public class Diernakomelingen
        {
            public string Werknummer { get; internal set; }
            public DateTime? geboorteDatum { get; internal set; }
            public int? dierSoort { get; internal set; }
            public string Haarkleur { get; internal set; }
            public int? Geslacht { get; internal set; }
            public string Levensnummer { get; internal set; }
        }

        #region overloadLNVDierdetailsV2

        public bool LNVDierdetailsV2(
     string pUsername, string pPassword,
     int pTestServer, string UBNnr, string BRSnr, string Levensnr,
     int pPrognr, int ophVerblijfplaatsen,
     int ophVlaggen, int ophNakomelingen,
     ref string Werknummer,
     ref DateTime Geboortedat, ref DateTime Importdat,
     ref string LandCodeHerkomst, ref string LandCodeOorsprong,
     ref string Geslacht, ref string Haarkleur,
     ref DateTime Einddatum, ref string RedenEinde,
     ref string LevensnrMoeder,
     ref string VervangenLevensnr,
     ref string Status, ref string Code, ref string Omschrijving)
        {
            List<Dierverblijfplaats> Verblijfplaatsen = new List<Dierverblijfplaats>();
            int LNVPrognr = 0;
            return LNVDierdetailsV2(pUsername, pPassword,
                pTestServer,
                UBNnr, BRSnr,
                Levensnr,
                pPrognr, ophVerblijfplaatsen,
                ophVlaggen, ophNakomelingen,
                ref LNVPrognr,
                ref Werknummer,
                ref Geboortedat, ref Importdat,
                ref LandCodeHerkomst, ref LandCodeOorsprong,
                ref Geslacht, ref Haarkleur,
                ref Einddatum, ref RedenEinde,
                ref LevensnrMoeder,
                ref VervangenLevensnr,
                ref Verblijfplaatsen,
                ref Status, ref Code, ref Omschrijving);

        }


        public bool LNVDierdetailsV2(
     string pUsername, string pPassword,
     int pTestServer, string UBNnr, string BRSnr, string Levensnr,
     int pPrognr, int ophVerblijfplaatsen,
     int ophVlaggen, int ophNakomelingen,
     ref string Werknummer,
     ref DateTime Geboortedat, ref DateTime Importdat,
     ref string LandCodeHerkomst, ref string LandCodeOorsprong,
     ref string Geslacht, ref string Haarkleur,
     ref DateTime Einddatum, ref string RedenEinde,
     ref string LevensnrMoeder,
     ref string VervangenLevensnr,
     ref List<Dierverblijfplaats> Verblijfplaatsen,
     ref string Status, ref string Code, ref string Omschrijving)
        {
            int LNVPrognr = 0;
            return LNVDierdetailsV2(pUsername, pPassword,
                            pTestServer,
                            UBNnr, BRSnr,
                            Levensnr,
                            pPrognr, ophVerblijfplaatsen,
                            ophVlaggen, ophNakomelingen,
                            ref LNVPrognr,
                            ref Werknummer,
                            ref Geboortedat, ref Importdat,
                            ref LandCodeHerkomst, ref LandCodeOorsprong,
                            ref Geslacht, ref Haarkleur,
                            ref Einddatum, ref RedenEinde,
                            ref LevensnrMoeder,
                            ref VervangenLevensnr,
                            ref Verblijfplaatsen,
                            ref Status, ref Code, ref Omschrijving);
        }

        #endregion




        public bool LNVDierdetailsV2(
             string pUsername, string pPassword,
             int pTestServer, string UBNnr, string BRSnr, string Levensnr,
             int pPrognr, int ophVerblijfplaatsen,
             int ophVlaggen, int ophNakomelingen,
             ref int LNVPrognr,
             ref string Werknummer,
             ref DateTime Geboortedat, ref DateTime Importdat,
             ref string LandCodeHerkomst, ref string LandCodeOorsprong,
             ref string Geslacht, ref string Haarkleur,
             ref DateTime Einddatum, ref string RedenEinde,
             ref string LevensnrMoeder,
             ref string VervangenLevensnr,
             ref List<Dierverblijfplaats> Verblijfplaatsen,
             ref string Status, ref string Code, ref string Omschrijving)
        {

            Status = "F";
            Omschrijving = "Geen reactie";

            dierDetailsType result;
            List<dierDetailsVerblijfplaatsenType> lnvVerblijfplaatsen;
            List<dierDetailsVlaggenType> lnvDierVlaggen;
            List<dierDetailsNakomelingenType> lnvNakomelingen;
            verwerkingsresultaatType Resultaat;

            bool ret = raadplegenDierDetails(pUsername, pPassword,
                                      pTestServer,
                                      UBNnr, BRSnr,
                                      Levensnr, pPrognr,
                                      ophVerblijfplaatsen, ophVlaggen, ophNakomelingen,
                                      out result, out lnvVerblijfplaatsen, out lnvDierVlaggen, out lnvNakomelingen, out Resultaat);

            if (ret)
            {
                Status = "G";
                if (result.dierSoort.HasValue)
                    LNVPrognr = LNVdiersoortToPrognr(result.dierSoort.Value);
                Werknummer = result.dierWerknummer;
                Geboortedat = Convert.ToDateTime(result.geboorteDatum);
                Importdat = Convert.ToDateTime(result.importDatum);
                LandCodeHerkomst = result.dierHerkomstLandcode;
                LandCodeOorsprong = result.dierOorsprongLandcode;
                Geslacht = result.dierGeslacht;
                Haarkleur = result.dierHaarkleur;
                Einddatum = Convert.ToDateTime(result.dierEinddatum);
                RedenEinde = result.dierRedenEinde;
                LevensnrMoeder = (result.moederLandcode + " " + result.moederLevensnummer).Trim();
                VervangenLevensnr = (result.dierVervangenLandcode + " " + result.dierVervangenLevensnummer).Trim();
                Dierverblijfplaats movement;
                if (lnvVerblijfplaatsen != null)
                {
                    foreach (dierDetailsVerblijfplaatsenType Verblijfplaats in lnvVerblijfplaatsen)
                    {
                        movement = new Dierverblijfplaats();
                        movement.AanvoerDatum = Convert.ToDateTime(Verblijfplaats.aanvoerDatumME);
                        movement.UBN = removevoorloopnullen(Verblijfplaats.meldingeenheid);
                        movement.AfvoerDatum = Convert.ToDateTime(Verblijfplaats.afvoerDatumME);
                        movement.Bedrijfstype = Verblijfplaats.typeBedrijfsvestiging;
                        movement.Postcode = Verblijfplaats.postcodeOmsME;
                        Verblijfplaatsen.Add(movement);
                    }
                }
            }
            if (Resultaat.soortFoutIndicator != String.Empty && Resultaat.soortFoutIndicator != null)
                Status = Resultaat.soortFoutIndicator;
            Code = Resultaat.foutcode;
            Omschrijving = Resultaat.foutmelding;

            return ret;
        }


        public bool LNVDierdetailsV2(
     string pUsername, string pPassword,
     int pTestServer, string UBNnr, string BRSnr, string Levensnr,
     int pPrognr, int ophVerblijfplaatsen,
     int ophVlaggen, int ophNakomelingen,
     ref int LNVPrognr,
     ref string Werknummer,
     ref DateTime Geboortedat, ref DateTime Importdat,
     ref string LandCodeHerkomst, ref string LandCodeOorsprong,
     ref string Geslacht, ref string Haarkleur,
     ref DateTime Einddatum, ref string RedenEinde,
     ref string LevensnrMoeder,
     ref string VervangenLevensnr,
     ref List<Diernakomelingen> Nakomelingen,
     ref string Status, ref string Code, ref string Omschrijving)
        {

            Status = "F";
            Omschrijving = "Geen reactie";

            dierDetailsType result;
            List<dierDetailsVerblijfplaatsenType> lnvVerblijfplaatsen;
            List<dierDetailsVlaggenType> lnvDierVlaggen;
            List<dierDetailsNakomelingenType> lnvNakomelingen;
            verwerkingsresultaatType Resultaat;

            bool ret = raadplegenDierDetails(pUsername, pPassword,
                                      pTestServer,
                                      UBNnr, BRSnr,
                                      Levensnr, pPrognr,
                                      ophVerblijfplaatsen, ophVlaggen, ophNakomelingen,
                                      out result, out lnvVerblijfplaatsen, out lnvDierVlaggen, out lnvNakomelingen, out Resultaat);

            if (ret)
            {
                Status = "G";
                if (result.dierSoort.HasValue)
                    LNVPrognr = LNVdiersoortToPrognr(result.dierSoort.Value);
                Werknummer = result.dierWerknummer;
                Geboortedat = Convert.ToDateTime(result.geboorteDatum);
                Importdat = Convert.ToDateTime(result.importDatum);
                LandCodeHerkomst = result.dierHerkomstLandcode;
                LandCodeOorsprong = result.dierOorsprongLandcode;
                Geslacht = result.dierGeslacht;
                Haarkleur = result.dierHaarkleur;
                Einddatum = Convert.ToDateTime(result.dierEinddatum);
                RedenEinde = result.dierRedenEinde;
                LevensnrMoeder = (result.moederLandcode + " " + result.moederLevensnummer).Trim();
                VervangenLevensnr = (result.dierVervangenLandcode + " " + result.dierVervangenLevensnummer).Trim();
                Diernakomelingen nakomeling;
                if (lnvNakomelingen != null)
                {
                    foreach (dierDetailsNakomelingenType child in lnvNakomelingen)
                    {
                        nakomeling = new Diernakomelingen();
                        nakomeling.Levensnummer = child.dierLandcode + " " + child.dierLevensnummer;
                        nakomeling.Geslacht = child.dierGeslacht.ToUpper() == "V" ? 2 : 1;
                        nakomeling.Haarkleur = child.dierHaarkleur;
                        nakomeling.dierSoort = child.dierSoort;
                        nakomeling.Werknummer = child.dierWerknummer;
                        nakomeling.geboorteDatum = Convert.ToDateTime(child.geboorteDatum);
                    
                        Nakomelingen.Add(nakomeling);
                    }
                }
            }
            if (Resultaat.soortFoutIndicator != String.Empty && Resultaat.soortFoutIndicator != null)
                Status = Resultaat.soortFoutIndicator;
            Code = Resultaat.foutcode;
            Omschrijving = Resultaat.foutmelding;

            return ret;
        }



        public void LNVDierDetailstatusV2(
                string pUsername, string pPassword,
                int pTestServer,
                string Levensnr, int pPrognr,
                ref string BRSnrHouder, ref string UBNhouder,
                ref string Werknummer,
                ref DateTime Geboortedat, ref DateTime Importdat,
                ref string LandCodeHerkomst, ref string LandCodeOorsprong,
                ref string Geslacht, ref string Haarkleur,
                ref DateTime Einddatum, ref string RedenEinde,
                ref string LevensnrMoeder,
                ref string Status, ref string Code, ref string Omschrijving)
        {

            Status = "F";
            Omschrijving = "Geen reactie";

            dierDetailsType result;
            List<dierDetailsVerblijfplaatsenType> Verblijfplaatsen;
            List<dierDetailsVlaggenType> DierVlaggen;
            List<dierDetailsNakomelingenType> Nakomelingen;
            verwerkingsresultaatType Resultaat;
            if (raadplegenDierDetails(pUsername, pPassword,
                                      pTestServer,
                                      UBNhouder, String.Empty,
                                      Levensnr, pPrognr,
                                      1, 0, 0,
                                      out result, out Verblijfplaatsen, out DierVlaggen, out Nakomelingen, out Resultaat))
            {
                Status = "G";
                Werknummer = result.dierWerknummer;
                Geboortedat = Convert.ToDateTime(result.geboorteDatum);
                Importdat = Convert.ToDateTime(result.importDatum);
                LandCodeHerkomst = result.dierHerkomstLandcode;
                LandCodeOorsprong = result.dierOorsprongLandcode;
                Geslacht = result.dierGeslacht;
                Haarkleur = result.dierHaarkleur;
                Einddatum = Convert.ToDateTime(result.dierEinddatum);
                RedenEinde = result.dierRedenEinde;
                if (Verblijfplaatsen != null)
                {
                    var VerblijfplaatsenZonderAfvoer = Verblijfplaatsen.Where(vrbpl => vrbpl.afvoerDatumME == null);
                    if (VerblijfplaatsenZonderAfvoer.Count() > 0)
                    {
                        dierDetailsVerblijfplaatsenType Verblijfplaats = VerblijfplaatsenZonderAfvoer.First();
                        UBNhouder = removevoorloopnullen(Verblijfplaats.meldingeenheid);
                    }
                }
                LevensnrMoeder = (result.moederLandcode + " " + result.moederLevensnummer).Trim();
            }
            if (Resultaat.soortFoutIndicator != String.Empty && Resultaat.soortFoutIndicator != null)
                Status = Resultaat.soortFoutIndicator;
            Code = Resultaat.foutcode;
            Omschrijving = Resultaat.foutmelding;
        }


        [Obsolete("Niet meer ondersteund door RVO", true)]
        private void LNVDierstatusV2(
                string pUsername, string pPassword,
                int pTestServer,
                string Levensnr, int pPrognr,
                ref string BRSnrHouder, ref string UBNhouder, ref string Werknummer,
                ref DateTime Geboortedat, ref DateTime Importdat,
                ref string LandCodeHerkomst, ref string LandCodeOorsprong,
                ref string Geslacht, ref string Haarkleur,
                ref DateTime Einddatum, ref string RedenEinde,
                ref string LevensnrMoeder,
                ref string Status, ref string Code, ref string Omschrijving, string pLogFile, int pMaxStrLen)
        {
            Status = "F";
            Omschrijving = "Geen reactie";
            dierStatusType result;
            verwerkingsresultaatType Resultaat;
            if (raadplegenDierStatus(pUsername, pPassword,
                                     pTestServer,
                                     Levensnr, pPrognr,
                                     out result, out Resultaat))
            {
                Status = "G";
                BRSnrHouder = result.relatienummerHouder;
                UBNhouder = removevoorloopnullen(result.meldingeenheid);
                Werknummer = result.dierWerknummer;
                Geboortedat = Convert.ToDateTime(result.geboorteDatum);
                Importdat = Convert.ToDateTime(result.importDatum);
                LandCodeHerkomst = result.dierHerkomstLandcode;
                LandCodeOorsprong = result.dierOorsprongLandcode;
                Geslacht = result.dierGeslacht;
                LevensnrMoeder = (result.moederLandcode + " " + result.moederLevensnummer).Trim();
            }
            if (Resultaat.soortFoutIndicator != String.Empty && Resultaat.soortFoutIndicator != null)
                Status = Resultaat.soortFoutIndicator;
            Code = Resultaat.foutcode;
            Omschrijving = Resultaat.foutmelding;

        }

        private static int PrognrToLNVdiersoort(int prog)
        {
            if (prog == 3) // schapen
                return 3;
            else if (prog == 5) // geiten
                return 4;
            else // rundvee
                return 1;
        }

        private static int LNVdiersoortToPrognr(int Diersoort)
        {
            if (Diersoort == 4) // geiten
                return 5;
            else return Diersoort;
        }

        private static string removevoorloopnullen(string p)
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

        private bool raadplegenDierDetails(
            String pUsername, String pPassword, int pTestServer,
            string UBNnr, string BRSnr, string Levensnr, int pPrognr, int ophVerblijfplaatsen, int ophVlaggen, int ophNakomelingen,
            out dierDetailsType dierDetails,
            out List<dierDetailsVerblijfplaatsenType> Verblijfplaatsen,
            out List<dierDetailsVlaggenType> Vlaggen,
            out List<dierDetailsNakomelingenType> Nakomelingen,
            out verwerkingsresultaatType Resultaat)
        {
            string fn = $"{nameof(SOAPLNVDieren)}.{nameof(raadplegenDierDetails)}";
            string lPrefix = $"{fn} Dier: {Levensnr} -";

            try
            {
           
                DierenServiceService webservice = new DierenServiceService();

                if (pUsername == "1293561")
                    webservice.PreAuthenticate = true;

                webservice.Credentials = new System.Net.NetworkCredential(pUsername, pPassword);
                if (Timeout.HasValue)
                    webservice.Timeout = Timeout.Value;

                if (pTestServer == 0)
                    webservice.Url = "https://dbrbms.agro.nl/osbbms_v2_0/services/DierenWS";
                if (pTestServer == 1)
                    webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air2/services/DierenWS";
                if (pTestServer == 2)
                    webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air1/services/DierenWS";


                dierDetailsRequestType req = new dierDetailsRequestType();

                req.requestID = $"VSM{Thread.CurrentThread.ManagedThreadId}";

                if (!string.IsNullOrWhiteSpace(UBNnr))
                {
                    // kan/mag leeg zijn
                    req.selMeldingeenheid = UBNnr;
                }

                if (!string.IsNullOrWhiteSpace(BRSnr))
                {
                    // kan/mag leeg zijn
                    req.selRelatienummerHouder = BRSnr;
                }

                req.selDierLandcode = Levensnr.Substring(0, 3).Trim();
                req.selDierLevensnummer = Levensnr.Substring(3).Trim();

                // selDierWerknummer
                req.selDierSoort = PrognrToLNVdiersoort(pPrognr);

                req.indVerblijfplaatsen = ophVerblijfplaatsen == 1 ? "J" : "N";

                req.indVlaggen = ophVlaggen == 1 ? "J" : "N";

                req.indNakomelingen = ophNakomelingen == 1 ? "J" : "N";
                string tijd = DateTime.Now.Ticks.ToString();
                IRUtils.writerequest<dierDetailsRequestType>(UBNnr, webservice.Url, tijd, req);
                dierDetailsResponseType resp = webservice.raadplegenDierDetails(req);
                IRUtils.writerequest<dierDetailsResponseType>(UBNnr, webservice.Url, tijd, resp);
                dierDetails = resp.dierDetails;
                Resultaat = resp.verwerkingsresultaat;

                if (resp.dierDetailsVerblijfplaatsen != null)
                    Verblijfplaatsen = resp.dierDetailsVerblijfplaatsen.ToList();
                else
                    Verblijfplaatsen = null;

                if (resp.dierVlaggen != null)
                    Vlaggen = resp.dierVlaggen.ToList();
                else
                    Vlaggen = null;

                if (resp.dierNakomelingen != null)
                    Nakomelingen = resp.dierNakomelingen.ToList();
                else
                    Nakomelingen = null;

                return resp.verwerkingsresultaat.succesIndicator == "J";

            }
            catch (System.IO.IOException ex)
            {
                dierDetails = null;
                Resultaat = new verwerkingsresultaatType();
                Resultaat.soortFoutIndicator = "F";
                Resultaat.foutmelding = ex.Message;
                Verblijfplaatsen = null;
                Vlaggen = null;
                Nakomelingen = null;

                unLogger.WriteError($"{lPrefix} IO Ex: {ex.Message}");
                unLogger.WriteDebug($"{lPrefix} IO Ex: {ex.Message}", ex);

                return false;
            }
            catch (System.Net.WebException ex)
            {
                dierDetails = null;
                Resultaat = new verwerkingsresultaatType();
                Resultaat.soortFoutIndicator = "F";
                if (ex.Response is System.Net.HttpWebResponse)
                {
                    Resultaat.foutcode = String.Format("HTTP{0}", (int)(ex.Response as System.Net.HttpWebResponse).StatusCode);
                }
                Resultaat.foutmelding = ex.Message;
                Verblijfplaatsen = null;
                Vlaggen = null;
                Nakomelingen = null;

                if (Resultaat.foutcode == "401")
                {
                    string msg = $"{lPrefix} 401: Unauthorized.";

                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);
                }
                else
                {
                    string msg = $"{lPrefix} Web Ex: {ex.Message}";

                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);
                }

                return false;
            }
            catch (Exception ex)
            {
                dierDetails = null;
                Resultaat = new verwerkingsresultaatType();
                Resultaat.soortFoutIndicator = "F";
                Resultaat.foutmelding = ex.Message;
                Verblijfplaatsen = null;
                Vlaggen = null;
                Nakomelingen = null;

                unLogger.WriteError($"{lPrefix} Ex: " + ex.Message, ex);

                return false; ;
            }
        }


        private bool raadplegenDierStatus(
            String pUsername, String pPassword, int pTestServer, string Levensnr, int pPrognr,
            out dierStatusType dierStatus, out verwerkingsresultaatType Resultaat)
        {
            try
            {
                DierenServiceService webservice = new DierenServiceService();
                if (pUsername == "1293561")
                    webservice.PreAuthenticate = true;
                webservice.Credentials = new System.Net.NetworkCredential(pUsername, pPassword);
                if (Timeout.HasValue)
                    webservice.Timeout = Timeout.Value;
                if (pTestServer == 1) webservice.Url = "https://dbrbms-acc.agro.nl/bms20_air2/services/DierenWS";
                if (pTestServer == 2) webservice.Url = "https://dbrbms-acc.agro.nl/bms20_air1/services/DierenWS";
                dierStatusRequestType req = new dierStatusRequestType();
                req.requestID = String.Format("VSM_D{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                req.selDierLandcode = Levensnr.Substring(0, 3).Trim();
                req.selDierLevensnummer = Levensnr.Substring(3).Trim();
                req.selDierSoort = PrognrToLNVdiersoort(pPrognr);

                string tijd = DateTime.Now.Ticks.ToString();
                IRUtils.writerequest<dierStatusRequestType>(pUsername, webservice.Url, tijd, req);
                dierStatusResponseType resp = webservice.raadplegenDierStatus(req);
                IRUtils.writerequest<dierStatusResponseType>(pUsername, webservice.Url, tijd, resp);
                dierStatus = resp.dierStatus;
                Resultaat = resp.verwerkingsresultaat;
                if (resp.verwerkingsresultaat.succesIndicator == "J") return true;
                else return false;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(ex.Message, ex);
                dierStatus = new dierStatusType();
                Resultaat = new verwerkingsresultaatType();
                return false; ;
            }
        }
    }


}
