using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32Lst_LabelPrinter : Win32
    {
        [ThreadStatic]
        private static Win32Lst_LabelPrinter singleton;

        public static Win32Lst_LabelPrinter Init()
        {
            if (singleton == null)
            {
                singleton = new Win32Lst_LabelPrinter();
            }
            return singleton;
        }

        public int call_lst_AB_LabelPrinter_Bedrijf(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        int pFarmId)
        {
            lock (typeof(Win32Lst_LabelPrinter))
            {
                string sFilename = "Lst_LabelPrinter.dll";

                lst_AB_LabelPrinter_Bedrijf handle =
                    (lst_AB_LabelPrinter_Bedrijf)ExecuteProcedureDLL(
                        typeof(lst_AB_LabelPrinter_Bedrijf), sFilename, "lst_AB_LabelPrinter_Bedrijf");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pFarmId);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public int call_lst_AB_LabelPrinter_Animal(
                                int pProgID, int pProgramID, int pSoortBestand,
                                string pUbnnr, string pHostName, string pUserName, string pPassword,
                                string pBestand, string pLog,
                                pCallback ReadDataProc,
                                string pLifeNumber)
        {
            lock (typeof(Win32Lst_LabelPrinter))
            {
                string sFilename = "Lst_LabelPrinter.dll";

                lst_AB_LabelPrinter_Animal handle =
                    (lst_AB_LabelPrinter_Animal)ExecuteProcedureDLL(
                        typeof(lst_AB_LabelPrinter_Animal), sFilename, "lst_AB_LabelPrinter_Animal");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pLifeNumber);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        public int call_lst_AB_LabelPrinter_BedrijfZiekte(
                        int pProgID, int pProgramID, int pSoortBestand,
                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                        string pBestand, string pLog,
                        pCallback ReadDataProc,
                        int pBedrijfZiekteId, int pAantalWeken)
        {
            lock (typeof(Win32Lst_LabelPrinter))
            {
                string sFilename = "Lst_LabelPrinter.dll";

                lst_AB_LabelPrinter_BedrijfZiekte handle =
                    (lst_AB_LabelPrinter_BedrijfZiekte)ExecuteProcedureDLL(
                        typeof(lst_AB_LabelPrinter_BedrijfZiekte), sFilename, "lst_AB_LabelPrinter_BedrijfZiekte");

                int tmp = handle(pProgID, pProgramID, pSoortBestand,
                                 pUbnnr, pHostName, pUserName, Decodeer_PW(pPassword), pBestand, pLog,
                                 ReadDataProc,
                                 pBedrijfZiekteId, pAantalWeken);

                FreeDLL(sFilename);
                return tmp;
            }
        }

        delegate int lst_AB_LabelPrinter_Bedrijf(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        int pFarmId);

        delegate int lst_AB_LabelPrinter_Animal(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        string pLifeNumber);

        delegate int lst_AB_LabelPrinter_BedrijfZiekte(
                                        int pProgID, int pProgramID, int pSoortBestand,
                                        string pUbnnr, string pHostName, string pUserName, string pPassword,
                                        string pBestand, string pLog,
                                        pCallback ReadDataProc,
                                        int pBedrijfZiekteId, int pAantalWeken);

        public delegate void pCallback(int PercDone, string Msg);
    }
}
