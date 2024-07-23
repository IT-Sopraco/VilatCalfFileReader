using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB
{
    /// <summary>
    /// Helper class as Key for a dictionairy containing Animals. 
    /// </summary>
    public class AnimalKey : IEquatable<AnimalKey>
    {
        private readonly string _lifeNumber;

        /// <summary>
        /// Lifenumber of the Animal
        /// </summary>
        public string LifeNumber => _lifeNumber;

        /// <summary>
        /// Field to specifiy if the ANIMAL specified by the AnimalKey should be saved to the DB
        /// </summary>
        public bool ForceUpdate { get; set; }

        public AnimalKey(string lifeNumber, bool forceUpdate = false)
        {
            this._lifeNumber = lifeNumber;
            this.ForceUpdate = forceUpdate;
        }

        public override int GetHashCode()
        {
            return _lifeNumber.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var ak = other as AnimalKey;
            return ak != null && Equals(ak);
        }

        public bool Equals(AnimalKey other)
        {            
            return other != null && _lifeNumber == other._lifeNumber;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnimalKeyComparer : IEqualityComparer<AnimalKey>
    {
        public bool Equals(AnimalKey ak1, AnimalKey ak2)
        {
            return (ak1.LifeNumber == ak2.LifeNumber);
        }

        public int GetHashCode(AnimalKey ak)
        {
            return ak.LifeNumber.GetHashCode();
        }
    }
}