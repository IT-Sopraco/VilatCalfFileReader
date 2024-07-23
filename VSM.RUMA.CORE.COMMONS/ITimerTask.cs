using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.COMMONS
{
    public interface ITimerTask
    {
        TimeSpan GetInterval();
        TimeSpan GetDelay();
        bool DoTask(DBConnectionToken pAgroFactuurToken);
        void setFacade(IFacade value);

    }
}
