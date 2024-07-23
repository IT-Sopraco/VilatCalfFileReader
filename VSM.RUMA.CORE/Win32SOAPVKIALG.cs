using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace VSM.RUMA.CORE
{
    public class Win32SOAPVKIALG : Win32
    {
        //   [ThreadStatic]
        //private static Win32SOAPVKIALG singleton;

        //public static Win32SOAPVKIALG Init()
        //{
        //    if (singleton == null)
        //    {
        //        singleton = new Win32SOAPVKIALG();
        //    }
        //    return singleton;
        //}
        public Win32SOAPVKIALG()
            : base(false)
        {
        }

        /*
         procedure CRVVKIaddPlannedDeparture(pUsername, pPassword: PAnsiChar; 
            pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; pSoortAfvoer: integer;
            pV1A1, pV2A1: boolean; pV2A2: PAnsiChar;
            pV3A1, pV4A1: boolean;
            pV4A2: PAnsiChar; pV5A1, pV6A1, pV7A1, pV8A1,
            pV9A1, pV10A1: boolean; pAANV1: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            pMaxStrLen: integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRVVKIaddPlannedDeparture(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr, int pSoortAfvoer,
                                bool pV1A1,bool pV2A1,String pV2A2,
                                bool pV3A1,bool pV4A1,
                                String pV4A2,bool pV5A1, bool pV6A1, bool pV7A1, bool pV8A1,
                                bool pV9A1,bool  pV10A1,String pAANV1,
                                ref String Status, ref String Code, ref String Omschrijving, 
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRVVKIaddPlannedDeparture(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr, int pSoortAfvoer,
                                bool pV1A1, bool pV2A1, String pV2A2,
                                bool pV3A1, bool pV4A1,
                                String pV4A2, bool pV5A1, bool pV6A1, bool pV7A1, bool pV8A1,
                                bool pV9A1, bool pV10A1, String pAANV1,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {


            dCRVVKIaddPlannedDeparture handle = (dCRVVKIaddPlannedDeparture)ExecuteProcedureDLL(typeof(dCRVVKIaddPlannedDeparture), "SOAPVKIALG.DLL", "CRVVKIaddPlannedDeparture");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword,pTestServer,
                                 pUBNnr,  pLevensnr, pSoortAfvoer,
                                 pV1A1,  pV2A1,  pV2A2,
                                 pV3A1,  pV4A1,
                                 pV4A2,  pV5A1,  pV6A1,  pV7A1,  pV8A1,
                                 pV9A1,  pV10A1,  pAANV1,
                                    ref  Status, ref  Code, ref  Omschrijving,
                                    pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }

        /*
         procedure CRVVKIdeletePlannedDeparture(pUsername, pPassword: PAnsiChar;
            pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; pSoortAfvoer: integer;
            var Status, Code, Omschrijving: PAnsiChar;
            pMaxStrLen: integer); stdcall;
        */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRVVKIdeletePlannedDeparture(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr, int pSoortAfvoer,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRVVKIdeletePlannedDeparture(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr, int pSoortAfvoer,
                                ref String Status, ref String Code, ref String Omschrijving,
                                int pMaxStrLen)
        {


            dCRVVKIdeletePlannedDeparture handle = (dCRVVKIdeletePlannedDeparture)ExecuteProcedureDLL(typeof(dCRVVKIdeletePlannedDeparture), "SOAPVKIALG.DLL", "CRVVKIdeletePlannedDeparture");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pUBNnr, pLevensnr, pSoortAfvoer,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }

        /*
         procedure CRVVKIlistPlannedDeparture(pUsername, pPassword: PAnsiChar; 
                pTestServer: integer;
                pUBNnr: PAnsiChar; pBegindat, pEinddat: TDateTime;
                var Status, Code, Omschrijving: PAnsiChar;
                var csvLevensnr, csvSoortafvoer, csvUitsteldatum, csvUitstelreden,
                csvAfkeurdatum, csvAfkeurreden, csvVertrekdatum, csvVertrekreden: PAnsiChar;
                pMaxStrLen: integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRVVKIlistPlannedDeparture(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr,DateTime pBegindat,DateTime pEinddat,
                                ref String Status, ref String Code, ref String Omschrijving,
                                ref String csvLevensnr, ref String csvSoortafvoer, ref String csvUitsteldatum, ref String csvUitstelreden,
                                ref String csvAfkeurdatum, ref String csvAfkeurreden, ref String csvVertrekdatum, ref String csvVertrekreden,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRVVKIlistPlannedDeparture(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, DateTime pBegindat, DateTime pEinddat,
                                ref String Status, ref String Code, ref String Omschrijving,
                                ref String csvLevensnr, ref String csvSoortafvoer, ref String csvUitsteldatum, ref String csvUitstelreden,
                                ref String csvAfkeurdatum, ref String csvAfkeurreden, ref String csvVertrekdatum, ref String csvVertrekreden,                    
            int pMaxStrLen)
        {


            dCRVVKIlistPlannedDeparture handle = (dCRVVKIlistPlannedDeparture)ExecuteProcedureDLL(typeof(dCRVVKIlistPlannedDeparture), "SOAPVKIALG.DLL", "CRVVKIlistPlannedDeparture");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);

            StringBuilder lcsvLevensnr = new StringBuilder();
            lcsvLevensnr.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvSoortafvoer = new StringBuilder();
            lcsvSoortafvoer.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvUitsteldatum = new StringBuilder();
            lcsvUitsteldatum.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvUitstelreden = new StringBuilder();
            lcsvUitstelreden.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvAfkeurdatum = new StringBuilder();
            lcsvAfkeurdatum.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvAfkeurreden = new StringBuilder();
            lcsvAfkeurreden.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvVertrekdatum = new StringBuilder();
            lcsvVertrekdatum.EnsureCapacity(pMaxStrLen);
            StringBuilder lcsvVertrekreden = new StringBuilder();
            lcsvVertrekreden.EnsureCapacity(pMaxStrLen);

            handle(pUsername, pPassword, pTestServer,
                                 pUBNnr,  pBegindat, pEinddat,
                                    ref  Status, ref  Code, ref  Omschrijving,
                                    ref csvLevensnr, ref  csvSoortafvoer, ref  csvUitsteldatum, ref  csvUitstelreden,
                                    ref csvAfkeurdatum, ref  csvAfkeurreden, ref  csvVertrekdatum, ref  csvVertrekreden,
                                    pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

            csvLevensnr = lcsvLevensnr.ToString();
            csvSoortafvoer = lcsvSoortafvoer.ToString();
            csvUitsteldatum = lcsvUitsteldatum.ToString();
            csvUitstelreden = lcsvUitstelreden.ToString();
            csvAfkeurdatum = lcsvAfkeurdatum.ToString();
            csvAfkeurreden = lcsvAfkeurreden.ToString();
            csvVertrekdatum = lcsvVertrekdatum.ToString();
            csvVertrekreden = lcsvVertrekreden.ToString();
           

        }

        /*
         procedure CBDVKIaddVKIFormulier(pUsername, pPassword: PAnsiChar;
            pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; 
            pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, 
            pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9, pV10: boolean;
            pV5opm, pV6opm, pV7opm, pV8opm, pV9opm, pV10opm: PAnsiChar;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedVoorVraag, csvMedRegNL, csvMedNaam, csvDatumLstBehandeling,
            csvDatumEindeWachttijd, csvDiagnoseBehandeling: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;

         * 31/12/2010
         procedure CBDVKIaddVKIFormulier(
            pUsername, pPassword: PAnsiChar;
            pTestServer: integer;
            pUBNnr, csvLevensnrs: PAnsiChar;
            pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, 
            pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9, pV10: boolean;
            csvV5opm, pV6opm, pV7opm, pV8opm, pV9opm, pV10opm: PAnsiChar;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedicijnen, csvDiagnoseBehandeling: PAnsiChar;
            var csvStatus, csvCode, csvOmschrijving: PAnsiChar;
            pMaxStrLen: integer); stdcall;

         * 
         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIaddVKIFormulier(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String csvLevensnrs, 
                                DateTime pAfleverdatum,DateTime pDatumOndertekening,
                                String pVoorletters,String pTussenvoegsel,String pAchternaam, 
                                String pPlaatsOndertekening,
            bool pV1,bool pV2,bool  pV3,bool  pV4,bool  pV5,bool  pV6,bool  pV7,bool  pV8,bool  pV9,bool  pV10,
            String csvV5opm, String pV6opm, String pV7opm, String pV8opm, String pV9opm, String pV10opm,
            String pDAPNaam, String pDAPStraat,String  pDAPHuisnr,String  pDAPHuisnrToevoeging,
            String pDAPPostcode, String pDAPPlaats,String  pDAPTelnr,String  pEmailadres1,String  pEmailadres2,
            String pFaxnummer1, String pFaxnummer2,
            String csvMedicijnen, String csvDiagnoseBehandeling,
                                ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIaddVKIFormulier(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String csvLevensnrs,
                                DateTime pAfleverdatum, DateTime pDatumOndertekening,
                                String pVoorletters, String pTussenvoegsel, String pAchternaam,
                                String pPlaatsOndertekening,
            bool pV1, bool pV2, bool pV3, bool pV4, bool pV5, bool pV6, bool pV7, bool pV8, bool pV9, bool pV10,
            String csvV5opm, String pV6opm, String pV7opm, String pV8opm, String pV9opm, String pV10opm,
            String pDAPNaam, String pDAPStraat, String pDAPHuisnr, String pDAPHuisnrToevoeging,
            String pDAPPostcode, String pDAPPlaats, String pDAPTelnr, String pEmailadres1, String pEmailadres2,
            String pFaxnummer1, String pFaxnummer2,
            String csvMedicijnen, String csvDiagnoseBehandeling,
                                ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                                int pMaxStrLen)
        {

            lock (typeof(Win32SOAPVKIALG))
            {
                try
                {
                    dCBDVKIaddVKIFormulier handle = (dCBDVKIaddVKIFormulier)ExecuteProcedureDLL(typeof(dCBDVKIaddVKIFormulier), "SOAPVKIALG.DLL", "CBDVKIaddVKIFormulier");
                    //StringBuilder lStatus = new StringBuilder();
                    //lStatus.EnsureCapacity(pMaxStrLen);
                    //StringBuilder lCode = new StringBuilder();
                    //lCode.EnsureCapacity(pMaxStrLen);
                    //StringBuilder lOmschrijving = new StringBuilder();
                    //lOmschrijving.EnsureCapacity(pMaxStrLen);


                    handle(pUsername, pPassword, pTestServer,
                                         pUBNnr, csvLevensnrs,
                                         pAfleverdatum, pDatumOndertekening,
                                         pVoorletters, pTussenvoegsel, pAchternaam,
                                         pPlaatsOndertekening,
                                         pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9, pV10,
                                         csvV5opm, pV6opm, pV7opm, pV8opm, pV9opm, pV10opm,
                                         pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
                                         pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
                                         pFaxnummer1, pFaxnummer2,
                                         csvMedicijnen, csvDiagnoseBehandeling,
                                         ref  csvStatus, ref  csvCode, ref csvOmschrijving,
                                         pMaxStrLen);
                    if (!FreeDLL("SOAPVKIALG.DLL"))
                    {
                        unLogger.WriteInfo("free SOAPVKIALG.DLL niet gelukt.");
                    }
                }
                catch(Exception exc)
                {
                    unLogger.WriteInfo(exc.ToString());
                }
                //csvStatus = lStatus.ToString();
                //csvCode = lCode.ToString();
                //csvOmschrijving = lOmschrijving.ToString();
            }
        }

        /*procedure CBDVKIGetSlachtKeurgegevens(pUsername, pPassword: PAnsiChar; 
            pTestServer: integer;
            pLevensnr: pAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            var pSlachtdatum: TDateTime;
            var pABpositief, pVinnen: boolean;
            pMaxStrLen: integer); stdcall;
            */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIGetSlachtKeurgegevens(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                DateTime pSlachtdatum,
                                bool pABpositief,bool pVinnen,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIGetSlachtKeurgegevens(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
            DateTime pSlachtdatum,
                                bool pABpositief, bool pVinnen,                    
            int pMaxStrLen)
        {


            dCBDVKIGetSlachtKeurgegevens handle = (dCBDVKIGetSlachtKeurgegevens)ExecuteProcedureDLL(typeof(dCBDVKIGetSlachtKeurgegevens), "SOAPVKIALG.DLL", "CBDVKIGetSlachtKeurgegevens");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pLevensnr,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pSlachtdatum,
                                 pABpositief,pVinnen,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }

        /*
         procedure CBDVKIGetVKI(pUsername, pPassword: PAnsiChar; 
            pTestServer: integer; pLevensnr: pAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            var pVKIaanwezig: boolean;
            pMaxStrLen: integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIGetVKI(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                bool pVKIaanwezig,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIGetVKI(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                bool pVKIaanwezig,
                                int pMaxStrLen)
        {


            dCBDVKIGetVKI handle = (dCBDVKIGetVKI)ExecuteProcedureDLL(typeof(dCBDVKIGetVKI), "SOAPVKIALG.DLL", "CBDVKIGetVKI");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pLevensnr,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pVKIaanwezig,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }


        /*procedure CBDVKIaddVKIFormulierSchaap(pUsername, pPassword: PAnsiChar;
             pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9: boolean;
            pV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm: PAnsiChar;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedVoorVraag, csvMedRegNL, csvMedNaam, csvDatumLstBehandeling,
            csvDatumEindeWachttijd, csvDiagnoseBehandeling: PAnsiChar;
            pLogfile,
            var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;

         * 
         * procedure CBDVKIaddVKIFormulierSchaap(pUsername, pPassword: PAnsiChar;
             pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9: boolean;
            pV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm: PAnsiChar;
            pExportWaardigheid: integer;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedVoorVraag, csvMedRegNL, csvMedNaam, csvDatumLstBehandeling,
            csvDatumEindeWachttijd, csvDiagnoseBehandeling: PAnsiChar;
            pLogfile: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;

         31/12/2010
         *procedure CBDVKIaddVKIFormulierSchaap(
            pUsername, pPassword: PAnsiChar;
            pTestServer: integer;
            pUBNnr, csvLevensnrs: PAnsiChar;
            pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, 
            pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9: boolean;
            csvV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm: PAnsiChar;
            pExportWaardigheid: integer;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedicijnen, csvDiagnoseBehandeling,
            pLogfile: PAnsiChar;
            var csvStatus, csvCode, csvOmschrijving: PAnsiChar;
            pMaxStrLen: integer); stdcall;

            */
        //1++++
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIaddVKIFormulierSchaap(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String csvLevensnrs,
                                DateTime pAfleverdatum, DateTime pDatumOndertekening,
                                String pVoorletters, String pTussenvoegsel, String pAchternaam,
                                String pPlaatsOndertekening,
            bool pV1, bool pV2, bool pV3, bool pV4, bool pV5, bool pV6, bool pV7, bool pV8, bool pV9,
            String csvV4opm, String pV5opm, String pV6opm, String pV7opm, String pV8opm, String pV9opm, 
            int pExportWaardigheid,
            String pDAPNaam, String pDAPStraat, String pDAPHuisnr, String pDAPHuisnrToevoeging,
            String pDAPPostcode, String pDAPPlaats, String pDAPTelnr, String pEmailadres1, String pEmailadres2,
            String pFaxnummer1, String pFaxnummer2,
            String csvMedicijnen, String csvDiagnoseBehandeling,
            String pLogfile,
                                ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                                int pMaxStrLen);
        //1++++
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIaddVKIFormulierSchaap(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String csvLevensnrs,
                                DateTime pAfleverdatum, DateTime pDatumOndertekening,
                                String pVoorletters, String pTussenvoegsel, String pAchternaam,
                                String pPlaatsOndertekening,
            bool pV1, bool pV2, bool pV3, bool pV4, bool pV5, bool pV6, bool pV7, bool pV8, bool pV9,
            String csvV4opm, String pV5opm, String pV6opm, String pV7opm, String pV8opm, String pV9opm, 
            int pExportWaardigheid,
            String pDAPNaam, String pDAPStraat, String pDAPHuisnr, String pDAPHuisnrToevoeging,
            String pDAPPostcode, String pDAPPlaats, String pDAPTelnr, String pEmailadres1, String pEmailadres2,
            String pFaxnummer1, String pFaxnummer2,
            String csvMedicijnen, String csvDiagnoseBehandeling,
            String pLogfile,
                                ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                                int pMaxStrLen)
        {

            lock (typeof(Win32SOAPVKIALG))
            {
                try
                {
                    dCBDVKIaddVKIFormulierSchaap handle = (dCBDVKIaddVKIFormulierSchaap)ExecuteProcedureDLL(typeof(dCBDVKIaddVKIFormulierSchaap), "SOAPVKIALG.DLL", "CBDVKIaddVKIFormulierSchaap");

                    //StringBuilder lStatus = new StringBuilder();
                    //lStatus.EnsureCapacity(pMaxStrLen);
                    //StringBuilder lCode = new StringBuilder();
                    //lCode.EnsureCapacity(pMaxStrLen);
                    //StringBuilder lOmschrijving = new StringBuilder();
                    //lOmschrijving.EnsureCapacity(pMaxStrLen);


                    handle(pUsername, pPassword, pTestServer,
                                         pUBNnr, csvLevensnrs,
                                         pAfleverdatum, pDatumOndertekening,
                                         pVoorletters, pTussenvoegsel, pAchternaam,
                                         pPlaatsOndertekening,
                                         pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9,
                                         csvV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm,
                                         pExportWaardigheid,
                                         pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
                                         pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
                                         pFaxnummer1, pFaxnummer2,
                                         csvMedicijnen, csvDiagnoseBehandeling,
                                         pLogfile,
                                         ref  csvStatus, ref  csvCode, ref  csvOmschrijving,
                                         pMaxStrLen);
                    if (!FreeDLL("SOAPVKIALG.DLL"))
                    {
                        unLogger.WriteInfo("free SOAPVKIALG.DLL niet gelukt.");
                    }
                }
                catch (Exception exc)
                {
                    unLogger.WriteInfo(exc.ToString());
                }
                //csvStatus = lStatus.ToString();
                //csvCode = lCode.ToString();
                //csvOmschrijving = lOmschrijving.ToString();

            }
        }
        //===========================================
        /*
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCRDIRaanvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime Aanvoerdat,
                                int SoortAanvoer,
                                ref StringBuilder Status, ref StringBuilder Code, ref StringBuilder Omschrijving, ref StringBuilder UBNherkomst,
                                String pLogfile, int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CRDIRaanvoermelding(String pUsername, String pPassword, Boolean pTestServer,
                                String Land, String UBNnr, String Levensnr, DateTime Aanvoerdat,
                                int SoortAanvoer,
                                ref String Status, ref String Code, ref String Omschrijving, ref String UBNherkomst,
                                String pLogfile, int pMaxStrLen)
        {


            dCRDIRaanvoermelding handle = (dCRDIRaanvoermelding)ExecuteProcedureDLL(typeof(dCRDIRaanvoermelding), "SOAPIRALG.DLL", "CRDIRaanvoermelding");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            StringBuilder lUBNherkomst = new StringBuilder();
            lUBNherkomst.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                Land, UBNnr, Levensnr, Aanvoerdat,
                                SoortAanvoer,
                                ref lStatus, ref lCode, ref lOmschrijving, ref lUBNherkomst, pLogfile, pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();
            UBNherkomst = lUBNherkomst.ToString();

        }*/
        //==============================================
        /*
         procedure CBDVKIGetSlachtKeurgegevensSchaap(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                pLevensnr: pAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                var pSlachtdatum: TDateTime;
                var pAanvulling: PAnsiChar;
                var pLarven, pABpositief: boolean;
                pMaxStrLen: integer); stdcall;
         * 
         * 
         * procedure CBDVKIGetSlachtKeurgegevensSchaap(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                pUBNnr, pLevensnr: pAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                var pSlachtdatum: TDateTime;
                var pAanvulling: PAnsiChar;
                var pLarven, pABpositief: boolean;
                pMaxStrLen: integer); stdcall;


         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIGetSlachtKeurgegevensSchaap(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                DateTime pSlachtdatum,
                                String pAanvulling,
                                bool pLarven,bool pABpositief,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIGetSlachtKeurgegevensSchaap(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                ref DateTime pSlachtdatum,
                                ref String pAanvulling,
                                ref bool pLarven, ref bool pABpositief,
                                int pMaxStrLen)
        {


            dCBDVKIGetSlachtKeurgegevensSchaap handle = (dCBDVKIGetSlachtKeurgegevensSchaap)ExecuteProcedureDLL(typeof(dCBDVKIGetSlachtKeurgegevensSchaap), "SOAPVKIALG.DLL", "CBDVKIGetSlachtKeurgegevensSchaap");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pUBNnr, pLevensnr,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pSlachtdatum,
                                 pAanvulling,
                                 pLarven, pABpositief,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }

        /*
         procedure CBDVKIGetVKIschaap(pUsername, pPassword: PAnsiChar; 
            pTestServer: integer;
            pLevensnr: pAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            var pVKIaanwezig: boolean;
            pMaxStrLen: integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIGetVKIschaap(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                bool pVKIaanwezig,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIGetVKIschaap(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                bool pVKIaanwezig,
                                int pMaxStrLen)
        {


            dCBDVKIGetVKIschaap handle = (dCBDVKIGetVKIschaap)ExecuteProcedureDLL(typeof(dCBDVKIGetVKIschaap), "SOAPVKIALG.DLL", "CBDVKIGetVKIschaap");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pLevensnr,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pVKIaanwezig,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }

        /*procedure CBDVKIaddVKIFormulierGeit(pUsername, pPassword: PAnsiChar;
             pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9: boolean;
            pV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm: PAnsiChar;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedVoorVraag, csvMedRegNL, csvMedNaam, csvDatumLstBehandeling,
            csvDatumEindeWachttijd, csvDiagnoseBehandeling: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;
         
         * procedure CBDVKIaddVKIFormulierGeit(pUsername, pPassword: PAnsiChar;
             pTestServer: integer;
            pUBNnr, pLevensnr: PAnsiChar; pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9: boolean;
            pV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm: PAnsiChar;
            pExportWaardigheid: integer;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedVoorVraag, csvMedRegNL, csvMedNaam, csvDatumLstBehandeling,
            csvDatumEindeWachttijd, csvDiagnoseBehandeling: PAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar; pMaxStrLen: integer); stdcall;

         31/12/2010
         * procedure CBDVKIaddVKIFormulierGeit(
            pUsername, pPassword: PAnsiChar; pTestServer: integer;
            pUBNnr, csvLevensnrs: PAnsiChar;
            pAfleverdatum, pDatumOndertekening: TDateTime;
            pVoorletters, pTussenvoegsel, pAchternaam, 
            pPlaatsOndertekening: PAnsiChar;
            pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9: boolean;
            csvV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm: PAnsiChar;
            pExportWaardigheid: integer;
            pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
            pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
            pFaxnummer1, pFaxnummer2: PAnsiChar;
            csvMedicijnen, csvDiagnoseBehandeling: PAnsiChar;
            var csvStatus, csvCode, csvOmschrijving: PAnsiChar;
            pMaxStrLen: integer); stdcall;

         */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIaddVKIFormulierGeit(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String csvLevensnrs,
                                DateTime pAfleverdatum, DateTime pDatumOndertekening,
                                String pVoorletters, String pTussenvoegsel, String pAchternaam,
                                String pPlaatsOndertekening,
            bool pV1, bool pV2, bool pV3, bool pV4, bool pV5, bool pV6, bool pV7, bool pV8, bool pV9,
            String csvV4opm, String pV5opm, String pV6opm, String pV7opm, String pV8opm, String pV9opm,
            int pExportWaardigheid,
            String pDAPNaam, String pDAPStraat, String pDAPHuisnr, String pDAPHuisnrToevoeging,
            String pDAPPostcode, String pDAPPlaats, String pDAPTelnr, String pEmailadres1, String pEmailadres2,
            String pFaxnummer1, String pFaxnummer2,
            String csvMedicijnen, String csvDiagnoseBehandeling,
                                ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIaddVKIFormulierGeit(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String csvLevensnrs,
                                DateTime pAfleverdatum, DateTime pDatumOndertekening,
                                String pVoorletters, String pTussenvoegsel, String pAchternaam,
                                String pPlaatsOndertekening,
            bool pV1, bool pV2, bool pV3, bool pV4, bool pV5, bool pV6, bool pV7, bool pV8, bool pV9,
            String csvV4opm, String pV5opm, String pV6opm, String pV7opm, String pV8opm, String pV9opm,
            int pExportWaardigheid,
            String pDAPNaam, String pDAPStraat, String pDAPHuisnr, String pDAPHuisnrToevoeging,
            String pDAPPostcode, String pDAPPlaats, String pDAPTelnr, String pEmailadres1, String pEmailadres2,
            String pFaxnummer1, String pFaxnummer2,
            String csvMedicijnen, String csvDiagnoseBehandeling,
                                ref StringBuilder csvStatus, ref StringBuilder csvCode, ref StringBuilder csvOmschrijving,
                                int pMaxStrLen)
        {
            lock (typeof(Win32SOAPVKIALG))
            {
                try
                {
                    dCBDVKIaddVKIFormulierGeit handle = (dCBDVKIaddVKIFormulierGeit)ExecuteProcedureDLL(typeof(dCBDVKIaddVKIFormulierGeit), "SOAPVKIALG.DLL", "CBDVKIaddVKIFormulierGeit");
                    //StringBuilder lStatus = new StringBuilder();
                    //lStatus.EnsureCapacity(pMaxStrLen);
                    //StringBuilder lCode = new StringBuilder();
                    //lCode.EnsureCapacity(pMaxStrLen);
                    //StringBuilder lOmschrijving = new StringBuilder();
                    //lOmschrijving.EnsureCapacity(pMaxStrLen);


                    handle(pUsername, pPassword, pTestServer,
                                         pUBNnr, csvLevensnrs,
                                         pAfleverdatum, pDatumOndertekening,
                                         pVoorletters, pTussenvoegsel, pAchternaam,
                                         pPlaatsOndertekening,
                                         pV1, pV2, pV3, pV4, pV5, pV6, pV7, pV8, pV9,
                                         csvV4opm, pV5opm, pV6opm, pV7opm, pV8opm, pV9opm,
                                         pExportWaardigheid,
                                         pDAPNaam, pDAPStraat, pDAPHuisnr, pDAPHuisnrToevoeging,
                                         pDAPPostcode, pDAPPlaats, pDAPTelnr, pEmailadres1, pEmailadres2,
                                         pFaxnummer1, pFaxnummer2,
                                         csvMedicijnen, csvDiagnoseBehandeling,
                                         ref  csvStatus, ref  csvCode, ref  csvOmschrijving,
                                         pMaxStrLen);
                    if (!FreeDLL("SOAPVKIALG.DLL"))
                    {
                        unLogger.WriteInfo("free SOAPVKIALG.DLL niet gelukt.");
                    }
                }
                catch (Exception exc)
                {
                    unLogger.WriteInfo(exc.ToString());
                }
                //csvStatus = lStatus.ToString();
                //csvCode = lCode.ToString();
                //csvOmschrijving = lOmschrijving.ToString();
            }

        }

        /*procedure CBDVKIGetSlachtKeurgegevensGeit(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                pLevensnr: pAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                var pSlachtdatum: TDateTime;
                var pAanvulling: PAnsiChar;
                var pLarven, pABpositief: boolean;
                pMaxStrLen: integer); stdcall;
         * 
         * procedure CBDVKIGetSlachtKeurgegevensGeit(
                pUsername, pPassword: PAnsiChar; pTestServer: integer;
                pUBNnr, pLevensnr: pAnsiChar;
                var Status, Code, Omschrijving: PAnsiChar;
                var pSlachtdatum: TDateTime;
                var pAanvulling: PAnsiChar;
                var pLarven, pABpositief: boolean;
                pMaxStrLen: integer); stdcall;

            */
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIGetSlachtKeurgegevensGeit(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                DateTime pSlachtdatum,
                                String pAanvulling,
                                bool pLarven, bool pABpositief,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIGetSlachtKeurgegevensGeit(String pUsername, String pPassword, int pTestServer,
                                String pUBNnr, String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                DateTime pSlachtdatum,
                                String pAanvulling,
                                bool pLarven, bool pABpositief,
                                int pMaxStrLen)
        {


            dCBDVKIGetSlachtKeurgegevensGeit handle = (dCBDVKIGetSlachtKeurgegevensGeit)ExecuteProcedureDLL(typeof(dCBDVKIGetSlachtKeurgegevensGeit), "SOAPVKIALG.DLL", "CBDVKIGetSlachtKeurgegevensGeit");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pUBNnr, pLevensnr,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pSlachtdatum,
                                 pAanvulling,
                                 pLarven, pABpositief,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }

        /*
         procedure CBDVKIGetVKIgeit(pUsername, pPassword: PAnsiChar; 
            pTestServer: integer;
            pLevensnr: pAnsiChar;
            var Status, Code, Omschrijving: PAnsiChar;
            var pVKIaanwezig: boolean;
            pMaxStrLen: integer); stdcall;

         */

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        delegate void dCBDVKIGetVKIgeit(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                bool pVKIaanwezig,
                                int pMaxStrLen);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CBDVKIGetVKIgeit(String pUsername, String pPassword, int pTestServer,
                                String pLevensnr,
                                ref String Status, ref String Code, ref String Omschrijving,
                                bool pVKIaanwezig,
                                int pMaxStrLen)
        {


            dCBDVKIGetVKIgeit handle = (dCBDVKIGetVKIgeit)ExecuteProcedureDLL(typeof(dCBDVKIGetVKIgeit), "SOAPVKIALG.DLL", "CBDVKIGetVKIgeit");
            StringBuilder lStatus = new StringBuilder();
            lStatus.EnsureCapacity(pMaxStrLen);
            StringBuilder lCode = new StringBuilder();
            lCode.EnsureCapacity(pMaxStrLen);
            StringBuilder lOmschrijving = new StringBuilder();
            lOmschrijving.EnsureCapacity(pMaxStrLen);
            handle(pUsername, pPassword, pTestServer,
                                 pLevensnr,
                                 ref  Status, ref  Code, ref  Omschrijving,
                                 pVKIaanwezig,
                                 pMaxStrLen);
            Status = lStatus.ToString();
            Code = lCode.ToString();
            Omschrijving = lOmschrijving.ToString();

        }
    }
}
