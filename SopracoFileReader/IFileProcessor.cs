using System;

namespace SopracoFileReader
{

    public interface IFileProcessor
    {
        void OnContinue();
        void OnPause();
        void OnStart();
        void OnStop();
    }
}
