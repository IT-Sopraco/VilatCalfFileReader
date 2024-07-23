using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using VSM.RUMA.CORE.DB.DataTypes;

 
using System.Text;

namespace VSM.RUMA.CORE
{

    /// <summary>
    /// Summary description for HIClient
    /// </summary>
 
    public class HIClient
    {
        // The port number for the remote device.
        private const int port = 2222;
        private const int testport = 2223;
        private int poort { get; set; }
   
        private string TemplogDir { get; set; }
        private string OutLogDir { get; set; }

        public event EventHandler RequestReady;

        protected void OnRequestReady(object sender, MovFuncEvent e)
        {
            if (RequestReady != null)
                RequestReady(sender, e);
        }

        public HIClient(int pTestserver,string pTemplogDir,string pOutLogDir)
        {
            // Connect to a remote device.
            try
            {
                poort = port;
                if (pTestserver > 0)
                { poort = testport; }
              
                if(!pTemplogDir.EndsWith(@"\"))
                {
                    pTemplogDir = pTemplogDir + @"\";
                }
                if (!pOutLogDir.EndsWith(@"\"))
                {
                    pOutLogDir = pOutLogDir + @"\";
                }
                TemplogDir = pTemplogDir;
                OutLogDir = pOutLogDir;

            }
            catch (Exception e)
            {
             
                unLogger.WriteError(e.ToString());
            }
        }

        private string getUSERID(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,out string Pin,out string mitbenutzer)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            FTPUSER fusoap = DB.GetFtpuser(pUBNid, pProgramid, pProgId, 9997);
            string USERID = fusoap.UserName;
            Pin = fusoap.Password;
            mitbenutzer = DB.getUbnProperty(pUBNid, "mitbenutzer");
            //"051540281004";
            //string Pin = "513620";
            if (poort == 2223)
            {
                USERID = "276090000000015";// fusoap.UserName;
                Pin = "900015";// fusoap.Password;
            }
            return USERID;
        }

        /// <summary>
        /// Deze eerst aanroepen dan worden de stallijst de geboortes de aanvoeren en de afvoeren opgehaald
        /// </summary>
        /// <param name="mToken"></param>
        /// <param name="pUBNid"></param>
        /// <param name="pProgId"></param>
        /// <param name="pProgramid"></param>
        /// <param name="pBegindatum"></param>
        /// <param name="pEinddatum"></param>
        /// <returns></returns>
        private SOAPLOG HTierRaadplegenAlgemeen(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                                   DateTime pBegindatum, DateTime pEinddatum)
        {
            SOAPLOG dReturn = new SOAPLOG();
            dReturn.Kind = 1400;
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            string HitBatchBASEDIR = DB.GetProgramConfigValue(pProgramid, "HitBatchBASEDIR");
            if (HitBatchBASEDIR == "")
            {
                HitBatchBASEDIR = @"\projekt\HIT";
            }
            UBN lUBN = DB.GetubnById(pUBNid);
          
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();

            string Pin = "513620";// fusoap.Password;
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out Pin, out mitbenutzer); // fusoap.UserName;

            if (USERID != "")
            {

                string lTijd = DateTime.Now.ToString("yyyyMMdd");
                string inifile = CreateIniFile(USERID, Pin, mitbenutzer, HitBatchBASEDIR);
                string INFILE = CreateINFILES(USERID, pBegindatum, pEinddatum);

                if (inifile != "" && INFILE != "")
                {
                    String Output = unLogger.getLogDir("CSVData") + lUBN.Bedrijfsnummer + "_HTierRaadplegenMeldingen_" + lTijd + ".csv";
                    String LogDir = unLogger.getLogDir("IenR") + lUBN.Bedrijfsnummer + "_HTierRaadplegenMeldingen_" + lTijd + ".log";

                    dReturn = doHitBatchcmd(HitBatchBASEDIR, inifile, USERID, lUBN.Bedrijfsnummer);
                    dReturn.Code = "RaadplegenMeldingen";
                 
                }
                else
                {
                    dReturn.Code = "RaadplegenMeldingen";
                    dReturn.Date = DateTime.Now.Date;
                    dReturn.FarmNumber = lUBN.Bedrijfsnummer;
                    dReturn.Internalnr = 0;
                    dReturn.Kind = 1400;
                    dReturn.Lifenumber = "";
                    dReturn.Omschrijving = "Error creating HTier files";
                    dReturn.Status = "F";
                    dReturn.Time = DateTime.Now;
                }
            }
            else
            {
                dReturn.Code = "RaadplegenMeldingen";
                dReturn.Date = DateTime.Now.Date;
                dReturn.FarmNumber = lUBN.Bedrijfsnummer;
                dReturn.Internalnr = 0;
                dReturn.Kind = 1400;
                dReturn.Lifenumber = "";
                dReturn.Omschrijving = "Geen inlogcodes gevonden";
                dReturn.Status = "F";
                dReturn.Time = DateTime.Now;
            }
            return dReturn;
        }


        public SOAPLOG HtierRaadplegenmeldingen(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                                   DateTime pBegindatum, DateTime pEinddatum, string pOutputfileCsv)
        {
            string pin = "";
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            if (pBegindatum.Date == pEinddatum.Date && pEinddatum > DateTime.MinValue.Date)
            {
                pBegindatum = pEinddatum.AddDays(-3);
            }
          
            SOAPLOG dReturn = HTierRaadplegenAlgemeen(mToken, pUBNid, pProgId, pProgramid, pBegindatum, pEinddatum);
            if (dReturn.Status == "G" || dReturn.Omschrijving == "Data found")
            {

                string[] KolsBETRH = { "#BESTREG", "BNR15", "BNR15_X", "DAT_VON", "DAT_BIS", "LOM", "LOM_X", "GEB_DATR", "RASSE", "RASSE_X", "GESCHL_R", "GESCHL_X", "LOM_MUT", "LOM_MUTX", "LOM_A", "LAND_URS", "LAND_URX", "TIER_EIN", "TIER_EINX", "DAT_EIN", "BNR15_VB", "BNR15_VBX", "TIER_AUS", "TIER_AUSX", "DAT_AUS", "BNR15_NB", "BNR15_NBX", "GVE", "EKALBLOM", "EKALBLOMX", "EKALBDAT", "LKALBDAT", "KALBANZ", "KALBUNGANZ", "TIER_END", "TIER_ENDX", "DAT_END", "BNR15_EB", "BNR15_EBX", "GVE_L8VJA", "GVE_L8HEU", "SCHL_KAT", "SCHL_KATX", "SCHL_NR", "SCHL_GEW", "SCHL_LEB", "BNR15_AB", "BNR15_ABX", "BNR15_GEB", "BNR15_GEX", "GVE_ERROR", "BNR15_VX", "BNR15_VXX", "BEW_DAT", "BEW_ART", "BEW_MELD", "PLAUSINR", "SCHWERE", "FEHLERTEXT", "SCHWEREX", "BNR15_LMON", "BNR15_LMOX", "LAND_ZIE", "LAND_ZIX", "BEW_ART_TD", "DATUM", "DAT_LMON", "ZKZ_DURCH", "ZKZ_DURCHT", "BNR15_BT", "BNR15_BT_X", "BREG_TAMKM", "BREG_TAMKW", "BREG_TAMRM", "BREG_TAMRW", "RD_INUTZ", "RD_STALLNR", "RD_TEILNR", "RD_TEILBEZ", "RD_WERTG", "RD_WERTD", "RD_BEMERK", "RD_DATUM", "RD_BNR15", "RD_BNR15UN" };
                DataTable tblBETRH = utils.GetCsvData(KolsBETRH, TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT", ';', "DeutschBETRH");
                if (tblBETRH.Rows.Count > 1)
                {
                    tblBETRH.Rows.Remove(tblBETRH.Rows[0]);
                }
                string[] KolsGEBURT = { "LOM", "BNR15", "GEB_DATR", "RASSE", "GESCHL_R", "GEB_VERL", "VERBLEIB", "MEHRLING", "MEHRLADR", "ET_KALB", "KENZ_ART", "MELD_DAT", "LOM_MUT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST", "TIERNAMENR", "TIERNAME", "GEB_FEST", "GEB_NABBR", "GEB_NAGEBV" };
                DataTable tblGEBURT = utils.GetCsvData(KolsGEBURT, TemplogDir + lTijd + "_" + USERID + "_GEBURT.OUT", ';', "Deutsch");
                if (tblGEBURT.Rows.Count > 1)
                {
                    tblGEBURT.Rows.Remove(tblGEBURT.Rows[0]);
                }
                string[] KolsABGANG = { "LOM", "BNR15", "ABGA_DAT", "MELD_DAT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST" };
                DataTable tblABGANG = utils.GetCsvData(KolsABGANG, TemplogDir + lTijd + "_" + USERID + "_ABGANG.OUT", ';', "Deutsch");
                if (tblABGANG.Rows.Count > 1)
                {
                    tblABGANG.Rows.Remove(tblABGANG.Rows[0]);
                }
                string[] KolsZUGANG = { "LOM", "BNR15", "ZUGA_DAT", "MELD_DAT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST" };
                DataTable tblZUGANG = utils.GetCsvData(KolsZUGANG, TemplogDir + lTijd + "_" + USERID + "_ZUGANG.OUT", ';', "Deutsch");
                if (tblZUGANG.Rows.Count > 1)
                {
                    tblZUGANG.Rows.Remove(tblZUGANG.Rows[0]);
                }
                //TOD ??
                StreamWriter wr = new StreamWriter(pOutputfileCsv, false);
                try
                {
                    string[] Kols = { "LOM","Levensnummer", "Meldingtype", "Datum", "ubn2ePartij", "meldingsnummer", "meldingstatus", "hersteld" };
                    string MeldingStatus = "1"; //??? geen idee, maar niet op 2 zetten
                    string hersteld = "0";
                    //ivm BUG: niet alle Zugang staat in tblZUGANG
                    //extra controle via stallijst
                    string Meldingtype = "1";
                    StringBuilder bld = new StringBuilder();
                    foreach (DataRow rwBE in tblBETRH.Rows)
                    {
                        if (rwBE["DAT_EIN"] != DBNull.Value && rwBE["DAT_EIN"].ToString() != "%--" && rwBE["DAT_EIN"].ToString() != "")
                        {
                            if (rwBE["DAT_AUS"] == DBNull.Value || rwBE["DAT_AUS"].ToString() == "" || rwBE["DAT_AUS"].ToString() == "%--")
                            {
                                DateTime betein = new DateTime();
                                DateTime.TryParse(rwBE["DAT_EIN"].ToString(), out betein);
                                bld.AppendLine(rwBE["LOM"].ToString() + ";" + getLifeNumberFromLOM_X(rwBE["LOM_X"].ToString()) + ";" + Meldingtype + ";" + betein.ToString("yyyyMMdd") + ";" + ";" + ";" + MeldingStatus + ";" + hersteld + ";");

                            }
                        }
                    }

                    Meldingtype = "1";
                    /* NL 446572774;4;20140924;;150616249;1;0;
                     */
                    foreach (DataRow rwZU in tblZUGANG.Rows)
                    {
                        if (rwZU["ZUGA_DAT"] != DBNull.Value && rwZU["ZUGA_DAT"].ToString() != "")
                        {
                            DateTime toe = new DateTime();
                            DateTime.TryParse(rwZU["ZUGA_DAT"].ToString(), out toe);
                            DateTime melddate = pEinddatum.Date;
                            try 
                            {
                                DateTime.TryParse(rwZU["MELD_DAT"].ToString(), out melddate);
                            }
                            catch { }
                            if (melddate.Date >= pBegindatum.Date && melddate.Date <= pEinddatum.Date)
                            {
                               DataRow[] dier = tblBETRH.Select("LOM='" + rwZU["LOM"].ToString() + "'");
                               if (dier.Count() > 0)
                               {
                                   wr.WriteLine(rwZU["LOM"].ToString() + ";" + getLifeNumberFromLOM_X(dier[0]["LOM_X"].ToString()) + ";" + Meldingtype + ";" + toe.ToString("yyyyMMdd") + ";" + ";" + ";" + MeldingStatus + ";" + hersteld + ";");
                               }
                            }
                        }
                    }
                    //Meldingtype 2
                    Meldingtype ="2";
                    foreach (DataRow rwAB in tblABGANG.Rows)
                    {
                        if (rwAB["ABGA_DAT"] != DBNull.Value && rwAB["ABGA_DAT"].ToString() != "")
                        {
                            DateTime afg = new DateTime(); 
                            DateTime.TryParse(rwAB["ABGA_DAT"].ToString(), out afg);
                            DateTime melddate = pEinddatum.Date;
                            try
                            {
                                DateTime.TryParse(rwAB["MELD_DAT"].ToString(), out melddate);
                            }
                            catch { }
                            if (melddate.Date >= pBegindatum.Date && melddate.Date <= pEinddatum.Date)
                            {
                                DataRow[] dier = tblBETRH.Select("LOM='" + rwAB["LOM"].ToString() + "'");
                                if (dier.Count() > 0)
                                {
                                    wr.WriteLine(rwAB["LOM"].ToString() + ";" + getLifeNumberFromLOM_X(dier[0]["LOM_X"].ToString()) + ";" + Meldingtype + ";" + afg.ToString("yyyyMMdd") + ";" + ";" + ";" + MeldingStatus + ";" + hersteld + ";");
                                }
                            }
                        }
                    }
                    //Meldingtype 3
                    Meldingtype ="3";
                    foreach (DataRow rwGEB in tblGEBURT.Rows)
                    {
                        if (rwGEB["GEB_DATR"] != DBNull.Value && rwGEB["GEB_DATR"].ToString() != "")
                        {
                            DateTime geb = new DateTime();
                            DateTime.TryParse(rwGEB["GEB_DATR"].ToString(), out geb);
                            DateTime melddate = pEinddatum.Date;
                            try
                            {
                                DateTime.TryParse(rwGEB["MELD_DAT"].ToString(), out melddate);
                            }
                            catch { }
                            if (melddate.Date >= pBegindatum.Date && melddate.Date <= pEinddatum.Date)
                            {
                                DataRow[] dier = tblBETRH.Select("LOM='" + rwGEB["LOM"].ToString() + "'");
                                if (dier.Count() > 0)
                                {
                                    wr.WriteLine(rwGEB["LOM"].ToString() + ";" + getLifeNumberFromLOM_X(dier[0]["LOM_X"].ToString()) + ";" + Meldingtype + ";" + geb.ToString("yyyyMMdd") + ";" + ";" + ";" + MeldingStatus + ";" + hersteld + ";" + dier[0]["LOM_MUT"].ToString());
                                }
                            }
                        }
                    }

                }
                catch (Exception exc)
                {
                    dReturn.Code = "";
                    dReturn.Date = DateTime.Now.Date;
                    dReturn.FarmNumber = "";
                    dReturn.Internalnr = 0;
                    dReturn.Kind = 1400;
                    dReturn.Lifenumber = "";
                    dReturn.Omschrijving = "Error Hitbatch read csv data ";
                    dReturn.Status = "F";
                    dReturn.Time = DateTime.Now;
                    unLogger.WriteError(exc.ToString());
                }
                finally { wr.Flush(); wr.Close(); }
            }
            return dReturn;
        }

        public ANIMAL fillAnimalFromHtierTbl(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid, string pHiTierLOM_nummer, string pHiTierLOM_X_nummer, int pChangedBy, int pSourceID)
        {
            return fillAnimalFromHtierTbl(mToken, pUBNid, pProgId, pProgramid, pHiTierLOM_nummer, pHiTierLOM_X_nummer, 0, pChangedBy, pSourceID);
        }

        public ANIMAL fillAnimalFromHtierTbl(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid, string pHiTierLOM_nummer, string pHiTierLOM_X_nummer, int pLevel, int pChangedBy, int pSourceID)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            UBN ub = DB.GetubnById(pUBNid);
            ANIMAL a = new ANIMAL();
            if (pHiTierLOM_nummer != null && pHiTierLOM_nummer.Length > 8)
            {
                a = DB.GetAnimalByAniAlternateNumber(pHiTierLOM_nummer);
                if (a.AniId <= 0)
                {
                    if (pHiTierLOM_X_nummer != null && pHiTierLOM_X_nummer.Length > 8)
                    {
                        a = DB.GetAnimalByAniAlternateNumber(pHiTierLOM_X_nummer);
                    }
                }
                string pin = "";
                string mitbenutzer = "";
                string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
                string lTijd = DateTime.Now.ToString("yyyyMMdd");

                if (!File.Exists(TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT"))
                {
                    HTierRaadplegenAlgemeen(mToken, pUBNid, pProgId, pProgramid, DateTime.Now.AddDays(-10), DateTime.Now);
                }
                if (File.Exists(TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT"))
                {
                   
                    string[] KolsBETRH = { "#BESTREG", "BNR15", "BNR15_X", "DAT_VON", "DAT_BIS", "LOM", "LOM_X", "GEB_DATR", "RASSE", "RASSE_X", "GESCHL_R", "GESCHL_X", "LOM_MUT", "LOM_MUTX", "LOM_A", "LAND_URS", "LAND_URX", "TIER_EIN", "TIER_EINX", "DAT_EIN", "BNR15_VB", "BNR15_VBX", "TIER_AUS", "TIER_AUSX", "DAT_AUS", "BNR15_NB", "BNR15_NBX", "GVE", "EKALBLOM", "EKALBLOMX", "EKALBDAT", "LKALBDAT", "KALBANZ", "KALBUNGANZ", "TIER_END", "TIER_ENDX", "DAT_END", "BNR15_EB", "BNR15_EBX", "GVE_L8VJA", "GVE_L8HEU", "SCHL_KAT", "SCHL_KATX", "SCHL_NR", "SCHL_GEW", "SCHL_LEB", "BNR15_AB", "BNR15_ABX", "BNR15_GEB", "BNR15_GEX", "GVE_ERROR", "BNR15_VX", "BNR15_VXX", "BEW_DAT", "BEW_ART", "BEW_MELD", "PLAUSINR", "SCHWERE", "FEHLERTEXT", "SCHWEREX", "BNR15_LMON", "BNR15_LMOX", "LAND_ZIE", "LAND_ZIX", "BEW_ART_TD", "DATUM", "DAT_LMON", "ZKZ_DURCH", "ZKZ_DURCHT", "BNR15_BT", "BNR15_BT_X", "BREG_TAMKM", "BREG_TAMKW", "BREG_TAMRM", "BREG_TAMRW", "RD_INUTZ", "RD_STALLNR", "RD_TEILNR", "RD_TEILBEZ", "RD_WERTG", "RD_WERTD", "RD_BEMERK", "RD_DATUM", "RD_BNR15", "RD_BNR15UN" };
                    DataTable tblBETRH = utils.GetCsvData(KolsBETRH, TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT", ';', "DeutschBETRH");
                    DataRow[] foundRows = tblBETRH.Select("LOM='" + pHiTierLOM_nummer + "'");
                    if (foundRows.Count() > 0)
                    {
                        if (a.AniId <= 0)
                        {
                            a = DB.GetAnimalByAniAlternateNumber(getLifeNumberFromLOM_X(foundRows[0]["LOM_X"].ToString()));
                        }
                        if (a.AniId <= 0)
                        {
                            a.AniAlternateNumber = foundRows[0]["LOM"].ToString();
                            a = DB.GetAnimalByAniAlternateNumber(a.AniAlternateNumber);
                        }

                        if (a.AniId <= 0)
                        {
                            a.AniAlternateNumber = getLifeNumberFromLOM_X(foundRows[0]["LOM_X"].ToString());
                            a.AniLifeNumber = getLifeNumberFromLOM_X(foundRows[0]["LOM_X"].ToString());
                        }
                        DateTime birthdate = new DateTime();
                        DateTime.TryParse(foundRows[0]["GEB_DATR"].ToString(), out birthdate);
                        if (birthdate > DateTime.MinValue)
                        {

                            a.AniBirthDate = birthdate;
                        }
                        int lAnisex = 0;
                        int.TryParse(foundRows[0]["GESCHL_R"].ToString(), out lAnisex);
                        a.AniSex = lAnisex;
                        if (foundRows[0]["LOM_MUT"] != DBNull.Value && foundRows[0]["LOM_MUT"].ToString() != "")
                        {
                            ANIMAL moeder = DB.GetAnimalByAniAlternateNumber(foundRows[0]["LOM_MUT"].ToString());
                            if (moeder.AniId <= 0)
                            {
                                if (pLevel == 0)
                                {
                                    if (foundRows[0]["LOM_MUTX"] != DBNull.Value && foundRows[0]["LOM_MUTX"].ToString() != "")
                                    {
                                        string mAnilifnumber = getLifeNumberFromLOM_X(foundRows[0]["LOM_MUTX"].ToString());
                                        moeder = DB.GetAnimalByLifenr(mAnilifnumber);
                                        if (moeder.AniId <= 0)
                                        {

                                            moeder = fillAnimalFromHtierTbl(mToken, pUBNid, pProgId, pProgramid, foundRows[0]["LOM_MUT"].ToString(), foundRows[0]["LOM_MUTX"].ToString(),1, pChangedBy, pSourceID);
                                        }
                                        if (moeder.AniId <= 0)
                                        {
                                            try
                                            {
                                                moeder = new ANIMAL();//noodgreep; want voor msoptimabox zijn de moeders belangrijk
                                                moeder.AniLifeNumber = mAnilifnumber;
                                                moeder.AniAlternateNumber = mAnilifnumber;
                                                moeder.AniSex = 2;
                                                moeder.Changed_By = pChangedBy;
                                                moeder.SourceID = pSourceID;
                                                DB.SaveAnimal(ub.ThrID, moeder);
                                            }
                                            catch (Exception exc) { unLogger.WriteError("Opslaan HiTier moeder niet gelukt " + mAnilifnumber + " " + exc.ToString()); }
                                        }
                                        else
                                        {

                                            if (moeder.AniBirthDate == DateTime.MinValue || moeder.AniSex == 0)
                                            {

                                                moeder = fillAnimalFromHtierTbl(mToken, pUBNid, pProgId, pProgramid, foundRows[0]["LOM_MUT"].ToString(), foundRows[0]["LOM_MUTX"].ToString(), 1, pChangedBy, pSourceID);

                                            } 

                                        }
                                    }
                                }

                            }
                            if (moeder.AniId > 0)
                            {
                                a.AniIdMother = moeder.AniId;
                            }
                        }
                        string[] KolsGEBURT = { "LOM", "BNR15", "GEB_DATR", "RASSE", "GESCHL_R", "GEB_VERL", "VERBLEIB", "MEHRLING", "MEHRLADR", "ET_KALB", "KENZ_ART", "MELD_DAT", "LOM_MUT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST", "TIERNAMENR", "TIERNAME", "GEB_FEST", "GEB_NABBR", "GEB_NAGEBV" };
                        DataTable tblGEBURT = utils.GetCsvData(KolsGEBURT, TemplogDir + lTijd + "_" + USERID + "_GEBURT.OUT", ';', "Deutsch");

                        DataRow[] geboorterows = tblGEBURT.Select("LOM='" + pHiTierLOM_nummer + "'");
                        if (geboorterows.Count() > 0)
                        {

                            int meerling = 0;
                            int.TryParse(geboorterows[0]["MEHRLADR"].ToString(), out meerling);
                            a.AniNling = meerling;
                            a.ThrId = ub.ThrID;
                        }
                        a.Changed_By = pChangedBy;
                        a.SourceID = pSourceID;
                        DB.SaveAnimal(ub.ThrID, a);
                        
                    }
                  
                }
            }
            return a;
        }

        private SOAPLOG doHitBatchcmd(string HitBatchBASEDIR, string pInifile,string pUSERID, string pBedrijfsnummer)
        {
            SOAPLOG dReturn = new SOAPLOG();
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            if (File.Exists("C:" + HitBatchBASEDIR + @"\HitBatch\HitBatch.cmd"))
            {
                try
                {
                    var fileContents = System.IO.File.ReadAllText("C:" + HitBatchBASEDIR + @"\HitBatch\HitBatch.cmd");


                    fileContents = fileContents.Replace("HitBatch.ini", pInifile);

                    StreamWriter wr = new StreamWriter("C:" + HitBatchBASEDIR + @"\HitBatch\" + lTijd + "_" + pUSERID  + ".cmd", false);
                    try 
                    {
                        wr.Write(fileContents);
                    }
                    catch(Exception exc) { unLogger.WriteError(exc.ToString()); }
                    finally { wr.Flush(); wr.Close(); }
                    string strCommand = "C:" + HitBatchBASEDIR + @"\HitBatch\" + lTijd + "_" + pUSERID + ".cmd";

                    /*
                     * Find the file and
                     * REPLACE HitBatch.ini with your inifile
                     * 
                        @echo off
                        REM
                        REM Aufrufscript
                        REM

                        cd /d %~dp0

                        set BASEDIR=\projekt\HIT


                        :AGAIN
                        set HBINI=%1
                        if %HBINI%.==. set HBINI=HitBatch.ini
                        if exist %HBINI% goto RUNHB
                        echo Parameter-Datei: %HBINI% nicht gefunden
                        goto FERTIG

                        :RUNHB
                        echo HitBatch mit Parameterdatei %HBINI%
                        java -classpath "%BASEDIR%\HitUpros\HitUpros.jar;%BASEDIR%\HitBatch\HitBatch.jar" HitBatch.HitBatchMain %HBINI%
                        if ERRORLEVEL 5 goto ERROR
                        if ERRORLEVEL 4 goto REPEAT
                        if ERRORLEVEL 1 goto ERROR
                        goto FERTIG

                        :REPEAT
                        echo Wiederholen ???
                        pause
                        goto AGAIN1

                        :ERROR
                        echo Transfer der Daten nicht erfolgreich
                        pause
                        goto FERTIG


                        ERRORLEVELs:   0 = Verarbeitung OK, Details siehe Ergebns-Dateien
                                       1 = Impressum ausgegeben, keine weitere Verarbeitung
                                       2 = Parameter-Fehler, keine weitere Verarbeitung
                                       3 = Lese-Fehler, Verarbeitung wegen Problemen abgebrochen
                                       4 = Fehler in Verbindung zum HIT-Server, Verarbeitung abgebrochen
                                       5 = System-Fehler, Fehlertext bitte an Entwicklung melden

                        :FERTIG
                        exit /B
                     */

                    //Create process
                    System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

                    //strCommand is path and file name of command to run
                    pProcess.StartInfo.FileName = strCommand;

                    //strCommandParameters are parameters to pass to program
                    //pProcess.StartInfo.Arguments = strCommandParameters;

                    pProcess.StartInfo.UseShellExecute = false;

                    //Set output of program to be written to process output stream
                    pProcess.StartInfo.RedirectStandardOutput = true;
                    pProcess.StartInfo.CreateNoWindow = true;
                    //Optional
                    //pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;

                    //Start the process
                    pProcess.Start();

                    //Get program output
                    string strOutput = pProcess.StandardOutput.ReadToEnd();
                    dReturn.Code = "";
                    dReturn.Date = DateTime.Now.Date;
                    dReturn.FarmNumber = pBedrijfsnummer;
                    dReturn.Internalnr = 0;
                    dReturn.Kind = 1400;
                    dReturn.Lifenumber = "";
                    int index = strOutput.LastIndexOf("Antwort #");
                    if (index > 0)
                    {
                        strOutput = strOutput.Remove(0, index);
                    }
                    dReturn.Omschrijving = strOutput;
                    dReturn.Status = "F";
                    dReturn.Time = DateTime.Now;
                    unLogger.WriteDebug(strOutput);
                    //Wait for process to finish
                    pProcess.WaitForExit();
                    //als er iets in OUT staat en niets in BAD 
                    StreamReader rdrOUT = new StreamReader(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.OUT");
                    try 
                    {
                        string lLine = rdrOUT.ReadToEnd();
                        if (lLine.Length > 0)
                        {

                            dReturn.Code = "";
                            dReturn.Date = DateTime.Now.Date;
                            dReturn.FarmNumber = pBedrijfsnummer;

                            dReturn.Kind = 1400;
                            dReturn.Lifenumber = "";
                            dReturn.Omschrijving = "Data found";
                            dReturn.Status = "G";
                            dReturn.Time = DateTime.Now;
                        }
                        else
                        {
                            dReturn.Code = "";
                            dReturn.Date = DateTime.Now.Date;
                            dReturn.FarmNumber = pBedrijfsnummer;

                            dReturn.Kind = 1400;
                            dReturn.Lifenumber = "";
                            dReturn.Omschrijving = " No Data found : ";
                            dReturn.Status = "F";
                            dReturn.Time = DateTime.Now;
                        }
                    }
                    catch { }
                    finally { rdrOUT.Close(); }
                    if (dReturn.Status != "G")
                    {
                        StreamReader rdrBAD = new StreamReader(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.BAD");
                        //dReturn.Omschrijving = TemplogDir + lTijd + "_" + pUSERID + "_BETRH.BAD";
                        try
                        {
                            string lLine = rdrBAD.ReadToEnd();
                            if (lLine.Length > 0)
                            {
                                dReturn.Omschrijving += lLine;
                            }
                        }
                        catch { }
                        finally { rdrBAD.Close(); }
                        StreamReader rdrLOG = new StreamReader(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.LOG");
                        //dReturn.Omschrijving += TemplogDir + lTijd + "_" + pUSERID + "_BETRH.LOG";
                        try
                        {
                            string lLine = rdrLOG.ReadToEnd();
                            if (lLine.Length > 0)
                            {
                                dReturn.Omschrijving += lLine;
                            }
                        }
                        catch { }
                        finally { rdrLOG.Close(); }
                    }
                }
                catch (Exception exc)
                {
                    dReturn.Code = "";
                    dReturn.Date = DateTime.Now.Date;
                    dReturn.FarmNumber = pBedrijfsnummer;
                   
                    dReturn.Kind = 1400;
                    dReturn.Lifenumber = "";
                    dReturn.Omschrijving = "Error:" + exc.Message;
                    dReturn.Status = "F";
                    dReturn.Time = DateTime.Now;
                    unLogger.WriteError("Error Hitbatch.cmd" + exc.ToString());
                }
                finally { }
            }
            return dReturn;
        }

        private string CreateIniFile(string pUSERID, string pPin, string mitbenutzer, string pHitBatchBASEDIR)
        {
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
         
            StreamWriter wr = new StreamWriter(@"C:\" + pHitBatchBASEDIR + @"\HitBatch\" + lTijd + "_" + pUSERID + ".ini ", false);
            bool err = false;
            try
            {
                wr.WriteLine("[GLOBAL] ");

                wr.WriteLine(" USERID=" + pUSERID + "");//051540281004 ");
                wr.WriteLine(" PIN=" + pPin + "");//513620 ");

                if (string.IsNullOrWhiteSpace(mitbenutzer))
                {
                    wr.WriteLine(" MBN=0 ");
                }
                else
                {
                    wr.WriteLine($@" MBN={mitbenutzer} ");
                }
                
                wr.WriteLine(" MANDANT=0 ");

                wr.WriteLine(" PRIMARYSERVER=hitserver.hi-tier.de ");
                wr.WriteLine(" PRIMARYPORT=" + poort + "");
                wr.WriteLine(" BACKUPSERVER=hitbackup.hi-tier.de ");
                wr.WriteLine(" BACKUPPORT=" + poort + "");

                wr.WriteLine(" PROTOLEVEL=2 ");
                wr.WriteLine(" SATZINTERVALL=100 ");
                wr.WriteLine(" MELDEWEG=4 ");
                wr.WriteLine(" ANTWORTVERBOSE=3 ");
                wr.WriteLine(" SCHWERE=1 ");
                wr.WriteLine(" TIMEOUT=200 ");
                wr.WriteLine(" BLOCKMAX=100 ");
                wr.WriteLine(" BLOCKRESTLOG=blockrest.log ");


                wr.WriteLine(" OutAppend=0 ");
                wr.WriteLine(" LogAppend=0 ");
                wr.WriteLine(" GoodAppend=0 ");
                wr.WriteLine(" BadAppend=0 ");

                wr.WriteLine(" SETCOUNT=4 ");
                wr.WriteLine(" STARTSET=1 ");


                wr.WriteLine(" [SET-1] ");
                wr.WriteLine(" MELDUNG=BETRD ");//stallijst
                wr.WriteLine(" COMMAND=RS ");
                wr.WriteLine(" DATALINESSOFAR=0 ");
                wr.WriteLine(" INPUTAFTERSUCCESS=0 ");
                wr.WriteLine(" TESTDOPPELTEHEADER=1 ");
                wr.WriteLine(@" INFILE=" + TemplogDir + lTijd + "_" + pUSERID + "_BETRH.CSV ");
                wr.WriteLine(@" OUFILE=" + TemplogDir + lTijd + "_" + pUSERID + "_BETRH.OUT ");
                wr.WriteLine(@" LOGFILE=" + TemplogDir + lTijd + "_" + pUSERID + "_BETRH.LOG ");
                wr.WriteLine(@" GOODFILE=" + TemplogDir + lTijd + "_" + pUSERID + "_BETRH.GOD ");
                wr.WriteLine(@" BADFILE=" + TemplogDir + lTijd + "_" + pUSERID + "_BETRH.BAD ");
                wr.WriteLine(" CSVIN=0 ");
                wr.WriteLine(" CSVOUT=0 ");
                wr.WriteLine(" CSVLOG=0 ");


                wr.WriteLine("[SET-2]");
                wr.WriteLine("Meldung=GEBURT ");//Geboortemeldingen
                wr.WriteLine("Command=RS ");
                wr.WriteLine("VerhaltenBeiNachfrage=1 ");
                wr.WriteLine("DataLinesSoFar=0 ");
                wr.WriteLine("InputAfterSuccess=0 ");
                wr.WriteLine("InFile=" + TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.CSV ");
                wr.WriteLine("OuFile=" + TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.OUT ");
                wr.WriteLine("LogFile=" + TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.LOG ");
                wr.WriteLine("CsvIn=1 ");
                wr.WriteLine("CsvOut=0 ");
                wr.WriteLine("CsvLog=0 ");
                wr.WriteLine("TestDoppelteHeader=0 ");

                wr.WriteLine("[SET-3]");
                wr.WriteLine("Meldung=ABGANG ");//afvoermeldingen
                wr.WriteLine("Command=RS ");
                wr.WriteLine("VerhaltenBeiNachfrage=1 ");
                wr.WriteLine("DataLinesSoFar=0 ");
                wr.WriteLine("InputAfterSuccess=0 ");
                wr.WriteLine("InFile=" + TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.CSV ");
                wr.WriteLine("OuFile=" + TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.OUT ");
                wr.WriteLine("LogFile=" + TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.LOG ");
                wr.WriteLine("CsvIn=1 ");
                wr.WriteLine("CsvOut=0 ");
                wr.WriteLine("CsvLog=0 ");
                wr.WriteLine("TestDoppelteHeader=0 ");

                wr.WriteLine("[SET-4]");
                wr.WriteLine("Meldung=ZUGANG ");//aanvoermeldingen ?????????
                wr.WriteLine("Command=RS ");
                wr.WriteLine("VerhaltenBeiNachfrage=1 ");
                wr.WriteLine("DataLinesSoFar=0 ");
                wr.WriteLine("InputAfterSuccess=0 ");
                wr.WriteLine("InFile=" + TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.CSV ");
                wr.WriteLine("OuFile=" + TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.OUT ");
                wr.WriteLine("LogFile=" + TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.LOG ");
                wr.WriteLine("CsvIn=1 ");
                wr.WriteLine("CsvOut=0 ");
                wr.WriteLine("CsvLog=0 ");
                wr.WriteLine("TestDoppelteHeader=0 ");

            }
            catch (Exception exc) { unLogger.WriteError("HtierClient  CreateIniFile:" + exc.ToString()); err = true; }
            finally { wr.Flush(); wr.Close(); }
            if (err) { return ""; }
            return lTijd + "_" + pUSERID + ".ini";
        }
   
        private string CreateMeldungsIniFile(string pUSERID, string pPin, string mitbenutzer, string pHitBatchBASEDIR, int pCodeMutation)
        {
            switch (pCodeMutation)
            { 
              case 1:
                        //Aanvoer
                     
                        break;
                    case 2:
                        //Geboorte
                      
                        break;
                    case 3:
                        //Doodgeb.
               
                        break;
                    case 4:
                        //Afvoer
              
                        break;
                    case 5:
                        //IKB afvoer
             
                        break;
                    case 6:
                        //Dood
                      

                        break;
                    case 7:
                        //Import
                 
                        break;
                    case 8:
                        //Aan-/afvoer
                     
                        break;
                    case 9:
                        //Export
          
                        break;
                    case 10:
                        //Slacht
          
                        break;
                    case 11:
                    //aanvoer
          
                        break;
                    case 12:
                        //Uitscharen
            
                        break;
                    case 13:
                        //Noodslacht
              
                        break;
                    case 16:
                        //Q-Krts vacc1
                  
                        break;
                    case 17:
                      //QKrtsvacc2
                        
                        break;
                    case 18:
                      //QKrtsvaccH
                    
                        break;
                    case 101://INTREKmeldingen
                    case 102:
                    case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                    case 108:
                    case 109:
                    case 110:
                    case 111:
                    case 112:
                    case 113:
                    case 116:
                    case 117:
                    case 118:
               

                        break;

                    //herstellen  
                    case 201:
                        //Aanvoer
                     
                        break;
                    case 202:
                        //Geboorte
                      
                        break;
                    case 203:
                        //Doodgeb.
                      
                        break;
                    case 204:
                        //Afvoer
                    
                        break;
                    case 205:
                        //IKB afvoer
                 
                        break;
                    case 206:
                        //Dood
                     
                        break;
                    case 207:
                        //Import
                  
                        break;
                    case 208:
                        //Aan-/afvoer
                 
                        break;
                    case 209:
                        //Export
                     
                        break;
                    case 210:
                        //Slacht
                  
                        break;
                    case 211:
                    //HerstelAanvoer11
                       
                        break;
                    case 212:
                        //Uitscharen
 
                        break;
                    case 213:
                        //Noodslacht
                    
                        break;
                }
            
            return "";
        }
   
        private string CreateINFILES(string pUSERID, DateTime pBegindatum, DateTime pEinddatum)
        {
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            string lBegin = pBegindatum.ToString("dd.MM.yyyy");
            string lEind = pEinddatum.ToString("dd.MM.yyyy");
            StreamWriter wr1 = new StreamWriter(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.CSV", false);
            StreamWriter wr2 = new StreamWriter(TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.CSV", false);
            StreamWriter wr3 = new StreamWriter(TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.CSV", false);
            StreamWriter wr4 = new StreamWriter(TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.CSV", false);
            bool err = false;
            try
            {


          
                //Set 1

                FileStream strOUT = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.OUT ");
                strOUT.Close();
                FileStream strLOG = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.LOG ");
                strLOG.Close();
                FileStream strGOD = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.GOD ");
                strGOD.Close();
                FileStream strBAD = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_BETRH.BAD ");
                strBAD.Close();

                wr1.WriteLine("#BESTREG(" + lBegin + ";" + lEind + ")");
                wr1.WriteLine("BNR15;EQ;" + pUSERID);

                //Set 2

                FileStream strOUT2 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.OUT ");
                strOUT2.Close();
                FileStream strLOG2 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.LOG ");
                strLOG2.Close();
                FileStream strGOD2 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.GOD ");
                strGOD2.Close();
                FileStream strBAD2 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_GEBURT.BAD ");
                strBAD2.Close();

                wr2.WriteLine("*");
                wr2.WriteLine("BNR15;EQ;" + pUSERID);

                //Set 3

                FileStream strOUT3 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.OUT ");
                strOUT3.Close();
                FileStream strLOG3 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.LOG ");
                strLOG3.Close();
                FileStream strGOD3 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.GOD ");
                strGOD3.Close();
                FileStream strBAD3 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ABGANG.BAD ");
                strBAD3.Close();

                wr3.WriteLine("*");
                wr3.WriteLine("BNR15;EQ;" + pUSERID);

                //Set 4

                FileStream strOUT4 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.OUT ");
                strOUT4.Close();
                FileStream strLOG4 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.LOG ");
                strLOG4.Close();
                FileStream strGOD4 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.GOD ");
                strGOD4.Close();
                FileStream strBAD4 = File.Create(TemplogDir + lTijd + "_" + pUSERID + "_ZUGANG.BAD ");
                strBAD4.Close();

                wr4.WriteLine("*");
                wr4.WriteLine("BNR15;EQ;" + pUSERID);
            }
            catch (Exception exc) { unLogger.WriteError("HtierClient  CreateINFILE:" + exc.ToString()); err = true; }
            finally 
            { 
                wr1.Flush(); wr1.Close();
                wr2.Flush(); wr2.Close();
                wr3.Flush(); wr3.Close();
                wr4.Flush(); wr4.Close();
            }
            if (err) { return ""; }
            return TemplogDir + lTijd + "_" + pUSERID + "_BETRH.CSV";
        }

        public string getLifeNumberFromLOM_X(string pLOM_X)
        {
            char[] spl = { ' ' };
            string[] lef = pLOM_X.Split(spl);
            if (lef.Length >= 2)
            {
                StringBuilder bld = new StringBuilder();
                for (int i = 0; i < lef.Length; i++)
                {
                    if (i == 0)
                    { bld.Append(lef[i] + " "); }
                    else { bld.Append(lef[i]); }
                }
                return bld.ToString();
            }
            else { return pLOM_X; }

        }
        //DE 05 335 34895   //DE 0533534895
        private string getLOM_XFromLifeNumber(string pLifeNumber)
        {
            char[] spl = { ' ' };
            string[] lef = pLifeNumber.Split(spl);
            if (lef.Length == 2)
            {
                StringBuilder bld = new StringBuilder(lef[0] + " ");
                for (int i = 0; i < lef[1].Length; i++)
                {
                    if (i == 2 || i==6)
                    { bld.Append(lef[i] + " "); }
                    else { bld.Append(lef[i]); }
                }
                return bld.ToString();
            }
            else { return pLifeNumber; }

        }

        public DataTable getHtierStallijstPreloaded(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                                   DateTime pBegindatum, DateTime pEinddatum, out SOAPLOG dReturn)
        {
            dReturn = new SOAPLOG();
            DataTable stallijst = new DataTable();
            string pin = "";
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            string[] KolsBETRH = { "#BESTREG", "BNR15", "BNR15_X", "DAT_VON", "DAT_BIS", "LOM", "LOM_X", "GEB_DATR", "RASSE", "RASSE_X", "GESCHL_R", "GESCHL_X", "LOM_MUT", "LOM_MUTX", "LOM_A", "LAND_URS", "LAND_URX", "TIER_EIN", "TIER_EINX", "DAT_EIN", "BNR15_VB", "BNR15_VBX", "TIER_AUS", "TIER_AUSX", "DAT_AUS", "BNR15_NB", "BNR15_NBX", "GVE", "EKALBLOM", "EKALBLOMX", "EKALBDAT", "LKALBDAT", "KALBANZ", "KALBUNGANZ", "TIER_END", "TIER_ENDX", "DAT_END", "BNR15_EB", "BNR15_EBX", "GVE_L8VJA", "GVE_L8HEU", "SCHL_KAT", "SCHL_KATX", "SCHL_NR", "SCHL_GEW", "SCHL_LEB", "BNR15_AB", "BNR15_ABX", "BNR15_GEB", "BNR15_GEX", "GVE_ERROR", "BNR15_VX", "BNR15_VXX", "BEW_DAT", "BEW_ART", "BEW_MELD", "PLAUSINR", "SCHWERE", "FEHLERTEXT", "SCHWEREX", "BNR15_LMON", "BNR15_LMOX", "LAND_ZIE", "LAND_ZIX", "BEW_ART_TD", "DATUM", "DAT_LMON", "ZKZ_DURCH", "ZKZ_DURCHT", "BNR15_BT", "BNR15_BT_X", "BREG_TAMKM", "BREG_TAMKW", "BREG_TAMRM", "BREG_TAMRW", "RD_INUTZ", "RD_STALLNR", "RD_TEILNR", "RD_TEILBEZ", "RD_WERTG", "RD_WERTD", "RD_BEMERK", "RD_DATUM", "RD_BNR15", "RD_BNR15UN" };
            if (USERID == "" || pin == "")
            {
                AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
                UBN lUBN = DB.GetubnById(pUBNid);
                dReturn.Code = "Stallijst";
                dReturn.Date = DateTime.Now.Date;
                dReturn.FarmNumber = lUBN.Bedrijfsnummer;

                dReturn.Kind = 1400;
                dReturn.Lifenumber = "";
                dReturn.Omschrijving = "Geen inlogcodes gevonden";
                dReturn.Status = "F";
                dReturn.Time = DateTime.Now;
                return stallijst;
            }
            if (File.Exists(TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT"))
            {
                stallijst = utils.GetCsvData(KolsBETRH, TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT", ';', "DeutschBETRH");
                if (stallijst.Rows.Count > 1)
                {
                    stallijst.Rows.Remove(stallijst.Rows[0]);
                    dReturn.Status = "G";
                }
                else
                {
                    dReturn.Status = "F";
                    dReturn.Omschrijving = "Ophalen stallijst niet gelukt.";
                }
                
            }
            return stallijst;
        }

        public DataTable getHtierStallijst(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                                   DateTime pBegindatum, DateTime pEinddatum, out SOAPLOG dReturn)
        {
            dReturn = new SOAPLOG();
            DataTable stallijst = new DataTable();
            string pin = "";
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            if (USERID == "" || pin == "")
            {
                AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
                UBN lUBN = DB.GetubnById(pUBNid);
                dReturn.Code = "Stallijst";
                dReturn.Date = DateTime.Now.Date;
                dReturn.FarmNumber = lUBN.Bedrijfsnummer;
              
                dReturn.Kind = 1400;
                dReturn.Lifenumber = "";
                dReturn.Omschrijving = "Geen inlogcodes gevonden";
                dReturn.Status = "F";
                dReturn.Time = DateTime.Now;
            }
            else
            {
                string[] KolsBETRH = { "#BESTREG", "BNR15", "BNR15_X", "DAT_VON", "DAT_BIS", "LOM", "LOM_X", "GEB_DATR", "RASSE", "RASSE_X", "GESCHL_R", "GESCHL_X", "LOM_MUT", "LOM_MUTX", "LOM_A", "LAND_URS", "LAND_URX", "TIER_EIN", "TIER_EINX", "DAT_EIN", "BNR15_VB", "BNR15_VBX", "TIER_AUS", "TIER_AUSX", "DAT_AUS", "BNR15_NB", "BNR15_NBX", "GVE", "EKALBLOM", "EKALBLOMX", "EKALBDAT", "LKALBDAT", "KALBANZ", "KALBUNGANZ", "TIER_END", "TIER_ENDX", "DAT_END", "BNR15_EB", "BNR15_EBX", "GVE_L8VJA", "GVE_L8HEU", "SCHL_KAT", "SCHL_KATX", "SCHL_NR", "SCHL_GEW", "SCHL_LEB", "BNR15_AB", "BNR15_ABX", "BNR15_GEB", "BNR15_GEX", "GVE_ERROR", "BNR15_VX", "BNR15_VXX", "BEW_DAT", "BEW_ART", "BEW_MELD", "PLAUSINR", "SCHWERE", "FEHLERTEXT", "SCHWEREX", "BNR15_LMON", "BNR15_LMOX", "LAND_ZIE", "LAND_ZIX", "BEW_ART_TD", "DATUM", "DAT_LMON", "ZKZ_DURCH", "ZKZ_DURCHT", "BNR15_BT", "BNR15_BT_X", "BREG_TAMKM", "BREG_TAMKW", "BREG_TAMRM", "BREG_TAMRW", "RD_INUTZ", "RD_STALLNR", "RD_TEILNR", "RD_TEILBEZ", "RD_WERTG", "RD_WERTD", "RD_BEMERK", "RD_DATUM", "RD_BNR15", "RD_BNR15UN" };

                dReturn = HTierRaadplegenAlgemeen(mToken, pUBNid, pProgId, pProgramid, pBegindatum, pEinddatum);
                dReturn.Code = "Stallijst";
                if (dReturn.Status == "G" || dReturn.Omschrijving == "Data found")
                {

                    stallijst = utils.GetCsvData(KolsBETRH, TemplogDir + lTijd + "_" + USERID + "_BETRH.OUT", ';', "DeutschBETRH");
                    if (stallijst.Rows.Count > 1)
                    {
                        stallijst.Rows.Remove(stallijst.Rows[0]);
                    }
                    //string[] KolsGEBURT = { "LOM", "BNR15", "GEB_DATR", "RASSE", "GESCHL_R", "GEB_VERL", "VERBLEIB", "MEHRLING", "MEHRLADR", "ET_KALB", "KENZ_ART", "MELD_DAT", "LOM_MUT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST", "TIERNAMENR", "TIERNAME", "GEB_FEST", "GEB_NABBR", "GEB_NAGEBV" };
                    //DataTable tblGEBURT = utils.GetCsvData(KolsGEBURT, TemplogDir + lTijd + "_" + USERID + "_GEBURT.OUT", ';', "Deutsch");
                    //tblGEBURT.Rows.Remove(tblGEBURT.Rows[0]);

                    //string[] KolsABGANG = { "LOM", "BNR15", "ABGA_DAT", "MELD_DAT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST" };
                    //DataTable tblABGANG = utils.GetCsvData(KolsABGANG, TemplogDir + lTijd + "_" + USERID + "_ABGANG.OUT", ';', "Deutsch");
                    //tblABGANG.Rows.Remove(tblABGANG.Rows[0]);

                    //string[] KolsZUGANG = { "LOM", "BNR15", "ZUGA_DAT", "MELD_DAT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST" };
                    //DataTable tblZUGANG = utils.GetCsvData(KolsZUGANG, TemplogDir + lTijd + "_" + USERID + "_ZUGANG.OUT", ';', "Deutsch");
                    //tblZUGANG.Rows.Remove(tblZUGANG.Rows[0]);

                }
            }
            return stallijst;
        }

        public DataRow[] getAanvoeren(string pLOM, DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid)
        {
            string pin = "";
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            string[] KolsZUGANG = { "LOM", "BNR15", "ZUGA_DAT", "MELD_DAT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST" };
            DataTable tblZUGANG = utils.GetCsvData(KolsZUGANG, TemplogDir + lTijd + "_" + USERID + "_ZUGANG.OUT", ';', "Deutsch");
          
            DataRow[] result = tblZUGANG.Select("LOM='" + pLOM + "'");
            return result;
        }

        public DataRow[] getAfvoeren(string pLOM, DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid)
        {
            string pin = "";
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            string[] KolsABGANG = { "LOM", "BNR15", "ABGA_DAT", "MELD_DAT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST" };
            DataTable tblABGANG = utils.GetCsvData(KolsABGANG, TemplogDir + lTijd + "_" + USERID + "_ABGANG.OUT", ';', "Deutsch");
          
            DataRow[] result = tblABGANG.Select("LOM='" + pLOM + "'");
            return result;
        }

        public DataRow[] getGeboorte(string pLOM, DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid)
        {
            string pin = "";
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out pin, out mitbenutzer);
            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            string[] KolsGEBURT = { "LOM", "BNR15", "GEB_DATR", "RASSE", "GESCHL_R", "GEB_VERL", "VERBLEIB", "MEHRLING", "MEHRLADR", "ET_KALB", "KENZ_ART", "MELD_DAT", "LOM_MUT", "MELD_WG", "MELD_BNR", "MELD_MBN", "SYS_VON", "SYS_BIS", "SYS_STAT", "SYS_CLUST", "TIERNAMENR", "TIERNAME", "GEB_FEST", "GEB_NABBR", "GEB_NAGEBV" };
            DataTable tblGEBURT = utils.GetCsvData(KolsGEBURT, TemplogDir + lTijd + "_" + USERID + "_GEBURT.OUT", ';', "Deutsch");
          
            DataRow[] result = tblGEBURT.Select("LOM='" + pLOM + "'");
            return result;
        }

        public void HitierAanvoerMelding(DBConnectionToken mToken, int pTestserver, int pUBNid, int pProgId, int pProgramid,
                                    string Farmnumber, string lBRSnummer, string pLifenumber, string pFarmNumberFrom, DateTime pMutationDate,
                                    int HerstelMelding, string MeldingNummer,
                                    ref string lStatus, ref string lCode, ref string lOmschrijving, ref string lMeldingsnr, string LogFile)
        {

            poort = port;
            if (pTestserver > 0)
            { poort = testport; }

            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            string HitBatchBASEDIR = DB.GetProgramConfigValue(pProgramid, "HitBatchBASEDIR");
            if (HitBatchBASEDIR == "")
            {
                HitBatchBASEDIR = @"\projekt\HIT";
            }
            UBN lUBN = DB.GetubnById(pUBNid);

          
            SOAPLOG Result = new SOAPLOG();

            string Pin = "513620";// fusoap.Password;
            string mitbenutzer = "";
            string USERID = getUSERID(mToken, pUBNid, pProgId, pProgramid, out Pin, out mitbenutzer); // fusoap.UserName;

            string lTijd = DateTime.Now.ToString("yyyyMMdd");
            string melding = @" ;LOM;BNR15;ZUGA_DAT
                X;276000900000001;" + USERID + ";19.10.1999 ";
            string inifile = CreateMeldungsIniFile(USERID, Pin, mitbenutzer, HitBatchBASEDIR, 1);
           

        }

        public SOAPLOG FindAnimalVerblijfplaatsen(string userName, string password, string bedrijfsnummer, List<KeyValuePair<string, int>> dcfAnimalList, int v, DateTime pBegindatum, DateTime pEinddatum, string pOutputfile)
        {
            SOAPLOG sl = new SOAPLOG();
            sl.Status = "F";
            sl.Omschrijving = "Nog niet beschikbaar";
            return sl;
        }
    }
}