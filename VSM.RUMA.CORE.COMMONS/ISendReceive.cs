using System;
namespace VSM.RUMA.CORE
{
    public interface ISendReceive
    {
        void VerzendenOntvangen(int pThrId, DBConnectionToken pToken,VSM.RUMA.CORE.DB.DataTypes.BEDRIJF Farm, int changedby, int sourceid);
        void EDINRSOphalenEnVerwerken(int pThrId, DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.BEDRIJF Farm);
        void FTPVerzendenOfOntvangen(VSM.RUMA.CORE.DB.DataTypes.FTPINFO lFtpInfo, DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.BEDRIJF Farm);
        void FTPVerzendenOfOntvangen(VSM.RUMA.CORE.DB.DataTypes.FTPINFO lFtpInfo, DBConnectionToken pToken, VSM.RUMA.CORE.DB.DataTypes.BEDRIJF Farm, out int iOk, out int iError, out int iNoData);
    }
}
