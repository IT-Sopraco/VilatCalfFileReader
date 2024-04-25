using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;
using System.Threading;
using System.Data;
using System.IO;
using System.Security.Cryptography;

namespace VSM.RUMA.SRV.FILEREADER.SOPRACO
{

    //public sealed class Bloodque
    //{
    //    private static Bloodque instance = null;
    //    private static readonly object padlock = new object();
    //    public List<string> bloodque { get; set; }
    //    Bloodque()
    //    {
    //    }

    //    public static Bloodque Instance
    //    {
    //        get
    //        {
    //            lock (padlock)
    //            {
    //                if (instance == null)
    //                {
    //                    instance = new Bloodque();
    //                    instance.bloodque = new List<string>();
    //                }
    //                return instance;
    //            }
    //        }
    //    }
    //}

    public class FilebufferSync
    {
        readonly AFSavetoDB DB;
        readonly UserRightsToken _token;
        readonly private char[] splitters = { ';' };
        readonly private char[] bloodsplitters = { ' ' };
        readonly private char[] bloodkommasplitter = { ',' };
        readonly private IEnumerable<AGRO_LABELS> labelssalClassificationMeat;
        readonly private IEnumerable<AGRO_LABELS> labelssalDeviationMeat;
        readonly int progid = 6;
        //private readonly int programid = 8;
        readonly int _ChangedBy = 2948;
    
        private FilebufferSync(UserRightsToken pToken, int changedBy)
        {
            _token = pToken;
            Facade.GetInstance().getRechten().VeranderDierDatabase(ref _token, (int)LABELSConst.ProgId.rose_kalveren);
            DB = Facade.GetInstance().getSaveToDB(pToken);
            List<int> lbs = new List<int>
            {
                25,
                26
            };

            var labels = DB.GetAgroLabels(lbs, 0, 0, 0);
            labelssalClassificationMeat = from n in labels where n.LabKind == 25 select n;
            labelssalDeviationMeat = from n in labels where n.LabKind == 26 select n;
            Soapkind = (int)LABELSConst.SOAPLOGKind.FileReaderSopraco;
            _ChangedBy = changedBy;
        
        }

        private static FilebufferSync _instance = null;

        private static Object _mutex = new Object();

        public static FilebufferSync GetInstance(UserRightsToken pToken, int changedBy)
        {
            if (_instance == null)
            {
                lock (_mutex) // now I can claim some form of thread safety...
                {
                    if (_instance == null)
                    {
                        _instance = new FilebufferSync( pToken, changedBy);
                    }
                }
            }

            return _instance;
        }

        public int Soapkind { get; set; }

        public enum FILE_IMPORT_STATUS
        {
            ImportZonderBeoordeling = 0, // is automatische xml-import
            NogNietBeoordeeld = 1,
            Bevestigd = 2,
            Afgekeurd = 3,
            AfgekeurdDoorAdmin = 4
        }

        public enum FILE_IMPORT_Filekind
        {
            Voer = 1,
            Medicijnen = 2,
            Bloedonderzoek1 = 3,
            Bloedonderzoek2 = 4,
            Bloedonderzoek3 = 5,
            Dieraanvoer = 6,
            SlachtdataSopraco = 7,
            SlachtdataSarreguemines = 8,
            SlachtdataAnder = 9
        }

        public class Fokker
        {
            public string FokkersnummerUitRegel { get; set; }
            public int ThrIdDestination { get; set; }
            public int UbnIdDestination { get; set; }
        }

        public FILE_IMPORT LeveringenImport(FILE_IMPORT_Filekind pFilekind, string pFilename, string pCsvLine, FILE_IMPORT_STATUS pStatus, int Regelnr)
        {
            /*
                Zie G:\agrobase\Projecten\Sopraco\ImportVoerMedicijnenSopraco.doc
             */
           
            int subkind = 6013;
            SOAPLOG sl = new SOAPLOG
            {
                Changed_By = _ChangedBy,
                Code = "",
                Date = DateTime.Now.Date,
                FarmNumber = "",
                Kind = Soapkind,
                SubKind = subkind,
                Lifenumber = "",
                Omschrijving = "",
                SourceID = 0,
                Status = "G",
                Time = DateTime.Now
            };
            FILE_IMPORT import = new FILE_IMPORT();

            string soort = "Voerlevering";

            switch (pFilekind)
            {
                case FILE_IMPORT_Filekind.Voer:
                    soort = "Voerlevering";
                    break;
                case FILE_IMPORT_Filekind.Bloedonderzoek1:
                    soort = "Bloedonderzoek1";
                    break;
                case FILE_IMPORT_Filekind.Bloedonderzoek2:
                    soort = "Bloedonderzoek2";
                    break;
                case FILE_IMPORT_Filekind.Bloedonderzoek3:
                    soort = "Bloedonderzoek3";
                    break;
                case FILE_IMPORT_Filekind.Medicijnen:
                    soort = "Medicijnlevering";
                    break;
                case FILE_IMPORT_Filekind.SlachtdataSopraco:
                    soort = "Slachtdata Sopraco";
                    break;
                case FILE_IMPORT_Filekind.SlachtdataSarreguemines:
                    soort = "Slachtdata Sarreguemines";
                    break;
                case FILE_IMPORT_Filekind.SlachtdataAnder:

                    throw new Exception($@"Not implemented: {FILE_IMPORT_Filekind.SlachtdataAnder}");

            }
            unLogger.WriteInfo($@"{pFilename} leveringenImport:{soort} CsvLine:{pCsvLine}");

            pCsvLine = pCsvLine.Trim();

            if (!String.IsNullOrEmpty(pCsvLine))
            {

                try
                {

                    int UbnIdDestination = 0;
                    int ThrIdDestination = 0;


                    int Fileimporttype = 0;


                    string[] importline = pCsvLine.Split(splitters);
                    unLogger.WriteInfo("importline.Length:" + importline.Length.ToString());
                    string[] bloodfilename = System.IO.Path.GetFileName(pFilename).Split(bloodsplitters);
                    //unLogger.WriteInfo("bloodfilename.Length:" + bloodfilename.Length.ToString());
                    string[] bloodline = pCsvLine.Split(bloodkommasplitter);
                    unLogger.WriteInfo("bloodline.Length:" + bloodline.Length.ToString());
                    if (bloodline.Length > 2)
                    {
                        if (bloodline[0].ToUpper() == "DATE")
                        {
                            return import;
                        }
                    }
                    if (importline.Length > 2)
                    {
                        if (importline[0].ToUpper() == "SLACHTDATUM")
                        {
                            return import;
                        }
                        if (importline[0].ToUpper() == "LEVERDATUM")
                        {
                            return import;
                        }
                    }
                    string Fokkersnummer = "";
                    string Inrichtingsnr = "";
                    int volgnr = Regelnr;
                    switch (pFilekind)
                    {
                        case FILE_IMPORT_Filekind.Voer:
                            if (importline.Length >= 8)
                            {
                                Fokkersnummer = importline[7];
                                Inrichtingsnr = importline[4];
                            }
                            Fileimporttype = 1;
                            subkind = 6013;
                            break;
                        case FILE_IMPORT_Filekind.Bloedonderzoek1:

                            if (bloodfilename.Length > 1)
                            { Fokkersnummer = bloodfilename[0]; }
                            if (bloodline.Length >= 2)
                            {
                                if (!int.TryParse(bloodline[0], out volgnr))
                                {
                                    volgnr = Regelnr;
                                }
                            }
                            subkind = 6010;
                            break;
                        case FILE_IMPORT_Filekind.Bloedonderzoek2:
                            // 3 files to read

                            if (bloodfilename.Length > 1)
                            { Fokkersnummer = bloodfilename[0]; }
                            if (bloodline.Length >= 3)
                            {
                                if (!int.TryParse(bloodline[3], out volgnr))
                                {
                                    volgnr = Regelnr;
                                }
                            }
                            subkind = 6010;
                            break;
                        case FILE_IMPORT_Filekind.Bloedonderzoek3:
                            // 3 files to read

                            if (bloodfilename.Length > 1)
                            { Fokkersnummer = bloodfilename[0]; }
                            if (bloodline.Length >= 2)
                            {
                                if (!int.TryParse(bloodline[0], out volgnr))
                                {
                                    volgnr = Regelnr;
                                }
                            }
                            subkind = 6010;
                            break;
                        case FILE_IMPORT_Filekind.Medicijnen:
                            if (importline.Length >= 12)
                            {
                                Fokkersnummer = importline[12];
                                Inrichtingsnr = importline[11];
                            }
                            subkind = 6014;
                            break;
                        case FILE_IMPORT_Filekind.SlachtdataSopraco:
                            if (importline.Length >= 8)
                            {
                                Fokkersnummer = importline[12];
                                Inrichtingsnr = importline[11];
                            }
                            subkind = 6012;
                            break;
                        case FILE_IMPORT_Filekind.SlachtdataSarreguemines:
                            if (importline.Length >= 11)
                            {
                                Fokkersnummer = importline[2];
                                Inrichtingsnr = importline[2];
                            }
                            subkind = 6015;
                            break;
                    }
                    unLogger.WriteInfo($@" Fokkersnummer:{Fokkersnummer} Inrichtingsnr:{Inrichtingsnr} ");
                    sl.SubKind = subkind;
                    Fileimporttype = (int)pFilekind;
                    if (string.IsNullOrEmpty(Fokkersnummer))
                    {
                        unLogger.WriteError($@"{soort}  Geen fokkersnummer gevonden FILE_IMPORT_Filekind:{(int)pFilekind} Filename:{pFilename}  regel:{pCsvLine}");
                        sl.Status = "F";
                        sl.Omschrijving = $@"{soort}  Geen fokkersnummer gevonden FILE_IMPORT_Filekind:{(int)pFilekind} Filename:{pFilename}  regel:{pCsvLine}";
                        DB.WriteSoapError(sl);
                        return import;
                    }
                    
                    if (ThrIdDestination == 0)
                    {

                        THIRD t = DB.GetThirdByStamboeknr(Fokkersnummer);
                        ThrIdDestination = t.ThrId;
                        var ubns = DB.getUBNsByThirdID(ThrIdDestination);
                        if (ubns.Count() > 0)
                        {
                            UBN u = ubns.ElementAt(0);
                            sl.FarmNumber = u.Bedrijfsnummer;
                            UbnIdDestination = u.UBNid;
                            Fokker f = new Fokker
                            {
                                FokkersnummerUitRegel = Fokkersnummer,
                                ThrIdDestination = ThrIdDestination,
                                UbnIdDestination = UbnIdDestination
                            };
                        }

                    }
                    unLogger.WriteInfo($@"ThrIdDestination:{ThrIdDestination} UbnIdDestination:{UbnIdDestination}  ");

                    if (ThrIdDestination > 0)
                    {
                        if (UbnIdDestination > 0)
                        {

                            List<FILE_IMPORT> fils;
                            if (pFilekind == FILE_IMPORT_Filekind.Voer || pFilekind == FILE_IMPORT_Filekind.SlachtdataSopraco || pFilekind == FILE_IMPORT_Filekind.SlachtdataSarreguemines)
                            {
                                fils = new List<FILE_IMPORT>();
                                if (pFilekind == FILE_IMPORT_Filekind.SlachtdataSopraco)
                                {
                                    FILE_IMPORT imp = DB.getFile_ImportByData(pCsvLine);
                                    if (imp.FILE_IMPORT_ID > 0)
                                    {
                                        fils.Add(imp);
                                    }
                                }
                                else 
                                {
                                    fils = DB.GetFile_ImportsByData(UbnIdDestination, "", pCsvLine, (int)pFilekind);
                                }
                            }
                            else
                            {
                                fils = DB.GetFile_ImportsByData(UbnIdDestination, System.IO.Path.GetFileName(pFilename), pCsvLine, (int)pFilekind);
                            }
                            //unLogger.WriteInfo($@"fillscount:{fils.Count()} UbnIdDestination:{UbnIdDestination}  ");

                            if (fils == null || fils.Count() == 0)
                            {
                                FILE_IMPORT fi = new FILE_IMPORT();
                                if (Fileimporttype > 0)
                                {
                                    fi.File_Import_Type_ID = Convert.ToSByte(Fileimporttype.ToString());

                                    fi.FI_Data_Row = pCsvLine;
                                    fi.FI_Filename = System.IO.Path.GetFileName(pFilename);
                                    fi.FI_State = (sbyte)pStatus;
                                    fi.ProgID_Origin = -1;//TODO 
                                    fi.ThrID_Destination = ThrIdDestination;
                                    fi.ThrID_Origin = -1;//TODO
                                    fi.UbnID_Destination = UbnIdDestination;
                                    fi.FI_Row_Nr = volgnr;
                                    fi.Changed_By = _ChangedBy;
                                    fi.SourceID = subkind;
                                    int ret = DB.saveFile_Import(fi);
                                    fi.FILE_IMPORT_ID = ret;
                                    import = fi;
                                    sl.Omschrijving = $@"{soort} leveringenImport:regel  ingelezen:" + ret.ToString();
                                    DB.WriteSoapError(sl);
                                    unLogger.WriteInfo(soort + " regel ingelezen  FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " regel:" + pCsvLine);

                                }
                                else
                                {
                                    unLogger.WriteError(soort + " Cannot find FileImport  " + pCsvLine);
                                    sl.Status = "F";
                                    sl.Omschrijving = $@"{soort} leveringenImport:regel:" + pCsvLine + " FileImport_type niet gevonden";
                                    DB.WriteSoapError(sl);
                                    return fi;
                                }
                            }
                            else
                            {
                                import = fils.ElementAt(0);
                                unLogger.WriteError(soort + " regel reeds eerder ingelezen  FILE_IMPORT_ID:" + fils.ElementAt(0).FILE_IMPORT_ID.ToString() + " regel:" + pCsvLine);
                                sl.Status = "W";
                                sl.Omschrijving = $@"{soort} Reeds eerder ingelezen: FILE_IMPORT_ID:" + fils.ElementAt(0).FILE_IMPORT_ID.ToString();
                                DB.WriteSoapError(sl);
                                
                            }
                        }
                        else
                        {
                            unLogger.WriteError(soort + " Cannot Find UBN fokkersnummer" + Fokkersnummer + " inrichtingsnr:" + Inrichtingsnr);
                            sl.Status = "F";
                            sl.Omschrijving = $@"{soort} leveringenImport:regel:" + pCsvLine + " Bedrijf niet gevonden:" + Fokkersnummer;
                            DB.WriteSoapError(sl);
                        }
                    } 
                    else
                    {
                        unLogger.WriteError(soort + " Cannot Find THIRD fokkersnummer" + Fokkersnummer + " inrichtingsnr:" + Inrichtingsnr);
                        sl.Status = "F";
                        sl.Omschrijving = "leveringenImport:regel:" + pCsvLine + " Bedrijf niet gevonden:" + Fokkersnummer;
                        DB.WriteSoapError(sl);
                    }

                }
                catch (Exception exc)
                {
                    unLogger.WriteError(exc.ToString());
                    sl.Status = "F";
                    sl.Omschrijving = "leveringenImport:regel:" + pCsvLine + " Err:" + exc.Message;
                    DB.WriteSoapError(sl);
                }
            }
            return import;
        }

        internal void controleervoeren(FILE_IMPORT_Filekind fileKind, string bestandsnaam, List<FILE_IMPORT> result)
        {
            
            int subkind = 4700001;
            int kind = 6012;
            int soapsubkind = 6010;
            if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.Voer)
            {
                SOAPLOG sl = new SOAPLOG
                {
                    Changed_By = _ChangedBy,
                    Code = "Voerlevering",
                    Date = DateTime.Now.Date,
                    FarmNumber = "",
                    Kind = Soapkind,
                    SubKind = soapsubkind,
                    Lifenumber = "",
                    Omschrijving = $@"{bestandsnaam}",
                    SourceID = 0,
                    Status = "G",
                    Time = DateTime.Now
                };

                String qry = $@"SELECT
                            a.ArtId,
                            a.ArtNumber,
                            a.ArtNaam,
                            a.ArtGroep,
                            a.ArtShowOnWebsite,
                                a.ArtPrijsPer,
                                a.Art_Inactive,
                            l.LAB_Label AS Soort,
                                l.LAB_Value AS VoerSoortId,
                                av.NormVevi,
                                av.NormRE,
                                av.DrogeStof,
                                av.NormRC,
                                av.NormSelenium,
                                av.NormKoper,
                                av.NormIjzer,
                                av.BulkArtikel,
                                av.Voergroup,
                                pm.ThrId
                            FROM agrolink.PROGRAM_GROUP pg
                            JOIN agrolink.PROGRAM_MEMBER_GROUP pmg ON pmg.PgId = pg.PgId AND pmg.PmgId > 0
                            JOIN agrolink.PROGRAM_MEMBER pm ON pm.Pm_Id = pmg.PmId and pm.ProgId = 82
                            JOIN agrofactuur.ARTIKEL a ON a.ArtThrId = pm.ThrId AND a.ArtId > 0
                            LEFT JOIN agrofactuur.ARTIKEL_VOER av ON av.artId = a.ArtId
                            LEFT JOIN agrolink.LABELS l ON l.LAB_Kind = 101 AND l.LAB_CountryISO = 0 AND l.LAB_AnimalKind = 7 AND l.LAB_Value = a.ArtGroep AND l.ProgId = 82
                            WHERE pg.ProgId = 82 AND pg.PgId > 0 AND av.AV_ID is not null  AND pg.PgPrefix = 'Voerleverancier' AND a.ArtId > 0
                            ORDER BY a.ArtNaam";
                DataTable tbl = DB.GetDataBase().QueryData(_token, new StringBuilder(qry), MissingSchemaAction.Add);
                if (tbl.Rows.Count == 0)
                {
                    sl.Status = "F";
                    sl.Omschrijving += ":Geen voeren gevonden";
                    DB.WriteSoapError(sl);
                }
                else
                {
                    try
                    {
                        foreach (FILE_IMPORT fi in result)
                        {
                            string[] regel = fi.FI_Data_Row.Split(splitters);
                            if (regel.Length >= 7 && !string.IsNullOrWhiteSpace(regel[1]))
                            {
                                var artikels = tbl.Select("ArtNumber='" + regel[1] + "'");
                                if (artikels.Count() == 0)
                                {
                                    //ONBEKEND
                                    UBN lUBN = DB.GetubnById(fi.UbnID_Destination);
                                    SOAPLOG slvoer = new SOAPLOG
                                    {
                                        Changed_By = _ChangedBy,
                                        Code = "Voerlevering",
                                        Date = DateTime.Now.Date,
                                        FarmNumber = lUBN.Bedrijfsnummer,
                                        Kind = kind,
                                        SubKind = subkind,
                                        Lifenumber = "",
                                        Omschrijving = $@"Levering onbekend voer, ArtNumber:{regel[1]}",
                                        SourceID = 0,
                                        Status = "W",
                                        Time = DateTime.Now,

                                    };
                                    DB.WriteSoapError(slvoer);
                                }
                            }
                        }
                        DB.WriteSoapError(sl);
                    }
                    catch(Exception exc)
                    {

                        sl.Status = "F";
                        sl.Omschrijving += exc.Message;
                        DB.WriteSoapError(sl);
                    }
                }
            }
        }

        /// <summary>
        /// van het brok bestand: alleen de  locaties opslaan die erin staan
        /// </summary>
        /// <param name="fileKind"></param>
        /// <param name="bestandspath"></param>
        /// <param name="v"></param>
        internal void Broklocaties(FILE_IMPORT_Filekind fileKind, string bestandspath)
        {
            int subkind = 6016;
      
            string bestandsnaam = Path.GetFileName(bestandspath);
            
            if (fileKind == FILE_IMPORT_Filekind.Bloedonderzoek3)
            {
                SOAPLOG sl = new SOAPLOG
                {
                    Changed_By = _ChangedBy,
                    Code = $@"{nameof(Broklocaties)}",
                    Date = DateTime.Now.Date,
                    FarmNumber = bestandsnaam,
                    Kind = Soapkind,
                    SubKind = subkind,
                    Lifenumber = "",
                    Omschrijving = $@"",
                    SourceID = 0,
                    Status = "G",
                    Time = DateTime.Now,

                };
                unLogger.WriteInfo($@"{nameof(Broklocaties)}");
           
             
                var imports = DB.GetFile_ImportsByName(bestandsnaam, (int)FILE_IMPORT_Filekind.Bloedonderzoek3);
                sl.Omschrijving += $@"Aantal: {imports.Count()}";
                if (imports.Count() > 0)
                {
                    UBN lUBN = DB.GetubnById(imports.ElementAt(0).UbnID_Destination);
                    sl.FarmNumber = lUBN.Bedrijfsnummer;

                    unLogger.WriteDebug($@"Ubn:{lUBN.Bedrijfsnummer} {lUBN.Bedrijfsnaam}");
              
                    var countries = DB.GetAllCountries();
                    var animals = DB.GetAnimalsByUbnId(lUBN.UBNid, (int)CORE.DB.LABELSConst.AniCategory.AanwezigGeweest).ToList();
              
              
                    foreach (FILE_IMPORT fi in imports)
                    {
                        SOAPLOG slplace = new SOAPLOG
                        {
                            Changed_By = _ChangedBy,
                            Code = $@"{nameof(Broklocaties)}",
                            Date = DateTime.Now.Date,
                            FarmNumber = lUBN.Bedrijfsnummer,
                            Kind = Soapkind,
                            SubKind = subkind,
                            Lifenumber = "",
                            Omschrijving ="",
                            SourceID = 0,
                            Status = "G",
                            Time = DateTime.Now,

                        };
                        DateTime datumbestand = DateTime.MinValue;

                        try
                        {
                            string[] datums = fi.FI_Filename.Split(' ').ElementAt(1).Split('-');
                            datumbestand = new DateTime(int.Parse(datums[2]), int.Parse(datums[1]), int.Parse(datums[0]));
                        }
                        catch { }
                        string[] data = fi.FI_Data_Row.Split(',');

                        string afdeling = data[1];
                       
                       
                        ANIMAL animal = findAnimal(data[2], animals, countries, lUBN.Bedrijfsnummer);
                        try
                        {
                            
                            if (!string.IsNullOrEmpty(afdeling))
                            {
                                try
                                {


                                 
                                    if (animal == null || animal.AniId <= 0)
                                    {
                                        slplace.Status = "F";
                                        slplace.Omschrijving = $@" ANIMAL: {data[2]}  not found";
                                    }
                                    else
                                    {
                                        slplace.Lifenumber = animal.AniLifeNumber;
                                        unLogger.WriteInfo($@"{animal.AniLifeNumber}: ThrID_Destination:{fi.ThrID_Destination}  UbnID_Destination:{fi.UbnID_Destination}");
                                        MOVEMENT movplace = DB.GetMovementByDateAniIdKind(datumbestand.Date, animal.AniId, (int)CORE.DB.LABELSConst.MovementKind.LOKATIE, fi.UbnID_Destination);

                                     
                                        PLACE pl = new PLACE();
                                        if (movplace.MovId > 0)
                                        {
                                            pl = DB.GetPlace(movplace.MovId);
                                        }

                                        var remarks = DB.getFarmRemarksByUbnId(fi.UbnID_Destination).FindAll(x => x.LabKind == (int)LABELSConst.RemarkLabKind.afdeling && x.LabId == 1);
                                       
                                        if (remarks == null)
                                        {
                                            remarks = new List<REMARK>();
                                        }
                                        int newremarkid = 1;
                                        if (remarks.Count() > 0)
                                        {
                                            newremarkid = remarks.Max(x => x.RemId) + 1;
                                        }
                                        REMARK rem = new REMARK();

                                        var remsearch = from n in remarks
                                                        where n.RemLabel == afdeling
                                                        select n;
                                        if (remsearch.Count() > 0)
                                        {
                                            rem = remsearch.ElementAt(0);
                                        }


                                        if (rem.RemId == 0)
                                        {
                                            var r = new REMARK
                                            {
                                                //Men bedoelde hier afdeling
                                                RemLabel = afdeling,
                                                LabId = 1,
                                                LabKind = (int)LABELSConst.RemarkLabKind.afdeling,
                                                Farmid = Getfarmid(fi.UbnID_Destination),
                                                UbnId = fi.UbnID_Destination,
                                                Changed_By = _ChangedBy,
                                                SourceID = fi.ThrID_Destination,
                                                RemId = newremarkid

                                            };
                                            DB.SaveRemark(r);
                                            pl.SectionNr = r.RemId;
                                        }
                                        else
                                        {
                                            pl.SectionNr = rem.RemId;
                                        }
                                        

                                        if (pl.MovId == 0)
                                        {
                                            MOVEMENT mnew = new MOVEMENT
                                            {
                                                AniId = animal.AniId,
                                                UbnId = fi.UbnID_Destination,
                                                MovKind = 8,
                                                MovDate = datumbestand,
                                                Changed_By = _ChangedBy
                                            };
                                            pl.AniId = animal.AniId;
                                            pl.StableNr = 1;
                                            DB.SaveMovement(mnew);
                                            pl.MovId = mnew.MovId;
                                            pl.Changed_By = _ChangedBy;
                                            pl.FarmNumber = lUBN.Bedrijfsnummer;
                                            DB.savePlace(pl);
                                            unLogger.WriteInfo($@"{animal.AniLifeNumber}: Insertmovid:{mnew.MovId} {datumbestand.ToString("dd-MM-yyyy")} pl.SectionNr:{pl.SectionNr} = Remlabel:{afdeling}");
                                            slplace.Omschrijving = $@"Place Insert movid:{mnew.MovId} {datumbestand.ToString("dd-MM-yyyy")} SectionNr:{pl.SectionNr}  Remlabel:{afdeling}";
                                        }
                                        else
                                        {
                                            pl.Changed_By = _ChangedBy;
                                            pl.FarmNumber = lUBN.Bedrijfsnummer;
                                            DB.savePlace(pl);
                                            unLogger.WriteInfo($@"{animal.AniLifeNumber}: Updatemovid:{pl.MovId} {datumbestand.ToString("dd-MM-yyyy")} pl.SectionNr:{pl.SectionNr} = Remlabel:{afdeling}");
                                            slplace.Omschrijving = $@"Place Update movid {pl.MovId} SectionNr:{pl.SectionNr} {datumbestand.ToString("dd-MM-yyyy")}  Remlabel:{afdeling}";
                                        }
                                    }
                                    DB.WriteSoapError(slplace);
                                }
                                catch (Exception exc)
                                {
                                    unLogger.WriteInfo($@"{data[2]}: Error in saving place: {exc.ToString()}");
                                    slplace.Status = "F";
                                    slplace.Omschrijving = exc.Message;
                                    DB.WriteSoapError(slplace);
                                }
                            }
                            else
                            {
                                unLogger.WriteInfo($@"{data[2]}: no place found ");
                                slplace.Status = "F";
                                slplace.Omschrijving += $@"{data[2]}: no place found ";
                                DB.WriteSoapError(slplace);
                            }
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("  levensnummer:" + data[2] + " ERR:" + exc.ToString());
                            slplace.Status = "F";
                            slplace.Omschrijving += $@":{data[2]}:{exc.Message} ";
                            DB.WriteSoapError(slplace);
                        }
                    }
                }
                DB.WriteSoapError(sl);
            }
        }

        /// <summary>
        /// Alleen nr2 bestanden hebben een brok bestand. Rekening houden met de groepen.
        /// Only nr2 files have a "brok" file. Take the groups into account.
        /// the purpose is to compare "brok"  compare="vergelijk"
        /// </summary>
        /// <param name="fileKind"></param>
        /// <param name="bestandspath"></param>

        internal void Brokvergelijk(FILE_IMPORT_Filekind fileKind, string bestandspath)
        {
            int subkind = 4700010;//zie 
            int kind = 6012;
            string bestandsnaam = Path.GetFileName(bestandspath);
            if (!bestandsnaam.Contains("nr2"))
            {
                return;
            }
            if (fileKind == FILE_IMPORT_Filekind.Bloedonderzoek1)
            {
                SOAPLOG sl1 = new SOAPLOG
                {
                    Changed_By = _ChangedBy,
                    Code = "Brokvergelijk",
                    Date = DateTime.Now.Date,
                    FarmNumber = bestandsnaam,
                    Kind = Soapkind,
                    SubKind = 6010,
                    Lifenumber = "",
                    Omschrijving = $@"{nameof(Brokvergelijk)}, {System.IO.Path.GetFileName(bestandsnaam)}",
                    SourceID = 0,
                    Status = "G",
                    Time = DateTime.Now,

                };
                SOAPLOG slenkelvergelijk = new SOAPLOG
                {
                    Changed_By = _ChangedBy,
                    Code = "Brokvergelijk",
                    Date = DateTime.Now.Date,
                    FarmNumber = bestandsnaam,
                    Kind = kind,
                    SubKind = subkind,
                    Lifenumber = "",
                    Omschrijving = $@"{nameof(Brokvergelijk)}, {System.IO.Path.GetFileName(bestandsnaam)}",
                    SourceID = 0,
                    Status = "G",
                    Time = DateTime.Now,

                };
                unLogger.WriteInfo($@"Start: {sl1.Omschrijving}");
                StringBuilder bld = new StringBuilder();
                bld.AppendLine($@"Bestanden vergeleken:");
                string bestand3 = bestandsnaam;
              
                string bestand5 = bestandsnaam.Replace("nr","brok nr");
                var imports = DB.GetFile_ImportsByName(bestand3, (int)FILE_IMPORT_Filekind.Bloedonderzoek1);
                bld.AppendLine($@"{bestand3}: aantal:{imports.Count()}");
                unLogger.WriteInfo($@"{bestand3}: aantal:{imports.Count()}");
                var imports5 = DB.GetFile_ImportsByName(bestand5, (int)FILE_IMPORT_Filekind.Bloedonderzoek3);
                if (imports5.Count() == 0)
                {
                    unLogger.WriteInfo($@"{bestand5} niet gevonden");
                    bld.AppendLine($@"{bestand5}: niet gevonden.");
                }
                else
                {
                    unLogger.WriteInfo($@"{bestand5}  gevonden");
                    unLogger.WriteInfo($@"{bestand5}: aantal:{imports5.Count()}");
                    bld.AppendLine($@"{bestand5}: aantal:{imports5.Count()}");
                    imports.AddRange(imports5);
                }

                if (imports.Count() > 0)
                {
                    

                    #region groeps
                    var aanwezig = GetAnimalsByUbnIdAndStateWithPlaceExt(imports.ElementAt(0).UbnID_Destination, AnimalState.PresentOnFarm, null);
                    unLogger.WriteInfo($@"{nameof(Brokvergelijk)} GetAnimalsByUbnIdAndStateWithPlaceExt: aantal:{aanwezig.Rows.Count}");
                    #endregion

                    UBN lUBN = DB.GetubnById(imports.ElementAt(0).UbnID_Destination);
                    sl1.FarmNumber = lUBN.Bedrijfsnummer;
                    slenkelvergelijk.FarmNumber = lUBN.Bedrijfsnummer;
                    unLogger.WriteInfo($@"Ubn:{lUBN.Bedrijfsnummer} {lUBN.Bedrijfsnaam}");

                    bld.AppendLine($@"Ubn:{lUBN.Bedrijfsnummer} {lUBN.Bedrijfsnaam}");
                    var countries = DB.GetAllCountries();
                    var animals = DB.GetAnimalsByUbnId(lUBN.UBNid, (int)CORE.DB.LABELSConst.AniCategory.AanwezigGeweest).ToList();
                    var aanwezigenietgescand = animals.FindAll(x => x.AniCategory < 4);
                    var gescandnietgevondenanimals = new List<ANIMAL>();
                    List<string> groepen = new List<string>();
                    foreach (FILE_IMPORT fi in imports)
                    {
                        DateTime datumbestand = DateTime.MinValue;

                        try
                        {
                            string[] datums = fi.FI_Filename.Split(' ').ElementAt(1).Split('-');
                            datumbestand = new DateTime(int.Parse(datums[2]), int.Parse(datums[1]), int.Parse(datums[0]));
                        }
                        catch { }
                        string[] data = fi.FI_Data_Row.Split(',');

                        string afdeling = data[1];
                        string land = data[2].Substring(0, 3);
                        string nummer = data[2].Substring(3, data[2].Length - 3);
                   
                        ANIMAL animal = findAnimal(data[2], animals, countries, lUBN.Bedrijfsnummer);
                        if (animal.AniId > 0)
                        {
                            try
                            {
                                if (groepen.Count() < 2)
                                {
                                    var vdier = aanwezig.Select($@"anialternatenumber='{animal.AniLifeNumber}'");
                                    if (vdier.Count() > 0)
                                    {
                                        string groep = vdier[0]["GroupName"].ToString();
                                        if (!string.IsNullOrWhiteSpace(groep))
                                        {
                                            if (!groepen.Contains(groep))
                                            {
                                                groepen.Add(groep);
                                                unLogger.WriteDebug("groepen Add:" + groep);
                                            }
                                        }
                                    }
                                }

                            }
                            catch (Exception exc)
                            {
                                unLogger.WriteError("groepen:" + exc.ToString());
                            }
                            if (animals.Any(x => x.AniAlternateNumber == animal.AniLifeNumber))
                            {
                                if (aanwezigenietgescand.Any(x => x.AniAlternateNumber == animal.AniLifeNumber))
                                {
                                    aanwezigenietgescand.RemoveAll(x => x.AniAlternateNumber == animal.AniLifeNumber);
                                    //Dieren aanwezig in vilatcalf maar niet gescand, zijn degene die overblijven
                                    unLogger.WriteInfo($@"aanwezigenietgescand.RemoveAll:{animal.AniLifeNumber} rest:{aanwezigenietgescand.Count()}");
                                }
                            }
                            else
                            {
                                //Dieren gescand maar niet in vilatcalf
                                gescandnietgevondenanimals.Add(new ANIMAL { AniLifeNumber = animal.AniLifeNumber, AniAlternateNumber = animal.AniLifeNumber });
                                unLogger.WriteInfo($@"gescandnietgevondenanimals.Add:{animal.AniLifeNumber} rest:{gescandnietgevondenanimals.Count()}");
                            }
                        }
                    }
                    if (groepen.Count() > 0)
                    {
                        foreach (DataRow rw in aanwezig.Rows)
                        {
                            var a = aanwezigenietgescand.RemoveAll(x => x.AniAlternateNumber == rw["anialternatenumber"].ToString() && !groepen.Contains(rw["GroupName"].ToString()));
                        }
                    }
                    //verder met MESSAGES
                    sl1.Omschrijving = $@"{bestandsnaam}: gescand maar niet gevonden:{gescandnietgevondenanimals.Count()} aanwezig maar niet gescand:{aanwezigenietgescand.Count()}";
                    unLogger.WriteInfo(sl1.Omschrijving);
                    if (gescandnietgevondenanimals.Count() > 0 || aanwezigenietgescand.Count() > 0)
                    {
                        var relations = DB.GetAgrolinkRelations("beheerder", 82);
                        MESSAGES beheerderbericht = new MESSAGES();


                        bld.AppendLine($@"Dieren gescand maar niet in vilatcalf: aantal:{gescandnietgevondenanimals.Count()}");

                        foreach (ANIMAL a in gescandnietgevondenanimals)
                        {
                            //bld.AppendLine($@"{a.AniAlternateNumber}");
                            //unLogger.WriteInfo($@"{a.AniAlternateNumber}");
                        }

                        bld.AppendLine($@"Dieren aanwezig in vilatcalf maar niet gescand: aantal:{aanwezigenietgescand.Count()}");


                        foreach (ANIMAL a in aanwezigenietgescand)
                        {
                            //bld.AppendLine($@"{a.AniAlternateNumber}");
                            //unLogger.WriteDebug($@"{a.AniAlternateNumber}");
                        }

                        unLogger.WriteDebug($@"Bericht sturen naar beheerder:");
                        beheerderbericht.MesMessage = bld.ToString();
                        beheerderbericht.MesBegin_DateTime = DateTime.Now;
                        beheerderbericht.MesEnd_DateTime = DateTime.Now.AddDays(7);
                        beheerderbericht.MesFromThrId = lUBN.ThrID;
                        beheerderbericht.MesProgramID = -99;

                        beheerderbericht.MesProgId = 82;
                        beheerderbericht.MesSubject = "Bloeduitslagen dieren vergelijk ";
                        beheerderbericht.MesThrId = relations.First().ThrId;
                        beheerderbericht.MesUbnId = 0;
                        string MesVersion = Guid.NewGuid().ToString();
                        beheerderbericht.MesVersion = MesVersion;
                        beheerderbericht.MesState = 1;
                        DB.SaveMessage(beheerderbericht);
                        slenkelvergelijk.Omschrijving = bld.ToString();
                        slenkelvergelijk.Status = "F";
                        DB.WriteSoapError(slenkelvergelijk);

                    }
                    else 
                    {
                        DB.WriteSoapError(slenkelvergelijk);
                    }
                   
                }
                else 
                {
                    sl1.Omschrijving = $@"{bestandsnaam}:Geen fileimports ingelezen";
                    unLogger.WriteInfo(sl1.Omschrijving);
                }
                DB.WriteSoapError(sl1);
            }
        }
   
        public int Controlebloodfilename(FILE_IMPORT_Filekind fileKind, string filename)
        {
            string bestand = System.IO.Path.GetFileNameWithoutExtension(filename);
            int subkind = 6010;
            SOAPLOG slerr = new SOAPLOG
            {
                Changed_By = _ChangedBy,
                Code = "",
                Date = DateTime.Now.Date,
                FarmNumber = "",
                Kind = Soapkind,
                SubKind = subkind,
                Lifenumber = "",
                Omschrijving = "Save Blood:",
                SourceID = 0,
                Status = "F",
                Time = DateTime.Now
            };
            if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek1 || fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek2)
            {
              
              
                string[] bloodfilename = bestand.Split(bloodsplitters);
                if (bloodfilename.Count() != 3)
                {
                    unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File not correct:" + bestand);
                    slerr.Omschrijving = $@"Name File not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
                THIRD tfname = DB.GetThirdByStamboeknr(bloodfilename[0]);
                if (tfname.ThrId <= 0)
                {
                    unLogger.WriteError($@"savebloodindatabase fileKind:{fileKind} Name File Unknown Stamboeknr:{bloodfilename[0]}  " + bestand);
                    slerr.Omschrijving = $@"Name File Stamboeknr not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
                DateTime? vd = GetDateFormatFromFileName(bloodfilename[1]);
                if (!vd.HasValue)
                {
                    unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File Date not correct:" + bestand);
                    slerr.Omschrijving = $@"Name File DATE not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
                if (!int.TryParse(bloodfilename[2].ToUpper().Replace("NR", ""), out int BLOODBlnNumber))
                {
                    unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File NR not correct:" + bestand);
                    slerr.Omschrijving = $@"Name File NR not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
            }
            else if (fileKind == FilebufferSync.FILE_IMPORT_Filekind.Bloedonderzoek3)
            {
                string[] bloodfilename = bestand.Split(bloodsplitters);
                if (bloodfilename.Count() != 4)
                {
                    unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File not correct:" + bestand);
                    slerr.Omschrijving = $@"Name File not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
                THIRD tfname = DB.GetThirdByStamboeknr(bloodfilename[0]);
                if (tfname.ThrId <= 0)
                {
                    unLogger.WriteError($@"savebloodindatabase fileKind:{fileKind} Name File Unknown Stamboeknr:{bloodfilename[0]}  " + bestand);
                    slerr.Omschrijving = $@"Name File Stamboeknr not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
                DateTime? vd = GetDateFormatFromFileName(bloodfilename[1]);
                if (!vd.HasValue)
                {
                    unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File Date not correct:" + bestand);
                    slerr.Omschrijving = $@"Name File DATE not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
                if (!int.TryParse(bloodfilename[3].ToUpper().Replace("NR", ""), out int BLOODBlnNumber))
                {
                    unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File NR not correct:" + bestand);
                    slerr.Omschrijving = $@"Name File NR not correct:" + bestand;
                    DB.WriteSoapError(slerr);
                    return -2;
                }
            }
            return 1;
        }
   
        public void Savebloodindatabase(FILE_IMPORT_Filekind fileKind, string pFilename, int pAgroLinkProgId = 0)
        {
            int subkind = 6010;
            SOAPLOG sl1 = new SOAPLOG
            {
                Changed_By = _ChangedBy,
                Code = "",
                Date = DateTime.Now.Date,
                FarmNumber =  fileKind.ToString() ,
                Kind = Soapkind,
                SubKind = subkind-1,
                Lifenumber = "",
                Omschrijving = $@"Start Inlezen database, {System.IO.Path.GetFileName(pFilename)}",
                SourceID = 0,
                Status = "G",
                Time = DateTime.Now,

            };
            if (fileKind == FILE_IMPORT_Filekind.Bloedonderzoek1)
            {
                int looprunner = 0;
                int sleep = 10000;
                while (looprunner < 60)
                {
                    looprunner += 1;
                    if (File.Exists(pFilename.Replace(".txt", ".csv")))
                    {
                        unLogger.WriteInfo($@"Thread.Sleep({sleep}) tegenbestand csv nog aanwezig van File:" + System.IO.Path.GetFileName(pFilename));
                        Thread.Sleep(sleep);
                    }
                }
                if (File.Exists(pFilename.Replace(".txt", ".csv")))
                {
                    unLogger.WriteInfo("return exit fileKind:" + fileKind.ToString() + " File:" + System.IO.Path.GetFileName(pFilename));
                    sl1.Omschrijving = $@"Niet inlezen database, {System.IO.Path.GetFileName(pFilename)}";
                    DB.WriteSoapError(sl1);
                    return;
                }
            }
            else if (fileKind == FILE_IMPORT_Filekind.Bloedonderzoek2)
            {
                if (File.Exists(pFilename.Replace(".csv", ".txt")))
                {
                    unLogger.WriteInfo($@"return exit tegenbestand:{pFilename.Replace(".csv", ".txt")} nog aanwezig fileKind:" + fileKind.ToString() + " File:" + System.IO.Path.GetFileName(pFilename));
                    sl1.Omschrijving = $@"Niet inlezen database, {System.IO.Path.GetFileName(pFilename)}";
                    DB.WriteSoapError(sl1);
                    return;
                }
            }
            else 
            {
                return;
            }
            DB.WriteSoapError(sl1);
            unLogger.WriteInfo($@"Start inlezen: {nameof(Savebloodindatabase)} fileKind:{fileKind} File:{System.IO.Path.GetFileName(pFilename)}");
            if (pFilename.ToUpper().Contains("BROK")) { if (fileKind != FILE_IMPORT_Filekind.Bloedonderzoek3) { unLogger.WriteError("Wrong filekind brok " + pFilename); return; } }
            string bestand = System.IO.Path.GetFileNameWithoutExtension(pFilename);
        


            string[] bloodfilename = bestand.Split(bloodsplitters);
            if (!int.TryParse(bloodfilename[2].ToUpper().Replace("NR", ""), out int BLOODBlnNumber))
            {
                unLogger.WriteError("savebloodindatabase fileKind:" + fileKind.ToString() + " Name File NR not correct:" + bestand);

            }
            unLogger.WriteInfo("savebloodindatabase BLOODBlnNumber:" + BLOODBlnNumber.ToString());
            var relations = DB.GetAgrolinkRelations("beheerder", pAgroLinkProgId);
            THIRD tMessageFrom = new THIRD();
            if ((relations != null) && (relations.Count() > 0))
            {
                tMessageFrom = relations.ElementAt(0);
            }
            List<Tuple<THIRD, MESSAGES>> berichten = new List<Tuple<THIRD, MESSAGES>>();
            if (fileKind == FILE_IMPORT_Filekind.Bloedonderzoek1 || fileKind == FILE_IMPORT_Filekind.Bloedonderzoek2)
            {

                var tblBloedonderzoek1 = DB.GetFile_ImportsByName(bestand + ".txt", (int)FILE_IMPORT_Filekind.Bloedonderzoek1);
                var tblBloedonderzoek2 = DB.GetFile_ImportsByName(bestand + ".csv", (int)FILE_IMPORT_Filekind.Bloedonderzoek2);

                unLogger.WriteInfo("savebloodindatabase Bloedonderzoek1:" + tblBloedonderzoek1.Count().ToString() + " Bloedonderzoek2:" + tblBloedonderzoek2.Count().ToString());
                if (tblBloedonderzoek1.Count() > 0 && tblBloedonderzoek2.Count() > 0)
                {
                    if (tblBloedonderzoek1.Count() != tblBloedonderzoek2.Count())
                    {
                        unLogger.WriteError($@"Aantallen van de  bestanden ongelijk: '{bestand}' UbnID_Destination:{tblBloedonderzoek1.ElementAt(0).UbnID_Destination}" );
                        sl1.Omschrijving = $@"Aantallen van de  bestanden ongelijk: '{bestand}'  UbnID_Destination:{tblBloedonderzoek1.ElementAt(0).UbnID_Destination}";
                        sl1.Status = "F";
                        DB.WriteSoapError(sl1);
                    }
                    else
                    {
                      
                        unLogger.WriteInfo($@"inlezen {pFilename}");
                     
                      
                        var countries = DB.GetAllCountries();

                        try
                        {

                            UBN lUBN = DB.GetubnById(tblBloedonderzoek1.ElementAt(0).UbnID_Destination);
                            var animals = DB.GetAnimalsByUbnId(lUBN.UBNid, 4);
                            foreach (FILE_IMPORT rw1 in tblBloedonderzoek1)
                            {
                                int _farmid = Getfarmid(rw1.UbnID_Destination);
                                //volgnr:int,hoknr:string,levensnr:string
                                string[] row1 = rw1.FI_Data_Row.Split(bloodkommasplitter);
                                foreach (FILE_IMPORT rw2 in tblBloedonderzoek2)
                                {
                                    SOAPLOG sl = new SOAPLOG
                                    {
                                        Changed_By = _ChangedBy,
                                        Code = "",
                                        Date = DateTime.Now.Date,
                                        FarmNumber = lUBN.Bedrijfsnummer,
                                        Kind = Soapkind,
                                        SubKind = subkind,
                                        Lifenumber = "",
                                        Omschrijving = $@"volgnr:{row1[0].Trim()}",
                                        SourceID = 0,
                                        Status = "G",
                                        Time = DateTime.Now,
                                        
                                    };
                                    //DATE:string,TIME:string,MODE:string,NO:string,PID:string,FLAG:string,ANA-MODE:string,ONBEKEND1:string,WBC:string,RBC:string,HGB:string,HCT:string,MCV:string,MCH:string,MCHC:string,PLT:string,RDW-SD:string,RDW-CV:string,PDW:string,MPV:string,P-LCR:string,PCT,NEUT#:string,LYMPH#:string,MONO#:string,EOSI#:string,BASO#:string,NEUT%:string,LYMPH%:string,MONO%:string,EOSI%:string,BASO%:string,NRBC#:string,NRBC%:string,RET%:string,RET#:string,IRF:string,LFR:string,MFR:string,HFR:string,
                                    string[] row2 = rw2.FI_Data_Row.Split(bloodkommasplitter);

                                    if (row1[0].Trim() == row2[3].Trim())
                                    {
                                        //Bij elkaar horende regels gevonden
                                        string afdeling = row1[1];
                                        string levensnummer = row1[2];

                                        sl.Lifenumber = levensnummer;
                                        ANIMAL animal = findAnimal(levensnummer, animals.ToList(), countries, lUBN.Bedrijfsnummer);


                                        if (animal != null && animal.AniId > 0)
                                        {
                                            string uitslag = row2[10];

                                            ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandUbnid(animal.AniId, rw1.UbnID_Destination);
                                            if (ac.AniId > 0 && ac.Anicategory <= 4)
                                            {

                                            }
                                            else
                                            {
                                                unLogger.WriteError($@"VSM.RUMA.SRV.FILEREADER.SOPRACO savebloodindatabase {animal.AniAlternateNumber} onbekend bij UBN id:{rw1.UbnID_Destination}");
                                                sl.Status = "F";
                                                sl.Omschrijving = $@" {animal.AniAlternateNumber} onbekend bij UBN id:{rw1.UbnID_Destination} ";
                                                DB.WriteSoapError(sl);
                                                continue;
                                            }
                                            sl.Lifenumber = animal.AniLifeNumber;
                                            bool test = false;
                                            if(!sl.Lifenumber.StartsWith("BE"))
                                            {
                                                test = true;
                                            }
                                            DateTime? datum = GetVilatcaDate(row2[0], row2[1]);
                                            unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: datum:{datum}");
                                            if (datum.HasValue)
                                            {

                                                THIRD u = DB.GetThirdByThirId(rw1.ThrID_Destination);
                                                unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: ThrID_Destination:{rw1.ThrID_Destination} rw1.UbnID_Destination:{rw1.UbnID_Destination}");
                                                MOVEMENT movplace = DB.GetMovementByDateAniIdKind(datum.Value.Date, animal.AniId, 8, rw1.UbnID_Destination);

                                                PLACE pl = new PLACE();
                                                if (movplace.MovId > 0)
                                                {
                                                    pl = DB.GetPlace(movplace.MovId);
                                                }
                                                try
                                                {
                                                    if (pl.Groupnr == 0)
                                                    {
                                                        List<MOVEMENT> movs = DB.GetMovementsByAniId(animal.AniId).FindAll(x => x.UbnId == rw1.UbnID_Destination || x.UbnId == 0);
                                                        if (movs.Count() > 0)
                                                        {
                                                            var OrderedMovs = movs.AsEnumerable().OrderBy(row => row.MovDate).ThenBy(row => row.MovOrder);
                                                            var oldplace = OrderedMovs.FirstOrDefault();
                                                            if (oldplace != null)
                                                            {
                                                                pl.Groupnr = oldplace.AG_Id;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception exc)
                                                {
                                                    unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: instellen Groupnr  error:{exc.ToString()}");
                                                }

                                                if (!string.IsNullOrEmpty(afdeling))
                                                {
                                                    try
                                                    {
                                                        var remarks = DB.getFarmRemarksByUbnId(rw1.UbnID_Destination).FindAll(x => x.LabKind == (int)LABELSConst.RemarkLabKind.afdeling && x.LabId == 1);
                                                        if (remarks == null)
                                                        {
                                                            remarks = new List<REMARK>();
                                                        }
                                                        int newremarkid = 1;
                                                        if (remarks.Count() > 0)
                                                        {
                                                            newremarkid = remarks.Max(x => x.RemId) + 1;
                                                        }
                                                        REMARK rem = new REMARK();
                                                        //var rem = remarks.FirstOrDefault(x => x.RemLabel == afdeling);
                                                        var remsearch = from n in remarks
                                                                        where n.RemLabel == afdeling
                                                                        select n;
                                                        if (remsearch.Count() > 0)
                                                        {
                                                            rem = remsearch.ElementAt(0);
                                                        }


                                                        if (rem.RemId == 0)
                                                        {
                                                            var r = new REMARK
                                                            {
                                                                //Men bedoelde hier afdeling
                                                                RemLabel = afdeling,
                                                                LabId = 1,
                                                                LabKind = (int)LABELSConst.RemarkLabKind.afdeling,
                                                                Farmid = _farmid,
                                                                UbnId = rw1.UbnID_Destination,
                                                                Changed_By = _ChangedBy,
                                                                SourceID = rw1.ThrID_Destination,
                                                                RemId = newremarkid

                                                            };
                                                            DB.SaveRemark(r);
                                                            pl.SectionNr = r.RemId;
                                                        }
                                                        else
                                                        {
                                                            pl.SectionNr = rem.RemId;
                                                        }
                                                        unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: pl.SectionNr:{pl.SectionNr} = Remlabel:{afdeling}");
                                                        sl.Omschrijving = $@"Place SectionNr:{pl.SectionNr}  Remlabel:{afdeling}";
                                                        DB.WriteSoapError(sl);
                                                        if (pl.MovId == 0)
                                                        {
                                                            MOVEMENT mnew = new MOVEMENT
                                                            {
                                                                AniId = animal.AniId,
                                                                UbnId = rw1.UbnID_Destination,
                                                                MovKind = 8,
                                                                MovDate = datum.Value.Date,
                                                                Changed_By = _ChangedBy
                                                            };
                                                            pl.AniId = animal.AniId;
                                                            pl.StableNr = 1;
                                                            DB.SaveMovement(mnew);
                                                            pl.MovId = mnew.MovId;
                                                            pl.Changed_By = _ChangedBy;
                                                            pl.FarmNumber = lUBN.Bedrijfsnummer;
                                                            DB.savePlace(pl);
                                                            unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: Insertmovid:{mnew.MovId} pl.SectionNr:{pl.SectionNr} = Remlabel:{afdeling}");

                                                        }
                                                        else
                                                        {
                                                            pl.Changed_By = _ChangedBy;
                                                            pl.FarmNumber = lUBN.Bedrijfsnummer;
                                                            DB.savePlace(pl);
                                                            unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: Updatemovid:{pl.MovId} pl.SectionNr:{pl.SectionNr} = Remlabel:{afdeling}");

                                                        }
                                                    }
                                                    catch (Exception exc)
                                                    {
                                                        unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: Error in saving place: {exc.ToString()}");
                                                    }
                                                }
                                                else
                                                {
                                                    unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}: no place found ");

                                                }

                                                var mBloods = DB.getEventsByAniIdKindUbn(animal.AniId, (int)LABELSConst.EventKind.BLOEDONDERZOEK, rw1.UbnID_Destination).FindAll(x => x.EveDate.Date == datum.Value.Date);
                                                if (!mBloods.Any())
                                                {
                                                    var ev = new EVENT
                                                    {
                                                        AniId = (int)animal.AniId,
                                                        ThirdId = rw1.ThrID_Destination,
                                                        UBNId = rw1.UbnID_Destination,
                                                        EveDate = datum.Value.Date,
                                                        EveKind = (int)LABELSConst.EventKind.BLOEDONDERZOEK,
                                                        Changed_By = _ChangedBy,
                                                        SourceID = rw1.FILE_IMPORT_ID,
                                                        EveOrder = 1
                                                    };
                                                    var bl = new BLOOD
                                                    {
                                                        BloKind = 0,

                                                        BloNumber = (short)BLOODBlnNumber,

                                                    };

                                                    double.TryParse(uitslag.Replace(",", "."), out double dUitslag);
                                                    bl.BloResult = dUitslag / 10;


                                                    if (DB.SaveEvent(ev))
                                                    {
                                                        bl.EventId = ev.EventId;
                                                        bl.SourceID = rw1.FILE_IMPORT_ID;
                                                        bl.Changed_By = ev.Changed_By;
                                                        DB.SaveBlood(bl);
                                                        Addbericht(ref berichten, u, rw1.UbnID_Destination, "Bloeduitslag ingelezen " + u.ThrStamboeknr, "Bloeduitslagen " + u.ThrStamboeknr);
                                                        unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}:insert uitslag:{uitslag} datum:{datum} eventid:{bl.EventId} ");
                                                        sl.Omschrijving = $@"Bloedwaarde :insert uitslag:{uitslag} datum:{datum} eventid:{bl.EventId} ";
                                                        DB.WriteSoapError(sl);
                                                        rw2.FI_State = 2;
                                                        DB.saveFile_Import(rw2);
                                                    }
                                                    else
                                                    {
                                                        unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}:  blood uitslag " + datum.Value.ToShortDateString() + " " + uitslag + " niet opgeslagen in agrobase: Error opslaan BLOOD Eventid:" + bl.EventId.ToString());
                                                        sl.Status = "F";
                                                        sl.Omschrijving += " niet opgeslagen in agrobase: Error opslaan BLOOD Eventid " + bl.EventId.ToString();
                                                        DB.WriteSoapError(sl);
                                                    }


                                                }
                                                else
                                                {
                                                    BLOOD bl = DB.GetBlood(mBloods.ElementAt(0).EventId);
                                                    bl.Changed_By = _ChangedBy;
                                                    bl.SourceID = rw1.FILE_IMPORT_ID;
                                                    double.TryParse(uitslag.Replace(",", "."), out double dUitslag);
                                                    double oud = bl.BloResult;
                                                    short oudnr = bl.BloNumber;
                                                    bl.BloResult = dUitslag / 10;
                                                    bl.BloNumber = (short)BLOODBlnNumber;
                                                    DB.SaveBlood(bl);
                                                    sl.Lifenumber = animal.AniAlternateNumber;
                                                    sl.FarmNumber = lUBN.Bedrijfsnummer;
                                                    sl.Omschrijving = $@"Bloedwaarde :update uitslag:{uitslag} datum:{datum} eventid:{bl.EventId}";
                                                    DB.WriteSoapError(sl);
                                                    unLogger.WriteInfo($@"Bloedwaarden:{animal.AniLifeNumber}:update uitslag:{uitslag} datum:{datum} eventid:{bl.EventId} ");
                                                }
                                            }
                                            else
                                            {
                                                sl.Status = "F";
                                                sl.Omschrijving += " kan datum " + row2[0] + " niet bepalen ";
                                                DB.WriteSoapError(sl);
                                            }
                                        }
                                        else
                                        {
                                            unLogger.WriteError(levensnummer + " kan niet bepaald worden ");
                                            sl.Status = "F";
                                            sl.Omschrijving += " onbekend levensnummer:" + levensnummer;
                                            DB.WriteSoapError(sl);
                                        }
                                        break;
                                    }
                                }
                                rw1.FI_State = 2;

                                DB.saveFile_Import(rw1);
                            }

                            //berichten afhandelen en totaal naar beheerder tMessageFrom
                            if (berichten.Count() > 0)
                            {
                                MESSAGES beheerderbericht = new MESSAGES();
                                foreach (var ub in berichten)
                                {
                                    ub.Item2.MesFromThrId = tMessageFrom.ThrId;
                                    ub.Item2.MesProgId = pAgroLinkProgId;
                                    ub.Item2.MesProgramID = -99;

                                    if (DB.SaveMessage(ub.Item2))
                                    {
                                        beheerderbericht.MesMessage += ub.Item1.ThrStamboeknr + Environment.NewLine + "<br />";
                                        beheerderbericht.MesMessage += ub.Item2.MesMessage + Environment.NewLine + "<br />";
                                    }
                                }
                                beheerderbericht.MesBegin_DateTime = DateTime.Now;
                                beheerderbericht.MesEnd_DateTime = DateTime.Now.AddDays(7);
                                beheerderbericht.MesFromThrId = tMessageFrom.ThrId;
                                beheerderbericht.MesProgramID = -99;

                                beheerderbericht.MesProgId = pAgroLinkProgId;
                                beheerderbericht.MesSubject = "Bloeduitslagen ingelezen ";
                                beheerderbericht.MesThrId = tMessageFrom.ThrId;
                                beheerderbericht.MesUbnId = 0;
                                string MesVersion = Guid.NewGuid().ToString();
                                beheerderbericht.MesVersion = MesVersion;
                                beheerderbericht.MesState = 1;
                                //_messages.InsertMessages(_token, beheerderbericht);
                            }

                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteError("VSM.RUMA.SRV.FILEREADER.SOPRACO savebloodindatabase" + exc.ToString());

                            SOAPLOG slerr = new SOAPLOG
                            {
                                Changed_By = _ChangedBy,
                                Code = "",
                                Date = DateTime.Now.Date,
                                FarmNumber = System.IO.Path.GetFileName(pFilename),
                                Kind = Soapkind,
                                Lifenumber = "",
                                SourceID = 0,
                                Status = "F",
                                Time = DateTime.Now
                            };
                            slerr.Omschrijving += exc.Message;
                            DB.WriteSoapError(slerr);
                        }
                    }
                    
                
                }
                else
                {
                    unLogger.WriteError("TegenBestand nog niet ingelezen van " + pFilename);
                }

            }

        }

        public ANIMAL findAnimal(string levensnummer, List<ANIMAL> animals, List<COUNTRY> countries, string ubn)
        {
            ANIMAL animal = new ANIMAL();
            try
            {
                unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer}");
                if (!string.IsNullOrEmpty(levensnummer))
                {
                    if (levensnummer.Length >= 9)
                    {
                        for (int i = levensnummer.Length; i >= 9; i--)
                        {
                            var b = from n in animals where n.AniLifeNumber.Contains(levensnummer.Substring(i - 9, 9)) select n;
                            if (b != null && b.Count() > 0)
                            {
                                animal = b.ElementAt(0);
                                unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: found  {levensnummer} == {animal.AniLifeNumber} ");
                                break;
                            }
                        }
                        if (animal.AniId > 0)
                        {
                            return animal;
                        }
                    }
                }
                string deflandcode1 = levensnummer.Substring(0, 3);
                string deflandcode2 = levensnummer.Substring(0, 4);
                string levnr = "";
                if (deflandcode1 == "000")
                {
                    deflandcode1 = "056";
                }
                levnr = levensnummer.Substring(3, levensnummer.Length - 3);
                if (levensnummer.StartsWith("0056000"))
                {
                    unLogger.WriteDebug($@"determine 0056000 levensnummer:{levensnummer}");
                    deflandcode1 = "056";
                    levnr = levensnummer.Substring(7, levensnummer.Length - 7);
                }
                int[] goodcountriesbelow100 = { 20, 32, 36, 40, 56, 76 };
                // 0124 actualy canada but 012 gives you algeria
                // maybe start with first 4 but then it is the other way around.
                // it gives you canada but you needed algeria.
                var c = from n in countries where n.LandNummer == int.Parse(deflandcode1) && goodcountriesbelow100.Contains(int.Parse(deflandcode1)) select n;

                if (c.Count() == 0)
                {
                    unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer} {deflandcode1} 1e c.Count() == 0 ");

                    c = from n in countries where n.LandNummer == int.Parse(deflandcode2) select n;
                    levnr = levensnummer.Substring(4, levensnummer.Length - 4);

                    if (c.Count() == 0)
                    {
                        //find animal
                        unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer} {deflandcode2}  levnr:{levnr}  not found at ubn:{ubn} ");
    
                    }
                    else
                    {
                        unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer}  2e: {c.ElementAt(0).LandAfk2} {levnr} ");

                        var a = from n in animals where n.AniLifeNumber== c.ElementAt(0).LandAfk2+ " " + levnr select n;
                        if (a != null && a.Count() > 0)
                        {
                            animal = a.ElementAt(0); ;
                        }
                        else
                        {
                            unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer}  2e: not found at ubn:{ubn} ");
                        }
                    }

                }
                else
                {
                    unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer}  1e: {c.ElementAt(0).LandAfk2} {levnr} ");

                    var a = from n in animals where n.AniLifeNumber == c.ElementAt(0).LandAfk2 + " " + levnr select n;
                    if (a != null && a.Count() > 0)
                    {
                        animal = a.ElementAt(0); ;
                    }
                    else
                    {
                        unLogger.WriteInfo($@"VSM.RUMA.SRV.FILEREADER.SOPRACO findAnimal: {levensnummer}  1e: not found at ubn:{ubn} ");

                    }
                }

            }
            catch (Exception exc)
            {
                unLogger.WriteError("VSM.RUMA.SRV.FILEREADER.SOPRACO findanimal  levensnummer:" + levensnummer + " ERR:" + exc.ToString());
              
            }
            return animal;
        }

        private int Getfarmid(int ubnID)
        {
            List<BEDRIJF> b = DB.GetBedrijfByUbnIdProgId(ubnID,progid).ToList();
            if (b.Any())
            {
                return b.ElementAt(0).FarmId;
            }
            return 0;
        }
   
        public void Saveslachtdata(FILE_IMPORT_Filekind fileKind,string bestandsnaam, List<FILE_IMPORT> imports, int pAgroLinkProgId = 0)
        {
            unLogger.WriteInfo("Saveslachtdata:" + bestandsnaam);
       
            bestandsnaam = System.IO.Path.GetFileNameWithoutExtension(bestandsnaam);
            var slachtdata = DB.GetFile_ImportsByName(bestandsnaam + ".csv", (int)fileKind);
            if (slachtdata == null || slachtdata.Count() == 0)
            {
                slachtdata = DB.GetFile_ImportsByName(bestandsnaam + ".txt", (int)fileKind);
            }
            if (slachtdata == null || slachtdata.Count() == 0)
            {
                if (imports.Count > 0)
                {
                    slachtdata = imports;
                }
                else
                {
                    unLogger.WriteError("Geen slachtdata gevonden in FILE_IMPORT bestand:" + bestandsnaam + " fileKind:" + fileKind.ToString());
                    return;
                }
            }
            var relations = DB.GetAgrolinkRelations("Beheerder", pAgroLinkProgId);
            THIRD tMessageFrom = new THIRD();
            if (relations != null)
            {
                tMessageFrom = relations.ElementAt(0);
            }
          
            List<Tuple<THIRD, MESSAGES>> berichten = new List<Tuple<THIRD, MESSAGES>>();

            if (fileKind == FILE_IMPORT_Filekind.SlachtdataSarreguemines)
            {

                SaveSlachtdataSarreguemines(slachtdata.ToList(), ref berichten);

            }
            else
            {
                Saveslachtdata(slachtdata.ToList(), ref berichten);
            }

            //berichten afhandelen
            if (berichten.Count() > 0)
            {
                MESSAGES beheerderbericht = new MESSAGES();
                foreach (var ub in berichten)
                {
                    ub.Item2.MesFromThrId = tMessageFrom.ThrId;
                    ub.Item2.MesProgId = pAgroLinkProgId;
                    ub.Item2.MesProgramID = -99;

                    if (DB.SaveMessage(ub.Item2))
                    {
                        beheerderbericht.MesMessage += ub.Item1.ThrStamboeknr + Environment.NewLine + "<br />";
                        beheerderbericht.MesMessage += ub.Item2.MesMessage + Environment.NewLine + "<br />";
                    }
                }
                beheerderbericht.MesBegin_DateTime = DateTime.Now;
                beheerderbericht.MesEnd_DateTime = DateTime.Now.AddDays(7);
                beheerderbericht.MesFromThrId = tMessageFrom.ThrId;
                beheerderbericht.MesProgramID = -99;

                beheerderbericht.MesProgId = pAgroLinkProgId;
                beheerderbericht.MesSubject = "Slachtbestand ingelezen " + bestandsnaam;
                beheerderbericht.MesThrId = tMessageFrom.ThrId;
                beheerderbericht.MesUbnId = 0;// tMessageFrom.UbnId ?? 0;
                string MesVersion = Guid.NewGuid().ToString();
                beheerderbericht.MesVersion = MesVersion;
                beheerderbericht.MesState = 1;
                //_messages.InsertMessages(_token, beheerderbericht);
            }
        }
   
        private void SaveSlachtdataSarreguemines(List<FILE_IMPORT> fiimports, ref List<Tuple<THIRD, MESSAGES>> berichten)
        {
            int subkind = 6015;
            //const string DierAfgekeurd = "3";
            //const string DierDoodOpSlachterij = "4";
            string retbericht = "";

            try
            {
                // 
                //ID slachthuis; Slachtdatum; Mesternummer; Slachtnummer; Levensnummer; Geslacht gewicht in 0,01 kg; Kwaliteit; Vet; Kleur;?; Geboortedatum; 2de positie kwaliteit
                //57631300; 28012019; 80049; 51001; BE661819486; 20286; E; 2; 2; ; ;= 
                var ubns = DB.getUBNsByThirdIDs(fiimports.Select(x => x.ThrID_Destination).ToList());
                var relations = DB.GetThirdsByThirdIds(fiimports.Select(x => x.ThrID_Destination).ToList());
                 THIRD vervoerder = DB.GetAgrolinkRelations("Vervoerder", 82).FirstOrDefault();
                var destionations = DB.getThirdPropertys("erkenningsnummer");
                List<SALE> oldsales = new List<SALE>();
                List<MOVEMENT> oldsalesmovs = new List<MOVEMENT>();
                List<SALE> newsales = new List<SALE>();
                List<MOVEMENT> newsalesmovs = new List<MOVEMENT>();
                bool binlezenafvoeren = false;
                string inlezenafvoeren = DB.GetConfigValue(82, 0, "SlachtdataSarregueminesAfvoeren");
                if (!string.IsNullOrEmpty(inlezenafvoeren))
                {
                    binlezenafvoeren = Convert.ToBoolean(inlezenafvoeren);
                     
                }
                SOAPLOG sl = new SOAPLOG
                {
                    Kind = Soapkind,
                    SubKind = subkind,
                    Code = "",
                    Date = DateTime.Now.Date,
                    FarmNumber = ubns.FirstOrDefault().Bedrijfsnummer,
                    Omschrijving = "saveSlachtdataSarreguemines",
                    Status = "G",
                    Time = DateTime.Now
                };
                if (!binlezenafvoeren)
                {
                    string bestandsnaam = fiimports.Count() > 0 ? fiimports.ElementAt(0).FI_Filename : "Onbekend";
                    sl.Omschrijving = $@"Slachtdata Sarreguemines ingelezen. Bestand:{bestandsnaam} Aantal({fiimports.Count()})";
                    DB.WriteSoapError(sl);
                    foreach (FILE_IMPORT fi in fiimports)
                    {
                       
                      
                        var t = relations.FirstOrDefault(x => x.ThrId == fi.ThrID_Destination);
                        Addbericht(ref berichten, t, fi.UbnID_Destination, t.ThrStamboeknr + Environment.NewLine + $@"Slachtdata Sarreguemines ingelezen. Bestand:{fi.FI_Filename} Aantal:{fiimports.Count()}", "Slachtdata Sarreguemines:" + t.ThrStamboeknr);
                   
                    }
                }
                else
                {
                    foreach (FILE_IMPORT fi in fiimports)
                    {
                      

                        try
                        {
                            if (fi.File_Import_Type_ID == (int)FILE_IMPORT_Filekind.SlachtdataSarreguemines)
                            {
                                string[] file = fi.FI_Data_Row.Split(splitters);

                                var ubnsbb = ubns.FirstOrDefault(x => x.UBNid == fi.UbnID_Destination);
                                destionations.FirstOrDefault(x => x.TP_Value == file[0]);
                                THIRD Slachtbedrijf_Destination = DB.GetAgrolinkRelations("Export", 82).FirstOrDefault(x => x.ThrCompanyName.Contains("Sarreguemines"));
                                if (destionations != null && destionations.Count() > 0)
                                {
                                    Slachtbedrijf_Destination = DB.GetThirdByThirId(destionations.ElementAt(0).ThrID);
                                }
                                var t = relations.FirstOrDefault(x => x.ThrId == fi.ThrID_Destination);

                                if (Slachtbedrijf_Destination == null || Slachtbedrijf_Destination.ThrId == 0)
                                {
                                    unLogger.WriteInfo("Slachtdata:  van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " Kan Agrolink Slachtbedrijf niet bepalen van ProgramId:82 ");
                                    sl.Status = "F";
                                    sl.Omschrijving = "Slachtdata:  van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " Kan Agrolink Slachtbedrijf niet bepalen van ProgramId:82";
                                    DB.WriteSoapError(sl);
                                    continue;
                                }
                                sl.ThrId = Slachtbedrijf_Destination.ThrId;
                                if (!oldsalesmovs.Any(x =>  x.UbnId == ubnsbb.UBNid))
                                {
                                    var sls = DB.GetMovementsOnlyByUbn(ubnsbb.UBNid).FindAll(x => x.MovKind == 2);
                                    oldsalesmovs.AddRange(sls);
                                }
                                if (t.ThrId > 0 && ubnsbb.UBNid > 0)
                                {
                                    sl.FarmNumber = ubnsbb.Bedrijfsnummer;

                                    DateTime datum = utils.getDateSlacht(file[1]);
                                    string countrycode = file[4].Substring(0, 2);
                                    string levnr = file[4].Substring(2, file[4].Length - 2);
                                    string Slachtnummer = file[3];
                                    ANIMAL a = DB.GetAnimalByAniAlternateNumber(countrycode + " " + levnr);
                                    //Nu moet bij het inlezen de afvoer aangemaakt worden
                                    sl.Lifenumber = countrycode + " " + levnr;
                                    if (a != null && a.AniId > 0)
                                    {
                                        ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandUbnid(a.AniId, fi.UbnID_Destination);
                                        if (ac.AniId > 0 && ac.Anicategory <= 4)
                                        {

                                        }
                                        else
                                        {
                                            unLogger.WriteError($@"VSM.RUMA.SRV.FILEREADER.SOPRACO SlachtdataSarreguemines {a.AniAlternateNumber} onbekend bij UBN id:{fi.UbnID_Destination}");
                                            sl.Status = "F";
                                            sl.Omschrijving = $@" {a.AniAlternateNumber} onbekend bij UBN id:{fi.UbnID_Destination} ";
                                            DB.WriteSoapError(sl);
                                            continue;
                                        }
                                        sl.Lifenumber = a.AniAlternateNumber;
                                        MOVEMENT m = new MOVEMENT
                                        {
                                            AniId = a.AniId,
                                            MovDate = datum.Date,
                                            UbnId = fi.UbnID_Destination,
                                            ThrId = fi.ThrID_Origin,
                                            MovKind = 2,
                                            Changed_By=_ChangedBy
                                        };
                                        SALE s = new SALE
                                        {
                                            SalKind = (int)LABELSConst.SaleKind.Export,

                                            SalReason = 5,// SaleReason;// SaleReasons.FirstOrDefault(x => x.Key == AniId)?.Value ?? 0;
                                            SalSlaughter = 1,

                                            Changed_By = _ChangedBy,
                                            SalDestination = Slachtbedrijf_Destination.ThrId,
                                            SalTransporter = vervoerder.ThrId
                                        };
                                        if (string.IsNullOrEmpty(vervoerder.ThrVATNumber))
                                        {
                                            unLogger.WriteDebug($@"saveSlachtdataSarreguemines: vervoerder.ThrVATNumber niet bekend ");
                                        }
                                      
                                        List<MOVEMENT> movs = DB.GetMovementsByAniIdMovkindUbn(a.AniId, 2, fi.UbnID_Destination);
                                        var check = movs.FindAll(x=>x.MovDate.Date == datum.Date);
                                        unLogger.WriteInfo($"Number of sale movements for {a.AniAlternateNumber} :{check.Count() } ");
                                        if (check.Count() > 0)
                                        {
                                            m = check.ElementAt(0);
                                            s = DB.GetSale(m.MovId);
                                            s.SalKind = (int)LABELSConst.SaleKind.Export;
                                            s.SalReason = 5;
                                            s.SalDestination = Slachtbedrijf_Destination.ThrId;
                                        }

                                        m.MovComment = "Slachtnummer:" + Slachtnummer + Environment.NewLine;// + " kleur/kwaliteit/Vet:" + file[7];


                                        //////////////////////////////////////////////////

                                        if (!double.TryParse(file[5].Replace(".", ","), out double gewicht))
                                        {
                                            gewicht = 0;
                                        }
                                        gewicht /= 100;
                                        s.SalCarcassWeight = gewicht;
                                        s.SalAliveWeight = Event_functions.getCalculatedSalAliveWeight(_token, 82, 0, gewicht);

                                        string Kwaliteit = file[6].Trim();  //E; 2; 2; ; 
                                        string Vet = file[7].Trim();
                                        string Kleur = file[8].Trim();
                                        string tweededepositiekwaliteit = file[11].Trim();// ;= 
                                        string code = Kwaliteit + tweededepositiekwaliteit.Replace("=", "");
                                        GetClassificationMeatAndDeviation(code, out int classification, out int Deviation);

                                        s.SalClassificationMeat = (byte)classification;
                                        s.SalDeviationMeat = (byte)Deviation;
                                        try
                                        {
                                            //eerste getal
                                            s.SallColorMeat = string.IsNullOrEmpty(Kleur) ? 0 : int.Parse(Kleur);
                                        }
                                        catch
                                        {
                                            unLogger.WriteError($@"Error:saveSlachtdataSarreguemines FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  SallColorMeat {Kleur}");
                                        }
                                        try
                                        {
                                            //laatste getal

                                            s.SalClassificationFat = string.IsNullOrEmpty(Vet) ? 0 : int.Parse(Vet);
                                        }
                                        catch (Exception exc)
                                        {
                                            unLogger.WriteError($@"Error: saveSlachtdataSarreguemines FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  SalClassificationFat {Vet} {exc.ToString()}");
                                        }


                                        if (m.MovId > 0)
                                        {
                                            //TODO: check UpdateSale failed
                                            s.MovId = m.MovId;
                                            unLogger.WriteInfo($@"Update: saveSlachtdataSarreguemines FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  Sale Movid={m.MovId}");
                                            sl.Omschrijving = $@"Slachtdata update {a.AniAlternateNumber} FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + $@"{code}={s.SalClassificationMeat}:{s.SalDeviationMeat}";
                                            DB.SaveSale(s);
                                            var mlg = DB.GetMutaLogByMovId(m.MovId);
                                            if (mlg.Internalnr <= 0)
                                            {
                                                var mut = DB.GetMutationByMovId(m.MovId);
                                                if (mut.Internalnr <= 0)
                                                {
                                                    BEDRIJF b = new BEDRIJF
                                                    {
                                                        ProgId = 1,
                                                        Programid = 1,
                                                        UBNid = fi.UbnID_Destination
                                                    };
                                                    List<BEDRIJF> bedr = DB.getBedrijvenByUBNId(fi.UbnID_Destination);
                                                    if (bedr.Count() > 0)
                                                    {
                                                        b = bedr.FirstOrDefault(x => x.ProgId != 3 && x.ProgId != 5);
                                                        if (b == null)
                                                        {
                                                            b = new BEDRIJF
                                                            {
                                                                ProgId = 1,
                                                                Programid = 1,
                                                                UBNid = fi.UbnID_Destination
                                                            };
                                                        }
                                                    }

                                                    string inlezen = DB.GetConfigValue(82, 0, "SlachtdataSarregueminesAfvmelding");
                                                    if (!string.IsNullOrEmpty(inlezen))
                                                    {
                                                        if (Convert.ToBoolean(inlezen) == true)
                                                        {
                                                            MovFunc.saveAfvoerMutation(_token, b, a, m, s);
                                                        }
                                                        else
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            newsales.Clear();
                                            newsales.Add(s);
                                            //TODO: check AddSales failed
                                            m.Changed_By = _ChangedBy;
                                            s.Changed_By = _ChangedBy;
                                            m.SourceID = fi.FILE_IMPORT_ID;
                                            s.SourceID = fi.FILE_IMPORT_ID;
                                            if (DB.SaveMovement(m))
                                            {
                                                s.MovId = m.MovId;
                                                unLogger.WriteInfo($@"Insert: saveSlachtdataSarreguemines FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  Sale Movid={m.MovId}");
                                                sl.Omschrijving = $@"Slachtdata insert {a.AniAlternateNumber} FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + $@"{code}={s.SalClassificationMeat}:{s.SalDeviationMeat}"; ;
                                                if (DB.SaveSale(s))
                                                {
                                                    BEDRIJF b = new BEDRIJF
                                                    {
                                                        ProgId = 1,
                                                        Programid = 1,
                                                        UBNid = fi.UbnID_Destination
                                                    };
                                                    List<BEDRIJF> bedr = DB.getBedrijvenByUBNId(fi.UbnID_Destination);
                                                    if (bedr.Count() > 0)
                                                    {
                                                        b = bedr.FirstOrDefault(x => x.ProgId != 3 && x.ProgId != 5);
                                                        if (b == null)
                                                        {
                                                            b = new BEDRIJF
                                                            {
                                                                ProgId = 1,
                                                                Programid = 1,
                                                                UBNid = fi.UbnID_Destination
                                                            };
                                                        }
                                                    }
                                                    string inlezen = DB.GetConfigValue(82, 0, "SlachtdataSarregueminesAfvmelding");
                                                    if (!string.IsNullOrEmpty(inlezen))
                                                    {
                                                        if (Convert.ToBoolean(inlezen) == true)
                                                        {
                                                            MovFunc.saveAfvoerMutation(_token, b, a, m, s);
                                                        }
                                                        else
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                    Addbericht(ref berichten, t, fi.UbnID_Destination, t.ThrStamboeknr + Environment.NewLine + "Er staan nieuwe afvoermeldingen voor u klaar. ", "Slachtdata en afvoermeldingen:" + t.ThrStamboeknr);
                                                   
                                                    if (ac.Anicategory < 4)
                                                    {
                                                        unLogger.WriteInfo($@"{a.AniLifeNumber} Update Anicategory from {ac.Anicategory} to 4 UBNid:{m.UbnId} ");
                                                        ac.Anicategory = 4;
                                                        ac.Changed_By = _ChangedBy;
                                                        ac.SourceID = m.ThrId;
                                                        ac.AniId = m.AniId;
                                                        ac.UbnId = m.UbnId;
                                                        DB.SaveAnimalCategory(ac);
                                                    }
                                                    unLogger.WriteInfo("Slachtdata: ingelezen van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " dier:" + countrycode + " " + levnr);
                                                    retbericht = "Slachtdata ingelezen dier:" + file[2];
                                                    
                                                    sl.Lifenumber = a.AniAlternateNumber;
                                                }
                                                else
                                                {
                                                    sl.Status = "F";
                                                    sl.Omschrijving = "Kan geen Afvoerdetails opslaan";
                                                }
                                            }
                                            else
                                            {
                                                sl.Status = "F";
                                                sl.Omschrijving = "Kan geen Afvoer opslaan";
                                            }


                                        }


                                        DB.WriteSoapError(sl);

                                    }
                                    else
                                    {
                                        retbericht = "Onbekend dier:" + countrycode + " " + levnr;
                                        unLogger.WriteError("Sopracoslachtdata onbekend dier " + countrycode + " " + levnr);
                                        sl.Status = "F";
                                        sl.Omschrijving = "Onbekend dier FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " dier " + countrycode + " " + levnr;
                                        DB.WriteSoapError(sl);
                                    }
                                    //addbericht(ref berichten, u, retbericht, "Slachtdata ingelezen");
                                    //addbericht(ref berichten, u, "Slachtdata ingelezen: " + u.ThrStamboeknr, "Slachtdata " + u.ThrStamboeknr);
                                }
                                else
                                {
                                    unLogger.WriteInfo("Sopracoslachtdata onbekende bestemming FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString());
                                    sl.Status = "F";
                                    sl.Omschrijving = "Kan bedrijf niet bepalen  FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString();
                                    DB.WriteSoapError(sl);
                                }
                            }
                        }
                        catch (Exception exc) { unLogger.WriteError("Filebuffersync inlezenslachtdata FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " ERROR:" + exc.ToString()); }
                    }
                }
            }
            catch (Exception exc) { unLogger.WriteError("Filebuffersync inlezenslachtdata FILE_IMPORT_ID: ERROR:" + exc.ToString()); }
        }
   
        internal void Saveslachtdata(List<FILE_IMPORT> fiimports, ref List<Tuple<THIRD, MESSAGES>> berichten)
        {
            int subkind = 6012;
            const string DierAfgekeurd = "3";
            const string DierDoodOpSlachterij = "4";
            string retbericht = "";
            // bestand van slachthuis geel
            try
            {
                //Slachtdatum;Slachtuur;Levensnummer;Slachtgewicht;Slachtnummer;Fokkerijnummer;Mester                                  ;kleur/kwaliteit/Vet;geboortedatum;negeren;Negeren;inrichtingsnumemr;Fokkerijnummer
                //26.06.17   ;8:02:30  ;BE58728417  ;117,12       ;787480      ;80075         ;FRISO BRUURS I BVBA                     ;1P2                ;25.12.16     ;Halal  ;2      ;10173874         ;80075

                
                    var ubns = DB.getUBNsByThirdIDs(fiimports.Select(x => x.ThrID_Destination).ToList());
                    var relations = DB.GetThirdsByThirdIds(fiimports.Select(x => x.ThrID_Destination).ToList());
                    THIRD Slachtbedrijf_Destination = DB.GetAgrolinkRelations("Slachtbedrijf", 82).FirstOrDefault();
                
       
                foreach (FILE_IMPORT fi in fiimports)
                {
                    SOAPLOG sl = new SOAPLOG
                    {
                        Kind = Soapkind,
                        SubKind = subkind,
                        Code = "",
                        Date = DateTime.Now.Date,
                        FarmNumber = ubns.FirstOrDefault().Bedrijfsnummer,
                        Omschrijving = "Saveslachtdata",
                        Status = "G",
                        ThrId = Slachtbedrijf_Destination.ThrId,
                        Time = DateTime.Now,
                        Changed_By=_ChangedBy,
                         SourceID=fi.FILE_IMPORT_ID 
                    };

                    try
                    {
                        if (fi.File_Import_Type_ID == (int)FILE_IMPORT_Filekind.SlachtdataSopraco)
                        {
                             var ubnsbb = ubns.FirstOrDefault(x => x.UBNid == fi.UbnID_Destination);
                             

                            var t = relations.FirstOrDefault(x => x.ThrId == fi.ThrID_Destination);
                            
                            if (Slachtbedrijf_Destination == null || Slachtbedrijf_Destination.ThrId == 0)
                            {
                                unLogger.WriteInfo("Slachtdata:  van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " Kan Agrolink Slachtbedrijf niet bepalen van ProgramId:82 ");
                                sl.Status = "F";
                                sl.Omschrijving = "Slachtdata:  van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " Kan Agrolink Slachtbedrijf niet bepalen van ProgramId:82";
                                DB.WriteSoapError(sl);
                                continue;
                            }
                            
                            if (t.ThrId > 0 && ubnsbb.UBNid > 0)
                            {

                                string[] file = fi.FI_Data_Row.Split(splitters);
                                DateTime? datum = GetDateFormat(file[0], file[1]);
                                string countrycode = file[2].Substring(0, 2);
                                string levnr = file[2].Substring(2, file[2].Length - 2);
                                string Slachtnummer = file[4];
                                sl.Lifenumber = countrycode + " " + levnr;
                                ANIMAL a = DB.GetAnimalByAniAlternateNumber(countrycode + " " + levnr);
                                //Nu moet bij het inlezen de afvoer aangemaakt worden
                              
                                if (a != null && a.AniId > 0)
                                {
                                    ANIMALCATEGORY ac = DB.GetAnimalCategoryByIdandUbnid(a.AniId, fi.UbnID_Destination);
                                    if (ac.AniId > 0 && ac.Anicategory <= 4)
                                    {

                                    }
                                    else
                                    {
                                        unLogger.WriteError($@"VSM.RUMA.SRV.FILEREADER.SOPRACO saveslachtdata {a.AniAlternateNumber} onbekend bij UBN id:{fi.UbnID_Destination}");
                                        sl.Status = "F";
                                        sl.Omschrijving = $@" {a.AniAlternateNumber} onbekend bij UBN id:{fi.UbnID_Destination} ";
                                        DB.WriteSoapError(sl);
                                        continue;
                                    }
                                    sl.Lifenumber = a.AniAlternateNumber;

                                    MOVEMENT m = new MOVEMENT
                                    {
                                        AniId = a.AniId,
                                        MovDate = datum.Value.Date,
                                        UbnId = fi.UbnID_Destination,
                                        ThrId = fi.ThrID_Origin,
                                        MovKind = 2,
                                        Changed_By = _ChangedBy
                                    };
                                    SALE s = new SALE
                                    {
                                        SalKind = (int)LABELSConst.SaleKind.Slaughter,

                                        SalReason = 5,// SaleReason;// SaleReasons.FirstOrDefault(x => x.Key == AniId)?.Value ?? 0;
                                        SalSlaughter = 1,

                                        Changed_By = _ChangedBy,
                                        SalDestination = Slachtbedrijf_Destination.ThrId
                                    };


                                    List<MOVEMENT> movs = DB.GetMovementsByAniIdMovkindUbn(a.AniId, 2, fi.UbnID_Destination);
                                    var check = movs.FindAll(x => x.MovDate.Date == datum.Value.Date);
                                    unLogger.WriteInfo($"Number of sale movements for {a.AniAlternateNumber}:{check.Count() } ");
                                    if (check.Count() > 0)
                                    {
                                        m = check.ElementAt(0);
                                        s = DB.GetSale(m.MovId);
                                        s.SalKind = (int)LABELSConst.SaleKind.Slaughter;
                                        s.SalReason = 5;
                                        s.SalDestination = Slachtbedrijf_Destination.ThrId;
                                    }

                                    m.MovComment = "Slachtnummer:" + Slachtnummer + Environment.NewLine;// + " kleur/kwaliteit/Vet:" + file[7];


                                    //////////////////////////////////////////////////

                                    if (!double.TryParse(file[3].Replace(".", ","), out double gewicht))
                                    {
                                        gewicht = 0;
                                    }

                                    s.SalCarcassWeight = gewicht;
                                    s.SalAliveWeight = Event_functions.getCalculatedSalAliveWeight(_token, 82, 0, gewicht);

                                    string code = file[7].Trim();

                                    GetClassificationMeatAndDeviation(code, out int classification, out int Deviation);

                                    s.SalClassificationMeat = (byte)classification;
                                    s.SalDeviationMeat = (byte)Deviation;
                                    try
                                    {
                                        //eerste getal
                                        s.SallColorMeat = code.Length >= 1 ? int.Parse(code.Substring(0, 1)) : 0;
                                    }
                                    catch
                                    {
                                        unLogger.WriteError($@"Error: FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  SallColorMeat {code}");
                                    }
                                    try
                                    {
                                        //laatste getal
                                        
                                        string eind = code.Substring(code.Length - 1, 1);
                                
                                        s.SalClassificationFat = string.IsNullOrEmpty(eind) ? 0:int.Parse(eind);
                                    }
                                    catch(Exception exc)
                                    {
                                        unLogger.WriteError($@"Error: FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  SalClassificationFat {code} {exc.ToString()}");
                                    }
                                    //Veld 9 en 10 waren voor halal, wordt nu afkeur en dood bij slachterij opgeslagen, gewicht en kwaliteit moeten leeg zijn bij afkeur of dood.
                                    if (file[10] == DierAfgekeurd)
                                    {
                                        s.SalCarcassWeight = 0;
                                        s.SalClassificationMeat = 0;
                                        s.SalDeviationMeat = 0;
                                        s.SalKeuringKalf = (int)LABELSConst.SalKeuringKalf.afkeur;
                                    }

                                   
                                    if (file[10] == DierDoodOpSlachterij)
                                    {
                                        s.SalCarcassWeight = 0;
                                        s.SalClassificationMeat = 0;
                                        s.SalDeviationMeat = 0;
                                        s.SalKeuringKalf = (int)LABELSConst.SalKeuringKalf.doodaangevoerd;
                                    }



                                    if (m.MovId > 0)
                                    {
                                        //TODO: check UpdateSale failed
                                        s.MovId = m.MovId;
                                        unLogger.WriteInfo($@"Update: saveSlachtdata Geel Animal:{a.AniAlternateNumber} FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  Sale Movid={m.MovId}");
                                        DB.SaveSale(s);
                                        sl.Omschrijving = $@"Slachtdata update {a.AniAlternateNumber} FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + $@"{code}={s.SalClassificationMeat}:{s.SalDeviationMeat}";
                                    }
                                    else
                                    {
                                    
                                        //TODO: check AddSales failed
                                        m.Changed_By = _ChangedBy;
                                        s.Changed_By = _ChangedBy;
                                        m.SourceID = fi.FILE_IMPORT_ID;
                                        s.SourceID = fi.FILE_IMPORT_ID;
                                        //ret = CORE.MovFunc.opslaan_afvoer(fi.ThrID_Destination, fi.UbnID_Destination, progid, programid, _token, m, s, _ChangedBy, fi.ThrID_Origin);
                                        if (DB.SaveMovement(m))
                                        {
                                            s.MovId = m.MovId;
                                
                                            unLogger.WriteInfo($@"Insert: saveSlachtdata Geel Animal:{a.AniAlternateNumber} FILE_IMPORT_ID:{fi.FILE_IMPORT_ID}  Sale Movid={m.MovId}");

                                            if (DB.SaveSale(s))
                                            {
                                                BEDRIJF b = new BEDRIJF
                                                {
                                                    ProgId = 1,
                                                    Programid = 1,
                                                    UBNid = fi.UbnID_Destination
                                                };
                                                List<BEDRIJF> bedr = DB.getBedrijvenByUBNId(fi.UbnID_Destination);
                                                if (bedr.Count() > 0)
                                                {
                                                    b = bedr.FirstOrDefault(x => x.ProgId != 3 && x.ProgId != 5);
                                                    if (b == null)
                                                    {
                                                        b = new BEDRIJF
                                                        {
                                                            ProgId = 1,
                                                            Programid = 1,
                                                            UBNid = fi.UbnID_Destination
                                                        };
                                                    }
                                                }

                                                MovFunc.saveAfvoerMutation(_token, b, a, m, s);
                                                Addbericht(ref berichten, t, fi.UbnID_Destination, t.ThrStamboeknr + Environment.NewLine + "Er staan nieuwe afvoermeldingen voor u klaar. ", "Slachtdata en afvoermeldingen:" + t.ThrStamboeknr);
                                                ANIMALCATEGORY acc = DB.GetAnimalCategoryByIdandUbnid(m.AniId, m.UbnId);
                                                if (acc.Anicategory < 4)
                                                {
                                                    unLogger.WriteInfo($@"{a.AniLifeNumber} Update Anicategory from {acc.Anicategory} to 4 UBNid:{m.UbnId} ");
                                                    acc.Anicategory = 4;
                                                    acc.Changed_By = _ChangedBy;
                                                    acc.SourceID = m.ThrId;
                                                    acc.AniId = m.AniId;
                                                    acc.UbnId = m.UbnId;
                                                    DB.SaveAnimalCategory(acc);
                                                }
                                                unLogger.WriteInfo("Slachtdata Geel: ingelezen van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " dier:" + countrycode + " " + levnr);
                                                retbericht = "Slachtdata ingelezen dier:" + file[2];
                                                sl.Omschrijving = $@"Slachtdata insert {a.AniAlternateNumber} FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + $@"{code}={s.SalClassificationMeat}:{s.SalDeviationMeat}";
                                                sl.Lifenumber = a.AniAlternateNumber;
                                            }
                                            else
                                            {
                                                sl.Status = "F";
                                                sl.Omschrijving = "Kan geen Afvoerdetails opslaan";
                                            }
                                            fi.FI_State = (int)FILE_IMPORT_STATUS.Bevestigd;
                                            DB.saveFile_Import(fi);
                                            
                                        }
                                        else
                                        {
                                            sl.Status = "F";
                                            sl.Omschrijving = "Kan geen Afvoer opslaan";
                                        }
                                        Addbericht(ref berichten, t, fi.UbnID_Destination ,t.ThrStamboeknr + Environment.NewLine + "Er staan nieuwe afvoermeldingen voor u klaar. ", "Slachtdata en afvoermeldingen:" + t.ThrStamboeknr);

                                    }
                               
                                    if (ac.Anicategory < 4)
                                    {
                                        unLogger.WriteInfo($@"{a.AniLifeNumber} Update Anicategory from {ac.Anicategory} to 4 UBNid:{m.UbnId} ");
                                        ac.Anicategory = 4;
                                        ac.AniId = m.AniId;
                                        ac.UbnId = m.UbnId;
                                        ac.FarmId = Getfarmid(m.UbnId);
                                        ac.Changed_By = _ChangedBy;
                                        ac.SourceID = m.ThrId;
                                        DB.SaveAnimalCategory(ac);
                                    }
                                    unLogger.WriteInfo("Slachtdata: ingelezen van FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " dier:" + file[2]);
                                    retbericht = "Slachtdata ingelezen dier:" + file[2];
                                    
                                    DB.WriteSoapError(sl);
                                   
                                }
                                else
                                {
                                    retbericht = "Onbekend dier:" + countrycode + " " + levnr;
                                    unLogger.WriteInfo("Sopracoslachtdata onbekend dier " + countrycode + " " + levnr);
                                    sl.Status = "F";
                                    sl.Omschrijving = "Onbekend dier FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString();
                                    DB.WriteSoapError(sl);
                                }
                                //addbericht(ref berichten, u, retbericht, "Slachtdata ingelezen");
                                //addbericht(ref berichten, u, "Slachtdata ingelezen: " + u.ThrStamboeknr, "Slachtdata " + u.ThrStamboeknr);
                            }
                            else
                            {
                                unLogger.WriteInfo("Sopracoslachtdata onbekende bestemming FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString());
                                sl.Status = "F";
                                sl.Omschrijving = "Kan bedrijf niet bepalen  FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString();
                                DB.WriteSoapError(sl);
                            }
                        }
                    }
                    catch (Exception exc) { unLogger.WriteError("Filebuffersync inlezenslachtdata FILE_IMPORT_ID:" + fi.FILE_IMPORT_ID.ToString() + " ERROR:" + exc.ToString()); }
                }
            }
            catch (Exception exc) { unLogger.WriteError("Filebuffersync inlezenslachtdata FILE_IMPORT_ID: ERROR:" + exc.ToString()); }
        }
   
        private void Addbericht(ref List<Tuple<THIRD, MESSAGES>> berichten, THIRD u, int UbnID, string regel, string subject)
        {
            var zoekbericht = from n in berichten
                              where n.Item1.ThrId == u.ThrId
                              select n;
            if (zoekbericht.Count() == 0)
            {
                string MesVersion = Guid.NewGuid().ToString();
                MESSAGES m = new MESSAGES
                {
                    MesBegin_DateTime = DateTime.Now,
                    MesEnd_DateTime = DateTime.Now.AddDays(4),
                    MesMessage = regel + Environment.NewLine + "<br />",
                    MesProgId = 0, //TODO invullen buiten de function
                    MesState = 1,
                    MesThrId = u.ThrId,
                    MesUbnId = UbnID,
                    MesVersion = MesVersion,
                    MesSubject = subject,
                    MesFromThrId = 0, //TODO invullen buiten de function
                    Changed_By = _ChangedBy
                };
              
          
                Tuple<THIRD, MESSAGES> mes = new Tuple<THIRD, MESSAGES>(u, m);
                berichten.Add(mes);
            }
            else
            {
                //zoekbericht.ElementAt(0).Item2.MesMessage = regel + Environment.NewLine;
            }
        }
   
        public DateTime? GetDateFormatFromFileName(string datum)
        {
            //VERWACHT: 26-06-2017
            try
            {
                char[] split = { '-' };
                string[] arrdatum = datum.Split(split);
                //unLogger.WriteInfo("getDateFormat datum:" + datum + " split {'.', ':'} length:" + arrdatum.Length.ToString());

                int jaar = Convert.ToInt32(arrdatum[2]);
                int maand = Convert.ToInt32(arrdatum[1]);
                int dag = Convert.ToInt32(arrdatum[0]);
                return new DateTime(jaar, maand, dag);
            }
            catch (Exception exc)
            {
                unLogger.WriteError(nameof(GetDateFormatFromFileName) + $@": datum:{datum} error:" + exc.ToString());
                return null;
            }
        }
   
        public DateTime? GetDateFormat(string datum, string tijd)
        {
            //VERWACHT: 26.06.17 , 8:02:30
            try
            {
                char[] split = { '.', ':' };
                string[] arrdatum = datum.Split(split);
                //unLogger.WriteInfo("getDateFormat datum:" + datum + " split {'.', ':'} length:" + arrdatum.Length.ToString());

                int jaar = Convert.ToInt32(arrdatum[2]) + 2000;
                int maand = Convert.ToInt32(arrdatum[1]);
                int dag = Convert.ToInt32(arrdatum[0]);
                return new DateTime(jaar, maand, dag);
            }
            catch (Exception exc)
            {
                unLogger.WriteError(nameof(GetDateFormat) + $@": datum:{datum} error:" + exc.ToString());
                return null;
            }
        }

        private DateTime? GetVilatcaDate(string datum, string tijd)
        {
            //VERWACHT: 16/08/04,8:00:33
            //jaar/maand/dag
            char[] split = { '/', ':' };
            string[] arrdatum = datum.Split(split);
            unLogger.WriteInfo("VilatcaDate:" + datum );
            try
            {
                int jaar = Convert.ToInt32(arrdatum[0]) + 2000;
                int maand = Convert.ToInt32(arrdatum[1]);
                int dag = Convert.ToInt32(arrdatum[2]);
                //int uur = Convert.ToInt32(arrtijd[0]);
                //int min = Convert.ToInt32(arrtijd[1]);
                //int sec = Convert.ToInt32(arrtijd[2]);
               
                return new DateTime(jaar, maand, dag);
            }
            catch(Exception exc)
            {
                unLogger.WriteError(nameof(GetVilatcaDate) + ":" + exc.ToString());
            }
            return null;
        }

        public void GetClassificationMeatAndDeviation(string v, out int classification, out int deviation)
        {
            // labelssalClassificationMeat  DB, D, S,E, U, R, O, P, ADB,  AU, AO,AR enz., zie AGRO_LABELS -> LabKind = 25
            // labelssalDeviationMeat -,0,+
            // volgens Sopraco  file[7]  DB, D+,S,E, U, R+,R, R-, O+,  O, O-, P, ADB,  AU, AO,AR  
            deviation = 0;
            classification = 0;
            
            if (string.IsNullOrWhiteSpace(v))
            {
                unLogger.WriteError($@"{nameof(FilebufferSync)}.getClassificationMeatAndDeviation kan niets bepalen.");
            }
            else
            {
                string dev = "";
                v = v.Trim();
                if (int.TryParse(v.Substring(0, 1), out int res))
                {
                    // v examples:
                    // 1S3, 1P-3, 3AE1, 3AR2, 1ADB2

                    // remove last character
                    string v1 = v.Substring(0, v.Length - 1);
                    // remove first character
                    string v2 = v1.Substring(1, v1.Length - 1);
                    v = v2;                    
                }

                // determine deviation
                dev = v.Substring(v.Length - 1, 1);
                if (dev == "-" || dev == "+")
                    v = v.Substring(0, v.Length - 1);
                else
                    dev = "";
                
                unLogger.WriteInfo($"getClassificationMeatAndDeviation: {v} {dev}");
                var een = from n in labelssalClassificationMeat where n.LabLabel == v select n;
                if (een.Count() > 0)
                {
                    classification = een.ElementAt(0).LabID;
                    switch (dev)
                    {
                        case "-":
                            deviation = 1;
                            break;
                        case "+":
                            deviation = 3;
                            break;
                        default:
                            deviation = 2;
                            break;
                    }
                }                
            }
        } /* GetClassificationMeatAndDeviation */

        #region sopracoqueries

        private DataTable GetAnimalsByUbnIdAndStateWithPlaceExt(int ubnID_Destination, AnimalState presentOnFarm, object p)
        {

            string query = GetAnimalsByUbnIdAndStateWithPlaceQuery(ubnID_Destination, presentOnFarm, null);
            return DB.GetDataBase().QueryData(_token, new StringBuilder(query));
        }
        private  string GetAnimalsByUbnIdAndStateWithPlaceQuery(int ubnId, AnimalState animalState, DierSelectieFilterModel SelectieFilter)
        {
            var critAnicategory = string.Empty;
            switch (animalState)
            {
                default:
                    critAnicategory = string.Empty;
                    break;
                case AnimalState.MovedOut:
                    critAnicategory = "AND ac.Anicategory IN(4)";
                    break;
                case AnimalState.NeverBeenPresent:
                    critAnicategory = "AND ac.Anicategory IN(5)";
                    break;
                case AnimalState.PresentOnFarm:
                    critAnicategory = " AND ac.Anicategory IN(1,2,3) ";
                    break;
                case AnimalState.PresentOrMovedOut:
                    critAnicategory = "AND ac.Anicategory IN(1, 2, 3, 4)";
                    break;
                case AnimalState.Unknown:
                    critAnicategory = "AND ac.Anicategory IN(99)";
                    break;
            }


            string critAllMovs = "";
            string critAllEvents = "";
            string HavingClause = "";
            string HavingClausePresent = "";
            if (SelectieFilter?.PresentAtDate.HasValue ?? false)
            {
                critAllMovs = $" AND m.MovDate < '{SelectieFilter.PresentAtDate.Value.ToString("yyyy-MM-dd 00:00:00")}' ";
                critAllEvents = $" AND ev.EveDate <= '{SelectieFilter.PresentAtDate.Value.ToString("yyyy-MM-dd 23:59:59")}' ";
                critAnicategory = string.Empty;
                HavingClausePresent = " StillPresent = 1 ";
            }

            string QueryHasVki = "";
            string HavingClauseVki = "";

            if ((SelectieFilter?.HasEvki == 1) || (SelectieFilter?.HasEvki == 0))
            {
                QueryHasVki = $@"(SELECT 
                                        IF(COUNT(AniId) > 0, 1, 0)
                                        FROM agrobase_calf.VKIINFO vki 
                                        LEFT JOIN agrobase_calf.VKIDIER vkid ON vki.Internalnr = vkid.Internalnr
                                        LEFT JOIN agrofactuur.UBN ub on ub.Bedrijfsnummer = vki.FarmNumber
                                        WHERE ub.UbnId = {ubnId} and vkid.AniId = a.AniId
                                        AND DATE_ADD(vkid.ReportTime, INTERVAL 7 DAY) > NOW()) As HasVki, ";
            }


            string QueryVkiVerwAfvoerDatum = "";

            if (SelectieFilter?.HasEvki == 1)
            {
                HavingClauseVki = " HasVki = 1 ";
                if (SelectieFilter.VkiVerwAfvoerDatum != null)
                {
                    string VkiVerwAfvoerDatumFMt = SelectieFilter?.VkiVerwAfvoerDatum?.ToString("yyyy-MM-dd") ?? "";
                    HavingClauseVki += $" AND DATE(VkiVerwAfvoerDatum) = '{VkiVerwAfvoerDatumFMt}' ";

                    QueryVkiVerwAfvoerDatum = $@"(SELECT 
                                        vki.VerwAfvDatum
                                        FROM agrobase_calf.VKIINFO vki 
                                        LEFT JOIN agrobase_calf.VKIDIER vkid ON vki.Internalnr = vkid.Internalnr
                                        LEFT JOIN agrofactuur.UBN ub on ub.Bedrijfsnummer = vki.FarmNumber
                                        WHERE ub.UbnId = {ubnId} and vkid.AniId = a.AniId
                                        AND DATE_ADD(vkid.ReportTime, INTERVAL 7 DAY) > NOW() ORDER BY vki.VerwAfvDatum DESC LIMIT 1) As VkiVerwAfvoerDatum, ";
                }
            }

            if (SelectieFilter?.HasEvki == 0)
            {
                HavingClauseVki = " HasVki = 0 ";
            }


            string CritGroups = "";
            if ((SelectieFilter?.Groepen != null) && (SelectieFilter.Groepen.Count() > 0)) { CritGroups = " AND IF(movPlaceGroup.MovDate is null or movIn.MovDate > movPlaceGroup.MovDate, agMovIn.AG_Id, movPlaceGroup.AG_Id ) in (" + String.Join(",", SelectieFilter.Groepen) + " ) "; };
            string CritSections = "";
            if ((SelectieFilter?.Compartimenten != null) && (SelectieFilter.Compartimenten.Count() > 0)) { CritSections = " AND pl.SectionNr in (" + String.Join(",", SelectieFilter.Compartimenten) + ") "; };
            string CritCages = "";
            if ((SelectieFilter?.lokaties != null) && (SelectieFilter.lokaties.Count() > 0)) { CritCages = " AND CONCAT('402','_', IFNULL(pl.SectionNr, 0), '_', IFNULL(pl.CageNrId, 0)) in ('" + String.Join("','", SelectieFilter.lokaties) + "') "; };
            string CritHairColor = "";
            if ((SelectieFilter?.HaarKleuren != null) && (SelectieFilter.HaarKleuren.Count() > 0)) { CritHairColor = " AND a.Anihaircolor in (" + String.Join(",", SelectieFilter.HaarKleuren) + " ) "; };
            string CritQuality = "";
            if ((SelectieFilter?.Kwaliteiten != null) && (SelectieFilter.Kwaliteiten.Count() > 0)) { CritQuality = " AND a.AniQuality in (" + String.Join(",", SelectieFilter.Kwaliteiten) + ") "; };
            //string CritStatus = "";
            // if ((SelectieFilter?.Statussen != null) && (SelectieFilter.Statussen.Count() > 0)) { CritStatus = " AND a.AniStatus in (" + String.Join(",", SelectieFilter.Statussen) + ") "; };
            string CritSexe = "";
            if ((SelectieFilter?.Geslachten != null) && (SelectieFilter.Geslachten.Count() > 0)) { CritSexe = " AND a.AniSex in (" + String.Join(",", SelectieFilter.Geslachten) + ") "; };
            string CritAanvoerDatum = "";
            if ((SelectieFilter?.AanvoerDatum != null)) { CritAanvoerDatum = " AND DATE(movIn.MovDate) = '" + SelectieFilter.AanvoerDatum.Value.ToString("yyyy-MM-dd") + "' "; };
            string CritBehandelDatum = "";
            if ((SelectieFilter?.Behandeldatum != null)) { CritBehandelDatum = " AND DATE(q.TreatedDate) = '" + SelectieFilter.Behandeldatum.Value.ToString("yyyy-MM-dd") + "' "; };
            string CritSalDestination = "";
            if ((SelectieFilter?.SalDestination != null)) { CritSalDestination = " AND s.SalDestination =  " + SelectieFilter.SalDestination + " "; };
            string CritWerknr = "";
            if (SelectieFilter?.Werknr != null) { CritWerknr = " AND AnimalNumber like '%" + SelectieFilter.Werknr + "%' "; };
            string CritLevensnummer = "";
            if (SelectieFilter?.Levensnummer != null) { CritLevensnummer = " AND AniLifeNumber like '%" + SelectieFilter.Levensnummer + "%'"; };
            string CritBirthDate = "";
            if (SelectieFilter?.BirthDateAfter != null) { CritBirthDate = " AND AniBirthDate >= '" + SelectieFilter.BirthDateAfter.Value.ToString("yyyy-MM-dd") + "'"; };

            //string CritVkiVerwAfvoerDatum = "";
            string CritAfvoerDatumVan = "";
            string CritAfvoerDatumTot = "";
            string CritAfvoerDatum = "";

            string CritAanvoerDatumVan = "";

            string AfvoerDatumFmt = SelectieFilter?.AfvoerDatum?.ToString("yyyy-MM-dd") ?? "";
            string AfvoerDatumVanFmt = SelectieFilter?.AfvoerDatumVan?.ToString("yyyy-MM-dd") ?? "";
            string AfvoerDatumTotFmt = SelectieFilter?.AfvoerDatumTot?.ToString("yyyy-MM-dd") ?? "";
            string AanvoerDatumVanFmt = SelectieFilter?.AanvoerDatumVan?.ToString("yyyy-MM-dd") ?? "";


            string statusQuery = "0 As StatusCount ";
            string HavingClauseStatus = "";
            if ((SelectieFilter?.Statussen != null) && (SelectieFilter.Statussen.Count() > 0))
            {
                statusQuery = $@"
                   ( SELECT COUNT(stat.`Status`) 
                    FROM agrobase_calf.ANIMAL ani 
                    LEFT JOIN agrobase_calf.EVENT ev ON ani.AniId = ev.AniId AND ev.EveKind = 13
                    LEFT JOIN agrobase_calf.`STATUS` stat ON ev.EventId = stat.EventId
                    LEFT JOIN agrolink.LABELS labstat on stat.`Status` = labstat.Lab_Value AND labstat.Lab_Kind=106 AND labstat.ProgId=82 AND labstat.LAB_CountryISO = 0
                    WHERE ani.AniId = q.AniId AND ev.EventId > 0 AND stat.`Status` IN ({String.Join(",", SelectieFilter.Statussen)})) AS StatusCount
                ";
                HavingClauseStatus = " StatusCount > 0 ";

            }



            //if (SelectieFilter?.VkiVerwAfvoerDatum != null)
            //{
            //    CritVkiVerwAfvoerDatum = $@" AND (DATE(movOut.MovDate) >= '{AfvoerDatumFmt} 00:00:00' OR DATE(movDead.MovDate) >= '{AfvoerDatumFmt} 00:00:00' )";
            //    CritVkiVerwAfvoerDatum += $@" AND (DATE(movOut.MovDate) <= '{AfvoerDatumFmt} 23:59:59' OR DATE(movDead.MovDate) <= '{AfvoerDatumFmt} 23:59:59' )";
            //}

            if (SelectieFilter?.AanvoerDatumVan != null)
            {
                CritAanvoerDatumVan = $@" AND DATE(movIn.MovDate) >= '{AanvoerDatumVanFmt} 00:00:00' ";
            }

            if (SelectieFilter?.AfvoerDatum != null)
            {
                CritAfvoerDatum = $@" AND (DATE(movOut.MovDate) >= '{AfvoerDatumFmt} 00:00:00' OR DATE(movDead.MovDate) >= '{AfvoerDatumFmt} 00:00:00' )";
                CritAfvoerDatum += $@" AND (DATE(movOut.MovDate) <= '{AfvoerDatumFmt} 23:59:59' OR DATE(movDead.MovDate) <= '{AfvoerDatumFmt} 23:59:59' )";
            }
            else
            {
                if (animalState == AnimalState.PresentOrMovedOut)
                {
                    if ((SelectieFilter?.AfvoerDatumVan != null)) { CritAfvoerDatumVan = $@" AND (DATE(movOut.MovDate) >= '{AfvoerDatumVanFmt} 00:00:00' OR DATE(movDead.MovDate) >= '{AfvoerDatumVanFmt} 00:00:00' OR q.AniCategory in (1, 2, 3) )"; };
                    if ((SelectieFilter?.AfvoerDatumTot != null)) { CritAfvoerDatumTot = $@" AND (DATE(movOut.MovDate) <= '{AfvoerDatumTotFmt} 23:59:59' OR DATE(movDead.MovDate) <= '{AfvoerDatumTotFmt} 23:59:59' OR q.AniCategory in (1, 2, 3) )"; };
                }
                else
                {
                    if ((SelectieFilter?.AfvoerDatumVan != null)) { CritAfvoerDatumVan = $@" AND (DATE(movOut.MovDate) >= '{AfvoerDatumVanFmt} 00:00:00' OR DATE(movDead.MovDate) >= '{AfvoerDatumVanFmt} 00:00:00' )"; };
                    if ((SelectieFilter?.AfvoerDatumTot != null)) { CritAfvoerDatumTot = $@" AND (DATE(movOut.MovDate) <= '{AfvoerDatumTotFmt} 23:59:59' OR DATE(movDead.MovDate) <= '{AfvoerDatumTotFmt} 23:59:59' )"; };
                }
            }

            if (!string.IsNullOrEmpty(HavingClausePresent))
            {
                HavingClause += HavingClausePresent;
            }

            if (!string.IsNullOrEmpty(HavingClauseStatus))
            {
                HavingClause += string.IsNullOrEmpty(HavingClause) ? HavingClauseStatus : " AND " + HavingClauseStatus;
            }

            if (!string.IsNullOrEmpty(HavingClauseVki))
            {
                HavingClause += string.IsNullOrEmpty(HavingClause) ? HavingClauseVki : " AND " + HavingClauseVki;
            }

            if (!string.IsNullOrEmpty(HavingClause))
            {
                HavingClause = " HAVING " + HavingClause;
            }

            var query = $@"
                SELECT
	                q.*, 
	                (
		                SELECT
		                datediff(date(e.EveDate)+interval (am.ArtMed_DaysWaitingMeat + 1) day,current_date())
		                FROM agrobase_calf.`EVENT` e
		                JOIN agrobase_calf.TREATMEN t ON t.EventId = e.EventId
		                JOIN agrofactuur.ARTIKEL_MEDIC am ON am.artId = t.ArtID
		                WHERE e.AniId = q.AniId AND e.UBNId = q.AcUbnId AND e.EventId > 0 AND e.EveKind = 6 AND Date(e.EveDate) <= CURRENT_DATE() AND (datediff(date(e.EveDate)+interval (am.ArtMed_DaysWaitingMeat + 1) day,current_date())) > 0
		                ORDER BY e.EveDate DESC LIMIT 1
	                ) AS TreatmentDaysWaitingMeat,
                    {statusQuery},
	                movIn.MovDate AS MovedInDate,
	                movOut.MovDate AS MovedOutDate,
	                movDead.MovDate AS DeadDate,
                    movPlace.MovDate AS PlaceDate,
                    movPlace.MovOrder AS PlaceMovOrder,
                    IF(IFNULL(movOut.MovDate, DATE('1900-01-01')) < movIn.MovDate AND IFNULL(movDead.movDate, DATE('1900-01-01')) < movIn.MovDate, 1, 0) AS StillPresent,
	                b.PurKind AS MovedInReasonID,
	                s.SalKind AS MovedOutReasonID,
                    s.SalDestination,
                    IF(s.MovId IS NULL, (IF(l.MovId IS NULL, NULL ,l.LosVersienrVertrekluik)), s.SalVersienrVertrekluik ) AS MovedoutVersieNrPasspoort,
	                l.LosReason AS DeadReasonID,

                    pl.StableNr AS StableId,
                    pl.SectionNr AS SectionId,
                    pl.CageNrId AS CageId,

                    #CONCAT(IF(pl.CageNrId = 0, '401', '402'), '_', IFNULL(p.SectionNr, 0), '_', IFNULL(pl.CageNrId, 0)) AS SectionAndCageId,
                    IF(pl.CageNrId = 0, '401_0_0', CONCAT('402','_', IFNULL(pl.SectionNr, 0), '_', IFNULL(pl.CageNrId, 0)) ) AS SectionAndCageId,
                    hc.LabLabel as HairColor,
	                IF(movPlaceGroup.MovDate is null or movIn.MovDate > movPlaceGroup.MovDate, agMovIn.AG_Id, agMovPlace.AG_Id ) AS GroupID,
                    IF(movPlaceGroup.MovDate is null or movIn.MovDate > movPlaceGroup.MovDate, agMovIn.AG_Name, agMovPlace.AG_Name ) AS GroupName,
                    lk.LabLabel as Quality,
                    remsection.RemLabel As Compartiment
                FROM
                (
	                SELECT
		                #CAST(ac.AniWorknumber AS UNSIGNED) AS AnimalNumber, GD: CAST weggehaald
                        ac.UbnId as AcUbnId,
                        ac.AniCategory,
                        ac.AniWorknumber AS AnimalNumber,
		                t.ThrCompanyName AS FokkerName,
                        (SELECT ev.EveDate FROM agrobase_calf.EVENT ev WHERE ev.AniId = a.AniId AND ev.UBNId = ac.UbnId AND ev.EventId > 0 {critAllEvents} AND ev.EveKind = 6 ORDER BY ev.EveDate DESC, ev.EveOrder DESC LIMIT 1) AS TreatedDate,
		                (SELECT m.MovId FROM agrobase_calf.MOVEMENT m WHERE m.AniId = a.AniId AND m.UbnId = ac.UbnId AND m.MovId > 0 {critAllMovs} AND m.MovKind = 1 ORDER BY m.MovDate DESC LIMIT 1) AS MovIdIn,
		                (SELECT m.MovId FROM agrobase_calf.MOVEMENT m WHERE m.AniId = a.AniId AND m.UbnId = ac.UbnId AND m.MovId > 0 {critAllMovs} AND m.MovKind = 2 ORDER BY m.MovDate DESC LIMIT 1) AS MovIdOut,
		                (SELECT m.MovId FROM agrobase_calf.MOVEMENT m WHERE m.AniId = a.AniId AND m.UbnId = ac.UbnId AND m.MovId > 0 {critAllMovs} AND m.MovKind = 3 ORDER BY m.MovDate DESC LIMIT 1) AS MovIdDead,
                        (SELECT m.MovId FROM agrobase_calf.MOVEMENT m 
						    LEFT JOIN agrobase_calf.PLACE pl1 on m.MovId = pl1.MovId
						    WHERE m.AniId = a.AniId AND m.UbnId = ac.UbnId AND m.MovId > 0 {critAllMovs} AND m.MovKind = 8 AND pl1.SectionNr > 0 ORDER BY m.MovDate DESC, m.MovOrder DESC LIMIT 1) AS MovIdPlace,
                        (SELECT m.MovId FROM agrobase_calf.MOVEMENT m WHERE m.AniId = a.AniId AND m.UbnId = ac.UbnId AND m.MovId > 0 {critAllMovs} AND m.MovKind = 8 AND m.Ag_Id > 0 ORDER BY m.MovDate DESC, m.MovOrder DESC LIMIT 1) AS MovIdPlaceGroup,
                        {QueryHasVki}
                        {QueryVkiVerwAfvoerDatum}
		               # a.AniWorkNumber AS worknr,
                        a.AniLifeNumber, a.AniId, a.anialternatenumber, a.AniBirthDate,  a.AniSex, a.Anihaircolor, a.AniQuality, a.AniStatus,
                        ac.UbnId, 
                        (CASE a.AniSex 
			                WHEN 1 
				                THEN 'M' 
			                WHEN 2 
			                    THEN 'V' 
			                   ELSE '' 
		                   END) as MV,
                           FLOOR((((TIMESTAMPDIFF(Day, a.AniBirthDate, CurDate())) / 30.4167) / 12)) + Round(MOD((TIMESTAMPDIFF(Day, a.AniBirthDate, CurDate())) / 30.4167, 12) / 12, 2) As 'LftJaarMaand' #TODO: GD deze weghalen
	                FROM agrobase_calf.ANIMALCATEGORY ac
	                JOIN agrobase_calf.ANIMAL a ON a.AniId = ac.AniId AND a.AniId > 0
	                LEFT JOIN agrofactuur.UBN ub ON ac.UbnId = ub.UBNid
	                LEFT JOIN agrofactuur.THIRD t ON t.ThrId = ub.ThrId
	                WHERE ac.AC_ID > 0 AND ac.UbnId = {ubnId} {critAnicategory} {CritHairColor} {CritQuality} {CritSexe} 
	                ORDER BY AnimalNumber
                )q
                LEFT JOIN agrobase_calf.MOVEMENT movIn ON movIn.MovId = q.MovIdIn
                LEFT JOIN agrobase_calf.MOVEMENT movOut ON movOut.MovId = q.MovIdOut
                LEFT JOIN agrobase_calf.MOVEMENT movDead ON movDead.MovId = q.MovIdDead
                LEFT JOIN agrobase_calf.MOVEMENT movPlace ON movPlace.MovId = q.MovIdPlace
                LEFT JOIN agrobase_calf.MOVEMENT movPlaceGroup ON movPlaceGroup.MovId = q.MovIdPlaceGroup
                LEFT JOIN agrobase_calf.BUYING b ON b.MovId = movIn.MovId
                LEFT JOIN agrobase_calf.SALE s ON s.MovId = movOut.MovId
                LEFT JOIN agrobase_calf.LOSS l ON l.MovId = movDead.MovId
                LEFT JOIN agrobase_calf.PLACE pl ON pl.MovId = movPlace.MovId
                LEFT JOIN agrobase_calf.ANIMAL_GROUP agMovIn ON agMovIn.AG_Id = movIn.AG_Id
                LEFT JOIN agrobase_calf.ANIMAL_GROUP agMovPlace ON agMovPlace.AG_Id = movPlaceGroup.AG_Id
                LEFT JOIN agrolink.LABELS labstat on q.AniStatus = labstat.Lab_Value AND labstat.Lab_Kind=106 AND labstat.ProgId=82 AND labstat.LAB_CountryISO = 0
                LEFT JOIN agrofactuur.AGRO_LABELS lk on q.AniQuality = lk.LabId AND lk.LabKind=135 AND lk.LAbProgId=0
                LEFT JOIN agrofactuur.AGRO_LABELS hc on q.Anihaircolor = hc.LabId AND hc.LabKind=108 AND hc.LAbProgId=0 AND hc.LabCountry=56
                LEFT JOIN agrobase_calf.REMARK remsection ON remsection.LabKind = 401 AND remsection.RemId= pl.SectionNr and pl.StableNr = remsection.LabId and remsection.UbnID = {ubnId}
                WHERE true
                {CritGroups}
                {CritSections}
                {CritCages}
                {CritAanvoerDatum}
                {CritAanvoerDatumVan}
                {CritBehandelDatum}
                {CritAfvoerDatumVan}
                {CritAfvoerDatumTot}
                {CritAfvoerDatum}
                {CritSalDestination}
                {CritWerknr}
                {CritLevensnummer}
                {CritBirthDate}
                {HavingClause}
                ORDER BY AnimalNumber";
            return query;
        }


        #endregion

    }
    public enum AnimalState
    {
        All,
        PresentOnFarm,
        MovedOut,
        NeverBeenPresent,
        Unknown,
        PresentOrMovedOut
    }
    public class DierSelectieFilterModel
    {
        public List<string> Groepen { get; set; }
        public List<string> lokaties { get; set; }
        public List<string> Compartimenten { get; set; }
        public List<string> HaarKleuren { get; set; }
        public List<string> Kwaliteiten { get; set; }
        public List<string> Geslachten { get; set; }
        public List<string> Statussen { get; set; }
        public DateTime? AanvoerDatumVan { get; set; }
        public DateTime? AanvoerDatum { get; set; }
        public DateTime? Behandeldatum { get; set; }
       
        public DateTime? AfvoerDatumVan { get; set; }
      
        public DateTime? AfvoerDatumTot { get; set; }
        public DateTime? AfvoerDatum { get; set; }
        public DateTime? BirthDateAfter { get; set; }
        public int? SalDestination { get; set; } //Thrid bestemming Sale
        public AnimalState? AnimalState { get; set; }
        public int? HasEvki { get; set; }
        public DateTime? VkiVerwAfvoerDatum { get; set; }
        public string Levensnummer { get; set; }
        public string Werknr { get; set; }
        public bool OnlyWithSlaughterData { get; set; }

        public DateTime? PresentAtDate { get; set; }
    }
}
