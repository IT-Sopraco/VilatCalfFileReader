using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.SOAPLNV
{
    public class OpvragenLNVDierstatusV2
    {

        public static void VulDiergegevensvanuitLNV(IFacade Facade, DBConnectionToken pToken, BEDRIJF farm, ANIMAL ani,
            String Lifenr)
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
            //string pLogfile = unLogger.getLogDir("IenR") + "LNVDierstatusV2-" + Lifenr + "-" + DateTime.Today.ToString("yyyy'-'MM'-'dd") +".log";
            string pLogfile =
                unLogger.getLogDir() + "LNVDierstatusV2.log"; // logging aangezet Tasktools lopen massaal vast op de DLL
            int pMaxStrLen = 255;
            String lUsername = "";
            String lPassword = "";
            string pTempBrsnr = "";
            FTPUSER fusoap = Facade.getSaveToDB(pToken).GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9992);
            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }


            //SOAPLNVDieren.LNVDierstatusV2(lUsername, lPassword, 0, Lifenr, farm.ProgId,
            //    ref BRSnrHouder, ref UBNhouder, ref Werknummer, ref Geboortedat,
            //    ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
            //    ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
            //    ref LevensnrMoeder,
            //    ref Status, ref Code, ref Omschrijving,
            //    pLogfile, pMaxStrLen);

            SOAPLNVDieren soapdieren = new SOAPLNVDieren();
            soapdieren.LNVDierDetailstatusV2(lUsername, lPassword, 0, Lifenr, farm.ProgId,
                ref BRSnrHouder, ref UBNhouder, ref Werknummer, ref Geboortedat,
                ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                ref LevensnrMoeder,
                ref Status, ref Code, ref Omschrijving);



            //SOAPLNVDieren.LNVDierdetailsV2(lUsername, lPassword, 0,
            //     Lifenr, farm.ProgId,
            //    1,0,0,
            //    ref UBNhouder, ref BRSnrHouder,
            //    ref Werknummer, ref Geboortedat,
            //    ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
            //    ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
            //    ref LevensnrMoeder,
            //    ref Status, ref Code, ref Omschrijving);

            // meer info ophalen
            ani.AniAlternateNumber = Lifenr;
            ani.AniLifeNumber = Lifenr;
            if (farm.ProgId == 1) ani.Anihaircolor = Facade.getSaveToDB(pToken).GetHcoId(Haarkleur);
            ani.AniBirthDate = Geboortedat;
            if (Geslacht == "M")
                ani.AniSex = 1;
            else if (Geslacht == "V")
                ani.AniSex = 2;
            if (LevensnrMoeder != String.Empty)
            {
                ANIMAL aniMother = Facade.getSaveToDB(pToken).GetAnimalByAniAlternateNumber(LevensnrMoeder);
                if (aniMother.AniId == 0)
                {
                    VulDiergegevensvanuitLNV(Facade, pToken, farm, aniMother, LevensnrMoeder);
                }

                if (ani.AniIdDam == 0)
                {
                    ani.AniIdMother = aniMother.AniId;
                }
            }

            Facade.getSaveToDB(pToken).SaveAnimal(-99, ani);
            ANIMALCATEGORY anicat = Facade.getSaveToDB(pToken).GetAnimalCategoryByIdandFarmid(ani.AniId, farm.FarmId);
            if (anicat.FarmId == 0)
            {
                anicat.AniId = ani.AniId;
                anicat.AniWorknumber = Werknummer;
                anicat.FarmId = farm.FarmId;
                anicat.Anicategory = 5;
                Facade.getSaveToDB(pToken).SaveAnimalCategory(anicat);
            }

        }

        [Obsolete("Gebruik de andere DieropBedrijf functie, bij storing lnv geeft deze functie een exception")]
        public static bool DieropBedrijf(IFacade Facade, DBConnectionToken pToken, BEDRIJF farm, String Lifenr)
        {
            string ActiefUBN;
            return DieropBedrijf(Facade, pToken, farm, Lifenr, out ActiefUBN).Value;
        }

        public static bool? DieropBedrijf(IFacade Facade, DBConnectionToken pToken, BEDRIJF farm, String Lifenr,
            out String ActiefUBN)
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
            String lUsername = "";
            String lPassword = "";
            FTPUSER fusoap = Facade.getSaveToDB(pToken).GetFtpuser(farm.UBNid, farm.Programid, farm.ProgId, 9992);
            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }


            SOAPLNVDieren soapdieren = new SOAPLNVDieren();
            soapdieren.LNVDierDetailstatusV2(lUsername, lPassword, 0, Lifenr, farm.ProgId,
                ref BRSnrHouder, ref UBNhouder, ref Werknummer, ref Geboortedat,
                ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
                ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
                ref LevensnrMoeder,
                ref Status, ref Code, ref Omschrijving);

            //SOAPLNVDieren.LNVDierdetailsV2(lUsername, lPassword, 0,Lifenr, farm.ProgId,
            //        1, 0, 0,
            //        ref UBNhouder, ref BRSnrHouder,
            //        ref Werknummer, ref Geboortedat,
            //        ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong,
            //        ref Geslacht, ref Haarkleur, ref Einddatum, ref RedenEinde,
            //        ref LevensnrMoeder,
            //        ref Status, ref Code, ref Omschrijving);

            // meer info ophalen
            ActiefUBN = UBNhouder;
            if (Status == "G" || Status == "W" || (Status == "F" && Code == "IRD-00192"))
            {

                UBN ubn = Facade.getSaveToDB(pToken).GetubnById(farm.UBNid);
                return ubn.Bedrijfsnummer == UBNhouder;
            }
            else return null;
        }

        [Obsolete("Gebruik de andere DieropBedrijf functie, bij storing lnv geeft deze functie een exception")]
        public static bool DieropBedrijf(AFSavetoDB saveToDb, String Bedrijfsnummer, int pPrognr, String Levensnr)
        {
            string ActiefUBN;
            return DieropBedrijf(saveToDb, Bedrijfsnummer, pPrognr, Levensnr, out ActiefUBN).Value;
        }

        public static bool? DieropBedrijf(AFSavetoDB saveToDb, String Bedrijfsnummer, int pPrognr, String Levensnr,
            out String ActiefUBN)
        {
            string prefix = $"{nameof(OpvragenLNVDierstatusV2)}.{nameof(DieropBedrijf)} -";

            UBN ubn = saveToDb.getUBNByBedrijfsnummer(Bedrijfsnummer);
            //REMARK remark = saveToDb.getFarmRemarksByUbnId(ubn.UBNid).First();
            var bedrs = saveToDb.GetBedrijfByUbnIdProgId(ubn.UBNid, pPrognr);
            //BEDRIJF bedrijf = saveToDb.GetBedrijfById(remark.Farmid);
    

            

            ANIMAL animal = saveToDb.GetAnimalByLifenr(Levensnr);
            Dictionary<AniCatKey, ANIMALCATEGORY> AniCategories = new Dictionary<AniCatKey, ANIMALCATEGORY>();

            if (!bedrs.Any())
            {
                //Logger
                ActiefUBN = null;
                return false;
            }
            Masking m = new Masking();
            string pword =m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
            OpvragenLNVDierDetailsV2.CorrigeerMovementsvanuitLNV(saveToDb, bedrs.FirstOrDefault(), ubn, ConfigurationManager.AppSettings["LNVDierDetailsusername"], pword, 0, animal, Levensnr, 5,
                AniCategories, bedrs.Select(b=> b.FarmId), false, true, 1440, false, 10);
            
            ANIMALCATEGORY aniCategory = AniCategories.Values.FirstOrDefault();

            if (aniCategory != null)
            {
                if (aniCategory.Anicategory >= 4)
                {
                    ActiefUBN = null;
                    return false;
                }

                ActiefUBN = Bedrijfsnummer;
                return true;
            }
            else
           {
               ActiefUBN = null;
               unLogger.WriteDebug($"{prefix} Geen aniCategory bekend voor levensnummer '{Levensnr}' bij ubn '{Bedrijfsnummer}'");
               return true;
           }
        }
    }
}
