using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class DelphiWrapper
    {

        public static bool controlelevensnr(int prognr, string landcode, string levnr, bool elecID)
        {
            return contrlnr.Units.contrlnr.controlelevensnr(prognr, landcode, levnr, elecID);
        }

        public static bool controlelevnrstier(int prognr, string landcode, string levnr, bool elecID)
        {
            return contrlnr.Units.contrlnr.controlelevnrstier(prognr, landcode, levnr, elecID);
        }

        public static void levensnummerScanner(bool oornr, string lnumscan, string deflandcode, ref string landcode, ref string levnr)
        {
            unDelphiProcedures.TunDelphiProcedures Delphi = new unDelphiProcedures.TunDelphiProcedures();
            Delphi.levensnummerScanner(oornr, lnumscan, deflandcode, ref landcode, ref levnr);
        }

        public static string ISOlandcode(string landcode)
        {
            unDelphiProcedures.TunDelphiProcedures Delphi = new unDelphiProcedures.TunDelphiProcedures();
            return Delphi.ISOlandcode(landcode);
        }

        public static string landcodeISO(string isocode)
        {
            unDelphiProcedures.TunDelphiProcedures Delphi = new unDelphiProcedures.TunDelphiProcedures();
            return Delphi.landcodeISO(isocode);
        }

        public static string fDiernr(int prognr, string landcode, string levnr, bool elecid)
        {

            return contrlnr.Units.contrlnr.fDiernr(prognr, landcode, levnr, elecid);
        }
    }
}
