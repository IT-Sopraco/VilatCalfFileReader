using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Threading;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQSoaplog
    {
        private DBSelectQueries Parent;

        public DBSQSoaplog(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

    }
}
