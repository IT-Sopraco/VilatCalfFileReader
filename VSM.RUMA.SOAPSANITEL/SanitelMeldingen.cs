using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.SOAPSANITEL.SanitelServices;
using System.Net;
using System.ServiceModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VSM.RUMA.SOAPSANITEL
{
    public class SanitelMeldingen
    {

        private SanitelServices.Sanitel_SanitelServices _service;
        public string Api_access_token { get; internal set; }

        /// <summary>
        /// for webreference Sanitel and for http://api.dev.agrobase.nl/v2-2/454545/sanitel/
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="testserver"></param>
        public SanitelMeldingen(string username, string password, int testserver, string ipadress, int countrycode)
        {
            /*
          *  #region webreference uses a direct webreference to sanitel called:SanitelServices
          *  we are waiting for the api to have these calls implemented
          *  #region sendmeldingen uses http://api.dev.agrobase.nl/v2-2/454545/sanitel/SendBirthNotifications etc....
          *  to send   notifications
          *  username and password are for #region sendmeldingen to get the accestoken 
          */

            //#region webreference settings
            _service = new SanitelServices.Sanitel_SanitelServices();

            //Error:Could not create SSL/TLS secure channel:
            //Voeg onderstaande 2 regels toe;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
 

            //onderstaande dient ook ingesteld te worden
            string NetworkCredentialuser = "WS4SNTRC";
            string NetworkCredentialpass = "ed5R#CK5";
            _service.Credentials = new NetworkCredential(NetworkCredentialuser, NetworkCredentialpass);

            //#region sendmeldingen settings
            try
            {
                Api_access_token = login(username, password, testserver, ipadress, countrycode);
            }
            catch 
            {
            
            }

        }

        /// <summary>
        /// Only for webreference Sanitel
        /// </summary>
        public SanitelMeldingen()
        {
            //#region webreference settings
            _service = new SanitelServices.Sanitel_SanitelServices();

            //Error:Could not create SSL/TLS secure channel:
            //Voeg onderstaande 2 regels toe;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //onderstaande dient ook ingesteld te worden
            string NetworkCredentialuser = "WS4SNTRC";
            string NetworkCredentialpass = "ed5R#CK5";
            _service.Credentials = new NetworkCredential(NetworkCredentialuser, NetworkCredentialpass);
        }
        #region webreference
        /// <summary>
        /// schrijft meldnummers en meldtype naar LocalOutputFile csv bestand
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="testserver"></param>
        /// <param name="taal"></param>
        /// <param name="progid"></param>
        /// <param name="inrichtingsnr"></param>
        /// <param name="beslagnr"></param>
        /// <param name="melddatumvan"></param>
        /// <param name="melddatumtot"></param>
        /// <param name="soortmelding"></param>
        /// <param name="LocalOutputFile"></param>
        /// <param name="LogFile"></param>
        /// <param name="status"></param>
        /// <param name="omschrijving"></param>
        public void STRaadplegenMeldingenAlg(string username, string password, int testserver,
       string taal, int progid, string inrichtingsnr, string beslagnr,
       DateTime melddatumvan, DateTime melddatumtot, int soortmelding,
       string LocalOutputFile, string LogFile,
       ref string status, ref string omschrijving)
        {

            status = "G";
            omschrijving = "";
            if (string.IsNullOrEmpty(LocalOutputFile))
            {
                status = "F";
                omschrijving = "LocalOutputFile: Empty!";
                return;
            }


            IRUtils.writelogline(LogFile, $@"==={nameof(STRaadplegenMeldingenAlg)}===");
            IRUtils.writelogline(LogFile, $@"username:{username}");
            IRUtils.writelogline(LogFile, $@"testserver:{testserver}");
            IRUtils.writelogline(LogFile, $@"taal:{taal}");
            IRUtils.writelogline(LogFile, $@"inrichtingsnr:{inrichtingsnr}");
            IRUtils.writelogline(LogFile, $@"beslagnr:{beslagnr}");
            IRUtils.writelogline(LogFile, $@"melddatumvan:{melddatumvan}");
            IRUtils.writelogline(LogFile, $@"melddatumtot:{melddatumtot}");
            IRUtils.writelogline(LogFile, $@"soortmelding:{soortmelding}");
            IRUtils.writelogline(LogFile, $@"LocalOutputFile:{LocalOutputFile}");
            IRUtils.writelogline(LogFile, $@"LogFile:{LogFile}");



            _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices;
            if (testserver != 0 && testserver != 10)
            {
                _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test;
            }
            IRUtils.writelogline(LogFile, $@"service.Url:{_service.Url}");
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");


            MovementNotificationOverviewExportRequest request = new MovementNotificationOverviewExportRequest();
            request.User = username;
            request.SecurityToken = password;
            request.UserLanguage = taal;
            MovementNotificationOverviewExportRequestMovementNotification mv = new MovementNotificationOverviewExportRequestMovementNotification();
            mv.NOTF_ID = soortmelding;

            mv.DTE_FRM = melddatumvan;
            mv.DTE_FRMSpecified = true;//Elke datum specified op true zetten. 
            mv.DTE_TO = melddatumtot;
            mv.DTE_TOSpecified = true;

            mv.ANTP_CDE = IRUtils.getAnimaltype(progid);
            mv.FCLT_CDE = inrichtingsnr;
            mv.UPD_DTE_FRM = melddatumvan;
            mv.UPD_DTE_FRMSpecified = true;
            mv.UPD_DTE_TO = melddatumtot;
            mv.UPD_DTE_TOSpecified = true;
            mv.SNUN_CDE = beslagnr;
            List<MovementNotificationOverviewExportRequestMovementNotification> l = new List<MovementNotificationOverviewExportRequestMovementNotification>();
            l.Add(mv);
            request.MovementNotification = l.ToArray();
            try
            {

                IRUtils.writelogline(LogFile, $@"request:{IRUtils.getrequest<MovementNotificationOverviewExportRequest>(request)}");
                var result = _service.ExportMovementNotificationOverview(request);
                IRUtils.writelogline(LogFile, $@"result:{IRUtils.getrequest<MovementNotificationOverviewExport>(result)}");

                status = result.ReturnCode;
                omschrijving = result.ReturnMessage;
                if (result.ReturnCode == "0" && result.ReturnMessage == "XML OK")
                {
                    status = "G";
                    omschrijving = "";
                }
                if (result != null && result.MovementNotificationOverview != null)
                {
                    string[] Kols = { "soortmelding", "meldingsnummer", "Datum", "melddatum" };
                    StringBuilder bld = new StringBuilder();
                    StringBuilder line = new StringBuilder();
                    int teller = 0;
                    try
                    {
                        foreach (var mnoe in result.MovementNotificationOverview)
                        {
                            teller += 1;
                            line.Clear();
                            int Notificationtype = IRUtils.getMeldingtype(mnoe.NOTP_CDE);
                            if (Notificationtype == 0)
                            {
                                IRUtils._log.Error($@"{nameof(result.MovementNotificationOverview)} Regel:{teller} onbekende meldtype:{mnoe.NOTP_CDE}");
                                IRUtils.writelogline(LogFile, $@"{nameof(result.MovementNotificationOverview)} Regel:{teller} onbekende meldtype:{mnoe.NOTP_CDE}");
                            }
                            line.Append($@"{Notificationtype};");

                            line.Append($@"{mnoe.NOTF_ID};");
                            line.Append($@"{mnoe.OCR_DTE.ToString("yyyyMMdd")};");
                            if (mnoe.NOTF_RCV_DTE != null && mnoe.NOTF_RCV_DTE > DateTime.MinValue)
                            {
                                line.Append($@"{mnoe.NOTF_RCV_DTE.ToString("yyyyMMdd")}");
                            }
                            bld.AppendLine(line.ToString());
                        }
                    }
                    catch (Exception exc)
                    {
                        IRUtils._log.Error($@"{nameof(result.MovementNotificationOverview)} Regel:{teller} error:{exc.Message}");
                        IRUtils.writelogline(LogFile, $@"{nameof(result.MovementNotificationOverview)} Regel:{teller} error:{exc.Message}");
                    }
                    using (StreamWriter wr = new StreamWriter(LocalOutputFile))
                    {
                        wr.Write(bld);
                    }
                }
            }
            catch (Exception exc)
            {
                IRUtils._log.Error(exc.ToString());
                status = "F";
                omschrijving = exc.Message;
                IRUtils.writelogline(LogFile, exc.Message);
            }

        }

        /// <summary>
        /// vult List<MeldnrAnimal> de meldnummers, met bijbehorende levensnummers
        /// voor de meldingtypes 1,2 en 6
        /// haal de meldnummers eerst op met STRaadplegenMeldingenAlg
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="testserver"></param>
        /// <param name="taal"></param>
        /// <param name="levensnummers"></param>
        /// <param name="status"></param>
        /// <param name="omschrijving"></param>
        /// <param name="LogFile2"></param>
        public void STRaadplegenMutatieDetails(string username, string password, int testserver,
                                   string taal, ref List<MeldnrAnimal> levensnummers,
                                   ref string status, ref string omschrijving,
                                   string LogFile2)
        {
            IRUtils.writelogline(LogFile2, $@"==={nameof(STRaadplegenMutatieDetails)}===");
            IRUtils.writelogline(LogFile2, $@"username:{username}");
            IRUtils.writelogline(LogFile2, $@"testserver:{testserver}");
            _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices;
            if (testserver != 0 && testserver != 10)
            {
                _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test;
            }
            IRUtils.writelogline(LogFile2, $@"taal:{taal}");
            IRUtils.writelogline(LogFile2, $@"lMeldingsnummers:{levensnummers.Count}");
            IRUtils.writelogline(LogFile2, $@"LogFile:{LogFile2}");
            IRUtils.writelogline(LogFile2, $@"Url:{_service.Url}");
            AnimalMovementNotificationOverviewExportRequest em = new AnimalMovementNotificationOverviewExportRequest();

            em.User = username;
            em.SecurityToken = password;
            em.UserLanguage = taal;

            List<AnimalMovementNotificationOverviewExportRequestMovement> arr = new List<AnimalMovementNotificationOverviewExportRequestMovement>();

            int aantalrequests = levensnummers.Count();
            if (ConfigurationManager.AppSettings["SanitelMaxrequests"] != null)
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["SanitelMaxrequests"], out aantalrequests))
                {
                    aantalrequests = levensnummers.Count();
                }
            }
            if (aantalrequests == 0)
            {
                status = "F";
                omschrijving = "Nothing to request";
                return;
            }
            var loops = Math.Ceiling((decimal)levensnummers.Count() / (decimal)aantalrequests);
            int teller = 0;
            for (int i = 0; i < loops; i++)
            {
                arr.Clear();
                for (int j = 0; j < aantalrequests; j++)
                {
                    if (teller < levensnummers.Count())
                    {

                        AnimalMovementNotificationOverviewExportRequestMovement arr1 = new AnimalMovementNotificationOverviewExportRequestMovement();
                        arr1.NOTF_ID = levensnummers[teller].meldnr;
                        arr1.NOTF_IDSpecified = true;
                        arr.Add(arr1);
                        teller += 1;

                    }
                }
                em.Movement = arr.ToArray();

                try
                {

                    IRUtils.writelogline(LogFile2, $@"request:{IRUtils.getrequest<AnimalMovementNotificationOverviewExportRequest>(em)}");
                    var result = _service.ExportAnimalMovementNotificationOverview(em);
                    IRUtils.writelogline(LogFile2, $@"result:{IRUtils.getrequest<AnimalMovementNotificationOverviewExport>(result)}");

                    status = result.ReturnCode;
                    omschrijving = result.ReturnMessage;
                    if (result.ReturnCode == "0" && result.ReturnMessage == "XML OK")
                    {
                        status = "G";
                        omschrijving = "";
                    }
                    if (result != null && result.Movement != null)
                    {
                        foreach (var mnoe in result.Movement)
                        {

                            if (!string.IsNullOrWhiteSpace(mnoe.ANML_REF_NBR))
                            {
                                string pLevensnr = mnoe.ANML_REF_NBR.Replace(" ", "");

                                int lengte = pLevensnr.Length;
                                if (lengte > 3)
                                {
                                    string substr2 = pLevensnr.Substring(2, lengte - 2);
                                    pLevensnr = pLevensnr.Substring(0, 2) + " " + substr2;
                                    string controlegetal = IRUtils.getControlegetal(pLevensnr);
                                    if (!string.IsNullOrWhiteSpace(controlegetal))
                                    {
                                        pLevensnr = pLevensnr.Substring(0, 2) + " " + controlegetal + substr2;
                                    }
                                }
                                levensnummers.FirstOrDefault(x => x.meldnr == mnoe.NOTF_ID).lifenr = pLevensnr;
                            }

                        }
                    }
                }
                catch (Exception exc)
                {
                    IRUtils._log.Error(exc.ToString());
                    status = "F";
                    omschrijving = exc.Message;
                    IRUtils.writelogline(LogFile2, exc.Message);
                }

            }
            //MovementNotificationOverviewExportRequest request1 = new MovementNotificationOverviewExportRequest();
            //SanitelServices.ChangeNotificationOverviewExportRequest request = new ChangeNotificationOverviewExportRequest();
            //SanitelServices.AnimalOverviewExportRequest am = new AnimalOverviewExportRequest();

            //SanitelServices.ChangeNotificationOverviewExportRequest creq = new ChangeNotificationOverviewExportRequest();
            //creq.ChangeNotification.NOTF_ID = lMeldingsnummer;

        }

        /// <summary>
        /// vult List<MeldnrAnimal> de meldnummers, met bijbehorende levensnummers
        /// voor de meldingtypes anders dan: 1,2 en 6
        /// haal de meldnummers eerst op met STRaadplegenMeldingenAlg
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="testserver"></param>
        /// <param name="taal"></param>
        /// <param name="levensnummers"></param>
        /// <param name="LogFile3"></param>
        /// <param name="status"></param>
        /// <param name="omschrijving"></param>
        public void STRaadplegenVerplaatsingDetails(string username, string password, int testserver,
                                string taal, ref List<MeldnrAnimal> levensnummers, string LogFile3,
                                ref string status, ref string omschrijving)
        {

            IRUtils.writelogline(LogFile3, $@"==={nameof(STRaadplegenVerplaatsingDetails)}===");
            IRUtils.writelogline(LogFile3, $@"username:{username}");
            IRUtils.writelogline(LogFile3, $@"testserver:{testserver}");
            _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices;
            if (testserver != 0 && testserver != 10)
            {
                _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test;
            }
            IRUtils.writelogline(LogFile3, $@"taal:{taal}");
            IRUtils.writelogline(LogFile3, $@"lMeldingsnummers:{levensnummers.Count}");

            IRUtils.writelogline(LogFile3, $@"Url:{_service.Url}");
            status = "G";
            omschrijving = "";



            MovementNotificationExportRequest request = new MovementNotificationExportRequest();
            request.User = username;
            request.SecurityToken = password;
            request.UserLanguage = taal;
            List<MovementNotificationExportRequestMovement> lmv = new List<MovementNotificationExportRequestMovement>();
            int aantalrequests = levensnummers.Count();
            if (ConfigurationManager.AppSettings["SanitelMaxrequests"] != null)
            {
                if (!int.TryParse(ConfigurationManager.AppSettings["SanitelMaxrequests"], out aantalrequests))
                {
                    aantalrequests = levensnummers.Count();
                }
            }
            if (aantalrequests == 0)
            {
                status = "F";
                omschrijving = "Nothing to request";
                return;
            }
            var loops = Math.Ceiling((decimal)levensnummers.Count() / (decimal)aantalrequests);
            int teller = 0;
            for (int i = 0; i < loops; i++)
            {
                lmv.Clear();
                for (int j = 0; j < aantalrequests; j++)
                {
                    if (teller < levensnummers.Count())
                    {
                        MovementNotificationExportRequestMovement mv = new MovementNotificationExportRequestMovement();
                        mv.ID = levensnummers[teller].meldnr;
                        lmv.Add(mv);
                        teller += 1;
                    }
                }
                if (lmv.Count() > 0)
                {
                    request.Movement = lmv.ToArray();
                    try
                    {
                        IRUtils.writelogline(LogFile3, $@"request:{IRUtils.getrequest<MovementNotificationExportRequest>(request)}");
                        var result = _service.ExportMovementNotification(request);
                        IRUtils.writelogline(LogFile3, $@"response:{IRUtils.getrequest<MovementNotificationExport>(result)}");
                        status = result.ReturnCode;
                        omschrijving = result.ReturnMessage;
                        if (result.ReturnCode == "0" && result.ReturnMessage == "XML OK")
                        {
                            status = "G";
                            omschrijving = "";
                        }

                        if (result != null && result.Movement != null)
                        {
                            foreach (var mnoe in result.Movement)
                            {
                                foreach (var ani in mnoe.Animal)
                                {
                                    string pLevensnr = ani.REF_NBR.Replace(" ", "");

                                    int lengte = pLevensnr.Length;
                                    if (lengte > 3)
                                    {
                                        string substr2 = pLevensnr.Substring(2, lengte - 2);
                                        pLevensnr = pLevensnr.Substring(0, 2) + " " + substr2;
                                        string controlegetal = IRUtils.getControlegetal(pLevensnr);
                                        if (!string.IsNullOrEmpty(controlegetal))
                                        {
                                            pLevensnr = pLevensnr.Substring(0, 2) + " " + controlegetal + substr2;
                                        }
                                    }

                                    levensnummers.FirstOrDefault(x => x.meldnr == mnoe.ID).lifenr = pLevensnr;
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        IRUtils._log.Error(exc.ToString());
                        status = "F";
                        omschrijving = exc.Message;
                        IRUtils.writelogline(LogFile3, exc.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Ophalen dierdetails en schrijft deze naar csv bestand LocalOutputFile
        /// levensnr; naam; geb.dat; geslacht; haarkleur; rastype; versienr paspoort; levensnr moeder; inrichtingsnr fokker
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="testserver"></param>
        /// <param name="taal"></param>
        /// <param name="progid"></param>
        /// <param name="lifenrs"></param>
        /// <param name="LocalOutputFile"></param>
        /// <param name="logfile"></param>
        /// <param name="status"></param>
        /// <param name="omschrijving"></param>
        public void STDierdetails(string username, string password, int testserver,
                                string taal, int progid, List<string> lifenrs, string LocalOutputFile, string logfile, ref string status, ref string omschrijving)
        {

            IRUtils.writelogline(logfile, $@"==={nameof(STDierdetails)}===");
            IRUtils.writelogline(logfile, $@"username:{username}");
            IRUtils.writelogline(logfile, $@"testserver:{testserver}");
            _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices;
            if (testserver != 0 && testserver != 10)
            {
                _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test;
            }
            IRUtils.writelogline(logfile, $@"taal:{taal}");
            IRUtils.writelogline(logfile, $@"lifenrs:{lifenrs.Count}");

            IRUtils.writelogline(logfile, $@"Url:{_service.Url}");
            status = "G";
            omschrijving = "";

            string ANTP_CDE = IRUtils.getAnimaltype(progid);



            AnimalExportRequest req = new AnimalExportRequest();
            req.User = username;
            req.SecurityToken = password;
            req.UserLanguage = taal;
            _service.Timeout = 5 * 60 * 1000;

            List<AnimalExportRequestAnimal> list = new List<AnimalExportRequestAnimal>();
            foreach (string lnr in lifenrs)
            {
                AnimalExportRequestAnimal ar = new AnimalExportRequestAnimal();
                ar.ANTP_CDE = ANTP_CDE;
                ar.CDE = lnr;
                list.Add(ar);
            }
            req.Animal = list.ToArray();
            try
            {
                IRUtils.writelogline(logfile, $@"request:{IRUtils.getrequest<AnimalExportRequest>(req)}");
                var result = _service.ExportAnimal(req);
                IRUtils.writelogline(logfile, $@"response:{IRUtils.getrequest<AnimalExport>(result)}");
                status = result.ReturnCode;
                omschrijving = result.ReturnMessage;
                if (result.ReturnCode == "0" && result.ReturnMessage == "XML OK")
                {
                    status = "G";
                    omschrijving = "";
                }
                StringBuilder bld = new StringBuilder();
                StringBuilder line = new StringBuilder();
                //levensnr; naam; geb.dat; geslacht; haarkleur; rastype; versienr paspoort; levensnr moeder; inrichtingsnr fokker
                //        Rastype
                //            1 = Melk
                //            2 = Vlees
                //            3 = Gemend
                foreach (var r in result.Animal)
                {
                    line.Clear();
                    string pLevensnr = r.CDE.Replace(" ", "");
                    int lengte = pLevensnr.Length;
                    if (lengte > 3)
                    {
                        string substr2 = pLevensnr.Substring(2, lengte - 2);
                        pLevensnr = pLevensnr.Substring(0, 2) + " " + substr2;
                        string controlegetal = IRUtils.getControlegetal(pLevensnr);
                        if (!string.IsNullOrWhiteSpace(controlegetal))
                        {
                            pLevensnr = pLevensnr.Substring(0, 2) + " " + controlegetal + substr2;
                        }
                    }
                    line.Append($@"{pLevensnr};");
                    line.Append($@";");
                    if (r.BTH_DTESpecified)
                    {
                        line.Append($@"{Convert.ToDateTime(r.BTH_DTE).ToString("yyyyMMdd")};");
                    }
                    else
                    {
                        line.Append($@";");
                    }
                    string geslacht = r.GNDR_CDE == "FEM" ? "V" : "M";
                    line.Append($@"{geslacht};");
                    line.Append($@"{r.HRTP_CDE};");
                    int rastype = r.ANRT_CDE == "MILK" ? 1 : 2;
                    line.Append($@"{rastype};");
                    line.Append($@"{r.PPT_VRS_NBR};");

                    string levnrmother = r.CDE_MHR.Replace(" ", "");
                    lengte = levnrmother.Length;
                    if (lengte > 3)
                    {
                        string substr2m = levnrmother.Substring(2, lengte - 2);
                        levnrmother = levnrmother.Substring(0, 2) + " " + substr2m;
                        string controlegetal = IRUtils.getControlegetal(levnrmother);
                        if (!string.IsNullOrWhiteSpace(controlegetal))
                        {
                            levnrmother = levnrmother.Substring(0, 2) + " " + controlegetal + substr2m;
                        }
                    }
                    line.Append($@"{levnrmother};");

                    line.Append($@"{r.FCLT_CDE_BTH};");
                    bld.AppendLine(line.ToString());
                }
                using (StreamWriter wr = new StreamWriter(LocalOutputFile))
                {
                    wr.Write(bld.ToString());
                }
            }
            catch (Exception exc)
            {
                IRUtils._log.Error(exc.ToString());
                status = "F";
                omschrijving = exc.Message;
                IRUtils.writelogline(logfile, exc.Message);
            }

        }
        public class MeldnrAnimal
        {
            public long meldnr { get; set; }
            public string lifenr { get; set; }
        }

        #endregion

        #region sendmeldingen by MAPI

        public List<soaplog> SendBirthNotifications(string ubn, int testserver, List<SanitelBirthNotification> SanitelBirthNotifications, string logfile)
        {
            string prefix = nameof(SendBirthNotifications);
            List<soaplog> ret = new List<soaplog>();
     
            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");
            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No api acces token" };
                IRUtils.writelogline(logfile, $@"{prefix}:F: {sl1.omschrijving}");
                ret.Add(sl1);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl1);
                return ret;
            }
            if (SanitelBirthNotifications.Count() == 0)
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl1);
                return ret;
            }
            try
            {

                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelBirthNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress =  new Uri(apiurl);
           
                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendBirthNotifications"}");
             
                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendBirthNotifications", content).Result;

                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    var sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: {detail.Code};{detail.Description};{detail.Reference} ");
                        }
                    }
                }
                else
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                    ret.Add(sl);
                }

            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
            
                IRUtils.writelogline(logfile, exc.ToString());
            }

            return ret;
        }

        public List<soaplog> SendImportNotifications(string ubn, int testserver, List<SanitelImportNotification> SanitelImportNotifications, string logfile)
        {
            string prefix = nameof(SendImportNotifications);
            List<soaplog> ret = new List<soaplog>();
 
            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No api acces token" };
                ret.Add(sl1);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl1);
                return ret;
            }
            if (SanitelImportNotifications.Count() == 0)
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl1);
                return ret;
            }
            try
            {

                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelImportNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendImportNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendImportNotifications", content).Result;

                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    soaplog sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: {detail.Code};{detail.Description};{detail.Reference} ");
                        }
                    }
                }
                else
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                    ret.Add(sl);
                }

            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
                IRUtils.writelogline(logfile, exc.ToString());
            }
            //});
            return ret;
        }

        public List<soaplog> SendExportNotifications(string ubn, int testserver, List<SanitelExportNotification> SanitelExportNotifications, string logfile)
        {
            string prefix = nameof(SendExportNotifications);
            List<soaplog> ret = new List<soaplog>();
     
            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl = new soaplog { status = "F", omschrijving = "No api acces token" };
                ret.Add(sl);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl);
                return ret;
            }
            if (SanitelExportNotifications.Count()==0)
            {
                var sl = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl);
                return ret;
            }
            try
            {
                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelExportNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
             
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendExportNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendExportNotifications", content).Result;


                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                    ret.Add(sl);

                }
                else
                {
                 
                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    var sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: Code:{detail.Code}; Description: {detail.Description}; Reference:{detail.Reference} ");
                        }
                    }
                }


            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
                IRUtils.writelogline(logfile, exc.ToString());
            }
            //});
            return ret;
        }

        public List<soaplog> SendDeathNotifications(string ubn, int testserver, List<SanitelDeathNotification> SanitelDeathNotifications, string logfile)
        {
            string prefix = nameof(SendDeathNotifications);
            List<soaplog> ret = new List<soaplog>();
       
            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl = new soaplog { status = "F", omschrijving = "No api acces token" };
                ret.Add(sl);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl);
                return ret;
            }
            if (SanitelDeathNotifications.Count() == 0)
            {
                var sl = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl);
                return ret;
            }
            try
            {

                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelDeathNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendDeathNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendDeathNotifications", content).Result;



                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    
                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    var sl = new soaplog(); 
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: {detail.Code};{detail.Description};{detail.Reference} ");
                        }
                    }
                }
                else
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                }


            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
                IRUtils.writelogline(logfile, exc.ToString());
            }
           
            return ret;
        }

        #endregion

        #region sendmeldingen by SOAP

        public List<soaplog> SendBirthNotificationsV1(string ubn, int testserver, List<SanitelBirthNotification> SanitelBirthNotifications, string logfile)
        {
            string prefix = nameof(SendBirthNotifications);
            List<soaplog> ret = new List<soaplog>();

            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices;
            if (testserver != 0 && testserver != 10)
            {
                _service.Url = Properties.Settings.Default.SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test;
            }
            IRUtils.writelogline(logfile, $@"service.Url:{_service.Url}");
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (String.IsNullOrEmpty(ubn))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl1);
                return ret;
            }
            if (SanitelBirthNotifications.Count() == 0)
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl1);
                return ret;
            }
            try
            {
              

                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelBirthNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendBirthNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendBirthNotifications", content).Result;

                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    var sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            sl2.lifenumber = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: {detail.Code};{detail.Description};{detail.Reference} ");
                        }
                    }
                }
                else
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                    ret.Add(sl);
                }

            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);

                IRUtils.writelogline(logfile, exc.ToString());
            }

            return ret;
        }

        public List<soaplog> SendImportNotificationsV1(string ubn, int testserver, List<SanitelImportNotification> SanitelImportNotifications, string logfile)
        {
            string prefix = nameof(SendImportNotifications);
            List<soaplog> ret = new List<soaplog>();

            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No api acces token" };
                ret.Add(sl1);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl1);
                return ret;
            }
            if (SanitelImportNotifications.Count() == 0)
            {
                var sl1 = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl1);
                return ret;
            }
            try
            {

                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelImportNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendImportNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendImportNotifications", content).Result;

                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    soaplog sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            sl2.lifenumber = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: {detail.Code};{detail.Description};{detail.Reference} ");
                        }
                    }
                }
                else
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                    ret.Add(sl);
                }

            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
                IRUtils.writelogline(logfile, exc.ToString());
            }
            //});
            return ret;
        }

        public List<soaplog> SendExportNotificationsV1(string ubn, int testserver, List<SanitelExportNotification> SanitelExportNotifications, string logfile)
        {
            string prefix = nameof(SendExportNotifications);
            List<soaplog> ret = new List<soaplog>();

            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl = new soaplog { status = "F", omschrijving = "No api acces token" };
                ret.Add(sl);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl);
                return ret;
            }
            if (SanitelExportNotifications.Count() == 0)
            {
                var sl = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl);
                return ret;
            }
            try
            {
                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelExportNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendExportNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendExportNotifications", content).Result;


                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                    ret.Add(sl);

                }
                else
                {

                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    var sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            sl2.lifenumber = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: Code:{detail.Code}; Description: {detail.Description}; Reference:{detail.Reference} ");
                        }
                    }
                }


            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
                IRUtils.writelogline(logfile, exc.ToString());
            }
            //});
            return ret;
        }

        public List<soaplog> SendDeathNotificationsV1(string ubn, int testserver, List<SanitelDeathNotification> SanitelDeathNotifications, string logfile)
        {
            string prefix = nameof(SendDeathNotifications);
            List<soaplog> ret = new List<soaplog>();

            IRUtils.writelogline(logfile, $@"{prefix}:UBN: {ubn}");
            IRUtils.writelogline(logfile, $@"{prefix}:Testserver: {testserver}");

            if (String.IsNullOrEmpty(Api_access_token))
            {
                var sl = new soaplog { status = "F", omschrijving = "No api acces token" };
                ret.Add(sl);
                return ret;
            }
            if (String.IsNullOrEmpty(ubn))
            {
                var sl = new soaplog { status = "F", omschrijving = "No farmnumber parameter " };
                ret.Add(sl);
                return ret;
            }
            if (SanitelDeathNotifications.Count() == 0)
            {
                var sl = new soaplog { status = "F", omschrijving = "No notifications to send " };
                ret.Add(sl);
                return ret;
            }
            try
            {

                HttpClient client = new HttpClient();
                string jsonsendstring = JsonConvert.SerializeObject(SanitelDeathNotifications.ToArray());
                var content = new StringContent(jsonsendstring, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonsendstring}");
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                client.DefaultRequestHeaders.Add("access_token", Api_access_token);
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                client.BaseAddress = new Uri(apiurl);

                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                IRUtils.writelogline(logfile, $@"{prefix}:Url: {apiurl + "/" + $@"{ubn}/sanitel/SendDeathNotifications"}");

                var result = client.PostAsync(apiurl + "/" + $@"{ubn}/sanitel/SendDeathNotifications", content).Result;



                string jsonString = result.Content.ReadAsStringAsync().Result.Replace("null", "");
                if (!string.IsNullOrWhiteSpace(jsonString))
                {

                    NotificationResult res = JsonConvert.DeserializeObject<NotificationResult>(jsonString);
                    var sl = new soaplog();
                    sl.status = res.Status == 1 ? "G" : res.Status == 2 ? "W" : res.Status == 3 ? "F" : "F";
                    sl.omschrijving = res.Description == null ? sl.status == "G" ? "" : jsonString : res.Description;
                    sl.meldnummer = res.ReferenceNr;
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, $@"{prefix}:jsonstring: {jsonString}");
                    if (res.Details != null)
                    {
                        foreach (var detail in res.Details)
                        {
                            soaplog sl2 = new soaplog { status = "G", omschrijving = "" };
                            sl2.status = detail.Code == 1 ? "G" : detail.Code == 2 ? "W" : detail.Code == 3 ? "F" : "F";
                            sl2.omschrijving = detail.Description;
                            sl2.meldnummer = detail.Reference;
                            sl2.lifenumber = detail.Reference;
                            ret.Add(sl2);
                            IRUtils.writelogline(logfile, $@"Details: {detail.Code};{detail.Description};{detail.Reference} ");
                        }
                    }
                }
                else
                {
                    var sl = new soaplog();
                    sl.status = "F";
                    sl.omschrijving = $@"  {result.ReasonPhrase} ";
                    ret.Add(sl);
                    IRUtils.writelogline(logfile, sl.omschrijving + " :");
                }


            }
            catch (Exception exc)
            {
                var sl = new soaplog();
                sl.status = "F";
                sl.omschrijving = exc.Message;
                ret.Add(sl);
                IRUtils.writelogline(logfile, exc.ToString());
            }

            return ret;
        }

        #endregion

        #region Helpers


        public string login(string ApiUsername, string ApiPasswordDecoded, int testserver, string ipadress, int countrycode)
        {

            string pw = ApiPasswordDecoded;// Facade.GetInstance().getRechten().DeCodeer_String(ApiPasswordEncoded);

            ////http://api.dev.agrobase.nl/v2-2/user/login?username=vetwerk_inlees&password=qXrZfazN&ip_address=1&country_iso=528 
            /*
                    Vetwerk inlees

                    Bussiness token
                    10-YMbDDBLFLwKFicdtiie9sp3htUDTiMK0mlkeAdJ4Xu0fZQ9lhAgoeJdjJtKiyrXy

                    Username
                    vetwerk_inlees

                    Password
                    qXrZfazN
             */
            /*
             SELECT * FROM API_LOG_9.RESPONSE  
order by response_id desc
LIMIT 20
             */
            try
            {

                HttpClient client = new HttpClient();
                string apiurl = ConfigurationManager.AppSettings["ApiUrl"];
                if (apiurl.EndsWith("/"))
                {
                    apiurl = apiurl.Remove(apiurl.Length - 1, 1);
                }
                apiurl = apiurl + "/" + $@"user/login?username={ApiUsername}&password={pw}&ip_address={ipadress}&country_iso={countrycode}";
            
                client.DefaultRequestHeaders.Add("bussiness_token", ConfigurationManager.AppSettings["ProgToken"]);
                var result = client.GetAsync(apiurl).Result;

                if (result.IsSuccessStatusCode)
                {
                    string jsonString = result.Content.ReadAsStringAsync().Result;
                    UserTokenRead res = JsonConvert.DeserializeObject<UserTokenRead>(jsonString);
                    Api_access_token = res.access_token;
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Login Error Exception", exc.InnerException);
            }

            if (string.IsNullOrEmpty(Api_access_token))
            {
                throw new Exception("Login Error Exception");
            }
            return Api_access_token;
        }

      
        #endregion
    }

    [Serializable]
    public class soaplog
    {
        public string lifenumber { get; set; }
        public string status { get; set; }
        public string omschrijving { get; set; }
        public string meldnummer { get; set; }
    }

    [Serializable]
    public class UserTokenRead
    {
        public string access_token { get; set; }
        public DateTime expire_date { get; set; }
    }

    [Serializable]
    public class NotificationResult 
    {
        public int? Status { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = Success, 2 = Warn, 3 = Error] = ['0', '1', '2', '3'],
        public string Description { get; set; }
        //(string, optional),
        public string ReferenceNr { get; set; }
        //(string, optional),
        public NotificationResultDetail[] Details { get; set; }
        //(Array[NotificationResultDetail], optional, read only)
    }

    [Serializable]
    public class NotificationResultDetail 
    {
        public int? Code { get; set; }
        //(integer, optional),
        public string Description { get; set; }
        //(string, optional),
        public string Reference { get; set; }
        //(string, optional)
    } 

    [Serializable]
    public class SanitelBirthNotification
    {
        public string Lifenumber { get; set; }
        //(string, optional),
        public string Name { get; set; }
        //(string, optional),
        public string LifenumberMother { get; set; }
        //(string, optional),
        public int? AnimalType { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = MilkCow, 3 = Sheep, 5 = Goat, 6 = Calf] = ['0', '1', '3', '5', '6'],
        public bool? Borndead { get; set; }
        //(boolean, optional),
        public int? RaceType { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = Milk, 2 = Meat, 3 = Mixed] = ['0', '1', '2', '3'],
        public DateTime? Birthdate { get; set; }
        //(string, optional),
        public int? Sex { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = Male, 2 = Female, 3 = FreeMartin] = ['0', '1', '2', '3'],
        public string Haircolor { get; set; }
        //(string, optional),
        public bool? SucklingCalf { get; set; }
        //(boolean, optional),
        public int? EventId { get; set; }
        //(integer, optional),
        public bool? EmbryoTransplant { get; set; }
        //(boolean, optional) :MUTATION.ET ,
        public int? Beefiness { get; set; }
        //(integer, optional) :MUTATION.MeatScore[1 = MelkveeKalf, 2 = WeinigBespierd, 3 = NormaalBespierd, 4 = ExtraBespierd, 5 = DikBil] = ['1', '2', '3', '4', '5'],
        public int? BirthCourse { get; set; }
        //(integer, optional) :MUTATION.CalvingEase[0 = Unknown, 1 = Easy, 2 = Normal, 3 = Difficult, 4 = Caesarean, 5 = SawedOff, 6 = OtherAid] = ['0', '1', '2', '3', '4', '5', '6'],
        public int? RegistrationCard { get; set; }
        //(integer, optional) :MUTATION.RegistrationCard; [0 = NA, 1 = Yes, 2 = No] = ['0', '1', '2'],
        public int? Specialities { get; set; }
        //(integer, optional) :MUTATION.Speciality[0 = NoSpecialities, 1 = LightCalf, 2 = HeavyCalf, 3 = BornToEarly, 4 = BornToLate, 5 = Plurar, 6 = EmbryoTransplantation, 7 = HeritableDefect, 8 = UnregisteredInsemination, 9 = MultipleSpecialities] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'],
        public int? BirthWeight { get; set; }
        // (integer, optional) :MUTATION.Weight ,
        public int? Nling { get; set; }
        //(integer, optional) : MUTATION.Nling ,
        public bool? MotherBoughtRecent { get; set; }
        //(boolean, optional) :  MUTATION.MotherBoughtRecent
    }


    [Serializable]
    public class SanitelImportNotification
    {
        public string Lifenumber { get; set; }
        //(string, optional),
        public DateTime? ImportDate { get; set; }
        //(string, optional),
        public int? AnimalType { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = MilkCow, 3 = Sheep, 5 = Goat, 6 = Calf] = ['0', '1', '3', '5', '6'],
        public int? ArrivalState { get; set; }
        //(integer, optional) :pRecord.AniState[1 = Alive, 2 = Dead] = ['1', '2'],
        public int? RegistrationCard { get; set; }
        //(integer, optional) :MUTATION.RegistrationCard; [0 = NA, 1 = Yes, 2 = No] = ['0', '1', '2'],
        public int? SubAnimalKind { get; set; }
        //(integer, optional) :Required for farmer, not allowed otherwise. [1 = Rund, 2 = Vleeskalf] = ['1', '2'],
        public string Licenceplate { get; set; }
        //(string, optional) :MUTATION.LicensePlate ,
        public int? VersionNrPassport { get; set; }
        //(integer, optional),
        public string ProductionUnit { get; set; }
        //(string, optional),
        public int? ProductionUnitIndex { get; set; }
        //(integer, optional),
        public string VatNrDestination { get; set; }
        //(string, optional),
        public string VatNrTransporter { get; set; }
        //(string, optional),
        public int? MovementId { get; set; }
        //(integer, optional)
    }


    [Serializable]
    public class SanitelExportNotification
    {
        public string Lifenumber { get; set; }
        //(string, optional),
        public DateTime? ExportDate { get; set; }
        //(string, optional),
        public int? AnimalType { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = MilkCow, 3 = Sheep, 5 = Goat, 6 = Calf] = ['0', '1', '3', '5', '6'],
        public int? SubAnimalKind { get; set; }
        //(integer, optional) :Required for farmer, not allowed otherwise. [1 = Rund, 2 = Vleeskalf] = ['1', '2'],
        public string Licenceplate { get; set; }
        //(string, optional) :MUTATION.LicensePlate ,
        public int? TypeOfExport { get; set; }
        //pRecord.AniState [0 = Normaal, 1 = NaarSlachthuis] = ['0', '1']
        public int? RegistrationCard { get; set; }
        //(integer, optional) :MUTATION.RegistrationCard; [0 = NA, 1 = Yes, 2 = No] = ['0', '1', '2'],
        public int? VersionNrPassport { get; set; }
        //(integer, optional),
        public string VatNumberDestination { get; set; }
        //(string, optional),
        public string VatNumberTransporter { get; set; }
        //(string, optional),
        public int? VrvExportReason { get; set; }
        public int? MovementId { get; set; }
        //(integer, optional)
    }

    [Serializable]
    public class SanitelDeathNotification
    {
        public string Lifenumber { get; set; }
        //(string, optional),
        public DateTime? DeathDate { get; set; }
        //(string, optional),
        public int? AnimalType { get; set; }
        //(integer, optional) :[0 = Unknown, 1 = MilkCow, 3 = Sheep, 5 = Goat, 6 = Calf] = ['0', '1', '3', '5', '6'],
        public int? SubAnimalKind { get; set; }
        //(integer, optional) :Required for farmer, not allowed otherwise. [1 = Rund, 2 = Vleeskalf] = ['1', '2'],
        public int? VersionNrPassport { get; set; }
        //(integer, optional),
        public int? VrvExportReason { get; set; }
        public int? MovementId { get; set; }
        //(integer, optional)
    }
}
