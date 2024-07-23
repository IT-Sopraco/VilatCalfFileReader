using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Configuration;

namespace VSM.RUMA.CORE.EDINRS
{
    public class EDINRSSOAP
    {
        protected AFSavetoDB mSavetoDB;
        public EDINRSSOAP(AFSavetoDB pSavetoDB)
        {
            mSavetoDB = pSavetoDB;
        }

        public void LeesBedrijf(int pUBNid)
        {
            String lUsername, lPassword;
            String Status, Code, Omschrijving;
            FTPUSER fusoap = mSavetoDB.GetFtpuser(pUBNid, 9991);
            lUsername = fusoap.UserName;
            lPassword = fusoap.Password;
            if (lUsername == String.Empty && lPassword == String.Empty)
            {
                FTPUSER fusoapold = mSavetoDB.GetFtpuser(pUBNid, 12);
                lUsername = fusoapold.UserName;
                lPassword = fusoapold.Password;
            }
            UBN lUBN = mSavetoDB.GetubnById(pUBNid);
            THIRD lThird = mSavetoDB.GetThirdByThirId(lUBN.ThrID.ToString());
            bool lTestServer = true;
            GetAnimalInformation(lUsername, lPassword, lTestServer, lThird.ThrCountry, lUBN.Bedrijfsnummer,
                out  Status, out Code, out Omschrijving);
        }


        public void GetAnimalInformation(String pUsername,
            String pPassword, Boolean pTestServer, String LandCode, String UBNnr,
            out String Status, out String Code, out String Omschrijving)
        {
            rsReproduction.ESB_rsReproduction_Service service = new VSM.RUMA.CORE.EDINRS.rsReproduction.ESB_rsReproduction_Service();
            rsReproduction.listAnimalReproductionRequest request = new VSM.RUMA.CORE.EDINRS.rsReproduction.listAnimalReproductionRequest();
            rsReproduction.participantId keeper;
            request.contextMessage = getContext(pUsername, pPassword, LandCode, UBNnr, out keeper);
            rsReproduction.listAnimalReproductionRequestType reqMessage = new VSM.RUMA.CORE.EDINRS.rsReproduction.listAnimalReproductionRequestType();
            List<rsReproduction.participantAnimalRequest> Req = new List<rsReproduction.participantAnimalRequest>();
            //VSM.RUMA.CORE.EDINRS.rsReproduction.participantAnimalRequest req = new VSM.RUMA.CORE.EDINRS.rsReproduction.participantAnimalRequest();
            //req.numberSubType = String.Empty;
            //req.numberType = String.Empty;
            //Req.Add(req);            
            //reqMessage.participantAnimalRequest = Req.ToArray();
            reqMessage.keeper = keeper;
            request.requestMessage = reqMessage;
            if (pTestServer) service.Url = Properties.Settings.Default.VSM_RUMA_CORE_EDINRS_rsReproduction_ESB_rsReproduction_TestService;
            rsReproduction.listAnimalReproductionResponse response = service.listAnimalReproduction(request);
            rsReproduction.serviceMessage Result = response.serviceMessage;
            Code = Result.messageCode;
            Omschrijving = Result.messageText;
            Status = Result.messageType;

            if (Result.messageType != "E")
            {
                List<rsReproduction.animalReproduction> Animals = response.responseMessage.animalReproduction.ToList();
                ANIMAL ani;
                ANIMALCATEGORY AniCat;
                foreach (rsReproduction.animalReproduction Animal in Animals)
                {
                    ani = new ANIMAL();
                    AniCat = new ANIMALCATEGORY();
                    ani.AniName = Animal.animalName;
                    ani.AniLifeNumber = Animal.animal.animalNumber;
                    AniCat.AniWorknumber = Animal.farmersAnimalNr;
                    //ani.AniSex = Animal.sex;
                    //ani.AniBirthDate = Animal.birthDate;
                }
            }
        }




        protected rsReproduction.contextMessage getContext(String pUsername,
            String pPassword, String LandCode, String UBNnr, out rsReproduction.participantId keeper)
        {
            rsReproduction.contextMessage Context = new rsReproduction.contextMessage();
            rsReproduction.participantId BedrijfId = new rsReproduction.participantId();
            //BedrijfId.countryCode = String.Empty;
            BedrijfId.participantCode = UBNnr;
            if (LandCode == "BE") BedrijfId.participantCodeType = "UVN";
            else BedrijfId.participantCodeType = "UBN";
            rsReproduction.participantId PARId = new rsReproduction.participantId();
            //PARId.countryCode = String.Empty;
            PARId.participantCode = UBNnr;
            PARId.participantCodeType = "PAR";
            keeper = BedrijfId;
            Context.keeper = BedrijfId;
            //Context.instanceId = String.Empty;
            //Context.sessionId = String.Empty;
            //Context.processId = String.Empty;
            Context.username = pUsername;
            Context.password = pPassword;
            Context.userType = "C";
            Context.customer = PARId;
            ////Context.languageCode = "NLD";
            ////Context.timeZone = "NL";
            //Context.languageCode = String.Empty;
            //Context.timeZone = String.Empty;
            List<rsReproduction.contextMessageDetail> ContextDetail = new List<rsReproduction.contextMessageDetail>();
            rsReproduction.contextMessageDetail CD1 = new rsReproduction.contextMessageDetail();
            CD1.contextCode = "organisation";
            if (LandCode == "BE")
            {
                CD1.contextValue = "crv.be";
            }
            else
            {
                CD1.contextValue = "crv.nl";
            }
            ContextDetail.Add(CD1);
            Context.contextMessageDetail = ContextDetail.ToArray();
            return Context;
        }

    }
}
