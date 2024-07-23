using System;
using System.Runtime.Serialization;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE
{
    [Serializable()]
    [DataContract]
    [KnownType(typeof(DBConnectionToken))]
    public class FarmUserRightsToken : DBConnectionToken
    {
        [DataMember]
        private String mUBNId;
        [DataMember]
        private String mProgId;
        [DataMember]
        private String mProgramId;
        [DataMember]
        private String mFarmId;
        internal FarmUserRightsToken(ref Facade pFacade, BEDRIJF pFarm,
                                    String pMasterDB, String pMasterIP,
                                    String pMasterUser, String pMasterPass,
                                    String pSlaveDB, String pSlaveIP,
                                    String pSlaveUser, String pSlavePass) 
            : base(pMasterDB, pMasterIP, pMasterUser, pMasterPass, pSlaveDB, pSlaveIP, pSlaveUser, pSlavePass)
        {
            mProgramId = pFacade.getRechten().Codeer_String(pFarm.Programid.ToString());
            mUBNId = pFacade.getRechten().Codeer_String(pFarm.UBNid.ToString());
            mFarmId = pFacade.getRechten().Codeer_String(pFarm.FarmId.ToString());
            mProgId = pFacade.getRechten().Codeer_String(pFarm.ProgId.ToString());
        }

        public override bool SameConnection(DBConnectionToken obj)
        {
            if (obj.GetType() == typeof(FarmUserRightsToken))
            {
                FarmUserRightsToken FRT = ((FarmUserRightsToken)obj);
                if (
                    FRT.UBNId == this.UBNId &&
                    FRT.FarmId == this.FarmId &&
                    FRT.ProgId == this.ProgId &&
                    FRT.ProgramId == this.ProgramId &&
                    base.SameConnection(obj))
                {
                    return true;
                }
            }
            return false;
        }


        #region Properties


        public int UBNId
        {
            get
            {
                return Convert.ToInt32(Facade.GetInstance().getRechten().DeCodeer_String(mUBNId));
            }
        }

        public int FarmId
        {
            get
            {
                return Convert.ToInt32(Facade.GetInstance().getRechten().DeCodeer_String(mFarmId));
            }
        }

        public int ProgId
        {
            get
            {
                return Convert.ToInt32(Facade.GetInstance().getRechten().DeCodeer_String(mProgId));
            }
        }

        public int ProgramId
        {
            get
            {
                return Convert.ToInt32(Facade.GetInstance().getRechten().DeCodeer_String(mProgramId));
            }
        }


        #endregion

    }
}
