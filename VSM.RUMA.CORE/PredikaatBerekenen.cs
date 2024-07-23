using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE
{
    public class PredikaatBerekenen
    {
        //Zie BUG 1460 voor AGRO_LABELS
        public string ChildrenDetailFolderPath { get; set; }
        public string ChildrenDetailFilename { get; set; }
        public bool WriteChildrenDetails { get; set; }
        private StringBuilder SbldExterieurCheckTexelaarNakomelingTelling = new StringBuilder();
        #region  Texelaar

        public DataTable BerekenSterooiTexelaar(UserRightsToken pToken, int? Stamboek, int? FarmId)
        {
            StringBuilder QRY_Sterooi = new StringBuilder();
            QRY_Sterooi.Append(" SELECT agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,agrofactuur.BEDRIJF.FarmId,  ANIMAL.AniLifeNumber,  ANIMAL.AniBirthDate,  \r\n");
            QRY_Sterooi.Append(" ANIMAL.AniId, MAX(EVENT.EveDate) AS LaatsteWorp, \r\n");
            QRY_Sterooi.Append(" COUNT(DISTINCT(EVENT.EveDate)) as Worpen,  0 as Lammeren,  \r\n");
            QRY_Sterooi.Append(" 0 AS AlgemeenVoorkomen, 0 AS Bespiering, \r\n");
            QRY_Sterooi.Append(" 0 AS LamOoi, 0 AS LamRam,\r\n");
            QRY_Sterooi.Append(" 0 AS LamOoiDefinitief, 0 AS LamRamDefinitief,\r\n");
            QRY_Sterooi.Append(" 0 AS NieuwePunten, ANIMALPREDIKAAT.PreScore AS OudePunten\r\n");
            QRY_Sterooi.Append(" FROM ANIMAL \r\n");
            QRY_Sterooi.Append(" JOIN ANIMALCATEGORY  ON ANIMAL.AniId = ANIMALCATEGORY.AniId  \r\n");
            QRY_Sterooi.Append(" INNER JOIN agrofactuur.BEDRIJF  ON agrofactuur.BEDRIJF.FarmId = ANIMALCATEGORY.FarmId  \r\n");
            QRY_Sterooi.Append(" INNER JOIN agrofactuur.UBN  ON agrofactuur.BEDRIJF.UBNid = agrofactuur.UBN.UBNid  \r\n");
            QRY_Sterooi.Append(" INNER JOIN EXTERIEUR  ON EXTERIEUR.aniId = ANIMAL.AniId  AND EXTERIEUR.ExtKind > 1\r\n");
            QRY_Sterooi.Append(" LEFT JOIN ANIMALPREDIKAAT \r\n");
            QRY_Sterooi.Append(" ON ANIMALPREDIKAAT.PreAniId = ANIMAL.AniId\r\n");
            QRY_Sterooi.Append(" AND ANIMALPREDIKAAT.PreTypePredikaat IN (8,9,10,11)\r\n");
            QRY_Sterooi.Append(" AND PreBegindatum <= CURRENT_DATE()\r\n");
            QRY_Sterooi.Append(" AND (PreEinddatum >= CURRENT_DATE() OR ISNULL(PreEinddatum))\r\n");
            QRY_Sterooi.Append(" INNER JOIN EVENT  \r\n");
            QRY_Sterooi.Append("  ON (ANIMAL.AniId = EVENT.AniId)  AND EVENT.EveKind = 5  \r\n");
            QRY_Sterooi.Append(" INNER JOIN BIRTH   \r\n");
            QRY_Sterooi.Append("  ON (BIRTH.EventId = EVENT.EventId)  AND BIRTH.BornDead = false  AND BIRTH.CalfId > 0 \r\n");
            QRY_Sterooi.Append("   WHERE (DATEDIFF(EVENT.EveDate, ANIMAL.AniBirthDate) > 465) \r\n");
            QRY_Sterooi.Append("	 AND ANIMAL.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 4 YEAR) \r\n");
            QRY_Sterooi.Append("	  AND ANIMALCATEGORY.Anicategory < 5 \r\n");
            QRY_Sterooi.Append("      AND ANIMAL.AniSex = 2  AND ANIMAL.AniId > 0 \r\n");
            if (Stamboek.HasValue)
                QRY_Sterooi.AppendFormat("	  AND agrofactuur.BEDRIJF.Programid IN ({0}) \r\n", Stamboek.Value);
            if (FarmId.HasValue)
                QRY_Sterooi.AppendFormat("	  AND agrofactuur.BEDRIJF.FarmId IN ({0}) \r\n", FarmId.Value);
            QRY_Sterooi.Append("	  GROUP BY agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,  \r\n");
            QRY_Sterooi.Append("	  ANIMAL.AniLifeNumber,  ANIMAL.AniId,  ANIMAL.AniBirthDate \r\n");
            QRY_Sterooi.Append("  HAVING ((DATEDIFF(CURDATE(), ANIMAL.AniBirthDate)) / 365) <= COUNT(*)  \r\n");
            //QRY_Sterooi.Append("  HAVING ((DATEDIFF(MAX(EVENT.EveDate), ANIMAL.AniBirthDate)) / 365) <= COUNT(*)  \r\n");            
            QRY_Sterooi.Append(" AND\r\n");
            QRY_Sterooi.Append(" (\r\n");
            QRY_Sterooi.Append(" SELECT COUNT(*)  \r\n");
            QRY_Sterooi.Append(" FROM ANIMAL AS LAM  \r\n");
            QRY_Sterooi.Append(" INNER JOIN EXTERIEUR AS LAMEXTERIEUR  \r\n");
            QRY_Sterooi.Append(" ON LAMEXTERIEUR.aniId = LAM.AniId   \r\n");
            QRY_Sterooi.Append(" WHERE LAM.AniIdMother = ANIMAL.AniId  \r\n");
            QRY_Sterooi.Append(" AND LAMEXTERIEUR.ExtKind > 1   \r\n");
            QRY_Sterooi.Append(" )");
            QRY_Sterooi.Append(" >= (((DATEDIFF(MAX(EVENT.EveDate), ANIMAL.AniBirthDate)) / 365) - 2)\r\n");
            QRY_Sterooi.Append("  AND ((ANIMAL.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 6 YEAR)) \r\n");
            //QRY_Sterooi.Append("  AND ((ANIMAL.AniBirthDate <= DATE_SUB(MAX(EVENT.EveDate),INTERVAL 6 YEAR)) \r\n");
            QRY_Sterooi.Append("  OR (COUNT(DISTINCT(EVENT.EveDate)) >= 3 AND \r\n");
            QRY_Sterooi.Append("  (( \r\n");
            QRY_Sterooi.Append("  COUNT(DISTINCT(EVENT.EveDate)) = 3 AND \r\n");
            QRY_Sterooi.Append("  COUNT(EVENT.EventId) >= 5 \r\n");
            QRY_Sterooi.Append("  )  \r\n");
            QRY_Sterooi.Append("  OR \r\n");
            QRY_Sterooi.Append("  ( \r\n");
            QRY_Sterooi.Append("  COUNT(DISTINCT(EVENT.EveDate)) = 4 AND \r\n");
            QRY_Sterooi.Append("  COUNT(EVENT.EventId) >= 7 \r\n");
            QRY_Sterooi.Append("  )  \r\n");
            QRY_Sterooi.Append("  OR \r\n");
            QRY_Sterooi.Append("  ( \r\n");
            QRY_Sterooi.Append("  COUNT(DISTINCT(EVENT.EveDate)) = 5 AND \r\n");
            QRY_Sterooi.Append("  COUNT(EVENT.EventId) >= 9 \r\n");
            QRY_Sterooi.Append("  )  \r\n");
            QRY_Sterooi.Append("  OR \r\n");
            QRY_Sterooi.Append("  ( \r\n");
            QRY_Sterooi.Append("  COUNT(DISTINCT(EVENT.EveDate)) = 6 AND \r\n");
            QRY_Sterooi.Append("  COUNT(EVENT.EventId) >= 11 \r\n");
            QRY_Sterooi.Append("  ) \r\n");
            QRY_Sterooi.Append("  OR \r\n");
            QRY_Sterooi.Append("  ( \r\n");
            QRY_Sterooi.Append("  COUNT(DISTINCT(EVENT.EveDate)) > 6 AND \r\n");
            QRY_Sterooi.Append("  COUNT(EVENT.EventId) >= 11 \r\n");
            QRY_Sterooi.Append("  ) \r\n");
            QRY_Sterooi.Append("  ))  \r\n");       
            QRY_Sterooi.Append("  ) \r\n");
            Facade.GetInstance().UpdateProgress(1, "Gegevens ophalen...");
            DataTable tbl = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Sterooi, MissingSchemaAction.Add);

            List<DataRow> drList = new List<DataRow>();

            drList.AddRange(tbl.Select());
            StringBuilder bldChildrenDetails = new StringBuilder();


            foreach (DataRow dr in drList)
            {



                unLogger.WriteDebug("###############################################################################");
                unLogger.WriteDebug("Berekening voor Dier :" + dr["AniLifeNumber"].ToString());
                double perc = (drList.IndexOf(dr) / (Convert.ToDouble(drList.Count) / 100));
                Facade.GetInstance().UpdateProgress(Convert.ToInt32(Math.Floor(perc)), String.Format("Berekenen... {0} % ", Convert.ToInt32(perc)));
                int AniId = Convert.ToInt32(dr["AniId"]);
                int ExtAlgemeenVoorkomen = 0;
                int ExtBespiering = 0;
                if (!CheckTexelaarProductie(pToken, AniId))
                {

                    unLogger.WriteDebug("Dier voldoet niet aan de Vruchtbaarheid voorwaarden. (3)");
                    tbl.Rows.Remove(dr);
                    continue;
                }

                if (!ExterieurCheckTexelaar(pToken, AniId, ref ExtAlgemeenVoorkomen, ref ExtBespiering))
                {
                    unLogger.WriteDebug("Dier voldoet niet aan de Exterieur voorwaarden. (1, 2)");
                    tbl.Rows.Remove(dr);
                    continue;
                }
                dr["AlgemeenVoorkomen"] = ExtAlgemeenVoorkomen;
                dr["Bespiering"] = ExtBespiering;
                bldChildrenDetails.Append(dr["FarmId"].ToString() + ";" + dr["AniId"].ToString() + ";" + dr["AniLifeNumber"].ToString() + ";AlgemeenVoorkomen:" + ExtAlgemeenVoorkomen.ToString() + ";Bespiering:" + ExtBespiering.ToString() + "\r\n");


                StringBuilder QRY_Children = new StringBuilder();



                QRY_Children.Append(" SELECT * ");
                QRY_Children.Append(" FROM ANIMAL");
                QRY_Children.Append(" INNER JOIN EXTERIEUR ");
                QRY_Children.Append(" ON EXTERIEUR.aniId = ANIMAL.AniId ");
                QRY_Children.AppendFormat(" WHERE ANIMAL.AniIdMother = {0}  AND ANIMAL.AniId > 0 ", AniId);
                QRY_Children.Append(" AND EXTERIEUR.ExtKind > 1 ");
                QRY_Children.Append(" GROUP BY ANIMAL.AniId ");
                DataTable tblchildren = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
                int Years = Event_functions.getDatumFormat(dr["LaatsteWorp"], "LaatsteWorp").Year - Event_functions.getDatumFormat(dr["AniBirthDate"], "AniBirthDate").Year;

                dr["LamOoiDefinitief"] = tblchildren.Select("AniSex = 2").GroupBy(row => row["AniId"]).Count();
                dr["LamRamDefinitief"] = tblchildren.Select("AniSex = 1").GroupBy(row => row["AniId"]).Count();
                int KinderenDefinief = tblchildren.Rows.Count;
                if (KinderenDefinief < 2)
                {
                    tbl.Rows.Remove(dr);
                    bldChildrenDetails.Append("Minder dan 2 kinderen\r\n");
                    unLogger.WriteDebug("Minder dan 2 kinderen definitief gekeurd(4.c)");
                    continue;
                }
                else if (KinderenDefinief < Years - 2)
                {
                    tbl.Rows.Remove(dr);
                    bldChildrenDetails.Append("kinderen zijn jonger dan 2 jaar\r\n");
                    unLogger.WriteDebug("Te weinig kinderen definitief gekeurd (4.c)");
                    continue;
                }
                else
                {
                    QRY_Children = new StringBuilder();
                    QRY_Children.Append(" SELECT * ");
                    QRY_Children.Append(" FROM ANIMAL");
                    QRY_Children.AppendFormat(" WHERE ANIMAL.AniIdMother = {0}  AND ANIMAL.AniId > 0 ", AniId);
                    tblchildren = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
                    dr["Lammeren"] = tblchildren.Rows.Count;
                    dr["LamOoi"] = tblchildren.Select("AniSex = 2").Count();
                    dr["LamRam"] = tblchildren.Select("AniSex = 1").Count();

                    int bespiering = tblchildren.Rows.Count;
                    int punten = 0;
                    int extrapunten = 0;
                    unLogger.WriteDebug("---------------------------------------------------------------------");

                    foreach (DataRow drchildren in tblchildren.Rows)
                    {
                        int ChildId = Convert.ToInt32(drchildren["AniId"]);
                        int AniSex = Convert.ToInt32(drchildren["AniSex"]);
                        unLogger.WriteDebug("Nakomeling " + drchildren["AniLifeNumber"].ToString());
                        bldChildrenDetails.Append("#" + ChildId.ToString() + ":");

                        if (!ExterieurCheckTexelaarNakomeling(pToken, ChildId, AniSex, ref punten))
                        {
                            bespiering = bespiering - 1;
                            bldChildrenDetails.Append("Bespiering:-1:Nakomelingen:" + SbldExterieurCheckTexelaarNakomelingTelling.ToString() + ":Predikaat:0:");

                            if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                            {
                                SbldExterieurCheckTexelaarNakomelingTelling.Remove(0, SbldExterieurCheckTexelaarNakomelingTelling.Length);
                            }
                            continue;
                        }
                        if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                        {
                            bldChildrenDetails.Append("Nakomelingen:" + SbldExterieurCheckTexelaarNakomelingTelling.ToString() + ":");
                            SbldExterieurCheckTexelaarNakomelingTelling.Remove(0, SbldExterieurCheckTexelaarNakomelingTelling.Length);
                        }
                        StringBuilder QRY_PredikatenKinderen = new StringBuilder();
                        QRY_PredikatenKinderen.Append(" SELECT * ");
                        QRY_PredikatenKinderen.Append(" FROM ANIMAL");
                        QRY_PredikatenKinderen.Append(" INNER JOIN ANIMALPREDIKAAT ");
                        QRY_PredikatenKinderen.Append(" ON ANIMALPREDIKAAT.PreAniId = ANIMAL.AniId ");
                        QRY_PredikatenKinderen.AppendFormat(" WHERE ANIMAL.AniId = {0} ", ChildId);
                        QRY_PredikatenKinderen.Append(" AND PreBegindatum <= CURRENT_DATE()");
                        QRY_PredikatenKinderen.Append(" AND (PreEinddatum >= CURRENT_DATE() OR ISNULL(PreEinddatum))");
                        DataTable PredikatenKinderen = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_PredikatenKinderen, MissingSchemaAction.Add);
                        int predikaatteller = 0;
                        foreach (DataRow PredikaatKinderen in PredikatenKinderen.Rows)
                        {
                            int type = Convert.ToInt32(PredikaatKinderen["PreTypePredikaat"]);
                            switch (type)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    extrapunten += 2;
                                    predikaatteller += 2;
                                    unLogger.WriteDebug(" Predikaat extrapunten +2");
                                    break;
                                case 4:
                                    extrapunten += 4;
                                    predikaatteller += 4;
                                    unLogger.WriteDebug(" Predikaat extrapunten +4");
                                    break;
                                case 5:
                                case 6:
                                case 7:
                                case 13:
                                    predikaatteller += 10;
                                    unLogger.WriteDebug(" Predikaat extrapunten +10");
                                    extrapunten += 10;
                                    break;
                                default:
                                    break;
                            }
                        }
                        bldChildrenDetails.Append("Predikaat:" + predikaatteller.ToString() + ":");
                        unLogger.WriteDebug("---------------------------------------------------------------------");

                    }
                    bldChildrenDetails.Append("\r\n");
                    if (bespiering < (KinderenDefinief * 0.75) || punten < 13)
                    {
                        //bldChildrenDetails.Append("Bespiering:punten verwijderd of aantal punten<13\r\n");
                        unLogger.WriteDebug("Dier voldoet niet aan de Nakomelingen voorwaarden. (4c,4d) ");
                        tbl.Rows.Remove(dr);
                        continue;
                    }
                    else if (dr["OudePunten"].GetType() != typeof(DBNull) && Convert.ToInt32(dr["OudePunten"]) == (punten + extrapunten))
                    {
                        //dr["NieuwePunten"] = 0;// String.Empty;
                        tbl.Rows.Remove(dr);
                        unLogger.WriteDebug("Aantal punten gelijk aan oude punten\r\n");
                        //bldChildrenDetails.Append("NieuwePunten:0 of aantal punten gelijk aan oude punten\r\n");
                    }
                    else
                    {
                        dr["NieuwePunten"] = (punten + extrapunten);
                        //bldChildrenDetails.Append("NieuwePunten:punten=" + punten.ToString() +  " extrapunten:"  + extrapunten.ToString() + "\r\n");
                    }

                }

            }
            if (bldChildrenDetails.Length > 0)
            {
                writeChildFile(bldChildrenDetails.ToString(), false);
            }
            return tbl;
        }

        public DataTable BerekenSterooiTES(UserRightsToken pToken, int? Stamboek, int? FarmId)
        {
            StringBuilder QRY_Sterooi = new StringBuilder();
            QRY_Sterooi.Append(" SELECT agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,agrofactuur.BEDRIJF.FarmId,  ANIMAL.AniLifeNumber,  ANIMAL.AniBirthDate,MAX(MOVEMENT.MovDate) AS Afvoerdatum, \r\n");
            QRY_Sterooi.Append(" ANIMAL.AniId,MIN(EVENT.EveDate) AS EersteWorp, MAX(EVENT.EveDate) AS LaatsteWorp, \r\n");
            QRY_Sterooi.Append("  COUNT(DISTINCT(EVENT.EveDate)) as Worpen,  0 as Lammeren, \r\n");
            QRY_Sterooi.Append(" 0 AS AlgemeenVoorkomen, 0 AS Bespiering, 0 AS Punten, \r\n");
            QRY_Sterooi.Append(" 0 AS LamOoi, 0 AS LamRam,\r\n");
            QRY_Sterooi.Append(" 0 AS LamOoiDefinitief, 0 AS LamRamDefinitief,\r\n");
            QRY_Sterooi.Append(" '' AS NieuwPredikaat, PREDIKAAT.LabLabel AS VorigPredikaat\r\n");

            QRY_Sterooi.Append(", YEAR((SELECT MAX(EveDate) AS maxWorpDatum FROM EVENT ");
            QRY_Sterooi.Append("             WHERE EveDate <=   NOW()  AND AniId=ANIMAL.AniId AND EveKind=5 AND EventId>0)) ");
            QRY_Sterooi.Append("         -YEAR(ANIMAL.AniBirthDate) AS Leeftijd ");


            QRY_Sterooi.Append(" FROM ANIMAL \r\n");
            QRY_Sterooi.Append(" JOIN ANIMALCATEGORY  ON ANIMAL.AniId = ANIMALCATEGORY.AniId  \r\n");
            QRY_Sterooi.Append(" INNER JOIN agrofactuur.BEDRIJF  ON agrofactuur.BEDRIJF.FarmId = ANIMALCATEGORY.FarmId  \r\n");
            QRY_Sterooi.Append(" INNER JOIN agrofactuur.UBN  ON agrofactuur.BEDRIJF.UBNid = agrofactuur.UBN.UBNid  \r\n");
            QRY_Sterooi.Append(" LEFT OUTER JOIN agrofactuur.LABELS AS PREDIKAAT  ON PREDIKAAT.LabId = ANIMAL.AniPredikaat  AND PREDIKAAT.LabKind = 90 AND PREDIKAAT.LabCountry = 528 \r\n");
            QRY_Sterooi.Append(" LEFT OUTER JOIN MOVEMENT");
            QRY_Sterooi.Append("  ON (ANIMAL.AniId = MOVEMENT.AniId)  AND MOVEMENT.MovKind IN (2,3) AND ANIMALCATEGORY.Anicategory = 4"); 
            QRY_Sterooi.Append(" INNER JOIN EVENT  \r\n");
            QRY_Sterooi.Append("  ON (ANIMAL.AniId = EVENT.AniId)  AND EVENT.EveKind = 5  \r\n");
            QRY_Sterooi.Append(" INNER JOIN BIRTH   \r\n");
            QRY_Sterooi.Append("  ON (BIRTH.EventId = EVENT.EventId)  \r\n");
            QRY_Sterooi.Append("   WHERE ANIMAL.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 4 YEAR)  \r\n");
            QRY_Sterooi.Append("	  AND ANIMALCATEGORY.Anicategory < 5 \r\n");
            QRY_Sterooi.Append("      AND ANIMAL.AniSex = 2  AND ANIMAL.AniId > 0 \r\n");
            if (Stamboek.HasValue)
                QRY_Sterooi.AppendFormat("	  AND agrofactuur.BEDRIJF.Programid IN ({0}) \r\n", Stamboek.Value);
            if (FarmId.HasValue)
                QRY_Sterooi.AppendFormat("	  AND agrofactuur.BEDRIJF.FarmId IN ({0}) \r\n", FarmId.Value);
            QRY_Sterooi.Append("	  GROUP BY agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,  \r\n");
            QRY_Sterooi.Append("	  ANIMAL.AniLifeNumber,  ANIMAL.AniId,  ANIMAL.AniBirthDate \r\n");
            QRY_Sterooi.Append(" HAVING  IF(ISNULL(MAX(MOVEMENT.MovDate)),\r\n");
 	        //QRY_Sterooi.Append(" IF((DATEDIFF(CURDATE(), ANIMAL.AniBirthDate) / 365) = COUNT(DISTINCT(EVENT.EveDate)),\r\n");
            QRY_Sterooi.Append(" IF(YEAR((SELECT MAX(EveDate) AS maxWorpDatum FROM EVENT ");
            QRY_Sterooi.Append("             WHERE EveDate <=   NOW()  AND AniId=ANIMAL.AniId AND EveKind=5 AND EventId>0)) ");
            QRY_Sterooi.Append("         -YEAR(ANIMAL.AniBirthDate) = COUNT(DISTINCT(EVENT.EveDate)),\r\n");
            QRY_Sterooi.Append(" ((COUNT(DISTINCT(EVENT.EveDate))-1) * 1.75) <= COUNT(DISTINCT(EVENT.EventId)),\r\n");
            QRY_Sterooi.Append(" (COUNT(DISTINCT(EVENT.EveDate)) * 1.75) <= COUNT(DISTINCT(EVENT.EventId))),\r\n");
            //QRY_Sterooi.Append(" IF((DATEDIFF(MAX(MOVEMENT.MovDate), ANIMAL.AniBirthDate) / 365) = COUNT(DISTINCT(EVENT.EveDate)),\r\n");
            QRY_Sterooi.Append(" IF(YEAR((SELECT MAX(EveDate) AS maxWorpDatum FROM EVENT ");
            QRY_Sterooi.Append("             WHERE EveDate <=   MAX(MOVEMENT.MovDate)  AND AniId=ANIMAL.AniId AND EveKind=5 AND EventId>0)) ");
            QRY_Sterooi.Append("         -YEAR(ANIMAL.AniBirthDate) = COUNT(DISTINCT(EVENT.EveDate)),\r\n");
            QRY_Sterooi.Append(" ((COUNT(DISTINCT(EVENT.EveDate))-1) * 1.75) <= COUNT(DISTINCT(EVENT.EventId)),\r\n");
            QRY_Sterooi.Append(" (COUNT(DISTINCT(EVENT.EveDate)) * 1.75) <= COUNT(DISTINCT(EVENT.EventId))))\r\n");
            QRY_Sterooi.Append(" AND \r\n");
            QRY_Sterooi.Append(" IF(ISNULL(MAX(MOVEMENT.MovDate)), \r\n");
            QRY_Sterooi.Append(" ANIMAL.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 4 YEAR), \r\n");
            QRY_Sterooi.Append(" ANIMAL.AniBirthDate <= DATE_SUB(MAX(MOVEMENT.MovDate),INTERVAL 4 YEAR)) \r\n");
            QRY_Sterooi.Append(" AND\r\n");
            QRY_Sterooi.Append(" (\r\n");
            QRY_Sterooi.Append(" SELECT COUNT(*)  \r\n");
            QRY_Sterooi.Append(" FROM ANIMAL AS LAM  \r\n");
            QRY_Sterooi.Append(" INNER JOIN EXTERIEUR AS LAMEXTERIEUR  \r\n");
            QRY_Sterooi.Append(" ON LAMEXTERIEUR.aniId = LAM.AniId   \r\n");
            QRY_Sterooi.Append(" WHERE LAM.AniIdMother = ANIMAL.AniId  \r\n");
            QRY_Sterooi.Append(" AND LAMEXTERIEUR.ExtKind > 1  \r\n");
            //QRY_Sterooi.Append(" AND LAMEXTERIEUR.extDatum > DATE_ADD(LAM.AniBirthDate,INTERVAL 1 YEAR)  \r\n");
            QRY_Sterooi.Append(" )");
            QRY_Sterooi.Append(" >= IF(ISNULL(MAX(MOVEMENT.MovDate)),(\r\n");
            //QRY_Sterooi.Append("((DATEDIFF(CURDATE(), ANIMAL.AniBirthDate)) / 365)\r\n");
            QRY_Sterooi.Append("YEAR((SELECT MAX(EveDate) AS maxWorpDatum FROM EVENT ");
            QRY_Sterooi.Append("             WHERE EveDate <=   NOW()  AND AniId=ANIMAL.AniId AND EveKind=5 AND EventId>0)) ");
            QRY_Sterooi.Append("         -YEAR(ANIMAL.AniBirthDate)");
            QRY_Sterooi.Append("    - 2),(");
            QRY_Sterooi.Append("YEAR((SELECT MAX(EveDate) AS maxWorpDatum FROM EVENT ");
            QRY_Sterooi.Append("             WHERE EveDate <= MAX(MOVEMENT.MovDate)  AND AniId=ANIMAL.AniId AND EveKind=5 AND EventId>0)) ");
            QRY_Sterooi.Append("         -YEAR(ANIMAL.AniBirthDate)");
            QRY_Sterooi.Append(" - 2))\r\n");
            Facade.GetInstance().UpdateProgress(1, "Gegevens ophalen...");
            DataTable tbl = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Sterooi, MissingSchemaAction.Add);
            List<DataRow> drList = new List<DataRow>();
            drList.AddRange(tbl.Select());
            StringBuilder bldChildrenDetails = new StringBuilder();
            foreach (DataRow dr in drList)
            {
                unLogger.WriteDebug("###############################################################################");
                unLogger.WriteDebug("Berekening voor Dier :" + dr["AniLifeNumber"].ToString());
                double perc = (drList.IndexOf(dr) / (Convert.ToDouble(drList.Count) / 100));
                Facade.GetInstance().UpdateProgress(Convert.ToInt32(Math.Floor(perc)), String.Format("Berekenen... {0} % ", Convert.ToInt32(perc)));
                int AniId = Convert.ToInt32(dr["AniId"]);
                int ExtAlgemeenVoorkomen = 0;
                int ExtBespiering = 0;
                if (!CheckTESProductie(pToken, AniId))
                {
                    unLogger.WriteDebug("Dier heeft onvoldoende levend geboren kinderen. (4c)");
                    if (dr["VorigPredikaat"].GetType() == typeof(DBNull))
                    {
                        tbl.Rows.Remove(dr);
                    }
                    continue;
                }

                ExterieurCheckTexelaar(pToken, AniId, ref ExtAlgemeenVoorkomen, ref ExtBespiering);
                dr["AlgemeenVoorkomen"] = ExtAlgemeenVoorkomen;
                dr["Bespiering"] = ExtBespiering;
                bldChildrenDetails.Append(dr["FarmId"].ToString() + ";" + dr["AniId"].ToString() + ";" + dr["AniLifeNumber"].ToString() + ";AlgemeenVoorkomen:" + ExtAlgemeenVoorkomen.ToString() + ";Bespiering:" + ExtBespiering.ToString() + "\r\n");


                StringBuilder QRY_Children = new StringBuilder();
                QRY_Children.Append(" SELECT * ");
                QRY_Children.Append(" FROM ANIMAL");
                QRY_Children.Append(" INNER JOIN EXTERIEUR ");
                QRY_Children.Append(" ON EXTERIEUR.aniId = ANIMAL.AniId ");
                QRY_Children.AppendFormat(" WHERE ANIMAL.AniIdMother = {0}  AND ANIMAL.AniId > 0 ", AniId);
                QRY_Children.Append(" AND EXTERIEUR.ExtKind > 1 ");
                //QRY_Children.Append(" AND EXTERIEUR.extDatum > DATE_ADD(ANIMAL.AniBirthDate,INTERVAL 1 YEAR) ");
                QRY_Children.Append(" GROUP BY ANIMAL.AniId ");
                DataTable tblchildren = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
                int Years = int.Parse(dr["Leeftijd"].ToString());
                if (dr["Afvoerdatum"].GetType() != typeof(DBNull))
                {
                    Years = Event_functions.getDatumFormat(dr["Afvoerdatum"], "Afvoerdatum").Year - Event_functions.getDatumFormat(dr["AniBirthDate"], "AniBirthDate").Year;
                }



                dr["LamOoiDefinitief"] = tblchildren.Select("AniSex = 2").GroupBy(row => row["AniId"]).Count();
                dr["LamRamDefinitief"] = tblchildren.Select("AniSex = 1").GroupBy(row => row["AniId"]).Count();
                int KinderenDefinief = tblchildren.Rows.Count;
                if (KinderenDefinief < 2)
                {
                    if (dr["VorigPredikaat"].GetType() == typeof(DBNull))
                    {
                        tbl.Rows.Remove(dr);
                    }
                    bldChildrenDetails.Append("Minder dan 2 kinderen\r\n");
                    unLogger.WriteDebug("Minder dan 2 kinderen definief gekeurd(4.d)");
                    continue;
                }
                else if (KinderenDefinief < Years - 2)
                {
                    if (dr["VorigPredikaat"].GetType() == typeof(DBNull))
                    {
                        tbl.Rows.Remove(dr);
                    }
                    bldChildrenDetails.Append("aantal nakomelingen minder dan leeftijd -2\r\n");
                    unLogger.WriteDebug("Te weinig kinderen definief gekeurd (4.d)");
                    continue;
                }
                else
                {

                    QRY_Children = new StringBuilder();
                    QRY_Children.Append(" SELECT Count(e.EventId) AS Lammeren FROM agrobase_sheep.EVENT  e ");
                    QRY_Children.Append(" JOIN agrobase_sheep.BIRTH b ON b.EventId=e.EventId  ");
                    QRY_Children.Append(" LEFT JOIN agrobase_sheep.ANIMAL a ON a.AniID=b.CalfId ");
                    QRY_Children.AppendFormat(" WHERE e.AniId={0} AND e.EveKind=5 AND e.EventId>0 ", AniId);
                    DataTable tblaantal = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
                    int Lammeren = int.Parse(tblaantal.Rows[0][0].ToString());
                    dr["Lammeren"] = Lammeren;
                    int Worpen = int.Parse(dr["Worpen"].ToString());
                    if (dr["EersteWorp"] != DBNull.Value)
                    {
                        if (dr["AniBirthDate"] != DBNull.Value)
                        {
                           DateTime eersteworp =  Event_functions.getDatumFormat(dr["EersteWorp"], "Eersteworp");
                           DateTime geboorte = Event_functions.getDatumFormat(dr["AniBirthDate"], "AniBirthDate");
                           if (eersteworp > DateTime.MinValue.AddYears(100) && geboorte > DateTime.MinValue.AddYears(100))
                           {
                               if (eersteworp.Subtract(geboorte).Days < 365)
                               {
                                   //weet niet of dit zo mag Artikel 7:4
                                   //dr["Lammeren"] = Lammeren - 1;
                                   //dr["Worpen"] = Worpen - 1;
                               }
                           }
                        }
                    }

                    QRY_Children = new StringBuilder();
                    QRY_Children.Append(" SELECT * ");
                    QRY_Children.Append(" FROM ANIMAL");
                    QRY_Children.AppendFormat(" WHERE ANIMAL.AniIdMother = {0}  AND ANIMAL.AniId > 0 ", AniId);
                    tblchildren = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
               
                    dr["LamOoi"] = tblchildren.Select("AniSex = 2").Count();
                    dr["LamRam"] = tblchildren.Select("AniSex = 1").Count();

                    int bespiering = tblchildren.Rows.Count;
                    int punten = 0;
                    int extrapunten = 0;
                    unLogger.WriteDebug("---------------------------------------------------------------------");

                    foreach (DataRow drchildren in tblchildren.Rows)
                    {
                        int ChildId = Convert.ToInt32(drchildren["AniId"]);
                        int AniSex = Convert.ToInt32(drchildren["AniSex"]);
                        unLogger.WriteDebug("Nakomeling " + drchildren["AniLifeNumber"].ToString());
                        bldChildrenDetails.Append("#" + ChildId.ToString() + ":");

                        if (!ExterieurCheckTESNakomeling(pToken, ChildId, AniSex, ref punten))
                        {
                            bespiering = bespiering - 1;
                            bldChildrenDetails.Append("Bespiering:-1:Nakomelingen:" + SbldExterieurCheckTexelaarNakomelingTelling.ToString() + ":Predikaat:0:");
                            if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                            {
                                SbldExterieurCheckTexelaarNakomelingTelling.Remove(0, SbldExterieurCheckTexelaarNakomelingTelling.Length);
                            }
                            continue;
                        }
                        if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                        {
                            bldChildrenDetails.Append("Nakomelingen:" + SbldExterieurCheckTexelaarNakomelingTelling.ToString() + ":");
                            SbldExterieurCheckTexelaarNakomelingTelling.Remove(0, SbldExterieurCheckTexelaarNakomelingTelling.Length);
                        }

                        StringBuilder QRY_PredikatenKinderen = new StringBuilder();
                        QRY_PredikatenKinderen.Append(" SELECT * ");
                        QRY_PredikatenKinderen.Append(" FROM ANIMAL");
                        QRY_PredikatenKinderen.AppendFormat(" WHERE ANIMAL.AniId = {0} ", ChildId);
                        QRY_PredikatenKinderen.AppendFormat(" AND NOT ISNULL(ANIMAL.AniPredikaat)");
                        DataTable PredikatenKinderen = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_PredikatenKinderen, MissingSchemaAction.Add);
                        int predikaatteller = 0;
                        foreach (DataRow PredikaatKinderen in PredikatenKinderen.Rows)
                        {
                            int type = Convert.ToInt32(PredikaatKinderen["AniPredikaat"]);
                            switch (type)
                            {
                                case 25:
                                    extrapunten += 10;
                                    predikaatteller += 10;
                                    unLogger.WriteDebug(" Predikaat extrapunten +10");
                                    break;
                                case 27:
                                    extrapunten += 5;
                                    predikaatteller += 5;
                                    unLogger.WriteDebug(" Predikaat extrapunten +5");
                                    break;
      
                                default:
                                    break;
                            }
                        }
                        bldChildrenDetails.Append("Predikaat:" + predikaatteller.ToString() + ":");
                        unLogger.WriteDebug("---------------------------------------------------------------------");

                    }
                    bldChildrenDetails.Append("\r\n");
                    if (punten < 13)
                    {
                        unLogger.WriteDebug("Dier heeft onvoldoende punten. ");
                        if (dr["VorigPredikaat"].GetType() == typeof(DBNull))
                        {
                            tbl.Rows.Remove(dr);
                        }
                        continue;
                    }
                    //else if (dr["OudePunten"].GetType() != typeof(DBNull) && Convert.ToInt32(dr["OudePunten"]) == (punten + extrapunten))
                    //{
                    //    //dr["NieuwePunten"] = 0;// String.Empty;
                    //    tbl.Rows.Remove(dr);
                    //    unLogger.WriteDebug("Aantal punten gelijk aan oude punten\r\n");
                    //    //bldChildrenDetails.Append("NieuwePunten:0 of aantal punten gelijk aan oude punten\r\n");
                    //}
                    else
                    {
                        int puntentotaal = (punten + extrapunten);
                        dr["Punten"] = puntentotaal;
                        StringBuilder QRY_Predikaten = new StringBuilder();
                        QRY_Predikaten.Append(" SELECT * ");
                        QRY_Predikaten.Append(" FROM agrofactuur.LABELS AS PREDIKAAT");
                        QRY_Predikaten.Append(" WHERE PREDIKAAT.LabKind = 90 AND PREDIKAAT.LabCountry = 528 ");
                        QRY_Predikaten.Append(" ORDER BY LabId");
                        DataTable tblPredikaten = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken, QRY_Predikaten);

                        if (puntentotaal >= 13)
                        {
                            dr["NieuwPredikaat"] = tblPredikaten.Rows[0]["LabLabel"].ToString();
                            if (puntentotaal >= 20)
                            {
                                dr["NieuwPredikaat"] = tblPredikaten.Rows[1]["LabLabel"].ToString();
                                if (puntentotaal > 30)// >30 voor Beltes was eerst >=30
                                {
                                    dr["NieuwPredikaat"] = tblPredikaten.Rows[2]["LabLabel"].ToString();
                                }
                            }
                        }

                        //bldChildrenDetails.Append("NieuwePunten:punten=" + punten.ToString() +  " extrapunten:"  + extrapunten.ToString() + "\r\n");
                    }

                }

            }
            if (bldChildrenDetails.Length > 0)
            {
                writeChildFile(bldChildrenDetails.ToString(), false);
            }
            return tbl;
        }

        private bool CheckTESProductie(UserRightsToken pToken, int AniId)
        {
            StringBuilder QRY_Productie = new StringBuilder();
            QRY_Productie.Append(" SELECT ANIMAL.AniId, ANIMAL.AniBirthDate");
            QRY_Productie.Append(" FROM  ANIMAL");
            QRY_Productie.Append(" LEFT OUTER JOIN MOVEMENT");
            QRY_Productie.Append(" ON (ANIMAL.AniId = MOVEMENT.AniId) AND MOVEMENT.MovKind IN (2,3)");   
            QRY_Productie.Append(" LEFT OUTER JOIN EVENT");
            QRY_Productie.Append(" ON EVENT.AniId = ANIMAL.AniId AND EVENT.EveKind=5 ");
            QRY_Productie.Append(" LEFT OUTER JOIN BIRTH");
            QRY_Productie.Append(" ON EVENT.EventId = BIRTH.EventId");
            QRY_Productie.Append(" AND BIRTH.BornDead = false ");
            QRY_Productie.AppendFormat(" WHERE ANIMAL.AniId ={0} \r\n", AniId);
            QRY_Productie.Append(" AND ANIMAL.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 4 YEAR) ");
            QRY_Productie.Append(" GROUP BY  ANIMAL.AniId");
            //QRY_Productie.Append(" HAVING  IF(ISNULL(MAX(MOVEMENT.MovDate)),(((DATEDIFF(CURDATE(), ANIMAL.AniBirthDate)) / 365) * 1.5) <= COUNT(*),(((DATEDIFF(MAX(MOVEMENT.MovDate), ANIMAL.AniBirthDate)) / 365)* 1.5) <= COUNT(*))");
            QRY_Productie.Append(" HAVING IF(ISNULL(MAX(MOVEMENT.MovDate)),");
 	        QRY_Productie.Append(" IF((DATEDIFF(CURDATE(), ANIMAL.AniBirthDate) / 365) = COUNT(DISTINCT(EVENT.EveDate)),");
            QRY_Productie.Append(" ((COUNT(DISTINCT(EVENT.EveDate))-1) * 1.5) <= COUNT(DISTINCT(EVENT.EventId)),");
            QRY_Productie.Append(" (COUNT(DISTINCT(EVENT.EveDate)) * 1.5) <= COUNT(DISTINCT(EVENT.EventId))),");
 	        QRY_Productie.Append(" IF((DATEDIFF(MAX(MOVEMENT.MovDate), ANIMAL.AniBirthDate) / 365),");
            QRY_Productie.Append(" ((COUNT(DISTINCT(EVENT.EveDate))-1) * 1.5) <= COUNT(DISTINCT(EVENT.EventId)),");
            QRY_Productie.Append(" (COUNT(DISTINCT(EVENT.EveDate)) * 1.5) <= COUNT(DISTINCT(EVENT.EventId))))");
            DataTable tbl = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Productie, MissingSchemaAction.Add);
            return tbl.Rows.Count > 0;
        }



        private bool CheckTexelaarProductie(UserRightsToken pToken, int AniId)
        {
            StringBuilder QRY_Productie = new StringBuilder();
            QRY_Productie.Append(" SELECT ANIMAL.AniId");
            QRY_Productie.Append(" FROM  ANIMAL");
            QRY_Productie.Append(" LEFT OUTER JOIN EVENT");
            QRY_Productie.Append(" ON EVENT.AniId = ANIMAL.AniId");
            QRY_Productie.Append(" AND EVENT.EveKind = 5");
            QRY_Productie.Append(" LEFT OUTER JOIN BIRTH");
            QRY_Productie.Append(" ON EVENT.EventId = BIRTH.EventId");
            QRY_Productie.Append(" AND BIRTH.BornDead = false ");
            QRY_Productie.AppendFormat(" WHERE ANIMAL.AniId ={0} \r\n", AniId);
            QRY_Productie.Append(" AND( ANIMAL.AniBirthDate > DATE_SUB(CURDATE(),INTERVAL 6 YEAR)");
            QRY_Productie.Append(" OR EVENT.EveDate <= DATE_Add(ANIMAL.AniBirthDate,INTERVAL 6 YEAR))");
            QRY_Productie.Append(" GROUP BY  ANIMAL.AniId");
            QRY_Productie.Append(" HAVING (");
            QRY_Productie.Append(" (COUNT(DISTINCT(EVENT.EveDate)) >= 3 AND ");
            QRY_Productie.Append(" (( ");
            QRY_Productie.Append(" COUNT(DISTINCT(EVENT.EveDate)) = 3 AND ");
            QRY_Productie.Append(" COUNT(EVENT.EventId) >= 5 ");
            QRY_Productie.Append(" )  ");
            QRY_Productie.Append(" OR ");
            QRY_Productie.Append(" ( ");
            QRY_Productie.Append(" COUNT(DISTINCT(EVENT.EveDate)) = 4 AND ");
            QRY_Productie.Append(" COUNT(EVENT.EventId) >= 7 ");
            QRY_Productie.Append(" )  ");
            QRY_Productie.Append(" OR ");
            QRY_Productie.Append(" ( ");
            QRY_Productie.Append(" COUNT(DISTINCT(EVENT.EveDate)) = 5 AND ");
            QRY_Productie.Append(" COUNT(EVENT.EventId) >= 9 ");
            QRY_Productie.Append(" )  ");
            QRY_Productie.Append(" OR ");
            QRY_Productie.Append(" ( ");
            QRY_Productie.Append(" COUNT(DISTINCT(EVENT.EveDate)) = 6 AND ");
            QRY_Productie.Append(" COUNT(EVENT.EventId) >= 11 ");
            QRY_Productie.Append(" ))");
            QRY_Productie.Append(" )");
            QRY_Productie.Append(" )   ");
            DataTable tbl = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Productie, MissingSchemaAction.Add);
            return tbl.Rows.Count > 0;
        }

        //private bool ExterieurCheckTexelaarNakomeling(UserRightsToken pToken, int AniId, int AniSex, ref int punten)
        //{
        //    List<EXTERIEUR> ExterieurList;
        //    List<EXTERIEUR_WAARDEN> ExterieurWaardeList;
        //    ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).Where(ext => ext.ExtKind > 1).ToList();
        //    if (ExterieurList.Count == 0)
        //    {
        //        return false;
        //    }

        //    foreach (EXTERIEUR Exterieur in ExterieurList)
        //    {
        //        try
        //        {
        //            ExterieurWaardeList = Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(Exterieur.ExtId);
        //            if (ExterieurWaardeList.Count(extw => extw.ExtwType == 4 && Convert.ToInt32(extw.ExtwWaarde) > 80) > 0)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}


        private bool ExterieurCheckTESNakomeling(UserRightsToken pToken, int AniId, int AniSex, ref int punten)
        {
            int puntennakomeling = 0;
            bool result = false;
            List<EXTERIEUR> ExterieurList;
            List<EXTERIEUR_WAARDEN> ExterieurWaardeList;
            if (AniSex == 1)
                ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).OrderBy(ext => ext.ExtDatum).ToList();
            else
                ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).Where(ext => ext.ExtKind > 1).OrderBy(ext => ext.ExtDatum).ToList();
            if (ExterieurList.Count == 0)
            {
                if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                {
                    SbldExterieurCheckTexelaarNakomelingTelling.Append(puntennakomeling.ToString());
                }
                return false;
            }

            foreach (EXTERIEUR Exterieur in ExterieurList)
            {
                try
                {
                    ExterieurWaardeList = Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(Exterieur.ExtId);
                    if (Exterieur.ExtKind == 1 && AniSex == 1)
                    {
                        foreach (EXTERIEUR_WAARDEN extw in ExterieurWaardeList.Where(extw => extw.ExtwType == 9))
                        {
                            if (Convert.ToInt32(extw.ExtwWaarde) >= 80)
                            {
                                if (Convert.ToInt32(extw.ExtwWaarde) >= 85)
                                {
                                    puntennakomeling = 2;
                                    unLogger.WriteDebug(" Voorlopig gekeurd Algemeen voorkomen > 85  punten +2");
                                }
                                else
                                {
                                    puntennakomeling = 1;
                                    unLogger.WriteDebug(" Voorlopig gekeurd Algemeen voorkomen > 80  punten +1");
                                }
                            }
                        }

                    }
                    else
                    {
                        if (ExterieurWaardeList.Count(extw => extw.ExtwType == 4 && Convert.ToInt32(extw.ExtwWaarde) > 80) > 0)
                        {
                            result = true;
                        }

                        foreach (EXTERIEUR_WAARDEN extw in ExterieurWaardeList.Where(extw => extw.ExtwType == 9))
                        {
                            if (Convert.ToInt32(extw.ExtwWaarde) >= 80)
                            {
                                if (Convert.ToInt32(extw.ExtwWaarde) >= 85)
                                {
                                    if (Convert.ToInt32(extw.ExtwWaarde) >= 90)
                                    {
                                        if (AniSex == 1)
                                        {
                                            unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 90  punten +8");
                                            puntennakomeling = 8;
                                        }
                                        else
                                        {
                                            unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 90  punten +7");
                                            puntennakomeling = 7;
                                        }
                                    }
                                    else if (AniSex == 1)
                                    {
                                        unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 85  punten +6");
                                        puntennakomeling = 6;
                                    }
                                    else
                                    {
                                        unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 85  punten +5");
                                        puntennakomeling = 5;
                                    }
                                }
                                else if (AniSex == 1)
                                {
                                    unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 80  punten +3");
                                    puntennakomeling = 3;
                                }
                                else
                                {
                                    unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 80  punten +1");
                                    puntennakomeling = 1;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    unLogger.WriteInfo("Berekening SterOoi", ex);
                }
            }
            if (SbldExterieurCheckTexelaarNakomelingTelling != null)
            {
                SbldExterieurCheckTexelaarNakomelingTelling.Append(puntennakomeling.ToString());
            }

            unLogger.WriteDebug(" Totaal nakomeling: +" + puntennakomeling.ToString());
            punten += puntennakomeling;
            return result;
        }



        private bool ExterieurCheckTexelaarNakomeling(UserRightsToken pToken, int AniId, int AniSex, ref int punten)
        {
            int puntennakomeling = 0;
            bool result = false;
            List<EXTERIEUR> ExterieurList;
            List<EXTERIEUR_WAARDEN> ExterieurWaardeList;
            if (AniSex == 1)
                ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).OrderBy(ext => ext.ExtDatum).ToList();
            else
                ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).Where(ext => ext.ExtKind > 1).OrderBy(ext => ext.ExtDatum).ToList();
            if (ExterieurList.Count == 0)
            {
                if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                {
                    SbldExterieurCheckTexelaarNakomelingTelling.Append(puntennakomeling.ToString());
                }
                return false;
            }

            foreach (EXTERIEUR Exterieur in ExterieurList)
            {
                try
                {
                    ExterieurWaardeList = Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(Exterieur.ExtId);
                    if (Exterieur.ExtKind == 1 && AniSex == 1)
                    {
                        foreach (EXTERIEUR_WAARDEN extw in ExterieurWaardeList.Where(extw => extw.ExtwType == 9))
                        {
                            if (Convert.ToInt32(extw.ExtwWaarde) >= 80)
                            {
                                if (Convert.ToInt32(extw.ExtwWaarde) >= 85)
                                {
                                    puntennakomeling = 2;
                                    unLogger.WriteDebug(" Voorlopig gekeurd Algemeen voorkomen > 85  punten +2");
                                }
                                else
                                {
                                    puntennakomeling = 1;
                                    unLogger.WriteDebug(" Voorlopig gekeurd Algemeen voorkomen > 80  punten +1");
                                }
                            }
                        }

                    }
                    else
                    {
                        if (ExterieurWaardeList.Count(extw => extw.ExtwType == 4 && Convert.ToInt32(extw.ExtwWaarde) > 80) > 0)
                        {
                            result = true;
                        }

                        foreach (EXTERIEUR_WAARDEN extw in ExterieurWaardeList.Where(extw => extw.ExtwType == 9))
                        {
                            if (Convert.ToInt32(extw.ExtwWaarde) >= 75)
                            {
                                if (Convert.ToInt32(extw.ExtwWaarde) >= 80)
                                {
                                    if (Convert.ToInt32(extw.ExtwWaarde) >= 85)
                                    {
                                        if (AniSex == 1)
                                        {
                                            unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 85  punten +6");
                                            puntennakomeling = 6;
                                        }
                                        else
                                        {
                                            unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 85  punten +5");
                                            puntennakomeling = 5;
                                        }
                                    }
                                    else if (AniSex == 1)
                                    {
                                        unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 80  punten +4");
                                        puntennakomeling = 4;
                                    }
                                    else
                                    {
                                        unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 80  punten +3");
                                        puntennakomeling = 3;
                                    }
                                }
                                else if (AniSex == 1)
                                {
                                    unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 75  punten +3");
                                    puntennakomeling = 2;
                                }
                                else
                                {
                                    unLogger.WriteDebug(" Definitief gekeurd Algemeen voorkomen > 75  punten +1");
                                    puntennakomeling = 1;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    unLogger.WriteInfo("Berekening SterOoi", ex);
                }
            }
            if (SbldExterieurCheckTexelaarNakomelingTelling != null)
            {
                SbldExterieurCheckTexelaarNakomelingTelling.Append(puntennakomeling.ToString());
            }

            unLogger.WriteDebug(" Totaal nakomeling: +" + puntennakomeling.ToString());
            punten += puntennakomeling;
            return result;
        }

        private bool ExterieurCheckTexelaar(UserRightsToken pToken, int AniId, ref int AV, ref int Bespiering)
        {
            List<EXTERIEUR> ExterieurList;
            List<EXTERIEUR_WAARDEN> ExterieurWaardeList;
            ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).Where(ext => ext.ExtKind > 1).ToList();
            if (ExterieurList.Count == 0)
            {
                return false;
            }

            foreach (EXTERIEUR Exterieur in ExterieurList)
            {
                try
                {

                    ExterieurWaardeList = Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(Exterieur.ExtId);
                    if (ExterieurWaardeList.Count(extw => extw.ExtwType == 9 && Convert.ToInt32(extw.ExtwWaarde) > 80) > 0 &&
                        ExterieurWaardeList.Count(extw => extw.ExtwType == 4 && Convert.ToInt32(extw.ExtwWaarde) > 80) > 0)
                    {
                        AV = Convert.ToInt32((from extw in Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(ExterieurList.First().ExtId)
                                              where extw.ExtwType == 9
                                              select extw.ExtwWaarde).First());
                        Bespiering = Convert.ToInt32((from extw in Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(ExterieurList.First().ExtId)
                                                      where extw.ExtwType == 4
                                                      select extw.ExtwWaarde).First());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }


        #endregion
        #region  BleuDuMaine


        public DataTable BerekenSterooiBleuDuMaine(UserRightsToken pToken, int? Stamboek, int? FarmId)
        {
            StringBuilder QRY_Sterooi = new StringBuilder();
            QRY_Sterooi.Append(" SELECT agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,agrofactuur.BEDRIJF.FarmId,  ANIMAL.AniLifeNumber,  ANIMAL.AniBirthDate,  ");
            QRY_Sterooi.Append(" ANIMAL.AniId,  MAX(EVENT.EveDate) AS LaatsteWorp,");
            QRY_Sterooi.Append(" COUNT(DISTINCT(EVENT.EveDate)) as Worpen,  0 AS Lammeren,  ");
            QRY_Sterooi.Append(" 0 AS AlgemeenVoorkomen, 0 AS Bespiering, ");
            QRY_Sterooi.Append(" 0 AS LamOoi, 0 AS LamRam,");
            QRY_Sterooi.Append(" 0 AS LamOoiDefinitief, 0 AS LamRamDefinitief,");
            QRY_Sterooi.Append(" 0 AS NieuwePunten, ANIMALPREDIKAAT.PreScore AS OudePunten");
            QRY_Sterooi.Append(" FROM ANIMAL ");
            QRY_Sterooi.Append(" JOIN ANIMALCATEGORY  ON ANIMAL.AniId = ANIMALCATEGORY.AniId  ");
            QRY_Sterooi.Append(" INNER JOIN agrofactuur.BEDRIJF  ON agrofactuur.BEDRIJF.FarmId = ANIMALCATEGORY.FarmId  ");
            QRY_Sterooi.Append(" INNER JOIN agrofactuur.UBN  ON agrofactuur.BEDRIJF.UBNid = agrofactuur.UBN.UBNid  ");
            QRY_Sterooi.Append(" INNER JOIN EXTERIEUR  ON EXTERIEUR.aniId = ANIMAL.AniId   AND EXTERIEUR.ExtKind > 1");
            QRY_Sterooi.Append(" LEFT JOIN ANIMALPREDIKAAT ");
            QRY_Sterooi.Append(" ON ANIMALPREDIKAAT.PreAniId = ANIMAL.AniId");
            QRY_Sterooi.Append(" AND ANIMALPREDIKAAT.PreTypePredikaat IN (8,9,10,11)");///BUG 1460
            QRY_Sterooi.Append(" AND PreBegindatum <= CURRENT_DATE()");
            QRY_Sterooi.Append(" AND (PreEinddatum >= CURRENT_DATE() OR ISNULL(PreEinddatum))");
            QRY_Sterooi.Append(" INNER JOIN EVENT");
            QRY_Sterooi.Append("  ON (ANIMAL.AniId = EVENT.AniId)  AND EVENT.EveKind = 5  ");
            QRY_Sterooi.Append(" INNER JOIN BIRTH");
            QRY_Sterooi.Append("  ON (BIRTH.EventId = EVENT.EventId)  AND BIRTH.BornDead = false  AND BIRTH.CalfId > 0 ");
            QRY_Sterooi.Append("   WHERE (DATEDIFF(EVENT.EveDate, ANIMAL.AniBirthDate) > 465) ");
            QRY_Sterooi.Append("	 AND ANIMAL.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 4 YEAR) ");
            QRY_Sterooi.Append("	  AND ANIMALCATEGORY.Anicategory < 5 ");
            QRY_Sterooi.Append("      AND ANIMAL.AniSex = 2 AND ANIMAL.AniId > 0 ");
            if (Stamboek.HasValue)
                QRY_Sterooi.AppendFormat("	  AND agrofactuur.BEDRIJF.Programid IN ({0}) ", Stamboek.Value);
            if (FarmId.HasValue)
                QRY_Sterooi.AppendFormat("	  AND agrofactuur.BEDRIJF.FarmId IN ({0}) ", FarmId.Value);
            QRY_Sterooi.Append("	  GROUP BY agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,  ");
            QRY_Sterooi.Append("	  ANIMAL.AniLifeNumber,  ANIMAL.AniId,  ANIMAL.AniBirthDate ");
            QRY_Sterooi.Append("  HAVING ((DATEDIFF(CURDATE(), ANIMAL.AniBirthDate)) / 365) <= COUNT(*)  ");
            QRY_Sterooi.Append(" AND");
            QRY_Sterooi.Append(" (");
            QRY_Sterooi.Append(" SELECT COUNT(*)  ");
            QRY_Sterooi.Append(" FROM ANIMAL AS LAM  ");
            QRY_Sterooi.Append(" INNER JOIN EXTERIEUR AS LAMEXTERIEUR  ");
            QRY_Sterooi.Append(" ON LAMEXTERIEUR.aniId = LAM.AniId   ");
            QRY_Sterooi.Append(" WHERE LAM.AniIdMother = ANIMAL.AniId  ");
            QRY_Sterooi.Append(" AND LAMEXTERIEUR.ExtKind > 1   ");
            QRY_Sterooi.Append(" )");
            QRY_Sterooi.Append(" >= (((DATEDIFF(MAX(EVENT.EveDate), ANIMAL.AniBirthDate)) / 365) - 2)");
            QRY_Sterooi.Append("  AND COUNT(DISTINCT(EVENT.EveDate)) <= (COUNT(EVENT.EventId) * 2) ");

            DataTable tbl = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Sterooi, MissingSchemaAction.Add);

            List<DataRow> drList = new List<DataRow>();

            drList.AddRange(tbl.Select());
            StringBuilder bldChildrenDetails = new StringBuilder();
            foreach (DataRow dr in drList)
            {
                unLogger.WriteDebug("###############################################################################");
                unLogger.WriteDebug("Berekening voor Dier :" + dr["AniLifeNumber"].ToString());

                double perc = (drList.IndexOf(dr) / (Convert.ToDouble(drList.Count) / 100));
                Facade.GetInstance().UpdateProgress(Convert.ToInt32(perc), String.Format("Berekenen... {0} % ", Convert.ToInt32(perc)));


                int AniId = Convert.ToInt32(dr["AniId"]);

                int AlgemeenVoorkomen = 0;
                int Bespiering = 0;

                if (!ExterieurCheckBleuDuMaine(pToken, AniId, ref AlgemeenVoorkomen, ref Bespiering))
                {
                    tbl.Rows.Remove(dr);
                    continue;
                }

                dr["AlgemeenVoorkomen"] = AlgemeenVoorkomen;
                dr["Bespiering"] = Bespiering;

                bldChildrenDetails.Append(dr["FarmId"].ToString() + ";" + dr["AniId"].ToString() + ";" + dr["AniLifeNumber"].ToString() + ";AlgemeenVoorkomen:" + AlgemeenVoorkomen.ToString() + ";Bespiering:" + Bespiering.ToString() + "\r\n");

                StringBuilder QRY_Children = new StringBuilder();
                QRY_Children.Append(" SELECT * ");
                QRY_Children.Append(" FROM ANIMAL");
                QRY_Children.AppendFormat(" WHERE ANIMAL.AniIdMother = {0}  AND ANIMAL.AniId > 0 ", AniId);
                DataTable tblchildren = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
                dr["Lammeren"] = tblchildren.Rows.Count;
                dr["LamOoi"] = tblchildren.Select("AniSex = 2").Count();
                dr["LamRam"] = tblchildren.Select("AniSex = 1").Count();

                QRY_Children = new StringBuilder();
                QRY_Children.Append(" SELECT * ");
                QRY_Children.Append(" FROM ANIMAL");
                QRY_Children.Append(" INNER JOIN EXTERIEUR ");
                QRY_Children.Append(" ON EXTERIEUR.aniId = ANIMAL.AniId ");
                QRY_Children.AppendFormat(" WHERE ANIMAL.AniIdMother = {0}  AND ANIMAL.AniId > 0 ", AniId);
                QRY_Children.Append(" AND EXTERIEUR.ExtKind > 1 ");
                QRY_Children.Append(" GROUP BY ANIMAL.AniId ");
                tblchildren = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Children);
                int Years = DateTime.Today.Year - Event_functions.getDatumFormat(dr["AniBirthDate"], "AniBirthDate").Year;
                dr["LamOoiDefinitief"] = tblchildren.Select("AniSex = 2").GroupBy(row => row["AniId"]).Count();
                dr["LamRamDefinitief"] = tblchildren.Select("AniSex = 1").GroupBy(row => row["AniId"]).Count();
                if (tblchildren.Rows.Count < 2)
                {
                    tbl.Rows.Remove(dr);
                    bldChildrenDetails.Append("Minder dan 2 kinderen\r\n");
                    continue;
                }
                else if (tblchildren.Rows.Count < Years - 2)
                {
                    tbl.Rows.Remove(dr);
                    bldChildrenDetails.Append("kinderen zijn jonger dan 2 jaar\r\n");
                    continue;
                }
                else
                {
                    unLogger.WriteDebug("---------------------------------------------------------------------");
                    int punten = 0;
                    int extrapunten = 0;
                    foreach (DataRow drchildren in tblchildren.Rows)
                    {
                        unLogger.WriteDebug("Nakomeling " + drchildren["AniLifeNumber"].ToString());
                        int ChildId = Convert.ToInt32(drchildren["AniId"]);
                        int AniSex = Convert.ToInt32(drchildren["AniSex"]);
                        ExterieurCheckBleuDuMaineNakomeling(pToken, ChildId, AniSex, ref punten);
                        bldChildrenDetails.Append("#" + ChildId.ToString() + ":");
                        if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                        {
                            bldChildrenDetails.Append("Nakomelingen:" + SbldExterieurCheckTexelaarNakomelingTelling.ToString() + ":");
                            SbldExterieurCheckTexelaarNakomelingTelling.Remove(0, SbldExterieurCheckTexelaarNakomelingTelling.Length);
                        }

                        StringBuilder QRY_PredikatenKinderen = new StringBuilder();
                        QRY_PredikatenKinderen.Append(" SELECT * ");
                        QRY_PredikatenKinderen.Append(" FROM ANIMAL");
                        QRY_PredikatenKinderen.Append(" INNER JOIN ANIMALPREDIKAAT ");
                        QRY_PredikatenKinderen.Append(" ON ANIMALPREDIKAAT.PreAniId = ANIMAL.AniId ");
                        QRY_PredikatenKinderen.AppendFormat(" WHERE ANIMAL.AniId = {0} ", ChildId);
                        QRY_PredikatenKinderen.Append(" AND PreBegindatum <= CURRENT_DATE()");
                        QRY_PredikatenKinderen.Append(" AND (PreEinddatum >= CURRENT_DATE() OR ISNULL(PreEinddatum))");
                        DataTable PredikatenKinderen = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_PredikatenKinderen, MissingSchemaAction.Add);
                        int xtratelling = 0;
                        foreach (DataRow PredikaatKinderen in PredikatenKinderen.Rows)
                        {
                            int type = Convert.ToInt32(PredikaatKinderen["PreTypePredikaat"]);
                            switch (type)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    extrapunten += 2;
                                    xtratelling += 2;
                                    break;
                                case 4:
                                    extrapunten += 4;
                                    xtratelling += 4;

                                    break;
                                case 5:
                                case 6:
                                case 7:
                                case 13:
                                    extrapunten += 10;
                                    xtratelling += 10;
                                    break;
                                default:
                                    break;
                            }
                        }
                        bldChildrenDetails.Append("Predikaat:" + xtratelling.ToString() + ":");
                        unLogger.WriteDebug("---------------------------------------------------------------------");
                    }
                    bldChildrenDetails.Append("\r\n");

                    if (punten < 13)
                    {
                        tbl.Rows.Remove(dr);
                        //bldChildrenDetails.Append("Aantal punten<13\r\n");
                        continue;
                    }
                    else if (dr["OudePunten"].GetType() != typeof(DBNull) && Convert.ToInt32(dr["OudePunten"]) == (punten + extrapunten))
                    {
                        //dr["NieuwePunten"] = 0;// String.Empty;
                        tbl.Rows.Remove(dr);
                        //bldChildrenDetails.Append("NieuwePunten:0 Aantal punten gelijk aan oude punten\r\n");
                    }
                    else
                    {
                        dr["NieuwePunten"] = (punten + extrapunten);
                        //bldChildrenDetails.Append("NieuwePunten:punten=" + punten.ToString() + " extrapunten:" + extrapunten.ToString() + "\r\n");
                    }

                }
            }
            if (bldChildrenDetails.Length > 0)
            {
                writeChildFile(bldChildrenDetails.ToString(), false);
            }
            return tbl;
        }

        private bool ExterieurCheckBleuDuMaineNakomeling(UserRightsToken pToken, int AniId, int AniSex, ref int punten)
        {
            int lTemppunten = 0;
            int puntennakomeling = 0;
            bool result = false;
            List<EXTERIEUR> ExterieurList;
            List<EXTERIEUR_WAARDEN> ExterieurWaardeList;
            if (AniSex == 1)
                ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId);
            else
                ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).Where(ext => ext.ExtKind > 1).ToList();
            if (ExterieurList.Count == 0)
            {
                if (SbldExterieurCheckTexelaarNakomelingTelling != null)
                {
                    SbldExterieurCheckTexelaarNakomelingTelling.Append(lTemppunten.ToString());
                }
                return false;
            }

            foreach (EXTERIEUR Exterieur in ExterieurList)
            {
                try
                {
                    ExterieurWaardeList = Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(Exterieur.ExtId);
                    if (Exterieur.ExtKind == 1 && AniSex == 1 && ExterieurList.Count == 1)
                    {
                        foreach (EXTERIEUR_WAARDEN extw in ExterieurWaardeList.Where(extw => extw.ExtwType == 9))
                        {
                            if (Convert.ToInt32(extw.ExtwWaarde) >= 80)
                            {
                                lTemppunten = 1;
                                if (Convert.ToInt32(extw.ExtwWaarde) >= 85)
                                {
                                    puntennakomeling = 2;
                                    lTemppunten = 2;
                                }
                                else puntennakomeling = 1;
                            }
                        }

                    }
                    else
                    {
                        foreach (EXTERIEUR_WAARDEN extw in ExterieurWaardeList.Where(extw => extw.ExtwType == 9))
                        {
                            if (Convert.ToInt32(extw.ExtwWaarde) >= 75)
                            {
                                if (Convert.ToInt32(extw.ExtwWaarde) >= 80)
                                {
                                    if (Convert.ToInt32(extw.ExtwWaarde) >= 85)
                                    {
                                        if (AniSex == 1) { puntennakomeling = 6; lTemppunten = 6; }
                                        else { puntennakomeling = 5; lTemppunten = 5; }
                                    }
                                    else if (AniSex == 1) { puntennakomeling = 4; lTemppunten = 4; }
                                    else { puntennakomeling = 3; lTemppunten = 3; }
                                }
                                else if (AniSex == 1) { puntennakomeling = 2; lTemppunten = 2; }
                                else { puntennakomeling = 1; lTemppunten = 1; }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    unLogger.WriteInfo("Berekening SterOoi", ex);
                }
            }
            if (SbldExterieurCheckTexelaarNakomelingTelling != null)
            {
                SbldExterieurCheckTexelaarNakomelingTelling.Append(lTemppunten.ToString());
            }

            unLogger.WriteDebug(" Totaal nakomeling: +" + puntennakomeling.ToString());
            punten += puntennakomeling;

            return result;
        }

        private bool ExterieurCheckBleuDuMaine(UserRightsToken pToken, int AniId, ref int AV, ref int Bespiering)
        {
            List<EXTERIEUR> ExterieurList;
            List<EXTERIEUR_WAARDEN> ExterieurWaardeList;
            ExterieurList = Facade.GetInstance().getSaveToDB(pToken).getExterieurByAnimal(AniId).Where(ext => ext.ExtKind > 1).ToList();
            if (ExterieurList.Count == 0)
            {
                return false;
            }

            foreach (EXTERIEUR Exterieur in ExterieurList)
            {
                try
                {
                    ExterieurWaardeList = Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(Exterieur.ExtId);
                    if (ExterieurWaardeList.Count(extw => extw.ExtwType == 9 && Convert.ToInt32(extw.ExtwWaarde) > 85) > 0 &&
                        ExterieurWaardeList.Count(extw => extw.ExtwType == 4 && Convert.ToInt32(extw.ExtwWaarde) > 85) > 0)
                    {
                        AV = Convert.ToInt32((from extw in Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(ExterieurList.First().ExtId)
                                              where extw.ExtwType == 9
                                              select extw.ExtwWaarde).First());
                        Bespiering = Convert.ToInt32((from extw in Facade.GetInstance().getSaveToDB(pToken).getExterieurWaardes(ExterieurList.First().ExtId)
                                                      where extw.ExtwType == 4
                                                      select extw.ExtwWaarde).First());

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public DataTable BerekenPreferentschapBlueDuMaine(UserRightsToken pToken, int? Stamboek, int? FarmId)
        {
            StringBuilder QRY_Preferentschap = new StringBuilder();
            QRY_Preferentschap.Append(" SELECT agrofactuur.UBN.Bedrijfsnaam,  agrofactuur.BEDRIJF.Fokkers_Nr,agrofactuur.BEDRIJF.FarmId,  ANIMAL.AniLifeNumber ");
            QRY_Preferentschap.Append(" FROM ANIMAL");
            QRY_Preferentschap.Append(" INNER JOIN ANIMALPREDIKAAT");
            QRY_Preferentschap.Append(" ON ANIMALPREDIKAAT.PreAniId = ANIMAL.AniId ");
            QRY_Preferentschap.Append(" JOIN ANIMALCATEGORY  ON ANIMAL.AniId = ANIMALCATEGORY.AniId");
            QRY_Preferentschap.Append(" INNER JOIN agrofactuur.BEDRIJF  ON agrofactuur.BEDRIJF.FarmId = ANIMALCATEGORY.FarmId ");
            QRY_Preferentschap.Append(" INNER JOIN agrofactuur.UBN  ON agrofactuur.BEDRIJF.UBNid = agrofactuur.UBN.UBNid ");
            QRY_Preferentschap.Append(" WHERE ANIMAL.AniSex = 1");
            if (Stamboek.HasValue)
                QRY_Preferentschap.AppendFormat("	  AND agrofactuur.BEDRIJF.Programid IN ({0}) ", Stamboek.Value);
            if (FarmId.HasValue)
                QRY_Preferentschap.AppendFormat("	  AND agrofactuur.BEDRIJF.FarmId IN ({0}) ", FarmId.Value);
            QRY_Preferentschap.Append(" AND ANIMALCATEGORY.Anicategory < 4 ");
            QRY_Preferentschap.Append(" AND PreTypePredikaat = 4");
            QRY_Preferentschap.Append(" AND PreBegindatum <= CURRENT_DATE()");
            QRY_Preferentschap.Append(" AND (PreEinddatum >= CURRENT_DATE() OR ISNULL(PreEinddatum))");
            QRY_Preferentschap.Append("  AND ");
            QRY_Preferentschap.Append("  (");
            QRY_Preferentschap.Append("  SELECT COUNT(DISTINCT(LAM.AniId))");
            QRY_Preferentschap.Append("  FROM ANIMAL AS LAM");
            QRY_Preferentschap.Append("  WHERE LAM.AniIdFather = ANIMAL.AniId");
            QRY_Preferentschap.Append("  AND LAM.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 1 YEAR) 	  ");
            QRY_Preferentschap.Append("  ) >= 10");
            QRY_Preferentschap.Append("   AND ");
            QRY_Preferentschap.Append("  (");
            QRY_Preferentschap.Append("  SELECT COUNT(DISTINCT(LAM.AniId))");
            QRY_Preferentschap.Append("  FROM ANIMAL AS LAM");
            QRY_Preferentschap.Append("  LEFT JOIN EVENT");
            QRY_Preferentschap.Append("  ON LAM.AniId = EVENT.AniId");
            QRY_Preferentschap.Append("  AND EVENT.EveKind = 5");
            QRY_Preferentschap.Append("  WHERE LAM.AniIdFather = ANIMAL.AniId");
            QRY_Preferentschap.Append("  AND LAM.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 1 YEAR) 	  ");
            QRY_Preferentschap.Append("  AND LAM.AniSex = 2");
            QRY_Preferentschap.Append("  ");
            QRY_Preferentschap.Append("  ) >= 10");
            QRY_Preferentschap.Append(" AND (");
            QRY_Preferentschap.Append("  SELECT COUNT(DISTINCT(LAM.AniId))");
            QRY_Preferentschap.Append("  FROM ANIMAL AS LAM");
            QRY_Preferentschap.Append("  JOIN EVENT");
            QRY_Preferentschap.Append("  ON LAM.AniId = EVENT.AniId");
            QRY_Preferentschap.Append("  AND EVENT.EveKind = 5");
            QRY_Preferentschap.Append("  WHERE LAM.AniIdFather = ANIMAL.AniId");
            QRY_Preferentschap.Append("  AND LAM.AniSex = 2");
            QRY_Preferentschap.Append("  AND LAM.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 3 YEAR) 	  ");
            QRY_Preferentschap.Append("  AND EVENT.EveDate <= DATE_SUB(CURDATE(),INTERVAL 1 YEAR) 	  ");
            QRY_Preferentschap.Append("  AND (DATEDIFF(CURDATE(), LAM.AniBirthDate) > 365)");
            QRY_Preferentschap.Append(" ) >= 10");
            QRY_Preferentschap.Append("  AND ");
            QRY_Preferentschap.Append("  (");
            QRY_Preferentschap.Append("  SELECT COUNT(DISTINCT(LAM.AniId))");
            QRY_Preferentschap.Append("  FROM ANIMAL AS LAM");
            QRY_Preferentschap.Append("  WHERE LAM.AniIdFather = ANIMAL.AniId");
            QRY_Preferentschap.Append("  AND LAM.AniSex = 1");
            QRY_Preferentschap.Append("  AND LAM.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 1 YEAR) 	  ");
            QRY_Preferentschap.Append("  ) >= 4   ");
            QRY_Preferentschap.Append("	AND ");
            QRY_Preferentschap.Append("  (");
            QRY_Preferentschap.Append("  SELECT COUNT(DISTINCT(LAM.AniId))");
            QRY_Preferentschap.Append("  FROM ANIMAL AS LAM");
            QRY_Preferentschap.Append("  WHERE LAM.AniIdFather = ANIMAL.AniId");
            QRY_Preferentschap.Append("  AND LAM.AniSex = 1");
            QRY_Preferentschap.Append("  AND (DATEDIFF(CURDATE(), LAM.AniBirthDate) > 365)");
            QRY_Preferentschap.Append("  AND LAM.AniBirthDate <= DATE_SUB(CURDATE(),INTERVAL 3 YEAR) 	  ");
            QRY_Preferentschap.Append("  ) >= 2 ");
            Facade.GetInstance().UpdateProgress(20, "Lijst Samenstellen");
            return Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken.getLastChildConnection(), QRY_Preferentschap, MissingSchemaAction.Add);

        }

        #endregion

        private void writeChildFile(string pTekst, bool pAppend)
        {
            if (WriteChildrenDetails)
            {
                if (ChildrenDetailFolderPath != null && ChildrenDetailFilename != null)
                {

                    try
                    {
                        StreamWriter wr = new StreamWriter(ChildrenDetailFolderPath + ChildrenDetailFilename, pAppend);
                        try
                        {
                            wr.WriteLine(pTekst);
                            wr.Flush();
                        }
                        catch (Exception exc) { unLogger.WriteInfo(exc.ToString()); }
                        finally
                        {
                            wr.Close();
                        }
                    }
                    catch (Exception exc) { unLogger.WriteInfo(exc.ToString()); }
                }
            }
        }
    }
}
