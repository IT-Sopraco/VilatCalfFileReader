using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace VSM.RUMA.CORE
{
    class Win32SOAPIMPNRS : Win32
    {

        public Win32SOAPIMPNRS()
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
                    FileInfo fileInf = new FileInfo(GetBaseDir() + "lib\\SOAPIMPNRS.DLL");
                    int add = 1;
                    while (fileInf.Exists || add == 1)
                    {
                        if (fileInf.Exists)
                            locklist.Add(new LockObject(fileInf.Name));
                        fileInf = new FileInfo(String.Format(GetBaseDir() + "lib\\SOAPIMPNRS{0}.DLL", add));
                        add++;
                    }
                    LockList = locklist.ToArray();
                }
                return LockList;
            }
        }

        public delegate void Callback(int PercDone, string Msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void AB_CRDAnimalReproduction(
        int pPrognr, int pProgramID,
        string pLandCode, string pUbnnr, string pUserNameAB, string pPasswordAB,
        string pHostName, string pLogdir,
        string pKlantnr, string pUserNameCRD, string pPasswordCRD,
        bool pCRDtestserver, Callback pCallback,
        ref int AantDieren,
        ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, int pMaxStrLen);




        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dAB_CRDaddAnimalReproductionIndication(
        string pUserNameCRD, string pPasswordCRD, bool pCRDtestserver,
        string pLandCode, string pUbnnr, string Levensnr, string Indicatie, string Indzekerheid,
        DateTime IndicatieStartDatum, DateTime IndicatieEindDatum,
         string pLogdir,
        ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, int pMaxStrLen);


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDAnimalReproduction(
        int pPrognr, int pProgramID,
        string pLandCode, string pUbnnr, string pUserNameAB, string pPasswordAB,
        string pHostName, string pLogdir,
        string pKlantnr, string pUserNameCRD, string pPasswordCRD,
        bool pCRDtestserver, Callback pCallback,
        ref int AantDieren,
        ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            LockObject padlock;

            //AB_CRDAnimalReproduction handle = (AB_CRDAnimalReproduction)ExecuteProcedureDLL(typeof(AB_CRDAnimalReproduction), "SOAPIMPNRS.DLL", "AB_CRDAnimalReproduction");
            AB_CRDAnimalReproduction handle = (AB_CRDAnimalReproduction)ExecuteProcedureDLLStack(typeof(AB_CRDAnimalReproduction), "AB_CRDAnimalReproduction", GetLockList(), out padlock);
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
                handle(pPrognr, pProgramID,
                                pLandCode, pUbnnr, pUserNameAB, pPasswordAB,
                                pHostName, pLogdir,
                                pKlantnr, pUserNameCRD, pPasswordCRD,
                                pCRDtestserver, pCallback,
                                ref AantDieren,
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



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AB_CRDaddAnimalReproductionIndication(
        string pUserNameCRD, string pPasswordCRD, bool pCRDtestserver,
        string pLandCode, string pUbnnr, string Levensnr, string Indicatie, string Indzekerheid,
        DateTime IndicatieStartDatum, DateTime IndicatieEindDatum,
         string pLogdir,
        ref String Status, ref String Code, ref String Omschrijving, int pMaxStrLen)
        {
            LockObject padlock;

            //dAB_CRDaddAnimalReproductionIndication handle = (dAB_CRDaddAnimalReproductionIndication)ExecuteProcedureDLL(typeof(dAB_CRDaddAnimalReproductionIndication), "SOAPIMPNRS.DLL", "CRDaddAnimalReproductionIndication");
            dAB_CRDaddAnimalReproductionIndication handle = (dAB_CRDaddAnimalReproductionIndication)ExecuteProcedureDLLStack(typeof(dAB_CRDaddAnimalReproductionIndication), "CRDaddAnimalReproductionIndication", GetLockList(), out padlock);
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
                handle(pUserNameCRD, pPasswordCRD, pCRDtestserver,
                                pLandCode, pUbnnr, Levensnr, Indicatie, Indzekerheid,
                                IndicatieStartDatum, IndicatieEindDatum,
                                pLogdir,
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
