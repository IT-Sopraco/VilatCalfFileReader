using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE.DB.DataTypes;

namespace VSM.RUMA.CORE.DB
{
    public class SettingsDecrypter : MaskingClass
    {

        public SettingsDecrypter()
            : base("cm51ZW1rYXJkYWJ2")
        {

        }

        internal void DecryptConfigPW(ref MySql.Data.MySqlClient.MySqlConnectionStringBuilder ConnectionString)
        {
            ConnectionString.Password = DeCodeer_String(ConnectionString.Password);
        }


        internal void EncryptFtpUser(ref FTPUSER pFtpUser)
        {
            String EncryptedValue = pFtpUser.Password;
            pFtpUser.Password = Codeer_Base64String(EncryptedValue + "RUMACRYPT");
        }

        internal void DecryptFtpUser(ref FTPUSER pFtpUser)
        {
            String Value = pFtpUser.Password;
            try
            {
                if (pFtpUser.Password.Length % 4 == 0)
                {
                    String EncryptedValue = DeCodeerBase64_String(pFtpUser.Password);
                    if (EncryptedValue.EndsWith("RUMACRYPT"))
                    {
                        pFtpUser.Password = EncryptedValue.Substring(0, EncryptedValue.LastIndexOf("RUMACRYPT"));
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(String.Format("DecryptFtpUser Value = {0}", Value), ex);
            }
        }

        internal void EncryptAnimalPassword(ref ANIMALPASSWORD pAnimalPassword)
        {
            String EncryptedValue = pAnimalPassword.AP_Password;
            pAnimalPassword.AP_Password = Codeer_Base64String(EncryptedValue + "RUMACRYPT");
        }

        internal void DecryptAnimalPassword(ref ANIMALPASSWORD pAnimalPassword)
        {
            String Value = pAnimalPassword.AP_Password;
            try
            {
                if (pAnimalPassword.AP_Password.Length % 4 == 0)
                {
                    String EncryptedValue = DeCodeerBase64_String(pAnimalPassword.AP_Password);
                    if (EncryptedValue.EndsWith("RUMACRYPT"))
                    {
                        pAnimalPassword.AP_Password = EncryptedValue.Substring(0, EncryptedValue.LastIndexOf("RUMACRYPT"));
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(String.Format("DecryptAnimalPassword Value = {0}", Value), ex);
            }
        }

        internal void DecryptFtpInfo(ref FTPINFO pFtpInfo)
        {
            String Value = pFtpInfo.Password;
            try
            {
                if (pFtpInfo.Password.Length % 4 == 0)
                {
                    String EncryptedValue = DeCodeerBase64_String(pFtpInfo.Password);
                    if (EncryptedValue.EndsWith("RUMACRYPT"))
                    {
                        pFtpInfo.Password = EncryptedValue.Substring(0, EncryptedValue.LastIndexOf("RUMACRYPT"));
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(String.Format("DecryptFtpUser Value = {0}", Value), ex);
            }
        }

        internal void EncryptFarmConfig(ref FARMCONFIG pFarmConfig, bool pEncrypt)
        {
            if (pEncrypt)
            {
                String EncryptedValue = pFarmConfig.FValue;
                pFarmConfig.FValue = Codeer_Base64String(EncryptedValue + "RUMACRYPT");
            }
            unLogger.WriteDebug("EncryptFarmConfig was disabled");
        }

        internal void EncryptFarmConfig(ref FARMCONFIG pFarmConfig)
        {
            //String EncryptedValue = pFarmConfig.FValue;
            //pFarmConfig.FValue = Codeer_Base64String(EncryptedValue + "RUMACRYPT");
            unLogger.WriteDebug("EncryptFarmConfig was disabled");
        }

        internal void DecryptFarmConfig(ref FARMCONFIG pFarmConfig)
        {
            String Value = pFarmConfig.FValue;
            try
            {
                if (pFarmConfig.FValue.Length % 4 == 0)
                {
                    String EncryptedValue = DeCodeerBase64_String(pFarmConfig.FValue);
                    if (EncryptedValue.EndsWith("RUMACRYPT"))
                    {
                        pFarmConfig.FValue = EncryptedValue.Substring(0, EncryptedValue.LastIndexOf("RUMACRYPT"));
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(String.Format("DecryptFarmConfig Value = {0}", Value), ex);
            }
        }

        internal void DecryptProgramConfig(ref PROGRAMCONFIG pProgramConfig)
        {
            String Value = pProgramConfig.FValue;
            try
            {
                if (pProgramConfig.FValue.Length % 4 == 0)
                {
                    String EncryptedValue = DeCodeerBase64_String(pProgramConfig.FValue);
                    if (EncryptedValue.EndsWith("RUMACRYPT"))
                    {
                        pProgramConfig.FValue = EncryptedValue.Substring(0, EncryptedValue.LastIndexOf("RUMACRYPT"));
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(String.Format("DecryptProgramConfig Value = {0}", Value), ex);
            }
        }

        internal void DecryptThirdLogin(ref THIRD_LOGIN pThrdLogin)
        {
            String Value = pThrdLogin.Password;
            try
            {
                if (pThrdLogin.Password.Length % 4 == 0)
                {
                    String EncryptedValue = DeCodeerBase64_String(pThrdLogin.Password);
                    if (EncryptedValue.EndsWith("RUMACRYPT"))
                    {
                        pThrdLogin.Password = EncryptedValue.Substring(0, EncryptedValue.LastIndexOf("RUMACRYPT"));
                    }
                }
            }
            catch (Exception ex)
            {
                unLogger.WriteDebug(String.Format("DecryptFtpUser Value = {0}", Value), ex);
            }
        }

        internal void EncryptThirdLogin(ref THIRD_LOGIN pThrdLogin)
        {
            String EncryptedValue = pThrdLogin.Password;
            pThrdLogin.Password = Codeer_Base64String(EncryptedValue + "RUMACRYPT");
        }
    }
}
