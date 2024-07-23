using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data.Common;
using System.Data;

namespace VSM.RUMA.CORE.DB
{
    public class DBCommons
    {

        protected IDatabase mDatabase;
        protected DBConnectionToken mToken;
        public virtual IDatabase GetDataBase()
        {
            return mDatabase;
        }


        [Obsolete]
        protected virtual DBConnectionToken FetchRightsToken()
        {
            lock (mDatabase)
            {
                return mToken;
            }
        }

        [Obsolete("Gebruik DbCommand niet ivm disposables",true)]
        public DbCommand CreateCommand()
        {
            return  mDatabase.CreateCommand(mToken.getLastChildConnection());
        }



        //[Obsolete("Gebruik DbCommand niet ivm disposables", true)]
        //public DbCommand CreateCommandFactuur()
        //{
        //    return mDatabase.CreateCommand(mToken);
        //}

        //[Obsolete("Gebruik DbCommand niet ivm disposables", true)]
        //public DataTable QueryData(DbCommand pCommand)
        //{
        //    return QueryData(pCommand, MissingSchemaAction.AddWithKey);
        //}

        //[Obsolete("Gebruik DbCommand niet ivm disposables", true)]
        //public DataTable QueryData(DbCommand pCommand, MissingSchemaAction MissingSchemaAction)
        //{
        //    return mDatabase.QueryData(mToken.getLastChildConnection(), null, pCommand, string.Empty, MissingSchemaAction);
        //}


        public DataTable QueryData(String Query)
        {
            return QueryData(Query, MissingSchemaAction.AddWithKey);
        }

        public DataTable QueryData(String Query, MissingSchemaAction MissingSchemaAction)
        {
            return mDatabase.QueryData(mToken.getLastChildConnection(), new StringBuilder(Query), MissingSchemaAction);
        }


        [Obsolete("Gebruik normaal querydata met prefix agrofactuur.")]
        public DataTable QueryDataFactuur(String Query)
        {
            return QueryDataFactuur(Query, MissingSchemaAction.AddWithKey);
        }
        [Obsolete("Gebruik normaal querydata met prefix agrofactuur.")]
        public DataTable QueryDataFactuur(String Query, MissingSchemaAction MissingSchemaAction)
        {
            return mDatabase.QueryData(mToken, new StringBuilder(Query), MissingSchemaAction);
        }

        
        public bool isFilledByDb(DataObject pData)
        {
            return pData.FilledByDb;
        }

        protected string getDbPrefix(int ProgId)
        {
            //koe?

            string db = "";
            if (ProgId == 5)
            {
                db = "agrobase_goat.";
            }
            else if (ProgId == 3)
            {
                db = "agrobase_sheep.";
            }
            else if (ProgId == 25)
            {
                db = "agrobase_dog.";
            }
            else
            {
                throw new NotImplementedException("onbekend progid");
            }

            return db;
        }


        public T getSingleItem<T>(String qry) where T : DataObject, new()
        {
            return getSingleItem<T>(mToken.getLastChildConnection(), new StringBuilder(qry));
        }

        [Obsolete("Gebruik getSingleItem zonder de token parameter.")]
        public T getSingleItem<T>(DBConnectionToken token, String qry) where T : DataObject, new()
        {
            return getSingleItem<T>(token, new StringBuilder(qry));
        }

        [Obsolete("Gebruik getSingleItem zonder de token parameter.")]
        public T getSingleItem<T>(DBConnectionToken token, StringBuilder qry) where T : DataObject, new()
        {
            T item = new T();
            bool hasresults = mDatabase.FillObject(token, item, qry);
            return hasresults ? item : null;
        }

        [Obsolete("Gebruik van DbCommand is obsolete")]
        public T getSingleItem<T>(DbCommand cmd) where T : DataObject, new()
        {
            T item = new T();
            bool hasresults = mDatabase.FillObject(FetchRightsToken(), item, cmd);
            return hasresults ? item : null;
        }




        public List<T> getList<T>(DataTable dt) where T : DataObject, new()
        {
            List<T> lResultList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T l = new T();
                if (mDatabase.FillObject(l, dr))
                {
                    lResultList.Add(l);
                }
            }
            return lResultList;
        }

        public bool FillObject(DataObject pData, DataRow drData)
        {
            return mDatabase.FillObject(pData, drData);
        }



        public string GetSearchNumbers(string pAniUniqueLifeNumber)
        {
            StringBuilder searcharry = new StringBuilder();
            searcharry.Append("'" + pAniUniqueLifeNumber + "',");
            char[] spl = { ' ' };
            string[] lNumber = pAniUniqueLifeNumber.Split(spl);
            if (lNumber.Length == 2)
            {
                while (lNumber[1].StartsWith("0"))
                {
                    if (lNumber[1].Length > 2)
                    {
                        lNumber[1] = lNumber[1].Remove(0, 1);
                        searcharry.Append("'" + lNumber[0] + " " + lNumber[1] + "',");
                    }
                    else { break; }
                }
                while (lNumber[1].Length < 15)
                {
                    lNumber[1] = "0" + lNumber[1];
                    searcharry.Append("'" + lNumber[0] + " " + lNumber[1] + "',");
                }

            }
            return searcharry.ToString().Remove(searcharry.ToString().Length - 1, 1);
        }

        public DateTime? StringToNDT(string dt)
        {
            DateTime ret; ;

            if (DateTime.TryParse(dt, out ret))
                return ret;

            return null;
        }


        public string EnumerableToCommaSeperatedString<T>(IEnumerable<T> intList)
        {
            String str = "";
            String sKomma = "";

            foreach (var obj in intList)
            {
                str += sKomma + obj.ToString();
                sKomma = ",";
            }
            return str;
        }

        //TODO aanpassen naar/ vervangen door meer algemene functie??
        public IEnumerable<T> DataTableColumnToList<T>(DataTable dt, string columnName) 
        {
            List<T> lResultList = new List<T>();

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lResultList.Add((T)Convert.ChangeType(dr[columnName],typeof(T)));
                }
            }

            return lResultList;
        }
    }
}
