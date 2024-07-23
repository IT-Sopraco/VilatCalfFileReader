using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Linq;

namespace VSM.RUMA.CORE
{
    public class Lijsten
    {

        private int pCountryCode { get; set; }

        public bool getStayNotLocal(int programId, string soort)
        {

            //return false;//TEST zodat met livedata getest kan worden (zie ook lijstenservice.asmx.cs)
            bool staynotlocal = true; //default

            if ((programId == 90) //forfarmers (ivm. doorgeven xml bestand aan kopreslijstmulti)
                //|| (soort == "BloedonderzoekAuthorisatie") //local ivm. te verzenden mail, kan op termijn naar lijstenservice soap 
                )
            {
               //staynotlocal = false;
            }

            return staynotlocal;
        }

        public delegate void pCallback(int PercDone, string Msg);

        private enum ListFileTypes { PDF = 0, Excel = 1, TXT = 2, RTF = 3, JPG = 4, XML = 5, CSV = 6 };

        private string getExt(int filetype)
        {
            string ext = "";

            switch (filetype)
            {
                case (int)ListFileTypes.PDF: ext = ".pdf";
                    break;

                case (int)ListFileTypes.Excel: ext = ".xls";
                    break;

                case (int)ListFileTypes.TXT: ext = ".txt";
                    break;

                case (int)ListFileTypes.RTF: ext = ".rtf";
                    break;

                case (int)ListFileTypes.JPG: ext = ".jpg";
                    break;

                case (int)ListFileTypes.XML: ext = ".xml";
                    break;
                case (int)ListFileTypes.CSV: ext = ".csv";
                    break;
            }

            return ext;
        }

        private int getAniIdFromStringOrUser(string strAniId, int gebruikerAnimalAniId)
        {
            int aniId = 0;

            if ((strAniId != "") && (strAniId != "null"))
            {
                int.TryParse(strAniId, out aniId);
            }
            if (aniId == 0)
            {
                try
                {
                    aniId = gebruikerAnimalAniId;
                }
                catch (Exception exc)
                {
                    unLogger.WriteError(exc.ToString());
                }
            }

            return aniId;
        }
    
        public int MaakLijst(
           DBConnectionToken conToken, string DBhost, string AgroUser, string AgroPassword,
           int pUserFarmId, int pUserThirdId, int pLijstenLabId,
           int UbnId, int pProgId, int pProgramId,
           string soort, DateTime begindatum, DateTime einddatum, string addons, string outputDir,
           string logDir, string fileNameWithoutExt, string serverMainDir,
           pCallback EventCallbackmethod,
           bool isAdmin, int gebruikerAnimalAniId, bool pTestDB,
           out string filePath, int _pCountryCode)
        {
            pCountryCode = _pCountryCode;
            if (pCountryCode == 0) { pCountryCode = 528; }
            return MaakLijst( conToken,DBhost, AgroUser,  AgroPassword,
             pUserFarmId, pUserThirdId,  pLijstenLabId,
             UbnId,  pProgId, pProgramId,
             soort,  begindatum, einddatum,  addons,  outputDir,
             logDir,  fileNameWithoutExt,  serverMainDir,
             EventCallbackmethod,
             isAdmin,  gebruikerAnimalAniId, pTestDB,
            out  filePath);
        }

        public int MaakLijst(
            DBConnectionToken conToken, string DBhost, string AgroUser, string AgroPassword,
            int pUserFarmId, int pUserThirdId, int pLijstenLabId,
            int UbnId, int pProgId, int pProgramId,
            string soort, DateTime begindatum, DateTime einddatum, string addons, string outputDir,
            string logDir, string fileNameWithoutExt, string serverMainDir,
            pCallback EventCallbackmethod,
            bool isAdmin, int gebruikerAnimalAniId, bool pTestDB,
            out string filePath)
        {
            //default initializations
            filePath = "";
            int result = 999;
            int filetype = 0;
            unLogger.WriteInfo(" Lijst created with class lijsten.MaakLijst ");
            string logfilePath = logDir + fileNameWithoutExt + getExt(2); //log=txt
            string pResourceFolder = logDir.Replace("log", "lib");
            string pUbnnr = "0";
            //AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(conToken);
            DB.DBMasterQueries lMstb = new CORE.DB.DBMasterQueries(conToken);
            //if (DBhost == "" && (pProgramId==18000 || pProgramId==18099)) { DBhost = "217.148.183.77"; } 
            UBN u = lMstb.GetubnById(UbnId);
            if (u != null) { pUbnnr = u.Bedrijfsnummer; }

            //LAAD ADDONS IN ARRAY tmp
            char[] dl = { '-' };
            if ((soort == "BloedonderzoekAuthorisatie") ||
                (soort == "kopreslijst") ||
                (soort == "kopreslijstmulti"))
            {
                char[] dlA = { '|' };
                dl = dlA;
            }
            if (pCountryCode <= 0)
            {
                pCountryCode = 528;
            }
            string[] addonArr = addons.Split(dl);
            //int lCountryCode = 0;
            //try
            //{
            //    int.TryParse(addonArr[addonArr.Length-1], out lCountryCode);
            //}
            //catch { }
            COUNTRY cntry = lMstb.GetCountryByLandNummer(pCountryCode);
            string landcode = cntry.LandAfk2;
            int pLandnr = 528;
            int pLandid = 0;
            THIRD t = lMstb.GetThirdByThirId(u.ThrID);
            int.TryParse(t.ThrCountry, out pLandid);
            if (pLandid > 0)
            {
                cntry = lMstb.GetCountryByLandid(pLandid);
                pLandnr = cntry.LandNummer;
            }

            unLogger.WriteInfo("ProgId:" + pProgId.ToString() + "- ProgramId:" + pProgramId.ToString() + "-AgroUser:" + AgroUser );
            unLogger.WriteInfo($@"opvragen lijst:{soort }-{u.Bedrijfsnaam}-{ u.Bedrijfsnummer}- dbhost:{DBhost}");
            //unLogger.WriteInfo($@"conToken.MasterIP:{conToken.MasterIP }- conToken.SlaveIP:{conToken.SlaveIP} getLastChildConnection().MasterIP:{conToken.getLastChildConnection().MasterIP}");

            if (pUbnnr != "0")
            {
                switch (soort)
                {
                    case "stallijstrund":
                        try
                        {
                            DateTime beginl = begindatum;

                            Win32stallijst.pCallback doConversion = new Win32stallijst.pCallback(EventCallbackmethod);

                            int medeeig = 0; //medeeigenaar ? 1 : 0;

                            int groupid = Convert.ToInt32(addonArr[0]);
                            int sorton = Convert.ToInt32(addonArr[1]);

                            filetype = Convert.ToInt32(addonArr[2]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pToonKeto = 0;
                            int.TryParse(addonArr[3], out pToonKeto);
                            int pAlleenAfgekalfd = 0;
                            int.TryParse(addonArr[4], out pAlleenAfgekalfd);
                            
                            Win32stallijst DLLstallijst = new Win32stallijst();

                            unLogger.WriteInfo("DLLstallijst  parameters(" + pProgId.ToString() + "," + pProgramId.ToString() + "," +
                                                         filetype.ToString() + "," + pUbnnr.ToString() + "," + DBhost +
                                                         "," + AgroUser + "," + AgroPassword +
                                                         "," + filePath + "," + logfilePath + ",doConversion," +
                                                         beginl.ToString(), medeeig.ToString() + "," + groupid.ToString() + "," + sorton.ToString() + ")"); ;

                            result = DLLstallijst.call_lst_AB_RundveeStallijst
                                                        (pProgId, pProgramId,
                                                         filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                         filePath, logfilePath,pResourceFolder,landcode, pCountryCode, doConversion,
                                                         beginl, medeeig, groupid, sorton, pToonKeto, pAlleenAfgekalfd);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("call_lst_AB_RundveeStallijst error ", exc);
                        }
                        break;
                    case "stallijstrundketo":
                        try
                        {
                            DateTime beginl = begindatum;

                            Win32stallijst.pCallback doConversion = new Win32stallijst.pCallback(EventCallbackmethod);

                            int medeeig = 0; //medeeigenaar ? 1 : 0;

                            int groupid = Convert.ToInt32(addonArr[0]);
                            int sorton = Convert.ToInt32(addonArr[1]);

                            filetype = Convert.ToInt32(addonArr[2]);

                            int pToonKeto = Convert.ToInt32(addonArr[3]);
                            //int lCountryCode = 0;
                            //try
                            //{
                            //    int.TryParse(addonArr[5], out lCountryCode);
                            //}
                            //catch { }
                            //string landcode = lMstb.GetCountryByLandNummer(lCountryCode).LandAfk2;
                            int pAlleenAfgekalfd = 0;

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32stallijst DLLstallijst = new Win32stallijst();
                            unLogger.WriteInfo("DLLstallijst  parameters(" + pProgId.ToString() + "," + pProgramId.ToString() + "," +
                                                         filetype.ToString() + "," + pUbnnr.ToString() + "," + DBhost +
                                                         "," + AgroUser + "," + AgroPassword +
                                                         "," + filePath + "," + logfilePath + ",doConversion," +
                                                         beginl.ToString(), medeeig.ToString() + "," + groupid.ToString() + "," + sorton.ToString() + ")"); ;
                            result = DLLstallijst.call_lst_AB_RundveeStallijst
                                                        (pProgId, pProgramId,
                                                         filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                         filePath, logfilePath,pResourceFolder,landcode,pCountryCode, doConversion,
                                                         beginl, medeeig, groupid, sorton, pToonKeto, pAlleenAfgekalfd);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("call_lst_AB_RundveeStallijst error ", exc);
                        }
                        break;
                    case "stallijstrundafgekalfd":
                        try
                        {
                            DateTime beginl = begindatum;

                            Win32stallijst.pCallback doConversion = new Win32stallijst.pCallback(EventCallbackmethod);

                            int medeeig = 0; //medeeigenaar ? 1 : 0;

                            int groupid = Convert.ToInt32(addonArr[0]);
                            int sorton = Convert.ToInt32(addonArr[1]);

                            filetype = Convert.ToInt32(addonArr[2]);

                            //int lCountryCode = 0;
                            //try
                            //{
                            //    int.TryParse(addonArr[5], out lCountryCode);
                            //}
                            //catch { }
                            //string landcode = lMstb.GetCountryByLandNummer(lCountryCode).LandAfk2;

                            int pToonKeto = 0;// Convert.ToInt32(addonArr[3]); //anders ziet ie er hetzefde uit als stallijstrundketo

                            int pAlleenAfgekalfd = 1;

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32stallijst DLLstallijst = new Win32stallijst();
                            unLogger.WriteInfo("DLLstallijst  parameters(" + pProgId.ToString() + "," + pProgramId.ToString() + "," +
                                                         filetype.ToString() + "," + pUbnnr.ToString() + "," + DBhost +
                                                         "," + AgroUser + "," + AgroPassword +
                                                         "," + filePath + "," + logfilePath + ",doConversion," +
                                                         beginl.ToString(), medeeig.ToString() + "," + groupid.ToString() + "," + sorton.ToString() + ")"); ;
                            result = DLLstallijst.call_lst_AB_RundveeStallijst
                                                        (pProgId, pProgramId,
                                                         filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                         filePath, logfilePath, pResourceFolder,landcode,pCountryCode, doConversion,
                                                         beginl, medeeig, groupid, sorton, pToonKeto, pAlleenAfgekalfd);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("call_lst_AB_RundveeStallijst error ", exc);
                        }
                        break;
                    case "Identificatielijstgeit":
                        try
                        {
                            DateTime pDatum = begindatum;

                            Win32geitenid.pCallback doConversion = new Win32geitenid.pCallback(EventCallbackmethod);

                            //UITZONDERING // usually used '-' but caused a conflict with date format xx-xx-xxxx
                            char[] tmpSplit = { '+' };
                            string[] tmp = addons.Split(tmpSplit);
                            /*
                             pSoortlijst
                                 0 = Alle Dieren
                                 1 = Alle Melkgeiten
                                 2 = Alle Jonge geitjes
                                 3 = Alle Bokken op bedrijf
                                 4 = Alle Dekbokken
                                 5 = Dieren geboren vanaf:
                                 6 = Aanwezige dieren op:

                                pWeergaveOuders/pWeergaveBeknopt
                                   0 = Naam
                                   1 = Levensnr

                                pSortering
                                  0 = Diernummer
                                  1 = Naam
                                  2 = Levensnummer
                                  3 = Hoornfactor
                                  4 = Laktatienr
                                  5 = Ras
                                  6 = Geboortedatum
                                  7 = Afvoerdatum
                                  8 = Vader
                                  9 = Vadersvader
                                  10 = Moeder
                                  11 = Moedersmoeder
                                  12 = Geslacht
                             */
                            /*
                             addons = pSoortlijst + "+" + pAlleenAanwezig + "+" + pBeknopt + "+" + 
                             *        pWeergaveBeknopt + "+" + pSortering + "+" +
                                      pPerHok + "+" + pAlleenHok + "+" + pHoknr + "+" + fileformat
                             */
                            int pSoortlijst = Convert.ToInt32(tmp[0]);
                            bool pAlleenAanwezig = Convert.ToBoolean(tmp[1]);
                            bool pBeknopt = Convert.ToBoolean(tmp[2]);
                            int pWeergaveBeknopt = Convert.ToInt32(tmp[3]);
                            int pSortering = Convert.ToInt32(tmp[4]);

                            int pWeergaveOuders = 0;// Convert.ToInt32(tmp[1]);

                            bool pExtraGegevens = false;// Convert.ToBoolean(tmp[5]);
                            bool pResponders = false;// Convert.ToBoolean(tmp[6]);

                            bool pPerHok = false;
                            bool pAlleenHok = false;
                            string pHoknr = "";

                            try
                            {
                                pPerHok = Convert.ToBoolean(tmp[5]);
                                pAlleenHok = Convert.ToBoolean(tmp[6]);
                                pHoknr = tmp[7];

                            }
                            catch { }




                            filetype = Convert.ToInt32(tmp[8]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32geitenid DLLstallijst = new Win32geitenid();

                            result = DLLstallijst.call_lst_AB_GeitenIdlijst
                                                        (pProgId, pProgramId,
                                                         filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                         filePath, logfilePath, doConversion,
                                                         pSoortlijst, pWeergaveOuders, pWeergaveBeknopt,
                                                         pDatum, pAlleenAanwezig, pBeknopt, pExtraGegevens,
                                                         pResponders, pPerHok, pAlleenHok, pHoknr, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("call_lst_AB_GeitenIdlijst error ", exc);
                        }
                        break;
                    case "stallijstgeit":
                    case "stallijstschaap":
                        try
                        {
                            DateTime beginl = begindatum;

                            Win32stallijst.pCallback doConversion = new Win32stallijst.pCallback(EventCallbackmethod);

                            //UITZONDERING // usually used '-' but caused a conflict with date format xx-xx-xxxx
                            char[] tmpSplit = { '+' };
                            string[] tmp = addons.Split(tmpSplit);

                            DateTime date_van = tmp[0].Trim() != "" ? Convert.ToDateTime(tmp[0]) : DateTime.MinValue;
                            DateTime date_tot = tmp[1].Trim() != "" ? Convert.ToDateTime(tmp[1]) : DateTime.MinValue;
                            int medeeig = Convert.ToBoolean(tmp[2]) ? 1 : 0;
                            int soortlijst = Convert.ToBoolean(tmp[3]) ? 1 : 0;//vadermoeder
                            int sorton = Convert.ToInt32(tmp[4]);
                            int scrapie = Convert.ToInt32(tmp[5]);
                            bool alles = Convert.ToBoolean(tmp[6]);
                            bool ooien = Convert.ToBoolean(tmp[7]);
                            bool rammen = Convert.ToBoolean(tmp[8]);
                            int showAniAlternateNumber = Convert.ToBoolean(tmp[10]) ? 1 : 0;

                            int anisex = 0;
                            if (alles) { anisex = 0; }
                            else if (rammen) { anisex = 1; }
                            else if (ooien) { anisex = 2; }

                            bool pPerHok = false;
                            bool pAlleenHok = false;
                            string pHoknr = "";
                            filetype = Convert.ToInt32(tmp[9]);
                            try
                            {
                                pPerHok = Convert.ToBoolean(tmp[11]);
                                pAlleenHok = Convert.ToBoolean(tmp[12]);
                                pHoknr = tmp[13];

                            }
                            catch 
                            {
                                try
                                {
                                    //VOOR COLLECTIEVE LIJSTEN
                                    //char[] s = {'-'};
                                    //pUbnnr = tmp[11].Split(s)[0];
                                    //UBN lUbn = lMstb.getUBNByBedrijfsnummer(pUbnnr);
                                    //List<BEDRIJF> bedrijven = lMstb.getBedrijvenByUBNId(lUbn.UBNid);
                                    //var ch = from n in bedrijven where n.ProgId==pProgId select n;
                                    //if (ch.Count() > 0)
                                    //{
                                    //    StringBuilder b = new StringBuilder();
                                    //    foreach (BEDRIJF r in ch)
                                    //    {
                                    //        b.AppendLine($@"stallijst bedrijf:Farmid:{r.FarmId} ProgId:{r.ProgId} Programid:{r.Programid}");
                                    //    }
                                    //    unLogger.WriteInfo(b.ToString());
                                    //    pProgramId = ch.ElementAt(0).Programid;
                                    //}

                                }
                                catch { }
                            }
                            int pLevnrNaam = 0;
                            try { pLevnrNaam = Convert.ToInt32(tmp[14]); }
                            catch { }
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            // 1-1-0001 0:00:00+1-1-0001 0:00:00+False+False+0+0+True+False+False+0+False
                            Win32stallijst DLLstallijst = new Win32stallijst();

                            result = DLLstallijst.call_lst_AB_SchapenStallijst
                                                        (pProgId, pProgramId,
                                                         filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                         filePath, logfilePath,pResourceFolder, landcode ,pCountryCode, doConversion,
                                                         beginl, medeeig, anisex, scrapie, soortlijst, sorton, showAniAlternateNumber,
                                                         date_van, date_tot,
                                                         pPerHok, pAlleenHok, pHoknr, pLevnrNaam);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("call_lst_AB_SchapenStallijst error ", exc);
                        }
                        break;
                    case "bedrijfsregisterrund":
                        try
                        {
                            DateTime beginl = begindatum;
                            DateTime eindl = einddatum;

                            Win32BEDREGIS.pCallback doConversion = new Win32BEDREGIS.pCallback(EventCallbackmethod);

                            filetype = Convert.ToInt32(addonArr[0]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            /*
                               Volgorde:
                               filetype, pCompact, pGroeppnr, pAanvoerGroep, pStal, pAfdeling: 
                               pLand, pIncclAanwInPeriode, pSortering 
                            */
                            int pCompact = int.Parse(addonArr[1].ToString());
                            int pGroeppnr = int.Parse(addonArr[2].ToString());
                            int pAanvoerGroep = 0;
                            if (addonArr[3].ToString().ToLower() == "true")
                            {
                                pAanvoerGroep = 1;
                            }
                            int pStal = int.Parse(addonArr[4].ToString());
                            int pAfdeling = int.Parse(addonArr[5].ToString());

                            int pLand = int.Parse(addonArr[6].ToString());
                            var pIncclAanwInPeriode = 0;
                            if (addonArr[7].ToString() == "true")
                            { pIncclAanwInPeriode = 1; }
                            var pSortering = int.Parse(addonArr[8].ToString());

                        
                            unLogger.WriteInfo("DLLBEDREGIS bedrijfsregisterrund parameters(" + pProgId.ToString() + "," + pProgramId.ToString() + "," +
                                                                                    filetype.ToString() + "," + pUbnnr.ToString() + "," + DBhost +
                                                                                    "," + AgroUser + "," + AgroPassword +
                                                                                    "," + filePath + "," + logfilePath + ",doConversion," +
                                                                                     pCompact.ToString() + "," + pGroeppnr.ToString() + "," + pAanvoerGroep.ToString() + "," +
                                                                  pStal.ToString() + "," + pAfdeling.ToString() + "," +
                                                                                    beginl.ToString().ToString() + "," + eindl.ToString() + "," + pLand.ToString() + "," + pIncclAanwInPeriode.ToString() + "," + pSortering.ToString() + ")");

                            result = Win32BEDREGIS.Init().call_lst_AB_bedrijfsregister(
                                                                  pProgId, pProgramId,
                                                                  filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                  filePath, logfilePath, doConversion,
                                                                  pCompact, pGroeppnr, pAanvoerGroep,
                                                                  pStal, pAfdeling,
                                                                  beginl, eindl, pLand, pIncclAanwInPeriode, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Bedrijfsregister rund error ", exc);
                        }
                        break;
                    case "bedrijfsregister":
                        try
                        {
                            DateTime beginl = begindatum;
                            DateTime eindl = einddatum;

                            win32lstabbedreg.pCallback doConversion = new win32lstabbedreg.pCallback(EventCallbackmethod);

                            filetype = Convert.ToInt32(addonArr[0]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = win32lstabbedreg.Init().call_lst_AB_Bedrijfsregister(
                                                                  pProgId, pProgramId,
                                                                  filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                  filePath, logfilePath, doConversion,
                                                                  beginl, eindl);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Bedrijfsregister rund error ", exc);
                        }
                        break;

                    case "bedrregsg":
                        try
                        {
                            DateTime beginl = begindatum;
                            DateTime eindl = einddatum;

                            Win32BEDREGIS.pCallback doConversion = new Win32BEDREGIS.pCallback(EventCallbackmethod);

                            bool MutatiesOptellen = Convert.ToBoolean(addonArr[0]);
                            filetype = Convert.ToInt32(addonArr[1]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32BEDREGIS.Init().call_lst_AB_bedrijfsregisterSG(
                                                                    pProgId, pProgramId,
                                                                    filetype, pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                    filePath, logfilePath, doConversion,
                                                                    beginl, eindl, MutatiesOptellen);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Bedrijfsregister schaap/geit error ", exc);
                        }
                        break;

                    case "deklijst":
                        try
                        {
                            filetype = 0;
                            //if (addonArr.Length == 1)
                            //{
                            //    char[] spl = { '-', '+' };
                            //    addonArr = addons.Split(spl);
                            //}

                            int.TryParse(addonArr[0], out filetype);

                            int pSort = 0;
                            int.TryParse(addonArr[1], out pSort);

                            bool lEmptydeklijst = Convert.ToBoolean(addonArr[2]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            try
                            {
                                //VOOR COLLECTIEVELIJSTEN.aspx
                                //if (addonArr.Length >= 4)//LET OP  Countrycode is er achteraangeplakt
                                //{
                                   
                                //    pUbnnr = addonArr[3];
                                //    UBN lUbn = lMstb.getUBNByBedrijfsnummer(pUbnnr);
                                //    List<BEDRIJF> bedrijven = lMstb.getBedrijvenByUBNId(lUbn.UBNid);
                                //    var ch = from n in bedrijven where n.ProgId==pProgId select n;
                                //    if (ch.Count() > 0)
                                //    {
                                //        StringBuilder b = new StringBuilder();
                                //        foreach (BEDRIJF r in ch)
                                //        {
                                //            b.AppendLine($@"deklijst bedrijf:Farmid:{r.FarmId} ProgId:{r.ProgId} Programid:{r.Programid}");
                                //        }
                                //        unLogger.WriteInfo(b.ToString());
                                //        pProgramId = ch.ElementAt(0).Programid;
                                //    }
                                //}

                            }
                            catch { }

                            if (lEmptydeklijst)
                            {
                                if (addonArr.Length >= 4)//LET OP  Countrycode is er achteraangeplakt
                                {
                                    pUbnnr = addonArr[3];
                                }
                                Win32AANWEOOI.pCallback doConversion = new Win32AANWEOOI.pCallback(EventCallbackmethod);
                                unLogger.WriteInfo($@"Win32Deklijst call_lst_AB_Deklijsten lEmptydeklijst : pProgId:{pProgId}, pProgramId:{pProgramId}, filetype:{filetype},
                                                                         pUbnnr:{pUbnnr}, DBhost:{DBhost}, AgroUser:{AgroUser}, AgroPassword,
                                                                         filePath, logfilePath, 
                                                                         pResourceFolder, landcode:{landcode}, pCountryCode:{pCountryCode}, 
                                                                         doConversion,
                                                                         begindatum:{einddatum}, einddatum:{einddatum}, pSort:{pSort}");
                                result = Win32AANWEOOI.Init().call_lst_AB_Deklijsten_Leeg(
                                                                         pProgId, pProgramId, filetype,
                                                                         pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                         filePath, logfilePath, 
                                                                         pResourceFolder, landcode, pCountryCode,
                                                                         doConversion);
                            }
                            else
                            {
                                Win32Deklijst.pCallback doConversion = new Win32Deklijst.pCallback(EventCallbackmethod);
                                unLogger.WriteInfo($@"Win32Deklijst call_lst_AB_Deklijsten: pProgId:{pProgId}, pProgramId:{pProgramId}, filetype:{filetype},
                                                                         pUbnnr:{pUbnnr}, DBhost:{DBhost}, AgroUser:{AgroUser}, AgroPassword,
                                                                         filePath, logfilePath, 
                                                                         pResourceFolder, landcode:{landcode}, pCountryCode:{pCountryCode}, 
                                                                         doConversion,
                                                                         begindatum:{einddatum}, einddatum:{einddatum}, pSort:{pSort}");
                                result = Win32Deklijst.Init().call_lst_AB_Deklijsten(
                                                                         pProgId, pProgramId, filetype,
                                                                         pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                         filePath, logfilePath, 
                                                                         pResourceFolder, landcode, pCountryCode, 
                                                                         doConversion,
                                                                         begindatum, einddatum, pSort);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("deklijst error ", exc);
                        }
                        break;

                    case "vmlijst":
                        try
                        {
                            Win32VADERMOE.pCallback doConversion = new Win32VADERMOE.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            int pSort = 0;
                            int.TryParse(addonArr[1], out pSort);

                            int pDierAanwezig = Convert.ToBoolean(addonArr[2]) ? 1 : 0;

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32VADERMOE.Init().call_lst_AB_Vadermoederlijst(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath,
                                                  pResourceFolder, landcode, pCountryCode,
                                                  doConversion,
                                                  begindatum, einddatum,
                                                  pSort, pDierAanwezig);
                        }
                        catch (System.Runtime.InteropServices.SEHException ex)
                        {
                            unLogger.WriteDebug(ex.ToString());
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("vadermoederlijst error ", exc);
                        }
                        break;

                    case "weeglijst":
                        try
                        {
                            Win32Weeglyst.pCallback doConversion = new Win32Weeglyst.pCallback(EventCallbackmethod);

                            bool eigenselectie = Convert.ToBoolean(addonArr[0]);
                            bool aanwdieren = Convert.ToBoolean(addonArr[1]);
                            bool afgdieren = Convert.ToBoolean(addonArr[2]);
                            bool inclafgmoeders = Convert.ToBoolean(addonArr[3]);
                            bool weegrslt = Convert.ToBoolean(addonArr[4]);
                            int stalnr = Convert.ToInt32(addonArr[5]);
                            int groepnr = Convert.ToInt32(addonArr[6]);
                            int sort = Convert.ToInt32(addonArr[7]);

                            //hokken zitten nog niet in de parameters van de DLL
                            //maar dat komt nog WELL
                            int pPerHok = 0;
                            int.TryParse(addonArr[8], out pPerHok);

                            string pAlleenHok = "";
                            if (pPerHok == 1)
                            {
                                pAlleenHok = addonArr[9];
                            }
                            filetype = Convert.ToInt32(addonArr[10]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            string pAniids = "";
                            try 
                            {
                                pAniids = addonArr[11];
                            }
                            catch { }
                            result = Win32Weeglyst.Init().call_lst_AB_Weeglijsten(
                                                                     pProgId, pProgramId, filetype,
                                                                     pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                     filePath, logfilePath, 
                                                                     pResourceFolder, landcode, pCountryCode,
                                                                     doConversion,
                                                                     eigenselectie, aanwdieren, afgdieren, inclafgmoeders, weegrslt,
                                                                     stalnr, groepnr, sort, pAlleenHok, pAniids);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("weeglijst error ", exc);
                        }
                        break;

                    case "geblijst":
                        try
                        {
                            Win32Geblyst.pCallback doConversion = new Win32Geblyst.pCallback(EventCallbackmethod);

                            bool normalGebLijst = Convert.ToBoolean(addonArr[0]);
                            bool legeGebLijst = Convert.ToBoolean(addonArr[1]);
                            bool kolomGebverloop = Convert.ToBoolean(addonArr[2]);
                            bool kolomNaam = Convert.ToBoolean(addonArr[3]);
                            int aantRegels = Convert.ToInt32(addonArr[4]);

                            filetype = Convert.ToInt32(addonArr[5]);

                            //pListMode --> 0=Normale geboortelijst - 1=Lege geboortelijst 
                            //pLabVerloop - Invulling van laatste kolom (alleen nodig bij pListMode=0) 
                            //--> 0=Geboorteverloop - 1=Naam
                            //pNumberOfLines --> aantal regels bij een lege geboortelijst (alleen nodig bij pListMode=1) 

                            int listmode = 0, labverloop = 0, nroflines = 0;
                            if (normalGebLijst)
                            {
                                listmode = 0;
                                if (kolomNaam)
                                { labverloop = 1; }
                                else
                                { labverloop = 0; }
                            }
                            else
                            {
                                listmode = 1;
                                nroflines = aantRegels;
                            }

                            int pSort = 0;
                            int.TryParse(addonArr[6], out pSort);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32Geblyst.Init().call_lst_AB_geboortelijst(
                                                                     pProgId, pProgramId, filetype,
                                                                     pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                     filePath, logfilePath,
                                                                     pResourceFolder, landcode, pCountryCode,
                                                                     doConversion,
                                                                     listmode, labverloop, nroflines,
                                                                     pSort);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("geboortelijst error ", exc);
                        }
                        break;

                    case "nlinglijst":
                        try
                        {
                            Win32SELNLING.pCallback doConversion = new Win32SELNLING.pCallback(EventCallbackmethod);

                            int pAniling = Convert.ToInt32(addonArr[0]);
                            int pSort = Convert.ToInt32(addonArr[1]);

                            int pDierAanwezig = 0;
                            if (addonArr[2] == "true") { pDierAanwezig = 1; }

                            filetype = Convert.ToInt32(addonArr[3]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pPerhok = 0;
                            int.TryParse(addonArr[4], out pPerhok);

                            string pAlleenHok = "";
                            pAlleenHok = addonArr[5];

                            result = Win32SELNLING.Init().call_lst_AB_selectie_Nling(
                                                                         pProgId, pProgramId, filetype,
                                                                         pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                         filePath, logfilePath,
                                                                         pResourceFolder, landcode, pCountryCode,
                                                                         doConversion,
                                                                         pSort, pAniling, pDierAanwezig,
                                                                         begindatum, einddatum, pPerhok, pAlleenHok);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("nlinglijst error ", exc);
                        }
                        break;

                    case "fokwaardenoverzicht":
                        try
                        {
                            Win32LstFokExt.pCallback doConversion = new Win32LstFokExt.pCallback(EventCallbackmethod);

                            filetype = Convert.ToInt32(addonArr[0]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pDierAanwezig = Convert.ToBoolean(addonArr[1]) ? 1 : 0;
                            int pSort = Convert.ToInt32(addonArr[2]);

                            result = Win32LstFokExt.Init().call_lst_AB_Fokwaardenoverzicht(
                                                                        pProgId, pProgramId, filetype,
                                                                        pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                        filePath, logfilePath, doConversion,
                                                                        begindatum,
                                                                        pSort, pDierAanwezig);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("fokwaardenoverzicht error", exc);
                        }
                        break;

                    case "exterieurwaardenoverzicht":
                    case "meetwaardenoverzicht":
                        try
                        {
                            Win32LstFokExt.pCallback doConversion = new Win32LstFokExt.pCallback(EventCallbackmethod);

                            filetype = Convert.ToInt32(addonArr[0]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pDierAanwezig = Convert.ToBoolean(addonArr[1]) ? 1 : 0;
                            int pSort = Convert.ToInt32(addonArr[2]);
                            int pExtJaar = Convert.ToInt32(addonArr[3]);
                            int pSoortLijst = Convert.ToInt32(addonArr[4]);

                            result = Win32LstFokExt.Init().call_lst_AB_Exterieurwaardenoverzicht(
                                                                        pProgId, pProgramId, filetype,
                                                                        pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                        filePath, logfilePath, doConversion,
                                                                        pExtJaar, pSort, pDierAanwezig, pSoortLijst);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("exterieurwaardenoverzicht error", exc);
                        }
                        break;

                    case "stoveevervanging":
                        try
                        {
                            DateTime beginl = begindatum;
                            DateTime eindl = einddatum;

                            Win32stolist.pCallback doConversion = new Win32stolist.pCallback(EventCallbackmethod);

                            filetype = Convert.ToInt32(addonArr[0]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32stolist.Init().call_lst_AB_STO_veevervanging(
                                                                     pProgId, pProgramId, filetype,
                                                                     pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                     filePath, logDir, doConversion,
                                                                     beginl);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("STO veevervanging lijst error ", exc);
                        }
                        break;

                    case "medsupply":
                        try
                        {
                            Win32Medicijn.pCallback doConversion = new Win32Medicijn.pCallback(EventCallbackmethod);

                            //date values sometimes irrelevant - only medsupply has a case in which sometimes 2 dates are needed 
                            //and sometimes none (the block below the dates is the same as the above 'else'.
                            begindatum = DateTime.Now;
                            einddatum = DateTime.Now;

                            bool huidigevoorraad = Convert.ToBoolean(addonArr[0]);
                            bool allemedicijnen = Convert.ToBoolean(addonArr[1]);
                            bool aangekochtemeds = Convert.ToBoolean(addonArr[2]);

                            filetype = Convert.ToInt32(addonArr[3]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            if (huidigevoorraad)
                            {
                                result = Win32Medicijn.Init().call_lst_AB_MedicijnVoorraad(
                                                                                  pProgId, pProgramId, filetype,
                                                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                                  filePath, logfilePath,
                                                                                  pResourceFolder, landcode, pCountryCode,
                                                                                  doConversion,
                                                                                  allemedicijnen);
                            }
                            else
                            {
                                result = Win32Medicijn.Init().call_lst_AB_MedicijnAankoop(
                                                                                 pProgId, pProgramId, filetype,
                                                                                 pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                                 filePath, logfilePath,
                                                                                 pResourceFolder, landcode, pCountryCode,
                                                                                 doConversion,
                                                                                 begindatum, einddatum);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("medicijnvoorraad lijst error ", exc);
                        }
                        break;

                    case "voerleverlijst":
                        try
                        {
                            Win32FEEDSTCK.pCallback doConversion = new Win32FEEDSTCK.pCallback(EventCallbackmethod);

                            bool subtotaalpoeder = Convert.ToBoolean(addonArr[0]);
                            int sortvalue = Convert.ToInt32(addonArr[1]);

                            filetype = Convert.ToInt32(addonArr[2]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32FEEDSTCK.Init().call_lst_AB_poederleveringsoverzicht(
                                                                        pProgId, pProgramId, filetype,
                                                                        pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                        filePath, logfilePath, doConversion,
                                                                        begindatum, einddatum, sortvalue, subtotaalpoeder);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("poederlevering lijst error ", exc);
                        }
                        break;
                    case "voerverbruik":
                        try
                        {
                            Win32FEEDSTCK.pCallback doConversion = new Win32FEEDSTCK.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSoortVoer = Convert.ToInt32(addonArr[1]); ;
                            int pGroepnr = Convert.ToInt32(addonArr[2]);

                            if (pGroepnr > 0)
                            {

                                result = Win32FEEDSTCK.Init().call_lst_AB_voerverbruik(
                                                                            pProgId, pProgramId, filetype,
                                                                            pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                            filePath, logfilePath, doConversion,
                                                                            pSoortVoer, pGroepnr);
                            }
                            else { result = 0; }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("voerverbruik lijst error ", exc);
                        }
                        break;
                    case "Voervoorraad":
                        try
                        {
                            Win32FEEDSTCK.pCallback doConversion = new Win32FEEDSTCK.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            result = Win32FEEDSTCK.Init().call_lst_AB_VoerVoorraad(
                                                                        pProgId, pProgramId, filetype,
                                                                        pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                        filePath, logfilePath, doConversion,
                                                                        einddatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Voervoorraad lijst error ", exc);
                        }
                        break;
                    case "afvselect":
                        try
                        {
                            Win32Scanlist.pCallback doConversion = new Win32Scanlist.pCallback(EventCallbackmethod);

                            int destThrId = 0;
                            int HandelaarThrId = 0;

                            int.TryParse(addonArr[0], out destThrId);
                            int.TryParse(addonArr[1], out HandelaarThrId);

                            filetype = Convert.ToInt32(addonArr[2]);

                            int pUniekLevNr = 0;
                            int.TryParse(addonArr[3], out pUniekLevNr);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32Scanlist.Init().call_lst_AB_AfvoerSelectieLijst(
                                                                  pProgId, pProgramId, filetype,
                                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                  filePath, logfilePath, doConversion,
                                                                  begindatum, destThrId, HandelaarThrId, pUniekLevNr);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("afvoer selectie lijst error ", exc);
                        }
                        break;

                    case "wachttijdlijst":
                        try
                        {
                            Win32CALFLIST.pCallback doConversion = new Win32CALFLIST.pCallback(EventCallbackmethod);

                            int sorton = Convert.ToInt32(addonArr[0]);
                            filetype = Convert.ToInt32(addonArr[1]);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pPerHok = 0;
                            int.TryParse(addonArr[2], out pPerHok);

                            string pAlleenHok = "";
                            pAlleenHok = addonArr[3];

                            result = Win32CALFLIST.Init().call_lst_AB_Wachttijdlijst(
                                                            pProgId, pProgramId, filetype,
                                                            pUbnnr, DBhost, AgroUser, AgroPassword,
                                                            filePath, logfilePath, pResourceFolder, landcode, pCountryCode, pLandnr, doConversion,
                                                            begindatum, sorton, pPerHok, pAlleenHok);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("wachttijd lijst error ", exc);
                        }
                        break;

                    case "afleverlijst":
                        try
                        {
                            Win32CALFLIST.pCallback doConversion = new Win32CALFLIST.pCallback(EventCallbackmethod);

                            filetype = Convert.ToInt32(addonArr[0]);

                            int groupId = 0;
                            int sort = 0;

                            int.TryParse(addonArr[1], out groupId);
                            int.TryParse(addonArr[2], out sort);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32CALFLIST.Init().call_lst_AB_Afleverlijst(
                                                                pProgId, pProgramId, filetype,
                                                                pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                filePath, logfilePath, doConversion,
                                                                begindatum, einddatum, groupId, sort);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("afleverlijst error ", exc);
                        }
                        break;

                    case "behandelinglijst":
                        try
                        {
                            Win32CALFLIST.pCallback doConversion = new Win32CALFLIST.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            bool rbBehandelingValue = false;
                            bool.TryParse(addonArr[1], out rbBehandelingValue);

                            int ddlBehandelingValue = 0;
                            int.TryParse(addonArr[2], out ddlBehandelingValue);

                            int pTreKind = 0; //alle behandelingen
                            if (rbBehandelingValue == true) { pTreKind = ddlBehandelingValue; }

                            bool pLaatsteBehandeling = false;
                            bool.TryParse(addonArr[3], out pLaatsteBehandeling);

                            bool pDierZonder = false;
                            bool.TryParse(addonArr[4], out pDierZonder);

                            bool pDierAanwezig = true;
                            bool.TryParse(addonArr[5], out pDierAanwezig);

                            int sort = 0;
                            int.TryParse(addonArr[6], out sort);

                            int pGroupId = 0;
                            int.TryParse(addonArr[7], out pGroupId);

                            string pUBN = pUbnnr;
                            string ubnKeuze = addonArr[8];

                            if (pProgramId == 100) //Elda superuser
                            {
                                if (ubnKeuze == "ubnKeuze_ALLE") { pUBN = ""; }
                                else if (ubnKeuze != "")
                                {
                                    pUBN = ubnKeuze;
                                    pProgramId = 7;
                                }
                            }

                            int pPerHok = 0;
                            int.TryParse(addonArr[9], out pPerHok);

                            string pAlleenHok = "";
                            pAlleenHok = addonArr[10];

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pPlanId = 0;
                            int.TryParse(addonArr[11], out pPlanId);

                            result = Win32CALFLIST.Init().call_lst_AB_Behandelinglijst(
                                                                pProgId, pProgramId, filetype,
                                                                pUBN, DBhost, AgroUser, AgroPassword,
                                                                filePath, logfilePath,
                                                                pResourceFolder, landcode, pCountryCode,
                                                                doConversion,
                                                                begindatum, einddatum, pTreKind, pPlanId,
                                                                pLaatsteBehandeling, pDierZonder,
                                                                pDierAanwezig, sort, pGroupId, pPerHok, pAlleenHok);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("behandelinglijst error ", exc);
                        }
                        break;

                    case "kopreslijst":
                        try
                        {
                            Win32Weeklyst.pCallback doConversion = new Win32Weeklyst.pCallback(EventCallbackmethod);

                            int pGroupId = Convert.ToInt32(addonArr[1]);

                            int pUitvalCorrectie = Convert.ToBoolean(addonArr[2]) ? 1 : 0;
                            int pInclVat = Convert.ToBoolean(addonArr[3]) ? 1 : 0;
                            int pDefaultBehandelKosten = Convert.ToBoolean(addonArr[4]) ? 1 : 0;

                            int pSavedValueId = -1;
                            int.TryParse(addonArr[5], out pSavedValueId);

                            int pEnableLegenda = Convert.ToBoolean(addonArr[6]) ? 1 : 0;

                            filetype = Convert.ToInt32(addonArr[0]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            if(DBhost==""){DBhost="217.148.183.77";}
                            result = Win32Weeklyst.Init().call_lst_AB_Koppelresultaten(pProgId, pProgramId, filetype,
                                                                                       pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                                       filePath, logfilePath, doConversion,
                                                                                       pGroupId,
                                                                                       pUitvalCorrectie, pInclVat, pDefaultBehandelKosten,
                                                                                       pSavedValueId, pEnableLegenda);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("koppelresultatenlijst error ", exc);
                        }
                        break;

                    case "kopreslijstmulti":
                        try
                        {
                            Win32Weeklyst.pCallback doConversion = new Win32Weeklyst.pCallback(EventCallbackmethod);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            string xmlBestand = addonArr[1];
                            string pXmlBestand = outputDir + xmlBestand;

                            int pUitvalCorrectie = Convert.ToBoolean(addonArr[2]) ? 1 : 0;
                            int pInclVat = Convert.ToBoolean(addonArr[3]) ? 1 : 0;
                            int pDefaultBehandelKosten = Convert.ToBoolean(addonArr[4]) ? 1 : 0;

                            int pSavedValueId = -1;
                            int.TryParse(addonArr[5], out pSavedValueId);

                            int pEnableLegenda = Convert.ToBoolean(addonArr[6]) ? 1 : 0;

                            int pSaveValues = Convert.ToBoolean(addonArr[7]) ? 1 : 0;
                            string pSaveValues_Title = addonArr[8];
                            string pSaveValues_Comment = addonArr[9];
                            if (DBhost == "") { DBhost = "217.148.183.77"; }
                            result = Win32Weeklyst.Init().call_lst_AB_Koppelresultaten_Multi(
                                                                    pProgId, pProgramId, filetype,
                                                                    DBhost, AgroUser, AgroPassword,
                                                                    pXmlBestand, filePath, logfilePath,
                                                                    doConversion,
                                                                    pUitvalCorrectie, pInclVat, pDefaultBehandelKosten,
                                                                    pSavedValueId, pEnableLegenda,
                                                                    pSaveValues, pSaveValues_Title, pSaveValues_Comment);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("koppelresultatenlijst multi error ", exc);
                        }
                        break;

                    case "vkilijst":
                        try
                        {
                            Win32lstvkialg.pCallback doConversion = new Win32lstvkialg.pCallback(EventCallbackmethod);

                            int internalnr = 0;
                            if (addonArr[0] != "")
                            {
                                if (utils.isIntegerNumber(addonArr[0]))
                                {
                                    internalnr = Convert.ToInt32(addonArr[0]);
                                }
                            }

                            //hokken zitten nog niet in de parameters van de DLL
                            //maar dat komt nog WELL
                            int pPerHok = 0;
                            int.TryParse(addonArr[1], out pPerHok);

                            string pAlleenHok = "";
                            if (addonArr.Length > 4)//LET OP  Countrycode is er achteraangeplakt
                            {
                                if (pPerHok == 1)
                                {
                                    pAlleenHok = addonArr[2];
                                }

                                if (addonArr[3] != "")
                                {
                                    if (utils.isIntegerNumber(addonArr[3]))
                                    {
                                        filetype = Convert.ToInt32(addonArr[3]);
                                    }
                                }
                            }
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            unLogger.WriteInfo("vkilijst parameters:" + pProgId + ", " + pProgramId + ", " + filetype.ToString() + ", " +
                                                                          pUbnnr + ", " + DBhost + ", " + AgroUser + ", " + AgroPassword + ", " + filePath + ", " + logfilePath + ", " +
                                                                          "doConversion" + ", " +
                                                                          internalnr.ToString());
                            result = Win32lstvkialg.Init().call_lst_AB_VKIalgemeen(
                                                                          pProgId, pProgramId, filetype,
                                                                          pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                          filePath, logfilePath,
                                                                          doConversion,
                                                                          internalnr, pAlleenHok);
                            unLogger.WriteInfo("vkilijst " + pUbnnr + " Win32lstvkialg klaar");
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("VKIlijst error ", exc);
                        }
                        break;

                    case "predikaatlijst":
                        try
                        {
                            Win32lstPredikaat.pCallback doConversion = new Win32lstPredikaat.pCallback(EventCallbackmethod);

                            //UITZONDERING
                            char[] tmpSplit = { '#' };
                            string[] tmp = addons.Split(tmpSplit);

                            if (tmp[0] != "")
                            {
                                if (utils.isIntegerNumber(tmp[0]))
                                {
                                    filetype = Convert.ToInt32(tmp[0]);
                                }
                            }

                            string lifeNr = tmp[1];
                            int pSoortLijst = int.Parse(tmp[2]);

                            //probeer via lifeNr het dier te achterhalen (kan zowel alternatelifenr als lifenr zijn).
                            ANIMAL animal;
                            int pAniId = 0;

                            animal = lMstb.GetAnimalByAniAlternateNumber(lifeNr);
                            pAniId = animal.AniId;
                            if (pAniId == 0)
                            {
                                animal = lMstb.GetAnimalByLifenr(lifeNr);
                                pAniId = animal.AniId;
                            }

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32lstPredikaat.Init().call_lst_AB_Predikaatlijst(
                                                pProgId, pProgramId, filetype,
                                                pUbnnr, DBhost, AgroUser, AgroPassword,
                                                filePath, logfilePath, doConversion,
                                                pAniId, pSoortLijst);
                        }
                        catch (Exception e)
                        {
                            unLogger.WriteError("Predikaatlijst error ", e);
                        }
                        break;
                    case "dierkaart":
                        try
                        {
                            Win32KKAART.pCallback doConversion = new Win32KKAART.pCallback(EventCallbackmethod);
                            int pAniId = getAniIdFromStringOrUser(addonArr[0], gebruikerAnimalAniId);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            unLogger.WriteDebug("Win32KKAART.Init().call_lst_AB_Dierkaart PARAMETERS:(" + pProgId.ToString() + "," + pProgramId.ToString() + "," + filetype +
                                  "," + pUbnnr.ToString() + "," + DBhost.ToString() + "," + AgroUser.ToString() + "," + AgroPassword +
                                  filePath.ToString() + "," + logfilePath.ToString() + "," + doConversion.ToString() + "," +
                                  pAniId.ToString() + ")");
                            result = Win32KKAART.Init().call_lst_AB_Dierkaart(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, pLandnr , doConversion,
                                                  pAniId);  
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("dierkaart error ", exc);
                        }
                        break;
                    case "koekaart":
                        try
                        {
                            Win32KKAART.pCallback doConversion = new Win32KKAART.pCallback(EventCallbackmethod);
                            int pAniId = getAniIdFromStringOrUser(addonArr[0], gebruikerAnimalAniId);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            unLogger.WriteDebug("Win32KKAART.Init().call_lst_AB_Koekaart PARAMETERS:(" + pProgId.ToString() + "," + pProgramId.ToString() + "," + filetype +
                                  "," + pUbnnr.ToString() + "," + DBhost.ToString() + "," + AgroUser.ToString() + "," + AgroPassword +
                                  filePath.ToString() + "," + logfilePath.ToString() + "," + doConversion.ToString() + "," +
                                  pAniId.ToString() + ",1)");
                            result = Win32KKAART.Init().call_lst_AB_Koekaart(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  pAniId, 1); //soortlijst=1 - uitgebreid
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Koekaart error ", exc);
                        }
                        break;
                    case "geitkaart":
                        try
                        {
                            Win32GEITKRT.pCallback doConversion = new Win32GEITKRT.pCallback(EventCallbackmethod);

                            int pAniId = getAniIdFromStringOrUser(addonArr[0], gebruikerAnimalAniId);

                            bool pRightsMilk = false;
                            try { pRightsMilk = Convert.ToBoolean(addonArr[1]); }
                            catch { }

                            bool pBeperkt = false;
                            try { pBeperkt = Convert.ToBoolean(addonArr[2]); }
                            catch { }

                            bool pAfstamming = false;
                            try { pAfstamming = Convert.ToBoolean(addonArr[3]); }
                            catch { }

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32GEITKRT.Init().call_lst_AB_Geitkaart(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  pAniId, pRightsMilk, pBeperkt, pAfstamming);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Geitkaart error ", exc);
                        }
                        break;

                    case "schapenkaart":
                        try
                        {
                            Win32Ooikaart.pCallback doConversion = new Win32Ooikaart.pCallback(EventCallbackmethod);

                            int pAniId = getAniIdFromStringOrUser(addonArr[0], gebruikerAnimalAniId);
                            bool pRamProdUitgebreid = true;// Convert.ToBoolean(addonArr[1]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32Ooikaart.Init().call_lst_AB_Schapenkaart(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath,
                                                  pResourceFolder, landcode, pCountryCode, 
                                                  doConversion,
                                                  pAniId, pRamProdUitgebreid);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Schapenkaart error ", exc);
                        }
                        break;

                    case "afstambewijs":
                        try
                        {
                            Win32Ooikaart.pCallback doConversion = new Win32Ooikaart.pCallback(EventCallbackmethod);

                            int pAniId = getAniIdFromStringOrUser(addonArr[0], gebruikerAnimalAniId);

                            int pSoortLijst = 0;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32Ooikaart.Init().call_lst_AB_Afstambewijs(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath,
                                                  pResourceFolder, landcode, pCountryCode,
                                                  doConversion,
                                                  pAniId, pSoortLijst);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Afstambewijs error ", exc);
                        }
                        break;
                    case "zootechnischcert":
                        try
                        {
                            Win32Ooikaart.pCallback doConversion = new Win32Ooikaart.pCallback(EventCallbackmethod);

                            int pAniId = getAniIdFromStringOrUser(addonArr[0], gebruikerAnimalAniId);

                            int pSoortLijst = 0;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32Ooikaart.Init().call_lst_AB_Afstambewijs2(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath,
                                                  pResourceFolder, landcode, pCountryCode,
                                                  doConversion,
                                                  pAniId, pSoortLijst);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("zootechnischcert error ", exc);
                        }
                        break;
                        
                    case "afstambewijsgeit":
                        try
                        {
                            Win32GEITKRT.pCallback doConversion = new Win32GEITKRT.pCallback(EventCallbackmethod);



                            int pSoortLijst = 0;
                            int.TryParse(addonArr[0], out pSoortLijst);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pAniId = getAniIdFromStringOrUser(addonArr[1], gebruikerAnimalAniId);
                            bool pDochters = Convert.ToBoolean(addonArr[2]);
                            bool pZonen = Convert.ToBoolean(addonArr[3]);
                            bool pZussen = Convert.ToBoolean(addonArr[4]);
                            bool pBroers = Convert.ToBoolean(addonArr[5]);

                            result = Win32GEITKRT.Init().call_lst_AB_AfstammingsbewijsGeit(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  pAniId, pDochters, pZonen, pZussen, pBroers);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Afstambewijs error ", exc);
                        }
                        break;
                    case "attentielijst_kalf":
                    case "attentielijst_kalf_dagelijks":
                    case "attentielijst_kalf_regelmatig":
                    case "attentielijst_kalf_controle":
                        try
                        {
                            Win32lstattentielijst.pCallback doConversion = new Win32lstattentielijst.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            int pSoortLijst = 0;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            int pAantalBehandelingen = 0;
                            int.TryParse(addonArr[2], out pAantalBehandelingen);

                            DateTime pDtBegin;
                            DateTime pDtEnd;
                            if (pSoortLijst == 2)
                            {
                                pDtBegin = begindatum;
                                pDtEnd = einddatum;
                            }
                            else
                            {
                                pDtEnd = DateTime.Today;
                                pDtBegin = pDtEnd.AddDays(-30);
                            }

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32lstattentielijst.Init().call_lst_AB_Attentielijst_Kalf(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  pSoortLijst, pDtBegin, pDtEnd, pAantalBehandelingen);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("call_lst_AB_Attentielijst_Kalf error", exc);
                        }
                        break;

                    case "aanvoerafvoerlijst":
                        try
                        {
                            Win32LstAanAfvoe.pCallback doConversion = new Win32LstAanAfvoe.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            int pSoortLijst = 0;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            int pSort = 0;
                            int.TryParse(addonArr[2], out pSort);

                            int pThrIdAfnemer = 0;
                            int.TryParse(addonArr[3], out pThrIdAfnemer);

                            int pDierAanwezig = Convert.ToBoolean(addonArr[4]) ? 1 : 0;
                            int pSubTotHandelaar = Convert.ToBoolean(addonArr[5]) ? 1 : 0;

                            int pUniekLevNr = 0;
                            int.TryParse(addonArr[6], out pUniekLevNr);

                            //hokken zitten nog niet in de parameters van de DLL
                            //maar dat komt nog WELL
                            int pPerHok = 0;
                            int.TryParse(addonArr[7], out pPerHok);

                            string pAlleenHok = "";
                            if (pPerHok == 1)
                            {
                                pAlleenHok = addonArr[8];
                            }

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32LstAanAfvoe.Init().call_lst_AB_AanvoerAfvoerLijst(
                                                 pProgId, pProgramId, filetype,
                                                 pUbnnr, DBhost, AgroUser, AgroPassword,
                                                 filePath, logfilePath,
                                                 pResourceFolder,landcode, pCountryCode,
                                                 doConversion,
                                                 begindatum, einddatum, pSoortLijst, pSort,
                                                 pDierAanwezig, pThrIdAfnemer, pSubTotHandelaar, pUniekLevNr, pAlleenHok);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("aanvoer/afvoerlijst error", exc);
                        }
                        break;

                    case "uitvallijst":
                        try
                        {
                            Win32Uitval.pCallback doConversion = new Win32Uitval.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            int pSort = 0;
                            int.TryParse(addonArr[1], out pSort);

                            int uitvallijstType = 0;
                            int.TryParse(addonArr[2], out uitvallijstType);

                            int pGroupId = 0;
                            int.TryParse(addonArr[3], out pGroupId);

                            int pGroupBy = 0;
                            int.TryParse(addonArr[4], out pGroupBy);

                            int uitvallijstBedrijfId = 0;
                            int.TryParse(addonArr[5], out uitvallijstBedrijfId);

                            int pThrIdLeverancier = 0;
                            int.TryParse(addonArr[6], out pThrIdLeverancier);

                            int pPerhok = 0;
                            int.TryParse(addonArr[7], out pPerhok);

                            string pAlleenHok = "";
                            pAlleenHok = addonArr[8];


                            int iProgramId = pProgramId;
                            int iProgId = pProgId;
                            String sUbn = pUbnnr;

                            if ((isAdmin == true) && (uitvallijstBedrijfId > 0))
                            {
                                //vul bedrijf en ubn objecten met het ondergeschikte bedrijf
                                BEDRIJF bedrijf = null;
                                UBN ubn = null;

                                if ((iProgramId == 80) || (iProgramId == 81))
                                {
                                    //navobi en vilatca; uitvallijstBedrijfId is een ubnid
                                    int tmpUbnId = uitvallijstBedrijfId;

                                    if (tmpUbnId > 0)
                                    {
                                        ubn = lMstb.GetubnById(tmpUbnId);

                                        List<BEDRIJF> bedrijven = lMstb.getBedrijvenByUBNId(tmpUbnId);
                                        var cc = from n in bedrijven where n.ProgId == pProgId select n;
                                        if (cc.Count() > 0)
                                        {
                                            bedrijf = cc.ElementAt(0);  
                                        }
                                    }
                                }
                                else
                                {
                                    //uitvallijstBedrijfId is een farmid
                                    int tmpFarmId = uitvallijstBedrijfId;
                                    if (tmpFarmId > 0)
                                    {
                                        bedrijf = lMstb.GetBedrijfById(tmpFarmId);
                                        ubn = lMstb.GetubnById(bedrijf.UBNid);
                                    }
                                }

                                if ((bedrijf != null) && (ubn != null))
                                {
                                    //vervang parameters door parameters van het ondergeschikte bedrijf
                                    iProgramId = bedrijf.Programid;
                                    iProgId = bedrijf.ProgId;
                                    sUbn = ubn.Bedrijfsnummer;
                                }
                                else
                                {
                                    iProgramId = 0;
                                    iProgId = 0;
                                    sUbn = "";
                                }
                            }

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            if (uitvallijstType == 0)
                            {
                                result = Win32Uitval.Init().call_lst_AB_Uitvallijst_MetHok(
                                                        iProgId, iProgramId, filetype,
                                                        sUbn, DBhost, AgroUser, AgroPassword,
                                                        filePath, logfilePath,
                                                        pResourceFolder, landcode, pCountryCode,
                                                        doConversion,
                                                        begindatum, einddatum,
                                                        pGroupId, pSort, pThrIdLeverancier, pPerhok, pAlleenHok);
                            }
                            else if (uitvallijstType == 1)
                            {
                                result = Win32Uitval.Init().call_lst_AB_Uitvallijst_Overzicht(
                                                        iProgId, iProgramId, filetype,
                                                        sUbn, DBhost, AgroUser, AgroPassword,
                                                        filePath, logfilePath,
                                                        pResourceFolder, landcode, pCountryCode,
                                                        doConversion,
                                                        begindatum, einddatum,
                                                        pSort, pGroupBy, pThrIdLeverancier);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("uitvallijst error", exc);
                        }
                        break;

                    case "transportlijst":
                        try
                        {
                            Win32LSTTRANSPORT.pCallback doConversion = new Win32LSTTRANSPORT.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            int pSort = 0;
                            int.TryParse(addonArr[1], out pSort);

                            int pMovThridExecutive = 0;
                            int.TryParse(addonArr[2], out pMovThridExecutive);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32LSTTRANSPORT.Init().call_lst_AB_Transportlijst(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  begindatum, einddatum,
                                                  pSort, pMovThridExecutive);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("transportlijst error", exc);
                        }
                        break;

                    case "barcodelijst":
                        try
                        {
                            Win32Lst_Barcode.pCallback doConversion = new Win32Lst_Barcode.pCallback(EventCallbackmethod);

                            int pZiekteID = 0;
                            int.TryParse(addonArr[0], out pZiekteID);

                            int FarmId = 0;
                            int.TryParse(addonArr[1], out FarmId);

                            BEDRIJF bedrijf = lMstb.GetBedrijfById(FarmId);
                            if (bedrijf.Programid == 47)
                            {
                                List<BEDRIJF> bedrijven = lMstb.getBedrijvenByFarmId(FarmId);
                                if (bedrijven.Count() > 0)
                                {
                                    var theOtherOne = from n in bedrijven
                                                      where n.ProgId == bedrijf.ProgId
                                                      && (n.Programid != 47)
                                                      select n;
                                    if (theOtherOne.Count() > 0)
                                    {
                                        bedrijf = theOtherOne.ElementAt(0);
                                    }
                                }
                            }
                            UBN ubn = lMstb.GetubnById(bedrijf.UBNid);

                            int iProgramId = bedrijf.Programid;
                            int iProgId = bedrijf.ProgId;
                            String sUbn = ubn.Bedrijfsnummer;

                            filePath = outputDir + fileNameWithoutExt + getExt(0);

                            string pRavBestand = serverMainDir + "lib\\rvAnimalBarcode.rav";

                            result = Win32Lst_Barcode.Init().call_lst_Barcode_Afdrukken(
                                                  iProgId, iProgramId, filetype,
                                                  sUbn, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  pRavBestand, pZiekteID);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Barcodelijst error", exc);
                        }
                        break;
                    case "gezondheidsverklaring":
                        try
                        {
                            Win32Lst_Gezondheidsverklaring.pCallback doConversion = new Win32Lst_Gezondheidsverklaring.pCallback(EventCallbackmethod);

                            int FarmIdG = 0;
                            int.TryParse(addonArr[0], out FarmIdG);

                            BEDRIJF bedrijf = lMstb.GetBedrijfById(FarmIdG);
                            UBN ubn = lMstb.GetubnById(bedrijf.UBNid);

                            int iProgramId = bedrijf.Programid;
                            int iProgId = bedrijf.ProgId;
                            String sUbn = ubn.Bedrijfsnummer;

                            filePath = outputDir + fileNameWithoutExt + getExt(0);

                            string pRavBestand = serverMainDir + "lib\\rvGezondheidsverklaring.rav";

                            result = Win32Lst_Gezondheidsverklaring.Init().call_lst_Gezondheidsverklaring_Afdrukken(
                                                  iProgId, iProgramId, filetype,
                                                  sUbn, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  pRavBestand);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Gezondheidsverklaring error", exc);
                        }
                        break;

                    case "veesaldolijst":
                        try
                        {
                            Win32Veesaldo.pCallback doConversion = new Win32Veesaldo.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            //int pJaar = 0;
                            //int.TryParse(addonArr[1], out pJaar);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            result = Win32Veesaldo.Init().call_lst_AB_Veesaldo(
                                                  pProgId, pProgramId, filetype,
                                                  pUbnnr, DBhost, AgroUser, AgroPassword,
                                                  filePath, logfilePath, doConversion,
                                                  begindatum, einddatum);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Veesaldolijst error", exc);
                        }
                        break;

                    case "mprlijst":
                        try
                        {
                            Win32LSTRDBMPR.pCallback doConversion = new Win32LSTRDBMPR.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32LSTRDBMPR.Init().call_lst_AB_MPRuitslag(
                                                    pProgId, pProgramId, filetype,
                                                    pUbnnr, DBhost, AgroUser, AgroPassword,
                                                    filePath, logfilePath, doConversion,
                                                    begindatum);



                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("mprlijst error ", exc);
                        }
                        break;
                    case "mprlijst_all":
                        try
                        {
                            Win32Milkcont.pCallback doConversion = new Win32Milkcont.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pExtraGegevens = 0;
                            int.TryParse(addonArr[1], out pExtraGegevens);
                            int pSubTotaalPerRantsoenGroep = 0;
                            int.TryParse(addonArr[2], out pSubTotaalPerRantsoenGroep);
                            int pSoortDiernr = 0;
                            int.TryParse(addonArr[3], out pSoortDiernr);
                            int pSoortDieren = 0;
                            int.TryParse(addonArr[4], out pSoortDieren);
                            int pSoortHok = 0;
                            int.TryParse(addonArr[5], out pSoortHok);
                            int pAlleenSubtotalen = 0;
                            int.TryParse(addonArr[6], out pAlleenSubtotalen);
                            int pAantalBesteSlechtste = 0;
                            int.TryParse(addonArr[7], out pAantalBesteSlechtste);
                            int pSortering = 0;
                            int.TryParse(addonArr[8], out pSortering);
                            string pHoknr = addonArr[9];
                            int pLogoID = pProgramId;
                            unLogger.WriteInfo($@"mprlijst_all addons={addons} addonArr.count={addonArr.Count()}");
                      

                            int[] possiblecountrys = { 56, 208, 250, 276, 380, 528, 826, 883 };
                            try
                            {
                                if (!int.TryParse(addonArr[10], out pLogoID))
                                {
                                    pLogoID = pProgramId;
                                }
                                if (possiblecountrys.Contains(pLogoID))
                                {
                                    pLogoID = pProgramId;
                                }
                            }
                            catch { }
                            result = Win32Milkcont.Init().call_lst_AB_MPRuitslag(
                                                    pProgId, pProgramId, pLogoID, filetype,
                                                    pUbnnr, DBhost, AgroUser, AgroPassword,
                                                    filePath, logfilePath, 
                                                    pResourceFolder, landcode, pCountryCode,
                                                    doConversion,
                                                    begindatum, pExtraGegevens, pSubTotaalPerRantsoenGroep, pSoortDiernr,
                                                    pSoortDieren, pSoortHok, pAlleenSubtotalen, pAantalBesteSlechtste, pSortering,
                                                    pHoknr);

                          

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("mprlijst error ", exc);
                        }
                        break;
                    case "melkproductielijst":
                        try
                        {
                            Win32Melkprod.pCallback doConversion = new Win32Melkprod.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSoortLijst = 0;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            result = Win32Melkprod.Init().call_lst_AB_Melkproductielijst(
                                                    pProgId, pProgramId, filetype,
                                                    pUbnnr, DBhost, AgroUser, AgroPassword,
                                                    filePath, logfilePath, doConversion,
                                                    begindatum, einddatum, pSoortLijst);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("melkproductielijst error ", exc);
                        }
                        break;

                    case "BloedonderzoekAuthorisatie":
                        try
                        {
                            Win32Lst_Bloedonderzoeken.pCallback doConversion = new Win32Lst_Bloedonderzoeken.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            int pUbnId;
                            int.TryParse(addonArr[1], out pUbnId);

                            int pBloAuthorized;
                            int.TryParse(addonArr[2], out pBloAuthorized);

                            int pBloAuthorizedByThrID;
                            int.TryParse(addonArr[3], out pBloAuthorizedByThrID);

                            string pDierZiekteCSV = addonArr[4];

                            int pTypeUitslag;
                            int.TryParse(addonArr[5], out pTypeUitslag);

                            string pInzendNummer = addonArr[6];
                            string pRedenInzending = addonArr[7];
                            string pOpmerking = addonArr[8];
                            string fileNameWithoutExtOverload = addonArr[9];

                            bool pEmailList = false;
                            bool.TryParse(addonArr[10], out pEmailList);

                            string pEmailTo = string.Empty;
                            string pEmailCC = string.Empty;
                            string pEmailCCVet = string.Empty;

                            if (addonArr.Length >= 13)//LET OP  Countrycode is er achteraangeplakt 
                            {
                                pEmailTo = addonArr[11];
                                pEmailCC = addonArr[12];
                                pEmailCCVet = addonArr[13];
                            }

                            if (fileNameWithoutExtOverload != string.Empty)
                                fileNameWithoutExt = fileNameWithoutExtOverload;

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            logfilePath = logDir + fileNameWithoutExt + getExt(2); //log=txt
                            UBN lUbn = lMstb.GetubnById(pUbnId);
                            int pFarmId = 0;
                            int lProgramId = pProgramId;
                            List<BEDRIJF> bedrijven = lMstb.getBedrijvenByUBNId(lUbn.UBNid);
                            var sel = from n in bedrijven where n.ProgId == pProgId orderby n.Programid select n ;
                            if (sel.Count() > 0) { pFarmId = sel.ElementAt(0).FarmId; lProgramId = sel.ElementAt(0).Programid; }
                            result = Win32Lst_Bloedonderzoeken.Init().call_lst_AB_BloedonderzoekAuthorisatie(
                                                    pProgId, lProgramId, filetype,
                                                    lUbn.Bedrijfsnummer, DBhost, AgroUser, AgroPassword,
                                                    filePath, logfilePath, doConversion,
                                                    pFarmId, pBloAuthorized, pBloAuthorizedByThrID,
                                                    begindatum,
                                                    pDierZiekteCSV,
                                                    pTypeUitslag,
                                                    pInzendNummer,
                                                    pRedenInzending,
                                                    pOpmerking);

                            if ((result == 1) && (pEmailList == true))
                            {
                                //vieze code..., heeft te maken met de ajax requests.
                                string pSubject = "Bloedonderzoek Autorisatie Resultaat:" + fileNameWithoutExt;

                                if (pEmailTo != string.Empty)
                                {
                                    string cc = pEmailCC + ";" + pEmailCCVet;

                                    StringBuilder pMailBody = new StringBuilder();

                                    pMailBody.AppendLine("Geachte heer, mevrouw,");
                                    pMailBody.AppendLine();

                                    pMailBody.AppendLine(
                                        "In de bijlage vindt u de uitslag van het door u aangevraagde bloedonderzoek. " +
                                        "Indien dit een deeluitslag betreft, staat er bij uitslagtype 'Deeluitslag'. " +
                                        "Indien het gaat om een einduitslag, dan staat er bij uitslagtype 'Einduitslag'.");

                                    pMailBody.AppendLine();

                                    pMailBody.AppendLine(
                                        "Dit is een automatisch gegenereerde e-mail, waarop u niet kunt reageren. " +
                                        "Voor vragen over deze uitslag kunt u contact opnemen met de NSFO via kantoor@nsfo.nl " +
                                        "of via telefoon 0418-561712 (maandag en donderdag). ");

                                    pMailBody.AppendLine();
                                    pMailBody.AppendLine("Met vriendelijke groeten,");
                                    pMailBody.AppendLine();
                                    pMailBody.AppendLine("Marjo van Bergen");
                                    pMailBody.AppendLine("hoofdinspecteur NSFO");

                                    int mailRes = mailLijst(pEmailTo, cc, filePath, pSubject, pMailBody, pTestDB);

                                    //string strSession = bestandnaam;
                                    //Application.Set(strSession, "Stop Email is verzonden");
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("BloedonderzoekAuthorisatie", exc);
                        }
                        break;
                    case "Brucella_Inzend_Formulier":
                        try
                        {
                            Win32Lst_Brucella_Monitoring.pCallback doConversion = new Win32Lst_Brucella_Monitoring.pCallback(EventCallbackmethod);

                            string lBedrijfsnummer = addonArr[1];
                            int Pbzs_ID = Convert.ToInt32(addonArr[2]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            logfilePath = logDir + fileNameWithoutExt + getExt(2); //log=txt

                            result = Win32Lst_Brucella_Monitoring.Init().call_lst_Brucella_Inzend_Formulier(
                                pProgId, pProgramId, filetype,
                                lBedrijfsnummer, DBhost, AgroUser, AgroPassword,
                                filePath, logfilePath, doConversion, Pbzs_ID);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Brucella_Inzend_Formulier", exc);
                        }
                        break;
                    case "Brucella_Declaratie_Formulier":
                        try
                        {
                            Win32Lst_Brucella_Monitoring.pCallback doConversion = new Win32Lst_Brucella_Monitoring.pCallback(EventCallbackmethod);
                            string lBedrijfsnummer = addonArr[1];
                            int Pbzs_ID = Convert.ToInt32(addonArr[2]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            logfilePath = logDir + fileNameWithoutExt + getExt(2); //log=txt

                            result = Win32Lst_Brucella_Monitoring.Init().call_lst_Brucella_Declaratie_Formulier(
                                pProgId, pProgramId, filetype,
                                lBedrijfsnummer, DBhost, AgroUser, AgroPassword,
                                filePath, logfilePath, doConversion, Pbzs_ID);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Brucella_Inzend_Formulier", exc);
                        }
                        break;
                    case "Brucella_Instructie_Formulier":
                        try
                        {
                            Win32Lst_Brucella_Monitoring.pCallback doConversion = new Win32Lst_Brucella_Monitoring.pCallback(EventCallbackmethod);
                            string lBedrijfsnummer = addonArr[1];
                            int Pbzs_ID = Convert.ToInt32(addonArr[2]);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            logfilePath = logDir + fileNameWithoutExt + getExt(2); //log=txt

                            result = Win32Lst_Brucella_Monitoring.Init().call_lst_Brucella_Instructie_Formulier(
                                pProgId, pProgramId, filetype,
                                lBedrijfsnummer, DBhost, AgroUser, AgroPassword,
                                filePath, logfilePath, doConversion, Pbzs_ID);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Brucella_Inzend_Formulier", exc);
                        }
                        break;
                    case "LabelPrinter":
                        try
                        {
                            Win32Lst_LabelPrinter.pCallback doConversion = new Win32Lst_LabelPrinter.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);

                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSoortLijst;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            if (pSoortLijst == 1)
                            {
                                //Bedrijfslabel
                                int pFarmId = 0;
                                int.TryParse(addonArr[2], out pFarmId);

                                result = Win32Lst_LabelPrinter.Init().call_lst_AB_LabelPrinter_Bedrijf(
                                           pProgId, pProgramId, filetype,
                                           pUbnnr, DBhost, AgroUser, AgroPassword,
                                           filePath, logfilePath, doConversion,
                                           pFarmId);
                            }
                            else if (pSoortLijst == 2)
                            {
                                //Dierlabel
                                string pLifeNumber = addonArr[2];

                                result = Win32Lst_LabelPrinter.Init().call_lst_AB_LabelPrinter_Animal(
                                           pProgId, pProgramId, filetype,
                                           pUbnnr, DBhost, AgroUser, AgroPassword,
                                           filePath, logfilePath, doConversion,
                                           pLifeNumber);
                            }
                            else if (pSoortLijst == 3)
                            {
                                int pBedrijfZiekteId = 0;
                                int.TryParse(addonArr[2], out pBedrijfZiekteId);

                                int pAantalWeken = 0;
                                int.TryParse(addonArr[3], out pAantalWeken);

                                result = Win32Lst_LabelPrinter.Init().call_lst_AB_LabelPrinter_BedrijfZiekte(
                                           pProgId, pProgramId, filetype,
                                           pUbnnr, DBhost, AgroUser, AgroPassword,
                                           filePath, logfilePath, doConversion,
                                           pBedrijfZiekteId, pAantalWeken);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("LabelPrinter", exc);
                        }
                        break;
                    case "kruisingsoverzicht":
                    case "verwantschap":
                        try
                        {
                            Win32SELFATH.pCallback doConversion = new Win32SELFATH.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            string pGeselecteerdeBulls = addonArr[1];

                            bool pAlleenRelatie = true;
                            bool.TryParse(addonArr[2], out pAlleenRelatie);

                            bool pRelatieAanwezig = true;
                            bool.TryParse(addonArr[3], out pRelatieAanwezig);

                            if (soort == "kruisingsoverzicht")
                            {
                                result = Win32SELFATH.Init().call_lst_AB_Kruisingsoverzicht(
                                                            pProgId, pProgramId, filetype,
                                                            pUbnnr, DBhost, AgroUser, AgroPassword,
                                                            filePath, logfilePath, pResourceFolder, landcode, pCountryCode, doConversion,
                                                            pGeselecteerdeBulls, pAlleenRelatie);
                            }
                            else if (soort == "verwantschap")
                            {
                                result = Win32SELFATH.Init().call_lst_AB_Verwantschap(
                                                            pProgId, pProgramId, filetype,
                                                            pUbnnr, DBhost, AgroUser, AgroPassword,
                                                            filePath, logfilePath, pResourceFolder, landcode, pCountryCode, doConversion,
                                                            pGeselecteerdeBulls, pAlleenRelatie, pRelatieAanwezig);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32SELFATH error (" + soort + "): ", exc);
                        }

                        break;
                    case "Aflamlijst":
                        //BUG 1800  1888
                        try
                        {
                            Win32AFLAM.pCallback doConversion = new Win32AFLAM.pCallback(EventCallbackmethod);

                            //Addons
                            //0: filetype
                            //1: soort lijst
                            //2: Lamdetails
                            //3: totaaloverzicht
                            //4: alleenaanwezigemoeders
                            //5: sortering

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSoortLijst = 0;
                            int.TryParse(addonArr[1], out pSoortLijst);

                            int pLamDetails = 0;
                            int.TryParse(addonArr[2], out pLamDetails);

                            int pTotaaloverzicht = 0;
                            int.TryParse(addonArr[3], out pTotaaloverzicht);

                            int pAlleenAanwezigeMoeders = 0;
                            int.TryParse(addonArr[4], out pAlleenAanwezigeMoeders);

                            int pSortering = 0;
                            int.TryParse(addonArr[5], out pSortering);

                            int pPerHok = 0;
                            int.TryParse(addonArr[6], out pPerHok);
                            string pAlleenHok = addonArr[7].ToString();

                            result = Win32AFLAM.Init().call_lst_AB_Aflamlijst(pProgId, pProgramId, filetype, pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath, 
                                pResourceFolder, landcode, pCountryCode, pCountryCode,
                                doConversion,
                                pSoortLijst, pLamDetails, pTotaaloverzicht, begindatum, einddatum, pAlleenAanwezigeMoeders, pSortering,
                                pPerHok, pAlleenHok);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32AFLAM error (" + soort + "): ", exc);
                        }

                        break;
                    case "Gemprodooi":
                        //BUG 1888  LabId 53 labkind 6200
                        try
                        {
                            Win32GEMPROD.pCallback doConversion = new Win32GEMPROD.pCallback(EventCallbackmethod);

                            //Addons
                            //0: filetype
                            //1: pWorpnr
                            //2: pAflamperiode
                            //3: pPerRas
                            //4: pPerGroep
                            //5: pGrafieken
                            //6: pPerHok
                            //7: pAlleenHok
                            //8: pSortering


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);



                            int pWorpnr = 0;
                            int.TryParse(addonArr[1], out pWorpnr);

                            int pAflamperiode = 0;
                            int.TryParse(addonArr[2], out pAflamperiode);

                            int pPerRas = 0;
                            int.TryParse(addonArr[3], out pPerRas);

                            int pPerGroep = 0;
                            int.TryParse(addonArr[4], out pPerGroep);

                            int pGrafieken = 0;
                            int.TryParse(addonArr[5], out pGrafieken);

                            int pPerHok = 0;
                            int.TryParse(addonArr[6], out pPerHok);

                            string pAlleenHok = addonArr[7].ToString();

                            int pSortering = 0;
                            int.TryParse(addonArr[8], out pSortering);


                            result = Win32GEMPROD.Init().call_lst_AB_gemproduktielijst_ooi(pProgId, pProgramId, filetype,
                                pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath,
                                pResourceFolder, landcode, pCountryCode,
                                doConversion,
                                pWorpnr, pAflamperiode, begindatum, einddatum,
                                pPerRas, pPerGroep, pGrafieken, pPerHok,
                                pAlleenHok, pSortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32GEMPROD error (" + soort + "): ", exc);
                        }

                        break;
                    case "Gemprodlam":
                        //BUG 1888  LabId 54 labkind 6200
                        try
                        {
                            Win32GEMPROD.pCallback doConversion = new Win32GEMPROD.pCallback(EventCallbackmethod);

                            //Addons
                            //0: filetype
                            //1: pGroepnr
                            //2: pPerHok
                            //3: pAlleenHok
                            //4: pSortering


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pGroepnr = 0;
                            int.TryParse(addonArr[1], out pGroepnr);

                            int pPerHok = 0;
                            int.TryParse(addonArr[2], out pPerHok);

                            string pAlleenHok = addonArr[3].ToString();

                            int pSortering = 0;
                            int.TryParse(addonArr[4], out pSortering);



                            result = Win32GEMPROD.Init().call_lst_AB_gemproduktielijst_lam(pProgId, pProgramId, filetype,
                                pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath,
                                pResourceFolder, landcode, pCountryCode,
                                doConversion,
                                pGroepnr, pPerHok,
                                pAlleenHok, pSortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32GEMPROD error (" + soort + "): ", exc);
                        }

                        break;
                    case "Drachtcontrolelijst":
                        //BUG 1802
                        try
                        {
                            Win32DRACHTPE.pCallback doConversion = new Win32DRACHTPE.pCallback(EventCallbackmethod);

                            //Addons
                            //0: filetype
                            //1: alleen status
                            //2: Alleen aanwezig
                            //3: hoknr
                            //4: sortering

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pAlleenStatus = 0;
                            int.TryParse(addonArr[1], out pAlleenStatus);

                            int pAlleenAanwezig = 0;
                            int.TryParse(addonArr[2], out pAlleenAanwezig);

                            string pHoknr = addonArr[3];

                            int pSortering = 0;
                            int.TryParse(addonArr[4], out pSortering);

                            result = Win32DRACHTPE.Init().call_lst_AB_drachtcontrolelijst(pProgId, pProgramId, filetype, pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath,
                                pResourceFolder, landcode, pCountryCode, doConversion,
                                pAlleenStatus, pAlleenAanwezig, pHoknr, pSortering, begindatum, einddatum);



                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32SELFATH error (" + soort + "): ", exc);
                        }
                        break;

                    case "Samenweidingslijst":
                        //BUG 1799
                        try
                        {
                            Win32GROUPGRZ.pCallback doConversion = new Win32GROUPGRZ.pCallback(EventCallbackmethod);

                            //Addons
                            //0: Filetype
                            //1: Jaar
                            //2: Periode
                            //3: InclAfgevoerd
                            //4: AniIdFather
                            //5: Sortering    

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            int Jaar = 0;
                            int.TryParse(addonArr[1], out Jaar);

                            int Periode = 0;
                            int.TryParse(addonArr[2], out Periode);

                            int InclAfgevoerd = 0;
                            int.TryParse(addonArr[3], out InclAfgevoerd);

                            int AniIdFather = 0;
                            int.TryParse(addonArr[4], out AniIdFather);

                            int Sortering = 0;
                            int.TryParse(addonArr[5], out Sortering);

                            result = Win32GROUPGRZ.Init().call_lst_AB_GrazingTogetherList(pProgId, pProgramId, filetype, pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath,
                                pResourceFolder, landcode, pCountryCode,
                                doConversion,
                                Jaar, Periode, begindatum, einddatum, InclAfgevoerd, AniIdFather, Sortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32GROUPGRZ error (" + soort + "): ", exc);
                        }
                        break;
                    case "Droogzetlijst":
                        //BUG 1803
                        try
                        {
                            Win32DROOGZET.pCallback doConversion = new Win32DROOGZET.pCallback(EventCallbackmethod);

                            //0: Filetype
                            //1: Selectie
                            //2: AlleenNuDroog
                            //3: Sortering  

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int Selectie = 0;
                            int.TryParse(addonArr[1], out Selectie);

                            int AlleenNuDroog = 0;
                            int.TryParse(addonArr[2], out AlleenNuDroog);

                            int Sortering = 0;
                            int.TryParse(addonArr[3], out Sortering);

                            result = Win32DROOGZET.Init().call_lst_AB_Droogzetlijst(pProgId, pProgramId, filetype, pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, Selectie, AlleenNuDroog, Sortering, begindatum, einddatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32DROOGZET error (" + soort + "): ", exc);
                        }
                        break;
                    case "Sponslijst":
                        //BUG 1804
                        try
                        {
                            Win32SPONSLSG.pCallback doConversion = new Win32SPONSLSG.pCallback(EventCallbackmethod);

                            //0: Filetype
                            //1: Sortering  

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int Sortering = 0;
                            int.TryParse(addonArr[1], out Sortering);

                            result = Win32SPONSLSG.Init().call_lst_AB_Sponslijst(pProgId, pProgramId, filetype, pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, Sortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32SPONSLSG error (" + soort + "): ", exc);
                        }
                        break;
                    case "Drachtanalyse":
                        //BUG 1801
                        try
                        {
                            Win32DRACHTPE.pCallback doConversion = new Win32DRACHTPE.pCallback(EventCallbackmethod);

                            //0: Filetype
                            //1: Sortering  

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            result = Win32DRACHTPE.Init().call_lst_AB_drachtanalyse(pProgId, pProgramId, filetype, pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, begindatum, einddatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32DRACHTPE error (" + soort + "): ", exc);
                        }
                        break;
                    case "Kengetallst":
                        //BUG 1891                                                
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pJaar = 0;
                            int.TryParse(addonArr[1], out pJaar);


                            if (pProgramId == 22)
                            {
                                int FarmIdG = 0;
                                int.TryParse(addonArr[2], out FarmIdG);

                                BEDRIJF bedrijf = lMstb.GetBedrijfById(FarmIdG);
                                UBN ubn = lMstb.GetubnById(bedrijf.UBNid);

                                int iProgramId = bedrijf.Programid;
                                int iProgId = bedrijf.ProgId;
                                String iUbnnr = ubn.Bedrijfsnummer;

                                pProgramId = iProgramId;
                                pProgId = iProgId;
                                pUbnnr = iUbnnr;
                            }
                            else
                            {
                            }

                            Win32KengetallenlijstNSFO.pCallback doConversion = new Win32KengetallenlijstNSFO.pCallback(EventCallbackmethod);

                            result = Win32KengetallenlijstNSFO.Init().call_lst_AB_KengetallenNSFO(
                                                                      pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                        pJaar);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32KengetallenlijstNSFO error (" + soort + "): ", exc);
                        }

                        break;
                    case "Kengetallijstnsfostamboek":

                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pDdlProgramId = 0;
                            int.TryParse(addonArr[1], out pDdlProgramId);


                            string pGebied = addonArr[2];

                            int pJaar = 0;
                            int.TryParse(addonArr[3], out pJaar);

                            if (pDdlProgramId == 40 || pDdlProgramId == 47)
                            {
                                pProgId = 5;
                            }
                            else { pProgId = 3; }

                            Win32KengetallenlijstNSFO.pCallback doConversion = new Win32KengetallenlijstNSFO.pCallback(EventCallbackmethod);

                            result = Win32KengetallenlijstNSFO.Init().call_lst_AB_kengetallenNSFO_stamboek(
                                                                      pProgId, pDdlProgramId, filetype,
                                                                      DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      pGebied, pJaar);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32KengetallenlijstNSFO error (" + soort + "): ", exc);
                        }

                        break;
                    case "Kengetallijstgeiten":

                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pMaand = 0;
                            int.TryParse(addonArr[1], out pMaand);
                            if (pMaand == 0) { pMaand = 1; }
                            int pJaar = 0;
                            int.TryParse(addonArr[2], out pJaar);
                            if (pJaar == 0) { pJaar = DateTime.Now.Year; }

                            int FarmIdG = 0;
                            int.TryParse(addonArr[3], out FarmIdG);
                            UBN ubn = new UBN();

                            if (FarmIdG > 0)
                            {
                                BEDRIJF bedrijf = lMstb.GetBedrijfById(FarmIdG);
                                pProgramId = bedrijf.Programid;
                                ubn = lMstb.GetubnById(bedrijf.UBNid);
                            }
                            else
                            {
                                ubn.Bedrijfsnummer = u.Bedrijfsnummer;
                            }

                            Win32kengetalGeiten.pCallback doConversion = new Win32kengetalGeiten.pCallback(EventCallbackmethod);

                            result = Win32kengetalGeiten.Init().call_lst_AB_kengetallenGeiten(
                                                                      pProgId, pProgramId, filetype, ubn.Bedrijfsnummer,
                                                                      DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      pMaand, pJaar, 0, 0);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32KengetallenlijstGeit error (" + soort + "): ", exc);
                        }

                        break;
                    case "Afstamlijst":
                        //bug 1903
                        try
                        {
                            Win32Afstamlijst.pCallback doConversion = new Win32Afstamlijst.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int selectie = 0;
                            int.TryParse(addonArr[1], out selectie);

                            int grootvaders = 0;
                            int.TryParse(addonArr[2], out grootvaders);

                            int responders = 0;
                            int.TryParse(addonArr[3], out responders);

                            string ubnnrBestemming = addonArr[4];
                            string pHoknr = addonArr[5];

                            int sortering = 0;
                            int.TryParse(addonArr[6], out sortering);

                            result = Win32Afstamlijst.Init().call_lst_AB_Afstamlijst(
                                                                      pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, 
                                                                      pResourceFolder, landcode, pCountryCode, pLandnr, 
                                                                      doConversion, selectie, grootvaders, responders,
                                                                      ubnnrBestemming, begindatum, einddatum,
                                                                      pHoknr, sortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32Afstamlijst error (" + soort + "): ", exc);
                        }

                        break;
                    case "Behandelplannen":
                        //BUG 1890
                        try
                        {
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32Behandelplannen.pCallback doConversion = new Win32Behandelplannen.pCallback(EventCallbackmethod);

                            result = Win32Behandelplannen.Init().call_lst_AB_Behandelplannen(
                                                                      pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, pResourceFolder, landcode, pCountryCode, doConversion);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32Behandelplannen error (" + soort + "): ", exc);
                        }
                        break;
                    case "GroepsPrestatie":
                        //BUG 1889
                        try
                        {
                            //filetype + dl + pAanwezigheid + dl + pAflamperiode + dl + pPerHok + dl + pAlleenHok + dl + pSortering;
                            Win32GroepPrestatie.pCallback doConversion = new Win32GroepPrestatie.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pAanwezigheid = 0;
                            int.TryParse(addonArr[1], out pAanwezigheid);
                            int pAflamperiode = 0;
                            int.TryParse(addonArr[2], out pAflamperiode);
                            int pPerHok = 0;
                            int.TryParse(addonArr[3], out pPerHok);

                            string pAlleenHok = addonArr[4];
                            int pSortering = 0;
                            int.TryParse(addonArr[5], out pSortering);


                            result = Win32GroepPrestatie.Init().call_lst_AB_groepprestatie(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath,
                                                                      pResourceFolder, landcode, pCountryCode,
                                                                      doConversion, pAanwezigheid,
                                                                      pAflamperiode, begindatum, einddatum, pPerHok, pAlleenHok, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Win32GroepsPrestatie error (" + soort + "): ", exc);
                        }
                        break;
                    case "Vervoersdocument":
                        //BUG 1945
                        try
                        {
                            Win32VERVOERD.pCallback doConversion = new Win32VERVOERD.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            DateTime pTransportDatumTijd = begindatum;
                            string start = addonArr[1].ToString();
                            char[] spl = { ':' };
                            string[] startuur = start.Split(spl);
                            try
                            {
                                double uur1 = 0;
                                double.TryParse(startuur[0].ToString(), out uur1);
                                pTransportDatumTijd = pTransportDatumTijd.AddHours(uur1);
                                double min1 = 0;
                                double.TryParse(startuur[1].ToString(), out min1);
                                pTransportDatumTijd = pTransportDatumTijd.AddMinutes(min1);
                            }
                            catch { }
                            DateTime pTransportDuur = DateTime.Now.Date;

                            string duur = addonArr[2].ToString();
                            string[] einduur = duur.Split(spl);
                            try
                            {
                                double uur2 = 0;
                                double.TryParse(einduur[0].ToString(), out uur2);
                                pTransportDuur = pTransportDuur.AddHours(uur2);
                                double min2 = 0;
                                double.TryParse(einduur[1].ToString(), out min2);
                                pTransportDuur = pTransportDuur.AddMinutes(min2);
                            }
                            catch { }

                            int pThrIdBestemming = 0;
                            int.TryParse(addonArr[3], out pThrIdBestemming);
                            int pUBNidBestamming = 0;
                            int.TryParse(addonArr[4], out pUBNidBestamming);
                            int pThrIdTransporteur = 0;
                            int.TryParse(addonArr[5], out pThrIdTransporteur);
                            int pUBNidTransporteur = 0;
                            int.TryParse(addonArr[6], out pUBNidTransporteur);
                            int pIdTransportmiddel = 0;
                            int.TryParse(addonArr[7], out pIdTransportmiddel);
                            //pAniIds moet zijn 23,43,224,4442, door komma's gescheiden

                            string pAniIds = addonArr[8].ToString();

                            if (t.ThrCountry == "58")
                            {
                                result = Win32VERVOERD.Init().call_lst_AB_VervoersdocumentSG_DK(pProgId, pProgramId, filetype,
                                                                                                    pUbnnr, DBhost, AgroUser, AgroPassword, filePath, logfilePath,
                                                                                                    pResourceFolder, landcode, pCountryCode,
                                                                                                    doConversion,
                                                                                                    pUbnnr, pTransportDatumTijd,
                                                                                                    pAniIds);
                            }
                            else
                            {
                                result = Win32VERVOERD.Init().call_lst_AB_VervoersdocumentSG(pProgId, pProgramId, filetype,
                                                                          pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                          filePath, logfilePath, doConversion,
                                                                          pTransportDatumTijd, pTransportDuur,
                                                                          pThrIdBestemming, pUBNidBestamming,
                                                                          pThrIdTransporteur, pUBNidTransporteur,
                                                                          pIdTransportmiddel, pAniIds);
                            }
                            
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Vervoersdocument error (" + soort + "): ", exc);
                        }
                        break;
                    case "Aanvoerlijst":
                        try
                        {
                            Win32VZPLISTS.pCallback doConversion = new Win32VZPLISTS.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pThridLeverancier = 0;
                            int.TryParse(addonArr[1], out pThridLeverancier);

                            int pSortering = 0;
                            int.TryParse(addonArr[2], out pSortering);

                            result = Win32VZPLISTS.Init().call_lst_AB_Aanvoerlijst(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      begindatum, einddatum, pThridLeverancier, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Aanvoerlijs error (" + soort + "): ", exc);
                        }
                        break;
                    case "Afvoerlijst":
                        try
                        {
                            Win32VZPLISTS.pCallback doConversion = new Win32VZPLISTS.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            //filetype + dl + pAfnemer + dl + pSortering + dl + pMovKind + dl + FarmId + dl + UbnId
                            int pThridAfnemer = 0;
                            int.TryParse(addonArr[1], out pThridAfnemer);

                            int pSortering = 0;
                            int.TryParse(addonArr[2], out pSortering);


                            int pMovkind = 0;
                            int.TryParse(addonArr[3], out pMovkind);

                            int lFarmId = 0;
                            int.TryParse(addonArr[4], out lFarmId);
                            int lUbnId = 0;
                            int.TryParse(addonArr[5], out lUbnId);


                            int pMovkind2 = 0;
                            int pThridAfnemer2 = 0;
                            int pThirdTransporteur2 = 0;
                            int pTransportid2 = 0;
                            DateTime pDatum2 = DateTime.MinValue; ;// begindatum;
                            if (pMovkind == 2 &&   lUbnId > 0)
                            {
                              
                                List<MOVEMENT> afvoeren = lMstb.getMovementsByMovThird_UbnAndKind(lUbnId, 2);
                                //List<MUTATION> nietgemelde = lMstb.GetMutationsByUbn(lUbnId);
                                //List<MOVEMENT> UniekeAfvDatums = afvoeren.Distinct(new MoveDateComparer()).ToList();
                                var selectie = (from n in afvoeren
                                                where n.MovDate.Date == begindatum.Date
                                                select n.MovId).ToList();
                                if (selectie.Count() > 0)
                                {
                                    List<SALE> verkopen = lMstb.GetSales(selectie);
                                    var heeftdata = from n in verkopen
                                                    where n.SalReason > 0
                                                    select n;
                                    if (heeftdata.Count() > 0)
                                    {
                                        pMovkind2 = 2;//Kan ook 3 zijn of vehuur 6
                                        pThridAfnemer2 = heeftdata.ElementAt(0).SalDestination;
                                        pThirdTransporteur2 = heeftdata.ElementAt(0).SalTransporter;
                                        pTransportid2 = heeftdata.ElementAt(0).SalTransportID;
                                        pThridAfnemer = 0;
                                        pDatum2 = begindatum;
                                        begindatum = DateTime.MinValue;
                                        einddatum = DateTime.MinValue;
                                    }
                                }
                            }

                            result = Win32VZPLISTS.Init().call_lst_AB_Afvoerlijst(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath,
                                                                      pResourceFolder, landcode, pCountryCode,
                                                                      doConversion,
                                                                      begindatum, einddatum, pThridAfnemer,
                                                                      pDatum2, pMovkind2, pThridAfnemer2,
                                                                      pThirdTransporteur2, pTransportid2, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Afvoerlijs error (" + soort + "): ", exc);
                        }
                        break;
                    case "Leeftijdlijst":
                        try
                        {
                            Win32Leeftijd.pCallback doConversion = new Win32Leeftijd.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);

                            int pGroepid = 0;
                            int.TryParse(addonArr[2], out pGroepid);
                            int pStalid = 0;
                            int.TryParse(addonArr[3], out pStalid);
                            int pAfdelingid = 0;
                            int.TryParse(addonArr[4], out pAfdelingid);

                            string pHoktext = addonArr[5];

                            result = Win32Leeftijd.Init().call_lst_AB_Leeftijdlijst(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      begindatum, einddatum,
                                                                      pGroepid, pStalid, pAfdelingid,
                                                                      pHoktext, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Leeftijdlijs error (" + soort + "): ", exc);
                        }
                        break;
                    case "Locatielijst":
                        try
                        {
                            Win32LOCATIE.pCallback doConversion = new Win32LOCATIE.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);
                            int pAanwezigheid = 0;
                            int.TryParse(addonArr[2], out pAanwezigheid);
                            int pAlleenLaatsteLocatie = 0;
                            int.TryParse(addonArr[3], out pAlleenLaatsteLocatie);



                            int pInPeriode = 0;
                            int.TryParse(addonArr[4], out pInPeriode);
                            int pSoortLocatie = 0;
                            int.TryParse(addonArr[5], out pSoortLocatie);

                            int pGroepid = 0;
                            int.TryParse(addonArr[6], out pGroepid);
                            int pStalId = 0;
                            int.TryParse(addonArr[7], out pStalId);
                            int pAfdelingid = 0;
                            int.TryParse(addonArr[8], out pAfdelingid);
                            string pHokVanText = addonArr[9];
                            string pHokTotText = addonArr[10];

                            if (pInPeriode == 0)
                            {
                                begindatum = DateTime.MinValue;
                                einddatum = DateTime.MinValue;
                                if (pSoortLocatie == 1)
                                {
                                    pStalId = 0;
                                    pAfdelingid = 0;
                                    pHokTotText = "";
                                    pHokVanText = "";
                                }
                                else
                                {
                                    pGroepid = 0;
                                }
                            }
                            else
                            {
                                pStalId = 0;
                                pAfdelingid = 0;
                                pHokTotText = "";
                                pHokVanText = "";
                                pGroepid = 0;
                            }




                            int pFarmId = 0; ;
                            int.TryParse(addonArr[11], out pFarmId);
                            List<REMARK> opmerkingen = lMstb.getFarmRemarks(pFarmId);



                            result = Win32LOCATIE.Init().call_lst_AB_Locatielijst(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath,
                                                                      pResourceFolder, landcode, pCountryCode,
                                                                      doConversion,
                                                                      pInPeriode, begindatum, einddatum,
                                                                      pAlleenLaatsteLocatie, pAanwezigheid, pSoortLocatie, pGroepid,
                                                                      pStalId, pAfdelingid,
                                                                      pHokVanText, pHokTotText, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Aanvoerlijs error (" + soort + "): ", exc);
                        }
                        break;
                    case "Statuslijst":
                        try
                        {
                            Win32STATUSLS.pCallback doConversion = new Win32STATUSLS.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);

                            int pStatus = 0;
                            int.TryParse(addonArr[2], out pStatus);
                            int pAlleenLaatsteStatus = 0;
                            int.TryParse(addonArr[3], out pAlleenLaatsteStatus);
                            int pInclAfgevoerd = 0;
                            int.TryParse(addonArr[4], out pInclAfgevoerd);


                            result = Win32STATUSLS.Init().call_lst_AB_Statuslijst(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath,
                                                                      pResourceFolder, landcode, pCountryCode,
                                                                      doConversion,
                                                                      begindatum, einddatum,
                                                                      pStatus, pAlleenLaatsteStatus, pInclAfgevoerd, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Aanvoerlijs error (" + soort + "): ", exc);
                        }
                        break;
                    case "Medicijngebruikperkoppel":
                        try
                        {
                            //filetype + dl + pSoortlijst + dl + pGroepid + dl + pDetails1 + dl + pGrafieken1;
                            Win32LSTKOPBH.pCallback doConversion = new Win32LSTKOPBH.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            int pSoortlijst = 0;
                            int.TryParse(addonArr[1], out pSoortlijst);
                            int pGroepid = 0;
                            int.TryParse(addonArr[2], out pGroepid);
                            int pDetails1 = 0;
                            int.TryParse(addonArr[3], out pDetails1);
                            int pGrafieken1 = 0;
                            int.TryParse(addonArr[3], out pGrafieken1);

                            result = Win32LSTKOPBH.Init().call_lst_AB_BehandelingenPerKoppel(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      pGroepid, pSoortlijst, pDetails1, pGrafieken1);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Medicijngebruikperkoppel error (" + soort + "): ", exc);
                        }
                        break;
                    case "Registratiekaart":
                        try
                        {
                            //filetype + dl + pAniid;
                            Win32INSKAART.pCallback doConversion = new Win32INSKAART.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            int pAniid = 0;
                            int.TryParse(addonArr[1], out pAniid);



                            result = Win32INSKAART.Init().call_lst_AB_Registratiekaart(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      pAniid);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Registratiekaart error (" + soort + "): ", exc);
                        }
                        break;
                    case "Levensproductie":
                        try
                        {

                            Win32LEVPROD.pCallback doConversion = new Win32LEVPROD.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            int pInclAfgevoerd = 0;
                            int.TryParse(addonArr[1], out pInclAfgevoerd);
                            int pToonDieren = 0;
                            int.TryParse(addonArr[2], out pToonDieren);
                            int pSortering = 0;
                            int.TryParse(addonArr[3], out pSortering);



                            result = Win32LEVPROD.Init().call_lst_AB_Levensproduktielijst(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath, doConversion,
                                                                      begindatum, einddatum, pInclAfgevoerd, pToonDieren, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Levensproductie error (" + soort + "): ", exc);
                        }
                        break;
                    case "Medicijnkaart":
                        try
                        {
                            //UITZONDERING // usually used '-' but caused a conflict with date format xx-xx-xxxx
                            char[] tmpSplit = { '+' };
                            addonArr = addons.Split(tmpSplit);

                            Win32Medicijnkaart.pCallback doConversion = new Win32Medicijnkaart.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            int pGroepnr = 0;
                            int.TryParse(addonArr[1], out pGroepnr);
                            DateTime pAfvoerdat = DateTime.MinValue;
                            try
                            {
                                if (addonArr[2] != "")
                                {
                                    pAfvoerdat = utils.getDate(addonArr[2]);
                                }
                            }
                            catch { }
                            int pThridBestemming = 0;
                            int.TryParse(addonArr[3], out pThridBestemming);
                            string pHoknr = addonArr[4];

                            int pGeregistreerdeMedicijnen = 0;
                            int.TryParse(addonArr[5], out pGeregistreerdeMedicijnen);
                            int pNietGeregistreerdeMedicijnen = 0;
                            int.TryParse(addonArr[6], out pNietGeregistreerdeMedicijnen);
                            int pExclKoppelBehandelingen = 0;
                            int.TryParse(addonArr[7], out pExclKoppelBehandelingen);
                            int pSortering = 0;
                            int.TryParse(addonArr[8], out pSortering);

                            result = Win32Medicijnkaart.Init().call_lst_AB_Medicijnregistratiekaart(pProgId, pProgramId, filetype,
                                                                      pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                      filePath, logfilePath,
                                                                      pResourceFolder, landcode, pCountryCode,
                                                                      doConversion,
                                                                      begindatum, einddatum, pGroepnr, pAfvoerdat,
                                                                      pThridBestemming, pHoknr, pGeregistreerdeMedicijnen, pNietGeregistreerdeMedicijnen,
                                                                      pExclKoppelBehandelingen, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Medicijnkaart error (" + soort + "): ", exc);
                        }
                        break;
                    case "geboortebewijs":
                        try
                        {
                            Win32lstabgebgew.pCallback doConversion = new Win32lstabgebgew.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);

                            string pRavBestand = serverMainDir + "lib\\GEBBEW.RAV";
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int lAniId = 0;
                            string Lifenumber = "";
                            if (addonArr[2] != "")
                            {
                                Lifenumber = addonArr[2];
                            }
                            else if (addonArr[1] != "")
                            {
                                int.TryParse(addonArr[1], out lAniId);
                                ANIMAL lAnimal = lMstb.GetAnimalById(lAniId);
                                Lifenumber = lAnimal.AniAlternateNumber;
                            }

                            result = Win32lstabgebgew.Init().call_lst_AB_Geboortebewijs(pProgId, pProgramId, filetype,
                                                                    pUbnnr, DBhost, AgroUser, AgroPassword,
                                                                    filePath, logfilePath, doConversion,
                                                                    pRavBestand,
                                                                    Lifenumber);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("geboortebewijs error (" + soort + "): ", exc);
                        }
                        break;
                    case "Fokwaardenbestand":
                        try
                        {
                            Win32Abfokwaarden.pCallback doConversion = new Win32Abfokwaarden.pCallback(EventCallbackmethod);
                            int.TryParse(addonArr[0], out filetype);


                            int FarmId = 0;
                            int.TryParse(addonArr[1], out FarmId);

                            BEDRIJF b = new BEDRIJF();
                            UBN ubn = new UBN();
                            THIRD tr = new THIRD();
                            COUNTRY c = new COUNTRY();
                            lMstb.getCompanyByFarmId(FarmId, out b, out ubn, out tr, out c);
                            filePath = outputDir + "abgeitfw" + ubn.Bedrijfsnummer + ".csv";
                            if (File.Exists(filePath))
                            {
                                try
                                {
                                    File.Delete(filePath);
                                }
                                catch { }
                            }
                            string filePath2 = outputDir + "abbokfw" + ubn.Bedrijfsnummer + ".csv";
                            if (File.Exists(filePath2))
                            {
                                try
                                {
                                    File.Delete(filePath2);
                                }
                                catch { }
                            }
                            string filePath3 = outputDir + "abfwgrenzen" + ubn.Bedrijfsnummer + ".csv";
                            if (File.Exists(filePath3))
                            {
                                try
                                {
                                    File.Delete(filePath3);
                                }
                                catch { }
                            }
                            result = Win32Abfokwaarden.Init().call_lst_AB_maakFokwaardenBestandGeiten(b.ProgId, b.Programid, ubn.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, logfilePath, doConversion, outputDir, begindatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Fokwaardenbestand error (" + soort + "): ", exc);
                        }
                        break;
                    case "fokwaarden":
                        try
                        {
                            Win32Fokwaard.pCallback doConversion = new Win32Fokwaard.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int selectie = 0;
                            int.TryParse(addonArr[1], out selectie);
                            int lSoort = 0;
                            int.TryParse(addonArr[2], out lSoort);

                            string pHok = addonArr[3];
                            string pCsvKolommen = addonArr[4];

                            int pSortering = 1;
                            if (lSoort == 1)
                            { pSortering = 0; }

                            int AniIdFW = 0;
                            int.TryParse(addonArr[5], out AniIdFW);
                            result = Win32Fokwaard.Init().call_lst_AB_Fokwaarden(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion,begindatum, selectie, lSoort, AniIdFW, pHok, pCsvKolommen, pSortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("fokwaarden error (" + soort + "): ", exc);
                        }
                        break;
                    case "Restlijst":
                        try
                        {
                            Win32RestList.pCallback doConversion = new Win32RestList.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            //    pMinKgRestVoerX100, pMinPercRestvoer,
                            //    pMinPercAfwMelk, p9voersoorten, pSubtotalen,
                            //    pAantDecimalenVoer
                            //    pAlleenHok
                            int pMinKgRestVoerX100 = 0;
                            int pMinPercRestvoer = 0;
                            int pMinPercAfwMelk = 0;
                            int p9voersoorten = 0;
                            int pSubtotalen = 0;
                            int pAantDecimalenVoer = 0;
                            int pAlleenDierenMetVoer = 0;

                            int.TryParse(addonArr[1], out pMinKgRestVoerX100);
                            int.TryParse(addonArr[2], out pMinPercRestvoer);
                            int.TryParse(addonArr[3], out pMinPercAfwMelk);
                            int.TryParse(addonArr[4], out p9voersoorten);
                            int.TryParse(addonArr[5], out pSubtotalen);
                            int.TryParse(addonArr[6], out pAantDecimalenVoer);
                            int.TryParse(addonArr[7], out pAlleenDierenMetVoer);
                            string pAlleenHok = addonArr[8];
                            if (pAlleenHok == "null") { pAlleenHok = ""; }
                            if (pSubtotalen != 1) { pAlleenHok = ""; }
                            result = Win32RestList.Init().call_lst_AB_Restlijst(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion,
                                    begindatum, pMinKgRestVoerX100, pMinPercRestvoer, pMinPercAfwMelk, p9voersoorten, pSubtotalen, pAantDecimalenVoer, pAlleenDierenMetVoer, pAlleenHok);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Restlijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Ketoattentie":

                        try
                        {
                            Win32RestList.pCallback doConversion = new Win32RestList.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            //int lCountryCode = 0;
                            //try
                            //{
                            //    int.TryParse(addonArr[1], out lCountryCode);
                            //}
                            //catch { }
                            //string landcode = lMstb.GetCountryByLandNummer(lCountryCode).LandAfk2;

                            result = Win32RestList.Init().call_lst_AB_KetoAttentie(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath,pResourceFolder,landcode,pCountryCode, doConversion,
                                    begindatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Ketoattentie error (" + soort + "): ", exc);
                        }
                        break;
                    case "Voerboxbezoeken":
                        try
                        {
                            Win32RestList.pCallback doConversion = new Win32RestList.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            //int lCountryCode = 0;
                            //try
                            //{
                            //    int.TryParse(addonArr[1], out lCountryCode);
                            //}
                            //catch { }
                            //string landcode = lMstb.GetCountryByLandNummer(lCountryCode).LandAfk2;
                            int pSortering = 0;
                            int.TryParse(addonArr[2], out pSortering);
                            int pAantaldagen = 0;
                            int.TryParse(addonArr[3], out pAantaldagen);
                            result = Win32RestList.Init().call_lst_AB_VoerboxBezoeken(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, pResourceFolder, landcode, pCountryCode, doConversion, begindatum, pSortering, pAantaldagen);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Voerboxbezoeken error (" + soort + "): ", exc);
                        }
                        break;
                    case "Supplementverstrekking":
                        try
                        {
                            Win32RestList.pCallback doConversion = new Win32RestList.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            //int lCountryCode = 0;
                            //try
                            //{
                            //    int.TryParse(addonArr[1], out lCountryCode);
                            //}
                            //catch { }
                            //string landcode = lMstb.GetCountryByLandNummer(lCountryCode).LandAfk2;
                            int pSortering = 0;
                            int.TryParse(addonArr[2], out pSortering);
                            result = Win32RestList.Init().call_lst_AB_Supplementverstrekking(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath,pResourceFolder,landcode,pCountryCode, doConversion, begindatum, pSortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Supplementverstrekking error (" + soort + "): ", exc);
                        }
                        break;
                    case "AttentielijstMelkZoog":
                        try
                        {
                            Win32Attentie.pCallback doConversion = new Win32Attentie.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pAttLijstnr = 0;//agrofactuur.ATTINST.Internanr
                            int.TryParse(addonArr[1], out pAttLijstnr);
                            //pAttLijstnr moet groter zijn dan 0, anders wordt er niets gevonden door de dll
                            result = Win32Attentie.Init().call_lst_AB_AttentieLijst(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pAttLijstnr);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Attentielijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Overzichtdiertelling":
                        try
                        {
                            Win32Anilist.pCallback doConversion = new Win32Anilist.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSoort = 0;
                            int.TryParse(addonArr[1], out pSoort);
                            if (pSoort == 0)
                            {
                                DateTime lTemp = einddatum;
                                einddatum = begindatum;
                                begindatum = lTemp;
                            }
                            result = Win32Anilist.Init().call_lst_AB_OverzichtDiertelling(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pSoort, begindatum, einddatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Overzichtdiertelling error (" + soort + "): ", exc);
                        }
                        break;
                    case "Conditiescore":
                        try
                        {
                            Win32GROUPSCR.pCallback doConversion = new Win32GROUPSCR.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            if (einddatum < begindatum)
                            {
                                DateTime lTemp = einddatum;
                                einddatum = begindatum;
                                begindatum = lTemp;
                            }
                            int pToondieren = 0;
                            int.TryParse(addonArr[1], out pToondieren);

                            int pCurve = 0;
                            int.TryParse(addonArr[2], out pCurve);

                            result = Win32GROUPSCR.Init().call_lst_AB_Conditiescorelijst(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pToondieren, pCurve);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Conditiescorelijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Klauwscore":
                        try
                        {
                            Win32GROUPSCR.pCallback doConversion = new Win32GROUPSCR.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            if (einddatum < begindatum)
                            {
                                DateTime lTemp = einddatum;
                                einddatum = begindatum;
                                begindatum = lTemp;
                            }
                            int pToondieren = 0;
                            int.TryParse(addonArr[1], out pToondieren);

                            int pCurve = 0;
                            int.TryParse(addonArr[2], out pCurve);

                            result = Win32GROUPSCR.Init().call_lst_AB_Klauwscorelijst(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pToondieren, pCurve);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Klauwscore error (" + soort + "): ", exc);
                        }
                        break;
                    case "InteeltCoefficient":
                        try
                        {
                            Win32SELFATH.pCallback doConversion = new Win32SELFATH.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pFarmid = 0;
                            int.TryParse(addonArr[1], out pFarmid);

                            int pBullid = 0;
                            int.TryParse(addonArr[2], out pBullid);

                            int pAlleDieren = 0;
                            int.TryParse(addonArr[3], out pAlleDieren);
                            string pAniids = addonArr[4];

                            if (pAlleDieren > 0)
                            {
                                pAniids = "";
                                DataTable dt = lMstb.getGroepsGewijzeDeklijstSchaap(pFarmid, 0, 0, "AniWorknumber");
                                foreach (DataRow rw in dt.Rows)
                                {
                                    pAniids = pAniids + rw["AniId"].ToString() + ";";

                                }
                            }

                          
                            result = Win32SELFATH.Init().call_lst_AB_InteeltCoefficient(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                    DBhost, AgroUser, AgroPassword, filePath, logfilePath, pResourceFolder, landcode, pCountryCode, doConversion, pBullid, pAniids);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("InteeltCoefficient error (" + soort + "): ", exc);
                        }
                        break;
                    case "ab2xml":
                        try
                        {
                            Win32ab2xml.pCallback doConversion = new Win32ab2xml.pCallback(EventCallbackmethod);


                            filePath = outputDir + fileNameWithoutExt + getExt(5);
                            try 
                            {
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }
                            }
                            catch { }
                            //string pFarm = addonArr[1];
                        
                            string pFarmHerkomst = addonArr[2];
                        
                            int pInclmelkgegevens = 0;
                            int.TryParse(addonArr[3], out pInclmelkgegevens);
                            //int diersoort = 5;
                            //int.TryParse(addonArr[4], out diersoort);
                            if (u.Bedrijfsnummer != "")
                            {
                                //UBN uFarm = lMstb.getUBNByBedrijfsnummer(pFarm);

                                //List<BEDRIJF> bedrijven = lMstb.getBedrijvenByUBNId(uFarm.UBNid);
                                ////bepaal programid
                                //var s = from n in bedrijven where n.ProgId == diersoort select n;
                                //int programid = 7;
                                //if (s.Count() > 0)
                                //{
                                //    programid = s.ElementAt(0).Programid;
                                //}
                                result = Win32ab2xml.Init().call_AB_aangekochteGeitenProductieXML(pProgId, pProgramId, u.Bedrijfsnummer,
                                       DBhost, AgroUser, AgroPassword, logfilePath, doConversion, filePath, begindatum, einddatum, pFarmHerkomst, pInclmelkgegevens);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("ab2xml error (" + soort + "): ", exc);
                        }
                        break;
                    case "Stamboom":
                        try
                        {
                            Win32Stamboom.pCallback doConversion = new Win32Stamboom.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pAniId = 0;
                            int.TryParse(addonArr[1], out pAniId);
                            int pFarmid = 0;
                            int.TryParse(addonArr[2], out pFarmid);

                            if (pFarmid > 0 && pAniId > 0)
                            {
                                BEDRIJF b = lMstb.GetBedrijfById(pFarmid);
                                u = lMstb.GetubnById(b.UBNid);
                                int pInclmelkgegevens = 0;
                                int.TryParse(addonArr[2], out pInclmelkgegevens);


                                result = Win32Stamboom.Init().call_lst_AB_Stamboom(b.ProgId, b.Programid, filetype, u.Bedrijfsnummer,
                                       DBhost, AgroUser, AgroPassword, filePath, logfilePath, pResourceFolder, landcode, pCountryCode,doConversion, pAniId);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Stamboom error (" + soort + "): ", exc);
                        }
                        break;
                    case "Hoklijst":
                        try
                        {
                            Win32Hok.pCallback doConversion = new Win32Hok.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSoortlijst = 0;
                            int.TryParse(addonArr[1], out pSoortlijst);

                            string pHoknr = addonArr[2];

                            string pHoknummers = addonArr[3];
                            if (pSoortlijst != 3)
                            { pHoknummers = ""; }
                            else { pHoknummers = pHoknummers.Replace(",", ";"); pHoknr = pHoknummers; }
                            result = Win32Hok.Init().call_lst_AB_hoklijst(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pSoortlijst, pHoknr);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("hoklijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Jongveeweeglijst":
                        try
                        {
                            Win32Jvwglyst.pCallback doConversion = new Win32Jvwglyst.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);
                            int pSoort = 0;
                            int.TryParse(addonArr[2], out pSoort);
                            int pAlleenAanwezig = 0;
                            int.TryParse(addonArr[3], out pAlleenAanwezig);
                            int pVrouwelijk = 0;
                            int.TryParse(addonArr[4], out pVrouwelijk);
                            int pMannelijk = 0;
                            int.TryParse(addonArr[5], out pMannelijk);
                            string pToonCurves = addonArr[6];
                            int pToonDieren = 0;
                            int.TryParse(addonArr[7], out pToonDieren);
                            int pAfwijkCurve = 0;
                            int.TryParse(addonArr[8], out pAfwijkCurve);
                            int pMinPercAfwijking = 0;
                            int.TryParse(addonArr[9], out pMinPercAfwijking);



                            result = Win32Jvwglyst.Init().call_lst_AB_JongveeWeeglijst(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pSortering, pSoort, begindatum, einddatum,
                                   pAlleenAanwezig, pVrouwelijk, pMannelijk, pToonCurves, pToonDieren, pAfwijkCurve, pMinPercAfwijking);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("jongveeweeglijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Inlezenmonsterbestand":
                        try
                        {
                            //STOP NIET GETEST Inlezenmonsterbestand.aspx via agrobaseoverall3.asmx
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            string pad = outputDir + addonArr[1];
                         
                            if (File.Exists(pad))
                            {

                        
                                string tijd = DateTime.Now.ToString("yyyyMMddHHmm");
                                Win32verwerkVVBmonster vb = new Win32verwerkVVBmonster();
                                Win32verwerkVVBmonster.pCallback cal = new Win32verwerkVVBmonster.pCallback(EventCallbackmethod);
                                DateTime pMPRDatum = DateTime.Now;

                                string pOutPutfile = outputDir + @"csvdata\UBN_" + u.Bedrijfsnummer + "_verwerkVVBmonsterOutput_" + tijd + ".csv";
                                int ret = vb.call_AB_verwerkVVBmonster(pProgId, pProgramId, u.Bedrijfsnummer, DBhost, AgroUser,
                                    AgroPassword, logfilePath, cal, pad, pOutPutfile, out pMPRDatum);

                                if (File.Exists(pOutPutfile))
                                {


                                    Win32EDINRSEldaMelkcontrol emm = new Win32EDINRSEldaMelkcontrol();
                                    Win32EDINRSEldaMelkcontrol.pCallback callie = new Win32EDINRSEldaMelkcontrol.pCallback(EventCallbackmethod);
                                    ret = emm.call_AB_leesEldaMelkcontrole(pProgId, pProgramId, u.Bedrijfsnummer, DBhost, AgroUser,
                                    AgroPassword, logfilePath, callie, pMPRDatum, 4, pOutPutfile);

                                    Win32ABLWBSK te = new Win32ABLWBSK();
                                    Win32ABLWBSK.pCallback ablw = new Win32ABLWBSK.pCallback(EventCallbackmethod);
                                    string pLWBSKmodule = outputDir + @"lib\NRSLW044.exe";
                                    ret = te.AB_lwbsk(pProgId, pProgramId, u.Bedrijfsnummer, DBhost, AgroUser,
                                    AgroPassword, logfilePath, ablw, pMPRDatum, 2, pLWBSKmodule);


                                }
                                
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("jongveeweeglijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Stocklijst_Sopraco":
                        try
                        {
                            Win32AB_Stocklijst_Sopraco.pCallback doConversion = new Win32AB_Stocklijst_Sopraco.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pSort = 0;
                            int.TryParse(addonArr[1], out pSort);

                            result = Win32AB_Stocklijst_Sopraco.Init().call_AB_function_Stocklijst_Sopraco(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pSort);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Stocklijst_Sopraco error (" + soort + "): ", exc);
                        }
                        break;
                    case "Celgetaluitslag":
                        try
                        {
                            Win32Milkcont.pCallback doConversion = new Win32Milkcont.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            int pToonAlleDieren = 0;
                            int.TryParse(addonArr[1], out  pToonAlleDieren);
                            int pLogoID = pProgramId;
                            unLogger.WriteInfo($@"Celgetaluitslag addons={addons} addonArr.count={addonArr.Count()}");
                         
                            int[] possiblecountrys = { 56,208,250,276,380,528,826,883};
                            try
                            {
                                if (!int.TryParse(addonArr[2], out pLogoID))
                                {
                                    pLogoID = pProgramId;
                                }
                                if (possiblecountrys.Contains(pLogoID))
                                {
                                    pLogoID = pProgramId;
                                }  
                            }
                            catch { }
                            result = Win32Milkcont.Init().call_lst_AB_CelgetalUitslag(pProgId, pProgramId, pLogoID, filetype, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pToonAlleDieren);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Stocklijst_Sopraco error (" + soort + "): ", exc);
                        }
                        break;
                    case "EMMlijst":
                        try
                        {
                            Win32EdiEmm.pCallback doConversion = new Win32EdiEmm.pCallback(EventCallbackmethod);

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
 

                            result = Win32EdiEmm.Init().call_lst_AB_EMMlist(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, begindatum, einddatum);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("EMMlijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Snelzichtlijst":
                        try
                        {
                            Win32ABLWBSK.pCallback doConversion = new Win32ABLWBSK.pCallback(EventCallbackmethod);
                            string pLWBSKmodule =  serverMainDir + @"lib\NRSLW044.exe";
                            string pOutputfile = outputDir + u.Bedrijfsnummer + "_ABLWBSK_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "csv";
                            Win32ABLWBSK DllCall = new Win32ABLWBSK();
                            result = DllCall.AB_VerwachteMelkgift(pProgId, pProgramId, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, logfilePath, doConversion, einddatum, pLWBSKmodule, pOutputfile);

                            if (File.Exists(pOutputfile))
                            {
                                int.TryParse(addonArr[0], out filetype);
                                filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                                Win32Milkcont.pCallback doConversion2 = new Win32Milkcont.pCallback(EventCallbackmethod);
                                result =  Win32Milkcont.Init().call_lst_AB_Snelzicht(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion2, einddatum, pOutputfile);

                            }

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Snelzichtlijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "Duurzaamheidslijst":
                        try
                        {
                            Win32lstdzhmon.pCallback doConversion = new Win32lstdzhmon.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);
                            result = Win32lstdzhmon.Init().call_lst_AB_DuurzaamheidDLV(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                               DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, begindatum, einddatum, pSortering);



                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Duurzaamheidslijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "DuurzaamheidsMonitorCRV":
                        try
                        {
                            Win32lstdzhmon.pCallback doConversion = new Win32lstdzhmon.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                        
                            result = Win32lstdzhmon.Init().call_lst_AB_DuurzaamheidsMonitorCRV(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                               DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion,einddatum);



                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Duurzaamheidslijst error (" + soort + "): ", exc);
                        }
                        break;
                    case "STO_Vruchtbaarheid":
                        try
                        {
                            Win32stolist.pCallback doConversion = new Win32stolist.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                          
                            result = Win32stolist.Init().call_lst_AB_STO_vruchtbaarheid(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                               DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum);



                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("STOVruchtbaarheid error (" + soort + "): ", exc);
                        }
                        break;
                    case "STO_dieroverzicht":
                        try
                        {
                            Win32stolist.pCallback doConversion = new Win32stolist.pCallback(EventCallbackmethod);


                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);

                            result = Win32stolist.Init().call_lst_AB_STO_dieroverzicht(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                               DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pSortering);



                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("STO_dieroverzicht error (" + soort + "): ", exc);
                        }
                        break;
                    case "dekdrachtaflam":
                        try
                        {
                            

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSoortBok = 0;
                            int.TryParse(addonArr[1], out pSoortBok);
                            int pBokid = 0;
                            int.TryParse(addonArr[2], out pBokid);
                            int pSubtotaalPerBok = 0;
                            int.TryParse(addonArr[3], out pSubtotaalPerBok);
                            int pGeitDetails = 0;
                            int.TryParse(addonArr[4], out pGeitDetails);
                            int pAlleBedrijven = 0;
                            int.TryParse(addonArr[5], out pAlleBedrijven);

                            Win32Dekpe.pCallback doConversion = new Win32Dekpe.pCallback(EventCallbackmethod);

                            result = Win32Dekpe.Init().call_lst_AB_DekDrachtAflam(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, begindatum, einddatum, pSoortBok,
                              pBokid, pSubtotaalPerBok, pGeitDetails, pAlleBedrijven);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("dekdrachtaflam error (" + soort + "): ", exc);
                        }
                        break;
                    case "periodeoverzichtdekkingen":
                        try
                        {
                            
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                           

                            Win32Dekpe.pCallback doConversion = new Win32Dekpe.pCallback(EventCallbackmethod);

                            result = Win32Dekpe.Init().call_lst_AB_periodeoverzichtdekkingen(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, 
                              pResourceFolder, landcode, pCountryCode, 
                              doConversion, begindatum, einddatum);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("dekdrachtaflam error (" + soort + "): ", exc);
                        }
                        break;
                    case "KIManagementoverzicht":
                        try
                        {
                            //Beyer_Overzicht
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            string ODBCName = "odbcbeyers"; //staat hier op de dev al ingevuld  lMstb.GetProgramConfigValue(0, "MSAccessODBCname");
                            if (String.IsNullOrEmpty(ODBCName))
                            {
                                ODBCName = "odbcbeyers";//staat hier op de dev al ingevuld
                            }
                            string pMDBFilepath = lMstb.GetProgramConfigValue(pProgramId, "MSAccessname");// @"C:\vbs\Focus.mdb";// addonArr[1];
                            if (String.IsNullOrEmpty(pMDBFilepath))
                            {
                                pMDBFilepath = lMstb.GetFarmConfigValue(pUserFarmId, "MSAccessname");
                            }
                            Win32Beyers.pCallback doConversion = new Win32Beyers.pCallback(EventCallbackmethod);
                            int pVeehouderID = 0;
                            int.TryParse(addonArr[1], out pVeehouderID);
                            int pMedewerkerID = 0;
                            int.TryParse(addonArr[2], out pMedewerkerID);
                            result = Win32Beyers.Init().call_Beyer_Overzicht(ODBCName, pMDBFilepath, filetype, filePath, logfilePath, doConversion
                                 , pVeehouderID, pMedewerkerID, begindatum, einddatum);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Beyer_Overzicht error (" + soort + "): ", exc);
                        }
                        break;
                    case "Melkafrekening":
                        try
                        {
                            
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pInvoiceNumberYear = 0;
                            int.TryParse(addonArr[1], out pInvoiceNumberYear);

                            Win32Melkafrekening.pCallback doConversion = new Win32Melkafrekening.pCallback(EventCallbackmethod);

                            int pCooperationNumber = 0;
                            int pSupplierNumber = 0;
                            try
                            {
                                if (addonArr.Length >= 4)
                                {
                                    if (!string.IsNullOrEmpty(addonArr[2]) && !string.IsNullOrEmpty(addonArr[3]))
                                    {
                                        int.TryParse(addonArr[2], out pCooperationNumber);
                                        int.TryParse(addonArr[3], out pSupplierNumber);
                                    }
                                }
                            }
                            catch { }

                            if (pCooperationNumber == 0 || pSupplierNumber == 0)
                            {
                                StringBuilder bld = new StringBuilder(@"SELECT DISTINCT M.InvoiceNumberYear,CONCAT(SUBSTR(M.InvoiceNumberYear,5,2),'-',SUBSTR(M.InvoiceNumberYear,1,4))Maand
                                                                    ,M.CooperationNumber , M.SupplierNumber 
                                                                    FROM agrofactuur.MILKINVO M
                                                                    WHERE M.UbnID=" + UbnId.ToString() + @" AND M.InvoiceNumberYear=" + pInvoiceNumberYear.ToString());
                                DataTable tbl = lMstb.GetDataBase().QueryData(conToken, bld, MissingSchemaAction.Add);


                                if (tbl.Rows.Count > 0)
                                {
                                    if (tbl.Rows[0]["CooperationNumber"] != DBNull.Value && tbl.Rows[0]["CooperationNumber"].ToString() != "")
                                    {
                                        int.TryParse(tbl.Rows[0]["CooperationNumber"].ToString(), out pCooperationNumber);
                                    }
                                    if (tbl.Rows[0]["SupplierNumber"] != DBNull.Value && tbl.Rows[0]["SupplierNumber"].ToString() != "")
                                    {
                                        int.TryParse(tbl.Rows[0]["SupplierNumber"].ToString(), out pSupplierNumber);
                                    }
                                }
                            }
                           
                            //string landcode = lMstb.GetCountryByLandNummer(pCountryCode).LandAfk2;

                            result = Win32Melkafrekening.Init().call_lst_AB_Melkafrekening(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, pResourceFolder, landcode, pCountryCode, doConversion,
                              pCooperationNumber, pSupplierNumber, pInvoiceNumberYear);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Melkafrekening error (" + soort + "): ", exc);
                        }
                        break;
                    case "STPLCelgetal":
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32Milkcont.pCallback doConversion = new Win32Milkcont.pCallback(EventCallbackmethod);

                            result = Win32Milkcont.Init().call_lst_AB_StoplichtCelgetal(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("STPLCelgetal error (" + soort + "): ", exc);
                        }
                        break;
                    case "Afgevoerdedieren":
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);

                            Win32Afvoerlijst.pCallback doConversion = new Win32Afvoerlijst.pCallback(EventCallbackmethod);

                            result = Win32Afvoerlijst.Init().call_lst_AB_AfgevoerdeKoeien(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pSortering);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Afgevoerdedieren error (" + soort + "): ", exc);
                        }
                        break;
                    case "Koeattenties":
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            Win32ABLWBSK.pCallback doConversion = new Win32ABLWBSK.pCallback(EventCallbackmethod);
                            string pLWBSKmodule = serverMainDir + @"lib\NRSLW044.exe";
                            string pOutputfile = outputDir + u.Bedrijfsnummer + "_ABLWBSK_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "csv";
                            Win32ABLWBSK DllCall = new Win32ABLWBSK();
                            result = DllCall.AB_VerwachteMelkgift(pProgId, pProgramId, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, logfilePath, doConversion, einddatum, pLWBSKmodule, pOutputfile);




                            Win32Milkcont.pCallback doConversion2 = new Win32Milkcont.pCallback(EventCallbackmethod);

                            result = Win32Milkcont.Init().call_lst_AB_KoeAttenties(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion2, einddatum, pOutputfile);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Koeattenties error (" + soort + "): ", exc);
                        }
                        break;
                    case "MPRVoeding":
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);


                            Win32ABLWBSK.pCallback doConversion = new Win32ABLWBSK.pCallback(EventCallbackmethod);
                            string pLWBSKmodule = serverMainDir + @"lib\NRSLW044.exe";
                            string pOutputfile = outputDir + u.Bedrijfsnummer + "_ABLWBSK_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "csv";
                            Win32ABLWBSK DllCall = new Win32ABLWBSK();
                            result = DllCall.AB_VerwachteMelkgift(pProgId, pProgramId, u.Bedrijfsnummer,
                                   DBhost, AgroUser, AgroPassword, logfilePath, doConversion, einddatum, pLWBSKmodule, pOutputfile);




                            Win32Milkcont.pCallback doConversion2 = new Win32Milkcont.pCallback(EventCallbackmethod);

                            result = Win32Milkcont.Init().call_lst_AB_MPRvoeding(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion2, einddatum, pOutputfile);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("MPRvoeding error (" + soort + "): ", exc);
                        }
                        break;
                    case "Droogstandevaluatie":
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            // pIndeling: 1=Maand, 2=Kwartaal
                            int pJaar, pMaand, pIndeling;

                            int.TryParse(addonArr[1], out pJaar);
                            int.TryParse(addonArr[2], out pMaand);
                            int.TryParse(addonArr[3], out pIndeling);


                            Win32DROOGZET.pCallback doConversion = new Win32DROOGZET.pCallback(EventCallbackmethod);


                            result = Win32DROOGZET.Init().call_lst_AB_DroogstandEvaluatie(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pJaar, pMaand, pIndeling);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Droogstandevaluatie error (" + soort + "): ", exc);
                        }
                        break;
                    case "Bedrijfsvruchtbaarheid":
                        try
                        {

                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            /*
                                pToonDieren: 0/1
                                pWachtperiode: aantal dagen dat een veehouder wacht 
                             *                  voordat hij een koe voor de eerste keer insemineert na afkalven. 
                             *                  Moet de veehouder dus zelf in kunnen geven op het instelscherm voor de lijst
                                pDraagtijd: draagtijd op het bewuste bedrijf 
                             */
                            int pToonDieren, pWachtperiode, pDraagtijd;

                            int.TryParse(addonArr[1], out pToonDieren);
                            int.TryParse(addonArr[2], out pWachtperiode);
                            int.TryParse(addonArr[3], out pDraagtijd);


                            Win32stolist.pCallback doConversion = new Win32stolist.pCallback(EventCallbackmethod);


                            result = Win32stolist.Init().call_lst_AB_BedrijfsVruchtbaarheid(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, einddatum, pToonDieren, pWachtperiode, pDraagtijd);

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Droogstandevaluatie error (" + soort + "): ", exc);
                        }
                        break;
                    case "Klauwgezondheid":
                        try
                        {
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);

                            Win32Zieklyst.pCallback doConversion = new Win32Zieklyst.pCallback(EventCallbackmethod);

                            result = Win32Zieklyst.Init().call_lst_AB_Klauwgezondheid(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, begindatum);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Klauwgezondheid error (" + soort + "): ", exc);
                        }
                        break;
                    case "Koewerk":
                        try
                        {
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pSortering = 0;
                            int.TryParse(addonArr[1], out pSortering);
                            Win32Vetwerk.pCallback doConversion = new Win32Vetwerk.pCallback(EventCallbackmethod);

                            result = Win32Vetwerk.Init().call_lst_AB_Koewerk(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pSortering);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("AB_Koewerk error (" + soort + "): ", exc);
                        }
                        break;
                    case "Koeinfo":
                        try
                        {
                            int.TryParse(addonArr[0], out filetype);
                            filePath = outputDir + fileNameWithoutExt + getExt(filetype);
                            int pAniid = 0;
                            int.TryParse(addonArr[1], out pAniid);
                            Win32Vetwerk.pCallback doConversion = new Win32Vetwerk.pCallback(EventCallbackmethod);

                            result = Win32Vetwerk.Init().call_lst_AB_Koeinfo(pProgId, pProgramId, filetype, u.Bedrijfsnummer,
                              DBhost, AgroUser, AgroPassword, filePath, logfilePath, doConversion, pAniid);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("Koeinfo error (" + soort + "): ", exc);
                        }
                        break;
                    default:
                        break;
                }

            } //pUbnnr != "0"

            unLogger.WriteInfo("lijst:" + soort + " - " + "result:" + result.ToString());

            //TODO 
            //[BUG 1542] Logging in agrologs.REPORT_LOGGING
            List<AGRO_LABELS> reports = lMstb.GetAgroLabels((int)CORE.DB.LABELSConst.labKind.LIJSTEN, 0, 0, 0);
            REPORT_LOGGING lReport_logging = new REPORT_LOGGING();
            lReport_logging.Rl_ReportId = pLijstenLabId;
            lReport_logging.Rl_DateTime = DateTime.Now;
            lReport_logging.Rl_ThrId = pUserThirdId;
            lReport_logging.Rl_FarmId = pUserFarmId;
            lReport_logging.Rl_UbnId = UbnId;// startWorldGebruiker.Animal_UBNId;
            lReport_logging.Rl_State = result;
            if (pLijstenLabId > 0)
            {
                lMstb.InsertReportLog(lReport_logging);
            }
            return result;
        }

        private int mailLijst(string pEmailTo, string pEmailCC, string filePath, string pSubject, StringBuilder pMailBody, bool ptestDB)
        {
            try
            {
                if (pEmailTo == string.Empty)
                    return 0;

                //Verstuur mail met lijst
                MailMessage mail = new MailMessage();
                mail.Attachments.Add(new Attachment(filePath));

                mail.From = new MailAddress("noreply@nsfo.nl");
                mail.To.Add(pEmailTo);

                if (pEmailCC != string.Empty)
                {
                    string[] ccArr = pEmailCC.Split(';');
                    foreach (string cc in ccArr)
                    {
                        if (cc != string.Empty)
                            mail.CC.Add(cc);
                    }
                }


                mail.Bcc.Add("log@agrobase.nl");
                mail.Subject = pSubject;

                mail.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                mail.Body = pMailBody.ToString();

                SmtpClient smtp = new SmtpClient("smtp.vsm-hosting.nl");
                if (!ptestDB)
                {
                    smtp.Send(mail);
                }

                return 1;
            }
            catch (Exception e)
            {
                unLogger.WriteError(e.Message);
                return -1;
            }
        }

        public string updateBreedval(DBConnectionToken conToken, string aniid, string datum, string kind, string veldnr, string waarde)
        {
            string ret = "success";
            try
            {
                AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(conToken);

                BREEDVAL br = new BREEDVAL();// lMstb.getBreedval(anid, dat, kindval, Fieldv);
                br.AniId = int.Parse(aniid);
                br.BVDate = utils.getDate(datum);
                br.BVKindOfValue = int.Parse(kind);
                br.BVFieldNumber = int.Parse(veldnr);

                double dummyDoub = 0;
                if (Double.TryParse(waarde, out dummyDoub))
                    br.BVFieldValue1 = int.Parse(waarde);
                else
                    br.BVFieldValue1 = 0;

                lMstb.UpdateBreedval(br);
            }
            catch (Exception exc)
            {
                ret = exc.ToString();
            }

            return ret;
        }

        public string beeinddap(string edidapnaam, string serverMainDir)
        {
            string ret = "success";
            try
            {
                if (serverMainDir != null)
                {
                    FileInfo fi = new FileInfo(serverMainDir + "Datacom/in/" + edidapnaam);
                    FileInfo fi2 = new FileInfo(serverMainDir + "Datacom/" + edidapnaam);
                    if (fi2.Exists)
                    {
                        fi2.Delete();
                    }
                    if (fi.Exists)
                    {
                        fi.MoveTo(serverMainDir + "Datacom/" + edidapnaam);
                    }
                }
                else
                    ret = "Server not available.!!!";
            }
            catch (Exception exc)
            {
                ret = exc.ToString();
            }

            return ret;
        }

        public int getLijstenlabId(UserRightsToken pUr, string pSoortlijst)
        {
            //Dit is de koppeling tussen lijstentree.xml  Value  en  LabId
            //Agro_labels LabKind=6200 voor de teksten die getoond worden
            //KUNNEN soms dus niet overeen komen met onderstaande teksten
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            RIGHTS_LISTS rl = lMstb.getRightsListsByListName(pSoortlijst);
            if (rl.LabId > 0)
            {
                return rl.LabId;
            }
            int pLijstenLabId = 0;
            switch (pSoortlijst)
            {


                case "kopreslijstmulti":
                    pLijstenLabId = 0;
                    break;
                case "aanvoerafvoerlijst":
                    pLijstenLabId = 1;
                    break;
                case "stallijstgeit":
                case "stallijstschaap":
                case "stallijstrund":
                    pLijstenLabId = 2;
                    break;
                case "bedrijfsregisterrund":
                case "bedrijfsregister":
                case "bedrregsg":
                    pLijstenLabId = 3;
                    break;
                case "vmlijst":
                    pLijstenLabId = 4;
                    break;
                case "weeglijst":
                    pLijstenLabId = 5;
                    break;
                case "nlinglijst":
                    pLijstenLabId = 6;
                    break;
                case "fokwaardenoverzicht":
                    pLijstenLabId = 7;
                    break;
                case "meetwaardenoverzicht":
                    pLijstenLabId = 8;
                    break;
                case "exterieurwaardenoverzicht":
                    pLijstenLabId = 9;
                    break;
                case "uitvallijst":
                    pLijstenLabId = 10;
                    break;
                case "transportlijst":
                    pLijstenLabId = 11;
                    break;
                case "behandelinglijst":
                    pLijstenLabId = 12;
                    break;
                case "attentielijst_kalf":
                case "attentielijst_kalf_dagelijks":
                case "attentielijst_kalf_controle":
                    pLijstenLabId = 13;
                    break;
                case "attentielijst_kalf_regelmatig":
                    pLijstenLabId = 14;
                    break;
                case "AttentielijstMelkZoog":
                    pLijstenLabId = 73;
                    break;
                case "medsupply":
                    pLijstenLabId = 15;
                    break;
                case "wachttijdlijst":
                    pLijstenLabId = 16;
                    break;
                case "voerleverlijst":
                    pLijstenLabId = 17;
                    break;
                case "deklijst":
                    pLijstenLabId = 18;
                    break;
                case "geblijst":
                    pLijstenLabId = 19;
                    break;
                case "vkilijst":
                    pLijstenLabId = 20;
                    break;
                case "afleverlijst":
                    pLijstenLabId = 21;
                    break;
                case "slachtanalyse":
                    pLijstenLabId = 22;
                    break;
                case "mprlijst":
                    pLijstenLabId = 23;
                    break;
                case "predikaatlijst":
                    pLijstenLabId = 24;
                    break;
                case "afvselect":
                    pLijstenLabId = 25;
                    break;
                case "kopreslijst":
                    pLijstenLabId = 26;
                    break;
                case "stoveevervanging":
                    pLijstenLabId = 27;
                    break;
                case "veesaldolijst":
                    pLijstenLabId = 28;
                    break;
                case "barcodelijst":
                    pLijstenLabId = 29;
                    break;
                case "gezondheidsverklaring":
                    pLijstenLabId = 30;
                    break;
                case "koekaart":
                case "geitkaart":
                case "schapenkaart":
                case "dierkaart":
                    pLijstenLabId = 31;
                    break;
                case "afstambewijs":
                    pLijstenLabId = 32;
                    break;
                case "kruisingsoverzicht":
                    pLijstenLabId = 33;
                    break;
                case "verwantschap":
                    pLijstenLabId = 34;
                    break;
                case "Aflamlijst":
                    pLijstenLabId = 36;
                    break;
                case "Drachtcontrolelijst":
                    pLijstenLabId = 37;
                    break;
                case "Samenweidingslijst":
                    pLijstenLabId = 38;
                    break;
                case "Droogzetlijst":
                    pLijstenLabId = 39;
                    break;
                case "Sponslijst":
                    pLijstenLabId = 40;
                    break;
                case "Drachtanalyse":
                    pLijstenLabId = 41;
                    break;
                case "melkproductielijst":
                    pLijstenLabId = 42;
                    break;
                case "BloedonderzoekAuthorisatie":
                    pLijstenLabId = 43;
                    break;
                case "LabelPrinter":
                    pLijstenLabId = 44;
                    break;
                case "Identificatielijstgeit":
                    pLijstenLabId = 45;
                    break;
                case "Kengetallst":
                    pLijstenLabId = 46;
                    break;
                case "Afstamlijst":
                    pLijstenLabId = 47;
                    break;
                case "Behandelplannen":
                    pLijstenLabId = 48;
                    break;
                case "GroepsPrestatie":
                    pLijstenLabId = 49;
                    break;
                case "Brucella_Inzend_Formulier":
                    pLijstenLabId = 50;
                    break;
                case "Brucella_Declaratie_Formulier":
                    pLijstenLabId = 51;
                    break;
                case "Brucella_Instructie_Formulier":
                    pLijstenLabId = 52;
                    break;
                case "Gemprodooi":
                    pLijstenLabId = 53;
                    break;
                case "Gemprodlam":
                    pLijstenLabId = 54;
                    break;
                case "Vervoersdocument":
                    pLijstenLabId = 55;
                    break;
                case "Aanvoerlijst":
                    pLijstenLabId = 56;
                    break;
                case "Afvoerlijst":
                    pLijstenLabId = 57;
                    break;
                case "Leeftijdlijst":
                    pLijstenLabId = 58;
                    break;
                case "Locatielijst":
                    pLijstenLabId = 59;
                    break;
                case "Statuslijst":
                    pLijstenLabId = 60;
                    break;
                case "Medicijngebruikperkoppel":
                    pLijstenLabId = 61;
                    break;
                case "Registratiekaart":
                    pLijstenLabId = 62;
                    break;
                case "Levensproductie":
                    pLijstenLabId = 63;
                    break;
                case "Medicijnkaart":
                    pLijstenLabId = 64;
                    break;
                case "Geboortebewijs_dierkaart":
                    pLijstenLabId = 65;
                    break;
                case "Fokwaardenbestand":
                    pLijstenLabId = 66;
                    break;
                case "fokwaarden":
                    pLijstenLabId = 67;
                    break;
                case "Kengetallijstnsfostamboek":
                    pLijstenLabId = 68;
                    break;
                case "Restlijst":
                    pLijstenLabId = 69;
                    break;
                case "Voerboxbezoeken":
                    pLijstenLabId = 70;
                    break;
                case "Supplementverstrekking":
                    pLijstenLabId = 71;
                    break;
                case "Ketoattentie":
                    pLijstenLabId = 72;
                    break;
                case "Overzichtdiertelling":
                    pLijstenLabId = 74;
                    break;
                case "Conditiescore":
                    pLijstenLabId = 75;
                    break;
                case "InteeltCoefficient":
                    pLijstenLabId = 76;
                    break;
                case "Klauwscore":
                    pLijstenLabId = 77;
                    break;
                case "Stamboom":
                    pLijstenLabId = 78;
                    break;
                case "Hoklijst":
                    pLijstenLabId = 79;
                    break;
                case "Jongveeweeglijst":
                    pLijstenLabId = 80;
                    break;
                default:
                    pLijstenLabId = 0;
                    break;
            }
            return pLijstenLabId;
        }

        public List<DateTime> getMilkContDates(string pAgroUser, string pDBhost, string pAgropassword, string pUbn, int pProgId, int pProgramId)
        {
            List<DateTime> datums = new List<DateTime>();
            int MaxString = 255;
            String strBdatums = String.Empty;// new StringBuilder(MaxString);
            string logfilePath = unLogger.getLogDir() + pUbn + "_MilkContDates " + DateTime.Now.ToString("yyyyMMdd") + ".log";
            string datumsPath = unLogger.getLogDir() + pUbn + "_MilkContDates " + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            Win32Milkcont.pCallback doConversion = new Win32Milkcont.pCallback(lEventDummyCallbackmethod);

            Win32Milkcont.Init().call_AB_CelgetalDatums(pProgId, pProgramId, pUbn, pDBhost, pAgroUser, pAgropassword, logfilePath, doConversion, ref strBdatums, MaxString);


            if (strBdatums != null && strBdatums != "")
            {
                char[] split = { ';' };
                string[] splits = strBdatums.Split(split);
                for (int i = 0; i < splits.Length; i++)
                {
                    DateTime dat = utils.getDateLNV(splits[i]);
                    if (dat > DateTime.MinValue)
                    {
                        datums.Add(dat);
                    }
                }
            }

            return datums;
        }
         
        private void lEventDummyCallbackmethod(int PercDone, string Msg)
        {
        
        }
    }
}
