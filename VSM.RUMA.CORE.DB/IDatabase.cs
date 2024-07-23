using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
namespace VSM.RUMA.CORE
{
    [Obsolete("interface IDatabase")]
    public interface IDatabase
    {
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        int ExecuteNonQueryCommand(System.Data.Common.DbCommand pCommand);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        System.Data.Common.DbCommand CreateCommand(DBConnectionToken pToken);

        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        System.Data.Common.DbCommand CreateParameters(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject pData, String pPrefix);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        System.Data.Common.DbCommand CreateParameters(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject pData);


        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        bool AddParameter(System.Data.Common.DbCommand pCmd, String pName, object pValue);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        bool FillObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject pData, System.Data.Common.DbCommand pCommand);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        bool FillObject(DBConnectionToken pToken, System.Collections.Generic.List<VSM.RUMA.CORE.DB.DataTypes.DataObject> pDataList, System.Data.Common.DbCommand pCommand, VSM.RUMA.CORE.DB.DataTypes.DataObject pData);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        System.Data.DataTable QueryData(DBConnectionToken pToken, System.Data.DataSet pDataSet, System.Data.Common.DbCommand pCommand, string TableName);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        System.Data.DataTable QueryData(DBConnectionToken pToken, System.Data.Common.DbCommand pCommand);
        [Obsolete("Gebruik DbCommand niet ivm disposables")]
        System.Data.DataTable QueryData(DBConnectionToken pToken, DataSet pDataSet, DbCommand pCommand, String TableName, System.Data.MissingSchemaAction MissingSchemaAction);

        [Obsolete("Disconnect gaat tegenwoordig automatisch met pooling")]
        void ForceDisconnect(DBConnectionToken pToken);


        string CreateInsertCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData);
        String CreateInsertCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, String[] pKeyFields);
        string CreateInsertCommandWithoutChangedByAndSourceID(VSM.RUMA.CORE.DB.DataTypes.DataObject pData);
        //string CreateInsertCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, Boolean pDelayed, Boolean pLowPriority);
        string CreateReplaceCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData);
        string CreateUpdateCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, string pKeyField, string[] pUpdateFields);
        string CreateUpdateALLCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, String[] pKeyFields);
        string CreateDeleteCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, String[] pKeyFields);
        string CreateSetNegativeCommandText(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, String[] pKeyFields);

        int InsertObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject dataItem);
        int InsertObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject dataItem, string database, String[] pKeyFields);
        int UpdateObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject dataItem, string[] primaryKey);
        int ModifyObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject dataItem, String CommandText);

        int ExecuteNonQuery(DBConnectionToken pToken, String CommandText);
        int InsertQuery(DBConnectionToken pToken, string query, List<KeyValuePair<string, object>> parameters);

        bool FillObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject pData, System.Text.StringBuilder pQuery);
        bool FillObject(DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.DataObject pData, System.Data.DataRow drData);
        bool FillObject(DBConnectionToken pToken, System.Collections.Generic.List<VSM.RUMA.CORE.DB.DataTypes.DataObject> pDataList, System.Text.StringBuilder pQuery, Type DataType);
        bool FillObject(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, System.Data.DataRow drData);
        System.Data.DataTable QueryData(DBConnectionToken pToken, System.Data.DataSet pDataSet, System.Text.StringBuilder pQueryBuilder, string TableName);
        System.Data.DataTable QueryData(DBConnectionToken pToken, System.Data.DataSet pDataSet, System.Text.StringBuilder pQueryBuilder, String TableName, System.Data.MissingSchemaAction MissingSchemaAction);
        System.Data.DataTable QueryData(DBConnectionToken pToken, System.Text.StringBuilder pQueryBuilder);
        System.Data.DataTable QueryData(DBConnectionToken pToken, System.Text.StringBuilder pQueryBuilder, System.Data.MissingSchemaAction MissingSchemaAction);
        string CreateUpdateCommandTextDP(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, string[] pkeys, string[] UpdateParams);

        int GetSingleInt(DBConnectionToken pToken, string qry);
        object GetSingleValue(DBConnectionToken pToken, string qry);
        string getDBHost();
        int GetLastRowId(IDbConnection conn);
    }
}
