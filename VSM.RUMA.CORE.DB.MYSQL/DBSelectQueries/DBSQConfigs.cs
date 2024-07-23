using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ 
{
    public class DBSQConfigs
    {
        private DBSelectQueries Parent;

        public DBSQConfigs(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        public bool getAutoresponders(int pFarmId, int pProgramId)
        {
            bool Result = false;
            FARMCONFIG lFarmConfig = new FARMCONFIG();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT FarmId AS ID, fc.FValue FROM agrofactuur.FARMCONFIG fc ");
            lQuery.AppendFormat(" WHERE FarmId = {0} AND FKey = 'autoresponders' ", pFarmId);
            lQuery.Append("  UNION ");
            lQuery.Append(" SELECT ProgramID AS ID, pc.FValue FROM agrofactuur.PROGRAMCONFIG pc  ");
            lQuery.AppendFormat(" WHERE pc.ProgramId = {0} AND FKey = 'autoresponders'  ", pProgramId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            if (tbl.Rows.Count > 0)
            {
                if (tbl.Rows[0][1] != DBNull.Value)
                {
                    Result = Convert.ToBoolean(tbl.Rows[0][1]);
                }
            }

            return Result;
        }

        public bool getBmsresponders(int pFarmId, int pProgramId)
        {
            bool Result = false;
            FARMCONFIG lFarmConfig = new FARMCONFIG();
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(" SELECT FarmId AS ID, fc.FValue FROM agrofactuur.FARMCONFIG fc ");
            lQuery.AppendFormat(" WHERE FarmId = {0} AND FKey = 'keto_BMS_responders_import' ", pFarmId);
            lQuery.Append("  UNION ");
            lQuery.Append(" SELECT ProgramID AS ID, pc.FValue FROM agrofactuur.PROGRAMCONFIG pc  ");
            lQuery.AppendFormat(" WHERE pc.ProgramId = {0} AND FKey = 'keto_BMS_responders_import'  ", pProgramId);
            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            if (tbl.Rows.Count > 0)
            {
                if (tbl.Rows[0][1] != DBNull.Value)
                {
                    Result = Convert.ToBoolean(tbl.Rows[0][1]);
                }
            }

            return Result;
        }
    }
}
