using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE
{
    public class utils
    {

        public static string addleadingZero(string getalLTten)
        {
            if (getalLTten.Length == 1)
            { getalLTten = "0" + getalLTten; }
            return getalLTten;
        }

        public static string addleadingCharacters(string pWord,char pChar,int pMaxPositions)
        {
            while (pWord.Length < pMaxPositions)
            { pWord = pChar + pWord; }
            return pWord;
        }

        public static string RemoveAnimalLifeNumberLeadingZeros(string pAnimalUniqueNumber)
        {
            char[] split = { ' ' };
            string[] lNumber = pAnimalUniqueNumber.Split(split);
            if (lNumber.Length == 2)
            {
                while (lNumber[1].StartsWith("0"))
                {
                    if (lNumber[1].Length > 4)
                    {
                        lNumber[1] = lNumber[1].Remove(0, 1);
                    }
                    else { break; }
                }
                return lNumber[0] + " " + lNumber[1];
            }
            return pAnimalUniqueNumber;

        }

        public static string getcurrentlanguage()
        {
            try
            {
                string lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

                return lang.ToUpper();
            }
            catch(Exception exc) { unLogger.WriteError(exc.ToString()); return "NL"; }
        }
   
        public static bool isEULand(string pLandCode2)
        {
            switch (pLandCode2)
            {
                case "AD":
                    return true;
                case "AL":
                    return true;
                case "AT":
                    return true;
                case "BA":
                    return true;
                case "BE":
                    return true;
                case "BG":
                    return true;
                case "BY":
                    return true;
                case "CH":
                    return true;
                case "CY":
                    return true;
                case "CZ":
                    return true;
                case "DE":
                    return true;
                case "DK":
                    return true;
                case "EE":
                    return true;
                case "ES":
                    return true;
                case "FI":
                    return true;
                case "FO":
                    return true;
                case "FR":
                    return true;
                case "GG":
                    return true;
                case "GI":
                    return true;
                case "GR":
                    return true;
                case "HR":
                    return true;
                case "HU":
                    return true;
                case "IE":
                    return true;
                case "IM":
                    return true;
                case "IS":
                    return true;
                case "IT":
                    return true;
                case "JE":
                    return true;
                case "LI":
                    return true;
                case "LT":
                    return true;
                case "LU":
                    return true;
                case "LV":
                    return true;
                case "MC":
                    return true;
                case "MD":
                    return true;
                case "MK":
                    return true;
                case "MT":
                    return true;
                case "NL":
                    return true;
                case "NO":
                    return true;
                case "PL":
                    return true;
                case "PT":
                    return true;
                case "RO":
                    return true;
                case "RU":
                    return true;
                case "SE":
                    return true;
                case "SI":
                    return true;
                case "SJ":
                    return true;
                case "SK":
                    return true;
                case "SM":
                    return true;
                case "TR":
                    return true;
                case "UA":
                    return true;
                case "UK":
                    return true;
                case "VA":
                    return true;
                case "YU":
                    return true;
            }
            return false;
           
        }
     
        public static void writeLogFile(StringBuilder rm, bool append)
        {
            if (rm != null)
            {
                try
                {

                    DirectoryInfo dir = new DirectoryInfo(@"C:/Datacom");
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    string pad = @"C:/Datacom/errRumaonlinelog.log";
                    
                    StreamWriter wr = new StreamWriter(pad, append);
                    unLogger.WriteObject(rm);
                    wr.WriteLine(rm.ToString());
                    wr.Flush();
                    wr.Close();
                }
                catch { }
            }
        }

        public static string getLabelsLabcountrycode()
        {
            try
            {
                switch (utils.getcurrentlanguage())
                {
                    case "NL":
                        return "528";// "1"; oude LabCountry Codes
                    case "DE":
                        return "276";// "3";
                    case "FR":
                        return "250";// "4";
                    case "EN":
                        return "826";// "2";
                    default:
                        return "528";
                }
            }
            catch { return "528"; }
        }

        public static string getDatumFormat(DateTime datum)
        {
            return datum.ToString("dd-MM-yyyy");
        }

        public static string getDatumExchange(DateTime datum)
        {
            return addleadingZero(datum.Day.ToString()) + "-" + addleadingZero(datum.Month.ToString()) + "-" + datum.Year.ToString();
        }

        public static string getTimeExchange(DateTime datum)
        {
            return addleadingZero(datum.Hour.ToString()) + ":" + addleadingZero(datum.Minute.ToString()) + ":" + addleadingZero(datum.Second.ToString());
        }

        public static DateTime getDate(string datum)
        {
            try
            {
                if (!string.IsNullOrEmpty(datum))
                {
                    DateTime ret = new DateTime();
                    ret = DateTime.MinValue;
                    if (DateTime.TryParse(datum, out ret))
                    {
                        return ret;
                    }
                    else
                    {
                        ret = getDateFormat(datum);
                        if (ret == DateTime.MinValue)
                        {
                            ret = getDateLNV(datum);
                        }
                    }
                }
                else { unLogger.WriteError("utils.getDate parameter datum=null "); }
            }
            catch (Exception exc) { unLogger.WriteError("utils.getDate " + exc.ToString()); }
            return DateTime.MinValue;
        }

        public static DateTime getDateFormat(string datum)
        {
            //verwachte  format = 31-12-2009
            // of  31-12-2009 00:00:00
            char[] split = { '-', ' ' };
            string[] datum1 = datum.Split(split);
            if (datum1.Length == 3 || datum1.Length == 4)
            {

                try
                {
                    DateTime dat1 = new DateTime(int.Parse(datum1[2]), int.Parse(datum1[1]), int.Parse(datum1[0]));
                    return dat1;
                }
                catch { return DateTime.MinValue; }

            }
            else { return DateTime.MinValue; }

        }

        /// <summary>
        /// verwachte  format = 20101231  2010 decemer 31
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>
        public static DateTime getDateLNV(string datum)
        {
            //verwachte  format = 20101231

            try
            {
                datum = datum.Trim();
                if (datum.Length == 8)
                {
                    string jaar = datum.Substring(0, 4);
                    string maand = datum.Substring(4, 2);
                    string dag = datum.Substring(6, 2);
                    DateTime dat1 = new DateTime(int.Parse(jaar), int.Parse(maand), int.Parse(dag));
                    return dat1;
                }
                else 
                {
                    unLogger.WriteInfo("CORE.utils.getDateLNV datum="+datum); 
                    return DateTime.MinValue; 
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
                return DateTime.MinValue;
            }

        }

        /// <summary>
        /// verwachte  format: 28012019 28 januari 2019
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>
        public static DateTime getDateSlacht(string datum)
        {
           
            try
            {
                datum = datum.Trim();
                if (datum.Length == 8)
                {
                    string jaar = datum.Substring(4, 4);
                    string maand = datum.Substring(2, 2);
                    string dag = datum.Substring(0, 2);
                    DateTime dat1 = new DateTime(int.Parse(jaar), int.Parse(maand), int.Parse(dag));
                    return dat1;
                }
                else
                {
                    unLogger.WriteInfo("CORE.utils.getDateSlacht datum=" + datum);
                    return DateTime.MinValue;
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
                return DateTime.MinValue;
            }

        }

        public static string getpercentage(string getal)
        {
            switch (getal)
            {
                case "1":
                    return "12,5%";
                case "2":
                    return "25%";
                case "3":
                    return "37,5%";
                case "4":
                    return "50%";
                case "5":
                    return "62,5%";
                case "6":
                    return "75%";
                case "7":
                    return "87,5%";
                case "8":
                    return "100%";
                default:
                    return "0%";
            }
        }

        public static bool isLNVStaldier(string pLNVdiersoort)
        {
            bool isStaldier = false;
            int lControle = -1;
            int.TryParse(pLNVdiersoort, out lControle);
            if (lControle > -1)
            {
                switch (lControle)
                { 
                    case 11:
                        isStaldier = true;
                        break;
                    case 40:
                        isStaldier = true;
                        break;
                    case 30:
                        isStaldier = true;
                        break;
                    case 20:
                        isStaldier = true;
                        break;
                    case 75:
                        isStaldier = true;
                        break;
                    case 90:
                        isStaldier = true;
                        break;
                    case 15:
                        isStaldier = true;
                        break;
                    case 16:
                        isStaldier = true;
                        break;
                    case 17:
                        isStaldier = true;
                        break;
                    case 18:
                        isStaldier = true;
                        break;
                    case 19:
                        isStaldier = true;
                        break;
                    case 80:
                        isStaldier = true;
                        break;
                    case 25:
                        isStaldier = true;
                        break;
                    case 26:
                        isStaldier = true;
                        break;
                    case 27:
                        isStaldier = true;
                        break;
                    case 28:
                        isStaldier = true;
                        break;
                    case 29:
                        isStaldier = true;
                        break;
                    case 95:
                        isStaldier = true;
                        break;
                    case 35:
                        isStaldier = true;
                        break;
                    case 36:
                        isStaldier = true;
                        break;
                    case 37:
                        isStaldier = true;
                        break;
                }
            }
            return isStaldier;
        }

        public static int getLabId260(int pMinascategory)
        {

            switch (pMinascategory)
            {
                case 0:
                    return 10;
                case 1:
                    return 20;
                case 2:
                    return 30;
                case 3:
                    return 40;
                case 4:
                    return 55;
                case 5:
                    return 60;
                case 6:
                    return 0;
                case 7:
                    return 7;
                case 8:
                    return 0;// "000";
                case 9:
                    return 90;// "090";
                case 10:
                    return 0;// "000";
                case 11:
                    return 0;// "000";
                case 12:
                    return 0;// "000";
                case 13:
                    return 0;// "000";
                case 14:
                    return 0;// "000";
                case 15:
                    return 15;// "015";
                case 16:
                    return 16;// "016";
                case 17:
                    return 17;// "017";
                case 18:
                    return 18;// "018";
                case 19:
                    return 19;// "019";
                case 20:
                    return 25;// "025";
                case 21:
                    return 26;// "026";
                case 22:
                    return 27;// "027";
                case 23:
                    return 28;// "028";
                case 24:
                    return 29;// "029";
                case 25:
                    return 35;// "035";
                case 26:
                    return 36;// "036";
                case 27:
                    return 37;// "037";
                case 28:
                    return 94;// "094";
                case 29:
                    return 96;// "096";
                case 30:
                    return 97;// "097";
                case 31:
                    return 98;// "098";
                case 32:
                    return 99;// "099";
                default:
                    return 0;// "000";
            }
        }

        public static string getLNVDiercodeByMinascategory(int pMinascategory)
        {
                /*Agro_labels.Labkind = 260

                0 Rundvee --> 010
                1 Kalkoenen --> 020
                2 Kippen --> 030
                3 Varkens --> 040
                4 Schapen --> 055
                5 Geiten --> 060
                6 Vossen --> 
                7 Nertsen -->
                8 Eenden --> 
                9 Konijnen --> 090
                10 Parelhoenders --> 
                11 Meststoffen
                12 Overige diergroep
                13 Ruwvoer + enkelvoudig voer
                14 Vleeskalveren
                15 Bruine ratten --> 015
                16 Tamme Muizen --> 016
                17 Cavia's --> 017
                18 Goudhamsters --> 018
                19 Gerbils --> 019
                20 Struisvogels --> 025
                21 Emoe's --> 026
                22 Nandoe's --> 027
                23 Knobbelganzen --> 028
                24 Grauwe ganzen --> 029
                25 Fazanten --> 035
                26 Patrijzen --> 036
                27 Vleesduiven --> 037
                28 Paarden --> 094
                29 Ezels --> 096
                30 Midden-Europese Edelherten --> 097
                31 Damherten --> 098
                32 Waterbuffels --> 099
                */
            switch (pMinascategory)
            {
                case 0:
                    return "010";
                case 1:
                    return "020";
                case 2:
                    return "030";
                case 3:
                    return "040";
                case 4:
                    return "055";
                case 5:
                    return "060";
                case 6:
                    return "000";
                case 7:
                    return "000";
                case 8:
                    return "000";
                case 9:
                    return "090";
                case 10:
                    return "000";
                case 11:
                    return "000";
                case 12:
                    return "000";
                case 13:
                    return "000";
                case 14:
                    return "000";
                case 15:
                    return "015";
                case 16:
                    return "016";
                case 17:
                    return "017";
                case 18:
                    return "018";
                case 19:
                    return "019";
                case 20:
                    return "025";
                case 21:
                    return "026";
                case 22:
                    return "027";
                case 23:
                    return "028";
                case 24:
                    return "029";
                case 25:
                    return "035";
                case 26:
                    return "036";
                case 27:
                    return "037";
                case 28:
                    return "094";
                case 29:
                    return "096";
                case 30:
                    return "097";
                case 31:
                    return "098";
                case 32:
                    return "099";
                default:
                    return "000";
            }
        }
   
        public static List<KeyValuePair<string, string>> getLNVDiercodeNamesByMinascategory()
        {
            /*Agro_labels.Labkind = 260

            0 Rundvee --> 010
            1 Kalkoenen --> 020
            2 Kippen --> 030
            3 Varkens --> 040
            4 Schapen --> 055
            5 Geiten --> 060
            6 Vossen --> 
            7 Nertsen -->
            8 Eenden --> 
            9 Konijnen --> 090
            10 Parelhoenders --> 
            11 Meststoffen
            12 Overige diergroep
            13 Ruwvoer + enkelvoudig voer
            14 Vleeskalveren
            15 Bruine ratten --> 015
            16 Tamme Muizen --> 016
            17 Cavia's --> 017
            18 Goudhamsters --> 018
            19 Gerbils --> 019
            20 Struisvogels --> 025
            21 Emoe's --> 026
            22 Nandoe's --> 027
            23 Knobbelganzen --> 028
            24 Grauwe ganzen --> 029
            25 Fazanten --> 035
            26 Patrijzen --> 036
            27 Vleesduiven --> 037
            28 Paarden --> 094
            29 Ezels --> 096
            30 Midden-Europese Edelherten --> 097
            31 Damherten --> 098
            32 Waterbuffels --> 099
            */
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();



            ret.Add(new KeyValuePair<string, string>("010", "Rundvee"));
            ret.Add(new KeyValuePair<string, string>("020", "Kalkoenen"));
            ret.Add(new KeyValuePair<string, string>("030", "Kippen"));
            ret.Add(new KeyValuePair<string, string>("040", "Varkens"));
            ret.Add(new KeyValuePair<string, string>("055", "Schapen"));
            ret.Add(new KeyValuePair<string, string>("060", "Geiten"));
            ret.Add(new KeyValuePair<string, string>("090", "Konijnen"));

            ret.Add(new KeyValuePair<string, string>("015", "Bruine ratten"));
            ret.Add(new KeyValuePair<string, string>("016", "Tamme Muizen"));
            ret.Add(new KeyValuePair<string, string>("017", "Cavias"));
            ret.Add(new KeyValuePair<string, string>("018", "Goudhamsters"));
            ret.Add(new KeyValuePair<string, string>("019", "Gerbils"));
            ret.Add(new KeyValuePair<string, string>("025", "Struisvogels"));
            ret.Add(new KeyValuePair<string, string>("026", "Emoes"));
            ret.Add(new KeyValuePair<string, string>("027", "Nandoes"));
            ret.Add(new KeyValuePair<string, string>("028", "Knobbelganzen"));
            ret.Add(new KeyValuePair<string, string>("029", "Grauwe ganzen"));
            ret.Add(new KeyValuePair<string, string>("035", "Fazanten"));
            ret.Add(new KeyValuePair<string, string>("036", "Patrijzen"));
            ret.Add(new KeyValuePair<string, string>("037", "Vleesduiven"));

            ret.Add(new KeyValuePair<string, string>("094", "Paarden"));
            ret.Add(new KeyValuePair<string, string>("096", "Ezels"));
            ret.Add(new KeyValuePair<string, string>("097", "Midden-Europese Edelherten"));
            ret.Add(new KeyValuePair<string, string>("098", "Damherten"));
            ret.Add(new KeyValuePair<string, string>("099", "Waterbuffels"));

            return ret;


        }
  
        public static void getSanitraceNummers(CORE.DB.DataTypes.THIRD pThird,CORE.DB.DataTypes.UBN pUbn,
            out String Annexnr, out String Inrichtingsnr, out String Beslagnr,out String PENnr)
        {
            // zie ook AfsavetoDB getCompanyBE_ByInrichtingsnr
            //er is ook een THIRD.ThrBeslagnr ?? en een UBN.BRSnummer
            //Als er een

            Inrichtingsnr = "BE";
            Annexnr = pUbn.Extranummer1;

            if (pThird.Thr_Brs_Number.Trim().Length > 0)
            {
                if (pThird.Thr_Brs_Number.ToUpperInvariant().StartsWith("BE"))
                {
                    unLogger.WriteWarn($"utils.getSanitraceNummers - Inrichtingsnummer '{pThird.Thr_Brs_Number.Trim()}' van UBN: '{pUbn.Bedrijfsnummer}' heeft al voorloop 'BE'.");
                    Inrichtingsnr = pThird.Thr_Brs_Number.Trim();
                }                        
                else
                    Inrichtingsnr = "BE" + pThird.Thr_Brs_Number.Trim();
            }
            else if (pUbn.BRSnummer.Trim().Length > 0)
            {
                if (pUbn.BRSnummer.ToUpperInvariant().StartsWith("BE"))
                {

                    unLogger.WriteWarn($"utils.getSanitraceNummers - BRSnummer '{pUbn.BRSnummer.Trim()}' van UBN: '{pUbn.Bedrijfsnummer}' heeft al voorloop 'BE'.");
                    Inrichtingsnr = pUbn.BRSnummer.Trim();
                }
                else
                {
                    Inrichtingsnr = "BE" + pUbn.BRSnummer.Trim();
                }

               
            }
            Beslagnr = Inrichtingsnr + "-" + Annexnr;
            PENnr=pUbn.Bedrijfsnummer + getChecksumPenNumber(pUbn.Bedrijfsnummer);
        }

        private static string getChecksumPenNumber(string pBedrijfsnummer)
        {
            //aanvullen tot 10 cijfers met MOD 97
            //36011076-20 kan schijnbaar ook opgeslagen zijn 
            //maar moet dan 3601107620  zijn
            //20 is dan de MOD 97 waarde
            string ret = pBedrijfsnummer.Replace("-", "");
            if (ret.Length >= 10)
            {
                return ret;
                
            }
            string sMod = "";
            if (ret.Length > 0)
            {
                try
                {
                    Int64 lNumber = Convert.ToInt64(ret);
                    Int64 mod = lNumber % 97;
                    sMod = mod.ToString();
                    int requerdlength = 10 - ret.Length;
                    while (sMod.Length < requerdlength)
                    {
                        sMod = "0" + sMod;
                    }
                }
                catch { }
            }
            return sMod;
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

        public static DataTable GetCsvData(string[] pKolomnamen, string pPathCsvFile, char pSplitter, string pTablename)
        {

            string strLine;
            string[] strArray;
            char[] charArray = new char[] { pSplitter };
            DataSet ds = new DataSet();
            DataTable dt = ds.Tables.Add(pTablename);
            if (File.Exists(pPathCsvFile))
            {
                try
                {
                    if (pKolomnamen.Length > 0)
                    {
                        FileStream aFile = new FileStream(pPathCsvFile, FileMode.Open, FileAccess.Read, FileShare.Write);
                        StreamReader sr = new StreamReader(aFile);

                        try
                        {
                            DataColumn myDataColumn;
                            foreach (string Kolom in pKolomnamen)
                            {
                                myDataColumn = new DataColumn();
                                myDataColumn.DataType = Type.GetType("System.String");
                                myDataColumn.ColumnName = Kolom;
                                dt.Columns.Add(myDataColumn);
                            }
                            if (pPathCsvFile != "")
                            {

                                strLine = sr.ReadLine();
                                while (strLine != null)
                                {
                                    strArray = strLine.Split(charArray);
                                    if (strArray.Length >= pKolomnamen.Length)
                                    {
                                        DataRow dr = dt.NewRow();
                                        for (int i = 0; i < pKolomnamen.Length; i++)// = strArray.GetUpperBound(0); i++)
                                        {
                                            dr[i] = strArray[i].Trim();
                                        }
                                        dt.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        DataRow dr = dt.NewRow();
                                        for (int i = 0; i < strArray.Length; i++)// = strArray.GetUpperBound(0); i++)
                                        {
                                            dr[i] = strArray[i].Trim();
                                        }
                                        dt.Rows.Add(dr);
                                    }
                                    strLine = sr.ReadLine();
                                }
                            }
                        }
                        catch (Exception exc) { unLogger.WriteError("Read csvFile: " + pPathCsvFile + "\r\n" + exc.ToString()); }
                        finally
                        {
                            sr.Close();
                           
                            aFile.Close();
                          
                        }
                    }
                    else
                    {
                        unLogger.WriteError("Read csvFile: het aantal kolommen waarop gezocht moet worden = 0 in \r\n" + pPathCsvFile + "\r\n");
                    }
                }
                catch (Exception exc2) { unLogger.WriteError("Read csvFile: " + pPathCsvFile + "\r\n" + exc2.ToString()); }
            
            }
            return ds.Tables[pTablename];

        }

        public static int getBE_HairColorID(string pSanitraceHairColor)
        {
            /*
                * LabKind = 65
                   1	Rood            R
                   2	Roodwit         RW
                   3	Witrood         WR
                   4	Wit             W
                   5	Witblauw        WBL
                   6	Blauwwit        BLUW
                   7	Witzwart        WZ
                   8	Zwartwit        BLW
                   9	Zwart           BL
                   10	Muisbruin       MBR
                   11	Lichtbruin      LBR
                   12	Donkerbruin     DBR
                   13	Grijs           G
                   99	Overig          OTH
                */
            switch (pSanitraceHairColor)
            {
                case "R":
                    return 1;
                case "RW":
                    return 2;
                case "WR":
                    return 3;
                case "W":
                    return 4;
                case "WBL":
                    return 5;
                case "BLUW":
                    return 6;
                case "WZ":
                    return 7;
                case "BLW":
                    return 8;
                case "BL":
                    return 9;
                case "MBR":
                    return 10;
                case "LBR":
                    return 11;
                case "DBR":
                    return 12;
                case "G":
                    return 13;
                default:
                    return 99;
            }
        }

        public static string getIR_RelatieFile(string pChipnummer,string pVsmRelatieNr, CORE.DB.DataTypes.UBN pHouderUbn, CORE.DB.DataTypes.THIRD pHouderThird, CORE.DB.DataTypes.COUNTRY pHouderCountry, CORE.DB.DataTypes.THIRD pChipperThird, CORE.DB.DataTypes.COUNTRY  pChipperCountry, int pHouderKind, int pChipperKind, string pBaseDir)
        {
            /*
             * Landcode
            verplicht  Bedrijfsnaam, kvkNummer, Naam, Voorletters, AdresRegel1, Huisnr, Staartnaam, postcode, Plaats
             * indien van toepassing
             */
            string createtijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            string retPath = unLogger.getLogDir("CSVData") + pChipnummer + "_" + createtijd + "_RelatieFile.csv";
            string contentHouder = "";
            string contentChipper = "";
            pHouderThird.ThrZipCode = pHouderThird.ThrZipCode.Replace(" ", "").ToUpper();
            pChipperThird.ThrZipCode = pChipperThird.ThrZipCode.Replace(" ", "").ToUpper();
            if (pHouderThird.ThrInitials == "")
            {
                if (pHouderThird.ThrFirstName.Trim().Length > 1)
                {
                    pHouderThird.ThrInitials = pHouderThird.ThrFirstName.Trim().Substring(0, 1);
                }
                if (pHouderThird.ThrInitials == "")
                {
                    if (pHouderThird.ThrCompanyName.Trim().Length > 1)
                    {
                        pHouderThird.ThrInitials = pHouderThird.ThrCompanyName.Trim().Substring(0, 1);
                    }
                }
                if (pHouderThird.ThrInitials == "")
                {
                    if (pHouderThird.ThrSecondName.Trim().Length > 1)
                    {
                        pHouderThird.ThrInitials = pHouderThird.ThrSecondName.Trim().Substring(0, 1);
                    }
                }
                if (pHouderThird.ThrInitials == "")
                {
                    pHouderThird.ThrInitials = "-";//schijnt erg verplicht te zijn dat er iets meekomt. 
                }
            }
            if (pHouderThird.ThrExt.Trim() == "")
            {
                pHouderThird.ThrExt = ExtractHouseNumberFromAdress(pHouderThird.ThrStreet1);
            }
            if (pHouderThird.ThrExt.Trim() != "")
            {
                if (pHouderThird.ThrStreet1.Contains(pHouderThird.ThrExt))
                {
                    try
                    {
                        pHouderThird.ThrStreet1 = pHouderThird.ThrStreet1.Replace(pHouderThird.ThrExt, "");
                    }
                    catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
                }
            }
            string huisnrtoevoeging = "";
            int huisnr = 0;
            if (pHouderThird.ThrExt == "")
            {
                pHouderThird.ThrExt = "1";
            }
            if (!int.TryParse(pHouderThird.ThrExt,out huisnr))
            {
                for (int i = pHouderThird.ThrExt.Length - 1; i > 0; i--)
                {
                    huisnrtoevoeging = pHouderThird.ThrExt[i] + huisnrtoevoeging;
                    pHouderThird.ThrExt = pHouderThird.ThrExt.Remove(i, 1);
                    if (int.TryParse(pHouderThird.ThrExt, out huisnr))
                    {
                        break;
                    }
                }
            }

            switch (pHouderKind)
            { 
                case 1:
                    contentHouder = "HouderN;" + pVsmRelatieNr + ";" + //pHouderUbn.Bedrijfsnummer +
                        ";" + xmlhtmlescape(pHouderThird.ThrSecondName) +
                        ";" +
                        ";" + xmlhtmlescape(pHouderThird.ThrInitials) +
                        ";" + huisnr.ToString() +
                        ";" + xmlhtmlescape(huisnrtoevoeging) +
                        ";" + xmlhtmlescape(pHouderThird.ThrCity) +
                        ";" + pHouderThird.ThrPostBoxnr +
                        ";" + pHouderThird.ThrZipCode +
                        ";" + xmlhtmlescape(pHouderThird.ThrStreet1) + 
                        ";0";
                    break;
                case 2:
                    contentHouder = "HouderNbu;" + pVsmRelatieNr + ";" + //pHouderUbn.Bedrijfsnummer +
                        ";" + xmlhtmlescape(pHouderThird.ThrSecondName) +
                        ";" +
                        ";" + xmlhtmlescape(pHouderThird.ThrInitials) +
                        ";" + xmlhtmlescape(pHouderThird.ThrStreet1) +
                        ";" + xmlhtmlescape(pHouderThird.ThrStreet2) +
                        ";" +
                        ";" + pHouderCountry.LandAfk2.ToUpper() +
                        ";" + xmlhtmlescape(pHouderThird.ThrCity) +
                        ";" + pHouderThird.ThrZipCode;
                    break;
                case 3:
                    contentHouder = "HouderR;" + pVsmRelatieNr + ";" + //pHouderUbn.Bedrijfsnummer +
                        ";" + xmlhtmlescape(pHouderThird.ThrCompanyName) +
                        ";" + pHouderThird.ThrKvkNummer +
                        ";11" +
                        ";" + huisnr.ToString() +
                        ";" + xmlhtmlescape(huisnrtoevoeging) +
                        ";" + xmlhtmlescape(pHouderThird.ThrCity) +
                        ";" + xmlhtmlescape(pHouderThird.ThrPostBoxnr) +
                        ";" + xmlhtmlescape(pHouderThird.ThrZipCode) +
                        ";" + xmlhtmlescape(pHouderThird.ThrStreet1) +
                        ";0";
                    break;
                case 4:
                    contentHouder = "HouderRbu;" + pVsmRelatieNr + ";" + //pHouderUbn.Bedrijfsnummer + 
                        ";" + xmlhtmlescape(pHouderThird.ThrCompanyName) +
                        ";" + pHouderThird.ThrKvkNummer + 
                        ";11" +
                        ";" + xmlhtmlescape(pHouderThird.ThrStreet1) + " " + huisnr.ToString() + 
                        ";" + 
                        ";" +
                        ";" + pHouderCountry.LandAfk2.ToUpper() +
                        ";" + xmlhtmlescape(pHouderThird.ThrCity) +
                        ";" + pHouderThird.ThrZipCode;
                    break;
            }
            if (pChipperThird.ThrSecondName == "")
            {
                pChipperThird.ThrSecondName = pChipperThird.ThrCompanyName;
                if (pChipperThird.ThrSecondName == "")
                {
                    pChipperThird.ThrSecondName = pChipperThird.ThrZipCode + " " + pChipperThird.ThrExt;
                }
            }
            if (pChipperThird.ThrInitials == "")
            {
                if (pChipperThird.ThrFirstName.Trim().Length > 1)
                {
                    pChipperThird.ThrInitials = pChipperThird.ThrFirstName.Trim().Substring(0, 1);
                }
                if (pChipperThird.ThrInitials == "")
                {
                    if (pChipperThird.ThrCompanyName.Trim().Length > 1)
                    {
                        pChipperThird.ThrInitials = pChipperThird.ThrCompanyName.Trim().Substring(0, 1);
                    }
                }
                if (pChipperThird.ThrInitials == "")
                {
                    if (pChipperThird.ThrSecondName.Trim().Length > 1)
                    {
                        pChipperThird.ThrInitials = pChipperThird.ThrSecondName.Trim().Substring(0, 1);
                    }
                }
                if (pChipperThird.ThrInitials == "")
                {
                    pChipperThird.ThrInitials = "A";
                }
            }
            if (pChipperThird.ThrExt == "")
            {
                pChipperThird.ThrExt = ExtractHouseNumberFromAdress(pChipperThird.ThrStreet1);
            }
            if (pChipperThird.ThrExt != "")
            {
                if (pChipperThird.ThrStreet1.Contains(pChipperThird.ThrExt))
                {
                    try
                    {
                        pChipperThird.ThrStreet1 = pChipperThird.ThrStreet1.Replace(pChipperThird.ThrExt, "");
                    }
                    catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
                }
            }
            if (pChipperThird.ThrExt == "")
            { pChipperThird.ThrExt = "1"; }
            
            huisnrtoevoeging = "";
            huisnr = 0;

            if (!int.TryParse(pChipperThird.ThrExt, out huisnr))
            {
                for (int i = pChipperThird.ThrExt.Length - 1; i > 0; i--)
                {
                    huisnrtoevoeging = pChipperThird.ThrExt[i] + huisnrtoevoeging;
                    pChipperThird.ThrExt = pChipperThird.ThrExt.Remove(i, 1);
                    if (int.TryParse(pChipperThird.ThrExt, out huisnr))
                    {
                        break;
                    }
                }
            }
            switch (pChipperKind)
            { 
                case 1:
                    contentChipper = "Chipper;" + xmlhtmlescape(pChipperThird.ThrSecondName) + 
                        ";" +
                        ";" + xmlhtmlescape(pChipperThird.ThrInitials) +
                        ";" + huisnr.ToString() + 
                        ";" + huisnrtoevoeging + 
                        ";" + xmlhtmlescape(pChipperThird.ThrCity) + 
                        ";" + pChipperThird.ThrPostBoxnr + 
                        ";" + pChipperThird.ThrZipCode +
                        ";" + xmlhtmlescape(pChipperThird.ThrStreet1) + 
                        ";0";
                    break;
                case 2:
                    contentChipper = "Chipperbu;" + xmlhtmlescape(pChipperThird.ThrSecondName) +
                        ";" +
                        ";" + xmlhtmlescape(pChipperThird.ThrInitials) +


                        ";" + xmlhtmlescape(pChipperThird.ThrStreet1) + " " + pChipperThird.ThrExt +
                        ";" + 
                        ";" +
                        ";" + pChipperCountry.LandAfk2.ToUpper() +
                        ";" + xmlhtmlescape(pChipperThird.ThrCity) +
                        ";" + pChipperThird.ThrZipCode;
                    break;
            }
            StreamWriter wr = new StreamWriter(retPath, false);
            try
            {
                wr.WriteLine(contentHouder);
                if (pChipperThird.ThrId > 0)
                {
                    wr.WriteLine(contentChipper);
                }
                wr.Flush();
            }
            catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
            finally
            {
                wr.Close();
            }
            /*
                pRelatieFile : is een csv bestand en bevat de gegevens van de Houder en eventuele Chipper. Per bestand maximaal 1 Houder en 1 Chipper regel
                
                voor houder zijn er 4 mogelijke regels voor chipper 2 mogelijke regels:

                Houder = Natuurlijk persoon binnenland (soorthouder = “HouderN”)
                SoortHouder;Klantnr;Registratienr;Naam;Tussenvoegsel;Voorletters;Huisnr;
                HuisnrToevoeging;Plaatsnaam;Postbusnr;Postcode;Straatnaam;wwwbVerwijzing

                Houder = Natuurlijk persoon buitenland (soorthouder = “HouderNbu”)
                Soorthouder;Klantnr;Registratienr;Naam;Tussenvoegsel;Voorletters;AdresRegel1;
                AdresRegel2;AdresRegel3;Landcode;Plaats;Postcode

                Houder = Niet-natuurlijk persoon (rechtspersoon) binnenland (soorthouder = “HouderR”)
                SoortHouder;Klantnr;Registratienr;Bedrijfsnaam;kvkNummer;Rechtsvorm;
                Huisnr;HuisnrToevoeging;Plaatsnaam;Postbusnr;Postcode;Straatnaam;
                wwwbVerwijzing

                Houder = Niet-natuurlijk persoon (rechtspersoon) buitenland (soorthouder = “HouderRbu”)
                SoortHouder;Klantnr;Registratienr;Bedrijfsnaam;kvkNummer;Rechtsvorm;
                AdresRegel1;AdresRegel2;AdresRegel3;Landcode;Plaats;Postcode

                Chipper = Natuurlijk persoon binnenland (soorthouder = “Chipper”)
                SoortChipper;Naam;Tussenvoegsels;Voorletters;Huisnr;HuisnrToevoeging;
                Plaatsnaam;Postbusnr;Postcode;Straatnaam;wwwbVerwijzing
                 
                Chipper = Natuurlijk persoon buitenland (soorthouder = “Chipperbu”)
                SoortChipper;Naam;Tussenvoegsels;Voorletters;AdresRegel1;AdresRegel2;
                AdresRegel3;Landcode;Plaats;Postcode


                Voorbeelden
                HouderR;2229999;999922221;TEST;88887777;2;15;a;Sambeek;51;5455GR;Grotestraat;0
                HouderN;2229999;999922221;Houder;de;G;15;a;Sambeek;51;5455GR;Grotestraat;2

                Betekenis van velden in relatiebestand:
                Klantnr : door aanleveraar (= VSM) toegekend (verplicht)
                Registratienr : ubn van rechtspersoon (optioneel)
                Rechtsvorm (verplicht)
                     1 : Agentschap
                     2 : Besloten_vennootschap
                     3 : Commanditaire_vennootschap
                     4 : Cooperatie
                     5 : De_Staat_Ministerie
                     6 : Eenmanszaak
                     7 : Gemeente
                     8 : Kerkgenootschap
                     9 : Maatschap
                    10 : Naamloze_vennootschap
                    11 : Onbekend
                    12 : Onderlinge_waarborgmaatschappij
                    13 : Openbare_vennootschap
                    14 : Overige_privaatrechtelijke_rechtspersonen
                    15 : Provincie
                    16 : Publiekrechtelijk_rechtspersoon
                    17 : Rechtspersoon_in_oprichting
                    18 : Rechtsvorm_op_basis_van_Europees_recht
                    19 : Rechtsvorm_uit_een_ander_Europees_land
                    20 : Rechtsvorm_uit_het_buitenland__niet_Europa
                    21 : Rederij
                    22 : Staat
                    23 : Stichting
                    24 : Stille_vennootschap
                    25 : Veenschap_veenpolder
                    26 : Vennootschap_onder_firma
                    27 : Vereniging
                    28 : Vereniging_van_eigenaars
                    29 : Waterschap
                    30 : ZBO

                wwwbVerwijzing (verplicht)
                    0 = n.v.t
                    1 = Woonboot
                    2 = Woonwagen

                Landcode (verplicht)
                Standaard 2 letterige landcode

                Overige verplichte velden voor Houder en Chipper (indien van toepassing):
                Bedrijfsnaam, kvkNummer, Naam, Voorletters, AdresRegel1, Huisnr, Staartnaam, postcode, Plaats

             */
            return retPath;
        }

        public static string xmlhtmlescape(string pStringtoEscape)
        {
            return pStringtoEscape.Replace(";",":");
            //UTF8Encoding encoder = new UTF8Encoding();
            //byte[] bytes = Encoding.UTF8.GetBytes(pStringtoEscape); 
            //string utf8ReturnString = encoder.GetString(bytes);
            //return utf8ReturnString;
            //return SecurityElement.Escape(HttpUtility.HtmlEncode(pStringtoEscape));
            //KAN NIET WANT &euro; in een csv ;;; kan niet
        }

        #region programids progids

        public static List<int> getCompanyProgramIds(int pProgramId)
        {
            if (isRietveld(pProgramId))
            { 
                return getRietveldProgramids();
            }
            else if (isNsfo(pProgramId))
            { 
                return getNsfoProgramIds(); 
            }
            else if (isBelTes(pProgramId))
            { 
                return getBelTesProgramIds(); 
            }
            else if (isNFS(pProgramId))
            {
                return getNFS();
            }
            else if (isNijland(pProgramId))
            {
                return getNijlandProgramIds();
            }
            else 
            {
                List<int> p = new List<int>();
                p.Add(pProgramId);
                return p;
            }
        }

        public static List<int> getRietveldProgramids()
        {
            int[] rietvelders = { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69 };
            return rietvelders.ToList();
        }
     
        public static bool isRietveld(int pProgramid)
        {
            if (getRietveldProgramids().Contains(pProgramid))
            { return true; }
            return false;
        }
      
        public static bool isNsfo(int programId)
        {
            if (getNsfoProgramIds().Contains(programId))
            { return true; }
            else { return false; }
       
        }

        public static bool isNFS(int pProgramid)
        {
            return ((pProgramid == 52) || ((pProgramid == 51)));
        }
    
        public static List<int> getNFS()
        {
            int[] tessers = { 51, 52 };
            return tessers.ToList();
        }
   
        public static List<int> getNsfoProgramIds()
        {
            /*
                16-10-2015
                NSFO is zoals bekend een beetje aan de weg aan het timmeren qua stamboeken.
                Vandaar dat we de reeks van ProgramID's gaan uitbreiden :
                Nu zijn dit :
                22 = NSFO admin
                24 - 32 = de oudere stamboeken. (waar er 2 van weg zijn inmiddels)
                33 - Blessum --> is er al langer
                34 - RUI --> is er al een jaartje oid
                35 - Dorset --> voor DK
                40 - NSFO Geit
                47 - DGP Geit
                49 - DGP Schaap
                Lijkt me het handigst dat we 36, 37, 38, 39 er alvast in de source bijzetten.
                (er komt er sowieso dus al 1 bij, dat zal 39 "NSFO - Schaap (Algemeen)" worden)
             */
            /*
               ivm diverse queries is het soms niet wenselijk om 47 en 49 hier toe te voegen
             * ze worden toegevoegd bij de diverse queries 
             * 35 erafgehaald 1-3-2016 doen niet meer mee DK
             */
            int[] l = { 22, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 36, 37, 38, 39, 40 };
            return l.ToList();
        }

        public static bool isBelTes(int programId)
        {
            return ((programId == 16) || (programId == 160));
        }

        public static List<int> getBelTesProgramIds()
        {
            int[] tessers = { 16, 160 };
            return tessers.ToList();
        }

        public static bool isNijland(int pProgramId)
        {
            if (getNijlandProgramIds().Contains(pProgramId))
            { return true; }
            else { return false; }
        }

        public static List<int> getNijlandProgramIds()
        {
            int[] l = { 17000,17099 };
            return l.ToList();
        }

        public static bool isMilkProducer(int pProgId)
        {
            //BUG 1553  en Allerlei.txt op de Samba.sys2/agrobase documenten
            if (pProgId == 1 || pProgId == 3 || pProgId == 4 || pProgId == 5)
            {
                return true;
            }
            else { return false; }
        }

        #endregion

        public static string getBedrijfstypeLNV(string pLNVType)
        {
            if (pLNVType == null) { return pLNVType; }
            switch (pLNVType)
            {
                case "DT":
                    return "Destructor";
                case "ET":
                    return "Evenemententerrein";
                case "ML":
                    return "Merkleverancier";
                case "SP":
                    return "Slachtplaats";
                case "VH":
                    return "Veehouderij";
                case "VP":
                    return "Verzamelplaats";
            }
            return pLNVType;

        }

        public static string getLNVBedrijfstypeCode(int pAgrobaseLabId)
        {

            switch (pAgrobaseLabId)
            {
                case 3:
                    return "DT";
                case 10:
                    return "SP";
                case 9:
                case 11:
                case 12: 
                case 14:
                case 15:
                    return "VH";
            }
            return "";

        }

        public static string getLNVredeneinde(string pLNVCode)
        { 
             /*       RedenEinde
            EX = export
            GI = Geboorte ingetrokken
            II = Import ingetrokken
            ND = Natuurlijke dood
            RM = Ruiming
            SL = Slacht
            VM = Vermissing
            */
            switch (pLNVCode)
            {
                case "EX":
                    return "Export";
                case "GI":
                    return "Geboorte ingetrokken";
                case "II":
                    return "Import ingetrokken";
                case "ND":
                    return "Natuurlijke dood";
                case "RM":
                    return "Ruiming";
                case "SL":
                    return "Slacht";
                case "VM":
                    return "Vermissing";

                
            }
            return pLNVCode;
        }

        public static string getTRANSMITKoppelingText(int pKoppelnr)
        {
            switch (pKoppelnr)
            {
                case 99:
                    return "Weegcomp";
                case 97:
                    return "Handterm";
                case 98:
                    return "AL";
                default:
                    if (pKoppelnr > 9700 && pKoppelnr < 9800)
                    {
                        return "Handterm";
                    }
                    if (pKoppelnr > 9800 && pKoppelnr < 9900)
                    {
                        return "AL";
                    }
                    if (pKoppelnr > 9900 && pKoppelnr < 10000)
                    {
                        return "Weegcomp";
                    }
                    return pKoppelnr.ToString();
            }
        }

        public enum procesComputerReferenceNumber
        {
            Agrobase = 1001,
            Ruma = 1002,
            Generic = 1101,
            Fusion = 1201,
            Velos = 1403,
            VC5 = 1401,
            VeeCode = 1402,
            Delpro = 1501,
            ID2000 = 1502,
            Dairyplan = 1601,
            Rovecom = 1701,
            CRVVoerAdvies = 1302,
            Lely = 1801,
            Alpro45 = 1901,
            Alpro6 = 1902,
            Taurus = 2000,
            TaurusMS = 2010,
            BMI = 2101,
            SAC = 2102,
            DairyMaster = 2201,
            MSOptimabox = 2301,
        }
 
        public static List<KeyValuePair<int, string>> getprocescomputerdata()
        {

            List<KeyValuePair<int, string>> ret = new List<KeyValuePair<int, string>>();

            //List<LABELS> lbls = aFSaveToDBObj.GetLabels(40, 528);
            foreach (CORE.utils.procesComputerReferenceNumber nr in Enum.GetValues(typeof(CORE.utils.procesComputerReferenceNumber)))
            {
                
                KeyValuePair<int, string> r = new KeyValuePair<int, string>((int)nr, nr.ToString());
                ret.Add(r);


            }
            //int[] doubleids = { 3, 4, 6, 8, 14, 15, 17, 18, 21, 22, 23, 25, 26, 27, 28, 29 };//zie mail van luc
            //foreach (LABELS lbl in lbls)
            //{
            //    enumgrid gr = new enumgrid();
            //    gr.IdField = lbl.LabId;
            //    gr.Type = lbl.LabLabel;
            //    gr.Name = "";
            //    if (!doubleids.Contains(gr.IdField))
            //    {
            //        var checkerin = from n in gridenum where n.IdField == gr.IdField select n;
            //        if (checkerin.Count() == 0)
            //        {
            //            gridenum.Add(gr);
            //        }
            //    }

            //}
            for (int i = 97; i < 100; i++)
            {
                string naam = CORE.utils.getTRANSMITKoppelingText(i);
             
                KeyValuePair<int, string> r = new KeyValuePair<int, string>(i, naam);
                ret.Add(r);
            }

            return ret;
        }

        public enum DcfAniTypeCode
        {
            Runderen = 12,//Kvæg = Vee oftewel Cattle
            Schapen = 13, //Schapen (via derk vink)
            Geiten = 14,
            Onbekend = 0,
        }
 
        public static int getDcfAniTypeCode(int pProgId)
        {
            if (pProgId == 3 || pProgId == 5)
            {
                if (pProgId == 3)
                {
                    return (int)DcfAniTypeCode.Schapen;
                }
                else
                {
                    return (int)DcfAniTypeCode.Geiten;
                }
            }
            else
            {
                return (int)DcfAniTypeCode.Runderen;
            }
            /*
             Ik kan het nergens vinden maar, (alleen 12 in de pdf )
                Zoek zelf eens op https://accepttest.dcf.ws.dlbr.dk/DLBR.DCF.Documentation/ExternalDocs/index.aspx
                User: prk2252
                PW: derkvink
             */
        }
   
        public static void setDekperiode(out DateTime pBegindatum, out DateTime pEindDatum)
        {
            /*
             * De dekperiode loopt van 1 aug 2010 tot 31 juli 2011.
        
             */

            DateTime Datum = DateTime.Now;

            int beginDay = 1;
            int beginMomth = 7;
            int endDay = 30;
            int endMonth = 6;

            int beginYear;
            int endYear;

            if (Datum.Date.Month > 7)
            {
                beginYear = Datum.Year;
                endYear = (Datum.Year + 1);
            }
            else
            {
                beginYear = (Datum.Year - 1);
                endYear = Datum.Year;
            }

            pBegindatum = new DateTime(beginYear, beginMomth, beginDay);
            pEindDatum = new DateTime(endYear, endMonth, endDay);
     
        }

        public static bool isIntegerNumber(string number)
        {
            int dummyInt;
            if (Int32.TryParse(number, out dummyInt))
            { return true; }
            else
            { return false; }
        }

        public static string getOormerkFabrikant(string pCode)
        {
            switch (pCode)
            {
                case "A":
                    return "Allflex";

                case "C":
                    return "Chevillot";
                case "D":
                    return "Daploma";
                case "Y":
                    return "y-tex";
                case "Q":
                    return "Q-Flex";
                case "P":
                    return "Schippers";
                case "M":
                    return "Metagam";
                case "G":
                    return "Os";
                case "X":
                    return "Reyflex France";
                case "N":
                    return "Dalton";
                case "E":
                    return "Dalton (elektronisch)";
                case "S":
                    return "Caisley";
                default: return "Onbekend";
            }
        }

        public static System.Xml.Linq.XDocument getConfigDoc(string pFolder,string naamdotxml)
        {
            string bestand = unLogger.getLogDir() + Path.DirectorySeparatorChar + pFolder + Path.DirectorySeparatorChar + naamdotxml;
          
            if(pFolder=="")
            {
                bestand = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + naamdotxml;
            }
        
            if (!File.Exists(bestand))
            {
                DirectoryInfo dir = new DirectoryInfo(unLogger.getLogDir());
                if (dir.Exists)
                {
                    DirectoryInfo dirparent = dir.Parent;
                    if (dirparent.Exists)
                    {
                        bestand = dirparent.FullName + Path.DirectorySeparatorChar + pFolder + Path.DirectorySeparatorChar + naamdotxml;

                        if (pFolder == "")
                        {
                            bestand = dirparent.FullName + Path.DirectorySeparatorChar + naamdotxml;
                        }
                    }
                }
                unLogger.WriteInfo("ConfigDoc1=" + bestand);
                if (!File.Exists(bestand))
                {
                    System.Xml.Linq.XDocument xd = new System.Xml.Linq.XDocument();
                    if (naamdotxml == "berichtenbestand.xml")
                    {
                        System.Xml.Linq.XElement root = new System.Xml.Linq.XElement("companies");
                        xd.Add(root);
                    }
                }
            }
            unLogger.WriteInfo("ConfigDoc2=" + bestand);
            try
            {
                StreamReader strReader = new StreamReader(bestand);
                System.Xml.Linq.XDocument document = new System.Xml.Linq.XDocument();
                try
                {
                    document = System.Xml.Linq.XDocument.Load(strReader, System.Xml.Linq.LoadOptions.None);
                }
                catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
                finally { strReader.Close(); }
                return document;
            }
            catch (Exception exc) { unLogger.WriteError(exc.ToString()); }

            return null;
        }

        public static string getRightsElementValue(System.Xml.Linq.XDocument pxDoc,int pProgramid, string pElementName)
        {
            string ret = "";
            if (pxDoc != null)
            {
                if (pProgramid > 0)
                {
                    pElementName = pElementName.Trim();
                    if (pElementName != "")
                    {
                        try
                        {
                            XElement xEle = pxDoc.XPathSelectElement("//ProgramId[@ProgramId=" + pProgramid.ToString() + "]");
                            if (xEle != null)
                            {
                                if (xEle.Element(pElementName) != null)
                                {
                                    ret = xEle.Element(pElementName).Value;
                                }
                            }
                            if (ret == "")
                            {
                                XElement xElepElementName = pxDoc.Root.Element(pElementName);
                                if (xElepElementName != null)
                                {
                                    ret = xElepElementName.Value;
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            return ret;
        }

        public static void MissingDCFRace(AGRO_LABELS lbl)
        {
            try
            {
                string bestand = Path.Combine(unLogger.getLogDir(), "MissingDCFRaces.txt");
                char[] spl = { '-' };
                if (!string.IsNullOrWhiteSpace(lbl.LabLabel))
                {
                    bool addlabel = true;
                    string[] splits = lbl.LabLabel.Split(spl);
                   
                  
                    if (File.Exists(bestand))
                    {
                        using (StreamReader reader = new StreamReader(bestand))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (splits.Count() == 3)
                                {
                                    if (line.Contains(splits[2]))
                                    {
                                        addlabel = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                      
                    }
                    if (addlabel)
                    {
                        using (StreamWriter wrtr = new StreamWriter(bestand, true))
                        {
                            if (splits.Count() == 3)
                            {
                                wrtr.WriteLine($@"ProgID:{lbl.LabProgID} DCF Code: {splits[0]} , DCF Afkorting: {splits[1]} , DCF Omschrijving: {splits[2]} ");
                            }
                           
                             
                        }
                    }
                }
            }
            catch(Exception exc) { unLogger.WriteError($@"unLogger.getLogDir():{unLogger.getLogDir()} :" + exc.ToString()); }
        }

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }


}
