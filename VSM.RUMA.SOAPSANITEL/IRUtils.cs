using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace VSM.RUMA.SOAPSANITEL
{
    public class IRUtils
    {
        public enum logkind
        {
            info = 1,
            debug = 2,
            warn = 3,
            error = 4

        }


        public const string NOTIFICATIONTYPE_BIRTH = "ARBTH";
        public const string NOTIFICATIONTYPE_DEPARTURE_DISPOSAL = "MMDPD";
        public const string NOTIFICATIONTYPE_DEPARTURE = "MMDPR";
        public const string NOTIFICATIONTYPE_IMPORT = "ARIMO";
        public const string NOTIFICATIONTYPE_EXPORT = "MMEPO";
        public const string NOTIFICATIONTYPE_ARRIVAL = "MMARV";
        public const string NOTIFICATIONTYPE_DEPARTURE_SLAUGHTER = "MMDPS";
        public const string NOTIFICATIONTYPE_ARRIVAL_SLAUGHTER = "MMSLG";
        public const string NOTIFICATIONTYPE_ARRIVAL_DISPOSAL = "MMDST";
        public const string NOTIFICATIONTYPE_PRUN = "PRUN";
        public static log4net.ILog _log => log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void writerequest<T>(string ubn, string url, string tijd, T req)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["WriteSoapRequest"] != null && System.Configuration.ConfigurationManager.AppSettings["WriteSoapRequest"].ToString().ToUpper() == "TRUE")
                {
                    XmlSerializer xsreq = new XmlSerializer(typeof(T));

                    string filename = typeof(T) + "_" + ubn + "_" + tijd + ".xml";
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(getLogDir(), "IenR"));
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    string logfile = Path.Combine(getLogDir(), "IenR", filename);
                    var xml = "";

                    using (var sww = new StringWriter())
                    {
                        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww))
                        {
                            xsreq.Serialize(writer, req);
                            XDocument xd = XDocument.Parse(sww.ToString());
                            xd.Root.Add(new XAttribute("Uri", url));
                            xml = xd.ToString();
                        }
                    }

                    using (StreamWriter wr = new StreamWriter(logfile))
                    {
                        wr.Write(xml);
                    }
                    //unLogger.WriteDebug($@"  - UBN: {ubn} Logfile: {logfile}");
                }

            }
            catch (Exception exc)
            {
                _log.Error(exc.ToString());
            }
        }

        public static string getLogDir()
        {
            foreach (log4net.Appender.IAppender Appender in _log.Logger.Repository.GetAppenders())
            {
                if (Appender.Name == "DefaultFile" || Appender.Name == "File")
                {
                    return Path.GetDirectoryName(((log4net.Appender.FileAppender)Appender).File) + Path.DirectorySeparatorChar;
                }
                else if (Appender is log4net.Appender.FileAppender || Appender is log4net.Appender.RollingFileAppender)
                {
                    return Path.GetDirectoryName(((log4net.Appender.FileAppender)Appender).File) + Path.DirectorySeparatorChar;
                }
            }
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "log" + Path.DirectorySeparatorChar;
        }

        public static void writelogline(string logfile, string line)
        {
            try
            {
                if (!string.IsNullOrEmpty(logfile) && !string.IsNullOrEmpty(line))
                {
                    using (StreamWriter wr = new StreamWriter(logfile, true))
                    {
                        wr.WriteLine(line);
                    }
                }
            }
            catch(Exception exc)
            {
                _log.Error(exc.ToString());
            }
        }

        internal static string getAnimaltype(int progid)
        {
            string ANTP_CDE = "BOV";
            if (progid == 3)
            {
                ANTP_CDE = "SHP";
            }
            else if (progid == 5)
            {
                ANTP_CDE = "GOAT";
            }
            return ANTP_CDE;
        }

        internal static object getrequest<T>(T req)
        {
            try
            {
                if (req != null)
                {
                    XmlSerializer xsreq = new XmlSerializer(typeof(T));

                    var xml = "";

                    using (var sww = new StringWriter())
                    {
                        using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww))
                        {
                            xsreq.Serialize(writer, req);
                            XDocument xd = XDocument.Parse(sww.ToString());

                            xml = xd.ToString();
                        }
                    }

                    return xml;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc.ToString());
            }
            return "";
        }

        public static string getControlegetal(string pAniAlternateNumber)
        {
            if (string.IsNullOrEmpty(pAniAlternateNumber))
            {
                return "";
            }
            int som = 0;
            string getal = "";
            if (pAniAlternateNumber.StartsWith("BE"))
            {
                getal = pAniAlternateNumber.Replace("BE", "").Trim();
                if (getal.Length == 8)
                {
                    long controle = 0;
                    if (long.TryParse(getal, out controle))
                    {

                        som = 3 * int.Parse(getal[0].ToString()) +
                         2 * int.Parse(getal[1].ToString()) +
                        7 * int.Parse(getal[2].ToString()) +
                        6 * int.Parse(getal[3].ToString()) +
                        5 * int.Parse(getal[4].ToString()) +
                        4 * int.Parse(getal[5].ToString()) +
                        3 * int.Parse(getal[6].ToString()) +
                        2 * int.Parse(getal[7].ToString());
                        var result = Math.Abs((som % 11) - 9).ToString();
                        return result;
                    }
                }
            }
            else if (pAniAlternateNumber.StartsWith("LU"))
            {
                getal = pAniAlternateNumber.Replace("LU", "").Trim();
                if (getal.Length == 8)
                {
                    long controle = 0;
                    if (long.TryParse(getal, out controle))
                    {
                        som = 3 * int.Parse(getal[0].ToString()) +
                         2 * int.Parse(getal[1].ToString()) +
                        7 * int.Parse(getal[2].ToString()) +
                        6 * int.Parse(getal[3].ToString()) +
                        5 * int.Parse(getal[4].ToString()) +
                        4 * int.Parse(getal[5].ToString()) +
                        3 * int.Parse(getal[6].ToString()) +
                        2 * int.Parse(getal[7].ToString());
                        var result = Math.Abs((som % 11) - 9).ToString();
                        return result;
                    }
                }
            }
            else if (pAniAlternateNumber.StartsWith("NL"))
            {
                getal = pAniAlternateNumber.Replace("NL", "").Trim();
                if (getal.Length == 8)
                {
                    long controle = 0;
                    if (long.TryParse(getal, out controle))
                    {
                        _log.Info($@"9*{getal[0].ToString()}  + 3*{getal[1].ToString()}+ 1*{getal[2].ToString()}+ 7*{getal[3].ToString()}+ 9*{getal[4].ToString()}+ 3*{getal[5].ToString()}+ 1*{getal[6].ToString()}+ 7*{getal[7].ToString()}");

                        som = (9 * int.Parse(getal[0].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (3 * int.Parse(getal[1].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (1 * int.Parse(getal[2].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (7 * int.Parse(getal[3].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (9 * int.Parse(getal[4].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (3 * int.Parse(getal[5].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (1 * int.Parse(getal[6].ToString()));
                        _log.Info($@"som:{som} ");
                        som = som + (7 * int.Parse(getal[7].ToString()));
                        _log.Info($@"som:{som} ");
                        var result = (som % 10).ToString();
                        _log.Info($@"result:{result} ");
                        return result;
                    }
                }
            }
            return "";
        }

        internal static int getMeldingtype(string nOTP_CDE)
        {
            switch (nOTP_CDE)
            {
                case "ARBTH":
                    return 1;
                case "ARIMO":
                    return 2;
                case "MMARV":
                    return 3;
                case "MMDPR":
                    return 4;
                case "MMDPS":
                    return 5;
                case "MMDPD":
                    return 6;
                case "MMEPO":
                    return 7;
                default:
                    return 0;
            }
        }

    }
}
