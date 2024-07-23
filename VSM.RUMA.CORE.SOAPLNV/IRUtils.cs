using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.COMMONS;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace VSM.RUMA.CORE.SOAPLNV
{
    public class IRUtils 
    {
        private const int LOG_LENGTH_PREFIX = 120;
        private const int LOG_LENGTH_PREFIX_LOG = 160;
        private const int FORMAT_ELASPED_DESC_LENGTH = 30;
        private const int FORMAT_ELASPED_COUNT_LENGTH = 5;

        /// <summary>
        /// Formats prefix to set length for readability Staat ook in rsReprofunctions
        /// </summary>
        /// <param name="logPrefix"></param>
        /// <returns></returns>
        public static string formatLogPrefix(string logPrefix)
        {
            int logLength = LOG_LENGTH_PREFIX;
            if (logPrefix.Length > logLength - 2)
                logLength = LOG_LENGTH_PREFIX_LOG;

            return logPrefix.PadRight(logLength - 2, ' ') + " -";
        }

        /// <summary>
        /// Format elapsed. Staat ook in rsReprofunctions
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="count"></param>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public static string formatElapsed(string desc, int count, TimeSpan elapsed)
        {
            if (count >= 0)
                return $"{(desc + ":").PadRight(FORMAT_ELASPED_DESC_LENGTH, ' ')} Aantal: {count.ToString().PadLeft(FORMAT_ELASPED_COUNT_LENGTH, ' ')} Elapsed: {elapsed}";
            return $"{(desc + ":").PadRight(FORMAT_ELASPED_DESC_LENGTH, ' ')}               Elapsed: {elapsed}";
        }

        /// <summary>
    /// 
    /// </summary>
    /// <param name="aniCatBuffer"></param>
    /// <param name="farmId"></param>
    /// <param name="aniId"></param>
    /// <returns></returns>
        public static KeyValuePair<AniCatKey, ANIMALCATEGORY> GetAniCatFromCache(Dictionary<AniCatKey, ANIMALCATEGORY> aniCatBuffer, int farmId, int aniId)
        {
            string lPrefix = formatLogPrefix($"{nameof(IRUtils)}.{nameof(GetAniCatFromCache)}");

            if (aniCatBuffer == null)
            {
                unLogger.WriteError($"{lPrefix} aniCatBuffer niet gezet.");
                return default(KeyValuePair<AniCatKey, ANIMALCATEGORY>);
            }

            if (farmId <= 0)
            {
                unLogger.WriteError($"{lPrefix} ongeldig farmId: {farmId}");
                return default(KeyValuePair<AniCatKey, ANIMALCATEGORY>);
            }

            if (aniId <= 0)
            {
                unLogger.WriteError($"{lPrefix} ongeldig aniId: {aniId}");
                return default(KeyValuePair<AniCatKey, ANIMALCATEGORY>);
            }

            var kvp = aniCatBuffer.FirstOrDefault(x => x.Key.AniId == aniId && x.Key.FarmId == farmId);
            if (kvp.Key == null || kvp.Key.FarmId != farmId)
            {
                unLogger.WriteDebug($"{lPrefix} Geen anicat gevonden, FarmId: {farmId} AniId: {aniId}. Aanmaken.");

                var ack = new AniCatKey(farmId, aniId, null, true);
                var ac = new ANIMALCATEGORY()
                {
                    AniId = aniId,
                    FarmId = farmId,
                    Changed_By = (int)LABELSConst.ChangedBy.IRUtils,
                    SourceID = 0,
                    Anicategory = 5
                };

                var ret = new KeyValuePair<AniCatKey, ANIMALCATEGORY>(ack, ac);
                aniCatBuffer.Add(ack, ac);
                return ret;
            }
            return kvp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movements"></param>
        /// <param name="aniId"></param>
        /// <returns></returns>
        public static int? GetAniCategoryFromMovements(IEnumerable<MOVEMENT> movements, ANIMAL ani, UBN ubn, int minMovementDaysBeforeDelete)
        {
            //Geen movements voor zijn ubn bij RVO
            //En geen movements voor zijn ubn in AB
            //En het dier ouder dan een maand
            //En het dier niet geboren op eigen bedrijf.
            try
            {
                string lPrefix = formatLogPrefix($"{nameof(IRUtils)}.{nameof(GetAniCategoryFromMovements)} Bedrijf: '{ubn.Bedrijfsnummer}' Dier: '{ani.AniAlternateNumber}'");

                var enMovements = movements as MOVEMENT[] ?? movements.Where(m => m.AniId == ani.AniId && m.UbnId == ubn.UBNid).ToArray();

                MOVEMENT mov = enMovements.Where(m => (m.MovKind == 1 || m.MovKind == 2 || m.MovKind == 3) && m.MovDate.Date <= DateTime.Now.Date).OrderByDescending(m => m.MovDate).ThenByDescending(m => m.MovOrder).FirstOrDefault();
                if (mov == null || mov.MovId <= 0)
                {

                    if (ani.ThrId == ubn.ThrID || ani.UbnId == ubn.UBNid)
                    {
                        if (movements.Any(m => (m.MovKind == 1 || m.MovKind == 2 || m.MovKind == 3) && m.MovDate.Date < DateTime.Now.Date.AddDays(-minMovementDaysBeforeDelete)))
                        {
                            unLogger.WriteWarn($"{lPrefix} Dier zou op eigen bedrijf geboren zijn, maar andere bedrijven hebben movements met het dier (meer dan {minMovementDaysBeforeDelete} dagen geleden), zet op aanwezig geweest.");
                            return (int)LABELSConst.AniCategory.AanwezigGeweest;
                        }

                        unLogger.WriteTrace($"{lPrefix} Geen movements, dier op eigen bedrijf geboren. Zet op Aanwezig.");
                        return (int)LABELSConst.AniCategory.Aanwezig;
                    }
                                                                        
                    if (ani.AniBirthDate < DateTime.Now.Date.AddMonths(-1))
                    {
                        unLogger.WriteTrace($"{lPrefix} Geen movements, niet op eigen bedrijf geboren en dier ouder dan een maand. Zet op nooit aanwezig geweest.");
                        return (int)LABELSConst.AniCategory.NooitAanwezigGeweest;
                    }

                    unLogger.WriteTrace($"{lPrefix} Geen movements gevonden en dier minder oud als een maand. Pas niks aan.");
                    return null;
                }
                else
                {
                    switch (mov.MovKind)
                    {
                        case (int)LABELSConst.MovKind.Buying:
                            if (ani.AniSex == (int)LABELSConst.AniSex.Mannelijk)
                                return (int)LABELSConst.AniCategory.Meststier;

                            return (int)LABELSConst.AniCategory.Aanwezig;
                        case (int)LABELSConst.MovKind.Sale:
                        case (int)LABELSConst.MovKind.Loss:
                            //afvoer
                            return (int)LABELSConst.AniCategory.AanwezigGeweest;

                        default:
                            unLogger.WriteError($"{lPrefix} Onbekend MovKind: '{mov.MovKind}'.");
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteError($"{nameof(OpvragenLNVDierDetailsV2)}.{nameof(GetAniCategoryFromMovements)} - Ex: {ex.Message} - Pas niks aan.", ex);
                return null;
            }
        }

        /// <summary>
        /// Kijk of bij de BIRTH van de moeder het CalfId goed staat.
        /// IVM Tweeling, indien bij een van de worpen het CalfId goed staat, niks verder aanpassen
        /// en maar 1 worp updaten als het CalfId onbekend is, ook al voldoen meerdere records er aan.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ani"></param>
        /// <param name="sLogger"></param>
        /// <param name="changedBy"></param>
        /// <param name="sourceId"></param>
        /// <param name="lPrefixAnimal"></param>
        public static void CorrigeerWorpBijMoeder(AFSavetoDB db, UBN requestingUBN, ANIMAL ani, int changedBy, int sourceId)
        {
            string lPrefixAnimal = formatLogPrefix($"{nameof(IRUtils)}.{nameof(CorrigeerWorpBijMoeder)} Bedrijf: '{requestingUBN?.Bedrijfsnummer}' Dier: '{ani.AniAlternateNumber}'");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                //unLogger.WriteDebug(sLogger, $"{lPrefixAnimal} controleer afkalvingen bij moeder.");

                var events = db.getEventsByDateAniIdKind(ani.AniBirthDate, ani.AniIdMother, (int)LABELSConst.EventKind.AFKALVEN);
                List<BIRTH> births = new List<BIRTH>(events.Count);
                foreach (EVENT ev in events)
                {
                    births.Add(db.GetBirth(ev.EventId));
                }

                if (births.Any(b => b.CalfId == ani.AniId))
                {
                    //unLogger.WriteDebug(sLogger, $"{lPrefixAnimal} Dier staat bij worp moeder.");
                }
                else if (births.Any(b => b.CalfId == 0 && b.BornDead == 0))
                {
                    //update
                    BIRTH bir = births.First(b => b.CalfId == 0 && b.BornDead == 0);
                    if (bir != null && bir.EventId > 0)
                    {
                        bir.CalfId = ani.AniId;
                        bir.Changed_By = changedBy;
                        bir.SourceID = sourceId;

                        if (!db.SaveBirth(bir))
                        {
                            unLogger.WriteError($"{lPrefixAnimal} Fout tijdens updaten worp ID: {bir.EventId}.");
                        }
                        else
                        {
                            unLogger.WriteInfo($"{lPrefixAnimal} Worp ID: {bir.EventId} geupdate met CalfId.");
                        }
                    }
                }
                else
                {
                    //unLogger.WriteDebug(sLogger, $"{lPrefixAnimal} Geen worp gevonden bij moeder voor dier.");
                }
            }
            catch (Exception ex)
            {
                string msg = $"{lPrefixAnimal} Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
            }
            finally
            {
                unLogger.WriteDebug($"{lPrefixAnimal} controleer afkalvingen bij moeder klaar. Elapsed: {sw.Elapsed}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iServer"></param>
        /// <returns></returns>
        private static string GetBaseEndpoint(int iServer)
        {
            switch (iServer)
            {
                case 0:
                    return "https://dbrbms.agro.nl/osbbms_v2_0/services/";
                case 1:
                    return "https://dbrbms-acc.agro.nl/osbbms20_air1/services/";//air2 werkt niet
                case 2:
                    return "https://dbrbms-acc.agro.nl/osbbms20_air1/services/";
                default:
                    unLogger.WriteWarn($"{nameof(IRUtils)}.{nameof(GetBaseEndpoint)} - iServer: {iServer} - Unknown, using LIVE enpoint.");
                    return GetBaseEndpoint(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iServer"></param>
        /// <returns></returns>
        public static string GetMeldingWSEndpoint(int iServer = 0)
        {
            return GetBaseEndpoint(iServer) + "MeldingenWS";
        }


        public static int PrognrToLNVdiersoort(int prog)
        {
            if (prog == 3) // schapen
                return 3;
            else if (prog == 5) // geiten
                return 4;
            else // rundvee
                return 1;
        }

        public static int LNVdiersoortToPrognr(int Diersoort)
        {
            if (Diersoort == 4) // geiten
                return 5;
            else return Diersoort;
        }

        public static string removevoorloopnullen(string p)
        {
            while (p.StartsWith("0"))
            {
                int lengte = p.Length;
                if (lengte > 1)
                {
                    p = p.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }
            return p;
        }
        public static int ProgIdToRvoDiersoort(LABELSConst.ProgId progId)
        {
            switch (progId)
            {
                case LABELSConst.ProgId.schapen:
                    return 3;
                case LABELSConst.ProgId.geiten:
                    return 4;
                case LABELSConst.ProgId.rundvee:
                    return 1;

                default:
                    unLogger.WriteWarn($"Geen Diersoort gespecificeerd voor progId: '{progId}' Gebruik 1.");
                    return 1;
            }
        }

        public static string RumaMeldingStatusNaarRvo(int MeldStatus)
        {
            switch (MeldStatus)
            {
                case 1:
                    return "DG"; // Definitief geregistreerd
                case 2:
                    return "VG"; // voorlopig geregistreerd
                case 3:
                    return "HE"; // Hersteld
                case 4:
                    return "IC"; // Inconsistent
                case 5:
                    return "ID"; // Ingediend
                case 6:
                    return "IT"; // Ingetrokken
                default:
                    return "";
            }
        }

        public static int RvoMeldingStatusNaarRuma(string meldingtatus)
        {
            switch (meldingtatus)
            {
                case "DG":
                    return 1; // Definitief geregistreerd
                case "VG":
                    return 2; // voorlopig geregistreerd
                case "HE":
                    return 3; // Hersteld
                case "IC":
                    return 4; // Inconsistent
                case "ID":
                    return 5; // Ingediend
                case "IT":
                    return 6; // Ingetrokken
                default:
                    return 0;
            }
        }

        public static string RumaMeldingTypeNaarRvo(int BerichtType)
        {
            {
                switch (BerichtType)
                {
                    case 1:
                        return "AAN"; //  aanvoer op dit ubn
                    case 2:
                        return "AFV"; // afvoer van dit ubn
                    case 3:
                        return "GER"; //  geboorte
                    case 4:
                        return "AFK"; // doodgeboren
                    case 5:
                        return "IMP"; // import
                    case 6:
                        return "EXP"; // export
                    case 7:
                        return "DOO"; // dood
                    case 8:
                        return "DVL"; // diervlag
                    case 9:
                        return "VEM"; // vervangend merk (omnummering)
                    case 10:
                        return "NOO"; // noodslachting
                    case 11:
                        return "SLA";

                    default:
                        return "";
                }
            }
        }

        public static int RVOBerichtTypeNaarRumaMeldingType(string berichttype)
        {
            switch (berichttype)
            {
                case "AAN":
                    return 1;
                case "AFV":
                    return 2;
                case "GER":
                    return 3;
                case "AFK":
                    return 4;
                case "IMP": 
                    return 5;
                case "EXP": 
                    return 6;
                case "DOO":
                    return 7;
                case "DVL": 
                    return 8;
                case "VEM": 
                    return 9;
                case "NOO":
                    return 10;
                case "SLA":
                    return 11;
                default:
                    return 0;
            }
        }

        public static void writerequest<T>(string ubn, string url, string tijd, T req)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["WriteSoapRequest"] != null && System.Configuration.ConfigurationManager.AppSettings["WriteSoapRequest"].ToString().ToUpper() == "TRUE")
                {
                    XmlSerializer xsreq = new XmlSerializer(typeof(T));

                    string filename = typeof(T) + "_" + ubn + "_" + tijd + ".xml";
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(unLogger.getLogDir(), "IenR"));
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    string logfile = Path.Combine(unLogger.getLogDir(), "IenR", filename);
                    var xml = "";

                    using (var sww = new StringWriter())
                    {
                        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww))
                        {
                            xsreq.Serialize(writer, req);
                            XDocument xd = XDocument.Parse(sww.ToString());
                            xd.Root.Add(new XAttribute("Uri", url));
                            xml = xd.ToString();
                        }
                    }

                    using (StreamWriter wr = new StreamWriter(logfile))
                    {
                        wr.Write(xml);
                    }
                    //unLogger.WriteDebug($@"  - UBN: {ubn} Logfile: {logfile}");
                }

            }
            catch (Exception exc)
            {
                unLogger.WriteError(exc.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static bool IsGemeld(int report)
        {
            //2 Is geblokkeerd niet gemeld.
            return report > 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool DeleteMovement(AFSavetoDB db, MOVEMENT delmov, LABELSConst.ChangedBy changedBy, int sourceId)
        {
            string prefix = formatLogPrefix($"{nameof(IRUtils)}.{nameof(DeleteMovement)}");

            if (delmov.MovId <= 0)
            {
                unLogger.WriteError($"{prefix} Ongeldig movId.");
                return false;
            }

            unLogger.WriteTrace($"{prefix} MovId: '{delmov.MovId}' Verplaatsing (kind: {delmov.MovKind}) op: {delmov.MovDate:dd-MM-yyyy} MovId: {delmov.MovId}.");

            MUTATION mut = db.GetMutationByMovId(delmov.MovId);

            if (mut.Internalnr == 0) // geen klaarstaande melding voor CRV/LNV, melding ingetrokkken?
            {
                MUTALOG mutlog = db.GetMutaLogByMovId(delmov.MovId);

                if (!IsGemeld(mutlog.Report))
                {
                    if (delmov.MovMutationDate <= DateTime.Now)
                    {
                        delmov.Changed_By = (int)changedBy;
                        delmov.SourceID = sourceId;

                        if (db.DeleteMovement(delmov))
                        {
                            unLogger.WriteDebug($"{prefix} Movement: '{delmov.MovId}' (kind: {delmov.MovKind}) op: {delmov.MovDate:dd-MM-yyyy} MovId: {delmov.MovId} - Verwijderd.");
                            return true;
                        }
                        else
                        {
                            unLogger.WriteError($"{prefix} Kan movement: '{delmov.MovId}' (kind: {delmov.MovKind}) op: {delmov.MovDate:dd-MM-yyyy} MovId: {delmov.MovId} - Fout bij verwijderen.");
                            return true;
                        }
                    }
                    else
                    {
                        unLogger.WriteError($"{prefix} Kan movement: '{delmov.MovId}'  niet verwijderen. MutationDate ligt na vandaag.");
                        return true;
                    }                
                }
                else
                {
                    unLogger.WriteError($"{prefix} Kan movement: '{delmov.MovId}' niet verwijderen, is al gemeld.");
                    return false;
                }
            }
            else
            {
                unLogger.WriteWarn($"{prefix} Kan movement: '{delmov.MovId}' niet verwijderen, er staat een melding voor klaar.");
                return false;
            }
        }


        /// <summary>
        /// Zelfde als rsReproFunctions
        /// </summary>
        /// <param name="cat1"></param>
        /// <param name="cat2"></param>
        /// <returns></returns>
        public static bool UpdateAniCatRequired(int cat1, int? cat2)
        {
            if (!cat2.HasValue)
                return true;

            //Bv aanwezig -> meststier is nutteloos
            if (cat1 >= 1 && cat1 <= 3 && cat2.Value >= 1 && cat2.Value <= 3)
                return false;

            return cat1 != cat2.Value;
        }

        public static string LNVIRHaarKleur(int Haircolor, string AniHaircolor_Memo)
        {
            if (AniHaircolor_Memo != "")
            { return AniHaircolor_Memo; }
            switch (Haircolor)
            {
                case 1: 
                    return "ZB";
                case 2:
                    return "RB";
                case 3:
                    return "BZ";
                case 4:
                    return "BR";
                case 5:
                    return "EB";
                case 6:
                    return "ER";
                case 7:
                    return "EZ";
                case 8:
                    return "EW";
                case 9:
                    return "EG";
                case 10:
                    return "MZ";
                case 11:
                    return "MR";
                case 12:
                    return "ZW";
                case 13:
                    return "RW";
                case 14:
                    return "VB";
                case 15:
                    return "DK";
                case 16:
                    return "BB";
                case 17:
                    return "OV";
                case 18:
                    return "AZ";
                case 19:
                    return "AR";
                default: return null;
            }
        }

    }

    public class Masking : MaskingClass
    {
        public Masking()
          : base("cm51ZW1rYXJkYWJ2")
        {

        }
        #region Masking
        public string Codeer_base64(string data)
        {
            return base.base64Encode(data);

        }

        internal string Codeer_Tekst(string Tekst)
        {
            return base.Codeer_String(Tekst);

        }

        internal string DeCodeer_Tekst(string Tekst)
        {
            return base.DeCodeer_String(Tekst);
        }



        // LEGACY
        [Obsolete("gebruik Codeer_base64")]
        public new string base64Encode(string data)
        {
            return base.base64Encode(data);

        }

        [Obsolete("gebruik Codeer_Tekst")]
        public new string Codeer_String(string Tekst)
        {
            return base.Codeer_String(Tekst);

        }

        [Obsolete("gebruik DeCodeer_Tekst")]
        public new string D(string Tekst)
        {
            return base.DeCodeer_String(Tekst);
        }

        [Obsolete("gebruik de Win32 functie of PasswordDecryptor")]
        internal string Decodeer_PW(string Tekst)
        {
            return base.base64Decode(Tekst);
        }

        #endregion
    }
}
