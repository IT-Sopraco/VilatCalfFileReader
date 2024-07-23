using System;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL
{
    public class MysqlDateTimeConverter
    {

        public static MySql.Data.Types.MySqlDateTime? ConvertToMysqlDateTime(DateTime? InputDT)
        {

            if (!InputDT.HasValue)
                return null;

                MySql.Data.Types.MySqlDateTime datum = new MySql.Data.Types.MySqlDateTime(InputDT.Value);
                return datum;
        }

        public static bool IsMySqlDateTime(Object VariableToCheck)
        {
            return VariableToCheck is MySql.Data.Types.MySqlDateTime;

        }
        public static DateTime? ConvertObjToDateTime(Object MysqlDT)
        {
            if (IsMySqlDateTime(MysqlDT))
            {
                DateTime DT = ((MySql.Data.Types.MySqlDateTime)MysqlDT).Value;
                return DT;
            }
            return null;
        }


        public static DateTime? ConvertToDateTime(MySql.Data.Types.MySqlDateTime MysqlDT)
        {
            try
            {
                return MysqlDT.Value;
            }
            catch
            {
                return null;
            }
        }


        public static void SetMysqlDateTimeProperty(DataRow pDR, String ColumnName, DateTime? DT)
        {
            if (!DT.HasValue)
                pDR[ColumnName] = DBNull.Value;
            else
            {
                pDR[ColumnName] = MysqlDateTimeConverter.ConvertToMysqlDateTime(DT.Value);
            }

        }

        public static void SetMysqlDateTimeProperty(DataRow pDR, String ColumnName, int year, int month, int day, int hour, int minute, int second, int microsecond)
        {


            pDR[ColumnName] = new MySql.Data.Types.MySqlDateTime(year, month, day, hour, minute, second, microsecond);

        }


    }
}
