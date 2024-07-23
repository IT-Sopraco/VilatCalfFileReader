using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE
{
    [Serializable()]
    [DataContract]
    [KnownType(typeof(FarmUserRightsToken))]
    [KnownType(typeof(DBConnectionToken))]
    public class UserRightsToken : DBConnectionToken
    {
        [DataMember]
        private String mUser;
        [DataMember]
        private String mPassword;
        [DataMember]
        private String mHost;

        [DataMember]
        private UserRightsTokenContext mContext = new UserRightsTokenContext();

        public UserRightsToken(ref Facade pFacade, String pUser,
                                    String pPassword,
                                    String pMasterDB, String pMasterIP,
                                    String pMasterUser, String pMasterPass,
                                    String pSlaveDB, String pSlaveIP,
                                    String pSlaveUser, String pSlavePass)
            : base(pMasterDB, pMasterIP, pMasterUser, pMasterPass, pSlaveDB, pSlaveIP, pSlaveUser, pSlavePass)
        {
            mUser = pFacade.getRechten().Codeer_String(pUser);
            mPassword = pFacade.getRechten().Codeer_String(pPassword);
            mHost = pMasterIP;
        }
        #region Properties

        internal String User
        {
            get
            {
                return Facade.GetInstance().getRechten().DeCodeer_String(mUser);
            }
        }

        internal String Password
        {
            get
            {
                return Facade.GetInstance().getRechten().DeCodeer_String(mPassword);
            }
        }

        internal String Host
        {
            get { return mHost; }
        }
        public FarmUserRightsToken getChildConnection(int pProgramId)
        {
            return (FarmUserRightsToken)base.getChildConnection(pProgramId);
        }
        #endregion



        public override bool SameConnection(DBConnectionToken obj)
        {
            if (obj.GetType() == typeof(UserRightsToken))
            {
                UserRightsToken URT = ((UserRightsToken)obj);
                if (
                    URT.User == this.User &&
                    URT.Password == this.Password &&
                    URT.Host == this.Host &&
                    base.SameConnection(obj))
                {
                    return true;
                }
            }
            return false;
        }

    }

    [Serializable()]
    [DataContract]
    [Obsolete("vervangen door agrolink?")]
    public class UserRightsTokenContext
    {
        public int FarmId;
        public int UBNId;
        public int ProgId;
        public int ProgramId;
        public int ThrId;
        public int AdminUser;
        public string AgroUser;
        public string AgroPassword;
        public string SessionId;
        public List<string> tabbladen;
        public List<int> ondergeschikten;
        public List<int> events;
        public List<int> movements;
        public List<string> MainTreenodes;
        public List<string> SubTreenodes;
        public List<string> MainMenunodes;
        public List<string> SubMenunodes;
        public List<string> LijstenMaintreenodes;
        public List<string> LijstenSubtreenodes;
        public UserRightsToken AgroRightsToken;
        public string refIPadress;
        public string refDNSname;
        public string refBrowser;
        public int Animal_AniId;
        public int Animal_FarmId;
        public int Animal_UBNId;
        public int Animal_ProgId;
        public int Animal_ProgramId;
        public int isgeteld;
        public string thema;

        // VOOR KLANTLINK //
        public int GastThrId;
        public bool RechtInsert;
        public bool RechtDelete;
        public bool RechtEdit;
    }

}
