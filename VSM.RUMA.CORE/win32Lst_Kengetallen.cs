using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Kengetallen: Win32
    {
        [ThreadStatic]
        private static Win32Kengetallen singleton;

        public static Win32Kengetallen Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Kengetallen();
            }
            return singleton;
        }

        public int call_lst_Kengetallen(int pJaar)
        {
            lock (typeof(Win32Kengetallen))
            {
                string sFilename = "Kengetallen.dll";

                lst_Kengetallen handle = (lst_Kengetallen)ExecuteProcedureDLL(
                        typeof(lst_Kengetallen), sFilename, "lst_Kengetallen");

                int tmp = handle(pJaar);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_Kengetallen(int pJaar);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
