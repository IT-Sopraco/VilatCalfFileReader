using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSM.RUMA.CORE.COMMONS
{
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
                unLogger.WriteDebug("masking.cs-base64Decode: Error:Base 64 string is null.");
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
                unLogger.WriteError("Error in base64Encode" + e.ToString());
                
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

        /// <summary>
        /// ATTENTION: Geldige tekens: ALLEEN Letters en Nummers.
        /// </summary>
        /// <param name="Tekst"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ATTENTION: Geldige tekens: ALLEEN Letters en Nummers
        /// </summary>
        /// <param name="Tekst"></param>
        /// <returns></returns>
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

    internal class PasswordDecryptor : MaskingClass
    {
        public PasswordDecryptor()
            : base(String.Empty)
        { }

        internal string Decodeer_PW(string Tekst)
        {
            return base64Decode(Tekst);
        }
    }

    public class NedapDecryptor : MaskingClass
    {
        public NedapDecryptor()
            : base("fMaStV8pgardke2N", "Aa1Bb2Cc3Dd4Ee5Ff6Gg7Hh8Ii9Jj0KkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz")
        { 

        }

        public string DeCodeer_Tekst(string Tekst)
        {
            return base.DeCodeer_String(Tekst);
        }
    }


}
