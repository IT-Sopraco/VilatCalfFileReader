using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.COMMONS
{
    public interface IDirectoryReaderPlugin :IReaderPlugin
    {
        String GetDirectory();
    }
}
