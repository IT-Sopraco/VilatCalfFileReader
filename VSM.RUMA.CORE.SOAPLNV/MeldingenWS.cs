using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSM.RUMA.CORE.SOAPLNV.srMeldingenWS;
using VSM.RUMA.CORE.DB.DataTypes;
namespace VSM.RUMA.CORE.SOAPLNV
{
    public class MeldingenWS
    {

        public MeldingenWS()
        {
            //Kan geen veilig kanaal tot stand brengen voor SSL/TLS met autoriteit dbrbms-acc.agro.nl.
            //onderstaande 2 regels toevoegen bij optreden van deze fout.
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private const int TIMEOUT_SEC = 30;
        public void raadplegenMeldingenAlg(String pUsername, String pPassword, int pTestServer, String ubn, string brsnummer,
            int prognr, DateTime begindatum, DateTime einddatum, string outputfile, string logfile, ref string status, ref string code, ref string omschrijving)
        {
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            status = "G";
            code = "";
            omschrijving = "";
            string fn = $"{nameof(SOAPLNVDieren)}.{nameof(raadplegenMeldingenAlg)}:UBN:{ubn}:Brs:{brsnummer}";
            /*
             * in de App.config or WebConfig
                   <basicHttpsBinding>
                    <binding name="MeldingenWSSoapBinding"  >
                      <security mode="Transport">
                        <transport clientCredentialType="Basic" />
                      </security>
                    </binding>
                  </basicHttpsBinding>

                  <endpoint address="https://dbrbms.agro.nl/osbbms_v2_0/services/MeldingenWS"
                        binding="basicHttpsBinding" bindingConfiguration="MeldingenWSSoapBinding"
                        contract="srMeldingenWS.MeldingenService" name="MeldingenWSSoapBinding" >
                  </endpoint>

                 en dan code: MeldingenServiceClient cl = new MeldingenServiceClient("MeldingenWSSoapBinding");
             */
            //of in code:
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.MaxReceivedMessageSize = 6553600;
            EndpointAddress adr = new EndpointAddress(IRUtils.GetMeldingWSEndpoint(pTestServer));

            MeldingenServiceClient cl = new MeldingenServiceClient(binding, adr);
            cl.ClientCredentials.UserName.UserName = pUsername;
            cl.ClientCredentials.UserName.Password = pPassword;


            var rq = new raadplegenMeldingAlgRequestType();
            rq.requestID = $@"1"; // $"VSM{Thread.CurrentThread.ManagedThreadId}"; // of "1"
            //rq.gebeurtenisBegindatum = begindatum.ToString("dd-MM-yyyy");
            //rq.gebeurtenisEinddatum = einddatum.ToString("dd-MM-yyyy");
            rq.berichttypegegevens = new berichttypegegevensType();//verplicht, maar niets invullen

            if (!string.IsNullOrWhiteSpace(ubn))
            {
                // kan/mag leeg zijn
                rq.meldingeenheid = ubn;
            }

            if (!string.IsNullOrWhiteSpace(brsnummer))
            {
                // kan/mag leeg zijn
                rq.relatienummerHouder = brsnummer;
            }
            //rq.aantal =  ? ;
            //rq.indAlleenHerstelbaar = "N"; fout niet gemachtigd
            rq.indAndereMeldingen = "N";
            rq.indEigenMeldingen = "J";
            rq.indHerstelInformatieLeveren = "N";
            rq.periodeBegindatum = begindatum.ToString("dd-MM-yyyy");
            rq.periodeEinddatum = einddatum.ToString("dd-MM-yyyy");
            //rq.selDiercategorie = "";
            //rq.selDierLandcode = "";
            //rq.selDierLevensnummer = "";
            rq.selDiersoort = IRUtils.PrognrToLNVdiersoort(prognr);
            //rq.selMeldingeenheidTweedePartij = "";
            //rq.selMeldingnummerHoog = "";
            //rq.selMeldingnummerLaag = "";
            //rq.selTransKenteken="";
            //rq.selTransportnummer = "";
            rq.statusgegevens = new statusgegevensType();//verplicht, maar niets invullen 
            //rq.statusgegevens.selMeldingStatus = "N";
            try
            {
                IRUtils.writerequest<raadplegenMeldingAlgRequestType>(ubn, "", tijd, rq);
                var resp = cl.raadplegenMeldingenAlg(rq);
                IRUtils.writerequest<raadplegenMeldingAlgResponseType>(ubn, cl.Endpoint.Address.Uri.ToString(), tijd, resp);
                if (resp.verwerkingsresultaat != null)
                {
                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                    {
                        status = "F";
                        code = resp.verwerkingsresultaat.foutcode;
                        omschrijving = resp.verwerkingsresultaat.foutmelding;
                        unLogger.WriteDebug($@"{fn}:{resp.verwerkingsresultaat.foutmelding}");
                    }
                }
                if (resp.berichttypegegevens != null)
                {
                    // ? wat moeten we ermee
                }
                if (resp.meldinggegevens != null)//de meldingen zelf
                {

                    string[] Kols = { "Levensnummer", "Meldingtype", "Datum", "ubn2ePartij", "meldingsnummer", "meldingstatus", "hersteld", "naam2ePartij" };
                    StringBuilder bld = new StringBuilder();
                    StringBuilder line = new StringBuilder();
                    char[] spl = { '-' };
                    foreach (var m in resp.meldinggegevens)
                    {
                        line.Clear();
                        line.Append($@"{m.dierLandcode } {m.dierLevensnummer};");
                        line.Append($@"{IRUtils.RVOBerichtTypeNaarRumaMeldingType(m.berichttype)};");
                        line.Append($@"{m.gebeurtenisdatum.Split(spl)[2]}{m.gebeurtenisdatum.Split(spl)[1]}{m.gebeurtenisdatum.Split(spl)[0]};");
                        line.Append($@"{m.meldingeenheidTweedePartij};");
                        line.Append($@"{m.meldingnummer};");
                        if (m.meldingStatus == null)
                        {
                            m.meldingStatus = getMeldstatus(cl, m.meldingnummer, rq.meldingeenheid, rq.relatienummerHouder);
                        }
                        line.Append($@"{IRUtils.RvoMeldingStatusNaarRuma(m.meldingStatus)};");
                        int hersteld = m.herstelIndicator == "N" ? 0 : 1;
                        line.Append($@"{hersteld};");
                        line.Append($@"{m.naamMeTweedePartij};");
                        bld.AppendLine(line.ToString());
                    }
                    using (StreamWriter wr = new StreamWriter(outputfile))
                    {
                        wr.Write(bld.ToString());
                    }
                }
            }
            catch (Exception exc)
            {
                status = "F";
                code = "";
                omschrijving = exc.Message;
                unLogger.WriteError($@"{fn}:{exc.ToString()}");
            }

        }

        private string getMeldstatus(MeldingenServiceClient cl, string meldingnummer, string meldingeenheid, string relatienummerhouder)
        {
            raadplegenMeldingDetailRequestType rq = new raadplegenMeldingDetailRequestType();
            rq.meldingnummer = meldingnummer;
            rq.meldingeenheid = meldingeenheid;
            rq.requestID = "1";
            rq.relatienummerHouder = relatienummerhouder;
            var resp = cl.raadplegenMeldingDetail(rq);

            return resp.meldingStatusCode;
        }

        public void raadplegenMeldingDetail(String pUsername, String pPassword, int pTestServer, String ubn, string brsnummer, string meldingnummer,
            ref int progid, ref int lDetailMeldingType, ref int lDetailMeldingStatus,
                                     ref DateTime MutDate, ref DateTime pHerstelDatum, ref DateTime pIntrekDatum,
                                     ref string lLevensnr_Oud, ref string lLevensnr_Nieuw, ref MeldingDetails Result,
                                     ref string lStatus, ref string lCode, ref string lOmschrijving)
        {
            lStatus = "G";
            lCode = "";
            lOmschrijving = "";
            string fn = $"{nameof(SOAPLNVDieren)}.{nameof(raadplegenMeldingDetail)}:UBN:{ubn}:Brs:{brsnummer}";
            try
            {
                string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                EndpointAddress adr = new EndpointAddress(IRUtils.GetMeldingWSEndpoint(pTestServer));

                MeldingenServiceClient cl = new MeldingenServiceClient(binding, adr);
                cl.ClientCredentials.UserName.UserName = pUsername;
                cl.ClientCredentials.UserName.Password = pPassword;

                raadplegenMeldingDetailRequestType rq = new raadplegenMeldingDetailRequestType();
                rq.meldingnummer = meldingnummer;
                rq.meldingeenheid = ubn;
                rq.requestID = $@"1";
                rq.relatienummerHouder = brsnummer;

                IRUtils.writerequest<raadplegenMeldingDetailRequestType>(ubn, cl.Endpoint.Address.Uri.ToString(), tijd, rq);
                //verzenden
                var resp = cl.raadplegenMeldingDetail(rq);
                IRUtils.writerequest<raadplegenMeldingDetailResponseType>(ubn, cl.Endpoint.Address.Uri.ToString(), tijd, resp);

                if (resp == null)
                {
                    lStatus = "F";
                    lOmschrijving = "Geen antwoord van Lnv ";
                    unLogger.WriteError($@"{fn}:  Geen antwoord van Lnv ");
                    return;
                }

                if (resp.verwerkingsresultaat != null)
                {
                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                    {
                        lStatus = "F";
                        lCode = resp.verwerkingsresultaat.foutcode;
                        lOmschrijving = resp.verwerkingsresultaat.foutmelding;
                        unLogger.WriteDebug($@"{fn}:{resp.verwerkingsresultaat.foutmelding}");
                        return;
                    }

                }


                lDetailMeldingType = IRUtils.RVOBerichtTypeNaarRumaMeldingType(resp.berichttype);
                lDetailMeldingStatus = IRUtils.RvoMeldingStatusNaarRuma(resp.meldingStatusCode);
                MutDate = Convert.ToDateTime(resp.verwerkingsdatum);
                //Wat is de Mutdate?  resp.datumEinde, resp.dierKalfdatum,resp.gebeurtenisdatum,resp.geboortedatum,resp.intrekdatum
                pHerstelDatum = Convert.ToDateTime(resp.hersteldatum);
                pIntrekDatum = Convert.ToDateTime(resp.intrekdatum);
                lLevensnr_Oud = resp.dierLandcode + " " + resp.dierLevensnummer;
                lLevensnr_Nieuw = resp.dierVervangendLandcode + " " + resp.dierVervangendLevensnummer;

                Result = new MeldingDetails();
                Result.aantalDieren = resp.aantalDieren;
                Result.AantalDierenOpBedrijf = resp.AantalDierenOpBedrijf;
                Result.aanvullendeMatches = new List<aanvullendeMatch>();
                if (resp.aanvullendeMatches != null)
                {
                    foreach (var rr in resp.aanvullendeMatches)
                    {
                        try
                        {
                            Result.aanvullendeMatches.Add(new aanvullendeMatch { meldingnummerEigen = rr.meldingnummerEigen, meldingnummerTweedePartij = rr.meldingnummerTweedePartij });
                        }
                        catch { }
                    }
                }
                Result.berichtdatum = resp.berichtdatum;
                Result.berichttijd = resp.berichttijd;
                Result.berichttype = resp.berichttype;
                Result.communicatiekanaal = resp.communicatiekanaal;
                Result.datumEinde = resp.datumEinde;
                Result.dierAliasLandcode = resp.dierAliasLandcode;
                Result.dierAliasLevensnummer = resp.dierAliasLevensnummer;
                Result.dierAliasWerknummer = resp.dierAliasWerknummer;
                Result.dierBestemmingLandcode = resp.dierBestemmingLandcode;
                Result.dierCategorie = resp.dierCategorie;
                Result.dierGeslacht = resp.dierGeslacht;
                Result.dierHaarkleur = resp.dierHaarkleur;
                Result.dierHerkomstLandcode = resp.dierHerkomstLandcode;
                Result.dierKalfdatum = resp.dierKalfdatum;
                Result.dierLandcode = resp.dierLandcode;
                Result.dierLevensnummer = resp.dierLevensnummer;
                Result.dierOorsprongLandcode = resp.dierOorsprongLandcode;
                Result.dierOorspronkelijkeIdentificatie = resp.dierOorspronkelijkeIdentificatie;
                Result.dierPremiestatus = resp.dierPremiestatus;
                Result.diersoort = resp.diersoort;
                Result.dierTijdelijkLandcode = resp.dierTijdelijkLandcode;
                Result.dierTijdelijkLevensnummer = resp.dierTijdelijkLevensnummer;
                Result.dierTijdelijkWerknummer = resp.dierTijdelijkWerknummer;
                Result.dierVervangendLandcode = resp.dierVervangendLandcode;
                Result.dierVervangendLevensnummer = resp.dierVervangendLevensnummer;
                Result.dierVervangendWerknummer = resp.dierVervangendWerknummer;
                Result.dierWerknummer = resp.dierWerknummer;
                Result.gebeurtenisdatum = resp.gebeurtenisdatum;
                Result.geboortedatum = resp.geboortedatum;
                Result.groepsgegevens = resp.groepsgegevens;
                Result.hersteldatum = resp.hersteldatum;
                Result.herstelIndicator = resp.herstelIndicator;
                Result.herstelMeldingnummerVolgend = resp.herstelMeldingnummerVolgend;
                Result.herstelMeldingnummerVorig = resp.herstelMeldingnummerVorig;
                Result.hersteltijd = resp.hersteltijd;
                Result.importdatumCIS = resp.importdatumCIS;
                Result.indHerstelMogelijkheden = resp.indHerstelMogelijkheden;
                Result.indIntrekbaar = resp.indIntrekbaar;
                Result.intrekdatum = resp.intrekdatum;
                Result.intrektijd = resp.intrektijd;
                Result.meldingeenheid = resp.meldingeenheid;
                Result.meldingeenheidBestemming = resp.meldingeenheidBestemming;
                Result.meldingeenheidDestructor = resp.meldingeenheidDestructor;
                Result.meldingeenheidHerkomst = resp.meldingeenheidHerkomst;
                Result.meldingeenheidNoodslacht = resp.meldingeenheidNoodslacht;
                Result.meldingnummer = resp.meldingnummer;
                Result.meldingnummerMatch = resp.meldingnummerMatch;
                Result.meldingStatusCode = resp.meldingStatusCode;
                Result.meldingStatusOms = resp.meldingStatusOms;
                Result.moederGeboortedatum = resp.moederGeboortedatum;
                Result.moederGeslacht = resp.moederGeslacht;
                Result.moederHaarkleur = resp.moederHaarkleur;
                Result.moederLandcode = resp.moederLandcode;
                Result.moederLevensnummer = resp.moederLevensnummer;
                Result.moederWerknummer = resp.moederWerknummer;
                Result.naamMeTweedePartij = resp.naamMeTweedePartij;
                Result.nummerGezondheidscertificaat = resp.nummerGezondheidscertificaat;
                Result.redenBlokkade = resp.redenBlokkade;
                Result.redenRuiming = resp.redenRuiming;
                Result.relatienummerAcceptant = resp.relatienummerAcceptant;
                Result.relatienummerHouder = resp.relatienummerHouder;
                Result.relatienummerMelder = resp.relatienummerMelder;
                Result.relatienummerOverdrager = resp.relatienummerOverdrager;
                Result.requestID = resp.requestID;
                Result.transKenteken = resp.transKenteken;
                Result.transNaamVervoerder = resp.transNaamVervoerder;
                Result.transportnummer = resp.transportnummer;
                Result.transRelatienummerVervoerder = resp.transRelatienummerVervoerder;
                Result.transTijdstipVertrek = resp.transTijdstipVertrek;
                Result.transVerwachteTransportduur = resp.transVerwachteTransportduur;
                Result.verwerkingsdatum = resp.verwerkingsdatum;
                Result.verwerkingsresultaat = resp.verwerkingsresultaat;
                Result.verwerkingstijd = resp.verwerkingstijd;
                Result.vlagsoortCodeReden = resp.vlagsoortCodeReden;
            }
            catch (Exception exc)
            {
                lStatus = "F";
                lOmschrijving = exc.Message;
                unLogger.WriteError($@"{fn}: {exc.ToString()}");
            }

        }

        public vlagresultaat raadplegenVlaggen(String username, String password, string ubn, string brsnummer, int testserver)
        {
            vlagresultaat ret = new vlagresultaat();
            ret.berichttijd = DateTime.Now;
            ret.Omschrijving = "";
            ret.vlaggegevens = new List<vlaggegevens>();
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            EndpointAddress adr = new EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));

            MeldingenServiceClient cl = new MeldingenServiceClient(binding, adr);
            cl.ClientCredentials.UserName.UserName = username;
            cl.ClientCredentials.UserName.Password = password;

            raadplegenVlaggenRequestType rq = new raadplegenVlaggenRequestType();
            rq.requestID = "1";
            rq.relatienummerHouder = brsnummer;
            rq.meldingeenheid = ubn;
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                IRUtils.writerequest<raadplegenVlaggenRequestType>(username, cl.Endpoint.Address.Uri.ToString(), tijd, rq);
                var result = cl.raadplegenVlaggen(rq);
                IRUtils.writerequest<raadplegenVlaggenResponseType>(username, cl.Endpoint.Address.Uri.ToString(), tijd, result);
                ret.Omschrijving = result.verwerkingsresultaat.foutmelding;
                foreach (var vlag in result.vlaggegevens)
                {
                    try
                    {
                        vlaggegevens v = new vlaggegevens();
                        v.datumEinde = vlag.datumEinde;
                        v.datumIngang = vlag.datumIngang;
                        v.dierLandcode = vlag.dierLandcode;
                        v.dierLevensnummer = vlag.dierLevensnummer;
                        v.dierSoort = vlag.dierSoort;
                        v.dierWerknummer = vlag.dierWerknummer;
                        v.vlagsoortCodeReden = vlag.vlagsoortCodeReden;
                        ret.vlaggegevens.Add(v);
                    }
                    catch (Exception exc)
                    {
                        unLogger.WriteError($@"{nameof(raadplegenVlaggen)} : {exc.ToString()}");
                        ret.Omschrijving = exc.Message;
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(raadplegenVlaggen)} : {exc.ToString()}");
                ret.Omschrijving = exc.ToString();
            }
            return ret;
        }
        public List<Soaplogmelding> LNVIRaanvoermeldingV2(List<MUTATION> pRecords, String username, String password, int testserver, string ubn, string brsnummer, int Diersoort, int herstelIndicator)
        {
           
            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 1;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRaanvoermeldingV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (var mut in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = mut.CodeMutation;
                sl.Lifenumber = mut.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = mut.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRaanvoerV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    srMeldingenWS.vastleggenAanvoerMeldingRequestType aanv = new srMeldingenWS.vastleggenAanvoerMeldingRequestType();

                    aanv.actie = "V";
                    aanv.herstelIndicator = herstelIndicator == 0 ? "N" : "J";
                    List<srMeldingenWS.diergroepsgegevensRequestType> l = new List<srMeldingenWS.diergroepsgegevensRequestType>();
                    int[] meldcodes = { 1, 11, 201, 211 };
                    foreach (var pRecord in pRecords)
                    {
                        srMeldingenWS.diergroepsgegevensRequestType t = new srMeldingenWS.diergroepsgegevensRequestType();
                        char[] split = { ' ' };
                        string[] lnrs = pRecord.Lifenumber.Split(split);
                        t.dierLandcode = lnrs[0];
                        t.dierLevensnummer = lnrs[1];

                        t.dierSoort = Diersoort;
                        if (aanv.herstelIndicator == "J")
                        {
                            t.meldingnummerOorsprong = pRecord.MeldingNummer;
                        }
                        if (meldcodes.Contains(pRecord.CodeMutation))
                        {
                            l.Add(t);
                        }
                        else
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = $@"Codemutation not correct:{pRecord.CodeMutation} expected: 1, 11, 201 or 211 ";
                            }
                        }
                    }
                    aanv.requestID = $@"1";
                    aanv.diergegevensRequest = l.ToArray();
                    aanv.transportAanvoerGegevens = new srMeldingenWS.transportAanvoerGegevensType();
                    aanv.transportAanvoerGegevens.aantalDieren = l.Count();
                    aanv.transportAanvoerGegevens.aanvoerdatum = pRecords[0].MutationDate.ToString("dd-MM-yyyy");
                    if (!string.IsNullOrWhiteSpace(pRecords[0].FarmNumberFrom))
                    {
                        aanv.transportAanvoerGegevens.meldingeenheidHerkomst = pRecords[0].FarmNumberFrom;
                    }


                    aanv.meldingeenheid = ubn;
                    aanv.relatienummerHouder = brsnummer;
                    IRUtils.writerequest<srMeldingenWS.vastleggenAanvoerMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, aanv);
                    //verzenden
                    var result = m.vastleggenAanvoerMelding(aanv);
                    IRUtils.writerequest<srMeldingenWS.vastleggenAanvoerMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensResponse.Count() > 0)
                    {
                        foreach (var resp in result.diergegevensResponse)
                        {
                            var sls = from n in slm
                                     where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                     select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                if (resp.verwerkingsresultaat.succesIndicator != "J")
                                {
                                    string status = resp.verwerkingsresultaat.succesIndicator;
                                    sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(sls.ElementAt(0).meldnummer) || sls.ElementAt(0).meldnummer == "null" ? "F" : "W";
                                }
                                if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                {
                                    sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                    {
                                        sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        
                       
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = $@"No data returned:{result.actie}";
                        
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRaanvoermeldingV2)}:{exc.ToString()}");
          
             
                slm.RemoveRange(1, slm.Count() - 1);
                slm[0].Status = "F";
                slm[0].Omschrijving = $@"{exc.Message}";
            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRgeboortemeldingV2(List<MUTATION> pRecords, string username, string password, int testserver,
                    string ubn, string brsnummer, int diersoort
                    , int herstelIndicator)
        {
             
            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 2;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRgeboortemeldingV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (var mut in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = mut.CodeMutation;
                sl.Lifenumber = mut.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = mut.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRGeboorteV2";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;


                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    srMeldingenWS.vastleggenGeboorteMeldingRequestType geb = new vastleggenGeboorteMeldingRequestType();

                    geb.actie = "V";
                    geb.herstelIndicator = herstelIndicator == 0 ? "N" : "J";
                    List<srMeldingenWS.diergegevensGeboorteRequestType> l = new List<srMeldingenWS.diergegevensGeboorteRequestType>();
                    foreach (MUTATION pRecord in pRecords)
                    {
                        srMeldingenWS.diergegevensGeboorteRequestType t = new srMeldingenWS.diergegevensGeboorteRequestType();
                        char[] split = { ' ' };
                        string[] lnrs = pRecord.Lifenumber.Split(split);
                        t.selDierLandcode = lnrs[0];
                        t.selDierLevensnummer = lnrs[1];
                        if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                        {
                            t.selDierWerknummer = pRecord.Worknumber;
                        }
                        t.dierSoort = diersoort;
                        string[] moeder = pRecord.LifenumberMother.Split(split);
                        t.selMoederLandcode = moeder[0];
                        t.selMoederLevensnummer = moeder[1];
                        t.geboortedatum = pRecord.MutationDate.ToString("dd-MM-yyyy");
                        t.dierGeslacht = pRecord.Sex == 1 ? "M" : "V";
                        if (diersoort != 3 && diersoort != 5)
                        {
                            t.dierHaarkleur = IRUtils.LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo);

                        }
                        if (geb.herstelIndicator == "J")
                        {
                            t.meldingnummerOorsprong = pRecord.MeldingNummer;
                        }
                        if (pRecord.CodeMutation == 2 || pRecord.CodeMutation == 202)
                        {
                            l.Add(t);
                        }
                        else 
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = $@"Codemutation not correct:{pRecord.CodeMutation} expected: 2 or 202 ";
                            }
                        }
                    }
                    geb.requestID = $@"1";
                    geb.diergegevensGeboorteRequest = l.ToArray();
                 
                    geb.meldingeenheid = ubn;
                    geb.relatienummerHouder = brsnummer;
                    IRUtils.writerequest<srMeldingenWS.vastleggenGeboorteMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, geb);
                    //verzenden
                    var result = m.vastleggenGeboorteMelding(geb);
                    IRUtils.writerequest<srMeldingenWS.vastleggenGeboorteMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensGeboorteResponse.Count() > 0)
                    {
                        foreach (var resp in result.diergegevensGeboorteResponse)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                if (resp.verwerkingsresultaat.succesIndicator != "J")
                                {
                                    string status = resp.verwerkingsresultaat.succesIndicator;
                                    sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                }
                                if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                {
                                    sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                    {
                                        sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = $@"No data returned:{result.meldingeenheid}";
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRgeboortemeldingV2)}:{exc.ToString()}");
                foreach (var s in slm)
                {
                    //when did the error occurr we don't know
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRafvoermeldingV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {
            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 4;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRafvoermeldingV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRAfvoerV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            //per MutationDate en farmnumberto
            var groep = pRecords.GroupBy(x => new { x.MutationDate.Date, x.FarmNumberTo });
            int[] meldingen = { 4, 5, 12, 204, 205, 212 };
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;


                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    foreach (var gr in groep)
                    {
                        srMeldingenWS.vastleggenAfvoerMeldingRequestType afv = new vastleggenAfvoerMeldingRequestType();

                        afv.actie = "V";
                        afv.herstelIndicator = herstelmelding == 0 ? "N" : "J";
                        List<srMeldingenWS.diergegevensSelRequestType> l = new List<srMeldingenWS.diergegevensSelRequestType>();
                       
                        foreach (MUTATION pRecord in gr)
                        {
                            srMeldingenWS.diergegevensSelRequestType t = new srMeldingenWS.diergegevensSelRequestType();
                            char[] split = { ' ' };
                            string[] lnrs = pRecord.Lifenumber.Split(split);
                            t.selDierLandcode = lnrs[0];
                            t.selDierLevensnummer = lnrs[1];
                            if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                            {
                                //t.selDierWerknummer = pRecord.Worknumber;//uitgezet want:veel te vaak de fout:Het opgegeven werknummer is niet in overeenstemming met de opgegeven ID-code.
                            }
                            t.dierSoort = diersoort;
                            string[] moeder = pRecord.LifenumberMother.Split(split);
                            if (afv.herstelIndicator == "J")
                            {
                                afv.diergegevensSelRequest[0].meldingnummerOorsprong = pRecord.MeldingNummer;
                            }
                            if (meldingen.Contains(pRecord.CodeMutation))
                            {
                                l.Add(t);
                            }
                            else 
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == pRecord.Lifenumber
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).Status = "F";
                                    sls.ElementAt(0).Omschrijving = $@"Codemutation not correct:{pRecord.CodeMutation} expected:4, 5, 12, 204, 205 or 212";
                                }
                            }
                        }

                        afv.diergegevensSelRequest = l.ToArray();
                        afv.requestID = $@"1";
                        afv.transportAfvoerGegevens = new transportAfvoerGegevensType();
                        afv.transportAfvoerGegevens.aantalDieren = l.Count();
                        afv.transportAfvoerGegevens.afvoerdatum = gr.ElementAt(0).MutationDate.ToString("dd-MM-yyyy");
                        if (!string.IsNullOrEmpty(gr.ElementAt(0).FarmNumberTo))
                        {
                            afv.transportAfvoerGegevens.meldingeenheidBestemming = gr.ElementAt(0).FarmNumberTo;
                        }
                        afv.transportAfvoerGegevens.transVerwachteTransportduur = 0;


                       
                       
                        afv.meldingeenheid = ubn;
                        afv.relatienummerHouder = brsnummer;
                        IRUtils.writerequest<srMeldingenWS.vastleggenAfvoerMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, afv);
                        //verzenden
                        var result = m.vastleggenAfvoerMelding(afv);
                        IRUtils.writerequest<srMeldingenWS.vastleggenAfvoerMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                        if (result.diergegevensSelResponse.Count() > 0)
                        {
                            foreach (var resp in result.diergegevensSelResponse)
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                    if (resp.verwerkingsresultaat.succesIndicator != "J")
                                    {
                                        string status = resp.verwerkingsresultaat.succesIndicator;
                                        sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                    }
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                    {
                                        sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                        if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                        {
                                            sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRafvoermeldingV2)}:No reaction from LNV.");
                            foreach (var g in gr)
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == g.Lifenumber
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).Status = "F";
                                    sls.ElementAt(0).Omschrijving = "No reaction from LNV.";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRafvoermeldingV2)}:{exc.ToString()}");
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }

            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRimportmeldingV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {
            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 7;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";
               
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRimportV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRimportV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
           
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    //per MutationDate 
                    var groep = pRecords.GroupBy(x => new { x.MutationDate.Date });
                    foreach (var gr in groep)
                    {
                        srMeldingenWS.vastleggenImportMeldingRequestType imp = new srMeldingenWS.vastleggenImportMeldingRequestType();

                        imp.actie = "V";
                        imp.herstelIndicator = herstelmelding == 0 ? "N" : "J";
                       
                        List<srMeldingenWS.diergegevensImportRequestType> l = new List<srMeldingenWS.diergegevensImportRequestType>();
                        foreach (var pRecord in gr)
                        {
                            srMeldingenWS.diergegevensImportRequestType t = new srMeldingenWS.diergegevensImportRequestType();
                            char[] split = { ' ' };
                            string[] lnrs = pRecord.Lifenumber.Split(split);
                            t.dierLandcode = lnrs[0];
                            t.dierLevensnummer = lnrs[1];

                            t.dierSoort = diersoort;
                            t.dierGeboortedatum = pRecord.IDRBirthDate.ToString("dd-MM-yyyy");
                            t.dierGeslacht = pRecord.Sex == 1 ? "M" : "V";
                            t.dierHerkomstLandcode = pRecord.CountryCodeDepart;
                            t.dierOorsprongLandcode = pRecord.CountryCodeBirth;
                            t.dierPremiestatus = pRecord.Subsidy == 0 ? "N" : "J";
                            if (diersoort == 1)
                            {
                                t.dierHaarkleur = IRUtils.LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo.Trim());

                            }
                            string[] lnrsmoeder = pRecord.LifenumberMother.Split(split);
                            if (lnrsmoeder.Length == 2)
                            {
                                t.moederLandcode = lnrsmoeder[0];
                                t.moederLevensnummer = lnrsmoeder[1];
                            }
                            if (imp.herstelIndicator == "J")
                            {
                                t.meldingnummerOorsprong = pRecord.MeldingNummer;
                            }
                            if (pRecord.CodeMutation == 7 || pRecord.CodeMutation == 207)
                            {
                                l.Add(t);
                            }
                            else 
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == pRecord.Lifenumber
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).Status = "F";
                                    sls.ElementAt(0).Omschrijving = $@"Codemutation not correct:{pRecord.CodeMutation} expected:7 or 207";
                                }
                            }
                        }
                        imp.requestID = $@"1";
                        imp.diergegevensImportRequest = l.ToArray();



                        imp.meldingeenheid = ubn;
                        imp.relatienummerHouder = brsnummer;
                        imp.transportImportGegevens = new transportImportGegevensType();
                        imp.transportImportGegevens.aantalDieren = l.Count();
                        imp.transportImportGegevens.importdatum = gr.ElementAt(0).MutationDate.ToString("dd-MM-yyyy");

                        IRUtils.writerequest<srMeldingenWS.vastleggenImportMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, imp);
                        //verzenden
                        var result = m.vastleggenImportMelding(imp);
                        IRUtils.writerequest<srMeldingenWS.vastleggenImportMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                        if (result.diergegevensImportResponse.Count() > 0)
                        {
                            foreach (var resp in result.diergegevensImportResponse)
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                    if (resp.verwerkingsresultaat.succesIndicator != "J")
                                    {
                                        string status = resp.verwerkingsresultaat.succesIndicator;
                                        sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                    }
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                    {
                                        sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                        if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                        {
                                            sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var pRecord in gr)
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == pRecord.Lifenumber
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).Status = "F";
                                    sls.ElementAt(0).Omschrijving = $@"No data returned:{result.actie}";
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRimportmeldingV2)}:{exc.ToString()}");
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRexportmeldingV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {

            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 4;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRexportV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRAfvoerV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }
                    //per MutationDate 
                    var groep = pRecords.GroupBy(x => new { x.MutationDate.Date });
                    foreach (var gr in groep)
                    {
                        srMeldingenWS.vastleggenExportMeldingRequestType exp = new srMeldingenWS.vastleggenExportMeldingRequestType();

                        exp.actie = "V";
                        exp.herstelIndicator = herstelmelding == 0 ? "N" : "J";
                        List<srMeldingenWS.diergegevensExportRequestType> l = new List<srMeldingenWS.diergegevensExportRequestType>();
                        foreach (var pRecord in gr)
                        {
                            srMeldingenWS.diergegevensExportRequestType t = new srMeldingenWS.diergegevensExportRequestType();
                            char[] split = { ' ' };
                            string[] lnrs = pRecord.Lifenumber.Split(split);
                            t.selDierLandcode = lnrs[0];
                            t.selDierLevensnummer = lnrs[1];
                            if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                            {
                                t.selDierWerknummer = pRecord.Worknumber;
                            }
                            t.dierSoort = diersoort;
                            if (exp.herstelIndicator == "J")
                            {
                                t.meldingnummerOorsprong = pRecord.MeldingNummer;
                            }
                            if (pRecord.CodeMutation == 9 || pRecord.CodeMutation == 209)
                            {
                                l.Add(t);
                            }
                            else 
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == pRecord.Lifenumber
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).Status = "F";
                                    sls.ElementAt(0).Omschrijving = $@"Codemutation not correct:{pRecord.CodeMutation} expected:9 or 209";
                                }
                            }
                        }
                        exp.requestID = $@"1";
                        exp.diergegevensExportRequest = l.ToArray();
                        exp.meldingeenheid = ubn;
                        exp.relatienummerHouder = brsnummer;

                        exp.transportExportGegevens = new transportExportGegevensType();
                        exp.transportExportGegevens.aantalDieren = l.Count();
                        exp.transportExportGegevens.afvoerdatum = gr.ElementAt(0).MutationDate.ToString("dd-MM-yyyy");
                        exp.transportExportGegevens.transVerwachteTransportduur = 0;
                        IRUtils.writerequest<srMeldingenWS.vastleggenExportMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, exp);
                        //verzenden
                        var result = m.vastleggenExportMelding(exp);
                        IRUtils.writerequest<srMeldingenWS.vastleggenExportMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                        if (result.diergegevensExportResponse.Count() > 0)
                        {
                            foreach (var resp in result.diergegevensExportResponse)
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                    if (resp.verwerkingsresultaat.succesIndicator != "J")
                                    {
                                        string status = resp.verwerkingsresultaat.succesIndicator;
                                        sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                    }
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                    {
                                        sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                        if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                        {
                                            sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var pRecord in gr)
                            {
                                var sls = from n in slm
                                          where n.Lifenumber == pRecord.Lifenumber
                                          select n;
                                if (sls.Count() > 0)
                                {
                                    sls.ElementAt(0).Status = "F";
                                    sls.ElementAt(0).Omschrijving = $@"No data returned: ";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRexportmeldingV2)}:{exc.ToString()}");
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRdoodgeborenmeldingV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort)
        {

            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 7;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRdoodgebV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRdoodgebV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    srMeldingenWS.vastleggenDoodGeborenDierMeldingRequestType dbd = new srMeldingenWS.vastleggenDoodGeborenDierMeldingRequestType();

                    dbd.actie = "V";
                    List<srMeldingenWS.diergegevensDoodGeborenDierRequestType> l = new List<srMeldingenWS.diergegevensDoodGeborenDierRequestType>();
                    foreach (var pRecord in pRecords)
                    {
                        srMeldingenWS.diergegevensDoodGeborenDierRequestType t = new srMeldingenWS.diergegevensDoodGeborenDierRequestType();
                        char[] split = { ' ' };
                        string[] lnrs = pRecord.Lifenumber.Split(split);
                        t.selDierLandcode = lnrs[0];
                        t.selDierLevensnummer = lnrs[1];
                        if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                        {
                            t.selDierWerknummer = pRecord.Worknumber;
                        }
                        t.dierSoort = diersoort;
                        t.doodDatum = pRecord.MutationDate.ToString("dd-MM-yyyy");
                        t.meldingeenheidDestructor = pRecord.FarmNumberTo;
                      

                        l.Add(t);
                    }
                    dbd.requestID = $@"1";
                    dbd.diergegevensDoodGeborenDierRequest = l.ToArray();
                   
                   
                    dbd.meldingeenheid = ubn;
                    dbd.relatienummerHouder = brsnummer;

                    IRUtils.writerequest<srMeldingenWS.vastleggenDoodGeborenDierMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, dbd);
                    //verzenden
                    var result = m.vastleggenDoodGeborenDierMelding(dbd);
                    IRUtils.writerequest<srMeldingenWS.vastleggenDoodGeborenDierMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensDoodGeborenDierResponse.Count() > 0)
                    {
                        foreach (var resp in result.diergegevensDoodGeborenDierResponse)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                if (resp.verwerkingsresultaat.succesIndicator != "J")
                                {
                                    string status = resp.verwerkingsresultaat.succesIndicator;
                                    sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                }
                                if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                {
                                    sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                    {
                                        sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var pRecord in pRecords)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = $@"No data returned:{result.actie}";
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRdoodgeborenmeldingV2)}:{exc.ToString()}");
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRdoodmeldingV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {
            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 7;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRdoodV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRdoodV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    srMeldingenWS.vastleggenDoodMeldingRequestType dm = new srMeldingenWS.vastleggenDoodMeldingRequestType();

                    dm.actie = "V";
                    dm.requestID = $@"1";
                    dm.herstelIndicator = herstelmelding > 0 ? "J" : "N";
                    List<srMeldingenWS.diergegevensDoodRequestType> l = new List<srMeldingenWS.diergegevensDoodRequestType>();

                    foreach (var pRecord in pRecords)
                    {
                        srMeldingenWS.diergegevensDoodRequestType t = new srMeldingenWS.diergegevensDoodRequestType();
                        char[] split = { ' ' };
                        string[] lnrs = pRecord.Lifenumber.Split(split);
                        t.selDierLandcode = lnrs[0];
                        t.selDierLevensnummer = lnrs[1];
                        if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                        {
                            t.selDierWerknummer = pRecord.Worknumber;
                        }
                        t.dierSoort = diersoort;
                        t.doodDatum = pRecord.MutationDate.ToString("dd-MM-yyyy");
                        t.meldingeenheidDestructor = pRecord.FarmNumberTo;

                       
                        if (herstelmelding > 0)
                        {
                            t.meldingnummerOorsprong = pRecord.MeldingNummer;
                        }


                        l.Add(t);
                    }
                    dm.diergegevensDoodRequest = l.ToArray();

                
                    
                    dm.meldingeenheid = ubn;
                    dm.relatienummerHouder = brsnummer;

                    IRUtils.writerequest<srMeldingenWS.vastleggenDoodMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, dm);
                    //verzenden
                    var result = m.vastleggenDoodMelding(dm);
                    IRUtils.writerequest<srMeldingenWS.vastleggenDoodMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensDoodResponse.Count() > 0)
                    {
                        foreach (var resp in result.diergegevensDoodResponse)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                if (resp.verwerkingsresultaat.succesIndicator != "J")
                                {
                                    string status = resp.verwerkingsresultaat.succesIndicator;
                                    sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                }
                                if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                {
                                    sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                    {
                                        sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var pRecord in pRecords)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = $@"No data returned:{result.actie}";
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRdoodmeldingV2)}:{exc.ToString()}");
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }
            return slm;
        }

        public List<Soaplogmelding> LNVIRslachtmeldingV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {
            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 7;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRslachtV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRslachtV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    srMeldingenWS.vastleggenSlachtMeldingRequestType slacht = new srMeldingenWS.vastleggenSlachtMeldingRequestType();

                    slacht.actie = "V";
                    slacht.herstelIndicator = herstelmelding > 0 ? "J" : "N";
                    List<srMeldingenWS.diergegevensSlachtRequestType> l = new List<srMeldingenWS.diergegevensSlachtRequestType>();
                    foreach (var pRecord in pRecords)
                    {
                        srMeldingenWS.diergegevensSlachtRequestType t = new srMeldingenWS.diergegevensSlachtRequestType();
                        char[] split = { ' ' };
                        string[] lnrs = pRecord.Lifenumber.Split(split);
                        t.dierLandcode = lnrs[0];
                        t.dierLevensnummer = lnrs[1];
                        if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                        {
                            t.dierWerknummer = pRecord.Worknumber;
                        }
                        t.dierSoort = diersoort;
                        t.slachtdatum = pRecord.MutationDate.ToString("dd-MM-yyyy");
                        t.meldingeenheidDestructor = pRecord.FarmNumberTo;
                        if (herstelmelding > 0)
                        {
                            t.meldingnummerOorsprong = pRecord.MeldingNummer;
                        }

                        l.Add(t);
                    }
                    
                    slacht.requestID = $@"1";
                    slacht.diergegevensSlachtRequest = l.ToArray();


                    slacht.meldingeenheid = ubn;
                    slacht.relatienummerHouder = brsnummer;

                    IRUtils.writerequest<srMeldingenWS.vastleggenSlachtMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, slacht);
                    //verzenden
                    var result = m.vastleggenSlachtMelding(slacht);
                    IRUtils.writerequest<srMeldingenWS.vastleggenSlachtMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensSlachtResponse.Count() > 0)
                    {
                        foreach (var resp in result.diergegevensSlachtResponse)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).meldnummer = resp.meldingnummer;
                                if (resp.verwerkingsresultaat.succesIndicator != "J")
                                {
                                    string status = resp.verwerkingsresultaat.succesIndicator;
                                    sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                }
                                if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                {
                                    sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                    {
                                        sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var pRecord in pRecords)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = $@"No data returned:{result.actie}";
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError("LNVIRslachtmeldingV2 " + exc.ToString());
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }
            return slm;
        }

        public Soaplogmelding LNVIRnoodslachtV2(MUTATION pRecord, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {
            
            Soaplogmelding sl = new Soaplogmelding();
            sl.Date = DateTime.Now.Date;
            sl.FarmNumber = ubn;
            sl.Kind = pRecord.CodeMutation;
            sl.Lifenumber = pRecord.Lifenumber;
            sl.Status = "G";
            sl.Omschrijving = "";
            sl.ThrId = pRecord.tbv_ThrID;
            sl.Time = DateTime.Now;
            sl.Code = "LNVIRSlachtV2";
            string tijd = sl.Time.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        sl.Status = "F";
                        sl.Omschrijving = "Username or Password empty.";
                        return sl;
                    }

                    srMeldingenWS.vastleggenNoodslachtMeldingRequestType slacht = new srMeldingenWS.vastleggenNoodslachtMeldingRequestType();

                    slacht.actie = "V";
                    slacht.diergegevensNoodslachtRequest = new diergegevensNoodslachtRequestType();

                    char[] split = { ' ' };
                    string[] lnrs = pRecord.Lifenumber.Split(split);
                    slacht.diergegevensNoodslachtRequest.selDierLandcode = lnrs[0];
                    slacht.diergegevensNoodslachtRequest.selDierLevensnummer = lnrs[1];
                    slacht.diergegevensNoodslachtRequest.dierSoort = diersoort;
                    slacht.diergegevensNoodslachtRequest.slachtdatum = pRecord.MutationDate.ToString("dd-MM-yyyy");
                    slacht.diergegevensNoodslachtRequest.meldingeenheidNoodslacht = pRecord.FarmNumberTo;
                    if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                    {
                        slacht.diergegevensNoodslachtRequest.selDierWerknummer = pRecord.Worknumber;
                    }

                    slacht.requestID = $@"{pRecord.Internalnr}";
                    slacht.meldingeenheid = ubn;
                    slacht.relatienummerHouder = brsnummer;

                    IRUtils.writerequest<srMeldingenWS.vastleggenNoodslachtMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, slacht);
                    //verzenden
                    var result = m.vastleggenNoodslachtMelding(slacht);
                    IRUtils.writerequest<srMeldingenWS.vastleggenNoodslachtMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensNoodslachtResponse != null)
                    {
                        sl.meldnummer = result.diergegevensNoodslachtResponse.meldingnummer;
                        if (result.diergegevensNoodslachtResponse.verwerkingsresultaat.succesIndicator != "J")
                        {
                            string status = result.diergegevensNoodslachtResponse.verwerkingsresultaat.succesIndicator;
                            sl.Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(sl.meldnummer) || sl.meldnummer == "null" ? "F" : "W";
                        }
                        if (!string.IsNullOrWhiteSpace(result.diergegevensNoodslachtResponse.verwerkingsresultaat.foutmelding))
                        {
                            sl.Omschrijving = result.diergegevensNoodslachtResponse.verwerkingsresultaat.foutmelding;
                            if (!string.IsNullOrWhiteSpace(result.diergegevensNoodslachtResponse.verwerkingsresultaat.foutcode))
                            {
                                sl.Code = result.diergegevensNoodslachtResponse.verwerkingsresultaat.foutcode;
                            }
                        }
                    }
                    else
                    {
                        sl.Status = "F";
                        sl.Omschrijving = $@"No data returned:{result.actie}";
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRslachtmeldingV2)}:{exc.ToString()}");
                sl.Status = "F";
                sl.Omschrijving = $@"{exc.Message}";
            }

            return sl;
        }

        public List<Soaplogmelding> LNVIRQkoortsV2(List<MUTATION> pRecords, string username, string password, int testserver, string ubn, string brsnummer, int diersoort, int herstelmelding)
        {

            List<Soaplogmelding> slm = new List<Soaplogmelding>();
            if (pRecords.Count() == 0)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = 7;
                sl.Lifenumber = "";
                sl.Status = "F";
                sl.Omschrijving = "Nothing to report";

                sl.Time = DateTime.Now;
                sl.Code = "LNVIRQkrtsV2";
                sl.meldnummer = "";
                slm.Add(sl);
                return slm;
            }
            foreach (MUTATION pRecord in pRecords)
            {
                Soaplogmelding sl = new Soaplogmelding();
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = ubn;
                sl.Kind = pRecord.CodeMutation;
                sl.Lifenumber = pRecord.Lifenumber;
                sl.Status = "G";
                sl.Omschrijving = "";
                sl.ThrId = pRecord.tbv_ThrID;
                sl.Time = DateTime.Now;
                sl.Code = "LNVIRQkrtsV2";
                sl.meldnummer = "";
                slm.Add(sl);
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        slm.RemoveRange(1, slm.Count() - 1);
                        slm[0].Status = "F";
                        slm[0].Omschrijving = "Username or Password empty.";
                        return slm;
                    }

                    srMeldingenWS.vastleggenDiervlagMeldingRequestType vlag = new srMeldingenWS.vastleggenDiervlagMeldingRequestType();

                    vlag.actie = "V";
                    vlag.requestID = $@"1";
                    vlag.herstelIndicator = herstelmelding == 0 ? "N" : "J";
                    List<diergegevensDiervlagMeldingRequestType> l = new List<diergegevensDiervlagMeldingRequestType>();

                    foreach (var pRecord in pRecords)
                    {
                        diergegevensDiervlagMeldingRequestType t = new diergegevensDiervlagMeldingRequestType();
                        t.datumIngang = pRecord.MutationDate.ToString("dd-MM-yyyy");
                        t.dierSoort = diersoort;
                        char[] split = { ' ' };
                        string[] lnrs = pRecord.Lifenumber.Split(split);
                        t.selDierLandcode = lnrs[0];
                        t.selDierLevensnummer = lnrs[1];
                        if (!string.IsNullOrWhiteSpace(pRecord.Worknumber))
                        {
                            t.selDierWerknummer = pRecord.Worknumber;
                        }
                        if (pRecord.CodeMutation == (int)VSM.RUMA.CORE.DB.LABELSConst.CodeMutation.QKrtsvacc2)
                        {
                            t.vlagsoortCodeReden = "Q_B_S_19";
                        }
                        else if (pRecord.CodeMutation == (int)VSM.RUMA.CORE.DB.LABELSConst.CodeMutation.QKrtsvaccH)
                        {
                            t.vlagsoortCodeReden = "Q_H_S_19";
                        }
                        else
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = "DiervlagMeldingRequest: vlagsoortCodeReden onbekend";
                                continue;
                            }
                        }
                        if (herstelmelding > 0)
                        {
                            t.meldingnummerOorsprong = pRecord.MeldingNummer;
                        }

                        l.Add(t);
                    }
                    vlag.diergegevensDiervlagMeldingRequest = l.ToArray();
                    vlag.meldingeenheid = ubn;
                    vlag.relatienummerHouder = brsnummer;

                    IRUtils.writerequest<srMeldingenWS.vastleggenDiervlagMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, vlag);
                    //verzenden
                    var result = m.vastleggenDiervlagMelding(vlag);
                    IRUtils.writerequest<srMeldingenWS.vastleggenDiervlagMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.diergegevensDiervlagMeldingResponse.Count() > 0)
                    {
                        foreach (var resp in result.diergegevensDiervlagMeldingResponse)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == resp.dierLandcode + " " + resp.dierLevensnummer
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).meldnummer  = resp.meldingnummer;
                                if (resp.verwerkingsresultaat.succesIndicator != "J")
                                {
                                    string status = resp.verwerkingsresultaat.succesIndicator;
                                    sls.ElementAt(0).Status = status == "J" ? "G" : string.IsNullOrWhiteSpace(resp.meldingnummer) || resp.meldingnummer == "null" ? "F" : "W";
                                }
                                if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutmelding))
                                {
                                    sls.ElementAt(0).Omschrijving = resp.verwerkingsresultaat.foutmelding;
                                    if (!string.IsNullOrWhiteSpace(resp.verwerkingsresultaat.foutcode))
                                    {
                                        sls.ElementAt(0).Code = resp.verwerkingsresultaat.foutcode;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var pRecord in pRecords)
                        {
                            var sls = from n in slm
                                      where n.Lifenumber == pRecord.Lifenumber
                                      select n;
                            if (sls.Count() > 0)
                            {
                                sls.ElementAt(0).Status = "F";
                                sls.ElementAt(0).Omschrijving = $@"No data returned:{result.actie}";
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRQkoortsV2)}:{exc.ToString()}");
             
                foreach (var s in slm)
                {
                    //when did the error occurr
                    if (string.IsNullOrWhiteSpace(s.meldnummer))
                    {
                        if (s.Status == "G" && string.IsNullOrWhiteSpace(s.Omschrijving))
                        {
                            s.Status = "F";
                            s.Omschrijving = exc.Message;
                        }
                    }
                }
            }

            return slm;
        }

        public void LNVIntrekkenMelding(MUTATION pRecord, string username, string password, int testserver, string ubn, string brsnummer, string meldingsnr, int pProgId, ref string lStatus, ref string lCode, ref string lOmschrijving)
        {
        
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                BasicHttpsBinding binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

                System.ServiceModel.EndpointAddress adr = new System.ServiceModel.EndpointAddress(IRUtils.GetMeldingWSEndpoint(testserver));
                using (srMeldingenWS.MeldingenServiceClient m = new srMeldingenWS.MeldingenServiceClient(binding, adr))
                {

                    m.ClientCredentials.UserName.UserName = username;
                    m.ClientCredentials.UserName.Password = password;

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        lStatus = "F";
                        lOmschrijving = "Username or Password empty.";
                        return;
                    }

                    intrekkenMeldingRequestType intr = new intrekkenMeldingRequestType();
                    intr.meldingeenheid = ubn;
                    intr.meldingnummer = meldingsnr;
                    intr.relatienummerHouder = brsnummer;
                    intr.requestID = $@"{pRecord.Internalnr}";
                 

                    IRUtils.writerequest<intrekkenMeldingRequestType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, intr);
                    //verzenden
                    var result = m.intrekkenMelding(intr);
                    IRUtils.writerequest<intrekkenMeldingResponseType>(ubn, m.Endpoint.Address.Uri.ToString(), tijd, result);
                    if (result.verwerkingsresultaat!=null)
                    {
                    
                        if (result.verwerkingsresultaat.succesIndicator != "J")
                        {
                            string status = result.verwerkingsresultaat.succesIndicator;
                            lStatus = status == "J" ? "G" : "F";
                        }
                        if (!string.IsNullOrWhiteSpace(result.verwerkingsresultaat.foutmelding))
                        {
                            lOmschrijving = result.verwerkingsresultaat.foutmelding;
                            if (!string.IsNullOrWhiteSpace(result.verwerkingsresultaat.foutcode))
                            {
                                lCode = result.verwerkingsresultaat.foutcode;
                            }
                        }
                    }
                    else
                    {
                        lStatus = "F";
                        lOmschrijving = $@"No result returned.";
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(LNVIRQkoortsV2)}:{exc.ToString()}");
                lStatus = "F";
                lOmschrijving = $@"{exc.Message}";
            }

    
        }


    }

    [Serializable]
    public class vlagresultaat
    {
        public DateTime? berichttijd { get; internal set; }
        public string Omschrijving { get; internal set; }

        public List<vlaggegevens> vlaggegevens { get; internal set; }

    }

    [Serializable]
    public class vlaggegevens
    {
        public string datumIngang { get; internal set; }
        public string datumEinde { get; internal set; }

        public int? dierSoort { get; internal set; }

        public string vlagsoortCodeReden { get; internal set; }
        public string vlagsoort { get; internal set; }
        public string dierLandcode { get; internal set; }
        public string dierLevensnummer { get; internal set; }
        public string dierWerknummer { get; internal set; }
 
    }

    [Serializable]
    public class vlagsoortMaatregel
    {
        public string maatregelCode { get; internal set; }
        public string Omschrijving { get; internal set; }
        public string datumIngang { get; internal set; }
        public string datumEinde { get; internal set; }
    }

    [Serializable]
    public class MeldingDetails
    {
        public string berichtdatum { get; internal set; }
        public DateTime? berichttijd { get; internal set; }
        public string berichttype { get; internal set; }
        public string communicatiekanaal { get; internal set; }
        public string datumEinde { get; internal set; }
        public string dierAliasLandcode { get; internal set; }
        public string dierAliasLevensnummer { get; internal set; }
        public int? aantalDieren { get; set; }
        public int? AantalDierenOpBedrijf { get; internal set; }
        public string dierAliasWerknummer { get; internal set; }
        public string dierBestemmingLandcode { get; internal set; }
        public string dierCategorie { get; internal set; }
        public string dierGeslacht { get; internal set; }
        public string geboortedatum { get; internal set; }
        public string gebeurtenisdatum { get; internal set; }
        public string dierWerknummer { get; internal set; }
        public string dierVervangendWerknummer { get; internal set; }
        public string dierVervangendLevensnummer { get; internal set; }
        public string dierVervangendLandcode { get; internal set; }
        public string dierTijdelijkWerknummer { get; internal set; }
        public string dierTijdelijkLevensnummer { get; internal set; }
        public string dierTijdelijkLandcode { get; internal set; }
        public int? diersoort { get; internal set; }
        public string dierPremiestatus { get; internal set; }
        public string dierOorspronkelijkeIdentificatie { get; internal set; }
        public string dierOorsprongLandcode { get; internal set; }
        public string dierLevensnummer { get; internal set; }
        public string dierLandcode { get; internal set; }
        public string dierKalfdatum { get; internal set; }
        public string dierHerkomstLandcode { get; internal set; }
        public string dierHaarkleur { get; internal set; }
        public List<aanvullendeMatch> aanvullendeMatches { get; internal set; }
        public string vlagsoortCodeReden { get; internal set; }
        public DateTime? verwerkingstijd { get; internal set; }
        public verwerkingsresultaatType verwerkingsresultaat { get; internal set; }
        public string verwerkingsdatum { get; internal set; }
        public int? transVerwachteTransportduur { get; internal set; }
        public string transTijdstipVertrek { get; internal set; }
        public string transRelatienummerVervoerder { get; internal set; }
        public decimal? transportnummer { get; internal set; }
        public string transNaamVervoerder { get; internal set; }
        public string transKenteken { get; internal set; }
        public string requestID { get; internal set; }
        public string relatienummerOverdrager { get; internal set; }
        public string relatienummerMelder { get; internal set; }
        public string relatienummerHouder { get; internal set; }
        public string relatienummerAcceptant { get; internal set; }
        public string redenRuiming { get; internal set; }
        public string redenBlokkade { get; internal set; }
        public string nummerGezondheidscertificaat { get; internal set; }
        public string naamMeTweedePartij { get; internal set; }
        public string moederWerknummer { get; internal set; }
        public string moederLevensnummer { get; internal set; }
        public string moederLandcode { get; internal set; }
        public string moederHaarkleur { get; internal set; }
        public string moederGeslacht { get; internal set; }
        public string moederGeboortedatum { get; internal set; }
        public string meldingStatusOms { get; internal set; }
        public string meldingStatusCode { get; internal set; }
        public string meldingnummerMatch { get; internal set; }
        public string meldingnummer { get; internal set; }
        public string meldingeenheidNoodslacht { get; internal set; }
        public string meldingeenheidHerkomst { get; internal set; }
        public string meldingeenheidDestructor { get; internal set; }
        public string meldingeenheidBestemming { get; internal set; }
        public string meldingeenheid { get; internal set; }
        public DateTime? intrektijd { get; internal set; }
        public string intrekdatum { get; internal set; }
        public string indIntrekbaar { get; internal set; }
        public string indHerstelMogelijkheden { get; internal set; }
        public string importdatumCIS { get; internal set; }
        public DateTime? hersteltijd { get; internal set; }
        public string herstelMeldingnummerVorig { get; internal set; }
        public string herstelMeldingnummerVolgend { get; internal set; }
        public string herstelIndicator { get; internal set; }
        public string hersteldatum { get; internal set; }
        public string groepsgegevens { get; internal set; }
    }

    [Serializable]
    public class aanvullendeMatch
    {
        public string meldingnummerEigen { get; internal set; }
        public string meldingnummerTweedePartij { get; internal set; }
    }

    public class Soaplogmelding : SOAPLOG
    {
        public string meldnummer { get; set; }
    }
}
