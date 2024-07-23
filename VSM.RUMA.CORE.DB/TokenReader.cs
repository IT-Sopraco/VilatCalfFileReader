using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB
{
    public abstract class TokenReader
    {
        //public TokenReader(IDatabase DB)
        //{
        //}

        protected String ReadToken(DBConnectionToken pToken, String PropertyName)
        {

            switch (PropertyName)
            {
                case "MasterDB":
                    return pToken.MasterDB;
                case "MasterIP":
                    return pToken.MasterIP;
                case "MasterUser":
                    return pToken.MasterUser;
                case "MasterPass":
                    return pToken.MasterPass;
                case "SlaveDB":
                    return pToken.SlaveDB;
                case "SlaveIP":
                    return pToken.SlaveIP;
                case "SlaveUser":
                    return pToken.SlaveUser;
                case "SlavePass":
                    return pToken.SlavePass;
                default: throw new NotSupportedException(PropertyName + " does not exist in Token.");
            }



        }
        protected void setFilledByDb(VSM.RUMA.CORE.DB.DataTypes.DataObject pData, Boolean pValue)
        {
            pData.FilledByDb = pValue;
        }

        //public abstract string getDBHost();
    }
}
