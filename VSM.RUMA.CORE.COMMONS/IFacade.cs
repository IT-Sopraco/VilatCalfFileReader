using System;
namespace VSM.RUMA.CORE
{
    public delegate void DomainEventHandler(IFacade sender, DBConnectionToken pToken, int FarmId);

    public interface IFacade
    {
        event DomainEventHandler Application_Loading;
        ISendReceive getSendReceive();
        AFSavetoDB getSaveToDB(DBConnectionToken pToken);
        //[Obsolete("GetSaveToDB voortaan aanroepen met DB Token")]
        //AFSavetoDB getSaveToDB();
        void LoadApplication(DBConnectionToken pToken, int FarmId);
        void setDatacomPath(string Path);
        void setSaveToDB(AFSavetoDB value, DBConnectionToken pToken);
        void UpdateProgress(int progress, string message);
        string Version();
    }
}
