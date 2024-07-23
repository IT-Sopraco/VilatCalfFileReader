using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB
{
    /// <summary>
    /// Helper class as Key for a dictionairy containing AnimalCategories
    /// </summary>
    public class AniCatKey : IEquatable<AniCatKey>
    {
        public int FarmId { get; }
        public int AniId { get; }
        public int? OldAniCat { get; }
        public bool ForceUpdate { get; set; }


        public AniCatKey(int farmId, int aniId, int? oldAniCat = null, bool forceUpdate = false)
        {
            this.FarmId = farmId;
            this.AniId = aniId;
            this.OldAniCat = oldAniCat;
            this.ForceUpdate = forceUpdate;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + FarmId;
            hash = hash * 31 + AniId;
            return hash;
        }

        public override string ToString()
        {
            return $"FarmId: {FarmId} AniId: {AniId} OldCat: {OldAniCat} ForceUpdate: {ForceUpdate}";
        }

        public override bool Equals(object other)
        {
            var aniCatKey = other as AniCatKey;
            return aniCatKey != null && Equals(aniCatKey);
        }

        public bool Equals(AniCatKey other)
        {
            return other != null && (FarmId == other.FarmId && AniId == other.AniId);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AniCatKeyComparer : IEqualityComparer<AniCatKey>
    {
        public bool Equals(AniCatKey ack1, AniCatKey ack2)
        {
            if (ack1.FarmId == ack2.FarmId && ack1.AniId == ack2.AniId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(AniCatKey bx)
        {
            int hCode = bx.FarmId ^ bx.AniId;
            return hCode.GetHashCode();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnimalCategoryComparer : IEqualityComparer<ANIMALCATEGORY>
    {
        public bool Equals(ANIMALCATEGORY ac1, ANIMALCATEGORY ac2)
        {
            if (ac1.FarmId == ac2.FarmId && ac1.AniId == ac2.AniId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(ANIMALCATEGORY bx)
        {
            int hCode = bx.FarmId ^ bx.AniId;
            return hCode.GetHashCode();
        }
    }
}