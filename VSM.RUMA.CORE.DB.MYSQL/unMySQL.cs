using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Timers;
using System.Text;
using System.Configuration;
using System.Runtime.CompilerServices;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Threading;

namespace VSM.RUMA.CORE.DB.MYSQL
{

    class ConnnectionWaiter
    {
        private static AutoResetEvent connectionLock = new AutoResetEvent(false);
        internal void WaitForConnection(MySqlConnection con)
        {
            con.StateChange += new StateChangeEventHandler(con_StateChange);
            connectionLock.WaitOne();
            con.StateChange -= new StateChangeEventHandler(con_StateChange);
        }

        void con_StateChange(object sender, StateChangeEventArgs e)
        {
            connectionLock.Set();
        }
    }

    class unMySQL : TokenReader, IDatabase
    {
        readonly static int QRYTIMEOUT = 6000;

        private static bool regCleanup = false;

        private int autherrors = 0;
        
        internal enum DBHostType
        {
            MASTER = 1,
            SLAVE = 2,

        }

        private DBHostType hosttype;

        protected internal unMySQL(DBHostType HostType)
        //: base(this)
        {
            if (!regCleanup)
            {
                AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
                regCleanup = true;
            }

            this.hosttype = HostType;
        }

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            ClearAllPools();
        }
        
        internal void LogQueryData(string Heading, DbCommand cmd, Exception ex, TimeSpan? elapsed)
        {
            StringBuilder Message = new StringBuilder("----- " + Heading + " -----" + Environment.NewLine);

            string url = "Unknown";
            if (System.Web.HttpContext.Current != null)
                url = System.Web.HttpContext.Current.Request.Url.ToString();
            Message.AppendFormat("Url:              {0}{1}", url, Environment.NewLine);
            Message.AppendLine();
            Message.AppendFormat("Connection:       {0}", cmd.Connection.ToString());
            Message.AppendLine();
            Message.AppendFormat("Connection State: {0}", cmd.Connection.State.ToString());
            Message.AppendLine();
            Message.AppendFormat("Connection is up: {0}", ConnUpCheck(cmd.Connection));
            Message.AppendLine();
            Message.AppendLine();
            Message.AppendFormat("DataSource: {0}", cmd.Connection.DataSource);
            Message.AppendLine();
            Message.AppendFormat("Database: {0}", cmd.Connection.Database);
            Message.AppendLine();
            Message.AppendLine("Query: ");
            Message.AppendFormat("{0}", cmd.CommandText);
            Message.AppendLine();
            Message.AppendLine("Parameters: ");
            foreach (DbParameter param in cmd.Parameters)
            {
                string paramName = param.ParameterName ?? "null";
                string paramValue = "null";
                if (param.Value != null)
                    paramValue = param.Value.ToString();

                Message.AppendFormat("{0}: {1}{2}", paramName, paramValue, Environment.NewLine);
            }
            Message.AppendLine();

            if (ex != null)
            {
                Message.AppendLine("Exception: ");

                if (ex is MySqlException)
                    Message.AppendFormat("MySQL Error #{0} : {1}{2}", (ex as MySqlException).Number, ex, Environment.NewLine);
                else
                    Message.AppendFormat("{0}{1}", ex, Environment.NewLine);
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Message.AppendLine("InnerException: ");
                    Message.AppendFormat("{0}{1}", innerEx, Environment.NewLine);
                    innerEx = innerEx.InnerException;
                }
            }

            if (elapsed != null)
            {
                Message.AppendFormat("Time elapsed: {0}{1}", elapsed.Value, Environment.NewLine);
            }
            Message.AppendLine("-------------------------------------------- " + Heading + " -----");

            unLogger.WriteError("MYSQLDB", Message.ToString());
        }
        
        internal void LogQueryData(string Heading, String ConnectionString, String CommandText, List<MySqlParameter> Parameters, Exception ex, TimeSpan? elapsed)
        {
            string shortmsg = $"{Heading}: {CommandText}";

            StringBuilder Message = new StringBuilder("----- " + Heading + " -----" + Environment.NewLine);

            string url = "Unknown";
            if (System.Web.HttpContext.Current != null)
                url = System.Web.HttpContext.Current.Request.Url.ToString();
            Message.AppendFormat("Url:              {0}{1}", url, Environment.NewLine);
            Message.AppendLine();

            var mcsb = new MySqlConnectionStringBuilder(ConnectionString);
            mcsb.Password = "".PadLeft(mcsb.Password.Length, '*');                 

            Message.AppendFormat("ConnectionString:       {0}", mcsb.ToString());
            Message.AppendLine();
            Message.AppendLine("Query: ");
            Message.AppendFormat("{0}", CommandText);
            Message.AppendLine();
            Message.AppendLine("Parameters: ");
            foreach (DbParameter param in Parameters)
            {
                string paramName = param.ParameterName ?? "null";
                string paramValue = "null";
                if (param.Value != null)
                    paramValue = param.Value.ToString();

                Message.AppendFormat("{0}: {1}{2}", paramName, paramValue, Environment.NewLine);
            }
            Message.AppendLine();

            if (ex != null)
            {
                Message.AppendLine("Exception: ");

                if (ex is MySqlException)
                    Message.AppendFormat("MySQL Error #{0} : {1}{2}", (ex as MySqlException).Number, ex, Environment.NewLine);
                else
                    Message.AppendFormat("{0}{1}", ex, Environment.NewLine);
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Message.AppendLine("InnerException: ");
                    Message.AppendFormat("{0}{1}", innerEx, Environment.NewLine);
                    innerEx = innerEx.InnerException;
                }
            }

            if (elapsed != null)
            {
                Message.AppendFormat("Time elapsed: {0}{1}", elapsed.Value, Environment.NewLine);
            }
            Message.AppendLine("-------------------------------------------- " + Heading + " -----");


            unLogger.WriteError(shortmsg);
            unLogger.WriteDebug(Message.ToString());
            unLogger.WriteError(ex.ToString());
        }
       
        
        internal String CreateMasterConnectionString(DBConnectionToken pToken)
        {
            return pToken.MasterConnectionString;
            /*
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] == null && hosttype != DBHostType.MASTER)
            { }   //return CreateMasterConnectionString(pToken);
            MySqlConnectionStringBuilder lConnectionString;
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null
                && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString.ToLower().Contains("user id=")
                && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString.ToLower().Contains("pwd="))
            {

                lConnectionString = new MySqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString);
            }
            else
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ConnectionOptions"]))
                {
                    lConnectionString = new MySqlConnectionStringBuilder();
                    lConnectionString.UseCompression = true;
                    lConnectionString.SslMode = MySqlSslMode.Preferred;
                    lConnectionString.AllowZeroDateTime = true;
                }
                else lConnectionString = new MySqlConnectionStringBuilder(ConfigurationManager.AppSettings["ConnectionOptions"]);
                if (String.IsNullOrEmpty(lConnectionString.UserID))
                {
                    lConnectionString.UserID = ReadToken(pToken, "MasterUser");
                    lConnectionString.Password = ReadToken(pToken, "MasterPass");
                }
                else
                {
                    SettingsDecrypter Decrypter = new SettingsDecrypter();
                    Decrypter.DecryptConfigPW(ref lConnectionString);
                }
                lConnectionString.Server = ReadToken(pToken, "MasterIP");
                unLogger.SetThreadContext("MasterIP", ReadToken(pToken, "MasterIP"));
            }
            lConnectionString.Database = ReadToken(pToken, "MasterDB");

            return lConnectionString.ToString();
        */
        }

        private String CreateReadOnlyConnectionString(DBConnectionToken pToken)
        {
            return pToken.SlaveConnectionString;
            /*
            MySqlConnectionStringBuilder lConnectionString;
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
            {
                return CreateMasterConnectionString(pToken);
            }
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ConnectionOptions"]))
            {
                lConnectionString = new MySqlConnectionStringBuilder();
                lConnectionString.UseCompression = true;
                lConnectionString.SslMode = MySqlSslMode.Preferred;
                lConnectionString.AllowZeroDateTime = true;
            }
            else lConnectionString = new MySqlConnectionStringBuilder(ConfigurationManager.AppSettings["ConnectionOptions"]);
            if (String.IsNullOrEmpty(lConnectionString.UserID))
            {
                lConnectionString.UserID = ReadToken(pToken, "SlaveUser");
                lConnectionString.Password = ReadToken(pToken, "SlavePass");
            }
            else
            {
                SettingsDecrypter Decrypter = new SettingsDecrypter();
                Decrypter.DecryptConfigPW(ref lConnectionString);
            }


            lConnectionString.Server = ReadToken(pToken, "SlaveIP");
            lConnectionString.Database = ReadToken(pToken, "SlaveDB");

            return lConnectionString.ToString();
            */
        }
        
        internal void LogConnectData(string Heading, MySqlConnection pConnection, Exception ex, TimeSpan? elapsed)
        {
            StringBuilder Message = new StringBuilder("----- " + Heading + " -----" + Environment.NewLine);

            string url = "Unknown";
            if (System.Web.HttpContext.Current != null)
            {
                url = System.Web.HttpContext.Current.Request.Url.ToString();
                Message.AppendFormat("Url:              {0}{1}", url, Environment.NewLine);
            }
            else
            {
                Message.AppendFormat("AppDomain:        {0}{1}", AppDomain.CurrentDomain.FriendlyName, Environment.NewLine);
                Message.AppendFormat("Thread:           {0}", Thread.CurrentThread.Name);
            }
            Message.AppendLine();
            Message.AppendFormat("Connection:       {0}", pConnection.ToString());
            Message.AppendLine();
            Message.AppendFormat("Connection State: {0}", pConnection.State.ToString());
            Message.AppendLine();
            Message.AppendFormat("Connection is up: {0}", ConnUpCheck(pConnection));
            Message.AppendLine();
            Message.AppendLine();
            Message.AppendFormat("DataSource:       {0}", pConnection.DataSource);
            Message.AppendLine();
            Message.AppendFormat("Database:         {0}", pConnection.Database);
            Message.AppendLine();
            Message.AppendLine("ConnectionString: ");
            Message.AppendFormat("{0}", pConnection.ConnectionString);
            Message.AppendLine();

            if (ex != null)
            {
                Message.AppendLine("Exception: ");

                if (ex is MySqlException)
                    Message.AppendFormat("MySQL Error #{0} : {1}{2}", (ex as MySqlException).Number, ex, Environment.NewLine);
                else
                    Message.AppendFormat("{0}{1}", ex, Environment.NewLine);
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Message.AppendLine("InnerException: ");
                    Message.AppendFormat("{0}{1}", innerEx, Environment.NewLine);
                    innerEx = innerEx.InnerException;
                }
            }

            if (elapsed != null)
            {
                Message.AppendFormat("Time elapsed: {0}{1}", elapsed.Value, Environment.NewLine);
            }
            Message.AppendLine("-------------------------------------------- " + Heading + " -----");

            unLogger.WriteError("MYSQLDB", Message.ToString());
        }
        
        private static bool ConnUpCheck(IDbConnection c)
        {
            if (!(c is MySqlConnection))
            {
                unLogger.WriteDebug("#NOMYSQL ConnUpCheck Failed: Connection is not a MySqlConnection");
                return false;
            }
            MySqlConnection con = c as MySqlConnection;
            try
            {

                switch (con.State)
                {
                    case ConnectionState.Closed:
                    case ConnectionState.Broken:
                        return false;
                    case ConnectionState.Executing:
                    case ConnectionState.Fetching:
                        unLogger.WriteDebugFormat("DB", "[{0}] ConnUpCheck Connection Busy: ServerThreadId {1}", con.Database, con.ServerThread);
                        ConnnectionWaiter conwaiter = new ConnnectionWaiter();
                        conwaiter.WaitForConnection(con);
                        return ConnUpCheck(con);
                    case ConnectionState.Connecting:
                        unLogger.WriteDebugFormat("DB", "[{0}] ConnUpCheck Connection Connecting: ServerThreadId {1}", con.Database, con.ServerThread);
                        ConnnectionWaiter conopenwaiter = new ConnnectionWaiter();
                        conopenwaiter.WaitForConnection(con);
                        return ConnUpCheck(con);
                    default:
                        return con.Ping();
                }
            }
            catch (MySqlException exc)
            {


                unLogger.WriteDebug(String.Format("#{0} ServerThreadId {1} ConnUpCheck Failed: {2}", exc.Number, con.ServerThread, exc.Message), exc);
                return false;

            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(String.Format("#UNKNOWN ServerThreadId {0} ConnUpCheck Failed: {1}", con.ServerThread, exc.Message), exc);
                return false;
            }
        }
        
        public void ForceDisconnect(DBConnectionToken pToken)
        {
            unLogger.WriteTraceFormat("DB", "Connection.ForceDisconnect TokenHash: {0}", pToken.GetHashCode());

        }

        public DbCommand CreateCommand(DBConnectionToken pToken)
        {
            lock (pToken)
            {
                MySqlConnection con = new MySqlConnection(CreateMasterConnectionString(pToken));
                con.Open();
                return con.CreateCommand();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ExecuteNonQueryCommand(DbCommand pCommand)
        {
            try
            {
                if (pCommand.Connection.State != ConnectionState.Open && !ConnUpCheck(pCommand.Connection))
                {
                    unLogger.WriteWarnFormat("DB", "[{0}] Connection was closed or broken, reopen connection", pCommand.Connection.Database);
                    unLogger.WriteDebugFormat("DB", "Broken Connection State {0} Query: {1}", pCommand.Connection.State.ToString(), pCommand.CommandText);
                    String DB = pCommand.Connection.Database;
                    pCommand.Connection.Close();
                    pCommand.Connection.Open();
                    if (pCommand.Connection.Database != DB)
                        ((MySqlConnection)pCommand.Connection).ChangeDatabase(DB);
                }

                return pCommand.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException exc)
            {
                LogQueryData("ExecuteNonQuery Exception", pCommand, exc, null);
//                int res = exc.Number;
                switch (exc.Number)
                {
                    case 1205:
                        unLogger.WriteWarnFormat("DB", "DB Lock wait Time-out Query: {0}", pCommand.CommandText);
                        break;
                    case 1213:
                        unLogger.WriteWarnFormat("DB", "DB Deadlock when trying to get lock Query: {0}", pCommand.CommandText);
                        break;
                    case 1046:
                        unLogger.WriteWarnFormat("DB", "DB No database selected: {0}", pCommand.CommandText);
                        break;
                    case 1062:
                        unLogger.WriteWarnFormat("DB", "DB  Duplicate entry: {0}", pCommand.CommandText);
                        break;

                    case 0:
                        if (exc.Message.Contains("'mysql_native_password'") && autherrors < 10)
                        {
                            Interlocked.Increment(ref autherrors);
                            LogQueryData("ExecuteNonQuery Auth Exception", pCommand, exc, null);
                            ClearAllPools();
                            int rows = ExecuteNonQueryCommand(pCommand);
                            Interlocked.Decrement(ref autherrors);
                            return rows;
                        }
                        break;
                }
                throw;
                //if (res == 1205 || res == 1213)
                //{
                //    continue;
                //}
                //else if (res == 1046) // No database selected
                //{
                //    String DB = pCommand.Connection.Database;
                //    ((MySqlConnection)pCommand.Connection).ChangeDatabase(DB);
                //    if (current >= 5)
                //    {
                //        unLogger.WriteDebug("No Database selected.. Quit trying to execute command");
                //        throw exc;
                //    }
                //}
                //else if (res == 1062) // Duplicate entry
                //{
                //    throw exc;
                //}
                //else if (current <= 10) // retry
                //{

                //    if (current >= 5)
                //    {
                //        try
                //        {
                //            String DB = pCommand.Connection.Database;
                //            pCommand.Connection.Close();
                //            pCommand.Connection.Open();
                //            if (pCommand.Connection.Database != DB)
                //                ((MySqlConnection)pCommand.Connection).ChangeDatabase(DB);
                //        }
                //        catch (Exception ex)
                //        {
                //            unLogger.WriteError("Error Reopening connection", ex);
                //        }
                //    }
                //    continue;
                //}
                //else
                //{
                //    throw exc;
                //}
            }
            catch (Exception ex)
            {
                LogQueryData("ExecuteNonQuery Exception", pCommand, ex, null);
                throw;
            }
        }
        
        public int ModifyObject(DBConnectionToken pToken, DataObject dataItem, String CommandText)
        {
            List<MySqlParameter> Parameters = new List<MySqlParameter>();
            try
            {
                MySqlParameter lParam;
                PropertyInfo[] lDataProperties;
                lDataProperties = dataItem.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in lDataProperties)
                {
                    if (propertyInfo.GetValue(dataItem, null) == null)
                    {

                        lParam = new MySqlParameter("?" + propertyInfo.Name, null);
                        lParam.SourceColumn = propertyInfo.Name;
                        lParam.SourceVersion = DataRowVersion.Current;
                        Parameters.Add(lParam);
                        continue;
                    }

                    if (propertyInfo.GetValue(dataItem, null).GetType() == Type.GetType("System.DateTime"))
                    {
                        DateTime ParamValue = Convert.ToDateTime(propertyInfo.GetValue(dataItem, null));
                        if (ParamValue == DateTime.MinValue)
                        {
                            lParam = new MySqlParameter("?" + propertyInfo.Name, null);
                            lParam.SourceColumn = propertyInfo.Name;
                            lParam.SourceVersion = DataRowVersion.Current;
                            Parameters.Add(lParam);
                            continue;
                        }
                    }
                    lParam = new MySqlParameter("?" + propertyInfo.Name, propertyInfo.GetValue(dataItem, null));
                    lParam.SourceColumn = propertyInfo.Name;
                    lParam.SourceVersion = DataRowVersion.Current;
                    Parameters.Add(lParam);
                }
                return MySqlHelper.ExecuteNonQuery(CreateMasterConnectionString(pToken), CommandText, Parameters.ToArray());

            }
            catch (MySql.Data.MySqlClient.MySqlException exc)
            {
                LogQueryData("ModifyObject Exception", CreateMasterConnectionString(pToken), CommandText, Parameters, exc, null);
                int res = exc.Number;
                switch (exc.Number)
                {
                    case 1205:
                        unLogger.WriteWarnFormat("DB", "DB Lock wait Time-out Query: {0}", CommandText);
                        break;
                    case 1213:
                        unLogger.WriteWarnFormat("DB", "DB Deadlock when trying to get lock Query: {0}", CommandText);
                        break;
                    case 1046:
                        unLogger.WriteWarnFormat("DB", "DB No database selected: {0}", CommandText);
                        break;
                    case 1062:
                        unLogger.WriteWarnFormat("DB", "DB  Duplicate entry: {0}", CommandText);
                        break;
                    case 0:
                        if (exc.Message.Contains("'mysql_native_password'") && autherrors < 10)
                        {
                            Interlocked.Increment(ref autherrors);
                            LogQueryData("ModifyObject Auth ", CreateMasterConnectionString(pToken), CommandText, Parameters, exc, null);
                            ClearAllPools();
                            int rows = ModifyObject(pToken, dataItem, CommandText);
                            Interlocked.Decrement(ref autherrors);
                            return rows;
                        }
                        break;
                }
                //return -1;
                throw;
            }
            catch (Exception ex)
            {
                LogQueryData("ModifyObject Exception", CreateMasterConnectionString(pToken), CommandText, Parameters, ex, null);
                throw;
            }

        }

        public int InsertQuery(DBConnectionToken pToken, string query, List<KeyValuePair<string, object>> parameters)
        {
            int ret = 0;
            List<MySqlParameter> mysqlparameters = new List<MySqlParameter>();
            foreach (KeyValuePair<string, object> val in parameters)
            {
                mysqlparameters.Add(new MySqlParameter(val.Key, val.Value));
            }
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandText = query;

                cmd.Parameters.AddRange(mysqlparameters.ToArray());
                var mysql = new MySqlConnection(pToken.MasterConnectionString);
                cmd.Connection = mysql;
                try
                {
                    mysql.Open();
                    ret = cmd.ExecuteNonQuery();
                    ret = GetLastRowId(mysql);
                }
                catch (Exception exc)
                {
                    unLogger.WriteError(query + ":" + exc.ToString());
                }
                finally
                {
                    if (mysql.State == ConnectionState.Open)
                    {
                        mysql.Close();
                    }
                }

            }
            return ret;
        }



        public int ExecuteNonQuery(DBConnectionToken pToken, String CommandText)
        {
            //int MAX_ATTEMPS = 11;
            //int current = 0;
            //while (current++ < MAX_ATTEMPS)
            //{
            try
            {
                return MySqlHelper.ExecuteNonQuery(CreateMasterConnectionString(pToken), CommandText);

            }
            catch (MySql.Data.MySqlClient.MySqlException exc)
            {
                LogQueryData("ExecuteNonQuery MySqlException", CreateMasterConnectionString(pToken), CommandText, new List<MySqlParameter>(), exc, null);
                int res = exc.Number;
                switch (exc.Number)
                {
                    case 1205:
                        unLogger.WriteWarnFormat("DB", "DB Lock wait Time-out Query: {0}", CommandText);
                        break;
                    case 1213:
                        unLogger.WriteWarnFormat("DB", "DB Deadlock when trying to get lock Query: {0}", CommandText);
                        break;
                    case 1046:
                        unLogger.WriteWarnFormat("DB", "DB No database selected: {0}", CommandText);
                        break;
                    case 1062:
                        unLogger.WriteWarnFormat("DB", "DB  Duplicate entry: {0}", CommandText);
                        break;
                    case 0:
                        if (exc.Message.Contains("'mysql_native_password'") && autherrors < 10)
                        {
                            Interlocked.Increment(ref autherrors);
                            LogQueryData("ExecuteNonQuery Auth Error", CreateMasterConnectionString(pToken), CommandText, new List<MySqlParameter>(), exc, null);
                            ClearAllPools();
                            int rows = ExecuteNonQuery(pToken, CommandText);
                            Interlocked.Decrement(ref autherrors);
                            return rows;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                //LogQueryData("ExecuteNonQuery Exception", pCommand, ex, null);
                LogQueryData("ExecuteNonQuery Exception", CreateMasterConnectionString(pToken), CommandText, new List<MySqlParameter>(), ex, null);
                throw;
            }


            //}
            return 0;
        }
        











        public bool FillObject(DBConnectionToken pToken, DataObject pData, DbCommand pCommand)
        {
            lock (pToken)
            {
                using (DbDataReader reader = pCommand.ExecuteReader())
                {
                    try
                    {




                        if (!reader.Read())
                            return false;

                        PropertyInfo[] lDataProperties;
                        lDataProperties = pData.GetType().GetProperties();
                        foreach (PropertyInfo propertyInfo in lDataProperties)
                        {
                            object Value;
                            try
                            {
                                int lOrdinal = reader.GetOrdinal(propertyInfo.Name);
                                Value = reader.GetValue(lOrdinal);
                            }
                            catch (Exception ex)
                            {
                                unLogger.WriteInfo(
                                    "FillObject " + pData.GetType().Name + "." + propertyInfo.Name, ex);
                                Value = DBNull.Value;
                            }

                            if (propertyInfo.PropertyType == typeof(Nullable<int>))
                            {
                                if (Value == null || Value.ToString() == "")
                                    Value = null;
                                else
                                    Value = int.Parse(Value.ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(Nullable<Int16>))
                            {
                                if (Value == null || Value.ToString() == "")
                                    Value = null;
                                else
                                    Value = Int16.Parse(Value.ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(Nullable<decimal>))
                            {
                                if (Value == null || Value.ToString() == "")
                                    Value = null;
                                else
                                    Value = decimal.Parse(Value.ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(Nullable<sbyte>))
                            {
                                if (Value == null || Value.ToString() == "")
                                    Value = null;
                                else
                                    Value = sbyte.Parse(Value.ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(Nullable<float>))
                            {
                                if (Value == null || Value.ToString() == "")
                                    Value = null;
                                else
                                    Value = float.Parse(Value.ToString());
                            }
                            else if (Value != DBNull.Value)
                                Value = Convert.ChangeType(Value, propertyInfo.PropertyType);
                            else
                                Value = null;

                            propertyInfo.SetValue(pData, Value, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError(
                            $"{nameof(unMySQL)}.{nameof(FillObject)} (1) Type: {pData.GetType().Name} Ex: {ex.Message}",
                            ex);
                        return false;
                    }
                }
                setFilledByDb(pData, true);
                return true;
            }
        }

        public bool FillObject(DBConnectionToken pToken, List<DataObject> pDataList, DbCommand pCommand, DataObject pData)
        {
            lock (pToken)
            {
                using (DbDataReader reader = pCommand.ExecuteReader())
                {
                    try
                    {
                        DataObject lData;

                        while (reader.Read()) //reader.NextResult())
                        {
                            lData = (DataObject) Activator.CreateInstance(pData.GetType());
                            PropertyInfo[] lDataProperties;
                            lDataProperties = lData.GetType().GetProperties();

                            foreach (PropertyInfo propertyInfo in lDataProperties)
                            {
                                int lOrdinal = reader.GetOrdinal(propertyInfo.Name);
                                object Value = Convert.ChangeType(reader.GetValue(lOrdinal), propertyInfo.PropertyType);
                                propertyInfo.SetValue(lData, Value, null);
                            }
                            setFilledByDb(pData, true);
                            pDataList.Add(lData);
                        }
                    }
                    catch (Exception ex)
                    {                        
                        unLogger.WriteError($"{nameof(unMySQL)}.{nameof(FillObject)} (2) Type: {pData.GetType().Name} Ex: {ex.Message}", ex);
                        return false;
                    }

                    return true;
                }
            }
        }



        public bool FillObject(DataObject pData, DataRow drData)
        {
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties();
//            int i = 0;

            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                //

                //unLogger.WriteInfo($"pdata PropertyInfo name:{propertyInfo.Name} ");
                //unLogger.WriteInfo($"drdata Table column name:{drData.Table.Columns[i].ColumnName} ");
                //i++;

                object Value;
                if (drData.Table.Columns.Contains(propertyInfo.Name) &&
                    (propertyInfo.PropertyType == typeof(DateTime)) &&
                    drData[propertyInfo.Name].GetType() == typeof(MySql.Data.Types.MySqlDateTime)
                    && ((MySql.Data.Types.MySqlDateTime)drData[propertyInfo.Name]).Year == 0)
                {
                    DateTime dt = DateTime.MinValue;
                    DateTime.TryParse(drData[propertyInfo.Name].ToString(), out dt);
                    Value = dt;
                }
                else if (drData.Table.Columns.Contains(propertyInfo.Name)
                         && (propertyInfo.PropertyType == typeof(Nullable<int>)))
                {
                    if (drData[propertyInfo.Name] == null || drData[propertyInfo.Name].ToString() == "")
                        Value = null;
                    else
                        Value = int.Parse(drData[propertyInfo.Name].ToString());
                }
                else if (drData.Table.Columns.Contains(propertyInfo.Name)
                         && (propertyInfo.PropertyType == typeof(Nullable<Int16>)))
                {
                    if (drData[propertyInfo.Name] == null || drData[propertyInfo.Name].ToString() == "")
                        Value = null;
                    else
                        Value = Int16.Parse(drData[propertyInfo.Name].ToString());
                }
                else 
                if (drData.Table.Columns.Contains(propertyInfo.Name)
                         && (propertyInfo.PropertyType == typeof(Nullable<decimal>)))
                {
                    if (drData[propertyInfo.Name] == null || drData[propertyInfo.Name].ToString() == "")
                        Value = null;
                    else
                        Value = decimal.Parse(drData[propertyInfo.Name].ToString());
                }
                else if (drData.Table.Columns.Contains(propertyInfo.Name)
                         && (propertyInfo.PropertyType == typeof(Nullable<sbyte>)))
                {
                    if (drData[propertyInfo.Name] == null || drData[propertyInfo.Name].ToString() == "")
                        Value = null;
                    else
                        Value = sbyte.Parse(drData[propertyInfo.Name].ToString());
                }
                else if (drData.Table.Columns.Contains(propertyInfo.Name)
                         && (propertyInfo.PropertyType == typeof(Nullable<float>)))
                {
                    if (drData[propertyInfo.Name] == null || drData[propertyInfo.Name].ToString() == "")
                        Value = null;
                    else
                        Value = float.Parse(drData[propertyInfo.Name].ToString());
                }
                else if (propertyInfo.PropertyType.IsEnum)
                {
                    if (drData[propertyInfo.Name] == null || drData[propertyInfo.Name].ToString() == "")
                        Value = 0;
                    else
                        Value = int.Parse(drData[propertyInfo.Name].ToString());

                    var val = Enum.ToObject(propertyInfo.PropertyType, Value);
                    Convert.ChangeType(val, propertyInfo.PropertyType);
                    Value = val;
                }
                else if (drData.Table.Columns.Contains(propertyInfo.Name) && drData[propertyInfo.Name] != DBNull.Value)
                {
                    Value = Convert.ChangeType(drData[propertyInfo.Name], propertyInfo.PropertyType);
                }
                else
                {
                    Value = null;
                }
                propertyInfo.SetValue(pData, Value, null);
            }
            setFilledByDb(pData, true);
            return true;
        }

        public bool FillObject(DBConnectionToken pToken, DataObject pData, StringBuilder pQuery)
        {

            DataTable dtData = QueryData(pToken, pQuery);
            if (dtData.Rows.Count == 0) return false;
            DataRow drData = dtData.Rows[0];
            return FillObject(pData, drData);
        }

        public bool FillObject(DBConnectionToken pToken, List<DataObject> pDataList, StringBuilder pQuery, Type DataType)
        {
            DataTable dtData = QueryData(pToken, pQuery);
            foreach (DataRow drData in dtData.Rows)
            {
                DataObject pData = (DataObject)DataType.InvokeMember(null, BindingFlags.Public | BindingFlags.CreateInstance, null, null, null);
                if (!FillObject(pData, drData))
                {
                    return false;
                }

                pDataList.Add(pData);
            }

            return true;
        }

        public bool FillObject(DBConnectionToken pToken, DataObject pData, System.Data.DataRow drData)
        {
            return FillObject(pData, drData);
        }

        [Obsolete("Gebruik GenerateParameters")]
        public DbCommand CreateParameters(DBConnectionToken pToken, DataObject pData)
        {
            return CreateParameters(pToken, pData, "?");
        }

        [Obsolete("Gebruik GenerateParameters")]
        public DbCommand CreateParameters(DBConnectionToken pToken, DataObject pData, String pPrefix)
        {
            lock (pToken)
            {
                MySqlCommand lCmd = (MySqlCommand)this.CreateCommand(pToken);
                lCmd.CommandType = CommandType.Text;
                GenerateParameters(lCmd.Parameters, pData, pPrefix);
                return lCmd;
            }
        }

        private void GenerateParameters(DbParameterCollection parameterCollection, DataObject pData)
        {
            GenerateParameters(parameterCollection, pData, "?");
        }

        public void GenerateParameters(DbParameterCollection parameterCollection, DataObject pData, String pPrefix)
        {

            MySqlParameter lParam;
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                if (propertyInfo.Name.ToLower() != "ts")
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
        }

        public bool AddParameter(DbCommand pCmd, String pName, object pValue)
        {
            MySqlParameter lParam;
            MySqlCommand lCmd = (MySqlCommand)pCmd;
            lParam = lCmd.Parameters.AddWithValue("?" + pName, pValue);
            lParam.Direction = ParameterDirection.Input;
            return true;
        }

        public String CreateInsertCommandText(DataObject pData)
        {

            StringBuilder Result = new StringBuilder("INSERT INTO " +  pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name.ToLower() != "ts" && datat.Name.ToLower() != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" ( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append(propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            Result.Append(" VALUES( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append("?" + propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            return Result.ToString();
        }

        public String CreateInsertCommandText(DataObject pData, String[] pKeyFields)
        {

            StringBuilder Result = new StringBuilder("INSERT INTO " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name.ToLower() != "ts" && datat.Name != "ins" && datat.Name != "ActionType" && !pKeyFields.Contains(datat.Name)).ToArray();
            Result.Append(" ( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append(propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            Result.Append(" VALUES( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append("?" + propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            return Result.ToString();
        }

        public String CreateInsertCommandWithoutChangedByAndSourceID(DataObject pData)
        {

            StringBuilder Result = new StringBuilder("INSERT INTO " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType" && datat.Name != "Changed_By" && datat.Name != "SourceID").ToArray();
            Result.Append(" ( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append(propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            Result.Append(" VALUES( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append("?" + propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            return Result.ToString();
        }

        public String CreateInsertCommandText(DataObject pData, Boolean pLowPriority)
        {

            StringBuilder Result = new StringBuilder();
            Result.Append(" INSERT");
            if (pLowPriority)
                Result.Append(" LOW_PRIORITY");

            Result.Append(" INTO " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" ( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append(propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            Result.Append(" VALUES( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.Append("?" + propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            return Result.ToString();
        }

        public String CreateUpdateCommandText(DataObject pData,
        String pKeyField, String[] pUpdateFields)
        {

            StringBuilder Result = new StringBuilder("UPDATE " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" SET ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                for (int i = 0; i < pUpdateFields.Length; i++)
                    if (propertyInfo.Name == pUpdateFields[i])
                    {
                        Result.Append(propertyInfo.Name + " = ");
                        Result.Append("?" + propertyInfo.Name + ", ");
                        break;
                    }
            }
            if(!Result.ToString().Contains("Changed_By"))
            {
                Result.Append(" Changed_By=" + pData.Changed_By.ToString() + ", ");
            }
            if (!Result.ToString().Contains("SourceID"))
            {
                Result.Append(" SourceID=" + pData.SourceID.ToString() + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" WHERE ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                if (propertyInfo.Name == pKeyField)
                {
                    Result.Append(propertyInfo.Name + " = ");
                    Result.Append("?" + propertyInfo.Name);
                }
            }

            return Result.ToString();
        }

        public String CreateUpdateALLCommandText(DataObject pData, String[] pKeyFields)
        {

            StringBuilder Result = new StringBuilder("UPDATE " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" SET ");

            var proper = from pinfo in lDataProperties select pinfo.Name;
            foreach (String PropertyName in proper.Except(pKeyFields))
            {
                Result.Append(PropertyName + " = ");
                Result.Append("?" + PropertyName + ", ");
                //break;
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" WHERE ");
            foreach (String KeyName in proper.Intersect(pKeyFields))
            {
                Result.Append(KeyName + " = ");
                Result.Append("?" + KeyName);
                Result.Append(" AND ");
            }
            return Result.ToString().Substring(0, Result.Length - 4);
        }

        public String CreateUpdateCommandText(DataObject pData)
        {

            StringBuilder Result = new StringBuilder("UPDATE " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
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

        public String CreateUpdateCommandTextDP(DataObject pData, String[] pKeyField, String[] pUpdateFields)
        {

            StringBuilder Result = new StringBuilder("UPDATE " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" SET ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                for (int i = 0; i < pUpdateFields.Length; i++)
                    if (propertyInfo.Name == pUpdateFields[i])
                    {
                        Result.Append(propertyInfo.Name + " = ");
                        Result.Append("?" + propertyInfo.Name + ", ");
                        break;
                    }
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" WHERE ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                for (int j = 0; j < pKeyField.Length; j++)
                {
                    if (propertyInfo.Name == pKeyField[j])
                    {
                        Result.Append(propertyInfo.Name + " = ");
                        Result.Append("?" + propertyInfo.Name + " and ");
                    }
                }
            }
            Result.Replace("and", "", Result.Length - 5, 5);

            return Result.ToString();
        }

        public String CreateReplaceCommandText(DataObject pData)
        {

            StringBuilder Result = new StringBuilder("REPLACE INTO " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" ( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.AppendFormat(propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            Result.Append(" VALUES( ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                Result.AppendFormat("?" + propertyInfo.Name + ", ");
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" )");
            return Result.ToString();
        }

        public String CreateDeleteCommandText(DataObject pData, String[] pKeyFields)
        {
            StringBuilder Result = new StringBuilder("DELETE FROM " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" WHERE ");
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                for (int i = 0; i < pKeyFields.Length; i++)
                {
                    if (propertyInfo.Name == pKeyFields[i])
                    {
                        Result.Append(" " + propertyInfo.Name + " = ");
                        Result.Append("?" + propertyInfo.Name);

                        Result.Append(" AND ");
                    }
                }
            }

            int start = Result.Length - 4;
            Result = Result.Remove(start, 4);
            return Result.ToString();
        }

        public String CreateSetNegativeCommandText(DataObject pData, String[] pKeyFields)
        {
            StringBuilder Result = new StringBuilder("UPDATE " + pData.GetType().Name);
            PropertyInfo[] lDataProperties;
            lDataProperties = pData.GetType().GetProperties().Where(datat => datat.Name != "ts" && datat.Name != "ins" && datat.Name != "ActionType").ToArray();
            Result.Append(" SET ");

            var proper = from pinfo in lDataProperties select pinfo.Name;
            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                for (int i = 0; i < pKeyFields.Length; i++)
                {
                    if (pKeyFields[i] == "deleted")
                    {
                        if (!Result.ToString().Contains("deleted=1"))
                        {
                            Result.Append(" deleted=1" + ", ");
                        }

                    }
                    else if (propertyInfo.Name == pKeyFields[i])
                    {
                        Result.Append(propertyInfo.Name + " =-");
                        Result.Append("?" + propertyInfo.Name + ", ");
                        break;
                    }
                   
                }
                if (propertyInfo.Name == "Changed_By")
                {
                    Result.Append(propertyInfo.Name + "=" + pData.Changed_By.ToString() + ", ");
                }
                if (propertyInfo.Name == "SourceID")
                {
                    Result.Append(propertyInfo.Name + "=" + pData.SourceID.ToString() + ", ");
                }
            }
            Result.Replace(",", "", Result.Length - 3, 3);
            Result.Append(" WHERE ");
            foreach (String KeyName in proper.Intersect(pKeyFields))
            {
                Result.Append(KeyName + " = ");
                Result.Append("?" + KeyName);
                Result.Append(" AND ");
            }
            return Result.ToString().Substring(0, Result.Length - 4);
        }

        public DataTable QueryData(DBConnectionToken pToken, StringBuilder pQueryBuilder)
        {
            return QueryData(pToken, null, pQueryBuilder, string.Empty, MissingSchemaAction.AddWithKey);
        }

        public DataTable QueryData(DBConnectionToken pToken, StringBuilder pQueryBuilder, MissingSchemaAction MissingSchemaAction)
        {
            return QueryData(pToken, null, pQueryBuilder, string.Empty, MissingSchemaAction);
        }

        public DataTable QueryData(DBConnectionToken pToken, DataSet pDataSet, StringBuilder pQueryBuilder, String TableName)
        {
            return QueryData(pToken, pDataSet, pQueryBuilder, TableName, MissingSchemaAction.AddWithKey);
        }

        public DataTable QueryData(DBConnectionToken pToken, DataSet pDataSet, StringBuilder pQueryBuilder, String TableName, MissingSchemaAction MissingSchemaAction)
        {
            MySqlDataAdapter lDataAdapter = null; ;
            DataTable lDataTable = null;

            try
            {
                lock (pToken)
                {                   
                    lDataAdapter = new MySqlDataAdapter(pQueryBuilder.ToString(), CreateReadOnlyConnectionString(pToken));
                    // toegevoegd 10 nov
                    // zet de primary key in de tabel informatie
                    lDataAdapter.MissingSchemaAction = MissingSchemaAction;// MissingSchemaAction.AddWithKey;
                    lDataAdapter.SelectCommand.CommandTimeout = QRYTIMEOUT;

                    //unLogger.WriteDebug("SQL QUERY on " + QDSlaveconn.Database + " : " + pQueryBuilder.ToString());
                    if (pDataSet == null)
                    {
                        lDataTable = new DataTable();

                    }
                    else if (pDataSet.Tables.Contains(TableName))
                    {
                        lDataTable = pDataSet.Tables[TableName];
                        lDataTable.Clear();
                    }
                    else
                    {
                        lDataTable = pDataSet.Tables.Add(TableName);
                    }

                    lDataAdapter.Fill(lDataTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException exc)
            {
                //int res = exc.Number;
                //unLogger.WriteError(String.Format("SQL Error in Query: ({0}) {1}", current, pQueryBuilder));
                //unLogger.WriteError(String.Format("#{0} : {1}", exc.Number, exc.Message), exc);
                switch (exc.Number)
                {
                    case 1205:
                        unLogger.WriteWarnFormat("DB", "DB Lock wait Time-out Query: {0}", pQueryBuilder.ToString());
                        break;
                    case 1213:
                        unLogger.WriteWarnFormat("DB", "DB Deadlock when trying to get lock Query: {0}", pQueryBuilder.ToString());
                        break;
                    case 1046:
                        unLogger.WriteWarnFormat("DB", "DB No database selected: {0}", pQueryBuilder.ToString());
                        break;
                    case 1062:
                        unLogger.WriteWarnFormat("DB", "DB  Duplicate entry: {0}", pQueryBuilder.ToString());
                        break;
                    case 0:
                        if (exc.Message.Contains("'mysql_native_password'") && autherrors < 10)
                        {
                            Interlocked.Increment(ref autherrors);
                            LogQueryData("QueryData AUTH error", CreateMasterConnectionString(pToken), pQueryBuilder.ToString(), new List<MySqlParameter>(), exc, null);
                            
                            //In logs staan deze errors meerdere keren direct achter elkaar
                            //En tests wijzen er op dat dit te maken heeft met te snel connectie opzetten, daarom sleep
                            System.Threading.Thread.Sleep(1000);

                            ClearAllPools();
                            lDataTable = QueryData(pToken, pDataSet, pQueryBuilder, TableName, MissingSchemaAction);
                            Interlocked.Decrement(ref autherrors);
                            return lDataTable;
                        }
                        break;
                }
                LogQueryData("QueryData MySqlException", CreateMasterConnectionString(pToken), pQueryBuilder.ToString(), new List<MySqlParameter>(), exc, null);


                throw;
            }
            catch (System.IO.IOException exc)
            {
                LogQueryData("QueryData IOException", CreateMasterConnectionString(pToken), pQueryBuilder.ToString(), new List<MySqlParameter>(), exc, null);
                //unLogger.WriteError(String.Format("SQL Error in Query: ({0}) {1}", current, pQueryBuilder));
                //unLogger.WriteError(String.Format("IOFout: {1}", exc.Message), exc);
                //unLogger.WriteDebug(exc.ToString());
                throw;
            }
            //}
            return lDataTable;
        }

        public DataTable QueryData(DBConnectionToken pToken, DbCommand pCommand)
        {
            return QueryData(pToken, null, pCommand, string.Empty);
        }

        public DataTable QueryData(DBConnectionToken pToken, DataSet pDataSet, DbCommand pCommand, String TableName)
        {
            return QueryData(pToken, pDataSet, pCommand, TableName, MissingSchemaAction.AddWithKey);
        }

        public DataTable QueryData(DBConnectionToken pToken, DataSet pDataSet, DbCommand pCommand, String TableName, MissingSchemaAction MissingSchemaAction)
        {
            DataTable lDataTable = null;
            try
            {

                lock (pToken)
                {
                    MySqlDataAdapter lDataAdapter = new MySqlDataAdapter((MySqlCommand)pCommand);
                    lDataAdapter.MissingSchemaAction = MissingSchemaAction;// MissingSchemaAction.AddWithKey;
                    lDataAdapter.SelectCommand.CommandTimeout = QRYTIMEOUT;
                    //unLogger.WriteInfo("SQL Command on " + pCommand.Connection.Database + " : " + pCommand.CommandText);
                    if (pDataSet == null)
                    {
                        lDataTable = new DataTable();

                    }
                    else if (pDataSet.Tables.Contains(TableName))
                    {
                        lDataTable = pDataSet.Tables[TableName];
                        lDataTable.Clear();
                    }
                    else
                    {
                        lDataTable = pDataSet.Tables.Add(TableName);
                    }
                    lDataAdapter.Fill(lDataTable);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException exc)
            {
                LogQueryData("QueryData MySqlException", pCommand, exc, null);
                int res = exc.Number;
                switch (exc.Number)
                {
                    case 1205:
                        unLogger.WriteWarnFormat("DB", "DB Lock wait Time-out Query: {0}", pCommand.CommandText);
                        break;
                    case 1213:
                        unLogger.WriteWarnFormat("DB", "DB Deadlock when trying to get lock Query: {0}", pCommand.CommandText);
                        break;
                    case 1046:
                        unLogger.WriteWarnFormat("DB", "DB No database selected: {0}", pCommand.CommandText);
                        break;
                    case 1062:
                        unLogger.WriteWarnFormat("DB", "DB  Duplicate entry: {0}", pCommand.CommandText);
                        break;
                    case 0:
                        if (exc.Message.Contains("'mysql_native_password'") && autherrors < 10)
                        {
                            Interlocked.Increment(ref autherrors);
                            LogQueryData("QueryData Exception", pCommand, exc, null);
                            ClearAllPools();
                            lDataTable = QueryData(pToken, pDataSet, pCommand, TableName, MissingSchemaAction);
                            Interlocked.Decrement(ref autherrors);
                            return lDataTable;
                        }
                        break;
                }
                throw;
            }
            catch (System.IO.IOException exc)
            {
                LogQueryData("QueryData IOException", pCommand, exc, null);
                unLogger.WriteDebug(exc.ToString());
            }
            catch (Exception exc)
            {
                LogQueryData("QueryData Exception", pCommand, exc, null);
                throw;
            }
            return lDataTable;
        }

        public int GetLastRowId(IDbConnection conn)
        {
            using (DbDataReader reader = MySql.Data.MySqlClient.MySqlHelper.ExecuteReader((MySql.Data.MySqlClient.MySqlConnection)conn, "SELECT LAST_INSERT_ID();"))
            {
                reader.Read();
                int Result = Convert.ToInt32(reader.GetValue(0));
                return Result;
            }
        }

        private uint GetUIntLastRowId(IDbConnection conn)
        {
            using (DbDataReader reader = MySql.Data.MySqlClient.MySqlHelper.ExecuteReader((MySql.Data.MySqlClient.MySqlConnection)conn, "SELECT LAST_INSERT_ID();"))
            {
                reader.Read();
                uint Result = Convert.ToUInt32(reader.GetValue(0));
                return Result;
            }

        }

        public int InsertObject(DBConnectionToken pToken, DataObject dataItem)
        {
            try
            {
                using (DbConnection con = new MySqlConnection(CreateMasterConnectionString(pToken)))
                {
                    con.Open();
                    using (DbCommand cmd = con.CreateCommand())
                    {
                        GenerateParameters(cmd.Parameters, dataItem);
                        cmd.CommandText = CreateInsertCommandText(dataItem);

                        if (ExecuteNonQueryCommand(cmd) == 1)
                        {
                            dataItem.GetType().GetProperties().Where(pi => pi.GetCustomAttributes(true).Where(ca => ca is System.ComponentModel.DataObjectFieldAttribute && (ca as System.ComponentModel.DataObjectFieldAttribute).IsIdentity).Count() > 0);

                            foreach (PropertyInfo propertyInfo in dataItem.GetType().GetProperties())
                            {
                                if (propertyInfo.GetCustomAttributes(true).Where(ca => ca is System.ComponentModel.DataObjectFieldAttribute && (ca as System.ComponentModel.DataObjectFieldAttribute).IsIdentity).Count() > 0)
                                {

                                    if (propertyInfo.PropertyType == typeof(Int32))
                                    {
                                        int pk = GetLastRowId(con);
                                        propertyInfo.SetValue(dataItem, pk, null);
                                        setFilledByDb(dataItem, true);
                                        return pk;
                                    }
                                    else if (propertyInfo.PropertyType == typeof(UInt32))
                                    {
                                        uint pk = GetUIntLastRowId(con);
                                        propertyInfo.SetValue(dataItem, pk, null);
                                        setFilledByDb(dataItem, true);
                                        if (pk < Int32.MaxValue)
                                            return Convert.ToInt32(pk);
                                        else
                                            return 0;
                                    }
                                }
                            }
                            setFilledByDb(dataItem, true);
                            return 0;
                        }
                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error Saving DataObject :" + ex.Message, ex);
                return -1;
            }
        }

        public int InsertObject(DBConnectionToken pToken, DataObject dataItem, string database, String[] pKeyFields)
        {
            try
            {
                using (DbConnection con = new MySqlConnection(CreateMasterConnectionString(pToken)))
                {
                    con.Open();
                    using (DbCommand cmd = con.CreateCommand())
                    {
                        GenerateParameters(cmd.Parameters, dataItem);
                        cmd.CommandText = CreateInsertCommandText(dataItem, pKeyFields);
                        cmd.CommandText = cmd.CommandText.Replace(dataItem.GetType().Name, database + "." + dataItem.GetType().Name);
                        if (ExecuteNonQueryCommand(cmd) == 1)
                        {
                            dataItem.GetType().GetProperties().Where(pi => pi.GetCustomAttributes(true).Where(ca => ca is System.ComponentModel.DataObjectFieldAttribute && (ca as System.ComponentModel.DataObjectFieldAttribute).IsIdentity).Count() > 0);

                            foreach (PropertyInfo propertyInfo in dataItem.GetType().GetProperties())
                            {
                                if (propertyInfo.GetCustomAttributes(true).Where(ca => ca is System.ComponentModel.DataObjectFieldAttribute && (ca as System.ComponentModel.DataObjectFieldAttribute).IsIdentity).Count() > 0)
                                {

                                    if (propertyInfo.PropertyType == typeof(Int32))
                                    {
                                        int pk = GetLastRowId(con);
                                        propertyInfo.SetValue(dataItem, pk, null);
                                        setFilledByDb(dataItem, true);
                                        return pk;
                                    }
                                    else if (propertyInfo.PropertyType == typeof(UInt32))
                                    {
                                        uint pk = GetUIntLastRowId(con);
                                        propertyInfo.SetValue(dataItem, pk, null);
                                        setFilledByDb(dataItem, true);
                                        if (pk < Int32.MaxValue)
                                            return Convert.ToInt32(pk);
                                        else
                                            return 0;
                                    }
                                }
                            }
                            setFilledByDb(dataItem, true);
                            return 0;
                        }
                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Error Saving DataObject :" + ex.Message, ex);
                return -1;
            }
        }

        public int UpdateObject(DBConnectionToken pToken, DataObject dataItem, string[] primaryKey)
        {
            try
            {
                using (DbConnection con = new MySqlConnection(CreateMasterConnectionString(pToken)))
                {
                    con.Open();
                    using (DbCommand cmd = con.CreateCommand())
                    {
                        GenerateParameters(cmd.Parameters, dataItem);
                        cmd.CommandText = CreateUpdateALLCommandText(dataItem, primaryKey);
                        if (ExecuteNonQueryCommand(cmd) == 1)
                        {
                            if (primaryKey.Count() == 1)
                            {
                                object keyVal = dataItem.GetType().GetProperty(primaryKey.First()).GetValue(dataItem, null);
                                return int.Parse(keyVal.ToString());
                            }
                            else return 0;
                        }
                    }
                }

                return -1;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(ex.Message, ex);
                return -1;
            }
        }

        public int GetSingleInt(DBConnectionToken pToken, string qry)
        {
            int result = 0;
            object value = GetSingleValue(pToken, qry);
            if (value != DBNull.Value)
                result = Convert.ToInt32(value);
            return result;
        }

        public object GetSingleValue(DBConnectionToken pToken, string qry)
        {
            using (DbConnection con = new MySqlConnection(CreateMasterConnectionString(pToken)))
            {
                con.Open();
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = qry;
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.GetValue(0) != DBNull.Value)
                            return reader.GetValue(0);
                        else return null;
                    }
                }
            }
        }
        
        public string getDBHost()
        {
            return DBHost();
        }

        public static string DBHost()
        {
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null
             && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString != string.Empty)
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString))
                {
                    return conn.DataSource;
                }
            }
            else
            { return string.Empty; }
        }
        
        public static void ClearAllPools()
        {
            unLogger.WriteDebug("ClearAllPools");
            MySqlConnection.ClearAllPools();
        }
        
        private string changeIP(string pConnectionString, string pNewIP)
        {
            string pattern = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}";

            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern);
            if (rgx.Match(pNewIP).Success)
            {
                string ret = rgx.Replace(pConnectionString, pNewIP);
                return ret;
            }
            return pConnectionString;

        }
    }
}
