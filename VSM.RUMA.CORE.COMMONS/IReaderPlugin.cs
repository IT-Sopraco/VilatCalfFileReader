using System;
using System.Collections.Generic;
namespace VSM.RUMA.CORE
{
    public interface IReaderPlugin
    {
        String GetFilter();
        void setSaveToDB(AFSavetoDB value);
        List<String> getExcludeList();
        int LeesFile(int thrId, DBConnectionToken agroFactuurToken, int programId, String agrobaseUser,
                                    String agrobasePassword, int fileLogId, String Bestandsnaam); 
    }
}
