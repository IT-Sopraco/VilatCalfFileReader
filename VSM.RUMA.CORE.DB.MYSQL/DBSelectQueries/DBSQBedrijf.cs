using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQBedrijf
    {
        private DBSelectQueries Parent;

        public DBSQBedrijf(DBSelectQueries parent)
        {
            this.Parent = parent;
        }


        public List<BEDRIJF> getBedrijvenByUBNId(int pUBNId)
        {
            var lResultList = new List<BEDRIJF>();
            if (pUBNId > 0)
            {
                DataTable tbl = Parent.QueryDataFactuur(String.Format(" SELECT *  FROM agrofactuur.BEDRIJF WHERE UBNid = {0} AND agrofactuur.BEDRIJF.FarmId>0", pUBNId));

                foreach (DataRow dr in tbl.Rows)
                {
                    var l = new BEDRIJF();
                    if (Parent.FillObject(l, dr))
                    {
                        lResultList.Add(l);
                    }
                }
            }
            return lResultList;
        }

    }
}
