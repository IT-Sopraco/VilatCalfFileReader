using System;
namespace VSM.RUMA.CORE.DB
{
    public interface IunLogger
    {
        void baseWriteError(String Message, Exception ex);
        void baseWriteError(String Message);
        void baseWriteWarn(String Message);
        void baseWriteInfo(String Message);
        void baseWriteDebug(String Message);
    }
}
