using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.Xml.Linq;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;
using System.Threading;
using System.IO;
using System.Configuration;

namespace VSM.RUMA.CORE
{

    public class Event_functions
    {
        public static bool UseOldMedicine = true;

        public static int addInsemination(UserRightsToken pUr, EVENT e, INSEMIN i)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = Convert.ToInt32(VSM.RUMA.CORE.DB.LABELSConst.EventKind.INSEMINEREN);
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, Convert.ToInt32(RUMA.CORE.DB.LABELSConst.EventKind.INSEMINEREN), e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            i.EventId = e.EventId;

            if (!lMstb.SaveInsemin(i))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            else if (i.InsKind > 1)
            {
                SavenewDHZ(pUr, e, i);
            }
            return e.EventId;
        }

        public static void SavenewDHZ(UserRightsToken pUr, EVENT e, INSEMIN i)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);
            // klaarzetten DHZ
            StringBuilder QRY_DHZ = new StringBuilder();
            QRY_DHZ.Append(" SELECT * ");
            QRY_DHZ.Append(" FROM DHZ");
            QRY_DHZ.AppendFormat(" WHERE EventID={0}", e.EventId);
            DataTable dtDHZ = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), QRY_DHZ);
            DHZ lDHZ = new DHZ();
            if (dtDHZ.Rows.Count > 0) lMstb.GetDataBase().FillObject(lDHZ, dtDHZ.Rows[0]);

            lDHZ.EventID = e.EventId;
            lDHZ.UbnId = e.UBNId;
            lDHZ.InsInfo = i.InsKind.ToString();
            if (i.InsKind == 4) lDHZ.Frozen = 1;
            else lDHZ.Frozen = 0;
            lDHZ.FarmNumber = lMstb.GetubnById(e.UBNId).Bedrijfsnummer;

            lDHZ.InsDate = e.EveDate;
            ANIMAL ani = lMstb.GetAnimalById(e.AniId);
            lDHZ.AniLifenumber = ani.AniLifeNumber;
            ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(e.AniId, e.happened_at_FarmID);
            lDHZ.AniWorknumber = anicat.AniWorknumber;

            lDHZ.AniName = ani.AniName;
            ANIMAL Bull = lMstb.GetAnimalById(i.AniIdFather);
            lDHZ.BullLifeNumber = Bull.AniLifeNumber;
            lDHZ.BullAInumber = Bull.BullAiNumber;
            lDHZ.BullName = Bull.AniName;
            lDHZ.ChargeNumber = i.InsChargeNumber;
            lDHZ.InsAmount = i.InsAmount;

            if (i.ReportStatus == 0)
            {
                lDHZ.Report = 1;
            }
            else lDHZ.Report = 0;


            if (lDHZ.Internalnr == 0) lMstb.InsertDHZ(lDHZ);
            else lMstb.UpdateDHZ(lDHZ);
        }

        public static int addGrztogth(UserRightsToken pUr, EVENT e, GRZTOGTH g)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = Convert.ToInt32(VSM.RUMA.CORE.DB.LABELSConst.EventKind.SAMENWEIDEN);
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, Convert.ToInt32(RUMA.CORE.DB.LABELSConst.EventKind.SAMENWEIDEN), e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            g.EventId = e.EventId;

            if (!lMstb.SaveGRZTOGTH(g))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            return e.EventId;
        }

        public static int addInheat(UserRightsToken pUr, EVENT e, INHEAT i)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = Convert.ToInt32(VSM.RUMA.CORE.DB.LABELSConst.EventKind.TOCHTIG);
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, Convert.ToInt32(RUMA.CORE.DB.LABELSConst.EventKind.TOCHTIG), e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            i.EventId = e.EventId;

            if (!lMstb.SaveInHeat(i))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            return e.EventId;
        }

        public static int addDryOff(UserRightsToken pUr, EVENT e, DRYOFF d)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = Convert.ToInt32(VSM.RUMA.CORE.DB.LABELSConst.EventKind.DROOGZETTEN);
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, Convert.ToInt32(RUMA.CORE.DB.LABELSConst.EventKind.DROOGZETTEN), e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            d.EventId = e.EventId;

            if (!lMstb.SaveDryoff(d))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            return e.EventId;
        }

        public static int addGestation(UserRightsToken pUr, EVENT e, GESTATIO g)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = Convert.ToInt32(VSM.RUMA.CORE.DB.LABELSConst.EventKind.DRACHTIGHEID);
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, Convert.ToInt32(RUMA.CORE.DB.LABELSConst.EventKind.DRACHTIGHEID), e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            g.EventId = e.EventId;

            if (!lMstb.SaveGestation(g))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            return e.EventId;
        }

        public static int saveTreatment(UserRightsToken pUr, EVENT e, TREATMEN t)
        {
            CORE.DB.DBMasterQueries m = new DB.DBMasterQueries(pUr);
            int ret = e.EventId;
            int TreFirstApplicationId = 0;
            DateTime evedate = e.EveDate.Date;

            for (int i = 0; i < t.TreMedDaysTreat; i++)
            {
                EVENT opsl = CORE.utils.Clone<EVENT>(e);

                TREATMEN opsltr = CORE.utils.Clone<TREATMEN>(t);

                opsltr.TreMedDaysTreat = 1;
                opsl.EveDate = evedate.AddDays((i * t.TreMedHoursRepeat) / 24);

                opsl.EveOrder = i + 1;// getNewEventOrder(pUr, opsl.EveDate, opsl.EveKind,opsl.UBNId, opsl.AniId);

                if (m.SaveEvent(opsl))
                {
                    opsltr.EventId = opsl.EventId;
                    ret = opsl.EventId;
                    if (i == 0)
                    {
                        TreFirstApplicationId = opsl.EventId;
                    }
                    opsltr.TreFirstApplicationId = TreFirstApplicationId;
                    if (opsltr.TreMedPlannr > 0 && opsltr.TreMedPlanUniqueNr == 0)
                    {
                        opsltr.TreMedPlanUniqueNr = TreFirstApplicationId;
                    }
                    opsltr.TreTime = evedate.AddHours(i * t.TreMedHoursRepeat);
                    m.SaveTreatmen(opsltr);

                }
                else
                {
                    unLogger.WriteError($@"ERROR Event_functions saveTreatment Aniid:{e.AniId} UbnId:{e.UBNId} medicijn:{t.MedId} {t.ArtID} Date:{e.EveDate.ToString()}");
                }
            }

            return ret;
        }

        public static int addBlood(UserRightsToken pUr, EVENT e, BLOOD b)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = (int)VSM.RUMA.CORE.DB.LABELSConst.EventKind.BLOEDONDERZOEK;
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, (int)RUMA.CORE.DB.LABELSConst.EventKind.BLOEDONDERZOEK, e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            b.EventId = e.EventId;

            if (!lMstb.SaveBlood(b))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            return e.EventId;
        }

        public static int addTransplant(UserRightsToken pUr, EVENT e, TRANSPLA t)
        {
            AFSavetoDB lMstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            e.EveKind = Convert.ToInt32(VSM.RUMA.CORE.DB.LABELSConst.EventKind.INPLANTEREN);
            e.EveMutationDate = DateTime.Now.Date;
            e.EveMutationTime = DateTime.Now;
            e.EveOrder = getNewEventOrder(pUr, e.EveDate, Convert.ToInt32(RUMA.CORE.DB.LABELSConst.EventKind.INPLANTEREN), e.UBNId, e.AniId);

            if (!lMstb.SaveEvent(e))
                return -1;

            t.EventId = e.EventId;

            if (!lMstb.SaveTranspla(t))
            {
                lMstb.DeleteEvent(e);
                return -1;
            }
            return e.EventId;
        }

        private static AFSavetoDB getMysqlDb(DBConnectionToken pUr)
        {
            return Facade.GetInstance().getSaveToDB(pUr);
        }

        public static List<AGRO_LABELS> getEventFarmAgroLabels(UserRightsToken pUr, int pProgId, int pProgramId, List<int> pEventKinds, int pCountryCode)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<AGRO_LABELS> events = lMstb.GetAgroLabels(CORE.DB.LABELSConst.labKind.EVEKIND, pCountryCode, pProgramId, pProgId);
            for (int j = events.Count - 1; j >= 0; j--)
            {
                if (!pEventKinds.Contains(events[j].LabID))
                {
                    events.Remove(events[j]);
                }
            }
            return events;
        }

        public static List<LABELS> getEventFarmLabelsOLD(UserRightsToken pUr, int pProgId, int pProgramId, List<int> pEventKinds, int pCountryCode)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            /*
                           int Programid 
              
                   528	9100	0	** PROGRAMMA'S **
                   528	9100	1	RUMA Rundvee Senior 2
                   528	9100	2	RUMA Rundvee Senior 1
                   528	9100	3	RUMA Rundvee Junior
                   528	9100	4	RUMA Stierenmanagement Jr.
                   528	9100	5	RUMA Stierenmanagement Sr.
                   528	9100	6	Belexpert Senior
                   528	9100	7	EGAM Geitenmanagement
                   528	9100	8	RUMA Witvlees Senior
                   528	9100	9	RUMA Rose Senior agroscoop forfarmers
                   528	9100	10	RUMA I&R
                   528	9100	11	RUMA I&R/DHZ-KI
                   528	9100	12	RUMA Zoogkoeien
                   528	9100	13	RUMA Jongvee
                   528	9100	14	Belexpert Junior
                   528	9100	15	Kalveren Bedrijfsregister
                   528	9100	16	BelTes  (online Actief)
                   528	9100	17	BelExpert Basis
                   528	9100	18	Kerry Hill
                   528	9100	19	RUMA Witvlees Junior
                   528	9100	20	RUMA Rose Junior
                   528	9100	21	RUMA Zoogkoe (huur)
                   528	9100	22 en 24 t/m 34	NSFO
                                23  Paligroup
                                50  van Veengroep
                                51  NFS admin net als 22  schapen
                                52  NFS lid net als rest bij NSFO  schapen
                               100  admin van 7
             *                 230 admin van 23
             * 
             *      1   Tochtig
                    2   Insemineren
                    3   Drachtigheid
                    4   Droogzetten
                    5   Afkalven
                    6   Behandeling         stier ook
                    7   Ziekte              stier ook
                    8   Spoelen Embryo
                    9   Implanteren
                    10  Spenen
                    11  Bloedonderzoek      stier ook
                    12  Samenweiden
                    13  Status               stier ook
                */
            //Nieuwe code:  in gebruik
            List<LABELS> lbls = new List<LABELS>();
            if (pProgId == 3)
            {
                lbls = lMstb.GetLabels(47, pCountryCode);
            }
            else if (pProgId == 5)
            {
                lbls = lMstb.GetLabels(57, pCountryCode);
            }
            else
            {
                lbls = lMstb.GetLabels(2, pCountryCode);
            }

            for (int j = lbls.Count - 1; j >= 0; j--)
            {
                if (!pEventKinds.Contains(lbls[j].LabId))
                {
                    lbls.Remove(lbls[j]);
                }
            }
            return lbls;

            //hieronder staat nog de oude code 
            //(zet de rechten nu via utils.getgebruikerrechten)            
        }

        public static List<EVENT> getAnimalEventList(UserRightsToken pUr, ANIMAL pAnimal, ANIMALCATEGORY pAniCategory, int pFarmId, int pUbnId, int pProgramId, List<int> pEventKinds)
        {


            AFSavetoDB lMstb = getMysqlDb(pUr);

            ArrayList AND_EveKind_In = new ArrayList();

            //List<EVENT> aniUBNevents = lMstb.getEventsByAniIdUbn(pAniId, pUbnId);

            //List<EVENT> aniUBNNulevents = lMstb.getEventsByAniIdUbn(pAniId, 0);

            List<EVENT> anievents = new List<EVENT>();// aniUBNevents.Concat(aniUBNNulevents).ToList();

            StringBuilder lQuery = new StringBuilder();

            lQuery.Append(" SELECT EVENT.* FROM EVENT");
            //lQuery.AppendFormat(" WHERE AniId = {0} AND  (UBNId = {1} OR UBNId = 0) AND EventId>0 ORDER BY EveDate DESC", pAnimal.AniId, pUbnId);
            lQuery.AppendFormat(" WHERE AniId = {0}  AND EventId>0 ORDER BY EveDate DESC, EveKind DESC", pAnimal.AniId);

            System.Data.DataTable dtEvents = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), lQuery);
            foreach (DataRow drEvent in dtEvents.Rows)
            {
                EVENT ev = new EVENT();
                if (lMstb.GetDataBase().FillObject(ev, drEvent))
                {
                    anievents.Add(ev);
                }

            }
            for (int i = anievents.Count - 1; i > -1; i--)
            {
                if (!pEventKinds.Contains(anievents[i].EveKind))
                {
                    anievents.Remove(anievents[i]);
                }
            }

            //history BUG 90 BUG 914
            DateTime lastdate = getLastEventViewDate(pUr, pAniCategory, pUbnId);

            var realevents = from n in anievents
                             where n.UBNId == pUbnId || n.UBNId == 0
                             select n;
            List<EVENT> lRealEvents = realevents.ToList();
            #region worpen en insemininaties
            //historie worpen 5 en inseminaties 2 en samenweidens 12 mogen aanschouwd worden.
            //historie transplantaties 9 mogen ook aanschouwd worden.
            if (pEventKinds.Contains(2))
            {
                var t = from n in anievents
                        where n.EveKind == 2
                        select n;
                List<EVENT> anievents2 = t.ToList();// lMstb.getEventsByAniIdKind(pAniId, 2);
                //haal ze er eerst af en voeg ze er later weer bij
                for (int i = anievents.Count - 1; i > -1; i--)
                {
                    if (anievents[i].EveKind == 2)
                    { anievents.Remove(anievents[i]); }
                }
                for (int i = anievents2.Count - 1; i > -1; i--)
                {
                    if (anievents2[i].EveDate.CompareTo(lastdate) > 0)
                    { anievents2.Remove(anievents2[i]); }
                }
                foreach (EVENT erbij in anievents2)
                {
                    var eralin = from n in lRealEvents
                                 where n.EventId == erbij.EventId
                                 select n;
                    if (eralin.Count() == 0)
                    {
                        lRealEvents.Add(erbij);
                    }
                }

            }
            if (pEventKinds.Contains(9))
            {
                var t = from n in anievents
                        where n.EveKind == 9
                        select n;
                List<EVENT> anievents2 = t.ToList();
                for (int i = anievents.Count - 1; i > -1; i--)
                {
                    if (anievents[i].EveKind == 9)
                    { anievents.Remove(anievents[i]); }
                }
                for (int i = anievents2.Count - 1; i > -1; i--)
                {
                    if (anievents2[i].EveDate.CompareTo(lastdate) > 0)
                    { anievents2.Remove(anievents2[i]); }
                }
                foreach (EVENT erbij in anievents2)
                {
                    var eralin = from n in lRealEvents
                                 where n.EventId == erbij.EventId
                                 select n;
                    if (eralin.Count() == 0)
                    {
                        lRealEvents.Add(erbij);
                    }
                }

            }
            if (pEventKinds.Contains(12))
            {

                var t12 = from n12 in anievents
                          where n12.EveKind == 12
                          select n12;
                List<EVENT> anievents12 = t12.ToList();// lMstb.getEventsByAniIdKind(pAniId, 12);



                //haal ze er eerst af en voeg ze er later weer bij
                for (int i = anievents.Count - 1; i > -1; i--)
                {
                    if (anievents[i].EveKind == 12)
                    { anievents.Remove(anievents[i]); }
                }

                for (int i = anievents12.Count - 1; i > -1; i--)
                {
                    if (anievents12[i].EveDate.CompareTo(lastdate) > 0)
                    { anievents12.Remove(anievents12[i]); }
                }

                foreach (EVENT erbij in anievents12)
                {
                    var eralin = from n in lRealEvents
                                 where n.EventId == erbij.EventId
                                 select n;
                    if (eralin.Count() == 0)
                    {
                        lRealEvents.Add(erbij);
                    }
                }
            }
            if (pEventKinds.Contains(5))
            {

                var t5 = from n5 in anievents
                         where n5.EveKind == 5
                         select n5;
                List<EVENT> anievents5 = t5.ToList();// lMstb.getEventsByAniIdKind(pAniId, 5);


                //haal ze er eerst af en voeg ze er later weer bij
                for (int i = anievents.Count - 1; i > -1; i--)
                {
                    if (anievents[i].EveKind == 5)
                    { anievents.Remove(anievents[i]); }
                }

                for (int i = anievents5.Count - 1; i > -1; i--)
                {
                    if (anievents5[i].EveDate.CompareTo(lastdate) > 0)
                    { anievents5.Remove(anievents5[i]); }
                }
                foreach (EVENT erbij in anievents5)
                {
                    var eralin = from n in lRealEvents
                                 where n.EventId == erbij.EventId
                                 select n;
                    if (eralin.Count() == 0)
                    {
                        lRealEvents.Add(erbij);
                    }
                }
            }
            #endregion

            if (pProgramId == 9 || pProgramId == 91)
            {
                //historische medicijngegevens 6 en 7 ook  toevoegen voor agroscoop

                List<EVENT> anievents6 = lMstb.getEventsByAniIdKind(pAnimal.AniId, 6);
                List<EVENT> anievents7 = lMstb.getEventsByAniIdKind(pAnimal.AniId, 7);


                for (int i = anievents.Count - 1; i > -1; i--)
                {
                    if (anievents[i].EveKind == 6 || anievents[i].EveKind == 7)
                    { anievents.Remove(anievents[i]); }
                }
                for (int i = anievents6.Count - 1; i > -1; i--)
                {
                    if (anievents6[i].EveDate.CompareTo(lastdate) > 0)
                    { anievents6.Remove(anievents6[i]); }
                }
                for (int i = anievents7.Count - 1; i > -1; i--)
                {
                    if (anievents7[i].EveDate.CompareTo(lastdate) > 0)
                    { anievents7.Remove(anievents7[i]); }
                }
                foreach (EVENT erbij in anievents6)
                {
                    var eralin = from n in lRealEvents
                                 where n.EventId == erbij.EventId
                                 select n;
                    if (eralin.Count() == 0)
                    {
                        lRealEvents.Add(erbij);
                    }
                }
                foreach (EVENT erbij in anievents7)
                {
                    var eralin = from n in lRealEvents
                                 where n.EventId == erbij.EventId
                                 select n;
                    if (eralin.Count() == 0)
                    {
                        lRealEvents.Add(erbij);
                    }
                }

            }
            else
            {

            }


            //sorteren
            var temp3 = from n
                        in lRealEvents
                        orderby n.EveDate.Date descending, n.EveKind descending, n.EveOrder descending
                        select n;

            return temp3.ToList();

        }

        internal static DateTime getLastEventViewDate(UserRightsToken pUr, ANIMALCATEGORY pAnicat, int pUbnId)
        {
            DateTime last = DateTime.Now.AddDays(5);
            if (pAnicat.Anicategory > 3)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                List<MOVEMENT> afvoeren = lMstb.GetMovementsByAniId(pAnicat.AniId);
                var t = from n in afvoeren
                        where (n.MovKind == 2 || n.MovKind == 3 || n.MovKind == 5 || n.MovKind == 6)
                        && (n.UbnId == pUbnId || n.UbnId == 0)
                        orderby n.MovId descending, n.MovDate descending
                        select n;
                afvoeren = t.ToList();

                DateTime closest = DateTime.MinValue;
                if (afvoeren.Count() > 0)
                {
                    closest = afvoeren.ElementAt(0).MovDate.Date;
                }
                if (closest.CompareTo(DateTime.MinValue) > 0)
                {
                    last = closest;
                }
            }
            return last;
        }

        public static string ChangeAnimalFather(int pThrId, UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, int pNewFatherId, int pMutation, int pDekkingen, int pBrothersSisters, int pChangedBy, int pSoureceID)
        {
            /*Er wordt hier gewoon de vader veranderd, 
             * je moet zelf checken of dat in jouw geval mag, 
             * er wordt hier alleen
             * gecheckt of het een mannetje is en of die wel in de database zit.
             PARAMETERS  int pMutation 1 wel melden,2 niet melden, 3 klaarstaande melding aanpassingen, 4 niet van toepassing
             * 1 :er wordt een GEBOORTE MUTATION gemaakt. 2 en 4 Er wordt een GEBOORTE MUTALOG gemaakt. 3 bestaande GEBOORTE MUTATION wordt veranderd (als die er niet is wordt er niks gedaan)
             * int pDekkingen, 1 Dekking Of Samenweiden aanpassen, 2 niet aanpassen.
             * int pBrothersSisters 1 Broers en zusters ook aanpassen, 2 niet aanpassen.
             */
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";// Checker.checkChangeFather(pBedrijf, pAnimal, pNewFatherId);
            if (pAnimal.AniBirthDate == null || pAnimal.AniBirthDate.CompareTo(DateTime.MinValue) <= 0)
            {
                antwoord = "Geen geldige geboortedatum";
            }
            ANIMAL Nieuwevader = lMstb.GetAnimalById(pNewFatherId);

            if ((Nieuwevader.AniId == 0) || (Nieuwevader.AniSex != 1))
            {
                antwoord = "Vader is geen geldig dier";
            }
            ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);

            if (antwoord == "")
            {
                UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
                THIRD HuidigeThird = lMstb.GetThirdByThirId(ubu.ThrID);
                COUNTRY UBNLand;
                if (HuidigeThird.ThrCountry.Trim() != "")
                {
                    UBNLand = lMstb.GetCountryByLandid(int.Parse(HuidigeThird.ThrCountry));
                }
                else
                {
                    UBNLand = lMstb.GetCountryByLandid(151);
                }
                FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");


                ANIMAL OuweVader = lMstb.GetAnimalById(pAnimal.AniIdFather);


                BIRTH birt = lMstb.GetBirthByCalfId(pAnimal.AniId);
                EVENT birtevent = lMstb.GetEventdByEventId(birt.EventId);
                if ((birtevent.EventId == 0) && pAnimal.AniIdMother != 0)
                {
                    ChangeAnimalMother(pThrId, pUr, pBedrijf, pAnimal, pAnimal.AniIdMother, pMutation, pDekkingen, pBrothersSisters, pChangedBy, pSoureceID);
                    birt = lMstb.GetBirthByCalfId(pAnimal.AniId);
                    birtevent = lMstb.GetEventdByEventId(birt.EventId);
                }


                MUTATION muta = lMstb.GetMutationByEventId(birtevent.EventId);
                MUTALOG mutlog = lMstb.GetMutaLogByEventId(birtevent.EventId);

                List<EVENT> vaderwordtEvents = new List<EVENT>();

                if (pDekkingen == 1)
                {
                    if (moeder.AniId > 0)
                    {
                        Event_functions.getWorpFathers(pUr, pBedrijf, moeder, pAnimal.AniBirthDate, out vaderwordtEvents);
                        if (vaderwordtEvents.Count() > 0)
                        {
                            antwoord = switchvaderDekEvents(pUr, vaderwordtEvents.ElementAt(0), Nieuwevader);
                        }
                    }
                }
                if (antwoord == "")
                {
                    if (pBrothersSisters == 1)
                    {
                        unLogger.WriteInfo("Parameters:" + pAnimal.AniBirthDate.ToLongDateString() + "-" + moeder.AniId.ToString() + "-" + "5" + "-" + pBedrijf.UBNid.ToString());
                        List<EVENT> siblings = lMstb.getEventsByDateAniIdKindUbn(pAnimal.AniBirthDate, moeder.AniId, 5, pBedrijf.UBNid);
                        List<EVENT> siblingsUBNnul = lMstb.getEventsByDateAniIdKindUbn(pAnimal.AniBirthDate, moeder.AniId, 5, 0);
                        siblings = siblings.Concat(siblingsUBNnul).ToList();

                        if (siblings.Count() == 0)
                        {
                            pAnimal.AniIdFather = pNewFatherId;
                            lMstb.UpdateANIMAL(pThrId, pAnimal);
                        }
                        else
                        {
                            foreach (EVENT geb in siblings)
                            {
                                BIRTH gebSibling = lMstb.GetBirth(geb.EventId);
                                gebSibling.AniFatherID = Nieuwevader.AniId;
                                if (lMstb.SaveBirth(gebSibling))
                                {
                                    ANIMAL sibbel = lMstb.GetAnimalById(gebSibling.CalfId);
                                    sibbel.AniIdFather = Nieuwevader.AniId;
                                    sibbel.AniStatus = berekenStatus(pUr, Nieuwevader.AniId, sibbel.AniIdMother, pBedrijf);

                                    if (lMstb.UpdateANIMAL(pThrId, sibbel))
                                    {
                                        setSECONRACRasdelen(pUr, sibbel, pBedrijf);

                                        bool setmutlog = true;
                                        if (FCIRviaModem.FValue.ToLower() == "true")
                                        {
                                            if (pMutation == 1)
                                            {
                                                saveNewMutation(pUr, 2, pBedrijf, "", "", geb, new MOVEMENT(), sibbel, moeder, Nieuwevader, pChangedBy, pSoureceID);
                                                setmutlog = false;
                                            }
                                            else if (pMutation == 3)
                                            {
                                                MUTATION repair = lMstb.GetMutationByEventId(geb.EventId);
                                                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                                                {
                                                    repair.LifeNumberFather = Nieuwevader.AniAlternateNumber;
                                                }
                                                else
                                                {
                                                    repair.LifenumberMother = Nieuwevader.AniLifeNumber;
                                                }
                                                if (repair.EventId != 0)
                                                {
                                                    lMstb.SaveMutation(repair);
                                                }
                                                setmutlog = false;
                                            }

                                        }
                                        if (setmutlog)
                                        {
                                            saveNewMutaLog(pUr, 2, pBedrijf, "", "", geb, new MOVEMENT(), sibbel, moeder, Nieuwevader, pChangedBy, pSoureceID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (antwoord == "")
                        {
                            birt.AniFatherID = Nieuwevader.AniId;

                            if (lMstb.SaveBirth(birt))
                            {

                                pAnimal.AniIdFather = Nieuwevader.AniId;
                                pAnimal.AniStatus = berekenStatus(pUr, Nieuwevader.AniId, pAnimal.AniIdMother, pBedrijf);

                                lMstb.UpdateANIMAL(pThrId, pAnimal);
                                setSECONRACRasdelen(pUr, pAnimal, pBedrijf);

                            }
                        }
                    }
                    if (pBrothersSisters != 1)
                    {
                        bool setMutalog = true;

                        if (FCIRviaModem.FValue.ToLower() == "true")
                        {
                            if (pMutation == 1)
                            {
                                saveNewMutation(pUr, 2, pBedrijf, "", "", birtevent, new MOVEMENT(), pAnimal, moeder, Nieuwevader, pChangedBy, pSoureceID);
                                setMutalog = false;
                            }
                            else if (pMutation == 3)
                            {
                                if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                                {
                                    muta.LifeNumberFather = Nieuwevader.AniAlternateNumber;
                                }
                                else
                                {
                                    muta.LifeNumberFather = Nieuwevader.AniLifeNumber;
                                }
                                if (muta.EventId != 0)
                                {
                                    lMstb.SaveMutation(muta);
                                }
                                setMutalog = false;
                            }
                        }
                        if (setMutalog)
                        {
                            saveNewMutaLog(pUr, 2, pBedrijf, "", "", birtevent, new MOVEMENT(), pAnimal, moeder, Nieuwevader, pChangedBy, pSoureceID);

                        }
                    }
                }

            }
            return antwoord;
        }

        public static void saveNewMutation(UserRightsToken pUr, int pCodeMutation, BEDRIJF pBedrijf, string pBedrijffrom, string pBedrijfto, EVENT pEvent, MOVEMENT pMovement, ANIMAL pAnimal, ANIMAL pMoeder, ANIMAL pVader, int pChangedBy, int pSourceID)
        {
            saveNewMutation(pUr, pCodeMutation, pBedrijf, pBedrijffrom, pBedrijfto, pEvent, pMovement, pAnimal, pMoeder, pVader, 0, pChangedBy, pSourceID);
        }

        public static void saveNewMutation(UserRightsToken pUr, int pCodeMutation, BEDRIJF pBedrijf, string pBedrijffrom, string pBedrijfto, EVENT pEvent, MOVEMENT pMovement, ANIMAL pAnimal, ANIMAL pMoeder, ANIMAL pVader, int Returnresult, int pChangedBy, int pSourceID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            /*MUTATION CodeMutation
                LABELS LabKind 102

                528	102	1	Aanvoer
                528	102	2	Geboorte
                528	102	3	Doodgeb.
                528	102	4	Afvoer
                528	102	5	IKB afvoer
                528	102	6	Dood
                528	102	7	Import
                528	102	8	Aan-/afvoer
                528	102	9	Export
                528	102	10	Slacht
             */
            UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
            MUTATION mutnew = new MUTATION();
            mutnew.EventId = pEvent.EventId;
            mutnew.MovId = pMovement.MovId;
            mutnew.Returnresult = Returnresult;
            mutnew.IDRBirthDate = pAnimal.AniBirthDate;
            mutnew.CountryCodeBirth = pAnimal.AniCountryCodeBirth;
            mutnew.CodeMutation = pCodeMutation;
            if (pMovement.MovKind == 3 || pMovement.MovKind == 2)
            {
                if (pMovement.MovKind == 2)
                {
                    SALE sal = lMstb.GetSale(pMovement.MovId);
                    if (sal.SalKind == 2 || sal.SalKind == 3)
                    {
                        mutnew.CodeMutation = 10;//slacht
                        mutnew.IDRLossDate = pMovement.MovDate;
                        mutnew.LossDate = mutnew.IDRLossDate;
                    }
                }
                else { mutnew.IDRLossDate = pMovement.MovDate; mutnew.LossDate = mutnew.IDRLossDate; }

            }
            mutnew.AlternateLifeNumber = pAnimal.AniAlternateNumber;
            mutnew.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, pAnimal.AniAlternateNumber);
            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                mutnew.Lifenumber = pAnimal.AniAlternateNumber;
                mutnew.LifenumberMother = pMoeder.AniAlternateNumber;
                mutnew.LifeNumberFather = pVader.AniAlternateNumber;
            }
            else
            {
                mutnew.Lifenumber = pAnimal.AniLifeNumber;
                mutnew.LifenumberMother = pMoeder.AniLifeNumber;
                mutnew.LifeNumberFather = pVader.AniLifeNumber;
            }

            mutnew.Farmnumber = ubu.Bedrijfsnummer;
            mutnew.Nling = pAnimal.AniNling;
            mutnew.Sex = pAnimal.AniSex;
            mutnew.UbnId = ubu.UBNid;
            mutnew.FarmNumberFrom = pBedrijffrom;
            mutnew.FarmNumberTo = pBedrijfto;
            mutnew.Haircolor = pAnimal.Anihaircolor;
            mutnew.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
            if (pBedrijf.ProgId == 25)
            {
                mutnew.SendTo = 35;
            }
            else
            {
                RUMAIenRMeldingen r = new RUMAIenRMeldingen();
                string defIenRaction = r.getdefIenRaction(pUr, pBedrijf.UBNid, pBedrijf.ProgId, pBedrijf.Programid);
                FARMCONFIG IenRCom = Facade.GetInstance().getSaveToDB(pUr).getFarmConfig(pBedrijf.FarmId, "VerstuurIenR", defIenRaction);

                mutnew.SendTo = IenRCom.ValueAsInteger();
            }
            switch (pCodeMutation)
            {
                case 2:
                    mutnew.MutationDate = pEvent.EveDate;
                    mutnew.MutationTime = pEvent.EveMutationTime;
                    mutnew.Worknumber = "";
                    break;
                case 6:
                    mutnew.MutationDate = pMovement.MovDate;
                    mutnew.MutationTime = pMovement.MovMutationTime;
                    ANIMALCATEGORY anicatloss = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
                    mutnew.Worknumber = anicatloss.AniWorknumber;
                    mutnew.IDRLossDate = pMovement.MovDate;
                    break;
                default:
                    mutnew.MutationDate = pMovement.MovDate;
                    mutnew.MutationTime = pMovement.MovMutationTime;
                    ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
                    mutnew.Worknumber = anicat.AniWorknumber;
                    break;
            }
            mutnew.Changed_By = pChangedBy;
            mutnew.SourceID = pSourceID;
            lMstb.SaveMutation(mutnew);
        }

        public static int getOrCreateBirnr(UserRightsToken pUr, int pMoederAniId, DateTime geboortedag)
        {
            //Een worp krijgt nog hetzelfde birNr als er worpen 7 dagen ervoor of erna gebeurd zijn
            int ret = 1;
            if (pUr != null)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                if (pMoederAniId > 0 && geboortedag.CompareTo(DateTime.MinValue.AddDays(7)) > 0)
                {
                    List<EVENT> liworpen = lMstb.getEventsByAniIdKind(pMoederAniId, 5);
                    List<BIRTH> libirths = lMstb.GetBirthsByAnimal(pMoederAniId);
                    if (liworpen.Count > 0)
                    {
                        var sortedev = from n in liworpen
                                       orderby n.EveDate
                                       select n;
                        liworpen = sortedev.ToList();
                        int birnr = 1;
                        DateTime lastworpdate = liworpen.ElementAt(0).EveDate.Date;
                        bool toegewezen = false;
                        foreach (EVENT worp in liworpen)
                        {
                            //werkt alleen als die ergens netjes inpast
                            if (worp.EveDate.Date > lastworpdate.AddDays(7))
                            {
                                birnr = birnr + 1;
                                lastworpdate = worp.EveDate.Date;
                            }
                            if (lastworpdate == DateTime.MinValue)
                            {
                                lastworpdate = DateTime.MinValue.AddYears(100);
                            }
                            if (geboortedag.Date >= lastworpdate.AddDays(-7) && geboortedag.Date <= lastworpdate.AddDays(7))
                            {
                                ret = birnr;
                                toegewezen = true;
                            }

                            var bird = from n in libirths
                                       where n.EventId == worp.EventId
                                       select n;
                            if (bird.Count() > 0)
                            {
                                BIRTH aanpassen = bird.ElementAt(0);
                                bird.ElementAt(0).BirNumber = birnr;
                                aanpassen.BirNumber = birnr;
                                lMstb.SaveBirth(aanpassen);
                            }
                        }
                        if (toegewezen)
                        {
                            return ret;
                        }
                        if (liworpen.OrderByDescending(w => w.EveDate).First().EveDate < geboortedag.AddDays(-7))
                        {
                            //Nieuw birNr
                            BIRTH b = lMstb.GetBirth(liworpen.OrderByDescending(w => w.EveDate).First().EventId);
                            return b.BirNumber + 1;
                        }
                        else
                        {
                            //BirNr valt ertussenin/ervoor

                            var worpen = liworpen.Where(w => (w.EveDate >= geboortedag.AddDays(-7)) && (w.EveDate <= geboortedag.AddDays(7))).ToList();
                            if (worpen.Count > 0)
                            {
                                //Is een bestaand BirNr //MAAR De Birh is al opgeslagen met Birthnr = 0 en ik wil een nieuw Birnr
                                //kan ook 
                                int i = worpen.First().EventId;
                                var check = from n in libirths
                                            where n.EventId == i
                                            select n;
                                if (check.Count() > 0)
                                {
                                    if (check.ElementAt(0).BirNumber > 0)
                                    {
                                        ret = check.ElementAt(0).BirNumber;
                                    }
                                    else
                                    {
                                        //Birthnr = 0
                                        var worpenervoor = liworpen.Where(w => (w.EveDate < geboortedag.AddDays(-7)));
                                        var worpenerna = liworpen.Where(w => (w.EveDate >= geboortedag.AddDays(7)));
                                        ret = 1;
                                        if (worpenervoor.Count() > 0)
                                        {
                                            int id = worpenervoor.First().EventId;
                                            var check2 = from n in libirths
                                                         where n.EventId == id
                                                         select n;
                                            if (check2.Count() > 0)
                                            {
                                                ret = check2.ElementAt(0).BirNumber + 1;
                                                if (worpenerna.Count() > 0)
                                                {
                                                    foreach (EVENT e in worpenerna)
                                                    {
                                                        BIRTH b = lMstb.GetBirth(e.EventId);
                                                        b.BirNumber = b.BirNumber + 1;
                                                        lMstb.SaveBirth(b);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Nieuw BirNr er tussenin zetten
                                var worpenVoor = liworpen.Where(w => w.EveDate < geboortedag).ToList();
                                var worpenNa = liworpen.Where(w => w.EveDate > geboortedag).ToList();

                                var lvoor = worpenVoor.OrderByDescending(w => w.EveDate);
                                int eVoor = 1;
                                if (lvoor.Count() > 0)
                                    eVoor = lvoor.First().EventId;

                                int iWorpVoor = lMstb.GetBirth(eVoor).BirNumber;

                                int eNa = worpenNa.OrderBy(w => w.EveDate).First().EventId;


                                int iWorpNa = lMstb.GetBirth(eNa).BirNumber;

                                if (iWorpNa > iWorpVoor + 1)
                                    return iWorpVoor + 1;

                                ret = iWorpNa;

                                foreach (EVENT e in worpenNa)
                                {
                                    BIRTH b = lMstb.GetBirth(e.EventId);
                                    b.BirNumber = b.BirNumber + 1;
                                    lMstb.SaveBirth(b);
                                }


                            }

                        }


                    }
                    else
                    { return 1; }
                }
                else
                { return 1; }
            }
            else
            { return 1; }
            return ret;
        }

        public static void saveNewMutaLog(UserRightsToken pUr, int pCodeMutation, BEDRIJF pBedrijf, string pBedrijffrom, string pBedrijfto, EVENT pEvent, MOVEMENT pMovement, ANIMAL pAnimal, ANIMAL pMoeder, ANIMAL pVader, int pChangedBy, int pSourceID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            /*MUTATION CodeMutation
                LABELS LabKind 102

                528	102	1	Aanvoer
                528	102	2	Geboorte
                528	102	3	Doodgeb.
                528	102	4	Afvoer
                528	102	5	IKB afvoer
                528	102	6	Dood
                528	102	7	Import
                528	102	8	Aan-/afvoer
                528	102	9	Export
                528	102	10	Slacht
             */
            UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
            MUTALOG mutnew = new MUTALOG();

            mutnew.EventId = pEvent.EventId;
            mutnew.MovId = pMovement.MovId;
            mutnew.IDRBirthDate = pAnimal.AniBirthDate;
            mutnew.CodeMutation = pCodeMutation;
            mutnew.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, pAnimal.AniAlternateNumber);
            mutnew.AlternateLifeNumber = pAnimal.AniAlternateNumber;

            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                mutnew.Lifenumber = pAnimal.AniAlternateNumber;
                mutnew.LifenumberMother = pMoeder.AniAlternateNumber;
                mutnew.LifeNumberFather = pVader.AniAlternateNumber;
            }
            else
            {
                mutnew.Lifenumber = pAnimal.AniLifeNumber;
                mutnew.LifenumberMother = pMoeder.AniLifeNumber;
                mutnew.LifeNumberFather = pVader.AniLifeNumber;
            }

            mutnew.Farmnumber = ubu.Bedrijfsnummer;
            mutnew.Nling = pAnimal.AniNling;
            mutnew.Sex = pAnimal.AniSex;
            mutnew.UbnId = ubu.UBNid;
            mutnew.FarmNumberFrom = pBedrijffrom;
            mutnew.FarmNumberTo = pBedrijfto;
            mutnew.Haircolor = pAnimal.Anihaircolor;
            mutnew.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
            mutnew.IDRLossDate = pMovement.MovDate;

            switch (pCodeMutation)
            {
                case 2:
                    mutnew.MutationDate = pEvent.EveDate;
                    mutnew.MutationTime = pEvent.EveMutationTime;
                    mutnew.Worknumber = "";
                    break;
                case 6:
                    mutnew.MutationDate = pMovement.MovDate;
                    mutnew.MutationTime = pMovement.MovMutationTime;
                    ANIMALCATEGORY anicatloss = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
                    mutnew.Worknumber = anicatloss.AniWorknumber;
                    mutnew.IDRLossDate = pMovement.MovDate;
                    break;
                default:
                    mutnew.MutationDate = pMovement.MovDate;
                    mutnew.MutationTime = pMovement.MovMutationTime;
                    ANIMALCATEGORY anicat = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
                    mutnew.Worknumber = anicat.AniWorknumber;
                    break;
            }
            mutnew.Changed_By = pChangedBy;
            mutnew.SourceID = pSourceID;
            lMstb.InsertMutLog(mutnew);
        }

        private static string switchvaderDekEvents(UserRightsToken pUr, EVENT dekevent, ANIMAL nieuwVader)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            if (dekevent.EventId != 0)
            {

                INSEMIN ins = lMstb.GetInsemin(dekevent.EventId);
                if (ins.EventId == 0)
                {
                    GRZTOGTH graas = lMstb.GetGRZTOGTHByEventId(dekevent.EventId);
                    graas.AniIdFather = nieuwVader.AniId;
                    if (!lMstb.SaveGRZTOGTH(graas))
                    {
                        antwoord = "veranderen van vader niet gelukt.";
                    }
                }
                else
                {
                    ins.AniIdFather = nieuwVader.AniId;
                    if (!lMstb.SaveInsemin(ins))
                    {
                        antwoord = "veranderen van vader niet gelukt.";
                    }
                    else if (ins.InsKind > 1)
                    {
                        SavenewDHZ(pUr, dekevent, ins);
                    }
                }

            }
            return antwoord;
        }

        public static void getWorpFathers(UserRightsToken pUr, BEDRIJF bedr, ANIMAL aniMother, DateTime worpdatum, out List<EVENT> VaderEvents)
        {

            AFSavetoDB lMstb = getMysqlDb(pUr);
            if (worpdatum.CompareTo(DateTime.MinValue) == 0)
            {
                worpdatum = worpdatum.AddYears(5);
            }
            int draagtijd = Checker.getDraagtijd(pUr, bedr);


            DateTime lPossibleDekDate = worpdatum.AddDays(-draagtijd);
            DateTime lMinimumDekDate = lPossibleDekDate.AddMonths(-6);//Worpdatum - de draagtijd en dan nog eens 6 maanden daarvoor, anders komen de eventuele vaders van het jaar daarvoor in beeld
            unLogger.WriteInfo("draagtijd=" + draagtijd.ToString() + " lPossibleDekDate=" + lPossibleDekDate.ToString("dd-MM-yyyy"));

            ANIMAL vader = new ANIMAL();
            VaderEvents = new List<EVENT>();

            StringBuilder sbEvents = new StringBuilder();
            sbEvents.Append(" SELECT EVENT.*,GRZTOGTH.EndDate,GRZTOGTH.AniIdFather AS GRZTOGTHFather,INSEMIN.AniIdFather AS INSEMINFather,INSEMIN.InsPMSG AS INSEMINInsPMSG FROM EVENT ");
            sbEvents.Append(" LEFT JOIN GRZTOGTH ON GRZTOGTH.EventId = EVENT.EventId  ");
            sbEvents.Append(" LEFT JOIN INSEMIN ON INSEMIN.EventId = EVENT.EventId  ");
            sbEvents.Append(" WHERE AniId = " + aniMother.AniId.ToString());
            sbEvents.Append(" AND ( EVENT.EveKind = 2 OR EVENT.EveKind = 12) AND EVENT.EveDate>" + lMstb.MySQL_Datum(lMinimumDekDate, 0) + " AND EVENT.EventId>0 ");
            sbEvents.Append(" ORDER BY EVENT.EveDate DESC ,EVENT.EventId DESC ");

            DataTable tblEvents = new DataTable();
            if (aniMother.AniId > 0)
            {
                tblEvents = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sbEvents);
            }

            //List<EVENT> dekkingen = lMstb.getEventsByAniIdKindUbn(aniMother.AniId, 2, bedr.UBNid);
            //List<EVENT> samenweidenens = lMstb.getEventsByAniIdKindUbn(aniMother.AniId, 12, bedr.UBNid);
            //List<EVENT> dekkingenUbnNul = lMstb.getEventsByAniIdKindUbn(aniMother.AniId, 2, 0);
            //List<EVENT> samenweidenensUbnNul = lMstb.getEventsByAniIdKindUbn(aniMother.AniId, 12, 0);
            //dekkingen = dekkingen.Concat(samenweidenens).ToList();
            //dekkingen = dekkingen.Concat(dekkingenUbnNul).ToList();
            //dekkingen = dekkingen.Concat(samenweidenensUbnNul).ToList();
            //var temp = from n
            //           in dekkingen
            //           orderby n.EveDate descending, n.EventId descending
            //           select n;
            int evid = 0;

            DateTime laatstedekking = DateTime.MinValue;
            if (tblEvents.Rows.Count > 0)
            {
                foreach (DataRow rw in tblEvents.Rows)
                {

                    EVENT dekevent = new EVENT();
                    Event_functions.FillDataObjectFromDataRow(rw, dekevent);
                    laatstedekking = dekevent.EveDate.Date;
                    if (dekevent.EveKind == 12)
                    {
                        //GRZTOGTH gr = lMstb.GetGRZTOGTHByEventId(dekevent.EventId);

                        DateTime einddatum = getDatumFormat(rw["EndDate"], "Event_functions getWorpFather GRZTOGTH EndDate ");
                        if (dekevent.EveDate.Date.AddDays(-10).CompareTo(lPossibleDekDate) <= 0 && einddatum.Date.AddDays(10).CompareTo(lPossibleDekDate) >= 0)
                        {
                            laatstedekking = dekevent.EveDate;
                            evid = dekevent.EventId;
                            VaderEvents.Add(dekevent);
                            //if (lPossibleDekDate.Date.CompareTo(dekevent.EveDate.Date) >= 0 && lPossibleDekDate.Date.CompareTo(einddatum.Date) <= 0)
                            //{
                            //    break;
                            //}
                        }
                        else
                        {
                            if (bedr.Programid == 16 || bedr.Programid == 160)
                            {
                                VaderEvents.Add(dekevent);
                            }
                        }
                    }
                    else
                    {
                        if (dekevent.EveDate.Date.AddDays(-10).CompareTo(lPossibleDekDate) <= 0 && dekevent.EveDate.Date.AddDays(10).CompareTo(lPossibleDekDate) >= 0)
                        {
                            laatstedekking = dekevent.EveDate;
                            evid = dekevent.EventId;
                            VaderEvents.Add(dekevent);

                        }
                        else
                        {
                            if (bedr.Programid == 16 || bedr.Programid == 160)
                            {
                                if (dekevent.EveDate.AddDays(10) <= worpdatum)
                                {
                                    VaderEvents.Add(dekevent);
                                }
                            }
                        }
                    }
                }
            }


            int aniidvader = 0;
            if (evid > 0)
            {
                var Vaders = (from n in VaderEvents
                              where n.EventId == evid
                              select n).ToList();
                EVENT VaderEvent = new EVENT();
                if (Vaders.Count() > 0)
                {
                    VaderEvent = Vaders.ElementAt(0);
                }

                DataRow[] FoundRows = tblEvents.Select("EventId = " + evid.ToString() + " ");
                if (VaderEvent.EveKind == 2)
                {
                    if (FoundRows.Length > 0)
                    {
                        int.TryParse(FoundRows[0]["INSEMINFather"].ToString(), out aniidvader);
                    }
                }
                else
                {

                    if (FoundRows.Length > 0)
                    {
                        int.TryParse(FoundRows[0]["GRZTOGTHFather"].ToString(), out aniidvader);
                    }

                }
                if (aniidvader > 0)
                {
                    //vader = lMstb.GetAnimalById(aniidvader);
                }

            }
            //return aniidvader;
        }

        public static void getWorpFathersNsfo(UserRightsToken pUr, BEDRIJF bedr, ANIMAL aniMother, DateTime worpdatum, out List<EVENT> pVaderEvents, out ANIMAL pFather, out EVENT pDekevent, out List<EVENT> pMultipleFathers)
        {
            /*
             Hallo Nico,
                De overlap van 10 dagen voor en na de dekdatum / periode mag aangescherpt worden naar 5 dagen voor en na de dekdatum / periode.
                Vervolgens mag er tot maximaal 8 dagen na (de dekdatum + 143 dagen), nog gekozen worden voor de ram die bij deze periode / datum hoort. Dus 152 dagen na de dekperiode / datum kan een ram niet meer in aanmerking komen voor het vaderschap van de lammeren.
                Aan de voorkant leggen we geen begrenzing op, ooien kunnen nl wel eens 4 weken te vroeg aflammeren en het is wel van belang dat de goede ram dan bij de worp ingevoerd kan worden.
                Mocht het niet duidelijk zijn, dan hoor ik het graag.
                Met vriendelijke groet,
                Marjo van Bergen
             * 
             * 15-4-2015
             * Reinard en ik hebben nog eens nagedacht over onderstaand probleem en wij vinden het wenselijk dat, 
             * wanneer er 2 rammen in aanmerking komen voor het vaderschap 
             * (zij hebben 143 dagen (met een marge van -5 en + 5) voor de lamdatum bij de ooi gelopen, 
             * er een pop up scherm verschijnt, die meldt dat meerdere rammen als vader in aanmerking komen. 
             * Vervolgens moet de fokker dan zelf de juiste vader aanklikken.
             */
            pFather = new ANIMAL();
            pVaderEvents = new List<EVENT>();
            pMultipleFathers = new List<EVENT>();
            pDekevent = new EVENT();
            AFSavetoDB lMstb = getMysqlDb(pUr);
            if (worpdatum.CompareTo(DateTime.MinValue) == 0)
            {
                return;
            }

            int draagtijd = Checker.getDraagtijd(pUr, bedr);


            DateTime lExactDekDate = worpdatum.AddDays(-draagtijd);
            DateTime lMinimumDekDate = lExactDekDate.AddMonths(-6);//Worpdatum - de draagtijd en dan nog eens 6 maanden daarvoor, anders komen de eventuele vaders van het jaar daarvoor in beeld
            unLogger.WriteInfo("draagtijd=" + draagtijd.ToString() + " lPossibleDekDate=" + lExactDekDate.ToString("dd-MM-yyyy"));



            StringBuilder sbEvents = new StringBuilder();
            sbEvents.Append(" SELECT EVENT.*,GRZTOGTH.EndDate,GRZTOGTH.AniIdFather AS GRZTOGTHFather,INSEMIN.AniIdFather AS INSEMINFather FROM EVENT ");
            sbEvents.Append(" LEFT JOIN GRZTOGTH ON GRZTOGTH.EventId = EVENT.EventId  ");
            sbEvents.Append(" LEFT JOIN INSEMIN ON INSEMIN.EventId = EVENT.EventId  ");
            sbEvents.Append(" WHERE AniId = " + aniMother.AniId.ToString());
            sbEvents.Append(" AND ( EVENT.EveKind = 2 OR EVENT.EveKind = 12) AND EVENT.EventId>0 AND EVENT.EveDate>" + lMstb.MySQL_Datum(lMinimumDekDate, 0) + " ");
            sbEvents.Append(" ORDER BY EVENT.EveDate DESC ,EVENT.EventId DESC ");

            DataTable tblEvents = new DataTable();
            if (aniMother.AniId > 0)
            {
                tblEvents = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sbEvents);
            }

            int evid = 0;
            int aniidvader = 0;

            if (tblEvents.Rows.Count > 0)
            {
                foreach (DataRow rw in tblEvents.Rows)
                {
                    EVENT dekevent = new EVENT();
                    Event_functions.FillDataObjectFromDataRow(rw, dekevent);
                    pVaderEvents.Add(dekevent);
                    if (dekevent.EveKind == 12)
                    {
                        //dekperiode

                        DateTime laatstedekdatum = getDatumFormat(rw["EndDate"], "Event_functions getWorpFather GRZTOGTH EndDate ");
                        if (worpdatum < laatstedekdatum.Date.AddDays(draagtijd + 8))
                        {


                            if (lExactDekDate >= dekevent.EveDate.Date.AddDays(-5) && lExactDekDate <= laatstedekdatum.Date.AddDays(5))
                            {
                                evid = dekevent.EventId;
                                int.TryParse(rw["GRZTOGTHFather"].ToString(), out aniidvader);
                                pDekevent = dekevent;
                                pMultipleFathers.Add(dekevent);
                            }

                        }
                    }
                    else
                    {
                        //dekking
                        if (worpdatum < dekevent.EveDate.Date.AddDays(draagtijd + 8))
                        {


                            if (lExactDekDate >= dekevent.EveDate.Date.AddDays(-5) && lExactDekDate <= dekevent.EveDate.Date.AddDays(5))
                            {
                                evid = dekevent.EventId;
                                int.TryParse(rw["INSEMINFather"].ToString(), out aniidvader);
                                pDekevent = dekevent;
                                pMultipleFathers.Add(dekevent);
                            }

                        }
                    }
                }
            }
            if (aniidvader > 0)
            {
                pFather = lMstb.GetAnimalById(aniidvader);
            }
            else
            {
                //if (VaderEvents.Count() > 0)
                //{
                //    pDekevent = VaderEvents.ElementAt(0);
                //    if (pDekevent.EveKind == 2)
                //    {
                //        INSEMIN ins = lMstb.GetInsemin(pDekevent.EventId);
                //        aniidvader = ins.AniIdFather;
                //    }
                //    else
                //    {
                //        GRZTOGTH grz = lMstb.GetGRZTOGTHByEventId(pDekevent.EventId);
                //        aniidvader = grz.AniIdFather;
                //    }

                //}
                //if (aniidvader > 0)
                //{
                //    pFather = lMstb.GetAnimalById(aniidvader);
                //}
            }
        }

        public static string ChangeGeslacht(int pThrId, UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string ret = "";
            ANIMALCATEGORY cat = lMstb.GetAnimalCategoryByIdandFarmid(pAnimal.AniId, pBedrijf.FarmId);
            if (pAnimal.AniSex == 1)
            {
                cat.Anicategory = 3;
            }
            else
            {
                cat.Anicategory = 1;
            }
            lMstb.UpdateANIMAL(pThrId, pAnimal);
            if (cat.AniWorknumber == "")
            {
                THIRD HuidigeThird = lMstb.GetThirdByThirId(pThrId);
                string Stamboeknr = Event_functions.getStamboekNr(pBedrijf, HuidigeThird);
                int Hvolgnr = Event_functions.getHoogsteVolgnr(pUr, pBedrijf, Stamboeknr, lMstb.GetAnimalsByFarmId(pBedrijf.FarmId));
                string worknr = "";
                string tempnr = "";
                ANIMAL vader = new ANIMAL();
                if (pAnimal.AniIdFather > 0)
                {
                    vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                }
                FARMCONFIG fcon = lMstb.getFarmConfig(pBedrijf.FarmId, "rfid", "1");
                Event_functions.getRFIDNumbers(pUr, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, Hvolgnr, out worknr, out tempnr);
                cat.AniWorknumber = worknr;
            }
            lMstb.SaveAnimalCategory(cat);
            return ret;
        }

        public static string ChangeAnimalMother(int pThrId, UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, int pNewMotherId, int pMutation, int pDekkingen, int pBrothersSisters, int pChangedBy, int pSoureceID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            /*Er wordt hier gewoon de moeder veranderd,  

             PARAMETERS  int pMutation 1 wel melden,2 niet melden, 3 klaarstaande melding aanpassingen, 4 niet van toepassing
             * 1 :er wordt een GEBOORTE MUTATION gemaakt. 2 en 4 Er wordt een GEBOORTE MUTALOG gemaakt. 3 bestaande GEBOORTE MUTATION wordt veranderd (als die er niet is wordt er niks gedaan)
             * int pDekkingen, 1 Dekking Of Samenweiden aanpassen, 2 niet aanpassen.
             * int pBrothersSisters 1 Broers en zusters ook aanpassen, 2 niet aanpassen.
             */
            ANIMAL Nieuwemoeder = lMstb.GetAnimalById(pNewMotherId);
            string antwoord = "";// Checker.checkChangeMother(pBedrijf, pAnimal, pNewMotherId);
            if (Nieuwemoeder.AniId < 1 || Nieuwemoeder.AniSex != 2)
            {
                antwoord = "Nieuwe moeder is geen geldig dier";
            }
            if (antwoord == "")
            {
                bool noOldMother = false;
                if (pAnimal.AniIdMother > 0)
                {
                    noOldMother = true;
                }
                UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
                THIRD HuidigeThird = lMstb.GetThirdByThirId(ubu.ThrID);
                COUNTRY UBNLand;
                if (HuidigeThird.ThrCountry.Trim() != "")
                {
                    UBNLand = lMstb.GetCountryByLandid(int.Parse(HuidigeThird.ThrCountry));
                }
                else
                {
                    UBNLand = lMstb.GetCountryByLandid(151);
                }
                FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");

                ANIMAL Ouwemoeder = new ANIMAL();
                if (pAnimal.AniIdMother > 0)
                {
                    Ouwemoeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                }
                ANIMAL vaders = new ANIMAL();
                if (pAnimal.AniIdFather > 0)
                {
                    vaders = lMstb.GetAnimalById(pAnimal.AniIdFather);
                }
                BIRTH birt = lMstb.GetBirthByCalfId(pAnimal.AniId);
                EVENT birtevent = lMstb.GetEventdByEventId(birt.EventId);
                MUTATION muta = lMstb.GetMutationByEventId(birtevent.EventId);
                MUTALOG mutlog = lMstb.GetMutaLogByEventId(birtevent.EventId);
                List<EVENT> zwangerwordtEvents;
                Event_functions.getWorpFathers(pUr, pBedrijf, Ouwemoeder, pAnimal.AniBirthDate, out zwangerwordtEvents);

                if (birt.EventId > 0)
                {
                    lMstb.DeleteBirth(birt);
                    lMstb.DeleteEvent(birtevent);
                    birt = new BIRTH();
                    birtevent = new EVENT();
                }
                if (Nieuwemoeder.AniId > 0)
                {
                    pAnimal.AniIdMother = Nieuwemoeder.AniId;
                    pAnimal.AniStatus = berekenStatus(pUr, pAnimal.AniIdFather, Nieuwemoeder.AniId, pBedrijf);
                    if (lMstb.UpdateANIMAL(pThrId, pAnimal))
                    {
                        createMotherBirthEvents(pUr, pAnimal, Nieuwemoeder, pBedrijf, out birtevent, out birt);
                        setNling(pThrId, pUr, pBedrijf.UBNid, Nieuwemoeder.AniId, birt.BirNumber);

                        int birNrOuweMoeder = getOrCreateBirnr(pUr, Ouwemoeder.AniId, pAnimal.AniBirthDate);
                        setNling(pThrId, pUr, pBedrijf.UBNid, Ouwemoeder.AniId, birNrOuweMoeder);

                        setSECONRACRasdelen(pUr, pAnimal, pBedrijf);

                        //Nling wordt wel gezet maar 
                        //Als de methode terugkomt in Tabblad dier 
                        //worden de gegevens die er staan ingevuld opgeslagen
                        //dwz Nling was 2 ; Nling wordt 1 hier ; maar wordt hierna weer 2 , omdat dat stond ingevuld op
                        //tabblad dier 
                    }
                }


                if (pDekkingen == 1)
                {
                    if (zwangerwordtEvents.Count() > 0)
                    {
                        zwangerwordtEvents.ElementAt(0).AniId = Nieuwemoeder.AniId;
                        lMstb.SaveEvent(zwangerwordtEvents.ElementAt(0));
                    }

                }

                if (pBrothersSisters == 1)
                {
                    if (Ouwemoeder.AniId > 0)
                    {
                        List<EVENT> siblings = lMstb.getEventsByAniIdKind(Ouwemoeder.AniId, 5);
                        var temp = from n in siblings
                                   where n.EveDate.AddDays(-10) <= pAnimal.AniBirthDate && pAnimal.AniBirthDate <= n.EveDate.AddDays(10)
                                   select n;
                        siblings = temp.ToList();

                        if (siblings.Count() == 0)
                        {
                            siblings.Add(birtevent);
                        }

                        int birNr = getOrCreateBirnr(pUr, Nieuwemoeder.AniId, pAnimal.AniBirthDate);
                        if (siblings.Count() > 0)
                        {
                            foreach (EVENT geb in siblings)
                            {
                                BIRTH siblinggeboorte = lMstb.GetBirth(geb.EventId);
                                siblinggeboorte.BirNumber = birNr;
                                if (lMstb.SaveBirth(siblinggeboorte))
                                {
                                    geb.AniId = Nieuwemoeder.AniId;
                                    if (lMstb.SaveEvent(geb))
                                    {
                                        ANIMAL sibbel = lMstb.GetAnimalById(siblinggeboorte.CalfId);

                                        sibbel.AniIdMother = Nieuwemoeder.AniId;
                                        sibbel.AniStatus = berekenStatus(pUr, sibbel.AniIdFather, Nieuwemoeder.AniId, pBedrijf);
                                        if (lMstb.UpdateANIMAL(pThrId, sibbel))
                                        {
                                            setSECONRACRasdelen(pUr, sibbel, pBedrijf);
                                            setNling(pThrId, pUr, pBedrijf.UBNid, Nieuwemoeder.AniId, birNr);
                                            bool setmutlog = true;
                                            if (FCIRviaModem.FValue.ToLower() == "true")
                                            {
                                                if (pMutation == 1)
                                                {
                                                    saveNewMutation(pUr, 2, pBedrijf, "", "", geb, new MOVEMENT(), sibbel, Nieuwemoeder, vaders, pChangedBy, pSoureceID);
                                                    setmutlog = false;
                                                }
                                                else if (pMutation == 3)
                                                {
                                                    MUTATION repair = lMstb.GetMutationByEventId(geb.EventId);
                                                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                                                    {
                                                        repair.LifenumberMother = Nieuwemoeder.AniAlternateNumber;
                                                    }
                                                    else
                                                    {
                                                        repair.LifenumberMother = Nieuwemoeder.AniLifeNumber;
                                                    }
                                                    if (repair.EventId != 0)
                                                    {
                                                        lMstb.SaveMutation(repair);
                                                    }
                                                    setmutlog = false;
                                                }

                                            }
                                            if (setmutlog)
                                            {
                                                saveNewMutaLog(pUr, 2, pBedrijf, "", "", geb, new MOVEMENT(), sibbel, Nieuwemoeder, vaders, pChangedBy, pSoureceID);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    bool setMutalog = true;
                    if (FCIRviaModem.FValue.ToLower() == "true")
                    {
                        if (pMutation == 1)
                        {
                            saveNewMutation(pUr, 2, pBedrijf, "", "", birtevent, new MOVEMENT(), pAnimal, Nieuwemoeder, vaders, pChangedBy, pSoureceID);
                            setMutalog = false;
                        }
                        else if (pMutation == 3)
                        {
                            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                            {
                                muta.LifenumberMother = Nieuwemoeder.AniAlternateNumber;
                            }
                            else
                            {
                                muta.LifenumberMother = Nieuwemoeder.AniLifeNumber;
                            }
                            if (muta.EventId != 0)
                            {
                                lMstb.SaveMutation(muta);
                            }
                            setMutalog = false;
                        }

                    }
                    if (setMutalog)
                    {
                        saveNewMutaLog(pUr, 2, pBedrijf, "", "", birtevent, new MOVEMENT(), pAnimal, Nieuwemoeder, vaders, pChangedBy, pSoureceID);

                    }
                }

            }
            return antwoord;
        }

        public static string ChangeAniBirthDay(int pThrId, UserRightsToken pUr, DateTime pNewAniBirthDay, ANIMAL pAnimal, ANIMAL pAniMother, BEDRIJF pBedrijf, int pChangeWorp, int pBrothersSisters, int pChangeLNV)
        {
            //pChangeWorp (Worp datum moeder) 1 = aanpassen 2 =niet aanpassen
            //pBrothersSisters (geboortedatums broers en zusters)  1 = aanpassen 2 =niet aanpassen
            //pChangeLNV Melding voor RVO 1=aanpassen 2 = niet van toepassing
            //  bij gekomen   Melding voor RVO 3=aanpassen, 4=niet van toepassing
            string returnStr = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            DateTime lOldBirthDay = pAnimal.AniBirthDate;
            string FCIRviaModem = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "IRviaModem", "true");

            if (pAniMother.AniId > 0)
            {

                List<BIRTH> geboortes = lMstb.GetBirthsByAnimal(pAniMother.AniId);
                var TempGeboortes = from n in geboortes
                                    where n.CalfId == pAnimal.AniId
                                    select n;
                if (TempGeboortes.Count() == 1)
                {
                    BIRTH pAnimalBIRTH = TempGeboortes.ElementAt(0);
                    var BrothrsSisters = from bs in geboortes
                                         where bs.AniFatherID == pAnimalBIRTH.AniFatherID &&
                                         bs.BirNumber == pAnimalBIRTH.BirNumber &&
                                         bs.Nling == pAnimalBIRTH.Nling
                                         select bs;
                    if (BrothrsSisters.Count() > 0 && pBrothersSisters == 1)
                    {

                        foreach (BIRTH b in BrothrsSisters)
                        {
                            if (b.CalfId != pAnimal.AniId)
                            {
                                EVENT ev = lMstb.GetEventdByEventId(b.EventId);
                                ANIMAL dier = lMstb.GetAnimalById(b.CalfId);
                                dier.AniBirthDate = pNewAniBirthDay;
                                lMstb.UpdateANIMAL(pThrId, dier);
                                if (pChangeWorp == 1)
                                {

                                    handleEventTimes(ref ev, pNewAniBirthDay);
                                    lMstb.UpdateEVENT(ev);
                                }
                                if (pChangeLNV == 1 || pChangeLNV == 3)
                                {
                                    MUTATION m = lMstb.GetMutationByEventId(b.EventId);
                                    if (m.EventId > 0)
                                    {
                                        m.IDRBirthDate = pNewAniBirthDay;
                                        m.MutationDate = ev.EveDate;
                                        m.MutationTime = ev.EveDate;
                                        lMstb.SaveMutation(m);
                                    }
                                    else
                                    {
                                        MUTALOG ml = lMstb.GetMutaLogByEventId(b.EventId);
                                        if (ml.EventId > 0)
                                        {
                                            MUTATION t = Facade.GetInstance().getMeldingen().ConverttoMutation(ml);
                                            if (t.CodeMutation < 100)
                                            {
                                                t.CodeMutation = t.CodeMutation + 200;
                                            }
                                            t.IDRBirthDate = pNewAniBirthDay;
                                            t.MutationDate = ev.EveDate;
                                            t.MutationTime = ev.EveDate;
                                            lMstb.SaveMutation(t);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    EVENT eigenGeboorteevent = lMstb.GetEventdByEventId(pAnimalBIRTH.EventId);
                    if (pChangeWorp == 1)
                    {

                        handleEventTimes(ref eigenGeboorteevent, pNewAniBirthDay);
                        eigenGeboorteevent.EveOrder = getNewEventOrder(pUr, pNewAniBirthDay, 5, pBedrijf.UBNid, pAniMother.AniId);

                        lMstb.UpdateEVENT(eigenGeboorteevent);
                        BIRTH br = lMstb.GetBirth(eigenGeboorteevent.EventId);
                        int OldBirnr = br.BirNumber;

                        int birNr = getOrCreateBirnr(pUr, pAniMother.AniId, pNewAniBirthDay);

                        br.BirNumber = birNr;
                        lMstb.SaveBirth(br);
                        setNling(pThrId, pUr, pBedrijf.UBNid, pAniMother.AniId, birNr);
                        setNling(pThrId, pUr, pBedrijf.UBNid, pAniMother.AniId, OldBirnr);


                    }
                    if (pChangeLNV == 1 || pChangeLNV == 3)
                    {

                        MUTATION m = lMstb.GetMutationByEventId(pAnimalBIRTH.EventId);
                        if (m.EventId > 0)
                        {
                            m.IDRBirthDate = pNewAniBirthDay;
                            m.MutationDate = eigenGeboorteevent.EveDate;
                            m.MutationTime = eigenGeboorteevent.EveDate;
                            lMstb.SaveMutation(m);
                        }
                        else
                        {
                            MUTALOG ml = lMstb.GetMutaLogByEventId(eigenGeboorteevent.EventId);
                            MUTATION t = Facade.GetInstance().getMeldingen().ConverttoMutation(ml);
                            t.CodeMutation = t.CodeMutation + 200;
                            t.IDRBirthDate = pNewAniBirthDay;
                            t.MutationDate = eigenGeboorteevent.EveDate;
                            t.MutationTime = eigenGeboorteevent.EveDate;
                            lMstb.SaveMutation(t);

                        }
                    }
                }
            }

            pAnimal.AniBirthDate = pNewAniBirthDay;
            lMstb.UpdateANIMAL(pThrId, pAnimal);
            return returnStr;
        }

        public static void createMotherBirthEvents(UserRightsToken pUr, ANIMAL pAnimal, ANIMAL pMoeder, BEDRIJF pBedrijf, out EVENT birtevent, out BIRTH birt)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);

            birtevent = new EVENT();
            birtevent.AniId = pMoeder.AniId;
            Event_functions.handleEventTimes(ref birtevent, pAnimal.AniBirthDate);
            birtevent.EveKind = 5;

            //birtevent.EveOrder = getNewEventOrder(pUr, pAnimal.AniBirthDate, 5, pBedrijf.UBNid, pMoeder.AniId);
            birtevent.UBNId = pBedrijf.UBNid;
            birtevent.happened_at_FarmID = pBedrijf.FarmId;
            lMstb.SaveEvent(birtevent);

            birt = new BIRTH();
            birt.EventId = birtevent.EventId;
            birt.CalfId = pAnimal.AniId;
            birt.AniFatherID = pAnimal.AniIdFather;
            birt.BirNumber = getOrCreateBirnr(pUr, pMoeder.AniId, pAnimal.AniBirthDate);
            birt.BornDead = 0;
            birt.Nling = 1; //wordt later met setNling alsnoggedaan;
            lMstb.SaveBirth(birt);
        }

        public static string saveWorpen(int pThrId, UserRightsToken pUr, Worp pWorp, BEDRIJF pBedrijf, int pChangedBy)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            string antwoord = "";
            UBN ubu = lMstb.GetubnById(pBedrijf.UBNid);
            THIRD HuidigeThird = lMstb.GetThirdByThirId(ubu.ThrID);
            //COUNTRY UBNLand;
            //int LandId = 0;
            //if (int.TryParse(HuidigeThird.ThrCountry, out LandId))
            //{
            //    UBNLand = lMstb.GetCountryByLandid(LandId);
            //}
            //else
            //{
            //    UBNLand = lMstb.GetCountryByLandid(151);
            //}
            //pWorp.lAnimal.AniCountryCodeBirth = UBNLand.LandAfk2;
            pWorp.lAnimal.ThrId = HuidigeThird.ThrId;
            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");
            FARMCONFIG fcon = lMstb.getFarmConfig(pBedrijf.FarmId, "rfid", "1");
            string worpinfo = "Opslaan worp:" + pBedrijf.Omschrijving + " " + ubu.Bedrijfsnummer + ":";
            int AniIdMoeder = pWorp.lAnimal.AniIdMother;
            DateTime geboortedag = pWorp.lEvent.EveDate.Date;

            int birnummer = 0;
            if (AniIdMoeder > 0)
            {
                birnummer = getOrCreateBirnr(pUr, AniIdMoeder, geboortedag);
            }


            if (pWorp.lBirth.BornDead != 1)
            {
                ANIMAL lExistingCalf = new ANIMAL();
                if (pWorp.lAnimal.AniAlternateNumber.Trim() != "")
                {
                    lExistingCalf = lMstb.GetAnimalByAniAlternateNumber(pWorp.lAnimal.AniAlternateNumber);
                }

                if (lExistingCalf.AniId == 0)
                {
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    { }
                    else
                    {
                        if (pWorp.lAnimal.AniLifeNumber != "")
                        {
                            lExistingCalf = lMstb.GetAnimalByLifenr(pWorp.lAnimal.AniLifeNumber);
                        }
                    }
                }


                if (lExistingCalf.AniId > 0)
                {
                    //antwoord ="Niet opgeslagen:" + pWorp.lAnimal.AniAlternateNumber + " " + pWorp.lAnimal.AniLifeNumber + " Is al bekend in de database.";
                    unLogger.WriteDebug(worpinfo + " Worp opslaan" + antwoord);
                }
                if (antwoord == "")
                {


                    ANIMAL ani = pWorp.lAnimal;
                    ANIMALCATEGORY anicat = pWorp.lAnimalCategory;

                    EVENT ev = pWorp.lEvent;
                    BIRTH br = pWorp.lBirth;

                    ev.AniId = AniIdMoeder;
                    ev.UBNId = pBedrijf.UBNid;
                    ev.EveKind = 5;

                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        antwoord = Checker.IsValidLevensnummer(ani.AniAlternateNumber, true, pBedrijf.ProgId, pBedrijf.Programid);
                        unLogger.WriteInfo(worpinfo + antwoord);
                    }
                    else
                    {
                        antwoord = Checker.IsValidLevensnummer(ani.AniLifeNumber, true, pBedrijf.ProgId, pBedrijf.Programid);
                        unLogger.WriteInfo(worpinfo + antwoord);

                    }

                    if (ani.AniSex == 1)
                    {
                        ani.BullAiNumber = pWorp.lAnimalCategory.AniWorknumber;
                    }

                    if (antwoord == "")
                    {



                        if (ani.AniScrapie > 0)
                        {
                            if (ani.AniDatumScrapie == DateTime.MinValue)
                            {
                                ani.AniDatumScrapie = ani.AniBirthDate;
                            }
                        }

                        bool isSaved = false;

                        if (lExistingCalf.AniId > 0)
                        {

                            if (lExistingCalf.AniIdFather < 1)
                            {
                                lExistingCalf.AniIdFather = ani.AniIdFather;
                            }
                            if (lExistingCalf.AniIdMother < 1)
                            {
                                lExistingCalf.AniIdMother = ani.AniIdMother;
                            }
                            lExistingCalf.AniNling = ani.AniNling;
                            if (lExistingCalf.AniSex == 0)
                            {
                                lExistingCalf.AniSex = ani.AniSex;
                            }
                            lExistingCalf.Changed_By = pChangedBy;
                            lExistingCalf.SourceID = pThrId;
                            if (lMstb.UpdateANIMAL(pThrId, lExistingCalf))
                            {
                                isSaved = true;
                                ani = lExistingCalf;
                                anicat.AniId = ani.AniId;
                            }
                            ANIMALCATEGORY lCat = lMstb.GetAnimalCategoryByIdandFarmid(lExistingCalf.AniId, pBedrijf.FarmId);
                            lCat.Changed_By = pChangedBy;
                            lCat.SourceID = pThrId;
                            if (lCat.FarmId > 0)
                            {
                                anicat = lCat;
                            }
                            BIRTH checkBirth = lMstb.GetBirthByCalfId(lExistingCalf.AniId);
                            if (checkBirth.EventId > 0)
                            {
                                EVENT checkEvent = lMstb.GetEventdByEventId(checkBirth.EventId);

                                ev = checkEvent;
                                if (checkBirth.AniFatherID < 1)
                                {
                                    checkBirth.AniFatherID = br.AniFatherID;
                                }
                                //ivm savebirth idFilledByDb
                                checkBirth.BirNumber = br.BirNumber;
                                checkBirth.BirthCourse = br.BirthCourse;
                                checkBirth.BornDead = br.BornDead;
                                checkBirth.CalfWeight = br.CalfWeight;
                                checkBirth.Nling = br.Nling;
                                checkBirth.Position = br.Position;
                                checkBirth.Changed_By = pChangedBy;
                                checkBirth.SourceID = pThrId;
                                br = checkBirth;
                            }
                        }
                        else
                        {
                            if (ani.AniCountryCodeBirth == "" && ani.ThrId > 0)
                            {
                                THIRD tCountryCodeBirth = lMstb.GetThirdByThirId(ani.ThrId);
                                try
                                {
                                    COUNTRY c = lMstb.GetCountryByLandid(int.Parse(tCountryCodeBirth.ThrCountry));
                                    if (ani.AniAlternateNumber.StartsWith(c.LandAfk2))
                                    {
                                        ani.AniCountryCodeBirth = c.LandAfk2;
                                    }
                                    else
                                    {
                                        ani.AniCountryCodeBirth = ani.AniAlternateNumber.Substring(0, 2);
                                    }
                                }
                                catch { }
                            }
                            ani.Changed_By = pChangedBy;
                            ani.SourceID = pThrId;
                            if (lMstb.SaveAnimal(pThrId, ani))
                            {
                                isSaved = true;
                            }
                        }

                        if (isSaved)
                        {
                            EVENT nChipEvent = new EVENT();
                            if (pBedrijf.ProgId == 25)
                            {
                                if (pWorp.chipperThrId > 0 && pWorp.chipdatum > DateTime.MinValue)
                                {

                                    nChipEvent.AniId = ani.AniId;
                                    nChipEvent.EveDate = pWorp.chipdatum;
                                    Event_functions.handleEventTimes(ref nChipEvent, pWorp.chipdatum);
                                    nChipEvent.EveKind = 15;
                                    nChipEvent.EveOrder = 1;
                                    nChipEvent.happened_at_FarmID = pBedrijf.FarmId;
                                    nChipEvent.EveComment = "";
                                    nChipEvent.ThirdId = pWorp.chipperThrId;
                                    nChipEvent.UBNId = pBedrijf.UBNid;
                                    nChipEvent.Changed_By = pChangedBy;
                                    nChipEvent.SourceID = pThrId;
                                    lMstb.SaveEvent(nChipEvent);
                                }
                            }
                            insertWorpAnimalAfwijkingen(pThrId, lMstb, pBedrijf, ani, pWorp.lAfwijkingen);
                            unLogger.WriteInfo(worpinfo + ani.AniAlternateNumber + " bij werpen opgeslagen");
                            char[] split = { ' ' };
                            string[] numbers = pWorp.lAnimal.AniAlternateNumber.Split(split);
                            if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                            {
                                numbers = pWorp.lAnimal.AniLifeNumber.Split(split);
                            }
                            string nummie = "";
                            for (int i = 1; i < numbers.Length; i++)
                            {
                                nummie += numbers[i] + " ";
                            }
                            //Chipnr 528123456789012
                            if (numbers.Length == 1)
                            {
                                if (numbers[0].Length > 3)
                                {
                                    //lf.LifCountrycode = numbers[0].Substring(0, 3);
                                    nummie = numbers[0].Substring(3, numbers[0].Length - 3);
                                }
                            }
                            LIFENR lf = lMstb.GetLifenrByLifenr(pBedrijf.FarmId, nummie.Trim());
                            lMstb.DeleteLifenr(pBedrijf.FarmId, lf);

                            anicat.Changed_By = pChangedBy;
                            anicat.SourceID = pThrId;
                            anicat.AniId = ani.AniId;
                            MovFunc.setcat123(ref anicat, ani.AniSex, false);
                            anicat.FarmId = pBedrijf.FarmId;
                            if (anicat.AniWorknumber == "")
                            {
                                string Stamboeknr = Event_functions.getStamboekNr(pBedrijf, HuidigeThird);
                                int Hvolgnr = Event_functions.getHoogsteVolgnr(pUr, pBedrijf, Stamboeknr, lMstb.GetAnimalsByFarmId(pBedrijf.FarmId));
                                string worknr = "";
                                string tempnr = "";
                                ANIMAL vader = new ANIMAL();
                                if (ani.AniIdFather > 0)
                                {
                                    vader = lMstb.GetAnimalById(ani.AniIdFather);
                                }
                                Event_functions.getRFIDNumbers(pUr, pBedrijf, fcon.FValue, ani.AniAlternateNumber, vader, Hvolgnr, out worknr, out tempnr);
                                anicat.AniWorknumber = worknr;
                            }
                            if (lMstb.SaveAnimalCategory(anicat))
                            {
                                unLogger.WriteDebug(worpinfo + anicat.AniId.ToString() + " " + ani.AniAlternateNumber + " bij worp van MoederAniId=" + AniIdMoeder.ToString() + " opgeslagen");
                                if (AniIdMoeder > 0)
                                {
                                    if (ev.EveOrder < 1)
                                    {
                                        ev.EveOrder = getNewEventOrder(pUr, ev.EveDate, 5, ev.UBNId, AniIdMoeder);
                                    }
                                    ev.happened_at_FarmID = anicat.FarmId;
                                    if (lMstb.SaveEvent(ev))
                                    {
                                        br.EventId = ev.EventId;
                                        br.BirNumber = birnummer;
                                        br.Nling = pWorp.lAnimal.AniNling;// int.Parse(rw["AniNling"].ToString());

                                        br.CalfId = ani.AniId;

                                        br.AniFatherID = ani.AniIdFather;
                                        lMstb.SaveBirth(br);



                                        Feedadviceer fav = new Feedadviceer();

                                        ThreadStart t = delegate { fav.saveKetoBoxFeedAdvices((UserRightsToken)pUr.Clone(), pWorp.lAnimal.AniIdMother, pWorp.lEvent.EveDate.Date, pBedrijf, pChangedBy, pThrId); };
                                        Thread parathread = new Thread(t);
                                        parathread.Start();



                                        List<SECONRAC> lstRasDeel = berekenRasDeel(pUr, ani.AniIdMother, ani.AniIdFather, pBedrijf.ProgId, pBedrijf.Programid);
                                        foreach (SECONRAC sr in lstRasDeel)
                                        {
                                            sr.AniId = ani.AniId;
                                            sr.Changed_By = pChangedBy;
                                            sr.SourceID = pThrId;
                                            lMstb.SaveSeconRace(sr);
                                        }
                                    }
                                    else
                                    {


                                        antwoord = "Fout bij invoeren EVENT";

                                    }
                                }
                                EXTERIEUR ext = new EXTERIEUR();
                                ext.AniId = ani.AniId;
                                ext.ExtDatum = geboortedag.Date;
                                ext.ext_Soort = (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.EXTERIEUR_N_Boven;
                                string staartlengte = pWorp.staartlengte;// rw["staartlengte"].ToString();
                                if (staartlengte.Trim() != "")
                                {
                                    double controlestaart = 0;
                                    double.TryParse(staartlengte.Trim(), out controlestaart);
                                    if (controlestaart > 0)
                                    {
                                        if (pBedrijf.ProgId == 3)
                                        {
                                            ext.ExtId = lMstb.saveExterieur(ext);

                                            EXTERIEUR_WAARDEN extw = new EXTERIEUR_WAARDEN();
                                            extw.ExtwType = 20;
                                            extw.ExtwWaarde = staartlengte;
                                            extw.ExtId = ext.ExtId;
                                            extw.Changed_By = pChangedBy;
                                            extw.SourceID = pThrId;
                                            lMstb.saveExterieurWaarden(extw);
                                        }
                                    }
                                }

                                if (br.CalfWeight > 0)
                                {
                                    WEIGHT w = lMstb.getWeight(ani.AniId, ani.AniBirthDate.Date);
                                    if (w.AniId < 1)
                                    {
                                        w = new WEIGHT();
                                        w.AniId = ani.AniId;
                                        w.WeightDate = ani.AniBirthDate.Date;
                                        w.WeightOrder = 1;
                                    }

                                    w.WeightKg = br.CalfWeight;
                                    if (w.WeightKg > 0)
                                    {
                                        w.Changed_By = pChangedBy;
                                        w.SourceID = pThrId;
                                        lMstb.SaveWeight(w);
                                    }
                                    //WEIGHT w = new WEIGHT();
                                    //w.AniId = ani.AniId;
                                    //w.WeightDate = geboortedag.Date;
                                    //w.WeightKg = br.CalfWeight;
                                    //w.WeightOrder = 1;
                                    //lMstb.SaveWeight(w);
                                    //if (pBedrijf.ProgId == 3)
                                    //{
                                    //    if (ext.ExtId == 0)
                                    //    {
                                    //        ext.ExtId = lMstb.saveExterieur(ext);
                                    //    }
                                    //    EXTERIEUR_WAARDEN extw = new EXTERIEUR_WAARDEN();
                                    //    extw.ExtwType = 15;
                                    //    extw.ExtwWaarde = br.CalfWeight.ToString();
                                    //    extw.ExtId = ext.ExtId;

                                    //    lMstb.saveExterieurWaarden(extw);
                                    //}
                                }
                            }
                            else
                            {


                                antwoord = "Fout bij invoeren dier category";

                            }

                            if (FCIRviaModem.FValue.ToLower() == "true")
                            {
                                bool algemeld = false;
                                MUTATION mut = pWorp.lMutation;
                                MUTATION ExistingMut = lMstb.GetMutationByEventId(ev.EventId);
                                if (ExistingMut.EventId > 0)
                                {
                                    mut = ExistingMut;
                                }
                                else
                                {
                                    MUTALOG ExistingMutlog = lMstb.GetMutaLogByEventId(ev.EventId);
                                    if (ExistingMutlog.EventId > 0)
                                    {
                                        algemeld = true;
                                    }
                                }


                                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                                {
                                    mut.AlternateLifeNumber = "";// ani.AniLifeNumber;
                                    mut.CodeMutation = 2;
                                    mut.EventId = ev.EventId;
                                    mut.Farmnumber = ubu.Bedrijfsnummer;
                                    mut.IDRBirthDate = ani.AniBirthDate;
                                    mut.Lifenumber = ani.AniLifeNumber;
                                    mut.CountryCodeBirth = ani.AniCountryCodeBirth;
                                    mut.Haircolor = ani.Anihaircolor;
                                    mut.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                                    mut.Worknumber = anicat.AniWorknumber;
                                    mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                                    if (AniIdMoeder > 0)
                                    {
                                        ANIMAL moeder = lMstb.GetAnimalById(AniIdMoeder);
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
                                    mut.Sex = ani.AniSex;
                                    mut.UbnId = pBedrijf.UBNid;
                                    mut.Weight = br.CalfWeight;
                                    //mut.Worknumber = "";
                                }
                                else
                                {
                                    mut.AlternateLifeNumber = "";//ani.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                                    mut.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());
                                    mut.EventId = ev.EventId;
                                    mut.Farmnumber = ubu.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                                    mut.FarmNumberTo = "";
                                    mut.FarmNumberFrom = "";
                                    mut.Haircolor = ani.Anihaircolor;
                                    mut.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                                    mut.Worknumber = anicat.AniWorknumber;
                                    mut.IDRBirthDate = ani.AniBirthDate;
                                    mut.Lifenumber = getGoodAlternateNumber(ani.AniAlternateNumber);
                                    mut.CountryCodeBirth = ani.AniCountryCodeBirth;
                                    mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                                    if (AniIdMoeder > 0)
                                    {
                                        ANIMAL moeder = lMstb.GetAnimalById(AniIdMoeder);

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
                                    mut.Weight = br.CalfWeight;// double.Parse(rw["CalfWeight"].ToString());
                                    // rw["AniWorknumber"].ToString();
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
                                    if (IenRCom.ValueAsInteger() > 0)
                                    {
                                        mut.SendTo = IenRCom.ValueAsInteger();
                                    }
                                    else
                                    {
                                        mut.SendTo = int.Parse(defIenRaction);
                                    }
                                }

                                if (!algemeld)
                                {
                                    mut.Changed_By = pChangedBy;
                                    mut.SourceID = pThrId;
                                    lMstb.SaveMutation(mut);
                                }
                            }
                            else
                            {
                                MUTALOG mutlog = new MUTALOG();
                                MUTALOG ExistingMutlog = lMstb.GetMutaLogByEventId(ev.EventId);

                                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                                {
                                    mutlog.AlternateLifeNumber = ani.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                                    mutlog.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());
                                    mutlog.EventId = ev.EventId;
                                    mutlog.Farmnumber = ubu.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                                    mutlog.FarmNumberTo = "";
                                    mutlog.FarmNumberFrom = "";
                                    mutlog.Haircolor = ani.Anihaircolor;
                                    mutlog.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                                    mutlog.Worknumber = anicat.AniWorknumber;
                                    mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                                    mutlog.IDRBirthDate = ani.AniBirthDate;
                                    mutlog.CountryCodeBirth = ani.AniCountryCodeBirth;
                                    mutlog.Lifenumber = ani.AniLifeNumber;
                                    if (AniIdMoeder > 0)
                                    {
                                        ANIMAL moeder = lMstb.GetAnimalById(AniIdMoeder);
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
                                    mutlog.Weight = br.CalfWeight;// double.Parse(rw["CalfWeight"].ToString());
                                    //mutlog.Worknumber = "";// rw["AniWorknumber"].ToString();
                                }
                                else
                                {
                                    mutlog.AlternateLifeNumber = ani.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                                    mutlog.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());
                                    mutlog.EventId = ev.EventId;
                                    mutlog.Farmnumber = ubu.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                                    mutlog.FarmNumberTo = "";
                                    mutlog.FarmNumberFrom = "";
                                    mutlog.Haircolor = ani.Anihaircolor;
                                    mutlog.AniHaircolor_Memo = ani.AniHaircolor_Memo;
                                    mutlog.Worknumber = anicat.AniWorknumber;
                                    mutlog.IDRBirthDate = geboortedag;
                                    mutlog.CountryCodeBirth = ani.AniCountryCodeBirth;
                                    mutlog.Lifenumber = getGoodAlternateNumber(ani.AniAlternateNumber);
                                    mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, ani.AniAlternateNumber);
                                    if (AniIdMoeder > 0)
                                    {
                                        ANIMAL moeder = lMstb.GetAnimalById(AniIdMoeder);

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
                                    mutlog.Weight = br.CalfWeight;// double.Parse(rw["CalfWeight"].ToString());
                                    //mutlog.Worknumber = "";// rw["AniWorknumber"].ToString();
                                }
                                if (ExistingMutlog.EventId < 1)
                                {
                                    mutlog.Changed_By = pChangedBy;
                                    mutlog.SourceID = pThrId;
                                    lMstb.InsertMutLog(mutlog);
                                }
                            }
                        }
                        else
                        {
                            unLogger.WriteInfo(worpinfo + ani.AniAlternateNumber + " bij saveanimal NIET opgeslagen");
                        }
                    }

                }
            }
            else
            {
                //doodgeboren
                if (AniIdMoeder > 0)
                {
                    EVENT evdead = pWorp.lEvent;

                    BIRTH brdead = pWorp.lBirth;

                    evdead.AniId = AniIdMoeder;

                    evdead.EveKind = 5;
                    evdead.EveOrder = getNewEventOrder(pUr, evdead.EveDate, 5, evdead.UBNId, AniIdMoeder);

                    evdead.UBNId = pBedrijf.UBNid;// int.Parse(rw["UbnId"].ToString());
                    evdead.happened_at_FarmID = pBedrijf.FarmId;


                    brdead.AniFatherID = pWorp.lAnimal.AniIdFather;
                    evdead.Changed_By = pChangedBy;
                    evdead.SourceID = pThrId;
                    if (lMstb.SaveEvent(evdead))
                    {
                        brdead.EventId = evdead.EventId;
                        brdead.BirNumber = birnummer;

                        brdead.AniFatherID = pWorp.lAnimal.AniIdFather;
                        brdead.Changed_By = pChangedBy;
                        brdead.SourceID = pThrId;
                        lMstb.SaveBirth(brdead);
                        Feedadviceer fav = new Feedadviceer();
                        ThreadStart t = delegate { fav.saveKetoBoxFeedAdvices((UserRightsToken)pUr.Clone(), pWorp.lAnimal.AniIdMother, pWorp.lEvent.EveDate.Date, pBedrijf, pChangedBy, pThrId); };
                        Thread parathread = new Thread(t);
                        parathread.Start();
                    }
                    else
                    {

                        lMstb.WriteLogMessage(pBedrijf.UBNid, 0, "invoeren EVENT bij aflammen dood kalf");
                        antwoord = "Fout bij invoeren EVENT";

                    }
                }
            }

            //alle lammeren lekker opgeslagen 
            //maar nu het Nling nr veranderen aan de hand van het BirNr
            if (antwoord == "")
            {
                if (AniIdMoeder > 0)
                {
                    setNling(pThrId, pUr, pBedrijf.UBNid, AniIdMoeder, birnummer);
                }
            }
            return antwoord;
        }

        public static string saveWorpZonderMoeder(int LogedInThrId, UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThird, COUNTRY pCountry, ANIMAL pAnimal, ANIMALCATEGORY pAnimalcategory, List<ANIMAL_AFWIJKING> pAfwijkingen, double pGewicht, double pStaartlengte, int pChipperThrId, DateTime pChipdatum)
        {
            string ret = "";
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMAL check = new ANIMAL();
            if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
            {
                check = lMstb.GetAnimalByAniAlternateNumber(pAnimal.AniAlternateNumber);
            }
            else
            {
                check = lMstb.GetAnimalByAniAlternateNumber(pAnimal.AniAlternateNumber);
            }

            if (pAnimal.ThrId != pUbn.ThrID)
            {
                pAnimal.ThrId = pUbn.ThrID;
            }
            if (check.AniId <= 0)
            {
                lMstb.SaveAnimal(LogedInThrId, pAnimal);

                if (pAnimalcategory.Anicategory > 3)
                {
                    MovFunc.setcat123(ref pAnimalcategory, pAnimal.AniSex, false);
                }
                pAnimalcategory.AniId = pAnimal.AniId;
                pAnimalcategory.FarmId = pBedrijf.FarmId;
                if (pAnimalcategory.AniWorknumber == "")
                {

                    string Stamboeknr = Event_functions.getStamboekNr(pBedrijf, pThird);
                    int Hvolgnr = Event_functions.getHoogsteVolgnr(pUr, pBedrijf, Stamboeknr, lMstb.GetAnimalsByFarmId(pBedrijf.FarmId));
                    string worknr = "";
                    string tempnr = "";
                    ANIMAL vader = new ANIMAL();
                    if (pAnimal.AniIdFather > 0)
                    {
                        vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                    }
                    FARMCONFIG fcon = lMstb.getFarmConfig(pBedrijf.FarmId, "rfid", "1");
                    Event_functions.getRFIDNumbers(pUr, pBedrijf, fcon.FValue, pAnimal.AniAlternateNumber, vader, Hvolgnr, out worknr, out tempnr);
                    pAnimalcategory.AniWorknumber = worknr;

                }
                lMstb.SaveAnimalCategory(pAnimalcategory);
                if (pBedrijf.ProgId == 25)
                {
                    if (pChipperThrId > 0 && pChipdatum > DateTime.MinValue)
                    {
                        EVENT n = new EVENT();
                        n.AniId = pAnimal.AniId;
                        n.EveDate = pChipdatum;
                        Event_functions.handleEventTimes(ref n, pChipdatum);
                        n.EveKind = 15;
                        n.EveOrder = 1;
                        n.happened_at_FarmID = pBedrijf.FarmId;
                        n.EveComment = "";
                        n.ThirdId = pChipperThrId;
                        n.UBNId = pBedrijf.UBNid;
                        lMstb.SaveEvent(n);
                    }
                }
                if (pGewicht > 0)
                {
                    WEIGHT w = lMstb.getWeight(pAnimal.AniId, pAnimal.AniBirthDate.Date);
                    if (w.AniId < 1)
                    {
                        w = new WEIGHT();
                        w.AniId = pAnimal.AniId;
                        w.WeightDate = pAnimal.AniBirthDate.Date;
                        w.WeightOrder = 1;
                    }
                    w.WeightKg = pGewicht;
                    lMstb.SaveWeight(w);

                }
                if (pStaartlengte > 0)
                {
                    EXTERIEUR ext = new EXTERIEUR();
                    ext.AniId = pAnimal.AniId;
                    ext.ExtDatum = pAnimal.AniBirthDate.Date;
                    string staartlengte = pStaartlengte.ToString();
                    if (staartlengte.Trim() != "")
                    {
                        if (pBedrijf.ProgId == 3)
                        {
                            ext.ExtId = lMstb.saveExterieur(ext);
                            if (ext.ExtId > 0)
                            {
                                EXTERIEUR_WAARDEN extw = new EXTERIEUR_WAARDEN();
                                extw.ExtwType = 20;
                                extw.ExtwWaarde = staartlengte;
                                extw.ExtId = ext.ExtId;

                                lMstb.saveExterieurWaarden(extw);
                            }
                        }
                    }
                }
                if (pAfwijkingen.Count() > 0)
                {
                    insertWorpAnimalAfwijkingen(LogedInThrId, lMstb, pBedrijf, pAnimal, pAfwijkingen);
                }
                #region LIFENR verwijderen
                char[] split = { ' ' };
                string[] numbers = pAnimal.AniAlternateNumber.Split(split);
                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                {
                    numbers = pAnimal.AniLifeNumber.Split(split);
                }

                string nummie = "";
                for (int i = 1; i < numbers.Length; i++)
                {
                    nummie += numbers[i] + " ";
                }
                if (numbers.Length == 1)
                {
                    if (numbers[0].Length > 3)
                    {
                        //lf.LifCountrycode = numbers[0].Substring(0, 3);
                        nummie = numbers[0].Substring(3, numbers[0].Length - 3);
                    }
                }

                if (nummie.Trim().Length > 0)
                {
                    LIFENR lf = lMstb.GetLifenrByLifenr(pBedrijf.FarmId, nummie.Trim());
                    if (lf.LifLifenr.Length > 0)
                    {
                        lMstb.DeleteLifenr(pBedrijf.FarmId, lf);
                    }
                }
                #endregion
                saveNoMotherBirthMutation(LogedInThrId, pUr, pBedrijf, pUbn, pThird, pCountry, pAnimal, pAnimalcategory, pGewicht);
            }
            else
            {
                ret = "Dit levensnummer is al voorkomend bij een nog I&R te melden dier.";
            }
            return ret;
        }

        private static void saveNoMotherBirthMutation(int LogedInThrId, UserRightsToken pUr, BEDRIJF pBedrijf, UBN pUbn, THIRD pThird, COUNTRY pCountry, ANIMAL pAnimal, ANIMALCATEGORY pAnimalcategory, double pGewicht)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");
            if (FCIRviaModem.FValue.ToLower() == "true")
            {
                bool algemeld = false;
                MUTATION mut = new MUTATION();


                List<MUTATION> ExistingMuts = lMstb.GetMutationsByLifeNumber(pAnimal.AniAlternateNumber);
                var testmut = from n in ExistingMuts
                              where n.Lifenumber == pAnimal.AniLifeNumber
                              && n.MutationDate.Date == pAnimal.AniBirthDate.Date
                              && n.CodeMutation == 2
                              select n;
                if (testmut.Count() > 0)
                {
                    mut = ExistingMuts.ElementAt(0);
                }
                else
                {
                    List<MUTALOG> ExistingMutalogs = lMstb.GetMutaLogsByLifeNumber(pAnimal.AniAlternateNumber);
                    var testmutlogs = from n in ExistingMutalogs
                                      where n.Lifenumber == pAnimal.AniLifeNumber
                                      && n.MutationDate.Date == pAnimal.AniBirthDate.Date
                                      && n.CodeMutation == 2 && (n.Returnresult == 1 || n.Returnresult == 3)
                                      select n;
                    if (testmutlogs.Count() > 0)
                    { algemeld = true; }
                }

                mut.CodeMutation = 2;
                mut.Farmnumber = pUbn.Bedrijfsnummer;
                mut.IDRBirthDate = pAnimal.AniBirthDate;
                mut.Lifenumber = pAnimal.AniAlternateNumber;
                if (pAnimal.AniCountryCodeBirth != "")
                {
                    mut.CountryCodeBirth = pAnimal.AniCountryCodeBirth;
                }
                else
                {
                    if (pBedrijf.ProgId != 25)
                    {
                        mut.CountryCodeBirth = pAnimal.AniAlternateNumber.Substring(0, 2);
                    }
                }
                mut.Haircolor = pAnimal.Anihaircolor;
                mut.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
                mut.Worknumber = pAnimalcategory.AniWorknumber;
                mut.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, pAnimal.AniAlternateNumber);
                if (pAnimal.AniIdMother > 0)
                {
                    ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                    mut.LifenumberMother = moeder.AniLifeNumber;
                }
                if (pAnimal.AniIdFather > 0)
                {
                    ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                    mut.LifeNumberFather = vader.AniLifeNumber;
                }
                mut.MutationDate = pAnimal.AniBirthDate;
                mut.MutationTime = pAnimal.AniBirthDate;
                mut.FarmNumberTo = "";
                mut.FarmNumberFrom = "";
                mut.Sex = pAnimal.AniSex;
                mut.UbnId = pBedrijf.UBNid;
                mut.Weight = pGewicht;
                //mut.Worknumber = "";


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

                if (!algemeld)
                {
                    lMstb.SaveMutation(mut);
                }
            }
            else
            {
                MUTALOG mutlog = new MUTALOG();
                MUTALOG ExistingMutlog = new MUTALOG();
                List<MUTALOG> ExistingMutlogs = lMstb.GetMutaLogsByUbn(pUbn.UBNid);
                if (ExistingMutlogs.Count() > 0)
                {
                    if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                    {
                        var test = from n in ExistingMutlogs
                                   where n.Lifenumber == pAnimal.AniLifeNumber
                                   && n.MutationDate.Date == pAnimal.AniBirthDate.Date
                                   && n.CodeMutation == 2
                                   select n;
                        if (test.Count() > 0)
                        {
                            ExistingMutlog = ExistingMutlogs.ElementAt(0);
                        }
                    }
                    else
                    {
                        var test = from n in ExistingMutlogs
                                   where n.Lifenumber == getGoodAlternateNumber(pAnimal.AniAlternateNumber)
                                   && n.MutationDate.Date == pAnimal.AniBirthDate.Date
                                   && n.CodeMutation == 2
                                   select n;
                        if (test.Count() > 0)
                        {
                            ExistingMutlog = ExistingMutlogs.ElementAt(0);
                        }
                    }
                }


                if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                {
                    mutlog.AlternateLifeNumber = pAnimal.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                    mutlog.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());

                    mutlog.Farmnumber = pUbn.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                    mutlog.FarmNumberTo = "";
                    mutlog.FarmNumberFrom = "";
                    mutlog.Haircolor = pAnimal.Anihaircolor;
                    mutlog.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
                    mutlog.Worknumber = pAnimalcategory.AniWorknumber;
                    mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, pAnimal.AniAlternateNumber);
                    mutlog.IDRBirthDate = pAnimal.AniBirthDate;
                    mutlog.CountryCodeBirth = pAnimal.AniCountryCodeBirth;
                    mutlog.Lifenumber = pAnimal.AniLifeNumber;
                    if (pAnimal.AniIdMother > 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                        mutlog.LifenumberMother = moeder.AniLifeNumber;
                    }
                    if (pAnimal.AniIdFather > 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                        mutlog.LifeNumberFather = vader.AniLifeNumber;
                    }
                    mutlog.MutationDate = pAnimal.AniBirthDate;// (DateTime)rw["EveDate"];
                    mutlog.MutationTime = pAnimal.AniBirthDate;// (DateTime)rw["EveDate"];

                    mutlog.Sex = pAnimal.AniSex;// int.Parse(rw["Sex"].ToString());
                    mutlog.UbnId = pBedrijf.UBNid;// int.Parse(rw["UbnId"].ToString());
                    mutlog.Weight = pGewicht;// double.Parse(rw["CalfWeight"].ToString());
                    //mutlog.Worknumber = "";// rw["AniWorknumber"].ToString();
                }
                else
                {
                    mutlog.AlternateLifeNumber = pAnimal.AniAlternateNumber;// rw["AlternateLifeNumber"].ToString();
                    mutlog.CodeMutation = 2;// int.Parse(rw["CodeMutation"].ToString());

                    mutlog.Farmnumber = pUbn.Bedrijfsnummer;// rw["Farmnumber"].ToString();
                    mutlog.FarmNumberTo = "";
                    mutlog.FarmNumberFrom = "";
                    mutlog.Haircolor = pAnimal.Anihaircolor;
                    mutlog.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
                    mutlog.Worknumber = pAnimalcategory.AniWorknumber;
                    mutlog.IDRBirthDate = pAnimal.AniBirthDate;
                    mutlog.CountryCodeBirth = pAnimal.AniCountryCodeBirth;
                    mutlog.Lifenumber = getGoodAlternateNumber(pAnimal.AniAlternateNumber);
                    mutlog.IDRRace = lMstb.BepaalIDRRascode(pBedrijf, pAnimal.AniAlternateNumber);
                    if (pAnimal.AniIdMother > 0)
                    {
                        ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);

                        mutlog.LifenumberMother = getGoodAlternateNumber(moeder.AniAlternateNumber);
                    }
                    if (pAnimal.AniIdFather > 0)
                    {
                        ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);

                        mutlog.LifeNumberFather = getGoodAlternateNumber(vader.AniAlternateNumber);
                    }
                    mutlog.MutationDate = pAnimal.AniBirthDate;// (DateTime)rw["EveDate"];
                    mutlog.MutationTime = pAnimal.AniBirthDate;// (DateTime)rw["EveDate"];

                    mutlog.Sex = pAnimal.AniSex;// int.Parse(rw["Sex"].ToString());
                    mutlog.UbnId = pBedrijf.UBNid;// int.Parse(rw["UbnId"].ToString());
                    mutlog.Weight = pGewicht;// double.Parse(rw["CalfWeight"].ToString());
                    //mutlog.Worknumber = "";// rw["AniWorknumber"].ToString();
                }
                if (ExistingMutlog.EventId < 1)
                {
                    lMstb.InsertMutLog(mutlog);
                }
            }

        }

        public static void updateBirthOnlyDEPRECATEDWeights(AFSavetoDB lMstb, BEDRIJF pBedrijf, ANIMAL pAnimal, double pNewWeight)
        {

            BIRTH b = lMstb.GetBirthByCalfId(pAnimal.AniId);
            if (b.EventId > 0)
            {
                EVENT lEvent = lMstb.GetEventdByEventId(b.EventId);
                if (lEvent.EveDate.Date == pAnimal.AniBirthDate.Date)
                {
                    b.CalfWeight = pNewWeight;
                    lMstb.SaveBirth(b);
                }
            }
            WEIGHT w = lMstb.getWeight(pAnimal.AniId, pAnimal.AniBirthDate.Date);
            if (w.AniId < 1)
            {
                w = new WEIGHT();
                w.AniId = pAnimal.AniId;
                w.WeightDate = pAnimal.AniBirthDate.Date;
                w.WeightOrder = 1;
            }

            w.WeightKg = pNewWeight;
            if (w.WeightKg > 0)
            {
                lMstb.SaveWeight(w);
            }
            else
            {
                if (w.AniId > 0)
                {
                    lMstb.DeleteWeight(w);
                }
            }
            if (pBedrijf.ProgId == 234325)
            {
                List<AGRO_LABELS> labs = lMstb.GetAgroLabels(VSM.RUMA.CORE.DB.LABELSConst.labKind.EXTERIEUR_N_Boven, 0, pBedrijf.Programid, pBedrijf.ProgId);
                var gewichtlbl = from n in labs
                                 where n.LabID == 15
                                 select n;
                if (gewichtlbl.Count() > 0)
                {
                    if (pNewWeight > 0)
                    {
                        List<EXTERIEUR> exts = lMstb.getExterieurByAnimal(pAnimal.AniId);
                        var temp = from n in exts
                                   where n.ExtDatum.Date == pAnimal.AniBirthDate.Date
                                   && n.ext_Soort == 67
                                   select n;
                        exts = temp.ToList();
                        EXTERIEUR ext = new EXTERIEUR();
                        if (exts.Count() == 1)
                        {
                            ext = exts.ElementAt(0);
                        }
                        else
                        {
                            ext.AniId = pAnimal.AniId;
                            ext.ExtDatum = pAnimal.AniBirthDate.Date;
                            ext.ext_Soort = 67; //exterieur-boven. OUD: BVKindOFValue 5
                            ext.ExtId = lMstb.saveExterieur(ext);
                            ext = lMstb.getExterieur(ext.ExtId);
                        }

                        if (ext.ExtId > 0)
                        {
                            EXTERIEUR_WAARDEN extw = lMstb.getExterieurWaarde(ext.ExtId, 15);
                            if (pNewWeight > 0)
                            {
                                if (extw == null)
                                {
                                    extw = new EXTERIEUR_WAARDEN();
                                    extw.ExtwType = 15;
                                    // String.Format("{0:0}", pNewWeight);//TODO navragen of het zo {0:0.00} moet
                                    extw.ExtId = ext.ExtId;
                                }
                                extw.ExtwWaarde = pNewWeight.ToString();

                                lMstb.saveExterieurWaarden(extw);
                            }
                            else
                            {
                                if (extw != null)
                                {
                                    if (extw.ExtId > 0)
                                    {
                                        lMstb.deleteExterieurWaarden(extw.ExtId, 15);
                                        List<EXTERIEUR_WAARDEN> chekwaarden = lMstb.getExterieurWaardes(extw.ExtId);
                                        if (chekwaarden.Count() == 0)
                                        {
                                            lMstb.deleteExterieur(extw.ExtId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        public static void insertWorpAnimalAfwijkingen(int pThrId, AFSavetoDB lMstb, BEDRIJF pBedrijf, ANIMAL pAnimal, List<ANIMAL_AFWIJKING> pAnimalAfwijkingen)
        {
            if (pAnimalAfwijkingen.Count() > 0)
            {
                if (pAnimal.AniId > 0)
                {
                    List<ANIMAL_AFWIJKING> checkAfwijkingenAgain = lMstb.GetAnimalAfwijkingen(pAnimal.AniId, 0);
                    if (checkAfwijkingenAgain.Count() == 0)
                    {
                        foreach (ANIMAL_AFWIJKING anAfwijking in pAnimalAfwijkingen)
                        {
                            anAfwijking.AniId = pAnimal.AniId;

                            lMstb.InsertAnimalAfwijking(pThrId, anAfwijking);

                            if (anAfwijking.AfwijkingID == 1)
                            {
                                pAnimal.AniStatus = 4;
                                lMstb.UpdateANIMAL(pThrId, pAnimal);
                            }
                        }
                    }
                }
            }
        }

        public static string getGoodAlternateNumber(string AniAlternateNumber)
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

        public static int getNewEventOrderThread(AFSavetoDB lMstb, List<EVENT> events, DateTime pEventdate, int pEventkind, int pUbnId, int pAnimalId)
        {
            int returnOrder = 1;
            if (events.Count > 0)
            {
                int temp = 0;
                foreach (EVENT ev in events)
                {
                    if (ev.EveDate.CompareTo(pEventdate) == 0)
                    {
                        if (temp < ev.EveOrder)
                        { temp = ev.EveOrder; }
                    }
                }
                returnOrder = temp + 1;
            }
            return returnOrder;
        }

        public static int getNewEventOrder(DBConnectionToken pUr, DateTime pEventdate, int pEventkind, int pUbnId, int pAnimalId)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<EVENT> events = lMstb.getEventsByDateAniIdKind(pEventdate.Date, pAnimalId, pEventkind);
            var ordered = (from n in events orderby n.EveOrder select n).ToList();
            int returnOrder = 1;
            if (events.Count > 0)
            {
                foreach (EVENT ev in ordered)
                {
                    if (ev.EveOrder != returnOrder)
                    {
                        //ev.EveOrder = returnOrder;
                        //if (lMstb.SaveEvent(ev))
                        //{
                        //    unLogger.WriteError("Eventorder niet geUpdate bij getNewEventOrder pAnimalId:" + pAnimalId.ToString() + " evekind" + pEventkind.ToString() + " date:" + pEventdate.ToString("dd-MM-yyyy")); 
                        //}
                    }
                    returnOrder += 1;
                }
            }
            return returnOrder;
        }

        public static int insertziekteThread(AFSavetoDB mstb, int pFarmId, int pAniId, DateTime ziektedatum, int disId, int subdisId, int newgeval, int pChangedBy, int pSourceID)
        {
            //AFSavetoDB mstb = (AFSavetoDB)lMstb;
            //mstb.SaveInlog("", "", pUr);
            int EveKind = 7;
            BEDRIJF bedr = mstb.GetBedrijfById(pFarmId);

            EVENT ev = new EVENT();// mstb.GetEventByDateAniIdKind(ziektedatum.Date, pAniId, 7, bedr.UBNid);

            DISEASE dis = new DISEASE();
            if (ev.EventId != 0)
            {
                dis = mstb.GetDisease(pAniId, ev.EventId);
            }

            ev.EveDate = ziektedatum.Date;
            ev.AniId = pAniId;
            ev.EveKind = EveKind;
            ev.ThirdId = pSourceID;
            ev.Changed_By = pChangedBy;
            ev.SourceID = pSourceID;
            ev.UBNId = bedr.UBNid;
            ev.EveMutationDate = DateTime.Now;
            ev.EveMutationTime = DateTime.Now;

            dis.AniId = pAniId;

            dis.NewCase = Convert.ToSByte(newgeval);
            dis.DisMainCode = disId;
            dis.Changed_By = pChangedBy;
            dis.SourceID = pSourceID;
            dis.DisSubCode = subdisId;
            ev.happened_at_FarmID = bedr.FarmId;
            if (ev.EventId == 0)
            {
                List<EVENT> events = mstb.getEventsByAniIdKindUbn(ev.AniId, ev.EveKind, bedr.UBNid);

                ev.EveOrder = getNewEventOrderThread(mstb, events, ev.EveDate, ev.EveKind, bedr.UBNid, ev.AniId);
            }
            if (mstb.SaveEvent(ev))
            {
                dis.EventId = ev.EventId;
                mstb.SaveDisease(dis);
            }
            return ev.EventId;
        }

        public static int insertziekte(UserRightsToken pUr, int pFarmId, int pAniId, DateTime ziektedatum, int disId, int subdisId, int newgeval, int geziendoor, int pChangedBy, int pSourceID)
        {
            return insertziekte(pUr, pFarmId, pAniId, ziektedatum, disId, subdisId, newgeval, geziendoor, 0, pChangedBy, pSourceID);
        }

        public static int insertziekte(UserRightsToken pUr, int pFarmId, int pAniId, DateTime ziektedatum, int disId, int subdisId, int newgeval, int geziendoor, int eveMutationBy, int pChangedBy, int pSourceID)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);

            int EveKind = 7;
            BEDRIJF bedr = lMstb.GetBedrijfById(pFarmId);

            EVENT ev = new EVENT();// mstb.GetEventByDateAniIdKind(ziektedatum.Date, pAniId, 7, bedr.UBNid);

            DISEASE dis = new DISEASE();
            if (ev.EventId != 0)
            {
                dis = lMstb.GetDisease(pAniId, ev.EventId);
            }

            ev.EveDate = ziektedatum.Date;
            ev.AniId = pAniId;
            ev.EveKind = EveKind;
            ev.ThirdId = geziendoor;
            ev.UBNId = bedr.UBNid;
            ev.EveMutationDate = DateTime.Now;
            ev.EveMutationTime = DateTime.Now;
            ev.EveMutationBy = eveMutationBy;
            ev.Changed_By = pChangedBy;
            ev.SourceID = pSourceID;
            dis.AniId = pAniId;

            dis.NewCase = Convert.ToSByte(newgeval);
            dis.DisMainCode = disId;

            dis.DisSubCode = subdisId;
            ev.happened_at_FarmID = bedr.FarmId;
            if (ev.EventId == 0)
            {
                ev.EveOrder = getNewEventOrder(pUr, ev.EveDate, ev.EveKind, bedr.UBNid, ev.AniId);
            }
            if (lMstb.SaveEvent(ev))
            {
                dis.EventId = ev.EventId;
                lMstb.SaveDisease(dis);
            }
            return ev.EventId;
        }

        public static int insertLossebehandelplanThread(UserRightsToken pUr, AFSavetoDB mstb, int pFarmId, int pAnimalId,
                                                        DateTime pbehdatum, MEDPLAND pMedpland, int pBehandeling,
                                                        int UitvoerendeThirdId, int pTreGroupTreat, int pTreMedPlanUniqueNr, int pTreDiseaseId, int pChangedBy, out string pMelden)
        {


            BEDRIJF bedr = new BEDRIJF();
            UBN ubn = new UBN();
            THIRD third = new THIRD();
            COUNTRY country = new COUNTRY();
            mstb.getCompanyByFarmId(pFarmId, out bedr, out ubn, out third, out country);


            pMelden = "";

            int aantalbehandelingen = pMedpland.ToedieningEind;
            EVENT evbehandeling = new EVENT();
            int TreFirstApplicationId = 0;
            for (int i = 0; i < aantalbehandelingen; i++)
            {
                if (UseOldMedicine)
                {
                    MEDICINE MedplDmedicijn = mstb.GetMedicineByMedId(pMedpland.MedId);
                    ANIMAL ani = mstb.GetAnimalById(pAnimalId);

                    List<MEDSTOCK> lMedsocks = mstb.getMedstocksByUbnId(bedr.UBNid);
                    evbehandeling = new EVENT();
                    TREATMEN trm = new TREATMEN();
                    MEDSTOCK m = new MEDSTOCK();

                    if (pMedpland.ToedieningStart > 0)
                    {
                        pbehdatum = pbehdatum.Date.AddDays(pMedpland.ToedieningStart - 1);
                    }
                    DateTime tijd = DateTime.Now;
                    evbehandeling.EveDate = pbehdatum.Date.AddDays((i * pMedpland.MedHoursRepeat) / 24);


                    evbehandeling.AniId = pAnimalId;
                    evbehandeling.EveKind = 6;
                    evbehandeling.UBNId = bedr.UBNid;
                    evbehandeling.EveMutationDate = tijd;
                    evbehandeling.EveMutationTime = tijd;
                    evbehandeling.Changed_By = pChangedBy;
                    evbehandeling.SourceID = UitvoerendeThirdId;
                    evbehandeling.EveOrder = i + 1;
                    //if (evbehandeling.EventId == 0)
                    //{
                    //    if (pTreMedPlanUniqueNr == 0)
                    //    {
                    //        List<EVENT> events = mstb.getEventsByAniIdKindUbn(evbehandeling.AniId, evbehandeling.EveKind, bedr.UBNid);

                    //        evbehandeling.EveOrder = getNewEventOrderThread(mstb, events, pbehdatum.Date, evbehandeling.EveKind, bedr.UBNid, pAnimalId);
                    //    }
                    //    else
                    //    {
                    //        try
                    //        {
                    //            StringBuilder sb = new StringBuilder();
                    //            sb.Append("select count(EventId) as aantal from TREATMEN where TreMedPlanUniqueNr=" + pTreMedPlanUniqueNr.ToString());
                    //            DataTable t = mstb.GetDataBase().QueryData(pUr.getLastChildConnection(), sb);
                    //            evbehandeling.EveOrder = int.Parse(t.Rows[0][0].ToString()) + 1;
                    //        }
                    //        catch (Exception exc) { unLogger.WriteDebug(exc.ToString()); }
                    //    }
                    //}
                    evbehandeling.ThirdId = UitvoerendeThirdId;
                    evbehandeling.happened_at_FarmID = pFarmId;
                    if (mstb.SaveEvent(evbehandeling))
                    {
                        trm.EventId = evbehandeling.EventId;
                        if (i == 0)
                        {
                            TreFirstApplicationId = trm.EventId;
                        }
                        trm.TreFirstApplicationId = TreFirstApplicationId;

                        trm.TreTime = pbehdatum.Date.AddHours(i * pMedpland.MedHoursRepeat);
                        trm.MedId = pMedpland.MedId;
                        trm.ArtID = getArtikelMedicineByMedId(pUr, pMedpland.MedId);
                        trm.TreKind = pBehandeling;
                        trm.Changed_By = pChangedBy;
                        trm.SourceID = UitvoerendeThirdId;
                        if (pTreMedPlanUniqueNr == 0)
                        {
                            trm.TreMedPlanUniqueNr = TreFirstApplicationId;
                        }
                        else
                        {
                            trm.TreMedPlanUniqueNr = pTreMedPlanUniqueNr;
                        }
                        trm.TreTime = pbehdatum;
                        //hoeveelheid per kg 
                        //gewicht / MedKgAliveWeight * MedAmountPerXKg


                        MEDICINE getHoeveelheidMedicijnPerKgBerkening = new MEDICINE();
                        getHoeveelheidMedicijnPerKgBerkening.MedAmountPerXKg = pMedpland.HoeveelheidPerXkg;
                        getHoeveelheidMedicijnPerKgBerkening.MedKgAliveWeight = pMedpland.KgLevendGew;

                        //double gewicht = getCalculatedAnimalWeightThread(mstb, ani, bedr, evbehandeling.EveDate);
                        //string quant = getHoeveelheidMedicijnPerKg(gewicht, getHoeveelheidMedicijnPerKgBerkening);
                        if (pMedpland.KgLevendGew > 0 && pMedpland.HoeveelheidPerXkg > 0)
                        {
                            trm.TreQuantity = pMedpland.HoeveelheidPerXkg;
                        }
                        else
                        {
                            trm.TreQuantity = pMedpland.HoeveelheidPerXkg;
                        }
                        trm.TreDiseaseId = pTreDiseaseId;
                        //interval
                        trm.TreMedHoursRepeat = pMedpland.MedHoursRepeat;
                        trm.TreMedPlannr = pMedpland.Internalnr;

                        trm.TreGroupTreat = Convert.ToSByte(pTreGroupTreat);

                        trm.TreMedDaysTreat = 1;// pMedpland.ToedieningEind;

                        trm.TreMedUnit = MedplDmedicijn.MedUnit;
                        trm.TreMedUnitVolume = MedplDmedicijn.MedUnitVolume;
                        trm.TreDaysWaitingMeat = MedplDmedicijn.MedDaysWaitingMeat;
                        trm.TreDaysWaitingMilk = MedplDmedicijn.MedDaysWaitingMilk;
                        trm.TreMedBatchNumber = MedplDmedicijn.MedBatchNumber;
                        trm.TreMedApply = MedplDmedicijn.MedApply;
                        trm.TreMedFunction = MedplDmedicijn.MedFunction;

                        trm.TreMedReg = MedplDmedicijn.MedReg;
                        trm.TreMedUDD = MedplDmedicijn.MedUDD;




                        /* --> VERVANGEN DOOR GETMEDCOSTS
                        if (trm.TreMedDaysTreat == 0)
                        { trm.TreMedDaysTreat = 1; }
                        if (MedplDmedicijn.MedPriceUnit == 0)
                        { MedplDmedicijn.MedPriceUnit = 1; }
                        if (MedplDmedicijn.MedPrice == 0)
                        { MedplDmedicijn.MedPrice = 1; }
                        double costst = trm.TreMedDaysTreat * trm.TreQuantity * MedplDmedicijn.MedPrice / Convert.ToDouble(MedplDmedicijn.MedPriceUnit);
                        */

                        double costst = getMedCosts(MedplDmedicijn, pMedpland.KgLevendGew, trm.TreMedDaysTreat);

                        //trm.TreMedPrice = MedplDmedicijn.MedPrice;
                        trm.TrePrice = costst;
                        trm.TrePriceEuro = costst;// MedplDmedicijn.MedPriceEuro;

                        trm.TreHoursWaitingMilk = MedplDmedicijn.MedHoursWaitingMilk;
                        trm.TreHoursWaitingMeat = MedplDmedicijn.MedHoursWaitingMeat;
                        if ((i + 1) == aantalbehandelingen)
                        {
                            trm.TreDaysForControl = trm.TreMedDaysTreat * trm.TreMedHoursRepeat / 24;
                        }
                        mstb.SaveTreatmen(trm);

                        m.FarmID = bedr.FarmId;
                        m.MSDate = evbehandeling.EveDate;
                        m.MSMedId = trm.MedId;
                        m.MSMedunit = trm.TreMedUnit;
                        m.MSMutation = 0;
                        m.MSQuantity = trm.TreQuantity * trm.TreMedDaysTreat;
                        m.MSRemark = evbehandeling.EveComment;
                        m.MSTreatId = trm.EventId;
                        mstb.SaveMedstock(m);
                        if (i == 0)
                        {
                            pMelden = CheckAndInsertQkoortsMelding(pUr, evbehandeling, true, bedr, ubn);
                        }

                    }

                }
                else
                {
                    ARTIKEL MedplArt = new ARTIKEL();
                    ARTIKEL_MEDIC MedplArtmedic = new ARTIKEL_MEDIC();

                    mstb.getMedicijnArtikel(pMedpland.MedId, out MedplArt, out MedplArtmedic);

                    ANIMAL ani = mstb.GetAnimalById(pAnimalId);

                    List<MEDSTOCK> lMedsocks = mstb.getMedstocksByUbnId(bedr.UBNid);
                    evbehandeling = new EVENT();// mstb.GetEventByDateAniIdKind(pbehdatum.Date, pAnimalId, 6, bedr.UBNid);
                    TREATMEN trm = new TREATMEN();
                    MEDSTOCK m = new MEDSTOCK();
                    DateTime tijd = DateTime.Now;
                    evbehandeling.EveDate = pbehdatum.Date.AddDays((i * pMedpland.MedHoursRepeat) / 24);
                    evbehandeling.AniId = pAnimalId;
                    evbehandeling.EveKind = 6;
                    evbehandeling.UBNId = bedr.UBNid;
                    evbehandeling.EveMutationDate = tijd;
                    evbehandeling.EveMutationTime = tijd;

                    if (evbehandeling.EventId == 0)
                    {
                        List<EVENT> events = mstb.getEventsByAniIdKindUbn(evbehandeling.AniId, evbehandeling.EveKind, bedr.UBNid);

                        evbehandeling.EveOrder = getNewEventOrderThread(mstb, events, pbehdatum.Date, evbehandeling.EveKind, bedr.UBNid, pAnimalId);
                    }
                    evbehandeling.ThirdId = UitvoerendeThirdId;
                    evbehandeling.happened_at_FarmID = pFarmId;
                    if (mstb.SaveEvent(evbehandeling))
                    {
                        trm.EventId = evbehandeling.EventId;
                        if (i == 0)
                        {
                            TreFirstApplicationId = trm.EventId;
                        }

                        trm.TreFirstApplicationId = TreFirstApplicationId;

                        if (pTreMedPlanUniqueNr == 0)
                        {
                            trm.TreMedPlanUniqueNr = TreFirstApplicationId;
                        }
                        else
                        {
                            trm.TreMedPlanUniqueNr = pTreMedPlanUniqueNr;
                        }
                        trm.MedId = pMedpland.MedId;
                        trm.TreKind = pBehandeling;

                        trm.TreTime = pbehdatum.Date.AddHours(i * pMedpland.MedHoursRepeat);
                        //hoeveelheid per kg 
                        //gewicht / MedKgAliveWeight * MedAmountPerXKg
                        double gewicht = getCalculatedAnimalWeightThread(mstb, ani, bedr, evbehandeling.EveDate);

                        //MEDICINE getHoeveelheidMedicijnPerKgBerkening = new MEDICINE();
                        //getHoeveelheidMedicijnPerKgBerkening.MedAmountPerXKg = pMedpland.HoeveelheidPerXkg;
                        //getHoeveelheidMedicijnPerKgBerkening.MedKgAliveWeight = pMedpland.KgLevendGew;

                        ARTIKEL_MEDIC tempCalcArtMedic = new ARTIKEL_MEDIC();
                        tempCalcArtMedic.ArtMed_Hoeveelheid = pMedpland.HoeveelheidPerXkg;
                        tempCalcArtMedic.ArtMed_Hoeveelheid_B = pMedpland.KgLevendGew;

                        //string quant = getHoeveelheidMedicijnPerKg(gewicht, tempCalcArtMedic);// getHoeveelheidMedicijnPerKgBerkening);
                        //trm.TreQuantity = double.Parse(quant);

                        trm.TreQuantity = tempCalcArtMedic.ArtMed_Hoeveelheid;

                        //interval
                        trm.TreMedHoursRepeat = pMedpland.MedHoursRepeat;
                        trm.TreMedPlannr = pMedpland.Internalnr;
                        trm.TreDiseaseId = pTreDiseaseId;
                        trm.TreGroupTreat = Convert.ToSByte(pTreGroupTreat);


                        trm.TreMedDaysTreat = 1;// pMedpland.ToedieningEind;


                        trm.TreMedUnit = Convert.ToInt32(MedplArt.ArtStandaardEenheid); //Convert.ToInt32(MedplArt.ArtPrijsPer);// MedplDmedicijn.MedUnit;

                        trm.TreMedUnitVolume = Convert.ToInt32(MedplArt.ArtStandaardEenheid);// MedplDmedicijn.MedUnitVolume;


                        trm.TreDaysWaitingMeat = MedplArtmedic.ArtMed_DaysWaitingMeat;// MedplDmedicijn.MedDaysWaitingMeat;
                        trm.TreDaysWaitingMilk = MedplArtmedic.ArtMed_DaysWaitingMilk;// MedplDmedicijn.MedDaysWaitingMilk;
                                                                                      //trm.TreMedBatchNumber =  MedplDmedicijn.MedBatchNumber;
                        trm.TreMedApply = MedplArtmedic.ArtMed_Toedieningswijze;// MedplDmedicijn.MedApply;
                        trm.TreMedFunction = MedplArtmedic.ArtMed_Function;// MedplDmedicijn.MedFunction;

                        trm.TreMedReg = MedplArtmedic.ArtMed_Reg;// MedplDmedicijn.MedReg;
                        trm.TreMedUDD = MedplArtmedic.ArtMed_UDD;// MedplDmedicijn.MedUDD;

                        if (trm.TreMedDaysTreat == 0)
                        { trm.TreMedDaysTreat = 1; }

                        if (MedplArtmedic.ArtMed_PriceUnit == 0)
                        { MedplArtmedic.ArtMed_PriceUnit = 1; }

                        if (MedplArt.ArtVasteprijs == 0)
                        { MedplArt.ArtVasteprijs = 1; }

                        double medicatieKosten = getMedCosts(pUr, pFarmId, MedplArt.ArtId, trm.TreQuantity, trm.TreMedDaysTreat);

                        trm.TrePrice = medicatieKosten;
                        trm.TrePriceEuro = medicatieKosten;

                        trm.TreHoursWaitingMilk = MedplArtmedic.ArtMed_HoursWaitingMilk;// MedplDmedicijn.MedHoursWaitingMilk;
                        trm.TreHoursWaitingMeat = MedplArtmedic.ArtMed_HoursWaitingMeat;// MedplDmedicijn.MedHoursWaitingMeat;

                        mstb.SaveTreatmen(trm);

                        m.FarmID = bedr.FarmId;
                        m.MSDate = evbehandeling.EveDate;
                        m.MSMedId = trm.MedId;
                        m.MSMedunit = trm.TreMedUnit;
                        m.MSMutation = 0;
                        m.MSQuantity = trm.TreQuantity;
                        m.MSRemark = evbehandeling.EveComment;
                        m.MSTreatId = trm.EventId;
                        mstb.SaveMedstock(m);
                        if (i == 0)
                        {
                            pMelden = CheckAndInsertQkoortsMelding(pUr, evbehandeling, true, bedr, ubn);
                        }
                    }

                }
            }

            return TreFirstApplicationId;
        }

        public static int getArtikelMedicineByMedId(UserRightsToken pUr, int medId)
        {
            int ArtId = 0;
            if (medId > 0)
            {
                try
                {
                    AFSavetoDB lMstb = getMysqlDb(pUr);
                    MEDICINE m = lMstb.GetMedicineByMedId(medId);
                    ARTIKEL_MEDIC art = lMstb.getartikelbyMedicine(m);
                    if (art != null)
                    {
                        return art.ArtId;
                    }
                }
                catch (Exception exc) { unLogger.WriteError("getArtikelMedicineByMedId:" + exc.ToString()); }
            }
            return ArtId;
        }

        public static int insertLossebehandelplan(UserRightsToken pUr, int pFarmId, int pAnimalId, DateTime pbehdatum, MEDPLAND pMedpland, int pBehandeling, int UitvoerendeThirdId, int pTreGroupTreat, int pTreMedPlanUniqueNr, int pTreDiseaseId, int pChangedBy, out string pMelden)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            return insertLossebehandelplanThread(pUr, lMstb, pFarmId, pAnimalId, pbehdatum, pMedpland, pBehandeling, UitvoerendeThirdId, pTreGroupTreat, pTreMedPlanUniqueNr, pTreDiseaseId, pChangedBy, out pMelden);
            //MEDICINE MedplDmedicijn = lMstb.GetMedicineByMedId(pMedpland.MedId);
            //ANIMAL ani = lMstb.GetAnimalById(pAnimalId);
            //BEDRIJF bedr = lMstb.GetBedrijfById(pFarmId);
            //UBN ubn = lMstb.GetubnById(bedr.UBNid);

            //EVENT evbehandeling = new EVENT();// lMstb.GetEventByDateAniIdKind(pbehdatum.Date, pAnimalId, 6, bedr.UBNid);
            //TREATMEN trm = new TREATMEN();
            //if (evbehandeling.EventId != 0)
            //{
            //    trm = lMstb.GetTreatmen(evbehandeling.EventId);
            //}

            //DateTime tijd = DateTime.Now;
            //evbehandeling.EveDate = pbehdatum.Date;
            //evbehandeling.AniId = pAnimalId;
            //evbehandeling.EveKind = 6;
            //evbehandeling.UBNId = bedr.UBNid;
            //evbehandeling.EveMutationDate = tijd;
            //evbehandeling.EveMutationTime = tijd;
            //evbehandeling.EveMutationBy = ubn.UBNid;
            //if (evbehandeling.EventId == 0)
            //{
            //    evbehandeling.EveOrder = getNewEventOrder(pUr, pbehdatum.Date, evbehandeling.EveKind, bedr.UBNid, pAnimalId);
            //}
            //evbehandeling.ThirdId = UitvoerendeThirdId;
            //evbehandeling.happened_at_FarmID = pFarmId;
            //if (lMstb.SaveEvent(evbehandeling))
            //{
            //    trm.EventId = evbehandeling.EventId;
            //    trm.MedId = pMedpland.MedId;
            //    trm.TreKind = pBehandeling;

            //    trm.TreTime = pbehdatum;
            //    //hoeveelheid per kg 
            //    //gewicht / MedKgAliveWeight * MedAmountPerXKg
            //    double gewicht = getCalculatedAnimalWeight(pUr, ani, bedr, evbehandeling.EveDate);

            //    MEDICINE getHoeveelheidMedicijnPerKgBerkening = new MEDICINE();
            //    getHoeveelheidMedicijnPerKgBerkening.MedAmountPerXKg = pMedpland.HoeveelheidPerXkg;
            //    getHoeveelheidMedicijnPerKgBerkening.MedKgAliveWeight = pMedpland.KgLevendGew;
            //    string quant = getHoeveelheidMedicijnPerKg(gewicht, getHoeveelheidMedicijnPerKgBerkening);
            //    trm.TreQuantity = double.Parse(quant);

            //    //interval
            //    trm.TreMedHoursRepeat = pMedpland.MedHoursRepeat;
            //    trm.TreMedPlannr = pMedpland.Internalnr;

            //    trm.TreGroupTreat = Convert.ToSByte(pTreGroupTreat);


            //    trm.TreMedDaysTreat = pMedpland.ToedieningEind;


            //    trm.TreMedUnit = MedplDmedicijn.MedUnit;

            //    trm.TreMedUnitVolume = MedplDmedicijn.MedUnitVolume;


            //    trm.TreDaysWaitingMeat = MedplDmedicijn.MedDaysWaitingMeat;
            //    trm.TreDaysWaitingMilk = MedplDmedicijn.MedDaysWaitingMilk;
            //    trm.TreMedBatchNumber = MedplDmedicijn.MedBatchNumber;
            //    trm.TreMedApply = MedplDmedicijn.MedApply;
            //    trm.TreMedFunction = MedplDmedicijn.MedFunction;

            //    trm.TreMedReg = MedplDmedicijn.MedReg;
            //    trm.TreMedUDD = MedplDmedicijn.MedUDD;
            //    trm.TreMedUnit = MedplDmedicijn.MedUnit;
            //    trm.TreMedUnitVolume = MedplDmedicijn.MedUnitVolume;


            //    if (trm.TreMedDaysTreat == 0)
            //    { trm.TreMedDaysTreat = 1; }
            //    if (MedplDmedicijn.MedPriceUnit == 0)
            //    { MedplDmedicijn.MedPriceUnit = 1; }
            //    if (MedplDmedicijn.MedPrice == 0)
            //    { MedplDmedicijn.MedPrice = 1; }
            //    double costst = trm.TreMedDaysTreat * trm.TreQuantity * MedplDmedicijn.MedPrice / Convert.ToDouble(MedplDmedicijn.MedPriceUnit);

            //    //trm.TreMedPrice = MedplDmedicijn.MedPrice;
            //    trm.TrePrice = costst;
            //    trm.TrePriceEuro = costst;// MedplDmedicijn.MedPriceEuro;

            //    trm.TreHoursWaitingMilk = MedplDmedicijn.MedHoursWaitingMilk;
            //    trm.TreHoursWaitingMeat = MedplDmedicijn.MedHoursWaitingMeat;

            //    lMstb.SaveTreatmen(trm);
            //}
            //return evbehandeling.EventId;
        }

        public static string CheckAndInsertQkoortsMelding(UserRightsToken pUr, EVENT pBehandeling, bool pNewEvent, BEDRIJF pBedrijf, UBN pUbn)
        {
            //lUsername, lPassword, lTestServer,
            //pRecord.Farmnumber, lBRSnummer, pRecord.Lifenumber, pProgId, pRecord.MutationDate,
            //1,
            //ref lStatus, ref lCode, ref lOmschrijving, ref lMeldingsnr, LogFile, MaxString);
            // CodeMutation  16    17    herhaling=18
            string melden = "";
            //pNewEvent als ze het event willen updaten:Dan is  pNewEvent=false;
            char[] split = { ';' };
            if (pNewEvent == true)
            {

                AFSavetoDB lMstb = getMysqlDb(pUr);
                THIRD landUbnThird = lMstb.GetThirdByThirId(pUbn.ThrID);
                List<FARMCONFIG> farmconfigs = lMstb.getFarmConfigs(pBedrijf.FarmId);
                List<FARMCONFIG> Qkoortsen = (from n in farmconfigs where n.FKey.Contains("Qkoorts") select n).ToList();
                List<PROGRAMCONFIG> nsfoQkoortsenALL = lMstb.getProgramConfigs(pBedrijf.Programid);
                List<PROGRAMCONFIG> nsfoQkoortsen = (from n in nsfoQkoortsenALL where n.FKey.Contains("Qkoorts") select n).ToList();
                if (Qkoortsen.Count() > 0 || nsfoQkoortsen.Count() > 0)
                {
                    TREATMEN lTreatmen = lMstb.GetTreatmen(pBehandeling.EventId);
                    bool insertmutation = false;
                    int lSoortMelding = 0;
                    int lFirstBasicMelding = 0;
                    List<int> lMedicijnQkoorts = new List<int>();
                    List<int> lQkoortsplan1 = new List<int>();
                    List<int> lQkoortsplan2 = new List<int>();
                    List<int> lQkoortsplanHerh = new List<int>();
                    var sdf = from n in Qkoortsen where n.FKey == "medicijnQkoorts" select n;
                    if (sdf.Count() > 0)
                    {
                        string[] meds = sdf.ElementAt(0).FValue.Split(split);
                        foreach (string mq in meds)
                        {
                            int locMedicijnQkoorts = 0;
                            int.TryParse(mq, out locMedicijnQkoorts);
                            if (locMedicijnQkoorts > 0)
                            {
                                if (!lMedicijnQkoorts.Contains(locMedicijnQkoorts))
                                {
                                    lMedicijnQkoorts.Add(locMedicijnQkoorts);
                                }
                            }
                        }
                    }
                    var progsdf = from n in nsfoQkoortsen where n.FKey == "medicijnQkoorts" select n;
                    if (progsdf.Count() > 0)
                    {
                        string[] meds = progsdf.ElementAt(0).FValue.Split(split);
                        foreach (string mq in meds)
                        {
                            int locMedicijnQkoorts = 0;
                            int.TryParse(mq, out locMedicijnQkoorts);
                            if (locMedicijnQkoorts > 0)
                            {
                                if (!lMedicijnQkoorts.Contains(locMedicijnQkoorts))
                                {
                                    lMedicijnQkoorts.Add(locMedicijnQkoorts);
                                }
                            }
                        }
                    }

                    var tdf = from n in Qkoortsen where n.FKey == "Qkoortsplan1" select n;
                    if (tdf.Count() > 0)
                    {
                        string[] plans1 = tdf.ElementAt(0).FValue.Split(split);
                        foreach (string pl in plans1)
                        {
                            int locQkoortsplan1 = 0;
                            int.TryParse(pl, out locQkoortsplan1);
                            if (locQkoortsplan1 > 0)
                            {
                                if (!lQkoortsplan1.Contains(locQkoortsplan1))
                                {
                                    lQkoortsplan1.Add(locQkoortsplan1);
                                }
                            }
                        }
                    }
                    var progtdf = from n in nsfoQkoortsen where n.FKey == "Qkoortsplan1" select n;
                    if (progtdf.Count() > 0)
                    {
                        string[] plans1 = progtdf.ElementAt(0).FValue.Split(split);
                        foreach (string pl in plans1)
                        {
                            int locQkoortsplan1 = 0;
                            int.TryParse(pl, out locQkoortsplan1);
                            if (locQkoortsplan1 > 0)
                            {
                                if (!lQkoortsplan1.Contains(locQkoortsplan1))
                                {
                                    lQkoortsplan1.Add(locQkoortsplan1);
                                }
                            }
                        }
                    }


                    var udf = from n in Qkoortsen where n.FKey == "Qkoortsplan2" select n;
                    if (udf.Count() > 0)
                    {
                        string[] plans2 = udf.ElementAt(0).FValue.Split(split);
                        foreach (string pl in plans2)
                        {
                            int locQkoortsplan2 = 0;
                            int.TryParse(pl, out locQkoortsplan2);
                            if (locQkoortsplan2 > 0)
                            {
                                if (!lQkoortsplan2.Contains(locQkoortsplan2))
                                {
                                    lQkoortsplan2.Add(locQkoortsplan2);
                                }
                            }
                        }
                    }
                    var progudf = from n in nsfoQkoortsen where n.FKey == "Qkoortsplan2" select n;
                    if (progudf.Count() > 0)
                    {
                        string[] plans2 = progudf.ElementAt(0).FValue.Split(split);
                        foreach (string pl in plans2)
                        {
                            int locQkoortsplan2 = 0;
                            int.TryParse(pl, out locQkoortsplan2);
                            if (locQkoortsplan2 > 0)
                            {
                                if (!lQkoortsplan2.Contains(locQkoortsplan2))
                                {
                                    lQkoortsplan2.Add(locQkoortsplan2);
                                }
                            }
                        }
                    }


                    var vdf = from n in Qkoortsen where n.FKey == "Qkoortsplanherh" select n;
                    if (vdf.Count() > 0)
                    {
                        string[] plansH = vdf.ElementAt(0).FValue.Split(split);
                        foreach (string pl in plansH)
                        {
                            int locQkoortsplanHerh = 0;
                            int.TryParse(pl, out locQkoortsplanHerh);
                            if (locQkoortsplanHerh > 0)
                            {
                                if (!lQkoortsplanHerh.Contains(locQkoortsplanHerh))
                                {
                                    lQkoortsplanHerh.Add(locQkoortsplanHerh);
                                }
                            }
                        }
                    }
                    var progvdf = from n in nsfoQkoortsen where n.FKey == "Qkoortsplanherh" select n;
                    if (progvdf.Count() > 0)
                    {
                        string[] plansH = progvdf.ElementAt(0).FValue.Split(split);
                        foreach (string pl in plansH)
                        {
                            int locQkoortsplanherh = 0;
                            int.TryParse(pl, out locQkoortsplanherh);
                            if (locQkoortsplanherh > 0)
                            {
                                if (!lQkoortsplanHerh.Contains(locQkoortsplanherh))
                                {
                                    lQkoortsplanHerh.Add(locQkoortsplanherh);
                                }
                            }
                        }
                    }


                    //beslissen of er een melding klaargezet moet worden
                    //zie Qkoorts.doc samba/sys2/agrobase/documentatiealgemeen
                    if (pBehandeling.EventId > 0 && pBehandeling.AniId > 0)
                    {
                        if (lMedicijnQkoorts.Count > 0)
                        {
                            if (lMedicijnQkoorts.Contains(lTreatmen.MedId))
                            {
                                if (lTreatmen.TreMedPlannr > 0 && lQkoortsplanHerh.Contains(lTreatmen.TreMedPlannr))
                                {
                                    insertmutation = true;
                                    lSoortMelding = 3;
                                }
                                else if (lTreatmen.TreMedPlannr > 0 && lQkoortsplan2.Contains(lTreatmen.TreMedPlannr))
                                {
                                    insertmutation = true;
                                    lSoortMelding = 2;
                                }
                                else if (lTreatmen.TreMedPlannr > 0 && lQkoortsplan1.Contains(lTreatmen.TreMedPlannr))
                                {
                                    insertmutation = false;
                                    lSoortMelding = 0;//Zie ienralg.doc nieuwe versie
                                }
                                else
                                {
                                    //historische potentiele behandelingen
                                    StringBuilder bld = new StringBuilder();

                                    bld.Append(" SELECT * FROM EVENT  ");
                                    bld.Append(" JOIN TREATMEN ON TREATMEN.EventId=EVENT.EventId  ");
                                    bld.AppendFormat(" WHERE EVENT.AniId={0} AND EVENT.EventId>0 AND TREATMEN.MedId IN (" + lMstb.intListToString(lMedicijnQkoorts) + ") ", pBehandeling.AniId);
                                    //bld.Append(" AND date_format(EveDate,'%Y-%m-%d')< '" + pBehandeling.EveDate.ToString("yyyy-MM-dd") + "' ");
                                    bld.Append(" ORDER BY EveDate DESC  ");
                                    DataSet ds = new DataSet();
                                    DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), ds, bld, "", MissingSchemaAction.Add);
                                    if (tbl.Rows.Count > 1)
                                    {
                                        //Omdat de datums van invoeren niet chonologisch hoeft te gebeuren
                                        EVENT lBehandeling = new EVENT();
                                        lMstb.GetDataBase().FillObject(lBehandeling, tbl.Rows[0]);
                                        pBehandeling = lBehandeling;
                                        if (tbl.Rows.Count == 2)
                                        {
                                            insertmutation = true;
                                            lSoortMelding = 2;
                                        }
                                        else
                                        {
                                            MUTALOG alGemeld = lMstb.GetMutaLogByEventId(pBehandeling.EventId);
                                            if (alGemeld.Internalnr > 0 && (alGemeld.Returnresult == 1 || alGemeld.Returnresult == 3))
                                            {
                                                unLogger.WriteInfo("QKoorts herh melding reeds gemeld eventid=" + pBehandeling.EventId.ToString());
                                            }
                                            else
                                            {
                                                insertmutation = true;
                                                lSoortMelding = 3;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    if (insertmutation)
                    {
                        if (landUbnThird.ThrCountry == "151")
                        {
                            ANIMAL dier = lMstb.GetAnimalById(pBehandeling.AniId);
                            //Niet met animalcategory, want het dier kan op een andere administratie staan

                            MUTATION lMutQkoorts = new MUTATION();
                            MUTATION lBasicFirstMutQkoorts = new MUTATION();
                            if (lSoortMelding == 1) { lMutQkoorts.CodeMutation = 16; }
                            else if (lSoortMelding == 2) { lMutQkoorts.CodeMutation = 17; }
                            else if (lSoortMelding == 3) { lMutQkoorts.CodeMutation = 18; }
                            if (lMutQkoorts.CodeMutation > 0)
                            {
                                lMutQkoorts.AlternateLifeNumber = dier.AniAlternateNumber;
                                lMutQkoorts.CountryCodeBirth = dier.AniCountryCodeBirth;
                                lMutQkoorts.EventId = pBehandeling.EventId;
                                lMutQkoorts.Farmnumber = pUbn.Bedrijfsnummer;
                                lMutQkoorts.IDRBirthDate = dier.AniBirthDate;
                                lMutQkoorts.Haircolor = dier.Anihaircolor;
                                lMutQkoorts.AniHaircolor_Memo = dier.AniHaircolor_Memo;
                                lMutQkoorts.Lifenumber = dier.AniAlternateNumber;
                                if (dier.AniIdFather > 0)
                                {
                                    ANIMAL vader = lMstb.GetAnimalById(dier.AniIdFather);
                                    lMutQkoorts.LifeNumberFather = vader.AniAlternateNumber;
                                }
                                if (dier.AniIdMother > 0)
                                {
                                    ANIMAL moeder = lMstb.GetAnimalById(dier.AniIdMother);
                                    lMutQkoorts.LifenumberMother = moeder.AniAlternateNumber;
                                }

                                lMutQkoorts.MutationDate = pBehandeling.EveDate;
                                lMutQkoorts.MutationTime = pBehandeling.EveDate;
                                lMutQkoorts.Name = dier.AniName;
                                lMutQkoorts.Nling = dier.AniNling;
                                lMutQkoorts.Sex = dier.AniSex;
                                lMutQkoorts.ThrID = pUbn.ThrID;
                                lMutQkoorts.UbnId = pUbn.UBNid;
                                lMutQkoorts.Worknumber = "";
                                lMstb.SaveMutation(lMutQkoorts);
                                melden = "melden";
                            }
                        }
                    }
                }
            }
            return melden;
        }

        #region Medicijnen berekeningen

        [Obsolete("gebruik : getHoeveelheidMedicijnPerKg(double pGewicht,ARTIKEL_MEDIC pArtMedic)")]
        public static string getHoeveelheidMedicijnPerKg(double pGewicht, MEDICINE pMed)
        {
            double quantity = 0;

            if ((pMed.MedKgAliveWeight > 0) && (pMed.MedAmountPerXKg > 0))
            {
                //bepaal dosering                
                //MedKgAliveWeight = dosering per x kg
                //MedAmountPerXKg = hoeveelheid per MedKgAliveWeight

                //voorbeeld;
                //Gewicht = 200kg, MedKgAliveWeight = 100kg, MedAmountPerXKg = 10cc
                //quantity = (200/100) * 10 = 20cc

                quantity = (pGewicht / pMed.MedKgAliveWeight) * pMed.MedAmountPerXKg;
            }

            return Math.Round(quantity, 2).ToString();
        }

        [Obsolete("gebruik :  getMedCosts(ARTIKEL pArt,ARTIKEL_MEDIC pArtMedic, double pGewicht, int pTreMedDaysTreat) ")]
        public static double getMedCosts(MEDICINE pMed, double pGewicht, int pTreMedDaysTreat)
        {
            double Quantity = double.Parse(getHoeveelheidMedicijnPerKg(pGewicht, pMed));
            if (pTreMedDaysTreat == 0)
            {
                pTreMedDaysTreat = pMed.MedDaysTreat;
            }
            if (pTreMedDaysTreat == 0)
            { pTreMedDaysTreat = 1; }
            if (pMed.MedPriceUnit == 0)
            { return 0; }
            if (pMed.MedPrice == 0)
            { return 0; }
            return pTreMedDaysTreat * Quantity * pMed.MedPrice / Convert.ToDouble(pMed.MedPriceUnit);

        }

        public static double getMedCosts(UserRightsToken pUr, int farmId, int artId, double pTreQuantity, int pTreMedDaysTreat)
        {
            AFSavetoDB mstb = (AFSavetoDB)Facade.GetInstance().getSaveToDB(pUr);

            ARTIKEL pArt = new ARTIKEL();
            ARTIKEL_MEDIC pArtMedic = new ARTIKEL_MEDIC();
            mstb.getMedicijnArtikel(artId, out pArt, out pArtMedic);

            if (pTreMedDaysTreat == 0)
            {
                pTreMedDaysTreat = pArtMedic.ArtMed_NrOfRepeat;
            }

            if (pTreQuantity == 0)
            {
                pTreQuantity = pArtMedic.ArtMed_Hoeveelheid;
                if (pTreQuantity == 0)
                {
                    pTreQuantity = 1;
                }
            }

            if (pArtMedic.ArtMed_PriceUnit == 0)
            {
                pArtMedic.ArtMed_PriceUnit = 1;
            }

            double artikelPrijs = mstb.getArtVastePrijs(farmId, pArt.ArtId);

            if (artikelPrijs == 0)
            {
                //dan weten we echt niet hoeveel het kost 
                return 0;
            }
            //1e poging
            //return pTreMedDaysTreat * pTreQuantity * artikelPrijs / Convert.ToDouble(pArtMedic.ArtMed_PriceUnit);


            double eenheidbedrag = getPrijsperToedieningseenheid(mstb, artikelPrijs, pArt, pArtMedic);
            //    //artikelPrijs = ArtAantalper 3 per ArtPrijsper 59=stuks : 
            //ik moet weten per kg en per toedieningseenheid per KG
            if (pTreMedDaysTreat == 0) { pTreMedDaysTreat = 1; }
            if (pTreQuantity == 0) { pTreQuantity = 1; }
            if (pArtMedic.ArtMed_Hoeveelheid_B == 0) { pArtMedic.ArtMed_Hoeveelheid_B = 1; }
            return pTreMedDaysTreat * pTreQuantity * eenheidbedrag / Convert.ToDouble(pArtMedic.ArtMed_Hoeveelheid_B);
        }

        private static double getPrijsperToedieningseenheid(AFSavetoDB mstb, double pVasteArtikelPrijs, ARTIKEL pArt, ARTIKEL_MEDIC pArtMedic)
        {
            //dit is dus de prijs per pArtMedic.ArtMed_Eenheid

            if (pArt.ArtStandaardEenheid == 0) { pArt.ArtStandaardEenheid = 1; }
            double stuksprijs = pVasteArtikelPrijs / pArt.ArtStandaardEenheid;
            if (pArt.ArtStdMaakHvlhd == 0) { pArt.ArtStdMaakHvlhd = 1; }
            double prijspervolumeeenheid = stuksprijs / pArt.ArtStdMaakHvlhd;
            List<AGRO_LABELS> lbls = mstb.GetAgroLabels(211, 528, 0, 0);
            double verhouding = 1;
            if (lbls.Count > 0)
            {
                verhouding = getomrekenFactor211(pArt.ArtPrijsPer, pArtMedic.ArtMed_Eenheid); //vermenigvuldigen met  
            }
            else
            {
                verhouding = getomrekenFactor217(pArt.ArtPrijsPer, pArtMedic.ArtMed_Eenheid); //vermenigvuldigen met
            }
            double prijsje = verhouding * prijspervolumeeenheid;
            return prijsje;
        }

        private static double getomrekenFactor217(long pArtPrijsPer, int pArtMed_Eenheid)
        {
            //gebaseerd op labkind 217  eenheden
            if (pArtPrijsPer == pArtMed_Eenheid)
            {
                return 1;
            }
            else
            {
                double verhouding = 1;
                if (pArtMed_Eenheid == 17 || pArtMed_Eenheid == 18)
                {

                    if (pArtPrijsPer == 1 || pArtPrijsPer == 4)
                    {
                        verhouding = 10;
                    }
                    else if (pArtPrijsPer == 2 || pArtPrijsPer == 7)
                    {

                        verhouding = 1 / 1000000;
                    }
                    else if (pArtPrijsPer == 5)
                    {

                        verhouding = 1 / 1000;
                    }
                    else if (pArtPrijsPer == 3 || pArtPrijsPer == 6)
                    {

                        verhouding = 1000;
                    }
                }
                else if (pArtMed_Eenheid == 16 || pArtMed_Eenheid == 15 || pArtMed_Eenheid == 14 || pArtMed_Eenheid == 13 || pArtMed_Eenheid == 12 || pArtMed_Eenheid == 11 || pArtMed_Eenheid == 10)
                {
                    verhouding = 1;
                }
                else if (pArtMed_Eenheid > 0 && pArtMed_Eenheid < 8)
                {
                    switch (pArtMed_Eenheid)
                    {
                        case 1:
                            switch (pArtPrijsPer)
                            {
                                case 2:
                                    //liter ml
                                    verhouding = 1 / 1000;
                                    break;
                                case 3:
                                    verhouding = 1000;
                                    break;
                            }
                            break;
                        case 3:
                            switch (pArtPrijsPer)
                            {
                                case 1:
                                    verhouding = 1 / 1000;
                                    break;
                                case 2:
                                    verhouding = 1 / 1000000;
                                    break;
                            }
                            break;
                        case 4:
                            switch (pArtPrijsPer)
                            {
                                case 5:
                                    verhouding = 1 / 1000;
                                    break;
                                case 6:
                                    verhouding = 1000;
                                    break;
                                case 7:
                                    verhouding = 1 / 1000000;
                                    break;
                            }
                            break;
                        case 5:
                            switch (pArtPrijsPer)
                            {
                                case 4:
                                    verhouding = 1000;
                                    break;
                                case 6:
                                    verhouding = 1000000;
                                    break;
                                case 7:
                                    verhouding = 1 / 1000;
                                    break;
                            }
                            break;
                        case 6:
                            break;

                    }
                }
                return verhouding;
            }
        }

        private static double getomrekenFactor211(long pArtPrijsPer, int pArtMed_Eenheid)
        {
            int ArtPrijsPer = (int)pArtPrijsPer;
            //gebaseerd op labkind 211  eenheden
            int[] massaKG = { 1, 2, 4, 5, 7 };
            int[] volumeLTR = { 10, 11, 12, 13 };
            int[] afstandMTR = { 38, 39, 40, 41 };
            //de rest is appels met peren vergelijken
            if (ArtPrijsPer == pArtMed_Eenheid)
            {
                return 1;
            }
            else
            {
                double verhouding = 1;

                if (massaKG.Contains(ArtPrijsPer) && massaKG.Contains(pArtMed_Eenheid))
                {
                    switch (ArtPrijsPer)
                    {
                        case 1:
                            switch (pArtMed_Eenheid)
                            {
                                case 2:
                                    verhouding = 100;
                                    break;
                                case 4:
                                    verhouding = 1 / 1000;
                                    break;
                                case 5:
                                    verhouding = 1000;
                                    break;
                                case 7:
                                    verhouding = 1 / 1000000;
                                    break;
                            }
                            break;
                        case 2:
                            switch (pArtMed_Eenheid)
                            {
                                case 1:
                                    verhouding = 1 / 100;
                                    break;
                                case 4:
                                    verhouding = 1 / 100000;
                                    break;
                                case 5:
                                    verhouding = 10;
                                    break;
                                case 7:
                                    verhouding = 1 / 100000000;
                                    break;
                            }
                            break;
                        case 4:
                            switch (pArtMed_Eenheid)
                            {
                                case 1:
                                    verhouding = 1000;
                                    break;
                                case 2:
                                    verhouding = 100000;
                                    break;
                                case 5:
                                    verhouding = 1000000;
                                    break;
                                case 7:
                                    verhouding = 1 / 1000;
                                    break;
                            }
                            break;
                        case 5:
                            switch (pArtMed_Eenheid)
                            {
                                case 1:
                                    verhouding = 1 / 1000;
                                    break;
                                case 2:
                                    verhouding = 1 / 10;
                                    break;
                                case 4:
                                    verhouding = 1 / 1000000;
                                    break;
                                case 7:
                                    verhouding = 1 / 1000000000;
                                    break;
                            }
                            break;
                        case 7:
                            switch (pArtMed_Eenheid)
                            {
                                case 1:
                                    verhouding = 1000000;
                                    break;
                                case 2:
                                    verhouding = 100000000;
                                    break;
                                case 4:
                                    verhouding = 1000;
                                    break;
                                case 5:
                                    verhouding = 1000000000;
                                    break;
                            }
                            break;
                    }
                }
                else if (volumeLTR.Contains(ArtPrijsPer) && volumeLTR.Contains(pArtMed_Eenheid))
                {
                    switch (ArtPrijsPer)
                    {
                        case 10:
                            switch (pArtMed_Eenheid)
                            {
                                case 11:
                                    verhouding = 1 / 10;
                                    break;
                                case 12:
                                    verhouding = 1 / 100;
                                    break;
                                case 13:
                                    verhouding = 1 / 1000;
                                    break;
                            }
                            break;
                        case 11:
                            switch (pArtMed_Eenheid)
                            {
                                case 10:
                                    verhouding = 10;
                                    break;
                                case 12:
                                    verhouding = 1 / 10;
                                    break;
                                case 13:
                                    verhouding = 1 / 100;
                                    break;
                            }
                            break;
                        case 12:
                            switch (pArtMed_Eenheid)
                            {
                                case 10:
                                    verhouding = 100;
                                    break;
                                case 11:
                                    verhouding = 10;
                                    break;
                                case 13:
                                    verhouding = 1 / 10;
                                    break;
                            }
                            break;
                        case 13:
                            switch (pArtMed_Eenheid)
                            {
                                case 10:
                                    verhouding = 1000;
                                    break;
                                case 11:
                                    verhouding = 100;
                                    break;
                                case 12:
                                    verhouding = 10;
                                    break;
                            }
                            break;
                    }
                }
                else if (afstandMTR.Contains(ArtPrijsPer) && afstandMTR.Contains(pArtMed_Eenheid))
                {
                    switch (ArtPrijsPer)
                    {
                        case 38:
                            switch (pArtMed_Eenheid)
                            {
                                case 39:
                                    verhouding = 1 / 1000;
                                    break;
                                case 40:
                                    verhouding = 1 / 10000;
                                    break;
                                case 41:
                                    verhouding = 1 / 100000;
                                    break;
                            }
                            break;
                        case 39:
                            switch (pArtMed_Eenheid)
                            {
                                case 38:
                                    verhouding = 1000;
                                    break;
                                case 40:
                                    verhouding = 1 / 100;
                                    break;
                                case 41:
                                    verhouding = 1 / 1000;
                                    break;
                            }
                            break;
                        case 40:
                            switch (pArtMed_Eenheid)
                            {
                                case 38:
                                    verhouding = 100000;
                                    break;
                                case 39:
                                    verhouding = 100;
                                    break;
                                case 41:
                                    verhouding = 1 / 10;
                                    break;
                            }
                            break;
                        case 41:
                            switch (pArtMed_Eenheid)
                            {
                                case 38:
                                    verhouding = 1000000;
                                    break;
                                case 39:
                                    verhouding = 1000;
                                    break;
                                case 40:
                                    verhouding = 10;
                                    break;
                            }
                            break;
                    }
                }

                return verhouding;
            }
        }

        public static string getHoeveelheidMedicijnPerKg(double pGewicht, ARTIKEL_MEDIC pArtMedic)
        {
            return "0"; //erik - uitgezet, dit werkt anders voor ARTIKEL_MEDIC

            StringBuilder sb = new StringBuilder();
            if (pArtMedic.ArtMed_Hoeveelheid_B != 0)// pMed.MedKgAliveWeight != 0)
            {
                if (pArtMedic.ArtMed_Hoeveelheid != 0)// pMed.MedAmountPerXKg != 0)
                {
                    sb.Append(Math.Round(((pGewicht / pArtMedic.ArtMed_Hoeveelheid_B) * pArtMedic.ArtMed_Hoeveelheid), 2).ToString());
                }
                else
                {
                    sb.Append(Math.Round((pGewicht / pArtMedic.ArtMed_Hoeveelheid_B), 2).ToString());
                }
            }
            else
            {
                if (pArtMedic.ArtMed_Hoeveelheid != 0)
                {
                    sb.Append(Math.Round(((pGewicht / 50) * pArtMedic.ArtMed_Hoeveelheid), 2).ToString());
                }
                else
                {
                    sb.Append(Math.Round((pGewicht / 50), 2).ToString());
                }

            }
            return sb.ToString();
        }

        #endregion

        public static int TreatmenStop(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, DateTime pDateCheck)
        {
            //BUG 1501 dier is dood maar wordt nog behandeld
            int aantaltoedieningen = 0;
            try
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                DataTable AnimalTreatmens = lMstb.getAnimalTreatMenGegevens(pAnimal.AniId);
                //ORDER BY = EveDate desc
                foreach (DataRow rw in AnimalTreatmens.Rows)
                {
                    DateTime Evedate = getDatumFormat(rw["EveDate"], "EveDate");
                    if (Evedate.Date <= pDateCheck.Date)
                    {
                        //beide integers
                        if (rw["TreMedDaysTreat"] != DBNull.Value && rw["TreMedHoursRepeat"] != DBNull.Value)
                        {
                            int TreMedDaysTreat = 0;
                            int.TryParse(rw["TreMedDaysTreat"].ToString(), out TreMedDaysTreat);
                            int TreMedHoursRepeat = 0;
                            int.TryParse(rw["TreMedHoursRepeat"].ToString(), out TreMedHoursRepeat);
                            DateTime lastDate = Evedate.AddHours((double)(TreMedDaysTreat * TreMedHoursRepeat));
                            if (lastDate > pDateCheck.Date)
                            {
                                if (TreMedDaysTreat > 1)
                                {
                                    TimeSpan tSp = pDateCheck.Date.Subtract(Evedate);

                                    aantaltoedieningen = (int)tSp.TotalHours / TreMedHoursRepeat;
                                    if (aantaltoedieningen > 0)
                                    {
                                        TREATMEN tr = new TREATMEN();
                                        lMstb.GetDataBase().FillObject(tr, rw);
                                        tr.TreMedDaysTreat = aantaltoedieningen;
                                        lMstb.SaveTreatmen(tr);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return aantaltoedieningen;
        }

        public static double getGemiddeldGewicht(UserRightsToken pUserRightsToken, BEDRIJF pBedrijf, DataTable pGeselecteerddieren, DateTime pBehandeldag, out double pTotalWeight)
        {
            double gewicht = 0;
            pTotalWeight = 0;
            if (pBehandeldag == DateTime.MinValue)
            { pBehandeldag = DateTime.Now; }
            if (pBedrijf.FarmId > 0 && pGeselecteerddieren.Rows.Count > 0)
            {
                List<int> geselecteerddieren = new List<int>();
                IEnumerable<object> geselecteerd = from n in pGeselecteerddieren.Select()
                                                   select n["AniId"];
                if (geselecteerd.Count() > 0)
                {
                    geselecteerddieren = geselecteerd.Select(s => Convert.ToInt32(s)).ToList();
                    AFSavetoDB lMstb = getMysqlDb(pUserRightsToken);
                    int groupnr = lMstb.GetGroupnr(geselecteerddieren.ElementAt(0), pBehandeldag);
                    int curvenr = 0;

                    //verkrijg curvenr uit de group of farm

                    if (groupnr > 0)
                    {
                        GROUPS group = lMstb.GetGroups(pBedrijf.FarmId, groupnr);
                        curvenr = group.WeightCurve;
                    }
                    else
                    {
                        List<WGCURVE> wgcurveList = lMstb.GetWgcurveListByFarmId(pBedrijf.FarmId);
                        var selected_one = from n in wgcurveList where n.Selected == 1 && n.AniKind == pBedrijf.ProgId && n.Program == pBedrijf.Programid select n;
                        if (selected_one.Count() > 0)
                        {
                            curvenr = selected_one.ElementAt(0).Curvenr;
                        }
                        else
                        {
                            selected_one = from n in wgcurveList where n.Selected == 1 && n.AniKind == pBedrijf.ProgId select n;
                            if (selected_one.Count() > 0)
                            {
                                curvenr = selected_one.ElementAt(0).Curvenr;
                            }
                            else
                            {
                                selected_one = from n in wgcurveList where n.Selected == 1 select n;
                                if (selected_one.Count() > 0)
                                {
                                    curvenr = selected_one.ElementAt(0).Curvenr;
                                }
                                else
                                {
                                    if (wgcurveList.Count() > 0)
                                    {
                                        curvenr = wgcurveList.ElementAt(0).Curvenr;
                                    }
                                }
                            }
                        }
                    }

                    //Verkrijg curve
                    List<WGCURVED> wcdList = GetWgcurveDList(lMstb, pBedrijf, curvenr);
                    List<WEIGHT> wsAll = lMstb.getWeights(geselecteerddieren);
                    double lGewicht = 0;
                    foreach (int lAniId in geselecteerddieren)
                    {

                        DataRow[] lAnimalRow = pGeselecteerddieren.Select("AniId=" + lAniId.ToString());
                        if (lAnimalRow.Count() > 0)
                        {
                            DateTime startdate = getDatumFormat(lAnimalRow[0]["AniBirthDate"], "AniBirthDate");
                            var ws = from n in wsAll
                                     where n.AniId == lAniId
                                     select n;
                            List<WGCURVED> lWcdList = new List<WGCURVED>();
                            if (ws.Count() > 2)
                            {
                                //ach het beestje is toch gewogen

                                foreach (WEIGHT w in ws)
                                {
                                    WGCURVED wn = new WGCURVED();
                                    wn.Curvenr = 0;
                                    wn.fd_Day = Convert.ToInt32(w.WeightDate.Subtract(startdate).TotalDays);
                                    wn.WeightKg = w.WeightKg;
                                    lWcdList.Add(wn);
                                }
                            }

                            if (lWcdList.Count() > 0)
                            {
                                lGewicht = getAnimalWeight_By_WGCURVED(lWcdList, startdate, pBehandeldag);
                            }
                            else
                            {
                                lGewicht = getAnimalWeight_By_WGCURVED(wcdList, startdate, pBehandeldag);
                            }
                            pTotalWeight += lGewicht;
                        }
                    }
                }
            }
            if (pGeselecteerddieren.Rows.Count > 0)
            {
                gewicht = pTotalWeight / pGeselecteerddieren.Rows.Count;
            }
            else { gewicht = pTotalWeight; }
            return gewicht;
        }

        public static double getCalculatedAnimalWeightThread(AFSavetoDB pMstb, ANIMAL ani, BEDRIJF bedr, DateTime behandeldag)
        {
            double gewicht = 0;

            int groupnr = pMstb.GetGroupnr(ani.AniId, behandeldag);
            int curvenr = 1;

            //verkrijg curvenr uit de group of farm

            if (groupnr != 0)
            {
                GROUPS group = pMstb.GetGroups(bedr.FarmId, groupnr);
                curvenr = group.WeightCurve;
            }
            else
            {
                List<WGCURVEFARM> wgcurvefarmList = pMstb.getWGCurveFarms(bedr.FarmId);

                foreach (WGCURVEFARM wgcurvefarm in wgcurvefarmList)
                {
                    if (wgcurvefarm.Curvenr > 0)
                    {
                        curvenr = wgcurvefarm.Curvenr;
                        break;
                    }
                }
            }

            //Verkrijg curve
            List<WGCURVED> wcdList = GetWgcurveDList(pMstb, bedr, curvenr);


            List<WEIGHT> ws = pMstb.getWeights(ani.AniId);
            if (ws.Count() > 2)
            {
                //ach het beestje is toch gewogen
                wcdList = new List<WGCURVED>();
                DateTime startdate = ani.AniBirthDate;
                foreach (WEIGHT w in ws)
                {
                    WGCURVED wn = new WGCURVED();
                    wn.Curvenr = 0;
                    wn.fd_Day = Convert.ToInt32(w.WeightDate.Subtract(startdate).TotalDays);
                    wn.WeightKg = w.WeightKg;
                    wcdList.Add(wn);
                }
            }

            //Verkrijg gewicht met de curveList
            gewicht = getAnimalWeight_By_WGCURVED(wcdList, ani.AniBirthDate, behandeldag);



            return gewicht;
        }

        public static List<WGCURVED> GetWgcurveDList(AFSavetoDB pMstb, BEDRIJF pBedrijf, int pCurvenr)
        {
            List<WGCURVED> wcdList = new List<WGCURVED>();
            if (pCurvenr > 0)
            {
                wcdList = pMstb.GetWgcurveDList(pCurvenr);
            }
            else
            {
                if ((pBedrijf.ProgId != 3) && (pBedrijf.ProgId != 5))
                {
                    //Rundvee default gewicht
                    //tot 7 dagen = 43kg
                    //56 dagen = 80kg
                    //vanaf 200 dagen = 245kg

                    wcdList = new List<WGCURVED>();

                    for (int i = 0; i < 3; i++)
                    {
                        WGCURVED wcd = new WGCURVED();

                        switch (i)
                        {
                            case 0:
                                wcd.fd_Day = 7;
                                wcd.WeightKg = 43;
                                break;
                            case 1:
                                wcd.fd_Day = 56;
                                wcd.WeightKg = 80;
                                break;
                            case 2:
                                wcd.fd_Day = 200;
                                wcd.WeightKg = 245;
                                break;
                        }

                        wcdList.Add(wcd);
                    }
                }
                else
                {
                    string lCurvenr_str = pMstb.GetProgramConfigValue(pBedrijf.Programid, "ideaalcurve", "1");
                    List<WGCURVED> lIdeaal = new List<WGCURVED>();
                    if (lCurvenr_str != null)
                    {
                        int lCurvenr = 0;

                        if (int.TryParse(lCurvenr_str, out lCurvenr))
                        {
                            lIdeaal = pMstb.GetWgcurveDList(lCurvenr);
                        }
                    }
                    if (lIdeaal.Count() == 0)
                    {

                    }
                }
            }
            return wcdList;
        }

        public static double getCalculatedAnimalWeight(UserRightsToken pUr, ANIMAL ani, BEDRIJF bedr, DateTime behandeldag)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            double gewicht = getCalculatedAnimalWeightThread(lMstb, ani, bedr, behandeldag);
            return gewicht;
        }

        public static double getAnimalWeight_By_WGCURVED(List<WGCURVED> wcdList, DateTime aniBirthDate, DateTime datum)
        {
            //bepaal gewicht dmv. curve, waarbij de waarde wordt berekend met een lineaire functie (y=ax+b);

            double gewicht = 0;

            if (wcdList != null)
            {
                if (wcdList.Count >= 2) //minimaal 2 punten nodig voor interval
                {
                    TimeSpan sp = datum.Subtract(aniBirthDate);
                    int levensdagen = (int)sp.TotalDays;

                    if (levensdagen <= wcdList[0].fd_Day)
                    {
                        gewicht = wcdList[0].WeightKg;
                    }
                    else if (levensdagen >= wcdList[wcdList.Count - 1].fd_Day)
                    {
                        gewicht = wcdList[wcdList.Count - 1].WeightKg;
                    }
                    else
                    {
                        //default
                        int startDay = 0;
                        int endDay = 0;
                        double startWeight = 0;
                        double endWeight = 0;

                        //aantal dagen bevind zicht tussen een lineaire interval, verkrijg grenswaarden
                        for (int i = 1; i < wcdList.Count; i++)
                        {
                            if (levensdagen <= wcdList[i].fd_Day)
                            {
                                startDay = wcdList[i - 1].fd_Day;
                                startWeight = wcdList[i - 1].WeightKg;

                                endDay = wcdList[i].fd_Day;
                                endWeight = wcdList[i].WeightKg;

                                break;
                            }
                        }

                        //bereken gewicht
                        double diffWeight = endWeight - startWeight;
                        int diffDays = endDay - startDay;

                        gewicht = ((diffWeight / diffDays) * (levensdagen - startDay)) + startWeight;
                    }
                }
            }

            return gewicht;
        }

        public static double getCalculatedSalAliveWeight(UserRightsToken pUr, int programid, int farmid, double SalCarcassWeight)
        {
            double SalAliveWeight = 0;
            string FKey_Aanhoudperc = "Aanhoudperc";
            string def_Aanhoudperc = "54,5";
            double Aanhoudperc = 54.5;
            if (SalCarcassWeight > 0)
            {
                string tmpStr = Facade.GetInstance().getSaveToDB(pUr).GetConfigValue(programid, farmid, FKey_Aanhoudperc, def_Aanhoudperc);
                if (double.TryParse(tmpStr, out Aanhoudperc))
                {
                    if (Aanhoudperc > 0)
                        SalAliveWeight = Math.Round(((SalCarcassWeight / Aanhoudperc) * 10000) / 100, 2);
                }
            }
            return SalAliveWeight;
        }

        public static DateTime EinddatumsBehandeling(TREATMEN pTr, DateTime behDatum, out DateTime pEindeWT2, int soortDieren)
        {
            //einde behandeling
            DateTime eindeBehandeling = behDatum;// +dr["TreTime"];
            pEindeWT2 = behDatum;
            int uren = 0;
            if (pTr.TreMedHoursRepeat > 0)
            { uren = pTr.TreMedHoursRepeat; }
            else { uren = 24; }

            if (pTr.TreMedDaysTreat > 1)
            {
                float tmpDays = (pTr.TreMedDaysTreat - 1) * (uren / 24);
                eindeBehandeling = eindeBehandeling.AddDays(tmpDays);
            }
            else if (uren > 0)
            { eindeBehandeling = eindeBehandeling.AddDays(uren / 24); }
            //einde wachttijd
            uren = 0;
            if (soortDieren == 0)
            { soortDieren = pTr.TreKindAnimal; }// int.Parse(pTr.TreKindAnimal dr["TreKindAnimal"].ToString()); }

            if (soortDieren == 1) //melk
            {
                if (pTr.TreHoursWaitingMilk > 0)
                { uren = pTr.TreHoursWaitingMilk; }
                else
                { uren = pTr.TreDaysWaitingMilk * 24; }
            }
            else if (soortDieren == 2) //vlees
            {
                if (pTr.TreHoursWaitingMeat > 0)
                { uren = pTr.TreHoursWaitingMeat; }
                else
                { uren = pTr.TreDaysWaitingMeat * 24; }
            }

            DateTime tmp = eindeBehandeling;
            if (uren > 0)
            { pEindeWT2 = tmp.AddDays(uren / 24); }
            //.AddDays(uren / 24)
            //return uren > 0 ? tmp : DateTime.MinValue;
            return eindeBehandeling;
        }

        public static List<SECONRAC> berekenRasDeelOLD(UserRightsToken pUr, int AniIdMother, int AniIdFather, int progid)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<SECONRAC> rasMa = lMstb.GetSeconRacSByAnimalId(AniIdMother);
            List<SECONRAC> rasPa = lMstb.GetSeconRacSByAnimalId(AniIdFather);
            List<SECONRAC> rasKid = new List<SECONRAC>();

            //mammie
            if (progid == 5 && rasMa.Count == 0)
            {
                SECONRAC sr = new SECONRAC();
                sr.RacId = 8; //overig 100%
                sr.SraRate = 8;
                rasKid.Add(sr);
            }
            else if (progid == 3 && rasMa.Count == 0)
            {
                SECONRAC sr = new SECONRAC();
                sr.RacId = 35; //NN onbekend 100%
                sr.SraRate = 8;
                rasKid.Add(sr);
            }
            else
            {

                foreach (SECONRAC sr in rasMa)
                {
                    SECONRAC sr2 = new SECONRAC();
                    sr2.RacId = sr.RacId;
                    sr2.SraRate = sr.SraRate;
                    rasKid.Add(sr2);
                }
            }

            //pappie
            if (
                ((progid == 5 || progid == 3) && (rasPa.Count == 0))
                    ||
                (AniIdFather == 0)
               )
            {

                if (progid == 3)
                {
                    SECONRAC sr = new SECONRAC();
                    sr.RacId = 35; //NN onbekend 100%
                    sr.SraRate = 8;
                    rasKid.Add(sr);
                }
                else
                {
                    SECONRAC sr = new SECONRAC();
                    sr.RacId = 8; //overig 100%
                    sr.SraRate = 8;
                    rasKid.Add(sr);
                }

            }
            else
            {
                foreach (SECONRAC sr in rasPa)
                {
                    SECONRAC sr2 = new SECONRAC();
                    sr2.RacId = sr.RacId;
                    sr2.SraRate = sr.SraRate;
                    var temp = from n in rasKid
                               where n.RacId == sr2.RacId
                               select n;
                    bool addras = true;
                    if (temp != null)
                    {
                        if (temp.Count() == 1)
                        {
                            temp.First().SraRate = temp.First().SraRate + sr2.SraRate;
                            addras = false;
                        }
                    }
                    if (addras)
                    {
                        rasKid.Add(sr2);
                    }
                }
            }

            //1 race for dad/mom that's the same
            if (rasMa.Count == 1 && rasPa.Count == 1)
            {
                if (rasMa[0].RacId == rasPa[0].RacId)
                {
                    rasKid.Clear();
                    SECONRAC sr = new SECONRAC();
                    sr.RacId = rasMa[0].RacId;
                    sr.SraRate = rasMa[0].SraRate;
                    rasKid.Add(sr);
                }
                else
                {
                    foreach (SECONRAC sr in rasKid)
                    {
                        double tmp1 = sr.SraRate / 2;
                        sr.SraRate = Convert.ToInt32(Math.Truncate(tmp1));
                    }
                }
            }
            else if (rasMa.Count == 1 && rasPa.Count == 0)
            {
                rasKid.Clear();
                SECONRAC sr = new SECONRAC();
                sr.RacId = rasMa[0].RacId;
                sr.SraRate = rasMa[0].SraRate;
                rasKid.Add(sr);
            }
            else
            {

                foreach (SECONRAC sr in rasKid)
                {
                    double tmp = sr.SraRate / 2;
                    if (sr.SraRate <= 1)
                    { tmp = 1; }
                    sr.SraRate = Convert.ToInt32(Math.Truncate(tmp));
                }
            }
            return rasKid;
        }

        public static List<SECONRAC> berekenRasDeel(UserRightsToken pUr, int AniIdMother, int AniIdFather, int progid, int programid)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<SECONRAC> rasMa = lMstb.GetSeconRacSByAnimalId(AniIdMother);
            List<SECONRAC> rasPa = lMstb.GetSeconRacSByAnimalId(AniIdFather);
            return berekenRasDeel(pUr, rasMa, rasPa, progid, programid);
        }

        /// <summary>
        /// Calc races
        /// </summary>
        /// <param name="AniIdMother">AniId mother</param>
        /// <param name="AniIdFather">AniId father (0 if none)</param>
        /// <returns>
        /// List races for kiddo. Warning: AniId for SECONRAC return values not set!
        /// Zie document berekening_ras.doc in documentatie algemeen
        /// </returns>
        public static List<SECONRAC> berekenRasDeel(UserRightsToken pUr, List<SECONRAC> rasMa, List<SECONRAC> rasPa, int progid, int programid)
        {
            //Zie document berekening_ras.doc in documentatie algemeen
            AFSavetoDB lMstb = getMysqlDb(pUr);

            List<SECONRAC> rasKid = new List<SECONRAC>();
            List<int> onbekende = new List<int>();
            int aanvullenmet = 99;
            if (progid == 5)
            {
                onbekende.Add(8);
                aanvullenmet = 8;

            }
            else if (progid == 3)
            {
                onbekende.Add(39);//OV
                onbekende.Add(35);//NN
                aanvullenmet = 39;

            }
            else
            {
                onbekende.Add(99);

            }

            rasMa = (from n in rasMa
                     where (!onbekende.Contains(n.RacId))
                     select n).ToList();
            rasPa = (from n in rasPa
                     where (!onbekende.Contains(n.RacId))
                     select n).ToList();

            //mammie
            if (rasMa.Count == 0)
            {
                SECONRAC sr = new SECONRAC();
                sr.RacId = aanvullenmet;
                sr.SraRate = 8;
                rasKid.Add(sr);
            }
            else
            {
                if (rasMa.Count() == 1)
                {
                    foreach (SECONRAC sr in rasMa)
                    {
                        SECONRAC sr2 = new SECONRAC();
                        sr2.RacId = sr.RacId;
                        sr2.SraRate = sr.SraRate;
                        rasKid.Add(sr2);
                    }
                }
                else if (rasMa.Count() == 2)
                {
                    foreach (SECONRAC sr in rasMa)
                    {
                        SECONRAC sr2 = new SECONRAC();
                        sr2.RacId = sr.RacId;
                        if (sr.SraRate == 1)
                        {
                            //sr2.RacId = aanvullenmet;
                        }
                        sr2.SraRate = sr.SraRate;
                        rasKid.Add(sr2);
                    }
                }
                else
                {

                    foreach (SECONRAC sr in rasMa)
                    {
                        SECONRAC sr2 = new SECONRAC();
                        sr2.RacId = sr.RacId;
                        sr2.SraRate = sr.SraRate;
                        rasKid.Add(sr2);
                        //if (sr.SraRate == 1)
                        //{
                        //    //sr2.RacId = aanvullenmet;
                        //    //var temp = from n in rasKid
                        //    //           where n.RacId == aanvullenmet
                        //    //           select n;
                        //    //bool addras = true;
                        //    //if (temp.Count() > 0)
                        //    //{
                        //    //    temp.First().SraRate = temp.First().SraRate + 1;
                        //    //    addras = false;
                        //    //}
                        //    //if (addras)
                        //    //{
                        //    //    rasKid.Add(sr2);
                        //    //}
                        //}
                        //else
                        //{
                        //    sr2.SraRate = sr.SraRate;
                        //    rasKid.Add(sr2);
                        //}
                    }
                }
            }

            //pappie
            if (
                ((progid == 5 || progid == 3) && (rasPa.Count == 0))
               //    ||
               //(AniIdFather == 0)
               )
            {
                SECONRAC sr = new SECONRAC();

                sr.RacId = aanvullenmet; //overig 100%
                sr.SraRate = 8;


                var temp = from n in rasKid
                           where n.RacId == sr.RacId
                           select n;
                bool addras = true;
                if (temp.Count() == 1)
                {
                    temp.First().SraRate = temp.First().SraRate + sr.SraRate;
                    addras = false;
                }
                if (addras)
                {
                    rasKid.Add(sr);
                }
            }
            else
            {
                if (rasPa.Count() == 1)
                {
                    SECONRAC sr2 = new SECONRAC();
                    sr2.RacId = rasPa.ElementAt(0).RacId;
                    sr2.SraRate = rasPa.ElementAt(0).SraRate;
                    var temp = from n in rasKid
                               where n.RacId == sr2.RacId
                               select n;
                    if (temp.Count() > 0)
                    {
                        temp.ElementAt(0).SraRate = temp.ElementAt(0).SraRate + sr2.SraRate;
                    }
                    else
                    {
                        rasKid.Add(sr2);
                    }

                }
                else if (rasPa.Count() == 2)
                {
                    foreach (SECONRAC sr in rasPa)
                    {
                        SECONRAC sr2 = new SECONRAC();
                        sr2.RacId = sr.RacId;
                        if (sr.SraRate == 1)
                        {
                            //sr2.RacId = aanvullenmet;
                        }
                        sr2.SraRate = sr.SraRate;
                        var temp = from n in rasKid
                                   where n.RacId == sr2.RacId
                                   select n;
                        if (temp.Count() > 0)
                        {
                            temp.ElementAt(0).SraRate = temp.ElementAt(0).SraRate + sr2.SraRate;
                        }
                        else
                        {
                            rasKid.Add(sr2);
                        }
                    }

                }
                else
                {
                    foreach (SECONRAC sr in rasPa)
                    {
                        SECONRAC sr2 = new SECONRAC();
                        sr2.RacId = sr.RacId;
                        sr2.SraRate = sr.SraRate;
                        if (sr2.SraRate == 1)
                        {
                            //sr2.RacId = aanvullenmet;
                        }
                        var temp = from n in rasKid
                                   where n.RacId == sr2.RacId
                                   select n;
                        bool addras = true;
                        if (temp != null)
                        {
                            if (temp.Count() == 1)
                            {
                                temp.First().SraRate = temp.First().SraRate + sr2.SraRate;
                                addras = false;
                            }
                        }
                        if (addras)
                        {
                            rasKid.Add(sr2);
                        }
                    }
                }
            }
            /*      ----- Original Message -----
                    From: NSFO kantoor
                    To: 'Nico de Groot (V.S.M. Automatisering)'
                    Sent: Tuesday, September 02, 2014 9:00 AM
                    Subject: RE: Rasbalk NH
                    Hallo Nico,
                    Ik meende dat ik aangeven had dat NH altijd naar boven wordt afgerond.
                    Met vriendelijke groet,
                    Marjo van Bergen
             * 
             * NH = RacId 33
             */

            if (utils.isNsfo(programid))
            {

                foreach (SECONRAC srsub in rasKid)
                {
                    if (srsub.RacId == 33)
                    {
                        if (srsub.SraRate % 2 != 0)
                        {
                            srsub.SraRate += 1;
                        }
                    }
                }
            }

            rasKid = (from n in rasKid
                      where n.SraRate > 1
                      select n).ToList();

            int subtotaal = 0;

            foreach (SECONRAC srsub in rasKid)
            {
                subtotaal += srsub.SraRate;
            }
            //eventueel aanvullen met OV of ONB etc..
            if (subtotaal < 16)
            {

                SECONRAC sr = new SECONRAC();
                sr.RacId = aanvullenmet; //overig 100%
                sr.SraRate = 16 - subtotaal;

                var temp = from n in rasKid
                           where n.RacId == sr.RacId
                           select n;
                if (temp.Count() > 0)
                {
                    temp.First().SraRate = temp.First().SraRate + sr.SraRate;
                }
                else
                {
                    rasKid.Add(sr);
                }

            }

            int totaal = 0;

            int aantaloneven = (
                from n in rasKid
                where (n.SraRate % 2 != 0) && (!onbekende.Contains(n.RacId))
                select n).Count();

            rasKid = (from n in rasKid
                      orderby n.SraRate descending, n.RacId descending
                      select n).ToList();

            List<SECONRAC> rasKidCopy = getCopy(rasKid);


            int aantalkeer = 1;
            if (aantaloneven > 0) { aantalkeer += aantaloneven; }
            int verhogen = 0;
            for (int i = 0; i < aantalkeer; i++)
            {
                if (totaal != 8)
                {
                    if (totaal > 0 && totaal < 8)
                    {
                        verhogen += 1;
                        rasKid = getCopy(rasKidCopy);//OPNIEUW
                        totaal = 0;
                    }
                    else if (totaal > 8)
                    {
                        if (verhogen == 0)
                        {
                            verhogen = aantaloneven;
                        }
                        verhogen -= 1;
                        rasKid = getCopy(rasKidCopy);//OPNIEUW
                        totaal = 0;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            break;
                        }
                    }
                    int teller = 0;

                    foreach (SECONRAC sr in rasKid)
                    {

                        if (!onbekende.Contains(sr.RacId) && sr.SraRate % 2 != 0 && sr.SraRate > 1 && totaal < 8 && i > 0)
                        {
                            if (teller < verhogen)
                            {
                                sr.SraRate += 1;
                                teller += 1;
                            }
                        }

                        double tmp = sr.SraRate / 2;


                        sr.SraRate = Convert.ToInt32(Math.Truncate(tmp));
                        totaal += sr.SraRate;
                    }
                }
            }
            //degene met SraRate=0 verwijderen
            var sort = from n in rasKid
                       where n.SraRate >= 1
                       select n;
            rasKid = sort.ToList();



            //aanvullen met onbekend als het totaal<8
            if (totaal < 8)
            {

                SECONRAC sr = new SECONRAC();
                sr.RacId = aanvullenmet; //overig 100%
                sr.SraRate = 8 - totaal;

                var temp = from n in rasKid
                           where n.RacId == sr.RacId
                           select n;
                if (temp.Count() > 0)
                {
                    temp.First().SraRate = temp.First().SraRate + sr.SraRate;
                }
                else
                {
                    rasKid.Add(sr);
                }

            }
            return rasKid;
        }

        private static List<SECONRAC> getCopy(List<SECONRAC> pSeconracList)
        {
            //ivm refereneces die anders de Copy lijst ook aanpassen
            List<SECONRAC> lCopy = new List<SECONRAC>();
            foreach (SECONRAC sr in pSeconracList)
            {
                SECONRAC srn = new SECONRAC();
                srn.AniId = sr.AniId;
                srn.RacId = sr.RacId;
                srn.SraRate = sr.SraRate;
                lCopy.Add(srn);
            }
            return lCopy;
        }

        public static void setNling(int pThrId, UserRightsToken pUr, int pUBNId, int pAniIdMother, int pBirNumber)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<int> lNlingEventsIds = lMstb.getNlingCheckEventIds(pUBNId, pAniIdMother, pBirNumber);

            int newNlingnr = lNlingEventsIds.Count;

            if (newNlingnr > 0)
            {
                int newVolgnr = 1;
                foreach (int lEventBirthId in lNlingEventsIds)
                {
                    EVENT evVolgnr = lMstb.GetEventdByEventId(lEventBirthId);
                    evVolgnr.EveOrder = newVolgnr;
                    lMstb.SaveEvent(evVolgnr);
                    newVolgnr += 1;

                    BIRTH checkBirth = lMstb.GetBirth(lEventBirthId);
                    checkBirth.Nling = newNlingnr;
                    lMstb.SaveBirth(checkBirth);

                    ANIMAL lam = new ANIMAL();
                    if (checkBirth.CalfId > 0)
                    {
                        lam = lMstb.GetAnimalById(checkBirth.CalfId);
                        lam.AniNling = newNlingnr;
                        lMstb.UpdateANIMAL(pThrId, lam);
                    }
                    unLogger.WriteInfo("pBirNumber:" + pBirNumber.ToString() + "Lam:" + lam.AniLifeNumber + " Lam Nling" + lam.AniNling.ToString());
                }
            }
        }

        public static bool isDoubleNumber(string number)
        {
            double dummyDoub;
            if (Double.TryParse(number, out dummyDoub))
            { return true; }
            else
            { return false; }
        }

        public static string checkNewStatus(UserRightsToken pUr, ANIMAL pAnimal, int pNewStatus, BEDRIJF pBedr)
        {
            string ret = "";
            if (pNewStatus == 1 && pAnimal.AniStatus != 1)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);
                if (pAnimal.AniIdFather > 0 && pAnimal.AniIdMother > 0)
                {

                    ANIMAL moeder = lMstb.GetAnimalById(pAnimal.AniIdMother);
                    ANIMAL vader = lMstb.GetAnimalById(pAnimal.AniIdFather);
                    if (moeder.AniStatus != 1 && vader.AniStatus != 1)
                    {
                        ret = "Vader en moeder zijn niet volbloed. <br />Wilt u de status toch aanpassen?";
                    }
                    else if (moeder.AniStatus != 1)
                    {
                        ret = "De moeder is niet volbloed. <br />Wilt u de status toch aanpassen?";
                    }
                    else if (vader.AniStatus != 1)
                    {
                        ret = "De vader is niet volbloed. <br />Wilt u de status toch aanpassen?";
                    }
                }
                else
                {
                    if (pAnimal.AniIdMother <= 0 && pAnimal.AniIdFather <= 0)
                    {
                        ret = "De vader en moeder zijn niet bekend. <br />Wilt u de status toch aanpassen?";
                    }
                    else if (pAnimal.AniIdMother <= 0)
                    {
                        ret = "De moeder is niet bekend. <br />Wilt u de status toch aanpassen?";
                    }
                    else if (pAnimal.AniIdFather <= 0)
                    {
                        ret = "De vader is niet bekend. <br />Wilt u de status toch aanpassen?";
                    }
                }
            }
            return ret;
        }

        public static int berekenStatus(UserRightsToken pUr, int pFatherAniId, int pMotherAniId, BEDRIJF pBedr)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);

            int Status = 0;
            //BUG 1227 - 1229

            bool getstatus = true;
            if (!utils.isNsfo(pBedr.Programid))
            {
                getstatus = false;
            }
            if (utils.isBelTes(pBedr.Programid))
            {
                return CheckBeltesStatusConditions(pUr, pFatherAniId, pMotherAniId, pBedr);
            }
            //getstatus = false;
            if (getstatus)
            {
                //List<SECONRAC> secsvader = lMstb.GetSeconRacSByAnimalId(pFatherAniId);
                //List<SECONRAC> secsmoeder = lMstb.GetSeconRacSByAnimalId(pMotherAniId);
                //if (secsvader.Count > 0 && secsmoeder.Count > 0)
                //{
                //    var svader = from sv in secsvader
                //                 where sv.SraRate == 8
                //                 select sv;
                //    var smoeder = from sv in secsmoeder
                //                  where sv.SraRate == 8
                //                  select sv;
                //    if (svader.Count() == 1 && smoeder.Count() == 1)
                //    {
                //        SECONRAC sVader = svader.ElementAt(0);
                //        SECONRAC sMoeder = smoeder.ElementAt(0);
                //        if (sVader.RacId == sMoeder.RacId && sVader.SraRate == sMoeder.SraRate)
                //        {
                //            Status = 1;//Volbloed 
                //        }
                //    }
                //}
            }

            return Status;
        }

        private static int CheckBeltesStatusConditions(UserRightsToken pUr, int pFatherAniId, int pMotherAniId, BEDRIJF pBedr)
        {
            //9= Algemeen voorkomen voor BELTES 67 = soort  (op de dev is de dbwaarde soort=5 nog in de exterieurtabellen)
            //Als beide gevuld dan mag het een  "Volbloed" worden (als seconracs overeen komen) anders "Register".
            //en anders 0
            //aangepast mei 2018 zie Jira TESONLINE-31 (AGRO_LABELS  labkind=207)
            int status = 0;
            AFSavetoDB lMstb = getMysqlDb(pUr);
            int ext_soort = (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.EXTERIEUR_N_Boven;
            EXTERIEUR_WAARDEN lextF = lMstb.getLastExterieurWaarde(pFatherAniId, 9, ext_soort);
            EXTERIEUR_WAARDEN lextM = lMstb.getLastExterieurWaarde(pMotherAniId, 9, ext_soort);
            int jaarnu = DateTime.Now.Year;
            string vaderwaarde = "";
            if (lextF != null)
            {

                vaderwaarde = lextF.ExtwWaarde;

            }
            string moederwaarde = "";
            if (lextM != null)
            {

                moederwaarde = lextM.ExtwWaarde;

            }
            List<SECONRAC> secsvader = lMstb.GetSeconRacSByAnimalId(pFatherAniId);
            List<SECONRAC> secsmoeder = lMstb.GetSeconRacSByAnimalId(pMotherAniId);
            if (secsvader.Count > 0 && secsmoeder.Count > 0)
            {
                var svader = from sv in secsvader
                             where sv.SraRate == 8
                             select sv;
                var smoeder = from sv in secsmoeder
                              where sv.SraRate == 8
                              select sv;
                if (svader.Count() > 0 && smoeder.Count() > 0)
                {
                    SECONRAC sVader = svader.ElementAt(0);
                    SECONRAC sMoeder = smoeder.ElementAt(0);
                    if (sVader.RacId == sMoeder.RacId && sVader.SraRate == sMoeder.SraRate)
                    {
                        if (moederwaarde != "" && vaderwaarde != "")
                        {

                            status = 2;//Register
                            //aangepast mei 2018 //weer aangepast TESONLINE-42 
                            List<EXTERIEUR> extM = lMstb.getExterieurByAnimal(pMotherAniId);
                            if (extM.Count() > 0 && (jaarnu - extM.ElementAt(0).ExtDatum.Year) == 1)
                            {
                                if (extM.ElementAt(0).ExtKind == 1)
                                {
                                    status = 1;//Volbloed 
                                }
                            }
                            if (extM.Count() > 0 && extM.ElementAt(0).ExtKind >= 3)
                            {
                                status = 1;//Volbloed 
                            }
                            if (status == 1)
                            {
                                status = 2;//Register Vader moet ook kloppen
                                List<EXTERIEUR> extF = lMstb.getExterieurByAnimal(pFatherAniId);
                                if (extF.Count() > 0 && (jaarnu - extF.ElementAt(0).ExtDatum.Year) == 1)
                                {
                                    int[] volbloed = { 1, 3, 20 };
                                    if (volbloed.Contains(extF.ElementAt(0).ExtKind))
                                    {
                                        status = 1;//Volbloed 
                                    }
                                }
                                if (extF.Count() > 0 && (extF.ElementAt(0).ExtKind == 3 || extF.ElementAt(0).ExtKind == 4 || extF.ElementAt(0).ExtKind == 5 || extF.ElementAt(0).ExtKind == 10))
                                {
                                    status = 1;//Volbloed 
                                }
                            }
                        }
                        else
                        {
                            status = 2;//Register
                        }
                    }
                }
            }

            var vader = lMstb.GetAnimalById(pFatherAniId);
            var moeder = lMstb.GetAnimalById(pMotherAniId);
            if ((vader.AniStatus > 2 || moeder.AniStatus > 2)||(vader.AniStatus == 0 || moeder.AniStatus == 0))
            {
                status = (vader.AniStatus == 0 || moeder.AniStatus == 0)?0:vader.AniStatus > moeder.AniStatus ? vader.AniStatus : moeder.AniStatus;
            }

            return status;
        }

        public static void setSECONRACRasdelen(UserRightsToken pUr, ANIMAL pAnimal, BEDRIJF pBedrijf)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<SECONRAC> nieuwerassen = Event_functions.berekenRasDeel(pUr, pAnimal.AniIdMother, pAnimal.AniIdFather, pBedrijf.ProgId, pBedrijf.Programid);
            List<SECONRAC> huidigerasssen = lMstb.GetSeconRacSByAnimalId(pAnimal.AniId);
            foreach (SECONRAC secNu in huidigerasssen)
            {
                lMstb.DeleteSeconRace(secNu);
            }
            foreach (SECONRAC secDalijk in nieuwerassen)
            {
                secDalijk.AniId = pAnimal.AniId;
                lMstb.SaveSeconRace(secDalijk);
            }
        }

        public static bool AllowAppendingAnimal(UserRightsToken pUr, ANIMAL pDier, BEDRIJF pBedrijf, out DateTime maxafvoerdatum)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            DataTable maxafv = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), new StringBuilder("select MAX(MovDate) as MovDate from MOVEMENT where AniId = " + pDier.AniId.ToString() + " and MovKind = 2"));
            DataTable maxaanv = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), new StringBuilder("select MAX(MovDate) as MovDate from MOVEMENT where AniId = " + pDier.AniId.ToString() + " and MovKind = 1"));
            bool vraagtoevoegen = true;
            maxafvoerdatum = DateTime.MinValue;
            DateTime maxaanvoerdatum = DateTime.MinValue;
            if (maxafv.Rows.Count > 0)
            {
                if (maxafv.Rows[0][0] != DBNull.Value)
                {
                    maxafvoerdatum = getDatumFormat(maxafv.Rows[0]["MovDate"], "MovDate");
                }
            }
            if (maxaanv.Rows.Count > 0)
            {
                if (maxaanv.Rows[0][0] != DBNull.Value)
                {
                    maxaanvoerdatum = getDatumFormat(maxaanv.Rows[0]["MovDate"], "MovDate");
                }
            }
            if (maxaanvoerdatum.CompareTo(DateTime.MinValue) == 0)
            {
                //nergens een aanvoer, wel afvoer boeit dan niet
            }
            else
            {
                if (maxafvoerdatum.CompareTo(maxaanvoerdatum) < 0)
                {
                    // er is ergens een aanvoer maar geen latere of gelijke afvoer. 
                    vraagtoevoegen = false;

                }
            }
            return vraagtoevoegen;
        }

        public static string getStamboekNr(BEDRIJF pBedrijf, THIRD pThird)
        {

            string ThrStamboeknr = pBedrijf.Fokkers_Nr.Trim();
            if (ThrStamboeknr == "")
            {
                ThrStamboeknr = pThird.ThrStamboeknr.Trim();
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

        //BUG 1615
        public static string getWorkNumber(string pRfid, string pAniLifenumber)
        {
            if (pRfid == "3")
            {
                if (pAniLifenumber.Length > 5)
                {
                    return pAniLifenumber.Substring(pAniLifenumber.Length - 5, pAniLifenumber.Length - 1);
                }
                else { return pAniLifenumber; }
            }
            else { return pAniLifenumber; }
        }

        public static void getRFIDNumbers(UserRightsToken pUr, BEDRIJF pBedrijf, string rfid, string pLNVlifeNumber_AlternateNumber, ANIMAL pAniFather, int pBirthCurrentHeighestVolgNummer, out string pWorknr, out string pAniLifenumber)
        {
            //pas CORE.unAnimalactions ook aan 
            //Creeert nieuw werknummer en levensnummer voor als een dier nog 
            //niet in de database staat
            //anders retourneert  hij de bestaande
            char[] split = { ' ' };
            AFSavetoDB lMstb = getMysqlDb(pUr);
            ANIMAL aniCheck = lMstb.GetAnimalByAniAlternateNumber(pLNVlifeNumber_AlternateNumber);
            if (aniCheck.AniId == 0)
            {
                aniCheck = lMstb.GetAnimalByLifenr(pLNVlifeNumber_AlternateNumber);
            }
            UBN lUbn = lMstb.GetubnById(pBedrijf.UBNid);
            THIRD lThr = lMstb.GetThirdByThirId(lUbn.ThrID);
            string lFokkernr = getStamboekNr(pBedrijf, lThr);
            string NFSFatherLetter = "0"; //zie BUG 23
            pWorknr = "";
            pAniLifenumber = "";
            bool getLifenumber = true;
            if (aniCheck.AniId > 0)
            {
                pAniLifenumber = aniCheck.AniLifeNumber;
                ANIMALCATEGORY aniCatCheck = lMstb.GetAnimalCategoryByIdandFarmid(aniCheck.AniId, pBedrijf.FarmId);
                if (aniCatCheck.FarmId != 0)
                {
                    pWorknr = aniCatCheck.AniWorknumber.Trim();
                    if (pWorknr != "")
                    {
                        getLifenumber = false;
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
                                if (pWorknr == null)
                                {
                                    pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], true);
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            pWorknr = pAniLifenumber.Substring(lStart1, 5);
                        }
                        getLifenumber = false;
                    }
                }
            }
            if (getLifenumber)
            {
                //NSFO heeft altijd 2
                if (utils.isNsfo(pBedrijf.Programid) || pBedrijf.Programid == 49)
                { rfid = "2"; }
                if (pBedrijf.Programid == 51 || pBedrijf.Programid == 52)
                {
                    NFSFatherLetter = extractNFSFatherLetter(pAniFather.AniLifeNumber);
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
                        string pWorknrTemp = pWorknr;
                        if (rfid == "2")
                        {
                            if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                            {
                                string[] spl = pLNVlifeNumber_AlternateNumber.Split(split);
                                try
                                {
                                    pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], false);
                                    if (pWorknr == null)
                                    {
                                        pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], true);
                                    }
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
                                    if (pWorknr == null)
                                    {
                                        pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], true);
                                    }
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
                                    lHoogstevolgnr = getHoogsteVolgnr(pUr, pBedrijf, lFokkernr, lMstb.GetAnimalsByFarmId(pBedrijf.FarmId));
                                }
                                lHoogstevolgnr = lHoogstevolgnr + 1;
                                string hNumber = getHoogsteVolgnrAsString(lHoogstevolgnr, 5);
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
                            if (pBedrijf.ProgId != 3 && pBedrijf.ProgId != 5)
                            {
                                string[] spl = pLNVlifeNumber_AlternateNumber.Split(split);
                                try
                                {
                                    pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], false);
                                    if (pWorknr == null)
                                    {
                                        pWorknr = DelphiWrapper.fDiernr(pBedrijf.ProgId, spl[0], spl[1], true);
                                    }
                                }
                                catch { }
                                if (pAniLifenumber == "")
                                {
                                    pAniLifenumber = pLNVlifeNumber_AlternateNumber;
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
                        if (String.IsNullOrEmpty(pWorknr))
                        {
                            pWorknr = pWorknrTemp;
                        }
                    }
                }
            }
        }

        public static string extractNFSFatherLetter(string pLifenumber)
        {
            if (pLifenumber.Contains("-"))
            {
                int lStart = pLifenumber.IndexOf("-") + 1;
                if (pLifenumber.Length >= (lStart + 1))
                {
                    string letter = pLifenumber.Substring(lStart, 1);
                    Regex r = new Regex("^[A-Za-z]$");
                    Match m = r.Match(letter);
                    if (m.Success)
                    { return letter; }
                    else { return "0"; }
                }
                else
                {
                    return "0";
                }

            }
            else
            {
                return "0";
            }
        }

        public static int getHoogsteVolgnr(UserRightsToken userRightsToken, BEDRIJF pBedrijf, string pStamboeknr, List<ANIMAL> pAnimals)
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

        private static string getHoogsteVolgnrAsString(int pHoogstevolgnr, int pMaxLengte)
        {
            if (pMaxLengte > 5 || pMaxLengte < 1)
            {
                pMaxLengte = 5;
            }
            if (pHoogstevolgnr <= 0)
            { pHoogstevolgnr = 1; }
            string volgnr = pHoogstevolgnr.ToString();
            if (pHoogstevolgnr > 99999)
            { volgnr = "00001"; }

            while (volgnr.Length < pMaxLengte)
            {
                volgnr = "0" + volgnr;
            }

            return volgnr;
        }

        public static void handleEventTimes(ref EVENT pEvent, DateTime pInvoerdatum)
        {
            pInvoerdatum = pInvoerdatum.Date;
            DateTime tijd = pInvoerdatum;
            if (pInvoerdatum.CompareTo(DateTime.Now.Date) > 0)
            {

                tijd = tijd.AddMinutes(1);

            }
            else
            {
                tijd = tijd.AddHours(DateTime.Now.Hour);
                tijd = tijd.AddMinutes(DateTime.Now.Minute);
                tijd = tijd.AddSeconds(DateTime.Now.Second);

            }
            pEvent.EveDate = pInvoerdatum;
            pEvent.EveMutationDate = DateTime.Now;
            pEvent.EveMutationTime = DateTime.Now;

        }

        public static int saveBloedonderzoek(UserRightsToken pUr, BEDRIJF pBedr, EVENT pEvent, BLOOD pBlood)
        {
            AFSavetoDB lMstb = getMysqlDb(pUr);
            if (pEvent.EventId != 0)
            {
                if (lMstb.UpdateEVENT(pEvent))
                {
                    lMstb.SaveBlood(pBlood);
                }
                else
                {
                    //lblopmerkingen.Text = "opslaan mislukt!";
                }
            }
            else
            {

                pEvent.EveOrder = Event_functions.getNewEventOrder(pUr, pEvent.EveDate, pEvent.EveKind, pEvent.UBNId, pEvent.AniId);
                pEvent.happened_at_FarmID = pBedr.FarmId;
                if (lMstb.SaveEvent(pEvent))
                {
                    pBlood.EventId = pEvent.EventId;
                    lMstb.SaveBlood(pBlood);
                }
                else
                {
                    //lblopmerkingen.Text = "opslaan mislukt!";
                }

            }
            return pEvent.EventId;
        }

        public static int BerekenScrapie(UserRightsToken pUr, BEDRIJF pBedrijf, int scrapieM, int ScrapieV, Boolean pDeelnemerBedrijfZiekteNsfo)
        {

            if (scrapieM == 0 || ScrapieV == 0)
            {
                if (!utils.isBelTes(pBedrijf.Programid))
                {
                    return 0;
                }
            }
            /* BUG 907 scrapie = labkind 2
           */
            //int[] scrapiebedrijven = Event_functions.getStamboekenDieMeedoenMetScrapieZiekte();
            AFSavetoDB lMstb = getMysqlDb(pUr);
            //Onderste uitgezet ivm nsfo is geen klant meer en andere bedrijven zouden toch door dit functie gedeelte kunnen lopen 
            //if (pDeelnemerBedrijfZiekteNsfo || CORE.utils.isNsfo(pBedrijf.Programid))
            //{
            //    List<BEDRIJF_ZIEKTE> bs = lMstb.getCurrentBedrijfZiekteStatus(pBedrijf.FarmId);
            //    //ik neem aan dat de laatste datum alleen mee komt volgens de query
            //    //maar zal m voor de zekerheid nog sorteren
            //    if (bs != null)
            //    {
            //        var b = from d in bs
            //                where d.Bz_ZiekteID == 2
            //                orderby d.Bz_Datum descending
            //                select d;
            //        if (b.Count() > 0)
            //        {
            //            if (b.ElementAt(0).Bz_StatusID != 1)
            //            {
            //                return 0;
            //            }
            //            else
            //            {
            //                if (scrapieM == 1 && ScrapieV == 1)
            //                {
            //                    return 1;
            //                }
            //                else { return 0; }
            //            }
            //        }
            //        else
            //        {
            //            return 0;
            //        }
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //}
            List<AGRO_LABELS> list = lMstb.GetAgroLabels((int)CORE.DB.LABELSConst.labKind.ANISCRAPI, 0, pBedrijf.Programid, pBedrijf.ProgId);// getSessionLabels(lGebr, 49);// lMstb.GetLabels(49, int.Parse(utils.getLabelsLabcountrycode()));


            string v1 = string.Empty, v2 = string.Empty, m1 = string.Empty, m2 = string.Empty, s = string.Empty,
                         res1 = string.Empty, res2 = string.Empty;
            int p;

            foreach (AGRO_LABELS lab in list)
            {
                if (lab.LabID == ScrapieV)
                {
                    s = lab.LabLabel;
                    p = s.IndexOf('/');
                    if (p > 0)
                    {
                        v1 = s.Substring(0, p);// - 1
                        v2 = s.Substring(p + 1, s.Length - (p + 1));
                    }
                }

                if (lab.LabID == scrapieM)
                {
                    s = lab.LabLabel;
                    p = s.IndexOf('/');
                    if (p > 0)
                    {
                        m1 = s.Substring(0, p);
                        m2 = s.Substring(p + 1, s.Length - (p + 1));
                    }
                }
            }

            if (v1 == v2)
            { res1 = v1; }
            if (m1 == m2)
            { res2 = m1; }

            s = res1;
            if (s != string.Empty)
            { s = s + "/"; }
            s = s + res2;

            foreach (AGRO_LABELS l in list)
            {
                if (l.LabLabel == s)
                { return l.LabID; }
            }

            s = res2;
            if (s != string.Empty)
            { s = s + "/"; }
            s = s + res1;

            foreach (AGRO_LABELS l in list)
            {
                if (l.LabLabel == s)
                { return l.LabID; }
            }

            return 0;
        }

        #region NSFO Dierziekte boekhouding

        public static string getRamScrapie(UserRightsToken pUr, int pAniId)
        {
            if (pAniId > 0)
            {
                AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
                ANIMAL ram = lMstb.GetAnimalById(pAniId);

                List<AGRO_LABELS> lblsschAniScrapieucws = lMstb.GetAgroLabels(DB.LABELSConst.labKind.ANISCRAPI, 0, 0, 0);
                AGRO_LABELS tekstlbl = lblsschAniScrapieucws.Find(delegate (AGRO_LABELS p) { return p.LabID == ram.AniScrapie; });

                if (tekstlbl != null)
                {

                    return tekstlbl.LabLabel;

                }
                else { return "Onbekend"; }

            }
            else { return ""; }
        }

        public static List<string[]> checkDierenScrapies(UserRightsToken pUr, DataTable pDieren)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            //Session["DierenkiezenGeselecteerdeDieren"] wordt hier gecontroleerd op rammen met scrapie>1
            //en teruggegeven in List<string> levensnummer # scrapie
            List<AGRO_LABELS> lblsschAniScrapieucws = lMstb.GetAgroLabels(DB.LABELSConst.labKind.ANISCRAPI, 528, 0, 0);
            //LABELS tekstlbl = lblsschAniScrapieucws.Find(delegate(LABELS p) { return p.LabId == ram.AniScrapie; });
            List<string[]> scrapies = new List<string[]>();
            foreach (DataRow rw in pDieren.Rows)
            {

                if (rw["AniScrapie"] == DBNull.Value)
                {
                    string[] s = { rw["AniLifenumber"].ToString(), "Onbekend" };
                    scrapies.Add(s);
                }
                else
                {
                    if (rw["AniScrapie"].ToString() == "" || rw["AniScrapie"].ToString() == "0")
                    {
                        string[] s = { rw["AniLifenumber"].ToString(), "Onbekend" };
                        scrapies.Add(s);
                    }
                    else
                    {
                        int scr = Convert.ToInt32(rw["AniScrapie"].ToString());
                        if (scr != 1)
                        {

                            AGRO_LABELS tekstlbl = lblsschAniScrapieucws.Find(delegate (AGRO_LABELS p) { return p.LabID == scr; });
                            if (tekstlbl != null)
                            {
                                string[] s = { rw["AniLifenumber"].ToString(), tekstlbl.LabLabel };
                                scrapies.Add(s);
                            }
                            else
                            {
                                string[] s = { rw["AniLifenumber"].ToString(), "Onbekend" };
                                scrapies.Add(s);
                            }

                        }
                    }
                }

            }
            return scrapies;
        }

        //schakel hier de programids in of uit die meedoen 
        public static int[] getStamboekenDieMeedoenMetScrapieZiekteDeprecated()
        {
            //nsfo in ieder geval nu dus
            //iemand is deelnemer als zijn programID hierin zit
            //Maar ook als zijn FarmId  in de Bedrijfsziektetabel zit
            //Let wel: elke FarmId van zijn UBN 
            return new int[] { 22, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 49 };

        }


        [Obsolete]
        private static void CheckAndSaveAndMailBedrijfScrapieZiekte(int pThrId, UserRightsToken pUr, XDocument pXMailAdressen, int pRamId, DateTime pInvoerDatumTijd, BEDRIJF pBedrijf, bool pZetEigenZiek, int pDerdeUBNId, int pDerdeThrId, string pMailBeschrijvingReden, bool pTestDatabase)
        {
            if (pXMailAdressen != null)
            {
                AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);

                ANIMAL ram = lMstb.GetAnimalById(pRamId);

                if (ram.AniScrapie != 1)
                {
                    bool lSendMail = sendBedrijfsZiekteMail(pUr, pBedrijf, (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_Scrapie);
                    UBN bedrijfzelf = lMstb.GetubnById(pBedrijf.UBNid);
                    //pZetEigenZiek is meestal false want het bedrijf heeft dan waarschijnlijk al een status
                    if (pZetEigenZiek)
                    {
                        List<BEDRIJF> bests = lMstb.getBedrijvenByUBNId(bedrijfzelf.UBNid);
                        List<BEDRIJF> bestemmingen = new List<BEDRIJF>();
                        foreach (BEDRIJF be in bests)
                        {
                            if (be.ProgId == pBedrijf.ProgId)
                            {
                                bestemmingen.Add(be);
                            }
                        }

                        if (bestemmingen.Count() > 0)
                        {
                            //foreach (BEDRIJF b in bestemmingen)
                            //{
                            //    if (getStamboekenDieMeedoenMetScrapieZiekte().Contains(b.Programid))
                            //    {
                            //        if (b.ProgId == 3)
                            //        {
                            //            BEDRIJF_ZIEKTE bBest = new BEDRIJF_ZIEKTE();
                            //            bBest.Bz_Datum = pInvoerDatumTijd;
                            //            bBest.Bz_FarmID = b.FarmId;
                            //            bBest.Bz_StatusID = 3;
                            //            bBest.Bz_ZiekteID = 2;
                            //            lMstb.saveBedrijf_ZiekteBesmetting(pThrId, bBest);
                            //        }
                            //    }
                            //}
                        }
                    }
                    bool derdePartij = false;
                    bool derdePartijZiek = false;
                    if (pDerdeUBNId > 0)
                    {

                        List<BEDRIJF> bests = lMstb.getBedrijvenByUBNId(pDerdeUBNId);
                        List<BEDRIJF> bestemmingen = new List<BEDRIJF>();
                        foreach (BEDRIJF be in bests)
                        {
                            if (be.ProgId == pBedrijf.ProgId)
                            {
                                bestemmingen.Add(be);
                            }
                        }

                        if (bestemmingen.Count() > 0)
                        {
                            derdePartij = true;
                            //foreach (BEDRIJF b in bestemmingen)
                            //{
                            //    if (getStamboekenDieMeedoenMetScrapieZiekte().Contains(b.Programid))
                            //    {
                            //        if (b.ProgId == 3)
                            //        {
                            //            BEDRIJF_ZIEKTE bBest = new BEDRIJF_ZIEKTE();
                            //            bBest.Bz_Datum = pInvoerDatumTijd;
                            //            bBest.Bz_FarmID = b.FarmId;
                            //            bBest.Bz_StatusID = 3;
                            //            bBest.Bz_ZiekteID = 2;
                            //            lMstb.saveBedrijf_ZiekteBesmetting(pThrId, bBest);
                            //        }
                            //    }
                            //}
                            derdePartijZiek = true;
                        }
                    }
                    UBN ubnderde = new UBN();
                    THIRD lThderde = new THIRD();
                    StringBuilder mBld = new StringBuilder();
                    mBld.Append("<table>");
                    mBld.Append("<tr><th>Automatisch ingestelde Bedrijfsziekte statussen </th></tr>");
                    if (pZetEigenZiek)
                    {
                        unLogger.WriteDebug("De Bedrijfsziekte status van " + bedrijfzelf.Bedrijfsnaam + " UBNnummer:" + bedrijfzelf.Bedrijfsnummer + " is op 'in onderzoek' ingesteld.");
                        unLogger.WriteDebug(" De actie:" + pMailBeschrijvingReden + " is uitgevoerd met Ram Unieklevensnr:" + ram.AniAlternateNumber + " Scrapiestatus:" + getRamScrapie(pUr, ram.AniId));
                        unLogger.WriteDebug(" Actie datum:" + pInvoerDatumTijd.ToString("dddd, dd MMMM yyyy"));

                        mBld.Append("<tr><td>De Bedrijfsziekte status van " + bedrijfzelf.Bedrijfsnaam + " UBNnummer:" + bedrijfzelf.Bedrijfsnummer + " is op 'in onderzoek' ingesteld.</td></tr>");
                        mBld.Append("<tr><td> De actie:" + pMailBeschrijvingReden + " is uitgevoerd met Ram Unieklevensnr:" + ram.AniAlternateNumber + " Scrapiestatus:" + getRamScrapie(pUr, ram.AniId) + "</td></tr>");
                        mBld.Append("<tr><td> Actie datum:" + pInvoerDatumTijd.ToString("dddd, dd MMMM yyyy") + "</td></tr>");
                    }
                    else
                    {

                        unLogger.WriteDebug("Actie van: " + bedrijfzelf.Bedrijfsnaam + " UBNnummer:" + bedrijfzelf.Bedrijfsnummer);
                        unLogger.WriteDebug("Actie:" + pMailBeschrijvingReden + " is uitgevoerd met Ram Unieklevensnr:" + ram.AniAlternateNumber + " Scrapiestatus:" + getRamScrapie(pUr, ram.AniId));
                        unLogger.WriteDebug("Actie datum:" + pInvoerDatumTijd.ToString("dddd, dd MMMM yyyy"));

                        mBld.Append("<tr><td> Actie van: " + bedrijfzelf.Bedrijfsnaam + " UBNnummer:" + bedrijfzelf.Bedrijfsnummer + " </td></tr>");
                        mBld.Append("<tr><td> Actie:" + pMailBeschrijvingReden + " is uitgevoerd met Ram Unieklevensnr:" + ram.AniAlternateNumber + " Scrapiestatus:" + getRamScrapie(pUr, ram.AniId) + "</td></tr>");
                        mBld.Append("<tr><td> Actie datum:" + pInvoerDatumTijd.ToString("dddd, dd MMMM yyyy") + "</td></tr>");
                    }
                    if (derdePartij)
                    {
                        ubnderde = lMstb.GetubnById(pDerdeUBNId);
                        lThderde = lMstb.GetThirdByThirId(ubnderde.ThrID);
                        if (derdePartijZiek)
                        {
                            unLogger.WriteDebug("De Bedrijfsziekte status van " + lThderde.ThrCompanyName + " UBNnummer:" + ubnderde.Bedrijfsnummer + " is ook op 'in onderzoek' ingesteld.");
                            unLogger.WriteDebug("En is als derde partij hierbij betrokken");

                            mBld.Append("<tr><td>De Bedrijfsziekte status van " + lThderde.ThrCompanyName + " UBNnummer:" + ubnderde.Bedrijfsnummer + " is ook op 'in onderzoek' ingesteld.</td></tr>");
                            mBld.Append("<tr><td>En is als derde partij hierbij betrokken </td></tr>");
                        }
                        else
                        {
                            unLogger.WriteDebug("De Bedrijfsziekte status van de derde partij: " + lThderde.ThrCompanyName + " UBNnummer:" + ubnderde.Bedrijfsnummer + " is hierbij <u>niet</u> op 'in onderzoek' ingesteld.");
                            unLogger.WriteDebug("Het instellen daarvan is niet gelukt.");

                            mBld.Append("<tr><td>De Bedrijfsziekte status van de derde partij: " + lThderde.ThrCompanyName + " UBNnummer:" + ubnderde.Bedrijfsnummer + " is hierbij <u>niet</u> op 'in onderzoek' ingesteld.</td></tr>");
                            mBld.Append("<tr><td>Het instellen daarvan is niet gelukt. </td></tr>");
                        }
                    }
                    //MailToNSFOenPartijen(pUr, pXMailAdressen, mBld, derdePartij, derdePartijZiek, pBedrijf, pZetEigenZiek, ubnderde, lThderde, pTestDatabase, lSendMail);
                }
            }

        }

        private static bool sendBedrijfsZiekteMail(UserRightsToken pUr, BEDRIJF pBedrijf, int pZiekteId)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            BEDRIJF_ZIEKTE CurrentbZ = lMstb.getCurrentBedrijfZiekteStatus(pBedrijf.FarmId, pZiekteId);
            /*
             * BUG 1267 punt 2: als CurrentbZ==null dan wel mail want dan is niet bekend dat ie status(Onbekend) heeft.Dus is er ook nog niks gemaild
               alleen als statusID==0 dan is bekend dat het Onbekend is en dan hoeft er niet gemaild te worden.
             * Als je begrijpt wat ik bedoel.
             
             */
            bool sendmail = true;
            if (CurrentbZ != null)
            {

                if (CurrentbZ.Bz_StatusID == 0 || CurrentbZ.Bz_StatusID == 3)
                {
                    sendmail = false;
                }
            }
            return sendmail;
        }

        private static void MailToNSFOenPartijen(UserRightsToken pUr, XDocument pXMailAdressen, StringBuilder pHTMLMail, bool derdePartij, bool derdePartijZiek, BEDRIJF pBedrijf, bool pZetEigenZiek, UBN pDerdeUBN, THIRD pDerdeThr, bool pTestDatabase, bool pSendMail)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);



            UBN bedrijfzelf = lMstb.GetubnById(pBedrijf.UBNid);

            THIRD lThzelf = lMstb.GetThirdByThirId(bedrijfzelf.ThrID);
            bool pBCCleden = false;
            try
            {
                foreach (XElement companie in pXMailAdressen.Root.Nodes())
                {
                    if (companie.Attribute("name") != null)
                    {
                        if (companie.Attribute("name").Value == "nsfo")
                        {
                            foreach (XElement emaillist in companie.Nodes())
                            {
                                if (emaillist.Name == "emailbccnaarleden")
                                {
                                    if (emaillist.Value == "1")
                                    {
                                        pBCCleden = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { return; }
            pHTMLMail.Append("<table>");
            List<string> pBccLedenList = new List<string>();
            if (!pBCCleden)
            {
                pBccLedenList.Clear();
                pHTMLMail.Append("<tr><td><b><u>Automatisch Email-optie is uitgeschakeld.</u></b></td></tr>");

            }

            if (lThzelf.ThrEmail != "")
            {
                if (pBCCleden)
                {
                    pBccLedenList.Add(lThzelf.ThrEmail);
                    pHTMLMail.Append("<tr><td> Dit bericht is ook verstuurd aan:" + lThzelf.ThrCompanyName + " met emailadres:" + lThzelf.ThrEmail + "</td></tr>");

                }
                else
                {
                    pHTMLMail.Append("<tr><td> Dit bericht is <u>niet</u> verstuurd naar:" + lThzelf.ThrCompanyName + "  emailadres:" + lThzelf.ThrEmail + "</td></tr>");
                }
            }
            else
            {
                pHTMLMail.Append("<tr><td> Dit bericht is <u>niet</u> verstuurd naar:" + lThzelf.ThrCompanyName + "  emailadres:onbekend</td></tr>");

            }
            if (derdePartij)
            {
                if (pDerdeThr.ThrEmail != "")
                {
                    //Match m = regMail.Match(pDerdeThr.ThrEmail);
                    if (pBCCleden)
                    {
                        pBccLedenList.Add(pDerdeThr.ThrEmail);
                        pHTMLMail.Append("<tr><td> Dit bericht is ook verstuurd aan:" + pDerdeThr.ThrCompanyName + " met emailadres:" + pDerdeThr.ThrEmail + "</td></tr>");
                    }
                    else
                    {
                        pHTMLMail.Append("<tr><td> Dit bericht is <u>niet</u> verstuurd naar:" + pDerdeThr.ThrCompanyName + "  emailadres:" + pDerdeThr.ThrEmail + "</td></tr>");
                    }
                    //else
                    //{
                    //    pHTMLMail.Append("<tr><td> Dit bericht is <u>niet</u> verstuurd naar:" + pDerdeThr.ThrCompanyName + "  emailadres:" + pDerdeThr.ThrEmail + " is niet correct.</td></tr>");
                    //}
                }
                else
                {
                    pHTMLMail.Append("<tr><td> Dit bericht is <u>niet</u> verstuurd naar:" + pDerdeThr.ThrCompanyName + "  emailadres:onbekend</td></tr>");
                }
            }


            if (!pBCCleden)
            {
                pBccLedenList.Clear();
                //pHTMLMail.Append("<tr><td> Dit bericht is <u>uiteindelijk niet</u> verstuurd naar bovenstaande leden; Deze optie is uitgezet.</td></tr>");
                unLogger.WriteInfo("Scrapie meldingen: Geen mail naar leden.");

            }
            pHTMLMail.Append("</table>");

            string pfrom = "";
            List<string> pTO = new List<string>();
            List<string> pCC = new List<string>();
            List<string> pBCC = new List<string>();

            pBCC.AddRange(pBccLedenList);

            foreach (XElement companie in pXMailAdressen.Root.Nodes())
            {
                if (companie.Attribute("name") != null)
                {
                    if (companie.Attribute("name").Value.ToLower() == "nsfo")
                    {
                        foreach (XElement emaillist in companie.Nodes())
                        {
                            if (emaillist.Name == "emaillist")
                            {
                                if (emaillist.Attribute("groep") != null)
                                {
                                    if (emaillist.Attribute("groep").Value == "scrapiemail")
                                    {
                                        if (emaillist.Attribute("kind") != null)
                                        {
                                            if (emaillist.Attribute("kind").Value == "from")
                                            {
                                                foreach (XElement adres in emaillist.Nodes())
                                                {
                                                    pfrom = adres.Value;
                                                    break;
                                                }
                                            }
                                            if (emaillist.Attribute("kind").Value == "to")
                                            {
                                                foreach (XElement adres in emaillist.Nodes())
                                                {
                                                    if (!pTO.Contains(adres.Value))
                                                    {
                                                        pTO.Add(adres.Value);
                                                    }
                                                }
                                            }
                                            if (emaillist.Attribute("kind").Value == "cc")
                                            {
                                                foreach (XElement adres in emaillist.Nodes())
                                                {
                                                    if (!pCC.Contains(adres.Value))
                                                    {
                                                        pCC.Add(adres.Value);
                                                    }
                                                }
                                            }
                                            if (emaillist.Attribute("kind").Value == "bcc")
                                            {
                                                foreach (XElement adres in emaillist.Nodes())
                                                {
                                                    if (!pBCC.Contains(adres.Value))
                                                    {
                                                        pBCC.Add(adres.Value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (pTestDatabase)
            {
                if (pSendMail)
                {
                    Mail.sendMail(pfrom, pTO.ToArray(), pCC.ToArray(), pBCC.ToArray(), "Online bedrijfsziekte melding TestDatabase", pHTMLMail.ToString(), "", null);
                }
            }
            else
            {
                if (pSendMail)
                {
                    Mail.sendMail(pfrom, pTO.ToArray(), pCC.ToArray(), pBCC.ToArray(), "[Online bedrijfsziekte melding]", pHTMLMail.ToString(), "", null);
                }
            }

        }

        #endregion

        #region NSFO Bedrijfziekte mailen nieuw inclusief Scrapie en ZwoegerZiekte

        public enum BedrijfziekteReden { Inseminatie = 1, Samenweiden = 2, Aanvoer = 3, Groepsaanvoer = 4, Groepsgewijzedekkinginvoer = 5, toevoegendekram = 6, Geboorteinvoer = 7 };

        public enum BedrijfziekteCheck { Opslaan = 1, Waarschuwing = 2 };

        public static string checkAanvoerBedrijfZiekteStatussenByAniId(int pOpslaThrID, UserRightsToken pUr, BEDRIJF pEigenBedrijf, UBN pAanvoerUBN, BedrijfziekteCheck pCheck, BedrijfziekteReden pReden, ANIMAL pControleDier, DateTime pActieDatum, out Hashtable pZiekteStatusToChange, XDocument pXMailAdressen, bool pTestserver)
        {
            string retour = "";
            string pActiereden = "";
            string HuidigeStatus = "onbekend";
            string NieuwStatus = "onbekend";
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            pZiekteStatusToChange = new Hashtable();
            List<BEDRIJF_ZIEKTE> beigenStatussen = new List<BEDRIJF_ZIEKTE>();
            List<AGRO_LABELS> lAgroLAbels = new List<AGRO_LABELS>();
            ANIMAL dier = pControleDier;
            DateTime minDate = DateTime.Now.AddYears(-1);
            //26-maart-2013 iom Nico maximaal van 1 jaar geleden check erbij gezet
            //dit naar aanleiding van de RVO bug die er toen was 
            // Nav NSFO vergadering 18-06-2013 weer uitgeschakeld
            //if (isdeelnemerBedrijfZiekteNsfo && (minDate <= pActieDatum))

            if (pZiekteStatusToChange.Count > 0)
            {
                if (pCheck == BedrijfziekteCheck.Opslaan)
                {
                    /*
                     de pZiekteStatusToChange zijn minimaal met 1 gevuld
                     * en de pCheck is opslaan is bedoeld om opgeslagen te worden
                     */
                    bool sendmail = false;
                    if (pEigenBedrijf.ProgId == 3 || pEigenBedrijf.ProgId == 5)//alleen voor schapen en nu ook geiten indien: doet mee
                    {
                        var lblsZiektenStatussen = from b in lAgroLAbels
                                                   where b.LabKind == (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE_STATUS
                                                   select b;
                        List<KeyValuePair<int, bool>> sendmails = new List<KeyValuePair<int, bool>>();
                        foreach (int lZiekteID in pZiekteStatusToChange.Keys)
                        {
                            //int lZiekteID = 0;
                            //int.TryParse(lZiekteIDstr, out lZiekteID);
                            if (lZiekteID > 0)
                            {
                                bool change = true;
                                if (beigenStatussen.Count() > 0)
                                {


                                    //je hebt een eigen ziektestatus
                                    var bch = from bz in beigenStatussen
                                              where bz.Bz_ZiekteID == lZiekteID
                                              //&& (bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_Inonderzoek || bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_Onbekend || bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_InObservatie || bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_StatusbijGD)
                                              orderby bz.Bz_Datum descending
                                              select bz;
                                    if (bch.Count() > 0)
                                    {
                                        if (bch.ElementAt(0).Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_Inonderzoek || bch.ElementAt(0).Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_InObservatie || bch.ElementAt(0).Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_StatusbijGD)
                                        {
                                            change = false;
                                        }
                                        else
                                        {

                                            if (bch.ElementAt(0).Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_Vrij)
                                            {
                                                //if (pActieDatum.Date >= bch.ElementAt(0).Bz_Datum.Date && bch.ElementAt(0).Bz_Datum.Date < pActieDatum.Date.AddDays(8))
                                                //{
                                                if (bch.ElementAt(0).Bz_Datum.Date.AddDays(8) > DateTime.Now.Date)
                                                {
                                                    //Bij een keurings dag kwamen er 7 dagen lang mailtjes binnen
                                                    //elke dag zet nsfo de status weer op vrij
                                                    //en elke nacht wordt met inlezen de status weer op observatie gezet
                                                    change = false;
                                                }
                                                else
                                                {
                                                    KeyValuePair<int, bool> mail = new KeyValuePair<int, bool>(lZiekteID, true);
                                                    sendmails.Add(mail);
                                                }
                                            }
                                            else
                                            {
                                                KeyValuePair<int, bool> mail = new KeyValuePair<int, bool>(lZiekteID, true);
                                                sendmails.Add(mail);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //je hebt nog geen eigen ziektestatus van die ziekteID
                                        KeyValuePair<int, bool> mail = new KeyValuePair<int, bool>(lZiekteID, true);
                                        sendmails.Add(mail);
                                    }

                                }
                                else
                                {
                                    //je hebt nog geen eigen ziektestatus
                                    KeyValuePair<int, bool> mail = new KeyValuePair<int, bool>(lZiekteID, true);
                                    sendmails.Add(mail);

                                }
                                if (change)
                                {
                                    /*
                                        alleen pReden is per dier
                                        betreft het een groeps actie dan is het de bedoeling
                                        dat je dit doet met
                                        sendmailAndSaveBedrijfZiekteStatussenByGroep
                                     *  via  Hashtable pZiekteStatusToChange wordt bewaard wat je moet doen
                                    */
                                    if ((int)pReden < 4 || (int)pReden > 5)
                                    {
                                        BEDRIJF_ZIEKTE bBest = new BEDRIJF_ZIEKTE();
                                        bBest.Bz_Datum = pActieDatum;
                                        bBest.Bz_FarmID = pEigenBedrijf.FarmId;
                                        bBest.Bz_StatusID = (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_InObservatie;
                                        bBest.Bz_ZiekteID = Convert.ToSByte(lZiekteID);
                                        bBest.Bz_NSFO_Dierziekte_Geldigheid = 24;
                                        if (lMstb.saveBedrijf_ZiekteBesmetting(pOpslaThrID, bBest))
                                        {
                                            var nstatus = lblsZiektenStatussen.Where(l => l.LabID == bBest.Bz_StatusID);
                                            if (nstatus.Count() == 0)
                                            {
                                                NieuwStatus = "Onbekend";
                                            }
                                            else
                                            {

                                                NieuwStatus = nstatus.First().LabLabel;
                                            }

                                            sendmail = true;
                                        }
                                        else
                                        {
                                            string b = "fout in (plaats 1) saveBedrijf_ZiekteBesmetting:FarmId " + bBest.Bz_FarmID.ToString() +
                                                " bBest.Bz_ZiekteID : " + bBest.Bz_ZiekteID.ToString() +
                                                " bBest.Bz_Datum : " + bBest.Bz_Datum.ToString() +
                                                " pOpslaThrID:" + pOpslaThrID.ToString();
                                            unLogger.WriteError(b);
                                        }
                                    }
                                }
                            }
                        }
                        if (sendmail)
                        {
                            /*
                              alleen pReden is per dier
                              betreft het een groeps actie dan is het de bedoeling
                             * dat je dit doet met
                             * sendmailAndSaveBedrijfZiekteStatussenByGroep
                             * via  Hashtable pZiekteStatusToChange wordt bewaard wat je moet doen
                                */
                            if ((int)pReden < 4 || (int)pReden > 5)
                            {
                                var lblsZiekten = from a in lAgroLAbels
                                                  where a.LabKind == (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE
                                                  select a;
                                DateTime InvoerDatumTijd = DateTime.Now;
                                BEDRIJF b = new BEDRIJF();
                                UBN u = new UBN();
                                THIRD t = new THIRD();
                                COUNTRY land = new COUNTRY();
                                lMstb.getCompanyByFarmId(pEigenBedrijf.FarmId, out b, out u, out t, out land);
                                StringBuilder mBld = new StringBuilder();
                                mBld.Append("Dit is een geautomatiseerde email mbt : <br>");
                                mBld.Append("<table>");
                                mBld.Append("<tr><th colspan=\"3\">Bedrijfsziekte statuswijziging</th></tr>");
                                mBld.Append("<tr><td colspan=\"3\">De Bedrijfsziekte status van :</td></tr>");
                                mBld.Append("<tr><td></td><td><b> " + t.ThrCompanyName + "</b> </td><td><b>UBN : " + u.Bedrijfsnummer + "</b></td></tr>");
                                mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                                mBld.Append("<tr><td>is gewijzigd</td><td colspan=\"2\"></tr>");
                                mBld.Append("<tr><td colspan=\"3\"><hr></td></tr>");
                                mBld.Append("<tr><td colspan=\"3\">Voor onderstaande Bedrijfsziekten :</td></tr>");

                                foreach (int lZiekteID in pZiekteStatusToChange.Keys)
                                {
                                    var send = from n in sendmails
                                               where (int)n.Key == lZiekteID
                                               select n;
                                    if (send.Count() > 0)
                                    {
                                        var ziektenaam = from k in lblsZiekten
                                                         where k.LabID == lZiekteID
                                                         select k;
                                        if (ziektenaam.Count() > 0)
                                        {
                                            mBld.Append("<tr><td></td><td colspan=\"2\"><b> " + ziektenaam.ElementAt(0).LabLabel + "</b></td></tr>");
                                        }

                                        var huidig = from n in beigenStatussen
                                                     where n.Bz_ZiekteID == lZiekteID
                                                     select n;
                                        if (huidig.Count() > 0)
                                        {
                                            var huidigestatus = from n in lblsZiektenStatussen
                                                                where n.LabID == huidig.ElementAt(0).Bz_StatusID
                                                                select n;
                                            if (huidigestatus.Count() > 0)
                                            {
                                                mBld.Append("<tr><td>van: </td><td colspan=\"2\"><b>" + huidigestatus.ElementAt(0).LabLabel + "</b></td></tr>");
                                                mBld.Append("<tr><td>naar: </td><td colspan=\"2\"><b>" + NieuwStatus + "</b></td></tr>");
                                            }
                                            else
                                            {
                                                mBld.Append("<tr><td>van: </td><td colspan=\"2\"><b>" + HuidigeStatus + "</b></td></tr>");
                                                mBld.Append("<tr><td>naar: </td><td colspan=\"2\"><b>" + NieuwStatus + "</b></td></tr>");
                                            }
                                        }
                                        else
                                        {
                                            mBld.Append("<tr><td>van: </td><td colspan=\"2\"><b>" + HuidigeStatus + "</b></td></tr>");
                                            mBld.Append("<tr><td>naar: </td><td colspan=\"2\"><b>" + NieuwStatus + "</b></td></tr>");
                                        }
                                        mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                                    }
                                }


                                mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                                mBld.Append("<tr><td colspan=\"3\"> Gebeurtenissen :</td></tr>");
                                //mBld.Append("<tr><td>" + pActieDatum.ToString("dddd, dd MMMM yyyy") + "</td>");
                                mBld.Append("<tr><td> " + pActieDatum.ToString("dd-MM-yyyy") + "</td>");
                                mBld.Append("<td>" + dier.AniAlternateNumber + "</td>");
                                mBld.Append("<td>" + pActiereden + "</td>");
                                mBld.Append("</tr>");
                                mBld.Append("<tr><td colspan=\"3\"><hr></td></tr>");
                                mBld.Append("</table>");

                                MailToNSFOenPartijen(pUr, pXMailAdressen, mBld, false, false, pEigenBedrijf, true, new UBN(), new THIRD(), pTestserver, true);

                            }
                        }
                    }

                }
                else
                {
                    retour = retour + "<tr><td>Als u deze " + pActiereden + " opslaat, wordt uw bedrijfsziektestatus op 'In observatie' ingesteld.</td></tr>";
                }
            }
            if (retour.Trim() == "")
            {
                return retour;
            }
            return "<table>" + retour + "</table>";
        }

        public static void sendmailAndSaveBedrijfZiekteStatussenByGroep(int pOpslaThrID, UserRightsToken pUr, BEDRIJF pEigenBedrijf, Event_functions.BedrijfziekteReden pReden, Hashtable pZiekteStatusToChange, List<string> pLifenumbers, DateTime pActieDatum, XDocument pXMailAdressen, bool pTestserver)
        {
            string HuidigeStatus = "Onbekend";
            string NieuwStatus = "onbekend";
            if (pZiekteStatusToChange.Count > 0)
            {
                if (pReden == BedrijfziekteReden.Groepsaanvoer || pReden == BedrijfziekteReden.Groepsgewijzedekkinginvoer || pReden == BedrijfziekteReden.toevoegendekram || pReden == BedrijfziekteReden.Geboorteinvoer)
                {
                    AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
                    List<BEDRIJF_ZIEKTE> beigenStatussen = lMstb.getCurrentBedrijfZiekteStatus(pEigenBedrijf.FarmId);
                    string customercode = lMstb.GetProgramConfigValue(pEigenBedrijf.Programid, "CustomerCode");
                    string Actiereden = "";
                    switch (pReden)
                    {
                        case BedrijfziekteReden.Groepsaanvoer:
                            Actiereden = "Groepsaanvoer";
                            break;
                        case BedrijfziekteReden.Groepsgewijzedekkinginvoer:
                            Actiereden = "Groepsgewijze dekkinginvoer";
                            break;
                        case BedrijfziekteReden.toevoegendekram:
                            Actiereden = "Toevoegen van dekrammen";
                            break;
                        case BedrijfziekteReden.Geboorteinvoer:
                            Actiereden = "Invoeren geboortes";
                            break;
                        default:
                            break;
                    }
                    int[] agrolabelkinds = { (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE, (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE_STATUS, (int)DB.LABELSConst.labKind.ANISCRAPI };
                    List<AGRO_LABELS> lAgroLAbels = lMstb.GetAgroLabels(agrolabelkinds.ToList(), int.Parse(utils.getLabelsLabcountrycode()), pEigenBedrijf.Programid, pEigenBedrijf.ProgId);

                    var lblsZiekten = from a in lAgroLAbels
                                      where a.LabKind == (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE
                                      select a;
                    var lblsZiektenStatussen = from b in lAgroLAbels
                                               where b.LabKind == (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE_STATUS
                                               select b;
                    var lblsschAniScrapieucws = from c in lAgroLAbels
                                                where c.LabKind == (int)VSM.RUMA.CORE.DB.LABELSConst.labKind.ANISCRAPI
                                                select c;
                    bool sendmail = false;
                    foreach (int ziekteid in pZiekteStatusToChange.Keys)
                    {
                        bool change = true;
                        if (beigenStatussen.Count() > 0)
                        {
                            var bch = from bz in beigenStatussen
                                      where bz.Bz_ZiekteID == ziekteid
                                      select bz;
                            if (bch.Where(bz => bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_Inonderzoek ||
                                                bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_Onbekend ||
                                                bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_InObservatie ||
                                                bz.Bz_StatusID == (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_StatusbijGD).Count() > 0)
                            {
                                change = false;
                            }
                            else if (bch.Count() > 0)
                            {
                                var hstatus = lblsZiektenStatussen.Where(l => l.LabID == bch.First().Bz_StatusID);
                                if (hstatus.Count() > 0)
                                {
                                    HuidigeStatus = hstatus.First().LabLabel;
                                }
                            }
                        }

                        if (change)
                        {



                            BEDRIJF_ZIEKTE bBest = new BEDRIJF_ZIEKTE();
                            bBest.Bz_Datum = pActieDatum;
                            bBest.Bz_FarmID = pEigenBedrijf.FarmId;
                            bBest.Bz_StatusID = (int)VSM.RUMA.CORE.DB.LABELSConst.labIds.BEDRIJF_ZIEKTE_STATUS_InObservatie;
                            bBest.Bz_ZiekteID = Convert.ToSByte(ziekteid);
                            bBest.Bz_NSFO_Dierziekte_Geldigheid = 24;
                            if (lMstb.saveBedrijf_ZiekteBesmetting(pOpslaThrID, bBest))
                            {
                                sendmail = true;

                                var nstatus = lblsZiektenStatussen.Where(l => l.LabID == bBest.Bz_StatusID);
                                if (nstatus.Count() == 0)
                                {
                                    NieuwStatus = "Onbekend";
                                }
                                else
                                {

                                    NieuwStatus = nstatus.First().LabLabel;
                                }
                            }
                            else
                            {
                                string b = "fout (plaats2) in saveBedrijf_ZiekteBesmetting:FarmId " + bBest.Bz_FarmID.ToString() +
                                    " bBest.Bz_ZiekteID : " + bBest.Bz_ZiekteID.ToString() +
                                                " bBest.Bz_Datum : " + bBest.Bz_Datum.ToString() +
                                    "pOpslaThrID:" + pOpslaThrID.ToString();
                                unLogger.WriteError(b);
                            }
                        }
                    }
                    if (sendmail)
                    {


                        DateTime InvoerDatumTijd = DateTime.Now;
                        BEDRIJF b = new BEDRIJF();
                        UBN u = new UBN();
                        THIRD t = new THIRD();
                        COUNTRY land = new COUNTRY();
                        lMstb.getCompanyByFarmId(pEigenBedrijf.FarmId, out b, out u, out t, out land);
                        StringBuilder mBld = new StringBuilder();
                        mBld.Append("Dit is een geautomatiseerde email mbt : <br>");
                        mBld.Append("<table>");
                        mBld.Append("<tr><th colspan=\"3\">Bedrijfsziekte statuswijziging</th></tr>");
                        mBld.Append("<tr><td colspan=\"3\">De Bedrijfsziekte status van :</td></tr>");
                        mBld.Append("<tr><td></td><td><b> " + t.ThrCompanyName + "</b> </td><td><b>UBN : " + u.Bedrijfsnummer + "</b></td></tr>");
                        mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                        mBld.Append("<tr><td>is gewijzigd</td><td colspan=\"2\"></tr>");
                        mBld.Append("<tr><td>van: </td><td colspan=\"2\"><b>" + HuidigeStatus + "</b></td></tr>");
                        mBld.Append("<tr><td>naar: </td><td colspan=\"2\"><b>" + NieuwStatus + "</b></td></tr>");
                        mBld.Append("<tr><td colspan=\"3\"><hr></td></tr>");
                        mBld.Append("<tr><td colspan=\"3\">Voor onderstaande Bedrijfsziekten :</td></tr>");
                        foreach (int lZiekteID in pZiekteStatusToChange.Keys)
                        {
                            var ziektenaam = from k in lblsZiekten
                                             where k.LabID == lZiekteID
                                             select k;
                            if (ziektenaam.Count() > 0)
                            {
                                mBld.Append("<tr><td></td><td colspan=\"2\"><b> " + ziektenaam.ElementAt(0).LabLabel + "</b></td></tr>");
                            }
                        }
                        mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                        mBld.Append("<tr><td colspan=\"3\"> Gebeurtenissen :</td></tr>");
                        foreach (string lnummer in pLifenumbers)
                        {

                            mBld.Append("<tr><td> " + pActieDatum.ToString("dd-MM-yyyy") + "</td>");
                            mBld.Append("<td>" + lnummer + "</td>");
                            mBld.Append("<td>" + Actiereden + "</td>");
                            mBld.Append("</tr>");

                        }
                        mBld.Append("<tr><td colspan=\"3\"><hr></td></tr>");
                        mBld.Append("</table>");


                        MailToNSFOenPartijen(pUr, pXMailAdressen, mBld, false, false, pEigenBedrijf, true, new UBN(), new THIRD(), pTestserver, true);


                    }
                }
            }
        }

        private static List<sDestination> checkAanvoerBedrijfZiekteStatussen(UserRightsToken pUr, BEDRIJF pEigenBedrijf, int pAniId, List<sDestination> pHerkomsten)
        {
            //Als de return list leeg is hoeft er niets gemaild of opgeslagen te worden

            List<sDestination> retSDest = new List<sDestination>();
         
            return retSDest;

        }

        private static void SaveAndMailBedrijfZiektePerHerkomstGroep(int pThrId, UserRightsToken pUr, XDocument pXMailAdressen, DateTime pAanvoerDatumTijd, BEDRIJF pEigenBedrijf, bool pZetEigenZiek, List<sDestination> pHerkomsten, string pMailBeschrijvingReden, bool pTestDatabase, VSM.RUMA.CORE.DB.LABELSConst.labIds pZiekteConstId)
        {
            string HuidigeStatus = "Onbekend";
            string NieuwStatus = "onbekend";
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            List<AGRO_LABELS> lblsZiekten = lMstb.GetAgroLabels(VSM.RUMA.CORE.DB.LABELSConst.labKind.BEDRIJF_ZIEKTE, 528, pEigenBedrijf.Programid, pEigenBedrijf.ProgId);
            AGRO_LABELS lZiekte = lblsZiekten.Find(delegate (AGRO_LABELS p) { return p.LabID == (int)pZiekteConstId; });
            if (lZiekte != null)
            {
                if (pXMailAdressen != null)
                {
                    if (pHerkomsten.Count() > 0)//Zie checkBedrijfZiekteStatussen
                    {
                        bool lSendMail = sendBedrijfsZiekteMail(pUr, pEigenBedrijf, (int)pZiekteConstId);
                        UBN lEigenUbn = lMstb.GetubnById(pEigenBedrijf.UBNid);
                        if (pZetEigenZiek)
                        {
                            BEDRIJF_ZIEKTE bBest = new BEDRIJF_ZIEKTE();
                            bBest.Bz_Datum = pAanvoerDatumTijd;
                            bBest.Bz_FarmID = pEigenBedrijf.FarmId;
                            bBest.Bz_StatusID = 3;
                            bBest.Bz_ZiekteID = 2;
                            bBest.Bz_NSFO_Dierziekte_Geldigheid = 24;
                            if (lMstb.saveBedrijf_ZiekteBesmetting(pThrId, bBest))
                            {
                                NieuwStatus = "in onderzoek";
                            }
                            else
                            {
                                string b = " fout in (plaats 3) saveBedrijf_ZiekteBesmetting:FarmId " + bBest.Bz_FarmID.ToString() +
                                    " bBest.Bz_ZiekteID : " + bBest.Bz_ZiekteID.ToString() +
                                    " bBest.Bz_Datum : " + bBest.Bz_Datum.ToString() +
                                    " bBest.Bz_StatusID : " + bBest.Bz_StatusID.ToString() +
                                    " pOpslaThrID:" + pThrId.ToString();
                                unLogger.WriteError(b);
                            }
                        }

                        if (lSendMail)
                        {

                            StringBuilder mBld = new StringBuilder();

                            mBld.Append("Dit is een geautomatiseerde email mbt : <br>");
                            mBld.Append("<table>");
                            mBld.Append("<tr><th colspan=\"3\">Bedrijfsziekte statuswijziging</th></tr>");


                            if (pZetEigenZiek)
                            {

                                mBld.Append("<tr><td colspan=\"3\">De Bedrijfsziekte status van :</td></tr>");
                                mBld.Append("<tr><td></td><td><b> " + lEigenUbn.Bedrijfsnaam + "</b> </td><td><b>UBN : " + lEigenUbn.Bedrijfsnummer + "</b></td></tr>");
                                mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                                mBld.Append("<tr><td>is gewijzigd</td><td colspan=\"2\"></tr>");
                                mBld.Append("<tr><td>van: </td><td colspan=\"2\"><b>" + HuidigeStatus + "</b></td></tr>");
                                mBld.Append("<tr><td>naar: </td><td colspan=\"2\"><b>" + NieuwStatus + "</b></td></tr>");
                                mBld.Append("<tr><td colspan=\"3\"><hr></td></tr>");
                                mBld.Append("<tr><td colspan=\"3\">Voor onderstaande Bedrijfsziekten :</td></tr>");
                                mBld.Append("<tr><td></td><td colspan=\"2\"><b> " + lZiekte.LabLabel + "</b></td></tr>");
                                mBld.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                                mBld.Append("<tr><td colspan=\"3\"> Gebeurtenissen :</td></tr>");
                                //mBld.Append("<tr><td>" + pActieDatum.ToString("dddd, dd MMMM yyyy") + "</td>");
                                mBld.Append("<tr><td> " + pAanvoerDatumTijd.ToString("dd-MM-yyyy") + "</td>");
                                //mBld.Append("<td>" + dier.AniAlternateNumber + "</td>");
                                mBld.Append("<td>" + pMailBeschrijvingReden + "</td>");
                                mBld.Append("</tr>");
                                mBld.Append("<tr><td colspan=\"3\"><hr></td></tr>");
                                mBld.Append("</table>");



                                //mBld.Append("<tr><td>De Bedrijfsziekte status voor " + lZiekte.LabLabel + " van " + lEigenUbn.Bedrijfsnaam + " UBNnummer:" + lEigenUbn.Bedrijfsnummer + " is op 'in onderzoek' ingesteld.</td></tr>");
                                //mBld.Append("<tr><td> De actie:" + pMailBeschrijvingReden + " is uitgevoerd </td></tr>");
                                //mBld.Append("<tr><td> Actie datum:" + pAanvoerDatumTijd.ToString("dddd, dd MMMM yyyy") + "</td></tr>");

                                var herken = from n in pHerkomsten
                                             where n.UbnId > 0
                                             select n.UbnId;
                                List<int> lUbnIds = herken.ToList();
                                foreach (sDestination sHerk in pHerkomsten)
                                {

                                }

                                //mBld.Append("</table><table><tr><th>Uniek levensnummer</th><th>Scrapie</th></tr>");
                                //foreach (string[] dier in pScrapieDieren)
                                //{
                                //    mBld.Append("<tr><td>" + dier[0] + " </td><td> " + dier[1] + "</td></tr>");
                                //}
                                //mBld.Append("</table>");

                                //MailToNSFOenPartijen(pUr, pXMailAdressen, mBld, derdePartij, derdePartijZiek, pBedrijf, pZetEigenZiek, ubnderde, lThderde, pTestDatabase, lSendMail);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        public static string verwijderWorp(int pThrId, UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pMother, int pEventId, int pChangedBy, int pSourceID, bool admin)
        {
            //aflammen werpen
            //als niet meer in mutation dan niet verwijderen 
            //behalve als FARMCONFIG alIRgemeld =false;dan mag het weer wel;
            //LET OP zet m even op false en je kunt een al IRgemelde geboorte wel verwijderen 

            //probleem: er staan dubbele worpen in de database , die mogen verwijderd worden, 
            //echter opletten met MUTATION en MUTALOGS en geen LIFENR aanpassen  
            string ret = "";
            if (pEventId <= 0)
            {
                if (!admin)
                {
                    return VSM_Ruma_OnlineCulture.getStaticResource("geboortekannietverw", "Geboorte kan niet verwijderd worden. neem contact op met de Helpdesk.");
                }
            }
            DB.DBMasterQueries lMstb = new DB.DBMasterQueries(pUr);
            UBN lUbn = lMstb.GetubnById(pBedrijf.UBNid);
            THIRD lThird = lMstb.GetThirdByThirId(lUbn.ThrID);
            unLogger.WriteInfo("Verwijderen Worp UBN:" + lUbn.Bedrijfsnummer + "  EventId=" + pEventId.ToString() + " bij moeder:" + pMother.AniId.ToString() + " " + pMother.AniAlternateNumber + " ChangedBy=" + pChangedBy.ToString() + " SourceID=" + pSourceID.ToString());

            FARMCONFIG FCIRviaModem = lMstb.getFarmConfig(pBedrijf.FarmId, "IRviaModem", "true");
            EVENT lEvent = lMstb.GetEventdByEventId(pEventId);
            lEvent.Changed_By = pChangedBy;
            lEvent.SourceID = pSourceID;
            BIRTH br = lMstb.GetBirth(pEventId);
            if (br.EventId <= 0)
            {
                if (!admin)
                {
                    return VSM_Ruma_OnlineCulture.getStaticResource("geboortekannietverw", "Geboorte kan niet verwijderd worden. neem contact op met de Helpdesk.");
                }
            }
            br.Changed_By = pChangedBy;
            br.SourceID = pSourceID;
            //CONTROLEER op dubbele records 
            sbyte lBorndead = br.BornDead;
            ANIMAL calf = new ANIMAL();
            bool alIRgemeld = true;
            if (br.CalfId > 0)
            {
                calf = lMstb.GetAnimalById(br.CalfId);
            }

            if (calf.AniId > 0)
            {
                calf.Changed_By = pChangedBy;
                calf.SourceID = pSourceID;
                unLogger.WriteInfo("Verwijderen Worp UBN:" + lUbn.Bedrijfsnummer + "  EventId=" + pEventId.ToString() + " bij moeder:" + pMother.AniId.ToString() + " " + pMother.AniAlternateNumber + " Calf=" + calf.AniId.ToString() + " AniAlternatenumber=" + calf.AniAlternateNumber);

                //dubbele geboortes
                List<BIRTH> bees = lMstb.CheckBirthsByCalfId(br.CalfId);
                var check = from n in bees where n.EventId != pEventId select n;
                if (check.Count() > 0)
                {
                    MUTATION mutcheck = lMstb.GetMutationByEventId(pEventId);
                    MUTALOG mutlogcheck = lMstb.GetMutaLogByEventId(pEventId);
                    if (mutcheck.Internalnr == 0 && mutlogcheck.Internalnr == 0)
                    {
                        // dan is het niet het event waarvoor de meldingen eventueel gedaan zijn 
                        //als dit wel zo is moeten ze maar een ander dubbel event kiezen om te verwijderen
                        if (lMstb.DeleteBirth(br))
                        {
                            lMstb.DeleteEvent(lEvent);
                            return "";
                        }
                    }
                    else
                    {
                        if (mutlogcheck.Internalnr > 0)
                        {
                            /*
                               where (n.CodeMutation == 2 && n.Returnresult == 97)  
                                               || (n.CodeMutation == 102 && (n.Returnresult == 1 || n.Returnresult == 3))
                             */

                            if ((mutlogcheck.CodeMutation == 2 && mutcheck.Returnresult == 97) || (mutcheck.CodeMutation == 102 && (mutcheck.Returnresult == 1 || mutcheck.Returnresult == 3)))
                            {

                            }

                        }
                        if (mutcheck.Internalnr > 0)
                        {

                        }
                    }
                }
                int kinderen = lMstb.GetChildrenCount(calf.AniId);
                if (kinderen > 0 && calf.AniId > 0 && !admin)
                {
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        if (calf.AniSex == 1)
                        {
                            return VSM_Ruma_OnlineCulture.getStaticResource("lam", "Lam") + " " + calf.AniLifeNumber + " " + VSM_Ruma_OnlineCulture.getStaticResource("noglammeren", "heeft zelf nog lammeren, verwijder eerst deze worpen");

                        }
                        else
                        {
                            return VSM_Ruma_OnlineCulture.getStaticResource("lam", "Lam") + " " + calf.AniLifeNumber + " " + VSM_Ruma_OnlineCulture.getStaticResource("noglammeren", "heeft zelf nog lammeren, verwijder daar eerst de worpen");
                        }
                    }
                    else
                    {
                        if (calf.AniSex == 1)
                        {
                            return VSM_Ruma_OnlineCulture.getStaticResource("kalf", "Kalf") + " " + calf.AniLifeNumber + " " + VSM_Ruma_OnlineCulture.getStaticResource("nogkalveren", "heeft zelf nog kalveren, verwijder eerst deze afkalvingen");

                        }
                        else
                        {
                            return VSM_Ruma_OnlineCulture.getStaticResource("kalf", "Kalf") + " " + calf.AniLifeNumber + " " + VSM_Ruma_OnlineCulture.getStaticResource("nogkalveren", "heeft zelf nog kalveren, verwijder daar eerst de afkalvingen");
                        }
                    }
                }
                List<EVENT> gebeurtenissen = lMstb.getEventsByAniId(calf.AniId);
                if (gebeurtenissen.Count() > 0 && !admin)
                {
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        return VSM_Ruma_OnlineCulture.getStaticResource("lam", "Lam") + " " + calf.AniLifeNumber + " " + VSM_Ruma_OnlineCulture.getStaticResource("noggegevens", "heeft nog gegevens in de database, verwijder deze eerst.");
                    }
                    else
                    {
                        return VSM_Ruma_OnlineCulture.getStaticResource("kalf", "Kalf") + " " + calf.AniLifeNumber + " " + VSM_Ruma_OnlineCulture.getStaticResource("noggegevens", "heeft nog gegevens in de database, verwijder deze eerst.");
                    }
                }
            }
            else 
            {
                if (admin)
                {
                    alIRgemeld = false;
                }
                else
                {
                    if (lBorndead == 1)
                    {
                        alIRgemeld = false;
                    }
                    else
                    {
                        return VSM_Ruma_OnlineCulture.getStaticResource("geboortekannietverw", "Geboorte kan niet verwijderd worden. neem contact op met de Helpdesk.");
                    }
                }
            }
           

            int mutid = 0;
            MUTATION mCheck = lMstb.GetMutationByEventId(pEventId);
            if (mCheck.Internalnr > 0)
            {
                alIRgemeld = false;
                mutid = mCheck.Internalnr;
            }
            if (lBorndead == 1)
            {
                alIRgemeld = false;
            }

            if (FCIRviaModem.FValue.ToLower() == "false")
            {
                alIRgemeld = false;

            }
            
            if (pBedrijf.Programid == 102 || pBedrijf.Programid == 103)
            {
                alIRgemeld = false;
                if (FCIRviaModem.FValue.ToLower() == "true")
                {
                    MUTALOG lMutalogIntrekmeldingGeboorte = lMstb.GetMutaLogByEventId(pEventId);
                    if (lMutalogIntrekmeldingGeboorte.EventId > 0)
                    {
                        SOAPLOG Result = Facade.GetInstance().getMeldingen().LNV2MeldingIntrekken(lMutalogIntrekmeldingGeboorte, pBedrijf.UBNid, pBedrijf.ProgId, pBedrijf.Programid, pUr);
                        lMstb.WriteSoapError(Result);
                        unLogger.WriteDebugObject("SOAPMessage :", Result);
                        if (Result.Status == "G" || Result.Status == "W")
                        {
                            unLogger.WriteInfo("Verwijderen Worp UBN:" + lUbn.Bedrijfsnummer + "  EventId=" + pEventId.ToString() + " bij moeder:" + pMother.AniId.ToString() + " " + pMother.AniAlternateNumber + " Calf=" + calf.AniId.ToString() + " AniAlternatenumber=" + calf.AniAlternateNumber + " melding:" + Result.Internalnr.ToString());

                        }
                        else
                        {
                            unLogger.WriteInfo("Verwijderen Worp UBN:" + lUbn.Bedrijfsnummer + "  EventId=" + pEventId.ToString() + " bij moeder:" + pMother.AniId.ToString() + " " + pMother.AniAlternateNumber + " Calf=" + calf.AniId.ToString() + " AniAlternatenumber=" + calf.AniAlternateNumber + " melding niet gelukt:" + Result.Omschrijving);
                            ret = "Het intrekken van de geboortemelding is niet gelukt. U kunt dit later nogmaals proberen via: Communicatie/I&R melden/vorige I&R." + Result.Omschrijving;
                        }
                    }
                }
            }
            else
            {
                if (alIRgemeld)
                {
                    //geboortemelding ingetrokken ?? en gemeld dan ok
                    if (calf.AniId > 0)
                    {
                        List<MUTALOG> meldingen = lMstb.GetMutaLogsByLifeNumber(calf.AniAlternateNumber);
                        List<MUTALOG> ingetrokken = (from n in meldingen
                                                     where n.EventId == pEventId
                                                     select n).ToList();
                        if (ingetrokken.Count() > 0)
                        {
                            var reedsvoldaan = from n in ingetrokken
                                               where (n.CodeMutation == 2 && n.Returnresult == 97)
                                               || (n.CodeMutation == 102 && (n.Returnresult == 1 || n.Returnresult == 3))
                                               || (n.CodeMutation == 2 && n.Returnresult == 99)  // deze regel toegevoegd ivm meldingen die op andere manier zijn ingetrokken
                                               select n;
                            //een geboortemelding die is ingetrokken  OF een intrekmelding voor een geboorte die gelukt is of een waarschuwing heeft
                            if (reedsvoldaan.Count() > 0)
                            {
                                alIRgemeld = false;
                            }
                        }
                    }
                    else if(admin)
                    {
                        alIRgemeld = false;
                    }
                }
            }
            if (alIRgemeld)
            {
                // via details kijken of de melding wel bestaat
                if (lThird.ThrCountry == "151")
                {
                    Win32SOAPIRALG lDllCall = new Win32SOAPIRALG();




                    //SOAPLOG Result = Facade.GetInstance().getMeldingen().LNVIRRaadplegenMeldingenAlg(pUr, b.UBNid,
                    //            b.ProgId, b.Programid, an.AniAlternateNumber, 0, 0,
                    //            "", m.MovDate.Date, m.MovDate.Date.AddMonths(6), 1, Output);

                    String BRSnr = lThird.Thr_Brs_Number;
                    string pBrsnummer = "";
                    FTPUSER fulnvsoap = lMstb.GetFtpuser(pBedrijf.UBNid, pBedrijf.Programid, pBedrijf.ProgId, 9992, out pBrsnummer);
                    string Gebruikersnaam = fulnvsoap.UserName;
                    string Wachtwoord = fulnvsoap.Password;
                    string uBedrijfsnummer = lUbn.Bedrijfsnummer;
                    string uBRSnr = BRSnr;
                    int pMaxStrLen = 255;
                    string datumtijd = DateTime.Now.ToString("yyyyMMddhhmmss");
                    if (Gebruikersnaam == String.Empty && Wachtwoord == String.Empty)
                    {
                        Gebruikersnaam = ConfigurationManager.AppSettings["LNVDierDetailsusername"]; 
                        Wachtwoord = Facade.GetInstance().getRechten().DeCodeer_String(ConfigurationManager.AppSettings["LNVDierDetailspassword"]);
                        uBedrijfsnummer = "";
                        uBRSnr = "";
                        //TODO make 1 function
                    }
                    //////////
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

                    string pLogfile = unLogger.getLogDir("IenR") + lUbn.Bedrijfsnummer + "_LNVDierdetailsV2_" + calf.AniAlternateNumber + "_" + datumtijd + ".log";
                    string pDetailfile = unLogger.getLogDir("IenR") + lUbn.Bedrijfsnummer + "_LNVDierdetailsV2_" + calf.AniAlternateNumber + "_D_" + datumtijd + ".log";

                    //1;ubn;aanvoerdatum;afvoerdatum;adres;postcode;wooplaats;bedrijfstype

                    lDllCall.LNVDierdetailsV2(Gebruikersnaam, Wachtwoord, 0, uBedrijfsnummer,
                                 uBRSnr, calf.AniAlternateNumber, pBedrijf.ProgId, 1, 0, 0, ref LNVprognr, ref Werknummer,
                    ref Geboortedat, ref Importdat,
                    ref LandCodeHerkomst, ref LandCodeOorsprong,
                    ref Geslacht, ref Haarkleur,
                    ref Einddatum, ref RedenEinde,
                    ref LevensnrMoeder, ref VervangenLevensnr,
                    ref Status, ref Code, ref Omschrijving,
                    pDetailfile, pLogfile, pMaxStrLen);
                    if (File.Exists(pDetailfile) && (Status.ToUpper() == "G" || Status.ToUpper() == "W"))//IRD-00192
                    {
                        if (Geboortedat.Year < 1901)
                        {
                            alIRgemeld = false;
                        }
                    }
                    else if (Status == "F" && Code == "IRD-00192")
                    {
                        alIRgemeld = false;//niets gevonden
                    }
                    else
                    {
                        return Omschrijving;
                    }
                }
                else { alIRgemeld = false; }
            }
            if (!alIRgemeld)
            {

                ANIMALCATEGORY anicatcalf = new ANIMALCATEGORY();
                if (calf.AniId > 0)
                {
                    anicatcalf = lMstb.GetAnimalCategoryByIdandFarmid(calf.AniId, pBedrijf.FarmId);
                }
                anicatcalf.Changed_By = pChangedBy;
                anicatcalf.SourceID = pSourceID;
                if (mutid > 0)
                {
                    MUTATION mut = lMstb.GetMutationById(mutid);
                    unLogger.WriteInfo("Verwijder worp : verwijder MUTATION: mutid" + mutid.ToString());
                    mut.Changed_By = pChangedBy;
                    mut.SourceID = pSourceID;
                    lMstb.DeleteMutation(mut);
                }
                //Nling andere worpen weer aanpassen
                int Birnummer = br.BirNumber;
                DateTime geboortedag = lEvent.EveDate;

                lMstb.DeleteBirth(br);
                lMstb.DeleteEvent(lEvent);
                if (anicatcalf.AniId > 0)
                {
                    unLogger.WriteInfo("Verwijder worp : verwijder ANIMALCATEGORY: AniID:" + anicatcalf.AniId.ToString() + " FarmID:" + anicatcalf.FarmId.ToString());
                    lMstb.DeleteAnimalCategory(anicatcalf);
                }
                unLogger.WriteInfo("Verwijder worp : setNling: moeder.AniID:" + pMother.AniId.ToString());
                setNling(pThrId, pUr, pBedrijf.UBNid, pMother.AniId, Birnummer);

                //lifenumber weer terugplaatsen: bijv: NL 4345543543
                //in LIFENR
                if (lBorndead == 0 && calf.AniId > 0)//als wel lBorndead dan geen calf en geen lifenumber om terug te zetten
                {
                    char[] split = { ' ' };
                    string[] numbers = calf.AniLifeNumber.Split(split);
                    string nummie = "";
                    for (int i = 1; i < numbers.Length; i++)
                    {
                        nummie += numbers[i] + " ";
                    }
                    nummie = nummie.Trim();
                    string countrycode = "";
                    if (pBedrijf.ProgId == 3 || pBedrijf.ProgId == 5)
                    {
                        string altnr = calf.AniAlternateNumber;
                        //if (utils.isDoubleNumber(altnr))
                        //{
                        //    countrycode = altnr.Substring(0, 3).Trim();
                        //    nummie = altnr.Remove(0, 3).Trim();
                        //}
                        //else
                        //{
                        countrycode = altnr.Substring(0, 2).Trim();
                        nummie = altnr.Remove(0, 3).Trim();
                        //}
                    }
                    else
                    {
                        countrycode = numbers[0].Trim();
                    }

                    LIFENR lf = new LIFENR();
                    lf.FarmNumber = lUbn.Bedrijfsnummer;
                    lf.LifCountrycode = countrycode;
                    lf.LifLifenr = nummie;
                    lf.Program = pBedrijf.Programid;
                    if (lf.LifLifenr.Length > 5)
                    {
                        int start = lf.LifLifenr.Trim().Length - 5;

                        lf.LifSort = lf.LifLifenr.Trim().Substring(start, 4);
                    }
                    LIFENR effechecke = lMstb.GetLifenrByLifenr(pBedrijf.FarmId, lf.LifLifenr);
                    if (effechecke.FarmNumber == "")
                    {
                        lMstb.InsertLifenr(lf);
                    }
                    List<SECONRAC> calfRac = lMstb.GetSeconRacSByAnimalId(calf.AniId);
                    foreach (SECONRAC sr in calfRac)
                    {
                        sr.Changed_By = pChangedBy;
                        sr.SourceID = pSourceID;
                        lMstb.DeleteSeconRace(sr);
                    }
                    calf.Changed_By = pChangedBy;
                    calf.SourceID = pSourceID;
                    lMstb.DeleteAnimal(calf);

                }
                List<EVENT> geboortes = lMstb.getEventsByAniIdKind(pMother.AniId, 5);
                var geb = from n in geboortes where n.EveDate.Date == geboortedag.Date select n;
                if (geb.Count() == 0)
                {
                    //dan is er een komplete geboorte weg en moet de Birnummers van de bovenliggende met 1 verlaagd worden.
                    geb = from n in geboortes where n.EveDate.Date > geboortedag.Date select n;
                    if (geb.Count() > 0)
                    {
                        foreach (EVENT ev in geb)
                        {
                            BIRTH b = lMstb.GetBirth(ev.EventId);
                            b.BirNumber = b.BirNumber - 1;
                            b.Changed_By = pChangedBy;
                            b.SourceID = pSourceID;
                            lMstb.SaveBirth(b);
                        }
                    }
                }

            }
            else
            {
                ret = "Deze geboorte is al I&R gemeld en kan hier niet worden verwijderd. Trek de melding eerst in via:  Communicatie/I&R melden/vorige I&R.";
            }

            return ret;
        }

        public static VSM.RUMA.CORE.DB.DataTypes.DataObject FillDataObjectFromDataRow(DataRow GevuldeDataRow, VSM.RUMA.CORE.DB.DataTypes.DataObject pEmptyDataObject)
        {
            //Noooo kan voor elke row en Dataobject gebruikt worden
            PropertyInfo[] lDataProperties;
            lDataProperties = pEmptyDataObject.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                //
                if (propertyInfo.PropertyType.ToString() == "System.DateTime" || propertyInfo.PropertyType.ToString() == "DateTime" || propertyInfo.PropertyType.ToString().EndsWith("MySqlDateTime"))
                {
                    if (GevuldeDataRow.Table.Columns.Contains(propertyInfo.Name))
                    {
                        DateTime d = getDatumFormat(GevuldeDataRow[propertyInfo.Name], propertyInfo.Name);
                        propertyInfo.SetValue(pEmptyDataObject, d, null);
                    }
                    else { propertyInfo.SetValue(pEmptyDataObject, null, null); }
                }
                else
                {
                    object Value;

                    if (GevuldeDataRow.Table.Columns.Contains(propertyInfo.Name))
                    {
                        if (GevuldeDataRow[propertyInfo.Name] != DBNull.Value)
                        {
                            Value = Convert.ChangeType(GevuldeDataRow[propertyInfo.Name], propertyInfo.PropertyType);
                        }
                        else
                        {
                            Value = null;
                        }
                        propertyInfo.SetValue(pEmptyDataObject, Value, null);
                    }
                    else { propertyInfo.SetValue(pEmptyDataObject, null, null); }
                }
            }

            return pEmptyDataObject;
        }

        public static VSM.RUMA.CORE.DB.DataTypes.DataObject FillDataObjectFromDataRow(DataRowView pGevuldeDataRowView, VSM.RUMA.CORE.DB.DataTypes.DataObject pEmptyDataObject)
        {
            //Noooo kan voor elke row en Dataobject gebruikt worden
            PropertyInfo[] lDataProperties;
            lDataProperties = pEmptyDataObject.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                //
                if (propertyInfo.PropertyType.ToString() == "System.DateTime" || propertyInfo.PropertyType.ToString() == "DateTime" || propertyInfo.PropertyType.ToString().EndsWith("MySqlDateTime"))
                {
                    //DateTime d = getDatumFormat(pGevuldeDataRowView[propertyInfo.Name], propertyInfo.Name);
                    //propertyInfo.SetValue(pEmptyDataObject, d, null);
                }
                else
                {
                    object Value;
                    if (pGevuldeDataRowView[propertyInfo.Name] != DBNull.Value)
                    {
                        Value = Convert.ChangeType(pGevuldeDataRowView[propertyInfo.Name], propertyInfo.PropertyType);
                    }
                    else
                    {
                        Value = null;
                    }
                    propertyInfo.SetValue(pEmptyDataObject, Value, null);
                }
            }

            return pEmptyDataObject;
        }

        [Obsolete()]
        public static DataRow FillDataRowFromDataObject(DataRow pEmptyDataRow, VSM.RUMA.CORE.DB.DataTypes.DataObject pDataObject)
        {

            PropertyInfo[] lDataProperties;
            lDataProperties = pDataObject.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in lDataProperties)
            {
                //

                if (propertyInfo.PropertyType.ToString() == "System.DateTime" || propertyInfo.PropertyType.ToString() == "DateTime")
                {
                    //eerst datetime

                    object Valu = propertyInfo.GetValue(pDataObject, null);
                    //unLogger.WriteInfo("Om te zetten waarde = " + Valu.ToString());

                    DateTime DT = DateTime.MinValue;
                    DateTime.TryParse(Valu.ToString(), out DT);
                    //unLogger.WriteInfo("DateTime.TryParse test = " + test.ToString());
                    try
                    {
                        MysqlDateTimeConverter.SetMysqlDateTimeProperty(pEmptyDataRow, propertyInfo.Name, DT);
                    }
                    catch (Exception exc)
                    {
                        unLogger.WriteInfo("FillDataRowFromDataObject van Value naar MySqlDateTime" + exc.ToString());
                        try
                        {

                            pEmptyDataRow[propertyInfo.Name] = DT;
                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteInfo("FillDataRowFromDataObject van Value naar MySqlDateTime 2e keer" + ex.ToString());
                        }
                    }

                }
                else
                {
                    try
                    {
                        object Value = propertyInfo.GetValue(pDataObject, null);
                        pEmptyDataRow[propertyInfo.Name] = Value;
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteInfo("FillDataRowFromDataObject van Value naar Data 1" + ex.ToString());
                    }
                }


            }
            try
            {
                //bijvoorbeeld:  het object ANIMAL heeft niet de kolom ts, maar de Datarow uit ANIMAL wel.
                //Een Dbnull waarde mee teruggeven, mag niet.
                //En ook Type waarde komt niet overeen met kolomtype Verwachte type is MySqlDateTime
                //maar bij een eigengemaakte Table kan dit c# Datetime zijn
                if (pEmptyDataRow.Table.Columns.Contains("ts"))
                {
                    DateTime ccheck = getDatumFormat(pEmptyDataRow["ts"], "ts");
                    if (ccheck == DateTime.MinValue)
                    {
                        try
                        {
                            MysqlDateTimeConverter.SetMysqlDateTimeProperty(pEmptyDataRow, "ts", 2000, 1, 1, 1, 1, 1, 1);//mindate werkt niet
                        }
                        catch
                        {
                            try
                            {
                                pEmptyDataRow["ts"] = new DateTime(2000, 1, 1, 1, 1, 1);//mindate werkt niet
                            }
                            catch { }
                        }
                    }

                }
                if (pEmptyDataRow.Table.Columns.Contains("ins"))
                {
                    DateTime ccheck2 = getDatumFormat(pEmptyDataRow["ins"], "ins");
                    if (ccheck2 == DateTime.MinValue)
                    {
                        try
                        {
                            MysqlDateTimeConverter.SetMysqlDateTimeProperty(pEmptyDataRow, "ins", 2000, 1, 1, 1, 1, 1, 1);//mindate werkt niet
                        }
                        catch
                        {
                            try
                            {
                                pEmptyDataRow["ins"] = new DateTime(2000, 1, 1, 1, 1, 1);//mindate werkt niet
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            return pEmptyDataRow;
        }
 
        public static DateTime getDatumFormat(Object pColumnDateValue, string ColumnName)
        {

            DateTime returnwaarde = DateTime.MinValue;

            

            if (pColumnDateValue == null || string.IsNullOrWhiteSpace(pColumnDateValue.ToString()))
            {

                //unLogger.WriteDebug("Event_functions getDatumFormat(Object pColumnDateValue) is NULL of lege Datum betreft: " + ColumnName);

                return returnwaarde;
            }
            try
            {

                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParse(pColumnDateValue.ToString(), out dt))
                {
                    return dt;
                }
            }
            catch
            {
                
            }
            return returnwaarde;
        }

        public static string animal_getProductieGegevens(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pAnimal, DateTime pDate)
        {
            //30-03-2012 - ERIK; 
            //Gebruik MysqlSaveToDB implementatie, zodat de werking hetzelfde blijft!

            int leeftijd = 0;
            int worpNr = 0;
            int aantalLevend = 0;
            int aantalDood = 0;
            bool jaarLing = false;
            bool pFictief = false;
            if (pAnimal.AniSex < 1 || pAnimal.AniSex > 2)
            {
                return "(Geslacht onbekend)";
            }
            AFSavetoDB lMstb = getMysqlDb(pUr);
            List<int> programids = new List<int>();
            if (utils.isNsfo(pBedrijf.Programid))
            {
                programids = utils.getNsfoProgramIds();
            }
            lMstb.animal_getProductieGegevens(pAnimal.AniId, pAnimal.AniSex, pDate, programids,
                    out leeftijd, out worpNr, out aantalLevend, out aantalDood, out jaarLing, out pFictief);


            String s = String.Format("{0}/{1}/{2}/{3}", leeftijd, worpNr, aantalDood + aantalLevend, aantalLevend);
            if (jaarLing)
            { s += "*"; }
            if (pFictief)
            { s += "(f)"; }
            return s;
        }

        public static void saveDogWorpen(UserRightsToken pUr, BEDRIJF pBedrijf, ANIMAL pMother, ANIMAL pFather, DateTime pEventDate, int pTotalPuppies)
        {
            if (pTotalPuppies > 0)
            {
                if (pMother.AniId > 0)
                {
                    AFSavetoDB lMstb = getMysqlDb(pUr);
                    BEDRIJF lBedrijf = new BEDRIJF();
                    UBN lUbn = new UBN();
                    THIRD lThird = new THIRD();
                    COUNTRY lCountry = new COUNTRY();
                    lMstb.getCompanyByFarmId(pBedrijf.FarmId, out lBedrijf, out lUbn, out lThird, out lCountry);
                    int lBirNumber = getOrCreateBirnr(pUr, pMother.AniId, pEventDate);

                    for (int i = 0; i < pTotalPuppies; i++)
                    {
                        BIRTH b = new BIRTH();
                        b.AniFatherID = pFather.AniId;
                        b.BirNumber = lBirNumber;
                        b.Nling = pTotalPuppies;
                        EVENT lev = new EVENT();
                        lev.AniId = pMother.AniId;
                        handleEventTimes(ref lev, pEventDate);
                        lev.EveKind = 5;

                        lev.EveOrder = i + 1;
                        lev.happened_at_FarmID = pBedrijf.FarmId;
                        lev.ThirdId = lUbn.ThrID;
                        lev.UBNId = pBedrijf.UBNid;
                        if (lMstb.SaveEvent(lev))
                        {
                            b.EventId = lev.EventId;
                            lMstb.SaveBirth(b);
                        }
                    }
                }
            }
        }

        public static string saveNewLifenr(UserRightsToken pUr, BEDRIJF pBedrijf, THIRD pThirdBedrijf, bool pIsChipnr, string pNewLnvLifenumber)
        {
            pNewLnvLifenumber = pNewLnvLifenumber.Trim();
            string check = "";
            if (pNewLnvLifenumber != "")
            {
                LIFENR lf = new LIFENR();
                AFSavetoDB lMstb = getMysqlDb(pUr);
                check = Checker.IsValidLevensnummer(pNewLnvLifenumber, true, pBedrijf.ProgId, pBedrijf.Programid);

                if (check == "")
                {
                    UBN lUbn = new UBN();
                    if (pBedrijf.FarmId > 0)
                    {
                        lUbn = lMstb.GetubnById(pBedrijf.UBNid);
                    }
                    int pAniIdLevnrMut = 0;
                    if (!Checker.isExistingLevensnummer(pUr, pNewLnvLifenumber, pBedrijf.ProgId, out pAniIdLevnrMut))
                    {
                        List<LIFENR> LIFENRnummers = new List<LIFENR>();
                        if (pBedrijf.ProgId == 25)
                        {
                            LIFENRnummers = lMstb.GetLifenummersBy_owner_ThrID(pThirdBedrijf.ThrId);
                        }
                        else
                        {
                            if (pBedrijf.FarmId > 0)
                            {
                                LIFENRnummers = lMstb.GetLifenummersByFarmId(pBedrijf.FarmId);
                            }
                            else
                            {
                                LIFENRnummers = lMstb.GetLifenummersBy_owner_ThrID(pThirdBedrijf.ThrId);
                            }
                        }
                        bool iseralin = false;
                        foreach (LIFENR ltemp in LIFENRnummers)
                        {
                            if (pNewLnvLifenumber.EndsWith(ltemp.LifLifenr.Trim()))
                            {
                                iseralin = true;
                                break;
                            }
                        }
                        var t = from n in LIFENRnummers
                                where pNewLnvLifenumber.Contains(n.LifLifenr.Trim())
                                select n;
                        if (!iseralin)
                        {


                            lf.FarmNumber = lUbn.Bedrijfsnummer;
                            lf.owner_ThrID = pThirdBedrijf.ThrId;

                            if (pIsChipnr)
                            {
                                CHIP_STOCK cs = new CHIP_STOCK();
                                CHIP_BOX cb = new CHIP_BOX();
                                lMstb.getChipboxChipstockByChipnumber(pNewLnvLifenumber, out cb, out cs);
                                lf.Lev_ThrID = cs.cs_supplier_thrid;

                                if (pNewLnvLifenumber.Length > 3)
                                {
                                    lf.LifCountrycode = pNewLnvLifenumber.Substring(0, 3);
                                    lf.LifLifenr = pNewLnvLifenumber.Substring(3, pNewLnvLifenumber.Length - 3);
                                }
                            }
                            else
                            {
                                char[] spl = { ' ' };
                                string[] twee = pNewLnvLifenumber.Split(spl);
                                if (twee.Length == 2)
                                {

                                    lf.LifCountrycode = twee[0];
                                    lf.LifLifenr = twee[1];
                                }
                            }
                            lf.Program = pBedrijf.Programid;
                            if (lf.LifLifenr.Length > 5)
                            {
                                int start = pNewLnvLifenumber.Trim().Length - 5;
                                if (pIsChipnr)
                                {
                                    start = pNewLnvLifenumber.Length - 5;
                                    lf.LifSort = pNewLnvLifenumber.Substring(start, 4);
                                }
                                else
                                {
                                    lf.LifSort = pNewLnvLifenumber.Trim().Substring(start, 4);
                                }
                            }

                            if (lf.LifCountrycode != "" && lf.LifLifenr != "")
                            {

                                lMstb.InsertLifenr(lf);
                            }
                            else
                            {
                                check = pNewLnvLifenumber + " is niet correct.";
                            }
                        }
                        else
                        {
                            check = pNewLnvLifenumber + " zit al in de lijst";

                        }
                    }
                    else
                    {
                        check = pNewLnvLifenumber + " is al aan een dier toegekend.";

                    }

                }

            }
            return check;
        }

        public static string saveSpenen(UserRightsToken pUr, BEDRIJF pBedrijf, EVENT pEvent, WEAN pWean, ANIMAL pAnimal, bool pSaveAnyWay)
        {
            string antwoord = "";
            if (pAnimal.AniId > 0)
            {
                AFSavetoDB lMstb = getMysqlDb(pUr);

                antwoord = Checker.checkSpeendatum(pUr, pBedrijf, pAnimal, pEvent.EveDate);

                if (pSaveAnyWay || antwoord == "" || pEvent.EventId > 0)
                {
                    if (lMstb.SaveEvent(pEvent))
                    {
                        pWean.EventId = pEvent.EventId;
                        lMstb.SaveWean(pWean);
                        antwoord = "";
                    }
                }

            }
            return antwoord;
        }
    }

    public class Feedadviceer
    {
        public event EventHandler RequestUpdate;

        protected void OnRequestUpdate(object sender, MovFuncEvent e)
        {
            if (RequestUpdate != null)
                RequestUpdate(sender, e);
        }

        #region FEEDADVICE ketoproduct

        public void saveKetoBoxFeedAdvices(UserRightsToken pUr, int pAniIdMother, DateTime pWorpDate, BEDRIJF pBedrijf, int changedBy, int sourceId)
        {
            if (pBedrijf.Programid != (int)CORE.DB.LABELSConst.programId.MS_OPTIMA_BOX)
            {
                unLogger.WriteDebug($@"saveKetoBoxFeedAdvices BEDRIJF not Msoptimabox: {pBedrijf.Omschrijving}, FarmId={pBedrijf.FarmId}, programid={pBedrijf.Programid}");
                return;
            }
            unLogger.WriteDebug($@"saveKetoBoxFeedAdvices Bedrijf:{pBedrijf.Omschrijving} FarmId={pBedrijf.FarmId} AniIdMother:{pAniIdMother}, worpdate:{pWorpDate}, changedby:{changedBy} , sourceID:{sourceId}");

            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            string ketodagen = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "ketoboxdagen", "");
            string ketodagendubbel = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "ketoboxdagendubbel", "");
            string ketoml = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "ketoboxmlproduct", "");
            double kmldefault = 0.0;
            double.TryParse(lMstb.GetProgramConfigValue(pBedrijf.Programid, "ketomldefault"), out kmldefault);
            int kdagen = 0;
            int kdagendubbel = 0;
            double kml = 0.0;
            int.TryParse(ketodagen, out kdagen);
            int.TryParse(ketodagendubbel, out kdagendubbel);
            double.TryParse(ketoml, out kml);
            if (pWorpDate > DateTime.MinValue && pWorpDate.Year > 2009 && pAniIdMother > 0 && pBedrijf.FarmId > 0)
            {
                pWorpDate = pWorpDate.Date;

                string ketodagenI = lMstb.GetAnimalPropertyValue(pBedrijf.FarmId, pAniIdMother, "ketoboxdagen", "");
                string ketodagendubbelI = lMstb.GetAnimalPropertyValue(pBedrijf.FarmId, pAniIdMother, "ketoboxdagendubbel", "");
                string ketomlI = lMstb.GetAnimalPropertyValue(pBedrijf.FarmId, pAniIdMother, "ketoboxmlproduct", "");
                int kdagen2 = 0;
                int kdagendubbel2 = 0;
                double kml2 = 0;
                int.TryParse(ketodagenI, out kdagen2);
                int.TryParse(ketodagendubbelI, out kdagendubbel2);
                double.TryParse(ketomlI, out kml2);
                if (kdagen2 > 0 && kml2 > 0)
                {
                    kdagen = kdagen2;
                    kml = kml2;
                }
                if (kdagendubbel2 > 0)
                {
                    kdagendubbel = kdagendubbel2;
                }
                if (kdagen == 0)
                {
                    int.TryParse(lMstb.GetProgramConfigValue(pBedrijf.Programid, "ketodagendefault"), out kdagen);
                }


                if (kdagen >= 49)
                {
                    kml = kmldefault;

                    ANIMALCATEGORY ac = lMstb.GetAnimalCategoryByIdandFarmid(pAniIdMother, pBedrijf.FarmId);
                    ANIMAL moeder = lMstb.GetAnimalById(pAniIdMother);
                    if (ac.AniId > 0 && ac.Anicategory < 4)//Toch ?
                    {
                        StringBuilder bld = new StringBuilder();
                        bld.Append("SELECT FEED_ADVICE.*,FEED_ADVICE_DETAIL.* FROM FEED_ADVICE ");
                        bld.Append("  JOIN FEED_ADVICE_DETAIL ON FEED_ADVICE_DETAIL.FA_ID=FEED_ADVICE.FA_ID  ");
                        bld.AppendFormat(" WHERE AniID={0}  ", pAniIdMother);
                        bld.Append(" AND date_format(FA_DateTime,'%Y-%m-%d') >= '" + pWorpDate.Date.ToString("yyyy-MM-dd") + "' AND FEED_ADVICE_DETAIL.FAD_AB_Feednr=1 AND FEED_ADVICE_DETAIL.FAD_Calculation_Kind = -9 ");
                        DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), bld);

                        List<FEED_ADVICE> bestaande = new List<FEED_ADVICE>();
                        List<FEED_ADVICE_DETAIL> bestaandedetails = new List<FEED_ADVICE_DETAIL>();
                        foreach (DataRow rw in tbl.Rows)
                        {
                            FEED_ADVICE ft = new FEED_ADVICE();
                            lMstb.GetDataBase().FillObject(ft, rw);
                            bestaande.Add(ft);
                            FEED_ADVICE_DETAIL ftdtl = new FEED_ADVICE_DETAIL();
                            lMstb.GetDataBase().FillObject(ftdtl, rw);
                            bestaandedetails.Add(ftdtl);
                        }


                        int toegevoegd = 0;
                        List<FEED_ADVICE_DETAIL> fads = new List<FEED_ADVICE_DETAIL>();
                        for (int i = 0; i <= (kdagen + 1); i++)//geboortedag + aantal + 1 met 0 
                        {
                            FEED_ADVICE fa = new FEED_ADVICE();
                            FEED_ADVICE_DETAIL fad = new FEED_ADVICE_DETAIL();
                            fa.AniID = pAniIdMother;
                            fa.FA_DateTime = pWorpDate.AddDays(i);
                            fa.Changed_By = changedBy;
                            fa.SourceID = sourceId;
                            if (i <= kdagen)
                            {
                                fad.FAD_KG_Feed_Advice = kml / 1000;//ml naar Kg
                                if (i <= kdagendubbel)
                                {
                                    fad.FAD_KG_Feed_Advice = fad.FAD_KG_Feed_Advice * 2;
                                }
                            }
                            else
                            {
                                fad.FAD_KG_Feed_Advice = 0;
                            }
                            fad.FAD_Calculation_Kind = -9;
                            fad.FAD_AB_Feednr = 1;
                            fad.Changed_By = changedBy;
                            fad.SourceID = sourceId;

                            fads = new List<FEED_ADVICE_DETAIL>();
                            fads.Add(fad);
                            var bestaand = from n in bestaande where n.FA_DateTime.Date == fa.FA_DateTime.Date select n;
                            if (bestaand.Count() == 0)
                            {
                                lMstb.saveFeedAdvice(fa, fads, changedBy, sourceId);
                                toegevoegd += 1;
                            }
                            else 
                            {
                                var bestaanddetail = (from n in bestaandedetails where n.FA_ID == bestaand.ElementAt(0).FA_ID select n).ToList();
                                if (bestaanddetail.Count() > 0)
                                {
                                    if (bestaanddetail.ElementAt(0).FAD_KG_Feed_Advice != fad.FAD_KG_Feed_Advice)
                                    {
                                        bestaanddetail.ElementAt(0).FAD_KG_Feed_Advice = fad.FAD_KG_Feed_Advice;
                                        bestaanddetail.ElementAt(0).Changed_By = changedBy;
                                        bestaanddetail.ElementAt(0).SourceID = sourceId;
                                        bestaand.ElementAt(0).Changed_By = changedBy;
                                        bestaand.ElementAt(0).SourceID = sourceId;
                                        lMstb.saveFeedAdvice(bestaand.ElementAt(0), bestaanddetail, changedBy, sourceId);
                                        toegevoegd += 1;
                                    }
                                }
                            }
                        }
                        unLogger.WriteInfo("saveKetoBoxFeedAdvices: Toegevoegd:" + toegevoegd.ToString() + " Bestaande Feedadvices:" + bestaande.Count().ToString() + " datum:" + pWorpDate.ToString() + " Moeder:" + pAniIdMother.ToString() + " " + moeder.AniAlternateNumber + " Farmid:" + pBedrijf.FarmId.ToString() + " Programid:" + pBedrijf.Programid.ToString());

                    }
                    else { unLogger.WriteInfo("geen saveKetoBoxFeedAdvices: moeder niet aanwezig. Ketoboxdagen=" + kdagen.ToString() + " datum:" + pWorpDate.ToString() + " Moeder:" + pAniIdMother.ToString() + " Farmid:" + pBedrijf.FarmId.ToString() + " Programid:" + pBedrijf.Programid.ToString()); }

                }
                else { unLogger.WriteInfo("geen saveKetoBoxFeedAdvices: Ketoboxdagen<49 dagen=" + kdagen.ToString() + " datum:" + pWorpDate.ToString() + " Moeder:" + pAniIdMother.ToString() + " Farmid:" + pBedrijf.FarmId.ToString() + " Programid:" + pBedrijf.Programid.ToString()); }

            }
            else { unLogger.WriteInfo("geen saveKetoBoxFeedAdvices: datum:" + pWorpDate.ToString() + " Moeder:" + pAniIdMother.ToString() + " Farmid:" + pBedrijf.FarmId.ToString() + " Programid:" + pBedrijf.Programid.ToString()); }
        }

        public void corrigeerKetoboxFeedAdvices(UserRightsToken pUr, BEDRIJF pBedrijf, int pAniId, int changedBy, int sourceId)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);

            string ketodagen = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "ketoboxdagen", "");
            string ketoboxdagendubbel = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "ketoboxdagendubbel", "");
            string ketoml = lMstb.GetFarmConfigValue(pBedrijf.FarmId, "ketoboxmlproduct", "");

            int kdagen = 0;
            int kdagendubbel = 0;
            double kml = 0.0;
            int.TryParse(ketodagen, out kdagen);
            int.TryParse(ketoboxdagendubbel, out kdagendubbel);
            double.TryParse(ketoml, out kml);
            List<string> kets = new List<string>();
            kets.Add("ketoboxdagen");
            kets.Add("ketoboxdagendubbel");
            kets.Add("ketoboxmlproduct");
            //die hebben   eigen waardes en dienen al leeggemaakt te worden als ze toch meegenomen moeten worden
            List<ANIMAL_PROPERTY> props = lMstb.getAnimalPropertys(pBedrijf.FarmId, kets);
            List<int> AniIdsNotToChange = (from n in props where (n.AP_Key == "ketoboxdagen") select n.AniID).ToList();
            MutationUpdater m = new MutationUpdater(new XDocument(), 1, "");
            if (pAniId > 0)//bedoelt om alleen 1 individueel dier te doen
            {
                string ketodagenI = lMstb.GetAnimalPropertyValue(pBedrijf.FarmId, pAniId, "ketoboxdagen", "");
                string ketodagendubbelI = lMstb.GetAnimalPropertyValue(pBedrijf.FarmId, pAniId, "ketoboxdagendubbel", "");
                string ketomlI = lMstb.GetAnimalPropertyValue(pBedrijf.FarmId, pAniId, "ketoboxmlproduct", "");
                int kdagen2 = 0;
                int kdagendubbel2 = 0;
                double kml2 = 0;
                int.TryParse(ketodagenI, out kdagen2);
                int.TryParse(ketodagendubbelI, out kdagendubbel2);
                double.TryParse(ketomlI, out kml2);
                if (kdagen2 > 0 && kml2 > 0)
                {
                    kdagen = kdagen2;
                    kdagendubbel = kdagendubbel2;
                    kml = kml2;
                }
            }
            if (kdagen == 0)
            {
                int.TryParse(lMstb.GetProgramConfigValue(pBedrijf.Programid, "ketodagendefault"), out kdagen);
            }

            double.TryParse(lMstb.GetProgramConfigValue(pBedrijf.Programid, "ketomldefault"), out kml);


            if (kdagen >= 49)
            {
                //kml = 175; men mag nu ook dubbele porties en halve 20160713

                //Wat ze al gehad hebben blijft staan, wat er na vandaag nog komt aanpassen;
                //dit geld dan ook voor de hoeveelheid 
                //ouwe =100 nieuwe = 30  en vandaag = dag 53
                //Query  van feedavices die vandaag nog bezig zijn  
                DateTime controldate = DateTime.Now.AddDays(-kdagen);

                StringBuilder bld = new StringBuilder();
                //bld.AppendFormat(" SELECT  DISTINCT(EVENT.AniID), fa.*,fad.* FROM FEED_ADVICE fa ");
                //bld.Append(" JOIN FEED_ADVICE_DETAIL  fad ON fad.FA_ID = fa.FA_ID AND fad.FAD_AB_Feednr=1 ");
                //bld.Append(" JOIN ANIMALCATEGORY  ac ON ac.AniId = fa.AniID");
                //bld.Append(" JOIN EVENT ON EVENT.AniID = ac.AniID AND EVENT.EveKind=5  AND EVENT.EveDate=(SELECT MAX(EveDate) FROM EVENT e2 WHERE e2.AniId=ac.AniID AND e2.EveKind=5 )  ");
                //bld.AppendFormat(" WHERE ac.FarmId={0}", pBedrijf.FarmId);
                //bld.Append(" AND  ac.AniCategory < 4 ");
                //bld.Append(" AND DATE(fa.FA_DateTime) >= DATE(EVENT.EveDate)  ");
                //bld.Append(" ORDER BY fa.AniID,fa.FA_DateTime ");

                /*
                1-aug 2014 Query aangepast en code dan ook , inclusief datum vandaag en opslaan datum als Date 
                 */

                bld.Append(" SELECT  DISTINCT(ev.AniID),a.AniLifeNumber,datediff( CURRENT_DATE, ev.evedate) AS Verschil,ev.evedate ");
                //bld.Append(" , fa.*,fad.* ");
                bld.Append(" FROM EVENT ev  ");
                //bld.Append(" LEFT OUTER JOIN FEED_ADVICE fa ON fa.AniID = ev.aniid  ");
                // bld.Append(" LEFT OUTER JOIN FEED_ADVICE_DETAIL  fad ON fad.FA_ID = fa.FA_ID AND fad.FAD_AB_Feednr=1   ");
                bld.Append(" JOIN ANIMALCATEGORY  ac ON ac.AniId = ev.AniID  ");
                bld.Append(" JOIN ANIMAL  a ON a.AniId = ev.AniID  ");
                bld.AppendFormat(" WHERE ac.FarmId={0}  ", pBedrijf.FarmId);
                if (pAniId > 0)
                {
                    bld.AppendFormat(" AND ac.AniId={0}  ", pAniId);
                }
                else if (AniIdsNotToChange.Count() > 0)
                {
                    bld.Append(" AND NOT ac.AniId IN (" + lMstb.intListToString(AniIdsNotToChange) + ")  ");
                }
                bld.Append(" AND  ac.AniCategory IN (1,2,3)  ");
                bld.Append(" AND ev.EveKind=5  AND ev.EventId>0 ");
                bld.Append(" AND datediff( CURRENT_DATE, ev.evedate) < 365  ORDER BY ev.AniID ");
                //bld.Append(" ,FA_Datetime ");

                DataSet ds = new DataSet();
                DataTable tbl = lMstb.GetDataBase().QueryData(pUr.getLastChildConnection(), ds, bld, "feeds", MissingSchemaAction.Add);

                DateTime maxdate = DateTime.Now.AddYears(4);
                int teller = 1;
                int totaal = tbl.Rows.Count;

                MovFuncEvent b = new MovFuncEvent();
                b.Progress = 0;
                b.Message = tbl.Rows.Count.ToString() + " aantal corrigeren";
                if (RequestUpdate != null)
                {
                    RequestUpdate(this, b);
                }
                //List<FEED_ADVICE> advices = new List<FEED_ADVICE>();
                //List<FEED_ADVICE_DETAIL> advicedetails = new List<FEED_ADVICE_DETAIL>();
                foreach (DataRow rw in tbl.Rows)
                {

                    int procent = teller * 100 / totaal;
                    b = new MovFuncEvent();
                    if (procent == 100)
                    { procent = 99; }
                    b.Progress = procent;
                    b.Message = "&nbsp;" + rw["AniLifeNumber"].ToString();
                    if (RequestUpdate != null)
                    {
                        RequestUpdate(this, b);
                    }
                    teller += 1;

                    int lAniId = Convert.ToInt32(rw["AniID"]);
                    int Verschil = Convert.ToInt32(rw["Verschil"]);
                    DateTime begin = DateTime.Now.Date;
                    DateTime worpdate = Event_functions.getDatumFormat(rw["evedate"], "evedate bij corrigeerKetoboxFeedAdvices ");
                    if (worpdate > DateTime.MinValue && worpdate.Year >= 2016 && lAniId > 0)
                    {
                        List<FEED_ADVICE> currentadvices = new List<FEED_ADVICE>();
                        List<FEED_ADVICE_DETAIL> currentadvicedetails = new List<FEED_ADVICE_DETAIL>();
                        lMstb.getFeed_AdvicesByAniId(lAniId, begin, maxdate, out currentadvices, out currentadvicedetails);
                        m.checkForMsOptimaboxRespondernumber(pUr, pBedrijf, lMstb.GetAnimalById(lAniId), false);

                        if (worpdate.Subtract(begin).Days <= Verschil)
                        {

                            var huidigevoordier = from n in currentadvices where n.AniID == lAniId orderby n.FA_DateTime select n;
                            if (huidigevoordier.Count() == 0)//nog geen advies
                            {

                                FEED_ADVICE fa = new FEED_ADVICE();
                                for (int v = Verschil; v <= kdagen; v++)
                                {
                                    fa = new FEED_ADVICE();
                                    fa.AniID = lAniId;
                                    fa.FA_DateTime = begin;

                                    FEED_ADVICE_DETAIL fad = new FEED_ADVICE_DETAIL();
                                    List<FEED_ADVICE_DETAIL> feeddetails = new List<FEED_ADVICE_DETAIL>();

                                    if (v <= kdagen)
                                    {
                                        fad.FAD_KG_Feed_Advice = kml / 1000;//ml naar Kg
                                        if (v <= kdagendubbel)
                                        {
                                            fad.FAD_KG_Feed_Advice = fad.FAD_KG_Feed_Advice * 2;
                                        }
                                    }
                                    else
                                    {
                                        fad.FAD_KG_Feed_Advice = 0;
                                    }
                                    fad.FAD_Calculation_Kind = -9;
                                    fad.FAD_AB_Feednr = 1;
                                    feeddetails.Add(fad);
                                    if (fa.FA_DateTime.Date >= DateTime.Now.Date)
                                    {
                                        lMstb.saveFeedAdvice(fa, feeddetails, changedBy, sourceId);
                                    }
                                    begin = begin.AddDays(1);
                                }
                            }
                            else//wel advivies
                            {
                                DateTime datum = DateTime.Now.Date;
                                for (int v = Verschil; v <= (kdagen + 1); v++)
                                {
                                    //veranderen of toevoegen
                                    FEED_ADVICE fa = new FEED_ADVICE();
                                    fa.AniID = lAniId;
                                    fa.FA_DateTime = datum;
                                    FEED_ADVICE_DETAIL fadcorr = new FEED_ADVICE_DETAIL();
                                    fadcorr.FAD_Calculation_Kind = -9;
                                    fadcorr.FAD_AB_Feednr = 1;
                                    List<FEED_ADVICE_DETAIL> feeddetails = new List<FEED_ADVICE_DETAIL>();
                                    var check = from n in huidigevoordier where n.FA_DateTime.Date == datum.Date select n;
                                    if (check.Count() > 0)
                                    {
                                        fa = check.ElementAt(0);
                                        fa.FA_DateTime = datum;
                                        fadcorr = (from n in currentadvicedetails where n.FA_ID == fa.FA_ID && n.FAD_AB_Feednr == 1 select n).ToList().ElementAt(0);

                                    }
                                    if (v <= kdagen)
                                    {
                                        fadcorr.FAD_KG_Feed_Advice = kml / 1000;//ml naar Kg
                                        if (v <= kdagendubbel)
                                        {
                                            fadcorr.FAD_KG_Feed_Advice = fadcorr.FAD_KG_Feed_Advice * 2;
                                        }
                                    }
                                    else
                                    {
                                        fadcorr.FAD_KG_Feed_Advice = 0;
                                    }
                                    if (fa.FA_DateTime.Date >= DateTime.Now.Date)
                                    {
                                        feeddetails.Add(fadcorr);
                                        lMstb.saveFeedAdvice(fa, feeddetails, changedBy, sourceId);
                                    }
                                    //datum verder
                                    datum = datum.AddDays(1);
                                }
                                //datum is nu 1 dag na de laatste, de rest moet weg als die er zijn
                                if (datum.Date == DateTime.Now.Date)
                                {
                                    datum = datum.AddDays(1);
                                }

                                //verwijderen
                                List<FEED_ADVICE> verwijderen = (from n in currentadvices where n.FA_DateTime.Date >= datum.Date select n).ToList();
                                if (verwijderen.Count() > 0)
                                {
                                    for (int i = verwijderen.Count() - 1; i >= 0; i--)
                                    {



                                        List<FEED_ADVICE_DETAIL> fadrems = (from n in currentadvicedetails where n.FA_ID == verwijderen.ElementAt(i).FA_ID && n.FAD_AB_Feednr == 1 select n).ToList();

                                        FEED_ADVICE fa = verwijderen.ElementAt(i);
                                        if (fa.FA_DateTime >= DateTime.Now.Date)
                                        {
                                            if (!lMstb.deleteFeedAdvice(fa, fadrems))
                                            { unLogger.WriteError(" corrigeerFeedadvices FA_ID=" + fa.FA_ID.ToString() + " NOT Deleted"); }

                                        }
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        unLogger.WriteInfo("corrigeerKetoboxFeedAdvices EventDate Worp = null ");
                    }


                    //////////////////////////////////////////////////////////////////////////////
                    //fa = new FEED_ADVICE();

                    //if (fa.FA_ID > 0 && fa.FA_DateTime.Date >= DateTime.Now.Date)
                    //{
                    //    //advices.Add(fa);
                    //    //FEED_ADVICE_DETAIL fad = new FEED_ADVICE_DETAIL();
                    //    //lMstb.GetDataBase().FillObject(fad, rwfeed);
                    //    //advicedetails.Add(fad);
                    //}
                    //else if (fa.FA_ID == 0)
                    //{
                    //degene die nog niets hebben


                    //for (int v = Verschil; v <= kdagen; v++)
                    //{
                    //    fa = new FEED_ADVICE();
                    //    fa.AniID = lAniId;
                    //    fa.FA_DateTime = begin;

                    //    FEED_ADVICE_DETAIL fad = new FEED_ADVICE_DETAIL();
                    //    List<FEED_ADVICE_DETAIL> feeddetails = new List<FEED_ADVICE_DETAIL>();

                    //    if (v < kdagen)
                    //    {
                    //        fad.FAD_KG_Feed_Advice = kml / 1000;//ml naar Kg
                    //    }
                    //    else
                    //    {
                    //        fad.FAD_KG_Feed_Advice = 0;
                    //    }
                    //    fad.FAD_Calculation_Kind = -9;
                    //    fad.FAD_AB_Feednr = 1;
                    //    feeddetails.Add(fad);
                    //    if (fa.FA_DateTime.Date >= DateTime.Now.Date)
                    //    {
                    //        lMstb.saveFeedAdvice(fa, feeddetails);
                    //    }
                    //    begin = begin.AddDays(1);
                    //}

                    //}
                    //else 
                    //{


                    //}
                }
                List<int> AniIds = new List<int>();
                //foreach (DataRow rwfeed in tbl.Rows)
                //{
                //    int lAniId = Convert.ToInt32(rwfeed["AniID"]);
                //    if (!AniIds.Contains(lAniId))
                //    {
                //        DataRow[] foundrows = tbl.Select("AniId=" + lAniId);
                //        int Verschil = Convert.ToInt32(rwfeed["Verschil"]);
                //        DateTime lEventdate = Event_functions.getDatumFormat(rwfeed["evedate"], "evedate");
                //        DateTime begin = DateTime.Now.Date;
                //        if (lEventdate.Subtract(begin).Days <= Verschil)
                //        {
                //            var controle = from n in  advices where n.AniID == lAniId select n;
                //            if (controle.Count() == 0)
                //            {
                //                AniIds.Add(lAniId);
                //                FEED_ADVICE fa = new FEED_ADVICE();
                //                for (int v = Verschil; v <= kdagen; v++)
                //                {
                //fa = new FEED_ADVICE();
                //fa.AniID = lAniId;
                //fa.FA_DateTime = begin;

                //FEED_ADVICE_DETAIL fad = new FEED_ADVICE_DETAIL();
                //List<FEED_ADVICE_DETAIL> feeddetails = new List<FEED_ADVICE_DETAIL>();

                //if (v < kdagen)
                //{
                //    fad.FAD_KG_Feed_Advice = kml / 1000;//ml naar Kg
                //}
                //else
                //{
                //    fad.FAD_KG_Feed_Advice = 0;
                //}
                //fad.FAD_Calculation_Kind = -9;
                //fad.FAD_AB_Feednr = 1;
                //feeddetails.Add(fad);
                //if (fa.FA_DateTime.Date >= DateTime.Now.Date)
                //{
                //    lMstb.saveFeedAdvice(fa, feeddetails);
                //}
                //begin = begin.AddDays(1);
                //                }
                //            }
                //        }
                //    }
                //}
                List<int> aniids = new List<int>();// (from n in advices select n.AniID).Distinct().ToList();
                //degene die wel iets hebben
                //int teller = 1;
                //int totaal = aniids.Count();

                //MovFuncEvent b = new MovFuncEvent();
                //b.Progress = 0;
                //b.Message = aniids.ToString() + " aantal corrigeren";
                //if (RequestUpdate != null)
                //{
                //    RequestUpdate(this, b);
                //}
                //foreach (int AniId in aniids)
                //{
                //    ANIMAL a = lMstb.GetAnimalById(AniId);
                //    DataRow[] foundrows = tbl.Select("AniId=" + a.AniId);
                //    if (foundrows.Count() > 0)
                //    {
                //        int Verschil = Convert.ToInt32(foundrows[0]["Verschil"]);
                //        int procent = teller * 100 / totaal;
                //        b = new MovFuncEvent();
                //        if (procent == 100)
                //        { procent = 99; }
                //        b.Progress = procent;
                //        b.Message = "&nbsp;" + a.AniLifeNumber;
                //        if (RequestUpdate != null)
                //        {
                //            RequestUpdate(this, b);
                //        }
                //        teller += 1;
                //        List<FEED_ADVICE> faCurrent = (from n in advices where n.AniID == AniId orderby n.FA_DateTime select n).ToList();
                //        DateTime datum = DateTime.Now.Date;
                //        for (int v = Verschil; v <= kdagen; v++)
                //        {
                //            //veranderen of toevoegen
                //            FEED_ADVICE fa = new FEED_ADVICE();
                //            fa.AniID = a.AniId;
                //            fa.FA_DateTime = datum;
                //            FEED_ADVICE_DETAIL fadcorr = new FEED_ADVICE_DETAIL();
                //            fadcorr.FAD_Calculation_Kind = -9;
                //            fadcorr.FAD_AB_Feednr = 1;
                //            List<FEED_ADVICE_DETAIL> feeddetails = new List<FEED_ADVICE_DETAIL>();
                //            var check = from n in faCurrent where n.FA_DateTime.Date == datum.Date select n;
                //            if (check.Count() > 0)
                //            {
                //                fa = check.ElementAt(0);
                //                fa.FA_DateTime = datum;
                //                fadcorr = (from n in advicedetails where n.FA_ID == fa.FA_ID && n.FAD_AB_Feednr == 1 select n).ToList().ElementAt(0);

                //            }
                //            if (v < kdagen)
                //            {
                //                fadcorr.FAD_KG_Feed_Advice = kml / 1000;//ml naar Kg
                //            }
                //            else
                //            {
                //                fadcorr.FAD_KG_Feed_Advice = 0;
                //            }
                //            if (fa.FA_DateTime.Date >= DateTime.Now.Date)
                //            {
                //                feeddetails.Add(fadcorr);
                //                lMstb.saveFeedAdvice(fa, feeddetails);
                //            }
                //            //datum verder
                //            datum = datum.AddDays(1);
                //        }
                //        //datum is nu 1 dag na de laatste, de rest moet weg als die er zijn
                //        if (datum.Date == DateTime.Now.Date)
                //        {
                //            datum = datum.AddDays(1);
                //        }

                //        //verwijderen
                //        List<FEED_ADVICE> verwijderen = (from n in faCurrent where n.FA_DateTime.Date >= datum.Date select n).ToList();
                //        if (verwijderen.Count() > 0)
                //        {
                //            for (int i = verwijderen.Count() - 1; i >= 0; i--)
                //            {



                //                List<FEED_ADVICE_DETAIL> fadrems = (from n in advicedetails where n.FA_ID == verwijderen.ElementAt(i).FA_ID && n.FAD_AB_Feednr == 1 select n).ToList();

                //                FEED_ADVICE fa = verwijderen.ElementAt(i);
                //                if (!lMstb.deleteFeedAdvice(fa, fadrems))
                //                { unLogger.WriteError(" corrigeerFeedadvices FA_ID=" + fa.FA_ID.ToString() + " NOT Deleted"); }


                //            }
                //        }
                //    }
                //}
                b = new MovFuncEvent();
                b.Progress = 100;
                b.Message = " gecorrigeerd.";
                if (RequestUpdate != null)
                {
                    RequestUpdate(this, b);
                }
            }
        }

        #endregion

        public string saveRantsoenberekening(UserRightsToken pUr, BEDRIJF pBedrijf, int pAniId, int pSoortberekening, DateTime pVoerDate, double pKgAdvice, int pVoerNr, int changedBy, int sourceId)
        {
            AFSavetoDB lMstb = Facade.GetInstance().getSaveToDB(pUr);
            DBSelectQueries DBSelectQueriesObj = new DBSelectQueries(pUr);
            ANIMALCATEGORY ac = lMstb.GetAnimalCategoryByIdandFarmid(pAniId, pBedrijf.FarmId);
            if (lMstb.rdDierAanwezig(pAniId, pBedrijf.FarmId, pBedrijf.UBNid, pVoerDate, 0, ac.Ani_Mede_Eigenaar, pBedrijf.ProgId))
            {


                List<FEED_ADVICE> bestaande = new List<FEED_ADVICE>();
                List<FEED_ADVICE_DETAIL> bestaandedetails = new List<FEED_ADVICE_DETAIL>();
                lMstb.getFeed_AdvicesByAniId(pAniId, pVoerDate.Date.AddDays(-10), pVoerDate.Date.AddDays(365), out bestaande, out bestaandedetails);
                FEED_ADVICE_DETAIL fNewDetail = new FEED_ADVICE_DETAIL();
                var vandaagvoordier = from n in bestaande where n.FA_DateTime.Date == pVoerDate.Date select n;

                if (vandaagvoordier.Count() == 0)
                {
                    FEED_ADVICE vandaag = new FEED_ADVICE();
                    var gisterenvoordier = from n in bestaande where n.FA_DateTime.Date == pVoerDate.Date.AddDays(-1) select n;
                    if (gisterenvoordier.Count() == 0)
                    {
                        //gisteren ook geen voer gehad , gewoon opslaan
                        savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                    }
                    else
                    {
                        //gisteren wel voer gehad
                        var gisterendetailsdag = from n in bestaandedetails where n.FA_ID == gisterenvoordier.ElementAt(0).FA_ID && n.FAD_AB_Feednr == pVoerNr select n;
                        if (gisterendetailsdag.Count() > 0 && pSoortberekening == -9) //Rovecom , anders gewoon opslaan
                        {

                            List<FEED_STEP> steps = DBSelectQueriesObj.Feeds.getFeedrest(pBedrijf.FarmId);
                            if (steps.Count() > 0 && gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice != pKgAdvice)
                            {
                                if (gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice < pKgAdvice)
                                {
                                    var f = from n in steps where n.FS_AB_Feednr == pVoerNr && n.FS_KG_Max_Rise > 0 select n;
                                    if (f.Count() > 0 && (pKgAdvice - gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice > f.ElementAt(0).FS_KG_Max_Rise))
                                    {
                                        pKgAdvice = gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice + f.ElementAt(0).FS_KG_Max_Rise;
                                        savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                    }
                                    else
                                    {
                                        savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                    }
                                }
                                else if (gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice > pKgAdvice)
                                {
                                    var f = from n in steps where n.FS_AB_Feednr == pVoerNr && n.FS_KG_Max_Decent > 0 select n;
                                    if (f.Count() > 0 && (gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice - pKgAdvice > f.ElementAt(0).FS_KG_Max_Decent))
                                    {
                                        pKgAdvice = gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice - f.ElementAt(0).FS_KG_Max_Decent;
                                        savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                    }
                                    else
                                    {
                                        savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                    }
                                }

                            }
                            else
                            {
                                savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                            }

                        }
                        else
                        {
                            //gisteren toch geen voer gehad voor voernummer , gewoon opslaan
                            savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                        }
                    }
                }
                else
                {
                    FEED_ADVICE vandaag = vandaagvoordier.ElementAt(0);
                    var vandaagdetailsdag = from n in bestaandedetails where n.FA_ID == vandaag.FA_ID && n.FAD_AB_Feednr == pVoerNr select n;
                    if (vandaagdetailsdag.Count() == 0)
                    {
                        //vandaag heeft ie voer maar niet voor dit voernr
                        var gisterenvoordier = from n in bestaande where n.FA_DateTime.Date == pVoerDate.Date.AddDays(-1) select n;
                        if (gisterenvoordier.Count() == 0)
                        {
                            //gisteren ook geen voer gehad , gewoon opslaan
                            savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                        }
                        else
                        {
                            //gisteren wel voer gehad
                            var gisterendetailsdag = from n in bestaandedetails where n.FA_ID == gisterenvoordier.ElementAt(0).FA_ID && n.FAD_AB_Feednr == pVoerNr select n;
                            if (gisterendetailsdag.Count() > 0 && pSoortberekening == -9)//Rovecom , anders gewoon opslaan
                            {

                                List<FEED_STEP> steps = DBSelectQueriesObj.Feeds.getFeedrest(pBedrijf.FarmId);
                                if (steps.Count() > 0 && gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice != pKgAdvice)
                                {

                                    if (gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice < pKgAdvice)
                                    {
                                        var f = from n in steps where n.FS_AB_Feednr == pVoerNr && n.FS_KG_Max_Rise > 0 select n;
                                        if (f.Count() > 0 && (pKgAdvice - gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice > f.ElementAt(0).FS_KG_Max_Rise))
                                        {
                                            pKgAdvice = gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice + f.ElementAt(0).FS_KG_Max_Rise;
                                            savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                        }
                                        else
                                        {
                                            savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                        }
                                    }
                                    else if (gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice > pKgAdvice)
                                    {
                                        var f = from n in steps where n.FS_AB_Feednr == pVoerNr && n.FS_KG_Max_Decent > 0 select n;
                                        if (f.Count() > 0 && (gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice - pKgAdvice > f.ElementAt(0).FS_KG_Max_Decent))
                                        {
                                            pKgAdvice = gisterendetailsdag.ElementAt(0).FAD_KG_Feed_Advice - f.ElementAt(0).FS_KG_Max_Decent;
                                            savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                        }
                                        else
                                        {
                                            savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                        }
                                    }
                                }
                                else
                                {
                                    savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                                }

                            }
                            else
                            {
                                //gisteren toch geen voer gehad voor voernummer , gewoon opslaan
                                savefeed(lMstb, pAniId, vandaag, fNewDetail, pVoerDate, pVoerNr, pSoortberekening, pKgAdvice, changedBy, sourceId);
                            }
                        }
                    }
                }
            }
            else
            {
                return VSM_Ruma_OnlineCulture.getStaticResource("nietaanwezigop", "Dier  niet aanwezig op ") + " " + pVoerDate.ToString("dd-MM-yyyy");
            }
            return "";

        }

        private void savefeed(AFSavetoDB lMstb, int pAniId, FEED_ADVICE pFeedAdvice, FEED_ADVICE_DETAIL pFdetail, DateTime pVoerDate, int pVoerNr, int pSoortberekening, double pKgAdvice, int changedby, int sourceid)
        {

            if (pFeedAdvice.FA_ID == 0)
            {
                pFeedAdvice = new FEED_ADVICE();
            }

            if (pFdetail.FA_ID == 0)
            {
                pFdetail = new FEED_ADVICE_DETAIL();
            }
            pFeedAdvice.AniID = pAniId;
            pFeedAdvice.FA_DateTime = pVoerDate;
            pFdetail.FA_ID = pFeedAdvice.FA_ID;
            pFdetail.FAD_AB_Feednr = pVoerNr;
            pFdetail.FAD_Calculation_Kind = pSoortberekening;
            pFdetail.FAD_KG_Feed_Advice = pKgAdvice;
            List<FEED_ADVICE_DETAIL> fdtails = new List<FEED_ADVICE_DETAIL>();
            fdtails.Add(pFdetail);
            if (pKgAdvice > 0)
            {
                lMstb.saveFeedAdvice(pFeedAdvice, fdtails, changedby, sourceid);
            }
        }
    }

    [Serializable()]
    public class Worp
    {
        //tbv worpeninvoer.aspx
        public string dier { get; set; }

        public string tbhok { get; set; }
        public string staartlengte { get; set; }
        public ANIMAL lAnimal { get; set; }
        public ANIMALCATEGORY lAnimalCategory { get; set; }
        public EVENT lEvent { get; set; }
        public BIRTH lBirth { get; set; }
        public MUTATION lMutation { get; set; }
        public List<ANIMAL_AFWIJKING> lAfwijkingen { get; set; }
        public string lnrvader { get; set; }
        public string lnrmoeder { get; set; }
        public string comment { get; set; }
        public int chipperThrId { get; set; }
        public DateTime chipdatum { get; set; }
        public int teller { get; set; }
        public Worp()
        {
            dier = "";
            tbhok = "";
            staartlengte = "0";
            lAnimal = new ANIMAL();
            lAnimalCategory = new ANIMALCATEGORY();
            lEvent = new EVENT();
            lBirth = new BIRTH();
            lMutation = new MUTATION();
            lAfwijkingen = new List<ANIMAL_AFWIJKING>();
            lnrmoeder = "";
            lnrvader = "";
            comment = "";
            chipperThrId = 0;
            chipdatum = DateTime.MinValue;
        }
    }
}
