using System;
using System.Collections.Generic;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE;

namespace VSM.RUMA.CORE.EDINRS
{
    public class RUMAcomEDINRS : AFcomEDINRS
    {

        IFacade mFacade;

        public RUMAcomEDINRS(IFacade pFacade, DBConnectionToken mToken, int FileLogId)
            : base(pFacade.getSaveToDB(mToken),  FileLogId)
        {

            mFacade = pFacade;
        }

        public override DateTime LeesHeader(String pBestandsnaam)
        {
            String lBerichttype;
            String lVersienr;
            String lSpecificatie;
            DateTime lBestandsDatum;
            DateTime lBestandsTijd;
            int error;
            unLogger.WriteDebug("RUMAEDINRS : Headerinfo");
            Win32EDINRS lDllCall = new Win32EDINRS();
            error = lDllCall.edinrs_HeaderInformatie(pBestandsnaam,
                      out lBerichttype, out lVersienr, out lSpecificatie,
                      out lBestandsDatum, out lBestandsTijd);
            unLogger.WriteDebug("RUMAEDINRS : Headerinfo result :" + error.ToString());
            lDllCall.Dispose();
            if (error != -1)
            {
                return lBestandsDatum;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public string LeesUBN(String pBestandsnaam)
        {
            String lFarmNumber;
            String lLand;
            String lNaam;
            String lVoorvoegsel;
            String lStraat;
            String lHuisnummer;
            String lPostcode;
            String lWoonplaats;
            String lTelefoonnr;
            int error;
            error = OpvragenBedrijfsgegevens(pBestandsnaam, out lLand, out lFarmNumber,
                 out lNaam, out lVoorvoegsel, out lStraat, out lHuisnummer,
                      out lPostcode, out lWoonplaats, out lTelefoonnr);

            return lFarmNumber;
        }

        public override bool LeesBestand(int pThrId, String pBestandsnaam, String pUBN, int pProgId, int Programid)
        {
            int UBNid = mSavetoDB.GetUBNidbyUBN(pUBN);
            return LeesBestand(pThrId, pBestandsnaam, UBNid, pProgId, Programid);
        }

        public override bool LeesBestand(int pThrId, String pBestandsnaam, int UBNid, int pProgId, int Programid)
        {
            lFarm = mSavetoDB.GetBedrijfByUbnIdProgIdProgramid(UBNid, pProgId, Programid);
            return LeesBestand(pThrId, pBestandsnaam, lFarm.FarmId);
        }

        public bool LeesEVOBestand(int pThrId, String pBestandsnaam, int pFarmId)
        {
            try
            {

                lFarm = mSavetoDB.GetBedrijfById(pFarmId);


                String lFarmNumber;
                String lLand;
                String lNaam;
                String lVoorvoegsel;
                String lStraat;
                String lHuisnummer;
                String lPostcode;
                String lWoonplaats;
                String lTelefoonnr;
                int error;
                error = OpvragenBedrijfsgegevens(pBestandsnaam, out lLand, out lFarmNumber,
                     out lNaam, out lVoorvoegsel, out lStraat, out lHuisnummer,
                          out lPostcode, out lWoonplaats, out lTelefoonnr);
                //
                if (error != -1)
                {
                    UBN lUBN = mSavetoDB.getUBNByBedrijfsnummer(removevoorloopnullen(lFarmNumber));
                    if (lUBN.UBNid != lFarm.UBNid)
                    {
                        unLogger.WriteDebug("Het UBN van dit bestand zit niet in het programma!");
                        mFacade.UpdateProgress(-1, "Het UBN van dit bestand zit niet in het programma! \r\n \r\nUBN in bestand: " + lFarmNumber);
                    }
                    else if (lUBN.UBNid != 0 && mSavetoDB.UBNinDB(removevoorloopnullen(lFarmNumber)))
                    {
                        THIRD lThFarm = mSavetoDB.GetThirdByThirId(lUBN.ThrID);
                        lThFarm.ThrZipCode = lPostcode;
                        lThFarm.ThrFirstName = lVoorvoegsel;
                        lThFarm.ThrFarmNumber = removevoorloopnullen(lFarmNumber);
                        lThFarm.ThrSecondName = lNaam;
                        lThFarm.ThrCity = lWoonplaats;
                        lThFarm.ThrPhoneNumber = lTelefoonnr;
                        lThFarm.ThrStreet1 = lStraat;
                        lThFarm.ThrExt = lHuisnummer;
                        if (lThFarm.ThrId == 0) mSavetoDB.SaveThird(lThFarm);
                        else
                        {
                            mSavetoDB.SaveThird(lThFarm);
                            lUBN.ThrID = lThFarm.ThrId;
                        }
                        mFacade.UpdateProgress(60, "Inlezen Melkcontrole");
                        LeesProduktie(pThrId, pBestandsnaam, true);
                        mFacade.UpdateProgress(95, "Inlezen voltooien");
                        mFacade.UpdateProgress(99, "Inlezen voltooid.");
                        lFarm = null;
                        return true;
                    }
                    else if (mSavetoDB.Plugin() == "T4C")
                    {
                        unLogger.WriteDebug("UBN niet in T4C! \r\n \r\nUBN in bestand: " + lFarmNumber);
                        mFacade.UpdateProgress(-1, "UBN niet in T4C! \r\n \r\nUBN in bestand: " + lFarmNumber);
                    }
                    else
                    {
                        unLogger.WriteDebug("UBN niet in de database!\r\n \r\nUBN in bestand: " + lFarmNumber);
                        mFacade.UpdateProgress(-1, "UBN niet in de database!\r\n \r\nUBN in bestand: " + lFarmNumber);
                    }
                }
                else
                {
                    unLogger.WriteDebug("Fout bij lezen bestand!");
                    mFacade.UpdateProgress(-1, "Fout bij lezen bestand!");
                }
            }
            catch (Exception ex)
            {
                mFacade.UpdateProgress(-1, "Onbekende Fout! \r\n" + ex.Message);
                unLogger.WriteError(ex.Message, ex);
            }
            lFarm = null;
            return false;
        }

        public override bool LeesBestand(int pThrId, String pBestandsnaam, int pFarmId)
        {
            try
            {
      
                lFarm = mSavetoDB.GetBedrijfById(pFarmId);


                String lFarmNumber;
                String lLand;
                String lNaam;
                String lVoorvoegsel;
                String lStraat;
                String lHuisnummer;
                String lPostcode;
                String lWoonplaats;
                String lTelefoonnr;
                int error;
                error = OpvragenBedrijfsgegevens(pBestandsnaam, out lLand, out lFarmNumber,
                     out lNaam, out lVoorvoegsel, out lStraat, out lHuisnummer,
                          out lPostcode, out lWoonplaats, out lTelefoonnr);
                //
                if (error != -1)
                {
                    UBN lUBN = mSavetoDB.getUBNByBedrijfsnummer(removevoorloopnullen(lFarmNumber));
                    if (lUBN.UBNid != lFarm.UBNid)
                    {
                        unLogger.WriteDebug("Het UBN van dit bestand zit niet in het programma!");
                        mFacade.UpdateProgress(-1, "Het UBN van dit bestand zit niet in het programma! \r\n \r\nUBN in bestand: " + lFarmNumber);
                    }
                    else if (lUBN.UBNid != 0 && mSavetoDB.UBNinDB(removevoorloopnullen(lFarmNumber)))
                    {
                        THIRD lThFarm = mSavetoDB.GetThirdByThirId(lUBN.ThrID);
                        lThFarm.ThrZipCode = lPostcode;
                        lThFarm.ThrFirstName = lVoorvoegsel;
                        lThFarm.ThrFarmNumber = removevoorloopnullen(lFarmNumber);
                        lThFarm.ThrSecondName = lNaam;
                        lThFarm.ThrCity = lWoonplaats;
                        lThFarm.ThrPhoneNumber = lTelefoonnr;
                        lThFarm.ThrStreet1 = lStraat;
                        lThFarm.ThrExt = lHuisnummer;
                        if (lThFarm.ThrId == 0) mSavetoDB.SaveThird(lThFarm);
                        else
                        {
                            mSavetoDB.SaveThird(lThFarm);
                            lUBN.ThrID = lThFarm.ThrId;
                        }
                        mFacade.UpdateProgress(5, "Inlezen Werknummers");
                        LeesDierRegistratie(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(10, "Inlezen Stieren");
                        LeesKIstier(pBestandsnaam);
                        mFacade.UpdateProgress(15, "Inlezen Dieren");
                        LeesDieren(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(20, "Inlezen Raskenmerken");
                        LeesRas(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(30, "Inlezen Ouders");
                        LeesDieren(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(40, "Inlezen Afkalven");
                        LeesAfkalven(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(45, "Inlezen Inseminaties");
                        LeesInseminatie(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(45, "Inlezen Embryo Implantatie");
                        LeesEmbryo(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(50, "Inlezen Drachtcontrole");
                        LeesDrachtigheidsstatus(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(55, "Inlezen Droogzetten");
                        LeesProduktiestatus(pThrId,  pBestandsnaam);
                        mFacade.UpdateProgress(60, "Inlezen Melkcontrole");
                        LeesProduktie(pThrId, pBestandsnaam,false);
                        mFacade.UpdateProgress(75, "Inlezen Aanvoer");
                        LeesAanvoer(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(80, "Inlezen Afvoer");
                        LeesAfvoer(pThrId, pBestandsnaam);
                        mFacade.UpdateProgress(85, "Inlezen Bedrijfsproductie");
                        LeesBedrijfsgegevensDagproduktie(pBestandsnaam);
                        //na opslaan toegevoegd 20-07
                        mFacade.UpdateProgress(88, "Inlezen Melkproductie");
                        LeesMelking(pBestandsnaam);
                        mFacade.UpdateProgress(99, "Inlezen voltooid.");
                        lFarm = null;
                        return true;
                    }
                    else if (mSavetoDB.Plugin() == "T4C")
                    {
                        unLogger.WriteDebug("UBN niet in T4C! \r\n \r\nUBN in bestand: " + lFarmNumber);
                        mFacade.UpdateProgress(-1, "UBN niet in T4C! \r\n \r\nUBN in bestand: " + lFarmNumber);
                    }
                    else
                    {
                        unLogger.WriteDebug("UBN niet in de database!\r\n \r\nUBN in bestand: " + lFarmNumber);
                        mFacade.UpdateProgress(-1, "UBN niet in de database!\r\n \r\nUBN in bestand: " + lFarmNumber);
                    }
                }
                else
                {
                    unLogger.WriteDebug("Fout bij lezen bestand!");
                    mFacade.UpdateProgress(-1, "Fout bij lezen bestand!");
                }
            }
            catch (Exception ex)
            {
                mFacade.UpdateProgress(-1, "Onbekende Fout! \r\n" + ex.Message);
                unLogger.WriteError(ex.Message, ex);
            }
            lFarm = null;
            return false;
        }

    }
}
