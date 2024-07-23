using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.CORE.DB.DataTypes;
using VSM.RUMA.CORE.DB.MYSQL;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE
{
    public class dog_handler
    {
        UserRightsToken _token;
        DBMasterQueries _dbmaster;
        DBSelectQueries _dbselect;
        int _changedby;
        int _sourceid;

        public enum dogpresent
        {
            BORN = 1,
            PRESENT = 2,
            NOTPRESENT = 3,
            DEAD = 4,
            UNDETERMINED = 0  
        }

        public dog_handler(UserRightsToken urt, int changedby, int sourceid)
        {
            _token = urt;
            _dbmaster = new DB.DBMasterQueries(_token);
            _dbselect = Facade.GetInstance().getSlave(_token);
            _changedby = changedby;
            _sourceid = sourceid;
        }

        public dogpresent checkdogpresent(ANIMAL dog, THIRD tbv_ThrId, int CodeMutation, DateTime datum)
        {

            //Doel: verhinderen dubbele meldingen
            dogpresent pres = dogpresent.UNDETERMINED;
            List<MUTALOG> l = _dbmaster.GetMutaLogsByLifeNumber(dog.AniLifeNumber);
            if (CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.DOOD || CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.GEBOORTE)
            {
                if (l.Any(x => x.CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.DOOD && x.Report != 2))
                {
                    pres = dogpresent.DEAD;
                }
                else if (l.Any(x => x.CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.GEBOORTE && x.Report != 2))
                {
                    pres = dogpresent.BORN;
                }
            }
            if (pres == dogpresent.UNDETERMINED && tbv_ThrId.ThrId > 0)
            {
                var meldingen = from n in l
                                where n.tbv_ThrID == tbv_ThrId.ThrId
                                && (n.CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.AANVOER
                                || n.CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.IMPORT
                                || n.CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.AFVOER
                                || n.CodeMutation == (int)CORE.DB.LABELSConst.CodeMutation.EXPORT
                                )
                                && n.Report != 2
                                && n.MutationDate.Date <= datum.Date
                                orderby n.MutationDate descending, n.MutationTime descending
                                select n;
                if (meldingen.Count() > 0)
                {
                    switch (meldingen.ElementAt(0).CodeMutation)
                    {
                        case (int)CORE.DB.LABELSConst.CodeMutation.AANVOER:
                        case (int)CORE.DB.LABELSConst.CodeMutation.IMPORT:
                            pres = dogpresent.PRESENT;
                            break;
                        case (int)CORE.DB.LABELSConst.CodeMutation.AFVOER:
                        case (int)CORE.DB.LABELSConst.CodeMutation.EXPORT:
                            pres = dogpresent.NOTPRESENT;
                            break;
                    }
                }
            }
            else
            {
                unLogger.WriteWarn($@"checkdogpresent melding {dog.AniLifeNumber} eigenaar niet bekend");
            }
            return pres;
        }

        public string saveMutationHondThird(UBN pUbnWill, THIRD pGebruiker, ANIMAL pAnimal, THIRD pTbv_Third, MOVEMENT pMov, EVENT pEve, int pCodeMutation, out MUTATION pMutation, bool pHouderTbv_Meldzelf)
        {
            string ret = "";
           
            pMutation = new MUTATION();
            pMutation.SendTo = 35;

            if (pMov.MovId > 0)
            {
                pMutation = _dbmaster.GetMutationByMovId(pMov.MovId);

                // kijken of het een herstelmelding is.
                if (pMutation.Internalnr == 0)
                {
                    MUTALOG mutlog = _dbmaster.GetMutaLogByMovId(pMov.MovId);
                    if (mutlog.Internalnr != 0 && (mutlog.Returnresult == 1 || mutlog.Returnresult == 3))
                    {
                        pMutation.MeldingNummer = mutlog.MeldingNummer;
                        pMutation.Returnresult = 96;
                        pMutation.CodeMutation += 100;
                    }
                }
                pMutation.MutationDate = pMov.MovDate;
                pMutation.MutationTime = pMov.MovTime;
                pMutation.UbnId = pMov.UbnId;
            }
            else if (pEve.EventId > 0)
            {
                pMutation = _dbmaster.GetMutationByEventId(pEve.EventId);

                // kijken of het een herstelmelding is.
                if (pMutation.Internalnr == 0)
                {
                    MUTALOG mutlog = _dbmaster.GetMutaLogByMovId(pEve.EventId);
                    if (mutlog.Internalnr != 0 && (mutlog.Returnresult == 1 || mutlog.Returnresult == 3))
                    {
                        pMutation.MeldingNummer = mutlog.MeldingNummer;
                        pMutation.Returnresult = 96;
                        pMutation.CodeMutation += 100;
                    }
                }
                pMutation.MutationDate = pEve.EveDate;
                pMutation.MutationTime = pEve.EveDate;
                pMutation.UbnId = pEve.UBNId;
            }

            if (pEve.EventId == 0 && pMov.MovId == 0)
            {
                if (pEve.EveDate > DateTime.MinValue)
                {
                    pMutation.MutationDate = pEve.EveDate;
                    pMutation.MutationTime = pEve.EveDate;
                }
                else if (pMov.MovDate > DateTime.MinValue)
                {
                    pMutation.MutationDate = pMov.MovDate;
                    pMutation.MutationTime = pMov.MovTime;
                }
            }
            if (pMutation.MutationDate == DateTime.MinValue)
            {
                if (pCodeMutation == 2)
                {
                    pMutation.MutationDate = pAnimal.AniBirthDate;
                    pMutation.MutationTime = pAnimal.AniBirthDate;
                }
                else
                {
                    return "De datum is niet ingevuld";
                }
            }

            pMutation.Lifenumber = pAnimal.AniLifeNumber;
            pMutation.CountryCodeDepart = pAnimal.AniCountryCodeDepart;
            if (pMov.MovKind == 1)
            {
                pMutation.CodeMutation = 1;//aanvoer
            }
            if (pMov.MovKind == 2)
            {
                pMutation.CodeMutation = 4;//afvoer
            }
            if (pMov.MovKind == 3)
            {
                pMutation.CodeMutation = 6;//dood
            }
            if (pMov.MovKind == 9)
            {
                pMutation.CodeMutation = 20;//Vermist
            }
            if (pMov.MovKind == 10)
            {
                pMutation.CodeMutation = 21;//Gevonden
            }
            if (pMov.MovKind == 11)
            {
                pMutation.CodeMutation = 23;//Gevonden
            }
            if (pEve.EveKind == 14)
            {
                pMutation.CodeMutation = 22;//Contactmelding
            }
            if (pEve.EveKind == 15)
            {
                pMutation.CodeMutation = 19;//Omnummeren Chippen
                //Bij de eerste keer is het een geboortemelding
            }

            if (pCodeMutation > 0)
            {
                pMutation.CodeMutation = pCodeMutation;
            }

            pMutation.ThrID = pGebruiker.ThrId;
            if (pHouderTbv_Meldzelf)
            {
                pMutation.ThrID = pTbv_Third.ThrId;
            }
            pMutation.tbv_ThrID = pTbv_Third.ThrId;


            pMutation.UbnId = pUbnWill.UBNid;
            pMutation.Farmnumber = pUbnWill.Bedrijfsnummer;
            pMutation.Name = pAnimal.AniName;
            pMutation.CountryCodeBirth = "NL";
            pMutation.IDRBirthDate = pAnimal.AniBirthDate;
            pMutation.AlternateLifeNumber = pAnimal.AniLifeNumber;
            pMutation.Sex = pAnimal.AniSex;
            pMutation.Haircolor = pAnimal.Anihaircolor;
            pMutation.AniHaircolor_Memo = pAnimal.AniHaircolor_Memo;
            pMutation.MovId = pMov.MovId;
            pMutation.EventId = pEve.EventId;
            pMutation.Nling = pAnimal.AniNling;
            pMutation.Changed_By = _changedby;
            pMutation.SourceID = _sourceid;

            if (pAnimal.AniIdFather > 0)
            {
                ANIMAL vader = _dbmaster.GetAnimalById(pAnimal.AniIdFather);

                pMutation.LifeNumberFather = vader.AniLifeNumber;

            }
            pMutation.SendTo = 35;
            if (!_dbmaster.SaveMutation(pMutation))
            {
                pMutation = new MUTATION();
                pMutation.SendTo = 35;
                ret = "Interne melding opsla fout";
            }
            return ret;
        }

        public void savebasisinentinghond(UBN pUbnWill, THIRD pGebruiker, ANIMAL dog, EVENT _behandelevent)
        {
            string pr = _dbmaster.getUbnProperty(pUbnWill.UBNid, "Basisvaccinatie");
            try
            {
                if (!string.IsNullOrEmpty(pr) && dog.AniId > 0)
                {
                    MEDPLANM m = _dbmaster.GetMedPlanM(Convert.ToInt32(pr));
                    List<MEDPLAND> dees = _dbmaster.GetMedPlanDDen(m.Internalnr.ToString());

                    foreach (MEDPLAND pMedpland in dees)
                    {

                        ARTIKEL_MEDIC MedplArtmedic = _dbmaster.GetArtikelMedicById(pMedpland.MedId);
                        if (MedplArtmedic == null)
                        {
                            MedplArtmedic = new ARTIKEL_MEDIC();
                        }
                        EVENT behandelevent = _behandelevent;
                        behandelevent.Changed_By = _changedby;
                        behandelevent.SourceID = _sourceid;
                        if (_dbmaster.SaveEvent(behandelevent))
                        {
                            TREATMEN trm = new TREATMEN();
                            trm.EventId = behandelevent.EventId;
                            trm.MedId = pMedpland.MedId;
                            trm.TreKind = 0;

                            trm.TreTime = behandelevent.EveDate;


                            //interval

                            trm.TreMedPlannr = m.Internalnr;
                            trm.TreDiseaseId = m.PlanDisMaincode;



                            trm.TreMedApply = MedplArtmedic.ArtMed_Toedieningswijze;
                            trm.TreMedFunction = MedplArtmedic.ArtMed_Function;

                            trm.TreMedReg = MedplArtmedic.ArtMed_Reg;
                            trm.TreMedUDD = MedplArtmedic.ArtMed_UDD;

                            if (trm.TreMedDaysTreat == 0)
                            { trm.TreMedDaysTreat = 1; }

                            if (MedplArtmedic.ArtMed_PriceUnit == 0)
                            { MedplArtmedic.ArtMed_PriceUnit = 1; }

                            trm.Changed_By = _changedby;
                            trm.SourceID = _sourceid;
                            _dbmaster.SaveTreatmen(trm);

                        }
                    }

                }
            }
            catch(Exception exc)
            {
                unLogger.WriteError(exc.ToString(), exc);
            }
        }
    }
}
