using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE
{

    public static class configHelper
    {
        private const string CONFIGKEY_SANITELUSERNAME = "SanitelUsername";
        private const string CONFIGKEY_SANITELPASSWORD = "SanitelPassword";

        private const string CONFIGKEY_LNVUSERNAME = "LNVUsername";
        private const string CONFIGKEY_LNVPASSWORD = "LNVPassword";

        private const string CONFIGKEY_RUNPARRALEL = "RunParralel";
        private const string CONFIGKEY_CRV_Missing_Data_Task_Get_Additional_Info = "CRV_Missing_Data_Task_Get_Additional_Info";
        private const string CONFIGKEY_INRCORRIGEERWORPMOEDER = "InRCorrigeerWorpMoeder";
        private const string CONFIGKEY_INRUPDATEVERIFIEDANIMAL = "InRUpdateVerifiedAnimal";
        private const string CONFIGKEY_INRUSEVERIFIEDANIMAL = "InRUseVerifiedAnimal";
        private const string CONFIGKEY_INRMAXAGEVERIFIEDANIMALMINUTES = "InRMaxAgeVerifiedAnimalMinutes";
        private const string CONFIGKEY_REPORTCALFLESSBIRTHASBORNDEADAFTERDAYS = "ReportCalflessBirthAsBornDeadAfterDays";

        private const string CONFIGKEY_INRFORCECHECKALLPRESENTANIMALS = "InRForceCheckALlPresentAnimals";
        private const string CONFIGKEY_CHECKANDFORCEANIMALSWITHOUTHBIRTHMOVEMENT = "CheckAndForceAnimalsWithouthBirthMovement";
        private const string CONFIGKEY_FORCERVOCHECKONFAILEDREPRO = "ForceRvoCheckOnFailedRepro";
        private const string CONFIGKEY_CREATEBIRTHMOVEMENTS = "CreateBirthMovements";
        private const string CONFIGKEY_SANITELCHECKALLMOVEMENTSFORDELETION = "SanitelCheckAllMovementsForDeletion";

        private const bool DEFAULT_CONFIGVALUE_INRFORCECHECKALLPRESENTANIMALS = false;
        private const bool DEFAULT_CONFIGVALUE_CONFIGKEY_CHECKANDFORCEANIMALSWITHOUTHBIRTHMOVEMENT = false;
        private const bool  DEFAULT_CONFIGVALUE_CONFIGKEY_FORCERVOCHECKONFAILEDREPRO = false;
        private const bool DEFAULT_CONFIGVALUE_CONFIGKEY_CREATEBIRTHMOVEMENTS = false;
        private const bool DEFAULT_CONFIGVALUE_CONFIGKEY_SANITELCHECKALLMOVEMENTSFORDELETION = false;

        private const string CONFIGKEY_WARNDUPLICATEMOVEMENTS = "InRWarnOnDuplicateMovemetns";
        private const string CONFIGKEY_LOGGINGSENDMAIL = "LoggingSendMail";
        private const string CONFIGKEY_LOGCRVSOAPDATA = "LogCRVSoapData";
        private const string CONFIGKEY_USECRDTESTSERVER = "UseCRDTestserver";
        private const string CONFIGKEY_CRVVERWIJDERHERSTELDEGEBROORTES = "CrvVerwijderHersteldeGeboortes";
        private const string CONFIGKEY_CRVHERSTELGEBOORTESNADAGEN = "CrvHerstelGeboortesNaDagen";

        private const string CONFIGKEY_MAXNUMBEROFTHREADS = "MaxNumberOfThreads";

        private const string CONFIGKEY_MINIMALE_DRAAGTIJD_VERWERPEN = "MinimaleDraagtijdVerwerpen";

        private const string CONFIGKEY_MINMOVEMENTDAYSBEFOREDELETE = "MinMovementDaysBeforeDelete";

        private const string CONFIGKEY_EXTRAUBNLOGGING = "ExtraUbnLogging";
        private const bool DEFAULT_CONFIGVALUE_EXTRAUBNLOGGING = false;

        private const string CONFIGKEY_DHZ_XML_LOG = "DhzXmlLog";
        private const bool DEFAULT_CONFIGVALUE_DHZ_XML_LOG = false;

        private const string CONFIGKEY_DHZ_MAX_AGE_DAYS = "DhzMaxAgeDays";
        private const int DEFAULT_CONFIGVALUE_DHZ_MAX_AGE_DAYS = 100;

        private const string CONFIGKEY_USEEVENTCACHE = "UseEventCache";
        private const string CONFIGKEY_DEBUGEVENTCACHE = "DebugEventCache";

        private const string CONFIGKEY_SKIPINRCHECK = "SkipInRCheck";

        private const string CONFIGKEY_INRMAXCALLERRORS = "InRMaxCallErrors";
        private const int DEFAULT_CONFIGVALUE_INRMAXCALLERRORS = 5;

        private const string CONFIGKEY_SANITELMAXREQUESTSIZE = "SanitelMaxRequestSize";
        private const int DEFAULT_CONFIGVALUE_SANITELMAXREQUESTSIZE = 10;

        private const bool DEFAULT_CONFIGVALUE_RUNPARRALEL = false;
        private const bool DEFAULT_CONFIGVALUE_CRV_Missing_Data_Task_Get_Additional_Info = false;
        private const int DEFAULT_CONFIGVALUE_MAXNUMBEROFTHREADS = 32;

        private const bool DEFAULT_CONFIGVALUE_INRCORRIGEERWORPMOEDER = true;

        private const bool DEFAULT_CONFIGVALUE_INRUPDATEVERIFIEDANIMAL = true;
        private const bool DEFAULT_CONFIGVALUE_INRUSEVERIFIEDANIMAL = false;
        private const int DEFAULT_CONFIGVALUE_INRMAXAGEVERIFIEDANIMALMINUTES = 1440; //1 Dag

        private const bool DEFAULT_CONFIGVALUE_LOGGINGSENDMAIL = true;
        private const bool DEFAULT_CONFIGVALUE_LOGCRVSOAPDATA = true;

        private const int DEFAULT_CONFIGVALUE_MINIMALE_DRAAGTIJD_VERWERPEN = 210;
        private const int DEFAULT_CONFIGVALUE_MINMOVEMENTDAYSBEFOREDELETE = 10;

        private const bool DEFAULT_CONFIGVALUE_CRVVERWIJDERHERSTELDEGEBROORTES = true;
        private const int DEFAULT_CONFIGVALUE_CRVHERSTELGEBOORTESNADAGEN = 7;


        private const bool DEFAULT_CONFIGVALUE_USEEVENTCACHE = false;
        private const bool DEFAULT_CONFIGVALUE_DEBUGEVENTCACHE = false;

        private const bool DEFAULT_CONFIGVALUE_SKIPINRCHECK = false;

        public const string IRMissingMovements = "IRMissingMovements";
        public const string LogMailHost = "smtp.vsm-hosting.nl";

        public const string IRverblijfplaatsenTask_csvLanden = "151";
        public const string IRverblijfplaatsenTask_maxMovdateInterval = "30 DAY";
        public const string IRverblijfplaatsenTask_maxBirthdateInterval = "20 YEAR";

        private const string CONFIGKEY_GEADROOGZETMAXINSEMINDAYS = "GeaDroogzetMaxInseminDays";
        private const string CONFIGKEY_GEADROOGZETMISSINGEMMDAYS = "GeaDroogzetMissingEmmDays";

        private const string CONFIGKEY_GEADROOGZETMINIMAALDAGENVOORVERWACHTEAFKALFDATUM = "GeaDroogzetMinimaalDagenVoorVerwachteAfkalfDatum";

        private const int DEFAULT_CONFIGVALUE_GEADROOGZETMAXINSEMINDAYS = 230;
        private const int DEFAULT_CONFIGVALUE_GEADROOGZETMISSINGEMMDAYS = 7;
        private const int DEFAULT_CONFIGVALUE_GEADROOGZETMINIMAALDAGENVOORVERWACHTEAFKALFDATUM = 30;

        public const int CONST_SETTING_FICTIEVEDROOGZET_MAX_DAYS_INSEMIN = 230;

        private const string DEFAULT_CONFIGVALUE_FacadeUser = "FacadeUser";
        private const string DEFAULT_CONFIGVALUE_FacadePassword = "FacadePassword";
        private const string DEFAULT_CONFIGVALUE_MasterTableName = "MasterTableName";
        private const string DEFAULT_CONFIGVALUE_MasterHost = "MasterHost";
        private const string DEFAULT_CONFIGVALUE_MasterUser = "MasterUser";
        private const string DEFAULT_CONFIGVALUE_MasterPassword = "MasterPassword";
        private const string DEFAULT_CONFIGVALUE_SlaveTableName = "SlaveTableName";
        private const string DEFAULT_CONFIGVALUE_SlaveHost = "SlaveHost";
        private const string DEFAULT_CONFIGVALUE_SlaveUser = "SlaveUser";
        private const string DEFAULT_CONFIGVALUE_SlavePassword = "SlavePassword";

        private const int DEFAULT_CONFIGVALUE_REPORTCALFLESSBIRTHASBORNDEADAFTERDAYS = 14;


        public static string FacadeUser
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_FacadeUser];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }

        public static string FacadePassword
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_FacadePassword];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }


        public static string MasterTableName
        {
            get
            {
                return (System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_MasterTableName]);
            }
        }

        public static string MasterHost
        {
            get
            {
                return (System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_MasterHost]);
            }
        }

        public static string MasterUser
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_MasterUser];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }


        public static string MasterPassword
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_MasterPassword];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }


        public static string SlaveTableName
        {
            get
            {
                return (System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_SlaveTableName]);
            }
        }

        public static string SlaveHost
        {
            get
            {
                return (System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_SlaveHost]);
            }
        }

        public static string SlaveUser
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_SlaveUser];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }

        public static string SlavePassword
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[DEFAULT_CONFIGVALUE_SlavePassword];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }

        //Gebruikt tijdens i&r controle
        public const bool DEFAULT_CONFIGVALUE_INRWARNDUPLICATEMOVEMENTS = false;
        private static bool? _InRWarnDuplicateMovements = null;
        public static bool InRWarnDuplicateMovements
        {
            get
            {
                if (!_InRWarnDuplicateMovements.HasValue)
                {
                    string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_WARNDUPLICATEMOVEMENTS];
                    if (string.IsNullOrWhiteSpace(val))
                        _InRWarnDuplicateMovements = DEFAULT_CONFIGVALUE_INRWARNDUPLICATEMOVEMENTS;
                    else
                        _InRWarnDuplicateMovements = val.ToUpperInvariant().Equals("TRUE");
                }

                return _InRWarnDuplicateMovements.Value;
            }
        }

        /// <summary>
        /// Maak gebruik van RVO test server
        /// </summary>
        public static bool UseLNVTestServer
        {
            get
            {
                return (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UseLNVTestserver"]) == 1);
            }
        }

        /// <summary>
        /// RVO gebruikersnaam
        /// </summary>
        public static string LNVUsername
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string userName = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_LNVUSERNAME];
                return Decrypt.Decodeer_PW(userName) ?? "";
            }
        }

        /// <summary>
        /// RVO wachtwoord
        /// </summary>
        public static string LNVPassword
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_LNVPASSWORD];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }

        /// <summary>
        /// Santitel wachtwoord
        /// </summary>
        public static string SanitelUsername
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string userName = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_SANITELUSERNAME];
                return Decrypt.Decodeer_PW(userName) ?? "";
            }
        }

        /// <summary>
        /// Sanitel wachtwoord
        /// </summary>
        public static string SanitelPassword
        {
            get
            {
                configDecrypter Decrypt = new configDecrypter();
                string password = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_SANITELPASSWORD];
                return Decrypt.Decodeer_PW(password) ?? "";
            }
        }

        /// <summary>
        /// Parrallel.Foreach ipv Foreach gebruik in main loop
        /// </summary>
        public static bool RunParralel
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_RUNPARRALEL];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_RUNPARRALEL;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        public static bool CRV_Missing_Data_Task_Get_Additional_Info
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_CRV_Missing_Data_Task_Get_Additional_Info];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_CRV_Missing_Data_Task_Get_Additional_Info;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }


        /// <summary>
        /// Max threads voor runParralel, normaal = aantal cores
        /// </summary>
        public static int MaxNumberOfThreads
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_MAXNUMBEROFTHREADS];
                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_MAXNUMBEROFTHREADS;
                else
                    return i;
            }
        }

        /// <summary>
        /// Gebruik CRD Test server
        /// </summary>
        public static int CRDReproductionTestserver
        {
            get
            {
                int iUseTestServer = 0;
                var sUseTestServer = System.Configuration.ConfigurationManager.AppSettings["CRDReproductionTestserver"];

                if (string.IsNullOrWhiteSpace(sUseTestServer) || !Int32.TryParse(sUseTestServer, out iUseTestServer))
                    return 0;

                return iUseTestServer;
            }
        }

        /// <summary>
        /// Gebruikt voor o.a.? EMM gebruik test server
        /// </summary>
        public static bool UseCRDTestserver
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_USECRDTESTSERVER];
                if (string.IsNullOrWhiteSpace(val))
                    throw new ConfigurationErrorsException($"Configkey '{CONFIGKEY_USECRDTESTSERVER}' not found in config.");
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }




        /// <summary>
        /// Corrigeer worpen van moeder via I&R werkt vertragend.
        /// </summary>
        public static bool InRCorrigeerWorpMoeder
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_INRCORRIGEERWORPMOEDER];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_INRCORRIGEERWORPMOEDER;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        /// <summary>
        /// Gebruik de agrodata.VERIFIED_ANIMAL en VERIFIED_MOVEMENT tabellen om calls naar INR organisaties te verminderen
        /// </summary>
        public static bool InRUseVerifiedAnimal
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_INRUSEVERIFIEDANIMAL];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_INRUSEVERIFIEDANIMAL;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        /// <summary>
        /// Tijdens INR controle maximum age van record in minuten dat een record oud mag zijn om toch te gebruiken zonder opnieuw op te halen
        /// </summary>
        public static int InRMaxAgeVerifiedAnimalMinutes
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_INRMAXAGEVERIFIEDANIMALMINUTES];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_INRMAXAGEVERIFIEDANIMALMINUTES;
                else
                    return i;
            }
        }

        /// <summary>
        /// Korter of gelijk dan.
        /// </summary>
        public static int VerwerpenIndienDraagtijdKorterDan
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_MINIMALE_DRAAGTIJD_VERWERPEN];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_MINIMALE_DRAAGTIJD_VERWERPEN;
                else
                    return i;
            }
        }

        /// <summary>
        /// Send log mail.
        /// </summary>
        public static bool LogginSendMail
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_LOGGINGSENDMAIL];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_LOGGINGSENDMAIL;
                else
                    return !val.ToUpperInvariant().Equals("FALSE");
            }
        }


        /// <summary>
        /// Log SOAP requests.
        /// </summary>
        public static bool LogCRVSoapData
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_LOGCRVSOAPDATA];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_LOGCRVSOAPDATA;
                else
                    return !val.ToUpperInvariant().Equals("FALSE");
            }
        }




        /// <summary>
        /// Hoeveel dagen movement gelden moet zijn voordat hij eventueel verwijderd mag worden
        /// </summary>
        public static int MinMovementDaysBeforeDelete
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_MINMOVEMENTDAYSBEFOREDELETE];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_MINMOVEMENTDAYSBEFOREDELETE;
                else
                    return i;
            }
        }


        public static bool CrvVerwijderHersteldeGeboortes
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_CRVVERWIJDERHERSTELDEGEBROORTES];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_CRVVERWIJDERHERSTELDEGEBROORTES;
                else
                    return !val.ToUpperInvariant().Equals("FALSE");
            }
        }

        /// <summary>
        /// Aantal dagen geleden dat een geboortemelding na nu moet zijn om verwijderd te mogen worden indien niet aanwezig in VM
        /// </summary>
        public static int CrvHerstelGeboortesNaDagen
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_CRVHERSTELGEBOORTESNADAGEN];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_CRVHERSTELGEBOORTESNADAGEN;
                else
                    return i;
            }
        }

        /// <summary>
        /// Gebruik maken van EVENT cache tabel, Zou gebruikt moeten worden in live situaties IVM snelheid.
        /// </summary>
        public static bool UseEventCache
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_USEEVENTCACHE];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_USEEVENTCACHE;
                else
                    return !val.ToUpperInvariant().Equals("FALSE");
            }
        }

        /// <summary>
        /// Extra optie om debugging van de EventCache aan te zetten, hij vergelijkt dan op plekken hoe het verschillend zou zijn zonder event cache.
        /// </summary>
        public static bool DebugEventCache
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_DEBUGEVENTCACHE];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_DEBUGEVENTCACHE;
                else
                    return !val.ToUpperInvariant().Equals("FALSE");
            }
        }

        /// <summary>
        /// I&R Check overslaan.
        /// </summary>
        public static bool SkipInRCheck
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_SKIPINRCHECK];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_SKIPINRCHECK;
                else
                    return !val.ToUpperInvariant().Equals("FALSE");
            }
        }

        /// <summary>
        /// Indien er meer errors dan dit in VERIFIED_ANIMAL_CALL bij de I&R organisatie, dan niet meer controleren, is dan een 'error' dier.
        /// </summary>
        public static int InRMaxCallErrors
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_INRMAXCALLERRORS];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_INRMAXCALLERRORS;
                else
                    return i;
            }
        }

        /// <summary>
        /// Wanneer een klant een GEA heeft, en EMMSoap aan, een processerver die EMM verstuurd en al 'GeaDroogzetMissingEmmDays' geen EMM meer heeft, 
        /// al 'GeaDroogzetMinimaalDagenNaVerwachteAfkalfDatum' geleden verwacht afgekalfd zou zijn
        /// en meer dan 'GeaDroogzetMaxInseminDays' geleden geinsemineerd, berekende droogzet toevoegen.
        /// </summary>
        public static int GeaDroogzetMaxInseminDays
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_GEADROOGZETMAXINSEMINDAYS];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_GEADROOGZETMAXINSEMINDAYS;
                else
                    return i;
            }
        }

        /// <summary>
        /// Indien er eventueel een berekende droogzet voor GEA aangemaakt word moet er minstens dit aantal dagen geen EMM meer binnen zijn gekomen.
        /// </summary>
        public static int GeaDroogzetMissingEmmDays
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_GEADROOGZETMISSINGEMMDAYS];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_GEADROOGZETMISSINGEMMDAYS;
                else
                    return i;
            }
        }

        /// <summary>
        /// Alleen droogzetten indien verwachte afkalfdatum met dit aantal dagen is overschreden.
        /// </summary>
        public static int GeaDroogzetMinimaalDagenVoorVerwachteAfkalfDatum
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_GEADROOGZETMINIMAALDAGENVOORVERWACHTEAFKALFDATUM];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_GEADROOGZETMINIMAALDAGENVOORVERWACHTEAFKALFDATUM;
                else
                    return i;
            }
        }

        /// <summary>
        /// Forceer Alle aanwezige dieren te controleren ipv alleen dieren waarvan laatste movement + fokker + aanwezigheid niet klopt
        /// </summary>
        public static bool InRForceCheckAllPresentAnimals
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_INRFORCECHECKALLPRESENTANIMALS];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_INRFORCECHECKALLPRESENTANIMALS;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        /// <summary>
        /// Gets a value indicating whether [check and force animals withouth birth movement].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [check and force animals withouth birth movement]; otherwise, <c>false</c>.
        /// </value>
        public static bool CheckAndForceAnimalsWithouthBirthMovement
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_CHECKANDFORCEANIMALSWITHOUTHBIRTHMOVEMENT];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_CONFIGKEY_CHECKANDFORCEANIMALSWITHOUTHBIRTHMOVEMENT;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
            
        }

        public static bool ForceRvoCheckOnFailedRepro
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_FORCERVOCHECKONFAILEDREPRO];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_CONFIGKEY_FORCERVOCHECKONFAILEDREPRO;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        public static bool CreateBirthMovements
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_CREATEBIRTHMOVEMENTS];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_CONFIGKEY_CREATEBIRTHMOVEMENTS;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        public static bool SanitelCheckAllMovementsForDeletion
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_SANITELCHECKALLMOVEMENTSFORDELETION];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_CONFIGKEY_SANITELCHECKALLMOVEMENTSFORDELETION;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int SanitelMaxRequestSize
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_SANITELMAXREQUESTSIZE];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_SANITELMAXREQUESTSIZE;
                else
                    return i;
            }
        }

        public static bool ExtraUbnLogging
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_EXTRAUBNLOGGING];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_EXTRAUBNLOGGING;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        public static bool DhzXmlLog
        {
            get
            {
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_DHZ_XML_LOG];
                if (string.IsNullOrWhiteSpace(val))
                    return DEFAULT_CONFIGVALUE_DHZ_XML_LOG;
                else
                    return val.ToUpperInvariant().Equals("TRUE");
            }
        }

        /// <summary>
        /// Gets or sets the DHZ maximum age in days of DHZ.ts to determine if the DHZ should be reported.
        /// </summary>
        public static int DhzMaxAgeDays
        {
            get
            {
                int i;
                string val = System.Configuration.ConfigurationManager.AppSettings[CONFIGKEY_DHZ_MAX_AGE_DAYS];

                if (string.IsNullOrWhiteSpace(val) || !int.TryParse(val, out i))
                    return DEFAULT_CONFIGVALUE_DHZ_MAX_AGE_DAYS;
                else
                    return i;
            }
}
/// <summary>
/// Bij geboorte in reproLastParity (stap 2) waar er geen kalf voor bekend is in de herdPedigree (stap 1). Deze kalveren
/// kunnen later gemeld worden wanneer er na X dagen nog geen calf bij de BIRTH is te komen hangen, zet het record dan op BornDead
/// </summary>
public static int ReportCalflessBirthAsBornDeadAfterDays
        {            
            get
            {
                int i;
                string s = ConfigurationManager.AppSettings[CONFIGKEY_REPORTCALFLESSBIRTHASBORNDEADAFTERDAYS];

                if (string.IsNullOrWhiteSpace(s) || !Int32.TryParse(s, out i))
                    return DEFAULT_CONFIGVALUE_REPORTCALFLESSBIRTHASBORNDEADAFTERDAYS;

                return i;
            }
        }

        /// <summary>
        /// Test functie data encrhypten.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string CryptString(string msg)
        {
            return new configDecrypter().Codeer_PW(msg);
        }

    }

    class configDecrypter : VSM.RUMA.CORE.COMMONS.MaskingClass
    {
        public configDecrypter() : base("")
        {

        }

        internal string Decodeer_PW(string Tekst)
        {
            return base64Decode(Tekst);
        }

        internal string Codeer_PW(string Tekst)
        {
            return base64Encode(Tekst);
        }

    }
}
