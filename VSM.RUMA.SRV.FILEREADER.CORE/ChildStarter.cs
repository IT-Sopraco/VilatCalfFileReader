using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE;
using System.Threading;
using System.IO;

namespace VSM.RUMA.SRV.FILEREADER
{
    public class ChildStarter
    {


        public int LeesFile(IReaderPlugin Plugin, int pProgramid, string Bestandsnaam, DBConnectionToken pAgroFactuurToken, int FileLogId)
        {
            string lPrefix = $"{nameof(ChildStarter)}.{nameof(LeesFile)} -";

            int Result = -1;
            string childProcess = "FilereaderChild.exe";

            if (File.Exists(childProcess))
            {
                
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                try
                {
                    unLogger.WriteInfo($"{lPrefix} Spawning ChildProcess for file: {Bestandsnaam}.");

                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = childProcess;

                    //unLogger.WriteDebug("CurrentDirectory :" + System.IO.Directory.GetCurrentDirectory());
                    //unLogger.WriteDebug("BaseDirectory    : " + AppDomain.CurrentDomain.BaseDirectory);

                    p.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    StringBuilder SBArgs = new StringBuilder();
                    SBArgs.AppendFormat(" -Module \"{0}\" ", Plugin.GetType().FullName);
                    SBArgs.AppendFormat(" -Programid \"{0}\" ", pProgramid);
                    SBArgs.AppendFormat(" -Filename \"{0}\" ", Bestandsnaam);
                    SBArgs.AppendFormat(" -FileLogId \"{0}\" ", FileLogId);
                    p.StartInfo.Arguments = SBArgs.ToString();
                    p.Start();
                    p.WaitForExit();
                    Result = p.ExitCode;
     
                }
                catch (Exception ex)
                {
                    string msg = $"{lPrefix} Ex: {ex.Message}";
                    unLogger.WriteError(msg);
                    unLogger.WriteDebug(msg, ex);

                    Result = -2;
                }
                finally
                {
                    if (!p.HasExited)
                    {
                        Result = -3;
                        p.Kill();
                    }
                }
            }
            else
            {
                unLogger.WriteError($"{lPrefix} ChildProcess: '{childProcess}' Not found.");
            }

            return Result;
        }
    }
}
