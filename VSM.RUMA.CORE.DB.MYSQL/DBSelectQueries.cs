using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data.Common;
using System.Data;
using VSM.RUMA.CORE.DB.MYSQL.DBSQ;

namespace VSM.RUMA.CORE.DB.MYSQL
{
    public partial class DBSelectQueries : DBCommons
    {
        internal DBSelectQueries(DBConnectionToken pDbconntoken, IDatabase pDatabase)
        {
            mDatabase = pDatabase;
            mToken = pDbconntoken;
        }

        public DBSelectQueries(DBConnectionToken pDbconntoken)
        {
            mDatabase = new unMySQL(unMySQL.DBHostType.SLAVE);
            mToken = pDbconntoken;
        }

        private DBSQDeenssysteem _Deenssysteem;
        public DBSQDeenssysteem Deenssysteem
        {
            get
            {
                if(_Deenssysteem == null)
                    _Deenssysteem = new DBSQDeenssysteem(this);

                return _Deenssysteem;
            }
            
        }

        private DBSQMPR _MPR;
        public DBSQMPR MPR
        {
            get
            {
                if (_MPR == null)
                    _MPR = new DBSQMPR(this);

                return _MPR;
            }
        }

        private DBSQBedrijf _Bedrijf;
        public DBSQBedrijf Bedrijf
        {
            get
            {
                if (_Bedrijf == null)
                    _Bedrijf = new DBSQBedrijf(this);

                return _Bedrijf;
            }
        }

        private DBSQAnimal _Animal;
        public DBSQAnimal Animal
        {
            get
            {
                if (_Animal == null)
                    _Animal = new DBSQAnimal(this);

                return _Animal;
            }
        }

        private DBSQAnimalCategory _AnimalCategory;
        public DBSQAnimalCategory AnimalCategory
        {
            get
            {
                if (_AnimalCategory == null)
                    _AnimalCategory = new DBSQAnimalCategory(this);

                return _AnimalCategory;
            }
        }
        
        private DBSQEvent _Event;
        public DBSQEvent Event
        {
            get
            {
                if (_Event == null)
                    _Event = new DBSQEvent(this);

                return _Event;
            }
        }

        private DBSQFeeds _Feeds;
        public DBSQFeeds Feeds
        {
            get
            {
                if (_Feeds == null)
                    _Feeds = new DBSQFeeds(this);

                return _Feeds;
            }

        }

        private DBSQResponders _Responders;
        public DBSQResponders Responders
        {
            get
            {
                if (_Responders == null)
                    _Responders = new DBSQResponders(this);

                return _Responders;
            }

        }

        private DBSQConfigs _Configs;
        public DBSQConfigs Configs
        {
            get
            {
                if (_Configs == null)
                    _Configs = new DBSQConfigs(this);

                return _Configs;
            }

        }

        private DBSQTempTable _TempTable;
        public DBSQTempTable TempTable
        {
            get
            {
                if (_TempTable == null)
                    _TempTable = new DBSQTempTable(this,mToken);

                return _TempTable;
            }

        }

        private DBSQRDBTools _RDBTools;
        public DBSQRDBTools RDBTools
        {
            get
            {
                if (_RDBTools == null)
                    _RDBTools = new DBSQRDBTools(this);

                return _RDBTools;
            }

        }

        private DBSQOptimaBox _OptimaBox;
        public DBSQOptimaBox OptimaBox
        {
            get
            {
                if (_OptimaBox == null)
                {
                    _OptimaBox = new DBSQOptimaBox(this);
                }
                return _OptimaBox;
            }
        }

        private DBSQLORAWeideRapport _loraWeideRapport;
        public DBSQLORAWeideRapport LORAWeideRapport
        {
            get
            {
                if (_loraWeideRapport == null)
                {
                    _loraWeideRapport = new DBSQLORAWeideRapport(this);
                }
                return _loraWeideRapport;
            }
        }

        private DBSQSoaplog _Soaplog;
        public DBSQSoaplog Soaplog
        {
            get
            {
                if (_Soaplog == null)
                {
                    _Soaplog = new DBSQSoaplog(this);
                }
                return _Soaplog;
            }
        }

        private DBSQVerified _Verified;
        public DBSQVerified Verified
        {
            get
            {
                if (_Verified == null)
                {
                    _Verified = new DBSQVerified(this);
                }
                return _Verified;
            }
        }

        public DbCommand GetDBCommand()
        { 
            return mDatabase.CreateCommand(this.mToken);
        }

        public List<int> getListProgramIdOndergeschikten(int pProgramId)
        {
            List<int> listProgramId = new List<int>();

            switch (pProgramId)
            {
                case 22: //nsfo  
                    //listProgramId.Add(24);
                    //listProgramId.Add(25);
                    //listProgramId.Add(26);
                    //listProgramId.Add(27);
                    //listProgramId.Add(28);
                    //listProgramId.Add(29);
                    //listProgramId.Add(30);
                    //listProgramId.Add(31);
                    //listProgramId.Add(32);
                    //listProgramId.Add(33);
                    //listProgramId.Add(34);
                    listProgramId = utils.getNsfoProgramIds();
                    listProgramId.Add(47);
                    listProgramId.Add(49);
                    break;
                case 90: //forfarmers
                    listProgramId.Add(9);
                    break;
                case 100: //elda
                    listProgramId.Add(7);
                    break;
                case 160: //beltes
                    listProgramId.Add(16);
                    break;
                case 230: //pali
                    listProgramId.Add(23);
                    break;
            }

            return listProgramId;
        }

        protected internal int getUbnIdOndergeschiktenType(int pProgramId)
        {
            //type 1 = verkrijg bedrijven ondergeschikten op basis van programId
            //type 2 = verkrijg bedrijven ondergeschikten vanuit soap

            if ((pProgramId == 22) || //nsfo
                (pProgramId == 90) || //forfarmers
                //(pProgramId == 100) || //elda
                (pProgramId == 160) || //beltes
                (pProgramId == 230)) //pali
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        internal IEnumerable<AniIdDate> dtEventToAniIdDate(DataTable dt)
        {
            List<AniIdDate> ret = new List<AniIdDate>();
            foreach (DataRow dr in dt.Rows)
            {
                var md = dr.Field<MySql.Data.Types.MySqlDateTime?>("EveDate");
                if (md == null)
                    continue;

                var date = new DateTime(md.Value.Year, md.Value.Month, md.Value.Day);
                ret.Add(new AniIdDate(dr.Field<int>("AniId"), date));
            }

            //distinct ivm dubbele events in agrobase
            return ret.Distinct(new AniIdDateComparer());
        }

        public string MySQL_Datum(DateTime datum, int iFormat)
        {
            string sFormat = "";
            if (datum == DateTime.MinValue)
            {
                return "''";
            }
            if (iFormat == 0)
            {
                sFormat = "yyyy-MM-dd HH:mm:ss";
            }
            else if (iFormat == 1)
            {
                sFormat = "yyyy-MM-dd";
            }

            string sDt = "'" + datum.ToString(sFormat) + "'";
            return sDt;
        }
    }
}
