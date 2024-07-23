using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.CORE;

namespace VSM.RUMA.SRV.FILEREADER.CORE
{
    internal sealed class unServiceRechten : unRechten
    {

        internal UserRightsToken GetToken()
        {
            UserRightsToken ServiceAccount = base.getServiceAccount();
            return ServiceAccount;
        }


        internal void VeranderAdministratie(UserRightsToken Token, int programid)
        {
            base.VeranderAdministratieService(ref Token, programid);
        }
    }
}
