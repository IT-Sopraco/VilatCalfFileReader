using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.DataTypes
{
    [Serializable()]
    public class MUTATION__ADMIN : MUTATION
    {

        public class ColumnNamesMUTATION
        {
            public const string FarmNameFrom = "FarmNameFrom";
            public const string FarmNameTo = "FarmNameTo";
        }


        public string FarmNameFrom
        {
            get
            {
                return base.Getstring(ColumnNamesMUTATION.FarmNameFrom);
            }
            set
            {
                base.Setstring(ColumnNamesMUTATION.FarmNameFrom, value);
            }
        }

        public string FarmNameTo
        {
            get
            {
                return base.Getstring(ColumnNamesMUTATION.FarmNameTo);
            }
            set
            {
                base.Setstring(ColumnNamesMUTATION.FarmNameTo, value);
            }
        }
    }
}
