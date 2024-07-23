using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQTempTable
    {

        /// <summary>
        /// DBSQTempTable bevat alle functies die gebruik maken van temptables
        /// </summary>
        /// <remarks>
        /// 	<para>de enige selectqueries unit die insert statements mag maken</para>
        /// </remarks>
        /// 


        private DBSelectQueries Parent;

        public DBSQTempTable(DBSelectQueries parent, DBConnectionToken token)
        {
            this.Parent = parent;
            //parent.GetDataBase().CreateCommand()
            mtoken = token;
        }

        private DbConnection con;
        private DBConnectionToken mtoken;

        public string temptable_ubnIds(int pProgramId, List<BEDRIJF> listBedrijfFromSoap)
        {
            string sql_fill = string.Empty;

            if (Parent.getUbnIdOndergeschiktenType(pProgramId) == 1)
            {
                //admins op basis van programIds;

                List<int> listProgramId = Parent.getListProgramIdOndergeschikten(pProgramId);

                sql_fill =
                    //"INSERT INTO agrofactuur.temptable_ubnIds " + 
                    " (" +
                        "SELECT DISTINCT UbnId" +
                        " FROM agrofactuur.BEDRIJF" +
                        " WHERE (ProgramId IN (" + Parent.EnumerableToCommaSeperatedString(listProgramId) + "))" +
                        " AND (UbnId > 0)" +
                    ") ";
            }
            else
            {
               
                List<int> listUbnIds = new List<int>();

                foreach (BEDRIJF b in listBedrijfFromSoap)
                {
                    if (!listUbnIds.Contains(b.UBNid))
                    {
                        listUbnIds.Add(b.UBNid);
                    }
                }

                sql_fill =
                    // "INSERT INTO agrofactuur.temptable_ubnIds" + 
                    " (" +
                        "SELECT DISTINCT UbnId" +
                        " FROM agrofactuur.BEDRIJF" +
                        " WHERE (UbnId IN (" + Parent.EnumerableToCommaSeperatedString(listUbnIds) + "))" +
                        " AND (UbnId > 0)" +
                    ") ";
            }

            string sql_create = "CREATE TEMPORARY TABLE  IF NOT EXISTS  agrofactuur.temptable_ubnIds (" +
                   "ubnId INT (11) NOT NULL," +
                   "PRIMARY KEY(ubnId)" +
                   ") ENGINE=MEMORY  " +
                   " AS " + sql_fill + ";";
           
         
            return sql_create;
        }

        [Obsolete("gebruik temptable_ubnIds hierboven en plak deze string voor de Query met de temptable_ubnIds")]
        public int init_temptable_ubnIds(int pProgramId, List<BEDRIJF> listBedrijfFromSoap)
        {
            if (con == null)
            {
                unLogger.WriteError("GEEN SESSIE GEOPEND VOOR GEBRUIK TEMPTABLES!!");
                StartSession();
            }
            //Zelfde functie als getListUbnIdOndergeschikten, maar deze zet de resultaten in een MySQL temptable.

            bool table_created = false;
            bool table_filled = false;


            string sql_create = "CREATE TEMPORARY TABLE  IF NOT EXISTS  agrofactuur.temptable_ubnIds (" +
                   "ubnId INT (11) NOT NULL," +
                   "PRIMARY KEY(ubnId)" +
                   ") ENGINE=MEMORY; " +
                   " SELECT COUNT(ubnId) FROM agrofactuur.temptable_ubnIds; ";

            try
            {
                /*
                 Make sure everywhere use same token FetchRightsToken().getLastChildConnection()
                 */
                unLogger.WriteDebug("init_temptable_ubnIds START");
                DataTable dt = Parent.QueryData(sql_create, MissingSchemaAction.Add);
                if (dt.Rows.Count > 0)
                {
                    table_created = true;
                    if (dt.Rows[0][0] != DBNull.Value && dt.Rows[0][0].ToString() != "")
                    {

                        if (int.Parse(dt.Rows[0][0].ToString()) > 0)
                        {
                            table_filled = true;
                        }
                    }
                }
                
                unLogger.WriteDebug("init_temptable_ubnIds table_created:" + table_created.ToString() + " table_filled:" + table_filled.ToString());
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.ToString(), ex);//We krijgen anders erg veel mail
            }

            //EXECUTE QUERY
            if (table_created)
            {
                if (!table_filled)
                {
                    string sql_fill = string.Empty;

                    if (Parent.getUbnIdOndergeschiktenType(pProgramId) == 1)
                    {
                        //admins op basis van programIds;

                        List<int> listProgramId = Parent.getListProgramIdOndergeschikten(pProgramId);

                        sql_fill =
                            "INSERT INTO agrofactuur.temptable_ubnIds (" +
                                "SELECT DISTINCT UbnId" +
                                " FROM agrofactuur.BEDRIJF" +
                                " WHERE (ProgramId IN (" + Parent.EnumerableToCommaSeperatedString(listProgramId) + "))" +
                                " AND (UbnId > 0)" +
                            ");";
                    }
                    else
                    {
                        //admins zonder programIds (bijvoorbeeld Navobi en Vilatca;
                        //haal de ubns voor het overzicht uit de agrobase soap

                        List<int> listUbnIds = new List<int>();

                        foreach (BEDRIJF b in listBedrijfFromSoap)
                        {
                            if (!listUbnIds.Contains(b.UBNid))
                            {
                                listUbnIds.Add(b.UBNid);
                            }
                        }

                        sql_fill =
                            "INSERT INTO agrofactuur.temptable_ubnIds (" +
                                "SELECT DISTINCT UbnId" +
                                " FROM agrofactuur.BEDRIJF" +
                                " WHERE (UbnId IN (" + Parent.EnumerableToCommaSeperatedString(listUbnIds) + "))" +
                                " AND (UbnId > 0)" +
                            ");";
                    }

                    try
                    {
                        //TODO FIXEN TEMPTABLES
                        DataTable dt = Parent.QueryData(sql_fill, MissingSchemaAction.Add);
                        //Parent.GetDataBase().ExecuteNonQuery(mtoken,sql_fill);
                        dt = Parent.QueryData(" SELECT COUNT(ubnId) FROM agrofactuur.temptable_ubnIds;  ", MissingSchemaAction.Add);
                        if (dt.Rows.Count > 0)
                        {
                            table_created = true;
                            if (dt.Rows[0][0] != DBNull.Value && dt.Rows[0][0].ToString() != "")
                            {
                                unLogger.WriteDebug("init_temptable_ubnIds dt.Rows.Count:" + dt.Rows.Count.ToString());

                                if (int.Parse(dt.Rows[0][0].ToString()) > 0)
                                {
                                    table_filled = true;
                                }
                            }
                        }
                      
                        //using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                        //{
                        //    cmd.CommandText = sql_fill;
                        //    int res = Parent.GetDataBase().ExecuteNonQueryCommand(cmd);
                        //    table_filled = true;
                        //}
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError(ex.ToString(), ex);
                    }
                }
            }
            unLogger.WriteDebug("init_temptable_ubnIds END table_created:" + table_created.ToString() + " table_filled:" + table_filled.ToString());
            return (((table_created == true) && (table_filled == true)) == true) ? 1 : 0;
        }

        public int init_temptable_rdAanwezigeDieren(int ubnId, int aniId, DateTime datum, int groupId, int mede_eigenaar, int pProgId)
        {
            if (con == null)
            {
                unLogger.WriteError("GEEN SESSIE GEOPEND VOOR GEBRUIK TEMPTABLES!!");
                StartSession();
            }


            bool table_exists = false;
            bool table_filled = true;
            if (table_exists == false)
            {
                //--------------------------
                // Create temptable
                //--------------------------

                try
                {
                    //unLogger.WriteDebug("before CREATE TEMPORARY TABLE agrofactuur.temptable_rdAanwezigeDieren");

                    string sql_create =
                        "CREATE TEMPORARY TABLE  IF NOT EXISTS  agrofactuur.temptable_rdAanwezigeDieren (" +
                        "aniId INT (11) NOT NULL," +
                        "PRIMARY KEY(aniId)" +
                        ") ENGINE=MEMORY;";

                    using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sql_create;
                        //TODO FIXEN TEMPTABLES
                        int res = Parent.GetDataBase().ExecuteNonQueryCommand(cmd);
                        table_exists = true;

                        try
                        {
                            //---------------------------------------------------
                            // Clear old data (and check if the temptable exists)
                            //---------------------------------------------------

                            string sql_clear =
                                "DELETE FROM agrofactuur.temptable_rdAanwezigeDieren";


                            cmd.CommandText = sql_clear;
                            //TODO FIXEN TEMPTABLES
                            res = Parent.GetDataBase().ExecuteNonQueryCommand(cmd);
                            table_exists = true;
                        }
                        catch (Exception ex)
                        {
                            //Exception means that the table does not exists and needs to be created
                            //Console.WriteLine(ex.Message);
                            unLogger.WriteError(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    unLogger.WriteError(ex.Message, ex);
                }
            }

            if (table_exists == true)
            {
                //--------------------------
                // Load temptable content
                //--------------------------

                try
                {
                    List<BEDRIJF> farms = Parent.Bedrijf.getBedrijvenByUBNId(ubnId);
                    List<int> aanw = new List<int>();
                    foreach (BEDRIJF b in farms)
                    {

                        if (b.ProgId == pProgId)
                        {

                            List<int> aanw2 = Parent.RDBTools.rdAanwezigeDieren(b.FarmId, b.UBNid, datum, 0, mede_eigenaar, pProgId);
                            aanw.AddRange(aanw2);
                            aanw = aanw.Distinct().ToList();


                        }
                    }
                    StringBuilder bld = new StringBuilder(" INSERT INTO agrofactuur.temptable_rdAanwezigeDieren (aniId) VALUES ");
                    if (aanw.Count() > 0)
                    {
                        for (int i = 0; i < aanw.Count(); i++)
                        {
                            if (i == aanw.Count() - 1)
                            {
                                bld.AppendLine("(" + aanw[i].ToString() + ")");
                            }
                            else
                            {
                                bld.AppendLine("(" + aanw[i].ToString() + "),");
                            }
                        }
                        using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandText = bld.ToString();
                            //TODO FIXEN TEMPTABLES
                            int res = Parent.GetDataBase().ExecuteNonQueryCommand(cmd);
                            table_filled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    unLogger.WriteError(ex.Message, ex);
                    unLogger.WriteDebug("exception INSERT INTO agrofactuur.temptable_rdAanwezigeDieren");
                }
            }

            return (table_filled == true) ? 1 : 0;
        }


        #region overzichtaantallen

        public int getAantalAanwezigeDierenrdAanwezigeDieren()
        {
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*)" +
                                  " FROM agrofactuur.temptable_rdAanwezigeDieren";
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    int result = 0;
                    if (reader.GetValue(0) != DBNull.Value)
                        result = Convert.ToInt32(reader.GetValue(0));
                    return 0;

                }
            }
        }



        public int getAantalLammerenrdAanwezigeDieren(DateTime pPeilDatum)
        {
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText =  "SELECT COUNT(a.AniId)" +
                       " FROM agrofactuur.temptable_rdAanwezigeDieren tmpt" +
                       " LEFT JOIN ANIMAL a ON a.AniId=tmpt.AniId" +
                       " WHERE (TIMESTAMPDIFF(YEAR, a.AniBirthDate, '" + pPeilDatum.ToString("yyyy-MM-dd") + "') < 1)";
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    int result = 0;
                    if (reader.GetValue(0) != DBNull.Value)
                        result = Convert.ToInt32(reader.GetValue(0));
                    return 0;

                }
            }
        }

        public DataTable getAantalFokdierenrdAanwezigeDieren(DateTime pPeilDatum)
        {
            using (DbCommand cmd = con.CreateCommand())
            {
                DataTable dt = null;
                try
                {
                    cmd.CommandText =   "SELECT a.AniSex, COUNT(a.AniId) as c" +
                                        " FROM agrofactuur.temptable_rdAanwezigeDieren tmpt" +
                                        " LEFT JOIN ANIMAL a ON a.AniId=tmpt.AniId" +
                                        " WHERE (TIMESTAMPDIFF(YEAR, a.AniBirthDate, '" + pPeilDatum.ToString("yyyy-MM-dd") + "') >= 1)" +
                                        " GROUP BY a.AniSex";
                    dt = Parent.GetDataBase().QueryData(mtoken, cmd);
                }
                catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
                return dt;
            }
        }



        public DataTable getAantalScrapieDierenrdAanwezigeDieren()
        {
            using (DbCommand cmd = con.CreateCommand())
            {
                DataTable dt = null;
                try
                {
                    cmd.CommandText =   "SELECT a.AniScrapie, COUNT(*) as c" +
                                        " FROM agrofactuur.temptable_rdAanwezigeDieren tmpt" +
                                        " LEFT JOIN ANIMAL a ON a.AniId=tmpt.AniId" +
                                        " GROUP BY a.AniScrapie";
                    dt = Parent.GetDataBase().QueryData(mtoken, cmd);
                }
                catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
                return dt;
            }
        }
        #endregion



        public DataTable overzichtaantallenQuery(String Query)
        {
            using (DbCommand cmd = con.CreateCommand())
            {
                DataTable dt = null;
                try
                {
                    cmd.CommandText = Query;
                    dt = Parent.GetDataBase().QueryData(mtoken, null, cmd, string.Empty, MissingSchemaAction.Add);
                }
                catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
                return dt;
            }
        }

        public DataTable BloedonderzoekAuthorisatie_GetBedrijvenMetData(int show, int pProgramId)
        {
            //verkrijgt bedrijven met bloedauthorisatie-data
            //(zodat niet alle bedrijven ingeladen hoeven te worden)

            //LET OP: DEZE FUNCTIE MAAKT GEBRUIK VAN temptable_ubnIds
            //Zorg dat deze aangemaakt is met functie init_temptable_ubnIds

            string database = "agrobase_sheep.";//LET OP DE PUNT
            if (pProgramId == 5)
            {
                database = "agrobase_goat.";
            }

            string sHaving = "";

            switch (show)
            {
                case 0: //Alle bedrijven met openstaande (nog niet-geauthoriseerde) bloedonderzoeken
                    sHaving = " HAVING cntNonAuthorized>0";
                    break;
                case 1: //Alle bedrijven met verwerkte (geauthoriseerde) bloedonderzoeken
                    sHaving = " HAVING cntAuthorized>0";
                    break;
                case 2: //Alle bedrijven met bloedonderzoeken (zowel geauthoriseerd als niet-geauthoriseerd)
                    sHaving = " HAVING cntEvents>0";
                    break;
            }
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText =
                    @"SELECT DISTINCT 
                     u.UBNid, u.Bedrijfsnummer 
                    , b.FarmId, b.ProgramId
                    , t.ThrId, t.ThrCompanyName, t.ThrStreet1, t.ThrExt, t.ThrZipCode, t.ThrCity, t.ThrEmail
                    , l.LabLabel as prognaam

                    , COUNT(e.EventId) as cntEvents
                    , COUNT((SELECT EventId FROM " + database + @"BLOOD WHERE EventId=bl.EventId AND (BloAuthorized=0 OR ISNULL(BloAuthorized)) )) as cntNonAuthorized
                    , COUNT((SELECT EventId FROM " + database + @"BLOOD WHERE EventId=bl.EventId AND BloAuthorized=1)) as cntAuthorized

                    , DATE(MAX(IF(brd.BRD_Date_Sampled IS NULL, e.EveDate, brd.BRD_Date_Sampled))) AS maxDtMonster

                    , bz1.bz_StatusID as 'status1', lbz1.LabLabel as 'lab_status1'
                    , bz2.bz_StatusID as 'status2', lbz2.LabLabel as 'lab_status2'
                    , bz3.bz_StatusID AS 'status3', lbz3.LabLabel AS 'lab_status3' 
                    , bz4.bz_StatusID as 'status4', lbz2.LabLabel as 'lab_status4'
                    , bz5.bz_StatusID AS 'status5', lbz3.LabLabel AS 'lab_status5' 

                 FROM " + database + @"ANIMAL a
                 LEFT JOIN " + database + @"ANIMALCATEGORY ac ON (ac.AniId=a.AniId)

                 RIGHT JOIN agrofactuur.BEDRIJF b ON (b.FarmId=ac.FarmId )  

                 RIGHT JOIN agrofactuur.UBN u ON (u.UbnId=b.UbnId)
                 LEFT JOIN agrofactuur.THIRD t ON (t.ThrId=u.ThrID)

                 RIGHT JOIN agrofactuur.temptable_ubnIds tmpt ON (tmpt.UbnId=u.UbnId)

                 LEFT JOIN agrofactuur.AGRO_LABELS l ON (l.LabKind=9100 AND l.LabProgID=0 AND l.LabProgramID=0 AND l.LabCountry=0 AND l.LabID=b.Programid AND b.Programid>0)

                 LEFT JOIN agrofactuur.BEDRIJF_ZIEKTE bz1 ON (bz1.bz_FarmID=b.FarmId AND bz1.bz_ZiekteID=1
                     AND bz1.bz_Datum = (SELECT MAX(bz_Datum) FROM agrofactuur.BEDRIJF_ZIEKTE WHERE bz_FarmID=b.FarmId AND bz_ZiekteID=bz1.bz_ZiekteID))

                 LEFT JOIN agrofactuur.BEDRIJF_ZIEKTE bz2 ON (bz2.bz_FarmID=b.FarmId AND bz2.bz_ZiekteID=2
                     AND bz2.bz_Datum = (SELECT MAX(bz_Datum) FROM agrofactuur.BEDRIJF_ZIEKTE WHERE bz_FarmID=b.FarmId AND bz_ZiekteID=bz2.bz_ZiekteID))

                  LEFT JOIN agrofactuur.BEDRIJF_ZIEKTE bz3 ON (bz3.bz_FarmID=b.FarmId AND bz3.bz_ZiekteID=3 
                     AND bz3.bz_Datum = (SELECT MAX(bz_Datum) FROM agrofactuur.BEDRIJF_ZIEKTE WHERE bz_FarmID=b.FarmId AND bz_ZiekteID=bz3.bz_ZiekteID)) 

                  LEFT JOIN agrofactuur.BEDRIJF_ZIEKTE bz4 ON (bz4.bz_FarmID=b.FarmId AND bz4.bz_ZiekteID=4 
                     AND bz4.bz_Datum = (SELECT MAX(bz_Datum) FROM agrofactuur.BEDRIJF_ZIEKTE WHERE bz_FarmID=b.FarmId AND bz_ZiekteID=bz4.bz_ZiekteID)) 

                 LEFT JOIN agrofactuur.BEDRIJF_ZIEKTE bz5 ON (bz5.bz_FarmID=b.FarmId AND bz5.bz_ZiekteID=5 
                     AND bz5.bz_Datum = (SELECT MAX(bz_Datum) FROM agrofactuur.BEDRIJF_ZIEKTE WHERE bz_FarmID=b.FarmId AND bz_ZiekteID=bz5.bz_ZiekteID)) 


                 LEFT JOIN agrofactuur.AGRO_LABELS lbz1 ON (lbz1.LabKind=251 AND lbz1.LabProgID=0 AND lbz1.LabProgramID=0 AND lbz1.LabCountry=0 AND lbz1.LabID=bz1.bz_StatusID)
                 LEFT JOIN agrofactuur.AGRO_LABELS lbz2 ON (lbz2.LabKind=251 AND lbz2.LabProgID=0 AND lbz2.LabProgramID=0 AND lbz2.LabCountry=0 AND lbz2.LabID=bz2.bz_StatusID)
                 LEFT JOIN agrofactuur.AGRO_LABELS lbz3 ON (lbz3.LabKind=251 AND lbz3.LabProgID=0 AND lbz3.LabProgramID=0 AND lbz3.LabCountry=0 AND lbz3.LabID=bz3.bz_StatusID)
                 LEFT JOIN agrofactuur.AGRO_LABELS lbz4 ON (lbz4.LabKind=251 AND lbz4.LabProgID=0 AND lbz4.LabProgramID=0 AND lbz4.LabCountry=0 AND lbz4.LabID=bz4.bz_StatusID)
                 LEFT JOIN agrofactuur.AGRO_LABELS lbz5 ON (lbz5.LabKind=251 AND lbz5.LabProgID=0 AND lbz5.LabProgramID=0 AND lbz5.LabCountry=0 AND lbz5.LabID=bz5.bz_StatusID)


                 RIGHT JOIN " + database + @"EVENT e ON (e.AniId=a.AniId )
                 LEFT JOIN " + database + @"BLOOD bl ON (bl.EventId=e.EventId)
                 LEFT JOIN agrofactuur.BLOOD_RESEARCH_DETAIL brd ON (brd.BRD_BloID=bl.BRD_BloID)

                 WHERE (e.EveKind=11)
                 AND (bl.BloKind < 0)
                 AND (e.AniId > 0) AND (e.EventId > 0) AND (u.UBNid > 0) 
                 AND (NOT b.ProgramId = 100)
                 GROUP BY
                     u.UBNid, u.Bedrijfsnummer
                    , b.FarmId, b.ProgramId
                    , t.ThrCompanyName, t.ThrStreet1, t.ThrExt, t.ThrZipCode, t.ThrCity, t.ThrEmail
                    , l.LabLabel
                    , status1, lab_status1, status2, lab_status2 " +

                    sHaving +

                    " ORDER BY u.Bedrijfsnummer";

                DataTable dt = null;
                try
                {
                    dt = Parent.GetDataBase().QueryData(mtoken, null, cmd, string.Empty, MissingSchemaAction.Add);
                }
                catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }

                return dt;
            }
        }





        // Moet altijd aangeroepen worden voordat 
        public void StartSession()
        {
            using (DbCommand cmdstart = Parent.GetDataBase().CreateCommand(mtoken))
            {
                con = cmdstart.Connection;
            }
        }

        public void EndSession()
        {
            this.con.Dispose();
        }
    }
}
