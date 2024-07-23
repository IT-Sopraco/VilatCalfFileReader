using System;
namespace VSM.RUMA.CORE
{
    public interface IPluginStarter
    {
        bool getInlog(ref String pUsername, ref String pPassword);
        bool Execute(ref Facade pFacade, DBConnectionToken pToken);
        //void setIenR(ref Facade pFacade);      
    }
}
