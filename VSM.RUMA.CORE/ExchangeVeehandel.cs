using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Xml.Linq;

namespace VSM.RUMA.CORE
{

    [Serializable()]
    public class ExchangeVeehandelValues
    {
        public DateTime Afleverdatum;
        public DateTime Laaddatum;
        public DateTime Losdatum;
        public sDestination Bestemming;
        public sDestination Handelaar;
        public sDestination Herkomst;
        public bool isVKI;
        public int Transporteur;
        public int voertuig;
        public int aanhangwagen;
    }

    public delegate void ExchangeVeehandelProgress(double progress, string message);


    public class ExchangeVeehandel
    {
        /*  BUG 611
         *  ExchangeVeehandel
         *  
            Versie				: 0.5
            Berichtversie		: 0.3
         *  creeeren van een XML bestand vlgns bijlage bij BUG 611 dat verstuurd moet worden
            controle =  ExchangeVeehandel.xsd
         */


        public event ExchangeVeehandelProgress Update;

        public void UpdateProgress(double progress, string message)
        {
            unLogger.WriteInfo("progress:" + progress.ToString() + " message:" + message);
            if (Update != null) { Update(progress, message); }
        }

        public  XDocument getBasicExchangeVeehandel(UserRightsToken pUr, UBN pUBNHouder)
        {
            THIRD lThrd = Facade.GetInstance().getSaveToDB(pUr).GetThirdByThirId(pUBNHouder.ThrID);
            string HouderNaam = "";
            if (lThrd.ThrCompanyName != "")
            { HouderNaam = lThrd.ThrCompanyName; }
            else { HouderNaam = lThrd.ThrSecondName; }
            XDocument xd = new XDocument();
            XElement xml = new XElement("ExchangeVeehandel",
                    new XElement("BerichtVersie", "0.3"),
                    new XElement("HouderUBN", pUBNHouder.Bedrijfsnummer),
                    new XElement("HouderNaam", HouderNaam),
                    new XElement("Ritten")
                );
            xd.Add(xml);
            return xd;

        }

        private double aantal;
        private double teller;
        public  void addRit(ref XDocument pExchangeVeehandelXdocument, UserRightsToken pUr, UBN pUBNHouder, TRANSPRT pTransport,TRANSPRT pTransportAanhanger, DateTime pLaadTijd, DateTime pLosTijd, DateTime pAfleverTijd,List<ANIMAL> pDieren,bool pVKI_JN,UBN pHerkomstUBN,UBN pBestemmingsUBN,THIRD pHandelaar,string LNVDiersoort)
        {
            
            THIRD lTransporter = new THIRD();
            if (pTransport.Transporter > 0)
            {
                lTransporter = Facade.GetInstance().getSaveToDB(pUr).GetThirdByThirId(pTransport.Transporter);
            }
            XElement Rit = new XElement("Rit",
                new XElement("TransporteurErkenningsnummer", lTransporter.ThrKvkNummer),
                new XElement("TransporteurNaam", lTransporter.ThrCompanyName),
                new XElement("AutoKenteken",pTransport.LicensePlate),
                new XElement("AutoAanhangerKenteken", pTransportAanhanger.LicensePlate),
                new XElement("Afleverdatum",utils.getDatumExchange(pAfleverTijd)),
                new XElement("Aflevertijd", utils.getTimeExchange(pAfleverTijd)),
                new XElement("Dieren")
                );
            aantal = Convert.ToDouble(pDieren.Count);
            
            if (pDieren.Count > 0)
            {
                
                
                XElement Dieren =  Rit.Element("Dieren");
                teller = 1;
               
                foreach (ANIMAL lAni in pDieren)
                {
                   
                    addDier(ref Dieren, pUr, pUBNHouder, pLaadTijd, pLosTijd, lAni, pVKI_JN, pHerkomstUBN, pBestemmingsUBN, pHandelaar, LNVDiersoort);
                    
                    try
                    {
                        double process = (teller / aantal) * 100;
                        //unLogger.WriteInfo(teller.ToString() + "/" + aantal.ToString() + " * 100 = " + process.ToString());
                        UpdateProgress(process, lAni.AniLifeNumber);
                    }
                    catch (Exception exc) { unLogger.WriteInfo(exc.ToString()); }
                    teller = teller + 1;
                }
                pExchangeVeehandelXdocument.Root.Element("Ritten").Add(Rit);
            }

        }

        private  void Geneesmiddel(UserRightsToken pUr, int pMedicijnID, out string pNaam, out string pRegNLmiddel, out int pWaitDays)
        {
          
            if (Event_functions.UseOldMedicine)
            {
                MEDICINE med = Facade.GetInstance().getSaveToDB(pUr).GetMedicineByMedId(pMedicijnID);
                pNaam = med.MedName;
                pRegNLmiddel = med.MedCode;
                pWaitDays = 0;
                if (med.MedDaysWaitingMeat > pWaitDays)
                {
                    pWaitDays = med.MedDaysWaitingMeat;
                }
                if (med.MedDaysWaitingMilk > pWaitDays)
                {
                    pWaitDays = med.MedDaysWaitingMilk;
                }
                if (med.MedHoursWaitingMeat > (pWaitDays * 24))
                {
                    pWaitDays = med.MedHoursWaitingMeat / 24;
                }
                if (med.MedHoursWaitingMilk > (pWaitDays * 24))
                {
                    pWaitDays = med.MedHoursWaitingMilk / 24;
                }
            }
            else
            {
                ARTIKEL lArt = new ARTIKEL();
                ARTIKEL_MEDIC lArtmedic = new ARTIKEL_MEDIC();
                Facade.GetInstance().getSaveToDB(pUr).getMedicijnArtikel(pMedicijnID, out lArt, out lArtmedic);
                pNaam = lArt.ArtNaam;
                pRegNLmiddel = lArtmedic.ArtMed_RegNumber;
                pWaitDays = 0;
                if (lArtmedic.ArtMed_DaysWaitingMeat > pWaitDays)
                {
                    pWaitDays = lArtmedic.ArtMed_DaysWaitingMeat;
                }
                if (lArtmedic.ArtMed_DaysWaitingMilk > pWaitDays)
                {
                    pWaitDays = lArtmedic.ArtMed_DaysWaitingMilk;
                }
                if (lArtmedic.ArtMed_HoursWaitingMeat > (pWaitDays * 24))
                {
                    pWaitDays = lArtmedic.ArtMed_HoursWaitingMeat / 24;
                }
                if (lArtmedic.ArtMed_HoursWaitingMilk > (pWaitDays * 24))
                {
                    pWaitDays = lArtmedic.ArtMed_HoursWaitingMilk / 24;
                }
            }
        }
 

        private  void addDier(ref XElement pXdieren, UserRightsToken pUr, UBN pUBNHouder, DateTime pLaadTijd, DateTime pLosTijd, ANIMAL pDier, bool pVKI_JN, UBN pHerkomstUBN, UBN pBestemmingsUBN, THIRD pHandelaar,string LNVDiersoort)
        {

            string VKI_JN = "Nee";
            if (pVKI_JN)
            { VKI_JN = "Ja"; }
            string llandcode="";
            string llevensnr="";
     
            ANIMAL lMother = new ANIMAL();
            string lMothland = "";
            string lMothlife = "";
            if (pDier.AniIdMother > 0)
            {
                lMother = Facade.GetInstance().getSaveToDB(pUr).GetAnimalById(pDier.AniIdMother);
            }
            ANIMAL lFather = new ANIMAL();
            string lFathland = "";
            string lFathlife = "";
            if (pDier.AniIdFather > 0)
            {
                lFather = Facade.GetInstance().getSaveToDB(pUr).GetAnimalById(pDier.AniIdMother);
            }
            string haarkleur = "";
            if (LNVDiersoort == "1")
            {
                List<LABELS> haarkleuren = Facade.GetInstance().getSaveToDB(pUr).GetLabels(16, 528);//16 = voor de afkortingen 9 voor de woorden
                var haarkleurarr = from kleur in haarkleuren
                                   where pDier.Anihaircolor == kleur.LabId
                                   select kleur;
                if (haarkleurarr.Count() == 1) haarkleur = haarkleurarr.First().LabLabel;
            }
            switch(LNVDiersoort)
            {
                case "1"://Rund
                    llandcode = pDier.AniLifeNumber.Substring(0,2).ToUpper();
                    llevensnr = pDier.AniLifeNumber.Remove(0,3);
                    if (lMother.AniLifeNumber.Length > 5)
                    {
                        lMothland = lMother.AniLifeNumber.Substring(0, 2).ToUpper();
                        lMothlife = lMother.AniLifeNumber.Remove(0, 3);
                    }
                    if (lFather.AniLifeNumber.Length > 5)
                    {
                        lFathland = lFather.AniLifeNumber.Substring(0, 2).ToUpper();
                        lFathlife = lFather.AniLifeNumber.Remove(0, 3);
                    }
                    break;
                case "2"://Varken
                    llandcode = pDier.AniLifeNumber.Substring(0,2).ToUpper();
                    llevensnr = pDier.AniLifeNumber.Remove(0,3);
                    if (lMother.AniLifeNumber.Length > 5)
                    {
                        lMothland = lMother.AniLifeNumber.Substring(0, 2).ToUpper();
                        lMothlife = lMother.AniLifeNumber.Remove(0, 3);
                    }
                    if (lFather.AniLifeNumber.Length > 5)
                    {
                        lFathland = lFather.AniLifeNumber.Substring(0, 2).ToUpper();
                        lFathlife = lFather.AniLifeNumber.Remove(0, 3);
                    }
                    break;
                case "3"://Schaap
                    llandcode = pDier.AniAlternateNumber.Substring(0,2).ToUpper();
                    llevensnr = pDier.AniAlternateNumber.Remove(0,3);
                    if (lMother.AniAlternateNumber.Length > 5)
                    {
                        lMothland = lMother.AniAlternateNumber.Substring(0, 2).ToUpper();
                        lMothlife = lMother.AniAlternateNumber.Remove(0, 3);
                    }
                    if (lFather.AniAlternateNumber.Length > 5)
                    {
                        lFathland = lFather.AniAlternateNumber.Substring(0, 2).ToUpper();
                        lFathlife = lFather.AniAlternateNumber.Remove(0, 3);
                    }
                    break;
                case "4"://Geit
                    llandcode = pDier.AniAlternateNumber.Substring(0,2).ToUpper();
                    llevensnr = pDier.AniAlternateNumber.Remove(0,3);
                    if (lMother.AniAlternateNumber.Length > 5)
                    {
                        lMothland = lMother.AniAlternateNumber.Substring(0, 2).ToUpper();
                        lMothlife = lMother.AniAlternateNumber.Remove(0, 3);
                    }
                    if (lFather.AniAlternateNumber.Length > 5)
                    {
                        lFathland = lFather.AniAlternateNumber.Substring(0, 2).ToUpper();
                        lFathlife = lFather.AniAlternateNumber.Remove(0, 3);
                    }
                    break;
            }
            string sex = "M";
            if (pDier.AniSex == 2)
            {
                sex = "V";
            }
            else if (pDier.AniSex > 2)
            { sex = "O"; }

            THIRD ThrHerk = new THIRD();
            COUNTRY cHerk = new COUNTRY();
            if (pHerkomstUBN.ThrID > 0)
            {
                ThrHerk = Facade.GetInstance().getSaveToDB(pUr).GetThirdByThirId(pHerkomstUBN.ThrID);
                cHerk = Facade.GetInstance().getSaveToDB(pUr).GetCountryByLandid(int.Parse(ThrHerk.ThrCountry));
            }
            THIRD ThrBest = new THIRD();
            COUNTRY cBest = new COUNTRY();
            if (pBestemmingsUBN.ThrID > 0)
            {
                ThrBest = Facade.GetInstance().getSaveToDB(pUr).GetThirdByThirId(pBestemmingsUBN.ThrID);
                cBest = Facade.GetInstance().getSaveToDB(pUr).GetCountryByLandid(int.Parse(ThrBest.ThrCountry));
            }
            XElement Dier = new XElement("Dier",
                new XElement("Diersoort", LNVDiersoort),
                new XElement("Landcode", llandcode),
                new XElement("Levensnummer",llevensnr),
                new XElement("Geboortedatum", pDier.AniBirthDate.ToString("dd-MM-yyyy")),
                new XElement("Geslacht", sex),
                new XElement("Haarkleur", haarkleur),
                new XElement("Ras"),
                new XElement("MoederLandcode",lMothland),
                new XElement("MoederLevensnummer",lMothlife),
                new XElement("VaderLandcode",lFathland),
                new XElement("VaderLevensnummer",lFathlife),
                new XElement("HerkomstUBN",pHerkomstUBN.Bedrijfsnummer),
                new XElement("BestemmingUBN",pBestemmingsUBN.Bedrijfsnummer),
                new XElement("HerkomstLandcode",cHerk.LandAfk2),
                new XElement("BestemmingLandcode",cBest.LandAfk2),
                new XElement("Laaddatum",utils.getDatumExchange(pLaadTijd)),
                new XElement("Laadtijd", utils.getTimeExchange(pLaadTijd)),
                new XElement("Losdatum", utils.getDatumExchange(pLosTijd)),
                new XElement("Lostijd", utils.getTimeExchange(pLosTijd)),
                new XElement("HandelaarErkenningsnummer",pHandelaar.ThrKvkNummer),
                new XElement("HanderlaarNaam",pHandelaar.ThrCompanyName),
                new XElement("VKI_JN", VKI_JN)
                );
            
            //Geneesmiddelen
            List<EVENT> lBehandelingen = Facade.GetInstance().getSaveToDB(pUr).getEventsByAniIdKindUbn(pDier.AniId, Convert.ToInt32(DB.LABELSConst.EventKind.BEHANDELING), pUBNHouder.UBNid);
            if (lBehandelingen.Count > 0)
            {
                XElement gen = new XElement("Geneesmiddelen");
                foreach (EVENT lEv in lBehandelingen)
                {
                    TREATMEN tr = Facade.GetInstance().getSaveToDB(pUr).GetTreatmen(lEv.EventId);
                    string pNaam = "";
                    string pRegNLmiddel = "";
                    int lWaitdays = 0;
                    Geneesmiddel(pUr, tr.MedId,out pNaam,out pRegNLmiddel,out lWaitdays);
                    
                    XElement xGeneesmiddel = new XElement("Geneesmiddel",
                        new XElement("DatumEersteBehandeling",utils.getDatumExchange(lEv.EveDate)),
                        new XElement("DatumLaatsteBehandeling", utils.getDatumExchange(lEv.EveDate.AddDays(tr.TreMedDaysTreat))),
                        new XElement("AantalBehandelingen", tr.TreMedDaysTreat.ToString()),
                        new XElement("DiagnoseBehandeling"),
                        new XElement("GeneesmiddelNaam",pNaam),
                        new XElement("RegNLmiddel",pRegNLmiddel),
                        new XElement("DatumEindeWachttermijn", utils.getDatumExchange(lEv.EveDate.AddDays(tr.TreMedDaysTreat).AddDays(lWaitdays)))
                        );

                    gen.Add(xGeneesmiddel);

                }
                Dier.Add(gen);
            }
           
            pXdieren.Add(Dier);
        }

    }
}
