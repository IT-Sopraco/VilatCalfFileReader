using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE.DB;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using MySql.Data.Types;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQVerified
    {
        private DBSelectQueries Parent;

        public DBSQVerified(DBSelectQueries parent)
        {
            this.Parent = parent;
        }

        #region RVO

        public VERIFIED_ANIMAL GetVerifiedAnimal(string lifeNumber)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(GetVerifiedAnimal)} -";
            try
            {
                string sql = $"SELECT * FROM agrodata.VERIFIED_ANIMAL WHERE AnimalLifeNumber = '{lifeNumber}'";
                return Parent.getSingleItem<VERIFIED_ANIMAL>(sql);
            }
            catch (Exception ex)
            {
                unLogger.WriteError($"{prefix} Ex: {ex.Message}");
                unLogger.WriteDebug($"{prefix} Ex: {ex}");
                return new VERIFIED_ANIMAL();
            }
        }

        public bool IsUpdateVerfifiedAnimalRequired(VERIFIED_ANIMAL vaOld, VERIFIED_ANIMAL vaNew)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(IsUpdateVerfifiedAnimalRequired)} -";

            if (vaOld == null)
                return true;

            return
              (
                    vaOld.AnimalLifenumber != vaNew.AnimalLifenumber
                 || vaOld.AnimalBirthDate.Date != vaNew.AnimalBirthDate.Date
                 || vaOld.AnimalMotherLifenumber != vaNew.AnimalMotherLifenumber
                 || vaOld.AnimalSex != vaNew.AnimalSex
                 || vaOld.AnimalHaircolor != vaNew.AnimalHaircolor
                 || vaOld.AnimalBornOnUBN != vaNew.AnimalBornOnUBN
                 || vaOld.AnimalSpecies != vaNew.AnimalSpecies
                 || vaOld.CurrentUBN != vaNew.CurrentUBN
                 || vaOld.AnimalWorkNr != vaNew.AnimalWorkNr
                 || vaOld.AnimalImportDate != vaNew.AnimalImportDate
                 || vaOld.AnimalDateEnd != vaNew.AnimalDateEnd
                 || vaOld.AnimalReasonEnd != vaNew.AnimalReasonEnd
                 || vaOld.AnimalReplacementLifenumber != vaNew.AnimalReplacementLifenumber
                 || vaOld.AnimalCountryCodeOrigin1 != vaNew.AnimalCountryCodeOrigin1
                 || vaOld.AnimalCountryCodeOrigin2 != vaNew.AnimalCountryCodeOrigin2
              );
        }

        public void UpdateVerfiedAnimalIfRequired(ANIMAL ani, string birthUBN, string currentUBN, int changedBy, int sourceId)
        {
            string lPrefixAnimal = $"{nameof(DBSQVerified)}.{nameof(UpdateVerfiedAnimalIfRequired)} Dier: '{ani.AniAlternateNumber}' -";
            if (string.IsNullOrWhiteSpace(ani.AniAlternateNumber))
            {
                unLogger.WriteError($"{lPrefixAnimal} - Levensnummer leeg! (AniId: {ani.AniId})");
                return;
            }

            VERIFIED_ANIMAL newVa = new VERIFIED_ANIMAL();
            newVa.AnimalBirthDate = ani.AniBirthDate;
            newVa.AnimalBornOnUBN = birthUBN;
            newVa.AnimalHaircolor = ani.AniHaircolor_Memo;
            newVa.AnimalLifenumber = ani.AniAlternateNumber;
            if (ani.AniIdMother == 0)
                newVa.AnimalMotherLifenumber = "";
            else
                newVa.AnimalMotherLifenumber = Parent.Animal.GetAnimalById(ani.AniIdMother)?.AniAlternateNumber;

            newVa.AnimalSex = (sbyte)ani.AniSex;
            newVa.AnimalSpecies = (int)LABELSConst.AnimalSpecies.Rund;
            newVa.CurrentUBN = currentUBN;

            //if (IsUpdateVerfifiedAnimalRequired(existingVa, newVa))
            {
                newVa.MutationDataSource = (int)changedBy;
                newVa.MutationIdentifier = sourceId;
                newVa.MutationDate = DateTime.Now;
                newVa.Changed_By = (int)changedBy;
                newVa.SourceID = sourceId;

                if (!UpdateOrInsertVerifiedAnimal(newVa))
                {
                    unLogger.WriteError($"{lPrefixAnimal} Error tijdens updaten VERIFIED_ANIMAL.");
                }
                else
                {
                    unLogger.WriteInfo($"{lPrefixAnimal} VERIFIED_ANIMAL geupdate.");
                }
            }
        }

        public bool UpdateVerifiedAnimalTimestamp(string lifeNumber, int changed_By = 0, int sourceId = 0)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(UpdateVerifiedAnimalTimestamp)} -";
            try
            {
                if (string.IsNullOrWhiteSpace(lifeNumber))
                {
                    unLogger.WriteError($"{prefix} Levensnummer leeg!");
                    return false;
                }

                string sql = $"UPDATE agrodata.VERIFIED_ANIMAL SET MutationDate = Now(), MutationDataSource = {changed_By}, MutationIdentifier = {sourceId} WHERE AnimalLifeNumber = '{lifeNumber}'";

                using (System.Data.Common.DbCommand cmd = Parent.GetDBCommand())
                {
                    cmd.CommandText = sql;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string msg = $"{prefix} LifeNumber: '{lifeNumber}' Ex: {ex.Message}";
                        unLogger.WriteError(msg);
                        unLogger.WriteDebug(msg, ex);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string msg = $"{prefix} LifeNr: '{lifeNumber}' Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
                return false;
            }
        }

        public bool UpdateOrInsertVerifiedAnimal(VERIFIED_ANIMAL va)
        {
            if (string.IsNullOrWhiteSpace(va.AnimalLifenumber))
            {
                unLogger.WriteError($"{nameof(DBSQVerified)}.{nameof(UpdateOrInsertVerifiedAnimal)} - Levensnummer leeg!");
                return false;
            }
            string qry = @"REPLACE INTO agrodata.VERIFIED_ANIMAL 
                   (AnimalLifenumber, AnimalBirthDate, AnimalMotherLifenumber, AnimalSex, AnimalHaircolor, AnimalBornOnUBN, AnimalSpecies, AnimalWorkNr, AnimalImportDate, AnimalDateEnd, AnimalReasonEnd, AnimalReplacementLifenumber, AnimalCountryCodeOrigin1, AnimalCountryCodeOrigin2,  MutationDate, MutationDataSource, MutationIdentifier, CurrentUBN)
                   VALUES
                   (?AnimalLifenumber, ?AnimalBirthDate, ?AnimalMotherLifenumber, ?AnimalSex, ?AnimalHaircolor, ?AnimalBornOnUBN, ?AnimalSpecies, ?AnimalWorkNr, ?AnimalImportDate, ?AnimalDateEnd,
                    ?AnimalReasonEnd, ?AnimalReplacementLifenumber, ?AnimalCountryCodeOrigin1, ?AnimalCountryCodeOrigin2,  ?MutationDate, ?MutationDataSource, ?MutationIdentifier, ?CurrentUBN)";

            using (System.Data.Common.DbCommand cmd = Parent.GetDBCommand())
            {
                cmd.CommandText = qry;

                cmd.Parameters.Add(new MySqlParameter("AnimalLifenumber", va.AnimalLifenumber));
                cmd.Parameters.Add(new MySqlParameter("AnimalBirthDate", va.AnimalBirthDate));
                cmd.Parameters.Add(new MySqlParameter("AnimalMotherLifenumber", va.AnimalMotherLifenumber));
                cmd.Parameters.Add(new MySqlParameter("AnimalSex", va.AnimalSex));
                cmd.Parameters.Add(new MySqlParameter("AnimalHaircolor", va.AnimalHaircolor));
                cmd.Parameters.Add(new MySqlParameter("AnimalBornOnUBN", va.AnimalBornOnUBN));
                cmd.Parameters.Add(new MySqlParameter("AnimalSpecies", va.AnimalSpecies));
                cmd.Parameters.Add(new MySqlParameter("AnimalWorkNr", va.AnimalWorkNr));
                cmd.Parameters.Add(new MySqlParameter("AnimalImportDate", va.AnimalImportDate));
                cmd.Parameters.Add(new MySqlParameter("AnimalDateEnd", va.AnimalDateEnd));
                cmd.Parameters.Add(new MySqlParameter("AnimalReasonEnd", va.AnimalReasonEnd));
                cmd.Parameters.Add(new MySqlParameter("AnimalReplacementLifenumber", va.AnimalReplacementLifenumber));
                cmd.Parameters.Add(new MySqlParameter("AnimalCountryCodeOrigin1", va.AnimalCountryCodeOrigin1));
                cmd.Parameters.Add(new MySqlParameter("AnimalCountryCodeOrigin2", va.AnimalCountryCodeOrigin2));
                cmd.Parameters.Add(new MySqlParameter("MutationDate", DateTime.Now.Date));
                cmd.Parameters.Add(new MySqlParameter("MutationDataSource", va.MutationDataSource));
                cmd.Parameters.Add(new MySqlParameter("MutationIdentifier", va.MutationIdentifier));
                cmd.Parameters.Add(new MySqlParameter("CurrentUBN", va.CurrentUBN));

                try
                {
                    return (cmd.ExecuteNonQuery() >= 1);
                }
                catch (Exception ex)
                {
                    string msg = $"{nameof(DBSQVerified)}.{nameof(UpdateOrInsertVerifiedAnimal)} LifeNumber: '{va.AnimalLifenumber}' Ex: {ex.Message}";
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);
                    return false;
                }
            }

        }

        public IEnumerable<VERIFIED_MOVEMENT> GetVerifiedMovements(string lifeNumber)
        {
            string sql = $"SELECT * FROM agrodata.VERIFIED_MOVEMENT WHERE AnimalLifeNumber = '{lifeNumber}'";
            return Parent.getList<VERIFIED_MOVEMENT>(Parent.QueryData(sql));
        }

        public string GetBirthUBN(IEnumerable<VERIFIED_MOVEMENT> movements)
        {
            if (movements == null || !movements.Any())
                return "";

            return movements.First().MovementUBN;
        }

        public string GetCurrentUBN(IEnumerable<VERIFIED_MOVEMENT> movements)
        {
            if (movements == null || !movements.Any())
                return "";

            return movements.Last().MovementUBN;
        }

        public bool IsUpdateVerifiedMovementsRequired(IEnumerable<VERIFIED_MOVEMENT> vmsOld, IEnumerable<VERIFIED_MOVEMENT> vmsNew)
        {
            if (vmsOld.Count() != vmsNew.Count())
                return true;

            foreach (VERIFIED_MOVEMENT vm in vmsOld)
            {
                VERIFIED_MOVEMENT vm2 = vmsNew.FirstOrDefault(x => x.MovementId == vm.MovementId);

                if (vm2 == null || vm2.MovementId == 0)
                    return true;

                else if (IsUpdateVerifiedMovementRequired(vm, vm2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUpdateVerifiedMovementRequired(VERIFIED_MOVEMENT vmOld, VERIFIED_MOVEMENT vmNew)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(IsUpdateVerifiedMovementRequired)} -";

            if (vmOld == null)
                return true;

            return (
                        vmOld.MovementId != vmNew.MovementId
                     || vmOld.AnimalLifenumber != vmNew.AnimalLifenumber
                     || vmOld.MovementOrder != vmNew.MovementOrder
                     || vmOld.MovementInDate != vmNew.MovementInDate
                     || vmOld.MovementOutDate != vmNew.MovementOutDate
                     || vmOld.MovementUBN != vmNew.MovementUBN
                     || vmOld.MovementZipcode != vmNew.MovementZipcode
                     || vmOld.MovementCompanyType != vmNew.MovementCompanyType
                );
        }

        public bool SetVerifiedAnimalAndMovements(string lifeNumber, string werknummer, DateTime geboortedat, DateTime importdat, string landCodeHerkomst, string landCodeOorsprong, int aniSex, string haarkleur, DateTime einddatum, string redenEinde, string levensnrMoeder, string vervangenLevensnr, IEnumerable<VERIFIED_MOVEMENT> verblijfplaatsen, int animalSpecies, int changed_By, int sourceId)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(lifeNumber))
                {
                    unLogger.WriteError($"{nameof(DBSQVerified)}.{nameof(SetVerifiedAnimalAndMovements)} - Levensnummer leeg! ");
                    return false;
                }
                var dbAnimal = GetVerifiedAnimal(lifeNumber);
                var dbMovs = GetVerifiedMovements(lifeNumber);

                VERIFIED_ANIMAL va = new VERIFIED_ANIMAL();

                va.AnimalLifenumber = lifeNumber;
                va.AnimalBirthDate = geboortedat;
                va.AnimalBornOnUBN = GetBirthUBN(verblijfplaatsen);
                va.AnimalCountryCodeOrigin1 = landCodeHerkomst;
                va.AnimalCountryCodeOrigin2 = landCodeOorsprong;
                va.AnimalDateEnd = einddatum;
                va.AnimalHaircolor = haarkleur;
                va.AnimalImportDate = importdat;

                va.AnimalMotherLifenumber = levensnrMoeder;
                va.AnimalReasonEnd = redenEinde;
                va.AnimalReplacementLifenumber = vervangenLevensnr;

                va.AnimalSex = (sbyte)aniSex;
                va.AnimalSpecies = animalSpecies;
                va.AnimalWorkNr = werknummer;
                va.CurrentUBN = GetCurrentUBN(verblijfplaatsen);
                va.MutationDate = DateTime.Now;

                va.MutationDataSource = changed_By;
                va.MutationIdentifier = sourceId;

                if (IsUpdateVerfifiedAnimalRequired(dbAnimal, va))
                    UpdateOrInsertVerifiedAnimal(va);
                else
                    UpdateVerifiedAnimalTimestamp(lifeNumber, changed_By, sourceId);

                if (IsUpdateVerifiedMovementsRequired(dbMovs, verblijfplaatsen))
                    SetMovementsForAnimal(lifeNumber, verblijfplaatsen, changed_By, sourceId);
                else
                    UpdateVerifiedMovementsTimestamp(lifeNumber, changed_By, sourceId);

                return true;

            }
            catch (Exception ex)
            {
                string msg = $"{nameof(DBSQVerified)}.{nameof(SetVerifiedAnimalAndMovements)} LifeNumber: '{lifeNumber}' - Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
                return false;
            }
        }

        private bool SetMovementsForAnimal(string lifeNumber, IEnumerable<VERIFIED_MOVEMENT> movements, int changed_By, int SourceId)
        {
            string lPrefix = $"{nameof(DBSQVerified)}.{nameof(SetMovementsForAnimal)} -";
            if (string.IsNullOrWhiteSpace(lifeNumber))
            {
                unLogger.WriteError($"{lPrefix} - Levensnummer leeg! ");
                return false;
            }

            if (movements.Any(m => m.AnimalLifenumber != lifeNumber))
            {
                unLogger.WriteError($"{lPrefix} Niet alle movements zijn van het lifeNumber: '{lifeNumber}'");
                return false;
            }

            using (System.Data.Common.DbCommand cmd = Parent.GetDBCommand())
            {
                try
                {
                    cmd.CommandText = "START TRANSACTION; ";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"DELETE FROM agrodata.VERIFIED_MOVEMENT WHERE AnimalLifenumber = ?lifeNumber";
                    cmd.Parameters.Add(new MySqlParameter("lifeNumber", lifeNumber));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    int order = 1;
                    cmd.CommandText = @"INSERT INTO agrodata.VERIFIED_MOVEMENT 
                                        (AnimalLifenumber, MovementOrder, MovementInDate, MovementOutDate, MovementUBN, MovementZipcode, MovementCompanyType, MutationDate, Changed_By, SourceId)
                                        VALUES
                                        (?AnimalLifenumber, ?MovementOrder, ?MovementInDate, ?MovementOutDate, ?MovementUBN, ?MovementZipcode, ?MovementCompanyType, ?MutationDate, ?Changed_By, ?SourceId)";

                    foreach (VERIFIED_MOVEMENT vm in movements)
                    {
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new MySqlParameter("AnimalLifenumber", vm.AnimalLifenumber));
                        cmd.Parameters.Add(new MySqlParameter("MovementOrder", order));
                        cmd.Parameters.Add(new MySqlParameter("MovementInDate", vm.MovementInDate));
                        cmd.Parameters.Add(new MySqlParameter("MovementOutDate", vm.MovementOutDate));
                        cmd.Parameters.Add(new MySqlParameter("MovementUBN", vm.MovementUBN));
                        cmd.Parameters.Add(new MySqlParameter("MovementZipcode", vm.MovementZipcode));
                        cmd.Parameters.Add(new MySqlParameter("MovementCompanyType", vm.MovementCompanyType));
                        cmd.Parameters.Add(new MySqlParameter("MutationDate", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("Changed_By", changed_By));
                        cmd.Parameters.Add(new MySqlParameter("SourceId", SourceId));

                        order++;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Parameters.Clear();

                    cmd.CommandText = "COMMIT";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "ROLLBACK";
                    cmd.ExecuteNonQuery();

                    string msg = $"{lPrefix} Exception ex: " + ex.Message;
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);

                    return false;
                }
            }
            return true;

        }

        public bool UpdateVerifiedMovementsTimestamp(string lifeNumber, int changed_By = 0, int sourceId = 0)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(UpdateVerifiedMovementsTimestamp)} -";

            try
            {
                if (string.IsNullOrWhiteSpace(lifeNumber))
                {
                    unLogger.WriteError($"{prefix} Levensnummer leeg! ");
                    return false;
                }

                string sql = $@"UPDATE agrodata.VERIFIED_MOVEMENT 
                                SET 
                                    MutationDate = Now(),
                                    Changed_By = {changed_By},
                                    SourceId = {sourceId} 
                                
                                WHERE AnimalLifeNumber = '{lifeNumber}'";

                using (System.Data.Common.DbCommand cmd = Parent.GetDBCommand())
                {
                    cmd.CommandText = sql;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string msg = $"{prefix} LifeNumber: '{lifeNumber}' Ex: {ex.Message}";
                        unLogger.WriteError(msg);
                        unLogger.WriteDebug(msg, ex);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string msg = $"{prefix} LifeNumber: '{lifeNumber}' Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
                return false;
            }
        }

        #endregion


        #region Sanitel

        public IEnumerable<VERIFIED_ANIMAL_SANITEL> GetVerifiedAnimalsSanitel(IEnumerable<string> lifeNumbers)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(GetVerifiedAnimalsSanitel)} -";

            if (lifeNumbers == null || lifeNumbers.Count() == 0)
            {
                unLogger.WriteError($"{prefix} Parameter lifeNumbers is leeg.");
                return new List<VERIFIED_ANIMAL_SANITEL>();
            }

            try
            {
                string sql = $"SELECT * FROM agrodata.VERIFIED_ANIMAL_SANITEL WHERE AnimalLifeNumber IN ({"'" + string.Join("','", lifeNumbers) + "'"})";
                return Parent.getList<VERIFIED_ANIMAL_SANITEL>(Parent.QueryData(sql));
                
            }
            catch (Exception ex)
            {
                unLogger.WriteError($"{prefix} Ex: {ex.Message}");
                unLogger.WriteDebug($"{prefix} Ex: {ex}");
                return new List<VERIFIED_ANIMAL_SANITEL>();
            }
        }

        public IEnumerable<VERIFIED_MOVEMENT_SANITEL> GetVerifiedMovementsSanitel(IEnumerable<string> lifeNumbers)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(GetVerifiedMovementsSanitel)} -";

            if (lifeNumbers == null || lifeNumbers.Count() == 0)
            {
                unLogger.WriteError($"{prefix} Parameter lifeNumbers is leeg.");
                return new List<VERIFIED_MOVEMENT_SANITEL>();
            }

            string sql = $"SELECT * FROM agrodata.VERIFIED_MOVEMENT_SANITEL WHERE AnimalLifeNumber IN ({"'" + string.Join("','", lifeNumbers) + "'"})";
            return Parent.getList<VERIFIED_MOVEMENT_SANITEL>(Parent.QueryData(sql));
        }

        /// <summary>
        /// Overschrijft data voor VERIFIED_ANIMAL_SANITEL en VERIFIED_MOVEMENT_SANITEL. movements die niet worden meegegeven worden verwijderd
        /// uit de db.
        /// </summary>
        /// <param name="animals"></param>
        /// <param name="movements"></param>
        /// <param name="changedBy"></param>
        /// <param name="SourceId"></param>
        /// <returns></returns>
        public bool UpdateVerifiedDataSanitel(IEnumerable<VERIFIED_ANIMAL_SANITEL> animals, IEnumerable<VERIFIED_MOVEMENT_SANITEL> movements, LABELSConst.ChangedBy changedBy, int SourceId)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(UpdateVerifiedDataSanitel)} -";

            if (animals.Count() == 0)
            {
                unLogger.WriteError($"{prefix} Geen dieren meegegeven. ");
                return false;
            }
            string lifeNrs = $"'{string.Join("','", animals.Select(a => a.AnimalLifenumber))}'";


            using (System.Data.Common.DbCommand cmd = Parent.GetDBCommand())
            {
                MySqlTransaction trans = null;
                try
                {
                    trans = (cmd.Connection as MySqlConnection).BeginTransaction();
                    cmd.Transaction = trans;


                    cmd.CommandText = $"DELETE FROM agrodata.VERIFIED_ANIMAL_SANITEL WHERE AnimalLifenumber IN ({lifeNrs});";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"DELETE FROM agrodata.VERIFIED_MOVEMENT_SANITEL WHERE AnimalLifenumber IN ({lifeNrs});";
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();

                    foreach(var va in animals)
                    {
                        cmd.CommandText = @"INSERT INTO agrodata.VERIFIED_ANIMAL_SANITEL
	                        (AnimalLifenumber, AnimalBirthDate, AnimalMotherLifenumber, AnimalHaircolor, AnimalGenderCode, AnimalVRVName, AnimalType, AnimalPurchaseDate, AnimalDepartureDate, AnimalDeathDate, AnimalFacility, AnimalSanitaryUnit, AnimalProductionUnit, AnimalBirthFacility, AnimalPassportVersionNr, RequestDate, Changed_By, SourceID)
                            VALUES
	                        (?AnimalLifenumber, ?AnimalBirthDate, ?AnimalMotherLifenumber, ?AnimalHaircolor, ?AnimalGenderCode, ?AnimalVRVName, ?AnimalType, ?AnimalPurchaseDate, ?AnimalDepartureDate, ?AnimalDeathDate, ?AnimalFacility, ?AnimalSanitaryUnit, ?AnimalProductionUnit, ?AnimalBirthFacility, ?AnimalPassportVersionNr, Now(), ?Changed_By, ?SourceID)";

                        cmd.Parameters.Add(new MySqlParameter("AnimalLifenumber", va.AnimalLifenumber));
                        cmd.Parameters.Add(new MySqlParameter("AnimalBirthDate", va.AnimalBirthDate));
                        cmd.Parameters.Add(new MySqlParameter("AnimalMotherLifenumber", va.AnimalMotherLifenumber));
                        cmd.Parameters.Add(new MySqlParameter("AnimalHaircolor", va.AnimalHaircolor));
                        cmd.Parameters.Add(new MySqlParameter("AnimalGenderCode", va.AnimalGenderCode));
                        cmd.Parameters.Add(new MySqlParameter("AnimalVRVName", va.AnimalVRVName));
                        cmd.Parameters.Add(new MySqlParameter("AnimalType", va.AnimalType));
                        cmd.Parameters.Add(new MySqlParameter("AnimalPurchaseDate", va.AnimalPurchaseDate));
                        cmd.Parameters.Add(new MySqlParameter("AnimalDepartureDate", va.AnimalDepartureDate));
                        cmd.Parameters.Add(new MySqlParameter("AnimalDeathDate", va.AnimalDeathDate));
                        cmd.Parameters.Add(new MySqlParameter("AnimalFacility", va.AnimalFacility));
                        cmd.Parameters.Add(new MySqlParameter("AnimalSanitaryUnit", va.AnimalSanitaryUnit));
                        cmd.Parameters.Add(new MySqlParameter("AnimalProductionUnit", va.AnimalProductionUnit));
                        cmd.Parameters.Add(new MySqlParameter("AnimalBirthFacility", va.AnimalBirthFacility));
                        cmd.Parameters.Add(new MySqlParameter("AnimalPassportVersionNr", va.AnimalPassportVersionNr));
                        cmd.Parameters.Add(new MySqlParameter("Changed_By", va.Changed_By));
                        cmd.Parameters.Add(new MySqlParameter("SourceID", va.SourceID));

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        cmd.CommandText = @"INSERT INTO agrodata.VERIFIED_MOVEMENT_SANITEL
                                        	(AnimalLifenumber, MovementDate, MovementOrder, MovementNotificationType, MovementAnimalCondition, MovementSourceSanitaryUnit, MovementDestinationSanitaryUnit, MovementSourceFacility, MovementDestinationFacility, Changed_By, SourceID)

                                        VALUES
                                        	(?AnimalLifenumber, ?MovementDate, ?MovementOrder, ?MovementNotificationType, ?MovementAnimalCondition, ?MovementSourceSanitaryUnit, ?MovementDestinationSanitaryUnit, ?MovementSourceFacility, ?MovementDestinationFacility, ?Changed_By, ?SourceID);";

                        DateTime lastDate = DateTime.MinValue;
                        int movOrder = 1;

                        var orderedMovs = movements.Where(m => m.AnimalLifenumber == va.AnimalLifenumber).OrderBy(m => m.MovementDate).ThenBy(m => m.MovementOrder);
                        foreach (VERIFIED_MOVEMENT_SANITEL vm in orderedMovs)
                        {
                            cmd.Parameters.Clear();
                        
                            if (vm.MovementDate.Date != lastDate)
                            {
                                movOrder = 1;
                                lastDate = vm.MovementDate.Date;                                     
                            }

                            cmd.Parameters.Add(new MySqlParameter("AnimalLifenumber", vm.AnimalLifenumber));
                            cmd.Parameters.Add(new MySqlParameter("MovementDate", vm.MovementDate));
                            cmd.Parameters.Add(new MySqlParameter("MovementOrder", movOrder));
                            cmd.Parameters.Add(new MySqlParameter("MovementNotificationType", vm.MovementNotificationType));
                            cmd.Parameters.Add(new MySqlParameter("MovementAnimalCondition", vm.MovementAnimalCondition));
                            cmd.Parameters.Add(new MySqlParameter("MovementSourceSanitaryUnit", vm.MovementSourceSanitaryUnit));
                            cmd.Parameters.Add(new MySqlParameter("MovementDestinationSanitaryUnit", vm.MovementDestinationSanitaryUnit));
                            cmd.Parameters.Add(new MySqlParameter("MovementSourceFacility", vm.MovementSourceFacility));
                            cmd.Parameters.Add(new MySqlParameter("MovementDestinationFacility", vm.MovementDestinationFacility));
                            cmd.Parameters.Add(new MySqlParameter("Changed_By", (int)changedBy));
                            cmd.Parameters.Add(new MySqlParameter("SourceID", SourceId));

                            movOrder++;

                            cmd.ExecuteNonQuery();
                        }
                        cmd.Parameters.Clear();
                    }
                    trans.Commit();

                    unLogger.WriteInfo($"{prefix} Cache geupdate voor {animals.Count()} animals met {movements.Count()} movements.");

                    return true;
                }
                catch (Exception ex)
                {
                    string msg;
                    try
                    {
                        if (trans != null)
                            trans.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        msg = $"{prefix} Exception during rollback: " + rollbackEx.Message;
                        unLogger.WriteError(msg);
                        unLogger.WriteDebug(msg, rollbackEx);
                    }

                    msg = $"{prefix} Exception ex: " + ex.Message;
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);

                    return false;
                }
            }
        }

        #endregion





        #region Call INFO

        public IEnumerable<AnimalCallInfo> GetVerifiedCalls(LABELSConst.VerifiedDataSource dataSource, IEnumerable<string> lifeNrs)
        {
            var ret = new List<AnimalCallInfo>(lifeNrs.Count());

            string prefix = $"{nameof(DBSQVerified)}.{nameof(GetVerifiedCalls)} -";

            if (lifeNrs.Count() == 0)
            {
                unLogger.WriteError($"{prefix} Geen levensummers opgegeven.");
                return ret;
            }

            string strLifenrs = $"'{string.Join("','", lifeNrs)}'";

            string fields;
            switch (dataSource)
            {
                case LABELSConst.VerifiedDataSource.RVO:
                    fields = "AnimalLifenumber, vac_RVO_Count_Ok, vac_RVO_Count_Fail, vac_RVO_TS";
                    break;
                case LABELSConst.VerifiedDataSource.Sanitel:
                    fields = "AnimalLifenumber, vac_Sanitel_Count_Ok, vac_Sanitel_Count_Fail, vac_Sanitel_TS";
                    break;
                case LABELSConst.VerifiedDataSource.HiTier:
                    fields = "AnimalLifenumber, vac_Hitier_Count_Ok, vac_Hitier_Count_Fail, vac_Hitier_TS";
                    break;
                case LABELSConst.VerifiedDataSource.DCF:
                    fields = "AnimalLifenumber, vac_DCF_Count_Ok, vac_DCF_Count_Fail, vac_DCF_TS";
                    break;
                default:
                    unLogger.WriteError($"{prefix} dataSource: '{dataSource}' Not Implemented.");
                    throw new NotImplementedException();
            }

            string sql = $"SELECT {fields} FROM agrodata.VERIFIED_ANIMAL_CALL WHERE AnimalLifenumber IN ({strLifenrs})";

            var dt = Parent.QueryData(sql);

            foreach (DataRow dr in dt.Rows)
            {
                string lifenr = dr.Field<string>("AnimalLifenumber");
                int Ok = dr.Field<int>(1);
                int Error = dr.Field<int>(2);
                DateTime? Ts = null;
                if (dr[3] != DBNull.Value)
                    Ts = MysqlDateTimeConverter.ConvertToDateTime(dr.Field<MySqlDateTime>(3));

                ret.Add(new AnimalCallInfo(lifenr, Ok, Error, Ts));
            }

            foreach (string lnr in lifeNrs)
            {
                if (!ret.Any(aci => aci.LifeNumber == lnr))
                    ret.Add(new AnimalCallInfo(lnr, 0, 0, null));
            }

            return ret;
        }

        public AnimalCallInfo GetCall(LABELSConst.VerifiedDataSource dataSource, string lifenr)
        {
            return GetVerifiedCalls(dataSource, new string[] { lifenr }).FirstOrDefault();
        }

        /// <summary>
        /// Format of callLogs = Lifenr, CallisError
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="callLogs"></param>
        public bool SetVerifiedCalls(LABELSConst.VerifiedDataSource dataSource, IEnumerable<Tuple<string, bool>> callLogs)
        {
            string prefix = $"{nameof(DBSQVerified)}.{nameof(SetCall)} -";

            try
            {
                string field_ok;
                string field_error;
                string ts;

                switch (dataSource)
                {
                    case LABELSConst.VerifiedDataSource.RVO:
                        ts = "vac_RVO_TS";
                        field_ok = "vac_RVO_Count_Ok";
                        field_error = "vac_RVO_Count_Fail";
                        break;
                    case LABELSConst.VerifiedDataSource.Sanitel:
                        ts = "vac_Sanitel_TS";
                        field_ok = "vac_Sanitel_Count_Ok";
                        field_error = "vac_Sanitel_Count_Fail";
                        break;
                    case LABELSConst.VerifiedDataSource.HiTier:
                        ts = "vac_Hitier_TS";
                        field_ok = "vac_Hitier_Count_Ok";
                        field_error = "vac_Hitier_Count_Fail";
                        break;
                    case LABELSConst.VerifiedDataSource.DCF:
                        ts = "vac_DCF_TS";
                        field_ok = "vac_DCF_Count_Ok";
                        field_error = "vac_DCF_Count_Fail";
                        break;
                    default:
                        unLogger.WriteError($"{prefix} dataSource: '{dataSource}' Not Implemented.");
                        throw new NotImplementedException();
                }


                using (System.Data.Common.DbCommand cmd = Parent.GetDBCommand())
                {
                    MySqlTransaction trans = null;
                    try
                    {
                        trans = (cmd.Connection as MySqlConnection).BeginTransaction();
                        cmd.Transaction = trans;

                        foreach (var cl in callLogs)
                        {
                            string lifeNr = cl.Item1;
                            bool callIsError = cl.Item2;

                            int iOk = callIsError ? 0 : 1;
                            int iError = callIsError ? 1 : 0;
                            string dupField = callIsError ? field_error : field_ok;

                            string qry = $@"INSERT INTO agrodata.VERIFIED_ANIMAL_CALL 
                                (AnimalLifeNumber, {field_ok}, {field_error}, {ts}) 
                                              VALUES 
                                              ('{lifeNr}', {iOk}, {iError}, Now())                                    
                                              ON DUPLICATE KEY UPDATE 
                                                {dupField} = {dupField}+1,
                                                {ts}=Now()";
                                 if (!callIsError)
                                    qry += $", {field_error} = 0";

                                cmd.CommandText = qry;
                                cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex2)
                    {
                        string msg;
                        try
                        {
                            if (trans != null)
                                trans.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            msg = $"{prefix} Exception during rollback: " + rollbackEx.Message;
                            unLogger.WriteError(msg);
                            unLogger.WriteDebug(msg, rollbackEx);
                        }

                        msg = $"{prefix} Exception ex: " + ex2.Message;
                        unLogger.WriteError(msg);
                        unLogger.WriteDebug(msg, ex2);
                        return false;
                    }                
                }
            }
            catch (Exception ex)
            {
                string msg = $"{prefix} Datasource: '{dataSource}' Nr of Calllogs: {callLogs.Count()} Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);
                return false;
            }
        }

        public void SetCall(LABELSConst.VerifiedDataSource dataSource, string lifenr, bool callIsError)
        {
            var tup = new Tuple<string, bool>(lifenr, callIsError);
            SetVerifiedCalls(dataSource, new List<Tuple<string, bool>>() { tup }); 
        }

        #endregion



    }
}