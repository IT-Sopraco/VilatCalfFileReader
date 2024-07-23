using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.CORE;

namespace SopracoFileReader
{
    internal sealed class unServiceRechten : unRechten
    {

        internal UserRightsToken GetToken()
        {
            //string pGebruiker = "vilatcalf_write";
            //string pPassword = "Ay9OwPhbYqRm";
            //string pMasterDB = "agrofactuur";
            //string pMasterIP = "192.168.150.3";
            //string pMasterUser = "vilatcalf_write";
            //string pMasterPass = "Ay9OwPhbYqRm";
            //string pSlaveDB = "agrofactuur";
            //string pSlaveIP = "192.168.150.3";
            //string pSlaveUser = "vilatcalf_write";
            //string pSlavePass = "Ay9OwPhbYqRm";
            //Facade pFacade = Facade.GetInstance();

            //return new UserRightsToken(ref pFacade, pGebruiker, base64Decode(pPassword), pMasterDB, pMasterIP, pMasterUser, pMasterPass, pSlaveDB, pSlaveIP, pSlaveUser, pSlavePass);

            UserRightsToken ServiceAccount = base.getServiceAccount();
            return ServiceAccount;
        }


        internal void VeranderAdministratie(UserRightsToken Token, int programid)
        {
            base.VeranderAdministratieService(ref Token, programid);
        }
    }
}
