using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE.SOAPLNV;
using System.Data;
using System.Diagnostics;
using System.Configuration;

namespace VSM.RUMA.CORE
{
	public class Checker
	{	//eerste insem / draagtijd constants copied from online.utils class..
        public const int DgnEersteInsemSchaap = 180;
        public const int DgnEersteInsemRund = 390;
        public const int DgnEersteInsemGeit = 120;
        public const int Draagtijdrund = 280;// +- 9 maanden
        public const int DraagtijdSchaap = 143;// +- 4 maanden 2 weken
        public const int DraagtijdGeit = 150;// +- 5 maanden

		public static string minimumdatum = "01-01-2000";

        private static AFSavetoDB getMysqlDb(UserRightsToken pUr)
        {
            return Facade.GetInstance().getSaveToDB(pUr);
         
        }

		/// <summary>
		/// check UBN nr validity
		/// </summary>
		/// <param name="ubnnr">UBN nr to check</param>
        /// <param name="isoCountryCode">country code, 528=NL</param>
		/// <returns>IsValid</returns>
        /// 
        public static int getDgnEersteInsem(UserRightsToken pUr, BEDRIJF pBedrijf)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string Lft1eDek = getMysqlDb(pUr).GetFarmConfigValue(pBedrijf.FarmId, "DgnEersteInsem");
            int dagen = 0;
            int.TryParse(Lft1eDek, out dagen);
            if (dagen==0)  
            {
                Lft1eDek = getMysqlDb(pUr).GetProgramConfigValue(pBedrijf.Programid, "DgnEersteInsem");
                int.TryParse(Lft1eDek, out dagen);
                if (dagen == 0)  
                {
                    if (pBedrijf.ProgId == 3) { dagen = DgnEersteInsemSchaap; }//"180";
                    else if (pBedrijf.ProgId == 5) { dagen = DgnEersteInsemGeit; } //"120";
                    else { dagen = DgnEersteInsemRund; }
                }
                //lMstb.SetFarmConfigValue(pBedrijf.FarmId, "DgnEersteInsem", dagen.ToString());
    
            }
            return dagen;
        }

        public static int getDraagtijd(UserRightsToken pUr, BEDRIJF pBedrijf)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string Draagtijd = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "Draagtijd");
            int dagen = 0;
            int.TryParse(Draagtijd, out dagen);
            if (dagen == 0) 
            {
                Draagtijd = lMstb.GetProgramConfigValue(pBedrijf.Programid, "Draagtijd");
                int.TryParse(Draagtijd, out dagen);
                if (dagen == 0)
                {
                    if (pBedrijf.ProgId == 3) { dagen = DraagtijdSchaap; }
                    else if (pBedrijf.ProgId == 5) { dagen = DraagtijdGeit; }
                    else { dagen = Draagtijdrund; }

                }
                //lMstb.SetFarmConfigValue(pBedrijf.FarmId, "Draagtijd", dagen.ToString());

            }
            return dagen;
        }

        public static string checkActieWijziging(UserRightsToken pUr, DateTime pEventDate, int pAniId, BEDRIJF pBedrijf, int pAdminNumber)
        {
            string ret = "";
            if (pAniId < 1)
            {
                ret = VSM_Ruma_OnlineCulture.getStaticResource("", "Dier is niet bekend");
            }
            else
            {
                if (pBedrijf.FarmId < 1)
                {
                    ret = VSM_Ruma_OnlineCulture.getStaticResource("", "Bedrijf is niet bekend");
                }
                else
                {
                    AFSavetoDB lMstb = getMysqlDb(pUr);

                    ANIMAL ani = new ANIMAL();// lMstb.GetAnimalById(pAniId);
                    ANIMALCATEGORY anicat = new ANIMALCATEGORY();// lMstb.GetAnimalCategoryByIdandFarmid(pAniId, pBedrijf.FarmId);
                    lMstb.getAnimalAndCategory(pAniId, pBedrijf.FarmId, out ani, out anicat);
                    if (anicat.FarmId > 0)
                    {
                        if (pEventDate.CompareTo(ani.AniBirthDate) < 0)
                        {
                            ret = VSM_Ruma_OnlineCulture.getStaticResource("datumvoorgeboortedatum", "Datum ligt voor de geboortedatum.");
                        }
                        if (DateTime.Now.AddDays(5).CompareTo(pEventDate) < 0)
                        {
                            ret = VSM_Ruma_OnlineCulture.getStaticResource("", "Datum ligt te ver in de toekomst");
                        }
                        if (ret == "")
                        {
                            int[] admins = { 5 };
                            if (admins.Contains(pAdminNumber))
                            {
                                return "";
                            }
                            else
                            {
                                if (anicat.AniId == 0)
                                {
                                    ret = VSM_Ruma_OnlineCulture.getStaticResource("", "Dit dier is niet van u.");
                                }
                                else
                                {
                                    if (anicat.Ani_Mede_Eigenaar == 1)
                                    {
                                        ret = VSM_Ruma_OnlineCulture.getStaticResource("", "U bent alleen mede-eigenaar.");
                                    }
                                    if (ret == "")
                                    {
                                        if (anicat.Anicategory > 4)
                                        {
                                            ret = VSM_Ruma_OnlineCulture.getStaticResource("nooitaanwezig", "Dit dier is nooit aanwezig geweest");
                                            //BUG 606
                                            bool magwel = checkWelAanwezigGeweest(pUr, pBedrijf, ani, anicat);
                                            if (magwel)
                                            {
                                                ret = "";
                                            }
                                        }
                                        if (ret == "")
                                        {
                                            if (anicat.Anicategory == 4)
                                            {

                                                if (!lMstb.rdDierAanwezig(ani.AniId, pBedrijf.FarmId,pBedrijf.UBNid, pEventDate.Date, 0, anicat.Ani_Mede_Eigenaar, pBedrijf.ProgId))
                                                {
                                                    if (pEventDate.Date < DateTime.Now.Date)
                                                    {
                                                        ret = VSM_Ruma_OnlineCulture.getStaticResource("nietaanwezigop", "Dit dier was niet aanwezig op ") + " " + pEventDate.ToString("dd-MM-yyyy");
                                                    }
                                                    else
                                                    {
                                                        ret = VSM_Ruma_OnlineCulture.getStaticResource("nietaanwezig", "Dit dier is niet aanwezig ");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else { ret = VSM_Ruma_OnlineCulture.getStaticResource("", "Dit dier is niet bij u bekend."); }
                }
            }
            return ret;
        }

        private static bool checkWelAanwezigGeweest(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL ani, ANIMALCATEGORY anicat)
        {
           //BUG 606
            bool ret = false;
            
            if (pBedrijf.FarmId == anicat.FarmId && ani.AniId == anicat.AniId)
            {
                if (anicat.Anicategory > 4)
                {
                    AFSavetoDB lMstb = getMysqlDb(pUr);

                    if (ani.ThrId > 0)
                    {
                        UBN u = lMstb.GetubnById(pBedrijf.UBNid);
                        if (ani.ThrId == u.ThrID)
                        {
                            return true;
                        }
                        else
                        {
                            THIRD lThird = lMstb.GetThirdByThirId(ani.ThrId);
                            if (lThird.ThrZipCode != "" && lThird.ThrExt != "")
                            {
                                List<THIRD> lZelfde = lMstb.GetThirdsByHouseNrAndZipCode(lThird.ThrExt, lThird.ThrZipCode);
                                var zelfde = from n in lZelfde
                                             where n.ThrId == ani.ThrId
                                             select n;
                                if (zelfde.Count() > 0)
                                {
                                    return true;
                                }
                            }
                            if (lThird.ThrCountry == "151")
                            {
                                List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen = new List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats>();
                                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                                {
                                    //       Dieropbedrijf is NIET geboren op dit bedrijf
                                    //       DieropBedrijf stond hier eerst 
                                    return MovFunc.DieropBedrijfGeboren(pUr, pBedrijf,u, lThird, ani.AniAlternateNumber );
                                }
                                else
                                {

                                    //       Dieropbedrijf is NIET geboren op dit bedrijf
                                    return MovFunc.DieropBedrijfGeboren(pUr, pBedrijf, u, lThird, ani.AniLifeNumber );
                                }
                            }
                        }
                    }

                    StringBuilder sm = new StringBuilder();
                    sm.Append(" SELECT (SELECT DISTINCT(TABLE_NAME) FROM information_schema.tables WHERE TABLE_NAME ='BIRTH') AS TABLE_NAME,EVENT.AniId AS Moeder, EVENT.EveDate AS Datum, EVENT.EveKind AS Kind,EVENT.UBNId AS UbnId,EVENT.happened_at_FarmID AS FarmId FROM BIRTH ");
                    sm.Append(" INNER JOIN EVENT ON EVENT.EventId = BIRTH.EventId ");
                    sm.Append(" WHERE CalfId = " + ani.AniId.ToString());
                    sm.Append(" UNION ");
                    sm.Append(" SELECT (SELECT DISTINCT(TABLE_NAME) FROM information_schema.tables WHERE TABLE_NAME ='MOVEMENT') AS TABLE_NAME, MOVEMENT.AniId AS Zelf,  MOVEMENT.MovDate AS Datum,MOVEMENT.MovKind AS Kind,MOVEMENT.UbnId AS UbnId, MOVEMENT.happened_at_FarmID AS FarmId FROM MOVEMENT ");
                    sm.Append(" WHERE MOVEMENT.AniId = " + ani.AniId.ToString() );
                    DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sm);
                    if (tbl.Rows.Count > 0)
                    {
                        DataRow[] ubns = tbl.Select(" UbnId="+ pBedrijf.UBNid.ToString());
                        if (ubns.Count() > 0)
                        {
                            return true;
                        }
                        DataRow[] farms = tbl.Select(" FarmId=" + pBedrijf.FarmId.ToString());
                        if (farms.Count() > 0)
                        {
                            return true;
                        }
                    }
                }
                else { ret = true; }
            }
            
            return ret;
        }

        public static string checkActieDeleting(UserRightsToken pUr, EVENT pEvent, BEDRIJF pBedrijf, int pAdminNumber)
        {
            string ret = "";
            if (pEvent.EventId < 1)
            {
                ret = "Actie is niet bekend";
            }
            else
            {
                if (pBedrijf.FarmId < 1)
                {
                    ret = "Bedrijf is niet bekend";
                }
                else
                {
                    if (pAdminNumber > 5)
                    {
                        AFSavetoDB lMstb = getMysqlDb(pUr);
                        ANIMAL ani = lMstb.GetAnimalById(pEvent.AniId);
                        ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(pEvent.AniId, pBedrijf.FarmId);
                        if (anicat.AniId == 0)
                        {
                            ret = "Dit dier is niet van u.";
                        }
                        else
                        {
                            if (ret == "")
                            {
                                if (anicat.Ani_Mede_Eigenaar == 1)
                                {
                                    ret = "U bent alleen mede-eigenaar.";
                                }
                                if (ret == "")
                                {
                                    if (anicat.Anicategory > 4)
                                    {
                                        ret = "Dit dier is nooit aanwezig geweest";
                                    }
                                    else
                                    {
                                        if (anicat.Anicategory == 4)
                                        {
                                            if (pEvent.UBNId != pBedrijf.UBNid)
                                            {
                                                ret = "Deze actie is niet door u ingebracht.";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }

        public static string IsValidLevensnummer(string pUniqueLifenumber, bool isLNVnr, int pProgId, int pProgramid)
        {
            string foutLifenr = "";
            string antw = VSM_Ruma_OnlineCulture.getStaticResource("ditlevensnummerklopt", " Verkeerd nummer: ");
            if (pUniqueLifenumber.Length > 4)
            {
                if (pProgId == 25)
                {
                    Regex regExNumbers = new Regex(@"^\d{15}$");
                    Match mn = regExNumbers.Match(pUniqueLifenumber);
                    if (!mn.Success)
                    {
                        return " Verkeerd Chipnummer: " + pUniqueLifenumber;
                    }
                    return "";
                }

                Regex regExSpatieOpderdePlek = new Regex(@"^[A-Z]{2}\s[\D\d-]{1,12}$");
                Match msp = regExSpatieOpderdePlek.Match(pUniqueLifenumber);
                if (!msp.Success)
                {
                    foutLifenr = antw + pUniqueLifenumber;
                }
                else
                {
                    if (isLNVnr)
                    {


                        char[] spl = { ' ' };
                        string[] lnv = pUniqueLifenumber.Split(spl);
                        string rest = pUniqueLifenumber.Remove(0, 3);
                        if (lnv[0].Trim().ToUpper() == "BE")//Mailen met jos, 19 dec 2011 
                        {
                         
                            if (rest.Length != 8)
                            {
                                if (pProgId == 3 || pProgId == 5)
                                {
                                    //return "";
                                    while (rest.StartsWith("0"))
                                    {
                                        if (rest.Length > 4)
                                        {
                                            rest = rest.Remove(0, 1);
                                        }
                                        else { break; }
                                    }
                                    
                                    int erbijteller = rest.Length;
                                    rest = utils.addleadingCharacters(rest, '0', erbijteller);
                                    while (rest.Length < 12)
                                    {
                                        if (!DelphiWrapper.controlelevensnr(pProgId, lnv[0].Trim().ToUpper(), rest, false))
                                        {
                                            if (!DelphiWrapper.controlelevensnr(pProgId, lnv[0].Trim().ToUpper(), rest, true))
                                            {
                                                erbijteller += 1;
                                                rest = utils.addleadingCharacters(rest, '0', erbijteller);
                                            }
                                            else { break; }
                                        }
                                        else { break; }
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            //while (rest.StartsWith("0"))
                            //{
                            //    if (rest.Length > 4)
                            //    {
                            //        rest = rest.Remove(0, 1);
                            //    }
                            //    else { break; }
                            //}
                        }
                        if (!DelphiWrapper.controlelevensnr(pProgId, lnv[0], rest, false))
                        {
                            unLogger.WriteInfo(lnv[0] + " " + rest + " afgekeurd door controlnr.pas met elecID=false;");
                            if (!DelphiWrapper.controlelevensnr(pProgId, lnv[0], rest, true))
                            {
                                unLogger.WriteInfo(lnv[0] + " " + rest + " afgekeurd door controlnr.pas met elecID=true;");



                                if (pProgId == 3 || pProgId == 5)
                                {

                                    foutLifenr = antw + pUniqueLifenumber;

                                }
                                else
                                {
                                    foutLifenr = antw + pUniqueLifenumber;
                                }

                            }
                        }
                    }
                    else
                    {
                        if (pUniqueLifenumber.Contains("-"))
                        {
                            if (pProgramid == 51 || pProgramid == 52)
                            {
                                //Regex regNFS = new Regex(@"^([A-Z]{2}\s[0-9a-zA-Z]{5}-[0-9a-zA-Z]{5,6})|([A-Z]{2}\s[\D\d-]{6,12})$");
                                Regex regNFS = new Regex(@"^[A-Z]{2}\s[\D\d-]{6,12}$");
                                Match m = regNFS.Match(pUniqueLifenumber);
                                if (!m.Success)
                                {
                                    foutLifenr = antw + pUniqueLifenumber;
                                }
                            }
                            else if (pProgramid == 16 || pProgramid == 160)
                            {

                                Regex regTES = new Regex(@"^[A-Z]{2}\s[\D\d]{5}-[\D\d]{5}$");
                                Match m = regTES.Match(pUniqueLifenumber);
                                if (!m.Success)
                                {
                                    foutLifenr = antw + pUniqueLifenumber;
                                }

                            }
                            else
                            {
                                Regex regNormal = new Regex(@"^[A-Z]{2}\s[\D\d-]{6,12}$");
                                //Regex reg = new Regex(@"^[A-Z]{2}\s\d{6,12}$");
                                //Regex regNotNL = new Regex(@"^[A-Z]{2}\s\d{6,12}$");
                                //Regex regUniqueNL = new Regex(@"^NL\s\d{12}$");
                                Match m = regNormal.Match(pUniqueLifenumber);
                                if (!m.Success)
                                {
                                    foutLifenr = antw + pUniqueLifenumber;
                                }
                            }
                        }
                        else
                        {
                            char[] spl = { ' ' };
                            string[] lnv = pUniqueLifenumber.Split(spl);
                            string rest = pUniqueLifenumber.Remove(0, 3);
                            if (!DelphiWrapper.controlelevensnr(pProgId, lnv[0], rest, false))
                            {
                                unLogger.WriteInfo(lnv[0] + " " + rest + " afgekeurd door controlnr.pas met elecID=false; isLNVnr=FALSE");
                                if (!DelphiWrapper.controlelevensnr(pProgId, lnv[0], rest, true))
                                {
                                    unLogger.WriteInfo(lnv[0] + " " + rest + " afgekeurd door controlnr.pas met elecID=true; isLNVnr=TRUE");
                                    if (pProgId == 3 || pProgId == 5)
                                    {
                                        if (isLNVnr)
                                        {
                                            foutLifenr = antw + pUniqueLifenumber;
                                        }
                                        else { foutLifenr = antw + pUniqueLifenumber; }

                                    }
                                    else
                                    {
                                        foutLifenr = antw + pUniqueLifenumber;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else 
            {
                if (pProgId == 3 || pProgId == 5)
                {
                    if (isLNVnr)
                    {
                        foutLifenr = antw;
                    }
                    else 
                    {
                        foutLifenr = antw;
                    }
                }
                else 
                {
                    foutLifenr = antw;
                }
            }
            return foutLifenr;
        }
   
        /// <summary>
        /// BUG 1979 Uitgebeidere controle: incl opvragen bij RVO of het dier bekend is.
        /// </summary>
        /// <param name="pUniqueLifenumber"></param>
        /// <param name="isLNVnr"></param>
        /// <param name="pBedrijf"></param>
        /// <param name="pUr"></param>
        /// <param name="pOwnerThird"></param>
        /// <returns></returns>
        public static string IsValidLevensnummer(string pUniqueLifenumber, bool isLNVnr, BEDRIJF pBedrijf, UserRightsToken pUr, THIRD pOwnerThird)
        {
            if (!isLNVnr)
            {
                return IsValidLevensnummer(pUniqueLifenumber, isLNVnr, pBedrijf.ProgId, pBedrijf.Programid);
            }
            else 
            {
                string ret = IsValidLevensnummer(pUniqueLifenumber, isLNVnr, pBedrijf.ProgId, pBedrijf.Programid);
                if (ret.Trim() == "")
                {
                    //if (pOwnerThird.ThrCountry == "151")
                    //{
                    //    if (AnimalExcists(pUniqueLifenumber, isLNVnr, pBedrijf, pUr, pOwnerThird))
                    //    {
                    //        //dit kan niet
                    //        //Het dier bestaat altijd, 
                    //        ret = VSM_Ruma_OnlineCulture.getStaticResource("levsnruniekbestaatal", " Dit uniek levensnummer is al aan een ander dier toegewezen ");
                    //    }
                    //}
                }
                return ret;
            }
        }
   
        public static bool AnimalExcistsAtLNV(string pUniqueLifenumber, bool isLNVnr, BEDRIJF pBedrijf, UserRightsToken pUr, THIRD pOwnerThird)
        {
            bool ret = false;
            if (isLNVnr)
            {
                if (pOwnerThird.ThrCountry == "151")
                {
                    int pMaxStrLen = 255;
                    String lUsername = "";
                    String lPassword = "";
                    FTPUSER fusoap = Facade.GetInstance().getSaveToDB(pUr).GetFtpuser(pBedrijf.UBNid, pBedrijf.Programid, pBedrijf.ProgId, 9992);
                    UBN u = Facade.GetInstance().getSaveToDB(pUr).GetubnById(pBedrijf.UBNid);
                    //int standaardgeslacht = 0;
                    //int.TryParse(Facade.GetInstance().getSaveToDB(pUr).GetFarmConfigValue(pBedrijf.FarmId, "standaardgeslacht", "0"), out standaardgeslacht);
                    //if (standaardgeslacht == 0)
                    //{
                    //    int.TryParse(Facade.GetInstance().getSaveToDB(pUr).GetProgramConfigValue(pBedrijf.Programid, "standaardgeslacht", "0"), out standaardgeslacht);
                    
                    //}
                    if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
                    {

                        lUsername = fusoap.UserName;
                        lPassword = fusoap.Password;
                    }
                    if (lUsername != "" && lPassword != "")
                    {
                        String BRSnr = pOwnerThird.Thr_Brs_Number;

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
                        string pLogfile = String.Empty;
                        Win32SOAPIRALG dllcall = new Win32SOAPIRALG();
                        dllcall.LNVDierdetailsV2(lUsername, lPassword, 0,
                                                    u.Bedrijfsnummer, BRSnr, pUniqueLifenumber, pBedrijf.ProgId,
                                                    0, 0, 0,
                                                    ref LNVprognr, ref Werknummer,
                                                    ref Geboortedat, ref Importdat,
                                                    ref LandCodeHerkomst, ref LandCodeOorsprong,
                                                    ref Geslacht, ref Haarkleur,
                                                    ref Einddatum, ref RedenEinde,
                                                    ref LevensnrMoeder, ref VervangenLevensnr,
                                                    ref Status, ref  Code, ref Omschrijving,
                                                    "", "", pMaxStrLen);
                        try
                        {
                            if (Code != "IRD-00192")
                            {
                                ret = true;
                            }
                        }
                        catch { }
                    }
                }
            }
            return ret;
        }

        public static bool canChangeAniSex(string PcanChangeAniSex, int pAdminNumber)
        {
            bool ret = false;
            switch (pAdminNumber)
            {
                case 10:
                    if(PcanChangeAniSex.ToLower()=="true")
                    {
                        ret = true;
                    }
                    break;
                default:
                    ret = true;
                    break;
            }
            return ret;
        }

        public static bool IsValidUbnNr(string ubnnr, string isoCountryCode)
		{
            if (isoCountryCode == "528")
            {
                string ubnWithoutLeadingZeros = ubnnr.Trim().TrimStart('0');
				try
				{
					long test = long.Parse(ubnWithoutLeadingZeros); //if not numeric or too big (>64bit) return false & log error
				}
				catch (Exception e)
				{ 
					unLogger.WriteError("UBN: " + ubnnr + " " + e.ToString());
					return false;
				}

                if (long.Parse(ubnWithoutLeadingZeros) > 0)
                {
                    if (isoCountryCode == "528")
                    {
                        //if (ubnWithoutLeadingZeros.Length <= 7)
                        //{
                        try
                        {
                            int tmp = ubnWithoutLeadingZeros.Length;
                            if (tmp <= 7)
                            {
                                for (int i = 0; i < 7 - tmp; i++)
                                { ubnWithoutLeadingZeros = "0" + ubnWithoutLeadingZeros; }
                            }
                            int som = int.Parse(ubnWithoutLeadingZeros[0].ToString());
                            som = som + 7 * int.Parse(ubnWithoutLeadingZeros[1].ToString()) +
                                                    3 * int.Parse(ubnWithoutLeadingZeros[2].ToString()) +
                                                    1 * int.Parse(ubnWithoutLeadingZeros[3].ToString()) +
                                                    7 * int.Parse(ubnWithoutLeadingZeros[4].ToString()) +
                                                    3 * int.Parse(ubnWithoutLeadingZeros[5].ToString()) +
                                                    1 * int.Parse(ubnWithoutLeadingZeros[6].ToString());
                            if ((som % 10) == 0)
                            { return true; }//goed ubn
                        }
                        catch { return true; }
                        //}
                    }
                    else
                    { return true; }//gevuld niet-nederlands ubn/bedrijfsnummer
                }
                return false;
            }
            else { return true; }
		}

        public static string checkdekking(UserRightsToken pUr, int AniId, int LfarmId, DateTime checkDate,DateTime pEindDate, int pUpdateEventId, int pAdminNumber)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			
            //Als een dekking niet mag dan een X in de tekst plaatsen
			string antwoord = "";
			BEDRIJF bedereif = lMstb.GetBedrijfById(LfarmId);
			ANIMAL ani = lMstb.GetAnimalById(AniId);
			string ProgID = bedereif.ProgId.ToString();
            int lSoort = 2;
            if (pEindDate > DateTime.MinValue)
            { lSoort = 12; }
            else { pEindDate = checkDate; }
            //FARMCONFIG frmcnfg = lMstb.getFarmConfig(LfarmId, "Lft1eDek");

			//FARMCONFIG frmcnfg = lMstb.getFarmConfig(LfarmId, "DgnEersteInsem");			
            //string Lft1eDek = frmcnfg.FValue;

            int Lft1eDek = getDgnEersteInsem(pUr, bedereif);// lMstb.GetFarmConfigValue(LfarmId, "DgnEersteInsem");
            int Draagtijd = getDraagtijd(pUr, bedereif);

			List<EVENT> dekevnts = lMstb.getEventsByAniIdKindUbn(AniId, 2, int.Parse(bedereif.UBNid.ToString()));
			DateTime laatstedekking = utils.getDate(minimumdatum);
			foreach (EVENT ev in dekevnts)
			{
				if (ev.EveDate.CompareTo(laatstedekking) > 0)
                {
                    if (ev.EventId != pUpdateEventId)
                    {
                        laatstedekking = ev.EveDate;
                    }
				}
			}


			List<EVENT> aflamevnts = lMstb.getEventsByAniIdKind(AniId, 5);
			DateTime laatsteaflamming = utils.getDate(minimumdatum);
			foreach (EVENT evl in aflamevnts)
			{
				if (evl.EveDate.CompareTo(laatsteaflamming) > 0)
				{
					laatsteaflamming = evl.EveDate;
				}
			}
			List<EVENT> drachtigheidsevents = lMstb.getEventsByAniIdKindUbn(AniId, 3, int.Parse(bedereif.UBNid.ToString()));
			DateTime laatstedrachtigheid = utils.getDate(minimumdatum);
			foreach (EVENT evdr in drachtigheidsevents)
			{
				if (evdr.EveDate.CompareTo(laatstedrachtigheid) > 0)
				{
					GESTATIO gest = lMstb.GetGestatio(evdr.EventId);
					if (gest.GesStatus < 3)
					{
						laatstedrachtigheid = evdr.EveDate;
					}
				}
			}

			if (checkDate.CompareTo(System.DateTime.Now) > 0)
			{
				antwoord += "Datum ligt na vandaag.<br />";
			}
			switch (ProgID)
			{
				case "1":
					/*  Default waardes 
								Dekking   450 dagen  na geboorte
									Aflamming na geboortedatum: minimaal 14 maanden. = 400 dagen
						Dekking na positieve drachtverklaring: door gebruiker laten bevestigen

							*/
					if (ani.AniBirthDate.AddDays(Lft1eDek).CompareTo(checkDate) > 0)
					{
						antwoord += "Dekking ligt minder dan " + Lft1eDek + " dagen na geboortedatum. <br />";
					}
					if (checkDate.CompareTo(laatsteaflamming) < 0)
					{
						antwoord += "Dekking ligt voor laatste afkalven.<br />";
					}
					if (checkDate.CompareTo(laatstedekking) < 0)
					{
						antwoord += "Inseminatie ligt voor laatste inseminatie.<br />";
					}
					if (laatstedrachtigheid.CompareTo(laatsteaflamming) > 0)
					{
						if (checkDate.CompareTo(laatstedrachtigheid) > 0)
						{
							antwoord += "Inseminatie ligt na laatste Drachtigheidsverklaring.<br />";
						}
					}
					break;
				case "3":
					/*  Default waardes 
							-	Leeftijd eerste dekking (Lft1eDek): standaard 180 dagen, instelbaar door gebruiker
							-	Dekking minder dan Lft1eDek dagen na geboortedatum: door gebruiker laten bevestigen
							-	Dekking voor een aflamming: door gebruiker laten bevestigen
							-	Dekking voor een andere dekking: door gebruiker laten bevestigen
							-	Geen dekking na positief drachtig mogelijk
							*/
                    if (antwoord == "")
                    {
                        //return antwoord;
                        if (utils.isNsfo(bedereif.Programid) && AniId > 0)
                        {

                            DateTime begin;
                            DateTime eind;
                            CORE.utils.setDekperiode(out begin, out eind);
                            DataTable dtDekkingEventLijst = lMstb.getDekkingEventLijstDekperiode(bedereif.FarmId, begin, eind, bedereif.ProgId);
                            string verwijderen = " Verwijder eerst de vorige dekking";// door te klikken op, Reeds ingevoerde dekkingen en dan op verwijderen.  ";

                            DataRow[] foundrows2 = dtDekkingEventLijst.Select("AniId=" + AniId.ToString() + " AND eveKind=2 ");
                            if (foundrows2.Count() > 0)
                            {
                                for (int i = 0; i < foundrows2.Count(); i++)
                                {
                                    DateTime Evedate = Event_functions.getDatumFormat(foundrows2[i]["EveDate"], "Evedate");
                                    int lEveId = 0;
                                    int.TryParse(foundrows2[i]["EventId"].ToString(), out lEveId);
                                    if (lEveId != pUpdateEventId)
                                    {
                                        if (Evedate.Date >= checkDate.Date.AddDays(-10) && Evedate.Date <= checkDate.Date.AddDays(10))
                                        {
                                            if (lSoort == 2)
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is op deze datum al een andere dekking geregistreerd. " + verwijderen;
                                            }
                                            else
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is in deze periode al een dekking geregistreerd. " + verwijderen;
                                            }
                                        }
                                        else if (checkDate.Date <= Evedate.Date && Evedate.Date <= pEindDate.Date)
                                        {
                                            if (lSoort == 2)
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is op deze datum al een andere dekking geregistreerd. " + verwijderen;
                                            }
                                            else
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is in deze periode al een dekking geregistreerd. " + verwijderen;
                                            }
                                        }
                                    }
                                }
                            }


                            DataRow[] foundrows12 = dtDekkingEventLijst.Select("AniId=" + ani.AniId.ToString() + " AND eveKind=12 ");
                            if (foundrows12.Count() > 0)
                            {
                                for (int j = 0; j < foundrows12.Count(); j++)
                                {
                                    DateTime Evedate = Event_functions.getDatumFormat(foundrows12[j]["EveDate"], "Evedate");
                                    DateTime EndGRdate = Event_functions.getDatumFormat(foundrows12[j]["EndDate"], "Evedate");
                                    int lEveId = 0;
                                    int.TryParse(foundrows12[j]["EventId"].ToString(), out lEveId);
                                    if (lEveId != pUpdateEventId)
                                    {
                                        if (checkDate.Date <= Evedate.Date && pEindDate.Date >= Evedate.Date)
                                        {
                                            if (lSoort == 2)
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is op deze datum al een andere dekking geregistreerd. " + verwijderen;
                                            }
                                            else
                                            {
                                                antwoord = "XVoor " + ani.AniLifeNumber + " is in deze periode al een dekking geregistreerd. " + verwijderen;
                                            }
                                        }
                                        else if (checkDate.Date <= EndGRdate.Date && pEindDate.Date >= EndGRdate.Date)
                                        {
                                            if (lSoort == 2)
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is op deze datum al een andere dekking geregistreerd. " + verwijderen;
                                            }
                                            else
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is in deze periode al een dekking geregistreerd. " + verwijderen;
                                            }
                                        }
                                        else if (checkDate.Date >= Evedate.Date && pEindDate.Date <= EndGRdate.Date)
                                        {
                                            if (lSoort == 2)
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is op deze datum al een andere dekking geregistreerd. " + verwijderen;
                                            }
                                            else
                                            {
                                                antwoord = "X Voor " + ani.AniLifeNumber + " is in deze periode al een dekking geregistreerd. " + verwijderen;
                                            }
                                        }
                                    }
                                }
                            }

                            if (!antwoord.StartsWith("X"))
                            {
                                if (laatsteaflamming.AddDays(210) > (checkDate.Date.AddDays(Draagtijd)))
                                {
                                    antwoord += "Samen met een draagtijd van " + Draagtijd.ToString() + " dagen. ligt de volgende worpdatum minder dan 210 dagen na de vorige worp van  " + laatsteaflamming.ToString("dd-MM-yyyy");      
                                }
                            }
                        }
                    }
                    if (antwoord == "")
                    {
                        if (ani.AniBirthDate.AddDays(Lft1eDek).CompareTo(checkDate) > 0)
                        {
                            antwoord += "Dekking ligt minder dan " + Lft1eDek + " dagen na geboortedatum.<br />";
                        }
                        if (checkDate.CompareTo(laatsteaflamming) < 0)
                        {
                            antwoord += "Dekking ligt voor laatste worp.<br />";
                        }
                        if (checkDate.CompareTo(laatstedekking) < 0)
                        {
                            antwoord += "Dekking ligt voor laatste dekking.<br />";
                        }
                        if (laatstedrachtigheid.CompareTo(laatsteaflamming) > 0)
                        {
                            if (checkDate.CompareTo(laatstedrachtigheid) > 0)
                            {
                                antwoord = "X Geen dekking mogelijk<br />";
                            }
                        }

                    }
					break;
				case "5":
					/*  -   Default waardes 
					 *  -	Leeftijd eerste dekking (Lft1eDek): 120 dagen, instelbaar door gebruiker
							-	Dekking voor laatste aflamming: door gebruiker laten bevestigen
							-	Dekking voor laatste dekking: door gebriuker laten bevestigen
							-	Dekking na positieve drachtverklaring: door gebruiker laten bevestigen
							-	Dekking na droogzetting: door gebruiker laten bevestigen
							-	Dekking minder dan 100 dagen na vorige dekking: door gebruiker laten bevestiigen
							*/
					if (ani.AniBirthDate.AddDays(Lft1eDek).CompareTo(checkDate) > 0)
					{
						antwoord += "Dekking ligt minder dan " + Lft1eDek + " dagen na geboortedatum.<br />";
					}
					if (checkDate.CompareTo(laatsteaflamming) < 0)
					{
						antwoord += "Dekking ligt voor laatste worp.<br />";
					}
					if (checkDate.CompareTo(laatstedekking) < 0)
					{
						antwoord += "Dekking ligt voor laatste dekking.<br />";
					}
					if (laatstedrachtigheid.CompareTo(laatsteaflamming) > 0)
					{
						if (checkDate.CompareTo(laatstedrachtigheid) > 0)
						{
							antwoord += "Dekking ligt na laatste drachtigheidsverklaring.<br />";
						}
					}

					break;
			}
			if (antwoord != "")
			{
				if (!antwoord.Contains("X"))
				{
                    antwoord += "<br />Wilt u deze gegevens toch invoeren?";
				}
                if (pAdminNumber < 6)
                {
                    if (antwoord.StartsWith("X"))
                    {
                        antwoord = antwoord.Remove(0, 1);
                    }
                }
			}
			return antwoord;
		}

        public static string checkaflamming(UserRightsToken pUr, ANIMAL pAnimal, BEDRIJF pBedrijf, DateTime checkDate,int pAdminNumber,out List<EVENT> pVaderEvents,out int pVaderAniId,out int pPMSGdekking)
        {
            pVaderAniId = 0;
            pPMSGdekking = 0;//P.S. is alleen bedoeld als voorstel bij het invoeren van worpen,en niet om dit bij een worp op te slaan.
            pVaderEvents = new List<EVENT>();
            List<EVENT> pNSFO_MultipleFathers = new List<EVENT>();
            int[] pProgramidsNotTocheck = { 102, 103 };
            if (checkDate > DateTime.MinValue.AddYears(1) && pAnimal.AniId > 0 && pBedrijf.FarmId > 0 && pAdminNumber > 0)
            {
                //Stopwatch s = new Stopwatch();
                //s.Start();
                AFSavetoDB lMstb = getMysqlDb(pUr);
                //Als een worp niet mag dan een X in de tekst plaatsen
                //BEDRIJF bedereif = lMstb.GetBedrijfById(LfarmId);
                //ANIMAL ani = lMstb.GetAnimalById(AniId);
                //unLogger.WriteInfo("checkaflamming  animal" + s.ElapsedMilliseconds.ToString());
                string ProgID = pBedrijf.ProgId.ToString();
                string dekverplicht = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "dekkingverplicht");
                List<int> cyclievents = new List<int>();


                bool FarmCanDekken = checkFarmCanDekken(pBedrijf);
     
                string worpnavorigeverplicht = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "dagenworpnavorigeverplicht");

            
                int draagtijd = getDraagtijd(pUr, pBedrijf);
                 

                DateTime lPossibleDekDate = checkDate.AddDays(-draagtijd);
                DateTime lPossibleDekDatemin = lPossibleDekDate.AddDays(-10);
                DateTime lPossibleDekDatemax = lPossibleDekDate.AddDays(10);
                DateTime lMinimumFinalDekDate = lPossibleDekDatemin.AddMonths(-6);//Worpdatum min de draagtijd en min 10 dagen en dan nog eens 6 maanden daarvoor, anders komen de eventuele vaders van het jaar daarvoor in beeld
             
                int[] nsfo = utils.getNsfoProgramIds().ToArray();
               
                string antwoord = "";
                List<EVENT> aflamevnts = new List<EVENT>();
                if (pAnimal.AniId > 0)
                {
                    aflamevnts = lMstb.getEventsByAniIdKind(pAnimal.AniId, 5);
                }
                //unLogger.WriteInfo("checkaflamming  aflamevents" + s.ElapsedMilliseconds.ToString());
                var lastOne = from af in aflamevnts
                              orderby af.EveDate descending, af.EventId descending
                              select af;
                DateTime laatsteaflamming = utils.getDate(minimumdatum).AddYears(1);
                if (lastOne.Count() > 0)
                {
                    foreach (EVENT aflamming in aflamevnts)
                    {
                        if (aflamming.EveDate.Date > laatsteaflamming)
                        {
                            laatsteaflamming = aflamming.EveDate.Date;
                        }
                    }
                }

                bool isvanzelfdeworp = false;
                DateTime laatsteaflammingmin = laatsteaflamming.AddDays(-10);
                DateTime laatsteaflammingmax = laatsteaflamming.AddDays(10);
                if (checkDate.Date.CompareTo(laatsteaflammingmin.Date) >= 0 && checkDate.Date.CompareTo(laatsteaflammingmax.Date) <= 0)
                {
                    isvanzelfdeworp = true;
                    //dan de vader van de laatste pakken
                    if (utils.isNsfo(pBedrijf.Programid))
                    {
                        ANIMAL lFather= new ANIMAL();
                        EVENT pVaderEvent = new EVENT();

                        Event_functions.getWorpFathersNsfo(pUr, pBedrijf, pAnimal, checkDate, out pVaderEvents, out lFather, out pVaderEvent, out pNSFO_MultipleFathers);
                        pVaderAniId = lFather.AniId;
                        INSEMIN lTempins = lMstb.GetInsemin(pVaderEvent.EventId);//als die niet bestaat is Pmsg toch 0
                        pPMSGdekking = lTempins.InsPMSG;
                    }
                    else
                    {
                        BIRTH b = lMstb.GetBirth(lastOne.ElementAt(0).EventId);
                        pPMSGdekking = b.InsPMSG;
                        if (b.AniFatherID > 0)
                        {
                            pVaderAniId = b.AniFatherID;

                        }
                        Event_functions.getWorpFathers(pUr, pBedrijf, pAnimal, checkDate, out pVaderEvents);
                        if (pVaderAniId == 0)
                        {
                            if (pVaderEvents.Count() > 0)
                            {
                                if (pVaderEvents.ElementAt(0).EveKind == 12)
                                {
                                    GRZTOGTH gr = lMstb.GetGRZTOGTHByEventId(pVaderEvents.ElementAt(0).EventId);
                                    pVaderAniId = gr.AniIdFather;
                                }
                                else if (pVaderEvents.ElementAt(0).EveKind == 2)
                                {
                                    INSEMIN gr = lMstb.GetInsemin(pVaderEvents.ElementAt(0).EventId);
                                    pVaderAniId = gr.AniIdFather;
                                   
                                }
                            }
                        }
                    }
                }
                if (!isvanzelfdeworp)
                {
                    DateTime laatstedekking = DateTime.MinValue;// AllerLaatsteDekEvent.EveDate.Date;
                    DateTime samenweidebegin = DateTime.MinValue;// AllerLaatsteDekEvent.EveDate.Date;
                    DateTime samenweideeind = DateTime.MinValue;// AllerLaatsteGrazeTogether.EndDate.Date;
                   
                    int lDekkingEvekind = 2;
                    EVENT AllerLaatsteDekEvent = new EVENT();
                    GRZTOGTH AllerLaatsteGrazeTogether = new GRZTOGTH();
                    EVENT pVaderEvent = new EVENT();
                    if (utils.isNsfo(pBedrijf.Programid))
                    {
                        ANIMAL lFather = new ANIMAL();

                        Event_functions.getWorpFathersNsfo(pUr, pBedrijf, pAnimal, checkDate, out pVaderEvents, out lFather, out pVaderEvent, out pNSFO_MultipleFathers);
                        pVaderAniId = lFather.AniId;

                        if (pVaderEvent.EventId > 0 && pVaderAniId>0 )
                        {
                            lDekkingEvekind = pVaderEvent.EveKind;
                            AllerLaatsteDekEvent = pVaderEvent;
                            laatstedekking = pVaderEvent.EveDate.Date;
                            samenweidebegin = pVaderEvent.EveDate.Date;
                            if (lDekkingEvekind == 12)
                            {
                                GRZTOGTH gr = lMstb.GetGRZTOGTHByEventId(pVaderEvent.EventId);
                                samenweideeind = gr.EndDate;
                            }
                            else 
                            {
                                INSEMIN ins = lMstb.GetInsemin(pVaderEvent.EventId);
                                pPMSGdekking = ins.InsPMSG;
                            }
                        }
                        else if (pVaderEvents.Count() > 0)
                        { 
                            //dan voldoet er geen een aan: if (worpdatum < laatstedekdatum.Date.AddDays(draagtijd + 8))
                            //maar er zijn wel dekkingen
                            int aantal = pVaderEvents.Count();
                            var sorted = from n in pVaderEvents
                                         orderby n.EveDate descending
                                         select n;
                            if (sorted.ElementAt(0).EveKind == 12)
                            {
                                GRZTOGTH gr = lMstb.GetGRZTOGTHByEventId(sorted.ElementAt(0).EventId);
                                samenweideeind = gr.EndDate;
                                lDekkingEvekind = 12;
                            }
                            else
                            {
                                INSEMIN ins = lMstb.GetInsemin(sorted.ElementAt(0).EventId);
                                pPMSGdekking = ins.InsPMSG;
                            }
                            laatstedekking = sorted.ElementAt(aantal - 1).EveDate.Date;
                            samenweidebegin = sorted.ElementAt(aantal-1).EveDate.Date;
                        }
                    }
                    else
                    {
                        StringBuilder sbEvents = new StringBuilder();
                        sbEvents.Append(" SELECT * FROM EVENT WHERE AniId = " + pAnimal.AniId.ToString());
                        sbEvents.Append(" AND ( EveKind = 2 OR EveKind = 12) AND EveDate>" + lMstb.MySQL_Datum(lMinimumFinalDekDate, 0) + " AND EVENT.EventId>0  ORDER BY EveDate DESC ,EventId DESC ");
                        DataTable tblEvents = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sbEvents);
                       
                       
                        if (tblEvents.Rows.Count > 0)
                        {
                            foreach (DataRow rw in tblEvents.Rows)
                            {
                                Event_functions.FillDataObjectFromDataRow(rw, AllerLaatsteDekEvent);
                                laatstedekking = AllerLaatsteDekEvent.EveDate.Date;
                                if (AllerLaatsteDekEvent.EveKind == 12)
                                {
                                    lDekkingEvekind = 12;
                                    AllerLaatsteGrazeTogether = lMstb.GetGRZTOGTHByEventId(AllerLaatsteDekEvent.EventId);
                                }
                                else
                                {
                                    INSEMIN ins = lMstb.GetInsemin(AllerLaatsteDekEvent.EventId);
                                    pPMSGdekking = ins.InsPMSG;
                                }
                                break;
                            }
                        }
                       


                        //de mogelijkheid bestaat dat dit niet  het AllerLaatsteDekEvent is
                        Event_functions.getWorpFathers(pUr, pBedrijf, pAnimal, checkDate, out pVaderEvents);
                        
                        if (pVaderEvents.Count() > 0)
                        {
                            pVaderEvent = pVaderEvents.ElementAt(0);
                            if (pVaderEvents.Count() > 1)
                            {
                                var een = from n in pVaderEvents
                                          where n.EveKind == 2
                                          && (n.EveDate.AddDays(-10) <= (checkDate.AddDays(-draagtijd)) && n.EveDate.AddDays(10) >= (checkDate.AddDays(-draagtijd)))
                                          && n.EveDate > lMinimumFinalDekDate
                                          orderby n.EveDate descending
                                          select n;
                                if (een.Count() > 0)
                                {
                                    pVaderEvent = een.ElementAt(0);
                                }
                                else
                                {
                                    var twee = from n in pVaderEvents
                                               where n.EveKind == 12
                                               && (n.EveDate.AddDays(-10) <= (checkDate.AddDays(-draagtijd)))
                                               && n.EveDate > lMinimumFinalDekDate
                                               orderby n.EveDate descending
                                               select n;
                                    if (twee.Count() > 0)
                                    {
                                        pVaderEvent = twee.ElementAt(0);
                                    }
                                }
                            }
                            //unLogger.WriteInfo("checkaflamming  getWorpFather" + s.ElapsedMilliseconds.ToString());

                            lDekkingEvekind = 2;
                            laatstedekking = pVaderEvent.EveDate.Date;
                            samenweidebegin = pVaderEvent.EveDate.Date;
                            if (pVaderEvent.EveKind == 12)
                            {
                                lDekkingEvekind = 12;
                                GRZTOGTH gr = lMstb.GetGRZTOGTHByEventId(pVaderEvent.EventId);
                                //unLogger.WriteInfo("checkaflamming  GetGRZTOGTHByEventId" + s.ElapsedMilliseconds.ToString());
                                samenweideeind = gr.EndDate;
                                pVaderAniId = gr.AniIdFather;
                            }
                            else
                            {
                                INSEMIN ins = lMstb.GetInsemin(pVaderEvent.EventId);
                                pVaderAniId = ins.AniIdFather;
                                pPMSGdekking = ins.InsPMSG;
                            }

                        }
                    }
                    if (laatstedekking.CompareTo(DateTime.MinValue.Date) == 0)
                    {
                        laatstedekking = laatstedekking.AddYears(1);
                    }
                    if (samenweidebegin.CompareTo(DateTime.MinValue.Date) == 0)
                    {
                        samenweidebegin = samenweidebegin.AddYears(1);
                    }
                    if (samenweideeind.CompareTo(DateTime.MinValue.Date) == 0)
                    {
                        samenweideeind = samenweideeind.AddYears(1);
                    }
                    //"droogzetting"
                    List<EVENT> droogevnts = lMstb.getEventsByAniIdKind(pAnimal.AniId, 4);
                    //unLogger.WriteInfo("checkaflamming  droogevents" + s.ElapsedMilliseconds.ToString());
                    DateTime laatstedroog = utils.getDate(minimumdatum);
                    foreach (EVENT evdrg in droogevnts)
                    {
                        if (evdrg.EveDate.CompareTo(laatstedroog) > 0)
                        {
                            laatstedroog = evdrg.EveDate;
                        }
                    }
                    if (checkDate.CompareTo(System.DateTime.Now) > 0)
                    {
                        antwoord += "Datum ligt na vandaag.<br />";
                    }
                    
                    switch (ProgID)
                    {
                        case "1":
                            /*  RUNDEREN (zelf gekeken)
                                 -	Draagtijd: standaard 280 dagen, instelbaar door gebruiker
                                 -	Afkalven minder dan 400 dagen na geboorte: mag niet.
                                 -	Afkalven minder dan 210 dagen na vorige Afkalven: door gebruiker laten bevestigen
                                 -	Afkalven voor vorige Afkalven: door gebruiker laten bevestigen
                                 -	Afkalven zonder dekking: door gebruiker laten bevestigen
                         */
                            if (pAnimal.AniBirthDate.AddMonths(14).CompareTo(checkDate) > 0)
                            {
                                antwoord = "X Afkalven ligt minder dan 14 maanden na geboortedatum<br />";
                                break;
                            }
                            if (laatsteaflamming.AddDays(210).CompareTo(checkDate) > 0)
                            {
                                if (worpnavorigeverplicht == "1")
                                {
                                    antwoord = "X Afkalving ligt minder dan 210 dagen na laatste afkalving.<br />";
                                    break;
                                }
                                else
                                {
                                    antwoord += "Afkalving ligt minder dan 210 dagen na laatste afkalving.<br />";
                                }
                            }
                            if (checkDate.Date.CompareTo(laatsteaflammingmin.Date) < 0)
                            {
                                antwoord += "Afkalving ligt voor laatste afkalven.<br />";
                            }
                            if (laatstedekking.CompareTo(utils.getDate(minimumdatum)) == 0)
                            {
                                if (dekverplicht == "1")
                                { antwoord = "X Geen inseminatie ingevoerd.<br />"; break; }
                                else 
                                {
                                    if (FarmCanDekken)
                                    {
                                        antwoord += "Geen inseminatie ingevoerd.<br />";
                                    }
                                }
                            }
                            else
                            {
                                if (checkDate.AddDays(210).CompareTo(laatsteaflamming) >= 0)
                                {
                                    if (laatstedekking.CompareTo(laatsteaflamming) < 0)
                                    {
                                        if (dekverplicht == "1")
                                        { antwoord = "X Geen inseminatie ingevoerd.<br />"; break; }
                                        else
                                        {
                                            if (FarmCanDekken)
                                            {
                                                antwoord += "Geen inseminatie ingevoerd.<br />";
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "3":
                            //unLogger.WriteInfo("checkaflamming  case 3 start" + s.ElapsedMilliseconds.ToString());
                            /*  SCHAPEN
                                 -	Draagtijd: standaard 143 dagen, instelbaar door gebruiker
                                 -	Aflamming minder dan 300 dagen na geboorte: door gebruiker laten bevestigen
                                 -	Aflamming minder dan 180 dagen na vorige aflamming: door gebruiker laten bevestigen
                                 -  nsfo laatste verplicht 
                                 -	Aflamming voor vorige aflamming: door gebruiker laten bevestigen
                                 -	Aflamming zonder dekking: door gebruiker laten bevestigen
                                 -	Aflamming voor een dekking: door gebruiker laten bevestigen
                                 -	Aflamming minder dan Draagtijd-10 dagen na dekking: door gebruiker laten bevestigen
                                 -	Aflamming meer dan Draagtijd+10 dagen na dekking: door gebruiker laten bevestigen
                                 */

                            if (pAnimal.AniBirthDate.AddDays(300).CompareTo(checkDate) > 0)
                            {
                                antwoord += "De ingevoerde worp  ligt minder dan 300 dagen na geboortedatum<br />";
                            }
                            if (utils.isNsfo(pBedrijf.Programid))
                            {
                                if ((checkDate > laatsteaflammingmax) && laatsteaflammingmax.AddDays(210).CompareTo(checkDate) > 0)
                                {
                                    antwoord = "X De ingevoerde worp  (" + checkDate.ToString("dd-MM-yyyy") + ") ligt te dicht op de voorgaande worp (" + laatsteaflamming.ToString("dd-MM-yyyy") + ")<br />";
                                    break;
                                }
                            }
                            else
                            {
                                if (laatsteaflammingmax.AddDays(draagtijd).CompareTo(checkDate) > 0)
                                {


                                    if (worpnavorigeverplicht == "1")
                                    {
                                        antwoord = "X De ingevoerde worp  (" + checkDate.ToString("dd-MM-yyyy") + ") ligt te dicht op de voorgaande worp (" + laatsteaflamming.ToString("dd-MM-yyyy") + ")<br />";
                                        break;
                                    }
                                    else
                                    {
                                        antwoord += "De ingevoerde worp  (" + checkDate.ToString("dd-MM-yyyy") + ") ligt te dicht op de voorgaande worp (" + laatsteaflamming.ToString("dd-MM-yyyy") + ")<br />";
                                    }

                                }
                            }
                            if (checkDate.CompareTo(laatsteaflammingmin) < 0)
                            {
                                antwoord += "De ingevoerde worp  ligt voor vorige worp<br />";
                            }
                            if (pVaderEvent.EventId == 0 && AllerLaatsteDekEvent.EventId == 0 && pVaderEvents.Count() == 0)
                            {

                                if (dekverplicht == "1")
                                {
                                    antwoord = "X Geen dekking ingevoerd.<br />"; break;

                                }
                                else
                                {
                                    if (FarmCanDekken)
                                    { antwoord += "Geen dekking ingevoerd.<br />"; }
                                }


                            }
                            if (checkDate.CompareTo(laatsteaflammingmax) > 0)
                            {
                                if (laatstedekking.CompareTo(laatsteaflamming) < 0 && pVaderEvents.Count()==0)
                                {
                                    if (dekverplicht == "1")
                                    { antwoord = "X Geen dekking ingevoerd.<br />"; break; }
                                    else
                                    {
                                        if (!antwoord.Contains("Geen dekking ingevoerd"))
                                        {
                                            if (FarmCanDekken)
                                            {
                                                antwoord += "Verschil tussen dekdatum en werpdatum is meer dan " + draagtijd.ToString() + " dagen";
                                                //antwoord += "Geen dekking  ingevoerd.<br />";
                                            }
                                        }
                                    }
                                }
                            }
                            if (checkDate.CompareTo(laatstedekking) < 0)
                            {
                                antwoord += "De ingevoerde worp  ligt voor laatste dekking.<br />";
                            }
                            if (antwoord == "")
                            {
                                if (laatstedekking.CompareTo(DateTime.MinValue.Date.AddYears(1)) > 0)
                                {
                                    if (lDekkingEvekind == 12)
                                    {
                                        if (utils.isNsfo(pBedrijf.Programid))
                                        {
                                            if (samenweidebegin.CompareTo(laatsteaflamming) > 0)
                                            {
                                                if (checkDate.CompareTo(samenweidebegin.AddDays(draagtijd - 28)) < 0)
                                                {
                                                    antwoord += "De ingevoerde worp  ligt minder dan " + (draagtijd - 28).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + samenweidebegin.AddDays(draagtijd - 28).ToString("dd-MM-yyyy") + " tot " + samenweideeind.AddDays(draagtijd + 8).ToString("dd-MM-yyyy") + "<br />";
                                                }
                                                else if (checkDate.CompareTo(samenweideeind.AddDays(draagtijd + 8)) >= 0)
                                                {
                                                    antwoord += "De ingevoerde worp  ligt meer dan " + (draagtijd + 8).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + samenweidebegin.AddDays(draagtijd - 28).ToString("dd-MM-yyyy") + " tot " + samenweideeind.AddDays(draagtijd + 8).ToString("dd-MM-yyyy") + "<br />";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (samenweidebegin.CompareTo(laatsteaflamming) > 0)
                                            {
                                                if (checkDate.CompareTo(samenweidebegin.AddDays(draagtijd - 10)) < 0)
                                                {
                                                    antwoord += "De ingevoerde worp  ligt minder dan " + (draagtijd).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + samenweidebegin.AddDays(draagtijd - 10).ToString("dd-MM-yyyy") + " tm " + samenweideeind.AddDays(draagtijd + 10).ToString("dd-MM-yyyy") + "<br />";
                                                }
                                                else if (checkDate.CompareTo(samenweideeind.AddDays(draagtijd + 10)) > 0)
                                                {
                                                    antwoord += "De ingevoerde worp  ligt meer dan " + (draagtijd).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + samenweidebegin.AddDays(draagtijd - 10).ToString("dd-MM-yyyy") + " tm " + samenweideeind.AddDays(draagtijd + 10).ToString("dd-MM-yyyy") + "<br />";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (utils.isNsfo(pBedrijf.Programid))
                                        {
                                            if (laatstedekking.CompareTo(laatsteaflamming) > 0)
                                            {
                                                if (checkDate.CompareTo(laatstedekking.AddDays(draagtijd - 28)) < 0)
                                                {

                                                    antwoord += "De ingevoerde worp  ligt minder dan " + (draagtijd-28).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + laatstedekking.AddDays(draagtijd - 28).ToString("dd-MM-yyyy") + " tm " + laatstedekking.AddDays(draagtijd + 8).ToString("dd-MM-yyyy") + "<br />";

                                                }
                                                else if (checkDate.CompareTo(laatstedekking.AddDays(draagtijd + 8)) > 0)
                                                {
                                                    antwoord += "De ingevoerde worp  ligt meer dan " + (draagtijd + 8).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + laatstedekking.AddDays(draagtijd - 28).ToString("dd-MM-yyyy") + " tm " + laatstedekking.AddDays(draagtijd + 8).ToString("dd-MM-yyyy") + "<br />";

                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (laatstedekking.CompareTo(laatsteaflamming) > 0)
                                            {

                                                if (checkDate.CompareTo(laatstedekking.AddDays(draagtijd - 10)) < 0)
                                                {

                                                    antwoord += "De ingevoerde worp  ligt minder dan " + (draagtijd).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + laatstedekking.AddDays(draagtijd - 10).ToString("dd-MM-yyyy") + " tm " + laatstedekking.AddDays(draagtijd + 10).ToString("dd-MM-yyyy") + "<br />";

                                                }
                                                else if (checkDate.CompareTo(laatstedekking.AddDays(draagtijd + 10)) > 0)
                                                {
                                                    antwoord += "De ingevoerde worp  ligt meer dan " + (draagtijd).ToString() + " dagen na de laatste dekking.<br />";
                                                    antwoord += "De worpperiode wordt geaccepteerd van <br />" + laatstedekking.AddDays(draagtijd - 10).ToString("dd-MM-yyyy") + " tm " + laatstedekking.AddDays(draagtijd + 10).ToString("dd-MM-yyyy") + "<br />";

                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            //unLogger.WriteInfo("checkaflamming  case 3 end" + s.ElapsedMilliseconds.ToString());
                            break;
                        case "5":
                            /*-	
                            * GEITEN ELDA
                                         Draagtijd: standaard 150 dagen, instelbaar door gebruiker
                                 -	Aflamming na geboortedatum: minimaal 230 dagen. 
                                         Indien 230-270 dagen dan door gebruiker laten bevestigen
                                 -	Aflamming na vorige aflamming: minimaal 110 dagen. 
                                         Indien 110-Draagtijd(150) dan door gebruiker laten bevestigen.
                                 -	Invoer aflamming: indien geen dekking aanwezig dan vragen of de gebruiker die eerst aan wil maken. 
                                         Indien Nee dan een dekking aanmaken (op datum lamdatum – Draagtijd)
                                 -	Aflamming moet na een "eventuele" droogzetdatum liggen
                                 -	Aflamming moet na een dekking liggen
                                 -	Indien aflamming minder dan Draagtijd-10 dagen na de laatste dekking ligt dan door gebruiker laten bevestigen. 
                                            Behalve als er nog nog dekkingen voor de laatste dekking aanwezig zijn dan de gebruiker laten kiezen uit welke dekking de aflamming is voortgekomen.
                                 -	Indien aflamming meer dan Draagtijd+10 dagen na de laatste dekking ligt dan de gebruiker vragen of hij eerst een dekking in wil voeren. 
                                         Zo nee, dan een dekking aanmaken op datum Lamdatum – Draagtijd.
                                 -	Indien de lamdatum minder dan 150 dagen na sponzen, gust verklaring of verwerpen/afspuiten (zie disease.db) ligt dan door gebruiker laten bevestigen
                                 */
                            if (pAnimal.AniBirthDate.AddDays(230).CompareTo(checkDate) > 0)
                            {
                                antwoord = "X Werpen ligt minder dan 230 dagen na geboortedatum<br />";
                                break;
                            }
                            if (pAnimal.AniBirthDate.AddDays(270).CompareTo(checkDate) >= 0)
                            {
                                antwoord += "Werpen ligt minder dan 270 dagen na geboortedatum<br />";
                            }
                            if (laatsteaflamming.AddDays(110).CompareTo(checkDate) > 0)
                            {
                                antwoord = "X Werpen ligt minder dan 110 dagen na vorige worp<br />";
                                break;
                            }

                            if (laatsteaflamming.AddDays(double.Parse(draagtijd.ToString())).CompareTo(checkDate) > 0)
                            {
                                antwoord += "Werpen ligt minder dan " + draagtijd.ToString() + " dagen na vorige worp<br />";
                            }
                            if (laatstedekking.CompareTo(utils.getDate(minimumdatum)) == 0)
                            {
                                if (dekverplicht == "1")
                                { antwoord = "X Geen dekking ingevoerd.<br />"; break; }
                                else
                                {
                                    if (FarmCanDekken)
                                    { antwoord += "Geen dekking ingevoerd.<br />"; }
                                }
                            }
                            else
                            {
                                if (checkDate.CompareTo(laatsteaflamming) > 0)
                                {
                                    if (laatstedekking.CompareTo(laatsteaflamming) < 0)
                                    {
                                        if (dekverplicht == "1")
                                        { antwoord = "X Geen dekking ingevoerd.<br />"; break; }
                                        else
                                        {
                                            if (FarmCanDekken)
                                            { antwoord += "Geen dekking ingevoerd.<br />"; }
                                        }
                                    }
                                }
                            }
                            if (droogevnts.Count > 0)
                            {
                                if (checkDate.CompareTo(laatstedroog) <= 0)
                                {
                                    antwoord = "X Werpen ligt voor laatste droogzetting.<br />";
                                    break;
                                }
                            }
                            if (checkDate.CompareTo(laatstedekking) <= 0)
                            {
                                antwoord = "X Werpen ligt voor laatste dekking.<br />";
                                break;
                            }
                            double drgtijd2 = double.Parse(draagtijd.ToString());
                            if (laatstedekking.AddDays(drgtijd2 - 10).CompareTo(checkDate) > 0)
                            {
                                antwoord += "Werpen ligt minder dan " + (drgtijd2 - 10).ToString() + " dagen na de laatste dekking.<br />";
                            }
                            if (laatstedekking.AddDays(drgtijd2 + 10).CompareTo(checkDate) < 0)
                            {
                                antwoord = "X Werpen ligt meer dan " + (drgtijd2 + 10).ToString() + " dagen na de laatste dekking. Voer een nieuwe dekking in, of pas de worpdatum aan.<br />";
                                break;
                            }
                            DateTime laatstevanallescheckdate = utils.getDate(minimumdatum);
                            string laatstevanallescheckwoord = "";
                            List<EVENT> sponsenevents = lMstb.getEventsByAniIdKindUbn(pAnimal.AniId, 1, pBedrijf.UBNid);
                            List<EVENT> gestatiogustevents = lMstb.getEventsByAniIdKindUbn(pAnimal.AniId, 3, pBedrijf.UBNid);
                            List<EVENT> diseaseevents = lMstb.getEventsByAniIdKindUbn(pAnimal.AniId, 7, pBedrijf.UBNid);

                            foreach (EVENT ev1 in sponsenevents)
                            {
                                if (ev1.EveDate.CompareTo(laatstevanallescheckdate) > 0)
                                {
                                    laatstevanallescheckdate = ev1.EveDate;
                                    laatstevanallescheckwoord = "sponsen";
                                }
                            }
                            foreach (EVENT ev2 in gestatiogustevents)
                            {
                                GESTATIO ges = lMstb.GetGestatio(ev2.EventId);
                                if (ges.GesStatus == 5)
                                {
                                    if (ev2.EveDate.CompareTo(laatstevanallescheckdate) > 0)
                                    {
                                        laatstevanallescheckdate = ev2.EveDate;
                                        laatstevanallescheckwoord = "gust verklaring";
                                    }
                                }
                            }
                            foreach (EVENT ev3 in diseaseevents)
                            {
                                DISEASE dissie = lMstb.GetDisease(pAnimal.AniId, ev3.EventId);
                                if (dissie.DisSubCode == 125)
                                {
                                    if (ev3.EveDate.CompareTo(laatstevanallescheckdate) > 0)
                                    {
                                        laatstevanallescheckdate = ev3.EveDate;
                                        laatstevanallescheckwoord = @"verwerpen / afspuiten";
                                    }
                                }
                            }
                            if (laatstevanallescheckdate.AddDays(150).CompareTo(checkDate) > 0)
                            {
                                antwoord += "Werpen ligt op minder dan 150 dagen na " + laatstevanallescheckwoord + "<br />";
                            }
                            break;
                    }
                }
                if(pProgramidsNotTocheck.Contains(pBedrijf.Programid))
                {
                    antwoord = "";
                }
                if (utils.isNsfo(pBedrijf.Programid))
                {
                    //dan moeten nu de verkeerde dekevents eruit gehaald worden
                    //kon ze er eerst niet uithalen anders gaf het programma de melding dat er geen dekkingen zijn ingevoerd
                    //maar die zijn er wel, ze voldoen alleen niet aan de eisen.
                    List<EVENT> okEvents = new List<EVENT>();
                    foreach (EVENT ev in pVaderEvents)
                    {
                        if (ev.EveKind == 12)
                        {
                            //dekperiode
                            GRZTOGTH gr = lMstb.GetGRZTOGTHByEventId(ev.EventId);
                            if (checkDate < gr.EndDate.Date.AddDays(draagtijd + 8))
                            {

                                okEvents.Add(ev);
                                if (pVaderAniId == 0)
                                {
                                    pVaderAniId = gr.AniIdFather;
                                }
                            }
                        }
                        else
                        {
                            //dekking
                            if (checkDate < ev.EveDate.Date.AddDays(draagtijd + 5 + 8))
                            {
                                okEvents.Add(ev);
                                if (pVaderAniId == 0)
                                {
                                    INSEMIN ins = lMstb.GetInsemin(ev.EventId);
                                    pVaderAniId = ins.AniIdFather;
                                }
                            }
                        }
                    }
                    //pVaderEvents = okEvents;
                    pVaderEvents = pNSFO_MultipleFathers;
                }
                if (antwoord != "")
                {
                    if (!antwoord.Contains("X"))
                    {
                        antwoord += "<br />Wilt u deze gegevens toch invoeren?";
                     
                    }
                    if (antwoord.Contains("X"))
                    {
                        if (pAdminNumber < 6)
                        {
                            antwoord = antwoord.Remove(0, 1);
                        }
                    }
                }
                //s.Stop();
                return antwoord;
            }
            else { return "Parameter empty"; }
		}

        private static bool checkFarmCanDekken(BEDRIJF pBedrijf)
        {
            try
            {
                XDocument xdRichts = utils.getConfigDoc("App_Data", "rights.xml");
                string events = utils.getRightsElementValue(xdRichts, pBedrijf.Programid, "events");
                string SubTreenodes = utils.getRightsElementValue(xdRichts, pBedrijf.Programid, "SubTreenodes");
                string MainTreenodes = utils.getRightsElementValue(xdRichts, pBedrijf.Programid, "MainTreenodes");
               
                if (SubTreenodes.ToLower().Contains("groepsdekkingen") && MainTreenodes.ToLower().Contains("groepsinvoer"))
                { return true; }
                char[] splitters = { '#', ',', ';' };
                string[] eve = events.Split(splitters);
                if (eve.Contains("2") || eve.Contains("12"))
                {
                    return true;
                }
            }
            catch (Exception exc) { unLogger.WriteError(exc.ToString()); }

            return false;
        }

        public static string allowucverplaatsing(UserRightsToken pUr, int pFarmId, int AniId, int UBNId, int ProgId, int MovId)
		{
			StringBuilder messg = new StringBuilder("");
            AFSavetoDB lMstb = getMysqlDb(pUr);
            BEDRIJF bedr = lMstb.GetBedrijfById(pFarmId);
			//check of het dier al niet is aangevoerd bij een andere ubn
            List<MOVEMENT> aanvoermovs = lMstb.GetMovements(AniId, 1);
            List<MOVEMENT> afvoermovs = lMstb.GetMovements(AniId, 2);
            List<MOVEMENT> doodmovs = lMstb.GetMovements(AniId, 3);
			if (doodmovs.Count > 0)
			{
                try
                {
                    bool magveranderen = false;
                    foreach (MOVEMENT doodmov in doodmovs)
                    {
                        if (doodmov.MovId == MovId)
                        {
                            magveranderen = true;
                            break;
                        }
                    }
                    if (magveranderen == false)
                    {
                        messg.Append("Dit dier is al doodverklaard");
                    }
                }
                catch { }
            }
			else
			{
				if (afvoermovs.Count > 0)
				{
                    try
                    {
                        MOVEMENT afvoermov = afvoermovs.Find(delegate(MOVEMENT p) { return p.MovId == MovId; });
                        if (afvoermov == null)
                        {
                            //messg.Append("Dit dier is al afgevoerd");
                        }
                        //anders wordt alleen de datum geupdate van deze dood
                    }
                    catch { }
				}
			}
			return messg.ToString();
		}

        public static string checkopslaanhuur(UserRightsToken pUr, BEDRIJF pBedr, MOVEMENT mv, int pProgramid)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			string antwoord = "";
            List<MOVEMENT> animovs = lMstb.GetMovementsByAniId(mv.AniId);
            var t = from n in animovs
                    where (n.UbnId == pBedr.UBNid || n.UbnId == 0)
                           && n.MovDate.Date <= mv.MovDate.Date
                    orderby n.MovDate descending, n.MovId descending
                    select n;
           
            if (t.Count() > 0)
            {
                int[] aanwezig = { 1, 4, 7 };
                if (aanwezig.Contains(t.ElementAt(0).MovKind))
                {
                    if (t.ElementAt(0).MovId != mv.MovId)
                    {
                        antwoord = "Dit dier is al aanwezig.";
                    }
                }
             
            }
			return antwoord;
		}

        public static string checkopslaaneindehuur(UserRightsToken pUr, BEDRIJF pBedr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			//er moet een huur voor staan Movkind huur=4
			string antwoord = "";
			BEDRIJF bedr = lMstb.GetBedrijfByUbnIdProgIdProgramid(mv.UbnId, mv.Progid,pProgramID);
			List<MOVEMENT> mofshuren = lMstb.GetMovements(mv.AniId, 4);
			DateTime laatstehuur = DateTime.MinValue;
			foreach (MOVEMENT huur in mofshuren)
			{
				if (huur.MovDate.CompareTo(laatstehuur) > 0)
				{
					laatstehuur = huur.MovDate;
				}
			}
			List<MOVEMENT> mofseindehuren = lMstb.GetMovements(mv.AniId, 5);
			DateTime laatsteeindehuur = DateTime.MinValue;
			foreach (MOVEMENT eindehuren in mofseindehuren)
			{
				if (eindehuren.MovDate.CompareTo(laatsteeindehuur) > 0)
				{
					laatsteeindehuur = eindehuren.MovDate;
				}
			}
			if (laatsteeindehuur.CompareTo(laatstehuur) >= 0 && laatsteeindehuur.CompareTo(DateTime.MinValue) > 0)
			{
				antwoord = " Er is er al een Einde huur "; //+  lMstb.getSingleLabLabel(LabId,LabKind,Labcountry);
			}
			if (laatstehuur.CompareTo(DateTime.MinValue) == 0)
			{
				antwoord = " Voer eerst een Huur in";
			}

			return antwoord;
		}

        public static string checkremovehuur(UserRightsToken pUr, BEDRIJF pBedr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			//er mag geen einde huur nog erna staan.
			//eerst einde huur verwijderen.
			string antwoord = "";
            List<MOVEMENT> mofshuren = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 4, pBedr.UBNid);
            List<MOVEMENT> mofshurenNul = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 4, 0);
            mofshuren.Concat(mofshurenNul);

			DateTime laatstehuur = DateTime.MinValue;
			foreach (MOVEMENT huur in mofshuren)
			{
				if (huur.MovDate.CompareTo(laatstehuur) > 0)
				{
					laatstehuur = huur.MovDate;
				}
			}
            List<MOVEMENT> mofseindhuren = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 5, pBedr.UBNid);
            List<MOVEMENT> mofseindhurenNul = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 5, 0);
            mofseindhuren.Concat(mofseindhurenNul);

			DateTime laatsteeindehuur = DateTime.MinValue;
			foreach (MOVEMENT eindehuren in mofseindhuren)
			{
				if (eindehuren.MovDate.CompareTo(laatsteeindehuur) > 0)
				{
					laatsteeindehuur = eindehuren.MovDate;
				}
			}

			if (mv.MovDate.CompareTo(laatsteeindehuur) <= 0)
			{
				//antwoord = "Verwijder eerst einde huur";
			}
			return antwoord;
		}

        public static string checkremoveeindehuur(UserRightsToken pUr, BEDRIJF pBedr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			//er moet nog een huur voor staan
			string antwoord = "";
          
			List<MOVEMENT> mofshuren = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 4,pBedr.UBNid);
            List<MOVEMENT> mofshurenNul = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 4, 0);
            mofshuren.Concat(mofshurenNul);


			DateTime laatstehuur = DateTime.MinValue;
			foreach (MOVEMENT huur in mofshuren)
			{
				if (huur.MovDate.Date.CompareTo(laatstehuur.Date) >= 0)
				{
					laatstehuur = huur.MovDate;
				}
			}
            List<MOVEMENT> mofseindhuren = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 5, pBedr.UBNid);
            List<MOVEMENT> mofseindhurenNul = lMstb.GetMovementsByAniIdMovkindUbn(mv.AniId, 5, 0);
            mofseindhuren.Concat(mofseindhurenNul);

			DateTime laatsteeindehuur = DateTime.MinValue;
            foreach (MOVEMENT eindehuren in mofseindhuren)
			{
				if (eindehuren.MovDate.Date.CompareTo(laatsteeindehuur.Date) >= 0)
				{
					laatsteeindehuur = eindehuren.MovDate;
				}
			}
			if (mv.MovDate.CompareTo(laatstehuur) <= 0)
			{
				//antwoord = "Verwijder eerst  huur";
			}
			return antwoord;

		}

        public static string checkopslaanuitscharen(UserRightsToken pUr,BEDRIJF pBedr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			// dier moet aanwezig zijn category = 1 of 2 of 3
			string antwoord = "";
            List<MOVEMENT> animovs = lMstb.GetMovementsByAniId(mv.AniId);
            var t = from n in animovs
                    where (n.UbnId == pBedr.UBNid || n.UbnId == 0)
                           && n.MovDate.Date < mv.MovDate
                    orderby n.MovDate descending, n.MovId descending
                    select n;
           
            if (t.Count() > 0)
            {
                ANIMALCATEGORY catje = lMstb.GetAnimalCategoryByIdandFarmid(mv.AniId, pBedr.FarmId);
                if (catje.Ani_Mede_Eigenaar == 0)
                {
                    int[] afwezig={2,3,5,6};
                    if (afwezig.Contains(t.ElementAt(0).MovKind))
                    {
                        if (t.ElementAt(0).MovId != mv.MovId)
                        {
                            antwoord = "Dier is al afwezig";
                        }
                    }
                }
                else { antwoord = "U bent alleen medeeigenaar."; }
            }
			return antwoord;
		}

        public static string checkopslaaneindeuitscharen(UserRightsToken pUr, BEDRIJF pBedr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			// er moet een uitscharen zijn ervoor
			string antwoord = "";
     
			List<MOVEMENT> mofsuitscharen = lMstb.GetMovements(mv.AniId, 6);
			DateTime laatsteuitscharen = DateTime.MinValue;
			foreach (MOVEMENT huur in mofsuitscharen)
			{
				if (huur.MovDate.CompareTo(laatsteuitscharen) > 0)
				{
					laatsteuitscharen = huur.MovDate;
				}
			}
			List<MOVEMENT> mofseindeuitscharen = lMstb.GetMovements(mv.AniId, 7);
			DateTime laatsteeindeuitscharen = DateTime.MinValue;
			foreach (MOVEMENT eindeuitscharenm in mofseindeuitscharen)
			{
				if (eindeuitscharenm.MovDate.CompareTo(laatsteeindeuitscharen) > 0)
				{
					laatsteeindeuitscharen = eindeuitscharenm.MovDate;
				}
			}
			if (laatsteuitscharen.CompareTo(laatsteeindeuitscharen) < 0)
			{
				//antwoord = "Er is geen uitscharen";
			}
			return antwoord;
		}

        public static string checkremoveuitscharen(UserRightsToken pUr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			//er mag geen eindeuitscharen er achter staan
			string antwoord = "";
            BEDRIJF bedr = lMstb.GetBedrijfByUbnIdProgIdProgramid(mv.UbnId, mv.Progid, pProgramID);
			List<MOVEMENT> mofsuitscharen = lMstb.GetMovements(mv.AniId, 6);
			DateTime laatsteuitscharen = DateTime.MinValue;
			foreach (MOVEMENT huur in mofsuitscharen)
			{
				if (huur.MovTime.CompareTo(laatsteuitscharen) > 0)
				{
					laatsteuitscharen = huur.MovTime;
				}
			}
			List<MOVEMENT> mofseindeuitscharen = lMstb.GetMovements(mv.AniId, 7);
			DateTime laatsteeindeuitscharen = DateTime.MinValue;
			foreach (MOVEMENT eindeuitscharenm in mofseindeuitscharen)
			{
				if (eindeuitscharenm.MovTime.CompareTo(laatsteeindeuitscharen) > 0)
				{
					laatsteeindeuitscharen = eindeuitscharenm.MovTime;
				}
			}
			if (mv.MovTime.CompareTo(laatsteeindeuitscharen) <= 0)
			{
				//antwoord = "Verwijder eerst einde uitscharen";
			}
			return antwoord;
		}

        public static string checkremoveeindeuitscharen(UserRightsToken pUr, MOVEMENT mv, int pProgramID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
			//er moet nog een uitscharen voor staan
			string antwoord = "";
            BEDRIJF bedr = lMstb.GetBedrijfByUbnIdProgIdProgramid(mv.UbnId, mv.Progid, pProgramID);
			List<MOVEMENT> mofsuitscharen = lMstb.GetMovements(mv.AniId, 6);
			DateTime laatsteuitscharen = DateTime.MinValue;
			foreach (MOVEMENT huur in mofsuitscharen)
			{
				if (huur.MovTime.CompareTo(laatsteuitscharen) > 0)
				{
					laatsteuitscharen = huur.MovTime;
				}
			}
			List<MOVEMENT> mofseindeuitscharen = lMstb.GetMovements(mv.AniId, 7);
			DateTime laatsteeindeuitscharen = DateTime.MinValue;
			foreach (MOVEMENT eindeuitscharenm in mofseindeuitscharen)
			{
				if (eindeuitscharenm.MovTime.CompareTo(laatsteeindeuitscharen) > 0)
				{
					laatsteeindeuitscharen = eindeuitscharenm.MovTime;
				}
			}
			if (laatsteuitscharen.CompareTo(mv.MovTime) >= 0)
			{
				//antwoord = "Er klopt iets niet.";
			}
			return antwoord;
		}

        public static string checkopslaanlokatie(UserRightsToken pUr, ANIMAL ani, DateTime opsladatum, BEDRIJF bedr, MOVEMENT mv)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            //movkind=8
            //voor geboortedatum is al gedaan
            //niet dood en niet afgevoerd en dus niet anicategory > 3
            List<MOVEMENT> h = lMstb.GetMovements(ani.AniId, 8);
            ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, bedr.FarmId);
            if (anicat.Anicategory == 4)
            { antwoord = "Dier is al afgevoerd "; }
            if (anicat.Anicategory == 5)
            { antwoord = "Dier is  niet aanwezig "; }
            if (opsladatum.CompareTo(DateTime.Now.AddDays(5)) > 0)
            { antwoord = "Datum ligt meer dan 5 dagen in de toekomst "; }
            else
            {
                //mocht er een FARMCONFIG voor datumcheck komen dan wel doen
                //if (mv.MovId != 0)
                //{
                //    if (mv.MovDate.CompareTo(opsladatum) != 0)
                //    {
                //        DateTime laatstedatumervoor = DateTime.MinValue;
                //        DateTime laatstedatumerna = DateTime.Now.AddDays(5);
                //        foreach (MOVEMENT oldmoffie in h)
                //        {
                //            if (oldmoffie.UbnId == bedr.UBNid)
                //            {
                //                if (oldmoffie.MovId != mv.MovId)
                //                {
                //                    if (oldmoffie.MovDate.CompareTo(mv.MovDate) < 0)
                //                    {
                //                        if (oldmoffie.MovDate.CompareTo(laatstedatumervoor) > 0)
                //                        {
                //                            laatstedatumervoor = oldmoffie.MovDate;
                //                        }
                //                    }
                //                    else
                //                    {
                //                        if (laatstedatumerna.CompareTo(oldmoffie.MovDate) > 0)
                //                        {
                //                            laatstedatumerna = oldmoffie.MovDate;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //        if (laatstedatumervoor.CompareTo(opsladatum) > 0)
                //        {
                //            antwoord = "Datum ligt voor de vorige lokatie verplaatsing";
                //        }
                //        if (laatstedatumerna.CompareTo(opsladatum) < 0)
                //        {
                //            antwoord = "Datum ligt na de volgende lokatie verplaatsing";
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (MOVEMENT moffie in h)
                //    {
                //        if (moffie.UbnId == bedr.UBNid)
                //        {
                //            if (moffie.MovDate.CompareTo(opsladatum) > 0)
                //            {
                //                antwoord = "Datum ligt voor een andere lokatie verplaatsing. Dit is niet toegestaan.";
                //                break;
                //            }
                //        }
                //    }

                //}
            }
            return antwoord;
        }

        public static string checkremovelokatie(UserRightsToken pUr, MOVEMENT mv, ANIMAL ani, BEDRIJF bedr)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            //niet dood en niet afgevoerd en dus niet anicategory > 3
            ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, bedr.FarmId);
            if (anicat.Anicategory == 4)
            { antwoord = "Dier is al afgevoerd "; }
            if (anicat.Anicategory == 5)
            { antwoord = "Dier is  niet aanwezig "; }
            return antwoord;
        }

        public static string checkGroupsnummer(UserRightsToken pUr, int oudnummer, int nieuwnummer, BEDRIJF bedr)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            List<GROUPS> groepen = lMstb.GetGroupsList(bedr.FarmId);
            foreach (GROUPS gr in groepen)
            {
                if (gr.GroupId == nieuwnummer)
                {
                    if (gr.GroupId != oudnummer)
                    {

                        antwoord = VSM_Ruma_OnlineCulture.getStaticResource("nummeralingebruik", "Nummer is al in gebruik");

                        break;
                    }
                }
            }
            return antwoord;
        }

        public static string checkremovemedicine(UserRightsToken pUr, int MediId, int FarmId, int UbnId)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            List<MEDPLANM> medmm = lMstb.GetMedPlanMMen(FarmId);
            foreach (MEDPLANM medm in medmm)
            {
                List<MEDPLAND> meddd = lMstb.GetMedPlanDDen(medm.Internalnr.ToString());
                foreach (MEDPLAND med in meddd)
                {
                    if (med.MedId == MediId)
                    {
                        antwoord = "Medicijn is in gebruik, u kunt deze niet verwijderen.";
                        break;
                    }
                }
            }
            if (antwoord == "")
            {
                StringBuilder sb = new StringBuilder("select * from TREATMEN INNER JOIN EVENT ON TREATMEN.EventId=EVENT.EventId WHERE EVENT.UbnId=" + UbnId.ToString() + " AND EVENT.EventId>0  AND TREATMEN.MedId = " + MediId.ToString());

                DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sb);
                if (tbl.Rows.Count > 0)
                {
                    antwoord = "Medicijn is in gebruik, u kunt deze niet verwijderen.";
                }
            }
            return antwoord;
            
        }

        public static string checkChangeFather(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, int pNewFatherId)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");
            if (FCIRviaModem.FValue.ToLower() == "true")
            {
                List<MUTATION> muts = lMstb.GetMutationsByUbn(pBedrijf.UBNid);
                string checklevensnummer = "";
                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    checklevensnummer = pAnimal.AniAlternateNumber;
                }
                else
                {
                    checklevensnummer = pAnimal.AniLifeNumber;
                }
                bool isalgemeld = true;
                foreach (MUTATION mut in muts)
                {
                    if (mut.Lifenumber == checklevensnummer)
                    {
                        if (mut.CodeMutation == 2)
                        {
                            isalgemeld = false;
                        }
                    }
                }
                if (isalgemeld)
                {
                    antwoord = "De geboorte is al bij RVO gemeld";
                }
            }
            return antwoord;
        }
  
        public static string checkChangeMother(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, int pNewMotherId)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");
            if (FCIRviaModem.FValue.ToLower() == "true")
            {
                List<MUTATION> muts = lMstb.GetMutationsByUbn(pBedrijf.UBNid);
                string checklevensnummer = "";
                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    checklevensnummer = pAnimal.AniAlternateNumber;
                }
                else 
                {
                    checklevensnummer = pAnimal.AniLifeNumber;
                }
                bool isalgemeld = true;
                foreach (MUTATION mut in muts)
                {
                    if (mut.Lifenumber == checklevensnummer)
                    {
                        if (mut.CodeMutation == 2)
                        {
                            isalgemeld = false;
                        }
                    }
                }
                if (isalgemeld)
                {
                    antwoord = "De geboorte is al bij RVO gemeld";
                }
            }
            return antwoord;
        }

        public static bool checkremoveMedplanm(UserRightsToken pUr, int pInternalnr, int pUbnId)
        {
            StringBuilder b = new StringBuilder();
            b.Append("SELECT TREATMEN.EventId FROM TREATMEN INNER JOIN EVENT ON ");
            b.Append("TREATMEN.EventId = EVENT.EventId ");
            b.AppendFormat("WHERE EVENT.EveKind = 6 AND EVENT.UBNId= {0} AND EVENT.EventId>0  AND TREATMEN.TreMedPlannr = {1}", pUbnId, pInternalnr);
            DataTable tbl = Facade.GetInstance().getSaveToDB(pUr).GetDataBase().QueryData(pUr.getLastChildConnection(), b);
            if (tbl.Rows.Count > 0)
            { return false; }
            else { return true; }
        }

        public static string checkSupplyGroupUpdate(UserRightsToken pUr, int pSupplyId, int pGroupId, double pKg,string pKind)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);

            SUPPLY1 lCheckSupply = lMstb.GetSupply(pSupplyId);
            double maxVolume = lCheckSupply.SupVolume;

            SUPPLY1_GROUP sg = lMstb.getSupplyGroup(pSupplyId, pGroupId);
            if (sg == null)
            {
                sg = new SUPPLY1_GROUP();
                sg.SupplyId = pSupplyId;
                sg.GroupId = pGroupId;
            }

            List<SUPPLY1_GROUP> lSupGrops = lMstb.getSupplyGroups(pSupplyId);
            double TotalFills = 0;
            foreach (SUPPLY1_GROUP lSupGrop in lSupGrops)
            {
                if (lSupGrop.GroupId != pGroupId)
                {
                    TotalFills = TotalFills + lSupGrop.SupVolume;
                }
            }
            if (TotalFills + pKg > maxVolume)
            {
                if (pKind == "voer")
                {
                    return "Er is meer voer toegewezen dan de voerlevering bevat.";
                }
                else if (pKind == "medicijnen")
                {
                    return "Er is meer medicijn toegewezen dan de levering bevat.";
                }
                else 
                {
                    return "Er is meer voer toegewezen dan de voerlevering bevat.";
                }
            }
            else 
            {
                return "";
            }
        }

        public static string checkRemoveGroups(UserRightsToken pUr, int pGroupsId, BEDRIJF pBedr)
        {
            string ret = "";
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            List<MOVEMENT> movs = lMstb.getMovementsByBedrijf(pBedr);
            foreach (MOVEMENT m in movs)
            {
                if (m.Groupnr == pGroupsId)
                {
                    ret = "Deze groep is nog in gebruik, u kunt deze niet verwijderen.";
                    break;
                }
            }
            return ret;
        }

        public static bool isRammenRingBull(UserRightsToken pUr, ANIMAL pZoekAnimal)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            List<BULLUBN> getbullies = lMstb.GetBullUbnListByAniID(pZoekAnimal.AniId);
         
            StringBuilder b = new StringBuilder();
            b.Append(" SELECT DISTINCT BEDRIJF.FarmId FROM BEDRIJF ");
            //b.Append(" INNER JOIN agrofactuur.BEDRIJF ON BULLUBN.FarmId = agrofactuur.BEDRIJF.FarmId ");
            b.Append(" WHERE BEDRIJF.ProgramId =  52 AND BEDRIJF.FarmId>0");
            DataTable tbl = lMstb.GetDataBase().QueryData(pUr, b);
            int aantalbedrijven = tbl.Rows.Count;
       
            if (aantalbedrijven > 1)
            {
                List<int> farms = new List<int>();
                foreach (DataRow rw in tbl.Rows)
                {
                    int f = 0;
                    int.TryParse(rw[0].ToString(), out f);
                    if (f > 0)
                    {
                        if (!farms.Contains(f))
                        {
                            farms.Add(f);
                        }
                    }
                }
                if (getbullies.Count > 1)
                {
                    try
                    {
                        var temp = from n
                            in getbullies
                                   where farms.Contains(n.FarmId)
                                   select n;
                        if (temp.Count() == aantalbedrijven)
                        {
                            return true;
                        }
                    }
                    catch { return false; }
                }
            }
            return false;
        }

        public static string checkDrachtigheid(UserRightsToken pUr, int pAniId, int pFarmId, DateTime pEventDate, int pAdminNumber)
        {
            string ret = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            //Als een droog niet mag dan een X in de tekst plaatsen
            BEDRIJF bedereif = lMstb.GetBedrijfById(pFarmId);
           if (pAdminNumber > 5)
           {
                List<AGRO_LABELS> lbls = lMstb.GetAgroLabels(CORE.DB.LABELSConst.labKind.EVEKIND, int.Parse(utils.getLabelsLabcountrycode()), bedereif.Programid, bedereif.ProgId);
                string inseminatie = "inseminatie";
                var insemin = from n in lbls
                              where n.LabID == 2
                              select n;
                if (insemin.Count() > 0)
                {
                    inseminatie = insemin.ElementAt(0).LabLabel;
                }
                List<EVENT> allevents = lMstb.getEventsByAniId(pAniId);

                var alls = from af in allevents
                           where af.EveDate.Date.CompareTo(pEventDate.Date) <= 0 
                           && (af.EveKind==2 ||af.EveKind==12 || af.EveKind==5  )
                           orderby af.EveDate descending, af.EventId descending
                           select af;
                if (alls.Count() > 0)
                {
                    if (alls.ElementAt(0).EveKind == 5)
                    {

                        ret = "Geen dekking ingevuld";
                       
                    }
                }
                else
                {

                    ret = "Geen dekking ingevuld.";// Dit dier heeft voor deze datum nog geen " + inseminatie + ".";
                   
                }
               
            }
            return ret;
        }

        public static string checkDroogZetting(UserRightsToken pUr, int pAniId, int pFarmId, DateTime pEventDate, int pAdminNumber)
        {
            string diersoort = "kalf";

            string ret = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            //Als een droog niet mag dan een X in de tekst plaatsen
            BEDRIJF bedereif = lMstb.GetBedrijfById(pFarmId);
            if (pAdminNumber > 5)
            {
                if (bedereif.ProgId == 3 || bedereif.ProgId == 5)
                {
                    diersoort = "lam";
                }
                List<EVENT> deks1 = lMstb.getEventsByAniIdKind(pAniId, 2);
                List<EVENT> deks2 = lMstb.getEventsByAniIdKind(pAniId, 12);
                deks1 = deks1.Concat(deks2).ToList();
                var deks = from d in deks1
                           orderby d.EveDate descending, d.EventId descending
                           select d;
                if (deks.Count() > 0)
                {
                    if (pEventDate.Date.CompareTo(deks.ElementAt(0).EveDate.Date) < 0)
                    {
                        ret = "X Droogzetdatum ligt voor de laatste inseminatiedatum. ";
                    }
                    if (pEventDate.Date.AddDays(200).CompareTo(deks.ElementAt(0).EveDate.Date) < 0)
                    {
                        ret = " droogzetdatum ligt minder dan 200 dagen na de laatste inseminatiedatum. ";
                    }
                }
                List<EVENT> afkalven1 = lMstb.getEventsByAniIdKind(pAniId, 5);
                var afkalven = from af in afkalven1
                               orderby af.EveDate descending, af.EventId descending
                               select af;
                DateTime lLaatsteAfkalfDate = DateTime.MinValue;
                if (afkalven.Count() > 0)
                {
                    lLaatsteAfkalfDate = afkalven.ElementAt(0).EveDate.Date;
                    if (pEventDate.Date.CompareTo(afkalven.ElementAt(0).EveDate.Date) < 0)
                    {
                        ret = " droogzetdatum ligt voor de laatste " + diersoort + "datum.<br />Wilt u deze  gegevens toch invoeren?";
                    }
                }
                else
                {
                    if (bedereif.ProgId == 3 || bedereif.ProgId == 5)
                    {
                        ret = "X Dit dier heeft nog niet gelamd. ";
                    }
                    else
                    {
                        ret = "X Dit dier heeft nog niet gekalfd. ";
                    }

                }
                List<EVENT> drgs1 = lMstb.getEventsByAniIdKind(pAniId, 4);
                var drgs = from dr in drgs1
                           orderby dr.EveDate descending, dr.EventId descending
                           select dr;
                if (drgs.Count() > 0)
                {
                    if (lLaatsteAfkalfDate.CompareTo(DateTime.MinValue) > 0 && lLaatsteAfkalfDate.CompareTo(pEventDate) < 0)
                    {
                        if (drgs.ElementAt(0).EveDate.Date.CompareTo(lLaatsteAfkalfDate.Date) > 0)
                        {
                            //dan is er droog 
                            ret = "X Dit dier staat al droog.";
                        }
                    }
                }
            }
            return ret;
        }

        public static string checkSamenweiden(UserRightsToken pUr,BEDRIJF pBedrijf,ANIMAL pBull, DateTime pStartDate, DateTime pEndDate)
        {
            //Check of the bull op het bedrijf was in die periode
            string antwoord = "";
     
                if (pBull.AniId > 0 && pBull.AniSex == 1)
                {
                    if (pStartDate.CompareTo(DateTime.MinValue.AddYears(1)) > 0)
                    {
                        if (pEndDate.CompareTo(DateTime.MinValue.AddYears(1)) > 0)
                        {
                            if (pEndDate.CompareTo(pStartDate) < 0)
                            {
                                DateTime temp = pEndDate;
                                pEndDate = pStartDate;
                                pStartDate = temp;
                            }
                            pStartDate = pStartDate.AddDays(-10);
                            pEndDate = pEndDate.AddDays(10);
                            AFSavetoDB lMstb = getMysqlDb(pUr);
                            UBN u = lMstb.GetubnById(pBedrijf.UBNid);
                            bool geborenopbedrijf = false;
                            if (pBull.ThrId == u.ThrID)
                            {geborenopbedrijf=true;}
                            ANIMALCATEGORY anicatbull = lMstb.GetAnimalCategoryByIdandFarmid(pBull.AniId, pBedrijf.FarmId);
                            if (anicatbull.Anicategory < 1 || anicatbull.Anicategory > 3)
                            {
                                List<MOVEMENT> manmovs = lMstb.GetMovementsByAniId(pBull.AniId);
                                if (manmovs.Count() > 0)
                                {
                                    var ordered = from n in manmovs
                                                  orderby n.MovDate
                                                  select n;
                                    bool aanwezig = geborenopbedrijf;
                                    foreach (MOVEMENT m in ordered.ToList())
                                    { 
                                    
                                    }
                                }
                                else
                                {
                                   if(!geborenopbedrijf)
                                   {
                                    //misschien geboren op bedrijf??
                                  
                                        antwoord = "X " + pBull.AniLifeNumber + " is niet aanwezig geweest.";
                                    }
                                }                               
                            }
                    
                        }
                        else
                        {
                            antwoord = "X Einddatum niet correct";
                        }
                    }
                    else 
                    {
                        antwoord = "X Begindatum niet correct";
                    }
                }

            
     
            return antwoord;
        }

        public static string checkSpeendatum(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, DateTime pSpeendatum)
        {
            /*
                - Dier moet aanwezig zijn op speendatum
                - Dier moet vrouwelijk zijn.
                - Dier is nog niet gespeend
                - Dier moet gelamd hebben
                - Als speendatum meer dan 90 dagen na de lamdatum ligt dan de gebruiker vragen of dit klopt 
              
                  antwoord  startswith X then continue not allowed
             */
            string antwoord = "";
            if (pAnimal.AniSex == 2)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                if (lMstb.rdDierAanwezig(pAnimal.AniId, pBedrijf.FarmId,pBedrijf.UBNid, pSpeendatum, 0, 0, pBedrijf.ProgId))
                {
                    List<EVENT> events = lMstb.getEventsByAniId(pAnimal.AniId);
                    var geworpen = from n in events
                                   where n.EveKind == 5
                                   && n.EveDate.Date <= pSpeendatum.Date
                                   orderby n.EveDate descending
                                   select n;
                    DateTime MaxWorpDate = DateTime.MinValue;
                    if (geworpen.Count() == 0)
                    {
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            antwoord = "X Dier heeft niet gelamd";
                        }
                        else
                        {
                            antwoord = "X Dier heeft niet geworpen";
                        }
                    }
                    else
                    {
                        MaxWorpDate = geworpen.ElementAt(0).EveDate;


                        var algespeend = from n in events
                                         where (n.EveKind == 5 || n.EveKind == 10)
                                         && n.EveDate.Date > MaxWorpDate.Date
                                         orderby n.EveDate ascending
                                         select n;
                        if (algespeend.Count() > 0)
                        {
                            // if first ==10 then al gespeend
                            if (algespeend.ElementAt(0).EveKind == 10)
                            {
                                antwoord = "X Dier is al gespeend.";
                            }
                        }


                        if (antwoord == "")
                        {
                            TimeSpan ts = pSpeendatum.Subtract(MaxWorpDate);
                            if (ts.TotalDays > 90)
                            {
                                antwoord = "Datum ligt meer dan 90 dagen na geboortedatum van lam.";
                            }
                        }
                    }
                }
                else
                {
                    antwoord = "X Dier niet aanwezig op " + pSpeendatum.ToString("dd-MM-yyyy");
                }
            }
            else
            {
                if (pAnimal.AniSex == 1)
                {
                    antwoord = "X Dier is mannelijk.";
                }
                else
                {
                    antwoord = "X Geslacht is onbekend.";
                }
            }
            return antwoord;
        }

        public static string checkAanvoerHerkomst(int pFarmId, UserRightsToken pUr, MOVEMENT pMovement, BUYING pBuying)
        {
            if (pBuying.PurKind != 1)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);

                bool herkomstVerplicht = false;

                BEDRIJF eigenBedrijf = new BEDRIJF();
                UBN eigenubn = new UBN();
                THIRD eigenthird = new THIRD();
                COUNTRY eigenland = new COUNTRY();
                lMstb.getCompanyByFarmId(pFarmId, out eigenBedrijf, out eigenubn, out eigenthird, out eigenland);

                if (eigenBedrijf.ProgId == 5 || eigenBedrijf.ProgId == 3)
                {
                    herkomstVerplicht = true;
                }


                if (eigenland.LandAfk2.ToLower() == "nl")
                {
                    if (herkomstVerplicht)
                    {

                        if (pMovement.MovThird_UBNid == 0) { return "Herkomst UBN is verplicht"; }

                    }
                }
            }
            return "";
        }
  
        public static string checkAfvoerDestination(UserRightsToken pUr, sDestination psDestination)
        {
            if (psDestination.ThirdId > 0 || psDestination.UbnId > 0)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                List<string> rendacnrs = MovFunc.RendacUbns(pUr);
                if (psDestination.UbnId > 0)
                {
                    UBN urendac = lMstb.GetubnById(psDestination.UbnId);
                    if (rendacnrs.Contains(urendac.Bedrijfsnummer))
                    {
                        return "Voor een afvoer naar Rendac dient u een Doodmelding in te voeren.";
                    }
                }
                else if (psDestination.ThirdId > 0)
                {
                    List<UBN> rens = lMstb.getUBNsByThirdID(psDestination.ThirdId);
                    foreach (UBN u in rens)
                    {
                        if (rendacnrs.Contains(u.Bedrijfsnummer))
                        {
                            return "Voor een afvoer naar Rendac dient u een Doodmelding in te voeren.";
                        }
                    }
                    THIRD tCheck = lMstb.GetThirdByThirId(psDestination.ThirdId);
                    if (tCheck.ThrCompanyName.ToLower().Contains("rendac")) { return "Voor een afvoer naar Rendac dient u een Doodmelding in te voeren."; }
                    if (tCheck.ThrSecondName.ToLower().Contains("rendac")) { return "Voor een afvoer naar Rendac dient u een Doodmelding in te voeren."; }
                }
            }
            return "";
        }
  
        public static string AllownewDood(UserRightsToken pUr, ANIMAL pAnimal, DateTime pDeceasedDate, string pGebeurtenis,string pVerplaatsing)
        {
            string ret = "";
            if (pAnimal.AniBirthDate.Date.CompareTo(pDeceasedDate.Date) > 0)
            {
                ret = "Datum ligt voor de geboortedatum";
            }

            if (ret == "")
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                List<MOVEMENT> maxmovdates = lMstb.GetMovementsByAniId(pAnimal.AniId);
                var last = from n in maxmovdates
                           where n.MovDate.Date > pDeceasedDate.Date
                           orderby n.MovDate descending
                           select n;
                if (last.Count() > 0)
                {

                    ret = "Er staan onder " + pVerplaatsing + " na " + pDeceasedDate.ToString("dd-MM-yyyy") + " nog gegevens, verwijder deze eerst.";
                }
                if (ret == "")
                {
                    List<EVENT> maxevdates = lMstb.getEventsByAniId(pAnimal.AniId);
                    var lastevents = from n in maxevdates
                               where n.EveDate.Date > pDeceasedDate.Date
                               orderby n.EveDate descending
                               select n;
                    if (lastevents.Count() > 0)
                    {

                        ret = "Er staan onder " + pGebeurtenis + " na " + pDeceasedDate.ToString("dd-MM-yyyy") + " nog gegevens, verwijder deze eerst.";
                    }
                }
            }
            return ret;
        }

        public static string AllowDoodUpdate(UserRightsToken pUr, MOVEMENT pMov, ANIMAL pAnimal, DateTime pNewDeceasedDate, string pGebeurtenis, string pVerplaatsing)
        {
            string ret = "";
            if (pAnimal.AniBirthDate.Date.CompareTo(pNewDeceasedDate.Date) > 0)
            {
                ret = "Datum ligt voor de geboortedatum";
            }
            if (ret == "")
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                List<MOVEMENT> maxmovdates = lMstb.GetMovementsByAniId(pAnimal.AniId);
                var last = from n in maxmovdates
                           where n.MovDate.Date > pNewDeceasedDate.Date
                           && n.MovId != pMov.MovId
                           orderby n.MovDate descending
                           select n;
                if (last.Count() > 0)
                {

                    ret = "Er staan onder " + pVerplaatsing + " na " + pNewDeceasedDate.ToString("dd-MM-yyyy") + " nog gegevens, verwijder deze eerst.";
                }
                if (ret == "")
                {
                    List<EVENT> maxevdates = lMstb.getEventsByAniId(pAnimal.AniId);
                    var lastevents = from n in maxevdates
                                     where n.EveDate.Date > pNewDeceasedDate.Date
                                     orderby n.EveDate descending
                                     select n;
                    if (lastevents.Count() > 0)
                    {

                        ret = "Er staan onder " + pGebeurtenis + " na " + pNewDeceasedDate.ToString("dd-MM-yyyy") + " nog gegevens, verwijder deze eerst.";
                    }
                }
            }
            return ret;
        }

        public static bool AllowScrapieWijziging(UserRightsToken pUr, ANIMAL pAnimal, DateTime pDatum)
        {
            //BUG 1653
            bool ret = true;
            
            return ret;
        }

        public static string checkNFSFatherLetter(string pAniLifenumber, string pAniFatherLifenumber)
        {
            string ret = "";

            string L1 =  Event_functions.extractNFSFatherLetter(pAniLifenumber);
            string L2 = Event_functions.extractNFSFatherLetter(pAniFatherLifenumber);
            if (L1 != L2)
            {
                ret = "De stamboomletter is niet gelijk aan die van de vader.";
            }
            return ret;
        }

        public static bool isExistingLevensnummer(UserRightsToken pUr, string pUniekLevensnummer, int pProgId,out int pAniIdLevnrMut)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMAL tbanicheck = new ANIMAL();
            pAniIdLevnrMut = 0;
            tbanicheck = lMstb.GetAnimalByAniAlternateNumber(pUniekLevensnummer);//.GetDataBase().QueryData(ur.getLastChildConnection(), new StringBuilder("SELECT AniLifeNumber FROM ANIMAL WHERE AniAlternateNumber = '" + levensnummer + "' AND AniId>0 "));

            if (tbanicheck.AniId > 0)
            {
                List<LEVNRMUT> oldnumbers = lMstb.getLevNrMuts(tbanicheck.AniId);
                var controle = from n in oldnumbers where n.LevnrOud == pUniekLevensnummer select n;
                if (controle.Count() > 0)
                {
                    tbanicheck = new ANIMAL();
                    pAniIdLevnrMut = controle.ElementAt(0).Aniid;
                }
            }
            if (tbanicheck.AniId > 0)
            { return true; }
            else { return false; }
        }

        public static string controleerMestnummerInsert(UserRightsToken pUr, string newMestnummer, List<UBN> pUbns)
        {
            string ret = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            MESTNR bestaande = lMstb.getMestnr(newMestnummer);
            if (bestaande.Mestnummer != "")
            {
                List<string> Bedrijfnummers = (from n in pUbns select n.Bedrijfsnummer).ToList();
                ret = "Dit mestnummer is al in gebruik. ";
                List<MESTUBN> mestubns = lMstb.getMestUbns(newMestnummer);
                if (mestubns.Count() > 0)
                {
                    List<MESTUBN> zelfde = (from n in mestubns where Bedrijfnummers.Contains(n.FarmNumber) select n).ToList();
                    if (zelfde.Count() > 0)
                    {
                        ret = "Dit mestnummer is al door u in gebruik.";
                    }
                    else 
                    {
                        ret = "Dit mestnummer is al in gebruik bij een ander UBN.";
                        foreach (string negbedrijfsnr in Bedrijfnummers)
                        {
                            var negatieven = from n in mestubns where n.FarmNumber == ("-" + negbedrijfsnr) select n;
                            if (negatieven.Count() > 0)
                            {
                                ret = "";
                            }
                        }
                        
                    }
                    
                }
                
            }
            return ret;
        }

        public static string controleerMestnummerUpdate(UserRightsToken pUr, string oldMestnummer, string newMestnummer, List<UBN> pUbns)
        {
            string ret = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            DataTable tblcheck = lMstb.getTablesnamesFilledByMestnummer(oldMestnummer);
            if (tblcheck.Rows.Count > 0)
            {
                if (tblcheck.Rows.Count > 1)
                {
                    ret = "Er zijn " + tblcheck.Rows.Count.ToString() + " gegevens opgeslagen onder het Mestnummer" + oldMestnummer + "<br />Verwijder deze gegevens eerst, voordat u het Mestnummer veranderd in " + newMestnummer;
                }
                else 
                {
                    ret = "Er is nog " + tblcheck.Rows.Count.ToString() + " gegeven opgeslagen onder het Mestnummer" + oldMestnummer + "<br />Verwijder deze eerst, voordat u het Mestnummer veranderd in " + newMestnummer;
                
                }
            }
            return ret;
        }

        public static string checkDeleteAnimal(UserRightsToken pUr, BEDRIJF pBedrijf,UBN pUbn, THIRD pThird,COUNTRY pCountry, ANIMAL pAnimal)
        {
     
            string ret = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            DataTable tblcheck = lMstb.getAnimalActivities(pAnimal.AniId);
            if (tblcheck.Rows.Count > 0)
            {
                foreach (DataRow rw in tblcheck.Rows)
                {
                    ANIMALCATEGORY ac = new ANIMALCATEGORY();
                    lMstb.GetDataBase().FillObject(ac, rw);
                    if (ac.FarmId != pBedrijf.FarmId && ac.Ani_Mede_Eigenaar != 1)
                    {
                        return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + pAnimal.AniLifeNumber + " is ook geregistreerd bij een ander bedrijf";
                    }
                    else
                    {
                        if (ac.Ani_Mede_Eigenaar == 1)
                        {
                            return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />U bent alleen medeeigenaar";
                        }
                        else if (ac.Anicategory == 4)
                        {
                            return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + pAnimal.AniLifeNumber + " is niet aanwezig";
                        }
                        else if (ac.Anicategory == 5)
                        {
                            return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + pAnimal.AniLifeNumber + " is nooit aanwezig geweest";
                        }
                        else
                        {
                            if (rw["MovId"] != DBNull.Value)
                            {
                                List<AGRO_LABELS> movs = lMstb.GetAgroLabels(1, 0, pBedrijf.Programid, pBedrijf.ProgId);
                                int lmovk = 0;
                                int.TryParse(rw["MovKind"].ToString(), out lmovk);
                                string retvar = "verplaatsing";
                                var s = from n in movs where n.LabID == lmovk select n;
                                if (s.Count() > 0)
                                { retvar = s.ElementAt(0).LabLabel; }
                                return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + pAnimal.AniLifeNumber + " heeft nog een " + retvar;
                            }
                            else if (rw["EventId"] != DBNull.Value)
                            {
                                List<AGRO_LABELS> evs = lMstb.GetAgroLabels(2, 0, pBedrijf.Programid, pBedrijf.ProgId);
                                int lmovk = 0;
                                int.TryParse(rw["EveKind"].ToString(), out lmovk);
                                string retvar = "actie";
                                var s = from n in evs where n.LabID == lmovk select n;
                                if (s.Count() > 0)
                                { retvar = s.ElementAt(0).LabLabel; }
                                return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + pAnimal.AniLifeNumber + " heeft nog een " + retvar;

                            }
                            else if (rw["MoederVan"] != DBNull.Value)
                            {
                                return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + rw["mAniLifenumber"].ToString() + " is moeder van " + pAnimal.AniLifeNumber;
                            }
                            else if (rw["VaderVan"] != DBNull.Value)
                            {
                                return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + rw["fAniLifenumber"].ToString() + " is vader van " + pAnimal.AniLifeNumber;
                            }
                            else if (rw["Report"] != DBNull.Value)
                            {
                                if (rw["Report"].ToString() != "2")
                                {
                                    return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />Er zijn voor " + pAnimal.AniLifeNumber + " meldingen gedaan.";
                                }
                            }
                            else if (rw["CodeMutation"] != DBNull.Value)
                            {

                                List<AGRO_LABELS> movs = lMstb.GetAgroLabels(102, 0, pBedrijf.Programid, pBedrijf.ProgId);
                                int lmovk = 0;
                                int.TryParse(rw["EveKind"].ToString(), out lmovk);
                                string retvar = "actie";
                                var s = from n in movs where n.LabID == lmovk select n;
                                if (s.Count() > 0)
                                { retvar = s.ElementAt(0).LabLabel; }
                                return "X U kunt " + pAnimal.AniLifeNumber + " niet verwijderen<br />" + pAnimal.AniLifeNumber + " heeft nog een " + retvar + " melding klaarstaan";
                            }
                        }
                    }
                }
            }
            BIRTH b = new BIRTH();
            StringBuilder bld = new StringBuilder();
            bld.Append("SELECT EVENT.UBNId FROM BIRTH LEFT JOIN EVENT ON EVENT.EventId=BIRTH.EventId WHERE BIRTH.CalfId=" + pAnimal.AniId.ToString() + " AND BIRTH.EventId>0 ");
            DataSet ds = new DataSet();
            DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), ds, bld, "rdDierGeborenOpBedrijf", MissingSchemaAction.Add);
            if (tbl.Rows.Count > 0)
            {
                if (tbl.Rows[0][0] != DBNull.Value)
                {
                    if (tbl.Rows[0][0].ToString() == pBedrijf.UBNid.ToString() || tbl.Rows[0][0].ToString() == "0")
                    {
                        return "X U kunt " + pAnimal.AniLifeNumber + " op deze manier niet verwijderen<br />" + pAnimal.AniLifeNumber + " is op dit bedrijf geboren<br />verwijder de worp";
                    }
                }
            }
            if (pUbn.ThrID == pAnimal.ThrId)
            { return "X U kunt " + pAnimal.AniLifeNumber + " op deze manier niet verwijderen<br />" + pAnimal.AniLifeNumber + " is op dit bedrijf geboren<br />verwijder de worp"; }
            else if (lMstb.getUBNsByThirdID(pAnimal.ThrId).Count == 0)
            {
                //BUG 1983 fokker stond 3 X in de Third tabel en dier.ThrId!= UBN.ThrId
                // maar is het wel dezelfde fokker
                //Dus
                if (pThird.ThrZipCode != "" && pThird.ThrExt != "")
                {
                    List<THIRD> lZelfde = lMstb.GetThirdsByHouseNrAndZipCode(pThird.ThrExt, pThird.ThrZipCode);
                    var zelfde = from n in lZelfde
                                 where n.ThrId == pAnimal.ThrId
                                 select n;
                    if (zelfde.Count() > 0)
                    {
                        return "X U kunt " + pAnimal.AniLifeNumber + " op deze manier niet verwijderen<br />" + pAnimal.AniLifeNumber + " is op dit bedrijf geboren<br />verwijder de worp";
                    }
                }
                
            }
         
            if (pThird.ThrCountry == "151")
            {
                string Werknummer = "";
                DateTime Geboortedat = DateTime.MinValue; DateTime Importdat = DateTime.MinValue;
                string LandCodeHerkomst = ""; string LandCodeOorsprong = "";
                string Geslacht = ""; string Haarkleur = "";
                DateTime Einddatum = DateTime.MinValue; string RedenEinde = "";
                string LevensnrMoeder = "";
                string VervangenLevensnr = "";
                string Status = ""; string Code = ""; string Omschrijving = "";
                String lUsername, lPassword;
                FTPUSER fusoap = lMstb.GetFtpuser(pBedrijf.UBNid,pBedrijf.Programid,pBedrijf.ProgId, 9992);
                if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
                {

                    lUsername = fusoap.UserName;
                    lPassword = fusoap.Password;
                }
                else
                {
                    lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"];
                    lPassword = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]); ;
                }
                SOAPLNVDieren soapdieren = new SOAPLNVDieren();
                soapdieren.LNVDierdetailsV2(lUsername, lPassword, 0, pUbn.Bedrijfsnummer, pThird.Thr_Brs_Number, pAnimal.AniAlternateNumber, pBedrijf.ProgId, 0, 0, 0, ref Werknummer,
                    ref Geboortedat, ref Importdat, ref LandCodeHerkomst, ref LandCodeOorsprong, ref Geslacht, ref Haarkleur,
                    ref Einddatum, ref RedenEinde, ref LevensnrMoeder, ref VervangenLevensnr,
                    ref Status, ref Code, ref Omschrijving);
                if (Geboortedat != null && Geboortedat > DateTime.MinValue)
                {
                    return "X U kunt " + pAnimal.AniLifeNumber + " op deze manier niet verwijderen<br />" + pAnimal.AniLifeNumber + " is bekend bij RVO als " + pAnimal.AniAlternateNumber;
                }
            }
            else if (pThird.ThrCountry == "21" || pThird.ThrCountry == "125")
            {
                String Taal = CORE.utils.getcurrentlanguage();
                String BTWnr = pThird.ThrVATNumber;
                String Annexnr = "";
                String Inrichtingsnr = "";
                String Beslagnr = "";
                String PENnr = pUbn.Bedrijfsnummer;
                CORE.utils.getSanitraceNummers(pThird, pUbn, out Annexnr, out Inrichtingsnr, out Beslagnr, out PENnr);
                
                FTPUSER fulnvsoap = lMstb.GetFtpuser(pUbn.UBNid, 9992);

                string Gebruikersnaam = fulnvsoap.UserName;
                string Wachtwoord = fulnvsoap.Password;
                String BRSnr = pThird.Thr_Brs_Number;
                int testserver = 0;
                if (pThird.ThrCountry == "125") { testserver = 10; }
                int MaxStrLen = 255;
                String Status = new StringBuilder(MaxStrLen).ToString();
                String Omschrijving = new StringBuilder(MaxStrLen).ToString();
                Win32SANITRACEALG DLLcall = new Win32SANITRACEALG();
                string pLogfile = unLogger.getLogDir() + pUbn.Bedrijfsnummer + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";
                string pResultFile = unLogger.getLogDir() + pUbn.Bedrijfsnummer + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";
                DLLcall.STverblijfplaatsen(Gebruikersnaam, Wachtwoord, testserver, Taal, Inrichtingsnr, Beslagnr,
                    pAnimal.AniAlternateNumber, 1, pLogfile, pResultFile, ref Status, ref Omschrijving, MaxStrLen);
                if (File.Exists(pResultFile))
                {
                    StreamReader sr = new StreamReader(pResultFile);
                    bool hascontent = false;
                    try
                    {
                        string lLine = "";
                        while ((lLine = sr.ReadLine()) != null)
                        {
                            hascontent = true;
                            break;
                        }
                       
                    }
                    catch
                    {

                    }
                    finally { sr.Close(); }
                    if (hascontent)
                    {
                        return "X U kunt " + pAnimal.AniLifeNumber + " op deze manier niet verwijderen<br />" + pAnimal.AniLifeNumber + " is bekend bij Sanitrace als " + pAnimal.AniAlternateNumber;
                    }
                }
            }
            return ret;

        }

        public static string checkGeboorteGewicht(UserRightsToken pUr, BEDRIJF pBedrijf, double pGewicht)
        {
            string ret = "";
            if (utils.isNsfo(pBedrijf.Programid))
            {
                if ((pGewicht < 0 || pGewicht >= 10))
                {
                    ret = "Geboortegewicht moet tussen de 0 en 10 Kg liggen!";
                }
            }
            return ret;
        }

        public static bool showfeedadvice(UserRightsToken pUr, int pAniID, BEDRIJF pBedrijf, UBN pUbn ,out  List<MOVEMENT> pShowDates)
        {
            bool ret = true;
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMALCATEGORY ac = new ANIMALCATEGORY();
            ANIMAL a = new ANIMAL();
            lMstb.getAnimalAndCategory(pAniID, pBedrijf.FarmId, out a, out ac);
            pShowDates = new List<MOVEMENT>();

            if (ac.Ani_Mede_Eigenaar == 0 && ac.Anicategory > 4)
            {
                ret = false;
            }
            if (ac.Anicategory == 4)
            {
                pShowDates = lMstb.GetMovementsByAniId(a.AniId);
                var t = from n in pShowDates
                        where (n.MovKind < 8)
                        orderby n.MovDate, n.MovId
                        select n;
                pShowDates = t.ToList();
            }
            return ret;
        }

        public static bool isHandMatigeKeto(UserRightsToken pUr, int pAniID,DateTime pVanAfDatum, int pAantalDagen)
        {
            bool ret = true;
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<EVENT> worpen = (from n in lMstb.getEventsByAniIdKind(pAniID, 5) orderby n.EveDate descending select n).ToList();
            if(worpen.Count()>0)
            {
                if (worpen.ElementAt(0).EveDate.AddDays(pAantalDagen) >= pVanAfDatum)
                {
                    ret = false;
                }
            }

            return ret;
        }
    }

    public class VSM_Ruma_OnlineCulture
    {
        /// <summary>
        /// Om vertalingen uit de globalresource bestanden te krijgen, zonder dit via rumaonline te moeten doen
        /// </summary>
        /// <param name="pName"> Naam van de resource </param>
        /// <param name="pNederlandsetekst"> Default nederlandse tekst, wanneer de resource niet bestaat, of niet bekend is</param>
        /// <returns></returns>
        public static string getStaticResource(string pName, string pNederlandsetekst)
        {
            string ret = "";
            if (!String.IsNullOrEmpty(pName))
            {
                try
                {
                    string land = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();

                    string path = unLogger.getLogDir().Replace("log", "App_GlobalResources") + "GlobalResource.resx";
                    if (land != "nl")
                    {
                        path = unLogger.getLogDir().Replace("log", "App_GlobalResources") + "GlobalResource." + land + ".resx";
                    }
                    if (File.Exists(path))
                    {

                        XDocument xd = XDocument.Load(path);
                        /*EXAMPLE : aantal op z'n Deens land='da'
                        * <root> 
                          <data name="aantal" xml:space="preserve">
                             <value>Antal</value>
                           </data>
                        * </root>
                        */
                        XElement xEleMain = xd.Root.XPathSelectElement("data[@name='" + pName + "']");
                        if (xEleMain != null)
                        {
                            ret = xEleMain.Element("value").Value;
                        }
                        else
                        {
                            if (land != "nl")
                            {
                                path = unLogger.getLogDir().Replace("log", "App_GlobalResources") + "GlobalResource.resx";
                                if (File.Exists(path))
                                {
                                    xd = XDocument.Load(path);
                                    xEleMain = xd.Root.XPathSelectElement("data[@name='" + pName + "']");
                                    if (xEleMain != null)
                                    {
                                        ret = xEleMain.Element("value").Value;
                                    }
                                }
                            }
                        }
                       
                    }
                }
                catch { }
            }
            if (ret == "")
            { return pNederlandsetekst; }
            return ret;
        }

        //public string getResource(string pName)
        //{

        //}
    }
}
