using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Net;

namespace VSM.RUMA.CORE
{
    public class RawSoap
    {
        private String ErrorMessage = "";
        private String sOutput = "";

        public void sendRawSoap(string strWebServiceURL,string SoapRequestXmlFilePath,string SoapResponseXmlFilePath,string pUserName,string pPassWord)
        {
            //the URL is on the application
            string strURL = strWebServiceURL;
            //go find the raw XML


            //load the XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SoapRequestXmlFilePath);
            //Create the Web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);

            // Set 'Preauthenticate'  property to true.  Credentials will be sent with the request.
            request.PreAuthenticate = true;
            string UserName = pUserName;
            string Password = pPassWord;
            // Create a New 'NetworkCredential' object.
            NetworkCredential networkCredential = new NetworkCredential(UserName, Password);

            // Associate the 'NetworkCredential' object with the 'WebRequest' object.
            request.Credentials = networkCredential;




            //set the properties
            request.Headers.Add("SOAPAction", "\"\"");
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.Timeout = 30 * 1000;
            //open the pipe?
            Stream request_stream = request.GetRequestStream();
            //write the XML to the open pipe (e.g. stream)
            xmlDoc.Save(request_stream);
            //CLOSE THE PIPE !!! Very important or next step will time out!!!!
            request_stream.Close();

            //get the response from the webservice
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream r_stream = response.GetResponseStream();
                //convert it
                StreamReader response_stream = new StreamReader(r_stream, System.Text.Encoding.GetEncoding("utf-8"));
                sOutput = response_stream.ReadToEnd();
                response_stream.Close();
                XmlDocument Xresponse = new XmlDocument();
                Xresponse.LoadXml(sOutput);
                Xresponse.Save(SoapResponseXmlFilePath);
            }
            catch (Exception exc) 
            {  
                ErrorMessage = exc.Message;
                unLogger.WriteInfo(exc.ToString());
                try
                {
                    XDocument XDOTNetresponse = new XDocument();
                    XElement root = new XElement("AspNetError");
                    XElement error = new XElement("Error");
                    error.Value = exc.Message;
                    if (error.Value.Contains("401"))
                    {
                        error.Value = "Gebruikersnaam of wachtwoord niet correct.";
                    }
                    root.Add(error);
                    XDOTNetresponse.Add(root);
                    XDOTNetresponse.Save(SoapResponseXmlFilePath);
                }
                catch (Exception exc2) { unLogger.WriteInfo(exc2.ToString()); }
            }
            finally { }


        }

        public XDocument getBasicSoapDocument()
        {
            XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
            XDocument xd = new XDocument();
            XElement soapH = new XElement(soapenv + "Envelope");
            XAttribute atr = new XAttribute(XNamespace.Xmlns + "SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
            soapH.Add(atr);
            XElement soapB = new XElement(soapenv + "Body");
            soapH.Add(soapB);
            xd.Add(soapH);
            return xd;
        }

    }
}
