using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB.MYSQL
{
    [Obsolete("gebruik DBMasterQueries")]
    public class MySQLSavetoDB : DBMasterQueries
    {
        private bool oldconstructor = false;
        [Obsolete("gebruik DBMasterQueries",true)] 
        public MySQLSavetoDB(DBConnectionToken pDbconntoken) : base(pDbconntoken)
        {
            if (pDbconntoken == null)
                unLogger.WriteError("Nieuw db instantie : Token = null!!!!");
            oldconstructor = false;
        }


        [Obsolete("gebruik DBMasterQueries",true)]
        public MySQLSavetoDB()
            : base((DBConnectionToken)System.Web.HttpContext.Current.Session["AgroRightsToken"])
        {
            unLogger.WriteDebug("Oude db instantie");
            //if (DBToken == null)
            try
            {
                mToken = (DBConnectionToken)System.Web.HttpContext.Current.Session["AgroRightsToken"];
            }
            catch (Exception ex)
            {
                VSM.RUMA.CORE.unLogger.WriteError("MySQLSavetoDB oude constructor", ex);
                //quick fix

            }
            oldconstructor = true;
        }


        [Obsolete("geef token mee aan constructor",true)]
        public new void setToken(DBConnectionToken pDbconntoken)
        {
            lock (mDatabase)
            {
                mToken = pDbconntoken;
            }
        }
    }
}