using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;


namespace VSM.RUMA.CORE
{
    public class Voersupply
    {
        /*
         * klasse om voor voerleveranciers uit te rekenen wat ze geleverd hebben
            en om dit te verzenden naar RVO via webservice
         */
        private static AFSavetoDB getMysqlDb(UserRightsToken pUr)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            return lMstb;
        }

        public List<MUTA_VOER> berekenleveranties(UserRightsToken pUr, int pThrId,int pYear)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            THIRD levraarTHIRD = lMstb.GetThirdByThirId(pThrId);
            DateTime jaar = new DateTime(pYear,1,1);

            int ervoor = pYear - 1;
            int erna = pYear + 1;

            StringBuilder sQsuppArtikelen = new StringBuilder();
            sQsuppArtikelen.Append(" SELECT ARTIKEL_VOER.MinasCategory as Diersoort, SUPPLY1.*, ARTIKEL.*  FROM SUPPLY1");
            sQsuppArtikelen.Append(" INNER JOIN FACTUUR ON SUPPLY1.Factid = FACTUUR.FactId");
            sQsuppArtikelen.Append(" INNER JOIN ARTIKEL ON SUPPLY1.Artikelid = ARTIKEL.ArtId");
            sQsuppArtikelen.Append(" INNER JOIN ARTIKEL_VOER ON ARTIKEL_VOER.artid = ARTIKEL.ArtId");
            sQsuppArtikelen.Append(" WHERE FACTUUR.FactRelatieId = " + pThrId.ToString() );
            sQsuppArtikelen.Append(" AND date_format(FactDatum,'%Y') > '" + ervoor.ToString() + "'  AND  date_format(FactDatum,'%Y') < '" + erna.ToString() + "' ");
            sQsuppArtikelen.Append(" ORDER BY Diersoort");//perdiersoort
            DataTable suppArtikelen = lMstb.GetDataBase().QueryData(pUr, sQsuppArtikelen);

            List<FACTUUR> factList = lMstb.getLeverancierFactuurlist(pThrId, pYear);
            //per voerleverancier per diersoort
         
            List<int> derden = new List<int>();
            foreach (FACTUUR fv in factList)
            {
                if (!derden.Contains(fv.ThrID))
                {
                    if (fv.ThrID > 0)
                    {
                        derden.Add(fv.ThrID);
                    }
                    else { unLogger.WriteInfo("FACTUUR:" + fv.FactId.ToString() + " heeft geen ThirdId"); }
                }
            }

            List<MUTA_VOER> TemplevList = new List<MUTA_VOER>();

            int DiersoortLabid = 0;
      
            float Totaal = 0;
            float NormRE = 0;
            float NormP = 0;

            if (factList.Count() > 0)
            {

                foreach (int lThrId in derden)
                {
                    var FacturenDerden = from n in factList
                                         where n.ThrID == lThrId
                                         select n;
                    THIRD afNemerThrd = lMstb.GetThirdByThirId(lThrId);

                    List<int> factuurids = new List<int>();
                    foreach (FACTUUR fade in FacturenDerden)
                    {
                        if (!factuurids.Contains(fade.FactId))
                        {
                            factuurids.Add(fade.FactId);
                        }
                    }
                    DataRow[] foundRows;
                    foundRows = suppArtikelen.Select("Factid in (" + lMstb.intListToString(factuurids) + ")");
                    Array.Sort(foundRows, delegate(DataRow a, DataRow b) { return String.Compare(a["Diersoort"].ToString(), b["Diersoort"].ToString()); });

                    int diersoort = 0;
                    int teller = foundRows.Count();
                    if (teller > 0)
                    {
                        if (foundRows[0]["Diersoort"] == DBNull.Value)
                        {
                            diersoort = 2;
                        }
                        else
                        {
                            diersoort = Convert.ToInt32(foundRows[0]["Diersoort"].ToString());
                        }
                    }
                    unLogger.WriteInfo("-----------------------------------------------------------------------------------------");
                    unLogger.WriteInfo("Berekening voor:" + afNemerThrd.Thr_Brs_Number + " " + afNemerThrd.ThrCompanyName + " " + afNemerThrd.ThrSecondName);
                    foreach (DataRow rw in foundRows)
                    {
                        teller -= 1;
                        DiersoortLabid = 0;
                        int.TryParse(rw["Diersoort"].ToString(),out DiersoortLabid);
                        if (DiersoortLabid != diersoort)
                        {
                            //opslaan
                            MUTA_VOER m = new MUTA_VOER();
                            m.Mv_datum = jaar;
                            m.Mv_Totaal_P205 =  (float)Math.Round(NormP,2);
                            m.Mv_Totaal_N = (float)Math.Round(NormRE,2);
                            m.Mv_Totaal_KG = (float)Math.Round(Totaal,2);
                            m.Mv_BRS = levraarTHIRD.Thr_Brs_Number;
                            m.Mv_KvK = levraarTHIRD.ThrKvkNummer;
                            m.Mv_Afnemer_BRS = afNemerThrd.Thr_Brs_Number;
                            m.Mv_BRS = levraarTHIRD.Thr_Brs_Number;
                            m.Mv_ThrID = pThrId;
                            m.Mv_Afnemer_ThrID = afNemerThrd.ThrId;
                            m.Mv_Totaal_KG = (float)Math.Round(Totaal,2);
                            m.Mv_Totaal_N = (float)Math.Round(NormRE,2);
                            m.Mv_Totaal_P205 = (float)Math.Round(NormP, 2);
                            unLogger.WriteInfo("afNemerThrd.Thr_Brs_Number:" + afNemerThrd.Thr_Brs_Number + " Totaal_KG:" + m.Mv_Totaal_KG.ToString() + " Mv_Totaal_N:" + m.Mv_Totaal_N.ToString() + " Mv_Totaal_P205:" + m.Mv_Totaal_P205.ToString());

                            if (diersoort > 0)
                            {
                                //uit agrolabels diersoort
                                //
                                m.Mv_Diersoort = diersoort;
                                m.Mv_MinasCategory = diersoort;
                            }
                            else 
                            {
                                m.Mv_Diersoort = 1;
                                m.Mv_MinasCategory = diersoort;
                            }
                    
                          
                            TemplevList.Add(m);
                            //nieuwe Farm en(is) diersoort
                            diersoort = DiersoortLabid;
                            float tot = float.Parse(rw["SupVolume"].ToString());
                            Totaal = tot;
                            NormRE = getSupNormRE(tot, double.Parse(rw["SupNormRE"].ToString()), int.Parse(rw["SupDryMatter"].ToString()), Convert.ToBoolean(rw["SupDryMatterMeasurement"]));
                            NormP = getSupNormP(tot, double.Parse(rw["SupNormP"].ToString()), int.Parse(rw["SupDryMatter"].ToString()), Convert.ToBoolean(rw["SupDryMatterMeasurement"]));
                        }
                        else
                        {
                            float tot = float.Parse(rw["SupVolume"].ToString());
                            Totaal = Totaal + tot;
                            NormRE = NormRE + getSupNormRE(tot,double.Parse(rw["SupNormRE"].ToString()), int.Parse(rw["SupDryMatter"].ToString()), Convert.ToBoolean(rw["SupDryMatterMeasurement"]));
                            NormP = NormP + getSupNormP(tot, double.Parse(rw["SupNormP"].ToString()), int.Parse(rw["SupDryMatter"].ToString()), Convert.ToBoolean(rw["SupDryMatterMeasurement"]));
                        }
                        if (teller == 0)
                        {
                            //opslaan
                            MUTA_VOER m = new MUTA_VOER();
                            m.Mv_datum = jaar;
                            m.Mv_Totaal_P205 = (float)Math.Round(NormP,2);
                            m.Mv_Totaal_N = (float)Math.Round(NormRE,2);
                            m.Mv_Totaal_KG = (float)Math.Round(Totaal,2);
                            m.Mv_BRS = levraarTHIRD.Thr_Brs_Number;
                            m.Mv_KvK = levraarTHIRD.ThrKvkNummer;
                            m.Mv_Afnemer_BRS = afNemerThrd.Thr_Brs_Number;
                            m.Mv_BRS = levraarTHIRD.Thr_Brs_Number;
                            m.Mv_ThrID = pThrId;
                            m.Mv_Afnemer_ThrID = afNemerThrd.ThrId;
                            m.Mv_Totaal_KG = (float)Math.Round(Totaal,2);
                            m.Mv_Totaal_N = (float)Math.Round(NormRE,2);
                            m.Mv_Totaal_P205 = (float)Math.Round(NormP, 2);
                            unLogger.WriteInfo("afNemerThrd.Thr_Brs_Number:" + afNemerThrd.Thr_Brs_Number + " Totaal_KG:" + m.Mv_Totaal_KG.ToString() + " Mv_Totaal_N:" + m.Mv_Totaal_N.ToString() + " Mv_Totaal_P205:" + m.Mv_Totaal_P205.ToString());
                            if (diersoort > 0)
                            {
                                //uit agrolabels diersoort
                                //
                                m.Mv_Diersoort = diersoort;
                                m.Mv_MinasCategory = diersoort;
                            }
                            else
                            {
                                m.Mv_Diersoort = 1;
                                m.Mv_MinasCategory = diersoort;
                            }

                        
                            TemplevList.Add(m);
                            DiersoortLabid = 0;
                     
                            Totaal = 0;
                            NormRE = 0;
                            NormP = 0;
                        }

                    }

                }
            }    
            //controle op alleen staldieren
            for (int t = TemplevList.Count() - 1; t > -1; t--)
            {
                MUTA_VOER mb = TemplevList.ElementAt(t);
                int Labid = utils.getLabId260(mb.Mv_MinasCategory);
                string diercode = utils.getLNVDiercodeByMinascategory(mb.Mv_MinasCategory);
                if (!utils.isLNVStaldier(diercode))
                {
                    TemplevList.Remove(TemplevList.ElementAt(t));
                    
                }
            }
            //DateTime nu = DateTime.Now;
            //if (nu.Year == jaar.Year || nu.Year == jaar.Year - 1)
            //{
            List<MUTA_VOER_LOG> voerloggen = lMstb.getMutaVoerenLoggen(pThrId, jaar);
            for (int c = TemplevList.Count()-1; c > -1; c--)
            { 
                MUTA_VOER mTempp = TemplevList.ElementAt(c);

                var temp = from n in voerloggen
                           where n.Mv_Afnemer_ThrID == mTempp.Mv_Afnemer_ThrID && n.Mv_MinasCategory==mTempp.Mv_MinasCategory && n.Mv_Totaal_KG==mTempp.Mv_Totaal_KG
                           select n;
                if (temp != null)
                {
                    if (temp.Count() > 0)
                    {
                        TemplevList.Remove(TemplevList.ElementAt(c));
                    }
                }
                      
            }
            DataTable tbl = lMstb.getMutaVoeren(pThrId, jaar);
            List<MUTA_VOER> bestaand = new List<MUTA_VOER>();
            foreach (DataRow drVoer in tbl.Rows)
            {
                MUTA_VOER mp = new MUTA_VOER();
                if (lMstb.GetDataBase().FillObject(mp, drVoer))
                {
                    bestaand.Add(mp);
                }
            }
            foreach (MUTA_VOER best in bestaand)
            {
                lMstb.deleteMutaVoer(best);
            }
            foreach (MUTA_VOER m in TemplevList)
            {
                lMstb.insertMutaVoer(m);
            }
            //}
            return TemplevList;

        }

        public float getSupNormRE(float pTotal, double pSupNormRE, int pSupDryMatter, bool pSupDryMatterMeaserment)
        {
            float dry = ((float)pSupDryMatter / 1000);
            float re = (float)pSupNormRE;
            float div1 = 6.25F;
            if (pSupDryMatterMeaserment)
            {

                unLogger.WriteInfo("getSupNormRE pSupDryMatterMeaserment=true 1 :" + pTotal.ToString() + "*" + re.ToString() + "/" + div1.ToString() + "/1000 * " + dry.ToString());
                
                return pTotal * re / div1 / 1000 * dry;
            }
            else
            {

                unLogger.WriteInfo("getSupNormRE pSupDryMatterMeaserment=false 0 :" + pTotal.ToString() + "*" + re.ToString() + "/" + div1.ToString() + "/1000");
                
                return pTotal * re / div1 / 1000;
            }
            //return ((re * (dry)) / div1);
        }

        public float getSupNormP(float pTotal,double pSupNormP, int pSupDryMatter, bool pSupDryMatterMeaserment)
        {
            float dry = ((float)pSupDryMatter / 1000);
            float np = (float)pSupNormP;
            float div2 = 2.29F;
            if (pSupDryMatterMeaserment)
            {
                unLogger.WriteInfo("getSupNormP pSupDryMatterMeaserment=true 1 :" + pTotal.ToString() + "*" + np.ToString() + "/" + div2.ToString() + "/1000 * " + dry.ToString());
                
                return pTotal * np * div2 / 1000 * dry;
            }
            else
            {
                unLogger.WriteInfo("getSupNormP pSupDryMatterMeaserment=false 0 :" + pTotal.ToString() + "*" + np.ToString() + "/" + div2.ToString() + "/1000");
                
                return pTotal * np * div2 / 1000;
            }
            //return ((np * (dry)) * div2);
        }


       
    }
}
