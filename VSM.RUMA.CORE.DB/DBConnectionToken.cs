using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MySql.Data.MySqlClient;
using VSM.RUMA.CORE;
using VSM.RUMA.CORE.DB;

namespace VSM.RUMA.CORE
{
    [Serializable()]
    [DataContract]
    public abstract class DBConnectionToken : ICloneable
    {
        [DataMember]
        private Dictionary<int, DBConnectionToken> mChildren = new Dictionary<int, DBConnectionToken>();
        [DataMember]
        private String mMasterDB, mMasterIP, mMasterUser, mMasterPass, mSlaveDB, mSlaveIP, mSlaveUser, mSlavePass;


        private string _slaveconnectionstring = null;
        public string SlaveConnectionString
        {
            get
            {
                if (_slaveconnectionstring == null)
                {
                    MySqlConnectionStringBuilder lConnectionString;
                    if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
                    {
                        if (!MasterConnectionString.Contains("Allow User Variables=True"))
                        {
                            return MasterConnectionString + ";Allow User Variables=True;";
                        }
                        return MasterConnectionString;
                    }
                    if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ConnectionOptions"]))
                    {
                        lConnectionString = new MySqlConnectionStringBuilder();
                        lConnectionString.UseCompression = true;
                        lConnectionString.SslMode = MySqlSslMode.Preferred;
                        lConnectionString.AllowZeroDateTime = true;
                        lConnectionString.AllowUserVariables = true;

                    }
                    else
                    {
                        lConnectionString = new MySqlConnectionStringBuilder(ConfigurationManager.AppSettings["ConnectionOptions"]);
                    }
                    if (String.IsNullOrEmpty(lConnectionString.UserID))
                    {
                        lConnectionString.UserID = SlaveUser;
                        lConnectionString.Password = SlavePass;
                    }
                    else
                    {
                        dbTokenSettingsDecrypter Decrypter = new dbTokenSettingsDecrypter();
                        Decrypter.DecryptConfigPW(ref lConnectionString);
                    }


                    lConnectionString.Server = SlaveIP;
                    lConnectionString.Database = SlaveDB;

                    _slaveconnectionstring = lConnectionString.ToString();
                }
                return _slaveconnectionstring;
            }
        }

        //a;sljkdfa;slkdfad;ljkfas
        //dan maar ook hierheen copieren, serieus.
        public class MaskingClass
        {

            private string geldigetekens;
            private String KEYPHRASE;
            private String dbww;

            protected MaskingClass(String pSleutel)
            {
                dbww = base64Decode(pSleutel);
                KEYPHRASE = "aYHJsucCyuSxgesWsfBoPODkldmwuHUdoieAKMswoODoSWsldksmaadsfazDIsl";
                geldigetekens = "xGd6Ok03gNpKy5uBcSFrWiHa7z2LfJvMlPnRqYtAh1DeVCjQ9ZoTI4UbEsXw8m";
            }

            internal MaskingClass(String pVersleuteltekst, String pVersleutelString)
            {
                dbww = pVersleuteltekst;
                KEYPHRASE = "aYHJsucCyuSxgesWsfBoPODkldmwuHUdoieAKMswoODoSWsldksmaadsfazDIsl";
                geldigetekens = pVersleutelString;
            }

            private string Sleutel_Ww()
            {

                int i = 0;
                int chars = (dbww.Length / 2) - 1;
                string result = String.Empty;
                for (i = 0; i <= chars; i++)
                {
                    result = result + dbww.Substring((dbww.Length - i * 2) - 1, 1);
                }
                return result;
            }

            #region Masking
            protected string base64Encode(string data)
            {
                try
                {
                    byte[] encData_byte = new byte[data.Length];
                    encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                    string encodedData = Convert.ToBase64String(encData_byte);
                    return encodedData;
                }
                catch (Exception e)
                {
                    //unLogger.WriteInfo("Error in base64Encode" + e.ToString());

                    throw new Exception("Error in base64Encode" + e.Message);
                }
            }

            protected string base64Decode(string data)
            {
                try
                {
                    if (!String.IsNullOrEmpty(data))
                    {
                        byte[] encData_byte = Convert.FromBase64String(data);
                        string decodedData = System.Text.Encoding.UTF8.GetString(encData_byte);
                        return decodedData;
                    }
                    return data;
                }
                catch (System.ArgumentNullException)
                {
                    // Luc, als je een exeption throwd, loopt dan niet 
                    // het werkprocess vast? en dus de webserver?
                    // heb deze 2 catchen gevonden voor meer duidelijkheid.
   /////////////////                 unLogger.WriteDebug("masking.cs-base64Decode: Error:Base 64 string is null.");
                    return data;
                }
                catch (System.FormatException)
                {
                    //unLogger.WriteDebug("masking.cs-base64Decode: Error:Base 64 string length is not " +
                    //    "4 or is not an even multiple of 4.");
                    return data;
                }
                catch (Exception e)
                {
                    ///////////unLogger.WriteError("Error in base64Encode" + e.ToString());

                    throw new Exception("Error in base64Decode" + e.Message);
                }


            }

            protected string PrepareSOAPString(string pInputString)
            {
                return base64Encode(XOREncryption(pInputString, KEYPHRASE));
            }

            private string XOREncryption(string pInputString, string KeyPhrase)
            {
                StringBuilder sbInputString = new StringBuilder(pInputString);
                int KeyPhraseLength = KeyPhrase.Length;
                for (int i = 0, strlen = sbInputString.Length; i < strlen; ++i)
                {
                    int rPos = i % KeyPhraseLength;
                    int r = Convert.ToInt32(sbInputString[i]) ^ Convert.ToInt32(KeyPhrase[rPos]);
                    sbInputString[i] = System.Convert.ToChar(r);
                }
                return sbInputString.ToString();
            }


            protected string Codeer_Base64String(string UTFTekst)
            {
                string Tekst = base64Encode(UTFTekst);
                string lettersbase64 = "xGd6Ok03gNpKy5uBcSFrWiHa7z2L+fJvMlPnRqYtAh1De/VCjQ9ZoTI4UbE=sXw8m";

                int pz, ps, i;
                String sleutel;
                sleutel = Sleutel_Ww();
                string result = sleutel;

                while (result.Length < Tekst.Length)
                    result = result + sleutel;

                sleutel = result;
                result = string.Empty;

                for (i = 0; i < Tekst.Length; i++)
                {
                    pz = lettersbase64.IndexOf(Tekst[i]) + 1;
                    ps = lettersbase64.IndexOf(sleutel[i]) + 1;
                    result = result + lettersbase64[(pz + ps - 1) % lettersbase64.Length];
                }
                return result;
            }

            protected string DeCodeerBase64_String(string Tekst)
            {
                if (Tekst != "true" && Tekst != "false")
                {
                    string lettersbase64 = "xGd6Ok03gNpKy5uBcSFrWiHa7z2L+fJvMlPnRqYtAh1De/VCjQ9ZoTI4UbE=sXw8m";

                    int pz, ps, i;
                    String sleutel;
                    sleutel = Sleutel_Ww();
                    string result = sleutel;

                    while (result.Length < Tekst.Length)
                        result = result + sleutel;

                    sleutel = result;
                    result = string.Empty;

                    for (i = 0; i < Tekst.Length; i++)
                    {
                        pz = lettersbase64.IndexOf(Tekst[i]) + 1;
                        ps = lettersbase64.IndexOf(sleutel[i]) + 1;
                        result = result + lettersbase64[(pz - ps - 1 + lettersbase64.Length) % lettersbase64.Length];
                    }
                    return base64Decode(result);
                }
                return Tekst;
            }


            protected string Codeer_String(string Tekst)
            {


                int pz, ps, i;
                String sleutel;
                sleutel = Sleutel_Ww();
                string result = sleutel;

                while (result.Length < Tekst.Length)
                    result = result + sleutel;

                sleutel = result;
                result = string.Empty;

                for (i = 0; i < Tekst.Length; i++)
                {
                    if (char.IsLetterOrDigit(Tekst[i]))
                    {
                        pz = geldigetekens.IndexOf(Tekst[i]) + 1;
                        ps = geldigetekens.IndexOf(sleutel[i]) + 1;
                        result = result + geldigetekens[(pz + ps - 1) % geldigetekens.Length];
                    }
                }
                return result;
            }


            public string DeCodeer_String(string Tekst)
            {


                int pz, ps, i;
                String sleutel;
                sleutel = Sleutel_Ww();
                string result = sleutel;

                while (result.Length < Tekst.Length)
                    result = result + sleutel;

                sleutel = result;
                result = string.Empty;

                for (i = 0; i < Tekst.Length; i++)
                {
                    if (char.IsLetterOrDigit(Tekst[i]))
                    {
                        pz = geldigetekens.IndexOf(Tekst[i]) + 1;
                        ps = geldigetekens.IndexOf(sleutel[i]) + 1;
                        result = result + geldigetekens[(pz - ps - 1 + geldigetekens.Length) % geldigetekens.Length];
                    }
                }
                return result;
            }
            #endregion
        }

        public class dbTokenSettingsDecrypter : MaskingClass
        {

            //public SettingsDecrypter()
            //    : base("cm51ZW1rYXJkYWJ2")
           // {
           //
//            }

            public dbTokenSettingsDecrypter() : base("cm51ZW1rYXJkYWJ2")
            {

            }

            internal void DecryptConfigPW(ref MySql.Data.MySqlClient.MySqlConnectionStringBuilder ConnectionString)
            {
                ConnectionString.Password = DeCodeer_String(ConnectionString.Password);
            }
        }

        private string _masterConnectionString = null;
        public string MasterConnectionString
        {
            get
            {
                if (_masterConnectionString == null)
                {
                    MySqlConnectionStringBuilder lConnectionString;
                    if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null
                        && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString.ToLower().Contains("user id=")
                        && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString.ToLower().Contains("pwd="))
                    {

                        try
                        {
                            char[] split1 = { ';' };
                            char[] split2 = { '=' };
                            StringBuilder connstring = new StringBuilder();

                            string[] splitconnstring = ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString.Split(split1);
                            for (int i = 0; i < splitconnstring.Length; i++)
                            {
                                if (splitconnstring[i].ToLower().StartsWith("user id=") || splitconnstring[i].ToLower().StartsWith("pwd="))
                                {
                                    string[] pwd = splitconnstring[i].Split(split2);
                                    if (pwd.Length == 2)
                                    {
                                        connstring.Append(pwd[0] + "=" + base64Decode(pwd[1]));
                                    }
                                }
                                else
                                {
                                    connstring.Append(splitconnstring[i]);
                                }
                                connstring.Append(";");

                            }

                            lConnectionString = new MySqlConnectionStringBuilder(connstring.ToString());
                            //test
                            MySqlConnection conn = new MySqlConnection(lConnectionString.ToString());
                            conn.Open();
                            conn.Close();
                        }
                        catch
                        {
                            lConnectionString = new MySqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString);

                        }

                    }
                    else
                    {
                        if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ConnectionOptions"]))
                        {
                            lConnectionString = new MySqlConnectionStringBuilder();
                            lConnectionString.UseCompression = true;
                            lConnectionString.SslMode = MySqlSslMode.Preferred;
                            lConnectionString.AllowZeroDateTime = true;
                            lConnectionString.AllowUserVariables = true;
                        }
                        else lConnectionString = new MySqlConnectionStringBuilder(ConfigurationManager.AppSettings["ConnectionOptions"]);
                        if (String.IsNullOrEmpty(lConnectionString.UserID))
                        {
                            lConnectionString.UserID = MasterUser;
                            lConnectionString.Password = MasterPass;
                        }
                        else
                        {
                            dbTokenSettingsDecrypter Decrypter = new dbTokenSettingsDecrypter();
                            Decrypter.DecryptConfigPW(ref lConnectionString);
                        }
                        lConnectionString.Server = MasterIP;
                    }
                    lConnectionString.Database = MasterDB;
                    lConnectionString.AllowUserVariables = true;
                    _masterConnectionString = lConnectionString.ToString();
                }
                return _masterConnectionString;
            }
        }




        protected DBConnectionToken(String pMasterDB, String pMasterIP,
                                    String pMasterUser, String pMasterPass,
                                    String pSlaveDB, String pSlaveIP,
                                    String pSlaveUser, String pSlavePass)
        {

            mMasterDB = base64Encode(pMasterDB);
            mMasterIP = base64Encode(pMasterIP);
            mMasterUser = base64Encode(pMasterUser);
            mMasterPass = base64Encode(pMasterPass);
            mSlaveDB = base64Encode(pSlaveDB);
            mSlaveIP = base64Encode(pSlaveIP);
            mSlaveUser = base64Encode(pSlaveUser);
            mSlavePass = base64Encode(pSlavePass);
            
            Globalid = Guid.NewGuid();
        }

        public string MasterDB
        {
            get
            {
                return base64Decode(mMasterDB);
            }
        }

        public string MasterIP
        {
            get
            {
                return base64Decode(mMasterIP);
            }
        }

        public string MasterUser
        {
            get
            {
                return base64Decode(mMasterUser);
            }
        }

        public string MasterPass
        {
            get
            {
                return base64Decode(mMasterPass);
            }
        }

        public string SlaveDB
        {
            get
            {
                return base64Decode(mSlaveDB);
            }
        }

        public string SlaveIP
        {
            get
            {
                return base64Decode(mSlaveIP);
            }
        }

        public string SlaveUser
        {
            get
            {
                return base64Decode(mSlaveUser);
            }
        }

        public string SlavePass
        {
            get
            {
                return base64Decode(mSlavePass);
            }
        }

        [DataMember]
        private int lastprogramid = 0;

        [DataMember]
        private Guid Globalid;

        public string getDBNameSlave()
        {
            return SlaveDB;
        }

        internal bool HasAnimalDatabase()
        {
            return mChildren.Count > 0;
        }

        public void AddChildConnection(int pProgramId, DBConnectionToken pChild)
        {
            if (!mChildren.ContainsKey(pProgramId))
            {
                mChildren.Add(pProgramId, pChild);
                lastprogramid = 0;
            }
            else if (!mChildren[pProgramId].Equals(pChild))
            {
                mChildren.Remove(pProgramId);
                mChildren.Add(pProgramId, pChild);
                lastprogramid = pProgramId;
            }
            else lastprogramid = pProgramId;
        }


        public string getStatsConnection(string db_host)
        {
            MySqlConnectionStringBuilder lConnectionString = new MySqlConnectionStringBuilder();
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
            {
                return MasterConnectionString;
            }
            else
            {

                lConnectionString.Server = db_host;
                lConnectionString.UserID = MasterUser;
                lConnectionString.Password = MasterPass;
                return lConnectionString.ToString();

            }
        }

        public DBConnectionToken getChildConnection(int pProgramId)
        {
            if (!mChildren.ContainsKey(pProgramId)) throw new NullReferenceException("DB voor ProgramId " + pProgramId.ToString() + " niet geïnitialiseerd");
            lastprogramid = pProgramId;
            return mChildren[pProgramId];
        }

        public DBConnectionToken getLastChildConnection()
        {
            if (mChildren.Count == 0)
            {
//                throw new NotSupportedException("Geen DierToken in administratie");
                return this;
            }
            if (lastprogramid != 0) return getChildConnection(lastprogramid);
            return mChildren.Last().Value;
        }



        #region Masking

        private string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];

                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);

                string encodedData = Convert.ToBase64String(encData_byte.Reverse().ToArray());
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode " + ex.Message,ex);
            }
        }

        private string base64Decode(string data)
        {
            try
            {
                byte[] encData_byte = Convert.FromBase64String(data);
                string decodedData = System.Text.Encoding.UTF8.GetString(encData_byte.Reverse().ToArray());
                return decodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message,ex);
            }
        }
        #endregion

        [Obsolete]
        public virtual bool SameConnection(DBConnectionToken obj)
        {
            if (obj.MasterDB == this.MasterDB &&
                obj.MasterIP == this.MasterIP &&
                obj.MasterUser == this.MasterUser &&
                obj.MasterPass == this.MasterPass &&
                obj.SlaveDB == this.SlaveDB &&
                obj.SlaveIP == this.SlaveIP &&
                obj.SlaveUser == this.SlaveUser &&
                obj.SlavePass == this.SlavePass &&
                obj.Globalid == this.Globalid)
            {
                return true;
            }
            else return false;
        }


        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() == typeof(DBConnectionToken))
        //        return this.SameConnection((DBConnectionToken)obj);
        //    else
        //        return base.Equals(obj);
        //}

        [Obsolete]
        public object Clone()
        {
            DBConnectionToken lCloned = (DBConnectionToken)this.MemberwiseClone();
            lCloned.mChildren = new Dictionary<int, DBConnectionToken>();
            foreach(KeyValuePair<int,DBConnectionToken> Child in mChildren)
            {
                lCloned.mChildren.Add(Child.Key, (DBConnectionToken)Child.Value.Clone());
            }
            lCloned.Globalid = Guid.NewGuid();
            return lCloned;
        }

    }
}
