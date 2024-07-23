using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQOptimaBox
    {
        private DBSelectQueries Parent;

        public DBSQOptimaBox(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        public DataTable GetBoxFeedAdvices()
        {
            StringBuilder query = new StringBuilder($@"SELECT DISTINCT  
             UBN.Bedrijfsnummer, ANIMAL.anialternatenumber, TRANSMIT.TransmitterNumber, FEED_ADVICE.Fa_DateTime, FEED_ADVICE_DETAIL.*  
             FROM agrofactuur.BEDRIJF  
             INNER JOIN agrofactuur.UBN ON UBN.ubnid=BEDRIJF.ubnid  
             INNER JOIN agrobase.ANIMALCATEGORY ON ANIMALCATEGORY.farmid=BEDRIJF.farmid AND ANIMALCATEGORY.Anicategory<4  
             INNER JOIN agrobase.ANIMAL ON ANIMAL.AniID=ANIMALCATEGORY.AniID  
             INNER JOIN agrobase.FEED_ADVICE ON FEED_ADVICE.AniID=ANIMAL.AniID AND FEED_ADVICE.FA_DateTime=CURRENT_DATE  
             INNER JOIN agrobase.FEED_ADVICE_DETAIL ON FEED_ADVICE_DETAIL.FA_ID=FEED_ADVICE.FA_ID  
          
             LEFT OUTER JOIN agrobase.TRANSMIT  
             ON TRANSMIT.farmid=BEDRIJF.farmid  
             AND TRANSMIT.AniId=ANIMAL.aniid  
             AND TRANSMIT.ProcesComputerId=230101  
             WHERE BEDRIJF.programid=16000  
             AND BEDRIJF.farmid>0  ");
            DataTable dt = Parent.QueryData(query.ToString());
            /*
               #TODO:add / uncomment once this is used
            # INNER JOIN agrofactuur.FARMCONFIG ON FARMCONFIG.farmid=BEDRIJF.farmid AND FARMCONFIG.fkey='MSOPTIMA_v2' AND FARMCONFIG.FValue='True'  
           
             */
            return dt;
        }
    }
}
