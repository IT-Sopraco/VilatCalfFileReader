using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;

using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;
//using vsm.ruma.navbullsearch;

using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.SOAPLNV;

namespace VSM.RUMA.CORE
{
    public class MutationUpdater
    {
        //BUG 1645 zie : VSM.RUMA.SRV.IRMUT class LNVIRmeldingenRaadplegen
        //Bij aanpassingen deze ook aanpassen iom luc
        static readonly object padlock = new object();
        private int TestServer;
        private XDocument xBerichtenbestand;
        private const int ChangedBy = 15; //IRMeldingenRaadplegen
        public event EventHandler RequestUpdate;
        private string lLogFilePath;
        public int TasklogID { get; set; }
        public int? SoapKind { get; set; }
        protected void OnRequestUpdate(object sender, MovFuncEvent e)
        {
            if (RequestUpdate != null)
                RequestUpdate(sender, e);
        }

        public MutationUpdater(XDocument pxBerichtenbestand, int pTestserver, string pLogfile)
        {
            TestServer = pTestserver;
            if (pxBerichtenbestand == null)
            { xBerichtenbestand = new XDocument(new XElement("root")); }
            else
            {
                xBerichtenbestand = pxBerichtenbestand;
            }
            lLogFilePath = pLogfile;
        }
 

        private List<COUNTRY> countrycodes;
        public SOAPLOG DoeIRMeldingenRaadplegen(UserRightsToken pToken, BEDRIJF farm, String lLevensnr, int MeldingType, int MeldingStatus,
                                            String UBNnr2ePartijd, DateTime Begindatum, DateTime Einddatum)
        {
            SOAPLOG Result = null;
            UserRightsToken lToken = (UserRightsToken)pToken.Clone();
            // Parameter MeldingStatus = 2 wordt gebruikt om ontbrekende meldingen op te halen.
         
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pToken);
            DBSelectQueries DBSelect = Facade.GetInstance().getSlave(lToken);
            Feedadviceer FaAdviser = new Feedadviceer();
            int pIndGebeurtenisdatum = 0;
            UBN farmUBN = lMstb.GetubnById(farm.UBNid);
            THIRD farmThird = lMstb.GetThirdByThirId(farmUBN.ThrID);
            FARMCONFIG fcon = lMstb.getFarmConfig(farm.FarmId, "rfid", "1");
            writeLog("De soap log zit in LOG/IenR/LNV2IRSOAP_Meldingen_" + farmUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log");
            writeLog("Start:" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + " Parameters: FarmID:" + farm.FarmId.ToString() + " lLevensnr:" + lLevensnr + " MeldingType:" + MeldingType.ToString());
            writeLog(" MeldingStatus:" + MeldingStatus.ToString() + " UBNnr2ePartijd:" + UBNnr2ePartijd + " Begindatum:" + Begindatum.ToString() + " Einddatum:" + Begindatum.ToString());
            unLogger.WriteDebug("DoeIRMeldingenRaadplegen:" + Begindatum.ToString("dd-MM-yyyy") + " TOT " + Einddatum.ToString("dd-MM-yyyy"));

          
            String Output = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_" + farm.UBNid + DateTime.Now.Ticks + ".csv";
            Result = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(lToken, farm.UBNid, farm.ProgId, farm.Programid, lLevensnr, MeldingType, MeldingStatus,
                                                  UBNnr2ePartijd, Begindatum, Einddatum, pIndGebeurtenisdatum, Output);

            writeLog("Code:" + Result.Code + " Status:" + Result.Status + " Omschrijving:" + Result.Omschrijving);
            string[] Kols = { "Levensnummer", "Meldingtype", "Datum", "ubn2ePartij", "meldingsnummer", "meldingstatus", "hersteld", "naam2ePartij" };
            Result.TaskLogID = TasklogID > 0 ? TasklogID : Result.TaskLogID;
            if (Result.Kind <= 0)
            {
                Result.Kind = SoapKind.HasValue ? SoapKind.Value : (int)DB.LABELSConst.SOAPLOGKind.LNV_Meldingen_Raadplegen;
            }
            char spl = ';';
            DataTable tblLNV = utils.GetCsvData(Kols, Output, spl, "Mutaties");
            try
            {
                tblLNV.DefaultView.Sort = "Levensnummer,Datum,meldingsnummer";//Chronologisch volgens RVO
                tblLNV = tblLNV.DefaultView.ToTable(true);//http://stackoverflow.com/questions/5494698/sorting-and-updating-datatable-not-producing-expected-results
            }
            catch { }
            if (string.IsNullOrEmpty(Result.Omschrijving))
            {
                Result.Omschrijving = $@"Aantal Meldingen:{tblLNV.Rows.Count}";
            }
            else 
            {
                Result.Omschrijving += $@" Aantal Meldingen:{tblLNV.Rows.Count}";
            }
            lMstb.WriteSoapError(Result);
            string Thr_Brs_Number = "";
            if (!string.IsNullOrEmpty(farmThird.Thr_Brs_Number))
            {
                if (Convert.ToBoolean(lMstb.GetConfigValue(farm.Programid, farm.FarmId, "add_ubns_with_same_Brs_Number", "false")))
                {
                    var tblextra = lMstb.QueryData($@"SELECT u.*,b.* FROM agrofactuur.UBN u
                        JOIN agrofactuur.BEDRIJF b ON b.UbnID=u.UbnID
                        JOIN agrofactuur.THIRD t ON t.Thrid=u.thrid
                        WHERE t.Thr_Brs_Number='{farmThird.Thr_Brs_Number}' AND NOT u.UbnID={farmUBN.UBNid} ");
                    if (tblextra.Rows.Count > 0)
                    {
                        try
                        {
                            foreach (DataRow r in tblextra.Rows)
                            {
                                BEDRIJF b2 = new BEDRIJF();
                                UBN u2 = new UBN();
                                lMstb.FillObject(b2, r);
                                lMstb.FillObject(u2, r);
                                var Output2 = unLogger.getLogDir("CSVData") + farm.Programid + "_" + u2.Bedrijfsnummer + "_" + u2.UBNid + DateTime.Now.Ticks + ".csv";

                                var Result2 = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(lToken, b2.UBNid, b2.ProgId, farm.Programid, lLevensnr, MeldingType, MeldingStatus,
                                                            UBNnr2ePartijd, Begindatum, Einddatum, pIndGebeurtenisdatum, Output2);
                                DataTable tblLNV2 = utils.GetCsvData(Kols, Output2, spl, "Mutaties");

                                if (tblLNV2.Rows.Count > 0)
                                {
                                    foreach (DataRow nr in tblLNV2.Rows)
                                    {
                                        DataRow r2 = tblLNV.NewRow();
                                        foreach (string kol in Kols)
                                        {
                                            r2[kol] = nr[kol];
                                        }
                                        int lMeldingType2 = Convert.ToInt32(r2["Meldingtype"]);
                                        //Births only from the other administration
                                        if (lMeldingType2 == 3 || lMeldingType2 == 4)
                                        {
                                            tblLNV.Rows.Add(r2);
                                            Thr_Brs_Number = farmThird.Thr_Brs_Number;
                                        }
                                    }
                                }
                            }

                            tblLNV.DefaultView.Sort = "Levensnummer,Datum,meldingsnummer";//Chronologisch volgens RVO
                            tblLNV = tblLNV.DefaultView.ToTable(true);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError($@"Error Extra Births same Brsnumber: {farmThird.Thr_Brs_Number} {exc}");
                        }
                    }
                }
            }
            MovFuncEvent b = new MovFuncEvent();
            if (tblLNV.Rows.Count == 0)
            {
                if (RequestUpdate != null)
                {

                    b.Progress = 100;
                    if (Result.Code == "IRD-00364")
                    {
                        b.Message = " Klaar Er zijn geen dieren verwerkt, want zijn meer dan 2500 meldingen gevonden, probeer een kortere periode.";
                        RequestUpdate(this, b);
                    }
                    else
                    {
                        if (Result.Omschrijving != "")
                        {
                            b.Message = " Klaar Er zijn geen dieren verwerkt. " + Result.Omschrijving;
                            RequestUpdate(this, b);
                        }
                        else
                        {
                            b.Message = " Klaar Er zijn geen meldingen gevonden voor deze periode. ";
                            RequestUpdate(this, b);
                        }
                    }
                }
            }
            else
            {
                //while (Result.Code == "IRD-00364" && tblLNV.Rows.Count > 0)
                //{
                //    Facade.GetInstance().getSaveToDB(pToken).WriteSoapError(Result);
                //    DataRow Row = tblLNV.AsEnumerable().OrderBy(row => row["Datum"]).First();
                //    DateTime TussenDatum = LNVAlgutils.getDateLNV(Row["Datum"].ToString());
                //    Result = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(pToken, farm.UBNid, farm.ProgId, lLevensnr, MeldingType, MeldingStatus,
                //                                     UBNnr2ePartijd, Begindatum, TussenDatum, Output);

                //}
                int teller = 0;
                int totaal = tblLNV.Rows.Count;

                foreach (DataRow row in tblLNV.Rows)
                {
                    teller += 1;
                    int procent = teller * 100 / totaal;
                    if (RequestUpdate != null)
                    {

                        b.Progress = procent;
                        b.Message = VSM_Ruma_OnlineCulture.getStaticResource("verwerken", "Verwerken") + " " + row["Levensnummer"].ToString();
                        RequestUpdate(this, b);
                    }

                  
                    int lMeldingType = Convert.ToInt32(row["Meldingtype"]);
                    int lStatusMelding = Convert.ToInt32(row["meldingstatus"]);
                    DateTime MutDate = utils.getDateLNV(row["Datum"].ToString());
                    //TIJDELIJKE FIX IVM RVO BUG
                    //if (MutDate >= Begindatum.Date && MutDate <= Einddatum.Date)
                    //{
                    String UBN2e = row["ubn2ePartij"].ToString();
                    string meldingsnummer = row["meldingsnummer"].ToString();
                    int lHersteld = Convert.ToInt32(row["hersteld"]);

                    writeLog("Verwerken:" + row["Levensnummer"].ToString() + " Meldingtype:" + lMeldingType.ToString() + " Datum:" + MutDate.ToString());
                    writeLog("ubn2ePartij:" + UBN2e + " meldingsnummer:" + meldingsnummer + " meldingstatus:" + lStatusMelding.ToString());
                    int[] aStatusMeldingen = { 0, 1, 2, 4, 5 };
                    SOAPLOG detail = new SOAPLOG
                    {
                        Changed_By = ChangedBy,
                        Code = Result.Code,
                        Date = DateTime.Now.Date,
                        FarmNumber = farmUBN.Bedrijfsnummer,
                        Kind = (int)LABELSConst.SOAPLOGKind.LNV_Meldingen_Raadplegen_Details,
                        Lifenumber = row["Levensnummer"].ToString(),
                        Omschrijving = "",
                        TaskLogID = Result.TaskLogID,
                        SourceID = Result.SourceID,
                        Time = DateTime.Now,
                        Status = "G"
                    };
                    if (aStatusMeldingen.Contains(lStatusMelding) && lHersteld == 0) // = Definitief en Voorlopig geregistreerd
                    {
                        // Parameter MeldingStatus = 2 wordt gebruikt om ontbrekende meldingen op te halen.
                        // Hierbij moet een afvoer een aanvoer worden.
                        if (MeldingStatus == 2 && UBNnr2ePartijd != String.Empty)
                        {
                            if (lMeldingType == 1)
                                lMeldingType = 2;
                            else if (lMeldingType == 2)
                                lMeldingType = 1;
                        }


                        ANIMAL ani = lMstb.GetAnimalByLifenr(row["Levensnummer"].ToString());
                        if (farm.ProgId == 3 || farm.ProgId == 5)
                        {
                            ani = lMstb.GetAnimalByAniAlternateNumber(row["Levensnummer"].ToString());
                        }

                        #region TESTING ?
                        bool ff = false;


                        if (ani.AniAlternateNumber == "NL 870923027" || ani.AniAlternateNumber == "NL 735917295")
                        {
                            ff = true;
                        }
                        if (ff)
                        {
                            writeLog(ani.AniAlternateNumber);
                        }
                        #endregion

                        if (ani.AniId <= 0)
                        {
                            String Lifenr = row["Levensnummer"].ToString();
                            VulDiergegevensvanuitLNV(lToken, farm, ani, Lifenr);
                        }
                        ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        anicat.Changed_By = ChangedBy;
                        anicat.SourceID = farmUBN.ThrID;
                        if (anicat.FarmId == 0 || anicat.AniId == 0)
                        {
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            anicat.UbnId = farm.UBNid;
                            anicat.Anicategory = 5;
                        }
                        if (anicat.Anicategory == 0)
                        {
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            anicat.UbnId = farm.UBNid;
                            anicat.Anicategory = 5;
                        }
                        if (anicat.AniWorknumber == "")
                        {
                            string worknr = "";
                            string tempnr = "";
                            ANIMAL vader = new ANIMAL();
                            Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, ani.AniAlternateNumber, vader, 0, out worknr, out tempnr);
                            anicat.AniWorknumber = worknr;
                        }

                        MOVEMENT mov;
                        EVENT eve;
                        if ((lMeldingType == 1 || lMeldingType == 5) && farm.Programid == 16)
                        {
                            string Query = @"SELECT ac.*  FROM 
                                                        agrobase_sheep.ANIMALCATEGORY  ac
                                                        JOIN agrobase_sheep.ANIMAL a ON a.AniID=ac.AniID
                                                        JOIN agrofactuur.BEDRIJF b ON b.FarmId=ac.FarmID
                                                        where a.AniAlternateNumber='" + ani.AniAlternateNumber + "' AND b.Programid IN (16,160)";
                            DataTable tblBeltes = DBSelect.QueryData(Query);
                            if (tblBeltes.Rows.Count == 0)
                            {
                                unLogger.WriteDebug("IRMUT", "Onbrekende Aanvoer : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                                unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                                unLogger.WriteError("IRMUT", " Het dier kan niet aangevoerd worden want het is niet bekend in het TES stamboek, De houder dient het afstammingsbewijs naar het secretariaat te sturen voordat de diergegevens verwerkt kunnen worden");
                                MovFunc.onbekendstamboekdierBeltesMessage(lToken, farmUBN.ThrID, ChangedBy, farmUBN.Bedrijfsnummer, ani.AniAlternateNumber);
                                detail.Status = "W";
                                detail.Omschrijving = "Beltes: is niet bekend in het TES stamboek";
                                lMstb.WriteSoapError(detail);
                                continue;
                            }
                        }


                        anicat.Changed_By = (int)LABELSConst.ChangedBy.LNVIRmeldingenRaadplegen_DoeIRMeldingenRaadplegen;
                        anicat.SourceID = farm.UBNid;

                        switch (lMeldingType)
                        {
                            case 1: // Aanvoer                                      

                                anicat.Anicategory = 1;
                                detail.Omschrijving = "Buying:";
                                mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                                bool IRAanvoer = Convert.ToBoolean(lMstb.GetConfigValue(farm.Programid, farm.FarmId, "IRAanvoerAutoRecieve", "True"));
                                if (mov.MovId == 0 && IRAanvoer)
                                {
                                    detail.Omschrijving = "New Buying";
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Aanvoer : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;

                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(pToken, MutDate, "", ref mov);
                                    mov.MovKind = 1;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    if (UBN2e != String.Empty)
                                    {
                                        UBN ubnfrom = lMstb.getUBNByBedrijfsnummer(UBN2e);
                                        mov.MovThird_UBNid = ubnfrom.UBNid;
                                        mov.ThrId = ubnfrom.ThrID;
                                    }
                                    lMstb.SaveMovement(mov);
                                    BUYING buy = new BUYING();
                                    buy.MovId = mov.MovId;
                                    lMstb.SaveBuying(buy);
                                    if (row["meldingstatus"].ToString() == "2" && UBN2e != String.Empty)
                                    {
                                        //FARMCONFIG fcIR = lMstb.getFarmConfig(farm.FarmId, "IRaanvoer", "True");
                                        //MovFunc.saveAanVoerMutation(pToken, farm, fcIR, mov, buy, ani.AniCountryCodeDepart, ani.AniCountryCodeBirth);
                                    }
                                    else mov.ReportDate = MutDate;
                                    writeLog("Aanvoer MovId:" + mov.MovId.ToString());


                                    MovFunc.SetSaveAnimalCategory(lToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
 
                                }
                               
                               
                                lMstb.WriteSoapError(detail);
                                checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);

                                break;
                            case 2: // Afvoer
                                anicat.Anicategory = 4;
                                //doDetail(pToken, farmUBN, farm, farmThird, meldingsnummer);
                                mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                                detail.Omschrijving = $@"Sale:{MutDate}";
                                if (mov.MovId == 0)
                                {
                                    detail.Omschrijving = $@"New Sale:{MutDate}";
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Afvoer : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", UBN2e));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;

                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.MovKind = 2;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    if (UBN2e != String.Empty)
                                    {
                                        UBN ubnto = lMstb.getUBNByBedrijfsnummer(UBN2e);
                                        mov.MovThird_UBNid = ubnto.UBNid;
                                        mov.ThrId = ubnto.ThrID;
                                    }
                                    lMstb.SaveMovement(mov);
                                    SALE sal = new SALE();
                                    sal.MovId = mov.MovId;
                                    lMstb.SaveSale(sal);

                                    if (row["meldingstatus"].ToString() == "2" && UBN2e != String.Empty)
                                    {
                                        //MovFunc.saveAfvoerMutation(lToken, farm, ani, mov, sal);
                                    }
                                    else
                                    {
                                        mov.ReportDate = MutDate;
                                    }

                                    writeLog("Afvoer MovId:" + mov.MovId.ToString());
                                    MovFunc.SetSaveAnimalCategory(lToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                                    MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                }
                                lMstb.WriteSoapError(detail);
                                break;
                            case 3: // Geboorte
                                detail.Omschrijving = $@"Birth:{MutDate}";
                                anicat.Anicategory = 1;
                                UBN Fokker = lMstb.GetubnById(farm.UBNid);
                                detail.Omschrijving = $@"Birth:{MutDate}";
                                if (ani.ThrId == 0 && ani.AniId > 0)
                                {
                                    ANIMAL check = lMstb.GetAnimalById(ani.AniId);
                                    check.ThrId = Fokker.ThrID;
                                    check.Changed_By = ChangedBy;
                                    check.SourceID = farmUBN.ThrID;
                                    lMstb.UpdateANIMAL(Fokker.ThrID, check);
                                }
                                ANIMAL dam = null;
                                if (ani.AniIdDam > 0)
                                {
                                    dam = lMstb.GetAnimalById(ani.AniIdDam);
                                }
                                else if (ani.AniIdMother > 0)
                                {
                                    dam = lMstb.GetAnimalById(ani.AniIdMother);
                                }

                                if (dam == null || dam.AniId <= 0)
                                {
                                    unLogger.WriteInfo("IRMUT", "Onbrekende Geboorte, moedergegevens aanvullen vanuit RVO");
                                    String LevnrMoeder = VSM.RUMA.CORE.SOAPLNV.OpvragenLNVDierDetailsV2.GetDraagmoedervanuitLNV(farm.ProgId, ani.AniAlternateNumber);
                                    if (LevnrMoeder.Trim() != String.Empty)
                                    {
                                        unLogger.WriteInfo("IRMUT", "Onbrekende Geboorte, GetDraagmoedervanuitLNV RVO: " + LevnrMoeder);
                                        dam = lMstb.GetAnimalByAniAlternateNumber(LevnrMoeder);
                                        if (dam.AniId > 0)
                                        {
                                            ani.AniIdMother = dam.AniId;
                                            lMstb.UpdateANIMAL(Fokker.ThrID, ani);

                                            ani.Changed_By = (int)LABELSConst.ChangedBy.LNVIRmeldingenRaadplegen_DoeIRMeldingenRaadplegen;
                                        }
                                        else
                                        {
                                            VulDiergegevensvanuitLNV(lToken, farm, dam, LevnrMoeder);
                                        }
                                    }
                                    else { unLogger.WriteInfo("IRMUT", "Onbrekende Geboorte, GetDraagmoedervanuitLNV RVO geen resultaat"); }
                                    if (dam == null || dam.AniId <= 0)
                                    {

                                        //UBN Fokker = Facade.GetInstance().getSaveToDB(pToken).GetubnById(farm.UBNid);
                                        unLogger.WriteInfo("IRMUT", "Onbrekende Geboorte ZONDER MOEDERDIER! : ");
                                        unLogger.WriteInfo("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                        unLogger.WriteInfo("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                        unLogger.WriteInfo("IRMUT", String.Format("UBN: {0}", Fokker.Bedrijfsnummer));
                                        //ani.ThrId = Fokker.ThrID;
                                        writeLog("Onbrekende Geboorte ZONDER MOEDERDIER! ");
                                        break;
                                    }
                                }

                                ANIMALCATEGORY acm = lMstb.GetAnimalCategoryByIdandFarmid(dam.AniId, farm.FarmId);
                                acm.Changed_By = ChangedBy;
                                acm.SourceID = farmUBN.ThrID;
                                if (acm.FarmId == 0)
                                {
                                    acm.FarmId = farm.FarmId;
                                    acm.AniId = dam.AniId;
                                    acm.Anicategory = 1;


                                    acm.Changed_By = (int)LABELSConst.ChangedBy.LNVIRmeldingenRaadplegen_DoeIRMeldingenRaadplegen;
                                    acm.SourceID = farm.UBNid;

                                    lMstb.SaveAnimalCategory(acm);
                                }
                                if (acm.AniWorknumber == "")
                                {
                                    string worknr = "";
                                    string tempnr = "";
                                    ANIMAL vader = new ANIMAL();

                                    Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, dam.AniAlternateNumber, vader, 0, out worknr, out tempnr);
                                    acm.AniWorknumber = worknr;

                                    lMstb.SaveAnimalCategory(acm);
                                }
                                checkForMsOptimaboxRespondernumber(lToken, farm, dam, false);

                                eve = lMstb.GetEventByDateAniIdKind(MutDate, dam.AniId, 5);
                                if (eve.EventId == 0)
                                {
                                    List<EVENT> births = lMstb.getEventsByAniIdKind(dam.AniId, 5);
                                    foreach (EVENT evebirth in births)
                                    {
                                        BIRTH bir = lMstb.GetBirth(evebirth.EventId);
                                        if (bir.CalfId == ani.AniId)
                                        {
                                            eve = evebirth;
                                            break;
                                        }

                                    }
                                }
                                detail.Omschrijving = $@"Birth for:{dam.AniAlternateNumber}:{MutDate}";
                                if (eve.EventId == 0)
                                {
                                    detail.Omschrijving = $@"New Birth for:{dam.AniAlternateNumber}:{MutDate}";
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Geboorte : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                                    eve.Changed_By = ChangedBy;
                                    eve.SourceID = farmUBN.ThrID;
                                    eve.EveMutationBy = 15;
                                    eve.EveMutationDate = DateTime.Today;
                                    eve.EveMutationTime = DateTime.Now;
                                    eve.AniId = dam.AniId;

                                    eve.EveDate = MutDate;
                                    Event_functions.handleEventTimes(ref eve, MutDate);
                                    eve.EveKind = 5;
                                    eve.UBNId = farm.UBNid;
                                    eve.happened_at_FarmID = anicat.FarmId;
                                    eve.EveOrder = Event_functions.getNewEventOrder(lToken, MutDate.Date, 5, farm.UBNid, dam.AniId);

                                    lMstb.SaveEvent(eve);
                                    BIRTH bir = new BIRTH();
                                    bir.EventId = eve.EventId;
                                    bir.CalfId = ani.AniId;
                                    bir.BirNumber = Event_functions.getOrCreateBirnr(lToken, dam.AniId, MutDate);
                                    bir.Meldnummer = meldingsnummer;
                                    bir.Changed_By = ChangedBy;
                                    bir.SourceID = farmUBN.ThrID;

                                    lMstb.SaveBirth(bir);
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                    checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                    writeLog("Geboorte EventId:" + eve.EventId.ToString());
                                
                                }
                                FaAdviser.saveKetoBoxFeedAdvices(lToken, dam.AniId, eve.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                                lMstb.WriteSoapError(detail);
                                break;
                            case 4: // Doodgeboren
                                //Dit is geen doodmelding maar een Doodgeboren melding 
                                //Dus er moet een doodgeboren calf aan dit dier gekoppeld worden.
                                unLogger.WriteDebug("IRMUT", "Onbrekende Doodgeboren : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("UBN : {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                                StringBuilder bld = new StringBuilder();
                                bld.Append(" SELECT EVENT.* FROM EVENT LEFT JOIN BIRTH ");
                                bld.Append(" ON BIRTH.EventId = EVENT.EventId ");
                                bld.AppendFormat(" WHERE EVENT.AniId={0} AND BIRTH.BornDead=1", ani.AniId);
                                bld.Append(" AND EveKind = 5 AND EVENT.EventId>0 ");
                                bld.Append(" AND date_format(EveDate,'%Y-%m-%d') = '" + MutDate.ToString("yyyy-MM-dd") + "'");
                                bld.AppendFormat(" AND  UBNId = {0} ", farm.UBNid);
                                DataTable tbl = lMstb.GetDataBase().QueryData(lToken.getLastChildConnection(), new DataSet(), bld, "Event", MissingSchemaAction.Add);
                                EVENT doodgeboren = new EVENT();
                                try
                                {
                                    if (tbl.Rows.Count > 0)
                                    {
                                        lMstb.GetDataBase().FillObject(doodgeboren, tbl.Rows[0]);
                                    }
                                }
                                catch { }
                                detail.Omschrijving = $@"Borndeath :{MutDate}";
                                if (doodgeboren.EventId == 0)
                                {
                                    detail.Omschrijving = $@"New Borndeath :{MutDate}";
                                    BIRTH newBirth = new BIRTH();
                                    Event_functions.handleEventTimes(ref doodgeboren, MutDate);
                                    doodgeboren.EveKind = 5;
                                    doodgeboren.AniId = ani.AniId;
                                    doodgeboren.Changed_By = ChangedBy;
                                    doodgeboren.SourceID = farmUBN.ThrID;
                                    doodgeboren.EveMutationBy = 15;
                                    doodgeboren.EveOrder = Event_functions.getNewEventOrder(lToken, MutDate.Date, 5, farm.UBNid, ani.AniId);
                                    doodgeboren.happened_at_FarmID = anicat.FarmId;
                                    doodgeboren.ThirdId = farmUBN.ThrID;
                                    doodgeboren.UBNId = farm.UBNid;
                                    newBirth.BornDead = 1;
                                    newBirth.BirNumber = Event_functions.getOrCreateBirnr(lToken, ani.AniId, MutDate.Date);
                                    if (row["meldingsnummer"] != DBNull.Value)
                                    {
                                        newBirth.Meldnummer = row["meldingsnummer"].ToString();
                                    }
                                    List<int> lNlingEventsIds = lMstb.getNlingCheckEventIds(farm.UBNid, ani.AniId, newBirth.BirNumber);
                                    newBirth.Nling = lNlingEventsIds.Count() + 1;
                                    if (lMstb.SaveEvent(doodgeboren))
                                    {
                                        newBirth.EventId = doodgeboren.EventId;
                                        lMstb.SaveBirth(newBirth);
                                        writeLog("Doodgeboren EventId:" + doodgeboren.EventId.ToString());
                                    }
                                }
                                checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                FaAdviser.saveKetoBoxFeedAdvices(lToken, ani.AniId, doodgeboren.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                                lMstb.WriteSoapError(detail);
                                break;
                            case 5: // Import
                                anicat.Anicategory = 1;
                                mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                                detail.Omschrijving = $@"Import :{MutDate}";
                                if (mov.MovId == 0)
                                {
                                    detail.Omschrijving = $@"New Import :{MutDate}";
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Import : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.ReportDate = MutDate;
                                    mov.MovKind = 1;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 1, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = farm.FarmId;
                                    if (UBN2e != String.Empty)
                                    {
                                        UBN ubnfrom = lMstb.getUBNByBedrijfsnummer(UBN2e);
                                        mov.MovThird_UBNid = ubnfrom.UBNid;
                                        mov.ThrId = ubnfrom.ThrID;
                                    }
                                    lMstb.SaveMovement(mov);
                                    BUYING buy = new BUYING();
                                    buy.PurKind = 1;
                                    buy.MovId = mov.MovId;
                                    lMstb.SaveBuying(buy);
                                    writeLog("Import MovId:" + mov.MovId.ToString());

                                    MovFunc.SetSaveAnimalCategory(lToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);

                                    checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                    
                                }
                                lMstb.WriteSoapError(detail);
                                break;
                            case 6: // Export
                                anicat.Anicategory = 4;
                                mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                                detail.Omschrijving = $@"Export :{MutDate}";
                                if (mov.MovId == 0)
                                {
                                    detail.Omschrijving = $@"New Export :{MutDate}";
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Export : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", UBN2e));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.ReportDate = MutDate;
                                    mov.MovKind = 2;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    if (UBN2e != String.Empty)
                                    {
                                        UBN ubnto = lMstb.getUBNByBedrijfsnummer(UBN2e);
                                        mov.MovThird_UBNid = ubnto.UBNid;
                                        mov.ThrId = ubnto.ThrID;
                                    }
                                    lMstb.SaveMovement(mov);
                                    SALE sal = new SALE();
                                    sal.MovId = mov.MovId;
                                    sal.SalKind = 1;

                                    sal.Changed_By = ChangedBy;
                                    sal.SourceID = farmUBN.ThrID;

                                    lMstb.SaveSale(sal);

                                    MovFunc.SetSaveAnimalCategory(lToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);

                                    MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                    writeLog("Export MovId:" + mov.MovId.ToString());
                                }
                                lMstb.WriteSoapError(detail);
                                break;
                            case 7: // Dood
                                detail.Omschrijving = $@"Death :{MutDate}";
                                anicat.Anicategory = 4;
                                mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
                                if (mov.MovId == 0)
                                {
                                    detail.Omschrijving = $@"New Death :{MutDate}";
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Doodmelding : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.ReportDate = MutDate;
                                    mov.MovKind = 3;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 3, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    if (UBN2e != String.Empty)
                                    {
                                        UBN ubnto = lMstb.getUBNByBedrijfsnummer(UBN2e);
                                        mov.MovThird_UBNid = ubnto.UBNid;
                                        mov.ThrId = ubnto.ThrID;
                                    }
                                    lMstb.SaveMovement(mov);
                                    LOSS loss = new LOSS();
                                    loss.MovId = mov.MovId;
                                    lMstb.SaveLoss(loss);
                                    MovFunc.SetSaveAnimalCategory(lToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                                    writeLog("Dood MovId:" + mov.MovId.ToString());
                                    List<ANIMALCATEGORY> allcats = lMstb.GetAnimalCategoryById(anicat.AniId);
                                    foreach (ANIMALCATEGORY cat in allcats)
                                    {
                                        if (cat.Anicategory < 4)
                                        {
                                            cat.Anicategory = 4;
                                            cat.Changed_By = ChangedBy;
                                            cat.SourceID = farmUBN.ThrID;

                                            lMstb.SaveAnimalCategory(cat);
                                        }
                                    }
                                    MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                }
                                lMstb.WriteSoapError(detail);
                                break;


                            case 8: // Diervlag             
                                unLogger.WriteDebug("IRMUT", "Onbrekende Diervlag : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                                detail.Omschrijving = $@"Missing Flag :{MutDate}";
                                lMstb.WriteSoapError(detail);
                                break;
                            case 9: // Omnummering             
                                unLogger.WriteDebug("IRMUT", "Onbrekende Omnummering : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                                detail.Omschrijving = $@"Missing Number: {MutDate}";
                                String MeldingNummer = row["meldingsnummer"].ToString();
                                int lDetailMeldingType = 9;
                                int lDetailMeldingStatus = lStatusMelding;
                                DateTime pGebeurtenisDatum = MutDate;
                                DateTime pHerstelDatum = DateTime.MinValue;
                                DateTime pIntrekDatum = DateTime.MinValue;
                                String lLevensnr_Oud = row["Levensnummer"].ToString();
                                String lLevensnr_Nieuw = String.Empty;
                                //Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
                                int progid = farm.ProgId;
                                int MaxString = 255;
                                String lUsername = "";
                                String lPassword = "";

                                lLevensnr = "";
                                String lStatus = string.Empty;
                                String lCode = string.Empty;
                                String lOmschrijving = string.Empty;

                                FTPUSER fusoap = lMstb.GetFtpuser(farmUBN.UBNid, farm.Programid, farm.ProgId, 9992);

                                if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)// && LNVPasswordCheck(fusoap.UserName, fusoap.Password) == 1)
                                {

                                    lUsername = fusoap.UserName;
                                    lPassword = fusoap.Password;
                                }


                                String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MeldingDetailsV2_" + farmUBN.Bedrijfsnummer + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";

                                MeldingenWS slv = new MeldingenWS();
                                MeldingDetails Resultdetail = new MeldingDetails();

                                slv.raadplegenMeldingDetail(lUsername, lPassword, 0,
                                     farmUBN.Bedrijfsnummer, farmThird.Thr_Brs_Number, MeldingNummer,
                                     ref progid, ref lDetailMeldingType, ref lDetailMeldingStatus,
                                     ref MutDate, ref pHerstelDatum, ref pIntrekDatum,
                                     ref lLevensnr_Oud, ref lLevensnr_Nieuw, ref Resultdetail,
                                     ref lStatus, ref lCode, ref lOmschrijving);

                                //DLLcall.LNVIRRaadplegenMeldingDetailsV2(lUsername, lPassword, 0,
                                //     farmUBN.Bedrijfsnummer, farmThird.Thr_Brs_Number, MeldingNummer,
                                //     ref progid, ref lDetailMeldingType, ref lDetailMeldingStatus,
                                //     ref MutDate, ref pHerstelDatum, ref pIntrekDatum,
                                //     ref lLevensnr_Oud, ref  lLevensnr_Nieuw,
                                //     ref lStatus, ref lCode, ref lOmschrijving,
                                //     LogFile, MaxString);
                                //DLLcall.Dispose();

                                if (lLevensnr_Nieuw != null && lLevensnr_Nieuw.Length > 0)
                                {
                                    if (lLevensnr_Oud != null && lLevensnr_Oud.Length > 0)
                                    {
                                        ANIMAL aniNieuw = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Nieuw);
                                        ANIMAL aniOud = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Oud);
                                        if (farm.ProgId != 3 && farm.ProgId != 5)
                                        {
                                            aniNieuw = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Nieuw);
                                            aniOud = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Oud);
                                        }

                                        if (aniNieuw.AniId > 0 && aniOud.AniId > 0)
                                        {
                                            if (aniNieuw.AniId != aniOud.AniId)
                                            {
                                                writeLog("ANIMAL_ERROR omnummer verandering AniId:" + aniNieuw.AniId.ToString() + " " + aniNieuw.AniAlternateNumber + " naar AniId:" + aniOud.AniId.ToString() + " " + aniOud.AniAlternateNumber);
                                                lMstb.InsertAnimalError(aniNieuw.AniId, aniOud.AniId);
                                            }

                                        }
                                        else if (aniNieuw.AniId > 0)
                                        {
                                            List<LEVNRMUT> muta = lMstb.getLevNrMuts(aniNieuw.AniId);
                                            var c = from n in muta
                                                    where n.LevnrOud == lLevensnr_Oud
                                                    select n;
                                            if (c.Count() == 0)
                                            {
                                                detail.Omschrijving = $@"New Missing Number {lLevensnr_Nieuw} : {MutDate}";
                                                LEVNRMUT lnM = new LEVNRMUT();
                                                lnM.Aniid = aniNieuw.AniId;
                                                lnM.Datum = MutDate;
                                                lnM.LevnrNieuw = lLevensnr_Nieuw;
                                                lnM.LevnrOud = lLevensnr_Oud;
                                                lnM.LNVMeldNr = MeldingNummer;

                                                lnM.Changed_By = ChangedBy;
                                                lnM.SourceID = farmUBN.SourceID;

                                                lMstb.saveLevNrMut(lnM);
                                            }

                                        }
                                        else if (aniOud.AniId > 0)
                                        {
                                            List<LEVNRMUT> muta = lMstb.getLevNrMuts(aniOud.AniId);
                                            detail.Omschrijving = $@"New Missing Number {lLevensnr_Nieuw} : {MutDate}";
                                            MovFunc.NummerDierOm(lToken, farm, farmUBN, aniOud, lLevensnr_Nieuw, MeldingNummer, ChangedBy, farmUBN.ThrID);

                                        }

                                    }
                                }

                                //Result = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingDetailsV2(pToken, farm.UBNid, farm.ProgId,
                                //                 MeldingNummer, lDetailMeldingType, lDetailMeldingStatus,
                                //                 Begindatum, lLevensnr_Oud, lLevensnr_Nieuw);

                                lMstb.WriteSoapError(detail);
                                break;
                            default:
                                String Lifenr = row["Levensnummer"].ToString();
                                unLogger.WriteInfo(String.Format("Onbekende Melding van RVO : Datum {0}  Levensnr : {1} LNVType : {2}", MutDate, Lifenr, lMeldingType));
                                detail.Omschrijving = $@"Default Type:{lMeldingType}: {MutDate}";
                                lMstb.WriteSoapError(detail);
                                continue;

                        }
                        MovFunc.SetSaveAnimalCategory(lToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                        writeLog("Anicategory:" + anicat.Anicategory.ToString());

                    }
                    else if (lStatusMelding == 6) // ingetrokkken meldingen
                    {
                        detail.Omschrijving = $@"Notification withdrawn:  {MutDate}";
                        ANIMAL ani = lMstb.GetAnimalByLifenr(row["Levensnummer"].ToString());
                        if (farm.ProgId == 3 || farm.ProgId == 5)
                        {
                            ani = lMstb.GetAnimalByAniAlternateNumber(row["Levensnummer"].ToString());
                        }
                        if (ani.AniId > 0)
                        {
                            ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            MOVEMENT mov;
                            EVENT eve;

                            //UBN ubn;
                            //if (UBN2e != String.Empty)
                            //{
                            //    ubn = Facade.GetInstance().getSaveToDB(pToken).getUBNByBedrijfsnummer(UBN2e);
                            //&& ubn.UBNid == mov.MovThird_UBNid
                            //}
                            switch (lMeldingType)
                            {
                                case 1: // Aanvoer
                                    mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                                    if (mov.MovId > 0)
                                    {
                                        writeLog("Ingetrokkken Aanvoer MovId:" + mov.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                                        BUYING buy = lMstb.GetBuyingByMovId(mov.MovId);
                                        lMstb.DeleteBuying(buy);
                                        lMstb.DeleteMovement(mov);
                                        anicat.Changed_By = ChangedBy;
                                        anicat.SourceID = farmUBN.ThrID;
                                        lMstb.DeleteAnimalCategory(anicat);
                                    }
                                    detail.Omschrijving += $@" Sale ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                case 2: // Afvoer
                                    mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                                    if (mov.MovId > 0)
                                    {
                                        writeLog("Ingetrokkken Afvoer MovId:" + mov.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                                        SALE sal = lMstb.GetSale(mov.MovId);
                                        lMstb.DeleteSale(sal);
                                        lMstb.DeleteMovement(mov);
                                        anicat.Anicategory = 1;

                                        anicat.Changed_By = ChangedBy;
                                        anicat.SourceID = farmUBN.ThrID;

                                        lMstb.SaveAnimalCategory(anicat);
                                    }
                                    detail.Omschrijving += $@" Buying ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                case 3: // Geboorte
                                    eve = lMstb.GetEventByDateAniIdKind(MutDate, ani.AniIdMother, 5);
                                    if (eve.EventId > 0)
                                    {
                                        writeLog("Ingetrokkken Geboorte EveId:" + eve.EventId.ToString() + " AniId:" + ani.AniId.ToString());
                                        BIRTH bir = lMstb.GetBirth(eve.EventId);
                                        lMstb.DeleteBirth(bir);
                                        lMstb.DeleteEvent(eve);
                                        lMstb.DeleteAnimalCategory(anicat);
                                    }
                                    detail.Omschrijving += $@" Birth ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                case 4: // Doodgeboren
                                    unLogger.WriteDebug("IRMUT", "Onbrekende Doodgeboren Ingetrokken : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBN : {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                                    detail.Omschrijving += $@" Born Death ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                case 5: // Import                               
                                    mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                                    if (mov.MovId > 0)
                                    {
                                        writeLog("Ingetrokkken Import MovId:" + mov.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                                        BUYING buy = lMstb.GetBuyingByMovId(mov.MovId);
                                        lMstb.DeleteBuying(buy);
                                        lMstb.DeleteMovement(mov);
                                        anicat.Changed_By = ChangedBy;
                                        anicat.SourceID = farmUBN.ThrID;
                                        lMstb.DeleteAnimalCategory(anicat);
                                    }
                                    detail.Omschrijving += $@" Import ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                case 6: // Export
                                    mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                                    if (mov.MovId > 0)
                                    {
                                        writeLog("Ingetrokkken Export MovId:" + mov.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                                        SALE sal = lMstb.GetSale(mov.MovId);
                                        lMstb.DeleteSale(sal);
                                        lMstb.DeleteMovement(mov);
                                        anicat.Anicategory = 1;
                                        anicat.Changed_By = ChangedBy;
                                        anicat.SourceID = farmUBN.ThrID;

                                        lMstb.SaveAnimalCategory(anicat);
                                    }
                                    detail.Omschrijving += $@" Export ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                case 7: // Dood
                                    mov = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
                                    if (mov.MovId > 0)
                                    {
                                        writeLog("Ingetrokkken Dood MovId:" + mov.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                                        LOSS loss = lMstb.GetLoss(mov.MovId);
                                        lMstb.DeleteLoss(loss);
                                        lMstb.DeleteMovement(mov);
                                        anicat.Anicategory = 1;// let op: de dood is ingetrokken 
                                        anicat.Changed_By = ChangedBy;

                                        anicat.SourceID = farmUBN.ThrID;

                                        lMstb.SaveAnimalCategory(anicat);
                                    }
                                    detail.Omschrijving += $@" Death ";
                                    lMstb.WriteSoapError(detail);
                                    break;
                                default:
                                    String Lifenr = row["Levensnummer"].ToString();
                                    detail.Omschrijving += $@" Unknown Kindtype:{lMeldingType} ";
                                    lMstb.WriteSoapError(detail);
                                    unLogger.WriteInfo(String.Format("Onbekende Melding van RVO : Datum {0}  Levensnr : {1} LNVType : {2}", MutDate, Lifenr, lMeldingType));
                                    continue;
                            }
                            try { writeLog("Anicategory:" + anicat.Anicategory.ToString()); }
                            catch { }
                        }
                        else 
                        {
                            detail.Omschrijving += " Unknown Animal";
                            lMstb.WriteSoapError(detail);
                        }
                    }
                    else if (lStatusMelding == 3) // herstel meldingen 
                    {
                        //een herstelmelding bestaat uit 2 meldingen
                        //deze met StatusMelding=3
                        //en de melding die je moet opvragen met de Hersteldatum uit getHerstelDetail
                        //
                        //deze heeft hersteld op 1 staan
                        detail.Omschrijving = $@"Notification restored:  {MutDate}";
                        #region dierzoeken en/of maken
                        ANIMAL ani = lMstb.GetAnimalByLifenr(row["Levensnummer"].ToString());
                        if (farm.ProgId == 3 || farm.ProgId == 5)
                        {
                            ani = lMstb.GetAnimalByAniAlternateNumber(row["Levensnummer"].ToString());
                        }
                        if (ani.AniId <= 0)
                        {
                            String Lifenr = row["Levensnummer"].ToString();
                            VulDiergegevensvanuitLNV(lToken, farm, ani, Lifenr);
                            unLogger.WriteInfo(row["Levensnummer"].ToString() + " aangemaakt via een  melding met de status: Hersteld.");
                        }
                        ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        if (anicat.FarmId == 0 || anicat.AniId == 0)
                        {
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            anicat.UbnId = farm.UBNid;
                            anicat.Anicategory = 5;
                        }
                        if (anicat.Anicategory == 0)
                        {
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            anicat.UbnId = farm.UBNid;
                            anicat.Anicategory = 5;
                        }
                        if (anicat.AniWorknumber == "")
                        {
                            string worknr = "";
                            string tempnr = "";
                            ANIMAL vader = new ANIMAL();
                            Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, ani.AniAlternateNumber, vader, 0, out worknr, out tempnr);
                            anicat.AniWorknumber = worknr;
                        }

                        anicat.Changed_By = ChangedBy;
                        anicat.SourceID = farmUBN.SourceID;


                        #endregion
                        if (ani.AniId > 0)
                        {


                            DateTime pMutDateDetail = DateTime.MinValue;
                            DateTime pHerstelDatum = DateTime.MinValue;
                            DateTime pIntrekDatum = DateTime.MinValue;
                            getHerstelDetail(lToken, farmUBN, farm, farmThird, meldingsnummer,
                                out pMutDateDetail, out pHerstelDatum, out pIntrekDatum);
                            // de pHerstelDatum is de datum waarop de herstelde melding staat
                            // ook al is de gebeurtenisdatum anders
                            // Dus een herstelde afvoer van 3-11 naar 2-11 
                            // staat bij RVO geregistreerd op pHerstelDatum ( bijvoorbeeld 5-12 ) 
                            // als de datums hetzelfde zijn wordt dit hierboven via tblLNV al rechtgetrokken
                            // als HerstelDatum tussen begindatum en einddatum ligt 
                            // zit die ook al boven in de tblLNV MAAR dan kun je hem niet met zekerheid vinden 
                            // dus het enigste wat je moet doen is hier de datum veranderen
                            // en eventueel de ubn2epartij



                            SOAPLOG ResultAgain = null;
                            //FTPUSER fusoap = lMstb.GetFtpuser(farm.UBNid, 9992);
                            String OutputAgain = unLogger.getLogDir("CSVData") + farm.Programid + farm.UBNid + DateTime.Now.Ticks + "_Herstel.csv";
                            ResultAgain = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(lToken, farm.UBNid, farm.ProgId, farm.Programid,
                                row["Levensnummer"].ToString(), MeldingType, MeldingStatus,
                                UBNnr2ePartijd, pHerstelDatum, pHerstelDatum, 1, OutputAgain);
                            ResultAgain.TaskLogID = TasklogID;
                            lMstb.WriteSoapError(ResultAgain);
                            DataTable tblLNVAgain = utils.GetCsvData(Kols, OutputAgain, spl, "Mutaties");
                            if (tblLNVAgain.Rows.Count > 0)
                            {
                                DataRow[] foundrows = tblLNVAgain.Select("Levensnummer='" + row["Levensnummer"].ToString() + "' AND Meldingtype=" + lMeldingType.ToString() + " AND hersteld=1");
                                if (foundrows.Count() > 0)
                                {
                                    string ret = checkHerstelMelding(lToken, ani, anicat, farm, farmUBN, farmThird, lMeldingType, pMutDateDetail, foundrows[0]);
                                    if (ret != "")
                                    {
                                        unLogger.WriteError(ret);
                                    }
                                }
                                else { unLogger.WriteError("Geen herstelmelding gevonden tussen de gevonden herstelmeldingen voor: " + row["Levensnummer"].ToString() + " dit betreft een herstelde melding, met meldingtype:" + lMeldingType.ToString() + " meldnummer:" + meldingsnummer); }

                            }
                            else { unLogger.WriteError("Geen herstelmelding gevonden: van " + row["Levensnummer"].ToString() + " dit betreft een herstelde melding, met meldingtype=" + lMeldingType.ToString() + " meldnummer:" + meldingsnummer); }


                        }
                        else { unLogger.WriteError("Het dier: " + row["Levensnummer"].ToString() + " is niet gevonden en niet aangemaakt: dit betreft een herstelde melding, met meldingtype=" + lMeldingType.ToString() + " meldnummer:" + meldingsnummer); }
                    }
                    else if (lHersteld == 1)
                    {
                        //een herstelmelding bestaat uit 2 meldingen
                        //deze met lHersteld == 1
                        //en de vorige met  (lStatusMelding == 3)
                        //Hier moet je zoeken naar de herstelmelding met lStatusMelding == 3
                        //om te weten wat de juiste datum is van de oorspronkelijke melding
                        //p.s Als het dier niet bestaat dan is de oorspronkelijke melding nog niet opgeslagen
                        detail.Omschrijving = $@"Notification restored:  {MutDate}";
                        SOAPLOG ResultAgain = null;
                        String OutputAgain = unLogger.getLogDir("CSVData") + farm.Programid + farm.UBNid + DateTime.Now.Ticks + "_Herstel2.csv";
                        ResultAgain = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(lToken, farm.UBNid, farm.ProgId, farm.Programid,
                            row["Levensnummer"].ToString(), lMeldingType, 3,
                            UBNnr2ePartijd, MutDate.AddDays(-31), MutDate.AddDays(31), 1, OutputAgain);
                        ResultAgain.TaskLogID = TasklogID;
                        lMstb.WriteSoapError(ResultAgain);
                        DataTable tblLNVAgain = utils.GetCsvData(Kols, OutputAgain, spl, "Mutaties");
                        if (tblLNVAgain.Rows.Count > 0)
                        {
                            DataRow[] foundrows = tblLNVAgain.Select("Levensnummer='" + row["Levensnummer"].ToString() + "' AND Meldingtype=" + lMeldingType.ToString());
                            if (foundrows.Count() > 0)
                            {
                                foreach (DataRow rFrow in foundrows)
                                {
                                    DateTime herstelDatum = utils.getDateLNV(row["Datum"].ToString());
                                    DateTime pMutDateDetail = DateTime.MinValue;
                                    DateTime pHerstelDatum = DateTime.MinValue;
                                    DateTime pIntrekDatum = DateTime.MinValue;
                                    getHerstelDetail(lToken, farmUBN, farm, farmThird, rFrow["meldingsnummer"].ToString(),
                                        out pMutDateDetail, out pHerstelDatum, out pIntrekDatum);
                                    #region dierzoeken en/of maken
                                    ANIMAL ani = lMstb.GetAnimalByLifenr(row["Levensnummer"].ToString());
                                    if (farm.ProgId == 3 || farm.ProgId == 5)
                                    {
                                        ani = lMstb.GetAnimalByAniAlternateNumber(row["Levensnummer"].ToString());
                                    }
                                    if (ani.AniId <= 0)
                                    {
                                        String Lifenr = row["Levensnummer"].ToString();
                                        VulDiergegevensvanuitLNV(lToken, farm, ani, Lifenr);
                                        unLogger.WriteInfo(row["Levensnummer"].ToString() + " aangemaakt via een herstelmelding.");
                                    }
                                    ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                                    if (anicat.FarmId == 0 || anicat.AniId == 0)
                                    {
                                        anicat.AniId = ani.AniId;
                                        anicat.FarmId = farm.FarmId;
                                        anicat.UbnId = farm.UBNid;
                                        anicat.Anicategory = 5;
                                    }
                                    if (anicat.Anicategory == 0)
                                    {
                                        anicat.AniId = ani.AniId;
                                        anicat.FarmId = farm.FarmId;
                                        anicat.UbnId = farm.UBNid;
                                        anicat.Anicategory = 5;
                                    }
                                    if (anicat.AniWorknumber == "")
                                    {
                                        string worknr = "";
                                        string tempnr = "";
                                        ANIMAL vader = new ANIMAL();
                                        Event_functions.getRFIDNumbers(pToken, farm, fcon.FValue, ani.AniAlternateNumber, vader, 0, out worknr, out tempnr);
                                        anicat.AniWorknumber = worknr;
                                    }
                                    #endregion
                                    checkHerstelMelding(lToken, ani, anicat, farm, farmUBN, farmThird, lMeldingType, pMutDateDetail, row);

                                }
                            }
                            else { unLogger.WriteError("Herstelmelding: geen (oorspronkelijke) melding, met meldingtype=" + lMeldingType.ToString() + ", gevonden betreffende:" + row["Levensnummer"].ToString() + " met de  status  3 (Hersteld) tussen de gevonden meldingen."); }

                        }
                        else { unLogger.WriteError("Herstelmelding: geen (oorspronkelijke) melding, met meldingtype=" + lMeldingType.ToString() + ", gevonden betreffende:" + row["Levensnummer"].ToString() + " met de  status  3 (Hersteld) "); }

                        //////////////////////////////////////////////////////////////////////////////////////////////
                        //ANIMAL ani = lMstb.GetAnimalByLifenr(row["Levensnummer"].ToString());
                        //if (farm.ProgId == 3 || farm.ProgId == 5)
                        //{
                        //    ani = lMstb.GetAnimalByAniAlternateNumber(row["Levensnummer"].ToString());
                        //}
                        //if (ani.AniId > 0)
                        //{
                        //    ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        //    anicat.AniId = ani.AniId;
                        //    anicat.FarmId = farm.FarmId;
                        //    int lMovkind = 0;
                        //    int lEvekind = 0;

                        //    if (lMovkind > 0 || lEvekind > 0)
                        //    {
                        //        bool goOnn = false;
                        //        if (lMovkind > 0)
                        //        {
                        //            var check = from n in movs
                        //                        where n.MovKind == lMovkind
                        //                        && n.MovDate.Date == MutDate.Date
                        //                        select n;
                        //            if (check.Count() == 0)//Anders is ie er al en hersteld
                        //            {
                        //                goOnn = true;
                        //            }
                        //        }
                        //        if (lEvekind > 0)
                        //        {
                        //            var check = from n in MotherEvents
                        //                        where n.EveKind == lEvekind
                        //                        && n.EveDate.Date == MutDate.Date
                        //                        select n;
                        //            if (check.Count() == 0)//Anders is ie er al en hersteld
                        //            {
                        //                goOnn = true;
                        //            }
                        //        }
                        //        if (goOnn)
                        //        {
                        //            goOnn = false;
                        //            SOAPLOG ResultAgain = null;
                        //            String OutputAgain = unLogger.getLogDir("CSVData") + farm.Programid + farm.UBNid + DateTime.Now.Ticks + "_Herstel2.csv";
                        //            ResultAgain = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(lToken, farm.UBNid, farm.ProgId, farm.Programid,
                        //                row["Levensnummer"].ToString(), lMeldingType, 3,
                        //                UBNnr2ePartijd, MutDate.AddDays(-31), MutDate.AddDays(31), 1, OutputAgain);

                        //            DataTable tblLNVAgain = utils.GetCsvData(Kols, OutputAgain, spl, "Mutaties");
                        //            if (tblLNVAgain.Rows.Count > 0)
                        //            {
                        //                DataRow[] foundrows = tblLNVAgain.Select("Levensnummer='" + row["Levensnummer"].ToString() + "' AND Meldingtype=" + lMeldingType.ToString());
                        //                if (foundrows.Count() > 0)
                        //                {
                        //                    foreach (DataRow rFrow in foundrows)
                        //                    {
                        //                        DateTime herstelDatum = utils.getDateLNV(row["Datum"].ToString());
                        //                        DateTime pMutDateDetail = DateTime.MinValue;
                        //                        DateTime pHerstelDatum = DateTime.MinValue;
                        //                        DateTime pIntrekDatum = DateTime.MinValue;
                        //                        getHerstelDetail(lToken, farmUBN, farm, farmThird, rFrow["meldingsnummer"].ToString(),
                        //                            out pMutDateDetail, out pHerstelDatum, out pIntrekDatum);

                        //                        if (lMovkind > 0)
                        //                        {
                        //                            var check = from n in movs
                        //                                        where n.MovKind == lMovkind
                        //                                        && n.MovDate.Date == pMutDateDetail.Date
                        //                                        select n;
                        //                            if (check.Count() > 0)//dan is ie nog niet hersteld
                        //                            {
                        //                                changeHerstelMelding(lToken, ani, anicat, farm, farmUBN, farmThird, pMutDateDetail, lMeldingType, row);
                        //                            }
                        //                            else //dan zijn ze er allebei niet
                        //                            {
                        //                                addHerstelMelding(lToken, ani, anicat, farm, farmUBN, farmThird, lMeldingType, row, new MOVEMENT(), new EVENT());
                        //                            }
                        //                        }
                        //                        if (lEvekind > 0)
                        //                        {
                        //                            var check = from n in MotherEvents
                        //                                        where n.EveKind == lEvekind
                        //                                        && n.EveDate.Date == pMutDateDetail.Date
                        //                                        select n;
                        //                            if (check.Count() > 0)//Anders is ie er al en hersteld
                        //                            {
                        //                                changeHerstelMelding(lToken, ani, anicat, farm, farmUBN, farmThird, pMutDateDetail, lMeldingType, row);
                        //                            }
                        //                            else //dan zijn ze er allebei niet
                        //                            {
                        //                                addHerstelMelding(lToken, ani, anicat, farm, farmUBN, farmThird, lMeldingType, row, new MOVEMENT(), new EVENT());
                        //                            }
                        //                        }

                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }

                        //}
                    }
                    //}

                    if (RequestUpdate != null)
                    {

                        b.Progress = 100;
                        b.Message = " Klaar " + totaal.ToString() + " " + VSM_Ruma_OnlineCulture.getStaticResource("dieren", "dieren") + " " + VSM_Ruma_OnlineCulture.getStaticResource("verwerkt", "verwerkt");
                        RequestUpdate(this, b);
                    }
                }
            }
            
           
            checkmissingbirths(pToken, farm.FarmId, ChangedBy);             
            return Result;
        }

        /// <summary>
        /// Farmconfig or programconfig: IRMissingBirths:=True
        /// Counrty: NL
        /// </summary>
        /// <param name="pToken"></param>
        /// <param name="farmid"></param>
        /// <param name="changedby"></param>
        public void checkmissingbirths(UserRightsToken pToken, int farmid, int changedby)
        {
            UserRightsToken lToken = (UserRightsToken)pToken.Clone();
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(lToken);
            BEDRIJF farm = new BEDRIJF();
            UBN farmUBN = new UBN();
            THIRD farmThird = new THIRD();
            COUNTRY c = new COUNTRY();
            lMstb.getCompanyByFarmId(farmid, out farm, out farmUBN, out farmThird, out c);
            string checkbirths = lMstb.GetConfigValue(farm.Programid, farmid, "IRMissingBirths","False");
            if (string.IsNullOrEmpty(checkbirths) ||  checkbirths.ToLower()  != "true")
            {
                unLogger.WriteInfo($@"{nameof(checkmissingbirths)}:IRMissingBirths:={checkbirths}: UBN {farmUBN.Bedrijfsnummer}  {farmUBN.Bedrijfsnaam} ");
                return;
            }
            if (c.LandId != 151)
            {
                unLogger.WriteInfo($@"{nameof(checkmissingbirths)}:geen NL: UBN {farmUBN.Bedrijfsnummer}  {farmUBN.Bedrijfsnaam} ");
                return;
            }
          
            StringBuilder bld = new StringBuilder();
            //bld.Append($@"
            //    SELECT a.AniID,a.AniAlternateNumber,a.Anibirthdate, 
            //    a.AniName,ac.Ubnid,ac.FarmID,
            //    ev.* 
            //    FROM ANIMAL a
            //    JOIN ANIMALCATEGORY ac ON ac.AniID=a.Aniid
            //    LEFT JOIN EVENT ev ON ev.AniId=a.AniId AND ev.EveKind=5
            //    WHERE a.AniSex=2  AND a.AniId>0
            //    AND PERIOD_DIFF( EXTRACT( YEAR_MONTH FROM NOW() ), EXTRACT( YEAR_MONTH FROM a.AniBirthDate ) ) > 18
            //    AND isnull(ev.EventId) 
            //    AND PERIOD_DIFF( EXTRACT( YEAR_MONTH FROM NOW() ), EXTRACT( YEAR_MONTH FROM a.AniBirthDate ) ) < 240
            //    AND ac.UbnId=@UbnId AND ac.Anicategory<4
            //    GROUP BY a.AniID
            //");
            //bld = new StringBuilder();
            bld.AppendLine($@"SET @UbnId	:= {farmUBN.UBNid};");
            string selquery = lMstb.getSelection_Query("IRMissingBirthsAnimals");
            if (string.IsNullOrWhiteSpace(selquery))
            {
                unLogger.WriteInfo($@"{nameof(checkmissingbirths)}:geen selectionquery: IRMissingBirthsAnimals ");
                return;
            }
            bld.Append(lMstb.getSelection_Query("IRMissingBirthsAnimals"));
            DataTable tbl = lMstb.GetDataBase().QueryData(lToken.getLastChildConnection(), bld);
            if (tbl.Rows.Count == 0)
            {
                unLogger.WriteInfo($@"{nameof(checkmissingbirths)}:No Animals to check: UBN {farmUBN.Bedrijfsnummer}  {farmUBN.Bedrijfsnaam} ");
                return;
            }
            Console.WriteLine($@"Missende geboortes Ubn:{farmUBN.Bedrijfsnummer}  {farmUBN.Bedrijfsnaam}");
            unLogger.WriteInfo($@"{nameof(checkmissingbirths)}:{tbl.Rows.Count} Animals to check: UBN {farmUBN.Bedrijfsnummer}  {farmUBN.Bedrijfsnaam} ");
        
            SOAPLOG Result = new SOAPLOG
            {
                Changed_By = ChangedBy,
                Code = "",
                Date = DateTime.Now.Date,
                FarmNumber = farmUBN.Bedrijfsnummer,
                Kind = (int)LABELSConst.SOAPLOGKind.LNV_Meldingen_Raadplegen_Details,
                Lifenumber = "",
                Omschrijving = "",
                SourceID = farmThird.ThrId,
                Status = "G",
                SubKind = 0,
                TaskLogID = TasklogID,
                ThrId = farmThird.ThrId,
                Time = DateTime.Now
            };
            FTPUSER fusoap = lMstb.GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9992);
            String BRSnr = string.IsNullOrEmpty(farmThird.Thr_Brs_Number) ? farmUBN.BRSnummer : farmThird.Thr_Brs_Number;
            if (fusoap.UserName == String.Empty || fusoap.Password == String.Empty || BRSnr == string.Empty)
            {
                Result.Status = "F";
                Result.Omschrijving = $@"(fusoap.UserName  || fusoap.Password || Thr_Brs_Number)  String.Empty  ";
                lMstb.WriteSoapError(Result);
                unLogger.WriteInfo($@"{nameof(checkmissingbirths)}:(fusoap.UserName:{fusoap.UserName}  || fusoap.Password:{fusoap.Password} || Thr_Brs_Number:{BRSnr})  String.Empty: UBN {farmUBN.Bedrijfsnummer}  {farmUBN.Bedrijfsnaam} ");
                return;
            }

            string datumtijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            string tl_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           
            int tasklogid = lMstb.SaveNewTaskLog(Environment.MachineName,$@"{nameof(VSM.RUMA.CORE.MutationUpdater)}.{nameof(checkmissingbirths)}", tl_version, DateTime.Now, farm.ProgId, 0, 0, 0, 0, 0, "", Path.Combine(unLogger.getLogDir(), "CSVData") + @" AND \IenR", (int)LABELSConst.TL_State.BEGIN);
            Result.TaskLogID = tasklogid;
            List<int> lLabkinds = new List<int>();
            Feedadviceer FaAdviser = new Feedadviceer();
            //Win32SOAPIRALG dllcall = new Win32SOAPIRALG();
            int ok = 0;
            int warn = 0;
            int nodata = 0;
            int error = 0;
            Directory.CreateDirectory(Path.Combine(unLogger.getLogDir(), "CSVData"));
            Directory.CreateDirectory(Path.Combine(unLogger.getLogDir(), "IenR"));
            string[] kolommen = { "nr", "AniAlternateNumber", "Geboortedatum", "Geslacht", "Kleur" };

            MovFuncEvent mfe = new MovFuncEvent();
            if (RequestUpdate != null)
            {
                mfe.Progress = 0;
                mfe.Message = $@"{VSM_Ruma_OnlineCulture.getStaticResource("missingbirths", "Geboortes:")} {Result.FarmNumber} ; {tbl.Rows.Count} ";
                RequestUpdate(this, mfe);

            }

          

            int teller = 0;
            double totaal = tbl.Rows.Count;
            double procent = 0;
            foreach (DataRow rw in tbl.Rows)
            {
                teller += 1;
                procent = teller * 100 /totaal;
                Result.Lifenumber = rw["AniAlternateNumber"].ToString();

                if (RequestUpdate != null)
                {
                    mfe.Progress = (int)procent;
                    mfe.Message = $@"{VSM_Ruma_OnlineCulture.getStaticResource("missingbirths", "Geboortes:")} {Result.Lifenumber} ; {teller} van {totaal} ";
                    RequestUpdate(this, mfe);
                }


                ANIMAL moeder = lMstb.GetAnimalByAniAlternateNumber(Result.Lifenumber);
                int LNVprognr = 0;
                string BRSnrHouder = String.Empty;
                string UBNhouder = String.Empty;
                string Werknummer = String.Empty;
                DateTime Geboortedat = DateTime.MinValue;
                DateTime Importdat = DateTime.MinValue;
                string LandCodeHerkomst = String.Empty;
                string LandCodeOorsprong = String.Empty;
                string Geslacht = String.Empty;
                string Haarkleur = String.Empty;
                DateTime Einddatum = DateTime.MinValue;
                string RedenEinde = String.Empty;
                string LevensnrMoeder = String.Empty;
                string VervangenLevensnr = String.Empty;
                string Status = String.Empty;
                string Code = String.Empty;
                string Omschrijving = String.Empty;
                int lnvprognr = 0;
                int pMaxStrLen = 255;
               
                string pLogfile = unLogger.getLogDir("IenR") + farmUBN.Bedrijfsnummer + "_Dierdetails_" + moeder.AniAlternateNumber.ToString().Replace(" ","") + "_" + datumtijd + ".log";
                string pDetailfile = unLogger.getLogDir("CSVData") + farmUBN.Bedrijfsnummer + "_Dierdetails_" + moeder.AniAlternateNumber.ToString().Replace(" ", "") + "_" + datumtijd + ".csv";

                SOAPLNVDieren slnv = new SOAPLNVDieren();
             
                List<SOAPLNVDieren.Diernakomelingen> Nakomelingen = new List<SOAPLNVDieren.Diernakomelingen>();
                slnv.LNVDierdetailsV2(fusoap.UserName, fusoap.Password, 0,
                                            farmUBN.Bedrijfsnummer, BRSnr, moeder.AniAlternateNumber, farm.ProgId,
                                            0, 0, 0, ref lnvprognr,
                                            ref Werknummer,
                                            ref Geboortedat, ref Importdat,
                                            ref LandCodeHerkomst, ref LandCodeOorsprong,
                                            ref Geslacht, ref Haarkleur,
                                            ref Einddatum, ref RedenEinde,
                                            ref LevensnrMoeder, ref VervangenLevensnr, ref Nakomelingen,
                                            ref Status, ref Code, ref Omschrijving);

                //dllcall.LNVDierdetailsV2(fusoap.UserName, fusoap.Password, 0,
                //                            farmUBN.Bedrijfsnummer, BRSnr, moeder.AniAlternateNumber, farm.ProgId,
                //                            0, 0, 1,
                //                            ref LNVprognr, ref Werknummer,
                //                            ref Geboortedat, ref Importdat,
                //                            ref LandCodeHerkomst, ref LandCodeOorsprong,
                //                            ref Geslacht, ref Haarkleur,
                //                            ref Einddatum, ref RedenEinde,
                //                            ref LevensnrMoeder, ref VervangenLevensnr,
                //                            ref Status, ref Code, ref Omschrijving,
                //                            pDetailfile, pLogfile, pMaxStrLen);

                Result.Status = Status;
                Result.Code = Code;
                Result.Omschrijving = string.IsNullOrEmpty(Omschrijving) ? $@"{Nakomelingen.Count()} nakomelingen." : Omschrijving;
                lMstb.WriteSoapError(Result);
                if (Status.ToUpper() != "G")
                {
                    error += 1;
                 
                    continue;
                }

                unLogger.WriteInfo($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} :ubn:{farmUBN.Bedrijfsnummer}  {moeder.AniAlternateNumber}: Nakomelingen:{Nakomelingen.Count()} ");
               
                if (Nakomelingen.Count() > 0)
                {
                    StringBuilder b = new StringBuilder();
                    StringBuilder line = new StringBuilder();

                    foreach (SOAPLNVDieren.Diernakomelingen child in Nakomelingen)
                    {
                        line.Clear();
                        line.Append($@"3;");//getal (soort=nakomelingen) uit Delphie DLL. Zie soapiralg.doc 
                        line.Append($@"{child.Levensnummer};");
                        line.Append($@"{child.geboorteDatum};");
                        line.Append($@"{child.Geslacht};");
                        line.Append($@"{child.Haarkleur};");
                        line.Append($@"{child.Werknummer};");//niet in Delphie DLL 
                        b.AppendLine(line.ToString());
                    }
                    using (StreamWriter wr = new StreamWriter(pDetailfile))
                    {
                        wr.Write(b.ToString());
                    }
                }
                if (File.Exists(pDetailfile))
                {
                    DataTable tblgeb = utils.GetCsvData(kolommen, pDetailfile, ';', "Geboortes");
                 
                    if (tblgeb.Rows.Count == 0)
                    {
                        nodata += 1;
                       
                    }
                    else
                    {
                        ok += 1;
                        
                        foreach (DataRow rwgeb in tblgeb.Rows)
                        {
                            if (rwgeb["nr"].ToString().Trim() != "3")
                            {
                             
                                unLogger.WriteInfo($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} : wrong datarow number:{rwgeb["nr"].ToString()}:  ubn:{farmUBN.Bedrijfsnummer}");
                            }
                            else
                            {
                                DateTime Geboortedatum = utils.getDateLNV(rwgeb["Geboortedatum"].ToString());
                                if (rwgeb["AniAlternateNumber"] == DBNull.Value || string.IsNullOrEmpty(rwgeb["AniAlternateNumber"].ToString()))
                                {
                                    
                                    var events = lMstb.getEventsByDateAniIdKind(Geboortedatum, moeder.AniId, (int)LABELSConst.EventKind.AFKALVEN);
                                    List<BIRTH> births = new List<BIRTH>(events.Count);
                                    foreach (EVENT ev in events)
                                    {
                                        births.Add(lMstb.GetBirth(ev.EventId));
                                    }
                                    if (births.Any(b => b.CalfId == 0 && b.BornDead == 1))
                                    {

                                    }
                                    else
                                    {
                                        EVENT ev = new EVENT();
                                        ev.AniId = moeder.AniId;
                                        ev.EveDate = Geboortedatum;
                                        ev.EveKind = (int)LABELSConst.EventKind.AFKALVEN;
                                        ev.UBNId = farmUBN.UBNid;
                                        ev.EveMutationDate = DateTime.Now.Date;
                                        ev.EveMutationTime = DateTime.Now;
                                        ev.EveOrder = Event_functions.getNewEventOrder(lToken, ev.EveDate, 5, farm.UBNid, moeder.AniId);
                                        ev.Changed_By = changedby;
                                        ev.SourceID = 0;
                                        if (!lMstb.SaveEvent(ev))
                                        {
                                            unLogger.WriteError($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer} moeder:{moeder.AniAlternateNumber} Er is iets fout gegaan bij opslaan EVENT voor birth van born death.");

                                        }
                                        BIRTH b = new BIRTH();
                                        b.EventId = ev.EventId;
                                        b.CalfId = 0;

                                        b.BornDead = 1;
                                        b.Changed_By = changedby;
                                        b.SourceID = 0;

                                        if (!lMstb.SaveBirth(b))
                                        {
                                            unLogger.WriteError($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer} moeder:{moeder.AniAlternateNumber} Er is iets fout gegaan bij opslaan BIRTH   van borndeath.");

                                        }
                                         unLogger.WriteInfo($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer}  moeder:{moeder.AniAlternateNumber} Add birth:born death.");
                                        if (farm.Programid == (int)CORE.DB.LABELSConst.programId.MS_OPTIMA_BOX)
                                        {
                                            if (ev.EveDate.Date > DateTime.Now.AddDays(-120))
                                            {
                                                FaAdviser.saveKetoBoxFeedAdvices(lToken, moeder.AniId, ev.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                               
                                    int AniSex = rwgeb["Geslacht"].ToString().ToUpper() == "M" ? 1 : 2;
                                    ANIMAL child = lMstb.GetAnimalByAniAlternateNumber(rwgeb["AniAlternateNumber"].ToString());
                                    if (child.AniId == 0)
                                    {
                                        child = new ANIMAL
                                        {
                                            AniLifeNumber = rwgeb["AniAlternateNumber"].ToString(),
                                            AniAlternateNumber = rwgeb["AniAlternateNumber"].ToString(),
                                            AniHaircolor_Memo = rwgeb["Kleur"].ToString(),
                                            AniSex = AniSex,
                                            AniBirthDate = Geboortedatum
                                        };
                                        VulDiergegevensvanuitLNV(lToken, farm, child, rwgeb["AniAlternateNumber"].ToString());
                                    }

                                    if (child.AniId == 0)
                                    {
                                        unLogger.WriteError($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer} moeder:{moeder.AniAlternateNumber} kan het kind:{child.AniAlternateNumber}. niet opslaan.");
                                       
                                    }
                                    else
                                    {
                                        if (child.AniIdMother > 0 && child.AniIdMother != moeder.AniId)
                                        {
                                            
                                            unLogger.WriteInfo($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer} moeder:{moeder.AniId} ; {moeder.AniAlternateNumber} child:{child.AniId} ; {child.AniAlternateNumber} has wrong mother: {child.AniIdMother}");
                                            ok -= 1;
                                            error += 1;
                                            continue;
                                        }
                                        //geboortes corrigeren / aanvullen
                                        var events = lMstb.getEventsByDateAniIdKind(child.AniBirthDate, moeder.AniId, (int)LABELSConst.EventKind.AFKALVEN);
                                        List<BIRTH> births = new List<BIRTH>(events.Count);
                                        foreach (EVENT ev in events)
                                        {
                                            births.Add(lMstb.GetBirth(ev.EventId));
                                        }

                                        if (births.Any(b => b.CalfId == child.AniId))
                                        {

                                            continue;
                                        }
                                        else if (births.Any(b => b.CalfId == 0 && b.BornDead == 0))
                                        {
                                            //update
                                            BIRTH bir = births.First(b => b.CalfId == 0 && b.BornDead == 0);
                                            if (bir != null && bir.EventId > 0)
                                            {
                                                bir.CalfId = child.AniId;
                                                lMstb.SaveBirth(bir);
                                            }
                                        }
                                        else if (child.AniBirthDate > DateTime.MinValue)
                                        {
                                            EVENT ev = new EVENT();
                                            ev.AniId = moeder.AniId;
                                            ev.EveDate = child.AniBirthDate.Date;
                                            ev.EveKind = (int)LABELSConst.EventKind.AFKALVEN;
                                            ev.UBNId = farmUBN.UBNid;
                                            ev.EveMutationDate = DateTime.Now.Date;
                                            ev.EveMutationTime = DateTime.Now;
                                            ev.EveOrder = Event_functions.getNewEventOrder(lToken, ev.EveDate, 5, farm.UBNid, moeder.AniId);
                                            ev.Changed_By = changedby;
                                            ev.SourceID = 0;
                                            if (!lMstb.SaveEvent(ev))
                                            {
                                                unLogger.WriteError($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer} moeder:{moeder.AniAlternateNumber} Er is iets fout gegaan bij opslaan EVENT voor birth van dier {child.AniAlternateNumber}.");

                                            }
                                            BIRTH b = new BIRTH();
                                            b.EventId = ev.EventId;
                                            b.CalfId = child.AniId;

                                            b.BornDead = 0;
                                            b.Changed_By = changedby;
                                            b.SourceID = 0;

                                            if (!lMstb.SaveBirth(b))
                                            {
                                                unLogger.WriteError($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer} moeder:{moeder.AniAlternateNumber} Er is iets fout gegaan bij opslaan BIRTH   van dier {child.AniAlternateNumber}.");

                                            }
                                            
                                            unLogger.WriteInfo($@"{nameof(MutationUpdater)}.{nameof(checkmissingbirths)} ubn:{farmUBN.Bedrijfsnummer}  {moeder.AniAlternateNumber} Add birth: {child.AniAlternateNumber}.");
                                            if (farm.Programid == (int)CORE.DB.LABELSConst.programId.MS_OPTIMA_BOX)
                                            {
                                                if (ev.EveDate.Date > DateTime.Now.AddDays(-120))
                                                {
                                                    FaAdviser.saveKetoBoxFeedAdvices(lToken, moeder.AniId, ev.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
               
             
                
            }
            lMstb.UpdateTaskLog(tasklogid, DateTime.Now, ok, warn, nodata, error, $@"aantal controles {tbl.Rows.Count}", (int)LABELSConst.TL_State.DONE_OK);
            if (RequestUpdate != null)
            {
                mfe.Progress = 100;
                mfe.Message = $@"Klaar ";
                RequestUpdate(this, mfe);
            }
        }

        private string checkHerstelMelding(UserRightsToken pToken, ANIMAL ani, ANIMALCATEGORY anicat, BEDRIJF farm, UBN farmUBN, THIRD farmThird, int pMeldingType, DateTime pMutDateDetail, DataRow pDataRow)
        {
            string ret = "";
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pToken);
            List<MOVEMENT> movs = lMstb.GetMovementsByAniId(ani.AniId);
            List<EVENT> MotherEvents = lMstb.getEventsByAniIdKind(ani.AniIdMother, 5);
            int lMovkind = 0;
            int lEvekind = 0;
            switch (pMeldingType)
            {
                case 1: // Aanvoer

                    lMovkind = 1;
                    break;
                case 2: // Afvoer
                    //mov1 = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                    lMovkind = 2;
                    break;
                case 3: // Geboorte
                    //eve1 = lMstb.GetEventByDateAniIdKind(MutDate, ani.AniIdMother, 5, farm.UBNid);
                    lEvekind = 5;
                    break;
                case 4: // Doodgeboren

                    break;
                case 5: // Import                               
                    lMovkind = 1;
                    break;
                case 6: // Export
                    lMovkind = 2;
                    break;
                case 7: // Dood
                    lMovkind = 3;
                    break;
            }
            if (lMovkind > 0)
            {
                var check = from n in movs
                            where n.MovKind == lMovkind
                            && n.MovDate.Date == pMutDateDetail.Date
                            select n;
                if (check.Count() > 0)//dan is ie nog niet hersteld
                {
                    changeHerstelMelding(pToken, ani, anicat, farm, farmUBN, farmThird, pMutDateDetail, pMeldingType, pDataRow);
                }
                else //dan zijn ze er allebei niet
                {
                    addHerstelMelding(pToken, ani, anicat, farm, farmUBN, farmThird, pMeldingType, pDataRow, new MOVEMENT(), new EVENT());
                }
            }
            else if (lEvekind == 5)
            {
                BIRTH lbitrth = lMstb.GetBirthByCalfId(ani.AniId);
                if (lbitrth.EventId > 0)//Anders is ie er al en hersteld
                {
                    changeHerstelMelding(pToken, ani, anicat, farm, farmUBN, farmThird, pMutDateDetail, pMeldingType, pDataRow);
                }
                else //dan zijn ze er allebei niet
                {
                    addHerstelMelding(pToken, ani, anicat, farm, farmUBN, farmThird, pMeldingType, pDataRow, new MOVEMENT(), new EVENT());
                }
            }
            else { ret = "Herstelmelding: geen (oorspronkelijke) melding, met meldingtype=" + pMeldingType.ToString() + ", gevonden betreffende:" + ani.AniAlternateNumber + " met de  status  3 (Hersteld) : meldingtype komt niet overeen."; }
            return ret;
        }

        private void addHerstelMelding(UserRightsToken pToken, ANIMAL ani, ANIMALCATEGORY anicat, BEDRIJF farm, UBN farmUBN, THIRD farmThird, int pMeldingType, DataRow pDataRow, MOVEMENT pMov, EVENT pEvent)
        {
            Feedadviceer FaAdviser = new Feedadviceer();
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pToken);
            int lMeldingType = Convert.ToInt32(pDataRow["Meldingtype"]);
            int lStatusMelding = Convert.ToInt32(pDataRow["meldingstatus"]);
            DateTime MutDate = utils.getDateLNV(pDataRow["Datum"].ToString());
            String UBN2e = pDataRow["ubn2ePartij"].ToString();
            string meldingsnummer = pDataRow["meldingsnummer"].ToString();

            UBN ubntweede = new UBN();
            if (UBN2e != String.Empty)
            {
                ubntweede = lMstb.getUBNByBedrijfsnummer(UBN2e);

            }



            switch (lMeldingType)
            {
                case 1: // Aanvoer                                      

                    anicat.Anicategory = 1;
                    bool IRAanvoer = Convert.ToBoolean(lMstb.GetConfigValue(farm.Programid, farm.FarmId, "IRAanvoerAutoRecieve", "True"));
                    if (pMov.MovId == 0 && IRAanvoer)
                    {
                        unLogger.WriteDebug("IRMUT", "Onbrekende Aanvoer : ");
                        unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                        pMov.MovMutationBy = 15;
                        pMov.MovMutationDate = DateTime.Today;
                        pMov.MovMutationTime = DateTime.Now;
                        pMov.AniId = ani.AniId;
                        pMov.MovDate = MutDate;
                        pMov.MovTime = MutDate.AddHours(pMov.MovMutationTime.Hour).AddMinutes(pMov.MovMutationTime.Minute).AddSeconds(pMov.MovMutationTime.Second);
                        pMov.MovKind = 1;
                        pMov.MovOrder = MovFunc.getNewMovementOrder(pToken, 2, MutDate, ani.AniId, farm.UBNid);
                        pMov.UbnId = farm.UBNid;
                        pMov.Progid = farm.ProgId;
                        pMov.happened_at_FarmID = anicat.FarmId;
                        if (UBN2e != String.Empty)
                        {
                            UBN ubnfrom = lMstb.getUBNByBedrijfsnummer(UBN2e);
                            pMov.MovThird_UBNid = ubnfrom.UBNid;
                            pMov.ThrId = ubnfrom.ThrID;
                        }
                        lMstb.SaveMovement(pMov);
                        BUYING buy = new BUYING();
                        buy.MovId = pMov.MovId;
                        lMstb.SaveBuying(buy);
                        MovFunc.SetSaveAnimalCategory(pToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);

                        if (pDataRow["meldingstatus"].ToString() == "2" && UBN2e != String.Empty)
                        {
                            //FARMCONFIG fcIR = Facade.GetInstance().getSaveToDB(pToken).getFarmConfig(farm.FarmId, "IRaanvoer", "True");
                            //MovFunc.saveAanVoerMutation(pToken, farm, fcIR, pMov, buy, "", "");
                        }
                        else pMov.ReportDate = MutDate;
                        writeLog("Aanvoer MovId:" + pMov.MovId.ToString());
                    }

                   
                    checkForMsOptimaboxRespondernumber(pToken, farm, ani, false);
                    break;
                case 2: // Afvoer
                    anicat.Anicategory = 4;
                  
                    if (pMov.MovId == 0)
                    {
                        unLogger.WriteDebug("IRMUT", "Onbrekende Afvoer : ");
                        unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", UBN2e));


                        pMov.MovMutationBy = 15;
                        pMov.MovMutationDate = DateTime.Today;
                        pMov.MovMutationTime = DateTime.Now;
                        pMov.AniId = ani.AniId;
                        pMov.MovDate = MutDate;
                        pMov.MovTime = MutDate.AddHours(pMov.MovMutationTime.Hour).AddMinutes(pMov.MovMutationTime.Minute).AddSeconds(pMov.MovMutationTime.Second);
                        pMov.MovKind = 2;
                        pMov.MovOrder = MovFunc.getNewMovementOrder(pToken, 2, MutDate, ani.AniId, farm.UBNid);
                        pMov.UbnId = farm.UBNid;
                        pMov.Progid = farm.ProgId;
                        pMov.happened_at_FarmID = anicat.FarmId;
                        SALE sal = new SALE();
                        if (UBN2e != String.Empty)
                        {
                            UBN ubnto = lMstb.getUBNByBedrijfsnummer(UBN2e);
                            pMov.MovThird_UBNid = ubnto.UBNid;
                            sal.SalDestination = ubnto.ThrID;
                        }
                        lMstb.SaveMovement(pMov);

                        sal.MovId = pMov.MovId;
                        lMstb.SaveSale(sal);
                        MovFunc.SetSaveAnimalCategory(pToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                        MovFunc.returnTransmitters(pToken, farm, pMov.AniId);
                        if (pDataRow["meldingstatus"].ToString() == "2" && UBN2e != String.Empty)
                        {
                            //MovFunc.saveAfvoerMutation(pToken, farm, ani, pMov, sal);
                        }
                        else pMov.ReportDate = MutDate;
                        writeLog("Afvoer MovId:" + pMov.MovId.ToString());
                    }
                    break;
                case 3: // Geboorte
                    anicat.Anicategory = 1;
                    UBN Fokker = lMstb.GetubnById(farm.UBNid);
                    if (ani.ThrId == 0 && ani.AniId > 0)
                    {
                        ANIMAL check = lMstb.GetAnimalById(ani.AniId);
                        check.ThrId = Fokker.ThrID;
                        lMstb.UpdateANIMAL(Fokker.ThrID, check);
                    }
                    if (ani.AniIdMother > 0)
                    {
                     
                        if (pEvent.EventId == 0)
                        {
                            unLogger.WriteDebug("IRMUT", "Onbrekende Geboorte : ");
                            unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                            unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                            unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                            pEvent.Changed_By = ChangedBy;
                            pEvent.SourceID = farmUBN.ThrID;
                            pEvent.EveMutationBy = 15;
                            pEvent.EveMutationDate = DateTime.Today;
                            pEvent.EveMutationTime = DateTime.Now;
                            pEvent.AniId = ani.AniIdMother;
                            pEvent.EveDate = MutDate;
                            pEvent.EveKind = 5;
                            pEvent.UBNId = farm.UBNid;
                            pEvent.happened_at_FarmID = anicat.FarmId;
                            pEvent.EveOrder = Event_functions.getNewEventOrder(pToken, MutDate, 5, farm.UBNid, ani.AniIdMother);
                            lMstb.SaveEvent(pEvent);
                            BIRTH bir = new BIRTH();
                            bir.EventId = pEvent.EventId;
                            bir.CalfId = ani.AniId;
                            lMstb.SaveBirth(bir);
                            writeLog("Geboorte EventId:" + pEvent.EventId.ToString());
                        }
                        ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);
                        checkForMsOptimaboxRespondernumber(pToken, farm, moeder, false);
                        FaAdviser.saveKetoBoxFeedAdvices(pToken, ani.AniIdMother, pEvent.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                    }
                    else
                    {
                        
                        unLogger.WriteInfo("IRMUT", "Onbrekende Geboorte ZONDER MOEDERDIER! : ");
                        unLogger.WriteInfo("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                        unLogger.WriteInfo("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                        unLogger.WriteInfo("IRMUT", String.Format("UBN: {0}", Fokker.Bedrijfsnummer));
                        //ani.ThrId = Fokker.ThrID;
                        writeLog("Onbrekende Geboorte ZONDER MOEDERDIER! ");
                    }
                    break;
                case 4: // Doodgeboren
                    //Dit is geen doodmelding maar een Doodgeboren melding 
                    //Dus er moet een doodgeboren calf aan dit dier gekoppeld worden.
                    unLogger.WriteDebug("IRMUT", "Onbrekende Doodgeboren : ");
                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                    unLogger.WriteDebug("IRMUT", String.Format("UBN : {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                    StringBuilder bld = new StringBuilder();
                    bld.Append(" SELECT EVENT.* FROM EVENT LEFT JOIN BIRTH ");
                    bld.Append(" ON BIRTH.EventId = EVENT.EventId ");
                    bld.AppendFormat(" WHERE EVENT.AniId={0} AND BIRTH.BornDead=1", ani.AniId);
                    bld.Append(" AND EveKind = 5 AND EVENT.EventId>0 ");
                    bld.Append(" AND date_format(EveDate,'%Y-%m-%d') = '" + MutDate.ToString("yyyy-MM-dd") + "'");
                    bld.AppendFormat(" AND  UBNId = {0} ", farm.UBNid);
                    DataTable tbl = lMstb.GetDataBase().QueryData(pToken.getLastChildConnection(), new DataSet(), bld, "Event", MissingSchemaAction.Add);
                    EVENT doodgeboren = new EVENT();
                    try
                    {
                        if (tbl.Rows.Count > 0)
                        {
                            lMstb.GetDataBase().FillObject(doodgeboren, tbl.Rows[0]);
                        }
                    }
                    catch { }
                    if (doodgeboren.EventId == 0)
                    {
                        BIRTH newBirth = new BIRTH();
                        Event_functions.handleEventTimes(ref doodgeboren, MutDate);
                        doodgeboren.EveKind = 5;
                        doodgeboren.AniId = ani.AniId;
                        doodgeboren.Changed_By = ChangedBy;
                        doodgeboren.SourceID = farmUBN.ThrID;
                        doodgeboren.EveMutationBy = 15;
                        doodgeboren.EveOrder = Event_functions.getNewEventOrder(pToken, MutDate.Date, 5, farm.UBNid, ani.AniId);
                        doodgeboren.happened_at_FarmID = anicat.FarmId;
                        doodgeboren.ThirdId = farmUBN.ThrID;
                        doodgeboren.UBNId = farm.UBNid;
                        newBirth.BornDead = 1;
                        newBirth.BirNumber = Event_functions.getOrCreateBirnr(pToken, ani.AniId, MutDate.Date);
                        if (pDataRow["meldingsnummer"] != DBNull.Value)
                        {
                            newBirth.Meldnummer = pDataRow["meldingsnummer"].ToString();
                        }
                        List<int> lNlingEventsIds = lMstb.getNlingCheckEventIds(farm.UBNid, ani.AniId, newBirth.BirNumber);
                        newBirth.Nling = lNlingEventsIds.Count() + 1;
                        if (lMstb.SaveEvent(doodgeboren))
                        {
                            newBirth.EventId = doodgeboren.EventId;
                            lMstb.SaveBirth(newBirth);
                            writeLog("Doodgeboren EventId:" + doodgeboren.EventId.ToString());
                        }
                    }
                    FaAdviser.saveKetoBoxFeedAdvices(pToken, ani.AniId, doodgeboren.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                    //anicat.Anicategory = 4;
                    //if (ani.AniAlternateNumber != String.Empty)
                    //{
                    //    mov = Facade.GetInstance().getSaveToDB(pToken).GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
                    //    if (mov.MovId == 0)
                    //    {

                    //        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));

                    //        mov.MovMutationBy = 15;
                    //        mov.MovMutationDate = DateTime.Today;
                    //        mov.MovMutationTime = DateTime.Now;
                    //        mov.AniId = ani.AniId;
                    //        mov.MovDate = MutDate;
                    //        mov.ReportDate = MutDate;
                    //        mov.MovKind = 3;
                    //        mov.MovComment = "Doodgeboren";
                    //        mov.MovOrder = MovFunc.getNewMovementOrder(pToken, 3, MutDate, ani.AniId, farm.UBNid);
                    //        mov.UbnId = farm.UBNid;
                    //        mov.Progid = farm.ProgId;
                    //        mov.happened_at_FarmID = anicat.FarmId;
                    //        if (UBN2e != String.Empty)
                    //        {
                    //            UBN ubnto = Facade.GetInstance().getSaveToDB(pToken).getUBNByBedrijfsnummer(UBN2e);
                    //            mov.MovThird_UBNid = ubnto.UBNid;
                    //            mov.ThrId = ubnto.ThrID;
                    //        }
                    //        Facade.GetInstance().getSaveToDB(pToken).SaveMovement(mov);
                    //        LOSS loss = new LOSS();
                    //        loss.MovId = mov.MovId;
                    //        Facade.GetInstance().getSaveToDB(pToken).SaveLoss(loss);
                    //    }
                    //}
                    break;
                case 5: // Import
                    anicat.Anicategory = 1;
                     if (pMov.MovId == 0)
                    {
                        unLogger.WriteDebug("IRMUT", "Onbrekende Import : ");
                        unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                        pMov.Changed_By = ChangedBy;
                        pMov.SourceID = farmUBN.ThrID;
                        pMov.MovMutationBy = 15;
                        pMov.MovMutationDate = DateTime.Today;
                        pMov.MovMutationTime = DateTime.Now;
                        pMov.AniId = ani.AniId;
                        pMov.MovDate = MutDate;
                        pMov.MovTime = MutDate.AddHours(pMov.MovMutationTime.Hour).AddMinutes(pMov.MovMutationTime.Minute).AddSeconds(pMov.MovMutationTime.Second);
                        pMov.ReportDate = MutDate;
                        pMov.MovKind = 1;
                        pMov.MovOrder = MovFunc.getNewMovementOrder(pToken, 1, MutDate, ani.AniId, farm.UBNid);
                        pMov.UbnId = farm.UBNid;
                        pMov.Progid = farm.ProgId;
                        pMov.happened_at_FarmID = farm.FarmId;
                        if (UBN2e != String.Empty)
                        {
                            UBN ubnfrom = lMstb.getUBNByBedrijfsnummer(UBN2e);
                            pMov.MovThird_UBNid = ubnfrom.UBNid;
                            pMov.ThrId = ubnfrom.ThrID;
                        }
                        lMstb.SaveMovement(pMov);
                        BUYING buy = new BUYING();
                        buy.PurKind = 1;
                        buy.MovId = pMov.MovId;
                        lMstb.SaveBuying(buy);
                        writeLog("Import MovId:" + pMov.MovId.ToString());
                        MovFunc.SetSaveAnimalCategory(pToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);


                    }
                    break;
                case 6: // Export
                    anicat.Anicategory = 4;
                    //mov = Facade.GetInstance().getSaveToDB(pToken).GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                    if (pMov.MovId == 0)
                    {
                        unLogger.WriteDebug("IRMUT", "Onbrekende Export : ");
                        unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                        unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", UBN2e));

                        pMov.Changed_By = ChangedBy;
                        pMov.SourceID = farmUBN.ThrID;
                        pMov.MovMutationBy = 15;
                        pMov.MovMutationDate = DateTime.Today;
                        pMov.MovMutationTime = DateTime.Now;
                        pMov.AniId = ani.AniId;
                        pMov.MovDate = MutDate;
                        pMov.MovTime = MutDate.AddHours(pMov.MovMutationTime.Hour).AddMinutes(pMov.MovMutationTime.Minute).AddSeconds(pMov.MovMutationTime.Second);
                        pMov.ReportDate = MutDate;
                        pMov.MovKind = 2;
                        pMov.MovOrder = MovFunc.getNewMovementOrder(pToken, 2, MutDate, ani.AniId, farm.UBNid);
                        pMov.UbnId = farm.UBNid;
                        pMov.Progid = farm.ProgId;
                        pMov.happened_at_FarmID = anicat.FarmId;
                        if (UBN2e != String.Empty)
                        {
                            UBN ubnto = lMstb.getUBNByBedrijfsnummer(UBN2e);
                            pMov.MovThird_UBNid = ubnto.UBNid;
                            pMov.ThrId = ubnto.ThrID;
                        }
                        lMstb.SaveMovement(pMov);
                        SALE sal = new SALE();
                        sal.MovId = pMov.MovId;
                        sal.SalKind = 1;
                        sal.Changed_By = ChangedBy;
                        sal.SourceID = farmUBN.ThrID;
                        lMstb.SaveSale(sal);
                        MovFunc.SetSaveAnimalCategory(pToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                        MovFunc.returnTransmitters(pToken, farm, pMov.AniId);
                        writeLog("Export MovId:" + pMov.MovId.ToString());
                    }
                    break;
                case 7: // Dood

                    anicat.Anicategory = 4;
                    if (pMov.MovId == 0)
                    {
                        unLogger.WriteDebug("IRMUT", "Onbrekende Doodmelding : ");
                        unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                        unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                        pMov.Changed_By = ChangedBy;
                        pMov.SourceID = farmUBN.ThrID;
                        pMov.MovMutationBy = 15;
                        pMov.MovMutationDate = DateTime.Today;
                        pMov.MovMutationTime = DateTime.Now;
                        pMov.AniId = ani.AniId;
                        pMov.MovDate = MutDate;
                        pMov.MovTime = MutDate.AddHours(pMov.MovMutationTime.Hour).AddMinutes(pMov.MovMutationTime.Minute).AddSeconds(pMov.MovMutationTime.Second);
                        pMov.ReportDate = MutDate;
                        pMov.MovKind = 3;
                        pMov.MovOrder = MovFunc.getNewMovementOrder(pToken, 3, MutDate, ani.AniId, farm.UBNid);
                        pMov.UbnId = farm.UBNid;
                        pMov.Progid = farm.ProgId;
                        pMov.happened_at_FarmID = anicat.FarmId;
               
                        if (UBN2e != String.Empty)
                        {
                            UBN ubnto = lMstb.getUBNByBedrijfsnummer(UBN2e);
                            pMov.MovThird_UBNid = ubnto.UBNid;
                            pMov.ThrId = ubnto.ThrID;
                        }
                        lMstb.SaveMovement(pMov);
                        LOSS loss = new LOSS();
                        loss.MovId = pMov.MovId;
                        loss.Changed_By = ChangedBy;
                        loss.SourceID = farmUBN.ThrID;
                        lMstb.SaveLoss(loss);
                        MovFunc.SetSaveAnimalCategory(pToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                        MovFunc.returnTransmitters(pToken, farm, pMov.AniId);
                        writeLog("Dood MovId:" + pMov.MovId.ToString());
                        List<ANIMALCATEGORY> allcats = lMstb.GetAnimalCategoryById(anicat.AniId);
                        foreach (ANIMALCATEGORY cat in allcats)
                        {
                            if (cat.Anicategory < 4)
                            {
                                cat.Anicategory = 4;
                                lMstb.SaveAnimalCategory(cat);
                            }
                        }
                    }
                    break;


                case 8: // Diervlag             
                    unLogger.WriteDebug("IRMUT", "Onbrekende Diervlag : ");
                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                    unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));
                    break;
                case 9: // Omnummering             
                    unLogger.WriteDebug("IRMUT", "Onbrekende Omnummering : ");
                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                    unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", lMstb.GetubnById(farm.UBNid).Bedrijfsnummer));

                    String MeldingNummer = pDataRow["meldingsnummer"].ToString();
                    int lDetailMeldingType = 9;
                    int lDetailMeldingStatus = lStatusMelding;
                    DateTime pGebeurtenisDatum = MutDate;
                    DateTime pHerstelDatum = DateTime.MinValue;
                    DateTime pIntrekDatum = DateTime.MinValue;
                    String lLevensnr_Oud = pDataRow["Levensnummer"].ToString();
                    String lLevensnr_Nieuw = String.Empty;
                    Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
                    int progid = farm.ProgId;
                    int MaxString = 255;
                    String lUsername = "";
                    String lPassword = "";

                    string lLevensnr = "";
                    String lStatus = string.Empty;
                    String lCode = string.Empty;
                    String lOmschrijving = string.Empty;

                    FTPUSER fusoap = lMstb.GetFtpuser(farmUBN.UBNid, farm.Programid, farm.ProgId, 9992);

                    if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)// && LNVPasswordCheck(fusoap.UserName, fusoap.Password) == 1)
                    {

                        lUsername = fusoap.UserName;
                        lPassword = fusoap.Password;
                    }


                    String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MeldingDetailsV2_" + farmUBN.Bedrijfsnummer + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";
                    DLLcall.LNVIRRaadplegenMeldingDetailsV2(lUsername, lPassword, 0,
                         farmUBN.Bedrijfsnummer, farmThird.Thr_Brs_Number, MeldingNummer,
                         ref progid, ref lDetailMeldingType, ref lDetailMeldingStatus,
                         ref MutDate, ref pHerstelDatum, ref pIntrekDatum,
                         ref lLevensnr_Oud, ref  lLevensnr_Nieuw,
                         ref lStatus, ref lCode, ref lOmschrijving,
                         LogFile, MaxString);
                    DLLcall.Dispose();

                    if (lLevensnr_Nieuw != null && lLevensnr_Nieuw.Length > 0)
                    {
                        if (lLevensnr_Oud != null && lLevensnr_Oud.Length > 0)
                        {
                            ANIMAL aniNieuw = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Nieuw);
                            ANIMAL aniOud = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Oud);
                            if (farm.ProgId != 3 && farm.ProgId != 5)
                            {
                                aniNieuw = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Nieuw);
                                aniOud = lMstb.GetAnimalByAniAlternateNumber(lLevensnr_Oud);
                            }

                            if (aniNieuw.AniId > 0 && aniOud.AniId > 0)
                            {
                                if (aniNieuw.AniId != aniOud.AniId)
                                {
                                    writeLog("ANIMAL_ERROR omnummer verandering AniId:" + aniNieuw.AniId.ToString() + " " + aniNieuw.AniAlternateNumber + " naar AniId:" + aniOud.AniId.ToString() + " " + aniOud.AniAlternateNumber);
                                    lMstb.InsertAnimalError(aniNieuw.AniId, aniOud.AniId);
                                }

                            }
                            else if (aniNieuw.AniId > 0)
                            {
                                List<LEVNRMUT> muta = lMstb.getLevNrMuts(aniNieuw.AniId);
                                var c = from n in muta
                                        where n.LevnrOud == lLevensnr_Oud
                                        select n;
                                if (c.Count() == 0)
                                {
                                    LEVNRMUT lnM = new LEVNRMUT();
                                    lnM.Aniid = aniNieuw.AniId;
                                    lnM.Datum = MutDate;
                                    lnM.LevnrNieuw = lLevensnr_Nieuw;
                                    lnM.LevnrOud = lLevensnr_Oud;
                                    lnM.LNVMeldNr = MeldingNummer;
                                    lMstb.saveLevNrMut(lnM);
                                }

                            }
                            else if (aniOud.AniId > 0)
                            {
                                List<LEVNRMUT> muta = lMstb.getLevNrMuts(aniOud.AniId);
                                MovFunc.NummerDierOm(pToken, farm, farmUBN, aniOud, lLevensnr_Nieuw, MeldingNummer,ChangedBy,farmUBN.ThrID);
                            }

                        }
                    }

                    //Result = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingDetailsV2(pToken, farm.UBNid, farm.ProgId,
                    //                 MeldingNummer, lDetailMeldingType, lDetailMeldingStatus,
                    //                 Begindatum, lLevensnr_Oud, lLevensnr_Nieuw);


                    break;
                default:
                    String Lifenr = pDataRow["Levensnummer"].ToString();
                    unLogger.WriteInfo(String.Format("Onbekende Melding van RVO : Datum {0}  Levensnr : {1} LNVType : {2}", MutDate, Lifenr, lMeldingType));
                    break;

            }
            MovFunc.SetSaveAnimalCategory(pToken, lMstb.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);

        }

        private void changeHerstelMelding(UserRightsToken pToken, ANIMAL ani, ANIMALCATEGORY anicat, BEDRIJF farm, UBN farmUBN, THIRD farmThird, DateTime MutDate, int pMeldingType, DataRow pDataRow)
        {
           
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pToken);
            int lMeldingType2 = Convert.ToInt32(pDataRow["Meldingtype"]);
            int lStatusMelding2 = Convert.ToInt32(pDataRow["meldingstatus"]);
            DateTime MutDate2 = utils.getDateLNV(pDataRow["Datum"].ToString());
            String UBN2e2 = pDataRow["ubn2ePartij"].ToString();
            string meldingsnummer2 = pDataRow["meldingsnummer"].ToString();

            UBN ubntweede2 = new UBN();
            if (UBN2e2 != String.Empty)
            {
                ubntweede2 = lMstb.getUBNByBedrijfsnummer(UBN2e2);

            }

            MOVEMENT mov1;//Oude movement
            EVENT eve1;//Oude Event

            switch (pMeldingType)
            {
                case 1: // Aanvoer
                    mov1 = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                    if (mov1.MovId > 0)
                    {
                        if (MutDate2.Date != mov1.MovDate.Date)
                        {
                            MovFunc.HandleMovTimes(pToken, MutDate2, mov1.MovTime.ToString("HH:mm"), ref mov1);
                        }
                        if (ubntweede2.UBNid > 0)
                        {
                            writeLog("Herstelde Aanvoer MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                            BUYING buy = lMstb.GetBuyingByMovId(mov1.MovId);
                            buy.PurOrigin = ubntweede2.ThrID;
                            lMstb.SaveBuying(buy);
                        }
                        lMstb.SaveMovement(mov1);
                    }

                    break;
                case 2: // Afvoer
                    mov1 = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                    if (mov1.MovId > 0)
                    {
                        writeLog("Herstelde Afvoer MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                        if (MutDate2.Date != mov1.MovDate.Date)
                        {
                            MovFunc.HandleMovTimes(pToken, MutDate2, mov1.MovTime.ToString("HH:mm"), ref mov1);
                        }
                        if (ubntweede2.UBNid > 0)
                        {
                            writeLog("Herstelde Aanvoer MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                            SALE sal = lMstb.GetSale(mov1.MovId);
                            sal.SalDestination = ubntweede2.ThrID;
                            lMstb.SaveSale(sal);
                        }
                        lMstb.SaveMovement(mov1);
                    }

                    break;
                case 3: // Geboorte
                    BIRTH bir = lMstb.GetBirthByCalfId(ani.AniId);
                    eve1 = lMstb.GetEventdByEventId(bir.EventId);
                    if (eve1.EventId > 0)
                    {
                        writeLog("Herstelde Geboorte EveId:" + eve1.EventId.ToString() + " AniId:" + ani.AniId.ToString());

                        if (MutDate2.Date != eve1.EveDate.Date)
                        {
                            Event_functions.handleEventTimes(ref eve1, MutDate2);
                            ani.AniBirthDate = MutDate2.Date;
                            lMstb.UpdateANIMAL(farmUBN.ThrID, ani);
                            lMstb.SaveEvent(eve1);
                        }
                        ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);
                        checkForMsOptimaboxRespondernumber(pToken, farm, moeder, false);
                        Feedadviceer FaAdviser = new Feedadviceer();
                        FaAdviser.saveKetoBoxFeedAdvices(pToken, ani.AniIdMother, eve1.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                    }

                    break;
                case 4: // Doodgeboren

                    break;
                case 5: // Import                               
                    mov1 = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);

                    if (mov1.MovId > 0)
                    {
                        writeLog("Herstelde  Import MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                        if (MutDate2.Date != mov1.MovDate.Date)
                        {
                            MovFunc.HandleMovTimes(pToken, MutDate2, mov1.MovTime.ToString("HH:mm"), ref mov1);
                        }
                        if (ubntweede2.UBNid > 0)
                        {
                            writeLog("Herstelde Aanvoer MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                            BUYING buy = lMstb.GetBuyingByMovId(mov1.MovId);
                            buy.PurOrigin = ubntweede2.ThrID;
                            lMstb.SaveBuying(buy);
                        }
                        lMstb.SaveMovement(mov1);
                    }

                    break;
                case 6: // Export
                    mov1 = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                    if (mov1.MovId > 0)
                    {
                        writeLog("Herstelde Export MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                        if (MutDate2.Date != mov1.MovDate.Date)
                        {
                            MovFunc.HandleMovTimes(pToken, MutDate2, mov1.MovTime.ToString("HH:mm"), ref mov1);
                        }
                        if (ubntweede2.UBNid > 0)
                        {
                            writeLog("Herstelde Aanvoer MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                            SALE sal = lMstb.GetSale(mov1.MovId);
                            sal.SalDestination = ubntweede2.ThrID;
                            lMstb.SaveSale(sal);
                        }
                        lMstb.SaveMovement(mov1);
                    }

                    break;
                case 7: // Dood
                    mov1 = lMstb.GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
                    if (mov1.MovId > 0)
                    {
                        if (MutDate2.Date != mov1.MovDate.Date)
                        {
                            writeLog("Herstelde Dood MovId:" + mov1.MovId.ToString() + " AniId:" + ani.AniId.ToString());
                            MovFunc.HandleMovTimes(pToken, MutDate2, mov1.MovTime.ToString("HH:mm"), ref mov1);
                            lMstb.SaveMovement(mov1);

                        }

                    }
                    break;
                default:
                    String Lifenr = pDataRow["Levensnummer"].ToString();
                    unLogger.WriteInfo(String.Format("Onbekende Melding van RVO : Datum {0}  Levensnr : {1} LNVType : {2}", MutDate, Lifenr, pMeldingType));
                    break;
            }
        }

        private void getHerstelDetail(UserRightsToken pToken, UBN farmUBN, BEDRIJF farm, THIRD farmThird, string meldingsnummer,
            out DateTime pMutDateDetail, out DateTime pHerstelDatum, out DateTime pIntrekDatum)
        {
            String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MeldingDetailsV2_" + farmUBN.Bedrijfsnummer + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";

            //Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername = "";
            String lPassword = "";
            DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);


            FTPUSER fusoap = DB.GetFtpuser(farmUBN.UBNid, farm.Programid, farm.ProgId, 9992);

            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)// && LNVPasswordCheck(fusoap.UserName, fusoap.Password) == 1)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }

            String LogFileDetail = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MeldingDetailsV2_" + farmUBN.Bedrijfsnummer + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";

            // de refs
            int progid = farm.ProgId;
            int lDetailMeldingType = 0;
            int lDetailMeldingStatus = 0;
            pMutDateDetail = DateTime.MinValue;
            pHerstelDatum = DateTime.MinValue;
            pIntrekDatum = DateTime.MinValue;
            String lLevensnr_Oud = String.Empty;
            String lLevensnr_Nieuw = String.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;

            MeldingenWS slnv = new MeldingenWS();
            MeldingDetails mdetls = new MeldingDetails();
            slnv.raadplegenMeldingDetail(lUsername, lPassword, 0,
                 farmUBN.Bedrijfsnummer, farmThird.Thr_Brs_Number, meldingsnummer,
                 ref progid, ref lDetailMeldingType, ref lDetailMeldingStatus,
                 ref pMutDateDetail, ref pHerstelDatum, ref pIntrekDatum,
                 ref lLevensnr_Oud, ref lLevensnr_Nieuw,ref mdetls,
                 ref lStatus, ref lCode, ref lOmschrijving);

            //DLLcall.LNVIRRaadplegenMeldingDetailsV2(lUsername, lPassword, 0,
            //     farmUBN.Bedrijfsnummer, farmThird.Thr_Brs_Number, meldingsnummer,
            //     ref progid, ref lDetailMeldingType, ref lDetailMeldingStatus,
            //     ref pMutDateDetail, ref pHerstelDatum, ref pIntrekDatum,
            //     ref lLevensnr_Oud, ref  lLevensnr_Nieuw,
            //     ref lStatus, ref lCode, ref lOmschrijving,
            //     LogFileDetail, MaxString);
            //DLLcall.Dispose();

            //SOAPLOG ResultDetail = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingDetailsV2(pToken,
            //    farm.UBNid, farm.ProgId, farm.Programid, meldingsnummer, lMeldingType, lStatusMelding, MutDate, row["Levensnummer"].ToString(), lLevensnr_Nieuw);
            unLogger.WriteDebug("LNVIRRaadplegenMeldingDetailsV2:result: progid:" + progid.ToString() + ", MeldingType:" + lDetailMeldingType.ToString() + ", MeldingStatus:" + lDetailMeldingStatus.ToString() + ", Date:" +
                 pMutDateDetail.ToString("dd-MM-yyyy") + ", lLevensnr_Oud:" + lLevensnr_Oud.ToString() + ", lLevensnr_Nieuw:" + lLevensnr_Nieuw +
                 ", lStatus:" + lStatus + ", lCode:" + lCode + ", lOmschrijving:" + lOmschrijving);

            string name = lLevensnr_Oud.Replace(" ", "");
            String OutputHerstelFile = unLogger.getLogDir("CSVData") + "_Herstel.csv";

        }

        private bool checkVervangenLevensnr(UserRightsToken pToken, BEDRIJF farm, ANIMAL ani, String Lifenr, string pUsername, string pPassword, Win32SOAPIRALG pDllCall, out string pVervangenLevensnr)
        {
            bool ret = true;
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pToken);
            UBN u = lMstb.GetubnById(farm.UBNid);
            THIRD lPersoon = lMstb.GetThirdByThirId(u.ThrID);
            //int standaardgeslacht = 0;
            //int.TryParse(lMstb.GetFarmConfigValue(farm.FarmId, "standaardgeslacht", "0"), out standaardgeslacht);
            //if (standaardgeslacht == 0)
            //{
            //    int.TryParse(Facade.GetInstance().getSaveToDB(pToken).GetProgramConfigValue(farm.Programid, "standaardgeslacht", "0"), out standaardgeslacht);

            //}
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            String BRSnr = lPersoon.Thr_Brs_Number;
            pVervangenLevensnr = "";
            int LNVprognr = 0;
            string BRSnrHouder = String.Empty;
            string UBNhouder = String.Empty;
            string Werknummer = String.Empty;
            DateTime Geboortedat = DateTime.MinValue;
            DateTime Importdat = DateTime.MinValue;
            string LandCodeHerkomst = String.Empty;
            string LandCodeOorsprong = String.Empty;
            string Geslacht = String.Empty;
            string Haarkleur = String.Empty;
            DateTime Einddatum = DateTime.MinValue;
            string RedenEinde = String.Empty;
            string LevensnrMoeder = String.Empty;
            string VervangenLevensnr = String.Empty;
            string Status = String.Empty;
            string Code = String.Empty;
            string Omschrijving = String.Empty;
            string pLogfile = Path.Combine(unLogger.getLogDir(),"IenR",$@"Details_{u.Bedrijfsnummer}_{Lifenr.Replace(" ","")}_{tijd}.log");
            int pMaxStrLen = 255;
            SOAPLOG sl = new SOAPLOG
            {
                Changed_By = ChangedBy,
                Code = "",
                Date = DateTime.Now.Date,
                FarmNumber = u.Bedrijfsnummer,
                Kind = (int)LABELSConst.SOAPLOGKind.LNV_Meldingen_Raadplegen_Details,
                Lifenumber = Lifenr,
                Omschrijving = "",
                SourceID = u.ThrID,
                Status = "G",
                SubKind = 0,
                TaskLogID = TasklogID,
                ThrId = u.ThrID,
                Time = DateTime.Now
            };
            SOAPLNVDieren slnv = new SOAPLNVDieren();
 
            slnv.LNVDierdetailsV2(pUsername, pPassword, 0, u.Bedrijfsnummer, lPersoon.Thr_Brs_Number, Lifenr,
                farm.ProgId, 0, 0, 1, ref Werknummer,
                                        ref Geboortedat, ref Importdat,
                                        ref LandCodeHerkomst, ref LandCodeOorsprong,
                                        ref Geslacht, ref Haarkleur,
                                        ref Einddatum, ref RedenEinde,
                                        ref LevensnrMoeder, ref VervangenLevensnr,
                                        ref Status, ref Code, ref Omschrijving);
            sl.Status = Status;
            sl.Code = Code;
       
            //pDllCall.LNVDierdetailsV2(pUsername, pPassword, 0,
            //                            u.Bedrijfsnummer, BRSnr, Lifenr, farm.ProgId,
            //                            0, 0, 0,
            //                            ref LNVprognr, ref Werknummer,
            //                            ref Geboortedat, ref Importdat,
            //                            ref LandCodeHerkomst, ref LandCodeOorsprong,
            //                            ref Geslacht, ref Haarkleur,
            //                            ref Einddatum, ref RedenEinde,
            //                            ref LevensnrMoeder, ref VervangenLevensnr,
            //                            ref Status, ref Code, ref Omschrijving,
            //                            "", pLogfile, pMaxStrLen);

            //sl.Status = Status;
            //sl.Code = Code;
            //sl.Omschrijving = Omschrijving;
            //if (!string.IsNullOrEmpty(sl.Omschrijving))
            //{
            //    sl.Omschrijving = $@"User:{pUsername} {sl.Omschrijving}";
            //}
            //lMstb.WriteSoapError(sl);
            try
            {
                //    XDocument gegevens = new XDocument();
                //    gegevens.Add(new XElement("root"));
                //    gegevens.Root.Add(new XElement("Animal", new XAttribute("AniAlternateNumber", Lifenr)),
                //        new XElement("LNVprognr", LNVprognr),
                //        new XElement("Werknummer", Werknummer),
                //        new XElement("Geboortedat", Geboortedat),
                //        new XElement("Importdat", Importdat),
                //        new XElement("LandCodeHerkomst", LandCodeHerkomst),
                //        new XElement("LandCodeOorsprong", LandCodeOorsprong),
                //        new XElement("Geslacht", Geslacht),
                //        new XElement("Haarkleur", Haarkleur),
                //        new XElement("Einddatum", Einddatum),
                //        new XElement("RedenEinde", RedenEinde),
                //        new XElement("LevensnrMoeder", LevensnrMoeder),
                //        new XElement("VervangenLevensnr", VervangenLevensnr),
                //        new XElement("Status", Status),
                //        new XElement("Code", Code),
                //        new XElement("Omschrijving", Omschrijving));
                //    DateTime opvraag = DateTime.Now;
                //    string bestandfile = Win32SOAPIRALG.GetBaseDir() + "\\log\\LNVDierdetailsV2_" + Lifenr.Replace(" ", "") + "_" + opvraag.ToString("yyyyMMddHHmmss") + ".xml";
                //    gegevens.Save(bestandfile);
                unLogger.WriteInfo("LNVDierdetailsV2: " + Lifenr + " LNVprognr:" + LNVprognr.ToString() + ", Werknummer: " + Werknummer + " " +
                                      "  Geboortedat: " + Geboortedat.ToString() + " Importdat: " + Importdat.ToString() +
                                      "  LandCodeHerkomst:" + LandCodeHerkomst + "  LandCodeOorsprong:" + LandCodeOorsprong +
                                      "    Geslacht" + Geslacht + "  Haarkleur: " + Haarkleur +

                                      "   LevensnrMoeder: " + LevensnrMoeder + " " +
                                      "   Status: " + Status + " Omschrijving:" + Omschrijving);
            }
            catch (Exception exc) { unLogger.WriteError("Error writing LNVDierdetailsV2: tbv parameter LiFenumber " + Lifenr + " LogInfo Mutationupdater : checkVervangenLevensnr " + exc.ToString()); }
            sl.Omschrijving = string.IsNullOrEmpty(Omschrijving) ? "" : $@"User:{pUsername}: {Omschrijving}";

            if (VervangenLevensnr != null && VervangenLevensnr.Length > 0)
            {
                pVervangenLevensnr = VervangenLevensnr;
                List<LEVNRMUT> muts = lMstb.getLevNrMutsByLifeNr(VervangenLevensnr);
                if (muts.Count() > 0 && muts.ElementAt(0).Aniid > 0)
                {
                    ani = lMstb.GetAnimalById(muts.ElementAt(0).Aniid);
                }
                else
                {
                    ANIMAL checkAnimal = lMstb.GetAnimalByAniAlternateNumber(VervangenLevensnr);
                    if (farm.ProgId != 3 && farm.ProgId != 5)
                    {

                        checkAnimal = lMstb.GetAnimalByLifenr(VervangenLevensnr);
                    }

                    if (VervangenLevensnr == Lifenr)
                    {
                        if (checkAnimal.AniId > 0)
                        {
                            LEVNRMUT nM = new LEVNRMUT();
                            nM.Aniid = checkAnimal.AniId;
                            nM.LevnrOud = VervangenLevensnr;
 
                            nM.SourceID = lPersoon.ThrId;
                            nM.Changed_By = ChangedBy;
 
                            lMstb.saveLevNrMut(nM);
                            ani = checkAnimal;
                           
                        }
                        else
                        {
                            //Het dier is omgenummerd maar wij weten niet naar welk dier
                            //dan gaan we niet een nieuw dier aanmaken maar mag je niet verder gaan
                            //dan moetde klant doorgaan met opvragen 
                            //totdat VervangenLevensnr != Lifenr
                            //Dan zie hieronder
                            ret = false;
                        }

                    }
                    else
                    {


                        if (checkAnimal.AniId > 0)//Dit dier is Omgenummerd naar pLifeNumber bij RVO
                        {
                            string lnvOmnummerNr = "";
                            List<LEVNRMUT> lMuts = lMstb.getLevNrMuts(checkAnimal.AniId);
                            if (lMuts.Count() > 0)
                            {
                                lnvOmnummerNr = lMuts.ElementAt(0).LNVMeldNr;
                            }

                            MovFunc.NummerDierOm(pToken, farm, u, checkAnimal, Lifenr, lnvOmnummerNr,ChangedBy,u.ThrID);
                         
                            ani = checkAnimal;
                        }
                        else
                        {
                            //wij kunnen het dier niet vinden 
                            //dat is omgenummerd
                            //maar je moet wel doorgaan
                            //en eigenlijk een LEVNRMUT opslaan met VervangenLevensnr
                            //Als AniId bekend is
                        }
                    }
                }
            }
            if (ani.AniId <= 0 && ret)
            {
                // meer info ophalen
                sl.Omschrijving = string.IsNullOrEmpty(Omschrijving) ? "New Animal." : sl.Omschrijving;
                DateTime mindate = new DateTime(1900, 1, 1);
                //if (Geboortedat > mindate)
                //{
                ani.Changed_By = ChangedBy;
                ani.SourceID = lPersoon.ThrId;
                ani.AniAlternateNumber = Lifenr;
                ani.AniLifeNumber = Lifenr;
                ani.AniHaircolor_Memo = Haarkleur;// Facade.GetInstance().getSaveToDB(pToken).GetHcoId(Haarkleur);
                ani.AniBirthDate = Geboortedat;
                if (Geslacht.ToLower() == "m")
                    ani.AniSex = 1;
                else if (Geslacht.ToLower() == "v")
                    ani.AniSex = 2;
                //else
                //    ani.AniSex = standaardgeslacht;
                if (LevensnrMoeder != String.Empty)
                {
                    ANIMAL aniMother = lMstb.GetAnimalByAniAlternateNumber(LevensnrMoeder);
                    if (aniMother.AniId == 0)
                    {
                        VulDiergegevensvanuitLNV(pToken, farm, aniMother, LevensnrMoeder);
                    }
                    if (ani.AniIdDam == 0 || ani.AniIdMother <= 0)
                        ani.AniIdMother = aniMother.AniId;
                }
 


                ani.Changed_By = (int)LABELSConst.ChangedBy.MutationUpdatercheckVervangenLevensnr;
                ani.ThrId = u.ThrID;


                lMstb.SaveAnimal( ani, -99, LABELSConst.ChangedBy.MutationUpdatercheckVervangenLevensnr);
                ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
 
                if (anicat.FarmId == 0 || anicat.UbnId==0)
                {
                    anicat.AniId = ani.AniId;
                    anicat.AniWorknumber = Werknummer;
                    anicat.FarmId = farm.FarmId;
                    anicat.Anicategory = 5;
                    anicat.UbnId = farm.UBNid;
                    anicat.Changed_By = ChangedBy;
                    anicat.SourceID = lPersoon.ThrId;
 
                    lMstb.SaveAnimalCategory(anicat);
                }
                if (VervangenLevensnr.Length > 0)
                {
                    List<LEVNRMUT> muts = lMstb.getLevNrMutsByLifeNr(VervangenLevensnr);
                    if (muts.Count() > 0 && muts.ElementAt(0).Aniid > 0)
                    {

                    }
                    else
                    {
                        LEVNRMUT lM = new LEVNRMUT();
                        lM.Aniid = ani.AniId;
                        lM.LevnrOud = VervangenLevensnr;
                        lM.LevnrNieuw = ani.AniAlternateNumber;
 
                        lM.Changed_By = ChangedBy;
                        lM.SourceID = lPersoon.ThrId;
 
                        lMstb.saveLevNrMut(lM);
                    }
                }
                //}

                ret = false;
            }
            
            lMstb.WriteSoapError(sl);
            return ret;
        }

        public void VulDiergegevensvanuitLNV(UserRightsToken pToken, BEDRIJF farm, ANIMAL ani, String Lifenr)
        {
            Win32SOAPIRALG dllcall = new Win32SOAPIRALG();
       
            String lUsername = "";
            String lPassword = "";
            DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
            FTPUSER fusoap = DB.GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9992);
   
            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }

            string pVervangenLevensnr = "";
            bool GoOn = checkVervangenLevensnr(pToken, farm, ani, Lifenr, lUsername, lPassword, dllcall, out pVervangenLevensnr);

            dllcall.Dispose();
        }

        private void writeLog(string pText)
        {
            try
            {
                if (lLogFilePath != null)
                {
                    if (lLogFilePath.Length > 0)
                    {
                        StreamWriter wr = new StreamWriter(lLogFilePath, true);
                        try
                        {
                            wr.WriteLine(pText);
                            wr.Flush();
                        }
                        catch (Exception exc) { unLogger.WriteDebug("MutationUpdaterA:" + exc.ToString()); }
                        finally
                        {
                            wr.Close();
                        }
                    }
                }
            }
            catch (Exception exc) { unLogger.WriteDebug("MutationUpdaterB:" + exc.ToString()); }
        }

        public SOAPLOG DoeHitierMeldingenRaadplegen(UserRightsToken pTokenA, BEDRIJF farm, String lLevensnr, int MeldingSoort, DateTime Begindatum, DateTime Einddatum)
        {
            SOAPLOG Result = new SOAPLOG();
            Result.Kind = 1400;
            UserRightsToken lToken = (UserRightsToken)pTokenA.Clone();
            MovFuncEvent mvEvent = new MovFuncEvent();
         
            DB.DBMasterQueries DB = new DB.DBMasterQueries(lToken);
            FARMCONFIG fcon = DB.getFarmConfig(farm.FarmId, "rfid", "1");
            Feedadviceer FaAdviser = new Feedadviceer();
            unLogger.WriteInfo("DoeHitierMeldingenRaadplegen");
            String Output = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farm.UBNid + "_" + DateTime.Now.Ticks + ".csv";
          
            int pTestServer = 0;
            HIClient hiCL = new HIClient(pTestServer, unLogger.getLogDir("CSVData"), unLogger.getLogDir("CSVData"));
            Result = hiCL.HtierRaadplegenmeldingen(lToken, farm.UBNid, farm.ProgId, farm.Programid, Begindatum, Einddatum, Output);
            Result.Changed_By = ChangedBy;
            SOAPLOG uit = new SOAPLOG();
            DataTable stallijst = hiCL.getHtierStallijstPreloaded(lToken, farm.UBNid, farm.ProgId, farm.Programid, Begindatum, Einddatum, out uit);

            if (stallijst.Rows.Count > 0 && Begindatum.Date <= DateTime.Now.Date && Einddatum.Date >= DateTime.Now.Date)
            {
                List<int> idnumbers = new List<int>();
                List<string> levnrs = new List<string>();
                foreach (DataRow rw in stallijst.Rows)
                {
                    string aa = hiCL.getLifeNumberFromLOM_X(rw["LOM_X"].ToString());
                    if (!string.IsNullOrEmpty(aa) && aa != "%--")
                    {
                        levnrs.Add(aa);
                        
                    }
                }
                if (levnrs.Count > 0)
                {
                    StringBuilder b = new StringBuilder();
                    b.Append($@"SELECT a.AniId,a.AniAlternatenumber FROM ANIMALCATEGORY ac 
                                JOIN ANIMAL a ON a.AniID=ac.AniID 
                                WHERE a.AniId>0
                                AND ac.UbnID={farm.UBNid}
                                AND ac.AniCategory>3
                                AND a.AniAlternatenumber  IN ('{string.Join("','", levnrs)}')");
                    DataTable t = DB.GetDataBase().QueryData(lToken.getLastChildConnection(),b);
                    foreach (DataRow r in t.Rows)
                    {
                        idnumbers.Add(Convert.ToInt32(r["AniId"]));
                        unLogger.WriteInfo($@"{r["AniAlternatenumber"]}  AniCategory=>1 for UbnId={farm.UBNid} AND AniId={r["AniId"]}");
                    }
                }
                if (idnumbers.Count > 0)
                {
               
                    StringBuilder b = new StringBuilder();
                    b.Append($@"UPDATE ANIMALCATEGORY SET AniCategory=1, Changed_By={ChangedBy} WHERE UbnID={farm.UBNid} AND AniId IN ({string.Join(",", idnumbers)})");
                    DB.ExecuteQuery(b, lToken.getLastChildConnection());
                }
            }
            Result.TaskLogID = TasklogID > 0 ? TasklogID : Result.TaskLogID;
            DB.WriteSoapError(Result);
           
            string[] Kols = { "LOM", "Levensnummer", "Meldingtype", "Datum", "ubn2ePartij", "meldingsnummer", "meldingstatus", "hersteld" };

            char spl = ';';
            DataTable tblHtier = utils.GetCsvData(Kols, Output, spl, "Mutaties");
            try
            {
                tblHtier.DefaultView.Sort = "Levensnummer,Datum";//Chronologisch volgens logica
                tblHtier = tblHtier.DefaultView.ToTable(true);
            }
            catch { }//gaat fout als tblLNV.rows.count =0;

            UBN farmUBN = DB.GetubnById(farm.UBNid);
            THIRD farmThird = DB.GetThirdByThirId(farmUBN.ThrID);
            bool IRAanvoer = Convert.ToBoolean(DB.GetConfigValue(farm.Programid, farm.FarmId, "IRAanvoerAutoRecieve", "True"));
            FARMCONFIG fcIR = DB.getFarmConfig(farm.FarmId, "IRaanvoer", "True");

            int[] aStatusMeldingen = { 0, 1, 2, 4, 5 };
            if (tblHtier.Rows.Count == 0)
            {
                if (RequestUpdate != null)
                {

                    mvEvent.Progress = 100;
                    if (Result.Code == "IRD-00364")
                    {
                        mvEvent.Message = " Klaar Er zijn geen dieren verwerkt, want zijn meer dan 2500 meldingen gevonden, probeer een kortere periode.";
                        RequestUpdate(this, mvEvent);
                    }
                    else
                    {
                        if (Result.Omschrijving != "")
                        {
                            mvEvent.Message = " Klaar Er zijn geen dieren verwerkt. " + Result.Omschrijving;
                            RequestUpdate(this, mvEvent);
                        }
                        else
                        {
                            mvEvent.Message = " Klaar Er zijn geen meldingen gevonden voor deze periode. ";
                            RequestUpdate(this, mvEvent);
                        }
                    }
                }
                else 
                {
                    if (Result.Omschrijving != "")
                    {
                       
                    }
                    else
                    {
                        mvEvent.Message = " Er zijn geen meldingen gevonden voor deze periode. ";
                        RequestUpdate(this, mvEvent);
                    }
                }
            }
            else
            {
                int teller = 0;
                int totaal = tblHtier.Rows.Count;
                foreach (DataRow row in tblHtier.Rows)
                {
                    int procent = teller * 100 / totaal;

                    if (RequestUpdate != null)
                    {

                        mvEvent.Progress = procent;
                        mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("verwerken", "Verwerken") + " " + row["Levensnummer"].ToString();
                        RequestUpdate(this, mvEvent);
                    }
                    teller += 1;
                    int lMeldingtype = Convert.ToInt32(row["Meldingtype"]);
                    string meldingsnummer = row["meldingsnummer"].ToString();//is vooralsnog leeg want ik kan die nog nergens uithalen
                    DateTime MutDate = utils.getDateLNV(row["Datum"].ToString());

                    ANIMAL ani = DB.GetAnimalByAniAlternateNumber(row["LOM"].ToString());

                    if (ani.AniId <= 0) // bestaat nog niet in agrobase
                    {
                        lock (padlock) // 2014-05-01 als 2 administraties hetzelfde dier bevatten komt hij dubbel in agrobase
                        {
                            // binnen de lock nogmaals checken, misschien is hij inmiddels door een andere thread aangemaakt.
                            ani = DB.GetAnimalByAniAlternateNumber(row["LOM"].ToString());
                            if (ani.AniId <= 0)
                            {
                                ani = DB.GetAnimalByAniAlternateNumber(row["Levensnummer"].ToString());
                                if (ani.AniId <= 0)
                                {
                                    ani = DB.GetAnimalByLifenr(row["Levensnummer"].ToString());
                                    if (ani.AniId <= 0)
                                    {
                                        // dier is niet gevonden, data ophalen vanaf HITIER en agrobase vullen.
                                        String Lomnr = row["LOM"].ToString();

                                        ani = hiCL.fillAnimalFromHtierTbl(lToken, farm.UBNid, farm.ProgId, farm.Programid, Lomnr, "", ChangedBy, 0);
                                    }
                                }
                            }
                        }
                    }
                    ANIMALCATEGORY anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                    if (anicat.FarmId == 0 || anicat.AniId == 0)
                    {
                        anicat.AniId = ani.AniId;
                        anicat.FarmId = farm.FarmId;
                        anicat.UbnId = farm.UBNid;
                        anicat.Anicategory = 5;
                    }
                    else if (anicat.Anicategory == 0)
                    {
                        anicat.AniId = ani.AniId;
                        anicat.FarmId = farm.FarmId;
                        anicat.UbnId = farm.UBNid;
                        anicat.Anicategory = 5;
                    }
                    if (anicat.AniWorknumber == "")
                    {
                        string worknr = "";
                        string tempnr = "";
                        ANIMAL vader = new ANIMAL();
                        Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, ani.AniAlternateNumber, vader, 0, out  worknr, out tempnr);
                        anicat.AniWorknumber = worknr;
                    }
                    MOVEMENT mov;
                    EVENT eve;



                    switch (lMeldingtype)
                    {
                        case 3: // Geboorte  GEBURT
                            anicat.Anicategory = 1;
                            if (ani.ThrId <= 0)
                            {
                                ani.ThrId = farmUBN.ThrID;
                                DB.UpdateANIMAL(farmUBN.ThrID, ani);
                            }

                            if (ani.AniIdMother > 0)
                            {
                                ANIMAL mother = DB.GetAnimalById(ani.AniIdMother);
                                ANIMALCATEGORY acm = DB.GetAnimalCategoryByIdandFarmid(ani.AniIdMother, farm.FarmId);
                                if (acm.FarmId == 0)
                                {
                                    acm.FarmId = farm.FarmId;
                                    acm.AniId = ani.AniIdMother;
                                    acm.Anicategory = 1;
                                    DB.SaveAnimalCategory(acm);
                                }
                                if (mother.AniId > 0 && mother.AniSex == 0)
                                {
                                    mother.AniSex = 2;
                                    DB.SaveAnimal(farmUBN.ThrID, mother);
                                }
                                if (acm.AniWorknumber == "")
                                {
                                    string worknr = "";
                                    string tempnr = "";
                                    ANIMAL vader = new ANIMAL();

                                    Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, mother.AniAlternateNumber, vader, 0, out  worknr, out tempnr);
                                    acm.AniWorknumber = worknr;
                                    DB.SaveAnimalCategory(acm);
                                }
                                checkForMsOptimaboxRespondernumber(lToken, farm, mother, false);

                                eve = DB.GetEventByDateAniIdKind(MutDate, ani.AniIdMother, 5);
                                // 
                                if (eve.EventId != 0)
                                {
                                    BIRTH bir = DB.GetBirth(eve.EventId);
                                    if (bir.CalfId != ani.AniId) // is dier niet gelijk aan gemelde dier
                                    {
                                        eve = new EVENT();
                                    }
                                }


                                if (eve.EventId == 0) // is dit dier geboren op een ander UBN?
                                {

                                    List<EVENT> births = DB.getEventsByAniIdKind(ani.AniIdMother, 5);
                                    foreach (EVENT evebirth in births)
                                    {
                                        BIRTH bir = DB.GetBirth(evebirth.EventId);
                                        if (bir.CalfId == ani.AniId) // is dier gelijk aan gemelde dier
                                        {
                                            eve = evebirth;
                                            break;
                                        }
                                        else if (bir.CalfId == 0 && bir.BornDead == 0 && ani.AniBirthDate == evebirth.EveDate && births.Count == 1)
                                        // is de geboorte wel bekend maar het dier dat geboren is niet?
                                        {
                                            eve = evebirth;
                                            break;
                                        }

                                    }
                                }
                                if (eve.EventId == 0)
                                {
                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Geboorte : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));

                                    eve.Changed_By = ChangedBy;
                                    eve.SourceID = farmUBN.ThrID;
                                    eve.EveMutationBy = 15;
                                    eve.EveMutationDate = DateTime.Today;
                                    eve.EveMutationTime = DateTime.Now;
                                    eve.AniId = ani.AniIdMother;
                                    eve.EveDate = MutDate;
                                    Event_functions.handleEventTimes(ref eve, MutDate);
                                    eve.EveKind = 5;
                                    eve.UBNId = farm.UBNid;
                                    eve.happened_at_FarmID = anicat.FarmId;
                                    eve.EveOrder = Event_functions.getNewEventOrder(lToken, MutDate.Date, 5, farm.UBNid, ani.AniIdMother);

                                    DB.SaveEvent(eve);
                                    BIRTH bir = new BIRTH();
                                    bir.EventId = eve.EventId;
                                    bir.CalfId = ani.AniId;
                                    bir.BirNumber = Event_functions.getOrCreateBirnr(lToken, ani.AniIdMother, MutDate);
                                    bir.Meldnummer = meldingsnummer;
                                    DB.SaveBirth(bir);
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);

                                    checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                    
                                    //if (DB.isdeelnemerBedrijfZiekteNsfo(farm.UBNid, farm.Programid))
                                    //{
                                    //    //mov.MovMutationDate == DateTime.Today
                                    //    if (ani.AniIdMother > 0 && ani.AniIdFather > 0)
                                    //    {
                                    //        if (ani.AniScrapie == 0)
                                    //        {
                                    //            ANIMAL moeder = DB.GetAnimalById(ani.AniIdMother);
                                    //            ANIMAL vader = DB.GetAnimalById(ani.AniIdFather);
                                    //            ani.AniScrapie = Event_functions.BerekenScrapie(pToken, farm, moeder.AniScrapie, vader.AniScrapie, true);
                                    //            if (ani.AniScrapie > 0)
                                    //            {
                                    //                ani.AniDatumScrapie = ani.AniBirthDate;
                                    //                DB.UpdateANIMAL(farmUBN.ThrID, ani);
                                    //            }
                                    //        }
                                    //    }
                                    //    DateTime invoerdatum = MutDate;
                                    //    System.Collections.Hashtable hash = new System.Collections.Hashtable();

                                    //    bool tdb = false;
                                    //    if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString != string.Empty)
                                    //    {
                                    //        tdb = true;
                                    //    }
                                    //    unLogger.WriteDebug("IRMUT", "checkGeboorteBedrijfZiekteStatussen");
                                    //    //UBN ubnfrom = DB.getUBNByBedrijfsnummer(UBN2e);
                                    //    //bij geboorte is het t eigen UBN
                                    //    Event_functions.checkAanvoerBedrijfZiekteStatussenByAniId(202064, pToken, farm, farmUBN, Event_functions.BedrijfziekteCheck.Opslaan,
                                    //            Event_functions.BedrijfziekteReden.Geboorteinvoer, ani, invoerdatum, out hash,
                                    //            getConfigDoc("berichtenbestand.xml"), tdb);

                                    //}
                                }
                                
                                    
                                 
                                MovFunc.SetSaveAnimalCategory(lToken, DB.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                                FaAdviser.saveKetoBoxFeedAdvices(lToken, ani.AniIdMother, eve.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                                DB.WriteSoapError(new SOAPLOG
                                {
                                    Changed_By = ChangedBy,
                                    Date = DateTime.Now.Date,
                                    FarmNumber = farmUBN.Bedrijfsnummer,
                                    Kind = (int)LABELSConst.SOAPLOGKind.HITIER_Meldingen_Raadplegen,
                                    Lifenumber = ani.AniAlternateNumber,
                                    Omschrijving = $@"Birth Mother AniID:{ani.AniIdMother} {MutDate}",
                                    SourceID = Result.SourceID,
                                    TaskLogID = Result.TaskLogID,
                                    Status = "G",
                                    Time = DateTime.Now
                                });
                            }
                            else
                            {

                                unLogger.WriteInfo("IRMUT", "Ontbrekende Geboorte ZONDER MOEDERDIER! : ");
                                unLogger.WriteInfo("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteInfo("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteInfo("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));
                                MovFunc.SetSaveAnimalCategory(lToken, DB.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                                DB.WriteSoapError(new SOAPLOG
                                {
                                    Changed_By = ChangedBy,
                                    Date = DateTime.Now.Date,
                                    FarmNumber = farmUBN.Bedrijfsnummer,
                                    Kind = (int)LABELSConst.SOAPLOGKind.HITIER_Meldingen_Raadplegen,
                                    Lifenumber = ani.AniAlternateNumber,
                                    Omschrijving = $@"Birth No Mother Found {MutDate}",
                                    SourceID = Result.SourceID,
                                    TaskLogID = Result.TaskLogID,
                                    Status = "F",
                                    Time = DateTime.Now
                                });
                            }
                            break;
                        case 200000: // Import
                            //anicat.Anicategory = 1;
                            //mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                            //if (mov.MovId == 0)
                            //{
                            //    unLogger.WriteInfo("IRMUT", "Ontbrekende Import : ");
                            //    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                            //    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                            //    //unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                            //    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", farmUBN.Bedrijfsnummer));

                            //    mov.MovMutationBy = 15;
                            //    mov.MovMutationDate = DateTime.Today;
                            //    mov.MovMutationTime = DateTime.Now;
                            //    mov.AniId = ani.AniId;
                            //    mov.MovDate = MutDate;
                            //    MovFunc.HandleMovTimes(pToken, MutDate, "", ref mov);
                            //    mov.ReportDate = MutDate;
                            //    mov.MovKind = 1;
                            //    mov.MovOrder = MovFunc.getNewMovementOrder(pToken, 1, MutDate, ani.AniId, farm.UBNid);
                            //    mov.UbnId = farm.UBNid;
                            //    mov.Progid = farm.ProgId;
                            //    mov.happened_at_FarmID = farm.FarmId;
                            //    //if (UBN2e != String.Empty)
                            //    //{
                            //    //    UBN ubnfrom = DB.getUBNByBedrijfsnummer(UBN2e);
                            //    //    mov.MovThird_UBNid = ubnfrom.UBNid;
                            //    //    mov.ThrId = ubnfrom.ThrID;
                            //    //}
                            //    DB.SaveMovement(mov);
                            //    BUYING buy = new BUYING();
                            //    buy.PurKind = 1;
                            //    buy.MovId = mov.MovId;
                            //    DB.SaveBuying(buy);


                            //    //if (DB.isdeelnemerBedrijfZiekteNsfo(farm.UBNid, farm.Programid) && ani.AniSex == 1)
                            //    //{
                            //    //    DateTime invoerdatum = MutDate;
                            //    //    System.Collections.Hashtable hash = new System.Collections.Hashtable();

                            //    //    bool tdb = false;
                            //    //    if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString != string.Empty)
                            //    //    {
                            //    //        tdb = true;
                            //    //    }
                            //    //    unLogger.WriteDebug("IRMUT", "checkAanvoerBedrijfZiekteStatussen");
                            //    //    UBN ubnfrom = DB.getUBNByBedrijfsnummer(UBN2e);
                            //    //    Event_functions.checkAanvoerBedrijfZiekteStatussenByAniId(202064, pToken, farm, ubnfrom, Event_functions.BedrijfziekteCheck.Opslaan,
                            //    //        Event_functions.BedrijfziekteReden.Aanvoer, ani, invoerdatum, out hash,
                            //    //        getConfigDoc("berichtenbestand.xml"), tdb);
                            //    //}
                            //}
                            break;
                        case 1: // Aanvoer                                    

                            anicat.Anicategory = 1;

                            mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);

                            if (mov.MovId == 0 && IRAanvoer)
                            {
                                unLogger.WriteInfo("IRMUT", "Ontbrekende Aanvoer : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                //unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                                unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", farmUBN.Bedrijfsnummer));
                                String CountryCodeDepart = "";

                                mov.Changed_By = ChangedBy;
                                mov.SourceID = farmUBN.ThrID;
                                mov.MovMutationBy = 15;
                                mov.MovMutationDate = DateTime.Today;
                                mov.MovMutationTime = DateTime.Now;
                                mov.AniId = ani.AniId;
                                mov.MovDate = MutDate;
                                MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                mov.MovKind = 1;
                                mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                mov.UbnId = farm.UBNid;
                                mov.Progid = farm.ProgId;
                                mov.happened_at_FarmID = anicat.FarmId;

                                DB.SaveMovement(mov);
                                BUYING buy = new BUYING();
                                buy.MovId = mov.MovId;
                                DB.SaveBuying(buy);

                                mov.ReportDate = MutDate;
                                MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                //if (DB.isdeelnemerBedrijfZiekteNsfo(farm.UBNid, farm.Programid))
                                //{
                                //    //mov.MovMutationDate == DateTime.Today

                                //    DateTime invoerdatum = MutDate;
                                //    System.Collections.Hashtable hash = new System.Collections.Hashtable();

                                //    bool tdb = false;
                                //    if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString != string.Empty)
                                //    {
                                //        tdb = true;
                                //    }
                                //    unLogger.WriteDebug("IRMUT", "checkAanvoerBedrijfZiekteStatussen");
                                //    UBN ubnfrom = DB.getUBNByBedrijfsnummer(UBN2e);
                                //    Event_functions.checkAanvoerBedrijfZiekteStatussenByAniId(202064, pToken, farm, ubnfrom, Event_functions.BedrijfziekteCheck.Opslaan,
                                //            Event_functions.BedrijfziekteReden.Aanvoer, ani, invoerdatum, out hash,
                                //            getConfigDoc("berichtenbestand.xml"), tdb);

                                //}
                                DB.WriteSoapError(new SOAPLOG
                                {
                                    Changed_By = ChangedBy,
                                    Date = DateTime.Now.Date,
                                    FarmNumber = farmUBN.Bedrijfsnummer,
                                    Kind = (int)LABELSConst.SOAPLOGKind.HITIER_Meldingen_Raadplegen,
                                    Lifenumber = ani.AniAlternateNumber,
                                    Omschrijving = $@"New Buying {MutDate}",
                                    SourceID = Result.SourceID,
                                    TaskLogID = Result.TaskLogID,
                                    Status = "G",
                                    Time = DateTime.Now
                                });
                            }
                            break;
                        case 2: // Afvoer   ABGANG
                            anicat.Anicategory = 4;
                            mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                            if (mov.MovId == 0)
                            {
                                unLogger.WriteInfo("IRMUT", "Ontbrekende Afvoer : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", farmUBN.Bedrijfsnummer));

                                mov.Changed_By = ChangedBy;
                                mov.SourceID = farmUBN.ThrID;
                                mov.MovMutationBy = 15;
                                mov.MovMutationDate = DateTime.Today;
                                mov.MovMutationTime = DateTime.Now;
                                mov.AniId = ani.AniId;
                                mov.MovDate = MutDate;
                                MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                mov.MovKind = 2;
                                mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                mov.UbnId = farm.UBNid;
                                mov.Progid = farm.ProgId;
                                mov.happened_at_FarmID = anicat.FarmId;
                                mov.ReportDate = MutDate;
                                DB.SaveMovement(mov);
                                SALE sal = new SALE();
                                sal.MovId = mov.MovId;
                                DB.SaveSale(sal);
                                MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                DB.WriteSoapError(new SOAPLOG
                                {
                                    Changed_By = ChangedBy,
                                    Date = DateTime.Now.Date,
                                    FarmNumber = farmUBN.Bedrijfsnummer,
                                    Kind = (int)LABELSConst.SOAPLOGKind.HITIER_Meldingen_Raadplegen,
                                    Lifenumber = ani.AniAlternateNumber,
                                    Omschrijving = $@"New Sale {MutDate}",
                                    SourceID = Result.SourceID,
                                    TaskLogID = Result.TaskLogID,
                                    Status = "G",
                                    Time = DateTime.Now
                                });
                            }

                            break;
                        case 6: // Dood  // zit er nog niet in  SLACHT misschien

                            anicat.Anicategory = 4;
                            mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
                            if (mov.MovId == 0)
                            {
                                unLogger.WriteInfo("IRMUT", "Ontbrekende Doodmelding : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));

                                mov.Changed_By = ChangedBy;
                                mov.SourceID = farmUBN.ThrID;
                                mov.MovMutationBy = 15;
                                mov.MovMutationDate = DateTime.Today;
                                mov.MovMutationTime = DateTime.Now;
                                mov.AniId = ani.AniId;
                                mov.MovDate = MutDate;
                                MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                mov.ReportDate = MutDate;
                                mov.MovKind = 3;
                                mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 3, MutDate, ani.AniId, farm.UBNid);
                                mov.UbnId = farm.UBNid;
                                mov.Progid = farm.ProgId;
                                mov.happened_at_FarmID = anicat.FarmId;

                                DB.SaveMovement(mov);
                                LOSS loss = new LOSS();
                                loss.MovId = mov.MovId;
                                loss.Changed_By = ChangedBy;
                                loss.SourceID = farmUBN.ThrID;
                                DB.SaveLoss(loss);
                                MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                DB.WriteSoapError(new SOAPLOG
                                {
                                    Changed_By = ChangedBy,
                                    Date = DateTime.Now.Date,
                                    FarmNumber = farmUBN.Bedrijfsnummer,
                                    Kind = (int)LABELSConst.SOAPLOGKind.HITIER_Meldingen_Raadplegen,
                                    Lifenumber = ani.AniAlternateNumber,
                                    Omschrijving = $@"New Loss{MutDate}",
                                    SourceID = Result.SourceID,
                                    TaskLogID = Result.TaskLogID,
                                    Status = "G",
                                    Time = DateTime.Now
                                });
                            }

                            break;

                        default:
                            String Lifenr = row["Levensnummer"].ToString();
                            unLogger.WriteInfo(String.Format("Onbekende Melding van RVO : Datum {0}  Levensnr : {1} Meldingtype : {2}", MutDate, Lifenr, lMeldingtype));
                            DB.WriteSoapError(new SOAPLOG
                            {
                                Changed_By = ChangedBy,
                                Date = DateTime.Now.Date,
                                FarmNumber = farmUBN.Bedrijfsnummer,
                                Kind = (int)LABELSConst.SOAPLOGKind.HITIER_Meldingen_Raadplegen,
                                Lifenumber = ani.AniAlternateNumber,
                                Omschrijving = $@"Action not set:  Meldingtype:{ lMeldingtype} {MutDate}",
                                SourceID = Result.SourceID,
                                TaskLogID = Result.TaskLogID,
                                Status = "F",
                                Time = DateTime.Now
                            });
                            continue;

                    }



                }
                if (RequestUpdate != null)
                {

                    mvEvent.Progress = 100;
                    mvEvent.Message = " Klaar " + totaal.ToString() + " " + VSM_Ruma_OnlineCulture.getStaticResource("dieren", "dieren") + " " + VSM_Ruma_OnlineCulture.getStaticResource("verwerkt", "verwerkt");
                    RequestUpdate(this, mvEvent);
                }
            }
            DB.WriteSoapError(Result);
            return Result;
        }

        public SOAPLOG DoeSanitelMeldingenRaadplegen(UserRightsToken pTokenA, BEDRIJF farm, String lLevensnr, int MeldingSoort, DateTime Begindatum, DateTime Einddatum)
        {
            UserRightsToken lToken = (UserRightsToken)pTokenA.Clone();

            SOAPLOG Result = new SOAPLOG();
            Result.Kind = 1200;
            MovFuncEvent mvEvent = new MovFuncEvent();
            DB.DBMasterQueries DB = new DB.DBMasterQueries(pTokenA);
            DBSelectQueries DBSelect = Facade.GetInstance().getSlave(pTokenA);
            UBN farmUBN = DB.GetubnById(farm.UBNid);
            THIRD farmThird = DB.GetThirdByThirId(farmUBN.ThrID);
            Feedadviceer FaAdviser = new Feedadviceer();
            FARMCONFIG fcon = DB.getFarmConfig(farm.FarmId, "rfid", "1");
            if (String.IsNullOrEmpty(lLevensnr))
            { lLevensnr = ""; }
            unLogger.WriteInfo("DoeSanitelMeldingenRaadplegen: UBN:"+ farmUBN.Bedrijfsnummer + " UbnID:" + farm.UBNid.ToString() + " Programid:"+  farm.Programid.ToString() +
                                                          lLevensnr + " MeldingSoort±" + MeldingSoort.ToString() + " begindatum:" + Begindatum.ToString()+ " Einddatum:" +  Einddatum.ToString());
            String Output = unLogger.getLogDir("CSVData") + farm.Programid +"_"+ farmUBN.Bedrijfsnummer + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

            Result = Facade.GetInstance().getMeldingen().STRaadplegenMeldingenAlg(lToken, farm.UBNid, farm.ProgId, farm.Programid,
                                                          lLevensnr, MeldingSoort, Begindatum, Einddatum, Output);
            if (Result.Kind == 0)
            {
                Result.Kind = 1200;
            }
            Result.TaskLogID = TasklogID > 0 ? TasklogID : Result.TaskLogID;
            DB.WriteSoapError(Result); 
            string[] Kols = { "Levensnummer", "soortmelding", "meldingsnummer", "Datum", "melddatum" };
            char spl = ';';
            DataTable tblSanitel = utils.GetCsvData(Kols, Output, spl, "Mutaties");
            try
            {
                tblSanitel.DefaultView.Sort = "melddatum,meldingsnummer";//Chronologisch volgens RVO
                tblSanitel = tblSanitel.DefaultView.ToTable(true);
            }
            catch { }//gaat fout als tblLNV.rows.count =0;

         
            bool IRAanvoer = Convert.ToBoolean(DB.GetConfigValue(farm.Programid, farm.FarmId, "IRAanvoerAutoRecieve", "True"));
            FARMCONFIG fcIR = DB.getFarmConfig(farm.FarmId, "IRaanvoer", "True");
            //Result.FarmNumber = farmUBN.Bedrijfsnummer;
            //Result.Omschrijving = "Ophalen Mutaties nog niet mogelijk";
            //Result.Status = "F";
            //return Result;
            int[] aStatusMeldingen = { 0, 1, 2, 4, 5 };

            if (tblSanitel.Rows.Count == 0)
            {
                if (RequestUpdate != null)
                {

                    mvEvent.Progress = 100;
                    if (Result.Code == "IRD-00364")
                    {
                        mvEvent.Message = " Klaar Er zijn geen dieren verwerkt, want zijn meer dan 2500 meldingen gevonden, probeer een kortere periode.";
                        RequestUpdate(this, mvEvent);
                    }
                    else
                    {
                        if (Result.Omschrijving != "")
                        {
                            mvEvent.Message = " Klaar Er zijn geen dieren verwerkt. " + Result.Omschrijving;
                            RequestUpdate(this, mvEvent);
                        }
                        else
                        {
                            mvEvent.Message = " Klaar Er zijn geen meldingen gevonden voor deze periode. ";
                            RequestUpdate(this, mvEvent);
                        }
                    }
                }
            }
            else
            {
                int teller = 0;
                int totaal = tblSanitel.Rows.Count;

                //"soortmelding", "meldingsnummer", "Datum", "melddatum"




                foreach (DataRow row in tblSanitel.Rows)
                {
                    int lSoortMelding = Convert.ToInt32(row["soortmelding"]);
                    string meldingsnummer = row["meldingsnummer"].ToString();
                    DateTime MutDate = utils.getDateLNV(row["Datum"].ToString());
                    //DateTime melddatum = utils.getDateLNV(row["melddatum"].ToString());
                    String Lifenr = row["Levensnummer"].ToString();
                    ANIMAL ani = DB.GetAnimalByAniAlternateNumber(Lifenr);

                    int procent = teller * 100 / totaal;

                    if (RequestUpdate != null)
                    {

                        mvEvent.Progress = procent;
                        mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("verwerken", "Verwerken") + " " + Lifenr;
                        RequestUpdate(this, mvEvent);
                    }
                    teller += 1;


                    if (ani.AniId <= 0) // bestaat nog niet in agrobase
                    {
                        //lock (padlock) // 2014-05-01 als 2 administraties hetzelfde dier bevatten komt hij dubbel in agrobase
                        //{
                        // binnen de lock nogmaals checken, misschien is hij inmiddels door een andere thread aangemaakt.
                        ani = DB.GetAnimalByAniAlternateNumber(Lifenr);
                        if (ani.AniId <= 0)
                        {
                            // dier is niet gevonden, data ophalen vanaf Sanitrace en agrobase vullen.

                            VulDiergegevensvanuitSanitrace(lToken, farm, ref ani, Lifenr, 1);
                        }
                        //}
                    }
                    if (ani.AniId > 0)
                    {
                        ANIMALCATEGORY anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        if (anicat.FarmId == 0 || anicat.AniId == 0)
                        {
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            anicat.Anicategory = 5;
                            anicat.UbnId = farm.UBNid;
                            DB.SaveAnimalCategory(anicat);
                            anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        }
                        else if (anicat.Anicategory == 0)
                        {
                            anicat.AniId = ani.AniId;
                            anicat.FarmId = farm.FarmId;
                            anicat.Anicategory = 5;
                            anicat.UbnId = farm.UBNid;
                            DB.SaveAnimalCategory(anicat);
                            anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        }

                        if (anicat.AniWorknumber == "")
                        {
                            string worknr = "";
                            string tempnr = "";
                            ANIMAL vader = new ANIMAL();
                            Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, ani.AniAlternateNumber, vader, 0, out  worknr, out tempnr);
                            anicat.AniWorknumber = worknr;
                            DB.SaveAnimalCategory(anicat);
                            anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                        }
                        MOVEMENT mov;
                        EVENT eve;

                        if ((lSoortMelding == 2 || lSoortMelding == 3) && farm.Programid == 16)
                        {
                            string Query = @"SELECT ac.*  FROM 
                                                        agrobase_sheep.ANIMALCATEGORY  ac
                                                        JOIN agrobase_sheep.ANIMAL a ON a.AniID=ac.AniID
                                                        JOIN agrofactuur.BEDRIJF b ON b.FarmId=ac.FarmID
                                                        where a.AniAlternateNumber='" + ani.AniAlternateNumber + "' AND b.Programid IN (16,160)";
                            DataTable tblBeltes = DBSelect.QueryData(Query);
                            if (tblBeltes.Rows.Count == 0)
                            {
                                unLogger.WriteDebug("IRMUT", "Onbrekende Aanvoer : ");
                                unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));
                                unLogger.WriteError("IRMUT", " Het dier kan niet aangevoerd worden want het is niet bekend in het TES stamboek, De houder dient het afstammingsbewijs naar het secretariaat te sturen voordat de diergegevens verwerkt kunnen worden");
                                MovFunc.onbekendstamboekdierBeltesMessage(pTokenA, farmUBN.ThrID, ChangedBy, farmUBN.Bedrijfsnummer, ani.AniAlternateNumber);
                               
                                continue;
                            }
                        }
                        switch (lSoortMelding)
                        {
                            case 1: // Geboorte
                                anicat.Anicategory = 1;
                                if (ani.ThrId <= 0)
                                {
                                    ani.ThrId = farmUBN.ThrID;
                                    ani.Changed_By = ChangedBy;
                                    ani.SourceID = farmUBN.ThrID;
                                    DB.UpdateANIMAL(farmUBN.ThrID, ani);
                                }
                                if (ani.AniIdMother <= 0)
                                {
                                    VulDiergegevensvanuitSanitrace(lToken, farm, ref ani, Lifenr, 1);
                                }
                                if (ani.AniIdMother > 0)
                                {
                                    ANIMAL mother = DB.GetAnimalById(ani.AniIdMother);
                                    ANIMALCATEGORY acm = DB.GetAnimalCategoryByIdandFarmid(ani.AniIdMother, farm.FarmId);
                                    if (acm.FarmId == 0)
                                    {
                                        acm.FarmId = farm.FarmId;
                                        acm.AniId = ani.AniIdMother;
                                        acm.Anicategory = 1;
                                        acm.Changed_By = ChangedBy;
                                        acm.SourceID = farmUBN.ThrID;
                                        DB.SaveAnimalCategory(acm);
                                    }
                                    if (acm.AniWorknumber == "")
                                    {
                                        string worknr = "";
                                        string tempnr = "";
                                        ANIMAL vader = new ANIMAL();

                                        Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, mother.AniAlternateNumber, vader, 0, out  worknr, out tempnr);
                                        acm.AniWorknumber = worknr;
                                        acm.Changed_By = ChangedBy;
                                        acm.SourceID = farmUBN.ThrID;
                                        DB.SaveAnimalCategory(acm);
                                    }
                                    checkForMsOptimaboxRespondernumber(lToken, farm, mother, false);

                                    eve = DB.GetEventByDateAniIdKind(MutDate, ani.AniIdMother, 5);
                                    //Luc 2-7-2013 orginele import geboortes heeft ubnid op 0 staan
                                    if (eve.EventId != 0)
                                    {
                                        BIRTH bir = DB.GetBirth(eve.EventId);
                                        if (bir.CalfId != ani.AniId) // is dier niet gelijk aan gemelde dier
                                        {
                                            eve = new EVENT();
                                        }
                                    }


                                    if (eve.EventId == 0) // is dit dier geboren op een ander UBN?
                                    {

                                        List<EVENT> births = DB.getEventsByAniIdKind( ani.AniIdMother, 5);
                                        foreach (EVENT evebirth in births)
                                        {
                                            BIRTH bir = DB.GetBirth(evebirth.EventId);
                                            if (bir.CalfId == ani.AniId) // is dier gelijk aan gemelde dier
                                            {
                                                eve = evebirth;
                                                break;
                                            }
                                            else if (bir.CalfId == 0 && bir.BornDead == 0 && ani.AniBirthDate == evebirth.EveDate && births.Count == 1)
                                            // is de geboorte wel bekend maar het dier dat geboren is niet?
                                            {
                                                eve = evebirth;
                                                break;
                                            }

                                        }
                                    }
                                    if (eve.EventId == 0)
                                    {
                                        unLogger.WriteInfo("IRMUT", "Ontbrekende Geboorte : ");
                                        unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                        unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                        unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));

                                        eve.Changed_By = ChangedBy;
                                        eve.SourceID = farmUBN.ThrID;
                                        eve.EveMutationBy = 15;
                                        eve.EveMutationDate = DateTime.Today;
                                        eve.EveMutationTime = DateTime.Now;
                                        eve.AniId = ani.AniIdMother;
                                        eve.EveDate = MutDate;
                                        Event_functions.handleEventTimes(ref eve, MutDate);
                                        eve.EveKind = 5;
                                        eve.UBNId = farm.UBNid;
                                        eve.happened_at_FarmID = anicat.FarmId;
                                        eve.EveOrder = Event_functions.getNewEventOrder(lToken, MutDate.Date, 5, farm.UBNid, ani.AniIdMother);

                                        DB.SaveEvent(eve);
                                        BIRTH bir = new BIRTH();
                                        bir.EventId = eve.EventId;
                                        bir.CalfId = ani.AniId;
                                        bir.BirNumber = Event_functions.getOrCreateBirnr(lToken, ani.AniIdMother, MutDate);
                                        bir.Meldnummer = meldingsnummer;
                                        bir.Changed_By = ChangedBy;
                                        bir.SourceID = farmUBN.ThrID;
                                        DB.SaveBirth(bir);
                                        
                                        MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                        checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);

                                    }
                                    FaAdviser.saveKetoBoxFeedAdvices(lToken, ani.AniIdMother, eve.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"Born Mother AniID:{ani.AniIdMother} {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "G",
                                        Time = DateTime.Now
                                    });
                                }
                                else
                                {

                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Geboorte ZONDER MOEDERDIER! : ");
                                    unLogger.WriteInfo("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteInfo("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteInfo("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));
                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"Born NO Mother found!! {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "F",
                                        Time = DateTime.Now
                                    });
                                }
                                break;
                            case 2: // Import
                                anicat.Anicategory = 1;
                                mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);
                                if (mov.MovId == 0)
                                {
                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Import : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    //unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", farmUBN.Bedrijfsnummer));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.ReportDate = MutDate;
                                    mov.MovKind = 1;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 1, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = farm.FarmId;
                                    //if (UBN2e != String.Empty)
                                    //{
                                    //    UBN ubnfrom = DB.getUBNByBedrijfsnummer(UBN2e);
                                    //    mov.MovThird_UBNid = ubnfrom.UBNid;
                                    //    mov.ThrId = ubnfrom.ThrID;
                                    //}
                                    DB.SaveMovement(mov);
                                    BUYING buy = new BUYING();
                                    buy.PurKind = 1;
                                    buy.MovId = mov.MovId;
                                    buy.Changed_By = ChangedBy;
                                    buy.SourceID = farmUBN.ThrID;
                                    DB.SaveBuying(buy);
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);

                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"New Import {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "G",
                                        Time = DateTime.Now
                                    });
                                }
                                checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                break;
                            case 3: // Aanvoer                                      

                                anicat.Anicategory = 1;

                                mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);

                                if (mov.MovId == 0 && IRAanvoer)
                                {
                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Aanvoer : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    //unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", farmUBN.Bedrijfsnummer));
                                    String CountryCodeDepart = "";

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.MovKind = 1;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    //if (UBN2e != String.Empty)
                                    //{
                                    //    UBN ubnfrom = DB.getUBNByBedrijfsnummer(UBN2e);
                                    //    mov.MovThird_UBNid = ubnfrom.UBNid;
                                    //    mov.ThrId = ubnfrom.ThrID;
                                    //}
                                    DB.SaveMovement(mov);
                                    BUYING buy = new BUYING();
                                    buy.MovId = mov.MovId;
                                    buy.Changed_By = ChangedBy;
                                    buy.SourceID = farmUBN.ThrID;
                                    DB.SaveBuying(buy);
                                    //if (MeldingStatus == 2 && UBNnr2ePartijd != String.Empty)
                                    //{

                                    //    MovFunc.saveAanVoerMutation(pToken, farm, fcIR, mov, buy, CountryCodeDepart, ani.AniCountryCodeBirth);
                                    //}
                                    //else 
                                    mov.ReportDate = MutDate;
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"New Buying {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "G",
                                        Time = DateTime.Now
                                    });
                                }
                                checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
                                break;
                            case 4: // Afvoer
                            case 5: // Afvoer naar slachthuis
                                anicat.Anicategory = 4;
                                mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                                if (mov.MovId == 0)
                                {
                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Afvoer : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", farmUBN.Bedrijfsnummer));
                                    //unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", UBN2e));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.MovKind = 2;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    //if (UBN2e != String.Empty)
                                    //{
                                    //    UBN ubnto = DB.getUBNByBedrijfsnummer(UBN2e);
                                    //    mov.MovThird_UBNid = ubnto.UBNid;
                                    //    mov.ThrId = ubnto.ThrID;
                                    //}
                                    DB.SaveMovement(mov);
                                    SALE sal = new SALE();
                                    sal.MovId = mov.MovId;
                                    sal.Changed_By = ChangedBy;
                                    sal.SourceID = farmUBN.ThrID;
                                    if (lSoortMelding == 5)
                                    {
                                        sal.SalKind = 2;
                                    }
                                    DB.SaveSale(sal);
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                    MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                    //if (MeldingStatus == 2 && UBNnr2ePartijd != String.Empty)
                                    //{
                                    //    MovFunc.saveAfvoerMutation(pToken, farm, ani, mov, sal);
                                    //}
                                    //else 
                                    mov.ReportDate = MutDate;
                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"New Sale {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "G",
                                        Time = DateTime.Now
                                    });
                                }
                                break;
                            case 6: // Dood

                                anicat.Anicategory = 4;
                                mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
                                if (mov.MovId == 0)
                                {
                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Doodmelding : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.ReportDate = MutDate;
                                    mov.MovKind = 3;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 3, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    //if (UBN2e != String.Empty)
                                    //{
                                    //    UBN ubnto = DB.getUBNByBedrijfsnummer(UBN2e);
                                    //    mov.MovThird_UBNid = ubnto.UBNid;
                                    //    mov.ThrId = ubnto.ThrID;
                                    //}
                                    DB.SaveMovement(mov);
                                    LOSS loss = new LOSS();
                                    loss.MovId = mov.MovId;
                                    loss.Changed_By = ChangedBy;
                                    loss.SourceID = farmUBN.ThrID;
                                    DB.SaveLoss(loss);
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                    MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"New Loss {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "G",
                                        Time = DateTime.Now
                                    });
                                }
                                break;
                      
                        
                           

                            case 7: // Export
                                anicat.Anicategory = 4;
                                mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
                                if (mov.MovId == 0)
                                {
                                    unLogger.WriteInfo("IRMUT", "Ontbrekende Export : ");
                                    unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
                                    unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
                                    unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", farmUBN.Bedrijfsnummer));
                                    //unLogger.WriteDebug("IRMUT", String.Format("UBNTo: {0}", UBN2e));

                                    mov.Changed_By = ChangedBy;
                                    mov.SourceID = farmUBN.ThrID;
                                    mov.MovMutationBy = 15;
                                    mov.MovMutationDate = DateTime.Today;
                                    mov.MovMutationTime = DateTime.Now;
                                    mov.AniId = ani.AniId;
                                    mov.MovDate = MutDate;
                                    MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
                                    mov.ReportDate = MutDate;
                                    mov.MovKind = 2;
                                    mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
                                    mov.UbnId = farm.UBNid;
                                    mov.Progid = farm.ProgId;
                                    mov.happened_at_FarmID = anicat.FarmId;
                                    //if (UBN2e != String.Empty)
                                    //{
                                    //    UBN ubnto = DB.getUBNByBedrijfsnummer(UBN2e);
                                    //    mov.MovThird_UBNid = ubnto.UBNid;
                                    //    mov.ThrId = ubnto.ThrID;
                                    //}
                                    DB.SaveMovement(mov);
                                    SALE sal = new SALE();
                                    sal.MovId = mov.MovId;
                                    sal.SalKind = 1;
                                    sal.Changed_By = ChangedBy;
                                    sal.SourceID = farmUBN.ThrID;
                                    DB.SaveSale(sal);
                                    MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                    MovFunc.returnTransmitters(lToken, farm, mov.AniId);
                                    DB.WriteSoapError(new SOAPLOG
                                    {
                                        Changed_By = ChangedBy,
                                        Date = DateTime.Now.Date,
                                        FarmNumber = farmUBN.Bedrijfsnummer,
                                        Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                        Lifenumber = ani.AniAlternateNumber,
                                        Omschrijving = $@"New Export {MutDate}",
                                        SourceID = Result.SourceID,
                                        TaskLogID = Result.TaskLogID,
                                        Status = "G",
                                        Time = DateTime.Now
                                    });
                                }
                                break;

                            default:

                                unLogger.WriteInfo(String.Format("Onbekende Melding van Sanitrace : Datum {0}  Levensnr : {1} MeldSoort : {2}", MutDate, Lifenr, lSoortMelding));
                                DB.WriteSoapError(new SOAPLOG
                                {
                                    Changed_By = ChangedBy,
                                    Date = DateTime.Now.Date,
                                    FarmNumber = farmUBN.Bedrijfsnummer,
                                    Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen,
                                    Lifenumber = ani.AniAlternateNumber,
                                    Omschrijving = $@"Type not found lSoortMelding:{lSoortMelding} {MutDate}",
                                    SourceID = Result.SourceID,
                                    TaskLogID = Result.TaskLogID,
                                    Status = "W",
                                    Time = DateTime.Now
                                });
                                continue;

                        }
                        MovFunc.SetSaveAnimalCategory(lToken, DB.GetBedrijfById(anicat.FarmId), anicat, ChangedBy, farmUBN.ThrID);
                        //if (MovFunc.getLastMovementDate(pToken, anicat.AniId, farm) == MutDate)
                        //{
                        //    MovFunc.setAnimalCategory(pToken, farm, anicat);
                        //    DB.SaveAnimalCategory(anicat);      
                        //}
                    }
                }
                if (RequestUpdate != null)
                {

                    mvEvent.Progress = 100;
                    mvEvent.Message = " Klaar " + totaal.ToString() + " " + VSM_Ruma_OnlineCulture.getStaticResource("dieren", "dieren") + " " + VSM_Ruma_OnlineCulture.getStaticResource("verwerkt", "verwerkt");
                    RequestUpdate(this, mvEvent);
                }
            }
            DB.WriteSoapError(Result);
            return Result;
        }

        public void VulDiergegevensvanuitSanitrace(UserRightsToken pToken, BEDRIJF farm, ref ANIMAL pAnimal, string pLifenr, int pGetMother)
        {
            if (!String.IsNullOrEmpty(pLifenr))
            {
                DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
                SOAPLOG sl = new SOAPLOG();
                sl.Changed_By = ChangedBy;
                sl.Code = "";
                sl.Date = DateTime.Now.Date;
                sl.FarmNumber = "farmid:"+farm.FarmId.ToString();
                sl.Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen;
                sl.Lifenumber = pLifenr;
             
                sl.Time = DateTime.Now;
                sl.Status = "G";

                try
                {
                    //Win32SANITRACEALG DLLcall = new Win32SANITRACEALG();
                    String Taal = utils.getcurrentlanguage();
             
                    String lStatus = string.Empty;
                    String lOmschrijving = string.Empty;
                    String LogDir = unLogger.getLogDir("IenR");
                    
                    Directory.CreateDirectory(LogDir);
                    

                    
                    int MaxString = 255;
                    String lUsername = ""; String lPassword = "";
                    FARMCONFIG fcon = DB.getFarmConfig(farm.FarmId, "rfid", "1");
                    UBN lUBN = DB.GetubnById(farm.UBNid);
                    THIRD lPersoon = DB.GetThirdByThirId(lUBN.ThrID);
                    String Beslagnr;
                    String PENnr;
                    String Inrichtingsnr;
                    String Annexnr;
                    utils.getSanitraceNummers(lPersoon, lUBN, out Annexnr, out Inrichtingsnr, out Beslagnr, out PENnr);
                    PENnr = "";
                    /*
                        Voor Belgie moet je PEN dus leeg laten.
                        Met vriendelijke groet,
                        Nico de Groot
                        V.S.M. Automatisering
                        ----- Original Message -----
                        From: Jos Wijdeven (VSM Automatisering bv)
                        To: Nico de Groot (V.S.M. Automatisering)
                        Sent: Friday, November 06, 2015 4:39 PM
                        Subject: Re: Fw: test optima Belgie

                        Nico,
                        Een gedeeld beslag is als 2 veehouder een beslag delen. Dus zelfde beslagnr meer verschillend PEN nr. Zal niet veel voorkomen maar is welk mogelijk (volgens mij is dit bij tenminste 1 vilatca mester).
                        Zonder PEN nr komen er wel dieren terug, zie bijlage.
                     */
                    FTPUSER fusoap = DB.GetFtpuser(farm.UBNid, 9994);
                    lUsername = fusoap.UserName;
                    lPassword = fusoap.Password;
                    if (pAnimal.AniId > 0)
                    {
                        pAnimal = DB.GetAnimalById(pAnimal.AniId);
                    }
                    if (pAnimal.AniId == 0)
                    {
                        pAnimal = DB.GetAnimalByAniAlternateNumber(pLifenr);
                        if (pAnimal.AniId == 0)
                        {
                            pAnimal = DB.GetAnimalByLifenr(pLifenr);
                        }
                    }
                    int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                    if (lTestServer > 0)
                    {
                        lTestServer = 0;

                        /*
                            Inloggegevens voor de testserver:
                            Gebruikersnaam: JOHVLT  of  TESTXML (ook echte gebruikersnamen zijn te gebuiken op de testserver)
                            Wachtwoord: (nog) n.v.t.

                         */
                        //lUsername = "JOHVLT";
                        //lPassword = "";
                        //PENnr = "1304804186";
                        //Beslagnr = "BE10192704-0151";

                        //lUsername = "GUYLAU";
                        //PENnr = "1300193757";
                        //Beslagnr = "10061200-0151";

                    }
                    if (lPersoon.ThrCountry == "125")
                    {
                        lTestServer = 10;
                    }
                    string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
                    String LogFile = LogDir + $@"{farm.Programid}_{lUBN.Bedrijfsnummer}_ST_Dierdetails_" + pLifenr.Replace(" ", "") + "_" + tijd + ".log";
                    String LocalOutputFile = LogDir + $@"{farm.Programid}_{lUBN.Bedrijfsnummer}_ST_Dierdetails" + pLifenr.Replace(" ", "") + "_" + tijd + ".csv";

                    sl.FarmNumber = lUBN.Bedrijfsnummer;
                    sl.Kind = (int)LABELSConst.SOAPLOGKind.Sanitel_Meldingen_Raadplegen;
                    sl.Lifenumber = pLifenr;
                    sl.Omschrijving = LocalOutputFile;
                    sl.SourceID = lPersoon.ThrId;

                    unLogger.WriteInfo($@"{nameof(VulDiergegevensvanuitSanitrace)} lifenr:{pLifenr} ");
                    SOAPSANITEL.SanitelMeldingen mld = new SOAPSANITEL.SanitelMeldingen();
                    mld.STDierdetails(lUsername, lPassword, lTestServer, Taal, farm.ProgId, new List<string> { pLifenr }, LocalOutputFile, LogFile, ref lStatus, ref lOmschrijving);

                    //DLLcall.STDierdetails(lUsername, lPassword, lTestServer, Taal, pLifenr, LocalOutputFile, LogFile, ref lStatus, ref lOmschrijving, MaxString);
                    /*
                        pDierFile is een csv bestand met per regel:
                        levensnr ; naam ; geb.dat ; geslacht ; haarkleur ; rastype ; versienr paspoort ; levensnr moeder ; inrichtingsnr fokker
                        Rastype
                            1 = Melk
                            2 = Vlees
                            3 = Gemend
                     */
                    sl.Status = lStatus;
                    sl.Omschrijving = lOmschrijving;
                    DB.WriteSoapError(sl);
                    if (File.Exists(LocalOutputFile))
                    {
                        //BE 613235041;  ;20160111;M;WR;1;0;BE 512920151;BE40043045
                        string[] kols = { "levensnr", "naam", "gebdat", "geslacht", "haarkleur", "rastype", "versienrpaspoort", "levensnrmoeder", "inrichtingsnrfokker" };
                        DataTable tbl = utils.GetCsvData(kols, LocalOutputFile, ';', "Animal");
                        if (tbl.Rows.Count > 0)
                        {
                            if (pAnimal.AniAlternateNumber.Trim() == "")
                            {
                                pAnimal.AniAlternateNumber = tbl.Rows[0]["levensnr"].ToString();
                            }
                            if (pAnimal.AniLifeNumber == "")
                            {
                                pAnimal.AniLifeNumber = tbl.Rows[0]["levensnr"].ToString();
                            }
                            if (pAnimal.AniName == "")
                            {
                                if (tbl.Rows[0]["naam"] != DBNull.Value && tbl.Rows[0]["naam"].ToString().Trim() != "")
                                {
                                    Regex regExLifenr = new Regex(@"^[A-Z]{2}\s[\D\d-]{1,12}$");
                                    if (regExLifenr.Match(tbl.Rows[0]["naam"].ToString().Trim()).Success)
                                    {
                                        unLogger.WriteInfo(" VulDiergegevensvanuitSanitrace AniName=" + tbl.Rows[0]["naam"].ToString().Trim() + " voor dier " + pAnimal.AniAlternateNumber);
                                    }

                                    pAnimal.AniName = tbl.Rows[0]["naam"].ToString();

                                }
                            }
                            if (pAnimal.AniBirthDate == DateTime.MinValue)
                            {
                                try
                                {
                                    DateTime bDate = utils.getDateLNV(tbl.Rows[0]["gebdat"].ToString());
                                    pAnimal.AniBirthDate = bDate;
                                }
                                catch { }
                            }
                            try
                            {
                                if (pAnimal.AniSex == 0)
                                {
                                    if (tbl.Rows[0]["geslacht"].ToString().ToUpper() == "V")
                                    {
                                        pAnimal.AniSex = 2;
                                    }
                                    else if (tbl.Rows[0]["geslacht"].ToString().ToUpper() == "M")
                                    {
                                        pAnimal.AniSex = 1;
                                    }
                                }
                            }
                            catch { }
                            pAnimal.AniHaircolor_Memo = tbl.Rows[0]["haarkleur"].ToString();
                            if (tbl.Rows[0]["inrichtingsnrfokker"].ToString() == Inrichtingsnr)
                            {
                                if (pAnimal.ThrId == 0)
                                {
                                    pAnimal.ThrId = lPersoon.ThrId;
                                }
                            }
                            int pAnimalAniId = DB.SaveObject(pAnimal, pToken.getLastChildConnection());
                            if (pAnimal.AniId == 0 && pAnimalAniId > 0)
                            {
                                pAnimal = DB.GetAnimalById(pAnimalAniId);
                            }
                            if (!String.IsNullOrEmpty(tbl.Rows[0]["levensnrmoeder"].ToString()))
                            {
                                if (pAnimal.AniIdMother <= 0)
                                {
                                    ANIMAL aniMother = DB.GetAnimalByAniAlternateNumber(tbl.Rows[0]["levensnrmoeder"].ToString());
                                    if (aniMother.AniId == 0)
                                    {
                                        aniMother = DB.GetAnimalByLifenr(tbl.Rows[0]["levensnrmoeder"].ToString());
                                        if (aniMother.AniId == 0 && pGetMother == 1)
                                        {
                                            VulDiergegevensvanuitSanitrace(pToken, farm, ref aniMother, tbl.Rows[0]["levensnrmoeder"].ToString(), 0);
                                        }
                                    }
                                    pAnimal.AniIdMother = aniMother.AniId;
                                    DB.SaveObject(pAnimal, pToken.getLastChildConnection());
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    sl.Status = "F";
                    sl.Omschrijving = exc.Message;
                    unLogger.WriteError(exc.ToString());
                    DB.WriteSoapError(sl);
                }
            }
        }

        private ANIMAL GetDCFAnimalByAniAlternateNumber(UserRightsToken pToken, String pAniAlternateNumber)
        {
            
            DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
            ANIMAL lAnimal = new ANIMAL();
            //Ik krijg van dcf alleen nummers en geen landcodes
            if (pAniAlternateNumber != String.Empty)
            {

                StringBuilder lQuery = new StringBuilder();
                lQuery.Append(" SELECT ANIMAL.* FROM ANIMAL ");
                lQuery.AppendFormat(" WHERE (SUBSTRING(ANIMAL.AniAlternateNumber,4) LIKE '{0}' OR ANIMAL.AniAlternateNumber='{0}') ", pAniAlternateNumber);
                lQuery.Append(" AND ANIMAL.AniId>0 ");
                lQuery.Append(" UNION ");
                lQuery.Append(" SELECT ANIMAL.* FROM ANIMAL ");
                lQuery.Append(" JOIN LEVNRMUT ON LEVNRMUT.Aniid=ANIMAL.AniId ");
                lQuery.AppendFormat(" WHERE ( SUBSTRING(LEVNRMUT.LevnrOud,4) LIKE '{0}' OR LEVNRMUT.LevnrOud='{0}')  AND LEVNRMUT.Aniid >0 ", pAniAlternateNumber);
                DataSet ds = new DataSet();
                DataTable tblanimal = DB.GetDataBase().QueryData(pToken.getLastChildConnection(), ds, lQuery, "animal", MissingSchemaAction.Add);
                if (tblanimal.Rows.Count > 0)
                {
                    DB.GetDataBase().FillObject(lAnimal, tblanimal.Rows[0]);
                }
                else { unLogger.WriteInfo("MutationUpdater.GetAnimalByAniAlternateNumber No Animal found: pAniAlternateNumber=" + pAniAlternateNumber); }

            }
            else
            {
                unLogger.WriteError("GetAnimalByAniAlternateNumber zonder levensnummer aangeroepen!");
            }
            return lAnimal;
        }

 //       public SOAPLOG DoeDCFMeldingenRaadplegen(UserRightsToken pTokenA, BEDRIJF farm, String lLevensnr, int MeldingSoort, DateTime Begindatum, DateTime Einddatum)
 //       {
 //           UserRightsToken lToken = (UserRightsToken)pTokenA.Clone();
 //           //AFSavetoDB DB = Facade.GetInstance().getSaveToDB(lToken);
 //           DB.DBMasterQueries DB = new DB.DBMasterQueries(pTokenA);
 //           SOAPLOG Result = new SOAPLOG();
 //           Result.Code = "DCFMeldingenRaadplegen";
 //           Result.Changed_By = ChangedBy;
 //           Result.Date = DateTime.Now;
 //           Result.Kind = 1300;
          
 //           string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
 //           FTPUSER fusoap = DB.GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9998);
 //           FARMCONFIG fcon = DB.getFarmConfig(farm.FarmId, "rfid", "1");
 //           UBN farmUBN = DB.GetubnById(farm.UBNid);
 //           Result.FarmNumber = farmUBN.Bedrijfsnummer;
 //           int dcfAniTypeCode = CORE.utils.getDcfAniTypeCode(farm.ProgId);

 //           MovFuncEvent mvEvent = new MovFuncEvent();

 //           if (dcfAniTypeCode == 0)
 //           {
               
             
 //               Result.Status = "F";
 //               Result.Omschrijving = "Kan ikke bestemme Animal Kind.Unable to determine the AnimalKind";
 //               mvEvent.Progress = 0;
 //               mvEvent.Message = "Klaar " + Result.Omschrijving;
 //               if (RequestUpdate != null)
 //                   RequestUpdate(this, mvEvent);
 //               return Result;

 //           }
 //           DirectoryInfo dir = new DirectoryInfo(unLogger.getLogDir() + @"\IenR\");
 //           if (!dir.Exists)
 //           {
 //               dir.Create();
 //           }
 //           dir = new DirectoryInfo(unLogger.getLogDir() + @"\CSVData\");
 //           if (!dir.Exists)
 //           {
 //               dir.Create();
 //           }
 //           Feedadviceer FaAdviser = new Feedadviceer();
 //           unLogger.WriteInfo("DoeDCFMeldingenRaadplegen");
 //           String OutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_" + tijd + ".csv";
 //           String MilkOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_Milk_" + tijd + ".csv";
 //           String InseminationsOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_Insemin_" + tijd + ".csv";
 //           String SamenweidenOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_Samenweiden_" + tijd + ".csv";
 //           String DryPeriodsOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_DryPeriods_" + tijd + ".csv";
 //           String PregnancyCheckOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_PregnancyCheck_" + tijd + ".csv";
 //           String ClinicalRecordingsOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_ClinicalRecordings_" + tijd + ".csv";
 //           String WeightsOutputCsv = unLogger.getLogDir("CSVData") + farm.Programid + "_" + farmUBN.Bedrijfsnummer + "_Weights_" + tijd + ".csv";

 //           //int pTestserver = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);//TODO 
        
 ////           SoapDCF.AnimalService soapservice = new SoapDCF.AnimalService(0, unLogger.getLogDir() + @"\IenR\");


 //           if (fusoap.UserName == "" || fusoap.Password == "")
 //           {
          
 //               Result.Date = DateTime.Now;
   
 //               Result.Status = "F";
 //               Result.Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("userpwordnot", "Uw DCF inlog gegevens zijn niet compleet");
 //               mvEvent.Progress = 0;
 //               mvEvent.Message = "Klaar " + Result.Omschrijving;
 //               if (RequestUpdate != null)
 //                   RequestUpdate(this, mvEvent);
 //               Result.Kind = 1300;
 //               Result.TaskLogID = TasklogID > 0 ? TasklogID : Result.TaskLogID;//alleen overschrijven als TasklogID > 0
 //               DB.WriteSoapError(Result);
 //               return Result;
 //           }

 //           mvEvent.Progress = 0;
 //           mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("gegevens", "Gegevens") + " " + VSM_Ruma_OnlineCulture.getStaticResource("ophalen", "ophalen");
 //           if (RequestUpdate != null)
 //               RequestUpdate(this, mvEvent);
 //           bool GetDataDCF = true;
 //           if (ConfigurationManager.AppSettings["GetDataDCF"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["GetDataDCF"]))
 //           {
 //               if (!bool.TryParse(ConfigurationManager.AppSettings["GetDataDCF"], out GetDataDCF))
 //               {
 //                   GetDataDCF = true;
 //               }
 //           }
 //           if (GetDataDCF)
 //           {
 //               Result = soapservice.FindAnimalActivitiesByCHRNumber(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, OutputCsv);
 //           }
 //           Result.Kind = 1300;
 //           Result.TaskLogID = TasklogID > 0 ? TasklogID : Result.TaskLogID;//alleen overschrijven als TasklogID > 0
 //           DB.WriteSoapError(Result);
 //           string[] SaveExtraDataDCF = { "Milk", "Inseminations", "NaturalServices", "DryPeriods", "PregnancyChecks", "ClinicalRecordings", "Weights" }; 
 //           if (ConfigurationManager.AppSettings["SaveExtraDataDCF"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["SaveExtraDataDCF"]))
 //           {
 //               char[] sp = { ';' };
 //               SaveExtraDataDCF = ConfigurationManager.AppSettings["SaveExtraDataDCF"].Split(sp);

 //           }
 //           SOAPLOG MilkResult = new SOAPLOG();
 //           SOAPLOG InsResult = new SOAPLOG();
 //           SOAPLOG SamResult = new SOAPLOG();
 //           SOAPLOG DryoffResult = new SOAPLOG();
 //           SOAPLOG PregnancyResult = new SOAPLOG();
 //           SOAPLOG ClinicalRecordings = new SOAPLOG();
 //           SOAPLOG AnimalWeights = new SOAPLOG();
 //           if (SaveExtraDataDCF.Contains("Milk"))
 //           {
 //               MilkResult = soapservice.FindMilk(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, MilkOutputCsv);
 //               DB.WriteSoapError(MilkResult);
 //           }
 //           if (SaveExtraDataDCF.Contains("Inseminations"))
 //           {
 //               InsResult = soapservice.FindInseminations(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, InseminationsOutputCsv);
 //               DB.WriteSoapError(InsResult);
 //           }
 //           if (SaveExtraDataDCF.Contains("NaturalServices"))
 //           {
 //               SamResult = soapservice.FindNaturalServicesByHerdNumber(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, SamenweidenOutputCsv);
 //               DB.WriteSoapError(SamResult);
 //           }
 //           if (SaveExtraDataDCF.Contains("DryPeriods"))
 //           {
 //               DryoffResult = soapservice.FindDryPeriodsByHerdNumber(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, DryPeriodsOutputCsv);
 //               DB.WriteSoapError(DryoffResult);
 //           }
 //           if (SaveExtraDataDCF.Contains("PregnancyChecks"))
 //           {
 //               PregnancyResult = soapservice.FindPregnancyChecksByHerdNumber(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, PregnancyCheckOutputCsv);
 //               DB.WriteSoapError(PregnancyResult);
 //           }
 //           if (SaveExtraDataDCF.Contains("ClinicalRecordings"))
 //           {
 //               ClinicalRecordings = soapservice.FindClinicalRecordingsByHerdNumber(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, ClinicalRecordingsOutputCsv);
 //               DB.WriteSoapError(ClinicalRecordings);
 //           }
 //           if (SaveExtraDataDCF.Contains("Weights"))
 //           {
 //               AnimalWeights = soapservice.FindAnimalWeightsByHerdNumber(fusoap.UserName, fusoap.Password, farmUBN.Bedrijfsnummer, dcfAniTypeCode, Begindatum, Einddatum, WeightsOutputCsv);//, WeightsOutputCsv);
 //               DB.WriteSoapError(AnimalWeights);
 //           }

 //           if (Result.Status == "F")
 //           {
 //               mvEvent.Progress = 100;
 //               mvEvent.Message = Result.Omschrijving;
 //               if (RequestUpdate != null)
 //                   RequestUpdate(this, mvEvent);
 //               if (MilkResult.Status == "G" || InsResult.Status == "G" || SamResult.Status == "G" || DryoffResult.Status == "G" || PregnancyResult.Status == "G" || ClinicalRecordings.Status == "G")//|| AnimalWeights.Status=="G")
 //               {
 //                   //go ON
 //               }
 //               else
 //               {
 //                   return Result;
 //               }
 //           }
           
 //           //AnimalNumber + ";" + Meldingtype + ";" + gebeurtenisdatum.ToString("yyyyMMdd") + ";" + ";" + ";" + MeldingStatus + ";" + hersteld + ";" + motherAnimalNumber + ";"
 //           string[] Kols = { "Levensnummer", "Meldingtype", "Datum", "ubn2ePartij", "meldingsnummer", "meldingstatus", "hersteld", "motherAnimalNumber", "LactationNumber", "totalBornDead" };

 //           char spl = ';';
 //           DataTable tblDCFActivities = utils.GetCsvData(Kols, OutputCsv, spl, "Mutaties");
 //           try
 //           {
 //               tblDCFActivities.DefaultView.Sort = "Levensnummer,Datum";//Chronologisch volgens logica
 //               tblDCFActivities = tblDCFActivities.DefaultView.ToTable(true);
 //           }
 //           catch { }//gaat fout als tblLNV.rows.count =0;


 //           bool IRAanvoer = Convert.ToBoolean(DB.GetConfigValue(farm.Programid, farm.FarmId, "IRAanvoerAutoRecieve", "True"));
 //           FARMCONFIG fcIR = DB.getFarmConfig(farm.FarmId, "IRaanvoer", "True");

 //           int[] aStatusMeldingen = { 0, 1, 2, 4, 5 };
 //           int totaal = tblDCFActivities.Rows.Count;
 //           int teller = 0;
 //           if (tblDCFActivities.Rows.Count == 0)
 //           {
               
 //               Result.Date = DateTime.Now;
   
 //               Result.Status = "F";
 //               Result.Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("geenresultaat", "Geen resultaat gevonden");


 //               mvEvent.Progress = 100;
 //               mvEvent.Message = Result.Omschrijving;
 //               if (RequestUpdate != null)
 //                   RequestUpdate(this, mvEvent);

 //           }
 //           else
 //           {
 //               Regex r = new Regex(@"^\d{5,15}$");//GetDataDCF

 //               if (GetDataDCF)
 //               {
 //                   foreach (DataRow row in tblDCFActivities.Rows)
 //                   {
 //                       teller += 1;
 //                       int procent = teller * 100 / totaal;


 //                       int lMeldingtype = Convert.ToInt32(row["Meldingtype"]);
 //                       string meldingsnummer = row["meldingsnummer"].ToString();//is vooralsnog leeg want ik kan die nog nergens uithalen
 //                       DateTime MutDate = utils.getDateLNV(row["Datum"].ToString());
 //                       if (row["Levensnummer"] != DBNull.Value && row["Levensnummer"].ToString() != "")
 //                       {
 //                           string levensnummer = row["Levensnummer"].ToString();
 //                           if (r.Match(row["Levensnummer"].ToString()).Success)
 //                           {
 //                               levensnummer = "DK 0" + levensnummer;
 //                           }
 //                           if (RequestUpdate != null)
 //                           {

 //                               mvEvent.Progress = procent;
 //                               mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("verwerken", "Verwerken") + " " + levensnummer;
 //                               RequestUpdate(this, mvEvent);
 //                           }
 //                           ANIMAL ani = getAnimalDCF(lToken, farm, farmUBN, soapservice, fusoap.UserName, fusoap.Password, levensnummer, row["motherAnimalNumber"].ToString(), ChangedBy, farmUBN.ThrID);

 //                           if (ani.AniId > 0)
 //                           {


 //                               ANIMALCATEGORY anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
 //                               if (anicat.FarmId == 0 || anicat.AniId == 0)
 //                               {
 //                                   anicat.AniId = ani.AniId;
 //                                   anicat.FarmId = farm.FarmId;
 //                                   anicat.UbnId = farm.UBNid;
 //                                   anicat.Anicategory = 5;
 //                               }
 //                               else if (anicat.Anicategory == 0)
 //                               {
 //                                   anicat.AniId = ani.AniId;
 //                                   anicat.FarmId = farm.FarmId;
 //                                   anicat.UbnId = farm.UBNid;
 //                                   anicat.Anicategory = 5;
 //                               }
 //                               if (anicat.AniWorknumber == "")
 //                               {
 //                                   string worknr = "";
 //                                   string tempnr = "";
 //                                   ANIMAL vader = new ANIMAL();
 //                                   Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, ani.AniAlternateNumber, vader, 0, out worknr, out tempnr);
 //                                   anicat.AniWorknumber = worknr;
 //                               }

 //                               MOVEMENT mov;
 //                               EVENT eve;

 //                               switch (lMeldingtype)
 //                               {
 //                                   case 3: // Geboorte  GEBURT
 //                                       anicat.Anicategory = 1;
 //                                       if (ani.ThrId <= 0)
 //                                       {
 //                                           ani.ThrId = farmUBN.ThrID;
 //                                           DB.UpdateANIMAL(farmUBN.ThrID, ani);
 //                                       }

 //                                       if (ani.AniIdMother > 0)
 //                                       {
 //                                           ANIMAL mother = DB.GetAnimalById(ani.AniIdMother);
 //                                           ANIMALCATEGORY acm = DB.GetAnimalCategoryByIdandFarmid(ani.AniIdMother, farm.FarmId);
 //                                           if (acm.FarmId == 0)
 //                                           {
 //                                               acm.FarmId = farm.FarmId;
 //                                               acm.AniId = ani.AniIdMother;
 //                                               acm.Anicategory = 1;
 //                                               DB.SaveAnimalCategory(acm);
 //                                           }
 //                                           if (acm.AniWorknumber == "")
 //                                           {
 //                                               string worknr = "";
 //                                               string tempnr = "";
 //                                               ANIMAL vader = new ANIMAL();

 //                                               Event_functions.getRFIDNumbers(lToken, farm, fcon.FValue, mother.AniAlternateNumber, vader, 0, out worknr, out tempnr);
 //                                               acm.AniWorknumber = worknr;
 //                                               DB.SaveAnimalCategory(acm);
 //                                           }


 //                                           checkForMsOptimaboxRespondernumber(lToken, farm, mother, false);
 //                                           int Lactanienr = 0;
 //                                           try
 //                                           {
 //                                               if (row["LactationNumber"] != DBNull.Value)
 //                                               {
 //                                                   if (!String.IsNullOrEmpty(row["LactationNumber"].ToString()))
 //                                                   {
 //                                                       int.TryParse(row["LactationNumber"].ToString(), out Lactanienr);
 //                                                   }
 //                                               }
 //                                           }
 //                                           catch { }
 //                                           eve = new EVENT();
 //                                           BIRTH bir = new BIRTH();

 //                                           List<EVENT> births = DB.getEventsByAniIdKind(ani.AniIdMother, 5);
 //                                           foreach (EVENT evebirth in births)
 //                                           {
 //                                               BIRTH birC = DB.GetBirth(evebirth.EventId);
 //                                               if (birC.CalfId == ani.AniId) // is dier gelijk aan gemelde dier
 //                                               {
 //                                                   eve = evebirth;
 //                                                   bir = birC;

 //                                                   break;
 //                                               }
 //                                           }


 //                                           if (eve.EventId == 0)
 //                                           {
 //                                               unLogger.WriteInfo("IRMUT DCF", "Ontbrekende Geboorte : ");
 //                                               unLogger.WriteDebug("IRMUT DCF", String.Format("Datum : {0}", MutDate.ToString()));
 //                                               unLogger.WriteDebug("IRMUT DCF", String.Format("Dier : {0}", ani.AniAlternateNumber));
 //                                               unLogger.WriteDebug("IRMUT DCF", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));

 //                                               eve.Changed_By = ChangedBy;
 //                                               eve.SourceID = farmUBN.ThrID;
 //                                               eve.EveMutationBy = 15;
 //                                               eve.EveMutationDate = DateTime.Today;
 //                                               eve.EveMutationTime = DateTime.Now;
 //                                               eve.AniId = ani.AniIdMother;
 //                                               eve.EveDate = MutDate;
 //                                               Event_functions.handleEventTimes(ref eve, MutDate);
 //                                               eve.EveKind = 5;
 //                                               eve.UBNId = farm.UBNid;
 //                                               eve.happened_at_FarmID = anicat.FarmId;
 //                                               eve.EveOrder = Event_functions.getNewEventOrder(lToken, MutDate.Date, 5, farm.UBNid, ani.AniIdMother);

 //                                               DB.SaveEvent(eve);
 //                                               bir = new BIRTH();
 //                                               bir.EventId = eve.EventId;
 //                                               bir.CalfId = ani.AniId;
 //                                               if (Lactanienr > 0)
 //                                               {
 //                                                   bir.BirNumber = Lactanienr;
 //                                               }
 //                                               else
 //                                               {
 //                                                   bir.BirNumber = Event_functions.getOrCreateBirnr(lToken, ani.AniIdMother, MutDate);
 //                                               }
 //                                               bir.Meldnummer = meldingsnummer;
 //                                               DB.SaveBirth(bir);
 //                                               FaAdviser.saveKetoBoxFeedAdvices(lToken, ani.AniIdMother, eve.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);
 //                                               MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
                                              
 //                                           }
 //                                           else
 //                                           {

 //                                               if (eve.EveOrder == 0)
 //                                               {
 //                                                   List<EVENT> events = DB.getEventsByDateAniIdKind(ani.AniBirthDate, eve.AniId, eve.EveKind);
 //                                                   var ordered = (from n in events orderby n.EveOrder select n).ToList();
 //                                                   int Order = 1;
 //                                                   if (events.Count > 0)
 //                                                   {
 //                                                       foreach (EVENT ev in ordered)
 //                                                       {
 //                                                           if (ev.EveOrder != Order)
 //                                                           {
 //                                                               ev.EveOrder = Order;
 //                                                               if (DB.SaveEvent(ev))
 //                                                               {
 //                                                               }
 //                                                           }
 //                                                           Order += 1;
 //                                                       }
 //                                                   }

 //                                               }

 //                                               if (Lactanienr > 0)
 //                                               {
 //                                                   bir.BirNumber = Lactanienr;
 //                                                   DB.SaveBirth(bir);
 //                                               }

 //                                           }
 //                                           checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
 //                                           FaAdviser.saveKetoBoxFeedAdvices(lToken, ani.AniIdMother, eve.EveDate.Date, farm, ChangedBy, farmUBN.ThrID);

 //                                           DB.WriteSoapError(new SOAPLOG
 //                                           {
 //                                               Changed_By = ChangedBy,
 //                                               Date = DateTime.Now.Date,
 //                                               FarmNumber = farmUBN.Bedrijfsnummer,
 //                                               Kind = (int)LABELSConst.SOAPLOGKind.DCF_Meldingen_Raadplegen,
 //                                               Lifenumber = ani.AniAlternateNumber,
 //                                               Omschrijving = $@"Born Mother AniID:{ani.AniIdMother} {MutDate}",
 //                                               SourceID = Result.SourceID,
 //                                               TaskLogID = Result.TaskLogID,
 //                                               Status = "G",
 //                                               Time = DateTime.Now
 //                                           });
 //                                       }
 //                                       else
 //                                       {

 //                                           unLogger.WriteInfo("IRMUT", "Ontbrekende Geboorte ZONDER MOEDERDIER! : ");
 //                                           unLogger.WriteInfo("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
 //                                           unLogger.WriteInfo("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
 //                                           unLogger.WriteInfo("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));
 //                                           DB.WriteSoapError(new SOAPLOG
 //                                           {
 //                                               Changed_By = ChangedBy,
 //                                               Date = DateTime.Now.Date,
 //                                               FarmNumber = farmUBN.Bedrijfsnummer,
 //                                               Kind = (int)LABELSConst.SOAPLOGKind.DCF_Meldingen_Raadplegen,
 //                                               Lifenumber = ani.AniAlternateNumber,
 //                                               Omschrijving = $@"Birth NO Mother found {MutDate}",
 //                                               SourceID = Result.SourceID,
 //                                               TaskLogID = Result.TaskLogID,
 //                                               Status = "F",
 //                                               Time = DateTime.Now
 //                                           });
 //                                       }
 //                                       break;
 //                                   case 4: // Doodgeboren


 //                                       //Dit is geen doodmelding maar een Doodgeboren melding 
 //                                       //Dus er moet een doodgeboren calf aan dit dier gekoppeld worden.
 //                                       unLogger.WriteDebug("IRMUT", "Onbrekende Doodgeboren : ");
 //                                       unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
 //                                       unLogger.WriteDebug("IRMUT", String.Format("UBN : {0}", farmUBN.Bedrijfsnummer));

 //                                       StringBuilder bld = new StringBuilder();
 //                                       bld.Append(" SELECT EVENT.* FROM EVENT LEFT JOIN BIRTH ");
 //                                       bld.Append(" ON BIRTH.EventId = EVENT.EventId ");
 //                                       bld.AppendFormat(" WHERE EVENT.AniId={0} AND BIRTH.BornDead=1", ani.AniId);
 //                                       bld.Append(" AND EveKind = 5 AND EVENT.EventId>0 ");
 //                                       bld.Append(" AND date_format(EveDate,'%Y-%m-%d') = '" + MutDate.ToString("yyyy-MM-dd") + "'");
 //                                       bld.AppendFormat(" AND  UBNId = {0} ", farm.UBNid);
 //                                       DataTable tbl = DB.GetDataBase().QueryData(lToken.getLastChildConnection(), new DataSet(), bld, "Event", MissingSchemaAction.Add);
 //                                       int totalBornDead = int.Parse(row["totalBornDead"].ToString());
 //                                       checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
 //                                       if (totalBornDead > tbl.Rows.Count)
 //                                       {
 //                                           EVENT doodgeboren = new EVENT();
 //                                           //try
 //                                           //{
 //                                           //    if (tbl.Rows.Count > 0)
 //                                           //    {
 //                                           //        DB.GetDataBase().FillObject(doodgeboren, tbl.Rows[0]);
 //                                           //    }
 //                                           //}
 //                                           //catch { }
 //                                           //if (doodgeboren.EventId == 0)
 //                                           //{
 //                                           BIRTH newBirth = new BIRTH();
 //                                           Event_functions.handleEventTimes(ref doodgeboren, MutDate);
 //                                           doodgeboren.EveKind = 5;
 //                                           doodgeboren.AniId = ani.AniId;
 //                                           doodgeboren.Changed_By = ChangedBy;
 //                                           doodgeboren.SourceID = farmUBN.ThrID;
 //                                           doodgeboren.EveMutationBy = 15;
 //                                           doodgeboren.EveOrder = Event_functions.getNewEventOrder(lToken, MutDate.Date, 5, farm.UBNid, ani.AniId);
 //                                           doodgeboren.happened_at_FarmID = anicat.FarmId;
 //                                           doodgeboren.ThirdId = farmUBN.ThrID;
 //                                           doodgeboren.UBNId = farm.UBNid;
 //                                           newBirth.BornDead = 1;
 //                                           newBirth.BirNumber = Event_functions.getOrCreateBirnr(lToken, ani.AniId, MutDate.Date);
 //                                           if (row["meldingsnummer"] != DBNull.Value)
 //                                           {
 //                                               newBirth.Meldnummer = row["meldingsnummer"].ToString();
 //                                           }
 //                                           List<int> lNlingEventsIds = DB.getNlingCheckEventIds(farm.UBNid, ani.AniId, newBirth.BirNumber);
 //                                           newBirth.Nling = lNlingEventsIds.Count() + 1;
 //                                           if (DB.SaveEvent(doodgeboren))
 //                                           {
 //                                               newBirth.EventId = doodgeboren.EventId;
 //                                               DB.SaveBirth(newBirth);
 //                                               writeLog("Doodgeboren EventId:" + doodgeboren.EventId.ToString());
 //                                           }
 //                                           //}

 //                                       }
 //                                       FaAdviser.saveKetoBoxFeedAdvices(lToken, ani.AniId, MutDate.Date, farm, ChangedBy, farmUBN.ThrID);

 //                                       DB.WriteSoapError(new SOAPLOG
 //                                       {
 //                                           Changed_By = ChangedBy,
 //                                           Date = DateTime.Now.Date,
 //                                           FarmNumber = farmUBN.Bedrijfsnummer,
 //                                           Kind = (int)LABELSConst.SOAPLOGKind.DCF_Meldingen_Raadplegen,
 //                                           Lifenumber = ani.AniAlternateNumber,
 //                                           Omschrijving = $@"BornDead  {MutDate}",
 //                                           SourceID = Result.SourceID,
 //                                           TaskLogID = Result.TaskLogID,
 //                                           Status = "G",
 //                                           Time = DateTime.Now
 //                                       });

 //                                       break;
 //                                   case 200000: // Import





 //                                       break;
 //                                   case 1: // Aanvoer                                      

 //                                       anicat.Anicategory = 1;

 //                                       mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 1, farm.UBNid);

 //                                       if (mov.MovId == 0 && IRAanvoer)
 //                                       {
 //                                           unLogger.WriteInfo("IRMUT DCF", "Ontbrekende Aanvoer : ");
 //                                           unLogger.WriteDebug("IRMUT DCF", String.Format("Datum : {0}", MutDate.ToString()));
 //                                           unLogger.WriteDebug("IRMUT DCF", String.Format("Dier : {0}", ani.AniAlternateNumber));
 //                                           //unLogger.WriteDebug("IRMUT", String.Format("UBNFrom: {0}", UBN2e));
 //                                           unLogger.WriteDebug("IRMUT DCF", String.Format("UBNTo: {0}", farmUBN.Bedrijfsnummer));
 //                                           String CountryCodeDepart = "";

 //                                           mov.Changed_By = ChangedBy;
 //                                           mov.SourceID = farmUBN.ThrID;
 //                                           mov.MovMutationBy = 15;
 //                                           mov.MovMutationDate = DateTime.Today;
 //                                           mov.MovMutationTime = DateTime.Now;
 //                                           mov.AniId = ani.AniId;
 //                                           mov.MovDate = MutDate;
 //                                           MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
 //                                           mov.MovKind = 1;
 //                                           mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 1, MutDate, ani.AniId, farm.UBNid);
 //                                           mov.UbnId = farm.UBNid;
 //                                           mov.Progid = farm.ProgId;
 //                                           mov.happened_at_FarmID = anicat.FarmId;

 //                                           DB.SaveMovement(mov);
 //                                           BUYING buy = new BUYING();
 //                                           buy.MovId = mov.MovId;
 //                                           DB.SaveBuying(buy);

 //                                           mov.ReportDate = MutDate;

 //                                           MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);

 //                                           DB.WriteSoapError(new SOAPLOG
 //                                           {
 //                                               Changed_By = ChangedBy,
 //                                               Date = DateTime.Now.Date,
 //                                               FarmNumber = farmUBN.Bedrijfsnummer,
 //                                               Kind = (int)LABELSConst.SOAPLOGKind.DCF_Meldingen_Raadplegen,
 //                                               Lifenumber = ani.AniAlternateNumber,
 //                                               Omschrijving = $@"New Buying {MutDate}",
 //                                               SourceID = Result.SourceID,
 //                                               TaskLogID = Result.TaskLogID,
 //                                               Status = "G",
 //                                               Time = DateTime.Now
 //                                           });
 //                                       }
 //                                       checkForMsOptimaboxRespondernumber(lToken, farm, ani, false);
 //                                       break;
 //                                   case 2: // Afvoer   
 //                                       anicat.Anicategory = 4;
 //                                       mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 2, farm.UBNid);
 //                                       if (mov.MovId == 0)
 //                                       {
 //                                           unLogger.WriteInfo("IRMUT DCF", "Ontbrekende Afvoer : ");
 //                                           unLogger.WriteDebug("IRMUT DCF", String.Format("Datum : {0}", MutDate.ToString()));
 //                                           unLogger.WriteDebug("IRMUT DCF", String.Format("Dier : {0}", ani.AniAlternateNumber));
 //                                           unLogger.WriteDebug("IRMUT DCF", String.Format("UBNFrom: {0}", farmUBN.Bedrijfsnummer));

 //                                           mov.Changed_By = ChangedBy;
 //                                           mov.SourceID = farmUBN.ThrID;
 //                                           mov.MovMutationBy = 15;
 //                                           mov.MovMutationDate = DateTime.Today;
 //                                           mov.MovMutationTime = DateTime.Now;
 //                                           mov.AniId = ani.AniId;
 //                                           mov.MovDate = MutDate;
 //                                           MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
 //                                           mov.MovKind = 2;
 //                                           mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 2, MutDate, ani.AniId, farm.UBNid);
 //                                           mov.UbnId = farm.UBNid;
 //                                           mov.Progid = farm.ProgId;
 //                                           mov.happened_at_FarmID = anicat.FarmId;
 //                                           mov.ReportDate = MutDate;
 //                                           DB.SaveMovement(mov);
 //                                           SALE sal = new SALE();
 //                                           sal.MovId = mov.MovId;
 //                                           DB.SaveSale(sal);
 //                                           MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
 //                                           MovFunc.returnTransmitters(lToken, farm, mov.AniId);
 //                                           DB.WriteSoapError(new SOAPLOG
 //                                           {
 //                                               Changed_By = ChangedBy,
 //                                               Date = DateTime.Now.Date,
 //                                               FarmNumber = farmUBN.Bedrijfsnummer,
 //                                               Kind = (int)LABELSConst.SOAPLOGKind.DCF_Meldingen_Raadplegen,
 //                                               Lifenumber = ani.AniAlternateNumber,
 //                                               Omschrijving = $@"New Sale {MutDate}",
 //                                               SourceID = Result.SourceID,
 //                                               TaskLogID = Result.TaskLogID,
 //                                               Status = "G",
 //                                               Time = DateTime.Now
 //                                           });
 //                                       }
 //                                       break;
 //                                   case 6: // Dood  // zit er nog niet in  

 //                                       anicat.Anicategory = 4;
 //                                       mov = DB.GetMovementByDateAniIdKind(MutDate, ani.AniId, 3, farm.UBNid);
 //                                       if (mov.MovId == 0)
 //                                       {
 //                                           unLogger.WriteInfo("IRMUT", "Ontbrekende Doodmelding : ");
 //                                           unLogger.WriteDebug("IRMUT", String.Format("Datum : {0}", MutDate.ToString()));
 //                                           unLogger.WriteDebug("IRMUT", String.Format("Dier : {0}", ani.AniAlternateNumber));
 //                                           unLogger.WriteDebug("IRMUT", String.Format("UBN: {0}", farmUBN.Bedrijfsnummer));

 //                                           mov.Changed_By = ChangedBy;
 //                                           mov.SourceID = farmUBN.ThrID;
 //                                           mov.MovMutationBy = 15;
 //                                           mov.MovMutationDate = DateTime.Today;
 //                                           mov.MovMutationTime = DateTime.Now;
 //                                           mov.AniId = ani.AniId;
 //                                           mov.MovDate = MutDate;
 //                                           MovFunc.HandleMovTimes(lToken, MutDate, "", ref mov);
 //                                           mov.ReportDate = MutDate;
 //                                           mov.MovKind = 3;
 //                                           mov.MovOrder = MovFunc.getNewMovementOrder(lToken, 3, MutDate, ani.AniId, farm.UBNid);
 //                                           mov.UbnId = farm.UBNid;
 //                                           mov.Progid = farm.ProgId;
 //                                           mov.happened_at_FarmID = anicat.FarmId;

 //                                           DB.SaveMovement(mov);
 //                                           LOSS loss = new LOSS();
 //                                           loss.MovId = mov.MovId;
 //                                           DB.SaveLoss(loss);
 //                                           MovFunc.SetSaveAnimalCategory(lToken, farm, anicat, ChangedBy, farmUBN.ThrID);
 //                                           MovFunc.returnTransmitters(lToken, farm, mov.AniId);
 //                                           DB.WriteSoapError(new SOAPLOG
 //                                           {
 //                                               Changed_By = ChangedBy,
 //                                               Date = DateTime.Now.Date,
 //                                               FarmNumber = farmUBN.Bedrijfsnummer,
 //                                               Kind = (int)LABELSConst.SOAPLOGKind.DCF_Meldingen_Raadplegen,
 //                                               Lifenumber = ani.AniAlternateNumber,
 //                                               Omschrijving = $@"New Loss {MutDate}",
 //                                               SourceID = Result.SourceID,
 //                                               TaskLogID = Result.TaskLogID,
 //                                               Status = "G",
 //                                               Time = DateTime.Now
 //                                           });
 //                                       }
 //                                       break;





 //                                   default:
 //                                       String Lifenr = row["Levensnummer"].ToString();
 //                                       unLogger.WriteInfo(String.Format("Onbekende Melding van  DCF : Datum {0}  Levensnr : {1} Meldingtype : {2}", MutDate, Lifenr, lMeldingtype));
 //                                       continue;

 //                               }
 //                               //MovFunc.SetSaveAnimalCategory(lToken, DB.GetBedrijfById(anicat.FarmId), anicat);

 //                           }
 //                           else
 //                           {
 //                               unLogger.WriteInfo("Chr: " + farmUBN.Bedrijfsnummer + "_DCF_ANIMAL : " + row["Levensnummer"].ToString() + " Niet gevonden !! ");
 //                           }

 //                       }
 //                   }
 //               }
 //           }
 //           if (Result.Status == "F")
 //           {
 //               if (MilkResult.Status == "G" || InsResult.Status == "G" || SamResult.Status == "G" || DryoffResult.Status == "G" || PregnancyResult.Status == "G" || ClinicalRecordings.Status == "G")// || AnimalWeights.Status == "G")
 //               {
 //                   saveExtraDataDCF(lToken, farm, farmUBN, soapservice, fusoap.UserName, fusoap.Password, MilkOutputCsv, InseminationsOutputCsv, SamenweidenOutputCsv, DryPeriodsOutputCsv, PregnancyCheckOutputCsv, ClinicalRecordingsOutputCsv, WeightsOutputCsv);
 //                   if (RequestUpdate != null)
 //                   {
 //                       mvEvent.Progress = 100;
 //                       mvEvent.Message = " Klaar " + VSM_Ruma_OnlineCulture.getStaticResource("ophalenvoltooid", " Ophalen voltooid:");
 //                       RequestUpdate(this, mvEvent);
 //                   }
 //                   DB.WriteSoapError(Result);
 //                   return Result;
 //               }
 //               else
 //               {
 //                   if (RequestUpdate != null)
 //                   {
 //                       mvEvent.Progress = 100;
 //                       mvEvent.Message = " Klaar " + Result.Omschrijving;
 //                       RequestUpdate(this, mvEvent);
 //                   }
 //                   DB.WriteSoapError(Result);
 //                   return Result;
 //               }
 //           }
 //           else
 //           {
 //               saveExtraDataDCF(lToken, farm, farmUBN, soapservice, fusoap.UserName, fusoap.Password, MilkOutputCsv, InseminationsOutputCsv, SamenweidenOutputCsv, DryPeriodsOutputCsv, PregnancyCheckOutputCsv, ClinicalRecordingsOutputCsv, WeightsOutputCsv);// WeightsOutputCsv);
 //               if (RequestUpdate != null)
 //               {

 //                   mvEvent.Progress = 100;
 //                   mvEvent.Message = " Klaar " + tblDCFActivities.Rows.Count.ToString() + " " + VSM_Ruma_OnlineCulture.getStaticResource("meldingen", "Meldingen") + " " + VSM_Ruma_OnlineCulture.getStaticResource("opgehaald", "opgehaald").ToLower();
 //                   RequestUpdate(this, mvEvent);
 //               }
 //               DB.WriteSoapError(Result);
 //               return Result;
 //           }
 //       }

        //private void saveExtraDataDCF(UserRightsToken pToken, BEDRIJF pBedrijf, UBN pUbn, SoapDCF.AnimalService soapservice, string fUserName, string fPassword, string pMilkOutput, string pInseminationsOutput, string pSamenweidenOutput, string pDryPeriodsOutput, string pPregnancyCheckCsv, string pClinicalRecordingsOutputCsv, string pAnimalWeigtscsv)
        //{
        //    if (pBedrijf.FarmId > 0 && pUbn.UBNid > 0)
        //    {
        //        DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
        //        FARMCONFIG fcon = DB.getFarmConfig(pBedrijf.FarmId, "rfid", "1");
        //        MovFuncEvent mvEvent = new MovFuncEvent();
        //        if (File.Exists(pMilkOutput))
        //        {
        //            string[] mKols = { "AnimalNumber", "HerdNumber", "AnaMilkDate", "AnaKgMilk", "AnaPercFat", "AnaPercProtein", "AnaLactose", "AnaUrea", "AnaIndKetose", "AnaMilkCellcount" };

        //            DataTable tblMilk = utils.GetCsvData(mKols, pMilkOutput, ';', "ANALYSE");
        //            int teller = 0;
        //            int totaal = tblMilk.Rows.Count;
        //            unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  Milkrecordings:{tblMilk.Rows.Count} ");
        //            if (tblMilk.Rows.Count > 0)
        //            {
                       

        //                foreach (DataRow rw in tblMilk.Rows)
        //                {
        //                    teller += 1;
        //                    int procent = teller * 100 / totaal;
        //                    ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, rw["AnimalNumber"].ToString(), "", ChangedBy, pUbn.ThrID);
        //                    if (pAnimal.AniId > 0)
        //                    {
        //                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);

        //                        if (ac.AniId > 0 && ac.Anicategory < 5)
        //                        {
        //                            if (ac.AniWorknumber == "")
        //                            {
        //                                string worknr = "";
        //                                string tempnr = "";
        //                                ANIMAL vader = new ANIMAL();
        //                                Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out worknr, out tempnr);
        //                                ac.AniWorknumber = worknr;
        //                                DB.SaveAnimalCategory(ac);
        //                            }
        //                            if (RequestUpdate != null)
        //                            {
        //                                mvEvent.Progress = procent;
        //                                mvEvent.Message = "MilkRecordings " + pAnimal.AniLifeNumber;
        //                                RequestUpdate(this, mvEvent);
        //                            }
        //                            ANALYSE an = new ANALYSE();

        //                            DB.GetDataBase().FillObject(an, rw);

        //                            ANALYSE check = DB.GetAnalyseByKey(pAnimal.AniId, an.AnaMilkDate);
        //                            if (check.AniId > 0)
        //                            {
        //                                //if (check.UbnId == 0)
        //                                //{
        //                                //    check.UbnId = pUbn.UBNid;
        //                                //}
        //                                //check.AnaKgMilk = an.AnaKgMilk;
        //                                //check.AnaPercFat = an.AnaPercFat;
        //                                //check.AnaPercProtein = an.AnaPercProtein;
        //                                //check.AnaLactose = an.AnaLactose;
        //                                //check.AnaUrea = an.AnaUrea;
        //                                //check.AnaIndKetose = an.AnaIndKetose;
        //                                //check.AnaMilkCellcount = an.AnaMilkCellcount;
        //                                //if (!DB.SaveAnalyse(check))
        //                                //{
        //                                //    unLogger.WriteError("Failed to UPDATE ANALYSE DCF " + pAnimal.AniLifeNumber);
        //                                //}
        //                            }
        //                            else
        //                            {
        //                                ANALYSE anaNew = new ANALYSE();
        //                                anaNew.AnaMilkDate = an.AnaMilkDate;
        //                                anaNew.AniId = pAnimal.AniId;
        //                                anaNew.UbnId = pUbn.UBNid;
        //                                anaNew.AnaKgMilk = an.AnaKgMilk;
        //                                anaNew.AnaPercFat = an.AnaPercFat;
        //                                anaNew.AnaPercProtein = an.AnaPercProtein;
        //                                anaNew.AnaLactose = an.AnaLactose;
        //                                anaNew.AnaUrea = an.AnaUrea;
        //                                anaNew.AnaIndKetose = an.AnaIndKetose;
        //                                anaNew.AnaMilkCellcount = an.AnaMilkCellcount;
        //                                unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : INSERT Analyse Dcf Datum:{an.AnaMilkDate} ");
        //                                if (!DB.SaveAnalyse(anaNew))
        //                                {
        //                                    unLogger.WriteError("Failed to INSERT ANALYSE DCF " + pAnimal.AniLifeNumber);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (File.Exists(pInseminationsOutput))
        //        {
        //            string[] mKols = { "AnimalNumber", "Date", "InseminationNumber", "Description", "BullHerdbookNumber", "BullName" };
        //            //AnimalNumber;Date;InseminationNumber;Description;BullHerdbookNumber;BullName
        //            DataTable tblIns = utils.GetCsvData(mKols, pInseminationsOutput, ';', "EVENT");
        //            int teller = 0;
        //            int totaal = tblIns.Rows.Count;
        //            unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  Inseminations:{tblIns.Rows.Count} ");
        //            if (tblIns.Rows.Count > 0)
        //            {
        //                int lEveKind = 2;
                       
        //                foreach (DataRow rw in tblIns.Rows)
        //                {
        //                    teller += 1;
        //                    int procent = teller * 100 / totaal;
        //                    ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, rw["AnimalNumber"].ToString(), "", ChangedBy, pUbn.ThrID);
        //                    if (pAnimal.AniId > 0)
        //                    {
        //                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
        //                        if (ac.AniId > 0 && ac.Anicategory < 5)
        //                        {
        //                            if (ac.AniWorknumber == "")
        //                            {
        //                                string worknr = "";
        //                                string tempnr = "";
        //                                ANIMAL vader = new ANIMAL();
        //                                Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out worknr, out tempnr);
        //                                ac.AniWorknumber = worknr;
        //                                DB.SaveAnimalCategory(ac);
        //                            }
        //                            if (RequestUpdate != null)
        //                            {
        //                                mvEvent.Progress = procent;
        //                                mvEvent.Message = "Inseminations " + pAnimal.AniLifeNumber;
        //                                RequestUpdate(this, mvEvent);
        //                            }

        //                            DateTime evdate = utils.getDateLNV(rw["Date"].ToString());// Event_functions.getDatumFormat(rw["Date"], "");
        //                            EVENT eve = DB.GetEventByDateAniIdKind(evdate, pAnimal.AniId, lEveKind);
        //                            if (eve.EventId == 0)
        //                            {
        //                                eve.EveDate = evdate;
        //                                Event_functions.handleEventTimes(ref eve, evdate);
        //                                eve.AniId = pAnimal.AniId;
        //                                eve.happened_at_FarmID = pBedrijf.FarmId;
        //                                eve.EveKind = lEveKind;
        //                                eve.Changed_By = ChangedBy;
        //                                eve.SourceID = pUbn.ThrID;
        //                                eve.EveMutationBy = 15;
        //                                eve.UBNId = pUbn.UBNid;

        //                                eve.EveOrder = Event_functions.getNewEventOrder(pToken, evdate, lEveKind, pUbn.UBNid, pAnimal.AniId);

        //                                eve.ThirdId = pUbn.ThrID;

        //                                if (rw["InseminationNumber"] != DBNull.Value)
        //                                {

        //                                }
        //                                if (rw["Description"] != DBNull.Value)
        //                                {
        //                                    eve.EveComment += rw["Description"].ToString();
        //                                }
        //                                if (DB.SaveEvent(eve))
        //                                {
        //                                    INSEMIN ins = new INSEMIN();
        //                                    ins.EventId = eve.EventId;
        //                                    ANIMAL father = getKIFather(pToken, pBedrijf, pUbn, rw["BullHerdbookNumber"].ToString(), rw["BullName"].ToString());
        //                                    ins.AniIdFather = father.AniId;
        //                                    DB.SaveInsemin(ins);
        //                                    unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : INSERT Isemin Dcf Datum:{eve.EveDate} ");

        //                                }
        //                                else { unLogger.WriteError("Error opslaan Insemination DCF " + pAnimal.AniLifeNumber); }


        //                            }
        //                            else
        //                            {
        //                                if (rw["Description"] != DBNull.Value && eve.EveComment == "")
        //                                {
        //                                    eve.EveComment = rw["Description"].ToString();
        //                                    DB.SaveEvent(eve);
        //                                }
        //                                INSEMIN ins = DB.GetInsemin(eve.EventId);
        //                                if (ins.AniIdFather == 0)
        //                                {
        //                                    ANIMAL father = getKIFather(pToken, pBedrijf, pUbn, rw["BullHerdbookNumber"].ToString(), rw["BullName"].ToString());
        //                                    if (father.AniId > 0)
        //                                    {
        //                                        ins.AniIdFather = father.AniId;
        //                                        DB.SaveInsemin(ins);
        //                                        unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : Update Isemin Dcf Datum:{eve.EveDate} father.AniId:{father.AniId}");

        //                                    }
        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (File.Exists(pSamenweidenOutput))
        //        {
        //            string[] mKols = { "AnimalNumber", "StartDate", "EndDate", "BullNumber" };
        //            int lEveKind = 12;
        //            DataTable tblSams = utils.GetCsvData(mKols, pSamenweidenOutput, ';', "EVENT");
        //            int teller = 0;
        //            int totaal = tblSams.Rows.Count;
        //            unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  Samenweiden:{tblSams.Rows.Count} ");
        //            if (tblSams.Rows.Count > 0)
        //            {
                       
        //                foreach (DataRow rw in tblSams.Rows)
        //                {
        //                    teller += 1;
        //                    int procent = teller * 100 / totaal;
        //                    ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, rw["AnimalNumber"].ToString(), "", ChangedBy, pUbn.ThrID);
        //                    if (pAnimal.AniId > 0)
        //                    {
        //                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
        //                        if (ac.AniId > 0 && ac.Anicategory < 5)
        //                        {
        //                            if (ac.AniWorknumber == "")
        //                            {
        //                                string worknr = "";
        //                                string tempnr = "";
        //                                ANIMAL vader = new ANIMAL();
        //                                Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out worknr, out tempnr);
        //                                ac.AniWorknumber = worknr;
        //                                DB.SaveAnimalCategory(ac);
        //                            }
        //                            ANIMAL pBull = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, rw["BullNumber"].ToString(), "", ChangedBy, pUbn.ThrID);

        //                            if (RequestUpdate != null)
        //                            {
        //                                mvEvent.Progress = procent;
        //                                mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("samenweiden", "NaturalServices") + " " + pAnimal.AniLifeNumber;
        //                                RequestUpdate(this, mvEvent);
        //                            }
        //                            DateTime evdate = utils.getDateLNV(rw["StartDate"].ToString());// Event_functions.getDatumFormat(rw["StartDate"], "");
        //                            EVENT eve = DB.GetEventByDateAniIdKind(evdate, pAnimal.AniId, lEveKind);
        //                            if (eve.EventId == 0)
        //                            {
        //                                eve.EveDate = evdate;
        //                                Event_functions.handleEventTimes(ref eve, evdate);
        //                                eve.AniId = pAnimal.AniId;
        //                                eve.happened_at_FarmID = pBedrijf.FarmId;
        //                                eve.EveKind = lEveKind;
        //                                eve.Changed_By = ChangedBy;
        //                                eve.SourceID = pUbn.ThrID;
        //                                eve.EveMutationBy = 15;
        //                                eve.UBNId = pUbn.UBNid;
        //                                eve.EveOrder = Event_functions.getNewEventOrder(pToken, evdate, lEveKind, pUbn.UBNid, pAnimal.AniId);

        //                                eve.ThirdId = pUbn.ThrID;

        //                                if (DB.SaveEvent(eve))
        //                                {
        //                                    GRZTOGTH gr = new GRZTOGTH();
        //                                    gr.AniIdFather = pBull.AniId;
        //                                    DateTime enddate = utils.getDateLNV(rw["EndDate"].ToString());
        //                                    gr.EndDate = enddate;
        //                                    gr.EventId = eve.EventId;
        //                                    DB.SaveGRZTOGTH(gr);
        //                                    unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : INSERT GRZTOGTH Dcf Datum:{eve.EveDate} ");
        //                                }
        //                            }
        //                            else
        //                            {

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (File.Exists(pDryPeriodsOutput))
        //        {
        //            string[] mKols = { "AnimalNumber", "Herdnumber", "Date" };
        //            int lEveKind = 4;
        //            DataTable tblIns = utils.GetCsvData(mKols, pDryPeriodsOutput, ';', "EVENT");
        //            int teller = 0;
        //            int totaal = tblIns.Rows.Count;
        //            unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  DryPeriods:{tblIns.Rows.Count} ");
        //            if (tblIns.Rows.Count > 0)
        //            {
                        
        //                foreach (DataRow rw in tblIns.Rows)
        //                {
        //                    teller += 1;
        //                    int procent = teller * 100 / totaal;
        //                    ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, rw["AnimalNumber"].ToString(), "", ChangedBy, pUbn.ThrID);
        //                    if (pAnimal.AniId > 0)
        //                    {
        //                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
        //                        if (ac.AniId > 0 && ac.Anicategory < 5)
        //                        {
        //                            if (ac.AniWorknumber == "")
        //                            {
        //                                string worknr = "";
        //                                string tempnr = "";
        //                                ANIMAL vader = new ANIMAL();
        //                                Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out worknr, out tempnr);
        //                                ac.AniWorknumber = worknr;
        //                                DB.SaveAnimalCategory(ac);
        //                            }
        //                        }
        //                        if (RequestUpdate != null)
        //                        {
        //                            mvEvent.Progress = procent;
        //                            mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("droogstaand", "DryPeriods") + " " + pAnimal.AniLifeNumber;
        //                            RequestUpdate(this, mvEvent);
        //                        }
        //                        DateTime evdate = utils.getDateLNV(rw["Date"].ToString());
        //                        EVENT eve = DB.GetEventByDateAniIdKind(evdate, pAnimal.AniId, lEveKind);
        //                        if (eve.EventId == 0)
        //                        {
        //                            eve.EveDate = evdate;
        //                            Event_functions.handleEventTimes(ref eve, evdate);
        //                            eve.AniId = pAnimal.AniId;
        //                            eve.happened_at_FarmID = pBedrijf.FarmId;
        //                            eve.EveKind = lEveKind;
        //                            eve.Changed_By = ChangedBy;
        //                            eve.SourceID = pUbn.ThrID;
        //                            eve.EveMutationBy = 15;
        //                            eve.UBNId = pUbn.UBNid;
        //                            eve.EveOrder = Event_functions.getNewEventOrder(pToken, evdate, lEveKind, pUbn.UBNid, pAnimal.AniId);

        //                            eve.ThirdId = pUbn.ThrID;

        //                            if (DB.SaveEvent(eve))
        //                            {
        //                                DRYOFF dry = new DRYOFF();
        //                                dry.EventId = eve.EventId;

        //                                DB.SaveDryoff(dry);
        //                                unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : INSERT DRYOFF Dcf Datum:{eve.EveDate}  ");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (File.Exists(pPregnancyCheckCsv))
        //        {
        //            string[] mKols = { "AnimalNumber", "Herdnumber", "Date", "IsPregnant", "Description" };
        //            int lEveKind = 3;
        //            DataTable tblIns = utils.GetCsvData(mKols, pPregnancyCheckCsv, ';', "EVENT");
        //            int teller = 0;
        //            int totaal = tblIns.Rows.Count;
        //            unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  PregnancyChecks:{tblIns.Rows.Count} ");
        //            if (tblIns.Rows.Count > 0)
        //            {
                       
        //                foreach (DataRow rw in tblIns.Rows)
        //                {
        //                    teller += 1;
        //                    int procent = teller * 100 / totaal;
        //                    ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, rw["AnimalNumber"].ToString(), "", ChangedBy, pUbn.ThrID);
        //                    if (pAnimal.AniId > 0)
        //                    {
        //                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
        //                        if (ac.AniId > 0 && ac.Anicategory < 5)
        //                        {
        //                            if (ac.AniWorknumber == "")
        //                            {
        //                                string worknr = "";
        //                                string tempnr = "";
        //                                ANIMAL vader = new ANIMAL();
        //                                Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out worknr, out tempnr);
        //                                ac.AniWorknumber = worknr;
        //                                DB.SaveAnimalCategory(ac);
        //                            }
        //                        }
        //                        if (RequestUpdate != null)
        //                        {
        //                            mvEvent.Progress = procent;
        //                            mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("drachtcontrole", "PregnancyChecks") + " " + pAnimal.AniLifeNumber;
        //                            RequestUpdate(this, mvEvent);
        //                        }
        //                        DateTime evdate = utils.getDateLNV(rw["Date"].ToString());
        //                        EVENT eve = DB.GetEventByDateAniIdKind(evdate, pAnimal.AniId, lEveKind);
        //                        if (eve.EventId == 0)
        //                        {
        //                            eve.EveDate = evdate;
        //                            Event_functions.handleEventTimes(ref eve, evdate);
        //                            eve.AniId = pAnimal.AniId;
        //                            eve.happened_at_FarmID = pBedrijf.FarmId;
        //                            eve.EveKind = lEveKind;
        //                            eve.Changed_By = ChangedBy;
        //                            eve.SourceID = pUbn.ThrID;
        //                            eve.EveMutationBy = 15;
        //                            eve.UBNId = pUbn.UBNid;
        //                            eve.EveOrder = Event_functions.getNewEventOrder(pToken, evdate, lEveKind, pUbn.UBNid, pAnimal.AniId);

        //                            eve.ThirdId = pUbn.ThrID;
        //                            if (rw["Description"] != DBNull.Value)
        //                            {
        //                                eve.EveComment = rw["Description"].ToString().Replace(";", ":");
        //                            }
        //                            if (DB.SaveEvent(eve))
        //                            {

        //                                /*
        //                                    <option value="2" Drachtig verklaard
        //                                    <option value="3"   Niet drachtig
 
        //                                 */
        //                                int lDrachtig = 3;
        //                                try
        //                                {
        //                                    int dr = Convert.ToInt32(rw["IsPregnant"]);
        //                                    if (dr > 0)
        //                                    { lDrachtig = 2; }
        //                                }
        //                                catch { }
        //                                GESTATIO ges = new GESTATIO();
        //                                ges.EventId = eve.EventId;
        //                                ges.GesStatus = lDrachtig;
        //                                DB.SaveGestation(ges);
        //                                unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : INSERT GESTATIO Dcf Datum:{eve.EveDate} ");

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (File.Exists(pClinicalRecordingsOutputCsv))
        //        {

        //            string[] mKols = { "AnimalNumber", "Herdnumber", "Date", "Code", "Description", "GroupName", "ClinicalValue" };

        //            DataTable tblIns = utils.GetCsvData(mKols, pClinicalRecordingsOutputCsv, ';', "EVENT");
        //            int teller = 0;
        //            int totaal = tblIns.Rows.Count;
        //            unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  ClinicalRecordings:{tblIns.Rows.Count}   DeensDiseaseConversion ");
        //            if (tblIns.Rows.Count > 0)
        //            {
                        
        //                DeensDiseaseConversion DanishImport = new DeensDiseaseConversion(pToken, pUbn, pBedrijf, ChangedBy);
        //                foreach (DataRow rw in  tblIns.Rows)
        //                {
        //                    teller += 1;
        //                    int procent = teller * 100 / totaal;
        //                    string lnummer = rw["AnimalNumber"].ToString();
        //                    ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, lnummer, "", ChangedBy, pUbn.ThrID);
        //                    if (pAnimal.AniId > 0)
        //                    {
        //                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
        //                        if (ac.AniId > 0 && ac.Anicategory < 5)
        //                        {
        //                            if (ac.AniWorknumber == "")
        //                            {
        //                                string worknr = "";
        //                                string tempnr = "";
        //                                ANIMAL vader = new ANIMAL();
        //                                Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out  worknr, out tempnr);
        //                                ac.AniWorknumber = worknr;
        //                                DB.SaveAnimalCategory(ac);
        //                            }
        //                        }
        //                        if (RequestUpdate != null)
        //                        {
        //                            mvEvent.Progress = procent;
        //                            mvEvent.Message = "ClinicalRecordings " + pAnimal.AniLifeNumber;
        //                            RequestUpdate(this, mvEvent);
        //                        }
        //                        DateTime evdate = utils.getDateLNV(rw["Date"].ToString());
        //                        int code = 0;
        //                        int.TryParse(rw["Code"].ToString(), out code);
        //                        if (code > 0)
        //                        {
        //                            DanishImport.ImportDiagnoseDCF(pAnimal.AniId, code, evdate);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (!String.IsNullOrEmpty(pAnimalWeigtscsv))
        //        {
        //            if (File.Exists(pAnimalWeigtscsv))
        //            {
        //                //lnummer + ";" + record.HerdNumber.ToString() + ";" + record.Date.ToString("yyyyMMdd") + ";" + gewicht.ToString() + ";"
        //                string[] mKols = { "AnimalNumber", "Herdnumber", "Date", "gewicht" };

        //                DataTable tblIns = utils.GetCsvData(mKols, pAnimalWeigtscsv, ';', "WEIGHT");
        //                int teller = 0;
        //                int totaal = tblIns.Rows.Count;
        //                unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer}  AnimalWeigts:{tblIns.Rows.Count} ");
        //                if (tblIns.Rows.Count > 0)
        //                {
        //                    //deenssysteemVS8.deenssysteemimporter DanishImport = new deenssysteemVS8.deenssysteemimporter(pToken, pBedrijf, pUbn);
        //                    foreach (DataRow rw in tblIns.Rows)
        //                    {
        //                        teller += 1;
        //                        int procent = teller * 100 / totaal;
        //                        string lnummer = rw["AnimalNumber"].ToString();
        //                        ANIMAL pAnimal = getAnimalDCF(pToken, pBedrijf, pUbn, soapservice, fUserName, fPassword, lnummer, "", ChangedBy, pUbn.ThrID);
        //                        if (pAnimal.AniId > 0)
        //                        {
        //                            ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
        //                            if (ac.AniId > 0 && ac.Anicategory < 5)
        //                            {
        //                                if (ac.AniWorknumber == "")
        //                                {
        //                                    string worknr = "";
        //                                    string tempnr = "";
        //                                    ANIMAL vader = new ANIMAL();
        //                                    Event_functions.getRFIDNumbers(pToken, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, 0, out  worknr, out tempnr);
        //                                    ac.AniWorknumber = worknr;
        //                                    DB.SaveAnimalCategory(ac);
        //                                }
        //                            }
        //                            if (RequestUpdate != null)
        //                            {
        //                                mvEvent.Progress = procent;
        //                                mvEvent.Message = VSM_Ruma_OnlineCulture.getStaticResource("gewichten", "Gewichten") + "  " + pAnimal.AniLifeNumber;
        //                                RequestUpdate(this, mvEvent);
        //                            }
        //                            DateTime evdate = utils.getDateLNV(rw["Date"].ToString());
        //                            double lGewicht = 0;
        //                            double.TryParse(rw["gewicht"].ToString(), out lGewicht);
        //                            if (lGewicht > 0)
        //                            {
        //                                List<WEIGHT> gewichten = DB.getWeights(pAnimal.AniId);
        //                                if (evdate.Date == pAnimal.AniBirthDate.Date)
        //                                {
        //                                    BIRTH b = DB.GetBirthByCalfId(pAnimal.AniId);
        //                                    if (b.EventId > 0 && b.CalfWeight == 0)
        //                                    {
        //                                        b.CalfWeight = lGewicht;
        //                                        DB.SaveBirth(b);
        //                                        unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : Update BIRTH CalfWeight   ");
        //                                    }
        //                                }

        //                                var opgeslagen = from n in gewichten where n.WeightDate.Date == evdate.Date select n;
        //                                if (opgeslagen.Count() == 0)
        //                                {
        //                                    WEIGHT w = new WEIGHT();
        //                                    w.AniId = pAnimal.AniId;
        //                                    w.WeightDate = evdate;
        //                                    w.WeightKg = lGewicht;
        //                                    w.WeightOrder = 1;
        //                                    DB.SaveWeight(w);
        //                                    unLogger.WriteInfo($@"UBN {pUbn.Bedrijfsnummer} {pAnimal.AniLifeNumber} : INSERT WEIGHT {w.WeightDate} gewicht:{lGewicht}  ");
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        
        //private ANIMAL getKIFather(UserRightsToken pToken, BEDRIJF pBedrijf, UBN pUbn, string pBullHerdbookNumber, string pName)
        //{
        //    Regex regExLifenr = new Regex(@"^[A-Z]{2}\s[\D\d-]{1,12}$");
        //    ANIMAL father = new ANIMAL();
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(pBullHerdbookNumber))
        //        {
        //            //AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
        //            DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
        //            string lLifenumber = "";
        //            string name = "";
        //            string errmes = "";
        //            if (!NavBullSearch.GetBullByKICode(pBullHerdbookNumber, "HOL", out lLifenumber, out name, out errmes))
        //            {
        //                if (!NavBullSearch.GetBullByKICode(pBullHerdbookNumber, "RDC", out lLifenumber, out name, out errmes))
        //                {
        //                    if (!NavBullSearch.GetBullByKICode(pBullHerdbookNumber, "JER", out lLifenumber, out name, out errmes))
        //                    {
        //                        if (!NavBullSearch.GetBullByKICode(pBullHerdbookNumber, "RED", out lLifenumber, out name, out errmes))
        //                        {

        //                        }
        //                    }
        //                }
        //            }

        //            //NLD000000764384574 	
        //            //replace as NLDM000764384574 	>> M 
        //            if (lLifenumber.Length > 8)
        //            {
        //                try
        //                {
        //                    if (lLifenumber.Substring(3, 3) == "000")
        //                    {
        //                        string een = lLifenumber.Substring(0, 3);
        //                        int l = lLifenumber.Length;
        //                        string twee = lLifenumber.Substring(6, l - 6);
        //                        lLifenumber = een + "M" + twee;
        //                    }

        //                    father = DB.GetAnimalByBullITBNumber(lLifenumber);
        //                    if (father.AniId > 0)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        father = new ANIMAL();
        //                        father.BullITBNumber = lLifenumber;
                                
        //                        if (regExLifenr.Match(name).Success)
        //                        {
        //                            unLogger.WriteInfo("MutationUpdater getKIFather AniName=" + name + " ipv Gebruikelijke naam voor : " + lLifenumber);
        //                        }
        //                        father.AniName = name;
        //                        father.AniSex = 1;

        //                        DB.SaveAnimal(pUbn.ThrID, father);
        //                    }
        //                }
        //                catch (Exception exc) { unLogger.WriteWarn("NavBullSearch.GetBullByKICode " + pBullHerdbookNumber + " Result:" + errmes + " Err:" + exc.ToString()); }
        //            }
        //            else
        //            {
        //                unLogger.WriteInfo("NavBullSearch.GetBullByKICode " + pBullHerdbookNumber + " Result:" + errmes);
        //            }
        //        }
        //    }
        //    catch { }
        //    return father;
        //}

        //public ANIMAL getAnimalDCF(UserRightsToken pToken, BEDRIJF pBedrijf, UBN pUbn, SoapDCF.AnimalService soapservice, string fUserName, string fPassword, string pAniAlternateNumber, string pAniMotherLifenumber,int changedby, int sourceid)
        //{
        //    ANIMAL ani = new ANIMAL();
        //    Regex r = new Regex(@"^\d{5,15}$");
        //    char[] splitras = { '-'};
           
        //    if (pAniAlternateNumber.Length > 6)
        //    {
        //        if (r.Match(pAniAlternateNumber).Success)
        //        {
        //            pAniAlternateNumber = "DK 0" + pAniAlternateNumber;
        //        }
        //        //AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
        //        DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
        //        ani = DB.GetAnimalByAniAlternateNumber(pAniAlternateNumber);
        //        ani.Changed_By = changedby;
        //        ani.SourceID = sourceid;
        //        if (ani.AniId <= 0) // bestaat nog niet in agrobase
        //        {

        //            //lock (padlock) // 2014-05-01 als 2 administraties hetzelfde dier bevatten komt hij dubbel in agrobase
        //            //{
        //            // binnen de lock nogmaals checken, misschien is hij inmiddels door een andere thread aangemaakt.
        //            ani = DB.GetAnimalByAniAlternateNumber(pAniAlternateNumber);//GetDCFAnimalByAniAlternateNumber(pToken, row["Levensnummer"].ToString());
        //            ani.Changed_By = changedby;
        //            ani.SourceID = sourceid;
        //            if (ani.AniId <= 0)
        //            {


        //                long dcfAnimalnumberMother = 0;
        //                long dcfAnimalnumberFather = 0;
        //                long dcfSurrogatMotherAnimalNumber = 0;
        //                long dcfnummer = 0;
        //                char[] splitje = { ' ' };
        //                string[] levensnr = pAniAlternateNumber.Split(splitje);
        //                if (levensnr.Count() == 2)
        //                {
        //                    long.TryParse(levensnr[1], out dcfnummer);
        //                }
        //                else { long.TryParse(pAniAlternateNumber, out dcfnummer); }



        //                string pUbnHerdnumber = "";
        //                string pAniRace = "";
        //                ani = soapservice.fillAnimal(dcfnummer, fUserName, fPassword, out dcfAnimalnumberMother, out dcfAnimalnumberFather, out pUbnHerdnumber, out dcfSurrogatMotherAnimalNumber, out pAniRace);
        //                ani.Changed_By = changedby;
        //                ani.SourceID = sourceid;
        //                string[] rasssplit = pAniRace.Split(splitras);
        //                if (ani.AniId > 0 && rasssplit.Count() == 3)
        //                {
        //                    List<AGRO_LABELS> rassen = DB.GetAgroLabels(205, 208, 0, 3);
        //                    if (pBedrijf.ProgId != 3)
        //                    {
        //                        rassen = DB.GetAgroLabels(205, 208, 0, 0);
        //                    }


        //                    var rass = from n in rassen where n.LabLabel.StartsWith(rasssplit[1].Trim()) select n;
        //                    if (rass.Count() > 0)
        //                    {
        //                        List<SECONRAC> rac = DB.GetSeconRacSByAnimalId(ani.AniId);
        //                        if (rac.Count() == 0)
        //                        {
        //                            SECONRAC srac = new SECONRAC();
        //                            srac.AniId = ani.AniId;
        //                            srac.RacId = rass.ElementAt(0).LabID;
        //                            srac.SraRate = 8;
        //                            DB.SaveSeconRace(srac);
        //                        }
        //                        else
        //                        {
                                    
        //                            AGRO_LABELS lbl = new AGRO_LABELS();
        //                            lbl.LabLabel = pAniRace;
        //                            lbl.LabKind = 205;
        //                            lbl.LabCountry = 208;
        //                            lbl.LabProgID = pBedrijf.ProgId;
        //                            lbl.LabProgramID = 0;

        //                            CORE.utils.MissingDCFRace(lbl);
        //                        }
        //                    }

        //                }
        //                if (ani.AniAlternateNumber != "")
        //                {
        //                    if (saveAnimalDCF(ani, soapservice, fUserName, fPassword, DB, pBedrijf, pUbn))
        //                    {
        //                        if (pAniMotherLifenumber.Length > 5)
        //                        {
        //                            string moedernr = pAniMotherLifenumber;
        //                            if (r.Match(moedernr).Success)
        //                            {
        //                                moedernr = "DK 0" + moedernr;
        //                            }
        //                            ANIMAL moeder = DB.GetAnimalByAniAlternateNumber(moedernr); //GetDCFAnimalByAniAlternateNumber(pToken, row["motherAnimalNumber"].ToString());
        //                            moeder.Changed_By = changedby;
        //                            moeder.SourceID = sourceid;
        //                            if (moeder.AniId == 0)
        //                            {
        //                                if (dcfAnimalnumberMother > 0)
        //                                {
        //                                    long dcfAnimalnumberMother2 = 0;
        //                                    long dcfAnimalnumberFather2 = 0;
        //                                    long dcfSurrogatMotherAnimalNumber2 = 0;
        //                                    string UBNnummerMoeder = "";
        //                                    string pAniRace2 = "";
        //                                    moeder = soapservice.fillAnimal(dcfAnimalnumberMother, fUserName, fPassword, out dcfAnimalnumberMother2, out dcfAnimalnumberFather2, out UBNnummerMoeder, out dcfSurrogatMotherAnimalNumber2, out pAniRace2);
        //                                    if (moeder.AniAlternateNumber != "")
        //                                    {
        //                                        long temp = 0;
        //                                        if (soapservice.bornOnFarm(dcfAnimalnumberMother, fUserName, fPassword, pUbn.Bedrijfsnummer, out temp))
        //                                        {
        //                                            moeder.ThrId = pUbn.ThrID;
        //                                        }

        //                                        if (saveAnimalDCF(moeder, soapservice, fUserName, fPassword, DB, pBedrijf, pUbn))
        //                                        {

        //                                        }
        //                                    }

        //                                }
        //                            }
                                    
        //                            if (moeder.AniId > 0 && moeder.AniSex == 0)
        //                            {
        //                                moeder.AniSex = 2;
        //                                DB.SaveAnimal(pUbn.ThrID, moeder);
        //                            }
        //                            if (ani.AniIdMother == 0)
        //                            {
        //                                ani.AniIdMother = moeder.AniId;
        //                                DB.UpdateANIMAL(pUbn.ThrID, ani);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        else
        //        {

        //            long dcfnummer = 0;
        //            char[] splitje = { ' ' };
        //            string[] levensnr = pAniAlternateNumber.Split(splitje);
        //            if (levensnr.Count() == 2)
        //            {
        //                long.TryParse(levensnr[1], out dcfnummer);
        //            }
        //            else { long.TryParse(pAniAlternateNumber, out dcfnummer); }
        //            List<SECONRAC> rassen = DB.GetSeconRacSByAnimalId(ani.AniId);
        //            if (ani.AniBirthDate == DateTime.MinValue || rassen.Count()==0)
        //            {
        //                long dcfAnimalnumberMother = 0;
        //                long dcfAnimalnumberFather = 0;
        //                long dcfSurrogatMotherAnimalNumber = 0;
        //                string pUbnHerdnumber = "";
        //                string pAniRace = "";
        //                ANIMAL aniTemp = soapservice.fillAnimal(dcfnummer, fUserName, fPassword, out dcfAnimalnumberMother, out dcfAnimalnumberFather, out pUbnHerdnumber,out dcfSurrogatMotherAnimalNumber, out pAniRace);
        //                string[] rasssplit = pAniRace.Split(splitras);
        //                if (aniTemp.AniBirthDate > DateTime.MinValue)
        //                {
        //                    ani.AniBirthDate = aniTemp.AniBirthDate;
        //                    DB.UpdateANIMAL(pUbn.ThrID, ani);
        //                }
        //                if (ani.AniId > 0 && rasssplit.Count() ==3)
        //                {
        //                    List<AGRO_LABELS> rassenlbls = DB.GetAgroLabels(205, 208, 0, 3);
        //                    if (pBedrijf.ProgId != 3)
        //                    {
        //                        rassenlbls = DB.GetAgroLabels(205, 208, 0, 0);
        //                    }
        //                    var rass = from n in rassenlbls where n.LabLabel.StartsWith(rasssplit[1].Trim()) select n;
        //                    if (rass.Count() > 0)
        //                    {
        //                        List<SECONRAC> rac = DB.GetSeconRacSByAnimalId(ani.AniId);
        //                        if (rac.Count() == 0)
        //                        {
        //                            SECONRAC srac = new SECONRAC();
        //                            srac.AniId = ani.AniId;
        //                            srac.RacId = rass.ElementAt(0).LabID;
        //                            srac.SraRate = 8;
        //                            DB.SaveSeconRace(srac);
        //                        }
        //                        else
        //                        {
        //                            AGRO_LABELS lbl = new AGRO_LABELS();
        //                            lbl.LabLabel = pAniRace;
        //                            lbl.LabKind = 205;
        //                            lbl.LabCountry = 208;
        //                            lbl.LabProgID = pBedrijf.ProgId;
        //                            lbl.LabProgramID = 0;

        //                            CORE.utils.MissingDCFRace(lbl);
        //                        }
        //                    }
        //                }
        //            }
                    

        //            if (ani.AniIdMother == 0)
        //            {
        //                if (pAniMotherLifenumber.Length > 5)
        //                {
        //                    string moedernr = pAniMotherLifenumber;
        //                    if (r.Match(moedernr).Success)
        //                    {
        //                        moedernr = "DK 0" + moedernr;
        //                    }
        //                    ANIMAL moeder = DB.GetAnimalByAniAlternateNumber(moedernr); //GetDCFAnimalByAniAlternateNumber(pToken, row["motherAnimalNumber"].ToString());
        //                    moeder.Changed_By = changedby;
        //                    moeder.SourceID = sourceid;
        //                    if (moeder.AniId == 0)
        //                    {
                                
        //                        long dcfAnimalnumberMother = 0;

        //                        string[] moederlevensnr = pAniMotherLifenumber.Split(splitje);
        //                        if (moederlevensnr.Count() == 2)
        //                        {
        //                            long.TryParse(moederlevensnr[1], out dcfAnimalnumberMother);
        //                        }
        //                        else { long.TryParse(pAniMotherLifenumber, out dcfAnimalnumberMother); }

        //                        if (dcfAnimalnumberMother > 0)
        //                        {
        //                            long dcfAnimalnumberMother2 = 0;
        //                            long dcfAnimalnumberFather2 = 0;
        //                            long dcfSurrogatMotherAnimalNumber2 = 0;
        //                            string UBNnummerMoeder = "";
        //                            string pAniRace = "";
        //                            moeder = soapservice.fillAnimal(dcfAnimalnumberMother, fUserName, fPassword, out dcfAnimalnumberMother2, out dcfAnimalnumberFather2, out UBNnummerMoeder, out dcfSurrogatMotherAnimalNumber2, out pAniRace);
        //                            if (moeder.AniAlternateNumber != "")
        //                            {
        //                                long temp = 0;
        //                                if (soapservice.bornOnFarm(dcfAnimalnumberMother, fUserName, fPassword, pUbn.Bedrijfsnummer, out temp))
        //                                {
        //                                    moeder.ThrId = pUbn.ThrID;
        //                                }
        //                                if (moeder.AniSex == 0)
        //                                {
        //                                    moeder.AniSex = 2;
        //                                }
        //                                if (saveAnimalDCF(moeder, soapservice, fUserName, fPassword, DB, pBedrijf, pUbn))
        //                                {

        //                                }
        //                            }

        //                        }
        //                    }
        //                    if (moeder.AniId > 0 && moeder.AniSex == 0)
        //                    {
        //                        moeder.AniSex = 2;
        //                        DB.SaveAnimal(pUbn.ThrID, moeder);
        //                    }
        //                    if (ani.AniIdMother == 0)
        //                    {
        //                        ani.AniIdMother = moeder.AniId;
        //                        DB.UpdateANIMAL(pUbn.ThrID, ani);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return ani;
        //}

        //public void vulrassen(UserRightsToken pToken, BEDRIJF pBedrijf, UBN pUbn, SoapDCF.AnimalService soapservice, ANIMAL ani, string fUserName, string fPassword)
        //{
        //    if (ani.AniId > 0)
        //    {
        //        //AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
        //        DB.DBMasterQueries DB = new DB.DBMasterQueries(pToken);
        //        List<SECONRAC> rac = DB.GetSeconRacSByAnimalId(ani.AniId);
        //        char[] splitras = { '-' };
        //        if (rac.Count() == 0)
        //        {
        //            long dcfAnimalnumberMother = 0;
        //            long dcfAnimalnumberFather = 0;
        //            long dcfSurrogatMotherAnimalNumber = 0;
        //            long dcfnummer = 0;
        //            char[] splitje = { ' ' };
        //            string[] levensnr = ani.AniAlternateNumber.Split(splitje);
        //            if (levensnr.Count() == 2)
        //            {
        //                long.TryParse(levensnr[1], out dcfnummer);
        //            }
        //            else { long.TryParse(ani.AniAlternateNumber, out dcfnummer); }

        //            string pUbnHerdnumber = "";
        //            string pAniRace = "";
        //            ANIMAL lDummyani = soapservice.fillAnimal(dcfnummer, fUserName, fPassword, out dcfAnimalnumberMother, out dcfAnimalnumberFather, out pUbnHerdnumber, out dcfSurrogatMotherAnimalNumber, out pAniRace);
        //            if (!String.IsNullOrEmpty(pAniRace))
        //            {
        //                string[] rasssplit = pAniRace.Split(splitras);
        //                if (rasssplit.Count() == 3)
        //                {
        //                    List<AGRO_LABELS> rassen = DB.GetAgroLabels(205, 208, 0, 3);
        //                    if (pBedrijf.ProgId != 3)
        //                    {
        //                        rassen = DB.GetAgroLabels(205, 208, 0, 0);
        //                    }
        //                    var rass = from n in rassen where n.LabLabel.StartsWith(rasssplit[1].Trim()) select n;
        //                    if (rass.Count() > 0)
        //                    {

        //                        SECONRAC srac = new SECONRAC();
        //                        srac.AniId = ani.AniId;
        //                        srac.RacId = rass.ElementAt(0).LabID;
        //                        srac.SraRate = 8;
        //                        DB.SaveSeconRace(srac);

        //                    }
        //                    else
        //                    {
        //                        AGRO_LABELS lbl = new AGRO_LABELS();
        //                        lbl.LabLabel = pAniRace;
        //                        lbl.LabKind = 205;
        //                        lbl.LabCountry = 208;
        //                        lbl.LabProgID = pBedrijf.ProgId;
        //                        lbl.LabProgramID = 0;

        //                        CORE.utils.MissingDCFRace(lbl);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public void checkForMsOptimaboxRespondernumber(UserRightsToken pToken, BEDRIJF farm, ANIMAL pAnimal, bool pOmnummeren)
        {
            /*
             Als bij de Nachtelijke Sync en/of PLC-aansturing dieren voorbij komen die geen transmitter/respondernummer 
             *  hebben voor de MsOptimaBox dan moet deze automatisch toegevoegd worden.
                (dit moet werkelijk opgeslagen worden ivm vermeldingen op lijsten etc).
                Voor NL dieren zal dit worden : 528 - opvulnullen - LEVENSNUMMER (totaal 15 karakters ?)
                Voor DE en BE dieren zal dit worden : 999 - opvulnullen - LEVENSNUMMER (totaal 15 karakters ?)
             */
            /*
             Wanneer een dier is afgevoerd of dood is dan geen responder nummer
             * 
             */
            unLogger.WriteInfo($@"VSM.RUMA.CORE MutationUpdater.cs checkForMsOptimaboxRespondernumber {pToken.MasterUser} ");// {Environment.StackTrace}"); geeft error
            try
            {
 
                DB.DBMasterQueries db = new DB.DBMasterQueries(pToken);
                if (farm.Programid == (int)CORE.DB.LABELSConst.programId.MS_OPTIMA_BOX && farm.FarmId > 0 && pAnimal.AniId > 0)
                {
                    ANIMALCATEGORY ac = db.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, farm.FarmId);
                    if (ac.Anicategory > 0 && ac.Anicategory < 4)
                    {
                        DBSelectQueries ds = new DBSelectQueries(pToken);
                        if (ds.Configs.getAutoresponders(farm.FarmId, farm.Programid))
                        {
                            int lMSOptimaboxNummer = 0;
                            fillcountrycodes(db);
                            DataTable tbl = ds.Responders.getProcessCompIds(farm.FarmId, farm.UBNid, farm.ProgId);
                            int lpMSOptimabox = (int)CORE.utils.procesComputerReferenceNumber.MSOptimabox;
                            foreach (DataRow rw in tbl.Rows)
                            {
                                if (rw["Nummer"] != DBNull.Value)
                                {

                                    int.TryParse(rw["Nummer"].ToString(), out lMSOptimaboxNummer);
                                    if (lMSOptimaboxNummer >= 100)
                                    {
                                        if (lMSOptimaboxNummer.ToString().StartsWith(lpMSOptimabox.ToString()))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            if (lMSOptimaboxNummer == 0)
                            {

                                lMSOptimaboxNummer = int.Parse(lpMSOptimabox.ToString() + "01");
                            }
                            string lTransmitterNumber = "";
                            char[] split = { ' ' };
                            string[] strAniAlternateNumber = pAnimal.AniAlternateNumber.Split(split);
                            if (strAniAlternateNumber.Length == 2)
                            {

                                while (strAniAlternateNumber[1].Length < 12)
                                {
                                    strAniAlternateNumber[1] = "0" + strAniAlternateNumber[1];

                                }
                                if (strAniAlternateNumber[0].ToUpper().Trim() == "NL")
                                {
                                    strAniAlternateNumber[0] = "528";
                                }
                                else
                                {
                                    string ccode = "999";
                                    try
                                    {
                                        var c = countrycodes.FirstOrDefault(x => x.LandAfk2 == strAniAlternateNumber[0].ToUpper().Trim());
                                        if (c != null && c.LandNummer > 0)
                                        {
                                            ccode = c.LandNummer.ToString("000");
                                        }
                                    }
                                    catch(Exception exc) 
                                    {
                                        unLogger.WriteError($@"ERROR converting: {strAniAlternateNumber[0]} to countrycode in MutationUpdater:checkForMsOptimaboxRespondernumber ");
                                    }
                                    strAniAlternateNumber[0] = ccode;
                                }
                                lTransmitterNumber = strAniAlternateNumber[0] + strAniAlternateNumber[1];
                            }
                            else
                            {
                                lTransmitterNumber = pAnimal.AniAlternateNumber.Trim();
                            }
                            List<TRANSMIT> trans = db.GetTransmitByAniIdFarmId(farm.FarmId, farm.UBNid, pAnimal.AniId);
                            var check = from n in trans where (n.ProcesComputerId == lMSOptimaboxNummer || n.Koppelnr == lMSOptimaboxNummer) select n;
                            if (check.Count() == 0)
                            {


                                if (lTransmitterNumber.Length > 0)
                                {
                                    UBN ubn = db.GetubnById(farm.UBNid);
                                    TRANSMIT tr = new TRANSMIT();
                                    tr.AniId = pAnimal.AniId;
                                    tr.farmid = farm.FarmId;
                                    tr.UbnID = farm.UBNid;
                                    tr.FarmNumber = ubn.Bedrijfsnummer;
                                    tr.Koppelnr = 0;
                                    tr.ProcesComputerId = lMSOptimaboxNummer;
                                    tr.TransmitterNumber = lTransmitterNumber;
                                    tr.Worknumber = db.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, farm.FarmId).AniWorknumber;
                                    tr.Changed_By = 15;
                                    tr.SourceID = ubn.ThrID;
                                    if (!db.SaveTransMit(tr))
                                    {
                                        unLogger.WriteError("Mutationupdater checkForMsOptimaboxRespondernumber TransmitterNumber= " + lTransmitterNumber + " checkForMsOptimaboxRespondernumber SaveTransMit ERROR 1 SaveTransMit AniId=" + pAnimal.AniId.ToString() + " UBN:" + tr.FarmNumber + " lMSOptimaboxNummer:" + lMSOptimaboxNummer);
                                    }
                                    else
                                    {
                                        unLogger.WriteInfo($@"Mutationupdater checkForMsOptimaboxRespondernumber toegevoegd: TransmitterNumber= " + lTransmitterNumber + " checkForMsOptimaboxRespondernumber  " + pAnimal.AniAlternateNumber + " UBN:" + tr.FarmNumber + " lMSOptimaboxNummer:" + lMSOptimaboxNummer);
                                    }
                                }
                                else { unLogger.WriteWarn("Mutationupdater checkForMsOptimaboxRespondernumber lTransmitterNumber.Length = 0 AniId=" + pAnimal.AniId.ToString() + " FarmId:" + farm.FarmId.ToString() + " lMSOptimaboxNummer:" + lMSOptimaboxNummer); }


                            }
                            else if (check.Count() > 0 && pOmnummeren)
                            {
                                if (lTransmitterNumber.Length > 0)
                                {
                                    UBN ubn = db.GetubnById(farm.UBNid);
                                    TRANSMIT tr = check.ElementAt(0);
                                    tr.TransmitterNumber = lTransmitterNumber;
                                    tr.UbnID = farm.UBNid;
                                    tr.farmid = farm.FarmId;
                                    tr.Changed_By = 15;
                                    tr.SourceID = ubn.ThrID;
                                    if (!db.SaveTransMit(tr))
                                    {
                                        unLogger.WriteError("Mutationupdater checkForMsOptimaboxRespondernumber TransmitterNumber= " + lTransmitterNumber + " SaveTransMit ERROR 2   AniId=" + pAnimal.AniId.ToString() + " UBN:" + tr.FarmNumber + " lMSOptimaboxNummer:" + lMSOptimaboxNummer + " Omnummeren");
                                    }
                                    else
                                    {
                                        unLogger.WriteInfo($@"Mutationupdater checkForMsOptimaboxRespondernumber omgenummerd: TransmitterNumber= " + lTransmitterNumber + " checkForMsOptimaboxRespondernumber   " + pAnimal.AniAlternateNumber + " UBN:" + tr.FarmNumber + " lMSOptimaboxNummer:" + lMSOptimaboxNummer);
                                    }
                                }
                            }
                            else if (check.Count() > 0)
                            {
                                TRANSMIT tr = check.ElementAt(0);
                                if (tr.farmid != farm.FarmId)
                                {
                                    UBN ubn = db.GetubnById(farm.UBNid);
                                    TRANSMIT trnew = new TRANSMIT();
                                    trnew.AniId = tr.AniId;
                                    trnew.FarmNumber = ubn.Bedrijfsnummer;
                                    trnew.Koppelnr = tr.Koppelnr;
                                    trnew.ProcesComputerId = tr.ProcesComputerId;
                                    trnew.TransmitterNumber = tr.TransmitterNumber;
                                    trnew.UbnID = farm.UBNid;
                                    trnew.Worknumber = tr.Worknumber;
                                    trnew.farmid = farm.FarmId;
                                    trnew.Changed_By = 15;
                                    trnew.SourceID = ubn.ThrID;
                                    if (db.DeleteTransmit(tr))
                                    {
                                        if (!db.SaveTransMit(trnew))
                                        {
                                            unLogger.WriteError("Mutationupdater checkForMsOptimaboxRespondernumber TransmitterNumber= " + tr.TransmitterNumber + " SaveTransMit ERROR 4   AniId=" + pAnimal.AniId.ToString() + " UBN:" + tr.FarmNumber + " lMSOptimaboxNummer:" + lMSOptimaboxNummer + " Omnummeren");

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
        }

        private void fillcountrycodes(DB.DBMasterQueries DB)
        {

            if(countrycodes == null)
            {
                countrycodes = DB.GetAllCountries();
            }
        }

        //public bool saveAnimalDCF(ANIMAL pAnimal, SoapDCF.AnimalService soapservice, string pUsername, string pPassword, VSM.RUMA.CORE.AFSavetoDB pAfsavetoDBobj, BEDRIJF pBedrijf, UBN pUbn)
        //{
        //    bool ret = false;
        //    if (pAnimal.AniAlternateNumber != "")
        //    {
        //        ANIMAL lCheck = pAfsavetoDBobj.GetAnimalByAniAlternateNumber(pAnimal.AniAlternateNumber);
        //        if (lCheck.AniId <= 0)
        //        {
        //            if (pAnimal.AniCountryCodeOrigin != "")
        //            {
        //                string pAnimalAniAlternateNumber = pAnimal.AniAlternateNumber.Replace("DK", pAnimal.AniCountryCodeOrigin);
        //                lCheck = pAfsavetoDBobj.GetAnimalByAniAlternateNumber(pAnimalAniAlternateNumber);
        //                if (lCheck.AniId > 0)
        //                {
        //                    lCheck.AniAlternateNumber = pAnimal.AniAlternateNumber;
        //                    lCheck.AniLifeNumber = pAnimal.AniLifeNumber;
        //                }
        //                else
        //                {
        //                    lCheck = pAfsavetoDBobj.GetAnimalByLifenr(pAnimalAniAlternateNumber);
        //                    if (lCheck.AniId > 0)
        //                    {
        //                        lCheck.AniAlternateNumber = pAnimal.AniAlternateNumber;
        //                        lCheck.AniLifeNumber = pAnimal.AniLifeNumber;
        //                    }
        //                }
        //            }
        //        }
        //        if (lCheck.AniId > 0)
        //        {
        //            if (pAnimal.AniLifenrOrigin != "" && pAnimal.AniCountryCodeOrigin != "")
        //            {
        //                if (lCheck.AniLifenrOrigin == "")
        //                {
        //                    lCheck.AniLifenrOrigin = pAnimal.AniLifenrOrigin;
        //                    lCheck.AniCountryCodeOrigin = pAnimal.AniCountryCodeOrigin;

        //                }
        //            }
        //            pAnimal = lCheck;
        //        }
        //        if (pAfsavetoDBobj.SaveAnimal(pUbn.ThrID, pAnimal))
        //        {
        //            ret = true;
        //            if (pAnimal.AniLifenrOrigin != "" && pAnimal.AniCountryCodeOrigin != "")
        //            {
        //                List<LEVNRMUT> levmutsen = pAfsavetoDBobj.getLevNrMutsByLifeNr(pAnimal.AniLifenrOrigin);
        //                if (levmutsen.Count() == 0)
        //                {
        //                    LEVNRMUT levmuts = new LEVNRMUT();
        //                    long dcfAnimalNumber = 0;
        //                    char[] spl = { ' ' };
        //                    string[] nummers = pAnimal.AniAlternateNumber.Split(spl);
        //                    if (nummers.Length > 1)
        //                    {
        //                        long.TryParse(nummers[1], out dcfAnimalNumber);
        //                    }
        //                    DateTime aanvoerdate = soapservice.getFirstEnteringDateDenmark(dcfAnimalNumber, pUsername, pPassword);
        //                    if (aanvoerdate == DateTime.MinValue)
        //                    {
        //                        aanvoerdate = DateTime.Now;
        //                    }
        //                    levmuts.Aniid = pAnimal.AniId;
        //                    levmuts.Datum = aanvoerdate;
        //                    levmuts.LevnrNieuw = pAnimal.AniAlternateNumber;
        //                    levmuts.LevnrOud = pAnimal.AniLifenrOrigin;

        //                    pAfsavetoDBobj.saveLevNrMut(levmuts);
        //                }
        //            }
        //        }
        //    }
        //    return ret;
        //}


    }

    public class DeensDiseaseConversion
    {
        DBMasterQueries DB { get; set; }
        UBN _ubn;
        BEDRIJF _bedr;
        int _changedby;
        int _sourceid;
        UserRightsToken _token;

        public DeensDiseaseConversion(UserRightsToken ptoken, UBN u, BEDRIJF b, int ChangedBy)
        {
            _token = ptoken;
            DB = new DBMasterQueries(ptoken);
            _ubn = u;
            _bedr = b;
            _changedby = ChangedBy;
            _sourceid = _ubn.ThrID;
        }

        public bool ImportDiagnoseDCF(int aniId, int code, DateTime evdate)
        {
            if (code <= 0 || _bedr.UBNid <= 0 || aniId <= 0)
            {
                unLogger.WriteInfo($@"VSM.RUMA.CORE.DeensDiseaseConversion.ImportDiagnoseDCF FAILED: UBN:{_ubn.Bedrijfsnummer} uBNid:{_bedr.UBNid},   aniId:{aniId},   code:{code}  ");
                return false;
            }
            unLogger.WriteInfo($@"ImportDiagnoseDCF aniId:{aniId},  code:{code},  evdate:{evdate.ToString()}");
            switch (code)
            {
                case 1:
                    ImportInheat(aniId, evdate, LABELSConst.HeatCertainty.NIET_TOCHTIG);
                    break;
                case 7:
                    ImportInheat(aniId, evdate, LABELSConst.HeatCertainty.NVT);
                    break;
                case 8:
                    ImportTreatment(aniId, evdate, LABELSConst.TreKindDefault.Hormonen);
                    break;
                case 45:
                    ImportTreatment(aniId, evdate, LABELSConst.TreKindDefault.Magneet);
                    break;
                case 190:
                    ImportBlood(aniId, evdate);
                    break;
                case 60:
                    ImportGestation(aniId, evdate, LABELSConst.gesStatus.BLIJFT_GUST);
                    break;
                case 80:
                    ImportTreatment(aniId, evdate, LABELSConst.TreKindDefault.Bekappen);
                    break;
                case 110:
                    //110 = DRACHTIG_NA_ONDERZOEK || DRACHTIG_VERKLAARD 
                    if (!DB.gestationExists(aniId, evdate, LABELSConst.gesStatus.DRACHTIG_NA_ONDERZOEK))
                        ImportGestation(aniId, evdate, LABELSConst.gesStatus.DRACHTIG_VERKLAARD);
                    break;
                case 113:
                    ImportGestation(aniId, evdate, LABELSConst.gesStatus.NIET_DRACHTIG);
                    break;
                case 172:
                    ImportTreatment(aniId, evdate, LABELSConst.TreKindDefault.VaccinatieOfEnting);
                    break;
                case 511:
                case 512:
                case 513:
                    //Afvoer/dood niet inlezen
                    unLogger.WriteWarn(String.Format("Afvoer/dood (code {0}) niet ingelezen AniId {1}", code,
                        aniId));
                    break;
                default:
                    {
                        int mainCode, subCode;
                        GetAgrobaseCode(code, out mainCode, out subCode);
                        ImportDisease(aniId, evdate, new Disease(mainCode, subCode));
                        break;
                    }
            }
            return true;
        }

        public bool GetAgrobaseCode(int deensSysteemDiagnoseCode, out int disMainCode, out int disSubCode)
        {
            disMainCode = (int)LABELSConst.disMainCode.DeensSysteemTijdelijk;
            disSubCode = deensSysteemDiagnoseCode;

            var dis =
                Items.FirstOrDefault(i => i.DeensSysteemDiagnoseCode == deensSysteemDiagnoseCode);
            if (dis == null)
                return false;

            disMainCode = dis.AgrobaseDisMainCode;
            disSubCode = dis.AgrobaseDisSubCode;
            return true;
        }

        /// <summary>
        /// Imports new inheat with status HeatCertainty if there is no INHEAT record allready on the same date
        /// </summary>
        /// <param name="aniId"></param>
        /// <param name="date"></param>
        /// <param name="heatCertainty"></param>
        /// <returns>true in case of success, false in case of an error</returns>
        internal bool ImportInheat(int aniId, DateTime date, LABELSConst.HeatCertainty heatCertainty)
        {
            if (DB.inheatExists(aniId, date, heatCertainty))
            {
                unLogger.WriteDebug(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importInheat, skipping inheat FarmId: {0} AniId: {1} Date: {2} heatCertainty {3}",
                    _bedr.FarmId , aniId, date.ToString("dd-MM-yyyy"), heatCertainty));
                return true;
            }

            EVENT e = EmptyEvent(aniId, date, LABELSConst.EventKind.TOCHTIG);
            INHEAT i = new INHEAT
            {
                HeatCertainty = (int)heatCertainty
            };

            if (Event_functions.addInheat(_token, e, i) < 0)
            {
                unLogger.WriteError(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importInheat, Error saving inheat FarmId: {0} AniId: {1} Date: {2} heatCertainty {3}",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), heatCertainty));
                return false;
            }

            unLogger.WriteDebug(String.Format(
                "VSM.RUMA.CORE.DeensDiseaseConversion -> importInheat, saved inheat FarmId: {0} AniId: {1} Date: {2} heatCertainty {3}",
                _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), heatCertainty));
            return true;
        }


        /// <summary>
        /// Imports new Event + blood record
        /// </summary>
        /// <param name="aniId"></param>
        /// <param name="date"></param>
        /// <returns>true in case of success, false in case of an error</returns>
        internal bool ImportBlood(int aniId, DateTime date)
        {
            if (DB.eventExists(aniId, date, LABELSConst.EventKind.BLOEDONDERZOEK))
            {
                unLogger.WriteDebug(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importBlood, skipping blood FarmId: {0} AniId: {1} Date: {2} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy")));
                return true;
            }
            EVENT e = EmptyEvent(aniId, date, LABELSConst.EventKind.BLOEDONDERZOEK);
            BLOOD b = new BLOOD();

            if (Event_functions.addBlood(_token, e, b) < 0)
            {
                unLogger.WriteError(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importBlood, error saving blood FarmId: {0} AniId: {1} Date: {2} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy")));
                return false;
            }

            unLogger.WriteDebug(String.Format(
                "VSM.RUMA.CORE.DeensDiseaseConversion -> importBlood, waved blood FarmId: {0} AniId: {1} Date: {2} ",
                _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy")));
            return true;
        }

        /// <summary>
        /// Import new treatment if a treatment with that type does not exist yet on that date
        /// </summary>
        /// <param name="aniId"></param>
        /// <param name="date"></param>
        /// <param name="treKind"></param>
        /// <returns></returns>
        internal bool ImportTreatment(int aniId, DateTime date, LABELSConst.TreKindDefault treKind)
        {
            if (DB.treatmentExists(aniId, date, treKind))
            {
                unLogger.WriteDebug(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importTreatment, skipping treatment FarmId: {0} AniId: {1} Date: {2} TreKind: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), treKind));
                return true;
            }

            EVENT e = EmptyEvent(aniId, date, LABELSConst.EventKind.BEHANDELING);

            TREATMEN t = new TREATMEN
            {
                TreKind = (int)treKind
            };

            if (Event_functions.saveTreatment(_token, e, t) <= 0)
            {
                unLogger.WriteError(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importTreatment, error saving treatment FarmId: {0} AniId: {1} Date: {2} TreKind: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), treKind));

                return false;
            }

            unLogger.WriteDebug(String.Format(
                "VSM.RUMA.CORE.DeensDiseaseConversion -> importTreatment, saved treatment FarmId: {0} AniId: {1} Date: {2} TreKind: {3} ",
                _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), treKind));
            return true;
        }

        internal bool ImportGestation(int aniId, DateTime date, LABELSConst.gesStatus gesStatus)
        {
            if (DB.gestationExists(aniId, date, gesStatus))
            {
                unLogger.WriteDebug(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importGestation, skipping gestation FarmId: {0} AniId: {1} Date: {2} GesStatus: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), gesStatus));
                return true;
            }
            EVENT e = EmptyEvent(aniId, date, LABELSConst.EventKind.DRACHTIGHEID);
            GESTATIO g = new GESTATIO
            {
                GesStatus = (int)gesStatus
            };

            if (Event_functions.addGestation(_token, e, g) < 0)
            {
                unLogger.WriteError(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importGestation, error saving gestation FarmId: {0} AniId: {1} Date: {2} GesStatus: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), gesStatus));

                return false;
            }

            unLogger.WriteDebug(String.Format(
                "VSM.RUMA.CORE.DeensDiseaseConversion -> importGestation, saved gestation FarmId: {0} AniId: {1} Date: {2} GesStatus: {3} ",
                _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), gesStatus));
            return true;
        }

        internal bool ImportDisease(int aniId, DateTime date, Disease dis)
        {
            return ImportDisease(aniId, date, dis, new List<Disease>() { dis });
        }

        private bool ImportDisease(int aniId, DateTime date, Disease dis, List<Disease> notIfOneOfTheseDiseasesOnDate)
        {
            if (notIfOneOfTheseDiseasesOnDate.Count == 0)
            {
                unLogger.WriteError(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importDisease, no Diseases to check against FarmId: {0} AniId: {1} Date: {2} Disease: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), dis));
                return false;
            }

            if (DB.diseaseExistsOnDate(aniId, date, notIfOneOfTheseDiseasesOnDate))
            {
                unLogger.WriteDebug(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importDisease, skipping disease FarmId: {0} AniId: {1} Date: {2} Disease: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), dis));
                return true;
            }

            if (Event_functions.insertziekte(_token, _bedr.FarmId, aniId, date, dis.MainCode, dis.SubCode, 0, 0,
                    (int)LABELSConst.MutationBy.HetDeensSysteem, _changedby, _sourceid) < 0)
            {
                unLogger.WriteError(String.Format(
                    "VSM.RUMA.CORE.DeensDiseaseConversion -> importDisease, error saving disease FarmId: {0} AniId: {1} Date: {2} Disease: {3} ",
                    _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), dis));

                return false;
            }

            unLogger.WriteDebug(String.Format(
                "VSM.RUMA.CORE.DeensDiseaseConversion -> importDisease, saved disease FarmId: {0} AniId: {1} Date: {2} Disease: {3} ",
                _bedr.FarmId, aniId, date.ToString("dd-MM-yyyy"), dis));
            return true;
        }

        private EVENT EmptyEvent(int AniId,DateTime date, LABELSConst.EventKind eveKind)
        {
            EVENT e = new EVENT
            {
                EveKind = (int)eveKind,
                AniId = AniId,
                EveDate = date,
                happened_at_FarmID = _bedr.FarmId,
                UBNId = _ubn.UBNid,
                EveMutationDate = DateTime.Now.Date,
                EveMutationTime = DateTime.Now,
                EveMutationBy = (int)LABELSConst.MutationBy.HetDeensSysteem,
                Changed_By = _changedby,
                SourceID = _sourceid
            };

            return e;
        }

        class DeensDiseaseConversionItem
        {
            public int AgrobaseDisMainCode { get; private set; }
            public int AgrobaseDisSubCode { get; private set; }
            public int DeensSysteemDiagnoseCode { get; private set; }

            public DeensDiseaseConversionItem(int disMainCode, int disSubCode, int diagnoseCode)
            {
                this.AgrobaseDisMainCode = disMainCode;
                this.AgrobaseDisSubCode = disSubCode;
                this.DeensSysteemDiagnoseCode = diagnoseCode;
            }
        }

        private static readonly IEnumerable<DeensDiseaseConversionItem> Items =
    new List<DeensDiseaseConversionItem>()
    {
                //Geen ziekte events
                //new DeensDiseaseConversionItem(0, 0, 1),
                //new DeensDiseaseConversionItem(0, 0, 7),
                //new DeensDiseaseConversionItem(0, 0, 8),
                //new DeensDiseaseConversionItem(0, 0, 45),
                //new DeensDiseaseConversionItem(0, 0, 60),
                //new DeensDiseaseConversionItem(0, 0, 80),
                //new DeensDiseaseConversionItem(0, 0, 110),
                //new DeensDiseaseConversionItem(0, 0, 113),
                //new DeensDiseaseConversionItem(0, 0, 172),
                //new DeensDiseaseConversionItem(0, 0, 190),
                //new DeensDiseaseConversionItem(0, 0, 511),
                //new DeensDiseaseConversionItem(0, 0, 512),
                //new DeensDiseaseConversionItem(0, 0, 513),


                new DeensDiseaseConversionItem(1, 2, 41),
                new DeensDiseaseConversionItem(1, 3, 41),
                new DeensDiseaseConversionItem(1, 4, 41),
                new DeensDiseaseConversionItem(1, 6, 41),
                new DeensDiseaseConversionItem(1, 11, 69),
                new DeensDiseaseConversionItem(1, 13, 176),
                new DeensDiseaseConversionItem(1, 14, 55),
                new DeensDiseaseConversionItem(1, 95, 49),
                new DeensDiseaseConversionItem(1, 134, 41),
                new DeensDiseaseConversionItem(1, 135, 176),


                new DeensDiseaseConversionItem(2, 4, 189),
                new DeensDiseaseConversionItem(2, 8, 51),
                new DeensDiseaseConversionItem(2, 9, 51),
                new DeensDiseaseConversionItem(2, 10, 205),
                new DeensDiseaseConversionItem(2, 11, 169),
                new DeensDiseaseConversionItem(2, 11, 170),
                new DeensDiseaseConversionItem(2, 15, 56),
                new DeensDiseaseConversionItem(2, 17, 52),
                new DeensDiseaseConversionItem(2, 20, 98),
                new DeensDiseaseConversionItem(2, 22, 164),
                new DeensDiseaseConversionItem(2, 41, 51),
                new DeensDiseaseConversionItem(2, 94, 25),
                new DeensDiseaseConversionItem(2, 96, 51),
                new DeensDiseaseConversionItem(2, 97, 24),
                new DeensDiseaseConversionItem(2, 99, 42),
                new DeensDiseaseConversionItem(2, 100, 42),
                new DeensDiseaseConversionItem(2, 115, 24),
                new DeensDiseaseConversionItem(2, 126, 97),
                new DeensDiseaseConversionItem(2, 127, 96),
                new DeensDiseaseConversionItem(2, 130, 29),
                new DeensDiseaseConversionItem(2, 901, 166),


                new DeensDiseaseConversionItem(3, 23, 37),
                new DeensDiseaseConversionItem(3, 24, 32),
                new DeensDiseaseConversionItem(3, 26, 34),
                new DeensDiseaseConversionItem(3, 27, 35),
                new DeensDiseaseConversionItem(3, 27, 156), // duplicated code with 35
                new DeensDiseaseConversionItem(3, 28, 33),
                new DeensDiseaseConversionItem(3, 30, 36),
                new DeensDiseaseConversionItem(3, 99, 39),
                new DeensDiseaseConversionItem(3, 100, 39),
                new DeensDiseaseConversionItem(3, 101, 39),
                new DeensDiseaseConversionItem(3, 102, 38),
                new DeensDiseaseConversionItem(3, 103, 144),
                new DeensDiseaseConversionItem(3, 201, 144),
                new DeensDiseaseConversionItem(3, 202, 151),
                new DeensDiseaseConversionItem(3, 998, 151),
                new DeensDiseaseConversionItem(3, 999, 151),


                new DeensDiseaseConversionItem(4, 60, 66),
                new DeensDiseaseConversionItem(4, 61, 66),
                new DeensDiseaseConversionItem(4, 62, 4),
                new DeensDiseaseConversionItem(4, 63, 2),
                new DeensDiseaseConversionItem(4, 64, 2),
                new DeensDiseaseConversionItem(4, 65, 2),
                new DeensDiseaseConversionItem(4, 66, 3),
                new DeensDiseaseConversionItem(4, 67, 68),

                new DeensDiseaseConversionItem(4, 68, -1),
                new DeensDiseaseConversionItem(4, 69, -1),


                new DeensDiseaseConversionItem(4, 70, 66),
                new DeensDiseaseConversionItem(4, 72, 175),
                new DeensDiseaseConversionItem(4, 73, 90),
                new DeensDiseaseConversionItem(4, 108, 91),
                new DeensDiseaseConversionItem(4, 109, 90),
                new DeensDiseaseConversionItem(4, 125, 2),


                new DeensDiseaseConversionItem(5, 4, 189),
                new DeensDiseaseConversionItem(5, 11, 169),
                new DeensDiseaseConversionItem(5, 114, 47),


                new DeensDiseaseConversionItem(6, 40, 220),
                new DeensDiseaseConversionItem(6, 41, 205),
                new DeensDiseaseConversionItem(6, 42, 205),
                new DeensDiseaseConversionItem(6, 44, 204),
                new DeensDiseaseConversionItem(6, 45, 217),
                new DeensDiseaseConversionItem(6, 46, 201),
                new DeensDiseaseConversionItem(6, 49, 214),
                new DeensDiseaseConversionItem(6, 53, 220),
                new DeensDiseaseConversionItem(6, 54, 206),
                new DeensDiseaseConversionItem(6, 118, 209),
                new DeensDiseaseConversionItem(6, 128, 215),
                new DeensDiseaseConversionItem(6, 129, 216),
                new DeensDiseaseConversionItem(6, 132, 213),
                new DeensDiseaseConversionItem(6, 133, 213),


                new DeensDiseaseConversionItem(7, 40, 220),
                new DeensDiseaseConversionItem(7, 41, 205),
                new DeensDiseaseConversionItem(7, 44, 204),
                new DeensDiseaseConversionItem(7, 45, 217),
                new DeensDiseaseConversionItem(7, 46, 201),
                new DeensDiseaseConversionItem(7, 49, 214),
                new DeensDiseaseConversionItem(7, 53, 220),
                new DeensDiseaseConversionItem(7, 54, 206),
                new DeensDiseaseConversionItem(7, 118, 209),
                new DeensDiseaseConversionItem(7, 128, 215),
                new DeensDiseaseConversionItem(7, 129, 216),


                new DeensDiseaseConversionItem(8, 36, 93),
                new DeensDiseaseConversionItem(8, 56, 93),
                new DeensDiseaseConversionItem(8, 57, 93),
                new DeensDiseaseConversionItem(8, 58, 16),
                new DeensDiseaseConversionItem(8, 59, 93),
                new DeensDiseaseConversionItem(8, 120, 16),
                new DeensDiseaseConversionItem(8, 121, 16),


                new DeensDiseaseConversionItem(9, 79, 22),
                new DeensDiseaseConversionItem(9, 80, 30),
                new DeensDiseaseConversionItem(9, 81, 21),
                new DeensDiseaseConversionItem(9, 84, 165),
                new DeensDiseaseConversionItem(9, 86, 125),
                new DeensDiseaseConversionItem(9, 87, 184),
                new DeensDiseaseConversionItem(9, 88, 184),
                new DeensDiseaseConversionItem(9, 92, 27),
                new DeensDiseaseConversionItem(9, 116, 186),
                new DeensDiseaseConversionItem(9, 117, 27),
                new DeensDiseaseConversionItem(9, 204, 186),
                new DeensDiseaseConversionItem(9, 205, 184),


                new DeensDiseaseConversionItem(13, 31, 129),
                new DeensDiseaseConversionItem(13, 32, 129),
                new DeensDiseaseConversionItem(13, 33, 178),
                new DeensDiseaseConversionItem(13, 34, 129),
                new DeensDiseaseConversionItem(13, 36, 6),
                new DeensDiseaseConversionItem(13, 141, 6),


                new DeensDiseaseConversionItem(16, 12, 170),


                new DeensDiseaseConversionItem(17, 106, 112),
                new DeensDiseaseConversionItem(17, 107, 112),
                new DeensDiseaseConversionItem(17, 108, 91),
                new DeensDiseaseConversionItem(17, 110, 92),


                new DeensDiseaseConversionItem(18, 16, 54),
                new DeensDiseaseConversionItem(18, 35, 53),
                new DeensDiseaseConversionItem(18, 52, 40),
                new DeensDiseaseConversionItem(18, 74, 41),
                new DeensDiseaseConversionItem(18, 84, 165),
                new DeensDiseaseConversionItem(18, 93, 53),
                new DeensDiseaseConversionItem(18, 98, 163),
                new DeensDiseaseConversionItem(18, 123, 131),
                new DeensDiseaseConversionItem(18, 124, 131),
                new DeensDiseaseConversionItem(18, 131, 175),


                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 3, 3),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 4, 4),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 5, 5),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 6, 6),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 9, 9),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 11, 11),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 12, 12),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 13, 13),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 14, 14),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 15, 15),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 17, 17),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 18, 18),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 19, 19),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 20, 20),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 21, 21),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 22, 22),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 23, 23),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 24, 24),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 26, 26),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 27, 27),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 28, 28),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 29, 29),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 31, 31),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 32, 32),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 33, 33),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 36, 36),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 37, 37),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 38, 38),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 39, 39),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 40, 40),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 41, 41),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 42, 42),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 46, 46),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 47, 47),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 48, 48),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 49, 49),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 51, 51),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 52, 52),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 57, 57),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 58, 58),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 59, 59),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 63, 63),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 64, 64),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 65, 65),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 66, 66),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 67, 67),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 68, 68),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 69, 69),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 70, 70),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 72, 72),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 79, 79),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 81, 81),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 82, 82),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 83, 83),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 84, 84),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 85, 85),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 86, 86),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 87, 87),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 88, 88),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 89, 89),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 90, 90),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 91, 91),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 93, 93),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 94, 94),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 95, 95),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 99, 99),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 109, 109),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 112, 112),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 114, 114),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 120, 120),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 121, 121),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 122, 122),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 123, 123),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 127, 127),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 128, 128),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 129, 129),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 130, 130),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 131, 131),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 132, 132),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 140, 140),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 141, 141),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 142, 142),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 143, 143),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 144, 144),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 145, 145),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 146, 146),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 147, 147),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 148, 148),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 149, 149),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 150, 150),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 151, 151),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 152, 152),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 153, 153),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 154, 154),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 155, 155),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 156, 156),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 160, 160),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 161, 161),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 162, 162),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 164, 164),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 166, 166),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 167, 167),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 168, 168),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 169, 169),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 171, 171),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 173, 173),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 174, 174),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 177, 177),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 178, 178),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 179, 179),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 180, 180),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 181, 181),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 182, 182),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 183, 183),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 184, 184),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 185, 185),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 186, 186),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 187, 187),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 188, 188),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 189, 189),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 201, 201),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 202, 202),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 203, 203),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 204, 204),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 205, 205),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 206, 206),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 207, 207),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 208, 208),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 209, 209),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 210, 210),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 211, 211),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 212, 212),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 213, 213),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 214, 214),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 215, 215),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 216, 216),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 217, 217),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 218, 218),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 219, 219),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 220, 220),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 250, 250),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 260, 260),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 300, 300),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 310, 310),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 320, 320),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 501, 501),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 502, 502),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 503, 503),
                new DeensDiseaseConversionItem((int) LABELSConst.disMainCode.DeensSysteemTijdelijk, 514, 514)
    };

    }
}
