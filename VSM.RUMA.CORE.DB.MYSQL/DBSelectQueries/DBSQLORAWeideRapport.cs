using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using MySql.Data.MySqlClient;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQLORAWeideRapport
    {
        private DBSelectQueries Parent;

        public DBSQLORAWeideRapport(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        protected DataTable GetDataTableFromStats(int ubnId, string query)
        {
            var dt = Parent.QueryData($@"

SELECT
	u.Bedrijfsnummer,
	d.ds_Host,
	i.il_Id,
	i.ProgramId

FROM agrofactuur.UBN u
JOIN flexdata.IMPORTLIST i ON i.il_UBN = u.Bedrijfsnummer AND i.il_Id > 0 AND i.il_ImportState = 4
JOIN flexdata.DATABASESERVERS d ON d.ds_Id = i.ds_Id

WHERE u.UBNid = {ubnId}

ORDER BY i.il_ImportDate DESC LIMIT 1

");
            if (dt.Rows.Count == 0) return null;
            var database = $"dbstats_{dt.Rows[0]["Bedrijfsnummer"].ToString()}_{dt.Rows[0]["ProgramId"].ToString()}_{dt.Rows[0]["il_Id"].ToString()}";
            var conString = $" server={dt.Rows[0]["ds_Host"].ToString()}; user id=agroread; Pwd=ag5054ad; database={database}; SslMode=none; Allow User Variables=True ";

            using (MySqlCommand cmd = new MySqlCommand(query, new MySqlConnection(conString)))
            {
                cmd.Connection.Open();

                dt = new DataTable();

                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;

                adapter.Fill(dt);

                cmd.Connection.Close();

                return dt;
            }
        }

        protected int GetLORADairyCows(int ubnId, string date)
        {
            var query = Parent.QueryData(" SELECT s.sq_Query_String FROM agrosettings.SELECTION_QUERY s WHERE s.sq_Function_Name = 'GetWeidegang' ").Rows[0][0].ToString();
            var dt = GetDataTableFromStats(ubnId, $" SET @ubnId := {ubnId}; {query} ");

            var rows = dt.Select($" Day = '{date}' ");

            if (rows.Count() == 0) return 0;

            return Convert.ToInt32(rows.First()["Milking"].ToString());

            //string query = $@"
            //    SELECT COUNT(*) AS aantal FROM (
            //        SELECT aniid, (
            //            SELECT COUNT(*) FROM agrobase.EVENT
            //            WHERE EVENT.aniid = MCT.AniID AND EVENT.evekind = 5 and EVENT.eventid > 0
            //        ) As Aantal_afkalvingen FROM agrodata.MEADOWCOWTIME MCT
            //        WHERE ubnid = {ubnId} AND date(MCT_StartgrazingDateTime) = '{date}'
            //        GROUP BY aniid
            //        HAVING Aantal_afkalvingen > 0
            //    ) blabla
            //";
            //DataTable dt = Parent.QueryData(query);
            //if (dt.Rows.Count <= 0)
            //    return 0;
            //DataRow dr = dt.Rows[0];
            //return (int?)dr.Field<long?>("aantal") ?? 0;
        }

        protected int GetLORAQualifiedCows(int ubnId, string date)
        {
            var query = Parent.QueryData(" SELECT s.sq_Query_String FROM agrosettings.SELECTION_QUERY s WHERE s.sq_Function_Name = 'GetWeidegang' ").Rows[0][0].ToString();
            var dt = GetDataTableFromStats(ubnId, $" SET @ubnId := {ubnId}; {query} ");

            var rows = dt.Select($" Day = '{date}' ");

            if (rows.Count() == 0) return 0;

            return Convert.ToInt32(rows.First()["Qualified"].ToString());

            //string query = $@"
            //    SELECT COUNT(*) AS aantal FROM (
            //        SELECT aniid, max(MCT_QualifiedForGrazing) AS qualified FROM agrodata.MEADOWCOWTIME
            //        WHERE ubnid = {ubnId} AND date(MCT_StartgrazingDateTime) = '{date}'
            //        GROUP BY aniid
            //        HAVING qualified = 1
            //    ) blabla
            //";
            //DataTable dt = Parent.QueryData(query);
            //if (dt.Rows.Count <= 0)
            //    return 0;
            //DataRow dr = dt.Rows[0];
            //return (int?)dr.Field<long?>("aantal") ?? 0;
        }

        protected int GetLORACowsLessThan60(int ubnId, string date)
        {
            var query = Parent.QueryData(" SELECT s.sq_Query_String FROM agrosettings.SELECTION_QUERY s WHERE s.sq_Function_Name = 'GetWeidegang' ").Rows[0][0].ToString();
            var dt = GetDataTableFromStats(ubnId, $" SET @ubnId := {ubnId}; {query} ");

            var rows = dt.Select($" Day = '{date}' ");

            if (rows.Count() == 0) return 0;

            return Convert.ToInt32(rows.First()["Inadequate"].ToString()) + Convert.ToInt32(rows.First()["Not"].ToString());

            //string query = $@"
            //    SELECT COUNT(*) AS aantal FROM (SELECT aniid, sum(Mct_grazingMinutes) AS grazingminutes , MCT_StartGrazingDateTime FROM agrodata.MEADOWCOWTIME 
            //    WHERE ubnid={ubnId}
            //    AND date(MCT_StartGrazingDateTime)='{date}'
            //    AND MCT_QualifiedForGrazing = 1
            //    GROUP BY date(MCT_StartGrazingDateTime), aniid 
            //    HAVING grazingminutes < 60) AS cows_60
            //";
            //DataTable dt = Parent.QueryData(query);
            //if (dt.Rows.Count <= 0)
            //    return 0;
            //DataRow dr = dt.Rows[0];
            //return (int?)dr.Field<long?>("aantal") ?? 0;
        }

        protected int GetLORAGrazingMinutes(int ubnId, string date)
        {
            var query = Parent.QueryData(" SELECT s.sq_Query_String FROM agrosettings.SELECTION_QUERY s WHERE s.sq_Function_Name = 'GetWeidegang' ").Rows[0][0].ToString();
            var dt = GetDataTableFromStats(ubnId, $" SET @ubnId := {ubnId}; {query} ");

            var rows = dt.Select($" Day = '{date}' ");

            if (rows.Count() == 0) return 0;

            return Convert.ToInt32(rows.First()["TotalMinutues"].ToString());

            //string query = $@"
            //    SELECT sum(Mct_grazingMinutes) as aantal from agrodata.MEADOWCOWTIME 
            //    WHERE ubnid={ubnId}
            //    AND date(MCT_StartGrazingDateTime)='{date}'
            //    AND MCT_QualifiedForGrazing = 1
            //";
            //DataTable dt = Parent.QueryData(query);
            //if (dt.Rows.Count <= 0)
            //    return 0;
            //DataRow dr = dt.Rows[0];
            //return (int?)dr.Field<decimal?>("aantal") ?? 0;
        }

        protected string GetLORATankNumber(int ubnId, string date)
        {
            string query = $@"
                SELECT CooperationNumber, SupplierNumber FROM agrofactuur.MILKSUPL 
                WHERE ubnid={ubnId}
                AND DeliveryDate <= '{date}'
                ORDER BY Deliverydate DESC LIMIT 1
            ";
            DataTable dt = Parent.QueryData(query);
            if (dt.Rows.Count <= 0)
                return null;

            int coop = dt.Rows[0].Field<int>("CooperationNumber");
            int tank = dt.Rows[0].Field<int>("SupplierNumber");
            return coop.ToString("D3") + tank.ToString("D6");
        }

        public DateTime? GetEquipmentPurchaseDate(string ubn)
        {
            string query = $@"
                SELECT UP_Value FROM agrofactuur.UBN_PROPERTY
                WHERE ubnid IN (SELECT UbnId FROM agrofactuur.UBN WHERE Bedrijfsnummer='{ubn}') AND UP_Key='AANSCHAF_WEIDESENSOR'
            ";
            DataTable dt = Parent.QueryData(query);
            if (dt.Rows.Count <= 0)
                return null;
            return DateTime.ParseExact(dt.Rows[0].Field<string>("UP_Value"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        protected double GetLORAGrazingDeviation(int ubnId, string date)
        {
            string query = $@"
                SELECT STDDEV(Mct_grazingMinutes) AS deviation FROM agrodata.MEADOWCOWTIME 
                WHERE ubnid={ubnId}
                AND date(MCT_StartGrazingDateTime)='{date}'
                AND MCT_QualifiedForGrazing = 1
            ";
            DataTable dt = Parent.QueryData(query);
            if (dt.Rows.Count <= 0)
                return 0.0;
            DataRow dr = dt.Rows[0];
            return dr.Field<double?>("deviation") ?? 0;
        }

        protected int GetLORACumGrazingDays(int ubnId, string date)
        {
            var query = Parent.QueryData(" SELECT s.sq_Query_String FROM agrosettings.SELECTION_QUERY s WHERE s.sq_Function_Name = 'GetWeidegang' ").Rows[0][0].ToString();
            var dt = GetDataTableFromStats(ubnId, $" SET @ubnId := {ubnId}; {query} ");

            var rows = dt.Select($" Day <= '{date}' AND PercentHour >= 90.0 ");

            return rows.Count();

            //string query = $@"
            //    SELECT COUNT(distinct date(MCT_StartgrazingDateTime)) AS aantal FROM agrodata.MEADOWCOWTIME
            //    WHERE ubnid={ubnId}
            //    AND MCT_StartgrazingDateTime >= MAKEDATE(YEAR('{date}'),1)
            //    AND MCT_EndgrazingDateTime <= LAST_DAY(DATE_ADD('{date}', INTERVAL 12-MONTH('{date}') MONTH))
            //    AND MCT_EndgrazingDateTime <= '{date}'
            //    AND MCT_QualifiedForGrazing = 1
            //";
            //DataTable dt = Parent.QueryData(query);
            //if (dt.Rows.Count <= 0)
            //    return 0;
            //DataRow dr = dt.Rows[0];
            //return (int?)dr.Field<long?>("aantal") + 1 ?? 1;
        }

        protected long GetLORACumGrazingTimeYear(UBN ubn, string date, int GrazingTime)
        {
            string query = $@"
                SELECT SUM(MR_FarmGrazingTime) AS aantal FROM agrodata.MEADOWREPORT
                WHERE UBNnummer='{ubn.Bedrijfsnummer}'
                AND MR_GrazingDate >= MAKEDATE(YEAR('{date}'),1)
                AND MR_GrazingDate < '{date}'
                AND MR_GrazingDay = 1
            ";
            DataTable dt = Parent.QueryData(query);
            if (dt.Rows.Count <= 0)
                return 0;
            DataRow dr = dt.Rows[0];
            return (long?)dr.Field<decimal?>("aantal") + GrazingTime ?? GrazingTime;
        }

        private LORA_GRAZINGREPORT CreateFromRow(DataRow dr)
        {
            var reportDateTime = dr["MR_ReportdateTime"] == DBNull.Value ? null : (DateTime?)((MySqlDateTime)dr["MR_ReportdateTime"]).Value;
            LORA_GRAZINGREPORT report = new LORA_GRAZINGREPORT()
            {
                id = dr.Field<int>("MR_ID"),
                UBNNumber = dr.Field<string>("UBNnummer"),
                TankNumber = dr.Field<string>("Tanknummer"),
                GrazingDate = dr.Field<MySqlDateTime>("MR_GrazingDate").Value,
                TotalNumberDairyCows = dr.Field<int>("MR_TotNumberDairyCows"),
                TotalNumberQualifiedCows = dr.Field<int>("MR_TotNumberQualifiedCows"),
                PercentageQualifiedForGrazing = dr.Field<double>("MR_PercentageQualifiedForGrazing"),
                GrazingDay = dr.Field<sbyte>("MR_GrazingDay") > 0,
                FarmGrazingTime = dr.Field<short>("MR_FarmGrazingTime"),
                GrazingDeviation = dr.Field<double>("MR_Grazing_Deviation"),
                CumulatedGrazingTimeYear = dr.Field<long>("MR_CumulatedGrazingTimeYear"),
                CumulatedGrazingDays = dr.Field<int>("MR_CumulatedGrazingDays"),
                ReportDateTime = reportDateTime,
            };
            return report;
        }

        public LORA_GRAZINGREPORT CreateLORAReport(UBN ubn, DateTime date)
        {
            string dbDate = date.ToString("yyyy-MM-dd");
            int dairyCows = GetLORADairyCows(ubn.UBNid, dbDate);
            int qualCows = GetLORAQualifiedCows(ubn.UBNid, dbDate);
            int less60 = GetLORACowsLessThan60(ubn.UBNid, dbDate);
            int grazMins = GetLORAGrazingMinutes(ubn.UBNid, dbDate);
            string tankNumber = GetLORATankNumber(ubn.UBNid, dbDate);

            double percentageGrazing = 0;
            bool grazingDay = false;
            int farmGrazingTime = 0;

            if (qualCows > 0)
            {
                percentageGrazing = (double)(qualCows - less60) / qualCows * 100.0;
                if (percentageGrazing >= 89.5)
                {
                    grazingDay = true;
                    farmGrazingTime = (int)Math.Round((double)grazMins / qualCows);
                }
            }

            double deviation = GetLORAGrazingDeviation(ubn.UBNid, dbDate);
            long cumGrazingTimeYear = GetLORACumGrazingTimeYear(ubn, dbDate, farmGrazingTime);
            int cumGrazingDays = GetLORACumGrazingDays(ubn.UBNid, dbDate);

            LORA_GRAZINGREPORT report = new LORA_GRAZINGREPORT
            {
                UBNNumber = ubn.Bedrijfsnummer,
                TankNumber = tankNumber,
                GrazingDate = date.Date,
                TotalNumberDairyCows = dairyCows,
                TotalNumberQualifiedCows = qualCows,
                PercentageQualifiedForGrazing = percentageGrazing,
                GrazingDay = grazingDay,
                FarmGrazingTime = farmGrazingTime,
                GrazingDeviation = deviation,
                CumulatedGrazingTimeYear = cumGrazingTimeYear,
                CumulatedGrazingDays = cumGrazingDays,
                ReportDateTime = null,
            };

            return report;
        }

        public LORA_GRAZINGREPORT GetLORAReport(UBN ubn, DateTime date)
        {
            string query = $@"
                SELECT MR_ID, UBNnummer, Tanknummer, MR_GrazingDate, MR_TotNumberDairyCows, MR_TotNumberQualifiedCows, MR_PercentageQualifiedForGrazing, MR_GrazingDay, MR_FarmGrazingTime, MR_Grazing_Deviation, MR_CumulatedGrazingTimeYear, MR_CumulatedGrazingDays, MR_ReportdateTime
                FROM agrodata.MEADOWREPORT
                WHERE UBNnummer='{ubn.Bedrijfsnummer}' AND MR_GrazingDate='{date.ToString("yyyy-MM-dd")}'
            ";
            DataTable dt = Parent.QueryData(query);
            if (dt.Rows.Count <= 0)
                return null;
            DataRow dr = dt.Rows[0];
            return CreateFromRow(dr);
        }

        public IList<LORA_GRAZINGREPORT> GetLORAReports(DateTime date)
        {
            string query = $@"
                SELECT MR_ID, UBNnummer, Tanknummer, MR_GrazingDate, MR_TotNumberDairyCows, MR_TotNumberQualifiedCows, MR_PercentageQualifiedForGrazing, MR_GrazingDay, MR_FarmGrazingTime, MR_Grazing_Deviation, MR_CumulatedGrazingTimeYear, MR_CumulatedGrazingDays, MR_ReportdateTime
                FROM agrodata.MEADOWREPORT
                WHERE MR_GrazingDate='{date.ToString("yyyy-MM-dd")}'
            ";
            DataTable dt = Parent.QueryData(query);

            List<LORA_GRAZINGREPORT> reports = new List<LORA_GRAZINGREPORT>();
            foreach (DataRow dr in dt.Rows)
            {
                reports.Add(CreateFromRow(dr));
            }
            return reports;
        }
    }
}