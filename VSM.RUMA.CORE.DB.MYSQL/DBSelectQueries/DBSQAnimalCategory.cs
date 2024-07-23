using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQAnimalCategory
    {
        private DBSelectQueries Parent;

        public DBSQAnimalCategory(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

    }
}
