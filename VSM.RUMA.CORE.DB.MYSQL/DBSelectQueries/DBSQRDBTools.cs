using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQRDBTools
    {
        /// <summary>
        /// DBSQRDBTools bevat alle functies die zich zowel in .NET als in DELPHI bevinden (RDBTools.pas)
        /// </summary>
        /// <remarks>
        /// 	<para>Bij het wijzingen van onderstaande functies ook de delphi implementatie aanpassen</para>
        /// </remarks>
        /// 


        private DBSelectQueries Parent;

        public DBSQRDBTools(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        public List<int> getListUbnIdOndergeschikten(int pProgramId, List<BEDRIJF> listBedrijfFromSoap)
        {
            //gebaseerd op Delphi -> rdbtools lst_getUbnIdArr

            //VERKRIJG LIJST MET BEDRIJVEN BEHORENDE BIJ DE ADMINISTRATOR/GEBRUIKER           
            // Verkrijgt ubnids op basis van programId of listBedrijf (verkregen vanuit SOAP)

            List<int> listUbnIds = new List<int>();

            if (Parent.getUbnIdOndergeschiktenType(pProgramId) == 1)
            {
                //admins op basis van programIds;
                //bepaal de ubns voor het overzicht met de programIds van de admin            
                List<int> listProgramId = Parent.getListProgramIdOndergeschikten(pProgramId);
                listUbnIds = getListUbnIdByListProgramId(listProgramId).ToList();
            }
            else
            {
                //admins zonder programIds (bijvoorbeeld Navobi en Vilatca;
                //haal de ubns voor het overzicht uit de agrobase soap
                foreach (BEDRIJF b in listBedrijfFromSoap)
                {
                    listUbnIds.Add(b.UBNid);
                }
            }

            return listUbnIds;
        }


        public IEnumerable<int> getListUbnIdByListProgramId(List<int> listProgramId)
        {
            String qry = "SELECT DISTINCT UbnId" +
                " FROM agrofactuur.BEDRIJF" +
                " WHERE (ProgramId IN (" + Parent.EnumerableToCommaSeperatedString(listProgramId) + "))" +
                " AND (UbnId > 0)";

            DataTable dt = null;
            try
            {
                dt = Parent.QueryData(qry, MissingSchemaAction.Add);
            }
            catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }

            IEnumerable<int> intList = Parent.DataTableColumnToList<int>(dt, "UbnId");
            return intList;
        }



        //*** LET OP! houdt deze up-to-date met Delphi
        //(Z:\prog\DELPHI\RUMAMYSQLDB\rdbtools.pas)
        
        private StringBuilder getQuery_Aanwezigheid(int farmId, int pUbnID, int aniId, DateTime datum, int groupId, int mede_eigenaar, int pProgId)
        {
            //Laatste update aanwezigheid-Query; 04-11-2011 (DELPHI VERSIE IS LEIDEND!)
            //Laatste update aanwezigheid-Query; 11-09-2013 
            //laatste Update aanwezigheid-Query; 06-03-2015 MovId>0 7 X toegepast bij de eerste subqueries

            string database = "agrobase.";//LET OP DE PUNT
            if (pProgId == 5)
            {
                database = "agrobase_goat.";
            }
            else if (pProgId == 3)
            {
                database = "agrobase_sheep.";
            }

            StringBuilder qry = new StringBuilder();

            string strDate = datum.ToString("yyyy-MM-dd HH:mm:ss");

            string sQ_farmId = "";
            string sQ_aniId = "";
            string sQ_groupId = "";
            string sQ_mede_eigenaar = "";

            if (farmId > 0 || pUbnID > 0)
            {
                if (pUbnID <= 0)
                {
                    sQ_farmId = " AND (ac.FarmId=" + farmId.ToString() + ") ";
                }
                else
                {
                    sQ_farmId = "  AND (ac.FarmId IN (SELECT FarmId from agrofactuur.BEDRIJF b2 WHERE b2.UbnID=" + pUbnID.ToString() + " AND b2.FarmID>0 )) ";
                }
            }
            if (aniId > 0)
                sQ_aniId = " AND (a.AniId=" + aniId.ToString() + ") ";

            if (groupId > 0)
                sQ_groupId = " AND ((SELECT Groupnr FROM " + database + "MOVEMENT " +
                                  " WHERE AniId=a.AniId AND UbnId=be.UbnId AND Groupnr > 0 " +
                                  " AND MovDate <= " + strDate +
                                  " ORDER BY MovDate DESC LIMIT 1) = " + groupId.ToString() + ") ";

            if (mede_eigenaar != 0)
                sQ_mede_eigenaar = " OR (ac.Ani_Mede_Eigenaar=1) ";

            if (datum.Date == DateTime.Today.Date)
            {
                //vandaag
                qry.Append(
                    " SELECT DISTINCT a.AniId, ac.AniCategory, a.AniBirthDate " +
                    " FROM " + database + "ANIMAL a " +
                    " LEFT JOIN " + database + "ANIMALCATEGORY ac ON (a.AniId=ac.AniId) " +
                    " LEFT JOIN agrofactuur.BEDRIJF be ON (be.FarmId=ac.FarmId) " +
                    " WHERE ((ac.AniCategory BETWEEN 0 AND 3)" + sQ_mede_eigenaar + ") " +

                    sQ_farmId +
                    sQ_aniId +
                    sQ_groupId +
                    //" GROUP BY a.AniId, ac.AniCategory ");
                    " GROUP BY a.AniId  ");
            }
            else
            {
                //Datum is uit het verleden (of toekomst). Achterhaal aanwezigheid dmv. movements + geboorte
                string sDtCol = " ADDTIME(MovDate, IF(ISNULL(MovTime), TIME('00:00'),  TIME(MovTime))) ";

                //qry.Append(" SELECT DISTINCT a.AniId, ac.AniCategory, a.AniBirthDate, a.ThrId as a_thrid, u.ThrId as u_thrid ");
                //qry.Append(" , (SELECT MIN( " + sDtCol + " ) AS dtA FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND UbnId=be.UbnId AND MovKind IN (1, 4, 7)) AS minAanvoer ");
                //qry.Append(" , (SELECT MIN( " + sDtCol + " ) AS dtB FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND UbnId=be.UbnId AND MovKind IN (2, 3, 5, 6)) AS minAfvoer ");
                //qry.Append(" , (SELECT MAX( " + sDtCol + " ) AS dtC FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND UbnId=be.UbnId AND MovKind IN (1, 4, 7) HAVING dtC <= '" + strDate + "') AS maxAanvoer ");
                //qry.Append(" , (SELECT MAX( " + sDtCol + " ) AS dtD FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND UbnId=be.UbnId AND MovKind IN (2, 3, 5, 6) HAVING dtD <= '" + strDate + "') AS maxAfvoer ");

                //qry.Append("  FROM " + database + "ANIMAL a ");
                //qry.Append("  LEFT JOIN " + database + "ANIMALCATEGORY ac ON (a.AniId=ac.AniId) ");
                //qry.Append("  LEFT JOIN agrofactuur.BEDRIJF be ON (be.FarmId=ac.FarmId) ");
                //qry.Append("  LEFT JOIN agrofactuur.UBN u ON (u.UBNid=be.UbnId) ");

                //qry.Append("  WHERE ((ac.AniCategory BETWEEN 0 AND 4)" + sQ_mede_eigenaar + ") ");

                //qry.Append(sQ_farmId);
                //qry.Append(sQ_aniId);
                //qry.Append(sQ_groupId);

                //qry.Append("  AND ((DATE(a.AniBirthDate) <= '" + strDate + "') OR ISNULL(a.AniBirthDate)) ");

                //qry.Append("  GROUP BY a.AniId, ac.AniCategory ");

                //qry.Append("  HAVING (   (ISNULL(minAanvoer) OR (minAanvoer <= '" + strDate + "'))  OR  ");
                //qry.Append("  (NOT(ISNULL(minAanvoer) AND ISNULL(minAfvoer))  ");
                //// IF statement is er voor aan en afvoer zelfde dag dan nemen we aan dat ie aanwezig is
                //// want de TIME kan niet chronologisch zijn 
                //// 
                //qry.Append("  AND (  IF( DATE(minAanvoer)=DATE(minAfvoer),DATE(minAanvoer) >= DATE(minAfvoer),minAanvoer > minAfvoer)   ))  ) ");
                ////eerste Having regel

                //qry.Append("  AND (  ISNULL(maxAfvoer) OR ");
                //qry.Append("  (NOT(ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) ");

                //qry.Append("  AND (    IF(DATE(maxAanvoer)=DATE(maxAfvoer),DATE(maxAanvoer) >= DATE(maxAfvoer),maxAanvoer>maxAfvoer)    ))  ) ");
                ////tweede Having regel

                //qry.Append("  AND NOT (ISNULL(AniBirthDate) AND ISNULL(minAanvoer) AND ISNULL(minAfvoer) AND ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) ");
                ////derde Having regel

                //qry.Append("  AND NOT( (ac.AniCategory = 4) AND (ISNULL(minAfvoer)) ) ");
                //vierde Having regel

                /////////////////////                ////////////////////////////////////////////////////////////

                qry = new StringBuilder();

                qry.AppendLine("  SELECT DISTINCT ac.AniId, ac.AniCategory, a.AniBirthDate, a.ThrId AS a_thrid, u.ThrId AS u_thrid, ");
                qry.AppendLine("  (SELECT MIN( " + sDtCol + " ) AS dtA FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR ISNULL(UbnID)) AND MovKind IN (1, 4, 7)  AND MovId>0  ) AS minAanvoer, ");
                qry.AppendLine("  (SELECT MIN( " + sDtCol + " ) AS dtB FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR ISNULL(UbnID)) AND MovKind IN (2, 3, 5, 6) AND MovId>0  ) AS minAfvoer, ");
                qry.AppendLine("  (SELECT MAX( " + sDtCol + " ) AS dtC FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR ISNULL(UbnID)) AND MovKind IN (1, 4, 7) AND MovId>0    AND MovDate <= '" + strDate + "' HAVING dtC <= '" + strDate + "') AS maxAanvoer, ");
                qry.AppendLine("  (SELECT MAX( " + sDtCol + " ) AS dtD FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR ISNULL(UbnID)) AND MovKind IN (2, 3, 5, 6) AND MovId>0  AND MovDate <= '" + strDate + "' HAVING dtD <= '" + strDate + "') AS maxAfvoer, ");
                qry.AppendLine("  (SELECT MAX( " + sDtCol + " ) AS dtE FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR isnull(UbnID)) AND MovKind IN (3) AND MovId>0  HAVING dtE <= '" + strDate + "') AS MaxDood, ");
                qry.AppendLine("  (SELECT MAX(MovID) FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR ISNULL(UbnID)) AND MovKind IN (1, 4, 7)  AND MovId>0   AND MovDate <='" + strDate + "' ) AS maxAanvoer_ID, ");
                qry.AppendLine("  (SELECT MAX(MovID) FROM " + database + "MOVEMENT WHERE AniId=a.AniId AND (UbnId=be.UbnId OR ISNULL(UbnID)) AND MovKind IN (2, 3, 5, 6) AND MovId>0  AND MovDate <='" + strDate + "' ) AS maxAfvoer_ID ");

                qry.AppendLine("  FROM " + database + "ANIMAL a ");

                qry.AppendLine("  LEFT JOIN " + database + "ANIMALCATEGORY ac ON (a.AniId=ac.AniId) ");
                qry.AppendLine("  LEFT JOIN agrofactuur.BEDRIJF be ON (be.FarmId=ac.FarmId) ");
                qry.AppendLine("  LEFT JOIN agrofactuur.UBN u ON (u.UBNid=be.UbnId) ");

                qry.AppendLine("  WHERE ((ac.AniCategory BETWEEN 0 AND 4) " + sQ_mede_eigenaar + ") ");

                qry.AppendLine(sQ_farmId);
                qry.AppendLine(sQ_aniId);
                qry.AppendLine(sQ_groupId);

                qry.AppendLine("  AND (( DATE(a.AniBirthDate) <= DATE('" + strDate + "')) OR ISNULL(a.AniBirthDate)) ");

                //qry.Append("  GROUP BY a.AniId, ac.AniCategory  HAVING (   (ISNULL(minAanvoer)  OR (minAanvoer <= '" + strDate + "'))  ");


                //qry.Append("  OR (NOT(ISNULL(minAanvoer) AND ISNULL(minAfvoer))  AND (minAanvoer > minAfvoer) ) ) ");
                //qry.Append("  OR ( (NOT(ISNULL(maxAanvoer)) AND NOT(ISNULL(maxAfvoer)))  AND (maxAanvoer = maxAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID)) ");
                //qry.Append("  OR ( (ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) AND (minAanvoer = minAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID))  ");
                //qry.Append("  AND (  ISNULL(maxAfvoer) OR (NOT(ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) AND (maxAanvoer > maxAfvoer))   ");
                //qry.Append("  OR ( NOT(ISNULL(maxAanvoer)) AND NOT(ISNULL(maxAfvoer)) AND (maxAanvoer = maxAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID)) ");
                //qry.Append("  OR ( (ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) AND (minAanvoer = minAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID)) ) ");
                //qry.Append("  AND NOT (ISNULL(AniBirthDate) AND ISNULL(minAanvoer) AND ISNULL(minAfvoer) AND ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) ");
                //qry.Append("  AND ((maxDood >= '" + strDate + "') OR ISNULL(MaxDood)) ");

                //aangepast ivm dubbele dieren 
                //qry.AppendLine("  GROUP BY a.AniId, ac.AniCategory  HAVING (   (ISNULL(minAanvoer)  OR (minAanvoer <= '" + strDate + "'))  ");
                qry.AppendLine("  GROUP BY a.AniId   HAVING (   (ISNULL(minAanvoer)  OR (minAanvoer <= '" + strDate + "'))  ");


                qry.AppendLine("  OR (NOT(ISNULL(minAanvoer) AND ISNULL(minAfvoer))  AND (minAanvoer > minAfvoer) ) OR ( (NOT(ISNULL(maxAanvoer)) AND NOT(ISNULL(maxAfvoer))) AND (maxAanvoer = maxAfvoer)  ");
                qry.AppendLine("  AND (maxAanvoer_ID > maxAfvoer_ID)) OR ( (ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) AND (minAanvoer = minAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID)) )  ");
                qry.AppendLine("  AND (  ISNULL(maxAfvoer) OR (NOT(ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) AND (maxAanvoer > maxAfvoer))    ");
                qry.AppendLine("  OR ( NOT(ISNULL(maxAanvoer)) AND NOT(ISNULL(maxAfvoer)) AND (maxAanvoer = maxAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID))  ");
                qry.AppendLine("  OR ( (ISNULL(maxAanvoer) AND ISNULL(maxAfvoer)) AND (minAanvoer = minAfvoer) AND (maxAanvoer_ID > maxAfvoer_ID)) ) AND NOT (ISNULL(AniBirthDate)  ");
                qry.AppendLine("  AND ISNULL(minAanvoer) AND ISNULL(minAfvoer) AND ISNULL(maxAanvoer) AND ISNULL(maxAfvoer))  ");
                qry.AppendLine(" AND NOT( (ac.AniCategory = 4) AND (ISNULL(minAfvoer)) ) ");
                qry.AppendLine(" AND ((maxDood >= '" + strDate + "') OR ISNULL(MaxDood)) ");
                ////////////////////////////////////////////////////////////////////////////////////
            }
            unLogger.WriteInfo(qry.ToString());
            return qry;
        }
        
        public bool rdDierAanwezig(int aniId, int farmId, int pUbnID, DateTime datum, int groupId, int mede_eigenaar, int pProgId)
        {
            if (farmId == 0) { return false; }

            StringBuilder qry = getQuery_Aanwezigheid(farmId, pUbnID, aniId, datum, groupId, mede_eigenaar, pProgId);

            DataTable dtResults = Parent.QueryData(qry.ToString());

            return (dtResults.Rows.Count > 0);
        }

        public int rdCountDierAanwezig(int farmId, int pUbnId, DateTime datum, int groupId, int mede_eigenaar, int pProgramId)
        {
            if (farmId == 0) { return 0; }

            StringBuilder qry = getQuery_Aanwezigheid(farmId, pUbnId, 0, datum, groupId, mede_eigenaar, pProgramId);

            DataTable dtResults = Parent.QueryData(qry.ToString());

            return dtResults.Rows.Count;
        }

        public List<int> rdAanwezigeDieren(int farmId, int pUbnID, DateTime datum, int groupId, int mede_eigenaar, int pProgId)
        {
            if (farmId == 0) { return null; }

            List<int> returnList = new List<int>();

            StringBuilder qry = getQuery_Aanwezigheid(farmId, pUbnID, 0, datum, groupId, mede_eigenaar, pProgId);

            DataTable dtResults = Parent.QueryData(qry.ToString());

            foreach (DataRow drRow in dtResults.Rows)
            {
                returnList.Add(int.Parse(drRow["AniId"].ToString()));
            }

            return returnList;
        }

    }
}
