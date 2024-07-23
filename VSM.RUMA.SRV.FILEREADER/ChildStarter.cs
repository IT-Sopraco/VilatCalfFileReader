using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSM.RUMA.CORE.COMMONS;
using VSM.RUMA.CORE;
using System.Threading;

namespace VSM.RUMA.SRV.FILEREADER
{
    public class ChildStarter
    {

        //static readonly object padlock = new object();

        //public Semaphore GetChildSemaphore()
        //{
        //    lock (padlock)
        //    {
        //        try
        //        {
        //            return Semaphore.OpenExisting("FilereaderChild");

        //        }
        //        catch (Exception ex)
        //        {

        //            unLogger.WriteDebug("GetChildSemaphore (" + Environment.ProcessorCount + ")" +ex.Message);
        //            return new Semaphore(Environment.ProcessorCount, Environment.ProcessorCount, "FilereaderChild");
        //        }
        //    }
        //}


        public int LeesFile(IReaderPlugin Plugin, int pProgramid, string Bestandsnaam, DBConnectionToken pAgroFactuurToken, int FileLogId)
        {

            int Result = -1;
            String ChildProcess = "FilereaderChild.exe";
            if (System.IO.File.Exists(ChildProcess))
            {
                
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                //unLogger.WriteInfo("GetChildSemaphore");
                //Semaphore childlock = GetChildSemaphore();
                try
                {
                    //unLogger.WriteInfo("Waiting to Spawn ChildProcess for file : " + Bestandsnaam);
                    //childlock.WaitOne();
                    unLogger.WriteInfo("Spawning ChildProcess for file : " + Bestandsnaam);
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = ChildProcess;
                    unLogger.WriteDebug("CurrentDirectory :" + System.IO.Directory.GetCurrentDirectory());
                    unLogger.WriteDebug("BaseDirectory    : " + AppDomain.CurrentDomain.BaseDirectory);
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
                    unLogger.WriteDebug(ex.Message, ex);
                    Result = -2;
                }
                finally
                {
                    if (!p.HasExited)
                    {
                        Result = -3;
                        p.Kill();
                    }
                    //childlock.Release();
                    unLogger.WriteInfo("Semaphore Released! " + Bestandsnaam);
                }
            }
            else
            {
                unLogger.WriteInfo("FilereaderChild.exe not found!");
            }

            return Result;
        }
    }
}
