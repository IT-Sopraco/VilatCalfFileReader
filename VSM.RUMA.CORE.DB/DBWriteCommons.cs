using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB
{
    public class DBWriteActions : DBCommons
    {

        public DBWriteActions(DBConnectionToken pDbconntoken, IDatabase pDatabase)
        {
            mDatabase = pDatabase;
            mToken = pDbconntoken;
        }

        //protected String ReadToken(DBConnectionToken pToken, String PropertyName)
        //{

        //    switch (PropertyName)
        //    {
        //        case "MasterDB":
        //            return pToken.MasterDB;
        //        case "MasterIP":
        //            return pToken.MasterIP;
        //        case "MasterUser":
        //            return pToken.MasterUser;
        //        case "MasterPass":
        //            return pToken.MasterPass;
        //        case "SlaveDB":
        //            return pToken.SlaveDB;
        //        case "SlaveIP":
        //            return pToken.SlaveIP;
        //        case "SlaveUser":
        //            return pToken.SlaveUser;
        //        case "SlavePass":
        //            return pToken.SlavePass;
        //        default: throw new NotSupportedException(PropertyName + " does not exist in Token.");
        //    }



        //}

        public int SaveObject(DataObject dataItem)
        {
            DBConnectionToken ActiveToken;
            if (dataItem.getDB != Database.agrofactuur)
            {
                if (mToken.HasAnimalDatabase())
                    ActiveToken = mToken.getLastChildConnection();
                else return -1;
            }
            else
            {
                ActiveToken = mToken;
            }
            if (dataItem.FilledByDb)
            {
                return mDatabase.UpdateObject(ActiveToken, dataItem, dataItem.UpdateParams);
            }
            else
            {
                return mDatabase.InsertObject(ActiveToken, dataItem);
            }

        }


    }
}
