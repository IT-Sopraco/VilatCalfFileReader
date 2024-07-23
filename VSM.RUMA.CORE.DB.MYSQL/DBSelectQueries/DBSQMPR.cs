using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQMPR
    {
        private DBSelectQueries Parent;

        public DBSQMPR(DBSelectQueries parent)
        {
            this.Parent = parent;
        }
    }
}
