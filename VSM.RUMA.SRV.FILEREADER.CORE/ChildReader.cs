using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSM.RUMA.CORE;

namespace VSM.RUMA.SRV.FILEREADER.CORE
{
    [Serializable()]
    public class ChildReader : CodeActivity<Int32> 
    {

        public ChildReader()
        {
            Console.Write("ChildReader Constructor");
        }

        public InArgument<String> Filename { get; set; }
        public InArgument<String> Module { get; set; }
        public InArgument<String> Programid { get; set; }
        public InArgument<String> FileLogId { get; set; }


        protected override Int32 Execute(CodeActivityContext context)
        {

            string pPlugin;
            int result = -10;
            try
            {
                string pFilename = Filename.Get(context);
                pPlugin = Module.Get(context);
                int Program = Convert.ToInt32(Programid.Get(context));
                int FileLogid = Convert.ToInt32(FileLogId.Get(context));             

                log4net.GlobalContext.Properties["AppName"] = pPlugin.Replace("VSM.RUMA.", "").Replace("CORE.", "") + "_" + System.IO.Path.GetFileName(pFilename);
                unLogger.Configure("log4net.config", unRechten.getDBHost());
                unLogger.WriteInfo("Filename            : " + pFilename);
                unLogger.WriteInfo("Module              : " + pPlugin);
                unLogger.WriteInfo("Programid           : " + Program);
                unLogger.WriteInfo("Hostname  : " + getDBHost("Live server!"));
                unServiceRechten servicerechten = new unServiceRechten();
                var mAgrofactuurToken = servicerechten.GetToken();
                servicerechten.VeranderAdministratie(mAgrofactuurToken, Program);
                unLogger.WriteDebug("Loading : " + pPlugin);
                IReaderPlugin reader = PluginLoader.LoadPluginDLL(mAgrofactuurToken, pPlugin);
                result = reader.LeesFile(-99, mAgrofactuurToken, Program, "00000", "kcD0QJC3", FileLogid, pFilename);
            }
            catch (Exception ex)
            {
                pPlugin = "Unknown";
                VSM.RUMA.CORE.unLogger.WriteError(ex.Message, ex);
            }

            //if (result < 1)
            //    Environment.ExitCode = 1;

            int exitCode = 0;

            if (result < 0)
                exitCode = 3; //ERROR
            else if (result == -99)
            {

            }
            else
                exitCode = result; //logResult code
            unLogger.WriteInfoFormat("ChildReader", "{0} ReaderResult : {1}", pPlugin, exitCode);
            return exitCode;
        }


        private static string getDBHost(String Default)
        {
            String dbhost = unRechten.getDBHost();
            if (dbhost == string.Empty)
                dbhost = Default;
            return dbhost;
        }
    }
}
