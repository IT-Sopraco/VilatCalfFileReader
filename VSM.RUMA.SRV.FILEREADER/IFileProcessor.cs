using System;
namespace VSM.RUMA.SRV.FILEREADER
{
    public interface IFileProcessor
    {
        void OnContinue();
        void OnPause();
        void OnStart();
        void OnStop();
    }
}
