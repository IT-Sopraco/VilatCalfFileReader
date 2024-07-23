using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.DataTypes
{
    [Serializable()]
    public class ADMINISTRATIEBEDRIJF :BEDRIJF
    {
        public int AdmisID { get; set; }
        public int ThrID { get; set; }
    }
}
