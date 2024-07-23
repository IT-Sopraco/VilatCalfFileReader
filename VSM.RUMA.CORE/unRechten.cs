using System;
using System.Data;
using System.Net;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using VSM.RUMA.CORE.DB.DataTypes;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using VSM.RUMA.CORE.COMMONS;
using System.Security.Policy;

namespace VSM.RUMA.CORE
{

    [Guid("5350AB45-B1E4-41d0-894B-95D61AE9D4A5"),
    ClassInterface(ClassInterfaceType.AutoDual)]
    public class unRechten : MaskingClass
    {
        public unRechten()
            : base("cm51ZW1rYXJkYWJ2")
        {

        }

        public delegate void ModuleLoadedEventHandler(Facade sender, long pUBNId);

        public string getBRSnummer(String pGebruiker, String pPassword)
        {
            if (pGebruiker == String.Empty) return null;

            String lPassword = base.Codeer_String(base64Decode(pPassword));
            try
            {
                Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                string brs = service.geefBRSnummer(pGebruiker, lPassword);
                return brs;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Licentie Login Brsnummer:", ex);
                throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
            }
        }

        public UserRightsToken Inloggen(String pGebruiker, String pPassword)
        {
            if (String.IsNullOrEmpty(pGebruiker)) return null;

            String lPassword = Codeer_String(base64Decode(pPassword));
            String mysqlhost = String.Empty;
            String mysqlgebruikersnaam = String.Empty;
            String mysqlwachtwoord = String.Empty;
            String mysqldatabase = String.Empty;
            String slavehost = String.Empty;
            String slavegebruikersnaam = String.Empty;
            String slavewachtwoord = String.Empty;
            String slavedatabase = String.Empty;


            try
            {
                Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();

                try
                {
                    GeefMysqlLogin(pGebruiker, lPassword, ref mysqlhost, ref mysqlgebruikersnaam, ref mysqlwachtwoord, ref mysqldatabase, ref slavehost, ref slavegebruikersnaam, ref slavewachtwoord, ref slavedatabase, service);
                }
                catch (Exception ex)
                {
                    string msg = $"{nameof(unRechten)}.{nameof(Inloggen)} -  WDR Login Ex: {ex.Message}";
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);

                    if (ex.GetType() != typeof(System.UnauthorizedAccessException))
                    {
                        StringBuilder Mail = new StringBuilder();
                        Mail.Append("Er is een fout opgetreden bij het verbinden met de licentieserver");
                        Mail.AppendLine();
                        Mail.AppendFormat("LicentieServer   : {0}", service.Url);
                        Mail.AppendLine();
                        Mail.AppendFormat("Host             : {0}", System.Net.Dns.GetHostName());
                        Mail.AppendLine();
                        Mail.AppendFormat("Gebruiker        : {0}", pGebruiker);
                        SendMail("[LicentieServer Rotterdam] Fout bij Licentiecontrole", Mail, ex);
                    }

                    service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                    mysqlgebruikersnaam = service.geefMysqlLogin(PrepareSOAPString(pGebruiker),
                                                                 PrepareSOAPString(lPassword),
                                                                 out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
                                                                 out slavegebruikersnaam, out slavewachtwoord,
                                                                 out slavehost, out slavedatabase);
                }

            }
            catch (Exception ex)
            {

                string msg = $"{nameof(unRechten)}.{nameof(Inloggen)} -  UNET Login Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);

                StringBuilder Mail = new StringBuilder();
                Mail.Append("Er is een fout opgetreden bij het verbinden met de licentieserver");
                Mail.AppendLine();
                Mail.AppendFormat("LicentieServer   : {0}", Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService);
                Mail.AppendLine();
                Mail.AppendFormat("Host             : {0}", System.Net.Dns.GetHostName());
                Mail.AppendLine();
                Mail.AppendFormat("Gebruiker        : {0}", pGebruiker);
                SendMail("[LicentieServer Boxmeer] Fout bij Licentiecontrole", Mail, ex);

                throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
            }
            if (!String.IsNullOrEmpty(mysqlhost))
            {
                if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
                {
                    mysqlhost = getDBHost();
                    slavehost = getDBHost();
                }
                String pMasterDB = mysqldatabase;
                String pMasterIP = mysqlhost;
                String pMasterUser = mysqlgebruikersnaam;
                String pMasterPass = mysqlwachtwoord;

                String pSlaveDB = slavedatabase;
                String pSlaveIP = slavehost;
                String pSlaveUser = slavegebruikersnaam;
                String pSlavePass = slavewachtwoord;

                Facade Facade = Facade.GetInstance();


                UserRightsToken URT = new UserRightsToken(ref Facade, pGebruiker, base64Decode(pPassword),
                                                          pMasterDB, pMasterIP, pMasterUser,
                                                          pMasterPass, pSlaveDB, pSlaveIP,
                                                          pSlaveUser, pSlavePass);


                //Facade.GetInstance().getSaveToDB(URT).SaveInlog(pGebruiker, base64Decode(pPassword), URT);
                return URT;
            }
            return null;
        }



        protected UserRightsToken getServiceAccount()
        {

            String lGebruiker = "00000";
            String lPassword = "qhxyJduM";
            String mysqlhost = String.Empty;
            String mysqlgebruikersnaam = String.Empty;
            String mysqlwachtwoord = String.Empty;
            String mysqldatabase = String.Empty;
            String slavehost = String.Empty;
            String slavegebruikersnaam = String.Empty;
            String slavewachtwoord = String.Empty;
            String slavedatabase = String.Empty;

            //while (String.IsNullOrEmpty(mysqlhost))
            //{
            //    try
            //    {
            //        Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();

            //        try
            //        {
            //            service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
            //            mysqlgebruikersnaam = service.geefMysqlLogin(PrepareSOAPString(lGebruiker),
            //                                                         PrepareSOAPString(lPassword),
            //                                                         out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
            //                                                         out slavegebruikersnaam, out slavewachtwoord,
            //                                                         out slavehost, out slavedatabase);
            //        }
            //        catch (Exception ex)
            //        {
            //            unLogger.WriteInfo("Licentie WDR Login :", ex);//WriteError aangepast naar WriteInfo, Wordt hieronder al gemaild
            //            if (ex.GetType() != typeof(System.UnauthorizedAccessException))
            //            {
            //                StringBuilder Mail = new StringBuilder();
            //                Mail.Append("Er is een fout opgetreden bij het verbinden met de licentieserver");
            //                Mail.AppendLine();
            //                Mail.AppendFormat("LicentieServer   : {0}", service.Url);
            //                Mail.AppendLine();
            //                Mail.AppendFormat("Host             : {0}", System.Net.Dns.GetHostName());
            //                Mail.AppendLine();
            //                Mail.AppendFormat("CommandLine      : {0}", Environment.CommandLine);
            //                Mail.AppendLine();
            //                Mail.AppendFormat("Foutmelding      : {0}", ex.Message);
            //                SendMail("[INLEESSERVER] Fout in verbinding naar licentieserver WeDare", Mail, ex);
            //            }
            //            service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
            //            mysqlgebruikersnaam = service.geefMysqlLogin(PrepareSOAPString(lGebruiker),
            //                                                         PrepareSOAPString(lPassword),
            //                                                         out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
            //                                                         out slavegebruikersnaam, out slavewachtwoord,
            //                                                         out slavehost, out slavedatabase);
            //        }

            //    }
            //    catch (Exception ex2)
            //    {
            //        unLogger.WriteInfo("Licentie Login :", ex2);
            //        unLogger.WriteError("Licentie Login :", ex2);
            //        StringBuilder Mail = new StringBuilder();
            //        Mail.Append("Er is een fout opgetreden bij het verbinden met de licentieserver");
            //        Mail.AppendLine();
            //        Mail.AppendFormat("LicentieServer   : {0}", Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService);
            //        Mail.AppendLine();
            //        Mail.AppendFormat("Host             : {0}", System.Net.Dns.GetHostName());
            //        Mail.AppendLine();
            //        Mail.AppendFormat("CommandLine      : {0}", Environment.CommandLine);
            //        Mail.AppendLine();
            //        Mail.AppendFormat("Foutmelding      : {0}", ex2.Message);
            //        SendMail("[INLEESSERVER] Fout in verbinding naar licentieserver UNET", Mail, ex2);
            //        System.Threading.Thread.Sleep(new TimeSpan(0, 0, 0, 30));
            //    }

            //}
            unLogger.WriteInfo($@"getServiceAccount():  mysqlgebruikersnaam:{  mysqlgebruikersnaam}");
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
            {
                mysqlhost = getDBHost();
                slavehost = getDBHost();
            }
            String pMasterDB = mysqldatabase;
            String pMasterIP = mysqlhost;                            
            String pMasterUser;
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBUser"])) pMasterUser = mysqlgebruikersnaam;
            else pMasterUser = ConfigurationManager.AppSettings["DBUser"];
            String pMasterPass;
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBPass"])) pMasterPass = mysqlwachtwoord;
            else pMasterPass = DeCodeer_Tekst(ConfigurationManager.AppSettings["DBPass"]);
            String pSlaveDB = slavedatabase;
            String pSlaveIP = slavehost;
            String pSlaveUser;
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBUserSlave"])) pSlaveUser = slavegebruikersnaam;
            else pSlaveUser = ConfigurationManager.AppSettings["DBUserSlave"];
            String pSlavePass;
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBPassSlave"])) pSlavePass = slavewachtwoord;
            else pSlavePass =  DeCodeer_Tekst(ConfigurationManager.AppSettings["DBPassSlave"]);

            if (String.IsNullOrEmpty(slavehost))
            {
                unLogger.WriteInfo("licentieserver returned no slave database; use master credentials");
                pSlaveDB = mysqldatabase;
                pSlaveIP = mysqlhost;
                pSlaveUser = mysqlgebruikersnaam;
                pSlavePass = mysqlwachtwoord;
            }

            Facade lFacade = Facade.GetInstance();
            UserRightsToken URT = new UserRightsToken(ref lFacade, lGebruiker, DeCodeer_Tekst(lPassword),
                                                      "agrofactuur", pMasterIP, pMasterUser,
                                                      pMasterPass, "agrofactuur", pSlaveIP,
                                                      pSlaveUser, pSlavePass);

            UserRightsToken animalDB = new UserRightsToken(ref lFacade, lGebruiker, DeCodeer_Tekst(lPassword),
                                                            "agrobase_calf", pMasterIP, pMasterUser,
                                                            pMasterPass, "agrobase_calf", pSlaveIP,
                                                            pSlaveUser, pSlavePass);
            URT.AddChildConnection(0, animalDB);

            return URT;
        }


        protected void VeranderAdministratieService(ref UserRightsToken pToken, int ProgramId)
        {
            String lPassword = Codeer_String(pToken.Password);
            String mysqlhost = pToken.Host;
            String mysqlgebruikersnaam = pToken.MasterUser;
            String mysqlwachtwoord = pToken.MasterPass;
            String mysqldatabase = pToken.MasterDB;
            String slavehost = pToken.Host;
            String slavegebruikersnaam = pToken.SlaveUser;
            String slavewachtwoord = pToken.SlavePass;
            String slavedatabase = pToken.SlaveDB;

            ////// TMP
            ////String pMasterDB = "agrobase_calf";
            ////String pMasterIP = mysqlhost;
            ////String pMasterUser = mysqlgebruikersnaam;
            ////String pMasterPass = mysqlwachtwoord;
            ////String pSlaveDB = "agrobase_calf";
            ////String pSlaveIP = slavehost;
            ////String pSlaveUser = slavegebruikersnaam;
            ////String pSlavePass = slavewachtwoord;
            ////Facade Facade = Facade.GetInstance();


            ////FarmUserRightsToken FRT = new FarmUserRightsToken(ref Facade, pUBNid, pProgramid.ToString(),
            ////                                          pMasterDB, pMasterIP, pMasterUser,
            ////                                          pMasterPass, pSlaveDB, pSlaveIP,
            ////                                          pSlaveUser, pSlavePass);
            ////pToken.AddChildConnection(pProgId, FRT);
            ////// TMP

            //try
            //{
            //    Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
            //    try
            //    {
            //        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
            //        mysqlgebruikersnaam = service.geefMysqlLogin_Dier(PrepareSOAPString(pToken.User),
            //                                                     PrepareSOAPString(Codeer_String(pToken.Password)), ProgramId.ToString(),
            //                                                     out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
            //                                                     out slavegebruikersnaam, out slavewachtwoord,
            //                                                     out slavehost, out slavedatabase);
            //    }
            //    catch (Exception ex)
            //    {
            //        unLogger.WriteInfo("Licentie WDR Login :", ex);

            //        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
            //        mysqlgebruikersnaam = service.geefMysqlLogin_Dier(PrepareSOAPString(pToken.User),
            //                                                     PrepareSOAPString(Codeer_String(pToken.Password)), ProgramId.ToString(),
            //                                                     out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
            //                                                     out slavegebruikersnaam, out slavewachtwoord,
            //                                                     out slavehost, out slavedatabase);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    unLogger.WriteInfo("Licentie Login :", ex);
            //    throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
            //}
            if (!String.IsNullOrEmpty(mysqlhost))
            {
                if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
                {
                    mysqlhost = getDBHost();
                    slavehost = getDBHost();
                }
                String pMasterDB = mysqldatabase;
                String pMasterIP = mysqlhost;
                String pMasterUser;
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBUser"])) pMasterUser = mysqlgebruikersnaam;
                else pMasterUser = ConfigurationManager.AppSettings["DBUser"];
                String pMasterPass;
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBPass"])) pMasterPass = mysqlwachtwoord;
                else pMasterPass = DeCodeer_Tekst(ConfigurationManager.AppSettings["DBPass"]);
                String pSlaveDB = slavedatabase;
                String pSlaveIP = slavehost;
                String pSlaveUser;
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBUserSlave"])) pSlaveUser = slavegebruikersnaam;
                else pSlaveUser = ConfigurationManager.AppSettings["DBUserSlave"];
                String pSlavePass;
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["DBPassSlave"])) pSlavePass = slavewachtwoord;
                else pSlavePass = DeCodeer_Tekst(ConfigurationManager.AppSettings["DBPassSlave"]);
                Facade Facade = Facade.GetInstance();
                BEDRIJF Farm = new BEDRIJF();
                Farm.Programid = ProgramId;


                UserRightsToken animalDB = new UserRightsToken(ref Facade, pToken.User, DeCodeer_Tekst(lPassword),
                                                                "agrobase_calf", pMasterIP, pMasterUser,
                                                                pMasterPass, "agrobase_calf", pSlaveIP,
                                                                pSlaveUser, pSlavePass);
                pToken.AddChildConnection(0, animalDB);
                //FarmUserRightsToken FRT = new FarmUserRightsToken(ref Facade, Farm,
                //                                          pMasterDB, pMasterIP, pMasterUser,
                //                                          pMasterPass, pSlaveDB, pSlaveIP,
                //                                          pSlaveUser, pSlavePass);
                //pToken.AddChildConnection(ProgramId, FRT);
                //niet nodig, wordt afgevangen in de unmysql
                //Facade.GetInstance().getSaveToDB(pToken).SaveInlog(pToken.User, pToken.Password, pToken);
                //Facade.GetInstance().getSaveToDB(pToken).setToken(pToken);
            }

        }

        private void GeefMysqlLogin(String pGebruiker, String lPassword, ref String mysqlhost, ref String mysqlgebruikersnaam, ref String mysqlwachtwoord, ref String mysqldatabase, ref String slavehost, ref String slavegebruikersnaam, ref String slavewachtwoord, ref String slavedatabase, Licentie.licentieagrobaseService service)
        {
            string lPrefix = $"{nameof(unRechten)}.{nameof(GeefMysqlLogin)} -";

            service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
            service.Timeout = 2000;
            try
            {
                mysqlgebruikersnaam = service.geefMysqlLogin(PrepareSOAPString(pGebruiker),
                                                             PrepareSOAPString(lPassword),
                                                             out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
                                                             out slavegebruikersnaam, out slavewachtwoord,
                                                             out slavehost, out slavedatabase);
            }
            catch (WebException wdrex)
            {
                if (wdrex.Status == WebExceptionStatus.Timeout)
                {
                    string msg = $"{lPrefix} (Primair) WebException (Timeout): {wdrex.Message}";
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, wdrex);

                    //unLogger.WriteInfo("Licentie WDR Fast Login :", wdrex);
                    try
                    {
                        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                        mysqlgebruikersnaam = service.geefMysqlLogin(PrepareSOAPString(pGebruiker),
                                                     PrepareSOAPString(lPassword),
                                                     out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
                                                     out slavegebruikersnaam, out slavewachtwoord,
                                                     out slavehost, out slavedatabase);
                    }
                    catch (WebException unetex)
                    {
                        string msg2 = $"{lPrefix} (Secundair) WebException (Timeout): {unetex.Message}";
                        unLogger.WriteError(msg2);
                        unLogger.WriteDebug(msg2, unetex);

                        //unLogger.WriteInfo("Licentie Unet Fast Login :", unetex);

                        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                        service.Timeout = 100000;

                        try
                        {

                            mysqlgebruikersnaam = service.geefMysqlLogin(PrepareSOAPString(pGebruiker),
                                                     PrepareSOAPString(lPassword),
                                                     out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
                                                     out slavegebruikersnaam, out slavewachtwoord,
                                                     out slavehost, out slavedatabase);
                        }
                        catch (Exception ex)
                        {
                            string msg3 = $"{lPrefix} (Secundair) (Lange timeout) Ex: {ex.Message}";
                            unLogger.WriteError(msg3);
                            unLogger.WriteDebug(msg3, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg2 = $"{lPrefix} (Secundair) Ex: {ex.Message}";
                        unLogger.WriteError(msg2);
                        unLogger.WriteDebug(msg2, ex);

                        throw;
                    }

                }
                else if (wdrex?.Status == WebExceptionStatus.TrustFailure)
                {
                    unLogger.WriteWarn($"{lPrefix} TrustFailure (certificaat niet geïnstalleerd?)");
                    return; //Niet dubbel loggen
                }
                else
                {
                    string msg = $"{lPrefix} (Primair) WebException ({wdrex.Status}): {wdrex.Message}";
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, wdrex);

                    throw;
                }

            }
            catch (Exception ex)
            {
                string msg = $"{lPrefix} Ex: {ex.Message}";
                unLogger.WriteError(msg);
                unLogger.WriteDebug(msg, ex);

                throw;
            }

            if (mysqlhost == "" || slavehost == "")
                throw new UnauthorizedAccessException("No mysqlhost available or password incorrect");
        }

        public String NedapKeyValidation(String pGebruiker, String pKey)
        {
            if (String.IsNullOrEmpty(pGebruiker)) return String.Empty;

            String lPassword = String.Empty;
            try
            {

                Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                try
                {
                    lPassword = service.geefNedapLogin(PrepareSOAPString(pGebruiker),
                                                                               PrepareSOAPString(pKey));
                }
                catch (Exception ex)
                {
                    unLogger.WriteError("Licentie WDR Nedap :", ex);
                    if (ex.GetType() != typeof(System.UnauthorizedAccessException))
                    {
                        StringBuilder Mail = new StringBuilder();
                        Mail.Append("Er is een fout opgetreden bij het verifiëren van de Nedap Sleutel");
                        Mail.AppendLine();
                        Mail.Append("Host       : Licentie2.agrobase.nl");
                        Mail.AppendLine();
                        Mail.Append("Gebruiker  : " + pGebruiker);
                        SendMail("Fout bij de Nedapcheck", Mail, ex);
                    }

                    service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                    lPassword = service.geefNedapLogin(PrepareSOAPString(pGebruiker),
                                                                 PrepareSOAPString(pKey));
                }

            }
            catch (Exception ex)
            {
                unLogger.WriteError("Licentie AMS Nedap :", ex);
                StringBuilder Mail = new StringBuilder();
                Mail.Append("Er is een fout opgetreden bij het verifiëren van de Nedap Sleutel");
                Mail.Append("\r\n");
                Mail.Append("Host       : Licentie.agrobase.nl");
                Mail.Append("\r\n");
                Mail.Append("Gebruiker  : " + pGebruiker);
                SendMail("Fout bij de Nedapcheck", Mail, ex);
            }
            if (lPassword != String.Empty)
            {
                return DeCodeer_Tekst(lPassword);
            }
            return String.Empty;
        }


        private static void SendMail(String Subject, StringBuilder Body, Exception ex)
        {
            string[] to = { "support@vsmhosting.nl" };
            string[] cc = { };
            string[] bcc = { "log@agrobase.nl" };

            StringBuilder MailBody = new StringBuilder();
            MailBody.Append(Body);
            MailBody.AppendLine();
            if (ex != null)
            {
                MailBody.AppendLine("Exception: ");
                MailBody.AppendFormat("{0}{1}", ex, Environment.NewLine);
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    MailBody.AppendLine("InnerException: ");
                    MailBody.AppendFormat("{0}{1}", innerEx, Environment.NewLine);
                    innerEx = innerEx.InnerException;
                }
            }
            Mail.sendMail("log@agrobase.nl", to, cc, bcc, Subject, String.Empty, MailBody.ToString(), null);
        }

        public string soap_geefPostbusnummerVanUBN(string ubn)
        {
            string postbusnummer = "";
            Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
            try
            {

                service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                postbusnummer = service.geefPostbusnummerVanUBN(ubn);

                return postbusnummer;
            }
            catch (Exception ex)
            {
                try
                {
                    unLogger.WriteError("geefPostbusnummerVanUBN WDR:", ex);
                    service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                    postbusnummer = service.geefPostbusnummerVanUBN(ubn);
                }
                catch (Exception exc)
                {
                    unLogger.WriteError("geefPostbusnummerVanUBN AMS:", exc);
                }
            }

            return postbusnummer;
        }

        public FTPINFO getFTPInfoBBS(int pProgId)
        {
            //connect met mysql.ruma-vsm.nl en verkrijg ftp-inloggegevens via de INSTELLING tabel
            string sServer = "mysql.ruma-vsm.nl";
            string sDb = "otrJYf2";
            string sUsr = "3kBVpK1";
            string sPwd = "poLnWC";

            sDb = Facade.GetInstance().getRechten().DeCodeer_String(sDb);
            sUsr = Facade.GetInstance().getRechten().DeCodeer_String(sUsr);
            sPwd = Facade.GetInstance().getRechten().DeCodeer_String(sPwd);

            string connectionString = "server=" + sServer + ";" + "database=" + sDb + ";" + "uid=" + sUsr + ";" + "pwd=" + sPwd;

            VSM.RUMA.CORE.DB.MYSQL.CustomConnections mysqlcons = new VSM.RUMA.CORE.DB.MYSQL.CustomConnections();
            return mysqlcons.getBBSFTP(connectionString, pProgId);

            //MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            //con.Open();

            //MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM INSTELLING WHERE (SOORTPROGID=@progId)", con);
            //MySql.Data.MySqlClient.MySqlParameter p1 = new MySql.Data.MySqlClient.MySqlParameter();
            //p1.ParameterName = "@progId";
            //p1.Value = pProgId;

            //cmd.Parameters.Add(p1);

            ////sla ftp gegevens op in ftpInfo object
            //MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();
            //reader.Read();

            //FTPINFO ftpInf = new FTPINFO();
            //ftpInf.FtpHostName = reader["INSTSERVERNAAM"].ToString();
            //ftpInf.UserName = reader["INSTLOGINNAAM"].ToString();
            //ftpInf.Password = reader["INSTPASSWORD"].ToString();

            //if (reader.IsClosed == false) { reader.Close(); }
            //con.Close();

            //return ftpInf;
        }

        public void VeranderAdministratie(ref UserRightsToken pToken, string Bedrijfsnummer, int programid)
        {
            AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
            UBN ubn = DB.getUBNByBedrijfsnummer(Bedrijfsnummer);
            BEDRIJF Farm = DB.GetBedrijfByUbnIdProgramid(ubn.UBNid, programid);
            VeranderAdministratie(ref pToken, Farm);
        }

        public void VeranderAdministratie(ref UserRightsToken pToken, BEDRIJF pFarm)
        {
            string lPrefix = $"{nameof(unRechten)}.{nameof(VeranderAdministratie)} -";

            if (pFarm == null)
            {
                unLogger.WriteError($"{lPrefix} BEDRIJF niet gevuld.");
                throw new ArgumentException($"{lPrefix} pFarm == null");
            }
              
            string lPassword = Codeer_String(pToken.Password);
            string mysqlhost;
            string mysqlgebruikersnaam;
            string mysqlwachtwoord;
            string mysqldatabase;
            string slavehost;
            string slavegebruikersnaam;
            string slavewachtwoord;
            string slavedatabase;

            try
            {
                Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                try
                {
                    service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                    mysqlgebruikersnaam = service.geefMysqlLogin_Dier(PrepareSOAPString(pToken.User),
                                                                 PrepareSOAPString(lPassword), pFarm.Programid.ToString(),
                                                                 out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
                                                                 out slavegebruikersnaam, out slavewachtwoord,
                                                                 out slavehost, out slavedatabase);
                }               
                catch (Exception ex)
                {
                    if (ex is System.Security.Authentication.AuthenticationException)
                    {
                        string msg = $"{lPrefix} (Primair) AuthenticationException: {ex.Message}";
                        unLogger.WriteWarn(msg);
                    }
                    else
                    {
                        string msg = $"{lPrefix} (Primair) Ex: {ex.Message} ";
                        unLogger.WriteWarn(msg);
                        unLogger.WriteDebug(msg, ex);
                    }
                    service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                    mysqlgebruikersnaam = service.geefMysqlLogin_Dier(PrepareSOAPString(pToken.User),
                                                                 PrepareSOAPString(lPassword), pFarm.Programid.ToString(),
                                                                 out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
                                                                 out slavegebruikersnaam, out slavewachtwoord,
                                                                 out slavehost, out slavedatabase);
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Security.Authentication.AuthenticationException)
                {
                    string msg = $"{lPrefix} (Secundair) AuthenticationException: {ex.Message}";
                    unLogger.WriteWarn(msg);
                }
                else
                {
                    string msg = $"{lPrefix} (Secundair) Ex: {ex.Message} ";
                    unLogger.WriteWarn(msg);
                    unLogger.WriteDebug(msg, ex);
                }
                throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
            }
            if (mysqlhost != String.Empty)
            {
                if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
                {
                    mysqlhost = getDBHost();
                    slavehost = getDBHost();
                }
                String pMasterDB = mysqldatabase;
                String pMasterIP = mysqlhost;
                String pMasterUser = mysqlgebruikersnaam;
                String pMasterPass = mysqlwachtwoord;
                String pSlaveDB = slavedatabase;
                String pSlaveIP = slavehost;
                String pSlaveUser = slavegebruikersnaam;
                String pSlavePass = slavewachtwoord;
                Facade Facade = Facade.GetInstance();

                FarmUserRightsToken FRT = new FarmUserRightsToken(ref Facade, pFarm,
                                                          pMasterDB, pMasterIP, pMasterUser,
                                                          pMasterPass, pSlaveDB, pSlaveIP,
                                                          pSlaveUser, pSlavePass);
                pToken.AddChildConnection(pFarm.Programid, FRT);

                unLogger.WriteDebug($"{lPrefix} AddChildConnection[{pFarm.Programid}] ChildIP:{FRT.MasterIP} ");
            }

        }




        public void VeranderDierDatabase(ref UserRightsToken pToken, int progid)
        {
            String lPassword = Codeer_String(pToken.Password);
            String mysqlhost = pToken.Host;
            String mysqlgebruikersnaam = pToken.MasterUser;
            String mysqlwachtwoord = pToken.MasterPass;
            String mysqldatabase = pToken.MasterDB;
            String slavehost = pToken.Host;
            String slavegebruikersnaam = pToken.SlaveUser;
            String slavewachtwoord = pToken.SlavePass;
            String slavedatabase = pToken.SlaveDB;


            int Programid;
            switch (progid)
            {
                case 1:
                case 4:
                    Programid = 1;
                    break;
                case 3:
                    Programid = 6;
                    break;
                case 5:
                    Programid = 7;
                    break;
                case 2:
                case 6:
                case 7:
                    Programid = 4;
                    break;

                default:
                    Programid = 0;

                    break;
            }

            //try
            //{
            //    Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
            //    try
            //    {
            //        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
            //        mysqlgebruikersnaam = service.geefMysqlLogin_Dier(PrepareSOAPString(pToken.User),
            //                                                     PrepareSOAPString(lPassword), Programid.ToString(),
            //                                                     out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
            //                                                     out slavegebruikersnaam, out slavewachtwoord,
            //                                                     out slavehost, out slavedatabase);
            //    }
            //    catch (Exception ex)
            //    {
            //        unLogger.WriteInfo("Licentie WDR Login :", ex);

            //        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
            //        mysqlgebruikersnaam = service.geefMysqlLogin_Dier(PrepareSOAPString(pToken.User),
            //                                                     PrepareSOAPString(lPassword), Programid.ToString(),
            //                                                     out mysqlwachtwoord, out mysqlhost, out mysqldatabase,
            //                                                     out slavegebruikersnaam, out slavewachtwoord,
            //                                                     out slavehost, out slavedatabase);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    unLogger.WriteInfo("Licentie Login :", ex);
            //    throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
            //}
            if (mysqlhost != String.Empty)
            {
                if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null)
                {
                    mysqlhost = getDBHost();
                    slavehost = getDBHost();
                }
                String pMasterDB = mysqldatabase;
                String pMasterIP = mysqlhost;
                String pMasterUser = mysqlgebruikersnaam;
                String pMasterPass = mysqlwachtwoord;
                String pSlaveDB = slavedatabase;
                String pSlaveIP = slavehost;
                String pSlaveUser = slavegebruikersnaam;
                String pSlavePass = slavewachtwoord;
                Facade Facade = Facade.GetInstance();


                BEDRIJF farm = new BEDRIJF();
                farm.FarmId = -1;
                farm.ProgId = progid;

                UserRightsToken animalDB = new UserRightsToken(ref Facade, pToken.User, DeCodeer_Tekst(lPassword),
                                                                "agrobase_calf", pMasterIP, pMasterUser,
                                                                pMasterPass, "agrobase_calf", pSlaveIP,
                                                                pSlaveUser, pSlavePass);
                pToken.AddChildConnection(0, animalDB);

                //FarmUserRightsToken FRT = new FarmUserRightsToken(ref Facade, farm,
                //                                          pMasterDB, pMasterIP, pMasterUser,
                //                                          pMasterPass, pSlaveDB, pSlaveIP,
                //                                          pSlaveUser, pSlavePass);
                //pToken.AddChildConnection(Programid, FRT);
            }

        }


        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<UBN> UBNlijst(UserRightsToken pToken)
        {
            try
            {
                List<UBN> lUBNList = new List<UBN>();
                if (pToken.User != null || pToken.Password != null)
                {
                    String lPassword = Codeer_String(pToken.Password);
                    Licentie.ubnnummers[] ubnnrs = null;
                    try
                    {
                        Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                        ubnnrs = service.geefUBNNummers(PrepareSOAPString(pToken.User), PrepareSOAPString(lPassword));
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError("LicentieUBN :", ex);
                        //throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
                    }
                    if (ubnnrs == null)
                    {
                        try
                        {
                            Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                            service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                            ubnnrs = service.geefUBNNummers(PrepareSOAPString(pToken.User), PrepareSOAPString(lPassword));

                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteError("LicentieUBN :", ex);
                        }
                    }

                    if (ubnnrs != null)
                    {
                        StringBuilder SBUBN = new StringBuilder("(");
                        for (int i = 0; i < ubnnrs.Length; i++)
                        {
                            if (ubnnrs[i].ubn != null && ubnnrs[i].ubn != String.Empty)
                            {
                                SBUBN.Append('\'' + ubnnrs[i].ubn + '\'');
                                if (i < ubnnrs.Length - 1) SBUBN.Append(", ");
                            }
                        }
                        SBUBN.Append(")");

                        StringBuilder QRY_Bedrijf = new StringBuilder();
                        QRY_Bedrijf.Append(" SELECT * ");
                        QRY_Bedrijf.Append(" FROM UBN");
                        QRY_Bedrijf.Append(" WHERE UBNid > 0");
                        QRY_Bedrijf.AppendFormat(" AND UBN.Bedrijfsnummer IN {0}", SBUBN.ToString());

                        DataTable dtResult = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken, QRY_Bedrijf);

                        foreach (DataRow ResultRow in dtResult.Rows)
                        {
                            UBN lUBN = new UBN();
                            Facade.GetInstance().getSaveToDB(pToken).GetDataBase().FillObject(pToken, lUBN, ResultRow);
                            lUBNList.Add(lUBN);
                        }
                    }
                }
                return lUBNList;
            }
            catch (System.Data.Common.DbException ex)
            {
                unLogger.WriteError("UBNlijst", ex);
                throw;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("UBNlijst", ex);
                throw new Exception("Authorization check Error", ex);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<BEDRIJF> Bedrijflijst(UserRightsToken pToken)
        {
            try
            {

                List<BEDRIJF> lBedrijfList = new List<BEDRIJF>();
                if (pToken.User != null || pToken.Password != null)
                {
                    String lPassword = Codeer_String(pToken.Password);
                    Licentie.ubnnummers[] ubnnrs = null;
                    try
                    {
                        Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                        ubnnrs = service.geefUBNNummers(PrepareSOAPString(pToken.User), PrepareSOAPString(lPassword));
                    }
                    catch (Exception ex)
                    {
                        unLogger.WriteError("LicentieUBN :", ex);
                        //throw new TimeoutException("Agrobase is tijdelijk niet beschikbaar, probeer het later nog eens.", ex);
                    }
                    if (ubnnrs == null)
                    {
                        try
                        {

                            Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                            service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                            ubnnrs = service.geefUBNNummers(PrepareSOAPString(pToken.User), PrepareSOAPString(lPassword));

                        }
                        catch (Exception ex)
                        {
                            unLogger.WriteError("LicentieUBN :", ex);
                        }
                    }
                    AFSavetoDB DB = Facade.GetInstance().getSaveToDB(pToken);
                    if (ubnnrs != null)
                    {
                        StringBuilder SBUBN = new StringBuilder("(");
                        for (int i = 0; i < ubnnrs.Length; i++)
                        {
                            if (ubnnrs[i].ubn != null && ubnnrs[i].ubn != String.Empty)
                            {
                                SBUBN.Append('\'' + ubnnrs[i].ubn + '\'');
                                if (i < ubnnrs.Length - 1) SBUBN.Append(", ");
                            }
                        }
                        SBUBN.Append(")");
                        //unLogger.WriteDebug("Inlog heeft de volgende UBN's : " + SBUBN);
                        //BUG 1775
                        StringBuilder QRY_Bedrijf = new StringBuilder();
                        QRY_Bedrijf.Append(" SELECT b.* ");
                        QRY_Bedrijf.Append(" , ");
                        QRY_Bedrijf.Append(" IF(b.ProgramId=47,(SELECT COUNT(ag.AniId) FROM agrobase_goat.ANIMALCATEGORY ag ");
                        QRY_Bedrijf.Append(" WHERE ag.FarmID=b.FarmId AND ag.AniCategory BETWEEN 0 AND 4) ,0)AS Aantal ");
                        QRY_Bedrijf.Append(" FROM BEDRIJF b");
                        QRY_Bedrijf.Append(" JOIN UBN ");
                        QRY_Bedrijf.Append(" ON UBN.UBNid = b.UBNid ");
                        QRY_Bedrijf.Append(" WHERE b.UBNid > 0");
                        QRY_Bedrijf.Append(" AND b.FarmId > 0");
                        QRY_Bedrijf.AppendFormat(" AND UBN.Bedrijfsnummer IN {0} AND b.Programid>0", SBUBN.ToString());
                        DataTable dtResult = DB.GetDataBase().QueryData(pToken, QRY_Bedrijf);
                        //unLogger.WriteDebug(dtResult.Rows.Count + " Administraties gevonden!");
                        foreach (DataRow ResultRow in dtResult.Rows)
                        {
                            BEDRIJF lBedrijf = new BEDRIJF();
                            DB.GetDataBase().FillObject(pToken, lBedrijf, ResultRow);
                            //unLogger.WriteDebug("Administratie " + lBedrijf.Omschrijving + " gevonden!");
                            if (lBedrijf.Programid == 47)
                            {
                                //BUG 1775
                                if (ResultRow["Aantal"] != DBNull.Value)
                                {
                                    if (ResultRow["Aantal"].ToString() != "")
                                    {
                                        int aantal = 0;
                                        int.TryParse(ResultRow["Aantal"].ToString(), out aantal);
                                        if (aantal > 0)
                                        {
                                            lBedrijfList.Add(lBedrijf);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lBedrijfList.Add(lBedrijf);
                            }
                        }
                        int lThrid = getThrIdFromBrsNr(pToken);

                        if (lThrid > 0)
                        {
                            StringBuilder QRY_Administratie = new StringBuilder();
                            QRY_Administratie.Append(" SELECT ADMINISTRATIE.* ");
                            QRY_Administratie.Append(" FROM ADMINISTRATIE");
                            QRY_Administratie.Append(" WHERE ADMINISTRATIE.ThrId > 0");
                            QRY_Bedrijf.Append(" AND ADMINISTRATIE.AdmisID > 0");
                            QRY_Bedrijf.Append(" AND ADMINISTRATIE.ProgramID > 0");
                            QRY_Administratie.AppendFormat("  AND ADMINISTRATIE.ThrId = {0} ", lThrid);

                            DataTable dtResult2 = DB.GetDataBase().QueryData(pToken, QRY_Administratie);
                            if (dtResult2.Rows.Count > 0)
                            {
                                foreach (DataRow ResultRow2 in dtResult2.Rows)
                                {
                                    ADMINISTRATIEBEDRIJF lBedrijf2 = new ADMINISTRATIEBEDRIJF();
                                    lBedrijf2.AdmisID = int.Parse(ResultRow2["AdmisID"].ToString());
                                    lBedrijf2.ThrID = int.Parse(ResultRow2["ThrID"].ToString());
                                    lBedrijf2.Programid = int.Parse(ResultRow2["ProgramID"].ToString());
                                    lBedrijf2.ProgId = int.Parse(ResultRow2["ProgID"].ToString());
                                    lBedrijf2.Omschrijving = ResultRow2["Ad_AdmisName"].ToString();
                                    lBedrijfList.Add(lBedrijf2);
                                }
                            }
                        }
                    }
                    else
                    {

                        int lThrid = getThrIdFromBrsNr(pToken);
                        if (lThrid > 0)
                        {
                            StringBuilder QRY_Administratie = new StringBuilder();
                            QRY_Administratie.Append(" SELECT ADMINISTRATIE.* ");
                            QRY_Administratie.Append(" FROM ADMINISTRATIE");
                            QRY_Administratie.Append(" WHERE ADMINISTRATIE.ThrId > 0 ");
                            QRY_Administratie.Append(" AND ADMINISTRATIE.AdmisID > 0 ");
                            QRY_Administratie.Append(" AND ADMINISTRATIE.ProgramID > 0 ");
                            QRY_Administratie.AppendFormat(" AND ADMINISTRATIE.ThrId = {0} ", lThrid);

                            DataTable dtResult = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken, QRY_Administratie);
                            if (dtResult.Rows.Count > 0)
                            {
                                foreach (DataRow ResultRow in dtResult.Rows)
                                {
                                    ADMINISTRATIEBEDRIJF lBedrijf = new ADMINISTRATIEBEDRIJF();
                                    lBedrijf.AdmisID = int.Parse(ResultRow["AdmisID"].ToString());
                                    lBedrijf.ThrID = int.Parse(ResultRow["ThrID"].ToString());

                                    lBedrijf.Programid = int.Parse(ResultRow["ProgramID"].ToString());
                                    lBedrijf.ProgId = int.Parse(ResultRow["ProgID"].ToString());
                                    lBedrijf.Omschrijving = ResultRow["Ad_AdmisName"].ToString();
                                    lBedrijfList.Add(lBedrijf);
                                }
                            }
                            unLogger.WriteInfo("ThrId uit THIRD van de Database=" + lThrid.ToString() + " aantal met ADMINISTRATIE.ThrId=" + dtResult.Rows.Count);
                        }
                        else
                        {
                            unLogger.WriteInfo("geen ThrId teruggekregen");
                        }

                    }
                }
                return lBedrijfList;
            }
            catch (System.Data.Common.DbException ex)
            {
                unLogger.WriteError("Bedrijflijst", ex);
                unLogger.FlushBuffers();
                throw;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Bedrijflijst", ex);
                unLogger.FlushBuffers();
                throw new Exception("Authorization check Error", ex);
            }
        }

        public int getThrIdFromBrsNr(UserRightsToken pToken)
        {
            if (pToken.User != null || pToken.Password != null)
            {

                String lPassword = Codeer_String(pToken.Password);


                Licentie.licentieagrobaseService service = new Licentie.licentieagrobaseService();
                service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieWDR_licentieagrobaseService;
                string lSocialnr = "";
                string lBrs = "";
                string lKvknr = "";
                try
                {
                    lKvknr = service.geefRelatieInfo(PrepareSOAPString(pToken.User), PrepareSOAPString(lPassword), out lSocialnr, out lBrs);
                }
                catch (Exception exc)
                {
                    unLogger.WriteError(" getThrIdFromBrsNr WDR:", exc);
                    try
                    {
                        service.Url = Properties.Settings.Default.VSM_RUMA_CORE_LicentieAMS_licentieagrobaseService;
                        lKvknr = service.geefRelatieInfo(PrepareSOAPString(pToken.User), PrepareSOAPString(lPassword), out lSocialnr, out lBrs);
                    }
                    catch (Exception exc2)
                    {
                        unLogger.WriteError(" getThrIdFromBrsNr AMS:", exc2);
                    }
                }

                if (lBrs == null && lKvknr == null && lSocialnr == null)
                {
                    unLogger.WriteInfo("****** licentieagrobaseService geeft alles null terug: voor gebruiker uit tabel administatie");
                }
                try
                {
                    THIRD inloggerTHIRD = new THIRD();
                    if (lBrs != null && lBrs.Trim() != "")
                    {
                        inloggerTHIRD = Facade.GetInstance().getSaveToDB(pToken).GetThirdByBrs_Number(lBrs.Trim());
                        unLogger.WriteInfo("geefRelatieInfo: Brs " + lBrs.Trim());

                    }
                    if (inloggerTHIRD.ThrId == 0)
                    {
                        if (lKvknr != null && lKvknr.Trim() != "")
                        {
                            inloggerTHIRD = Facade.GetInstance().getSaveToDB(pToken).GetThirdByKvKnr(lKvknr.Trim());
                            unLogger.WriteInfo("geefRelatieInfo: Kvknr " + lKvknr.Trim());

                        }
                    }
                    if (inloggerTHIRD.ThrId == 0)
                    {
                        if (lSocialnr != null && lSocialnr.Trim() != "")
                        {
                            inloggerTHIRD = Facade.GetInstance().getSaveToDB(pToken).GetThirdBySofiNumber(lSocialnr.Trim());
                            unLogger.WriteInfo("geefRelatieInfo: Socialnr " + lSocialnr.Trim());

                        }
                    }
                    return inloggerTHIRD.ThrId;
                }
                catch { }
                return 0;

            }
            else { return 0; }
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<LABELS> Labellijst(UserRightsToken pToken, int pLabelKind, String pFilter)
        {
            try
            {
                StringBuilder QRY_Labels = new StringBuilder();
                QRY_Labels.Append(" SELECT * ");
                QRY_Labels.Append(" FROM LABELS");
                QRY_Labels.AppendFormat(" WHERE LabKind = {0}", pLabelKind);
                QRY_Labels.AppendFormat(" AND LabCountry = {0}", utils.getLabelsLabcountrycode());
                QRY_Labels.AppendFormat(" AND ({0})", pFilter);
                DataTable dtResult = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken, QRY_Labels);
                List<LABELS> lLabelsList = new List<LABELS>();
                foreach (DataRow ResultRow in dtResult.Rows)
                {
                    LABELS lLABELS = new LABELS();
                    Facade.GetInstance().getSaveToDB(pToken).GetDataBase().FillObject(pToken, lLABELS, ResultRow);
                    lLabelsList.Add(lLABELS);
                }
                return lLabelsList;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("Labellijst", ex);
                throw ex;
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<LABELS> lijst(UserRightsToken pToken, int pLabelKind, String pFilter)
        {
            try
            {
                StringBuilder QRY_Labels = new StringBuilder();
                QRY_Labels.Append(" SELECT * ");
                QRY_Labels.Append(" FROM LABELS");
                QRY_Labels.AppendFormat(" WHERE LabKind = {0}", pLabelKind);
                QRY_Labels.AppendFormat(" AND LabCountry = {0}", utils.getLabelsLabcountrycode());
                QRY_Labels.AppendFormat(" AND ({0})", pFilter);
                DataTable dtResult = Facade.GetInstance().getSaveToDB(pToken).GetDataBase().QueryData(pToken, QRY_Labels);
                List<LABELS> lLabelsList = new List<LABELS>();
                foreach (DataRow ResultRow in dtResult.Rows)
                {
                    LABELS lLABELS = new LABELS();
                    Facade.GetInstance().getSaveToDB(pToken).GetDataBase().FillObject(pToken, lLABELS, ResultRow);
                    lLabelsList.Add(lLABELS);
                }
                return lLabelsList;
            }
            catch (Exception ex)
            {
                unLogger.WriteError("lijst", ex);
                throw ex;
            }
        }


        [Obsolete("vervangen door agrolink rechten", true)]
        public void getRechten(UserRightsToken pToken, String pUBNId)
        {
            //Facade.GetInstance().getSaveToDB(pToken).SaveInlog(pToken.User, pToken.Password, pToken);
        }

        [Obsolete("vervangen door agrolink rechten", true)]
        public bool IsVisible(String pText)
        {
            return true;
        }

        [Obsolete("vervangen door agrolink rechten", true)]
        public bool HasAccessToPage(UserRightsToken pToken, String pUBNId, String pUrl)
        {
            //Facade.GetInstance().getSaveToDB(pToken).SaveInlog(pToken.User, pToken.Password, pToken);
            return true;
        }


        [Obsolete("vervangen door agrolink rechten", true)]
        public bool HasAccessToMenuItem(UserRightsToken pToken, String MenuValue)
        {
            //TODO verwerken in rechten module als die af is.
            if (MenuValue == "Fokstieren" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Relaties" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Opmerkingen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Categorys" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Remark" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Edidap inlezen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Rantsoen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Poeder" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Aanvoer" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Transportmiddelen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Levensnrvoorraad" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Groepsinvoer" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Behandelplan" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Medicijnen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Groepen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Edidapinlezen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "verblijfplaatsenophalen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "stallijstophalen" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            else if (MenuValue == "Nullmeting" && Facade.GetInstance().getSaveToDB(pToken).Plugin() == "T4C")
            {
                return false;
            }
            return true;
        }

        [Obsolete("vervangen door agrolink rechten", true)]
        public string GetFarmConfigDefaultValue(int pProgramid, String ConfigValue)
        {
            //TODO verwerken in rechten module als die af is.
            switch (ConfigValue)
            {
                case "IRherstel":
                    switch (pProgramid)
                    {
                        case 22:
                        case 100:
                        case 51:
                        case 230:
                            return "True";
                        default:
                            return "False";
                    }
                default:
                    return "";
            }
        }

        public static string getDBHost()
        {
            if (ConfigurationManager.ConnectionStrings["RUMADATA"] != null
             && ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString != string.Empty)
            {
                unRechten u = new unRechten();
      
                string ret = ConfigurationManager.ConnectionStrings["RUMADATA"].ConnectionString;
                char[] split1 = { ';'};
                char[] split2 = { '=' };
                StringBuilder connstring = new StringBuilder();
                string[] splitconnstring = ret.Split(split1);
                for (int i = 0; i < splitconnstring.Length; i++)
                {
                    if (splitconnstring[i].StartsWith("user id=") || splitconnstring[i].StartsWith("Pwd="))
                    {
                        string[] pwd = splitconnstring[i].Split(split2);
                        if (pwd.Length == 2)
                        {
                            connstring.Append(pwd[0] + "=" + u.DeCodeer_Tekst(pwd[1]));
                        }
                    }
                    else
                    {
                        connstring.Append(splitconnstring[i]);
                    }
                    connstring.Append(";");
                    
                }
                try
                {
                    var dbh = VSM.RUMA.CORE.DB.MYSQL.CustomConnections.getDBHost(connstring.ToString());
                    if (dbh == null)
                    {
                        return VSM.RUMA.CORE.DB.MYSQL.CustomConnections.getDBHost(ret);
                    }
                    return dbh;
                }
                catch
                {
                    return VSM.RUMA.CORE.DB.MYSQL.CustomConnections.getDBHost(ret);
                }
                
            }
            else
            { return string.Empty; }
        }

        #region Masking
        public string Codeer_base64(string data)
        {
            return base.base64Encode(data);

        }

        internal string Codeer_Tekst(string Tekst)
        {
            return base.Codeer_String(Tekst);

        }

        internal string DeCodeer_Tekst(string Tekst)
        {
            return base.DeCodeer_String(Tekst);
        }



        // LEGACY
        [Obsolete("gebruik Codeer_base64")]
        public new string base64Encode(string data)
        {
            return base.base64Encode(data);

        }

        [Obsolete("gebruik Codeer_Tekst")]
        public new string Codeer_String(string Tekst)
        {
            return base.Codeer_String(Tekst);

        }

        [Obsolete("gebruik DeCodeer_Tekst")]
        public new string D(string Tekst)
        {
            return base.DeCodeer_String(Tekst);
        }

        [Obsolete("gebruik de Win32 functie of PasswordDecryptor")]
        internal string Decodeer_PW(string Tekst)
        {
            return base.base64Decode(Tekst);
        }

        #endregion

        /// <summary>
        /// Gets the crypted username and password out of the config (RVOUsername, RVOPassword)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Success indicator</returns>
        public bool GetRVOCredentialsFromConfig(out string username, out string password)
        {
            username = "";
            password = "";

            try
            {
                string user = ConfigurationManager.AppSettings["RVOUsername"];
                string pass = ConfigurationManager.AppSettings["RVOPassword"];
                
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    unLogger.WriteError("unrechten.GetRVOCredentialsFromConfig User or Password empty.");
                    return false;
                }

                username = DeCodeer_String(user);
                password = DeCodeer_String(pass);

                return true;
            }
            catch(Exception ex)
            {
                unLogger.WriteError(string.Format("unrechten.GetLNVCredentialsFromConfig ex: {0}", ex));
                return false;
            }
        }

        /// <summary>
        /// Gets the crypted username and password out of the config (AgrobaseUsername, AgrobasePassword)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Success indicator</returns>
        public bool GetAgrobaseCredentialsFromLogin(out string username, out string password)
        {
            username = "";
            password = "";

            try
            {
                string user = ConfigurationManager.AppSettings["AgrobaseUsername"];
                string pass = ConfigurationManager.AppSettings["AgrobasePassword"];

                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    unLogger.WriteError("unrechten.GetAgrobaseCredentialsFromLogin User or Password empty.");
                    return false;
                }

                username = DeCodeer_String(user);
                password = DeCodeer_String(pass);

                return true;
            }
            catch (Exception ex)
            {
                unLogger.WriteError(string.Format("unrechten.GetAgrobaseCredentialsFromLogin ex: {0}", ex));
                return false;
            }
        }
    }

    [Obsolete("alleen in gebruik voor oude T4C plugin",true)]
    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }


        #region ICertificatePolicy Members

        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {

            // Always accept
            return true;
        }

        #endregion
    }
}
