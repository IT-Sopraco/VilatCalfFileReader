using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Data;

namespace VSM.RUMA.CORE.DB.MYSQL.DBSQ
{
    public class DBSQAnimal
    {
        private DBSelectQueries Parent;

        public DBSQAnimal(DBSelectQueries parent)
        {
            this.Parent = parent;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAniId"></param>
        /// <returns></returns>
        public List<ANIMAL> getAnimalAndGreatGrandParents(int pAniId)
        {
            StringBuilder lQuery = new StringBuilder();
            lQuery.Append(@" 
                    #kind
                    SELECT 'c' AS Ancestor, a.* FROM ANIMAL a 
                    WHERE a.AniId = " + pAniId.ToString() + @"
                    # m
                     UNION 
                     SELECT 'm' AS Ancestor,  m.* FROM ANIMAL m 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    # f
                     UNION 
                     SELECT 'f' AS Ancestor,  f.* FROM ANIMAL f 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #mm
                      UNION 
                     SELECT 'mm' AS Ancestor,  mm.* FROM ANIMAL mm 
                     INNER JOIN ANIMAL m ON m.AniIdMother = mm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #mf
                      UNION 
                     SELECT 'mf' AS Ancestor,  mf.* FROM ANIMAL mf 
                     INNER JOIN ANIMAL m ON m.AniIdFather = mf.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #fm
                      UNION 
                     SELECT 'fm' AS Ancestor,  fm.* FROM ANIMAL fm 
                     INNER JOIN ANIMAL f ON f.AniIdMother = fm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #ff
                      UNION 
                     SELECT 'ff' AS Ancestor,  ff.* FROM ANIMAL ff 
                     INNER JOIN ANIMAL f ON f.AniIdFather = ff.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"

                     #REST A
                    #mm ->  mmm
                      UNION 
                     SELECT 'mmm' AS Ancestor,  mmm.* FROM ANIMAL mmm 
                     INNER JOIN ANIMAL mm ON mm.AniIdMother = mmm.AniId 
                     INNER JOIN ANIMAL m ON m.AniIdMother = mm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #mm ->  mmf
                      UNION 
                     SELECT 'mmf' AS Ancestor,  mmf.* FROM ANIMAL mmf 
                     INNER JOIN ANIMAL mm ON mm.AniIdFather = mmf.AniId 
                     INNER JOIN ANIMAL m ON m.AniIdMother = mm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"

                    #mf ->  mfm
                      UNION 
                     SELECT 'mfm' AS Ancestor,  mfm.* FROM ANIMAL mfm 
                     INNER JOIN ANIMAL mm ON mm.AniIdMother = mfm.AniId 
                     INNER JOIN ANIMAL m ON m.AniIdFather = mm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #mf ->  mff
                      UNION 
                     SELECT 'mff' AS Ancestor,  mff.* FROM ANIMAL mff 
                     INNER JOIN ANIMAL mf ON mf.AniIdFather = mff.AniId 
                     INNER JOIN ANIMAL m ON m.AniIdFather = mf.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdMother = m.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"

                    #REST B 
                    #fm -> fmm
                      UNION 
                     SELECT 'fmm' AS Ancestor,  fmm.* FROM ANIMAL fmm 
                     INNER JOIN ANIMAL fm ON fm.AniIdMother = fmm.AniId
                     INNER JOIN ANIMAL f ON f.AniIdMother = fm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #fm -> fmf 
                      UNION 
                     SELECT 'fmf' AS Ancestor,  fmf.* FROM ANIMAL fmf 
                     INNER JOIN ANIMAL fm ON fm.AniIdFather = fmf.AniId
                     INNER JOIN ANIMAL f ON f.AniIdMother = fm.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #ff -> ffm
                      UNION 
                     SELECT 'ffm' AS Ancestor,  ffm.* FROM ANIMAL ffm 
                     INNER JOIN ANIMAL ff ON ff.AniIdMother = ffm.AniId
                     INNER JOIN ANIMAL f ON f.AniIdFather = ff.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"
                    #ff -> fff
                      UNION 
                     SELECT 'fff' AS Ancestor,  fff.* FROM ANIMAL fff 
                     INNER JOIN ANIMAL ff ON ff.AniIdFather = fff.AniId 
                     INNER JOIN ANIMAL f ON f.AniIdFather = ff.AniId 
                     INNER JOIN ANIMAL a ON a.AniIdFather = f.AniId 
                     WHERE  a.AniId = " + pAniId.ToString() + @"

            ");

            DataTable tbl = Parent.QueryData(lQuery.ToString(), MissingSchemaAction.Add);
            List<ANIMAL> lResult = new List<ANIMAL>();
            foreach (DataRow rw in tbl.Rows)
            {
                ANIMAL a = new ANIMAL();
                Parent.FillObject(a, rw);
                lResult.Add(a);
            }
            return lResult;
        }

        /// <summary>
        /// Returns all distinct animals having a ANIMALCATEGORY for the ubn
        /// </summary>
        /// <param name="ubn">Bedrijfsnummer</param>
        /// <param name="MaxAniCategory">Maximale anicategory van op te vragen dieren, null is geen filtering</param>
        /// <returns></returns>
        public IEnumerable<ANIMAL> GetAnimalsByUBN(string ubn, int? MaxAniCategory = null)
        {
            string qry = @" SELECT DISCTINCT a.* FROM ANIMAL a
                            JOIN ANIMALCATEGORY ac ON a.AniId = ac.AniId
                            JOIN agrofactuur.BEDRIJF b ON ac.FarmId = b.FarmId
                            JOIN agrofactuur.UBN u ON b.UbnId = u.UbnId

                            WHERE a.AniId > 0 
                            AND b.FarmId > 0 
                            AND u.UbnId > 0
                            
                            AND u.Bedrijfsnummer = ?ubn";

            if (MaxAniCategory.HasValue)
                qry += string.Format(" AND ac.Anicategory <= {0}", MaxAniCategory.Value);
                        
            return Parent.getList<ANIMAL>(Parent.QueryData(qry));
        }

        /// <summary>
        /// Get ANIMAL by ID.
        /// </summary>
        /// <param name="pAnimalId"></param>
        /// <returns></returns>
        public ANIMAL GetAnimalById(int pAnimalId)
        {
            string sql = $"SELECT ANIMAL.* FROM ANIMAL WHERE AniId = '{pAnimalId}'";
            return Parent.getSingleItem<ANIMAL>(sql);
        }

    }
}
