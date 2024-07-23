using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQDeenssysteem
    {
        private DBSelectQueries Parent;

        public DBSQDeenssysteem(DBSelectQueries parent)
        {
            this.Parent = parent;
        }


        public DataTable GetK02Animals(int ubnId)
        {
            string qry_Animals = String.Format(
                @"SET @ani := 0, @num := 0, @lastDate := FROM_UNIXTIME(0);
                
				SELECT a.AniId, a.AniAlternateNumber, ac.AniWorknumber, a.AniBirthDate, 

                (SELECT min(e.EveDate) FROM EVENT e JOIN BIRTH b ON e.EventId = b.EventId WHERE e.EveKind = 5 AND e.AniId = a.AniId 
                    AND (b.BirNumber = 1 || b.BirNumber = 0 || b.BirNumber is null))                                                                              as FirstBirDate,

                (SELECT Max(e.EveDate) FROM EVENT e WHERE e.EventId > 0 AND e.EveKind = 5 AND e.AniId = a.AniId ) AS LastCalveDate,
                (SELECT Max(e.EveDate) FROM EVENT e WHERE e.EventId > 0 AND e.EveKind = 5 AND e.AniId = a.AniId AND e.EveDate < LastCalveDate) AS PenultimateCalveDate,                
                (SELECT Max(if(e.EveKind = 2, e.EveDate, DATE_ADD(e.EveDate, INTERVAL -7 DAY)))
                    FROM EVENT e WHERE e.EventId > 0 AND e.EveKind IN (2, 9) AND e.AniId = a.AniId
                    ) AS LastInseminDate,
                    
                                  (SELECT Max(if(e.EveKind = 2, e.EveDate, DATE_ADD(e.EveDate, INTERVAL -7 DAY)))
                    FROM EVENT e WHERE e.EventId > 0 AND e.EveKind IN (2, 9) AND e.AniId = a.AniId AND e.EveDate < LastInseminDate
                    ) AS PenultimateInseminDate,  
                (SELECT Max(e.EveDate) FROM EVENT e WHERE e.EventId > 0 AND e.EveKind = 1 AND e.AniId = a.AniId ) AS LastInheatDate,
                (SELECT Max(e.EveDate) FROM EVENT e WHERE e.EventId > 0 AND e.EveKind = 4 AND e.AniId = a.AniId ) AS LastInsertedDryoffDate, 	
                (SELECT Max(e.EveDate) FROM EVENT e JOIN GESTATIO g ON e.EventId = g.EventId 
                        WHERE e.EventId > 0 AND e.EveKind = 3 AND e.AniId = a.AniId AND (g.GesStatus = 1 OR g.GesStatus = 2) ) 
                        AS LastPositveGestation,
                (SELECT Max(e.EveDate) FROM EVENT e JOIN GESTATIO g ON e.EventId = g.EventId 
                        WHERE e.EventId > 0 AND e.EveKind = 3 AND e.AniId = a.AniId AND g.GesStatus = 3 ) 
                        AS LastNegativeGestation,
					(SELECT b2.CalfWeight FROM EVENT e2 JOIN BIRTH b2 ON e2.EventId = b2.EventId WHERE b2.CalfId = a.AniId ORDER BY b2.EventId DESC LIMIT 1)
					As CalvingWeigth,

                (SELECT UNIX_TIMESTAMP(Max(ana.AnaMilkDate)) FROM ANALYSE ana 
                    WHERE ana.AnaMilkDate < LastCalveDate AND ana.AniId = a.AniId AND ana.AnaKgMilk > 0) AS LastMilkDateBeforeCalving,
                (SELECT UNIX_TIMESTAMP(Min(ana.AnaMilkDate)) FROM ANALYSE ana 
                    WHERE ana.AnaMilkDate > LastCalveDate AND ana.AniId = a.AniId AND ana.AnaKgMilk > 0) AS FirstMilkDateAfterCalving,
                (SELECT DATE_ADD(FROM_UNIXTIME(LastMilkDateBeforeCalving), INTERVAL 15 DAY)) AS LastCalculatedDryOff,
		
					dtMaxBir.maxBir AS LactatieNr,
					(SELECT max(m.MovDate) FROM MOVEMENT m WHERE m.AniId = a.AniId AND m.MovKind = 1 AND m.UbnId = {0}) as LastArrival,
					if (ac.AniCategory = 4,
						(SELECT max(m.MovDate) FROM MOVEMENT m WHERE m.AniId = a.AniId AND m.MovKind IN (2,3) AND m.UbnId = {0})
						,
						''
					) as Departure,
					ac.Anicategory,
                    dtMaxBir.rasbalk
			

                FROM ANIMAL a 
                INNER JOIN ANIMALCATEGORY ac ON a.AniId = ac.AniId
					 JOIN agrofactuur.BEDRIJF b ON ac.FarmId = b.FarmId
                LEFT JOIN 
				(
                    SELECT dt.AniId, Max(birNr) as maxBir, GROUP_CONCAT(DISTINCT CONCAT(Lablabel, SraRate) ORDER BY SraRate DESC SEPARATOR '') as rasbalk
                    FROM
                    (
	                    SELECT e.AniId, e.EveDate,

	                    @lastDate := if (e.AniId = @ani, @lastDate, FROM_UNIXTIME(0)) as dummy0,
   
	                    @num := 
		                    if (e.AniId = @ani,
			                    if(e.EveDate > DATE_ADD(@lastDate, INTERVAL 7 DAY), @num + 1, @num),
			                    1
		                ) as birNr,

	                    @ani := e.AniId as dummy1,
                        @lastDate := e.EveDate as dummy2

                        FROM 
                        (
                            SELECT e.AniId, e.EveDate
                            FROM EVENT e
	                        JOIN ANIMALCATEGORY ac2 ON e.AniId = ac2.AniId
	                        JOIN agrofactuur.BEDRIJF b2 ON ac2.FarmId = b2.FarmId
	                        AND b2.UBNId = {0}
	                        AND ac2.AniCategory <= 4
	                        AND e.EveKind = 5
                            AND e.EventId > 0
                            ORDER BY e.AniId, e.EveDate ASC
                        ) e
                        ) as dt 
                        JOIN SECONRAC sr on sr.AniId = dt.AniId
						JOIN agrofactuur.AGRO_LABELS al on al.LabID = sr.RacId
						WHERE al.LabKind = 206 
				        AND al.labProgramId = 0 
				        AND al.LabProgId = 1 
				        AND al.LabCountry = 0
                        GROUP BY AniId	 
					 ) dtMaxBir ON dtMaxBir.AniId = a.AniId
                WHERE b.UBNId = {0}
                AND ac.Anicategory <= 4
                AND ac.FarmId > 0
                AND a.AniId > 0
                AND a.AniSex = 2                
                GROUP BY a.AniId
                HAVING (ac.AniCategory < 4 OR Departure > DATE_ADD(Now(), INTERVAL -1 YEAR))
					", ubnId);

            return Parent.QueryData(qry_Animals, MissingSchemaAction.Add);
        }

        public IEnumerable<WEIGHT> GetK02Weights(int ubnId)
        {
            string qry_Weights = 
                $@" SELECT w.*
                    FROM WEIGHT w
                    JOIN
                    (
                        SELECT DISTINCT ac.AniId FROM ANIMALCATEGORY ac 
                        JOIN agrofactuur.BEDRIJF b ON ac.FarmId = b.FarmId
                        WHERE ac.UbnId = {ubnId}
                        AND ac.AniId > 0
                        AND ac.FarmId > 0
                    ) anis ON w.AniId = anis.AniId";

            return Parent.getList<WEIGHT>(Parent.QueryData(qry_Weights));            
        }

        public IEnumerable<EVENT> GetK02Insemins(int ubnId)
        {
            string qry_Insemins =
                $@" SELECT e.*
                    FROM EVENT e
                    JOIN INSEMIN i ON e.EventId = i.EventId AND e.EveKind = 2
                    JOIN
                    (
                        SELECT DISTINCT ac.AniId FROM ANIMALCATEGORY ac 
                        JOIN agrofactuur.BEDRIJF b ON ac.FarmId = b.FarmId
                        WHERE ac.UbnId = {ubnId}
                        AND ac.AniId > 0
                        AND ac.FarmId > 0
                    ) anis ON e.AniId = anis.AniId";

            return Parent.getList<EVENT>(Parent.QueryData(qry_Insemins));
        }
        
        public DataTable getk02MPR(int ubnId)
        {
            string qry_Mpr = String.Format(
                @"SET @num := 0, @ani := '';

                SELECT *
                FROM (
                    SELECT *,
                    @num := if(@ani = AniId, @num + 1, 1) as row_number,
                    @ani := AniId as dummy
                    FROM agrobase.ANALYSE

                    #1x per maand melkmeters als mpr
#                    JOIN 
#                   (
#	                    SELECT MIN(ana2.AnaMilkDate) md
#	                    FROM agrobase.ANALYSE ana2
#	                    WHERE UbnId = {0}
#	                    GROUP BY Year(ana2.AnaMilkDate), Month(ana2.AnaMilkDate)
#                    ) bla2 ON AnaMilkDate = bla2.md

                    WHERE UbnId = {0}
                    AND AnaTypeOfControl IN (null, 0, 6)


					AND AnaKgMilk is not null
                    AND AnaKgMilk > 0
                    ORDER BY AniId, AnaMilkdate DESC
                ) as dta                 
                WHERE dta.row_number <= 3",
                ubnId);

            return Parent.QueryData(qry_Mpr);
        }

        /*
         * maxRows werkt niet meer, alles in query afvangen was te vervelend en dan word de query te ingewikkeld,
         * zo moeten bv alle ziektes helemaal gefiltered worden om te kijken of bv een disease record een record is van een disease
         * die herkent word in het deens systeem, en dus moet meetellen in aantal diagnose codes, of onbekend is, overgeslagen moet worden en
         * dus niet geselecteerd hoeft te worden hier. Het is veiliger om dit gewoon in de code te doen zodat de conversies centraal staan
         */
        public DataTable getK02Diagnoses(int ubnId, int maxAniCategory)
        {
            string qry = String.Format(
                @"SET @num := 0, @ani := '';

                SELECT *
                FROM 
                (
	                SELECT 
		                dt.*,
		                @num := if(@ani = dt.AniId, @num + 1, 1) as row_number,
		                @ani := dt.AniId as dummy
                   FROM 
                   (
		                SELECT 
      	                    e.AniId,
			                e.EveDate,
                            e.EveKind,
			                i.HeatCertainty,
			                t.TreKind,
			                g.GesStatus,
			                d.DisMainCode,
			                d.DisSubCode

                                  
	                      FROM EVENT e 
	                      LEFT JOIN INHEAT i ON e.EventId = i.EventId AND e.EveKind = 1
	                      LEFT JOIN TREATMEN t ON e.EventId = t.EventId AND e.EveKind = 6
	                      LEFT JOIN GESTATIO g ON e.EventId = g.EventId AND e.EveKind = 3
	                      LEFT JOIN DISEASE d ON e.EventId = d.EventId AND e.EveKind = 7
                	                
	                      JOIN 
                         (   
				                SELECT DISTINCT aniCat.AniId
	                         FROM ANIMALCATEGORY aniCat
	                         JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
				                WHERE  aniCat.AniCategory <= {0}
	                         AND bedr.UbnId = {1}
	                         AND aniCat.AniId > 0
                             AND aniCat.FarmId > 0
                         ) ac ON e.AniId = ac.AniId

			                WHERE e.EventId > 0

	                      AND ((e.EveKind = 11) #blood
	                      OR  (e.EveKind = 1 AND (i.HeatCertainty is null OR i.HeatCertainty IN (0, 11)))	#inheat
	                      OR  (e.EveKind = 6 AND (t.TreKind IN (10003, 10012, 10007, 10001, 10013))) #treatmen
	                      OR  (e.EveKind = 3 AND (g.GesStatus IN (1, 2, 3, 4)))
	                      OR  (e.EveKind = 7)
				          )
	                ) as dt
	                ORDER BY AniId, EveDate DESC
                ) as dt2 ", maxAniCategory, ubnId);

            return Parent.QueryData(qry);
        }

        [Obsolete("Maakt gebruik van een temp table zou niet meer in gebruik moeten zijn.")]
        public DataTable getK02EMM(int UbnId)
        {
            string qry_Mpr = String.Format(
                @"DROP TEMPORARY TABLE IF EXISTS agrodata.temptable_LastEmmDates;

                CREATE TEMPORARY TABLE IF NOT EXISTS agrodata.temptable_LastEmmDates 
                (
	                AniId INT (11) NOT NULL,
                    LastDate DATE NOT NULL,
	                PRIMARY KEY(AniId)
                ) ENGINE=MEMORY;    

                INSERT INTO agrodata.temptable_LastEmmDates
                (
	                SELECT 
		                e.AniId,
		                Max(e.DateMilking) AS LastDate
	                FROM agrodata.EMMMILK 		e
	                JOIN agrofactuur.BEDRIJF 	b 		ON e.FarmId 	= b.FarmId
	                JOIN agrofactuur.UBN 		u     ON b.UbnId  	= u.UbnId
	                WHERE u.UbnId = {0}
	                GROUP BY e.AniId
                );

                SELECT
                    LED.AniId,  
			        LED.LastDate,			
			           (SELECT sum(kgMilk) FROM agrodata.EMMMILK emm 
                            WHERE emm.AniId = LED.AniId AND emm.DateMilking = LED.LastDate) as ProdDay,
			           (SELECT sum(kgMilk) FROM agrodata.EMMMILK emm 
                            WHERE emm.AniId = LED.AniId AND emm.DateMilking > DATE_ADD(LED.LastDate, INTERVAL -1 WEEK)) as ProdWeek,
			           (SELECT count(distinct DateMilking) FROM agrodata.EMMMILK emm 
                            WHERE emm.AniId = LED.AniId AND emm.DateMilking > DATE_ADD(LED.LastDate, INTERVAL -1 WEEK)) as ProdDaysWeek,
			           (SELECT sum(kgMilk) FROM agrodata.EMMMILK emm 
                            WHERE emm.AniId = LED.AniId AND emm.DateMilking > DATE_ADD(LED.LastDate, INTERVAL -1 MONTH)) as ProdMonth,
			           (SELECT count(distinct DateMilking) FROM agrodata.EMMMILK emm
                            WHERE emm.AniId = LED.AniId AND emm.DateMilking > DATE_ADD(LED.LastDate, INTERVAL -1 MONTH)) as ProdDaysMonth

                FROM agrodata.temptable_LastEmmDates LED", UbnId);

            return Parent.QueryData(qry_Mpr);

        }


        public DataTable getK01Animals(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT a.AniId, a.AniAlternateNumber,
                    (SELECT max(m.MovDate) FROM MOVEMENT m WHERE m.AniId = a.AniId AND m.MovKind = 1 AND m.UbnId = {1}) as LastArrival,
		            (SELECT max(m.MovDate) FROM MOVEMENT m JOIN SALE s ON m.MovId = s.MovId WHERE (s.SalKind NOT IN (2, 3) OR s.SalKind is null)
                        AND m.AniId = a.AniId AND m.UbnId = {1}) as LastSaleLive, 
		            (SELECT max(m.MovDate) FROM MOVEMENT m JOIN SALE s ON m.MovId = s.MovId WHERE (s.SalKind IN (2, 3) OR s.SalSlaughter = 1)
                        AND m.AniId = a.AniId AND m.UbnId = {1}) as LastSaleCulling,
		            (SELECT max(m.MovDate) FROM MOVEMENT m WHERE m.MovKind = 3 AND m.AniId = a.AniId AND m.UbnId = {1}) as LossDate,
                    (SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND 
                        d.disMainCode = -99 AND d.disSubCode = 511 AND e.AniId = a.AniId) as DisCode511,		 
		            (SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND 
                        d.disMainCode = -99 AND d.disSubCode = 512 AND e.AniId = a.AniId) as DisCode512,		 
            		(SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND
                        d.disMainCode = -99 AND d.disSubCode = 513 AND e.AniId = a.AniId) as DisCode513
                    FROM ANIMAL a
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.AniId > 0
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON a.AniId = ac.AniId


                    WHERE a.AniId > 0", MaxAniCategory, UbnId);

            return Parent.QueryData(qry, MissingSchemaAction.Add);
        }


        public DataTable getK01DyrLaktaAnimals(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT a.AniId, 
                    a.AniLifeNumber,
                    a.AniAlternateNumber,
                    a.AniBirthDate,
                    (SELECT sr.RacId FROM SECONRAC sr WHERE sr.AniId = a.AniId AND sr.SraRate >= 4 LIMIT 1) as PrimairyRace,
                    a.AniSex,
                    if ((SELECT count(*) FROM EVENT e WHERE e.EveKind = 5 AND e.AniId = a.AniId) > 0, 1, 0) as GaveBirth,
                        (SELECT max(m.MovDate) FROM MOVEMENT m WHERE m.AniId = a.AniId AND m.MovKind = 1 AND m.UbnId = {0}) as LastArrival,
                        (SELECT min(e.EveDate) FROM EVENT e JOIN BIRTH b ON e.EventId = b.EventId WHERE e.EveKind = 5 AND e.AniId = a.AniId 
                        AND (b.BirNumber = 1 || b.BirNumber = 0 || b.BirNumber is null)) 
                    as FirstBirDate,
                    ac.AniCategory, 
		            ac.AniWorkNumber,
		            am.AniLifeNumber as LifeNumberMother,
		            (SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND d.disMainCode = -99 
                        AND d.disSubCode = 60 AND e.AniId = a.AniId) as DisCode60,		 
		            (SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND d.disMainCode = -99 
                        AND d.disSubCode = 511 AND e.AniId                        = a.AniId) as DisCode511,		 
		            (SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND d.disMainCode = -99 
                        AND d.disSubCode = 512 AND e.AniId                        = a.AniId) as DisCode512,		 
            		(SELECT max(e.EveDate) FROM EVENT e JOIN DISEASE d ON e.EventId = d.EventId WHERE e.EveKind = 7 AND d.disMainCode = -99 
                        AND d.disSubCode = 513 AND e.                            AniId = a.AniId) as DisCode513,		 
		            (SELECT max(e.EveDate) FROM EVENT e JOIN GESTATIO g ON e.EventId = g.EventId WHERE e.EveKind = 3 AND g.GesStatus = 4 
                        AND e.AniId = a.AniId) AS BewustGust,
		            (SELECT max(m.MovDate) FROM MOVEMENT m JOIN SALE s ON m.MovId = s.MovId WHERE (s.SalKind NOT IN (2, 3) OR s.SalKind is null)
                        AND m.AniId = a.AniId AND m.UbnId = {0}) as LastSaleLive, 
		            (SELECT max(m.MovDate) FROM MOVEMENT m JOIN SALE s ON m.MovId = s.MovId WHERE (s.SalKind IN (2, 3) OR s.SalSlaughter = 1)
                        AND m.AniId = a.AniId AND m.UbnId = {0}) as LastSaleCulling,
		            (SELECT max(m.MovDate) FROM MOVEMENT m WHERE m.MovKind = 3 AND m.AniId = a.AniId AND m.UbnId = {0}) as LossDate 
		 		 																																						
                FROM ANIMAL a
                JOIN ANIMAL am ON a.AniIdMother = am.AniId
                JOIN 
                (
                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
                    FROM ANIMALCATEGORY aniCat
                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
                    WHERE  aniCat.AniCategory <= {1}
                    AND bedr.UbnId = {0}
                    AND aniCat.FarmId > 0
                    GROUP BY aniCat.AniId
                ) ac ON a.AniId = ac.AniId
                WHERE a.AniId > 0", UbnId, MaxAniCategory);
            return Parent.QueryData(qry, MissingSchemaAction.Add);
        }

        public DataTable getK01DyrLaktaCalves(int UbnId, int MaxAniCategory)
        {
            /*
             * PAS OP!
             * 
             * Bij aanpassen selectie velden moet de code in DyrLactaExport ook worden aangepast,
             * deze moet namelijk een distinct doen over alle velden behalve EventId, als je hier een veld toevoegd of verwijderd
             * moet dit dus ook gebeuren in DyrLactaExport
             */
            

            string qry = String.Format(
                  @"SELECT DISTINCT e.EventId, a.AniId as MotherAniId, e.EveDate, b.BirNumber, calf.AniId, calf.AniAlternateNumber, 
                    calf.AniNLing, calf.AniSex, father.AniAlternateNumber as fatherNumber, b.BornDead
       		 																																						
                    FROM BIRTH b
                    JOIN EVENT e ON b.EventId = e.EventId
                    JOIN ANIMAL a ON e.AniId = a.AniId
                    LEFT JOIN ANIMAL calf ON b.CalfId = calf.AniId
                    LEFT JOIN ANIMAL father ON calf.AniIdFather = father.AniId

                    JOIN 
                    (
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON a.AniId = ac.AniId
                    WHERE a.AniId > 0
                    AND e.EventId > 0
                    ", MaxAniCategory, UbnId);

            return Parent.QueryData(qry, MissingSchemaAction.Add);
        }



        public IEnumerable<WERKLIJST_HET_DEENS_SYSTEEM> getK01GoldSundWorkListActiveAnimals(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT distinct whds.*
                    FROM WERKLIJST_HET_DEENS_SYSTEEM whds
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON whds.AniId = ac.AniId
                    WHERE whds.UbnId = {1}", MaxAniCategory, UbnId);

            return Parent.getList<WERKLIJST_HET_DEENS_SYSTEEM>(Parent.QueryData(qry, MissingSchemaAction.Add));
        }

        public IEnumerable<AniIdDate> getK01GoldSundDryOffDates(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT e.AniId, e.EveDate FROM
                    EVENT e
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON e.AniId = ac.AniId
                    JOIN DRYOFF d ON e.EventId = d.EventId
                    WHERE e.EveKind = 4 
                    AND e.EventId > 0 AND e.AniId > 1", MaxAniCategory, UbnId);

            DataTable dt = Parent.QueryData(qry, MissingSchemaAction.Add);
            return Parent.dtEventToAniIdDate(dt);
        }


        public IEnumerable<AniIdDate> getK01YdlReproInseminations(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT e.AniId, if(e.EveKind = 2, e.EveDate, DATE_ADD(e.EveDate, INTERVAL -7 DAY)) as EveDate
                    FROM EVENT e
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON e.AniId = ac.AniId
                    WHERE e.EveKind IN (2, 9)
                    AND e.EventId > 0 AND e.AniId > 0 AND e.EveDate is not NULL", MaxAniCategory, UbnId);

            DataTable dt = Parent.QueryData(qry, MissingSchemaAction.Add);
            return Parent.dtEventToAniIdDate(dt);
        }

        public IEnumerable<AniIdDate> getK01YdlReproPosPregchecks(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT e.AniId, e.EveDate FROM
                    EVENT e
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON e.AniId = ac.AniId
                    JOIN GESTATIO g ON e.EventId = g.EventId
                    WHERE e.EveKind = 3 
                    AND g.GesStatus IN (1, 2)
                    AND e.EventId > 0 AND e.AniId > 0 AND e.EveDate is not null", MaxAniCategory, UbnId);

            DataTable dt = Parent.QueryData(qry, MissingSchemaAction.Add);
            return Parent.dtEventToAniIdDate(dt);
        }

        public IEnumerable<AniIdDate> getK01YdlReproNegPregchecks(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT e.AniId, e.EveDate FROM
                    EVENT e
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON e.AniId = ac.AniId
                    JOIN GESTATIO g ON e.EventId = g.EventId
                    WHERE e.EveKind = 3 
                    AND g.GesStatus = 3
                    AND e.EventId > 0 AND e.AniId > 0 AND e.EveDate is not NULL", MaxAniCategory, UbnId);

            DataTable dt = Parent.QueryData(qry, MissingSchemaAction.Add);
            return Parent.dtEventToAniIdDate(dt);
        }

        public IEnumerable<ANALYSE> getK01YdlReproMpr(int UbnId, bool UseNonOriginal)
        {
            string filter = "0 , 6";
            if (UseNonOriginal)
                filter = "0, 3, 6";

            string qry = String.Format(
                  @"SELECT *
                    FROM agrobase.ANALYSE
#                  JOIN 
#                  (
#	                    SELECT MIN(ana2.AnaMilkDate) md
#	                    FROM agrobase.ANALYSE ana2
#	                    WHERE UbnId = {0}
#	                    GROUP BY Year(ana2.AnaMilkDate), Month(ana2.AnaMilkDate)
#                    ) bla2 ON AnaMilkDate = bla2.md
                    WHERE UbnId = {0}
                    AND AnaTypeOfControl IN (null, {1})
					AND AnaKgMilk is not null
                    AND AnaKgMilk > 0
                    AND AniId > 0", UbnId, filter);

            DataTable dt = Parent.QueryData(qry);
            return Parent.getList<ANALYSE>(dt);
        }

        public IEnumerable<AniIdDate> getK01YdlReproCalvingDates(int UbnId, int MaxAniCategory)
        {
            string qry = String.Format(
                  @"SELECT e.AniId, e.EveDate FROM
                    EVENT e
                    JOIN 
                    (   
	                    SELECT aniCat.AniId, aniCat.AniCategory, aniCat.AniWorkNumber
	                    FROM ANIMALCATEGORY aniCat
	                    JOIN agrofactuur.BEDRIJF bedr ON aniCat.FarmId = bedr.FarmId
	                    WHERE  aniCat.AniCategory <= {0}
	                    AND bedr.UbnId = {1}
                        AND aniCat.FarmId > 0
                        GROUP BY aniCat.AniId
                    ) ac ON e.AniId = ac.AniId
                    JOIN BIRTH b ON e.EventId = b.EventId
                    WHERE e.EveKind = 5
                    AND e.EventId > 0 AND e.AniId > 0 AND e.EveDate is not NULL", MaxAniCategory, UbnId);

            DataTable dt = Parent.QueryData(qry, MissingSchemaAction.Add);
            return Parent.dtEventToAniIdDate(dt);
        }

    }
}
