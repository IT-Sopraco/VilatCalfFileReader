using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Threading;

namespace VSM.RUMA.CORE.COMMONS
{
    public class MailMeldingQueue
    {
        static readonly object padlock = new object();
        int processed;
        int warnings;
        int errors;
        int count;

        private Dictionary<int, StringBuilder> Meldingen = new Dictionary<int, StringBuilder>();

        [Obsolete("Stringbuilder wordt niet meer gebruikt", true)]
        public MailMeldingQueue(ref StringBuilder pMeldingen, ref int pprocessed, ref int pwarnings, ref int perrors)
        {
            processed = pprocessed;
            warnings = pwarnings;
            errors = perrors;
            count = 0;

        }

        public MailMeldingQueue(ref int pprocessed, ref int pwarnings, ref int perrors)
        {
            processed = pprocessed;
            warnings = pwarnings;
            errors = perrors;
            count = 0;

        }

        public void AddMelding(int Severity, int ProgramId, String Melding)
        {
            unLogger.WriteInfo($"unAnimalActions.AddMelding: Severity: '{Severity}' ProgramId: '{ProgramId}' Melding: '{Melding}'");
            Interlocked.Increment(ref processed);
            switch (Severity)
            {
                case 0:
                    unLogger.WriteInfo(Melding);
                    return;
                case 1:
                    Interlocked.Increment(ref warnings);
                    break;
                case 2:
                    Interlocked.Increment(ref errors);
                    break;
            }
           
            Interlocked.Increment(ref count);
            lock (padlock)
            {
                StringBuilder messages;
                if (!Meldingen.TryGetValue(ProgramId, out messages))
                {
                    messages = new StringBuilder();
                    Meldingen.Add(ProgramId, messages);
                }
                messages.AppendLine($@"<tr><td>{count}</td><td>{DateTime.Now.ToShortTimeString()}</td>{Melding}</tr>");
            }
        }


        public void getData(ref StringBuilder pMeldingen, ref int pprocessed, ref int pwarnings, ref int perrors)
        {
            unLogger.WriteInfo($"unAnimalActions.getData: '{pMeldingen.ToString()}'");


            var meldingenvalues = from sb in Meldingen
                                  orderby sb.Key
                                  select sb.Value;

            foreach (var meldingenvalue in meldingenvalues)
            {
                pMeldingen.Append(meldingenvalue);
            }

            pprocessed = processed;
            pwarnings = warnings;
            perrors = errors;
        }
    }

    public class unAnimalActions
    {
        protected AFSavetoDB mSavetoDB;
        public unAnimalActions(AFSavetoDB pSavetoDB)
        {
            mSavetoDB = pSavetoDB;
        }

        public ANIMAL GetAnimalFromAnimals(int pThrId, BEDRIJF pFarm, string pLevensnr, int pAniCat, int pAniSex)
        {
            string AniWorkNumber;
            ANIMAL lAniItem = GetAnimalFromAnimals(pFarm, pLevensnr, out AniWorkNumber);
            if (!mSavetoDB.isFilledByDb(lAniItem) && AniWorkNumber != String.Empty)
            {
                //lAniItem.AniCategory = Convert.ToSByte(pAniCat);
                lAniItem.AniSex = pAniSex;
                mSavetoDB.SaveAnimal(pThrId, lAniItem);
                ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lAniItem.AniId, pFarm.FarmId);
                if (lAniCategory.AniWorknumber == String.Empty) lAniCategory.AniWorknumber = AniWorkNumber;
                lAniCategory.Anicategory = pAniCat;
                lAniCategory.AniId = lAniItem.AniId;
                lAniCategory.FarmId = pFarm.FarmId;
                mSavetoDB.SaveAnimalCategory(lAniCategory);

            }
            return lAniItem;
        }

        public ANIMAL GetAnimalFromAnimals(BEDRIJF pFarm, string pLevensnr, out string AniWorkNumber)
        {
            AniWorkNumber = String.Empty;
            if (pLevensnr == null)
            {
                return null;
            }

            ANIMAL lAniItem = mSavetoDB.GetAnimalByLifenr(pLevensnr);
            if (lAniItem.AniId < 1)
            {
                lAniItem = mSavetoDB.GetAnimalByAniAlternateNumber(pLevensnr);
            }
            if (!mSavetoDB.isFilledByDb(lAniItem))
            {
                if (pFarm.ProgId == 3 || pFarm.ProgId == 5)
                {
                    lAniItem.AniAlternateNumber = pLevensnr;
                    string lefnr = "";
                    string rfid = mSavetoDB.GetFarmConfigValue(pFarm.FarmId,"rfid");
                    getRFIDNumbers(pFarm, rfid, pLevensnr, 0, out AniWorkNumber, out lefnr);
                    lAniItem.AniLifeNumber = lefnr;
                  
                }
                else
                {
                    lAniItem.AniLifeNumber = pLevensnr;
                    lAniItem.AniAlternateNumber = pLevensnr;
                    string lLevensnr = String.Empty;
                    string lCountryCode = String.Empty;
                    if (pLevensnr.IndexOf(" ") > 0)
                    {
                        lLevensnr = pLevensnr.Substring(pLevensnr.IndexOf(" ") + 1);
                        lCountryCode = pLevensnr.Substring(0, pLevensnr.IndexOf(" "));
                    }
                    else lLevensnr = pLevensnr;
                    if (DelphiWrapper.controlelevensnr(pFarm.ProgId, lCountryCode, lLevensnr, false)
                        && (lAniItem.AniLifeNumber.IndexOf("-") < 0 || pFarm.ProgId != 1))
                    {
                        AniWorkNumber = DelphiWrapper.fDiernr(pFarm.ProgId, lCountryCode, lLevensnr, false);
                    }
                    else
                    {
                        unLogger.WriteWarnFormat("Ongeldig Levensnummer : {0}  UBNId : {1}", lLevensnr, pFarm.UBNid);
                    }
                }
            }
            else
            {
                
                ANIMALCATEGORY lAniCategory = mSavetoDB.GetAnimalCategoryByIdandFarmid(lAniItem.AniId, pFarm.FarmId);
                if (mSavetoDB.isFilledByDb(lAniCategory))
                {
                    AniWorkNumber = lAniCategory.AniWorknumber;
                }
                else
                {
                    if (pFarm.ProgId == 3 || pFarm.ProgId == 5)
                    {
                        lAniItem.AniAlternateNumber = pLevensnr;
                        string lefnr = "";
                        string rfid = mSavetoDB.GetFarmConfigValue(pFarm.FarmId, "rfid");
                        getRFIDNumbers(pFarm, rfid, pLevensnr, 0, out AniWorkNumber,out lefnr);
                    
                    }
                    else
                    {
                        string lLevensnr = String.Empty;
                        string lCountryCode = String.Empty;
                        if (pLevensnr.IndexOf(" ") > 0)
                        {
                            lLevensnr = pLevensnr.Substring(pLevensnr.IndexOf(" ") + 1);
                            lCountryCode = pLevensnr.Substring(0, pLevensnr.IndexOf(" "));
                        }
                        else lLevensnr = pLevensnr;
                        AniWorkNumber = DelphiWrapper.fDiernr(pFarm.ProgId, lCountryCode, lLevensnr, false);
                    }
                }
            }
            return lAniItem;
        }


        private void getRFIDNumbers(BEDRIJF pBedrijf, string rfid, string pLNVlifeNumber_AlternateNumber, int pBirthCurrentHeighestVolgNummer, out string pWorknr, out string pAniLifenumber)
        {
            //Creeert nieuw werknummer en levensnummer voor als een dier nog 
            //niet in de database staat
            //anders retourneert  hij de bestaande
            char[] split = { ' ' };

            ANIMAL aniCheck = mSavetoDB.GetAnimalByAniAlternateNumber(pLNVlifeNumber_AlternateNumber);
            if (aniCheck.AniId == 0)
            {
                aniCheck = mSavetoDB.GetAnimalByLifenr(pLNVlifeNumber_AlternateNumber);
            }
            UBN lUbn = mSavetoDB.GetubnById(pBedrijf.UBNid);
            string lFokkernr = getStamboeknr(pBedrijf);
            string NFSFatherLetter = "0"; //zie BUG 23
            pWorknr = "";
            pAniLifenumber = "";
            bool getWorknumber = true;
            if (aniCheck.AniId != 0)
            {
                pAniLifenumber = aniCheck.AniLifeNumber;
                ANIMALCATEGORY aniCatCheck = mSavetoDB.GetAnimalCategoryByIdandFarmid(aniCheck.AniId, pBedrijf.FarmId);
                if (aniCatCheck.FarmId != 0)
                {
                    pWorknr = aniCatCheck.AniWorknumber.Trim();
                    if (pWorknr != "")
                    {
                        getWorknumber = false;
                        if (pAniLifenumber == "")
                        {
                            pAniLifenumber = pLNVlifeNumber_AlternateNumber;
                        }

                    }
                }
                else
                {
                    if (pAniLifenumber.Length > 5)
                    {
                        int lStart1 = pAniLifenumber.Length - 5;
                        if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                        {
                            string[] spl = pLNVlifeNumber_AlternateNumber.Split(split);
                            try
                            {
                                pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], false);
                            }
                            catch { }
                        }
                        else
                        {
                            pWorknr = pAniLifenumber.Substring(lStart1, 5);
                        }
                        getWorknumber = false;
                    }
                }
            }
            if (getWorknumber)
            {
                //NSFO heeft altijd 2
                if (pBedrijf.Programid == 22 || (pBedrijf.Programid > 23 && pBedrijf.Programid < 33))
                { rfid = "2"; }
                if (pBedrijf.Programid == 51 || pBedrijf.Programid == 52)
                {
                    NFSFatherLetter = "O";//weet ik niet is een nieuw dier
                }
                if (pLNVlifeNumber_AlternateNumber.Length > 5)
                {
                    int lStart = pLNVlifeNumber_AlternateNumber.Length - 5;
                    pWorknr = pLNVlifeNumber_AlternateNumber.Substring(lStart, 5);


                    string[] numbers = pLNVlifeNumber_AlternateNumber.Split(split);
                    if (numbers.Length >= 2)
                    {
                        string lCountryCode = numbers[0];
                        string lNumberCode = numbers[1];

                        if (rfid == "2")
                        {
                            if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                            {
                                string[] spl = pLNVlifeNumber_AlternateNumber.Split(split);
                                try
                                {
                                    pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], false);
                                }
                                catch { }
                                if (pAniLifenumber == "")
                                {
                                    pAniLifenumber = pLNVlifeNumber_AlternateNumber;
                                }
                            }
                            else
                            {
                                string laatste5 = pLNVlifeNumber_AlternateNumber.Substring(lStart, 5);
                                pWorknr = laatste5;

                                if (pAniLifenumber == "")
                                {
                                    if (pBedrijf.Programid == 51 || pBedrijf.Programid == 52)
                                    {
                                        pAniLifenumber = lCountryCode + " " + lFokkernr + "-" + NFSFatherLetter + laatste5;
                                    }
                                    else
                                    {
                                        pAniLifenumber = lCountryCode + " " + lFokkernr + "-" + laatste5;
                                    }
                                }

                            }
                        }
                        else if (rfid == "3")
                        {

                            if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                            {
                                string[] spl = pLNVlifeNumber_AlternateNumber.Split(split);
                                try
                                {
                                    pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], false);
                                }
                                catch { }
                                if (pAniLifenumber == "")
                                {
                                    pAniLifenumber = pLNVlifeNumber_AlternateNumber;
                                }
                            }
                            else
                            {
                                int lHoogstevolgnr = pBirthCurrentHeighestVolgNummer;
                                if (lHoogstevolgnr == 0)
                                {
                                    lHoogstevolgnr = getHoogsteVolgnr( pBedrijf, lFokkernr,mSavetoDB.GetAnimalsByFarmId(pBedrijf.FarmId));
                                }
                                lHoogstevolgnr = lHoogstevolgnr + 1;
                                string hNumber = lHoogstevolgnr.ToString();
                                while (hNumber.Length < 5)
                                {
                                    hNumber = "0" + hNumber;
                                }
                                pWorknr = hNumber;
                                if (pAniLifenumber == "")
                                {
                                    if (pBedrijf.Programid == 51 || pBedrijf.Programid == 52)
                                    {
                                        pAniLifenumber = lCountryCode + " " + lFokkernr + "-" + NFSFatherLetter + hNumber;
                                    }
                                    else
                                    {
                                        pAniLifenumber = lCountryCode + " " + lFokkernr + "-" + hNumber;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (pAniLifenumber == "")
                            {
                                unLogger.WriteInfo(pLNVlifeNumber_AlternateNumber + " als AniLifeNumber toegewezen. bij Diergegevens ophalen");
                                pAniLifenumber = pLNVlifeNumber_AlternateNumber;
                            }
                        }
                    }
                }
            }
        }

        private string getStamboeknr(BEDRIJF pBedrijf)
        {
            UBN u = mSavetoDB.GetubnById(pBedrijf.UBNid);
            THIRD tr = mSavetoDB.GetThirdByThirId(u.ThrID);
            string ThrStamboeknr = pBedrijf.Fokkers_Nr.Trim();
            if (ThrStamboeknr == "")
            {
                ThrStamboeknr = tr.ThrStamboeknr.Trim();
            }
            while (ThrStamboeknr.Length < 5)
            {
                ThrStamboeknr = "0" + ThrStamboeknr;
            }
            if (ThrStamboeknr.Length > 5)
            {
                int begin = ThrStamboeknr.Length - 5;
                ThrStamboeknr = ThrStamboeknr.Substring(begin, 5);
            }
            return ThrStamboeknr;
        }

        private int getHoogsteVolgnr(BEDRIJF pBedrijf, string pStamboeknr, List<ANIMAL> pAnimals)
        {
            int hoogstevolgnr = 0;
            Regex r = new Regex(@"^\d+$");

            foreach (ANIMAL antje in pAnimals)
            {
                if (antje.AniLifeNumber.Contains(pStamboeknr + "-"))
                {
                    string nr = antje.AniLifeNumber.Remove(0, antje.AniLifeNumber.LastIndexOf("-") + 1);
                    if (nr != "")
                    {
                        while (nr.Length > 0)
                        {
                            if (nr[0] == '0')
                            {
                                nr = nr.Remove(0, 1);
                            }
                            else
                            {
                                Match m = r.Match(nr);
                                if (m.Success)
                                {
                                    break;
                                }
                                else
                                {
                                    nr = nr.Remove(0, 1);
                                }
                            }
                        }

                        int dummyInt;
                        if (Int32.TryParse(nr, out dummyInt))
                        {
                            if (int.Parse(nr) > hoogstevolgnr)
                            { hoogstevolgnr = int.Parse(nr); }
                        }
                    }
                }
            }
            return hoogstevolgnr;
        }


    }
}
