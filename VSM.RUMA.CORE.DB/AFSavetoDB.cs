using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data.Common;
using MySql.Data.MySqlClient;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE
{
    /// <summary>
    /// 
    /// </summary>
    public class VSMMysqlConnectionInfo
    {
        public bool Pooling { get; }
        public string User { get; }
        public string Host { get; }
        public uint MaxPoolSize { get; }
        public uint MinPoolSize { get; }

        public VSMMysqlConnectionInfo(MySqlConnectionStringBuilder mcsb)
        {
            if (mcsb == null)
            {
                throw new ArgumentException($"{nameof(VSMMysqlConnectionInfo)}.ctor: mcsb == null");
            }
            Pooling = mcsb.Pooling;
            User = mcsb.UserID;
            Host = mcsb.Server;
            MinPoolSize = mcsb.MinimumPoolSize;
            MaxPoolSize = mcsb.MaximumPoolSize;
        }
    }

    public class MprDate : DataObject
    {
        public DateTime Date { get; set; }
        public int NumberOfRecords { get; set; }
    }

    public class EventEvekind
    {
        public EventEvekind(int eventId, int eveKind)
        {
            this.EventId = eventId;
            this.EveKind = eveKind;
        }
        public int EventId { get; set; }
        public int EveKind { get; set; }
    }

    public class AnimalFokkerInfo
    {
        public AnimalFokkerInfo(int? thrId, int? ubnId, string bedrijfsnummer, string inrichtingsnr)
        {
            this.ThrId = thrId ?? 0;
            this.UbnId = ubnId;
            this.Bedrijfsnummer = bedrijfsnummer;
            this.InrichtingsNr = inrichtingsnr;
        }
        public int ThrId { get; set; }
        public int? UbnId { get; set; }
        public string Bedrijfsnummer { get; set; }
        public string InrichtingsNr { get; set; }
    }

    public class AnimalCallInfo
    {
        public string LifeNumber;
        public int Ok;
        public int Error;
        public DateTime? TS;

        public AnimalCallInfo(string lifeNr, int ok, int error, DateTime? ts)
        {
            this.LifeNumber = lifeNr;
            this.Ok = ok;
            this.Error = error;
            this.TS = ts;
        }
    }

    public class SanitelBedrijfInfo
    {
        public int UbnId;
        /// <summary>
        /// UBN.Bedrijfsnummer
        /// </summary>
        public string ProductionUnit;
        public string Facility;

        public SanitelBedrijfInfo(int ubnId, string productionUnit, string facility)
        {
            this.UbnId = ubnId;
            this.ProductionUnit = productionUnit;
            this.Facility = facility;
        }
    }

    public interface AFSavetoDB
    {
        IDatabase GetDataBase();
        bool isFilledByDb(DataObject pData);
   
        VSMMysqlConnectionInfo GetConnectionInfo(DBConnectionToken token);


        [Obsolete("setToken is een verouderde functie")]
        void setToken(DBConnectionToken pDbconntoken);
        [Obsolete("StartTransaction  is een verouderde functie")]
        bool StartTransaction();
        [Obsolete("Commit is een verouderde functie")]
        bool Commit();
        [Obsolete("RollBack is een verouderde functie")]
        bool RollBack();
        bool UBNinDB(String pUBN);
        int GetHcoId(String pEDINRS);
        int GetRacId(String pEDINRS, string ProgId);
        string BepaalIDRRascode(BEDRIJF lFarm, String AniLifeNumber);
        ADMINISTRATIE getAdminByAdmisId(int pAdmisId);
        ADMINISTRATIE getAdminByThrId(int pThrId);
        List<ADMINISTRATIE> getAdminsByThrId(int pThrId);
        ADMINISTRATIE getAdminByProgramIdAdmisName(int pProgramId, string pAdmisName);
        ANALYSE GetAnalyseByKey(int pAnimalId, DateTime pMilkDate);
        ANALYSE GetAnalyseByKeyAndTypeOfControl(int pAnimalId, DateTime pMilkDateTime, int AnaTypeOfControl);
        List<ANALYSE> GetAnalyseByAnimal(int pAnimalId, int pUbnId);
        ANIMAL GetAnimalById(int pAnimalId);
        ANIMAL GetAnimalById(int pAnimalId, int pProgID);
        ANIMAL getAnimalByBullAiNumber(String pBullAiNumber);
        ANIMAL GetAnimalByLifenr(String pAniLifeNumber);
        ANIMAL GetAnimalByAniAlternateNumber(String pAniAlternateNumber);
        ANIMAL GetAnimalByBullITBNumber(String pBullITBNumber);
        ANIMAL GetAnimalByAniAlternateNumber(String pAniAlternateNumber, int pProgID);
        ANIMAL_SHARE_INFO GetAnimalShareInfo(int pAniId);
        string getSelection_Query(string naam, int agrolink_program_progid = 0);
        void InsertAnimalError(int pAR_AniID, int pAR_AniID_2);
        void getAnimalAndCategory(int pAniId, int pFarmId, out ANIMAL pAnimal, out ANIMALCATEGORY pAniCategory);
        List<ANIMAL> GetAnimalAndParents(int pAniId);
        List<ANIMAL> GetAnimalByLifenrMotherAndBirthdate(String pAniLifeNumberMother, DateTime pBirthDate);
        List<ANIMAL> GetBullsByFarmId(int pFarmid);
        List<ANIMAL> GetBullsByFarmId(int pFarmid, int pCurrentFather);
        List<ANIMAL> GetBullsByProgramidByAnimalCategory(int pProgramid);
        DataTable GetAnimalsByEventIds(List<int> pEventids);
        DataTable GetBullsForRammenring();
        DataTable getHondenByloggedInKennelhouderThrId(int pKennelhouderThrId);
        DataTable getHondenByloggedInThrId(int pThrId, int pProgramId);
        DataTable getChipnumberInfo(string pChipnummer, int pProgramId);
        List<ANIMAL> getKennelhouderAnimals(int pKennelHouderThrId);
        List<ANIMAL> getKennelhouderAnimalsAanwezig(int pKennelHouderThrId);
        List<MOVEMENT> GetMovementsByThrIdKind(int pThrId, int pMovKind);
        int getDogCount(int pProgramId);
        double getCreditMutatieAantal(int pAdmisID, int pFarmid);
        bool setCreditMutatieAantal(int pAdmisID, int pFarmid, double pMutatieaantal);
        DataTable getChippedOverview(int AdminUbnId, int pChipperThrId, int pProgramID);
        DataTable getChipTotalOverzicht(int AdminUbnID, List<int> pProgramids);
        List<ANIMAL> GetBullsByProgramid(int pProgramid);
        List<ANIMAL> GetAnimalsByFarmId(int pFarmid);
        List<ANIMAL> GetAnimalsByFarmId(int pFarmid, int pMaxAniCategory);
        IEnumerable<ANIMAL> GetAnimalsByUbnId(int ubnId, int maxAniCategory);

        List<ANIMAL> GetChildren(int pAniId, int pAniSex);
        DataTable GetChildren3Generations(int pAniId, int pAniSex);
        int GetChildrenCount(int pAniId);
        List<ANIMAL> GetAnimalsByAniIds(List<int> pAnimalIds);
        DataTable getAnimalsInGroup(int pFarmId, int pGroupnr);
        void getCompanyByFarmId(int pFarmId, out BEDRIJF pBedrijf, out UBN pUbn, out THIRD pThird, out COUNTRY pCountry);
        List<CHIP_BOX> getChipBoxesBySupplier(int pSupplierThrId, int pBreederThrId, out List<CHIP_STOCK> pChipStoks);
        List<CHIP_BOX> getChipStocksByBoxIds(List<int> pBoxIds, int pBreederThrId, out List<CHIP_STOCK> pChipStoks);
        void getChipboxChipstockByChipnumber(string pChipnummer, out CHIP_BOX pChipbox, out CHIP_STOCK pChipstock);
        List<TREATMEN> GetTreatmens(int TreFirstApplicationId, out List<EVENT> events);
      
        bool sellChipsByBoxIds(List<int> pBoxIds, DateTime pSelDate, int pBreederThrId);
        bool SaveChipbox(CHIP_BOX pChipbox);
        bool SaveChipStock(CHIP_STOCK pChipStock);
        List<THIRD> GetThirdsByProgramIds2(List<int> pProgramIds);
        List<THIRD> getChipAdminLedenvanWill(List<int> pProgramIds);
        List<THIRD> getFilteredDogsByCompany(List<int> pProgramIds, string pZipCode, string pHouseNr);
        bool DeleteChipBoxAndChipStocks(CHIP_BOX pChipBox);
        int getMaxMutaLogInternalNumber();
        DataTable getAnimalDiscussionsOverview(int pThrId, int pProgramid);
        DataTable getAnimalDiscussionsByTypeIdAndKind(int pThrId, int pAD_TypeId, int pLabID, int pProgramid);
        DataTable getAnimalDiscussionsByAnimalAndThrId(int pThrId, int pAniId);
        bool saveAnimalDiscussion(ANIMAL_DISCUSSION pAnimalDiscussion);
        bool deleteAnimalDiscussion(ANIMAL_DISCUSSION pAnimalDiscussion);
        DataTable searchVBKChipnummers(string pChipnummer, int pLike, int pProgramid);
        THIRD getChipper(string pChipnummer, out EVENT pChipEvent);
        List<ANIMAL> GetCurrentAnimalsForFarm(int pFarmId);
        List<ANIMAL> GetCurrentAnimalsForFarmNietOmgenummerd(int pFarmId);
        bool AlOmgenummerd(int pAniId);
        List<ANIMAL> GetNullmetingAnimalsByFarmId(int pFarmid);
        List<ANIMAL> GetPosibleMothers(int pFarmId, DateTime pBirthDay, int pCurrentMother, int pProgId);
        List<ANIMAL> GetPosibleMothers(int pFarmId, DateTime pBirthDay, int pProgId);
        List<ANIMAL> GetCurrentAnimalsByFarmIdAndSex(int pFarmId, int pSex);
        List<ANIMAL> getAnimalsByFokker(int pFokkerThrId);
        ANIMAL_AFWIJKING GetAnimalAfwijking(int pAniId, DateTime pDatum, int pAfwijkingID, int pAA_Type = 0);
        List<ANIMAL_AFWIJKING> GetAnimalAfwijkingen(int pAniId, int pAA_Type);
        List<ANIMAL_PRODUCTION> GetAnimalProductions(int pAniId);
        AUTH_GROUPS_FARM getAuthGroupsFarm(int pFarmId, int pGroupId);
        List<AUTH_GROUPS_FARM> getAuthGroupsFarms(int pGroupId);
        List<AUTH_GROUPS_RIGHTS> getAuthGroupRights(int pModuleID, int pGroupId);
        int getAuthGroupRightsGroupID(int pFarmId, int pProgramid);
        void getAnimalCurrentOwner(int pAniId, int pProgId, out BEDRIJF pBedrijf, out UBN pUbn, out THIRD pThird);
        void getAnimalCurrentOwner(int pAniId, out BEDRIJF pBedrijf, out UBN pUbn, out THIRD pThird);
        DataTable getDogOwners(int pAniId);
        COMPLAINTS getComplaint(int pComlpaintID);
        COMPLAINTS_CONFIG getComplaintsConfig(int pProgramID);
        IEnumerable<EMMMILK> GetEMMData(UBN ubn, int progId, int aniId, DateTime beginDatum, DateTime eindDatum);
        IEnumerable<EMMMILK> GetAllEmmData(UBN ubn, int progId, int aniId, DateTime beginDatum, DateTime eindDatum);        
        List<EMMMILK> getEMMdata(UBN UBN, int ProgId, DateTime pBegindatum, DateTime pEindDatum);
        [Obsolete("Gebruik de functie met UBN + Progid")]
        List<EMMMILK> getEMMdata(int pFarmId, DateTime pBegindatum, DateTime pEindDatum);
        List<EMMMILK> getEMMdata(int pFarmId, int pAniId, DateTime pBegindatum, DateTime pEindDatum);
        bool SetEMMReportDate(EMMMILK pEMMMilk);

        REPORT_INFORMATION SaveEventReportDate(EVENT pEvent, SOAPLOG SoapResult, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization ReportedToOrganization, int changedBy = 0, int sourceId = 0);
        REPORT_INFORMATION SaveMovementReportDate(MOVEMENT pMovement, SOAPLOG SoapResult, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization ReportedToOrganization);
        REPORT_INFORMATION SaveEventImportedDate(EVENT pEvent, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization ReceivedFromOrganization, DateTime ImportDateTime, int changedBy = 0, int sourceId = 0);

        bool SaveReportInformation(REPORT_INFORMATION pReportInfo);

   
        List<SUPPLY1_DETAILS> GetSupply_DetailsByFactID(int pFactID);

        List<Int32> getProcesserverComputerMachineIdsWithXmlFields(int FarmId, ulong RumaXmlFields);

        List<Int32> getProcesserverComputerMachineIdsWithXmlFieldsByUbnId(int ubnId, ulong RumaXmlFields);
        IEnumerable<Tuple<int, int>> GetProcesserverComputerMachineIdAndFarmIdWithXmlFieldsByUbnId(int ubnId, ulong rumaXMLFields);
        
        PSC_SETTINGS getProcesserverSettings(String UserName);
        int SaveProcesserverSettings(PSC_SETTINGS pSettings);
        List<PSC_PROCESCOMPUTER> getProcesserverComputers(int psId);
        PSC_PROCESCOMPUTER getProcesserverComputer(int psId, int pspcmachineId);
        PSC_PROCESCOMPUTER getProcesserverComputer(int psId, String pspcType);
        PSC_PROCESCOMPUTER_VALUE getProcesserverComputerValue(int PspcId, string PspcKey);
        List<PSC_PROCESCOMPUTER_VALUE> getProcesserverComputerValues(int PspcId);
        List<PSC_PCLINK> getProcesserverComputerPclinks(int psId);
        List<PSC_RATIONADVICE> getProcesserverRationAdvice(int psId);
        int SaveProcesserverComputer(PSC_PROCESCOMPUTER pProcescomputer);
        int SaveProcesserverComputerValue(PSC_PROCESCOMPUTER_VALUE pProcescomputerValue);
        int SaveProcesserverComputerPcLink(PSC_PCLINK pProcescomputerPclink);
        int SaveProcesserverRationAdvice(PSC_RATIONADVICE pRationAdvice);
        bool DeleteProcesserverComputerPclinks(int psId);
        bool DeleteProcesserverRationAdviceBySettingsId(int psId);

        List<RIGHTS_LISTS> getRightsLists(int pFarmId, int pProgramid);
        RIGHTS_LISTS getRightsListsByListName(string pListName);
        DataTable GetMilkDataStats(string ubn);
        List<int> GetMilkTanknrs(int pUbnId);
        bool InsertMilkQual(MILKQUAL pMilkQual);

        int GetAnimalCount(int pFarmId, int pMaxAniCategory);
        int GetAnimalIndexForDierkaart(string pFarmConfiganimals, int pFarmId, int pAniId, out int pPresentAnimals, out int pTotalAnimals, out int pAniIdFirst, out int pAniIdLeft, out  int pAniIdRight, out  int pAniIdLast);
        DataTable GetAnimalsForDierkaart(string pFarmConfigValueanimals, int pFarmId, int pOffset, int pLimit, string pRowFilter, string pOrderBy, int pProgID);
        DataTable GetAnimalForDierkaart(int pFarmId, int pAniId);
        DataTable GetAnimalsForGroupWeight(int pFarmId, int pMaxAnimalCategory, bool pOnlyWithWeightDate);
        DataTable GetAnimalsForVerblijfplaatsen(int pFarmId, DateTime pBegindatum, DateTime pEinddatum);
        void GetAnimalCountForDierkaart(int pFarmId, out int pPresentAnimals, out int pTotalAnimals);
        DataTable getGVAttentielijstDataTable(int pFarmId);
        DataTable getAnimalActivities(int pAniId);
        #region ATTINST
        List<ATTINST> getAttentielijstinstellingen(int pFarmId);
        bool SaveAttinst(ATTINST pAttinst);
        bool DeleteAttinst(ATTINST pAttinst);
        List<ATTCUST> getAttcustinstellingen(int pFarmid, int pInternalnr);
        bool SaveAttcust(ATTCUST pAttinst);
        bool DeleteAttcust(ATTCUST pAttinst);
        #endregion
        DataRow getDierKaartWorpGegevens(int pFarmId, int pEventId);
        DataRow getDierKaartInseminGegevens(int pFarmId, int pEventId);
        DataRow getDierKaartGrztogthGegevens(int pFarmId, int pEventId);
        DataRow getDierKaartTreatMenGegevens(int pEventId);
        DataTable getAnimalTreatMenGegevens(int pAniId);
        void getQkoortsBehandelplannummers(int pFarmID, out List<int> pQKoortsplannnenEen, out List<int> pQKoortsplannnenTwee, out List<int> pQKoortsmedicijnen);
        DataRow getDierKaartDiseaseGegevens(int pEventId);
        ANIMALCATEGORY GetAnimalCategoryByIdandFarmid(int pAnimalId, int pFarmId);
        ANIMALCATEGORY GetAnimalCategoryByIdandUbnid(int pAnimalId, int pUbnId);
        IEnumerable<ANIMALCATEGORY> GetAnimalCategoriesByIdandUbnid(int animalId, int ubnId);
        List<ANIMALCATEGORY> GetAnimalCategorysByUbnid(int pUbnId);
        List<ANIMALCATEGORY> GetAllAnimalCategorysByUbnid(int pUbnId);
        List<ANIMALCATEGORY> GetAnimalCategoryById(int pAnimalId);
        List<ANIMALCATEGORY> GetAnimalCategoryByIdProgId(int pAnimalId, int pProgId);
        List<ANIMALCATEGORY> GetAnimalCategoryByFarmId(int pFarmId);
        List<ANIMALCATEGORY> GetBullsCategoryByFarmId(int pFarmId);
        int getNumberofAnimals(int pFarmId);
        List<ANIMALPREDIKAAT> GetAnimalPredikatenByAniId(int pAniId);
        ANIMALPREDIKAAT GetAnimalPredikatenByAniIdAndBegindatum(int pPreAniId, DateTime pPreBegindatum);
        List<ANIMALPREDIKAAT> GetAnimalPredikatenByAniIdsAndBeginDates(List<int> pPreAniIds, DateTime pPreBegindatum);
        List<MOVEMENT> GetTransfers();
        DHZ GetDHZById(int InternalId);
        DHZ GetDHZByEventId(int EventId);
        List<FEEDBUY> GetFeedbuyByFarmId(int farmId);
        bool InsertFeedBuy(FEEDBUY pFeedBuy);
        bool UpdateFeedBuy(FEEDBUY pFeedBuy);
        bool DeleteFeedBuy(FEEDBUY pFeedBuy);
        DataTable getRestVoerByFarmIdAndOrAniId(int pFarmId, int pAniId);
        DataTable getFeed_AdvicesByAniId(int pAniId, DateTime pFrom, DateTime pTo, out List<FEED_ADVICE> pFeedAdvices, out List<FEED_ADVICE_DETAIL> pFeedAdviceDetails);
        bool saveFeedAdvice(FEED_ADVICE pFeedAdvice, List<FEED_ADVICE_DETAIL> pFeedAdviceDetails, int changedBy, int sourceId);
        bool deleteFeedAdvice(FEED_ADVICE pFeedAdvice, List<FEED_ADVICE_DETAIL> pFeedAdviceDetails);
        bool deleteFeedAdviceDetail(FEED_ADVICE_DETAIL pFeedAdviceDetail);
        bool deleteFeedAdviceDetails(int pAniID, List<int> pFA_IDs, int pFAD_AB_Feednr);
        DataTable getBedrijvenByProgramIds(List<int> pProgramIds, string pSearchUbnNr);
        DataTable getScrapieSteekproefBedrijven();
        DataTable getSteekproeven(int pZiekteID);
        DataTable getSteekproefUbns(int pBzs_ID, int pbzs_ZiekteID);

        bool setSteekproefThird(int pBzs_ID, int pThrId);
        bool setSteekproef(List<int> pUbnIds, int pBzs_ID);
        bool isdeelnemerBedrijfZiekteNsfo(int pUbnId, int pProgramId);
        DataTable getDeelnemersDierziekteBestrijding(int pZiekteId);
        bool CorrigerenArrArr(UBN pUbn);
        bool CorrigerenArrArr(BEDRIJF pBedrijf, bool pCorrigeerAlleLammeren);
        DataTable getBedrijfZiekteDatumVerlopen(out string pProgramConfigScrapieAddWeeks, out string pProgramConfigZwoegerziekteAddWeeks);
        DataTable getBedrijfZiekteRecentGewijzigd(int pNumberOffDaysAgo);
        DataTable getBedrijfZiekteStatussenByAniIdMovements(List<int> pZiekteIds, int pAniId);
        DataTable getBedrijfZiekteStatussenByUbnIds(List<int> pUbnIds, List<int> pZiekteIds);

        List<STORAGE> GetSilosByFarmId(int farmId);
        List<LEVNRMUT> getLevNrMuts(int pAniId);
        List<LEVNRMUT> getLevNrMutsByLifeNr(string pLNVLifenumber);
        List<MEDSTOCK> getMedstocksByUbnId(int pUbnID);
        bool saveLevNrMut(LEVNRMUT pLnm);
        List<gdauth> getGDRelatieNrStamboeken();

        bool animal_getProductieGegevens(int pAniId, int pAniSex, DateTime datum, List<int> pProgramids,
                                                         out int leeftijd, out int worpnr, out int aantLam, out int aantdoodLam, out bool jaarling, out bool pFictief);

        bool isJaarling(int pAniId);
        int animal_getMaxBirNumber(int aniId, DateTime datum);
        int animal_ram_getAantalDochtersMetWorp(int aniId);
        int ophalenLeeftijd(int pAniId);

        int ophalenLeeftijdLaatsteWorp(ANIMAL pAnimal);

        [Obsolete("VOEDERS is herschreven naar ARTIKEL")]
        List<VOEDERS> GetVoeders();
        [Obsolete("VOEDERS is herschreven naar ARTIKEL")]
        VOEDERS GetVoedersByArtikelCode(string ArtCode);
        [Obsolete("VOEDERS is herschreven naar ARTIKEL")]
        VOEDERS getFeedNameByFeedNr(int feednr);
        [Obsolete("VOEDERS is herschreven naar ARTIKEL")]
        bool InsertVoeders(VOEDERS pVoeders);
        [Obsolete("VOEDERS is herschreven naar ARTIKEL")]
        bool UpdateVoeders(VOEDERS pVoeders);
        [Obsolete("VOEDERS is herschreven naar ARTIKEL")]
        bool DeleteVoeders(VOEDERS pVoeders);

        ARTIKEL GetArtikelByEAN(string pEan);
        ARTIKEL GetArtikelByArtNumber(string pArtNumber);

        List<ARTIKEL_VOER> GetAllVoerArtikel();

        List<ARTIKEL_VOER> GetVoerArtikelByUBN(int pUbnId);
        ARTIKEL_VOER GetVoerArtikelByEAN(string pEan);
        ARTIKEL_VOER GetVoerArtikel(int pArtId);
        List<ARTIKEL> searchMedicijnArtikelenByRegNumber(string pRegNumber);
        List<ARTIKEL> searchMedicijnArtikelenByArtNaam(string pArtNaam);
        DataTable getAllMedicijnArtikelen();
        DataTable getMedicijnArtikelenByArtIds(List<int> pArtIds);
        int GetArtIdByEAN(string ean);
        ARTIKEL GetArtikelById(int pArtId);
        ARTIKEL_MEDIC GetArtikelMedicById(int pArtId);
        ARTIKEL_MEDIC_REGNR GetArtikelMedicRegnrByArtIdAndCountry(int pArtId, int pCountry);
        ARTIKEL_MEDIC_WACHTTIJD GetArtikelMedicWachttijd(int pArtId, int pDierSoort, int pWachttijdSoort, int pCountry);
        ARTIKEL_MEDIC_TOEDIENING GetArtikelMedicToediening(int pArtId, int pToedieningsWijze);
        ARTIKEL_MEDIC_WERKZAMESTOF GetArtikelMedicWerkzameStofByFidinId(int pFidinId);
        ARTIKEL_MEDIC_DOSERING GetArtikelMedicDosering(int pArtId, int pDierSoort, int pWerkzameStofId);
        ARTIKEL_MEDIC_DIERSOORT GetArtikelMedicDiersoort(int pArtId, int pDiersoort);
        ARTIKEL_MEDIC_INFO GetArtikelMedicInfo(int pArtId, int pFidinId);

        void getMedicijnArtikel(int pArtId, out ARTIKEL pArtikel, out ARTIKEL_MEDIC pArtikelMedic);

        int SaveArtikel(ARTIKEL pArtikel);
        int SaveArtikelMedic(ARTIKEL_MEDIC pArtikelMedic);
        int SaveArtikelMedicRegnr(ARTIKEL_MEDIC_REGNR pArtikelMedicRegnr);
        int SaveArtikelMedicWachttijd(ARTIKEL_MEDIC_WACHTTIJD pTblObj);
        int SaveArtikelMedicToediening(ARTIKEL_MEDIC_TOEDIENING pTblObj);
        int SaveArtikelMedicWerkzameStof(ARTIKEL_MEDIC_WERKZAMESTOF pTblObj);
        int SaveArtikelMedicDosering(ARTIKEL_MEDIC_DOSERING pTblObj);
        int SaveArtikelMedicDiersoort(ARTIKEL_MEDIC_DIERSOORT pTblObj);
        int SaveArtikelMedicInfo(ARTIKEL_MEDIC_INFO pTblObj);
        bool SaveAnimalShareInfo(ANIMAL_SHARE_INFO pAniShareInfo);
        bool SaveAnimalProduction(ANIMAL_PRODUCTION pAniProduction);
        bool DeleteAnimalProduction(ANIMAL_PRODUCTION pAniProduction);
        DataTable getArtikelMedicByUbn(int pUbnId);

        DataTable getMedicineControlListMedicines();
        DataTable getMedicineControlListMedicines(int pProgramId);
        List<ARTIKEL_MEDIC_LIST> getArtikelMedicListByProgramId(int pProgramId);

        int SaveVoerArtikel(ARTIKEL_VOER pArtikel);

        ARTIKELUBN getArticleUbn(int pUbnId, int pArtId);
        bool AddArticleUbn(ARTIKELUBN pArtikelUbn);
        bool DeleteArticleUbn(ARTIKELUBN pArtikelUbn);
        bool InsertArtikelMedicList(ARTIKEL_MEDIC_LIST pAml);
        bool DeleteArtikelMedicList(ARTIKEL_MEDIC_LIST pAml);
        List<AGRO_LABELS> GetAgroLabels(List<VSM.RUMA.CORE.DB.LABELSConst.labKind> pLabKinds, int pLabCountry, int pLabProgramId, int pLabProgId);
        List<AGRO_LABELS> GetAgroLabels(List<int> pLabKinds, int pLabCountry, int pLabProgramId, int pLabProgId);

        List<AGRO_LABELS> GetAgroLabels(VSM.RUMA.CORE.DB.LABELSConst.labKind pLabKind, int pLabCountry, int pLabProgramId, int pLabProgId);

        List<AGRO_LABELS> GetAgroLabels(int pLabKind, int pLabCountry, int pLabProgramId, int pLabProgId);
        void SaveIdealPayement(ADMINISTRATIE WILL, BEDRIJF pIDealBedrijf, ARTIKEL pArtikel, double pSupQuantity, DAGBOEK pDagboek, string pMerchantId, string pTransactionReference, THIRD pTegenpartij, double pBedrag, DateTime pBetaaldatum, string pMemoOmschrijving, string pFactuurNr);
        int getNextFactuurNummerWill(int pFarmId, int pFactDagBoekId, out string pRealNumber);
        List<BANK_TRANSACTIE> getTransActiesByTegenPartij(int pTegenpartijThrId);
        int SaveBankTransaction(BANK_TRANSACTIE pBankTransaction);
        int SaveBankBoeking(BANK_BOEKING pBankBoeking);
        int GetNewBankBoekingRegel_Regel_Nr(int pBB_ID);
        int SaveBankBoekingRegel(BANK_BOEKING_REGEL pBankBoekingRegel);

        BANK_BOEKING getBANK_BOEKINGbyBB_ID(int pBB_ID);
        List<BANK_BOEKING_REGEL> getBANK_BOEKING_REGELsByBT_ID(int pBT_ID);
        
        BANK_TRANSACTIE getBANK_TRANSACTIE(string pTransactionReference);

        List<DAGBOEK> GetAllDagboek(int pFarmId);
        List<DAGBOEK> GetDagboekBySoort(int pFarmId, int pSoort);
        List<GROOTBOEK> GetAllGrootboek(int pFarmId);

        BTWPERC GetBTWPerc(int pBtwId);
        List<BTWPERC> GetBTWPercentages(int pFarmId);
        List<FACTUUR> GetAllFactuur(int pFarmId);
        DataTable GetFactuurByFactRelatie(int pFarmId, int pFactRelatieId, List<int> pArtikelIds);
        List<FACTUUR> GetFactuurByFarmIdMonthTypeRelation(int pFarmId, DateTime pYearAndMonth, int pFactRelatieId, int pFactSoort);

        [Obsolete("oude functie voor artikelaanvoer.aspx scherm, nieuwe functie artikelaanvoer_getFactuurList()")]
        List<FACTUUR> GetAllFactuurWithArtikelWithHerkomst(int pFarmId, int pArtHerkomst);

        DataTable artikelaanvoer_getFactuurList(int pFarmId, int pThrId, int pArtHerkomst);
        DataTable artikelaanvoer_getFactuurRegel(int pFarmId, int pFactId, int pArtHerkomst);
        int artikelaanvoer_getAantalFactuurRegels(int pFactId);

        FACTUUR GetFactuur(int pFactId);
        bool deleteFactuur(int pFactId);
        int SaveFactuur(FACTUUR pFact);

        int SaveMedstock(MEDSTOCK pMEdstock);

        SUPPLY1 getSupplyByExtDbAndType(int pFarmId, VSM.RUMA.CORE.DB.LABELSConst.GUI_EXTERNAL_KIND external_kind, string pExtdb_id);
        bool SaveGeneralUniqueId(GENERAL_UNIQUE_IDS guid);


        string factuur_getNextFactNummer(int pFarmId, int pDagBoekId);

        List<SUPPLY1> GetSupplyByFactuur(int pFactId);
        List<SUPPLY1_MEDIC> GetSupplyMedicByFactuur(int pFactId);
        SUPPLY1 GetSupply(int pSupplyId);
        SUPPLY1_MEDIC GetSupplyMedic(int pSupplyId);
        int SaveSupply(SUPPLY1 pSupply);
        int SaveSupplyMedic(SUPPLY1_MEDIC pSupplyMedic);
        int getNextBoekstukNummer(int FarmId, int DagboekId);
        bool deleteSupply(int pSupplyId);
        bool deleteSupplyMedic(int pSupplyId);
        bool deleteMedstock(MEDSTOCK pMedstock);
        int getFarmIdbySupply(int pSupplyId);

        SUPPLY1_VOER_VERDELING GetSupplyVoerVerdeling(int pSupplyID, int pSVV_ProgID);
        bool SaveSupplyVoerVerdeling(SUPPLY1_VOER_VERDELING pSUPPLY1_VOER_VERDELING);

        bool SaveSupply1Voer(SUPPLY1_VOER pSUPPLY1_VOER);

        List<SCCURVE> GetSccurves(int pFarmID, int pCurveKind, int pAniKind);
        SCCURVE GetSccurve(int pCurveNr);
        int SaveSccurve(SCCURVE pSCCurve);
        /// <summary>
        /// Sets Curvnr negative, use never i.c.w deleteSCCurveD
        /// </summary>
        /// <param name="pSCCurve"></param>
        /// <returns></returns>
        bool deleteSCCurve(SCCURVE pSCCurve);

        List<SCCURVED> GetSccurveDs(int pCurveNr);
        bool SaveSccurveD(SCCURVED pSCCurveD);
        /// <summary>
        /// Deletes Single SCCURVED by Day and CurveNr, use never i.c.w. deleteSCCurve
        /// </summary>
        /// <param name="pSCCurve"></param>
        /// <returns></returns>
        bool deleteSCCurveD(SCCURVED pSCCurveD);

        List<RIGHT_MODULE> getRightModules();
        RIGHT_MODULE getRightModuleByName(string pModuleName);
        RIGHT_MODULE getRightModuleById(int pModuleId);
        DataTable getWebPageRightsTable(int pFarmID, int pModuleID);
        DataTable getWebPageRightsTableByGroupID(int pGroupID, int pModuleID);
        bool saveSupplyGroup(SUPPLY1_GROUP pSupplyGroup);
        bool deleteSupplyGroup(int pSupplyId, int pGroupId);
        SUPPLY1_GROUP getSupplyGroup(int pSupplyId, int pGroupId);
        List<SUPPLY1_GROUP> getSupplyGroups(int pSupplyId);
        List<SUPPLY1_GROUP> getSupplyGroupsByList(List<int> pSupplyIds);
        List<FSTOCK> getFstockByFarmId(int farmId);
        bool InsertFStock(FSTOCK pFstock);
        bool UpdateFStock(FSTOCK pFstock);
        bool DeleteFstock(FSTOCK pFstock);
        bool InsertFeedStc3(FEEDSTC3 pFeedStc3);
        bool UpdateFeedStc3(FEEDSTC3 pFeedStc3);
        bool DeleteFeedStc3(FEEDSTC3 pFeedStc3);
        List<FEEDSTC3> getFeedstc3ByFarmId(int farmId);
        List<LABELS> GetLabels(int pLabkind, int pLabCountry);
        LABELS GetLabel(int pLabkind, int pLabCountry, int pLabId);
        List<LABELS> getMultipleLabels(string[] pLabkinds, int pLabCountry);

        LABELS GetLabelByIdAndKindAndCountry(int pLabId, int pLabkind, int pLabCountry);

        List<EVENT> getEventsByDateAniIdKindUbn(DateTime lDatum, int AniId, int Evkind, int UBNid);

        List<EVENT> getEventsByDateAniIdKind(DateTime lDatum, int AniId, int Evkind);

        List<EVENT> GetAllEventsForAniIds(IEnumerable<int> AniIds);
        Dictionary<int, DataObject> GetAllEventDetailssForAniIds(List<EventEvekind> eventIdsAndEveKinds, bool debugEventCache = false);

        List<EVENT> getEventsByAniIdKind(int pAniId, int pEvkind);
        List<EVENT> getEventsByAniId(int pAniId);
        List<EVENT> getEventsByAniIdKindUbn(int AniId, int Evkind, int UBNid);
        List<EVENT> getEventsByKindUbn(int Evkind, int UBNid);
        List<EVENT> getEventsByAniIdUbn(int pAniId, int pUBNid);
        List<EVENT> getEventsByAniIdUbn(int pAniId, int pUBNid, DateTime EveMutationDate);
        List<EVENT> getEventsByUbn(int pUBNid);
        List<EVENT> getEventsByAniIdUbnNSFO(int pAniId, int pUBNid);

        bool eventExists(int AniId, DateTime date, VSM.RUMA.CORE.DB.LABELSConst.EventKind eveKind);

        List<EVENT> getEventsByFarmId(int pFarmId);
        List<EVENT> GetEventsByFarmId(int pFarmId, VSM.RUMA.CORE.DB.LABELSConst.EventKind pEventKind);
        DataTable getWorpenMothers(int pFarmId, int pMinDays);
        BEDRIJF GetBedrijfById(int pFarmId);
        BEDRIJF GetBedrijfByUbnIdProgIdProgramid(int pUbnId, int pProgId, int pProgramid);
        BEDRIJF GetBedrijfByUbnIdProgramid(int pUbnId, int pProgramid);
        IEnumerable<BEDRIJF> GetBedrijfByUbnIdProgId(int ubnId, int progId);
        List<BEDRIJF> getBedrijven();
        List<BEDRIJF> getBedrijvenByUBNId(int pUBNId);
        List<BEDRIJF> getBedrijvenByProgramId(int pProgramId);
        List<BEDRIJF> getBedrijvenByProgramIds(List<int> pProgramIds);
        List<BEDRIJF> getBedrijvenByProgId(int pProgId);
        List<BEDRIJF> getBedrijvenByFarmId(int pFarmId);
        List<BEDRIJF> getBedrijvenByThrId(int pThrId);
        List<BEDRIJF> getBedrijvenPositiveProgramIdWithProgId(List<int> progIds);

        BULLUBN GetBullUbn(int FarmId, int AniId);
        List<BULLUBN> GetBullUbnList(int FarmId);
        List<BULLUBN> GetBullUbnListByAniID(int pAniId);
        BUYING GetBuyingByMovId(int pMovId);
        List<BUYING> GetBuyingsByMovIds(List<int> pMovIds);
        BLOOD GetBlood(int EventID);
        BLOOD GetBlood(int EventID, int pProgId);
        BLOOD_RESEARCH GetBloodResearch(int BR_File_ID);
        BLOOD_RESEARCH_DETAIL GetBloodResearchDetail(int BRD_BloID);
        WEAN GetWean(int pEventId);
        FOKWAARDEN getFokwaarden(int pFwId);
        DataTable getFokwaardenDataTable(int pAniId, int[] fwwTypeArr, int pFw_Soort);
        List<FOKWAARDEN> getFokwaarden(int pAniId, int[] fwwType);

        List<FOKWAARDEN> getFokwaardenByAnimal(int pAniId);
        FOKWAARDEN getFokwaarden(int pAniId, int pFWKind, DateTime pFWDatum);
        List<FOKWAARDEN_WAARDEN> getFokwaardenWaardes(int pFwId);
        List<FOKWAARDEN_WAARDEN> getFokwaardenWaardes(int pFwId, DateTime pDateTime);
        FOKWAARDEN_WAARDEN getFokWaardenWaarde(int pFwId, int pFwwType);
        int saveFokwaarden(FOKWAARDEN pFw);
        bool deleteFokwaarden(int pFwId);
        bool saveFokwaardenWaarden(FOKWAARDEN_WAARDEN pFww);
        bool deleteFokwaardenWaarden(int pFwId, int pFwwType);
        MESSAGES getMessage(int pMesID);
        List<MESSAGES> getMessages();
        List<MESSAGES> getMessages(int pProgramid);
        List<MESSAGES> getMessagesForAdminUse(int pProgramid);
        EXTERIEUR getExterieur(int pExtId);
        List<EXTERIEUR> getExterieurByAnimal(int pAniId);
        EXTERIEUR getExterieur(int pAniId, int pExtKind, DateTime pExtDatum);
        List<EXTERIEUR_WAARDEN> getExterieurWaardes(int pExtId);
        List<EXTERIEUR_WAARDEN> getExterieurWaardes(List<int> pExtIds);
        EXTERIEUR_WAARDEN getExterieurWaarde(int pExtId, int pExtwType);
        EXTERIEUR_WAARDEN getLastExterieurWaarde(int pAnimalId, int pExtwType, int pExt_soort);
        DataTable getAllExterieurWaarden(int pAnimalId, int pExtwType, int pExt_soort);
        DataTable getGewichtenNsfoMetingen(List<int> pProgramIds);
        int saveExterieur(EXTERIEUR pExt);
        bool deleteExterieur(int pExtId);
        bool saveExterieurWaarden(EXTERIEUR_WAARDEN pExtw);
        bool deleteExterieurWaarden(int pExtId, int pExtwType);

        KEURING_TYPE getKeuringType(int pKtId);
        List<KEURING_TYPE> getKeuringTypes(int pProgramId);
        int saveKeuringType(KEURING_TYPE pKt);
        bool deleteKeuringType(int pKtId);
        List<KEURING_TYPE_VELDEN> getKeuringTypeVelden(int pKtId);
        bool addKeuringTypeVeld(int pKtId, int pExtwType);
        bool deleteKeuringType(int pKtId, int pExtwType);
        KEURING_LIJST getKeuringLijst(int pKlId);
        List<KEURING_LIJST> getKeuringLijsten(int pProgramId);
        List<KEURING_LIJST> getAfgewerkteKeuringsLijsten(int pProgramId);
        List<KEURING_LIJST> getOnAfgewerkteKeuringsLijsten(int pProgramId);
        int saveKeuringLijst(KEURING_LIJST pKl);
        bool deleteKeuringLijs(int pKlId);
        List<KEURING_LIJST_DIER> getKeuringLijstDieren(int pKlId);
        KEURING_LIJST_DIER getKeuringLijstDier(int pKlId, int pAniId);
        int saveKeuringLijstDier(KEURING_LIJST_DIER pKld);
        bool deleteKeuringLijstDier(int pKlId, int pAniId);
        bool deleteKeuringLijstDier(int pKlId);

        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        List<BREED> getBreeds(int AniId, int BVKindOfValue);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        BREED getBreed(int AniId, DateTime BVDate, int BVKindOfValue);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        BREEDVAL getBreedval(int AniId, DateTime BVDate, int BVKindOfValue, int BVFieldNumber);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        List<BREEDVAL> getBreedvalsByBreed(BREED lBreed);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        bool InsertBreed(BREED pBreed);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        bool InsertBreedVal(BREEDVAL pBreedval);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        bool DeleteBreed(BREED pBreed);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        bool DeleteBreedval(BREEDVAL pBreedval);
        [Obsolete("BREED + BREEDVAL is omgeschreven naar EXTERIEUR EXTERIEUR_WAARDEN FOKWAARDEN FOKWAARDEN_WAARDEN")]
        bool UpdateBreedval(BREEDVAL pBreedval);

        List<FTPINFO> GetFTPINFO(int pUBNId);

        [Obsolete("gebruik GetGroupnr")]
        GROUPID GetGroupid(int AniId);

        int GetGroupnr(int aniId, DateTime datum);

        GROUPID GetGroupid(int Groupid, int AniId);
        List<GROUPID> GetGroupids(int pGroupid, int pFarmId);
        GROUPS GetGroups(int FarmId, int Groupid);
        GROUPS GetGroupsAndlabel(int FarmId, int Groupid, int pLabCountry, out LABELS pLabels);
        List<GROUPS> GetGroupsList(int pFarmId);
        GRZTOGTH GetGRZTOGTHByEventId(int pEventId);
        List<GRZTOGTH> GetGRZTOGTHByAniIdFather(int pAniIdFather);

        UBN GetubnById(int pUBNId);
        UBN getUBNByFarmId(int pFarmId);
        long getUbnnummerHondVBK(int thrid);
        long getUbnnummerHondVirbac(int thrid);
        THIRD GetThirdByUBN(string pThrFarmNumber);
        THIRD GetThirdByKvKnr(string pThrKvKNummer);
        THIRD GetThirdByStamboeknr(string pThrStamboeknr);
        /// <summary>
        /// The combination of the descriptionId (Label with labKind 131) and registrationNr should
        /// always be unique.
        /// </summary>
        /// <param name="descriptionId">LabId For label with labKind 131</param>
        /// <param name="registrationNr">unique identifier</param>
        /// <returns></returns>
        int GetThirdIdByDescriptionIdAndRegistrationNr(int descriptionId, string registrationNr);
        List<THIRD> GetThirdsByEmailAdres(string pEmailadres);
        THIRD GetThirdByBrs_Number(string pBrs_Number);
        THIRD GetThirdBySofiNumber(string pSocialSecurity_Number);
        THIRD GetThirdByHouseNrAndZipCode(string pHouseNr, string pZipCode);
        THIRD GetThirdByAddressZIPCity(String address, String zip, String city);
        THIRD GetThirdByVatNo(String vatNumber);
        List<THIRD> GetThirdsByHouseNrAndZipCode(string pHouseNr, string pZipCode);
        THIRD GetThirdByThirId(string pTHirdId);
        THIRD GetThirdByThirId(int pTHirdId);

        THIRD_NOTES GetThirdNotesByNo_ID(int pNo_ID);


        THIRDTHIRD GetThirdThirdByThrId1AndProgramId(int pThrId1, int pProgramId);
        THIRDTHIRD GetThirdThirdByTtId(int pTtId);
        bool DeleteThirdThird(THIRDTHIRD pThirdThird);

        bool DeleteThirdNotes(THIRD_NOTES pThirdNotes);



        List<THIRD> GetThirdsByAdMinProgramId(int pProgramId);
        THIRD GetOwnerForAnimal(int pAniId, int pProgID);
        BEDRIJF getFarmOwnerForAnimal(int pAniId);

        List<THIRD> GetThridByByUBNAndKind(int pUbnId, RUMA.CORE.DB.LABELSConst.relatieKind pRelatieKind);
        List<THIRD> GetThridByByFarmIDAndKind(int pFarmID, int pRelatieKind);
        List<THIRD> GetThirdByKind(int pFarmID, RUMA.CORE.DB.LABELSConst.relatieKind pRelatieKind);
        List<THIRD> GetThirdByKind(RUMA.CORE.DB.LABELSConst.relatieKind pRelatieKind);
        List<THIRD> GetThirdByKinds(List<int> pThirdKinds);
        List<THIRD> getThirdsByUBNid(string pUbnId);
        List<THIRD> GetMedeEigenaren(int pAniId);
        int GetAantalMedeEigenaren(int pAniId);
        List<THIRD> searchTHIRDS(string pNaamThird);
        List<THIRD> searchThirdsByCompanyAndCity(string pCompany, string pCity);
        List<THIRD> GetThirdsByThirdIds(List<int> pThirdIds);
        THIRDUBN GetThirdUBN(string pTHirdId, string pUbnId);
        List<THIRDUBN> getThirdUBNs(int pUbnId);
        THIRDKIN GetThirdKin(int pTHirdId, int pThkDescriptionId, int pFarmId);
        List<THIRDKIN> GetThirdKinds(int pGebruikersFarmId, int pTHirdId);
        TRANSPRT GetTransprt(int pTransportID);
        void getTransPorteurAndMiddel(int pTransporterThrId, out THIRD pTransporteur, out List<TRANSPRT> pLTransprt);
        List<TRANSPRT> GetTransprts(int pTransporter);
        List<TRANSPRT> GetAllTransprts();
        DISEASE GetDisease(int AniId, int EventID);
        BIRTH GetBirth(int EventID);
        BIRTH GetBirthByCalfId(int pCalfId);
        List<BIRTH> CheckBirthsByCalfId(int pCalfId);
        List<BIRTH> GetBirthsByFather(int pFatherAniId, out List<EVENT> pEvents);
        List<BIRTH> GetBirthsByAnimal(int pAniId);
        List<BIRTH> GetBirthsByUbnID(int pUbnID);
        BUYING GetBuying(int MovId);
        INHEAT GetInHeat(int EventId);
        INSEMIN GetInsemin(int EventID);
        List<INSEMIN> GetInseminByAniIdFather(int pAniIdFather);
        GESTATIO GetGestatio(int EventID);
        DRYOFF GetDryoff(int EventID);
        bool SaveDryoffDays(DRYOFFDAYS pDryoffDays);
        DRYOFFDAYS GetDryoffDays(int pUbnID, int pBCS, int pLactationnr);
        bool DeleteDryoffDays(DRYOFFDAYS pDryoffDays);
        List<DRYOFFDAYS> GetDryoffDaysByUbn(int pUbnID);
        LOSS GetLoss(int MovId);
        List<LOSS> GetLosses(List<int> pMovIds);
        List<SUPPLY1> getLeverancierSupplylist(int pFactRelatieId, int pYear);


        List<FACTUUR> getLeverancierFactuurlist(int pFactRelatieId, int pYear);
        LIFENR GetLifenrByLifenr(int FarmId, string Lifenr);
        LIFENR GetLifenrByLifenrOwnerThrID(int pOwner_ThrID, string Lifenr);
        List<LIFENR> GetLifenummersBy_owner_ThrID(int pOwner_ThrID);

        List<LIFENR> GetLifenummersByFarmId(int pFarmId);
        REMARK getRemarkByEverything(string pFarmId, string pLabKind, string pLabId, string pRemId);
        List<REMARK> getFarmRemarks(int pFarmid, int pLabkind, int pLabId);
        List<REMARK> getFarmRemarks(int pFarmid);
        List<REMARK> getFarmRemarksByUbnId(int ubnid);
        SALE GetSale(int MovId);
        List<SALE> GetSales(List<int> pMovIds);
        SCORE getScore(int AniId, DateTime pScDate);
        SECONRAC GetSeconRacByKey(int pAnimalId, int RacId);
        List<SECONRAC> GetSeconRacSByAnimalId(int pAnimalId);
        List<SECONRAC> GetSeconRacSByAnimalIds(List<int> pAnimalIds);
        STATUS GetStatus(int EventID);
        TRANSPLA GetTranspla(int EventID);
        TAKEEMBR getTakeEmbr(int pEventId);
        TREATMEN GetTreatmen(int EventID);
        void GetTreatmenByTreMedPlannr(int aniid, int tremedplannr, out EVENT pevent, out TREATMEN treatmen);
        List<TRANSMIT> GetTransmitByAniIdFarmId(int pFarmId, int pUbnID, int pAniId);
        List<TRANSMIT> GetTransmitByAniIdFarmId2(int pFarmId, int pUbnID, int pAniId);
        //List<TRANSMIT> GetTransmitByAniIdFarmId(int pFarmId, int pAniId);
        [Obsolete("Use DBSQResponders ")]
        List<TRANSMIT> GetTransmitsByFarmId(int pFarmId);

        List<TRANSMIT> GetTransmitsByUbnId(int ubnId);

        //DataTable getProcessCompIds(int pFarmId, int pUBNId, int pProgId);
        MEDICINE GetMedicineByMedId(int MedId);
        MEDICINE getMedicineByMedCode(string MedCode);
        MEDICINE getMedicineByMedName(string pMedName);
        List<MEDICINE> GetAllMedicines();
        List<MEDICINE> GetMedicinesByMedIds(List<int> pMedIds);
        List<MEDICINE> GetMedicinesByUbn(int pUBNId);
        MEDICINEUBN GetMedicineUbn(int MedId, int UBNId);
        List<MEDICINEUBN> GetMedicineUbns(int pUbnId);
        List<MEDPLANM> GetMedPlanMMen(int pFarmId);
        List<MEDPLANM> GetMedPlanMMen(int pFarmId, bool pIndividueel);
        MEDPLANM GetMedPlanM(int pInternalnr);
        MEDPLANM GetMedPlanM(int pFarmID, int pPlanNummer);
        MEDPLANM GetMedPlanM(int pFarmID, int pPlanNummer, int pPlanSoort);
        List<MEDPLAND> GetMedPlanDDen(string pInternalnr);
        MEDPLAND GetMedPlanD(int pInternalnr, int MedId);

        MESTNR getMestnr(string pMestnummer);
        List<MESTNR> getMestnrsByUbn(List<string> pBedrijfsnummers);
        MESTTANK getMestTank(string pMestnummer, int pTanknummer);
        List<MESTTANK> getMestTanks(string pMestnummer);
        MESTUBN getMestUbn(string pMestnummer, string pBedrijfsnummer);
        List<MESTUBN> getMestUbns(string pMestnummer);

        MOVEMENT GetMovementByDateAniIdKind(DateTime Date, int AniId, int MovKind, int UbnId);
        MOVEMENT GetMovementByMovId(int MovId);
        MOVEOUT getMoveOut(int pMovId);
        List<MOVEMENT> getMovementsByBedrijf(BEDRIJF pBedrijf);
        List<MOVEMENT> getMovementsByMovThird_UbnAndKind(int pMovThird_UBNid, int MovKind);

        List<MOVEMENT> GetMovements(int pAniId, int pMovKind);
        List<MOVEMENT> GetMovements(List<int> pMovIds);
        List<MOVEMENT> GetMovementsByUbn(int pAniId, int pUbnId);
        List<MOVEMENT> GetMovementsByAniIdMovkindUbn(int pAniId, int pMovKind, int pUbnId);
        List<MOVEMENT> GetMovementsByMovKindUbn(int pMovKind, BEDRIJF pBedrijf);
        DataTable GetLocatiesGroupedByDate(BEDRIJF pBedrijf);
        DataTable GetPlaces(int ubnid);
        DataTable GetCurrentPlaceByAniId(BEDRIJF pBedrijf, int pAniId);
        DataTable getHokkenByUbnId(int pUbnId);
        int DeleteHokkenByDateAndCageNr(BEDRIJF pBedrijf, DateTime pMovDate, int pCageNr);
        int DeleteHokkenByDateAndCageNr(BEDRIJF pBedrijf, DateTime pMovDate, string pCageNr);
        List<MOVEMENT> GetMovementsByAniId(int pAniId);
        List<MOVEMENT> GetMovementsOnlyByUbn(int pUbnId);
        List<MOVEMENT> GetMovementsByDateKind(int pMovKind, int pUbnId, int pProgId, DateTime pDate);
        void getMovsAndBuyingsByGroep(int pFarmId, int pAanvoergroepnr, out List<MOVEMENT> pAanvoerMovs, out List<BUYING> pAanvoerBuys);
        MUTATION GetMutationById(int InternalId);
        List<MUTATION> GetMutationsByUbn(int pUbnId);
        MUTATION GetMutationByEventId(int pEventId);
        MUTATION GetMutationByMovId(int pMovId);
        MUTALOG GetMutaLogById(int InternalId);
        MUTALOG GetMutaLogByEventId(int pEventId);
        MUTALOG GetMutaLogByMovId(int pMovId);
        MUTALOG GetMutaLogByLifeNumberMeldingNr(string pLifeNumber, string pMeldingNummer);
        List<MUTALOG> GetMutaLogsByUbn(int pUbnId);
        List<MUTALOG> GetMutaLogsByLifeNumber(string pLifeNumber);
        List<MUTATION> GetMutationsByLifeNumber(string pLifeNumber);
        DataTable getMutaVoeren(int pLeverancierThrID, DateTime pYear);
        bool deleteMutaVoer(MUTA_VOER pMutaVoer);
        bool insertMutaVoer(MUTA_VOER pMutaVoer);
        bool updateMutaVoer(MUTA_VOER pMutaVoer);
        List<MUTA_VOER_LOG> getMutaVoerenLoggen(int pLeverancierThrID, DateTime pYear);
        bool insertMutaVoerLog(MUTA_VOER_LOG pMutaVoerLog);
        bool deleteMutaVoerLog(MUTA_VOER_LOG pMutaVoerLog);
        List<int> getNlingCheckEventIds(int pUBNId, int pAniId, int pBirNumber);
        PLACE GetPlace(int MovId);
        [Obsolete("Use List<EVENT> GetEventsByDateAniIdKind ")] 
        EVENT GetEventByDateAniIdKind(DateTime Date, int AniId, int EveKind);
        [Obsolete("Use List<EVENT> GetEventsByDateAniIdKindInteval ")]
        EVENT GetEventByDateAniIdKindInteval(DateTime Date, int AniId, int EveKind, int Interval);
        List<EVENT> GetEventsByDateAniIdKindInteval(DateTime Date, int AniId, int EveKind, int Interval);
        [Obsolete("GetDataTable kan naar de verkeerde database wijzen! herschrijf de functie")]
        System.Data.DataTable GetDataTable(StringBuilder pQuery);
        DataTable GetExtraDataTable(int thrid, string connectionstring, string query, MissingSchemaAction MissingSchemaAction);
        bool Afkalven(DateTime pDatum, int pAantalKalveren, int pLactatie, ANIMAL pAniMother, int UBNid, int pFarmId);
        int GetUBNidbyUBN(String pUBN);
        int getUBNidAndProgIdByUBNNr(string UBNNr, out int ProgId);
        UBN getUBNByBedrijfsnummer(String pBedrijfsnummer);
        UBN getUBNByBedrijfsnummer(String pBedrijfsnummer, out THIRD pThird);
        [Obsolete("THIRD can return more then one UBN entry! for the moment function returns first result; use getUBNsByThirdID instead")]
        UBN getUBNByThirdID(int pThrID);
        string getBedrijfsnummer(int pUbnId);
        List<COUNTRY> GetAllCountries();
        COUNTRY GetCountryByLandid(int landid);
        COUNTRY GetCountryByLandNummer(int pLandNummer);
        IEnumerable<UBN> GetUBNsForLORAReportGen(DateTime date);
        bool StoreLORAReport(LORA_GRAZINGREPORT report);
        bool UpdateLORAReportDate(int mrID, DateTime date);
        List<UBN> getUBNsByThirdID(int pThrID);
        List<string> getUBNnummersByThirdID(int pThrID);
        List<UBN> getUBNsByThirdIDs(List<int> thirdids);
        List<UBN> getUBNsByProgramId(int pProgramId);
        List<UBN> GetUBNsByProgramId(List<int> pProgramIdList);
        List<UBN> getUBNsByProgId(int pProgId);
        EVENT GetEventdByEventId(int EventId);
        List<WEIGHT> getWeights(int pAniId);
        List<WEIGHT> getWeights(List<int> pAniIds);
        double getGemiddeldGewicht(int pProgramId);
        WEIGHT getWeight(int AniId, DateTime Datum);
        DataTable getFarmWeights(int pFarmId, int pProgramId);
        DataTable getFarmWeightsDeviation(int pProgramId);
        WGCURVE GetWgcurve(int pCurvenr);
        List<WGCURVE> GetWgcurveListByFarmId(int pFarmId);
        WGCURVED GetWgcurveD(int pCurvenr, int pfd_Day);
        List<WGCURVED> GetWgcurveDList(int pCurvenr);
        WGCURVEFARM getWGCurveFarm(int pCurvenr, int pFarmId);
        List<WGCURVEFARM> getWGCurveFarms(int pFarmId);
        VKIDIERMED GetVKIDierMedByAniIdInternalnr(int pAniId, int pInternalNr);
        bool MutLogMeldingIntrekken(MUTALOG pMutLog);

        int SaveAnimal(ANIMAL a, int sourceId = 0, LABELSConst.ChangedBy changedBy = LABELSConst.ChangedBy.UNKNOWN);

        [System.Obsolete("SaveAnimal is deprecated, gebruik SaveAnimal met ChangedBy en SourceId.", true)]
        int SaveAnimal(ANIMAL a);

        bool SaveAnimal(int pThrId, ANIMAL pAnimal);
        bool SaveAnimalCategory(ANIMALCATEGORY pAnimalCat);
        bool ReplaceAnimalCategory(ANIMALCATEGORY ac);
        bool SavePaddedAnimalCategory(ANIMALCATEGORY ac);
        bool SaveAnimalPredikaat(ANIMALPREDIKAAT pPreAnipredikaat);
        bool SaveBedrijf(BEDRIJF pBedrijf);
        bool SaveBedrprod(BEDRPROD pBedrprod);
        bool SaveBirth(BIRTH pBirth);
        bool SaveBlood(BLOOD pBlood);
        bool SaveBlood(BLOOD pBlood, int pProgId);
        bool SaveBloodResearch(BLOOD_RESEARCH pBloodResearch);
        bool SaveBloodResearchDetail(BLOOD_RESEARCH_DETAIL pObj);
        bool saveBullUbn(BULLUBN pBullUbn);
        bool SaveBulls(ANIMAL pBull);
        bool SaveBuying(BUYING pBuying);
        bool SaveComplaint(COMPLAINTS pComplaint);
        bool SaveDisease(DISEASE pDiss);
        bool SaveDryoff(DRYOFF pDryoff);
        bool InsertDHZ(DHZ pDHZ);
        bool InsertDHZLog(DHZLOG pDHZLog);
        bool SaveEvent(EVENT pEvent);
        bool SaveEvent(EVENT pEvent, int pProgId);
        bool SaveGestation(GESTATIO pGestation);
        bool SaveGroupId(GROUPID pGrid);
        bool SaveGroups(GROUPS pGroup);
        bool SaveGRZTOGTH(GRZTOGTH pGrztogth);
        bool SaveInHeat(INHEAT pInHeat);
        bool SaveInsemin(INSEMIN pInsemin);
        bool SaveLoss(LOSS pLoss);
        bool SaveMovement(MOVEMENT pMovement);
        bool SaveMovementDetail(DataObject detail);
        bool SaveMoveOut(MOVEOUT pMoveout);
        bool saveMedicineUbn(MEDICINEUBN pMedUbn);
        bool SaveMedplanM(MEDPLANM pMedplandM);
        bool SaveMedplanD(MEDPLAND pMedplandD);
        bool SaveMessage(MESSAGES pMessage);
        bool SaveMutation(MUTATION pMutation);
        bool InsertMutLog(MUTALOG pMutLog);
        bool savePlace(PLACE pPlace);
        bool SaveRemark(REMARK pRemark);
        bool SaveSale(SALE pSale);
        bool SaveScore(SCORE pScore);
        bool SaveSeconRace(SECONRAC pSeconRace);
        bool SaveStatus(STATUS pStatus);
        bool SaveTranspla(TRANSPLA pTranspla);
        bool SaveTakeEmbr(TAKEEMBR pTakeEmbr);
        bool SaveTransprt(TRANSPRT pTransprt);
        bool SaveTransMit(TRANSMIT pTransMit);
        bool SaveTransMStock(TRANSMSTOCK pTransMStock);
        bool SaveThird(THIRD pThird);
        //bool SaveThirdThird(THIRDTHIRD pThirdThird); // NIET MEER GEBRUIKEN (rikkert)
        bool SaveSupplyDetails(SUPPLY1_DETAILS pSupDetails);
        bool SaveWean(WEAN pWeaner);
        bool SaveWGCurve(WGCURVE pWgcurve);
        bool SaveWGCurveD(WGCURVED pWgcurveD);
        bool InsertAuthGroupsFarm(AUTH_GROUPS_FARM pAuthGroupsFarm);
        bool DeleteThird(THIRD pThird);
        bool InsertAnimalAfwijking(int pThrId, ANIMAL_AFWIJKING pAnimalAfwijking);
        bool InsertAnimalPredikaten(List<ANIMALPREDIKAAT> pAnimalpredikaten);
        bool InsertThirdUbn(THIRDUBN pThirdUbn);
        bool InsertThirdKin(THIRDKIN pThirdKin);
        bool InsertLifenr(LIFENR pLifenr);

        bool InsertMestnr(MESTNR pMestnummer);
        bool InsertMestTank(MESTTANK pMesttank);
        bool InsertMestUbn(MESTUBN pMestUbn);
        bool InsertReportLog(REPORT_LOGGING pReportLog);
        bool InsertStorage(STORAGE pStorage);
        bool UpdateStorage(STORAGE pStorage);
        bool DeleteStorage(STORAGE pStorage);
        bool UpdateAuth_Groups(AUTH_GROUPS pAuthGroups);
        bool DeleteAuthGroupsRights(AUTH_GROUPS_RIGHTS pAuthGroupsRights);
        bool InsertVKIINFO(VKIINFO pVkiInfo);
        bool UpdateVKIINFO(VKIINFO pVkiInfo);
        bool DeleteVKIINFO(VKIINFO pVkiInfo);

        bool InsertVKIDIER(VKIDIER pVkiDier);
        bool UpdateVKIDIER(VKIDIER pVkiDier);
        bool DeleteVKIDIER(VKIDIER pVkiDier);

        bool InsertVKIDIERMED(VKIDIERMED pVkiDierMed);
        bool UpdateVKIDIERMED(VKIDIERMED pVkiDierMed);
        bool DeleteVKIDIERMED(VKIDIERMED pVkiDierMed);

        List<VKIDIER> GetVKIDierenByInternalNr(int internalNr);
        VKIDIER GetVkiDierByAniId(int AniId);
        List<VKIINFO> GetAllVKIInfoByFarmNumber(string FarmNumber);
        VKIINFO GetVKIInfoByInternalNr(int InternalNr);
        List<VKIDIER> GetAllVKIDieren();
        List<TREATMEN> getAllUbnTreatmen(int pUbnId, out List<EVENT> ptreatmenEvents);
        List<STATUS> getAllUbnStatussen(int pUbnId);
        bool SaveAnalyse(ANALYSE pAnalyse);
        [Obsolete("SaveInlog is een verouderde functie", true)]
        bool SaveInlog(String pUsername, String pPassword, DBConnectionToken pToken);
        bool UpdateFokstier(ANIMAL pBull);
        bool SaveWeight(WEIGHT pGew);
        bool InsertWgcurvedFarm(WGCURVEFARM pWgcurvedFarm);
        bool DeleteAuthGroupsFarm(AUTH_GROUPS_FARM pAuthGroupsFarm);
        bool DeleteAnalyse(ANALYSE pAnalyse);
        bool DeleteAnimal(ANIMAL pAnimal);
        bool DeleteAnimalShareInfo(ANIMAL_SHARE_INFO pAnimalShareInfo);
        bool DeleteAnimalPassword(ANIMALPASSWORD pAnimalPassWord);
        bool DeleteAnimalCategory(ANIMALCATEGORY pAnimalCategory);
        bool DeleteAnimalPredikaat(ANIMALPREDIKAAT pAnimalPredikaat);
        bool DeleteAnimalAfwijking(ANIMAL_AFWIJKING pAnimalAfwijking);
        bool DeleteAnimalAfwijkingen(int pAniId, int aa_type);
        bool DeleteBedrijf(BEDRIJF pBedrijf);
        bool DeleteBirth(BIRTH pBirth);
        bool DeleteBlood(BLOOD pBlood);
        bool DeleteBullUbn(BULLUBN pBullUbn);
        bool DeleteBuying(BUYING pBuy);
        bool DeleteDisease(DISEASE pDisease);
        bool DeleteDHZ(DHZ pDHZ);
        bool DeleteDryoff(DRYOFF pDryoff);
        bool DeleteEvent(EVENT pEvent);
        bool DeleteGestatio(GESTATIO pGestatio);
        bool DeleteGroupId(GROUPID pGrid);
        bool DeleteGroups(GROUPS pGroup);
        bool DeleteGRZTOGTH(GRZTOGTH pGrztogth);
        bool DeleteInheat(INHEAT pInHeat);
        bool DeleteInsemin(INSEMIN pInsem);
        bool DeleteLoss(LOSS pLoss);
        bool DeleteLifenrByOwner_ThrID(int pOwner_ThrID, LIFENR pLifenr);
        bool DeleteLifenr(int pFarmId, LIFENR pLifenr);
        bool DeleteLifenr(int pFarmId, String pFullLifeNr);
        bool DeleteLifeNumbersByFarmId(int pFarmId, int pProgId);
        bool DeleteLifeNumbersByOwner_ThrID(int pOwner_ThrID);
        bool DeleteMovement(MOVEMENT pMovement);
        bool DeleteMoveout(MOVEOUT pMoveout);
        bool DeleteMedplanM(MEDPLANM pMedplandM);
        bool DeleteMedplanD(MEDPLAND pMedplandD);
        bool DeleteMedicineUbn(MEDICINEUBN pMedUbn);
        bool DeleteMestnr(MESTNR pMestnummer);
        bool DeleteMestTank(MESTTANK pMesttank);
        bool DeleteMestUbn(MESTUBN pMestUbn);
        bool DeleteMessage(MESSAGES pMessage);
        bool DeleteMutation(MUTATION pMutation);
        bool DeleteMutalog(MUTALOG pMutalog);
        bool DeletePlace(PLACE pPlace);
        bool DeleteRemark(REMARK pRemark);
        bool DeleteSale(SALE pSale);
        bool DeleteScore(SCORE pScore);
        bool DeleteSeconRace(SECONRAC pSeconRace);
        bool DeleteStatus(STATUS pStatus);
        bool DeleteTranspla(TRANSPLA pTranspla);
        bool DeleteTakeEmbr(TAKEEMBR pTakeEmbr);
        bool DeleteTransmit(TRANSMIT pTransMit, int changedBy = 0, int sourceId = 0);
        bool DeleteTransmStockNumbers(int pUbnID, int pProcesComputerId_Koppelnr, List<string> pTransmitNumbers);
        bool DeleteTransmStock(TRANSMSTOCK pTransMStock);
        bool DeleteTransprt(TRANSPRT pTransprt);
        bool DeleteThirdUbn(THIRDUBN pThirdUbn);
        bool DeleteThirdKin(THIRDKIN pThirdKin);
        bool DeleteTreatmen(TREATMEN pTreatmen);
        bool DeleteWeight(WEIGHT pGew);
        bool DeleteUBN(UBN pUbn);
        bool DeleteWean(WEAN pWeaner);
        bool DeleteWgcurve(WGCURVE pWgcurve);
        bool DeleteWgcurveD(WGCURVED pWgcurveD);
        bool DeleteWgcurveFarm(WGCURVEFARM pWgcurveFarm);

        bool SaveThirdNotes(THIRD_NOTES pThirdNotes);

        [Obsolete("RUMALOG niet gebruiken schrijf het in SoapLog", false)]
        void WriteLogMessage(int UbnId, int LogKind, String LogMessage);
        void WriteSoapError(SOAPLOG pSoapLog);
        List<SOAPLOG> GetSoaplogs(int pFarmId);
        List<SOAPLOG> GetSoaplogsbyUBNandDate(String FarmNumber, DateTime Begindatum, DateTime Einddatum);
        String Plugin();

        FTPLIST GetFtpListByFtpnumber(int pFtpnumber);
        FTPUSER GetFtpuser(int pUbnId, int pFtpnumber);
        /// <summary>
        /// zie BUG 2039 
        /// CAUTION: NIET !! GEBRUIKEN VOOR bijv algemene instellingen of Sanitrace
        /// </summary>
        /// <param name="pUbnId"></param>
        /// <param name="pProgramId"></param>
        /// <param name="pProgID"></param>
        /// <param name="pFtpnumber"></param>
        /// <returns> returns possible admin credentials when pFtpNumber == 9992 </returns>
        FTPUSER GetFtpuser(int pUbnId, int pProgramId, int pProgID, int pFtpnumber);
        FTPUSER GetFtpuser(int pUbnId, int pProgramId, int pProgID, int pFtpnumber, out string pBrsNummer);
        ANIMALPASSWORD GetAnimalPassword(int pAP_AniId, int pAP_ThrID, int pProgramId);
        //ANIMALPASSWORD GetAnimalPasswordByUserNameAndPassword(string pAP_UserName, string pAP_Password);       
        ANIMALPASSWORD GetAnimalPasswordByUserNameAndPassword(string pAP_UserName, string pAP_Password, int pProgramId);
        List<ANIMALPASSWORD> GetAnimalPasswordsByAniId(int pAp_AniId);
        List<ANIMALPASSWORD> GetAnimalPasswordsByThrID(int pAP_ThrID, int pProgramId);

        // nieuwe
        List<ANIMALPASSWORD> GetAnimalPasswordsByUserNameAndProgramId(string pAP_UserName, int pProgramId);
        List<ANIMALPASSWORD> GetAnimalPasswordsByUserNameAndPasswordAndProgramId(string pAP_UserName, string pAP_Password, int pProgramId);
        List<ANIMALPASSWORD> GetAnimalPasswordsByEmailadresAndProgramId(string pEmailadres, int pProgramId);

        List<FTPUSER> GetFtpusers(int pUbnId, List<int> pFtpnumbers);

        THIRD_LOGIN GetThirdLogin(int ThrID, int ThrSoort);
        THIRD_LOGIN GetThirdLogin(int farmId, int ubnId, int ThrID, int thkDescriptionId, int programId, int ThrSoort);

        THIRD_LOGIN_TYPE GetThirdLoginType(int pThrID, string pThr_IP_Adress);
        List<THIRD_LOGIN_TYPE> GetThirdLoginTypeByThrID(int pThrID);
        List<THIRD_LOGIN_TYPE> GetThirdLoginTypeByIP_Adress(string pThr_IP_Adress);
        FTPACTIO GetFtpActioByActionNumber(int pActionNumber);
        BEDRPROD getBedrProd(int UbnId, DateTime DatumProduktie);

        bool SaveFtpActio(FTPACTIO pFtpactio);
        bool SaveFtpuser(FTPUSER pFtpuser);
        bool SaveAnimalPassword(ANIMALPASSWORD pAnimalPassword, int pOldThirdId);
        bool SaveThirdLogin(THIRD_LOGIN pThirdLogin);
        bool SaveMedicine(MEDICINE pMed);
        bool SaveTreatmen(TREATMEN pTreatm);
        bool SaveUbn(UBN pUbn);
        bool UpdatePartialANIMAL(ANIMAL pAnimal, String[] pUpdateParams);
        bool UpdateANIMALFokker(int AniId, int thrId, int UbnId, int ChangedBy, int SourceId);
        bool UpdateANIMAL(int pThrId, ANIMAL pAnimal);
        bool UpdateANIMALcomment(ANIMAL pAnimal);
        bool UpdateBEDRIJF(BEDRIJF pBedrijf);
        bool UpdateBuying(BUYING bui);
        bool UpdateDisease(DISEASE pDiss);
        bool UpdateDHZ(DHZ pDHZ);
        bool UpdateEVENT(EVENT pEvent);
        bool UpdateFtpActio(FTPACTIO pFtpactio);
        bool UpdateFtplist(FTPLIST pFtplist);
        bool UpdateFtpuser(FTPUSER pFtpuser);
        bool UpdateThirdLogin(THIRD_LOGIN pThirdLogin);
        bool UpdateStatus(STATUS pStatus);
        bool UpdateThirdOld(THIRD pThird);
        bool UpdateThrCreatorUbnIdThrID(THIRD pThird);
        bool UpdateTreatmen(TREATMEN pTreatm);
        bool UpdateMEDICINE(MEDICINE pMed);
        bool UpdateMovement(MOVEMENT mv);
        bool UpdateMutationReport(MUTATION pMutation);
        bool UpdateMutation(MUTATION pMutation);
        bool UpdateUBN(UBN pUbn);

        bool saveBedrijf_ZiekteBesmetting(int pThrId, BEDRIJF_ZIEKTE pBedrijfsZiekte);
        BEDRIJF_ZIEKTE GetBedrijfZiekte(int pBzId);
        int saveBedrijf_ZiekteBesmettingInt(int pThrId, BEDRIJF_ZIEKTE pBedrijfsZiekte);
        List<BEDRIJF_ZIEKTE> getCurrentBedrijfZiekteStatus(int pFarmId);
        BEDRIJF_ZIEKTE getCurrentBedrijfZiekteStatus(int pFarmId, int pZiekteId);
        bool deleteBedrijf_Ziekte(int pThrId, int pBzId);

        bool addDierziekte(int pThrId, DIER_ZIEKTE dz, int pProgID);
        bool deleteDierziekte(int pThrId, int dz_Id, int pProgID);

        List<DIER_ZIEKTE> getCurrentDierZiekteStatus(int pAniId, int pProgID);


        #region aanwezige dieren

        //*** LET OP! houdt deze up-to-date met Delphi
        //(Z:\prog\DELPHI\RUMAMYSQLDB\rdbtools.pas)

        bool rdDierAanwezig(int aniId, int farmId, int pUbnId, DateTime datum, int groupId, int mede_eigenaar, int pProgId);
        int rdCountDierAanwezig(int farmId, int pUbnId, DateTime datum, int groupId, int mede_eigenaar, int pProgId);
        List<int> rdAanwezigeDieren(int farmId, int pUbnId, DateTime datum, int groupId, int mede_eigenaar, int pProgId);

        DataTable getDierenAanwezigInPeriode_minascategorie(int pFarmId, DateTime pBegindate, DateTime pEnddate);

        #endregion

        DataTable getForFarmersLaatsteWijzigingen(int pInterval);
        DataTable getVoerregistratieLaatsteWijzigingen(int pInterval, List<int> pProgramIds);
        DateTime getMinAanvoerDatum(int farmId, int groupId);

        DataTable getGroepsGewijzeDeklijstSchaap(int pFarmId, int pLimit, int pOffset, string pSort);
        DataTable getDekkingEventLijstDekperiode(int pFarmId, DateTime pBegindatum, DateTime pEindDatum, int pProgId);
        DataTable getDekrammenByFarmId(int pFarmid);

        //farmconfig - oud       
        List<FARMCONFIG> getFarmConfigsByKey(string pKey);
        List<FARMCONFIG> getFarmConfigsByStartKey(int pFarmID, string pKey);
        List<FARMCONFIG> getFarmConfigs(int pFarmId);
   

        bool DeleteFarmConfig(FARMCONFIG pFConfig);

        [Obsolete("GEBRUIK GetFarmConfigValue, zodat deze weg kan!")]
        FARMCONFIG getFarmConfig(int pFarmId, string pKey, string pDefaultValue);
        [Obsolete("GEBRUIK GetFarmConfigValue, zodat deze weg kan!")]
        FARMCONFIG getFarmConfig(int pFarmId, string pKey);

        //farmconfig
        string GetFarmConfigValue(int pFarmId, string pFKey, string pDefaultValue);
        string GetFarmConfigValue(int pFarmId, string pFKey);
        void SetFarmConfigValue(int pFarmId, string pFKey, string pFValue);
        void SetFarmConfigValue(int pFarmId, string pFKey, string pFValue, bool pEncrypt);

        IEnumerable<FARMCONFIG> GetFarmConfigValueByKeyForUBN(string pKey, int ubnId);



        //programconfig
        string GetProgramConfigValue(int pProgramId, string pFKey, string pDefaultValue);
        string GetProgramConfigValue(int pProgramId, string pFKey);
        void SetProgramConfigValue(int pProgramId, string pFKey, string pFValue);
        List<PROGRAMCONFIG> getProgramConfigs(int pProgramId);
        List<PROGRAMCONFIG> getProgramConfigsByKeyAndValue(string pKey, string pValue);
        List<PROGRAMCONFIG> GetProgramConfigsAdminCredentials(int pProgramId, out string pUsername, out string pPassword, out string pBrsnummer);
        void getStandaardKetoConfigWaardes(int pProgramId, int pFarmId, int pAniId, out int ketodagen, out int ketodagendubbel, out double ketoml);
        IEnumerable<PROGRAMCONFIG> GetProgramConfigValueByKeyForUBN(string pKey, int ubnId, int? filterOnProgId = null);

        //animal_property
        List<ANIMAL_PROPERTY> getAnimalPropertys(int pFarmID, int pAniID);
        List<ANIMAL_PROPERTY> getAnimalPropertys(int pFarmID, List<string> pAP_Keys);
        string GetAnimalPropertyValue(int pFarmID, int pAniID, string pAP_Key, string pDefaultValue);
        string GetAnimalPropertyValue(int pFarmID, int pAniID, string pAP_Key);
        string GetAnimalPropertyValue(int pAniID, string pAP_Key);
        void SetAnimalPropertyValue(int pUbnId, int pFarmID, int pAniID, string pAP_Key, string pAP_Value);
        bool DeleteAnimalProperty(ANIMAL_PROPERTY pObj);
        //groupconfig
        string GetGroupConfigValue(int pFarmId, int pGroupId, string pFKey, string pDefaultValue);
        string GetGroupConfigValue(int pFarmId, int pGroupId, string pFKey);
        string GetGroupConfigValue(int pProgramId, int pFarmId, int pGroupId, string pFKey, string pDefaultValue);

        void SetGroupConfigValue(int pFarmId, int pGroupId, string pFKey, string pFValue);

        //artikel_bedrijf_config
        string GetArtikelBedrijfConfigValue(int pFarmId, int pArtId, string pFKey, string pDefaultValue);
        string GetArtikelBedrijfConfigValue(int pFarmId, int pArtId, string pFKey);
        void SetArtikelBedrijfConfigValue(int pFarmId, int pArtId, string pFKey, string pFValue);

        //config-algemeen
        string GetConfigValue(int pProgramId, int pFarmId, string pFKey, string pDefaultValue);
        string GetConfigValue(int pProgramId, int pFarmId, string pFKey);

        double getArtVastePrijs(int farmId, int artId);

        string intListToString(List<int> intList);
        string stringListToString(List<string> stringList);

        string intArrToString(int[] intArr);

        List<int> getFarmIdsByProgramId(int pProgramId);
        List<int> getFarmIdsByProgramId(List<int> pProgramIdList);

        TreatmentTimes getTreatmentTimes(DateTime EveDate,
                                                         int DaysTreat, int HoursRepeat,
                                                         int DaysWaitingMeat, int HoursWaitingMeat,
                                                         int DaysWaitingMilk, int HoursWaitingMilk);

        TreatmentTimes getTreatmentTimes(int eventId);


        FILELOG getFileLogById(int Filelog_id);
        FILELOG getFileLogByHostnameAndFileName(String Hostname, String filepath);
        List<FILELOG> getFileLogsByHostnameAndFileName(String Hostname, String filepath);
        FILELOG_READER getFileLogReaderById(int reader_id);
        FILELOG_READER getFileLogReaderByDLLName(String dllname);
        int saveFileLog(FILELOG pFileLog);

        DataTable BloedonderzoekAuthorisatie_GetEventData(int pFarmId, int pProgId);
        DataTable BloedonderzoekAuthorisatie_GetBedrijvenMetData(int show, int pProgId);
        DataTable BloedonderzoekAuthorisatie_GetBedrijven(int show, int pProgId);
        DataTable BloedonderzoekAuthorisatie_GetBedrijven2(int show, int pProgId);
        int BloedonderzoekAuthorisatie_GetDMAantalMonsters(int programId, int ziekteId, int dmSoortId, int aantal);
        int BloedonderzoekAuthorisatie_CountDierAanwezig(int farmId, int pUbnId, DateTime datum, int groupId, int mede_eigenaar, int pProgId);

        List<int> getListUbnIdByListProgramId(List<int> listProgramId);

        List<int> getListProgramIdOndergeschikten(int pProgramId);
        List<int> getListUbnIdOndergeschikten(int pProgramId, List<BEDRIJF> listBedrijfFromSoap);

        [Obsolete("Verplaatst naar TempTable", true)]
        int init_temptable_ubnIds(int pProgramId, List<BEDRIJF> listBedrijfFromSoap);
        [Obsolete("Verplaatst naar TempTable", true)]
        int init_temptable_rdAanwezigeDieren(int pUbnId, int aniId, DateTime datum, int groupId, int mede_eigenaar, int pProgId);

        DataTable getDataTableAdministraties(int pProgramId, int pProgId, List<BEDRIJF> listBedrijfFromSoap);
        DataTable getDataTableBedrijven(List<int> listUbnId);

        string MySQL_Datum(DateTime datum, int iFormat);

        int mineraal_bepaalCategory(int progId, int programId, int aniId, int verplaatst, DateTime aniBirthDate, DateTime datum);
        int mineraal_bepaalCategory(int progId, int programId, int aniSex, int geworpen, int verplaatst, DateTime aniBirthDate, DateTime datum);

        DataTable getVKIAfvoerByUbn(string ubn);

        MEDLEVLOG GetMedlevlog(int mlId);
        MEDLEVLOG_DETAIL GetMedlevlog_Detail(int mldId);

        int SaveMedlevlog(MEDLEVLOG pMedlevlog);
        int SaveMedlevlog_Detail(MEDLEVLOG_DETAIL pMedlevlog_Detail);

        DataTable ucLijstenBullKeuze_loadDataGvBulls(int farmId);
        DataTable getDrachtcontrolesForAniIDsByDate(string pSeperatedAniIDs, int pUBNId, int pFarmId, DateTime pDate);

        DataTable getAvailableTiles(int pProgId, int pFarmId);
        DataTable getSelectedTiles(int pFarmId, int pProgId);
        DataTable getWeergaveMogelijkhedenForTile(int pProgId, int pInhoud);
        bool saveTile(TILES pTile);
        bool updateTile(TILES pTile);
        bool deleteTile(TILES pTile);
        bool saveTileDetail(TILES_DETAIL pTileDetail);
        bool deleteTileDetail(TILES_DETAIL pTileDetail);
        TILES GetTileByTileId(int pTile_Id);
        TILES_DETAIL GetTileDetailByTileId(int pTile_Id);
        TILES_SETTINGS GetTileSettingsByTileTitleProgId(string pTitle, int pProgId);
        TILES_SETTINGS getTileSettingsByInhoudProgId(int pTileInhoud, int pProgId);
        TILES_SETTINGS_DETAIL getTileSettingsDetailByInhoudProgIdWeergave(int pTileInhoud, int pProgId, int pWeergave);
        List<TILES_SETTINGS_DETAIL> getTileSettingsDetailsByInhoudProgId(int pTileInhoud, int pProgId);
        List<TILES> GetListChoosenTilesByFarmId(int pFarmId);
        List<TILES_SETTINGS> GetListTileSettingsByProgID(int pProgId);

        DataTable getChildRaces(ANIMAL pAnimal, int pProgramId, int pProgId);

        bool LOG_AgrologsLog(string pLogName, string pLogLevel, string pIP_Adress, string pThreadName
           , string pAgroUser, int pFarmID, int pUbnID, int pThirdID, int pLogCode, string pLogDescription, string pModule
           , string pFunctionCall, string pHTTP_Requesttype, string pPage, string pagrobase_versie, string pServerIP
           , string pMessage, string pStackTrace);


        PROCESCOMPUTERLOG CreateProcesComputerLog(int UbnId, sbyte pclType, string pclVersion, ulong PclXMLFields);
        bool CloseProcesComputerLog(PROCESCOMPUTERLOG log, sbyte Result);
        bool LogTasktoolMessageToAgrobase(PROCESCOMPUTERLOG log, int UbnId, int LogKind, int PcldPluginType, int PcldMessageKind, String Version, String Message);
        bool SetMovementReportDate(MOVEMENT pMovement);
        bool SetBirthReportDate(BIRTH pBirth);
        bool SetInseminReportDateAndStatus(INSEMIN pInsemin);
        List<EVENT> getEventsByFarmId(int pFarmId, int pMaxAniCategory);
        List<EVENT> getEventsByFarmId(int pFarmId, int pMaxAniCategory, DateTime MinMutationDate);
        List<EVENT> getUnreportedHeatsByFarmId(int farmId, LABELSConst.ReportOrganization cRV_IenR_WS);
        List<EVENT> getUnreportedEventsByFarmId(int pFarmId, VSM.RUMA.CORE.DB.LABELSConst.EventKind pEventKind, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization pReport);

        List<EVENT> GetUnreportedEventsByUbnId(int ubnId, LABELSConst.EventKind eveKind, LABELSConst.ReportOrganization report);
        
        string getLastIplogin(int thrid);

        #region Mineraal Minas

        List<LABMINAS> GetLabMinas(int pLabkind, int pLabCountry);
        List<LABMINAS> GetLabMinas(List<int> pLabkinds, int pLabCountry);
        LABMINAS GetLabMina(int pLabkind, int pLabCountry, int pLabId);



        MBSEXCR getMbSexCr(string pMestnummer, int pJaar);
        bool saveMbsexcr(MBSEXCR pMbsexcr);
        bool DeleteMbsexcr(MBSEXCR pMbsexcr);

        List<MGRGEBRK> getMgrgebrken(string pMestnummer, int pJaar);
        bool saveMgrgebrk(MGRGEBRK pMgrgebrk);
        bool DeleteMgrgebrk(MGRGEBRK pMgrgebrk);

        List<MBEGIN> getMBeginnen(string pMestnummer, int pJaar, int pDiersoort);
        bool saveMBegin(MBEGIN pMBegin);
        bool DeleteMBegin(MBEGIN pMBegin);

        List<MMUTAT> getMMutats(string pMestnummer, int pJaar);
        MMUTAT getMMutat(int pInternalnr);
        bool saveMMutat(MMUTAT pMMutat);
        bool DeleteMMutat(MMUTAT pMMutat);

        List<MMNDTEL> getMMndtels(string pMestnummer, int pJaar);
        MMNDTEL getMMdtel(int pInternalnr);
        bool saveMMdtel(MMNDTEL pMMdtel);
        bool DeleteMMdtel(MMNDTEL pMMdtel);

        MKOEMLK getMkoemlk(string pMestnummer, int pJaar, int pPrognose);
        bool saveMkoemlk(MKOEMLK pMkoemlk);

        List<MAFWSTLS> getMafWstels(string pMestnummer, int pJaar);
        bool saveMafWstels(MAFWSTLS pMafStel);

        List<MSTLDRPR> getMstldrprs(string pMestnummer, int pJaar, int pPrognose);
        MSTLDRPR getMstldrpr(int pInternalnr);
        bool saveMstldrpr(MSTLDRPR pMstldrpr);
        bool DeleteMstldrpr(MSTLDRPR pMstldrpr);

        List<MSTLDRGG> getMstldrggs(string pMestnummer, int pJaar, int pProductType, int pPrognose);
        MSTLDRGG getMstldrgg(int pInternalnr);
        bool saveMstldrgg(MSTLDRGG pMstldrgg);
        bool DeleteMstldrgg(MSTLDRGG pMstldrgg);
        List<REMMINAS> getRemminassen(string pMestnummer);
        bool saveRemminas(REMMINAS pRemminas);
        bool DeleteRemminas(REMMINAS pRemminas);

        List<MMESTGG> getMMestggs(string pMestnummer, int pJaar, int pMestType, int pPrognose);
        MMESTGG getMMestgg(int pInternalnr);
        bool saveMMestgg(MMESTGG pMMestgg);
        bool DeleteMMestgg(MMESTGG pMMestgg);

        List<MAANTDRN> getMaantdrns(string pMestnummer, int pJaar, int pAnimalKind);
        MAANTDRN getMaantdrn(string pMestnummer, int pJaar, int pAnimalKind, int pAniCategory);
        bool saveMaantdrn(MAANTDRN pMaantdrn);
        bool DeleteMaantdrn(MAANTDRN pMaantdrn);

        //reken tabellen
        List<MANINORM> getManinormenByYear(int pJaar);
        List<MEIEREN> getMeieren(int pJaar);
        List<MRUWVOER> getMRuwvoeren(int pJaar);
        List<MWCOEFF> getMWcoeffen(int pJaar);

        DataTable getTablesnamesFilledByMestnummer(string pMestnummer);

        #endregion

        #region feeds

        bool DeleteFeedRationSettings(FEED_RATION_SETTINGS pFeedRationSettings);
        bool DeleteFeedcurvedays(FEEDCURVE_DAYS pFeedcurvedays);
        bool DeleteFeedcurvedaysDetails(FEEDCURVE_DAYS_DETAIL pFeedcurvedaysDetails);
        bool DeleteFeedCurveMilk(FEEDCURVE_MILK pFeedcurvemilk);
        bool DeleteFeedCurveMilkDetailAndInfo(FEEDCURVE_MILK_MILKINFO pFeedcurvemilkinfo);
        bool DeleteFeedCurveUpDown(FEEDCURVE_UPDOWN pFeedcurveUpDown);
        bool DeleteFeedcurveUpDownDetails(FEEDCURVE_UPDOWN_DETAIL pFeedcurveUpDownDetails);
        bool saveFeedInComp(FEED_IN_FEEDCOMPUTER pFeedInComp);
        bool saveFeedstep(FEED_STEP pFeedStep);
        bool saveFeedAdviceRovecom(FEED_ADVICE_ROVECOM pFeedAdviceRovecom);
        bool DeleteFeedAdviceRovecom(FEED_ADVICE_ROVECOM pFeedAdviceRovecom);
        bool saveFeedRovecom(FEED_ROVECOM pFeedRovecom);
        bool DeleteFeedRovecom(FEED_ROVECOM pFeedRovecom);

        #endregion

        List<FTPUSER> getFtpUsersMultipleDatabases(List<string> dataBases, int FtpNumber);

        string getThirdProperty(int thrId, string tpKey);

        List<THIRD_PROPERTY> getThirdPropertys(string tpKey);

        bool setThirdProperty(int thrId, string tpKey, string tpValue);

        string getUbnProperty(int ubnId, string upKey);

        bool setUbnProperty(int ubnId, string upKey, string upValue);

        List<UBN_PROPERTY> getUbnProperties(string upKey);
        List<UBN_PROPERTY> getUbnPropertiesLike(string upKey);

        List<MOVEMENT> GetMovementsByAniIdMovkind(int aniId, int movKind);

        List<KeyValuePair<int, DateTime>> getUniekeWorpen(int aniId);



        DateTime? StringToNDT(string dt);

        bool treatmentExists(int AniId, DateTime date, VSM.RUMA.CORE.DB.LABELSConst.TreKindDefault TreKind);

        bool gestationExists(int AniId, DateTime date, VSM.RUMA.CORE.DB.LABELSConst.gesStatus gesStatus);

        bool inheatExists(int AniId, DateTime date, VSM.RUMA.CORE.DB.LABELSConst.HeatCertainty heatCertainty);


        /// <summary>
        /// Returns true if one of the Diseases is recorded for AniId on date
        /// </summary>
        /// <param name="AniId"></param>
        /// <param name="date"></param>
        /// <param name="Diseases"></param>
        /// <returns></returns>
        bool diseaseExistsOnDate(int AniId, DateTime date, List<Disease> Diseases);


        /*int addEventAndInheat(EVENT e, INHEAT i);
        int addEventAndBlood(EVENT e, BLOOD b);
        int addEventAndTreatment(EVENT e, TREATMEN t);*/


        int saveWerklijstHetDeensSysteem(WERKLIJST_HET_DEENS_SYSTEEM werklijst);


        DateTime GetLastMPRDate(int ubnId);

        bool werklijstExists(WERKLIJST_HET_DEENS_SYSTEEM whds);


        #region VERIFIED_ANIMAL/MOEMENT

        bool SetVerifiedAnimalAndMovements(string lifeNumber, string werknummer, DateTime geboortedat, DateTime importdat, string landCodeHerkomst, string landCodeOorsprong, int aniSex, string haarkleur, DateTime einddatum, string redenEinde, string levensnrMoeder, string vervangenLevensnr, IEnumerable<VERIFIED_MOVEMENT> verblijfplaatsen, int animalSpecies, int changed_By, int sourceId);

        VERIFIED_ANIMAL GetVerifiedAnimal(string lifeNumber);

        IEnumerable<VERIFIED_MOVEMENT> GetVerifiedMovements(string lifeNumber);

        bool UpdateVerifiedMovementsTimestamp(string lifeNumber, int changed_By, int sourceId);



        void VerifiedCallError(LABELSConst.VerifiedDataSource dataSource, string lifenr);

        void VerifiedCallOk(LABELSConst.VerifiedDataSource dataSource, string lifenr);

        AnimalCallInfo GetCallInfo(LABELSConst.VerifiedDataSource dataSource, string lifenr);

        IEnumerable<AnimalCallInfo> GetVerifiedCalls(LABELSConst.VerifiedDataSource dataSource, IEnumerable<string> lifeNrs);

        bool SetVerifiedCalls(LABELSConst.VerifiedDataSource dataSource, IEnumerable<Tuple<string, bool>> callLogs);

        IEnumerable<VERIFIED_ANIMAL_SANITEL> GetVerifiedAnimalsSanitel(IEnumerable<string> lifeNumbers);

        IEnumerable<VERIFIED_MOVEMENT_SANITEL> GetVerifiedMovementsSanitel(IEnumerable<string> lifeNumbers);

        bool UpdateVerifiedDataSanitel(IEnumerable<VERIFIED_ANIMAL_SANITEL> animals, IEnumerable<VERIFIED_MOVEMENT_SANITEL> movements, LABELSConst.ChangedBy changedBy, int SourceId);

        IEnumerable<SanitelBedrijfInfo> GetSanitelBedrijfInfo(IEnumerable<string> productionUnits, IEnumerable<string> facilities);

        bool SetSanitelFacility(int ubnId, string facility);

        bool SaveVerifiedAnimalCall(VERIFIED_ANIMAL_CALL vac);
        #endregion






        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // AGROLINK ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Int64 GetBinairFromInt(int pInt);
        Int64 GetBinairFromInts(List<int> pListInt);
        List<int> GetIntsFromBinair(Int64 pInt64);

        int SaveObject(DataObject pDataObject, DBConnectionToken pToken);
        void ExecuteQuery(StringBuilder pQuery, DBConnectionToken pToken);

        List<int> GetAgrolinkRechten(int pThrId1, int pThrId2, int pKind, string pMachtigingId);
        List<GEMACHTIGD> GetGemachtigd(int pThrId1, int pThrid2);
        List<THIRD> GetAgrolinkRelations(string soort, int agrolink_progid);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        int saveFile_Import(FILE_IMPORT pFile_Import);

        int saveFile_Import_Type(FILE_IMPORT_TYPE pFile_Import_Type);

        List<FILE_IMPORT> getFile_ImportsByUbnID_Destination(int pUbnID_Destination, VSM.RUMA.CORE.DB.LABELSConst.FILE_IMPORT_Status pStatus);

        FILE_IMPORT_TYPE getFile_Import_TypesByID(int pFile_Import_Type_ID);

        FILE_IMPORT getFile_ImportByData(string FI_Data_Row);

        List<FILE_IMPORT> GetFile_ImportsByData(int UbnID_Destination, string filename, string line, int File_Import_Type_ID);

        List<FILE_IMPORT> GetFile_ImportsByName(string bestandsnaam, int fileimportkind);

        List<T> getList<T>(DataTable dt) where T : DataObject, new();
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        T getSingleItem<T>(DbCommand cmd) where T : DataObject, new();
        T getSingleItem<T>(DBConnectionToken token, string qry) where T : DataObject, new();
        T getSingleItem<T>(DBConnectionToken token, StringBuilder qry) where T : DataObject, new();

        DataTable getK01DyrLaktaCalves(int UbnId, int MaxAniCategory);

        IEnumerable<UBN_PROPERTY> GetUbnPropertiesForKeyValue(string configKey, string value);
        
        /// <summary>
        /// Add a new Tasklog record to the database
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="processName"></param>
        /// <param name="processVersion"></param>
        /// <param name="processStart"></param>
        /// <param name="progId"></param>
        /// <param name="soaplogKind"></param>
        /// <param name="Ok"></param>
        /// <param name="Warning"></param>
        /// <param name="noData"></param>
        /// <param name="Error"></param>
        /// <param name="message"></param>
        /// <param name="logPath"></param>
        /// <param name="State"></param>
        /// <returns>a TasklogId of the newly inserted record, negative otherwise</returns>
        int SaveNewTaskLog(string hostname, string processName, string processVersion, DateTime processStart, int progId, int soaplogKind, int Ok, int Warning, int noData, int Error, string message, string logPath, int State);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="processName"></param>
        /// <param name="processVersion"></param>
        /// <param name="processStart"></param>
        /// <param name="progId"></param>
        /// <param name="soaplogKind"></param>
        /// <param name="message"></param>
        /// <param name="logPath"></param>
        /// <returns></returns>
        int SaveNewTaskLog(string hostname, string processName, string processVersion, DateTime processStart, int progId, int soaplogKind, string message, string logPath);

        /// <summary>
        /// Update an existing TaskLog record
        /// </summary>
        /// <param name="tasklogId"></param>
        /// <param name="processEnd"></param>
        /// <param name="Ok"></param>
        /// <param name="Warning"></param>
        /// <param name="noData"></param>
        /// <param name="Error"></param>
        /// <param name="message"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        bool UpdateTaskLog(int tasklogId, DateTime? processEnd, int ok, int warning, int noData, int error, string message, int state);
        ARTIKEL_MEDIC getartikelbyMedicine(MEDICINE m);

        IEnumerable<MprDate> GetOrignalMPRDatesByUbnId(int ubnId);
        IEnumerable<MprDate> GetAgrobaseOrignalMPRDatesByUbnId(int ubnId);

        /// <summary>
        /// Get an empty SOAPLOG object with Farmnumber and Date/Time set.
        /// </summary>
        /// <param name="bedrijfsnummer"></param>
        /// <param name="changedBy"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        SOAPLOG CreateSOAPLOG(string bedrijfsnummer, LABELSConst.ChangedBy changedBy, int sourceId);
        ANIMAL getbirthanimal(int eventId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bedrijfsnummer"></param>
        /// <param name="soapKind"></param>
        /// <param name="soapStatus"></param>
        /// <param name="soapCode"></param>
        /// <param name="lifenumber"></param>
        /// <param name="soapOmschrijving"></param>
        /// <param name="changedBy"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        SOAPLOG CreateSOAPLOG(string bedrijfsnummer, LABELSConst.SOAPLOGKind soapKind, string soapStatus, string soapCode, string lifenumber, string soapOmschrijving, LABELSConst.ChangedBy changedBy, int sourceId, int tasklogId);
        void SetAnimalPropertyValues(int farmId, int ubnid, List<int> dieren, string ap_key, string ap_value, int changedby, int sourceid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectlist"></param>
        /// <param name="reportdate"></param>
        /// <returns></returns>
        int UpdateCrv_MeldingenReportDate(List<CRV_MELDING> objectlist, DateTime reportdate, int Cm_Report_State);

        /// <summary>
        /// TRANSACTION
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectlist"></param>
        /// <param name="keykolom"></param>
        /// <param name="updatekoloms"></param>
        /// <returns></returns>
        int UpdateDataObjects<T>(List<T> objectlist, string keykolom, string[] updatekoloms, string database) where T : DataObject;
        
        /// <summary>
        ///  TRANSACTION
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectlist"></param>
        /// <param name="keykolom"></param>
        /// <returns></returns>
        int InsertDataObjects<T>(List<T> objectlist, string[] keykolom, string database) where T : DataObject;
    }

    public class Disease
    {
        public int MainCode;
        public int SubCode;

        public Disease(int pMainCode, int pSubCode)
        {
            MainCode = pMainCode;
            SubCode = pSubCode;
        }

        public override string ToString()
        {
            return MainCode.ToString() + "_" + SubCode.ToString();
        }
    }

    public class AniIdDate
    {
        public int AniId;
        public DateTime? Date;

        public AniIdDate(int pAniId, DateTime? pDate)
        {
            this.AniId = pAniId;
            this.Date = pDate;
        }
    }

    public class AniIdDateComparer : IEqualityComparer<AniIdDate>
    {
        public bool Equals(AniIdDate x, AniIdDate y)
        {
            return (x.AniId == y.AniId
                    && x.Date == y.Date);
        }

        public int GetHashCode(AniIdDate obj)
        {
            return obj.AniId.GetHashCode() ^ obj.Date.GetHashCode();
        }
    }


    public class TreatmentTimes
    {
        public DateTime EveDate;
        public DateTime dtEnd_Treatment;
        public DateTime dtEnd_WaitingMeat;
        public DateTime dtEnd_WaitingMilk;
        public DateTime dtEnd_WaitingTotal;
        public int dgn_Treatment;
        public int dgn_WaitingMeat;
        public int dgn_WaitingMilk;
        public int dgn_WaitingTotal;

        public TreatmentTimes()
        {
            EveDate = DateTime.MinValue;
            dtEnd_Treatment = DateTime.MinValue;
            dtEnd_WaitingMeat = DateTime.MinValue;
            dtEnd_WaitingMilk = DateTime.MinValue;
            dtEnd_WaitingTotal = DateTime.MinValue;
            dgn_Treatment = 0;
            dgn_WaitingMeat = 0;
            dgn_WaitingMilk = 0;
            dgn_WaitingTotal = 0;
        }
    }

}
