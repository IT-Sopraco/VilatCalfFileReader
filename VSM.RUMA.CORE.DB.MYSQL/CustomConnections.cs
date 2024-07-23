using MySql.Data.MySqlClient;
using System;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB.MYSQL
{
    public class CustomConnections
    {

        public FTPINFO getBBSFTP(String connectionString, int pProgId)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM INSTELLING WHERE (SOORTPROGID=@progId)", con);
                MySqlParameter p1 = new MySqlParameter();
                p1.ParameterName = "@progId";
                p1.Value = pProgId;
                cmd.Parameters.Add(p1);
                //sla ftp gegevens op in ftpInfo object
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    FTPINFO ftpInf = new FTPINFO();
                    ftpInf.FtpHostName = reader["INSTSERVERNAAM"].ToString();
                    ftpInf.UserName = reader["INSTLOGINNAAM"].ToString();
                    ftpInf.Password = reader["INSTPASSWORD"].ToString();
                    return ftpInf;
                }
            }

        }

        public DataTable geTable(String connectionString, String pQuery)
        {
            DataTable tbl = new DataTable();
            try
            {
                MySqlConnection con = new MySqlConnection(connectionString);
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(pQuery, con);
                    MySqlDataAdapter da = new MySqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables.Count>0)
                    {
                        tbl = ds.Tables[0];
                    }
                    
                }
                catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            catch(Exception exc)
            {
                unLogger.WriteError("getTable (2) ex: " + exc.ToString());
            }
            return tbl;
        }

        public static string getDBHost(String connectionString)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                return conn.DataSource;
            }
        }

        public int SaveObject(VSM.RUMA.CORE.DB.DataTypes.DataObject dataItem, String connectionString, string pKeyField, bool pInsert)
        {
            int ret = 0;
            MySqlConnection con = new MySqlConnection(connectionString);
            try
            {

                unMySQL un = new unMySQL(unMySQL.DBHostType.MASTER);
                string CommandQuery = "";
                if (pInsert)
                {
                    CommandQuery = un.CreateInsertCommandWithoutChangedByAndSourceID(dataItem);
                   
                }
                else
                {
                    CommandQuery = CreateUpdateCommandText(dataItem);

                }
                MySqlCommand cmd = new MySqlCommand(CommandQuery, con);
                GenerateParameters(cmd.Parameters, dataItem, "?");
                con.Open();
                ret = cmd.ExecuteNonQuery();
                if (pInsert)
                {
                   
                    foreach (PropertyInfo propertyInfo in dataItem.GetType().GetProperties())
                    {
                        if (propertyInfo.Name==pKeyField)
                        {

                            if (propertyInfo.PropertyType == typeof(Int32))
                            {
                                int pk = GetLastRowId(con);
                                
                                ret = pk;
                            }
                            else if (propertyInfo.PropertyType == typeof(UInt32))
                            {
                                uint pk = GetUIntLastRowId(con);
                                propertyInfo.SetValue(dataItem, pk, null);
                           
                                if (pk < Int32.MaxValue)
                                  ret=   Convert.ToInt32(pk);
                                
                            }
                        }
                    }
                   
                }
            }
            catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return ret;
        }

        public int DeleteObject(VSM.RUMA.CORE.DB.DataTypes.DataObject dataItem, String connectionString, string[] pKeyFields)
        {
            int ret = 0;
            MySqlConnection con = new MySqlConnection(connectionString);
            try
            {

                unMySQL un = new unMySQL(unMySQL.DBHostType.MASTER);
                string CommandQuery = un.CreateDeleteCommandText(dataItem, pKeyFields);
              
    
               
                MySqlCommand cmd = new MySqlCommand(CommandQuery, con);
                GenerateParameters(cmd.Parameters, dataItem, "?");
                con.Open();
                ret = cmd.ExecuteNonQuery();
 
            }
            catch (Exception exc) { unLogger.WriteError(exc.ToString()); }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return ret;
        }

        private void GenerateParameters(MySqlParameterCollection parameterCollection, DataObject pData, String pPrefix)
        {

            MySqlParameter lParam;
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                if (propertyInfo.GetValue(pData, null) == null)
                {
                    lParam = (parameterCollection as MySqlParameterCollection).AddWithValue(pPrefix + propertyInfo.Name, null);
                    lParam.SourceColumn = propertyInfo.Name;
                    lParam.SourceVersion = DataRowVersion.Current;
                    continue;
                }

                if (propertyInfo.GetValue(pData, null).GetType() == Type.GetType("System.DateTime"))
                {
                    DateTime ParamValue = Convert.ToDateTime(propertyInfo.GetValue(pData, null));
                    if (ParamValue == DateTime.MinValue)
                    {
                        lParam = (parameterCollection as MySqlParameterCollection).AddWithValue(pPrefix + propertyInfo.Name, null);
                        lParam.SourceColumn = propertyInfo.Name;
                        lParam.SourceVersion = DataRowVersion.Current;
                        continue;
                    }
                }
                lParam = (parameterCollection as MySqlParameterCollection).AddWithValue(pPrefix + propertyInfo.Name, propertyInfo.GetValue(pData, null));
                lParam.SourceColumn = propertyInfo.Name;
                lParam.SourceVersion = DataRowVersion.Current;
            }
        }

        private int GetLastRowId(MySqlConnection conn)
        {
            using (MySqlDataReader reader = MySql.Data.MySqlClient.MySqlHelper.ExecuteReader((MySql.Data.MySqlClient.MySqlConnection)conn, "SELECT LAST_INSERT_ID();"))
            {
                reader.Read();
                int Result = Convert.ToInt32(reader.GetValue(0));
                return Result;
            }
        }

        private uint GetUIntLastRowId(MySqlConnection conn)
        {
            using (MySqlDataReader reader = MySql.Data.MySqlClient.MySqlHelper.ExecuteReader((MySql.Data.MySqlClient.MySqlConnection)conn, "SELECT LAST_INSERT_ID();"))
            {
                reader.Read();
                uint Result = Convert.ToUInt32(reader.GetValue(0));
                return Result;
            }

        }

        public String CreateUpdateCommandText(DataObject pData)
        {

            StringBuilder Result = new StringBuilder("UPDATE " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType" && datat.Name != "Changed_By" && datat.Name != "SourceID").ToArray();
            Result.Append(" SET ");

            List<String> PrimaryKeyFields = new List<String>();
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                if (propertyInfo.GetCustomAttributes(true).Where(ca => ca is System.ComponentModel.DataObjectFieldAttribute && (ca as System.ComponentModel.DataObjectFieldAttribute).PrimaryKey).Count() > 0)
                {
                    PrimaryKeyFields.Add(propertyInfo.Name);
                    continue;
                }

                Result.Append(propertyInfo.Name + " = ");
                Result.Append("?" + propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" WHERE ");
            foreach (String KeyName in PrimaryKeyFields)
            {
                Result.Append(KeyName + " = ");
                Result.Append("?" + KeyName);
                Result.Append(" AND ");
            }
            return Result.ToString().Substring(0, Result.Length - 4);
        }
    }
}
