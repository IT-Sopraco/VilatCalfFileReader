using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    class Win32SOAPREPROALG : Win32
    {

        public Win32SOAPREPROALG()
            : base(false)
        {
        }


        static readonly object padlock = new object();


        private static LockObject[] LockList;

        public static LockObject[] GetLockList()
        {

            lock (padlock)
            {
                if (LockList == null)
                {
                    List<LockObject> locklist = new List<LockObject>();
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\SOAPREPROALG.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\SOAPREPROALG{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        public delegate void Callback(int PercDone, string Msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRVaddAnimalReproductionIndication(
        string pUsername, string pPassword, string pUbnnr, string pKlantnr, string pLand, int pTestServer,
        string pLevensnr,
        DateTime pDatumTijd, DateTime pEindDatumTijd,
        int pSoort, int pZekerheid,
        string pLogFile,
        ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AB_CRDaddAnimalReproductionIndication(
        string pUsername, string pPassword, string pUbnnr, string pKlantnr, string pLand, bool pTestServer,
        string Levensnr,
        DateTime IndicatieStartDatum, DateTime IndicatieEindDatum,
        int pSoort, int pZekerheid,
        string pLogFile,
        ref string Status, ref string Code, ref string Omschrijving, int pMaxStrLen)
        {

            LockObject padlock;
            dCRVaddAnimalReproductionIndication handle = (dCRVaddAnimalReproductionIndication)ExecuteProcedureDLLStack(typeof(dCRVaddAnimalReproductionIndication),"CRVaddAnimalReproductionIndication", GetLockList(), out padlock);
            try
            {
                StringBuilder lStatus = new StringBuilder();
                lStatus.EnsureCapacity(pMaxStrLen);
                StringBuilder lCode = new StringBuilder();
                lCode.EnsureCapacity(pMaxStrLen);
                StringBuilder lOmschrijving = new StringBuilder();
                lOmschrijving.EnsureCapacity(pMaxStrLen);
                StringBuilder lUBNherkomst = new StringBuilder();
                lUBNherkomst.EnsureCapacity(pMaxStrLen);
                int TestServer = 0;
                if (pTestServer) TestServer = 1;

                handle(pUsername, pPassword, pUbnnr, pKlantnr, pLand, TestServer,
                        Levensnr,
                        IndicatieStartDatum, IndicatieEindDatum,
                        pSoort, pZekerheid,
                        pLogFile,
                        ref lStatus, ref lCode, ref lOmschrijving, pMaxStrLen);

                Status = lStatus.ToString();
                if (lCode != null) Code = lCode.ToString();
                Omschrijving = lOmschrijving.ToString();
            }
            finally
            {
                System.Threading.Monitor.Exit(padlock);
            }
        }
    }
}
