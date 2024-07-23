using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQEvent
    {
        private DBSelectQueries Parent;

        public DBSQEvent(DBSelectQueries parent)
        {
            this.Parent = parent;
        }


        public bool inheatExists(int AniId, DateTime date, VSM.RUMA.CORE.DB.LABELSConst.HeatCertainty heatCertainty)
        {
            String qry = String.Format(
                                  @"SELECT e.* FROM EVENT e 
                                    JOIN INHEAT i ON e.EventId = i.EventId 
                                    WHERE e.AniId = {0} 
                                    AND e.EveKind = {1} 
                                    AND e.EventId > 0 
                                    AND Date(e.EveDate) = Date ({2})
                                    AND i.heatCertainty = {3} "
                                    , AniId, 
                                    (int)LABELSConst.EventKind.TOCHTIG, 
                                    Parent.MySQL_Datum(date, 1), 
                                    (int)heatCertainty
                                );

            DataTable dt = Parent.QueryData(qry);
            return Parent.getList<EVENT>(dt).Count > 0;
        }

    }
}
