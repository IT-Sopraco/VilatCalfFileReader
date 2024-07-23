using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.DB
{
    public class LABELSConst
    {
        public enum Exterieurwaarde_Tes
        {
            Kop = 2,
            Ontwikkeling = 3,
            Bespiering = 4,
            Evenredigheid = 5,
            Type = 6,
            Beenwerk = 7,
            Vacht = 8,
            Algemeen_voorkomen = 9,
            Lengte = 11,
            Hoogte = 12,
            Borstdiepte = 14,
        }

        public enum DNA
        {
            AA_Type = 1,
            AfwijkingID = 999,
        }

        public enum RemarkLabKind
        {
            hok = 402,
            afdeling = 401,
            stal = 400
        }

        public enum SaleKind
        {
            Normal = 0,
            Export = 1,
            Slaughter = 2,
            EmergencySlaughter = 3
        }

        public enum SalKeuringKalf
        {
            afkeur = 1,
            doodaangevoerd = 2,
            doodinstal = 3
        }
        //Zie G:\agrobase\Documentatie_algemeen\Database.doc
        //en  G:\agrobase\Documentatie_algemeen\AllerleiAgrobase.txt


        public enum labKind
        {
            //LabKind constanten; houd deze up-to-date met rdbtools (Delphi)
            //(Z:\prog\DELPHI\RUMAMYSQLDB\rdbtools.pas)

            MOVKIND = 1,
            EVEKIND = 2,
            ANISEX = 4,

            FOKWAARDEN_N_Bovenbalk = 12,
            FOKWAARDEN_N_Productie = 13,
            FOKWAARDEN_N_Onderbalk = 14,
            FOKWAARDEN_N_Overige = 15,
            FOKWAARDEN_N_Levensvatbaarheid = 16,
            HEATCERTAINTY = 45,
            EXTERIEUR_N_Boven = 67,
            EXTERIEUR_N_Onder = 68,

            EXTERIEUR_EXTTYPE_SHORT = 89,
            FOKWAARDEN_FWWTYPE_SHORT = 91,

            FOKWAARDEN_KINDOFVALUE_BOVEN = 202, //OUD
            FOKWAARDEN_KINDOFVALUE_ONDER = 203, //OUD
            EXTERIEUR_WAARDEN_EXTTYPE = 203,    //OUD
            EXTERIEUR_WAARDEN_EXTTYPE_LINEAR = 6700,
            EXTERIEUR_WAARDEN_EXTTYPE_LINEAR_VAN = 6701,
            EXTERIEUR_WAARDEN_EXTTYPE_LINEAR_TOT = 6702,
            HAARKLEUR = 9,
            HAARKLEUR_BELG = 65,//Als het programma door een ingelogde belg wordt gebruikt 
            HAARKLEUR_Value = 108,//BUG 1986
            HAARKLEUR_Text = 107,//BUG 1986
            AFTEKENING = 94,
            THKDESCRIPTIONID = 131,
            DIER_ZIEKTE = 133,
            MYOMAX = 146,
            AFWIJKINGEN = 150,
            ANICATEGORY = 200,
            ANISTATUS = 201,
            RAISEDBY = 32,
            ANISCRAPI = 204,
            SECONRACRAC_LONG = 205,
            SECONRACRAC_SHORT = 206,
            EXTERIEUR_EXTKIND = 207,
            FOKWAARDEN_FWKIND = 207,
            TOEDIENINGSWIJZE = 210,
            EENHEDEN = 211,
            ANIPREDIKAAT_LONG = 215,
            ANIPREDIKAAT_SHORT = 216,

            BOOLEAN_JA_NEE = 217,
            BOOLEAN_WEL_NIET = 218,

            BLINDFACTOR = 111,

            BEDRIJF_ZIEKTE = 250,
            BEDRIJF_ZIEKTE_STATUS = 251,
            DIER_ZIEKTE_STATUS = 252,

            MINERAAL_DIERSOORT = 259,
            MINERAAL_DIERCATEGORIE = 260,
            MINERAAL_STALSYSTEEM = 261,
            ANIMINAS = 260, //OUD           

            SALREASON = 2000,
            PROGRAMID = 9100,

            ZIEKTE_RUND = 10,//oude labels waardes: Laten staan want REMARKS maakt hier gebruik van
            ZIEKTE_SCHAAP = 39,
            ZIEKTE_GEIT = 66,
            ZIEKTESUBSTART_RUND = 10,
            ZIEKTESUBSTART_SCHAAP = 11,
            ZIEKTESUBSTART_GEIT = 12,
            BEHANDELING = 92,

            ZIEKTE = 10,    //agro-labels waarde BUG 1215 LAbKind  SubZiekte = 10 + ZIEKTE LabId met voorloopnul dus:1001 1002 etc

            DRACHTSTATUS = 53,
            GEBOORTEVERLOOP = 109,
            GEBOORTELIGGING = 30,
            SCHAAPPREDIKAAT = 90,

            STATUS = 81,

            DIERKAART_TABBLADEN = 6000,
            LIJSTEN_SOORT = 6100,
            LIJSTEN = 6200,
            LABEL_TEKSTEN = 6300,
            ANISPECIES = 550,

            TEGEL_WEERGAVEN = 2100
        };

        /* 
         * Labkind 133
         * incompleet
         */
        public enum DierkZiekte
        {
            Scrapie = -8
        };

        public enum labIds
        {
            BEDRIJF_ZIEKTE_Zwoegerziekte = 1,
            BEDRIJF_ZIEKTE_Scrapie = 2,
            BEDRIJF_ZIEKTE_STATUS_Onbekend = 0,
            BEDRIJF_ZIEKTE_STATUS_Vrij = 1,
            BEDRIJF_ZIEKTE_STATUS_Nietgecertificeerd = 2,
            BEDRIJF_ZIEKTE_STATUS_Inonderzoek = 3,
            BEDRIJF_ZIEKTE_STATUS_Nietvrij = 4,
            BEDRIJF_ZIEKTE_STATUS_InObservatie = 5,
            BEDRIJF_ZIEKTE_STATUS_StatusbijGD = 6,
            DIER_ZIEKTE_STATUS_Onbekend = 0,
            DIER_ZIEKTE_STATUS_Nietaangetoond = 1,
            DIER_ZIEKTE_STATUS_Aangetoond = 2,
            BEDRIJF_ZIEKTE_CAE = 4,
            BEDRIJF_ZIEKTE_CL = 5
        };

        public enum relatieKind
        {
            INSEMINATOR = 1,
            DIERENARTS = 2,
            DESTRUCTOR = 3,
            VEEHANDELAAR = 4,
            KIORGANISATIE = 5,
            VOERFABRIKANTHANDEL = 6,
            TRANSPORTEUR = 7,
            MEDEWERKER = 8,
            VEEHOURDER = 9,
            SLACHTHUIS = 10,
            SCHAPENHOUDER = 11,
            KALVERHOUDER = 12,
            LOONWERKER = 13,
            VLEESVARKENSHOUDER = 14,
            ZEUGENHOUDER = 15,
            MEDICIJN_PRODUCENT = 16,
            MEDICIJN_LEVERANCIER = 17,
            INSPECTEUR = 50,
            CHIPPER = 55,
            HONDENFOKKER = 56,
            HONDENEIGENAAR = 57,
            MELKMACHINE_DEALER = 201,
            MELKMACHINE_MONTEUR = 202,
            KOM_MEDEWERKER = 211,
            KOM_ADMIN = 212
        };

        public enum EventKind
        {
            TOCHTIG = 1,
            INSEMINEREN = 2,
            DRACHTIGHEID = 3,
            DROOGZETTEN = 4,
            AFKALVEN = 5,
            BEHANDELING = 6,
            ZIEKTE = 7,
            SPOELEN_EMBRYO = 8,
            INPLANTEREN = 9,
            SPENEN = 10,
            BLOEDONDERZOEK = 11,
            SAMENWEIDEN = 12,
            STATUS = 13,
            CONTACTMELDING = 14,
            CHIPPEN = 15
        };

        public enum MovementKind
        {
            AANVOER = 1,
            AFVOER = 2,
            DOOD = 3,
            HUREN = 4,
            EINDEHUUR = 5,
            VERHUREN = 6,
            EINDEVERHUREN = 7,
            LOKATIE = 8,
            VERMISSING = 9,
            GEVONDEN = 10,
            ADRESWIJZIGING = 11
        };

        public enum CodeMutation
        {
            AANVOER = 1,
            GEBOORTE = 2,
            DOODGEBOREN = 3,
            AFVOER = 4,
            IKBAFVOER = 5,
            DOOD = 6,
            IMPORT = 7,
            AANAFVOER = 8,
            EXPORT = 9,
            SLACHT = 10,
            INSCHAREN = 11,
            UITSCHAREN = 12,
            NOODSLACHT = 13,
            QKrtsvacc2 = 17,
            QKrtsvaccH = 18,
            OMNUMMEREN = 19,
            VERMISSING = 20,
            GEVONDEN = 21,
            CONTACTMELDING = 22,
            ADRESWIJZIGING = 23
            //INTREKKING = 24   is totaal NIET 24 zie: Agrolabels 102

        };
        /*
          case 20:
                    return 9;//vermissing MOVEMENT
                case 21:
                    return 10;//gevonden MOVEMENT
                case 28:
                    return 0;//adreswijziging ??????NOG ONBEKEND
                case 22:
                    return 14;//contactmelding EVENT
              
         */
        public enum insKind
        {
            INSEMINATIE = 1,
            NATUURLIJKE_DEKKING = 2,
            DHZ_KI_BEVROREN_RIETJE = 4,
            DHZ_KI_VERS_RIETJE = 6
        };

        public enum gesStatus
        {
            DRACHTIG_NA_ONDERZOEK = 1,
            DRACHTIG_VERKLAARD = 2,
            NIET_DRACHTIG = 3,
            BLIJFT_GUST = 4,
            AFMESTEN = 5,
            VETWEIDEN = 6,
            TWIJFELACHTIG = 7
        };

        public enum Blood
        {
            BLOODLABKIND = 2,
            SOORT = 110,
            RESULTAAT = 111,
            REDEN = 112
        };

        public enum programId
        {
            RUMA_RUNDVEE = 1,
            LELY = 2,
            CRDELTA = 53,
            GEA = 3600,
            DELAVAL = 3700,
            VELOS = 3800,
            SAC = 3900,
            DAIRYMASTER = 4000,
            BOUMATIC = 4100,
            GM = 4200,
            MSOptimaBox = 16000,


            FORFARMERS = 90,
            VWA = 900,
            VWAUBN = 901,
            FOKDAG = 902,
            HetDeensSysteem = 13000,

            RUMA_RUNDVEE_JUNIOR = 3,
            RUMA_STIERENMANAGEMENT_JR = 4,
            RUMA_STIERENMANAGEMENT_SR = 5,
            BELEXPERT_SENIOR = 6,
            EGAM_GEITENMANAGEMENT = 7,
            RUMA_WITVLEES_SENIOR = 8,
            FORFARMERS_ROSE = 9,
            RUMA_InR = 10,
            RUMA_InRDHZKI = 11,
            RUMA_ZOOGKOEIEN = 12,
            RUMA_JONGVEE = 13,
            BELEXPERT_JUNIOR = 14,
            KALVEREN_BEDRIJFSREGISTER = 15,
            BELTES = 16,
            BELEXPERT_BASIS = 17,
            KERRY_HILL = 18,
            RUMA_WITVLEES_JUNIOR = 19,
            RUMA_ROSE = 20,
            RUMA_ZOOGKOE_HUUR = 21,
            NSFO = 22,
            PALIGROUP = 23,
            NTS_NEDERLANDS_TEXELS_SCHAPENSTAMBOEK = 24,
            TSNH_TEXELS_SCHAPENSTAMBOEK_IN_NOORD_HOLLAND = 25,
            NH_SCHAPENFOKVERENIGING_DE_NOORD_HOLLANDER = 26,
            LAX_LUXEMBURGER_SCHAFERGENOSSENSCHAFT = 27,
            BDM_BLEU_DU_MAINE_RASVERENIGING = 28,
            CF_CLUN_FOREST_SCHAPENVERENIGING = 29,
            HD_HAMPSHIRE_DOWN_RASVERENIGING = 30,
            RY_REYLAND_RASVERENIGING = 31,
            SOAY_SOAY_RASVERENIGING = 32,
            BL_BLESSUM = 33,
            RUI_RUISCHAAP = 34,
            DK = 35,
            NSFO_SCHAAP = 39,
            NSFO_GEIT = 40,
            _ONBEKEND_STAMBOEK_GEIT = 47,
            _ONBEKEND_STAMBOEK_SCHAAP = 49,
            VAN_VEEN = 50,
            NFS_ADMIN = 51,
            NFS = 52,
            SUFFOLK = 54,
            SWIFTER = 55,
            ADV_MELKKALF = 61,
            ADV_ROOD = 62,
            ADV_SCHAAP = 63,
            ADV_WEIDEZOOG = 64,
            ADV_GEIT = 65,
            ADV_WIT = 66,
            ADV_ROSE = 67,
            NAVOBI_ADMIN = 80,
            VILATCA_ADMIN = 81,
            FORFARMERS_VERTEGENWOORDIGER = 91,
            EGAM_ADMIN = 100,
            EGAM_QLIP = 101,
            ENBASIS = 102,
            ENMANAGEMENT = 103,
            _ELDA_GERESERVEERD = 105,
            ENADMINISTRATOR = 109,
            ELDA_VLEESLAM = 115,
            BELTES_ADMIN = 160,
            VOERLEVERANCIER = 200,
            NSFO_DAP = 220,
            PALIGROUP_ADMIN = 230,
            NEDAP_LIVEID = 300,
            NEDAP_LIVEID_GEIT = 301,
            NAVOBI_WITVLEES = 800,
            VILATCA_WITVLEES = 810,
            VBK_PARTICULIER = 2500,
            VIRBAC_PARTICULIER = 2501,
            VIRBAC_ASIEL = 2521,
            VBK_FOKKERCHIPPER_LID_VBK = 2550,
            VBK_FOKKERCHIPPER_GEEN_LID_VBK = 2551,
            VBK_CHIPPER_DAP = 2570,
            VIRBAC_CHIPPER = 2571,
            VIRBAC_ADMINISTRATOR = 2598,
            VBK_ADMINISTRATOR = 2599,
            FULLWOOD = 3500,
            FULLWOOD_ADMIN = 3599,
            GEA_ADMIN = 3699,
            DELAVAL_ADMIN = 3799,
            VELOSADMIN = 3899,
            SACADMIN = 3999,
            DAIRYMASTERADMIN = 4099,
            BOUMATICADMIN = 4199,
            GMADMIN = 4299,
            BELEXPERT_EZI = 10000,
            DATAMARS_COMMUNICATOR = 10001,
            BELEXPERT_EZI_GEIT = 10050,
            VVB = 11000,
            VVB_ADMIN = 11099,
            FHRS = 12000,
            FHRSADMIN = 12099,
            DEENSSYSTEEM_ADMIN = 13099,
            BETERVEE = 14000,
            BETERVEE_ADMIN = 14099,
            VIB = 15000,
            VIB_ADMIN = 15099,
            MS_OPTIMA_BOX = 16000,
            MS_OPTIMA_BOX_ADMIN = 16099,
            NIJLAND = 17000,
            NIJLAND_ADMIN = 17099,
            AGRIFIRM = 18000,
            AGRIFIRM_ADMIN = 18099,
            NOG = 19000,
            NOG_ADMIN = 19099,
            HEEMSKERK = 20000,
            NTS_ADMIN_NSFO = 22024,
            TSNH_ADMIN_NSFO = 22025,
            NH_ADMIN_NSFO = 22026,
            LAX_ADMIN_NSFO = 22027,
            BM_ADMIN_NSFO = 22028,
            CF_ADMIN_NSFO = 22029,
            HD_ADMIN_NSFO = 22030,
            RY_ADMIN_NSFO = 22031,
            SOAY_ADMIN_NSFO = 22032,
            KOM = 30000,
            VETWERK = 31000,
            VALACON = 32000,
            DLV = 33000,
            DLVADMIN = 33099,
            ARLA = 34000,
            ARLAADMIN = 34099,
            VSM_ADMIN = 99999,

        };

        public enum factSoort
        {
            INKOOP = 0,
            VERKOOP = 1
        };

        public enum TYPEID
        {
            ANIMAL = 1,
            EVENT = 2,
            MOVEMENT = 3,
            AFWIJKING = 4,
            SUPPLY1 = 5,
        }

        public enum HeatCertainty
        {
            NVT = 0,
            INDICATIE = 1,
            VERDENKING = 2,
            NIET_TOCHTIG = 11
        }

        public enum BirthCourse
        {
            VLOT = 1,
            NORMAAL = 2,
            ZWAAR = 3,
            KEIZERSNEDE = 4,
            AFGEZAAGD = 5,
            ANDERE_HULP = 6
        }

        public enum GUI_EXTERNAL_KIND
        {
            NOT_SET = 0,
            MEDI_RUND = 1
        }



        /*
         * te gebruikern met UbnPropertyKey.propertyname.ToString() als key voor agrofactuur.UBN_PROPERTY tabel
         */
        public enum UbnPropertyKey
        {
            boerenbondGebruikersnaam,
            boerenbondWachtwoord,
            boerenbondKlantnummer,
            boerenbondLaatsteSequenceId,
            ABZDiervoedingGebruikersnaam,
            ABZDiervoedingWachtwoord,
            ABZDiervoedingKlantnummer,
            ABZDiervoedingLaatsteSequenceId,
            DeensSysteemLaatsteK01Timestamp,
            DeensSysteemLaatsteK02Timestamp,

            EdiFactuurKoerhuisGebruikersnaam,
            EdiFactuurKoerhuisWachtwoord,
            EdiFactuurKoerhuisKlantnummer,
            EdiFactuurKoerhuisLaatsteSequenceId,

            EdiFactuurForFarmersKlantnummer,
            EdiFactuurForFarmersLaatsteSequenceId,

            EdiFactuurCAVVZOSGebruikersnaam,
            EdiFactuurCAVVZOSWachtwoord,
            EdiFactuurCAVVZOSKlantnummer,
            EdiFactuurCAVVZOSLaatsteSequenceId,

            EdiFactuurFakkertGebruikersnaam,
            EdiFactuurFakkertWachtwoord,
            EdiFactuurFakkertKlantnummer,
            EdiFactuurFakkertLaatsteSequenceId,

            EdiFactuurWolswinkelGebruikersnaam,
            EdiFactuurWolswinkelWachtwoord,
            EdiFactuurWolswinkelKlantnummer,
            EdiFactuurWolswinkelLaatsteSequenceId,

            agrolink_settings_show_mpr_niet_officieel,

            SanitelFacility,
        }

        public enum TreKindDefault
        {
            VaccinatieOfEnting = 10001,
            Antibiotica = 10002,
            Hormonen = 10003,
            VitaminenEnMineralen = 10004,
            Wormmiddelen = 10005,
            OverigeGeneesmiddelen = 10006,
            Bekappen = 10007,
            Operatie = 10008,
            OpstekenSpeen = 10009,
            Voetbad = 10010,
            Speenstift = 10011,
            Magneet = 10012,
            Droogzetten = 10013,
            Overig = 10014,
        }

        //Voor andere zie
        //G:\agrobase\Documentatie_algemeen\AllerleiAgrobase.txt
        // http://wiki.vsm-hosting.nl/index.php/ChangedBy
        public enum MutationBy
        {
            EDIDAP = 2,
            EDINRS = 3,
            LNVIRMUT = 15,
            LNVStallijst = 23,
            XMLImport = 29,
            LNVVerblijfplaatsen = 107,

            LNVVerblijfplaatsenCorrectie = 123,
            SanitelVerblijfplaatsenCorrectie = 127,
            Rsreproduction2 = 103,
            HetDeensSysteem = 202,      //K02
            AnimalVerifier = 203,

        }

        public enum ChangedBy
        {
            UNKNOWN = 0,
            EDIDAP = 2,
            EDINRS = 3,
            LNVIRMUT = 15,
            LNVStallijst = 23,
            XMLImport = 29,
            LNVVerblijfplaatsen = 107,
            LNVVerblijfplaatsenCorrectie = 123,
            SanitelVerblijfplaatsenCorrectie = 123,

            Rsreproduction2 = 103,
            HetDeensSysteem = 202,      //K02
            AnimalVerifier = 203,
            CRVMissingDataTask2 = 303,
            IRUtils = 304,
            RVOVerblijfplaatsen = 306,
            CRVDataVerzenden = 308,

            CRVUpdateRespondersTask = 309,
            ABSendMPR = 310,

            AbstractEdiFactuur = 320,
            CRVAnimalactivity = 321,
            CRVEmm = 322,
            TaskSend_IR_Meldingen = 324,
            LNVIRmeldingenRaadplegen_DoeIRMeldingenRaadplegen = 1001,
            LNVIRmeldingenRaadplegen_addHerstelMelding = 1002,
            LNVIRmelden = 1003,
            MutationUpdatercheckVervangenLevensnr = 1101,
            MobilemateImport = 4200,
            Mark_Handmatig = 5555,
            ReproTool = 5558,
            ReproToolAnimalFix = 5570,
            OudeAgrobaseAPI = 6600,
            CRVChangedAnimal = 323,
            Elda_EdiCircle = 5005,
            Agrobase_Tes_import = 6006,
            TaskSend_Crv_Dhz_Meldingen = 6620,
            TaskSend_Crv_Animal_Dryoff = 6621,
            TaskSend_Vetwerk_Offective = 6622,
            TaskGet_EdidapVetwerkUniform_Ftp = 6623,
            TaskSend_CRV_Animal_Gestation = 6624,
            TaskGet_CRV_Animal_Responders = 6625,
            TaskSend_EdidapVetwerkUniform_Ftp = 6626,
            TaskSend_Crv_Melding = 6627
        }

        public enum SendTo
        {
            INR = 0,
            FHRS_1 = 1,
            Sanitrace_26 = 26,
            FHRS_27 = 27,
            CRD = 28,
            LNVV2IR = 29,
            Sanitrace_33 = 33,
            IRHond = 35,
            Hitier = 36,
            DCF = 37,
        }

        public enum ReportOrganization
        {
            // FTPAction Tabel
            CRV_FTP_Melkcontrole = 1,
            CRV_IenR_FTP = 2,
            CRV_DHZ_KI = 3,
            Zuivelnet = 4,
            CRV_FTP_Ophalen_logfile = 5,
            VSM_RUMA_VSM_Download = 6,
            VSM_RUMA_VSM_Upload = 7,
            KI_SAMEN_DHZ_KI = 8,
            Agrotel = 9,
            VSM_BBS = 10,
            KI_Kampen_DHZ_KI = 11,
            KI_Altapon_DHZ_KI = 12,
            Geboortekaart = 13,
            IDR_schapen = 14,
            CRV_EMM_FTP = 15,
            Webster = 16,
            KI_ABS_DHZ_KI = 17,
            Ophalen_DHZ_KI = 18,
            Swifter_Geboorteberichten = 19,
            EDI_Versie = 20,
            VSM_RUMA_Installatie_log = 21,
            CRV_FokWaarde_Versturen = 22,
            CRV_FokWaarde_Ophalen = 23,
            Nijland_FTP_Melkcontrole = 24,
            EDI_Zuivel = 25,
            Sanitel = 26,
            FHRS_FTP_Melkcontrole = 27,
            CRV_IenR_WS = 28,
            LNV_IenR_WS = 29,
            EDI_Tellus = 30,
            Reproplus_Westerkwartier_DHZ_KI = 31,
            Reproplus_NW_Friesland_DHZ_KI = 32,
            Sanitrace = 33,
            FHRS_DHZ_KI = 34,
            IenR_Hond = 35,

        }

        public enum IReaderPluginResult
        {
            InvalidFileTypeForPlugin = -99
        }

        public enum whds_Ovarium
        {                       //Deens systeem
            Actief = 1,         //1
            NietActief = 2,     //2
            Follikelcyste = 3,  //3
            LutealeCyste = 4,   //4
            Tochtig = 5,        //5
            Drachtig = 6        //dr
        }

        //Incompleet, staat in label tabel
        //CodeAandoening
        //Rundvee:    labkind=10
        public enum disMainCode
        {
            DeensSysteemTijdelijk = -99
        }

        public enum RUMAprog
        {
            rundvee = 1,
            stieren = 2,
            schapen = 3,
            zoogkoeien = 4,
            geiten = 5,
            witvlees = 6,
            rosekalveren = 7,
            IenRDHZ = 8,
            fokvarken = 20,
            vleesvarken = 21,
            hond = 25,
            kat = 26,
            paard = 30
        }

        public enum AniSex
        {
            Mannelijk = 1,
            Vrouwelijk = 2,
            KweenOfOnbekend = 3
        }

        public enum AnimalSpecies
        {
            Unknown = 0,
            Rund = 1,
            Schaap = 3,
            Geit = 5,
            Kalf = 6,
        }

        public enum MovKind
        {
            Buying = 1,
            Sale = 2,
            Loss = 3
        }

        public enum BVkindOfValueRund
        {
            FWBovenbalk = 1,
            FWMelkproductie = 2,
            FWOnderbalk = 3,
            FWOverigeKoe = 4,
            ExterieurBovenKoe = 5,
            ExterieurOnderKoe = 6,
            FWOverigeStier = 7,
            ExterieurVleesOonder = 8,
            FWlevensvatbaarheid = 9
        }

        public enum ExtSoort
        {
            ExterieurBovenKoe = 67,
            ExterieurOnderKoe = 68,
        }

        public enum BVFieldNumberFWBovenbalkRund
        {
            Betrouwbaarheid = 1,
            Ontwikkeling = 2,
            Frame = 3,
            Uier = 4,
            Benen = 5,
            Bespiering = 6,
            AlgemeenVoorkomen = 7,
            Type = 8,
            Robuustheid = 9,
        }

        public enum BVFieldNumberFWMelkproductieRund
        {
            Betrouwbaarheid = 1,
            Melkhoeveelheid = 2,
            VetGehalte = 3,
            EiwitGehalte = 4,
            VetHoeveelheid = 5,
            EiwitHoeveelheid = 6,
            INET = 7,
        }

        public enum BVFieldNumberFWOnderbalkRund
        {
            Hoogtemaat = 1,
            Inhoud = 2,
            Klauwhoek = 3,
            Kruisbreedte = 4,
            Kruisligging = 5,
            Bespiering = 6,
            StandAchterbenen = 7,
            Vooruieraanhechting = 8,
            Uierdiepte = 9,
            Achteruierhoogte = 10,
            Ophangband = 11,
            Voorspeenplaatsing = 12,
            Speenlengte = 13,
            Voorhand = 14,
            Achterspeenplaatsing = 15,
            Openheid = 16,
            Achterbeenstand = 17,
            Conditiescore = 18,
            Beengebruik = 19,
        }


        public enum BVFieldNumberFWOverigeKoeRund
        {
            Melkbaarheid = 1,
            Karakter = 2,
            Vruchtbaarheid = 3,
            Geboorteproblemen = 4,
            aAaCode = 99,
        }

        public enum BVFieldNumberExterieurBovenKoe
        {
            Betrouwbaarheid = 1,
            Ontwikkeling = 2,
            Frame = 3,
            Uier = 4,
            Benen = 5,
            Bespiering = 6,
            AlgemeenVoorkomen = 7,
            Type = 8,
            Robuustheid = 9,
        }

        public enum BVFieldNumberExterieurOnderKoe
        {
            Hoogtemaat = 1,
            Inhoud = 2,
            Klauwhoek = 3,
            Kruisbreedte = 4,
            Kruisligging = 5,
            Bespiering = 6,
            StandAchterbenen = 7,
            Vooruieraanhechting = 8,
            Uierdiepte = 9,
            Achteruierhoogte = 10,
            Ophangband = 11,
            Voorspeenplaatsing = 12,
            Speenlengte = 13,
            Voorhand = 14,
            Achterspeenplaatsing = 15,
            Openheid = 16,
            Achterbeenstand = 17,
            Conditiescore = 18,
            Beengebruik = 19,
        }

        public enum AniCategory
        {
            Onbekend = 0,
            Aanwezig = 1,
            Fokstier = 2,
            Meststier = 3,
            AanwezigGeweest = 4,
            NooitAanwezigGeweest = 5,
        }

        public enum SalReason
        {
            Ouderdom = 3,
            Overtollig = 4,
            Slachtrijp = 5,
            Been_klauwaandoeningen = 10,
            VoedingsStoornissen = 11,
            ProblemenAfkalven = 12,
            Uiergebreken = 13,
            VruchtbaarheidsStoornissen = 14,
            OverigeGezondheidsAandoening = 19,
            LageProduktieEnOfExterieur = 20,
            Melkbaarheid = 21,
            SlechtExterieur = 22,
            Gedrag = 23,
            Export = 24,
            NUKA = 26,
            Fokdag = 96,
            Quotum = 97,
            FokGebruiksvee = 98,
            Overige = 99,
        }

        public enum KIorganisatie
        {
            Clemens = 1,
            Propos = 2,
            Salland = 3,
            TellusFocus = 4,
            TellusTigra = 5,
            Twente = 6,
            XSires = 7
        }

        /// <summary>
        /// http://wiki.vsm-hosting.nl/index.php/Agrobase_Database#soapkind
        /// </summary>
        public enum SOAPLOGKind
        {
            VetwerkDrachtMelding = 451,
            CRV_DIERWIJZIGIGN_901 = 901,
            CRV_DIERWIJZIGING_902 = 902,

            CRV_Meldingen_Raadplegen_HerdPedigree = 1031,
            CRV_Meldingen_Raadplegen_HerdPedigree_Details = 1032,
            CRV_Meldingen_Raadplegen_LastParity = 1033,
            CRV_Meldingen_Raadplegen_LastParity_Details = 1034,
            CRV_Meldingen_Responders_bijwerken = 1035,
            CRV_Snr_Mailen = 1112,
            GD_IDR = 1130,
            LNV_Meldingen_Raadplegen = 1120,
            LNV_Meldingen_Raadplegen_Details = 1121,
            Sanitel_Meldingen_Raadplegen = 1200,
            DCF_Meldingen_Raadplegen = 1300,
            HITIER_Meldingen_Raadplegen = 1400,
            Agrobase_Readresponder = 2210,
            Agrobase_Cycli = 2211,
            Agrobase_Animal = 2212,
            Sanitel_Verblijfplaatsen = 4100,
            ABSanatize_Verblijfplaatsen_Begin = 4101,
            ABSanatize_Verblijfplaatsen_Detail = 4102,
            ABSanatize_Verblijfplaatsen_Einde = 4103,
            MobilemateExport = 4200,
            RVO_Verblijfplaatsen = 4300,
            CRVMissingDataTask2 = 4400,
            CRVUpdateResponder = 4410,
            VerstuurNaarVeemanagerTask2 = 4450,
            WolswinkelEdiFactuurFTPTask = 4501,
            BeijerMengvoedersEdiFactuurFTPTask = 4502,
            KoerhuisEdiFactuurFTPTask = 4503,
            ValkEdiFactuurFTPTask = 4504,
            KoenisEdiFactuurFTPTask = 4505,
            TheeuwesMengvoedersZFactuurTask = 4506,
            EdiCircleTask = 4507,
            BoerenbondEdiFactuurTask = 4508,
            ForFarmersEdiFactuurTask = 4509,
            ABZDiervoedingEdiFactuurTask = 4510,
            CAVVZuidOostSallandEdiFactuurTask = 4511,
            FakkertEdiFactuurTask = 4512,
            Vetwerk2offectiveTask = 4515,
            IRMissingTask_RVO = 4601,
            AgrobaseActions = 4700,
            Automatische_lijsten_versturen = 5000,
            FileReader = 6000,
            FileReaderSopraco = 6010,
            FileReaderAds2xml = 6011,
            TwaKa_Veevolg_Systeem_API_data_ophalen = 7000,
            DeensSysteemImportExport = 8100,
            K01_DyrLacta = 8101,
            K01_Goldsund = 8103,
            K01_YdlRepro = 8103,
            K02 = 8104,
            DeenssysteemWerklijstImport = 8105,
            Mccmilk = 2013,
            Agrobase_Movement = 2213,
            Beyer2AgrobaseExport = 9002,
            Beyerwebservice = 9001
        }

        /// <summary>
        /// http://wiki.vsm-hosting.nl/index.php/ProgId
        /// </summary>
        public enum ProgId
        {
            rundvee = 1,
            stieren = 2,
            schapen = 3,
            zoogkoeien = 4,
            geiten = 5,
            witvlees = 6,
            rose_kalveren = 7,
            IENRDHZ = 8,
            fokvarken = 20,
            vleesvarken = 21,
            hond = 25,
            kat = 26,
            paard = 30
        }

        public enum TaskResult
        {
            OK = 1,
            WARN = 2,
            ERROR = 3,
            NO_DATA = 4
        }


        /// <summary>
        /// http://wiki.vsm-hosting.nl/index.php/TASK_LOG#TL_State
        /// </summary>
        public enum TL_State
        {
            BEGIN = 1,
            DONE_OK = 10,
            DONE_ERROR = 11,
        }

        public enum FILE_IMPORT_Status
        {
            NogNietBeoordeeld = 1,
            Bevestigd = 2,
            Afgekeurd = 3
        }

        public enum FILE_IMPORT_Filekind
        {
            Voer = 1,
            Medicijnen = 2,
            Bloedonderzoek = 3
        }

        public enum ReportStatus
        {
            MELDEN = 0,
            DHZ = 1,
            ONBETROUWBAAR = 2,
        }

        public enum FtpNumber
        {
            CAMPINA_XML_OLD = 1001,
            EDS_XML = 1002,
            FRIESLAND_CAMPINA = 1003,
            SOAP_CRV_1 = 9991,
            SOAP_LNV = 9992,
            SOAP_CRV_3 = 9993,
            SOAP_SANITEL = 9994,
            PALIDATA = 9995,
            GD_PINCODE = 9996,
            HITEIR_DUITSLAND = 9997,
            DFC_DENEMARKEN = 9998
        }


        public enum VerifiedDataSource
        {
            Unknown = 0,
            RVO = 1,
            Sanitel = 2,
            HiTier = 3,
            DCF = 4
        }

    }
}
