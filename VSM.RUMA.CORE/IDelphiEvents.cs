using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VSM.RUMA.CORE
{
    [Guid("0FAF58ED-A5A9-49b3-B64A-B2D6D8EA7391")]
    public interface IDelphiEvents
    {
        void Application_Update(int progress, string message);
    }
}
