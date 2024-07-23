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
    public class MineraalUtils
    {
        public List<MAANTDRN> getGemiddeldeAantalDieren(UserRightsToken pUserRightsToken, string pMestnummer, int pJaar, int pDiersoort)
        {
            List<MAANTDRN> lResultSet = new List<MAANTDRN>();
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUserRightsToken);
            List<MBEGIN> beginnen = lMstb.getMBeginnen(pMestnummer, pJaar, pDiersoort);
            List<MMUTAT> mutats = lMstb.getMMutats(pMestnummer, pJaar);
            mutats = (from n in mutats where n.AniGroup == pDiersoort select n).ToList();
            List<MMNDTEL> maandtelling = lMstb.getMMndtels(pMestnummer, pJaar);
            maandtelling = (from n in maandtelling where n.Diergroep == pDiersoort select n).ToList();

            //By Diersoort // By Category // By Stalsysteem
            //ByMONTH AND tTHEn By Year
            // results in One MAANTDRN  
            List<LABMINAS> lCategorien = lMstb.GetLabMinas((20 + pDiersoort), 1);
            List<LABMINAS> lStalsystemen = lMstb.GetLabMinas((401), 1);
            //REDENS LabKind 503    1-2 = eraf 4=niks  3-5 erbij
            foreach(LABMINAS lCat in lCategorien)
            {
                foreach (LABMINAS lStal in lStalsystemen)
                {
                    List<MMUTAT> catstalmutats = (from n in mutats where n.AniCategory == lCat.LabId && n.Stalsysteem==lStal.LabId && (n.Reason!=4) select n).ToList();
                    List<MMNDTEL> catstalmaandtelling = (from n in maandtelling where n.Diercategorie == lCat.LabId && n.Stalsysteem == lStal.LabId select n).ToList();
                    List<MBEGIN> catstalbeginnen = (from n in beginnen where n.AniCategory == lCat.LabId && n.Stalsysteem == lStal.LabId select n).ToList();

                    int[] lAverageAantal = new int[12];
                    for (int lMonth = 1; lMonth < 13; lMonth++)
                    {
                        List<MMUTAT> maandmutatserbij = (from n in catstalmutats where n.MutDate.Month == lMonth && (n.Reason==3 || n.Reason==5 ) select n).ToList();
                        List<MMUTAT> maandmutatseraf = (from n in catstalmutats where n.MutDate.Month == lMonth && (n.Reason==1 || n.Reason==2 ) select n).ToList();
                        List<MMNDTEL> maandmaandtelling = (from n in catstalmaandtelling where n.Datum.Month == (lMonth) select n).ToList();
                        if (lMonth == 12)
                        {
                            maandmaandtelling = (from n in catstalmaandtelling where n.Datum.Month == lMonth && n.Datum.Day == 1 select n).ToList();
                        }
                        int catstalgem = 0;
                        if (lMonth == 1)
                        {
                            if (maandmaandtelling.Count() > 0)
                            {
                                catstalgem = maandmaandtelling.Sum(item => item.Aantal) / maandmaandtelling.Count;
                            }
                            else
                            {

                                if (catstalbeginnen.Count > 0)
                                {
                                    catstalgem = catstalbeginnen.Sum(item => item.Amount) / catstalbeginnen.Count;
                                }
                            }
                            lAverageAantal[lMonth - 1] = catstalgem + (int)(maandmutatserbij.Sum(item => item.Number)) - (int)(maandmutatseraf.Sum(item => item.Number));
                        }
                        else
                        {
                            if (maandmaandtelling.Count() > 0)
                            {
                                catstalgem = maandmaandtelling.Sum(item => item.Aantal) / maandmaandtelling.Count;
                            }
                            else
                            {
                                catstalgem = lAverageAantal[lMonth - 2];
                            }
                            lAverageAantal[lMonth - 1] = catstalgem + (int)(maandmutatserbij.Sum(item => item.Number)) - (int)(maandmutatseraf.Sum(item => item.Number));
                      
                        }
                    }
                    int delendoor = 0;
                    int totaal = 0;
                    foreach (int lAantal in lAverageAantal)
                    {
                        if (lAantal > 0)
                        {
                            delendoor += 1;
                            totaal += lAantal;
                        }
                    }
                    if (totaal > 0 && delendoor > 0)
                    {
                        MAANTDRN m = new MAANTDRN();
                        m.Amount = totaal / delendoor;
                        m.AniCategory = lCat.LabId;
                        m.AnimalKind = pDiersoort;
                        m.Mestnummer = pMestnummer;
                        m.Stalsysteem = lStal.LabId;
                        m.Year = pJaar;
                        lResultSet.Add(m);
                    }                    
                }
                
            }

            
            

            return lResultSet;
        }

        public void getEdiZuivelGemiddelden(UserRightsToken pUserRightsToken, string pMestnummer, int pJaar, int pUbnId, out int iKgmelk, out int iUreum, out double dPercVet, out double dPercProtein)
        {
            /*
             * Mineraal prognose overig
             De knop telt alle melkleveringen op van de afgelopen 12 maanden voor dat bedrijf. De te gebruiken tabel is MILKSUPL.

            Zoek eerst de maximale DeliveryDate en tel daarvan 12 maanden terug
            Gebruik voor ureum, vet en eiwit het gewogen gemiddelde (bijv: per levering kgmelk x %vet (=kgvet) en aan het eind delen door totaal kgmelk geleverd)

            Jos 
             */
            iKgmelk = 0;
            iUreum = 0;
            dPercVet = 0;    //Fat
            dPercProtein = 0;  //Protein
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUserRightsToken);
            List<MESTUBN> mestubns = new List<MESTUBN>();
            if (pMestnummer != "")
            {
                mestubns = lMstb.getMestUbns(pMestnummer);
                mestubns = (from n in mestubns where !n.FarmNumber.StartsWith("-") select n).ToList();
            }
            StringBuilder lquery = new StringBuilder();
            lquery.Append(" SELECT * FROM agrofactuur.MILKSUPL msup  ");
            lquery.Append(" JOIN agrofactuur.UBN ON UBN.UBNid=msup.UbnID ");
            lquery.Append(" JOIN agrofactuur.MESTUBN ON agrofactuur.MESTUBN.FarmNumber=agrofactuur.UBN.Bedrijfsnummer ");
            lquery.AppendFormat(" WHERE agrofactuur.MESTUBN.Mestnummer='{0}'  ", pMestnummer);
           
            lquery.Append(" AND msup.DeliveryDate <= (SELECT MAX(ms.DeliveryDate) FROM agrofactuur.MILKSUPL ms WHERE ms.UbnId=msup.UBNid) ");
            lquery.Append(" AND msup.DeliveryDate >= DATE_SUB( (SELECT MAX(ms.DeliveryDate) FROM agrofactuur.MILKSUPL ms WHERE ms.UbnId=msup.UBNid) , INTERVAL 1 YEAR)  ");
            lquery.Append(" ORDER BY DeliveryDate ");


            DataSet ds = new DataSet();
            DataTable tbl = lMstb.GetDataBase().QueryData(pUserRightsToken, ds, lquery, "milk", MissingSchemaAction.Add);
            if (tbl.Rows.Count > 0)
            {
                foreach (DataRow rw in tbl.Rows)
                {
                    MILKSUPL ms = new MILKSUPL();
                    lMstb.GetDataBase().FillObject(ms, rw);
                    iKgmelk += (int)ms.KgMilk;
                    iUreum += ms.UreumTank;
                    dPercVet += ms.FatTank;
                    dPercProtein += ms.ProteinTank;
                }
                dPercVet = dPercVet / tbl.Rows.Count;
                dPercProtein = dPercProtein / tbl.Rows.Count;
            }
        }
    }
}
