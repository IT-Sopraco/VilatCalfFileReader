using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text;

namespace VSM.RUMA.CORE
{
    class Win32SOAPVBHALG : Win32
    {
        public Win32SOAPVBHALG()
            : base(false)
        {
        }
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDDHZKImelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrKoe, DateTime Datum, DateTime Tijd,
                                String KiCodeStier, String LevensnrStier, String Chargenr, String RelatienrUitvoerende,
                                StringBuilder pLogfile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDNatuurlijkeDekkingMelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrKoe, DateTime Datum, DateTime Tijd,
                                String LevensnrStier, String RelatienrWaarnemer,
                                String pLogfile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDSamenWeidingMelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrKoe, DateTime Datum, DateTime Tijd, DateTime DatumEind,
                                String LevensnrStier, String RelatienrWaarnemer,
                                String pLogfile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);




        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDTochtMelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime DatumTocht, DateTime TijdTocht,
                                String RelatienrWaarnemer,
                                String pLogfile,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving,
                                int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDDrachtControleMelding(String pUsername, String pPassword, Boolean pTestServer,
                                    String Land, String UBNnr, String Levensnr, DateTime DatumDracht, DateTime TijdDracht,
                                    int StatusDracht, int MethodeControle,
                                    String RelatienrWaarnemer,
                                    String pLogfile,
                                    ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDVerwerpenMelding(String pUsername, String pPassword, Boolean pTestServer,
                                    String Land, String UBNnr, String Levensnr, DateTime DatumVerwerpen, DateTime TijdVerwerpen,
                                    String RelatienrWaarnemer,
                                    String pLogfile,
                                    ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, int pMaxStrLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDBewustGustMelding(String pUsername, String pPassword, Boolean pTestServer,
                                    String Land, String UBNnr, String Levensnr, DateTime DatumGust, DateTime TijdGust,
                                    String RelatienrWaarnemer,
                                    String pLogfile,
                                    ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, int pMaxStrLen);



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDDHZKImelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrKoe, DateTime Datum, DateTime Tijd,
                                String KiCodeStier, String LevensnrStier, String Chargenr, String RelatienrUitvoerende,
                                String pLogfile,
                                ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            dCRDDHZKImelding handle = (dCRDDHZKImelding)ExecuteProcedureDLL(typeof(dCRDDHZKImelding), "SOAPVBHALG.DLL", "CRDDHZKImelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            StringBuilder lLogfile = new StringBuilder(pLogfile);
            handle(pUsername, pPassword, pTestServer,
                              Land, UBNnr, LevensnrKoe, Datum, Tijd,
                              KiCodeStier, LevensnrStier, Chargenr, RelatienrUitvoerende,
                              lLogfile,
                              ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDNatuurlijkeDekkingMelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrKoe, DateTime Datum, DateTime Tijd,
                                String LevensnrStier, String RelatienrWaarnemer,
                                String pLogfile,
                                ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            dCRDNatuurlijkeDekkingMelding handle = (dCRDNatuurlijkeDekkingMelding)ExecuteProcedureDLL(typeof(dCRDNatuurlijkeDekkingMelding), "SOAPVBHALG.DLL", "CRDNatuurlijkeDekkingMelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                              Land, UBNnr, LevensnrKoe, Datum, Tijd,
                              LevensnrStier, RelatienrWaarnemer,
                              pLogfile,
                              ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDSamenWeidingMelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String LevensnrKoe, DateTime Datum, DateTime Tijd, DateTime DatumEind,
                                String LevensnrStier, String RelatienrWaarnemer,
                                String pLogfile,
                                ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            dCRDSamenWeidingMelding handle = (dCRDSamenWeidingMelding)ExecuteProcedureDLL(typeof(dCRDSamenWeidingMelding), "SOAPVBHALG.DLL", "CRDSamenWeidingMelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                              Land, UBNnr, LevensnrKoe, Datum, Tijd, DatumEind,
                              LevensnrStier, RelatienrWaarnemer,
                              pLogfile,
                               ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }





        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDTochtMelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime DatumTocht, DateTime TijdTocht,
                                String RelatienrWaarnemer,
                                String pLogfile,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {
            dCRDTochtMelding handle = (dCRDTochtMelding)ExecuteProcedureDLL(typeof(dCRDTochtMelding), "SOAPVBHALG.DLL", "CRDTochtMelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);

            handle(pUsername, pPassword, pTestServer,
                  Land, UBNnr, Levensnr, DatumTocht, TijdTocht,
                  RelatienrWaarnemer,
                  pLogfile,
                  ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDDrachtControleMelding(String pUsername, String pPassword, Boolean pTestServer,
                                    String Land, String UBNnr, String Levensnr, DateTime DatumDracht, DateTime TijdDracht,
                                    int StatusDracht, int MethodeControle,
                                    String RelatienrWaarnemer,
                                    String pLogfile,
                                    ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            dCRDDrachtControleMelding handle = (dCRDDrachtControleMelding)ExecuteProcedureDLL(typeof(dCRDDrachtControleMelding), "SOAPVBHALG.DLL", "CRDDrachtControleMelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);

            handle(pUsername, pPassword, pTestServer,
                  Land, UBNnr, Levensnr, DatumDracht, TijdDracht,
                  StatusDracht, MethodeControle, RelatienrWaarnemer,
                  pLogfile,
                  ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDVerwerpenMelding(String pUsername, String pPassword, Boolean pTestServer,
                                    String Land, String UBNnr, String Levensnr, DateTime DatumVerwerpen, DateTime TijdVerwerpen,
                                    String RelatienrWaarnemer,
                                    String pLogfile,
                                    ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            dCRDVerwerpenMelding handle = (dCRDVerwerpenMelding)ExecuteProcedureDLL(typeof(dCRDVerwerpenMelding), "SOAPVBHALG.DLL", "CRDVerwerpenMelding");

            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);

            handle(pUsername, pPassword, pTestServer,
                      Land, UBNnr, Levensnr, DatumVerwerpen, TijdVerwerpen,
                      RelatienrWaarnemer,
                      pLogfile,
                      ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDBewustGustMelding(String pUsername, String pPassword, Boolean pTestServer,
                                    String Land, String UBNnr, String Levensnr, DateTime DatumGust, DateTime TijdGust,
                                    String RelatienrWaarnemer,
                                    String pLogfile,
                                    ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            dCRDBewustGustMelding handle = (dCRDBewustGustMelding)ExecuteProcedureDLL(typeof(dCRDBewustGustMelding), "SOAPVBHALG.DLL", "CRDBewustGustMelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);

            handle(pUsername, pPassword, pTestServer,
            Land, UBNnr, Levensnr, DatumGust, TijdGust,
            RelatienrWaarnemer,
            pLogfile,
            ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
        }



        ~Win32SOAPVBHALG()
        {
            Thread.Sleep(new TimeSpan(0, 0, 0, 10, 0));
        }

    }
}
