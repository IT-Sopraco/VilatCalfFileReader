using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE;

namespace VSM.RUMA.SRV.FILEREADER
{
    public class unServerConnectionToken : DBConnectionToken
    {
        internal unServerConnectionToken(String pDB, String pIP,
                             String pUser, String pPass)
            : base(pDB, pIP, pUser, pPass, pDB, pIP, pUser, pPass)
        {
        }
    }
}
