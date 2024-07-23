using System;
using System.IO;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Web;


namespace VSM.RUMA.CORE.COMMONS
{


    [AttributeUsage(AttributeTargets.Method)]
    public class TextSoapExtensionAttribute : SoapExtensionAttribute
    {
        private int priority;

        public override Type ExtensionType
        {
            get { return typeof(TextSoapExtension); }
        }

        public override int Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }

    public class TextSoapExtension : SoapExtension
    {

        Stream oldStream;
        Stream newStream;

        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        public override void ProcessMessage(SoapMessage message)
        {

            String ThreadName = GetThreadName();
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    //message
                    break;
                case SoapMessageStage.AfterSerialize:
                    WriteToLog(String.Format(GetLogDir() + "req-{0}-{1}-{2}.txt", message.Action, DateTime.Now.ToString("yyyyMMddHHmmss"), ThreadName), message.Stream);
                    Copy(newStream, oldStream);
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    Copy(oldStream, newStream);
                    WriteToLog(String.Format(GetLogDir() + "resp-{0}-{1}-{2}.txt", message.Action, DateTime.Now.ToString("yyyyMMddHHmmss"), ThreadName), message.Stream);
                    break;
            }
        }

        private static String GetThreadName()
        {
            try
            {
                String ThreadName = Thread.CurrentThread.Name;
                if (String.IsNullOrEmpty(ThreadName))
                {
                    if (HttpContext.Current != null)
                    {
                        ThreadName = HttpContext.Current.Request.UserHostName;
                        if (ThreadName == String.Empty || ThreadName == null)
                        {
                            ThreadName = HttpContext.Current.Request.UserHostAddress;
                            if (ThreadName == String.Empty || ThreadName == null)
                            {
                                ThreadName = Thread.CurrentThread.ManagedThreadId.ToString();
                            }
                        }
                    }
                    else
                    {
                        ThreadName = Thread.CurrentThread.ManagedThreadId.ToString();
                    }
                }
                return ThreadName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fout bij GetThreadName " + ex.Message, ex);
                VSM.RUMA.CORE.unLogger.WriteError("Fout bij GetThreadName " + ex.Message, ex);
                return Guid.NewGuid().ToString();
            }
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override object GetInitializer(Type WebServiceType)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
        }


        void Copy(Stream from, Stream to)
        {
            //from.Position = 0;
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
        private String GetLogDir()
        {
            try
            {
                return VSM.RUMA.CORE.unLogger.getLogDir("SOAP");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fout bij ophalen logDir" + ex.Message, ex);
 
                return System.IO.Directory.GetCurrentDirectory();
            }

        }


        private void WriteToLog(String Filename, Stream logStream)
        {
            //bool lTestServer = configHelper.UseCRDTestserver;
            if (!File.Exists("c:\\helpdesk.dat") && !File.Exists("c:\\josser.dat") && !File.Exists("logsoap.dat"))// && !lTestServer)
            {
                logStream.Position = 0;
                return;
            }

            try
            {
                logStream.Position = 0;
                Stream FW = new System.IO.FileStream(Filename, System.IO.FileMode.Create);
                System.IO.TextWriter SW = new StreamWriter(FW);
                TextReader reader = new StreamReader(logStream);
                SW.WriteLine(reader.ReadToEnd());
                logStream.Position = 0;
                SW.Flush();
                SW.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fout bij Wegschrijven Logs" + ex.Message, ex);
                VSM.RUMA.CORE.unLogger.WriteError("Fout bij Wegschrijven Logs" + ex.Message, ex);

            }
        }
    }
}
