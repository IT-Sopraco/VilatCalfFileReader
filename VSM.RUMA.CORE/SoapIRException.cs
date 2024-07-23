using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE
{
    public class SoapIRException :Exception
    {
        private List<IRreportresult> mResultList;
        private SoapType mType;
        public enum SoapType { DHZKI, IR, EMM};

        public SoapIRException(List<IRreportresult> ResultList, SoapType pType)
            : base()
        {
            mResultList = ResultList;
            mType = pType;
        }

        public SoapIRException(string message, List<IRreportresult> ResultList, SoapType pType)
            : base(message)
        {
            mResultList = ResultList;
            mType = pType;
        }

        public List<IRreportresult> GetResultData
        {
            get
            {
                return mResultList;
            }
        }

        public SoapType GetSoapType
        {
            get
            {
                return mType;
            }
        }



    }
}
