using System;
using System.ComponentModel;
using System.Collections.Generic;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Text;
using System.Reflection;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE
{
    abstract public class AFIenRMeldingen : VSM.RUMA.CORE.IIenRMeldingen
    {
        public abstract void VulArrays(int pProgId, DBConnectionToken mToken);

        public abstract List<IRreportresult> ZetIenRMeldingenKlaar(int pUBNId, DBConnectionToken mToken);
        public abstract List<IRreportresult> ZetDHZMeldingenKlaar(int pUBNId, DBConnectionToken mToken);
        public abstract string getdefIenRaction(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramId);
        public abstract List<MUTATION__ADMIN> ListIenRAdmin(int pProgramId, DBConnectionToken mToken);
        public abstract List<MUTALOG__ADMIN> ListVorigeIenRAdmin(int pProgramId, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public abstract List<MUTATION> ListIenRMeldingen(String pUBNid, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public abstract List<MUTALOG> ListVorigeIenR(String pUBNid, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public abstract List<MUTATION> ListIenRMeldingenHond(int pFokkerThrId, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public abstract List<MUTALOG> ListVorigeIenRHond(int pFokkerThrId, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public abstract List<DHZ> ListDHZMeldingen(String pUBNid, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public abstract List<DHZLOG> ListVorigeDHZ(String pUBNid, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public abstract bool InsertMutation(MUTATION pMutation, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public abstract bool InsertMutLog(MUTALOG pMutLog, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public abstract bool InsertDHZ(DHZ pDHZ, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public abstract bool InsertDHZLog(DHZLOG pDHZLog, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public abstract bool UpdateReport(MUTATION pMutation, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public abstract bool DeleteMutation(MUTATION pMutation, DBConnectionToken mToken);
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public abstract bool DeleteDHZ(DHZ pDHZ, DBConnectionToken mToken);
        public abstract String getMutSoort(int CodeMutation);
        public abstract String getDHZSoort(int InsInfo);
        public abstract String getMutGeslacht(int Sex);
        public abstract String getMutHaarKleur(int Haircolor, string pAniHaircolor_Memo);
        public abstract int LNVPasswordCheck(String pUsername, String pPassword);
        public abstract String getMutResultaat(int Returnresult);
        public abstract List<SOAPLOG> MeldIRV2(List<MUTATION> pRecords, UBN ubn, BEDRIJF b, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, LABELSConst.ChangedBy changed = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0);
        public abstract SOAPLOG MeldIR(MUTATION pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, LABELSConst.ChangedBy changed = LABELSConst.ChangedBy.UNKNOWN, int sourceId = 0);
        public abstract SOAPLOG LNV2MeldingIntrekken(MUTALOG pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken);

        public abstract SOAPLOG LNV2MeldingIntrekken(MUTALOG pRecord, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken, bool usesoap = false);

        public abstract SOAPLOG LNVIRRaadplegenMeldingenAlg(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                                    String lLevensnr, int MeldingType, int MeldingStatus,
                                                    String UBNnr2ePartijd, DateTime Begindatum, DateTime Einddatum, int pIndGebeurtenisdatum, String OutputFile);

        public abstract SOAPLOG STRaadplegenMeldingenAlg(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramid,
                                    String lLevensnr, int SoortMelding, DateTime Begindatum, DateTime Einddatum, String OutputFile);

        public abstract SOAPLOG IRHondRaadplegenmeldingen(DBConnectionToken mToken, THIRD pRaadpleger, string pIpAdress, int pFarmId, Int64 pChipnummer, DateTime pStartDate, DateTime pEndDate, string pMeldingenFilecsv, string pRelatieFilecsv);
        public abstract SOAPLOG LNVIRRaadplegenMeldingDetailsV2(DBConnectionToken mToken, int pUBNid, int pProgId, int pProgramId,
                                             String MeldingNummer, int MeldingType, int MeldingStatus,
                                             DateTime Begindatum, String lLevensnr, String lLevensnr_Nieuw);


        public abstract SOAPLOG LNVIROntbrekendeMeldingenV2(DBConnectionToken mToken, int pUBNid,
                                            int pProgId, int pProgramid, int MeldingType,
                                             DateTime Begindatum, DateTime Einddatum,
                                            ref String csvMeldingType, ref String csvUBNnr2ePartij, ref  String csvDatum);

  
        public abstract SOAPLOG MeldTocht(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken, int changedby, int sourceid);
        //public abstract SOAPLOG MeldTochtIndicatie(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken);
        public abstract SOAPLOG MeldTochtIndicatie(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken, FTPUSER pCRVSOAPCredidentials, Boolean pTestServer, int changedby, int sourceid);
        public abstract SOAPLOG MeldDrachtControle(UBN pUBN, ANIMAL pAnimal, EVENT pEvent, DBConnectionToken mToken, int changedby, int sourceid);
        public abstract void MeldIDRnaarGD(List<MUTATION> pRecords, int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken);
        public abstract int maakIDRvoorraad(int pUBNid, int pProgId, int pProgramid, DBConnectionToken mToken);
        public abstract void MeldStamboekIDRnaarGD(List<MUTATION> pRecords, string StamBoeknr, int pProgId, DBConnectionToken mToken, out int iOk, out int iError);
        public abstract SOAPLOG MeldDHZ(DHZ pRecord, int pUBNid, DBConnectionToken mToken, int changedby, int sourceid);
        public abstract String Plugin();
        protected abstract String STIRHaarKleur(int Haircolor, string pAniHaircolor_Memo);
        protected abstract String LNVIRHaarKleur(int Haircolor, string pAniHaircolor_Memo);
        protected abstract String CRDIRHaarKleur(int Haircolor, String Land, string pAniHaircolor_Memo);

        public abstract SOAPLOG MeldIRLNVV2IR(MUTATION pRecord, int pUBNid, int pProgId, int pProgramId, DBConnectionToken mToken, FTPUSER pLNV2IRCredidentials, bool usesoap = false);
        protected String CRDIRGeslacht(int Sex)
        {
            switch (Sex)
            {
                case 1: return "M";
                case 2: return "V";
                case 3: return "";
                default: return "";
            }
        }

        protected String LNVIRGeslacht(int Sex)
        {
            switch (Sex)
            {
                case 1: return "M";
                case 2: return "V";
                default: return "O";
            }
        }

        public MUTATION ConverttoMutation(MUTALOG pMutLog)
        {
            MUTATION lMutation = new MUTATION();
            
            lMutation.AlternateLifeNumber = pMutLog.AlternateLifeNumber;
            lMutation.AniState = pMutLog.AniState;
            lMutation.AniSubType = pMutLog.AniSubType;
            lMutation.CalvingEase = pMutLog.CalvingEase;
            lMutation.CodeMutation = pMutLog.CodeMutation;
            lMutation.CountryCodeBirth = pMutLog.CountryCodeBirth;
            lMutation.CountryCodeDepart = pMutLog.CountryCodeDepart;
            lMutation.CullingReason = pMutLog.CullingReason;
            lMutation.ET = pMutLog.ET;
            lMutation.EventId = pMutLog.EventId;
            lMutation.Farmnumber = pMutLog.Farmnumber;
            lMutation.FarmNumberFrom = pMutLog.FarmNumberFrom;
            lMutation.FarmNumberTo = pMutLog.FarmNumberTo;
            lMutation.Haircolor = pMutLog.Haircolor;
            lMutation.AniHaircolor_Memo = pMutLog.AniHaircolor_Memo;
            lMutation.IDRBirthDate = pMutLog.IDRBirthDate;
            lMutation.IDRLossDate = pMutLog.IDRLossDate;
            lMutation.IDRRace = pMutLog.IDRRace;
            lMutation.ISONumber = pMutLog.ISONumber;
            lMutation.LicensePlate = pMutLog.LicensePlate;
            lMutation.LifenrOrigin = pMutLog.LifenrOrigin;
            lMutation.Lifenumber = pMutLog.Lifenumber;
            lMutation.LifeNumberFather = pMutLog.LifeNumberFather;
            lMutation.LifenumberMother = pMutLog.LifenumberMother;
            lMutation.LifeNumberUnique = pMutLog.LifeNumberUnique;
            lMutation.LossDate = pMutLog.LossDate;
            lMutation.MeatScore = pMutLog.MeatScore;
            lMutation.MeldingNummer = pMutLog.MeldingNummer;

            lMutation.MotherBoughtRecent = pMutLog.MotherBoughtRecent;
            lMutation.MovId = pMutLog.MovId;
            lMutation.MutationDate = pMutLog.MutationDate;
            lMutation.MutationTime = pMutLog.MutationTime;
            lMutation.Name = pMutLog.Name;
            lMutation.Nling = pMutLog.Nling;
            lMutation.Program = pMutLog.Program;
            lMutation.Purchaser = pMutLog.Purchaser;
            lMutation.Race = pMutLog.Race;
            lMutation.RegistrationCard = pMutLog.RegistrationCard;
            lMutation.RendacAniType = pMutLog.RendacAniType;
            lMutation.Report = pMutLog.Report;
            lMutation.ReportDate = pMutLog.ReportDate;
            lMutation.ReportForBuyer = pMutLog.ReportForBuyer;
            lMutation.ReportTime = pMutLog.ReportTime;
            lMutation.Returnresult = pMutLog.Returnresult;
            lMutation.SanitairyUnit = pMutLog.SanitairyUnit;
            lMutation.SendTo = pMutLog.SendTo;
            lMutation.Sex = pMutLog.Sex;
            lMutation.Slaughter = pMutLog.Slaughter;
            lMutation.Speciality = pMutLog.Speciality;
            lMutation.Subsidy = pMutLog.Subsidy;
            lMutation.UbnId = pMutLog.UbnId;
            lMutation.VersienrVertrekluik = pMutLog.VersienrVertrekluik;
            lMutation.Vervoersnr = pMutLog.Vervoersnr;
            lMutation.Weight = pMutLog.Weight;
            lMutation.Within7days = pMutLog.Within7days;
            lMutation.Worknumber = pMutLog.Worknumber;
            lMutation.ThrID = pMutLog.ThrID;
            lMutation.tbv_ThrID = pMutLog.tbv_ThrID;
            lMutation.NrGezondheidsCert = pMutLog.NrGezondheidsCert;
            lMutation.MeldResult = pMutLog.MeldResult;
            lMutation.Changed_By = pMutLog.Changed_By;
            lMutation.SourceID = pMutLog.SourceID;
            return lMutation;
        }



        public MUTALOG ConverttoMutLog(MUTATION pMutation)
        {
            MUTALOG lMutLog = new MUTALOG();

            lMutLog.AlternateLifeNumber = pMutation.AlternateLifeNumber;
            lMutLog.AniState = pMutation.AniState;
            lMutLog.AniSubType = pMutation.AniSubType;
            lMutLog.CalvingEase = pMutation.CalvingEase;
            lMutLog.CodeMutation = pMutation.CodeMutation;
            lMutLog.CountryCodeBirth = pMutation.CountryCodeBirth;
            lMutLog.CountryCodeDepart = pMutation.CountryCodeDepart;
            lMutLog.CullingReason = pMutation.CullingReason;
            lMutLog.ET = pMutation.ET;
            lMutLog.EventId = pMutation.EventId;
            lMutLog.Farmnumber = pMutation.Farmnumber;
            lMutLog.FarmNumberFrom = pMutation.FarmNumberFrom;
            lMutLog.FarmNumberTo = pMutation.FarmNumberTo;
            lMutLog.Haircolor = pMutation.Haircolor;
            lMutLog.AniHaircolor_Memo = pMutation.AniHaircolor_Memo;
            lMutLog.IDRBirthDate = pMutation.IDRBirthDate;
            lMutLog.IDRLossDate = pMutation.IDRLossDate;
            lMutLog.IDRRace = pMutation.IDRRace;
            lMutLog.ISONumber = pMutation.ISONumber;
            lMutLog.LicensePlate = pMutation.LicensePlate;
            lMutLog.LifenrOrigin = pMutation.LifenrOrigin;
            lMutLog.Lifenumber = pMutation.Lifenumber;
            lMutLog.LifeNumberFather = pMutation.LifeNumberFather;
            lMutLog.LifenumberMother = pMutation.LifenumberMother;
            lMutLog.LifeNumberUnique = pMutation.LifeNumberUnique;
            lMutLog.LossDate = pMutation.LossDate;
            lMutLog.MeatScore = pMutation.MeatScore;
            lMutLog.MeldingNummer = pMutation.MeldingNummer;

            lMutLog.MotherBoughtRecent = pMutation.MotherBoughtRecent;
            lMutLog.MovId = pMutation.MovId;
            lMutLog.MutationDate = pMutation.MutationDate;
            lMutLog.MutationTime = pMutation.MutationTime;
            lMutLog.Name = pMutation.Name;
            lMutLog.Nling = pMutation.Nling;
            lMutLog.Program = pMutation.Program;
            lMutLog.Purchaser = pMutation.Purchaser;
            lMutLog.Race = pMutation.Race;
            lMutLog.RegistrationCard = pMutation.RegistrationCard;
            lMutLog.RendacAniType = pMutation.RendacAniType;
            lMutLog.Report = pMutation.Report;
            lMutLog.ReportDate = pMutation.ReportDate;
            lMutLog.ReportForBuyer = pMutation.ReportForBuyer;
            lMutLog.ReportTime = pMutation.ReportTime;
            lMutLog.Returnresult = pMutation.Returnresult;
            lMutLog.SanitairyUnit = pMutation.SanitairyUnit;
            lMutLog.SendTo = pMutation.SendTo;
            lMutLog.Sex = pMutation.Sex;
            lMutLog.Slaughter = pMutation.Slaughter;
            lMutLog.Speciality = pMutation.Speciality;
            lMutLog.Subsidy = pMutation.Subsidy;
            lMutLog.UbnId = pMutation.UbnId;
            lMutLog.VersienrVertrekluik = pMutation.VersienrVertrekluik;
            lMutLog.Vervoersnr = pMutation.Vervoersnr;
            lMutLog.Weight = pMutation.Weight;
            lMutLog.Within7days = pMutation.Within7days;
            lMutLog.Worknumber = pMutation.Worknumber;
            lMutLog.ThrID = pMutation.ThrID;
            lMutLog.tbv_ThrID = pMutation.tbv_ThrID;
            lMutLog.NrGezondheidsCert = pMutation.NrGezondheidsCert;
            lMutLog.MeldResult = pMutation.MeldResult;
            lMutLog.Changed_By = pMutation.Changed_By;
            lMutLog.SourceID = pMutation.SourceID;
            return lMutLog;
        }
        
        public DHZLOG ConverttoDHZLog(DHZ pDHZ)
        {
            DHZLOG lDHZLog = new DHZLOG();

            lDHZLog.Internalnr = pDHZ.Internalnr;
            lDHZLog.FarmNumber = pDHZ.FarmNumber;
            lDHZLog.UbnId = pDHZ.UbnId;
            lDHZLog.InsDate = pDHZ.InsDate;
            lDHZLog.ReportDate = pDHZ.ReportDate;
            lDHZLog.ReportTime = pDHZ.ReportTime;
            lDHZLog.AniLifenumber = pDHZ.AniLifenumber;
            lDHZLog.AniWorknumber = pDHZ.AniWorknumber;
            lDHZLog.AniName = pDHZ.AniName;
            lDHZLog.BullAInumber = pDHZ.BullAInumber;
            lDHZLog.BullLifeNumber = pDHZ.BullLifeNumber;
            lDHZLog.BullName = pDHZ.BullName;
            lDHZLog.ChargeNumber = pDHZ.ChargeNumber;
            //lMutLog.AniId = pMutation.AniId;
            lDHZLog.Frozen = pDHZ.Frozen;
            lDHZLog.InsInfo = pDHZ.InsInfo;
            lDHZLog.Report = pDHZ.Report;
            lDHZLog.ReportTo = pDHZ.ReportTo;
            lDHZLog.InsAmount = pDHZ.InsAmount;
            lDHZLog.Imported = pDHZ.Imported;
            lDHZLog.Inseminator = pDHZ.Inseminator;
            lDHZLog.EventID = pDHZ.EventID;
            lDHZLog.InsNumber = pDHZ.InsNumber;
            lDHZLog.Changed_By = pDHZ.Changed_By;
            lDHZLog.SourceID = pDHZ.SourceID;
            return lDHZLog;
        }
        
    }
}
