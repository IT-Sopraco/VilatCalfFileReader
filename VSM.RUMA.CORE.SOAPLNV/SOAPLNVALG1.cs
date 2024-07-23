using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.SOAPLNV.ReferentieWS;
using VSM.RUMA.CORE.SOAPLNV.MachtigingenWS;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE.DB.DataTypes;
using System.ServiceModel;
using System.Configuration;

namespace VSM.RUMA.CORE.SOAPLNV
{
    public class SOAPLNVALG_Referentie1
    {
        //private LNVReferentie.ReferentieServiceService webservice;

        //public SOAPLNVALG1()
        //{
        //    webservice = new ReferentieServiceService();


        //}

        public int LoginCheck(String pUsername, String pPassword, int pTestServer)
        {
            try
            {
                Masking m = new Masking();
                string pword = m.DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
                diersoortenType[] resp = raadplegenDiersoorten(pUsername, pPassword, pTestServer, "AGROB" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                diersoortenType[] respruma = raadplegenDiersoorten(ConfigurationManager.AppSettings["LNVDierDetailsusername"], pword, pTestServer, "VSMS" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                if (resp.Count() == respruma.Count())
                    return 1;
                else if (respruma.Count() > 0)
                    return 0;
                else
                    return -1;

            }
            catch (Exception ex)
            {
                AppDomain.CurrentDomain.SetData("LNV_" + pUsername + "_Exception", ex);
                //unLogger.WriteDebug(ex.Message, ex);
                unLogger.WriteInfo($@" {nameof(SOAPLNV.SOAPLNVALG_Referentie1)}.{nameof(LoginCheck)} {pUsername} fault:{ex.Message}");
                return -1;
            }
        }

        public diersoortenType[] raadplegenDiersoorten(String pUsername, String pPassword, int pTestServer, String requestID)
        {
            try
            {
                ReferentieServiceService webservice = new ReferentieServiceService();
                if (pUsername == "1293561")
                    webservice.PreAuthenticate = true;
                webservice.Credentials = new System.Net.NetworkCredential(pUsername, pPassword);
                if (pTestServer == 1) webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air2/services/ReferentieWS";
                if (pTestServer == 2) webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air1/services/ReferentieWS";
                diersoortenRequestType req = new diersoortenRequestType();
                req.requestID = requestID;
                diersoortenResponseType resp = webservice.raadplegenDiersoorten(req);
                return resp.diersoorten;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(ex.Message, ex);
                return new List<diersoortenType>().ToArray();
            }
        }

        public bool raadplegenMachtigingen_Adres(String pUsername, String pPassword, int pTestServer, String UBN,
                ref String LNV_UBN, ref String  LNV_BRS, ref String  LNV_Naam, ref String  LNV_Adres, ref String  LNV_Postcode, ref String  LNV_Woonplaats)
        {
            try
            {
                MachtigingenServiceService webservice = new MachtigingenServiceService();
                webservice.Credentials = new System.Net.NetworkCredential(pUsername, pPassword);
                if (pTestServer == 1) webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air2/services/MachtigingenWS";
                if (pTestServer == 2) webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air1/services/MachtigingenWS";
                rdplnMEUBNWaarvoorGemachtigdRequestType req = new rdplnMEUBNWaarvoorGemachtigdRequestType();
                req.requestID = String.Format("VSMM{0}",System.Threading.Thread.CurrentThread.ManagedThreadId);
                req.selMeldingeenheid = UBN;
                rdplnMEUBNWaarvoorGemachtigdResponseType resp = webservice.rdplgnMEUBNWaarvoorGemachtigd(req);
                foreach (meldingeenheidgegevensType respdata in resp.meldingeenheidgegevens)
                {
                    LNV_BRS = respdata.relatienummerHouder;
                    LNV_Naam = respdata.naamHouder;
                    LNV_Adres = respdata.adresOmsME;
                    LNV_Postcode = respdata.postcodeME;
                    LNV_Woonplaats = respdata.plaatsnaamME;
                    LNV_UBN = respdata.MEnummer;
                }
                return true;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(ex.Message, ex);
                return false;
            }
        }

        public bool raadplegenmeldingen(String pUsername, String pPassword, int pTestServer, String UBN, string RelatienummerHouder, int Diersoort)
        {
            try
            {
                DierenWS.DierenServiceService webservice = new DierenWS.DierenServiceService();
                webservice.Credentials = new System.Net.NetworkCredential(pUsername, pPassword);
                if (pTestServer == 1) webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air2/services/MachtigingenWS";
                if (pTestServer == 2) webservice.Url = "https://dbrbms-acc.agro.nl/osbbms20_air1/services/MachtigingenWS";
                DierenWS.dierenRequestType t = new DierenWS.dierenRequestType();
                t.requestID = String.Format("VSMM{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                t.selMeldingeenheid = UBN;
                t.selRelatienummerHouder = RelatienummerHouder;
                t.selVlagsoortCodeReden = "?";
                t.selDierSoort = Diersoort;
                t.selPeilDatum = DateTime.Now.ToString("ddMMyyyy"); ;
                var resp = webservice.raadplegenDieren(t);
            }
            catch (Exception exc)
            {
                unLogger.WriteError($@"{nameof(SOAPLNVALG_Referentie1)}{nameof(raadplegenmeldingen)}:{exc.ToString()}");
            }
            return true;
        }

        //melden

    }
}
