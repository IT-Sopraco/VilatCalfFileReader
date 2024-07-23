using System;
using System.Collections.Generic;
using System.Linq;

using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.COMMONS;
using System.Configuration;

namespace VSM.RUMA.CORE
{
    public class SendReceive : ISendReceive
    {

        public void VerzendenOntvangen(int pThrId, DBConnectionToken pToken, BEDRIJF Farm, int changedby,int sourceid)
        {
            string AgrobaseUsername = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["AgrobaseUsername"]);
            string AgrobasePassword = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["AgrobasePassword"]);

            UserRightsToken URT = Facade.GetInstance().getRechten().Inloggen(AgrobaseUsername, Facade.GetInstance().getRechten().base64Encode(AgrobasePassword));
            Facade.GetInstance().getRechten().VeranderAdministratie(ref URT, Farm);

            Verzenden(URT, Farm.Programid, changedby, sourceid);
            Ontvangen(pThrId, URT, Farm.Programid);
        }

        public void VerzendenOntvangen(int pThrId, UserRightsToken pToken, int pProgramID)
        {
            // verzenden is nooit ingeschakeld ondanks de naam van deze functie, moet nog een keer getest worden
            //Verzenden(pToken, pProgramID);
            Ontvangen(pThrId, pToken, pProgramID);
        }

        private void Verzenden(UserRightsToken pToken, int pProgramID, int changedby, int sourceid)
        {
            Facade.GetInstance().UpdateProgress(1, "Verzenden");
            VerzendenIRMeldingen(pToken, pProgramID, true,  changedby, sourceid);
            VerzendenTochtmeldingen(pToken, pProgramID,true, changedby, sourceid);
            VerzendenDrachtStatusen(pToken, pProgramID, true, changedby, sourceid);
            VerzendenDHZmeldingen(pToken, pProgramID, true, changedby, sourceid);
            Facade.GetInstance().UpdateProgress(99, "Verzenden");
        }

        private void Ontvangen(int pThrId, UserRightsToken pToken, int pProgramID)
        {
            Facade.GetInstance().UpdateProgress(1, "Inlezen BBS");
            //InlezenSlachtBestandenBBS(pToken, pProgramID);

            Facade.GetInstance().UpdateProgress(33, "FTP Ontvangen");
            FTPontvangen(pToken.getChildConnection(pProgramID).UBNId, pToken);

            Facade.GetInstance().UpdateProgress(66, "Inlezen EDINRS");
            InlezenEDINRS(pThrId, pToken, pProgramID);
            Facade.GetInstance().UpdateProgress(90, "Ophalen Actieve Stallijst...");
            InlezenEDINRSSOAP(pToken, pProgramID);
        }

        public void EDINRSOphalenEnVerwerken(int pThrId, DBConnectionToken pToken, BEDRIJF Farm)
        {
            string AgrobaseUsername = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["AgrobaseUsername"]);
            string AgrobasePassword = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["AgrobasePassword"]);

            UserRightsToken urt = Facade.GetInstance().getRechten().Inloggen(AgrobaseUsername, Facade.GetInstance().getRechten().base64Encode(AgrobasePassword));
            Facade.GetInstance().getRechten().VeranderAdministratie(ref urt, Farm);
            Facade.GetInstance().UpdateProgress(33, "FTP Ontvangen");
            FTPontvangen(Farm.UBNid, urt);

            Facade.GetInstance().UpdateProgress(66, "Inlezen EDINRS");
            InlezenEDINRS(pThrId, urt, Farm.Programid);
            Facade.GetInstance().UpdateProgress(90, "Ophalen Actieve Stallijst...");
            InlezenEDINRSSOAP(urt, Farm.Programid);
        }

        public void FTPVerzendenOfOntvangen(FTPINFO lFtpInfo, DBConnectionToken token, BEDRIJF farm)
        {
            int iOk;
            int iError;
            int iNoData;
            FTPVerzendenOfOntvangen(lFtpInfo, token, farm, out iOk, out iError, out iNoData);
        }

        public void FTPVerzendenOfOntvangen(FTPINFO lFtpInfo, DBConnectionToken token, BEDRIJF farm, out int iOk, out int iError, out int iNoData)
        {
            iOk = 0;
            iError = 0;
            iNoData = 0;

            var singleton = Facade.GetInstance();
            var DB = singleton.getSaveToDB(token);
            singleton.UpdateProgress(1, "FTP VerzendenOfOntvangen");
            FTP ftpnrs = new FTP(lFtpInfo.FtpHostName, lFtpInfo.UserName, lFtpInfo.Password, lFtpInfo.PassiveMode == 1);
            singleton.UpdateProgress(10, "FTP " + lFtpInfo.FtpHostName);
            switch (lFtpInfo.Direction)
            {
                case 0:
                    singleton.UpdateProgress(10, "FTP Ontvangen " + lFtpInfo.FtpHostName);
                   
                    string[] files = ftpnrs.GetFileList(lFtpInfo.DirectoryFrom);
                    if (files != null) // luc sept 2016 nullpointer afvangen als er geen bestanden in de map staan.
                    {
                        foreach (string filename in files.Where(x => !string.IsNullOrWhiteSpace(x)))
                        {
                            unLogger.WriteInfo(string.Format("Bestand in FTP postbus: {0}", filename));
                            string ftppath = string.Format("{0}@{1}\\{2}\\{3}", lFtpInfo.UserName, lFtpInfo.FtpHostName, lFtpInfo.DirectoryFrom, filename);
                            unLogger.WriteDebug(string.Format("getFileLogByHostnameAndFileName({0},{1})", System.Net.Dns.GetHostName(), ftppath.Replace("\\", "\\\\")));
                            var filelog = DB.getFileLogByHostnameAndFileName(System.Net.Dns.GetHostName(), ftppath.Replace("\\", "\\\\"));
                            if (filelog.Filelog_id == 0 || filelog.Errormemo != lFtpInfo.UserName || lFtpInfo.AfterTransfer == 1) // luc 13-10-2016 CRV heeft geen unieke bestandsnamen in de postbus, dus bij verwijderen altijd opnieuw downloaden
                            {
                                unLogger.WriteInfo(string.Format("Start download bestand {0}", filename));
                                try
                                {
                                    ftpnrs.DownloadThrowsExceptions(lFtpInfo.DirectoryTo, lFtpInfo.DirectoryFrom, filename, lFtpInfo.AfterTransfer);
                                    FileLogger.AddFileLog(DB, 0, lFtpInfo.UserName, ftppath, string.Empty, 1);
                                    iOk++;
                                }
                                catch (Exception ex)
                                {
                                    unLogger.WriteError(string.Format("Download of {0} Failed... ", filename), ex);
                                    FileLogger.AddFileLog(DB, 0, ex.Message, ftppath, string.Empty, 3);
                                    iError++;
                                }
                            }
                            else
                                iNoData++;
                        }
                    }
                    break;
                case 1:
                    singleton.UpdateProgress(10, "FTP Verzenden " + lFtpInfo.FtpHostName);
                    ftpnrs.UploadtoDir(lFtpInfo.DirectoryFrom, lFtpInfo.DirectoryTo, lFtpInfo.AfterTransfer);
                    break;
            }
            singleton.UpdateProgress(99, "FTP VerzendenOfOntvangen");
        }

        /// <summary>
        /// verzenden per Array
        /// </summary>
        /// <param name="pToken"></param>
        /// <param name="ubn"></param>
        /// <param name="b"></param>
        /// <param name="ThrowImportFailure"></param>
        /// <param name="changedby"></param>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public List<SOAPLOG> VerzendenIRMeldingenV2(UserRightsToken pToken, UBN ubn, BEDRIJF b, bool ThrowImportFailure, int changedby, int sourceid, int tasklogid = 0)
        {
            var DB = Facade.GetInstance().getSaveToDB(pToken);
            string prefix = $"{nameof(SendReceive)}.{nameof(VerzendenIRMeldingenV2)} Bedrijf: '{ubn.Bedrijfsnummer}' Program: '{b.Programid}' - ";
            Facade.GetInstance().UpdateProgress(5, "I&R Versturen");
            Facade.GetInstance().getMeldingen().VulArrays(b.ProgId, pToken);
            string fcIR = DB.GetFarmConfigValue(b.FarmId, "IRviaModem", "True");
            string fcIDR = DB.GetFarmConfigValue(b.FarmId, "IDRviaModem", "True");
            unLogger.WriteDebug($"{prefix} Gestart. IRviaModem: '{fcIR}' IDRviaModem: '{fcIDR}'");
            List<SOAPLOG> SOAPResult = new List<SOAPLOG>();
            if (Convert.ToBoolean(fcIR))
            {
                CORE.DB.LABELSConst.ChangedBy _changedby = CORE.DB.LABELSConst.ChangedBy.UNKNOWN;
                try
                {
                    _changedby = (CORE.DB.LABELSConst.ChangedBy)changedby;
                }
                catch (Exception exc) 
                {
                    unLogger.WriteInfo($@"it is not possible to convert int:{changedby} to {nameof(CORE.DB.LABELSConst.ChangedBy)} ");
                }
                List<IRreportresult> MutResult = new List<IRreportresult>();
                if (MutResult.Count > 0 && ThrowImportFailure)
                {
                    throw new SoapIRException(MutResult, SoapIRException.SoapType.IR);
                }
                List<MUTATION> MutList = Facade.GetInstance().getMeldingen().ListIenRMeldingen(ubn.UBNid.ToString(), pToken);

                if (!MutList.Any())
                {
                    unLogger.WriteInfo($"{prefix} Geen klaarstaande meldingen.");
                    return SOAPResult;
                }

                List<MUTATION> lMutations = new List<MUTATION>();
                try
                {
                    unLogger.WriteInfo($@"{prefix} Aantal MUTATION: {MutList.Count} ");
                    var report1 = from n in MutList
                                  where n.Report >= 2
                                  select n;
                    var report2 = from n in MutList
                                  where n.Report < 2
                                  select n;

                    if (report2.Count() > 0)
                    {
                       
                        List<SOAPLOG> Results = Facade.GetInstance().getMeldingen().MeldIRV2(report2.ToList(), ubn, b, pToken,null, _changedby, sourceid);
                        foreach (var result in Results)
                        {
                            result.Changed_By = changedby;
                            result.SourceID = sourceid;
                            result.TaskLogID = result.TaskLogID == 0 ? tasklogid : result.TaskLogID;
                            Facade.GetInstance().getSaveToDB(pToken).WriteSoapError(result);
                        }
                        SOAPResult.AddRange(Results);

                        if ((b.ProgId == 3 || b.ProgId == 5) && Convert.ToBoolean(fcIDR))
                        {
                            foreach (var Record in report2)
                            {
                                try
                                {
                                    if (Record.IDRRace == "")
                                        Record.IDRRace = Facade.GetInstance().getSaveToDB(pToken).BepaalIDRRascode(b, Record.Lifenumber);

                                    Record.IDRRace = Facade.GetInstance().getSaveToDB(pToken).BepaalIDRRascode(b, Record.AlternateLifeNumber);
                                }
                                catch (Exception ex)
                                {
                                    if (Record.IDRRace == "")
                                        Record.IDRRace = "NN8";

                                    unLogger.WriteError($"{prefix} Bepaal IDR Rascode mislukt" + Record.Lifenumber, ex);
                                }
                                lMutations.Add(Record);
                            }
                        }
                    }
                    if (report1.Count() > 0)
                    {
                        foreach (var record in report1)
                        {
                            record.Changed_By = changedby;
                            record.SourceID = sourceid;
                            MUTALOG logrec = Facade.GetInstance().getMeldingen().ConverttoMutLog(record);
                            if (Facade.GetInstance().getMeldingen().InsertMutLog(logrec, pToken))
                            {
                                Facade.GetInstance().getMeldingen().DeleteMutation(record, pToken);
                            }
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    Facade.GetInstance().UpdateProgress(-1, "I&R Versturen Mislukt!" + ex.Message);
                    unLogger.WriteError($"{prefix} I&R verzenden Mislukt!", ex);
                }

                finally
                {
                    if (b.ProgId == 3 || b.ProgId == 5)
                    {
                        //lMutations is alleen gevuld als oa de setting IDRviaModem true is
                        List<gdauth> Lidmaatschap = Facade.GetInstance().getSaveToDB(pToken).getGDRelatieNrStamboeken().Where(stamboek => stamboek.ProgramId == b.Programid).ToList();
                        if (Lidmaatschap.Count == 0)
                        {
                            if ((b.ProgId == 3 || b.ProgId == 5) && lMutations.Count() > 0)
                            {
                                foreach (var m in lMutations)
                                {
                                    m.Changed_By = changedby;
                                }
                                Facade.GetInstance().getMeldingen().MeldIDRnaarGD(lMutations, ubn.UBNid, b.ProgId, b.Programid, pToken);
                            }
                        }
                    }
                }


                Facade.GetInstance().UpdateProgress(50, "I&R Versturen");
            }
            unLogger.WriteDebug($"{prefix} Klaar.");
            return SOAPResult;
        }

        /// <summary>
        /// Oude VerzendIRMeldingen, werkt maar vereist 32bit.
        /// </summary>
        /// <param name="pToken"></param>
        /// <param name="pProgramID"></param>
        /// <param name="ThrowImportFailure"></param>
        /// <returns></returns>
        [Obsolete("Meldingen via delphi 32bit vereist.")]
        public List<SOAPLOG> VerzendenIRMeldingen(UserRightsToken pToken, int pProgramID, bool ThrowImportFailure, int changedby, int sourceid)
        {
            var db = Facade.GetInstance().getSaveToDB(pToken);

            int pUBNId = pToken.getChildConnection(pProgramID).UBNId;
            int pProgId = pToken.getChildConnection(pProgramID).ProgId;
            int FarmId = pToken.getChildConnection(pProgramID).FarmId;

            UBN u = db.GetubnById(pUBNId);
            string prefix = $"{nameof(SendReceive)}.{nameof(VerzendenIRMeldingen)} Bedrijf: '{u.Bedrijfsnummer}' Program: '{pProgramID}' -";

            Facade.GetInstance().UpdateProgress(5, "I&R Versturen");
            Facade.GetInstance().getMeldingen().VulArrays(pProgId, pToken);

            FARMCONFIG fcIR = db.getFarmConfig(FarmId, "IRviaModem", "True");
            FARMCONFIG fcIDR = db.getFarmConfig(FarmId, "IDRviaModem", "True");

            unLogger.WriteDebug($"{prefix} Gestart. IRviaModem: '{fcIR?.FValue}' IDRviaModem: '{fcIDR?.FValue}'");

            List <SOAPLOG> SOAPResult = new List<SOAPLOG>();
            if (fcIR.ValueAsBoolean())
            {
                List<IRreportresult> MutResult = Facade.GetInstance().getMeldingen().ZetIenRMeldingenKlaar(pUBNId, pToken);
                if (MutResult.Count > 0 && ThrowImportFailure)
                {
                    throw new SoapIRException(MutResult, SoapIRException.SoapType.IR);
                }
                List<MUTATION> MutList = Facade.GetInstance().getMeldingen().ListIenRMeldingen(pUBNId.ToString(), pToken);
                List<MUTATION> lMutations = new List<MUTATION>();
                try
                {
                    unLogger.WriteDebug($@"{prefix} Aantal MUTATION: {MutList.Count} ");
                    foreach (MUTATION Record in MutList)
                    {
                        if (Record.Report < 2)
                        {
                            SOAPLOG Result = Facade.GetInstance().getMeldingen().MeldIR(Record, pUBNId, pProgId, pProgramID, pToken, null);
                            Facade.GetInstance().getSaveToDB(pToken).WriteSoapError(Result);
                            SOAPResult.Add(Result);
                            if ((pProgId == 3 || pProgId == 5) && fcIDR.ValueAsBoolean())
                            { 
                                BEDRIJF bedr = Facade.GetInstance().getSaveToDB(pToken).GetBedrijfByUbnIdProgIdProgramid(pUBNId, pProgId, pProgramID);
                                try
                                {
                                    if (Record.IDRRace == "") 
                                        Record.IDRRace = Facade.GetInstance().getSaveToDB(pToken).BepaalIDRRascode(bedr, Record.Lifenumber);

                                    Record.IDRRace = Facade.GetInstance().getSaveToDB(pToken).BepaalIDRRascode(bedr, Record.AlternateLifeNumber);
                                }
                                catch (Exception ex)
                                {
                                    if (Record.IDRRace == "") 
                                        Record.IDRRace = "NN8";

                                    unLogger.WriteError($"{prefix} Bepaal IDR Rascode mislukt" + Record.Lifenumber, ex);
                                }
                                lMutations.Add(Record);
                            }
                        }
                        else
                        {
                            MUTALOG logrec = Facade.GetInstance().getMeldingen().ConverttoMutLog(Record);
                            if (Facade.GetInstance().getMeldingen().InsertMutLog(logrec, pToken))
                            {
                                Facade.GetInstance().getMeldingen().DeleteMutation(Record, pToken);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Facade.GetInstance().UpdateProgress(-1, "I&R Versturen Mislukt!" + ex.Message);
                    unLogger.WriteError($"{prefix} I&R verzenden Mislukt!", ex);
                }

                finally
                {
                    if (pProgId == 3 || pProgId == 5)
                    {
                        //lMutations is alleen gevuld als oa de setting IDRviaModem true is
                        List<gdauth> Lidmaatschap = Facade.GetInstance().getSaveToDB(pToken).getGDRelatieNrStamboeken().Where(stamboek => stamboek.ProgramId == pProgramID).ToList();
                        if (Lidmaatschap.Count == 0)
                        {
                            if ((pProgId == 3 || pProgId == 5) && lMutations.Count() > 0)
                                Facade.GetInstance().getMeldingen().MeldIDRnaarGD(lMutations, pUBNId, pProgId, pProgramID, pToken);
                        }
                    }
                }


                Facade.GetInstance().UpdateProgress(50, "I&R Versturen");
            }
            unLogger.WriteInfo($"{prefix} Klaar.");
            return SOAPResult;
        }

        public List<SOAPLOG> VerzendenDHZmeldingen(UserRightsToken pToken, int pProgramID, bool ThrowImportFailure, int changedby, int sourceid, int TasklogID = 0)
        {
            int pUBNId = pToken.getChildConnection(pProgramID).UBNId;
            int pProgId = pToken.getChildConnection(pProgramID).ProgId;

            BEDRIJF bedr = new BEDRIJF { UBNid=pUBNId, ProgId=pProgId };
            if (TasklogID > 0)
            { 
                return VerzendenDHZmeldingen(pToken, bedr, ThrowImportFailure, changedby, sourceid, TasklogID); 
            }
            else
            {
                return VerzendenDHZmeldingen(pToken, bedr, ThrowImportFailure, changedby, sourceid);
            }
        }
        public List<SOAPLOG> VerzendenDHZmeldingen(UserRightsToken pToken, BEDRIJF bedr, bool ThrowImportFailure, int changedby, int sourceid, int TasklogID = 0)
        {
            int pUBNId = bedr.UBNid;
            int pProgId = bedr.ProgId;

            Facade.GetInstance().UpdateProgress(50, "DHZ Versturen");
            Facade.GetInstance().getMeldingen().VulArrays(pProgId, pToken);
 
            List<SOAPLOG> SOAPResult = new List<SOAPLOG>();
       
            List<DHZ> DHZList = Facade.GetInstance().getMeldingen().ListDHZMeldingen(pUBNId.ToString(), pToken);
            unLogger.WriteInfo($"Aantal DHZ meldingen: {DHZList.Count}");
            foreach (DHZ Record in DHZList)
            {
                if (Record.Report < 2)
                {
                    SOAPLOG Result = Facade.GetInstance().getMeldingen().MeldDHZ(Record, Convert.ToInt32(pUBNId), pToken, changedby, sourceid);
                    if (TasklogID > 0)
                    {
                        Result.TaskLogID = TasklogID;
                    }
                    Facade.GetInstance().getSaveToDB(pToken).WriteSoapError(Result);
                    SOAPResult.Add(Result);
                    if (Result.Code == "02")
                    {
                        unLogger.WriteWarn($"{nameof(SendReceive)}.{nameof(VerzendenDHZmeldingen)} - break on code '02' Result: '{Result}'.");
                        break;
                    }
                }
                else
                {
                    DHZLOG logrec = Facade.GetInstance().getMeldingen().ConverttoDHZLog(Record);
                    if (Facade.GetInstance().getMeldingen().InsertDHZLog(logrec, pToken))
                    {
                        Facade.GetInstance().getMeldingen().DeleteDHZ(Record, pToken);
                    }
                }
            }
            Facade.GetInstance().UpdateProgress(95, "DHZ Versturen");
            return SOAPResult;
        }

        public List<SOAPLOG> VerzendenTochtmeldingen(UserRightsToken pToken, int pProgramID, bool ThrowImportFailure, int changedby, int sourceid)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
          
            BEDRIJF b = DB.GetBedrijfById(pToken.getChildConnection(pProgramID).FarmId);
            int pProgId = pToken.getChildConnection(pProgramID).ProgId;
            b.ProgId = pProgId;
            b.Programid = pProgramID;
            return VerzendenTochtmeldingen(pToken, b, ThrowImportFailure, changedby, sourceid);
        }

        public List<SOAPLOG> VerzendenTochtmeldingen(UserRightsToken pToken, BEDRIJF bedr, bool ThrowImportFailure,int changedby,int sourceid)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
            UBN lUbn = DB.GetubnById(bedr.UBNid);
            FTPUSER ftp = null;
            int pProgId = bedr.ProgId;

            Facade.GetInstance().UpdateProgress(1, "Tocht Versturen");
            Facade.GetInstance().getMeldingen().VulArrays(pProgId, pToken);
            List<SOAPLOG> SOAPResult = new List<SOAPLOG>();
            Boolean lTestServer = configHelper.UseCRDTestserver;

            String fcTochtAuto = DB.GetConfigValue(bedr.Programid, bedr.FarmId, "TochtAutoReport", "False");
            String ReportWithUBNnr = DB.GetConfigValue(bedr.Programid, bedr.FarmId, "ReportTochtWithUBNnr", lUbn.Bedrijfsnummer);
            if (ReportWithUBNnr != lUbn.Bedrijfsnummer)
            {
                ftp = DB.GetFtpuser(DB.GetUBNidbyUBN(ReportWithUBNnr), 9991);
            }
            List<EVENT> UnreportedEvents = DB.getUnreportedHeatsByFarmId(bedr.FarmId, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_IenR_WS);
            if (UnreportedEvents.Count() == 0)
            {
                unLogger.WriteInfo($@"Tocht Versturen: Farmid:{bedr.FarmId} UBN:{lUbn.Bedrijfsnummer} geen UnreportedEvents gevonden.");
            }
            else
            {
                unLogger.WriteInfo($@"Tocht Versturen: Farmid:{bedr.FarmId} UBN:{lUbn.Bedrijfsnummer}  {UnreportedEvents.Count()} gevonden.");
            }

            foreach (EVENT lEvent in UnreportedEvents)
            {
                ANIMAL lAnimal = DB.GetAnimalById(lEvent.AniId);
                if (lAnimal.AniId > 0)
                {
                    if (Checker.IsValidLevensnummer(lAnimal.AniAlternateNumber, true, pProgId, bedr.Programid) != String.Empty)
                    {
                        unLogger.WriteInfo(String.Format("UBN[{1}] Levensnummer : {0} is ongeldig", lAnimal.AniAlternateNumber, lUbn.Bedrijfsnummer));
                        continue;
                    }
                    INHEAT inheat = DB.GetInHeat(lEvent.EventId);
                    SOAPLOG Result = new SOAPLOG();
                    Result.Changed_By = changedby;
                    Result.SourceID = sourceid;
                    if (fcTochtAuto == "True" && inheat.ReportDate <= (new DateTime(1900, 1, 1)) && inheat.HeatCertainty > 0 && inheat.HeatCertainty < 3)
                    {
                        //SOAPREPROALG.DLL
                        Result = Facade.GetInstance().getMeldingen().MeldTochtIndicatie(lUbn, lAnimal, lEvent, pToken, ftp, lTestServer, changedby, sourceid);
                        DB.WriteSoapError(Result);
                        SOAPResult.Add(Result);
                        unLogger.WriteInfo($@"UBN[{lUbn.Bedrijfsnummer}] Levensnummer : {lAnimal.AniAlternateNumber} :{Result.Status} {Result.Omschrijving}");
                        if (Result.Code == "RF0472" || Result.Code == "LOGINCRV")
                        {
                            break;
                        }
                    }
                    else if (inheat.ReportDate <= (new DateTime(1900, 1, 1)) && inheat.HeatCertainty == 0 && lEvent.EveMutationBy == 29 && lEvent.EveMutationDate >= DateTime.Today.AddDays(-1))
                    {
                        //VSM.RUMA.CRVSOAP.CrvVruchtbaarheid
                        Result = Facade.GetInstance().getMeldingen().MeldTocht(lUbn, lAnimal, lEvent, pToken, changedby, sourceid);
                        DB.WriteSoapError(Result);
                        SOAPResult.Add(Result);
                        unLogger.WriteInfo($@"UBN[{lUbn.Bedrijfsnummer}] Levensnummer : {lAnimal.AniAlternateNumber} :{Result.Status} {Result.Omschrijving}");
                        if (Result.Code == "RF0472" || Result.Code == "LOGINCRV")
                        {
                            break;
                        }
                    }
                    else if (inheat.ReportDate <= new DateTime(1900, 1, 1))
                    {
                        unLogger.WriteInfo($@"UBN[{lUbn.Bedrijfsnummer}] Niet verzonden: Levensnummer : {lAnimal.AniAlternateNumber} :{Result.Status} {Result.Omschrijving}");
                        unLogger.WriteInfo($@"fcTochtAuto:{fcTochtAuto},HeatCertainty:{inheat.HeatCertainty},EveMutationBy:{lEvent.EveMutationBy},ReportDate:{inheat.ReportDate.ToString()},EveMutationDate:{lEvent.EveMutationDate.ToString()}");

                        Result.Date = DateTime.Now;
                        Result.Time = Result.Date;
                        Result.Status = "I";
                        Result.Code = "";
                        Result.Omschrijving = $@"fcTochtAuto:{fcTochtAuto},HeatCertainty:{inheat.HeatCertainty},EveMutationBy:{lEvent.EveMutationBy},ReportDate:{inheat.ReportDate.ToString()},EveMutationDate:{lEvent.EveMutationDate.ToString()}";
                        Result.FarmNumber = lUbn.Bedrijfsnummer;
                        Result.Lifenumber = lAnimal.AniAlternateNumber;
                       
                        DB.WriteSoapError(Result);
                        SOAPResult.Add(Result);
                    }

                }

            }


            Facade.GetInstance().UpdateProgress(95, "Tocht Versturen");
            return SOAPResult;
        }

        public List<SOAPLOG> VerzendenDrachtStatusen(UserRightsToken pToken, BEDRIJF bedr, bool ThrowImportFailure, int changedby, int sourceid)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
            UBN lUbn = DB.GetubnById(bedr.UBNid);
       
            Facade.GetInstance().UpdateProgress(1, "Drachtstatus Versturen");
            Facade.GetInstance().getMeldingen().VulArrays(bedr.ProgId, pToken);
            List<SOAPLOG> SOAPResult = new List<SOAPLOG>();
            Boolean lTestServer = configHelper.UseCRDTestserver;
            List<EVENT> UnreportedEvents = DB.getUnreportedEventsByFarmId(bedr.FarmId, VSM.RUMA.CORE.DB.LABELSConst.EventKind.DRACHTIGHEID, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_IenR_WS);
            if (UnreportedEvents.Count() == 0)
            {
                unLogger.WriteInfo($@"Drachtstatus Versturen: Farmid:{bedr.FarmId} UBN:{lUbn.Bedrijfsnummer} geen UnreportedEvents gevonden.");
            }
            else
            {
                unLogger.WriteInfo($@"Drachtstatus Versturen: Farmid:{bedr.FarmId} UBN:{lUbn.Bedrijfsnummer}  {UnreportedEvents.Count()} gevonden.");
            }
            foreach (EVENT lEvent in UnreportedEvents)
            {

                ANIMAL lAnimal = DB.GetAnimalById(lEvent.AniId);
                if (lAnimal.AniId > 0)
                {
                    if (Checker.IsValidLevensnummer(lAnimal.AniAlternateNumber, true, bedr.ProgId, bedr.Programid) != String.Empty)
                    {
                        unLogger.WriteDebug("Levensnummer :" + lAnimal.AniAlternateNumber + " is ongelding");
                        continue;
                    }
                    SOAPLOG Result = Facade.GetInstance().getMeldingen().MeldDrachtControle(lUbn, lAnimal, lEvent, pToken, changedby, sourceid);
                    Facade.GetInstance().getSaveToDB(pToken).WriteSoapError(Result);
                    SOAPResult.Add(Result);
                    unLogger.WriteInfo($@"UBN[{lUbn.Bedrijfsnummer}] Levensnummer : {lAnimal.AniAlternateNumber} :{Result.Status} {Result.Omschrijving}");
                    if (Result.Status.ToUpper() == "F" && Result.Code == "02")//U bent niet geautoriseerd om met opgegeven gebruikersnaam / wachtwoord in te loggen
                    {
                        break;
                    }
                }
            }


            Facade.GetInstance().UpdateProgress(95, "Drachtstatus Versturen voltooid...");
            return SOAPResult;
        }
        public List<SOAPLOG> VerzendenDrachtStatusen(UserRightsToken pToken, int pProgramID, bool ThrowImportFailure, int changedby, int sourceid)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
            UBN lUbn = DB.GetubnById(pToken.getChildConnection(pProgramID).UBNId);
            int pProgId = pToken.getChildConnection(pProgramID).ProgId;
            Facade.GetInstance().UpdateProgress(1, "Drachtstatus Versturen");
            Facade.GetInstance().getMeldingen().VulArrays(pProgId, pToken);
            List<SOAPLOG> SOAPResult = new List<SOAPLOG>();
            Boolean lTestServer = configHelper.UseCRDTestserver;
            List<EVENT> UnreportedEvents = DB.getUnreportedEventsByFarmId(pToken.getChildConnection(pProgramID).FarmId, VSM.RUMA.CORE.DB.LABELSConst.EventKind.DRACHTIGHEID, VSM.RUMA.CORE.DB.LABELSConst.ReportOrganization.CRV_IenR_WS);
            if (UnreportedEvents.Count() == 0)
            {
                unLogger.WriteInfo($@"Drachtstatus Versturen: Farmid:{pToken.getChildConnection(pProgramID).FarmId} UBN:{lUbn.Bedrijfsnummer} geen UnreportedEvents gevonden.");
            }
            else
            {
                unLogger.WriteInfo($@"Drachtstatus Versturen: Farmid:{pToken.getChildConnection(pProgramID).FarmId} UBN:{lUbn.Bedrijfsnummer}  {UnreportedEvents.Count()} gevonden.");
            }
            foreach (EVENT lEvent in UnreportedEvents)
            {

                ANIMAL lAnimal = DB.GetAnimalById(lEvent.AniId);
                if (lAnimal.AniId > 0)
                {
                    if (Checker.IsValidLevensnummer(lAnimal.AniAlternateNumber, true, pProgId, pProgramID) != String.Empty)
                    {
                        unLogger.WriteDebug("Levensnummer :" + lAnimal.AniAlternateNumber + " is ongelding");
                        continue;
                    }
                    SOAPLOG Result = Facade.GetInstance().getMeldingen().MeldDrachtControle(lUbn, lAnimal, lEvent, pToken, changedby, sourceid);
                    Facade.GetInstance().getSaveToDB(pToken).WriteSoapError(Result);
                    SOAPResult.Add(Result);
                    unLogger.WriteInfo($@"UBN[{lUbn.Bedrijfsnummer}] Levensnummer : {lAnimal.AniAlternateNumber} :{Result.Status} {Result.Omschrijving}");
                    if (Result.Status.ToUpper()=="F" && Result.Code == "02")//U bent niet geautoriseerd om met opgegeven gebruikersnaam / wachtwoord in te loggen
                    {
                        break;
                    }
                }
            }


            Facade.GetInstance().UpdateProgress(95, "Drachtstatus Versturen voltooid...");
            return SOAPResult;
        }


        private void FTPverzenden(int pUBNId, UserRightsToken pToken)
        {
            Facade.GetInstance().UpdateProgress(1, "FTP Verzenden");

            List<FTPINFO> FtpInfoList = Facade.GetInstance().getSaveToDB(pToken).GetFTPINFO(pUBNId);
            int perc = 1;
            foreach (FTPINFO lFtpInfo in FtpInfoList)
            {
                if (lFtpInfo.FtpHostName != String.Empty)
                {
                    perc = FtpInfoList.IndexOf(lFtpInfo) / FtpInfoList.Count;

                    Facade.GetInstance().UpdateProgress(perc - 2, "FTP " + lFtpInfo.FtpHostName);

                    FTP FTPnrs = new FTP(lFtpInfo.FtpHostName, lFtpInfo.UserName, lFtpInfo.Password, lFtpInfo.PassiveMode == 1);
                    if (lFtpInfo.Direction == 1)
                    {
                        FTPnrs.UploadtoDir(lFtpInfo.DirectoryFrom, lFtpInfo.DirectoryTo, lFtpInfo.AfterTransfer);
                    }
                }
            }
            Facade.GetInstance().UpdateProgress(99, "FTP Verzenden");
        }

        private void FTPontvangen(int pUBNId, UserRightsToken pToken)
        {
            Facade.GetInstance().UpdateProgress(1, "FTP Ontvangen");
            String DatacomPath = Facade.GetInstance().getDatacomPath();

            List<FTPINFO> FtpInfoList = Facade.GetInstance().getSaveToDB(pToken).GetFTPINFO(pUBNId);
            int perc = 1;
            foreach (FTPINFO lFtpInfo in FtpInfoList)
            {
                if (lFtpInfo.FtpHostName != String.Empty)
                {
                    perc = FtpInfoList.IndexOf(lFtpInfo) / FtpInfoList.Count;

                    Facade.GetInstance().UpdateProgress(perc, "FTP " + lFtpInfo.FtpHostName);

                    FTP FTPnrs = new FTP(lFtpInfo.FtpHostName, lFtpInfo.UserName, lFtpInfo.Password, lFtpInfo.PassiveMode == 1);
                    if (lFtpInfo.Direction == 0)
                    {
                        FTPnrs.DownloadFromDir(DatacomPath + lFtpInfo.DirectoryTo, lFtpInfo.DirectoryFrom, lFtpInfo.AfterTransfer);
                    }
                }
            }
            Facade.GetInstance().UpdateProgress(99, "FTP Ontvangen");
        }

        private void InlezenEDINRS(int pThrId, UserRightsToken pToken, int pProgramID)
        {
            int pUBNId = pToken.getChildConnection(pProgramID).UBNId;
            int pProgId = pToken.getChildConnection(pProgramID).ProgId;
            int pFarmId = pToken.getChildConnection(pProgramID).FarmId;

            Facade.GetInstance().UpdateProgress(5, "Bestanden Inlezen");
            String Datacompad = Facade.GetInstance().getSaveToDB(pToken).GetConfigValue(pProgramID, pFarmId, "EDINRSAlternatePath", unDatacomReader.DatacomInDir);
            String Extension = "*";//"VEE";
            if (Datacompad != unDatacomReader.DatacomInDir && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                unDatacomReader.DatacomInDir = Datacompad;
                Extension = "*";
            }
            VSM.RUMA.CORE.EDINRS.AFcomEDINRS EDINRS = Facade.GetInstance().getEDINRS(pToken);
            List<String> lFileList = GetEDINRSFiles(EDINRS, pToken, Extension);
            foreach (String lFilename in lFileList)
            {
                unLogger.WriteDebugFormat("Gebruikerscode {0} Start EDINRS import bestand {1}", pToken.User, lFilename);
                if (EDINRS.LeesHeader(lFilename) == DateTime.MinValue)
                {
                    Facade.GetInstance().UpdateProgress(5, lFilename + " Bestand is geen EDINRS Bestand");
                    unLogger.WriteError(lFilename + " Import Mislukt!");
                    int ok = FileLogger.AddFileLog(Facade.GetInstance().getSaveToDB(pToken), 0, "", lFilename, "errors.log", 101);
                }
                else if (!EDINRS.LeesBestand(pThrId, lFilename, pUBNId, pProgId, pProgramID))
                {

                    Facade.GetInstance().getSaveToDB(pToken).WriteLogMessage(pUBNId,3,lFilename + " Import Failed!");
                    unLogger.WriteError(lFilename + " Import Mislukt!");
                    int ok = FileLogger.AddFileLog(Facade.GetInstance().getSaveToDB(pToken), 0, "", lFilename, "errors.log", 101);
                }
                else if (Extension == "VEE")
                {
                    unDatacomReader.RenameFileDatacomIn(lFilename, "edinrs" + DateTime.Now.Ticks);
                }
                else
                {
                    unLogger.WriteDebugFormat("Gebruikerscode {0} bestand {1} ingelezen", pToken.User, lFilename);
                    int ok = FileLogger.AddFileLog(Facade.GetInstance().getSaveToDB(pToken), 0, "", lFilename, "errors.log", 1);
                }
            }
        }

        private List<String> GetEDINRSFiles(VSM.RUMA.CORE.EDINRS.AFcomEDINRS EDINRS, UserRightsToken pToken, String Extension)
        {
            List<String> lFileList = Facade.GetInstance().SortFilebyAdisHeader(unDatacomReader.GetFilesDatacomIn("*"), pToken);
            List<String> result = new List<string>();

            //Facade.GetInstance().getSaveToDB(pToken).getfile
            foreach (String lFilename in lFileList)
            {
                if (!System.IO.Path.GetExtension(lFilename).StartsWith(".!") && EDINRS.LeesHeader(lFilename) != DateTime.MinValue)
                {
                    FILELOG log = Facade.GetInstance().getSaveToDB(pToken).getFileLogByHostnameAndFileName(System.Net.Dns.GetHostName(), lFilename.Replace("\\", "\\\\"));
                    if (log.Filelog_id == 0)
                        result.Add(lFilename);
                }
            }
            return result;
        }

        public void InlezenEDINRSSOAP(UserRightsToken pToken, int pProgramID)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
            int pUBNId = pToken.getChildConnection(pProgramID).UBNId;
            int ProgId = pToken.getChildConnection(pProgramID).ProgId;
            int FarmId = pToken.getChildConnection(pProgramID).FarmId;
            FARMCONFIG fcRepro = DB.getFarmConfig(FarmId, "CRVSOAPImport", "False");

            if ((ProgId == 1 || ProgId == 4) && fcRepro.ValueAsBoolean())
            {
                try
                {
                    Win32SOAPIMPNRS dllcall = new Win32SOAPIMPNRS();
                    Win32SOAPIMPNRS.Callback pCallback = new Win32SOAPIMPNRS.Callback(UpdateCallback);
                    String Status = String.Empty;
                    String Code = String.Empty;
                    String Omschrijving = String.Empty;
                    int AantDieren = 0;
                    int pMaxStrLen = 255;
                    bool pCRDtestserver = configHelper.UseCRDTestserver;

                    String lLand = GetCountry(pToken, pUBNId).LandAfk2;
                    UBN ubn = DB.GetubnById(pUBNId);
                    String LogFile = unLogger.getLogDir("rsReproduction");
                    FTPUSER fusoap;



                    String ReportWithUBNnr = DB.GetConfigValue(pProgramID, FarmId, "ReportTochtWithUBNnr", ubn.Bedrijfsnummer);
                    String fcTochtAuto = DB.GetConfigValue(pProgramID, FarmId, "TochtAutoReport", "False");
                    String SOAPWithGroup = DB.GetConfigValue(pProgramID, FarmId, "CRVSOAPWithTochtGroup", "False");
                    if (fcTochtAuto == "True" && SOAPWithGroup == "True" && ReportWithUBNnr != ubn.Bedrijfsnummer)
                    {
                        fusoap = DB.GetFtpuser(DB.GetUBNidbyUBN(ReportWithUBNnr), 9991);
                    }
                    else
                    {
                        fusoap = DB.GetFtpuser(pUBNId, 9991);
                    }

                    String lUsername = fusoap.UserName;
                    String lPassword = fusoap.Password;

                    SOAPLOG LogResult = new SOAPLOG();

                    if (lUsername != String.Empty || lPassword != String.Empty)
                    {
                        dllcall.CRDAnimalReproduction(ProgId, pProgramID,
                                        lLand, ubn.Bedrijfsnummer, pToken.User, pToken.Password,
                                        pToken.Host, LogFile,
                                        ReportWithUBNnr, lUsername, lPassword,
                                        pCRDtestserver, pCallback,
                                        ref AantDieren,
                                        ref Status, ref Code, ref Omschrijving, pMaxStrLen);
                    }

                    LogResult.Date = DateTime.Today;
                    LogResult.Time = DateTime.Now;
                    LogResult.Kind = 1110;
                    LogResult.Status = Status;
                    LogResult.Code = Code;
                    LogResult.Omschrijving = Omschrijving;
                    LogResult.FarmNumber = ubn.Bedrijfsnummer;
                    DB.WriteSoapError(LogResult);


                    Facade.GetInstance().UpdateProgress(98, "Ophalen Actieve Stallijst Voltooid.");
                    unLogger.WriteInfo(String.Format("UBN[{0}] RsReproduction Import voltooid!", ubn.Bedrijfsnummer));
                }
                catch (Exception ex)
                {
                    DB.WriteLogMessage(pUBNId, 1116,"rsReproduction Import Failed!");
                    unLogger.WriteError("rsReproduction Import Mislukt!" + ex.Message, ex);
                    Facade.GetInstance().UpdateProgress(-1, "Fout bij het ophalen van de actieve stallijst CRV! \r\n" + ex.Message);
                }
            }
            Facade.GetInstance().UpdateProgress(98, "Ophalen Actieve Stallijst Voltooid.");

        }

        private static COUNTRY GetCountry(DBConnectionToken mToken, int pUBNid)
        {
            COUNTRY country;
            try
            {
                UBN UBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
                THIRD thrid = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(UBN.ThrID);
                country = Facade.GetInstance().getSaveToDB(mToken).GetCountryByLandid(Convert.ToInt32(thrid.ThrCountry));
            }
            catch (Exception ex)
            {
                unLogger.WriteError(String.Format("Fout bij het opvragen van het land: {0}", ex.Message), ex);
                country = Facade.GetInstance().getSaveToDB(mToken).GetCountryByLandid(151);
            }
            return country;
        }


        protected void UpdateCallback(int PercDone, string Msg)
        {

        }

        //private void InlezenEDINRSSOAP(UserRightsToken pToken, int pProgramID)
        //{
        //    int pUBNId = pToken.getChildConnection(pProgramID).UBNId;
        //    int pProgId = pToken.getChildConnection(pProgramID).ProgId;

        //    Facade.GetInstance().UpdateProgress(5, "Gegevens ophalen bij NRS");
        //    EDINRS.EDINRSSOAP soap = new VSM.RUMA.CORE.EDINRS.EDINRSSOAP(Facade.GetInstance().getSaveToDB(pToken));
        //    soap.LeesBedrijf(pUBNId);
        //}

        //private void InlezenSlachtBestandenBBS(UserRightsToken pToken, int pProgramID)
        //{
        //    int pProgId = pToken.getChildConnection(pProgramID).ProgId;
        //    int pUBNId = pToken.getChildConnection(pProgramID).UBNId;

        //    UBN ubn = Facade.GetInstance().getSaveToDB(pToken).GetubnById(pUBNId);
        //    String sUBN = ubn.Bedrijfsnummer;

        //    Facade.GetInstance().getEdinrs().LeesBBS(pProgramID, pProgId, sUBN, pToken);
        //}
    }
}
