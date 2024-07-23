using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQFeeds
    {
        private DBSelectQueries Parent;

        public DBSQFeeds(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        public List<FEED_RATIONGROUP_KIND> getFeedRationGroupKinds(int pUbnId, int pProgId)
        {
            List<FEED_RATIONGROUP_KIND> Result = new List<FEED_RATIONGROUP_KIND>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM FEED_RATIONGROUP_KIND ");
            lQuery.AppendFormat(" WHERE UBNid = {0} AND AnimalKind_ID={1} ", pUbnId, pProgId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEED_RATIONGROUP_KIND lrecord = new FEED_RATIONGROUP_KIND();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public List<FEED_RATION_SETTINGS> getFeedRationSettings(int pUbnId, int pProgId, int pFR_GroupID)
        {
            List<FEED_RATION_SETTINGS> Result = new List<FEED_RATION_SETTINGS>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.AppendLine(" SELECT * FROM FEED_RATION_SETTINGS ");
            lQuery.AppendFormat(" WHERE UBNid = {0} AND AnimalKind_ID={1} AND FR_GroupID={2}", pUbnId, pProgId, pFR_GroupID);
            lQuery.AppendLine(" ORDER BY FR_PhaseNr ");
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEED_RATION_SETTINGS lrecord = new FEED_RATION_SETTINGS();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public List<FEEDCURVE_DAYS> getFeedcurveDays(int pUbnId, int pProgId)
        {
            List<FEEDCURVE_DAYS> Result = new List<FEEDCURVE_DAYS>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEEDCURVE_DAYS ");
            lQuery.AppendFormat(" WHERE UBNid = {0} AND AnimalKind_ID={1}", pUbnId, pProgId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEEDCURVE_DAYS lrecord = new FEEDCURVE_DAYS();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public List<FEEDCURVE_DAYS_DETAIL> getFeedcurveDaysDetails(int pFd_ID)
        {
            List<FEEDCURVE_DAYS_DETAIL> Result = new List<FEEDCURVE_DAYS_DETAIL>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEEDCURVE_DAYS_DETAIL ");
            lQuery.AppendFormat(" WHERE Fd_ID= {0}  ", pFd_ID);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEEDCURVE_DAYS_DETAIL lrecord = new FEEDCURVE_DAYS_DETAIL();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public List<FEEDCURVE_MILK> getFeedcurveMelkVoer(int pUbnId, int pProgId)
        {
            List<FEEDCURVE_MILK> Result = new List<FEEDCURVE_MILK>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEEDCURVE_MILK ");
            lQuery.AppendFormat(" WHERE UBNid = {0} AND AnimalKind_ID={1}", pUbnId, pProgId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEEDCURVE_MILK lrecord = new FEEDCURVE_MILK();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public void getFeedcurveMelkVoerInfo(int pFm_ID, out List<FEEDCURVE_MILK_MILKINFO> pMilkInfo, out List<FEEDCURVE_MILK_MILKINFO_DETAIL> pMilkInfoDetail)
        {
            pMilkInfo = new List<FEEDCURVE_MILK_MILKINFO>();
            pMilkInfoDetail = new List<FEEDCURVE_MILK_MILKINFO_DETAIL>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEEDCURVE_MILK_MILKINFO fmm ");
            lQuery.Append(" LEFT JOIN agrofactuur.FEEDCURVE_MILK_MILKINFO_DETAIL fmmd ON fmmd.fmm_ID=fmm.fmm_ID  ");
            lQuery.AppendFormat(" WHERE fmm.Fm_ID= {0}  ", pFm_ID);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEEDCURVE_MILK_MILKINFO lrecord = new FEEDCURVE_MILK_MILKINFO();
                if (Parent.FillObject(lrecord, dr))
                {
                    var check = from n in pMilkInfo where n.Fmm_ID == lrecord.Fmm_ID select n;
                    if (check.Count() == 0)
                    {
                        pMilkInfo.Add(lrecord);
                    }
                }
                FEEDCURVE_MILK_MILKINFO_DETAIL lrecordDetail = new FEEDCURVE_MILK_MILKINFO_DETAIL();
                if (Parent.FillObject(lrecordDetail, dr))
                {

                    pMilkInfoDetail.Add(lrecordDetail);

                }
            }

        }

        public List<FEEDCURVE_UPDOWN> getFeedcurvesUpDown(int pUbnId, int pProgId)
        {
            List<FEEDCURVE_UPDOWN> Result = new List<FEEDCURVE_UPDOWN>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEEDCURVE_UPDOWN ");
            lQuery.AppendFormat(" WHERE UBNid = {0} AND AnimalKind_ID={1}", pUbnId, pProgId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEEDCURVE_UPDOWN lrecord = new FEEDCURVE_UPDOWN();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }
      
        public List<FEEDCURVE_UPDOWN_DETAIL> getFeedcurvesUpDownDetails(int pFud_ID)
        {
            List<FEEDCURVE_UPDOWN_DETAIL> Result = new List<FEEDCURVE_UPDOWN_DETAIL>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEEDCURVE_UPDOWN_DETAIL ");
            lQuery.AppendFormat(" WHERE Fud_ID= {0}  ", pFud_ID);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEEDCURVE_UPDOWN_DETAIL lrecord = new FEEDCURVE_UPDOWN_DETAIL();
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public List<FEED_IN_FEEDCOMPUTER> getFeedInComp(int pFarmId)
        {

            List<FEED_IN_FEEDCOMPUTER> lResultValue = new List<FEED_IN_FEEDCOMPUTER>();

            StringBuilder lQuery = new StringBuilder(" SELECT * FROM agrofactuur.FEED_IN_FEEDCOMPUTER WHERE FarmId=" + pFarmId.ToString() + "  ");
            System.Data.DataTable dtFeeds = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow drwSteps in dtFeeds.Rows)
            {
                FEED_IN_FEEDCOMPUTER step = new FEED_IN_FEEDCOMPUTER();
                if (Parent.FillObject(step, drwSteps))
                { lResultValue.Add(step); }
            }
            return lResultValue;
        }

        public List<FEED_STEP> getFeedrest(int pFarmId)
        {

            List<FEED_STEP> lResultValue = new List<FEED_STEP>();

            StringBuilder lQuery = new StringBuilder(" SELECT * FROM agrofactuur.FEED_STEP WHERE FarmId=" + pFarmId.ToString() + "  ");
            System.Data.DataTable dtFeeds = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow drwSteps in dtFeeds.Rows)
            {
                FEED_STEP step = new FEED_STEP();
                if (Parent.FillObject(step, drwSteps))
                { lResultValue.Add(step); }
            }
            return lResultValue;
        }

        public List<FEED_ADVICE_ROVECOM> getFeedAdviceRovecom(int pUbnId)
        {
            List <FEED_ADVICE_ROVECOM> Result = new List<FEED_ADVICE_ROVECOM>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM FEED_ADVICE_ROVECOM ");
            lQuery.AppendFormat(" WHERE UbnID= {0} AND AniID>0 ", pUbnId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEED_ADVICE_ROVECOM lrecord = new FEED_ADVICE_ROVECOM();  
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }

        public List<FEED_ROVECOM> getFeedRovecom(int pUbnId, int pProgID)
        {
            List<FEED_ROVECOM> Result = new List<FEED_ROVECOM>();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT * FROM agrofactuur.FEED_ROVECOM ");
            lQuery.AppendFormat(" WHERE UbnID= {0} AND AnimalKind_ID={1} ", pUbnId, pProgID);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            foreach (DataRow dr in tbl.Rows)
            {
                FEED_ROVECOM lrecord = new FEED_ROVECOM();  
                if (Parent.FillObject(lrecord, dr))
                {
                    Result.Add(lrecord);
                }
            }
            return Result;
        }



    }
}
