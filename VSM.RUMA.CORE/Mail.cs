using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Configuration;

namespace VSM.RUMA.CORE
{
    public class Mail
    {
        public static bool isValidEmail(string pEmailadres)
        {
            //If the object Mailmessage does not accept the mailadress.
            //We cannot accept it.
            if (!string.IsNullOrWhiteSpace(pEmailadres))
            {
                try
                {
                    MailMessage mTemp = new MailMessage();
                    mTemp.To.Add(pEmailadres.Trim());
                    return true;
                }
                catch { }
            }
            return false;
        }
      
        public static void sendMail( string pSubject, string pHTMLBody, string pTextBody)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.vsmhosting.nl");

                if (ConfigurationManager.AppSettings["MailToAdmin"] != null && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MailToAdmin"]))
                { 
                    mail.From = new MailAddress("info@ruma-vsm.nl");
                    string mailto = ConfigurationManager.AppSettings["MailToAdmin"].Replace(';', ',');
                    mail.To.Add(mailto);
                    mail.Bcc.Add("log@agrobase.nl");
                    mail.Subject = pSubject;
                    if (pHTMLBody != "")
                    {
                        mail.IsBodyHtml = true;

                        mail.Body = pHTMLBody;
                    }
                    else if (pTextBody != "")
                    {
                        mail.Body = pTextBody;
                    }
                    else { mail.Body = "Test mail"; }
                    //SmtpServer.Port = 587;
                    //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                    //SmtpServer.EnableSsl = true;
                    if (ConfigurationManager.AppSettings["UseCRDTestserver"]!=null && ConfigurationManager.AppSettings["UseCRDTestserver"].ToLower() == "false")
                    {
                        SmtpServer.Send(mail);
                    }
                }
                else
                {
                    unLogger.WriteError($@"
You are missing the AppSettings for:
MailToAdmin    (comma seperated list), 
  <add key='MailToAdmin' value=''/>	
in settings.config .
Used for Relations changes, and  Password changes
");
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError(exc.Message);
            }
        }

        public static void sendErrorMail(string pSubject, string pTextBody)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SendErrorMail"] != null)
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendErrorMail"]))
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("mail.vsmhosting.nl");

                        mail.From = new MailAddress("foutmelding@agrobase.nl");
                        if (ConfigurationManager.AppSettings["SendErrorMailTo"] != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SendErrorMailTo"]))
                            {
                                string mailto = ConfigurationManager.AppSettings["SendErrorMailTo"].Replace(';', ',');
                                mail.To.Add(mailto);
                                if (ConfigurationManager.AppSettings["SendErrorMailCC"] != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SendErrorMailCC"]))
                                    {
                                        string mailcc = ConfigurationManager.AppSettings["SendErrorMailCC"].Replace(';', ',');
                                        mail.CC.Add(mailcc);
                                    }
                                }
                                     
                                if (ConfigurationManager.AppSettings["SendErrorMailBCC"] != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SendErrorMailBCC"]))
                                    {
                                        string mailbcc = ConfigurationManager.AppSettings["SendErrorMailBCC"].Replace(';', ',');
                                        mail.Bcc.Add(mailbcc);
                                    }
                                }
                                       
                                mail.Subject = pSubject;
                                if (!string.IsNullOrWhiteSpace(pTextBody))
                                {
                                    mail.Body = pTextBody;

                                    if (ConfigurationManager.AppSettings["UseCRDTestserver"] != null && ConfigurationManager.AppSettings["UseCRDTestserver"].ToLower() == "false")
                                    {
                                        SmtpServer.Send(mail);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    unLogger.WriteError($@"
You are missing the AppSettings for:
SendErrorMail    (True or False), 
if True Add:
SendErrorMailTo  (comma seperated list),
SendErrorMailCC  (comma seperated list),
SendErrorMailBCC (comma seperated list),

in settings.config

");
                }
            }
            catch (Exception exc)
            {
                unLogger.WriteError(exc.ToString());
            }
        }

        public static string sendMail(string pFrom, string[] pTo, string[] pCC, string[] pBCC, string pSubject, string pHTMLBody, string pTextBody, AlternateView pAlternateView)
        { 
            string[] pAttachments = { };
            return sendMail(pFrom, pTo, pCC, pBCC, pSubject, pHTMLBody, pTextBody, pAlternateView, pAttachments);
        }
        /// <summary>
        /// WARNING: This will send mail !!!
        /// </summary>
        /// <param name="pFrom"></param>
        /// <param name="pTo"></param>
        /// <param name="pCC"></param>
        /// <param name="pBCC"></param>
        /// <param name="pSubject"></param>
        /// <param name="pHTMLBody"></param>
        /// <param name="pTextBody"></param>
        /// <param name="pAlternateView"></param>
        /// <returns></returns>
        public static string sendMail(string pFrom, string[] pTo, string[] pCC, string[] pBCC, string pSubject, string pHTMLBody, string pTextBody, AlternateView pAlternateView, string[] pFileAttachments)
        {
            string ret = "";
            try
            {

                if (pFrom.Trim() != "")
                {
                    MailMessage mail = new MailMessage();

                    try 
                    {
                        if (pFileAttachments != null && pFileAttachments.Length > 0)
                        {
                           
                            foreach (string lAttachment in pFileAttachments)
                            {
                                if (File.Exists(lAttachment))
                                {
                                    System.Net.Mail.Attachment at = new System.Net.Mail.Attachment(lAttachment);
                                    mail.Attachments.Add(at);

                                }
                            }
                        }
                    }
                    catch (Exception exc) { unLogger.WriteError("Mail sendMail with pFileAttachments " + exc.ToString()); }
                   

                    SmtpClient SmtpServer = new SmtpClient("mail.vsmhosting.nl");
                    mail.From = new MailAddress(pFrom);

                    foreach (string sto in pTo)
                    {
                        if (sto.Trim().Length > 0)
                        {
                            try
                            {
                                mail.To.Add(sto.Trim());
                            }
                            catch { unLogger.WriteError(sto + " is NOT correct emailadres for TO "); }
                        }
                    }
                    foreach (string scc in pCC)
                    {

                        if (scc.Trim().Length > 0)
                        {
                            try
                            {
                                mail.CC.Add(scc.Trim());
                            }
                            catch { unLogger.WriteError(scc + ": is NOT correct emailadres for CC "); }
                        }
                    }
                    foreach (string sbcc in pBCC)
                    {
                        if (sbcc.Length > 0)
                        {
                            try
                            {
                                mail.Bcc.Add(sbcc);
                            }
                            catch { unLogger.WriteError(sbcc + ": is NOT correct emailadres for BCC "); }
                        }
                    }
                    if (!mail.Bcc.Any(x=>x.Address == "log@agrobase.nl"))
                    {
                        mail.Bcc.Add("log@agrobase.nl");
                    }
                    
                    mail.Subject = pSubject;

                    if (pHTMLBody != "")
                    {
                        mail.IsBodyHtml = true;

                        mail.Body = pHTMLBody;
                    }
                    else if (pTextBody != "")
                    {
                        mail.Body = pTextBody;
                    }
                    else if (pAlternateView != null)
                    {
                        mail.AlternateViews.Add(pAlternateView);
                        mail.IsBodyHtml = true;
                    }
                    else { mail.Body = " "; }

                    SmtpServer.Send(mail);

                }
            }
            catch (Exception exc)
            {
                ret = exc.Message;
                unLogger.WriteError(exc.ToString());
            }
            return ret;
        }
 
    }
}
