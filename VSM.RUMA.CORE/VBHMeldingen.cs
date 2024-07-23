using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE
{
    class VBHMeldingen
    {
        List<LABELS> mDHZSoort;
        List<LABELS> mDrachtStatus;

        public void VulArrays(int pProgId, DBConnectionToken mToken)
        {
            mDHZSoort = Facade.GetInstance().getSaveToDB(mToken).GetLabels(145, Convert.ToInt32(utils.getLabelsLabcountrycode()));
            mDrachtStatus = Facade.GetInstance().getSaveToDB(mToken).GetLabels(145, Convert.ToInt32(utils.getLabelsLabcountrycode()));
        }

        public List<IRreportresult> ZetDHZMeldingenKlaar(int pUBNId, DBConnectionToken mToken)
        {
            List<IRreportresult> lDHZList = new List<IRreportresult>();
            return lDHZList;
        }


        public String getDHZSoort(int InsInfo)
        {
            var InsInfoLabel = from CurLabel in mDHZSoort
                               where CurLabel.LabId == InsInfo
                               select CurLabel;
            if (InsInfoLabel.Count() == 0) return String.Empty;
            return InsInfoLabel.First().LabLabel;
        }

        private static COUNTRY GetCountry(DBConnectionToken mToken, int pUBNid)
        {
            UBN UBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            THIRD thrid = Facade.GetInstance().getSaveToDB(mToken).GetThirdByThirId(UBN.ThrID);
            COUNTRY country = Facade.GetInstance().getSaveToDB(mToken).GetCountryByLandid(Convert.ToInt32(thrid.ThrCountry));
            return country;
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
            return lDHZLog;
        }

        public bool DHZNietHerhalen(string FoutCode, DHZ pRecord)
        {
            switch (FoutCode)
            {
                case "IRD-00015":
                case "MFIRD-00015":
                    pRecord.Report = 2;
                    return true;
                case "IRD-00185":
                case "MFIRD-00185":
                    pRecord.Report = 2;
                    return true;
                case "MFIRD-01508":
                case "IRD-01508":
                    pRecord.Report = 2;
                    return true;
                default:
                    return false;
            }
        }



        public SOAPLOG MeldDHZ(DHZ pRecord, int pUBNid, DBConnectionToken mToken, int changedby, int sourceid)
        {
            //Win32SOAPVBHALG DLLcall = new Win32SOAPVBHALG();
            VSM.RUMA.CRVSOAP.CrvVruchtbaarheid crvvruchtbaarheid = new CRVSOAP.CrvVruchtbaarheid(changedby, sourceid);
           
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBNid).LandAfk3;
            String Uitvoerende = string.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            UBN ubn = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {

                FTPUSER fusoapold = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }

            lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = pRecord.AniLifenumber;
            Result.FarmNumber = pRecord.FarmNumber;
            String LogFile = unLogger.getLogDir("IenR") + "CRDDHZSOAP" + pRecord.FarmNumber + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDDHZSOAP" + pRecord.FarmNumber + "-" + DateTime.Now.Ticks + ".log");
            int InsKind, DHZKind;
            if (int.TryParse(pRecord.InsInfo, out DHZKind)) InsKind = DHZKind;
            else InsKind = 3;
            switch (InsKind)
            {

                case 2:

                    //DLLcall.CRDNatuurlijkeDekkingMelding(lUsername, lPassword, lTestServer,
                    //    lLand, pRecord.FarmNumber, pRecord.AniLifenumber, pRecord.InsDate, pRecord.InsDate,
                    //    pRecord.BullLifeNumber, Uitvoerende,
                    //    LogFile,
                    //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);
                    ANIMAL an = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.AniLifenumber);
                    ANIMAL bull = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.BullLifeNumber);
                    SOAPLOG Rest = crvvruchtbaarheid.SendNatuurlijkeDekking(ubn, an, fusoap, lLand, Uitvoerende, bull, pRecord.InsDate);
                    lStatus = Rest.Status.ToUpper();
                    lCode = Rest.Code;
                    lOmschrijving = Rest.Omschrijving;
                    break;
                case 3:
                case 4:
                case 5:
                    //DLLcall.CRDDHZKImelding(lUsername, lPassword, lTestServer,
                    //    lLand, pRecord.FarmNumber, pRecord.AniLifenumber, pRecord.InsDate, pRecord.InsDate,
                    //    pRecord.BullAInumber, pRecord.BullLifeNumber, pRecord.ChargeNumber, Uitvoerende,
                    //    LogFile,
                    //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);
                    ANIMAL an5 = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.AniLifenumber);
                    ANIMAL bull5 = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.BullLifeNumber);
                    SOAPLOG Rest5 = crvvruchtbaarheid.SendInsemin(ubn, an5, fusoap, lLand, Uitvoerende, bull5, pRecord.ChargeNumber, pRecord.InsDate);
                    lStatus = Rest5.Status.ToUpper();
                    lCode = Rest5.Code;
                    lOmschrijving = Rest5.Omschrijving;

                    break;
                case 9:
                    //DLLcall.CRDSamenWeidingMelding(lUsername, lPassword, lTestServer,
                    //    lLand, pRecord.FarmNumber, pRecord.AniLifenumber, pRecord.InsDate,
                    //    pRecord.InsDate, pRecord.EndDate,
                    //    pRecord.BullLifeNumber, Uitvoerende,
                    //    LogFile,
                    //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);
                    ANIMAL an9 = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.AniLifenumber);
                    ANIMAL bull9 = Facade.GetInstance().getSaveToDB(mToken).GetAnimalByAniAlternateNumber(pRecord.BullLifeNumber);
                    SOAPLOG Rest9 = crvvruchtbaarheid.SendGroupMating(ubn, an9, fusoap, lLand, Uitvoerende, bull9, pRecord.InsDate, pRecord.EndDate);
                    lStatus = Rest9.Status.ToUpper();
                    lCode = Rest9.Code;
                    lOmschrijving = Rest9.Omschrijving;
                    break;

            }
            Result.Kind = 500 + InsKind;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W") || DHZNietHerhalen(lCode, pRecord))
            {
                pRecord.ReportDate = Result.Date;
                pRecord.ReportTime = Result.Time;
                Facade.GetInstance().getMeldingen().InsertDHZLog(ConverttoDHZLog(pRecord), mToken);
                Facade.GetInstance().getMeldingen().DeleteDHZ(pRecord, mToken);
            }
            return Result;
        }

        public SOAPLOG MeldTocht(INHEAT pRecord, int pUBNid, DBConnectionToken mToken, int changedby, int sourceid)
        {
            //Win32SOAPVBHALG DLLcall = new Win32SOAPVBHALG();
            VSM.RUMA.CRVSOAP.CrvVruchtbaarheid crvvruchtbaarheid = new CRVSOAP.CrvVruchtbaarheid(changedby, sourceid);
         
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBNid).LandAfk3;
            String Uitvoerende = string.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            EVENT lEvent = Facade.GetInstance().getSaveToDB(mToken).GetEventdByEventId(pRecord.EventId);
            ANIMAL lAnimal = Facade.GetInstance().getSaveToDB(mToken).GetAnimalById(lEvent.AniId);
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);
            

            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {

                FTPUSER fusoapold = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }

            lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = lAnimal.AniAlternateNumber;
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            String LogFile = unLogger.getLogDir("IenR") + "CRDVBHSOAP" + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDVBHSOAP" + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log");


            //DLLcall.CRDTochtMelding(lUsername, lPassword, lTestServer,
            //    lLand, lUBN.Bedrijfsnummer, lAnimal.AniAlternateNumber, lEvent.EveDate, lEvent.EveDate,
            //    Uitvoerende,
            //    LogFile,
            //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);
            SOAPLOG Rest = crvvruchtbaarheid.SendInHeat(lUBN, lAnimal, fusoap, lLand, Uitvoerende, lEvent.EveDate);
            lStatus = Rest.Status.ToUpper();
            lCode = Rest.Code;
            lOmschrijving = Rest.Omschrijving;

            Result.Kind = 601;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {
                //lEvent.ReportDate = Result.Date;                
            }
            return Result;
        }


        public SOAPLOG MeldDracht(GESTATIO pRecord, int pUBNid, DBConnectionToken mToken, int changedby, int sourceid)
        {
            //Win32SOAPVBHALG DLLcall = new Win32SOAPVBHALG();
            VSM.RUMA.CRVSOAP.CrvVruchtbaarheid crvvruchtbaarheid = new CRVSOAP.CrvVruchtbaarheid(changedby, sourceid);
       
            String lUsername, lPassword;
            Boolean lTestServer;
            String lLand = GetCountry(mToken, pUBNid).LandAfk3;
            String Uitvoerende = string.Empty;
            String lStatus = string.Empty;
            String lCode = string.Empty;
            String lOmschrijving = string.Empty;
            SOAPLOG Result = new SOAPLOG();
            EVENT lEvent = Facade.GetInstance().getSaveToDB(mToken).GetEventdByEventId(pRecord.EventId);
            ANIMAL lAnimal = Facade.GetInstance().getSaveToDB(mToken).GetAnimalById(lEvent.AniId);
            UBN lUBN = Facade.GetInstance().getSaveToDB(mToken).GetubnById(pUBNid);


            FTPUSER fusoap = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {

                FTPUSER fusoapold = Facade.GetInstance().getSaveToDB(mToken).GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }

            lTestServer = configHelper.UseCRDTestserver;
            Result.Date = DateTime.Today;
            Result.Time = DateTime.Now;
            Result.Lifenumber = lAnimal.AniAlternateNumber;
            Result.FarmNumber = lUBN.Bedrijfsnummer;
            String LogFile = unLogger.getLogDir("IenR") + "CRDVBHSOAP" + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log";
            unLogger.WriteInfo(unLogger.getLogDir("IenR") + "CRDVBHSOAP" + lUBN.Bedrijfsnummer + "-" + DateTime.Now.Ticks + ".log");
            int StatusDracht;
            switch (pRecord.GesStatus)
            {
                case 2: 
                     StatusDracht = 25;
                    break;
                case 3 :  
                     StatusDracht = 26;
                    break;
                case 4 :
                     StatusDracht = 29;
                    break;
                default:
                    StatusDracht = 00;
                    break;
            }


            int MethodeControle = 0;

            //DLLcall.CRDDrachtControleMelding(lUsername, lPassword, lTestServer,
            //    lLand, lUBN.Bedrijfsnummer, lAnimal.AniAlternateNumber, lEvent.EveDate, lEvent.EveDate,
            //    StatusDracht,
            //    MethodeControle,                
            //    Uitvoerende,
            //    LogFile,
            //    ref lStatus, ref lCode, ref lOmschrijving, MaxString);
            SOAPLOG rest = crvvruchtbaarheid.SendPregCheck(lUBN, lAnimal, fusoap, lLand, Uitvoerende, lEvent.EveDate, StatusDracht, MethodeControle);
            lStatus = rest.Status;
            lCode = rest.Code;
            lOmschrijving = rest.Omschrijving;

            Result.Kind = 601;
            Result.Status = lStatus;
            Result.Code = lCode;
            Result.Omschrijving = lOmschrijving;
            if (lStatus.Equals("G") || lStatus.Equals("W"))
            {
                //lEvent.ReportDate = Result.Date;                
            }
            return Result;
        }
    }
}
