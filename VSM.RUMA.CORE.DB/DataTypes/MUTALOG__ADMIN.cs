using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.DataTypes
{
    [Serializable()]
    public class MUTALOG__ADMIN : MUTALOG
    {

        public class ColumnNamesMUTALOG
        {
            public const string FarmNameFrom = "FarmNameFrom";
            public const string FarmNameTo = "FarmNameTo";
        }


        public string FarmNameFrom
        {
            get
            {
                return base.Getstring(ColumnNamesMUTALOG.FarmNameFrom);
            }
            set
            {
                base.Setstring(ColumnNamesMUTALOG.FarmNameFrom, value);
            }
        }

        public string FarmNameTo
        {
            get
            {
                return base.Getstring(ColumnNamesMUTALOG.FarmNameTo);
            }
            set
            {
                base.Setstring(ColumnNamesMUTALOG.FarmNameTo, value);
            }
        }
    }
}
