using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using System.Configuration;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.SOAPCDD;
using VSM.RUMA.CORE.SOAPLNV;
using VSM.RUMA.SOAPSANITEL;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CRVSOAP;
using System.Threading.Tasks;

namespace VSM.RUMA.CORE
{

    public class RUMAIenRMeldingen : VSM.RUMA.CORE.AFIenRMeldingen
    {
        //SSL vingerafdruk i&r hond vbk certficaat
        private const string PKIIENRHOND = "0f4fe3599e7a054e20d94e492f17fe9a";//"1123dccc3dbc880799727abe037bdfe272dc449e"; //"9c91fb824d1de4ff19dd277bce5619a3cf6da942";

        //SSL vingerafdruk virbac certficaat
        private const string PKIVIRBAC = "82edc2da281504d577f09a528ef1aeff06922597";//"5e2019391350c702f8f1440a8ae742f30a823cd3";

        List<AGRO_LABELS> mMutSoort;
        List<AGRO_LABELS> mCRVIRHaarKleur;
        List<LABELS> mGeslacht;
        List<LABELS> mLNVIRHaarKleur;
        List<LABELS> mMutResultaat;
        List<LABELS> mMutHaarKleur;
        List<LABELS> mDHZSoort;
        List<LABELS> mSTIRHaarKleur;
        List<LABELS> mSTIRRasType;
        List<LABELS> mSTIRAfvoerReden;

        Dictionary<int, List<MUTATION>> IDRMut = new Dictionary<int, List<MUTATION>>();

        public override void VulArrays(int pProgId, DBConnectionToken mToken)
        {
            //unLogger.WriteDebug($@"{nameof(VulArrays)} start");
            if (mMutSoort == null || mMutSoort.Count() == 0) { mMutSoort = new List<AGRO_LABELS>(); }
            if (mGeslacht == null || mGeslacht.Count() == 0) { mGeslacht = new List<LABELS>(); }
            if (mLNVIRHaarKleur == null || mLNVIRHaarKleur.Count() == 0) { mLNVIRHaarKleur = new List<LABELS>(); }
            if (mCRVIRHaarKleur == null || mCRVIRHaarKleur.Count() == 0) { mCRVIRHaarKleur = new List<AGRO_LABELS>(); }
            if (mMutResultaat == null || mMutResultaat.Count() == 0) { mMutResultaat = new List<LABELS>(); ; }
            if (mMutHaarKleur == null || mMutHaarKleur.Count() == 0) { mMutHaarKleur = new List<LABELS>(); }
            if (mDHZSoort == null || mDHZSoort.Count() == 0) { mDHZSoort = new List<LABELS>(); }
            if (mSTIRHaarKleur == null || mSTIRHaarKleur.Count() == 0) { mSTIRHaarKleur = new List<LABELS>(); }
            if (mSTIRRasType == null || mSTIRRasType.Count() == 0) { mSTIRRasType = new List<LABELS>(); }
            if (mSTIRAfvoerReden == null || mSTIRAfvoerReden.Count() == 0) { mSTIRAfvoerReden = new List<LABELS>(); }

            //mGeslacht = new List<LABELS>();
            //mLNVIRHaarKleur = new List<LABELS>();
            //mCRVIRHaarKleur = new List<AGRO_LABELS>();
            //mMutResultaat = new List<LABELS>();
            //mMutHaarKleur = new List<LABELS>();
            //mDHZSoort = new List<LABELS>();
            //mSTIRHaarKleur = new List<LABELS>();
            //mSTIRRasType = new List<LABELS>();
            //mSTIRAfvoerReden = new List<LABELS>();

            int lCountryCode = Convert.ToInt32(utils.getLabelsLabcountrycode());
            if (mToken != null)
            {
                AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);

                mMutSoort = mMutSoort.Count() == 0 ? lMstb.GetAgroLabels(102, lCountryCode, 0, pProgId) : mMutSoort;
                mCRVIRHaarKleur = mCRVIRHaarKleur.Count()==0? lMstb.GetAgroLabels(108, lCountryCode, 0, 0):   mCRVIRHaarKleur;
                mGeslacht = mGeslacht.Count()==0? lMstb.GetLabels(4, lCountryCode): mGeslacht;
                mSTIRAfvoerReden = mSTIRAfvoerReden.Count()==0? lMstb.GetLabels(2000, lCountryCode) : mSTIRAfvoerReden;
                mMutResultaat = mMutResultaat.Count()==0? lMstb.GetLabels(80, lCountryCode): mMutResultaat;
                mDHZSoort = mDHZSoort.Count() == 0 ? lMstb.GetLabels(145, lCountryCode) : mDHZSoort;

                switch (pProgId)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 6:
                    case 7:
                        mLNVIRHaarKleur = mLNVIRHaarKleur.Count()==0? lMstb.GetLabels(16, lCountryCode): mLNVIRHaarKleur;
                        mSTIRHaarKleur = mSTIRHaarKleur.Count()==0? lMstb.GetLabels(108, lCountryCode): mSTIRHaarKleur;
                        mSTIRRasType = mSTIRRasType.Count()==0? lMstb.GetLabels(95, lCountryCode): mSTIRRasType;
                        mMutHaarKleur = mMutHaarKleur.Count()==0? lMstb.GetLabels(9, lCountryCode): mMutHaarKleur;
                        break;

                    case 3:
                        //mCRDIRHaarKleur =  Facade.GetInstance().getSaveToDB(mToken).GetLabels(0, lCountryCode);
                        //mMutHaarKleur = Facade.GetInstance().getSaveToDB(mToken).GetLabels(0, lCountryCode);
                        break;
                    case 5:
                        //mCRDIRHaarKleur = Facade.GetInstance().getSaveToDB(mToken).GetLabels(0, lCountryCode);
                        //mMutHaarKleur = Facade.GetInstance().getSaveToDB(mToken).GetLabels(0, lCountryCode);
                        break;
                }

            }
            //SendInseminunLogger.WriteDebug($@"{nameof(VulArrays)} end");
        }

        public override List<IRreportresult> ZetIenRMeldingenKlaar(int pUBNId, DBConnectionToken mToken)
        {

            List<IRreportresult> lMutationList = new List<IRreportresult>();
            //String lLand = GetCountry(mToken, pUBNId).LandAfk3;
            //if (lLand == "BEL")
            //{
            //    int MaxString = 255;
            //    String lUsername, lPassword;
            //    String Taal = utils.getcurrentlanguage();
            //    String lStatus = string.Empty;
            //    String lOmschrijving = string.Empty;
            //    String lMeldingsnr = string.Empty;
            //    Win32SANITRACEALG DLLcall = new Win32SANITRACEALG();
            //    AFSavetoDB lSaveToDB = Facade.GetInstance().getSaveToDB(mToken);
            //    UBN lUBN = lSaveToDB.GetubnById(pUBNId);

            //    StringBuilder QRY_MUTATION_NO_PASSPORTNR = new StringBuilder();
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" SELECT DISTINCT(MUTATION.Lifenumber)");
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" FROM MUTATION ");
            //    QRY_MUTATION_NO_PASSPORTNR.AppendFormat(" WHERE UBNId={0}", pUBNId);
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" AND MUTATION.Internalnr>0");
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" AND LENGTH(MUTATION.Lifenumber)>0");
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" AND MUTATION.VersienrVertrekluik=0");
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" AND MUTATION.CodeMutation != 3");
            //    QRY_MUTATION_NO_PASSPORTNR.Append(" AND MUTATION.CodeMutation != 4");
            //    DataTable dtLevensnrsPassports = lSaveToDB.GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTATION_NO_PASSPORTNR);
            //    string pDierfile = unLogger.getLogDir("IenR") + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + "_VersienrPaspoort_pDierfile.csv";
            //    string pOutputfile = unLogger.getLogDir("IenR") + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + "_VersienrPaspoort_Outputfile.csv";
            //    string LogFile = unLogger.getLogDir("IenR") + "STIRVersienrPaspoort" + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";

            //    StreamWriter wr2 = new StreamWriter(pDierfile, false);
            //    StringBuilder sbCSLevensnrs = new StringBuilder();

            //    StringBuilder sbCSVLine = new StringBuilder();
            //    //foreach (DataColumn Colum in dtLevensnrsPassports.Columns)
            //    //{
            //    //    sbCSVLine.AppendFormat("\"{0}\";", Colum.ColumnName);
            //    //}
            //    //sbCSLevensnrs.AppendLine(sbCSVLine.ToString());

            //    foreach (DataRow drRegel in dtLevensnrsPassports.Rows)
            //    {
            //        sbCSVLine = new StringBuilder();
            //        foreach (DataColumn Colum in dtLevensnrsPassports.Columns)
            //        {
            //            //sbCSVLine.AppendFormat("\"{0}\";", drRegel[Colum]);
            //            sbCSVLine.Append(drRegel[Colum].ToString().Trim());
            //        }
            //        sbCSLevensnrs.AppendLine(sbCSVLine.ToString());
            //    }
            //    wr2.Write(sbCSLevensnrs.ToString());
            //    wr2.Close();

            //    FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNId, 9992);
            //    lUsername = fusoap.UserName;
            //    lPassword = fusoap.Password;
            //    int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            //    if (lTestServer > 0 && lUsername == String.Empty)
            //    {
            //        lUsername = "TESTXML";
            //    }
            //    string[] KolsSanitracePaspoortnrs = { "levensnummer", "versienrpaspoort" };
            //    DLLcall.STVersienrPaspoort(lUsername, lPassword, lTestServer, Taal, pDierfile, pOutputfile, LogFile, ref lStatus, ref lOmschrijving, MaxString);
            //    if (lStatus != "F")
            //    {
            //        DataTable dtPasspoort = utils.GetCsvData(KolsSanitracePaspoortnrs, pOutputfile, ';', "PaspoortVersie");
            //        int PaspoortVersie;
            //        foreach (DataRow drRegel in dtPasspoort.Rows)
            //        {
            //            if (drRegel["versienrpaspoort"] != DBNull.Value && Int32.TryParse(drRegel["versienrpaspoort"].ToString(), out PaspoortVersie))
            //            {
            //                List<MUTATION> Mutations = lSaveToDB.GetMutationsByLifeNumber(drRegel["levensnummer"].ToString());

            //                foreach (MUTATION Mutation in Mutations)
            //                {
            //                    Mutation.VersienrVertrekluik = PaspoortVersie;
            //                    lSaveToDB.SaveMutation(Mutation);
            //                }
            //            }
            //        }
            //    }
            //}
            return lMutationList;
        }
 
        public override List<MUTATION__ADMIN> ListIenRAdmin(int pProgramId, DBConnectionToken mToken)
        {
            StringBuilder QRY_MUTATION = new StringBuilder();
            QRY_MUTATION.Append(" SELECT DISTINCT MUTATION.*, BEDRIJFTO.Bedrijfsnaam as FarmNameTo,");
            QRY_MUTATION.Append(" BEDRIJFFROM.Bedrijfsnaam as FarmNameFrom, ANIMAL.AniLifeNumber ");
            //QRY_MUTATION.Append(" al.LabLabel as MutationSoort");
            QRY_MUTATION.Append(" FROM MUTATION");
            QRY_MUTATION.Append(" JOIN agrofactuur.BEDRIJF as bedr");
            QRY_MUTATION.Append(" ON agrofactuur.bedr.UBNid = MUTATION.UbnId");
            QRY_MUTATION.Append(" LEFT JOIN agrofactuur.UBN as BEDRIJFFROM");
            QRY_MUTATION.Append(" ON BEDRIJFFROM.Bedrijfsnummer = MUTATION.FarmNumberFrom");
            QRY_MUTATION.Append(" LEFT JOIN agrofactuur.UBN as BEDRIJFTO");
            QRY_MUTATION.Append(" ON BEDRIJFTO.Bedrijfsnummer = MUTATION.FarmNumberTo");
            QRY_MUTATION.Append(" LEFT JOIN ANIMAL");
            QRY_MUTATION.Append(" ON ANIMAL.AniAlternateNumber = MUTATION.LifeNumber");
            //QRY_MUTATION.Append(" LEFT JOIN agrofactuur.AGRO_LABELS al ON ");
            QRY_MUTATION.Append(" AND MUTATION.LifeNumber != ''");
            QRY_MUTATION.Append(" AND NOT ISNULL(MUTATION.LifeNumber)");
            QRY_MUTATION.AppendFormat(" WHERE agrofactuur.bedr.Programid = {0} AND ANIMAL.AniId>0 ", pProgramId);
            QRY_MUTATION.Append(" ORDER BY MUTATION.MutationDate DESC ");
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTATION);
            List<MUTATION__ADMIN> lMutationList = new List<MUTATION__ADMIN>();
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                MUTATION__ADMIN lMutation = new MUTATION__ADMIN();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lMutation, ResultRow);
                //if (lMutation.LifeNumberUnique == "")
                //    lMutation.LifeNumberUnique = lMutation.Lifenumber;
                //lMutation.AlternateLifeNumber = ResultRow["AniLifeNumber"].ToString();
                lMutationList.Add(lMutation);
            }
            return lMutationList;
        }

        public override List<MUTALOG__ADMIN> ListVorigeIenRAdmin(int pProgramId, DBConnectionToken mToken)
        {
            StringBuilder QRY_MUTALOG = new StringBuilder();
            QRY_MUTALOG.Append(" SELECT DISTINCT MUTALOG.*, BEDRIJFTO.Bedrijfsnaam as FarmNameTo,");
            QRY_MUTALOG.Append(" BEDRIJFFROM.Bedrijfsnaam as FarmNameFrom, ANIMAL.AniLifeNumber");
            QRY_MUTALOG.Append(" FROM MUTALOG");
            QRY_MUTALOG.Append(" JOIN agrofactuur.BEDRIJF");
            QRY_MUTALOG.Append(" ON agrofactuur.BEDRIJF.UBNid = MUTALOG.UbnId");
            QRY_MUTALOG.Append(" LEFT JOIN agrofactuur.UBN as BEDRIJFFROM");
            QRY_MUTALOG.Append(" ON BEDRIJFFROM.Bedrijfsnummer = MUTALOG.FarmNumberFrom");
            QRY_MUTALOG.Append(" LEFT JOIN agrofactuur.UBN as BEDRIJFTO");
            QRY_MUTALOG.Append(" ON BEDRIJFTO.Bedrijfsnummer = MUTALOG.FarmNumberTo");
            QRY_MUTALOG.Append(" LEFT JOIN ANIMAL");
            QRY_MUTALOG.Append(" ON ANIMAL.AniAlternateNumber = MUTALOG.LifeNumber");
            QRY_MUTALOG.Append(" AND MUTALOG.LifeNumber != ''");
            QRY_MUTALOG.Append(" AND NOT ISNULL(MUTALOG.LifeNumber)");
            QRY_MUTALOG.AppendFormat("  WHERE agrofactuur.BEDRIJF.Programid = {0} AND ANIMAL.AniId>0 ", pProgramId);
            QRY_MUTALOG.Append(" ORDER BY MUTALOG.MutationDate DESC ");
            //QRY_MUTALOG.Append(" LIMIT 200");
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTALOG);
            List<MUTALOG__ADMIN> lMutlogList = new List<MUTALOG__ADMIN>();
            int i = 0;
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                if (i == 250)
                {
                    return lMutlogList;
                }

                MUTALOG__ADMIN lMutation = new MUTALOG__ADMIN();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lMutation, ResultRow);
                //if (lMutation.LifeNumberUnique == "")
                //    lMutation.LifeNumberUnique = lMutation.Lifenumber;
                //lMutation.AlternateLifeNumber = ResultRow["AniLifeNumber"].ToString();
                lMutlogList.Add(lMutation);

            }
            return lMutlogList;
        }

        public override List<MUTATION> ListIenRMeldingen(String pUBNid, DBConnectionToken mToken)
        {
            StringBuilder QRY_MUTATION = new StringBuilder();
            QRY_MUTATION.Append(" SELECT * ");
            QRY_MUTATION.Append(" FROM MUTATION ");
            QRY_MUTATION.AppendFormat(" WHERE UBNId={0} AND MUTATION.Internalnr>0 AND LENGTH(MUTATION.Lifenumber)>0 ", pUBNid);
            QRY_MUTATION.Append(" ORDER BY MUTATION.MutationDate DESC ");

            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTATION);
            List<MUTATION> lMutationList = new List<MUTATION>();
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                MUTATION lMutation = new MUTATION();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lMutation, ResultRow);
                lMutationList.Add(lMutation);
            }
            return lMutationList;
        }

        public override List<MUTALOG> ListVorigeIenR(String pUBNid, DBConnectionToken mToken)
        {
            StringBuilder QRY_MUTALOG = new StringBuilder();
            QRY_MUTALOG.Append(" SELECT * ");
            QRY_MUTALOG.Append(" FROM MUTALOG");
            //QRY_MUTALOG.Append(" LEFT OUTER JOIN ANIMAL");
            //QRY_MUTALOG.Append(" ON ANIMAL.AniAlternateNumber = MUTALOG.LifeNumber");
            QRY_MUTALOG.Append(" WHERE MUTALOG.LifeNumber != ''");
            QRY_MUTALOG.Append(" AND NOT ISNULL(MUTALOG.LifeNumber)");
            QRY_MUTALOG.AppendFormat(" AND MUTALOG.UbnId = {0}", pUBNid);
            QRY_MUTALOG.Append(" ORDER BY MUTALOG.MutationDate DESC ");
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTALOG);
            List<MUTALOG> lMutlogList = new List<MUTALOG>();
            int i = 0;
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                if (i == 50)
                {
                    return lMutlogList;
                }

                MUTALOG lMutation = new MUTALOG();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lMutation, ResultRow);
                //if (lMutation.LifeNumberUnique == "")
                //    lMutation.LifeNumberUnique = lMutation.Lifenumber;
                //lMutation.AlternateLifeNumber = ResultRow["AniLifeNumber"].ToString();
                lMutlogList.Add(lMutation);

            }
            return lMutlogList;
        }

        #region Hond getmeldingen

        /// <summary>
        /// SanitairyUnit (String) geleend voor eu paspoort BullITBNumber bij honden
        /// </summary>
        /// <param name="pFokkerThrId"></param>
        /// <param name="mToken"></param>
        /// <returns></returns>
        public override List<MUTATION> ListIenRMeldingenHond(int pFokkerThrId, DBConnectionToken mToken)
        {
            StringBuilder QRY_MUTATION = new StringBuilder();
            QRY_MUTATION.Append(" SELECT * ,al.LabLabel AS MutationSoort, a.BullITBNumber ");
            QRY_MUTATION.Append(" FROM MUTATION");
            QRY_MUTATION.Append(" JOIN agrofactuur.AGRO_LABELS al ON (al.LabId=MUTATION.CodeMutation AND al.LabKind=102 AND al.LabCountry='528' AND al.LabProgId=0 AND al.LAbProgramId=0 ) ");
            QRY_MUTATION.Append(" LEFT JOIN ANIMAL a ON a.AniLifeNumber=MUTATION.LifeNumber  AND a.AniID>0  ");
            QRY_MUTATION.Append(" WHERE MUTATION.LifeNumber != '' AND  MUTATION.Internalnr>0 ");
            QRY_MUTATION.Append(" AND NOT ISNULL(MUTATION.LifeNumber)");
            //QRY_MUTATION.AppendFormat(" AND MUTATION.ThrId={0} OR MUTATION.tbv_ThrID={1} ", pFokkerThrId, pFokkerThrId);
            QRY_MUTATION.AppendFormat(" AND  MUTATION.tbv_ThrID={0}   ", pFokkerThrId);
            QRY_MUTATION.Append(" GROUP BY MUTATION.Internalnr ORDER BY MUTATION.CodeMutation ");
            //DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataTable(QRY_MUTATION);
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTATION, MissingSchemaAction.Add);
            List<MUTATION> lMutationList = new List<MUTATION>();
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                MUTATION lMutation = new MUTATION();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lMutation, ResultRow);
                if (lMutation.LifeNumberUnique == "")
                    lMutation.LifeNumberUnique = lMutation.Lifenumber;
                lMutation.AlternateLifeNumber = lMutation.Lifenumber;
                lMutation.SanitairyUnit = ResultRow["BullITBNumber"] == DBNull.Value ? "" : ResultRow["BullITBNumber"].ToString();
                lMutationList.Add(lMutation);
            }
            return lMutationList;
        }

        /// <summary>
        /// SanitairyUnit (String) geleend voor eu paspoort BullITBNumber bij honden
        /// </summary>
        /// <param name="pFokkerThrId"></param>
        /// <param name="mToken"></param>
        /// <returns></returns>
        public override List<MUTALOG> ListVorigeIenRHond(int pFokkerThrId, DBConnectionToken mToken)
        {
            StringBuilder QRY_MUTALOG = new StringBuilder();
            QRY_MUTALOG.Append(" SELECT MUTALOG.*,  a.BullITBNumber  ");
            QRY_MUTALOG.Append(" FROM MUTALOG ");
            QRY_MUTALOG.Append(" LEFT JOIN ANIMAL a ON a.AniLifeNumber=MUTALOG.LifeNumber ");
            QRY_MUTALOG.Append(" WHERE MUTALOG.LifeNumber != '' AND MUTALOG.Internalnr>0 ");
            QRY_MUTALOG.Append(" AND NOT ISNULL(MUTALOG.LifeNumber)");
            //QRY_MUTALOG.AppendFormat(" AND  MUTALOG.ThrId={0} OR MUTALOG.tbv_ThrID={1}", pFokkerThrId, pFokkerThrId);
            QRY_MUTALOG.AppendFormat(" AND  MUTALOG.tbv_ThrID={0}", pFokkerThrId);
            QRY_MUTALOG.Append(" ORDER BY MUTALOG.CodeMutation ");
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_MUTALOG, MissingSchemaAction.Add);
            List<MUTALOG> lMutlogList = new List<MUTALOG>();
            int i = 0;
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                if (i == 50)
                {
                    return lMutlogList;
                }

                MUTALOG lMutation = new MUTALOG();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lMutation, ResultRow);
                if (lMutation.LifeNumberUnique == "")
                    lMutation.LifeNumberUnique = lMutation.Lifenumber;
                lMutation.AlternateLifeNumber = lMutation.Lifenumber;
                lMutation.SanitairyUnit = ResultRow["BullITBNumber"] == DBNull.Value ? "" : ResultRow["BullITBNumber"].ToString();
                lMutlogList.Add(lMutation);

            }
            return lMutlogList;
        }

        #endregion

        public override List<DHZ> ListDHZMeldingen(String pUBNid, DBConnectionToken mToken)
        {
            StringBuilder QRY_DHZ = new StringBuilder();
            QRY_DHZ.Append(" SELECT * ");
            QRY_DHZ.Append(" FROM DHZ");
            QRY_DHZ.AppendFormat(" WHERE DHZ.UbnId = {0}", pUBNid);
            QRY_DHZ.Append(" ORDER BY DHZ.InsDate DESC ");
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_DHZ);
            List<DHZ> lDHZList = new List<DHZ>();
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                DHZ lDHZ = new DHZ();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lDHZ, ResultRow);
                lDHZList.Add(lDHZ);
            }
            return lDHZList;
        }

        public override List<DHZLOG> ListVorigeDHZ(String pUBNid, DBConnectionToken mToken)
        {
            StringBuilder QRY_DHZLOG = new StringBuilder();
            QRY_DHZLOG.Append(" SELECT * ");
            QRY_DHZLOG.Append(" FROM DHZLOG");
            QRY_DHZLOG.AppendFormat(" WHERE DHZLOG.UbnId = {0}", pUBNid);
            QRY_DHZLOG.Append(" ORDER BY DHZLOG.InsDate DESC ");
            DataTable dtResult = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getLastChildConnection(), QRY_DHZLOG);
            List<DHZLOG> lDHZLOGList = new List<DHZLOG>();
            foreach (DataRow ResultRow in dtResult.Rows)
            {
                DHZLOG lDHZLOG = new DHZLOG();
                Facade.GetInstance().getSaveToDB(mToken).GetDataBase().FillObject(lDHZLOG, ResultRow);
                lDHZLOGList.Add(lDHZLOG);
            }
            return lDHZLOGList;
        }

        public override bool InsertMutation(MUTATION pMutation, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).SaveMutation(pMutation);
        }

        public override bool InsertMutLog(MUTALOG pMutLog, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).InsertMutLog(pMutLog);
        }

        public override bool InsertDHZ(DHZ pDHZ, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).InsertDHZ(pDHZ);
        }

        public override bool InsertDHZLog(DHZLOG pDHZLog, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).InsertDHZLog(pDHZLog);
        }

        public override bool DeleteDHZ(DHZ pDHZ, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).DeleteDHZ(pDHZ);
        }

        public override bool DeleteMutation(MUTATION pMutation, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(pMutation);
        }

        public override bool UpdateReport(MUTATION pMutation, DBConnectionToken mToken)
        {
            return Facade.GetInstance().getSaveToDB(mToken).UpdateMutationReport(pMutation);
        }

        public override String getMutSoort(int CodeMutation)
        {
            if (mMutSoort != null)
            {


                var MutSoortLabel = from CurLabel in mMutSoort
                                    where CurLabel.LabID == CodeMutation
                                    select CurLabel;
                if (MutSoortLabel.Count() == 0) return String.Empty;
                return MutSoortLabel.First().LabLabel;
            }
            else { return ""; }
        }

        public override String getDHZSoort(int InsInfo)
        {
            var InsInfoLabel = from CurLabel in mDHZSoort
                               where CurLabel.LabId == InsInfo
                               select CurLabel;
            if (InsInfoLabel.Count() == 0) return String.Empty;
            return InsInfoLabel.First().LabLabel;
        }

        public override String getMutGeslacht(int Sex)
        {
            try
            {
                var SexLabel = from CurLabel in mGeslacht
                               where CurLabel.LabId == Sex
                               select CurLabel;
                if (SexLabel.Count() == 0) return String.Empty;
                return SexLabel.First().LabLabel;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Fout bij getMutGeslacht", ex);
                return String.Empty;
            }
        }

        public override String getMutHaarKleur(int Haircolor, string pAniHairColor_Memo)
        {
            try
            {
                if (pAniHairColor_Memo != "")
                { return pAniHairColor_Memo; }
                if (mMutHaarKleur != null)
                {
                    var HaarkleurLabel = from CurLabel in mMutHaarKleur
                                         where CurLabel.LabId == Haircolor
                                         select CurLabel;
                    if (HaarkleurLabel.Count() == 0) return String.Empty;
                    return HaarkleurLabel.First().LabLabel;
                }
                else { return ""; }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Fout bij getMutHaarKleur", ex);
                return String.Empty;
            }
        }

        public override String getMutResultaat(int Returnresult)
        {
            try
            {

                var ResultaatLabel = from CurLabel in mMutResultaat
                                     where CurLabel.LabId == Returnresult
                                     select CurLabel;
                if (ResultaatLabel.Count() == 0) return String.Empty;
                return ResultaatLabel.First().LabLabel;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Fout bij getMutResultaat", ex);
                return String.Empty;
            }
        }

        protected override String CRDIRHaarKleur(int Haircolor, String Land, string pAniHairColor_Memo)
        {
            try
            {
                if (pAniHairColor_Memo != "")
                { return pAniHairColor_Memo; }
                if (Land == "BEL")
                {
                    return STIRHaarKleur(Haircolor, pAniHairColor_Memo);
                }
                else
                {
                    return LNVIRHaarKleur(Haircolor, pAniHairColor_Memo);
                }

            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Fout bij CRDIRHaarKleur", ex);
                return String.Empty;
            }
        }

        protected override String STIRHaarKleur(int Haircolor, string pAniHairColor_Memo)
        {
            try
            {
                if (pAniHairColor_Memo != "")
                { return pAniHairColor_Memo; }
                var HaarkleurLabel = from CurLabel in mSTIRHaarKleur
                                     where CurLabel.LabId == Haircolor
                                     select CurLabel;
                if (HaarkleurLabel.Count() == 0)
                {

                    return String.Empty;
                }
                return HaarkleurLabel.First().LabLabel;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Fout bij STIRHaarKleur", ex);
                return String.Empty;
            }
        }

        protected override String LNVIRHaarKleur(int Haircolor, string pAniHairColor_Memo)
        {
            try
            {
                if (pAniHairColor_Memo != "")
                { return pAniHairColor_Memo; }
                var HaarkleurLabel = from CurLabel in mLNVIRHaarKleur
                                     where CurLabel.LabId == Haircolor
                                     select CurLabel;
                if (HaarkleurLabel.Count() == 0)
                {

                    return String.Empty;
                }
                return HaarkleurLabel.First().LabLabel;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Fout bij LNVIRHaarKleur", ex);
                return String.Empty;
            }
        }

        public override int LNVPasswordCheck(String pUsername, String pPassword)
        {
            int lTestServer = 0;//Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            SOAPLNV.SOAPLNVALG_Referentie1 soap = new VSM.RUMA.CORE.SOAPLNV.SOAPLNVALG_Referentie1();
            return soap.LoginCheck(pUsername, pPassword, lTestServer);
        }

        //public override SOAPLOG MeldIR(List<MUTATION> pRecords, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, LABELSConst.ChangedBy changedBy = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0)
        //{
        //    AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);

        //    var ubn = DB.GetubnById(pUBNid);
        //    string prefix = $"{nameof(RUMAIenRMeldingen)}.{nameof(MeldIR)} Bedrijf: '{ubn?.Bedrijfsnummer}' ProgId: {pProgId} ProgramId: {pProgramid} MUTATIONs: {pRecords.Count()} - ";

        //    unLogger.WriteInfo($"{prefix} Gestart.");

        //    unLogger.WriteDebugObject("IRlog", "Meld IenR", pRecords);

        //    SOAPLOG Result;
        //    foreach (MUTATION pRecord in pRecords)
        //    {
        //        if (pRecord.SendTo == 0)
        //        {
        //            if (pProgId == 25)
        //            {
        //                pRecord.SendTo = 35;
        //                pRecord.Changed_By = (int)changedBy;
        //                pRecord.SourceID = sourceId;
        //                DB.SaveMutation(pRecord);
        //            }
        //            else
        //            {
        //                BEDRIJF bedr = DB.GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
        //                string defIenRaction = getdefIenRaction(mToken, pUBNid, pProgId, pProgramid);
        //                FARMCONFIG IenRCom = DB.getFarmConfig(bedr.FarmId, "VerstuurIenR", defIenRaction);
        //                pRecord.SendTo = IenRCom.ValueAsInteger();

        //                pRecord.Changed_By = (int)changedBy;
        //                pRecord.SourceID = sourceId;

        //                DB.SaveMutation(pRecord);
        //            }

        //        }
        //    }
        //    var newList = pRecords.GroupBy(x => new { x.CodeMutation, x.SendTo });
        //    foreach (var groep in newList)
        //    {
        //        switch (groep.ElementAt(0).SendTo)
        //        {
        //            case 26:
        //                Result = MeldSaniTrace(groep.ToList(), pUBNid, pProgId, mToken);
        //                break;
        //            case 27:
        //                //Result = MeldIRFHRS(pRecord, pUBNid, pProgId, pProgramid, mToken, pLNV2IRCredidentials);
        //                break;
        //            case 28:
        //                bool configvalue;
        //                BEDRIJF bedr = DB.GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
        //                bool fcSendAnimals = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendChangedAnimals", "True"), out configvalue) && configvalue;
        //                bool fcSendAnimalName = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendChangedAnimalsName", "False"), out configvalue) && configvalue;
        //                bool fcCRVSendNoName = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendNoName", "True"), out configvalue) && configvalue;
        //                Result = MeldIRCRD(pRecord, pUBNid, mToken, fcSendAnimals, fcSendAnimalName, fcCRVSendNoName, changedBy, sourceId);
        //                break;
        //            case 29:
        //                Result = MeldIRLNVV2IR(pRecord, pUBNid, pProgId, pProgramid, mToken, pLNV2IRCredidentials);
        //                break;
        //            case 33:
        //                Result = MeldSaniTrace(pRecord, pUBNid, pProgId, mToken);
        //                break;
        //            //case 2:
        //            //    Result = MeldIRLNVIR(pRecord, pUBNid, mToken);
        //            //    break;
        //            case 35: //Het zou kunnen dat dit voor meerdere diersoorten gaat gelden
        //                Result = MeldIRHond(pRecord, pUBNid, pProgId, pProgramid, mToken);
        //                break;
        //            case 36:
        //                Result = MeldHitier(pRecord, pUBNid, pProgId, pProgramid, mToken);
        //                break;
        //            case 37:
        //                Result = MeldDCF(pRecord, pUBNid, pProgId, pProgramid, mToken);
        //                break;
        //            default:
        //                Result = MeldIRNL(pRecord, pUBNid, pProgId, pProgramid, mToken);
        //                break;
        //        }
        //    }
        //}
        public override List<SOAPLOG> MeldIRV2(List<MUTATION> pRecords, UBN ubn, BEDRIJF b, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, LABELSConst.ChangedBy changedBy = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0)
        {
            List<SOAPLOG> result = new List<SOAPLOG>();
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            
            string prefix = $"{nameof(RUMAIenRMeldingen)}.{nameof(MeldIR)} Bedrijf: '{ubn?.Bedrijfsnummer}' ProgId: {b.ProgId} ProgramId: {b.Programid} MUTATIONs:{pRecords.Count()}:";
            unLogger.WriteInfo($@"{prefix} {nameof(VulArrays)}");
            VulArrays(b.ProgId, mToken);
            unLogger.WriteInfo($"{prefix} Gestart.");
            int sendto = 0;
            if (b.ProgId == 25)
            {
                sendto = 35;
            }
            else 
            {
                string defIenRaction = getdefIenRaction(mToken, ubn.UBNid, b.ProgId, b.Programid);
                string IenRCom = DB.GetFarmConfigValue(b.FarmId, "VerstuurIenR", defIenRaction);
                sendto = Convert.ToInt32(IenRCom);
            }
            foreach (var pRecord in pRecords)
            {
                if (pRecord.SendTo == 0)
                {
                    if (b.ProgId == 25)
                    {
                        pRecord.SendTo = 35;
                    }
                    else
                    {
                        pRecord.SendTo = sendto;
                        pRecord.Changed_By = (int)changedBy;
                        pRecord.SourceID = sourceId;
                        DB.SaveMutation(pRecord);
                    }
                }
            }
            int[] sendtos = { 26, 27, 28, 29, 33, 35, 36, 37 };
            if (sendto == 27)
            {
                sendto = 29;
            }
            if (!sendtos.Contains(sendto))
            {
                if (b.ProgId == 1 || b.ProgId == 4)
                {
                    
                    sendto = 28;
                }
                else
                {
                    sendto = 29;
                }
            }
            switch (sendto)
            {
                case 26:
                    bool useIRMeldingensoapversion_Sanitrace = false;
                    if (ConfigurationManager.AppSettings["useIRMeldingensoapversion_Sanitrace"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Sanitrace"]))
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Sanitrace"], out useIRMeldingensoapversion_Sanitrace);
                    }
                    var configsoapS = DB.GetConfigValue(b.Programid, b.FarmId, "useIRMeldingensoapversion_Sanitrace");
                    if (!string.IsNullOrWhiteSpace(configsoapS))
                    {
                        bool.TryParse(configsoapS, out useIRMeldingensoapversion_Sanitrace);
                    }


                    result.AddRange(MeldSaniTrace(pRecords, ubn.UBNid, b.ProgId, mToken, false));
                    
                    break;
                //case 27:
                //    Result = MeldIRFHRS(pRecord, pUBNid, pProgId, pProgramid, mToken, pLNV2IRCredidentials);
                //    break;
                case 28:
               
                    bool useIRMeldingensoapversion_Crv = false;
                    if (ConfigurationManager.AppSettings["useIRMeldingensoapversion_Crv"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Crv"]))
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Crv"], out useIRMeldingensoapversion_Crv);
                    }
                    var configsoap = DB.GetConfigValue(b.Programid, b.FarmId, "useIRMeldingensoapversion_Crv");
                    if (!string.IsNullOrWhiteSpace(configsoap))
                    {
                        bool.TryParse(configsoap, out useIRMeldingensoapversion_Crv);
                    }
                    bool configvalue;
                    bool fcSendAnimals = bool.TryParse(DB.GetConfigValue(b.Programid, b.FarmId, "CRVSendChangedAnimals", "True"), out configvalue) && configvalue;
                    bool fcSendAnimalName = bool.TryParse(DB.GetConfigValue(b.Programid, b.FarmId, "CRVSendChangedAnimalsName", "False"), out configvalue) && configvalue;
                    bool fcCRVSendNoName = bool.TryParse(DB.GetConfigValue(b.Programid, b.FarmId, "CRVSendNoName", "True"), out configvalue) && configvalue;
                    foreach (var pRecord in pRecords)
                    {
                        if (useIRMeldingensoapversion_Crv)
                        {
                            result.Add(MeldIRCRD(pRecord, ubn.UBNid, mToken, fcSendAnimals, fcSendAnimalName, fcCRVSendNoName,true, changedBy, sourceId));
                        }
                        else
                        {
                            result.Add(MeldIRCRD(pRecord, ubn.UBNid, mToken, fcSendAnimals, fcSendAnimalName, fcCRVSendNoName, changedBy, sourceId));
                        }
                    }
                    break;
                case 29:
                    bool useIRMeldingensoapversion_Lnv = false;
                    if (ConfigurationManager.AppSettings["useIRMeldingensoapversion_Lnv"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Lnv"]))
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Lnv"], out useIRMeldingensoapversion_Lnv);
                    }
                    var config = DB.GetConfigValue(b.Programid, b.FarmId, "useIRMeldingensoapversion_Lnv");
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        bool.TryParse(config, out useIRMeldingensoapversion_Lnv);
                    }
                    if (useIRMeldingensoapversion_Lnv)
                    {
                        result.AddRange(MeldIRLNVV2IRV2(pRecords, ubn, b, mToken, pLNV2IRCredidentials));
                    }
                    else
                    {
                        foreach (var pRecord in pRecords)
                        {
                            result.Add(MeldIRLNVV2IR(pRecord, ubn.UBNid, b.ProgId, b.Programid, mToken, pLNV2IRCredidentials));
                        }
                    }
                    break;
                case 33:
                    bool useIRMeldingensoapversion_Sanitrace2 = false;
                    if (ConfigurationManager.AppSettings["useIRMeldingensoapversion_Sanitrace"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Sanitrace"]))
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["useIRMeldingensoapversion_Sanitrace"], out useIRMeldingensoapversion_Sanitrace);
                    }
                    var configsoapS2 = DB.GetConfigValue(b.Programid, b.FarmId, "useIRMeldingensoapversion_Sanitrace");
                    if (!string.IsNullOrWhiteSpace(configsoapS2))
                    {
                        bool.TryParse(configsoapS2, out useIRMeldingensoapversion_Sanitrace);
                    }
                    result.AddRange(MeldSaniTrace(pRecords, ubn.UBNid, b.ProgId, mToken, useIRMeldingensoapversion_Sanitrace2));

                    //foreach (var pRecord in pRecords)
                    //{
                    //    result.Add(MeldSaniTrace(pRecord, ubn.UBNid, b.ProgId, mToken));
                    //}
                    break;
                case 35: //Het zou kunnen dat dit voor meerdere diersoorten gaat gelden
                    foreach (var pRecord in pRecords)
                    {
                       result.Add(MeldIRHond(pRecord, ubn.UBNid, b.ProgId, b.Programid, mToken));
                    }
                    break;
                case 36:
                    foreach (var pRecord in pRecords)
                    {
                        result.Add(MeldHitier(pRecord, ubn.UBNid, b.ProgId, b.Programid, mToken));
                    }
                    break;
                case 37:
                    foreach (var pRecord in pRecords)
                    {
                        result.Add(MeldDCF(pRecord, ubn.UBNid, b.ProgId, b.Programid, mToken));
                    }
                    break;
                default:

                
                    break;
            }

            return result;
        }

        private IEnumerable<SOAPLOG> MeldIRLNVV2IRV2(List<MUTATION> pRecords, UBN ubn, BEDRIJF b, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials)
        {
            String lUsername, lPassword;
            int lTestServer;
            List<SOAPLOG> Result = new List<SOAPLOG>();
            List<Soaplogmelding> retmeldingen = new List<Soaplogmelding>();
            string Farmnumber = ubn.Bedrijfsnummer;


            try
            {
              
                THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn.ThrID);
                String lBRSnummer = lPersoon.Thr_Brs_Number;

                foreach (var pRecord in pRecords)
                {
                    if (!string.IsNullOrWhiteSpace(pRecord.MeldingNummer) && pRecord.Returnresult == 96 && pRecord.CodeMutation < 100)
                    {
                        //HerstelMelding = 1;
                        pRecord.CodeMutation += 200;
                    }
                }



                FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(ubn.UBNid, b.Programid, b.ProgId, 9992);
                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
                if (pLNV2IRCredidentials != null && pLNV2IRCredidentials.Password != "" && pLNV2IRCredidentials.UserName != "")
                {
                    lUsername = pLNV2IRCredidentials.UserName;
                    lPassword = pLNV2IRCredidentials.Password;
                }

                lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                if (lTestServer > 0)
                {
  

                }
                String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MELDKIND_" + ubn.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";
           
                MeldingenWS ws = new MeldingenWS();
                var groepen = pRecords.GroupBy(x=>new { x.CodeMutation});
                foreach (var groep in groepen)
                {
                    switch (groep.ElementAt(0).CodeMutation)
                    {
                        case (int)LABELSConst.CodeMutation.AANVOER:
                            //Aanvoer
                            LogFile = LogFile.Replace("MELDKIND", "Aanvoer");

                            retmeldingen.AddRange(ws.LNVIRaanvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
 
                            break;
                        case (int)LABELSConst.CodeMutation.GEBOORTE:
                            //Geboorte
                            LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                            retmeldingen.AddRange(ws.LNVIRgeboortemeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                             
                            break;
                        case (int)LABELSConst.CodeMutation.DOODGEBOREN:
                            //Doodgeb.
                            LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                            retmeldingen.AddRange(ws.LNVIRdoodgeborenmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId));
                            
                            break;
                        case (int)LABELSConst.CodeMutation.AFVOER:
                            //Afvoer
                            LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                        
                            break;
                        case (int)LABELSConst.CodeMutation.IKBAFVOER:
                            //IKB afvoer
                            LogFile = LogFile.Replace("MELDKIND", "IKBAfvoer");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId,0));
                         
                          
                            break;
                        case (int)LABELSConst.CodeMutation.DOOD:
                            //Dood
                            //Rendac doet het doodmelden


                            //rundvee 1
                            //stieren 2
                            //schapen 3
                            //zoogkoeien 4
                            //geiten 5
                            //witvlees 6
                            //rose 7
                            LogFile = LogFile.Replace("MELDKIND", "Dood");
                            if (b.ProgId == 3 || b.ProgId == 5)
                            {
                                retmeldingen.AddRange(ws.LNVIRdoodmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                              
                            }
                            else
                            {
                                foreach (var pRecord in groep)
                                {
                                    var sls = new Soaplogmelding
                                    {
                                        Changed_By = pRecord.Changed_By,
                                        Date = DateTime.Now.Date,
                                        Time = DateTime.Now,
                                        FarmNumber = pRecord.Farmnumber,
                                        Kind = pRecord.CodeMutation,
                                        Lifenumber = pRecord.Lifenumber,
                                        meldnummer = pRecord.MeldingNummer,
                                        SourceID = ubn.ThrID,
                                        ThrId = ubn.ThrID,
                                        Status = "F",
                                        Code = "",
                                        Omschrijving = $"Rendac verzorgt de doodmelding van {pRecord.Lifenumber}."
                                    };
                                    retmeldingen.Add(sls);
                                   
                                }
                            }
                            break;
                        case (int)LABELSConst.CodeMutation.IMPORT:
                            //Import
                            LogFile = LogFile.Replace("MELDKIND", "Import");
                           
                            retmeldingen.AddRange(ws.LNVIRimportmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                          
                         
                            break;
                        case (int)LABELSConst.CodeMutation.AANAFVOER:
                            //Aan-/afvoer
                            LogFile = LogFile.Replace("MELDKIND", "AanAfvoer");
                            foreach (var pRecord in groep)
                            {
                                var sls = new Soaplogmelding
                                {
                                    Changed_By = pRecord.Changed_By,
                                    Date = DateTime.Now.Date,
                                    Time = DateTime.Now,
                                    FarmNumber = pRecord.Farmnumber,
                                    Kind = pRecord.CodeMutation,
                                    Lifenumber = pRecord.Lifenumber,
                                    meldnummer = pRecord.MeldingNummer,
                                    SourceID = ubn.ThrID,
                                    ThrId = ubn.ThrID,
                                    Status = "F",
                                    Code = "",
                                    Omschrijving = "Not implemented."
                                };
                                retmeldingen.Add(sls);
                            }
                            /*
                                    SELECT * FROM agrobase_calf.MUTALOG m
                                    WHERE m.codemutation=8
                                    order by m.MutationDate desc
                                    alleen in calf 6x uit 2010
                             */

                            break;
                        case (int)LABELSConst.CodeMutation.EXPORT:
                            //Export
                            LogFile = LogFile.Replace("MELDKIND", "Export");
                            retmeldingen.AddRange(ws.LNVIRexportmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                           

                            break;
                        case (int)LABELSConst.CodeMutation.SLACHT:
                            //Slacht
                            LogFile = LogFile.Replace("MELDKIND", "Slacht");
                            retmeldingen.AddRange(ws.LNVIRslachtmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                
                            break;
                        case (int)LABELSConst.CodeMutation.INSCHAREN:
                            LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                      
                            retmeldingen.AddRange(ws.LNVIRaanvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                         

                            break;
                        case (int)LABELSConst.CodeMutation.UITSCHAREN:
                            //Uitscharen
                            LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                         

                            break;
                        case (int)LABELSConst.CodeMutation.NOODSLACHT:
                            //Noodslacht
                            LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                            foreach (var pRecord in groep)
                            {
                                retmeldingen.Add(ws.LNVIRnoodslachtV2(pRecord, lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));

                            }
                            break;
                        case 16:
                            //Q-Krts vacc1
                            LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc1");
                            foreach (var pRecord in groep)
                            {
                                var sls = new Soaplogmelding
                                {
                                    Changed_By = pRecord.Changed_By,
                                    Date = DateTime.Now.Date,
                                    Time = DateTime.Now,
                                    FarmNumber = pRecord.Farmnumber,
                                    Kind = pRecord.CodeMutation,
                                    Lifenumber = pRecord.Lifenumber,
                                    meldnummer = pRecord.MeldingNummer,
                                    SourceID = ubn.ThrID,
                                    ThrId = ubn.ThrID,
                                    Status = "F",
                                    Code = "",
                                    Omschrijving = "Melding is niet meer nodig, alleen vacc2 en Herhaling is verplicht."
                                };
                                retmeldingen.Add(sls);
                            }
                            break;
                        case (int)LABELSConst.CodeMutation.QKrtsvacc2:
                            LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc2");
                            retmeldingen.AddRange(ws.LNVIRQkoortsV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                            

                            break;
                        case (int)LABELSConst.CodeMutation.QKrtsvaccH:
                            LogFile = LogFile.Replace("MELDKIND", "QKrtsvaccH");
                            retmeldingen.AddRange(ws.LNVIRQkoortsV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 0));
                           

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
                            foreach (var pRecord in groep)
                            {
                                 
                                string lStatus = "";
                                string lCode = "";
                                string lOmschrijving = "";
                                ws.LNVIntrekkenMelding(pRecord, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pRecord.MeldingNummer, b.ProgId, ref lStatus, ref lCode, ref lOmschrijving);
                                var sls = new Soaplogmelding
                                {
                                    Changed_By = pRecord.Changed_By,
                                    Date = DateTime.Now.Date,
                                    Time = DateTime.Now,
                                    FarmNumber = pRecord.Farmnumber,
                                    Kind = pRecord.CodeMutation,
                                    Lifenumber = pRecord.Lifenumber,
                                    meldnummer = pRecord.MeldingNummer,
                                    SourceID = ubn.ThrID,
                                    ThrId = ubn.ThrID,
                                    Status = lStatus,
                                    Code = lCode,
                                    Omschrijving = lOmschrijving
                                };
                                retmeldingen.Add(sls);
                            }
                            break;

                        //herstellen ook
                        case 201:
                            //Aanvoer
                            LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer");

                            retmeldingen.AddRange(ws.LNVIRaanvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer,  Farmnumber, lBRSnummer, b.ProgId, 1));
                       
                          
                            break;
                        case 202:
                            //Geboorte
                            LogFile = LogFile.Replace("MELDKIND", "HerstelGeboorte");
                            retmeldingen.AddRange(ws.LNVIRgeboortemeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                            
                            break;
                        case 203:
                            //Doodgeb. er is geen herstelmelding indicator
                            LogFile = LogFile.Replace("MELDKIND", "HerstelDoodgeb");
                            //retmelding = ws.LNVIRdoodgeborenmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);

                            foreach (var pRecord in groep)
                            {
                                var sls = new Soaplogmelding
                                {
                                    Changed_By = pRecord.Changed_By,
                                    Date = DateTime.Now.Date,
                                    Time = DateTime.Now,
                                    FarmNumber = pRecord.Farmnumber,
                                    Kind = pRecord.CodeMutation,
                                    Lifenumber = pRecord.Lifenumber,
                                    meldnummer = pRecord.MeldingNummer,
                                    SourceID = ubn.ThrID,
                                    ThrId = ubn.ThrID,
                                    Status = "F",
                                    Code = "",
                                    Omschrijving = "Not implemented."
                                };
                                retmeldingen.Add(sls);
                            }
                            break;
                        case 204:
                            //Afvoer
                            LogFile = LogFile.Replace("MELDKIND", "HerstelAfvoer");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                          
                            break;
                        case 205:
                            //IKB afvoer
                            LogFile = LogFile.Replace("MELDKIND", "HerstelIKBAfvoer");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                        
                            break;
                        case 206:
                            //Dood
                            LogFile = LogFile.Replace("MELDKIND", "HerstelDood");
                            retmeldingen.AddRange(ws.LNVIRdoodmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                        
                            break;
                        case 207:
                            //Import
                            LogFile = LogFile.Replace("MELDKIND", "HerstelImport");
                            retmeldingen.AddRange(ws.LNVIRimportmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                          
                            break;
                        case 208:
                            //Aan-/afvoer
                            LogFile = LogFile.Replace("MELDKIND", "HerstelAanAfvoer");
                            foreach (var pRecord in groep)
                            {
                                var sls = new Soaplogmelding
                                {
                                    Changed_By = pRecord.Changed_By,
                                    Date = DateTime.Now.Date,
                                    Time = DateTime.Now,
                                    FarmNumber = pRecord.Farmnumber,
                                    Kind = pRecord.CodeMutation,
                                    Lifenumber = pRecord.Lifenumber,
                                    meldnummer = pRecord.MeldingNummer,
                                    SourceID = ubn.ThrID,
                                    ThrId = ubn.ThrID,
                                    Status = "F",
                                    Code = "",
                                    Omschrijving = "Not implemented."
                                };
                                retmeldingen.Add(sls);
                            }
                            //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                            //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                            //            1, pRecord.MeldingNummer,
                            //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                            //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                            //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                            //            1, pRecord.MeldingNummer,
                            //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                            break;
                        case 209:
                            //Export
                            LogFile = LogFile.Replace("MELDKIND", "HerstelExport");
                            retmeldingen.AddRange(ws.LNVIRexportmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                             
                            break;
                        case 210:
                            //Slacht
                            LogFile = LogFile.Replace("MELDKIND", "HerstelSlacht");
                            retmeldingen.AddRange(ws.LNVIRslachtmeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                             
                            break;
                        case 211:
                            LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer11");
                         
                            retmeldingen.AddRange(ws.LNVIRaanvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                            
                            break;
                        case 212:
                            //Uitscharen
                            LogFile = LogFile.Replace("MELDKIND", "HerstelUitscharen");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                            
                            break;
                        case 213:
                            //Noodslacht
                            LogFile = LogFile.Replace("MELDKIND", "HerstelNoodslacht");
                            retmeldingen.AddRange(ws.LNVIRafvoermeldingV2(groep.ToList(), lUsername, lPassword, lTestServer, Farmnumber, lBRSnummer, b.ProgId, 1));
                             
                            break;
                    }
                }
            }
            catch (Exception Exc)
            {
               
                 
                unLogger.WriteDebug(Exc.ToString());
            }
            foreach (var sls in retmeldingen)
            {
                var record = from n in pRecords
                             where n.Lifenumber == sls.Lifenumber
                             select n;
                if (record.Count() > 0)
                {
                    MUTATION pRecord = record.ElementAt(0);
                    SOAPLOG sl = new SOAPLOG();
                    sl.Changed_By = sls.Changed_By;
                    sl.Code = sls.Code;
                    sl.Date = sls.Date;
                    sl.FarmNumber = sls.FarmNumber;
                    sl.Kind = sls.Kind;
                    sl.Lifenumber = sls.Lifenumber;
                    string lMeldingsnr = sls.meldnummer;
                    sl.Omschrijving = sls.Omschrijving;
                    sl.SourceID = sls.SourceID;
                    sl.Status = sls.Status;
                    sl.SubKind = sls.SubKind;
                    sl.TaskLogID = sls.TaskLogID;
                    sl.ThrId = sls.ThrId;
                    sl.Time = sls.Time;

                    sl.FarmNumber = pRecord.Farmnumber;
                    sl.Lifenumber = pRecord.Lifenumber;
                    pRecord.MeldResult = sl.Omschrijving;
                    //let op lStatus kan leeg zijn na doen van intrekmelding CodeMutation 101 t/m 113 
                    if (sl.Equals("G")) pRecord.Returnresult = 1;
                    else if (sl.Equals("F")) pRecord.Returnresult = 2;
                    else if (sl.Equals("W")) pRecord.Returnresult = 3;
                    else pRecord.Returnresult = 98;

                    if (sl.Equals("G") || sl.Equals("W") || IRNietHerhalen(sl.Code, pRecord))
                    {

                        lock (Facade.GetInstance())
                        {
                            MUTALOG MutLog = ConverttoMutLog(pRecord);
                            MutLog.ReportDate = sl.Date;
                            MutLog.ReportTime = sl.Time;
                            if (lMeldingsnr != "")
                            {
                                MutLog.MeldingNummer = lMeldingsnr;
                            }
                            if (pRecord.CodeMutation > 100 && pRecord.CodeMutation < 115)
                            {
                                MutLog.MeldingNummerOrg = pRecord.MeldingNummer;
                            }

                            try
                            {
                                if (pRecord.MovId > 0)
                                {
                                    MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                                    if (mov.ReportDate <= new DateTime(1900, 1, 1))
                                    {
                                        mov.ReportDate = sl.Date;
                                        Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                                    }


                                }
                                else if (pRecord.EventId > 0)
                                {
                                    BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                                    if (bir.Reportdate <= new DateTime(1900, 1, 1))
                                    {
                                        bir.Reportdate = sl.Date;
                                        Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                                    }

                                }
                            }
                            catch
                            {
                                unLogger.WriteInfo("Fout bij opslaan Reportdate");
                            }


                            if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                                unLogger.WriteError("IRlog", "Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                            if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                            {

                                pRecord.Report = 99;
                                UpdateReport(pRecord, mToken);
                                unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                                MUTATION delmut;
                                for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                                {
                                    if (DeleteMutation(pRecord, mToken))
                                        break;
                                    else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                                    {
                                        if (pRecord.EventId > 0)
                                            delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                        else if (pRecord.MovId > 0)
                                            delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                        else
                                        {
                                            delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                        }
                                        if (delmut.Internalnr == 0)
                                        {
                                            delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                        }
                                        if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                            break;
                                        else
                                            unLogger.WriteError("IRlog", "Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                                    }
                                    else
                                    {
                                        unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        pRecord.MeldResult = sl.Omschrijving;
                        Facade.GetInstance().getSaveToDB(mToken).UpdateMutation(pRecord);
                    }
                    Result.Add(sl);
                }
            }
            return Result;
        }

        public override SOAPLOG MeldIR(MUTATION pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, LABELSConst.ChangedBy changedBy = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);

            var ubn = DB.GetubnById(pUBNid);
            string prefix = $"{nameof(RUMAIenRMeldingen)}.{nameof(MeldIR)} Bedrijf: '{ubn?.Bedrijfsnummer}' ProgId: {pProgId} ProgramId: {pProgramid} MUTATION: {pRecord?.Internalnr} -";

            unLogger.WriteDebug($"{prefix} Gestart.");

            unLogger.WriteDebugObject("IRlog", "Meld IenR", pRecord);
                
            SOAPLOG Result;
            if (pRecord.SendTo == 0)
            {
                if (pProgId == 25)
                {
                    pRecord.SendTo = 35;
                }
                else
                {
                    BEDRIJF bedr = DB.GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
                    string defIenRaction = getdefIenRaction(mToken, pUBNid, pProgId, pProgramid);
                    string IenRCom = DB.GetFarmConfigValue(bedr.FarmId, "VerstuurIenR", defIenRaction);
                    pRecord.SendTo = Convert.ToInt32(IenRCom);

                    pRecord.Changed_By = (int)changedBy;
                    pRecord.SourceID = sourceId;

                    DB.SaveMutation(pRecord);
                }
            }

            switch (pRecord.SendTo)
            {
                case 26:
                    Result = MeldSaniTrace(pRecord, pUBNid, pProgId, mToken);
                    break;
                case 27:
                    Result = MeldIRFHRS(pRecord, pUBNid, pProgId, pProgramid, mToken, pLNV2IRCredidentials);
                    break;
                case 28:
                    bool configvalue;
                    BEDRIJF bedr = DB.GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
                    bool fcSendAnimals = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendChangedAnimals", "True"), out configvalue) && configvalue;
                    bool fcSendAnimalName = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendChangedAnimalsName", "False"), out configvalue) && configvalue;
                    bool fcCRVSendNoName = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendNoName", "True"), out configvalue) && configvalue;
                    Result = MeldIRCRD(pRecord, pUBNid, mToken, fcSendAnimals, fcSendAnimalName, fcCRVSendNoName, changedBy, sourceId);
                    break;
                case 29:
                    bool usesoapversion_lnv = false;
                    if (ConfigurationManager.AppSettings["usesoapversion_lnv"] != null)
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["usesoapversion_lnv"], out usesoapversion_lnv);
                    }
                    BEDRIJF bedrijf = DB.GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
                    var config = DB.GetConfigValue(bedrijf.Programid, bedrijf.FarmId, "usesoapversion_lnv");
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        bool.TryParse(config, out usesoapversion_lnv);
                    }
                    Result = MeldIRLNVV2IR(pRecord, pUBNid, pProgId, pProgramid, mToken, pLNV2IRCredidentials, usesoapversion_lnv);
                    break;
                case 33:
                    Result = MeldSaniTrace(pRecord, pUBNid, pProgId, mToken);
                    break;
                //case 2:
                //    Result = MeldIRLNVIR(pRecord, pUBNid, mToken);
                //    break;
                case 35: //Het zou kunnen dat dit voor meerdere diersoorten gaat gelden
                    Result = MeldIRHond(pRecord, pUBNid, pProgId, pProgramid, mToken);
                    break;
                case 36:
                    Result = MeldHitier(pRecord, pUBNid, pProgId, pProgramid, mToken);
                    break;
                case 37:
                    Result = MeldDCF(pRecord, pUBNid, pProgId, pProgramid, mToken);
                    break;
                default:
                    Result = MeldIRNL(pRecord, pUBNid, pProgId, pProgramid, mToken);
                    break;
            }

            unLogger.WriteDebug($"{prefix} Klaar.");
            return Result;
        }

        private SOAPLOG MeldHitier(MUTATION pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken)
        {
            //Duitsland
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);
            UBN lUbn = lMstb.GetubnById(pUBNid);
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9997);
            SOAPLOG Result = new SOAPLOG();
            Result.Code = "F";
            Result.Date = DateTime.Now;
            Result.FarmNumber = lUbn.Bedrijfsnummer;
            Result.Internalnr = pRecord.Internalnr;
            Result.Kind = pRecord.CodeMutation;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("nognietbeschikbaar", "Nog niet beschikbaar");

            Result.Time = DateTime.Now;
            Result.Status = "F";
            return Result;
            int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            if (lTestServer > 0)
            {

            }
            HIClient DLLcall = new HIClient(lTestServer, unLogger.getLogDir("IenR"), unLogger.getLogDir());

            String LogFile = unLogger.getLogDir("IenR") + "MeldingHitier_MELDKIND_" + pRecord.Farmnumber + "_" + pRecord.Lifenumber + "_" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "MeldingHitier_" + pRecord.Farmnumber + "_" + pRecord.Lifenumber + "-" + DateTime.Now.Ticks + ".log");
            string lStatus = "";
            string lCode = "";
            string lOmschrijving = "";
            string lMeldingsnr = "";
            if (pRecord.SendTo == 36)
            {
                switch (pRecord.CodeMutation)
                {
                    case 1:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 2:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                        //DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 3:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                        //DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 4:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 5:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "IKBAfvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 6:
                        //Dood
                        //Rendac doet het doodmelden


                        //rundvee 1
                        //stieren 2
                        //schapen 3
                        //zoogkoeien 4
                        //geiten 5
                        //witvlees 6
                        //rose 7
                        LogFile = LogFile.Replace("MELDKIND", "Dood");
                        if (pProgId == 3 || pProgId == 5)
                        {
                            //DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                            //                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                            //                    pProgId, pRecord.MutationDate,
                            //                    HerstelMelding, pRecord.MeldingNummer,
                            //                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        }

                        break;
                    case 7:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "Import");
                        unLogger.WriteInfo("Importmelding: " + pRecord.Lifenumber);
                        //DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                        //            pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 8:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "AanAfvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 9:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "Export");
                        //DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 10:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "Slacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 11:
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 12:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 13:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 16:
                        //Q-Krts vacc1
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc1");
                        //DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                        //            1,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 17:
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc2");
                        //DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                        //            2,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 18:
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvaccH");
                        //DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                        //            3,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
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
                        MUTALOG lIntrekTemp = ConverttoMutLog(pRecord);
                        //Result = LNV2MeldingIntrekken(lIntrekTemp, lUBN.UBNid, pProgId, pProgramId, mToken);

                        lStatus = Result.Status;
                        lCode = Result.Code;
                        lOmschrijving = Result.Omschrijving;

                        break;

                    //herstellen ook
                    case 201:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 202:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "HerstelGeboorte");
                        //DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 203:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDoodgeb");
                        //DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 204:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAfvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 205:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelIKBAfvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 206:
                        //Dood
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDood");
                        //DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 207:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "HerstelImport");
                        //DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                        //            pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 208:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanAfvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 209:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "HerstelExport");
                        //DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 210:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelSlacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 211:
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer11");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 212:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "HerstelUitscharen");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 213:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelNoodslacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
            }
            else
            {
                Result.Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("verkeerdeorganisatie", "Verkeerde Organisatie om te melden");

            }
            Result.Kind = pRecord.CodeMutation;
            if (Result.Status == "")
            {
                Result.Status = lStatus;
                Result.Code = lCode;
                Result.Omschrijving = lMeldingsnr + " " + lOmschrijving;
            }
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            //let op lStatus kan leeg zijn na doen van intrekmelding CodeMutation 101 t/m 113 
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;

            if (lStatus.Equals("G") || lStatus.Equals("W"))// || IRNietHerhalen(lCode, pRecord))
            {

                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    if (lMeldingsnr != "")
                    {
                        MutLog.MeldingNummer = lMeldingsnr;
                    }
                    if (pRecord.CodeMutation > 100 && pRecord.CodeMutation < 115)
                    {
                        MutLog.MeldingNummerOrg = pRecord.MeldingNummer;
                    }

                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }


                    if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                        unLogger.WriteError("IRlog", "Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                    if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                    {

                        pRecord.Report = 99;
                        UpdateReport(pRecord, mToken);
                        unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                        MUTATION delmut;
                        for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                        {
                            if (DeleteMutation(pRecord, mToken))
                                break;
                            else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                            {
                                if (pRecord.EventId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                else if (pRecord.MovId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                else
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (delmut.Internalnr == 0)
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                    break;
                                else
                                    unLogger.WriteError("IRlog", "Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                            else
                            {
                                unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                        }
                    }
                }
            }
            return Result;
        }

        private SOAPLOG MeldDCF(MUTATION pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken)
        {
            //Denemarken
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);
            UBN lUbn = lMstb.GetubnById(pUBNid);
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9998);
            SOAPLOG Result = new SOAPLOG
            {
                Code = "F",
                Date = DateTime.Now,
                FarmNumber = lUbn.Bedrijfsnummer,
                Internalnr = pRecord.Internalnr,
                Kind = pRecord.CodeMutation,
                Lifenumber = pRecord.Lifenumber,
                Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("nognietbeschikbaar", "Nog niet beschikbaar"),
                Time = DateTime.Now,
                Status = "F"
            };
            return Result;
            int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            if (lTestServer > 0)
            {

            }
            String LogFile = unLogger.getLogDir("IenR") + "MeldingDCF_MELDKIND_" + pRecord.Farmnumber + "_" + pRecord.Lifenumber + "_" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "MeldingDCF_" + pRecord.Farmnumber + "_" + pRecord.Lifenumber + "-" + DateTime.Now.Ticks + ".log");
            string lStatus = "";
            string lCode = "";
            string lOmschrijving = "";
            string lMeldingsnr = "";
            if (pRecord.SendTo == 37)
            {
                switch (pRecord.CodeMutation)
                {
                    case 1:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 2:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                        //DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 3:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                        //DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 4:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 5:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "IKBAfvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 6:
                        //Dood
                        //Rendac doet het doodmelden


                        //rundvee 1
                        //stieren 2
                        //schapen 3
                        //zoogkoeien 4
                        //geiten 5
                        //witvlees 6
                        //rose 7
                        LogFile = LogFile.Replace("MELDKIND", "Dood");
                        if (pProgId == 3 || pProgId == 5)
                        {
                            //DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                            //                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                            //                    pProgId, pRecord.MutationDate,
                            //                    HerstelMelding, pRecord.MeldingNummer,
                            //                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        }

                        break;
                    case 7:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "Import");
                        unLogger.WriteInfo("Importmelding: " + pRecord.Lifenumber);
                        //DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                        //            pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 8:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "AanAfvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 9:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "Export");
                        //DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 10:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "Slacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 11:
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 12:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 13:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 16:
                        //Q-Krts vacc1
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc1");
                        //DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                        //            1,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 17:
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc2");
                        //DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                        //            2,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 18:
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvaccH");
                        //DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                        //            3,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
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
                        MUTALOG lIntrekTemp = ConverttoMutLog(pRecord);
                        //Result = LNV2MeldingIntrekken(lIntrekTemp, lUBN.UBNid, pProgId, pProgramId, mToken);

                        lStatus = Result.Status;
                        lCode = Result.Code;
                        lOmschrijving = Result.Omschrijving;

                        break;

                    //herstellen ook
                    case 201:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 202:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "HerstelGeboorte");
                        //DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 203:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDoodgeb");
                        //DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 204:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAfvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 205:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelIKBAfvoer");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 206:
                        //Dood
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDood");
                        //DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 207:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "HerstelImport");
                        //DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                        //            pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 208:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanAfvoer");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 209:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "HerstelExport");
                        //DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 210:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelSlacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 211:
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer11");
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 212:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "HerstelUitscharen");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 213:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelNoodslacht");
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
            }
            else
            {
                Result.Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("verkeerdeorganisatie", "Verkeerde Organisatie om te melden");

            }
            Result.Kind = pRecord.CodeMutation;
            if (Result.Status == "")
            {
                Result.Status = lStatus;
                Result.Code = lCode;
                Result.Omschrijving = lMeldingsnr + " " + lOmschrijving;
            }
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            //let op lStatus kan leeg zijn na doen van intrekmelding CodeMutation 101 t/m 113 
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;

            if (lStatus.Equals("G") || lStatus.Equals("W"))// || IRNietHerhalen(lCode, pRecord))
            {

                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    if (lMeldingsnr != "")
                    {
                        MutLog.MeldingNummer = lMeldingsnr;
                    }
                    if (pRecord.CodeMutation > 100 && pRecord.CodeMutation < 115)
                    {
                        MutLog.MeldingNummerOrg = pRecord.MeldingNummer;
                    }

                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }


                    if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                        unLogger.WriteError("IRlog", "Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                    if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                    {

                        pRecord.Report = 99;
                        UpdateReport(pRecord, mToken);
                        unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                        MUTATION delmut;
                        for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                        {
                            if (DeleteMutation(pRecord, mToken))
                                break;
                            else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                            {
                                if (pRecord.EventId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                else if (pRecord.MovId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                else
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (delmut.Internalnr == 0)
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                    break;
                                else
                                    unLogger.WriteError("IRlog", "Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                            else
                            {
                                unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                        }
                    }
                }
            }

            return Result;
        }

        private SOAPLOG MeldIRHond(MUTATION pRecord, int pUBNid, int pProgId, int pProgramId, DBConnectionToken mToken)
        {
            SOAPIRHond IenRHond = new SOAPIRHond();
            int MaxString = 255;
            String lUsername, lPassword;
            int lTestServer = 1;
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);



            string pKVKaanleveraar = "";
            // Luc 12-01-16 overbodige Third opvragen verwijderd
            //THIRD t = lMstb.GetThirdByHouseNrAndZipCode("80", "5836AH");
            pKVKaanleveraar = "16085487";// "160.85.487";// t.ThrKvkNummer;// "235472137321";// t.ThrKvkNummer;


            SOAPLOG Result = new SOAPLOG();
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;
            //if (lBedrijf.FarmId > 0)
            //{

            String CertificaatVingerAfdruk; // MeldCertificaat Toevoegen
            String pCertificaatnaam; // CN naam van het certificaat (pki.ienrhond.nl of pkivirbac.ienrhond.nl)
            switch (pProgramId)
            {

                case 2500://	VBK - Particulier
                case 2599://	VBK - Administrator
                case 2550://	VBK - Kennelhouder
                case 2551://	VBK - Kennelhouder Light
                case 2570://	VBK - Chipper
                    //CertificaatVingerAfdruk = "9c91fb824d1de4ff19dd277bce5619a3cf6da942";
                    CertificaatVingerAfdruk = PKIIENRHOND;
                    pCertificaatnaam = "pki.ienrhond.nl";
                    break;
                case 2501://	Virbac - Particulier
                case 2571://	Virbac - Chipper
                case 2598://	Virbac - Adminitrator
                case 2521://	Virbac - Asiel
                    CertificaatVingerAfdruk = PKIVIRBAC;
                    //CertificaatVingerAfdruk = "5e2019391350c702f8f1440a8ae742f30a823cd3";
                    pCertificaatnaam = "pkivirbac.ienrhond.nl";
                    break;
                default:


                    switch (pUBNid)
                    {
                        case 109123://	VBK
                            //CertificaatVingerAfdruk = "9c91fb824d1de4ff19dd277bce5619a3cf6da942";
                            CertificaatVingerAfdruk = PKIIENRHOND;
                            pCertificaatnaam = "pki.ienrhond.nl";
                            break;
                        case 110040://	Virbac 
                            CertificaatVingerAfdruk = PKIVIRBAC;
                            pCertificaatnaam = "pkivirbac.ienrhond.nl";
                            //CertificaatVingerAfdruk = "5e2019391350c702f8f1440a8ae742f30a823cd3";
                            break;
                        default:
                            lStatus = "F";
                            lOmschrijving = "Deze administratie is niet gemachtigd om I&R Hond meldingen te doen";
                            Result.Kind = pRecord.CodeMutation;
                            Result.Status = lStatus;
                            Result.Code = lCode;
                            Result.Omschrijving = lOmschrijving;
                            Result.FarmNumber = pRecord.Farmnumber;
                            Result.Lifenumber = pRecord.Lifenumber;
                            return Result;
                    }
                    break;
            }
            if (ConfigurationManager.AppSettings["CertificaatVingerAfdruk"] != null)
            {
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CertificaatVingerAfdruk"]))
                {
                    CertificaatVingerAfdruk = ConfigurationManager.AppSettings["CertificaatVingerAfdruk"];
                }
            }
            ANIMAL lAnimal = lMstb.GetAnimalByLifenr(pRecord.Lifenumber);
            EVENT lChipevent = new EVENT();
            THIRD lChipperVerplicht = lMstb.getChipper(pRecord.Lifenumber, out lChipevent);
            if (lChipperVerplicht.ThrId == 0)
            {
                if (pRecord.CodeMutation == 2 || pRecord.CodeMutation == 19)//|| pRecord.CodeMutation == 7)
                {
                    lStatus = "F";
                    lOmschrijving = "Chipper is niet bekend, deze is wel verplicht.";
                    Result.Kind = pRecord.CodeMutation;
                    Result.Status = lStatus;
                    Result.Code = lCode;
                    Result.Omschrijving = lOmschrijving;
                    Result.FarmNumber = pRecord.Farmnumber;
                    Result.Lifenumber = pRecord.Lifenumber;
                    return Result;
                }

            }
            else
            {
                if (pRecord.CodeMutation == 2 || pRecord.CodeMutation == 19)//|| pRecord.CodeMutation == 7)
                { }
                else
                {
                    lChipperVerplicht = new THIRD();//dan gaan we hem niet meesturen 
                }
            }
            DateTime lChipDatum = pRecord.MutationDate;
            if (lChipevent.EveDate > DateTime.MinValue)
            {
                lChipDatum = lChipevent.EveDate;
            }
            THIRD lHouder = lMstb.GetThirdByThirId(pRecord.tbv_ThrID);
           
            long ubnnummerhouder = 0;
            try
            {
                string UbnnummerHond = lMstb.getUbnProperty(pRecord.UbnId, "UbnnummerHond");
                if (!string.IsNullOrWhiteSpace(UbnnummerHond) && Convert.ToBoolean(UbnnummerHond))
                {
                    switch (pProgramId)
                    {

                        case 2500://	VBK - Particulier
                        case 2599://	VBK - Administrator
                        case 2550://	VBK - Kennelhouder
                        case 2551://	VBK - Kennelhouder Light
                        case 2570://	VBK - Chipper
                            ubnnummerhouder = lMstb.getUbnnummerHondVBK(pRecord.tbv_ThrID);
                            break;
                        case 2501://	Virbac - Particulier
                        case 2571://	Virbac - Chipper
                        case 2598://	Virbac - Adminitrator
                        case 2521://	Virbac - Asiel
                            ubnnummerhouder = lMstb.getUbnnummerHondVirbac(pRecord.tbv_ThrID);
                            break;
                        default:
                            ubnnummerhouder = 0;
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError(exc.ToString());
            }

            UBN lUBN = lMstb.GetubnById(pUBNid);


            if (lHouder.ThrId > 0)
            {


                COUNTRY lHouderCountry = lMstb.GetCountryByLandid(int.Parse(lHouder.ThrCountry));
                COUNTRY lChipperCountry = lMstb.GetCountryByLandid(151);
                if (lChipperVerplicht.ThrCountry.Trim() != "")
                {
                    int lCID = 0;
                    int.TryParse(lChipperVerplicht.ThrCountry.Trim(), out lCID);
                    if (lCID > 0)
                    {
                        lChipperCountry = lMstb.GetCountryByLandid(lCID);
                    }
                }
                //String lBRSnummer = Owner.Thr_Brs_Number;


                DateTime lRegistrationDate = DateTime.MinValue;

                int HerstelMelding = 0;
                if (pRecord.MeldingNummer != String.Empty && pRecord.Returnresult == 96)
                {
                    HerstelMelding = 1;
                }
                //



                FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9992);
                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;



                lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
                String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_Hond_MELDKIND_" + pRecord.Lifenumber + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
                //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP_Hond" + pRecord.Lifenumber + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
                Result.Date = DateTime.Today;
                Result.Time = DateTime.Now;
                Result.Lifenumber = pRecord.Lifenumber;
                Result.FarmNumber = pRecord.Farmnumber;

                int lKenmerkAanleveraar = pRecord.Internalnr;
                int lDiersoort = 101;




                char[] spl = { ' ', '-' };
                string[] lifenr = pRecord.Lifenumber.Split(spl);
                long lChipnr = 0;
                try
                {
                    if (lifenr.Length > 1)
                    {
                        lChipnr = long.Parse(lifenr[1]);
                    }
                    else
                    {
                        lChipnr = long.Parse(pRecord.Lifenumber);
                    }
                }
                catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
                //DateTime lDatumOntvangstOntvanger = pRecord.MutationTime;
                DateTime lDatumOntvangstOntvanger = DateTime.Now;

                //de testserver is nog alleen actief 12-6-2012
                if (lTestServer > 0)
                {
                    lUsername = "";
                    lPassword = "";
                    //lTestServer = 1;
                }


                string pVsmRelatieNr = lHouder.ThrId.ToString();//zou het anders ook niet weten

                String lRelatieFile = utils.getIR_RelatieFile(lChipnr.ToString(), pVsmRelatieNr, lUBN, lHouder, lHouderCountry, lChipperVerplicht, lChipperCountry, 1, 1, Win32SOAPIRALG.GetBaseDir());
                if (lHouderCountry.LandAfk2.ToUpper() != "NL")
                {
                    if (lChipperCountry.LandAfk2 != "NL")
                    {
                        lRelatieFile = utils.getIR_RelatieFile(lChipnr.ToString(), pVsmRelatieNr, lUBN, lHouder, lHouderCountry, lChipperVerplicht, lChipperCountry, 2, 2, Win32SOAPIRALG.GetBaseDir());
                    }
                    else
                    {
                        lRelatieFile = utils.getIR_RelatieFile(lChipnr.ToString(), pVsmRelatieNr, lUBN, lHouder, lHouderCountry, lChipperVerplicht, lChipperCountry, 2, 1, Win32SOAPIRALG.GetBaseDir());

                    }
                }
                else
                {
                    if (lChipperCountry.LandAfk2 != "NL")
                    {
                        lRelatieFile = utils.getIR_RelatieFile(lChipnr.ToString(), pVsmRelatieNr, lUBN, lHouder, lHouderCountry, lChipperVerplicht, lChipperCountry, 1, 2, Win32SOAPIRALG.GetBaseDir());
                    }
                }
                long mnummer = 0;
                if (pRecord.CodeMutation > 100)
                {
                    if (pRecord.MeldingNummer.Trim() != "")
                    {
                        mnummer = long.Parse(pRecord.MeldingNummer);
                    }
                }
                switch (pRecord.CodeMutation)
                {
                    case 1:
                        //Aanvoer
                        //MOVEMENT maanv = lMstb.GetMovementByMovId(pRecord.MovId);
                        //BUYING bui = lMstb.GetBuying(pRecord.MovId);
                        //THIRD lRelatie = lMstb.GetThirdByThirId(maanv.ThrId.ToString());
                        //aanvoer 1-1 en 1-2 werken 
                        //String lRelatieFile1 = utils.getIR_RelatieFile("0001", lBedrijf, lUBN, lUBN, lPersoon, BirthCountry, lPersoon, 1, 1, Win32SOAPIRALG.GetBaseDir());
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                    
                        IenRHond.Aanvoermelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                             pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, ""
                             , lHouder, ubnnummerhouder, lHouderCountry.LandAfk2,
                             ref lRegistrationDate,
                             ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                        //IenRHond.Aanvoermelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar, "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    case 2:
                        //Geboorte
                        //1-1 werkt 1-2 niet
                        COUNTRY BirthCountry = lMstb.GetCountryByLandid(int.Parse(lHouder.ThrCountry));
                        LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                        IenRHond.Geboortemelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            lChipDatum, pRecord.IDRBirthDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, ""
                            , lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, lChipperVerplicht, lChipperCountry.LandAfk2,
                            ref lRegistrationDate,
                             ref lStatus, ref lOmschrijving, ref lMeldingsnr);
                        //DLLcall.IRHondGeboortemelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, lChipDatum, pRecord.IDRBirthDate,
                        //            lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar,
                        //            "", CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;

                    case 4:
                        //Afvoer
                        //String lRelatieFile4 = "";// utils.getIR_RelatieFile(lPersoon, lUBN, Win32SOAPIRALG.GetBaseDir());
                        MOVEMENT mafv = lMstb.GetMovementByMovId(pRecord.MovId);
                        LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                        IenRHond.Afvoermelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, lDiersoort, lKenmerkAanleveraar, "",
                            ref lRegistrationDate,
                             ref lStatus, ref lOmschrijving, ref lMeldingsnr);
                        //DLLcall.IRHondAfvoermelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar,
                        //            "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;

                    case 6:
                        //Dood
                        //String lRelatieFile6 = "";// utils.getIR_RelatieFile(lPersoon, lUBN, Win32SOAPIRALG.GetBaseDir());
                        LogFile = LogFile.Replace("MELDKIND", "Dood");
                        IenRHond.Doodmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, "", lHouder, ubnnummerhouder, lHouderCountry.LandAfk2,
                            ref lRegistrationDate,
                            ref lStatus, ref lOmschrijving, ref lMeldingsnr);
                        //DLLcall.IRHondDoodmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar,
                        //            "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    case 7:
                        //Import
                        //String lRelatieFile7 = "";// utils.getIR_RelatieFile(lPersoon, lUBN, Win32SOAPIRALG.GetBaseDir());
                        //1-1 OK 1-2 niet
                        LogFile = LogFile.Replace("MELDKIND", "Import");
                        IenRHond.Importmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                               pRecord.MutationDate, pRecord.IDRBirthDate, lDatumOntvangstOntvanger, pRecord.CountryCodeDepart,
                               lDiersoort, lKenmerkAanleveraar, "",
                               lHouder, ubnnummerhouder, lHouderCountry.LandAfk2,
                               ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                        //DLLcall.IRHondImportmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr,
                        //            pRecord.MutationDate, pRecord.IDRBirthDate,
                        //            lChipDatum, lDatumOntvangstOntvanger,
                        //            pRecord.CountryCodeDepart,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar,
                        //            "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);

                        break;

                    case 9:
                        //Export
                        //String lRelatieFile9 = "";// utils.getIR_RelatieFile(lPersoon, lUBN, Win32SOAPIRALG.GetBaseDir());
                        //1-1 1-2 ok
                        LogFile = LogFile.Replace("MELDKIND", "Export");
                        IenRHond.Exportmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, "",
                            lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr);
                        //DLLcall.IRHondExportmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar,
                        //            "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;


                    case 19:
                        //Chipnr vervangen cq Omnummeren
                        LogFile = LogFile.Replace("MELDKIND", "Vervangingmelding");
                        List<LEVNRMUT> lvs = lMstb.getLevNrMuts(lAnimal.AniId);
                        var ln = from n in lvs
                                 where n.LevnrNieuw == lChipnr.ToString()
                                 select n;
                        if (ln.Count() > 0)
                        {
                            List<EVENT> chipevents = lMstb.getEventsByDateAniIdKind(ln.ElementAt(0).Datum, lAnimal.AniId, 15);
                            string pReden = "Verloren";
                            if (chipevents.Count() > 0)
                            {
                                pReden = chipevents.ElementAt(0).EveComment;
                            }
                            if (pReden == "")
                            {
                                pReden = "Verloren";
                            }
                            long oldnr = long.Parse(ln.ElementAt(0).LevnrOud);

                            IenRHond.IRHondVervangingmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, oldnr,
                                       lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger, pReden,
                                       lDiersoort, lKenmerkAanleveraar, "", lHouder, ubnnummerhouder, lHouderCountry.LandAfk2,
                                       lChipperVerplicht, lChipperCountry.LandAfk2, ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                            //DLLcall.IRHondVervangingmelding(lUsername, lPassword, lTestServer,
                            //            oldnr, lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                            //            pReden, lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar, "",
                            //            CertificaatVingerAfdruk,
                            //            lRelatieFile, LogFile,
                            //            ref lRegistrationDate,
                            //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);

                            if (lMeldingsnr != null && lMeldingsnr != "")
                            {
                                LEVNRMUT lmut = ln.ElementAt(0);
                                lmut.LNVMeldNr = lMeldingsnr;
                                lMstb.saveLevNrMut(lmut);
                            }
                        }
                        else
                        {
                            lStatus = "F";
                            lOmschrijving = "Chipnummer niet gevonden";
                        }
                        break;
                    case 20:
                        //Vermissing
                        LogFile = LogFile.Replace("MELDKIND", "Vermissing");

                        IenRHond.IRHondVermissingmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, "",
                            lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                        
                        //DLLcall.IRHondVermissingmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar, "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    case 21:
                        //Gevonden
                        LogFile = LogFile.Replace("MELDKIND", "Gevonden");

                        IenRHond.IRHondGevondenmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, "",
                            lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                        //DLLcall.IRHondGevondenmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar, "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    case 22:
                        //Contactmelding
                        LogFile = LogFile.Replace("MELDKIND", "Contactmelding");
                        EVENT ev = lMstb.GetEventdByEventId(pRecord.EventId);
                        IenRHond.IRHondContactmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar,
                            ev.EveComment, "", lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        //DLLcall.IRHondContactmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar, ev.EveComment, "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    case 23:
                        //Adreswijziging
                        LogFile = LogFile.Replace("MELDKIND", "Adreswijziging");
                        IenRHond.IRHondAdreswijzigingmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lChipnr,
                            pRecord.MutationDate, lDatumOntvangstOntvanger, lDiersoort, lKenmerkAanleveraar, "",
                            lHouder, ubnnummerhouder, lHouderCountry.LandAfk2, ref lRegistrationDate,
                               ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                        //DLLcall.IRHondAdreswijzigingmelding(lUsername, lPassword, lTestServer,
                        //            lChipnr, pRecord.MutationDate, lDatumOntvangstOntvanger,
                        //            lDiersoort, lKenmerkAanleveraar, pKVKaanleveraar, "",
                        //            CertificaatVingerAfdruk,
                        //            lRelatieFile, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    //case 201://HERSTEL meldingen ( ER NIET ZIJN)
                    //case 202:
                    //case 204:
                    //case 206:
                    //case 207:
                    //case 209:
                    //case 219:
                    //case 220:
                    //case 221:
                    //case 222:


                    case 101://INTREKmeldingen
                    case 102:
                    case 104:
                    case 106:
                    case 107:
                    case 109:
                    case 119:
                    case 120:
                    case 121:
                    case 122:
                    case 123:
                        string pRedenIntrekking = "Onbekend";//waar moeten ze dat invullen??

                        LogFile = LogFile.Replace("MELDKIND", "intrekmelding");
                        IenRHond.IRHondIntrekmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, mnummer,
                            lDatumOntvangstOntvanger, pRedenIntrekking, lKenmerkAanleveraar, ref lRegistrationDate,
                                    ref lStatus, ref lOmschrijving, ref lMeldingsnr);

                        //DLLcall.IRHondIntrekmelding(lUsername, lPassword, lTestServer,
                        //            mnummer, lDatumOntvangstOntvanger, pRedenIntrekking, lKenmerkAanleveraar, pKVKaanleveraar,
                        //            CertificaatVingerAfdruk, LogFile,
                        //            ref lRegistrationDate,
                        //            ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;

                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
            }
            else
            {
                lStatus = "F";
                lOmschrijving = " Bij melding is geen houder gevonden.";
            }
            //}
            //else 
            //{
            //    lStatus = "F";
            //    lOmschrijving=" Bij melding is geen bedrijf gevonden.";
            //}

            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;
            try
            {
                if (IenRHond.Errsoaplog != null)
                {
                    AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
                    DB.WriteSoapError(IenRHond.Errsoaplog);
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError("Opslaan Hond ErrorSoaplog:" + exc.ToString());
            }
            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode, pRecord))
            {
                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);

                    int[] intrekken = { 101, 102, 104, 106, 107, 109, 119, 120, 121, 122, 123 };
                    if (intrekken.Contains(pRecord.CodeMutation))
                    {
                        MutLog.MeldingNummerOrg = pRecord.MeldingNummer;
                        MutLog.MeldingNummer = lMeldingsnr;
                        MutLog.CodeMutation -= 100;
                        MutLog.Returnresult = 97;
                    }
                    else
                    {
                        MutLog.MeldingNummer = lMeldingsnr;
                    }
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;

                    if (Result.Omschrijving == "")
                    {
                        Result.Omschrijving = " Melding gelukt. ";
                    }
                    unLogger.WriteInfo("I&RHond:Meldingsnr:" + lMeldingsnr);
                    Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken);
                    Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken);
                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }
                }
            }
            else
            {
                if (Result.Omschrijving == "Webservice storing")
                {
                    Result.Omschrijving = "Storing in de verbinding met het ministerie";
                }
            }
            //DLLcall.Dispose();
            if (lTestServer == 1)
            {
                Result.Omschrijving = "Testserver: " + Result.Omschrijving;
            }
            return Result;
        }

        // oude methode
        [Obsolete]
        private SOAPLOG MeldIRNL(MUTATION pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken)
        {
            //ProgId 
            //rundvee 1
            //stieren 2
            //schapen 3
            //zoogkoeien 4
            //geiten 5
            //witvlees 6
            //rose 7
            SOAPLOG Result;
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            BEDRIJF bedr = DB.GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
            switch (pProgId)
            {
                case 1:
                case 4:
                    bool configvalue;
                 
                    bool fcSendAnimals = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendChangedAnimals", "True"), out configvalue) && configvalue;
                    bool fcSendAnimalName = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendChangedAnimalsName", "False"), out configvalue) && configvalue;
                    bool fcCRVSendNoName = bool.TryParse(DB.GetConfigValue(bedr.Programid, bedr.FarmId, "CRVSendNoName", "True"), out configvalue) && configvalue;
                    Result = MeldIRCRD(pRecord, pUBNid, mToken, fcSendAnimals, fcSendAnimalName, fcCRVSendNoName);
                    break;
                case 2:
                case 3:
                case 5:
                case 6:
                case 7:
                case 8:
                default:
                    bool usesoapversion_lnv = false;
                    if (ConfigurationManager.AppSettings["usesoapversion_lnv"] != null)
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["usesoapversion_lnv"], out usesoapversion_lnv);
                    }
                 
                    var config = DB.GetConfigValue(bedr.Programid, bedr.FarmId, "usesoapversion_lnv");
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        bool.TryParse(config, out usesoapversion_lnv);
                    }
                    Result = MeldIRLNVV2IR(pRecord, pUBNid, pProgId, pProgramid, mToken, null, usesoapversion_lnv);
                    break;

            }
            return Result;
        }

        public override string getdefIenRaction(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramId)
        {
            //ProgId 
            //rundvee 1
            //stieren 2
            //schapen 3
            //zoogkoeien 4
            //geiten 5
            //witvlees 6
            //rose 7
            //hond 25
            int[] rietvelders = utils.getRietveldProgramids().ToArray();

            COUNTRY country = GetCountry(mToken, pUBNid);
            switch (pProgId)
            {
                case 1:
                case 4:
                    if (rietvelders.Contains(pProgramId))
                    {
                        return "29";
                    }
                    if (pProgramId == 12000 || pProgramId == 12099)//fhrs
                    {
                        return "29";
                    }
                    //return "28";
                    switch (country.LandId)
                    {
                        case 82:
                            return "36";//HiTier duitsland
                        case 58:
                            return "37";//DCF  denemarken
                        case 21:
                        case 125:
                            return "26";
                        default:
                            return "28";

                    }
                case 2:
                case 3:
                case 5:
                case 6:
                case 7:

                    switch (country.LandId)
                    {
                        case 82:
                            return "36";//HiTier duitsland

                        case 21:
                        case 125:
                            return "26";

                        case 58:

                            return "37";//DCF  denemarken

                        default:
                            return "29";

                    }

                case 25:
                    return "35";
                default:
                    switch (country.LandId)
                    {
                        case 82:
                            return "36";//HiTier duitsland

                        case 21:
                        case 125:
                            return "26";

                        case 58:

                            return "37";//DCF  denemarken
                

                        default:
                            return "29";

                    }
            }
        }

        private static COUNTRY GetCountry(DBConnectionToken mToken, int pUBNid)
        {
            COUNTRY country = new COUNTRY();
            try
            {
                if (pUBNid > 0)
                {
                    UBN UBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
                    if (UBN.ThrID > 0)
                    {
                        THIRD thrid = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(UBN.ThrID);
                        if (thrid.ThrCountry.Trim() != "")
                        {
                            country = Facade.GetInstance().getSaveToDB(mToken).GetCountryByLandid(Convert.ToInt32(thrid.ThrCountry));
                        }
                    }
                }
            }
            catch { }
            return country;
        }

        public override SOAPLOG MeldDHZ(DHZ pRecord, int pUBNid, DBConnectionToken mToken,int changedby, int sourceid)
        {
            //Indien er geen stier informatie is kan er niet gemeld worden.
            if (string.IsNullOrWhiteSpace(pRecord.BullLifeNumber) && string.IsNullOrWhiteSpace(pRecord.BullAInumber) && string.IsNullOrWhiteSpace(pRecord.BullName))
            {
                SOAPLOG res = new SOAPLOG();
                res.Changed_By = changedby;
                res.SourceID = sourceid;
                res.Date = DateTime.Today;
                res.Time = DateTime.Now;
                res.Lifenumber = pRecord.AniLifenumber;
                res.FarmNumber = pRecord.FarmNumber;
                res.Status = "E";               
                res.Omschrijving = $"Doe het zelfmelding: '{pRecord.Internalnr}' voor dier: '{pRecord.AniLifenumber}' op datum: '{pRecord.InsDate}' bevat geen stier info."; ;
                return res;
            }

            //Win32SOAPVBHALG DLLcall = new Win32SOAPVBHALG();
            VSM.RUMA.CRVSOAP.CrvVruchtbaarheid crvvruchtbaarheid = new CRVSOAP.CrvVruchtbaarheid(changedby, sourceid);
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBNid).LandAfk3;
            String Uitvoerende = string.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            Result.Changed_By = changedby;
            Result.SourceID = sourceid;
            UBN ubn = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {
                FTPUSER fusoapold = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }
            if (lUsername == String.Empty || lPassword == String.Empty)
            {
                Result.Kind = 431;
                Result.Status = "F";
                Result.Code = "LOGINCRV";
                Result.Omschrijving = "CRV Gebruikersnaam en/of Wachtwoord niet ingevoerd.";
                unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Gebruikersnaam en/of Wachtwoord niet ingevoerd!", pRecord.FarmNumber, pRecord.AniLifenumber, pRecord.InsDate));

                return Result;
            }
            lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.AniLifenumber;
            Result.FarmNumber = pRecord.FarmNumber;

            ANIMAL an = new ANIMAL();
            an.AniAlternateNumber = pRecord.AniLifenumber;
            an.AniLifeNumber = pRecord.AniLifenumber;
            an.AniId = pRecord.AniId;
            an.AniName = pRecord.AniName;
            an.AniWorkNumber = pRecord.AniWorknumber;

            ANIMAL dbBull = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.BullLifeNumber);
            ANIMAL bull = new ANIMAL();
            if (dbBull != null && dbBull.AniId > 0)
            {
                bull.AniAlternateNumber = dbBull.AniAlternateNumber;
                bull.AniLifeNumber = dbBull.AniAlternateNumber;
                bull.BullAiNumber = pRecord.BullAInumber;
                bull.BullShortName = pRecord.BullName;
            }
            else
            {
                unLogger.WriteWarn($"MeldDHZ - Bedrijf: '{ubn.Bedrijfsnummer}' - Record: {pRecord.Internalnr} - Could not find bull: '{pRecord.BullLifeNumber}', creating custom bull. ");
                bull.AniAlternateNumber = pRecord.BullLifeNumber;
                bull.AniLifeNumber = pRecord.BullLifeNumber;
                bull.BullAiNumber = pRecord.BullAInumber;
                bull.BullShortName = pRecord.BullName;
            }

            String LogFile = unLogger.getLogDir("IenR") + "CRDDHZSOAP" + pRecord.FarmNumber + "-" + DateTime.Now.Ticks + ".log";
            //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDDHZSOAP" + pRecord.FarmNumber + "-" + DateTime.Now.Ticks + ".log");
            int InsKind, DHZKind;
            if (int.TryParse(pRecord.InsInfo, out DHZKind)) InsKind = DHZKind;
            else InsKind = 3;
            switch (InsKind)
            {

                case 2:
 //                   SOAPLOG Rest = crvvruchtbaarheid.SendNatuurlijkeDekking(ubn, an, fusoap, lLand, Uitvoerende, bull, pRecord.InsDate, configHelper.DhzXmlLog);
                    //lStatus = Rest.Status.ToUpper();
                    //lCode = Rest.Code;
                    //lOmschrijving = Rest.Omschrijving;
                    break;
                case 3:
                case 4:
                case 5:
 //                   SOAPLOG Rest5 = crvvruchtbaarheid.SendInsemin(ubn, an, fusoap, lLand, Uitvoerende, bull, pRecord.ChargeNumber, pRecord.InsDate, "", configHelper.DhzXmlLog);
                    //lStatus = Rest5.Status.ToUpper();
                    //lCode = Rest5.Code;
                    //lOmschrijving = Rest5.Omschrijving;
                    break;
                case 9:
//                    SOAPLOG Rest9 = crvvruchtbaarheid.SendGroupMating(ubn, an, fusoap, lLand, Uitvoerende, bull, pRecord.InsDate, pRecord.EndDate, configHelper.DhzXmlLog);
                    //lStatus = Rest9.Status.ToUpper();
                    //lCode = Rest9.Code;
                    //lOmschrijving = Rest9.Omschrijving;
                    break;

            }
            Result.Kind = 500 + InsKind;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("I") || lStatus.Equals("G") || lStatus.Equals("W") || DHZNietHerhalen(lCode.Trim(), pRecord))
            {
                pRecord.ReportDate = Result.Date;
                pRecord.ReportTime = Result.Time;
                Facade.GetInstance().getMeldingen().InsertDHZLog(ConverttoDHZLog(pRecord), mToken);

                if (!Facade.GetInstance().getMeldingen().DeleteDHZ(pRecord, mToken))
                {
                    unLogger.WriteError("DHZlog", "Fout bij verwijderen DHZ Farmnumber = " + pRecord.FarmNumber + " Lifenumber = " + pRecord.AniLifenumber);

                    for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                    {
                        if (Facade.GetInstance().getMeldingen().DeleteDHZ(pRecord, mToken))
                            break;
                        unLogger.WriteError("DHZlog", "Fout bij verwijderen DHZ Farmnumber = " + pRecord.FarmNumber + " Lifenumber = " + pRecord.AniLifenumber);
                    }
                }
                //pRecord.EventID

                if (pRecord.EventID > 0)
                {
                    INSEMIN Insem = Facade.GetInstance().getSaveToDB(mToken).GetInsemin(pRecord.EventID);

                    if (Insem.ReportDate == DateTime.MinValue)
                    {
                        Insem.ReportDate = Result.Date;
                        Insem.ReportStatus = 1;
                        Insem.Changed_By = changedby;
                        Insem.SourceID = sourceid;
                        Facade.GetInstance().getSaveToDB(mToken).SetInseminReportDateAndStatus(Insem);
                    }

                }

            }
            //else
            //    unLogger.WriteError($"DHZ Code: {lCode.Trim()} - Status: '{lStatus}'- Omschrijving: '{lOmschrijving}' - Herhalen.");
            
            return Result;
        }

        public override SOAPLOG MeldTocht(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken, int changedby, int sourceid)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            //Win32SOAPVBHALG DLLcall = new Win32SOAPVBHALG();
            VSM.RUMA.CRVSOAP.CrvVruchtbaarheid crvvruchtbaarheid = new CRVSOAP.CrvVruchtbaarheid(changedby, sourceid);
            int MaxString = 255;
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBN.UBNid).LandAfk3;
            String Uitvoerende = string.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            FTPUSER fusoap = DB.GetFtpuser(pUBN.UBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {

                FTPUSER fusoapold = DB.GetFtpuser(pUBN.UBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }
            if (lUsername == String.Empty || lPassword == String.Empty)
            {
                Result.Kind = 431;
                Result.Status = "F";
                Result.Code = "LOGINCRV";
                Result.Omschrijving = "CRV Gebruikersnaam en/of Wachtwoord niet ingevoerd.";
                unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Gebruikersnaam en/of Wachtwoord niet ingevoerd!", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));

                return Result;
            }

            lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pAnimal.AniAlternateNumber;
            Result.FarmNumber = pUBN.Bedrijfsnummer;
            String LogFile = unLogger.getLogDir("IenR") + "CRDTochtSOAP" + pUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";
            //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDTochtSOAP" + pUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log");


            //DLLcall.CRDTochtMelding(lUsername, lPassword, lTestServer,
            //    lLand, pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate, pEvent.EveDate,
            //    Uitvoerende, LogFile,
            //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);


            SOAPLOG Rest = crvvruchtbaarheid.SendInHeat(pUBN, pAnimal, fusoap, lLand, Uitvoerende, pEvent.EveDate);
            lStatus = Rest.Status.ToUpper();
            lCode = Rest.Code;
            lOmschrijving = Rest.Omschrijving;

            Result.Kind = 402;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {

                pEvent.Changed_By = changedby;
                pEvent.SourceID = sourceid;
                switch (pEvent.EveKind)
                {
                    case 1:
                        INHEAT inheat = DB.GetInHeat(pEvent.EventId);
                        inheat.ReportDate = Result.Date;
                        inheat.Changed_By = changedby;
                        inheat.SourceID = sourceid;
                        DB.SaveInHeat(inheat);
                        break;
                }
                DB.SaveEventReportDate(pEvent, Result, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_IenR_WS);

            }
            return Result;

        }
        //public override SOAPLOG MeldTochtIndicatie(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken)
        //{
        //    return MeldTochtIndicatie(pUBN, pAnimal, pEvent, mToken, null);
        //}
        /// <summary>
        /// SOAPREPROALG.DLL
        /// </summary>
        /// <param name="pUBN"></param>
        /// <param name="pAnimal"></param>
        /// <param name="pEvent"></param>
        /// <param name="mToken"></param>
        /// <param name="pCRVSOAPCredidentials"></param>
        /// <param name="pTestServer"></param>
        /// <param name="changedby"></param>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public override SOAPLOG MeldTochtIndicatie(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken, FTPUSER pCRVSOAPCredidentials, Boolean pTestServer, int changedby, int sourceid)
        {
            Win32SOAPREPROALG DLLcall = new Win32SOAPREPROALG();
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            int MaxString = 255;
            String lUsername, lPassword;
            int Soort;
            int Zekerheid;
            String lLand = GetCountry(mToken, pUBN.UBNid).LandAfk3;
            DateTime StartTijd = pEvent.EveDate;
            DateTime EindTijd = pEvent.EveDate;
            switch (pEvent.EveKind)
            {
                case 1:
                    Soort = 1;
                    INHEAT inheat = DB.GetInHeat(pEvent.EventId);
                    Zekerheid = inheat.HeatCertainty;


                    if (inheat.HeatTime != DateTime.MinValue)
                    {
                        StartTijd = pEvent.EveDate.Date.AddHours(inheat.HeatTime.Hour).AddMinutes(inheat.HeatTime.Minute).AddSeconds(inheat.HeatTime.Second);
                    }

                    if (inheat.HeatEndTime != DateTime.MinValue)
                        EindTijd = inheat.HeatEndTime;
                    else
                        EindTijd = StartTijd.AddHours(36);

                    break;

                default:
                    Soort = 0;
                    Zekerheid = 0;
                    break;
            }



            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            Result.Changed_By = changedby;
            Result.SourceID = sourceid;
            String Klantnr = pUBN.Bedrijfsnummer;

            if (pCRVSOAPCredidentials == null)
            {
                Klantnr = pUBN.Bedrijfsnummer;
                FTPUSER fusoap = DB.GetFtpuser(pUBN.UBNid, 9991);
                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
                if (lUsername == String.Empty && lPassword == String.Empty)
                {

                    FTPUSER fusoapold = DB.GetFtpuser(pUBN.UBNid, 12);
                    lUsername = fusoapold.UserName;
                    lPassword = fusoapold.Password;
                }
            }
            else
            {
                UBN lCRVUBN = DB.GetubnById(Convert.ToInt32(pCRVSOAPCredidentials.UbnId));
                Klantnr = lCRVUBN.Bedrijfsnummer;
                lUsername = pCRVSOAPCredidentials.UserName;
                lPassword = pCRVSOAPCredidentials.Password;
            }
            if (lUsername == String.Empty || lPassword == String.Empty)
            {
                Result.Kind = 431;
                Result.Status = "F";
                Result.Code = "LOGINCRV";
                Result.Omschrijving = "CRV Gebruikersnaam en/of Wachtwoord niet ingevoerd.";
                unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Gebruikersnaam en/of Wachtwoord niet ingevoerd!", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));

                return Result;
            }
            //Klantnr = "31263750";
            //lUsername = "CRD31263750";
            //lPassword = "geaindicaties";


            //lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pAnimal.AniAlternateNumber;
            Result.FarmNumber = pUBN.Bedrijfsnummer;
            String LogFile = unLogger.getLogDir("IenR") + "CRDTochtindSOAP" + pUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDTochtindSOAP" + pUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log");


            DLLcall.AB_CRDaddAnimalReproductionIndication(lUsername, lPassword, pUBN.Bedrijfsnummer, Klantnr, lLand, pTestServer,
            pAnimal.AniAlternateNumber,
            StartTijd, EindTijd,
            Soort, Zekerheid,
            LogFile,
            ref lStatus, ref lCode, ref lOmschrijving, MaxString);

            Result.Kind = 401;
            Result.SubKind = 1;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {
                pEvent.Changed_By = changedby;
                pEvent.SourceID = sourceid;
                switch (pEvent.EveKind)
                {
                    case 1:
                        Soort = 1;
                        INHEAT inheat = DB.GetInHeat(pEvent.EventId);
                        inheat.ReportDate = Result.Date;
                        inheat.Changed_By = changedby;
                        inheat.SourceID = sourceid;
                        DB.SaveInHeat(inheat);
                        break;
                }
                DB.SaveEventReportDate(pEvent, Result, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_FTP_Ophalen_logfile);
            }
            return Result;

        }

        public override SOAPLOG MeldDrachtControle(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken, int changedby, int sourceid)
        {
            //Win32SOAPVBHALG DLLcall = new Win32SOAPVBHALG();
            VSM.RUMA.CRVSOAP.CrvVruchtbaarheid crvvruchtbaarheid = new CRVSOAP.CrvVruchtbaarheid(changedby, sourceid);
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            int MaxString = 255;
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBN.UBNid).LandAfk3;
            String Uitvoerende = string.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            FTPUSER fusoap = DB.GetFtpuser(pUBN.UBNid, 9991);
            int StatusDracht;
            int MethodeControle;
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {

                FTPUSER fusoapold = DB.GetFtpuser(pUBN.UBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;



            }
            if (lUsername == String.Empty || lPassword == String.Empty)
            {
                Result.Kind = 431;
                Result.Status = "F";
                Result.Code = "LOGINCRV";
                Result.Omschrijving = "CRV Gebruikersnaam en/of Wachtwoord niet ingevoerd.";
                unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Gebruikersnaam en/of Wachtwoord niet ingevoerd!", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));

                return Result;
            }

            lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pAnimal.AniAlternateNumber;
            Result.FarmNumber = pUBN.Bedrijfsnummer;
            String LogFile = unLogger.getLogDir("IenR") + "CRDDrachtSOAP" + pUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";
            //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDDrachtSOAP" + pUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log");


            if (pEvent.EveKind == 3)
            {
                GESTATIO evedracht = DB.GetGestatio(pEvent.EventId);
                if (evedracht.EventId > 0)
                {

                    switch (evedracht.GesStatus)
                    {
                        case 1:
                        case 2:
                            unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Drachtig", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));
                            StatusDracht = 1;
                            break;
                        // niet  drachtig
                        case 3:
                            unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Niet Drachtig", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));
                            StatusDracht = 2;
                            break;
                        case 4:
                            unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Bewust Gust", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));
                            //DLLcall.CRDBewustGustMelding(lUsername, lPassword, lTestServer,
                            //      lLand, pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate, pEvent.EveDate,
                            //      Uitvoerende, LogFile,
                            //      ref lStatus, ref lCode, ref lOmschrijving, MaxString);
                            SOAPLOG resta = crvvruchtbaarheid.SendBewustGust(pUBN, pAnimal, fusoap, lLand, Uitvoerende, pEvent.EveDate);
                            lStatus = resta.Status;
                            lCode = resta.Code;
                            lOmschrijving = resta.Omschrijving;
                            Result.Kind = 434;
                            Result.Status = lStatus;
                            Result.Code = lCode;
                            Result.Omschrijving = lOmschrijving;
                            if (lStatus.Equals("I") || lStatus.Equals("G") || lStatus.Equals("W"))
                            {
                                DB.SaveEventReportDate(pEvent, Result, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_IenR_WS);
                            }
                            return Result;
                        case 7:
                            StatusDracht = 0;
                            break;
                        default:
                            Result.Kind = 439;
                            Result.Status = "F";
                            Result.Code = "AB0009";
                            Result.Omschrijving = "Drachtstatus onbekend";
                            unLogger.WriteInfo(String.Format("UBN: {0} Dier: {1} Datum : {2} Drachtstatus onbekend!", pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate));

                            return Result;


                    }
                    MethodeControle = 0;
                    //DLLcall.CRDDrachtControleMelding(lUsername, lPassword, lTestServer,
                    //    lLand, pUBN.Bedrijfsnummer, pAnimal.AniAlternateNumber, pEvent.EveDate, pEvent.EveDate,
                    //    StatusDracht, MethodeControle,
                    //    Uitvoerende, LogFile,
                    //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);
                    SOAPLOG rest = crvvruchtbaarheid.SendPregCheck(pUBN, pAnimal, fusoap, lLand, Uitvoerende, pEvent.EveDate, StatusDracht, MethodeControle);
                    lStatus = rest.Status;
                    lCode = rest.Code;
                    lOmschrijving = rest.Omschrijving;
                }
            }
            Result.Kind = 431;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("I") || lStatus.Equals("G") || lStatus.Equals("W"))
            {
                DB.SaveEventReportDate(pEvent, Result, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_IenR_WS);
            }
            return Result;

        }


        SOAPLOG MeldIRCRD(MUTATION pRecord, int pUBNid, DBConnectionToken mToken, bool CRVSendChangedAnimals, bool CRVSendChangedAnimalsName, bool CRVSendEmptyName, bool usesoap, LABELSConst.ChangedBy changedBy = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0)
        {

            if (!usesoap)
            {
                return MeldIRCRD(pRecord, pUBNid, mToken, CRVSendChangedAnimals, CRVSendChangedAnimalsName, CRVSendEmptyName, changedBy, sourceId);
            }
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);

            UBN ubn = DB.GetubnById(pUBNid);

            string prefix = $"{nameof(RUMAIenRMeldingen)}.{nameof(MeldIRCRD)} Bedrijf: {ubn?.Bedrijfsnummer} Dier: '{pRecord.Lifenumber}' Naam: '{pRecord.Name}' CRVSendChangedAnimals: {CRVSendChangedAnimals} CRVSendChangedAnimalsName: {CRVSendChangedAnimalsName} CRVSendEmptyName: {CRVSendEmptyName} - ";

            unLogger.WriteInfo($"{prefix} Gestart.");
        
  
            String lUsername, lPassword;
            Boolean lTestServer;
            COUNTRY land = GetCountry(mToken, pUBNid);
            String lLand = land.LandAfk3;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            String LogFile = unLogger.getLogDir("IenR") + "CRDIRSOAP" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log";

            FTPUSER fusoap = DB.GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {
                FTPUSER fusoapold = DB.GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }
            lTestServer = configHelper.UseCRDTestserver;

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.FarmNumber = pRecord.Farmnumber;
            VulArrays(1, mToken);

            int CodeNaamgeving;
            // 1 = Automatisch
            // 2 = Volgnummer
            // 3 = Naam opgeven
            // 4 = Geen naam
            String Worknumber = String.Empty;
            String DierNaam = String.Empty;

            if (CRVSendChangedAnimals)
                Worknumber = pRecord.Worknumber;

            if (CRVSendChangedAnimalsName)
            {
                DierNaam = pRecord.Name;
                if (pRecord.Name == String.Empty)
                {
                    if (CRVSendEmptyName)
                    {
                        unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == True && Name is empty && CRVSendEmptyName == True - CodeNaamgeving = 4");
                        CodeNaamgeving = 4;
                    }
                    else
                    {
                        unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == True && Name is empty && CRVSendEmptyName == False - CodeNaamgeving = 1");
                        CodeNaamgeving = 1;
                    }
                }
                else
                {
                    unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == True && Name is not empty. - CodeNaamgeving = 3");
                    CodeNaamgeving = 3;
                }
            }
            else
            {
                unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == False. - CodeNaamgeving = 1");
                CodeNaamgeving = 1;
            }

            unLogger.WriteInfo($"{prefix} CodeNaamgeving = {CodeNaamgeving}");
           
            VSM.RUMA.CRVSOAP.CRVDataVerzenden verzenden = new VSM.RUMA.CRVSOAP.CRVDataVerzenden(mCRVIRHaarKleur);
            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            switch (pRecord.CodeMutation)
            {
                case 1:
                    //Aanvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Aanvoer_");
                    using (unLogger.AddStackMessage("CRDIRaanvoermelding"))
                    {
                        String lUBNherkomst = string.Empty;
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        verzenden.CRDIRAanvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber, pRecord.MutationDate,
                            pRecord.VersienrVertrekluik, CRVDataVerzenden.AanvoerType.Aanvoer , LABELSConst.ChangedBy.CRVDataVerzenden, ubn.ThrID,
                            ref lCode, ref lUBNherkomst, ref lMeldingsnr, ref lStatus, ref lOmschrijving);
                        //DLLcall.CRDIRaanvoermelding(lUsername, lPassword, lTestServer,
                        //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                        //         1, pRecord.VersienrVertrekluik,
                        //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, ref lMeldingsnr, LogFile, MaxString);
                        //Result.FarmNumber = lUBNherkomst;
                    }
                    break;
                case 2:
                    //Geboorte
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Geboorte_");
                    using (unLogger.AddStackMessage("CRDIRgeboortemelding"))
                    {
                        int pRasType = 1;
                        int PaspoortUrgentie = 1;
                        int GeboorteOverlevings = 0;
                        bool GeboorteOpfok = pRecord.RegistrationCard == 1;
                        bool RegistratieKaart = pRecord.RegistrationCard == 1;
                        if (lLand == "BEL" && pRecord.MotherBoughtRecent == 0)
                        {
                            pRecord.MotherBoughtRecent = 2; // standaard niet recent aangekocht.
                        }
                        if (pRecord.Weight <= 20 || pRecord.Weight >= 80)
                        {
                            pRecord.Weight = 0;
                        }
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());

                        verzenden.CRDIRGeboorteMelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber, pRecord.Worknumber, pRecord.MutationDate 
                                            , pRecord.Sex, pRecord.LifenumberMother, CodeNaamgeving, DierNaam, pRecord.Haircolor, pRecord.AniHaircolor_Memo, pRecord.Speciality 
                                            , pRecord.RegistrationCard, pRecord.CalvingEase, pRecord.MeatScore, pRecord.Weight, GeboorteOverlevings, pRecord.LossDate, pRasType, PaspoortUrgentie
                                            , pRecord.MotherBoughtRecent, changedBy, sourceId, ref lCode, ref lMeldingsnr, ref lStatus, ref lOmschrijving);

                        //DLLcall.CRDIRgeboortemelding(lUsername, lPassword, lTestServer,
                        //            lLand, pRecord.Farmnumber, pRecord.Lifenumber, DierNaam, Worknumber,
                        //            pRecord.LifenumberMother, pRecord.MutationDate, pRecord.IDRLossDate,
                        //            CRDIRGeslacht(pRecord.Sex), CRDIRHaarKleur(pRecord.Haircolor, lLand, pRecord.AniHaircolor_Memo), pRecord.Speciality, pRecord.CalvingEase,
                        //            pRecord.MeatScore, Convert.ToInt32(pRecord.Weight), GeboorteOverlevings, CodeNaamgeving,
                        //            pRasType, PaspoortUrgentie, pRecord.MotherBoughtRecent,
                        //            GeboorteOpfok, RegistratieKaart,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    }
                    break;
                case 3:
                    //Doodgeb.
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Doodgeb_");
                    if (pRecord.Weight <= 20 || pRecord.Weight >= 80)
                    {
                        pRecord.Weight = 0;
                    }
                    using (unLogger.AddStackMessage("CRDIRdoodgeborenmelding"))
                    {
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        int Meerling = 0;
                        if (pRecord.Nling > 1)
                            Meerling = 1;

                        int DoodgebOverlevings = 0;

                        verzenden.CRDIRDoodgeborenmelding(pRecord, lUsername, lPassword, ubn, land 
                                        , pRecord.LifenumberMother, pRecord.MutationDate, pRecord.Sex, pRecord.CalvingEase, pRecord.MeatScore 
                                        , pRecord.Weight, DoodgebOverlevings, Meerling, pRecord.MotherBoughtRecent, changedBy
                                        , sourceId, ref lCode, ref lMeldingsnr, ref lStatus, ref lOmschrijving);

                        //DLLcall.CRDIRdoodgeborenmelding(lUsername, lPassword, lTestServer,
                        //            lLand, pRecord.Farmnumber, pRecord.LifenumberMother, pRecord.MutationDate,
                        //            CRDIRGeslacht(pRecord.Sex), pRecord.CalvingEase, pRecord.MeatScore,
                        //            Convert.ToInt32(pRecord.Weight), DoodgebOverlevings, Meerling, pRecord.MotherBoughtRecent,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    }
                    break;
                case 4:
                    //Afvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Afvoer_");
                    using (unLogger.AddStackMessage("CRDIRafvoermelding"))
                    {
                        String lUBNbestemming;


                        if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                        {
                            pRecord.CountryCodeDepart = lLand;
                            if (pRecord.Farmnumber == pRecord.FarmNumberTo) lUBNbestemming = String.Empty;
                            else lUBNbestemming = pRecord.FarmNumberTo;
                        }
                        else lUBNbestemming = String.Empty;

                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        verzenden.CRDIRAfvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                        , pRecord.MutationDate, CRVDataVerzenden.AfvoerType.Afvoer, pRecord.CountryCodeDepart, pRecord.FarmNumberTo
                                        , pRecord.CullingReason, pRecord.VersienrVertrekluik, changedBy, sourceId
                                        , ref lCode, ref lUBNbestemming, ref lMeldingsnr, ref lStatus, ref lOmschrijving);

                        //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                        //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                        //         1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                        //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemming, ref lMeldingsnr, LogFile, MaxString);
                       
                    }
                    break;
                case 5:
                    //IKB afvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_IKBAfvoer_");

                    using (unLogger.AddStackMessage("CRDIRafvoermelding"))
                    {
                        String lIKBExport;
                        if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                        {
                            pRecord.CountryCodeDepart = lLand;
                            if (pRecord.Farmnumber == pRecord.FarmNumberTo) lIKBExport = String.Empty;
                            else lIKBExport = pRecord.FarmNumberTo;
                        }
                        else lIKBExport = String.Empty;
                        string lUBNbestemming = "";
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        verzenden.CRDIRAfvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                    , pRecord.MutationDate, CRVDataVerzenden.AfvoerType.Afvoer, pRecord.CountryCodeDepart, pRecord.FarmNumberTo
                                    , pRecord.CullingReason, pRecord.VersienrVertrekluik, changedBy, sourceId
                                    , ref lCode, ref lUBNbestemming, ref lMeldingsnr, ref lStatus, ref lOmschrijving);

                        //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                        //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                        //         1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                        //         ref lStatus, ref lCode, ref lOmschrijving, ref lIKBExport, ref lMeldingsnr, LogFile, MaxString);
                    }
                    break;
                case 6:
                    //Dood
                    pRecord.Report = 2;
                    lStatus = "W";
                    lOmschrijving = "Doodmelding niet verstuurd naar Veemanager, dier wordt afgemeld door de destructor";

                    //Rendac doet het doodmelden
                    //throw new NotImplementedException("SOAPIRALG.dll does not support this type (Dood)");
                    break;
                case 7:
                    //Import
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Import_");
                    using (unLogger.AddStackMessage("CRDIRimportmelding"))
                    {
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());

                        verzenden.CRDIRImportmelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                    , pRecord.MutationDate, pRecord.IDRBirthDate, pRecord.Sex, pRecord.Worknumber, DierNaam, pRecord.LifenrOrigin 
                                    , pRecord.LifenumberMother, pRecord.CountryCodeDepart, pRecord.CountryCodeBirth
                                    , pRecord.Haircolor, pRecord.AniHaircolor_Memo, Convert.ToBoolean(pRecord.Subsidy), changedBy, sourceId
                                    , ref lCode, ref lStatus, ref lOmschrijving);

                        //DLLcall.CRDIRimportmelding(lUsername, lPassword, lTestServer,
                        //            lLand, pRecord.Farmnumber, pRecord.Lifenumber, DierNaam,
                        //            Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            CRDIRGeslacht(pRecord.Sex), CRDIRHaarKleur(pRecord.Haircolor, lLand, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            pRecord.NrGezondheidsCert,
                        //            ref lStatus, ref lCode, ref lOmschrijving, LogFile, MaxString);
                    }
                    break;
                case 8:
                    //Aan-/afvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_AanAfvoer_");
                    using (unLogger.AddStackMessage("CRDIRaanvoermelding"))
                    {
                        String lUBNbestemming;
                        if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                        {
                            pRecord.CountryCodeDepart = lLand;
                            if (pRecord.Farmnumber == pRecord.FarmNumberTo) lUBNbestemming = String.Empty;
                            else lUBNbestemming = pRecord.FarmNumberTo;
                        }
                        else lUBNbestemming = String.Empty;

                        using (unLogger.AddStackMessage("CRDIRafvoermelding"))
                        {
                            String lUBNherkomst = string.Empty;

                            //DLLcall.CRDIRaanvoermelding(lUsername, lPassword, lTestServer,
                            //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                            //         1, pRecord.VersienrVertrekluik, ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, ref lMeldingsnr, LogFile, MaxString);



                            //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                            //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                            //         1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                            //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemming, ref lMeldingsnr, LogFile, MaxString);
                        }
                    }
                    break;
                case 9:
                    //Export
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Export_");
                    String lUBNExport = string.Empty;
                    verzenden.CRDIRAfvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                      , pRecord.MutationDate, CRVDataVerzenden.AfvoerType.Afvoer, pRecord.CountryCodeDepart, pRecord.FarmNumberTo
                                      , pRecord.CullingReason, pRecord.VersienrVertrekluik, changedBy, sourceId
                                      , ref lCode, ref lUBNExport, ref lMeldingsnr, ref lStatus, ref lOmschrijving);
                    //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                    //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                    //         1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                    //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNExport, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 10:
                    //Slacht
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Slacht_");
                    String lUBNSlacht = string.Empty;
              
                    verzenden.CRDIRAfvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                  , pRecord.MutationDate, CRVDataVerzenden.AfvoerType.Afvoer, pRecord.CountryCodeDepart, pRecord.FarmNumberTo
                                  , pRecord.CullingReason, pRecord.VersienrVertrekluik, changedBy, sourceId
                                  , ref lCode, ref lUBNSlacht, ref lMeldingsnr, ref lStatus, ref lOmschrijving);
                    //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                    //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                    //         1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                    //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNSlacht, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 11:
                    //Inscharen
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Inscharen_");
                    String lUBNherkomstInscharen = string.Empty;
                    verzenden.CRDIRAanvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber, pRecord.MutationDate 
                                        , pRecord.VersienrVertrekluik, CRVDataVerzenden.AanvoerType.Aanvoer, LABELSConst.ChangedBy.CRVDataVerzenden, ubn.ThrID 
                                        , ref lCode, ref lUBNherkomstInscharen, ref lMeldingsnr, ref lStatus, ref lOmschrijving);
                    //DLLcall.CRDIRaanvoermelding(lUsername, lPassword, lTestServer,
                    //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                    //         2, pRecord.VersienrVertrekluik, ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomstInscharen, ref lMeldingsnr, LogFile, MaxString);
                    //Result.FarmNumber = lUBNherkomstInscharen;
                    break;
                case 12:
                    //Uitscharen
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Uitscharen_");
                    if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                    {
                        pRecord.CountryCodeDepart = lLand;
                    }
                    String lUBNbestemmingUitscharen = string.Empty;
                    verzenden.CRDIRAfvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                      , pRecord.MutationDate, CRVDataVerzenden.AfvoerType.Afvoer, pRecord.CountryCodeDepart, pRecord.FarmNumberTo
                                      , pRecord.CullingReason, pRecord.VersienrVertrekluik, changedBy, sourceId
                                      , ref lCode, ref lUBNbestemmingUitscharen, ref lMeldingsnr, ref lStatus, ref lOmschrijving);
                    //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                    //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                    //         2, Convert.ToInt32(pRecord.CullingReason), pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                    //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemmingUitscharen, ref lMeldingsnr, LogFile, MaxString);
                    //Result.FarmNumber = lUBNbestemmingUitscharen;
                    break;
                case 13:
                    //Noodslacht
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Noodslacht_");
                    if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                    {
                        pRecord.CountryCodeDepart = lLand;
                    }
                    String lUBNNoodSlacht = string.Empty;
                    verzenden.CRDIRAfvoermelding(pRecord, lUsername, lPassword, ubn, land, pRecord.Lifenumber
                                      , pRecord.MutationDate, CRVDataVerzenden.AfvoerType.Afvoer, pRecord.CountryCodeDepart, pRecord.FarmNumberTo
                                      , pRecord.CullingReason, pRecord.VersienrVertrekluik, changedBy, sourceId
                                      , ref lCode, ref lUBNNoodSlacht, ref lMeldingsnr, ref lStatus, ref lOmschrijving);

                    //DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                    //         lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                    //         1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                    //         ref lStatus, ref lCode, ref lOmschrijving, ref lUBNNoodSlacht, ref lMeldingsnr, LogFile, MaxString);
                    break;
            }
            unLogger.WriteInfo(" LogFile:" + LogFile);
            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving + lMeldingsnr;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            if (lStatus.Equals("G"))
                pRecord.Returnresult = 1;
            else if (lStatus.Equals("F"))
                pRecord.Returnresult = 2;
            else if (lStatus.Equals("W"))
                pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;
            pRecord.MeldResult = Result.Omschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode.Trim(), pRecord))
            {
                lock (Facade.GetInstance())
                {
                    pRecord.ReportDate = Result.Date;
                    pRecord.ReportTime = Result.Time;

                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;

                                mov.Changed_By = (int)changedBy;
                                mov.SourceID = sourceId;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;

                                bir.Changed_By = (int)changedBy;
                                bir.SourceID = sourceId;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteError("IRlog", $"{prefix} Fout bij opslaan Reportdate Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);
                    }

                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    MutLog.MeldingNummer = lMeldingsnr;
                    MutLog.Changed_By = (int)changedBy;
                    MutLog.SourceID = sourceId;

                    if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                        unLogger.WriteError("IRlog", $"{prefix} Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                    if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                    {

                        pRecord.Report = 99;
                        UpdateReport(pRecord, mToken);
                        unLogger.WriteError("IRlog", $"{prefix} Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                        MUTATION delmut;
                        for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                        {
                            if (DeleteMutation(pRecord, mToken))
                                break;
                            else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                            {
                                if (pRecord.EventId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                else if (pRecord.MovId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                else
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (delmut.Internalnr == 0)
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                    break;
                                else
                                    unLogger.WriteError("IRlog", $"{prefix} Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                            else
                            {
                                unLogger.WriteError("IRlog", $"{prefix} Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                        }
                    }

                }
            }
            else if (lStatus.Equals("F"))
            {
                pRecord.ReportDate = Result.Date;
                pRecord.ReportTime = Result.Time;
                DB.UpdateMutationReport(pRecord);
            }
           
            unLogger.WriteInfo($"{prefix} Klaar.");
            return Result;



        }

        /// <summary>
        /// SOAPIRALG.DLL
        /// use VSM.RUMA.CRVSOAP CRVDataVerzenden.cs
        /// </summary>
        /// <param name="pRecord"></param>
        /// <param name="pUBNid"></param>
        /// <param name="mToken"></param>
        /// <param name="CRVSendChangedAnimals"></param>
        /// <param name="CRVSendChangedAnimalsName"></param>
        /// <param name="CRVSendEmptyName"></param>
        /// <param name="changedBy"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public SOAPLOG MeldIRCRD(MUTATION pRecord, int pUBNid, DBConnectionToken mToken, bool CRVSendChangedAnimals, bool CRVSendChangedAnimalsName, bool CRVSendEmptyName, LABELSConst.ChangedBy changedBy = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);

            var ubn = DB.GetubnById(pUBNid);
            string prefix = $"{nameof(RUMAIenRMeldingen)}.{nameof(MeldIRCRD)} Bedrijf: {ubn?.Bedrijfsnummer} Dier: '{pRecord.Lifenumber}' Naam: '{pRecord.Name}' CRVSendChangedAnimals: {CRVSendChangedAnimals} CRVSendChangedAnimalsName: {CRVSendChangedAnimalsName} CRVSendEmptyName: {CRVSendEmptyName} - ";

            unLogger.WriteDebug($"{prefix} Gestart.");

            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBNid).LandAfk3;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            String LogFile = unLogger.getLogDir("IenR") + "CRDIRSOAP" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log";

            FTPUSER fusoap = DB.GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {
                FTPUSER fusoapold = DB.GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }
            lTestServer = configHelper.UseCRDTestserver;
            if (lTestServer == true)
            {
              
                pRecord.Farmnumber = "316323";

            }
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.FarmNumber = pRecord.Farmnumber;


            int CodeNaamgeving;
            // 1 = Automatisch
            // 2 = Volgnummer
            // 3 = Naam opgeven
            // 4 = Geen naam
            String Worknumber = String.Empty;
            String DierNaam = String.Empty;

            if (CRVSendChangedAnimals)
                Worknumber = pRecord.Worknumber;

            if (CRVSendChangedAnimalsName)
            {
                DierNaam = pRecord.Name;
                if (pRecord.Name == String.Empty)
                {
                    if (CRVSendEmptyName)
                    {
                        unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == True && Name is empty && CRVSendEmptyName == True - CodeNaamgeving = 4");
                        CodeNaamgeving = 4;
                    }
                    else
                    {
                        unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == True && Name is empty && CRVSendEmptyName == False - CodeNaamgeving = 1");
                        CodeNaamgeving = 1;
                    }
                }
                else
                {
                    unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == True && Name is not empty. - CodeNaamgeving = 3");
                    CodeNaamgeving = 3;
                }
            }
            else
            {
                unLogger.WriteDebug($"{prefix} CRVSendChangedAnimalsName == False. - CodeNaamgeving = 1");
                CodeNaamgeving = 1;
            }

            unLogger.WriteDebug($"{prefix} CodeNaamgeving = {CodeNaamgeving}");

            switch (pRecord.CodeMutation)
            {
                case 1:
                    //Aanvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Aanvoer_");
                    using (unLogger.AddStackMessage("CRDIRaanvoermelding"))
                    {
                        String lUBNherkomst = string.Empty;
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());

                        DLLcall.CRDIRaanvoermelding(lUsername, lPassword, lTestServer,
                                 lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                                 1, pRecord.VersienrVertrekluik,
                                 ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, ref lMeldingsnr, LogFile, MaxString);
                        //Result.FarmNumber = lUBNherkomst;
                    }
                    break;
                case 2:
                    //Geboorte
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Geboorte_");
                    using (unLogger.AddStackMessage("CRDIRgeboortemelding"))
                    {
                        int pRasType = 1;
                        int PaspoortUrgentie = 1;
                        int GeboorteOverlevings = 0;
                        bool GeboorteOpfok = pRecord.RegistrationCard == 1;
                        bool RegistratieKaart = pRecord.RegistrationCard == 1;
                        if (lLand == "BEL" && pRecord.MotherBoughtRecent == 0)
                        {
                            pRecord.MotherBoughtRecent = 2; // standaard niet recent aangekocht.
                        }
                        if (pRecord.Weight <= 20 || pRecord.Weight >= 80)
                        {
                            pRecord.Weight = 0;
                        }
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());

                        DLLcall.CRDIRgeboortemelding(lUsername, lPassword, lTestServer,
                                    lLand, pRecord.Farmnumber, pRecord.Lifenumber, DierNaam, Worknumber,
                                    pRecord.LifenumberMother, pRecord.MutationDate, pRecord.IDRLossDate,
                                    CRDIRGeslacht(pRecord.Sex), CRDIRHaarKleur(pRecord.Haircolor, lLand, pRecord.AniHaircolor_Memo), pRecord.Speciality, pRecord.CalvingEase,
                                    pRecord.MeatScore, Convert.ToInt32(pRecord.Weight), GeboorteOverlevings, CodeNaamgeving,
                                    pRasType, PaspoortUrgentie, pRecord.MotherBoughtRecent,
                                    GeboorteOpfok, RegistratieKaart,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    }
                    break;
                case 3:
                    //Doodgeb.
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Doodgeb_");
                    if (pRecord.Weight <= 20 || pRecord.Weight >= 80)
                    {
                        pRecord.Weight = 0;
                    }
                    using (unLogger.AddStackMessage("CRDIRdoodgeborenmelding"))
                    {
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        int Meerling = 0;
                        if (pRecord.Nling > 1)
                            Meerling = 1;

                        int DoodgebOverlevings = 0;
                        DLLcall.CRDIRdoodgeborenmelding(lUsername, lPassword, lTestServer,
                                    lLand, pRecord.Farmnumber, pRecord.LifenumberMother, pRecord.MutationDate,
                                    CRDIRGeslacht(pRecord.Sex), pRecord.CalvingEase, pRecord.MeatScore,
                                    Convert.ToInt32(pRecord.Weight), DoodgebOverlevings, Meerling, pRecord.MotherBoughtRecent,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    }
                    break;
                case 4:
                    //Afvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Afvoer_");
                    using (unLogger.AddStackMessage("CRDIRafvoermelding"))
                    {
                        String lUBNbestemming;


                        if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                        {
                            pRecord.CountryCodeDepart = lLand;
                            if (pRecord.Farmnumber == pRecord.FarmNumberTo) lUBNbestemming = String.Empty;
                            else lUBNbestemming = pRecord.FarmNumberTo;
                        }
                        else lUBNbestemming = String.Empty;

                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());

                        DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                                 lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                                 1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                                 ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemming, ref lMeldingsnr, LogFile, MaxString);
                        //Result.FarmNumber = lUBNbestemming;
                    }
                    break;
                case 5:
                    //IKB afvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_IKBAfvoer_");

                    using (unLogger.AddStackMessage("CRDIRafvoermelding"))
                    {
                        String lIKBExport;
                        if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                        {
                            pRecord.CountryCodeDepart = lLand;
                            if (pRecord.Farmnumber == pRecord.FarmNumberTo) lIKBExport = String.Empty;
                            else lIKBExport = pRecord.FarmNumberTo;
                        }
                        else lIKBExport = String.Empty;

                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                                 lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                                 1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                                 ref lStatus, ref lCode, ref lOmschrijving, ref lIKBExport, ref lMeldingsnr, LogFile, MaxString);
                    }
                    break;
                case 6:
                    //Dood
                    pRecord.Report = 2;
                    lStatus = "W";
                    lOmschrijving = "Doodmelding niet verstuurd naar Veemanager, dier wordt afgemeld door de destructor";

                    //Rendac doet het doodmelden
                    //throw new NotImplementedException("SOAPIRALG.dll does not support this type (Dood)");
                    break;
                case 7:
                    //Import
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Import_");
                    using (unLogger.AddStackMessage("CRDIRimportmelding"))
                    {
                        unLogger.WriteDebug("IRlog", " Username = " + lUsername);
                        unLogger.WriteDebug("IRlog", " Farmnumber = " + pRecord.Farmnumber);
                        unLogger.WriteDebug("IRlog", " Lifenumber = " + pRecord.Lifenumber);
                        unLogger.WriteDebug("IRlog", " MutationDate = " + pRecord.MutationDate.ToString());
                        DLLcall.CRDIRimportmelding(lUsername, lPassword, lTestServer,
                                    lLand, pRecord.Farmnumber, pRecord.Lifenumber, DierNaam,
                                    Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                                    pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                                    pRecord.IDRBirthDate, pRecord.MutationDate,
                                    CRDIRGeslacht(pRecord.Sex), CRDIRHaarKleur(pRecord.Haircolor, lLand, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                                    pRecord.NrGezondheidsCert,
                                    ref lStatus, ref lCode, ref lOmschrijving, LogFile, MaxString);
                    }
                    break;
                case 8:
                    //Aan-/afvoer
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_AanAfvoer_");
                    using (unLogger.AddStackMessage("CRDIRaanvoermelding"))
                    {
                        String lUBNbestemming;
                        if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                        {
                            pRecord.CountryCodeDepart = lLand;
                            if (pRecord.Farmnumber == pRecord.FarmNumberTo) lUBNbestemming = String.Empty;
                            else lUBNbestemming = pRecord.FarmNumberTo;
                        }
                        else lUBNbestemming = String.Empty;

                        using (unLogger.AddStackMessage("CRDIRafvoermelding"))
                        {
                            String lUBNherkomst = string.Empty;

                            DLLcall.CRDIRaanvoermelding(lUsername, lPassword, lTestServer,
                                     lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                                     1, pRecord.VersienrVertrekluik, ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, ref lMeldingsnr, LogFile, MaxString);
                            DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                                     lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                                     1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                                     ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemming, ref lMeldingsnr, LogFile, MaxString);
                        }
                    }
                    break;
                case 9:
                    //Export
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Export_");
                    String lUBNExport = string.Empty;
                    DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                             lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                             1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                             ref lStatus, ref lCode, ref lOmschrijving, ref lUBNExport, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 10:
                    //Slacht
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Slacht_");
                    String lUBNSlacht = string.Empty;
                    DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                             lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                             1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                             ref lStatus, ref lCode, ref lOmschrijving, ref lUBNSlacht, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 11:
                    //Inscharen
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Inscharen_");
                    String lUBNherkomstInscharen = string.Empty;
                    DLLcall.CRDIRaanvoermelding(lUsername, lPassword, lTestServer,
                             lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                             2, pRecord.VersienrVertrekluik, ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomstInscharen, ref lMeldingsnr, LogFile, MaxString);
                    //Result.FarmNumber = lUBNherkomstInscharen;
                    break;
                case 12:
                    //Uitscharen
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Uitscharen_");
                    if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                    {
                        pRecord.CountryCodeDepart = lLand;
                    }
                    String lUBNbestemmingUitscharen = string.Empty;
                    DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                             lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                             2, Convert.ToInt32(pRecord.CullingReason), pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                             ref lStatus, ref lCode, ref lOmschrijving, ref lUBNbestemmingUitscharen, ref lMeldingsnr, LogFile, MaxString);
                    //Result.FarmNumber = lUBNbestemmingUitscharen;
                    break;
                case 13:
                    //Noodslacht
                    LogFile = LogFile.Replace("CRDIRSOAP", "CRDIRSOAP_Noodslacht_");
                    if (lLand == "BEL" && pRecord.CountryCodeDepart == String.Empty)
                    {
                        pRecord.CountryCodeDepart = lLand;
                    }
                    String lUBNNoodSlacht = string.Empty;
                    DLLcall.CRDIRafvoermelding(lUsername, lPassword, lTestServer,
                             lLand, pRecord.Farmnumber, pRecord.Lifenumber, pRecord.MutationDate,
                             1, pRecord.CullingReason, pRecord.VersienrVertrekluik, pRecord.CountryCodeDepart, pRecord.FarmNumberTo,
                             ref lStatus, ref lCode, ref lOmschrijving, ref lUBNNoodSlacht, ref lMeldingsnr, LogFile, MaxString);
                    break;
            }
            unLogger.WriteInfo($" LogFile: {LogFile}");
            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving + lMeldingsnr;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            if (lStatus.Equals("G")) 
                pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) 
                pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) 
                pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;
                pRecord.MeldResult = Result.Omschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode.Trim(), pRecord))
            {
                lock (Facade.GetInstance())
                {
                    pRecord.ReportDate = Result.Date;
                    pRecord.ReportTime = Result.Time;

                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;

                                mov.Changed_By = (int)changedBy;
                                mov.SourceID = sourceId;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;

                                bir.Changed_By = (int)changedBy;
                                bir.SourceID = sourceId;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteError("IRlog", $"{prefix} Fout bij opslaan Reportdate Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);
                    }

                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    MutLog.MeldingNummer = lMeldingsnr;
                    MutLog.Changed_By = (int)changedBy;
                    MutLog.SourceID = sourceId;

                    if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                        unLogger.WriteError("IRlog", $"{prefix} Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                    if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                    {

                        pRecord.Report = 99;
                        UpdateReport(pRecord, mToken);
                        unLogger.WriteError("IRlog", $"{prefix} Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                        MUTATION delmut;
                        for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                        {
                            if (DeleteMutation(pRecord, mToken))
                                break;
                            else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                            {
                                if (pRecord.EventId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                else if (pRecord.MovId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                else
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (delmut.Internalnr == 0)
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                    break;
                                else
                                    unLogger.WriteError("IRlog", $"{prefix} Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                            else
                            {
                                unLogger.WriteError("IRlog", $"{prefix} Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                        }
                    }

                }
            }
            else if (lStatus.Equals("F"))
            {
                pRecord.ReportDate = Result.Date;
                pRecord.ReportTime = Result.Time;
                DB.UpdateMutationReport(pRecord);
            }
            DLLcall.Dispose();

            unLogger.WriteDebug($"{prefix} Klaar.");
            return Result;
        }

        public bool IRNietHerhalen(string FoutCode, MUTATION pRecord)
        {
            switch (FoutCode)
            {
                case "IRD-00015":
                case "MFIRD-00015":
                    pRecord.Report = 2;
                    return true;
                case "IRD-00184":
                case "MFIRD-00184":
                    pRecord.Report = 2;
                    return true;
                case "IRD-00185":
                case "MFIRD-00185":
                    pRecord.Report = 2;
                    return true;
                case "MFIRD-01508":
                case "IRD-01508":
                    pRecord.Report = 2;
                    return true;
                case "IRD-00042":
                case "MFIRD-00042":
                    pRecord.Report = 2;
                    return true;
                case "IRD-01631":
                case "MFIRD-01631":
                    pRecord.Report = 2;
                    return true;
                case "MFSAN-ANEA": // België
                    pRecord.Report = 2;
                    return true;
                default:
                    return false;
            }
        }

        public bool DHZNietHerhalen(string FoutCode, DHZ pRecord)
        {
            switch (FoutCode)
            {
                case "IRD-00015":
                case "MFIRD-00015":
                    pRecord.Report = 2;
                    return true;
                case "IRD-00184":
                case "MFIRD-00184":
                    pRecord.Report = 2;
                    return true;
                case "IRD-00185":
                case "MFIRD-00185":
                    pRecord.Report = 2;
                    return true;
                case "MFIRD-01508":
                case "IRD-01508":
                    pRecord.Report = 2;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// SOAPIRALG.DLL
        /// </summary>
        /// <param name="pRecord"></param>
        /// <param name="pUBNid"></param>
        /// <param name="mToken"></param>
        /// <returns></returns>
        public SOAPLOG MeldIRLNVIR(MUTATION pRecord, int pUBNid, DBConnectionToken mToken)
        {


            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername, lPassword;
            Boolean lTestServer;

            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            unLogger.WriteWarn("I&R versie 1.1 gebruik", new Exception("I&R versie 1.1"));
            THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
            String lBRSnummer = lPersoon.Thr_Brs_Number;
            //if (lBRSnummer == string.Empty)
            //    lBRSnummer = lUBN.BRSnummer;

            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;

            SOAPLOG Result = new SOAPLOG();
            String LogFile = unLogger.getLogDir("IenR") + "LNVIRSOAP_MELDKIND_" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log";

            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9992);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            lTestServer = configHelper.UseCRDTestserver;

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.FarmNumber = pRecord.Farmnumber;


            switch (pRecord.CodeMutation)
            {
                case 1:
                    //Aanvoer
                    LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                    DLLcall.LNVIRaanvoermelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 2:
                    //Geboorte
                    LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                    DLLcall.LNVIRgeboortemelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                                pRecord.MutationDate,
                                LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 3:
                    //Doodgeb.
                    LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                    DLLcall.LNVIRdoodgeborenmelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                                pRecord.MutationDate,
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 4:
                    //Afvoer
                    LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                    DLLcall.LNVIRafvoermelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 5:
                    //IKB afvoer
                    LogFile = LogFile.Replace("MELDKIND", "IKBafvoer");
                    DLLcall.LNVIRafvoermelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 6:
                    //Dood
                    //Rendac doet het doodmelden
                    //throw new NotImplementedException("SOAPIRALG.dll does not support this type (Dood)");
                    break;
                case 7:
                    //Import
                    LogFile = LogFile.Replace("MELDKIND", "Import");
                    DLLcall.LNVIRimportmelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                                pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                                pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                                pRecord.IDRBirthDate, pRecord.MutationDate,
                                LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 8:
                    //Aan-/afvoer
                    LogFile = LogFile.Replace("MELDKIND", "AanvoerAfvoer");
                    DLLcall.LNVIRaanvoermelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    DLLcall.LNVIRafvoermelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 9:
                    //Export
                    LogFile = LogFile.Replace("MELDKIND", "Export");
                    DLLcall.LNVIRexportmelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 10:
                    //Slacht
                    DLLcall.LNVIRafvoermelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 11:
                    //Inscharen
                    LogFile = LogFile.Replace("MELDKIND", "Inscharen");
                    DLLcall.LNVIRaanvoermelding(lUsername, lPassword, lTestServer,
                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 12:
                    //Uitscharen
                    LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                    DLLcall.LNVIRafvoermelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
                case 13:
                    //Noodslacht
                    LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                    DLLcall.LNVIRafvoermelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                    break;
            }
            unLogger.WriteInfo(" LogFile:" + LogFile);
            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving + lMeldingsnr;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;

            Result.Omschrijving = lOmschrijving;
            pRecord.MeldResult = Result.Omschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode.Trim(), pRecord))
            {
                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    MutLog.MeldingNummer = lMeldingsnr;

                    Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken);
                    Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken);
                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }
                }
            }
            else
            {

                Facade.GetInstance().getSaveToDB(mToken).UpdateMutationReport(pRecord);
            }
            DLLcall.Dispose();
            return Result;
        }


        public override SOAPLOG MeldIRLNVV2IR(MUTATION pRecord, int pUBNid, int pProgId, int pProgramId, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials,bool usesoap=false)
        {
            if (!usesoap)
            {
                return MeldIRLNVV2IR(pRecord, pUBNid, pProgId, pProgramId, mToken, pLNV2IRCredidentials);
            }
         
            String lUsername, lPassword;
            int lTestServer;
            SOAPLOG Result = new SOAPLOG();
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;
            int MaxString = 255;
            try
            {
                UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
                THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
                String lBRSnummer = lPersoon.Thr_Brs_Number;
               
                int HerstelMelding = 0;
                if (pRecord.MeldingNummer != String.Empty && pRecord.Returnresult == 96)
                {
                    HerstelMelding = 1;
                }
              



                FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, pProgramId, pProgId, 9992);
                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
                if (pLNV2IRCredidentials != null && pLNV2IRCredidentials.Password != "" && pLNV2IRCredidentials.UserName != "")
                {
                    lUsername = pLNV2IRCredidentials.UserName;
                    lPassword = pLNV2IRCredidentials.Password;
                }
              
                lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                if (lTestServer > 0)
                {
                  
                    if (lTestServer == 1)
                    {
                 
                        lBRSnummer = "10005948";
                        pRecord.Farmnumber = "2510";
                    }
                    else if (lTestServer == 2)
                    {
                     
                        lBRSnummer = "10005948";
                        pRecord.Farmnumber = "152460";
                    }

                }
                String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MELDKIND_" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log";
                //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log");
                Result.Date = DateTime.Today;
                Result.Time = DateTime.Now;
                Result.Lifenumber = pRecord.Lifenumber;
                Result.FarmNumber = pRecord.Farmnumber;
                MeldingenWS ws = new MeldingenWS();
                Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
                SOAPLOG ret = new SOAPLOG();
                List<MUTATION> m = new List<MUTATION>();
                m.Add(pRecord);
                switch (pRecord.CodeMutation)
                {
                    case 1:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                
                        var retmeldingen = ws.LNVIRaanvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.GEBOORTE:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                        retmeldingen = ws.LNVIRgeboortemeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.DOODGEBOREN:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                        retmeldingen = ws.LNVIRdoodgeborenmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.AFVOER:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.IKBAFVOER:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "IKBAfvoer");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.DOOD:
                        //Dood
                        //Rendac doet het doodmelden

                       
                        //rundvee 1
                        //stieren 2
                        //schapen 3
                        //zoogkoeien 4
                        //geiten 5
                        //witvlees 6
                        //rose 7
                        LogFile = LogFile.Replace("MELDKIND", "Dood");
                        if (pProgId == 3 || pProgId == 5)
                        {
                            retmeldingen = ws.LNVIRdoodmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                            lStatus = retmeldingen.ElementAt(0).Status;
                            lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                            lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        }
                        else
                        {
                            lStatus = "F";
                            lOmschrijving = $"Rendac verzorgt de doodmelding van {pRecord.Lifenumber}.";
                        }
                        break;
                    case (int)LABELSConst.CodeMutation.IMPORT:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "Import");
                        unLogger.WriteInfo("Importmelding: " + pRecord.Lifenumber);
                        retmeldingen = ws.LNVIRimportmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                        //            pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            pRecord.NrGezondheidsCert, HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.AANAFVOER :
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "AanAfvoer");
                        lStatus = "F";
                        lOmschrijving = $@"Not implemented ";

                        /*
                                SELECT * FROM agrobase_calf.MUTALOG m
                                WHERE m.codemutation=8
                                order by m.MutationDate desc
                                alleen in calf 6x uit 2010
                         */

                        break;
                    case (int)LABELSConst.CodeMutation.EXPORT:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "Export");
                        retmeldingen = ws.LNVIRexportmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;

                        break;
                    case (int)LABELSConst.CodeMutation.SLACHT:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "Slacht");
                        retmeldingen = ws.LNVIRslachtmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;

                        break;
                    case (int)LABELSConst.CodeMutation.INSCHAREN:
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                  
                        retmeldingen = ws.LNVIRaanvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;

                        break;
                    case (int)LABELSConst.CodeMutation.UITSCHAREN:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;

                        break;
                    case (int)LABELSConst.CodeMutation.NOODSLACHT:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                        ret = ws.LNVIRnoodslachtV2(pRecord, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = ret.Status;
                        lOmschrijving = ret.Omschrijving;
                     
                        break;
                    case  16:
                        //Q-Krts vacc1
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc1");
                        lStatus = "F";
                        lOmschrijving = "Melding is niet meer nodig, alleen vacc2 en Herhaling is verplicht.";
                       
                        break;
                    case (int)LABELSConst.CodeMutation.QKrtsvacc2 :
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc2");
                        retmeldingen = ws.LNVIRQkoortsV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;

                        break;
                    case (int)LABELSConst.CodeMutation.QKrtsvaccH :
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvaccH");
                        retmeldingen = ws.LNVIRQkoortsV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, HerstelMelding);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;

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
                        MUTALOG lIntrekTemp = ConverttoMutLog(pRecord);
                        Result = LNV2MeldingIntrekken(lIntrekTemp, lUBN.UBNid, pProgId, pProgramId, mToken, usesoap);

                        lStatus = Result.Status;
                        lCode = Result.Code;
                        lOmschrijving = Result.Omschrijving;

                        break;

                    //herstellen ook
                    case 201:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer");
                  
                        retmeldingen = ws.LNVIRaanvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 202:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "HerstelGeboorte");
                        retmeldingen = ws.LNVIRgeboortemeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 203:
                        //Doodgeb. er is geen herstelmelding indicator
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDoodgeb");
                        //retmelding = ws.LNVIRdoodgeborenmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        //lStatus = retmelding.ElementAt(0).Status;
                        //lOmschrijving = retmelding.ElementAt(0).Omschrijving;
                        //lMeldingsnr = retmelding.ElementAt(0).meldnummer;
                        lStatus = "F";
                        lOmschrijving = "Not implemented ";
                        //DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                        //            pProgId, pRecord.MutationDate,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 204:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAfvoer");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 205:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelIKBAfvoer");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 206:
                        //Dood
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDood");
                        retmeldingen = ws.LNVIRdoodmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 207:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "HerstelImport");
                        retmeldingen = ws.LNVIRimportmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                        //            pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                        //            pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                        //            pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                        //            LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                        //            pRecord.NrGezondheidsCert, HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 208:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanAfvoer");
                        lStatus = "F";
                        lOmschrijving = "Not implemented.";
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 209:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "HerstelExport");
                        retmeldingen = ws.LNVIRexportmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                        //            pProgId, pRecord.MutationDate,
                        //            HerstelMelding, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 210:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelSlacht");
                        retmeldingen = ws.LNVIRslachtmeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 211:
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer11");
                       
                        retmeldingen = ws.LNVIRaanvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 212:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "HerstelUitscharen");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 213:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelNoodslacht");
                        retmeldingen = ws.LNVIRafvoermeldingV2(m, lUsername, lPassword, lTestServer, pRecord.Farmnumber, lBRSnummer, pProgId, 1);
                        lStatus = retmeldingen.ElementAt(0).Status;
                        lOmschrijving = retmeldingen.ElementAt(0).Omschrijving;
                        lMeldingsnr = retmeldingen.ElementAt(0).meldnummer;
                        //DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                        //            pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                        //            1, pRecord.MeldingNummer,
                        //            ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
            }
            catch (Exception Exc)
            {
                lStatus = "F";
                Result.Date = DateTime.Now;
                Result.Omschrijving = Exc.Message;
                unLogger.WriteDebug(Exc.ToString());
            }

            Result.Kind = pRecord.CodeMutation;
            if (Result.Status == "")
            {
                Result.Status = lStatus;
                Result.Code = lCode;
                Result.Omschrijving = lOmschrijving + lMeldingsnr;
            }
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            pRecord.MeldResult = Result.Omschrijving;
            //let op lStatus kan leeg zijn na doen van intrekmelding CodeMutation 101 t/m 113 
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;

            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode, pRecord))
            {

                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    if (lMeldingsnr != "")
                    {
                        MutLog.MeldingNummer = lMeldingsnr;
                    }
                    if (pRecord.CodeMutation > 100 && pRecord.CodeMutation < 115)
                    {
                        MutLog.MeldingNummerOrg = pRecord.MeldingNummer;
                    }

                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }


                    if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                        unLogger.WriteError("IRlog", "Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                    if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                    {

                        pRecord.Report = 99;
                        UpdateReport(pRecord, mToken);
                        unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                        MUTATION delmut;
                        for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                        {
                            if (DeleteMutation(pRecord, mToken))
                                break;
                            else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                            {
                                if (pRecord.EventId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                else if (pRecord.MovId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                else
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (delmut.Internalnr == 0)
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                    break;
                                else
                                    unLogger.WriteError("IRlog", "Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                            else
                            {
                                unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                        }
                    }
                }
            }
            else 
            {
                pRecord.MeldResult = Result.Omschrijving;
                Facade.GetInstance().getSaveToDB(mToken).UpdateMutation(pRecord);
            }
            //DLLcall.Dispose();
            return Result;
        }
            /// <summary>
            /// SOAPIRALG.DLL
            /// </summary>
            /// <param name="pRecord"></param>
            /// <param name="pUBNid"></param>
            /// <param name="pProgId"></param>
            /// <param name="pProgramId"></param>
            /// <param name="mToken"></param>
            /// <param name="pLNV2IRCredidentials"></param>
            /// <returns></returns>
        public SOAPLOG MeldIRLNVV2IR(MUTATION pRecord, int pUBNid, int pProgId, int pProgramId, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials)
        {
            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername, lPassword;
            int lTestServer;
            SOAPLOG Result = new SOAPLOG();
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;
            try
            {
                UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
                THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
                String lBRSnummer = lPersoon.Thr_Brs_Number;
                //if (lBRSnummer == string.Empty)
                //    lBRSnummer = lUBN.BRSnummer;


                // LEGACY  
                int HerstelMelding = 0;
                if (pRecord.MeldingNummer != String.Empty && pRecord.Returnresult == 96)
                {
                    HerstelMelding = 1;
                }
                //



                FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, pProgramId, pProgId, 9992);
                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
                if (pLNV2IRCredidentials != null && pLNV2IRCredidentials.Password != "" && pLNV2IRCredidentials.UserName != "")
                {
                    lUsername = pLNV2IRCredidentials.UserName;
                    lPassword = pLNV2IRCredidentials.Password;
                }
                //if (fusoap.UserName == String.Empty || fusoap.Password == String.Empty || LNVPasswordCheck(fusoap.UserName, fusoap.Password) < 1)
                //{ 
                //    //TODO wachtwoord niet ingevuld of niet correct
                //}

                //lTestServer = configHelper.UseCRDTestserver ? 1 : 0;
                lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                if (lTestServer > 0)
                {
                    
                 
                    if (lTestServer == 1)
                    {
                       
                        lBRSnummer = "10005948";
                        pRecord.Farmnumber = "2510";
                    }
                    else if (lTestServer == 2)
                    {
                    
                        lBRSnummer = "10005948";
                        pRecord.Farmnumber = "152460";
                    }

                }
                String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MELDKIND_" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log";
                //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log");
                Result.Date = DateTime.Today;
                Result.Time = DateTime.Now;
                Result.Lifenumber = pRecord.Lifenumber;
                Result.FarmNumber = pRecord.Farmnumber;

                switch (pRecord.CodeMutation)
                {
                    case 1:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 2:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                        DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                                    pProgId, pRecord.MutationDate,
                                    LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 3:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                        DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                                    pProgId, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 4:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 5:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "IKBAfvoer");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 6:
                        //Dood
                        //Rendac doet het doodmelden


                        //rundvee 1
                        //stieren 2
                        //schapen 3
                        //zoogkoeien 4
                        //geiten 5
                        //witvlees 6
                        //rose 7
                        LogFile = LogFile.Replace("MELDKIND", "Dood");
                        if (pProgId == 3 || pProgId == 5)
                        {
                            DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                                                pProgId, pRecord.MutationDate,
                                                HerstelMelding, pRecord.MeldingNummer,
                                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        }

                        break;
                    case 7:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "Import");
                        unLogger.WriteInfo("Importmelding: " + pRecord.Lifenumber);
                        DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                                    pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                                    pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                                    pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                                    LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                                    pRecord.NrGezondheidsCert, HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 8:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "AanAfvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 9:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "Export");
                        DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                                    pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 10:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "Slacht");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 11:
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 12:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 13:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 16:
                        //Q-Krts vacc1
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc1");
                        DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                                    1,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 17:
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvacc2");
                        DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                                    2,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 18:
                        LogFile = LogFile.Replace("MELDKIND", "QKrtsvaccH");
                        DLLcall.LNVQkoortsVaccinatieMelding(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
                                    3,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
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
                        MUTALOG lIntrekTemp = ConverttoMutLog(pRecord);
                        Result = LNV2MeldingIntrekken(lIntrekTemp, lUBN.UBNid, pProgId, pProgramId, mToken);

                        lStatus = Result.Status;
                        lCode = Result.Code;
                        lOmschrijving = Result.Omschrijving;

                        break;

                    //herstellen ook
                    case 201:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    1,pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 202:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "HerstelGeboorte");
                        DLLcall.LNVIRgeboortemeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.LifenumberMother,
                                    pProgId, pRecord.MutationDate,
                                    LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 203:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDoodgeb");
                        DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                                    pProgId, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 204:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAfvoer");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 205:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelIKBAfvoer");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 206:
                        //Dood
                        LogFile = LogFile.Replace("MELDKIND", "HerstelDood");
                        DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                                    pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 207:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "HerstelImport");
                        DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                                    pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                                    pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                                    pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                                    LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                                    pRecord.NrGezondheidsCert, HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 208:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanAfvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 209:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "HerstelExport");
                        DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                                    pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 210:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelSlacht");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 211:
                        LogFile = LogFile.Replace("MELDKIND", "HerstelAanvoer11");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 212:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "HerstelUitscharen");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 213:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "HerstelNoodslacht");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    1, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
            }
            catch (Exception Exc)
            {
                lStatus = "F";
                Result.Date = DateTime.Now;
                Result.Omschrijving = Exc.Message;
                unLogger.WriteDebug(Exc.ToString());
            }

            Result.Kind = pRecord.CodeMutation;
            if (Result.Status == "")
            {
                Result.Status = lStatus;
                Result.Code = lCode;
                Result.Omschrijving = lOmschrijving + lMeldingsnr;
            }
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            pRecord.MeldResult = Result.Omschrijving;
            //let op lStatus kan leeg zijn na doen van intrekmelding CodeMutation 101 t/m 113 
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;

            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode, pRecord))
            {

                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    if (lMeldingsnr != "")
                    {
                        MutLog.MeldingNummer = lMeldingsnr;
                    }
                    if (pRecord.CodeMutation > 100 && pRecord.CodeMutation < 115)
                    {
                        MutLog.MeldingNummerOrg = pRecord.MeldingNummer;
                    }

                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }


                    if (!Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken))
                        unLogger.WriteError("IRlog", "Fout bij opslaan in mutalog Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber);

                    if (!Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken))
                    {

                        pRecord.Report = 99;
                        UpdateReport(pRecord, mToken);
                        unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr);

                        MUTATION delmut;
                        for (int recusivecounter = 0; recusivecounter < 10; recusivecounter++)
                        {
                            if (DeleteMutation(pRecord, mToken))
                                break;
                            else if (pRecord.Internalnr <= 0 || recusivecounter > 5)
                            {
                                if (pRecord.EventId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByEventId(pRecord.EventId);
                                else if (pRecord.MovId > 0)
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationByMovId(pRecord.MovId);
                                else
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (delmut.Internalnr == 0)
                                {
                                    delmut = Facade.GetInstance().getSaveToDB(mToken).GetMutationById(pRecord.Internalnr);
                                }
                                if (Facade.GetInstance().getSaveToDB(mToken).DeleteMutation(delmut))
                                    break;
                                else
                                    unLogger.WriteError("IRlog", "Fout bij verwijderen MUTATION Farmnumber = " + delmut.Farmnumber + " Lifenumber = " + delmut.Lifenumber + " internalnr = " + delmut.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                            else
                            {
                                unLogger.WriteError("IRlog", "Fout bij verwijderen Mutation Farmnumber = " + pRecord.Farmnumber + " Lifenumber = " + pRecord.Lifenumber + " internalnr = " + pRecord.Internalnr + " Database: " + mToken.getLastChildConnection().getDBNameSlave());
                            }
                        }
                    }
                }
            }
            DLLcall.Dispose();
            return Result;
        }

        /// <summary>
        /// SOAPIRALG.DLL
        /// </summary> SendTo=27,  never used. nov 2019
        /// <param name="pRecord"></param>
        /// <param name="pUBNid"></param>
        /// <param name="pProgId"></param>
        /// <param name="pProgramId"></param>
        /// <param name="mToken"></param>
        /// <param name="pLNV2IRCredidentials"></param>
        /// <returns></returns>
        public SOAPLOG MeldIRFHRS(MUTATION pRecord, int pUBNid, int pProgId, int pProgramId, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials)
        {
            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername, lPassword;
            int lTestServer;
            SOAPLOG Result = new SOAPLOG();
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;
            int HerstelMelding = 0;
            try
            {
                UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
                THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
                String lBRSnummer = lPersoon.Thr_Brs_Number;

                FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9992);
                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
                if (pLNV2IRCredidentials != null && pLNV2IRCredidentials.Password != "" && pLNV2IRCredidentials.UserName != "")
                {
                    lUsername = pLNV2IRCredidentials.UserName;
                    lPassword = pLNV2IRCredidentials.Password;
                }

                lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                if (lTestServer > 0)
                {
                    

                }
                String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_MELDKIND_" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log";
                //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log");
                Result.Date = DateTime.Today;
                Result.Time = DateTime.Now;
                Result.Lifenumber = pRecord.Lifenumber;
                Result.FarmNumber = pRecord.Farmnumber;

                switch (pRecord.CodeMutation)
                {
                    case 1:
                        //Aanvoer
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 2:
                        //Geboorte
                        LogFile = LogFile.Replace("MELDKIND", "Geboorte");
                        bool GeboorteOpfok = pRecord.RegistrationCard == 1;
                        bool FHRSTestServer = lTestServer > 0;
                        DLLcall.FHRSgeboortemelding(FHRSTestServer,
                                    pRecord.Farmnumber, pRecord.Lifenumber, pRecord.Name, pRecord.Worknumber,
                                    pRecord.LifenumberMother, pRecord.MutationDate,
                                    CRDIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), pRecord.Speciality,
                                    GeboorteOpfok,
                                    ref lStatus, ref lOmschrijving, LogFile, MaxString);
                        break;
                    case 3:
                        //Doodgeb.
                        LogFile = LogFile.Replace("MELDKIND", "Doodgeb");
                        DLLcall.LNVIRdoodgeborenmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.LifenumberMother,
                                    pProgId, pRecord.MutationDate,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 4:
                        //Afvoer
                        LogFile = LogFile.Replace("MELDKIND", "Afvoer");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 5:
                        //IKB afvoer
                        LogFile = LogFile.Replace("MELDKIND", "IKBAfvoer");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 6:
                        //Dood
                        //Rendac doet het doodmelden


                        //rundvee 1
                        //stieren 2
                        //schapen 3
                        //zoogkoeien 4
                        //geiten 5
                        //witvlees 6
                        //rose 7
                        LogFile = LogFile.Replace("MELDKIND", "Dood");
                        if (pProgId == 3 || pProgId == 5)
                        {
                            DLLcall.LNVIRdoodmeldingV2(lUsername, lPassword, lTestServer,
                                                pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                                                pProgId, pRecord.MutationDate,
                                                HerstelMelding, pRecord.MeldingNummer,
                                                ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        }

                        break;
                    case 7:
                        //Import
                        LogFile = LogFile.Replace("MELDKIND", "Import");
                        DLLcall.LNVIRimportmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.Name,
                                    pRecord.Worknumber, pRecord.LifenumberMother, pRecord.CountryCodeDepart,
                                    pRecord.CountryCodeBirth, pRecord.AlternateLifeNumber,
                                    pProgId, pRecord.IDRBirthDate, pRecord.MutationDate,
                                    LNVIRGeslacht(pRecord.Sex), LNVIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), Convert.ToBoolean(pRecord.Subsidy),
                                    pRecord.NrGezondheidsCert, HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 8:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("MELDKIND", "AanAfvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 9:
                        //Export
                        LogFile = LogFile.Replace("MELDKIND", "Export");
                        DLLcall.LNVIRexportmeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber,
                                    pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 10:
                        //Slacht
                        LogFile = LogFile.Replace("MELDKIND", "Slacht");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 11:
                        LogFile = LogFile.Replace("MELDKIND", "Aanvoer");
                        DLLcall.LNVIRaanvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberFrom, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 12:
                        //Uitscharen
                        LogFile = LogFile.Replace("MELDKIND", "Uitscharen");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;
                    case 13:
                        //Noodslacht
                        LogFile = LogFile.Replace("MELDKIND", "Noodslacht");
                        DLLcall.LNVIRafvoermeldingV2(lUsername, lPassword, lTestServer,
                                    pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pRecord.FarmNumberTo, pProgId, pRecord.MutationDate,
                                    HerstelMelding, pRecord.MeldingNummer,
                                    ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
                        break;

                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
            }
            catch (Exception Exc)
            {
                lStatus = "F";
                Result.Date = DateTime.Now;
                Result.Omschrijving = Exc.Message;
                unLogger.WriteDebug(Exc.ToString());
            }
            Result.Kind = pRecord.CodeMutation;
            if (Result.Status == "")
            {
                Result.Status = lStatus;
                Result.Code = lCode;
                Result.Omschrijving = lOmschrijving + lMeldingsnr;
            }
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            pRecord.MeldResult = Result.Omschrijving;
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;

            if (lStatus.Equals("G") || lStatus.Equals("W") || IRNietHerhalen(lCode.Trim(), pRecord))
            {
                lock (Facade.GetInstance())
                {
                    MUTALOG MutLog = ConverttoMutLog(pRecord);
                    MutLog.ReportDate = Result.Date;
                    MutLog.ReportTime = Result.Time;
                    MutLog.MeldingNummer = lMeldingsnr;
                    Facade.GetInstance().getMeldingen().InsertMutLog(MutLog, mToken);
                    Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken);
                    try
                    {
                        if (pRecord.MovId > 0)
                        {
                            MOVEMENT mov = Facade.GetInstance().getSaveToDB(mToken).GetMovementByMovId(pRecord.MovId);
                            if (mov.ReportDate <= new DateTime(1900, 1, 1))
                            {
                                mov.ReportDate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetMovementReportDate(mov);
                            }


                        }
                        else if (pRecord.EventId > 0)
                        {
                            BIRTH bir = Facade.GetInstance().getSaveToDB(mToken).GetBirth(pRecord.EventId);

                            if (bir.Reportdate <= new DateTime(1900, 1, 1))
                            {
                                bir.Reportdate = Result.Date;
                                Facade.GetInstance().getSaveToDB(mToken).SetBirthReportDate(bir);
                            }

                        }
                    }
                    catch
                    {
                        unLogger.WriteInfo("Fout bij opslaan Reportdate");
                    }
                }
            }
            DLLcall.Dispose();
            return Result;
        }

        #region België

        public List<SOAPLOG> MeldSaniTrace(List<MUTATION> pRecords, int pUBNid, int pProgId, DBConnectionToken mToken, bool apisanitrace = false)
        {
            List<SOAPLOG> Returns = new List<SOAPLOG>();
            if (!apisanitrace)
            {
                foreach (MUTATION pRecord in pRecords)
                {
                    Returns.Add(MeldSaniTrace(pRecord, pUBNid, pProgId, mToken));

                }
                return Returns;
            }
            //int MaxString = 255;
            String lUsername, lPassword;


            String Taal = utils.getcurrentlanguage();

         

            UBN ubn = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD Third = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn.ThrID);
            string Farmnumber = ubn.Bedrijfsnummer.Replace("UBN", "");
            //String BTWnr = Third.ThrVATNumber;
            //String Annexnr = UBN.Extranummer1;
            //String Inrichtingsnr = "BE" + Third.Thr_Brs_Number;
            //String Beslagnr = Inrichtingsnr + "-" + Annexnr;
            //String Annexnr = "";
            //String Inrichtingsnr = "";
            //String Beslagnr = "";
            //String PENnrA = "";
            //utils.getSanitraceNummers(Third, ubn, out Annexnr, out Inrichtingsnr, out Beslagnr, out PENnrA);
            //PENnrA = "";
            /*
                Voor Belgie moet je PEN dus leeg laten.
                Met vriendelijke groet,
                Nico de Groot
                V.S.M. Automatisering
                ----- Original Message -----
                From: Jos Wijdeven (VSM Automatisering bv)
                To: Nico de Groot (V.S.M. Automatisering)
                Sent: Friday, November 06, 2015 4:39 PM
                Subject: Re: Fw: test optima Belgie

                Nico,
                Een gedeeld beslag is als 2 veehouder een beslag delen. Dus zelfde beslagnr meer verschillend PEN nr. Zal niet veel voorkomen maar is welk mogelijk (volgens mij is dit bij tenminste 1 vilatca mester).
                Zonder PEN nr komen er wel dieren terug, zie bijlage.
             */
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9994);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            string ipadress = Facade.GetInstance().getSaveToDB(mToken).getLastIplogin(ubn.ThrID);
            string apipassword = Facade.GetInstance().getRechten().DeCodeer_Tekst(ConfigurationManager.AppSettings["ApiPassword"]);
            int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            if (lTestServer > 0)
            {
                lTestServer = 1;
                if (Third.ThrCountry == "125")
                {
                    lTestServer = 11;//TEST Luxemburg
                }
            }
            else
            {
                if (Third.ThrCountry == "125")
                {
                    lTestServer = 10; //LIVE Luxemburg
                }
            }
            SanitelMeldingen smld = new SanitelMeldingen(ConfigurationManager.AppSettings["ApiUsername"], apipassword, lTestServer, ipadress, Convert.ToInt32(Third.ThrCountry));
            if (string.IsNullOrWhiteSpace(smld.Api_access_token))
            {

                Returns.Add(new Soaplogmelding
                {
                    Status = "F",
                    Omschrijving = "Api_access_token not set;",
                    Date = DateTime.Today,
                    Time = DateTime.Now,

                    FarmNumber = ubn.Bedrijfsnummer
                });
                return Returns;
            }
            String LogFile = unLogger.getLogDir("IenR") + ubn.Bedrijfsnummer + "_STIR" + "_SOORT_" + DateTime.Now.Ticks + ".log";
            List<soaplog> slreturns = new List<soaplog>();
            var groepen = pRecords.GroupBy(x => new { x.CodeMutation });
            foreach (var groep in groepen)
            {

                int CodeMutation = groep.ElementAt(0).CodeMutation;

                int Diersoort;
                int SubDiersoort;

                switch (pProgId)
                {
                    case 1:
                        Diersoort = 1;
                        SubDiersoort = 1;
                        break;
                    case 6:
                        Diersoort = 1;
                        SubDiersoort = 2;
                        break;
                    case 7:
                        Diersoort = 1;
                        SubDiersoort = 2;
                        break;
                    default:
                        Diersoort = 1;
                        SubDiersoort = 1;
                        break;
                }
                int PENVolgnr = 0;

                switch (CodeMutation)
                {
                    case (int)LABELSConst.CodeMutation.AANVOER:
                        //Aanvoer
                        LogFile = LogFile.Replace("_SOORT_", "_Aanvoer_");
                      
                        List<SanitelImportNotification> imports = new List<SanitelImportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to.ThrID);
                            String ThrVATNumber_to = Third_to.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelImportNotification imp = new SanitelImportNotification();
                            imp.AnimalType = Diersoort;
                            imp.SubAnimalKind = SubDiersoort;
                            imp.ArrivalState = pRecord.AniState;
                            imp.ImportDate = pRecord.MutationDate;
                            imp.Licenceplate = pRecord.LicensePlate;
                            imp.Lifenumber = pRecord.Lifenumber;
                            imp.MovementId = pRecord.MovId;
                            imp.ProductionUnit = "";
                            imp.ProductionUnitIndex = 0;
                            imp.VatNrDestination = ThrVATNumber_to;
                            imp.RegistrationCard = pRecord.RegistrationCard;
                            imp.VersionNrPassport = pRecord.VersienrVertrekluik;
                            imports.Add(imp);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        slreturns.AddRange(smld.SendImportNotifications(Farmnumber, lTestServer, imports, LogFile));
 

                        break;
                    case (int)LABELSConst.CodeMutation.GEBOORTE:
                        //Geboorte
                     

                        LogFile = LogFile.Replace("_SOORT_", "_Geboorte_");
                        List<SanitelBirthNotification> births = new List<SanitelBirthNotification>();
                        foreach (var pRecord in groep)
                        {
                            if (pRecord.Weight <= 20 || pRecord.Weight >= 80)
                            {
                                pRecord.Weight = 0;
                            }
                            SanitelBirthNotification brt = new SanitelBirthNotification();
                            brt.AnimalType = Diersoort;

                            brt.Beefiness = pRecord.MeatScore;
                            brt.BirthCourse = pRecord.CalvingEase;
                            brt.Birthdate = pRecord.MutationDate;
                            brt.BirthWeight = (int)pRecord.Weight;
                            brt.Borndead = false;
                            brt.EmbryoTransplant = pRecord.ET > 0;
                            brt.EventId = pRecord.EventId;
                            brt.Haircolor = pRecord.AniHaircolor_Memo;
                            brt.Lifenumber = pRecord.Lifenumber;
                            brt.LifenumberMother = pRecord.LifenumberMother;
                            brt.MotherBoughtRecent = pRecord.MotherBoughtRecent > 0;
                            brt.Name = pRecord.Name;
                            brt.Nling = pRecord.Nling;
                            brt.RaceType = pRecord.Race;
                            brt.RegistrationCard = pRecord.RegistrationCard;
                            brt.Sex = pRecord.Sex;
                            brt.Specialities = pRecord.Speciality;
                            //brt.SucklingCalf = false;
                            births.Add(brt);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);

                        }
                        slreturns.AddRange(smld.SendBirthNotifications(Farmnumber, lTestServer, births, LogFile));
                        break;
                    case (int)LABELSConst.CodeMutation.DOODGEBOREN:
                        //Doodgeb.
                        LogFile = LogFile.Replace("_SOORT_", "_Doodgeb_");
                        List<SanitelBirthNotification> birthsd = new List<SanitelBirthNotification>();
                        foreach (var pRecord in groep)
                        {
                            SanitelBirthNotification brtd = new SanitelBirthNotification();
                            brtd.AnimalType = Diersoort;

                            brtd.Beefiness = pRecord.MeatScore;
                            brtd.BirthCourse = pRecord.CalvingEase;
                            brtd.Birthdate = pRecord.MutationDate;
                            brtd.BirthWeight = (int)pRecord.Weight;
                            brtd.Borndead = true;
                            brtd.EmbryoTransplant = pRecord.ET > 0;
                            brtd.EventId = pRecord.EventId;
                            brtd.Haircolor = pRecord.AniHaircolor_Memo;
                            brtd.Lifenumber = pRecord.Lifenumber;
                            brtd.LifenumberMother = pRecord.LifenumberMother;
                            brtd.MotherBoughtRecent = pRecord.MotherBoughtRecent > 0;
                            brtd.Name = pRecord.Name;
                            brtd.Nling = pRecord.Nling;
                            brtd.RaceType = pRecord.Race;
                            brtd.RegistrationCard = pRecord.RegistrationCard;
                            brtd.Sex = pRecord.Sex;
                            brtd.Specialities = pRecord.Speciality;
                            //brt.SucklingCalf = false;
                            birthsd.Add(brtd);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        slreturns.AddRange(smld.SendBirthNotifications(Farmnumber, lTestServer, birthsd, LogFile));
                      
                        break;
                    case (int)LABELSConst.CodeMutation.AFVOER:
                        //Afvoer
                        LogFile = LogFile.Replace("_SOORT_", "_Afvoer_");
                       
                        List<SanitelExportNotification> afvs = new List<SanitelExportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to4 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to4 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to4.ThrID);
                            String ThrVATNumber_to4 = Third_to4.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelExportNotification ep = new SanitelExportNotification();
                            ep.AnimalType = Diersoort;
                            ep.SubAnimalKind = SubDiersoort;

                            ep.ExportDate = pRecord.MutationDate;
                            ep.Lifenumber = pRecord.Lifenumber;
                            ep.MovementId = pRecord.MovId;
                            ep.Licenceplate = pRecord.LicensePlate;
                            ep.RegistrationCard = pRecord.RegistrationCard;
                            ep.TypeOfExport = pRecord.AniState;
                            ep.VatNumberDestination = ThrVATNumber_to4;
                            ep.VrvExportReason = VRVafvoerreden;
                            ep.VersionNrPassport = pRecord.VersienrVertrekluik;
                            ep.Licenceplate = "";
                            ep.VatNumberTransporter = "";
                            afvs.Add(ep);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        slreturns.AddRange(smld.SendExportNotifications(Farmnumber, lTestServer, afvs, LogFile));
                      
                        break;
                    case (int)LABELSConst.CodeMutation.IKBAFVOER:
                        //IKB afvoer
                        LogFile = LogFile.Replace("_SOORT_", "_IKB_Afvoer_");
                       
                        List<SanitelExportNotification> afvs5 = new List<SanitelExportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to5 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to5 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to5.ThrID);
                            String ThrVATNumber_to5 = Third_to5.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelExportNotification ep5 = new SanitelExportNotification();
                            ep5.AnimalType = Diersoort;
                            ep5.SubAnimalKind = SubDiersoort;
                            ep5.ExportDate = pRecord.MutationDate;
                            ep5.Lifenumber = pRecord.Lifenumber;
                            ep5.MovementId = pRecord.MovId;
                            ep5.Licenceplate = pRecord.LicensePlate;
                            ep5.RegistrationCard = pRecord.RegistrationCard;
                            ep5.TypeOfExport = pRecord.AniState;
                            ep5.VatNumberDestination = ThrVATNumber_to5;
                            ep5.VrvExportReason = VRVafvoerreden;
                            ep5.VersionNrPassport = pRecord.VersienrVertrekluik;
                            afvs5.Add(ep5);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        slreturns.AddRange(smld.SendExportNotifications(Farmnumber, lTestServer, afvs5, LogFile));
                     
                        break;
                    case (int)LABELSConst.CodeMutation.DOOD:
                        //Dood
                        LogFile = LogFile.Replace("_SOORT_", "_Dood_");
                        List<SanitelDeathNotification> deads = new List<SanitelDeathNotification>();
                        foreach (var pRecord in groep)
                        {
                            THIRD ThirdRendac = Facade.GetInstance().getSaveToDB(mToken).GetThirdByHouseNrAndZipCode("Rendac", "Son");
                          
                            String BTWnrRendac = ThirdRendac.ThrVATNumber;
                            String InrichtingsnrRendac = ThirdRendac.ThrBeslagnr;
                            SanitelDeathNotification d = new SanitelDeathNotification();
                            d.AnimalType = Diersoort;
                            d.SubAnimalKind = SubDiersoort;
                            d.DeathDate = pRecord.MutationDate;
                            d.Lifenumber = pRecord.Lifenumber;
                            d.MovementId = pRecord.MovId;
                            d.VersionNrPassport = pRecord.VersienrVertrekluik;
                            //d.VrvExportReason = VRVafvoerreden;

                            deads.Add(d);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        slreturns.AddRange(smld.SendDeathNotifications(Farmnumber, lTestServer, deads, LogFile));
                     
                        break;
                    case (int)LABELSConst.CodeMutation.IMPORT:
                        //Import

                        LogFile = LogFile.Replace("_SOORT_", "_Import_");
                    
                        List<SanitelImportNotification> imports7 = new List<SanitelImportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to7 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to7 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to7.ThrID);
                            String ThrVATNumber_to7 = Third_to7.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelImportNotification imp7 = new SanitelImportNotification();
                            imp7.AnimalType = Diersoort;
                            imp7.SubAnimalKind = SubDiersoort;
                            imp7.ArrivalState = pRecord.AniState;
                            imp7.ImportDate = pRecord.MutationDate;
                            imp7.Licenceplate = pRecord.LicensePlate;
                            imp7.Lifenumber = pRecord.Lifenumber;
                            imp7.MovementId = pRecord.MovId;
                            imp7.VatNrDestination = ThrVATNumber_to7;
                            imp7.VersionNrPassport = pRecord.VersienrVertrekluik;
                            imp7.RegistrationCard = VRVidKaart;
                            imp7.ProductionUnit = "";
                            imp7.ProductionUnitIndex = 0;//

                            imports7.Add(imp7);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        slreturns.AddRange(smld.SendImportNotifications(Farmnumber, lTestServer, imports7, LogFile));
                   
                        break;
                    case (int)LABELSConst.CodeMutation.AANAFVOER:
                        //Aan-/afvoer
                        LogFile = LogFile.Replace("_SOORT_", "_8_Aanvoer_");
                        //DLLcall.STIRaanvoermelding(lUsername, lPassword, lTestServer,
                        //          Taal, pRecord.Farmnumber, PENVolgnr, Inrichtingsnr,
                        //          Beslagnr, pRecord.Lifenumber,
                        //          pRecord.MutationDate, pRecord.MutationDate,
                        //          Diersoort, SubDiersoort,
                        //          pRecord.AniState, pRecord.VersienrVertrekluik,
                        //          VRVidKaart,
                        //          pRecord.Purchaser, pRecord.Vervoersnr,
                        //          pRecord.LicensePlate, LogFile,
                        //          ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        LogFile = LogFile.Replace("_SOORT_", "_8_Afvoer_");
                        //DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                        //          Taal, Inrichtingsnr,
                        //          Beslagnr, pRecord.Lifenumber,
                        //          pRecord.MutationDate, pRecord.MutationDate,
                        //          0, Diersoort, SubDiersoort,
                        //          pRecord.VersienrVertrekluik, VRVafvoerreden,
                        //          pRecord.Purchaser, pRecord.Vervoersnr,
                        //          pRecord.LicensePlate, LogFile,
                        //          ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                        break;
                    case (int)LABELSConst.CodeMutation.EXPORT:
                        //Export
                        String ExportCertificaat = "";
                        LogFile = LogFile.Replace("_SOORT_", "_Export_");
                       
                        List<SanitelExportNotification> afvs9 = new List<SanitelExportNotification>();
                        foreach (var pRecord in groep)
                        {
                            ExportCertificaat = "";
                            UBN ubn_to9 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to9 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to9.ThrID);
                            String ThrVATNumber_to9 = Third_to9.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelExportNotification ep9 = new SanitelExportNotification();
                            ep9.AnimalType = Diersoort;
                            ep9.SubAnimalKind = SubDiersoort;

                            ep9.ExportDate = pRecord.MutationDate;
                            ep9.Lifenumber = pRecord.Lifenumber;
                            ep9.MovementId = pRecord.MovId;
                            ep9.Licenceplate = pRecord.LicensePlate;
                            ep9.RegistrationCard = pRecord.RegistrationCard;
                            ep9.TypeOfExport = pRecord.AniState;
                            ep9.VatNumberDestination = ThrVATNumber_to9;
                            ep9.VrvExportReason = VRVafvoerreden;
                            ep9.VersionNrPassport = pRecord.VersienrVertrekluik;

                            afvs9.Add(ep9);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        var sl9 = smld.SendExportNotifications(Farmnumber, lTestServer, afvs9, LogFile);
                  
                        break;
                    case (int)LABELSConst.CodeMutation.SLACHT:
                        //Slacht
                        LogFile = LogFile.Replace("_SOORT_", "_Slacht_");
                     
                        List<SanitelExportNotification> afvsS = new List<SanitelExportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_toS = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_toS = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_toS.ThrID);
                            String ThrVATNumber_toS = Third_toS.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelExportNotification epS = new SanitelExportNotification();
                            epS.AnimalType = Diersoort;
                            epS.SubAnimalKind = SubDiersoort;
                            epS.ExportDate = pRecord.MutationDate;
                            epS.Lifenumber = pRecord.Lifenumber;
                            epS.MovementId = pRecord.MovId;
                            epS.Licenceplate = pRecord.LicensePlate;
                            epS.RegistrationCard = pRecord.RegistrationCard;
                            epS.TypeOfExport = 1;// pRecord.AniState;
                            epS.VatNumberDestination = ThrVATNumber_toS;
                            epS.VrvExportReason = VRVafvoerreden;
                            epS.VersionNrPassport = pRecord.VersienrVertrekluik;

                            afvsS.Add(epS); 
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        var sl10 = smld.SendExportNotifications(Farmnumber, lTestServer, afvsS, LogFile);
                  
                        break;
                    case (int)LABELSConst.CodeMutation.INSCHAREN:
                        //Inscharen
                        LogFile = LogFile.Replace("_SOORT_", "_Inscharen_");
                      
                        List<SanitelImportNotification> imports11 = new List<SanitelImportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to11 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to11 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to11.ThrID);
                            String ThrVATNumber_to11 = Third_to11.ThrVATNumber;
                            SanitelImportNotification imp11 = new SanitelImportNotification();
                            imp11.AnimalType = Diersoort;
                            imp11.SubAnimalKind = SubDiersoort;
                            imp11.ArrivalState = pRecord.AniState;
                            imp11.ImportDate = pRecord.MutationDate;
                            imp11.Licenceplate = pRecord.LicensePlate;
                            imp11.Lifenumber = pRecord.Lifenumber;
                            imp11.MovementId = pRecord.MovId;
                            imp11.ProductionUnit = "";
                            imp11.ProductionUnitIndex = 0;
                            imp11.VatNrDestination = ThrVATNumber_to11;
                            imp11.RegistrationCard = pRecord.RegistrationCard;
                            imp11.VersionNrPassport = pRecord.VersienrVertrekluik;
                            imports11.Add(imp11);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        var sl11 = smld.SendImportNotifications(Farmnumber, lTestServer, imports11, LogFile);
                      
                        break;
                    case (int)LABELSConst.CodeMutation.UITSCHAREN:
                        //Uitscharen
                        LogFile = LogFile.Replace("_SOORT_", "_Uitscharen_");
                  
                        List<SanitelExportNotification> afvs12 = new List<SanitelExportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to12 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to12 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to12.ThrID);
                            String ThrVATNumber_to12 = Third_to12.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelExportNotification ep12 = new SanitelExportNotification();
                            ep12.AnimalType = Diersoort;
                            ep12.SubAnimalKind = SubDiersoort;

                            ep12.ExportDate = pRecord.MutationDate;
                            ep12.Lifenumber = pRecord.Lifenumber;
                            ep12.MovementId = pRecord.MovId;
                            ep12.Licenceplate = pRecord.LicensePlate;
                            ep12.RegistrationCard = pRecord.RegistrationCard;
                            ep12.TypeOfExport = pRecord.AniState;
                            ep12.VatNumberDestination = ThrVATNumber_to12;
                            ep12.VrvExportReason = VRVafvoerreden;
                            ep12.VersionNrPassport = pRecord.VersienrVertrekluik;

                            afvs12.Add(ep12);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        var sl12 = smld.SendExportNotifications( Farmnumber, lTestServer, afvs12, LogFile);
                      
                        break;
                    case (int)LABELSConst.CodeMutation.NOODSLACHT:
                        //Noodslacht
                        LogFile = LogFile.Replace("_SOORT_", "_Noodslacht_");
                    
                        List<SanitelExportNotification> afvs13 = new List<SanitelExportNotification>();
                        foreach (var pRecord in groep)
                        {
                            UBN ubn_to13 = Facade.GetInstance().getSaveToDB(mToken).getUBNByBedrijfsnummer(pRecord.FarmNumberTo);
                            THIRD Third_to13 = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(ubn_to13.ThrID);
                            String ThrVATNumber_to13 = Third_to13.ThrVATNumber;

                            int VRVidKaart = pRecord.RegistrationCard;
                            int VRVafvoerreden = pRecord.CullingReason;//
                            int PaspoortUrgentie = 1; // Snelheid afdruk
                            bool Meerling = pRecord.Nling > 1;

                            SanitelExportNotification ep13 = new SanitelExportNotification();
                            ep13.AnimalType = Diersoort;
                            ep13.SubAnimalKind = SubDiersoort;

                            ep13.ExportDate = pRecord.MutationDate;
                            ep13.Lifenumber = pRecord.Lifenumber;
                            ep13.MovementId = pRecord.MovId;
                            ep13.Licenceplate = pRecord.LicensePlate;
                            ep13.RegistrationCard = pRecord.RegistrationCard;
                            ep13.TypeOfExport = 1;// pRecord.AniState;
                            ep13.VatNumberDestination = ThrVATNumber_to13;
                            ep13.VrvExportReason = VRVafvoerreden;
                            ep13.VersionNrPassport = pRecord.VersienrVertrekluik;

                            afvs13.Add(ep13);
                            SOAPLOG sl = new SOAPLOG();
                            sl.Date = DateTime.Now.Date;
                            sl.FarmNumber = Farmnumber;
                            sl.Lifenumber = pRecord.Lifenumber;
                            sl.Kind = CodeMutation;
                            sl.Omschrijving = "";
                            sl.Status = "G";
                            sl.ThrId = Third.ThrId;
                            sl.Time = DateTime.Now;
                            Returns.Add(sl);
                        }
                        var sl13 = smld.SendExportNotifications(Farmnumber, lTestServer, afvs13, LogFile);
                     
                        break;
                }
                unLogger.WriteInfo(" LogFile:" + LogFile);
                foreach (var pRecord in pRecords)
                {
                    var s = from n in Returns
                            where n.Lifenumber == pRecord.Lifenumber
                            select n;
                    if (s.Count() > 0)
                    {
                        var Result = s.ElementAt(0);

                        var m = from n in slreturns
                                where n.lifenumber == Result.Lifenumber
                                select n;
                        //TODO verbind slreturns met Result 

                        Result.Kind = pRecord.CodeMutation;
                        Result.Status = m.ElementAt(0).status;
                        Result.Code = m.ElementAt(0).meldnummer;
                        Result.Omschrijving = m.ElementAt(0).omschrijving;
                        Result.FarmNumber = pRecord.Farmnumber;
                        Result.Lifenumber = pRecord.Lifenumber;
                        pRecord.MeldResult = Result.Omschrijving;
                        if (Result.Status.Equals("G")) pRecord.Returnresult = 1;
                        else if (Result.Status.Equals("F")) pRecord.Returnresult = 2;
                        else if (Result.Status.Equals("W")) pRecord.Returnresult = 3;
                        else pRecord.Returnresult = 98;
                        if (Result.Status.Equals("G") || Result.Status.Equals("W"))
                        {
                            pRecord.ReportDate = Result.Date;
                            pRecord.ReportTime = Result.Time;
                            pRecord.MeldingNummer = m.ElementAt(0).meldnummer;
                            Facade.GetInstance().getMeldingen().InsertMutLog(ConverttoMutLog(pRecord), mToken);
                            Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken);
                        }
                        else if (Result.Status.Equals("F"))
                        {
                            pRecord.ReportDate = Result.Date;
                            pRecord.ReportTime = Result.Time;
                        }
                    }
                }
            }
            return Returns;
        }

        /// <summary>
        /// SANITRACEALG.DLL
        /// </summary>
        /// <param name="pRecord"></param>
        /// <param name="pUBNid"></param>
        /// <param name="pProgId"></param>
        /// <param name="mToken"></param>
        /// <returns></returns>
        public SOAPLOG MeldSaniTrace(MUTATION pRecord, int pUBNid, int pProgId, DBConnectionToken mToken)
        {
            int MaxString = 255;
            String lUsername, lPassword;


            String Taal = utils.getcurrentlanguage();

            int VRVidKaart = pRecord.RegistrationCard;
            int VRVafvoerreden = pRecord.CullingReason;//
            int PaspoortUrgentie = 1; // Snelheid afdruk
            bool Meerling = pRecord.Nling > 1;

            UBN UBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD Third = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(UBN.ThrID);
            String BTWnr = Third.ThrVATNumber;
            //String Annexnr = UBN.Extranummer1;
            //String Inrichtingsnr = "BE" + Third.Thr_Brs_Number;
            //String Beslagnr = Inrichtingsnr + "-" + Annexnr;
            String Annexnr = "";
            String Inrichtingsnr = "";
            String Beslagnr = "";
            String PENnrA = "";
            utils.getSanitraceNummers(Third, UBN, out Annexnr, out Inrichtingsnr, out Beslagnr, out PENnrA);
            PENnrA = "";
            /*
                Voor Belgie moet je PEN dus leeg laten.
                Met vriendelijke groet,
                Nico de Groot
                V.S.M. Automatisering
                ----- Original Message -----
                From: Jos Wijdeven (VSM Automatisering bv)
                To: Nico de Groot (V.S.M. Automatisering)
                Sent: Friday, November 06, 2015 4:39 PM
                Subject: Re: Fw: test optima Belgie

                Nico,
                Een gedeeld beslag is als 2 veehouder een beslag delen. Dus zelfde beslagnr meer verschillend PEN nr. Zal niet veel voorkomen maar is welk mogelijk (volgens mij is dit bij tenminste 1 vilatca mester).
                Zonder PEN nr komen er wel dieren terug, zie bijlage.
             */
            int lTestServer;
            String lStatus = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = string.Empty;

            SOAPLOG Result = new SOAPLOG();

            String LogFile = unLogger.getLogDir("IenR") + pRecord.Farmnumber + "_STIR" + "_SOORT_" + DateTime.Now.Ticks + ".log";
            //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "STIR" + pRecord.Farmnumber + "-" + DateTime.Now.Ticks + ".log");

            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9994);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            if (lTestServer > 0)
            {
                lTestServer = 1;
               
                if (Third.ThrCountry == "125")
                {
                    lTestServer = 11;//TEST Luxemburg
                }
            }
            else
            {
                if (Third.ThrCountry == "125")
                {
                    lTestServer = 10; //LIVE Luxemburg
                }
            }
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.FarmNumber = pRecord.Farmnumber;
            Win32SANITRACEALG DLLcall = new Win32SANITRACEALG();

            int Diersoort;
            int SubDiersoort;

            switch (pProgId)
            {
                case 1:
                    Diersoort = 1;
                    SubDiersoort = 1;
                    break;
                case 6:
                    Diersoort = 1;
                    SubDiersoort = 2;
                    break;
                case 7:
                    Diersoort = 1;
                    SubDiersoort = 2;
                    break;
                default:
                    Diersoort = 1;
                    SubDiersoort = 1;
                    break;
            }
            int PENVolgnr = 0;

            switch (pRecord.CodeMutation)
            {
                case 1:
                    //Aanvoer
                    LogFile = LogFile.Replace("_SOORT_", "_Aanvoer_");
                    DLLcall.STIRaanvoermelding(lUsername, lPassword, lTestServer,
                              Taal, pRecord.Farmnumber, PENVolgnr, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              Diersoort, SubDiersoort,
                              pRecord.AniState, pRecord.VersienrVertrekluik,
                              VRVidKaart,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 2:
                    //Geboorte
                    if (pRecord.Weight <= 20 || pRecord.Weight >= 80)
                    {
                        pRecord.Weight = 0;
                    }
                   
                    LogFile = LogFile.Replace("_SOORT_", "_Geboorte_");
                    DLLcall.STIRgeboortemelding(lUsername, lPassword, lTestServer,
                                Taal, pRecord.Farmnumber, BTWnr, Inrichtingsnr, Beslagnr,
                                pRecord.Lifenumber, pRecord.Name,
                                pRecord.LifenumberMother,
                                pRecord.MutationDate,
                                false,
                                Convert.ToBoolean(pRecord.MotherBoughtRecent),
                                Convert.ToBoolean(pRecord.ET), pProgId == 4,
                                Diersoort, pRecord.Race, pRecord.Sex, STIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                                pRecord.Speciality,
                                pRecord.CalvingEase, Convert.ToInt32(pRecord.Weight), pRecord.MeatScore,
                                PaspoortUrgentie, VRVidKaart,
                                Meerling, LogFile,
                                ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 3:
                    //Doodgeb.
                    LogFile = LogFile.Replace("_SOORT_", "_Doodgeb_");
                    DLLcall.STIRgeboortemelding(lUsername, lPassword, lTestServer,
                                Taal, pRecord.Farmnumber, BTWnr, Inrichtingsnr, Beslagnr,
                                pRecord.Lifenumber, pRecord.Name,
                                pRecord.LifenumberMother,
                                pRecord.MutationDate,
                                true,
                                Convert.ToBoolean(pRecord.MotherBoughtRecent),
                                Convert.ToBoolean(pRecord.ET), pProgId == 4,
                                Diersoort, pRecord.Race, pRecord.Sex,
                                STIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo), pRecord.Speciality,
                                pRecord.CalvingEase, Convert.ToInt32(pRecord.Weight), pRecord.MeatScore,
                                PaspoortUrgentie, VRVidKaart,
                                Meerling, LogFile,
                                ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 4:
                    //Afvoer
                    LogFile = LogFile.Replace("_SOORT_", "_Afvoer_");
                    DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                              Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              0, Diersoort, SubDiersoort,
                              pRecord.VersienrVertrekluik, VRVafvoerreden,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 5:
                    //IKB afvoer
                    LogFile = LogFile.Replace("_SOORT_", "_IKB_Afvoer_");
                    DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                              Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              0, Diersoort, SubDiersoort,
                              pRecord.VersienrVertrekluik, VRVafvoerreden,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 6:
                    //Dood
                    THIRD ThirdRendac = Facade.GetInstance().getSaveToDB(mToken).GetThirdByHouseNrAndZipCode("Rendac", "Son");
                    LogFile = LogFile.Replace("_SOORT_", "_Dood_");
                    String BTWnrRendac = ThirdRendac.ThrVATNumber;
                    String InrichtingsnrRendac = ThirdRendac.ThrBeslagnr;
                    DLLcall.STIRdoodmelding(lUsername, lPassword, lTestServer,
                             Taal, Inrichtingsnr,
                             Beslagnr, pRecord.Lifenumber,
                             pRecord.MutationDate,
                             Diersoort, SubDiersoort,
                             pRecord.VersienrVertrekluik, VRVafvoerreden,
                             pRecord.Vervoersnr, BTWnrRendac, InrichtingsnrRendac, LogFile,
                             ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 7:
                    //Import
                    String ImportCertificaat = "";
                    LogFile = LogFile.Replace("_SOORT_", "_Import_");
                    DLLcall.STIRimportmelding(lUsername, lPassword, lTestServer,
                                 Taal, pRecord.Farmnumber, Inrichtingsnr,
                                 Beslagnr, pRecord.Lifenumber, pRecord.Name,
                                 pRecord.LifenumberMother,
                                 pRecord.IDRBirthDate, pRecord.MutationDate,
                                 Diersoort, SubDiersoort,
                                 pRecord.Race, pRecord.Sex, STIRHaarKleur(pRecord.Haircolor, pRecord.AniHaircolor_Memo),
                                 pRecord.AniState, PaspoortUrgentie, VRVidKaart,
                                 pRecord.CountryCodeBirth, ImportCertificaat, pRecord.Purchaser, LogFile,
                                 ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 8:
                    //Aan-/afvoer
                    LogFile = LogFile.Replace("_SOORT_", "_8_Aanvoer_");
                    DLLcall.STIRaanvoermelding(lUsername, lPassword, lTestServer,
                              Taal, pRecord.Farmnumber, PENVolgnr, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              Diersoort, SubDiersoort,
                              pRecord.AniState, pRecord.VersienrVertrekluik,
                              VRVidKaart,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    LogFile = LogFile.Replace("_SOORT_", "_8_Afvoer_");
                    DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                              Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              0, Diersoort, SubDiersoort,
                              pRecord.VersienrVertrekluik, VRVafvoerreden,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 9:
                    //Export
                    String ExportCertificaat = "";
                    LogFile = LogFile.Replace("_SOORT_", "_Export_");
                    DLLcall.STIRexportmelding(lUsername, lPassword, lTestServer,
                             Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate,
                             Diersoort,
                             pRecord.Vervoersnr,
                             ExportCertificaat, pRecord.CountryCodeDepart, LogFile,
                             ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 10:
                    //Slacht
                    LogFile = LogFile.Replace("_SOORT_", "_Slacht_");
                    DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                              Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              1, Diersoort, SubDiersoort,
                              pRecord.VersienrVertrekluik, VRVafvoerreden,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 11:
                    //Inscharen
                    LogFile = LogFile.Replace("_SOORT_", "_Inscharen_");
                    DLLcall.STIRaanvoermelding(lUsername, lPassword, lTestServer,
                              Taal, pRecord.Farmnumber, PENVolgnr, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              Diersoort, SubDiersoort,
                              pRecord.AniState, pRecord.VersienrVertrekluik,
                              VRVidKaart,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 12:
                    //Uitscharen
                    LogFile = LogFile.Replace("_SOORT_", "_Uitscharen_");
                    DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                              Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              0, Diersoort, SubDiersoort,
                              pRecord.VersienrVertrekluik, VRVafvoerreden,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
                case 13:
                    //Noodslacht
                    LogFile = LogFile.Replace("_SOORT_", "_Noodslacht_");
                    DLLcall.STIRafvoermelding(lUsername, lPassword, lTestServer,
                              Taal, Inrichtingsnr,
                              Beslagnr, pRecord.Lifenumber,
                              pRecord.MutationDate, pRecord.MutationDate,
                              1, Diersoort, SubDiersoort,
                              pRecord.VersienrVertrekluik, VRVafvoerreden,
                              pRecord.Purchaser, pRecord.Vervoersnr,
                              pRecord.LicensePlate, LogFile,
                              ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
                    break;
            }
            unLogger.WriteInfo(" LogFile:" + LogFile);
            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lMeldingsnr;
            Result.Omschrijving = lOmschrijving;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            pRecord.MeldResult = Result.Omschrijving;
            if (lStatus.Equals("G")) pRecord.Returnresult = 1;
            else if (lStatus.Equals("F")) pRecord.Returnresult = 2;
            else if (lStatus.Equals("W")) pRecord.Returnresult = 3;
            else pRecord.Returnresult = 98;
            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {
                pRecord.ReportDate = Result.Date;
                pRecord.ReportTime = Result.Time;
                pRecord.MeldingNummer = lMeldingsnr;
                Facade.GetInstance().getMeldingen().InsertMutLog(ConverttoMutLog(pRecord), mToken);
                Facade.GetInstance().getMeldingen().DeleteMutation(pRecord, mToken);
            }
            else if (lStatus.Equals("F"))
            {
                pRecord.ReportDate = Result.Date;
                pRecord.ReportTime = Result.Time;
            }
            return Result;
        }

        /// <summary>
        /// Soap  VSM.RUMA.SOAPSANITEL
        /// </summary>
        /// <param name="mToken"></param>
        /// <param name="pUBNid"></param>
        /// <param name="pProgId"></param>
        /// <param name="pProgramid"></param>
        /// <param name="lLevensnr"></param>
        /// <param name="SoortMelding"></param>
        /// <param name="Begindatum"></param>
        /// <param name="Einddatum"></param>
        /// <param name="OutputFile"></param>
        /// <returns></returns>
        public override SOAPLOG STRaadplegenMeldingenAlg(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                    String lLevensnr, int SoortMelding, DateTime Begindatum, DateTime Einddatum, String OutputFile)
        {

            String Taal = utils.getcurrentlanguage();
            SOAPLOG Result = new SOAPLOG();
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Kind = 1200;
            Result.SubKind = SoortMelding;
            Result.Status = "G";
            Result.Omschrijving = "";
           
            String lStatus = string.Empty;
            String lOmschrijving = string.Empty;
            String LogDir = unLogger.getLogDir("IenR");
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            int MaxString = 255;
            String lUsername = ""; String lPassword = "";
            int lTestServer;
            UBN lUBN = DB.GetubnById(pUBNid);
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            THIRD lPersoon = DB.GetThirdByThirId(lUBN.ThrID);
            String Beslagnr;
            String PENnr;
            String Inrichtingsnr;
            String Annexnr;
            utils.getSanitraceNummers(lPersoon, lUBN, out Annexnr, out Inrichtingsnr, out Beslagnr, out PENnr);
            PENnr = "";
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9994);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;

            lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
            if (lTestServer > 0)
            {
                lTestServer = 0;
                if (lPersoon.ThrCountry == "125")
                {
                    lTestServer = 10;
                }
            }
            else
            {
                if (lPersoon.ThrCountry == "125")
                {
                    lTestServer = 10;
                }
            }
            string tijd = DateTime.Now.ToString("yyyyMMddHHmmss");
            String LogFile = LogDir + pProgramid.ToString() + "_" + lUBN.Bedrijfsnummer + "_ST_RaadplegenMeldingen_1_" + tijd + ".log";
            String LocalOutputFile = LogDir + pProgramid.ToString() + "_" + lUBN.Bedrijfsnummer + "_ST_RaadplegenMeldingen_Output1_" + tijd + ".csv";
            if (lUsername == "" || lPassword == "" || Inrichtingsnr == "" || Beslagnr == "")
            {
                Result.Date = DateTime.Now.Date;
                Result.Time = DateTime.Now;
                Result.FarmNumber = lUBN.Bedrijfsnummer;
                Result.Omschrijving = VSM_Ruma_OnlineCulture.getStaticResource("userpwordnot", "Inlog gegevens Sanitrace niet ingevuld");
                Result.Status = "F";
                return Result;
            }

            SOAPSANITEL.SanitelMeldingen mld = new SanitelMeldingen();
            mld.STRaadplegenMeldingenAlg(lUsername, lPassword, lTestServer,
                                Taal, pProgId, Inrichtingsnr, Beslagnr,
                                Begindatum, Einddatum,
                                SoortMelding,
                                LocalOutputFile, LogFile,
                                ref lStatus, ref lOmschrijving);

            //Win32SANITRACEALG DLLcall = new Win32SANITRACEALG();
            //DLLcall.STRaadplegenMeldingenAlg(lUsername, lPassword, lTestServer,
            //                    Taal, Inrichtingsnr, Beslagnr,
            //                    Begindatum, Einddatum,
            //                    SoortMelding,
            //                    LocalOutputFile, LogFile,
            //                    ref lStatus, ref lOmschrijving,
            //                    MaxString);
            string[] Kols = { "soortmelding", "meldingsnummer", "Datum", "melddatum" };

            char spl = ';';
            DataTable tblSanitel = utils.GetCsvData(Kols, LocalOutputFile, spl, "Mutaties");
            if (lStatus != "G")
            {
                Result.Status = lStatus;
                Result.Omschrijving = lOmschrijving;
                DB.WriteSoapError(Result);
            }
            if (tblSanitel.Rows.Count > 0)
            {
                //Extra tussenstap om het bijbehorende levensnummer toe te voegen
                //Zie het Sanitrace WordDocument en Win32Sanitracealg.cs
                //Vervangen door VSM.RUMA.SOAPSANITEL
                StreamWriter wrtr = new StreamWriter(OutputFile, false);
                try
                {
                    //"Levensnummer", "soortmelding", "meldingsnummer", "Datum", "melddatum"

                    try
                    {
                        tblSanitel.DefaultView.Sort = "melddatum,meldingsnummer";//Chronologisch volgens RVO
                        tblSanitel = tblSanitel.DefaultView.ToTable(true);
                    }
                    catch { }
                    int[] exceptionmelding = { 1, 2, 6 };
                    int lvolgende = 0;
                    List<SanitelMeldingen.MeldnrAnimal> meldingenMutatieDetails = new List<SanitelMeldingen.MeldnrAnimal>();
                    List<SanitelMeldingen.MeldnrAnimal> meldingenVerplaatsingDetails = new List<SanitelMeldingen.MeldnrAnimal>();
                    foreach (DataRow row in tblSanitel.Rows)
                    {
                        int lSoortMelding = Convert.ToInt32(row["soortmelding"]);
                        string meldingsnummer = row["meldingsnummer"].ToString();
                    
                        var me = new SanitelMeldingen.MeldnrAnimal { meldnr = long.Parse(meldingsnummer), lifenr = "" };
                        if (exceptionmelding.Contains(lSoortMelding))
                        {
                            
                            meldingenMutatieDetails.Add(me);
                        }
                        else
                        {
                            meldingenVerplaatsingDetails.Add(me);
                        }
                    }
                 
                    
                    String LogFileMD = LogDir + pProgramid.ToString() + "_" + lUBN.Bedrijfsnummer + "_ST_MutatieDetails_2_" +  PENnr + "-" + tijd + ".log";
                    if (meldingenMutatieDetails.Count() > 0)
                    {
                        mld.STRaadplegenMutatieDetails(lUsername, lPassword, lTestServer,
                                       Taal, ref meldingenMutatieDetails,
                                       ref lStatus, ref lOmschrijving,
                                       LogFileMD);
                         
                            Result.Status = lStatus;
                            Result.Omschrijving = "MutatieDetails:" + lOmschrijving;
                       
                            DB.WriteSoapError(Result);
                        
                    }
                    String LogFileVD = LogDir + pProgramid.ToString() + "_" + lUBN.Bedrijfsnummer + "_ST_VerplaatsingDetails_2_" + PENnr + "-" + tijd + ".log";
                    if (meldingenVerplaatsingDetails.Count() > 0)
                    {
                        mld.STRaadplegenVerplaatsingDetails(lUsername, lPassword, lTestServer,
                                       Taal, ref meldingenVerplaatsingDetails,
                                       LogFileVD, ref lStatus, ref lOmschrijving);
                        
                            Result.Status = lStatus;
                            Result.Omschrijving = "VerplaatsingDetails:" + lOmschrijving;
                            DB.WriteSoapError(Result);
                        
                    }

                    foreach (DataRow row in tblSanitel.Rows)
                    {
                        lvolgende += 1;
                        string pLevensnr = string.Empty;
                        string pInrichtingsnr = string.Empty;
                        string pBeslagnr = string.Empty;
                        int lSoortMelding = Convert.ToInt32(row["soortmelding"]);
                        string meldingsnummer = row["meldingsnummer"].ToString();
                        long lMeldingsnummer = 0;
                        long.TryParse(meldingsnummer, out lMeldingsnummer);
                        DateTime Datum = utils.getDateLNV(row["Datum"].ToString());
                        DateTime melddatum = utils.getDateLNV(row["melddatum"].ToString());
                        if (exceptionmelding.Contains(lSoortMelding))
                        {
                            int pSoortmelding = 0;
                            DateTime pdatum = new DateTime();
                            String LogFile2 = LogDir + pProgramid.ToString() + "_" + lUBN.Bedrijfsnummer + "_ST_SOAP_Meldingen2_" + lvolgende.ToString() + tijd + ".log";
                            unLogger.WriteInfo(LogFile2);

                            //DLLcall.STRaadplegenMutatieDetails(lUsername, lPassword, lTestServer,
                            //        Taal, lMeldingsnummer, ref pSoortmelding, ref pdatum, ref pLevensnr,
                            //        ref pInrichtingsnr, ref pBeslagnr, ref lStatus, ref lOmschrijving,
                            //        MaxString, LogFile2);

                            pLevensnr = meldingenMutatieDetails.FirstOrDefault(x => x.meldnr == lMeldingsnummer).lifenr;
                            wrtr.WriteLine(pLevensnr + ";" + row["soortmelding"].ToString() + ";" + row["meldingsnummer"].ToString() + ";" + row["Datum"].ToString() + ";" + row["melddatum"].ToString() + ";0");
                        }
                        else
                        {
                            String LogFile3 = LogDir + "ST_SOAP_Meldingen3_" + lvolgende.ToString() + PENnr + "-" + tijd + ".log";
                            unLogger.WriteInfo(LogFile3);
                            String pDetailsFile = LogDir + pProgramid.ToString() + "_" + lUBN.Bedrijfsnummer + "_ST_Details" + lvolgende.ToString() + tijd + ".csv";
                            //DLLcall.STRaadplegenVerplaatsingDetails(lUsername, lPassword, lTestServer,
                            //    Taal, lMeldingsnummer, pDetailsFile, LogFile3, ref lStatus, ref lOmschrijving,
                            //        MaxString);
                            pLevensnr = meldingenVerplaatsingDetails.FirstOrDefault(x => x.meldnr == lMeldingsnummer).lifenr;
                            wrtr.WriteLine(pLevensnr + ";" + row["soortmelding"].ToString() + ";" + row["meldingsnummer"].ToString() + ";" + row["Datum"].ToString() + ";" + row["melddatum"].ToString() + ";0");

                            if (File.Exists(pDetailsFile))
                            {
                                StreamReader rdr = new StreamReader(pDetailsFile);
                                /*
                                        Regel begint met:
                                        1 = Meldingsdetails
                                        2 = Dierdetauls
                                        3 = Verantwoordelijke
                                        4 = Herkomst
                                        5 = Bestemming

                                        1 ; soort ; meldnummer ; datumgebeurtenis ; datumgemeld
                                        2 ; levensnr ; geboortedatum ; Geslacht ; haarkleur ; rastype
                                        3 ; btwnummer ; naam ; straat ; huisnr ; postcode ; plaats ; gemeente ; telefoonnr ; land
                                        4 : btwnummer ; inrichtingsnr ; beslagnr ; naam ; straat ; huisnr ; postcode ; plaats ; gemeente ; telefoonnr ; land
                                        5 : btwnummer ; inrichtingsnr ; beslagnr ; naam ; straat ; huisnr ; postcode ; plaats ; gemeente ; telefoonnr ; land

                                        Meerdere dierregels (nr 2) is mogelijk

                                 */
                                try
                                {
                                    string strLine = rdr.ReadLine();
                                    while (strLine != null)
                                    {
                                        string[] ltemp = strLine.Split(spl);
                                        if (ltemp[0] == "2")
                                        {
                                            wrtr.WriteLine(ltemp[1] + ";" + row["soortmelding"].ToString() + ";" + row["meldingsnummer"].ToString() + ";" + row["Datum"].ToString() + ";" + row["melddatum"].ToString() + ";" + ltemp[3]);

                                        }
                                        strLine = rdr.ReadLine();
                                    }
                                }
                                catch (Exception exc) { }
                                finally { rdr.Close(); }
                            }
                        }

                    }

                }
                catch (Exception exc) { lStatus = "F"; lOmschrijving = exc.Message; }
                finally { wrtr.Close(); }

            }

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Kind = 1200;
            Result.SubKind = SoortMelding;
            Result.Status = lStatus;
            Result.Omschrijving = lOmschrijving;
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            //Result.Lifenumber = lLevensnr;
            //DLLcall.Dispose();
            return Result;
        }

        #endregion

        public override SOAPLOG LNV2MeldingIntrekken(MUTALOG pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken, bool usesoap = false)
        {
            if (!usesoap)
            {
                return LNV2MeldingIntrekken(pRecord, pUBNid, pProgId, pProgramid, mToken);
            }
            string defIenRaction = getdefIenRaction(mToken, pUBNid, pProgId, pProgramid);
            SOAPIRHond IenRHond = new SOAPIRHond();
         
            int MaxString = 255;
            String lUsername, lPassword;
            int lTestServer;
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
            String lBRSnummer = lPersoon.Thr_Brs_Number;
            String lUBNnr = lUBN.Bedrijfsnummer;

            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = pRecord.MeldingNummer;
            SOAPLOG Result = new SOAPLOG();
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, pProgramid, pProgId, 9992);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
           
            String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_Intrekken_" + lUBNnr + "-" + DateTime.Now.Ticks + ".log";
        
            unLogger.WriteInfo(" LogFile:" + LogFile);
            lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
            if (lTestServer > 0)
            {
               
            }

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.FarmNumber = pRecord.Farmnumber;
            if (defIenRaction != "35")//Geen Hond
            {
                if (lUsername == "" && lPassword == "")
                {

                    //TODO make 1 function
                    lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"]; 
                    lPassword = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);  
                    //    uBedrijfsnummer = "";
                    //    uBRSnr = "";

                }
                if (lMeldingsnr == "")
                {
                    lStatus = "F";
                    lOmschrijving = "Geen meldingsnummer bekend, deze is wel verplicht.";
                    Result.Kind = pRecord.CodeMutation;
                    Result.Status = lStatus;
                    Result.Code = lCode;
                    Result.Omschrijving = lOmschrijving;
                    Result.FarmNumber = pRecord.Farmnumber;
                    Result.Lifenumber = pRecord.Lifenumber;
                }
                else
                {

                    MeldingenWS ws = new MeldingenWS();
                    MUTATION mut = ConverttoMutation(pRecord);
                    ws.LNVIntrekkenMelding(mut, lUsername, lPassword, lTestServer,
                             lUBNnr, lBRSnummer, lMeldingsnr,
                             pProgId,
                             ref lStatus, ref lCode, ref lOmschrijving);

                }
            }
            else
            {
                if (lTestServer > 0)
                {
                    //lTestServer = 1;
                    lUsername = "";
                    lPassword = "";
                }
                string pKVKaanleveraar = "";
                //THIRD t = Facade.GetInstance().getSaveToDB(mToken).GetThirdByHouseNrAndZipCode("81", "5836AH");
                //pKVKaanleveraar = t.ThrKvkNummer;
                pKVKaanleveraar = "16085487";
                String CertificaatVingerAfdruk;
                DateTime lRegistratieDatum = pRecord.ReportDate;
                long lMeldingsnummer = long.Parse(pRecord.MeldingNummer);
                DateTime lDatumOntvangstAanleveraar = pRecord.ReportDate;
                long lKenmerkAanleveraar = 1313;
                string pCertificaatnaam = "";
                switch (pProgramid)
                {

                    case 2500://	VBK - Particulier
                    case 2599://	VBK - Administrator
                    case 2550://	VBK - Kennelhouder
                    case 2551://	VBK - Kennelhouder Light
                    case 2570://	VBK - Chipper
                        CertificaatVingerAfdruk = PKIIENRHOND;
                        pCertificaatnaam = "pki.ienrhond.nl";
                        break;
                    case 2501://	Virbac - Particulier
                    case 2571://	Virbac - Chipper
                    case 2598://	Virbac - Adminitrator
                        CertificaatVingerAfdruk = PKIVIRBAC;
                        pCertificaatnaam = "pkivirbac.ienrhond.nl";
                        break;
                    default:
                        switch (pUBNid)
                        {
                            case 109123://	VBK
                                CertificaatVingerAfdruk = PKIIENRHOND;
                                pCertificaatnaam = "pki.ienrhond.nl";
                                break;
                            case 110040://	Virbac 
                                CertificaatVingerAfdruk = PKIVIRBAC;
                                pCertificaatnaam = "pkivirbac.ienrhond.nl";
                                break;
                            default:
                                lStatus = "F";
                                lOmschrijving = "Deze administratie is niet gemachtigd om I&R Hond meldingen te doen";
                                Result.Kind = pRecord.CodeMutation;
                                Result.Status = lStatus;
                                Result.Code = lCode;
                                Result.Omschrijving = lOmschrijving;
                                Result.FarmNumber = pRecord.Farmnumber;
                                Result.Lifenumber = pRecord.Lifenumber;
                                return Result;
                        }
                        break;
                }
                if (ConfigurationManager.AppSettings["CertificaatVingerAfdruk"] != null)
                {
                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CertificaatVingerAfdruk"]))
                    {
                        CertificaatVingerAfdruk = ConfigurationManager.AppSettings["CertificaatVingerAfdruk"];
                    }
                }
                IenRHond.IRHondIntrekmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lMeldingsnummer,
                    lDatumOntvangstAanleveraar, "", lKenmerkAanleveraar,
                    ref lRegistratieDatum, ref lStatus, ref lOmschrijving, ref lMeldingsnr);


            }
            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving + lMeldingsnr;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            pRecord.MeldResult = Result.Omschrijving;

            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {
                if (lMeldingsnr.Trim().Length > 0 && defIenRaction == "35")
                {
                    pRecord.MeldingNummerOrg = pRecord.MeldingNummer;
                    pRecord.MeldingNummer = lMeldingsnr;

                }
                pRecord.Returnresult = 97;
                Facade.GetInstance().getSaveToDB(mToken).MutLogMeldingIntrekken(pRecord);
            }
        
            return Result;
        }

        public override SOAPLOG LNV2MeldingIntrekken(MUTALOG pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken)
        //String pUsername, String pPassword, int pTestServer,
        // String UBNnr, String BRSnr, String MeldingsNr,
        // int pDierSoort,
        // ref String Foutcode, ref String Foutmelding,
        // ref String SoortfoutIndicator, ref String SuccesIndicator,
        // String pLogfile, int pMaxStrLen)
        {
    
            string defIenRaction = getdefIenRaction(mToken, pUBNid, pProgId, pProgramid);
            SOAPIRHond IenRHond = new SOAPIRHond();
            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername, lPassword;
            int lTestServer;
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
            String lBRSnummer = lPersoon.Thr_Brs_Number;
            //if (lBRSnummer == string.Empty)
            //    lBRSnummer = lUBN.BRSnummer;



            String lUBNnr = lUBN.Bedrijfsnummer;

            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            String lMeldingsnr = pRecord.MeldingNummer;
            SOAPLOG Result = new SOAPLOG();
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, pProgramid, pProgId, 9992);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            //lTestServer = configHelper.UseCRDTestserver ? 1 : 0;
            String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_Intrekken_" + lUBNnr + "-" + DateTime.Now.Ticks + ".log";
            //unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP" + lUBNnr + "-" + DateTime.Now.Ticks + ".log");
            unLogger.WriteInfo(" LogFile:" + LogFile);
            lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
            if (lTestServer > 0)
            {
               
            }

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.Lifenumber;
            Result.FarmNumber = pRecord.Farmnumber;
            if (defIenRaction != "35")//Geen Hond
            {
                if (lUsername == "" && lPassword == "")
                {

                    //TODO make 1 function
                    lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"];
                    lPassword = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]); 
                    //    uBedrijfsnummer = "";
                    //    uBRSnr = "";

                }
                if (lMeldingsnr == "")
                {
                    lStatus = "F";
                    lOmschrijving = "Geen meldingsnummer bekend, deze is wel verplicht.";
                    Result.Kind = pRecord.CodeMutation;
                    Result.Status = lStatus;
                    Result.Code = lCode;
                    Result.Omschrijving = lOmschrijving;
                    Result.FarmNumber = pRecord.Farmnumber;
                    Result.Lifenumber = pRecord.Lifenumber;
                }
                else
                {
                
                    DLLcall.LNVIntrekkenMelding(lUsername, lPassword, lTestServer,
                             lUBNnr, lBRSnummer, lMeldingsnr,
                             pProgId,
                             ref lStatus, ref lCode, ref lOmschrijving,
                             LogFile, MaxString);
                }
            }
            else
            {
                if (lTestServer > 0)
                {
                    //lTestServer = 1;
                    lUsername = "";
                    lPassword = "";
                }
                string pKVKaanleveraar = "";
                //THIRD t = Facade.GetInstance().getSaveToDB(mToken).GetThirdByHouseNrAndZipCode("81", "5836AH");
                //pKVKaanleveraar = t.ThrKvkNummer;
                pKVKaanleveraar = "16085487";
                String CertificaatVingerAfdruk;
                DateTime lRegistratieDatum = pRecord.ReportDate;
                long lMeldingsnummer = long.Parse(pRecord.MeldingNummer);
                DateTime lDatumOntvangstAanleveraar = pRecord.ReportDate;
                long lKenmerkAanleveraar = 1313;
                string pCertificaatnaam = "";
                switch (pProgramid)
                {

                    case 2500://	VBK - Particulier
                    case 2599://	VBK - Administrator
                    case 2550://	VBK - Kennelhouder
                    case 2551://	VBK - Kennelhouder Light
                    case 2570://	VBK - Chipper
                        CertificaatVingerAfdruk = PKIIENRHOND;
                        pCertificaatnaam = "pki.ienrhond.nl";
                        break;
                    case 2501://	Virbac - Particulier
                    case 2571://	Virbac - Chipper
                    case 2598://	Virbac - Adminitrator
                        CertificaatVingerAfdruk = PKIVIRBAC;
                        pCertificaatnaam = "pkivirbac.ienrhond.nl";
                        break;
                    default:
                        switch (pUBNid)
                        {
                            case 109123://	VBK
                                CertificaatVingerAfdruk = PKIIENRHOND;
                                pCertificaatnaam = "pki.ienrhond.nl";
                                break;
                            case 110040://	Virbac 
                                CertificaatVingerAfdruk = PKIVIRBAC;
                                pCertificaatnaam = "pkivirbac.ienrhond.nl";
                                break;
                            default:
                                lStatus = "F";
                                lOmschrijving = "Deze administratie is niet gemachtigd om I&R Hond meldingen te doen";
                                Result.Kind = pRecord.CodeMutation;
                                Result.Status = lStatus;
                                Result.Code = lCode;
                                Result.Omschrijving = lOmschrijving;
                                Result.FarmNumber = pRecord.Farmnumber;
                                Result.Lifenumber = pRecord.Lifenumber;
                                return Result;
                        }
                        break;
                }
                if (ConfigurationManager.AppSettings["CertificaatVingerAfdruk"] != null)
                {
                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CertificaatVingerAfdruk"]))
                    {
                        CertificaatVingerAfdruk = ConfigurationManager.AppSettings["CertificaatVingerAfdruk"];
                    }
                }
                IenRHond.IRHondIntrekmelding(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer, lMeldingsnummer,
                    lDatumOntvangstAanleveraar, "", lKenmerkAanleveraar,
                    ref lRegistratieDatum, ref lStatus, ref lOmschrijving, ref lMeldingsnr);


                //DLLcall.IRHondIntrekmelding(lUsername, lPassword, lTestServer,
                //    lMeldingsnummer, lDatumOntvangstAanleveraar,
                //    "Onbekend"
                //    , lKenmerkAanleveraar, pKVKaanleveraar, CertificaatVingerAfdruk, LogFile,
                //    ref lRegistratieDatum, ref lStatus, ref lOmschrijving, ref lMeldingsnr, MaxString);
            }
            Result.Kind = pRecord.CodeMutation;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving + lMeldingsnr;
            Result.FarmNumber = pRecord.Farmnumber;
            Result.Lifenumber = pRecord.Lifenumber;
            pRecord.MeldResult = Result.Omschrijving;

            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {
                if (lMeldingsnr.Trim().Length > 0 && defIenRaction == "35")
                {
                    pRecord.MeldingNummerOrg = pRecord.MeldingNummer;
                    pRecord.MeldingNummer = lMeldingsnr;

                }
                pRecord.Returnresult = 97;
                Facade.GetInstance().getSaveToDB(mToken).MutLogMeldingIntrekken(pRecord);
            }
            DLLcall.Dispose();
            return Result;

        }


        /// <summary>
        /// Soap VSM.RUMA.CORE.SOAPLNV
        /// </summary>
        /// <param name="mToken"></param>
        /// <param name="pUBNid"></param>
        /// <param name="pProgId"></param>
        /// <param name="pProgramid"></param>
        /// <param name="lLevensnr"></param>
        /// <param name="MeldingType"></param>
        /// <param name="MeldingStatus"></param>
        /// <param name="UBNnr2ePartijd"></param>
        /// <param name="Begindatum"></param>
        /// <param name="Einddatum"></param>
        /// <param name="pIndGebeurtenisdatum"></param>
        /// <param name="OutputFile"></param>
        /// <returns></returns>
        public override SOAPLOG LNVIRRaadplegenMeldingenAlg(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                                    String lLevensnr, int MeldingType, int MeldingStatus,
                                                    String UBNnr2ePartijd, DateTime Begindatum, DateTime Einddatum, int pIndGebeurtenisdatum, String OutputFile)
        //String pUsername, String pPassword, int pTestServer,
        // String UBNnr, String BRSnr, String MeldingsNr,
        // int pDierSoort,
        // ref String Foutcode, ref String Foutmelding,
        // ref String SoortfoutIndicator, ref String SuccesIndicator,
        // String pLogfile, int pMaxStrLen)
        {
            //Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            String LogDir = unLogger.getLogDir("IenR");
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(mToken);
            int MaxString = 255;
            String lUsername = ""; String lPassword = "";
            int lTestServer;
            UBN lUBN = DB.GetubnById(pUBNid);
            THIRD lPersoon = DB.GetThirdByThirId(lUBN.ThrID);
            String lBRSnummer = lPersoon.Thr_Brs_Number;
            String lUBNnr = lUBN.Bedrijfsnummer;
            //String lLevensnr = "";
            //int MeldingType = 0;
            //int MeldingStatus = 0;
            //String UBNnr2ePartijd = "";
            //DateTime Begindatum = DateTime.MinValue;
            //DateTime Einddatum = DateTime.MaxValue;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            FTPUSER fusoap = DB.GetFtpuser(pUBNid, pProgramid, pProgId, 9992);

            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)// && LNVPasswordCheck(fusoap.UserName, fusoap.Password) == 1)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }

            if (lUsername == String.Empty && lPassword == String.Empty)
            {
                lUsername = ConfigurationManager.AppSettings["LNVDierDetailsusername"];  
                lPassword = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
            }

            lTestServer = 0;//Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
            String LogFile = LogDir + "LNV2IRSOAP_Meldingen_" + lUBNnr + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(" LogFile:" + LogFile);
            if (string.IsNullOrWhiteSpace(lBRSnummer))
            {
                lBRSnummer = lUBN.BRSnummer;
                if (string.IsNullOrWhiteSpace(lBRSnummer))
                {
                    unLogger.WriteWarn("BRS nummer niet ingevuld: UBN ", lUBNnr);
                }
            }

          
            MeldingenWS dieren = new MeldingenWS();
            dieren.raadplegenMeldingenAlg(lUsername, lPassword, lTestServer, lUBNnr, lBRSnummer,
                pProgId, Begindatum, Einddatum, OutputFile, LogFile, ref lStatus, ref lCode, ref lOmschrijving);


            //DLLcall.LNVIRRaadplegenMeldingenAlgV2(lUsername, lPassword, lTestServer,
            //         lUBNnr, lBRSnummer, lLevensnr,
            //         pProgId, MeldingType, MeldingStatus, UBNnr2ePartijd,
            //         Begindatum, Einddatum, pIndGebeurtenisdatum, 1, 0, OutputFile,
            //         ref lStatus, ref lCode, ref lOmschrijving,
            //         LogFile, MaxString);

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Kind = 1120;
            Result.SubKind = MeldingType;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            Result.Lifenumber = lLevensnr;
            //DLLcall.Dispose();
            return Result;
        }

        public override SOAPLOG IRHondRaadplegenmeldingen(DBConnectionToken mToken, THIRD pRaadpleger, string pIpAdress, int pFarmId, Int64 pChipnummer, DateTime pStartDate, DateTime pEndDate, string pMeldingenFilecsv, string pRelatieFilecsv)
        {

            SOAPLOG Result = new SOAPLOG();
            String CertificaatVingerAfdruk;
            pIpAdress = pIpAdress.Replace(".", "-");
            string lOphaler = pRaadpleger.ThrZipCode.Trim().Replace(" ", "") + pRaadpleger.ThrExt.Trim().Replace(" ", "");
            String lUsername, lPassword;
            int lTestServer;
            int lMeldingType = 99999;
            string pCertificaatnaam = "";
            if (pChipnummer > 0)
            {
                if (pFarmId > 0)
                {
                    SOAPIRHond IenRHond = new SOAPIRHond();
                    AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);
                    BEDRIJF lBedrijf = new BEDRIJF();
                    UBN lUbn = new UBN();
                    THIRD lThird = new THIRD();
                    COUNTRY lCountry = new COUNTRY();
                    lMstb.getCompanyByFarmId(pFarmId, out lBedrijf, out lUbn, out lThird, out lCountry);
                    if (lBedrijf.FarmId > 0 && lUbn.UBNid > 0 && lThird.ThrId > 0 && lCountry.LandId > 0)
                    {

                        string pKVKaanleveraar = "";
                        // Luc 12-01-16 overbodige Third opvragen verwijderd
                        //THIRD t = lMstb.GetThirdByHouseNrAndZipCode("81", "5836AH");
                        //pKVKaanleveraar = t.ThrKvkNummer;

                        // 24-4-2017 omgezet naar SOAPCDD.SOAPIRHond
                        EVENT lChipevent = new EVENT();
                        THIRD lChipper = lMstb.getChipper(pChipnummer.ToString(), out lChipevent);

                        pKVKaanleveraar = "16085487";
                        String lBRSnummer = lThird.Thr_Brs_Number;
                        String lUBNnr = lUbn.Bedrijfsnummer;
                        String lStatus = string.Empty;
                        String lMeldingsnr = string.Empty;
                        String lOmschrijving = string.Empty;
                        int pMaxStrLen = 255;

                        FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(lUbn.UBNid, 9992);
                        if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
                        {

                            lUsername = fusoap.UserName;
                            lPassword = fusoap.Password;
                        }
                        else
                        {
                            lUsername = "";
                            lPassword = "";
                        }
                        String lDatetimeticks = DateTime.Now.Ticks.ToString();
                        lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                        if (lTestServer > 0)// && LNVPasswordCheck(lUsername, lPassword) != 1)
                        {
                            //lTestServer = 1;
                            lUsername = "";
                            lPassword = "";
                        }
                        String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP_HondRaadplegenmeldingen_" + lOphaler + "_" + pIpAdress + "_" + pChipnummer.ToString() + "_" + lDatetimeticks + ".log";

                        unLogger.WriteDebug(unLogger.getLogDir("IenR") + "LNV2IRSOAP_HondRaadplegenmeldingen_" + lOphaler + "_" + pIpAdress + "_" + pChipnummer.ToString() + "_" + lDatetimeticks + ".log");



                        // TODO lMstb.getChipperByChipnr(pChipnummer, out Chipper, out ChipperCountry);
                        string lNaamChipper = pRaadpleger.ThrSecondName;// "";//Chipper.ThrSecondName;
                        string lNaamHouder = pRaadpleger.ThrSecondName;//lThird.ThrSecondName;

                        DateTime lDatumOntvangstAanleveraarLaag = pStartDate;
                        DateTime lDatumOntvangstAanleveraarHoog = pEndDate;
                        DateTime lDatumRegistratieLaag = pStartDate;
                        DateTime lDatumRegistratieHoog = pEndDate;


                        int lDiersoort = 1;
                        int lHuisnrChipper = 1;// 1;// Chipper.ThrExt;     
                        int lHuisnrHouder = 1;//2;// lThird.ThrExt;
                        int lAlleenOpenbaar = 0;
                        int lIngetrokkenOverslaan = 0;
                        Int64 lKenmerkAanleveraarLaag = 1313;
                        Int64 lKenmerkAanleveraarHoog = 1313;
                        string lKvKNummerHouder = "Onbekend";// lThird.ThrKvkNummer;
                        int lMaxAantMeldingen = 0;
                        Int64 lMeldingsnrLaag = 0;
                        Int64 lMeldingsnrHoog = 0;
                        string lPlaatsnaamChipper = "Onbekend";// Chipper.ThrCity;
                        string lPlaatsnaamHouder = "Onbekend";// lThird.ThrCity;
                        string lPostcodeChipper = "Onbekend";//Chipper.ThrZipCode;
                        string lPostcodeHouder = "Onbekend";//lThird.ThrZipCode;

                        Int64 lRegnrAanleveraar = Int64.Parse(pKVKaanleveraar);
                        string lStraatnaamChipper = "Onbekend";//Chipper.ThrStreet1;
                        string lStraatNaamHouder = "Onbekend";//lThird.ThrStreet1;
                        int lTypeMelding = 0;


                        //switch (lBedrijf.Programid)
                        //{

                        //    case 2500://	VBK - Particulier
                        //    case 2599://	VBK - Administrator
                        //    case 2550://	VBK - Kennelhouder
                        //    case 2551://	VBK - Kennelhouder Light
                        //    case 2570://	VBK - Chipper
                        //        CertificaatVingerAfdruk = PKIIENRHOND;
                        //        break;
                        //    case 2501://	Virbac - Particulier
                        //    case 2571://	Virbac - Chipper
                        //    case 2598://	Virbac - Adminitrator
                        //        CertificaatVingerAfdruk = PKIVIRBAC;
                        //        break;
                        //    default:
                        //        CertificaatVingerAfdruk = "";
                        //        lStatus = "F";
                        //        lOmschrijving = "Deze administratie is niet gemachtigd om I&R Hond meldingen te raadplegen";
                        //        break;
                        //}

                        switch (lUbn.UBNid)
                        {
                            case 109123://	VBK
                                CertificaatVingerAfdruk = PKIIENRHOND;
                               
                                pCertificaatnaam = "pki.ienrhond.nl";
                                break;
                            case 110040://	Virbac 
                                CertificaatVingerAfdruk = PKIVIRBAC;
                                pCertificaatnaam = "pkivirbac.ienrhond.nl";
                                break;
                            default:
                                CertificaatVingerAfdruk = "";
                                lStatus = "F";
                                lOmschrijving = "Dit UBN is niet gemachtigd om I&R Hond meldingen te raadplegen";
                                break;
                        }
                        if (ConfigurationManager.AppSettings["CertificaatVingerAfdruk"] != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CertificaatVingerAfdruk"]))
                            {
                                CertificaatVingerAfdruk = ConfigurationManager.AppSettings["CertificaatVingerAfdruk"];
                            }
                        }
                        IenRHond.IRHondRaadplegenmeldingen(CertificaatVingerAfdruk, pKVKaanleveraar, lTestServer,
                            lNaamChipper, lNaamHouder, pChipnummer, pStartDate, pEndDate,
                                         new DateTime(1, 1, 1),
                                         new DateTime(1, 1, 1),
                                         new DateTime(1, 1, 1),
                                         new DateTime(1, 1, 1),
                                         lDiersoort, 0, 0,
                                         lAlleenOpenbaar,
                                         lIngetrokkenOverslaan,
                                         lKenmerkAanleveraarLaag,
                                         lKenmerkAanleveraarHoog,
                                         lKvKNummerHouder,
                                         lMaxAantMeldingen, lMeldingsnrLaag, lMeldingsnrHoog,
                                         lPlaatsnaamChipper, lPlaatsnaamHouder,
                                         lPostcodeChipper, lPostcodeHouder,
                                         lRegnrAanleveraar, lStraatnaamChipper,
                                         lStraatNaamHouder, lMeldingType,
                                         ref lStatus, ref lOmschrijving, ref lMeldingsnr
                                        );

                        //DLLcall.IRHondRaadplegenmeldingen("", "", lTestServer,
                        //                 "16085487",
                        //                 "", "",
                        //                 pChipnummer,
                        //                 pStartDate, pEndDate,
                        //                 new DateTime(1, 1, 1),
                        //                 new DateTime(1, 1, 1),
                        //                 new DateTime(1, 1, 1),
                        //                 new DateTime(1, 1, 1),
                        //                 lDiersoort,
                        //                 0, 0,
                        //                 0, 0,
                        //                 0,
                        //                 0,
                        //                 null,
                        //                 0,
                        //                 0, 0,
                        //                 null, null,
                        //                 null, null,
                        //                 0,
                        //                 null, null,
                        //                 0, CertificaatVingerAfdruk,
                        //                 pMeldingenFilecsv, pRelatieFilecsv, LogFile,
                        //                 ref lStatus, ref lOmschrijving, ref lMeldingsnr,
                        //                 255);

                        Result.Date = DateTime.Today;
                        Result.Time = DateTime.Now;
                        Result.Kind = 1130;
                        Result.Status = lStatus;
                        Result.Code = "";
                        Result.Omschrijving = lOmschrijving;
                        Result.FarmNumber = lUbn.Bedrijfsnummer;
                        Result.Lifenumber = pChipnummer.ToString();
                        //DLLcall.Dispose();
                    }
                    else { Result.Status = "F"; Result.Omschrijving = "Onbekend bedrijf"; }
                }
                else { Result.Status = "F"; Result.Omschrijving = "Onbekend bedrijf"; }
            }
            else { Result.Status = "F"; Result.Omschrijving = "Onbekend Dier"; }

            return Result;
        }

        /// <summary>
        /// Soap VSM.RUMA.CORE.SOAPLNV
        /// </summary>
        /// <param name="mToken"></param>
        /// <param name="pUBNid"></param>
        /// <param name="pProgId"></param>
        /// <param name="pProgramId"></param>
        /// <param name="MeldingNummer"></param>
        /// <param name="MeldingType"></param>
        /// <param name="MeldingStatus"></param>
        /// <param name="pGebeurtenisDatum"></param>
        /// <param name="lLevensnr"></param>
        /// <param name="lLevensnr_Nieuw"></param>
        /// <returns></returns>
        public override SOAPLOG LNVIRRaadplegenMeldingDetailsV2(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramId,
                                             String MeldingNummer, int MeldingType, int MeldingStatus,
                                             DateTime pGebeurtenisDatum, String lLevensnr, String lLevensnr_Nieuw)
        //String pUsername, String pPassword, int pTestServer,
        // String UBNnr, String BRSnr, String MeldingsNr,
        // int pDierSoort,
        // ref String Foutcode, ref String Foutmelding,
        // ref String SoortfoutIndicator, ref String SuccesIndicator,
        // String pLogfile, int pMaxStrLen)
        {


             
            String lUsername = "";
            String lPassword = "";
            int lTestServer;
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
            String lBRSnummer = lPersoon.Thr_Brs_Number;
            String lUBNnr = lUBN.Bedrijfsnummer;

            DateTime pHerstelDatum = DateTime.MinValue;
            DateTime pIntrekDatum = DateTime.MinValue;
            lLevensnr = "";
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;

            SOAPLOG Result = new SOAPLOG();


            //Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, pProgramId, pProgId, 9992);


            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty && LNVPasswordCheck(fusoap.UserName, fusoap.Password) == 1)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }

            lTestServer = 0;//Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
            String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP" + lUBNnr + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP" + lUBNnr + "-" + DateTime.Now.Ticks + ".log");

            MeldingenWS ws = new MeldingenWS();
            MeldingDetails det = new MeldingDetails();
           
            ws.raadplegenMeldingDetail(lUsername, lPassword, lTestServer,
                     lUBNnr, lBRSnummer, MeldingNummer,
                     ref pProgId, ref MeldingType, ref MeldingStatus,
                     ref pGebeurtenisDatum, ref pHerstelDatum, ref pIntrekDatum,
                     ref lLevensnr, ref lLevensnr, ref det,
                     ref lStatus, ref lCode, ref lOmschrijving);

            //DLLcall.LNVIRRaadplegenMeldingDetailsV2(lUsername, lPassword, lTestServer,
            //         lUBNnr, lBRSnummer, MeldingNummer,
            //         ref pProgId, ref MeldingType, ref MeldingStatus,
            //         ref pGebeurtenisDatum, ref pHerstelDatum, ref pIntrekDatum,
            //         ref lLevensnr, ref lLevensnr,
            //         ref lStatus, ref lCode, ref lOmschrijving,
            //         LogFile, MaxString);

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Kind = 1121;
            Result.SubKind = MeldingType;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            Result.Lifenumber = lLevensnr;
            //DLLcall.Dispose();





            return Result;
        }

        public override SOAPLOG LNVIROntbrekendeMeldingenV2(DBConnectionToken mToken, int pUBNid,
                                                    int pProgId, int pProgramid, int MeldingType,
                                                     DateTime Begindatum, DateTime Einddatum,
                                                    ref String csvMeldingType, ref String csvUBNnr2ePartij, ref String csvDatum)
        {
            Win32SOAPIRALG DLLcall = new Win32SOAPIRALG();
            int MaxString = 255;
            String lUsername = "";
            String lPassword = "";
            int lTestServer;
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD lPersoon = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(lUBN.ThrID);
            String lBRSnummer = lPersoon.Thr_Brs_Number;
            String lUBNnr = lUBN.Bedrijfsnummer;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, pProgramid, pProgId, 9992);
            if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty && LNVPasswordCheck(fusoap.UserName, fusoap.Password) == 1)
            {

                lUsername = fusoap.UserName;
                lPassword = fusoap.Password;
            }

            lTestServer = 0;//Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
            String LogFile = unLogger.getLogDir("IenR") + "LNV2IRSOAP" + lUBNnr + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "LNV2IRSOAP" + lUBNnr + "-" + DateTime.Now.Ticks + ".log");
            if (string.IsNullOrWhiteSpace(lBRSnummer))
            {
                lBRSnummer = lUBN.BRSnummer;
                if (string.IsNullOrWhiteSpace(lBRSnummer))
                {
                    unLogger.WriteWarn("BRS nummer niet ingevuld: UBN ", lUBNnr);
                }
            }


            //(String pUsername, String pPassword, int PTestServer,
            //                 String UBNNr, String BRSnr,
            //                 int pDierSoort, int pMeldingType,
            //                 DateTime pBegindatum, DateTime pEinddatum,
            //                 ref String csvMeldingType, ref String csvUBNnr2ePartij, ref  String csvDatum,
            //                 ref String Status, ref String Code, ref String Omschrijving,
            //                 String pLogfile, int pMaxStrLen)

            DLLcall.LNVIROntbrekendeMeldingenV2(lUsername, lPassword, lTestServer,
                     lUBNnr, lBRSnummer,
                     pProgId, MeldingType,
                     Begindatum, Einddatum,
                     ref csvMeldingType, ref csvUBNnr2ePartij, ref csvDatum,
                     ref lStatus, ref lCode, ref lOmschrijving,
                     LogFile, MaxString);

            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Kind = 1122;
            Result.SubKind = MeldingType;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            DLLcall.Dispose();
            return Result;
        }

        public override void MeldIDRnaarGD(List<MUTATION> pRecords, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken)
        {
            try
            {
                //TODO luc IDR melden
                String bestandsnaam;
                maakIDRregels(pRecords, pUBNid, pProgId, out bestandsnaam, pProgramid, mToken);
                unDatacomReader.MoveFiletoDatacomUit(Win32SOAPIRALG.GetBaseDir() + "\\lijsten\\" + bestandsnaam);
            }
            catch (Exception ex)
            {
                unLogger.WriteError("IDR Melden" + ex.Message, ex);
                throw new AccessViolationException("Onbekende Fout bij IDR Melden", ex);
            }
            try
            {
                int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]); ;
                if (lTestServer == 0)
                    FTPversturen(unDatacomReader.DatacomPath, pUBNid, "I*", mToken);
            }
            catch (Exception ex)
            {
                unLogger.WriteError("IDR Versturen" + ex.Message, ex);
            }
        }

        public override void MeldStamboekIDRnaarGD(List<MUTATION> pRecords, string StamBoeknr, int pProgId, DBConnectionToken mToken, out int iOk, out int iError)
        {
            iOk = 0;
            iError = 0;
            try
            {

                String bestandsnaam;
                maakIDRRegelStamBoek(pRecords, StamBoeknr, pProgId, out bestandsnaam, mToken);

                if (unDatacomReader.MoveFiletoDatacomUit(Win32SOAPIRALG.GetBaseDir() + "\\lijsten\\" + bestandsnaam))
                {
                    iError = 0;
                    iOk = 1;
                }
                else
                {
                    iError = 1;
                    iOk = 0;
                }

            }
            catch (Exception ex)
            {
                unLogger.WriteError("IDR Melden" + ex.Message, ex);
                iError = 1;
                iOk = 0;
                throw new AccessViolationException("Onbekende Fout bij IDR Melden", ex);
            }
            try
            {
                int lTestServer = Convert.ToInt32(ConfigurationManager.AppSettings["UseLNVTestserver"]);
                int FarmNr = Convert.ToInt32(StamBoeknr) * -1;
                if (lTestServer == 0)
                {
                    FTPversturen(unDatacomReader.DatacomPath, FarmNr, "I*", mToken);
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError("IDR Versturen" + ex.Message, ex);
                iError = 1;
                iOk = 0;
            }
        }


        #region IDR voorraad

        public override int maakIDRvoorraad(int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken)
        {
            int volgnr = 0;
            string regel;
            string result;
            string idvbestand = "";
            //nedUBN:=iniNederlandsUBN(ubnnr,prog);
            //elecID := iniElectronischeID(ubnnr, prog);
            BEDRIJF bedr = Facade.GetInstance().getSaveToDB(mToken).GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            List<FARMCONFIG> lConfigs = Facade.GetInstance().getSaveToDB(mToken).getFarmConfigs(bedr.FarmId);

            List<ANIMAL> AniLife = new List<ANIMAL>();

            foreach (BEDRIJF farm in Facade.GetInstance().getSaveToDB(mToken).getBedrijvenByUBNId(pUBNid))
            {
                AniLife.AddRange(Facade.GetInstance().getSaveToDB(mToken).GetCurrentAnimalsForFarm(farm.FarmId));
            }

            //List<ANIMAL> AniLife = Facade.GetInstance().getSaveToDB(mToken).
            if (AniLife.Count > 0)
            {


                //if (RUMAlandnr == 2)
                //  idvbestand = outdir +
                //             'G' + opvullen(nedUBN,'0',7,true) + ".IDV";
                //else
                idvbestand = 'G' + opvullen(lUBN.Bedrijfsnummer, '0', 7, true) + ".IDV";
                StreamWriter swbestand = new StreamWriter(Win32SOAPIRALG.GetBaseDir() + "\\lijsten\\" + idvbestand);

                result = idvbestand;
                // Datum-bericht
                String mutdat = DateTime.Now.Year.ToString() +
                        opvullen(DateTime.Now.Month.ToString(), '0', 2, true) +
                        opvullen(DateTime.Now.Day.ToString(), '0', 2, true);
                // Bestandstijd

                //Voorlooprecord
                if (pProgId == 3)
                    regel = "0IDRBELEX    ";
                else
                    regel = "0IDREGAM     ";

                regel = regel + opvullen(lUBN.Bedrijfsnummer, '0', 11, true);
                regel = regel + mutdat +
                       opvullen(DateTime.Now.Hour.ToString(), '0', 2, true) +
                       opvullen(DateTime.Now.Minute.ToString(), '0', 2, true);

                swbestand.WriteLine(regel);

                int aantregels = 0;
                foreach (ANIMAL Animal in AniLife)
                {
                    break;
                    //if (!VSM.RUMA.CORE.SOAPLNV.OpvragenLNVDierstatusV2.DieropBedrijf(Facade.GetInstance(), mToken, bedr, Animal.AniAlternateNumber))
                    //{
                    //    unLogger.WriteInfo("Dier " + Animal.AniAlternateNumber + " staat niet op dit Bedrijf!");
                    //    continue;
                    //}

                    aantregels++;
                    volgnr++;



                    regel = "1" + //recordsoort
                           opvullen(Convert.ToString(volgnr), ' ', 5, true) + //volgnr
                           "7" + //meldreden (voorraad)
                           mutdat; //mutatiedatum (vandaag)

                    regel = regel + opvullen(lUBN.Bedrijfsnummer, '0', 7, true); //ubn herkomst
                    regel = regel + opvullen("", ' ', 7, true); //ubn bestemming

                    //uniek levensnummer
                    regel = regel + DelphiWrapper.ISOlandcode(copy(Animal.AniAlternateNumber, 0, 3)) +
                    opvullen(copy(Animal.AniAlternateNumber, 3, 12), ' ', 12, false);  //uniek levnr

                    //alternatief levensnummer
                    if (pProgId == 3)
                        //schapen GEEN alternatief nummer meer doorsturen
                        regel = regel + opvullen("", ' ', 15, false);
                    else
                    {

                        if (Animal.AniLifeNumber != string.Empty)
                            regel = regel + DelphiWrapper.ISOlandcode(copy(Animal.AniLifeNumber, 0, 3)) +
                            opvullen(copy(Animal.AniLifeNumber, 3, 12), ' ', 12, false); //alternatief
                        else
                            regel = regel + DelphiWrapper.ISOlandcode(copy(Animal.AniAlternateNumber, 0, 3)) +
                            opvullen(copy(Animal.AniAlternateNumber, 3, 12), ' ', 12, false); //alternatief
                    }


                    //soort alternatief nummer
                    if (copy(Animal.AniAlternateNumber, 0, 2) == "NL")
                        regel = regel + "FK";
                    else
                        regel = regel + "BU";

                    //geboortedatum
                    regel = regel + Animal.AniBirthDate.Year.ToString() +
                           opvullen(Animal.AniBirthDate.Month.ToString(), '0', 2, true) +
                           opvullen(Animal.AniBirthDate.Day.ToString(), '0', 2, true) +
                           opvullen("", ' ', 8, true); //sterftedatum

                    switch (Animal.AniSex)
                    {
                        case 1:
                            regel = regel + 'M';
                            break;
                        case 2:
                            regel = regel + 'V';
                            break;
                        default:
                            regel = regel + 'V';
                            break;
                    }

                    if (pProgId == 3)  //schapen
                        regel = regel + '1'; //schaap
                    else
                        regel = regel + '2'; //geit                                                     

                    string IDRRace = BepaalRas(bedr, Animal.AniAlternateNumber, mToken);
                    if (IDRRace.Length < 1) IDRRace = "NN8";

                    ANIMAL AniMother = Facade.GetInstance().getSaveToDB(mToken).GetAnimalById(Animal.AniIdMother);
                    ANIMAL AniFather = Facade.GetInstance().getSaveToDB(mToken).GetAnimalById(Animal.AniIdFather);
                    try
                    {
                        regel = regel + ' ' + //diercategorie

                                                           opvullen(IDRRace, ' ', 24, false) +
                            DelphiWrapper.ISOlandcode(copy(AniMother.AniAlternateNumber, 0, 3)) +
                            opvullen(copy(AniMother.AniAlternateNumber, 3, 12), ' ', 12, false) +
                            DelphiWrapper.ISOlandcode(copy(AniFather.AniAlternateNumber, 0, 3)) +
                            opvullen(copy(AniFather.AniAlternateNumber, 3, 12), ' ', 12, false) +
                           opvullen("", ' ', 5, true); //aantal
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError("IDR melden Fout Vader: " + AniFather.AniAlternateNumber
                            + " Moeder: " + AniMother.AniAlternateNumber, ex);
                        throw ex;
                    }
                    regel = regel + opvullen("", ' ', 5, true); //aantal

                    swbestand.WriteLine(regel);


                }//foreach

                //afsluitrecord
                regel = '9' + opvullen(Convert.ToString(aantregels + 2), ' ', 5, true);
                swbestand.WriteLine(regel);

                swbestand.Close();

            }

            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);
            lMstb.SetFarmConfigValue(bedr.FarmId, "ExternNr", volgnr.ToString());
            return volgnr;
        }

        private string BepaalRas(BEDRIJF lFarm, String AniLifeNumber, DBConnectionToken mToken)
        {
            String Result = "";

            StringBuilder QRY_Ras = new StringBuilder();
            QRY_Ras.Append(" SELECT AniLifeNumber, agrofactuur.LABELS.LabLabel, SECONRAC.SraRate ");
            QRY_Ras.Append(" FROM ANIMAL ");
            QRY_Ras.Append(" JOIN SECONRAC  ");
            QRY_Ras.Append(" ON ANIMAL.AniId = SECONRAC.AniId ");
            QRY_Ras.Append(" JOIN agrofactuur.LABELS ");
            QRY_Ras.Append(" ON SECONRAC.RacId = agrofactuur.LABELS.LabId ");
            QRY_Ras.Append(" AND agrofactuur.LABELS.LabKind = 35 ");
            QRY_Ras.Append(" AND agrofactuur.LABELS.LabCountry = 528 ");
            //QRY_Ras.AppendFormat(" WHERE AniLifeNumber = '{0}' ", AniLifeNumber);
            QRY_Ras.AppendFormat(" WHERE ANIMAL.AniAlternateNumber = '{0}' ", AniLifeNumber);
            if (AniLifeNumber != String.Empty)
            {
                if (lFarm.Programid != 0) //
                {
                    DataTable dtRas = Facade.GetInstance().getSaveToDB(mToken).GetDataBase().QueryData(mToken.getChildConnection(lFarm.Programid), QRY_Ras);

                    foreach (DataRow row in dtRas.Rows)
                    {
                        Result += row["LabLabel"].ToString().Substring(0, 2) + row["SraRate"].ToString();
                    }
                }
            }
            return Result;
        }

        #endregion

        public int maakIDRregels(List<MUTATION> Records, int pUBNid, int pProgId, out string bestandsnaam, int pProgramid, DBConnectionToken mToken)
        {
            string regel;
            BEDRIJF bedr = Facade.GetInstance().getSaveToDB(mToken).GetBedrijfByUbnIdProgIdProgramid(pUBNid, pProgId, pProgramid);
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            List<FARMCONFIG> lConfigs = Facade.GetInstance().getSaveToDB(mToken).getFarmConfigs(bedr.FarmId);

            int extteller = 0;
            var cfgextteller = from iniextteller in lConfigs
                               where iniextteller.FKey == "ExtensieNr"
                               select iniextteller;
            if (cfgextteller.Count() == 1) extteller = cfgextteller.First().ValueAsInteger();
            if (extteller == 99) extteller = 0;
            extteller++;
            int volgnr = 0;
            var cfgvolgnr = from inivolgnr in lConfigs
                            where inivolgnr.FKey == "ExternNr"
                            select inivolgnr;
            if (cfgvolgnr.Count() == 1) volgnr = cfgvolgnr.First().ValueAsInteger();
            int RUMAlandnr = 1;
            var cfgRUMAlandnr = from inilandnr in lConfigs
                                where inilandnr.FKey == "Country"
                                select inilandnr;
            if (cfgRUMAlandnr.Count() == 1) RUMAlandnr = cfgRUMAlandnr.First().ValueAsInteger();

            string nedUBN = string.Empty;
            var cfgnedUBN = from ininedubn in lConfigs
                            where ininedubn.FKey == "NederlandsUBN"
                            select ininedubn;
            if (cfgnedUBN.Count() == 1) nedUBN = cfgnedUBN.First().ValueAsString();


            //IDR bestand
            if (RUMAlandnr == 2)
                bestandsnaam = 'G' + opvullen(nedUBN, '0', 7, true) + ".I" +
                              opvullen(Convert.ToString(extteller), '0', 2, true);
            else
                bestandsnaam = 'G' + opvullen(lUBN.Bedrijfsnummer, '0', 7, true) + ".I" +
                              opvullen(Convert.ToString(extteller), '0', 2, true);

            StreamWriter bestand = new StreamWriter(Win32SOAPIRALG.GetBaseDir() + "\\lijsten\\" + bestandsnaam);


            if (pProgId == 3)
                regel = "0IDRBELEX    ";
            else
                regel = "0IDREGAM     ";
            //josser 14-10-2004: rijnlam uitschakelen
            //if        ((prog=5) and (checkprotection('amalthea', true)=255)) then //relatienr ELDA invullen
            //  regel:=regel + opvullen('3350625', '0', 11, true)
            //else 
            if (pProgId == 5 && RUMAlandnr == 2)
                regel = regel + opvullen(nedUBN, '0', 11, true);
            else
                regel = regel + opvullen(lUBN.Bedrijfsnummer, '0', 11, true);
            regel = regel + DateTime.Now.Year +
                   opvullen(DateTime.Now.Month.ToString(), '0', 2, true) +
                   opvullen(DateTime.Now.Day.ToString(), '0', 2, true) +
                   opvullen(DateTime.Now.Hour.ToString(), '0', 2, true) +
                   opvullen(DateTime.Now.Minute.ToString(), '0', 2, true);
            bestand.WriteLine(regel);
            int aantregels = 0;


            for (int i = 1; i <= 6; i++)
            {
                //datarecords
                int meldreden = 0;
                int IDRreden = 0;
                switch (i)
                {
                    case 1:
                        meldreden = 2; //geboorte
                        IDRreden = 5;
                        break;
                    case 2:
                        meldreden = 1; //aanvoer
                        IDRreden = 2;
                        break;
                    case 3:
                        meldreden = 7; //import
                        IDRreden = 3;
                        break;
                    case 4:
                        meldreden = 4; //afvoer + 5 = ikb afvoer + 10 = slacht
                        IDRreden = 1;
                        break;
                    case 5:
                        meldreden = 9; //export
                        IDRreden = 4;
                        break;
                    case 6:
                        meldreden = 6; //dood
                        IDRreden = 6;
                        break;
                }

                foreach (MUTATION Record in Records)
                {

                    if ((Record.CodeMutation == meldreden) ||
                    (meldreden == 4 && (Record.CodeMutation == 5 || Record.CodeMutation == 10)))
                    {
                        switch (Record.CodeMutation)
                        {
                            case 2:
                            case 1:
                            case 7:
                                if (Record.FarmNumberTo == "") Record.FarmNumberTo = lUBN.Bedrijfsnummer;
                                break;
                            case 4:
                            case 5:
                            case 10:
                            case 9:
                            case 6:
                                if (Record.FarmNumberFrom == "") Record.FarmNumberFrom = lUBN.Bedrijfsnummer;
                                break;
                        }
                        volgnr++;
                        if (volgnr > 99999)
                            volgnr = 1;
                        aantregels++;
                        regel = '1' +  //recordsoort
                               opvullen(Convert.ToString(volgnr), ' ', 5, true) + //volgnr
                               Convert.ToString(IDRreden) + //reden
                               Record.MutationDate.Year.ToString() +
                               opvullen(Record.MutationDate.Month.ToString(), '0', 2, true) +
                               opvullen(Record.MutationDate.Day.ToString(), '0', 2, true) + //datum
                               opvullen(Record.FarmNumberFrom, ' ', 7, true) +
                               opvullen(Record.FarmNumberTo, ' ', 7, true);
                        try
                        {
                            regel = regel + DelphiWrapper.ISOlandcode(copy(Record.Lifenumber, 0, 3)) +
                                   opvullen(copy(Record.Lifenumber, 3, 12), ' ', 12, false);  //uniek levnr
                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteError("IDR melden Fout Levensnummer : " + Record.Lifenumber, ex);
                            throw ex;
                        }
                        //alternatief levensnummer
                        if (pProgId == 3)
                            //schapen GEEN alternatief nummer meer doorsturen
                            regel = regel + opvullen("", ' ', 15, false);
                        else
                        {

                            if (Record.AlternateLifeNumber != string.Empty)
                                regel = regel + DelphiWrapper.ISOlandcode(copy(Record.AlternateLifeNumber, 0, 3)) +
                                opvullen(copy(Record.AlternateLifeNumber, 3, 12), ' ', 12, false); //alternatief
                            else
                                regel = regel + DelphiWrapper.ISOlandcode(copy(Record.Lifenumber, 0, 3)) +
                                opvullen(copy(Record.Lifenumber, 3, 12), ' ', 12, false); //alternatief
                        }

                        //soort alternatief levensnummer
                        //if (pProgId == 3)
                        //    regel = regel + "  ";
                        //else 
                        if (copy(Record.Lifenumber, 0, 2) == "NL")
                            regel = regel + "FK";
                        else
                            regel = regel + "BU";

                        if (Record.IDRBirthDate != DateTime.MinValue)
                        {
                            regel = regel + Record.IDRBirthDate.Year.ToString() +
                                   opvullen(Record.IDRBirthDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.IDRBirthDate.Day.ToString(), '0', 2, true); //geboortedatum
                        }
                        else if (Record.MutationDate != DateTime.MinValue && (meldreden == 2))// || meldreden == 1 || meldreden == 7))
                        {
                            regel = regel + Record.MutationDate.Year.ToString() +
                                   opvullen(Record.MutationDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.MutationDate.Day.ToString(), '0', 2, true); //geboortedatum
                        }
                        else
                            regel = regel + opvullen("", ' ', 8, true);

                        if (Record.IDRLossDate != DateTime.MinValue)
                        {
                            regel = regel + Record.IDRLossDate.Year.ToString() +
                                   opvullen(Record.IDRLossDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.IDRLossDate.Day.ToString(), '0', 2, true); //sterftedatum
                        }
                        else if (Record.MutationDate != DateTime.MinValue && meldreden == 6)
                        // (meldreden == 4 || meldreden == 6 || meldreden == 9))
                        {
                            regel = regel + Record.MutationDate.Year.ToString() +
                                   opvullen(Record.MutationDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.MutationDate.Day.ToString(), '0', 2, true);  //sterftedatum
                        }
                        else
                            regel = regel + opvullen("", ' ', 8, true);

                        switch (Record.Sex)
                        {
                            case 1:
                                regel = regel + 'M';
                                break;
                            case 2:
                                regel = regel + 'V';
                                break;
                            default:
                                regel = regel + 'V';
                                break;
                        }

                        if (pProgId == 3) //schapen
                            regel = regel + "1"; //schaap
                        else
                            regel = regel + "2"; //geit

                        string IDRRace;
                        if (Record.IDRRace.Length > 1) IDRRace = Record.IDRRace;
                        else IDRRace = "NN8";
                        try
                        {
                            regel = regel + ' ' + //diercategorie
                               opvullen(IDRRace, ' ', 24, false) +
                                DelphiWrapper.ISOlandcode(copy(Record.LifenumberMother, 0, 3)) +
                                opvullen(copy(Record.LifenumberMother, 3, 12), ' ', 12, false) +
                                DelphiWrapper.ISOlandcode(copy(Record.LifeNumberFather, 0, 3)) +
                                opvullen(copy(Record.LifeNumberFather, 3, 12), ' ', 12, false) +
                               opvullen("", ' ', 5, true); //aantal
                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteError("IDR melden Fout Vader: " + Record.LifeNumberFather
                                + " Moeder: " + Record.LifenumberMother, ex);
                            throw ex;
                        }
                        bestand.WriteLine(regel);

                    }
                }
            }

            //afsluitrecord
            regel = '9' + opvullen(Convert.ToString(aantregels + 2), ' ', 5, true);
            bestand.WriteLine(regel);
            bestand.Close();

            /*
            //OUD
            FARMCONFIG fcextteller = Facade.GetInstance().getSaveToDB(mToken).getFarmConfig(bedr.FarmId, "ExtensieNr", "0");
            fcextteller.ValueAsInteger(extteller);
            SaveFarmConfig(bedr.FarmId, "ExtensieNr", fcextteller, mToken);

            FARMCONFIG fcvolgnr = Facade.GetInstance().getSaveToDB(mToken).getFarmConfig(bedr.FarmId, "ExternNr", "0");
            fcvolgnr.ValueAsInteger(volgnr);
            SaveFarmConfig(bedr.FarmId, "ExternNr", fcvolgnr, mToken);
            */

            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);
            lMstb.SetFarmConfigValue(bedr.FarmId, "ExtensieNr", extteller.ToString());
            lMstb.SetFarmConfigValue(bedr.FarmId, "ExternNr", volgnr.ToString());

            return volgnr;
        }


        public int maakIDRRegelStamBoek(List<MUTATION> Records, string StamBoeknr, int pProgId, out string bestandsnaam, DBConnectionToken mToken)
        {
            string regel;

            int FarmNr = Convert.ToInt32(StamBoeknr) * -1;

            List<FARMCONFIG> lConfigs = Facade.GetInstance().getSaveToDB(mToken).getFarmConfigs(FarmNr);

            int extteller = 0;
            var cfgextteller = from iniextteller in lConfigs
                               where iniextteller.FKey == "ExtensieNr"
                               select iniextteller;
            if (cfgextteller.Count() == 1) extteller = cfgextteller.First().ValueAsInteger();
            if (extteller == 99) extteller = 0;
            extteller++;
            int volgnr = 0;
            var cfgvolgnr = from inivolgnr in lConfigs
                            where inivolgnr.FKey == "ExternNr"
                            select inivolgnr;

            //IDR bestand
            bestandsnaam = 'G' + opvullen(StamBoeknr, '0', 7, true) + ".I" +
                          opvullen(Convert.ToString(extteller), '0', 2, true);

            string folder = Path.Combine(Win32SOAPIRALG.GetBaseDir(), "lijsten");
            string folderBck = Path.Combine(Win32SOAPIRALG.GetBaseDir(), "Bck");
            DirectoryInfo dir = new DirectoryInfo(folder);
            if (!dir.Exists)
            {
                try
                {
                    dir.Create();
                }
                catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
            }
            dir = new DirectoryInfo(folderBck);
            if (!dir.Exists)
            {
                try
                {
                    dir.Create();
                }
                catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
            }
            StreamWriter bestand = new StreamWriter(Path.Combine(folder, bestandsnaam));


            if (pProgId == 3)
                regel = "0IDRBELEX    ";
            else
                regel = "0IDREGAM     ";

            regel = regel + opvullen(StamBoeknr, '0', 11, true);
            regel = regel + DateTime.Now.Year +
                   opvullen(DateTime.Now.Month.ToString(), '0', 2, true) +
                   opvullen(DateTime.Now.Day.ToString(), '0', 2, true) +
                   opvullen(DateTime.Now.Hour.ToString(), '0', 2, true) +
                   opvullen(DateTime.Now.Minute.ToString(), '0', 2, true);
            bestand.WriteLine(regel);
            int aantregels = 0;


            for (int i = 1; i <= 6; i++)
            {
                //datarecords
                int meldreden = 0;
                int IDRreden = 0;
                switch (i)
                {
                    case 1:
                        meldreden = 2; //geboorte
                        IDRreden = 5;
                        break;
                    case 2:
                        meldreden = 1; //aanvoer
                        IDRreden = 2;
                        break;
                    case 3:
                        meldreden = 7; //import
                        IDRreden = 3;
                        break;
                    case 4:
                        meldreden = 4; //afvoer + 5 = ikb afvoer + 10 = slacht
                        IDRreden = 1;
                        break;
                    case 5:
                        meldreden = 9; //export
                        IDRreden = 4;
                        break;
                    case 6:
                        meldreden = 6; //dood
                        IDRreden = 6;
                        break;
                }

                foreach (MUTATION Record in Records)
                {

                    if ((Record.CodeMutation == meldreden) ||
                    (meldreden == 4 && (Record.CodeMutation == 5 || Record.CodeMutation == 10)))
                    {
                        switch (Record.CodeMutation)
                        {
                            case 2:
                            case 1:
                            case 7:
                                if (Record.FarmNumberTo == "") Record.FarmNumberTo = Record.Farmnumber;
                                break;
                            case 4:
                            case 5:
                            case 10:
                            case 9:
                            case 6:
                                if (Record.FarmNumberFrom == "") Record.FarmNumberFrom = Record.Farmnumber;
                                break;
                        }
                        volgnr++;
                        if (volgnr > 99999)
                            volgnr = 1;
                        aantregels++;
                        regel = '1' +  //recordsoort
                               opvullen(Convert.ToString(volgnr), ' ', 5, true) + //volgnr
                               Convert.ToString(IDRreden) + //reden
                               Record.MutationDate.Year.ToString() +
                               opvullen(Record.MutationDate.Month.ToString(), '0', 2, true) +
                               opvullen(Record.MutationDate.Day.ToString(), '0', 2, true) + //datum
                               opvullen(Record.FarmNumberFrom, ' ', 7, true) +
                               opvullen(Record.FarmNumberTo, ' ', 7, true);
                        try
                        {
                            regel = regel + DelphiWrapper.ISOlandcode(copy(Record.Lifenumber, 0, 3)) +
                                   opvullen(copy(Record.Lifenumber, 3, 12), ' ', 12, false);  //uniek levnr
                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteError("IDR melden Fout Levensnummer : " + Record.Lifenumber, ex);
                            throw ex;
                        }
                        //alternatief levensnummer
                        if (pProgId == 3)
                            //schapen GEEN alternatief nummer meer doorsturen
                            regel = regel + opvullen("", ' ', 15, false);
                        else
                        {

                            if (Record.AlternateLifeNumber != string.Empty)
                                regel = regel + DelphiWrapper.ISOlandcode(copy(Record.AlternateLifeNumber, 0, 3)) +
                                opvullen(copy(Record.AlternateLifeNumber, 3, 12), ' ', 12, false); //alternatief
                            else
                                regel = regel + DelphiWrapper.ISOlandcode(copy(Record.Lifenumber, 0, 3)) +
                                opvullen(copy(Record.Lifenumber, 3, 12), ' ', 12, false); //alternatief
                        }

                        //soort alternatief levensnummer
                        //if (pProgId == 3)
                        //    regel = regel + "  ";
                        //else 
                        if (copy(Record.Lifenumber, 0, 2) == "NL")
                            regel = regel + "FK";
                        else
                            regel = regel + "BU";

                        if (Record.IDRBirthDate != DateTime.MinValue)
                        {
                            regel = regel + Record.IDRBirthDate.Year.ToString() +
                                   opvullen(Record.IDRBirthDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.IDRBirthDate.Day.ToString(), '0', 2, true); //geboortedatum
                        }
                        else if (Record.MutationDate != DateTime.MinValue && (meldreden == 2))// || meldreden == 1 || meldreden == 7))
                        {
                            regel = regel + Record.MutationDate.Year.ToString() +
                                   opvullen(Record.MutationDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.MutationDate.Day.ToString(), '0', 2, true); //geboortedatum
                        }
                        else
                            regel = regel + opvullen("", ' ', 8, true);

                        if (Record.IDRLossDate != DateTime.MinValue)
                        {
                            regel = regel + Record.IDRLossDate.Year.ToString() +
                                   opvullen(Record.IDRLossDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.IDRLossDate.Day.ToString(), '0', 2, true); //sterftedatum
                        }
                        else if (Record.MutationDate != DateTime.MinValue && meldreden == 6)
                        //(meldreden == 4 || meldreden == 6 || meldreden == 9))
                        {
                            regel = regel + Record.MutationDate.Year.ToString() +
                                   opvullen(Record.MutationDate.Month.ToString(), '0', 2, true) +
                                   opvullen(Record.MutationDate.Day.ToString(), '0', 2, true);  //sterftedatum
                        }
                        else
                            regel = regel + opvullen("", ' ', 8, true);

                        switch (Record.Sex)
                        {
                            case 1:
                                regel = regel + 'M';
                                break;
                            case 2:
                                regel = regel + 'V';
                                break;
                            default:
                                regel = regel + 'V';
                                break;
                        }

                        if (pProgId == 3) //schapen
                            regel = regel + "1"; //schaap
                        else
                            regel = regel + "2"; //geit

                        string IDRRace;
                        if (Record.IDRRace.Length > 1) IDRRace = Record.IDRRace;
                        else IDRRace = "NN8";
                        try
                        {
                            regel = regel + ' ' + //diercategorie
                               opvullen(IDRRace, ' ', 24, false) +
                                DelphiWrapper.ISOlandcode(copy(Record.LifenumberMother, 0, 3)) +
                                opvullen(copy(Record.LifenumberMother, 3, 12), ' ', 12, false) +
                                DelphiWrapper.ISOlandcode(copy(Record.LifeNumberFather, 0, 3)) +
                                opvullen(copy(Record.LifeNumberFather, 3, 12), ' ', 12, false) +
                               opvullen("", ' ', 5, true); //aantal
                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteError("IDR melden Fout Vader: " + Record.LifeNumberFather
                                + " Moeder: " + Record.LifenumberMother, ex);
                            throw ex;
                        }
                        bestand.WriteLine(regel);

                    }
                }
            }

            //afsluitrecord
            regel = '9' + opvullen(Convert.ToString(aantregels + 2), ' ', 5, true);
            bestand.WriteLine(regel);
            bestand.Close();
            if (File.Exists(Path.Combine(folder, bestandsnaam)))
            {
                try
                {
                    File.Copy(Path.Combine(folder, bestandsnaam), Path.Combine(folderBck, bestandsnaam + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bck"));
                }
                catch (Exception exc)
                {
                    unLogger.WriteError(exc.ToString());
                }
            }
            /* 
            //OUD             
            FARMCONFIG fcextteller = Facade.GetInstance().getSaveToDB(mToken).getFarmConfig(FarmNr, "ExtensieNr", "0");
            fcextteller.ValueAsInteger(extteller);
            SaveFarmConfig(FarmNr, "ExtensieNr", fcextteller, mToken);

            FARMCONFIG fcvolgnr = Facade.GetInstance().getSaveToDB(mToken).getFarmConfig(FarmNr, "ExternNr", "0");
            fcvolgnr.ValueAsInteger(volgnr);
            SaveFarmConfig(FarmNr, "ExternNr", fcvolgnr, mToken);
            */

            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(mToken);
            lMstb.SetFarmConfigValue(FarmNr, "ExtensieNr", extteller.ToString());
            lMstb.SetFarmConfigValue(FarmNr, "ExternNr", volgnr.ToString());

            return volgnr;
        }

        /*
        //OUD
        private void SaveFarmConfig(int pFarmId, string pKey, FARMCONFIG pFarmConfig, DBConnectionToken mToken)
        {
            if (!Facade.GetInstance().getSaveToDB(mToken).isFilledByDb(pFarmConfig))
            {
                pFarmConfig.FarmId = pFarmId;
                pFarmConfig.FKey = pKey;
                Facade.GetInstance().getSaveToDB(mToken).InsertFarmConfig(pFarmConfig);
            }
            else
                Facade.GetInstance().getSaveToDB(mToken).UpdateFarmConfig(pFarmConfig);
        }
        */


        public string copy(string s, int start, int length)
        {
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (start >= 0 && (start + length) < s.Length)
                        return s.Substring(start, length);
                    else if (start >= 0)
                        return s.Substring(start);
                    else
                        return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug("Copy functie '" + s + "' ", ex);
                return string.Empty;
            }
        }


        public string opvullen(string s, char kar, int lengte, bool vooraan)
        {
            while (lengte > s.Length)
            {
                if (vooraan) s = kar + s;
                else s = s + kar;
            }
            return s;
        }


        private static void FTPversturen(String pDataComPath, int pUBNid, String pExtention, DBConnectionToken mToken)
        {
            List<FTPINFO> FtpInfoList = Facade.GetInstance().getSaveToDB(mToken).GetFTPINFO(pUBNid);
            var FtpInfoext = from FtpInfo in FtpInfoList
                             where FtpInfo.UseExtention == pExtention
                             select FtpInfo;

            if (FtpInfoext.Count() != 0)
            {
                FTPINFO lFtpInfo = FtpInfoext.First();
                if (lFtpInfo.FtpHostName != String.Empty)
                {
                    FTP FTPnrs = new FTP(lFtpInfo.FtpHostName, lFtpInfo.UserName, lFtpInfo.Password);
                    if (lFtpInfo.Direction == 1)
                    {
                        FTPnrs.UploadtoDir(pDataComPath + lFtpInfo.DirectoryFrom, lFtpInfo.DirectoryTo, lFtpInfo.AfterTransfer);
                    }
                }
            }
        }

        public override String Plugin()
        {
            return "RUMA";
        }

        public override List<IRreportresult> ZetDHZMeldingenKlaar(int pUBNId, DBConnectionToken mToken)
        {
            return new List<IRreportresult>();
        }
    }
}
