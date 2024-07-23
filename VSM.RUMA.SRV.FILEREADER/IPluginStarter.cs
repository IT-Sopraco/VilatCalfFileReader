using System;
namespace VSM.RUMA.CORE
{
    public interface IPluginStarter
    {
        bool Execute(ref Facade pFacade);
        //void setIenR(ref Facade pFacade);      
    }
}
