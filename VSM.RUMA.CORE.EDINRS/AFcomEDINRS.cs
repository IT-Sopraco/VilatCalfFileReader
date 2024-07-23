using System;
using System.Collections.Generic;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using VSM.RUMA.CORE.COMMONS;
using System.Text.RegularExpressions;
using System.Configuration;

namespace VSM.RUMA.CORE.EDINRS
{
    [Guid("027526B0-0C2F-47d1-BFD8-A10D7BA0E4D1"),
    ClassInterface(ClassInterfaceType.AutoDual)]
    abstract public class AFcomEDINRS : MarshalByRefObject
    {
        protected AFSavetoDB mSavetoDB;
        private Win32EDINRS mDllCall;
        private List<LABELS> mHairColors;
        private List<LABELS> mRaces;
        protected Dictionary<String, ANIMALCATEGORY> mWorkNumberList = new Dictionary<String, ANIMALCATEGORY>();
        private int flid;



        public AFcomEDINRS(AFSavetoDB pSavetoDB, int FileLogId)
        {
            mSavetoDB = pSavetoDB;
            mDllCall = new Win32EDINRS();
            flid = FileLogId;
        }
        ~AFcomEDINRS()
        {
            mDllCall.Dispose();
        }



        private int? GetHcoId(String pEDINRS)
        {
            var Hcolor = from lHairColor in mHairColors
                         where lHairColor.LabLabel == pEDINRS
                         select lHairColor;
            if (Hcolor.Count() == 0) return null;
            return Convert.ToInt32(Hcolor.First().LabId);

        }

        private int? GetRacId(String pEDINRS)
        {
            var Ras = from lRace in mRaces
                      where lRace.LabLabel == pEDINRS
                      select lRace;
            if (Ras.Count() == 0) return null;
            return Convert.ToInt32(Ras.First().LabId);

        }


        public abstract DateTime LeesHeader(String pBestandsnaam);

        public int LeesHeader(String pBestandsnaam, out String Berichttype, out String Versienr, out String Specificatie,
                      out DateTime BestandsDatum, out DateTime BestandsTijd)
        {
            int error;
            error = mDllCall.edinrs_HeaderInformatie(pBestandsnaam,
                      out Berichttype, out Versienr, out Specificatie,
                      out BestandsDatum, out BestandsTijd);


            unLogger.WriteDebug(String.Format("EDI HeaderInformatie {0} : BerichtType {1} Versienr {2} Specificatie {3}", pBestandsnaam, Berichttype, Versienr, Specificatie));
            return error;
        }


        public abstract bool LeesBestand(int pThrId, String pBestandsnaam, int pFarmId);

        public abstract bool LeesBestand(int pThrId, String pBestandsnaam, String pUBN, int pProgId, int Programid);

        public abstract bool LeesBestand(int pThrId, String pBestandsnaam, int pUBNId, int pProgId, int Programid);

        protected BEDRIJF lFarm;



        protected int OpvragenBedrijfsgegevens(String pBestandsnaam, out String pLand, out String pUBNnr,
              out String pNaam, out String pVoorvoegsel, out String pStraat, out String pHuisnummer,
              out String pPostcode, out String pWoonplaats, out String pTelefoonnr)
        {
            int error = mDllCall.edinrs_Bedrijfsgegevens(pBestandsnaam, out pLand, out pUBNnr,
                out pNaam, out pVoorvoegsel, out pStraat, out pHuisnummer,
                out pPostcode, out pWoonplaats, out pTelefoonnr);
            return error;
        }

        protected int LeesDieren(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_RegistratieRund");
            mHairColors = mSavetoDB.GetLabels(16, Convert.ToInt32(utils.getLabelsLabcountrycode()));
            int error = 0;
            int regeldieren = 0;
            String lLevensnr, lNaam, lGeslacht, lRegistratieSoort, lLevnrMoeder, lLevnrVader;
            String lUBNfokker, lHaarkleur, lLandHerkomst, Oornummer;
            DateTime lDatum, lGeboorteDatum;
            int Draagtijd, CodeRelatieMetBedrijf;
            while (error != -1)
            {
                regeldieren++;
                try
                {

                    error = mDllCall.edinrs_RegistratieRund(pBestandsnaam, regeldieren,
                    out lLevensnr, out lNaam, out lGeslacht, out lRegistratieSoort,
                    out lGeboorteDatum, out lDatum,
                    out lUBNfokker, out lHaarkleur, out lLandHerkomst,
                    out lLevnrMoeder, out lLevnrVader,
                    out Draagtijd, out CodeRelatieMetBedrijf);

                    if (error != -1)
                    {
                        ANIMAL lAniItem = GetAnimalFromAnimals(lLevensnr, out Oornummer);
                        ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lAniItem.AniId, lFarm.FarmId);
                        if (lAniItem.AniId == 0)
                        {

                            lAniItem.AniKind = 0;
                            lAniItem.AniBirthDate = lGeboorteDatum;

                            if (lGeslacht == "M")
                            {
                                lAniItem.AniSex = 1;
                            }
                            else if (lGeslacht == "V") lAniItem.AniSex = 2;
                            lAniItem.AniName = lNaam;
                            lAniItem.AniDraagtijd = Draagtijd;
                            lAniItem.AniCountryCodeOrigin = lLandHerkomst;
                            lAniItem.AniHaircolor_Memo = lHaarkleur;
                            int? haircolor = GetHcoId(lHaarkleur);
                            if (haircolor.HasValue)
                                lAniItem.Anihaircolor = haircolor.Value;
                            if (lAniItem.ThrId <= 0 && lUBNfokker != String.Empty && lUBNfokker != null)
                            {
                                int Fokkerid = GetThirdByUBN(lUBNfokker);
                                lAniItem.ThrId = Fokkerid;
                            }
                            bool KIstier = false;
                            lAniCategory.AniWorknumber = Oornummer;
                            switch (CodeRelatieMetBedrijf)
                            {
                                case 0:
                                    //lAniItem.AniCategory = 1;
                                    if (lAniItem.AniSex == 2)
                                        lAniCategory.Anicategory = 1;
                                    else
                                        lAniCategory.Anicategory = 3;
                                    break;
                                case 1:
                                    //lAniItem.AniCategory = 4;
                                    lAniCategory.Anicategory = 4;
                                    break;
                                case 2:
                                    lAniCategory.Anicategory = 5;
                                    //lAniItem.AniCategory = 5;
                                    if (lAniItem.AniSex == 1)
                                    {
                                        lAniItem.AniKind = 2;
                                        lAniItem.Changed_By = 2;
                                        lAniItem.SourceID = flid;
                                        mSavetoDB.SaveBulls(lAniItem);
                                        lAniCategory.AniId = lAniItem.AniId;
                                        lAniCategory.FarmId = lFarm.FarmId;
                                        lAniCategory.Changed_By = 2;
                                        lAniCategory.SourceID = flid;
                                        mSavetoDB.SaveAnimalCategory(lAniCategory);
                                        KIstier = true;
                                    }
                                    break;
                            }
                            if (!KIstier)
                            {
                                if (CodeRelatieMetBedrijf != 2)
                                {
                                    try
                                    {
                                        if (lLevnrVader != null && lLevnrVader != String.Empty)
                                            lAniItem.AniIdFather = GetAnimalFromAnimals(pThrId, lLevnrVader, 5, 1).AniId;
                                    }
                                    catch (Exception ex) { mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message); }
                                    try
                                    {
                                        if (lLevnrMoeder != null && lLevnrVader != String.Empty)
                                            lAniItem.AniIdMother = GetAnimalFromAnimals(pThrId, lLevnrMoeder, 5, 2).AniId;
                                    }
                                    catch (Exception ex) { mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message); }
                                }

                                lAniItem.Changed_By = 2;
                                lAniItem.SourceID = flid;
                                if (mSavetoDB.SaveAnimal(pThrId, lAniItem))
                                {
                                    lAniCategory.AniId = lAniItem.AniId;
                                    lAniCategory.FarmId = lFarm.FarmId;
                                    lAniCategory.Changed_By = 2;
                                    lAniCategory.SourceID = flid;
                                    mSavetoDB.SaveAnimalCategory(lAniCategory);
                                }
                                else
                                {
                                    mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Dier niet opgeslagen : " + lAniItem.AniLifeNumber);
                                }
                            }
                        }
                        else if (CodeRelatieMetBedrijf != 2 && lGeslacht == "M" && lAniItem.AniKind == 2)
                        {
                            if (lAniItem.AniKind == 2)
                            {
                                lAniItem.AniKind = 1;
                            }
                            lAniItem.AniBirthDate = lGeboorteDatum;
                            lAniItem.AniName = lNaam;
                            lAniItem.AniDraagtijd = Draagtijd;
                            lAniItem.AniCountryCodeOrigin = lLandHerkomst;
                            int? haircolor = GetHcoId(lHaarkleur);
                            if (haircolor.HasValue)
                                lAniItem.Anihaircolor = haircolor.Value;
                            if (lAniItem.ThrId <= 0)
                                lAniItem.ThrId = GetThirdByUBN(lUBNfokker);

                            lAniItem.Changed_By = 2;
                            lAniItem.SourceID = flid;
                            mSavetoDB.UpdateFokstier(lAniItem);
                            lAniCategory.AniId = lAniItem.AniId;
                            lAniCategory.FarmId = lFarm.FarmId;
                            lAniCategory.Changed_By = 2;
                            lAniCategory.SourceID = flid;
                            mSavetoDB.SaveAnimalCategory(lAniCategory);
                        }
                        else
                        {
                            if (lGeslacht == "M")
                            {
                                lAniItem.AniSex = 1;
                            }
                            else if (lGeslacht == "V") lAniItem.AniSex = 2;


                            if (CodeRelatieMetBedrijf == 0)
                            {
                                lAniItem.AniBirthDate = lGeboorteDatum;

                                if (lGeslacht == "M")
                                {
                                    lAniItem.AniSex = 1;
                                }
                                else if (lGeslacht == "V") lAniItem.AniSex = 2;
                                //lAniItem.AniImportDate = lDatum;
                                lAniItem.AniName = lNaam;
                                lAniItem.AniDraagtijd = Draagtijd;
                                lAniItem.AniCountryCodeOrigin = lLandHerkomst;
                                lAniItem.Anihaircolor = mSavetoDB.GetHcoId(lHaarkleur);
                                if (lAniItem.ThrId <= 0)
                                {
                                    int Fokkerid = GetThirdByUBN(lUBNfokker);
                                    lAniItem.ThrId = Fokkerid;
                                }
                                lAniCategory.AniWorknumber = Oornummer;
                                try
                                {
                                    if (lLevnrVader != null && lLevnrVader != String.Empty)
                                        lAniItem.AniIdFather = GetAnimalFromAnimals(pThrId, lLevnrVader, 5, 1).AniId;
                                }
                                catch (Exception ex) { mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message); }
                                try
                                {
                                    if (lLevnrMoeder != null && lLevnrMoeder != String.Empty)
                                        lAniItem.AniIdMother = GetAnimalFromAnimals(pThrId, lLevnrMoeder, 5, 2).AniId;
                                }
                                catch (Exception ex) { mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message); }
                                //lAniItem.AniCategory = 1;
                                lAniItem.Changed_By = 2;
                                lAniItem.SourceID = flid;
                                mSavetoDB.UpdateANIMAL(pThrId, lAniItem);
                                lAniCategory.AniId = lAniItem.AniId;
                                lAniCategory.FarmId = lFarm.FarmId;
                                if (lAniItem.AniSex == 2)
                                    lAniCategory.Anicategory = 1;
                                else
                                    lAniCategory.Anicategory = 3;
                                lAniCategory.Changed_By = 2;
                                lAniCategory.SourceID = flid;
                                mSavetoDB.SaveAnimalCategory(lAniCategory);
                            }
                            else if (CodeRelatieMetBedrijf == 2 && lAniItem.AniSex == 1)
                            {
                                lAniItem.AniName = lNaam;
                                lAniItem.AniBirthDate = lGeboorteDatum;
                                lAniItem.AniDraagtijd = Draagtijd;
                                lAniItem.AniCountryCodeOrigin = lLandHerkomst;
                                lAniItem.AniHaircolor_Memo = lHaarkleur;
                                int? haircolor = GetHcoId(lHaarkleur);
                                if (haircolor.HasValue)
                                    lAniItem.Anihaircolor = haircolor.Value;
                                if (lAniItem.ThrId <= 0 && lUBNfokker != String.Empty && lUBNfokker != null)
                                {
                                    lAniItem.ThrId = GetThirdByUBN(lUBNfokker);
                                }
                                lAniItem.AniKind = 2;
                                //lAniItem.AniCategory = 5;
                                lAniItem.Changed_By = 2;
                                lAniItem.SourceID = flid;
                                mSavetoDB.UpdateANIMAL(pThrId, lAniItem);
                                lAniCategory.AniId = lAniItem.AniId;
                                lAniCategory.FarmId = lFarm.FarmId;
                                lAniCategory.Anicategory = 5;
                                lAniCategory.AniWorknumber = Oornummer;
                                lAniCategory.Changed_By = 2;
                                lAniCategory.SourceID = flid;
                                mSavetoDB.SaveAnimalCategory(lAniCategory);
                            }
                            else
                            {
                                lAniItem.AniBirthDate = lGeboorteDatum;

                                if (lGeslacht == "M")
                                {
                                    lAniItem.AniSex = 1;
                                }
                                else if (lGeslacht == "V") lAniItem.AniSex = 2;
                                //lAniItem.AniImportDate = lDatum;
                                lAniItem.AniName = lNaam;
                                lAniItem.AniDraagtijd = Draagtijd;
                                lAniItem.AniCountryCodeOrigin = lLandHerkomst;
                                lAniItem.AniHaircolor_Memo = lHaarkleur;
                                int? haircolor = GetHcoId(lHaarkleur);
                                if (haircolor.HasValue)
                                    lAniItem.Anihaircolor = haircolor.Value;
                                if (lAniItem.ThrId <= 0)
                                {
                                    int Fokkerid = GetThirdByUBN(lUBNfokker);
                                    if (Fokkerid == 0 && lUBNfokker != String.Empty)
                                    {
                                        THIRD lThirdItem = new THIRD();
                                        lThirdItem.ThrFarmNumber = lUBNfokker;
                                        lThirdItem.ThrCompanyName = "Fokker " + lUBNfokker;
                                        lThirdItem.Changed_By = 2;
                                        lThirdItem.SourceID = flid;
                                        mSavetoDB.SaveThird(lThirdItem);
                                        Fokkerid = lThirdItem.ThrId;
                                    }
                                    lAniItem.ThrId = Fokkerid;
                                }
                                lAniCategory.AniWorknumber = Oornummer;
                                try
                                {
                                    if (lLevnrVader != null && lLevnrVader != String.Empty)
                                        lAniItem.AniIdFather = GetAnimalFromAnimals(pThrId, lLevnrVader, 5, 1).AniId;
                                }
                                catch (Exception ex) { mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message); }
                                try
                                {
                                    if (lLevnrMoeder != null && lLevnrVader != String.Empty)
                                        lAniItem.AniIdMother = GetAnimalFromAnimals(pThrId, lLevnrMoeder, 5, 2).AniId;
                                }
                                catch (Exception ex) { mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message); }

                                switch (CodeRelatieMetBedrijf)
                                {
                                    case 0:
                                        //lAniItem.AniCategory = 1;
                                        if (lAniItem.AniSex == 2)
                                            lAniCategory.Anicategory = 1;
                                        else
                                            lAniCategory.Anicategory = 3;
                                        break;
                                    case 1:
                                        //lAniItem.AniCategory = 4;
                                        lAniCategory.Anicategory = 4;
                                        break;
                                    case 2:
                                        lAniCategory.Anicategory = 5;
                                        //lAniItem.AniCategory = 5;
                                        break;
                                }
                                lAniItem.Changed_By = 2;
                                lAniItem.SourceID = flid;
                                mSavetoDB.UpdateANIMAL(pThrId, lAniItem);
                                lAniCategory.AniId = lAniItem.AniId;
                                lAniCategory.FarmId = lFarm.FarmId;
                                lAniCategory.Changed_By = 2;
                                lAniCategory.SourceID = flid;
                                mSavetoDB.SaveAnimalCategory(lAniCategory);
                            }
                            //mSavetoDB.WriteError("Bestaand Dier : " + lAniItem.AniLifeNumber + "\r\n", 1,9999);
                        }
                    }
                }
                catch (Exception ex)
                {
                    mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, ex.Message);
                    unLogger.WriteDebug(ex.Message, ex);
                }
            }
            //mDllCall.FreeHandles();
            return regeldieren;
        }

        protected int LeesKIstier(String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_KIstier");
            int error = 0;
            int regelKIstier = 0;
            String lLevensnr;
            String lKICode;
            String lShortName;
            String lNameOwner;
            while (error != -1)
            {
                regelKIstier++;
                error = mDllCall.edinrs_KIstier(pBestandsnaam, regelKIstier,
                out lLevensnr, out lKICode,
                out lShortName, out lNameOwner);
                if (error != -1)
                {
                    String Oornummer;
                    ANIMAL lItem = GetAnimalFromAnimals(lLevensnr, out Oornummer);
                    lItem.AniSex = 1;
                    lItem.BullAiNumber = lKICode;
                    lItem.BullShortName = lShortName;
                    lItem.AniName = lShortName;
                    lItem.AniLifeNumber = lLevensnr;
                    //string BullLifeNumber = lLevensnr.Substring(lLevensnr.IndexOf(" ") + 1);
                    //string BullCountrycode = lLevensnr.Substring(0, lLevensnr.IndexOf(" "));
                    ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lItem.AniId, lFarm.FarmId);
                    lAniCategory.AniWorknumber = Oornummer;
                    lAniCategory.Anicategory = 5;
                    //lItem.AniCategory = 5;
                    BULLUBN lBullUBN = mSavetoDB.GetBullUbn(lFarm.FarmId, Convert.ToInt32(lItem.AniId));

                    if (lItem.AniId != 0)
                    {
                        lItem.Changed_By = 2;
                        lItem.SourceID = flid;
                        mSavetoDB.UpdateFokstier(lItem);
                    }
                    else
                    {
                        lItem.AniKind = 2;
                        lItem.Changed_By = 2;
                        lItem.SourceID = flid;
                        mSavetoDB.SaveBulls(lItem);
                    }
                    lAniCategory.AniId = lItem.AniId;
                    lAniCategory.FarmId = lFarm.FarmId;
                    lAniCategory.Changed_By = 2;
                    lAniCategory.SourceID = flid;
                    mSavetoDB.SaveAnimalCategory(lAniCategory);
                    lBullUBN.FarmId = Convert.ToInt32(lFarm.FarmId);
                    lBullUBN.BullId = Convert.ToInt32(lItem.AniId);
                    lBullUBN.Changed_By = 2;
                    lBullUBN.SourceID = flid;
                    mSavetoDB.saveBullUbn(lBullUBN);

                }
            }
            //mDllCall.FreeHandles();
            return regelKIstier;
        }

        protected int LeesDierRegistratie(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_RegistratieRundOpBedrijf");
            int error = 0;
            int regeldierenreg = 0;
            String lLevensnr, lUBNnr;
            int groepsnr, koenr;
            while (error != -1)
            {
                regeldierenreg++;
                try
                {
                    error = mDllCall.edinrs_RegistratieRundOpBedrijf(pBestandsnaam, regeldierenreg,
                        out lLevensnr, out lUBNnr,
                        out groepsnr, out koenr);

                    if (error != -1)
                    {

                        if (koenr > 0)
                        {
                            String Diernummer = koenr.ToString();
                            while (Diernummer.Length < 4)
                            {
                                Diernummer = "0" + Diernummer;
                            }
                            ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                            SetAniWorknumber(lAniItem, Diernummer);


                            if (mWorkNumberList.ContainsKey(lLevensnr))
                            {
                                mWorkNumberList[lLevensnr].AniWorknumber = Diernummer;
                                mWorkNumberList[lLevensnr].Anicategory = 1;
                            }
                            else
                            {
                                mWorkNumberList.Add(lLevensnr, new ANIMALCATEGORY());
                                mWorkNumberList[lLevensnr].AniWorknumber = Diernummer;
                                mWorkNumberList[lLevensnr].Anicategory = 1;


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mSavetoDB.WriteLogMessage(lFarm.UBNid, 3,ex.Message);
                }
            }
            return regeldierenreg;
        }

        protected int LeesRas(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_RegistratieRas");
            mRaces = mSavetoDB.GetLabels(19, Convert.ToInt32(utils.getLabelsLabcountrycode()));
            int error = 0;
            int regelRas = 0;
            String lLevensnr;
            String lCodeRas;
            int lRasDeel;
            while (error != -1)
            {
                regelRas++;
                error = mDllCall.edinrs_RegistratieRas(pBestandsnaam, regelRas,
                out lLevensnr, out lCodeRas,
                out lRasDeel);
                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    int? RacId = GetRacId(lCodeRas);
                    if (RacId.HasValue)
                    {
                        SECONRAC lSeconRace = mSavetoDB.GetSeconRacByKey(lAniItem.AniId, RacId.Value);
                        lSeconRace.AniId = lAniItem.AniId;
                        lSeconRace.RacId = RacId.Value;
                        lSeconRace.SraRate = lRasDeel;
                        lSeconRace.Changed_By = 2;
                        lSeconRace.SourceID = flid;
                        mSavetoDB.SaveSeconRace(lSeconRace);
                    }
                }
            }
            //mDllCall.FreeHandles();
            return regelRas;
        }

        protected ANIMAL GetAnimalFromAnimals(int pThrId, string pLevensnr)
        {
            return GetAnimalFromAnimals(pThrId, pLevensnr, 99, 2);
        }

        protected ANIMAL GetBullFromAnimals(int pThrId, string pLevensnr)
        {
            return GetAnimalFromAnimals(pThrId, pLevensnr, 99, 1);
        }

        protected ANIMAL GetAnimalFromAnimals(int pThrId, string pLevensnr, int pAniCat, int pAniSex)
        {
            unAnimalActions lAction = new unAnimalActions(mSavetoDB);
            return lAction.GetAnimalFromAnimals(pThrId, lFarm, pLevensnr, pAniCat, pAniSex);
        }


        protected ANIMAL GetAnimalFromAnimals(string pLevensnr, out string AniWorkNumber)
        {
            unAnimalActions lAction = new unAnimalActions(mSavetoDB);
            ANIMAL Animal = lAction.GetAnimalFromAnimals(lFarm, pLevensnr, out AniWorkNumber);
            if (mWorkNumberList.ContainsKey(pLevensnr))
            {
                AniWorkNumber = mWorkNumberList[pLevensnr].AniWorknumber;
            }
            return Animal;
        }

        public void SetAniWorknumber(ANIMAL pAnimal, String AniWorknumber)
        {

            if (lFarm.FarmId > 0 && pAnimal.AniId > 0)
            {
                var Bedrijven = mSavetoDB.getBedrijvenByUBNId(lFarm.UBNid).Where(bedr => bedr.ProgId == 1 && bedr.FarmId != lFarm.FarmId).ToList();
                String Diernummer = AniWorknumber;
                while (AniWorknumber != String.Empty && Diernummer.Length < 4)
                {
                    Diernummer = "0" + Diernummer;
                }

                ANIMALCATEGORY anicat = mSavetoDB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, lFarm.FarmId);
                if (anicat.Anicategory > 0)
                {
                    anicat.FarmId = lFarm.FarmId;
                    anicat.AniWorknumber = Diernummer;
                    anicat.Changed_By = 2;
                    anicat.SourceID = flid;
                    mSavetoDB.SaveAnimalCategory(anicat);
                }
                foreach (BEDRIJF adminbedr in Bedrijven)
                {
                    ANIMALCATEGORY admincat = mSavetoDB.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, adminbedr.FarmId);
                    admincat.FarmId = lFarm.FarmId;
                    admincat.AniWorknumber = anicat.AniWorknumber;
                    admincat.Anicategory = anicat.Anicategory;
                    admincat.Changed_By = 2;
                    admincat.SourceID = flid;
                    mSavetoDB.SaveAnimalCategory(anicat);
                }

            }

        }


        protected int GetThirdByUBN(string pUBN)
        {
            if (pUBN == null || pUBN == String.Empty)
            {
                return 0;
            }
            try
            {
                THIRD lThird = mSavetoDB.GetThirdByUBN(removevoorloopnullen(pUBN));
                if (lThird.ThrId == 0)
                {

                    String LNV_UBN = String.Empty;
                    String LNV_BRS = String.Empty;
                    String LNV_Naam = String.Empty;
                    String LNV_Adres = String.Empty;
                    String LNV_Postcode = String.Empty;
                    String LNV_Woonplaats = String.Empty;
                    SOAPLNV.Masking m = new SOAPLNV.Masking();
                    VSM.RUMA.CORE.SOAPLNV.SOAPLNVALG_Referentie1 soapref = new VSM.RUMA.CORE.SOAPLNV.SOAPLNVALG_Referentie1();
                    string pword = m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
                    soapref.raadplegenMachtigingen_Adres(ConfigurationManager.AppSettings["LNVDierDetailsusername"], pword, 0, pUBN,
                         ref LNV_UBN, ref LNV_BRS, ref LNV_Naam, ref LNV_Adres, ref LNV_Postcode, ref LNV_Woonplaats);
                    lThird = mSavetoDB.GetThirdByHouseNrAndZipCode(ExtractHouseNumberFromAdress(LNV_Adres), LNV_Postcode);
                    if (lThird.ThrId == 0)
                    {
                        THIRD lThirdItem = new THIRD();
                        lThirdItem.ThrFarmNumber = pUBN;
                        if (LNV_Naam == String.Empty) lThirdItem.ThrCompanyName = "Fokker " + removevoorloopnullen(pUBN);
                        else lThirdItem.ThrCompanyName = LNV_Naam;
                        lThirdItem.ThrCity = LNV_Woonplaats;
                        lThirdItem.ThrZipCode = LNV_Postcode;
                        lThirdItem.ThrExt = ExtractHouseNumberFromAdress(LNV_Adres);
                        lThirdItem.ThrStreet1 = LNV_Adres;
                        lThirdItem.Changed_By = 2;
                        lThirdItem.SourceID = flid;
                        mSavetoDB.SaveThird(lThirdItem);
                        UBN lUbn = mSavetoDB.getUBNByBedrijfsnummer(removevoorloopnullen(pUBN));
                        if (lUbn.UBNid == 0)
                        {
                            lUbn.Bedrijfsnummer = removevoorloopnullen(pUBN);
                            lUbn.ThrID = lThirdItem.ThrId;
                        }
                        lThird = mSavetoDB.GetThirdByUBN(removevoorloopnullen(pUBN));
                    }
                }


                return lThird.ThrId;
            }
            catch
            {
                return 0;
            }
        }


        public static string ExtractHouseNumberFromAdress(string pAdress)
        {
            StringBuilder bld = new StringBuilder();
            bool isnumber = false;
            Regex r = new Regex(@"^\d{1}$");
            for (int i = pAdress.Length - 1; i > -1; i--)
            {
                Match m = r.Match(pAdress[i].ToString());
                if (m.Success)
                {
                    isnumber = true;
                }
                if (isnumber)
                {
                    if (pAdress[i].ToString() == " ")
                    {
                        break;
                    }
                }
                bld.Insert(0, pAdress[i].ToString());
            }
            return bld.ToString();
            //return string.Join(null, System.Text.RegularExpressions.Regex.Split(pAdress, "[^\\d]"));
        }

        /*
        protected ANIMAL GetAnimalFromAnimals(ref List<ANIMAL> pAnimals, string pLevensnr)
        {
            if (pLevensnr == null)
            {
                return null;
            }

            foreach (ANIMAL lAnimal in pAnimals)
            {
                if (lAnimal.AniLifeNumber == pLevensnr)
                {
                    return lAnimal;
                }
            }
            ANIMAL lAniItem = new ANIMAL();
            lAniItem.AniLifeNumber = pLevensnr;
            string lLevensnr = pLevensnr.Substring(pLevensnr.IndexOf(" ") + 1);
            string lCountryCode = pLevensnr.Substring(0, pLevensnr.IndexOf(" "));
            if (DelphiWrapper.controlelevensnr(1, lCountryCode, lLevensnr, false))
            {
                lAniItem.AniWorkNumber = DelphiWrapper.fDiernr(1, lCountryCode, lLevensnr, false);
                pAnimals.Add(lAniItem);
                lAniItem.AniId = pAnimals.IndexOf(lAniItem) + 1;
            }
            else
            {
                //mSavetoDB.WriteError("Ongeldig Levensnummer"+ "\r\n");
                throw new Exception("Ongeldig Levensnummer");
            }
            return lAniItem;
        }*/

        protected int LeesAanvoer(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_AanvoerRund");
            int error = 0;
            int regelaanvoer = 0;
            String lLevensnr;
            String lFarmNumber;
            DateTime lDatum;
            int lCodeAanvoer;
            while (error != -1)
            {
                regelaanvoer++;
                error = mDllCall.edinrs_AanvoerRund(pBestandsnaam, regelaanvoer,
                out lLevensnr, out lFarmNumber,
                out lDatum,
                out lCodeAanvoer);
                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr, 1, 2);
                    ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lAniItem.AniId, lFarm.FarmId);
                    int Ubnid = mSavetoDB.GetUBNidbyUBN(lFarmNumber);
                    if (lAniItem.AniId > 0)
                    {
                        //lAniItem.AniCategory = 1;
                        lAniCategory.Anicategory = 1;
                        lAniItem.Changed_By = 2;
                        lAniItem.SourceID = flid;
                        mSavetoDB.UpdateANIMAL(pThrId, lAniItem);
                        mSavetoDB.SaveAnimalCategory(lAniCategory);
                    }


                    if (lCodeAanvoer == 1)
                    {
                        MOVEMENT lMovItem = mSavetoDB.GetMovementByDateAniIdKind(lDatum, lAniItem.AniId, 1, Ubnid);
                        lMovItem.UbnId = Ubnid;
                        lMovItem.MovDate = lDatum;
                        lMovItem.AniId = lAniItem.AniId;
                        lMovItem.MovKind = 1;
                        lMovItem.MovMutationBy = 3;
                        lMovItem.MovMutationDate = DateTime.Now;
                        lMovItem.MovMutationTime = DateTime.Now;
                        lMovItem.Progid = lFarm.ProgId;

                        BUYING lPurItem;
                        if (lMovItem.MovId == 0)
                        {
                            lPurItem = new BUYING();
                            lMovItem.happened_at_FarmID = lFarm.FarmId;
                        }
                        else
                        {
                            lPurItem = mSavetoDB.GetBuying(lMovItem.MovId);
                        }
                        lMovItem.Changed_By = 2;
                        lMovItem.SourceID = flid;

                        if (mSavetoDB.SaveMovement(lMovItem))
                        {
                            lPurItem.MovId = lMovItem.MovId;
                            lPurItem.Changed_By = 2;
                            lPurItem.SourceID = flid;
                            if (!mSavetoDB.SaveBuying(lPurItem))
                                mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Aanvoer niet opgeslagen ");

                        }
                        else
                            mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Aanvoer niet opgeslagen ");
                    }
                }
            }
            //mDllCall.FreeHandles();
            return regelaanvoer;
        }

        protected int LeesAfvoer(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_AfvoerRund");
            int error = 0;
            int regelafvoer = 0;
            String lLevensnr;
            String lFarmNumber;
            DateTime lDatum;
            int lCodeAfvoer;
            int lCodeRedenAfvoer;
            while (error != -1)
            {
                regelafvoer++;
                error = mDllCall.edinrs_AfvoerRund(pBestandsnaam, regelafvoer,
                out lLevensnr, out lFarmNumber,
                out lDatum,
                out lCodeAfvoer, out lCodeRedenAfvoer);
                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr, 4, 0);
                    ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lAniItem.AniId, lFarm.FarmId);
                    //lAniItem.AniCategory = 4;
                    lAniCategory.Anicategory = 4;
                    lAniItem.Changed_By = 2;
                    lAniItem.SourceID = flid;
                    mSavetoDB.UpdateANIMAL(pThrId, lAniItem);
                    lAniCategory.Changed_By = 2;
                    lAniCategory.SourceID = flid;
                    mSavetoDB.SaveAnimalCategory(lAniCategory);

                    int Ubnid = mSavetoDB.GetUBNidbyUBN(lFarmNumber);
                    if (lCodeAfvoer == 1 || lCodeAfvoer == 4)
                    {
                        MOVEMENT lMovItem = mSavetoDB.GetMovementByDateAniIdKind(lDatum, lAniItem.AniId, 2, Ubnid);
                        lMovItem.UbnId = Ubnid;
                        lMovItem.AniId = lAniItem.AniId;
                        lMovItem.MovDate = lDatum;
                        lMovItem.Progid = lFarm.ProgId;
                        lMovItem.MovMutationBy = 3;
                        lMovItem.MovMutationDate = DateTime.Now;
                        lMovItem.MovMutationTime = DateTime.Now;
                        lMovItem.MovKind = 2;
                        SALE lSalItem;
                        if (lMovItem.MovId == 0)
                        {
                            lSalItem = new SALE();
                            lMovItem.happened_at_FarmID = lFarm.FarmId;
                        }
                        else
                        {
                            lSalItem = mSavetoDB.GetSale(lMovItem.MovId);
                        }
                        lMovItem.Changed_By = 2;
                        lMovItem.SourceID = flid;
                        if (mSavetoDB.SaveMovement(lMovItem))
                        {
                            lSalItem.MovId = lMovItem.MovId;
                            lSalItem.Changed_By = 2;
                            lSalItem.SourceID = flid;
                            if (mSavetoDB.SaveSale(lSalItem))
                            {

                            }
                            else
                                mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Afvoer niet opgeslagen");
                        }
                        else
                            mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Afvoer niet opgeslagen ");
                    }
                    else if (lCodeAfvoer == 3)
                    {
                        MOVEMENT lMovItem = mSavetoDB.GetMovementByDateAniIdKind(lDatum, lAniItem.AniId, 3, Ubnid);
                        lMovItem.UbnId = Ubnid;
                        lMovItem.AniId = lAniItem.AniId;
                        lMovItem.MovDate = lDatum;
                        lMovItem.Progid = lFarm.ProgId;
                        lMovItem.MovMutationBy = 3;
                        lMovItem.MovMutationDate = DateTime.Now;
                        lMovItem.MovMutationTime = DateTime.Now;
                        lMovItem.MovKind = 3;
                        LOSS lLossItem;
                        if (lMovItem.MovId == 0)
                        {
                            lLossItem = new LOSS();
                            lMovItem.happened_at_FarmID = lFarm.FarmId;
                        }
                        else
                        {
                            lLossItem = mSavetoDB.GetLoss(lMovItem.MovId);
                        }
                        lMovItem.Changed_By = 2;
                        lMovItem.SourceID = flid;
                        if (mSavetoDB.SaveMovement(lMovItem))
                        {
                            lLossItem.MovId = lMovItem.MovId;
                            lLossItem.Changed_By = 2;
                            lLossItem.SourceID = flid;
                            if (mSavetoDB.SaveLoss(lLossItem))
                            {

                            }
                            else
                                mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Afvoer (doodmelding) niet opgeslagen");
                        }
                        else
                            mSavetoDB.WriteLogMessage(lFarm.UBNid, 3, "Afvoer (doodmelding) niet opgeslagen ");
                    }
                    else if (lCodeAfvoer == 2)
                    {
                        MOVEMENT lMovItem = mSavetoDB.GetMovementByDateAniIdKind(lDatum, lAniItem.AniId, 3, Ubnid);
                        lMovItem.UbnId = Ubnid;
                        lMovItem.AniId = lAniItem.AniId;
                        lMovItem.MovDate = lDatum;
                        lMovItem.Progid = lFarm.ProgId;
                        lMovItem.MovMutationBy = 3;
                        lMovItem.MovMutationDate = DateTime.Now;
                        lMovItem.MovMutationTime = DateTime.Now;
                        lMovItem.MovKind = 6;
                        lMovItem.Changed_By = 2;
                        lMovItem.SourceID = flid;
                        if (mSavetoDB.SaveMovement(lMovItem))
                        {

                        }
                        else
                            mSavetoDB.WriteLogMessage(lFarm.UBNid, 3,"Afvoer (uitscharen) niet opgeslagen ");
                    }
                }
            }
            //mDllCall.FreeHandles();
            return regelafvoer;
        }

        protected int LeesAfkalven(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_AfkalvingKoe");
            int error = 0;
            int regelAfkalven = 0;
            String lLevensnr;
            DateTime lDatum;
            int lAantalKalveren;
            int lPariteit;
            while (error != -1)
            {
                regelAfkalven++;
                error = mDllCall.edinrs_AfkalvingKoe(pBestandsnaam, regelAfkalven,
                out lLevensnr,
                out lDatum,
                out lAantalKalveren, out lPariteit);

                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    mSavetoDB.Afkalven(lDatum, lAantalKalveren, lPariteit, lAniItem, lFarm.UBNid, lFarm.FarmId);
                }
            }
            //mDllCall.FreeHandles();
            return regelAfkalven;
        }

        protected int LeesInseminatie(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_DekkingRund");
            int error = 0;
            int regelInseminatie = 0;
            String lLevensnr, DekInfo, Levensnrstier;
            DateTime lDatum, DatumEindeSamenweiden;
            while (error != -1)
            {
                regelInseminatie++;
                error = mDllCall.edinrs_DekkingRund(pBestandsnaam, regelInseminatie,
                out lLevensnr,
                out lDatum,
                out DekInfo, out Levensnrstier, out DatumEindeSamenweiden);

                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    EVENT lEveItem = mSavetoDB.GetEventByDateAniIdKind(lDatum, lAniItem.AniId, 2);
                    lEveItem.UBNId = lFarm.UBNid;
                    lEveItem.EveDate = lDatum;
                    lEveItem.AniId = lAniItem.AniId;
                    lEveItem.EveMutationBy = 3;
                    lEveItem.EveMutationDate = DateTime.Now;
                    lEveItem.EveMutationTime = DateTime.Now;
                    lEveItem.EveKind = 2;
                    INSEMIN lInsemItem;
                    if (lEveItem.EventId == 0)
                    {
                        lInsemItem = new INSEMIN();

                    }
                    else
                    {
                        lInsemItem = mSavetoDB.GetInsemin(lEveItem.EventId);
                    }
                    lEveItem.Changed_By = 2;
                    lEveItem.SourceID = flid;
                    mSavetoDB.SaveEvent(lEveItem);
                    lInsemItem.EventId = lEveItem.EventId;
                    ANIMAL lBullItem = GetBullFromAnimals(pThrId, Levensnrstier);
                    if (lBullItem.AniSex == 1 && lBullItem.AniKind == 0)
                    {
                        lBullItem.AniKind = 1;
                        lBullItem.BullShortName = lBullItem.AniName;
                        lBullItem.Changed_By = 2;
                        lBullItem.SourceID = flid;
                        mSavetoDB.UpdateFokstier(lBullItem);
                    }

                    lInsemItem.AniIdFather = lBullItem.AniId;
                    switch (DekInfo)
                    {
                        case "I":
                            lInsemItem.InsKind = 1;
                            break;
                        case "D":
                            lInsemItem.InsKind = 4;
                            break;
                        case "N":
                            lInsemItem.InsKind = 2;
                            break;
                    }
                    if (lBullItem.AniSex != 2)
                    {
                        lInsemItem.Changed_By = 2;
                        lInsemItem.SourceID = flid;
                        mSavetoDB.SaveInsemin(lInsemItem);
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
            }
            //mDllCall.FreeHandles();
            return regelInseminatie;
        }

        protected int LeesProduktiestatus(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_MutatieProduktiestatus");
            int error = 0;
            int regelDroogzetten = 0;
            String lLevensnr;
            DateTime lDatum;
            int lStatusProduktie;
            while (error != -1)
            {
                regelDroogzetten++;
                error = mDllCall.edinrs_MutatieProduktiestatus(pBestandsnaam, regelDroogzetten,
                out lLevensnr,
                out lStatusProduktie,
                out lDatum);
                if (error != -1)
                {
                    switch (lStatusProduktie)
                    {
                        case 50:
                            ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                            EVENT lEveItem = mSavetoDB.GetEventByDateAniIdKindInteval(lDatum, lAniItem.AniId, 4, 90);
                            lEveItem.UBNId = lFarm.UBNid;
                            lEveItem.AniId = lAniItem.AniId;
                            lEveItem.EveDate = lDatum;
                            lEveItem.EveMutationBy = 3;
                            lEveItem.EveMutationDate = DateTime.Now;
                            lEveItem.EveMutationTime = DateTime.Now;
                            lEveItem.EveKind = 4;

                            DRYOFF lDryoffItem;
                            if (lEveItem.EventId == 0)
                            {
                                lDryoffItem = new DRYOFF();
                                lEveItem.Changed_By = 2;
                                lEveItem.SourceID = flid;
                                mSavetoDB.SaveEvent(lEveItem);
                                lDryoffItem.EventId = lEveItem.EventId;
                                lDryoffItem.Changed_By = 2;
                                lDryoffItem.SourceID = flid;
                                mSavetoDB.SaveDryoff(lDryoffItem);
                            }

                            break;
                    }
                }
            }
            //mDllCall.FreeHandles();
            return regelDroogzetten;
        }

        protected int LeesBedrijfsgegevensDagproduktie(String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_BedrijfsgegevensDagproduktie");
            int error = 0;
            int regelbedrprod = 0;
            String UBNnr;
            DateTime DatumProduktie;
            int AantKoeienBemonsterd;
            int AantKoeienDroog;
            int AantKoeienMelkgevend;
            int AantMelkingenPerDag;
            int CodeAfleidingDagprod;
            DateTime DatumOnderzoekVetEiwit;
            int KgMelkDagprod;
            int KgVetDagprodX100;
            int KgEiwitDagprodX100;
            int KgMelk305gem;
            int KgVet305gemX100;
            int KgEiwit305gemX100;
            int KgMelk305st;
            int KgVet305stX100;
            int KgEiwit305stX100;
            int NettoOpbrengst;
            int BSKx10;
            int StatusDagProduktie;
            int MPR24hLaatste;
            int MPR24hJaarGem;
            int IndicatieEMM;
            long Ubnid = lFarm.UBNid;
            while (error != -1)
            {
                regelbedrprod++;
                error = mDllCall.edinrs_BedrijfsgegevensDagproduktie(pBestandsnaam, regelbedrprod,
                  out UBNnr, out DatumProduktie,
                  out AantKoeienBemonsterd, out AantKoeienDroog, out AantKoeienMelkgevend,
                  out AantMelkingenPerDag, out CodeAfleidingDagprod,
                  out DatumOnderzoekVetEiwit,
                  out KgMelkDagprod, out KgVetDagprodX100, out KgEiwitDagprodX100,
                  out KgMelk305gem, out KgVet305gemX100, out KgEiwit305gemX100,
                  out KgMelk305st, out KgVet305stX100, out KgEiwit305stX100,
                  out NettoOpbrengst, out BSKx10, out StatusDagProduktie,
                  out MPR24hLaatste, out MPR24hJaarGem, out IndicatieEMM);

                if (error != -1)
                {
                    BEDRPROD lBedrprodItem = mSavetoDB.getBedrProd(int.Parse(Ubnid.ToString()), DatumProduktie);
                    if (!mSavetoDB.isFilledByDb(lBedrprodItem))
                    {
                        lBedrprodItem.UbnId = lFarm.UBNid;
                        lBedrprodItem.BSKValue = (double)Decimal.Divide(BSKx10, 10);
                        lBedrprodItem.Mpr24hLast = MPR24hLaatste;
                        lBedrprodItem.Mpr24hYearavg = MPR24hJaarGem;
                        lBedrprodItem.Average305Milk = KgMelk305gem;
                        lBedrprodItem.Average305MilkSt = KgMelk305st;
                        lBedrprodItem.Average305Protein = (double)Decimal.Divide(KgEiwit305gemX100, 100);
                        lBedrprodItem.Average305ProteinSt = (double)Decimal.Divide(KgEiwit305stX100, 100);
                        lBedrprodItem.Average305Fat = (double)Decimal.Divide(KgVet305gemX100, 100);
                        lBedrprodItem.Average305FatSt = (double)Decimal.Divide(KgVet305stX100, 100);
                        lBedrprodItem.CodeMilkCount = AantMelkingenPerDag;
                        lBedrprodItem.NumberCowDryOff = AantKoeienDroog;
                        lBedrprodItem.NumberCowMilking = AantKoeienMelkgevend;
                        lBedrprodItem.NumberCowSamples = AantKoeienBemonsterd;
                        lBedrprodItem.DateFatProteinResearch = DatumOnderzoekVetEiwit;
                        lBedrprodItem.ProductionDate = DatumProduktie;
                        lBedrprodItem.NettIncomeFarm = NettoOpbrengst;
                        lBedrprodItem.CodeStatusProdFarm = StatusDagProduktie;
                        lBedrprodItem.CodeDerivationProd = CodeAfleidingDagprod;
                        //toegevoegd 20-07
                        lBedrprodItem.TotMilk = (double)KgMelkDagprod;
                        lBedrprodItem.TotKgFat = (double)Decimal.Divide(KgVetDagprodX100, 100);
                        lBedrprodItem.TotKgProtein = (double)Decimal.Divide(KgEiwitDagprodX100, 100);
                        if (IndicatieEMM == 1)
                        {
                            lBedrprodItem.TypeOfControl = 4;
                        }
                        //lBedrprodItem.
                        lBedrprodItem.Changed_By = 2;
                        lBedrprodItem.SourceID = flid;
                        mSavetoDB.SaveBedrprod(lBedrprodItem);
                    }
                }
            }
            //mDllCall.FreeHandles();
            return regelbedrprod;
        }

        protected int LeesDrachtigheidsstatus(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_MutatieDrachtigheidsstatus");
            int error = 0;
            int regeldracht = 0;
            String lLevensnr;
            int StatusDracht;
            DateTime lDatum;

            while (error != -1)
            {
                regeldracht++;
                error = mDllCall.edinrs_MutatieDrachtigheidsstatus(pBestandsnaam, regeldracht,
                        out lLevensnr, out StatusDracht, out lDatum);

                if (error != -1 && lDatum.Year > 1900)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    EVENT lEveItem = mSavetoDB.GetEventByDateAniIdKind(lDatum, lAniItem.AniId, 3);
                    lEveItem.UBNId = lFarm.UBNid;
                    lEveItem.AniId = lAniItem.AniId;
                    lEveItem.EveDate = lDatum;
                    lEveItem.EveMutationBy = 3;
                    lEveItem.EveMutationDate = DateTime.Now;
                    lEveItem.EveMutationTime = DateTime.Now;
                    lEveItem.EveKind = 3;
                    GESTATIO lDrachtItem;
                    if (lEveItem.EventId == 0)
                    {
                        lDrachtItem = new GESTATIO();
                    }
                    else
                    {
                        lDrachtItem = mSavetoDB.GetGestatio(lEveItem.EventId);
                    }


                    switch (StatusDracht)
                    {
                        case 25:
                            lDrachtItem.GesStatus = 2;
                            break;
                        case 26:
                            lDrachtItem.GesStatus = 3;
                            break;
                        case 29:
                            lDrachtItem.GesStatus = 4;
                            break;
                        default:
                            continue;
                    }
                    lEveItem.Changed_By = 2;
                    lEveItem.SourceID = flid;
                    mSavetoDB.SaveEvent(lEveItem);
                    lDrachtItem.EventId = lEveItem.EventId;
                    //25 = Drachtig
                    //26 = Niet drachtig
                    //29 = Gust
                    lDrachtItem.Changed_By = 2;
                    lDrachtItem.SourceID = flid;
                    mSavetoDB.SaveGestation(lDrachtItem);
                }
            }
            //mDllCall.FreeHandles();
            return regeldracht;
        }

        protected void LeesProduktie(int pThrId, String pBestandsnaam, bool IsEVOBestand)
        {
            List<ANALYSE> lAnalyse = new List<ANALYSE>();
            LeesDagProduktie(pThrId, pBestandsnaam, IsEVOBestand, ref lAnalyse);
            LeesLactatieProduktie(pThrId, pBestandsnaam, ref lAnalyse);

        }

        protected ANALYSE GetAnalyseFromAnalyseList(ref List<ANALYSE> pAnalyse, long pAniId, DateTime pDatum)
        {
            if (pAniId == 0)
            {
                return null;
            }
            else
            {
                foreach (ANALYSE lAnalyse in pAnalyse)
                {
                    if (lAnalyse.AniId == pAniId && lAnalyse.AnaMilkDate == pDatum)
                    {
                        return lAnalyse;
                    }
                }
                return null;
            }
        }

        protected int LeesMelking(String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_BedrijfsgegevensMelking");
            int error = 0;
            int regelMelking = 0;
            DateTime DatumProduktie;
            string UBNnr;
            DateTime TijdBeginMelking = DateTime.MinValue;
            DateTime TijdEindMelking = DateTime.MinValue;
            while (error != -1)
            {
                regelMelking++;

                error = mDllCall.edinrs_BedrijfsgegevensMelking(pBestandsnaam, regelMelking,
                    out UBNnr, out DatumProduktie,
                    out TijdBeginMelking, out TijdEindMelking);

                if (error != -1)
                {
                    // opslaan 
                    //toegevoegd 20-07-09
                    //kijk of het een databaserecord is oftewel FilledByDb
                    BEDRPROD bder = mSavetoDB.getBedrProd(lFarm.UBNid, DatumProduktie);
                    if (mSavetoDB.isFilledByDb(bder))
                    {
                        //tijdstip-begin-melking
                        if (bder.MilkStartTime1.Equals(DateTime.MinValue))
                        { bder.MilkStartTime1 = TijdBeginMelking; }
                        else if (bder.MilkStartTime2.Equals(DateTime.MinValue))
                        { bder.MilkStartTime2 = TijdBeginMelking; }
                        else if (bder.MilkStartTime3.Equals(DateTime.MinValue))
                        { bder.MilkStartTime3 = TijdBeginMelking; }

                        //tijdstip-eind-melking
                        if (bder.MilkEndTime1.Equals(DateTime.MinValue))
                        { bder.MilkEndTime1 = TijdEindMelking; }
                        else if (bder.MilkEndTime2.Equals(DateTime.MinValue))
                        { bder.MilkEndTime2 = TijdEindMelking; }
                        else if (bder.MilkEndTime3.Equals(DateTime.MinValue))
                        { bder.MilkEndTime3 = TijdEindMelking; }
                        bder.Changed_By = 2;
                        bder.SourceID = flid;
                        mSavetoDB.SaveBedrprod(bder);
                    }
                    else
                    {
                        bder.UbnId = lFarm.UBNid;
                        bder.ProductionDate = DatumProduktie;
                        //tijdstip-begin-melking
                        bder.MilkStartTime1 = TijdBeginMelking;
                        //tijdstip-eind-melking
                        bder.MilkEndTime1 = TijdEindMelking;
                        bder.Changed_By = 2;
                        bder.SourceID = flid;
                        mSavetoDB.SaveBedrprod(bder);
                    }

                }//if (error
            }//while
            //mDllCall.FreeHandles();
            return regelMelking;
        }

        protected int LeesDagProduktie(int pThrId, String pBestandsnaam, bool IsEVOBestand, ref List<ANALYSE> pAnalyse)
        {
            unLogger.WriteDebug("edinrs_DagProduktie");
            int error = 0;
            int regelDagProduktie = 0;
            String lLevensnr;
            DateTime lDatum;
            int KgMelkX10;
            int PercEiwitX100;
            int PercVetX100;
            int PercLactoseX100;
            int Ureum;
            int MelkCelgetal;
            int IndicatieEMM;



            String Berichttype, Versienr, Specificatie;
            DateTime BestandsDatum, BestandsTijd;





            while (error != -1)
            {
                regelDagProduktie++;
                error = mDllCall.edinrs_DagProduktie(pBestandsnaam, regelDagProduktie,
                    out lLevensnr, out lDatum,
                    out KgMelkX10, out PercEiwitX100, out PercVetX100,
                    out PercLactoseX100, out Ureum, out MelkCelgetal, out IndicatieEMM);
                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    ANALYSE lItem;



                    if (IsEVOBestand)
                        lItem = mSavetoDB.GetAnalyseByKeyAndTypeOfControl(lAniItem.AniId, lDatum.Date, 6);
                    else
                        //if (IndicatieEMM == 1)
                        //    lItem = mSavetoDB.GetAnalyseByKeyAndTypeOfControl(lAniItem.AniId, lDatum.Date, 4);
                        //else
                        lItem = mSavetoDB.GetAnalyseByKeyAndTypeOfControl(lAniItem.AniId, lDatum.Date, 0);


                    lItem.UbnId = lFarm.UBNid;
                    lItem.AniId = lAniItem.AniId;
                    lItem.AnaMilkDate = lDatum;
                    lItem.AnaKgMilk = (double)Decimal.Divide(KgMelkX10, 10);
                    lItem.AnaPercProtein = (double)Decimal.Divide(PercEiwitX100, 100);
                    lItem.AnaPercFat = (double)Decimal.Divide(PercVetX100, 100);
                    lItem.AnaLactose = (double)Decimal.Divide(PercLactoseX100, 100);
                    lItem.AnaUrea = Ureum;
                    lItem.AnaMilkCellcount = MelkCelgetal;
                    if (IsEVOBestand)
                        lItem.AnaTypeOfControl = 6;

                    //else if (IndicatieEMM == 1)
                    //{
                    //    lItem.AnaTypeOfControl = 4;
                    //}
                    pAnalyse.Add(lItem);
                }
            }
            //mDllCall.FreeHandles();
            return regelDagProduktie;
        }

        protected int LeesLactatieProduktie(int pThrId, String pBestandsnaam, ref List<ANALYSE> pAnalyse)
        {
            unLogger.WriteDebug("edinrs_LactatieProduktie");
            List<ANALYSE> lExistingAnalyse = new List<ANALYSE>();
            int error = 0;
            int regelLactatieProduktie = 0;
            String lLevensnr;
            DateTime lDatum;
            int KgMelk;
            int KgEiwitX100;
            int KgVetX100;
            int KgMelk305;
            int KgEiwit305x100;
            int KgVet305x100;
            int LactatieWaarde;
            int NettoOpbrengst;
            int DagenInLactatie;
            int BSKx10;
            int IndicatieEMM;
            int DierStatus;
            while (error != -1)
            {
                regelLactatieProduktie++;
                error = mDllCall.edinrs_LactatieProduktie(pBestandsnaam, regelLactatieProduktie,
                    out lLevensnr, out lDatum,
                    out KgMelk, out KgEiwitX100, out KgVetX100,
                    out KgMelk305, out KgEiwit305x100, out KgVet305x100,
                    out LactatieWaarde, out NettoOpbrengst,
                    out DagenInLactatie, out BSKx10, out IndicatieEMM, out DierStatus);
                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    //ANALYSE lItem = new ANALYSE();
                    ANALYSE lItem = GetAnalyseFromAnalyseList(ref pAnalyse, lAniItem.AniId, lDatum.AddDays(DagenInLactatie));
                    //lItem.AniId = lAniItem.AniId;
                    //lItem.AnaMilkDate = lDatum;
                    if (lItem == null)
                    {

                        lItem = GetAnalyseFromAnalyseList(ref lExistingAnalyse, lAniItem.AniId, lDatum.AddDays(DagenInLactatie));
                        if (lItem == null)
                        {
                            lItem = mSavetoDB.GetAnalyseByKeyAndTypeOfControl(lAniItem.AniId, lDatum.AddDays(DagenInLactatie).Date, 0);
                            if (!mSavetoDB.isFilledByDb(lItem))
                            {
                                lItem = new ANALYSE();
                                lItem.UbnId = lFarm.UBNid;
                                lItem.AniId = lAniItem.AniId;
                                lItem.AnaMilkDate = lDatum.AddDays(DagenInLactatie);
                            }
                        }
                    }
                    lItem.AnaCummulatedKg = KgMelk;
                    if (KgMelk != 0)
                    {
                        lItem.AnaCummulatedPercFat = (double)Decimal.Divide(KgVetX100, KgMelk);
                        lItem.AnaCummulatedPercProtein = (double)Decimal.Divide(KgEiwitX100, KgMelk);
                    }
                    lItem.AnaKgMilk305 = KgMelk305;
                    if (KgMelk305 != 0)
                    {
                        lItem.AnaPercProtein305 = (double)Decimal.Divide(KgEiwit305x100, KgMelk305);
                        lItem.AnaPercFat305 = (double)Decimal.Divide(KgVet305x100, KgMelk305);
                    }
                    lItem.AnaLactationValue = LactatieWaarde;
                    lItem.AnaNettIncome = NettoOpbrengst;
                    lItem.AnaBSKValue = (double)Decimal.Divide(BSKx10, 10);

                    if (!lExistingAnalyse.Contains(lItem))
                    {
                        lItem.Changed_By = 2;
                        lItem.SourceID = flid;
                        if (mSavetoDB.SaveAnalyse(lItem))
                        {
                            if (pAnalyse.Contains(lItem))
                            {
                                pAnalyse.Remove(lItem);
                            }
                            lExistingAnalyse.Add(lItem);
                        }
                        else
                        {
                            unLogger.WriteWarn(String.Format("Fout bij opslaan ANALYSE Levensnr: {0} Datum: {1} UbnId : {2}", lLevensnr, lDatum, lFarm.UBNid));
                        }
                    }

                }
            }
            foreach (ANALYSE lItem in pAnalyse)
            {
                lItem.Changed_By = 2;
                lItem.SourceID = flid;
                if (!mSavetoDB.SaveAnalyse(lItem))
                {
                    unLogger.WriteWarn(String.Format("Fout bij opslaan ANALYSE AniId: {0} Datum: {1} UbnId : {2}", lItem.AniId, lItem.AnaMilkDate, lFarm.UBNid));
                }
            }
            //mDllCall.FreeHandles();
            return regelLactatieProduktie;
        }

        /*
         * oude functie
         *
         */

        protected int LeesProduktie2(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_DagLactatieProduktie");
            int error = 0;
            int regelDagProduktie = 0;
            String lLevensnr;
            DateTime lDatum, DatumAfkalven; //
            int KgMelkDagProdX10;
            int PercEiwitDagProdX100;
            int PercVetDagProdX100;
            int PercLactoseX100;
            int KgMelkLact;
            int KgEiwitLactX100;
            int KgVetLactX100;
            int KgMelk305;
            int KgEiwit305x100;
            int KgVet305x100;
            int LactatieWaarde;
            int Ureum;
            int MelkCelgetal;
            int NettoOpbrengst;
            int DagenInLactatie; //
            int BSKx10;
            int IndicatieEMM;
            int DierStatus;
            while (error != -1)
            {
                regelDagProduktie++;
                error = mDllCall.edinrs_DagLactatieProduktie(pBestandsnaam, regelDagProduktie,
                    out lLevensnr, out lDatum,
            out KgMelkDagProdX10, out PercEiwitDagProdX100, out PercVetDagProdX100,
            out PercLactoseX100, out Ureum, out MelkCelgetal,
            out DatumAfkalven,
            out KgMelkLact, out KgEiwitLactX100, out KgVetLactX100,
            out KgMelk305, out KgEiwit305x100, out KgVet305x100,
            out LactatieWaarde, out NettoOpbrengst,
            out DagenInLactatie, out BSKx10, out IndicatieEMM, out DierStatus);

                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    ANALYSE lItem = new ANALYSE();
                    lItem.UbnId = lFarm.UBNid;
                    lItem.AniId = lAniItem.AniId;
                    lItem.AnaMilkDate = lDatum;
                    lItem.AnaKgMilk = (double)Decimal.Divide(KgMelkDagProdX10, 10);
                    lItem.AnaPercProtein = (double)Decimal.Divide(PercEiwitDagProdX100, 100);
                    lItem.AnaPercFat = (double)Decimal.Divide(PercVetDagProdX100, 100);
                    lItem.AnaLactose = (double)Decimal.Divide(PercLactoseX100, 100);
                    lItem.AnaUrea = Ureum;
                    lItem.AnaMilkCellcount = MelkCelgetal;
                    lItem.AnaCummulatedKg = KgMelkLact;
                    if (KgMelkLact != 0)
                    {
                        lItem.AnaCummulatedPercFat = (double)Decimal.Divide(KgVetLactX100, KgMelkLact);
                        lItem.AnaCummulatedPercProtein = (double)Decimal.Divide(KgEiwitLactX100, KgMelkLact);
                    }
                    lItem.AnaKgMilk305 = KgMelk305;
                    if (KgMelk305 != 0)
                    {
                        lItem.AnaPercProtein305 = (double)Decimal.Divide(KgEiwit305x100, KgMelk305);
                        lItem.AnaPercFat305 = (double)Decimal.Divide(KgVet305x100, KgMelk305);
                    }
                    lItem.AnaLactationValue = LactatieWaarde;
                    lItem.AnaNettIncome = NettoOpbrengst;
                    lItem.AnaBSKValue = (double)Decimal.Divide(BSKx10, 10);
                    lItem.Changed_By = 2;
                    lItem.SourceID = flid;
                    mSavetoDB.SaveAnalyse(lItem);
                }
            }
            //mDllCall.FreeHandles();
            return regelDagProduktie;
        }
        //*/



        protected int LeesEmbryo(int pThrId, String pBestandsnaam)
        {
            unLogger.WriteDebug("edinrs_ImplantatieEmbryo");
            int error = 0;
            int regelEmbryo = 0;
            String lLevensnr;
            DateTime lDatum;
            String lLevensnrETmoeder;
            String lLevensnrstier;

            while (error != -1)
            {
                regelEmbryo++;
                error = mDllCall.edinrs_ImplantatieEmbryo(pBestandsnaam, regelEmbryo,
                    out lLevensnr,
                    out lDatum,
                    out lLevensnrETmoeder, out lLevensnrstier);
                if (error != -1)
                {
                    ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                    TRANSPLA lItem = new TRANSPLA();
                    EVENT lEveItem = mSavetoDB.GetEventByDateAniIdKind(lDatum, lAniItem.AniId, 9);
                    lEveItem.UBNId = lFarm.UBNid;
                    lEveItem.EveDate = lDatum;
                    lEveItem.AniId = lAniItem.AniId;
                    lEveItem.EveKind = 9;
                    lEveItem.EveMutationBy = 3;
                    lEveItem.EveMutationDate = DateTime.Now;
                    lEveItem.EveMutationTime = DateTime.Now;

                    if (lEveItem.EventId == 0)
                    {
                        lItem = new TRANSPLA();

                    }
                    else
                    {
                        lItem = mSavetoDB.GetTranspla(lEveItem.EventId);
                    }
                    lEveItem.Changed_By = 2;
                    lEveItem.SourceID = flid;

                    mSavetoDB.SaveEvent(lEveItem);
                    lItem.EventId = lEveItem.EventId;
                    ANIMAL lBullItem = GetBullFromAnimals(pThrId, lLevensnrstier);
                    if (lBullItem.AniSex == 1 && lBullItem.AniKind == 0)
                    {
                        lBullItem.AniKind = 1;
                        lBullItem.BullShortName = lBullItem.AniName;
                        lBullItem.Changed_By = 2;
                        lBullItem.SourceID = flid;
                        mSavetoDB.UpdateFokstier(lBullItem);
                    }
                    if (lLevensnrETmoeder == String.Empty)
                        lItem.AniIdMother = GetAnimalFromAnimals(pThrId, lLevensnrETmoeder, 5, 2).AniId;
                    lItem.AniIdFather = lBullItem.AniId;

                    if (lBullItem.AniSex != 2)
                    {
                        lBullItem.Changed_By = 2;
                        lBullItem.SourceID = flid;
                        mSavetoDB.SaveTranspla(lItem);
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }

            }
            //mDllCall.FreeHandles();
            return regelEmbryo;
        }

        /*   
        protected int LeesRegistratieRas(String pBestandsnaam, ref List<Race> pBulls)
        {
            int error = 0;
            int regelKIstier = 0;
            String lLevensnr;
            String lCodeRas;
            int lRasdeel = 0;

            while (error != -1)
            {
                regelKIstier++;
                error = mDllCall.edinrs_RegistratieRas(pBestandsnaam, regelKIstier,
                ref lLevensnr, ref lCodeRas, ref lRasdeel);
            }
            return regelKIstier;
        }
        */

        public int IRretour_Terugmelding(int pThrId, String pBestandsnaam)
        {
            int error = 0;
            int regelRIR = 0;
            String UBNnr;
            String lLevensnr;
            String Koenummer;
            String Naam;
            String Geslacht;
            String Haarkleur;
            String LevensnrMoeder;
            String CodeOpvragenNaam;
            DateTime DatumMutatie;
            int SoortMutatie;
            int BijzonderHeden;
            int IndVeeverbetering;
            DateTime DatumMelding;
            DateTime TijdMelding;
            DateTime DatumVerwerking;
            DateTime TijdVerwerking;
            int NummerBron;
            String Herkomst;
            String BerichtID;
            String VersienrBerichtType;
            String ReleasenrBerichtType;
            String ZenderID;
            String TypeMedium;
            String CodeVerwerking;
            String CodeResultaatVerwerking;
            String OmschrijvingResultaat;
            String IKBrund;
            String IRBlokkadeRund;

            while (error != -1)
            {
                regelRIR++;
                error = mDllCall.LeesIRretour_Terugmelding(pBestandsnaam, regelRIR,
                        out UBNnr, out lLevensnr, out Koenummer, out Naam, out Geslacht,
                        out Haarkleur, out LevensnrMoeder, out CodeOpvragenNaam,
                        out DatumMutatie,
                        out SoortMutatie, out BijzonderHeden,
                        out IndVeeverbetering,
                        out DatumMelding, out TijdMelding,
                        out DatumVerwerking, out TijdVerwerking,
                        out NummerBron,
                        out Herkomst, out BerichtID, out VersienrBerichtType,
                        out ReleasenrBerichtType, out ZenderID, out TypeMedium,
                        out CodeVerwerking, out CodeResultaatVerwerking,
                        out OmschrijvingResultaat, out IKBrund,
                        out IRBlokkadeRund);

                ANIMAL lAniItem = GetAnimalFromAnimals(pThrId, lLevensnr);
                ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lAniItem.AniId, lFarm.FarmId);
                switch (SoortMutatie)
                {
                    default:
                        continue;
                }
            }


            return regelRIR;


        }

        protected string removevoorloopnullen(string p)
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
