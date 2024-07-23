using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.SOAPLNV
{
    public class OpvragenLNVDierDetailsV2
    {
        private Masking masking = new Masking();
        /// <summary>
        /// M / V geslacht code naar AniSex
        /// </summary>
        /// <param name="geslacht"></param>
        /// <returns></returns>
        protected static int LNVGeslachtToAnisex(string geslacht)
        {
            if (geslacht == "M")
                return (int)LABELSConst.AniSex.Mannelijk;
            else if (geslacht == "V")
                return (int)LABELSConst.AniSex.Vrouwelijk;

            return 0;
        }

        /// <summary>
        /// AniSex naar M / V geslacht
        /// </summary>
        /// <param name="aniSex"></param>
        /// <returns></returns>
        protected static string AnisexToLNVGeslacht(int aniSex)
        {
            if (aniSex == (int)LABELSConst.AniSex.Mannelijk)
                return "M";
            else if (aniSex == (int)LABELSConst.AniSex.Vrouwelijk)
                return "V";

            return "";
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="verifiedMovements"></param>
        /// <returns></returns>
        private static List<SOAPLNVDieren.Dierverblijfplaats> VerifiedMovementsToRVOVerblijfplaatsen(IEnumerable<VERIFIED_MOVEMENT> verifiedMovements)
        {
            var retList = new List<SOAPLNVDieren.Dierverblijfplaats>(verifiedMovements.Count());

            foreach (var vm in verifiedMovements.Reverse())
            {
                var ver = new SOAPLNVDieren.Dierverblijfplaats();

                ver.AanvoerDatum = vm.MovementInDate;
                ver.AfvoerDatum = vm.MovementOutDate;
                ver.Bedrijfstype = vm.MovementCompanyType;
                ver.Postcode = vm.MovementZipcode;
                ver.UBN = vm.MovementUBN;                

                retList.Add(ver);
            }

            return retList;
        }

        private static IEnumerable<VERIFIED_MOVEMENT> RVOVerblijfplaatsenToVerifiedMovements(string lifeNr, IEnumerable<SOAPLNVDieren.Dierverblijfplaats> verblijfplaatsen)
        {
            var retList = new List<VERIFIED_MOVEMENT>(verblijfplaatsen.Count());

            int i = 1;
            foreach (var ver in verblijfplaatsen.Reverse())
            {
                var vm = new VERIFIED_MOVEMENT();

                vm.AnimalLifenumber = lifeNr;
                vm.MovementOrder = i;
                vm.MovementInDate = ver.AanvoerDatum;
                vm.MovementOutDate = ver.AfvoerDatum;

                vm.MovementCompanyType = ver.Bedrijfstype;
                vm.MovementZipcode = ver.Postcode;
                vm.MovementUBN = ver.UBN;

                i++;
                retList.Add(vm);
            }

            return retList;
        }

        private const int MAX_AGE_DAYS_UNKNOWN_LIFENR = 14;


        private static bool SetAniCat(int aniId, int aniCat, string workNr, IEnumerable<int> farmIds, Dictionary<AniCatKey, ANIMALCATEGORY> bufferedAniCat, int changedBy, int sourceId)
        {
            string prefix = IRUtils.formatLogPrefix($"{nameof(OpvragenLNVDierDetailsV2)},{nameof(SetAniCat)}");

            try
            {
                foreach (int farmId in farmIds)
                {
                    var c = bufferedAniCat.FirstOrDefault(bac => bac.Key.AniId == aniId && bac.Key.FarmId == farmId);

                    if (c.Key == null || c.Key.AniId != aniId)
                    {
                        var key = new AniCatKey(farmId, aniId, null, true);
                        var cat = new ANIMALCATEGORY();
                        cat.Anicategory = aniCat;
                        cat.AniWorknumber = workNr;
                        cat.Changed_By = changedBy;
                        cat.SourceID = sourceId;
                        bufferedAniCat.Add(key, cat);
                    }
                    else
                    {
                        if (c.Value.Anicategory != aniCat)
                        {
                            c.Key.ForceUpdate = true;
                            c.Value.Anicategory = aniCat;
                            c.Value.AniWorknumber = workNr;
                            c.Value.Changed_By = changedBy;
                            c.Value.SourceID = sourceId;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                unLogger.WriteError($"{prefix} Fout tijdens aanpassen anicategory in cache. Ex: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// LET OP, saved de animalcategories NIET indied bufferedAniCats niet NULL is (dan kun je het saven van de animalcategories later in een keer doen.)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="farm"></param>
        /// <param name="requestingUBN"></param>
        /// <param name="pUsername"></param>
        /// <param name="pPassword"></param>
        /// <param name="TestServer"></param>
        /// <param name="ani"></param>
        /// <param name="lifenr"></param>
        /// <param name="bufferedAniCats"></param>
        /// <param name="farmIds"></param>
        /// <param name="hasMovements">Indien nee meegegeven zet hij oude mannelijke dieren op NooitAanwezig geweest ipv AanwezigGeweest los van bufferedCats aangezien dat null kan zijn</param>
        /// <returns>
        /// </returns>
        public static bool CorrigeerMovementsvanuitLNV(AFSavetoDB db, BEDRIJF farm, UBN requestingUBN, string pUsername, string pPassword, int TestServer, ANIMAL ani, string lifenr, int maxCallErrors, Dictionary<AniCatKey, ANIMALCATEGORY> bufferedAniCats, IEnumerable<int> farmIds = null, bool updateBirthMotherWithCalfId = false, bool useVerifiedAnimal = false, int maxAgeVerifiedAnimalMinutes = 1440, bool warnForDuplicateMovements = false, int minMovementDaysBeforeDelete = 10, bool hasMovements = true, bool createBirthMovements = false)
        {
            int changedBy = (int)LABELSConst.ChangedBy.LNVVerblijfplaatsenCorrectie;
            int sourceId = requestingUBN.UBNid;

            string lPrefixAnimal = IRUtils.formatLogPrefix($"{nameof(OpvragenLNVDierDetailsV2)}.{nameof(CorrigeerMovementsvanuitLNV)} Bedrijf: '{requestingUBN.Bedrijfsnummer.PadRight(10, ' ')}' Dier: '{ani.AniAlternateNumber}'");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ANIMALCATEGORY anicat = null;

            try
            {
                #region Initialize parameters
                int DierSoort = farm.ProgId;
                string BRSnrHouder = string.Empty;
                string UBNhouder = string.Empty;
                string Werknummer = string.Empty;
                DateTime Geboortedat = DateTime.MinValue;
                DateTime Importdat = DateTime.MinValue;
                string LandCodeHerkomst = string.Empty;
                string LandCodeOorsprong = string.Empty;
                string Geslacht = string.Empty;
                string Haarkleur = string.Empty;
                DateTime Einddatum = DateTime.MinValue;
                string RedenEinde = string.Empty;
                string LevensnrMoeder = string.Empty;
                string VervangenLevensnr = string.Empty;
                string Status = string.Empty;
                string Code = string.Empty;
                string Omschrijving = string.Empty;
                #endregion

                #region soap call
                List<SOAPLNVDieren.Dierverblijfplaats> verblijfplaatsen = new List<SOAPLNVDieren.Dierverblijfplaats>();
                SOAPLNVDieren soapdieren = new SOAPLNVDieren();

                Stopwatch swSoapcall = new Stopwatch();
                swSoapcall.Start();

                if (!useVerifiedAnimal)
                {
                    soapdieren.LNVDierdetailsV2(pUsername, pPassword, TestServer,
                        UBNhouder, String.Empty,
                        lifenr, DierSoort,
                        1, 0, 0,
                        ref Werknummer, ref Geboortedat,
                        ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                        ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                        ref LevensnrMoeder,
                        ref VervangenLevensnr,
                        ref verblijfplaatsen,
                        ref Status, ref Code, ref Omschrijving);

                    if ((Code == "HTTP200" ) || (Code == null)) //GD: check op code toegevoegd bij response code 200 is code null, check op "HTTP200" just in case)
                    {
                        db.VerifiedCallOk(LABELSConst.VerifiedDataSource.RVO, lifenr);

                        db.SetVerifiedAnimalAndMovements(lifenr, Werknummer, Geboortedat, Importdat, LandCodeHerkomst, LandCodeOorsprong, LNVGeslachtToAnisex(Geslacht), Haarkleur, Einddatum, RedenEinde, LevensnrMoeder, VervangenLevensnr, RVOVerblijfplaatsenToVerifiedMovements(lifenr, verblijfplaatsen), farm.ProgId, changedBy, sourceId);
                    }
                    else if (Status == "F")
                    {
                        db.VerifiedCallError(LABELSConst.VerifiedDataSource.RVO, lifenr);
                    }
                }
                else
                {
                    VERIFIED_ANIMAL verifiedAnimal = db.GetVerifiedAnimal(ani.AniAlternateNumber);
                    IEnumerable<VERIFIED_MOVEMENT> verifiedMovements = db.GetVerifiedMovements(ani.AniAlternateNumber);

                    if (verifiedAnimal == null
                       || verifiedAnimal.MutationDate.AddMinutes(maxAgeVerifiedAnimalMinutes) < DateTime.Now
                       || verifiedMovements == null
                       || !verifiedMovements.Any()
                       || verifiedMovements.Max(m => m.MutationDate).AddMinutes(maxAgeVerifiedAnimalMinutes) < DateTime.Now
                       )
                    {
                        var callInf = db.GetCallInfo(LABELSConst.VerifiedDataSource.RVO, ani.AniAlternateNumber);
                        if (callInf.Error >= maxCallErrors)
                        {
                            unLogger.WriteWarn($"{lPrefixAnimal} Aantal Errors bij RVO >= '{maxCallErrors}' Ga dier niet ophalen. ");
                            Status = "F";
                            Code = "IRD-00192";
                        }
                        else if (callInf.Error > 0 && callInf.TS.HasValue && callInf.TS.Value.AddMinutes(maxAgeVerifiedAnimalMinutes) >= DateTime.Now)
                        {              
                            unLogger.WriteWarn($"{lPrefixAnimal} Recent error bij opvragen RVO. (Overgeslagen)");
                            Status = "F";
                            Code = "IRD-00192";                            
                        }
                        else
                        {
                            //Cache te oud / niet aanwezig
                            string maxAniMut = "null";
                            if (verifiedAnimal != null)
                                maxAniMut = verifiedAnimal.MutationDate.ToString("dd-MM-yyyy hh:mm");

                            string maxMovMut = "null";
                            if (verifiedMovements != null && verifiedMovements.Any())
                                maxMovMut = verifiedMovements.Max(m => m.MutationDate).ToString("dd-MM-yyyy hh:mm");

                            unLogger.WriteTrace($"{lPrefixAnimal} Cache invalid. va date: {maxAniMut} vm date: {maxMovMut}");

                            soapdieren.LNVDierdetailsV2(pUsername, pPassword, TestServer,
                                UBNhouder, String.Empty,
                                lifenr, DierSoort,
                                1, 0, 0,
                                ref Werknummer, ref Geboortedat,
                                ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                                ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                                ref LevensnrMoeder,
                                ref VervangenLevensnr,
                                ref verblijfplaatsen,
                                ref Status, ref Code, ref Omschrijving);

                            if ((Code == "HTTP200") || (Code == null)) //GD: check op code toegevoegd bij response code 200 is code null, check op "HTTP200" just in case
                            {
                                db.VerifiedCallOk(LABELSConst.VerifiedDataSource.RVO, lifenr);
                                db.SetVerifiedAnimalAndMovements(lifenr, Werknummer, Geboortedat, Importdat, LandCodeHerkomst, LandCodeOorsprong, LNVGeslachtToAnisex(Geslacht), Haarkleur, Einddatum, RedenEinde, LevensnrMoeder, VervangenLevensnr, RVOVerblijfplaatsenToVerifiedMovements(lifenr, verblijfplaatsen), farm.ProgId, changedBy, sourceId);
                            }
                            else if (Status == "F")
                            {
                                db.VerifiedCallError(LABELSConst.VerifiedDataSource.RVO, lifenr);
                            }

                        }
                    }
                    else
                    {
                        unLogger.WriteTrace($"{lPrefixAnimal} Gebruik cache.");

                        Werknummer = verifiedAnimal.AnimalWorkNr;
                        Geboortedat = verifiedAnimal.AnimalBirthDate;
                        Importdat = verifiedAnimal.AnimalImportDate;
                        LandCodeHerkomst = verifiedAnimal.AnimalCountryCodeOrigin1;
                        LandCodeOorsprong = verifiedAnimal.AnimalCountryCodeOrigin2;

                        Geslacht = AnisexToLNVGeslacht(verifiedAnimal.AnimalSex);
                        Haarkleur = verifiedAnimal.AnimalHaircolor;
                        Einddatum = verifiedAnimal.AnimalDateEnd;
                        RedenEinde = verifiedAnimal.AnimalReasonEnd;
                        LevensnrMoeder = verifiedAnimal.AnimalMotherLifenumber;
                        VervangenLevensnr = verifiedAnimal.AnimalReplacementLifenumber;
                        verblijfplaatsen = VerifiedMovementsToRVOVerblijfplaatsen(verifiedMovements);

                        Status = "G";
                        Code = "";
                        Omschrijving = "";
                    }
                }

                swSoapcall.Stop();

                // ML 21-1-2019
                // Na overleg Nico, de historische dieren mogen op Category 5 gezet worden wanneer er geen movements aanwezig zijn.
                bool forceerHistorisch = false;
                int forceerHistorieCat = hasMovements ? (int)LABELSConst.AniCategory.AanwezigGeweest : (int)LABELSConst.AniCategory.NooitAanwezigGeweest;

                if (Status == "F")
                {
                    //ML: Geen idee wat IRD-00192 voor een code is?
                    if (Code == "IRD-00192" && ani.AniId > 0)
                    {
                        if (ani.AniBirthDate < DateTime.Now.AddDays(-MAX_AGE_DAYS_UNKNOWN_LIFENR))
                        {
                            //AniCategory aanpassen, dit dier moet op 4 (Aanwezig geweest) gezet worden.
                            //schrijf hier later eventueel een logregel voor als je de category hebt, dan hoef je alleen te loggen als het dier aanwezig is.
                            forceerHistorisch = true;

                            if (forceerHistorieCat == (int)LABELSConst.AniCategory.NooitAanwezigGeweest)
                            {
                                unLogger.WriteInfo($"{lPrefixAnimal} Zet op NooitAanwezigGeweest. (geen nieuwe geboorte, geen movements).");
                            }
                        }
                        else
                        {
                            unLogger.WriteInfo($"{lPrefixAnimal} CRV GeboorteDatum {ani.AniBirthDate:dd-MM-yyyy} is niet bekend bij RVO maar staat wel in Agrobase. (Dier te jong, doe niks).");
                        }

                    }
                    else if (Code == "IRD-99999")
                    {
                        unLogger.WriteInfo($"{lPrefixAnimal} Technische problemen bij ophalen.");
                        return false;
                    }
                    else
                    {
                        // Dieren die niet bij LNV zijn gevonden niet aanmaken in agrobase                    
                        unLogger.WriteWarn($"{lPrefixAnimal} Dier niet gevonden bij RVO, niet aanmaken.");
                        return true; //Genereer maar geen waarschuwing, dit komt te vaak voor, en anders zouden we alle boeren met een enkel onbekend dier (bv buitenlandse KI-Stier met AniCat 4) altijd status waarschuwing hebben.
                    }
                }
                #endregion

                bool updateAnimalData = ani.AniId <= 0;

                #region Animal

                if (!forceerHistorisch)
                {
                    // meer info ophalen
                    if (ani.AniAlternateNumber == string.Empty)
                    {
                        ani.AniAlternateNumber = lifenr;
                        updateAnimalData = true;
                    }

                    if (string.IsNullOrWhiteSpace(ani.AniLifeNumber) || (ani.AniLifeNumber != null && ani.AniLifeNumber.Length < 8 && lifenr.Length >= 8))
                    {
                        ani.AniLifeNumber = lifenr;
                        updateAnimalData = true;
                    }

                    if (DierSoort == 1 && !string.IsNullOrEmpty(Haarkleur) && ani.AniHaircolor_Memo != Haarkleur)
                    {
                        ani.AniHaircolor_Memo = Haarkleur;
                        updateAnimalData = true;
                    }

                    // luc 26-08-15 dieren met datum 01-01-1950 datum niet overschijven, 
                    // standaard rvo datum indien werkelijke geboortedatum niet bekend
                    if (Geboortedat != DateTime.MinValue && Geboortedat.Date != new DateTime(1950, 01, 01) && ani.AniBirthDate != Geboortedat)
                    {
                        ani.AniBirthDate = Geboortedat;
                        updateAnimalData = true;
                    }

                    int Sex = LNVGeslachtToAnisex(Geslacht);
                    if (ani.AniSex != Sex && Sex != 0)
                    {
                        ani.AniSex = Sex;
                        updateAnimalData = true;
                    }

                    if (LevensnrMoeder != string.Empty && ani.AniIdDam == 0)
                    {
                        ANIMAL aniMother = db.GetAnimalByAniAlternateNumber(LevensnrMoeder);

                        if (aniMother.AniId > 0 && ani.AniIdMother != aniMother.AniId)
                        {
                            ani.AniIdMother = aniMother.AniId;
                            updateAnimalData = true;
                        }
                    }

                    if (updateAnimalData)
                    {
                        unLogger.WriteInfo($"{lPrefixAnimal} Gewijzigd. (TS: {ani.ts})");
                        ani.Changed_By = changedBy;
                        ani.SourceID = sourceId;
                        if (!db.SaveAnimal(-99, ani))
                        {
                            unLogger.WriteError($"{lPrefixAnimal} Fout tijdens opslaan.");
                        }
                    }

                    if (updateBirthMotherWithCalfId && ani.AniIdMother > 0)
                        IRUtils.CorrigeerWorpBijMoeder(db, requestingUBN, ani, changedBy, sourceId);
                }
                #endregion

                #region AniCat
                AniCatKey key = null;

                if (bufferedAniCats != null)
                {
                    var bla = bufferedAniCats.FirstOrDefault(fi => fi.Key.AniId == ani.AniId && fi.Key.FarmId == farm.FarmId);
                    if (bla.Key != null && bla.Key.AniId > 0)
                    {
                        if (bla.Key.AniId == ani.AniId)
                            key = bla.Key;
                    };

                    if (key != null)
                    {
                        if (!bufferedAniCats.TryGetValue(key, out anicat))
                            unLogger.WriteError($"{lPrefixAnimal} Could not get aniCategory from buffer! AniId: {key.AniId} FarmId: {key.FarmId}");
                    }
                }

                if (anicat == null)
                {
                    anicat = db.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                }

                if (anicat.FarmId == 0 && bufferedAniCats != null)
                {

                    anicat.AniId = ani.AniId;
                    anicat.AniWorknumber = Werknummer;
                    anicat.FarmId = farm.FarmId;

                    //Hier is AniCategory 0, welk zou dit moeten zijn?   
                    IEnumerable<KeyValuePair<AniCatKey, ANIMALCATEGORY>> cats = bufferedAniCats.Where(ac => ac.Key.AniId == anicat.AniId && ac.Key.ForceUpdate);
                    if (!cats.Any())
                    {
                        unLogger.WriteDebug($"{lPrefixAnimal} Ophalen category voor farmId: {farm.FarmId} geeen geupdate records gevonden, pak bestaande.");
                        cats = bufferedAniCats.Where(ac => ac.Key.AniId == anicat.AniId && ac.Value.Anicategory > 0);
                    }
                    if (!cats.Any())
                    {
                        unLogger.WriteWarn($"{lPrefixAnimal} Dier staat nog niet op FarmId: {farm.FarmId} kan category niet achterhalen, zet op {LABELSConst.AniCategory.NooitAanwezigGeweest}.");
                        cats = bufferedAniCats.Where(ac => ac.Key.AniId == anicat.AniId && ac.Value.Anicategory > 0);
                        anicat.Anicategory = (int)LABELSConst.AniCategory.NooitAanwezigGeweest; //?
                    }
                    else
                    {
                        var ac = cats.First();
                        if (verblijfplaatsen != null && verblijfplaatsen.Count > 0)
                        {
                            var check = verblijfplaatsen.FindAll(x => x.UBN == requestingUBN.Bedrijfsnummer);
                            if (!check.Any())
                            {
                                anicat.Anicategory = (int)LABELSConst.AniCategory.NooitAanwezigGeweest;
                            }
                            else
                            {
                                var ordered = check.OrderBy(x => x.AanvoerDatum).ToList();
                                foreach (SOAPLNVDieren.Dierverblijfplaats v in ordered)
                                {
                                    anicat.Anicategory = (int)LABELSConst.AniCategory.Aanwezig;
                                    if (v.AfvoerDatum.Date > DateTime.MinValue)
                                    {
                                        anicat.Anicategory = (int)LABELSConst.AniCategory.AanwezigGeweest;
                                    }

                                }
                            }
                            if (ac.Value.Anicategory != anicat.Anicategory)
                            {
                                unLogger.WriteDebug($"{lPrefixAnimal} Verblijfplaatsen gevonden: ({(LABELSConst.AniCategory)anicat.Anicategory} FarmId: {farm.FarmId}) Komt niet overeen met ({(LABELSConst.AniCategory)ac.Value.Anicategory} FarmId: {ac.Value.FarmId}).");
                            }
                        }
                        else
                        {
                            unLogger.WriteDebug($"{lPrefixAnimal} Geen verblijfplaatsen gevonden: useVerifiedAnimal={useVerifiedAnimal} Anicategory:{(LABELSConst.AniCategory)ac.Value.Anicategory} gevonden bij FarmId: {ac.Value.FarmId} voor FarmId: {farm.FarmId}.");
                            anicat.Anicategory = ac.Value.Anicategory;
                        }
                        unLogger.WriteDebug($"{lPrefixAnimal} Gebruik category {(LABELSConst.AniCategory)ac.Value.Anicategory} gevonden bij FarmId: {ac.Value.FarmId} voor FarmId: {farm.FarmId}.");
                       
                    }

                    unLogger.WriteInfo($"{lPrefixAnimal} Staat nog niet op FarmId: {farm.FarmId} zet op Cateogry:  {(LABELSConst.AniCategory)anicat.Anicategory}");               
                }
                else if (forceerHistorisch)
                {
                    #region Forceer Historisch

                    if (anicat.Anicategory != forceerHistorieCat)
                    {
                        anicat.Anicategory = forceerHistorieCat;
                        ani.Changed_By = changedBy;
                        ani.SourceID = sourceId;
                    }
                    #endregion
                }
                else if (anicat.Anicategory <= 4 && Status == "F" && Code == "IRD-00192" && (ani.AniBirthDate > DateTime.Today.AddYears(-20) || ani.AniBirthDate == DateTime.MinValue))
                {
                    #region error Aanwezig dier.

                    // Dieren die niet bij LNV zijn gevonden naar nooit aanwezig geweest zetten  
                    unLogger.WriteInfo($"{lPrefixAnimal} Niet bij LNV gevonden, maar nooit aanwezig geweest.");

                    BIRTH bir = db.GetBirthByCalfId(ani.AniId);
                    if (bir.EventId > 0 && lifenr.ToUpper().StartsWith("NL") && ani.ThrId == requestingUBN.ThrID) // 12-9-2016 luc buitenlandse ki-stieren werden verwijderd
                    {
                        MUTATION mut = db.GetMutationByEventId(bir.EventId);
                        if (mut.Internalnr == 0)
                        {
                            // geen klaarstaande melding voor CRV, geboortemelding ingetrokkken?

                            MUTALOG mutlog = db.GetMutaLogByEventId(bir.EventId);
                            if ((mutlog.Report != 2 && mutlog.Internalnr > 0) || (ani.AniBirthDate < DateTime.Today.AddDays(-minMovementDaysBeforeDelete) && ani.AniBirthDate != DateTime.MinValue)) // melding geblokkkeerd? 
                            {
                                EVENT eve = db.GetEventdByEventId(bir.EventId);
                                if (eve.EventId > 0 && eve.EveMutationDate < DateTime.Today && eve.happened_at_FarmID == farm.FarmId)
                                {
                                    eve.Changed_By = changedBy;
                                    eve.SourceID = sourceId;

                                    if (db.DeleteEvent(eve))
                                    {
                                        unLogger.WriteDebug($"{lPrefixAnimal} Event '{eve.EventId}' - Verwijderd.");

                                        if (db.DeleteAnimal(ani))
                                        {
                                            unLogger.WriteError($"{lPrefixAnimal} Verwijderd.");
                                            anicat.Anicategory = (int)LABELSConst.AniCategory.NooitAanwezigGeweest;
                                        }
                                        else
                                        {
                                            unLogger.WriteError($"{lPrefixAnimal} Fout tijdens verwijderen.");
                                        }
                                    }
                                    else
                                    {
                                        unLogger.WriteError($"{lPrefixAnimal} Event '{eve.EventId}' - Fout tijdens verwijderen.");
                                    }

                                    unLogger.WriteDebug($"{lPrefixAnimal} Returned after delete event!");
                                    return false;
                                }
                                else
                                {
                                    unLogger.WriteError($"{lPrefixAnimal} Geboorte met EventId '{eve.EventId}' - Fout tijdens ophalen.");
                                }
                            }
                            else
                            {
                                if (ani.AniBirthDate > DateTime.Today.AddDays(-minMovementDaysBeforeDelete))
                                    unLogger.WriteDebug($"{lPrefixAnimal} Afkalving: is een nieuwe geboorte binnen {-minMovementDaysBeforeDelete} dagen.");
                                else
                                    unLogger.WriteDebug($"{lPrefixAnimal} Afkalving: geen melding of oude geboorte.");
                            }
                        }
                        else
                        {
                            unLogger.WriteDebug($"{lPrefixAnimal} Geen mutation gevonden voor geboorte met EventId '{bir.EventId}'.");
                        }
                    }
                    else if (ani.AniBirthDate < DateTime.Today.AddDays(-2))
                    {
                        unLogger.WriteInfo($"{lPrefixAnimal} Op afwezig gezet. (Obekend levensnummer, geboorte datum meer dan 2 dagen geleden).");

                        if (anicat.Anicategory != (int)LABELSConst.AniCategory.NooitAanwezigGeweest)
                        {
                            anicat.Anicategory = (int)LABELSConst.AniCategory.NooitAanwezigGeweest;
                            ani.Changed_By = changedBy;
                            ani.SourceID = sourceId;

                            unLogger.WriteDebug($"{lPrefixAnimal} Opslaan AnimalCategory.");

                            if (bufferedAniCats == null)
                                db.SaveAnimalCategory(anicat);
                        }

                        //ml: dit doet niks.        
                        ani.ThrId = 0; // Fokker op onbekend zetten want dier is niet bekend bij LNV
                    }
                    #endregion
                }
                else if (Status == "F")
                {
                    #region Error overig
                    if (Code == "IRD-00192")
                    {
                        if (anicat.Anicategory >= 5)
                        {
                            unLogger.WriteInfo($"{lPrefixAnimal} (afwezig) dier niet bekend bij RVO. ");
                        }
                        else
                        {
                            //Dier bestaat niet bij LNV?
                            unLogger.WriteInfo($"{lPrefixAnimal} Dier niet bekend bij RVO. (Te oud - Geb datum: {ani.AniBirthDate:dd-MM-yyyy})");
                        }
                    }
                    else
                    {
                        unLogger.WriteError($"{lPrefixAnimal} stop na AniCategory; Status == \"F\" Code: {Code}.");
                    }
                    return false;
                    #endregion
                }
                #endregion

                //Leeg initialiseren ivm Use of unnasigned local variable, maar zouden altijd gezet moeten zijn via hun functies wanner forceerHistorisch false is.
                List<MOVEMENT> listMovements = new List<MOVEMENT>();
                List<MOVEMENT> allMovs = new List<MOVEMENT>();

                #region Movements
                if (!forceerHistorisch) //Laat movements van helemaal onbekende dieren intact, ga ze niet weg gooien omdat RVO het dier niet kent.
                {
                    listMovements = db.GetMovementsByAniId(ani.AniId);
                    listMovements = listMovements.Where(x => x.MovKind == 1 || x.MovKind == 2 || x.MovKind == 3).ToList();

                    verblijfplaatsen.Reverse();

                    MOVEMENT mov;

                    unLogger.WriteTrace($"{lPrefixAnimal} Aantal movements (verblijfplaatsen): {verblijfplaatsen.Count}.");

                    allMovs = new List<MOVEMENT>();
                    foreach (var m in listMovements)
                    {
                        allMovs.Add(m);
                    }

                    int movorder = 1;
                    DateTime lastDate = DateTime.MinValue;

                    #region loop movements
                    for (int iVerblijfplaats = 0; iVerblijfplaats < verblijfplaatsen.Count; iVerblijfplaats++)
                    {
                        var movement = verblijfplaatsen[iVerblijfplaats];

                        UBN movementubn;
                        if (requestingUBN.Bedrijfsnummer.Trim() == movement.UBN.Trim())
                            movementubn = requestingUBN;
                        else
                            movementubn = db.getUBNByBedrijfsnummer(movement.UBN);


                        if (movementubn.UBNid == 0 && movement.AanvoerDatum == Geboortedat && (ani.ThrId != 0 || ani.UbnId != 0 ))
                        {
                            ani.ThrId = 0;
                            ani.UbnId = 0;
                            ani.Changed_By = changedBy;
                            ani.SourceID = sourceId;
                            db.UpdateANIMALFokker(ani.AniId, movementubn.ThrID, movementubn.UBNid, changedBy, sourceId);
                            unLogger.WriteInfo($"{lPrefixAnimal} GeboorteDatum {Geboortedat:dd-MM-yyyy} Bedrijfstype: {movement.Bedrijfstype} UBN: {movement.UBN} Fokker verwijderen.");
                        }


                        if (movementubn.UBNid > 0)
                        {
                            #region Aanvoer

                            if (movement.AanvoerDatum == Geboortedat && movementubn.ThrID > 0 && (ani.ThrId != movementubn.ThrID || ani.UbnId != movementubn.UBNid) && iVerblijfplaats == 0)
                            {
                                #region fokker corrigeren

                                ani.ThrId = movementubn.ThrID;
                                ani.UbnId = movementubn.UBNid;
                                ani.Changed_By = changedBy;
                                ani.SourceID = sourceId;
                                db.UpdateANIMALFokker(ani.AniId, movementubn.ThrID, movementubn.UBNid, changedBy, sourceId);
                                unLogger.WriteInfo($"{lPrefixAnimal} GeboorteDatum {Geboortedat:dd-MM-yyyy} Bedrijfstype: {movement.Bedrijfstype} UBN: {movement.UBN} Fokker Corrigeren.");

                                #endregion
                            }

                            if (!createBirthMovements )
                                  
                            //Geboortes ook aanmaken 

                            if (!createBirthMovements && movement.AanvoerDatum == Geboortedat && movementubn.ThrID > 0 && ani.ThrId == movementubn.ThrID && movement.AfvoerDatum == DateTime.MinValue)
                            {
                                 //doe niks, geboorte record dit was zoals het was voor de geboorte movementes, dit overslaan zorgt ervoor dat ze aangemaakt worden.                           
                            }
                            else
                            {
                                IEnumerable<MOVEMENT> aanvoer = listMovements.Where(m => (m.MovKind == (int)LABELSConst.MovementKind.AANVOER || m.MovKind == (int)LABELSConst.MovementKind.HUREN || m.MovKind == (int)LABELSConst.MovementKind.EINDEVERHUREN) && m.MovDate.Date == movement.AanvoerDatum.Date && m.UbnId == movementubn.UBNid);

                                if (lastDate < movement.AanvoerDatum)
                                {
                                    movorder = 1;
                                    lastDate = movement.AanvoerDatum;
                                }
                                else
                                    movorder++;

                                /////////////////////////////////////////
                                //Filtering meerdere movements op een dag.

                                int reservedBuying = 0;

                                //Hier dus filteren? 
                                for (int i = iVerblijfplaats + 1; i < verblijfplaatsen.Count; i++)
                                {
                                    if (verblijfplaatsen[i].AanvoerDatum == lastDate && verblijfplaatsen[i].UBN == movement.UBN)
                                        reservedBuying++;
                                }

                                if (reservedBuying >= aanvoer.Count())
                                    aanvoer = new List<MOVEMENT>();
                                else
                                    aanvoer = aanvoer.Take(aanvoer.Count() - reservedBuying);
                                ////////////////////////////////////

                                if (!aanvoer.Any())
                                {
                                    //geen movement gevonden
                                    mov = new MOVEMENT
                                    {
                                        MovMutationBy = 123,
                                        MovMutationDate = DateTime.Today,
                                        MovMutationTime = DateTime.Now,
                                        AniId = ani.AniId,
                                        MovDate = movement.AanvoerDatum,
                                        MovTime = new DateTime(1899, 12, 30).AddSeconds(movorder),
                                        MovKind = 1,
                                        MovOrder = movorder,
                                        UbnId = movementubn.UBNid,
                                        Progid = DierSoort,
                                        ReportDate = DateTime.Today,
                                        Changed_By = changedBy,
                                        SourceID = sourceId
                                    };

                                    db.SaveMovement(mov);
                                    allMovs.Add(mov);

                                    BUYING buy = new BUYING
                                    {
                                        MovId = mov.MovId,
                                        Changed_By = changedBy,
                                        SourceID = sourceId
                                    };

                                    unLogger.WriteDebug($"{lPrefixAnimal} MovDate: {mov.MovDate:dd-MM-yyyy} Bedrijfstype: {movement.Bedrijfstype} UBN: {movement.UBN} Aanvoer toevoegen.");

                                    db.SaveBuying(buy);
                                }
                                else
                                {
                                    //Movements gevonden
                                    if (warnForDuplicateMovements && aanvoer.Count() > 2)
                                        unLogger.WriteWarn($"{lPrefixAnimal} MovDate: {movement.AanvoerDatum.Date:dd-MM-yyyy} Aantal aanvoer Movements: {aanvoer.Count()}");

                                    foreach (MOVEMENT movAanvoer in aanvoer.Where(m => m.MovOrder != movorder || m.MovTime != new DateTime(1899, 12, 30).AddSeconds(movorder)))
                                    {
                                        //Movement met verkeerde mov order
                                        unLogger.WriteInfo($"{lPrefixAnimal} MovDate: {movAanvoer.MovDate:dd-MM-yyyy} MovOrder {movAanvoer.MovOrder} -> {movorder} MovTime {movAanvoer.MovTime} -> {new DateTime(1899, 12, 30).AddSeconds(movorder)}");

                                        movAanvoer.MovOrder = movorder;
                                        movAanvoer.Changed_By = (int)changedBy;
                                        movAanvoer.SourceID = sourceId;
                                        movAanvoer.MovTime = new DateTime(1899, 12, 30).AddSeconds(movorder);
                                        movAanvoer.MovMutationBy = (int)LABELSConst.MutationBy.LNVVerblijfplaatsenCorrectie;
                                        movAanvoer.MovMutationDate = DateTime.Today;
                                        movAanvoer.MovMutationTime = DateTime.Now;

                                        db.UpdateMovement(movAanvoer);
                                    }

                                    foreach (var m in aanvoer.ToList())
                                        listMovements.Remove(m);

                                }
                            }

                            #endregion

                            if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum && ((movement.Bedrijfstype == "SP" && RedenEinde == "SL") || (RedenEinde == "ND" && verblijfplaatsen[verblijfplaatsen.Count - 1].Equals(movement))))
                            {
                                #region Dood
                                IEnumerable<MOVEMENT> dood = listMovements.Where(m => m.MovKind == (int)LABELSConst.MovementKind.DOOD && m.MovDate.Date == movement.AfvoerDatum.Date && m.UbnId == movementubn.UBNid);

                                if (lastDate < movement.AfvoerDatum)
                                {
                                    movorder = 1;
                                    lastDate = movement.AfvoerDatum;
                                }
                                else
                                    movorder++;

                                /////////////////////////////////////////
                                //Filtering meerdere movements op een dag.

                                int reservedDeath = 0;

                                //Hier dus filteren? 
                                for (int i = iVerblijfplaats + 1; i < verblijfplaatsen.Count; i++)
                                {
                                    if (verblijfplaatsen[i].AfvoerDatum == lastDate && verblijfplaatsen[i].UBN == movement.UBN && ((movement.Bedrijfstype == "SP" && RedenEinde == "SL") || (RedenEinde == "ND" && verblijfplaatsen[verblijfplaatsen.Count - 1].Equals(movement))))
                                        reservedDeath++;
                                }

                                if (reservedDeath >= dood.Count())
                                    dood = new List<MOVEMENT>();
                                else
                                    dood = dood.Take(dood.Count() - reservedDeath);
                                ////////////////////////////////////

                                var enDood = dood as MOVEMENT[] ?? dood.ToArray();
                                if (!enDood.Any())
                                {
                                    //geen movement gevonden
                                    mov = new MOVEMENT
                                    {
                                        MovMutationBy = 123,
                                        MovMutationDate = DateTime.Today,
                                        MovMutationTime = DateTime.Now,
                                        AniId = ani.AniId,
                                        MovDate = Einddatum,
                                        MovKind = 3,
                                        MovOrder = movorder,
                                        MovTime = new DateTime(1899, 12, 30).AddSeconds(movorder),
                                        UbnId = movementubn.UBNid,
                                        Progid = DierSoort,
                                        ReportDate = DateTime.Today,
                                        Changed_By = changedBy,
                                        SourceID = sourceId
                                    };

                                    db.SaveMovement(mov);
                                    allMovs.Add(mov);

                                    LOSS loss = new LOSS
                                    {
                                        MovId = mov.MovId,
                                        Changed_By = changedBy,
                                        SourceID = sourceId
                                    };

                                    unLogger.WriteDebug($"{lPrefixAnimal} MovDate: {mov.MovDate:dd-MM-yyyy} Bedrijfstype: {movement.Bedrijfstype} UBN: {movement.UBN} Doodmelding toevoegen.");

                                    db.SaveLoss(loss);
                                }
                                else
                                {
                                    //Movements gevonden
                                    if (warnForDuplicateMovements && enDood.Count() > 1)                                    
                                        unLogger.WriteWarn($"{lPrefixAnimal} MovDate: {movement.AanvoerDatum.Date:dd-MM-yyyy} Aantal dood Movements: {enDood.Count()}");
                                    

                                    foreach (MOVEMENT movDood in enDood.Where(m => m.MovOrder != movorder || m.MovTime != new DateTime(1899, 12, 30).AddSeconds(movorder)))
                                    {
                                        //Movement met verkeerde mov order of MovTime
                                        unLogger.WriteInfo($"{lPrefixAnimal} MovDate: {movDood.MovDate:dd-MM-yyyy} MovOrder {movDood.MovOrder} -> {movorder} MovTime {movDood.MovTime} -> {new DateTime(1899, 12, 30).AddSeconds(movorder)}");

                                        movDood.MovOrder = movorder;
                                        movDood.Changed_By = (int)changedBy;
                                        movDood.SourceID = sourceId;
                                        movDood.MovTime = new DateTime(1899, 12, 30).AddSeconds(movorder);
                                        movDood.MovMutationBy = (int)LABELSConst.MutationBy.LNVVerblijfplaatsenCorrectie;
                                        movDood.MovMutationDate = DateTime.Today;
                                        movDood.MovMutationTime = DateTime.Now;

                                        db.UpdateMovement(movDood);
                                    }

                                    foreach (var m in enDood.ToList())
                                        listMovements.Remove(m);

                                }
                                #endregion
                            }
                            else if (movement.AfvoerDatum != DateTime.MinValue)
                            {
                                #region Afvoer
                                IEnumerable<MOVEMENT> afvoer = listMovements.Where(m => (m.MovKind == (int)LABELSConst.MovementKind.AFVOER || m.MovKind == (int)LABELSConst.MovementKind.VERHUREN || m.MovKind == (int)LABELSConst.MovementKind.EINDEHUUR) && m.MovDate.Date == movement.AfvoerDatum.Date && m.UbnId == movementubn.UBNid);


                                if (lastDate < movement.AfvoerDatum)
                                {
                                    movorder = 1;
                                    lastDate = movement.AfvoerDatum;
                                }
                                else
                                    movorder++;


                                int reservedAfvoer = 0;

                                //Hier dus filteren? 
                                for (int i = iVerblijfplaats + 1; i < verblijfplaatsen.Count; i++)
                                {
                                    if (verblijfplaatsen[i].AfvoerDatum == lastDate && verblijfplaatsen[i].UBN == movement.UBN && !((movement.Bedrijfstype == "SP" && RedenEinde == "SL") || (RedenEinde == "ND" && verblijfplaatsen.First().Equals(movement))))
                                        reservedAfvoer++;
                                }

                                if (reservedAfvoer >= afvoer.Count())
                                    afvoer = new List<MOVEMENT>();
                                else
                                    afvoer = afvoer.Take(afvoer.Count() - reservedAfvoer);


                                var enAfvoer = afvoer as MOVEMENT[] ?? afvoer.ToArray();

                                if (!enAfvoer.Any())
                                {
                                    //geen movement gevonden
                                    mov = new MOVEMENT
                                    {
                                        MovMutationBy = 123,
                                        MovMutationDate = DateTime.Today,
                                        MovMutationTime = DateTime.Now,
                                        AniId = ani.AniId,
                                        MovDate = movement.AfvoerDatum,
                                        MovTime = new DateTime(1899, 12, 30).AddSeconds(movorder),
                                        MovKind = 2,
                                        MovOrder = movorder,
                                        UbnId = movementubn.UBNid,
                                        Progid = DierSoort,
                                        ReportDate = DateTime.Today,
                                        Changed_By = changedBy,
                                        SourceID = sourceId
                                    };

                                    db.SaveMovement(mov);
                                    allMovs.Add(mov);

                                    SALE sal = new SALE
                                    {
                                        MovId = mov.MovId,
                                        Changed_By = changedBy,
                                        SourceID = sourceId
                                    };

                                    //Slacht
                                    if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum && RedenEinde == "SL")
                                    {
                                        sal.SalKind = 2;
                                        sal.SalSlaughter = 1;
                                    }

                                    //Export
                                    if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum && RedenEinde == "EX")
                                    {
                                        sal.SalKind = 1;
                                    }

                                    unLogger.WriteDebug($"{lPrefixAnimal} MovDate: {mov.MovDate:dd-MM-yyyy} Bedrijfstype: {movement.Bedrijfstype} UBN: {movement.UBN} Afvoermelding toevoegen.");
                                    db.SaveSale(sal);
                                }
                                else
                                {
                                    //Movements gevonden
                                    if (warnForDuplicateMovements && enAfvoer.Count() > 2)
                                        unLogger.WriteWarn($"{lPrefixAnimal} MovDate: {movement.AfvoerDatum.Date:dd-MM-yyyy} Aantal afvoer Movements: {enAfvoer.Count()}");

                                    //2 afvoeren op een dag, kan 'gewoon'
                                    foreach (MOVEMENT movAfvoer in enAfvoer.Where(m => m.MovOrder != movorder || m.MovTime != new DateTime(1899, 12, 30).AddSeconds(movorder)))
                                    {
                                        //Movement met verkeerde mov order
                                        unLogger.WriteInfo($"{lPrefixAnimal} MovDate: {movAfvoer.MovDate:dd-MM-yyyy} MovOrder {movAfvoer.MovOrder} -> {movorder} MovTime: {movAfvoer.MovTime} -> {new DateTime(1899, 12, 30).AddSeconds(movorder)}");

                                        movAfvoer.MovOrder = movorder;
                                        movAfvoer.Changed_By = (int)changedBy;
                                        movAfvoer.SourceID = sourceId;
                                        movAfvoer.MovTime = new DateTime(1899, 12, 30).AddSeconds(movorder);
                                        movAfvoer.MovMutationBy = (int)LABELSConst.MutationBy.LNVVerblijfplaatsenCorrectie;
                                        movAfvoer.MovMutationDate = DateTime.Today;
                                        movAfvoer.MovMutationTime = DateTime.Now;

                                        db.UpdateMovement(movAfvoer);
                                    }

                                    foreach (var m in enAfvoer.ToList())
                                        listMovements.Remove(m);
                                }
                                #endregion
                            }
                            else if (movementubn.UBNid == requestingUBN.UBNid)
                            {
                                if (bufferedAniCats == null)
                                {
                                    unLogger.WriteError($"{lPrefixAnimal} bufferedAniCats == null.");
                                }
                                else
                                {
                                    if (Geslacht == "M" && anicat.Anicategory != 3)
                                    {
                                        var cat = bufferedAniCats.FirstOrDefault(x => x.Value == anicat);
                                        if (cat.Key == null)
                                        {
                                            unLogger.WriteError($"{lPrefixAnimal} cat.Key == null.");
                                        }
                                        else
                                        {
                                            cat.Key.ForceUpdate = true;
                                        }
                                        anicat.Anicategory = 3;
                                    }
                                    else if (Geslacht == "V" && anicat.Anicategory != 1)
                                    {             
                                        var cat = bufferedAniCats.FirstOrDefault(x => x.Value == anicat);
                                        if (cat.Key == null)
                                        {
                                            unLogger.WriteError($"{lPrefixAnimal} cat.Key == null.");
                                        }
                                        else
                                        {
                                            cat.Key.ForceUpdate = true;
                                        }
                                        anicat.Anicategory = 1;                                        
                                    }
                                }
                                
                            }
                        }
                    }
                }
                #endregion

                #endregion

                #region Verwijderen movements

                if (!forceerHistorisch)
                {
                    if (TestServer != 0)
                    {
                        unLogger.WriteDebug($"{lPrefixAnimal} test server, sla movements verwijderen over.");
                    }
                    else
                    {
                        var listMovsToDelete = listMovements.Where(m => m.MovDate < DateTime.Today.AddDays(minMovementDaysBeforeDelete)); // melding geblokkkeerd? 

                        if (listMovsToDelete.Any())
                            unLogger.WriteDebug($"{lPrefixAnimal} (Mogelijk) Aantal movements te verwijderen: {listMovsToDelete.Count()}.");

                        foreach (MOVEMENT delmov in listMovsToDelete)
                        {
                            if (IRUtils.DeleteMovement(db, delmov, LABELSConst.ChangedBy.LNVVerblijfplaatsenCorrectie, requestingUBN.UBNid))
                            {
                                if (!allMovs.Remove(delmov))
                                {
                                    unLogger.WriteWarn($"{lPrefixAnimal} Error Verplaatsing (kind: {delmov.MovKind}) op: {delmov.MovDate:dd-MM-yyyy} MovId: {delmov.MovId}. uit Lijst alle movements verwijderen.");
                                }
                            }
                        }
                    }                   
                }
                #endregion

                #region Set category


                int? newCat;
                if (forceerHistorisch)
                    newCat = (int)LABELSConst.AniCategory.AanwezigGeweest;
                else
                    newCat = IRUtils.GetAniCategoryFromMovements(allMovs, ani, requestingUBN, minMovementDaysBeforeDelete);

                //indien category niet gewijzigd
                if (bufferedAniCats != null)
                {
                    #region using cache
                    var kvp = IRUtils.GetAniCatFromCache(bufferedAniCats, anicat.FarmId, anicat.AniId);
                    if (kvp.Key == null || kvp.Key.FarmId != anicat.FarmId || kvp.Key.AniId != anicat.AniId)
                    {
                        unLogger.WriteError($"{lPrefixAnimal} Error ophalen AniCat - FarmId: {anicat.FarmId} AniId: {anicat.AniId} - Kan mogelijk niet updaten.");
                    }
                    else
                    {
                        // Als er geen movements zijn toegevoegd, maar de categorie toch goed gezet moet worden?

                       if (newCat.HasValue && UpdateAniCatRequired(newCat.Value, kvp.Key.OldAniCat))
                        {

                            if (forceerHistorisch)
                                unLogger.WriteInfo($"{lPrefixAnimal} CRV GeboorteDatum {ani.AniBirthDate:dd-MM-yyyy} is niet bekend bij RVO maar staat wel in Agrobase. Zet op '{newCat}'.");


                            string logCat = "Onbekend";
                            if (kvp.Key?.OldAniCat != null)
                                logCat = $"{(LABELSConst.AniCategory)kvp.Key?.OldAniCat}";

                            if (kvp.Key?.OldAniCat == newCat.Value)
                                unLogger.WriteInfo($"{lPrefixAnimal} Verander category {logCat} -> {(LABELSConst.AniCategory)kvp.Value.Anicategory} -> {(LABELSConst.AniCategory)newCat.Value}.");
                            else
                                unLogger.WriteInfo($"{lPrefixAnimal} Verander category {logCat} -> {(LABELSConst.AniCategory)newCat.Value}.");

                            if (!SetAniCat(ani.AniId, newCat.Value, anicat.AniWorknumber, farmIds, bufferedAniCats, changedBy, sourceId))
                            {
                                //error tijdens anicat zetten?
                                unLogger.WriteError($"{lPrefixAnimal} error tijdens updaten animalcategory.");
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Direct to DB

                    if (newCat.HasValue && newCat.Value != anicat.Anicategory)
                    {
                        if (forceerHistorisch)
                            unLogger.WriteInfo($"[LEGACY] {lPrefixAnimal} CRV GeboorteDatum {ani.AniBirthDate:dd-MM-yyyy} is niet bekend bij RVO maar staat wel in Agrobase. Zet op '{newCat}'.");

                        unLogger.WriteInfo($"[LEGACY] {lPrefixAnimal} Verander category {anicat.Anicategory} -> {newCat.Value}.");

                        anicat.Anicategory = newCat.Value;
                        ani.Changed_By = changedBy;
                        ani.SourceID = sourceId;

                        unLogger.WriteDebug($"[LEGACY] {lPrefixAnimal} Opslaan AnimalCategory.");
                        db.SaveAnimalCategory(anicat);
                    }
                    #endregion
                }

                #endregion

                #region vrijgeven transmitter
                //voor dieren die geforceerd op historisch zijn gezet, wel de transmitters vrijgeven. Ze kunnen toch niet gebruikt worden en anders komt
                //kun je deze nooit meer ontkoppelen.

                if (bufferedAniCats == null)
                {
                    if (anicat.Anicategory > 3)
                    {
                        unLogger.WriteDebug($"{lPrefixAnimal} Vrijgeven transmiters.");

                        //Transmitter vrijgeven
                        returnTransmitters(db, farm, ani.AniId);
                    }
                }
                else
                {
                    //Hoeft alleen vrij te geven als het dier voorheen op het bedrijf stond.
                    var bc = bufferedAniCats.Keys.FirstOrDefault(bac => bac.OldAniCat.HasValue && bac.OldAniCat.Value <= 3);
                    if (bc != null && bc.AniId == anicat.AniId && anicat.Anicategory > 3)
                    {
                        unLogger.WriteDebug($"{lPrefixAnimal} Vrijgeven transmiters.");

                        //Transmitter vrijgeven
                        returnTransmitters(db, farm, ani.AniId);
                    }
                }

                #endregion

                #region Udaten animal categories

                if (bufferedAniCats != null)
                {
                    if (farmIds == null)
                    {
                        string msg = $"{lPrefixAnimal} FarmId: {key.FarmId} - Kan niet aanroepen met buffer maar zonder FarmIds";
                        unLogger.WriteError(msg);
                        throw new ArgumentException(msg);
                    }

                    foreach (int i in farmIds)
                    {
                        bool bAdd = false;
                        key = null;

                        var c = bufferedAniCats.FirstOrDefault(fi => fi.Key.AniId == ani.AniId && fi.Key.FarmId == i);
                        if (c.Key != null && c.Key.AniId > 0)
                        {
                            if (c.Key.AniId == ani.AniId)
                                key = c.Key;
                        };
                        ANIMALCATEGORY ac2 = null;
                        if (key != null)
                        {
                            if (!bufferedAniCats.TryGetValue(key, out ac2))
                                unLogger.WriteError($"{lPrefixAnimal} Could not get aniCategory from buffer! AniId: {key.AniId} FarmId: {key.FarmId}");
                        }
                        else
                        {
                            key = new AniCatKey(i, ani.AniId);
                            ac2 = new ANIMALCATEGORY();
                            bAdd = true;
                        }

                        ac2.AniId = anicat.AniId;
                        ac2.AniWorknumber = anicat.AniWorknumber;
                        ac2.Anicategory = anicat.Anicategory;
                        ac2.FarmId = i;
                        ac2.Changed_By = changedBy;
                        ac2.SourceID = sourceId;

                        if (bAdd)
                            bufferedAniCats.Add(key, ac2);
                    }
                }    
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                string msg = $"{lPrefixAnimal} Ex: {ex.Message}";
                unLogger.WriteError(msg, ex);
                return false;
            }
            finally
            {
                sw.Stop();
                unLogger.WriteTrace($"{lPrefixAnimal} Category: {anicat?.Anicategory} Total elapsed: {sw.Elapsed}");
            }
        }

        private static bool UpdateAniCatRequired(int cat1, int? cat2)
        {
            if (!cat2.HasValue)
                return true;

            //Bv aanwezig -> meststier is nutteloos
            if (cat1 >= 1 && cat1 <= 3 && cat2.Value >= 1 && cat2.Value <= 3)
                return false;

            return cat1 != cat2.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pMstb"></param>
        /// <param name="pBedrijf"></param>
        /// <param name="pAniID"></param>
        private static void returnTransmitters(AFSavetoDB pMstb, BEDRIJF pBedrijf, int pAniID)
        {
            //Het transmitternummer moet weer in de voorraad gezet worden als een dier van het bedrijf wordt afgevoerd.
            if (pBedrijf.FarmId > 0 && pAniID > 0)
            {
                ANIMAL a = pMstb.GetAnimalById(pAniID);
                UBN u = pMstb.GetubnById(pBedrijf.UBNid);
                List<TRANSMIT> trans = pMstb.GetTransmitsByFarmId(pBedrijf.FarmId);
                var respondersanimal = (from n in trans where n.AniId == pAniID select n).ToList();

                if (!respondersanimal.Any())
                {
                    unLogger.WriteTrace($"{nameof(OpvragenLNVDierDetailsV2)}{nameof(returnTransmitters)} UBN: {u.Bedrijfsnummer} Dier: '{a.AniAlternateNumber}' - niets verwijderd, geen responders gevonden.");
                }

                foreach (TRANSMIT tr in respondersanimal)
                {
                    TRANSMIT mbg = tr;//ivm delete in een loop     ML: uhh wat?                    

                    mbg.UbnID = pBedrijf.UBNid;
                    mbg.farmid = pBedrijf.FarmId;
                    mbg.FarmNumber = u.Bedrijfsnummer;

                    if (pMstb.DeleteTransmit(mbg))//zet hem automatisch weer in de voorraad
                    {
                        unLogger.WriteDebug($"{nameof(OpvragenLNVDierDetailsV2)}{nameof(returnTransmitters)} UBN: {u.Bedrijfsnummer} Dier: '{a.AniAlternateNumber}' - Transmitter verwijderd " + tr.TransmitterNumber);
                    }
                    else
                    {
                        unLogger.WriteError($"{nameof(OpvragenLNVDierDetailsV2)}{nameof(returnTransmitters)} UBN: {u.Bedrijfsnummer} Dier: '{a.AniAlternateNumber}' - niet verwijderd: fout DeleteTransmit ");
                    }
                }
            }
        }

        public static bool VulMovementsvanuitLNV(AFSavetoDB DB, BEDRIJF farm, String pUsername, String pPassword, ANIMAL ani, String Lifenr)
        {
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

            List<SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen = new List<SOAPLNVDieren.Dierverblijfplaats>();

            SOAPLNVDieren soapdieren = new SOAPLNVDieren();
            soapdieren.LNVDierdetailsV2(pUsername, pPassword, 0,
                UBNhouder, String.Empty,
                Lifenr, farm.ProgId,
                1, 0, 0,
                ref Werknummer, ref Geboortedat,
                ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                ref LevensnrMoeder,
                ref VervangenLevensnr,
                ref Verblijfplaatsen,
                ref Status, ref Code, ref Omschrijving);
            // meer info ophalen
            if (ani.AniAlternateNumber == String.Empty)
                ani.AniAlternateNumber = Lifenr;
            if (ani.AniLifeNumber == String.Empty)
                ani.AniLifeNumber = Lifenr;
            if (farm.ProgId == 1 && Haarkleur != null && Haarkleur != String.Empty) ani.AniHaircolor_Memo = Haarkleur;// Anihaircolor = Facade.getSaveToDB(pToken).GetHcoId(Haarkleur);
            if (Geboortedat != DateTime.MinValue && Geboortedat.Date != new DateTime(1950, 01, 01))
                ani.AniBirthDate = Geboortedat;
            if (Geslacht == "M")
                ani.AniSex = 1;
            else if (Geslacht == "V")
                ani.AniSex = 2;
            if (LevensnrMoeder != String.Empty)
            {
                ANIMAL aniMother = DB.GetAnimalByAniAlternateNumber(LevensnrMoeder);
                if (aniMother.AniId == 0)
                {
                    VulMovementsvanuitLNV(DB, farm, pUsername, pPassword, aniMother, LevensnrMoeder);
                }
                if (ani.AniIdDam == 0)
                {
                    ani.AniIdMother = aniMother.AniId;
                }
            }
            ani.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
            ani.SourceID = farm.UBNid;
            DB.SaveAnimal(-99, ani);
            ANIMALCATEGORY anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);

            if (anicat.FarmId == 0)
            {
                anicat.AniId = ani.AniId;
                anicat.AniWorknumber = Werknummer;
                anicat.FarmId = farm.FarmId;
                anicat.Anicategory = 5;
                anicat.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                anicat.SourceID = farm.UBNid;
                DB.SaveAnimalCategory(anicat);
                //Roundtrip naar de server om key violation bij latere update te voorkomen
                anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
            }
            // Dieren die niet bij LNV zijn gevonden naar nooit aanwezig geweest zetten             
            else if (anicat.Anicategory <= 3 && Status == "F" && Code == "IRD-00192" && ani.AniBirthDate < DateTime.Today.AddDays(-90))
            {
                anicat.Anicategory = 5;
                ani.ThrId = 0; // Fokker op onbekend zetten want dier is niet bekend bij LNV
                anicat.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                anicat.SourceID = farm.UBNid;
                DB.SaveAnimalCategory(anicat);
                return false;
            }
            else if (Status == "F")
            {
                return false;
            }

            MOVEMENT mov;
            Verblijfplaatsen.Reverse();
            int movorder = 1;
            DateTime LastDate = DateTime.MinValue;
            foreach (SOAPLNVDieren.Dierverblijfplaats movement in Verblijfplaatsen)
            {
                UBN movementubn = DB.getUBNByBedrijfsnummer(movement.UBN);
                if (movementubn.UBNid > 0)
                {
                    if (movement.AanvoerDatum != Geboortedat)
                    {
                        if (LastDate < movement.AanvoerDatum)
                        {
                            movorder = 1;
                            LastDate = movement.AanvoerDatum;
                        }
                        else
                            movorder++;
                        mov = DB.GetMovementByDateAniIdKind(movement.AanvoerDatum, ani.AniId, 1, movementubn.UBNid);
                        if (mov.MovId == 0)
                        {
                            mov.MovMutationBy = 107;
                            mov.MovMutationDate = DateTime.Today;
                            mov.MovMutationTime = DateTime.Now;
                            mov.AniId = ani.AniId;
                            mov.MovDate = movement.AanvoerDatum;
                            mov.MovKind = 1;
                            mov.MovOrder = movorder;
                            mov.UbnId = movementubn.UBNid;
                            mov.Progid = farm.ProgId;
                            mov.ReportDate = DateTime.Today;
                            mov.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            mov.SourceID = farm.UBNid;
                            DB.SaveMovement(mov);
                            BUYING buy = new BUYING();
                            buy.MovId = mov.MovId;
                            buy.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            buy.SourceID = farm.UBNid;
                            DB.SaveBuying(buy);
                        }
                    }
                    //else if (ani.ThrId == 0 && ubnto.UBNid == farm.UBNid)
                    else if (movementubn.ThrID > 0 && movementubn.UBNid == farm.UBNid && ani.ThrId != movementubn.ThrID)
                    {
                        ani.ThrId = movementubn.ThrID;
                        DB.UpdateANIMAL(movementubn.ThrID, ani);
                    }
                    if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum &&
                        ((movement.Bedrijfstype == "SP" && RedenEinde == "SL") ||
                        (RedenEinde == "ND" && Verblijfplaatsen.Last().Equals(movement))))
                    {
                        if (LastDate < movement.AfvoerDatum)
                        {
                            movorder = 1;
                            LastDate = movement.AfvoerDatum;
                        }
                        else
                            movorder++;
                        mov = DB.GetMovementByDateAniIdKind(movement.AfvoerDatum, ani.AniId, 3, movementubn.UBNid);
                        if (mov.MovId == 0 && movementubn.UBNid > 0)
                        {
                            mov.MovMutationBy = 107;
                            mov.MovMutationDate = DateTime.Today;
                            mov.MovMutationTime = DateTime.Now;
                            mov.AniId = ani.AniId;
                            mov.MovDate = movement.AfvoerDatum;
                            mov.MovKind = 3;
                            mov.MovOrder = movorder;
                            mov.UbnId = movementubn.UBNid;
                            mov.Progid = farm.ProgId;
                            mov.ReportDate = DateTime.Today;
                            mov.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            mov.SourceID = farm.UBNid;
                            DB.SaveMovement(mov);
                            LOSS sal = new LOSS();
                            sal.MovId = mov.MovId;
                            sal.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            sal.SourceID = farm.UBNid;
                            DB.SaveLoss(sal);
                            if (movementubn.UBNid == farm.UBNid)
                            {
                                anicat.Anicategory = 4;
                                DB.SaveAnimalCategory(anicat);
                            }
                        }
                    }
                    else if (movement.AfvoerDatum != DateTime.MinValue)
                    {
                        if (LastDate < movement.AfvoerDatum)
                        {
                            movorder = 1;
                            LastDate = movement.AfvoerDatum;
                        }
                        else
                            movorder++;
                        mov = DB.GetMovementByDateAniIdKind(movement.AfvoerDatum, ani.AniId, 2, movementubn.UBNid);
                        if (mov.MovId == 0 && movementubn.UBNid > 0)
                        {
                            mov.MovMutationBy = 107;
                            mov.MovMutationDate = DateTime.Today;
                            mov.MovMutationTime = DateTime.Now;
                            mov.AniId = ani.AniId;
                            mov.MovDate = movement.AfvoerDatum;
                            mov.MovKind = 2;
                            mov.MovOrder = movorder;
                            mov.UbnId = movementubn.UBNid;
                            mov.Progid = farm.ProgId;
                            mov.ReportDate = DateTime.Today;
                            mov.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            mov.SourceID = farm.UBNid;
                            DB.SaveMovement(mov);
                            SALE sal = new SALE();
                            if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum && RedenEinde == "SL")
                            {
                                sal.SalKind = 2;
                                sal.SalSlaughter = 1;
                            }
                            sal.MovId = mov.MovId;
                            sal.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            sal.SourceID = farm.UBNid;
                            DB.SaveSale(sal);

                            if (movementubn.UBNid == farm.UBNid)
                            {
                                anicat.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                                anicat.SourceID = farm.UBNid;
                                anicat.Anicategory = 4;
                                DB.SaveAnimalCategory(anicat);
                            }
                        }
                    }
                    else if (movementubn.UBNid == farm.UBNid)
                    {
                        anicat.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                        anicat.SourceID = farm.UBNid;
                        if (Geslacht == "M")
                            anicat.Anicategory = 3;
                        else if (Geslacht == "V")
                            anicat.Anicategory = 1;
                        DB.SaveAnimalCategory(anicat);

                    }
                }
            }
            if (anicat.Anicategory > 3)
            {
                returnTransmitters(DB, farm, ani.AniId);
            }
            return true;
        }

        public static void VulMovementsvanuitLNV(IFacade Facade, DBConnectionToken pToken, BEDRIJF farm, ANIMAL ani, String Lifenr)
        {
            var DB = Facade.getSaveToDB(pToken);
            String lUsername = "";
            String lPassword = "";
            FTPUSER fusoap = DB.GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9992);
            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }
            else
            {
                Masking m = new Masking(); 
                lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"];
                lPassword = m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);        
            }
            VulMovementsvanuitLNV(DB, farm, lUsername, lPassword, ani, Lifenr);
        }

        public static string GetDraagmoedervanuitLNV(int pPrognr, string LevensnrCalf)
        {
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
            string Status = String.Empty;
            string Code = String.Empty;
            string Omschrijving = String.Empty;
            String lUsername, lPassword;
            Masking m = new Masking();
            lUsername =  ConfigurationManager.AppSettings["LNVDierDetailsusername"];
            lPassword = m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]); 

            SOAPLNVDieren soapdieren = new SOAPLNVDieren();
            soapdieren.LNVDierDetailstatusV2(lUsername, lPassword, 0, LevensnrCalf, pPrognr,
                ref BRSnrHouder, ref UBNhouder, ref Werknummer, ref Geboortedat,
                ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                ref LevensnrMoeder,
                ref Status, ref Code, ref Omschrijving);
            return LevensnrMoeder;

        }


        public delegate int GetMovementOrder(ANIMAL pAnimal, MOVEMENT mov);

        public static bool VerblijfplaatsenBijwerken(AFSavetoDB DB, int ProgId, ANIMAL ani, String Lifenr, GetMovementOrder MovOrderFunc)
        {
            string lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"];
            Masking m = new Masking();
            string lPassword = m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
            //const string lPassword =  pw; use of const is not possible with DeCodeer_String
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

            List<SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen = new List<SOAPLNVDieren.Dierverblijfplaats>();

            SOAPLNVDieren soapdieren = new SOAPLNVDieren();
            soapdieren.LNVDierdetailsV2(lUsername, lPassword, 0,
                UBNhouder, String.Empty,
                Lifenr, ProgId,
                1, 0, 0,
                ref Werknummer, ref Geboortedat,
                ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                ref LevensnrMoeder,
                ref VervangenLevensnr,
                ref Verblijfplaatsen,
                ref Status, ref Code, ref Omschrijving);


            bool UpdateAnimalData;

            if (ani.AniId > 0)
                UpdateAnimalData = false;
            else if (Status == "F" && Code == "IRD-00192" && ani.AniBirthDate < DateTime.Today.AddDays(-90))
            {
                // Dieren die niet bij LNV zijn gevonden niet aanmaken in agrobase
                return false;
            }
            else
            {
                UpdateAnimalData = true;
            }


            // meer info ophalen
            if (ani.AniAlternateNumber == String.Empty)
            {
                ani.AniAlternateNumber = Lifenr;
                UpdateAnimalData = true;
            }
            if (ani.AniLifeNumber == String.Empty)
            {
                ani.AniLifeNumber = Lifenr;
                UpdateAnimalData = true;
            }
            if (ProgId == 1 && Haarkleur != null && Haarkleur != String.Empty && ani.AniHaircolor_Memo != Haarkleur)
            {
                ani.AniHaircolor_Memo = Haarkleur;
                ani.Anihaircolor = DB.GetHcoId(Haarkleur);
                UpdateAnimalData = true;
            }

            // luc 26-08-15 dieren met datum 01-01-1950 datum niet overschijven, standaard rvo datum indien werkelijke geboortedatum niet bekend
            if (Geboortedat != DateTime.MinValue && Geboortedat.Date != new DateTime(1950, 01, 01) && ani.AniBirthDate != Geboortedat)
            {
                ani.AniBirthDate = Geboortedat;
                UpdateAnimalData = true;
            }

            int Sex = 0;
            if (Geslacht == "M")
                Sex = 1;
            else if (Geslacht == "V")
                Sex = 2;
            if (ani.AniSex != Sex && Sex != 0)
            {
                ani.AniSex = Sex;
                UpdateAnimalData = true;
            }
            if (LevensnrMoeder != String.Empty)
            {
                if (ani.AniIdDam == 0)
                {
                    ANIMAL aniMother = DB.GetAnimalByAniAlternateNumber(LevensnrMoeder);
                    if (aniMother.AniId > 0 && ani.AniIdMother != aniMother.AniId)
                    {
                        VerblijfplaatsenBijwerken(DB, ProgId, aniMother, LevensnrMoeder, MovOrderFunc);
                        ani.AniIdMother = aniMother.AniId;
                        UpdateAnimalData = true;
                    }
                }
            }
            if (Status == "F")
            {
                return false;
            }
            else if (UpdateAnimalData)
            {
                ani.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                ani.SourceID = -1;
                DB.SaveAnimal(-99, ani);
            }
            MOVEMENT mov;
            Verblijfplaatsen.Reverse();
            foreach (SOAPLNVDieren.Dierverblijfplaats movement in Verblijfplaatsen)
            {
                UBN movementubn = DB.getUBNByBedrijfsnummer(movement.UBN);
                if (movementubn.UBNid > 0)
                {

                    foreach (var bedr in DB.getBedrijvenByUBNId(movementubn.UBNid).Where(bedr => bedr.ProgId == ProgId))
                    {
                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, bedr.FarmId);
                        if (ac.FarmId > 0) continue;
                        try
                        {
                            ac.FarmId = bedr.FarmId;
                            ac.AniId = ani.AniId;
                            if (movement.AfvoerDatum != DateTime.MinValue) ac.Anicategory = (int)LABELSConst.AniCategory.Aanwezig;
                            else ac.Anicategory = (int)LABELSConst.AniCategory.AanwezigGeweest;
                            ac.AniWorknumber = Werknummer;
                            ac.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            ac.SourceID = -2;
                            DB.SaveAnimalCategory(ac);
                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteErrorFormat("IRMUT", "LNVVerblijfplaatsen Fout bij aanmaken AnimalCategory : {0}", ex);
                        }
                    }

                    if (movement.AanvoerDatum == Geboortedat)
                    {
                        if (movementubn.ThrID > 0 && ani.ThrId != movementubn.ThrID)
                        {
                            ani.ThrId = movementubn.ThrID;
                            ani.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            ani.SourceID = -2;
                            DB.UpdateANIMAL(movementubn.ThrID, ani);


                        }
                        //BIRTH bir = db.GetBirthByCalfId(ani.AniId);
                        //if (bir.EventId == 0 )
                        //{  
                        //        EVENT eve;
                        //    if(ani.AniIdDam > 0 )
                        //        eve = getEventByDateAniIdKind(db, ani.AniIdDam, Geboortedat);
                        //    else if(ani.AniIdMother > 0)
                        //        eve = getEventByDateAniIdKind(db, ani.AniIdMother, Geboortedat);
                        //    else continue;


                        //    eve.EveMutationBy = 107;
                        //    eve.EveMutationDate = DateTime.Today;
                        //    eve.EveMutationTime = DateTime.Now;
                        //    eve.AniId = ani.AniId;
                        //    eve.EveDate = movement.AanvoerDatum;
                        //    eve.EveKind = (int)LABELSConst.EventKind.AFKALVEN;
                        //    db.SaveEvent(eve);
                        //    bir.EventId = eve.EventId;
                        //    bir.CalfId = ani.AniId;
                        //    db.SaveBirth(bir);
                        //}
                    }
                    else
                    {
                        mov = DB.GetMovementByDateAniIdKind(movement.AanvoerDatum, ani.AniId, 1, movementubn.UBNid);
                        if (mov.MovId == 0)
                        {
                            mov.MovMutationBy = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            mov.MovMutationDate = DateTime.Today;
                            mov.MovMutationTime = DateTime.Now;
                            mov.AniId = ani.AniId;
                            mov.MovDate = movement.AanvoerDatum;
                            mov.MovKind = (int)LABELSConst.MovementKind.AANVOER;
                            mov.UbnId = movementubn.UBNid;
                            mov.Progid = ProgId;
                            mov.ReportDate = DateTime.Today;
                            mov.MovOrder = MovOrderFunc(ani, mov);
                            mov.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            DB.SaveMovement(mov);
                            BUYING buy = new BUYING();
                            buy.MovId = mov.MovId;
                            buy.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            DB.SaveBuying(buy);
                        }
                    }

                    if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum &&
                        ((movement.Bedrijfstype == "SP" && RedenEinde == "SL") ||
                        (RedenEinde == "ND" && Verblijfplaatsen.Last().Equals(movement))))
                    {
                        mov = DB.GetMovementByDateAniIdKind(movement.AfvoerDatum, ani.AniId, 3, movementubn.UBNid);
                        if (mov.MovId == 0 && movementubn.UBNid > 0)
                        {
                            mov.MovMutationBy = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            mov.MovMutationDate = DateTime.Today;
                            mov.MovMutationTime = DateTime.Now;
                            mov.AniId = ani.AniId;
                            mov.MovDate = movement.AfvoerDatum;
                            mov.MovKind = (int)LABELSConst.MovementKind.DOOD;
                            mov.UbnId = movementubn.UBNid;
                            mov.Progid = ProgId;
                            mov.ReportDate = DateTime.Today;
                            mov.MovOrder = MovOrderFunc(ani, mov);
                            mov.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            DB.SaveMovement(mov);
                            LOSS sal = new LOSS();
                            sal.MovId = mov.MovId;
                            sal.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            DB.SaveLoss(sal);
                        }
                    }
                    else if (movement.AfvoerDatum != DateTime.MinValue)
                    {

                        mov = DB.GetMovementByDateAniIdKind(movement.AfvoerDatum, ani.AniId, 2, movementubn.UBNid);
                        if (mov.MovId == 0 && movementubn.UBNid > 0)
                        {
                            mov.MovMutationBy = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            mov.MovMutationDate = DateTime.Today;
                            mov.MovMutationTime = DateTime.Now;
                            mov.AniId = ani.AniId;
                            mov.MovDate = movement.AfvoerDatum;
                            mov.MovKind = (int)LABELSConst.MovementKind.AFVOER;
                            mov.UbnId = movementubn.UBNid;
                            mov.Progid = ProgId;
                            mov.ReportDate = DateTime.Today;
                            mov.MovOrder = MovOrderFunc(ani, mov);
                            mov.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            DB.SaveMovement(mov);
                            SALE sal = new SALE();
                            if (Einddatum != DateTime.MinValue && Einddatum == movement.AfvoerDatum && RedenEinde == "SL")
                            {
                                sal.SalKind = 2;
                                sal.SalSlaughter = 1;
                            }
                            sal.MovId = mov.MovId;
                            sal.Changed_By = (int)LABELSConst.MutationBy.LNVVerblijfplaatsen;
                            DB.SaveSale(sal);
                        }
                    }
                }
            }
            return true;
        }

        //private static EVENT getEventByDateAniIdKind(AFSavetoDB db, int aniIdMother, DateTime Geboortedat)
        //{
        //    EVENT eve;
        //    List<EVENT> events = db.getEventsByDateAniIdKind(Geboortedat, aniIdMother, (int)LABELSConst.EventKind.AFKALVEN);

        //    return eve;
        //}


        public static void VulDiergegevensvanuitLNV(IFacade Facade, DBConnectionToken pToken, BEDRIJF farm, ANIMAL ani, String Lifenr)
        {
            var DB = Facade.getSaveToDB(pToken);
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
            string Status = String.Empty;
            string Code = String.Empty;
            string Omschrijving = String.Empty;
            string pLogfile = String.Empty;
            int pMaxStrLen = 255;
            String lUsername, lPassword;
            Masking m = new Masking();
            FTPUSER fusoap = DB.GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9992);
            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }
            else
            {
                lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"];
                lPassword = m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);  
            }
            //Controle op  omgenummerd dier
            string pVervangenLevensnr = "";
            bool GoOn = checkVervangenLevensnr(Facade, pToken, farm, ani, Lifenr, lUsername, lPassword, out pVervangenLevensnr);
            if (ani.AniId <= 0 && GoOn)
            {


                SOAPLNVDieren soapdieren = new SOAPLNVDieren();
                soapdieren.LNVDierDetailstatusV2(lUsername, lPassword, 0, Lifenr, farm.ProgId,
                    ref BRSnrHouder, ref UBNhouder, ref Werknummer, ref Geboortedat,
                    ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                    ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                    ref LevensnrMoeder,
                    ref Status, ref Code, ref Omschrijving);//,pLogfile, pMaxStrLen);

                // meer info ophalen
                ani.AniAlternateNumber = Lifenr;
                ani.AniLifeNumber = Lifenr;
                if (farm.ProgId == 1) ani.Anihaircolor = DB.GetHcoId(Haarkleur);
                if (Geboortedat != DateTime.MinValue && Geboortedat.Date != new DateTime(1950, 01, 01))
                    ani.AniBirthDate = Geboortedat;
                if (Geslacht == "M")
                    ani.AniSex = 1;
                else if (Geslacht == "V")
                    ani.AniSex = 2;
                if (LevensnrMoeder != String.Empty)
                {
                    if (ani.AniIdDam == 0)
                    {
                        ANIMAL aniMother = DB.GetAnimalByAniAlternateNumber(LevensnrMoeder);
                        if (aniMother.AniId == 0)
                        {
                            VulDiergegevensvanuitLNV(Facade, pToken, farm, aniMother, LevensnrMoeder);
                        }

                        ani.AniIdMother = aniMother.AniId;
                    }
                }
                DB.SaveAnimal(-99, ani);
                ANIMALCATEGORY anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                if (anicat.FarmId == 0)
                {
                    anicat.AniId = ani.AniId;
                    anicat.AniWorknumber = Werknummer;
                    anicat.FarmId = farm.FarmId;
                    anicat.Anicategory = 5;
                    DB.SaveAnimalCategory(anicat);
                }
            }
        }

        private static bool checkVervangenLevensnr(IFacade Facade, DBConnectionToken pToken, BEDRIJF farm, ANIMAL ani, String Lifenr, string pUsername, string pPassword, out string pVervangenLevensnr)
        {
            bool ret = true;
            AFSavetoDB DB = Facade.getSaveToDB(pToken);
            UBN u = DB.GetubnById(farm.UBNid);
            THIRD lPersoon = DB.GetThirdByThirId(u.ThrID);
            String BRSnr = lPersoon.Thr_Brs_Number;
            pVervangenLevensnr = "";
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
            string pLogfile = String.Empty;

            SOAPLNVDieren soapdieren = new SOAPLNVDieren();
            soapdieren.LNVDierdetailsV2(pUsername, pPassword, 0,
                                        u.Bedrijfsnummer, BRSnr, Lifenr, farm.ProgId,
                                        0, 0, 0,
                                        ref Werknummer,
                                        ref Geboortedat, ref Importdat,
                                        ref LandCodeHerkomst, ref LandCodeOorsprong,
                                        ref Geslacht, ref Haarkleur,
                                        ref Einddatum, ref RedenEinde,
                                        ref LevensnrMoeder, ref VervangenLevensnr,
                                        ref Status, ref Code, ref Omschrijving);

            if (VervangenLevensnr != null && VervangenLevensnr.Length > 0)
            {
                pVervangenLevensnr = VervangenLevensnr;
                List<LEVNRMUT> muts = DB.getLevNrMutsByLifeNr(VervangenLevensnr);
                if (muts.Any() && muts.ElementAt(0).Aniid > 0)
                {
                    ani = DB.GetAnimalById(muts.ElementAt(0).Aniid);
                }
                else
                {
                    ANIMAL checkAnimal = DB.GetAnimalByAniAlternateNumber(VervangenLevensnr);
                    if (farm.ProgId != 3 && farm.ProgId != 5)
                    {

                        checkAnimal = DB.GetAnimalByLifenr(VervangenLevensnr);
                    }

                    if (VervangenLevensnr == Lifenr)
                    {
                        if (checkAnimal.AniId > 0)
                        {
                            LEVNRMUT nM = new LEVNRMUT();
                            nM.Aniid = checkAnimal.AniId;
                            nM.LevnrOud = VervangenLevensnr;
                            DB.saveLevNrMut(nM);
                            ani = checkAnimal;
                        }
                        else
                        {
                            //Het dier is omgenummerd maar wij weten niet naar welk dier
                            //dan gaan we niet een nieuw dier aanmaken maar mag je niet verder gaan
                            //dan moetde klant doorgaan met opvragen 
                            //totdat VervangenLevensnr != lifenr
                            //Dan zie hieronder
                            ret = false;
                        }

                    }
                    else
                    {


                        if (checkAnimal.AniId > 0)//Dit dier is Omgenummerd naar pLifeNumber bij LNV
                        {
                            string lnvOmnummerNr = "via VervangenLevensnr";
                            List<LEVNRMUT> lMuts = DB.getLevNrMuts(checkAnimal.AniId);
                            if (lMuts.Any())
                            {
                                lnvOmnummerNr = lMuts.ElementAt(0).LNVMeldNr;
                            }

                            NummerDierOm(DB, farm, u, checkAnimal, Lifenr, lnvOmnummerNr);
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
                DateTime mindate = new DateTime(1900, 1, 1);
                if (Geboortedat > mindate)
                {
                    ani.AniAlternateNumber = Lifenr;
                    ani.AniLifeNumber = Lifenr;
                    if (farm.ProgId == 1) ani.Anihaircolor = DB.GetHcoId(Haarkleur);
                    if (Geboortedat != DateTime.MinValue && Geboortedat.Date != new DateTime(1950, 01, 01))
                        ani.AniBirthDate = Geboortedat;
                    if (Geslacht.ToLower() == "m")
                        ani.AniSex = 1;
                    else if (Geslacht.ToLower() == "v")
                        ani.AniSex = 2;
                    if (LevensnrMoeder != String.Empty)
                    {
                        ANIMAL aniMother = DB.GetAnimalByAniAlternateNumber(LevensnrMoeder);
                        if (aniMother.AniId == 0)
                        {
                            VulDiergegevensvanuitLNV(Facade, pToken, farm, aniMother, LevensnrMoeder);
                        }
                        if (ani.AniIdDam == 0)
                        {
                            ani.AniIdMother = aniMother.AniId;
                        }
                    }
                    DB.SaveAnimal(-99, ani);
                    ANIMALCATEGORY anicat = DB.GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
                    if (anicat.FarmId == 0)
                    {
                        anicat.AniId = ani.AniId;
                        anicat.AniWorknumber = Werknummer;
                        anicat.FarmId = farm.FarmId;
                        anicat.Anicategory = 5;
                        DB.SaveAnimalCategory(anicat);
                    }
                    if (VervangenLevensnr.Length > 0)
                    {
                        List<LEVNRMUT> muts = DB.getLevNrMutsByLifeNr(VervangenLevensnr);
                        if (muts.Any() && muts.ElementAt(0).Aniid > 0)
                        {

                        }
                        else
                        {
                            LEVNRMUT lM = new LEVNRMUT();
                            lM.Aniid = ani.AniId;
                            lM.LevnrOud = VervangenLevensnr;
                            lM.LevnrNieuw = ani.AniAlternateNumber;
                            DB.saveLevNrMut(lM);
                        }
                    }
                    ret = false;
                }
            }
            return ret;
        }

        private static void NummerDierOm(AFSavetoDB DB, BEDRIJF pBedrijf, UBN pUbn, ANIMAL pAnimal, string pNewLifeNumber, string pLNVMeldNr)
        {
            string lOldChipnr = pAnimal.AniLifeNumber;
            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                lOldChipnr = pAnimal.AniAlternateNumber;
            }
            if (lOldChipnr.Length > 5)
            {
                //List<LIFENR> LIFENRnummers = lMstb.GetLifenummersByFarmId(pBedrijf.FarmId);

                //bool iseralin = false;
                //foreach (LIFENR ltemp in LIFENRnummers)
                //{
                //    if (lOldChipnr.Contains(ltemp.LifLifenr.Trim()))
                //    {
                //        iseralin = true;
                //        break;
                //    }
                //}
                //if (!iseralin)
                //{


                //    LIFENR lf = new LIFENR();
                //    lf.LifCountrycode = lOldChipnr.Substring(0, 3);
                //    lf.LifLifenr = lOldChipnr.Substring(3, lOldChipnr.Length - 3);
                //    lf.FarmNumber = pUbn.Bedrijfsnummer;
                //    lf.LifId = pBedrijf.FarmId;
                //    lf.Program = pBedrijf.Programid;

                //    int start = lOldChipnr.Length - 5;
                //    lf.LifSort = lOldChipnr.Substring(start, 4);
                //    //lMstb.InsertLifenr(lf);

                //}

            }
            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                pAnimal.AniAlternateNumber = pNewLifeNumber;
                if (pAnimal.AniLifeNumber == lOldChipnr)
                {
                    pAnimal.AniLifeNumber = pNewLifeNumber;
                }
            }
            else
            {
                pAnimal.AniLifeNumber = pNewLifeNumber;
                pAnimal.AniAlternateNumber = pNewLifeNumber;
            }
            if (DB.UpdateANIMAL(pUbn.ThrID, pAnimal))
            {

                List<LEVNRMUT> lnms = DB.getLevNrMuts(pAnimal.AniId);
                var lnmu = from n in lnms
                           where n.LevnrOud == lOldChipnr
                           && n.LevnrNieuw == pNewLifeNumber
                           select n;
                LEVNRMUT lnm = new LEVNRMUT();
                if (lnmu.Any())
                {
                    lnm = lnmu.ElementAt(0);
                }
                lnm.Aniid = pAnimal.AniId;
                lnm.Datum = DateTime.Now;
                lnm.LevnrNieuw = pNewLifeNumber;
                lnm.LevnrOud = lOldChipnr;
                lnm.LNVMeldNr = pLNVMeldNr;
                DB.saveLevNrMut(lnm);
                try
                {

                    DB.DeleteLifenr(pBedrijf.FarmId, pNewLifeNumber);
                }
                catch { }
            }
        }

    }
}
