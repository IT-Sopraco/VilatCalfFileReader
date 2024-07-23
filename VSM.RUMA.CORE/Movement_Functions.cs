using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Configuration;



using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;

using System.Net;

namespace VSM.RUMA.CORE
{



    [Serializable()]
    public class sDestination
    {
        public int UbnId { get; set; }
        public int ThirdId { get; set; }
        public string Bedrijfsnummer { get; set; }
        public string ThrCity { get; set; }
        public string ThrSecondName { get; set; }
        public string ThrCompanyName { get; set; }
        public List<string[]> BedrijfsZiekteStatussen { get; set; }

        public sDestination()
        {
            UbnId = 0;
            ThirdId = 0;
            Bedrijfsnummer = "";
            ThrCity = "";
            ThrSecondName = "";
            ThrCompanyName = "";
            BedrijfsZiekteStatussen = new List<string[]>();
        }
    }

    public class MovFunc
    {
        private static AFSavetoDB getMysqlDb(UserRightsToken pUr)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            return lMstb;
        }

        public static List<MOVEMENT> getAllMovements(UserRightsToken pUr, int pAniId, int pUbnId)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            //List<MOVEMENT> movsNormalMovements = lMstb.GetMovementsByUbn(pAniId, pUbnId);

            //List<MOVEMENT> aniUBNNulMovements = lMstb.GetMovementsByUbn(pAniId, 0);

            List<MOVEMENT> movs = lMstb.GetMovementsByAniId(pAniId);// movsNormalMovements.Concat(aniUBNNulMovements).ToList();

            List<MOVEMENT> movs2 = new List<MOVEMENT>();
            //sorteren op datum en dan MovId ivm movements op dezelfde dag
            //ik kijk naar de hoogste MovId om de laatste van een dag te bepalen
            //ipv MovTime (die is in het verleden niet altijd goed gegaan)
            var temp = from n in movs
                       where n.UbnId == pUbnId 
                       orderby n.MovDate descending, n.MovTime descending, n.MovId descending
                       select n;
            return temp.ToList();

        }

        public static string opslaan_aanvoer(BEDRIJF pBedrijf, UserRightsToken pUr, MOVEMENT pMov, BUYING pBui, string pCountryCodeDepart, string pCountryCodeBirth, XDocument xBerichtenbestand, out int LMovId, out string melden)
        {
            return opslaan_aanvoer(pBedrijf, pUr, pMov, pBui, pCountryCodeDepart, pCountryCodeBirth, xBerichtenbestand, true, out LMovId, out melden);
        }

        public static string opslaan_aanvoer(BEDRIJF pBedrijf, UserRightsToken pUr, MOVEMENT pMov, BUYING pBui, string pCountryCodeDepart, string pCountryCodeBirth, XDocument xBerichtenbestand, bool Mutatieklaarzetten, out int LMovId, out string melden)
        {
            string returnantwoord = "";

            melden = "";
            LMovId = 0;
            if (pMov.AniId == 0)
            {
                return "Het dier is nog niet bekend tijdens deze aanvoer.";
            }
            #region opslaan
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(pMov.AniId, pBedrijf.FarmId);
            ANIMAL ani = lMstb.GetAnimalById(pMov.AniId);
            FARMCONFIG FCIRaanvoer = lMstb.getFarmConfig(pBedrijf.FarmId, "IRaanvoer", "True");
            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "True");
            UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);

            if (pMov.MovId > 0)
            {
                LMovId = pMov.MovId;

                pMov.MovOrder = getMovementOrder(pUr, pMov.MovId);
                if (lMstb.UpdateMovement(pMov))
                {
                    pBui.MovId = pMov.MovId;

                    if (!lMstb.UpdateBuying(pBui))
                    {
                        returnantwoord = "Error: Update Buying" + pMov.MovId.ToString();
                    }
                    else if (Mutatieklaarzetten)
                    {

                        melden = saveAanVoerMutation(pUr, pBedrijf, FCIRaanvoer, pMov, pBui, pCountryCodeDepart, pCountryCodeBirth);

                    }
                    //else if (!Mutatieklaarzetten)
                    //{
                    //    melden = updateAanVoerMutation(pUr, pBedrijf, FCIRaanvoer, pMov, pBui, pCountryCodeDepart, pCountryCodeBirth);
                    //}

                    if (aniscat.AniId != pMov.AniId)
                    {
                        aniscat = new ANIMALCATEGORY();
                        aniscat.AniId = pMov.AniId;
                        aniscat.FarmId = pBedrijf.FarmId;
                        aniscat.AniMinasCategory = ani.AniMinasCategory;
                        aniscat.AniWorknumber = getNewWorknumber(pUr, pBedrijf, ani, "");
                    }


                    MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pMov.Changed_By, pMov.SourceID);
                }
                else
                {
                    returnantwoord = "Error: Update Movement " + pMov.MovId.ToString();

                }
            }
            else
            {
                //nieuwe
                pMov.MovOrder = MovFunc.getNewMovementOrder(pUr, pMov.MovKind, pMov.MovDate, pMov.AniId, pBedrijf.UBNid);
                pMov.happened_at_FarmID = pBedrijf.FarmId;

                if (lMstb.SaveMovement(pMov))
                {
                    pBui.MovId = pMov.MovId;
                    LMovId = pMov.MovId;
                    if (lMstb.SaveBuying(pBui))
                    {

                        if (aniscat.AniId != pMov.AniId)
                        {
                            aniscat = new ANIMALCATEGORY();
                            aniscat.AniId = pMov.AniId;
                            aniscat.FarmId = pBedrijf.FarmId;
                            aniscat.AniMinasCategory = ani.AniMinasCategory;
                            aniscat.AniWorknumber = getNewWorknumber(pUr, pBedrijf, ani, "");
                        }


                        MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pMov.Changed_By, pMov.SourceID);

                        #region mutation
                        if (Mutatieklaarzetten)
                        {
                            melden = saveAanVoerMutation(pUr, pBedrijf, FCIRaanvoer, pMov, pBui, pCountryCodeDepart, pCountryCodeBirth);
                        }
                        #endregion mutation


                        /*
                                
                                Als voor NSFO een dier aangevoerd wordt vanuit een ander stamboek dan dient de Scrapiestatus van het dier dus altijd naar 0 gezet te worden.
                                (voor verplaatsingen van NSFO --> NSFO hoeft dat niet. Voor NSFO --> TES ook niet, en voor TES --> TES ook niet )
                         */
                        //List<int> nsfos = utils.getNsfoProgramIds();
                        //nsfos.Add(47);
                        //nsfos.Add(49);
                        //if (nsfos.Contains(pBedrijf.Programid) && pBedrijf.ProgId == 3)//het zal wel. maar...
                        //{
                        //    if (pMov.MovThird_UBNid > 0)
                        //    {

                        //        List<BEDRIJF> aanvoerenvan = lMstb.getBedrijvenByUBNId(pMov.MovThird_UBNid);
                        //        var c = (from n in aanvoerenvan where n.ProgId == 3 select n).ToList();
                        //        foreach (BEDRIJF br in c)
                        //        {
                        //            if (!nsfos.Contains(br.Programid))
                        //            {

                        //                ani.AniScrapie = 0;

                        //                lMstb.SaveAnimal(ubu.ThrID, ani);

                        //            }
                        //        }
                        //    }
                        //    //else 
                        //    //{
                        //    //    ani.AniScrapie = 0;

                        //    //    lMstb.SaveAnimal(ubu.ThrID, ani);
                        //    //}
                        //}
                    }
                }
                else
                {
                    //lblopmerkingen.Text = "opslaan mislukt!";
                    returnantwoord = "Error: Saving Movement " + pMov.MovId.ToString();

                }

            }
            #endregion
            return returnantwoord;
        }

        private static string getNewWorknumber(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, string pAltWorknumber)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            if (pAltWorknumber != null && pAltWorknumber.Trim() != "")
            {
                return pAltWorknumber;
            }
            else
            {
                int start = pAnimal.AniLifeNumber.Length - 5;
                if (pAnimal.AniLifeNumber.Length > 5)
                {
                    if (pBedrijf.ProgId == 1 || pBedrijf.ProgId == 2 || pBedrijf.ProgId == 4 || pBedrijf.ProgId == 6 || pBedrijf.ProgId == 7 || pBedrijf.ProgId == 8 || pBedrijf.ProgId == 9)
                    {
                        return pAnimal.AniLifeNumber.Trim().Substring(start, 4);
                    }
                    else
                    {
                        return pAnimal.AniLifeNumber.Trim().Substring(start, 5);
                    }
                }
                else
                {
                    return pAnimal.AniLifeNumber;
                }
            }

        }

        public static string verwijder_aanvoer(BEDRIJF pBedrijf, UserRightsToken pUr, int pMovId, int pChangedBy, int pSourceID)
        {
            if (pMovId > 0)
            {
                bool alIRgemeld = false;
                MUTATION mut = new MUTATION();
                AFSavetoDB lMstb = getMysqlDb(pUr);
                FARMCONFIG FCIRaanvoer = lMstb.getFarmConfig(pBedrijf.FarmId, "IRaanvoer", "True");
                FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "True");
                MOVEMENT mv = lMstb.GetMovementByMovId(pMovId);
                mv.Changed_By = pChangedBy;
                mv.SourceID = pSourceID;

                if (mv.MovKind > 0 )//== 1)
                {
                    if (mv.UbnId != pBedrijf.UBNid)
                    {
                        return "U mag deze gegevens niet verwijderen.";
                    }
                    ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(mv.AniId, pBedrijf.FarmId);
                    BUYING boei = lMstb.GetBuyingByMovId(pMovId);
                    MUTALOG mutlog = lMstb.GetMutaLogByMovId(pMovId);
                    aniscat.Changed_By = pChangedBy;
                    aniscat.SourceID = pSourceID;
                    boei.Changed_By = pChangedBy;
                    boei.SourceID = pSourceID;
                    mutlog.Changed_By = pChangedBy;
                    mutlog.SourceID = pSourceID;
                    if (aniscat.AniId != 0)
                    {
                        if (aniscat.Ani_Mede_Eigenaar != 1)
                        {
                            if (aniscat.Anicategory > 4)
                            { return "Dier is nooit bij u aanwezig geweest."; }
                        }
                        else { return "U bent alleen medeeigenaar."; }
                    }
                    else { return "Dier niet bij u bekend"; }

                    if (mutlog.MovId > 0)
                    {
                        if (mutlog.Returnresult == 1 || mutlog.Returnresult == 3)
                        {
                            alIRgemeld = true;
                        }
                        if (mutlog.Returnresult == 97)
                        {
                            alIRgemeld = false;//de melding is ingetrokken dus de aanvoer kan verwijderd worden
                        }
                    }

                    if (!alIRgemeld)
                    {
                        mut = lMstb.GetMutationByMovId(pMovId);
                    }

                    if (FCIRaanvoer.FValue.ToLower() == "false")
                    { alIRgemeld = false; }
                    if (!alIRgemeld)
                    {

            
                        if (lMstb.DeleteBuying(boei))
                        {
                            unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " BUYING verwijderen , verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer . pMovId:" + pMovId.ToString());
                               
                            if (lMstb.DeleteMovement(mv))
                            {
                                unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " MOVEMENT verwijderen  , verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer . pMovId:" + pMovId.ToString());
                                int oud = aniscat.Anicategory;
                                SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                                unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " ANIMALCATEGORY Aanpassen van "+ oud.ToString() + " naar " + aniscat.Anicategory.ToString() + ", verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer . pMovId:" + pMovId.ToString());
                                

                                if (mut.MovId > 0)
                                {
                                    lMstb.DeleteMutation(mut);
                                    unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " MUTATION, verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer . pMovId:" + pMovId.ToString());
                                
                                }
                            }
                            else
                            {
                                unLogger.WriteDebug("FarmId:" + pBedrijf.FarmId.ToString() + " BUYING verwijderen wel gelukt, verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer fout. pMovId:" + pMovId.ToString());
                                return "Verwijderen aanvoer niet gelukt";
                            }
                        }
                        else
                        {

                            if (lMstb.DeleteMovement(mv))
                            {
                                unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " MOVEMENT verwijderen  , verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer . pMovId:" + pMovId.ToString());
                                int oud = aniscat.Anicategory;
                                SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                                unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " ANIMALCATEGORY Aanpassen van " + oud.ToString() + " naar " + aniscat.Anicategory.ToString() + ", verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer . pMovId:" + pMovId.ToString());
                               
                                if (mut.MovId > 0)
                                {
                                    lMstb.DeleteMutation(mut);
                                }
                                unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " BUYING.movid=0 verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer fout. pMovId:" + pMovId.ToString());
                            }
                            else
                            {
                                unLogger.WriteDebug("FarmId:" + pBedrijf.FarmId.ToString() + " BUYING.movid=0 verwijderen aanvoer MOVEMENT in Mov_func.verwijder_aanvoer fout. pMovId:" + pMovId.ToString());
                                return "Verwijderen aanvoer niet gelukt";
                            }

                        }

                        return "";

                    }
                    else { return "Aanvoer is RVO gemeld"; }




                }
                else { return "Dit is geen aanvoer"; }
            }
            else
            {
                return "Geen aanvoer bekend";
            }
        }

        public static string verwijder_afvoer(BEDRIJF pBedrijf, UserRightsToken pUr, int pMovId, int pChangedBy, int pSourceID)
        {
            string debuginfo = "Verwijderen Afvoer :Farmid:" + pBedrijf.FarmId.ToString() + " verwijderen afvoer in Mov_func.verwijder_afvoer fout. MovId:" + pMovId.ToString();
            if (pMovId > 0)
            {

                AFSavetoDB lMstb = getMysqlDb(pUr);
                FARMCONFIG FCIRaanvoer = lMstb.getFarmConfig(pBedrijf.FarmId, "IRaanvoer", "True");
                FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "True");
                MOVEMENT mv = lMstb.GetMovementByMovId(pMovId);
                SALE sle = lMstb.GetSale(pMovId);
                ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(mv.AniId, pBedrijf.FarmId);
                mv.Changed_By = pChangedBy;
                mv.SourceID = pSourceID;
                sle.Changed_By = pChangedBy;
                sle.SourceID = pSourceID;
                aniscat.Changed_By = pChangedBy;
                aniscat.SourceID = pSourceID;
                if (mv.MovKind >0 )//== 2)
                {
                    if (mv.UbnId != pBedrijf.UBNid)
                    {
                        return "U mag deze gegevens niet verwijderen.";
                    }

                    if (aniscat.AniId != 0)
                    {
                        if (aniscat.Ani_Mede_Eigenaar != 1)
                        {
                            if (aniscat.Anicategory > 4)
                            { return "Dier is nooit bij u aanwezig geweest."; }
                        }
                        else { return "U bent alleen medeeigenaar."; }
                    }
                    else { return "Dier niet bij u bekend"; }


                    string ret = intrekDeleteMelding(pUr, FCIRviaModem, mv.MovId);

                    if (ret != "")
                    {
                        return ret;
                    }


                    if (lMstb.DeleteSale(sle))
                    {
                        unLogger.WriteInfo("Verwijder SALE afvoer MovId=" + pMovId.ToString());
                        if (lMstb.DeleteMovement(mv))
                        {
                            unLogger.WriteInfo("Verwijder MOVEMENT afvoer MovId=" + pMovId.ToString());
                            int oud = aniscat.Anicategory;
                            SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                            unLogger.WriteInfo("Aanpassen ANIMALCATEGORY afvoer MovId=" + pMovId.ToString() + " van " + oud.ToString() + "naar:" + aniscat.Anicategory.ToString());


                        }
                        else
                        {
                            unLogger.WriteDebug("FarmId:" + pBedrijf.FarmId.ToString() + " verwijderen Sale.MovId=" + pMovId.ToString() + " wel gelukt, maar afvoer MOVEMENT niet, in Mov_func.verwijder_afvoer fout. MOVEMENT.MovId:" + mv.MovId.ToString());
                            return "Verwijderen afvoer niet gelukt";
                        }
                    }
                    else
                    {


                        if (lMstb.DeleteMovement(mv))
                        {
                            unLogger.WriteInfo("Verwijder MOVEMENT afvoer MovId=" + pMovId.ToString());
                            int oud = aniscat.Anicategory;
                            SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                            unLogger.WriteInfo("Aanpassen ANIMALCATEGORY afvoer MovId=" + pMovId.ToString() + " van " + oud.ToString() + "naar:" + aniscat.Anicategory.ToString());

 
                        }
                        else
                        {
                            unLogger.WriteInfo("FarmId:" + pBedrijf.FarmId.ToString() + " verwijderen Sale.MovId=0 niet gelukt afvoer MOVEMENT in Mov_func.verwijder_afvoer fout. MOVEMENT.MovId:" + pMovId.ToString());
                            return "Verwijderen afvoer niet gelukt";
                        }

                    }

                    return ret;

                }
                else { return "Dit is geen Afvoer"; }
            }
            else
            {
                return "Geen Afvoer bekend";
            }
        }

        public static string verwijder_dood(BEDRIJF pBedrijf, UserRightsToken pUr, int pMovId, int pChangedBy, int pSourceID)
        {
            if (pMovId > 0)
            {
                //bool alIRgemeld = false;

                AFSavetoDB lMstb = getMysqlDb(pUr);
                FARMCONFIG FCIRaanvoer = lMstb.getFarmConfig(pBedrijf.FarmId, "IRaanvoer", "True");
                FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "True");
                MOVEMENT mv = lMstb.GetMovementByMovId(pMovId);
                mv.Changed_By = pChangedBy;
                mv.SourceID = pSourceID;
                if (mv.MovKind == 3)
                {
                    if (mv.UbnId != pBedrijf.UBNid)
                    {
                        return "U mag deze gegevens niet verwijderen.";
                    }
                    ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(mv.AniId, pBedrijf.FarmId);
                    LOSS los = lMstb.GetLoss(pMovId);
                    aniscat.Changed_By = pChangedBy;
                    aniscat.SourceID = pSourceID;
                    los.Changed_By = pChangedBy;
                    los.SourceID = pSourceID;
                    if (aniscat.AniId != 0)
                    {
                        if (aniscat.Ani_Mede_Eigenaar != 1)
                        {
                            if (aniscat.Anicategory > 4)
                            { return "Dier is nooit bij u aanwezig geweest."; }
                        }
                        else { return "U bent alleen medeeigenaar."; }
                    }
                    else { return "Dier niet bij u bekend"; }


                    string ret = intrekDeleteMelding(pUr, FCIRviaModem, mv.MovId);
                    if (ret != "")
                    {
                        return ret;
                    }
                    unLogger.WriteInfo("Verwijder dood MovID=" + pMovId.ToString());
                    if (lMstb.DeleteLoss(los))
                    {
                       
                        if (lMstb.DeleteMovement(mv))
                        {
                            int oud = aniscat.Anicategory;
                            SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                            unLogger.WriteInfo("Aanpassen ANIMALCATEGORY dood MovId=" + pMovId.ToString() + " van " + oud.ToString() + "naar:" + aniscat.Anicategory.ToString());

                        }
                        else
                        {
                            unLogger.WriteDebug("FarmId:" + pBedrijf.FarmId.ToString() + " verwijderen Loss wel gelukt, dood MOVEMENT in Mov_func.verwijder_dood fout.MovId:" + pMovId.ToString());
                            return "Verwijderen dood niet gelukt";
                        }
                    }
                    else
                    {
                        if (los.MovId == 0)
                        {
                            if (lMstb.DeleteMovement(mv))
                            {
                                int oud = aniscat.Anicategory;
                                SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                                unLogger.WriteInfo("Aanpassen ANIMALCATEGORY dood MovId=" + pMovId.ToString() + " van " + oud.ToString() + "naar:" + aniscat.Anicategory.ToString());
                            }
                            else
                            {
                                unLogger.WriteDebug("FarmId:" + pBedrijf.FarmId.ToString() + " verwijderen Loss.MovId=0 dood MOVEMENT in Mov_func.verwijder_dood fout.");
                                return "Verwijderen dood niet gelukt";
                            }
                        }
                        else
                        {
                            unLogger.WriteDebug("FarmId:" + pBedrijf.FarmId.ToString() + " verwijderen dood LOSS.MovId:" + los.MovId.ToString() + "  in Mov_func.verwijder_dood fout.");
                            return "Verwijderen dood niet gelukt";
                        }
                    }

                    return ret;


                }
                else { return "Dit is geen Dood"; }
            }
            else
            {
                return "Geen Dood bekend";
            }
        }
        
        public static string saveAanVoerMutation(UserRightsToken pUr, BEDRIJF pBedrijf, FARMCONFIG pFCIRaanvoer, MOVEMENT pMov, BUYING pBui, string pCountryCodeDepart, string pCountryCodeBirth)
        {
            string melden = "";
            pCountryCodeBirth = "";//Bij nader inzien leeglaten 
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMAL ani = lMstb.GetAnimalById(pMov.AniId);
            ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(pMov.AniId, pBedrijf.FarmId);
            THIRD Thridherkomst = lMstb.GetThirdByThirId(pBui.PurOrigin);
            UBN Ubnherkomst = lMstb.GetubnById(pMov.MovThird_UBNid);
            UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
            if (pFCIRaanvoer.FValue.ToLower() == "true")
            {
                MUTATION mut = new MUTATION();
                if (pMov.MovId > 0)
                {
                    mut = lMstb.GetMutationByMovId(pMov.MovId);

                    // kijken of het een herstelmelding is.
                    if (mut.Internalnr == 0)
                    {
                        MUTALOG mutlog = lMstb.GetMutaLogByMovId(pMov.MovId);
                        if (mutlog.Internalnr != 0 && (mutlog.Returnresult == 1 || mutlog.Returnresult == 3))
                        {
                            mut.MeldingNummer = mutlog.MeldingNummer;
                            mut.Returnresult = 96;
                            mut.CodeMutation += 100;
                        }
                    }

                }



                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    mut.Lifenumber = getGoodAlternateNumber(ani.AniAlternateNumber);
                }
                else
                {
                    mut.Lifenumber = ani.AniLifeNumber;
                }
                mut.Worknumber = aniscat.AniWorknumber;
                mut.IDRBirthDate = ani.AniBirthDate;
                if (pMov.MovKind == 3)
                {
                    mut.IDRLossDate = pMov.MovDate;
                    mut.LossDate = mut.IDRLossDate;
                }
                else if (pMov.MovKind == 2)
                {
                    SALE s = lMstb.GetSale(pMov.MovId);
                    if (s.SalKind == 2 || s.SalKind == 3)
                    {
                        mut.IDRLossDate = pMov.MovDate;
                        mut.LossDate = mut.IDRLossDate;
                    }
                }
                mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                mut.Name = ani.AniName;

                if (ani.AniCountryCodeBirth == "")
                {
                    mut.CountryCodeBirth = pCountryCodeBirth;
                    if (mut.CountryCodeBirth == "")
                    {
                        try
                        {
                            mut.CountryCodeBirth = mut.Lifenumber.Substring(0, 2);
                        }
                        catch (Exception exc) { unLogger.WriteInfo(exc.ToString()); }
                    }
                }
                else
                {
                    mut.CountryCodeBirth = ani.AniCountryCodeBirth;
                }
                if (Thridherkomst.ThrCountry != "")
                {
                    int lCode = 0;
                    int.TryParse(Thridherkomst.ThrCountry, out lCode);
                    if (lCode > 0)
                    {
                        COUNTRY lHerkomst = lMstb.GetCountryByLandid(lCode);
                        mut.CountryCodeDepart = lHerkomst.LandAfk2;
                    }
                }
                mut.AlternateLifeNumber = ani.AniAlternateNumber;
                mut.NrGezondheidsCert = pBui.PurNrGezondheidsCert;
                if (pBui.PurKind == 1)
                {

                    mut.CodeMutation = 7;//import

                    if (pCountryCodeDepart == "GB")
                    {
                        pCountryCodeDepart = "UK";
                    }
                    if (pCountryCodeDepart != "")
                    {
                        mut.CountryCodeDepart = pCountryCodeDepart;
                    }
                    if (mut.CountryCodeDepart == "")
                    {
                        string code = mut.Lifenumber;
                        try
                        {
                            mut.CountryCodeDepart = code.Substring(0, 2);
                        }
                        catch { }
                    }


                    if (CORE.utils.isEULand(mut.CountryCodeDepart))
                    {

                        mut.AlternateLifeNumber = "";
                    }
                    
                }
                else
                {

                    mut.CodeMutation = 1;//aanvoer

                }

                mut.Sex = ani.AniSex;
                mut.Haircolor = ani.Anihaircolor;
                mut.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                mut.MovId = pMov.MovId;
                mut.UbnId = pMov.UbnId;
                mut.Nling = ani.AniNling;
                mut.Farmnumber = ubu.Bedrijfsnummer;
                mut.FarmNumberTo = ubu.Bedrijfsnummer;
                TRANSPRT tr = lMstb.GetTransprt(pBui.PurTransportID);
                mut.LicensePlate = tr.LicensePlate;

                THIRD aanvvoertransporteur = lMstb.GetThirdByThirId(pBui.PurTransporter);
                mut.Vervoersnr = aanvvoertransporteur.ThrVATNumber;


                mut.Purchaser = Thridherkomst.ThrVATNumber;


                mut.FarmNumberFrom = Ubnherkomst.Bedrijfsnummer;

                if (ani.AniIdMother != 0)
                {
                    ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        mut.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                    }
                    else
                    {
                        mut.LifenumberMother = moeder.AniLifeNumber;
                    }
                }
                mut.MutationDate = pMov.MovDate;
                mut.MutationTime = pMov.MovTime;
                if (ani.AniIdFather != 0)
                {
                    ANIMAL vader = lMstb.GetAnimalById(ani.AniIdFather);
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        mut.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);

                    }
                    else
                    {
                        mut.LifeNumberFather = vader.AniLifeNumber;
                    }
                }

                if (pBedrijf.ProgId == 25)
                {
                    mut.SendTo = 35;
                }
                else
                {
                    RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                    string defIenRaction = r.getdefIenRaction(pUr, pBedrijf.UBNid, pBedrijf.ProgId, pBedrijf.Programid);
                    FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedrijf.FarmId, "VerstuurIenR", defIenRaction);

                    mut.SendTo = IenRCom.ValueAsInteger();
                }

                lMstb.SaveMutation(mut);
                melden = "melden";
            }
            else
            {
                MUTALOG mutlog = new MUTALOG();
                if (pMov.MovId != 0)
                { mutlog = lMstb.GetMutaLogByMovId(pMov.MovId); }

                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    mutlog.Lifenumber = getGoodAlternateNumber(ani.AniAlternateNumber);
                }
                else
                {
                    mutlog.Lifenumber = ani.AniLifeNumber;
                }
                mutlog.Worknumber = aniscat.AniWorknumber;
                mutlog.Name = ani.AniName;
                if (pBui.PurKind == 1)
                {

                    mutlog.CodeMutation = 7;

                }
                else
                {
                    mutlog.CodeMutation = 1;//aanvoer 
                }
                mutlog.IDRBirthDate = ani.AniBirthDate;
                if (pMov.MovKind == 3)
                { mutlog.IDRLossDate = pMov.MovDate; }
                mutlog.Sex = ani.AniSex;
                mutlog.Haircolor = ani.Anihaircolor;
                mutlog.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                mutlog.MovId = pMov.MovId;
                mutlog.UbnId = pMov.UbnId;
                mutlog.Nling = ani.AniNling;
                mutlog.Farmnumber = ubu.Bedrijfsnummer;
                mutlog.FarmNumberTo = ubu.Bedrijfsnummer;
                TRANSPRT tr = lMstb.GetTransprt(pBui.PurTransportID);
                mutlog.LicensePlate = tr.LicensePlate;
                THIRD aanvvoertransporteur = lMstb.GetThirdByThirId(pBui.PurTransporter);
                mutlog.Vervoersnr = aanvvoertransporteur.ThrVATNumber;

                mutlog.Purchaser = Thridherkomst.ThrVATNumber;

                mutlog.FarmNumberFrom = Ubnherkomst.Bedrijfsnummer;

                if (ani.AniIdMother != 0)
                {
                    ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        mutlog.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                    }
                    else
                    {
                        mutlog.LifenumberMother = moeder.AniLifeNumber;
                    }
                }
                mutlog.MutationDate = pMov.MovDate;
                mutlog.MutationTime = pMov.MovTime;
                if (ani.AniIdFather != 0)
                {
                    ANIMAL vader = lMstb.GetAnimalById(ani.AniIdFather);
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        mutlog.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);

                    }
                    else
                    {
                        mutlog.LifeNumberFather = vader.AniLifeNumber;
                    }
                }
                mutlog.AlternateLifeNumber = ani.AniAlternateNumber;

                lMstb.InsertMutLog(mutlog);
            }
            return melden;
        }

        public static string opslaan_afvoer(int pThrId, int pUbnId, int pProgid, int pProgramId, UserRightsToken pUr, MOVEMENT mov, SALE sal, int pChangedBy, int pSourceID)
        {
            if (pUbnId > 0)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                List<BEDRIJF> bedrijven = lMstb.getBedrijvenByUBNId(pUbnId);
                BEDRIJF b = new BEDRIJF();
                if (bedrijven.Count() == 0)
                {
                    b.UBNid = pUbnId;
                    b.ProgId = pProgid;
                    b.Programid = pProgramId;
                }
                else
                {

                    if (bedrijven.Any(x => x.ProgId == pProgid && x.Programid == pProgramId))
                    {
                        b = bedrijven.Find(x => x.ProgId == pProgid && x.Programid == pProgramId);
                    }
                    else
                    {
                        if (bedrijven.Any(x => x.ProgId == pProgid))
                        {
                            b = bedrijven.Find(x => x.ProgId == pProgid);
                        }
                        else
                        {
                            b.UBNid = pUbnId;
                            b.ProgId = pProgid;
                            b.Programid = pProgramId;
                        }
                    }
                }
               return  opslaan_afvoer(pThrId, b, pUr, mov, sal, pChangedBy, pSourceID);
            }
            return "Bedrijf onbekend";
        }

        public static string opslaan_afvoer(int pThrId, BEDRIJF pBedrijf, UserRightsToken pUr, MOVEMENT mov, SALE sal, int pChangedBy, int pSourceID)
        {
            try
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                //check verplichte velden
                bool bestemmingVerplicht = false;
                if (pBedrijf.ProgId == 5 || pBedrijf.ProgId == 3)
                {
                    bestemmingVerplicht = true;
                }
                //14-07-10,Herman: MUTATION.CodeMutation op sal.SalKind keuze gezet ipv Landwaardes; 
                UBN eigenubn = lMstb.GetubnById(pBedrijf.UBNid);
                THIRD eigenthird = lMstb.GetThirdByThirId(eigenubn.ThrID);
                COUNTRY eigenland = new COUNTRY();
                if (eigenthird.ThrCountry.Trim() != "")
                {
                    eigenland = lMstb.GetCountryByLandid(int.Parse(eigenthird.ThrCountry));
                }
                if (sal.SalKind != 1)
                {
                    if (eigenland.LandId == 0 || eigenland.LandAfk2.ToLower() == "nl")
                    {
                        if (bestemmingVerplicht)
                        {
                            //if (sal.SalDestination == 0) { return "Bestemming met UBNnummer is verplicht"; }
                            if (mov.MovThird_UBNid == 0) { return "Bestemming met UBNnummer is verplicht"; }
                            if (sal.SalDestination > 0)
                            {
                                THIRD bestemmingcheck = lMstb.GetThirdByThirId(sal.SalDestination);
                                if (bestemmingcheck.ThrCountry.Trim() != "")
                                {
                                    COUNTRY c = lMstb.GetCountryByLandid(int.Parse(bestemmingcheck.ThrCountry));
                                    if (c.LandAfk2.ToLower() != "nl")
                                    {
                                        return "U heeft een bestemming gekozen buiten nederland";
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (eigenland.LandId == 0 || eigenland.LandAfk2.ToLower() == "nl")
                    {
                        if (bestemmingVerplicht)
                        {
                            THIRD bestemmingcheck2 = lMstb.GetThirdByThirId(sal.SalDestination);
                            if (bestemmingcheck2.ThrCountry.Trim() != "")
                            {
                                COUNTRY c = lMstb.GetCountryByLandid(int.Parse(bestemmingcheck2.ThrCountry));
                                if (c.LandId == 0 || c.LandAfk2.ToLower() == "nl")
                                {
                                    return "U heeft een export binnen nederland";
                                }
                            }
                        }
                    }
                }

                if (mov.MovDate.CompareTo(DateTime.MinValue) == 0) { return "Datum is verplicht"; }
                if (sal.SalDestination > 0 || mov.MovThird_UBNid > 0)
                {
                    sDestination sdes = new sDestination();
                    sdes.ThirdId = sal.SalDestination;
                    sdes.UbnId = mov.MovThird_UBNid;
                    string ret = Checker.checkAfvoerDestination(pUr, sdes);
                    if (ret != "")
                    { return ret; }
                }
                string retvalue = "";
                string melden = "";
                string magopslaan = "";

                FARMCONFIG FCIRafvoer = lMstb.getFarmConfig(pBedrijf.FarmId, "IRafvoer", "True");

                if (mov.MovId > 0)
                {
                    magopslaan = AllowAfvoerUpdate(pUr, mov.MovId, mov.AniId, mov.MovDate, pBedrijf);
                }
                else
                {
                    magopslaan = AllowAfvoer(pUr, mov.AniId, mov.MovDate, pBedrijf);
                }

                if (magopslaan == "")
                {
                    //thirdId van de bestemming
                    UBN ubnBestemming = lMstb.GetubnById(mov.MovThird_UBNid);
                    THIRD thirdBestemming = lMstb.GetThirdByThirId(ubnBestemming.ThrID);
                    int thrId_bestemming = thirdBestemming.ThrId;
                    COUNTRY land = new COUNTRY();
                    if (thirdBestemming.ThrCountry.Trim() != "")
                    {
                        try
                        {
                            land = lMstb.GetCountryByLandid(int.Parse(thirdBestemming.ThrCountry));
                        }
                        catch (Exception exc) { unLogger.WriteInfo("ubnBestemming afvoer:" + ubnBestemming.Bedrijfsnaam + " geen landcode"); }
                    }
                    ANIMAL ani = lMstb.GetAnimalById(mov.AniId);
                    List<ANIMALCATEGORY> aniscats = lMstb.GetAnimalCategoriesByIdandUbnid(mov.AniId, pBedrijf.UBNid).ToList();
                    ANIMALCATEGORY aniscat = new ANIMALCATEGORY();
                    if (aniscats.Count() == 0)
                    {
                        aniscat = new ANIMALCATEGORY();
                        aniscat.AniId = mov.AniId;
                        aniscat.FarmId = pBedrijf.FarmId;
                        aniscat.UbnId = pBedrijf.UBNid;
                        aniscat.AniMinasCategory = ani.AniMinasCategory;
                    }
                    else
                    {
                        aniscat = aniscats.ElementAt(0);
                    }

                    //vul MOVEMENT en SALE waarden                
                    mov.UbnId = pBedrijf.UBNid;// fURT.UBNId;
                    mov.Progid = pBedrijf.ProgId;// fURT.ProgId;


                    if (mov.MovId > 0)
                    {
                        mov.MovOrder = getMovementOrder(pUr, mov.MovId);

                        if (lMstb.UpdateMovement(mov))
                        {
                            sal.MovId = mov.MovId;
                            if (!lMstb.SaveSale(sal))
                            {
                                retvalue = "Fout updating Movement/Sale: " + mov.MovId.ToString();
                            }
                        }
                        else
                        {
                            retvalue = "Fout updating Movement: " + mov.MovId.ToString();
                        }

                        if (FCIRafvoer.FValue.ToLower() == "true" && retvalue == "")
                        {
                            MUTATION mutrepairs = lMstb.GetMutationByMovId(mov.MovId);// GetMutationsByUbn(pBedrijf.UBNid);
                            if (mutrepairs.Internalnr > 0)
                            {


                                //hoeft niet te controleren of dat mag
                                //
                                mutrepairs.FarmNumberTo = ubnBestemming.Bedrijfsnummer;
                                mutrepairs.CodeMutation = 4;//afvoer
                                if (sal.SalKind == 1)
                                {
                                    mutrepairs.CodeMutation = 9;//export
                                }
                                else if (sal.SalKind == 2 || sal.SalKind == 3)
                                {
                                    mutrepairs.CodeMutation = 10;//slacht
                                    mutrepairs.IDRLossDate = mov.MovDate;
                                    mutrepairs.LossDate = mutrepairs.IDRLossDate;
                                }
                                if (mutrepairs.IDRBirthDate == DateTime.MinValue)
                                {
                                    mutrepairs.IDRBirthDate = ani.AniBirthDate;
                                }
                                mutrepairs.MutationDate = mov.MovDate;
                                mutrepairs.MutationTime = mov.MovTime;
                                TRANSPRT tr = lMstb.GetTransprt(sal.SalTransportID);
                                mutrepairs.LicensePlate = tr.LicensePlate;
                                THIRD afvoertransporteur = lMstb.GetThirdByThirId(sal.SalTransporter);
                                mutrepairs.Vervoersnr = afvoertransporteur.ThrVATNumber;

                                mutrepairs.Purchaser = thirdBestemming.ThrVATNumber;

                                lMstb.UpdateMutation(mutrepairs);



                            }
                            else
                            {
                                MUTALOG lMutLog = lMstb.GetMutaLogByMovId(mov.MovId);
                                if (lMutLog.Internalnr > 0)
                                {
                                    if ((lMutLog.Returnresult == 1 || lMutLog.Returnresult == 3) && lMutLog.MutationDate.Date != mov.MovDate.Date)
                                    {
                                        MUTATION lMut = Facade.GetInstance().getMeldingen().ConverttoMutation(lMutLog);

                                        lMut.MutationDate = mov.MovDate;
                                        lMut.MutationTime = mov.MovTime;
                                        lMut.Returnresult = 96;
                                        lMut.CodeMutation += 200;//herstellen
                                        lMstb.SaveMutation(lMut);
                                    }
                                }
                                else
                                {

                                    ANIMAL animother = lMstb.GetAnimalById(ani.AniIdMother);
                                    ANIMAL anifather = lMstb.GetAnimalById(ani.AniIdFather);
                                    //Event_functions.saveNewMutation(pUr, 4, pBedrijf, "", "", new EVENT(), mov, ani, animother, anifather, 96);
                                }
                            }
                        }
                        else
                        {
                            List<MUTALOG> mutlogrepairs = lMstb.GetMutaLogsByUbn(pBedrijf.UBNid);
                            if (mutlogrepairs.Count > 0)
                            {
                                foreach (MUTALOG muttela in mutlogrepairs)
                                {
                                    if (muttela.MovId == mov.MovId)
                                    {

                                        if (sal.SalDestination != 0)
                                        {
                                            UBN ubdest = lMstb.getUBNByThirdID(sal.SalDestination);
                                            muttela.FarmNumberTo = ubdest.Bedrijfsnummer;
                                        }
                                        else { muttela.FarmNumberTo = ""; }
                                        muttela.CodeMutation = 4;//afvoer
                                        if (sal.SalKind == 1)
                                        {
                                            muttela.CodeMutation = 9;//export
                                        }
                                        else if (sal.SalKind == 2 || sal.SalKind == 3)
                                        {
                                            muttela.CodeMutation = 10;//slacht
                                        }
                                        muttela.MutationDate = mov.MovDate;
                                        muttela.MutationTime = mov.MovTime;
                                        TRANSPRT tr = lMstb.GetTransprt(sal.SalTransportID);
                                        muttela.LicensePlate = tr.LicensePlate;
                                        THIRD afvoertransporteur = lMstb.GetThirdByThirId(sal.SalTransporter);
                                        muttela.Vervoersnr = afvoertransporteur.ThrVATNumber;

                                        muttela.Purchaser = thirdBestemming.ThrVATNumber;

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        mov.MovOrder = getNewMovementOrder(pUr, mov.MovKind, mov.MovDate, mov.AniId, pBedrijf.UBNid);
                        if (mov.MovId == 0)
                        {
                            mov.happened_at_FarmID = pBedrijf.FarmId;
                        }
                        if (lMstb.SaveMovement(mov))
                        {
                            sal.MovId = mov.MovId;
                            if (!lMstb.SaveSale(sal))
                            {
                                retvalue = "Fout inserting new  movement/sale: Animal:" + mov.AniId.ToString() + " Movid:" + sal.MovId.ToString();
                            }
                            else
                            {
                                aniscat.Anicategory = 4;
                                melden = saveAfvoerMutation(pUr, pBedrijf, ani, mov, sal);
                            }

                        }
                        else
                        {
                            retvalue = "Error inserting new  movement: Animal:" + mov.AniId.ToString() + " kind:" + mov.MovKind.ToString();
                        }
                    }

                    aniscat.Anicategory = 4;
                    SetSaveAnimalCategory(pUr, pBedrijf, aniscat, pChangedBy, pSourceID);
                    lMstb.UpdateANIMAL(pThrId, ani);
                    returnTransmitters(pUr, pBedrijf, mov.AniId);
                    if (retvalue == "")
                    { retvalue = melden; }
                }
                else
                {
                    retvalue = magopslaan;
                }

                return retvalue;
            }
            catch(Exception exc)
            {
                unLogger.WriteError(exc.ToString());
                return exc.Message;
            }
        }

        public static void returnTransmitters(UserRightsToken pUr, BEDRIJF pBedrijf, int pAniID)
        {
            //Het transmitternummer moet weer in de voorraad gezet worden als een dier van het bedrijf wordt afgevoerd.
            if (pBedrijf.FarmId > 0 && pAniID > 0)
            {
                DBSelectQueries ds = new DBSelectQueries(pUr);
                AFSavetoDB lMstb = getMysqlDb(pUr);
                ANIMAL a = lMstb.GetAnimalById(pAniID);
                UBN u = lMstb.GetubnById(pBedrijf.UBNid);
                ANIMALCATEGORY ac = lMstb.GetAnimalCategoryByIdandFarmid(pAniID, pBedrijf.FarmId);
                if (ac.Anicategory > 3)
                {
                    List<TRANSMIT> trans = ds.Responders.GetTransmitsByFarmId(pBedrijf.FarmId, pBedrijf.UBNid);
                    var respondersanimal = (from n in trans where n.AniId == pAniID select n).ToList();


                    //List<TRANSMSTOCK> Allstock = ds.Responders.GetTransmitterVoorraad(pBedrijf.UBNid);

                    //List<TRANSMSTOCK> stockdeleted = ds.Responders.GetTransmitterVoorraad(-pBedrijf.UBNid);//Let op de -
                    //controleren of die niet negatief, is gezet of er al in staat
                    if (respondersanimal.Count() == 0)
                    {
                        unLogger.WriteTrace($"{nameof(MovFunc)}.{nameof(returnTransmitters)} Dier: '{a.AniAlternateNumber}' - niets verwijderd, geen responders gevonden. ");
                        
                    }
                    foreach (TRANSMIT tr in respondersanimal)
                    {
                        TRANSMIT mbg = tr;//ivm delete in een loop
                       
                        mbg.UbnID = pBedrijf.UBNid;
                        mbg.farmid = pBedrijf.FarmId;
                        mbg.FarmNumber = u.Bedrijfsnummer;
                        if (lMstb.DeleteTransmit(mbg))//zet hem automatisch weer in de voorraad
                        {

                        }
                        else
                        {
                            unLogger.WriteError($"{nameof(MovFunc)}.{nameof(returnTransmitters)} Dier: '{a.AniAlternateNumber}' - niet verwijderd: fout DeleteTransmit ");
                        }
                    }
                }
                else 
                {
                    unLogger.WriteWarn($"{nameof(MovFunc)}.{nameof(returnTransmitters)} Dier: '{a.AniAlternateNumber}' - niet verwijderd: dier nog aanwezig");
                }
            }
        }

        public static List<string> RendacUbns(UserRightsToken pUr)
        {
            /*
             
             */
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<string> ubns = new List<string>();

            StringBuilder bld = new StringBuilder();
            bld.AppendLine(" SELECT DISTINCT(u.Bedrijfsnummer) FROM agrofactuur.UBN u where u.Bedrijfsnaam LIKE '%rendac%' ");
            bld.AppendLine(" UNION ");
            bld.AppendLine(" SELECT DISTINCT(u.Bedrijfsnummer) FROM agrofactuur.THIRD t ");
            bld.AppendLine(" JOIN  agrofactuur.UBN u  ON u.ThrId=t.ThrId ");
            bld.AppendLine(" WHERE t.ThrCompanyName LIKE '%rendac%' OR t.ThrSecondName LIKE '%rendac%' OR u.Bedrijfsnaam LIKE '%rendac%' ");
            DataSet sd = new DataSet();
            DataTable tbl = lMstb.GetDataBase().QueryData(pUr, sd, bld, "", MissingSchemaAction.Add);
            if (tbl.Rows.Count > 0)
            {
                foreach (DataRow rw in tbl.Rows)
                {
                    if (rw[0] != DBNull.Value && rw[0].ToString() != "")
                    {
                        ubns.Add(rw[0].ToString());
                    }
                }
            }
            if (!ubns.Contains("2299077"))
            {
                ubns.Add("2299077");
            }
            return ubns;
        }

        public static string saveAfvoerMutation(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, MOVEMENT pMov, SALE pSale)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            SOAPLOG sl = new SOAPLOG();
            sl.Changed_By = pMov.Changed_By;
            sl.SourceID = pMov.SourceID;
            sl.Date = DateTime.Now.Date;
            sl.Kind = 0;
            sl.Omschrijving = "Save AfvoerMutation";
            sl.Status = "G";
            sl.Time = DateTime.Now;
            
            if (pBedrijf == null || pAnimal == null || pMov == null || pSale == null)
            {
                string watisnull = pUr == null ? "pUr == null" : pBedrijf == null ? "pBedrijf == null" : pAnimal == null ? "pAnimal == null" : pMov == null ? "pMov == null" : pSale == null ? "pSale == null" : "";
                unLogger.WriteError($@"{nameof(MovFunc)}.saveAfvoerMutation parameters:   {watisnull} ");
                sl.Status = "F";
                sl.Omschrijving += " " + watisnull;
                lMstb.WriteSoapError(sl);
                return "AfvoerMelding:" + watisnull;
            }
            try
            {
              
                UBN ubn = lMstb.GetubnById(pMov.UbnId);
                sl.FarmNumber = ubn.Bedrijfsnummer;
                sl.Lifenumber = pAnimal.AniAlternateNumber;
                FARMCONFIG FCIRafvoer = lMstb.getFarmConfig(pBedrijf.FarmId, "IRafvoer", "True");
                string melden = "";


                UBN ubnBestemming = new UBN();
                THIRD thirdBestemming = new THIRD();
                if (pMov.MovThird_UBNid > 0)
                {
                    ubnBestemming = lMstb.GetubnById(pMov.MovThird_UBNid);
                    thirdBestemming = lMstb.GetThirdByThirId(ubnBestemming.ThrID);
                }
                
                if (ubnBestemming.UBNid == 0 || thirdBestemming.ThrId == 0)
                {
                    thirdBestemming = lMstb.GetThirdByThirId(pSale.SalDestination);
                    if (ubnBestemming.UBNid == 0 && thirdBestemming.ThrId > 0)
                    {
                       ubnBestemming = lMstb.getUBNsByThirdID(thirdBestemming.ThrId).FirstOrDefault();
                    }
                }
                if (ubnBestemming == null || ubnBestemming.UBNid == 0)
                {
                    ubnBestemming = new UBN();
                    sl.Status = "F";
                    sl.Omschrijving += " " + $@"ubnBestemming bedrijfsnummer niet bekend: Thrid:{thirdBestemming.ThrId} Naam:{thirdBestemming.ThrSecondName}";
                    lMstb.WriteSoapError(sl);
                    sl.Status = "G";
                    unLogger.WriteInfo($@"ubnBestemming bedrijfsnummer niet bekend: Thrid:{thirdBestemming.ThrId} Naam:{thirdBestemming.ThrSecondName}");
                }

                List<ANIMALCATEGORY> aniscat = lMstb.GetAnimalCategoriesByIdandUbnid(pMov.AniId, pMov.UbnId).ToList();

               
                bool mutatio = true;
                MUTATION mut = new MUTATION();
                MUTALOG mutlog = new MUTALOG();
                if (pMov.MovId > 0)
                {
                    mut = lMstb.GetMutationByMovId(pMov.MovId);
                }
                if (mut.Internalnr == 0)
                {
                    
                    if (pMov.MovId > 0)
                    {
                        mutlog = lMstb.GetMutaLogByMovId(pMov.MovId);
                        if (mutlog.Internalnr > 0)
                        {
                            mutatio = false;
                        }
                    }
                }
                string up = lMstb.getUbnProperty(ubn.UBNid, "SanitelMelden");
                if ((FCIRafvoer.FValue.ToLower() == "true" || up.ToLower() == "true") && mutatio==true)
                {
                   
                    mut.Changed_By = pMov.Changed_By;
                    mut.SourceID = pMov.SourceID;
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        mut.Lifenumber = getGoodAlternateNumber(pAnimal.AniAlternateNumber);
                    }
                    else
                    {

                        mut.Lifenumber = pAnimal.AniLifeNumber;
                    }
                    mut.IDRBirthDate = pAnimal.AniBirthDate;
                    if (pMov.MovKind == 3)
                    {
                        mut.IDRLossDate = pMov.MovDate;
                        mut.LossDate = mut.IDRLossDate;
                    }
                    else if (pMov.MovKind == 2)
                    {

                        if (pSale.SalKind == 2 || pSale.SalKind == 3)
                        {
                            mut.IDRLossDate = pMov.MovDate;
                            mut.LossDate = mut.IDRLossDate;
                        }
                    }
                    mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, pAnimal.AniAlternateNumber);
                    mut.Worknumber = aniscat != null && aniscat.Count() > 0 ? aniscat.FirstOrDefault().AniWorknumber : "";
                    mut.Name = string.IsNullOrEmpty(pAnimal.AniName) ? "" : pAnimal.AniName;
            
                   
                    mut.CodeMutation = 4;//afvoer
                    if (pSale.SalKind == 1)
                    {
                        mut.CodeMutation = 9;//export
                    }
                    else if (pSale.SalKind == 2 || pSale.SalKind == 3)
                    {
                        mut.CodeMutation = 10;//slacht
                    }
                      
                  
                    unLogger.WriteInfo($@" mut.CodeMutation {mut.CodeMutation}");
                    sl.Lifenumber = pAnimal.AniAlternateNumber;
                   
                    try
                    {
                        mut.Sex = pAnimal.AniSex;
                        mut.CountryCodeBirth = string.IsNullOrEmpty(pAnimal.AniCountryCodeBirth) ? "" : pAnimal.AniCountryCodeBirth;
                        mut.Haircolor = 0;// pAnimal.Anihaircolor;
                        mut.AniHaircolor_Memo = string.IsNullOrEmpty(pAnimal.AniHaircolor_Memo) ? "" : pAnimal.AniHaircolor_Memo;
                        mut.MovId = pMov.MovId;
                        mut.UbnId = pMov.UbnId;
                        mut.FarmNumberTo = string.IsNullOrEmpty(ubnBestemming.Bedrijfsnummer) ? "" : ubnBestemming.Bedrijfsnummer;
                        mut.Nling = pAnimal.AniNling;
                    }
                    catch (Exception exx)
                    {
                        sl.Status = "F";
                        sl.Omschrijving += " " + exx.Message;
                        lMstb.WriteSoapError(sl);
                        sl.Status = "G";
                        unLogger.WriteError("1:" + exx.ToString());
                    }
                    try
                    {
                        mut.FarmNumberFrom = string.IsNullOrEmpty(ubn.Bedrijfsnummer) ? "" : ubn.Bedrijfsnummer;
                        mut.Farmnumber = string.IsNullOrEmpty(ubn.Bedrijfsnummer) ? "" : ubn.Bedrijfsnummer;
                        TRANSPRT tr = lMstb.GetTransprt(pSale.SalTransportID);
                        mut.LicensePlate = string.IsNullOrEmpty(tr.LicensePlate) ? "" : tr.LicensePlate;
                        THIRD afvoertransporteur = lMstb.GetThirdByThirId(pSale.SalTransporter);
                        mut.Vervoersnr = afvoertransporteur.ThrVATNumber;
                        unLogger.WriteInfo($@"Mutation Purchaser thirdBestemming.ThrId: {thirdBestemming.ThrId} thirdBestemming.ThrVATNumber: {thirdBestemming.ThrVATNumber}");
                        mut.Purchaser = thirdBestemming.ThrVATNumber;
                    }
                    catch (Exception exx)
                    {
                        sl.Status = "F";
                        sl.Omschrijving += " " + exx.Message;
                        lMstb.WriteSoapError(sl);
                        sl.Status = "G";
                        unLogger.WriteError("2:" + exx.ToString());
                    }
                    if (pBedrijf.ProgId == 25)
                    {
                        mut.SendTo = 35;
                    }
                    else
                    {
                        RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                        string defIenRaction = r.getdefIenRaction(pUr, ubn.UBNid, pBedrijf.ProgId, pBedrijf.Programid);
                        FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedrijf.FarmId, "VerstuurIenR", defIenRaction);

                        mut.SendTo = IenRCom.ValueAsInteger();
                    }

                    if (pAnimal.AniIdMother != 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            mut.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                        }
                        else
                        {
                            mut.LifenumberMother = moeder.AniLifeNumber;
                        }
                    }
              
                    mut.MutationDate = pMov.MovDate;
                    mut.MutationTime = pMov.MovTime;
                    if (pAnimal.AniIdFather != 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            mut.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                        }
                        else
                        {

                            mut.LifeNumberFather = vader.AniLifeNumber;
                        }
                    }
                    mut.AlternateLifeNumber = pAnimal.AniAlternateNumber;

                    lMstb.SaveMutation(mut);
                    unLogger.WriteInfo($@"Saveafvoermutation {mut.Internalnr}");
                    lMstb.WriteSoapError(sl);
                    melden = "melden";
                }
                else
                {
                    sl.Omschrijving += " MUTALOG: ";
                   
                    mutlog.Changed_By = pMov.Changed_By;
                    mutlog.SourceID = pMov.SourceID;
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        mutlog.Lifenumber = getGoodAlternateNumber(pAnimal.AniAlternateNumber);
                    }
                    else
                    {

                        mutlog.Lifenumber = pAnimal.AniLifeNumber;
                    }
                    mutlog.Worknumber = aniscat != null && aniscat.Count() > 0 ? aniscat.FirstOrDefault().AniWorknumber : "";
                    mutlog.Name = pAnimal.AniName;
                    mutlog.CodeMutation = 4;//afvoer
                    if (pSale.SalKind == 1)
                    {
                        mutlog.CodeMutation = 9;//export
                    }
                    else if (pSale.SalKind == 2 || pSale.SalKind == 3)
                    {
                        mutlog.CodeMutation = 10;//slacht
                    }
                    mutlog.Sex = pAnimal.AniSex;
                    mutlog.Haircolor = pAnimal.Anihaircolor;
                    mutlog.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
                    mutlog.MovId = pMov.MovId;
                    mutlog.UbnId = int.Parse(pMov.UbnId.ToString());
                    mutlog.IDRBirthDate = pAnimal.AniBirthDate;
                    if (pMov.MovKind == 3)
                    { mutlog.IDRLossDate = pMov.MovDate; }


                    mutlog.FarmNumberTo = ubnBestemming.Bedrijfsnummer;
                    mutlog.Nling = pAnimal.AniNling;
                    UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
                    mutlog.FarmNumberFrom = ubu.Bedrijfsnummer;
                    mutlog.Farmnumber = ubu.Bedrijfsnummer;
                    TRANSPRT tr = lMstb.GetTransprt(pSale.SalTransportID);
                    mutlog.LicensePlate = tr.LicensePlate;
                    THIRD afvoertransporteur = lMstb.GetThirdByThirId(pSale.SalTransporter);
                    mutlog.Vervoersnr = afvoertransporteur.ThrVATNumber;

                    mutlog.Purchaser = thirdBestemming.ThrVATNumber;
                    if (pAnimal.AniIdMother != 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            mutlog.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                        }
                        else
                        {
                            mutlog.LifenumberMother = moeder.AniLifeNumber;
                        }
                    }
                    mutlog.MutationDate = pMov.MovDate;
                    mutlog.MutationTime = pMov.MovTime;
                    if (pAnimal.AniIdFather != 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            mutlog.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                        }
                        else
                        {

                            mutlog.LifeNumberFather = vader.AniLifeNumber;
                        }
                    }
                    mutlog.AlternateLifeNumber = pAnimal.AniAlternateNumber;
                    unLogger.WriteInfo($@"Saveafvoermutalog {mutlog.Internalnr}");
                    lMstb.InsertMutLog(mutlog);
                    lMstb.WriteSoapError(sl);
                }
                return melden;
            }
            catch (Exception exc)
            {
                unLogger.WriteError(exc.ToString());
                sl.Status = "F";
                sl.Omschrijving += " " + exc.Message;
                lMstb.WriteSoapError(sl);
                return exc.Message;
            }
        }

        public static void SetSaveAnimalCategory(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMALCATEGORY pAniCat, int changedby, int sourceid, bool forceUpdate = false)
        {
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pUr);
            ANIMAL lAni = lMstb.GetAnimalById(pAniCat.AniId);
            UBN lUbn = lMstb.GetubnById(pAniCat.UbnId);
            THIRD pThird = lMstb.GetThirdByThirId(lUbn.ThrID);
            SetSaveAnimalCategory(pUr, pBedrijf, lUbn, pThird, pAniCat, lAni, changedby, sourceid, true);
        }

        /// <summary>
        /// </summary>
        /// <param name="pUr"></param>
        /// <param name="pBedrijf"></param>
        /// <param name="pUbn"></param>
        /// <param name="pThird"></param>
        /// <param name="pAniCat"></param>
        /// <param name="lAni"></param>
        /// <param name="forceUpdate"></param>
        public static void SetSaveAnimalCategory(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThird, ANIMALCATEGORY pAniCat, ANIMAL lAni, int changedby, int sourceid, bool forceUpdate = false)
        {
            /*
                 1 = vrouwelijk aanwezig dier
                 2 = fokstier
                 3 = meststier
                 4 = afgevoerde dieren
                 5 = nooit aanwezig geweest
             * 
             * MovKind:
             *  1	Aanvoer
                2	Afvoer
                3	Dood
                4	Inscharen
                5	Einde inscharen
                6	Uitscharen
                7	Einde uitscharen
                8	Lokatie
             */


            bool isChanged = false;

            pAniCat.Changed_By = changedby;
            pAniCat.SourceID = sourceid;
            if (pAniCat.Ani_Mede_Eigenaar == 0)
            {
                DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pUr);

                pAniCat.FarmId = pBedrijf.FarmId;
                bool isFokstier = false;
                BULLUBN bu = lMstb.GetBullUbn(pBedrijf.FarmId, pAniCat.AniId);
                if (bu.BullId != 0)
                {
                    isFokstier = true;
                }
                List<MOVEMENT> movs = lMstb.GetMovementsByUbn(pAniCat.AniId, pUbn.UBNid);
                List<MOVEMENT> movs2 = lMstb.GetMovementsByUbn(pAniCat.AniId, 0);

                if (pBedrijf.ProgId != 25)
                {
                    movs = movs.Concat(movs2).ToList();
                }

                var check = from n in movs
                    where n.MovKind < 8
                    select n;
                movs = check.ToList();
                // Luc 11-12-14 dieren die dood zijn naar afwezig zetten
                var movs3 = movs.Where(m => m.MovKind == 3);
                if (movs3.Count() == 0)
                {
                    //Luc 10-12-15 bij een doodmelding van een ander bedrijf niet direct op afwezig zetten zodat de afvoermelding via rsrepro opgehaald wordt.
                    movs3 = lMstb.GetMovementsByAniIdMovkind(pAniCat.AniId, 3).Where(m => m.MovDate.Date < DateTime.Today && m.MovMutationDate.Date < DateTime.Today.AddDays(-5));
                }


                if (movs3.Count() > 0 || (lAni.AniBirthDate > (new DateTime(1901, 1, 1)) && lAni.AniBirthDate < (new DateTime(1995, 6, 1))))
                {
                    pAniCat.Anicategory = 4;
                }
                else if (movs.Count > 0)
                {

                    DateTime mindate = DateTime.MinValue;


                    MOVEMENT lMovTemp = new MOVEMENT();

                    var check2 = from n in movs

                        orderby n.MovDate descending, n.MovTime descending, n.MovId descending
                        select n;

                    List<MOVEMENT> lOrdered = check2.ToList();
                    if (lOrdered.Count() > 0)
                    {
                        controleerTopMovs(pUr, lAni, pBedrijf, pUbn, pThird, lOrdered, out lMovTemp);



                        switch (lMovTemp.MovKind)
                        {
                            case 1:
                                setcat123(ref pAniCat, lAni.AniSex, isFokstier);
                                break;
                            case 2:
                                pAniCat.Anicategory = 4;
                                break;
                            case 3:
                                pAniCat.Anicategory = 4;
                                break;
                            case 4:
                                setcat123(ref pAniCat, lAni.AniSex, isFokstier);
                                break;
                            case 5:
                                pAniCat.Anicategory = 4;
                                break;
                            case 6:
                                pAniCat.Anicategory = 4;
                                break;
                            case 7:
                                setcat123(ref pAniCat, lAni.AniSex, isFokstier);
                                break;
                            case 8:
                                setcat123(ref pAniCat, lAni.AniSex, isFokstier);
                                break;
                        }
                    }

                }
                else
                {
                    //er zijn geen aan of afvoeren bekend als het dier er wel geboren is dan aanwezig zetten
                    unLogger.WriteDebug(String.Format("Set animalcategory: AniId: {0} {1} van FarmId: {2} heeft geen Movements.", pAniCat.AniId, lAni.AniAlternateNumber, pAniCat.FarmId));
                    List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen = new List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats>();

                    if (rdDierGeborenOpBedrijf(pUr, lAni, pBedrijf, pUbn, pThird, ref Verblijfplaatsen))
                    {

                        unLogger.WriteDebug(String.Format("Set animalcategory: AniId: {0} {1} van FarmId: {2} van 4 naar 123", pAniCat.AniId, lAni.AniAlternateNumber, pAniCat.FarmId));
                        setcat123(ref pAniCat, lAni.AniSex, isFokstier);

                    }
                    else
                    {
                        string lnr = lAni.AniLifeNumber;
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            lnr = lAni.AniAlternateNumber;
                        }

                        if (Verblijfplaatsen.Where(verblpl => verblpl.UBN == pUbn.Bedrijfsnummer).Count() > 0)
                        {
                            pAniCat.Anicategory = 4;
                        }
                        else
                        {
                            unLogger.WriteDebug(String.Format("Set animalcategory: AniId: {0} {1} van FarmId: {2} van {3} naar 5", pAniCat.AniId, lAni.AniAlternateNumber, pAniCat.FarmId, pAniCat.Anicategory));
                            pAniCat.Anicategory = 5;
                        }

                    }
                }




                if (pAniCat.Anicategory == 0)
                {
                    setcat123(ref pAniCat, lAni.AniSex, false);
                    isChanged = true;
                }

                if (forceUpdate || isChanged)
                {
                    lMstb.SaveAnimalCategory(pAniCat);
                    unLogger.WriteInfo("");
                }
            }
        }
        
        /// <summary>
        /// ML: Wat doet deze functie??????
        /// </summary>
        /// <param name="pUr"></param>
        /// <param name="pAnimal"></param>
        /// <param name="pFarm"></param>
        /// <param name="pUbn"></param>
        /// <param name="pThird"></param>
        /// <param name="lOrderedDesc"></param>
        /// <param name="lMovTemp"></param>
        public static void controleerTopMovs(UserRightsToken pUr, ANIMAL pAnimal, BEDRIJF pFarm, UBN pUbn, THIRD pThird, List<MOVEMENT> lOrderedDesc, out MOVEMENT lMovTemp)
        {

            lMovTemp = lOrderedDesc.ElementAt(0);
            DateTime checkDate = lMovTemp.MovDate;
            int lMovId = lMovTemp.MovId;
            int[] aanvoer = { 1, 4, 7, 8 };
            int[] afvoer = { 2, 3, 5, 6 };
            int[] checkarr = aanvoer;
            if (aanvoer.Contains(lMovTemp.MovKind))
            {
                checkarr = afvoer;
            }
            var check = from n in lOrderedDesc
                where n.MovDate.Date == checkDate.Date
                      && n.MovId != lMovId
                      && checkarr.Contains(n.MovKind)
                select n;
            if (check.Count() > 0)
            {
                // dan is er op dezelfde (laatste)dag een andere afvoer/aanvoer gepleegd
                // en kan de sorteervolgorde verkeerd zijn
                // hierdoor kan de AniCategory verkeerd komen te staan
                // We doen hier een poging de juiste logische volgorde te bepalen
                if (check.ElementAt(0).MovDate.Date == lMovTemp.MovDate.Date)
                {
                    List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen = new List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats>();
                    bool geborenOpBedrijf = rdDierGeborenOpBedrijf(pUr, pAnimal, pFarm, pUbn, pThird, ref Verblijfplaatsen);

                    orderMovements(pUr, geborenOpBedrijf, checkDate, lOrderedDesc, out lMovTemp);
                }
            }
        }

        public static void orderMovements(UserRightsToken pUr, bool geborenOpBedrijf, DateTime pcheckDate, List<MOVEMENT> lOrderedDesc, out MOVEMENT lMovTemp)
        {
            lMovTemp = lOrderedDesc.ElementAt(0);
            List<MOVEMENT> lOrdered = (from m in lOrderedDesc
                                       where m.MovKind != 8
                                       orderby m.MovDate, m.MovTime, m.MovId
                                       select m).ToList();
            /* bijvoorbeeld
             *          movkind  
             12-12-2012 2
             12-12-2012 1
             01-10-2012 1
             01-09-2012 2
             geboren op bedrijf
             */
            int lTotal = lOrdered.Count();

            int[] aanvoer = { 1, 4, 7, 8 };
            int[] afvoer = { 2, 3, 5, 6 };


            DateTime currentDate = DateTime.MinValue;
            string aanofafwezig = "afwezig";
            if (geborenOpBedrijf) { aanofafwezig = "aanwezig"; }
            for (int i = 0; i < lTotal; i++)
            {
                currentDate = lOrdered[i].MovDate;
                if (aanofafwezig == "aanwezig")
                {
                    if (!afvoer.Contains(lOrdered[i].MovKind))
                    {
                        for (int j = i + 1; j < lTotal; j++)
                        {
                            if (lOrdered[j].MovDate.Date == currentDate.Date)
                            {
                                if (afvoer.Contains(lOrdered[j].MovKind))
                                {
                                    aanofafwezig = "afwezig";
                                    swap(pUr, i, j, ref lOrdered);

                                }
                            }
                        }
                    }
                    else
                    {
                        aanofafwezig = "afwezig";
                    }
                }
                else
                {
                    if (!aanvoer.Contains(lOrdered[i].MovKind))
                    {
                        for (int j = i + 1; j < lTotal; j++)
                        {
                            if (lOrdered[j].MovDate.Date == currentDate.Date)
                            {
                                if (aanvoer.Contains(lOrdered[j].MovKind))
                                {
                                    aanofafwezig = "aanwezig";
                                    swap(pUr, i, j, ref lOrdered);

                                }
                            }
                        }
                    }
                    else
                    {
                        aanofafwezig = "aanwezig";
                    }
                }

            }
            if (aanofafwezig == "aanwezig")
            {
                lMovTemp.MovKind = 1;
            }
            else
            {
                lMovTemp.MovKind = 2;
            }
        }

        private static void swap(UserRightsToken pUr, int i, int j, ref List<MOVEMENT> lOrdered)
        {
            MOVEMENT temp = lOrdered[i];
            lOrdered[i] = lOrdered[j];
            lOrdered[j] = temp;
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            if (i < j)
            {
                if (lOrdered[j].MovTime < lOrdered[i].MovTime)
                {
                    try
                    {
                        //DateTime dTemp = lOrdered[j].MovTime;
                        //lOrdered[j].MovTime = lOrdered[i].MovTime;
                        //lOrdered[i].MovTime = dTemp;
                        //lMstb.UpdateMovement(lOrdered[i]);
                        //lMstb.UpdateMovement(lOrdered[j]);
                    }
                    catch { }
                }
            }
            else if (i > j)
            {
                if (lOrdered[i].MovTime < lOrdered[j].MovTime)
                {
                    try
                    {
                        //DateTime dTemp = lOrdered[j].MovTime;
                        //lOrdered[j].MovTime = lOrdered[i].MovTime;
                        //lOrdered[i].MovTime = dTemp;
                        //lMstb.UpdateMovement(lOrdered[i]);
                        //lMstb.UpdateMovement(lOrdered[j]);
                    }
                    catch { }
                }
            }
        }

        public static bool rdDierGeborenOpBedrijf(UserRightsToken pUr, ANIMAL pAnimal, BEDRIJF pFarm, UBN pUbn, THIRD pThird, ref List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen)
        {
            if (pAnimal.AniId > 0 && pFarm.FarmId > 0)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);

                BIRTH b = new BIRTH();
                StringBuilder bld = new StringBuilder();
                bld.Append("SELECT EVENT.UBNId,  EVENT.happened_at_FarmID FROM BIRTH LEFT JOIN EVENT ON EVENT.EventId=BIRTH.EventId WHERE BIRTH.CalfId=" + pAnimal.AniId.ToString() + " AND BIRTH.EventId>0 ");
                DataSet ds = new DataSet();
                DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), ds, bld, "rdDierGeborenOpBedrijf", MissingSchemaAction.Add);
                // luc 17-11-2014 happened_at_FarmID als extra check toegevoegd, geboortes met ubnid == 0 via de fokker check laten lopen
                if (tbl.Rows.Count > 0 && tbl.Rows[0][0] != DBNull.Value)
                {
                    int happenedatf = 0;
                    if (tbl.Rows[0][1] != DBNull.Value && tbl.Rows[0][1].ToString() != "")
                    {
                        int.TryParse(tbl.Rows[0][1].ToString(), out happenedatf);
                    }
                    if (Convert.ToInt32(tbl.Rows[0][0]) == pFarm.UBNid)
                    {
                        if (pAnimal.ThrId > 0) 
                        {
                            if (pUbn.ThrID == pAnimal.ThrId)
                            { return true; }
                            else if (lMstb.getUBNsByThirdID(pAnimal.ThrId).Count == 0)
                            {
                                //BUG 1983 fokker stond 3 X in de Third tabel en dier.ThrId!= UBN.ThrId
                                // maar is het wel dezelfde fokker
                                //Dus
                                if (pThird.ThrZipCode != "" && pThird.ThrExt != "")
                                {
                                    List<THIRD> lZelfde = lMstb.GetThirdsByHouseNrAndZipCode(pThird.ThrExt, pThird.ThrZipCode);
                                    var zelfde = from n in lZelfde
                                                 where n.ThrId == pAnimal.ThrId
                                                 select n;
                                    if (zelfde.Count() > 0)
                                    {
                                        return true;
                                    }
                                }
                                return false;
                            }
                            else return false;
                        }
                        else
                            return true;
                    }

                }
            
                if (pAnimal.ThrId > 0) 
                {
                    if (pUbn.ThrID == pAnimal.ThrId)
                    { return true; }
                    else if (lMstb.getUBNsByThirdID(pAnimal.ThrId).Count == 0)
                    {
                        //BUG 1983 fokker stond 3 X in de Third tabel en dier.ThrId!= UBN.ThrId
                        // maar is het wel dezelfde fokker
                        //Dus
                        if (pThird.ThrZipCode != "" && pThird.ThrExt != "")
                        {
                            List<THIRD> lZelfde = lMstb.GetThirdsByHouseNrAndZipCode(pThird.ThrExt, pThird.ThrZipCode);
                            var zelfde = from n in lZelfde
                                         where n.ThrId == pAnimal.ThrId
                                         select n;
                            if (zelfde.Count() > 0)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    else return false;
                }
                else
                {
                    if (pThird.ThrCountry == "151")
                    {
                        if (pFarm.ProgId == 3 || pFarm.ProgId == 5)
                        {
                            return DieropBedrijfGeboren(pUr, pFarm, pUbn, pThird, pAnimal.AniAlternateNumber, ref Verblijfplaatsen);
                        }
                        else
                        {
                            return DieropBedrijfGeboren(pUr, pFarm, pUbn, pThird, pAnimal.AniLifeNumber, ref Verblijfplaatsen);
                        }
                    }
                    else if (pThird.ThrCountry == "58")//DCF
                    {
                        return DieropBedrijfGeboren(pUr, pFarm, pUbn, pThird, pAnimal.AniAlternateNumber, ref Verblijfplaatsen);
                    }
                }
            }
            return false;
        }

        public static bool DieropBedrijfGeboren(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThrid, String Lifenr)
        {
            List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen = new List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats>();
            return DieropBedrijfGeboren(pUr, pBedrijf, pUbn, pThrid, Lifenr, ref Verblijfplaatsen);
        }

        private static bool DieropBedrijfGeboren(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThrid, String Lifenr, ref  List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats> Verblijfplaatsen)
        {
            bool ret = false;
            AFSavetoDB lMstb = getMysqlDb(pUr);
            if (pThrid.ThrCountry == "151")
            {
                string Status = String.Empty;
                string Code = String.Empty;
                string Omschrijving = String.Empty;
                string pLogfile = String.Empty;
                //int pMaxStrLen = 255;
                String lUsername = "";
                String lPassword = "";
                FTPUSER fusoap = lMstb.GetFtpuser(pUbn.UBNid, pBedrijf.Programid, pBedrijf.ProgId, 9992);
                if (fusoap.UserName != String.Empty && fusoap.Password != String.Empty)
                {

                    lUsername = fusoap.UserName;
                    lPassword = fusoap.Password;
                }
                string BRSnrHouder = String.Empty;
                string UBNhouder = String.Empty;
                string Werknummer = String.Empty;
                DateTime Geboortedat = DateTime.MinValue;
                DateTime Importdat = DateTime.MinValue;
                string LandCodeHerkomst = String.Empty;
                string LandCodeOorsprong = String.Empty;
                string Geslacht = String.Empty;
                string Haarkleur = String.Empty;
                DateTime Einddatum = DateTime.MinValue;
                string RedenEinde = String.Empty;
                string LevensnrMoeder = String.Empty;
                string VervangenLevensnr = String.Empty;


                VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren soapdieren = new VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren();
                soapdieren.LNVDierdetailsV2(lUsername, lPassword, 0,
                                    pUbn.Bedrijfsnummer, pThrid.Thr_Brs_Number, Lifenr, pBedrijf.ProgId,
                                    1, 0, 0,
                                    ref Werknummer,
                                    ref Geboortedat, ref Importdat,
                                    ref LandCodeHerkomst, ref LandCodeOorsprong,
                                    ref Geslacht, ref Haarkleur,
                                    ref Einddatum, ref RedenEinde,
                                    ref LevensnrMoeder, ref VervangenLevensnr,
                                    ref Verblijfplaatsen,
                                    ref Status, ref  Code, ref Omschrijving);
                              
                if (Verblijfplaatsen.Count > 0)
                {
                    //1;ubn;aanvoerdatum;afvoerdatum;adres;postcode;wooplaats;bedrijfstype
                    //File existts
                    char[] spl1 = { ';' };
                    if (Geboortedat > DateTime.MinValue)
                    {
                        try
                        {
                            foreach (VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats vplaats in Verblijfplaatsen)
                            {
                                if (vplaats.UBN == pUbn.Bedrijfsnummer)
                                {
                                    DateTime aanvoerdate = DateTime.MinValue;

                                    if (vplaats.AanvoerDatum != null)
                                    {
                                        aanvoerdate = vplaats.AanvoerDatum;
                                        if (aanvoerdate.Date == Geboortedat.Date)
                                        {
                                            ret = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            else if (pThrid.ThrCountry == "58")
            {
                //FTPUSER fusoap = lMstb.GetFtpuser(pBedrijf.UBNid, pBedrijf.Programid, pBedrijf.ProgId, 9998);
                //SoapDCF.AnimalService serv = new SoapDCF.AnimalService(0, unLogger.getLogDir("IenR"));
                //char[] split = { ' ' };
                //char[] split2 = { ';' };
                //string[] lnrs = Lifenr.Split(split);
                //long nummer = 0;
                //if (lnrs.Length == 2)
                //{
                //    long.TryParse(lnrs[1], out nummer);
                //}
                //else
                //{
                //    long.TryParse(Lifenr, out nummer);
                //}
                //if (nummer > 0)
                //{
                //    long temp = 0;
                //    ret = serv.bornOnFarm(nummer, fusoap.UserName, fusoap.Password, pUbn.Bedrijfsnummer, out temp);
                //    Verblijfplaatsen = new List<VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats>();
                //    string pOutputfile = unLogger.getLogDir("CSVData") + "Verblijfplaatsen_" + pUbn.Bedrijfsnummer + "_" + Lifenr + ".csv";
                //    serv.getAnimalTransfers(Lifenr, fusoap.UserName, fusoap.Password, pOutputfile, "");
                //    if (File.Exists(pOutputfile))
                //    {
                //        //tr.Date.ToString("yyyyMMdd") + ";" + dir.ToString() + ";" + tr.HerdNumber.ToString() + ";" + tr.HerdNumberToFrom.ToString());
                //        StreamReader rdr2 = new StreamReader(pOutputfile);
                //        string line2 = "";
                //        while ((line2 = rdr2.ReadLine()) != null)
                //        {
                //            string[] regel = line2.Split(split2);
                //            //verblpl.UBN == pUbn.Bedrijfsnummer
                //            VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats v = new VSM.RUMA.CORE.SOAPLNV.SOAPLNVDieren.Dierverblijfplaats();
                //            if (regel[1] == "1")
                //            {
                //                if (pUbn.Bedrijfsnummer == regel[2])
                //                {
                //                    v.UBN = pUbn.Bedrijfsnummer;
                //                    v.AanvoerDatum = utils.getDateLNV(regel[0]);
                //                    Verblijfplaatsen.Add(v);
                //                }

                //            }
                //            if (regel[1] == "2")
                //            {
                //                if (pUbn.Bedrijfsnummer == regel[2])
                //                {
                //                    v.UBN = pUbn.Bedrijfsnummer;
                //                    v.AfvoerDatum = utils.getDateLNV(regel[0]);
                //                    Verblijfplaatsen.Add(v);
                //                }

                //            }
                //        }
                //        rdr2.Close();
                //    }
                //}
            }
            return ret;
        }

        /// <summary>
        /// returns if update is neccesary
        /// </summary>
        /// <param name="pAnicat"></param>
        /// <param name="pAnisex"></param>
        /// <param name="isFokstier"></param>
        public static bool setcat123(ref ANIMALCATEGORY pAnicat, int pAnisex, bool isFokstier)
        {
            if (pAnisex == 2)
            {
                if (pAnicat.Anicategory == 1)
                    return false;

                pAnicat.Anicategory = 1;
                return true;
            }
            else
            {
                if (isFokstier)
                {
                    if (pAnicat.Anicategory == 2)
                        return false;

                    pAnicat.Anicategory = 2;
                    return true;
                }
                else
                {
                    if (pAnisex == 0)
                    {
                        if (pAnicat.Anicategory == 1)
                            return false;
                        
                        pAnicat.Anicategory = 1;
                        return true;
                    }
                    else
                    {
                        if (pAnicat.Anicategory == 3)
                            return false;

                        pAnicat.Anicategory = 3;
                        return true;                        
                    }
                }
            }
        }

        internal static string AllowAfvoerUpdate(UserRightsToken pUr, int MovId, int AniId, DateTime newdatum, BEDRIJF bedr)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            //aanvoercheck op datum  aanvoer = 1, afvoer = 2
            List<MOVEMENT> afvoeren = lMstb.GetMovementsByAniIdMovkindUbn(AniId, 2, bedr.UBNid);
            List<MOVEMENT> aanvoeren = lMstb.GetMovementsByAniIdMovkindUbn(AniId, 1, bedr.UBNid);

            MOVEMENT mv = lMstb.GetMovementByMovId(MovId);
            DateTime nearestaanvoermin = DateTime.MinValue;
            DateTime nearestaanvoerplus = DateTime.MaxValue;
            foreach (MOVEMENT rw1 in aanvoeren)
            {

                if (rw1.MovDate.CompareTo(nearestaanvoermin) > 0 && rw1.MovDate.CompareTo(mv.MovDate) < 0)
                {
                    nearestaanvoermin = rw1.MovDate;
                }
                if (rw1.MovDate.CompareTo(nearestaanvoerplus) < 0 && rw1.MovDate.CompareTo(mv.MovDate) > 0)
                {
                    nearestaanvoerplus = rw1.MovDate;
                }
            }

            DateTime nearestafvmin = DateTime.MinValue;
            DateTime nearestafvplus = DateTime.MaxValue;
            foreach (MOVEMENT rw in afvoeren)
            {
                if (rw.MovDate.CompareTo(nearestafvmin) > 0 && rw.MovDate.CompareTo(mv.MovDate) < 0)
                {
                    nearestafvmin = rw.MovDate;
                }
                if (rw.MovDate.CompareTo(nearestafvplus) < 0 && rw.MovDate.CompareTo(mv.MovDate) > 0)
                {
                    nearestafvplus = rw.MovDate;
                }
            }
            if (newdatum.CompareTo(nearestaanvoermin) >= 0 && newdatum.CompareTo(nearestaanvoerplus) <= 0)// && nearestafvmin.CompareTo(nearestaanvoermin) <= 0 && nearestafvplus.CompareTo(nearestaanvoerplus) >= 0)
            { return ""; }
            else
            { return "Datum niet toegestaan!"; }
        }

        internal static string AllowAfvoer(UserRightsToken pUr, int AniId, DateTime datum, BEDRIJF bedr)
        {
            //aanvoercheck op datum  aanvoer = 1, afvoer = 2
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<MOVEMENT> afvoeren = lMstb.GetMovementsByAniIdMovkindUbn(AniId, 2, bedr.UBNid);
            List<MOVEMENT> aanvoeren = lMstb.GetMovementsByAniIdMovkindUbn(AniId, 1, bedr.UBNid);

            List<MOVEMENT> allen = new List<MOVEMENT>();
            allen.Concat(afvoeren);
            allen.Concat(aanvoeren);


            DateTime nearestafvmin = DateTime.MinValue;
            DateTime nearestafvplus = DateTime.MaxValue;
            DateTime nearestaanvoermin = DateTime.MinValue;
            DateTime nearestaanvoerplus = DateTime.MaxValue;

            foreach (MOVEMENT rw in afvoeren)
            {
                if (rw.MovDate.CompareTo(nearestafvmin) > 0 && rw.MovDate.CompareTo(datum) < 0)
                {
                    nearestafvmin = rw.MovDate;
                }
                if (rw.MovDate.CompareTo(nearestafvplus) < 0 && rw.MovDate.CompareTo(datum) > 0)
                {
                    nearestafvplus = rw.MovDate;
                }
            }


            foreach (MOVEMENT rw1 in aanvoeren)
            {

                if (rw1.MovDate.CompareTo(nearestaanvoermin) > 0 && rw1.MovDate.CompareTo(datum) < 0)
                {
                    nearestaanvoermin = rw1.MovDate;
                }
                if (rw1.MovDate.CompareTo(nearestaanvoerplus) < 0 && rw1.MovDate.CompareTo(datum) > 0)
                {
                    nearestaanvoerplus = rw1.MovDate;
                }
            }
            if (datum.CompareTo(nearestaanvoermin) >= 0 && datum.CompareTo(nearestaanvoerplus) <= 0)// && nearestafvmin.CompareTo(nearestaanvoermin) <= 0 && nearestafvplus.CompareTo(nearestaanvoerplus) >= 0)
            { return ""; }
            else
            { return "Datum niet toegestaan!"; }

        }

        public static int getMovementOrder(UserRightsToken ur, int movid)
        {
            AFSavetoDB lMstb = getMysqlDb(ur);
            string sql = "SELECT MovOrder FROM MOVEMENT WHERE MovId=" + movid.ToString();
            DataTable tb = lMstb.GetDataBase().QueryData(ur.getLastChildConnection(), new StringBuilder(sql));
            if (movid == 0)
            { return 0; }
            if (tb.Rows[0][0] == DBNull.Value)
            { return 1; }
            else
            {
                return int.Parse(tb.Rows[0][0].ToString());
            }
        }

        public static int getNewMovementOrderThread(AFSavetoDB msdb, int movkind, DateTime movdate, int animalid, int UbnId)
        {
            //UserRightsToken ur = getAgroRightsToken();
            //string sql = "select max(movorder) from MOVEMENT where AniId= " + animalid.ToString() + " and movkind=" + movkind.ToString() + " and date_format(movdate,'%Y-%m-%d')='" + movdate.Year.ToString() + "-" + addleadingZero(movdate.Month.ToString()) + "-" + addleadingZero(movdate.Day.ToString()) + "'";
            //DataTable tb = lMstb.GetDataBase().QueryData(ur.getLastChildConnection(), new StringBuilder(sql));
            List<MOVEMENT> movs = msdb.GetMovementsByAniIdMovkindUbn(animalid, movkind, UbnId);
            List<MOVEMENT> movs2 = msdb.GetMovementsByAniIdMovkindUbn(animalid, movkind, 0);
            movs = movs.Concat(movs2).ToList();
            if (movs.Count == 0)
            { return 1; }
            else
            {
                int returnOrder = 0;
                foreach (MOVEMENT mov in movs)
                {
                    if (movdate.CompareTo(mov.MovDate) == 0)
                    {
                        if (mov.MovOrder > returnOrder)
                        {
                            returnOrder = mov.MovOrder;
                        }
                    }
                }
                return returnOrder + 1;
            }
        }

        public static int getNewMovementOrder(UserRightsToken pUr, int movkind, DateTime movdate, int animalid, int UbnId)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            //string sql = "select max(movorder) from MOVEMENT where AniId= " + animalid.ToString() + " and movkind=" + movkind.ToString() + " and date_format(movdate,'%Y-%m-%d')='" + movdate.Year.ToString() + "-" + addleadingZero(movdate.Month.ToString()) + "-" + addleadingZero(movdate.Day.ToString()) + "'";
            //DataTable tb = lMstb.GetDataBase().QueryData(ur.getLastChildConnection(), new StringBuilder(sql));
            List<MOVEMENT> movs = lMstb.GetMovementsByAniIdMovkindUbn(animalid, movkind, UbnId);

            if (movs.Count == 0)
            { return 1; }
            else
            {
                int returnOrder = 0;
                foreach (MOVEMENT mov in movs)
                {
                    if (movdate.CompareTo(mov.MovDate) == 0)
                    {
                        if (mov.MovOrder > returnOrder)
                        {
                            returnOrder = mov.MovOrder;
                        }
                    }
                }
                return returnOrder + 1;
            }
        }

        //todo --> kopie uit ASP.RUMAONLINE.utils --> waar moet deze heen?
        internal static string getGoodAlternateNumber(string AniAlternateNumber)
        {
            Regex reg = new Regex(@"^NL\s\d{12}$");
            Regex regNotNL = new Regex(@"^[A-Z]{2}\s\d{6,12}$");
            Match m;
            if (AniAlternateNumber.StartsWith("NL") || AniAlternateNumber.StartsWith("528"))
            {
                m = reg.Match(AniAlternateNumber);
            }
            else
            {
                m = regNotNL.Match(AniAlternateNumber);
            }
            if (m.Success)
            {
                return AniAlternateNumber;
            }
            else
            {
                int lengte = AniAlternateNumber.Length;
                string countrycode = "";
                string nummer2 = "";
                if (isDoubleNumber(AniAlternateNumber))
                {
                    countrycode = "NL";
                    nummer2 = AniAlternateNumber.Remove(0, 3);
                }
                else
                {
                    if (AniAlternateNumber.Contains(" "))
                    {
                        char[] splitje1 = { ' ' };
                        string[] altners = AniAlternateNumber.Split(splitje1);
                        if (isDoubleNumber(altners[0]))
                        {
                            countrycode = "NL";
                        }
                        else
                        {
                            if (altners[0].Length == 2)
                            {
                                countrycode = altners[0].ToUpper();
                            }
                            else
                            {
                                countrycode = "NL";
                            }
                        }
                        nummer2 = altners[1];
                        for (int i = 2; i < altners.Length; i++)
                        {
                            nummer2 += altners[i];
                        }

                    }
                    else
                    {
                        //alles aan elkaar
                        if (AniAlternateNumber.Length >= 2)
                        {
                            countrycode = AniAlternateNumber.Substring(0, 2).ToUpper();
                        }
                        else { countrycode = "NL"; }
                        if (lengte >= 12)
                        {
                            nummer2 = AniAlternateNumber.Substring(lengte - 12, 12);
                        }
                        else { nummer2 = AniAlternateNumber; }

                    }
                }
                return countrycode.Trim() + " " + nummer2.Trim();
            }
        }

        //todo --> kopie uit ASP.RUMAONLINE.utils --> waar moet deze heen?
        public static bool isDoubleNumber(string number)
        {
            double dummyDoub;
            if (Double.TryParse(number, out dummyDoub))
            { return true; }
            else
            { return false; }
        }

        public static void setlastGroupidTHread(int pAniId, int pFarmId, AFSavetoDB mdb)
        {
            try
            {
                //AFSavetoDB mdb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);
                //mdb.setToken(pUr);
                //mdb.SaveInlog("", "", pUr);
                BEDRIJF bedr = mdb.GetBedrijfById(pFarmId);
                ANIMAL ani = mdb.GetAnimalById(pAniId);

                List<MOVEMENT> h = mdb.GetMovementsByAniIdMovkindUbn(ani.AniId, 1, bedr.UBNid);
                List<MOVEMENT> h2 = mdb.GetMovementsByAniIdMovkindUbn(ani.AniId, 8, bedr.UBNid);
                h = h.Concat(h2).ToList();

                if (h.Count > 0)
                {

                    DateTime lastmovdate = DateTime.MinValue;
                    GROUPID newgrid = new GROUPID();
                    newgrid.AniId = ani.AniId;

                    foreach (MOVEMENT m in h)
                    {
                        if (m.UbnId == bedr.UBNid)
                        {
                            if (m.Groupnr != 0)
                            {
                                if (m.MovDate.CompareTo(lastmovdate) > 0)
                                {
                                    lastmovdate = m.MovDate;
                                    newgrid.GroupId = m.Groupnr;
                                }
                            }
                        }
                    }
                    GROUPID groupidpresent = mdb.GetGroupid(ani.AniId);
                    if (groupidpresent.AniId != 0)
                    {
                        if (newgrid.GroupId != 0)
                        {
                            mdb.DeleteGroupId(groupidpresent);
                            mdb.SaveGroupId(newgrid);
                        }
                    }
                    else
                    {
                        if (newgrid.GroupId != 0)
                        {
                            mdb.SaveGroupId(newgrid);
                        }
                    }
                }
            }
            catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
        }

        public static void setlastGroupid(int pAniId, int pFarmId, UserRightsToken pUr)
        {
            try
            {
                AFSavetoDB mdb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);
                //mdb.setToken(pUr);
                //mdb.SaveInlog("", "", pUr);
                BEDRIJF bedr = mdb.GetBedrijfById(pFarmId);
                ANIMAL ani = mdb.GetAnimalById(pAniId);

                List<MOVEMENT> h = mdb.GetMovementsByAniIdMovkindUbn(ani.AniId, 1, bedr.UBNid);
                List<MOVEMENT> h2 = mdb.GetMovementsByAniIdMovkindUbn(ani.AniId, 8, bedr.UBNid);
                h = h.Concat(h2).ToList();

                if (h.Count > 0)
                {

                    DateTime lastmovdate = DateTime.MinValue;
                    GROUPID newgrid = new GROUPID();
                    newgrid.AniId = ani.AniId;

                    foreach (MOVEMENT m in h)
                    {
                        if (m.UbnId == bedr.UBNid)
                        {
                            if (m.Groupnr != 0)
                            {
                                if (m.MovDate.CompareTo(lastmovdate) > 0)
                                {
                                    lastmovdate = m.MovDate;
                                    newgrid.GroupId = m.Groupnr;
                                }
                            }
                        }
                    }
                    GROUPID groupidpresent = mdb.GetGroupid(ani.AniId);
                    if (groupidpresent.AniId != 0)
                    {
                        if (newgrid.GroupId != 0)
                        {
                            mdb.DeleteGroupId(groupidpresent);
                            mdb.SaveGroupId(newgrid);
                        }
                    }
                    else
                    {
                        if (newgrid.GroupId != 0)
                        {
                            mdb.SaveGroupId(newgrid);
                        }
                    }
                }
            }
            catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
        }

        public static VKIINFO convertDataTableToVKIINFO(DataTable dtVki)
        {
            VKIINFO vki = new VKIINFO();
            try
            {
                vki.Internalnr = Convert.ToInt32(dtVki.Rows[0]["Internalnr"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.DierenArtsID = Convert.ToInt32(dtVki.Rows[0]["DierenArtsID"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.FarmNumber = dtVki.Rows[0]["FarmNumber"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Program = Convert.ToInt32(dtVki.Rows[0]["Program"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.SoortAfvoer = Convert.ToInt32(dtVki.Rows[0]["SoortAfvoer"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.SoortDieren = Convert.ToInt32(dtVki.Rows[0]["SoortDieren"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.VerwAfvDatum = Event_functions.getDatumFormat(dtVki.Rows[0]["VerwAfvDatum"], "VerwAfvDatum");
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag1 = Convert.ToInt32(dtVki.Rows[0]["Vraag1"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag2 = Convert.ToInt32(dtVki.Rows[0]["Vraag2"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag3 = Convert.ToInt32(dtVki.Rows[0]["Vraag3"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag4 = Convert.ToInt32(dtVki.Rows[0]["Vraag4"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag5 = Convert.ToInt32(dtVki.Rows[0]["Vraag5"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag6 = Convert.ToInt32(dtVki.Rows[0]["Vraag6"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag7 = Convert.ToInt32(dtVki.Rows[0]["Vraag7"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag8 = Convert.ToInt32(dtVki.Rows[0]["Vraag8"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag9 = Convert.ToInt32(dtVki.Rows[0]["Vraag9"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag10 = Convert.ToInt32(dtVki.Rows[0]["Vraag10"]);
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag5opm = dtVki.Rows[0]["Vraag5opm"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag6opm = dtVki.Rows[0]["Vraag6opm"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag7opm = dtVki.Rows[0]["Vraag7opm"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag8opm = dtVki.Rows[0]["Vraag8opm"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag9opm = dtVki.Rows[0]["Vraag9opm"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            try
            {
                vki.Vraag10opm = dtVki.Rows[0]["Vraag10opm"].ToString();
            }
            catch (Exception exc)
            {
                unLogger.WriteDebug(exc.ToString());
            }
            return vki;
        }

        public static List<VKIDIER> convertDataTableToVKIDIERlist(DataTable vkiDierList)
        {
            List<VKIDIER> lstVkiDier = new List<VKIDIER>();
            if (vkiDierList.Rows.Count > 0)
            {
                if (vkiDierList.Rows[0][0] != DBNull.Value)
                {
                    foreach (DataRow drDier in vkiDierList.Rows)
                    {
                        VKIDIER vdier = new VKIDIER();
                        try
                        {
                            vdier.AniId = Convert.ToInt32(drDier["AniId"]);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteDebug(exc.ToString());
                        }
                        try
                        {
                            vdier.Internalnr = Convert.ToInt32(drDier["Internalnr"]);
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteDebug(exc.ToString());
                        }
                        try
                        {
                            vdier.ReportDate = Event_functions.getDatumFormat(drDier["ReportDate"], "ReportDate");
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteDebug(exc.ToString());
                        }
                        try
                        {
                            vdier.ReportTime = Event_functions.getDatumFormat(drDier["ReportTime"], "ReportTime");
                        }
                        catch (Exception exc)
                        {
                            unLogger.WriteDebug(exc.ToString());
                        }
                        lstVkiDier.Add(vdier);
                    }
                }
            }
            return lstVkiDier;
        }

        public static DataTable getAlAfgevoerdNogVKImelden(BEDRIJF pBedrijf, UserRightsToken pUr)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            StringBuilder sbAll = new StringBuilder("SELECT ANIMAL.*,ANIMALCATEGORY.AniworkNumber AS AniworkNumberCat, ANIMALCATEGORY.Anicategory AS AnicategoryCat FROM ANIMAL INNER JOIN ANIMALCATEGORY ON ANIMAL.AniId=ANIMALCATEGORY.AniId  WHERE ANIMALCATEGORY.FarmId=" + pBedrijf.FarmId.ToString() + " AND ANIMALCATEGORY.Anicategory =4  ORDER BY ANIMALCATEGORY.Anicategory,AniworkNumberCat");
            DataTable tblAll = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sbAll);
            if (tblAll.Rows.Count > 0)
            {
                if (tblAll.Rows[0][0] != DBNull.Value)
                {
                    for (int i = tblAll.Rows.Count - 1; i > -1; i--)
                    {
                        int aniid = int.Parse(tblAll.Rows[i]["AniId"].ToString());
                        List<MOVEMENT> mvs = lMstb.GetMovementsByAniIdMovkindUbn(aniid, 2, pBedrijf.UBNid);
                        if (mvs.Count > 0)
                        {
                            DateTime maxdate = DateTime.MinValue;
                            MOVEMENT m = new MOVEMENT();
                            foreach (MOVEMENT item in mvs)
                            {
                                if (item.MovDate.CompareTo(maxdate) > 0)
                                {
                                    m = item;
                                    maxdate = item.MovDate;
                                }
                            }
                            if (m.MovId != 0)
                            {
                                SALE se = lMstb.GetSale(m.MovId);
                                if (se.SalKind != 2 && se.SalKind != 3)
                                {
                                    //slacht
                                    tblAll.Rows.Remove(tblAll.Rows[i]);
                                }
                            }
                        }
                        else { tblAll.Rows.Remove(tblAll.Rows[i]); }
                    }
                }
            }
            return tblAll;
        }

        public static THIRD searchThird(UserRightsToken pUr, string pUbnNumber, string pZipcode, string pHousenumber, string pKvKnumber)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            THIRD tr = new THIRD();
            if (pUbnNumber.Trim() != "")
            {
                pUbnNumber = ToUpperSpaceAway(pUbnNumber);
                UBN ub = lMstb.getUBNByBedrijfsnummer(pUbnNumber);
                if (ub.ThrID != 0)
                {
                    tr = lMstb.GetThirdByThirId(ub.ThrID);
                }
                else
                {
                    if (ub.Bedrijfsnummer != "")
                    {
                        StringBuilder bld = new StringBuilder();
                        bld.Append("select * from THIRD where ThrSecondName like '%" + ub.Bedrijfsnummer + "%' or ThrFarmNumber like '%" + ub.Bedrijfsnummer + "%' or ThrCompanyName like '%" + ub.Bedrijfsnummer + "%'");
                        DataTable tbl = lMstb.GetDataBase().QueryData(pUr, bld);
                        if (tbl.Rows.Count > 0)
                        {
                            if (tbl.Rows[0][0] != DBNull.Value)
                            {
                                if (tbl.Rows.Count == 1)
                                {
                                    tr = lMstb.GetThirdByThirId(Convert.ToInt32(tbl.Rows[0]["ThrId"]));
                                    ub.ThrID = tr.ThrId;
                                    lMstb.UpdateUBN(ub);
                                }
                                else
                                {
                                    int goeieThrId = getGoodThirdFromTable(tbl);
                                    if (goeieThrId != 0)
                                    {
                                        tr = lMstb.GetThirdByThirId(goeieThrId);
                                        ub.ThrID = tr.ThrId;
                                        lMstb.UpdateUBN(ub);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (pHousenumber.Trim() != "" && pZipcode.Trim() != "")
            {
                pHousenumber = ToUpperSpaceAway(pHousenumber);
                pZipcode = ToUpperSpaceAway(pZipcode);
                tr = lMstb.GetThirdByHouseNrAndZipCode(pHousenumber, pZipcode);
            }
            else if (pKvKnumber.Trim() != "")
            {
                pKvKnumber = ToUpperSpaceAway(pKvKnumber);
                tr = lMstb.GetThirdByKvKnr(pKvKnumber);
            }
            return tr;
        }

        public static List<THIRD> searchThirds(UserRightsToken pUr, string pUbnNumber, string pZipcode, string pHousenumber, string pKvKnumber)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<THIRD> lThirds = new List<THIRD>();

            pUbnNumber = ToUpperSpaceAway(pUbnNumber);
            pHousenumber = ToUpperSpaceAway(pHousenumber);
            pZipcode = ToUpperSpaceAway(pZipcode);
            pKvKnumber = ToUpperSpaceAway(pKvKnumber);

            StringBuilder sql = null;

            //UBN
            if (pUbnNumber.Trim() != "")
            {
                sql = new StringBuilder();
                sql.Append("SELECT * FROM agrofactuur.THIRD  ");
                sql.Append(" INNER JOIN agrofactuur.UBN ");
                sql.Append(" ON agrofactuur.UBN.thrid = agrofactuur.THIRD.thrid ");
                sql.AppendFormat(" WHERE agrofactuur.UBN.Bedrijfsnummer = '{0}' ", pUbnNumber);

                DataTable tbl = lMstb.GetDataBase().QueryData(pUr, sql);
                foreach (DataRow dr in tbl.Rows)
                {
                    var l = new THIRD();
                    if (lMstb.GetDataBase().FillObject(l, dr))
                    {
                        lThirds.Add(l);
                    }
                }
                if (lThirds.Count() == 0)
                {
                    if (ConfigurationManager.ConnectionStrings["RUMAEXTRA"] != null)
                    {
                        DataTable tblxtra = lMstb.GetExtraDataTable(0, ConfigurationManager.ConnectionStrings["RUMAEXTRA"].ToString(), sql.ToString(), MissingSchemaAction.Add);
                        foreach (DataRow drw in tblxtra.Rows)
                        {
                            THIRD lThird = new THIRD();
                            if (lMstb.GetDataBase().FillObject(lThird, drw))
                            {
                                if (lThird.ThrCountry.Trim() == "")
                                {
                                    lThird.ThrCountry = "151";
                                }
                                lThirds.Add(lThird);
                            }
                        }
                    }
                }
            }
            if (pHousenumber.Trim() != "" && pZipcode.Trim() != "")
            {
                lThirds.AddRange(lMstb.GetThirdsByHouseNrAndZipCode(pHousenumber, pZipcode));
                if (lThirds.Count() == 0)
                {
                    if (ConfigurationManager.ConnectionStrings["RUMAEXTRA"] != null)
                    {
                        pHousenumber = pHousenumber.ToUpper().Replace(" ", "");
                        pZipcode = pZipcode.ToUpper().Replace(" ", "");

                        if (pHousenumber.Length > 0 && pZipcode.Length > 0)
                        {
                            StringBuilder lQuery = new StringBuilder();
                            lQuery.Append($@" SELECT * FROM THIRD 
                                    WHERE ThrExt= '{pHousenumber}'  
                            AND ThrZipcode = '{pZipcode}'  AND ThrId>0 ");
                            DataTable dtThirds = lMstb.GetExtraDataTable(0, ConfigurationManager.ConnectionStrings["RUMAEXTRA"].ToString(), lQuery.ToString(), MissingSchemaAction.Add);
                            foreach (DataRow drwThr in dtThirds.Rows)
                            {
                                THIRD lThird = new THIRD();
                                if (lMstb.GetDataBase().FillObject(lThird, drwThr))
                                {
                                    if (lThird.ThrCountry.Trim() == "")
                                    {
                                        lThird.ThrCountry = "151";
                                    }
                                    lThirds.Add(lThird);
                                }
                            }
                        }
                    }
                }
            }
            if (pKvKnumber.Trim() != "")
            {
                sql = new StringBuilder();
                sql.Append(" SELECT * FROM agrofactuur.THIRD");
                sql.AppendFormat(" WHERE  ThrKvKNummer='{0}' ", pKvKnumber);

                DataTable tbl = lMstb.GetDataBase().QueryData(pUr, sql);
                foreach (DataRow dr in tbl.Rows)
                {
                    var l = new THIRD();
                    if (lMstb.GetDataBase().FillObject(l, dr))
                    {
                        lThirds.Add(l);
                    }
                }
                if (lThirds.Count() == 0)
                {
                    if (ConfigurationManager.ConnectionStrings["RUMAEXTRA"] != null)
                    {
                        tbl = lMstb.GetExtraDataTable(0, ConfigurationManager.ConnectionStrings["RUMAEXTRA"].ToString(), sql.ToString(), MissingSchemaAction.Add);
                        foreach (DataRow dr in tbl.Rows)
                        {
                            var l = new THIRD();
                            if (lMstb.GetDataBase().FillObject(l, dr))
                            {
                                if (l.ThrCountry.Trim() == "")
                                {
                                    l.ThrCountry = "151";
                                }
                                lThirds.Add(l);
                            }
                        }
                    }
                }
            }

            //Nog even filteren voor dubbele ThirdId's
            List<THIRD> lThirds2 = new List<THIRD>();
            foreach (THIRD t in lThirds)
            {
                if (lThirds2.Where(r => r.ThrId == t.ThrId).Count() == 0)
                    lThirds2.Add(t);
            }
            lThirds2 = (from n in lThirds2 where n.ThrId > 0 select n).ToList();
              
            return lThirds2;
        }

        private static int getGoodThirdFromTable(DataTable pTableThirds)
        {
            /*pTableThirds moet meer als 1 row hebben.
             * de bedoeling is om uit een rijtje THIRDS ==pTableThirds die een 
               ubn nummer in secondname farmnumber of companyname hebben
             * diegene te kiezen die voldoet aan de hoge eisen van
             * de meeste velden zijn gevuld 
             * 
             */
            int ThrId = 0;
            int meestgevulderowindex = 0;
            int hoogsteaantal = 0;
            int gevuld = 0;
            for (int i = 0; i < pTableThirds.Rows.Count - 1; i++)
            {
                gevuld = 0;
                for (int j = 0; j < pTableThirds.Columns.Count - 1; j++)
                {
                    if (pTableThirds.Rows[i][j] != DBNull.Value)
                    {
                        gevuld = gevuld + 1;
                    }
                }
                if (gevuld > hoogsteaantal)
                {
                    hoogsteaantal = gevuld;
                    meestgevulderowindex = i;
                }
            }
            try
            {
                ThrId = int.Parse(pTableThirds.Rows[meestgevulderowindex]["ThrId"].ToString());
            }
            catch (Exception exc)
            { unLogger.WriteDebug(exc.ToString()); }
            return ThrId;
        }

        public static string ToUpperSpaceAway(string pTekst)
        {
            //uppercase en zonder spaties
            pTekst = pTekst.ToUpper();
            pTekst = pTekst.Replace(" ", "");
            return pTekst;
        }

        [Obsolete("moet nog verder uitgewerkt worden")]
        public static string getLevensnummer(UserRightsToken userRightsToken, BEDRIJF pBedrijf, string rfid, string LNVlevensNummer)
        {
            if (pBedrijf.Programid == 22 || (pBedrijf.Programid > 23 && pBedrijf.Programid < 33))
            { rfid = "2"; }

            if (rfid == "1")
            {
                return LNVlevensNummer;
            }
            else if (rfid == "2")
            {
                return LNVlevensNummer;
            }
            else if (rfid == "3")
            {

                return LNVlevensNummer;
            }
            else
            {
                return LNVlevensNummer;
            }
        }

        public static String getLNVHaarKleur(UserRightsToken pUr, BEDRIJF pBedrijf, int pHaircolor, string pAniHaircolor_Memo)
        {
            if (pAniHaircolor_Memo != "")
            { return pAniHaircolor_Memo; }
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<LABELS> mMutHaarKleur = lMstb.GetLabels(16, Convert.ToInt32(utils.getLabelsLabcountrycode()));
            var HaarkleurLabel = from CurLabel in mMutHaarKleur
                                 where CurLabel.LabId == pHaircolor
                                 select CurLabel;
            if (HaarkleurLabel.Count() == 0) return String.Empty;
            return HaarkleurLabel.First().LabLabel;
        }

        //public static int getAgrobaseLabIdHaarKleurLNVDeprecated(UserRightsToken pUr, BEDRIJF pBedrijf, string pHaircolor)
        //{
        //    AFSavetoDB lMstb = getMysqlDb(pUr);
        //    List<LABELS> mMutHaarKleur = lMstb.GetLabels(16, Convert.ToInt32(utils.getLabelsLabcountrycode()));
        //    var HaarkleurLabel = from CurLabel in mMutHaarKleur
        //                         where CurLabel.LabLabel == pHaircolor
        //                         select CurLabel;
        //    if (HaarkleurLabel.Count() == 0) return 0;
        //    return HaarkleurLabel.First().LabId;
        //}

        public static int getCountryId(UserRightsToken pUr, string pCode)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<COUNTRY> lnds = lMstb.GetAllCountries();
            var landjes = from CurLabel in lnds
                          where CurLabel.LandAfk3 == pCode
                          select CurLabel;
            if (landjes.Count() == 0)
            {
                var landjes2 = from CurLabel in lnds
                               where CurLabel.LandAfk2 == pCode
                               select CurLabel;
                if (landjes2.Count() == 0)
                {

                    return 0;
                }
                else
                {
                    return Convert.ToInt32(landjes2.First().LandId);
                }

            }
            else
            {
                return Convert.ToInt32(landjes.First().LandId);
            }
        }

        public static string getAgrobaseIdLNVRedenEinde(UserRightsToken userRightsToken, BEDRIJF lGebrBedr, string p)
        {
            return p;
            /*  <ns1:selDomeincode>IRD_CODE_REDEN_EINDE_DIR</ns1:selDomeincode> 
             - <ns1:domein>
               <ns1:code>AE</ns1:code> 
               <ns1:omschrijving>Ambtshalve einde</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>EX</ns1:code> 
               <ns1:omschrijving>Export</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>GI</ns1:code> 
               <ns1:omschrijving>Geboorte ingetrokken</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>II</ns1:code> 
               <ns1:omschrijving>Import ingetrokken</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>ND</ns1:code> 
               <ns1:omschrijving>Natuurlijke dood</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>RM</ns1:code> 
               <ns1:omschrijving>Ruiming</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>SL</ns1:code> 
               <ns1:omschrijving>Slacht</ns1:omschrijving> 
               </ns1:domein>
             - <ns1:domein>
               <ns1:code>VM</ns1:code> 
               <ns1:omschrijving>Vermissing</ns1:omschrijving> 
             */
        }

        public static THIRD getAndCheckUBNnummer(UserRightsToken pUr, string pUBNhouder)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            UBN aanvoerUBN = lMstb.getUBNByBedrijfsnummer(pUBNhouder);
            THIRD aanvoerThird = searchThird(pUr, pUBNhouder, "", "", "");

            if (aanvoerUBN.UBNid == 0)
            {
                aanvoerUBN.Bedrijfsnaam = pUBNhouder;
                aanvoerUBN.Bedrijfsnummer = pUBNhouder;
                if (aanvoerThird.ThrId != 0)
                {
                    aanvoerUBN.ThrID = aanvoerThird.ThrId;
                }
                else
                {
                    aanvoerThird = new THIRD();
                    aanvoerThird.ThrSecondName = pUBNhouder;
                    aanvoerThird.ThrCompanyName = pUBNhouder;
                    aanvoerThird.ThrFarmNumber = pUBNhouder;
                    lMstb.SaveThird(aanvoerThird);//insert
                    aanvoerUBN.ThrID = aanvoerThird.ThrId;
                }
                lMstb.SaveUbn(aanvoerUBN);// = insert
            }
            if (aanvoerThird.ThrId == 0)
            {
                aanvoerThird = new THIRD();
                aanvoerThird.ThrSecondName = pUBNhouder;
                aanvoerThird.ThrCompanyName = pUBNhouder;
                aanvoerThird.ThrFarmNumber = pUBNhouder;
                lMstb.SaveThird(aanvoerThird);//insert
                aanvoerUBN.ThrID = aanvoerThird.ThrId;
                lMstb.UpdateUBN(aanvoerUBN);//updaten
            }
            return aanvoerThird;
        }

        private static void setMovTime(UserRightsToken pUr, ref MOVEMENT pMov, string pMovTime)
        {
            Regex r = new Regex(@"^\d{1,2}:\d{2}$");
            Match m = r.Match(pMovTime);

            DateTime tijd = pMov.MovDate.Date;
            int Hours = 0;
            int Minutes = 0;
            if (m.Success)
            {
                char[] split = { ':' };
                string[] times = pMovTime.Split(split);
                if (int.TryParse(times[0], out Hours))
                {
                    tijd = tijd.AddHours(Hours);
                }
                if (int.TryParse(times[1], out Minutes))
                {
                    tijd = tijd.AddMinutes(Minutes);
                }

            }
            else
            {
                if (pMov.MovId == 0)
                {
                    if (pMov.AniId > 0)
                    {
                        AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
                        List<MOVEMENT> movs = lMstb.GetMovementsByAniId(pMov.AniId);
                        var same = from n in movs
                                   where n.MovDate.Date == tijd.Date
                                   orderby n.MovTime descending
                                   select n;
                        if (same.Count() > 0)
                        {
                            DateTime lastmovtime = same.ElementAt(0).MovTime;
                            tijd = tijd.AddHours((lastmovtime.Hour + 1));
                        }
                    }
                }
            }

            if (tijd.Hour == 0)
            {
                tijd = tijd.AddHours(8);
            }


            pMov.MovTime = tijd;

        }

        public static void HandleMovTimes(UserRightsToken pUr, DateTime pInvoerdatum, string pMovTime, ref MOVEMENT pMov)
        {
            //voor het bepalen van de animalcategory wordt ook gebruik gemaakt van de MovTime
            //dus de MovTime moet hetzelfde blijven, de Datum kan wel veranderen
            //maar kan ook weer teruggezet worden naar oude datum
            //Zie AcomEdidapAlg HandleMovTimes

            pInvoerdatum = pInvoerdatum.Date;

            pMov.MovDate = pInvoerdatum;

            pMov.MovMutationDate = DateTime.Now;
            pMov.MovMutationTime = DateTime.Now;

            setMovTime(pUr, ref pMov, pMovTime);

        }

        public static void HandleMovTimesOlD(DateTime pInvoerdatum, ref MOVEMENT pMov)
        {
            //voor het bepalen van de animalcategory wordt ook gebruik gemaakt van de MovTime
            //dus de MovTime moet hetzelfde blijven, de Datum kan wel veranderen
            //maar kan ook weer teruggezet worden naar oude datum
            //Zie AcomEdidapAlg HandleMovTimes

            pInvoerdatum = pInvoerdatum.Date;

            pMov.MovDate = pInvoerdatum;

            pMov.MovMutationDate = DateTime.Now;
            pMov.MovMutationTime = DateTime.Now;
            //setMovTime(ref pMov, pMov.MovTime.ToString("HH:mm"));

        }

        public static List<AGRO_LABELS> getMovementsByUser(UserRightsToken pUr, int pProgId, int pProgramId, List<int> pMoveIdList, int pCountryCode)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<AGRO_LABELS> lblsverplaatsingen = lMstb.GetAgroLabels(CORE.DB.LABELSConst.labKind.MOVKIND, pCountryCode, pProgramId, pProgId);

            //switch (pProgId)
            //{

            //    case 3:
            //        lblsverplaatsingen = lMstb.GetLabels(46, pCountryCode);
            //        break;
            //    case 5:
            //        lblsverplaatsingen = lMstb.GetLabels(56, pCountryCode);
            //        break;
            //    default:
            //        lblsverplaatsingen = lMstb.GetLabels(1, pCountryCode);
            //        break;
            //}
            for (int i = lblsverplaatsingen.Count - 1; i > -1; i--)
            {
                if (!pMoveIdList.Contains(lblsverplaatsingen[i].LabID))
                {
                    lblsverplaatsingen.Remove(lblsverplaatsingen[i]);
                }
            }
            return lblsverplaatsingen;
            //if (lblsverplaatsingen.Count > 0)
            //{
            //    /*
            //        1	Aanvoer
            //        2	Afvoer
            //        3	Dood
            //        4	Inscharen / huren
            //        5	Einde inscharen /einde huur
            //        6	Uitscharen / verhuren
            //        7	Einde uitscharen / einde verhuur
            //        8	Lokatie
            //     */
            //    ArrayList Labidseruit = new ArrayList();
            //    switch (Programid)
            //    {
            //        case 7:
            //            //geiten egam
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            Labidseruit.Add(8);
            //            break;
            //        case 100:
            //            //geiten egam Admin
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            Labidseruit.Add(8);
            //            break;
            //        case 8:
            //            //mestkalveren
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            break;
            //        case 9:
            //            //mestkalveren
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            break;
            //        case 14:
            //            //mestkalveren nee wordt schapen junior
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(8);

            //            break;
            //        case 15:
            //            //mestkalveren
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            break;
            //        case 16:
            //            Labidseruit.Add(8);
            //            break;
            //        case 17:
            //            //mestkalveren  nee wordt schapen basis
            //            //Labidseruit.Add(4);
            //            //Labidseruit.Add(5);
            //            //Labidseruit.Add(8);
            //            break;
            //        case 19:
            //            //mestkalveren
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            break;
            //        case 20:
            //            //mestkalveren
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            break;
            //        case 23:
            //            //paligroup
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            Labidseruit.Add(8);
            //            break;
            //        case 230:
            //            //paligroup admin
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            Labidseruit.Add(8);
            //            break;
            //        case 50:
            //            //Van veen (zelfde als paligroup)
            //            Labidseruit.Add(4);
            //            Labidseruit.Add(5);
            //            Labidseruit.Add(6);
            //            Labidseruit.Add(7);
            //            Labidseruit.Add(8);
            //            break;
            //        default:
            //            if (Programid == 22 || (Programid > 23 && Programid < 33))
            //            {
            //                //Labidseruit.Add(4);
            //                //Labidseruit.Add(5);
            //                //Labidseruit.Add(6);
            //                //Labidseruit.Add(7);
            //            }
            //            break;
            //    }
            //    for (int i = lblsverplaatsingen.Count - 1; i > -1; i--)
            //    {
            //        if (Labidseruit.Contains(lblsverplaatsingen[i].LabId))
            //        {
            //            lblsverplaatsingen.Remove(lblsverplaatsingen[i]);
            //        }
            //    }
            //}
        }

        public static string saveDoodMutations(UserRightsToken pUr, BEDRIJF pBedr, ANIMAL pAni, MOVEMENT pMov)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            UBN ubu = lMstb.GetubnById(pBedr.UBNid);

            ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(pAni.AniId, pBedr.FarmId);

            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedr.FarmId, "IRviaModem", "True");

            string melden = "";
            if (FCIRviaModem.FValue.ToLower() == "true")
            {
                MUTATION mut = new MUTATION();
                if (pMov.MovId > 0)
                {
                    mut = lMstb.GetMutationByMovId(pMov.MovId);
                }
                if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                {
                    mut.Lifenumber = Event_functions.getGoodAlternateNumber(pAni.AniAlternateNumber);

                }
                else
                {
                    mut.Lifenumber = pAni.AniLifeNumber;
                }
                mut.Worknumber = aniscat.AniWorknumber;
                mut.Name = pAni.AniName;
                mut.IDRRace = lMstb.BepaalIDRRascode(pBedr, pAni.AniAlternateNumber);
                mut.IDRBirthDate = pAni.AniBirthDate;
                mut.IDRLossDate = pMov.MovDate;
                mut.Nling = pAni.AniNling;
                mut.CodeMutation = 6;//dood
                mut.Sex = pAni.AniSex;
                mut.Haircolor = pAni.Anihaircolor;
                mut.AniHaircolor_Memo = pAni.AniHaircolor_Memo;
                mut.MovId = pMov.MovId;
                mut.UbnId = pMov.UbnId;
                mut.Farmnumber = ubu.Bedrijfsnummer;
                mut.FarmNumberFrom = ubu.Bedrijfsnummer;
                mut.FarmNumberTo = "";
                mut.CountryCodeBirth = pAni.AniCountryCodeBirth;

                if (pBedr.ProgId == 25)
                {
                    mut.SendTo = 35;
                }
                else
                {
                    RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                    string defIenRaction = r.getdefIenRaction(pUr, pBedr.UBNid, pBedr.ProgId, pBedr.Programid);
                    FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedr.FarmId, "VerstuurIenR", defIenRaction);

                    mut.SendTo = IenRCom.ValueAsInteger();
                }

                if (pMov.ThrId > 0)
                {
                    List<UBN> ubnToos = lMstb.getUBNsByThirdID(pMov.ThrId);
                    if (ubnToos.Count() > 0)
                    {
                        foreach (UBN ubnto in ubnToos)
                        {
                            if (ubnto.Bedrijfsnummer != "")
                            {
                                mut.FarmNumberTo = ubnto.Bedrijfsnummer;
                                break;
                            }
                        }
                    }
                }
                if (pAni.AniIdMother != 0)
                {
                    ANIMAL moeder = lMstb.GetAnimalById(pAni.AniIdMother);
                    if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                    {
                        mut.LifenumberMother = Event_functions.getGoodAlternateNumber(moeder.AniAlternateNumber);

                    }
                    else
                    {
                        mut.LifenumberMother = moeder.AniLifeNumber;
                    }
                }
                mut.MutationDate = pMov.MovTime;
                mut.MutationTime = pMov.MovTime;
                if (pAni.AniIdFather != 0)
                {
                    ANIMAL vader = lMstb.GetAnimalById(pAni.AniIdFather);
                    if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                    {
                        mut.LifeNumberFather = Event_functions.getGoodAlternateNumber(vader.AniAlternateNumber);

                    }
                    else
                    {
                        mut.LifeNumberFather = vader.AniLifeNumber;
                    }
                }
                mut.AlternateLifeNumber = pAni.AniAlternateNumber;

                lMstb.SaveMutation(mut);
                melden = "melden";
            }
            else
            {
                MUTALOG mutlog = new MUTALOG();
                if (pMov.MovId > 0)
                {
                    mutlog = lMstb.GetMutaLogByMovId(pMov.MovId);
                }
                if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                {
                    mutlog.Lifenumber = Event_functions.getGoodAlternateNumber(pAni.AniAlternateNumber);

                }
                else
                {
                    mutlog.Lifenumber = pAni.AniLifeNumber;
                }
                mutlog.Worknumber = aniscat.AniWorknumber;
                mutlog.Name = pAni.AniName;
                mutlog.CodeMutation = 6;//dood

                mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedr, pAni.AniAlternateNumber);
                mutlog.IDRBirthDate = pAni.AniBirthDate;
                mutlog.IDRLossDate = pMov.MovDate;
                mutlog.LossDate = mutlog.IDRLossDate;
                mutlog.Sex = pAni.AniSex;
                mutlog.Haircolor = pAni.Anihaircolor;
                mutlog.AniHaircolor_Memo = pAni.AniHaircolor_Memo;
                mutlog.MovId = pMov.MovId;
                mutlog.UbnId = pMov.UbnId;
                mutlog.Nling = pAni.AniNling;
                mutlog.Farmnumber = ubu.Bedrijfsnummer;
                mutlog.FarmNumberFrom = ubu.Bedrijfsnummer;
                List<UBN> ubnToos = lMstb.getUBNsByThirdID(pMov.ThrId);
                if (ubnToos.Count() > 0)
                {
                    foreach (UBN ubnto in ubnToos)
                    {
                        if (ubnto.Bedrijfsnummer != "")
                        {
                            mutlog.FarmNumberTo = ubnto.Bedrijfsnummer;
                            break;
                        }
                    }
                }
                if (pAni.AniIdMother != 0)
                {
                    ANIMAL moeder = lMstb.GetAnimalById(pAni.AniIdMother);
                    if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                    {
                        mutlog.LifenumberMother = Event_functions.getGoodAlternateNumber(moeder.AniAlternateNumber);

                    }
                    else
                    {
                        mutlog.LifenumberMother = moeder.AniLifeNumber;
                    }
                }
                mutlog.MutationDate = pMov.MovTime;
                mutlog.MutationTime = pMov.MovTime;
                if (pAni.AniIdFather != 0)
                {
                    ANIMAL vader = lMstb.GetAnimalById(pAni.AniIdFather);
                    if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                    {
                        mutlog.LifeNumberFather = Event_functions.getGoodAlternateNumber(vader.AniAlternateNumber);

                    }
                    else
                    {
                        mutlog.LifeNumberFather = vader.AniLifeNumber;
                    }
                }
                mutlog.AlternateLifeNumber = pAni.AniAlternateNumber;

                lMstb.InsertMutLog(mutlog);
            }
            return melden;
        }

        public static void saveInscharenMutation(UserRightsToken pUr, BEDRIJF pBedr, ANIMAL pAnimal, MOVEMENT pMovement)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedr.FarmId);

            MUTATION mut = new MUTATION();
            if (pMovement.MovId > 0)
            {
                mut = lMstb.GetMutationByMovId(pMovement.MovId);
            }
            if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
            {
                mut.Lifenumber = getGoodAlternateNumber(pAnimal.AniAlternateNumber);
            }
            else
            {
                mut.Lifenumber = pAnimal.AniLifeNumber;
            }
            mut.Worknumber = aniscat.AniWorknumber;
            mut.Name = pAnimal.AniName;
            mut.CountryCodeBirth = pAnimal.AniCountryCodeBirth;
            UBN ubu = lMstb.GetubnById(pBedr.UBNid);
            if (pMovement.MovKind == 4)
            {
                mut.CodeMutation = 11;
                mut.FarmNumberTo = ubu.Bedrijfsnummer;
                UBN fromUbn = lMstb.getUBNByThirdID(pMovement.ThrId);
                mut.FarmNumberFrom = fromUbn.Bedrijfsnummer;
            }
            else if (pMovement.MovKind == 5)
            {
                mut.CodeMutation = 4; // afvoer = einde inscharen
                mut.FarmNumberFrom = ubu.Bedrijfsnummer;
                UBN best = lMstb.getUBNByThirdID(pMovement.ThrId);
                mut.FarmNumberTo = best.Bedrijfsnummer;
            }
            mut.MutationDate = pMovement.MovDate;
            mut.MutationTime = pMovement.MovTime;
            mut.IDRRace = lMstb.BepaalIDRRascode(pBedr, pAnimal.AniAlternateNumber);
            mut.Sex = pAnimal.AniSex;
            mut.Haircolor = pAnimal.Anihaircolor;
            mut.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
            mut.MovId = pMovement.MovId;
            mut.UbnId = int.Parse(pMovement.UbnId.ToString());
            mut.Nling = pAnimal.AniNling;
            mut.Farmnumber = ubu.Bedrijfsnummer;

            if (pBedr.ProgId == 25)
            {
                mut.SendTo = 35;
            }
            else
            {
                RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                string defIenRaction = r.getdefIenRaction(pUr, pBedr.UBNid, pBedr.ProgId, pBedr.Programid);
                FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedr.FarmId, "VerstuurIenR", defIenRaction);

                mut.SendTo = IenRCom.ValueAsInteger();
            }

            if (pAnimal.AniIdMother != 0)
            {
                ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                {
                    mut.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);

                }
                else
                {
                    mut.LifenumberMother = moeder.AniLifeNumber;
                }
            }

            if (pAnimal.AniIdFather != 0)
            {
                ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                {
                    mut.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                }
                else
                {

                    mut.LifeNumberFather = vader.AniLifeNumber;
                }
            }
            mut.AlternateLifeNumber = pAnimal.AniAlternateNumber;

            lMstb.SaveMutation(mut);
        }

        public static void saveUitscharenMutation(UserRightsToken pUr, BEDRIJF pBedr, ANIMAL pAnimal, MOVEMENT pMovement)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMALCATEGORY aniscat = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedr.FarmId);
            UBN ubu = lMstb.GetubnById(pBedr.UBNid);
            MUTATION mut = new MUTATION();
            if (pMovement.MovId > 0)
            {
                mut = lMstb.GetMutationByMovId(pMovement.MovId);
            }
            if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
            {
                mut.Lifenumber = getGoodAlternateNumber(pAnimal.AniAlternateNumber);

            }
            else
            {
                mut.Lifenumber = pAnimal.AniLifeNumber;
            }
            mut.Worknumber = aniscat.AniWorknumber;
            mut.CountryCodeBirth = pAnimal.AniCountryCodeBirth;
            mut.Name = pAnimal.AniName;
            mut.IDRRace = lMstb.BepaalIDRRascode(pBedr, pAnimal.AniAlternateNumber);
            if (pMovement.MovKind == 6)
            {
                mut.CodeMutation = 12;
            }
            else if (pMovement.MovKind == 7)
            {
                mut.CodeMutation = 1;
            }
            if (pMovement.MovKind == 6)
            {
                if (pMovement.ThrId != 0)
                {
                    mut.FarmNumberFrom = ubu.Bedrijfsnummer;
                    UBN best = lMstb.getUBNByThirdID(pMovement.ThrId);
                    mut.FarmNumberTo = best.Bedrijfsnummer;
                }
                else
                {
                    mut.FarmNumberTo = "";
                }
            }
            else if (pMovement.MovKind == 7)
            {
                MOVEMENT lastmov = getlastMovementUitscharen(pUr, pBedr, pAnimal);

                mut.FarmNumberTo = ubu.Bedrijfsnummer;
                UBN fromUbn = lMstb.getUBNByThirdID(lastmov.ThrId);
                mut.FarmNumberFrom = fromUbn.Bedrijfsnummer;

            }

            mut.Sex = pAnimal.AniSex;
            mut.Haircolor = pAnimal.Anihaircolor;
            mut.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
            mut.MovId = pMovement.MovId;
            mut.UbnId = int.Parse(pMovement.UbnId.ToString());
            mut.MutationDate = pMovement.MovDate;
            mut.MutationTime = pMovement.MovTime;
            mut.Nling = pAnimal.AniNling;
            mut.Farmnumber = ubu.Bedrijfsnummer;

            if (pBedr.ProgId == 25)
            {
                mut.SendTo = 35;
            }
            else
            {
                RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                string defIenRaction = r.getdefIenRaction(pUr, pBedr.UBNid, pBedr.ProgId, pBedr.Programid);
                FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedr.FarmId, "VerstuurIenR", defIenRaction);

                mut.SendTo = IenRCom.ValueAsInteger();
            }

            if (pAnimal.AniIdMother != 0)
            {
                ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                {
                    mut.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                }
                else
                {

                    mut.LifenumberMother = moeder.AniLifeNumber;
                }
            }

            if (pAnimal.AniIdFather != 0)
            {
                ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                if (pBedr.ProgId == 3 || pBedr.ProgId == 5)
                {
                    mut.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                }
                else
                {

                    mut.LifeNumberFather = vader.AniLifeNumber;
                }
            }
            mut.AlternateLifeNumber = pAnimal.AniAlternateNumber;

            lMstb.SaveMutation(mut);
        }

        public static void insertGeboorteMutation(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL ani, EVENT ev, BIRTH birt)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");
            UBN lUBN = lMstb.GetubnById(pBedrijf.UBNid);
            if (FCIRviaModem.FValue.ToLower() == "true")
            {
                MUTATION mut = new MUTATION();
                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                {
                    mut.AlternateLifeNumber = ani.AniLifeNumber;
                    mut.CodeMutation = 2;
                    mut.EventId = ev.EventId;
                    mut.Farmnumber = lUBN.Bedrijfsnummer;
                    mut.IDRBirthDate = ani.AniBirthDate;
                    mut.CountryCodeBirth = ani.AniCountryCodeBirth;
                    mut.Lifenumber = ani.AniLifeNumber;
                    mut.Haircolor = ani.Anihaircolor;
                    mut.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                    mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                    if (ani.AniIdMother > 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);
                        mut.LifenumberMother = moeder.AniLifeNumber;
                    }
                    if (ani.AniIdFather > 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(ani.AniIdFather);
                        mut.LifeNumberFather = vader.AniLifeNumber;
                    }
                    mut.MutationDate = ev.EveDate;
                    mut.MutationTime = ev.EveDate;
                    mut.FarmNumberTo = "";
                    mut.FarmNumberFrom = "";

                    mut.UbnId = pBedrijf.UBNid;
                    mut.Weight = birt.CalfWeight;
                    mut.Worknumber = "";
                }
                else
                {
                    mut.AlternateLifeNumber = ani.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                    mut.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());
                    mut.EventId = ev.EventId;
                    mut.Farmnumber = lUBN.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                    mut.FarmNumberTo = "";
                    mut.FarmNumberFrom = "";
                    mut.Haircolor = ani.Anihaircolor;
                    mut.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                    mut.IDRBirthDate = ani.AniBirthDate;
                    mut.CountryCodeBirth = ani.AniCountryCodeBirth;
                    mut.Lifenumber = getGoodAlternateNumber(ani.AniAlternateNumber);
                    mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                    if (ani.AniIdMother > 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);

                        mut.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber); ;
                    }
                    if (ani.AniIdFather > 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(ani.AniIdFather);

                        mut.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                    }
                    mut.MutationDate = ev.EveDate;// (DateTime)rw["EveDate"];
                    mut.MutationTime = ev.EveDate;// (DateTime)rw["EveDate"];

                    mut.Sex = ani.AniSex;// int.Parse(rw["Sex"].ToString());
                    mut.UbnId = pBedrijf.UBNid;
                    mut.Weight = birt.CalfWeight;// double.Parse(rw["CalfWeight"].ToString());
                    mut.Worknumber = "";// rw["AniWorknumber"].ToString();
                }

                if (pBedrijf.ProgId == 25)
                {
                    mut.SendTo = 35;
                }
                else
                {
                    RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                    string defIenRaction = r.getdefIenRaction(pUr, pBedrijf.UBNid, pBedrijf.ProgId, pBedrijf.Programid);
                    FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedrijf.FarmId, "VerstuurIenR", defIenRaction);

                    mut.SendTo = IenRCom.ValueAsInteger();
                }

                lMstb.SaveMutation(mut);
            }
            else
            {
                MUTALOG mutlog = new MUTALOG();

                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                {
                    mutlog.AlternateLifeNumber = ani.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                    mutlog.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());
                    mutlog.EventId = ev.EventId;
                    mutlog.Farmnumber = lUBN.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                    mutlog.FarmNumberTo = "";
                    mutlog.FarmNumberFrom = "";
                    mutlog.Haircolor = ani.Anihaircolor;
                    mutlog.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                    mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                    mutlog.IDRBirthDate = ani.AniBirthDate;
                    mutlog.Lifenumber = ani.AniLifeNumber;
                    if (ani.AniIdMother > 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);
                        mutlog.LifenumberMother = moeder.AniLifeNumber;
                    }
                    if (ani.AniIdFather > 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(ani.AniIdFather);
                        mutlog.LifeNumberFather = vader.AniLifeNumber;
                    }
                    mutlog.MutationDate = ev.EveDate;// (DateTime)rw["EveDate"];
                    mutlog.MutationTime = ev.EveDate;// (DateTime)rw["EveDate"];

                    mutlog.Sex = ani.AniSex;// int.Parse(rw["Sex"].ToString());
                    mutlog.UbnId = pBedrijf.UBNid;// int.Parse(rw["UbnId"].ToString());
                    mutlog.Weight = birt.CalfWeight;// double.Parse(rw["CalfWeight"].ToString());
                    mutlog.Worknumber = "";// rw["AniWorknumber"].ToString();
                }
                else
                {
                    mutlog.AlternateLifeNumber = ani.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                    mutlog.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());
                    mutlog.EventId = ev.EventId;
                    mutlog.Farmnumber = lUBN.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                    mutlog.FarmNumberTo = "";
                    mutlog.FarmNumberFrom = "";
                    mutlog.Haircolor = ani.Anihaircolor;
                    mutlog.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                    mutlog.IDRBirthDate = ani.AniBirthDate;
                    mutlog.Lifenumber = getGoodAlternateNumber(ani.AniAlternateNumber);
                    mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                    if (ani.AniIdMother > 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(ani.AniIdMother);

                        mutlog.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                    }
                    if (ani.AniIdFather > 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(ani.AniIdFather);

                        mutlog.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                    }
                    mutlog.MutationDate = ev.EveDate;// (DateTime)rw["EveDate"];
                    mutlog.MutationTime = ev.EveDate;// (DateTime)rw["EveDate"];

                    mutlog.Sex = ani.AniSex;// int.Parse(rw["Sex"].ToString());
                    mutlog.UbnId = pBedrijf.UBNid;// int.Parse(rw["UbnId"].ToString());
                    mutlog.Weight = birt.CalfWeight;// double.Parse(rw["CalfWeight"].ToString());
                    mutlog.Worknumber = "";// rw["AniWorknumber"].ToString();
                }
                lMstb.InsertMutLog(mutlog);
            }
        }

        internal static MOVEMENT getlastMovementUitscharen(UserRightsToken pUr, BEDRIJF pBedr, ANIMAL pAnimal)
        {

            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<MOVEMENT> mofsuitscharen = lMstb.GetMovements(pAnimal.AniId, 6);
            DateTime laatsteuitscharen = DateTime.MinValue;
            MOVEMENT leste = new MOVEMENT();
            foreach (MOVEMENT huur in mofsuitscharen)
            {
                if (huur.MovDate.CompareTo(laatsteuitscharen) > 0)
                {
                    leste = huur;
                }
            }

            return leste;
        }

        public static DateTime getLastMovementDate(UserRightsToken pUr, int pAniId, BEDRIJF pBedrijf)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            DateTime lDate = DateTime.MinValue;
            List<MOVEMENT> movs = lMstb.GetMovementsByUbn(pAniId, pBedrijf.UBNid);
            var temp = from n
                       in movs
                       orderby n.MovDate descending, n.MovId descending
                       select n;
            if (temp != null)
            {
                if (temp.Count() > 0)
                {
                    MOVEMENT LastMov = temp.ElementAt(0);
                    lDate = LastMov.MovDate;
                }
            }
            return lDate;
        }

        public static DateTime getLastViewDate(UserRightsToken pUr, int pAniId, BEDRIJF pBedrijf)
        {
            //Alleen voor dieren die een movement hebben
            //en anicategory==4 ofhoger
            AFSavetoDB lMstb = getMysqlDb(pUr);
            DateTime lDate = DateTime.MinValue;
            List<MOVEMENT> movs = lMstb.GetMovementsByUbn(pAniId, pBedrijf.UBNid);
            int[] afvoeren = { 2, 3, 5, 6 };
            var temp = from n in movs
                       where afvoeren.Contains(n.MovKind)
                       orderby n.MovDate descending, n.MovId descending
                       select n;
            if (temp != null)
            {
                if (temp.Count() > 0)
                {
                    MOVEMENT LastMov = temp.ElementAt(0);
                    lDate = LastMov.MovDate;
                }
            }
            return lDate;
        }

        public static string MoveToAdministration(UserRightsToken pUr, int pAniId, int pFromFarmId, int pToFarmId,int changedby, int sourceid)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMAL aCheck = lMstb.GetAnimalById(pAniId);
            ANIMALCATEGORY aFrom = lMstb.GetAnimalCategoryByIdandFarmid(pAniId, pFromFarmId);
            ANIMALCATEGORY aTo = lMstb.GetAnimalCategoryByIdandFarmid(pAniId, pToFarmId);
            aFrom.Changed_By = changedby;
            aFrom.SourceID = sourceid;
            aTo.Changed_By = changedby;
            aTo.SourceID = sourceid;
            string ret = "";
            if (aFrom.FarmId > 0)
            {
                if (aTo.FarmId == 0)
                {
                    aTo.Ani_Mede_Eigenaar = aFrom.Ani_Mede_Eigenaar;
                    aTo.Anicategory = aFrom.Anicategory;
                    aTo.AniId = aFrom.AniId;
                    aTo.AniMinasCategory = aFrom.AniMinasCategory;
                    aTo.AniWorknumber = aFrom.AniWorknumber;
                    aTo.FarmId = pToFarmId;
                    if (lMstb.SaveAnimalCategory(aTo))
                    {
                        if (lMstb.DeleteAnimalCategory(aFrom))
                        {
                            ret = aCheck.AniLifeNumber + " administratie gewijzigd.";
                        }
                        else { ret = aCheck.AniLifeNumber + " oude registratie verwijderen niet gelukt."; }
                    }
                    else { ret = aCheck.AniLifeNumber + " registreren niet gelukt."; }
                }
                else { ret = aCheck.AniLifeNumber + " is al geregistreerd bij deze administratie."; }
            }
            else { ret = aCheck.AniLifeNumber + " hoort niet bij deze administratie."; }
            return ret;
        }

        public static void NummerDierOm(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, ANIMAL pAnimal, string pNewLifeNumber, string pLNVMeldNr, int pChangedBy, int pSourceID)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            string lOldChipnr = pAnimal.AniLifeNumber;
            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                lOldChipnr = pAnimal.AniAlternateNumber;
            }
            if (lOldChipnr.Length > 5)
            {
                //List<LIFENR> LIFENRnummers = lMstb.GetLifenummersByFarmId(pBedrijf.FarmId);

                //bool iseralin = false;
                //foreach (LIFENR ltemp in LIFENRnummers)
                //{
                //    if (lOldChipnr.Contains(ltemp.LifLifenr.Trim()))
                //    {
                //        iseralin = true;
                //        break;
                //    }
                //}
                //if (!iseralin)
                //{


                //    LIFENR lf = new LIFENR();
                //    lf.LifCountrycode = lOldChipnr.Substring(0, 3);
                //    lf.LifLifenr = lOldChipnr.Substring(3, lOldChipnr.Length - 3);
                //    lf.FarmNumber = pUbn.Bedrijfsnummer;
                //    lf.LifId = pBedrijf.FarmId;
                //    lf.Program = pBedrijf.Programid;

                //    int start = lOldChipnr.Length - 5;
                //    lf.LifSort = lOldChipnr.Substring(start, 4);
                //    //lMstb.InsertLifenr(lf);

                //}

            }
            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                pAnimal.AniAlternateNumber = pNewLifeNumber;
                if (pAnimal.AniLifeNumber == lOldChipnr)
                {
                    pAnimal.AniLifeNumber = pNewLifeNumber;
                }
            }
            else
            {
                pAnimal.AniLifeNumber = pNewLifeNumber;
                pAnimal.AniAlternateNumber = pNewLifeNumber;
            }
            pAnimal.Changed_By = pChangedBy;
            pAnimal.SourceID = pSourceID;
            if (lMstb.UpdateANIMAL(pUbn.ThrID, pAnimal))
            {

                List<LEVNRMUT> lnms = lMstb.getLevNrMuts(pAnimal.AniId);
                var lnmu = from n in lnms
                           where n.LevnrOud == lOldChipnr
                           && n.LevnrNieuw == pNewLifeNumber
                           select n;
                LEVNRMUT lnm = new LEVNRMUT();
                if (lnmu.Count() > 0)
                {
                    lnm = lnmu.ElementAt(0);
                }
                lnm.Aniid = pAnimal.AniId;
                lnm.Datum = DateTime.Now;
                lnm.LevnrNieuw = pNewLifeNumber;
                lnm.LevnrOud = lOldChipnr;
                lnm.LNVMeldNr = pLNVMeldNr;
                lnm.Changed_By = pChangedBy;
                lnm.SourceID = pSourceID;
                lMstb.saveLevNrMut(lnm);
                try
                {

                    lMstb.DeleteLifenr(pBedrijf.FarmId, pNewLifeNumber);
                }
                catch { }
                if (pBedrijf.Programid == 16000)
                {
                    int pTestserver = 1;//is niet relevant hier
                    MutationUpdater mu = new MutationUpdater(new XDocument(new XElement("root")), pTestserver, unLogger.getLogDir() + "NummerDierOm" + DateTime.Now.ToString("yyyyMMddHHmm") + ".log");
                    mu.checkForMsOptimaboxRespondernumber(pUr, pBedrijf, pAnimal, true);
                }
            }
        }

        public static string intrekDeleteMelding(UserRightsToken pUr, FARMCONFIG pFCIRviaModem, int pMovId)
        {
            string ret = "";
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            MUTALOG lIntrekmelding = lMstb.GetMutaLogByMovId(pMovId);
            MUTATION mutlIntrekmelding = lMstb.GetMutationByMovId(pMovId);

            bool isRemoved = false;
            if (mutlIntrekmelding.Internalnr > 0)
            {
                lMstb.DeleteMutation(mutlIntrekmelding);
                isRemoved = true;
            }
            if (lIntrekmelding.Internalnr > 0)
            {
                if (lIntrekmelding.Returnresult != 2)
                {
                    if (lIntrekmelding.CodeMutation < 100)//Anders is hij al ingetrokken verzonden
                    {
                        if (lIntrekmelding.Returnresult != 97)//Anders is  al ingetrokken via meldingen.aspx melding intrekken
                        {
                            lIntrekmelding.CodeMutation = lIntrekmelding.CodeMutation + 100;
                            mutlIntrekmelding = Facade.GetInstance().getMeldingen().ConverttoMutation(lIntrekmelding);
                            //lMstb.DeleteMutalog(lIntrekmelding);
                            //NIET DOEN
                            //want deze melding is gedaan 
                            //en de intrekmelding is wel voor deze melding maar is een nieuwe melding
                            lMstb.SaveMutation(mutlIntrekmelding);
                        }
                    }

                }
                else
                {
                    unLogger.WriteDebug("intrekkenMelding:" + pMovId.ToString() + " Deze  MUTALOG heeft Returnresult==2 ");
                }
            }
            else
            {
                if (pFCIRviaModem.FValue == "True")
                {
                    if (!isRemoved)
                    {
                        //Dan bij RVO verblijfplaatsen navragen (Dierdetails)
                        if (pMovId > 0)
                        {
                            MOVEMENT m = lMstb.GetMovementByMovId(pMovId);
                            BEDRIJF b = new BEDRIJF();
                            UBN u = new UBN();
                            THIRD t = new THIRD();
                            COUNTRY c = new COUNTRY();
                            lMstb.getCompanyByFarmId(pFCIRviaModem.FarmId, out b, out u, out t, out c);
                            if (t.ThrCountry.Trim() == "151")
                            {
                                Win32SOAPIRALG lDllCall = new Win32SOAPIRALG();
                                ANIMAL an = lMstb.GetAnimalById(m.AniId);
                                ANIMALCATEGORY ac = lMstb.GetAnimalCategoryByIdandFarmid(an.AniId, b.FarmId);
                                UBN lUbn2e = lMstb.GetubnById(m.MovThird_UBNid);
                                if (an.AniAlternateNumber == "")
                                {
                                    an.AniAlternateNumber = an.AniLifeNumber;
                                }

                                String Output = unLogger.getLogDir("CSVData") + u.Bedrijfsnummer + "_" + an.AniAlternateNumber.Replace(" ", "_") + "_Intrekmeldingzoeken_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                                String Logfile = unLogger.getLogDir("IenR") + u.Bedrijfsnummer + "_" + an.AniAlternateNumber.Replace(" ", "_") + "_Intrekmeldingzoeken_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                                
                                //SOAPLOG Result = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(pUr, b.UBNid,
                                //            b.ProgId, b.Programid, an.AniAlternateNumber, 0, 0,
                                //            "", m.MovDate.Date, m.MovDate.Date.AddMonths(6), 1, Output);

                                String BRSnr = t.Thr_Brs_Number;
                                string pBrsnummer = "";
                                FTPUSER fulnvsoap = lMstb.GetFtpuser(b.UBNid, b.Programid, b.ProgId, 9992, out pBrsnummer);
                                string Gebruikersnaam = fulnvsoap.UserName;
                                string Wachtwoord = fulnvsoap.Password;
                                string uBedrijfsnummer = u.Bedrijfsnummer;
                                string uBRSnr = BRSnr;
                                int pMaxStrLen = 255;
                                string datumtijd = DateTime.Now.ToString("yyyyMMddhhmmss");
                                if (Gebruikersnaam == String.Empty || Wachtwoord == String.Empty)
                                {
                                    //TODO make 1 function
                                    Gebruikersnaam = ConfigurationManager.AppSettings["LNVDierDetailsusername"];
                                    Wachtwoord = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
                                    uBedrijfsnummer = "";
                                    uBRSnr = "";
                                }
                                string lStatus = String.Empty;
                                string lCode = String.Empty;
                                string lOmschrijving = String.Empty;
                                lDllCall.LNVIRRaadplegenMeldingenAlgV2(Gebruikersnaam, Wachtwoord, 0,
                                    u.Bedrijfsnummer, BRSnr, an.AniAlternateNumber,
                                    b.ProgId, 0, 0, lUbn2e.Bedrijfsnummer,
                                    m.MovDate.Date, m.MovDate.Date.AddMonths(6), 1, 1, 0, Output,
                                    ref lStatus, ref lCode, ref lOmschrijving,
                                    Logfile, 255);


                                if (File.Exists(Output) && lOmschrijving=="")
                                {
                                
                                    string[] Kols = { "Levensnummer", "Meldingtype", "Datum", "ubn2ePartij", "meldingsnummer", "meldingstatus", "hersteld" };

                                    char spl = ';';
                                    DataTable tblLNV = utils.GetCsvData(Kols, Output, spl, "Mutaties");
                                    tblLNV.DefaultView.Sort = "Levensnummer,Datum,meldingsnummer";//Chronologisch volgens RVO
                                    tblLNV = tblLNV.DefaultView.ToTable(true);
                                    if (tblLNV.Rows.Count > 0)
                                    {
                                        mutlIntrekmelding = new MUTATION();
                                        lIntrekmelding = new MUTALOG();
                                        foreach (DataRow row in tblLNV.Rows)
                                        {
                                            int lMeldingType = Convert.ToInt32(row["Meldingtype"]);
                                            DateTime MutDate = utils.getDateLNV(row["Datum"].ToString());
                                            String UBN2e = row["ubn2ePartij"].ToString();
                                            string meldingsnummer = row["meldingsnummer"].ToString();
                                            string meldingstatus = row["meldingstatus"].ToString();//6 = Ingetrokken
                                            if (m.MovDate.Date == MutDate.Date)
                                            {
                                                /*
                                                    104 = Intrekken Afvoer
                                                    109 = Intrekken Export
                                                    106 = Intrekken Dood
                                                 */
                                                if (lMeldingType == 2 || lMeldingType == 6)
                                                {
                                                    if (m.MovKind == 2)
                                                    {
                                                        if (lMeldingType == 2)
                                                        {
                                                            if (meldingstatus != "6")//Anders is hij al ingetrokken via I&R melden bijv
                                                            {
                                                                mutlIntrekmelding.CodeMutation = 104;
                                                                mutlIntrekmelding.MeldingNummer = meldingsnummer;
                                                                mutlIntrekmelding.FarmNumberTo = UBN2e;
                                                            }
                                                            else
                                                            {
                                                                lIntrekmelding.CodeMutation = 104;
                                                                lIntrekmelding.MeldingNummer = meldingsnummer;

                                                            }

                                                        }
                                                        else
                                                        {
                                                            if (meldingstatus != "6")//Anders is hij al ingetrokken via I&R melden bijv
                                                            {
                                                                mutlIntrekmelding.CodeMutation = 109;
                                                                mutlIntrekmelding.MeldingNummer = meldingsnummer;
                                                                mutlIntrekmelding.FarmNumberTo = UBN2e;
                                                            }
                                                            else
                                                            {
                                                                lIntrekmelding.CodeMutation = 109;
                                                                lIntrekmelding.MeldingNummer = meldingsnummer;

                                                            }
                                                        }

                                                    }
                                                }
                                                else if (lMeldingType == 7)
                                                {
                                                    if (m.MovKind == 3)
                                                    {
                                                        if (meldingstatus != "6")//Anders is hij al ingetrokken via I&R melden bijv
                                                        {
                                                            mutlIntrekmelding.CodeMutation = 106;
                                                            mutlIntrekmelding.MeldingNummer = meldingsnummer;
                                                            mutlIntrekmelding.FarmNumberTo = UBN2e;
                                                        }
                                                        else
                                                        {
                                                            lIntrekmelding.CodeMutation = 106;
                                                            lIntrekmelding.MeldingNummer = meldingsnummer;

                                                        }
                                                    }
                                                }
                                            }
                                            else 
                                            {
                                             //dan is er op die datum geen melding 
                                                ret = "";
                                            }
                                            if (mutlIntrekmelding.CodeMutation > 0)
                                            {
                                                break;
                                            }
                                        }
                                        if (mutlIntrekmelding.CodeMutation > 0)
                                        {
                                            mutlIntrekmelding.AlternateLifeNumber = an.AniAlternateNumber;
                                            mutlIntrekmelding.Lifenumber = an.AniAlternateNumber;
                                            mutlIntrekmelding.CountryCodeBirth = an.AniCountryCodeBirth;
                                            mutlIntrekmelding.CountryCodeDepart = an.AniCountryCodeDepart;
                                            mutlIntrekmelding.Farmnumber = u.Bedrijfsnummer;
                                            mutlIntrekmelding.FarmNumberFrom = u.Bedrijfsnummer;
                                            mutlIntrekmelding.Nling = an.AniNling;
                                            mutlIntrekmelding.Sex = an.AniSex;
                                            mutlIntrekmelding.UbnId = u.UBNid;
                                            mutlIntrekmelding.ThrID = u.ThrID;
                                            mutlIntrekmelding.MutationDate = m.MovDate;
                                            mutlIntrekmelding.MutationTime = m.MovTime;
                                            mutlIntrekmelding.Worknumber = ac.AniWorknumber;

                                            RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                                            string defIenRaction = r.getdefIenRaction(pUr, b.UBNid, b.ProgId, b.Programid);
                                            FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(b.FarmId, "VerstuurIenR", defIenRaction);
                                            if (b.ProgId == 25)
                                            { mutlIntrekmelding.SendTo = 35; }
                                            else
                                            {
                                                mutlIntrekmelding.SendTo = IenRCom.ValueAsInteger();
                                            }

                                            lMstb.SaveMutation(mutlIntrekmelding);

                                        }
                                        else
                                        {
                                            if (lIntrekmelding.CodeMutation == 0)
                                            {
                                                //string reden = "Afvoer";
                                                //if (m.MovKind == 3)
                                                //{ reden = "Dood"; }
                                                //ret = " Het klaarzetten van een intrekmelding is niet gelukt, omdat er in de periode van "
                                                //  + m.MovDate.Date.ToString("dd-MM-yyyy") + " tot " +
                                                //  m.MovDate.AddMonths(1).Date.ToString("dd-MM-yyyy") + " geen " + reden + " meldingen zijn gevonden. ";
                                                /*
                                                        Wat is dit voor een vreemde melding ?

                                                        Een intrekmelding moet niet gelden voor een periode, enkel voor de ene dag.
                                                        Als de afvoermelding niet gevonden kan worden op die betreffende dag bij RVO, dan mag de afvoermelding in de Agrobase gewoon verwijderd worden.

                                                 */
                                            }
                                            else
                                            {
                                                unLogger.WriteDebug("Het klaarzetten van een intrekmelding is niet gedaan want MeldingNummer:" + lIntrekmelding.MeldingNummer + " heeft al de status 6 (ingetrokken)");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //ret = " Het klaarzetten van een intrekmelding is niet gelukt, omdat er in de periode van "
                                        //    + m.MovDate.Date.ToString("dd-MM-yyyy") + " tot " +
                                        //    m.MovDate.AddMonths(1).Date.ToString("dd-MM-yyyy") + " geen meldingen zijn gevonden. ";
                                    }
                                }
                                else
                                {
                                    if (lOmschrijving.ToLower().Contains("geen melding gevonden"))
                                    { }
                                    else
                                    {


                                        int LNVprognr = 0;
                                        string BRSnrHouder = String.Empty;
                                        string UBNhouder = String.Empty;
                                        string Werknummer = String.Empty;
                                        DateTime Geboortedat = DateTime.MinValue;
                                        DateTime Importdat = DateTime.MinValue;
                                        string LandCodeHerkomst = String.Empty;
                                        string LandCodeOorsprong = String.Empty;
                                        string Geslacht = String.Empty;
                                        string Haarkleur = String.Empty;
                                        DateTime Einddatum = DateTime.MinValue;
                                        string RedenEinde = String.Empty;
                                        string LevensnrMoeder = String.Empty;
                                        string VervangenLevensnr = String.Empty;
                                        string Status = String.Empty;
                                        string Code = String.Empty;
                                        string Omschrijving = String.Empty;
                                        string pLogfile = unLogger.getLogDir("IenR") + u.Bedrijfsnummer + "_LNVDierdetailsV2_" + an.AniAlternateNumber + "_" + datumtijd + ".log";
                                        string pDetailfile = unLogger.getLogDir("IenR") + u.Bedrijfsnummer + "_LNVDierdetailsV2_" + an.AniAlternateNumber + "_D_" + datumtijd + ".log";

                                        //1;ubn;aanvoerdatum;afvoerdatum;adres;postcode;wooplaats;bedrijfstype

                                        lDllCall.LNVDierdetailsV2(Gebruikersnaam, Wachtwoord, 0, uBedrijfsnummer,
                                                     uBRSnr, an.AniAlternateNumber, b.ProgId, 1, 0, 0, ref LNVprognr, ref Werknummer,
                                        ref Geboortedat, ref Importdat,
                                        ref LandCodeHerkomst, ref LandCodeOorsprong,
                                        ref Geslacht, ref Haarkleur,
                                        ref Einddatum, ref RedenEinde,
                                        ref LevensnrMoeder, ref VervangenLevensnr,
                                        ref Status, ref  Code, ref Omschrijving,
                                        pDetailfile, pLogfile, pMaxStrLen);
                                        if (Omschrijving=="" && Einddatum > DateTime.MinValue && m.MovDate.Date == Einddatum.Date && m.MovKind==(int)CORE.DB.LABELSConst.MovKind.Loss)
                                        {
                                            //Als er meerdere doodmovements zijn op die dag mogen ze verwijderd worden op 1 na.
                                            List<MOVEMENT> doden = lMstb.GetMovementsByAniIdMovkind(m.AniId, (int)CORE.DB.LABELSConst.MovKind.Loss);
                                            var aantal = from n in doden where n.MovDate.Date == m.MovDate.Date select n;
                                            if (aantal.Count() == 1)
                                            {
                                                return an.AniAlternateNumber + " is op " + Einddatum.ToString("dd-MM-yyyy") + " Doodverklaard";
                                            }
                                        }
                                        //else if (Status == "F" && Code == "IRD-00192")//dit geld hier niet, 
                                        //{
                                        //    return "";//wel contact maar niets gevonden, geld ook voor afvoeren
                                        //}
                                        else if (Omschrijving == "" && Einddatum > DateTime.MinValue && m.MovDate.Date != Einddatum.Date && m.MovKind == (int)CORE.DB.LABELSConst.MovKind.Loss)
                                        {
                                            
                                        }
                                         
                                        if (File.Exists(pDetailfile) && (Status.ToUpper()=="G" || Status.ToUpper()=="W"))
                                        {
                                            char[] spl = { ';' };
                                            StreamReader rdr = new StreamReader(pDetailfile);

                                            try
                                            {
                                                ret = "";
                                                string strLine = "";
                                                DateTime afvoerdatum = DateTime.MinValue;
                                                while ((strLine = rdr.ReadLine()) != null)
                                                {
                                                    string[] line = strLine.Split(spl);
                                                    int lengte = line.Length;

                                                    if (lengte > 1)
                                                    {
                                                        if (line[0] == "1")
                                                        {
                                                            if (line[1] == u.Bedrijfsnummer)
                                                            {
                                                                if (line[3] != "")
                                                                {
                                                                    afvoerdatum = utils.getDateLNV(line[3]);
                                                                    if (afvoerdatum.Date == m.MovDate.Date)
                                                                    {
                                                                        List<MOVEMENT> doden = lMstb.GetMovementsByAniIdMovkind(m.AniId, (int)CORE.DB.LABELSConst.MovKind.Sale);
                                                                        var aantal = from n in doden where n.MovDate.Date == m.MovDate.Date && (n.UbnId==m.UbnId || n.UbnId==0) select n;
                                                                        if (aantal.Count() == 1)
                                                                        {
                                                                            return an.AniAlternateNumber + " staat op " + m.MovDate.Date.ToString("dd-MM-yyyy") + " Afgevoerd bij RVO. ";
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch { }
                                            finally { rdr.Close(); }
                                        }
                                        else
                                        {
                                            ret = Omschrijving + " Probeer het later nogmaals.";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return ret;
        }

        public static void TEMPCorrigeerMSoptimaboxResponders(UserRightsToken pUr,BEDRIJF pBedrijf)
        {
            
            /*
                Correctie functie om de transmitters terug te zetten naar de transmstock van bedrijven
                van dieren die afgevoerd zijn
             */
            if (pBedrijf.Programid == 16000)
            {
                try
                {
                    AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB((DBConnectionToken)pUr.Clone());
                    List<BEDRIJF> bedrijven = new List<BEDRIJF>();
                    StringBuilder bld1 = new StringBuilder();
                    bld1.Append(@" SELECT * FROM agrofactuur.BEDRIJF b where b.programid=16000 and b.FarmId > 0 ");

                    DataTable tbl1 = lMstb.GetDataBase().QueryData(pUr, bld1, MissingSchemaAction.Add);
                    foreach (DataRow rw1 in tbl1.Rows)
                    {
                        BEDRIJF bed = new BEDRIJF();
                        lMstb.GetDataBase().FillObject(bed, rw1);
                        bedrijven.Add(bed);
                    }
                    int teller = 0;
                    unLogger.WriteInfo("START teller:" + teller.ToString());
                    //bedrijven.Clear();
                    //bedrijven.Add(pBedrijf);
                    foreach (BEDRIJF lBedrijf in bedrijven)
                    {
                        StringBuilder bld = new StringBuilder();
                        bld.AppendFormat(@"
                        SELECT  a.AniLifenumber,ac.Farmid,ac.AniCategory ,t.*    
                        FROM agrobase.ANIMALCATEGORY ac
                        JOIN agrobase.TRANSMIT t ON t.AniID=ac.AniID AND t.FarmId=ac.FarmID
                        JOIN agrofactuur.BEDRIJF b ON b.FarmId=ac.FarmID  and b.Programid=16000 
                        JOIN agrobase.ANIMAL a ON a.AniID=ac.AniID
                        where ac.Anicategory=4   
						AND t.ProcesComputerId like '2301%'  AND ac.FarmId={0}
                ", lBedrijf.FarmId);

                        DataTable tbl = lMstb.GetDataBase().QueryData(pUr, bld, MissingSchemaAction.Add);
                        if (tbl.Rows.Count > 0)
                        {
                            foreach (DataRow rw in tbl.Rows)
                            {
                                unLogger.WriteInfo(" BEDRIJF " + lBedrijf.FarmId.ToString() + " " + lBedrijf.Omschrijving + " AniID:" + rw["AniID"].ToString() + " AniLifenumber:" + rw["AniLifenumber"].ToString());
                                teller += 1;
                                returnTransmitters(pUr, lBedrijf, int.Parse(rw["AniID"].ToString()));
                            }
                        }
                        else
                        {
                            unLogger.WriteInfo(" BEDRIJF " + lBedrijf.FarmId.ToString() + " " + lBedrijf.Omschrijving + "  geen transmitters");

                        }
                    }
                    unLogger.WriteInfo("KLAAR teller:" + teller.ToString());

                }
                catch (Exception exc) { unLogger.WriteInfo("KLAAR  :" + exc.ToString()); }
            }
        }

        public static void onbekendstamboekdierBeltesMessage(UserRightsToken pUr, int pThrID, int pChanged_By, string pUbn, string pAniAlternatenumber)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB((DBConnectionToken)pUr);
            MESSAGES m = new MESSAGES();
            m.Changed_By = pChanged_By;
            m.MesBegin_DateTime = DateTime.Now;
            m.MesEnd_DateTime = DateTime.Now.AddMonths(1);
            m.MesProgramID = 160;
            //m.MesVersion = Facade.GetInstance().getVersion().ToString();
            m.SourceID = pThrID;
            m.MesMessage = "UBN:" + pUbn + " Aanvoerpoging:" + pAniAlternatenumber + " :Onbekend stamboek dier ";
            List<MESSAGES> ber = lMstb.getMessagesForAdminUse(160);
            var uniek = from n in ber where n.MesMessage == m.MesMessage select n;
            if (uniek.Count() == 0)
            {
                lMstb.SaveMessage(m);
            }
        }
    }

    public class MovFuncEvent : EventArgs
    {
        public int Progress;
        public string Message;
    }

    public class MovFuncUpdater
    {

        private string pLogFileUpdater;
        public MovFuncUpdater(string pLogFileForUpdater)
        {
            pLogFileUpdater = pLogFileForUpdater;
        }

        private void writeLog(string pText)
        {
            try
            {
                if (pLogFileUpdater.Length > 0)
                {
                    StreamWriter wr = new StreamWriter(pLogFileUpdater, true);
                    try
                    {
                        wr.WriteLine(pText);
                        wr.Flush();
                    }
                    catch (Exception exc) { unLogger.WriteDebug("A:" + exc.ToString()); }
                    finally
                    {
                        wr.Close();
                    }
                }
            }
            catch (Exception exc) { unLogger.WriteDebug("B:" + exc.ToString()); }
        }

        public event EventHandler RequestUpdate;

        protected void OnRequestUpdate(object sender, MovFuncEvent e)
        {
            if (RequestUpdate != null)
                RequestUpdate(sender, e);
        }

        //private void setHairColors(UserRightsToken pUr)
        //{
        //    lHairColors = Facade.GetInstance().getSaveToDB(pUr).GetLabels(16, 528);
        //}

        public void VerwerkVerblijfplaatsen(UserRightsToken pUr, BEDRIJF pBedrijf, UBN u, THIRD pThird, XDocument pDierfileXml, DateTime pBegindatum, DateTime pEinddatum, string pResultPath, int pChangedBy, int pSourceID)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            List<ANIMALCATEGORY> lAniCats = lMstb.GetAnimalCategoryByFarmId(pBedrijf.FarmId);
            //if (lHairColors == null || lHairColors.Count() == 0)
            //{
            //    setHairColors(pUr);
            //}
            //int standaardgeslacht = 0;
            //int.TryParse(lMstb.GetFarmConfigValue(pBedrijf.FarmId, "standaardgeslacht", "0"), out standaardgeslacht);
            //if (standaardgeslacht == 0)
            //{
            //    int.TryParse(Facade.GetInstance().getSaveToDB(pUr).GetProgramConfigValue(pBedrijf.Programid, "standaardgeslacht", "0"), out standaardgeslacht);

            //}
            int teller = 1;
            int totaal = pDierfileXml.Root.Elements().Count();
            foreach (XElement xAnimal in pDierfileXml.Root.Elements())
            {
                string Lifenumber = "";
                if (xAnimal.Attribute("AniLifeNumber") != null)
                {
                    Lifenumber = xAnimal.Attribute("AniLifeNumber").Value;

                    int procent = teller * 100 / totaal;

                    try
                    {
                        if (RequestUpdate != null)
                        {
                            MovFuncEvent b = new MovFuncEvent();
                            b.Progress = procent;
                            b.Message = " " + VSM_Ruma_OnlineCulture.getStaticResource("verwerken", "Verwerken") + " " + Lifenumber;
                            RequestUpdate(this, b);
                        }
                        DateTime pGeboortedatum = DateTime.MinValue;
                        XElement Geboortedat = xAnimal.Element("Geboortedat");
                        if (Geboortedat != null)
                        {
                            pGeboortedatum = utils.getDateFormat(Geboortedat.Value);
                        }
                        string pHaarkleur = "";
                        XElement Haarkleur = xAnimal.Element("Haarkleur");
                        if (Haarkleur != null)
                        {
                            pHaarkleur = Haarkleur.Value;
                        }
                        int pAniSex = 0;// standaardgeslacht;
                        XElement Geslacht = xAnimal.Element("Geslacht");
                        if (Geslacht != null)
                        {
                            if (Geslacht.Value.ToLower() == "m")
                            { pAniSex = 1; }
                            else if (Geslacht.Value.ToLower() == "v")
                            { pAniSex = 2; }
                        }


                        DateTime pEindDatum = DateTime.MinValue;
                        XElement Einddatum = xAnimal.Element("Einddatum");
                        if (Einddatum != null)
                        {
                            pEindDatum = utils.getDateFormat(Einddatum.Value);
                        }
                        DateTime pImportdat = DateTime.MinValue;
                        XElement xImportdat = xAnimal.Element("Importdat");
                        if (xImportdat != null)
                        {
                            pImportdat = utils.getDateFormat(xImportdat.Value);
                        }

                        ANIMAL lAnimal = lMstb.GetAnimalByLifenr(Lifenumber);
                        if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                        {
                            lAnimal = lMstb.GetAnimalByAniAlternateNumber(Lifenumber);
                        }

                        if (lAnimal.AniId == 0)
                        {
                            XElement xVervangenLevensnr = xAnimal.Element("VervangenLevensnr");
                            if (xVervangenLevensnr != null && xVervangenLevensnr.Value != "")
                            {
                                checkVervangenLevensnrDier(pUr, pBedrijf, u, pThird, xVervangenLevensnr.Value, Lifenumber, pChangedBy, pSourceID);
                                lAnimal = lMstb.GetAnimalByLifenr(Lifenumber);
                                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                                {
                                    lAnimal = lMstb.GetAnimalByAniAlternateNumber(Lifenumber);
                                }
                            }

                        }

                        if (lAnimal.AniId > 0)
                        {
                            if (lAnimal.AniBirthDate == DateTime.MinValue || lAnimal.AniBirthDate.Year < 1901)
                            {
                                if (pGeboortedatum > DateTime.MinValue)
                                {
                                    lAnimal.AniBirthDate = pGeboortedatum;

                                    lMstb.UpdateANIMAL(pThird.ThrId, lAnimal);
                                }
                            }
                            if (lAnimal.AniHaircolor_Memo == "")
                            {
                                if (pHaarkleur != "")
                                {
                                    lAnimal.AniHaircolor_Memo = pHaarkleur;

                                    lMstb.UpdateANIMAL(pThird.ThrId, lAnimal);
                                }
                            }
                            if (lAnimal.AniSex == 0 && pAniSex > 0)
                            {
                                lAnimal.AniSex = pAniSex;
                                lMstb.UpdateANIMAL(pThird.ThrId, lAnimal);
                            }
                            if (lAnimal.AniAlternateNumber == "")
                            {
                                lAnimal.AniAlternateNumber = Lifenumber;
                                lMstb.UpdateANIMAL(pThird.ThrId, lAnimal);
                            }
                            ANIMALCATEGORY acat = lAniCats.Find(delegate(ANIMALCATEGORY p) { return p.AniId == lAnimal.AniId; });
                            if (acat == null)
                            {
                                acat = new ANIMALCATEGORY();
                                acat.FarmId = pBedrijf.FarmId;
                                acat.AniId = lAnimal.AniId;
                                acat.Anicategory = 3;
                                string lTempnr = Lifenumber;
                                if (lTempnr.Length > 5)
                                {
                                    acat.AniWorknumber = lTempnr.Substring(lTempnr.Length - 5, 5);
                                }

                            }
                            List<MOVEMENT> AnimalMovs = lMstb.GetMovementsByAniId(lAnimal.AniId);
                            var varAnimalMovs = from n in AnimalMovs
                                                where n.UbnId == u.UBNid || n.UbnId == 0
                                                select n;
                            //Als het dier aanwezig is dan altijd controleren
                            //Anders alleen de Movents binnen de Controleperiode
                            //maar dat moet al in de Query zitten die de dieren hiervoor kiest 

                            checkMovements(lMstb, pUr, varAnimalMovs.ToList(), lAnimal, xAnimal.Element("Verblijfplaatsen"), u, pBedrijf, pThird, acat, pEindDatum, pGeboortedatum, pImportdat,pChangedBy,pSourceID);
                            XElement xNakomelingen = xAnimal.Element("Nakomelingen");
                            if (xNakomelingen != null)
                            {
                                if (xNakomelingen.HasElements)
                                {
                                    List<BIRTH> animalBirths = lMstb.GetBirthsByAnimal(lAnimal.AniId);
                                    checkAndSaveNakomelingen(pUr, lAnimal, xAnimal, animalBirths, u, pBedrijf, pThird, pThird.ThrId);
                                }
                            }
                        }
                        else
                        {
                            //TEST voor sanitrace AniIds zijn 0
                            //checkMovements(lMstb, pUr, new List<MOVEMENT>(), AniId, xAnimal.Element("Verblijfplaatsen"), u, pBedrijf,pThird, new ANIMALCATEGORY(), pEindDatum, pGeboortedatum);

                        }

                    }
                    catch (Exception exc) { unLogger.WriteInfo(exc.ToString()); }
                    teller += 1;

                }
                //TEMP TEST
                //break;
            }
            pDierfileXml.Save(pResultPath);
        }

        public void checkAndSaveNakomelingen(UserRightsToken pUr, ANIMAL pAnimal, XElement xAnimal, List<BIRTH> pAnimalBirths, UBN pUbn, BEDRIJF pBedrijf, THIRD pThird, int pOpslaThird)
        {
            if (pAnimal.AniSex == 2)
            {
                writeLog(" VerwerkNakomelingen :" + pAnimal.AniLifeNumber + " ;" + pAnimal.AniAlternateNumber);
            }
            else
            {

                writeLog("NIET VerwerkNakomelingen dit is een Mannelijkdier :" + pAnimal.AniLifeNumber + " ;" + pAnimal.AniAlternateNumber);
            }
            if (pAnimal.AniSex == 2)
            {
                if (xAnimal != null)
                {
                    XElement xNakomelingen = xAnimal.Element("Nakomelingen");
                    if (xNakomelingen != null)
                    {
                        AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
                        foreach (XElement xNakomeling in xNakomelingen.Elements())
                        {
                            /* <Nakomeling>
                                  <levensnr>NL 536337814</levensnr> 
                                  <geboortedatum>20090511</geboortedatum> 
                                  <geslacht>M</geslacht> 
                                  <haarkleur>ZB</haarkleur> 
                               </Nakomeling>
                            */
                            if (xNakomeling.Element("levensnr") != null && xNakomeling.Element("geboortedatum") != null && xNakomeling.Element("geslacht") != null && xNakomeling.Element("haarkleur") != null)
                            {
                                string lLifenr = xNakomeling.Element("levensnr").Value;
                                DateTime lGebDatum = utils.getDateLNV(xNakomeling.Element("geboortedatum").Value);
                                int lAniSex = 2;
                                if (xNakomeling.Element("geslacht").Value.ToLower() == "m")
                                {
                                    lAniSex = 1;
                                }
                                string lHairColor = xNakomeling.Element("haarkleur").Value;
                                //if (xNakomeling.Element("haarkleur").Value != "")
                                //{
                                //    var h = from n in pHairColors16
                                //            where n.LabLabel == xNakomeling.Element("haarkleur").Value
                                //            select n;
                                //    if (h.Count() > 0)
                                //    {
                                //        lHairColor = h.ElementAt(0).LabId;
                                //    }
                                //}
                                ANIMAL lChildAnimal = new ANIMAL();
                                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                                {
                                    lChildAnimal = lMstb.GetAnimalByAniAlternateNumber(lLifenr);
                                }
                                else
                                {
                                    lChildAnimal = lMstb.GetAnimalByLifenr(lLifenr);
                                }
                                bool addbirth = true;
                                if (lChildAnimal.AniId > 0)
                                {

                                    var check = from n in pAnimalBirths
                                                where n.CalfId == lChildAnimal.AniId
                                                select n;
                                    if (check.Count() > 0)
                                    {
                                        addbirth = false;
                                        if (lChildAnimal.AniIdMother != pAnimal.AniId)
                                        {
                                            lChildAnimal.AniIdMother = pAnimal.AniId;
                                            lMstb.UpdateANIMAL(pOpslaThird, lChildAnimal);
                                        }
                                    }
                                }
                                if (addbirth)
                                {
                                    UBN lFokker = new UBN();
                                    THIRD lFokkerThird = new THIRD();
                                    XElement xVerblijfplaatsen = xAnimal.Element("Verblijfplaatsen");
                                    if (xVerblijfplaatsen != null)
                                    {
                                        try
                                        {
                                            foreach (XElement xVerblijfplaats in xVerblijfplaatsen.Elements())
                                            {
                                                DateTime aanvoer = utils.getDateLNV(xVerblijfplaats.Element("aanvoerdatum").Value);
                                                DateTime afvoer = DateTime.MinValue;
                                                if (xVerblijfplaats.Element("afvoerdatum").Value != "")
                                                {
                                                    afvoer = utils.getDateLNV("afvoerdatum");
                                                }
                                                if (aanvoer <= lGebDatum)
                                                {
                                                    if (afvoer > DateTime.MinValue)
                                                    {
                                                        if (lGebDatum <= afvoer)
                                                        {
                                                            getCompany(lMstb, xVerblijfplaats, out lFokker, out lFokkerThird);
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        getCompany(lMstb, xVerblijfplaats, out lFokker, out lFokkerThird);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    List<EVENT> lFatherEvents = new List<EVENT>();
                                    int lFatherId = 0;
                                    int pPmsgDekking = 0;
                                    Checker.checkaflamming(pUr, pAnimal, pBedrijf, lGebDatum, 5, out lFatherEvents, out lFatherId, out pPmsgDekking);


                                    if (lChildAnimal.AniId <= 0)
                                    {
                                        lChildAnimal = new ANIMAL();
                                        lChildAnimal.AniLifeNumber = lLifenr;
                                        lChildAnimal.AniAlternateNumber = lLifenr;
                                        lChildAnimal.ThrId = lFokker.ThrID;
                                        lChildAnimal.AniHaircolor_Memo = lHairColor;
                                        lChildAnimal.AniIdMother = pAnimal.AniId;
                                        lChildAnimal.AniIdFather = lFatherId;

                                    }
                                    if (lChildAnimal.AniIdMother != pAnimal.AniId)
                                    {
                                        lChildAnimal.AniIdMother = pAnimal.AniId;

                                    }
                                    if (lFatherId > 0)
                                    {
                                        if (lChildAnimal.AniIdFather <= 0)
                                        {
                                            lChildAnimal.AniIdFather = lFatherId;
                                        }
                                    }
                                    if (lChildAnimal.AniId > 0)
                                    {
                                        lMstb.UpdateANIMAL(pOpslaThird, lChildAnimal);
                                    }
                                    else
                                    {
                                        lMstb.SaveAnimal(pOpslaThird, lChildAnimal);
                                    }
                                    writeLog("added new ANIMAL  AniId:" + lChildAnimal.AniId.ToString());
                                    EVENT lEvBirth = new EVENT();
                                    lEvBirth.AniId = pAnimal.AniId;
                                    lEvBirth.EveKind = 5;
                                    lEvBirth.EveDate = lGebDatum;
                                    Event_functions.handleEventTimes(ref lEvBirth, lGebDatum);

                                    lEvBirth.UBNId = lFokker.UBNid;//want daar was de moeder op dat moment
                                    lEvBirth.EveOrder = Event_functions.getNewEventOrder(pUr, lGebDatum.Date, 5, lFokker.UBNid, pAnimal.AniId);
                                    BIRTH lBirh = new BIRTH();
                                    lBirh.AniFatherID = lChildAnimal.AniIdFather;
                                    lBirh.BirNumber = Event_functions.getOrCreateBirnr(pUr, pAnimal.AniId, lGebDatum);
                                    lBirh.BornDead = 0;
                                    lBirh.CalfId = lChildAnimal.AniId;
                                    if (lMstb.SaveEvent(lEvBirth))
                                    {
                                        writeLog("added new EVENT  EveId:" + lEvBirth.EventId.ToString() + " EveKind:" + lEvBirth.EveKind.ToString());
                                        lBirh.EventId = lEvBirth.EventId;
                                        if (lMstb.SaveBirth(lBirh))
                                        {
                                            writeLog("added new BIRTH  EveId:" + lEvBirth.EventId.ToString());
                                        }
                                        Event_functions.setNling(pOpslaThird, pUr, lFokker.UBNid, pAnimal.AniId, lBirh.BirNumber);
                                    }


                                }
                            }
                        }
                    }
                }
            }
        }

        public void checkMovements(AFSavetoDB lMstb, UserRightsToken pUr, List<MOVEMENT> pAnimalMovs, ANIMAL pAnimal, XElement xVerblijfplaatsen, UBN pUbn, BEDRIJF pBedrijf, THIRD pThird, ANIMALCATEGORY pAnicat, DateTime pEindDatum, DateTime pGeboortedatum, DateTime pImportDatum,int pChangedBy,int pSourceId)
        {
            writeLog(" Verwerkverblijfplaatsen :" + pAnimal.AniLifeNumber + " ;" + pAnimal.AniAlternateNumber);
            List<MovClass> movs = new List<MovClass>();
            int aantalaanpassingen = 0;
            string land = "NL";
            XElement AgrobaseResultaat = new XElement("AgrobaseResultaat");
            //Eerst alles verzamelen wat op het eigen UBN slaat en wat een eventuele movement zou kunnen zijn
            foreach (XElement Verblijfplaats in xVerblijfplaatsen.Elements())
            {
                if (Verblijfplaats.Name == "Verblijfplaats")
                {
                    XElement xUbn = Verblijfplaats.Element("inrichtingsnr");
                    if (xUbn == null)
                    {


                        XAttribute xCHRNumber = Verblijfplaats.Attribute("CHRNumber");
                        XAttribute xHtier = Verblijfplaats.Attribute("Htier");
                        if (xCHRNumber == null && xHtier == null)
                        {
                            //herstel
                            //NEDERLAND RVO

                            xUbn = Verblijfplaats.Element("ubn");

                            XElement xaanvoerdatum = Verblijfplaats.Element("aanvoerdatum");
                            DateTime aanvoerdatum = DateTime.MinValue;
                            if (xaanvoerdatum != null)
                            {
                                aanvoerdatum = utils.getDateLNV(xaanvoerdatum.Value);
                            }
                            XElement xafvoerdatum = Verblijfplaats.Element("afvoerdatum");
                            DateTime afvoerdatum = DateTime.MinValue;
                            if (xafvoerdatum != null)
                            {
                                afvoerdatum = utils.getDateLNV(xafvoerdatum.Value);
                            }
                            if (xUbn != null && xUbn.Value.Trim().Length > 0)
                            {
                                if (xUbn.Value == pUbn.Bedrijfsnummer)
                                {
                                    MovClass bemovaanvoer = new MovClass();
                                    bemovaanvoer.lMovement.MovKind = 1;
                                    bemovaanvoer.lMovement.AniId = pAnimal.AniId;
                                    MovClass bemovafvoer = new MovClass();
                                    bemovafvoer.lMovement.MovKind = 2;
                                    bemovafvoer.lMovement.AniId = pAnimal.AniId;


                                    if (aanvoerdatum > DateTime.MinValue)
                                    {
                                        bemovaanvoer.HandleMovTimes(pUr, aanvoerdatum);

                                    }
                                    if (afvoerdatum > DateTime.MinValue)
                                    {
                                        if (pEindDatum.Date == afvoerdatum)
                                        {
                                            if (xUbn.Value == pUbn.Bedrijfsnummer)
                                            {
                                                if (Verblijfplaats.ElementsAfterSelf("Verblijfplaats").Count() == 0)
                                                {
                                                    bemovafvoer.lMovement.MovKind = 3;
                                                }
                                            }
                                        }
                                        bemovafvoer.HandleMovTimes(pUr, afvoerdatum);
                                    }



                                    bemovafvoer.lMovement.UbnId = pUbn.UBNid;
                                    bemovafvoer.lMovement.Progid = pBedrijf.ProgId;
                                    bemovafvoer.lMovement.happened_at_FarmID = pBedrijf.FarmId;
                                    bemovafvoer.lMovement.ThrId = pUbn.ThrID;
                                    bemovafvoer.lMovement.Changed_By = pChangedBy;
                                    bemovafvoer.lMovement.SourceID = pSourceId;

                                    bemovaanvoer.lMovement.UbnId = pUbn.UBNid;
                                    bemovaanvoer.lMovement.Progid = pBedrijf.ProgId;
                                    bemovaanvoer.lMovement.happened_at_FarmID = pBedrijf.FarmId;
                                    bemovaanvoer.lMovement.ThrId = pUbn.ThrID;
                                    bemovaanvoer.lMovement.Changed_By = pChangedBy;
                                    bemovaanvoer.lMovement.SourceID = pSourceId;

                                    if (Verblijfplaats.ElementsBeforeSelf("Verblijfplaats").Count() > 0)
                                    {
                                        XElement before = Verblijfplaats.ElementsBeforeSelf("Verblijfplaats").Last();
                                        if (before != null)
                                        {
                                            UBN u = new UBN();
                                            THIRD t = new THIRD();
                                            getCompany(lMstb, before, out u, out t);
                                            bemovaanvoer.lMovement.MovThird_UBNid = u.UBNid;
                                            bemovaanvoer.lBuying.PurOrigin = t.ThrId;
                                        }
                                    }
                                    if (Verblijfplaats.ElementsAfterSelf("Verblijfplaats").Count() > 0)
                                    {
                                        XElement after = Verblijfplaats.ElementsAfterSelf("Verblijfplaats").First();
                                        if (after != null)
                                        {
                                            UBN u = new UBN();
                                            THIRD t = new THIRD();
                                            getCompany(lMstb, after, out u, out t);
                                            bemovafvoer.lMovement.MovThird_UBNid = u.UBNid;
                                            bemovafvoer.lSale.SalDestination = t.ThrId;
                                        }
                                    }

                                    //else
                                    //{
                                    //    UBN lOtherUbn = new UBN();// lMstb.getUBNByBedrijfsnummer(xUbn.Value.Trim());
                                    //    THIRD lOtherThird = new THIRD();
                                    //    getCompany(lMstb, Verblijfplaats, out lOtherUbn, out lOtherThird);


                                    //    bemovafvoer.lMovement.UbnId = lOtherUbn.UBNid;
                                    //    bemovafvoer.lMovement.Progid = pBedrijf.ProgId;
                                    //    bemovafvoer.lMovement.happened_at_FarmID = pBedrijf.FarmId;
                                    //    bemovafvoer.lMovement.ThrId = lOtherUbn.ThrID;
                                    //    bemovaanvoer.lMovement.UbnId = lOtherUbn.UBNid;
                                    //    bemovaanvoer.lMovement.Progid = pBedrijf.ProgId;
                                    //    bemovaanvoer.lMovement.happened_at_FarmID = pBedrijf.FarmId;
                                    //    bemovaanvoer.lMovement.ThrId = lOtherUbn.ThrID;



                                    //}
                                    if (bemovafvoer.lMovement.MovDate.Date > DateTime.MinValue)
                                    {
                                        if (xUbn.Value == pUbn.Bedrijfsnummer)
                                        {
                                            movs.Add(bemovafvoer);
                                        }

                                    }
                                    if (bemovaanvoer.lMovement.MovDate.Date > DateTime.MinValue)
                                    {
                                        if (pGeboortedatum.Date != bemovaanvoer.lMovement.MovDate.Date)
                                        {
                                            if (xUbn.Value == pUbn.Bedrijfsnummer)
                                            {
                                                if (pImportDatum > DateTime.MinValue)
                                                {
                                                    if (bemovaanvoer.lMovement.MovDate.Date == pImportDatum.Date)
                                                    {
                                                        bemovaanvoer.lBuying.PurKind = 1;
                                                    }
                                                }
                                                movs.Add(bemovaanvoer);
                                            }
                                        }
                                        else
                                        {
                                            if (bemovaanvoer.lMovement.ThrId == pUbn.ThrID)
                                            {
                                                if (Verblijfplaats.ElementsBeforeSelf("Verblijfplaats").Count() == 0)
                                                {
                                                    if (pAnimal.ThrId <= 0)
                                                    {
                                                        pAnimal.ThrId = pUbn.ThrID;
                                                        lMstb.UpdateANIMAL(pUbn.ThrID, pAnimal);
                                                    }
                                                    //ivm setandsaveanimalcategory
                                                }
                                                else
                                                {
                                                    if (xUbn.Value == pUbn.Bedrijfsnummer)
                                                    {
                                                        movs.Add(bemovaanvoer);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                        }
                        else if (xCHRNumber != null)
                        {
                            land = "DK";//DCF denemarken
                            /*
                             Voorbeeld
                                 <Verblijfplaatsen CHRnumber="Bedrijfsnummer">
                                     <Verblijfplaats CHRNumber="24527">
                                        <ubn>24527</ubn>
                                        <geboortedatum>20090310</geboortedatum>
                                      </Verblijfplaats>
                                      <Verblijfplaats CHRNumber="24527">
                                        <AnimalTransfer>
                                          <ubn>24527</ubn>
                                          <ubnvantot>28203</ubnvantot>
                                          <afvoerdatum>20120704</afvoerdatum>
                                        </AnimalTransfer>
                                      </Verblijfplaats>
                                      <Verblijfplaats CHRNumber="24527">
                                        <AnimalTransfer>
                                          <ubn>24527</ubn>
                                          <ubnvantot>28203</ubnvantot>
                                          <aanvoerdatum>20120707</aanvoerdatum>
                                        </AnimalTransfer>
                                      </Verblijfplaats>
                                   </Verblijfplaatsen>
                             
                             */
                            if (Verblijfplaats.Element("AnimalTransfer") != null)
                            {
                                XElement xAnimalTransfer = Verblijfplaats.Element("AnimalTransfer");
                                string ubn = xAnimalTransfer.Element("ubn").Value;
                                if (ubn == pUbn.Bedrijfsnummer)
                                {
                                    string ubnvantot = "";
                                    UBN tegenpartij = new UBN();
                                    DateTime MovDate = new DateTime();
                                    int lMovkind = 0;
                                    if (xAnimalTransfer.Element("ubnvantot") != null)
                                    {
                                        ubnvantot = xAnimalTransfer.Element("ubnvantot").Value;
                                        if (!String.IsNullOrEmpty(ubnvantot))
                                        {
                                            tegenpartij = lMstb.getUBNByBedrijfsnummer(ubnvantot);
                                            if (tegenpartij.UBNid == 0)
                                            {

                                            }
                                        }
                                    }
                                    if (xAnimalTransfer.Element("aanvoerdatum") != null)
                                    {
                                        MovDate = utils.getDateLNV(xAnimalTransfer.Element("aanvoerdatum").Value);
                                        lMovkind = 1;
                                        pAnicat.Anicategory = 3;
                                    }
                                    else if (xAnimalTransfer.Element("afvoerdatum") != null)
                                    {
                                        MovDate = utils.getDateLNV(xAnimalTransfer.Element("afvoerdatum").Value);
                                        lMovkind = 2;
                                        pAnicat.Anicategory = 4;
                                    }
                                    if (lMovkind > 0 && MovDate > DateTime.MinValue)
                                    {
                                        var savedmov = from n in pAnimalMovs where n.MovDate.Date == MovDate.Date && n.MovKind == lMovkind && n.UbnId == pUbn.UBNid select n;
                                        if (savedmov.Count() == 0)
                                        {
                                            MOVEMENT m = new MOVEMENT();
                                            m.AniId = pAnimal.AniId;
                                            m.happened_at_FarmID = pBedrijf.FarmId;
                                            m.MovKind = lMovkind;
                                            m.MovDate = MovDate;
                                            MovFunc.HandleMovTimes(pUr, MovDate, "", ref m);
                                            m.MovOrder = MovFunc.getNewMovementOrder(pUr, lMovkind, MovDate, pAnimal.AniId, pUbn.UBNid);
                                            m.MovThird_UBNid = tegenpartij.UBNid;
                                            m.ThrId = tegenpartij.ThrID;
                                            m.Progid = pBedrijf.ProgId;
                                            m.UbnId = pUbn.UBNid;
                                            m.Changed_By = pChangedBy;
                                            m.SourceID = pSourceId;
                                            if (lMstb.SaveMovement(m))
                                            {
                                                aantalaanpassingen += 1;
                                                pAnimalMovs.Add(m);
                                                if (lMovkind == 1)
                                                {
                                                    BUYING bu = new BUYING();
                                                    bu.MovId = m.MovId;
                                                    bu.PurOrigin = tegenpartij.ThrID;
                                                    bu.Changed_By = pChangedBy;
                                                    bu.SourceID = pSourceId;
                                                    lMstb.SaveBuying(bu);

                                                    XElement xmovement = new XElement("NewMovement", new XAttribute("soort", "Aanvoer"), new XElement("MovId", m.MovId.ToString()), new XElement("MovDate", m.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", m.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", tegenpartij.ThrID.ToString()));
                                                    AgrobaseResultaat.Add(xmovement);
                                                }
                                                else if (lMovkind == 2)
                                                {
                                                    SALE sl = new SALE();
                                                    sl.MovId = m.MovId;
                                                    sl.SalDestination = tegenpartij.ThrID;
                                                    sl.Changed_By = pChangedBy;
                                                    sl.SourceID = pSourceId;
                                                    lMstb.SaveSale(sl);
                                                    XElement xmovement = new XElement("NewMovement", new XAttribute("soort", "Afvoer"), new XElement("MovId", m.MovId.ToString()), new XElement("MovDate", m.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", m.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", tegenpartij.ThrID.ToString()));
                                                    AgrobaseResultaat.Add(xmovement);
                                                }
                                                else if (lMovkind == 3)
                                                {

                                                }
                                                pAnicat.Changed_By = pChangedBy;
                                                pAnicat.SourceID = pSourceId;
                                                MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, pAnicat, pChangedBy, pSourceId);

                                            }
                                        }
                                        else
                                        {
                                            MOVEMENT mv = savedmov.ElementAt(0);
                                            if (tegenpartij.UBNid > 0)
                                            {
                                                if (mv.MovThird_UBNid == 0)
                                                {
                                                    mv.MovThird_UBNid = tegenpartij.UBNid;
                                                    mv.Changed_By = pChangedBy;
                                                    mv.SourceID = pSourceId;
                                                    lMstb.SaveMovement(mv);
                                                    aantalaanpassingen += 1;
                                                    if (mv.MovKind == 1)
                                                    {
                                                        BUYING bu = lMstb.GetBuying(mv.MovId);
                                                        bu.PurOrigin = tegenpartij.ThrID;
                                                        bu.Changed_By = pChangedBy;
                                                        bu.SourceID = pSourceId;
                                                        lMstb.SaveBuying(bu);
                                                        XElement xmovement = new XElement("UpdateMovement", new XAttribute("soort", "Aanvoer"), new XElement("MovId", mv.MovId.ToString()), new XElement("MovDate", mv.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", mv.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", tegenpartij.ThrID.ToString()));
                                                        AgrobaseResultaat.Add(xmovement);
                                                    }
                                                    else if (mv.MovKind == 2)
                                                    {
                                                        SALE sl = lMstb.GetSale(mv.MovId);
                                                        sl.SalDestination = tegenpartij.ThrID;
                                                        sl.Changed_By = pChangedBy;
                                                        sl.SourceID = pSourceId;
                                                        lMstb.SaveSale(sl);
                                                        XElement xmovement = new XElement("UpdateMovement", new XAttribute("soort", "Afvoer"), new XElement("MovId", mv.MovId.ToString()), new XElement("MovDate", mv.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", mv.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", tegenpartij.ThrID.ToString()));
                                                        AgrobaseResultaat.Add(xmovement);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Verblijfplaats.Element("geboortedatum") != null)
                            {
                                if (pAnimal.ThrId == 0)
                                {
                                    if (Verblijfplaats.Element("ubn").Value == pUbn.Bedrijfsnummer)
                                    {
                                        pAnimal.ThrId = pUbn.ThrID;
                                        pAnimal.Changed_By = pChangedBy;
                                        pAnimal.SourceID = pSourceId;
                                        lMstb.UpdateANIMAL(pUbn.ThrID, pAnimal);
                                        aantalaanpassingen += 1;
                                    }
                                }
                            }
                        }
                        else if (xHtier != null)
                        {
                            land = "DE";
                        }
                    }
                    else
                    {
                        land = "BE";
                        //BELGIE Sanitrace
                        String xVerblijfplaatsInrichtingsnr = xUbn.Value.Trim();
                        if (xVerblijfplaatsInrichtingsnr.Length > 4)
                        {
                            XElement xdatum = Verblijfplaats.Element("datum");
                            DateTime Datum = DateTime.MinValue;
                            if (xdatum != null)
                            {
                                Datum = utils.getDateLNV(xdatum.Value);
                            }
                            int soort = 0;
                            XElement xsoort = Verblijfplaats.Element("soort");
                            if (xsoort != null)
                            {
                                int.TryParse(xsoort.Value, out soort);

                            }

                            THIRD BEthird2 = new THIRD();
                            UBN BEubn2 = new UBN();
                            if (Verblijfplaats.ElementsBeforeSelf("Verblijfplaats").Count() > 0)
                            {
                                XElement before = Verblijfplaats.ElementsBeforeSelf("Verblijfplaats").Last();
                                if (before != null)
                                {

                                    getCompany(lMstb, before, out BEubn2, out BEthird2);
                                }
                            }
                            if (Verblijfplaats.ElementsAfterSelf("Verblijfplaats").Count() > 0)
                            {
                                XElement after = Verblijfplaats.ElementsAfterSelf("Verblijfplaats").First();
                                if (after != null)
                                {

                                    getCompany(lMstb, after, out BEubn2, out BEthird2);

                                }
                            }


                            MovClass bemov = new MovClass();

                            bemov.lMovement.AniId = pAnimal.AniId;
                            bemov.HandleMovTimes(pUr, Datum);
                            if (xVerblijfplaatsInrichtingsnr == "BE" + pThird.Thr_Brs_Number)//vanzichzelf
                            {
                                bemov.lMovement.UbnId = pUbn.UBNid;
                                bemov.lMovement.happened_at_FarmID = pBedrijf.FarmId;
                                bemov.lMovement.Progid = pBedrijf.ProgId;
                                bemov.lMovement.ThrId = pUbn.ThrID;
                            }
                            else
                            {
                                //NL 109990935790
                                //NL 109990635799

                                bemov.lMovement.UbnId = BEubn2.UBNid;
                                bemov.lMovement.Progid = pBedrijf.ProgId;
                                bemov.lMovement.ThrId = BEthird2.ThrId;
                                bemov.lMovement.happened_at_FarmID = pBedrijf.FarmId;
                                if (BEthird2.ThrId <= 0)
                                {
                                    bemov.Inrichtingsnr = xUbn.Value;
                                }

                            }
                            if (soort == 1 || soort == 4)
                            {
                                bemov.lMovement.MovKind = 1;
                                bemov.lBuying.PurOrigin = BEthird2.ThrId;
                                bemov.lMovement.MovThird_UBNid = BEubn2.UBNid;
                                if (soort == 4)
                                {
                                    bemov.lBuying.PurKind = 1;
                                }
                            }
                            else
                            {
                                if (soort == 3)
                                {
                                    bemov.lMovement.MovKind = soort;
                                    bemov.lLoss.AniId = pAnimal.AniId;
                                }
                                else
                                {
                                    bemov.lMovement.MovKind = 2;
                                    if (soort == 5)
                                    {
                                        bemov.lSale.SalKind = 1;
                                    }
                                    else if (soort == 6)
                                    {
                                        bemov.lSale.SalKind = 2;
                                    }
                                    bemov.lSale.SalDestination = BEthird2.ThrId;
                                    bemov.lMovement.MovThird_UBNid = BEubn2.UBNid;
                                }
                            }
                            if (xVerblijfplaatsInrichtingsnr == "BE" + pThird.Thr_Brs_Number)//vanzichzelf
                            {
                                movs.Add(bemov);
                            }
                        }
                    }
                }
            }
            var eigen = from n in movs
                        where n.lMovement.UbnId == pUbn.UBNid
                        select n;
            XAttribute xEigen = new XAttribute("EigenVerblijfplaats", eigen.Count().ToString());
            if (land == "DK")
            {
                xEigen.Value = aantalaanpassingen.ToString();
            }
            AgrobaseResultaat.Add(xEigen);
            //Zijn er movements voor het UBN?
            if (eigen.Count() > 0)
            {

                foreach (MovClass mc in eigen)
                {


                    var controle = from m in pAnimalMovs

                                   where (m.MovKind == mc.lMovement.MovKind || (m.MovKind > 2 && m.MovKind < 8))
                                   && m.UbnId == pUbn.UBNid
                                   && m.MovDate.Date == mc.lMovement.MovDate.Date

                                   select m;
                    //Update of een nieuwe Movement aanmaken
                    if (controle.Count() == 0)
                    {
                        //movement toevoegen
                        //invullen andere UBN gegevens
                        if (mc.lMovement.MovKind != 3)
                        {
                            XElement xOnbekend = new XElement("Onbekend", "Geen UBN met tegendatum bekend");
                            //if (de_ander.Count() > 0)
                            //{
                            //    MovClass ander = de_ander.ElementAt(0);


                            //    if (ander.Inrichtingsnr != "")
                            //    {
                            //        xOnbekend.Value = "Onbekend inrichtingsnr:" + ander.Inrichtingsnr;
                            //    }
                            //    else if (ander.lOtherUbn != "")
                            //    {
                            //        xOnbekend.Value = "Onbekend UBNnr:" + ander.lOtherUbn;

                            //    }




                            //    mc.lMovement.MovThird_UBNid = ander.lMovement.UbnId;
                            //    if (mc.lMovement.MovKind == 1)
                            //    {
                            //        mc.lBuying.PurOrigin = ander.lMovement.ThrId;
                            //    }
                            //    else if (mc.lMovement.MovKind == 2)
                            //    {
                            //        mc.lSale.SalDestination = ander.lMovement.ThrId;
                            //    }
                            //    if (ander.lMovement.ThrId > 0)
                            //    {
                            //        xOnbekend.Value = "";
                            //    }
                            //}
                            try
                            {
                                var tel = from n in pAnimalMovs
                                          where n.MovKind == mc.lMovement.MovKind
                                          && n.MovDate.Date == mc.lMovement.MovDate.Date
                                          select n;
                                mc.lMovement.MovOrder = tel.Count() + 1;
                            }
                            catch { }
                            //mc.lMovement.MovOrder = MovFunc.getNewMovementOrder(pUr, mc.lMovement.MovKind, mc.lMovement.MovDate, mc.lMovement.AniId, pBedrijf.UBNid);
                            //unLogger.WriteInfo("getNewMovementOrder(pUr"+mc.lMovement.MovKind.ToString()+","+ mc.lMovement.MovDate.ToString()+","+mc.lMovement.AniId.ToString()+","+pBedrijf.UBNid.ToString()+")");
                            if (pAnimal.AniId > 0)
                            {
                                mc.lMovement.Changed_By = pChangedBy;
                                mc.lMovement.SourceID = pSourceId;
                                if (lMstb.SaveMovement(mc.lMovement))
                                {
                                    //unLogger.WriteInfo("SaveMovement(mc.lMovement)");
                                    writeLog("addded new MOVEMENT  MovId:" + mc.lMovement.MovId.ToString() + " Movkind:" + mc.lMovement.MovKind.ToString());
                                    string soortmovement = "";
                                    int derde = 0;
                                    if (mc.lMovement.MovKind == 1)
                                    {
                                        mc.lBuying.MovId = mc.lMovement.MovId;
                                        derde = mc.lBuying.PurOrigin;
                                        mc.lBuying.Changed_By = pChangedBy;
                                        mc.lBuying.SourceID = pSourceId;
                                        lMstb.SaveBuying(mc.lBuying);
                                        writeLog("addded new BUYING  MovId:" + mc.lMovement.MovId.ToString() + " Movkind:" + mc.lMovement.MovKind.ToString());
                                        if (derde > 0)
                                        {
                                            xOnbekend.Value = "";
                                        }
                                        soortmovement = "Aanvoer";
                                        //unLogger.WriteInfo("SaveBuying(mc.lBuying);");

                                    }
                                    else if (mc.lMovement.MovKind == 2)
                                    {
                                        mc.lSale.MovId = mc.lMovement.MovId;
                                        derde = mc.lSale.SalDestination;
                                        mc.lSale.Changed_By = pChangedBy;
                                        mc.lSale.SourceID = pSourceId;
                                        lMstb.SaveSale(mc.lSale);
                                        writeLog("addded new SALE  MovId:" + mc.lMovement.MovId.ToString() + " Movkind:" + mc.lMovement.MovKind.ToString());
                                        if (derde > 0)
                                        {
                                            xOnbekend.Value = "";
                                        }
                                        soortmovement = "Afvoer";
                                        //unLogger.WriteInfo("SaveSale(mc.lSale);");
                                    }
                                    pAnicat.Changed_By = pChangedBy;
                                    pAnicat.SourceID = pSourceId;
                                    MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, pAnicat, pChangedBy, pSourceId);
                                    //unLogger.WriteInfo("SetSaveAnimalCategory(pUr, pBedrijf, pAnicat);");
                                    XElement xmovement = new XElement("NewMovement", new XAttribute("soort", soortmovement), new XElement("MovId", mc.lMovement.MovId.ToString()), new XElement("MovDate", mc.lMovement.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", mc.lMovement.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", derde.ToString()));
                                    if (xOnbekend.Value != "")
                                    {
                                        xmovement.Add(xOnbekend);
                                    }
                                    AgrobaseResultaat.Add(xmovement);
                                }
                            }
                        }
                        else
                        {
                            mc.lMovement.Changed_By = pChangedBy;
                            mc.lMovement.SourceID = pSourceId;
                            if (lMstb.SaveMovement(mc.lMovement))
                            {
                                writeLog("addded new MOVEMENT  MovId:" + mc.lMovement.MovId.ToString() + " Movkind:" + mc.lMovement.MovKind.ToString());
                                //unLogger.WriteInfo("SaveMovement(mc.lMovement)");
                                mc.lLoss.MovId = mc.lMovement.MovId;
                                mc.lLoss.Changed_By = pChangedBy;
                                mc.lLoss.SourceID = pSourceId;
                                lMstb.SaveLoss(mc.lLoss);
                                writeLog("addded new LOSS  MovId:" + mc.lMovement.MovId.ToString() + " Movkind:" + mc.lMovement.MovKind.ToString());
                                //unLogger.WriteInfo("SaveLoss(mc.lLoss)");
                                MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, pAnicat, pChangedBy, pSourceId);
                                //unLogger.WriteInfo("SetSaveAnimalCategory(pUr, pBedrijf, pAnicat)");
                                XElement xmovement = new XElement("NewMovement", new XAttribute("soort", "Dood"), new XElement("MovId", mc.lMovement.MovId.ToString()), new XElement("MovDate", mc.lMovement.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", mc.lMovement.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", "0"));
                                AgrobaseResultaat.Add(xmovement);
                            }


                        }

                    }
                    else
                    {
                        //eventueel een update

                        MOVEMENT mUpdate = controle.ElementAt(0);
                        string soortUpdate = "Dood";
                        int derde = 0;
                        if (mc.lMovement.MovThird_UBNid > 0)
                        {
                            mUpdate.MovThird_UBNid = mc.lMovement.MovThird_UBNid;
                        }
                        mUpdate.Changed_By = pChangedBy;
                        mUpdate.SourceID = pSourceId;
                        lMstb.UpdateMovement(mUpdate);
                        //unLogger.WriteInfo("UpdateMovement(mUpdate)");
                        if (mUpdate.MovKind == 1)
                        {
                            BUYING updatebui = lMstb.GetBuying(mUpdate.MovId);
                            if (mc.lBuying.PurOrigin > 0)
                            {
                                updatebui.PurOrigin = mc.lBuying.PurOrigin;
                            }
                            derde = updatebui.PurOrigin;
                            if (pImportDatum > DateTime.MinValue)
                            {
                                if (pImportDatum.Date == mUpdate.MovDate.Date)
                                {
                                    updatebui.PurKind = 1;
                                }
                            }
                            updatebui.Changed_By = pChangedBy;
                            updatebui.SourceID = pSourceId;
                            lMstb.UpdateBuying(updatebui);
                            writeLog(" update BUYING  MovId:" + mUpdate.MovId.ToString() + " Movkind:" + mUpdate.MovKind.ToString());
                            //unLogger.WriteInfo("UpdateBuying(updatebui)");
                            soortUpdate = "Aanvoer";
                        }
                        else if (mc.lMovement.MovKind == 2)
                        {
                            SALE updatesale = lMstb.GetSale(mUpdate.MovId);
                            if (mc.lSale.SalDestination > 0)
                            {
                                updatesale.SalDestination = mc.lSale.SalDestination;
                            }
                            derde = updatesale.SalDestination;
                            updatesale.Changed_By = pChangedBy;
                            updatesale.SourceID = pSourceId;
                            lMstb.SaveSale(updatesale);
                            writeLog("update SALE  MovId:" + mc.lMovement.MovId.ToString() + " Movkind:" + mc.lMovement.MovKind.ToString());
                            //unLogger.WriteInfo("SaveSale(updatesale)");
                            soortUpdate = "Afvoer";
                        }
                        pAnicat.Changed_By = pChangedBy;
                        pAnicat.SourceID = pSourceId;
                        MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, pAnicat, pChangedBy, pSourceId);
                        XElement xmovement = new XElement("UpdateMovement", new XAttribute("soort", soortUpdate), new XElement("MovId", mUpdate.MovId.ToString()), new XElement("MovDate", mUpdate.MovDate.ToString("dd-MM-yyyy")), new XElement("MovThird_UBNid", mUpdate.MovThird_UBNid.ToString()), new XElement("BuySaleThrId", derde.ToString()));
                        AgrobaseResultaat.Add(xmovement);



                    }
                }
            }
            else
            {
                pAnicat.Changed_By = pChangedBy;
                pAnicat.SourceID = pSourceId;
                MovFunc.SetSaveAnimalCategory(pUr, pBedrijf, pAnicat,pChangedBy, pSourceId);

            }
            xVerblijfplaatsen.Add(AgrobaseResultaat);
        }

        private void getCompany(AFSavetoDB pMstb, XElement pVerblijfplaats, out UBN pUbn, out THIRD pThird)
        {
            pUbn = new UBN();
            pThird = new THIRD();
            if (pVerblijfplaats != null)
            {
                XElement xUbn = pVerblijfplaats.Element("ubn");
                if (xUbn != null)
                {
                    pUbn = pMstb.getUBNByBedrijfsnummer(xUbn.Value.Trim());
                    if (pUbn.UBNid <= 0)
                    {
                        XElement xpostcode = pVerblijfplaats.Element("postcode");
                        XElement xwoonplaats = pVerblijfplaats.Element("woonplaats");
                        XElement xadres = pVerblijfplaats.Element("adres");

                        if (xadres != null && xpostcode != null && xwoonplaats != null)
                        {
                            if (xadres.Value.Length > 0 && xpostcode.Value.Length > 0 && xwoonplaats.Value.Length > 0)
                            {
                                pThird = new THIRD();
                                pThird.ThrZipCode = MovFunc.ToUpperSpaceAway(xpostcode.Value);
                                pThird.ThrExt = utils.ExtractHouseNumberFromAdress(xadres.Value);
                                pThird.ThrStreet1 = xadres.Value;
                                pThird.ThrCity = xwoonplaats.Value.Trim();
                                pMstb.SaveThird(pThird);
                                writeLog("addded new THIRD  ThrId:" + pThird.ThrId.ToString());

                            }
                        }

                        if (pThird.ThrId > 0)
                        {

                            pUbn.Bedrijfsnummer = xUbn.Value.Trim();
                            pUbn.ThrID = pThird.ThrId;
                            pUbn.Bedrijfsnaam = xUbn.Value.Trim();
                            pMstb.SaveUbn(pUbn);
                            writeLog("addded new UBN  UbnId:" + pUbn.UBNid.ToString());
                        }
                    }
                    else
                    {
                        if (pUbn.ThrID > 0)
                        {
                            pThird = pMstb.GetThirdByThirId(pUbn.ThrID);
                        }
                    }
                }
                else
                {
                    XElement xBEUbn = pVerblijfplaats.Element("inrichtingsnr");
                    if (xBEUbn != null)
                    {
                        String xVerblijfplaatsInrichtingsnr = xBEUbn.Value.Trim();
                        string Beslagnummer = xVerblijfplaatsInrichtingsnr.Remove(0, 2);
                        pThird = pMstb.GetThirdByBrs_Number(Beslagnummer);
                        //unLogger.WriteInfo("GetThirdByBrs_Number("+Beslagnummer+");");
                        if (pThird.ThrId > 0)
                        {
                            List<UBN> ubs = pMstb.getUBNsByThirdID(pThird.ThrId);
                            //unLogger.WriteInfo("getUBNsByThirdID(" + BEthird.ThrId.ToString() + ")");
                            if (ubs.Count() > 0)
                            {
                                pUbn = ubs.ElementAt(0);

                            }
                        }
                        else
                        {
                            if (xVerblijfplaatsInrichtingsnr.StartsWith("BE"))
                            {

                                pThird.ThrCompanyName = xVerblijfplaatsInrichtingsnr;
                                pThird.Thr_Brs_Number = Beslagnummer;
                                pThird.ThrCountry = "21";
                                pMstb.SaveThird(pThird);
                                writeLog("addded new THIRD  ThrId:" + pThird.ThrId.ToString());
                                //unLogger.WriteInfo("SaveThird(BEthird);" + Beslagnummer);
                            }
                        }
                    }
                }
            }
        }

        private class MovClass
        {
            public MOVEMENT lMovement { get; set; }
            public BUYING lBuying { get; set; }
            public SALE lSale { get; set; }
            public LOSS lLoss { get; set; }
            public string lOtherUbn { get; set; }

            public string Inrichtingsnr { get; set; }
            public DateTime EindDatum { get; set; }
            public MovClass()
            {
                lMovement = new MOVEMENT();
                lBuying = new BUYING();
                lSale = new SALE();
                lLoss = new LOSS();
                lOtherUbn = "";
                Inrichtingsnr = "";
                EindDatum = DateTime.MinValue;
            }

            public void HandleMovTimes(UserRightsToken pUr, DateTime pInvoerdatum)
            {
                pInvoerdatum = pInvoerdatum.Date;
                DateTime tijd = pInvoerdatum;

                lMovement.MovDate = pInvoerdatum;

                lMovement.MovMutationDate = DateTime.Now;
                lMovement.MovMutationTime = DateTime.Now;


                if (lMovement.MovId == 0)
                {
                    if (lMovement.AniId > 0)
                    {
                        AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
                        List<MOVEMENT> movs = lMstb.GetMovementsByAniId(lMovement.AniId);
                        var same = from n in movs
                                   where n.MovDate.Date == tijd.Date
                                   orderby n.MovTime descending
                                   select n;
                        if (same.Count() > 0)
                        {
                            DateTime lastmovtime = same.ElementAt(0).MovTime;
                            tijd = tijd.AddHours((lastmovtime.Hour + 1));
                        }
                    }
                }


                if (tijd.Hour == 0)
                {
                    tijd = tijd.AddHours(8);
                }
                lMovement.MovTime = tijd;
            }
        }

        public void addExtraElementsFromStallijstRow(XElement pXAnimal, DataRow pRow)
        {
            //kolommen pRow { "levensnummer", "naam", "geboortedatum", "geslacht", "haarkleur", "rastype", "versienrpaspoort", "levennrmoeder","landcode","werknummer" };
            ANIMAL lAnimal = new ANIMAL();
            ANIMALCATEGORY lAnimalCategory = new ANIMALCATEGORY();

            XElement xWerknummer = pXAnimal.Element("Werknummer");
            if (xWerknummer == null)
            { xWerknummer = new XElement("Werknummer"); pXAnimal.Add(xWerknummer); }
            XElement xGeboortedat = pXAnimal.Element("Geboortedat");
            if (xGeboortedat == null)
            { xGeboortedat = new XElement("Geboortedat"); pXAnimal.Add(xGeboortedat); }
            XElement xImportdat = pXAnimal.Element("Importdat");
            if (xImportdat == null)
            { xImportdat = new XElement("Importdat"); pXAnimal.Add(xImportdat); }
            XElement xLandCodeHerkomst = pXAnimal.Element("LandCodeHerkomst");
            if (xLandCodeHerkomst == null)
            { xLandCodeHerkomst = new XElement("LandCodeHerkomst"); pXAnimal.Add(xLandCodeHerkomst); }
            XElement xLandCodeOorsprong = pXAnimal.Element("LandCodeOorsprong");
            if (xLandCodeOorsprong == null)
            { xLandCodeOorsprong = new XElement("LandCodeOorsprong"); pXAnimal.Add(xLandCodeOorsprong); }
            XElement xGeslacht = pXAnimal.Element("Geslacht");
            if (xGeslacht == null)
            { xGeslacht = new XElement("Geslacht"); pXAnimal.Add(xGeslacht); }
            XElement xHaarkleur = pXAnimal.Element("Haarkleur");
            if (xHaarkleur == null)
            { xHaarkleur = new XElement("Haarkleur"); pXAnimal.Add(xHaarkleur); }
            XElement xEinddatum = pXAnimal.Element("Einddatum");
            if (xEinddatum == null)
            { xEinddatum = new XElement("Einddatum"); pXAnimal.Add(xEinddatum); }
            XElement xLevensnrMoeder = pXAnimal.Element("LevensnrMoeder");
            if (xLevensnrMoeder == null)
            { xLevensnrMoeder = new XElement("LevensnrMoeder"); pXAnimal.Add(xLevensnrMoeder); }
            XElement xVerblijfplaatsen = pXAnimal.Element("Verblijfplaatsen");
            if (xVerblijfplaatsen == null)
            { xVerblijfplaatsen = new XElement("Verblijfplaatsen"); pXAnimal.Add(xVerblijfplaatsen); }
            XElement xAniName = pXAnimal.Element("AniName");
            if (xAniName == null)
            { xAniName = new XElement("AniName"); pXAnimal.Add(xAniName); }
            XElement xRasType = pXAnimal.Element("RasType");
            if (xRasType == null)
            { xRasType = new XElement("RasType"); pXAnimal.Add(xRasType); }
            XElement xVersieNrPaspoort = pXAnimal.Element("VersieNrPaspoort");
            if (xVersieNrPaspoort == null)
            { xVersieNrPaspoort = new XElement("VersieNrPaspoort"); pXAnimal.Add(xVersieNrPaspoort); }
            if (pRow["naam"] != null)
            {
                if (pRow["naam"] != DBNull.Value)
                {
                    if (xAniName.Value.Trim() == "")
                    {
                        xAniName.Value = pRow["naam"].ToString();
                    }
                }
            }
            if (pRow["geboortedatum"] != null)
            {
                if (pRow["geboortedatum"] != DBNull.Value)
                {
                    if (xGeboortedat.Value.Trim() == "")
                    {
                        xGeboortedat.Value = pRow["geboortedatum"].ToString();
                    }
                }
            }
            if (pRow["geslacht"] != null)
            {
                if (pRow["geslacht"] != DBNull.Value)
                {
                    if (xGeslacht.Value.Trim() == "")
                    {
                        xGeslacht.Value = pRow["geslacht"].ToString();
                    }
                }
            }
            if (pRow["haarkleur"] != null)
            {
                if (pRow["haarkleur"] != DBNull.Value)
                {
                    if (xHaarkleur.Value.Trim() == "")
                    {
                        xHaarkleur.Value = pRow["haarkleur"].ToString();
                    }
                }
            }
            if (pRow["rastype"] != null)
            {
                if (pRow["rastype"] != DBNull.Value)
                {
                    if (xRasType.Value.Trim() == "")
                    {
                        xRasType.Value = pRow["rastype"].ToString();
                    }
                }
            }
            if (pRow["versienrpaspoort"] != null)
            {
                if (pRow["versienrpaspoort"] != DBNull.Value)
                {
                    if (xVersieNrPaspoort.Value.Trim() == "")
                    {
                        xVersieNrPaspoort.Value = pRow["versienrpaspoort"].ToString();
                    }
                }
            }
            if (pRow["levennrmoeder"] != null)
            {
                if (pRow["levennrmoeder"] != DBNull.Value)
                {
                    if (xLevensnrMoeder.Value.Trim() == "")
                    {
                        xLevensnrMoeder.Value = pRow["levennrmoeder"].ToString();
                    }
                }
            }
            if (pRow["werknummer"] != null)
            {
                if (pRow["werknummer"] != DBNull.Value)
                {
                    if (xWerknummer.Value.Trim() == "")
                    {
                        xWerknummer.Value = pRow["werknummer"].ToString();
                    }
                }
            }
        }

        public void VerwerkOpgevraagdStallijstDier(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThird, ref List<ANIMALCATEGORY> pAniCats, string pRfid, int pProcent, XElement xAnimal,int pChangedBy, int pSourceId)
        {

            //if (lHairColors == null || lHairColors.Count() == 0)
            //{
            //    setHairColors(pUr);
            //}
            //System.Diagnostics.Stopwatch lStopWatch = new System.Diagnostics.Stopwatch();
            //lStopWatch.Start();
            //String lAniLifeNumber=xAnimal.Attributes("")
            XElement xWerknummer = xAnimal.Element("Werknummer");
            XElement xGeboortedat = xAnimal.Element("Geboortedat");
            XElement xImportdat = xAnimal.Element("Importdat");
            XElement xLandCodeHerkomst = xAnimal.Element("LandCodeHerkomst");
            XElement xLandCodeOorsprong = xAnimal.Element("LandCodeOorsprong");
            XElement xGeslacht = xAnimal.Element("Geslacht");
            XElement xHaarkleur = xAnimal.Element("Haarkleur");
            XElement xEinddatum = xAnimal.Element("Einddatum");
            XElement xLevensnrMoeder = xAnimal.Element("LevensnrMoeder");
            XElement xVerblijfplaatsen = xAnimal.Element("Verblijfplaatsen");
            XElement xVersieNrPaspoort = xAnimal.Element("VersieNrPaspoort");
            XElement xRasType = xAnimal.Element("RasType");
            XElement xAniName = xAnimal.Element("AniName");
            XElement xFokker = xAnimal.Element("FokkerNummer");//alleen sanitrace

            /*
                <Werknummer>45278</Werknummer>
                <Geboortedat>27-02-2011</Geboortedat>
                <Importdat>17-03-2011</Importdat>
                <LandCodeHerkomst>DE</LandCodeHerkomst>
                <LandCodeOorsprong>DE</LandCodeOorsprong>
                <Geslacht>M</Geslacht>
                <Haarkleur>ZB</Haarkleur>
                <Einddatum>19-10-2011</Einddatum>
                <LevensnrMoeder>DE 0985191585</LevensnrMoeder>
             */
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            String LnvOfSanitel = "RVO";
            string defIenRaction = "29";
            if (pThird.ThrCountry == "21" || pThird.ThrCountry == "125")
            {
                defIenRaction = "26";
                LnvOfSanitel = "Sanitrace";
            }
            int lHoogstevolgnr = 0;
            if (pRfid == "3")
            {
                List<ANIMAL> animals = lMstb.GetAnimalsByFarmId(pBedrijf.FarmId);

                string lStamboeknr = Event_functions.getStamboekNr(pBedrijf, pThird);
                lHoogstevolgnr = Event_functions.getHoogsteVolgnr(pUr, pBedrijf, lStamboeknr, animals);
            }

            string Anilifenr = xAnimal.Attribute("AniLifeNumber").Value;// StallijstRow["landcode"].ToString() + " " + StallijstRow["levensnummer"].ToString();
            writeLog(" VerwerkOpgevraagdStallijstDier :" + Anilifenr);
            int lAniId = 0;
            if (xAnimal.Attribute("AniId") != null)
            {
                int.TryParse(xAnimal.Attribute("AniId").Value, out lAniId);
            }
            string naam = xAniName.Value;// StallijstRow["naam"].ToString();
            DateTime geboortedatum = utils.getDateLNV(xGeboortedat.Value);// utils.getDateLNV(StallijstRow["geboortedatum"].ToString());

            DateTime pImportdat = DateTime.MinValue;

            if (xImportdat != null)
            {
                pImportdat = utils.getDateFormat(xImportdat.Value);
            }

            int geslacht = 0;
            if (xGeslacht.Value == "V")// (StallijstRow["geslacht"].ToString() == "V")
            {
                geslacht = 2;
            }
            else if (xGeslacht.Value == "M")// (StallijstRow["geslacht"].ToString() == "M")
            {
                geslacht = 1;
            }
            string lHaarkleur = xHaarkleur.Value;
            if (defIenRaction == "26")
            {
                //lHaarkleur = CORE.utils.getBE_HairColorID(xHaarkleur.Value);// StallijstRow["haarkleur"].ToString());
            }
            ANIMAL MoederDier = new ANIMAL();
            string levennrmoeder = xLevensnrMoeder.Value;// StallijstRow["levennrmoeder"].ToString();
            if (levennrmoeder.Length > 0)
            {
                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    MoederDier = lMstb.GetAnimalByAniAlternateNumber(levennrmoeder);

                }
                else
                {
                    MoederDier = lMstb.GetAnimalByLifenr(levennrmoeder);
                }
                if (MoederDier.AniId <= 0)
                {
                    MoederDier.AniLifeNumber = levennrmoeder;
                    MoederDier.AniAlternateNumber = levennrmoeder;
                    MoederDier.AniSex = 2;
                    MoederDier.Changed_By = pChangedBy;
                    MoederDier.SourceID = pSourceId;
                    lMstb.SaveAnimal(pThird.ThrId, MoederDier);

                }
                if (MoederDier.AniId > 0)
                {
                    var test = from n in pAniCats
                               where n.AniId == MoederDier.AniId
                               select n;
                    if (test.Count() == 0)
                    {
                        ANIMALCATEGORY anew = new ANIMALCATEGORY();
                        anew.AniId = MoederDier.AniId;
                        anew.FarmId = pBedrijf.FarmId;
                        anew.Anicategory = 5;
                        anew.Changed_By = pChangedBy;
                        anew.SourceID = pSourceId;
                        if (lMstb.SaveAnimalCategory(anew))
                        {
                            writeLog("added new ANIMALCATEGORY  AniId:" + anew.AniId.ToString() + " FarmId:" + anew.FarmId.ToString() + " Category:" + anew.Anicategory.ToString());
                            pAniCats.Add(anew);
                        }
                    }
                }

            }

            //opslaan aanpassen
            ANIMAL DatabaseDier = new ANIMAL();
            if (lAniId > 0)
            {
                DatabaseDier = lMstb.GetAnimalById(lAniId);
            }
            else
            {
                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    DatabaseDier = lMstb.GetAnimalByAniAlternateNumber(Anilifenr);

                }
                else
                {
                    DatabaseDier = lMstb.GetAnimalByLifenr(Anilifenr);
                }
            }

            string lWorknr = "";
            string lLevensnummer = "";
            ANIMAL lFather = new ANIMAL();
            if (DatabaseDier.AniIdFather > 0)
            {
                lFather = lMstb.GetAnimalById(DatabaseDier.AniIdFather);
            }
            Event_functions.getRFIDNumbers(pUr, pBedrijf, pRfid, Anilifenr, lFather, lHoogstevolgnr, out lWorknr, out lLevensnummer);

            if (DatabaseDier.AniId <= 0)
            {
                DatabaseDier.AniLifeNumber = Anilifenr;
                DatabaseDier.AniAlternateNumber = Anilifenr;

                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                {
                    DatabaseDier.AniLifeNumber = lLevensnummer;
                    DatabaseDier.AniAlternateNumber = Anilifenr;
                }

            }
            if (DatabaseDier.AniIdMother == 0)
            {
                DatabaseDier.AniIdMother = MoederDier.AniId;
            }
            DatabaseDier.AniHaircolor_Memo = lHaarkleur;
            //if (defIenRaction == "26")
            //{
            //    if (DatabaseDier.Anihaircolor == 0)
            //    {
            //        DatabaseDier.Anihaircolor = lHaarkleur;
            //    }
            //}
            if (DatabaseDier.AniSex == 0)
            {
                DatabaseDier.AniSex = geslacht;
            }

            if (geboortedatum.Date > DateTime.MinValue)
            {
                if (DatabaseDier.AniBirthDate.Date == DateTime.MinValue)
                {
                    DatabaseDier.AniBirthDate = geboortedatum;
                }
            }
            if (DatabaseDier.ThrId == 0)
            {
                if (xFokker != null && xFokker.Value != "")
                {
                    xFokker.Value = xFokker.Value.Replace("BE", "");
                    if (xFokker.Value.Contains(pUbn.Bedrijfsnummer) || xFokker.Value.Contains(pThird.Thr_Brs_Number))
                    {
                        DatabaseDier.ThrId = pUbn.ThrID;
                    }
                }
            }
            if (DatabaseDier.AniName == "")
            {
                Regex regExLifenr = new Regex(@"^[A-Z]{2}\s[\D\d-]{1,12}$");
                if (regExLifenr.Match(naam).Success)
                {
                    unLogger.WriteInfo("Movement functions VerwerkOpgevraagdStallijstDier AniName=" + naam + " ipv Gebruikelijke naam voor : " + DatabaseDier.AniAlternateNumber);
                }
                
                DatabaseDier.AniName = naam;
                
            }
            List<BIRTH> lAnimalBirhs = new List<BIRTH>();
            if (DatabaseDier.AniId > 0)
            {
                DatabaseDier.Changed_By = pChangedBy;
                DatabaseDier.SourceID = pSourceId;
                lMstb.UpdateANIMAL(pThird.ThrId, DatabaseDier);

                lAnimalBirhs = lMstb.GetBirthsByAnimal(DatabaseDier.AniId);
            }
            else
            {
                DatabaseDier.Changed_By = pChangedBy;
                DatabaseDier.SourceID = pSourceId;
                lMstb.SaveAnimal(pThird.ThrId, DatabaseDier);
                writeLog("added new ANIMAL  AniId:" + DatabaseDier.AniId.ToString());
                ANIMALCATEGORY lNewCat = new ANIMALCATEGORY();
                lNewCat.Anicategory = 3;
                lNewCat.AniId = DatabaseDier.AniId;
                lNewCat.FarmId = pBedrijf.FarmId;
                lNewCat.AniWorknumber = lWorknr;
                lNewCat.Changed_By = pChangedBy;
                lNewCat.SourceID = pSourceId;
                lMstb.SaveAnimalCategory(lNewCat);
                writeLog("VerwerkOpgevraagdStallijstDier:  add new ANIMALCATEGORY  AniId:" + lNewCat.AniId.ToString() + " FarmId:" + lNewCat.FarmId.ToString() + " Category:" + lNewCat.Anicategory.ToString());
            }
            List<MOVEMENT> lMovs = lMstb.GetMovementsByAniId(DatabaseDier.AniId);
            var varAnimalMovs = from n in lMovs
                                where n.UbnId == pUbn.UBNid || n.UbnId == 0
                                select n;
            ANIMALCATEGORY acat = lMstb.GetAnimalCategoryByIdandFarmid(DatabaseDier.AniId, pBedrijf.FarmId);
           
            if (acat.FarmId <= 0)
            {
                acat = new ANIMALCATEGORY();
                acat.FarmId = pBedrijf.FarmId;
                acat.AniId = DatabaseDier.AniId;
                acat.AniWorknumber = lWorknr;
                acat.Anicategory = 3;
                acat.Changed_By = pChangedBy;
                acat.SourceID = pSourceId;
                lMstb.SaveAnimalCategory(acat);
            }
            if (acat.FarmId > 0 && acat.AniWorknumber == "")
            {
                acat.AniWorknumber = lWorknr;
                acat.Changed_By = pChangedBy;
                acat.SourceID = pSourceId;
                lMstb.SaveAnimalCategory(acat);
            }
            acat = lMstb.GetAnimalCategoryByIdandFarmid(DatabaseDier.AniId, pBedrijf.FarmId);
            acat.Anicategory = 3;
            acat.Changed_By = pChangedBy;
            acat.SourceID = pSourceId;
            lMstb.SaveAnimalCategory(acat);
            //writeLog("1_VerwerkOpgevraagdStallijstDier: " + DatabaseDier.AniAlternateNumber + " " + lWorknr + " AniId:" + acat.AniId.ToString() + " FarmId:" + acat.FarmId.ToString() + " Category:" + acat.Anicategory.ToString());

            checkMovements(lMstb, pUr, varAnimalMovs.ToList(), DatabaseDier, xVerblijfplaatsen, pUbn, pBedrijf, pThird, acat, DateTime.Now, DatabaseDier.AniBirthDate, pImportdat, pChangedBy, pSourceId);
            //acat = lMstb.GetAnimalCategoryByIdandFarmid(DatabaseDier.AniId, pBedrijf.FarmId);
            //writeLog("2_VerwerkOpgevraagdStallijstDier Verblijfplaatsen: " + DatabaseDier.AniAlternateNumber + " " + lWorknr + " AniId:" + acat.AniId.ToString() + " FarmId:" + acat.FarmId.ToString() + " Category:" + acat.Anicategory.ToString());

            checkAndSaveNakomelingen(pUr, DatabaseDier, xAnimal, lAnimalBirhs, pUbn, pBedrijf, pThird, pThird.ThrId);
            acat = lMstb.GetAnimalCategoryByIdandFarmid(DatabaseDier.AniId, pBedrijf.FarmId);
            writeLog("VerwerktOpgevraagdStallijstDier  " + DatabaseDier.AniAlternateNumber + " " + lWorknr + " AniId:" + acat.AniId.ToString() + " FarmId:" + acat.FarmId.ToString() + " Category:" + acat.Anicategory.ToString());

        }

        public void checkVervangenLevensnrDier(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThird, string pVervangenLevensnr, string pLifeNumber,int pChangeBy, int pSourceID)
        {

            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);

            if (pVervangenLevensnr.Trim().Length > 0)
            {

                ANIMAL lAnimal = lMstb.GetAnimalByAniAlternateNumber(pLifeNumber);
                ANIMAL checkAnimal = lMstb.GetAnimalByAniAlternateNumber(pVervangenLevensnr);

                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                {
                    lAnimal = lMstb.GetAnimalByLifenr(pLifeNumber);
                    checkAnimal = lMstb.GetAnimalByLifenr(pVervangenLevensnr);
                }

                if (checkAnimal.AniId > 0 && lAnimal.AniId <= 0)//Dit dier is Omgenummerd naar pLifeNumber bij RVO
                {
                    string lnvOmnummerNr = "";
                    List<LEVNRMUT> lMuts = lMstb.getLevNrMuts(checkAnimal.AniId);
                    if (lMuts.Count() > 0)
                    {
                        lnvOmnummerNr = lMuts.ElementAt(0).LNVMeldNr;
                    }

                    MovFunc.NummerDierOm(pUr, pBedrijf, pUbn, checkAnimal, pLifeNumber, lnvOmnummerNr, pChangeBy, pSourceID);

                }
            }
        }

        public string verwijderMelding(UserRightsToken pUr, MUTATION lSelMutation, BEDRIJF pBedr, THIRD pGebruiker, int pChangedBy)
        {
            string ret = "";
       
            if (pBedr.ProgId == 25)
            {
                AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
                int[] lReturnResults = { 0, 2, 98, 99 };
                if (lReturnResults.Contains(lSelMutation.Returnresult))
                {
                    if (lSelMutation.Internalnr > 0)
                    {
                        
                        if (lSelMutation.CodeMutation > 100 && lSelMutation.CodeMutation < 200)
                        {
                            MUTALOG logrec = Facade.GetInstance().getMeldingen().ConverttoMutLog(lSelMutation);
                            logrec.Report = 2;
                            logrec.Returnresult = 99;
                            logrec.Changed_By = pChangedBy;
                            logrec.SourceID = pGebruiker.ThrId;
                            if (Facade.GetInstance().getMeldingen().InsertMutLog(logrec, pUr))
                            {
                                unLogger.WriteInfo("Meldingverzenden:Niet verwijderd Verplaatst:Internalnrs:" + logrec.Internalnr.ToString() + " " + " Mutloginternalnr:" + logrec.Internalnr.ToString() + "  " + lSelMutation.Lifenumber + " Codemutation:" + lSelMutation.CodeMutation + " Report:" + lSelMutation.Report.ToString());
                                // "Niet verzonden. Verplaatst naar geblokkeerde I&R Meldingen";
                                Facade.GetInstance().getMeldingen().DeleteMutation(lSelMutation, pUr);
                                return "";
                            }
                            else
                            {
                                unLogger.WriteInfo("Meldingverzenden:Niet verzonden InsertMutLogError :Internalnr:" + lSelMutation.Internalnr.ToString() + " " + lSelMutation.Lifenumber + " Codemutation:" + lSelMutation.CodeMutation + " Report:" + lSelMutation.Report.ToString());
                                return   "Deze melding kan niet verwijderd worden.";
                            }
                        }
                  

                        int lCodeMutation = lSelMutation.CodeMutation;
                        string lLifenr = lSelMutation.Lifenumber;
                       
                        ANIMAL lAnimal = lMstb.GetAnimalByAniAlternateNumber(lSelMutation.Lifenumber);
                        int lAniId = lAnimal.AniId;



                        ANIMALCATEGORY ac = new ANIMALCATEGORY();
                        if (pBedr.FarmId > 0 && lAnimal.AniId > 0)
                        {
                            ac = lMstb.GetAnimalCategoryByIdandFarmid(lAnimal.AniId, pBedr.FarmId);
                        }
                        MUTALOG mCheck = new MUTALOG();

                        if (lSelMutation.MovId > 0)
                        {
                            mCheck = lMstb.GetMutaLogByMovId(lSelMutation.MovId);
                            MOVEMENT m = lMstb.GetMovementByMovId(lSelMutation.MovId);
                            switch (m.MovKind)
                            {
                                case 1:
                                    BUYING bu = lMstb.GetBuyingByMovId(m.MovId);
                                    //lMstb.DeleteBuying(bu);
                                    break;
                                case 2:
                                    SALE se = lMstb.GetSale(m.MovId);
                                    //lMstb.DeleteSale(se);
                                    break;
                                case 3:
                                    LOSS l = lMstb.GetLoss(m.MovId);
                                    //lMstb.DeleteLoss(l);
                                    break;

                            }
                            //lMstb.DeleteMovement(m);
                            if (ac.AniId > 0)
                            {
                                //MovFunc.SetSaveAnimalCategory(pUr, pBedr, ac);
                            }

                        }
                        else if (lSelMutation.EventId > 0)
                        {
                            mCheck = lMstb.GetMutaLogByEventId(lSelMutation.EventId);
                            EVENT ev = lMstb.GetEventdByEventId(lSelMutation.EventId);
                            switch (ev.EveKind)
                            {
                                case 5:
                                    if (lAnimal.AniIdMother > 0)
                                    {
                                        ANIMAL Moeder = lMstb.GetAnimalById(lAnimal.AniIdMother);
                                        //ret = Event_functions.verwijderWorp(pGebruiker.ThrId, pUr, pBedr, Moeder, ev.EventId);
                                    }
                                    break;
                            }
                            //lMstb.DeleteEvent(ev);
                        }

                        
                        //if (mCheck.Internalnr > 0)
                        //{
                        //    return "Deze melding kan niet verwijderd worden. want er is al een melding gedaan.";
                        //}
                        //else
                        //{
                            lMstb.DeleteMutation(lSelMutation);
                        //}
                        //return "";
                        //Hierna terug omdat het voorkomt dat er dieren verwijderd worden die niet
                        //verwijderd mogen worden.
                        List<int> notRemoveAnimal = new List<int>();
                        notRemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.AFVOER);
                        notRemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.CONTACTMELDING);
                        notRemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.DOOD);
                        notRemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.EXPORT);

                        List<int> RemoveAnimal = new List<int>();
                        RemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.AANVOER);
                        RemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.GEBOORTE);
                        RemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.IMPORT);
                        RemoveAnimal.Add((int)CORE.DB.LABELSConst.CodeMutation.OMNUMMEREN);
                        //Delete ANIMAL
                        if (RemoveAnimal.Contains(lCodeMutation))
                        {
                            if (lCodeMutation != (int)CORE.DB.LABELSConst.CodeMutation.OMNUMMEREN)
                            {
                                List<MUTALOG> mutlogs = lMstb.GetMutaLogsByLifeNumber(lAnimal.AniLifeNumber);
                                if (mutlogs.Count() == 0)//anders is het dier eerder aan of afgevoerd door een ander
                                {
                                    List<ANIMALCATEGORY> anicats = lMstb.GetAnimalCategoryById(lAnimal.AniId);
                                    foreach (ANIMALCATEGORY lAc in anicats)
                                    {
                                        //lMstb.DeleteAnimalCategory(lAc);
                                    }

                                    List<EVENT> oudeEvents = lMstb.getEventsByAniId(lAniId);
                                    foreach (EVENT ev in oudeEvents)
                                    {
                                        if (ev.EveKind == (int)CORE.DB.LABELSConst.EventKind.CHIPPEN)
                                        {
                                            //lMstb.DeleteEvent(ev);
                                        }
                                    }
                                    List<MOVEMENT> oudeMovements = lMstb.GetMovementsByAniId(lAniId);
                                    int[] movkinds = { 1, 2, 3 };
                                    foreach (MOVEMENT mv in oudeMovements)
                                    {
                                        if (movkinds.Contains(mv.MovKind))
                                        {
                                            switch (mv.MovKind)
                                            {
                                                case 1:
                                                    BUYING bu = lMstb.GetBuyingByMovId(mv.MovId);
                                                    //lMstb.DeleteBuying(bu);
                                                    break;
                                                case 2:
                                                    SALE se = lMstb.GetSale(mv.MovId);
                                                    //lMstb.DeleteSale(se);
                                                    break;
                                                case 3:
                                                    LOSS l = lMstb.GetLoss(mv.MovId);
                                                    //lMstb.DeleteLoss(l);
                                                    break;

                                            }
                                        }
                                        //lMstb.DeleteMovement(mv);

                                    }

                                    //lMstb.DeleteAnimalAfwijkingen(lAniId);
                                    StringBuilder bld = new StringBuilder();
                                    bld.Append(" UPDATE agrobase_dog.ANIMAL SET agrobase_dog.ANIMAL.AniId=-" + lAnimal.AniId.ToString());
                                    bld.Append(" WHERE agrobase_dog.ANIMAL.AniId=" + lAnimal.AniId.ToString());
                                    DataSet d = new DataSet();

                                    if (lMstb.GetDataBase().QueryData(pUr, d, bld, "minanimal", MissingSchemaAction.Add) != null)//ANIMAL.aniId wordt hier negatief gezet
                                    {
                                        List<ANIMALPASSWORD> lAnimalPassWords = lMstb.GetAnimalPasswordsByAniId(lAnimal.AniId);
                                        foreach (ANIMALPASSWORD ap in lAnimalPassWords)
                                        {
                                            lMstb.DeleteAnimalPassword(ap);
                                        }
                                        if (pBedr.ProgId == 25 && pGebruiker.ThrId > 0)
                                        {
                                            int[] VBKs = { 2500, 2550, 2551, 2570, 2599 };
                                            if (VBKs.Contains(pBedr.Programid))
                                            {
                                                CHIP_BOX box = new CHIP_BOX();
                                                CHIP_STOCK stok = new CHIP_STOCK();
                                                lMstb.getChipboxChipstockByChipnumber(lLifenr, out box, out stok);
                                                if (stok.cs_breeder_thrid == pGebruiker.ThrId)
                                                {
                                                    Event_functions.saveNewLifenr(pUr, pBedr, pGebruiker, true, lLifenr);
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                            else
                            {
                                List<LEVNRMUT> levnrmutsen = lMstb.getLevNrMuts(lAniId);
                                var t = from n in levnrmutsen
                                        where n.LevnrNieuw == lLifenr
                                        select n;
                                if (t.Count() > 0)
                                {
                                    lAnimal.AniLifeNumber = t.ElementAt(0).LevnrOud;
                                    lAnimal.AniAlternateNumber = t.ElementAt(0).LevnrOud;
                                    lMstb.UpdateANIMAL(pGebruiker.ThrId, lAnimal);
                                    string LifenrRetour = t.ElementAt(0).LevnrNieuw;
                                    if (pBedr.ProgId == 25 && pGebruiker.ThrId > 0)
                                    {

                                        Event_functions.saveNewLifenr(pUr, pBedr, pGebruiker, true, LifenrRetour);
                                    }
                                    //TO DO Delete LEVNRMUT
                                }
                            }
                        }
                    }
                    else
                    {
                        ret = "Deze melding kan niet verwijderd worden.";
                    }
                }
                else
                {
                   
                        ret = "Deze melding kan niet verwijderd worden.";
                  
                }
            }
            else
            {
                ret = "Dit is niet de juiste diersoort.";
            }
            return ret;
        }
    }

    public class MovFuncFileReader
    {

        public event EventHandler RequestUpdate;

        protected void OnRequestUpdate(object sender, MovFuncEvent e)
        {
            if (RequestUpdate != null)
                RequestUpdate(sender, e);
        }

        private void ReqUpdate(string pMessage, int pProcent)
        {
            if (RequestUpdate != null)
            {
                MovFuncEvent b = new MovFuncEvent();
                b.Message = pMessage;
                b.Progress = pProcent;
                RequestUpdate(this, b);
            }
        }

        public string InleesResult { get; private set; }

        public void StallijstInlezenCsv(UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThird, COUNTRY pCountry, string pCsVFile)
        {
            InleesResult = "";
            //Number, Lifenumber, Birth date en Respondernr.
            // Geslacht   importeer maar default allemaal als V.
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);

            int indexcount = 4;
            string Number, Lifenumber, Birthdate, Respondernr;

            if (File.Exists(pCsVFile))
            {
                try
                {
                    StreamReader rdr = new StreamReader(pCsVFile);
                    StringBuilder bld = new StringBuilder();
                    try
                    {
                        string file = rdr.ReadToEnd();
                        string[] lines = file.Split('\n');
                        int countOfLines = lines.GetLength(0);
                        rdr.Close();
                        rdr = new StreamReader(pCsVFile);

                        DBSelectQueries ds = new DBSelectQueries(pUr);
                        List<TRANSMIT> Alltransjes = ds.Responders.GetTransmitsByFarmId(pBedrijf.FarmId, pBedrijf.UBNid);
                        List<TRANSMSTOCK> trVoorraad = ds.Responders.GetTransmitterVoorraad(pBedrijf.UBNid);
                        string strLine = rdr.ReadLine();
                        int teller = 0;
                        int procent = 0;
                        char[] split = { ';' };

                        while (strLine != null)
                        {
                            if (strLine.Length > 0)
                            {
                                var arrLine = strLine.Split(split);
                                if (arrLine.Count() >= 4)
                                {
                                    teller += 1;
                                    procent = teller * 100 / countOfLines;

                                    Number = arrLine[0];
                                    Lifenumber = arrLine[1];
                                    Birthdate = arrLine[2];
                                    Respondernr = arrLine[3];
                                    if (!string.IsNullOrEmpty(Lifenumber))
                                    {
                                        ReqUpdate(Lifenumber, procent);
                                        ANIMAL ani = lMstb.GetAnimalByAniAlternateNumber(Lifenumber);
                                        if (ani.AniId <= 0)
                                        {
                                            ani = lMstb.GetAnimalByLifenr(Lifenumber);
                                        }
                                        if (ani.AniId <= 0)
                                        {
                                            ani = new ANIMAL();
                                            string ret = Checker.IsValidLevensnummer(Lifenumber, true, pBedrijf.ProgId, pBedrijf.Programid);
                                            if (ret != "")
                                            {
                                                ret = Checker.IsValidLevensnummer(Lifenumber, false, pBedrijf.ProgId, pBedrijf.Programid);
                                            }
                                            if (ret == "")
                                            {
                                                ani.AniAlternateNumber = Lifenumber;
                                                ani.AniLifeNumber = Lifenumber;
                                                ani.AniBirthDate = utils.getDate(Birthdate);
                                                ani.AniWorkNumber = Number;
                                                ani.AniCountryCodeOrigin = Lifenumber.Substring(0, 2).ToUpper();

                                                ani.AniSex = (int)CORE.DB.LABELSConst.AniSex.Vrouwelijk;

                                                lMstb.SaveAnimal(pThird.ThrId, ani);
                                                unLogger.WriteInfo(Lifenumber + ": Opgeslagen UBN:" + pUbn.Bedrijfsnummer);
                                            }
                                            else
                                            {
                                                unLogger.WriteError(Lifenumber + ":" + ret);
                                                ReqUpdate(Lifenumber + ":" + ret, 0);
                                            }
                                        }
                                        if (ani.AniId > 0)
                                        {
                                            if (ani.AniSex == 0)
                                            {
                                                ani.AniSex = (int)CORE.DB.LABELSConst.AniSex.Vrouwelijk;
                                                lMstb.SaveAnimal(pThird.ThrId, ani);
                                                unLogger.WriteInfo(Lifenumber + ": Opgeslagen UBN:" + pUbn.Bedrijfsnummer + " AniSex van 0 naar" + ((int)CORE.DB.LABELSConst.AniSex.Vrouwelijk).ToString());
                                            }
                                            if (ani.AniBirthDate == DateTime.MinValue)
                                            {
                                                ani.AniBirthDate = utils.getDate(Birthdate);
                                                lMstb.SaveAnimal(pThird.ThrId, ani);
                                                unLogger.WriteInfo(Lifenumber + ": Opgeslagen UBN:" + pUbn.Bedrijfsnummer + " AniBirthDate van null naar:" + Birthdate);

                                            }
                                            ANIMALCATEGORY ac = lMstb.GetAnimalCategoryByIdandFarmid(ani.AniId, pBedrijf.FarmId);
                                            if (ac.FarmId <= 0)
                                            {
                                                ac = new ANIMALCATEGORY();
                                                ac.Anicategory = 1;
                                                ac.AniId = ani.AniId;
                                                ac.FarmId = pBedrijf.FarmId;
                                                ac.AniWorknumber = Number;
                                                lMstb.SaveAnimalCategory(ac);
                                            }
                                            if (ac.FarmId > 0)
                                            {
                                                // responder
                                                if (!string.IsNullOrEmpty(Respondernr))
                                                {
                                                    var RTaken = from n in Alltransjes where n.TransmitterNumber == Respondernr select n;
                                                    if (RTaken.Count() > 0)
                                                    {
                                                        if (RTaken.ElementAt(0).AniId != ani.AniId)
                                                        {
                                                            ANIMAL aResponder = lMstb.GetAnimalById(RTaken.ElementAt(0).AniId);
                                                            unLogger.WriteInfo(Respondernr + " Omgehangen van:" + aResponder.AniAlternateNumber + " Naar:" + Lifenumber + ": UBN:" + pUbn.Bedrijfsnummer);
                                                            TRANSMIT tr = RTaken.ElementAt(0);
                                                            TRANSMIT trNew = new TRANSMIT();
                                                            trNew.AniId = ani.AniId;
                                                            trNew.farmid = pBedrijf.FarmId;
                                                            trNew.FarmNumber = pUbn.Bedrijfsnummer;
                                                            trNew.Koppelnr = tr.Koppelnr;
                                                            trNew.ProcesComputerId = tr.ProcesComputerId;
                                                            trNew.TransmitterNumber = tr.TransmitterNumber;
                                                            trNew.UbnID = pUbn.UBNid;
                                                            trNew.Worknumber = Number;
                                                            trNew.Changed_By = 6151;
                                                            trNew.SourceID = pUbn.ThrID;
                                                            tr.FarmNumber = pUbn.Bedrijfsnummer;
                                                            tr.farmid = pBedrijf.FarmId;
                                                            tr.UbnID = pBedrijf.UBNid;
                                                            lMstb.DeleteTransmit(tr);
                                                            lMstb.SaveTransMit(trNew);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int ProcesComputerId = 0;
                                                        var trInvoorraad = from n in trVoorraad where n.Respondernr == Respondernr select n;
                                                        if (trInvoorraad.Count() > 0)
                                                        {
                                                            TRANSMSTOCK tr = trInvoorraad.ElementAt(0);
                                                            ProcesComputerId = tr.ProcesComputerId;

                                                            lMstb.DeleteTransmStock(tr);
                                                        }
                                                        else
                                                        {


                                                            DataTable tbl = ds.Responders.getProcessCompIds(pBedrijf.FarmId, pBedrijf.UBNid, pBedrijf.ProgId);
                                                            int lpMSOptimabox = (int)CORE.utils.procesComputerReferenceNumber.MSOptimabox;
                                                            foreach (DataRow rw in tbl.Rows)
                                                            {
                                                                if (rw["Nummer"] != DBNull.Value)
                                                                {

                                                                    int.TryParse(rw["Nummer"].ToString(), out ProcesComputerId);
                                                                    if (ProcesComputerId >= 100)
                                                                    {
                                                                        if (ProcesComputerId.ToString().StartsWith(lpMSOptimabox.ToString()))
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            if (ProcesComputerId == 0)
                                                            {
                                                                if (pBedrijf.Programid == 16000)
                                                                {
                                                                    ProcesComputerId = int.Parse(lpMSOptimabox.ToString() + "01");
                                                                }
                                                                else
                                                                {
                                                                    try
                                                                    {
                                                                        int.TryParse(tbl.Rows[0]["Nummer"].ToString(), out ProcesComputerId);
                                                                    }
                                                                    catch { }
                                                                }
                                                            }
                                                        }
                                                        TRANSMIT trNew = new TRANSMIT();
                                                        trNew.AniId = ani.AniId;
                                                        trNew.farmid = pBedrijf.FarmId;
                                                        trNew.FarmNumber = pUbn.Bedrijfsnummer;
                                                        trNew.Koppelnr = 0;
                                                        trNew.ProcesComputerId = ProcesComputerId;
                                                        trNew.TransmitterNumber = Respondernr;
                                                        trNew.UbnID = pUbn.UBNid;
                                                        trNew.Worknumber = Number;
                                                        trNew.Changed_By = 6151;
                                                        trNew.SourceID = pUbn.ThrID;
                                                        lMstb.SaveTransMit(trNew);
                                                    }
                                                    bld.AppendLine(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("opgeslagen", "opgeslagen"));

                                                }
                                                else
                                                {
                                                    unLogger.WriteInfo(Lifenumber + " Geen respondernummer UBN:" + pUbn.Bedrijfsnummer + " Bedrijf:" + pBedrijf.FarmId.ToString() + ":" + strLine);
                                                    ReqUpdate(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("nietopgeslagen", "Niet opgeslagen"), procent);
                                                    bld.AppendLine(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("nietopgeslagen", "Niet opgeslagen"));
                                                }
                                            }
                                            else
                                            {
                                                unLogger.WriteInfo(Lifenumber + " Animalcategory :NIET Opgeslagen UBN:" + pUbn.Bedrijfsnummer + " Bedrijf:" + pBedrijf.FarmId.ToString() + ":" + strLine);
                                                ReqUpdate(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("nietopgeslagen", "Niet opgeslagen"), procent);
                                                bld.AppendLine(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("nietopgeslagen", "Niet opgeslagen"));

                                            }
                                        }
                                        else
                                        {
                                            unLogger.WriteInfo(Lifenumber + ":NIET Opgeslagen UBN:" + pUbn.Bedrijfsnummer);
                                            ReqUpdate(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("nietopgeslagen", "Niet opgeslagen"), procent);
                                            bld.AppendLine(Lifenumber + ": " + VSM_Ruma_OnlineCulture.getStaticResource("nietopgeslagen", "Niet opgeslagen"));
                                        }
                                    }
                                    else
                                    {
                                        unLogger.WriteError("Foute regel: " + strLine);
                                        ReqUpdate(VSM_Ruma_OnlineCulture.getStaticResource("regel", "Regel") + " " + VSM_Ruma_OnlineCulture.getStaticResource("fout", "Fout") + ":" + strLine, procent);
                                        bld.AppendLine(VSM_Ruma_OnlineCulture.getStaticResource("regel", "Regel") + " " + VSM_Ruma_OnlineCulture.getStaticResource("fout", "Fout") + ":" + strLine);
                                    }
                                }
                                else
                                {
                                    unLogger.WriteError("Foute regel: " + strLine);
                                    ReqUpdate(VSM_Ruma_OnlineCulture.getStaticResource("regel", "Regel") + " " + VSM_Ruma_OnlineCulture.getStaticResource("fout", "Fout") + ":" + strLine, procent);
                                    bld.AppendLine(VSM_Ruma_OnlineCulture.getStaticResource("regel", "Regel") + " " + VSM_Ruma_OnlineCulture.getStaticResource("fout", "Fout") + ":" + strLine);

                                }
                            }
                            strLine = rdr.ReadLine();
                        }

                    }
                    catch { }
                    finally
                    {
                        rdr.Close();

                    }
                    InleesResult = bld.ToString();
                }
                catch (Exception exc)
                {
                    unLogger.WriteError(exc.ToString());
                    ReqUpdate(exc.Message, 0);
                    InleesResult = exc.Message;
                }

            }
            else
            {
                unLogger.WriteError("Betand niet gevonden:" + pCsVFile);
                ReqUpdate(VSM_Ruma_OnlineCulture.getStaticResource("bestandvoor", "Bestand voor het inlezen niet gevonden."), 0);
                InleesResult = VSM_Ruma_OnlineCulture.getStaticResource("bestandvoor", "Bestand voor het inlezen niet gevonden.");
            }

        }
    }

    public class Translator
    {

        private const string URL_GOOGLETRANSLATE = "https://translate.google.nl/?hl=nl&tab=wT#{0}/{1}/{2}";
        public string Translate(string pStringToTransLate, string pFrom, string pTo)
        {
            return "";
            if (String.IsNullOrEmpty(pStringToTransLate) || String.IsNullOrEmpty(pFrom) || String.IsNullOrEmpty(pTo))
            {

                return "";
            }

            pStringToTransLate = Uri.EscapeDataString(pStringToTransLate);
            string requestUrl = String.Format(URL_GOOGLETRANSLATE, pFrom, pTo, pStringToTransLate);


            string html;
            try
            {
                using (WebClient client = new WebClient())
                {
                    html = client.DownloadString(requestUrl);
                }

            }
            catch (Exception ex)
            {
                return String.Format("Error downloading page Exception: {0}", ex);

            }

            if (string.IsNullOrEmpty(html))
            {
                return "Response is empty";

            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            try
            {
                doc.LoadHtml(html);
            }
            catch (Exception ex)
            {
                return String.Format("Error loading html Exception: {0}", ex);

            }


            HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class='search-result-wrapper']/table[1][@class='search-result']");
            if (node == null)
            {
                return "Could not get search result table";

            }

            HtmlAgilityPack.HtmlNodeCollection nodesMatchingXPath = node.SelectNodes("tr");

            if (nodesMatchingXPath == null || nodesMatchingXPath.Count <= 0 || nodesMatchingXPath[0] == null)
            {
                return "No Search results";

            }

            if (nodesMatchingXPath == null || nodesMatchingXPath.Count > 1)
            {
                return "Multiple search results";

            }

            node = nodesMatchingXPath[0];


            HtmlAgilityPack.HtmlNode idNode;
            try
            {
                idNode = node.SelectSingleNode("td[@class='id-cell']");
            }
            catch (Exception ex)
            {
                return String.Format("Error getting ID node Exception: {0}", ex);

            }
            if (idNode == null)
            {
                return "Error ID node == null";

            }

            HtmlAgilityPack.HtmlNode nameNode;
            try
            {
                nameNode = node.SelectSingleNode("td[@class='name-cell']");
            }
            catch (Exception ex)
            {
                return String.Format("Error getting name node Exception: {0}", ex);

            }
            if (nameNode == null)
            {
                return "Error name node == null";

            }

            return idNode.InnerText.Trim();
            string name = nameNode.InnerText.Trim();
        }

    }
}

